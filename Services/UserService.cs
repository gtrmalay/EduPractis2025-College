using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity; // Для Include

namespace EventsWPF_Practic
{
    public class UserService
    {
        private readonly EventsDB_PracticEntities _db;

        public UserService()
        {
            _db = new EventsDB_PracticEntities();
        }

        public Users Authenticate(string idNumber, string password)
        {
            var user = _db.Users
                .Include(u => u.Roles) // Подгружаем роли
                .FirstOrDefault(u => u.UserID.ToString() == idNumber.Trim() || u.EmailAddress == idNumber.Trim());
            if (user != null && VerifyPassword(password, user.UserPass))
            {
                return user;
            }
            return null;
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            // Временная проверка без хэширования
            return inputPassword == storedPassword;

            // Оригинальная версия (раскомментировать после синхронизации базы)
            // return HashPassword(inputPassword) == storedPassword;
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public void RegisterJury(Users user, int? eventId)
        {
            using (var db = new EventsDB_PracticEntities())
            {
                // Проверка уникальности UserID
                if (db.Users.Any(u => u.UserID == user.UserID))
                {
                    throw new InvalidOperationException("Пользователь с таким ID уже существует.");
                }

                // Добавление нового пользователя
                db.Users.Add(user);

                // Привязка к мероприятию
                if (eventId.HasValue)
                {
                    var existingEvent = db.Events.Find(eventId.Value);
                    if (existingEvent == null)
                    {
                        throw new InvalidOperationException("Указанное мероприятие не найдено.");
                    }

                    if (!db.EventsJury.Any(ej => ej.EventID == eventId.Value && ej.JuryUserID == user.UserID))
                    {
                        var eventJury = new EventsJury
                        {
                            EventID = eventId.Value,
                            JuryUserID = user.UserID,
                            ActivityID = 1 // Заглушка, замените на реальное значение
                        };
                        db.EventsJury.Add(eventJury);
                    }
                }

                try
                {
                    int changes = db.SaveChanges();
                    Console.WriteLine($"Сохранено изменений: {changes}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
                    throw;
                }
            }
        }

        public string GenerateUserId()
        {
            return (_db.Users.Any() ? _db.Users.Max(u => u.UserID) + 1 : 1).ToString();
        }

      
    }
}