''' <summary>
''' S00_0315 運行サブメニュー(利用人員確定)
''' </summary>
Public Class S00_0315
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
        Me.ActiveControl = Me.btnRiyouNinzuKakuteiRev

    End Sub

#Region "フッタ"

    ''' <summary>
    ''' F2：閉じるボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF2_ClickOrgProc()

        Me.Close()
    End Sub

#End Region

#Region "ボタン"

    ''' <summary>
    ''' 利用人員確定・修正ボタン実行時のイベント
    ''' </summary>
    Private Sub btnRiyouNinzuKakuteiRev_Click(sender As Object, e As EventArgs) Handles btnRiyouNinzuKakuteiRev.Click
        '利用人員確定・修正へ遷移
        Using form As New S04_0101()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' 船車券ＡＧＴ乗車確認入力ボタン実行時のイベント
    ''' </summary>
    Private Sub btnSensyaKenAＧＴJyosyaKakuninInput_Click(sender As Object, e As EventArgs) Handles btnSensyaKenAＧＴJyosyaKakuninInput.Click
        '船車券ＡＧＴ乗車確認入力へ遷移
        Using form As New S04_0103()
            form.ShowDialog()
        End Using
    End Sub

    ''' <summary>
    ''' 売上確定照会（定期）ボタン実行時のイベント
    ''' </summary>
    Private Sub btnUriageKakuteiInquiryTeiki_Click(sender As Object, e As EventArgs) Handles btnUriageKakuteiInquiryTeiki.Click
        '売上確定照会（定期）へ遷移
        Using form As New S04_0104()
            form.ShowDialog()
        End Using
    End Sub

#End Region

#End Region

#Region "メソッド"

    ''' <summary>
    ''' 画面初期化
    ''' </summary>
    Private Sub setControlInitiarize()

        'ベースフォームの設定
        Me.setFormId = "S00_0315"
        Me.setTitle = "運行サブメニュー（利用人員確定）"

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

#End Region

End Class