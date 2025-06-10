using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace EventsWPF_Practic
{
    public partial class MainWindow : Window
    {
        private readonly EventsDB_PracticEntities _db;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _db = new EventsDB_PracticEntities();
                LoadEvents(); // Загружаем мероприятия при старте
                LoadDirections(); // Загружаем направления для фильтра
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}\nДетали: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Исключение: {ex.ToString()}");
            }
        }

        private void LoadEvents()
        {
            var rawEvents = _db.Events.ToList(); // Сначала получаем все события
            var eventsWithDirections = rawEvents
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.EventDate,
                    DirectionName = e.EventTypeDirection.Select(etd => etd.Directions.DirectionName).FirstOrDefault(),
                    PhotoPath = FindImagePath(e.EventID) // Применяем метод после загрузки
                })
                .ToList();

            EventsListView.ItemsSource = eventsWithDirections; // Устанавливаем источник данных
        }

        private void LoadDirections()
        {
            var directions = _db.Directions.ToList();
            DirectionFilter.ItemsSource = directions; // Устанавливаем направления в ComboBox
        }

        private string FindImagePath(int eventId)
        {
            try
            {
                // Папка "Source" в корне проекта
                string projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string sourcePath = Path.Combine(projectDir, "Source");

                if (!Directory.Exists(sourcePath))
                {
                    System.Diagnostics.Debug.WriteLine($"Директория не найдена: {sourcePath}");
                    return null;
                }

                string[] extensions = { ".png", ".jpg", ".jpeg" };
                foreach (var ext in extensions)
                {
                    string fullPath = Path.Combine(sourcePath, eventId.ToString() + ext);
                    if (File.Exists(fullPath))
                    {
                        System.Diagnostics.Debug.WriteLine($"Found image: {fullPath}");
                        return new Uri(fullPath).AbsoluteUri;
                    }
                }

                // Путь к изображению по умолчанию
                string defaultPath = Path.Combine(sourcePath, "14.png");
                if (File.Exists(defaultPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Using default image: {defaultPath}");
                    return new Uri(defaultPath).AbsoluteUri;
                }

                System.Diagnostics.Debug.WriteLine("Default image not found, returning null");
                return null;
            } catch
            {
                return null;
            }
        }
        

        private void DirectionFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void DateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            DirectionFilter.SelectedItem = null;
            DateFilter.SelectedDate = null;
            LoadEvents(); // Сбрасываем фильтры и загружаем все мероприятия
        }

        private void ApplyFilters()
        {
            var rawEvents = _db.Events.ToList(); // Сначала получаем все события
            var query = rawEvents
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.EventDate,
                    DirectionName = e.EventTypeDirection.Select(etd => etd.Directions.DirectionName).FirstOrDefault(),
                    PhotoPath = FindImagePath(e.EventID) // Применяем метод после загрузки
                })
                .AsQueryable();

            // Фильтр по направлению
            if (DirectionFilter.SelectedItem is Directions selectedDirection)
            {
                query = query.Where(ev => ev.DirectionName == selectedDirection.DirectionName);
            }

            // Фильтр по дате
            if (DateFilter.SelectedDate.HasValue)
            {
                var selectedDate = DateFilter.SelectedDate.Value.Date;
                query = query.Where(ev => ev.EventDate.HasValue && ev.EventDate.Value.Date == selectedDate);
            }

            EventsListView.ItemsSource = query.ToList();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно авторизации
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }
    }
}