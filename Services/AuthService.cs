using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Assignment.Data;
using Assignment.Dto;
using Assignment.Models;
using Assignment.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Assignment.Services
{
    public class AuthService(ApplicationDbContext context, IConfiguration configuration) : IAuthService
    {
        
        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await context.User.FirstOrDefaultAsync(u => u.Email == request.Email);
             

            if (user is null)
            {
                return null; 
            }

            var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password); 


            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return null; 
            }


            var response = new TokenResponseDto()
            {
                AccessToken = CreateToken(user)
            }; 

            return response; 
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.User.AnyAsync(u => u.Email == request.Email))
            {
                return null; 
            }

            var user = new User(); 
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password); 

            user.Email = request.Email;
            user.PasswordHash = hashedPassword;

            context.User.Add(user); 
            await context.SaveChangesAsync(); 
            return user; 
        }

        private string CreateToken(User user)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                
            }; 

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!)
            ); 

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512); 

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:audience"),
                claims: claim,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            ); 

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor); 
        }
    }
}