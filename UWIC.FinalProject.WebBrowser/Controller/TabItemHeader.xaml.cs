using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UWIC.FinalProject.WebBrowser.ViewModel;

namespace UWIC.FinalProject.WebBrowser.Controller
{
    /// <summary>
    /// Interaction logic for TabItemHeader.xaml
    /// </summary>
    public partial class TabItemHeader
    {
        private static TabItemViewModel ViewModel { get; set; }
        public CharacterCasing Casing = CharacterCasing.Upper;

        public TabItemHeader(BitmapImage image, string title)
        {
            InitializeComponent();
            pageIcon.Source = image;
            PageTitle.Text = title;
            ToolTipService.SetToolTip(PageTitle, PageTitle.Text);
            Resources.Add("ConvertUpperCase", Casing);
        }

        private void btnClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var parent = (TabItem)(Parent);
            var hashCode = parent.GetHashCode();
            ViewModel.RemoveTabItem(hashCode);
        }

        public static void SetViewModel(TabItemViewModel vm)
        {
            ViewModel = vm;
        }
    }
}
