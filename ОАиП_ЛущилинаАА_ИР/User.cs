using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ОАиП_ЛущилинаАА_ИР
{
    class User : Config
    {
        // Перечисления свойств класса, которые будут использоваться в методах
        private int ID_User { get; set; }
        private bool Status { get; set; }
        private int ID_Group { get; set; }
        private string Login { get; set; }
        private string Email { get; set; }
        private string LastName { get; set; }
        private string FirstName { get; set; }
        private string MidName { get; set; }

        Database data_base = new Database();
        // Первичная аутентификация
        public bool Autentification(string LoginBox, string PasswordBox)
        {
            var data_table = data_base.Select("SELECT `id_user`,`id_group`, `user_login`, `user_email`, `user_password`, `last_name`, `first_name`, `mid_name` FROM `users` JOIN`employees`" +
                " WHERE(`users`.`user_login` = '" + LoginBox + "'OR `users`.`user_email` = '" + LoginBox + "') " +
                "AND(BINARY`users`.`user_password` = '" + Hash(PasswordBox) + "') AND(`users`.`id_user` = `employees`.`id_employee`)");
            Status = Convert.ToBoolean(data_table.Rows.Count);
            if (Status)
            {
                ID_User = Convert.ToInt32(data_table.Rows[0]["id_user"]);
                ID_Group = Convert.ToInt32(data_table.Rows[0]["id_group"]);
                Login = data_table.Rows[0]["user_login"].ToString();
                Email = data_table.Rows[0]["user_email"].ToString();
                LastName = data_table.Rows[0]["last_name"].ToString();

                FirstName = data_table.Rows[0]["first_name"].ToString();
                MidName = data_table.Rows[0]["mid_name"].ToString();
                CacheAuth();
            }
            return Status;
        }
           
    // Кэширование данных аутентификации
    private void CacheAuth()
        {
            // Объявление переменной Token, которая будет хранить в себе результат работы метода на основе идентификатора пользователя
            string Token = GenerateToken(ID_User);
            /* Занесение данных в локальное хранилище со значениями. Строчными буквами указан адрес,
             * а в тексте с использованием прописных записывается значение из переменный по её имени */
            localSettings.Values["id_user"] = ID_User;
            localSettings.Values["status"] = Status;
            localSettings.Values["id_group"] = ID_Group;
            localSettings.Values["user_login"] = Login;
            localSettings.Values["user_email"] = Email;
            localSettings.Values["last_name"] = LastName;
            localSettings.Values["first_name"] = FirstName;
            localSettings.Values["mid_name"] = MidName;
            localSettings.Values["token"] = Token;
        }
        // Проверка наличия сохранённых данных аутентификации
        public int CheckAuth()
        {
            try
            {
                string sql = "SELECT `tokens`.`id_user`, `token`,`last_name`, `first_name`, `mid_name` FROM `tokens`JOIN " +
                    "`users`, `employees` WHERE(`tokens`.`id_user` ='" + localSettings.Values["id_user"] + "') " +
                    "AND(`token` = '" + localSettings.Values["token"] + "') AND(`deactivated` = 0) AND(`tokens`.`id_user` " +
                    "= `users`.`id_user` AND `users`.`id_employee` =`employees`.`id_employee`)";
                var data_table = data_base.Select(sql);
                bool status = Convert.ToBoolean(data_table.Rows.Count);
                if (status && Convert.ToBoolean(localSettings.Values["status"]))
                {
                    /* Проверка на случай несовпадения ФИО в локальном хранилище и на сервере базы данных
                    на случай изменения данных сотрудника */
                    if (localSettings.Values["last_name"] != data_table.Rows[0]["last_name"].ToString() ||
                    localSettings.Values["first_name"] != data_table.Rows[0]["first_name"].ToString() ||
                    localSettings.Values["mid_name"] != data_table.Rows[0]["mid_name"].ToString())
                    {
                        // В случае несоответствия повторная запись всех данных о ФИО
                        localSettings.Values["last_name"] = data_table.Rows[0]["last_name"].ToString();
                        localSettings.Values["first_name"] = data_table.Rows[0]["first_name"].ToString();
                        localSettings.Values["mid_name"] = data_table.Rows[0]["mid_name"].ToString();
                    }
                    // Возвращения кода (статуса) метода для дальнейшей обработки
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 2;
            }
        }


    }
    
}
            