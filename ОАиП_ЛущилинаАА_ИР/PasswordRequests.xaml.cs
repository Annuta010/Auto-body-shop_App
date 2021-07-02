using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class PasswordRequests : Page
    {
        Users users = new Users();
        string table_row_ID;
        string user_row_ID;
        Config config = new Config();
        List<int> request_index_array = new List<int>();
        List<int> user_index_array = new List<int>();
        int[] access_groups = new int[] { 1 };
        bool access = false;
        public PasswordRequests()
        {
            this.InitializeComponent();
            Database db = new Database();
            db.Insert("DELETE FROM `reset_requests` WHERE TIMESTAMPDIFF(HOUR, `time`, CURDATE() ) > 24");
            users.GetList("SELECT * FROM `reset_requests`", requestList);
            request_index_array = users.GetIdexes("SELECT `id_request` FROM `reset_requests`");
            user_index_array = users.GetIdexes("SELECT `id_user` FROM `reset_requests`");
        }
        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            users.GetList("SELECT * FROM `reset_requests`", requestList);
            request_index_array.Clear();
            user_index_array.Clear();
            request_index_array = users.GetIdexes("SELECT `id_request` FROM `reset_requests`");
            user_index_array = users.GetIdexes("SELECT `id_user` FROM `reset_requests`");
        }
        
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (access)
            {
                processRequestGrid.Visibility = Visibility.Visible;
                requestList.Visibility = Visibility.Collapsed;
                CommandBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                checkaccessGrid.Visibility = Visibility.Visible;
                requestList.Visibility = Visibility.Collapsed;
                CommandBar.Visibility = Visibility.Collapsed;
            }
        }
       

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Database data_base = new Database();
            // Вызов метода для выборки с передачей SQL-запроса
            var data_table = data_base.Select("SELECT * FROM `users` WHERE(`users`.`id_user` = '" + user_row_ID + "')");
            bool row_count = Convert.ToBoolean(data_table.Rows.Count);

            if (row_count)
            {
               
               users.ChangeData("UPDATE `users` SET `user_password` = '" + config.Hash(PasswordBox.Password) + "' WHERE `users`.`id_user` = '" + user_row_ID + "' ",
                        "Данные порльзователя успешно изменены.");
                CancelButton_Click(sender, e);
                Database db = new Database();
                db.Insert("Delete FROM `reset_requests` WHERE(`id_request` = '" + table_row_ID + "') ");
                RenewButton_Click(sender, e);
                CancelButton_Click(sender, e);
            }
            else
            {
                await config.Alert("Такого пользователя нет.", "Ошибка");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            processRequestGrid.Visibility = Visibility.Collapsed;
            requestList.Visibility = Visibility.Visible;
            CommandBar.Visibility = Visibility.Visible;
        }

        private void requestList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditBtn.IsEnabled = true;
            try
            {
                table_row_ID = request_index_array[requestList.SelectedIndex].ToString();
                user_row_ID = user_index_array[requestList.SelectedIndex].ToString();
            }
            catch
            {

            }
        }

        private void Box_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ( PasswordBox.Password != "")
            {
                SaveButton.IsEnabled = true;
            }
        }

        private void AccessBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((AccessLoginBox.Text != "") && (AccessPasswordBox.Password != ""))
            {
                CheckButton.IsEnabled = true;
            }
        }

        private async void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (users.CheckAccess("SELECT `id_group` FROM `users` WHERE (`user_password` = '" + config.Hash(AccessPasswordBox.Password) + "' " +
                "AND (`user_email` = '" + AccessLoginBox.Text + "' OR `user_login` = '" + AccessLoginBox.Text + "') ) ", access_groups))
            {
                access = true;
                checkaccessGrid.Visibility = Visibility.Collapsed;
                EditButton_Click(sender, e);
                
            }
            else
            {
                await config.Alert("Ошибка доступа", "Ошибка");
            }
        }
    }
}
