using FeiNuo.Admin.Services.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeiNuo.Admin.Controllers.System
{
    /// <summary>
    /// API接口：菜单
    /// </summary>
    [Authorize]
    [Route("api/system/[controller]")]
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
        /// 主键查询
        /// </summary>
        /// <param name="menuId">主键ID</param>
        [HttpGet("{menuId}")]
        [EndpointSummary("主键查询")]
        public async Task<MenuDto> GetMenu(int menuId)
        {
            return await service.GetMenu(menuId);
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