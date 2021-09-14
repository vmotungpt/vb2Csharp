Imports System.ComponentModel

''' <summary>
'''     利用人員確定・修正
'''     該当コースの予約情報を一覧で表示し、予約情報の修正（利用人員の修正）を行う（＝利用人員の補正）。補正の履歴管理を行う。
''' </summary>
''' <remarks>
'''    Author：2019/02/07//DTS佐藤
''' </remarks>
Public Class S04_0101
    Inherits PT11

#Region "定数"

    ''' <summary>
    ''' 画面ID
    ''' </summary>
    Private Const ScreenId As String = "S04_0101"
    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private Const ScreenName As String = "利用人員確定・修正"

    ''' <summary>
    ''' 条件GroupBoxのTop座標
    ''' </summary>
    Public Const TopGbxCondition = 41
    ''' <summary>
    ''' 条件GroupBoxのマージン
    ''' </summary>
    Public Const MarginGbxCondition = 6

    ''' <summary>
    ''' グリッド列タイトル_予約状況
    ''' </summary>
    Private Const grgColTitleYoyakuNinzu As String = "予約人数"
    ''' <summary>
    ''' グリッド列タイトル_入金済
    ''' </summary>
    Private Const grdColTitleNyuukinAlready As String = "入金済"
    ''' <summary>
    ''' グリッド列タイトル_未入金
    ''' </summary>
    Private Const grdColTitleMiNyuukin As String = "未入金"
    ''' <summary>
    ''' グリッド列タイトル_チェックイン状況
    ''' </summary>
    Private Const grdColTitleCheckinSituation As String = "チェックイン状況"
    ''' <summary>
    ''' グリッド列タイトル_済
    ''' </summary>
    Private Const grdColTitleAlready As String = "済"
    ''' <summary>
    ''' グリッド列タイトル_仮
    ''' </summary>
    Private Const grdColTitleKari As String = "仮"
    ''' <summary>
    ''' グリッド列タイトル_未
    ''' </summary>
    Private Const grdColTitleMi As String = "未"
    ''' <summary>
    ''' グリッド列タイトル_NoShow
    ''' </summary>
    Private Const grdColTitleNoShow As String = "ＮｏＳｈｏｗ"
    ''' <summary>
    ''' グリッド列タイトル_キャンセル
    ''' </summary>
    Private Const grdColTitleCancel As String = "キャンセル"
    ''' <summary>
    ''' グリッド列タイトル_インファント
    ''' </summary>
    Private Const grdColTitleInfant As String = "インファント"

#End Region

#Region "プロパティ"

    ''' <summary>
    ''' 条件GroupBoxの高さ
    ''' </summary>
    ''' <returns></returns>
    Public Property HeightGbxCondition As Integer
        Get
            Return Me.gbxCondition.Height
        End Get
        Set(value As Integer)
            Me.gbxCondition.Height = value
        End Set
    End Property

    ''' <summary>
    ''' 条件GroupBoxの表示/非表示
    ''' </summary>
    ''' <returns></returns>
    Public Property VisibleGbxCondition As Boolean
        Get
            Return Me.gbxCondition.Visible
        End Get
        Set(value As Boolean)
            Me.gbxCondition.Visible = value
        End Set
    End Property

#End Region

