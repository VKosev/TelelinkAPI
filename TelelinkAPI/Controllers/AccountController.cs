using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TelelinkAPI.Data;
using TelelinkAPI.Models;
using TelelinkAPI.POCOs;

namespace TelelinkAPI.Controllers
{
    [Route("{api}/{controller}/{action}")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<ApplicationRole> roleManager,
                                 ApplicationDbContext context)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] POCOUser pocoUser) // POCOUser is used to get the password from JSON.
        {

            ApplicationUser appUser = new ApplicationUser
            {
                UserName = pocoUser.UserName,
                Email = pocoUser.Email,
                Owner = pocoUser.Owner
            };

            IdentityResult result = await _userManager.CreateAsync(appUser, pocoUser.Password);

            return Ok(result.Errors);
        }

        [HttpGet]
        public ActionResult<String> Login(ApplicationUser model)
        {
            var a = 1;
            return Ok(a);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(ApplicationRole appRole)
        {

            var roleExists = await _roleManager.RoleExistsAsync(appRole.Name);

            if (!roleExists)
            {
                IdentityResult result = await _roleManager.CreateAsync(appRole);
                return Ok(result.Succeeded);
            }
            else
            {
                return BadRequest("Failed to add Roles");
            }

        }
    }
}
