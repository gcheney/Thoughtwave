using System.Collections.Generic;
using Sophophile.Models;

namespace Sophophile.ViewModels.HomeViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<Question> Questions { get; set; }
    }
}
