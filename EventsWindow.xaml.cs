using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EventsWPF_Practic
{
    public partial class EventsWindow : Window
    {
        private readonly EventsDB_PracticEntities _db;
        private readonly Users _user;

        public EventsWindow(Users user)
        {
            InitializeComponent();
            _user = user;
            _db = new EventsDB_PracticEntities();
            LoadEvents();
        }

        private void LoadEvents()
        {
            var events = _db.Events.ToList(); // Загружаем все события
            if (!events.Any())
            {
                Console.WriteLine("Нет мероприятий в базе данных.");
            }
            EventsListView.ItemsSource = events;
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            var newEvent = new Events { UserID = _user.UserID }; // Связываем с текущим пользователем
            var editWindow = new EventEditWindow(newEvent, _db);
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    _db.Events.Add(newEvent);
                    _db.SaveChanges();
                    Console.WriteLine($"Сохранено мероприятие с ID: {newEvent.EventID}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
                    MessageBox.Show("Ошибка при сохранении мероприятия.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                LoadEvents();
            }
        }

        private void DeleteEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (EventsListView.SelectedItem is Events selectedEvent)
            {
                _db.Events.Remove(selectedEvent);
                _db.SaveChanges();
                LoadEvents();
            }
            else
            {
                MessageBox.Show("Выберите мероприятие для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

       
    }
}