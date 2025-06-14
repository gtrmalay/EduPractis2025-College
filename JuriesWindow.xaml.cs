﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EventsWPF_Practic
{
    public partial class JuriesWindow : Window
    {
        private readonly EventsDB_PracticEntities _db;
        private readonly Users _user;

        public JuriesWindow(Users user)
        {
            InitializeComponent();
            _user = user;
            _db = new EventsDB_PracticEntities();
            LoadJury();
        }

        private void LoadJury()
        {
            var jury = _db.Users
                .Join(_db.EventsJury, u => u.UserID, ej => ej.JuryUserID, (u, ej) => new { User = u, EventJury = ej })
                .Select(x => x.User)
                .Distinct()
                .ToList();
            Console.WriteLine($"Найдено жюри: {jury.Count}");
            if (!jury.Any())
            {
                Console.WriteLine("Нет жюри.");
            }
            JuryListView.ItemsSource = jury;
        }

        private void AddJuryButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_user.Roles.Any(r => r.RoleName == "организатор"))
            {
                MessageBox.Show("У вас нет прав для выполнения этой операции.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var registerJuryWindow = new RegisterJuryWindow();
            registerJuryWindow.ShowDialog();
        }

        private void DeleteJuryButton_Click(object sender, RoutedEventArgs e)
        {
            if (JuryListView.SelectedItem is Users selectedUser)
            {
                var juryEntry = _db.EventsJury.FirstOrDefault(ej => ej.JuryUserID == selectedUser.UserID);
                if (juryEntry != null)
                {
                    _db.EventsJury.Remove(juryEntry);
                    _db.SaveChanges();
                }
                LoadJury();
            }
            else
            {
                MessageBox.Show("Выберите члена жюри для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}