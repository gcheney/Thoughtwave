using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Thoughtwave.Models;
using Thoughtwave.ExtensionMethods;

namespace Thoughtwave.Data
{
    public class ThoughtwaveRepository : IThoughtwaveRepository
    {
        private ThoughtwaveDbContext _context;

        public ThoughtwaveRepository(ThoughtwaveDbContext context)
        {
            _context = context;
        }
        #region Thought GET Methods
    
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

        public async Task<Thought> GetThoughtByIdAsync(int id)
        {
            return await _context.Thoughts
                .Where(t => t.Id == id)
                .Include(t => t.Author)
                .SingleOrDefaultAsync();
        }

        public async Task<Thought> GetThoughtAndCommentsByIdAsync(int id)
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

        /* GET THOUGHTS BY TAG */

        public async Task<List<Thought>> GetThoughtsByTagAsync(string tag)
        {
            return await _context.Thoughts
                .Where(t => t.Tags.Contains(tag))
                .OrderByDescending(t => t.CreatedOn)
                .Include(t => t.Author)
                .ToListAsync();
        }


        /* GET THOUGHTS BY QUERY */

        public async Task<List<Thought>> GetThoughtsByQueryAsync(string query)
        {
            return await _context.Thoughts
                .Where(t => (t.Title.Contains(query, true)) || 
                    (t.Author.FullName.Contains(query, true)) ||
                    (t.Author.UserName.Contains(query, true)) ||
                    (t.Tags.Contains(query, true)))
                .Include(t => t.Author)
                .ToListAsync();
        }

         /* GET THOGUHTS BY QUERY AND CATEGORY */

        public async Task<List<Thought>> GetThoughtsByQueryAsync(string query, Category category)
        {
            return await _context.Thoughts
                .Where(t => t.Category == category)
                .Where(t => (t.Title.Contains(query, true)) || 
                    (t.Author.FullName.Contains(query, true)) ||
                    (t.Author.UserName.Contains(query, true)) ||
                    (t.Tags.Contains(query, true)))
                .OrderByDescending(t => t.CreatedOn)
                .Include(t => t.Author)
                .ToListAsync();
        }

        /* GET THOUGHTS BY USERNAME */

        public async Task<List<Thought>> GetThoughtsByUserNameAsync(string username)
        {
            return await _context.Thoughts  
                .Where(t => t.Author.UserName == username)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();
        }

        #endregion

        #region User GET Methods
        /* GET ALL USERS */

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderByDescending(u => u.SignUpDate)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsBanned == false)
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

        #endregion

        #region Thought CRUD Methods

        /* ADD A NEW THOUGHT */

        public async void AddThoughtAsync(Thought thought)
        {
             await _context.Thoughts.AddAsync(thought);
        }

        /* UPDATE A THOUGHT */

        public void UpdateThought(Thought thought)
        {
            _context.Thoughts.Update(thought);
        }

        /* Delete A Thought */

        public void DeleteThought(Thought thought)
        {
            _context.Thoughts.Remove(thought);
        }

        #endregion

        #region Comment CRUD Methods

        /* GET A COMMENT BY ID */
        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments  
                .Where(c => c.Id == id)
                .SingleOrDefaultAsync();
        }

        /* ADD A COMMENT */

        public async Task<bool> AddCommentAsync(int thoughtId, Comment comment)
        {
            var thought = await _context.Thoughts
                .Where(t => t.Id == thoughtId)
                .Include(t => t.Comments)
                .SingleOrDefaultAsync();

            if (thought != null)
            {
                thought.Comments.Add(comment);
                return true;
            }

            return false;
        } 

         /* REMOVE A COMMENT */

        public async Task<bool> RemoveCommentAsync(int thoughtId, Comment comment)
        {   
            var thought = await _context.Thoughts
                .Where(t => t.Id == thoughtId)
                .Include(t => t.Comments)
                .SingleOrDefaultAsync();
                
            if (thought != null)
            {
                _context.Comments.Remove(comment);
                thought.Comments.Remove(comment);
                return true;
            }

            return false;
        } 

        #endregion

        #region Commit
        /* SAVE CHANGES */

        public async Task<bool> CommitChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        #endregion
    }
}