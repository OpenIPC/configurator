using Avalonia.Controls;
using System.Threading;

namespace OpenIPC_Configurator_Crossplatform
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            bFetch.Click += BFetch_Click;
            bRead.Click += BRead_Click;
            bUpload.Click += BUpload_Click;
        }

        private void BUpload_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void BRead_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void BFetch_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        /*
         btnFetch
        Process()
            .StartInfo.UseShellExecute = False
            .StartInfo.FileName = "C:\OpenIPCConfigurator\get.bat"
            .StartInfo.Arguments = String.Format("{0}", txtIP.Text)
            .StartInfo.RedirectStandardOutput = True
            .Start()

         */

        /*
        btnSend
        Process()
            .StartInfo.UseShellExecute = False
            .StartInfo.FileName = "C:\OpenIPCConfigurator\send.bat"
            .StartInfo.Arguments = String.Format("{0}", txtIP.Text)
            .StartInfo.RedirectStandardOutput = True
            .Start()

         */

        /*
         

         */
    }
}