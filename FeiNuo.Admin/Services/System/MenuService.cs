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
            var menus = await ctx.Menus.OrderBy(a => a.SortNo).ToListAsync();
            return [.. menus.Where(a => a.Parent == null).Select(a => a.Adapt<MenuDto>())];
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PageResult<MenuDto>> FindPagedList(MenuQuery query, Pager pager, LoginUser user)
        {
            var dbSet = ctx.Menus.AsNoTracking().Where(query.GetQueryExpression());
            var lstData = await PageHelper.FindPagedList(dbSet, pager, o => o.OrderByDescending(t => t.CreateTime));
            return lstData.Map(o => o.Adapt<MenuDto>());
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
            //TODO 检查数据重复
            if (await ctx.Menus.AnyAsync(a => a.MenuId != dto.MenuId /* && (a.Name == dto.Name )*/))
            {
                throw new MessageException("当前已存在相同名称或编码的菜单");
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
            var menus = await ctx.Menus.Where(a => ids.Contains(a.MenuId)).ToListAsync();
            if (menus.Count != ids.Count())
            {
                throw new MessageException("查询出的数据和传入的ID不匹配，请刷新后再试。");
            }
            foreach (var menu in menus)
            {
                //TODO 判断是否能删除
                ctx.Menus.Remove(menu);
            }
            // 提交事务
            await ctx.SaveChangesAsync();
        }
        #endregion
    }
}