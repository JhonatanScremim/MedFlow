using MedFlow.Application.Contracts.Auth;
using MedFlow.Application.Exceptions;
using MedFlow.Application.UseCases.Auth.Interfaces;
using MedFlow.Domain.Entities;
using MedFlow.Infrastructure.Auth;
using MedFlow.Infrastructure.Persistence.Interfaces;
using MedFlow.Infrastructure.Security;

namespace MedFlow.Application.UseCases.Auth;

public sealed class RegisterUseCase(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IBaseRepository<User> userRepositoryCrud,
    IBaseRepository<Role> roleRepositoryCrud,
    IBaseRepository<Doctor> doctorRepositoryCrud,
    IBaseRepository<Patient> patientRepositoryCrud,
    IUserPasswordHasher passwordHasher,
    IJwtTokenIssuer jwtTokenIssuer) : IRegisterUseCase
{
    public async Task<AuthTokenResult> ExecuteAsync(RegisterRequest request)
    {
        if (request.Role is null)
            throw new BadRequestException("Informe a role: Doctor ou Patient.");

        var (normalizedEmail, pwd) = AuthValidation.ValidateAndNormalize(request.Email, request.Password);

        if (await userRepository.EmailExistsAsync(normalizedEmail))
            throw new BadRequestException("Este e-mail já está cadastrado.");

        var registrationRole = request.Role!.Value;
        var roleName = registrationRole.ToString();
        var role = await GetOrCreateRoleAsync(roleName);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(pwd),
            CreatedAt = DateTimeOffset.UtcNow
        };

        user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
        await userRepositoryCrud.CreateAsync(user);

        Guid? doctorId = null;
        Guid? patientId = null;

        if (registrationRole is RegistrationRole.Doctor)
        {
            var doctor = await doctorRepositoryCrud.CreateAsync(new Doctor
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTimeOffset.UtcNow
            });
            doctorId = doctor.Id;
        }

        if (registrationRole is RegistrationRole.Patient)
        {
            var patient = await patientRepositoryCrud.CreateAsync(new Patient
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                IsAnonymous = false,
                CreatedAt = DateTimeOffset.UtcNow
            });
            patientId = patient.Id;
        }

        return jwtTokenIssuer.Issue(new JwtSubjectData(user.Id, user.Email, [role.Name], doctorId, patientId));
    }

    private async Task<Role> GetOrCreateRoleAsync(string name)
    {
        var existing = await roleRepository.GetByNameAsync(name);
        if (existing is not null)
            return existing;

        var role = new Role { Id = Guid.NewGuid(), Name = name };
        await roleRepositoryCrud.CreateAsync(role);
        return role;
    }
}
