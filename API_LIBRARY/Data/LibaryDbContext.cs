using Microsoft.EntityFrameworkCore;
using API_LIBRARY.Models;
using System;
using System.Reflection.Emit;
using API_LIBRARY.Data;
namespace API_LIBRARY.Data
{
    public class LibaryDbContext : DbContext
    {
        public LibaryDbContext(DbContextOptions<LibaryDbContext> options) : base(options)
        {
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publisher { get; set; }
        public DbSet<BookAuthor> BookAuthor { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BookAuthor>()
            .HasOne(b => b.Book)
            .WithMany(ba => ba.BookAuthors)
            .HasForeignKey(b => b.BookId);
            builder.Entity<BookAuthor>()
                .HasOne(b => b.Author)
                .WithMany(ba => ba.BookAuthors)
                .HasForeignKey(bi => bi.AuthorId);

            base.OnModelCreating(builder);

            // Seed data
            builder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "Publisher A" },
                new Publisher { Id = 2, Name = "Publisher B" }
            );

            builder.Entity<Author>().HasData(
                new Author { Id = 1, FullName = "Author 1" },
                new Author { Id = 2, FullName = "Author 2" }
            );

            builder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "Book 1",
                    Description = "Description of Book 1",
                    isRead = true,
                    DateRead = DateTime.Now,
                    Genre = 1,
                    CoverUrl = "https://example.com/book1cover",
                    DateAdded = DateTime.Now,
                    PublisherId = 1 
                },
                new Book
                {
                    Id = 2,
                    Title = "Book 2",
                    Description = "Description of Book 2",
                    isRead = false,
                    DateRead = null,
                    Genre = 1,
                    CoverUrl = "https://example.com/book2cover",
                    DateAdded = DateTime.Now,
                    PublisherId = 2 
                }
            );

            builder.Entity<BookAuthor>().HasData(
                new BookAuthor { Id = 1, BookId = 1, AuthorId = 1 },
                new BookAuthor { Id = 2, BookId = 2, AuthorId = 2 }
            );
        }
    }
}
