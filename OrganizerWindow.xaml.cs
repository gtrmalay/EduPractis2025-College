using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EventsWPF_Practic
{
    public partial class OrganizerWindow : Window
    {
        private readonly Users _user;
        private readonly EventsDB_PracticEntities _db;

        public OrganizerWindow(Users user)
        {
            InitializeComponent();
            _user = user;
            _db = new EventsDB_PracticEntities();
            LoadUserInfo();
            LoadTabs();
        }

        public void LoadUserInfo()
        {
            ShowGreeting();
            if (!string.IsNullOrEmpty(_user.PhotoPath))
            {
                try
                {
                    string fileName = _user.PhotoPath.Split(new[] { "UserID" }, StringSplitOptions.None)[0].Trim();
                    string imagePath = $"/Source/{fileName}";
                    UserPhoto.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                }
                catch (Exception)
                {
                    UserPhoto.Source = null;
                }
            }
        }

        private void ShowGreeting()
        {
            string timeOfDay;
            var now = DateTime.Now.TimeOfDay;

            if (now >= TimeSpan.FromHours(6) && now < TimeSpan.FromHours(12))
                timeOfDay = "Доброе утро";
            else if (now >= TimeSpan.FromHours(12) && now < TimeSpan.FromHours(18))
                timeOfDay = "Добрый день";
            else if (now >= TimeSpan.FromHours(18) && now < TimeSpan.FromHours(24))
                timeOfDay = "Добрый вечер";
            else
                timeOfDay = "Доброй ночи";

            string prefix = GetUserGenderPrefix(_user.UserID);
            string fullName = $"{prefix} {_user.FirstName}".Trim();

            GreetingTextBlock.Text = $"{timeOfDay}\n{fullName}";
        }

        private string GetUserGenderPrefix(int userId)
        {
            var user = _db.Users.Find(userId);
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

        public void LoadTabs()
        {
            PatronymicTextBlock.Text = _user.Patronymic ?? "Не указано";
            ParticipantsTextBlock.Text = _db.Users
                .Where(u => _db.Events.Any(e => e.UserID == _user.UserID && u.Events.Any(ue => ue.EventID == e.EventID) && u.Roles.Any(r => r.RoleName == "участник")))
                .Distinct()
                .Count()
                .ToString();
            JuryTextBlock.Text = _db.EventsJury
                .Where(ej => _db.Events.Any(e => e.EventID == ej.EventID && e.UserID == _user.UserID))
                .Select(ej => ej.JuryUserID)
                .Distinct()
                .Count()
                .ToString();
        }

        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new OrganizerProfileWindow(_user)
            {
                Owner = this
            };
            if (profileWindow.ShowDialog() == true)
            {
                LoadUserInfo();
                LoadTabs();
            }
        }

        private void PatronymicButton_Click(object sender, RoutedEventArgs e)
        {
            var eventsWindow = new EventsWindow(_user);
            eventsWindow.ShowDialog();
        }

        private void ParticipantsButton_Click(object sender, RoutedEventArgs e)
        {
            var participantsWindow = new ParticipantsWindow(_user);
            participantsWindow.ShowDialog();
        }

        private void JuryButton_Click(object sender, RoutedEventArgs e)
        {
            var juriesWindow = new JuriesWindow(_user);
            juriesWindow.ShowDialog();
        }

        private void SponsorsButton_Click(object sender, RoutedEventArgs e)
        {
            var activitiesWindow = new ActivitiesWindow(_user);
            activitiesWindow.ShowDialog();
        }

   
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}