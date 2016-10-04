using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.TeamFoundation.MVVM;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechRecognitionEngine;
using UWIC.FinalProject.WebBrowser.Model;
using UWIC.FinalProject.WebBrowser.ViewModel;
using UWIC.FinalProject.WebBrowser.svcSendKeys;
using System.Speech.Synthesis;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for BrowserContainer.xaml
    /// </summary>
    public partial class BrowserContainer : UserControl
    {
        # region Events

        public event EventHandler PageLoadCompleted;

        # endregion

        # region Public Variables

        public string CommandText { get; set; }

        public string UrlText { get; set; }

        public Storyboard DownAnimation { get; set; }

        public BitmapImage Favicon { get; set; }

        public string WebPageTitle { get; set; }

        public Uri Url { get; set; }

        public bool WebBrowserVisible { get; set; }

        private static TabItemViewModel TabItemViewModel { get; set; }

        private Mode _commandMode;

        public Mode CommandMode
        {
            get { return _commandMode; }
            set
            {
                _commandMode = value;
                switch (_commandMode)
                {
                    case Mode.CommandMode:
                        {
                            var bc = new BrushConverter();
                            BottomBar.Background = (Brush)bc.ConvertFrom("#FF171717");
                            LabelCurrentMode.Content = "Command Mode";
                            break;
                        }
                    case Mode.DictationMode:
                        {
                            BottomBar.Background = Brushes.Crimson;
                            LabelCurrentMode.Content = "Dictation Mode";
                            break;
                        }
                    case Mode.GeneralSpellMode:
                        {
                            BottomBar.Background = Brushes.OrangeRed;
                            LabelCurrentMode.Content = "General Spelling Mode";
                            break;
                        }
                    case Mode.PasswordSpellMode:
                        {
                            BottomBar.Background = Brushes.SlateBlue;
                            LabelCurrentMode.Content = "Username/Password Mode";
                            break;
                        }
                    case Mode.WebsiteSpellMode:
                        {
                            BottomBar.Background = Brushes.ForestGreen;
                            LabelCurrentMode.Content = "Website Spelling Mode";
                            break;
                        }
                }
            }
        }

        public System.Speech.Recognition.SpeechRecognitionEngine SpeechRecognizerEngine { get; set; }

        # endregion

        # region Main Page Events & Methods

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BrowserContainer()
        {
            InitializeComponent();
            webBrowserMain.PreviewMouseMove += webBrowserMain_PreviewMouseMove; // Initialize Preview Mouse Move Event
        }

        /// <summary>
        /// This event will fire once the user control has been executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CloseEmulator();
            AcquireStoryboardAnimation();
            CommandMode = Mode.CommandMode;
            //timer.Elapsed += timer_Elapsed;
            //timer.Start();
            CloseMessageBox();

            #region Background Image
            try
            {
                if (!UriParser.IsKnownScheme("pack"))
                    UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);

                var dict = new ResourceDictionary();
                var uri = new Uri("/UWIC.FinalProject.WebBrowser;component/Resources/BackgroundImageResourceDictionary.xaml", UriKind.Relative);
                dict.Source = uri;
                Application.Current.Resources.MergedDictionaries.Add(dict);
                var backImageSource = (ImageSource)Application.Current.Resources["BackImage"];
                BImage.ImageSource = backImageSource;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
            # endregion
        }

        /// <summary>
        /// This method will set the DownAnimation Storyboard on the ViewModel
        /// </summary>
        public void AcquireStoryboardAnimation()
        {
            try
            {
                var storyBoard = (Storyboard)FindResource("DownAnimation");
                DownAnimation = storyBoard;
                DownAnimation.Completed += DownAnimation_Completed;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// This event will fire once the Down animation has been completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DownAnimation_Completed(object sender, EventArgs e)
        {
            WebBrowserVisible = true;
        }

        /// <summary>
        /// This event will fire once the browser container gets the focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowserContainer_OnGotFocus(object sender, RoutedEventArgs e)
        {
            //InitializeSpeechRecognizer();
        }

        /// <summary>
        /// This event will fire once the browser container lose the focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowserContainer_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //StoSpeechRecognizer();
        }

        #region Timer
        //Timer timer = new Timer(10000);
        

        //void timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    //var svcClient = new SendKeysServiceClient();
        //    //svcClient.PostMessage("post message");
        //}
        #endregion

        #endregion

        # region Mouse Move Events & Methods
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        public Point GetMousePosition()
        {
            try
            {
                var w32Mouse = new Win32Point();
                GetCursorPos(ref w32Mouse);
                return new Point(w32Mouse.X, w32Mouse.Y);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// This event will fire when the mouse moves over the web browser container
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void webBrowserMain_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var point = GetMousePosition();
                xyPosition.Content = "X Position = " + point.X.ToString(CultureInfo.InvariantCulture) + "; Y Position = " + point.Y.ToString(CultureInfo.InvariantCulture) + ";";
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private void SetPosition(int a, int b)
        {
            try
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                SetCursorPos(a, b);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02; /* left button down */
        public const int MOUSEEVENTF_LEFTUP = 0x04; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */

        //This simulates a left mouse click
        private void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        private void LeftMouseClick()
        {
            try
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                //Call the imported function with the cursor's current position
                var currentPosition = GetMousePosition();
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Convert.ToInt32(currentPosition.X),
                            Convert.ToInt32(currentPosition.Y), 0, 0);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
            
        }

        private void RightMouseClick()
        {
            //Thread.Sleep(TimeSpan.FromSeconds(3));
            //var currentPosition = GetMousePosition();
            //mouse_event(MOUSEEVENTF_RIGHTDOWN, Convert.ToInt32(currentPosition.X), Convert.ToInt32(currentPosition.Y), 0, 0);
        }

        private void DoubleClick()
        {
            try
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                var currentPosition = GetMousePosition();
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Convert.ToInt32(currentPosition.X), Convert.ToInt32(currentPosition.Y), 0, 0);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Convert.ToInt32(currentPosition.X), Convert.ToInt32(currentPosition.Y), 0, 0);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        # endregion

        # region Web Browser Basic Navigation

        private void RefreshBrowser()
        {
            try
            {
                webBrowserMain.Reload(false);
                SwitchToBusyState();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void MoveBackward()
        {
            try
            {
                if (!webBrowserMain.CanGoBack()) return;
                webBrowserMain.GoBack();
                SwitchToBusyState();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void MoveForward()
        {
            try
            {
                if (!webBrowserMain.CanGoForward()) return;
                webBrowserMain.GoForward();
                SwitchToBusyState();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void StopBrowser()
        {
            try
            {
                webBrowserMain.Stop();
                SwitchToNormalState();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void NavigateToUrl(string url = null)
        {
            try
            {
                Uri tempUri;
                if (url != null)
                    TryParseUrl(url, out tempUri);
                else
                    TryParseUrl(UrlText, out tempUri);
                if (!WebBrowserVisible)
                    DownAnimation.Begin();

                webBrowserMain.Source = tempUri;
                SwitchToBusyState();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private static bool TryParseUrl(string uriString, out Uri uri)
        {
            return Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri);
        }

        private void ExecuteFunction(string command)
        {
            try
            {
                FunctionalCommandType commandType;
                if (!Enum.TryParse(command, out commandType)) return;
                switch (commandType)
                {
                    case FunctionalCommandType.Backward:
                        {
                            MoveBackward();
                            break;
                        }
                    case FunctionalCommandType.Forward:
                        {
                            MoveForward();
                            break;
                        }
                    case FunctionalCommandType.Refresh:
                        {
                            RefreshBrowser();
                            break;
                        }
                    case FunctionalCommandType.Stop:
                        {
                            StopBrowser();
                            break;
                        }
                    case FunctionalCommandType.Go:
                        {
                            NavigateToUrl();
                            break;
                        }
                    case FunctionalCommandType.StartVoice:
                        {
                            StartStopVoiceRecognition(FunctionalCommandType.StartVoice);
                            break;
                        }
                    case FunctionalCommandType.StopVoice:
                        {
                            StartStopVoiceRecognition(FunctionalCommandType.StopVoice);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void StartStopVoiceRecognition(FunctionalCommandType commandType)
        {
            if (commandType == FunctionalCommandType.StartVoice)
            {
                InitializeSpeechRecognizer();
                StartVoiceButton.Visibility = Visibility.Collapsed;
                StopVoiceButton.Visibility = Visibility.Visible;
            }
            else
            {
                StoSpeechRecognizer();
                StartVoiceButton.Visibility = Visibility.Visible;
                StopVoiceButton.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        # region Web Browser Events & Methods

        /// <summary>
        /// This event will fire once a parituclar page has been loaded successfully
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowserMain_LoadingFrameComplete(object sender, Awesomium.Core.FrameEventArgs e)
        {
            try
            {
                if (!e.IsMainFrame) return;
                if (!webBrowserMain.IsDocumentReady) return;
                pbWebPageLoad.Visibility = Visibility.Collapsed;
                pbWebPageLoad.State = Elysium.Controls.ProgressState.Normal;

                Url = webBrowserMain.Source;
                SetWebPageTitleNFavicon();
                SetHeaderAndIcon();
                //if (this.PageLoadCompleted != null)
                //    this.PageLoadCompleted(this, e); 
                //SetCursorPos(36, 120);
                //LeftMouseClick(36, 120);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }
        
        /// <summary>
        /// Set the Web page title and Favicon to the Tab Item header
        /// </summary>
        public void SetWebPageTitleNFavicon()
        {
            try
            {
                Favicon = new BrowserContainerModel().GetFavicon(new BrowserContainerModel().GetImageSource(Url));
                WebPageTitle = new BrowserContainerModel().GetWebPageTitle(Url.AbsoluteUri);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// This method is used to set the Webpage Icon and the Title to the parent tab item
        /// </summary>
        private void SetHeaderAndIcon()
        {
            try
            {
                var parent = (UIElement)Parent;
                var _parent = (TabItem)parent;
                if (!String.IsNullOrEmpty(WebPageTitle))
                {
                    _parent.Header = new TabItemHeader(Favicon, WebPageTitle);
                }
                else
                {
                    _parent.Header = new TabItemHeader(Favicon, webBrowserMain.Source.Host);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// This event will fire once the Address of the web browser has been changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowserMain_AddressChanged(object sender, Awesomium.Core.UrlEventArgs e)
        {
            try
            {
                //ViewModel.URL = webBrowserMain.Source;
                txtURL.Text = webBrowserMain.Source.AbsoluteUri;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// this method will switch the progress bar to an indeterminate mode
        /// </summary>
        private void SwitchToBusyState()
        {
            try
            {
                pbWebPageLoad.Visibility = Visibility.Visible;
                pbWebPageLoad.State = Elysium.Controls.ProgressState.Indeterminate;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// This method will switch the progress bar to normal state and hide it
        /// </summary>
        private void SwitchToNormalState()
        {
            try
            {
                pbWebPageLoad.Visibility = Visibility.Collapsed;
                pbWebPageLoad.State = Elysium.Controls.ProgressState.Normal;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        #endregion

        #region Commands

        public ICommand FuncCommand;
        public ICommand FunctionCommand
        {
            get
            {
                return FuncCommand ??
                       (FuncCommand = new RelayCommand(param => ExecuteFunction(param.ToString())));
            }
        }

        public ICommand EmulCommand;
        public ICommand EmulatorCmd
        {
            get
            {
                return EmulCommand ??
                       (EmulCommand = new RelayCommand(ExecuteEmulator));
            }
        }

        public ICommand BmCommand;
        public ICommand BookmarkCommand
        {
            get { return BmCommand ?? (BmCommand = new RelayCommand(param => NavigateToUrl(param.ToString()))); }
        }

        #endregion

        #region Emulator

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    OpenEmulator();
                }
                else if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    CloseEmulator();
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void OpenEmulator()
        {
            try
            {
                var sb = (Storyboard)FindResource("EmulatorOpen");
                sb.Begin();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void CloseEmulator()
        {
            try
            {
                var sb = (Storyboard)FindResource("EmulatorClose");
                sb.Begin();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void ExecuteEmulator()
        {
            try
            {
                var speechEngine = new SpeechEngine();
                speechEngine.InitializeEmulator(CommandMode);
                if (String.IsNullOrEmpty(CommandText)) return;
                speechEngine.StartEmulatorRecognition(CommandText);
                speechEngine.SpeechProcessed += SpeechEngine_SpeechProcessed;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        void SpeechEngine_SpeechProcessed(object sender, EventArgs e)
        {
            try
            {
                var speechEngine = (SpeechEngine)sender;
                if (speechEngine.SpeechProcessingException != null)
                {
                    throw speechEngine.SpeechProcessingException;
                }
                var resultDictionary = speechEngine.ResultDictionary;
                StartCommandExecution(resultDictionary);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Visible, MessageBoxIcon.Error);
                throw;
            }
        }

        #endregion

        # region MessageBox

        private void CloseMessageBox()
        {
            try
            {
                var sb = (Storyboard)FindResource("MessageBoxClose");
                sb.Begin();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        private void OpenMessageBox()
        {
            try
            {
                var sb = (Storyboard)FindResource("MessageBoxOpen");
                sb.Begin();

                SpeakMessage();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        private void ShowMessageBoxDetails(string message, string messageTitle, Visibility yesButtonVisible, Visibility noButtonVisible, MessageBoxIcon icon)
        {
            SetImage(icon);
            LblTitle.Content = messageTitle;
            TxtMessage.Text = message;
            BtnYes.Visibility = yesButtonVisible;
            BtnNo.Visibility = noButtonVisible;
            BtnOk.Visibility = Visibility.Collapsed;
            OpenMessageBox();
        }

        private void ShowMessageBoxDetails(string message, string messageTitle, Visibility okButtonVisible, MessageBoxIcon icon)
        {
            SetImage(icon);
            LblTitle.Content = messageTitle;
            TxtMessage.Text = message;
            BtnOk.Visibility = okButtonVisible;
            BtnNo.Visibility = Visibility.Collapsed;
            BtnYes.Visibility = Visibility.Collapsed;
            OpenMessageBox();
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

        private void SpeakMessage()
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

        #endregion

        #region Voice Recognizer

        private void InitializeSpeechRecognizer()
        {
            try
            {
                var result = "";
                SpeechRecognizerEngine = new SpeechEngine().CreateSpeechEngine("en-GB", out result);
                SpeechRecognizerEngine.LoadGrammar(new DictationGrammar());
                SpeechRecognizerEngine.LoadGrammar(new SpeechEngine().GetSpellingGrammar());
                SpeechRecognizerEngine.LoadGrammar(new SpeechEngine().GetWebSiteNamesGrammar());
                SpeechRecognizerEngine.AudioLevelUpdated += SpeechRecognizerEngine_AudioLevelUpdated;
                SpeechRecognizerEngine.SpeechRecognized += SpeechRecognizerEngine_SpeechRecognized;

                // use the system's default microphone
                SpeechRecognizerEngine.SetInputToDefaultAudioDevice();

                // start listening
                SpeechRecognizerEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void StoSpeechRecognizer()
        {
            SpeechRecognizerEngine.RecognizeAsyncStop();
        }

        private void SpeechRecognizerEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            try
            {
                if (e.Result.Confidence >= 0.7)
                {
                    var resultDictionary = new SpeechEngine().InitializeSpeechProcessing(e.Result.Text);
                    StartCommandExecution(resultDictionary);
                }
                else
                {
                    ShowMessageBoxDetails("Your words cannot be recognized properly!", "Information", Visibility.Visible,
                                          MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
            
        }

        private void SpeechRecognizerEngine_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            try
            {
                PbAudioLevel.Value = e.AudioLevel;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        #endregion

        # region Tab Item

        public static void SetTabItemViewModel(TabItemViewModel tabItemViewModel)
        {
            try
            {
                TabItemViewModel = tabItemViewModel;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        private void AddNewTab()
        {
            try
            {
                TabItemViewModel.AddTabItem();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void RemoveTabByIndex(int index)
        {
            TabItemViewModel.RemoveTabItemByIndex(index);
        }

        private void GoToTabByIndex(int index)
        {
            try
            {
                TabItemViewModel.SetFocusOnTabItem(index - 1);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void RemoveCurrentTab()
        {
            try
            {
                TabItemViewModel.RemoveCurrentTabItem();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        # endregion

        # region Command Execution

        private void StartCommandExecution(Dictionary<CommandType, object> resultDictionary)
        {
            try
            {
                switch (CommandMode)
                {
                    case Mode.CommandMode:
                        ExecuteCommand(resultDictionary);
                        break;
                    case Mode.DictationMode:
                        ExecuteDictationCommand(resultDictionary);
                        CommandMode = Mode.CommandMode;
                        break;
                    case Mode.WebsiteSpellMode:
                        ExecuteSpellingCommand(true, resultDictionary);
                        CommandMode = Mode.CommandMode;
                        break;
                    case Mode.GeneralSpellMode:
                        ExecuteSpellingCommand(false, resultDictionary);
                        CommandMode = Mode.CommandMode;
                        break;
                    case Mode.PasswordSpellMode:
                        ExecutePasswordSpelling(resultDictionary);
                        CommandMode = Mode.CommandMode;
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        public void ExecuteCommand(Dictionary<CommandType, object> identifiedCommand)
        {
            try
            {
                var command = identifiedCommand.First();
                switch (command.Key)
                {
                    case CommandType.go:
                        {
                            var identifiedWebSite = command.Value.ToString();
                            if (!identifiedWebSite.Contains(".com") && !identifiedWebSite.Contains(".lk") &&
                                !identifiedWebSite.Contains(".org") && !identifiedWebSite.Contains(".info") &&
                                !identifiedWebSite.Contains(".net"))
                            {
                                identifiedWebSite += ".com";
                            }
                            var websiteName = "http://www." + identifiedWebSite;
                            NavigateToUrl(websiteName);
                            break;
                        }
                    case CommandType.back:
                        {
                            MoveBackward();
                            break;
                        }
                    case CommandType.forth:
                        {
                            MoveForward();
                            break;
                        }
                    case CommandType.refresh:
                        {
                            RefreshBrowser();
                            break;
                        }
                    case CommandType.stop:
                        {
                            StopBrowser();
                            break;
                        }
                    case CommandType.alter:
                        {
                            InvokePostMessageService("%");
                            break;
                        }
                    case CommandType.backspace:
                        {
                            InvokePostMessageService("{BS}");
                            break;
                        }
                    case CommandType.capslock:
                        {
                            InvokePostMessageService("{CAPSLOCK}");
                            break;
                        }
                    case CommandType.control:
                        {
                            InvokePostMessageService("^");
                            break;
                        }
                    case CommandType.down_arrow:
                        {
                            InvokePostMessageService("{DOWN}");
                            break;
                        }
                    case CommandType.enter:
                        {
                            InvokePostMessageService("{ENTER}");
                            break;
                        }
                    case CommandType.f5:
                        {
                            InvokePostMessageService("{F5}");
                            break;
                        }
                    case CommandType.left_arrow:
                        {
                            InvokePostMessageService("{LEFT}");
                            break;
                        }
                    case CommandType.right_arrow:
                        {
                            InvokePostMessageService("{RIGHT}");
                            break;
                        }
                    case CommandType.space:
                        {
                            InvokePostMessageService(" ");
                            break;
                        }
                    case CommandType.tab:
                        {
                            InvokePostMessageService("{TAB}");
                            break;
                        }
                    case CommandType.up_arrow:
                        {
                            InvokePostMessageService("{UP}");
                            break;
                        }
                    case CommandType.scroll_up:
                        {
                            InvokePostMessageService("{PGUP}");
                            break;
                        }
                    case CommandType.scroll_down:
                        {
                            InvokePostMessageService("{PGDN}");
                            break;
                        }
                    case CommandType.scroll_left:
                        {
                            InvokePostMessageService("{LEFT}");
                            break;
                        }
                    case CommandType.scroll_right:
                        {
                            InvokePostMessageService("{RIGHT}");
                            break;
                        }
                    case CommandType.move:
                        {
                            var coordinates = command.Value.ToString();
                            var seperatedCoordinates = coordinates.Split(',').ToList();
                            SetPosition(Convert.ToInt32(seperatedCoordinates.First()), Convert.ToInt32(seperatedCoordinates.Last()));
                            break;
                        }
                    case CommandType.click:
                        {
                            LeftMouseClick();
                            break;
                        }
                    case CommandType.right_click:
                        {
                            RightMouseClick();
                            break;
                        }
                    case CommandType.double_click:
                        {
                            DoubleClick();
                            break;
                        }
                    case CommandType.open_new_tab:
                        {
                            AddNewTab();
                            break;
                        }
                    case CommandType.go_to_tab:
                        {
                            var index = command.Value.ToString();
                            GoToTabByIndex(Convert.ToInt32(index));
                            break;
                        }
                    case CommandType.close_tab:
                        {
                            RemoveCurrentTab();
                            break;
                        }
                    case CommandType.start_dictation_mode:
                        {
                            CommandMode = Mode.DictationMode;
                            break;
                        }
                    case CommandType.start_website_spelling:
                        {
                            CommandMode = Mode.WebsiteSpellMode;
                            break;
                        }
                    case CommandType.start_password_spelling:
                        {
                            CommandMode = Mode.PasswordSpellMode;
                            break;
                        }
                    case CommandType.start_general_spelling:
                        {
                            CommandMode = Mode.GeneralSpellMode;
                            break;
                        }
                    case CommandType.yes:
                        {
                            ExecuteYesNoCommand(CommandType.yes);
                            break;
                        }
                    case CommandType.no:
                        {
                            ExecuteYesNoCommand(CommandType.no);
                            break;
                        }
                    case CommandType.ok:
                        {
                            ExecuteYesNoCommand(CommandType.ok);
                            break;
                        }
                    case CommandType.show_grid:
                        {
                            DisplayGird(true);
                            break;
                        }
                    case CommandType.hide_grid:
                        {
                            DisplayGird(false);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void ExecuteYesNoCommand(CommandType type)
        {
            try
            {
                switch (type)
                {
                    case CommandType.yes:
                        CloseMessageBox();
                        break;
                    case CommandType.no:
                        CloseMessageBox();
                        break;
                    case CommandType.ok:
                        CloseMessageBox();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void ExecuteDictationCommand(Dictionary<CommandType, object> dictationCommand)
        {
            try
            {
                foreach (var pair in dictationCommand)
                {
                    InvokePostMessageService(pair.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void ExecuteSpellingCommand(bool webSiteSpelling, Dictionary<CommandType, object> dictationCommand)
        {
            try
            {
                var firstPair = dictationCommand.First();
                var word = AcquireSpelledWord(firstPair.Value.ToString());
                if (webSiteSpelling)
                    AppendWebsiteToTextFile(word);
                else
                    InvokePostMessageService(word);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void ExecutePasswordSpelling(Dictionary<CommandType, object> dictationCommand)
        {
            var credential = "";
            var caseState = CaseState.Default;
            try
            {
                foreach (var segments in dictationCommand.Select(pair => pair.Value.ToString()).Select(command => command.Split(' ')))
                {
                    foreach (var segment in segments)
                    {
                        switch(caseState)
                        {
                            case CaseState.Default:
                                {
                                    if (String.Equals(segment, "capital", StringComparison.OrdinalIgnoreCase))
                                        caseState = CaseState.UpperCase;
                                    else if (String.Equals(segment, "simple", StringComparison.OrdinalIgnoreCase))
                                        caseState = CaseState.LowerCase;
                                    else if (String.Equals(segment, "one", StringComparison.OrdinalIgnoreCase))
                                        credential += 1;
                                    else if (String.Equals(segment, "two", StringComparison.OrdinalIgnoreCase))
                                        credential += 2;
                                    else if (String.Equals(segment, "three", StringComparison.OrdinalIgnoreCase))
                                        credential += 3;
                                    else if (String.Equals(segment, "four", StringComparison.OrdinalIgnoreCase))
                                        credential += 4;
                                    else if (String.Equals(segment, "five", StringComparison.OrdinalIgnoreCase))
                                        credential += 5;
                                    else if (String.Equals(segment, "six", StringComparison.OrdinalIgnoreCase))
                                        credential += 6;
                                    else if (String.Equals(segment, "seven", StringComparison.OrdinalIgnoreCase))
                                        credential += 7;
                                    else if (String.Equals(segment, "eight", StringComparison.OrdinalIgnoreCase))
                                        credential += 8;
                                    else if (String.Equals(segment, "nine", StringComparison.OrdinalIgnoreCase))
                                        credential += 9;
                                    else if (String.Equals(segment, "zero", StringComparison.OrdinalIgnoreCase))
                                        credential += 0;
                                    else if (String.Equals(segment, "underscore", StringComparison.OrdinalIgnoreCase))
                                        credential += "_";
                                    else if (String.Equals(segment, "hash", StringComparison.OrdinalIgnoreCase))
                                        credential += "#";
                                    else if (String.Equals(segment, "dot", StringComparison.OrdinalIgnoreCase))
                                        credential += ".";
                                    else if (String.Equals(segment, "at", StringComparison.OrdinalIgnoreCase))
                                        credential += "@";
                                    else
                                        credential += segment.ToLower();
                                    break;
                                }
                            case CaseState.UpperCase:
                                {
                                    credential += segment.ToUpper();
                                    caseState = CaseState.Default;
                                    break;
                                }
                            case CaseState.LowerCase:
                                {
                                    credential += segment.ToLower();
                                    caseState = CaseState.Default;
                                    break;
                                }
                        }
                    }
                }
                InvokePostMessageService(credential);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private void AppendWebsiteToTextFile(string website)
        {
            try
            {
                VbwFileManager.AppendToTextFile(VbwFileManager.FilePath() + "fnc_brwsr_websites" + VbwFileManager.FileExtension(), new List<string> { website.ToLower() });
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                ShowMessageBoxDetails(ex.Message, "Error", Visibility.Collapsed, MessageBoxIcon.Error);
                throw;
            }
        }

        private static void InvokePostMessageService(string message)
        {
            SendKeysServiceClient svcClient = null;
            try
            {
                svcClient = new SendKeysServiceClient();
                svcClient.PostMessage(message, 3);
            }
            catch (Exception ex)
            {
                if (svcClient != null) svcClient.Close();
                Log.ErrorLog(ex);
                throw;
            }
            finally
            {
                if (svcClient != null) svcClient.Abort();
            }
        }

        private static string AcquireSpelledWord(string spelledWord)
        {
            try
            {
                var letters = spelledWord.Split(' ');
                return letters.Aggregate("", (current, letter) => current + letter);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        private void DisplayGird(bool displayGrid)
        {
            if (displayGrid)
                TabItemViewModel.ShowGrid();
            else
                TabItemViewModel.HideGrid();
        }

        # endregion

        private void BtnTest_OnClick(object sender, RoutedEventArgs e)
        {
            ShowMessageBoxDetails("This is just a sample error message", "Sample Error Message", Visibility.Visible,
                                  MessageBoxIcon.Error);
        }

        private void BtnTest1_OnClick(object sender, RoutedEventArgs e)
        {
            CloseMessageBox();
        }

        private void ChangeBackground()
        {
            //BackgroundImage.ImageSource
        }

    }
}