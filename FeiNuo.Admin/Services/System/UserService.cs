using FeiNuo.Admin.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Admin.Services.System
{
    /// <summary>
    /// 服务类：用户
    /// </summary>
    public class UserService : BaseService<UserEntity>
    {
        #region 构造函数
        protected readonly FNDbContext ctx;
        public UserService(FNDbContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region 数据查询 
        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PageResult<UserDto>> FindPagedList(UserQuery query, Pager pager, LoginUser user)
        {
            var dbSet = ctx.Users.AsNoTracking().Where(query.GetQueryExpression());
            var lstData = await PageHelper.FindPagedList(dbSet, pager, o => o.OrderByDescending(t => t.CreateTime));
            return lstData.Map(o => o.Adapt<UserDto>());
        }

        /// <summary>
        /// 根据用户ID查询
        /// </summary>
        public async Task<UserDto> GetUser(int userId)
        {
            var entity = await FindByIdAsync(userId);
            return entity.Adapt<UserDto>();
        }
		
        private async Task<UserEntity> FindByIdAsync(int userId)
        {
            return await ctx.Users.FindAsync(userId) ?? throw new NotFoundException($"找不到指定数据,Id:{userId},Type:{typeof(UserEntity)}");
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增用户
        /// </summary>
        public async Task<UserDto> CreateUser(UserDto dto, LoginUser user)
        {
            // 新建对象
            var entity = new UserEntity();
            // 复制属性
            await CopyDtoToEntity(dto, entity, user, true);
            // 执行保存
            ctx.Users.Add(entity);
            await ctx.SaveChangesAsync();
            // 返回Dto
            return entity.Adapt<UserDto>();
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        public async Task UpdateUser(UserDto dto, LoginUser user)
        {
            // 原始数据
            var entity = await FindByIdAsync(dto.UserId);
            // 更新字段
            await CopyDtoToEntity(dto, entity, user, false);
            // 执行更新
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 复制dto属性到实体字段
        /// </summary>
        private async Task CopyDtoToEntity(UserDto dto, UserEntity entity, LoginUser user, bool isNew)
        {
            //TODO 检查数据重复
            if (await ctx.Users.AnyAsync(a => a.UserId != dto.UserId /* && (a.Name == dto.Name )*/))
            {
                throw new MessageException("当前已存在相同名称或编码的用户");
            }

            // 复制属性
            dto.Adapt(entity);

            // 记录操作人
            entity.AddOperator(user.Username, isNew);
        }

        /// <summary>
        /// 根据ID删除用户
        /// </summary>
        public async Task DeleteUserByIds(IEnumerable<int> ids, LoginUser user)
        {
            var users = await ctx.Users.Where(a => ids.Contains(a.UserId)).ToListAsync();
            if (users.Count != ids.Count())
            {
                throw new MessageException("查询出的数据和传入的ID不匹配，请刷新后再试。");
            }
            foreach (var u in users)
            {
                //TODO 判断是否能删除
                ctx.Users.Remove(u);
            }
            // 提交事务
            await ctx.SaveChangesAsync();
        }
        #endregion
    }
}