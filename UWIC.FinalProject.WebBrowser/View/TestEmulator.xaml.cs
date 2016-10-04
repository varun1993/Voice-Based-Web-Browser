using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.WebBrowser.View
{
    /// <summary>
    /// Interaction logic for TestEmulator.xaml
    /// </summary>
    public partial class TestEmulator : Elysium.Controls.Window
    {
        SpeechRecognitionEngine.SpeechEngine speechEngine { get; set; }
        public TestEmulator()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            speechEngine = new SpeechRecognitionEngine.SpeechEngine();
            string val = txtSpeech.Text;
            speechEngine.StartEmulatorRecognition(val);
        }

        private void TxtSpeech_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.U)
                EmulateKeyPress();
        }

        private void EmulateKeyPress()
        {
            //var key = Key.Space;                    // Key to send
            //var target = Keyboard.FocusedElement;    // Target element
            //var routedEvent = Keyboard.KeyDownEvent; // Event to send

            //if (Keyboard.PrimaryDevice.ActiveSource != null)
            //    target.RaiseEvent(
            //        new KeyEventArgs(
            //            Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource,
            //            0,
            //            key) { RoutedEvent = routedEvent }
            //        );

            //if (Keyboard.PrimaryDevice != null)
            //    if (Keyboard.PrimaryDevice.ActiveSource != null)
            //        InputManager.Current.ProcessInput(
            //            new KeyEventArgs(Keyboard.PrimaryDevice,
            //                             Keyboard.PrimaryDevice.ActiveSource,
            //                             0, Key.Space)
            //                {
            //                    RoutedEvent = Keyboard.KeyDownEvent
            //                }
            //            );

            var args = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.X)
            {
                RoutedEvent = Keyboard.KeyDownEvent
            };

            InputManager.Current.ProcessInput(args);
        }
    }
}
