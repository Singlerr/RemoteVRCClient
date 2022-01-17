using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernWpf.Controls;

namespace VRCTower
{
    public class MsgUtils
    {
        public static void ShowMessageBox(string title, string message)
        {
            MainWindow.Instance.Dispatcher.Invoke(delegate { _ShowMessageBox(title, message); });
        }

        public static async void _ShowMessageBox(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "확인",
                DefaultButton = ContentDialogButton.Primary
            };
            try
            {
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public static async Task ShowMessageBoxAsync(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "확인",
                DefaultButton = ContentDialogButton.Primary
            };
            try
            {
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public static async void ShowConnectionErrorMessageBox(string title, string message, ControlPanel instance)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "확인",
                SecondaryButtonText = "재시도",
                SecondaryButtonCommand = new RetryCommand(instance),
                DefaultButton = ContentDialogButton.Primary
            };
            try
            {
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }

    public class RetryCommand : ICommand
    {
        private readonly ControlPanel _inst;

        public RetryCommand(ControlPanel inst)
        {
            _inst = inst;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object obj)
        {
            _inst.StartWebSocket();
        }

        public bool CanExecute(object obj)
        {
            return true;
        }
    }
}