using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.IO;
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
            bSaveWfb.Click += BSaveWfb_Click;
            bSaveCameraSettings.Click += BSaveCameraSettings_Click;
        }

        private void BSaveCameraSettings_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void BSaveWfb_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
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
            if (!File.Exists( AppDomain.CurrentDomain.BaseDirectory + "\\get.bat"))
            {
                //File.Create(AppDomain.CurrentDomain.BaseDirectory + "\\get.bat");
                using var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\get.bat");
                // what mean /OpenIPCConfigurator? Is Default Folder?
                sw.WriteLine("pscp -scp -unsafe -pw 12345 root@%1:/etc/majestic.yaml /OpenIPCConfigurator/\r\npscp -scp -unsafe -pw 12345 root@%1:/etc/wfb.conf /OpenIPCConfigurator/");
                sw.Close();
            }

            var process = new ProcessStartInfo();
            process.UseShellExecute = false;
            process.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\get.bat";
            process.Arguments = string.Format("{0}", tbIpAddress.Text); // feat: check for correct ip
            process.RedirectStandardOutput = true;
            Process.Start(process);

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