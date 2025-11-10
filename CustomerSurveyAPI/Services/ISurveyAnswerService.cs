using CustomerSurveyAPI.Models;

namespace CustomerSurveyAPI.Services
{
    public interface ISurveyAnswerService
    {
        Task<IEnumerable<SurveyAnswer>> GetAllAsync();
        Task<SurveyAnswer?> GetByIdAsync(int id);
        Task<SurveyAnswer> CreateAsync(SurveyAnswer answer);
        Task<bool> UpdateAsync(SurveyAnswer answer);
        Task<bool> DeleteAsync(int id);

        // convenience methods
        Task DeleteByQuestionIdAsync(int questionId);
        Task DeleteByResponseIdAsync(int responseId);
    }
}
