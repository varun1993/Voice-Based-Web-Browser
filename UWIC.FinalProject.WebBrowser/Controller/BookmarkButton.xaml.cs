using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for BookmarkButton.xaml
    /// </summary>
    public partial class BookmarkButton
    {
        public BookmarkButton()
        {
            // Initialize dependency properties
            InitializeComponent();
        }

        #region Custom Control Properties

        /// <summary>
        /// The image displayed by the button.
        /// </summary>
        /// <remarks>The image is specified in XAML as an absolute or relative path.</remarks>
        [Description("The image displayed by the button"), Category("Common Properties")]
        public ImageSource DefaultImage
        {
            get { return (ImageSource)GetValue(ImageProperty1); }
            set { SetValue(ImageProperty1, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty ImageProperty1 = DependencyProperty.Register("DefaultImage", typeof(ImageSource), typeof(BookmarkButton), new UIPropertyMetadata(null));

        ///// <summary>
        ///// The text displayed by the button.
        ///// </summary>
        [Description("The hover image displayed by the button."), Category("Common Properties")]
        public ImageSource HoverImage
        {
            get { return (ImageSource)GetValue(ImageProperty2); }
            set { SetValue(ImageProperty2, value); }
        }
        // Dependency property backing variables
        public static readonly DependencyProperty ImageProperty2 = DependencyProperty.Register("HoverImage", typeof(ImageSource), typeof(BookmarkButton), new UIPropertyMetadata(null));

        #endregion
    }
}
