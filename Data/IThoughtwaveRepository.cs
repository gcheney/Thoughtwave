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

        Task<List<Thought>> GetThoughtsByCategoryAsync(Category category);

        Task<List<Thought>> GetThoughtsByQueryAsync(string query);

        Task<List<Thought>> GetThoughtsByQueryAsync(string query, Category category);

        Task<List<User>> GetAllUsersAsync();

        Task<User> GetUserActivityByUserNameAsync(string username);

        Task<List<Thought>> GetThoughtsByUserNameAsync(string username);
    }
}