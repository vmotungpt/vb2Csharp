Imports System.ComponentModel
''' <summary>
'''     売上確定照会
'''     ダイレクトチェックインを行った予約について、売上の確定を行う。（システム内のデータを発券済み状態にする）
''' </summary>
''' <remarks>
'''    Author：2019/02/04//DTS佐藤
''' </remarks>
Public Class S04_0104
    Inherits PT21
    Implements iPT21

#Region "定数"

    ''' <summary>
    ''' 画面ID
    ''' </summary>
    Private Const ScreenId As String = "S04_0104"
    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private Const ScreenName As String = "売上確定照会"
    ''' <summary>
    ''' 乗車地コード (初期設定値)    浜松町バスターミナル(コード値:15))
    ''' </summary>
    Private Const _initPlaceCd As String = "15"
    'グリッド表示用
    ''' <summary>
    ''' 精算方法_振込
    ''' </summary>
    Private Const strSeisanHohoHurikomi = "振込"
    ''' <summary>
    ''' 精算方法_営業所
    ''' </summary>
    Private Const strSeisanHohoEigyosyo = "営業所"
    ''' <summary>
    ''' 精算方法_当日払い
    ''' </summary>
    Private Const strSeisanHohoTojituPayment = "当日払い"
    ''' <summary>
    ''' 精算方法_ＡＧＴ
    ''' </summary>
    Private Const strSeisanHohoAgt = "ＡＧＴ"
    ''' <summary>
    ''' 精算方法_カード
    ''' </summary>
    Private Const strSeisanHohoCard = "カード"

    '画面遷移用
    ''' <summary>
    ''' 列番号(グリッド) 予約区分
    ''' </summary>
    Private Const NoColYoyakuKbn = 6
    ''' <summary>
    ''' 列番号(グリッド) 予約NO
    ''' </summary>
    Private Const NoColYoyakuNo = 7

    ''' <summary>
    ''' 条件GroupBoxのTop座標
    ''' </summary>
    Public Const TopGbxCondition = 41

    ''' <summary>
    ''' 条件GroupBoxのマージン
    ''' </summary>
    Public Const MarginGbxCondition = 6

    ''' <summary>
    ''' 条件GroupBox非表示時のGroupBoxArea1の高さ
    ''' </summary>
    Public Const HeightGbxAreas1OnNotVisibleCondition = 272

    ''' <summary>
    ''' 条件GroupBox非表示時のGroupBoxArea2の高さ
    ''' </summary>
    Public Const HeightGbxAreas2OnNotVisibleCondition = 429

    ''' <summary>
    ''' 条件GroupBoxの高さ
    ''' </summary>
    Private _heightGbxCondition As Integer

    ''' <summary>
    ''' GroupBoxArea1の高さ
    ''' </summary>
    Private _heightGbxArea1 As Integer

    ''' <summary>
    ''' GroupBoxArea2の高さ
    ''' </summary>
    Private _heightGbxArea2 As Integer

    ''' <summary>
    ''' GroupBoxArea1のTop座標
    ''' </summary>
    Private _topGbxArea1 As Integer

    ''' <summary>
    ''' GroupBoxArea2のTop座標
    ''' </summary>
    Private _topGbxArea2 As Integer

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

        'ベースフォームの設定
        Me.setFormId = ScreenId
        Me.setTitle = ScreenName

        'エラー有無のクリア
        clearError()

    End Sub

    ''' <summary>
    ''' 条件GroupBox表示制御ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnVisiblerCondition_Click(sender As Object, e As EventArgs) Handles btnVisiblerCondition.Click
        Me.VisibleGbxCondition = Not Me.VisibleGbxCondition

        Dim offSet As Integer = Me.HeightGbxCondition + MarginGbxCondition
        Dim heightCondition As Integer = CInt(Me.HeightGbxCondition / 2)

        'GrpBoxArea1, 2の座標, サイズを表示/非表示に応じて変更
        If Me.VisibleGbxCondition Then
            '表示状態
            Me.btnVisiblerCondition.Text = "非表示 <<"

            Me.setGrpLayout()
            Me.gbxArea1.Height -= heightCondition
            Me.gbxArea2.Height -= heightCondition
            Me.grdCrs.Height -= heightCondition
            Me.grdYoyaku.Height -= heightCondition
        Else
            '非表示状態
            Me.btnVisiblerCondition.Text = "表示 >>"
            Me.gbxArea1.Height = HeightGbxAreas1OnNotVisibleCondition
            Me.gbxArea2.Height = HeightGbxAreas2OnNotVisibleCondition

            Me.gbxArea1.Top = TopGbxCondition
            Me.gbxArea2.Top = TopGbxCondition + Me.gbxArea1.Height + MarginGbxCondition
            Me.grdCrs.Height += heightCondition
            Me.grdYoyaku.Height += heightCondition

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
    ''' F9：予約照会ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnYoyakuInquiry_Click(sender As Object, e As EventArgs) Handles btnYoyakuInquiry.Click

        ' F9:予約照会ボタン押下イベント実行
        MyBase.btnCom_Click(Me.btnYoyakuInquiry, e)

        ' log出力
        createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理")

    End Sub

    ''' <summary>
    ''' キーダウンイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub S04_0104_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyData = Keys.F8 Then
            e.Handled = True
            Me.btnSearch.Select()
            Me.btnSearch_Click(sender, e)

        ElseIf e.KeyData = Keys.F9 Then
            e.Handled = True
            Me.btnYoyakuInquiry_Click(sender, e)
        End If

    End Sub

    ''' <summary>
    ''' F8(検索)ボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF8_ClickOrgProc()

        'エラー有無のクリア
        clearError()

        ' [詳細エリア]検索結果部の項目初期化
        initDetailAreaItems()

        '検索条件項目入力チェック
        If Me.checkSearchItems() = True Then
            'エラーがない場合、検索処理を実行

            'コース一覧の取得
            setDataCrsGrid()

        End If

    End Sub

    ''' <summary>
    ''' F9(内訳)ボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF9_ClickOrgProc()

        Call setDataYoyakuGrid()

        ' 後処理
        Call MyBase.comPostEvent()

        Me.btnYoyakuInquiry.Focus()
    End Sub


    ''' <summary>
    ''' グリッドのデータソース変更時イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub FlexGridEx1_AfterDataRefresh(sender As Object, e As ListChangedEventArgs) Handles grdCrs.AfterDataRefresh
        'データ件数を表示(ヘッダー行分マイナス1)
        Dim formatedCount As String = (Me.grdCrs.Rows.Count - 1).ToString.PadLeft(6)
        Me.lblLengthGrd.Text = formatedCount + "件"
    End Sub

    ''' <summary>
    ''' 予約ボタン実行時の画面遷移イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub yoyakuUriageKakuteiBtnClick(ByVal sender As Object, ByVal e As C1.Win.C1FlexGrid.RowColEventArgs
                                  ) Handles grdYoyaku.CellButtonClick

        Dim grd As FlexGridEx
        Dim prm As New S04_0105ParamData
        Dim yoyakuKbn As String             '予約区分(画面遷移パラメータ)
        Dim yoyakuNo As Integer             '予約NO(画面遷移パラメータ)

        grd = TryCast(sender, FlexGridEx)

        '選択行が0以下の場合は処理をしない
        If grd.Row <= 0 Then
            Return
        End If

        '押下行の予約区分、予約NO
        yoyakuKbn = TryCast(grd.GetData(e.Row, NoColYoyakuKbn), String)

        If grd.GetData(e.Row, NoColYoyakuNo) Is Nothing Then
            Return
        End If

        yoyakuNo = Integer.Parse(grd.GetData(e.Row, NoColYoyakuNo).ToString())

        '予約ボタン押下
        '画面間パラメータを用意
        prm.YoyakuKbn = yoyakuKbn                         '予約区分
        prm.YoyakuNo = yoyakuNo                           '予約NO

        '売上確定入力　画面展開
        Using form As S04_0105 = New S04_0105()
            form.ParamData = prm
            form.ShowDialog()
        End Using
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
    Private Sub S04_0104_FormClosing() Handles Me.FormClosing

        Me.Dispose()

    End Sub


    ''' <summary>
    ''' コース一覧_全選択ボタン
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnAll_Click(sender As Object, e As EventArgs) Handles btnAll.Click

        For ii As Integer = 1 To grdCrs.Rows.Count - 1 Step 1
            grdCrs.Rows(ii).Item("colSelection") = True
        Next

    End Sub

    ''' <summary>
    ''' コース一覧_全解除ボタン
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnClear02_Click(sender As Object, e As EventArgs) Handles btnClear02.Click
        For ii As Integer = 1 To grdCrs.Rows.Count - 1 Step 1
            grdCrs.Rows(ii).Item("colSelection") = False
        Next
    End Sub

