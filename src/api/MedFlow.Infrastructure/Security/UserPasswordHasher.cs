using MedFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MedFlow.Infrastructure.Security;

public sealed class UserPasswordHasher : IUserPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) =>
        _hasher.HashPassword(new User(), password);

    public bool Verify(string passwordHash, string providedPassword) =>
        _hasher.VerifyHashedPassword(new User(), passwordHash, providedPassword) is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
}
