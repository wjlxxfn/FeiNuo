using FeiNuo.Admin.Services.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeiNuo.Admin.Controllers.System
{
    /// <summary>
    /// API接口：数据字典
    /// </summary>
    [Authorize]
    [Route("api/system/[controller]")]
    public class DictController : BaseController
    {
        #region 构造函数
        private readonly DictService service;
        public DictController(DictService service)
        {
            this.service = service;
        }
        #endregion

        #region 查询导出 
        /// <summary>
        /// 分页查询
        /// </summary>
        [HttpGet]
        [EndpointSummary("分页查询")]
        public async Task<PageResult<DictDto>> GetDictList([FromQuery] DictQuery query, [FromQuery] Pager pager)
        {
            return await service.FindPagedList(query, pager, CurrentUser);
        }

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="dictId">主键ID</param>
        [HttpGet("{dictId}")]
        [EndpointSummary("主键查询")]
        public async Task<DictDto> GetDict(int dictId)
        {
            return await service.GetDict(dictId);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        [HttpGet("export")]
        [EndpointSummary("导出Excel")]
        public async Task<ActionResult> ExportDicts([FromQuery] DictQuery query)
        {
            var pager = await service.FindPagedList(query, Pager.Unpaged, CurrentUser);
            var excel = new ExcelConfig($"数据字典导出{DateTime.Now:yyyyMMddHHmmss}.xlsx", pager.DataList, [
                new ExcelColumn<DictDto>("字典类型", d => d.DictType),
                new ExcelColumn<DictDto>("字典名称", d => d.DictName),
                new ExcelColumn<DictDto>("备注说明", d => d.Remark),
                new ExcelColumn<DictDto>("创建人", d => d.CreateBy),
                new ExcelColumn<DictDto>("创建时间", d => d.CreateTime),
            ]);
            var bytes = PoiHelper.GetExcelBytes(excel);
            return File(bytes, excel.ContentType, excel.FileName);
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增数据字典
        /// </summary>
        [HttpPost]
        [EndpointSummary("新增数据字典")]
        [Log("新增数据字典", OperateType.Create, false, true)]
        public async Task<ActionResult<DictDto>> CreateDict([FromBody] DictDto dto)
        {
            dto = await service.CreateDict(dto, CurrentUser);
            return CreatedAtAction("GetDict", new { dictId = dto.DictId }, dto);
        }

        /// <summary>
        /// 修改数据字典
        /// </summary>
        [HttpPut("{id}")]
        [EndpointSummary("修改数据字典")]
        [Log("修改数据字典", OperateType.Update)]
        public async Task<ActionResult> UpdateDict(int id, [FromBody] DictDto dto)
        {
            if (id != dto.DictId)
            {
                return ErrorMessage("要更新的数据和ID不匹配");
            }
            await service.UpdateDict(dto, CurrentUser);
            return NoContent();
        }

        /// <summary>
        /// 删除数据字典
        /// </summary>
        [HttpDelete]
        [EndpointSummary("删除数据字典")]
        [Log("删除数据字典", OperateType.Delete)]
        public async Task<ActionResult> DeleteDictByIds([FromQuery] int[] ids)
        {
            if (ids.Length == 0)
            {
                return ErrorMessage("参数错误：没有接收到要删除的主键值");
            }
            await service.DeleteDictByIds(ids, CurrentUser);
            return NoContent();
        }
        #endregion
    }
}