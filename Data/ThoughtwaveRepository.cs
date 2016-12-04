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
    public class ThoughtwaveRepository : IThoughtwaveRepository
    {
        private ThoughtwaveDbContext _context;
        private ILogger<ThoughtwaveRepository> _logger;

        public ThoughtwaveRepository(ThoughtwaveDbContext context, 
            ILogger<ThoughtwaveRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Article> GetAllArticles()
        {
            return _context.Articles
                .Include(a => a.Author)
                .OrderByDescending(q => q.CreatedOn)
                .ToList();
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _context.Articles
                .Include(a => a.Author)
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();
        }

        public IEnumerable<Article> GetRecentArticles()
        {
            return _context.Articles
                .Include(a => a.Author)
                .OrderByDescending(q => q.CreatedOn)
                .Take(3)
                .ToList();
        }

        public async Task<List<Article>> GetRecentArticlesAsync()
        {
            return await _context.Articles
                .Include(a => a.Author)
                .OrderByDescending(a => a.CreatedOn)
                .Take(3)
                .ToListAsync();
        }

        public Article GetArticleById(int id)
        {
            return _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .Where(a => a.Id == id)
                .FirstOrDefault();
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            return await _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}