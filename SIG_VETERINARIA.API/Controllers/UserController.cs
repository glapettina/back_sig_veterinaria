using Microsoft.AspNetCore.Mvc;
using SIG_VETERINARIA.Abstractions.IApplication;
using SIG_VETERINARIA.DTOs.Common;
using SIG_VETERINARIA.DTOs.User;

namespace SIG_VETERINARIA.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private IUserApplication _userApplication;

        public UserController(IUserApplication userApplication)
        {
            _userApplication = userApplication;
        }

        [HttpPost]
        [Route("list")]
        public async Task<ActionResult> GetAll([FromBody] UserListRequestDto request)
        {
            try
            {
                var res = await _userApplication.GetAll(request);
                return Ok(res);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("create")]

        public async Task<ActionResult> Create(UserCreateRequestDto request)
        {
            try
            {
                var res = await _userApplication.Create(request);
                return Ok(res);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult> Delete(DeleteDto request)
        {
            try
            {
                var res = await _userApplication.Delete(request);
                return Ok(res);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

    }
}
