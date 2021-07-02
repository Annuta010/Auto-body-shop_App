using System;
using System.Collections.Generic;
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
    public sealed partial class Client : Page
    {
        Users users = new Users();
        string table_row_ID;
        Config config = new Config();
        bool edit_mode = false;
        List<int> index_array = new List<int>();
        int[] access_groups = new int[] { 5 };
        bool access = false;
        int action;
        public Client()
        {
            this.InitializeComponent();
            users.GetList("SELECT * FROM `client`", clientsList);
            index_array = users.GetIdexes("SELECT `Client_ID` FROM `client`");
        }

        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            users.GetList("SELECT * FROM `client`", clientsList);
            index_array.Clear();
            index_array = users.GetIdexes("SELECT `Client_ID` FROM `client`");
        }



        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (access)
            {
                editClientsGrid.Visibility = Visibility.Visible;
                clientsList.Visibility = Visibility.Collapsed;
                CommandBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                checkaccessGrid.Visibility = Visibility.Visible;
                clientsList.Visibility = Visibility.Collapsed;
                CommandBar.Visibility = Visibility.Collapsed;
                action = 1;
            }


        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editClientsGrid.Visibility = Visibility.Collapsed;
            clientsList.Visibility = Visibility.Visible;
            CommandBar.Visibility = Visibility.Visible;
            edit_mode = false;
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (edit_mode)
            {

                users.ChangeData("UPDATE `client` SET `Surname_of_client` = '" + ClientLastNameBox.Text + "'," +
                    " `Name_of_client` = '" + ClientNameBox.Text + "', `Middle_name_of_client` = '" + ClientMidNameBox.Text + "' ," +
                    " `Phone_number_of_client` = '" + ClientPhoneBox.Text + "'  WHERE Client_ID = '" + table_row_ID + "' ",
                    "Данные клиента успешно изменены.");
                CancelButton_Click(sender, e);


            }
            else
            {
                Database data_base = new Database();
                // Вызов метода для выборки с передачей SQL-запроса
                var data_table = data_base.Select("SELECT * FROM `client` WHERE(`Surname_of_client` = '" + ClientLastNameBox.Text + "' AND" +
                    " `Name_of_client` = '" + ClientNameBox.Text + "' AND `Middle_name_of_client` = '" + ClientMidNameBox.Text + "' AND " +
                    " `Phone_number_of_client` = '" + ClientPhoneBox.Text + "'  )");
                bool row_count = Convert.ToBoolean(data_table.Rows.Count);

                if (!row_count)
                {

                    users.ChangeData("INSERT INTO client(`Surname_of_client`, `Name_of_client`, `Middle_name_of_client`, `Phone_number_of_client`) " +
                        "values('" + ClientLastNameBox.Text + "', '" + ClientNameBox.Text + "', '" + ClientMidNameBox.Text + "'," +
                        " '" + ClientPhoneBox.Text + "' )", "Новый клиент добавлен");
                    CancelButton_Click(sender, e);

                }
                else
                {
                    await config.Alert("Клиент с такими данными уже есть.", "Ошибка");
                }
            }
            edit_mode = false;
        }


        private void Box_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((ClientLastNameBox.Text != "") && (ClientPhoneBox.Text != ""))
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
        private void groupsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditBtn.IsEnabled = true;
            DeleteBtn.IsEnabled = true;
            try
            {
                table_row_ID = index_array[clientsList.SelectedIndex].ToString();
            }
            catch
            {

            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            AddButton_Click(sender, e);
            edit_mode = true;
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog("Вы действительно хотите удалить этого клиента?", "Подтверждение");

            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Нет", (command) =>
            {

            }));
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Да", (command) =>
            {
                if (access)
                {
                    string[] sql_array = new string[] { "Delete FROM `client` WHERE(`Client_ID` = '" + table_row_ID + "') " };
                    users.Delete(sql_array, "Клиент успешно удален.");
                }
                else
                {
                    checkaccessGrid.Visibility = Visibility.Visible;
                    clientsList.Visibility = Visibility.Collapsed;
                    CommandBar.Visibility = Visibility.Collapsed;
                    action = 2;
                }
            }));
            await messageDialog.ShowAsync();
        }

        private async void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (users.CheckAccess("SELECT `id_group` FROM `users` WHERE (`user_password` = '" + config.Hash(AccessPasswordBox.Password) + "' " +
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
                        DeleteBtn_Click(sender, e);
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
