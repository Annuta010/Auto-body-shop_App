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
    public sealed partial class Supply : Page
    {
        Users users = new Users();
        string table_row_ID;
        Config config = new Config();
        bool edit_mode = false;
        List<int> index_array = new List<int>();
        int[] access_groups = new int[] { 6 };
        bool access = false;
        int action;
        public Supply()
        {
            this.InitializeComponent();
            users.GetList("SELECT `Supply_ID`, `Quantity_of_product`, `Supply_cost`, `Supplier_ID`, `Product_ID` FROM `supply`", employeesList);
            index_array = users.GetIdexes("SELECT `Supply_ID` FROM `supply`");
        }
        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            users.GetList("SELECT `Supply_ID`, `Quantity_of_product`, `Supply_cost`, `Supplier_ID`, `Product_ID` FROM `supply`", employeesList);
            index_array.Clear();
            index_array = users.GetIdexes("SELECT `Supply_ID` FROM `supply`");

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (access)
            {
                editEmployeesGrid.Visibility = Visibility.Visible;
                employeesList.Visibility = Visibility.Collapsed;
                CommandBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                checkaccessGrid.Visibility = Visibility.Visible;
                employeesList.Visibility = Visibility.Collapsed;
                CommandBar.Visibility = Visibility.Collapsed;
                action = 1;
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editEmployeesGrid.Visibility = Visibility.Collapsed;
            employeesList.Visibility = Visibility.Visible;
            CommandBar.Visibility = Visibility.Visible;
            edit_mode = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (edit_mode)
            {

                users.ChangeData("UPDATE `supply` SET `Product_ID` = '" + ProductIdBox.Text + "'," +
                    " `Quantity_of_product` = '" + QuentityBox.Text + "', `Supplier_ID` = '" + SupplierIdBox.Text + "' ," +
                    " `Supply_cost` = '" + SupplyAmountBox.Text + "' " +
                    " WHERE Supply_ID = '" + table_row_ID + "' ", "Данные о поставке успешно изменены.");
                CancelButton_Click(sender, e);


            }
            else
            {
                users.ChangeData("INSERT INTO supply(`Product_ID`,`Quantity_of_product`,`Supplier_ID`, `Supply_cost` ) " +
                        "values('" + ProductIdBox.Text + "', '" + QuentityBox.Text + "', '" + SupplierIdBox.Text + "'," +
                        " '" + SupplyAmountBox.Text + "' )", "Новые данные о поставке добавлены");
                    CancelButton_Click(sender, e);

                
            }
            edit_mode = false;
        }

        private void Box_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((ProductIdBox.Text != "") && (QuentityBox.Text != "") && (SupplierIdBox.Text != "") && (SupplyAmountBox.Text != ""))
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

        private void employeesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditBtn.IsEnabled = true;
            DeleteBtn.IsEnabled = true;
            try
            {
                table_row_ID = index_array[employeesList.SelectedIndex].ToString();
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
            var messageDialog = new MessageDialog("Вы действительно хотите удалить данные об этой поставке?", "Подтверждение");

            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Нет", (command) =>
            {

            }));
            messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Да", (command) =>
            {
                if (access)
                {
                    string[] sql_array = new string[] { "Delete FROM `supply` WHERE(`Supply_ID` = '" + table_row_ID + "') " };
                    users.Delete(sql_array, "Данные о поставке успешно удалены.");
                }
                else
                {
                    checkaccessGrid.Visibility = Visibility.Visible;
                    employeesList.Visibility = Visibility.Collapsed;
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
