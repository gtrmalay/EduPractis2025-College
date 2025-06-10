using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EventsWPF_Practic;

namespace EventsWPF_Practic
{
    public partial class ParticipantsWindow : Window
    {
        private readonly EventsDB_PracticEntities _db;
        private readonly Users _user;

        public ParticipantsWindow(Users user)
        {
            InitializeComponent();
            _user = user;
            _db = new EventsDB_PracticEntities();
            LoadParticipants();
        }

        private void LoadParticipants()
        {
            var participants = _db.Users
                .Where(u => u.Roles.Any(r => r.RoleName == "участник")) // Только участники
                .Distinct()
                .ToList();
            Console.WriteLine($"Найдено участников: {participants.Count}");
            if (!participants.Any())
            {
                Console.WriteLine("Нет участников.");
            }
            ParticipantsListView.ItemsSource = participants;
        }

        private void AddParticipantButton_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new Users { UserID = 0 };
            var editWindow = new UserEditWindow(newUser, _db, _user);
            if (editWindow.ShowDialog() == true)
            {
                _db.Users.Add(newUser);
                var eventId = _db.Events.FirstOrDefault()?.EventID; // Привязка к первому событию
                if (eventId.HasValue)
                {
                    var existingEvent = _db.Events.FirstOrDefault(evnt => evnt.EventID == eventId.Value);
                    if (existingEvent != null)
                    {
                        newUser.Events.Add(existingEvent);
                        newUser.Roles.Add(new Roles { RoleName = "участник" });
                        _db.SaveChanges();
                    }
                }
                LoadParticipants();
            }
        }

        private void DeleteParticipantButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (ParticipantsListView.SelectedItem is Users selectedUser)
            {
                var eventToRemove = selectedUser.Events.FirstOrDefault();
                if (eventToRemove != null)
                {
                    selectedUser.Events.Remove(eventToRemove);
                }
                var roleToRemove = selectedUser.Roles.FirstOrDefault(r => r.RoleName == "участник");
                if (roleToRemove != null)
                {
                    _db.Roles.Remove(roleToRemove);
                }
                _db.Users.Remove(selectedUser);
                _db.SaveChanges();
                LoadParticipants();
            }
            else
            {
                MessageBox.Show("Выберите участника для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}