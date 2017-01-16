using System.Collections.Generic;
using System.Threading.Tasks;
using Thoughtwave.Models;

namespace Thoughtwave.Data
{
    public interface IThoughtwaveRepository
    {
        Task<List<Thought>> GetAllThoughtsAsync();

        Task<List<Thought>> GetRecentThoughtsAsync();
        
        Task<Thought> GetThoughtByIdAsync(int id);

        Task<Thought> GetThoughtAndCommentsByIdAsync(int id);

        Task<List<Thought>> GetThoughtsByCategoryAsync(Category category);

        Task<List<Thought>> GetThoughtsByTagAsync(string tag);

        Task<List<Thought>> GetThoughtsByQueryAsync(string query);

        Task<List<Thought>> GetThoughtsByQueryAsync(string query, Category category);

        Task<List<User>> GetAllUsersAsync();

        Task<List<User>> GetAllActiveUsersAsync();

        Task<User> GetUserActivityByUserNameAsync(string username);

        Task<List<Thought>> GetThoughtsByUserNameAsync(string username);

        void AddThought(Thought thought);

        void UpdateThought(Thought thought);

        void DeleteThought(Thought thought);

        Task<Comment> GetCommentByIdAsync(int id);

        Task<bool> AddCommentAsync(int thoughtId, Comment comment);

        Task<bool> RemoveCommentAsync(int thoughtId, Comment comment);

        Task<bool> CommitChangesAsync();
    }
}