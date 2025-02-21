using FeiNuo.Admin.Services.System;
using Microsoft.AspNetCore.Mvc;

namespace FeiNuo.Admin.Controllers.System
{
    /// <summary>
    /// API接口：菜单
    /// </summary>
    [Route("api/system/menus")]
    public class MenuController : BaseController
    {
        #region 构造函数
        private readonly MenuService service;
        public MenuController(MenuService service)
        {
            this.service = service;
        }
        #endregion

        #region 查询导出 
        /// <summary>
        /// 查询菜单树
        /// </summary>
        [HttpGet("tree")]
        [EndpointSummary("查询菜单树")]
		public async Task<List<MenuDto>> GetMenuTree()
        {
            return await service.GetMenuTree();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        [HttpGet]
        [EndpointSummary("分页查询")]
        public async Task<PageResult<MenuDto>> GetMenuList([FromQuery] MenuQuery query, [FromQuery] Pager pager)
        {
            return await service.FindPagedList(query, pager, CurrentUser);
        }

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="menuId">主键ID</param>
        [HttpGet("{menuId}")]
        [EndpointSummary("主键查询")]
        public async Task<MenuDto> GetMenu(int menuId)
        {
            return await service.GetMenu(menuId);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        [HttpGet("export")]
        [EndpointSummary("导出Excel")]
        public async Task<ActionResult> ExportMenus([FromQuery] MenuQuery query)
        {
            var pager = await service.FindPagedList(query, Pager.Unpaged, CurrentUser);
            var excel = new ExcelConfig($"菜单导出{DateTime.Now:yyyyMMddHHmmss}.xlsx", pager.DataList, [
                new ExcelColumn<MenuDto>("上级ID", d => d.ParentId, 15),
                new ExcelColumn<MenuDto>("菜单名称", d => d.MenuName, 15),
                new ExcelColumn<MenuDto>("菜单类型", d => d.MenuType, 15),
                new ExcelColumn<MenuDto>("菜单地址", d => d.MenuPath, 15),
                new ExcelColumn<MenuDto>("权限标识", d => d.Permission, 15),
                new ExcelColumn<MenuDto>("排序号", d => d.SortNo, 15),
                new ExcelColumn<MenuDto>("菜单图标", d => d.Icon, 15),
                new ExcelColumn<MenuDto>("备注说明", d => d.Remark, 15),
                new ExcelColumn<MenuDto>("创建人", d => d.CreateBy, 15),
                new ExcelColumn<MenuDto>("创建时间", d => d.CreateTime, 15),
            ]);
            var bytes = PoiHelper.GetExcelBytes(excel);
            return File(bytes, excel.ContentType, excel.FileName);
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增菜单
        /// </summary>
        [HttpPost]
        [EndpointSummary("新增菜单")]
        [Log("新增菜单", OperateType.Create, false, true)]
        public async Task<ActionResult<MenuDto>> CreateMenu([FromBody] MenuDto dto)
        {
            dto = await service.CreateMenu(dto, CurrentUser);
            return CreatedAtAction("GetMenu", new { menuId = dto.MenuId }, dto);
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        [HttpPut("{id}")]
        [EndpointSummary("修改菜单")]
        [Log("修改菜单", OperateType.Update)]
        public async Task<ActionResult> UpdateMenu(int id, [FromBody] MenuDto dto)
        {
            if (id != dto.MenuId)
            {
                return ErrorMessage("要更新的数据和ID不匹配");
            }
            await service.UpdateMenu(dto, CurrentUser);
            return NoContent();
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        [HttpDelete]
        [EndpointSummary("删除菜单")]
        [Log("删除菜单", OperateType.Delete)]
        public async Task<ActionResult> DeleteMenuByIds([FromQuery] int[] ids)
        {
            if (ids.Length == 0)
            {
                return ErrorMessage("参数错误：没有接收到要删除的主键值");
            }
            await service.DeleteMenuByIds(ids, CurrentUser);
            return NoContent();
        }
        #endregion
    }
}