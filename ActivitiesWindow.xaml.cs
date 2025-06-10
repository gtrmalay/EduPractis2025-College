using System;
using System.Data.Entity; // Для Include
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EventsWPF_Practic
{
    public partial class ActivitiesWindow : Window
    {
        private readonly EventsDB_PracticEntities _db;
        private readonly Users _user;

        public ActivitiesWindow(Users user)
        {
            InitializeComponent();
            _user = user;
            _db = new EventsDB_PracticEntities();
            LoadActivities();
        }

        private void LoadActivities()
        {
            var activities = _db.ActivitiesEvents
                .Include(ae => ae.Activities) // Включаем связанные данные из Activities
                .ToList();
            Console.WriteLine($"Найдено активностей: {activities.Count}");
            if (!activities.Any())
            {
                Console.WriteLine("Нет активностей.");
            }
            ActivitiesListView.ItemsSource = activities;
        }

        private void AddActivityButton_Click(object sender, RoutedEventArgs e)
        {
            var newActivity = new ActivitiesEvents { ModeratorUserID = _user.UserID };
            var editWindow = new ActivityEditWindow(newActivity, _db, _user);
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    _db.ActivitiesEvents.Add(newActivity);
                    _db.SaveChanges();
                    Console.WriteLine($"Сохранена активность с ID: {newActivity.ActivityID}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при сохранении активности: {ex.Message}");
                    MessageBox.Show("Ошибка при сохранении активности.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                LoadActivities();
            }
        }

        private void DeleteActivityButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActivitiesListView.SelectedItem is ActivitiesEvents selectedActivity)
            {
                _db.ActivitiesEvents.Remove(selectedActivity);
                _db.SaveChanges();
                LoadActivities();
            }
            else
            {
                MessageBox.Show("Выберите активность для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}