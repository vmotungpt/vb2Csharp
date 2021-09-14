''' <summary>
''' S00_0316 運行サブメニュー（チェックイン）
''' </summary>
Public Class S00_0316
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
        Me.ActiveControl = Me.btnMiCheckinYoyakuListInquiry

    End Sub

#Region "フッタ"

    ''' <summary>
    ''' F2：戻るボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF2_ClickOrgProc()

        Me.Close()
    End Sub

#End Region

#Region "ボタン"

    ''' <summary>
    ''' 未チェックイン予約一覧照会ボタン実行時のイベント
    ''' </summary>
    Private Sub btnMiCheckinYoyakuListInquiry_Click(sender As Object, e As EventArgs) Handles btnMiCheckinYoyakuListInquiry.Click
        '未チェックイン予約一覧照会へ遷移
        'Using form As New S04_0201
        '    form.ShowDialog()
        'End Using
    End Sub

    ''' <summary>
    ''' チェックイン状況一覧照会ボタン実行時のイベント
    ''' </summary>
    Private Sub btnCheckinSituationListInquiry_Click(sender As Object, e As EventArgs) Handles btnCheckinSituationListInquiry.Click
        'チェックイン状況一覧照会へ遷移
        'Using form As New S04_0203
        '    form.ShowDialog()
        'End Using
    End Sub

#End Region

#End Region

#Region "メソッド"

    ''' <summary>
    ''' 画面初期化
    ''' </summary>
    Private Sub setControlInitiarize()

        'ベースフォームの設定
        Me.setFormId = "S00_0316"
        Me.setTitle = "運行サブメニュー（チェックイン）"

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

        'ボタンを無条件で非活性(Phase2で使用するボタンの為)
        Me.btnCheckinSituationListInquiry.Enabled = False
        Me.btnMiCheckinYoyakuListInquiry.Enabled = False

    End Sub

#End Region

End Class