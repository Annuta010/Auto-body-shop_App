using MySqlConnector;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Windows.UI.Popups;

namespace ОАиП_ЛущилинаАА_ИР
{
    class Config
    {
        // Переменная для создания нового экземпляра класса MySqlConnection и описание подключения к серверу базы данных
        public MySqlConnection connection = new MySqlConnection("server=localhost; user=root;" +
            "password=;database=db13;port=3306");

        // Метод для получения значений из объектов по ключу, возвращающий строковый тип данных
        public string GetValue(object Object, string Key)
        {
            string Value = Object.GetType().GetProperty(Key).
            GetValue(Object, null).ToString();
            return Value;
        }

        // Метод для создания оповещений на экране, принимающий в себя строку для названия и заголовка соответственно
        public async System.Threading.Tasks.Task Alert(string Message, string Title)
        {
            var messageDialog = new MessageDialog(Message, Title);
            await messageDialog.ShowAsync();
        }

        // Объявление переменной для дальнейшей работы с кэшем данных
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        // Объявление переменной для хранения ключа сервера для шифрования передаваемых паролей
        public string salt = "AHYP1114QQQO";
        // Создание хэша из строки
        public string Hash(string input)
        {
            // Использование стандартных криптографических функций для имитации алгоритм хеширования MD5
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(input + salt);
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            // Сохранение и преобразование результата работы в переменную
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            // Возвращение строки
            return hashedString;
        }
        // Занесение действий в журнал
        public void Log(int user, int action)
        {
            Database data_base = new Database();
            string sql = "INSERT INTO `logs` (`id_user`, `id_action`) VALUES ('" + user + "', '" + action + "')";
            data_base.Insert(sql);
        }

        // Генерация ключа безопасности
        public string GenerateToken(int user)
        {
            Database data_base = new Database();
            // Объявление экземпляра класса Random для создания случайной строки(будущего ключа безопасности)
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string token = new string(Enumerable.Repeat(chars, 64).Select(s => s[random.Next(s.Length)]).ToArray());
            string sql = "INSERT INTO `tokens` (`id_user`, `token`) VALUES (" + user + ", '" + token + "')";
            data_base.Insert(sql);
            /* Вызов метода занесения действий в журнал с передачей идентификатора пользователя и первого типа действий в
            соответствии с базой данных(авторизация) */
           Log(user, 1);
           // Возвращение ключа безопасности для его дальнейшего использования
           return token;
        }
    }
}
