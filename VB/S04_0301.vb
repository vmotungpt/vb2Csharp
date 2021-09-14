Imports C1.Win.C1FlexGrid
Imports Hatobus.ReservationManagementSystem.Master
Imports Hatobus.ReservationManagementSystem.Yoyaku
Imports System.ComponentModel

''' <summary>
''' S04_0301 乗車状況照会
''' </summary>
Public Class S04_0301
    Inherits FormBase

#Region " 定数／変数宣言 "

#Region "変数"
    Private searchResultGridData As New DataTable                       ' 検索結果グリッドデータ（検索ボタン押下時（入力チェック後））
    Private searchBusUnitData As New DataSet                            ' バス単位の検索結果データ（コース情報グリッド選択時）
    Private searchResultHakkenSituationGridData As New DataTable        ' 発券状況の検索結果グリッドデータ（コース情報グリッド選択時）
    Private searchResultMihakkenYoyakuGridData As New DataTable         ' 未発券予約情報の検索結果グリッドデータ（コース情報グリッド選択時）
    Private operationDate As Date                                       ' オペレーション日
    Private checkDisplayFlg As Boolean = True                          ' 表示/非表示ボタン用フラグ(True:表示, False:非表示)

    '場所マスタエンティティ
    Private clsPlaceMasterEntity As New PlaceMasterEntity()

    '選択行取得キー
    Private EntityKeys As String() = New String() {"CRS_CD", "GOUSYA"}
    Private selectRowSyuptTime As String
    Private selectRowCrsCd As String
    Private selectRowGousya As Integer

#End Region

#Region "定数"
    '発券状況グリッドの最小行数
    Private Const HakkenSituationGridMinRow As Integer = 5
    '発券状況グリッドの最大行数
    Private Const HakkenSituationGridMaxRow As Integer = 10
    '発券状況（明細）グリッドの最大列数
    Private Const HakkenSituationDetailGridMaxCol As Integer = 31
    '発券状況（横計）グリッドの最大列数
    Private Const HakkenSituationYokoKeiGridMaxCol As Integer = 11
    '配車経由地保持の最大列数
    Private Const HaisyaKeiyuMaxCount As Integer = 5
    'Top座標
    Private Const TopLblLengthGrd01 As Integer = 165
    Private Const TopGrdCrs As Integer = 186
    Private Const TopGrdHakkenSituationDetail As Integer = 186
    Private Const TopGrdHakkenSituationYokoKei As Integer = 186
    Private Const TopGrdHakkenSituationTotal As Integer = 186
    Private Const TopGrdHakkenSituationBlock As Integer = 186
    Private Const TopLblLengthGrd02 As Integer = 426
    Private Const TopGrdMihakkenYoyaku As Integer = 447
    Private Const TopLblLengthGrd02BusUnit As Integer = 165
    Private Const TopGrdMihakkenYoyakuBusUnit As Integer = 186
    'グリッド高さ
    Private Const HeightGrdCrs As Integer = 234
    Private Const HeightGrdHakkenSituationDetail As Integer = 234
    Private Const HeightGrdHakkenSituationYokoKei As Integer = 234
    Private Const HeightGrdHakkenSituationTotal As Integer = 234
    Private Const HeightGrdHakkenSituationBlock As Integer = 234
    Private Const HeightGrdMihakkenYoyaku As Integer = 301
    Private Const HeightGrdMihakkenYoyakuBusUnit As Integer = 560
    Private Const HeightCyoseiGrdCrs As Integer = 40
    Private Const HeightCyoseiGrdMihakkenYoyaku As Integer = 82

    ' 最大取得件数
    ' TODO:最大取得件数指針決まり次第修正(仮対応)
    Private Const limitMaxData As Integer = 10000

    'チェックエラー表示項目
    Private Const ErrorDisplaySyuptDay As String = "出発日"
    Private Const ErrorDisplayNoribaCd As String = "乗車地"
    Private Const ErrorDisplaySyuptTime As String = "出発時間"
#End Region

#End Region
#Region "列挙"

    ''' <summary>
    ''' 列定義
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum GrdMihakkenYoyakuColumn As Integer
        <Value("遷移ボタン")>
        TransitionButton = 1
        <Value("コースコード")>
        CrsCd
        <Value("コース名")>
        CrsNm
        <Value("出発時間")>
        SyuptTime
        <Value("号車")>
        Gousya
        <Value("予約番号")>
        YoyakuNumber
        <Value("姓名")>
        SeiMei
        <Value("予約人数")>
        YoyakuNinzu
        <Value("PUルート")>
        PURoot
        <Value("PU")>
        PU
        <Value("PU出発時間")>
        PUSyuptTime
        <Value("乗車人数")>
        JyosyaNinzu
        <Value("状態")>
        State
        <Value("代理店名")>
        AgentNm
        <Value("予約区分")>
        YoyakuKbn
        <Value("予約NO")>
        YoyakuNo
        <Value("コース種別")>
        CrsKind
        <Value("バス指定コード")>
        BusReserveCd

    End Enum

    ''' <summary>
    ''' 行定義
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum GrdHakkenSituationRow As Integer
        <Value("料金区分名")>
        ChargeKbnNm
        <Value("料金区分（人員）名")>
        ChargeKbnJininNm
        <Value("未発券")>
        Mihakken
        <Value("発券済")>
        HakkenZumi
        <Value("縦計")>
        TateKei

    End Enum

    ''' <summary>
    ''' 表示単位
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum DisplayUnit As Integer
        <Value("予約単位")>
        YoyakuUnit
        <Value("バス単位")>
        BusUnit
    End Enum
#End Region

#Region "構造体"
    Public Structure MihakkenYoyakuParameter

        Private _syuptDay As Date
        Private _crsCd As String
        Private _crsNm As String
        Private _gousya As Integer
        Private _yoyakuNumber As String
        Private _seimei As String
        Private _syuptTime As String
        Private _agentNm As String
        Private _yoyakuKbn As String
        Private _yoyakuNo As Integer
        Private _crsKind As String
        Private _busReserveCd As String

        ''' <summary>
        ''' 出発日
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SyuptDay() As Date
            Get
                Return _syuptDay
            End Get
            Set(ByVal value As Date)
                _syuptDay = value
            End Set
        End Property

        ''' <summary>
        ''' コースコード
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CrsCd() As String
            Get
                Return _crsCd
            End Get
            Set(ByVal value As String)
                _crsCd = value
            End Set
        End Property

        ''' <summary>
        ''' コース名
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CrsNm() As String
            Get
                Return _crsNm
            End Get
            Set(ByVal value As String)
                _crsNm = value
            End Set
        End Property

        ''' <summary>
        ''' 号車
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Gousya() As Integer
            Get
                Return _gousya
            End Get
            Set(ByVal value As Integer)
                _gousya = value
            End Set
        End Property

        ''' <summary>
        ''' 予約番号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property YoyakuNumber() As String
            Get
                Return _yoyakuNumber
            End Get
            Set(ByVal value As String)
                _yoyakuNumber = value
            End Set
        End Property

        ''' <summary>
        ''' 姓名
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Seimei() As String
            Get
                Return _seimei
            End Get
            Set(ByVal value As String)
                _seimei = value
            End Set
        End Property

        ''' <summary>
        ''' 出発時間
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SyuptTime() As String
            Get
                Return _syuptTime
            End Get
            Set(ByVal value As String)
                _syuptTime = value
            End Set
        End Property

        ''' <summary>
        ''' 代理店名
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property AgentNm() As String
            Get
                Return _agentNm
            End Get
            Set(ByVal value As String)
                _agentNm = value
            End Set
        End Property

        ''' <summary>
        ''' 予約区分
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property YoyakuKbn() As String
            Get
                Return _yoyakuKbn
            End Get
            Set(ByVal value As String)
                _yoyakuKbn = value
            End Set
        End Property

        ''' <summary>
        ''' 予約NO
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property YoyakuNo() As Integer
            Get
                Return _yoyakuNo
            End Get
            Set(ByVal value As Integer)
                _yoyakuNo = value
            End Set
        End Property

        ''' <summary>
        ''' コース種別
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CrsKind() As String
            Get
                Return _crsKind
            End Get
            Set(ByVal value As String)
                _crsKind = value
            End Set
        End Property

        ''' <summary>
        ''' バス指定コード
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BusReserveCd() As String
            Get
                Return _busReserveCd
            End Get
            Set(ByVal value As String)
                _busReserveCd = value
            End Set
        End Property

    End Structure

#End Region

#Region "イベント"

#Region "表示／非表示ボタン"
    ''' <summary>
    ''' 条件GroupBox表示制御ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnVisiblerCondition_Click(sender As Object, e As EventArgs) Handles btnVisiblerCondition.Click

        'グリッドの座標, サイズを表示/非表示に応じて変更
        If checkDisplayFlg = True Then
            '検索条件を表示状態→非表示状態に変更
            Me.btnVisiblerCondition.Text = "表示 >>"                          'ボタンのキャプション切り替え

            If rdoDisplayTaniYoyaku.Checked = True Then
                '検索条件の表示単位が「予約」の場合
                Me.grdMihakkenYoyaku.Height += Me.gbxCondition.Height         '未発券予約情報グリッドの高さを変更
                Me.lblLengthGrd02.Top -= Me.gbxCondition.Height               '未発券予約情報の件数表示ラベルの位置を変更
                Me.grdMihakkenYoyaku.Top -= Me.gbxCondition.Height            '未発券予約情報グリッドの位置を変更
            Else
                '検索条件の表示単位が「バス」の場合
                Me.grdCrs.Height += HeightCyoseiGrdCrs                        'コース情報グリッドの高さを変更
                Me.grdHakkenSituationDetail.Height += HeightCyoseiGrdCrs      '発券状況（詳細）グリッドの高さを変更
                Me.grdHakkenSituationYokoKei.Height += HeightCyoseiGrdCrs     '発券状況（横計）グリッドの高さを変更
                Me.grdHakkenSituationTotal.Height += HeightCyoseiGrdCrs       '発券状況（合計）グリッドの高さを変更
                Me.grdHakkenSituationBlock.Height += HeightCyoseiGrdCrs       '発券状況（ブロック）グリッドの高さを変更
                Me.grdMihakkenYoyaku.Height += HeightCyoseiGrdMihakkenYoyaku  '未発券予約情報グリッドの高さを変更

                Me.lblLengthGrd01.Top -= Me.gbxCondition.Height               'コース情報の件数表示ラベルの位置を変更
                Me.grdCrs.Top -= Me.gbxCondition.Height                       'コース情報グリッドの位置を変更
                Me.grdHakkenSituationDetail.Top -= Me.gbxCondition.Height     '発券状況（詳細）グリッドの位置を変更
                Me.grdHakkenSituationYokoKei.Top -= Me.gbxCondition.Height    '発券状況（横計）グリッドの位置を変更
                Me.grdHakkenSituationTotal.Top -= Me.gbxCondition.Height      '発券状況（合計）グリッドの位置を変更
                Me.grdHakkenSituationBlock.Top -= Me.gbxCondition.Height      '発券状況（ブロック）グリッドの位置を変更
                Me.lblLengthGrd02.Top -= Me.gbxCondition.Height - HeightCyoseiGrdCrs    '未発券予約情報の件数表示ラベルの位置を変更
                Me.grdMihakkenYoyaku.Top -= Me.gbxCondition.Height - HeightCyoseiGrdCrs '未発券予約情報グリッドの位置を変更
            End If

            Me.gbxCondition.Visible = False
            checkDisplayFlg = False
        Else
            '検索条件を非表示状態→表示状態に変更
            Me.btnVisiblerCondition.Text = "非表示 <<"

            If rdoDisplayTaniYoyaku.Checked = True Then
                '検索条件の表示単位が「予約」の場合
                Me.lblLengthGrd02.Top = TopLblLengthGrd02BusUnit              '未発券予約情報の件数表示ラベルを元の位置に戻す
                Me.grdMihakkenYoyaku.Top = TopGrdMihakkenYoyakuBusUnit        '未発券予約情報グリッドを元の位置に戻す
                Me.grdMihakkenYoyaku.Height = HeightGrdMihakkenYoyakuBusUnit  '未発券予約情報グリッドの高さを元に戻す
            Else
                '検索条件の表示単位が「バス」の場合
                Me.lblLengthGrd01.Top = TopLblLengthGrd01                       'コース情報の件数表示ラベルを元の位置に戻す
                Me.grdCrs.Top = TopGrdCrs                                       'コース情報グリッドを元の位置に戻す
                Me.grdHakkenSituationDetail.Top = TopGrdHakkenSituationDetail   '発券状況（詳細）グリッドを元の位置に戻す
                Me.grdHakkenSituationYokoKei.Top = TopGrdHakkenSituationYokoKei '発券状況（横計）グリッドを元の位置に戻す
                Me.grdHakkenSituationTotal.Top = TopGrdHakkenSituationTotal     '発券状況（合計）グリッドを元の位置に戻す
                Me.grdHakkenSituationBlock.Top = TopGrdHakkenSituationBlock     '発券状況（ブロック）グリッドを元の位置に戻す
                Me.lblLengthGrd02.Top = TopLblLengthGrd02                       '未発券予約情報の件数表示ラベルを元の位置に戻す
                Me.grdMihakkenYoyaku.Top = TopGrdMihakkenYoyaku                 '未発券予約情報グリッドを元の位置に戻す
                Me.grdCrs.Height = HeightGrdCrs                                         'コース情報グリッドの高さを元に戻す
                Me.grdHakkenSituationDetail.Height = HeightGrdHakkenSituationDetail     '発券状況（詳細）グリッドの高さを元に戻す
                Me.grdHakkenSituationYokoKei.Height = HeightGrdHakkenSituationYokoKei   '発券状況（横計）グリッドの高さを元に戻す
                Me.grdHakkenSituationTotal.Height = HeightGrdHakkenSituationTotal       '発券状況（合計）グリッドの高さを元に戻す
                Me.grdHakkenSituationBlock.Height = HeightGrdHakkenSituationBlock       '発券状況（ブロック）グリッドの高さを元に戻す
                Me.grdMihakkenYoyaku.Height = HeightGrdMihakkenYoyaku                   '未発券予約情報グリッドの高さを元に戻す
            End If
            Me.gbxCondition.Visible = True
            checkDisplayFlg = True
        End If
    End Sub

#End Region

#Region "   条件クリア"
    ''' <summary>
    ''' 条件クリアボタン押下時
    ''' </summary>
    Protected Overrides Sub btnCLEAR_ClickOrgProc()
        ' 検索条件部の項目初期化
        initSearchAreaItems()
    End Sub

