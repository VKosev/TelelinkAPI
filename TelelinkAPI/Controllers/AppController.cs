using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelelinkAPI.Data;
using TelelinkAPI.Models;
using TelelinkAPI.POCOs;

namespace TelelinkAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("{api}/{controller = app}/{action}")]
    [ApiController]
    public class AppController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppController(ApplicationDbContext context)
        {
            this._context = context;

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Owners()
        {
            var Owners = (from o in _context.Owners
                         select o);
            if(Owners.Any())
            {
                return Ok(Owners);
            }
            else
            {
                return Ok("asdasdas");
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AllData()
        {
            var OwnerModelsCollection = _context.OwnerModels.Include(m => m.Model).Include(o => o.Owner).ThenInclude(u => u.User);

            return Ok(OwnerModelsCollection);
            
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult UserData(string username)  
        {
            DataUserPoco dataUserPoco = new DataUserPoco();
           

            var dbQueryData = (from u in _context.Users
                               join o in _context.Owners on u.Id equals o.UserId
                               join om in _context.OwnerModels on o.Id equals om.OwnerId
                               join m in _context.Models on om.ModelId equals m.Id
                               where u.UserName == username
                               select new { username = u.UserName, owner = o.Name, models = m.Name }).Distinct().ToList();

            foreach(var data in dbQueryData)
            {
                dataUserPoco.Username = data.username;
                dataUserPoco.OwnerName = data.owner;
                dataUserPoco.ModelsNames.Add(data.models);
            }

           
            return Ok(dataUserPoco);

        }
    }
}
