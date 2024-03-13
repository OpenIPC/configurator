Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1
    Private Sub btnGet_Click(sender As Object, e As EventArgs) Handles btnGet.Click
        With New Process()
            .StartInfo.UseShellExecute = False
            .StartInfo.FileName = "C:\OpenIPCConfigurator\get.bat"
            .StartInfo.Arguments = String.Format("{0}", txtIP.Text)
            .StartInfo.RedirectStandardOutput = True
            .Start()
        End With
    End Sub

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        With New Process()
            .StartInfo.UseShellExecute = False
            .StartInfo.FileName = "C:\OpenIPCConfigurator\send.bat"
            .StartInfo.Arguments = String.Format("{0}", txtIP.Text)
            .StartInfo.RedirectStandardOutput = True
            .Start()
        End With
    End Sub

    Public Function ReadLine(lineNumber As Integer, lines As List(Of String)) As String
        Return lines(lineNumber - 1)
    End Function

    Private Sub txtSaveFreq_Click(sender As Object, e As EventArgs) Handles txtSaveFreq.Click
        Dim WFBfilePath As String = "C:\OpenIPCConfigurator\wfb.conf"
        Dim lines = IO.File.ReadAllLines(WFBfilePath)
        If lines(6).StartsWith("channel=") Then
            lines(6) = txtFrequency.Text
        End If
        If lines(9).StartsWith("driver_txpower_override=") Then
            lines(9) = txtPower.Text
        End If
        If lines(7).StartsWith("frequency=") Then
            lines(7) = txtFreq24.Text
        End If
        If lines(8).StartsWith("txpower=") Then
            lines(8) = txtPower24.Text
        End If
        If lines(11).StartsWith("stbc=") Then
            lines(11) = txtSTBC.Text
        End If
        If lines(12).StartsWith("ldpc=") Then
            lines(12) = txtLDPC.Text
        End If
        If lines(13).StartsWith("mcs_index=") Then
            lines(13) = txtMCS.Text
        End If
        If lines(19).StartsWith("fec_k=") Then
            lines(19) = txtFECK.Text
        End If
        If lines(20).StartsWith("fec_n=") Then
            lines(20) = txtFECN.Text
        End If
        IO.File.WriteAllLines(WFBfilePath, lines)
    End Sub

    Private Sub txtSaveCam_Click(sender As Object, e As EventArgs) Handles txtSaveCam.Click
        Dim CamfilePath As String = "C:\OpenIPCConfigurator\majestic.yaml"
        Dim lines = IO.File.ReadAllLines(CamfilePath)
        If lines(7).StartsWith("  contrast: ") Then
            lines(7) = txtContrast.Text
        End If
        If lines(8).StartsWith("  hue: ") Then
            lines(8) = txtHue.Text
        End If
        If lines(9).StartsWith("  saturation:") Then
            lines(9) = txtSaturation.Text
        End If
        If lines(10).StartsWith("  luminance: ") Then
            lines(10) = txtLuminance.Text
        End If
        If lines(23).StartsWith("  bitrate: ") Then
            lines(23) = txtBitrate.Text
        End If
        If lines(24).StartsWith("  codec: ") Then
            lines(24) = txtEncode.Text
        End If
        If lines(27).StartsWith("  size: ") Then
            lines(27) = txtResolution.Text
        End If
        If lines(28).StartsWith("  fps: ") Then
            lines(28) = txtFPS.Text
        End If
        If lines(59).StartsWith("  sensorConfig: ") Then
            lines(59) = txtSensor.Text
        End If
        If lines(60).StartsWith("  exposure: ") Then
            lines(60) = txtExposure.Text
        End If
        IO.File.WriteAllLines(CamfilePath, lines)
    End Sub

    Private Sub btnRead_Click(sender As Object, e As EventArgs) Handles btnRead.Click
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
    End Sub
End Class
