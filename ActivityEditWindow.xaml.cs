using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EventsWPF_Practic
{
    public partial class ActivityEditWindow : Window
    {
        private readonly ActivitiesEvents _activity;
        private readonly EventsDB_PracticEntities _db;
        private readonly Users _user;

        public ActivityEditWindow(ActivitiesEvents activity, EventsDB_PracticEntities db, Users user)
        {
            InitializeComponent();
            _activity = activity;
            _db = db;
            _user = user;
            LoadData();
        }

        private void LoadData()
        {
            // Проверка на дефолтное значение для DayNumber (0 для int)
            DayNumberBox.Text = _activity.DayNumber != 0 ? _activity.DayNumber.ToString() : string.Empty;
            // Проверка на дефолтное значение для StartTime (TimeSpan.Zero)
            StartTimeBox.Text = _activity.StartTime != TimeSpan.Zero ? _activity.StartTime.ToString("hh\\:mm") : string.Empty;
            // Загрузка событий для ComboBox
            EventComboBox.ItemsSource = _db.Events.Where(e => e.UserID == _user.UserID).ToList();
            // Установка выбранного события
            if (_activity.EventID != 0) // Предполагаем, что 0 — значение по умолчанию для нового объекта
            {
                EventComboBox.SelectedItem = _db.Events.FirstOrDefault(e => e.EventID == _activity.EventID);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(DayNumberBox.Text, out int dayNumber))
                _activity.DayNumber = dayNumber;
            if (TimeSpan.TryParse(StartTimeBox.Text, out TimeSpan startTime))
                _activity.StartTime = startTime;
            if (EventComboBox.SelectedItem is Events selectedEvent)
                _activity.EventID = selectedEvent.EventID;
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