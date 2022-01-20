using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using VRChat.API.Api;
using VRChat.API.Client;

namespace VRCTower
{
    /// <summary>
    ///     MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        private void Click_VRC_Login(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_idText.Text) || string.IsNullOrWhiteSpace(_passwordText.Password))
            {
                MsgUtils.ShowMessageBox("알림", "아이디와 비밀번호 모두 입력해주세요.");
                return;
            }

            var conf = new Configuration
            {
                Username = _idText.Text.Trim(),
                Password = _passwordText.Password.Trim()
            };
            var authApi = new AuthenticationApi(conf);
            var task = authApi.GetCurrentUserWithHttpInfoAsync().GetAwaiter();
            task.OnCompleted(() =>
            {
                try
                {
                    var result = task.GetResult();
                    var user = result.Data;
                    var controlPanel = new ControlPanel(user.Id, GetCookie(result.Cookies, "auth").Value,
                        GetCookie(result.Cookies, "apiKey").Value, user.Username, conf.Username, conf.Password);
                    controlPanel.Show();
                    Hide();
                }
                catch (ApiException ex)
                {
                    MsgUtils.ShowMessageBox("알림", "아이디 혹은 비밀번호가 잘못되었습니다.");
                }

                _progress.Visibility = Visibility.Hidden;
            });

            _progress.Visibility = Visibility.Visible;
        }

        private Cookie GetCookie(List<Cookie> cookies, string cookieName)
        {
            return cookies.Find(c => c.Name.Equals(cookieName, StringComparison.InvariantCulture));
        }

        private void Click_Steam_Login(object sender, RoutedEventArgs e)
        {
            _progress.Visibility = Visibility.Visible;
        }
    }
}