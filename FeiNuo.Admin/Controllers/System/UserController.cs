using FeiNuo.Admin.Services.System;
using Microsoft.AspNetCore.Mvc;

namespace FeiNuo.Admin.Controllers.System
{
    /// <summary>
    /// API接口：用户
    /// </summary>
    [Route("api/system/users")]
    public class UserController : BaseController
    {
        #region 构造函数
        private readonly UserService service;
        public UserController(UserService service)
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
        public async Task<PageResult<UserDto>> GetUserList([FromQuery] UserQuery query, [FromQuery] Pager pager)
        {
            return await service.FindPagedList(query, pager, CurrentUser);
        }

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="userId">主键ID</param>
        [HttpGet("{userId}")]
        [EndpointSummary("主键查询")]
        public async Task<UserDto> GetUser(int userId)
        {
            return await service.GetUser(userId);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        [HttpGet("export")]
        [EndpointSummary("导出Excel")]
        public async Task<ActionResult> ExportUsers([FromQuery] UserQuery query)
        {
            var pager = await service.FindPagedList(query, Pager.Unpaged, CurrentUser);
            var excel = new ExcelConfig($"用户导出{DateTime.Now:yyyyMMddHHmmss}.xlsx", pager.DataList, [
                new ExcelColumn<UserDto>("登录用户名", d => d.Username, 15),
                new ExcelColumn<UserDto>("用户昵称", d => d.Nickname, 15),
                new ExcelColumn<UserDto>("登录密码", d => d.Password, 15),
                new ExcelColumn<UserDto>("部门ID", d => d.DeptId, 15),
                new ExcelColumn<UserDto>("性别：M/F/O", d => d.Gender, 15),
                new ExcelColumn<UserDto>("手机号码", d => d.Cellphone, 15),
                new ExcelColumn<UserDto>("电子邮箱", d => d.Email, 15),
                new ExcelColumn<UserDto>("头像", d => d.Avatar, 15),
                new ExcelColumn<UserDto>("用户状态:正常，锁定，密码过期等", d => d.Status, 15),
                new ExcelColumn<UserDto>("是否作废", d => d.Disabled, 15),
                new ExcelColumn<UserDto>("自我介绍", d => d.Introduction, 15),
                new ExcelColumn<UserDto>("备注", d => d.Remark, 15),
                new ExcelColumn<UserDto>("创建人", d => d.CreateBy, 15),
                new ExcelColumn<UserDto>("创建时间", d => d.CreateTime, 15),
            ]);
            var bytes = PoiHelper.GetExcelBytes(excel);
            return File(bytes, excel.ContentType, excel.FileName);
        }
        #endregion

        #region 增 删 改
        /// <summary>
        /// 新增用户
        /// </summary>
        [HttpPost]
        [EndpointSummary("新增用户")]
        [Log("新增用户", OperateType.Create, false, true)]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserDto dto)
        {
            dto = await service.CreateUser(dto, CurrentUser);
            return CreatedAtAction("GetUser", new { userId = dto.UserId }, dto);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        [HttpPut("{id}")]
        [EndpointSummary("修改用户")]
        [Log("修改用户", OperateType.Update)]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UserDto dto)
        {
            if (id != dto.UserId)
            {
                return ErrorMessage("要更新的数据和ID不匹配");
            }
            await service.UpdateUser(dto, CurrentUser);
            return NoContent();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpDelete]
        [EndpointSummary("删除用户")]
        [Log("删除用户", OperateType.Delete)]
        public async Task<ActionResult> DeleteUserByIds([FromQuery] int[] ids)
        {
            if (ids.Length == 0)
            {
                return ErrorMessage("参数错误：没有接收到要删除的主键值");
            }
            await service.DeleteUserByIds(ids, CurrentUser);
            return NoContent();
        }
        #endregion
    }
}