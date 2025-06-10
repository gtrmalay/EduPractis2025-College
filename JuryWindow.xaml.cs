using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EventsWPF_Practic
{
    public partial class JuryWindow : Window
    {
        private readonly Users _user;
        private readonly EventsDB_PracticEntities _db;

        public JuryWindow(Users user)
        {
            InitializeComponent();
            _user = user;
            _db = new EventsDB_PracticEntities();
            LoadUserInfo();
            LoadEvaluationActivities();
        }

        private void LoadUserInfo()
        {
            GreetingTextBlock.Text = $"Добро пожаловать, {_user.FirstName} {_user.Patronymic}!";
        }

        private void LoadEvaluationActivities()
        {
            var activities = (from ej in _db.EventsJury
                              where ej.JuryUserID == _user.UserID
                              join e in _db.Events on ej.EventID equals e.EventID
                              select new
                              {
                                  ej.ActivityID,
                                  EventName = e.EventName,
                                  EventDate = e.EventDate,
                                  ParticipantsCount = 0, // Временная заглушка
                                  AverageScore = 0.0 // Временная заглушка
                              }).ToList();
            ActivitiesListView.ItemsSource = activities;
        }
    }
}