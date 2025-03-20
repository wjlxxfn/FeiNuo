using FeiNuo.Admin.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Admin.Services.System
{
    /// <summary>
    /// 服务类：菜单
    /// </summary>
    public class MenuService : BaseService<MenuEntity>
    {
        #region 构造函数
        protected readonly FNDbContext ctx;
        public MenuService(FNDbContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region 数据查询 
        /// <summary>
        /// 查询菜单树
        /// </summary>
        public async Task<List<MenuDto>> GetMenuTree()
        {
            var menus = await ctx.Menus.AsNoTracking().ToListAsync();
            // 生成树形结构
            var menuTree = BuildMenuTree(menus);
            return menuTree.Adapt<List<MenuDto>>();
        }

        // 根据菜单集合生成树形结构
        private static List<MenuEntity> BuildMenuTree(List<MenuEntity> menus)
        {
            var tree = new List<MenuEntity>();
            foreach (var menu in menus)
            {
                if (menu.ParentId == null)
                {
                    tree.Add(menu);
                }
                else
                {
                    var parent = menus.FirstOrDefault(a => a.MenuId == menu.ParentId);
                    if (parent == null)
                    {
                        tree.Add(menu);
                    }
                    else
                    {
                        parent.Children ??= [];
                        parent.Children.Add(menu);
                    }
                }
            }
            return tree;
        }

        /// <summary>
        /// 根据菜单ID查询
        /// </summary>
        public async Task<MenuDto> GetMenu(int menuId)
        {
            var entity = await FindByIdAsync(menuId);
            return entity.Adapt<MenuDto>();
        }

        private async Task<MenuEntity> FindByIdAsync(int menuId)
        {
            return await ctx.Menus.FindAsync(menuId) ?? throw new NotFoundException($"找不到指定数据,Id:{menuId},Type:{typeof(MenuEntity)}");
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增菜单
        /// </summary>
        public async Task<MenuDto> CreateMenu(MenuDto dto, LoginUser user)
        {
            // 新建对象
            var entity = new MenuEntity();
            // 复制属性
            await CopyDtoToEntity(dto, entity, user, true);
            // 执行保存
            ctx.Menus.Add(entity);
            await ctx.SaveChangesAsync();
            // 返回Dto
            return entity.Adapt<MenuDto>();
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        public async Task UpdateMenu(MenuDto dto, LoginUser user)
        {
            // 原始数据
            var entity = await FindByIdAsync(dto.MenuId);
            // 更新字段
            await CopyDtoToEntity(dto, entity, user, false);
            // 执行更新
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 复制dto属性到实体字段
        /// </summary>
        private async Task CopyDtoToEntity(MenuDto dto, MenuEntity entity, LoginUser user, bool isNew)
        {
            // 检查数据重复
            if (await ctx.Menus.AnyAsync(a => a.MenuId != dto.MenuId && (a.ParentId == dto.ParentId && a.MenuName == dto.MenuName)))
            {
                throw new MessageException("当前已存在相同名称的菜单");
            }

            // 复制属性
            dto.Adapt(entity);

            // 记录操作人
            entity.AddOperator(user.Username, isNew);
        }

        /// <summary>
        /// 根据ID删除菜单
        /// </summary>
        public async Task DeleteMenuByIds(IEnumerable<int> ids, LoginUser user)
        {
            var menus = await ctx.Menus.Include(a => a.Children).Include(a => a.Roles).Where(a => ids.Contains(a.MenuId)).ToListAsync();
            if (menus.Count != ids.Count())
            {
                throw new MessageException("查询出的数据和传入的ID不匹配，请刷新后再试。");
            }
            foreach (var menu in menus.OrderByDescending(a => a.MenuId))
            {
                if (menu.Children.Count > 0)
                {
                    throw new MessageException("请先删除子菜单");
                }
                menu.Roles.Clear();
                ctx.Menus.Remove(menu);
            }
            // 提交事务
            await ctx.SaveChangesAsync();
        }
        #endregion
    }
}