using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Auth;

namespace BlueDragon.Excursion.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> Register(RegisterRequest request);
    Task<AuthResponse> Login(LoginRequest request);
}
