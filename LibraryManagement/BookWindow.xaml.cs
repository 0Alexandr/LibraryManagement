using LibraryManagement.Models;
using LibraryManagement.Data;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LibraryManagement
{
    public partial class BookWindow : Window
    {
        private LibraryContext _db = new LibraryContext();

        // Свойство для передачи книги обратно в главное окно
        public Book Book { get; set; }

        // Конструктор для добавления новой книги
        public BookWindow()
        {
            InitializeComponent();
            LoadLists();
            Book = new Book(); // Создаем пустой объект
        }

        // Конструктор для редактирования существующей книги
        public BookWindow(Book existingBook)
        {
            InitializeComponent();
            LoadLists();

            Book = existingBook;

            // Заполняем текстовые поля
            TitleTextBox.Text = Book.Title;
            IsbnTextBox.Text = Book.ISBN;
            YearTextBox.Text = Book.PublishYear.ToString();
            CountTextBox.Text = Book.QuantityInStock.ToString();

            // Выделяем авторов книги в списке
            foreach (var author in Book.Authors)
            {
                var item = AuthorsListBox.Items.Cast<Author>().FirstOrDefault(a => a.Id == author.Id);
                if (item != null) AuthorsListBox.SelectedItems.Add(item);
            }

            // Выделяем жанры книги в списке
            foreach (var genre in Book.Genres)
            {
                var item = GenresListBox.Items.Cast<Genre>().FirstOrDefault(g => g.Id == genre.Id);
                if (item != null) GenresListBox.SelectedItems.Add(item);
            }
        }

        private void LoadLists()
        {
            AuthorsListBox.ItemsSource = _db.Authors.ToList();
            GenresListBox.ItemsSource = _db.Genres.ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Валидация пустых полей
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || string.IsNullOrWhiteSpace(IsbnTextBox.Text))
            {
                MessageBox.Show("Заполните название и ISBN!");
                return;
            }

            // 2. Валидация ISBN (Регулярное выражение)
            // Формат: 978-X-XXX-XXXXX-X
            if (!Regex.IsMatch(IsbnTextBox.Text, @"^97[89]-\d-\d{2,}-\d{5,}-\d$"))
            {
                MessageBox.Show("Неверный формат ISBN!\nПример: 978-5-699-12014-7");
                return;
            }

            // 3. Валидация чисел (Год и Количество)
            if (!int.TryParse(YearTextBox.Text, out int year) || year < 0 || year > 2026)
            {
                MessageBox.Show("Введите корректный год издания!");
                return;
            }

            if (!int.TryParse(CountTextBox.Text, out int count) || count < 1)
            {
                MessageBox.Show("Количество не может быть меньше 1!");
                return;
            }

            // 4. Проверка выбора в списках
            var selectedAuthors = AuthorsListBox.SelectedItems.Cast<Author>().ToList();
            var selectedGenres = GenresListBox.SelectedItems.Cast<Genre>().ToList();

            if (selectedAuthors.Count == 0 || selectedGenres.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одного автора и один жанр!");
                return;
            }

            // 5. Заполнение объекта данными
            Book.Title = TitleTextBox.Text;
            Book.ISBN = IsbnTextBox.Text;
            Book.PublishYear = year;
            Book.QuantityInStock = count;

            // Важно для Many-to-Many: перезаписываем коллекции
            Book.Authors = selectedAuthors;
            Book.Genres = selectedGenres;

            this.DialogResult = true;
        }
    }
}
