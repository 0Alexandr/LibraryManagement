using LibraryManagement.Data;
using LibraryManagement.Models;
using System.Linq;
using System.Windows;

namespace LibraryManagement
{
    public partial class GenresWindow : Window
    {
        private LibraryContext _db;

        public GenresWindow()
        {
            InitializeComponent();
            _db = new LibraryContext();
            LoadGenres();
        }

        private void LoadGenres()
        {
            GenresDataGrid.ItemsSource = _db.Genres.ToList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _db.SaveChanges();
            MessageBox.Show("Изменения сохранены");
            LoadGenres();
        }

        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddGenreDialog();
            if (dialog.ShowDialog() == true)
            {
                string name = dialog.GenreName;
                string description = dialog.Description;

                // Проверяем дубликат
                bool exists = _db.Genres.Any(g => g.Name == name);
                if (exists)
                {
                    MessageBox.Show($"Жанр '{name}' уже существует!", "Ошибка");
                    return;
                }

                _db.Genres.Add(new Genre { Name = name, Description = description });
                _db.SaveChanges();
                LoadGenres();
            }
        }

        private void DeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            if (GenresDataGrid.SelectedItem is Genre genre)
            {
                var result = MessageBox.Show($"Удалить жанр '{genre.Name}'?",
                    "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _db.Genres.Remove(genre);
                    _db.SaveChanges();
                    LoadGenres();
                }
            }
            else
            {
                MessageBox.Show("Выберите жанр для удаления");
            }
        }
    }
}