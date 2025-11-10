using CustomerSurveyAPI.Models;

namespace CustomerSurveyAPI.Services
{
    public interface ISurveyService
    {
        Task<IEnumerable<Survey>> GetAllAsync();
        Task<Survey?> GetByIdAsync(int id);
        Task<Survey> CreateAsync(Survey survey);
        Task<bool> UpdateAsync(Survey survey);
        Task<bool> DeleteAsync(int id);
    }
}
