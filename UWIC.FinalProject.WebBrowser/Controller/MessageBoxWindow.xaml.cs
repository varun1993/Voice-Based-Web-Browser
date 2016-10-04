using System;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Media.Imaging;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for MessageBoxWindow.xaml
    /// </summary>
    public partial class MessageBoxWindow
    {

        public MessageBoxWindow()
        {
            InitializeComponent();
        }

        public MessageBoxWindow(string message, string messageTitle, Visibility yesButtonVisible, Visibility noButtonVisible, MessageBoxIcon icon)
        {
            InitializeComponent();
            SetImage(icon);
            LblTitle.Content = messageTitle;
            TxtMessage.Text = message;
            BtnYes.Visibility = yesButtonVisible;
            BtnNo.Visibility = noButtonVisible;
        }

        public MessageBoxWindow(string message, string messageTitle, Visibility okButtonVisible, MessageBoxIcon icon)
        {
            InitializeComponent();
            SetImage(icon);
            LblTitle.Content = messageTitle;
            TxtMessage.Text = message;
            BtnOk.Visibility = okButtonVisible;
        }

        private void SetImage(MessageBoxIcon icon)
        {
            var logo = new BitmapImage();
            logo.BeginInit();
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    logo.UriSource = new Uri("pack://application:,,,/UWIC.FinalProject.WebBrowser;component/Images/error-white.png");
                    break;
                case MessageBoxIcon.Information:
                    logo.UriSource = new Uri("pack://application:,,,/UWIC.FinalProject.WebBrowser;component/Images/info-white.png");
                    break;
            }
            logo.EndInit();
            ImgIcon.Source = logo;
        }

        private void MessageBoxWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var voice = new SpeechSynthesizer();
            try
            {
                voice.Volume = 100;
                voice.Rate = 0;
                voice.SpeakAsync(TxtMessage.Text);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
            }
        }
    }
}