#End Region

#Region "   検索ボタン"
    ''' <summary>
    ''' F8：検索ボタン押下時の独自処理
    ''' </summary>
    Protected Overrides Sub btnF8_ClickOrgProc()
        'Dim logmsg(0) As String
        '選択された行データ
        Dim selectData() As DataRow = Nothing
        '問合せ文字列（明細グリッド用）
        Dim whereString As String = String.Empty

        'データテーブルの初期化
        searchResultGridData = Nothing
        'チェックして検索
        If checkSearchItems() = False Then
            Exit Sub
        End If

        '対象データ取得
        searchResultGridData = getMstData()

        If searchResultGridData Is Nothing OrElse searchResultGridData.Rows.Count <= 0 Then
            ' 取得件数0件の場合、メッセージを表示
            CommonProcess.createFactoryMsg().messageDisp("E90_019")

            ' グリッドの設定
            initGrid()
        Else
            '最大取得件数で絞込み
            searchResultGridData = CommonDAUtil.checkLimitData(searchResultGridData, limitMaxData)
            ' 検索後処理
            reloadGridSearch()
        End If

    End Sub

#End Region

#Region "フォーム"
    Private Sub F8_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyData
            Case Keys.F8
                Me.btnSearch.Select()
                MyBase.btnCom_Click(Me.btnSearch, e)
            Case Else
                Exit Sub
        End Select
    End Sub

#End Region

#Region "   グリッドイベント"
    ''' <summary>
    ''' グリッドのデータソース変更時イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub grdMihakkenYoyaku_AfterDataRefresh(sender As Object, e As ListChangedEventArgs) Handles grdMihakkenYoyaku.AfterDataRefresh
        Dim grd As FlexGridEx = TryCast(sender, FlexGridEx)
        If grd Is Nothing Then
            Return
        End If

        'データ件数を表示
        ClientCommonKyushuUtil.setGridCount(grdMihakkenYoyaku, lblLengthGrd02)
    End Sub

    ''' <summary>
    ''' グリッドのデータソース変更時イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub grdCrs_AfterDataRefresh(sender As Object, e As ListChangedEventArgs) Handles grdCrs.AfterDataRefresh
        Dim grd As FlexGridEx = TryCast(sender, FlexGridEx)
        If grd Is Nothing Then
            Return
        End If

        'データ件数を表示
        ClientCommonKyushuUtil.setGridCount(grdCrs, lblLengthGrd01)
    End Sub
    ''' <summary>
    ''' 行選択時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub grdCrs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdCrs.Click

        'DBパラメータ
        Dim paramInfoListHakkenSituation As New Hashtable
        Dim paramInfoListMihakkenYoyakuInfo As New Hashtable
        'DataAccessクラス生成
        Dim dataAccess As New JyosyaSituationInquiry_DA

        searchBusUnitData = Nothing
        searchResultHakkenSituationGridData = Nothing
        searchResultMihakkenYoyakuGridData = Nothing

        '非表示化
        grdMihakkenYoyaku.Visible = False
        grdHakkenSituationDetail.Visible = False
        grdHakkenSituationYokoKei.Visible = False
        grdHakkenSituationTotal.Visible = False
        grdHakkenSituationBlock.Visible = False
        'lblLengthGrd01.Visible = False
        lblLengthGrd02.Visible = False

        '選択行データ取得
        getSelectedRowData()

        '発券状況検索用パラメータ設定処理
        paramInfoListHakkenSituation = setParameterHakkenSituation()
        '未発券予約情報検索用パラメータ設定処理
        paramInfoListMihakkenYoyakuInfo = setParameterMihakkenYoyaku()
        'バス単位検索処理
        searchBusUnitData = dataAccess.getBusUnitSearch(paramInfoListHakkenSituation, paramInfoListMihakkenYoyakuInfo)
        'データセットから各データテーブルへ設定
        searchResultHakkenSituationGridData = searchBusUnitData.Tables(0)
        searchResultMihakkenYoyakuGridData = searchBusUnitData.Tables(1)

        If searchResultHakkenSituationGridData.Rows.Count <= 0 OrElse
            (searchResultHakkenSituationGridData.Rows.Count > 0 AndAlso
            String.IsNullOrEmpty(searchResultHakkenSituationGridData.Rows(0)("JYOCHACHI_CD_1").ToString()) = True) Then
            ' 発券状況0件の場合、メッセージを表示
            '「該当データが存在しません。」のエラーを表示
            CommonProcess.createFactoryMsg().messageDisp("E90_019")
        Else
            If searchResultMihakkenYoyakuGridData.Rows.Count <= 0 Then
                ' 未発券予約情報0件の場合、メッセージを表示
                '「未発券の予約はありません。」のエラーを表示
                CommonProcess.createFactoryMsg().messageDisp("E04_002")
            Else
                '最大取得件数で絞込み
                searchResultMihakkenYoyakuGridData = CommonDAUtil.checkLimitData(searchResultMihakkenYoyakuGridData, limitMaxData)
            End If
            'コース情報グリッド選択時グリッド表示
            reloadGridCourseInfoGridSelect()
        End If

    End Sub
    ''' <summary>
    ''' 詳細ボタン押下イベント
    ''' </summary>
    ''' <param name="sender">イベント送信元</param>
    ''' <param name="e">イベントデータ</param>
    Private Sub btnGridRow_Click(ByVal sender As Object, ByVal e As C1.Win.C1FlexGrid.RowColEventArgs) Handles grdMihakkenYoyaku.CellButtonClick
        Dim transferParameter As New MihakkenYoyakuParameter
        Dim idx As Integer = 0

        '引渡パラメータ設定
        transferParameter.SyuptDay = CType(dtmSyuptDay.Value, Date)
        'コースコード
        transferParameter.CrsCd = CType(grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.CrsCd), String)
        'コース名
        transferParameter.CrsNm = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.CrsNm).ToString()
        '出発時間
        transferParameter.SyuptTime = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.SyuptTime).ToString()
        '号車
        transferParameter.Gousya = CType(grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.Gousya), Integer)
        '予約番号
        transferParameter.YoyakuNumber = CType(grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.YoyakuNumber), String)
        '姓名
        transferParameter.Seimei = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.SeiMei).ToString()
        '代理店名
        transferParameter.AgentNm = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.AgentNm).ToString()
        '予約区分
        transferParameter.YoyakuKbn = CType(grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.YoyakuKbn), String)
        '予約NO
        transferParameter.YoyakuNo = CType(grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.YoyakuNo), Integer)
        'コース種別
        transferParameter.CrsKind = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.CrsKind).ToString()
        'バス指定コード
        transferParameter.BusReserveCd = grdMihakkenYoyaku.GetData(e.Row, GrdMihakkenYoyakuColumn.BusReserveCd).ToString()

        Using form As New S04_0302()
            '引渡パラメータ設定
            form.setTransferParameter(transferParameter)
            openWindow(form, True)
        End Using
    End Sub

