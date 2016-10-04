using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for EmulatorWindow.xaml
    /// </summary>
    public partial class EmulatorWindow
    {
        public EmulatorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The image displayed by the button.
        /// </summary>
        /// <remarks>The image is specified in XAML as an absolute or relative path.</remarks>
        [Description("The image displayed by the button"), Category("Common Properties")]
        public string CommandText
        {
            get { return (string)GetValue(CommandTxt); }
            set { SetValue(CommandTxt, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty CommandTxt = DependencyProperty.Register("CommandText", typeof(string), typeof(BookmarkButton), new UIPropertyMetadata(null));

        [Description("The image displayed by the button"), Category("Common Properties")]
        public ICommand EmulatorCommand
        {
            get { return (ICommand)GetValue(EmulatorCmd); }
            set { SetValue(EmulatorCmd, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty EmulatorCmd = DependencyProperty.Register("EmulatorCommand", typeof(ICommand), typeof(BookmarkButton), new UIPropertyMetadata(null));
    }
}
