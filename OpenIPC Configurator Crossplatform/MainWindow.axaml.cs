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
            if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\send.bat"))
            {
                using var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\send.bat");
                sw.WriteLine("pscp -scp -unsafe -pw 12345 /OpenIPCConfigurator/majestic.yaml root@%1:/etc/ \r\npscp -scp -unsafe -pw 12345 /OpenIPCConfigurator/wfb.conf root@%1:/etc/ ");
                sw.Close();
            }

            var process = new ProcessStartInfo();
            process.UseShellExecute = false;
            process.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\send.bat";
            process.Arguments = string.Format("{0}", tbIpAddress.Text); // add here to checking correctly written ip for future
            process.RedirectStandardOutput = true;
            Process.Start(process);

        }

        private void BRead_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            using var wfbReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\wfb.conf");
            using var camReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\majestic.yaml");

            

            /*
             Dim WFBreader As New IO.StreamReader("C:\OpenIPCConfigurator\wfb.conf")
        Dim Camreader As New IO.StreamReader("C:\OpenIPCConfigurator\majestic.yaml")
        Dim WFBallLines = New List(Of String)
        Dim CamallLines = New List(Of String)
        Do While Not WFBreader.EndOfStream
            WFBallLines.Add(WFBreader.ReadLine)
        Loop
        WFBreader.Close()
        txtFrequency.Text = ReadLine(7, WFBallLines)
        txtPower.Text = ReadLine(10, WFBallLines)
        txtFreq24.Text = ReadLine(8, WFBallLines)
        txtPower24.Text = ReadLine(9, WFBallLines)
        txtMCS.Text = ReadLine(14, WFBallLines)
        txtSTBC.Text = ReadLine(12, WFBallLines)
        txtLDPC.Text = ReadLine(13, WFBallLines)
        txtFECK.Text = ReadLine(20, WFBallLines)
        txtFECN.Text = ReadLine(21, WFBallLines)

        Do While Not Camreader.EndOfStream
            CamallLines.Add(Camreader.ReadLine)
        Loop
        Camreader.Close()
        txtResolution.Text = ReadLine(28, CamallLines)
        txtFPS.Text = ReadLine(29, CamallLines)
        txtEncode.Text = ReadLine(25, CamallLines)
        txtBitrate.Text = ReadLine(24, CamallLines)
        txtExposure.Text = ReadLine(61, CamallLines)
        txtContrast.Text = ReadLine(8, CamallLines)
        txtHue.Text = ReadLine(9, CamallLines)
        txtSaturation.Text = ReadLine(10, CamallLines)
        txtLuminance.Text = ReadLine(11, CamallLines)
        txtSensor.Text = ReadLine(60, CamallLines)
             */
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