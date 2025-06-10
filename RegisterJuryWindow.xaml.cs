using Microsoft.Win32;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace EventsWPF_Practic
{
    public partial class RegisterJuryWindow : Window
    {
        private readonly EventsDB_PracticEntities _db;
        private readonly UserService _userService;
        private string _photoPath;

        public RegisterJuryWindow()
        {
            InitializeComponent();
            _db = new EventsDB_PracticEntities();
            _userService = new UserService();
            LoadComboBoxes();
            GeneratedIdText.Text = _userService.GenerateUserId();
        }

        private void LoadComboBoxes()
        {
            GenderComboBox.ItemsSource = _db.Genders.AsNoTracking().ToList();
            RoleComboBox.ItemsSource = _db.Roles.AsNoTracking().Where(r => r.RoleName == "жюри" || r.RoleName == "модератор").ToList();
            DirectionComboBox.ItemsSource = _db.Directions.AsNoTracking().ToList();
            EventComboBox.ItemsSource = _db.Events.AsNoTracking().ToList();
            EventComboBox.IsEnabled = false;
        }

        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "Image files (*.png;*.jpg)|*.png;*.jpg" };
            if (openFileDialog.ShowDialog() == true)
            {
                _photoPath = openFileDialog.FileName;
                ProfileImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_photoPath));
            }
        }

        private void AttachEventCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EventComboBox.IsEnabled = true;
        }

        private void AttachEventCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            EventComboBox.IsEnabled = false;
            EventComboBox.SelectedItem = null;
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = PasswordBox.Password;
            ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
            PasswordTextBox.Visibility = Visibility.Visible;
            ConfirmPasswordTextBox.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordBox.Visibility = Visibility.Collapsed;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = PasswordTextBox.Text;
            ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text;
            PasswordBox.Visibility = Visibility.Visible;
            ConfirmPasswordBox.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            using (var db = new EventsDB_PracticEntities())
            {
                var user = new Users
                {
                    UserID = int.Parse(GeneratedIdText.Text),
                    Surname = LastNameBox.Text.Trim(),
                    FirstName = FirstNameBox.Text.Trim(),
                    Patronymic = string.IsNullOrWhiteSpace(MiddleNameBox.Text) ? null : MiddleNameBox.Text.Trim(),
                    EmailAddress = EmailBox.Text.Trim(),
                    PhoneNumber = PhoneBox.Text.Trim(),
                    UserPass = _userService.HashPassword(PasswordBox.Password),
                    PhotoPath = !string.IsNullOrEmpty(_photoPath) ? _photoPath : null
                };

                // Временно закомментируем добавление связи Genders
                // if (GenderComboBox.SelectedItem is Genders gender)
                // {
                //     user.Genders.Add(gender);
                // }

                if (RoleComboBox.SelectedItem is Roles role)
                    user.Roles.Add(role);
                if (DirectionComboBox.SelectedItem is Directions direction)
                    user.Directions.Add(direction);

                int? eventId = null;
                if (AttachEventCheckBox.IsChecked == true && EventComboBox.SelectedItem is Events selectedEvent)
                    eventId = selectedEvent.EventID;

                try
                {
                    _userService.RegisterJury(user, eventId);
                    MessageBox.Show("Пользователь успешно зарегистрирован.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(LastNameBox.Text) || string.IsNullOrWhiteSpace(FirstNameBox.Text))
            {
                MessageBox.Show("Введите ФИО.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!Regex.IsMatch(EmailBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Введите корректный email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!Regex.IsMatch(PhoneBox.Text, @"^\+7\(\d{3}\)-\d{3}-\d{2}-\d{2}$"))
            {
                MessageBox.Show("Введите телефон в формате +7(999)-099-90-90.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!IsValidPassword(PasswordBox.Password))
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов, включать заглавные и строчные буквы, цифры и спецсимволы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли должны совпадать.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (GenderComboBox.SelectedItem == null || RoleComboBox.SelectedItem == null || DirectionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DirectionComboBox.IsEditable && !string.IsNullOrWhiteSpace(DirectionComboBox.Text))
            {
                var directionName = DirectionComboBox.Text.Trim();
                if (!_db.Directions.Any(d => d.DirectionName == directionName))
                {
                    var newDirection = new Directions { DirectionName = directionName };
                    _db.Directions.Add(newDirection);
                    _db.SaveChanges();
                    DirectionComboBox.ItemsSource = _db.Directions.AsNoTracking().ToList();
                }
            }

            return true;
        }

        private bool IsValidPassword(string password)
        {
            if (password.Length < 6)
            {
                Console.WriteLine("Длина пароля меньше 6 символов.");
                return false;
            }
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                Console.WriteLine("Нет заглавных букв.");
                return false;
            }
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                Console.WriteLine("Нет строчных букв.");
                return false;
            }
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                Console.WriteLine("Нет цифр.");
                return false;
            }
            if (!Regex.IsMatch(password, @"[!@#$%^&*_\-+=<>?/]"))
            {
                Console.WriteLine("Нет допустимых спецсимволов.");
                return false;
            }
            Console.WriteLine("Пароль валиден.");
            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}