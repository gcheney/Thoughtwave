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
                .Include(t => t.Author)
                .OrderByDescending(t => t.CreatedOn)
                .ToList();
        }

        public async Task<List<Thought>> GetAllThoughtsAsync()
        {
            return await _context.Thoughts
                .Include(t => t.Author)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();
        }

        /* GET RECENET ARTICLES */

        public IEnumerable<Thought> GetRecentThoughts()
        {
            return _context.Thoughts
                .Include(t => t.Author)
                .OrderByDescending(t => t.CreatedOn)
                .Take(3)
                .ToList();
        }

        public async Task<List<Thought>> GetRecentThoughtsAsync()
        {
            return await _context.Thoughts
                .Include(t => t.Author)
                .OrderByDescending(t => t.CreatedOn)
                .Take(3)
                .ToListAsync();
        }


        /* GET ARTICLE BY ID */

        public Thought GetThoughtById(int id)
        {
            return _context.Thoughts
                .Where(t => t.Id == id)
                .Include(t => t.Author)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefault();
        }

        public async Task<Thought> GetThoughtByIdAsync(int id)
        {
            return await _context.Thoughts
                .Where(t => t.Id == id)
                .Include(t => t.Author)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync();
        }


        /* GET ARTICLE BY CATEGORY */

        public IEnumerable<Thought> GetThoughtsByCategory(Category category)
        {
            return _context.Thoughts
                .Where(t => t.Category == category)
                .Include(t => t.Author)
                .ToList();
        }

        public async Task<List<Thought>> GetThoughtsByCategoryAsync(Category category)
        {
            return await _context.Thoughts
                .Where(t => t.Category == category)
                .Include(t => t.Author)
                .ToListAsync();
        }


        /* GET ARTICLES BY QUERY */

        public IEnumerable<Thought> GetThoughtsByQuery(string query)
        {
            return _context.Thoughts
                .Where(t => t.Title.Contains(query)
                    || t.Author.FullName.Contains(query)
                    || t.Author.UserName.Contains(query)
                    || t.Content.Contains(query))
                .Include(t => t.Author)
                .ToList();
        }

        public async Task<List<Thought>> GetThoughtsByQueryAsync(string query)
        {
            return await _context.Thoughts
                .Where(t => t.Title.Contains(query)
                    || t.Author.FullName.Contains(query)
                    || t.Author.UserName.Contains(query)
                    || t.Content.Contains(query))
                .Include(t => t.Author)
                .ToListAsync();
        }

        public IEnumerable<Thought> GetThoughtsByQuery(string query, Category category)
        {
            return _context.Thoughts
                .Where(t => t.Category == category)
                .Where(t => t.Title.Contains(query)
                    || t.Author.FullName.Contains(query)
                    || t.Author.UserName.Contains(query)
                    || t.Content.Contains(query))
                .Include(t => t.Author)
                .ToList();
        }

        public async Task<List<Thought>> GetThoughtsByQueryAsync(string query, Category category)
        {
            return await _context.Thoughts
                .Where(t => t.Category == category)
                .Where(t => t.Title.Contains(query)
                    || t.Author.FullName.Contains(query)
                    || t.Author.UserName.Contains(query)
                    || t.Content.Contains(query))
                .Include(t => t.Author)
                .ToListAsync();
        }
    }
}