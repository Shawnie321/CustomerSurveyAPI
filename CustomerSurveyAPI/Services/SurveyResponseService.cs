using CustomerSurveyAPI.Data;
using CustomerSurveyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerSurveyAPI.Services
{
    public class SurveyResponseService : ISurveyResponseService
    {
        private readonly AppDbContext _context;

        public SurveyResponseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SurveyResponse>> GetAllAsync()
        {
            return await _context.SurveyResponses
                .Include(r => r.Answers)
                    .ThenInclude(a => a.SurveyQuestion)
                .ToListAsync();
        }

        public async Task<IEnumerable<SurveyResponse>> GetBySurveyIdAsync(int surveyId)
        {
            return await _context.SurveyResponses
                .Where(r => r.SurveyId == surveyId)
                .Include(r => r.Answers)
                    .ThenInclude(a => a.SurveyQuestion)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();
        }

        public async Task<SurveyResponse?> GetByIdAsync(int id)
        {
            return await _context.SurveyResponses
                .Include(r => r.Answers)
                    .ThenInclude(a => a.SurveyQuestion)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<SurveyResponse> CreateAsync(SurveyResponse response)
        {
            _context.SurveyResponses.Add(response);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<bool> UpdateAsync(SurveyResponse response)
        {
            var existing = await _context.SurveyResponses.FindAsync(response.Id);
            if (existing == null) return false;

            existing.Username = response.Username;
            existing.SubmittedAt = response.SubmittedAt;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.SurveyResponses
                .Include(r => r.Answers)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existing == null) return false;

            if (existing.Answers != null && existing.Answers.Any())
                _context.SurveyAnswers.RemoveRange(existing.Answers);

            _context.SurveyResponses.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
