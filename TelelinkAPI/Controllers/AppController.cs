using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Serilog;
using TelelinkAPI.Data;
using TelelinkAPI.Models;
using TelelinkAPI.POCOs;
using TelelinkAPI.Services;

namespace TelelinkAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("{api}/{controller = app}/{action}")]
    [ApiController]
    public class AppController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SecurityService _securityService;

        public AppController(ApplicationDbContext context,
                            UserManager<ApplicationUser> userManager,
                            SecurityService securityService)
        {
            this._context = context;
            this._userManager = userManager;
            this._securityService = securityService;            
        }
          
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DeletePendingRegistration(int id)
        {
            try
            {
                PendingRegistration pendingRegistrations = (from pr in _context.PendingRegistrations
                                                            where pr.Id == id
                                                            select pr).First();

                _context.PendingRegistrations.Remove(pendingRegistrations);
                await _context.SaveChangesAsync();

                return Ok();

            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "Quering to the database failed");
                return BadRequest();
                
            }
            catch(DbUpdateException ex)
            {                
                Log.Error(ex, "failed removing pendingRegistration from the database");
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingRegistrations()
        {
            var pendingRegistrations = (from pr in _context.PendingRegistrations
                                        select pr).ToList();
            return Ok(pendingRegistrations);
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> CheckPendingRegistrations()
        {
            int count = (from pr in _context.PendingRegistrations
                         select pr).Count();
            if(count == 0)
            {
                return BadRequest("No Elements");
            }
            return count.ToString();
        } 
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterQueue(RoleAndPasswordUserPoco roleAndPasswordUserPoco)
        {
            Error error = new Error();
            var applicationUser = await _userManager.FindByNameAsync(roleAndPasswordUserPoco.UserName);

            if (await _userManager.CheckPasswordAsync(applicationUser, roleAndPasswordUserPoco.Password))
            {
                return BadRequest(error.Message="UserName already exist.");
            }
            
            bool emailExist = (from u in _context.Users
                                where u.Email == roleAndPasswordUserPoco.Email
                                select u.Email).Any();                          
            if (emailExist)
            {
                return BadRequest(error.Message="Email already exist");
            }

            bool ownerExist = (from o in _context.Owners
                               where o.Name == roleAndPasswordUserPoco.Owner.Name
                               select o.Name).Any();
            if(ownerExist)
            {
                return BadRequest(error.Message="Owner with that name already exist.");
            }

            PendingRegistration pendingRegistration = new PendingRegistration
            {
                UserName = roleAndPasswordUserPoco.UserName,
                EncriptedPassword = _securityService.AesEncryptToBytes(roleAndPasswordUserPoco.Password),
                Email = roleAndPasswordUserPoco.Email,
                OwnerName = roleAndPasswordUserPoco.Owner.Name,
                Role = roleAndPasswordUserPoco.Role
            };
            _context.PendingRegistrations.Add(pendingRegistration);
            _context.SaveChanges();

            return Ok("Registration Succesfull. Wait for admin approval.");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Security(string data)
        {
            byte[] key = Encoding.UTF8.GetBytes("AAECAwQFBgcICQoLDA0ODw=="); //Convert.FromBase64String("AAECAwQFBgcICQoLDA0ODw==");

            byte[] IV = Convert.FromBase64String("SDFCRwQFBghIQNoLDK0ODr==");

            //using my service to encrypt
            byte[] encrypted = _securityService.AesEncryptToBytes(data);

            string decriptedData = _securityService.AesDecryptFromBytes(encrypted);

            var a = new { encryptedData = Encoding.UTF8.GetString(encrypted), decriptedData = decriptedData };

            return Ok(a);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Owners()
        {
            var Owners = (from o in _context.Owners
                          select o);
            if (Owners.Any())
            {
                return Ok(Owners);
            }
            else
            {
                return Ok("asdasdas");
            }

        }

        [HttpGet]       
        [Authorize(Roles = "User, Admin")]
        public IActionResult AllData(int id) //id param used as flag indicating what sorting to do.
        {
            var allUsersDataWithModels = (from om in _context.OwnerModels
                           join o in _context.Owners on om.OwnerId equals o.Id
                           join m in _context.Models on om.ModelId equals m.Id
                           join u in _context.Users on o.UserId equals u.Id
                           select new { ownerModel = om, ownerName = o.Name, modelName = m.Name, userName = u.UserName, email = u.Email });

            var allUsersDataWithNoModels = (from o in _context.Owners                                                                                
                                          join u in _context.Users on o.UserId equals u.Id
                                          select new {ownerName = o.Name, userName = u.UserName, email = u.Email });

            List<AllUsersDataPoco> allUsersData = new List<AllUsersDataPoco>();
            
            foreach (var userData in allUsersDataWithModels)
            {
                AllUsersDataPoco allDataForOneUser = new AllUsersDataPoco
                {
                    OwnerModels = userData.ownerModel,
                    OwnerName = userData.ownerName,
                    ModelName = userData.modelName,
                    UserName = userData.userName,
                    Email = userData.email
                };
                allUsersData.Add(allDataForOneUser);
            }
            OwnerModel emptyOwnerModel = new OwnerModel
            {
                Description = "",
                Date = DateTime.Now,
            };
            foreach (var userData in allUsersDataWithNoModels)
            {
                AllUsersDataPoco allDataForOneUser2 = new AllUsersDataPoco
                {
                    OwnerModels = emptyOwnerModel,
                    ModelName = "",
                    OwnerName = userData.ownerName,                  
                    UserName = userData.userName,
                    Email = userData.email
                };
                allUsersData.Add(allDataForOneUser2);
            }
            
             switch (id)
            {
                case 1:
                    {
                        allUsersData = allUsersData.OrderBy(foo => foo.OwnerName).ToList();
                        return Ok(allUsersData);
                    }
                case 2:
                    {
                        allUsersData = allUsersData.OrderByDescending(o => o.OwnerName).ToList();
                        return Ok(allUsersData);
                    }
                case 3:
                    {
                        allUsersData = allUsersData.OrderBy(o => o.ModelName).ToList();
                        return Ok(allUsersData);
                    }
                case 4:
                    {
                        allUsersData = allUsersData.OrderByDescending(o => o.ModelName).ToList();
                        return Ok(allUsersData);
                    }
                case 5:
                    {
                        allUsersData = allUsersData.OrderBy(o => o.OwnerModels.Date).ToList();
                        return Ok(allUsersData);
                    }
                case 6:
                    {
                        allUsersData = allUsersData.OrderByDescending(o => o.OwnerModels.Date).ToList();
                        return Ok(allUsersData);
                    }
                default:
                    { 
                        return Ok(allUsersData);
                    }                    
            }           
        }
  
        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public IActionResult UserData(string username)  
        {
            UserDataPoco UserDataPoco = new UserDataPoco();
           
            var userWithModels = (from u in _context.Users
                               join o in _context.Owners on u.Id equals o.UserId
                               join om in _context.OwnerModels on o.Id equals om.OwnerId
                               join m in _context.Models on om.ModelId equals m.Id
                               where u.UserName == username
                               select new { username = u.UserName, email = u.Email, owner = o.Name, models = m.Name }).Distinct().ToList();

            if(userWithModels.Any())
            {

                foreach (var data in userWithModels)
                {
                    UserDataPoco.Username = data.username;
                    UserDataPoco.OwnerName = data.owner;
                    UserDataPoco.Email = data.email;
                    UserDataPoco.ModelsNames.Add(data.models);
                }
                return Ok(UserDataPoco);
            }
            else
            {
                var userWithNoModels = (from u in _context.Users
                               join o in _context.Owners on u.Id equals o.UserId
                               where u.UserName == username
                               select new { username = u.UserName, email = u.Email, owner = o.Name}).Distinct().ToList();

                foreach (var data in userWithNoModels)
                {
                    UserDataPoco.Username = data.username;
                    UserDataPoco.OwnerName = data.owner;
                    UserDataPoco.Email = data.email;
                }

                return Ok(UserDataPoco);
            }
        }
        
        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public IActionResult AddAssignModels(string username)
        {
            var allModels = (from m in _context.Models
                             select m);

            //should return only 1 result
            var owners = (from u in _context.Users
                         where u.UserName == username
                         join o in _context.Owners on u.Id equals o.UserId
                         select o).ToList();

           
            int ownerId = 0;

            //extracting owner id from the owners query.
            foreach (var owner in owners)
            {
                ownerId = (int)owner.Id;
            }

            /* string ownerName = (from o in _context.Owners
                                join u in _context.Users on o.UserId equals u.Id
                                where u.UserName == username
                                select new {name = o.Name }).ToString();*/
                       
            List<ModelAddsToOwnerCountPoco> modelAddsToOwnerCountPocosList = new List<ModelAddsToOwnerCountPoco>();

            foreach (var model in allModels)
            {
                ModelAddsToOwnerCountPoco modelAddsToOwnerCountPoco = new ModelAddsToOwnerCountPoco
                {
                    ModelName = model.Name,
                    ModelId = model.Id,
                    AddsToOwner = (from om in _context.OwnerModels
                                   where om.OwnerId == ownerId && model.Id == om.ModelId
                                   select om).Count()
                };
                
                modelAddsToOwnerCountPocosList.Add(modelAddsToOwnerCountPoco);
            }
            return Ok(modelAddsToOwnerCountPocosList);
        }

        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> AddModel(string modelName)
        {
            if(_context.Models.Any(m => m.Name == modelName))
            {
                return BadRequest(modelName+" already exist.");
            }

            Model model = new Model { Name = modelName };
             _context.Models.Add(model);
            var result = await _context.SaveChangesAsync();

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovePendingRegistration(int id)
        {
            PendingRegistration pendingRegistration = (from pr in _context.PendingRegistrations
                                                       where pr.Id == id
                                                       select pr).First();
            Error error = new Error();

            Owner owner = new Owner { Name = pendingRegistration.OwnerName };

            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = pendingRegistration.UserName,
                Email = pendingRegistration.Email,
                Owner = owner
            };
            string password = _securityService.AesDecryptFromBytes(pendingRegistration.EncriptedPassword);

            // Create User
            IdentityResult result = await _userManager.CreateAsync(applicationUser, password);
            if (!result.Succeeded)
            {
                return BadRequest(error.Message = "User already exist");
            }

            // Asign role to the new User
            result = await _userManager.AddToRoleAsync(applicationUser, pendingRegistration.Role);
            if (!result.Succeeded)
            {
                _context.Users.Remove(applicationUser);
                await _context.SaveChangesAsync();
                return BadRequest(error.Message = "Failed to assign role to use");
            }
            _context.PendingRegistrations.Remove(pendingRegistration);
            _context.SaveChanges();
            return (Ok(applicationUser));
        }

        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> AssignModel(string userName, string description, int id) // id is the id of the model;
        {
            Model model = await _context.Models.FindAsync(id);
            if(model == null)
            {
                return BadRequest("model is null");
            }
            model.OwnerModels = new List<OwnerModel>();
            var queryForOwner = (from u in _context.Users
                         where u.UserName == userName
                         join o in _context.Owners on u.Id equals o.UserId   
                         select o);

            Owner owner = queryForOwner.First();
            owner.OwnerModels = new List<OwnerModel>();
            OwnerModel ownerModel = new OwnerModel
            {
                Owner = owner,
                Model = model,
                Description = description,
                Date = DateTime.Now
            };
            model.OwnerModels.Add(ownerModel);
            owner.OwnerModels.Add(ownerModel);

            _context.OwnerModels.Add(ownerModel);
            _context.SaveChanges();
            return Ok(ownerModel);
        }
        
        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> DeleteModelFromUser(string userName, int id) // id is the id of the model;
        {
            Model model = await _context.Models.FindAsync(id);

            if (model == null)
            {
                return BadRequest("model is null");
            }

            try
            {
                Owner owner = (from u in _context.Users
                               where u.UserName == userName
                               join o in _context.Owners on u.Id equals o.UserId
                               select o).First();

                OwnerModel ownerModel = (from om in _context.OwnerModels
                                         where om.OwnerId == owner.Id && om.ModelId == id
                                         select om).First();

                _context.OwnerModels.Remove(ownerModel);
                _context.SaveChanges();
                return Ok(ownerModel);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error("Failed quering for owner", ex);
                return BadRequest("Model is already deleted");
               
            }                      
        }
    }

}
