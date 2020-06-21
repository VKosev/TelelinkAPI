using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TelelinkAPI.Data;
using TelelinkAPI.Models;

namespace TelelinkAPI.Controllers
{
    [Route("{api}/{controller}/{action}")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Register(User model)
        {
          
           
            var appUser = new IdentityUser<User>
            appUser.UserName = model.UserName;
            appUser.Email = model.Email;
            appUser.Owner = model.Owner;

            string password = "aksD23@!ds";
           

            IdentityResult result = await userManager.CreateAsync(appUser, password);

            var err = result.Errors;
                        
            return Ok(err);   
        }

        [HttpGet]
        public ActionResult<String> Login(User model)
        {
            var a = 1;
            return Ok(a);
        }
    }
}
