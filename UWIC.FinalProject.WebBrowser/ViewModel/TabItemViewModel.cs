using Microsoft.TeamFoundation.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UWIC.FinalProject.WebBrowser.Controller;
using Elysium;

namespace UWIC.FinalProject.WebBrowser.ViewModel
{
    public class TabItemViewModel : MainViewModel
    {
        # region Commands

        public ICommand ClickCommand { get; set; }

        public ICommand GridShowCommand { get; set; }

        public ICommand GridHideCommand { get; set; }

        # endregion

        #region Properties

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        private ObservableCollection<TabItem> _tabitems = new ObservableCollection<TabItem>();
        public ObservableCollection<TabItem> TabItems
        {
            get
            {
                return _tabitems;
            }
            set
            {
                _tabitems = value;
                OnPropertyChanged("TabItems");
            }
        }

        private Visibility _gridVisibility = Visibility.Collapsed;
        public Visibility GridVisibility
        {
            get { return _gridVisibility; }
            set
            {
                _gridVisibility = value;
                OnPropertyChanged("GridVisibility");
            }
        }
        
        #endregion

        public TabItemViewModel()
        {
            TabItem myItem = new TabItem();
            myItem.Header = getNewTabItemHeader();
            myItem.Content = new BrowserContainer();
            TabItems.Add(myItem);

            HideGrid();

            this.ClickCommand = new RelayCommand(AddTabItem);
            TabItemHeader.SetViewModel(this);
            BrowserContainer.SetTabItemViewModel(this);

            GridShowCommand = new RelayCommand(ShowGrid);

            GridHideCommand = new RelayCommand(HideGrid);
        }

        private TabItemHeader getNewTabItemHeader()
        {
            return new TabItemHeader(getBlankPageImage(), "New Tab");
        }

        public BitmapImage getBlankPageImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("/Images/icon-page.png", UriKind.RelativeOrAbsolute);
            image.EndInit();
            return image;
        }

        /// <summary>
        /// This method is used to add a new tab item to the tab control
        /// </summary>
        public void AddTabItem()
        {
            var myItem = new TabItem {Header = getNewTabItemHeader(), Content = new BrowserContainer()};
            TabItems.Add(myItem);
            SelectedIndex = TabItems.Count - 1;
        }

        /// <summary>
        /// This method is used to Remove a particular tabitem by providing the HashCode of it
        /// </summary>
        /// <param name="HashCode">Hash-Code of the tabitem</param>
        public void RemoveTabItem(object HashCode)
        {
            var res = TabItems.First(rec => rec.GetHashCode() == (int)HashCode);
            TabItems.Remove(res);
        }

        /// <summary>
        /// This method is used to Remove a particular tabitem by providing the index of it
        /// </summary>
        /// <param name="index">Index of the tabitem</param>
        public void RemoveTabItemByIndex(int index)
        {
            TabItems.RemoveAt(index);
        }

        /// <summary>
        /// This method is used to Remove a particular tabitem by providing the index of it
        /// </summary>
        /// <param name="index">Index of the tabitem</param>
        public void RemoveCurrentTabItem()
        {
            TabItems.RemoveAt(SelectedIndex);
        }

        /// <summary>
        /// Set Focus on a given tab item index
        /// </summary>
        /// <param name="index"></param>
        public void SetFocusOnTabItem(int index)
        {
            SelectedIndex = index;
        }

        public void ShowGrid()
        {
            GridVisibility = Visibility.Visible;
        }

        public void HideGrid()
        {
            GridVisibility = Visibility.Hidden;
        }
    }
}
