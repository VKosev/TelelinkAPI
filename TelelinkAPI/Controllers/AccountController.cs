using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TelelinkAPI.Data;
using TelelinkAPI.Models;
using TelelinkAPI.POCOs;
using TelelinkAPI.Services;

namespace TelelinkAPI.Controllers
{
    [Route("{api}/{controller = Account}/{action}")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly JwtAuthenticationService _jwtAuthenticationService;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<ApplicationRole> roleManager,
                                 ApplicationDbContext context,
                                 JwtAuthenticationService jwtAuthenticationService)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._context = context;
            this._jwtAuthenticationService = jwtAuthenticationService;
        }
    
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] POCOUser pocoUser) // POCOUser is used to get the password from JSON.
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] POCOUser pocoUser)
        {
            IActionResult response = Unauthorized();

            var appUser = await _userManager.FindByNameAsync(pocoUser.UserName);
            
           
            if ( await _userManager.CheckPasswordAsync(appUser, pocoUser.Password))
            {
                var tokenString = _jwtAuthenticationService.GenerateJsonWebToken(appUser);
                return Ok(new { token = tokenString });
            }

            return response;
        }

        [HttpGet]
        [AllowAnonymous]
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

                var responseOBJ = new
                {
                    StatusCode = 200,
                    Message = "First user as admin added successfully.",
                    AdminUsername = "MasterAdmin",
                    Password = "FirstAdmin"
                };

                return Ok(responseOBJ);
            }
        }
    }
}
