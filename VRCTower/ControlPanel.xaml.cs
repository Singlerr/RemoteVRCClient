using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ModernWpf.Controls;
using Newtonsoft.Json;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using WebSocketSharp;

namespace VRCTower
{
    public partial class ControlPanel : Window
    {
        private readonly string _apiKey;
        private readonly string _authCookie;
        private readonly string _id;
        private readonly string _password;
        private readonly string _userId;
        private readonly string _username;
        private readonly string _webSocketUrl = "ws://remotevrc.kro.kr:8080/cloud";
        private readonly Configuration _configuration;
        private WebSocket _webSocket;

        public ControlPanel(string userId, string authCookie, string apiKey, string username, string id,
            string password)
        {
            _userId = userId;
            _apiKey = apiKey;
            _id = id;
            _password = password;
            _authCookie = authCookie;
            _username = username;
            _configuration = new Configuration();
            _configuration.AddApiKey("apiKey", _apiKey);
            InitializeComponent();
            _usernameText.Text = _username;
            _uuid.Text = _userId;
            StartWebSocket();
            StartUpdatingTask();
        }

        private async Task StartUpdatingTask()
        {
            var config = new Configuration();
            config.AddApiKey("apiKey", _apiKey);
            var usersApi = new UsersApi(config);
            while (true)
            {
                var instanceId = (await usersApi.GetUserAsync(_userId)).Location;
                Dispatcher.Invoke(delegate { _instanceId.Text = instanceId; });
                await Task.Delay(3000);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Dispatcher.Invoke(delegate { MsgUtils.ShowConnectionErrorMessageBox("알림", "서버에 연결하지 못했습니다.", this); });
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            if (e.IsText)
                if (TryParse(e.Data, out ReadOnly readOnly))
                {
                    if (readOnly.Message == null)
                        return;
                    switch (readOnly.StatusCode)
                    {
                        case 200:
                        {
                            if (readOnly.Message.Equals("normal", StringComparison.InvariantCultureIgnoreCase))
                                Dispatcher.Invoke(delegate
                                {
                                    _btn_inivte_all.IsEnabled = false;
                                    _btn_delete_queue.IsEnabled = false;
                                    _btn_createQueue.IsEnabled = false;
                                    _btn_createQueue.Content = _btn_createQueue.Content.ToString()
                                        .Replace("${permissionStatus}", "- 큐 생성 권한 없음");
                                });
                            else if (readOnly.Message.Equals("admin", StringComparison.InvariantCultureIgnoreCase) ||
                                     readOnly.Message.Equals("creator", StringComparison.InvariantCultureIgnoreCase))
                                Dispatcher.Invoke(delegate
                                {
                                    _btn_createQueue.IsEnabled = true;
                                    _btn_createQueue.Content = _btn_createQueue.Content.ToString()
                                        .Replace("${permissionStatus}", "");
                                });
                            else
                                MsgUtils.ShowMessageBox("알림", "해당 작업이 완료되었습니다.");
                            break;
                        }
                        case 202:
                        {
                            var queues = readOnly.Message.Split(',');
                            Dispatcher.Invoke(delegate
                            {
                                var previousSelectedItem = _queueList.SelectedItem;
                                _queueList.Items.Clear();
                                if (previousSelectedItem == null)
                                    foreach (var queue in queues)
                                        _queueList.Items.Add(queue);
                                else
                                    foreach (var queue in queues)
                                    {
                                        _queueList.Items.Add(queue);
                                        if (queue.Equals(previousSelectedItem.ToString(),
                                                StringComparison.InvariantCulture))
                                            _queueList.SelectedIndex = _queueList.Items.Count - 1;
                                    }
                            });
                            break;
                        }
                        //Accept friend request
                        case -2:
                            var acceptWaiter = AcceptFriendRequest(readOnly.Message).GetAwaiter();
                            acceptWaiter.OnCompleted(delegate
                            {
                                _webSocket.SendAsync(JsonConvert.SerializeObject(acceptWaiter.GetResult()), null);
                            });
                            break;
                        //Accept invite request
                        case -3:
                            var waiter = AcceptInviteRequest(readOnly.Message).GetAwaiter();
                            waiter.OnCompleted(delegate
                            {
                                _webSocket.SendAsync(JsonConvert.SerializeObject(waiter.GetResult()), null);
                            });
                            break;
                        //Invite User
                        case -4:
                            var userId = readOnly.Message.Split('|')[0];
                            var worldId = readOnly.Message.Split('|')[1];
                            var inviteWaiter = InviteUser(userId, worldId).GetAwaiter();
                            inviteWaiter.OnCompleted(delegate
                            {
                                _webSocket.SendAsync(JsonConvert.SerializeObject(inviteWaiter.GetResult()), null);
                            });
                            break;
                        default:
                        {
                            Dispatcher.Invoke(delegate
                            {
                                MsgUtils.ShowMessageBox("오류",
                                    "해당 작업을 처리할 수 없습니다." + Environment.NewLine + readOnly.Message);
                            });
                            break;
                        }
                    }
                }
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            Dispatcher.Invoke(delegate { MsgUtils.ShowConnectionErrorMessageBox("알림", "서버와 연결이 끊겼습니다.", this); });
        }

        private void OnOpen(object sender, EventArgs e)
        {
            var handshake = new DataPacket(_id, _password, _userId);
            var str = JsonConvert.SerializeObject(handshake);
            _webSocket.SendAsync(str, null);
        }

        public void StartWebSocket()
        {
            _webSocket = new WebSocket(_webSocketUrl);
            _webSocket.OnOpen += OnOpen;
            _webSocket.OnClose += OnClose;
            _webSocket.OnError += OnError;
            _webSocket.OnMessage += OnMessageReceived;
            _webSocket.ConnectAsync();
        }

        private void _queueList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_queueList.SelectedItem != null) _queueName.Text = _queueList.SelectedItem.ToString();
        }

