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
        public DeptService(FNDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region 数据查询 
        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PageResult<DeptDto>> FindPagedList(DeptQuery query, Pager pager, LoginUser user)
        {
            var lstData = await FindPagedList(query, pager, o => o.OrderByDescending(t => t.CreateTime));
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
            // 更新字段
            await CopyDtoToEntity(dto, entity, user, false);
            // 执行更新
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 复制dto属性到实体字段
        /// </summary>
        private async Task CopyDtoToEntity(DeptDto dto, DeptEntity entity, LoginUser user, bool isNew)
        {
            //TODO 检查数据重复
            if (await ctx.Depts.AnyAsync(a => a.DeptId != dto.DeptId /* && (a.Name == dto.Name )*/))
            {
                throw new MessageException("当前已存在相同名称或编码的部门");
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
            var depts = await ctx.Depts.Where(a => ids.Contains(a.DeptId)).ToListAsync();
            if (depts.Count != ids.Count())
            {
                throw new MessageException("查询出的数据和传入的ID不匹配，请刷新后再试。");
            }
            foreach (var dept in depts)
            {
                //TODO 判断是否能删除
                ctx.Depts.Remove(dept);
            }
            // 提交事务
            await ctx.SaveChangesAsync();
        }
        #endregion
    }
}