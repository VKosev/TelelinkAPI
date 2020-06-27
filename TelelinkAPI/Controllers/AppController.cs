using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
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
            var allData = (from om in _context.OwnerModels
                            join o in _context.Owners on om.OwnerId equals o.Id
                            join m in _context.Models on om.ModelId equals m.Id
                            join u in _context.Users on o.UserId equals u.Id
                            orderby o.Name descending
                            select new {ownerModel = om, ownerName = o.Name, modelName = m.Name, userName = u.UserName, email = u.Email });

            /* List<AllUsersDataPoco> allUsersData = new List<AllUsersDataPoco>();

            foreach(var item in allData)
            {
                AllUsersDataPoco userData = new AllUsersDataPoco
                {
                    OwnerModels = item.ownerModel,
                    OwnerName = item.ownerName,
                    ModelName = item.modelName,
                    UserName = item.userName,
                    Email = item.email
                };
                allUsersData.Add(userData);
            } */


            return Ok(allData);
            
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult UserData(string username)  
        {
            UserDataPoco UserDataPoco = new UserDataPoco();
           
            var dbQueryData = (from u in _context.Users
                               join o in _context.Owners on u.Id equals o.UserId
                               join om in _context.OwnerModels on o.Id equals om.OwnerId
                               join m in _context.Models on om.ModelId equals m.Id
                               where u.UserName == username
                               select new { username = u.UserName, email = u.Email, owner = o.Name, models = m.Name }).Distinct().ToList();

            foreach(var data in dbQueryData)
            {
                UserDataPoco.Username = data.username;
                UserDataPoco.OwnerName = data.owner;
                UserDataPoco.Email = data.email;
                UserDataPoco.ModelsNames.Add(data.models);
            }
            
            return Ok(UserDataPoco);
        }
    }
}
