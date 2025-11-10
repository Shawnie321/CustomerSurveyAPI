using CustomerSurveyAPI.Data;
using CustomerSurveyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerSurveyAPI.Services
{
    public class SurveyQuestionService : ISurveyQuestionService
    {
        private readonly AppDbContext _context;

        public SurveyQuestionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SurveyQuestion>> GetAllAsync()
        {
            return await _context.SurveyQuestions
                .Include(q => q.Answers)
                .ToListAsync();
        }

        public async Task<IEnumerable<SurveyQuestion>> GetBySurveyIdAsync(int surveyId)
        {
            return await _context.SurveyQuestions
                .Where(q => q.SurveyId == surveyId)
                .Include(q => q.Answers)
                .OrderBy(q => q.Id)
                .ToListAsync();
        }

        public async Task<SurveyQuestion?> GetByIdAsync(int id)
        {
            return await _context.SurveyQuestions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<SurveyQuestion> CreateAsync(SurveyQuestion question)
        {
            _context.SurveyQuestions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<bool> UpdateAsync(SurveyQuestion question)
        {
            var existing = await _context.SurveyQuestions.FindAsync(question.Id);
            if (existing == null) return false;

            existing.QuestionText = question.QuestionText;
            existing.QuestionType = question.QuestionType;
            existing.Options = question.Options;
            existing.IsRequired = question.IsRequired;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.SurveyQuestions.FindAsync(id);
            if (existing == null) return false;

            _context.SurveyQuestions.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
