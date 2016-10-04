using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for CloseButton.xaml
    /// </summary>
    public partial class ControlButton
    {
        public ControlButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The image displayed by the button.
        /// </summary>
        /// <remarks>The image is specified in XAML as an absolute or relative path.</remarks>
        [Description("The image displayed by the button"), Category("Common Properties")]
        public ImageSource DefaultControlImage
        {
            get { return (ImageSource)GetValue(ImageProperty1); }
            set { SetValue(ImageProperty1, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty ImageProperty1 = DependencyProperty.Register("DefaultControlImage", typeof(ImageSource), typeof(ControlButton), new UIPropertyMetadata(null));

        ///// <summary>
        ///// The text displayed by the button.
        ///// </summary>
        [Description("The hover image displayed by the button."), Category("Common Properties")]
        public ImageSource HoverControlImage
        {
            get { return (ImageSource)GetValue(ImageProperty2); }
            set { SetValue(ImageProperty2, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty ImageProperty2 = DependencyProperty.Register("HoverControlImage", typeof(ImageSource), typeof(ControlButton), new UIPropertyMetadata(null));

    }
}
