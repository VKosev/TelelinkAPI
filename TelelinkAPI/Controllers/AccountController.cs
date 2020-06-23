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
        public async Task<IActionResult> Register([FromBody] POCOUser pocoUser, string modelName) // POCOUser is used to get the password from JSON.
        {

            //Model model = new Model { Name = modelName };
            
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

        [HttpGet]
        public async Task<IActionResult> GenerateFirstAdmin()
        {
            ApplicationRole adminRole = new ApplicationRole { Name = "Admin" };
            ApplicationRole userRole = new ApplicationRole { Name = "User" };

            IdentityResult result = new IdentityResult();

            result = await _roleManager.CreateAsync(adminRole);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            result = await _roleManager.CreateAsync(userRole);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
           
            Owner adminOwner = new Owner() { Name = "Admin Adminov" };
            ApplicationUser adminUser = new ApplicationUser()
            {
                UserName = "MasterAdmin",
                Email = "MasterAdmin@abv.bg",
                Owner = adminOwner,
            };

            string adminPassword = "FirstAdmin";

            result = await _userManager.CreateAsync(adminUser, adminPassword);
            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            result = await _userManager.AddToRoleAsync(adminUser, adminRole.Name);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            else
            {
                var response = new
                {
                    StatusCode = 200,
                    Message = "First user as admin added successfully.",
                    AdminUsername = "MasterAdmin",
                    Password = "FirstAdmin"
                };

                return Ok(response);
            }
        }
    }
}
