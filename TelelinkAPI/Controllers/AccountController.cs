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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Register([FromBody] POCOUser pocoUser)
        {

            ApplicationUser appUser = new ApplicationUser
                            {       
                                UserName = pocoUser.UserName,
                                Email = pocoUser.Email,
                                Owner = pocoUser.Owner
                            };

            IdentityResult result = await userManager.CreateAsync(appUser, pocoUser.Password);

            return Ok(result.Errors);   
        }

        [HttpGet]
        public ActionResult<String> Login(ApplicationUser model)
        {
            var a = 1;
            return Ok(a);
        }
    }
}
