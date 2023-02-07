using IdentityNetCore.Data;
using IdentityNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiSecurityController : ControllerBase
    {
        private readonly UserManager<SystemUser> _userManager;
        private readonly SignInManager<SystemUser> _signInManager;
        private readonly IConfiguration _configuration;

        public ApiSecurityController(UserManager<SystemUser> userManager, SignInManager<SystemUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [AllowAnonymous] // allowAnonymous first to let the user request token for authentication.
        [Route("Auth")]
        [HttpPost]
        public async Task<IActionResult> AuthToken(SignInDTO signIn)
        {
            // following are the steps we need to take:
            // 1. Need to signin user that comes with signIn modal/dto.
            // 2. Once user signedIn I need to create a token.
            // 3. Then I need to identify following things:
            //      a. Issuer -> One who is generating the token.
            //      b. Audiance -> Application/Client which will recieve this token, and for my case as this
            //      .. project will recieve data and this is the one who contains api that issue toke so both
            //      .. issuer and audiance are same.
            //      c. Key -> 16bits string.
            //      d. Claims -> List of JwtRegisteredClaimNames required for/by signedIn user.
            //      f. After obtaining/gathering a,b,c,d do following:
            //          1. Encode 16chars key in bytes.
            //          2. Convert these bytes into SymmetricSecurityKey().
            //          3. Create signing creditials by SigningCredentials() using SymmetricSecurityKey and
            //          .. some Security Algo e.g. SecurityAlgorith.HmacSha256.
            //          4. Expiry -> Time after which token will expire.
            //          5. Create Token by JwtSecurityToken(issuer: a, audiance: b, key: d, expiry: f->4,
            //          .. signingCredentials: f->3) steps details.
            //          6. Serialize token using JwtSecurityTokenHandler().WriteToken() so that we can use it.
            // 5. Normally for Issuer we put the base address of application/api. 
            // 6. Add required (issuer, audiance, key) in appsettings.json.

            var issuer = _configuration["JWT:Issuer"];
            var audiance = _configuration["JWT:Audiance"];
            var key = _configuration["JWT:KEY"];

            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(signIn.FullName, signIn.Password, signIn.RememberMe, false);

                if (signInResult.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(signIn.FullName);
                    
                    if (user != null)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Jti, user.Id), // jti is used for id:string
                            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        };

                        var keyBytes = Encoding.UTF8.GetBytes(key);
                        var symmetricKey = new SymmetricSecurityKey(keyBytes);
                        var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);
                        var expiry = DateTime.Now.AddMinutes(10); //TimeSpan.FromDays(1);
                        var token = new JwtSecurityToken(issuer, audiance, claims, expires: expiry, signingCredentials: signingCredentials);

                        return Ok(new
                        {
                            jwtToken = new JwtSecurityTokenHandler().WriteToken(token)
                        }); ;
                    }
                }
            }

            return BadRequest();
        }
    }
}