#Region "イベント"

    ''' <summary>
    ''' 画面初期処理
    ''' </summary>
    Protected Overrides Sub StartupOrgProc()

        MyBase.StartupOrgProc()

        'エラー有無のクリア
        clearError()

        ' 画面終了時確認不要
        MyBase.closeFormFlg = False

    End Sub

    ''' <summary>
    ''' 条件GroupBox表示制御ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnVisiblerCondition_Click(sender As Object, e As EventArgs) Handles btnVisiblerCondition.Click
        Me.VisibleGbxCondition = Not Me.VisibleGbxCondition

        'Panel, グリッドの座標, サイズを表示/非表示に応じて変更
        If Me.VisibleGbxCondition Then
            '表示状態
            Me.btnVisiblerCondition.Text = "非表示 <<"

            Me.PanelEx1.Top = TopGbxCondition + Me.HeightGbxCondition + MarginGbxCondition
            Me.PanelEx1.Height -= Me.HeightGbxCondition + MarginGbxCondition
            Me.grdRiyouJininKakuteiRev.Height -= (Me.HeightGbxCondition - 3)

        Else
            '非表示状態
            Me.btnVisiblerCondition.Text = "表示 >>"

            Me.PanelEx1.Top = TopGbxCondition
            Me.PanelEx1.Height += Me.HeightGbxCondition + MarginGbxCondition
            Me.grdRiyouJininKakuteiRev.Height += (Me.HeightGbxCondition - 3)
        End If
    End Sub

    ''' <summary>
    ''' 条件クリアボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click

        ' クリアボタン押下イベント実行
        MyBase.btnCom_Click(Me.btnClear, e)

    End Sub

    ''' <summary>
    ''' 条件クリアボタン押下時
    ''' </summary>
    Protected Overrides Sub btnCLEAR_ClickOrgProc()

        ' 初期処理と同じ処理を実行
        StartupOrgProc()

    End Sub

    ''' <summary>
    ''' F8：検索ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click

        ' 検索ボタン押下イベント実行
        MyBase.btnCom_Click(Me.btnSearch, e)

        ' log出力
        createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理")

    End Sub

    ''' <summary>
    ''' 検索ボタン押下時処理
    ''' </summary>
    Protected Overrides Sub btnF8_ClickOrgProc()

        'MyBase.btnF8_ClickOrgProc()

        'エラー有無のクリア
        clearError()

        '日付入力値の調整
        setYmdFromTo()

        ' [詳細エリア]検索結果部の項目初期化
        initDetailAreaItems()

        '検索条件項目入力チェック
        If checkSearchItems() = True Then
            'エラーがない場合、検索処理を実行

            ' Gridへの表示(グリッドデータの取得とグリッド表示)
            reloadGrid()

        End If

    End Sub

    ''' <summary>
    ''' F8キーダウンイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub S02_1501_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyData = Keys.F8 Then
            e.Handled = True
            Me.btnSearch.Select()
            Me.btnSearch_Click(sender, e)
        End If
    End Sub

    ''' <summary>
    ''' グリッドのデータソース変更時イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub FlexGridEx1_AfterDataRefresh(sender As Object, e As ListChangedEventArgs) Handles grdRiyouJininKakuteiRev.AfterDataRefresh
        'データ件数を表示(ヘッダー行分マイナス1)
        Dim formatedCount As String = (Me.grdRiyouJininKakuteiRev.Rows.Count - 2).ToString.PadLeft(6)
        lblLengthGrd.Text = formatedCount + "件"
    End Sub

    ''' <summary>
    ''' F2：戻るボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF2_ClickOrgProc()

        'MyBase.btnF2_ClickOrgProc()

        closeCheckFlg = False
        MyBase.closeFormFlg = False
        Me.Close()

    End Sub

    ''' <summary>
    ''' 画面終了時処理
    ''' </summary>
    Private Sub S02_1501_FormClosing() Handles Me.FormClosing

        Me.Dispose()

    End Sub

#End Region

