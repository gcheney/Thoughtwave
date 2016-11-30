using System.Collections.Generic;
using System.Threading.Tasks;
using Sophophile.Models;

namespace Sophophile.Data
{
    public interface IApplicationRepository
    {
        IEnumerable<Question> GetRecentQuestions();
        Task<List<Question>> GetRecentQuestionsAsync();

        IEnumerable<Question> GetAllQuestions();
        Task<List<Question>> GetAllQuestionsAsync();

        Question GetQuestionById(int id);
        Task<Question> GetQuestionByIdAsync(int id);
    }
}