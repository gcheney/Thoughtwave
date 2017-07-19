using AutoMapper;
using System.Net;
using Thoughtwave.Models;
using Thoughtwave.ExtensionMethods;
using Thoughtwave.ViewModels.ThoughtViewModels;
using Thoughtwave.ViewModels.ManageViewModels;

namespace Thoughtwave.Infrastructure
{
    public class AutoMapperProfileConfiguration : Profile
    {
        protected override void Configure()
        {
            CreateMap<Thought, ThoughtViewModel>().ReverseMap();

            CreateMap<CreateThoughtViewModel, Thought>()
                .BeforeMap((src, dest) => src.Content = WebUtility.HtmlEncode(src.Content))
                .BeforeMap((src, dest) => src.Tags = src.Tags.RemoveWhiteSpaces());

            CreateMap<EditThoughtViewModel, Thought>()
                .BeforeMap((src, dest) => src.Content = WebUtility.HtmlEncode(src.Content))
                .BeforeMap((src, dest) => src.Tags = src.Tags.RemoveWhiteSpaces())
                .ReverseMap()
                .AfterMap((src, dest) => dest.Content = WebUtility.HtmlDecode(dest.Content));

            CreateMap<EditProfileViewModel, User>().ReverseMap();
        }
    }
}