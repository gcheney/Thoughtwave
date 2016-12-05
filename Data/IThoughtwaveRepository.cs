using System.Collections.Generic;
using System.Threading.Tasks;
using Thoughtwave.Models;

namespace Thoughtwave.Data
{
    public interface IThoughtwaveRepository
    {
        IEnumerable<Article> GetAllArticles();
        Task<List<Article>> GetAllArticlesAsync();

        IEnumerable<Article> GetRecentArticles();
        Task<List<Article>> GetRecentArticlesAsync();

        Article GetArticleById(int id);
        Task<Article> GetArticleByIdAsync(int id);

        IEnumerable<Article> GetArticlesByCategory(Category category);
        Task<List<Article>> GetArticlesByCategoryAsync(Category category);
    }
}