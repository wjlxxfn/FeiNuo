using FeiNuo.Admin.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Admin.Services.System
{
    /// <summary>
    /// 服务类：部门
    /// </summary>
    public class DeptService : BaseService<DeptEntity>
    {
        #region 构造函数
        protected readonly FNDbContext ctx;
        public DeptService(FNDbContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region 数据查询 
        public async Task<IEnumerable<DeptDto>> GetDeptTree(DeptQuery query)
        {
            var parentId = query.ParentId;
            query.ParentId = null;
            var lstData = await ctx.Depts.Where(query.GetQueryExpression()).ToListAsync();

            var lstTree = parentId.HasValue
                ? lstData.Where(a => a.DeptId == parentId.Value)
                : lstData.Where(a => a.Parent == null);

            return lstTree.OrderBy(a => a.Status).ThenBy(a => a.SortNo).Select(a => a.Adapt<DeptDto>());
        }


        /// <summary>
        /// 获取通用的部门树，返回TreeOption，通常用于下拉选项
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="includeDisabled"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TreeOption>> GetDeptTree(int? deptId, bool includeDisabled = false)
        {
            var depts = await ctx.Depts.Where(a => includeDisabled || a.Status == ((int)StatusEnum.Normal)).ToListAsync();
            var lstTree = deptId.HasValue
               ? depts.Where(a => a.DeptId == deptId.Value)
               : depts.Where(a => a.Parent == null);
            return lstTree.OrderBy(a => a.Status).ThenBy(a => a.SortNo).Select(a => a.Adapt<TreeOption>());
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PageResult<DeptDto>> FindPagedList(DeptQuery query, Pager pager, LoginUser user)
        {
            var dbSet = ctx.Depts.AsNoTracking().Where(query.GetQueryExpression());
            var lstData = await PageHelper.FindPagedList(dbSet, pager, o => o.OrderBy(a => a.ParentId).ThenBy(a => a.SortNo));
            return lstData.Map(o => o.Adapt<DeptDto>());
        }

        /// <summary>
        /// 根据部门ID查询
        /// </summary>
        public async Task<DeptDto> GetDept(int deptId)
        {
            var entity = await FindByIdAsync(deptId);
            return entity.Adapt<DeptDto>();
        }

        private async Task<DeptEntity> FindByIdAsync(int deptId)
        {
            return await ctx.Depts.FindAsync(deptId) ?? throw new NotFoundException($"找不到指定数据,Id:{deptId},Type:{typeof(DeptEntity)}");
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增部门
        /// </summary>
        public async Task<DeptDto> CreateDept(DeptDto dto, LoginUser user)
        {
            // 新建对象
            var entity = new DeptEntity();
            // 复制属性
            await CopyDtoToEntity(dto, entity, user, true);
            // 执行保存
            ctx.Depts.Add(entity);
            await ctx.SaveChangesAsync();
            // 返回Dto
            return entity.Adapt<DeptDto>();
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        public async Task UpdateDept(DeptDto dto, LoginUser user)
        {
            // 原始数据
            var entity = await FindByIdAsync(dto.DeptId);
            var changeStatus = entity.Status != ((int)dto.Status);
            // 更新字段
            await CopyDtoToEntity(dto, entity, user, false);
            // 更新状态
            if (changeStatus)
            {
                await UpdateDeptStatus(dto.DeptId, dto.Status);
            }
            // 执行更新
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 复制dto属性到实体字段
        /// </summary>
        private async Task CopyDtoToEntity(DeptDto dto, DeptEntity entity, LoginUser user, bool isNew)
        {
            // 检查数据重复
            if (await ctx.Depts.AnyAsync(a => a.DeptId != dto.DeptId && a.ParentId == dto.ParentId && (a.DeptName == dto.DeptName)))
            {
                throw new MessageException("同一层级下已存在相同名称的部门");
            }

            // 复制属性
            dto.Adapt(entity);

            // 记录操作人
            entity.AddOperator(user.Username, isNew);
        }

        /// <summary>
        /// 根据ID删除部门
        /// </summary>
        public async Task DeleteDeptByIds(IEnumerable<int> ids, LoginUser user)
        {
            var depts = await ctx.Depts.Include(a => a.Children).Include(a => a.Users).Where(a => ids.Contains(a.DeptId)).ToListAsync();
            if (depts.Count != ids.Count())
            {
                throw new MessageException("查询出的数据和传入的ID不匹配，请刷新后再试。");
            }
            foreach (var dept in depts)
            {
                // 判断是否能删除
                if (dept.Users.Count > 0)
                {
                    throw new MessageException($"部门【{dept.DeptName}】下存在用户，不允许删除。");
                }
                if (dept.Children.Count > 0)
                {
                    throw new MessageException($"部门【{dept.DeptName}】下存在子部门，不允许删除。");
                }
                ctx.Depts.Remove(dept);
            }
            // 提交事务
            await ctx.SaveChangesAsync();
        }


        #endregion

        #region 修改部门状态
        /// <summary>
        /// 修改部门状态
        /// </summary>
        public async Task UpdateDeptStatus(int deptId, StatusEnum status)
        {
            if (deptId == 1) throw new MessageException("不允许调整根节点状态");

            // 作废的要把所有下级一起作废，启用的要把所有上级一起启用
            var depts = status == StatusEnum.Normal
                ? await SelectDeptParents(deptId)
                : await SelectDeptChildren(deptId);

            foreach (var dept in depts)
            {
                dept.Status = ((byte)status);
            }
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 根据部门ID查当前部门及所有下级子部门
        /// </summary>
        private async Task<List<DeptEntity>> SelectDeptChildren(int deptId, bool includeSelf = true, bool noTracking = false)
        {
            var sql = $@"WITH RECURSIVE T AS ( 
                    SELECT *  FROM sys_dept WHERE dept_id = {deptId}
                    UNION ALL
                    SELECT A.* FROM sys_dept AS A INNER JOIN T
                     ON A.parent_id = T.dept_id
                ) SELECT * FROM T
            ";
            var query = ctx.Depts.FromSqlRaw(sql);
            if (noTracking) query = query.AsNoTracking();
            var lstData = await query.ToListAsync();

            return [.. lstData.Where(a => includeSelf || a.DeptId != deptId)];
        }

        /// <summary>
        /// 查询指定科室的所有上级科室
        /// </summary>
        private async Task<List<DeptEntity>> SelectDeptParents(int deptId)
        {
            var sql = $@"WITH RECURSIVE T AS ( 
                    SELECT *  FROM sys_dept WHERE dept_id = {deptId}
                    UNION ALL
                    SELECT A.* FROM sys_dept AS A INNER JOIN T
                     ON A.dept_id = T.parent_id
                ) SELECT * FROM T
            ";
            return await ctx.Depts.FromSqlRaw(sql).ToListAsync();
        }
        #endregion
    }
}