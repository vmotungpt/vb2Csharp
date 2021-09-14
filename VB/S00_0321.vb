''' <summary>
''' S00_0321 運行サブメニュー（帳票）
''' </summary>
Public Class S00_0321
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
        Me.btnYoyakuPersonList.Focus()

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
    ''' 予約者リストボタン実行時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnYoyakuPersonList_Click(sender As Object, e As EventArgs) Handles btnYoyakuPersonList.Click
        '予約者リストへ遷移
        Using form As New S04_1001()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' 人員表／日別運行表ボタン実行時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnJininTableDayBetuUnkouTable_Click(sender As Object, e As EventArgs) Handles btnJininTableDayBetuUnkouTable.Click
        '人員表／日別運行表へ遷移
        Using form As New S04_1002()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' 乗客名簿ボタン実行時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnJyokyakuMeibo_Click(sender As Object, e As EventArgs) Handles btnJyokyakuMeibo.Click
        '乗客名簿へ遷移
        Using form As New S04_1003()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' 配車表ボタン実行時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnHaisyaTable_Click(sender As Object, e As EventArgs) Handles btnHaisyaTable.Click
        '配車表へ遷移
        'Using form As New S04_1004()
        '    form.ShowDialog()
        'End Using
    End Sub

#End Region

#End Region

#Region "メソッド"

    ''' <summary>
    ''' 画面表示時の初期設定
    ''' </summary>
    Private Sub setControlInitiarize()

        'ベースフォームの設定
        Me.setFormId = "S00_0321"
        Me.setTitle = "運行サブメニュー（帳票）"

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
        Me.btnHaisyaTable.Enabled = False

    End Sub

#End Region

End Class