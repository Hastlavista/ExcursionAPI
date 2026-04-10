using System;
using System.Threading.Tasks;
using BlueDragon.Excursion.Core.DTOs.Auth;

namespace BlueDragon.Excursion.Core.Interfaces;

public interface IAccountService
{
    Task<AuthResponse> RegenerateApiKey(Guid userId);
    Task<bool> ChangePassword(Guid userId, ChangePasswordRequest request);
    Task DeleteAccount(Guid userId);
}