using CustomerSurveyAPI.Models;

namespace CustomerSurveyAPI.Services
{
    public interface ISurveyResponseService
    {
        Task<IEnumerable<SurveyResponse>> GetAllAsync();
        Task<IEnumerable<SurveyResponse>> GetBySurveyIdAsync(int surveyId);
        Task<SurveyResponse?> GetByIdAsync(int id);
        Task<SurveyResponse?> GetBySurveyAndUsernameAsync(int surveyId, string username);
        Task<IEnumerable<int>> GetCompletedSurveyIdsByUsernameAsync(string username);
        Task<SurveyResponse> CreateAsync(SurveyResponse response);
        Task<bool> UpdateAsync(SurveyResponse response);
        Task<bool> DeleteAsync(int id);
    }
}
