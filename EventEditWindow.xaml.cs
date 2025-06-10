using System;
using System.Windows;

namespace EventsWPF_Practic
{
    public partial class EventEditWindow : Window
    {
        private readonly Events _event;
        private readonly EventsDB_PracticEntities _db;

        public EventEditWindow(Events evnt, EventsDB_PracticEntities db)
        {
            InitializeComponent();
            _event = evnt;
            _db = db;
            LoadData();
        }

        private void LoadData()
        {
            EventNameBox.Text = _event.EventName ?? string.Empty;
            EventDatePicker.SelectedDate = _event.EventDate;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _event.EventName = EventNameBox.Text.Trim();
            _event.EventDate = EventDatePicker.SelectedDate;
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