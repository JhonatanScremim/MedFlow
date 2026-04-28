using MedFlow.Application.Contracts.Exam;
using MedFlow.Application.UseCases.Exam.Interfaces;
using MedFlow.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedFlow.Api.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Patient")]
public sealed class ExamController(ICreateExamUseCase createExamUseCase) : BaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateExamResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> Create([FromBody] CreateExamRequest request) =>
        ExecuteSafeAsync(async () =>
        {
            var patientId = User.GetPatientId();
            if (patientId is null)
                return Unauthorized(new { message = "Token não possui claim patientId." });

            var createdExam = await createExamUseCase.ExecuteAsync(patientId.Value, request);
            return StatusCode(StatusCodes.Status201Created, createdExam);
        });
}
