using FeiNuo.Admin.Services.System;
using Microsoft.AspNetCore.Mvc;

namespace FeiNuo.Admin.Controllers.System
{
    /// <summary>
    /// API接口：操作日志
    /// </summary>
    [Route("api/system/logs")]
    public class LogController : BaseController
    {
        #region 构造函数
        private readonly LogService service;
        public LogController(LogService service)
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
        public async Task<PageResult<LogDto>> GetLogList([FromQuery] LogQuery query, [FromQuery] Pager pager)
        {
            return await service.FindPagedList(query, pager, CurrentUser);
        }

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="logId">主键ID</param>
        [HttpGet("{logId}")]
        [EndpointSummary("主键查询")]
        public async Task<LogDto> GetLog(long logId)
        {
            return await service.GetLog(logId);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        [HttpGet("export")]
        [EndpointSummary("导出Excel")]
        public async Task<ActionResult> ExportLogs([FromQuery] LogQuery query)
        {
            var pager = await service.FindPagedList(query, Pager.Unpaged, CurrentUser);
            var excel = new ExcelConfig($"操作日志导出{DateTime.Now:yyyyMMddHHmmss}.xlsx", pager.DataList, [
                new ExcelColumn<LogDto>("操作类型", d => d.OperateType, 15),
                new ExcelColumn<LogDto>("日志类别(简短描述)", d => d.LogTitle, 15),
                new ExcelColumn<LogDto>("日志内容", d => d.LogContent, 15),
                new ExcelColumn<LogDto>("请求URL", d => d.RequestPath, 15),
                new ExcelColumn<LogDto>("请求方式", d => d.RequestMethod, 15),
                new ExcelColumn<LogDto>("请求参数", d => d.RequestParam, 15),
                new ExcelColumn<LogDto>("是否执行成功", d => d.Success, 15),
                new ExcelColumn<LogDto>("执行时长,毫秒", d => d.ExecuteTime, 15),
                new ExcelColumn<LogDto>("客户端IP", d => d.ClientIp, 15),
                new ExcelColumn<LogDto>("客户端操作系统", d => d.ClientOs, 15),
                new ExcelColumn<LogDto>("客户端浏览器", d => d.ClientBrowser, 15),
                new ExcelColumn<LogDto>("创建人", d => d.CreateBy, 15),
                new ExcelColumn<LogDto>("创建时间", d => d.CreateTime, 15),
            ]);
            var bytes = PoiHelper.GetExcelBytes(excel);
            return File(bytes, excel.ContentType, excel.FileName);
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增操作日志
        /// </summary>
        [HttpPost]
        [EndpointSummary("新增操作日志")]
        [Log("新增操作日志", OperateType.Create, false, true)]
        public async Task<ActionResult<LogDto>> CreateLog([FromBody] LogDto dto)
        {
            dto = await service.CreateLog(dto, CurrentUser);
            return CreatedAtAction("GetLog", new { logId = dto.LogId }, dto);
        }

        /// <summary>
        /// 修改操作日志
        /// </summary>
        [HttpPut("{id}")]
        [EndpointSummary("修改操作日志")]
        [Log("修改操作日志", OperateType.Update)]
        public async Task<ActionResult> UpdateLog(long id, [FromBody] LogDto dto)
        {
            if (id != dto.LogId)
            {
                return ErrorMessage("要更新的数据和ID不匹配");
            }
            await service.UpdateLog(dto, CurrentUser);
            return NoContent();
        }

        /// <summary>
        /// 删除操作日志
        /// </summary>
        [HttpDelete]
        [EndpointSummary("删除操作日志")]
        [Log("删除操作日志", OperateType.Delete)]
        public async Task<ActionResult> DeleteLogByIds([FromQuery] long[] ids)
        {
            if (ids.Length == 0)
            {
                return ErrorMessage("参数错误：没有接收到要删除的主键值");
            }
            await service.DeleteLogByIds(ids, CurrentUser);
            return NoContent();
        }
        #endregion
    }
}