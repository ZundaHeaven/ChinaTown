using AutoMapper;
using ChinaTown.Application.Dto.Book;
using ChinaTown.Domain.Entities;

namespace ChinaTown.Application.Mappers;

public class BookMapper : Profile
{
    public BookMapper()
    {
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.Genres, 
                opt => opt.MapFrom(src => src.BookGenres.ToList()))
            .ForMember(dest => dest.Username, 
                opt => opt.MapFrom(src => src.Author.Username))
            .ForMember(dest => dest.LikesCount, 
                opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentsCount, 
                opt => opt.MapFrom(src => src.Comments.Count));
        CreateMap<BookDto, Book>()
            .ForMember(dest => dest.BookGenres,
                opt => opt.Ignore())
            .ForMember(dest => dest.Author,
                opt => opt.Ignore())
            .ForMember(dest => dest.Likes,
                opt => opt.Ignore())
            .ForMember(dest => dest.Comments,
                opt => opt.Ignore());
    }
}