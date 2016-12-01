using System.Collections.Generic;
using Thoughtwave.Models;

namespace Thoughtwave.ViewModels.HomeViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
    }
}
