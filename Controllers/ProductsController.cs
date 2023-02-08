using IdentityNetCore.Data;
using IdentityNetCore.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace IdentityNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize] // applied custom attribute to make authorize check by session using key department.
    public class ProductsController : ControllerBase // ControllerBase is basically apiController that used for mvc controller without views
    {
        [Route("List")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // added jwt validation for access this resource.
        public List<Product> GetProducts()
        {
            var chair = new Product
            {
                Name = "Chair",
                Price = 100
            };

            var desk = new Product
            {
                Name = "Wooden Desk",
                Price = 150
            };

            return new List<Product>() { chair, desk };
        }
    }
}
