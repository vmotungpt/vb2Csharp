''' <summary>
''' S00_0319 運行サブメニュー（ピックアップ管理）
''' </summary>
Public Class S00_0319
    Inherits FormBase

#Region "イベント"

    ''' <summary>
    ''' フォーム起動時の独自処理
    ''' </summary>
    Protected Overrides Sub StartupOrgProc()

        '画面上ボタン制御
        Me.setBtnControl()

        '画面表示時の初期設定
        Me.setControlInitiarize()

        'フォーカス設定
        Me.ActiveControl = Me.btnYoyakuListInquiryPickup

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
    ''' 予約一覧照会(ピックアップ) ボタン実行時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnYoyakuListInquiryPickup_Click(sender As Object, e As EventArgs) Handles btnYoyakuListInquiryPickup.Click
        '予約一覧照会（ピックアップ）へ遷移
        Using form As New S04_0601()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' ピックアップ用リストボタン実行時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnPickupForList_Click(sender As Object, e As EventArgs) Handles btnPickupForList.Click
        'ピックアップ用リストへ遷移
        Using form As New S04_0602()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' ピックアップ予約・乗車状況一覧照会ボタン実行時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnPickupYoyakuJyosyaSituationListInquiry_Click(sender As Object, e As EventArgs) Handles btnPickupYoyakuJyosyaSituationListInquiry.Click
        'ピックアップ予約・乗車状況一覧照会へ遷移
        'Using form As New S04_0603()
        '    form.ShowDialog()
        'End Using
    End Sub

    ''' <summary>
    ''' ピックアップ時刻修正ボタン実行時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnPickupTimeRev_Click(sender As Object, e As EventArgs) Handles btnPickupTimeRev.Click
        'ピックアップ時刻修正へ遷移
        Using form As New S04_0604()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' ピックアップ在庫調整ボタン実行時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnPickupStockCyosei_Click(sender As Object, e As EventArgs) Handles btnPickupStockCyosei.Click
        'ピックアップ在庫調整へ遷移
        Using form As New S04_0605()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' ピックアップ台帳修正（予約停止）ボタン実行時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnPickupLedgerRevYoyakuStop_Click(sender As Object, e As EventArgs) Handles btnPickupLedgerRevYoyakuStop.Click
        'ピックアップ台帳修正（予約停止）へ遷移
        Using form As New S04_0606()
            form.ShowDialog()
        End Using
    End Sub

#End Region

#End Region

#Region "メソッド"

    ''' <summary>
    ''' 画面表示時の初期設定
    ''' </summary>
    Private Sub setControlInitiarize()

        'ベースフォームの設定
        Me.setFormId = "S00_0319"
        Me.setTitle = "運行サブメニュー（ピックアップ管理）"

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
        Me.btnPickupYoyakuJyosyaSituationListInquiry.Enabled = False

    End Sub

#End Region

End Class