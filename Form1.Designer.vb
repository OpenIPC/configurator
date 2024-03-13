<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        btnGet = New Button()
        txtIP = New TextBox()
        btnSend = New Button()
        txtFrequency = New TextBox()
        txtResolution = New TextBox()
        txtFPS = New TextBox()
        txtEncode = New TextBox()
        txtBitrate = New TextBox()
        txtExposure = New TextBox()
        txtContrast = New TextBox()
        txtHue = New TextBox()
        txtSaturation = New TextBox()
        txtLuminance = New TextBox()
        txtPower = New TextBox()
        txtSensor = New TextBox()
        txtFreq24 = New TextBox()
        txtMCS = New TextBox()
        txtLDPC = New TextBox()
        txtSTBC = New TextBox()
        txtFECN = New TextBox()
        txtFECK = New TextBox()
        txtPower24 = New TextBox()
        TabControl1 = New TabControl()
        TabPage1 = New TabPage()
        txtSaveFreq = New Button()
        TabPage2 = New TabPage()
        txtSaveCam = New Button()
        btnRead = New Button()
        TabControl1.SuspendLayout()
        TabPage1.SuspendLayout()
        TabPage2.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnGet
        ' 
        btnGet.Location = New Point(12, 408)
        btnGet.Name = "btnGet"
        btnGet.Size = New Size(60, 30)
        btnGet.TabIndex = 0
        btnGet.Text = "Fetch"
        btnGet.UseVisualStyleBackColor = True
        ' 
        ' txtIP
        ' 
        txtIP.Location = New Point(12, 379)
        txtIP.Name = "txtIP"
        txtIP.Size = New Size(124, 23)
        txtIP.TabIndex = 1
        txtIP.Text = "192.168.0.4"
        ' 
        ' btnSend
        ' 
        btnSend.Location = New Point(142, 408)
        btnSend.Name = "btnSend"
        btnSend.Size = New Size(58, 30)
        btnSend.TabIndex = 2
        btnSend.Text = "Upload"
        btnSend.UseVisualStyleBackColor = True
        ' 
        ' txtFrequency
        ' 
        txtFrequency.Location = New Point(21, 21)
        txtFrequency.Name = "txtFrequency"
        txtFrequency.Size = New Size(191, 23)
        txtFrequency.TabIndex = 3
        ' 
        ' txtResolution
        ' 
        txtResolution.Location = New Point(21, 15)
        txtResolution.Name = "txtResolution"
        txtResolution.Size = New Size(161, 23)
        txtResolution.TabIndex = 4
        ' 
        ' txtFPS
        ' 
        txtFPS.Location = New Point(21, 44)
        txtFPS.Name = "txtFPS"
        txtFPS.Size = New Size(161, 23)
        txtFPS.TabIndex = 5
        ' 
        ' txtEncode
        ' 
        txtEncode.Location = New Point(21, 73)
        txtEncode.Name = "txtEncode"
        txtEncode.Size = New Size(161, 23)
        txtEncode.TabIndex = 6
        ' 
        ' txtBitrate
        ' 
        txtBitrate.Location = New Point(21, 102)
        txtBitrate.Name = "txtBitrate"
        txtBitrate.Size = New Size(161, 23)
        txtBitrate.TabIndex = 7
        ' 
        ' txtExposure
        ' 
        txtExposure.Location = New Point(21, 131)
        txtExposure.Name = "txtExposure"
        txtExposure.Size = New Size(161, 23)
        txtExposure.TabIndex = 8
        ' 
        ' txtContrast
        ' 
        txtContrast.Location = New Point(21, 160)
        txtContrast.Name = "txtContrast"
        txtContrast.Size = New Size(161, 23)
        txtContrast.TabIndex = 9
        ' 
        ' txtHue
        ' 
        txtHue.Location = New Point(21, 189)
        txtHue.Name = "txtHue"
        txtHue.Size = New Size(161, 23)
        txtHue.TabIndex = 10
        ' 
        ' txtSaturation
        ' 
        txtSaturation.Location = New Point(21, 218)
        txtSaturation.Name = "txtSaturation"
        txtSaturation.Size = New Size(161, 23)
        txtSaturation.TabIndex = 11
        ' 
        ' txtLuminance
        ' 
        txtLuminance.Location = New Point(21, 247)
        txtLuminance.Name = "txtLuminance"
        txtLuminance.Size = New Size(161, 23)
        txtLuminance.TabIndex = 12
        ' 
        ' txtPower
        ' 
        txtPower.Location = New Point(21, 50)
        txtPower.Name = "txtPower"
        txtPower.Size = New Size(191, 23)
        txtPower.TabIndex = 13
        ' 
        ' txtSensor
        ' 
        txtSensor.Location = New Point(21, 276)
        txtSensor.Name = "txtSensor"
        txtSensor.Size = New Size(266, 23)
        txtSensor.TabIndex = 14
        ' 
        ' txtFreq24
        ' 
        txtFreq24.Location = New Point(21, 79)
        txtFreq24.Name = "txtFreq24"
        txtFreq24.Size = New Size(191, 23)
        txtFreq24.TabIndex = 16
        ' 
        ' txtMCS
        ' 
        txtMCS.Location = New Point(21, 137)
        txtMCS.Name = "txtMCS"
        txtMCS.Size = New Size(191, 23)
        txtMCS.TabIndex = 15
        ' 
        ' txtLDPC
        ' 
        txtLDPC.Location = New Point(21, 195)
        txtLDPC.Name = "txtLDPC"
        txtLDPC.Size = New Size(191, 23)
        txtLDPC.TabIndex = 18
        ' 
        ' txtSTBC
        ' 
        txtSTBC.Location = New Point(21, 166)
        txtSTBC.Name = "txtSTBC"
        txtSTBC.Size = New Size(191, 23)
        txtSTBC.TabIndex = 17
        ' 
        ' txtFECN
        ' 
        txtFECN.Location = New Point(21, 253)
        txtFECN.Name = "txtFECN"
        txtFECN.Size = New Size(191, 23)
        txtFECN.TabIndex = 20
        ' 
        ' txtFECK
        ' 
        txtFECK.Location = New Point(21, 224)
        txtFECK.Name = "txtFECK"
        txtFECK.Size = New Size(191, 23)
        txtFECK.TabIndex = 19
        ' 
        ' txtPower24
        ' 
        txtPower24.Location = New Point(21, 108)
        txtPower24.Name = "txtPower24"
        txtPower24.Size = New Size(191, 23)
        txtPower24.TabIndex = 21
        ' 
        ' TabControl1
        ' 
        TabControl1.Controls.Add(TabPage1)
        TabControl1.Controls.Add(TabPage2)
        TabControl1.Location = New Point(12, 12)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(348, 367)
        TabControl1.TabIndex = 22
        ' 
        ' TabPage1
        ' 
        TabPage1.BackColor = Color.WhiteSmoke
        TabPage1.Controls.Add(txtSaveFreq)
        TabPage1.Controls.Add(txtFrequency)
        TabPage1.Controls.Add(txtPower24)
        TabPage1.Controls.Add(txtPower)
        TabPage1.Controls.Add(txtFECN)
        TabPage1.Controls.Add(txtMCS)
        TabPage1.Controls.Add(txtFECK)
        TabPage1.Controls.Add(txtFreq24)
        TabPage1.Controls.Add(txtLDPC)
        TabPage1.Controls.Add(txtSTBC)
        TabPage1.Location = New Point(4, 24)
        TabPage1.Name = "TabPage1"
        TabPage1.Padding = New Padding(3)
        TabPage1.Size = New Size(340, 339)
        TabPage1.TabIndex = 0
        TabPage1.Text = "WFB Settings"
        ' 
        ' txtSaveFreq
        ' 
        txtSaveFreq.Location = New Point(140, 303)
        txtSaveFreq.Name = "txtSaveFreq"
        txtSaveFreq.Size = New Size(58, 30)
        txtSaveFreq.TabIndex = 25
        txtSaveFreq.Text = "Save"
        txtSaveFreq.UseVisualStyleBackColor = True
        ' 
        ' TabPage2
        ' 
        TabPage2.BackColor = Color.WhiteSmoke
        TabPage2.Controls.Add(txtSaveCam)
        TabPage2.Controls.Add(txtResolution)
        TabPage2.Controls.Add(txtSensor)
        TabPage2.Controls.Add(txtFPS)
        TabPage2.Controls.Add(txtLuminance)
        TabPage2.Controls.Add(txtEncode)
        TabPage2.Controls.Add(txtSaturation)
        TabPage2.Controls.Add(txtBitrate)
        TabPage2.Controls.Add(txtHue)
        TabPage2.Controls.Add(txtExposure)
        TabPage2.Controls.Add(txtContrast)
        TabPage2.Location = New Point(4, 24)
        TabPage2.Name = "TabPage2"
        TabPage2.Padding = New Padding(3)
        TabPage2.Size = New Size(340, 339)
        TabPage2.TabIndex = 1
        TabPage2.Text = "Camera Settings"
        ' 
        ' txtSaveCam
        ' 
        txtSaveCam.Location = New Point(140, 303)
        txtSaveCam.Name = "txtSaveCam"
        txtSaveCam.Size = New Size(58, 30)
        txtSaveCam.TabIndex = 26
        txtSaveCam.Text = "Save"
        txtSaveCam.UseVisualStyleBackColor = True
        ' 
        ' btnRead
        ' 
        btnRead.Location = New Point(76, 408)
        btnRead.Name = "btnRead"
        btnRead.Size = New Size(60, 30)
        btnRead.TabIndex = 23
        btnRead.Text = "Read"
        btnRead.UseVisualStyleBackColor = True
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(377, 450)
        Controls.Add(btnRead)
        Controls.Add(TabControl1)
        Controls.Add(btnSend)
        Controls.Add(txtIP)
        Controls.Add(btnGet)
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "OpenIPC Configurator"
        TabControl1.ResumeLayout(False)
        TabPage1.ResumeLayout(False)
        TabPage1.PerformLayout()
        TabPage2.ResumeLayout(False)
        TabPage2.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnGet As Button
    Friend WithEvents txtIP As TextBox
    Friend WithEvents btnSend As Button
    Friend WithEvents txtFrequency As TextBox
    Friend WithEvents txtResolution As TextBox
    Friend WithEvents txtFPS As TextBox
    Friend WithEvents txtEncode As TextBox
    Friend WithEvents txtBitrate As TextBox
    Friend WithEvents txtExposure As TextBox
    Friend WithEvents txtContrast As TextBox
    Friend WithEvents txtHue As TextBox
    Friend WithEvents txtSaturation As TextBox
    Friend WithEvents txtLuminance As TextBox
    Friend WithEvents txtPower As TextBox
    Friend WithEvents txtSensor As TextBox
    Friend WithEvents txtFreq24 As TextBox
    Friend WithEvents txtMCS As TextBox
    Friend WithEvents txtLDPC As TextBox
    Friend WithEvents txtSTBC As TextBox
    Friend WithEvents txtFECN As TextBox
    Friend WithEvents txtFECK As TextBox
    Friend WithEvents txtPower24 As TextBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents txtSaveFreq As Button
    Friend WithEvents txtSaveCam As Button
    Friend WithEvents btnRead As Button

End Class
