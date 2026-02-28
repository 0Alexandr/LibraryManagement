using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;
using System.Collections.Generic;

namespace LibraryManagement
{
    public partial class MainWindow : Window
    {
        // Создаем подключение к базе данных
        private LibraryContext _db = new LibraryContext();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                _db.Database.EnsureCreated(); // создаст БД если её нет
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД:\n\n{ex.Message}\n\nInner: {ex.InnerException?.Message}",
                                "Критическая ошибка");
            }
        }

        // Метод для загрузки данных из БД в таблицу
        private void LoadData()
        {
            // Проверяем, есть ли данные. Если нет — добавляем тестовые.
            SeedData();

            // Берем книги и "подтягиваем" связанные данные об авторах и жанрах
            var books = _db.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .ToList();

            // Отправляем список в DataGrid
            BooksDataGrid.ItemsSource = books;

            GenreComboBox.ItemsSource = new List<Genre> { new Genre { Id = 0, Name = "Все жанры" } }.Concat(_db.Genres.ToList());
            AuthorComboBox.ItemsSource = new List<Author> { new Author { Id = 0, LastName = "Все авторы" } }.Concat(_db.Authors.ToList());      
        }

        private void SeedData()
        {
            // Если в базе уже есть книги, ничего не делаем
            if (_db.Books.Any()) return;

            // 1. Создаем авторов
            var tolstoy = new Author { FirstName = "Лев", LastName = "Толстой", Country = "Россия" };
            var dostoevsky = new Author { FirstName = "Федор", LastName = "Достоевский", Country = "Россия" };
            var ilf = new Author { FirstName = "Илья", LastName = "Ильф", Country = "СССР" };
            var petrov = new Author { FirstName = "Евгений", LastName = "Петров", Country = "СССР" };
            var bulgakov = new Author { FirstName = "Михаил", LastName = "Булгаков", Country = "СССР" };

            // 2. Создаем жанры
            // 2. Создаем жанры
            var classic = new Genre { Name = "Классика", Description = "Классическая литература" };
            var novel = new Genre { Name = "Роман", Description = "Романы" };
            var satire = new Genre { Name = "Сатира", Description = "Сатирические произведения" };
            var mystic = new Genre { Name = "Мистика", Description = "Мистические произведения" };

            // 3. Создаем список книг
            var books = new List<Book> {
                new Book { Title = "Война и мир", PublishYear = 1869, ISBN = "978-5-699-12014-7", QuantityInStock = 12, Authors = new List<Author>{tolstoy}, Genres = new List<Genre>{classic, novel} },
                new Book { Title = "Преступление и наказание", PublishYear = 1866, ISBN = "978-5-389-06155-2", QuantityInStock = 8, Authors = new List<Author>{dostoevsky}, Genres = new List<Genre>{classic, novel} },
                new Book { Title = "Мастер и Маргарита", PublishYear = 1967, ISBN = "978-5-177-43320-9", QuantityInStock = 15, Authors = new List<Author>{bulgakov}, Genres = new List<Genre>{novel, mystic} },
                new Book { Title = "Двенадцать стульев", PublishYear = 1928, ISBN = "978-5-187-83109-3", QuantityInStock = 5, Authors = new List<Author>{ilf, petrov}, Genres = new List<Genre>{satire, novel} },
                new Book { Title = "Золотой теленок", PublishYear = 1931, ISBN = "978-5-998-46455-5", QuantityInStock = 3, Authors = new List<Author>{ilf, petrov}, Genres = new List<Genre>{satire} }
            };

            _db.Books.AddRange(books); // Добавляем весь список сразу
            _db.SaveChanges(); // Сохраняем в базу
        }

        // Обработчики для новых окон управления
        private void ManageAuthors_Click(object sender, RoutedEventArgs e) 
        {
            new AuthorsWindow().ShowDialog(); LoadData(); 
        }
        private void ManageGenres_Click(object sender, RoutedEventArgs e) 
        {
            new GenresWindow().ShowDialog(); LoadData(); 
        }

        // Поиск по названию
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchTextBox.Text.ToLower();

            var filtered = _db.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .Where(b => b.Title.ToLower().Contains(search))
                .ToList();

            BooksDataGrid.ItemsSource = filtered;
        }

        // Фильтрация по автору и жанру
        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGenre = GenreComboBox.SelectedItem as Genre;
            var selectedAuthor = AuthorComboBox.SelectedItem as Author;

            // Начинаем с полного списка книг
            var query = _db.Books.Include(b => b.Authors).Include(b => b.Genres).AsQueryable();

            // Фильтр по жанру: проверяем, есть ли выбранный жанр в списке Genres этой книги
            if (selectedGenre != null && selectedGenre.Id != 0)
            {
                query = query.Where(b => b.Genres.Any(g => g.Id == selectedGenre.Id));
            }

            // Фильтр по автору: проверяем, есть ли выбранный автор в списке Authors этой книги
            if (selectedAuthor != null && selectedAuthor.Id != 0)
            {
                query = query.Where(b => b.Authors.Any(a => a.Id == selectedAuthor.Id));
            }

            BooksDataGrid.ItemsSource = query.ToList();
        }

        // Удаление выбранной книги
        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var result = MessageBox.Show($"Удалить книгу '{selectedBook.Title}'?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _db.Books.Remove(selectedBook);
                    _db.SaveChanges(); // Сохраняем изменения в базе
                    LoadData(); // Обновляем таблицу
                }
            }
        }

        // Добавление книги
        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            BookWindow win = new BookWindow();
            if (win.ShowDialog() == true)
            {
                // Загружаем авторов и жанры из НАШЕГО контекста по Id
                var book = win.Book;
                var authorIds = book.Authors.Select(a => a.Id).ToList();
                var genreIds = book.Genres.Select(g => g.Id).ToList();

                var newBook = new Book
                {
                    Title = book.Title,
                    ISBN = book.ISBN,
                    PublishYear = book.PublishYear,
                    QuantityInStock = book.QuantityInStock,
                    Authors = _db.Authors.Where(a => authorIds.Contains(a.Id)).ToList(),
                    Genres = _db.Genres.Where(g => genreIds.Contains(g.Id)).ToList()
                };

                _db.Books.Add(newBook);
                _db.SaveChanges();
                LoadData();
            }
        }

        // Редактирование книги
        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = BooksDataGrid.SelectedItem as Book;
            if (selectedBook == null)
            {
                MessageBox.Show("Пожалуйста, выберите книгу из списка для редактирования.");
                return;
            }

            var bookToEdit = _db.Books
                .Include(b => b.Authors)
                .Include(b => b.Genres)
                .FirstOrDefault(b => b.Id == selectedBook.Id);

            BookWindow win = new BookWindow(bookToEdit!);
            win.Title = "Редактирование книги";

            if (win.ShowDialog() == true)
            {
                var authorIds = win.Book.Authors.Select(a => a.Id).ToList();
                var genreIds = win.Book.Genres.Select(g => g.Id).ToList();

                bookToEdit.Title = win.Book.Title;
                bookToEdit.ISBN = win.Book.ISBN;
                bookToEdit.PublishYear = win.Book.PublishYear;
                bookToEdit.QuantityInStock = win.Book.QuantityInStock;

                // Загружаем из НАШЕГО контекста по Id
                bookToEdit.Authors = _db.Authors.Where(a => authorIds.Contains(a.Id)).ToList();
                bookToEdit.Genres = _db.Genres.Where(g => genreIds.Contains(g.Id)).ToList();

                _db.SaveChanges();
                LoadData();
            }
        }
    }
}