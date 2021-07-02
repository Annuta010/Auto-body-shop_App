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
    public sealed partial class lk : Page
    {
        User user = new User();
        Database db = new Database();
        Config config = new Config();
        public lk()
        {
            this.InitializeComponent();
            fillPage();
        }

        private void fillPage()
        {
            lastNameBlock.Text = (string)user.localSettings.Values["last_name"];
            firstNameBlock.Text = (string)user.localSettings.Values["first_name"];
            midNameBlock.Text = (string)user.localSettings.Values["mid_name"];
            groupBlock.Text = (Convert.ToInt32(user.localSettings.Values["id_group"])).ToString();
            emailBlock.Text = (string)user.localSettings.Values["user_email"];

        }

        private void PassBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((newPassBox.Password != "") && (currentPassBox.Password != ""))
            {
                confirmPassBtn.IsEnabled = true;
            }
        }

        private async void confirmPassBtn_Click(object sender, RoutedEventArgs e)
        {
            var data = db.Select("SELECT `user_password` from `users` WHERE (`user_password` = '" + config.Hash(currentPassBox.Password) + "')");
            if (data.Rows.Count != 0)
            {
                db.Insert("UPDATE `users` SET `user_password`='" + config.Hash(newPassBox.Password) + "' " +
                    " WHERE (`users`.`id_user` = '" + (Convert.ToInt32(user.localSettings.Values["id_user"])).ToString() + "')");
                
                await user.Alert("Пароль был успешно изменен", "Успешно");
            }
            else
            {
                await user.Alert("Произошла ошибка: неверные данные. Пароль не был изменен", "Ошибка");
            }
        }
    }
}
