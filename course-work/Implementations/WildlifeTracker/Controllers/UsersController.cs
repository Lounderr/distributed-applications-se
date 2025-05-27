using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Services;

namespace WildlifeTracker.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController(IOnlineUsersService onlineUsers) : ControllerBase
    {

        [HttpGet("online")]
        public ActionResult GetOnlineUsers()
        {
            return this.Ok(onlineUsers.GetOnlineUsers());
        }
    }
}
