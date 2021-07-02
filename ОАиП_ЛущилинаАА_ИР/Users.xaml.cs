using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace ОАиП_ЛущилинаАА_ИР
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>

    public sealed partial class Users : Page
    {
        string table_row_ID;
        Config config = new Config();
        bool edit_mode = false;
        List <int> index_array = new List<int>();
        int[] access_groups = new int[] { 1 };
        bool access = false;
        int action;
        public Users()
        {
            this.InitializeComponent();
            GetList("SELECT * FROM `users_list`", userList);
            index_array = GetIdexes("SELECT `Ключ` FROM `users_list`");

        }

        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            GetList("SELECT * FROM `users_list`", userList);
            index_array.Clear();
            index_array = GetIdexes("SELECT `Ключ` FROM `users_list`");
        }

       

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (access)
            {
                addUserGrid.Visibility = Visibility.Visible;
                userList.Visibility = Visibility.Collapsed;
                CommandBar.Visibility = Visibility.Collapsed;
            } 
            else
            {
                checkaccessGrid.Visibility = Visibility.Visible;
                userList.Visibility = Visibility.Collapsed;
                CommandBar.Visibility = Visibility.Collapsed;
                action = 1;
            }
               

        }
       
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            addUserGrid.Visibility = Visibility.Collapsed;
            userList.Visibility = Visibility.Visible;
            CommandBar.Visibility = Visibility.Visible;
            edit_mode = false;
            EmployeeIDBox.Text = "";
            userNameBox.Text = "";
            EmployeeIDBox.IsEnabled = true;
            userNameBox.IsEnabled = true;
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (PasswordBox.Password == RepeatPasswordBox.Password)
            {
                if (edit_mode)
                {
                    
                    ChangeData("UPDATE `users` SET `user_password` = '" + config.Hash(PasswordBox.Password) + "',`id_group` = '" + UserIDBox.Text + "'," +
                        " `user_email` = '" + MailBox.Text + "' WHERE `users`.`id_user` = '" + table_row_ID + "' ",
                        "Данные порльзователя успешно изменены.");
                    CancelButton_Click(sender, e);

                } 
                else
                {
                    Database data_base = new Database();
                    // Вызов метода для выборки с передачей SQL-запроса
                    var data_table = data_base.Select("SELECT * FROM `users` WHERE(`users`.`user_login` = '" + userNameBox.Text + "')");
                    bool row_count = Convert.ToBoolean(data_table.Rows.Count);

                    if (!row_count)
                    {

                        ChangeData("INSERT INTO users(`user_login`, `user_email`, `id_group`, `id_employee`, `user_password`) " +
                            "values('" + userNameBox.Text + "', '" + MailBox.Text + "'," +
                            " '" + UserIDBox.Text + "', '" + EmployeeIDBox.Text + "', '" + config.Hash(PasswordBox.Password) + "' )",
                            "Пользователь добавлен");
                        CancelButton_Click(sender, e);

                    }
                    else
                    {
                        await config.Alert("Пользователь с таким именем уже есть. Введите другое имя пользователя.", "Ошибка");
                    }
                }
                edit_mode = false;
               
            }
            else
            {
                await config.Alert("Пароль и повторно введенный пароль не совпадают. Проверьте введенные данные.", "Ошибка");
            }
        }
       
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Объявление нового сообщения 
            var messageDialog = new MessageDialog("Вы действительно хотите удалить этого пользователя?", "Подтверждение");
            //// Добавление в сообщение кнопок вместе с передачей процедур по нажатию на них.
            //От повторной загрузки экрана до выполнения запроса и обновления страницы
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Нет", (command) =>
            {
               
            }));
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Да", (command) =>
            {
                if (access)
                {
                    string[] sql_array = new string[] {"Delete FROM `users` WHERE(`users`.`id_user` = '" + table_row_ID + "') ",
                    "Delete FROM `tokens` WHERE(`tokens`.`id_user` = '" + table_row_ID + "') "};
                    Delete(sql_array, "Пользователь успешно удален");
                }
                else
                {
                    checkaccessGrid.Visibility = Visibility.Visible;
                    userList.Visibility = Visibility.Collapsed;
                    CommandBar.Visibility = Visibility.Collapsed;
                    action = 2;
                }
                

            }));
            await messageDialog.ShowAsync();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeeIDBox.Text = " ";
            userNameBox.Text = " ";
            EmployeeIDBox.IsEnabled = false;
            userNameBox.IsEnabled = false;
            edit_mode = true;
            AddButton_Click(sender, e);

        }
        
        private void userList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            EditBtn.IsEnabled = true;
            DeleteBtn.IsEnabled = true;
            try
            {
                table_row_ID = index_array[userList.SelectedIndex].ToString();
            }
            catch
            {
                
            }
        }

        private void AccessBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((AccessLoginBox.Text != "") && (AccessPasswordBox.Password != ""))
            {
                CheckButton.IsEnabled = true;
            }
        }
        private void Box_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (userNameBox.Text != "" && MailBox.Text != "" && RepeatPasswordBox.Password != "" && PasswordBox.Password != "" && UserIDBox.Text != "" && EmployeeIDBox.Text != "")
            {
                SaveButton.IsEnabled = true;
            }
        }

        public List<int> GetIdexes(string sql)
        {
            Database data_base = new Database();
            DataTable data_table = data_base.Select(sql);
            var indexes = new List<int>();
            foreach (DataRow row in data_table.Rows)
            {
                foreach (DataColumn dc in data_table.Columns)
                {
                    string temp = row[dc].ToString();
                    indexes.Add(Convert.ToInt32(temp));
                }
            }
            return indexes;
        }
        public void GetList(string sql, DataGrid dataGrid)
        {
            Database data_base = new Database();
            // Запрос на выборку из представления
            DataTable data_table = data_base.Select(sql);
            // Очистка столбцов по умолчанию в таблице с данными
            dataGrid.Columns.Clear();
            // Цикл, который на основе выборки добавляет необходимые колонки в таблицу с данными
            for (int i = 0; i < data_table.Columns.Count; i++)
            {
                dataGrid.Columns.Add(new DataGridTextColumn()
                {
                    // Заголовок задаётся на основе такового в MySQL (AS)
                    Header = data_table.Columns[i].ColumnName,
                    // Преобразование данные
                    Binding = new Binding
                    {
                        Path = new PropertyPath("[" + i.ToString() + "]")
                    }
                });
            }
            // Создание коллекции с данными, которые будут принимать в себя  выборку
            var collection = new ObservableCollection<object>();
            // Цикл, который добавляет каждую строку выборку в коллекцию
            foreach (DataRow row in data_table.Rows)
            {
                collection.Add(row.ItemArray);
            }
            // Запись значений коллекции в таблицу с данными
            dataGrid.ItemsSource = collection;
        }
        public async void ChangeData(string sql, string success_message)
        {
            Database data_base = new Database();
            try
            {
                data_base.Insert(sql);
                await config.Alert(success_message, "Успешно");

            }
            catch (Exception ex)
            {
                await config.Alert("Произошла ошибка: " + ex.ToString(), "Ошибка");
            }
        }
        public async void Delete(string [] sql_arr, string success_mes)
        {
            try
            {
                var data_base = new Database();
                foreach (string str_sql in sql_arr)
                {
                    data_base.Insert(str_sql);
                }
               
                await config.Alert(success_mes, "Успешно");
            }
            catch (Exception ex)
            {
                await config.Alert("Произошла ошибка: " + ex.ToString(), "Ошибка");
            }
        }

        public bool CheckAccess(string sql, int [] arr)
        {
            Database db = new Database();
            DataTable dt = db.Select(sql);
            bool result = false;
            if (dt.Rows.Count != 0)
            { 
                
                DataRow row = dt.Rows[0];
                int temp = Convert.ToInt32(row.ItemArray[0]);
                
                foreach (int item in arr)
                {
                    if(item == temp)
                    {
                        result = true;
                        break;
                    }
                    
                }

            }
            
            return result;
        }

        private async void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckAccess("SELECT `id_group` FROM `users` WHERE (`user_password` = '" + config.Hash(AccessPasswordBox.Password) + "' " +
                "AND (`user_email` = '" + AccessLoginBox.Text + "' OR `user_login` = '" + AccessLoginBox.Text + "') ) ", access_groups))
            {
                access = true;
                checkaccessGrid.Visibility = Visibility.Collapsed;
                switch (action)
                {
                    case 1:
                        AddButton_Click(sender, e);
                        break;
                    case 2:
                        DeleteButton_Click(sender, e);
                        break;
                }
                
            }
            else
            {
                await config.Alert("Ошибка доступа", "Ошибка");
            }
        }
    }
}