        private static bool TryParse<T>(string strToParse, out T result)
        {
            result = default;
            var success = true;
            try
            {
                result = JsonConvert.DeserializeObject<T>(strToParse);
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_webSocket != null && _webSocket.IsAlive)
            {
                _webSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            Application.Current.Shutdown();
        }


        private void _btn_createQueue_Click(object sender, RoutedEventArgs e)
        {
            if (_webSocket.IsAlive)
            {
                if (!string.IsNullOrWhiteSpace(_newQueueName.Text))
                {
                    var createQueue = new ActionPacket(_apiKey, _authCookie, _userId, "CREATE_QUEUE",
                        new[] { _newQueueName.Text });
                    _webSocket.SendAsync(JsonConvert.SerializeObject(createQueue), null);
                }
                else
                {
                    MsgUtils.ShowMessageBox("알림", "큐 이름을 적어주세요.");
                }
            }
            else
            {
                MsgUtils.ShowMessageBox("알림", "현재 서버와 연결이 끊긴 상태입니다.");
            }
        }

        private async Task Handle(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                MsgUtils.ShowMessageBox("알림", "초대 작업을 할 큐를 선택하십시오.");
                return;
            }

            var dialog = new InviteDialog();

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.None)
                return;
            if (!dialog.UseCurrentLocation && string.IsNullOrWhiteSpace(dialog._worldId.Text))
            {
                await MsgUtils.ShowMessageBoxAsync("오류", "현재 내 위치로 초대하지 않을 경우, 월드 ID를 입력해야합니다.");
                await dialog.ShowAsync();
            }

