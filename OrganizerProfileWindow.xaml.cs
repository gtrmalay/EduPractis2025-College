using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EventsWPF_Practic
{
    public partial class OrganizerProfileWindow : Window
    {
        private readonly Users _user;
        private readonly EventsDB_PracticEntities _db;

        public OrganizerProfileWindow(Users user)
        {
            InitializeComponent();
            _user = user ?? throw new ArgumentNullException(nameof(user), "Пользователь не может быть null.");
            _db = new EventsDB_PracticEntities(); // Новый контекст для каждого окна
            LoadProfileInfo();
            ShowGreeting();
        }

        private void LoadProfileInfo()
        {
            // Загружаем данные без трекинга, чтобы избежать конфликтов
            var userFromDb = _db.Users.AsNoTracking().FirstOrDefault(u => u.UserID == _user.UserID);
            if (userFromDb != null)
            {
                if (!string.IsNullOrEmpty(userFromDb.PhotoPath))
                {
                    try
                    {
                        string fileName = userFromDb.PhotoPath.Split(new[] { "UserID" }, StringSplitOptions.None)[0].Trim();
                        string imagePath = $"/Source/{fileName}";
                        ProfilePhoto.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                    }
                    catch (Exception)
                    {
                        ProfilePhoto.Source = null;
                    }
                }
                else
                {
                    ProfilePhoto.Source = null;
                }

                FirstNameBox.Text = userFromDb.FirstName ?? string.Empty;
                PatronymicBox.Text = userFromDb.Patronymic ?? string.Empty;
                SurnameBox.Text = userFromDb.Surname ?? string.Empty;
                EmailBox.Text = userFromDb.EmailAddress ?? string.Empty;
                PhoneBox.Text = userFromDb.PhoneNumber ?? string.Empty;
                BirthDatePicker.SelectedDate = userFromDb.BirthDate;
                CountryBox.Text = userFromDb.Countries?.NameRu ?? "Не указано";
            }
        }

        private void ShowGreeting()
        {
            string timeOfDay;
            var now = DateTime.Now.TimeOfDay; // 01:31 CEST

            if (now >= TimeSpan.FromHours(6) && now < TimeSpan.FromHours(12))
                timeOfDay = "Доброе утро";
            else if (now >= TimeSpan.FromHours(12) && now < TimeSpan.FromHours(18))
                timeOfDay = "Добрый день";
            else if (now >= TimeSpan.FromHours(18) && now < TimeSpan.FromHours(24))
                timeOfDay = "Добрый вечер";
            else
                timeOfDay = "Доброй ночи"; // Для 01:31 будет "Доброй ночи"

            string prefix = GetUserGenderPrefix(_user.UserID);
            string fullName = $"{prefix} {_user.FirstName} {_user.Surname}".Trim();

            var dynamicGreetingText = (TextBlock)this.FindName("DynamicGreetingText");
            if (dynamicGreetingText != null)
            {
                dynamicGreetingText.Text = $"{timeOfDay}, {fullName}!";
            }
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
            return "";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || string.IsNullOrWhiteSpace(SurnameBox.Text))
            {
                MessageBox.Show("Имя и фамилия обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(EmailBox.Text) && !IsValidEmail(EmailBox.Text))
            {
                MessageBox.Show("Некорректный формат email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Обновляем сущность заново
            var userToUpdate = _db.Users.FirstOrDefault(u => u.UserID == _user.UserID);
            if (userToUpdate == null)
            {
                MessageBox.Show("Пользователь не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            userToUpdate.FirstName = FirstNameBox.Text.Trim();
            userToUpdate.Patronymic = string.IsNullOrWhiteSpace(PatronymicBox.Text) ? null : PatronymicBox.Text.Trim();
            userToUpdate.Surname = SurnameBox.Text.Trim();
            userToUpdate.EmailAddress = string.IsNullOrWhiteSpace(EmailBox.Text) ? null : EmailBox.Text.Trim();
            userToUpdate.PhoneNumber = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim();
            userToUpdate.BirthDate = BirthDatePicker.SelectedDate;

            try
            {
                int changes = _db.SaveChanges();
                Console.WriteLine($"Сохранено изменений: {changes}");
                if (changes > 0)
                {
                    MessageBox.Show("Профиль успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _user.FirstName = userToUpdate.FirstName;
                    _user.Patronymic = userToUpdate.Patronymic;
                    _user.Surname = userToUpdate.Surname;
                    _user.EmailAddress = userToUpdate.EmailAddress;
                    _user.PhoneNumber = userToUpdate.PhoneNumber;
                    _user.BirthDate = userToUpdate.BirthDate;
                }
                else
                {
                    MessageBox.Show("Нет изменений для сохранения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                DialogResult = true;

                if (Owner is OrganizerWindow organizerWindow)
                {
                    organizerWindow.LoadUserInfo();
                    organizerWindow.LoadTabs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}