#End Region

#Region "メソッド"

    ''' <summary>
    ''' [検索エリア]検索条件部の項目初期化
    ''' </summary>
    Protected Overrides Sub initSearchAreaItems()
        MyBase.initSearchAreaItems()

        '出発日
        dtmSyuptDay.Value = CommonDateUtil.getSystemTime()
        '乗車地コード ←浜松町バスターミナル(コード値:15)を初期値でセット
        ucoNoribaCd.CodeText = _initPlaceCd　　'UserInfoManagement.eigyosyoCd
        ucoNoribaCd.ValueText = Nothing
        ' コード値のセットだけでは名称が表示されない (名称を取得)
        Me.ucoNoribaCd.ValueText = getPlaceName(Me.ucoNoribaCd.CodeText)

        '上記以外の項目は全て空欄
        'コースコード
        ucoCrsCd.CodeText = Nothing
        ucoCrsCd.ValueText = Nothing
        '出発時間
        dtmSyuptTime.Value = Nothing
        '号車
        txtGousya.Text = Nothing

        ' ロード時にフォーカスを設定する
        Me.ActiveControl = Me.dtmSyuptDay
    End Sub

    ''' <summary>
    ''' 場所マスタより名称取得
    ''' </summary>
    ''' <param name="code"></param>
    ''' <returns></returns>
    Private Function getPlaceName(ByVal code As String) As String

        ' 場所マスタ
        Dim clsPlace_DA = New Master.Place_DA
        Dim paramInfoList As New Hashtable
        paramInfoList.Add("placeCd", code)

        Dim returnPalce As DataTable = clsPlace_DA.getLocationCode(paramInfoList, String.Empty)
        If returnPalce.Rows.Count = 1 Then
            Return CType(returnPalce.Rows(0)("PLACE_NAME_1"), String)
        End If

        Return String.Empty
    End Function

    ''' <summary>
    ''' エラー有無のクリア
    ''' </summary>
    Private Sub clearError()

        ' ExistErrorプロパティのクリア
        dtmSyuptDay.ExistError = False
        ucoNoribaCd.ExistError = False
        ucoCrsCd.ExistError = False
        dtmSyuptTime.ExistError = False
        txtGousya.ExistError = False

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
        setGridCrs()
        Me.grdCrs.DataSource = dt
        Me.grdCrs.DataMember = ""
        Me.grdCrs.Refresh()
        Me.lblLengthGrd.Text = "     0件"

        setGridYoyaku()
        Me.grdYoyaku.DataSource = dt
        Me.grdYoyaku.DataMember = ""
        Me.grdYoyaku.Refresh()
        Me.txtNinzuTotal.Text = ""

        'F9ボタンを非活性にする
        btnYoyakuInquiry.Enabled = False

        '検索条件を表示状態のGroupAreaのレイアウトを保存
        Me.saveGrpLayout()

    End Sub

    ''' <summary>
    ''' [詳細エリア]予約一覧の項目初期化
    ''' </summary>
    Protected Sub initDetailAreaItemsYoyaku()

        Dim dt As New DataTable

        setGridYoyaku()
        Me.grdYoyaku.DataSource = dt
        Me.grdYoyaku.DataMember = ""
        Me.grdYoyaku.Refresh()

        Me.txtNinzuTotal.Text = ""

        '検索条件を表示状態のGroupAreaのレイアウトを保存
        Me.saveGrpLayout()

    End Sub

    ''' <summary>
    ''' フッタボタンの制御(表示\[活性]／非表示[非活性])
    ''' </summary>
    Protected Overrides Sub initFooterButtonControl()
        MyBase.initFooterButtonControl()

        F8Key_Visible = False
        F11Key_Visible = False
    End Sub

    ''' <summary>
    ''' GroupBoxのレイアウト保存
    ''' </summary>
    Private Sub saveGrpLayout()
        Me.gbxCondition.Height = Me.gbxCondition.Height
        Me._heightGbxArea1 = Me.gbxArea1.Height
        Me._heightGbxArea2 = Me.gbxArea2.Height
        Me._topGbxArea1 = Me.gbxArea1.Top
        Me._topGbxArea2 = Me.gbxArea2.Top
    End Sub

    ''' <summary>
    ''' GroupBoxのレイアウト設定
    ''' </summary>
    Private Sub setGrpLayout()
        Me.gbxCondition.Height = Me.gbxCondition.Height
        Me.gbxArea1.Height = Me._heightGbxArea1
        Me.gbxArea2.Height = Me._heightGbxArea2
        Me.gbxArea1.Top = Me._topGbxArea1
        Me.gbxArea2.Top = Me._topGbxArea2
    End Sub

    'コースグリッドの設定
    Private Sub setGridCrs()
        'グリッドの設定
        Me.grdCrs.AllowDragging = CType(False, AllowDraggingEnum)
        Me.grdCrs.AllowAddNew = False
        Me.grdCrs.AllowMerging = CType(8, AllowMergingEnum)
        Me.grdCrs.AutoGenerateColumns = False

        Me.grdCrs.Cols(2).AllowEditing = False
        Me.grdCrs.Cols(3).AllowEditing = False
        Me.grdCrs.Cols(4).AllowEditing = False
        Me.grdCrs.Cols(5).AllowEditing = False
        Me.grdCrs.Cols(6).AllowEditing = False
        Me.grdCrs.Cols(7).AllowEditing = False
    End Sub

    '予約グリッドの設定
    Private Sub setGridYoyaku()
        'グリッドの設定
        Me.grdYoyaku.AllowDragging = CType(False, AllowDraggingEnum)
        Me.grdYoyaku.AllowAddNew = False
        Me.grdYoyaku.AllowMerging = CType(8, AllowMergingEnum)
        Me.grdYoyaku.AutoGenerateColumns = False
        Me.grdYoyaku.ShowButtons = CType(2, ShowButtonsEnum)

        Me.grdYoyaku.Cols(2).AllowEditing = False
        Me.grdYoyaku.Cols(3).AllowEditing = False
        Me.grdYoyaku.Cols(4).AllowEditing = False
        Me.grdYoyaku.Cols(5).AllowEditing = False
    End Sub

    ''' <summary>
    '''検索条件項目入力チェック
    ''' </summary>
    ''' <returns>エラーがない場合：True、エラーの場合：False</returns>
    Protected Overrides Function checkSearchItems() As Boolean
        MyBase.checkSearchItems()

        '出発日入力チェック
        If String.IsNullOrEmpty(dtmSyuptDay.Value.ToString) = True Then

            '出発日が入力の場合、エラー
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "出発日")

            '背景色を赤にする
            dtmSyuptDay.ExistError = True

            '先頭のエラー項目にフォーカスを設定する
            Me.ActiveControl = Me.dtmSyuptDay

            Return False
        End If

        '乗車地コード入力チェック
        If String.IsNullOrEmpty(ucoNoribaCd.CodeText) = True Then

            '出発日が入力の場合、エラー
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "乗車地コード")

            '背景色を赤にする
            ucoNoribaCd.ExistError = True

            '先頭のエラー項目にフォーカスを設定する
            Me.ActiveControl = Me.ucoNoribaCd

            Return False
        End If

        Return True

    End Function


    'コース一覧の取得
    Private Sub setDataCrsGrid()

        Dim paramInfoList As New Hashtable
        Dim dtCrs = New DataTable

        'データアクセス
        Dim dataAccess As New S04_0104_DA
        Dim dataCrsInfo As New DataTable

        'グリッド表示用dataRow
        Dim drCrs As DataRow = Nothing
        'dt.NewRow
        Dim drX As DataRow = Nothing

        Dim SyuptTime As TimeSpan = Nothing

        'パラメータ設定
        '出発日
        paramInfoList.Add("SyuptDay", Format(Me.dtmSyuptDay.Value, "yyyyMMdd"))
        '乗車地コード
        paramInfoList.Add("JyochachiCd", Trim(Me.ucoNoribaCd.CodeText))

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

        'SQLでデータを取得
        dataCrsInfo = dataAccess.getCrsList(paramInfoList)

        'データ取得件数チェック
        If dataCrsInfo.Rows.Count <= 0 Then
            '取得件数が0件の場合、エラー

            ' [詳細エリア]検索結果部の項目初期化
            initDetailAreaItems()

            '該当データが存在しません。
            CommonProcess.createFactoryMsg().messageDisp("E90_019")

        Else
            'データが取得できた場合

            '列作成
            dtCrs.Columns.Add("colSelection")          '選択
            dtCrs.Columns.Add("colJyosyaTi")           '乗車地
            dtCrs.Columns.Add("colCrsCd")              'コースコード
            dtCrs.Columns.Add("colCrsName")            'コース名
            dtCrs.Columns.Add("colSyuptTime")          '出発時間
            dtCrs.Columns.Add("colGousya")             '号車
            dtCrs.Columns.Add("colYoyakuNinzu")        '予約人数
            dtCrs.Columns.Add("colSyuptDay")           '出発日(非表示）

            '取得した値を各列に設定
            For Each drCrs In dataCrsInfo.Rows
                drX = dtCrs.NewRow

                drX("colSelection") = ""
                drX("colJyosyaTi") = drCrs("PLACE_NAME_SHORT")
                drX("colCrsCd") = drCrs("CRS_CD")
                drX("colCrsName") = drCrs("CRS_NAME")
                drX("colSyuptTime") = drCrs("SYUPT_TIME")
                drX("colGousya") = drCrs("GOUSYA")
                drX("colYoyakuNinzu") = drCrs("YOYAKU_NUM")
                drX("colSyuptDay") = drCrs("SYUPT_DAY")

                dtCrs.Rows.Add(drX)
            Next

            'グリッドに取得したデータを表示する
            grdCrs.DataSource = dtCrs

            'F9ボタンを活性化する
            btnYoyakuInquiry.Enabled = True

        End If
    End Sub

    '予約一覧の取得
    Private Sub setDataYoyakuGrid()
        'DBパラメータ
        Dim paramSyuptDayList As New List(Of Integer)       '出発日
        Dim paramCrsCdList As New List(Of String)           'コースコード
        Dim paramGousyaList As New List(Of Integer)         '号車

        Dim dtYoyaku = New DataTable

        'データアクセス
        Dim dataAccess As New S04_0104_DA
        Dim dataYoyakuInfo As New DataTable

        'グリッド表示用dataRow
        Dim drYoyaku As DataRow = Nothing
        'dt.NewRow
        Dim drX As DataRow = Nothing

        '精算方法表示用
        Dim strSeisanHoho As String = Nothing
        '予約人数算出用
        Dim sumYoyakuNinzu As Integer = Nothing
        '発券状態取得用
        Dim hakkenStateArry As String() = Nothing
        For ii As Integer = 1 To grdCrs.Rows.Count - 1 Step 1

            'パラメータ設定
            If grdCrs.Rows(ii).Item("colSelection").ToString = Boolean.TrueString Then

                '出発日
                paramSyuptDayList.Add(CInt(grdCrs.Rows(ii).Item("colSyuptDay")))
                'コースコード
                paramCrsCdList.Add(grdCrs.Rows(ii).Item("colCrsCd").ToString)
                '号車
                paramGousyaList.Add(CInt(grdCrs.Rows(ii).Item("colGousya")))

            End If
        Next

        'パラメータチェック
        If paramSyuptDayList.Count = 0 AndAlso paramCrsCdList.Count = 0 AndAlso paramGousyaList.Count = 0 Then

            'コースが選択されていません。
            CommonProcess.createFactoryMsg().messageDisp("E90_024”, "コース")

            '予約一覧の項目初期化
            initDetailAreaItemsYoyaku()

            Return
        End If


        'SQLでデータを取得
        dataYoyakuInfo = dataAccess.getYoyakuList(paramSyuptDayList, paramCrsCdList, paramGousyaList)

        'データ取得件数チェック
        If dataYoyakuInfo.Rows.Count <= 0 Then
            '取得件数が0件の場合、エラー

            '予約一覧の項目初期化
            initDetailAreaItemsYoyaku()

            '該当データが存在しません。
            CommonProcess.createFactoryMsg().messageDisp("E90_019")

        Else
            'データが取得できた場合

            '列作成
            dtYoyaku.Columns.Add("colUriageKakutei")          '売上確定ボタン
            dtYoyaku.Columns.Add("colYoyakuNo")               '予約番号(表示用)
            dtYoyaku.Columns.Add("colSurnameName")            '姓名
            dtYoyaku.Columns.Add("colNinzu")                  '人数
            dtYoyaku.Columns.Add("colSiharaiHoho")            '支払方法
            dtYoyaku.Columns.Add("colYoyakuKbn")              '予約区分
            dtYoyaku.Columns.Add("colYoyakuNo02")             '予約番号
            dtYoyaku.Columns.Add("colCrsCd")                  'コースコード
            dtYoyaku.Columns.Add("colGousya")                 '号車
            dtYoyaku.Columns.Add("colJotai")                  '発券状態

            '取得した値を各列に設定
            For Each drYoyaku In dataYoyakuInfo.Rows
                drX = dtYoyaku.NewRow

                drX("colUriageKakutei") = ""
                drX("colYoyakuNo") = drYoyaku("YOYAKU_NO_DISP")
                drX("colSurnameName") = drYoyaku("SURNAME_NAME")
                drX("colNinzu") = drYoyaku("JYOSYA_NINZU")

                If Not String.IsNullOrEmpty(drYoyaku("SEISAN_HOHO").ToString) = True Then
                    If drYoyaku("SEISAN_HOHO").ToString = FixedCd.SeisanHouhou.furikomi Then
                        '精算方法が「振込」の場合
                        strSeisanHoho = strSeisanHohoHurikomi

                    ElseIf drYoyaku("SEISAN_HOHO").ToString = FixedCd.SeisanHouhou.eigyousyo Then
                        '精算方法が「営業所」の場合
                        strSeisanHoho = strSeisanHohoEigyosyo

                    ElseIf drYoyaku("SEISAN_HOHO").ToString = FixedCd.SeisanHouhou.toujitsubarai Then
                        '精算方法が「当日払い」の場合
                        strSeisanHoho = strSeisanHohoTojituPayment

                    ElseIf drYoyaku("SEISAN_HOHO").ToString = FixedCd.SeisanHouhou.agt Then
                        '精算方法が「ＡＧＴ」の場合
                        strSeisanHoho = strSeisanHohoAgt

                    Else
                        '精算方法が「カード」の場合
                        strSeisanHoho = strSeisanHohoCard
                    End If
                Else
                    strSeisanHoho = ""
                End If
                ' 発券状態
                ' 共通処理にて取得（パラメータ：キャンセルフラグ、座席指定予約フラグ、発券内容、状態）
                hakkenStateArry = CommonDAUtil.getYoyakuHakkenState(drYoyaku("CANCEL_FLG").ToString,
                                                                drYoyaku("ZASEKI_RESERVE_YOYAKU_FLG").ToString,
                                                                drYoyaku("HAKKEN_NAIYO").ToString,
                                                                drYoyaku("STATE").ToString)
                drX("colSiharaiHoho") = strSeisanHoho
                drX("colYoyakuKbn") = drYoyaku("YOYAKU_KBN")
                drX("colYoyakuNo02") = drYoyaku("YOYAKU_NO")
                drX("colCrsCd") = drYoyaku("CRS_CD")
                drX("colGousya") = drYoyaku("GOUSYA")
                drX("colJotai") = hakkenStateArry(1)

                dtYoyaku.Rows.Add(drX)

                '予約人数を算出
                sumYoyakuNinzu = sumYoyakuNinzu + CInt(drYoyaku("JYOSYA_NINZU"))

            Next

            'グリッドに取得したデータを表示する
            grdYoyaku.DataSource = dtYoyaku

            '予約人数を表示
            txtNinzuTotal.Text = sumYoyakuNinzu.ToString

        End If
    End Sub

    Public Sub setSeFirsttDisplayData() Implements iPT21.setSeFirsttDisplayData
        Throw New NotImplementedException()
    End Sub

    Public Sub DisplayDataToEntity(ByRef ent As iEntity) Implements iPT21.DisplayDataToEntity
        Throw New NotImplementedException()
    End Sub

    Public Sub EntityDataToDisplay(ByRef ent As iEntity) Implements iPT21.EntityDataToDisplay
        Throw New NotImplementedException()
    End Sub

    Public Sub OldDataToEntity(pDataRow As DataRow) Implements iPT21.OldDataToEntity
        Throw New NotImplementedException()
    End Sub

    Public Function CheckSearch() As Boolean Implements iPT21.CheckSearch
        Throw New NotImplementedException()
    End Function

    Public Function CheckInsert() As Boolean Implements iPT21.CheckInsert
        Throw New NotImplementedException()
    End Function

    Public Function CheckUpdate() As Boolean Implements iPT21.CheckUpdate
        Throw New NotImplementedException()
    End Function

    Public Function isExistHissuError() As Boolean Implements iPT21.isExistHissuError
        Throw New NotImplementedException()
    End Function

#End Region

End Class