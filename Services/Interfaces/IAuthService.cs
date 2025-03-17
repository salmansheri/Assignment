using Assignment.Dto;
using Assignment.Models;

namespace Assignment.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request); 
    }
}