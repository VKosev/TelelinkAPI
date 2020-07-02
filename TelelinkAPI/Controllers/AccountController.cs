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
using Serilog;
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
        public async Task<IActionResult> Register([FromBody] RoleAndPasswordUserPoco roleAndPasswordUserPoco) // POCOUser is used to get the password and role from JSON.
        {
            Error error = new Error();

            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = roleAndPasswordUserPoco.UserName,
                Email = roleAndPasswordUserPoco.Email,
                Owner = roleAndPasswordUserPoco.Owner
            };

            // Create User
            IdentityResult result = await _userManager.CreateAsync(applicationUser, roleAndPasswordUserPoco.Password);
            if (!result.Succeeded)
            {
                return BadRequest(error.Message = "User already exist");
            }

            // Asign role to the new User
            result = await _userManager.AddToRoleAsync(applicationUser, roleAndPasswordUserPoco.Role);
            if (!result.Succeeded)
            {
                _context.Users.Remove(applicationUser);
                await _context.SaveChangesAsync();
                return BadRequest(error.Message = "Failed to assign role to use");
            }

            return (Ok(applicationUser));
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] RoleAndPasswordUserPoco roleAndPasswordUserPoco)
        {
            Error error = new Error();

            var appUser = await _userManager.FindByNameAsync(roleAndPasswordUserPoco.UserName);

            if (await _userManager.CheckPasswordAsync(appUser, roleAndPasswordUserPoco.Password))
            {
                var tokenString = _jwtAuthenticationService.GenerateJsonWebToken(appUser);
                
                return Ok(new { token = tokenString });
            }
            
            return BadRequest(error.Message = "Password or Username incorrect.");          
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateFirstAdmin()
        {
            Error error = new Error();

            ApplicationRole adminRole = new ApplicationRole { Name = "Admin" };
            ApplicationRole userRole = new ApplicationRole { Name = "User" };

            Owner adminOwner = new Owner() { Name = "Admin Adminov" };

            ApplicationUser applicationUser = new ApplicationUser()
            {
                UserName = "MasterAdmin",
                Email = "MasterAdmin@abv.bg",
                Owner = adminOwner,
            };
            string adminPassword = "FirstAdmin";

            if (await _userManager.CheckPasswordAsync(applicationUser, adminPassword))
            {
                
                return BadRequest(error.Message = "First admin is already created");
            }

            IdentityResult result = new IdentityResult();
            result = await _roleManager.CreateAsync(adminRole);

            if (!result.Succeeded)
            {
                return BadRequest(error.Message = "There was a problem in creating admin role.");
            }

            result = await _roleManager.CreateAsync(userRole);
            if (!result.Succeeded)
            {
                return BadRequest(error.Message = "There was a problem in creating user role.");
            }

            result = await _userManager.CreateAsync(applicationUser, adminPassword);
            if (!result.Succeeded)
            {
                return BadRequest(error.Message = "There was a problem in creating the first admin user");
            }

            result = await _userManager.AddToRoleAsync(applicationUser, adminRole.Name);
            if (!result.Succeeded)
            {
                return BadRequest(error.Message = "There was a problem in asigning MasterAdmin user to role admin");
            }
            else
            {
                var responseObject = new
                {                   
                    Message = "First user as admin added successfully.",
                    UserName = "MasterAdmin",
                    Password = "FirstAdmin"
                };

                return Ok(responseObject);
            }
        }


    }
}
