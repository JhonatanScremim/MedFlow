using MedFlow.Application.Contracts.Exam;
using MedFlow.Application.UseCases.Exam.Interfaces;
using MedFlow.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedFlow.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public sealed class ExamController(
    ICreateExamUseCase createExamUseCase,
    IListExamsUseCase listExamsUseCase,
    IUpdateExamStatusUseCase updateExamStatusUseCase) : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExamResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<IActionResult> List() =>
        ExecuteSafeAsync(async () =>
        {
            var exams = await listExamsUseCase.ExecuteAsync(User.GetPatientId(), User.GetDoctorId());
            return Ok(exams);
        });

    [HttpPost]
    [Authorize(Roles = "Patient")]
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

    [HttpPut("{examId:guid}/status")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(ExamResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> UpdateStatus(Guid examId, [FromBody] UpdateExamStatusRequest request) =>
        ExecuteSafeAsync(async () =>
        {
            var doctorId = User.GetDoctorId();
            if (doctorId is null)
                return Unauthorized(new { message = "Token não possui claim doctorId." });

            var updatedExam = await updateExamStatusUseCase.ExecuteAsync(doctorId.Value, examId, request);
            return Ok(updatedExam);
        });
}
