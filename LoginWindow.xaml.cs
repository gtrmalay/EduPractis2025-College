using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EventsWPF_Practic
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;
        private int _loginAttempts = 0;
        private bool _isLocked = false;
        private string _captchaText;

        public LoginWindow()
        {
            InitializeComponent();
            _userService = new UserService();
            GenerateCaptcha();
            LoadCredentials();
        }

        private void GenerateCaptcha()
        {
            _captchaText = GenerateRandomString(4);
            const int width = 100;
            const int height = 40;

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));
                var formattedText = new FormattedText(
                    _captchaText,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    16,
                    Brushes.Black,
                    1.25);
                drawingContext.DrawText(formattedText, new Point(10, 10));
                var random = new Random();
                for (int i = 0; i < 50; i++)
                {
                    int x = random.Next(width);
                    int y = random.Next(height);
                    drawingContext.DrawRectangle(Brushes.Gray, null, new Rect(x, y, 1, 1));
                }
            }

            var renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);
            CaptchaImage.Source = renderBitmap;
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456";
            var random = new Random();
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }

        private void RefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
            CaptchaTextBox.Text = "";
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLocked)
            {
                MessageBox.Show("Система заблокирована. Подождите 10 секунд.");
                return;
            }

            if (CaptchaTextBox.Text != _captchaText)
            {
                MessageBox.Show("Неверная CAPTCHA.");
                GenerateCaptcha();
                CaptchaTextBox.Text = "";
                return;
            }

            var user = _userService.Authenticate(IdNumberTextBox.Text, PasswordBox.Password);
            if (user == null)
            {
                _loginAttempts++;
                if (_loginAttempts >= 3)
                {
                    _isLocked = true;
                    MessageBox.Show("Система заблокирована на 10 секунд.");
                    System.Threading.Thread.Sleep(10000);
                    _isLocked = false;
                    _loginAttempts = 0;
                }
                else
                {
                    MessageBox.Show($"Неверные учетные данные. Осталось попыток: {3 - _loginAttempts}");
                }
                GenerateCaptcha();
                CaptchaTextBox.Text = "";
                return;
            }

            if (RememberMeCheckBox.IsChecked == true)
            {
                Properties.Settings.Default.IdNumber = IdNumberTextBox.Text;
                Properties.Settings.Default.Save();
            }

            // Отладка: выводим роли пользователя
            var roles = string.Join(", ", user.Roles.Select(r => r.RoleName));
            if (string.IsNullOrEmpty(roles))
            {
                MessageBox.Show($"Ошибка: у пользователя с ID {user.UserID} не назначена роль. Роли: {roles}");
                return;
            }

            Window targetWindow = null;
            var roleName = user.Roles.FirstOrDefault()?.RoleName;
            if (roleName == "организатор")
            {
                targetWindow = new OrganizerWindow(user);
            }
            else if (roleName == "участник")
            {
                targetWindow = new ParticipantWindow(user);
            }
            else if (roleName == "модератор")
            {
                targetWindow = new ModeratorWindow(user);
            }
            else if (roleName == "жюри")
            {
                targetWindow = new JuryWindow(user);
            }
            else
            {
                MessageBox.Show($"Неизвестная роль пользователя: {roleName}");
                return;
            }

            targetWindow.Show();
            Close();
        }

        private void LoadCredentials()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.IdNumber))
            {
                IdNumberTextBox.Text = Properties.Settings.Default.IdNumber;
                RememberMeCheckBox.IsChecked = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}