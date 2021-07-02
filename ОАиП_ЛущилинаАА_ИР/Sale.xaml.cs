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
    public sealed partial class Sale : Page
    {
        Users users = new Users();
        string table_row_ID;
        Config config = new Config();
        bool edit_mode = false;
        List<int> index_array = new List<int>();
        int[] access_groups = new int[] { 5 };
        bool access = false;
        int action;
        public Sale()
        {
            this.InitializeComponent();
            users.GetList("SELECT * FROM `sale`", clientsList);
            index_array = users.GetIdexes("SELECT `Client_ID` FROM `client`");
        }

        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            users.GetList("SELECT * FROM `sale`", clientsList);
            index_array.Clear();
            index_array = users.GetIdexes("SELECT `Sale_ID` FROM `sale`");
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
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (edit_mode)
            {

                users.ChangeData("UPDATE `sale` SET `Sale_amount` = '" + SaleAmountBox.Text + "'," +
                    " `Client_ID` = '" + ClientIdBox.Text + "', `Warehouse_ID` = '" + ProductIdBox.Text + "' ," +
                    " `Discount_amount` = '" + DiscountBox.Text + "'  WHERE Sale_ID = '" + table_row_ID + "' ",
                    "Данные о продаже успешно изменены.");
                CancelButton_Click(sender, e);


            }
            else
            {
                
                users.ChangeData("INSERT INTO sale(`Sale_amount`, `Client_ID`,`Warehouse_ID`,  `Discount_amount`) " +
                        "values('" + SaleAmountBox.Text + "', '" + ClientIdBox.Text + "', '" + ProductIdBox.Text + "'," +
                        " '" + DiscountBox.Text + "' )", "Новая запись о продаже добавлена");
                    CancelButton_Click(sender, e);

            }
            edit_mode = false;
        }

        private void Box_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((SaleAmountBox.Text != "") && (ProductIdBox.Text != "") && (DiscountBox.Text != ""))
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
            var messageDialog = new MessageDialog("Вы действительно хотите удалить данные об этой продаже?", "Подтверждение");

            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Нет", (command) =>
            {

            }));
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Да", (command) =>
            {
                if (access)
                {
                    string[] sql_array = new string[] { "Delete FROM `sale` WHERE(`Sale_ID` = '" + table_row_ID + "') " };
                    users.Delete(sql_array, "Дфнные о продаже успешно удалены.");
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
