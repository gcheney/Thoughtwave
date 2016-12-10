using System.Collections.Generic;
using System.Threading.Tasks;
using Thoughtwave.Models;

namespace Thoughtwave.Data
{
    public interface IThoughtwaveRepository
    {
        IEnumerable<Thought> GetAllThoughts();
        Task<List<Thought>> GetAllThoughtsAsync();

        IEnumerable<Thought> GetRecentThoughts();
        Task<List<Thought>> GetRecentThoughtsAsync();

        Thought GetThoughtById(int id);
        Task<Thought> GetThoughtByIdAsync(int id);

        IEnumerable<Thought> GetThoughtsByCategory(Category category);
        Task<List<Thought>> GetThoughtsByCategoryAsync(Category category);

        IEnumerable<Thought> GetThoughtsByQuery(string query);
        Task<List<Thought>> GetThoughtsByQueryAsync(string query);

        IEnumerable<Thought> GetThoughtsByQuery(string query, Category category);
        Task<List<Thought>> GetThoughtsByQueryAsync(string query, Category category);

        IEnumerable<User> GetAllUsers();
        Task<List<User>> GetAllUsersAsync();
    }
}