#End Region

#Region "フッタ"

    ''' <summary>
    ''' F2：戻るボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF2_ClickOrgProc()
        Me.Close()
    End Sub

#End Region

#End Region

#Region "メソッド"

#Region "初期化処理"

    ''' <summary>
    ''' フォーム起動時の独自処理
    ''' </summary>
    Protected Overrides Sub StartupOrgProc()
        'オペレーション日取得
        operationDate = CommonDateUtil.getSystemTime()
        ' 検索条件部の設定
        initSearchAreaItems()
        ' グリッドの設定
        initGrid()
        ' フッタボタンの設定
        setButtonInitialize()

        '検索・条件クリアボタンの関連付け
        AddHandler btnSearch.Click, AddressOf MyBase.btnCom_Click
        AddHandler btnClear.Click, AddressOf MyBase.btnCom_Click
    End Sub

    ''' <summary>
    ''' 検索条件部の項目初期化
    ''' </summary>
    Private Sub initSearchAreaItems()

        '戻り値
        Dim returnValue As DataTable = Nothing

        'コントロール初期化
        CommonUtil.Control_Init(Me.gbxCondition.Controls)
        MyBase.clearExistErrorProperty(Me.gbxCondition.Controls)

        '出発日の表示
        dtmSyuptDay.Value = CommonDateUtil.getSystemTime()

        '場所コード・名称の表示
        returnValue = GetDbTablePlaceMaster()
        If returnValue.Rows.Count > 0 Then
            With clsPlaceMasterEntity
                ucoNoribaCd.CodeText = CType(returnValue.Rows(0)(.PlaceCd.PhysicsName), String)
                ucoNoribaCd.ValueText = CType(returnValue.Rows(0)(.PlaceName1.PhysicsName), String)
            End With
        End If

        Me.dtmSyuptTimeFromTo.FromTimeValue24 = Nothing  ' 出発時間From
        Me.dtmSyuptTimeFromTo.ToTimeValue24 = Nothing    ' 出発時間To

        '初期フォーカスのコントロールを設定を実装
        Me.dtmSyuptDay.Select()
    End Sub

    ''' <summary>
    ''' フッタボタンの設定
    ''' </summary>
    Private Sub setButtonInitialize()

        'Visible
        Me.F2Key_Visible = True

        Me.F1Key_Visible = False
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

        'Enabled
        Me.F2Key_Enabled = True

        'Text
        Me.F2Key_Text = "F2:戻る"

    End Sub

#End Region

#Region "チェック処理"

    ''' <summary>
    ''' 検索条件項目チェック
    ''' </summary>
    Private Function checkSearchItems() As Boolean
        '判定結果
        Dim returnResult As Boolean = True
        'フォーカスセットフラグ
        Dim focusSetFlg As Boolean = False
        'エラー表示項目
        Dim errorDisplay As String = String.Empty

        '戻り値
        Dim returnValue As DataTable = Nothing

        'searchGridData = Nothing

        'エラーの初期化
        MyBase.clearExistErrorProperty(Me.gbxCondition.Controls)

        '必須チェック
        If CommonUtil.checkHissuError(Me.gbxCondition.Controls) = True Then
            CommonProcess.createFactoryMsg().messageDisp("E90_022")
            '必須エラーフォーカス設定（エラーが発生した先頭にフォーカスを当てる）
            If dtmSyuptDay.ExistError = True Then
                focusSetFlg = True
                dtmSyuptDay.Focus()
            End If

            If focusSetFlg = False Then
                If ucoNoribaCd.ExistError = True Then
                    ucoNoribaCd.Focus()
                End If
            End If
            Return False
        End If

        '出発時間の大小チェック
        If IsNothing(Me.dtmSyuptTimeFromTo.FromTimeValue24) = False AndAlso IsNothing(Me.dtmSyuptTimeFromTo.ToTimeValue24) = False Then
            If dtmSyuptTimeFromTo.FromTimeValue24Int > dtmSyuptTimeFromTo.ToTimeValue24Int Then
                dtmSyuptTimeFromTo.ExistErrorForFromTime = True
                dtmSyuptTimeFromTo.ExistErrorForToTime = True
                dtmSyuptTimeFromTo.Focus()
                '「{1}の設定が不正です。」のエラーを表示
                CommonProcess.createFactoryMsg().messageDisp("E90_017", ErrorDisplaySyuptTime)
                Return False
            End If
        End If

        Return returnResult
    End Function

