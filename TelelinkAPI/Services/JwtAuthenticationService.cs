using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TelelinkAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using TelelinkAPI.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace TelelinkAPI.Services
{
    public class JwtAuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtAuthenticationService( 
            IConfiguration config, 
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _context = context;
            _userManager = userManager;

        }
        public async Task<String> GenerateJsonWebToken(ApplicationUser appUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(5)).ToUnixTimeSeconds().ToString())
            };

            var roleNames = await _userManager.GetRolesAsync(appUser);

            foreach (var roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }
          
        
            var token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"])),
                    SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));
                    
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
