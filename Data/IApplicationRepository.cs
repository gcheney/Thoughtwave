using System.Collections.Generic;
using Sophophile.Models;

namespace Sophophile.Data
{
    public interface IApplicationRepository
    {
        IEnumerable<Question> GetAllQuestions();
        Question GetQuestionById(int id);
    }
}