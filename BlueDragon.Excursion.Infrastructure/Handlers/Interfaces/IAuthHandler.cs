using System.Threading.Tasks;
using BlueDragon.Excursion.Infrastructure.Domain.Models;

namespace BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;

public interface IAuthHandler
{
    Task<bool> EmailExists(string email);
    Task AddUser(User user);
    Task<User> GetUserByCredentials(string email, string passwordHash);
    Task<User> GetUserByApiKey(string apiKey);
}
