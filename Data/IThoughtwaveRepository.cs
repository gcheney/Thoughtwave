using System.Collections.Generic;
using System.Threading.Tasks;
using Thoughtwave.Models;

namespace Thoughtwave.Data
{
    public interface IThoughtwaveRepository
    {
        IEnumerable<Article> GetAllArticles();
        Task<List<Article>> GetAllArticlesAsync();

        Article GetArticleById(int id);
        Task<Article> GetArticleByIdAsync(int id);
    }
}