#End Region

    ''' <summary>
    ''' 対象マスタのデータ取得
    ''' </summary>
    Private Function getMstData() As DataTable
        'DBパラメータ
        Dim paramInfoList As New Hashtable
        'DataAccessクラス生成
        Dim dataAccess As New JyosyaSituationInquiry_DA

        If rdoDisplayTaniYoyaku.Checked = True Then
            ' 検索条件部の表示単位で「予約」を選択時
            ' 未発券予約情報検索
            paramInfoList = setParameterMihakkenYoyaku()
            Return dataAccess.getMihakkenYoyaku(paramInfoList)
        Else
            ' 検索条件部の表示単位で「バス」を選択時
            ' コース情報検索
            Return getDbTableCourseInformation()

        End If
    End Function

#Region "DB検索処理"

    ''' <summary>
    ''' 場所コード・名称検索処理
    ''' </summary>
    ''' <returns>取得データ(DataTable)</returns>
    Private Function GetDbTablePlaceMaster() As DataTable
        '戻り値
        Dim returnValue As DataTable = Nothing
        'DBパラメータ
        Dim paramInfoList As New Hashtable

        'DataAccessクラス生成
        Dim dataAccess As New Place_DA

        'パラメータ設定
        With clsPlaceMasterEntity
            '会社コード
            paramInfoList.Add(.CompanyCd.PhysicsName, UserInfoManagement.companyCd)
            '営業所コード
            paramInfoList.Add(.EigyosyoCd.PhysicsName, UserInfoManagement.eigyosyoCd)
        End With


        Try

            '場所マスタ検索
            returnValue = dataAccess.GetPlaceMasterDataLoginUser(paramInfoList)
        Catch ex As OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

    ''' <summary>
    ''' コース情報検索処理
    ''' </summary>
    ''' <returns>取得データ(DataTable)</returns>
    Private Function getDbTableCourseInformation() As DataTable
        '戻り値
        Dim returnValue As DataTable = Nothing
        'DBパラメータ
        Dim paramInfoList As New Hashtable
        'DataAccessクラス生成
        Dim dataAccess As New JyosyaSituationInquiry_DA

        'コース台帳（基本）エンティティ
        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity()

        'パラメータ設定
        With clsCrsLedgerBasicEntity
            '出発日
            paramInfoList.Add(.syuptDay.PhysicsName, CType(CType(dtmSyuptDay.Value, String).Replace("/", ""), Int32))
            '乗車地コード
            paramInfoList.Add(.haisyaKeiyuCd1.PhysicsName, ucoNoribaCd.CodeText)
        End With
        '出発時間（From）
        If Not dtmSyuptTimeFromTo.FromTimeValue24 Is Nothing Then
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamSyuptTimeFrom, dtmSyuptTimeFromTo.FromTimeValue24Int.ToString())
        End If
        '出発時間（To）
        If Not dtmSyuptTimeFromTo.ToTimeValue24 Is Nothing Then
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamSyuptTimeTo, dtmSyuptTimeFromTo.ToTimeValue24Int.ToString())
        End If

        '共通パラメータ設定
        setCommonParameter(paramInfoList)

        Try
            'コース情報検索
            returnValue = dataAccess.getCourseInformation(paramInfoList)
        Catch ex As OracleException
            Throw
        Catch ex As Exception
            Throw
        End Try

        Return returnValue

    End Function

#Region "DBパラメータ設定"
    ''' <summary>
    ''' 未発券予約情報検索用パラメータ設定処理
    ''' </summary>
    ''' <returns>検索用パラメータ</returns>
    Private Function setParameterMihakkenYoyaku() As Hashtable
        'DBパラメータ
        Dim paramInfoList As New Hashtable

        'コース台帳（基本）エンティティ
        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity()
        'コース台帳（ダイヤ）エンティティ
        Dim clsCrsLedgerDiaEntity As New CrsLedgerDiaEntity()
        'ピックアップルート台帳 （ホテル）エンティティ
        Dim clsPickupRouteLedgerHotelEntity As New PickupRouteLedgerHotelEntity()

        'パラメータ設定
        '検索条件部の項目より設定
        With clsCrsLedgerDiaEntity
            '出発日
            paramInfoList.Add(.syuptDay.PhysicsName, CType(CType(dtmSyuptDay.Value, String).Replace("/", ""), Int32))
            '乗車地コード
            paramInfoList.Add(.jyochachiCd.PhysicsName, ucoNoribaCd.CodeText)
        End With
        '出発時間（From）
        If Not dtmSyuptTimeFromTo.FromTimeValue24 Is Nothing Then
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamSyuptTimeFrom, dtmSyuptTimeFromTo.FromTimeValue24Int.ToString())
        End If
        '出発時間（To）
        If Not dtmSyuptTimeFromTo.ToTimeValue24 Is Nothing Then
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamSyuptTimeTo, dtmSyuptTimeFromTo.ToTimeValue24Int.ToString())
        End If

        '共通パラメータ設定
        setCommonParameter(paramInfoList)

        'オペレーション日
        paramInfoList.Add(clsPickupRouteLedgerHotelEntity.startDay.PhysicsName, operationDate)

        '検索条件の表示単位が「バス」の場合はグリッド選択行の値をパラメータに設定
        If rdoDisplayTaniYoyaku.Checked = True Then
            paramInfoList.Add("DisplayUnit", DisplayUnit.YoyakuUnit)
        Else
            paramInfoList.Add("DisplayUnit", DisplayUnit.BusUnit)

            'コース情報グリッドの項目より設定
            '出発時間
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamSyuptTimeGrid, selectRowSyuptTime)
            'コースコード
            paramInfoList.Add(JyosyaSituationInquiry_DA.ParamCrsCdGrid, selectRowCrsCd)
            '号車
            paramInfoList.Add(clsCrsLedgerBasicEntity.gousya.PhysicsName, selectRowGousya)
        End If

        Return paramInfoList

    End Function

    ''' <summary>
    ''' 共通パラメータ設定処理
    ''' </summary>
    ''' <param name="paramList">パラメータリスト</param>
    Private Sub setCommonParameter(ByRef paramList As Hashtable)

        'コース台帳（基本）エンティティ
        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity()

        'パラメータ設定
        With clsCrsLedgerBasicEntity
            'コースコード
            paramList.Add(.crsCd.PhysicsName, ucoCrsCd.CodeText)
            'コース種別
            If chkCrsKindJapanese.Checked = True AndAlso chkCrsKindGaikokugo.Checked = True Then
                '日本語、外国語両方選択されている場合はパラメータ未設定
                paramList.Add(.houjinGaikyakuKbn.PhysicsName, String.Empty)
            Else
                If chkCrsKindJapanese.Checked = True Then
                    paramList.Add(.houjinGaikyakuKbn.PhysicsName, HoujinGaikyakuKbnType.Houjin)
                ElseIf chkCrsKindGaikokugo.Checked = True Then
                    paramList.Add(.houjinGaikyakuKbn.PhysicsName, HoujinGaikyakuKbnType.Gaikyaku)
                Else
                    paramList.Add(.houjinGaikyakuKbn.PhysicsName, String.Empty)
                End If
            End If
            '定期企画区分
            If chkTeikiKikakuKbnTeiki.Checked = True AndAlso chkTeikiKikakuKbnKikaku.Checked = True Then
                '定期、企画両方選択されている場合はパラメータ未設定
                paramList.Add(.teikiKikakuKbn.PhysicsName, String.Empty)
            Else
                If chkTeikiKikakuKbnTeiki.Checked = True Then
                    paramList.Add(.teikiKikakuKbn.PhysicsName, CInt(FixedCd.TeikiKikakuKbn.Teiki))

                ElseIf chkTeikiKikakuKbnKikaku.Checked = True Then
                    paramList.Add(.teikiKikakuKbn.PhysicsName, CInt(FixedCd.TeikiKikakuKbn.Kikaku))
                Else
                    paramList.Add(.teikiKikakuKbn.PhysicsName, String.Empty)
                End If
            End If
        End With

    End Sub

    ''' <summary>
    ''' 発券状況検索用パラメータ設定処理
    ''' </summary>
    ''' <returns>検索用パラメータ</returns>
    Private Function setParameterHakkenSituation() As Hashtable

        'DBパラメータ
        Dim paramInfoList As New Hashtable

        'DataAccessクラス生成
        Dim dataAccess As New JyosyaSituationInquiry_DA

        'コース台帳（基本）エンティティ
        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity()

        'パラメータ設定
        '出発日
        '検索条件部の項目より設定
        paramInfoList.Add(clsCrsLedgerBasicEntity.syuptDay.PhysicsName, CType(CType(dtmSyuptDay.Value, String).Replace("/", ""), Int32))

        'コース情報グリッドの項目より設定
        'コースコード
        paramInfoList.Add(JyosyaSituationInquiry_DA.ParamCrsCdGrid, selectRowCrsCd)
        '号車
        paramInfoList.Add(clsCrsLedgerBasicEntity.gousya.PhysicsName, selectRowGousya)

        Return paramInfoList

    End Function
#End Region

#Region "グリッド関連"

    ''' <summary>
    ''' グリッド初期設定
    ''' </summary>
    Private Sub initGrid()

        '非表示化
        grdMihakkenYoyaku.Visible = False
        grdCrs.Visible = False
        grdHakkenSituationDetail.Visible = False
        grdHakkenSituationYokoKei.Visible = False
        grdHakkenSituationTotal.Visible = False
        grdHakkenSituationBlock.Visible = False
        lblLengthGrd01.Visible = False
        lblLengthGrd02.Visible = False

    End Sub

    ''' <summary>
    ''' 選択行のデータを取得
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub getSelectedRowData()
        '選択された行データ
        Dim selectData() As DataRow = Nothing
        '問合せ文字列
        Dim whereString As String = String.Empty

        'コース台帳（基本）エンティティ
        Dim clsCrsLedgerBasicEntity As New CrsLedgerBasicEntity()

        '引数の設定
        whereString = CommonUnkouUtil.MakeWhere(grdCrs, EntityKeys)

        '問合せ対象データ取得
        selectData = searchResultGridData.Select(whereString)

        If selectData.Length > 0 Then
            With clsCrsLedgerBasicEntity
                selectRowSyuptTime = Replace(selectData(0).Item("SYUPT_TIME").ToString, ":", "")
                selectRowCrsCd = selectData(0).Item(.crsCd.PhysicsName).ToString
                selectRowGousya = CType(selectData(0).Item(.gousya.PhysicsName), Integer)
            End With
        End If
    End Sub

    ''' <summary>
    ''' 検索ボタン押下時グリッド表示
    ''' </summary>
    Private Sub reloadGridSearch()

        If rdoDisplayTaniYoyaku.Checked = True Then
            ' 検索条件部の表示単位で「予約」を選択時
            ' 未発券予約情報グリッドへの設定
            setMihakkenYoyakuInfo(searchResultGridData)
        Else
            ' 検索条件部の表示単位で「バス」を選択時
            ' コース情報グリッドへの設定
            Me.grdCrs.DataSource = searchResultGridData
        End If

        ' グリッド初期設定
        initGrid()

        ' 検索ボタン押下時グリッド初期設定
        initGridSearch()
    End Sub

    ''' <summary>
    ''' コース情報グリッド選択時グリッド表示
    ''' </summary>
    Private Sub reloadGridCourseInfoGridSelect()

        If searchResultMihakkenYoyakuGridData.Rows.Count > 0 Then
            ' 未発券予約情報グリッドへの設定
            setMihakkenYoyakuInfo(searchResultMihakkenYoyakuGridData)
        Else
            ''非表示化
            'grdMihakkenYoyaku.Visible = False
            'lblLengthGrd02.Visible = False
        End If

        ' 発券状況グリッドへの設定
        setHakkenSituationDetailGrid()

        ' コース情報グリッド選択時グリッド初期設定
        initGridCourseInfoGridSelect()
    End Sub

    ''' <summary>
    ''' 検索ボタン押下時グリッド初期設定
    ''' </summary>
    Private Sub initGridSearch()
        Dim saveIdx As Integer = 0
        Dim count As Integer = 1

        If rdoDisplayTaniYoyaku.Checked = True Then
            ' 検索条件部の表示単位で「予約」を選択時

            '未発券予約情報グリッド初期設定
            initMihakkenYoyakuGrid(165, 186, 560)

            'セルボタン配置（同一の予約番号が続いた場合は1行目のみにボタンを表示）
            ClientCommonKyushuUtil.setGridCellButton(grdMihakkenYoyaku, 6, 1)
        Else
            ' 検索条件部の表示単位で「バス」を選択時

            'コース情報グリッド初期設定
            'ヘッダ設定
            grdCrs.SetData(grdCrs.GetCellRange(0, 1), "コース" & vbCrLf & "コード")
            grdCrs.Rows(0).Height = 37

            '行選択モード
            grdCrs.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row
            '並び替え不可
            grdCrs.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None
            '編集不可
            grdCrs.AllowEditing = False
            '自動列生成なし
            grdCrs.AutoGenerateColumns = False
            'グリッド表示
            grdCrs.Visible = True
            '件数ラベル表示
            lblLengthGrd01.Visible = True

        End If

    End Sub

    ''' <summary>
    ''' コース情報グリッド選択時グリッド初期設定
    ''' </summary>
    Private Sub initGridCourseInfoGridSelect()

        ''発券状況グリッド
        '行選択モード
        grdHakkenSituationDetail.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row
        grdHakkenSituationYokoKei.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row
        grdHakkenSituationTotal.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row
        grdHakkenSituationBlock.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row
        '並び替え不可
        grdHakkenSituationDetail.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None
        grdHakkenSituationYokoKei.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None
        grdHakkenSituationTotal.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None
        grdHakkenSituationBlock.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None
        '編集不可
        grdHakkenSituationDetail.AllowEditing = False
        grdHakkenSituationYokoKei.AllowEditing = False
        grdHakkenSituationTotal.AllowEditing = False
        grdHakkenSituationBlock.AllowEditing = False
        '自動列生成なし
        grdHakkenSituationDetail.AutoGenerateColumns = False
        grdHakkenSituationYokoKei.AutoGenerateColumns = False
        grdHakkenSituationTotal.AutoGenerateColumns = False
        grdHakkenSituationBlock.AutoGenerateColumns = False
        'ソート不可
        grdHakkenSituationDetail.AllowSorting = CType(False, C1.Win.C1FlexGrid.AllowSortingEnum)
        grdHakkenSituationYokoKei.AllowSorting = CType(False, C1.Win.C1FlexGrid.AllowSortingEnum)
        grdHakkenSituationTotal.AllowSorting = CType(False, C1.Win.C1FlexGrid.AllowSortingEnum)
        grdHakkenSituationBlock.AllowSorting = CType(False, C1.Win.C1FlexGrid.AllowSortingEnum)
        'グリッド表示
        grdHakkenSituationDetail.Visible = True
        grdHakkenSituationYokoKei.Visible = True
        grdHakkenSituationTotal.Visible = True
        grdHakkenSituationBlock.Visible = True

        ''固定ヘッダ設定
        '明細グリッド
        grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(0, 0), "料金区分")
        grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(1, 0), "料金区分（人員）")
        grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(2, 0), "未発券")
        grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(3, 0), "発券済")
        grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(4, 0), "縦計")

        '横計グリッド
        For idx As Integer = 1 To grdHakkenSituationYokoKei.Cols.Count - 1
            grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(0, idx), "横計")
        Next

        ''文字位置設定
        '行ヘッダ（左寄せ）
        grdHakkenSituationDetail.Cols(0).TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.LeftCenter

        '明細グリッド
        For colIdx As Integer = 1 To grdHakkenSituationDetail.Cols.Count - 1
            'ヘッダ（中央寄せ）
            grdHakkenSituationDetail.Cols(colIdx).TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter
            'データセル（右寄せ）
            grdHakkenSituationDetail.Cols(colIdx).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.RightCenter
        Next
        '横計グリッド
        For colIdx As Integer = 1 To grdHakkenSituationYokoKei.Cols.Count - 1
            'ヘッダ（中央寄せ）
            grdHakkenSituationYokoKei.Cols(colIdx).TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter
            'データセル（右寄せ）
            grdHakkenSituationYokoKei.Cols(colIdx).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.RightCenter
        Next

        ''セル結合設定
        ' 料金区分の行で隣接するセルが同一の値の場合にマージする
        grdHakkenSituationDetail.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.FixedOnly
        grdHakkenSituationDetail.AllowMergingFixed = C1.Win.C1FlexGrid.AllowMergingEnum.Free
        grdHakkenSituationDetail.Rows(0).AllowMerging = True

        '発券状況グリッド（横計）のヘッダセル結合
        grdHakkenSituationYokoKei.AllowMergingFixed = C1.Win.C1FlexGrid.AllowMergingEnum.Free
        grdHakkenSituationYokoKei.Rows(0).AllowMerging = True

        '発券状況グリッド（合計）のヘッダセル結合
        grdHakkenSituationTotal.AllowMergingFixed = C1.Win.C1FlexGrid.AllowMergingEnum.Custom
        grdHakkenSituationTotal.Cols(1).AllowMerging = True
        grdHakkenSituationTotal.MergedRanges.Add(grdHakkenSituationTotal.GetCellRange(0, 1, 1, 1))

        '発券状況グリッド（営業所ブロック）のヘッダセル結合
        grdHakkenSituationBlock.AllowMergingFixed = C1.Win.C1FlexGrid.AllowMergingEnum.Custom
        grdHakkenSituationBlock.Cols(1).AllowMerging = True
        grdHakkenSituationBlock.MergedRanges.Add(grdHakkenSituationBlock.GetCellRange(0, 1, 1, 1))

        '背景色設定
        grdHakkenSituationBlock.Styles.Add("BackColorStyle")
        grdHakkenSituationBlock.Styles("BackColorStyle").BackColor = System.Drawing.SystemColors.ControlLight
        For i As Integer = 2 To 3
            grdHakkenSituationBlock.Rows(i).Style = grdHakkenSituationBlock.Styles("BackColorStyle")
        Next
        For i As Integer = 5 To grdHakkenSituationBlock.Rows.Count - 1
            grdHakkenSituationBlock.Rows(i).Style = grdHakkenSituationBlock.Styles("BackColorStyle")
        Next

        ''未発券予約情報グリッド
        ''コントロール配置設定
        If searchResultMihakkenYoyakuGridData.Rows.Count > 0 Then
            '検索条件の表示状態により未発券予約情報の件数ラベル、グリッドの座標, サイズを表示/非表示に応じて変更
            If checkDisplayFlg = True Then
                '検索条件表示状態
                initMihakkenYoyakuGrid(TopLblLengthGrd02, TopGrdMihakkenYoyaku, HeightGrdMihakkenYoyaku)
            Else
                '検索条件非表示状態
                initMihakkenYoyakuGrid(TopLblLengthGrd02 - Me.gbxCondition.Height + HeightCyoseiGrdCrs, TopGrdMihakkenYoyaku - Me.gbxCondition.Height + HeightCyoseiGrdCrs, HeightGrdMihakkenYoyaku + HeightCyoseiGrdMihakkenYoyaku)
            End If

            'セルボタン配置（同一の予約番号が続いた場合は1行目のみにボタンを表示）
            ClientCommonKyushuUtil.setGridCellButton(grdMihakkenYoyaku, GrdMihakkenYoyakuColumn.YoyakuNumber, GrdMihakkenYoyakuColumn.TransitionButton)
        End If

    End Sub

    ''' <summary>
    ''' 未発券予約情報表示
    ''' </summary>
    ''' <param name="paramGridData">データテーブル</param>
    Private Sub setMihakkenYoyakuInfo(paramGridData As DataTable)
        Dim editYoyakuNumber As String = String.Empty
        Dim idxRow As Integer = 1
        Dim hakkenStateArry As String() = Nothing

        '予約情報（基本）エンティティ
        Dim clsYoyakuInfoBasicEntity As New YoyakuInfoBasicEntity()

        For Each row As DataRow In paramGridData.Rows
            '予約番号をカンマ編集後設定
            With clsYoyakuInfoBasicEntity
                If Not String.IsNullOrEmpty(row(.yoyakuKbn.PhysicsName).ToString()) Then
                    ' 共通処理にてカンマ編集（パラメータ：予約区分、予約NOを文字列結合した値）
                    'editYoyakuNumber = CommonCheckUtil.editYoyakuNo(row(.yoyakuKbn.PhysicsName).ToString() & CType(String.Format("{0:D9}", CType(row(.yoyakuNo.PhysicsName).ToString(), Integer)), String))
                    editYoyakuNumber = CommonRegistYoyaku.createManagementNumber(row(.yoyakuKbn.PhysicsName).ToString(), CType(row(.yoyakuNo.PhysicsName).ToString(), Integer))
                    row("YOYAKU_NUMBER") = editYoyakuNumber
                End If
                '状態に表示する文言を取得後設定
                ' 共通処理にて取得（パラメータ：キャンセルフラグ、座席指定予約フラグ、発券内容、状態）
                hakkenStateArry = CommonDAUtil.getYoyakuHakkenState(row(.cancelFlg.PhysicsName).ToString(),
                                                                row(.zasekiReserveYoyakuFlg.PhysicsName).ToString(),
                                                                row(.hakkenNaiyo.PhysicsName).ToString(),
                                                                row(.state.PhysicsName).ToString())
                row("STATE_NAME") = hakkenStateArry(1)

                idxRow += 1
            End With
        Next

        ' 未発券予約情報グリッドへのデータバインド
        Me.grdMihakkenYoyaku.DataSource = paramGridData

    End Sub

    ''' <summary>
    ''' 未発券予約情報グリッド初期設定
    ''' </summary>
    ''' <param name="pramLabelYLocation">件数ラベルY座標</param>
    ''' <param name="pramGridYLocation">グリッドY座標</param>
    ''' <param name="pramGridHeight">グリッド高さ</param>
    Private Sub initMihakkenYoyakuGrid(pramLabelYLocation As Integer, pramGridYLocation As Integer, pramGridHeight As Integer)

        '行選択モード
        grdMihakkenYoyaku.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row
        '並び替え不可
        grdMihakkenYoyaku.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None
        '編集不可
        For i As Integer = GrdMihakkenYoyakuColumn.CrsCd To GrdMihakkenYoyakuColumn.AgentNm
            grdMihakkenYoyaku.Cols(i).AllowEditing = False
        Next
        '自動列生成なし
        grdMihakkenYoyaku.AutoGenerateColumns = False
        'グリッド表示
        grdMihakkenYoyaku.Visible = True
        '件数ラベル表示
        lblLengthGrd02.Visible = True

        '未発券予約情報の件数ラベルの位置を移動
        Me.lblLengthGrd02.Location = New System.Drawing.Point(1185, pramLabelYLocation)
        '未発券予約情報グリッドの位置を移動
        Me.grdMihakkenYoyaku.Location = New System.Drawing.Point(8, pramGridYLocation)
        '未発券予約情報グリッドの高さを変更
        Me.grdMihakkenYoyaku.Height = pramGridHeight

        'ヘッダ設定
        Dim range = grdMihakkenYoyaku.GetCellRange(0, 1)
        grdMihakkenYoyaku.Rows(0).Height = 37
        grdMihakkenYoyaku.SetData(grdMihakkenYoyaku.GetCellRange(0, GrdMihakkenYoyakuColumn.CrsCd), "コース" & vbCrLf & "コード")
        grdMihakkenYoyaku.SetData(grdMihakkenYoyaku.GetCellRange(0, GrdMihakkenYoyakuColumn.SyuptTime), "出発" & vbCrLf & "時間")
        grdMihakkenYoyaku.SetData(grdMihakkenYoyaku.GetCellRange(0, GrdMihakkenYoyakuColumn.PURoot), "PU" & vbCrLf & "ルート")
        grdMihakkenYoyaku.SetData(grdMihakkenYoyaku.GetCellRange(0, GrdMihakkenYoyakuColumn.PUSyuptTime), "PU" & vbCrLf & "出発時間")

    End Sub

    ''' <summary>
    ''' 発券状況グリッド設定
    ''' </summary>
    Private Sub setHakkenSituationDetailGrid()
        '選択された行データ
        Dim selectData() As DataRow = Nothing
        '問合せ文字列（明細グリッド用）
        Dim whereString As String = String.Empty
        'グリッド列数
        Dim colCount As Integer = 0
        '縦計
        Dim tateKei As Integer = 0
        '発券計
        Dim hakkenKei As Integer = 0
        '配車経由地表示フラグ
        Dim displayHaisyaKeiyuchiFlag As Boolean = False
        '発券状況データ件数
        Dim hakkenDetailCount As Integer = 0
        '未発券人数
        Dim miHakkenNinzuCount As Integer = 0
        '発券済人数
        Dim hakkenZumiNinzuCount As Integer = 0
        'キー比較結果フラグ
        Dim sameValueFlag As Boolean = False
        '発券状況データインデックス
        Dim gridRowIdx As Integer = 0
        '合計グリッド集計用
        Dim totalCount As Integer = 0

        Try
            'グリッド初期化
            '明細グリッド初期化
            initHakkenSituationGrid(grdHakkenSituationDetail, 0)
            '横計グリッド初期化
            initHakkenSituationGrid(grdHakkenSituationYokoKei, 1)
            '合計グリッド初期化
            initHakkenSituationGrid(grdHakkenSituationTotal, 1)
            '営業所ブロックグリッド初期化
            grdHakkenSituationBlock.SetData(grdHakkenSituationBlock.GetCellRange(4, 1), String.Empty)

            '各グリッドの行数初期設定（固定行5行＋配車経由地：最大4行＋発券計行1行＝最大10行）
            grdHakkenSituationDetail.Rows.Count = HakkenSituationGridMaxRow
            grdHakkenSituationYokoKei.Rows.Count = HakkenSituationGridMaxRow
            grdHakkenSituationTotal.Rows.Count = HakkenSituationGridMaxRow
            grdHakkenSituationBlock.Rows.Count = HakkenSituationGridMaxRow
            '明細グリッドの列数初期設定（料金区分：最大3個×料金区分（人員）：最大10個＋行ヘッダ＝最大31個）
            grdHakkenSituationDetail.Cols.Count = HakkenSituationDetailGridMaxCol
            '横計グリッドの列数初期設定（料金区分（人員）：最大10個＋行ヘッダ＝最大11個）
            grdHakkenSituationYokoKei.Cols.Count = HakkenSituationYokoKeiGridMaxCol

            ''明細グリッド（料金区分名、料金区分（人員）名）の設定
            For Each row As DataRow In searchResultHakkenSituationGridData.Rows
                sameValueFlag = False
                For colIdx As Integer = 1 To grdHakkenSituationDetail.Cols.Count - 1
                    '料金区分、料金区分（人員）が既に設定されている場合は読み飛ばす
                    If CType(grdHakkenSituationDetail.GetData(0, colIdx), String) = (row("CHARGE_NAME").ToString()) _
                    And CType(grdHakkenSituationDetail.GetData(1, colIdx), String) = (row("CHARGE_KBN_JININ_NAME").ToString()) Then
                        sameValueFlag = True
                        Exit For
                    End If
                Next

                If sameValueFlag = False Then
                    colCount += 1
                    '料金区分名
                    grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(GrdHakkenSituationRow.ChargeKbnNm, colCount), row("CHARGE_NAME").ToString())
                    '料金区分（人員）名
                    grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(GrdHakkenSituationRow.ChargeKbnJininNm, colCount), row("CHARGE_KBN_JININ_NAME").ToString())
                End If
            Next
            '明細グリッドの列数設定
            grdHakkenSituationDetail.Cols.Count = colCount + 1

            ''横計グリッド（料金区分（人員）名）の設定
            colCount = 0
            For Each row As DataRow In searchResultHakkenSituationGridData.Rows
                sameValueFlag = False
                For colIdx As Integer = 1 To grdHakkenSituationYokoKei.Cols.Count - 1
                    '料金区分（人員）が既に設定されている場合は読み飛ばす
                    If CType(grdHakkenSituationYokoKei.GetData(GrdHakkenSituationRow.ChargeKbnJininNm, colIdx), String) = (row("CHARGE_KBN_JININ_NAME").ToString()) Then
                        sameValueFlag = True
                        Exit For
                    End If
                Next

                If sameValueFlag = False Then
                    colCount += 1
                    '料金区分（人員）名
                    grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(GrdHakkenSituationRow.ChargeKbnJininNm, colCount), row("CHARGE_KBN_JININ_NAME").ToString())
                End If
            Next
            '横計グリッドの列数設定
            grdHakkenSituationYokoKei.Cols.Count = colCount + 1

            '検索条件部に指定された乗車地の人数設定
            '条件の設定
            whereString = "JYOCHACHI_CD_1 = '" & ucoNoribaCd.CodeText & "'"
            '問合せ対象データ取得
            selectData = searchResultHakkenSituationGridData.Select(whereString)

            ''明細グリッド（未発券人数、発券済人数、縦計）の設定
            For colIdx As Integer = 1 To grdHakkenSituationDetail.Cols.Count - 1
                miHakkenNinzuCount = 0
                hakkenZumiNinzuCount = 0
                For Each row As DataRow In selectData
                    '料金区分、料金区分（人員）単位に未発券人数と発券済人数を集計
                    If CType(grdHakkenSituationDetail.GetData(GrdHakkenSituationRow.ChargeKbnNm, colIdx), String) = (row("CHARGE_NAME").ToString()) _
                    AndAlso CType(grdHakkenSituationDetail.GetData(GrdHakkenSituationRow.ChargeKbnJininNm, colIdx), String) = (row("CHARGE_KBN_JININ_NAME").ToString()) Then
                        miHakkenNinzuCount += CInt(row("MIHAKKEN_NINZU").ToString())
                        hakkenZumiNinzuCount += CInt(row("HAKKEN_NINZU").ToString())
                    End If
                Next
                '未発券人数
                grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(GrdHakkenSituationRow.Mihakken, colIdx), miHakkenNinzuCount)
                '発券済人数
                grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(GrdHakkenSituationRow.HakkenZumi, colIdx), hakkenZumiNinzuCount)
                '縦計
                tateKei = miHakkenNinzuCount + hakkenZumiNinzuCount
                grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(GrdHakkenSituationRow.TateKei, colIdx), tateKei)
            Next

            ''横計グリッド（未発券人数、発券済人数、縦計）の設定
            For colIdx As Integer = 1 To grdHakkenSituationYokoKei.Cols.Count - 1
                miHakkenNinzuCount = 0
                hakkenZumiNinzuCount = 0
                For Each row As DataRow In selectData
                    '料金区分（人員）単位に未発券人数と発券済人数を集計
                    If CType(grdHakkenSituationYokoKei.GetData(GrdHakkenSituationRow.ChargeKbnJininNm, colIdx), String) = (row("CHARGE_KBN_JININ_NAME").ToString()) Then
                        miHakkenNinzuCount += CInt(row("MIHAKKEN_NINZU").ToString())
                        hakkenZumiNinzuCount += CInt(row("HAKKEN_NINZU").ToString())
                    End If
                Next

                '未発券人数
                grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(GrdHakkenSituationRow.Mihakken, colIdx), miHakkenNinzuCount)
                '発券済人数
                grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(GrdHakkenSituationRow.HakkenZumi, colIdx), hakkenZumiNinzuCount)
                '縦計
                grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(GrdHakkenSituationRow.TateKei, colIdx), miHakkenNinzuCount + hakkenZumiNinzuCount)
            Next

            '配車経由地の人数設定（配車経由地が存在する場合のみ）
            For idx As Integer = 1 To HaisyaKeiyuMaxCount
                '条件の設定
                whereString = "JYOCHACHI_CD_1 = HAISYA_KEIYU_CD_" & idx & " And JYOCHACHI_CD_1 <> '" & ucoNoribaCd.CodeText & "'"
                '問合せ対象データ取得
                selectData = searchResultHakkenSituationGridData.Select(whereString)

                If selectData.Count <= 0 Then
                    '条件に合致しない場合は次のループ処理を行う
                    Continue For
                End If

                '配車経由地の表示ありのためフラグをONにする
                displayHaisyaKeiyuchiFlag = True

                '設定行を決定
                For rowIdx As Integer = HakkenSituationGridMinRow To grdHakkenSituationDetail.Rows.Count - 1
                    If grdHakkenSituationDetail.GetData(rowIdx, 0) Is Nothing _
                    OrElse String.IsNullOrEmpty(CType(grdHakkenSituationDetail.GetData(rowIdx, 0), String)) Then
                        gridRowIdx = rowIdx
                        Exit For
                    End If
                Next

                '明細グリッド（発券済人数）の設定
                hakkenDetailCount = 0
                For colIdx As Integer = 1 To grdHakkenSituationDetail.Cols.Count - 1
                    hakkenZumiNinzuCount = 0
                    For Each row As DataRow In selectData
                        hakkenDetailCount += 1

                        If hakkenDetailCount = 1 Then
                            '配車経由地名
                            grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(gridRowIdx, 0), row("PLACE_NAME_SHORT").ToString())
                        End If

                        '料金区分、料金区分（人員）単位に発券済人数を集計
                        If CType(grdHakkenSituationDetail.GetData(GrdHakkenSituationRow.ChargeKbnNm, colIdx), String) = row("CHARGE_NAME").ToString() _
                        AndAlso CType(grdHakkenSituationDetail.GetData(GrdHakkenSituationRow.ChargeKbnJininNm, colIdx), String) = row("CHARGE_KBN_JININ_NAME").ToString() Then
                            hakkenZumiNinzuCount += CInt(row("HAKKEN_NINZU").ToString())
                        End If
                    Next

                    '発券済人数
                    grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(gridRowIdx, colIdx), hakkenZumiNinzuCount)
                Next

                '横計グリッド（発券済人数）の設定
                For colIdx As Integer = 1 To grdHakkenSituationYokoKei.Cols.Count - 1
                    hakkenZumiNinzuCount = 0
                    For Each row As DataRow In selectData

                        '料金区分（人員）単位に発券済人数を集計
                        If CType(grdHakkenSituationYokoKei.GetData(GrdHakkenSituationRow.ChargeKbnJininNm, colIdx), String) = row("CHARGE_KBN_JININ_NAME").ToString() Then
                            hakkenZumiNinzuCount += CInt(row("HAKKEN_NINZU").ToString())
                        End If

                    Next

                    '発券済人数
                    grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(gridRowIdx, colIdx), hakkenZumiNinzuCount)
                Next
            Next

            If displayHaisyaKeiyuchiFlag = True Then
                '配車経由地の表示ありの場合

                '行数の設定（データ件数＋ヘッダ行（1行）＋発券計行（1行））
                grdHakkenSituationDetail.Rows.Count = gridRowIdx + 2
                grdHakkenSituationYokoKei.Rows.Count = gridRowIdx + 2
                grdHakkenSituationTotal.Rows.Count = gridRowIdx + 2
                grdHakkenSituationBlock.Rows.Count = gridRowIdx + 2

                '発券計行の設定
                grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(gridRowIdx + 1, 0), "発券計")

                '明細グリッド（発券計）の設定
                For colIdx As Integer = 1 To grdHakkenSituationDetail.Cols.Count - 1
                    hakkenKei = 0
                    '検索条件部に指定された乗車地の発券済人数を加算
                    hakkenKei += CType(grdHakkenSituationDetail.GetData(GrdHakkenSituationRow.HakkenZumi, colIdx), Integer)
                    '配車経由地の発券済人数を加算
                    For rowIdx As Integer = 5 To gridRowIdx
                        hakkenKei += CType(grdHakkenSituationDetail.GetData(rowIdx, colIdx), Integer)
                    Next

                    '発券計
                    grdHakkenSituationDetail.SetData(grdHakkenSituationDetail.GetCellRange(gridRowIdx + 1, colIdx), hakkenKei)
                Next

                '横計グリッド（発券計）の設定
                For colIdx As Integer = 1 To grdHakkenSituationYokoKei.Cols.Count - 1
                    hakkenKei = 0
                    '検索条件部に指定された乗車地の発券済人数を加算
                    hakkenKei += CType(grdHakkenSituationYokoKei.GetData(GrdHakkenSituationRow.HakkenZumi, colIdx), Integer)
                    '配車経由地の発券済人数を加算
                    For rowIdx As Integer = 5 To gridRowIdx
                        hakkenKei += CType(grdHakkenSituationYokoKei.GetData(rowIdx, colIdx), Integer)
                    Next

                    '発券計
                    grdHakkenSituationYokoKei.SetData(grdHakkenSituationYokoKei.GetCellRange(gridRowIdx + 1, colIdx), hakkenKei)
                Next
            Else
                '配車経由地の表示なしの場合

                '行数の設定（固定行5行のみを表示）
                grdHakkenSituationDetail.Rows.Count = HakkenSituationGridMinRow
                grdHakkenSituationYokoKei.Rows.Count = HakkenSituationGridMinRow
                grdHakkenSituationTotal.Rows.Count = HakkenSituationGridMinRow
                grdHakkenSituationBlock.Rows.Count = HakkenSituationGridMinRow

            End If

            ''合計グリッドの設定
            For rowIdx As Integer = GrdHakkenSituationRow.Mihakken To grdHakkenSituationYokoKei.Rows.Count - 1
                totalCount = 0
                '横計グリッドの1列ごとの人数を集計
                For colIdx As Integer = 1 To grdHakkenSituationYokoKei.Cols.Count - 1
                    totalCount += CType(grdHakkenSituationYokoKei.GetData(rowIdx, colIdx), Integer)
                Next

                '合計
                grdHakkenSituationTotal.SetData(grdHakkenSituationTotal.GetCellRange(rowIdx, 1), totalCount)
            Next

            ''営業所ブロックグリッドの設定
            grdHakkenSituationBlock.SetData(grdHakkenSituationBlock.GetCellRange(4, 1), searchResultHakkenSituationGridData(0)("EI_BLOCK").ToString())
        Catch ex As Exception
            Throw
        End Try


    End Sub

    ''' <summary>
    ''' 発券状況グリッド初期化
    ''' </summary>
    ''' <param name="grid">グリッドコントロール名</param>
    ''' <param name="idxStart">インデックス開始位置</param>
    Private Sub initHakkenSituationGrid(grid As C1.Win.C1FlexGrid.C1FlexGrid, idxStart As Integer)
        For colIdx As Integer = idxStart To grid.Cols.Count - 1
            For rowIdx As Integer = idxStart To grid.Rows.Count - 1
                grid.SetData(grid.GetCellRange(rowIdx, colIdx), String.Empty)
            Next
        Next

    End Sub
#End Region

#End Region

#End Region

End Class