using FeiNuo.Admin.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Admin.Services.System
{
    /// <summary>
    /// 服务类：参数配置
    /// </summary>
    public class ConfigService : BaseService<ConfigEntity>
    {
        #region 构造函数
        protected readonly FNDbContext ctx;
        public ConfigService(FNDbContext ctx) : base(ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region 数据查询 
        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PageResult<ConfigDto>> FindPagedList(ConfigQuery query, Pager pager, LoginUser user)
        {
            var lstData = await FindPagedList(query, pager, o => o.OrderByDescending(t => t.CreateTime));
            return lstData.Map(o => o.Adapt<ConfigDto>());
        }

        /// <summary>
        /// 根据参数配置ID查询
        /// </summary>
        public async Task<ConfigDto> GetConfig(int configId)
        {
            var entity = await FindByIdAsync(configId);
            return entity.Adapt<ConfigDto>();
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增参数配置
        /// </summary>
        public async Task<ConfigDto> CreateConfig(ConfigDto dto, LoginUser user)
        {
            // 新建对象
            var entity = new ConfigEntity();
            // 复制属性
            await CopyDtoToEntity(dto, entity, user, true);
            // 执行保存
            ctx.Configs.Add(entity);
            await ctx.SaveChangesAsync();
            // 返回Dto
            return entity.Adapt<ConfigDto>();
        }

        /// <summary>
        /// 修改参数配置
        /// </summary>
        public async Task UpdateConfig(ConfigDto dto, LoginUser user)
        {
            // 原始数据
            var entity = await FindByIdAsync(dto.ConfigId);
            // 更新字段
            await CopyDtoToEntity(dto, entity, user, false);
            // 执行更新
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 复制dto属性到实体字段
        /// </summary>
        private async Task CopyDtoToEntity(ConfigDto dto, ConfigEntity entity, LoginUser user, bool isNew)
        {
            //TODO 检查数据重复
            if (await ctx.Configs.AnyAsync(a => a.ConfigId != dto.ConfigId /* && (a.Name == dto.Name )*/))
            {
                throw new MessageException("当前已存在相同名称或编码的参数配置");
            }

            // 复制属性
            dto.Adapt(entity);

            // 记录操作人
            entity.AddOperator(user.Username, isNew);
        }

        /// <summary>
        /// 根据ID删除参数配置
        /// </summary>
        public async Task DeleteConfigByIds(IEnumerable<int> ids, LoginUser user)
        {
            foreach (var id in ids)
            {
                var config = await ctx.Configs.FindAsync(id);
                if (config == null) throw new MessageException($"不存在【id={id}】的参数配置。");
                //TODO 判断是否能删除
                ctx.Configs.Remove(config);
            }
            // 提交事务
            await ctx.SaveChangesAsync();
        }
        #endregion
    }
}