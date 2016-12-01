using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thoughtwave.Models;

namespace Thoughtwave.Data
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

    #region GetRecentArticles  
        public IEnumerable<Article> GetRecentArticles()
        {
            return _context.Articles
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedOn)
                .Take(5)
                .ToList();
        }

        public async Task<List<Article>> GetRecentArticlesAsync()
        {
            return await _context.Articles
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedOn)
                .Take(5)
                .ToListAsync();
        }
    #endregion

    #region GetAllArticles
        public IEnumerable<Article> GetAllArticles()
        {
            return _context.Articles
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedOn)
                .ToList();
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _context.Articles
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedOn)
                .ToListAsync();
        }
    #endregion

    #region GetArticleById
        public Article GetArticleById(int id)
        {
            return _context.Articles
                .Include(q => q.User)
                .Include(q => q.Comments)
                    .ThenInclude(a => a.User)
                .Where(q => q.Id == id)
                .FirstOrDefault();
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            return await _context.Articles
                .Include(q => q.User)
                .Include(q => q.Comments)
                    .ThenInclude(a => a.User)
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();
        }
    #endregion

    }
}