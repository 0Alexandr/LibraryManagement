using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;


namespace LibraryManagement.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Настройка подключения к локальной базе SQL Server
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LibraryDb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройки для Книги
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
            });

            // Настройки для Автора
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.LastName).IsRequired().HasMaxLength(100);
                entity.Property(a => a.FirstName).IsRequired().HasMaxLength(100);
            });

            // Настройки для Жанра
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
            });

            // Связи многие-ко-многим EF Core создаст сам, 
            // так как увидел ICollection в классах Book, Author и Genre.
        }
    }
}
