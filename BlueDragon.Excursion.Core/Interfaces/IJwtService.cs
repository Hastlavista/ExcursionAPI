using System;

namespace BlueDragon.Excursion.Core.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email);
}
