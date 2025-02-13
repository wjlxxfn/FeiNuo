using FeiNuo.Admin.Services.System;
using Microsoft.AspNetCore.Mvc;

namespace FeiNuo.Admin.Controllers.System
{
    /// <summary>
    /// API接口：角色
    /// </summary>
    [Route("api/system/roles")]
    public class RoleController : BaseController
    {
        #region 构造函数
        private readonly RoleService service;
        public RoleController(RoleService service)
        {
            this.service = service;
        }
        #endregion

        #region 查询导出 
        /// <summary>
        /// 分页查询
        /// </summary>
        [HttpGet]
        public async Task<PageResult<RoleDto>> GetRoleList([FromQuery] RoleQuery query, [FromQuery] Pager pager)
        {
            return await service.FindPagedList(query, pager, CurrentUser);
        }

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="roleId">主键ID</param>
        [HttpGet("{roleId}")]
        public async Task<RoleDto> GetRole(int roleId)
        {
            return await service.GetRole(roleId);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        [HttpGet("export")]
        public async Task<ActionResult> ExportRoles([FromQuery] RoleQuery query)
        {
            var pager = await service.FindPagedList(query, Pager.Unpaged, CurrentUser);
            var excel = new ExcelConfig($"角色导出{DateTime.Now:yyyyMMddHHmmss}.xlsx", pager.DataList, [
                new ExcelColumn<RoleDto>("角色编码", d => d.RoleCode, 15),
                new ExcelColumn<RoleDto>("角色名称", d => d.RoleName, 15),
                new ExcelColumn<RoleDto>("角色状态", d => d.Disabled?"作废":"正常", 10),
                new ExcelColumn<RoleDto>("备注说明", d => d.Remark, 25),
                new ExcelColumn<RoleDto>("创建人", d => d.CreateBy, 15),
                new ExcelColumn<RoleDto>("创建时间", d => d.CreateTime, 12),
            ]);
            var bytes = PoiHelper.GetExcelBytes(excel);
            return File(bytes, excel.ContentType, excel.FileName);
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增角色
        /// </summary>
        [HttpPost]
        [Log("新增角色", OperateType.Create, false, true)]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] RoleDto dto)
        {
            dto = await service.CreateRole(dto, CurrentUser);
            return CreatedAtAction("GetRole", new { roleId = dto.RoleId }, dto);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        [HttpPut("{id}")]
        [Log("修改角色", OperateType.Update)]
        public async Task<ActionResult> UpdateRole(int id, [FromBody] RoleDto dto)
        {
            if (id != dto.RoleId)
            {
                return ErrorMessage("要更新的数据和ID不匹配");
            }
            await service.UpdateRole(dto, CurrentUser);
            return NoContent();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [HttpDelete]
        [Log("删除角色", OperateType.Delete)]
        public async Task<ActionResult> DeleteRoleByIds([FromQuery] int[] ids)
        {
            if (ids.Length == 0)
            {
                return ErrorMessage("参数错误：没有接收到要删除的主键值");
            }
            await service.DeleteRoleByIds(ids, CurrentUser);
            return NoContent();
        }

        /// <summary>
        /// 修改角色状态
        /// </summary>
        [HttpPatch("{roleId}/status/{status}")]
        [Log("修改角色状态", OperateType.Update)]
        public async Task<ActionResult> UpdateRoleStatus(int roleId, int status)
        {
            var disabled = status == 1;
            await service.UpdateRoleStatus(roleId, disabled, CurrentUser);
            return NoContent();
        }

        /// <summary>
        /// 修改角色权限
        /// </summary>
        [HttpPatch("{roleId}/menus")]
        [Log("修改角色权限", OperateType.Update)]
        public async Task<ActionResult> UpdateRoleMenus(int roleId, [FromBody] int[] menuIds)
        {
            await service.UpdateRoleMenus(roleId, menuIds, CurrentUser);
            return NoContent();
        }
        #endregion
    }
}