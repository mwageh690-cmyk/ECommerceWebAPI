using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using E_CommerceBussinessLayer;


namespace StudentApi.Controllers
{
    // This controller is responsible for authentication-related actions,
    // such as logging in and issuing JWT tokens.
    [ApiController]
    [Route("api/Authentacation")]
    public class AuthController : ControllerBase
    {
        // This endpoint handles user login.
        // It verifies credentials and returns a JWT token if login succeeds.
        [HttpPost("login")]
        public IActionResult Login([FromBody] clsLoginRequest request)
        {
            // Step 1: Find the student by email from the in-memory data store.
            // Email acts as the unique login identifier.
            var student = clsCustomer.Find(request.Email , request.Password);


            // If no student is found with the given email,
            // return 401 Unauthorized without revealing which field was wrong.
            if (student == null)
                return Unauthorized("Invalid credentials");


            
           


            // Step 3: Create claims that represent the authenticated user's identity.
            // These claims will be embedded inside the JWT.
            var claims = new[]
            {
                // Unique identifier for the student
                new Claim(ClaimTypes.NameIdentifier, student.CustomerID.ToString()),


                // Student email address
                new Claim(ClaimTypes.Email, student.Username),


                // Role (Student or Admin) used later for authorization
                new Claim(ClaimTypes.Role, student.Role)
            };


            // Step 4: Create the symmetric security key used to sign the JWT.
            // This key must match the key used in JWT validation middleware.
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"));


            // Step 5: Define the signing credentials.
            // This specifies the algorithm used to sign the token.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // Step 6: Create the JWT token.
            // The token includes issuer, audience, claims, expiration, and signature.
            var token = new JwtSecurityToken(
                issuer: "ECommerceApi",
                audience: "ECommerceApiUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );


            // Step 7: Return the serialized JWT token to the client.
            // The client will send this token with future requests.
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}