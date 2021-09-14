''' <summary>
''' S00_0318 運行サブメニュー（準備金管理）
''' </summary>
Public Class S00_0318
    Inherits FormBase

#Region "イベント"

    ''' <summary>
    ''' フォーム起動時の独自処理
    ''' </summary>
    Protected Overrides Sub StartupOrgProc()
        '画面上ボタン制御
        Me.setBtnControl()

        '画面初期化
        Me.setControlInitiarize()

        'フォーカス設定
        Me.ActiveControl = Me.btnS04_0501
    End Sub

    ''' <summary>
    ''' F2：戻るボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF2_ClickOrgProc()
        Me.Close()
    End Sub

#End Region

#Region "フッタ"

#End Region

#Region "メソッド"

    ''' <summary>
    ''' 画面初期化
    ''' </summary>
    Private Sub setControlInitiarize()

        'ベースフォームの設定
        'Visible
        Me.F1Key_Visible = False
        Me.F2Key_Visible = True
        Me.F3Key_Visible = False
        Me.F4Key_Visible = False
        Me.F5Key_Visible = False
        Me.F6Key_Visible = False
        Me.F7Key_Visible = False
        Me.F8Key_Visible = False
        Me.F9Key_Visible = False
        Me.F10Key_Visible = False
        Me.F11Key_Visible = False
        Me.F12Key_Visible = False

        'Text
        Me.F2Key_Text = "F2:戻る"
    End Sub

    ''' <summary>
    ''' 画面上ボタン制御
    ''' </summary>
    Private Sub setBtnControl()
        ' 現状処理なし
    End Sub
    ''' <summary>
    ''' '起動画面 ： 『準備金基準情報設定』 （S04_0501）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnS04_0501_Click(sender As Object, e As EventArgs) Handles btnS04_0501.Click
        Using form As New S04_0501
            Call form.patternSettings()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' 起動画面 ： 『準備金管理情報設定』 （S04_0502）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnS04_0502_Click(sender As Object, e As EventArgs) Handles btnS04_0502.Click
        '準備金管理情報設定ボタン押下時処理
        Using form As New S04_0502
            Call form.patternSettings()
            form.ShowDialog()
        End Using
    End Sub
    ''' <summary>
    ''' 起動画面 ： 『準備金申請確定』 （S04_0503）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnS04_0503_Click(sender As Object, e As EventArgs) Handles btnS04_0503.Click
        '準備金申請確定ボタン押下時処理
        Using form As New S04_0503
            Call form.patternSettings()
            form.ShowDialog()
        End Using
    End Sub
    ''' <summary>
    ''' 起動画面 ： 『準備金ラベル』 （S04_0504）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnS04_0504_Click(sender As Object, e As EventArgs) Handles btnS04_0504.Click
        '準備金ラベルボタン押下時処理
        Using form As New S04_0504
            Call form.patternSettings()
            form.ShowDialog()
        End Using
    End Sub
    ''' <summary>
    ''' 起動画面 ： 『準備金仮払精算明細書』 （S04_0505）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnS04_0505_Click(sender As Object, e As EventArgs) Handles btnS04_0505.Click
        '準備金仮払精算明細表ボタン押下時処理
        Using form As New S04_0505
            Call form.patternSettings()
            form.ShowDialog()
        End Using
    End Sub

#End Region

End Class