#Region "メソッド"
    ''' <summary>
    ''' [検索エリア]検索条件部の項目初期化
    ''' </summary>
    Protected Overrides Sub initSearchAreaItems()
        MyBase.initSearchAreaItems()

        '乗車地コード
        ucoJyosyaTiCd.CodeText = UserInfoManagement.eigyosyoCd
        ucoJyosyaTiCd.ValueText = Nothing

        '出発日FromTo：システム日付を設定
        dtmSyuptDayFromTo.FromDateText = CommonDateUtil.getSystemTime()
        dtmSyuptDayFromTo.ToDateText = CommonDateUtil.getSystemTime()

        'コース種別
        If UserInfoManagement.gaikokugoCrsSelectFlg = True Then
            'ユーザーが国際事業部の場合は外国語
            chkGaikokugo.Checked = True
            chkJapanese.Checked = False
        Else
            'それ以外の場合は日本語をONに設定
            chkJapanese.Checked = True
            chkGaikokugo.Checked = False
        End If

        '上記以外の項目は全て空欄/チェックOFF
        'コースコード
        ucoCrsCd.CodeText = ""
        ucoCrsCd.ValueText = Nothing

        '号車
        txtGousya.Text = ""

        '出発時間
        dtmSyuptTime.Text = ""

        'コース区分
        chkTeikiNoon.Checked = False
        chkTeikiNight.Checked = False
        chkKikakuDayTrip.Checked = False
        chkKikakuStay.Checked = False
        chkNightLine.Checked = False
        chkBoat.Checked = False
        chk2StayMore.Checked = False
        chkRCrs.Checked = False

        ' ロード時にフォーカスを設定する
        Me.ActiveControl = Me.ucoJyosyaTiCd
    End Sub


    ''' <summary>
    ''' エラー有無のクリア
    ''' </summary>
    Private Sub clearError()

        ' ExistErrorプロパティのクリア
        ucoJyosyaTiCd.ExistError = False
        dtmSyuptDayFromTo.ExistErrorForFromDate = False
        dtmSyuptDayFromTo.ExistErrorForToDate = False
        chkJapanese.ExistError = False
        chkGaikokugo.ExistError = False

        ucoCrsCd.ExistError = False
        txtGousya.ExistError = False
        dtmSyuptTime.ExistError = False

        chkTeikiNoon.ExistError = False
        chkTeikiNight.ExistError = False
        chkKikakuDayTrip.ExistError = False
        chkKikakuStay.ExistError = False
        chkNightLine.ExistError = False
        chkBoat.ExistError = False
        chk2StayMore.ExistError = False
        chkRCrs.ExistError = False

        ' Exceptionのクリア
        Me.Exception = Nothing

        ' エラーフラグの初期化
        Me.ErrorFlg = False

    End Sub

    ''' <summary>
    ''' [詳細エリア]検索結果部の項目初期化
    ''' </summary>
    Protected Overrides Sub initDetailAreaItems()
        MyBase.initDetailAreaItems()

        Dim dt As New DataTable
        setgrdRiyouJininKakuteiRev()
        Me.grdRiyouJininKakuteiRev.DataSource = dt
        Me.grdRiyouJininKakuteiRev.DataMember = ""
        Me.grdRiyouJininKakuteiRev.Refresh()
        Me.lblLengthGrd.Text = "     0件"
    End Sub

    ''' <summary>
    ''' フッタボタンの制御(表示\[活性]／非表示[非活性])
    ''' </summary>
    Protected Overrides Sub initFooterButtonControl()
        MyBase.initFooterButtonControl()

        Me.F4Key_Visible = False       ' F4:非表示
        Me.F10Key_Visible = False       ' F10:非表示
        Me.F11Key_Visible = False       ' F11:非表示

    End Sub

    '利用人員確定・修正グリッドの設定
    Private Sub setgrdRiyouJininKakuteiRev()

        ' 行ヘッダを作成
        grdRiyouJininKakuteiRev.AllowDragging = CType(False, AllowDraggingEnum)
        grdRiyouJininKakuteiRev.AllowAddNew = False
        grdRiyouJininKakuteiRev.AutoGenerateColumns = False
        grdRiyouJininKakuteiRev.AllowEditing = False

        ' 行ヘッダを作成
        grdRiyouJininKakuteiRev.Styles.Normal.WordWrap = True
        grdRiyouJininKakuteiRev.Cols.Count = 16     ' gridの列数
        grdRiyouJininKakuteiRev.Rows.Fixed = 2      ' 行固定
        grdRiyouJininKakuteiRev.Cols.Frozen = 4     ' 列固定
        grdRiyouJininKakuteiRev.AllowMerging = AllowMergingEnum.Custom
        grdRiyouJininKakuteiRev.Rows(0).AllowMerging = True
        grdRiyouJininKakuteiRev.Cols(0).AllowMerging = True

        Dim cr As CellRange

        ' 出発日
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 1, 1, 1)
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr)
        ' 乗車地
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 2, 1, 2)
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr)
        ' コースコード
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 3, 1, 3)
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr)
        ' コース名
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 4, 1, 4)
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr)
        ' 出発時間
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 5, 1, 5)
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr)
        '号車
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 6, 1, 6)
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr)
        '予約人数
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 7, 0, 8)
        cr.Data = grgColTitleYoyakuNinzu
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr)
        grdRiyouJininKakuteiRev(1, 7) = grdColTitleNyuukinAlready
        grdRiyouJininKakuteiRev(1, 8) = grdColTitleMiNyuukin
        'チェックイン状況
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 9, 0, 14)
        cr.Data = grdColTitleCheckinSituation
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr)
        grdRiyouJininKakuteiRev(1, 9) = grdColTitleAlready
        grdRiyouJininKakuteiRev(1, 10) = grdColTitleKari
        grdRiyouJininKakuteiRev(1, 11) = grdColTitleMi
        grdRiyouJininKakuteiRev(1, 12) = grdColTitleNoShow
        grdRiyouJininKakuteiRev(1, 13) = grdColTitleCancel
        grdRiyouJininKakuteiRev(1, 14) = grdColTitleInfant

        '利用人員
        cr = grdRiyouJininKakuteiRev.GetCellRange(0, 15, 1, 15)
        grdRiyouJininKakuteiRev.MergedRanges.Add(cr)

    End Sub

    ''' <summary>
    ''' 出発日入力値の調整
    ''' </summary>
    Private Sub setYmdFromTo()
        If dtmSyuptDayFromTo.FromDateText IsNot Nothing And dtmSyuptDayFromTo.ToDateText Is Nothing Then
            '出発日From <> ブランク かつ 出発日To = ブランクの場合、出発日To に 出発日From の値をセット
            dtmSyuptDayFromTo.ToDateText = dtmSyuptDayFromTo.FromDateText

        ElseIf dtmSyuptDayFromTo.ToDateText IsNot Nothing And dtmSyuptDayFromTo.FromDateText Is Nothing Then
            '出発日To <> ブランク かつ 出発日From = ブランクの場合、出発日From に 出発日To の値をセット
            dtmSyuptDayFromTo.FromDateText = dtmSyuptDayFromTo.ToDateText
        End If
    End Sub

    ''' <summary>
    '''検索条件項目入力チェック
    ''' </summary>
    ''' <returns>エラーがない場合：True、エラーの場合：False</returns>
    Protected Overrides Function checkSearchItems() As Boolean
        MyBase.checkSearchItems()

        '乗車地コード入力チェック
        If String.IsNullOrEmpty(ucoJyosyaTiCd.CodeText) = True Then

            '乗車地コードが入力されていない場合、エラー        
            '乗車地コードを入力してください。
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "乗車地コード")

            '背景色を赤にする
            ucoJyosyaTiCd.ExistError = True

            '先頭のエラー項目にフォーカスを設定する
            Me.ActiveControl = Me.ucoJyosyaTiCd

            Return False
        End If

        'コース種別入力チェック
        If chkJapanese.Checked = False And chkGaikokugo.Checked = False Then

            'コース種別が選択されていません
            CommonProcess.createFactoryMsg().messageDisp("E90_024", "コース種別")

            '背景色を赤にする
            chkJapanese.ExistError = True
            chkGaikokugo.ExistError = True

            '先頭のエラー項目にフォーカスを設定する
            Me.ActiveControl = Me.chkJapanese

            Return False
        End If

        '日付入力チェック
        If dtmSyuptDayFromTo.FromDateText Is Nothing And dtmSyuptDayFromTo.ToDateText Is Nothing Then

            '日付Fromと日付Toどちらも未入力の場合、エラー
            '日付を入力してください。
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "日付")

            '背景色を赤にする
            dtmSyuptDayFromTo.ExistErrorForToDate = True
            dtmSyuptDayFromTo.ExistErrorForFromDate = True

            '先頭のエラー項目にフォーカスを設定する
            Me.ActiveControl = Me.dtmSyuptDayFromTo

            Return False
        End If

        '日付入力値チェック
        If CommonDateUtil.chkDayFromTo(CDate(dtmSyuptDayFromTo.FromDateText), CDate(dtmSyuptDayFromTo.ToDateText)) = False Then

            '日付From＞日付Toの場合、エラー
            '日付の設定が不正です。
            CommonProcess.createFactoryMsg().messageDisp("E90_017", "日付")

            '背景色を赤にする
            dtmSyuptDayFromTo.ExistErrorForToDate = True
            dtmSyuptDayFromTo.ExistErrorForFromDate = True

            '先頭のエラー項目にフォーカスを設定する
            Me.ActiveControl = Me.dtmSyuptDayFromTo

            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' Gridへの表示(グリッドデータの取得とグリッド表示)
    ''' </summary>
    Protected Overrides Sub reloadGrid()
        MyBase.reloadGrid()

        'DBパラメータ
        Dim paramInfoList As New Hashtable
        Dim dt = New DataTable

        '日付(パラメータ)編集用変数
        Dim dteTmpDate As DateTime = Nothing

        'データアクセス
        Dim dataAccess As New S04_0101_DA
        Dim dataRiyouJininList As New DataTable

        'グリッド表示用dataRow
        Dim dr As DataRow = Nothing
        'dt.NewRow
        Dim drX As DataRow = Nothing

        Dim SyuptTime As TimeSpan = Nothing

        'パラメータ設定
        '乗車地コード
        paramInfoList.Add("JyosyaTiCd", Trim(Me.ucoJyosyaTiCd.CodeText))

        '出発日
        paramInfoList.Add("SyuptDayFrom", Format(Me.dtmSyuptDayFromTo.FromDateText, "yyyyMMdd"))
        paramInfoList.Add("SyuptDayTo", Format(Me.dtmSyuptDayFromTo.ToDateText, "yyyyMMdd"))

        'コースコード
        paramInfoList.Add("CrsCd", Trim(Me.ucoCrsCd.CodeText))

        '号車
        paramInfoList.Add("Gousya", Trim(Me.txtGousya.Text))

        '出発時間
        If Not String.IsNullOrEmpty(dtmSyuptTime.Value.ToString) = True Then
            SyuptTime = CType(Me.dtmSyuptTime.Value, TimeSpan)
            paramInfoList.Add("SyuptTime", String.Concat(SyuptTime.Hours.ToString.PadLeft(2, "0"c),
                                                         SyuptTime.Minutes.ToString.PadLeft(2, "0"c)))
        Else
            paramInfoList.Add("SyuptTime", Trim(Me.dtmSyuptTime.Value.ToString))
        End If

        'コース種別
        '日本語
        paramInfoList.Add("Japanese", Trim(CType(Me.chkJapanese.Checked, String)))
        '外国語
        paramInfoList.Add("Gaikokugo", Trim(CType(Me.chkGaikokugo.Checked, String)))

        'コース区分
        '定期（昼）
        paramInfoList.Add("TeikiNoon", Trim(CType(Me.chkTeikiNoon.Checked, String)))
        '定期（夜）
        paramInfoList.Add("TeikiNight", Trim(CType(Me.chkTeikiNight.Checked, String)))
        '企画（日帰り）
        paramInfoList.Add("KikakuDayTrip", Trim(CType(Me.chkKikakuDayTrip.Checked, String)))
        '企画（宿泊）
        paramInfoList.Add("KikakuStay", Trim(CType(Me.chkKikakuStay.Checked, String)))
        '夜行
        paramInfoList.Add("NightLine", Trim(CType(Me.chkNightLine.Checked, String)))
        '船舶
        paramInfoList.Add("Boat", Trim(CType(Me.chkBoat.Checked, String)))
        '２泊以上
        paramInfoList.Add("2StayMore", Trim(CType(Me.chk2StayMore.Checked, String)))
        'Ｒコース
        paramInfoList.Add("RCrs", Trim(CType(Me.chkRCrs.Checked, String)))

        'SQLでデータを取得
        dataRiyouJininList = dataAccess.getRiyouJininList(paramInfoList)

        'データ取得件数チェック
        If dataRiyouJininList.Rows.Count <= 0 Then
            '取得件数が0件の場合、エラー

            ' [詳細エリア]検索結果部の項目初期化
            initDetailAreaItems()

            '該当データが存在しません。
            CommonProcess.createFactoryMsg().messageDisp("E90_019")

        Else
            'データが取得できた場合

            '列作成
            dt.Columns.Add("colSyuptDay")           '出発日
            dt.Columns.Add("colJyosyaTi")           '乗車地
            dt.Columns.Add("colCrsCd")              'コースコード
            dt.Columns.Add("colCrsName")            'コース名
            dt.Columns.Add("colSyuptTime")          '出発時間
            dt.Columns.Add("colGousya")             '号車
            dt.Columns.Add("colNyuukinAlready")     '入金済
            dt.Columns.Add("colMiNyuukin")          '未入金
            dt.Columns.Add("colCheckinAlready")     '済
            dt.Columns.Add("colKariCheckin")        '仮
            dt.Columns.Add("colMiCheckin")          '未
            dt.Columns.Add("colNoShow")             'ＮｏＳｈｏｗ
            dt.Columns.Add("colCancel")             'キャンセル
            dt.Columns.Add("colInfant")             'インファント
            dt.Columns.Add("colRiyouJinin")         '利用人員

            '取得した値を各列に設定
            For Each dr In dataRiyouJininList.Rows
                drX = dt.NewRow

                drX("colSyuptDay") = Format(dr("SYUPT_DAY"), "yy/MM/dd")
                drX("colJyosyaTi") = dr("PLACE_NAME_SHORT")
                drX("colCrsCd") = dr("CRS_CD")
                drX("colCrsName") = dr("CRS_NAME")
                drX("colSyuptTime") = dr("SYUPT_TIME")
                drX("colGousya") = dr("GOUSYA")
                drX("colNyuukinAlready") = dr("NYUUKIN_ALREADY")
                drX("colMiNyuukin") = dr("MI_NYUUKIN")
                drX("colCheckinAlready") = 0
                drX("colKariCheckin") = 0
                drX("colMiCheckin") = dr("MI_CHECKIN")
                drX("colNoShow") = dr("NO_SHOW")
                drX("colCancel") = dr("CANCEL")
                drX("colInfant") = dr("INFANT")
                drX("colRiyouJinin") = dr("RIYOU_JININ")

                dt.Rows.Add(drX)

            Next

            'グリッドに取得したデータを表示する
            grdRiyouJininKakuteiRev.DataSource = dt

        End If
    End Sub

#End Region

End Class