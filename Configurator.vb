Imports System.Runtime.CompilerServices

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
                .StartInfo.Arguments = "dl " + String.Format("{0}", txtIP.Text)
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
                .StartInfo.Arguments = "ul " + String.Format("{0}", txtIP.Text)
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
        Dim wfbconf = "wfb.conf"
        If Not System.IO.File.Exists(wfbconf) Then
            MsgBox("File " + wfbconf + " not found!")
            Return
        End If

        Dim WFBfilePath As String = wfbconf
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
        Dim majestic = "majestic.yaml"
        If Not System.IO.File.Exists(majestic) Then
            MsgBox("File " + majestic + " not found!")
            Return
        End If

        Dim CamfilePath As String = majestic
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
        Dim wfbconf = "wfb.conf"
        If Not System.IO.File.Exists(wfbconf) Then
            MsgBox("File " + wfbconf + " not found!")
            Return
        End If
	
        Dim majestic = "majestic.yaml"
        If Not System.IO.File.Exists(majestic) Then
            MsgBox("File " + majestic + " not found!")
            Return
        End If

        Dim WFBreader As New IO.StreamReader(wfbconf)
        Dim Camreader As New IO.StreamReader(majestic)
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
        cmbExposure.Items.Add("10")
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
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim sInput = ComboBox1.SelectedItem.ToString
        Dim last4Letter = sInput.Substring(sInput.Length - 4).Replace("]", "").Replace("[", "")
        txtFrequency.Text = "channel=" & last4Letter
        txtFreq24.Text = "frequency="
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        txtPower.Text = "driver_txpower_override=" & ComboBox2.SelectedItem.ToString
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
        txtMCS.Text = "msc_index=" & ComboBox5.SelectedItem.ToString
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
    End Sub

    Private Sub cmbFPS_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFPS.SelectedIndexChanged
        txtFPS.Text = "  fps: " & cmbFPS.SelectedItem.ToString
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
End Class
