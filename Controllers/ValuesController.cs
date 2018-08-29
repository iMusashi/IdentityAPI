using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityAPI.Controllers
{
    public class clsModel
    {
        public string username { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] clsModel model)
        {
            var username = model.username;
            //var password = context.Request.Form["password"];

            //Ideally, the GetIdentity method should be async since we are getting the values from database
            var identity = GetIdentity(username);
            if (identity == null)
            {

                return Unauthorized();
            }

            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, "Keshav"),
            };
            //var issuer = Configuration["Jwt:Issuer"];
            //var audience = Configuration["Jwt:Audience"];
            //var key = Configuration["Jwt:Key"];
            //var tokenString = GenerateJSONWebToken(new ClaimsIdentity(claims), issuer, audience, key);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: "Identity",
                audience: "Api",
                claims: claims,
                notBefore: now,
                expires: now.Add(new TimeSpan(0, 5, 0)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("WMS_Volkswagen_2.0")), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            return Ok(encodedJwt);
    //        var response = new
    //        {
    //            access_token = encodedJwt,
    //            expires_in = (int)_options.Expiration.TotalSeconds
    //        };

    //        // Serialize and return the response
    //        context.Response.ContentType = "application/json";
    //        context.Response.Cookies.Append(
    //    "x",
    //    encodedJwt,
    //    new CookieOptions()
    //    {
    //        Path = "/"
    //    }
    //);
    //        await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private Task<ClaimsIdentity> GetIdentity(string username)
        {
            // DON'T do this in production, obviously!
            if (username == "Keshav")
            {
                return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(username, "Token"), new Claim[] { }));
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
