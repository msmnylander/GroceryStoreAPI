using GroceryStoreAPI.Contracts;
using GroceryStoreAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Controllers
{
    /// <summary>
    /// The Login API endpoint.
    /// </summary>
    [Route("api/login")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class LoginController : ControllerBase
    {
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        /// <summary>
        /// Authenticate a user.
        /// This allows anonymous access.
        /// If successful, this will return a persistent cookie that can be used for subsequent logins.
        /// In this instance, the cookie never expires but in a real app, we probably would want to set a lifetime.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("authenticate")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            user = await _loginService.Authenticate(user.Username,user.Password);
            if (user == null)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Invalid username or password.");
            }

            //
            // Create a claim ticket.
            //
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim("FullName", user.Username),
                    new Claim(ClaimTypes.Role, "User"),
                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //
            // Cookie properties.
            //
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true
            };

            //
            // Let the runtime bake a cookie for us.
            //
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            return Ok(user);
        }

        private ILoginService _loginService;

    }
}
