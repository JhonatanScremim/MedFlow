using System.Diagnostics.CodeAnalysis;
using MedFlow.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace MedFlow.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected async Task<IActionResult> ExecuteSafeAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (AuthenticationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Ocorreu um erro ao processar a requisição.",
                error = ex.Message
            });
        }
    }
}
