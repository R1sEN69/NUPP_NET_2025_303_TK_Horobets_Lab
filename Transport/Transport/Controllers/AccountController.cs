using Microsoft.AspNetCore.Identity;       
using Microsoft.AspNetCore.Mvc;        
using Microsoft.IdentityModel.Tokens;   
using System.IdentityModel.Tokens.Jwt;   
using System.Security.Claims;            
using System.Text;                      
using Transport.Infrastructure.Data;     
using Transport.REST.Models;           
using Microsoft.AspNetCore.Authorization;

namespace Transport.REST.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] 
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;     
        private readonly SignInManager<ApplicationUser> _signInManager;  
        private readonly IConfiguration _configuration;                
        private readonly RoleManager<IdentityRole> _roleManager;       


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email, 
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                if (await _roleManager.RoleExistsAsync("User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
                else
                {
                    Console.WriteLine("Warning: 'User' role does not exist. User created without a default role.");
                }

                return Ok(new { Message = "Користувача успішно зареєстровано!" }); 
            }

            return BadRequest(result.Errors); 
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Неправильні облікові дані." }); 
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = "Неправильні облікові дані." }); 
            }

            var token = await GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { Message = "Користувача не знайдено." });
            }


            var roleExist = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExist)
            {
                return BadRequest(new { Message = $"Роль '{model.RoleName}' не існує." }); 
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (result.Succeeded)
            {
                return Ok(new { Message = $"Користувачу '{model.Email}' успішно призначено роль '{model.RoleName}'." }); 
            }

            return BadRequest(result.Errors); 
        }


        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey not found.");
            var validIssuer = jwtSettings["ValidIssuer"];
            var validAudience = jwtSettings["ValidAudience"];
            var expiryInMinutes = Convert.ToDouble(jwtSettings["ExpiryInMinutes"]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),       
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),     
                new Claim(ClaimTypes.Name, user.UserName!)                   
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(expiryInMinutes);

            var token = new JwtSecurityToken(
                issuer: validIssuer,
                audience: validAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}