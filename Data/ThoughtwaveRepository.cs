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

        /* GET ALL THOUGHTS */

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

        /* GET RECENET THOUGHTS */

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


        /* GET THOUGHTS BY ID */

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


        /* GET THOUGHTs BY CATEGORY */

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


        /* GET THOUGHTS BY QUERY */

        public IEnumerable<Thought> GetThoughtsByQuery(string query)
        {
            query = query.ToLower();

            return _context.Thoughts
                .Where(t => t.Title.ToLower().Contains(query)
                    || t.Author.FullName.ToLower().Contains(query)
                    || t.Author.UserName.ToLower().Contains(query)
                    || t.Content.ToLower().Contains(query))
                .Include(t => t.Author)
                .ToList();
        }

        public async Task<List<Thought>> GetThoughtsByQueryAsync(string query)
        {
            query = query.ToLower();

            return await _context.Thoughts
                .Where(t => t.Title.ToLower().Contains(query)
                    || t.Author.FullName.ToLower().Contains(query)
                    || t.Author.UserName.ToLower().Contains(query)
                    || t.Content.ToLower().Contains(query))
                .Include(t => t.Author)
                .ToListAsync();
        }

         /* GET THOGUHTS BY QUERY AND CATEGORY */

        public IEnumerable<Thought> GetThoughtsByQuery(string query, Category category)
        {
            query = query.ToLower();

            return _context.Thoughts
                .Where(t => t.Category == category)
                .Where(t => t.Title.ToLower().Contains(query)
                    || t.Author.FullName.ToLower().Contains(query)
                    || t.Author.UserName.ToLower().Contains(query)
                    || t.Content.ToLower().Contains(query))
                .Include(t => t.Author)
                .ToList();
        }

        public async Task<List<Thought>> GetThoughtsByQueryAsync(string query, Category category)
        {
            query = query.ToLower();

            return await _context.Thoughts
                .Where(t => t.Category == category)
                .Where(t => t.Title.ToLower().Contains(query)
                    || t.Author.FullName.ToLower().Contains(query)
                    || t.Author.UserName.ToLower().Contains(query)
                    || t.Content.ToLower().Contains(query))
                .Include(t => t.Author)
                .ToListAsync();
        }


        /* GET ALL USERS */

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users
                .OrderByDescending(u => u.SignUpDate)
                .ToList();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderByDescending(u => u.SignUpDate)
                .ToListAsync();
        }

        /* GET USER BY USERNAME */

        public User GetUserByUserName(string username)
        {
            return _context.Users   
                .Where(u => u.UserName == username)
                .Include(u => u.Thoughts)
                .Include(u => u.Comments)
                    .ThenInclude(c => c.Thought)
                .FirstOrDefault();
        }

        public async Task<User> GetUserByUserNameAsync(string username)
        {
            return await _context.Users   
                .Where(u => u.UserName == username)
                .Include(u => u.Thoughts)
                .Include(u => u.Comments)
                    .ThenInclude(c => c.Thought)
                .FirstOrDefaultAsync();
        }
    }
}