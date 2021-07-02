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
    public sealed partial class PanelFrame : Page
    {
        Config config = new Config();
        public PanelFrame()
        {
            this.InitializeComponent();
            /* Объявление переменную для хранения инициалов пользователя, 
            обрезание значения «Имя» до одного символа с точкой */
            string initials = " " + config.localSettings.Values["first_name"].ToString().Substring(0, 1) + ".";
            /* Проверка на пустое значение поля «Отчество». В случае
            наличия, происходит обрезание значения до одного символа с
            точкой по аналогии с именем сотрудника */
            if (config.localSettings.Values["mid_name"] != "") initials += " " +
                    config.localSettings.Values["mid_name"].ToString().Substring(0, 1) + ".";
            // Назначение содержания на элемент
            UserProfileBtn.Content = config.localSettings.Values["last_name"] + initials;

           //lk
            NavigationViewItem edit_lk = new NavigationViewItem();
            edit_lk.Content = "Личный кабинет";
            edit_lk.Icon = new SymbolIcon(Symbol.Account);
            edit_lk.Tapped += (s, e) =>
            {
                userMenu.Navigate(typeof(lk));
            };
            workSpace.MenuItems.Add(edit_lk);
            //employees
            NavigationViewItem edit_employees = new NavigationViewItem();
            edit_employees.Content = "Сотрудники";
            edit_employees.Icon = new SymbolIcon(Symbol.People);
            edit_employees.Tapped += (s, e) =>
            {
                userMenu.Navigate(typeof(Employees));
            };
            //client
            NavigationViewItem edit_client = new NavigationViewItem();
            edit_client.Content = "Клиенты";
            edit_client.Icon = new SymbolIcon(Symbol.OtherUser);
            edit_client.Tapped += (s, e) =>
            {
                userMenu.Navigate(typeof(Client));
            };
            //sale
            NavigationViewItem sales = new NavigationViewItem();
            sales.Content = "Продажи";
            sales.Icon = new SymbolIcon(Symbol.Shop);
            sales.Tapped += (s, e) =>
            {
                userMenu.Navigate(typeof(Sale));
            };
            //supplies
            NavigationViewItem supplies = new NavigationViewItem();
            supplies.Content = "Поставки";
            supplies.Icon = new SymbolIcon(Symbol.Accept);
            supplies.Tapped += (s, e) =>
            {
                userMenu.Navigate(typeof(Supply));
            };
            // Объявление конструкции выбора
            switch (Convert.ToInt32(config.localSettings.Values["id_group"]))
            {
                 // Идентификатор группы
                 case 1:
                    workSpace.MenuItems.Clear();
                    // Объявление нового элемента навигационного меню в качестве переменной
                    NavigationViewItem edit_users = new NavigationViewItem();
                     // Задание названия
                     edit_users.Content = "Пользователи";
                     // Задание икноки
                     edit_users.Icon = new SymbolIcon(Symbol.People);
                    // Создание обработчика события при нажатии — открытие нового Фрейма внутри
                     edit_users.Tapped += (s, e) =>
                     {
                         userMenu.Navigate(typeof(Users));
                     };
                    // Добавление элемента в общий список навигационного меню
                    workSpace.MenuItems.Add(edit_users);

                    NavigationViewItem edit_pass_request = new NavigationViewItem();
                    edit_pass_request.Content = "Заявки на восстановление пароля";
                    edit_pass_request.Icon = new SymbolIcon(Symbol.Accept);
                    edit_pass_request.Tapped += (s, e) =>
                    {
                        userMenu.Navigate(typeof(PasswordRequests));
                    };
                    workSpace.MenuItems.Add(edit_pass_request);

                    NavigationViewItem edit_user_groups = new NavigationViewItem();
                    edit_user_groups.Content = "Группы пользователей";
                    edit_user_groups.Icon = new SymbolIcon(Symbol.Import);
                    edit_user_groups.Tapped += (s, e) =>
                    {
                        userMenu.Navigate(typeof(Groups));
                    };
                    workSpace.MenuItems.Add(edit_user_groups);
                    workSpace.MenuItems.Add(edit_lk);
                    break;
                case 2:
                    workSpace.MenuItems.Clear();
                    workSpace.MenuItems.Add(edit_lk);
                    workSpace.MenuItems.Add(sales);
                    workSpace.MenuItems.Add(supplies);
                    break;
                case 3:
                    workSpace.MenuItems.Clear();
                    workSpace.MenuItems.Add(edit_lk);
                    workSpace.MenuItems.Add(sales);
                    workSpace.MenuItems.Add(supplies);
                    workSpace.MenuItems.Add(edit_employees);
                    break;
                case 4:
                    workSpace.MenuItems.Clear();
                    workSpace.MenuItems.Add(edit_lk);
                    workSpace.MenuItems.Add(edit_employees);
                    break;
                case 5:
                    workSpace.MenuItems.Clear();
                    workSpace.MenuItems.Add(edit_lk);
                    workSpace.MenuItems.Add(edit_client);
                    workSpace.MenuItems.Add(sales);
                    break;
                case 6:
                    workSpace.MenuItems.Clear();
                    workSpace.MenuItems.Add(edit_lk);
                    workSpace.MenuItems.Add(supplies);
                    break;


            }

        }

         
    }
}
