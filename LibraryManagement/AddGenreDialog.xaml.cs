using System.Windows;

namespace LibraryManagement
{
    public partial class AddGenreDialog : Window
    {
        public string GenreName { get; private set; }
        public string Description { get; private set; }

        public AddGenreDialog()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название жанра!", "Ошибка");
                return;
            }

            GenreName = NameBox.Text.Trim();
            Description = DescriptionBox.Text.Trim();

            this.DialogResult = true;
        }
    }
}