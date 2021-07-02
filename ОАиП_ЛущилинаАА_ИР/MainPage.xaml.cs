using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
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

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace ОАиП_ЛущилинаАА_ИР
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        User user = new User();

        public MainPage()
        {
            this.InitializeComponent();
            MainAsync();
            //AsyncContext.Run(() => MainAsync());
        }


        /* Приватный асинхронный метод MainAsync для определения
        состояния входа пользователя.Если он был авторизован ранее
        и имеет активный ключ безопасности, то будет незамедлительно
        отправлен к рабочему пространству */

        //private async System.Threading.Tasks.Task MainAsync()
        private async void MainAsync()
        {
            // Проверка состояния аутентификации по коду
            switch (user.CheckAuth())
            {
                // Код 0 возвращает необходимость повторно пройти процедуру аутентификации
                case 0:
                    navFrame.Navigate(typeof(Autentification));
                break;
                /* Код 1 даёт возможность сразу приступать к работе,
                так как данные пользователя являются актуальными и
                продолжать работу с приложением безопасносо */
                case 1:
                    navFrame.Navigate(typeof(PanelFrame));
                break;
                /* Любой другой код говорит о необходимости проверить
                подключение к Интернету или же серверу базы данных при
                помощи цикла внутри данного окна. Если нет
                подключения, то метод CheckAuth() будет вечно
                возвращать код 2, что не входит в границы используемой
                конструкции */
                default:
                    // Объявление нового сообщения об ошибке
                    var messageDialog = new MessageDialog("Произошла ошибка при подключении к Интернету.Пожалуйста, повторите попытку.",
                        "Ошибка");
                    /* Добавление в сообщение об ошибке кнопок с
                    вместе с передачей процедур по нажатию на них.От
                    повторной загрузки экрана до выхода из
                    приложения */
                    messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Повторить",
                    (command) =>
                    {
                        navFrame.Navigate(typeof(MainPage));
                    }));
                    messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Выход", (command) =>
                    {
                        Application.Current.Exit();
                    }));
                    await messageDialog.ShowAsync();
                break;

            }
        }

    }
}