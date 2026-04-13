using Microsoft.AspNetCore.Mvc;

namespace MedFlow.Api.Controllers;

[Route("[controller]")]
public sealed class HealthController : BaseController
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok" });
}
