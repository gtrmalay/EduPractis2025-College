using System;
using System.Windows;

namespace EventsWPF_Practic
{
    public partial class UserEditWindow : Window
    {
        private readonly Users _user;
        private readonly EventsDB_PracticEntities _db;
        private readonly Users _organizer;

        public UserEditWindow(Users user, EventsDB_PracticEntities db, Users organizer)
        {
            InitializeComponent();
            _user = user;
            _db = db;
            _organizer = organizer;
            LoadData();
        }

        private void LoadData()
        {
            FirstNameBox.Text = _user.FirstName ?? string.Empty;
            PatronymicBox.Text = _user.Patronymic ?? string.Empty;
            SurnameBox.Text = _user.Surname ?? string.Empty;
            EmailBox.Text = _user.EmailAddress ?? string.Empty;
            PhoneBox.Text = _user.PhoneNumber ?? string.Empty;
            BirthDatePicker.SelectedDate = _user.BirthDate;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _user.FirstName = FirstNameBox.Text.Trim();
            _user.Patronymic = string.IsNullOrWhiteSpace(PatronymicBox.Text) ? null : PatronymicBox.Text.Trim();
            _user.Surname = SurnameBox.Text.Trim();
            _user.EmailAddress = string.IsNullOrWhiteSpace(EmailBox.Text) ? null : EmailBox.Text.Trim();
            _user.PhoneNumber = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim();
            _user.BirthDate = BirthDatePicker.SelectedDate;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}