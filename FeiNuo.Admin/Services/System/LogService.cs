using FeiNuo.Admin.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Admin.Services.System
{
    /// <summary>
    /// 服务类：操作日志
    /// </summary>
    public class LogService : BaseService<LogEntity>
    {
        #region 构造函数
        protected readonly FNDbContext ctx;
        public LogService(FNDbContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region 数据查询 
        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PageResult<LogDto>> FindPagedList(LogQuery query, Pager pager, LoginUser user)
        {
            var dbSet = ctx.Logs.AsNoTracking().Where(query.GetQueryExpression());
            var lstData = await PageHelper.FindPagedList(dbSet, pager, o => o.OrderBy(a => a.CreateTime));
            return lstData.Map(o => o.Adapt<LogDto>());
        }

        /// <summary>
        /// 根据操作日志ID查询
        /// </summary>
        public async Task<LogDto> GetLog(long logId)
        {
            var entity = await ctx.Logs.FindAsync(logId);
            return entity.Adapt<LogDto>();
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增操作日志
        /// </summary>
        public async Task<LogDto> CreateLog(LogDto dto, LoginUser user)
        {
            // 新建对象
            var entity = new LogEntity();
            // 复制属性
            await CopyDtoToEntity(dto, entity, user, true);
            // 执行保存
            ctx.Logs.Add(entity);
            await ctx.SaveChangesAsync();
            // 返回Dto
            return entity.Adapt<LogDto>();
        }

        /// <summary>
        /// 修改操作日志
        /// </summary>
        public async Task UpdateLog(LogDto dto, LoginUser user)
        {
            // 原始数据
            var entity = await ctx.Logs.FindAsync(dto.LogId);
            // 更新字段
            await CopyDtoToEntity(dto, entity, user, false);
            // 执行更新
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 复制dto属性到实体字段
        /// </summary>
        private async Task CopyDtoToEntity(LogDto dto, LogEntity entity, LoginUser user, bool isNew)
        {
            //TODO 检查数据重复
            if (await ctx.Logs.AnyAsync(a => a.LogId != dto.LogId /* && (a.Name == dto.Name )*/))
            {
                throw new MessageException("当前已存在相同名称或编码的操作日志");
            }

            // 复制属性
            dto.Adapt(entity);

            // 记录操作人
            entity.AddOperator(user.Username, isNew);
        }

        /// <summary>
        /// 根据ID删除操作日志
        /// </summary>
        public async Task DeleteLogByIds(IEnumerable<long> ids, LoginUser user)
        {
            var logs = await ctx.Logs.Where(a => ids.Contains(a.LogId)).ToListAsync();
            if (logs.Count != ids.Count())
            {
                throw new MessageException("查询出的数据和传入的ID不匹配，请刷新后再试。");
            }
            foreach (var log in logs)
            {
                //TODO 判断是否能删除
                ctx.Logs.Remove(log);
            }
            // 提交事务
            await ctx.SaveChangesAsync();
        }
        #endregion
    }
}