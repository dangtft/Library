﻿using API_LIBRARY.Data;
using API_LIBRARY.DTO;
using API_LIBRARY.Interfaces;
using API_LIBRARY.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace API_LIBRARY.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly LibaryDbContext _db;
        public BookRepository(LibaryDbContext context)
        {
            _db = context;
        }

        #region Books

        public async Task<List<BookAuthorAndPublisher>> GetBooksAsync(string? filterOn=null, string? filterQuery=null,
                              string? sortBy = null, bool isAscending = true,
                              int pageNumber = 1, int pageSize = 100)
        {
            try
            {
                IQueryable<Book> query = _db.Books.Include(b => b.Publisher).Include(b => b.BookAuthors).ThenInclude(ba => ba.Author);

                var allBooks = _db.Books.Select(Books => new BookAuthorAndPublisher()
                {
                    Id = Books.Id,
                    Title = Books.Title,
                    Description = Books.Description,
                    isRead = Books.isRead,
                    DateRead = (bool)Books.isRead ? Books.DateRead.Value : null,                 
                    Genre = Books.Genre,
                    CoverUrl = Books.CoverUrl,
                    PublisherName = Books.Publisher.Name,
                    AuthorNames = Books.BookAuthors.Select(n => n.Author.FullName).ToList()
                }).AsQueryable();

                //filtering
                if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
                {
                    if (filterOn.Equals("title", StringComparison.OrdinalIgnoreCase))
                    {
                        allBooks = allBooks.Where(x => x.Title.Contains(filterQuery));
                    }
                }


                //sorting
                if (string.IsNullOrWhiteSpace(sortBy) == false)
                {
                    if (sortBy.Equals("title", StringComparison.OrdinalIgnoreCase))
                    {
                        allBooks = isAscending ? allBooks.OrderBy(x => x.Title) :
                       allBooks.OrderByDescending(x => x.Title);
                    }
                }

                //pagination
                var skipResults = (pageNumber - 1) * pageSize;
                return allBooks.Skip(skipResults).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return null;
            }
        }

        public async Task<BookAuthorAndPublisher> GetBookAsync(int id)
        {
            try
            {
                var book = await _db.Books.Include(b => b.Publisher).Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                                           .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                    return null;

                return new BookAuthorAndPublisher
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    isRead = book.isRead,
                    DateRead = book.DateRead,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    DateAdded = book.DateAdded,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.BookAuthors.Select(author => author.Author.FullName).ToList()
                };
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return null;
            }
        }

        public async Task<bool> AddBookAsync(BookDTO bookDTO)
        {
            try
            {
                var book = new Book
                {
                    Title = bookDTO.Title,
                    Description = bookDTO.Description,
                    isRead = bookDTO.isRead,
                    DateRead = bookDTO.DateRead,
                    Genre = bookDTO.Genre,
                    CoverUrl = bookDTO.CoverUrl,
                    DateAdded = bookDTO.DateAdded,
                    PublisherId = bookDTO.PublisherId
                };
                book.BookAuthors = bookDTO.AuthorId.Select(authorId => new BookAuthor { AuthorId = authorId }).ToList();

                _db.Books.Add(book);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return false;
            }
        }

        public async Task<bool> UpdateBookAsync(int id, BookDTO bookDTO)
        {
            try
            {
                var book = await _db.Books.FindAsync(id);

                if (book == null)
                    return false;

                book.Title = bookDTO.Title;
                book.Description = bookDTO.Description;
                book.isRead = bookDTO.isRead;
                book.DateRead = bookDTO.DateRead;
                book.Genre = bookDTO.Genre;
                book.CoverUrl = bookDTO.CoverUrl;
                book.DateAdded = bookDTO.DateAdded;
                book.PublisherId = bookDTO.PublisherId;

                _db.Entry(book).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return false;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                var book = await _db.Books.FindAsync(id);

                if (book == null)
                    return false;

                _db.Books.Remove(book);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return false;
            }
        }

        #endregion Books

        #region Authors

        public async Task<List<Author>> GetAuthorsAsync()
        {
            var authors = await _db.Authors.Include(a => a.BookAuthors).ThenInclude(ba => ba.Book).ToListAsync();

            return authors;
        }

        public async Task<Author> GetAuthorAsync(int id, bool includeBooks)
        {
            try
            {
                var query = _db.Authors.AsQueryable();
                if (includeBooks)
                {
                    query = query.Include(a => a.BookAuthors).ThenInclude(ba => ba.Book);
                }
                return await query.FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Author> AddAuthorAsync(AddAuthor authorDTO)
        {
            try
            {
                var author = new Author
                {
                    FullName = authorDTO.FullName

                };
                author.BookAuthors = authorDTO.BookId.Select(bookid => new BookAuthor { BookId = bookid }).ToList();
                _db.Authors.Add(author);
                await _db.SaveChangesAsync();

                return author;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateAuthorAsync(int id, AuthorDTO authorDTO)
        {
            try
            {
                var author = await _db.Authors.FindAsync(id);

                if (author == null)
                    return false;

                author.FullName = authorDTO.FullName;

                _db.Entry(author).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            try
            {
                var author = await _db.Authors.FindAsync(id);

                if (author == null)
                    return false;

                _db.Authors.Remove(author);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion Author

        #region Pulishers
        public async Task<List<Publisher>> GetPublishersAsync()
        {
            return await _db.Publisher.ToListAsync();
        }

        public async Task<Publisher> GetPublisherAsync(int id, bool includeBooks = false)
        {
            var query = _db.Publisher.AsQueryable();
            if (includeBooks)
            {
                query = query.Include(p => p.Books);
            }
            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Publisher> AddPublisherAsync(PulisherDTO publisherDTO)
        {
            try
            {
                var publisher = new Publisher
                {
                    Name = publisherDTO.Name
                };

                _db.Publisher.Add(publisher);
                await _db.SaveChangesAsync();

                return publisher;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<bool> UpdatePublisherAsync(int id, PulisherDTO publisherDTO)
        {
            try
            {
                var publisher = await _db.Publisher.FindAsync(id);

                if (publisher == null)
                    return false;

                publisher.Name = publisherDTO.Name;

                _db.Entry(publisher).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeletePublisherAsync(int id)
        {
            try
            {
                var publisher = await _db.Publisher.FindAsync(id);

                if (publisher == null)
                    return false;

                _db.Publisher.Remove(publisher);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return false;
            }
        }

        #endregion Pulisher
    }
}
