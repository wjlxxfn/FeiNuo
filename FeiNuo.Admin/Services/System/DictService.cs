using FeiNuo.Admin.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FeiNuo.Admin.Services.System
{
    /// <summary>
    /// 服务类：数据字典
    /// </summary>
    public class DictService : BaseService<DictEntity>
    {
        #region 构造函数
        protected readonly FNDbContext ctx;
        public DictService(FNDbContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion

        #region 数据查询 
        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PageResult<DictDto>> FindPagedList(DictQuery query, Pager pager, LoginUser user, bool includeItems = false)
        {
            var dbSet = ctx.Dicts.AsNoTracking().Where(query.GetQueryExpression());
            if (includeItems) dbSet = dbSet.Include(a => a.DictItems);

            var lstData = await PageHelper.FindPagedList(dbSet, pager, o => o.OrderByDescending(t => t.CreateTime));
            return lstData.Map(o => o.Adapt<DictDto>());
        }

        /// <summary>
        /// 根据数据字典ID查询
        /// </summary>
        public async Task<DictDto> GetDict(int dictId)
        {
            var entity = await FindByIdAsync(dictId);
            return entity.Adapt<DictDto>();
        }

        private async Task<DictEntity> FindByIdAsync(int dictId)
        {
            return await ctx.Dicts.Include(a => a.DictItems).FirstAsync(a => a.DictId == dictId) ?? throw new NotFoundException($"找不到指定数据,Id:{dictId},Type:{typeof(DictEntity)}");
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增数据字典
        /// </summary>
        public async Task<DictDto> CreateDict(DictDto dto, LoginUser user)
        {
            // 新建对象
            var entity = new DictEntity();
            // 复制属性
            await CopyDtoToEntity(dto, entity, user, true);
            // 执行保存
            ctx.Dicts.Add(entity);
            await ctx.SaveChangesAsync();
            // 返回Dto
            return entity.Adapt<DictDto>();
        }

        /// <summary>
        /// 修改数据字典
        /// </summary>
        public async Task UpdateDict(DictDto dto, LoginUser user)
        {
            // 原始数据
            var entity = await FindByIdAsync(dto.DictId);
            // 更新字段
            await CopyDtoToEntity(dto, entity, user, false);
            // 执行更新
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// 复制dto属性到实体字段
        /// </summary>
        private async Task CopyDtoToEntity(DictDto dto, DictEntity entity, LoginUser user, bool isNew)
        {
            // 检查数据重复
            if (await ctx.Dicts.AnyAsync(a => a.DictId != dto.DictId && (a.DictType == dto.DictType || a.DictName == dto.DictName)))
            {
                throw new MessageException("当前已存在相同名称或类型的数据字典");
            }

            // 复制属性（不含明细）
            dto.Adapt(entity);

            // 明细数据
            if (isNew)
            {
                entity.DictItems = dto.DictItems.Adapt<List<DictItemEntity>>();
            }
            else
            {
                // 明细数据
                var existingItems = entity.DictItems.ToList();
                var updatedItems = dto.DictItems.ToList();

                // 删除不存在的明细
                foreach (var existingItem in existingItems)
                {
                    if (!updatedItems.Any(u => u.DictItemId == existingItem.DictItemId))
                    {
                        ctx.DictItems.Remove(existingItem);
                    }
                }

                // 更新或新增明细
                foreach (var updatedItem in updatedItems)
                {
                    var existingItem = existingItems.FirstOrDefault(e => e.DictItemId == updatedItem.DictItemId);
                    if (existingItem != null)
                    {
                        // 更新现有明细
                        updatedItem.Adapt(existingItem);
                    }
                    else
                    {
                        // 新增明细
                        var newItem = updatedItem.Adapt<DictItemEntity>();
                        entity.DictItems.Add(newItem);
                    }
                }
            }

            // 记录操作人
            entity.AddOperator(user.Username, isNew);
        }

        /// <summary>
        /// 根据ID删除数据字典
        /// </summary>
        public async Task DeleteDictByIds(IEnumerable<int> ids, LoginUser user)
        {
            var dicts = await ctx.Dicts.Include(a => a.DictItems).Where(a => ids.Contains(a.DictId)).ToListAsync();
            if (dicts.Count != ids.Count())
            {
                throw new MessageException("查询出的数据和传入的ID不匹配，请刷新后再试。");
            }
            foreach (var dict in dicts)
            {
                dict.DictItems.Clear();
                ctx.Dicts.Remove(dict);
            }
            // 提交事务
            await ctx.SaveChangesAsync();
        }
        #endregion
    }
}