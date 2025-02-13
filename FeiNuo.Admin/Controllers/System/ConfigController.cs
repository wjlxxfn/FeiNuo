using FeiNuo.Admin.Services.System;
using Microsoft.AspNetCore.Mvc;

namespace FeiNuo.Admin.Controllers.System
{
    /// <summary>
    /// API接口：参数配置
    /// </summary>
    [Route("api/system/configs")]
    public class ConfigController : BaseController
    {
        #region 构造函数
        private readonly ConfigService service;
        public ConfigController(ConfigService service)
        {
            this.service = service;
        }
        #endregion

        #region 查询导出 
        /// <summary>
        /// 分页查询
        /// </summary>
        [HttpGet]
        public async Task<PageResult<ConfigDto>> GetConfigList([FromQuery] ConfigQuery query, [FromQuery] Pager pager)
        {
            return await service.FindPagedList(query, pager, CurrentUser);
        }

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="configId">主键ID</param>
        [HttpGet("{configId}")]
        public async Task<ConfigDto> GetConfig(int configId)
        {
            return await service.GetConfig(configId);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        [HttpGet("export")]
        public async Task<ActionResult> ExportConfigs([FromQuery] ConfigQuery query)
        {
            var pager = await service.FindPagedList(query, Pager.Unpaged, CurrentUser);
            var excel = new ExcelConfig($"参数配置导出{DateTime.Now:yyyyMMddHHmmss}.xlsx", pager.DataList, [
                new ExcelColumn<ConfigDto>("参数编码", d => d.ConfigCode, 15),
                new ExcelColumn<ConfigDto>("参数名称", d => d.ConfigName, 15),
                new ExcelColumn<ConfigDto>("配置内容", d => d.ConfigValue, 15),
                new ExcelColumn<ConfigDto>("其他配置", d => d.ExtraValue, 15),
                new ExcelColumn<ConfigDto>("备注说明", d => d.Remark, 15),
                new ExcelColumn<ConfigDto>("创建人", d => d.CreateBy, 15),
                new ExcelColumn<ConfigDto>("创建时间", d => d.CreateTime, 15),
            ]);
            var bytes = PoiHelper.GetExcelBytes(excel);
            return File(bytes, excel.ContentType, excel.FileName);
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增参数配置
        /// </summary>
        [HttpPost]
        [Log("新增参数配置", OperateType.Create, false, true)]
        public async Task<ActionResult<ConfigDto>> CreateConfig([FromBody] ConfigDto dto)
        {
            dto = await service.CreateConfig(dto, CurrentUser);
            return CreatedAtAction("GetConfig", new { configId = dto.ConfigId }, dto);
        }

        /// <summary>
        /// 修改参数配置
        /// </summary>
        [HttpPut("{id}")]
        [Log("修改参数配置", OperateType.Update)]
        public async Task<ActionResult> UpdateConfig(int id, [FromBody] ConfigDto dto)
        {
            if (id != dto.ConfigId)
            {
                return ErrorMessage("要更新的数据和ID不匹配");
            }
            await service.UpdateConfig(dto, CurrentUser);
            return NoContent();
        }

        /// <summary>
        /// 删除参数配置
        /// </summary>
        [HttpDelete]
        [Log("删除参数配置", OperateType.Delete)]
        public async Task<ActionResult> DeleteConfigByIds([FromQuery] int[] ids)
        {
            if (ids.Length == 0)
            {
                return ErrorMessage("参数错误：没有接收到要删除的主键值");
            }
            await service.DeleteConfigByIds(ids, CurrentUser);
            return NoContent();
        }
        #endregion
    }
}