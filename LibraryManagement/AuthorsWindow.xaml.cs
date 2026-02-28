using LibraryManagement.Data;
using LibraryManagement.Models;
using System;
using System.Linq;
using System.Windows;

namespace LibraryManagement
{
    public partial class AuthorsWindow : Window
    {
        private LibraryContext _db;

        public AuthorsWindow()
        {
            InitializeComponent();
            _db = new LibraryContext();
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            AuthorsDataGrid.ItemsSource = _db.Authors.ToList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _db.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены!", "Успех");
                LoadAuthors();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            // Показываем диалог ввода
            var dialog = new AddAuthorDialog();
            if (dialog.ShowDialog() == true)
            {
                string firstName = dialog.FirstName;
                string lastName = dialog.LastName;
                string country = dialog.Country;
                DateTime birthDate = dialog.BirthDate;

                // Проверяем дубликат
                bool exists = _db.Authors.Any(a =>
                    a.FirstName == firstName && a.LastName == lastName);

                if (exists)
                {
                    MessageBox.Show("Автор с таким именем уже существует!", "Ошибка");
                    return;
                }

                _db.Authors.Add(new Author
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Country = country,
                    BirthDate = birthDate
                });
                _db.SaveChanges();
                LoadAuthors();
            }
        }

        private void DeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (AuthorsDataGrid.SelectedItem is Author author)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить автора {author.FullName}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.Authors.Remove(author);
                        _db.SaveChanges();
                        LoadAuthors();
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось удалить автора. Возможно, за ним числятся книги.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите автора из списка.");
            }
        }
    }
}