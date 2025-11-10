using CustomerSurveyAPI.Data;
using CustomerSurveyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerSurveyAPI.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly AppDbContext _context;

        public SurveyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Survey>> GetAllAsync()
        {
            return await _context.Surveys
                .Include(s => s.Questions)
                .Include(s => s.Responses)
                .ToListAsync();
        }

        public async Task<Survey?> GetByIdAsync(int id)
        {
            return await _context.Surveys
                .Include(s => s.Questions)
                .Include(s => s.Responses)
                .ThenInclude(r => r.Answers)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Survey> CreateAsync(Survey survey)
        {
            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();
            return survey;
        }

        public async Task<bool> UpdateAsync(Survey survey)
        {
            var existing = await _context.Surveys.FindAsync(survey.Id);
            if (existing == null) return false;

            existing.Title = survey.Title;
            existing.Description = survey.Description;
            // do not overwrite navigation collections here

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Surveys.FindAsync(id);
            if (existing == null) return false;

            _context.Surveys.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
