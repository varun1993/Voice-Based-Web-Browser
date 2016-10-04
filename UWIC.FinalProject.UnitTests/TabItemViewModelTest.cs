using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UWIC.FinalProject.WebBrowser.ViewModel;

namespace UWIC.FinalProject.UnitTests
{
    [TestClass]
    public class TabItemViewModelTest
    {
        [TestMethod]
        public void GetBlankPageImageTest()
        {
            var vm = new TabItemViewModel();
            var image = vm.getBlankPageImage();
            Assert.AreEqual(image.UriSource.ToString(), "/Images/icon-page.png");
        }
    }
}
