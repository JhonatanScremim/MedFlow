namespace MedFlow.Infrastructure.Security;

public interface IUserPasswordHasher
{
    string Hash(string password);

    bool Verify(string passwordHash, string providedPassword);
}
