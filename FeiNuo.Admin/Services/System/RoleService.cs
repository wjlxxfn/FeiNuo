using FeiNuo.Admin.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Admin.Services.System
{
    /// <summary>
    /// 服务类：角色
    /// </summary>
    public class RoleService : BaseService<RoleEntity>
    {
        #region 构造函数
        protected readonly FNDbContext ctx;
        public RoleService(FNDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region 数据查询 
        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PageResult<RoleDto>> FindPagedList(RoleQuery query, Pager pager, LoginUser user)
        {
            if (!user.IsSuperAdmin)
            {
                query.AddExpression(a => a.RoleCode != AppConstants.SUPER_ADMIN);
            }
            var lstData = await FindPagedList(query, pager, o => o.OrderByDescending(t => t.CreateTime));
            return lstData.Map(o => o.Adapt<RoleDto>());
        }

        /// <summary>
        /// 根据角色ID查询
        /// </summary>
        public async Task<RoleDto> GetRole(int roleId)
        {
            var entity = await ctx.Roles.Include(a => a.Menus).SingleOrDefaultAsync(a => a.RoleId == roleId)
                ?? throw new NotFoundException("找不到角色:" + roleId);
            return entity.Adapt<RoleDto>();
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增角色
        /// </summary>
        public async Task<RoleDto> CreateRole(RoleDto dto, LoginUser user)
        {
            // 新建对象
            var entity = new RoleEntity();
            // 复制属性
            await CopyDtoToEntity(dto, entity, user, true);
            // 执行保存
            ctx.Roles.Add(entity);
            await ctx.SaveChangesAsync();
            // 返回Dto
            return entity.Adapt<RoleDto>();
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        public async Task UpdateRole(RoleDto dto, LoginUser user)
        {
            // 原始数据
            var entity = await FindByIdAsync(dto.RoleId);
            // 逻辑判断通用都使用编码，编码不让修改
            if (entity.RoleCode != dto.RoleCode)
            {
                throw new MessageException("不允许修改角色编码");
            }
            // 更新字段
            await CopyDtoToEntity(dto, entity, user, false);
            // 执行更新
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 复制dto属性到实体字段
        /// </summary>
        private async Task CopyDtoToEntity(RoleDto dto, RoleEntity entity, LoginUser user, bool isNew)
        {
            // 检查数据重复
            if (await ctx.Roles.AnyAsync(a => a.RoleId != dto.RoleId && (a.RoleName == dto.RoleName || a.RoleCode == dto.RoleCode)))
            {
                throw new MessageException("当前已存在相同名称或编码的角色");
            }

            // 复制属性
            dto.Adapt(entity);

            // 记录操作人
            entity.AddOperator(user.Username, isNew);
        }

        /// <summary>
        /// 根据ID删除角色
        /// </summary>
        public async Task DeleteRoleByIds(IEnumerable<int> ids, LoginUser user)
        {
            var roles = await ctx.Roles.Include(a => a.Users).Include(a => a.Menus).Where(a => ids.Contains(a.RoleId)).ToListAsync();
            if (roles.Count != ids.Count())
            {
                throw new MessageException("查询出的数据和传入的ID不匹配，请刷新后再试。");
            }
            foreach (var role in roles)
            {
                if (role.RoleCode == AppConstants.SUPER_ADMIN)
                {
                    throw new MessageException("不能删除超级管理员角色");
                }
                // 关联用户的不能删
                if (role.Users.Count != 0)
                {
                    throw new MessageException($"角色{role.RoleName}存在关联用户，不能删除", MessageType.Warning);
                }
                // 删除菜单关联关系
                role.Menus.Clear();
                ctx.Roles.Remove(role);
            }
            // 提交事务
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 修改角色状态
        /// </summary>
        public async Task UpdateRoleStatus(int roleId, bool disabled, LoginUser currentUser)
        {
            var role = await FindByIdAsync(roleId);
            if (role.RoleCode == AppConstants.SUPER_ADMIN)
            {
                throw new MessageException("不允许操作超级管理员角色");
            }
            role.Disabled = disabled;
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 修改角色权限
        /// </summary>
        public async Task UpdateRoleMenus(int roleId, IEnumerable<int> menuIds, LoginUser currentUser)
        {
            var role = await ctx.Roles.Include(a => a.Menus).SingleOrDefaultAsync(a => a.RoleId == roleId)
               ?? throw new Exception("找不到角色：" + roleId);
            if (role.RoleCode == AppConstants.SUPER_ADMIN)
            {
                throw new MessageException("不允许操作超级管理员角色");
            }
            role.Menus = await ctx.Menus.Where(a => menuIds.Contains(a.MenuId)).ToListAsync();
            await ctx.SaveChangesAsync();
        }
        #endregion
    }
}