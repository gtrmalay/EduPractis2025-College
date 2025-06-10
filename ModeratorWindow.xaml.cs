using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EventsWPF_Practic
{
    public partial class ModeratorWindow : Window
    {
        private readonly Users _user;
        private readonly EventsDB_PracticEntities _db;

        public ModeratorWindow(Users user)
        {
            InitializeComponent();
            _user = user;
            _db = new EventsDB_PracticEntities();
            LoadUserInfo();
            LoadAvailableEvents(); // Загружаем мероприятия при старте
        }

        private void LoadUserInfo()
        {
            GreetingTextBlock.Text = $"Добро пожаловать, {_user.FirstName} {_user.Patronymic}!";
        }

        private void LoadAvailableEvents()
        {
            var events = _db.Events.ToList();
            EventsComboBox.ItemsSource = events;
            if (events.Any())
            {
                EventsComboBox.SelectedIndex = 0; // Выбираем первое мероприятие по умолчанию
            }
        }

        private void EventsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventsComboBox.SelectedItem != null)
            {
                int selectedEventId = (int)EventsComboBox.SelectedValue;
                LoadAvailableActivities(selectedEventId);
            }
        }

        private void LoadAvailableActivities(int eventId)
        {
            var activities = (from ae in _db.ActivitiesEvents
                              join e in _db.Events on ae.EventID equals e.EventID
                              where ae.EventID == eventId
                              select new ModeratedActivity
                              {
                                  ActivityID = ae.ActivityID,
                                  EventID = e.EventID,
                                  EventName = e.EventName,
                                  DayNumber = ae.DayNumber
                              }).Distinct().ToList();
            ActivitiesListView.ItemsSource = activities;
        }

        private void RegisterActivityButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedActivity = ActivitiesListView.SelectedItem as ModeratedActivity;
            if (selectedActivity == null)
            {
                MessageBox.Show("Пожалуйста, выберите активность для модерации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем, не назначен ли пользователь уже как модератор
            bool alreadyAssigned = _db.EventsJury.Any(ej =>
                ej.EventID == selectedActivity.EventID &&
                ej.ActivityID == selectedActivity.ActivityID &&
                ej.JuryUserID == _user.UserID);

            if (alreadyAssigned)
            {
                MessageBox.Show("Вы уже назначены модератором на эту активность.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Создаем новую запись в EventsJury
            var newAssignment = new EventsJury
            {
                EventID = selectedActivity.EventID,
                ActivityID = selectedActivity.ActivityID,
                JuryUserID = _user.UserID
            };

            try
            {
                _db.EventsJury.Add(newAssignment);
                _db.SaveChanges();
                MessageBox.Show("Вы успешно назначены модератором на эту активность!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAvailableActivities(selectedActivity.EventID); // Обновляем список для текущего мероприятия
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ModeratedActivity
    {
        public int ActivityID { get; set; }
        public int EventID { get; set; }
        public string EventName { get; set; }
        public int DayNumber { get; set; }
    }
}