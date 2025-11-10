using CustomerSurveyAPI.Data;
using CustomerSurveyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerSurveyAPI.Services
{
    public class SurveyAnswerService : ISurveyAnswerService
    {
        private readonly AppDbContext _context;

        public SurveyAnswerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SurveyAnswer>> GetAllAsync()
        {
            return await _context.SurveyAnswers
                .ToListAsync();
        }

        public async Task<SurveyAnswer?> GetByIdAsync(int id)
        {
            return await _context.SurveyAnswers.FindAsync(id);
        }

        public async Task<SurveyAnswer> CreateAsync(SurveyAnswer answer)
        {
            _context.SurveyAnswers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task<bool> UpdateAsync(SurveyAnswer answer)
        {
            var existing = await _context.SurveyAnswers.FindAsync(answer.Id);
            if (existing == null) return false;

            existing.AnswerText = answer.AnswerText;
            existing.RatingValue = answer.RatingValue;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.SurveyAnswers.FindAsync(id);
            if (existing == null) return false;

            _context.SurveyAnswers.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteByQuestionIdAsync(int questionId)
        {
            var answers = await _context.SurveyAnswers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();

            if (!answers.Any()) return;

            _context.SurveyAnswers.RemoveRange(answers);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByResponseIdAsync(int responseId)
        {
            var answers = await _context.SurveyAnswers
                .Where(a => a.SurveyResponseId == responseId)
                .ToListAsync();

            if (!answers.Any()) return;

            _context.SurveyAnswers.RemoveRange(answers);
            await _context.SaveChangesAsync();
        }
    }
}