            if (!dialog.UseCurrentLocation)
            {
                var invite = new ActionPacket(_apiKey, _authCookie, _userId, "INVITE_SUCCESSIVELY",
                    new[] { queueName, dialog._worldId.Text });
                _webSocket.SendAsync(JsonConvert.SerializeObject(invite), null);
            }
            else
            {
                var invite = new ActionPacket(_apiKey, _authCookie, _userId, "INVITE_SUCCESSIVELY",
                    new[] { queueName, "use_current" });
                _webSocket.SendAsync(JsonConvert.SerializeObject(invite), null);
            }
        }

        private void _btn_invite_all_Click(object sender, RoutedEventArgs e)
        {
            Handle(_queueName.Text);
        }

        private void _btn_leave_queue_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_queueName.Text))
            {
                MsgUtils.ShowMessageBox("알림", "탈퇴할 큐를 선택하십시오.");
                return;
            }

            var leaveQueue = new ActionPacket(_apiKey, _authCookie, _userId, "LEAVE_QUEUE",
                new[] { _queueName.Text.Trim() });
            _webSocket.SendAsync(JsonConvert.SerializeObject(leaveQueue), null);
        }

        private void _btn_jojn_queue_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_queueName.Text))
            {
                MsgUtils.ShowMessageBox("알림", "참가할 큐를 선택하십시오.");
                return;
            }

            var joinQueue = new ActionPacket(_apiKey, _authCookie, _userId, "JOIN_QUEUE",
                new[] { _queueName.Text.Trim() });
            _webSocket.SendAsync(JsonConvert.SerializeObject(joinQueue), null);
        }

        private void _btn_delete_queue_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_queueName.Text))
            {
                MsgUtils.ShowMessageBox("알림", "삭제할 큐를 선택하십시오.");
                return;
            }

            var deleteQueue = new ActionPacket(_apiKey, _authCookie, _userId, "DELETE_QUEUE",
                new[] { _queueName.Text.Trim() });
            _webSocket.SendAsync(JsonConvert.SerializeObject(deleteQueue), null);
        }

        private async Task<ReadOnly> AcceptFriendRequest(string notificationId)
        {
            var notificationApi = new NotificationsApi(_configuration);

            var acceptResp = await notificationApi.AcceptFriendRequestWithHttpInfoAsync(notificationId);

            if (acceptResp.StatusCode != HttpStatusCode.OK)
                return new ReadOnly("해당 친구 요청을 찾을 수 없거나, 친구 요청을 수락하는 과정에서 오류가 발생했습니다.", -1);

            return new ReadOnly(notificationId, 200);
        }

        private async Task<ReadOnly> AcceptInviteRequest(string notificationId)
        {
            var inviteApi = new InviteApi(_configuration);

            var acceptResp = await inviteApi.RespondInviteWithHttpInfoAsync(notificationId);

            if (acceptResp.StatusCode != HttpStatusCode.OK)
                return new ReadOnly("해당 초대 요청을 찾을 수 없거나, 초대 요청을 수락하는 과정에서 오류가 발생했습니다.", -1);

            return new ReadOnly(notificationId, 200);
        }

        private async Task<ReadOnly> InviteUser(string userId, string worldId)
        {
            var inviteApi = new InviteApi(_configuration);

            var usersApi = new UsersApi(_configuration);

            var friendsApi = new FriendsApi(_configuration);


            var targetUserResp = await usersApi.GetUserWithHttpInfoAsync(userId);

            if (targetUserResp.StatusCode != HttpStatusCode.OK) return new ReadOnly("초대하려는 유저를 찾을 수 없습니다.", -1);

            var inviteUserResp = await inviteApi.InviteUserWithHttpInfoAsync(userId, new InviteRequest(worldId));
            if (inviteUserResp.StatusCode == HttpStatusCode.Forbidden)
            {
                //Not friend
                var friendReqResp = await friendsApi.FriendWithHttpInfoAsync(userId);
                if (friendReqResp.StatusCode == HttpStatusCode.OK)
                    return new ReadOnly("Accept friend request", -2);
                return new ReadOnly("Internal error", -1);
            }

            if (inviteUserResp.StatusCode == HttpStatusCode.OK)
                return new ReadOnly("Accept invite request", -3);
            return new ReadOnly("Internal error", -1);
        }

    }
}