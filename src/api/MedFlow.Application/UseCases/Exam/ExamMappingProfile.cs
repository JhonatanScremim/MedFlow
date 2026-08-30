using AutoMapper;
using MedFlow.Application.Contracts.Exam;
using MedFlow.Domain.Enums;
using ExamEntity = MedFlow.Domain.Entities.Exam;

namespace MedFlow.Application.UseCases.Exam;

public sealed class ExamMappingProfile : Profile
{
    public ExamMappingProfile()
    {
        CreateMap<CreateExamRequest, ExamEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.PatientId, opt => opt.MapFrom((_, _, _, context) => (Guid)context.Items["PatientId"]))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => ExamStatus.Requested))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom((_, _, _, context) => (DateTimeOffset)context.Items["Now"]))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom((_, _, _, context) => (DateTimeOffset)context.Items["Now"]));

        CreateMap<ExamEntity, CreateExamResponse>()
            .ForCtorParam("ConversationId", opt => opt.MapFrom(src => src.Conversation == null ? null : (Guid?)src.Conversation.Id));

        CreateMap<ExamEntity, ExamResponse>()
            .ForCtorParam("ConversationId", opt => opt.MapFrom(src => src.Conversation == null ? null : (Guid?)src.Conversation.Id));
    }
}
