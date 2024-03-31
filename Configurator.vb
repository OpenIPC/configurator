Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Reflection

Public Class Configurator
    Private Sub btnGet_Click(sender As Object, e As EventArgs) Handles btnGet.Click
        Dim extern = "extern.bat"
        If Not System.IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                If rBtnNVR.Checked Then
                    .StartInfo.Arguments = "dlvrx " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                ElseIf rBtnRadxaZero3w.Checked Then
                    .StartInfo.Arguments = "dlwfbng " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                Else
                    .StartInfo.Arguments = "dl " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                End If
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        Dim extern = "extern.bat"
        If Not System.IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                If rBtnNVR.Checked Then
                    .StartInfo.Arguments = "ulvrx " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                ElseIf rBtnRadxaZero3w.Checked Then
                    .StartInfo.Arguments = "ulwfbng " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                Else
                    .StartInfo.Arguments = "ul " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                End If
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Public Function ReadLine(lineNumber As Integer, lines As List(Of String)) As String
        Return lines(lineNumber - 1)
    End Function

    Private Sub txtSaveFreq_Click(sender As Object, e As EventArgs) Handles txtSaveFreq.Click
        If txtFrequency.Text <> "" Then
            Dim wfbconf = "wfb.conf"
            If Not IO.File.Exists(wfbconf) Then
                MsgBox("File " + wfbconf + " not found!")
                Return
            End If
            Dim x, y As Integer
            Dim WFBfilePath = wfbconf
            Dim lines = IO.File.ReadAllLines(WFBfilePath)
            For x = 0 To lines.Count() - 1
                If rBtnRadxaZero3w.Checked Then
                    Dim wfbng = "wifibroadcast.cfg"
                    If Not IO.File.Exists(wfbng) Then
                        MsgBox("File " + wfbng + " not found!")
                        Return
                    End If

                    Dim wfbngfilePath = wfbng
                    Dim WFBlines = IO.File.ReadAllLines(wfbngfilePath)

                    For y = 0 To WFBlines.Count() - 1
                        If WFBlines(y).StartsWith("wifi_channel = ") Then
                            WFBlines(y) = txtFrequency.Text
                        End If
                        If WFBlines(y).StartsWith("peer = 'connect://") Then
                            WFBlines(y) = txtMCS.Text
                        End If
                        If WFBlines(y).StartsWith("peer = 'connect://") Then
                            WFBlines(y) = txtSTBC.Text
                        End If
                    Next
                    IO.File.WriteAllLines(wfbngfilePath, WFBlines)
                    If lines(x).StartsWith("rtw_tx_pwr_idx_override=") Then
                        lines(x) = txtPower.Text
                    End If
                Else
                    If lines(x).StartsWith("channel=") Then
                        lines(x) = txtFrequency.Text
                    End If
                    If lines(x).StartsWith("driver_txpower_override=") Then
                        lines(x) = txtPower.Text
                    End If
                    If lines(x).StartsWith("frequency=") Then
                        lines(x) = txtFreq24.Text
                    End If
                    If lines(x).StartsWith("txpower=") Then
                        lines(x) = txtPower24.Text
                    End If
                    If rBtnNVR.Checked Then
                        If lines(x).StartsWith("udp_addr=") Then
                            lines(x) = txtMCS.Text
                        End If
                        If lines(x).StartsWith("udp_port=") Then
                            lines(x) = txtSTBC.Text
                        End If
                    Else
                        If lines(x).StartsWith("stbc=") Then
                            lines(x) = txtSTBC.Text
                        End If
                        If lines(x).StartsWith("ldpc=") Then
                            lines(x) = txtLDPC.Text
                        End If
                        If lines(x).StartsWith("mcs_index=") Then
                            lines(x) = txtMCS.Text
                        End If
                        If lines(x).StartsWith("fec_k=") Then
                            lines(x) = txtFECK.Text
                        End If
                        If lines(x).StartsWith("fec_n=") Then
                            lines(x) = txtFECN.Text
                        End If
                    End If
                End If
            Next
            IO.File.WriteAllLines(WFBfilePath, lines)
            MsgBox("Settings saved successfully", MsgBoxStyle.Information, "OpenIPC")
        End If
    End Sub

    Private Sub txtSaveCam_Click(sender As Object, e As EventArgs) Handles txtSaveCam.Click
        If txtResolution.Text <> "" Then
            Dim majestic = "majestic.yaml"
            If Not IO.File.Exists(majestic) Then
                MsgBox("File " + majestic + " not found!")
                Return
            End If
            Dim x As Integer
            Dim CamfilePath = majestic
            Dim lines = IO.File.ReadAllLines(CamfilePath)
            For x = 0 To lines.Count() - 1
                If lines(x).StartsWith("  contrast: ") Then
                    lines(x) = txtContrast.Text
                End If
                If lines(x).StartsWith("  hue: ") Then
                    lines(x) = txtHue.Text
                End If
                If lines(x).StartsWith("  saturation:") Then
                    lines(x) = txtSaturation.Text
                End If
                If lines(x).StartsWith("  luminance: ") Then
                    lines(x) = txtLuminance.Text
                End If
                If lines(x).StartsWith("  bitrate: ") Then
                    lines(x) = txtBitrate.Text
                End If
                If lines(x).StartsWith("  codec: ") Then
                    lines(x) = txtEncode.Text
                End If
                If lines(x).StartsWith("  size: ") Then
                    lines(x) = txtResolution.Text
                End If
                If lines(x).StartsWith("  fps: ") Then
                    lines(x) = txtFPS.Text
                End If
                If lines(x).StartsWith("  sensorConfig: ") Then
                    lines(x) = txtSensor.Text
                End If
                If lines(x).StartsWith("  exposure: ") Then
                    lines(x) = txtExposure.Text
                End If
            Next
            IO.File.WriteAllLines(CamfilePath, lines)
            MsgBox("Settings saved successfully", MsgBoxStyle.Information, "OpenIPC")
        End If
    End Sub

    Private Sub btnRead_Click(sender As Object, e As EventArgs) Handles btnRead.Click
        Dim wfbconf = "wfb.conf"
        If Not System.IO.File.Exists(wfbconf) Then
            MsgBox("File " + wfbconf + " not found!")
            Return
        End If
        Dim WFBreader As New IO.StreamReader(wfbconf)
        Dim WFBallLines = New List(Of String)

        Do While Not WFBreader.EndOfStream
            WFBallLines.Add(WFBreader.ReadLine)
        Loop
        WFBreader.Close()

        If rBtnNVR.Checked Then
            txtFrequency.Text = ReadLine(7, WFBallLines)
            txtPower.Text = ReadLine(10, WFBallLines)
            txtFreq24.Text = ReadLine(8, WFBallLines)
            txtPower24.Text = ReadLine(9, WFBallLines)
            txtMCS.Text = ReadLine(14, WFBallLines)
            txtSTBC.Text = ReadLine(15, WFBallLines)
        ElseIf rBtnRadxaZero3w.Checked Then
            Dim wfbng = "wifibroadcast.cfg"
            If Not System.IO.File.Exists(wfbng) Then
                MsgBox("File " + wfbng + " not found!")
                Return
            End If
            Dim WFBngreader As New IO.StreamReader(wfbng)
            Dim WFBngallLines = New List(Of String)

            Do While Not WFBngreader.EndOfStream
                WFBngallLines.Add(WFBngreader.ReadLine)
            Loop
            WFBngreader.Close()
            txtFrequency.Text = ReadLine(2, WFBngallLines)
            txtPower.Text = ReadLine(6, WFBallLines)
            txtMCS.Text = ReadLine(8, WFBngallLines)
            txtSTBC.Text = ReadLine(12, WFBngallLines)
        Else
            txtFrequency.Text = ReadLine(7, WFBallLines)
            txtPower.Text = ReadLine(10, WFBallLines)
            txtFreq24.Text = ReadLine(8, WFBallLines)
            txtPower24.Text = ReadLine(9, WFBallLines)
            txtMCS.Text = ReadLine(14, WFBallLines)
            txtSTBC.Text = ReadLine(12, WFBallLines)
            txtLDPC.Text = ReadLine(13, WFBallLines)
            txtFECK.Text = ReadLine(20, WFBallLines)
            txtFECN.Text = ReadLine(21, WFBallLines)
        End If

        If rBtnNVR.Checked Or rBtnCam.Checked Then
            Dim telemetry = "telemetry.conf"
            If Not System.IO.File.Exists(telemetry) Then
                MsgBox("File " + telemetry + " not found!")
                Return
            End If
            Dim TLMreader As New IO.StreamReader(telemetry)
            Dim TLMallLines = New List(Of String)

            Do While Not TLMreader.EndOfStream
                TLMallLines.Add(TLMreader.ReadLine)
            Loop
            TLMreader.Close()
            txtSerial.Text = ReadLine(4, TLMallLines)
            txtBaud.Text = ReadLine(5, TLMallLines)
            txtRouter.Text = ReadLine(8, TLMallLines)
            txtMCSTLM.Text = ReadLine(14, TLMallLines)
            If rBtnNVR.Checked Then
                Dim vdec = "vdec.conf"
                If Not System.IO.File.Exists(vdec) Then
                    MsgBox("File " + vdec + " not found!")
                    Return
                End If
                Dim VDECreader As New IO.StreamReader(vdec)
                Dim VDECallLines = New List(Of String)

                Do While Not VDECreader.EndOfStream
                    VDECallLines.Add(VDECreader.ReadLine)
                Loop
                VDECreader.Close()
                txtResolutionVRX.Text = ReadLine(22, VDECallLines)
                txtCodecVRX.Text = ReadLine(7, VDECallLines)
                txtFormat.Text = ReadLine(11, VDECallLines)
                txtPortVRX.Text = ReadLine(3, VDECallLines)
                txtMavlinkVRX.Text = ReadLine(26, VDECallLines)
                txtOSD.Text = ReadLine(30, VDECallLines)
                txtExtras.Text = ReadLine(52, VDECallLines)
            Else
                Dim majestic = "majestic.yaml"
                If Not System.IO.File.Exists(majestic) Then
                    MsgBox("File " + majestic + " not found!")
                    Return
                End If
                Dim Camreader As New IO.StreamReader(majestic)
                Dim CamallLines = New List(Of String)

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
            End If
        Else
            Dim setdisplay = "setdisplay.sh"
            If Not System.IO.File.Exists(setdisplay) Then
                MsgBox("File " + setdisplay + " not found!")
                Return
            End If
            Dim DisplayReader As New IO.StreamReader(setdisplay)
            Dim DisplayReaderallLines = New List(Of String)

            Do While Not DisplayReader.EndOfStream
                DisplayReaderallLines.Add(DisplayReader.ReadLine)
            Loop
            DisplayReader.Close()
            txtResolutionVRX.Text = ReadLine(6, DisplayReaderallLines)
            txtCodecVRX.Text = ReadLine(7, DisplayReaderallLines)
        End If
        MsgBox("Settings loaded successfully", MsgBoxStyle.Information, "OpenIPC")
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("5180 MHz [36]")
        ComboBox1.Items.Add("5200 MHz [40]")
        ComboBox1.Items.Add("5220 MHz [44]")
        ComboBox1.Items.Add("5240 MHz [48]")
        ComboBox1.Items.Add("5260 MHz [52]")
        ComboBox1.Items.Add("5280 MHz [56]")
        ComboBox1.Items.Add("5300 MHz [60]")
        ComboBox1.Items.Add("5320 MHz [64]")
        ComboBox1.Items.Add("5500 MHz [100]")
        ComboBox1.Items.Add("5520 MHz [104]")
        ComboBox1.Items.Add("5540 MHz [108]")
        ComboBox1.Items.Add("5560 MHz [112]")
        ComboBox1.Items.Add("5580 MHz [116]")
        ComboBox1.Items.Add("5600 MHz [120]")
        ComboBox1.Items.Add("5620 MHz [124]")
        ComboBox1.Items.Add("5640 MHz [128]")
        ComboBox1.Items.Add("5660 MHz [132]")
        ComboBox1.Items.Add("5680 MHz [136]")
        ComboBox1.Items.Add("5700 MHz [140]")
        ComboBox1.Items.Add("5720 MHz [144]")
        ComboBox1.Items.Add("5745 MHz [149]")
        ComboBox1.Items.Add("5765 MHz [153]")
        ComboBox1.Items.Add("5785 MHz [157]")
        ComboBox1.Items.Add("5805 MHz [161]")
        ComboBox1.Items.Add("5825 MHz [165]")
        ComboBox1.Items.Add("5845 MHz [169]")
        ComboBox1.Items.Add("5865 MHz [173]")
        ComboBox1.Items.Add("5885 MHz [177]")
        ComboBox1.Text = "Select 5.8GHz Frequency"

        ComboBox2.Items.Clear()
        ComboBox2.Items.Add("20")
        ComboBox2.Items.Add("25")
        ComboBox2.Items.Add("30")
        ComboBox2.Items.Add("35")
        ComboBox2.Items.Add("40")
        ComboBox2.Items.Add("45")
        ComboBox2.Items.Add("50")
        ComboBox2.Items.Add("55")
        ComboBox2.Items.Add("58")
        ComboBox2.Text = "Select 5.8GHz TX Power"

        ComboBox3.Items.Clear()
        ComboBox3.Items.Add("2412 MHz [1]")
        ComboBox3.Items.Add("2417 MHz [2]")
        ComboBox3.Items.Add("2422 MHz [3]")
        ComboBox3.Items.Add("2427 MHz [4]")
        ComboBox3.Items.Add("2432 MHz [5]")
        ComboBox3.Items.Add("2437 MHz [6]")
        ComboBox3.Items.Add("2442 MHz [7]")
        ComboBox3.Items.Add("2447 MHz [8]")
        ComboBox3.Items.Add("2452 MHz [9]")
        ComboBox3.Items.Add("2457 MHz [10]")
        ComboBox3.Items.Add("2462 MHz [11]")
        ComboBox3.Items.Add("2467 MHz [12]")
        ComboBox3.Items.Add("2472 MHz [13]")
        ComboBox3.Items.Add("2484 MHz [14]")
        ComboBox3.Text = "Select 2.4GHz Frequency"

        ComboBox4.Items.Clear()
        ComboBox4.Items.Add("20")
        ComboBox4.Items.Add("25")
        ComboBox4.Items.Add("30")
        ComboBox4.Items.Add("35")
        ComboBox4.Items.Add("40")
        ComboBox4.Items.Add("45")
        ComboBox4.Items.Add("50")
        ComboBox4.Items.Add("55")
        ComboBox4.Items.Add("58")
        ComboBox4.Text = "Select 2.4GHz TX Power"

        ComboBox5.Items.Clear()
        ComboBox5.Items.Add("0")
        ComboBox5.Items.Add("1")
        ComboBox5.Items.Add("2")
        ComboBox5.Items.Add("3")
        ComboBox5.Items.Add("4")
        ComboBox5.Items.Add("5")
        ComboBox5.Items.Add("6")
        ComboBox5.Items.Add("7")
        ComboBox5.Items.Add("8")
        ComboBox5.Items.Add("9")
        ComboBox5.Text = "Select MCS INDEX"

        ComboBox6.Items.Clear()
        ComboBox6.Items.Add("0")
        ComboBox6.Items.Add("1")
        ComboBox6.Text = "Select STBC"

        ComboBox7.Items.Clear()
        ComboBox7.Items.Add("0")
        ComboBox7.Items.Add("1")
        ComboBox7.Text = "Select LDPC"

        ComboBox8.Items.Clear()
        ComboBox8.Items.Add("1")
        ComboBox8.Items.Add("2")
        ComboBox8.Items.Add("3")
        ComboBox8.Items.Add("4")
        ComboBox8.Items.Add("5")
        ComboBox8.Items.Add("6")
        ComboBox8.Items.Add("7")
        ComboBox8.Items.Add("8")
        ComboBox8.Items.Add("9")
        ComboBox8.Items.Add("10")
        ComboBox8.Items.Add("11")
        ComboBox8.Items.Add("12")
        ComboBox8.Text = "Select FEC K"

        ComboBox9.Items.Clear()
        ComboBox9.Items.Add("1")
        ComboBox9.Items.Add("2")
        ComboBox9.Items.Add("3")
        ComboBox9.Items.Add("4")
        ComboBox9.Items.Add("5")
        ComboBox9.Items.Add("6")
        ComboBox9.Items.Add("7")
        ComboBox9.Items.Add("8")
        ComboBox9.Items.Add("9")
        ComboBox9.Items.Add("10")
        ComboBox9.Items.Add("11")
        ComboBox9.Items.Add("12")
        ComboBox9.Text = "Select FEC N"

        cmbResolution.Items.Clear()
        cmbResolution.Items.Add("1280x720")
        cmbResolution.Items.Add("1920x1080")
        cmbResolution.Items.Add("3200x1800")
        cmbResolution.Items.Add("3840x2160")
        cmbResolution.Text = "Select Resolution"

        cmbOSD.Items.Clear()
        cmbOSD.Items.Add("simple")
        cmbOSD.Items.Add("none")
        cmbOSD.Text = "Select OSD"

        cmbFormat.Items.Clear()
        cmbFormat.Items.Add("frame")
        cmbFormat.Items.Add("stream")
        cmbFormat.Text = "Select Format"

        cmbResolutionVRX.Items.Clear()
        cmbResolutionVRX.Items.Add("720p60")
        cmbResolutionVRX.Items.Add("1080p60")
        cmbResolutionVRX.Items.Add("1024x768x60")
        cmbResolutionVRX.Items.Add("1366x768x60")
        cmbResolutionVRX.Items.Add("1280x1024x60")
        cmbResolutionVRX.Items.Add("1600x1200x60")
        cmbResolutionVRX.Items.Add("2560x1440x30")
        cmbResolutionVRX.Text = "Select Resolution"

        cmbFPS.Items.Clear()
        cmbFPS.Items.Add("20")
        cmbFPS.Items.Add("30")
        cmbFPS.Items.Add("50")
        cmbFPS.Items.Add("60")
        cmbFPS.Items.Add("90")
        cmbFPS.Items.Add("100")
        cmbFPS.Items.Add("110")
        cmbFPS.Items.Add("120")
        cmbFPS.Text = "Select FPS"

        cmbCodec.Items.Clear()
        cmbCodec.Items.Add("h264")
        cmbCodec.Items.Add("h265")
        cmbCodec.Text = "Select Codec"

        cmbCodecVRX.Items.Clear()
        cmbCodecVRX.Items.Add("h264")
        cmbCodecVRX.Items.Add("h265")
        cmbCodecVRX.Text = "Select Codec"

        cmbBitrate.Items.Clear()
        cmbBitrate.Items.Add("1024")
        cmbBitrate.Items.Add("2048")
        cmbBitrate.Items.Add("3072")
        cmbBitrate.Items.Add("4096")
        cmbBitrate.Items.Add("5120")
        cmbBitrate.Items.Add("6144")
        cmbBitrate.Items.Add("7168")
        cmbBitrate.Items.Add("8192")
        cmbBitrate.Items.Add("9216")
        cmbBitrate.Items.Add("10240")
        cmbBitrate.Items.Add("11264")
        cmbBitrate.Items.Add("12288")
        cmbBitrate.Items.Add("13312")
        cmbBitrate.Text = "Select Bitrate"

        cmbExposure.Items.Clear()
        cmbExposure.Items.Add("1")
        cmbExposure.Items.Add("5")
        cmbExposure.Items.Add("8")
        cmbExposure.Items.Add("10")
        cmbExposure.Items.Add("11")
        cmbExposure.Items.Add("20")
        cmbExposure.Items.Add("30")
        cmbExposure.Items.Add("40")
        cmbExposure.Items.Add("50")
        cmbExposure.Items.Add("60")
        cmbExposure.Text = "Select Exposure"

        cmbContrast.Items.Clear()
        cmbContrast.Items.Add("1")
        cmbContrast.Items.Add("5")
        cmbContrast.Items.Add("10")
        cmbContrast.Items.Add("20")
        cmbContrast.Items.Add("30")
        cmbContrast.Items.Add("40")
        cmbContrast.Items.Add("50")
        cmbContrast.Items.Add("60")
        cmbContrast.Items.Add("70")
        cmbContrast.Items.Add("80")
        cmbContrast.Items.Add("90")
        cmbContrast.Items.Add("100")
        cmbContrast.Text = "Select Contrast"

        cmbHue.Items.Clear()
        cmbHue.Items.Add("1")
        cmbHue.Items.Add("5")
        cmbHue.Items.Add("10")
        cmbHue.Items.Add("20")
        cmbHue.Items.Add("30")
        cmbHue.Items.Add("40")
        cmbHue.Items.Add("50")
        cmbHue.Items.Add("60")
        cmbHue.Items.Add("70")
        cmbHue.Items.Add("80")
        cmbHue.Items.Add("90")
        cmbHue.Items.Add("100")
        cmbHue.Text = "Select Hue"

        cmbSaturation.Items.Clear()
        cmbSaturation.Items.Add("1")
        cmbSaturation.Items.Add("5")
        cmbSaturation.Items.Add("10")
        cmbSaturation.Items.Add("20")
        cmbSaturation.Items.Add("30")
        cmbSaturation.Items.Add("40")
        cmbSaturation.Items.Add("50")
        cmbSaturation.Items.Add("60")
        cmbSaturation.Items.Add("70")
        cmbSaturation.Items.Add("80")
        cmbSaturation.Items.Add("90")
        cmbSaturation.Items.Add("100")
        cmbSaturation.Text = "Select Saturation"

        cmbLuminance.Items.Clear()
        cmbLuminance.Items.Add("1")
        cmbLuminance.Items.Add("5")
        cmbLuminance.Items.Add("10")
        cmbLuminance.Items.Add("20")
        cmbLuminance.Items.Add("30")
        cmbLuminance.Items.Add("40")
        cmbLuminance.Items.Add("50")
        cmbLuminance.Items.Add("60")
        cmbLuminance.Items.Add("70")
        cmbLuminance.Items.Add("80")
        cmbLuminance.Items.Add("90")
        cmbLuminance.Items.Add("100")
        cmbLuminance.Text = "Select Luminance"

        cmbSerial.Items.Clear()
        cmbSerial.Items.Add("/dev/ttyS0")
        cmbSerial.Items.Add("/dev/ttyS1")
        cmbSerial.Items.Add("/dev/ttyS2")
        cmbSerial.Text = "Select Serial Port"

        cmbBaud.Items.Clear()
        cmbBaud.Items.Add("4800")
        cmbBaud.Items.Add("9600")
        cmbBaud.Items.Add("19200")
        cmbBaud.Items.Add("38400")
        cmbBaud.Items.Add("57600")
        cmbBaud.Items.Add("112500")
        cmbBaud.Text = "Select Baud Rate"

        cmbRouter.Items.Clear()
        cmbRouter.Items.Add("0")
        cmbRouter.Items.Add("1")
        cmbRouter.Text = "Select MAVFWD(0)/MAVLINK(1)"

        cmbMCSTLM.Items.Clear()
        cmbMCSTLM.Items.Add("0")
        cmbMCSTLM.Items.Add("1")
        cmbMCSTLM.Items.Add("2")
        cmbMCSTLM.Items.Add("3")
        cmbMCSTLM.Items.Add("4")
        cmbMCSTLM.Items.Add("5")
        cmbMCSTLM.Items.Add("6")
        cmbMCSTLM.Items.Add("7")
        cmbMCSTLM.Items.Add("8")
        cmbMCSTLM.Items.Add("9")
        cmbMCSTLM.Text = "Select MCS INDEX"

        rBtnCam.BackColor = Color.Gold
        rBtnCam.ForeColor = Color.Black
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim sInput = ComboBox1.SelectedItem.ToString
        Dim last4Letter = sInput.Substring(sInput.Length - 4).Replace("]", "").Replace("[", "")
        If rBtnRadxaZero3w.Checked Then
            txtFrequency.Text = "wifi_channel = " & last4Letter
        Else
            txtFrequency.Text = "channel=" & last4Letter
            txtFreq24.Text = "frequency="
        End If
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If rBtnRadxaZero3w.Checked Then
            txtPower.Text = "rtw_tx_pwr_idx_override=" & ComboBox2.SelectedItem.ToString
        Else
            txtPower.Text = "driver_txpower_override=" & ComboBox2.SelectedItem.ToString
        End If
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged
        Dim sInput = ComboBox3.SelectedItem.ToString
        Dim last3Letter = sInput.Substring(sInput.Length - 3).Replace("]", "").Replace("[", "")
        txtFrequency.Text = "channel="
        txtFreq24.Text = "frequency=" & last3Letter
    End Sub

    Private Sub ComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox4.SelectedIndexChanged
        txtPower24.Text = "txpower=" & ComboBox4.SelectedItem.ToString
    End Sub

    Private Sub ComboBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox5.SelectedIndexChanged
        txtMCS.Text = "mcs_index=" & ComboBox5.SelectedItem.ToString
    End Sub

    Private Sub ComboBox6_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox6.SelectedIndexChanged
        txtSTBC.Text = "stbc=" & ComboBox6.SelectedItem.ToString
    End Sub

    Private Sub ComboBox7_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox7.SelectedIndexChanged
        txtLDPC.Text = "ldpc=" & ComboBox7.SelectedItem.ToString
    End Sub

    Private Sub ComboBox8_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox8.SelectedIndexChanged
        txtFECK.Text = "fec_k=" & ComboBox8.SelectedItem.ToString
    End Sub

    Private Sub ComboBox9_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox9.SelectedIndexChanged
        txtFECN.Text = "fec_n=" & ComboBox9.SelectedItem.ToString
    End Sub

    Private Sub cmbResolution_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbResolution.SelectedIndexChanged
        txtResolution.Text = "  size: " & cmbResolution.SelectedItem.ToString
        If cmbResolution.SelectedItem = "1280x720" Then
            txtFPS.Text = "  fps: 120"
            cmbFPS.Text = "120"
            txtExposure.Text = "  exposure: 8"
            cmbExposure.Text = "8"
        ElseIf cmbResolution.SelectedItem = "1920x1080" Then
            txtFPS.Text = "  fps: 90"
            cmbFPS.Text = "90"
            txtExposure.Text = "  exposure: 11"
            cmbExposure.Text = "11"
        ElseIf cmbResolution.SelectedItem = "3200x1800" Then
            txtFPS.Text = "  fps: 30"
            cmbFPS.Text = "30"
            txtExposure.Text = "  exposure: 20"
            cmbExposure.Text = "20"
        ElseIf cmbResolution.SelectedItem = "3840x2160" Then
            txtFPS.Text = "  fps: 20"
            cmbFPS.Text = "20"
            txtExposure.Text = "  exposure: 20"
            cmbExposure.Text = "20"
        End If
    End Sub

    Private Sub cmbFPS_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFPS.SelectedIndexChanged
        txtFPS.Text = "  fps: " & cmbFPS.SelectedItem.ToString
        txtExposure.Text = "  exposure: " & Math.Floor(1000 / CInt(cmbFPS.SelectedItem.ToString))
    End Sub

    Private Sub cmbCodec_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCodec.SelectedIndexChanged
        txtEncode.Text = "  codec: " & cmbCodec.SelectedItem.ToString
    End Sub

    Private Sub cmbBitrate_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBitrate.SelectedIndexChanged
        txtBitrate.Text = "  bitrate: " & cmbBitrate.SelectedItem.ToString
    End Sub

    Private Sub cmbExposure_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbExposure.SelectedIndexChanged
        txtExposure.Text = "  exposure: " & cmbExposure.SelectedItem.ToString
    End Sub

    Private Sub cmbContrast_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbContrast.SelectedIndexChanged
        txtContrast.Text = "  contrast: " & cmbContrast.SelectedItem.ToString
    End Sub

    Private Sub cmbHue_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbHue.SelectedIndexChanged
        txtHue.Text = "  hue: " & cmbHue.SelectedItem.ToString
    End Sub

    Private Sub cmbSaturation_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSaturation.SelectedIndexChanged
        txtSaturation.Text = "  saturation: " & cmbSaturation.SelectedItem.ToString
    End Sub

    Private Sub cmbLuminance_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbLuminance.SelectedIndexChanged
        txtLuminance.Text = "  luminance: " & cmbLuminance.SelectedItem.ToString
    End Sub

    Function IsValidIP(ByVal ipAddress As String) As Boolean
        Return System.Text.RegularExpressions.Regex.IsMatch(ipAddress,
    "^(25[0-5]|2[0-4]\d|[0-1]?\d?\d)(\.(25[0-5]|2[0-4]\d|[0-1]?\d?\d)){3}$")
    End Function

    Private Sub btnReboot_Click(sender As Object, e As EventArgs) Handles btnReboot.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "rb " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub cmbSerial_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSerial.SelectedIndexChanged
        txtSerial.Text = "serial=" & cmbSerial.SelectedItem.ToString
    End Sub

    Private Sub cmbBaud_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBaud.SelectedIndexChanged
        txtBaud.Text = "baud=" & cmbBaud.SelectedItem.ToString
    End Sub

    Private Sub cmbRouter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbRouter.SelectedIndexChanged
        txtRouter.Text = "router=" & cmbRouter.SelectedItem.ToString
    End Sub

    Private Sub cmbMCSTLM_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbMCSTLM.SelectedIndexChanged
        txtMCSTLM.Text = "mcs_index=" & cmbMCSTLM.SelectedItem.ToString
    End Sub

    Private Sub txtSaveTLM_Click(sender As Object, e As EventArgs) Handles txtSaveTLM.Click
        If txtSerial.Text <> "" Then
            Dim telemetry = "telemetry.conf"
            If Not System.IO.File.Exists(telemetry) Then
                MsgBox("File " + telemetry + " not found!")
                Return
            End If
            Dim x As Integer
            Dim TLMfilePath As String = telemetry
            Dim lines = IO.File.ReadAllLines(TLMfilePath)
            For x = 0 To lines.Count() - 1
                If lines(x).StartsWith("serial=") Then
                    lines(x) = txtSerial.Text
                End If
                If lines(x).StartsWith("baud=") Then
                    lines(x) = txtBaud.Text
                End If
                If lines(x).StartsWith("router=") Then
                    lines(x) = txtRouter.Text
                End If
                If lines(x).StartsWith("mcs_index=") Then
                    lines(x) = txtMCSTLM.Text
                End If
            Next
            IO.File.WriteAllLines(TLMfilePath, lines)
            MsgBox("Settings saved successfully", MsgBoxStyle.Information, "OpenIPC")
        End If
    End Sub

    Private Sub txtSaveVRX_Click(sender As Object, e As EventArgs) Handles txtSaveVRX.Click
        If txtResolutionVRX.Text <> "" Then
            If rBtnRadxaZero3w.Checked Then
                Dim setdisplay = "setdisplay.sh"
                If Not System.IO.File.Exists(setdisplay) Then
                    MsgBox("File " + setdisplay + " not found!")
                    Return
                End If
                Dim x, y As Integer
                Dim setdisplayfilePath As String = setdisplay
                Dim setdisplaylines = IO.File.ReadAllLines(setdisplayfilePath)
                For x = 0 To setdisplaylines.Count() - 1
                    If setdisplaylines(x).StartsWith("MODE=") Then
                        setdisplaylines(x) = txtResolutionVRX.Text
                    End If
                    If setdisplaylines(x).StartsWith("RATE=") Then
                        setdisplaylines(x) = txtCodecVRX.Text
                    End If
                Next
                IO.File.WriteAllLines(setdisplayfilePath, setdisplaylines)
            Else
                Dim vdec = "vdec.conf"
                If Not System.IO.File.Exists(vdec) Then
                    MsgBox("File " + vdec + " not found!")
                    Return
                End If

                Dim VDECfilePath As String = vdec
                Dim lines = IO.File.ReadAllLines(VDECfilePath)
                For y = 0 To lines.Count() - 1
                    If lines(y).StartsWith("mode=") Then
                        lines(y) = txtResolutionVRX.Text
                    End If
                    If lines(y).StartsWith("codec=") Then
                        lines(y) = txtCodecVRX.Text
                    End If
                    If lines(y).StartsWith("format=") Then
                        lines(y) = txtFormat.Text
                    End If
                    If lines(y).StartsWith("port=") Then
                        lines(y) = txtPortVRX.Text
                    End If
                    If lines(y).StartsWith("mavlink_port=") Then
                        lines(y) = txtMavlinkVRX.Text
                    End If
                    If lines(y).StartsWith("osd=") Then
                        lines(y) = txtOSD.Text
                    End If
                    If lines(y).StartsWith("extra=") Then
                        lines(y) = txtExtras.Text
                    End If
                Next
                IO.File.WriteAllLines(VDECfilePath, lines)
            End If
            MsgBox("Settings saved successfully", MsgBoxStyle.Information, "OpenIPC")
        End If
    End Sub

    Private Sub cmbResolutionVRX_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbResolutionVRX.SelectedIndexChanged
        If rBtnRadxaZero3w.Checked Then
            txtResolutionVRX.Text = "MODE=" & cmbResolutionVRX.SelectedItem.ToString
        Else
            txtResolutionVRX.Text = "mode=" & cmbResolutionVRX.SelectedItem.ToString
        End If
    End Sub

    Private Sub cmbCodecVRX_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCodecVRX.SelectedIndexChanged
        If rBtnRadxaZero3w.Checked Then
            txtCodecVRX.Text = "RATE=" & cmbCodecVRX.SelectedItem.ToString
        Else
            txtCodecVRX.Text = "codec=" & cmbCodecVRX.SelectedItem.ToString
        End If
    End Sub

    Private Sub cmbOSD_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbOSD.SelectedIndexChanged
        txtOSD.Text = "osd=" & cmbOSD.SelectedItem.ToString
    End Sub

    Private Sub cmbFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFormat.SelectedIndexChanged
        txtFormat.Text = "format=" & cmbFormat.SelectedItem.ToString
    End Sub

    Private Sub btnGenerateKeys_Click(sender As Object, e As EventArgs) Handles btnGenerateKeys.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "keysgen " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnReceiveKeys_Click(sender As Object, e As EventArgs) Handles btnReceiveKeys.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "keysdl " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnSendKeys_Click_1(sender As Object, e As EventArgs) Handles btnSendKeys.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "keysul " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnUpdate_Click_1(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "sysup " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnRestartWFB_Click(sender As Object, e As EventArgs) Handles btnRestartWFB.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "rswfb " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnRestartMajestic_Click(sender As Object, e As EventArgs) Handles btnRestartMajestic.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "rsmaj " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub rBtnNVR_CheckedChanged(sender As Object, e As EventArgs) Handles rBtnNVR.CheckedChanged
        Dim rb = DirectCast(sender, RadioButton)

        If rb.Checked Then
            rb.BackColor = Color.Gold
            rb.ForeColor = Color.Black
        Else
            rb.BackColor = Color.FromArgb(45, 45, 45)
            rb.ForeColor = Color.White
        End If
        btnSendKeys.Visible = False
        btnGenerateKeys.Visible = True
        btnUpdate.Visible = False
        txtSaveVRX.Visible = True
        btnUART2.Visible = False
        btnUART2OFF.Visible = False
        btnRestartWFB.Visible = True
        btnRestartMajestic.Visible = False
        txtSaveCam.Visible = False
        txtSaveTLM.Visible = False
        ComboBox3.Visible = True
        ComboBox4.Visible = True
        ComboBox5.Visible = False
        ComboBox6.Visible = False
        ComboBox7.Visible = False
        ComboBox8.Visible = False
        ComboBox9.Visible = False
        cmbOSD.Visible = True
        cmbFormat.Visible = True
        txtLDPC.Visible = False
        txtFECK.Visible = False
        txtFECN.Visible = False
        txtOSD.Visible = True
        txtFormat.Visible = True
        txtFreq24.Visible = True
        txtPower24.Visible = True
        txtPortVRX.Visible = True
        txtMavlinkVRX.Visible = True
        txtExtras.Visible = True
        Label2.Visible = True
        txtMCS.ReadOnly = False
        txtSTBC.ReadOnly = False
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("5180 MHz [36]")
        ComboBox1.Items.Add("5200 MHz [40]")
        ComboBox1.Items.Add("5220 MHz [44]")
        ComboBox1.Items.Add("5240 MHz [48]")
        ComboBox1.Items.Add("5260 MHz [52]")
        ComboBox1.Items.Add("5280 MHz [56]")
        ComboBox1.Items.Add("5300 MHz [60]")
        ComboBox1.Items.Add("5320 MHz [64]")
        ComboBox1.Items.Add("5500 MHz [100]")
        ComboBox1.Items.Add("5520 MHz [104]")
        ComboBox1.Items.Add("5540 MHz [108]")
        ComboBox1.Items.Add("5560 MHz [112]")
        ComboBox1.Items.Add("5580 MHz [116]")
        ComboBox1.Items.Add("5600 MHz [120]")
        ComboBox1.Items.Add("5620 MHz [124]")
        ComboBox1.Items.Add("5640 MHz [128]")
        ComboBox1.Items.Add("5660 MHz [132]")
        ComboBox1.Items.Add("5680 MHz [136]")
        ComboBox1.Items.Add("5700 MHz [140]")
        ComboBox1.Items.Add("5720 MHz [144]")
        ComboBox1.Items.Add("5745 MHz [149]")
        ComboBox1.Items.Add("5765 MHz [153]")
        ComboBox1.Items.Add("5785 MHz [157]")
        ComboBox1.Items.Add("5805 MHz [161]")
        ComboBox1.Items.Add("5825 MHz [165]")
        ComboBox1.Items.Add("5845 MHz [169]")
        ComboBox1.Items.Add("5865 MHz [173]")
        ComboBox1.Items.Add("5885 MHz [177]")
        ComboBox1.Text = "Select 5.8GHz Frequency"
        cmbResolutionVRX.Items.Clear()
        cmbResolutionVRX.Items.Add("720p60")
        cmbResolutionVRX.Items.Add("1080p60")
        cmbResolutionVRX.Items.Add("1024x768x60")
        cmbResolutionVRX.Items.Add("1366x768x60")
        cmbResolutionVRX.Items.Add("1280x1024x60")
        cmbResolutionVRX.Items.Add("1600x1200x60")
        cmbResolutionVRX.Items.Add("2560x1440x30")
        cmbResolutionVRX.Text = "Select Resolution"
        cmbCodecVRX.Items.Clear()
        cmbCodecVRX.Items.Add("h264")
        cmbCodecVRX.Items.Add("h265")
        cmbCodecVRX.Text = "Select Codec"
        txtPassword.Text = "12345"
    End Sub

    Private Sub rBtnCam_CheckedChanged(sender As Object, e As EventArgs) Handles rBtnCam.CheckedChanged
        Dim rb = DirectCast(sender, RadioButton)

        If rb.Checked Then
            rb.BackColor = Color.Gold
            rb.ForeColor = Color.Black
        Else
            rb.BackColor = Color.FromArgb(45, 45, 45)
            rb.ForeColor = Color.White
        End If
        btnSendKeys.Visible = True
        btnGenerateKeys.Visible = False
        btnUpdate.Visible = True
        txtSaveVRX.Visible = False
        btnUART2.Visible = True
        btnUART2OFF.Visible = True
        btnRestartWFB.Visible = True
        btnRestartMajestic.Visible = True
        txtSaveCam.Visible = True
        txtSaveTLM.Visible = True
        ComboBox3.Visible = True
        ComboBox4.Visible = True
        ComboBox5.Visible = True
        ComboBox6.Visible = True
        ComboBox7.Visible = True
        ComboBox8.Visible = True
        ComboBox9.Visible = True
        cmbOSD.Visible = True
        cmbFormat.Visible = True
        txtLDPC.Visible = True
        txtFECK.Visible = True
        txtFECN.Visible = True
        txtOSD.Visible = True
        txtFormat.Visible = True
        txtFreq24.Visible = True
        txtPower24.Visible = True
        txtPortVRX.Visible = True
        txtMavlinkVRX.Visible = True
        txtExtras.Visible = True
        Label2.Visible = True
        txtMCS.ReadOnly = True
        txtSTBC.ReadOnly = True
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("5180 MHz [36]")
        ComboBox1.Items.Add("5200 MHz [40]")
        ComboBox1.Items.Add("5220 MHz [44]")
        ComboBox1.Items.Add("5240 MHz [48]")
        ComboBox1.Items.Add("5260 MHz [52]")
        ComboBox1.Items.Add("5280 MHz [56]")
        ComboBox1.Items.Add("5300 MHz [60]")
        ComboBox1.Items.Add("5320 MHz [64]")
        ComboBox1.Items.Add("5500 MHz [100]")
        ComboBox1.Items.Add("5520 MHz [104]")
        ComboBox1.Items.Add("5540 MHz [108]")
        ComboBox1.Items.Add("5560 MHz [112]")
        ComboBox1.Items.Add("5580 MHz [116]")
        ComboBox1.Items.Add("5600 MHz [120]")
        ComboBox1.Items.Add("5620 MHz [124]")
        ComboBox1.Items.Add("5640 MHz [128]")
        ComboBox1.Items.Add("5660 MHz [132]")
        ComboBox1.Items.Add("5680 MHz [136]")
        ComboBox1.Items.Add("5700 MHz [140]")
        ComboBox1.Items.Add("5720 MHz [144]")
        ComboBox1.Items.Add("5745 MHz [149]")
        ComboBox1.Items.Add("5765 MHz [153]")
        ComboBox1.Items.Add("5785 MHz [157]")
        ComboBox1.Items.Add("5805 MHz [161]")
        ComboBox1.Items.Add("5825 MHz [165]")
        ComboBox1.Items.Add("5845 MHz [169]")
        ComboBox1.Items.Add("5865 MHz [173]")
        ComboBox1.Items.Add("5885 MHz [177]")
        ComboBox1.Text = "Select 5.8GHz Frequency"
        cmbResolutionVRX.Items.Clear()
        cmbResolutionVRX.Items.Add("720p60")
        cmbResolutionVRX.Items.Add("1080p60")
        cmbResolutionVRX.Items.Add("1024x768x60")
        cmbResolutionVRX.Items.Add("1366x768x60")
        cmbResolutionVRX.Items.Add("1280x1024x60")
        cmbResolutionVRX.Items.Add("1600x1200x60")
        cmbResolutionVRX.Items.Add("2560x1440x30")
        cmbResolutionVRX.Text = "Select Resolution"
        cmbCodecVRX.Items.Clear()
        cmbCodecVRX.Items.Add("h264")
        cmbCodecVRX.Items.Add("h265")
        cmbCodecVRX.Text = "Select Codec"
        txtPassword.Text = "12345"
    End Sub

    Private Sub rBtnRadxaZero3w_CheckedChanged(sender As Object, e As EventArgs) Handles rBtnRadxaZero3w.CheckedChanged
        Dim rb = DirectCast(sender, RadioButton)

        If rb.Checked Then
            rb.BackColor = Color.Gold
            rb.ForeColor = Color.Black
        Else
            rb.BackColor = Color.FromArgb(45, 45, 45)
            rb.ForeColor = Color.White
        End If
        btnSendKeys.Visible = False
        btnGenerateKeys.Visible = True
        btnUpdate.Visible = False
        txtSaveVRX.Visible = True
        btnUART2.Visible = False
        btnUART2OFF.Visible = False
        btnRestartWFB.Visible = False
        btnRestartMajestic.Visible = False
        txtSaveCam.Visible = False
        txtSaveTLM.Visible = False
        ComboBox3.Visible = False
        ComboBox4.Visible = False
        ComboBox5.Visible = False
        ComboBox6.Visible = False
        ComboBox7.Visible = False
        ComboBox8.Visible = False
        ComboBox9.Visible = False
        cmbOSD.Visible = False
        cmbFormat.Visible = False
        txtLDPC.Visible = False
        txtFECK.Visible = False
        txtFECN.Visible = False
        txtOSD.Visible = False
        txtFormat.Visible = False
        txtFreq24.Visible = False
        txtPower24.Visible = False
        txtPortVRX.Visible = False
        txtMavlinkVRX.Visible = False
        txtExtras.Visible = False
        Label2.Visible = False
        txtMCS.ReadOnly = False
        txtSTBC.ReadOnly = False
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("2412 MHz [1]")
        ComboBox1.Items.Add("2417 MHz [2]")
        ComboBox1.Items.Add("2422 MHz [3]")
        ComboBox1.Items.Add("2427 MHz [4]")
        ComboBox1.Items.Add("2432 MHz [5]")
        ComboBox1.Items.Add("2437 MHz [6]")
        ComboBox1.Items.Add("2442 MHz [7]")
        ComboBox1.Items.Add("2447 MHz [8]")
        ComboBox1.Items.Add("2452 MHz [9]")
        ComboBox1.Items.Add("2457 MHz [10]")
        ComboBox1.Items.Add("2462 MHz [11]")
        ComboBox1.Items.Add("2467 MHz [12]")
        ComboBox1.Items.Add("2472 MHz [13]")
        ComboBox1.Items.Add("2484 MHz [14]")
        ComboBox1.Items.Add("5180 MHz [36]")
        ComboBox1.Items.Add("5200 MHz [40]")
        ComboBox1.Items.Add("5220 MHz [44]")
        ComboBox1.Items.Add("5240 MHz [48]")
        ComboBox1.Items.Add("5260 MHz [52]")
        ComboBox1.Items.Add("5280 MHz [56]")
        ComboBox1.Items.Add("5300 MHz [60]")
        ComboBox1.Items.Add("5320 MHz [64]")
        ComboBox1.Items.Add("5500 MHz [100]")
        ComboBox1.Items.Add("5520 MHz [104]")
        ComboBox1.Items.Add("5540 MHz [108]")
        ComboBox1.Items.Add("5560 MHz [112]")
        ComboBox1.Items.Add("5580 MHz [116]")
        ComboBox1.Items.Add("5600 MHz [120]")
        ComboBox1.Items.Add("5620 MHz [124]")
        ComboBox1.Items.Add("5640 MHz [128]")
        ComboBox1.Items.Add("5660 MHz [132]")
        ComboBox1.Items.Add("5680 MHz [136]")
        ComboBox1.Items.Add("5700 MHz [140]")
        ComboBox1.Items.Add("5720 MHz [144]")
        ComboBox1.Items.Add("5745 MHz [149]")
        ComboBox1.Items.Add("5765 MHz [153]")
        ComboBox1.Items.Add("5785 MHz [157]")
        ComboBox1.Items.Add("5805 MHz [161]")
        ComboBox1.Items.Add("5825 MHz [165]")
        ComboBox1.Items.Add("5845 MHz [169]")
        ComboBox1.Items.Add("5865 MHz [173]")
        ComboBox1.Items.Add("5885 MHz [177]")
        ComboBox1.Text = "Select 5.8GHz Frequency"
        cmbResolutionVRX.Items.Clear()
        cmbResolutionVRX.Items.Add("1280x720")
        cmbResolutionVRX.Items.Add("1920x1080")
        cmbResolutionVRX.Text = "Select Resolution"
        cmbCodecVRX.Items.Clear()
        cmbCodecVRX.Items.Add("20")
        cmbCodecVRX.Items.Add("30")
        cmbCodecVRX.Items.Add("50")
        cmbCodecVRX.Items.Add("60")
        cmbCodecVRX.Items.Add("90")
        cmbCodecVRX.Items.Add("100")
        cmbCodecVRX.Items.Add("110")
        cmbCodecVRX.Items.Add("120")
        cmbCodecVRX.Text = "Select FPS"
        txtPassword.Text = "root"
    End Sub

    Public Class TabControl
        Inherits System.Windows.Forms.TabControl

#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Windows Form Designer.
            InitializeComponent()

            'Add any initialization after the InitializeComponent() call
            SetStyle(ControlStyles.AllPaintingInWmPaint Or
                ControlStyles.DoubleBuffer Or
                ControlStyles.ResizeRedraw Or
                ControlStyles.UserPaint, True)

        End Sub

        'UserControl1 overrides dispose to clean up the component list.
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            components = New System.ComponentModel.Container
        End Sub

#End Region

#Region " InterOP "

        <StructLayout(LayoutKind.Sequential)>
        Private Structure NMHDR
            Public HWND As Int32
            Public idFrom As Int32
            Public code As Int32
            Public Overloads Function ToString() As String
                Return String.Format("Hwnd: {0}, ControlID: {1}, Code: {2}", HWND, idFrom, code)
            End Function
        End Structure

        Private Const TCN_FIRST As Int32 = &HFFFFFFFFFFFFFDDA&
        Private Const TCN_SELCHANGING As Int32 = (TCN_FIRST - 2)

        Private Const WM_USER As Int32 = &H400&
        Private Const WM_NOTIFY As Int32 = &H4E&
        Private Const WM_REFLECT As Int32 = WM_USER + &H1C00&

#End Region

#Region " BackColor Manipulation "

        'As well as exposing the property to the Designer we want it to behave just like any other 
        'controls BackColor property so we need some clever manipulation.
        Private m_Backcolor As Color = Color.Empty
        <Browsable(True),
    Description("The background color used to display text and graphics in a control.")>
        Public Overrides Property BackColor() As Color
            Get
                If m_Backcolor.Equals(Color.Empty) Then
                    If Parent Is Nothing Then
                        Return Control.DefaultBackColor
                    Else
                        Return Parent.BackColor
                    End If
                End If
                Return m_Backcolor
            End Get
            Set(ByVal Value As Color)
                If m_Backcolor.Equals(Value) Then Return
                m_Backcolor = Value
                Invalidate()
                'Let the Tabpages know that the backcolor has changed.
                MyBase.OnBackColorChanged(EventArgs.Empty)
            End Set
        End Property
        Public Function ShouldSerializeBackColor() As Boolean
            Return Not m_Backcolor.Equals(Color.Empty)
        End Function
        Public Overrides Sub ResetBackColor()
            m_Backcolor = Color.Empty
            Invalidate()
        End Sub

#End Region

        Protected Overrides Sub OnParentBackColorChanged(ByVal e As System.EventArgs)
            MyBase.OnParentBackColorChanged(e)
            Invalidate()
        End Sub

        Protected Overrides Sub OnSelectedIndexChanged(ByVal e As System.EventArgs)
            MyBase.OnSelectedIndexChanged(e)
            Invalidate()
        End Sub

        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)
            e.Graphics.Clear(BackColor)
            Dim r As Rectangle = Me.ClientRectangle
            If TabCount <= 0 Then Return
            'Draw a custom background for Transparent TabPages
            r = SelectedTab.Bounds
            Dim sf As New StringFormat
            sf.Alignment = StringAlignment.Center
            sf.LineAlignment = StringAlignment.Center
            Dim DrawFont As New Font(Font.FontFamily, 24, FontStyle.Regular, GraphicsUnit.Pixel)
            ControlPaint.DrawStringDisabled(e.Graphics, "Micks Ownerdraw TabControl", DrawFont, BackColor, RectangleF.op_Implicit(r), sf)
            DrawFont.Dispose()
            'Draw a border around TabPage
            r.Inflate(3, 3)
            Dim tp As TabPage = TabPages(SelectedIndex)
            Dim PaintBrush As New SolidBrush(tp.BackColor)
            e.Graphics.FillRectangle(PaintBrush, r)
            ControlPaint.DrawBorder(e.Graphics, r, PaintBrush.Color, ButtonBorderStyle.Outset)
            'Draw the Tabs
            For index As Integer = 0 To TabCount - 1
                tp = TabPages(index)
                r = GetTabRect(index)
                Dim bs As ButtonBorderStyle = ButtonBorderStyle.Outset
                If index = SelectedIndex Then bs = ButtonBorderStyle.Inset
                PaintBrush.Color = Color.Gold
                e.Graphics.FillRectangle(PaintBrush, r)
                ControlPaint.DrawBorder(e.Graphics, r, PaintBrush.Color, bs)
                PaintBrush.Color = Color.Black

                'Set up rotation for left and right aligned tabs
                If Alignment = TabAlignment.Left Or Alignment = TabAlignment.Right Then
                    Dim RotateAngle As Single = 90
                    If Alignment = TabAlignment.Left Then RotateAngle = 270
                    Dim cp As New PointF(r.Left + (r.Width \ 2), r.Top + (r.Height \ 2))
                    e.Graphics.TranslateTransform(cp.X, cp.Y)
                    e.Graphics.RotateTransform(RotateAngle)
                    r = New Rectangle(-(r.Height \ 2), -(r.Width \ 2), r.Height, r.Width)
                End If
                'Draw the Tab Text
                If tp.Enabled Then
                    e.Graphics.DrawString(tp.Text, Font, PaintBrush, RectangleF.op_Implicit(r), sf)
                Else
                    ControlPaint.DrawStringDisabled(e.Graphics, tp.Text, Font, tp.BackColor, RectangleF.op_Implicit(r), sf)
                End If

                e.Graphics.ResetTransform()

            Next
            PaintBrush.Dispose()
        End Sub

        <Description("Occurs as a tab is being changed.")>
        Public Event SelectedIndexChanging As SelectedTabPageChangeEventHandler

        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            If m.Msg = (WM_REFLECT + WM_NOTIFY) Then
                Dim hdr As NMHDR = DirectCast(Marshal.PtrToStructure(m.LParam, GetType(NMHDR)), NMHDR)
                If hdr.code = TCN_SELCHANGING Then
                    Dim tp As TabPage = TestTab(Me.PointToClient(Cursor.Position))
                    If Not tp Is Nothing Then
                        Dim e As New TabPageChangeEventArgs(Me.SelectedTab, tp)
                        RaiseEvent SelectedIndexChanging(Me, e)
                        If e.Cancel OrElse tp.Enabled = False Then
                            m.Result = New IntPtr(1)
                            Return
                        End If
                    End If
                End If
            End If
            MyBase.WndProc(m)
        End Sub

        Private Function TestTab(ByVal pt As Point) As TabPage
            For index As Integer = 0 To TabCount - 1
                If GetTabRect(index).Contains(pt.X, pt.Y) Then
                    Return TabPages(index)
                End If
            Next
            Return Nothing
        End Function

    End Class

#Region " EventArgs Class's "

    Public Class TabPageChangeEventArgs
        Inherits EventArgs

        Private _Selected As TabPage
        Private _PreSelected As TabPage
        Public Cancel As Boolean = False

        Public ReadOnly Property CurrentTab() As TabPage
            Get
                Return _Selected
            End Get
        End Property

        Public ReadOnly Property NextTab() As TabPage
            Get
                Return _PreSelected
            End Get
        End Property

        Public Sub New(ByVal CurrentTab As TabPage, ByVal NextTab As TabPage)
            _Selected = CurrentTab
            _PreSelected = NextTab
        End Sub

    End Class

    Public Delegate Sub SelectedTabPageChangeEventHandler(ByVal sender As Object, ByVal e As TabPageChangeEventArgs)

    Private Sub btnUART2_Click_1(sender As Object, e As EventArgs) Handles btnUART2.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "UART2 " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnUART2OFF_Click_1(sender As Object, e As EventArgs) Handles btnUART2OFF.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "UART0 " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

#End Region
End Class
