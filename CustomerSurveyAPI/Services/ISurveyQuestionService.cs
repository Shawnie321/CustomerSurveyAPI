using CustomerSurveyAPI.Models;

namespace CustomerSurveyAPI.Services
{
    public interface ISurveyQuestionService
    {
        Task<IEnumerable<SurveyQuestion>> GetAllAsync();
        Task<IEnumerable<SurveyQuestion>> GetBySurveyIdAsync(int surveyId);
        Task<SurveyQuestion?> GetByIdAsync(int id);
        Task<SurveyQuestion> CreateAsync(SurveyQuestion question);
        Task<bool> UpdateAsync(SurveyQuestion question);
        Task<bool> DeleteAsync(int id);
    }
}
