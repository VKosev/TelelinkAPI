using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelelinkAPI.Models;

namespace TelelinkAPI.Services
{
    public interface IAuthenticationService
    {
        //
        // Summary:
        //     Generates a JSON Web Token for the ApplicationUser.
        //
        // Parameters:
        //   appUser:
        //     The user for whom to generate the token.
        //   password:
        //      The password for that user.
        // Returns:
        //     Return the generated JSON Web Token for the user. 
        public Task<string> GenerateJsonWebToken(ApplicationUser appUser);



    }
}
