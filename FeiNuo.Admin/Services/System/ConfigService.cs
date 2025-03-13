using FeiNuo.Admin.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FeiNuo.Admin.Services.System
{
    /// <summary>
    /// 服务类：参数配置
    /// </summary>
    public class ConfigService : BaseService<ConfigEntity>
    {
        #region 构造函数
        protected readonly FNDbContext ctx;
        private readonly IMemoryCache cache;
        public ConfigService(FNDbContext ctx, IMemoryCache cache)
        {
            this.ctx = ctx;
            this.cache = cache;
        }
        #endregion

        #region 数据查询 
        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PageResult<ConfigDto>> FindPagedList(ConfigQuery query, Pager pager, LoginUser user)
        {
            var dbSet = ctx.Configs.AsNoTracking().Where(query.GetQueryExpression());
            var lstData = await PageHelper.FindPagedList(dbSet, pager, o => o.OrderByDescending(t => t.CreateTime));
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

        private async Task<ConfigEntity> FindByIdAsync(int configId)
        {
            return await ctx.Configs.FindAsync(configId) ?? throw new NotFoundException($"找不到指定数据,Id:{configId},Type:{typeof(ConfigEntity)}");
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
            // 清除缓存
            RemoveCache(entity.ConfigCode);
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
            // 清除缓存
            RemoveCache(entity.ConfigCode);
        }

        /// <summary>
        /// 复制dto属性到实体字段
        /// </summary>
        private async Task CopyDtoToEntity(ConfigDto dto, ConfigEntity entity, LoginUser user, bool isNew)
        {
            // 检查数据重复
            if (await ctx.Configs.AnyAsync(a => a.ConfigId != dto.ConfigId && (a.ConfigName == dto.ConfigName || a.ConfigCode == dto.ConfigCode)))
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
            var configs = await ctx.Configs.Where(a => ids.Contains(a.ConfigId)).ToListAsync();
            if (configs.Count != ids.Count())
            {
                throw new MessageException("查询出的数据和传入的ID不匹配，请刷新后再试。");
            }
            foreach (var config in configs)
            {
                // 清除缓存
                RemoveCache(config.ConfigCode);
                ctx.Configs.Remove(config);
            }
            // 提交事务
            await ctx.SaveChangesAsync();
        }


        private void RemoveCache(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                cache.Remove(AppConstants.CACHE_PREFIX_CONFIG + key);
            }
        }
        #endregion

        #region 查询配置值
        public async Task<ConfigDto?> GetConfigByCode(string configCode)
        {
            return await cache.GetOrCreateAsync(AppConstants.CACHE_PREFIX_CONFIG + configCode, async o =>
            {
                o.SlidingExpiration = TimeSpan.FromHours(4);
                var cfg = await ctx.Configs.SingleOrDefaultAsync(a => a.ConfigCode == configCode);
                return cfg?.Adapt<ConfigDto>();
            });
        }
        public async Task<string> GetConfigValue(string configCode, string defaultValue = "")
        {
            var dto = await GetConfigByCode(configCode);
            return null == dto ? defaultValue : dto.ConfigValue;
        }
        public async Task<int> GetConfigValueInteger(string configCode, int defaultValue = 0)
        {
            var dto = await GetConfigByCode(configCode);
            return (null == dto || string.IsNullOrWhiteSpace(dto.ConfigValue)) ? defaultValue : int.Parse(dto.ConfigValue);
        }
        public async Task<decimal> GetConfigValueDecimal(string configCode, decimal defaultValue = 0)
        {
            var dto = await GetConfigByCode(configCode);
            return (null == dto || string.IsNullOrWhiteSpace(dto.ConfigValue)) ? defaultValue : decimal.Parse(dto.ConfigValue);
        }
        public async Task<bool> GetConfigValueBoolean(string configCode, bool defaultValue = false)
        {
            var dto = await GetConfigByCode(configCode);
            if (null == dto || string.IsNullOrWhiteSpace(dto.ConfigValue)) return defaultValue;
            return dto.ConfigValue == "1" || dto.ConfigValue.Trim().Equals("true", StringComparison.CurrentCultureIgnoreCase);
        }
        #endregion
    }
}