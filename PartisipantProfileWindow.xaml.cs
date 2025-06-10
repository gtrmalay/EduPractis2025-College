using EventsWPF_Practic;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Session1
{
    public partial class ParticipantProfileWindow : Window
    {
        private readonly EventsDB_PracticEntities _db;
        private Users _currentUser; // Сделано не readonly, чтобы можно было переопределить

        public ParticipantProfileWindow(Users user, EventsDB_PracticEntities db)
        {
            InitializeComponent();
            _db = db ?? throw new ArgumentNullException(nameof(db), "Контекст базы данных не может быть null.");
            _currentUser = _db.Users.FirstOrDefault(u => u.UserID == user.UserID)
                ?? throw new ArgumentException("Пользователь не найден в базе данных.", nameof(user));
            LoadUserData();
            ShowGreeting();
        }

        private void LoadUserData()
        {
            // Загрузка фотографии
            if (!string.IsNullOrEmpty(_currentUser.PhotoPath))
            {
                try
                {
                    string fileName = _currentUser.PhotoPath.Split(new[] { "UserID" }, StringSplitOptions.None)[0].Trim();
                    string imagePath = $"/Source/{fileName}"; // Предполагаемый путь к изображению
                    ProfilePhoto.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                }
                catch (Exception)
                {
                    ProfilePhoto.Source = null; // Очищаем, если изображение не загрузилось
                }
            }
            else
            {
                ProfilePhoto.Source = null; // Если PhotoPath пустой
            }

            // Загрузка остальных данных
            FirstNameBox.Text = _currentUser.FirstName ?? string.Empty;
            LastNameBox.Text = _currentUser.Surname ?? string.Empty;
            MiddleNameBox.Text = _currentUser.Patronymic ?? string.Empty;
            EmailBox.Text = _currentUser.EmailAddress ?? string.Empty;
            PhoneBox.Text = _currentUser.PhoneNumber ?? string.Empty;
            BirthDatePicker.SelectedDate = _currentUser.BirthDate;
        }

        private void ShowGreeting()
        {
            string timeOfDay;
            var now = DateTime.Now.TimeOfDay; // 02:07 PM CEST = 14:07

            if (now >= TimeSpan.FromHours(6) && now < TimeSpan.FromHours(12))
                timeOfDay = "Доброе утро";
            else if (now >= TimeSpan.FromHours(12) && now < TimeSpan.FromHours(18))
                timeOfDay = "Добрый день"; // 14:07 попадает сюда
            else if (now >= TimeSpan.FromHours(18) && now < TimeSpan.FromHours(24))
                timeOfDay = "Добрый вечер";
            else
                timeOfDay = "Доброй ночи";

            string prefix = GetUserGenderPrefix(_currentUser.UserID);
            string fullName = $"{prefix} {_currentUser.FirstName} {_currentUser.Surname}".Trim();

            DynamicGreetingText.Text = $"{timeOfDay}, {fullName}!";
        }

        private string GetUserGenderPrefix(int userId)
        {
            var user = _db.Users.AsNoTracking().FirstOrDefault(u => u.UserID == userId);
            if (user != null && user.Genders != null && user.Genders.Any())
            {
                string genderName = user.Genders.First().GenderName?.ToLower();
                if (genderName == "male" || genderName == "мужской")
                    return "Mr.";
                else if (genderName == "female" || genderName == "женский")
                    return "Ms.";
            }
            return ""; // Нейтральное приветствие, если пол неизвестен
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("Имя и фамилия обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(EmailBox.Text) && !IsValidEmail(EmailBox.Text))
            {
                MessageBox.Show("Некорректный формат email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Обновление данных
            _currentUser.FirstName = FirstNameBox.Text.Trim();
            _currentUser.Surname = LastNameBox.Text.Trim();
            _currentUser.Patronymic = string.IsNullOrWhiteSpace(MiddleNameBox.Text) ? null : MiddleNameBox.Text.Trim();
            _currentUser.EmailAddress = string.IsNullOrWhiteSpace(EmailBox.Text) ? null : EmailBox.Text.Trim();
            _currentUser.PhoneNumber = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim();
            _currentUser.BirthDate = BirthDatePicker.SelectedDate;

            try
            {
                int changes = _db.SaveChanges();
                Console.WriteLine($"Сохранено изменений: {changes}");
                MessageBox.Show("Данные успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true; // Возвращаем успешный результат
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Возвращаем неуспешный результат
            Close();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true; // Пустой email допустим
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

       
    }
}