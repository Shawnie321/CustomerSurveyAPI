using AutoMapper;
using CustomerSurveyAPI.DTOs;
using CustomerSurveyAPI.Models;

namespace CustomerSurveyAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Survey mappings
            CreateMap<Survey, SurveyReadDto>();
            CreateMap<SurveyCreateDto, Survey>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions ?? new List<SurveyQuestionCreateDto>()));

            // SurveyQuestion mappings
            CreateMap<SurveyQuestion, SurveyQuestionReadDto>()
                .ForMember(dest => dest.AnswerCount, opt => opt.MapFrom(src => src.Answers != null ? src.Answers.Count : 0));
            CreateMap<SurveyQuestionCreateDto, SurveyQuestion>();
            CreateMap<SurveyQuestionUpdateDto, SurveyQuestion>();

            // SurveyResponse mappings
            CreateMap<SurveyResponse, SurveyResponseReadDto>();
            CreateMap<SurveyResponseCreateDto, SurveyResponse>();

            // SurveyAnswer mappings
            CreateMap<SurveyAnswer, SurveyAnswerReadDto>();
            CreateMap<SurveyAnswerCreateDto, SurveyAnswer>();

            // Submit payload mappings (for response submissions)
            CreateMap<SubmitResponseDto, SurveyResponse>()
                .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers ?? new List<SubmitAnswerDto>()));
            CreateMap<SubmitAnswerDto, SurveyAnswer>();

            // User mappings
            CreateMap<UserCreateDto, User>();
            CreateMap<User, UserReadDto>();
        }
    }
}
