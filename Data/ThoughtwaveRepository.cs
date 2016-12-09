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

        /* GET ALL ARTICLES */

        public IEnumerable<Thought> GetAllThoughts()
        {
            return _context.Thoughts
                .Include(a => a.Author)
                .OrderByDescending(q => q.CreatedOn)
                .ToList();
        }

        public async Task<List<Thought>> GetAllThoughtsAsync()
        {
            return await _context.Thoughts
                .Include(a => a.Author)
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();
        }

        /* GET RECENET ARTICLES */

        public IEnumerable<Thought> GetRecentThoughts()
        {
            return _context.Thoughts
                .Include(a => a.Author)
                .OrderByDescending(q => q.CreatedOn)
                .Take(3)
                .ToList();
        }

        public async Task<List<Thought>> GetRecentThoughtsAsync()
        {
            return await _context.Thoughts
                .Include(a => a.Author)
                .OrderByDescending(a => a.CreatedOn)
                .Take(3)
                .ToListAsync();
        }


        /* GET ARTICLE BY ID */

        public Thought GetThoughtById(int id)
        {
            return _context.Thoughts
                .Where(a => a.Id == id)
                .Include(a => a.Author)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefault();
        }

        public async Task<Thought> GetThoughtByIdAsync(int id)
        {
            return await _context.Thoughts
                .Where(a => a.Id == id)
                .Include(a => a.Author)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync();
        }


        /* GET ARTICLE BY CATEGORY */

        public IEnumerable<Thought> GetThoughtsByCategory(Category category)
        {
            return _context.Thoughts
                .Where(a => a.Category == category)
                .Include(a => a.Author)
                .ToList();
        }

        public async Task<List<Thought>> GetThoughtsByCategoryAsync(Category category)
        {
            return await _context.Thoughts
                .Where(a => a.Category == category)
                .Include(a => a.Author)
                .ToListAsync();
        }


        /* GET ARTICLES BY QUERY */

        public IEnumerable<Thought> GetThoughtsByQuery(string query)
        {
            return _context.Thoughts
                .Where(a => a.Title.Contains(query)
                    || a.Author.FullName.Contains(query)
                    || a.Author.UserName.Contains(query)
                    || a.Content.Contains(query))
                .Include(a => a.Author)
                .ToList();
        }

        public async Task<List<Thought>> GetThoughtsByQueryAsync(string query)
        {
            return await _context.Thoughts
                .Where(a => a.Title.Contains(query)
                    || a.Author.FullName.Contains(query)
                    || a.Author.UserName.Contains(query)
                    || a.Content.Contains(query))
                .Include(a => a.Author)
                .ToListAsync();
        }

        public IEnumerable<Thought> GetThoughtsByQuery(string query, Category category)
        {
            return _context.Thoughts
                .Where(a => a.Category == category)
                .Where(a => a.Title.Contains(query)
                    || a.Author.FullName.Contains(query)
                    || a.Content.Contains(query))
                .Include(a => a.Author)
                .ToList();
        }

        public async Task<List<Thought>> GetThoughtsByQueryAsync(string query, Category category)
        {
            return await _context.Thoughts
                .Where(a => a.Category == category)
                .Where(a => a.Title.Contains(query)
                    || a.Author.FullName.Contains(query)
                    || a.Content.Contains(query))
                .Include(a => a.Author)
                .ToListAsync();
        }
    }
}