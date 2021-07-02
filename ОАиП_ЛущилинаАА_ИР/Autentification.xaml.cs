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
    public sealed partial class Autentification : Page
    {
        Database data_base = new Database();
        Config config = new Config();
        User user = new User();
        int user_id_user;
        int user_id_group;
        string user_last_name;
        public Autentification()
        {
            this.InitializeComponent();
        }
        private async void Activated()
        {
            User user = new User();
            Config config = new Config();
            try
            {
                /* Варианты действий на основе вызова метода из класса User
                для прохождения аутентификации.Передаваемые значение — содержимое полей из соответствующего экрана */
                if (user.Autentification(LoginBox.Text, PasswordBox.Password.ToString()))
                {
                    // Использование текущего экрана в качестве значения
                    Frame contentFrame = Window.Current.Content as Frame;
                    // Обращение к родительскому экрану
                    MainPage mp = contentFrame.Content as MainPage;
                    // Изменение состояния родительского экрана
                    mp.navFrame.Navigate(typeof(PanelFrame));
                }
                else
                {
                    await config.Alert("Проверьте правильность введённых данных и повторите попытку.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                /* Вывод усовершенствованного сообщения об ошибке с
                источником и системным уведомлением для упрощённой отладки */
                await config.Alert("Произошла ошибка с текстом исключения «"
                + ex.Message + "». Повторите попытку позднее или обратитесь к системному администратору.", "Ошибка " + ex.Source);
            }
        }
  
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Activated();
        }

        private void LoginBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (LoginBox.Text != "" && PasswordBox.Password != "")
            {
                LoginButton.IsEnabled = true;
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    Activated();
                }
                
            }
        }
        private void LoginBox_SelectionChanged(object sender, TextChangedEventArgs e)
        {
            var data = data_base.Select("SELECT `id_user`,`id_group`, `last_name` FROM `users` JOIN`employees`" +
                " WHERE(`users`.`user_login` = '" + LoginBox.Text + "'OR `users`.`user_email` = '" + LoginBox.Text + "') ");

            if (data.Rows.Count != 0)
            {
                user_id_user = Convert.ToInt32(data.Rows[0]["id_user"]);
                user_id_group = Convert.ToInt32(data.Rows[0]["id_group"]);
                user_last_name = data.Rows[0]["last_name"].ToString();
                ForgotPassBtn.IsEnabled = true;
            }
        }
        private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (LoginBox.Text != "" && PasswordBox.Password != "")
            {
                LoginButton.IsEnabled = true;
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    Activated();
                }
            }
        }

        private void ForgotPassBtn_Click(object sender, RoutedEventArgs e)
        {
            autorizationLayout.Visibility = Visibility.Collapsed;
            passwordResetLayout.Visibility = Visibility.Visible;

        }

        private async void SendRequestBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                data_base.Insert("DELETE FROM `reset_requests` WHERE TIMESTAMPDIFF(HOUR, `time`, CURDATE() ) > 24");
                var dt = data_base.Select("SELECT `id_user`, `time` FROM `reset_requests` " +
                    "WHERE (`id_user` = '" + user_id_user + "')");

                if (dt.Rows.Count == 0)
                {
                    data_base.Insert("INSERT INTO reset_requests(`id_user`) " +
                "values('" + user_id_user + "' )");
                    autorizationLayout.Visibility = Visibility.Visible;
                    passwordResetLayout.Visibility = Visibility.Collapsed;
                    await user.Alert("Заявка была успешно отправлена", "Восстановление пароля");
                } else
                {
                    await user.Alert("Можно отправить не более одной заявки в день", "Ошибка");
                }
                

            }
            catch (Exception ex)
            {
                await user.Alert("Произошла ошибка: " + ex.Message, "Ошибка");
            }
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            
            if ((user_last_name == question1.Text) &&
                (user_id_group.ToString() == question2.Text) )
            {
                data_base.Insert("UPDATE `users` SET `user_password`='" + config.Hash(NewPassword.Password) + "' " +
                    " WHERE (`users`.`id_user` = '" + user_id_user + "')");
                autorizationLayout.Visibility = Visibility.Visible;
                passwordResetLayout.Visibility = Visibility.Collapsed;
                await user.Alert("Пароль был успешно изменен", "Успешно");
            } else
            {
                await user.Alert("Произошла ошибка: неверные данные. Восстановление пароля отклонено" , "Ошибка");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            autorizationLayout.Visibility = Visibility.Visible;
            passwordResetLayout.Visibility = Visibility.Collapsed;
        }

        
    }

}
    
