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

        public async Task<List<Thought>> GetAllThoughtsAsync()
        {
            return await _context.Thoughts
                .Include(t => t.Author)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();
        }

        /* GET RECENET THOUGHTS */

        public async Task<List<Thought>> GetRecentThoughtsAsync()
        {
            return await _context.Thoughts
                .Include(t => t.Author)
                .OrderByDescending(t => t.CreatedOn)
                .Take(3)
                .ToListAsync();
        }


        /* GET THOUGHTS BY ID */

        public async Task<Thought> GetThoughtByIdAsync(int? id)
        {
            return await _context.Thoughts
                .Where(t => t.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<Thought> GetThoughtAndIncludesByIdAsync(int? id)
        {
            return await _context.Thoughts
                .Where(t => t.Id == id)
                .Include(t => t.Author)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .SingleOrDefaultAsync();
        }


        /* GET THOUGHTS BY CATEGORY */

        public async Task<List<Thought>> GetThoughtsByCategoryAsync(Category category)
        {
            return await _context.Thoughts
                .Where(t => t.Category == category)
                .OrderByDescending(t => t.CreatedOn)
                .Include(t => t.Author)
                .ToListAsync();
        }


        /* GET THOUGHTS BY QUERY */

        public async Task<List<Thought>> GetThoughtsByQueryAsync(string query)
        {
            query = query.ToLower();

            return await _context.Thoughts
                .Where(t => t.Title.ToLower().Contains(query)
                    || t.Author.FullName.ToLower().Contains(query)
                    || t.Author.UserName.ToLower().Contains(query)
                    || t.Content.ToLower().Contains(query))
                .OrderByDescending(t => t.CreatedOn)
                .Include(t => t.Author)
                .ToListAsync();
        }

         /* GET THOGUHTS BY QUERY AND CATEGORY */

        public async Task<List<Thought>> GetThoughtsByQueryAsync(string query, Category category)
        {
            query = query.ToLower();

            return await _context.Thoughts
                .Where(t => t.Category == category)
                .Where(t => t.Title.ToLower().Contains(query)
                    || t.Author.FullName.ToLower().Contains(query)
                    || t.Author.UserName.ToLower().Contains(query)
                    || t.Content.ToLower().Contains(query))
                .OrderByDescending(t => t.CreatedOn)
                .Include(t => t.Author)
                .ToListAsync();
        }


        /* GET ALL USERS */

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderByDescending(u => u.SignUpDate)
                .ToListAsync();
        }

        /* GET USER ACTIVITY BY USERNAME */

        public async Task<User> GetUserActivityByUserNameAsync(string username)
        {
            return await _context.Users   
                .Where(u => u.UserName == username)
                .Include(u => u.Thoughts)
                .Include(u => u.Comments)
                    .ThenInclude(c => c.Thought)
                .SingleOrDefaultAsync();
        }

        /* GET THOUGHTS BY USERNAME */

        public async Task<List<Thought>> GetThoughtsByUserNameAsync(string username)
        {
            return await _context.Thoughts  
                .Where(t => t.Author.UserName == username)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();
        }

        /* ADD A NEW THOUGHT */

        public void AddThought(Thought thought)
        {
             _context.Thoughts.Add(thought);
        }

        /* Delete A Thought */
        public void DeleteThought(Thought thought)
        {
            _context.Thoughts.Remove(thought);
        }

        /* SAVE CHANGES */

        public async Task<bool> CommitChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}