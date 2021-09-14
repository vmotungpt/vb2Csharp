''' <summary>
''' S00_0317 運行サブメニュー（受付状況確認）
''' </summary>
Public Class S00_0317
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
        Me.ActiveControl = Me.btnMihakkenYoyakuListInquiry

    End Sub

    ''' <summary>
    ''' F2：戻るボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF2_ClickOrgProc()

        Me.Close()
    End Sub

#End Region

#Region "フッタ"

    ''' <summary>
    ''' 乗車状況照会ボタン実行時のイベント
    ''' </summary>
    Private Sub btnMihakkenYoyakuListInquiry_Click(sender As Object, e As EventArgs) Handles btnMihakkenYoyakuListInquiry.Click
        '乗車状況照会へ遷移
        Using form As New S04_0301()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' NO SHOW一覧ボタン実行時のイベント
    ''' </summary>
    Private Sub btnNoShowList_Click(sender As Object, e As EventArgs) Handles btnNoShowList.Click
        'NO SHOW 一覧へ遷移
        Using form As New S04_0303
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' お客様状況一覧照会ボタン実行時のイベント
    ''' </summary>
    Private Sub btnCustomerSituationListInquiry_Click(sender As Object, e As EventArgs) Handles btnCustomerSituationListInquiry.Click
        'お客様状況一覧照会へ遷移
        'Using form As New S04_0304
        '    form.ShowDialog()
        'End Using
    End Sub

#End Region

#Region "メソッド"

    ''' <summary>
    ''' 画面初期化
    ''' </summary>
    Private Sub setControlInitiarize()

        'ベースフォームの設定
        Me.setFormId = "S00_0317"
        Me.setTitle = "運行サブメニュー（受付状況確認）"

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

    Private Sub setBtnControl()

        'ボタンを無条件で非活性(Phase2で使用するボタンの為)
        Me.btnCustomerSituationListInquiry.Enabled = False

    End Sub

#End Region

End Class