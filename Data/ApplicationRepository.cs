using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sophophile.Models;

namespace Sophophile.Data
{
    public class ApplicationRepository : IApplicationRepository
    {
        private ApplicationDbContext _context;
        private ILogger<ApplicationRepository> _logger;

        public ApplicationRepository(ApplicationDbContext context, 
            ILogger<ApplicationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

    #region GetRecentQuestions  
        public IEnumerable<Question> GetRecentQuestions()
        {
            return _context.Questions
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedOn)
                .Take(5)
                .ToList();
        }

        public async Task<List<Question>> GetRecentQuestionsAsync()
        {
            return await _context.Questions
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedOn)
                .Take(5)
                .ToListAsync();
        }
    #endregion

    #region GetAllQuestions
        public IEnumerable<Question> GetAllQuestions()
        {
            return _context.Questions
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedOn)
                .ToList();
        }

        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            return await _context.Questions
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedOn)
                .ToListAsync();
        }
    #endregion

    #region GetQuestionById
        public Question GetQuestionById(int id)
        {
            return _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .Where(q => q.Id == id)
                .FirstOrDefault();
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
    #endregion

    }
}