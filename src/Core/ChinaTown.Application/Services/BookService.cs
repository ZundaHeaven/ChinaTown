using ChinaTown.Application.Data;
using Microsoft.EntityFrameworkCore;
using ChinaTown.Application.Dto.Book;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Enums;
using ChinaTown.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ChinaTown.Application.Services;

public class BookService : IBookService
{
    private readonly ApplicationDbContext _context;
    private readonly MongoDbContext _mongoDb;

    public BookService(ApplicationDbContext context, MongoDbContext mongoDb)
    {
        _context = context;
        _mongoDb = mongoDb;
    }

    public async Task<PaginatedResult<BookDto>> GetBooksAsync(BookFilterDto filter)
    {
        var query = _context.Books
            .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
            .Include(b => b.Author)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Title))
            query = query.Where(b => b.Title.Contains(filter.Title));

        if (!string.IsNullOrEmpty(filter.AuthorName))
            query = query.Where(b => b.AuthorName.Contains(filter.AuthorName));

        if (filter.GenreIds != null && filter.GenreIds.Any())
            query = query.Where(b => b.BookGenres.Any(bg => filter.GenreIds.Contains(bg.GenreId)));

        if (filter.YearMin.HasValue)
            query = query.Where(b => b.YearOfPublish >= filter.YearMin);

        if (filter.YearMax.HasValue)
            query = query.Where(b => b.YearOfPublish <= filter.YearMax);

        if (filter.PagesMin.HasValue)
            query = query.Where(b => b.PageAmount >= filter.PagesMin);

        if (filter.PagesMax.HasValue)
            query = query.Where(b => b.PageAmount <= filter.PagesMax);

        if (filter.Available.HasValue)
            query = query.Where(b => filter.Available.Value ? b.Status == ContentStatus.Published : b.Status != ContentStatus.Published);

        query = filter.Sort switch
        {
            "year_desc" => query.OrderByDescending(b => b.YearOfPublish),
            "year_asc" => query.OrderBy(b => b.YearOfPublish),
            "created_desc" => query.OrderByDescending(b => b.CreatedOn),
            _ => query.OrderByDescending(b => b.CreatedOn)
        };

        var totalCount = await query.CountAsync();
        var books = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Slug = b.Slug,
                Excerpt = b.Excerpt ?? string.Empty,
                AuthorName = b.AuthorName,
                Description = b.Description,
                PageAmount = b.PageAmount,
                YearOfPublish = b.YearOfPublish,
                FileSizeBytes = b.FileSizeBytes,
                CreatedOn = b.CreatedOn,
                ModifiedOn = b.ModifiedOn,
                Genres = b.BookGenres.Select(bg => new GenreDto { Id = bg.Genre.Id, Name = bg.Genre.Name }).ToList(),
                CoverFileId = b.CoverFileId,
                BookFileId = b.BookFileId,
                Status = b.Status,
                UserId = b.UserId,
                Username = b.Author.Username,
                LikesCount = b.Likes.Count,
                CommentsCount = b.Comments.Count,
                ViewsCount = b.Views.Count
            })
            .ToListAsync();

        return new PaginatedResult<BookDto>
        {
            Items = books,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<BookDto> GetBookAsync(Guid id)
    {
        var book = await _context.Books
            .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            throw new NotFoundException("Book not found");

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Slug = book.Slug,
            Excerpt = book.Excerpt ?? string.Empty,
            AuthorName = book.AuthorName,
            Description = book.Description,
            PageAmount = book.PageAmount,
            YearOfPublish = book.YearOfPublish,
            FileSizeBytes = book.FileSizeBytes,
            CreatedOn = book.CreatedOn,
            ModifiedOn = book.ModifiedOn,
            Genres = book.BookGenres.Select(bg => new GenreDto { Id = bg.Genre.Id, Name = bg.Genre.Name }).ToList(),
            CoverFileId = book.CoverFileId,
            BookFileId = book.BookFileId,
            Status = book.Status,
            UserId = book.UserId,
            Username = book.Author.Username,
            LikesCount = book.Likes.Count,
            CommentsCount = book.Comments.Count,
            ViewsCount = book.Views.Count
        };
    }

    public async Task<BookDto> CreateBookAsync(BookCreateDto dto, Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        var book = new Book
        {
            Title = dto.Title,
            Slug = GenerateSlug(dto.Title),
            AuthorName = dto.AuthorName,
            Description = dto.Description,
            PageAmount = dto.PageAmount,
            YearOfPublish = dto.YearOfPublish,
            UserId = userId,
            BookFileId = Guid.Empty,
            CoverFileId = Guid.Empty,
            FileSizeBytes = 0,
            Status = ContentStatus.Draft
        };

        if (dto.GenreIds.Any())
        {
            var genres = await _context.Genres.Where(g => dto.GenreIds.Contains(g.Id)).ToListAsync();
            foreach (var genre in genres)
            {
                book.BookGenres.Add(new BookGenre { Genre = genre });
            }
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Slug = book.Slug,
            Excerpt = string.Empty,
            AuthorName = book.AuthorName,
            Description = book.Description,
            PageAmount = book.PageAmount,
            YearOfPublish = book.YearOfPublish,
            FileSizeBytes = book.FileSizeBytes,
            CreatedOn = book.CreatedOn,
            ModifiedOn = book.ModifiedOn,
            Genres = book.BookGenres.Select(bg => new GenreDto { Id = bg.Genre.Id, Name = bg.Genre.Name }).ToList(),
            CoverFileId = book.CoverFileId,
            BookFileId = book.BookFileId,
            Status = book.Status,
            UserId = book.UserId,
            Username = user.Username,
            LikesCount = 0,
            CommentsCount = 0,
            ViewsCount = 0
        };
    }

    public async Task<BookDto> UpdateBookAsync(Guid id, BookUpdateDto dto, Guid userId)
    {
        var book = await _context.Books
            .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            throw new NotFoundException("Book not found");

        if (book.UserId != userId)
            throw new UnauthorizedException("You can only update your own books");

        book.Title = dto.Title;
        book.Slug = GenerateSlug(dto.Title);
        book.AuthorName = dto.AuthorName;
        book.Description = dto.Description;
        book.PageAmount = dto.PageAmount;
        book.YearOfPublish = dto.YearOfPublish;
        book.ModifiedOn = DateTime.UtcNow;

        _context.BookGenres.RemoveRange(book.BookGenres);
        if (dto.GenreIds.Any())
        {
            var genres = await _context.Genres.Where(g => dto.GenreIds.Contains(g.Id)).ToListAsync();
            foreach (var genre in genres)
            {
                book.BookGenres.Add(new BookGenre { Genre = genre });
            }
        }

        await _context.SaveChangesAsync();

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Slug = book.Slug,
            Excerpt = book.Excerpt ?? string.Empty,
            AuthorName = book.AuthorName,
            Description = book.Description,
            PageAmount = book.PageAmount,
            YearOfPublish = book.YearOfPublish,
            FileSizeBytes = book.FileSizeBytes,
            CreatedOn = book.CreatedOn,
            ModifiedOn = book.ModifiedOn,
            Genres = book.BookGenres.Select(bg => new GenreDto { Id = bg.Genre.Id, Name = bg.Genre.Name }).ToList(),
            CoverFileId = book.CoverFileId,
            BookFileId = book.BookFileId,
            Status = book.Status,
            UserId = book.UserId,
            Username = book.Author.Username,
            LikesCount = book.Likes.Count,
            CommentsCount = book.Comments.Count,
            ViewsCount = book.Views.Count
        };
    }

    public async Task DeleteBookAsync(Guid id, Guid userId)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            throw new NotFoundException("Book not found");

        var user = await _context.Users.FindAsync(userId);
        var isAdmin = user?.Role.Name == "Admin";

        if (book.UserId != userId && !isAdmin)
            throw new UnauthorizedException("You can only delete your own books");

        if (book.BookFileId != Guid.Empty)
            await _mongoDb.DeleteFileAsync(book.BookFileId);

        if (book.CoverFileId != Guid.Empty)
            await _mongoDb.DeleteFileAsync(book.CoverFileId);

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    public async Task UploadCoverAsync(Guid bookId, Guid fileId, string fileName, Stream stream)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            throw new NotFoundException("Book not found");

        await _mongoDb.UploadFileAsync(fileId, fileName, stream);
        book.CoverFileId = fileId;
        book.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task UploadBookFileAsync(Guid bookId, Guid fileId, string fileName, Stream stream, int fileSize)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            throw new NotFoundException("Book not found");

        await _mongoDb.UploadFileAsync(fileId, fileName, stream);
        book.BookFileId = fileId;
        book.FileSizeBytes = fileSize;
        book.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<byte[]> ReadBookAsync(Guid bookId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null || book.BookFileId == Guid.Empty)
            throw new NotFoundException("Book or file not found");

        return await _mongoDb.DownloadFileAsync(book.BookFileId);
    }

    public async Task<PaginatedResult<CommentDto>> GetCommentsAsync(Guid bookId, int page, int pageSize)
    {
        var query = _context.Comments
            .Include(c => c.User)
            .Where(c => c.ContentId == bookId && c.ParentId == null);

        var totalCount = await query.CountAsync();
        var comments = await query
            .OrderByDescending(c => c.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Text = c.Text,
                UserId = c.UserId,
                Username = c.User.Username,
                AvatarId = c.User.AvatarId,
                ParentId = c.ParentId,
                ReplyCount = c.Replies.Count,
                CreatedOn = c.CreatedOn,
                ModifiedOn = c.ModifiedOn
            })
            .ToListAsync();

        return new PaginatedResult<CommentDto>
        {
            Items = comments,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResult<LikeDto>> GetLikesAsync(Guid bookId, int page, int pageSize)
    {
        var query = _context.Likes
            .Include(l => l.User)
            .Where(l => l.ContentId == bookId);

        var totalCount = await query.CountAsync();
        var likes = await query
            .OrderByDescending(l => l.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LikeDto
            {
                Id = l.Id,
                UserId = l.UserId,
                Username = l.User.Username,
                AvatarId = l.User.AvatarId,
                CreatedOn = l.CreatedOn
            })
            .ToListAsync();

        return new PaginatedResult<LikeDto>
        {
            Items = likes,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResult<BookDto>> GetMyBooksAsync(Guid userId, int page, int pageSize)
    {
        var query = _context.Books
            .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
            .Include(b => b.Author)
            .Where(b => b.UserId == userId);

        var totalCount = await query.CountAsync();
        var books = await query
            .OrderByDescending(b => b.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Slug = b.Slug,
                Excerpt = b.Excerpt ?? string.Empty,
                AuthorName = b.AuthorName,
                Description = b.Description,
                PageAmount = b.PageAmount,
                YearOfPublish = b.YearOfPublish,
                FileSizeBytes = b.FileSizeBytes,
                CreatedOn = b.CreatedOn,
                ModifiedOn = b.ModifiedOn,
                Genres = b.BookGenres.Select(bg => new GenreDto { Id = bg.Genre.Id, Name = bg.Genre.Name }).ToList(),
                CoverFileId = b.CoverFileId,
                BookFileId = b.BookFileId,
                Status = b.Status,
                UserId = b.UserId,
                Username = b.Author.Username,
                LikesCount = b.Likes.Count,
                CommentsCount = b.Comments.Count,
                ViewsCount = b.Views.Count
            })
            .ToListAsync();

        return new PaginatedResult<BookDto>
        {
            Items = books,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResult<BookDto>> GetArchivedBooksAsync(Guid userId, int page, int pageSize)
    {
        var query = _context.Books
            .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
            .Include(b => b.Author)
            .Where(b => b.UserId == userId && b.Status == ContentStatus.Archived);

        var totalCount = await query.CountAsync();
        var books = await query
            .OrderByDescending(b => b.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Slug = b.Slug,
                Excerpt = b.Excerpt ?? string.Empty,
                AuthorName = b.AuthorName,
                Description = b.Description,
                PageAmount = b.PageAmount,
                YearOfPublish = b.YearOfPublish,
                FileSizeBytes = b.FileSizeBytes,
                CreatedOn = b.CreatedOn,
                ModifiedOn = b.ModifiedOn,
                Genres = b.BookGenres.Select(bg => new GenreDto { Id = bg.Genre.Id, Name = bg.Genre.Name }).ToList(),
                CoverFileId = b.CoverFileId,
                BookFileId = b.BookFileId,
                Status = b.Status,
                UserId = b.UserId,
                Username = b.Author.Username,
                LikesCount = b.Likes.Count,
                CommentsCount = b.Comments.Count,
                ViewsCount = b.Views.Count
            })
            .ToListAsync();

        return new PaginatedResult<BookDto>
        {
            Items = books,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task PublishBookAsync(Guid bookId, Guid userId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            throw new NotFoundException("Book not found");

        if (book.UserId != userId)
            throw new UnauthorizedException("You can only publish your own books");

        book.Status = ContentStatus.Published;
        book.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task UnpublishBookAsync(Guid bookId, Guid userId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            throw new NotFoundException("Book not found");

        if (book.UserId != userId)
            throw new UnauthorizedException("You can only unpublish your own books");

        book.Status = ContentStatus.Draft;
        book.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task ArchiveBookAsync(Guid bookId, Guid userId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            throw new NotFoundException("Book not found");

        if (book.UserId != userId)
            throw new UnauthorizedException("You can only archive your own books");

        book.Status = ContentStatus.Archived;
        book.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task RestoreBookAsync(Guid bookId, Guid userId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            throw new NotFoundException("Book not found");

        if (book.UserId != userId)
            throw new UnauthorizedException("You can only restore your own books");

        book.Status = ContentStatus.Draft;
        book.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
    

    private static string GenerateSlug(string title)
    {
        return title.ToLower().Replace(" ", "-");
    }
}