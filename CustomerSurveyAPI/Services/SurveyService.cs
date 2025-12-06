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
            var existing = await _context.Surveys
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == survey.Id);

            if (existing == null) return false;

            existing.Title = survey.Title;
            existing.Description = survey.Description;

            if (survey.Questions != null)
            {
                var incoming = survey.Questions.ToList();

                foreach (var inc in incoming)
                {
                    if (inc.Id > 0)
                    {
                        var existQ = existing.Questions.FirstOrDefault(q => q.Id == inc.Id);
                        if (existQ != null)
                        {
                            existQ.QuestionText = inc.QuestionText;
                            existQ.QuestionType = inc.QuestionType;
                            existQ.Options = inc.Options;
                            existQ.IsRequired = inc.IsRequired;
                        }
                        else
                        {
                            existing.Questions.Add(new SurveyQuestion
                            {
                                QuestionText = inc.QuestionText,
                                QuestionType = inc.QuestionType,
                                Options = inc.Options,
                                IsRequired = inc.IsRequired
                            });
                        }
                    }
                    else
                    {
                        existing.Questions.Add(new SurveyQuestion
                        {
                            QuestionText = inc.QuestionText,
                            QuestionType = inc.QuestionType,
                            Options = inc.Options,
                            IsRequired = inc.IsRequired
                        });
                    }
                }

                var incomingIds = incoming.Where(i => i.Id > 0).Select(i => i.Id).ToHashSet();
                var toRemove = existing.Questions
                    .Where(q => q.Id > 0 && !incomingIds.Contains(q.Id))
                    .ToList();

                foreach (var rem in toRemove)
                {
                    _context.Remove(rem);
                }
            }

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
