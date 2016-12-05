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

        public ThoughtwaveRepository(ThoughtwaveDbContext context)
        {
            _context = context;
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
                .Where(a => a.Id == id)
                .Include(a => a.Author)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefault();
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            return await _context.Articles
                .Where(a => a.Id == id)
                .Include(a => a.Author)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync();
        }


        public IEnumerable<Article> GetArticlesByCategory(Category category)
        {
            return _context.Articles
                .Where(a => a.Category == category)
                .Include(a => a.Author)
                .ToList();
        }

        public async Task<List<Article>> GetArticlesByCategoryAsync(Category category)
        {
            return await _context.Articles
                .Where(a => a.Category == category)
                .Include(a => a.Author)
                .ToListAsync();
        }
    }
}