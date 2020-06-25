using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelelinkAPI.Data;
using TelelinkAPI.Models;

namespace TelelinkAPI.Controllers
{
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
        [Authorize]
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
    }
}
