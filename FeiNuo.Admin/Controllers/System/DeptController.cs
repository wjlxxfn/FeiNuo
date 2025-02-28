using FeiNuo.Admin.Services.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeiNuo.Admin.Controllers.System
{
    /// <summary>
    /// API接口：部门
    /// </summary>
    [Authorize]
    [Route("api/system/[controller]")]
    public class DeptController : BaseController
    {
        #region 构造函数
        private readonly DeptService service;
        public DeptController(DeptService service)
        {
            this.service = service;
        }
        #endregion

        #region 查询导出 
        /// <summary>
        /// 分页查询
        /// </summary>
        [HttpGet("tree")]
        [EndpointSummary("查询部门树")]
        public async Task<IEnumerable<TreeOption>> GetDeptTree([FromQuery] int? rootId)
        {
            return await service.GetDeptTree(rootId);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        [HttpGet]
        [EndpointSummary("分页查询")]
        public async Task<PageResult<DeptDto>> GetDeptList([FromQuery] DeptQuery query, [FromQuery] Pager pager)
        {
            return await service.FindPagedList(query, pager, CurrentUser);
        }

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="deptId">主键ID</param>
        [HttpGet("{deptId}")]
        [EndpointSummary("主键查询")]
        public async Task<DeptDto> GetDept(int deptId)
        {
            return await service.GetDept(deptId);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        [HttpGet("export")]
        [EndpointSummary("导出Excel")]
        public async Task<ActionResult> ExportDepts([FromQuery] DeptQuery query)
        {
            var pager = await service.FindPagedList(query, Pager.Unpaged, CurrentUser);
            var excel = new ExcelConfig($"部门导出{DateTime.Now:yyyyMMddHHmmss}.xlsx", pager.DataList, [
                new ExcelColumn<DeptDto>("上级ID", d => d.ParentId),
                new ExcelColumn<DeptDto>("部门名称", d => d.DeptName),
                new ExcelColumn<DeptDto>("排序号", d => d.SortNo),
                new ExcelColumn<DeptDto>("是否作废", d => d.Disabled),
                new ExcelColumn<DeptDto>("备注说明", d => d.Remark),
                new ExcelColumn<DeptDto>("创建人", d => d.CreateBy),
                new ExcelColumn<DeptDto>("创建时间", d => d.CreateTime),
            ]);
            var bytes = PoiHelper.GetExcelBytes(excel);
            return File(bytes, excel.ContentType, excel.FileName);
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增部门
        /// </summary>
        [HttpPost]
        [EndpointSummary("新增部门")]
        [Log("新增部门", OperateType.Create, false, true)]
        public async Task<ActionResult<DeptDto>> CreateDept([FromBody] DeptDto dto)
        {
            dto = await service.CreateDept(dto, CurrentUser);
            return CreatedAtAction("GetDept", new { deptId = dto.DeptId }, dto);
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        [HttpPut("{id}")]
        [EndpointSummary("修改部门")]
        [Log("修改部门", OperateType.Update)]
        public async Task<ActionResult> UpdateDept(int id, [FromBody] DeptDto dto)
        {
            if (id != dto.DeptId)
            {
                return ErrorMessage("要更新的数据和ID不匹配");
            }
            await service.UpdateDept(dto, CurrentUser);
            return Ok();
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        [HttpDelete]
        [EndpointSummary("删除部门")]
        [Authorize(Roles = "SuperAdmin")]
        [Log("删除部门", OperateType.Delete)]
        public async Task<ActionResult> DeleteDeptByIds([FromQuery] int[] ids)
        {
            if (ids.Length == 0)
            {
                return ErrorMessage("参数错误：没有接收到要删除的主键值");
            }
            await service.DeleteDeptByIds(ids, CurrentUser);
            return NoContent();
        }
        #endregion


        #region 其他操作
        /// <summary>
        /// 修改部门状态
        /// </summary>
        [HttpPatch("status/{deptId}")]
        [Log("修改部门状态", OperateType.Update)]
        public async Task<ActionResult> UpdateDeptStatus(int deptId, [FromQuery] bool disabled)
        {
            await service.UpdateDeptStatus(deptId, disabled);
            return Ok();
        }
        #endregion
    }
}