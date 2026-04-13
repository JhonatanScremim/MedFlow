using MedFlow.Application.Contracts.Auth;
using MedFlow.Application.UseCases.Auth.Interfaces;
using MedFlow.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
namespace MedFlow.Api.Controllers;

[Route("api/[controller]")]
public sealed class AuthController(
    IRegisterUseCase registerUseCase,
    ILoginUseCase loginUseCase) : BaseController
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthTokenResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Register([FromBody] RegisterRequest request) =>
        ExecuteSafeAsync(async () =>
        {
            var token = await registerUseCase.ExecuteAsync(request);
            return Ok(token);
        });

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthTokenResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<IActionResult> Login([FromBody] LoginRequest request) =>
        ExecuteSafeAsync(async () =>
        {
            var token = await loginUseCase.ExecuteAsync(request);
            return Ok(token);
        });
}
