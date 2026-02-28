using System;
using System.Windows;

namespace LibraryManagement
{
    public partial class AddAuthorDialog : Window
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Country { get; private set; }
        public DateTime BirthDate { get; private set; }

        public AddAuthorDialog()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameBox.Text))
            {
                MessageBox.Show("Заполните имя и фамилию!", "Ошибка");
                return;
            }

            FirstName = FirstNameBox.Text.Trim();
            LastName = LastNameBox.Text.Trim();
            Country = CountryBox.Text.Trim();
            BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now.AddYears(-30);

            this.DialogResult = true;
        }
    }
}