using Session1;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EventsWPF_Practic
{
    public partial class ParticipantWindow : Window
    {
        private Users _user;
        private readonly EventsDB_PracticEntities _db;

        public ParticipantWindow(Users user)
        {
            InitializeComponent();
            _user = user;
            _db = new EventsDB_PracticEntities();
            LoadUserInfo();
            LoadGroupActivities();
        }

        private void LoadUserInfo()
        {
            GreetingTextBlock.Text = $"Добро пожаловать, {_user.FirstName} {_user.Patronymic}!";
            if (!string.IsNullOrEmpty(_user.PhotoPath))
            {
                try
                {
                    string fileName = _user.PhotoPath.Split(new[] { "UserID" }, StringSplitOptions.None)[0].Trim();
                    string imagePath = $"/Source/{fileName}";
                    UserPhoto.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                }
                catch
                {
                    UserPhoto.Source = null;
                }
            }
        }

        private void LoadGroupActivities()
        {
            var activities = (from e in _db.Events
                              join ae in _db.ActivitiesEvents on e.EventID equals ae.EventID
                              where e.Users1.Any(u => u.UserID == _user.UserID && u.Roles.Any(r => r.RoleName == "участник"))
                              select new
                              {
                                  ActivityID = ae.ActivityID,
                                  EventName = e.EventName,
                                  DayNumber = ae.DayNumber,
                                  StartTime = ae.StartTime
                              }).Distinct().ToList();

            Console.WriteLine($"Найдено активностей через Users1 с ролью 'участник': {activities.Count}");

            if (activities.Count == 0)
            {
                var moderatorActivities = (from ae in _db.ActivitiesEvents
                                           join e in _db.Events on ae.EventID equals e.EventID
                                           where ae.ModeratorUserID == _user.UserID
                                           select new
                                           {
                                               ActivityID = ae.ActivityID,
                                               EventName = e.EventName,
                                               DayNumber = ae.DayNumber,
                                               StartTime = ae.StartTime
                                           }).Distinct().ToList();

                Console.WriteLine($"Найдено активностей через ModeratorUserID: {moderatorActivities.Count}");

                if (moderatorActivities.Count == 0)
                {
                    ActivitiesListView.Items.Add(new { EventName = "Нет назначенных активностей.", DayNumber = 0, StartTime = TimeSpan.Zero, ActivityID = 0 });
                }
                else
                {
                    ActivitiesListView.ItemsSource = moderatorActivities;
                }
            }
            else
            {
                ActivitiesListView.ItemsSource = activities;
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ParticipantProfileWindow(_user, _db);
            if (profileWindow.ShowDialog() == true)
            {
                // Обновляем _user из базы данных
                _user = _db.Users.FirstOrDefault(u => u.UserID == _user.UserID);
                if (_user == null)
                {
                    MessageBox.Show("Не удалось загрузить обновленные данные пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                LoadUserInfo();
                LoadGroupActivities();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the login window
            var loginWindow = new LoginWindow(); // Assuming LoginWindow exists in the project
            loginWindow.Show();

            // Close the current participant window
            this.Close();
        }
    }
}