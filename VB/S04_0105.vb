Imports Hatobus.ReservationManagementSystem.Yoyaku
Imports C1.Win.C1FlexGrid

''' <summary>
''' S04_0105 売上確定入力
''' </summary>
''' <remarks>発券画面とほぼ共通</remarks>
Public Class S04_0105
    Inherits FormBase

#Region "定数／変数"
#Region "定数"
    ''' <summary>
    ''' 画面ID
    ''' </summary>
    Private Const PgmId = "S04_0105"
    ''' <summary>
    ''' コースコントロール情報　KEY1
    ''' </summary>
    Private Const CrsControlInfoKey1_11 = "11"
    ''' <summary>
    ''' コースコントロール情報　KEY2
    ''' </summary>
    Private Const CrsControlInfoKey2_01 = "01"
    ''' <summary>
    ''' 無効の貸借対応コード(貸借先がない)
    ''' </summary>
    Private Const ErrorTaisyakuTaioCd = "000"
    ''' <summary>
    ''' 売上確定
    ''' </summary>
    Private Const UriageKakutei = "売上確定"

#Region "ハッシュキー"
    ''' <summary>
    ''' ハッシュキー　システム日付（日付型)
    ''' </summary>
    Private Const KeyDtSysDate = "dtSysDate"
    ''' <summary>
    ''' ハッシュキー　システム日付（文字列型)
    ''' </summary>
    Private Const KeyStrSysDate = "strSysDate"
    ''' <summary>
    ''' ハッシュキー　システム日付（数値型)
    ''' </summary>
    Private Const KeyIntSysDate = "intSysDate"
    ''' <summary>
    ''' ハッシュキー　システム時刻（時分秒）（文字列型)
    ''' </summary>
    Private Const KeyStrSysTimeHhMmSs = "strSysTimeHhMmSs"
    ''' <summary>
    ''' ハッシュキー　システム時刻（時分秒）（数値型)
    ''' </summary>
    Private Const KeyIntSysTimeHhMmSs = "intSysTimeHhMmSs"
    ''' <summary>
    ''' ハッシュキー　システム時刻（時分）（文字列型)
    ''' </summary>
    Private Const KeyStrSysTimeHhMm = "strSysTimeHhMm"
    ''' <summary>
    ''' ハッシュキー　システム時刻（時分）（数値型)
    ''' </summary>
    Private Const KeyIntSysTimeHhMm = "intSysTimeHhMm"
#End Region

#Region "列、行"
    ''' <summary>
    ''' 列名　割引グリッド　割引名
    ''' </summary>
    Private Const NameColGrdWaribikiWaribikiName = "WARIBIKI_NAME"
    ''' <summary>
    ''' 列名　割引グリッド　割引人数
    ''' </summary>
    Private Const NameColGrdWaribikiWaribikiNinzu = "WARIBIKI_NINZU"
    ''' <summary>
    ''' 列名　割引グリッド　料金区分
    ''' </summary>
    Private Const NameColGrdWaribikiChargeName = "CHARGE_NAME"
    ''' <summary>
    ''' 列名　割引グリッド　人員
    ''' </summary>
    Private Const NameColGrdWaribikiJinin = "CHARGE_KBN_JININ_NAME"
    ''' <summary>
    ''' 列名　精算グリッド　精算項目名
    ''' </summary>
    Private Const NameColGrdSeisanSeisanKoumokuName = "SEISAN_KOUMOKU_NAME"
    ''' <summary>
    ''' 列名　精算グリッド　補助券発行会社名
    ''' </summary>
    Private Const NameColGrdSeisanIssueCompanyName = "ISSUE_COMPANY_NAME"
    ''' <summary>
    ''' 列名　精算グリッド　金額
    ''' </summary>
    Private Const NameColGrdSeisanNyuukin = "KINGAKU"
    ''' <summary>
    ''' 列名　金額グリッド　金額
    ''' </summary>
    Private Const NameColGrdKingaku = "colKingaku"
    ''' <summary>
    ''' 行番号　精算グリッド　現金
    ''' </summary>
    Private Const NoRowGrdSeisanGenkin = 1
    ''' <summary>
    ''' 行番号　精算グリッド　クレジット
    ''' </summary>
    Private Const NoRowGrdSeisanCredit = 2
    ''' <summary>
    ''' 行番号　精算グリッド　振込
    ''' </summary>
    Private Const NoRowGrdSeisanHurikomi = 3
    ''' <summary>
    ''' 行番号　金額グリッド　料金
    ''' </summary>
    Private Const NoRowGrdKingakuCharge = 0
    ''' <summary>
    ''' 行番号　金額グリッド　キャンセル料
    ''' </summary>
    Private Const NoRowGrdKingakuCancel = 1
    ''' <summary>
    ''' 行番号　金額グリッド　割引
    ''' </summary>
    Private Const NoRowGrdKingakuWaribiki = 2
    ''' <summary>
    ''' 行番号　金額グリッド　取扱手数料
    ''' </summary>
    Private Const NoRowGrdKingakuToriatukaiFee = 3
    ''' <summary>
    ''' 行番号　金額グリッド　請求
    ''' </summary>
    Private Const NoRowGrdKingakuSeikyu = 4
    ''' <summary>
    ''' 行番号　金額グリッド２　既入金額
    ''' </summary>
    Private Const NoRowGrdKingaku2PreNyukinGaku = 0
    ''' <summary>
    ''' 行番号　金額グリッド２　入金
    ''' </summary>
    Private Const NoRowGrdKingaku2Nyukin = 1
    '''' <summary>
    '''' 行番号　金額グリッド２　内金請求
    '''' </summary>
    'Private Const NoRowGrdKingaku2UchikinSeikyu = 2
    ''' <summary>
    ''' 行番号　金額グリッド２　残金
    ''' </summary>
    Private Const NoRowGrdKingaku2Zankin = 2
    ''' <summary>
    ''' 行番号　金額グリッド２　予約センター返金
    ''' </summary>
    Private Const NoRowGrdKingaku2YoyakuCenterHenkin = 3
    ''' <summary>
    ''' 行番号　金額グリッド２　おつり
    ''' </summary>
    Private Const NoRowGrdKingaku2Otsuri = 4
    ''' <summary>
    ''' 列名　入返金情報　予約センター振込金額
    ''' </summary>
    Private Const NameColYoyakuCenterHurikomiKingaku = "NYUUKIN_GAKU_1"
    ''' <summary>
    ''' 列名　入返金情報　ｵﾝﾗｲﾝｸﾚｼﾞｯﾄ決済金額
    ''' </summary>
    Private Const NameColOnlineCreditKingaku = "NYUUKIN_GAKU_2"
#End Region
#End Region

#Region "変数"
    ''' <summary>
    ''' 宿泊あり
    ''' </summary>
    Private _isStayAri As Boolean = False
    ''' <summary>
    ''' 振込入金あり（予約センター振込入金ありの場合、返金方法は予約センター返金）
    ''' </summary>
    Private _existsHurikomiNyuukin As Boolean = False
    ''' <summary>
    ''' 割引あり
    ''' </summary>
    Private _existsWaribiki As Boolean = False
    ''' <summary>
    ''' 予約番号単位の総予約人数
    ''' </summary>
    Private _totalYoyakuNinzu As Integer = 0
    ''' <summary>
    ''' 取扱手数料　売上
    ''' </summary>
    Private _toriatukaiFeeUriage As Decimal = 0
    ''' <summary>
    ''' 取扱手数料　キャンセル
    ''' </summary>
    Private _toriatukaiFeeCancel As Decimal = 0
    ''' <summary>
    ''' 割引コード、人員コード単位の情報
    ''' </summary>
    Private _infoByWaribikiCdJininCd As DataTable = Nothing
    ''' <summary>
    ''' 割引種別単位の人数
    ''' </summary>
    Private _ninzuByWaribikiType As Dictionary(Of String, Integer) = Nothing
    ''' <summary>
    ''' 割引種別単位の正規料金
    ''' </summary>
    Private _seikiChargeByWaribikiType As Dictionary(Of String, Integer) = Nothing
    ''' <summary>
    ''' 割引種別単位の請求額
    ''' </summary>
    Private _seikyuByWaribikiType As Dictionary(Of String, Integer) = Nothing
    ''' <summary>
    ''' 割引種別単位の割引額
    ''' </summary>
    Private _waribikiByWaribikiType As Dictionary(Of String, Integer) = Nothing
    ''' <summary>
    ''' 精算項目単位の未按分金
    ''' </summary>
    Private _undistributedKinBySeisanKoumoku As Dictionary(Of String, Integer) = Nothing
    ''' <summary>
    ''' 券番
    ''' </summary>
    Private _newKenNo As String = ""
    ''' <summary>
    ''' ＳＥＱ１（発券情報）
    ''' </summary>
    Private _hakkenInfoSeq1 As String = ""
    ''' <summary>
    ''' ＳＥＱ２（発券情報）
    ''' </summary>
    Private _hakkenInfoSeq2 As Integer = 0
    ''' <summary>
    ''' 発券情報の精算方法
    ''' </summary>
    Private _SeisanHohoHakkenInfo As String = ""
    ''' <summary>
    ''' クローズ確認フラグ
    ''' </summary>
    Private _closeFormFlg As Boolean = True
    ''' <summary>
    ''' 使用中フラグ獲得状態（デフォルトFALSE）
    ''' </summary>
    Private _isUsingFlg As Boolean = False

#Region "テーブル"
    ''' <summary>
    ''' 予約情報（基本）テーブル
    ''' </summary>
    Private _yoyakuInfoBasicTable As DataTable = Nothing
    ''' <summary>
    ''' 発券情報テーブル
    ''' </summary>
    Private _hakkenInfoTable As DataTable = Nothing
    ''' <summary>
    ''' 代理店マスタテーブル
    ''' </summary>
    Private _agentMaster As DataTable = Nothing
    ''' <summary>
    ''' 予約情報（コース料金_料金区分）テーブル
    ''' </summary>
    Private _yoyakuInfoCrsChargeChargeKbn As DataTable = Nothing
    ''' <summary>
    ''' 割引コードマスタテーブル
    ''' </summary>
    Private _waribikiCdMaster As DataTable = Nothing
    ''' <summary>
    ''' 精算項目マスタテーブル
    ''' </summary>
    Private _seisanKoumokuMaster As DataTable = Nothing
    ''' <summary>
    ''' 補助券発行会社テーブル
    ''' </summary>
    Private _SubKenIssueCompany As DataTable = Nothing
#End Region
#End Region
#End Region

#Region "プロパティ"
#Region "Public"
    ''' <summary>
    ''' パラメータクラス
    ''' </summary>
    Public Property ParamData As S04_0105ParamData
    ''' <summary>
    ''' 当画面が更新されたかの判定
    ''' </summary>
    ''' <returns></returns>
    Public Property IsUpdated As Boolean = False
#End Region

    ''' <summary>
    ''' 定期
    ''' </summary>
    ''' <returns></returns>
    Private Property IsTeiki As Boolean
    ''' <summary>
    ''' 企画
    ''' </summary>
    ''' <returns></returns>
    Private Property IsKikaku As Boolean
    ''' <summary>
    ''' 宿泊あり
    ''' </summary>
    ''' <returns></returns>
    Private Property IsStayAri As Boolean
        Get
            Return Me._isStayAri
        End Get
        Set(value As Boolean)
            Me._isStayAri = value
            Me.grdChargeKbnStay.Visible = value
            Me.grdChargeKbn.Visible = Not value
        End Set
    End Property
    ''' <summary>
    ''' 発券済み
    ''' </summary>
    ''' <returns></returns>
    Private Property IsAlreadyHakken As Boolean
    ''' <summary>
    ''' 料金
    ''' </summary>
    ''' <returns></returns>
    Private Property ChargeTotal As Integer
    ''' <summary>
    ''' キャンセル料
    ''' </summary>
    ''' <returns></returns>
    Private Property Cancel As Integer
    ''' <summary>
    ''' キャンセル料（予約情報（基本））
    ''' </summary>
    ''' <returns></returns>
    Private Property CancelYoyakuInfoBasic As Integer
    ''' <summary>
    ''' 割引
    ''' </summary>
    ''' <returns></returns>
    Private Property WaribikiTotal As Integer
    ''' <summary>
    ''' 取扱手数料
    ''' </summary>
    Private Property ToriatukaiFee As Integer
    ''' <summary>
    ''' 請求
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property SeikyuTotal As Integer
        Get
            Return Me.ChargeTotal + Me.Cancel - Me.WaribikiTotal - Me.ToriatukaiFee
        End Get
    End Property
    ''' <summary>
    ''' 既入金額
    ''' </summary>
    ''' <returns></returns>
    Private Property PreNyukinGaku As Integer
    ''' <summary>
    ''' 入金額
    ''' </summary>
    ''' <returns></returns>
    Private Property Nyuukin As Integer
    ''' <summary>
    ''' 残金
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property Zankin As Integer
        Get
            Return Me.SeikyuTotal - Me.PreNyukinGaku
        End Get
    End Property
    ''' <summary>
    ''' 予約センター返金
    ''' </summary>
    ''' <returns></returns>
    Private Property YoyakuCenterHenkin As Integer
    ''' <summary>
    ''' おつり
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property Oturi As Integer
        Get
            Return Me.Nyuukin - Me.SeikyuTotal - Me.YoyakuCenterHenkin
        End Get
    End Property
    ''' <summary>
    ''' 予約センター入金額
    ''' </summary>
    ''' <returns></returns>
    Private Property YoyakuCenterNyuukin As Integer
    ''' <summary>
    ''' オンライン決済額
    ''' </summary>
    ''' <returns></returns>
    Private Property OnlineCredit As Integer
#End Region

#Region "イベント"

#Region "画面"
    ''' <summary>
    ''' 画面ロード時のイベント
    ''' </summary>
    ''' <param name="sender">イベント送信元</param>
    ''' <param name="e">イベントデータ</param>
    Private Sub S04_0105_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '画面表示時の初期設定
            Me.setControlInitiarize()

            '画面の初期表示
            Me.setScreen()

            '集計
            Me.calculateEachKingaku()

            If Me._existsHurikomiNyuukin Then
                '予約センター振込がある場合、予約センター返金とする
                Me.YoyakuCenterHenkin = Me.Nyuukin - Me.SeikyuTotal
                Me.grdKingaku2.Item(NoRowGrdKingaku2YoyakuCenterHenkin, NameColGrdKingaku) = Me.Nyuukin - Me.SeikyuTotal

            Else
                '予約センター返金がない場合、非活性
                CommonHakken.disableGrdCell(grdKingaku2,
                                        grdKingaku2.Rows(NoRowGrdKingaku2YoyakuCenterHenkin))
                Me.YoyakuCenterHenkin = 0
                Me.grdKingaku2.Item(NoRowGrdKingaku2YoyakuCenterHenkin, NameColGrdKingaku) = 0
            End If

            '金額の表示
            Me.setEachKingaku()

            '表示チェック
            If Not Me.isValidDisp() Then
                Return
            End If

            '使用中フラグON
            Dim isSuccess As Boolean = Me.updateUsingFlg(True)
            If isSuccess = False Then
                CommonProcess.createFactoryMsg.messageDisp("E90_040")
                Return
            Else
                ' 使用中フラグセットの状態を更新
                _isUsingFlg = True
            End If

        Catch ex As Exception
            Me._closeFormFlg = False
            createFactoryMsg().messageDisp("E90_046", ex.Message)
            Me.Close()

        Finally
            ' 後処理
            comPostEvent()
        End Try
    End Sub

    ''' <summary>
    ''' 画面終了時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub S04_0105_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        '更新完了時は確認不要
        If Me.IsUpdated = True Then
            Return
        End If

        '画面終了の確認
        If _closeFormFlg = True Then
            If CommonProcess.createFactoryMsg().messageDisp("W90_003", "入力された内容") = MsgBoxResult.Cancel Then
                e.Cancel = True
                Return
            End If
        End If

        Me._closeFormFlg = True
    End Sub

    ''' <summary>
    ''' 画面終了後のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub S02_0602_Closed(sender As Object, e As EventArgs) Handles MyBase.Closed
        Try
            ' 使用中設定したら、終了時に解除
            If _isUsingFlg = True Then
                Dim isSuccess As Boolean = Me.updateUsingFlg(False)
                If isSuccess = False Then
                    CommonProcess.createFactoryMsg().messageDisp("E90_025", "更新処理", "使用中フラグ解除")
                End If
            End If

        Catch ex As Exception
            createFactoryMsg().messageDisp("E90_046", ex.Message)
            Me.Close()

        Finally
            ' 後処理
            comPostEvent()
        End Try
    End Sub
#End Region

#Region "フッタ"
    ''' <summary>
    ''' F2：戻るボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF2_ClickOrgProc()
        MyBase.btnF2_ClickOrgProc()

        Me.Close()
    End Sub

    ''' <summary>
    ''' F10:登録ボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF10_ClickOrgProc()
        '確認メッセージ
        Dim msgResult As MsgBoxResult = CommonProcess.createFactoryMsg.messageDisp("Q90_001", UriageKakutei)
        If msgResult = MsgBoxResult.Cancel Then
            Return
        End If

        Dim hakkenNaiyo As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("HAKKEN_NAIYO"), "")
        Dim dHakkenKingaku As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("HAKKEN_KINGAKU"), 0)
        Dim hakkenKingaku As Integer = Convert.ToInt32(dHakkenKingaku)

        If Me.IsAlreadyHakken = True Then
            '発券済みの場合、予約情報（基本）.NO SHOWフラグのみ更新を行う
            Me.IsUpdated = Me.updateYoyakuInfoBasicForNoShow()

        Else
            ' エラー初期化
            CommonRegistYoyaku.removeGridBackColorStyle(Me.grdWaribikiCharge)

            '再計算
            Me.calculateEachKingaku()

            '入力チェック
            If Not Me.isValidInputHakken() Then
                Return
            End If

            '割引コード、種別単位の人数、請求額を集計
            Me.setInfosByWaribikiType()
            '精算項目単位の入金額を集計
            Me._undistributedKinBySeisanKoumoku = Me.setUndistributedKinBySeisanKoumoku()

            '券番を採番
            Me.numberingKenNo()

            '発券登録処理、更新件数を取得
            Me.IsUpdated = Me.insertHakkenGroup()

        End If

        If Me.IsUpdated = False Then
            '異常終了
            CommonProcess.createFactoryMsg.messageDisp("E90_025", UriageKakutei) 'TODO:オラクルエラー取り出し
            ' log出力
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, UriageKakutei, "登録処理")

            Return
        End If

        ' log出力
        createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.entry, UriageKakutei, "登録処理")

        '正常終了
        CommonProcess.createFactoryMsg.messageDisp("I90_002", UriageKakutei)

        Me.Close()
    End Sub
#End Region

    ''' <summary>
    ''' 追加ボタン押下イベント
    ''' </summary>
    ''' <param name="sender">イベント送信元</param>
    ''' <param name="e">イベントデータ</param>
    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim grd As FlexGridEx = TryCast(Me.grdSeisan, FlexGridEx)
        grd.Rows.Add()
    End Sub

    ''' <summary>
    ''' 削除ボタン押下イベント
    ''' </summary>
    ''' <param name="sender">イベント送信元</param>
    ''' <param name="e">イベントデータ</param>
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim grd As FlexGridEx = TryCast(Me.grdSeisan, FlexGridEx)

        If grd.Row <= 0 Then Return

        '支払済みなら削除失敗
        Dim shiharaizumi As String = If(TryCast(grd.Item(grd.Row, "SHIHARAIZUMI"), String), "")
        If shiharaizumi.Equals(FixedCd.CommonKigouType.maruMark) Then Return

        '削除
        grd.RemoveItem(grd.Row)

        '再計算
        Me.calculateEachKingaku()
        Me.setEachKingaku()
    End Sub

    ''' <summary>
    ''' メモ分類コンボボックスチェンジイベント
    ''' </summary>
    ''' <param name="sender">イベント送信元</param>
    ''' <param name="e">イベントデータ</param>
    Private Sub cmbBunrui_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBunrui.SelectedIndexChanged

        Dim value As String = CommonRegistYoyaku.convertObjectToString(Me.cmbBunrui.SelectedValue)

        If String.IsNullOrWhiteSpace(value) = True Then
            ' 空の場合は、フィルタをクリア
            Me.grdMemoList.ClearFilter()
            Return
        End If

        ' グリッドのフィルタリングを有効にします
        Me.grdMemoList.AllowFiltering = True
        ' 新しいValueFilterを作成します
        Dim filter As New ValueFilter()
        filter.ShowValues = New Object() {value}
        ' 新しいフィルタを1列目に割り当てます
        Me.grdMemoList.Cols("MEMO_BUNRUI").Filter = filter
        ' フィルタ条件を適用します
        Me.grdMemoList.ApplyFilters()
    End Sub

    ''' <summary>
    ''' 割引料金グリッド編集後イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub grdWaribikiCharge_TextChanged(sender As Object, e As EventArgs) Handles grdWaribikiCharge.AfterEdit
        Dim grd As FlexGridEx = TryCast(sender, FlexGridEx)
        Dim selectedCd As String = TryCast(grd.Item(grd.Row, grd.Col), String)
        Dim selectedColumnName As String = grd.Cols(grd.Col).Name

        Select Case selectedColumnName
            Case NameColGrdWaribikiWaribikiName
                '割引項目を変更の場合

                '選択コードを基に割引コードマスタより値を抽出
                Dim dr As DataRow = Me._waribikiCdMaster.AsEnumerable() _
                                             .FirstOrDefault(Function(row) row.Field(Of String)("WARIBIKI_CD").Equals(selectedCd))

                If dr Is Nothing Then Return

                Dim tani As String = If(dr.Field(Of String)("WARIBIKI_KBN"), "")

                '値のグリッドへの設定
                grd(grd.Row, "WARIBIKI_CD") = dr.Field(Of String)("WARIBIKI_CD")
                If tani.Equals(FixedCdYoyaku.Tani.Per) Then
                    '「率」の場合
                    grd(grd.Row, "WARIBIKI") = If(dr.Field(Of Short?)("WARIBIKI_PER"), 0)

                ElseIf tani.Equals(FixedCdYoyaku.Tani.Yen) Then
                    '「額」の場合
                    grd(grd.Row, "WARIBIKI") = If(dr.Field(Of Integer?)("WARIBIKI_KINGAKU"), 0)

                Else
                    grd(grd.Row, "WARIBIKI") = ""
                End If
                grd(grd.Row, "WARIBIKI_TYPE_KBN") = dr.Field(Of String)("WARIBIKI_TYPE_KBN")
                grd(grd.Row, "WARIBIKI_KBN") = tani
                grd(grd.Row, "KBN") = dr.Field(Of String)("KBN")
                grd(grd.Row, "CARRIAGE_WARIBIKI_FLG") = dr.Field(Of String)("CARRIAGE_WARIBIKI_FLG")
                grd(grd.Row, "YOYAKU_WARIBIKI_FLG") = dr.Field(Of String)("YOYAKU_WARIBIKI_FLG")
                grd(grd.Row, "WARIBIKI_APPLICATION_NINZU") = dr.Field(Of Short?)("WARIBIKI_APPLICATION_NINZU")


            Case NameColGrdWaribikiChargeName
                '料金区分を変更の場合

                '選択コードを基にコース料金より値を抽出
                Dim dr As DataRow = Me._yoyakuInfoCrsChargeChargeKbn.AsEnumerable() _
                                      .FirstOrDefault(Function(row) row.Field(Of Short?)("KBN_NO").ToString().Equals(selectedCd))
                grd(grd.Row, "KBN_NO") = If(dr Is Nothing, Nothing, dr.Field(Of Short?)("KBN_NO"))
                grd(grd.Row, "CHARGE_KBN") = If(dr Is Nothing, Nothing, dr.Field(Of String)("CHARGE_KBN"))

            Case NameColGrdWaribikiJinin
                grd.Item(grd.Row, "CHARGE_KBN_JININ_CD") = grd.Item(grd.Row, grd.Col)

                '人員を変更の場合
                If Me.IsKikaku Then
                    '企画の場合、区分No、料金区分を設定
                    Dim dr As DataRow = Me._yoyakuInfoCrsChargeChargeKbn.AsEnumerable() _
                                          .FirstOrDefault()
                    grd(grd.Row, "KBN_NO") = If(dr Is Nothing, Nothing, dr.Field(Of Short?)("KBN_NO"))
                    grd(grd.Row, "CHARGE_KBN") = If(dr Is Nothing, Nothing, dr.Field(Of String)("CHARGE_KBN"))
                End If

            Case NameColGrdWaribikiWaribikiNinzu
                '割引人数を変更の場合

                ' エラー初期化
                CommonRegistYoyaku.removeGridBackColorStyle(Me.grdWaribikiCharge)

                '割引コードが空
                If CommonHakken.isNullOrEmpty(grd(grd.Row, "WARIBIKI_CD")) Then
                    CommonRegistYoyaku.changeGridBackColor(grd.Row, 6, Me.grdWaribikiCharge)
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引コード")
                    grd(grd.Row, "WARIBIKI_NINZU") = ""
                    Return
                End If

                '料金区分が空
                If Me.IsStayAri = False Then

                    If CommonHakken.isNullOrEmpty(grd(grd.Row, "CHARGE_NAME")) Then

                        CommonRegistYoyaku.changeGridBackColor(grd.Row, 12, Me.grdWaribikiCharge)
                        CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "料金区分")
                        grd(grd.Row, "WARIBIKI_NINZU") = ""
                        Return
                    End If
                End If

                '割引適用者が空
                If CommonHakken.isNullOrEmpty(grd(grd.Row, "CHARGE_KBN_JININ_NAME")) Then

                    CommonRegistYoyaku.changeGridBackColor(grd.Row, 14, Me.grdWaribikiCharge)
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引適用者")
                    grd(grd.Row, "WARIBIKI_NINZU") = ""
                    Return
                End If

                Me.calculateWaribiki()
                Me.setEachKingaku()
                Return
        End Select

        '割引コードの予約人員との整合性を保つために、グリッドの変更時に以降の列を初期化
        For cntCol = 1 To grd.Cols.Count - 1
            '選択列以下の列番号なら次の列へ（初期化しない）
            If cntCol <= grd.Col Then
                Continue For
            End If

            '割引区分と区分と割引なら次の列へ（初期化しない）
            If grd.Cols(cntCol).Name.Equals("WARIBIKI_KBN") _
            OrElse grd.Cols(cntCol).Name.Equals("WARIBIKI") _
            OrElse grd.Cols(cntCol).Name.Equals("KBN") Then
                Continue For
            End If

            '以降の列へ空文字挿入
            grd.Item(grd.Row, cntCol) = ""
        Next
    End Sub

    ''' <summary>
    ''' 精算グリッド押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub grdSeisan_Click(sender As Object, e As EventArgs) Handles grdSeisan.Click
        Dim grd As FlexGridEx = TryCast(sender, FlexGridEx)

        'ヘッダー行選択時なら削除ボタンを非活性
        If grd.Row = 0 Then
            Me.btnDelete.Enabled = False
            Return
        End If

        '精算コードが空, 支払済みが空, 支払済みが○以外 のいずれかなら削除ボタンを活性化
        If grd.GetData(grd.Row, "SEISAN_KOUMOKU_CD") Is Nothing _
        OrElse grd.GetData(grd.Row, "SHIHARAIZUMI") Is Nothing _
        OrElse Not grd.GetData(grd.Row, "SHIHARAIZUMI").ToString().Equals(FixedCd.CommonKigouType.maruMark) Then

            Me.btnDelete.Enabled = True
            Return
        End If

        Me.btnDelete.Enabled = False
    End Sub

    ''' <summary>
    ''' 精算グリッド編集後イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub grdSeisan_AfterEdit(sender As Object, e As RowColEventArgs) Handles grdSeisan.AfterEdit
        Dim grd As FlexGridEx = TryCast(sender, FlexGridEx)
        Dim selectedCd As String = If(TryCast(grd.Item(grd.Row, grd.Col), String), "")
        Dim selectedColumnName As String = grd.Cols(grd.Col).Name


        Select Case selectedColumnName
            Case NameColGrdSeisanSeisanKoumokuName
                '精算項目名ドロップダウン変更時

                '空セル選択時、初期化
                If selectedCd.Equals(CommonHakken.EmptyCellKey) Then
                    For noCol = 1 To grd.Cols.Count - 1
                        grd.Item(grd.Row, noCol) = ""
                    Next
                    Return
                End If

                '選択コードを基に割引コードマスタより値を抽出
                Dim dr As DataRow = Me._seisanKoumokuMaster.AsEnumerable() _
                                   .FirstOrDefault(Function(row) row.Field(Of String)("SEISAN_KOUMOKU_CD").Equals(selectedCd))

                If dr Is Nothing Then Return

                '値のグリッドへの設定
                grd(grd.Row, "SEISAN_KOUMOKU_CD") = dr.Field(Of String)("SEISAN_KOUMOKU_CD")
                grd(grd.Row, "TAISYAKU_KBN") = dr.Field(Of String)("TAISYAKU_KBN")

                '金額の計算
                Me.calculateEachKingaku()
                '金額の表示
                Me.setEachKingaku()

            Case NameColGrdSeisanIssueCompanyName
                '補助券発行会社ドロップダウン変更時

                '選択コードを基に割引コードマスタより値を抽出
                Dim dr As DataRow = Me._SubKenIssueCompany.AsEnumerable() _
                                   .FirstOrDefault(Function(row) row.Field(Of String)("ISSUE_COMPANY_CD").Equals(selectedCd))

                If dr Is Nothing Then Return

                '値のグリッドへの設定
                grd(grd.Row, "ISSUE_COMPANY_CD") = dr.Field(Of String)("ISSUE_COMPANY_CD")

            Case NameColGrdSeisanNyuukin
                '入金額変更時

                '金額の計算
                Me.calculateEachKingaku()
                '金額の表示
                Me.setEachKingaku()
        End Select
    End Sub

    ''' <summary>
    ''' 金額グリッド１編集後イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub grdKingaku1_TextChanged(sender As Object, e As EventArgs) Handles grdKingaku.AfterEdit
        Dim grd As FlexGridEx = TryCast(sender, FlexGridEx)
        If grd.Rows.Count <= 0 Then Return

        'キャンセル料をプロパティへ
        If Not CommonHakken.isNull(grd.Item(NoRowGrdKingakuCancel, NameColGrdKingaku)) Then
            Dim strCancel As String = grd.Item(NoRowGrdKingakuCancel, NameColGrdKingaku).ToString()
            strCancel = CommonHakken.replaceNotNumber(strCancel)
            Integer.TryParse(strCancel, Me.Cancel)
        End If

        '取扱手数料をプロパティへ
        If Not CommonHakken.isNull(grd.Item(NoRowGrdKingakuToriatukaiFee, NameColGrdKingaku)) Then
            Dim strToriatukaiFee As String = grd.Item(NoRowGrdKingakuToriatukaiFee, NameColGrdKingaku).ToString()
            strToriatukaiFee = CommonHakken.replaceNotNumber(strToriatukaiFee)
            Integer.TryParse(strToriatukaiFee, Me.ToriatukaiFee)
        End If

        '金額の計算
        Me.calculateEachKingaku()
        '金額の表示
        Me.setEachKingaku()
    End Sub

    ''' <summary>
    ''' 金額グリッド２編集後イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub grdKingaku2_TextChanged(sender As Object, e As EventArgs) Handles grdKingaku2.AfterEdit
        Dim grd As FlexGridEx = TryCast(sender, FlexGridEx)
        If grd.Rows.Count <= 0 Then Return

        '予約センター返金をプロパティへ
        If Not CommonHakken.isNull(grd.Item(NoRowGrdKingaku2YoyakuCenterHenkin, NameColGrdKingaku)) Then
            Dim strYoyakuCenterHenkin As String = grd.Item(NoRowGrdKingaku2YoyakuCenterHenkin, NameColGrdKingaku).ToString()
            strYoyakuCenterHenkin = CommonHakken.replaceNotNumber(strYoyakuCenterHenkin)
            Integer.TryParse(strYoyakuCenterHenkin, Me.YoyakuCenterHenkin)
        End If

        '金額の計算
        Me.calculateEachKingaku()
        '金額の表示
        Me.setEachKingaku()
    End Sub
#End Region

#Region "メソッド"
#Region "画面の設定"
    ''' <summary>
    ''' 画面表示時の初期設定
    ''' </summary>
    Private Sub setControlInitiarize()
        'ベースフォームの設定
        Me.setFormId = PgmId
        Me.setTitle = UriageKakutei & "入力"

        'フッタボタンの設定
        Me.setButtonInitiarize()

        'パラメータ確認
        If String.IsNullOrWhiteSpace(Me.ParamData.YoyakuKbn) Then Return
        If Me.ParamData.YoyakuNo = 0 Then Return

        '予約情報（基本）の取得、存在確認
        Me._yoyakuInfoBasicTable = CommonHakken.getYoyakuInfoBasic(Me.ParamData.YoyakuKbn, Me.ParamData.YoyakuNo)
        If _yoyakuInfoBasicTable Is Nothing Then Return

        '発券情報の取得
        Me._hakkenInfoTable = CommonHakken.getHakkenInfo(Me.ParamData.YoyakuKbn, Me.ParamData.YoyakuNo)

        '各区分を設定
        Dim teikiKikakuKbn As String = _yoyakuInfoBasicTable.Rows(0).Field(Of String)("TEIKI_KIKAKU_KBN")
        Dim crsKind As String = _yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_KIND")
        Me.setEachKbn(teikiKikakuKbn, crsKind)

        '代理店の設定
        Me.setAgent(crsKind)
    End Sub

    ''' <summary>
    ''' 各区分を設定
    ''' </summary>
    Private Sub setEachKbn(teikiKikakuKbn As String, crsKind As String)
        '定期/企画区分の設定
        Dim intTeikiKikakuKbn As Integer = 0
        Integer.TryParse(teikiKikakuKbn, intTeikiKikakuKbn)
        If intTeikiKikakuKbn = FixedCd.TeikiKikakuKbn.Teiki Then
            Me.IsTeiki = True
            Me.IsKikaku = False
        End If
        If intTeikiKikakuKbn = FixedCd.TeikiKikakuKbn.Kikaku Then
            Me.IsTeiki = False
            Me.IsKikaku = True
        End If

        '宿泊有無の設定
        Me.IsStayAri = CommonCheckUtil.isStay(crsKind)
    End Sub

    ''' <summary>
    ''' 代理店の設定
    ''' </summary>
    ''' <param name="crsKind"></param>
    Private Sub setAgent(crsKind As String)
        '取扱手数料の設定
        Dim dToriatukaiFeeCancel As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("TORIATUKAI_FEE_CANCEL"), 0)
        Dim dToriatukaiFeeUriage As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("TORIATUKAI_FEE_URIAGE"), 0)
        Dim toriatukaiFeeCancel As Integer = Convert.ToInt32(dToriatukaiFeeCancel)
        Dim toriatukaiFeeUriage As Integer = Convert.ToInt32(dToriatukaiFeeUriage)

        Dim agentCd As String = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("AGENT_CD")
        '代理店ありの場合
        If Not String.IsNullOrWhiteSpace(agentCd) Then
            Me._agentMaster = CommonHakken.getAgentMaster(agentCd)
        End If

        '精算方法の確認
        Dim seisanHoho As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("SEISAN_HOHO"), "")

        Dim tojiTuPayment As String = CommonHakken.convertEnumToString(FixedCdYoyaku.PaymentHoho.tojituPayment)
        Dim agt As String = CommonHakken.convertEnumToString(FixedCdYoyaku.PaymentHoho.agt)
        If seisanHoho.Equals(tojiTuPayment) OrElse seisanHoho.Equals(agt) Then
            '精算方法が当日　または　代理店の場合

            Me._toriatukaiFeeCancel = toriatukaiFeeCancel
            Me._toriatukaiFeeUriage = 0
            Me.ToriatukaiFee = Convert.ToInt32(Me._toriatukaiFeeCancel + Me._toriatukaiFeeUriage)

        Else
            '精算方法が当日でない　かつ　代理店でない場合

            If String.IsNullOrWhiteSpace(agentCd) = True Then
                ' 代理店コードがない場合、処理終了
                Return
            End If

            '未契約AGENTのコミッション率初期値取得
            Dim prmAgentCom As New GetMiKeiyakuAgentComPerParam
            prmAgentCom.crsKind = crsKind
            YoyakuBizCommon.getMiKeiyakuAgentComPer(prmAgentCom)

            Dim prmCheckAgentCd As New CheckAgentCdParam
            prmCheckAgentCd.agentCd = agentCd
            If Me.IsTeiki Then
                prmCheckAgentCd.teikiSponsorshipKbn = CommonHakken.convertEnumToString(FixedCd.TeikiKikakuKbn.Teiki)
            Else
                prmCheckAgentCd.teikiSponsorshipKbn = CommonHakken.convertEnumToString(FixedCd.TeikiKikakuKbn.Kikaku)
            End If

            prmCheckAgentCd.crsCd = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_CD")
            prmCheckAgentCd.houjinGaikyakuKbn = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("HOUJIN_GAIKYAKU_KBN")

            '業者コードチェック
            Dim returnCd As Integer = YoyakuBizCommon.checkAgentCd(prmCheckAgentCd)

            '異常終了
            If returnCd <> CommonHakken.NormalEnd Then
                CommonProcess.createFactoryMsg.messageDisp("E90_014", "代理店コード")
                Return
            End If

            'コミッションの設定
            Dim commission As Integer = CommonHakken.setCommission(prmAgentCom, prmCheckAgentCd)
            '取扱手数料の設定
            Me.setToriatukaiFee(commission)
        End If
    End Sub

    ''' <summary>
    ''' 取扱手数料の設定
    ''' </summary>
    Private Sub setToriatukaiFee(commission As Integer)
        '正規料金総額
        Dim seikiChargeAllGaku As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("SEIKI_CHARGE_ALL_GAKU"), 0)
        '割引総額
        Dim waribikiAllGaku As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("WARIBIKI_ALL_GAKU"), 0)
        Dim prmCalcToriatukaiFee As New CalcToriatukaiFeeParam
        prmCalcToriatukaiFee.com = commission

        '取扱手数料/売上の設定
        prmCalcToriatukaiFee.charge = seikiChargeAllGaku - waribikiAllGaku
        Dim returnCd As Integer = YoyakuBizCommon.calcToriatukaiFee(prmCalcToriatukaiFee)
        If returnCd = CommonHakken.NormalEnd Then
            '正常終了
            Me.ToriatukaiFee += Convert.ToInt32(prmCalcToriatukaiFee.comgaku)
            Me._toriatukaiFeeUriage = prmCalcToriatukaiFee.comgaku
        End If

        'キャンセル料計
        Dim cancelRyoukei As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("CANCEL_RYOU_KEI"), 0)
        '取扱手数料/ｷｬﾝｾﾙの設定
        prmCalcToriatukaiFee.charge = cancelRyoukei
        returnCd = YoyakuBizCommon.calcToriatukaiFee(prmCalcToriatukaiFee)
        If returnCd = CommonHakken.NormalEnd Then
            '正常終了
            Me.ToriatukaiFee += Convert.ToInt32(prmCalcToriatukaiFee.comgaku)
            Me._toriatukaiFeeCancel = Convert.ToInt32(prmCalcToriatukaiFee.comgaku)
        End If
    End Sub

    ''' <summary>
    ''' フッタボタンの設定
    ''' </summary>
    Private Sub setButtonInitiarize()

        'Visible
        Me.F2Key_Visible = True
        Me.F10Key_Visible = True

        Me.F1Key_Visible = False
        Me.F3Key_Visible = False
        Me.F4Key_Visible = False
        Me.F5Key_Visible = False
        Me.F6Key_Visible = False
        Me.F7Key_Visible = False
        Me.F8Key_Visible = False
        Me.F9Key_Visible = False
        Me.F11Key_Visible = False
        Me.F12Key_Visible = False

        'Text
        Me.F2Key_Text = "F2:戻る"
        Me.F10Key_Text = "F10:登録"
    End Sub

    ''' <summary>
    ''' 画面の値設定
    ''' </summary>
    Private Sub setScreen()
        '予約情報（基本）の設定
        Me.setYoyakuInfoBasic()

        '発券情報の設定
        Me.setHakkenInfo()

        '予約情報（コース料金_料金区分）の設定
        Me.setYoyakuInfoCrsChargeChargeKbn()

        'メモ情報の設定
        Me.setMemoInfo()

        '割引料金グリッドの設定
        Me.setGrdWaribikiCharge()

        '精算グリッドの設定
        Me.setGrdSeisan()
    End Sub

#Region "予約情報（基本）の設定"
    ''' <summary>
    ''' 予約情報（基本）の設定
    ''' </summary>
    Private Sub setYoyakuInfoBasic()
        '値の取り出し
        Dim dt As DataTable = Me._yoyakuInfoBasicTable
        Dim yoyakuKbn As String = If(dt.Rows(0).Field(Of String)("YOYAKU_KBN"), "")
        Dim yoyakuNo As String = If(dt.Rows(0).Field(Of Integer?)("YOYAKU_NO"), 0).ToString()
        Dim yoyakuKbnNo As String = yoyakuKbn & yoyakuNo
        Dim intSyuptDay As Integer = If(dt.Rows(0).Field(Of Integer?)("SYUPT_DAY"), 0)
        Dim intSyuptTime As Integer = If(dt.Rows(0).Field(Of Short?)("SYUPT_TIME"), 0)
        Dim crsCd As String = If(dt.Rows(0).Field(Of String)("CRS_CD"), "")
        Dim crsNm As String = If(dt.Rows(0).Field(Of String)("CRS_NAME"), "")
        Dim jyoshaTi As String = If(dt.Rows(0).Field(Of String)("JYOSHATI"), "")
        Dim gousya As String = If(dt.Rows(0).Field(Of Short?)("GOUSYA"), 0).ToString()
        Dim yoyakuName As String = If(dt.Rows(0).Field(Of String)("NAME"), "")
        Dim telNo As String = If(dt.Rows(0).Field(Of String)("TEL_NO_1"), "")
        Dim telNo2 As String = If(dt.Rows(0).Field(Of String)("TEL_NO_2"), "")
        Dim agentCd As String = If(dt.Rows(0).Field(Of String)("AGENT_CD"), "")
        Dim agentName As String = If(dt.Rows(0).Field(Of String)("AGENT_NM"), "")
        Dim noShowFlg As String = If(dt.Rows(0).Field(Of String)("NO_SHOW_FLG"), "")

        '料金をプロパティへ設定
        Dim dSeikiChargeAllGaku As Decimal = If(dt.Rows(0).Field(Of Decimal?)("SEIKI_CHARGE_ALL_GAKU"), 0)
        Dim dAddChargeMaeBaraiKei As Decimal = If(dt.Rows(0).Field(Of Decimal?)("ADD_CHARGE_MAEBARAI_KEI"), 0)
        Dim charge As Integer = Convert.ToInt32(dSeikiChargeAllGaku) _
                              + Convert.ToInt32(dAddChargeMaeBaraiKei)
        Me.ChargeTotal = charge

        'キャンセル料をプロパティへ設定
        Me.Cancel = Convert.ToInt32(If(dt.Rows(0).Field(Of Decimal?)("CANCEL_RYOU_KEI"), 0))
        Me.CancelYoyakuInfoBasic = Convert.ToInt32(If(dt.Rows(0).Field(Of Decimal?)("CANCEL_RYOU_KEI"), 0))

        '入金額総計を設定
        Me.PreNyukinGaku = Convert.ToInt32(If(dt.Rows(0).Field(Of Decimal?)("NYUKINGAKU_SOKEI"), 0))

        '値の設定
        Me.ucoYoyakuNo.YoyakuText = yoyakuKbnNo
        Me.ucoSyuptDay.Value = CommonHakken.convertIntToDate(intSyuptDay)
        Me.txtCourseCd.Text = crsCd
        Me.txtCourseNm.Text = crsNm
        Me.txtJyosyaTi.Text = jyoshaTi
        Me.ucoTime.Value = CommonHakken.convertIntToTime(intSyuptTime)
        Me.txtGousya.Text = gousya
        Me.txtYoyakuPersonName.Text = yoyakuName
        Me.txtTel.Text = telNo
        Me.txtTel2.Text = telNo2
        Me.txtDairitencd.Text = agentCd
        Me.txtDairitenNm.Text = agentName

        If Not String.IsNullOrWhiteSpace(noShowFlg) = True Then
            'NoShowフラグ = 'Y'の場合、初期表示時チェックON
            Me.chkNoShow.Checked = True
        End If

    End Sub
#End Region

    ''' <summary>
    ''' 発券情報の設定
    ''' </summary>
    Private Sub setHakkenInfo()
        If Me._hakkenInfoTable Is Nothing Then
            Me.lblUriageKakuteiZumi.Text = ""
            Return
        End If

        '未VOIDの発券情報の存在確認
        Dim isHakkend As Boolean = Me._hakkenInfoTable.AsEnumerable() _
                                  .Any(Function(row) String.IsNullOrWhiteSpace(row.Field(Of String)("VOID_KBN")))
        If isHakkend Then
            Me.lblUriageKakuteiZumi.Text = UriageKakutei & "済"
            Me.lblUriageKakuteiZumi.ForeColor = Color.Red
            Dim kenNo As String = CommonHakken.createKenNoFromHakkenInfoRow(Me._hakkenInfoTable.Rows(0))
            Me.txtKenNo.Text = kenNo
            Me.txtHakkenTime.Text = Me._hakkenInfoTable.Rows(0).Field(Of Date)("SYSTEM_ENTRY_DAY").ToString()

        Else
            Me.lblUriageKakuteiZumi.Text = ""
        End If
    End Sub

#Region "予約情報（コース料金_料金区分）の設定"
    ''' <summary>
    ''' 予約情報（コース料金_料金区分）の設定
    ''' </summary>
    Private Sub setYoyakuInfoCrsChargeChargeKbn()
        'グリッドの初期化
        CommonHakken.setInitiarizeGrid(Me.grdChargeKbnStay)
        CommonHakken.setInitiarizeGrid(Me.grdChargeKbn)

        If Not Me.IsStayAri Then
            '宿泊なし
            Me.setGrdChargeKbn()

        Else
            '宿泊あり
            Me.setGrdChargeKbnStayAri()
        End If
    End Sub

#Region "料金区分(宿泊なし)グリッドの設定"
    ''' <summary>
    ''' 料金区分(宿泊なし)グリッドの設定
    ''' </summary>
    Private Sub setGrdChargeKbn()
        'グリッドの設定
        Me.grdChargeKbn.AllowDragging = AllowDraggingEnum.None
        Me.grdChargeKbn.AllowAddNew = False
        Me.grdChargeKbn.AutoGenerateColumns = False
        Me.grdChargeKbn.ShowButtons = ShowButtonsEnum.Always

        '予約情報（コース料金_料金区分）の取得
        Dim yoyakuInfoCrsChargeChargeKbn As DataTable _
            = CommonHakken.getYoyakuInfoCrsChargeChargeKbn(Me.ParamData.YoyakuKbn, Me.ParamData.YoyakuNo)
        If yoyakuInfoCrsChargeChargeKbn Is Nothing Then Return

        'フィールドへ格納
        Me._yoyakuInfoCrsChargeChargeKbn = yoyakuInfoCrsChargeChargeKbn

        '予約総人数を集計
        Me.calculateCharge()

        'グリッドへ設定
        Dim formatedDt As DataTable = CommonHakken.formatGrdChargeKbn(yoyakuInfoCrsChargeChargeKbn)
        Me.grdChargeKbn.DataSource = formatedDt
    End Sub

    ''' <summary>
    ''' コース料金（宿泊なし）の集計（非活性なので初期表示時のみ）
    ''' </summary>
    Private Sub calculateCharge()
        '宿泊なし
        For Each row As DataRow In Me._yoyakuInfoCrsChargeChargeKbn.AsEnumerable()
            Dim dNinzu As Decimal = If(row.Field(Of Decimal?)("CHARGE_APPLICATION_NINZU_1"), 0)
            Dim ninzu As Integer = Convert.ToInt32(dNinzu)

            'null,0 なら次の行へ
            If ninzu = 0 Then
                Continue For
            End If

            Dim dTanka As Decimal = If(row.Field(Of Decimal?)("TANKA_1"), 0)
            Dim tanka As Integer = Convert.ToInt32(dTanka)

            '予約総人数へ加算
            Me._totalYoyakuNinzu += ninzu
        Next
    End Sub
#End Region

#Region "料金区分(宿泊あり)グリッドの設定"
    ''' <summary>
    ''' 料金区分(宿泊あり)グリッドの設定
    ''' </summary>
    Private Sub setGrdChargeKbnStayAri()
        'グリッドの設定
        Me.grdChargeKbnStay.AllowDragging = AllowDraggingEnum.None
        Me.grdChargeKbnStay.AllowAddNew = False
        Me.grdChargeKbnStay.AllowMerging = AllowMergingEnum.Custom
        Me.grdChargeKbnStay.AutoGenerateColumns = False
        Me.grdChargeKbnStay.ShowButtons = ShowButtonsEnum.Always

        '予約情報（コース料金_料金区分）の取得
        Dim yoyakuInfoCrsChargeChargeKbn As DataTable _
         = CommonHakken.getYoyakuInfoCrsChargeChargeKbn(Me.ParamData.YoyakuKbn, Me.ParamData.YoyakuNo)
        If yoyakuInfoCrsChargeChargeKbn Is Nothing Then Return

        'フィールドへ格納（※割引時に正規料金を確認するため。
        Me._yoyakuInfoCrsChargeChargeKbn = yoyakuInfoCrsChargeChargeKbn

        '部屋タイプごとのデータを生成
        Dim dt As DataTable = CommonHakken.formatGrdChargeKbnStayAri(yoyakuInfoCrsChargeChargeKbn)

        '予約総人数を集計
        Me._totalYoyakuNinzu = dt.AsEnumerable().Sum(Function(dr) dr.Field(Of Integer)("CHARGE_APPLICATION_NINZU"))

        '料金区分(宿泊あり)テーブルをグリッドへ設定
        Me.grdChargeKbnStay.DataSource = dt.AsDataView()
    End Sub
#End Region
#End Region

    ''' <summary>
    ''' メモ一覧設定
    ''' </summary>
    Private Sub setMemoInfo()
        ' メモ分類
        Dim s02_0103Da As New S02_0103Da()
        CommonRegistYoyaku.setComboBoxData(s02_0103Da, Me.cmbBunrui, FixedCdYoyaku.CodeBunruiTypeMemoBunrui)

        Dim yoyakuMemoTable As DataTable = CommonRegistYoyaku.getYoyakuMemoList(Me.ParamData.YoyakuKbn, Me.ParamData.YoyakuNo)
        Me.grdMemoList.DataSource = yoyakuMemoTable
    End Sub

#Region "割引料金グリッドの設定"
    ''' <summary>
    ''' 割引料金グリッドの設定
    ''' </summary>
    Private Sub setGrdWaribikiCharge()
        'グリッドの設定
        Me.grdWaribikiCharge.AllowDragging = AllowDraggingEnum.None
        Me.grdWaribikiCharge.AllowAddNew = True
        Me.grdWaribikiCharge.AutoGenerateColumns = False
        Me.grdWaribikiCharge.ShowButtons = ShowButtonsEnum.Always

        '入力MAX桁数設定
        CommonHakken.setGridLength(Me.grdWaribikiCharge, "WARIBIKI", CommonHakken.KingakuMaxLength)
        CommonHakken.setGridLength(Me.grdWaribikiCharge, "WARIBIKI_NINZU", CommonHakken.NinzuMaxLength)

        If Me.IsStayAri Then
            Me.grdWaribikiCharge.Width -= Me.grdWaribikiCharge.Cols("CHARGE_NAME").Width
            Me.grdWaribikiCharge.Cols("CHARGE_NAME").Visible = False
        End If

        '割引コードの設定
        Me.setWaribikiCd()

        '料金区分、人員の設定
        Me.setWaribikiChargeKbn()

        '料金グリッドの行数になるまで、行を追加
        If Not Me.IsStayAri Then
            '宿泊なし
            While Me.grdWaribikiCharge.Rows.Count < Me.grdChargeKbn.Rows.Count
                Me.grdWaribikiCharge.Rows.Add()
            End While
        Else
            '宿泊あり
            While Me.grdWaribikiCharge.Rows.Count < Me.grdChargeKbnStay.Rows.Count
                Me.grdWaribikiCharge.Rows.Add()
            End While
        End If

        Dim crsKind As String = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_KIND")
        Dim houjinGaikyakuKbn As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("HOUJIN_GAIKYAKU_KBN"), "")

        '予約情報（割引）の取得
        Dim yoyakuInfoWaribikiTable As DataTable = CommonHakken.getYoyakuInfoWaribiki(Me.ParamData.YoyakuKbn,
                                                                                              Me.ParamData.YoyakuNo,
                                                                                              crsKind,
                                                                                              houjinGaikyakuKbn)
        If yoyakuInfoWaribikiTable Is Nothing Then Return

        Dim dv As DataView = Me.formatGrdWaribikiCharge(yoyakuInfoWaribikiTable)

        Me.grdWaribikiCharge.DataSource = dv
        Me.grdWaribikiCharge.Rows.Add()
    End Sub

    ''' <summary>
    ''' 割引コードの設定
    ''' </summary>
    Private Sub setWaribikiCd()
        Dim syuptDay As Integer = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY"), 0)
        Dim crsKind As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_KIND"), "")
        Dim houjinGaikyakuKbn As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("HOUJIN_GAIKYAKU_KBN"), "")

        '割引コードマスタの取得
        Me._waribikiCdMaster = CommonHakken.getWaribikiCdMaster(syuptDay, crsKind, houjinGaikyakuKbn)

        If Me._waribikiCdMaster IsNot Nothing Then
            '割引マスタ
            Me.grdWaribikiCharge.Cols("WARIBIKI_NAME").DataType = GetType(String)
            Me.grdWaribikiCharge.Cols("WARIBIKI_NAME").DataMap = CommonHakken.createCmbWaribikiCd(Me._waribikiCdMaster)

            'TODO:単位入力可　対応
            ''割引区分（単位）
            'Me.grdWaribikiCharge.Cols("WARIBIKI_KBN").DataType = GetType(String)
            'Me.grdWaribikiCharge.Cols("WARIBIKI_KBN").DataMap = CommonHakken.createCmbTani()

        Else
            Me.grdWaribikiCharge.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' 割引料金区分の設定
    ''' </summary>
    Private Sub setWaribikiChargeKbn()
        If IsNothing(_yoyakuInfoCrsChargeChargeKbn) = False Then
            '料金区分
            Me.grdWaribikiCharge.Cols("CHARGE_NAME").DataType = GetType(String)
            Me.grdWaribikiCharge.Cols("CHARGE_NAME").DataMap = CommonHakken.createCmbChargeKbn(Me._yoyakuInfoCrsChargeChargeKbn)

            '人員コード
            Me.grdWaribikiCharge.Cols("CHARGE_KBN_JININ_NAME").DataType = GetType(String)
            Me.grdWaribikiCharge.Cols("CHARGE_KBN_JININ_NAME").DataMap = CommonHakken.createCmbChargeKbnJininCd(Me._yoyakuInfoCrsChargeChargeKbn)
        End If
    End Sub

    ''' <summary>
    ''' 割引コード単位の割引のデータを生成（表示用）
    ''' </summary>
    ''' <returns>割引コード単位の割引グリッド表示用データ</returns>
    ''' <remarks>
    ''' 予約情報（割引）を部屋タイプ１～５を集計してデータ生成
    ''' </remarks>
    Private Function formatGrdWaribikiCharge(yoyakuInfoWaribikiTable As DataTable) As DataView
        '各行の値を取り出し
        '予約情報（割引）を部屋タイプ１～５で集計してデータ生成
        For Each row As DataRow In yoyakuInfoWaribikiTable.AsEnumerable()

            Dim tani As String = If(row.Field(Of String)("WARIBIKI_KBN"), "")
            Dim carriageWaribikiFlg As String = If(row.Field(Of String)("CARRIAGE_WARIBIKI_FLG"), "")
            Dim waribikiPer As Decimal = If(row.Field(Of Decimal?)("WARIBIKI_PER"), 0)
            Dim tanka As Decimal = If(row.Field(Of Decimal?)($"WARIBIKI_KINGAKU"), 0)

            '割引 率/額によって計算を変更
            row.Item("WARIBIKI") = If(tani.Equals(FixedCdYoyaku.Tani.Per), waribikiPer, tanka)

            Dim sumWaribikiGakuByRow As Integer = 0
            Dim sumNinzuByRow As Decimal = 0

            '割適用人数１～５を集計
            For roomType = CommonHakken.One1R To CommonHakken.FiveIjyou1R
                Dim ninzu As Decimal = If(row.Field(Of Decimal?)($"WARIBIKI_APPLICATION_NINZU_{roomType}"), 0)
                Dim waribikiTanka As Decimal = If(row.Field(Of Decimal?)($"WARIBIKI_TANKA_{roomType}"), 0)

                'null, 0なら次の人数
                If ninzu = 0 Then
                    Continue For
                End If

                '行ごとの人数へ加算
                sumNinzuByRow += ninzu

                '行ごとの割引額へ加算
                sumWaribikiGakuByRow += Convert.ToInt32(waribikiTanka * ninzu) '割引単価 * 割引人数
            Next
            '部屋タイプ１～５までのループ（1行単位）終了

            row.Item("WARIBIKI_NINZU") = sumNinzuByRow.ToString()
            row.Item("WARIBIKI_KINGAKU") = sumWaribikiGakuByRow.ToString()

            '料金区分Null対応
            If IsDBNull(row.Item("CHARGE_NAME")) Then

                Dim kbnNo As String = If(row.Field(Of Short?)("KBN_NO"), 0).ToString()
                row.Item("CHARGE_NAME") = kbnNo.ToString()
            End If
        Next
        '予約情報（割引）データ１行ずつのループ終了

        Return yoyakuInfoWaribikiTable.AsDataView()
    End Function
#End Region

#Region "精算グリッドの設定"
    '' <summary>
    '' 精算グリッドの設定
    '' </summary>
    Private Sub setGrdSeisan()

        Me.grdSeisan.AllowAddNew = False
        Me.grdSeisan.AutoGenerateColumns = False
        Me.grdSeisan.ShowButtons = ShowButtonsEnum.Always

        '入力MAX桁数設定
        CommonHakken.setGridLength(Me.grdSeisan, "KINGAKU", CommonHakken.KingakuMaxLength)

        '精算項目マスタの設定
        Me.setSeisanKoumoku()
        '補助券発行会社の設定
        Me.setSubKenIssueCompany()
        '内訳の設定
        Me.setHurikomiKbn()

        '固定行の用意（現金、クレジット）
        Me.setFixRowToGrdSeisan(NoRowGrdSeisanGenkin, FixedCd.SeisanItemCd.genkin)
        Me.setFixRowToGrdSeisan(NoRowGrdSeisanCredit, FixedCd.SeisanItemCd.credit_card)

        '入返金情報の設定
        Me.setNyuukinInfo()
    End Sub

    ''' <summary>
    ''' 精算項目の設定
    ''' </summary>
    Private Sub setSeisanKoumoku()
        '非表示の精算項目リスト
        Dim hidedSeisanKomokuCds() As String = CommonHakken.createHidedSeisanKomokuCds()
        '精算項目マスタの取得
        Me._seisanKoumokuMaster = CommonHakken.getSeisanKoumokuMaster(hidedSeisanKomokuCds)
        If Not CommonHakken.existsDatas(Me._seisanKoumokuMaster) Then Return

        Me.grdSeisan.Cols("SEISAN_KOUMOKU_NAME").DataType = GetType(String)
        Me.grdSeisan.Cols("SEISAN_KOUMOKU_NAME").DataMap = CommonHakken.createCmbSeisanKoumoku(Me._seisanKoumokuMaster)
    End Sub

    ''' <summary>
    ''' 補助券発行会社の設定
    ''' </summary>
    Private Sub setSubKenIssueCompany()
        '補助券発行会社（マスタ）の取得
        Me._SubKenIssueCompany = CommonHakken.getSubKenIssueCompany()
        If Not CommonHakken.existsDatas(Me._SubKenIssueCompany) Then Return

        Me.grdSeisan.Cols("ISSUE_COMPANY_NAME").DataType = GetType(String)
        Me.grdSeisan.Cols("ISSUE_COMPANY_NAME").DataMap = CommonHakken.createCmbSubKenIssueCompany(Me._SubKenIssueCompany)
    End Sub

    ''' <summary>
    ''' 振込区分の設定
    ''' </summary>
    Private Sub setHurikomiKbn()
        Me.grdSeisan.Cols("HURIKOMI_KBN").DataType = GetType(String)
        Me.grdSeisan.Cols("HURIKOMI_KBN").DataMap = CommonHakken.createCmbHurikomiKbn()
    End Sub

    ''' <summary>
    ''' 入返金情報の設定
    ''' </summary>
    Private Sub setNyuukinInfo()
        '入返金情報の取得
        Dim nyukinTable As DataTable = Me.getNyuukin(Me.ParamData.YoyakuKbn, Me.ParamData.YoyakuNo)
        If nyukinTable Is Nothing Then Return
        Dim IsOnline As Boolean = False 'オンライン決済クレジットフラグ（True：オンライン決済の場合、False：オンライン決済以外）
        '全行読出し
        For noTblRow = 0 To nyukinTable.Rows.Count - 1
            '発券振込区分に応じて処理を変更
            Dim hakkenHurikomiKbn As String = nyukinTable.Rows(noTblRow).Field(Of String)("HAKKEN_HURIKOMI_KBN")
            If String.IsNullOrWhiteSpace(hakkenHurikomiKbn) Then
                Continue For 'nullなら次の行へ
            End If
            If hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnYoyakuCenterNyuukin) _
            OrElse hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnYoyakuCenterHenkin) Then
                '■予約センター振込の場合

                '振込予約センター入金の固定行を設定
                Me.setFixRowToGrdSeisan(NoRowGrdSeisanHurikomi, FixedCd.SeisanItemCd.hurikomi_yoyaku_center)

                '振込入金ありにする
                Me._existsHurikomiNyuukin = True

                'nullなら次の行へ
                If nyukinTable.Rows(noTblRow).Field(Of Integer?)(NameColYoyakuCenterHurikomiKingaku) Is Nothing Then
                    Continue For
                End If

                '合計へ追加
                Dim intKingaku As Integer = If(nyukinTable.Rows(noTblRow).Field(Of Integer?)(NameColYoyakuCenterHurikomiKingaku), 0)
                If hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnYoyakuCenterNyuukin) Then
                    Me.YoyakuCenterNyuukin += intKingaku '入金
                ElseIf hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnYoyakuCenterHenkin) Then
                    Me.YoyakuCenterNyuukin -= intKingaku '返金
                End If
                Me.grdSeisan.Item(NoRowGrdSeisanHurikomi, "KINGAKU") = Me.YoyakuCenterNyuukin.ToString()

                Me.grdSeisan.Item(NoRowGrdSeisanHurikomi, "SHIHARAIZUMI") = FixedCd.CommonKigouType.maruMark
                Me.grdSeisan.Item(NoRowGrdSeisanHurikomi, "HURIKOMI_KBN") = nyukinTable.Rows(noTblRow).Item("HURIKOMI_KBN")
                Me.grdSeisan.Item(NoRowGrdSeisanHurikomi, "BIKO") = nyukinTable.Rows(noTblRow).Item("BIKO")

            ElseIf hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnOnlineCreditNyuukin) _
            OrElse hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnOnlineCreditHenkin) Then
                '■オンライン決済クレジットの場合

                '振込予約ｾﾝﾀｰ入金の項目をｵﾝﾗｲﾝ決済に変更
                Me.setFixRowToGrdSeisan(NoRowGrdSeisanHurikomi, FixedCd.SeisanItemCd.online_credit)

                'nullなら次の行へ
                If nyukinTable.Rows(noTblRow).Field(Of Integer?)(NameColOnlineCreditKingaku) Is Nothing Then
                    Continue For
                End If

                '合計へ追加
                Dim intKingaku As Integer = If(nyukinTable.Rows(noTblRow).Field(Of Integer?)(NameColOnlineCreditKingaku), 0)
                If hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnOnlineCreditNyuukin) Then
                    Me.OnlineCredit += intKingaku '入金
                ElseIf hakkenHurikomiKbn.Equals(CommonHakken.HakkenHurikomiKbnOnlineCreditHenkin) Then
                    Me.OnlineCredit -= intKingaku '返金
                End If
                Me.grdSeisan.Item(NoRowGrdSeisanHurikomi, "KINGAKU") = Me.OnlineCredit.ToString

                Me.grdSeisan.Item(NoRowGrdSeisanHurikomi, "SHIHARAIZUMI") = FixedCd.CommonKigouType.maruMark
                Me.grdSeisan.Item(NoRowGrdSeisanHurikomi, "HURIKOMI_KBN") = nyukinTable.Rows(noTblRow).Item("HURIKOMI_KBN")
                Me.grdSeisan.Item(NoRowGrdSeisanHurikomi, "BIKO") = nyukinTable.Rows(noTblRow).Item("BIKO")

                IsOnline = True	'オンライン決済クレジットフラグ（オンライン決済：True）

            End If
        Next '入金情報テーブル読出しのループ

        '支払済みの行を非活性化
        For Each grdRow As Row In Me.grdSeisan.Rows
            '"○"でなければ次の行へ
            If TryCast(grdRow.Item("SHIHARAIZUMI"), String) Is Nothing _
            OrElse grdRow.Item("SHIHARAIZUMI").ToString() <> FixedCd.CommonKigouType.maruMark Then
                Continue For
            End If

            '"○"の行を非活性
            CommonHakken.disableGrdCell(Me.grdSeisan, grdRow)
        Next '精算グリッドのループ
        'オンライン決済時、デフォルト表示される「現金」「クレジット」の行は表示しない。
        If IsOnline = True Then
            Me.grdSeisan.RemoveItem(NoRowGrdSeisanCredit)
            Me.grdSeisan.RemoveItem(NoRowGrdSeisanGenkin)
        End If
    End Sub

    ''' <summary>
    ''' 精算グリッドへの固定値の行の設定
    ''' </summary>
    Private Sub setFixRowToGrdSeisan(noRowGrdSeisan As Integer, enumSeisanItemCd As Object)
        '精算項目コードの設定
        Dim seisanKoumokuCd As String = CommonHakken.convertEnumToString(enumSeisanItemCd)
        Me.grdSeisan.Item(noRowGrdSeisan, "SEISAN_KOUMOKU_CD") = seisanKoumokuCd

        'コードをキーに精算項目マスタから抽出
        Dim dr As DataRow = Me._seisanKoumokuMaster.AsEnumerable() _
                            .FirstOrDefault(Function(x) x.Field(Of String)("SEISAN_KOUMOKU_CD").Equals(seisanKoumokuCd))
        Me.grdSeisan.Item(noRowGrdSeisan, "SEISAN_KOUMOKU_NAME") = dr.Item("SEISAN_KOUMOKU_NAME").ToString()
    End Sub
#End Region

#Region "金額の計算"
    ''' <summary>
    ''' 各金額の計算（各グリッドを集計）
    ''' </summary>
    Private Sub calculateEachKingaku()
        '割引の集計
        Me.calculateWaribiki()

        '入金額の集計
        Me.calculateNyuukin()
    End Sub

    ''' <summary>
    ''' 割引額の集計
    ''' </summary>
    Private Sub calculateWaribiki()
        Me.WaribikiTotal = 0

        '正規料金の人数を取得
        If IsNothing(_yoyakuInfoCrsChargeChargeKbn) = False Then
            'キー：料金カラムセット、値：部屋タイプ１～５の予約人数を格納した配列
            Dim yoyakuNinzusByChargeKbn As Dictionary(Of String, Integer()) = CommonHakken.setYoyakuNinzuByJininCd(Me._yoyakuInfoCrsChargeChargeKbn)

            '全行の割引人数（部屋タイプごと）を初期化
            Me.initiarizeGrdWaribikiNinzu()

            '割引人数（部屋タイプごと）を割り振り開始
            For Each row As Row In Me.grdWaribikiCharge.Rows
                If row.Index = 0 Then
                    Continue For
                End If
                Dim waribikiCd As String = If(TryCast(row.Item("WARIBIKI_CD"), String), "")
                If String.IsNullOrWhiteSpace(waribikiCd) Then
                    Continue For
                End If

                '人数
                Dim strNinzu As String = ""
                If Not CommonHakken.isNull(row.Item("WARIBIKI_NINZU")) Then
                    strNinzu = row.Item("WARIBIKI_NINZU").ToString()
                End If
                Dim ninzu As Integer = 0
                Integer.TryParse(strNinzu, ninzu)

                If ninzu = 0 Then
                    Continue For
                End If

                '単位
                Dim tani As String = If(TryCast(row.Item("WARIBIKI_KBN"), String), "")
                '割引
                Dim strWaribiki As String = ""
                If Not CommonHakken.isNull(row.Item("WARIBIKI")) Then
                    strWaribiki = row.Item("WARIBIKI").ToString()
                End If
                Dim waribiki As Decimal = 0
                Decimal.TryParse(strWaribiki, waribiki)

                '区分No
                Dim KbnNo As String = ""
                If Not CommonHakken.isNull(row.Item("KBN_NO")) Then
                    KbnNo = row.Item("KBN_NO").ToString()
                End If

                '料金区分
                Dim chargeKbn As String = If(TryCast(row.Item("CHARGE_KBN"), String), "")
                '人員コード
                Dim jininCd As String = If(TryCast(row.Item("CHARGE_KBN_JININ_CD"), String), "")
                '料金カラムセットの作成
                Dim chargeColumns As String = CommonHakken.createChargeColumnsSet(KbnNo, chargeKbn, jininCd)

                '運賃割引フラグ
                Dim carriageWaribikiFlg As String = If(TryCast(row.Item("CARRIAGE_WARIBIKI_FLG"), String), "")


                '料金カラムセットをキーに部屋毎の人数を取り出し
                Dim yoyakuNinzusByRoomType() As Integer = yoyakuNinzusByChargeKbn(chargeColumns)

                Dim tmpWaribikiNinzuByRow As Integer = ninzu '計算用の一時人数
                Dim waribikiKingakuByRow As Integer = 0 '行単位の割引金額

                '割引人数を予約のある、部屋タイプ１→５の人数へ振り分けていく
                '原則、「1名1室タイプの1人当たり単価」 > ... > 「5名1室の1人当たり単価」の為、
                '部屋タイプ1→５の順で割引金額を計算すると、割引額が最安値になる。
                For roomType = CommonHakken.One1R To CommonHakken.FiveIjyou1R

                    '残りの割引人数が0ならループ終了
                    If tmpWaribikiNinzuByRow = 0 Then
                        Exit For
                    End If

                    If yoyakuNinzusByRoomType(roomType) = 0 Then
                        '残りの予約人数が0
                        If roomType < CommonHakken.FiveIjyou1R Then
                            '１～４名1室なら次の部屋タイプへ
                            Continue For
                        Else
                            '５名1室までループして減らせなかった場合、エラー
                            CommonRegistYoyaku.changeGridBackColor(row.Index, 25, Me.grdWaribikiCharge)
                            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引人数", "予約人数以下")
                            waribikiKingakuByRow = 0
                            Exit For
                        End If
                    End If

                    Dim cntAdd As Integer = 0

                    '部屋ごとの予約人数が0になるか、行ごとの人数が0になるまで減算
                    While yoyakuNinzusByRoomType(roomType) > 0 AndAlso tmpWaribikiNinzuByRow > 0
                        '予約の人数を減算
                        yoyakuNinzusByRoomType(roomType) -= 1
                        tmpWaribikiNinzuByRow -= 1

                        cntAdd += 1

                        '部屋タイプ５が終わっても、割引人数が残っていればエラー
                        If roomType = CommonHakken.FiveIjyou1R _
                    AndAlso yoyakuNinzusByRoomType(roomType) = 0 _
                    AndAlso tmpWaribikiNinzuByRow > 0 Then

                            CommonRegistYoyaku.changeGridBackColor(row.Index, 25, Me.grdWaribikiCharge)
                            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引人数", "予約人数以下")
                            waribikiKingakuByRow = 0
                            Exit For
                        End If
                    End While

                    '割引の人数へ加算
                    Dim waribikiNinzu As Integer = Integer.Parse(row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}").ToString())
                    waribikiNinzu += cntAdd
                    row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}") = waribikiNinzu.ToString()

                    '割引単価の取得
                    Dim waribikiTanka As Integer = Me.setWaribikiTanka(tani, waribiki, jininCd, roomType, carriageWaribikiFlg)
                    row.Item($"WARIBIKI_TANKA_{roomType}") = waribikiTanka

                    '割引単価 * 割引適用人数を合計
                    waribikiKingakuByRow += waribikiTanka * waribikiNinzu
                Next 'ここまで部屋タイプ1～5のループ


                '割引金額の合計を表示
                row.Item("WARIBIKI_KINGAKU") = If(waribikiKingakuByRow = 0, "", waribikiKingakuByRow.ToString())

                '割引へ追加
                Me.WaribikiTotal += waribikiKingakuByRow
            Next
        End If
    End Sub

    ''' <summary>
    ''' 割引人数の初期化
    ''' </summary>
    Private Sub initiarizeGrdWaribikiNinzu()
        For Each row As Row In Me.grdWaribikiCharge.Rows
            If row.Index = 0 Then
                Continue For
            End If

            For roomType = CommonHakken.One1R To CommonHakken.FiveIjyou1R
                row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}") = "0"
                row.Item($"WARIBIKI_TANKA_{roomType}") = "0"
            Next

            row.Item($"WARIBIKI_KINGAKU") = ""
        Next
    End Sub

    ''' <summary>
    ''' 割引単価の設定
    ''' </summary>
    ''' <returns></returns>
    Private Function setWaribikiTanka(tani As String,
                                      waribiki As Decimal,
                                      jinincd As String,
                                      roomtype As Integer,
                                      carriagewaribikiflg As String) As Integer

        Dim dWaribikiTanka As Decimal = 0

        If tani.Equals(FixedCdYoyaku.Tani.Yen) Then
            '割引「額」の場合
            dWaribikiTanka = waribiki

        ElseIf tani.Equals(FixedCdYoyaku.Tani.Per) Then
            '割引「率」の場合

            '正規料金の取得（運賃割引フラグを考慮）
            Dim seikiCharge As Decimal = CommonHakken.getSeikiChargeWithCarriage(jinincd,
                                                                                 roomtype,
                                                                                 carriagewaribikiflg,
                                                                                 Me._yoyakuInfoCrsChargeChargeKbn)
            '端数処理前の割引金額
            Dim waribikiBeforeRound As Decimal = seikiCharge * waribiki / 100D

            '四捨五入
            dWaribikiTanka = YoyakuBizCommon.roundWaribiki(waribikiBeforeRound, Me.IsTeiki)
        End If

        Return Convert.ToInt32(dWaribikiTanka)
    End Function

    ''' <summary>
    ''' 入金額の集計
    ''' </summary>
    Private Sub calculateNyuukin()
        Me.Nyuukin = 0

        Dim grd As FlexGridEx = Me.grdSeisan

        For Each row As Row In grd.Rows
            Dim nyuukin As Integer = 0
            Dim henkin As Integer = 0

            'ヘッダーなら次の行へ
            If row.Index = 0 Then
                Continue For
            End If

            'nullなら次の行へ
            If CommonHakken.isNull(row.Item("KINGAKU")) Then
                Continue For
            End If

            '金額を取り出し
            Integer.TryParse(row.Item("KINGAKU").ToString(), nyuukin)
            row.Item("TAISYAKU_KINGAKU") = row.Item("KINGAKU")

            '貸方の場合、符号反転
            Dim TaisyakuKbn As String = If(TryCast(row.Item("TAISYAKU_KBN"), String), "")
            If TaisyakuKbn.Equals(CommonHakken.convertEnumToString(FixedCd.TaisyakuKbn.Kasikata)) Then
                henkin = nyuukin
                nyuukin = 0
                row.Item("TAISYAKU_KINGAKU") = (-henkin).ToString()
            End If

            '入金額へ追加
            Me.Nyuukin += nyuukin
            Me.Nyuukin -= henkin

            '予約センター返金の入力があれば、貸方分を減算
            If Me.YoyakuCenterHenkin > 0 Then
                Me.YoyakuCenterHenkin -= henkin
            End If
        Next
    End Sub

    ''' <summary>
    ''' 各金額グリッドの設定(各集計金額を再表示）
    ''' </summary>
    Private Sub setEachKingaku()
        Me.setKingaku1()
        Me.setKingaku2()
    End Sub

    ''' <summary>
    ''' 金額グリッド１の設定
    ''' </summary>
    Private Sub setKingaku1()
        grdKingaku.Rows.Fixed = 0
        grdKingaku2.Rows.Fixed = 0

        '金額グリッド1の初期化
        CommonHakken.setInitiarizeGrid(Me.grdKingaku)
        Dim dt As DataTable = New DataTable()
        dt.Columns.Add(NameColGrdKingaku)
        Dim row As DataRow = dt.NewRow

        '予約情報(基本）
        Dim yoyakuTbl As DataTable = Me._yoyakuInfoBasicTable

        '料金
        Me.setKingaku(dt, row, Me.ChargeTotal)

        'キャンセル料
        Me.setKingaku(dt, row, Me.Cancel)

        '割引額
        Me.setKingaku(dt, row, Me.WaribikiTotal)

        '取扱手数料
        Me.setKingaku(dt, row, Me.ToriatukaiFee)

        '請求額
        Me.setKingaku(dt, row, Me.SeikyuTotal)

        Me.grdKingaku.DataSource = dt

        '行ヘッダを設定
        grdKingaku.Rows(NoRowGrdKingakuCharge).Caption = "料金"
        grdKingaku.Rows(NoRowGrdKingakuCancel).Caption = "キャンセル料"
        grdKingaku.Rows(NoRowGrdKingakuWaribiki).Caption = "割引額"
        grdKingaku.Rows(NoRowGrdKingakuToriatukaiFee).Caption = "取扱手数料"
        grdKingaku.Rows(NoRowGrdKingakuSeikyu).Caption = "請求額"

        '割引行を赤文字
        grdKingaku.Styles.Add("redChar")
        grdKingaku.Styles("redChar").ForeColor = Color.Red
        grdKingaku.Rows(NoRowGrdKingakuWaribiki).Style = grdKingaku.Styles("redChar")

        ' 金額表示欄桁数付与
        CommonHakken.setGridLength(Me.grdKingaku, "colKingaku", 7)

        '料金、割引、請求を非活性
        CommonHakken.disableGrdCell(Me.grdKingaku,
                          grdKingaku.Rows(NoRowGrdKingakuCharge),
                          grdKingaku.Rows(NoRowGrdKingakuWaribiki),
                          grdKingaku.Rows(NoRowGrdKingakuSeikyu))
    End Sub

    ''' <summary>
    ''' 金額グリッド２の設定
    ''' </summary>
    Private Sub setKingaku2()
        Dim dt2 As DataTable = New DataTable()
        dt2.Columns.Add(NameColGrdKingaku)
        Dim row As DataRow = dt2.NewRow

        '既入金額
        Me.setKingaku(dt2, row, Me.PreNyukinGaku)

        '入金額
        Me.setKingaku(dt2, row, Me.Nyuukin)

        ''内金請求額
        'Me.setKingaku(dt2, row, Me.UtikinSeikyu)

        '残金
        Me.setKingaku(dt2, row, Me.Zankin)

        '予約センター返金
        Me.setKingaku(dt2, row, Me.YoyakuCenterHenkin)

        'おつり
        Me.setKingaku(dt2, row, Me.Oturi)

        Me.grdKingaku2.DataSource = dt2

        '行ヘッダを設定
        grdKingaku2.Rows(NoRowGrdKingaku2PreNyukinGaku).Caption = "既入金額"
        grdKingaku2.Rows(NoRowGrdKingaku2Nyukin).Caption = "入金額"
        grdKingaku2.Rows(NoRowGrdKingaku2Zankin).Caption = "残金"
        'grdKingaku2.Rows(NoRowGrdKingaku2UchikinSeikyu).Caption = "内金請求額"
        grdKingaku2.Rows(NoRowGrdKingaku2YoyakuCenterHenkin).Caption = "予約センター返金"
        grdKingaku2.Rows(NoRowGrdKingaku2Otsuri).Caption = "おつり"

        '既入金額、入金、残金、お釣を非活性
        CommonHakken.disableGrdCell(Me.grdKingaku2,
                          grdKingaku2.Rows(NoRowGrdKingaku2PreNyukinGaku),
                          grdKingaku2.Rows(NoRowGrdKingaku2Nyukin),
                          grdKingaku2.Rows(NoRowGrdKingaku2Zankin),
                          grdKingaku2.Rows(NoRowGrdKingaku2Otsuri))

        If Not Me._existsHurikomiNyuukin Then
            '予約センター返金がない場合、非活性
            CommonHakken.disableGrdCell(grdKingaku2,
                                        grdKingaku2.Rows(NoRowGrdKingaku2YoyakuCenterHenkin))
        End If
    End Sub

    ''' <summary>
    ''' 金額の設定
    ''' </summary>
    Private Sub setKingaku(ByRef dt As DataTable, row As DataRow, value As Integer)
        row = dt.NewRow()
        row(NameColGrdKingaku) = $"{value:#,0}"
        dt.Rows.Add(row)
    End Sub
#End Region
#End Region

#Region "チェック処理"
#Region "表示チェック"
    ''' <summary>
    ''' 表示チェック
    ''' </summary>
    ''' <returns></returns>
    Private Function isValidDisp() As Boolean
        '予約キャンセルチェック
        Dim cancelFlg As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("CANCEL_FLG"), "")
        If Not String.IsNullOrWhiteSpace(cancelFlg) Then
            If Me.IsTeiki Then
                'キャンセル済み　かつ　定期なら発券不可
                MyBase.F10Key_Enabled = False
            End If

            'キャンセル済み
            CommonProcess.createFactoryMsg.messageDisp("E90_034", "予約情報", "キャンセル")
            Me.IsUpdated = True
            Me.Close()
            Return False
        End If

        '入金内容チェック
        Dim seisanHoho As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("SEISAN_HOHO"), "")
        'If seisanHoho.Equals(FixedCdYoyaku.PaymentHoho.agt) = False _
        'AndAlso Me.SeikyuTotal <> Me.PreNyukinGaku Then
        '    '精算方法が代理店でない　かつ　請求額と入金額総計が異なる場合
        '    'TODO:メッセージ
        '    CommonProcess.createFactoryMsg.messageDisp("E90_006", "請求額を超える入金", "決済")
        '    Me.Close()
        'End If

        '過剰オンライン決済チェック
        If Me.OnlineCredit > Me.SeikyuTotal Then
            CommonProcess.createFactoryMsg.messageDisp("E90_006", "請求額を超える入金", "決済")
            Me.Close()
        End If

        Me.IsAlreadyHakken = False

        If Me.IsTeiki Then
            '定期のチェック
            If Not Me.isValidDispTeiki() Then
                Return False
            End If

        ElseIf Me.IsKikaku Then
            '企画のチェック
            If Not Me.isValidDispKikaku() Then
                Return False
            End If
        End If

        '使用中チェック
        Dim usingFlg As String = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("USING_FLG")
        If Not String.IsNullOrWhiteSpace(usingFlg) Then
            CommonProcess.createFactoryMsg.messageDisp("E90_040")
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' 表示チェック（定期）
    ''' </summary>
    ''' <returns></returns>
    Private Function isValidDispTeiki() As Boolean
        '運休チェック
        Dim unkyuKbn As String = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("UNKYU_KBN")
        Dim saikouKakuteiKbn As String = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("SAIKOU_KAKUTEI_KBN")
        saikouKakuteiKbn = If(saikouKakuteiKbn Is Nothing, "", saikouKakuteiKbn)
        If Not String.IsNullOrWhiteSpace(unkyuKbn) Then
            CommonProcess.createFactoryMsg.messageDisp("E02_046", "運休")

            '発券ボタンを非活性
            MyBase.F10Key_Enabled = False
            Return False
        End If

        '催行未確定
        If saikouKakuteiKbn.Equals(FixedCd.SaikouKakuteiKbn.Tyushi) Then
            CommonProcess.createFactoryMsg.messageDisp("E02_046", "催行未確定")

            '発券ボタンを非活性
            MyBase.F10Key_Enabled = False
            Return False
        End If

        '発券済みチェック（全金発券または残金発券の場合エラー）
        Dim hakkenNaiyo As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("HAKKEN_NAIYO"), "")
        If hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zenkin)) _
        OrElse hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zankin)) Then

            Me.IsAlreadyHakken = True

            CommonProcess.createFactoryMsg.messageDisp("E90_069", "予約情報", UriageKakutei)

            ''発券ボタンを非活性
            'MyBase.F10Key_Enabled = False
            'Return False

        ElseIf hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Uchikin)) Then

            '内金発券の場合、画面を閉じる
            CommonProcess.createFactoryMsg.messageDisp("E90_006", "内金発券済みの予約情報", UriageKakutei)

            Me.IsUpdated = True
            Me.Close()
            Return False

        End If

        '補助席/飛び席チェック
        Dim yoyakuZasekiKbn As String = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("YOYAKU_ZASEKI_KBN")
        yoyakuZasekiKbn = If(yoyakuZasekiKbn Is Nothing, "", yoyakuZasekiKbn)
        Dim tobiSeatFlg As String = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("TOBI_SEAT_FLG")
        tobiSeatFlg = If(tobiSeatFlg Is Nothing, "", tobiSeatFlg)


        If yoyakuZasekiKbn.Equals(CommonHakken.YoyakuSeatKbnHojoSeat) Then
            CommonProcess.createFactoryMsg.messageDisp("I02_001", "補助席")

        ElseIf yoyakuZasekiKbn.Equals(CommonHakken.YoyakuSeatKbn1FSeat) Then
            CommonProcess.createFactoryMsg.messageDisp("I02_001", "１Ｆ席")
        End If

        If tobiSeatFlg.Equals(CommonHakken.TobeSeatYes) Then
            CommonProcess.createFactoryMsg.messageDisp("I02_001", "とび席")
        End If

        Return True
    End Function

    ''' <summary>
    ''' 表示チェック（企画）
    ''' </summary>
    ''' <returns></returns>
    Private Function isValidDispKikaku() As Boolean
        '催行中止チェック
        Dim saikouKakuteiKbn As String = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("SAIKOU_KAKUTEI_KBN")
        saikouKakuteiKbn = If(saikouKakuteiKbn Is Nothing, "", saikouKakuteiKbn)
        If saikouKakuteiKbn.Equals(FixedCd.SaikouKakuteiKbn.Tyushi) Then
            CommonProcess.createFactoryMsg.messageDisp("E02_046", "催行未確定")

            '発券ボタンを非活性
            MyBase.F10Key_Enabled = False
            Return False
        End If

        '払戻チェック
        Dim dHakkenKingaku As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("HAKKEN_KINGAKU"), 0)
        Dim hakkenKingaku As Integer = Convert.ToInt32(dHakkenKingaku)
        Dim hakkenNaiyo As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("HAKKEN_NAIYO"), "")
        If hakkenKingaku <> 0 Then
            '発券金額存在時
            If Me.ChargeTotal <= hakkenKingaku _
            AndAlso hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zenkin)) Then

                Me.IsAlreadyHakken = True

                CommonProcess.createFactoryMsg.messageDisp("E90_069", "予約情報", UriageKakutei)

                '発券ボタンを非活性
                'Me.F10Key_Enabled = False
                'Return False

            ElseIf hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Uchikin)) Then

                '内金発券の場合、画面を閉じる
                CommonProcess.createFactoryMsg.messageDisp("E90_006", "内金発券済みの予約情報", UriageKakutei)

                Me.IsUpdated = True
                Me.Close()
                Return False

            End If
        End If

        'コースコントロールチェック
        Dim crsConrolInfo As DataTable = Me.getKey3(CrsControlInfoKey1_11, CrsControlInfoKey2_01)
        If crsConrolInfo IsNot Nothing Then
            Dim Key3CrsControl As String = crsConrolInfo.Rows(0).Field(Of String)("KEY_3")

            Dim last1DigitKey3CrsControl As String = Key3CrsControl.Substring(Key3CrsControl.Length - 1, 1)
            Dim last1DigitCrsCd As String = Me.txtCourseCd.Text.Substring(Me.txtCourseCd.Text.Length - 1, 1)
            'コースコード下1桁=コースコントロールのKEY3の下1桁
            If last1DigitCrsCd.Equals(last1DigitKey3CrsControl) Then
                CommonProcess.createFactoryMsg.messageDisp("E02_046", UriageKakutei & "不可")

                '発券ボタンを非活性
                Me.F10Key_Enabled = False
                Return False
            End If
        End If

        Return True
    End Function
#End Region

#Region "入力チェック"
    ''' <summary>
    ''' 入力チェック
    ''' </summary>
    ''' <returns></returns>
    Private Function isValidInputHakken() As Boolean
        '割引額チェック
        If Me.ChargeTotal < Me.WaribikiTotal Then
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引合計", "コース料金未満")
            Return False
        End If

        '請求不正チェック
        If Me.SeikyuTotal < 0 Then
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引合計", "請求額未満")
            Return False
        End If

        'キャンセル料チェック
        If Me.Cancel < 0 Then
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "キャンセル料", "0以上")
            Return False
        End If

        '取扱手数料チェック
        If Me.ToriatukaiFee < 0 Then
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "取扱手数料", "0以上")
            Return False
        End If

        '予約センター返金チェック
        If Me.YoyakuCenterHenkin < 0 Then
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "予約センター返金", "0以上")
            Return False
        End If

        '割引グリッドのチェック
        If Not Me.isValidGrdWaribiki() Then
            Return False
        End If

        '精算グリッドのチェック
        If Not Me.isValidGrdSeisan() Then
            Return False
        End If

        If Me.IsTeiki Then
            If Not Me.isValidInputHakkenTeiki() Then
                Return False
            End If

        ElseIf Me.IsKikaku Then
            If Not Me.isValidInputHakkenKikaku() Then
                Return False
            End If
        End If

        Return True
    End Function

    ''' <summary>
    ''' 入力チェック（定期）
    ''' </summary>
    ''' <returns></returns>
    Private Function isValidInputHakkenTeiki() As Boolean
        '入金なしチェック
        If Me._existsWaribiki = False AndAlso Me.Nyuukin = 0 Then
            CommonProcess.createFactoryMsg.messageDisp("E90_023", "入金額")
            Return False
        End If

        '入金不足チェック
        If Me.Nyuukin < Me.SeikyuTotal Then
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "入金額", "請求額以上")
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' 入力チェック（企画）
    ''' </summary>
    ''' <returns></returns>
    Private Function isValidInputHakkenKikaku() As Boolean
        '2重全金発券チェック
        Dim hakkenNaiyo As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("HAKKEN_NAIYO"), "")
        If hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zenkin)) Then
            CommonProcess.createFactoryMsg.messageDisp("E90_069", "予約情報", UriageKakutei)
            Return False
        End If

        '発券額チェック
        Dim dHakkenKingaku As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("HAKKEN_KINGAKU"), 0)
        Dim hakkenKingaku As Integer = Convert.ToInt32(dHakkenKingaku)
        If Me.ChargeTotal = hakkenKingaku Then
            CommonProcess.createFactoryMsg.messageDisp("E90_069", "予約情報", UriageKakutei)
            Return False
        End If

        '入金不足（全金）チェック
        If Me.Nyuukin < Me.SeikyuTotal Then
            CommonProcess.createFactoryMsg.messageDisp("E70_003", "入金額", "請求額以上")
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' 割引グリッドチェック
    ''' </summary>
    ''' <returns></returns>
    Private Function isValidGrdWaribiki() As Boolean
        Me._existsWaribiki = False

        Dim crsCd As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_CD"), "")
        Dim syuptDay As Integer = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY"), 0)

        '割引グリッド上のチェック
        For Each row As Row In Me.grdWaribikiCharge.Rows
            'ヘッダー行飛ばす
            If row.Index = 0 Then
                Continue For
            End If

            '値の取り出し
            Dim waribikiName As String = TryCast(Me.grdWaribikiCharge.Item(row.Index, "WARIBIKI_NAME"), String) '割引名
            Dim waribikiCd As String = TryCast(Me.grdWaribikiCharge.Item(row.Index, "WARIBIKI_CD"), String) '割引コード
            Dim tani As String = If(TryCast(row.Item("WARIBIKI_KBN"), String), "")  '単位
            Dim chargeName As String = TryCast(Me.grdWaribikiCharge.Item(row.Index, "CHARGE_NAME"), String) '料金区分
            Dim jininName As String = TryCast(Me.grdWaribikiCharge.Item(row.Index, "CHARGE_KBN_JININ_NAME"), String) '人員
            '割引
            Dim strWaribiki As String = ""
            If Not CommonHakken.isNull(row.Item("WARIBIKI")) Then
                strWaribiki = row.Item("WARIBIKI").ToString()
            End If
            Dim waribiki As Integer = 0
            Integer.TryParse(strWaribiki, waribiki)
            '割引人数
            Dim strWaribikiNinzu As String = ""
            If Not CommonHakken.isNull(row.Item("WARIBIKI_NINZU")) Then
                strWaribikiNinzu = row.Item("WARIBIKI_NINZU").ToString()
            End If
            Dim waribikiNinzu As Integer = 0
            Integer.TryParse(strWaribikiNinzu, waribikiNinzu)
            '割引可能な人数の上限
            Dim strMaxWaribikiNinzu As String = ""
            If Not CommonHakken.isNull(row.Item("WARIBIKI_APPLICATION_NINZU")) Then
                strMaxWaribikiNinzu = row.Item("WARIBIKI_APPLICATION_NINZU").ToString()
            End If
            Dim maxWaribikiNinzu As Integer = 0
            Integer.TryParse(strMaxWaribikiNinzu, maxWaribikiNinzu)
            Dim carriageWaribikiFlg As String = If(TryCast(row.Item("CARRIAGE_WARIBIKI_FLG"), String), "")
            '割引金額
            Dim strWaribikiKinagku As String = ""
            If Not CommonHakken.isNull(row.Item("WARIBIKI_KINGAKU")) Then
                strWaribikiKinagku = row.Item("WARIBIKI_KINGAKU").ToString()
            End If
            Dim waribikiKingaku As Integer = 0
            Integer.TryParse(strWaribikiKinagku, waribikiKingaku)
            '備考
            Dim biko As String = If(TryCast(row.Item("WARIBIKI_BIKO"), String), "")

            If String.IsNullOrWhiteSpace(waribikiName) Then
                '割引コードが空白アイテムの場合

                '割引コード, 名称必須チェック
                '全部空でなければエラー
                If Not (waribiki = 0 _
                AndAlso String.IsNullOrWhiteSpace(chargeName) _
                AndAlso String.IsNullOrWhiteSpace(jininName) _
                AndAlso waribikiNinzu = 0 _
                AndAlso String.IsNullOrWhiteSpace(biko)) Then

                    CommonRegistYoyaku.changeGridBackColor(row.Index, 6, Me.grdWaribikiCharge)
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引コード")
                    Return False
                End If

            Else
                '割引コード選択の場合
                Me._existsWaribiki = True

                '割引必須チェック
                If waribiki <= 0 Then

                    CommonRegistYoyaku.changeGridBackColor(row.Index, 7, Me.grdWaribikiCharge)
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引")
                    Return False
                End If

                If tani.Equals(FixedCdYoyaku.Tani.Per) Then
                    '割引率不正チェック
                    If waribiki > 100 Then

                        CommonRegistYoyaku.changeGridBackColor(row.Index, 7, Me.grdWaribikiCharge)
                        CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引率", "100未満の値")
                        Return False
                    End If

                ElseIf tani.Equals(FixedCdYoyaku.Tani.Yen) Then
                    '割引額不正チェック
                    For roomType = CommonHakken.One1R To CommonHakken.FiveIjyou1R
                        Dim seikiCharge As Integer = CommonHakken.getSeikiCharge(jininName, roomType, Me._yoyakuInfoCrsChargeChargeKbn)
                        If 0 < seikiCharge AndAlso seikiCharge < waribiki Then

                            CommonRegistYoyaku.changeGridBackColor(row.Index, 7, Me.grdWaribikiCharge)
                            CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引額", "コース料金以下")
                            Return False
                        End If
                    Next

                Else
                    '単位不正チェック
                    CommonProcess.createFactoryMsg.messageDisp("E90_016", "割引の単位")
                    Return False
                End If

                '料金区分必須チェック
                If Me.IsStayAri = False Then
                    If String.IsNullOrWhiteSpace(chargeName) Then

                        CommonRegistYoyaku.changeGridBackColor(row.Index, 12, Me.grdWaribikiCharge)
                        CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "料金区分")
                        Return False
                    End If
                End If

                '割引適用者必須チェック
                If String.IsNullOrWhiteSpace(jininName) Then

                    CommonRegistYoyaku.changeGridBackColor(row.Index, 14, Me.grdWaribikiCharge)
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引適用者")
                    Return False
                End If

                '割引人数必須チェック
                If waribikiNinzu = 0 Then

                    CommonRegistYoyaku.changeGridBackColor(row.Index, 25, Me.grdWaribikiCharge)
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引人数")
                    Return False
                End If

                '割引金額チェック
                If waribikiKingaku <= 0 Then
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の入力", "割引金額")
                    Return False
                End If

                '割引適用人数チェック
                If maxWaribikiNinzu < waribikiNinzu Then

                    CommonRegistYoyaku.changeGridBackColor(row.Index, 25, Me.grdWaribikiCharge)
                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "割引の人数", "割引適用人数内")
                    Return False
                End If

                'ホリデーチケット適用コース確認
                If Me.isAppliedCourseForHolidayTicket(crsCd, waribikiCd) = False Then

                    CommonRegistYoyaku.changeGridBackColor(row.Index, 6, Me.grdWaribikiCharge)
                    CommonProcess.createFactoryMsg().messageDisp("E02_018")
                    Return False
                End If

                'ホリデーチケット適用範囲確認
                If Me.isHolidayTicketScopeOfApplication(syuptDay, waribikiCd) = False Then

                    CommonRegistYoyaku.changeGridBackColor(row.Index, 6, Me.grdWaribikiCharge)
                    CommonProcess.createFactoryMsg().messageDisp("E02_019")
                    Return False
                End If

            End If    'ここまで割引コード選択の場合
        Next 'ここまで割引グリッドのチェック

        Return True
    End Function

    ''' <summary>
    ''' ホリデーチケット適用コース確認
    ''' </summary>
    ''' <param name="crsCd">コースコード</param>
    ''' <param name="waribikiCd">割引コード</param>
    ''' <returns>検証結果</returns>
    Private Function isAppliedCourseForHolidayTicket(crsCd As String, waribikiCd As String) As Boolean

        If waribikiCd <> "004" Then
            ' 割引コードがホリデーチケット以外の場合、チェックなし
            Return True
        End If

        Dim entity As New HolidayCrsCdEntity()
        entity.crsCd.Value = crsCd

        Dim s02_0103Da As New S02_0103Da()
        Dim holidayCrsInfo As DataTable = s02_0103Da.getHolidayCrs(entity)

        If holidayCrsInfo.Rows.Count < CommonRegistYoyaku.ZERO Then

            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' ホリデーチケット適用範囲確認
    ''' </summary>
    ''' <param name="syuptDay">出発日</param>
    ''' <param name="waribikiCd">割引コード</param>
    ''' <returns>検証結果</returns>
    Private Function isHolidayTicketScopeOfApplication(syuptDay As Integer, waribikiCd As String) As Boolean

        If waribikiCd <> "004" Then
            ' 割引コードがホリデーチケット以外の場合、チェックなし
            Return True
        End If

        Dim entity As New HolidayApplicationDayEntity()
        entity.applicationDayFrom.Value = syuptDay
        entity.applicationDayTo.Value = syuptDay

        Dim s02_0103Da As New S02_0103Da()
        Dim holidayCrsInfo As DataTable = s02_0103Da.getHolidayApplicationDay(entity)

        If holidayCrsInfo.Rows.Count < CommonRegistYoyaku.ZERO Then

            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' 精算グリッドチェック
    ''' </summary>
    ''' <returns></returns>
    Private Function isValidGrdSeisan() As Boolean
        '精算グリッド上のチェック
        For Each row As Row In Me.grdSeisan.Rows
            'ヘッダー行飛ばす
            If row.Index = 0 Then
                Continue For
            End If

            '値の取り出し
            '精算項目コード
            Dim seisanKoumokuCd As String = If(TryCast(row.Item("SEISAN_KOUMOKU_CD"), String), "")
            Dim seisanKoumokuName As String = TryCast(Me.grdSeisan.Item(row.Index, "SEISAN_KOUMOKU_NAME"), String) '精算項目名
            '金額
            Dim strKingaku As String = ""
            If Not CommonHakken.isNull(row.Item("KINGAKU")) Then
                strKingaku = row.Item("KINGAKU").ToString()
            End If
            Dim kingaku As Integer = 0
            Integer.TryParse(strKingaku, kingaku)
            Dim utiwake As String = TryCast(row.Item("HURIKOMI_KBN"), String) '内訳
            Dim biko As String = TryCast(row.Item("BIKO"), String) '備考


            If String.IsNullOrWhiteSpace(seisanKoumokuName) Then
                '精算項目が空白アイテムの場合

                '割引コード, 名称必須チェック
                '全部空でなければエラー
                If Not (kingaku = 0 _
                        AndAlso String.IsNullOrWhiteSpace(utiwake) _
                        AndAlso String.IsNullOrWhiteSpace(biko)) Then

                    CommonProcess.createFactoryMsg.messageDisp("E70_003", "入金の入力", "精算項目")
                    Return False
                End If

            Else
                '精算項目が選択されている場合

                '金額必須チェック
                '現金、ｸﾚｼﾞｯﾄ、予約C振込、オンライン決済は初期表示の為、0でも可
                If Not seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)) _
                AndAlso Not seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.credit_card)) _
                AndAlso Not seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.hurikomi_yoyaku_center)) _
                AndAlso Not seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.online_credit)) Then
                    If kingaku = 0 Then
                        CommonProcess.createFactoryMsg.messageDisp("E70_003", "入金の入力", "金額")
                        Return False
                    End If
                End If

                '精算項目がクレジットで金額が請求金額を超えておつりが発生する場合、処理中断
                If (seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.credit_card)) Or
                    seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.online_credit))) _
                                    AndAlso kingaku > Me.SeikyuTotal Then
                    CommonProcess.createFactoryMsg.messageDisp("E90_006", "精算項目がクレジットの場合", "返金") '("E02_054")
                    Return False
                End If

                '船車券チェック
                If seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sensya_ken)) _
                OrElse seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.bc)) Then
                    '船車券の入金の場合、出発日が未来ならエラーとする
                    Dim syuptDay As Date = Date.Parse(Me.ucoSyuptDay.Value.ToString())
                    Dim isMiraiSyuptDay As Boolean = CommonConvertUtil.ChkWhenDay(syuptDay, CommonConvertUtil.WhenMirai)
                    If isMiraiSyuptDay Then
                        CommonProcess.createFactoryMsg().messageDisp("E02_022")
                        Return False
                    End If
                End If
            End If 'ここまで精算項目コード選択の場合
        Next 'ここまで精算グリッドのチェック

        Return True

    End Function
#End Region
#End Region

#Region "売上確定処理"
    ''' <summary>
    ''' 券番の採番
    ''' </summary>
    Private Sub numberingKenNo()
        Dim syuptDay As Integer = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY"), 0)
        Me._newKenNo = CommonHakken.numberingKenNo(Me.IsTeiki, syuptDay)

        Me._hakkenInfoSeq1 = Me._newKenNo.PadLeft(10, "0"c).Substring(5, 1)
        Me._hakkenInfoSeq2 = Integer.Parse(Me._newKenNo.PadLeft(10, "0"c).Substring(6, 4))
    End Sub

    ''' <summary>
    ''' 割引種別単位の情報を設定
    ''' </summary>
    Private Sub setInfosByWaribikiType()
        'フィールドへインスタンスを設定
        '割引コード種別、人員コード単位の参加人数
        Me._infoByWaribikiCdJininCd = CommonHakken.createInfoWaribikiCdJininCd

        '割引種別単位の参加人数
        Me._ninzuByWaribikiType = New Dictionary(Of String, Integer)

        '割引種別ごとに各精算項目へ按分をかける請求金額
        Me._seikyuByWaribikiType = New Dictionary(Of String, Integer)

        '割引種別ごとに各精算項目へ按分をかける割引金額
        Me._waribikiByWaribikiType = New Dictionary(Of String, Integer)

        '割引種別ごとの正規料金 
        Me._seikiChargeByWaribikiType = New Dictionary(Of String, Integer)


        '割引グリッドの値を集計
        For Each row As Row In Me.grdWaribikiCharge.Rows
            'ヘッダー行なら次の行へ
            If row.Index() = 0 Then
                Continue For
            End If
            '空なら次の行へ
            Dim waribikiCd As String = If(TryCast(row.Item("WARIBIKI_CD"), String), "")
            If waribikiCd.Equals("") Then
                Continue For
            End If

            'グリッドの値を取り出し
            '人数
            Dim strNinzuByGrdRow As String = ""
            If Not CommonHakken.isNull(row.Item("WARIBIKI_NINZU")) Then
                strNinzuByGrdRow = row.Item("WARIBIKI_NINZU").ToString()
            End If
            Dim ninzuByGrdRow As Integer = 0
            Integer.TryParse(strNinzuByGrdRow, ninzuByGrdRow)
            '割引金額
            Dim strWaribikiKingakuByGrdRow As String = ""
            If Not CommonHakken.isNull(row.Item("WARIBIKI_KINGAKU")) Then
                strWaribikiKingakuByGrdRow = row.Item("WARIBIKI_KINGAKU").ToString()
            End If
            Dim waribikiKingakuByGrdRow As Integer = 0
            Integer.TryParse(strWaribikiKingakuByGrdRow, waribikiKingakuByGrdRow)


            '割引種別単位でデータを集計する
            Dim waribikiType As String = If(TryCast(row.Item("WARIBIKI_TYPE_KBN"), String), "")

            '割引コード種別、人員コード単位の人数にデータを分解
            CommonHakken.setInfoByWaribikiCdJininCd(row,
                                          waribikiCd,
                                          waribikiType,
                                          Me._infoByWaribikiCdJininCd)

            '割引種別単位の人数
            CommonHakken.setNinzuByWaribikiType(waribikiType, ninzuByGrdRow, Me._ninzuByWaribikiType)

            '割引種別ごとに各精算項目へ按分をかける請求金額
            Me.setSeikyuByWaribikiType(row,
                                       waribikiType,
                                       waribikiKingakuByGrdRow,
                                       ninzuByGrdRow)

        Next 'ここまで割引グリッドのループ

        '割引付きの請求額の合計
        Dim sumWaribikiSeikyu As Integer = Me._seikyuByWaribikiType.Sum(Function(seikyu) seikyu.Value)
        Dim sumWaribiki As Integer = Me._waribikiByWaribikiType.Sum(Function(waribiki) waribiki.Value)

        '残額を[割引種別：正規料金]として割引種別ごとの請求へ追加
        Dim seikyuSeikiCharge As Integer = Me.ChargeTotal - sumWaribikiSeikyu - sumWaribiki

        If seikyuSeikiCharge <> 0 Then
            Me._seikyuByWaribikiType.Add(CommonHakken.WaribikiTypeSeikiCharge, seikyuSeikiCharge)
            Me._seikiChargeByWaribikiType.Add(CommonHakken.WaribikiTypeSeikiCharge, seikyuSeikiCharge)
        End If
        '正規料金の人数を集計
        CommonHakken.setNinzuSeikiChargeAfterWaribiki(Me._yoyakuInfoCrsChargeChargeKbn,
                                    Me._infoByWaribikiCdJininCd)
    End Sub

    ''' <summary>
    ''' 割引種別ごとに各精算項目へ按分をかける請求金額
    ''' </summary>
    ''' <param name="row"></param>
    ''' <param name="waribikiType"></param>
    ''' <param name="waribikiKingakuByGrdRow"></param>
    Private Sub setSeikyuByWaribikiType(row As Row,
                                        waribikiType As String,
                                        waribikiKingakuByGrdRow As Integer,
                                        waribikiNinzuByGrdRow As Integer)

        '人員
        Dim jininCd As String = If(TryCast(row.Item("CHARGE_KBN_JININ_CD"), String), "")
        '運賃割引フラグ
        Dim carriageWaribikiFlg As String = If(TryCast(row.Item("CARRIAGE_WARIBIKI_FLG"), String), "")

        '室タイプ１～５の請求額を集計
        For roomType = CommonHakken.One1R To CommonHakken.FiveIjyou1R

            '割引人数
            Dim strWaribikiApplicationNinzuN As String = ""
            Dim waribikiApplicationNinzuN As Integer = 0
            If Not CommonHakken.isNull(row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}")) Then
                strWaribikiApplicationNinzuN = row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}").ToString()
            End If
            Integer.TryParse(strWaribikiApplicationNinzuN, waribikiApplicationNinzuN)

            '正規料金、人数を取得
            Dim seikiCharge As Integer = CommonHakken.getSeikiCharge(jininCd, roomType, Me._yoyakuInfoCrsChargeChargeKbn)
            Dim seikiNinzu As Integer = CommonHakken.getSeikiNinzu(jininCd, roomType, Me._yoyakuInfoCrsChargeChargeKbn)

            '割引種別単位の正規料金へ加算
            If Not Me._seikiChargeByWaribikiType.ContainsKey(waribikiType) Then
                Me._seikiChargeByWaribikiType.Add(waribikiType, 0)
            End If
            Me._seikiChargeByWaribikiType(waribikiType) += seikiCharge * waribikiApplicationNinzuN

            '割引種別単位の請求額へ加算
            If Not Me._seikyuByWaribikiType.ContainsKey(waribikiType) Then
                Me._seikyuByWaribikiType.Add(waribikiType, 0)
            End If
            Me._seikyuByWaribikiType(waribikiType) += seikiCharge * waribikiApplicationNinzuN
        Next 'ここまで室タイプ１～５のループ

        '割引種別単位の請求額から割引金額を減算
        Me._seikyuByWaribikiType(waribikiType) -= waribikiKingakuByGrdRow

        '割引種別単位の割引額へ割引金額を加算
        If Not Me._waribikiByWaribikiType.ContainsKey(waribikiType) Then
            Me._waribikiByWaribikiType.Add(waribikiType, 0)
        End If
        Me._waribikiByWaribikiType(waribikiType) += waribikiKingakuByGrdRow
    End Sub

    ''' <summary>
    ''' 精算項目単位の未按分金（入金額）を集計
    ''' </summary>
    Private Function setUndistributedKinBySeisanKoumoku() As Dictionary(Of String, Integer)

        Dim nyuukinBySeisanKoumoku As New Dictionary(Of String, Integer)
        Dim _GenkinSeisan As Boolean = False    '精算項目「現金」での精算処理済みフラグ（False：未処理、True：処理済み）
        '精算グリッドの値を取り出し
        For Each row As Row In Me.grdSeisan.Rows
            'ヘッダー行なら次の行へ
            If row.Index() = 0 Then
                Continue For
            End If
            '空なら次の行へ
            Dim seisanKoumokuCd As String = If(TryCast(row.Item("SEISAN_KOUMOKU_CD"), String), "")
            If seisanKoumokuCd.Equals("") Then
                Continue For
            End If

            'グリッドの値を取り出し
            '金額
            Dim strKingakuByGrdRow As String = ""
            If Not CommonHakken.isNull(row.Item("TAISYAKU_KINGAKU")) Then
                strKingakuByGrdRow = row.Item("TAISYAKU_KINGAKU").ToString()
            End If
            Dim kingakuByGrdRow As Integer = 0
            Integer.TryParse(strKingakuByGrdRow, kingakuByGrdRow)

            '0なら次の行へ
            If kingakuByGrdRow = 0 Then
                Continue For
            End If

            '精算項目単位の入金
            If Not nyuukinBySeisanKoumoku.ContainsKey(seisanKoumokuCd) Then
                nyuukinBySeisanKoumoku.Add(seisanKoumokuCd, 0)
            End If

            '精算項目コードに応じて、返金額を減算
            If seisanKoumokuCd.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)) _
            And _GenkinSeisan = False Then
                '現金の場合、請求額からおつりを減算
                If kingakuByGrdRow >= Me.Oturi Then
                    kingakuByGrdRow -= Me.Oturi
                    nyuukinBySeisanKoumoku(seisanKoumokuCd) += kingakuByGrdRow
                    _GenkinSeisan = True    '精算項目「現金」での精算処理完了
                Else
                    '減算するとマイナスになる場合は、精算項目「現金戻」におつりを計上する
                    If Not nyuukinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi)) Then
                        nyuukinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi), Me.Oturi)
                    Else
                        nyuukinBySeisanKoumoku(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi)) = Me.Oturi

                    End If
                    '精算項目「現金戻」におつりを計上したため、精算項目「現金」におつりは減算せずそのまま計上
                    nyuukinBySeisanKoumoku(seisanKoumokuCd) += kingakuByGrdRow
                End If
            Else
                nyuukinBySeisanKoumoku(seisanKoumokuCd) += kingakuByGrdRow
            End If
            '
        Next 'ここまで精算グリッドのループ

        '現金精算なし　かつ　おつりが発生時
        If Not nyuukinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)) Then
            '貸借先の精算項目へおつりを追加
            Me.setTaisyakuTaioHenkin(nyuukinBySeisanKoumoku)
        End If

        Return nyuukinBySeisanKoumoku
    End Function

    ''' <summary>
    ''' 貸借対応先（「現金戻」）の返金の設定
    ''' </summary>
    Private Sub setTaisyakuTaioHenkin(ByRef nyuukinBySeisanKoumoku As Dictionary(Of String, Integer))
        '振込時は追加しないで終了
        If Me._existsHurikomiNyuukin Then
            Return
        End If

        '精算グリッドに精算項目がない場合、終了
        If nyuukinBySeisanKoumoku.Any() = False Then
            Return
        End If

        ''精算項目ごとの入金から最大金額の精算項目コードを取得
        'Dim maxKingakuSeisanItem As String = nyuukinBySeisanKoumoku.OrderByDescending(Function(item) item.Value).First.Key
        ''最大金額の貸借対応コードを取得
        'Dim taisyakuTaioCd As String = Me.getTaisyakuTaioCd(maxKingakuSeisanItem)

        'If taisyakuTaioCd.Equals(ErrorTaisyakuTaioCd) Then
        '    '貸借先がない精算項目で返金が発生時には、オペレートミスとして例外スロー
        '    CommonProcess.createFactoryMsg.messageDisp("E02_054")
        '    Throw New Exception
        'Else
        '    '貸借先へおつり金額を格納
        '    nyuukinBySeisanKoumoku.Add(taisyakuTaioCd, Me.Oturi)
        'End If

        '貸借先「現金戻」へおつり金額を格納
        If Me.Oturi > 0 Then
            If Not nyuukinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi)) Then
                nyuukinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi), Me.Oturi)
            Else
                nyuukinBySeisanKoumoku(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin_modosi)) += Me.Oturi
            End If
        End If
    End Sub
#End Region

#Region "DataAccess"
#Region "検索"
    ''' <summary>
    ''' コースコントロール情報の取得
    ''' </summary>
    ''' <param name="key1">KEY1</param>
    ''' <param name="key2">KEY2</param>
    ''' <returns></returns>
    Private Function getKey3(key1 As String, key2 As String) As DataTable
        Dim da As New S02_0602Da
        Dim dt As DataTable = da.getKey3CrsControlInfo(key1, key2)

        Return If(CommonHakken.existsDatas(dt), dt, Nothing)
    End Function

    ''' <summary>
    ''' 入返金情報の取得
    ''' </summary>
    ''' <param name="yoyakuKbn"></param>
    ''' <param name="yoyakuNo"></param>
    ''' <returns></returns>
    Private Function getNyuukin(yoyakuKbn As String, yoyakuNo As Integer) As DataTable
        Dim da As New S02_0602Da
        Dim dt As DataTable = da.getNyuukin(yoyakuKbn, yoyakuNo)

        Return If(CommonHakken.existsDatas(dt), dt, Nothing)
    End Function

    ''' <summary>
    ''' 貸借対応コードの取得
    ''' </summary>
    ''' <param name="seisanKoumokuCd"></param>
    ''' <returns></returns>
    Private Function getTaisyakuTaioCd(seisanKoumokuCd As String) As String
        Dim da As New S02_0602Da
        Dim dt As DataTable = da.getTaisyakuTaioCd(seisanKoumokuCd)

        Dim taisyakuTaioCd = If(dt.Rows(0).Field(Of String)("TAISYAKU_TAIO_CD"), "")

        Return taisyakuTaioCd
    End Function
#End Region

#Region "更新/登録/削除"
    ''' <summary>
    ''' 使用中フラグの更新
    ''' </summary>
    Private Function updateUsingFlg(toRaising As Boolean) As Boolean
        'サーバー日付取得
        Dim sysdates As Hashtable = Me.getSysDates()

        'エンティティの設定
        Dim ent As New YoyakuInfoBasicEntity
        ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
        ent.yoyakuNo.Value = Me.ParamData.YoyakuNo
        ent.usingFlg.Value = If(toRaising, FixedCd.UsingFlg.Use, FixedCd.UsingFlg.Unused)
        ent.updatePersonCd.Value = UserInfoManagement.userId
        ent.updatePgmid.Value = PgmId
        ent.updateDay.Value = DirectCast(sysdates(KeyIntSysDate), Integer)
        ent.updateTime.Value = DirectCast(sysdates(KeyIntSysTimeHhMmSs), Integer)
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
        ent.systemUpdatePgmid.Value = PgmId
        ent.systemUpdateDay.Value = DirectCast(sysdates(KeyDtSysDate), Date)

        Dim da As New S02_0602Da
        Return da.updateUsingFlg(ent)
    End Function

    ''' <summary>
    ''' 発券情報グループの登録
    ''' </summary>
    ''' <returns></returns>
    Private Function insertHakkenGroup() As Boolean

        ' 座席状態更新
        Dim busReserveCd As String = String.Empty
        Dim beforeZasekiStateCd As Integer = 0
        ' 共通処理実行
        If setZasekiState(busReserveCd, beforeZasekiStateCd, False) = False Then
            Return False
        End If

        Try
            'エンティティの設定
            Dim hakkenGroupEntity As HakkenGroupEntity = Me.setHakkenGroup()

            Dim da As New S02_0602Da
            If da.insertHakkenGroup(hakkenGroupEntity, False) = True Then
                ' 更新成功
                Return True
            Else
                ' 更新失敗(exception以外) → 座席ステータスを戻す
                resetZasekiStateCd(beforeZasekiStateCd, busReserveCd)
                Return False
            End If

        Catch ex As Exception
            ' ここのcatchでは、座席のステータスを戻す必要がある
            resetZasekiStateCd(beforeZasekiStateCd, busReserveCd)
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 予約情報（基本）の更新（ノーショウのみ）
    ''' </summary>
    ''' <returns></returns>
    Private Function updateYoyakuInfoBasicForNoShow() As Boolean

        Try
            'エンティティの設定
            Dim YoyakuInfoBasicForNoShowEntity As HakkenGroupEntity = Me.setRegistInfoForNoShow()

            Dim da As New S02_0602Da
            If da.updateYoyakuInfoBaiscForNoShow(YoyakuInfoBasicForNoShowEntity) = True Then
                ' 更新成功
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Throw
        End Try

    End Function

    ''' <summary>
    ''' 共通処理を使用し、座席ステータスを更新する
    '''  ※F10⇒発券済み（処理区分10）を実行
    ''' </summary>
    ''' <param name="busReserveCd">byref。後続処理で失敗した場合に使用すること</param>
    ''' <param name="beforeZasekiStateCd">byref。後続処理で失敗した場合に使用すること</param>
    ''' <param name="voidFlg"></param>
    ''' <returns></returns>
    Private Function setZasekiState(ByRef busReserveCd As String, ByRef beforeZasekiStateCd As Integer, ByVal voidFlg As Boolean) As Boolean

        Dim da As New S02_0602Da

        '------------------------------
        ' 処理実行前準備
        '------------------------------
        ' バス指定コードの取得
        Dim tmpBusReserveCd As String = da.getBusReserveCd(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_CD"),
                                                           CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY")),
                                                           CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Short?)("GOUSYA")))
        ' 座席イメージ（バス）の取得
        Dim zasekiDt As DataTable = da.getZasekiImage(tmpBusReserveCd,
                                                      CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY")),
                                                      CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Short?)("GOUSYA")))
        ' 座席イメージ（座席）の取得
        Dim zasekiImageDt As DataTable = da.getZasekiImageInfo(tmpBusReserveCd,
                                                               CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY")),
                                                               CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Short?)("GOUSYA")))
        ' データ取得NGは処理終了
        If zasekiDt Is Nothing OrElse zasekiDt.Rows.Count = 0 OrElse
            zasekiImageDt Is Nothing OrElse zasekiImageDt.Rows.Count = 0 Then
            Return False
        End If
        ' エンティティへ格納
        Dim zasekiImage As EntityOperation(Of TZasekiImageEntity) = YoyakuBizCommon.setEntityFromDataTable(Of TZasekiImageEntity)(zasekiDt)
        Dim zasekiImageInfo As EntityOperation(Of TZasekiImageInfoEntity) = YoyakuBizCommon.setEntityFromDataTable(Of TZasekiImageInfoEntity)(zasekiImageDt)
        ' 座席イメージ（座席）の対象データをセット
        Dim query = (From d In zasekiImageInfo.EntityData
                     Where d.YoyakuKbn.Value = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("YOYAKU_KBN") AndAlso
                         d.YoyakuNo.Value = CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("YOYAKU_NO"))).First()

        '------------------------------
        ' 引数のパラーメタに値をセットする
        busReserveCd = tmpBusReserveCd
        beforeZasekiStateCd = CInt(query.ZasekiState.Value)

        '------------------------------
        ' 共通処理（座席自動配置Z0008）実行
        '------------------------------
        Dim z0008 As New Zaseki.Z0008
        Dim z0008Param As New Zaseki.Z0008_Param
        Dim z0008Result As New Zaseki.Z0008_Result

        ' パラメータセット
        If voidFlg = False Then
            ' 発券処理時
            z0008Param.ProcessKbn = Zaseki.Z0008_Param.Z0008_Param_ProcessKbn.ProcessKbn_10
        Else
            ' VOID処理時
            z0008Param.ProcessKbn = Zaseki.Z0008_Param.Z0008_Param_ProcessKbn.ProcessKbn_30
        End If
        z0008Param.CrsCd = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_CD")                      ' コースコード
        z0008Param.SyuptDay = CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY"))        ' 出発日
        z0008Param.Gousya = CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Short?)("GOUSYA"))               ' 号車
        z0008Param.BusReserveCd = tmpBusReserveCd                                                           ' バス指定コード
        z0008Param.YoyakuKbn = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("YOYAKU_KBN")              '予約区分
        z0008Param.YoyakuNo = CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("YOYAKU_NO"))        ' 予約No
        z0008Param.GroupNo = CInt(query.GroupNo.Value)                                                     ' グループNO
        z0008Param.TobiSeatKbn = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("TOBI_SEAT_FLG")         ' とび席区分
        z0008Param.Ninzu = Me._totalYoyakuNinzu                                                             ' 人数
        z0008Param.BusInfo = zasekiImage.EntityData(0)                                                      ' 座席イメージ（バス情報）
        Dim zasekiImageInfoList As New List(Of TZasekiImageInfoEntity)
        For Each tmp In zasekiImageInfo.EntityData                                                          ' 座席イメージ（座席情報）
            zasekiImageInfoList.Add(tmp)
        Next
        z0008Param.ZasekiInfo = zasekiImageInfoList

        ' 共通処理の実行
        z0008Result = z0008.Execute(z0008Param)

        ' 処理結果判定
        If z0008Result.Status <> Zaseki.Z0008_Result.Z0008_Result_Status.OK AndAlso
            z0008Result.Status <> Zaseki.Z0008_Result.Z0008_Result_Status.Kaku Then
            ' 正常終了(00)、架空車種(10)以外はエラー
            createFactoryLog.logOutput(LogKindType.debugLog, ProcessKindType.sonota, setFormId, setTitle, "座席共通処理実行(Z0008):処理NG（" & z0008Result.Status.ToString & "）")
            Return False
        Else
            createFactoryLog.logOutput(LogKindType.debugLog, ProcessKindType.sonota, setFormId, setTitle, "座席共通処理実行(Z0008):処理OK（" & z0008Result.Status.ToString & "）")
            Return True
        End If

        ' 応答の値を使用して、予約情報基本の、座席及び予約座席区分（YOYAKU_ZASEKI_KBN）をセットしなおす
        'hakkenGroupEntity.YoyakuInfoBasicEntity.zaseki.Value = ""
        'hakkenGroupEntity.YoyakuInfoBasicEntity.yoyakuZasekiKbn.Value = ""
        ' ⇒座席自動配置的に、席が替わることが無いとの事なので新システムでは更新不要

    End Function


    ''' <summary>
    ''' 座席イメージの状態コードを元に戻す
    ''' </summary>
    ''' <param name="zasekiStateCd"></param>
    ''' <param name="busReserveCd"></param>
    ''' <returns></returns>
    Private Function resetZasekiStateCd(ByVal zasekiStateCd As Integer,
                                      ByVal busReserveCd As String) As Boolean

        If zasekiStateCd = 0 OrElse busReserveCd = String.Empty Then
            Return False
        End If

        Dim Da As New S02_0602Da

        Dim syuptDay As Integer = CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY"))
        Dim gousya As Integer = CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Short?)("GOUSYA"))
        Dim yoyakuKbn As String = Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("YOYAKU_KBN")
        Dim yoyakuNo As Integer = CInt(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("YOYAKU_NO"))

        Dim sysUpdatePgmid As String = PgmId
        Dim sysUpdatePersonCd As String = UserInfoManagement.userId
        Dim sysUpdateDate As Date = CommonDateUtil.getSystemTime()

        Return Da.updateZasekiState(zasekiStateCd, busReserveCd, syuptDay, gousya,
                                     yoyakuKbn, yoyakuNo, sysUpdatePgmid, sysUpdatePersonCd, sysUpdateDate)

    End Function

#End Region
#End Region

#Region "エンティティへの設定"
#Region "発券情報グループの設定"
    ''' <summary>
    ''' 発券情報グループの設定
    ''' </summary>
    ''' <returns></returns>
    Private Function setHakkenGroup() As HakkenGroupEntity
        'サーバー日付を取得
        Dim sysDates As Hashtable = Me.getSysDates()

        '予約情報（割引）エンティティ
        Dim yoyakuInfoWaribikiList As List(Of YoyakuInfoWaribikiEntity) = Nothing
        If Me._existsWaribiki Then
            '割引あり
            yoyakuInfoWaribikiList = Me.createYoyakuInfoWaribikiEntityForHakken(sysDates)
        End If

        '予約情報（基本）エンティティ
        Dim yoyakuInfoBasicPhysicsNameList As New List(Of String)
        Dim yoyakuInfoBasic As YoyakuInfoBasicEntity = Me.createYoyakuInfoBasicEntityForHakken(yoyakuInfoBasicPhysicsNameList,
                                                               sysDates)

        '予約情報２エンティティ
        Dim existsYoyakuInfo2 As Boolean = CommonHakken.extistsYoyakuInfo2(Me.ParamData.YoyakuKbn, Me.ParamData.YoyakuNo)
        Dim yoyakuInfo2 As YoyakuInfo2Entity = Me.createYoyakuInfo2(sysDates, existsYoyakuInfo2)

        '予約情報（コース料金）エンティティ
        Dim yoyakuInfoCrsCharge As YoyakuInfoCrsChargeEntity = Me.createYoyakuInfoCrsChargeEntityForHakken(sysDates)

        '予約情報（コース料金_料金区分）エンティティ
        Dim yoyakuInfoCrsChargeChargeKbn As YoyakuInfoCrsChargeChargeKbnEntity = Me.createYoyakuInfoCrsChargeChargeKbnEntityForHakken(sysDates)
        Dim yoyakuInfoCrsChargeChargeKbnList As New List(Of YoyakuInfoCrsChargeChargeKbnEntity)
        yoyakuInfoCrsChargeChargeKbnList.Add(yoyakuInfoCrsChargeChargeKbn)


        '発券情報エンティティ
        Dim hakkenInfo As HakkenInfoEntity = Me.createHakkenInfoEntityForHakken(sysDates)

        '発券情報（料金）エンティティ
        Dim hakkenInfoCharge As List(Of HakkenInfoChargeEntity) = Me.createHakkenInfoChargeEntityListForHakken(sysDates)

        '精算情報セットエンティティ　※割引種別ごとに各精算項目へ按分をかける
        Dim seisanInfoSetEntity As SeisanInfoListSetEntity = Me.distributeSeisanInfo(sysDates)

        '入返金情報エンティティ
        Dim nyuukinInfoEntity As NyuukinInfoEntity = Me.createNyuukinInfoEntity(False, sysDates)

        '入返金情報エンティティ（予約センター返金あり
        Dim nyuukinInfoEntityHenkinAriPhysicsNameList As List(Of String) = Nothing
        Dim nyuukinInfoEntityHenkinAri As NyuukinInfoEntity = Nothing
        If Me.YoyakuCenterHenkin > 0 Then
            nyuukinInfoEntityHenkinAriPhysicsNameList = New List(Of String)
            nyuukinInfoEntityHenkinAri = Me.createNyuukinInfoEntityHenkinAri(nyuukinInfoEntityHenkinAriPhysicsNameList,
            sysDates)
        End If

        '発券情報グループへエンティティを格納
        Dim hakkenGroupEntity As New HakkenGroupEntity
        hakkenGroupEntity.YoyakuInfoWaribikiEntityList = yoyakuInfoWaribikiList
        hakkenGroupEntity.YoyakuInfoBasicEntity = yoyakuInfoBasic
        hakkenGroupEntity.YoyakuInfoBasicPhysicsNameList = yoyakuInfoBasicPhysicsNameList
        hakkenGroupEntity.YoyakuInfo2Entity = yoyakuInfo2
        hakkenGroupEntity.YoyakuInfoCrsChargeEntity = yoyakuInfoCrsCharge
        hakkenGroupEntity.YoyakuInfoCrsChargeChargeKbnEntityList = yoyakuInfoCrsChargeChargeKbnList
        hakkenGroupEntity.HakkenInfoEntity = hakkenInfo
        hakkenGroupEntity.HakkenInfoChargeEntityList = hakkenInfoCharge
        hakkenGroupEntity.SeisanInfoListSetEntity = seisanInfoSetEntity
        hakkenGroupEntity.NyuukinInfoEntity = nyuukinInfoEntity
        hakkenGroupEntity.NyuukinInfoEntityHenkinAri = nyuukinInfoEntityHenkinAri

        Return hakkenGroupEntity
    End Function

    ''' <summary>
    ''' 登録情報の設定(ノーショウのみ登録用)
    ''' </summary>
    ''' <returns></returns>
    Private Function setRegistInfoForNoShow() As HakkenGroupEntity
        'サーバー日付を取得
        Dim sysDates As Hashtable = Me.getSysDates()

        '予約情報（基本）エンティティ
        Dim yoyakuInfoBasicPhysicsNameList As New List(Of String)
        Dim yoyakuInfoBasic As YoyakuInfoBasicEntity = Me.createYoyakuInfoBasicEntityForNoShow(yoyakuInfoBasicPhysicsNameList,
                                                               sysDates)

        '発券情報グループへエンティティを格納
        Dim hakkenGroupEntity As New HakkenGroupEntity
        hakkenGroupEntity.YoyakuInfoBasicEntity = yoyakuInfoBasic
        hakkenGroupEntity.YoyakuInfoBasicPhysicsNameList = yoyakuInfoBasicPhysicsNameList

        Return hakkenGroupEntity
    End Function

#Region "予約情報エンティティ"
    ''' <summary>
    ''' 予約情報（割引）エンティティの設定（発券）
    ''' </summary>
    ''' <returns></returns>
    Private Function createYoyakuInfoWaribikiEntityForHakken(sysDates As Hashtable) As List(Of YoyakuInfoWaribikiEntity)

        Dim list As New List(Of YoyakuInfoWaribikiEntity)

        For Each row As Row In Me.grdWaribikiCharge.Rows
            If row.Index = 0 Then
                Continue For
            End If
            '割引コード
            Dim waribikiCd As String = TryCast(row.Item("WARIBIKI_CD"), String)
            Dim strKbnNo As String = ""
            '区分No（区分No→料金区分コード→料金区分名と紐づく）
            If Not CommonHakken.isNull(row.Item("KBN_NO")) Then
                strKbnNo = row.Item("KBN_NO").ToString()
            End If
            Dim kbnNo As Integer = 0
            Integer.TryParse(strKbnNo, kbnNo)
            '人員コード
            Dim chargeKbnJininCd As String = TryCast(row.Item("CHARGE_KBN_JININ_CD"), String)

            'nullなら次の行へ
            If String.IsNullOrWhiteSpace(waribikiCd) _
            OrElse kbnNo = 0 _
            OrElse String.IsNullOrWhiteSpace(chargeKbnJininCd) Then

                Continue For
            End If

            'グリッドからの値の取り出し
            '割引
            Dim strWaribiki As String = ""
            Dim waribiki As Integer = 0
            If Not CommonHakken.isNull(row.Item("WARIBIKI")) Then
                strWaribiki = row.Item("WARIBIKI").ToString()
            End If
            Integer.TryParse(strWaribiki, waribiki)
            '割引区分（単位）
            Dim tani As String = If(TryCast(row.Item("WARIBIKI_KBN"), String), "")
            '割引人数
            Dim strWaribikiNinzu As String = ""
            Dim waribikiNinzu As Integer = 0
            If Not CommonHakken.isNull(row.Item("WARIBIKI_NINZU")) Then
                strWaribikiNinzu = row.Item("WARIBIKI_NINZU").ToString()
            End If
            Integer.TryParse(strWaribikiNinzu, waribikiNinzu)
            '割引適用人数
            Dim strWaribikiApplicationNinzu As String = ""
            Dim waribikiApplicationNinzu As Integer = 0
            If Not CommonHakken.isNull(row.Item("WARIBIKI_APPLICATION_NINZU")) Then
                strWaribikiApplicationNinzu = row.Item("WARIBIKI_APPLICATION_NINZU").ToString()
            End If
            Integer.TryParse(strWaribikiApplicationNinzu, waribikiApplicationNinzu)
            '運賃割引フラグ
            Dim carriageWaribikiFlg As String = If(TryCast(row.Item("CARRIAGE_WARIBIKI_FLG"), String), "")

            'エンティティへの設定
            Dim ent As New YoyakuInfoWaribikiEntity
            ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
            ent.yoyakuNo.Value = Me.ParamData.YoyakuNo
            ent.kbnNo.Value = kbnNo
            ent.chargeKbnJininCd.Value = chargeKbnJininCd
            ent.waribikiCd.Value = waribikiCd
            Dim intYear As Integer = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY"), 0)
            Dim strYear As String = intYear.ToString().PadLeft(8, "0"c).Substring(0, 4)
            ent.year.Value = Integer.Parse(strYear)
            Dim waribikiName As String = TryCast(Me.grdWaribikiCharge.Cols("WARIBIKI_NAME").DataMap(waribikiCd), String)
            ent.waribikiRiyuu.Value = If(waribikiName, "")
            Dim waribikiKbn As String = ""
            If tani.Equals(FixedCdYoyaku.Tani.Per) Then
                waribikiKbn = CommonHakken.convertEnumToString(FixedCd.WaribikiKubun.WaribikiRitsu)
            ElseIf tani.Equals(FixedCdYoyaku.Tani.Yen) Then
                waribikiKbn = CommonHakken.convertEnumToString(FixedCd.WaribikiKubun.WaribikiGaku)
            End If
            ent.waribikiKbn.Value = waribikiKbn
            ent.carriageWaribikiFlg.Value = carriageWaribikiFlg
            ent.yoyakuWaribikiFlg.Value = If(TryCast(row.Item("YOYAKU_WARIBIKI_FLG"), String), "")

            For roomType = CommonHakken.One1R To CommonHakken.FiveIjyou1R
                '割引人数
                Dim strWaribikiApplicationNinzuN As String = ""
                Dim waribikiApplicationNinzuN As Integer = 0
                If Not CommonHakken.isNull(row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}")) Then
                    strWaribikiApplicationNinzuN = row.Item($"WARIBIKI_APPLICATION_NINZU_{roomType}").ToString()
                End If
                Integer.TryParse(strWaribikiApplicationNinzuN, waribikiApplicationNinzuN)

                '割引単価
                Dim strWaribikiTankaN As String = ""
                Dim waribikiTankaN As Integer = 0
                If Not CommonHakken.isNull(row.Item($"WARIBIKI_TANKA_{roomType}")) Then
                    strWaribikiTankaN = row.Item($"WARIBIKI_TANKA_{roomType}").ToString()
                End If
                Integer.TryParse(strWaribikiTankaN, waribikiTankaN)

                Select Case roomType
                    Case CommonHakken.One1R
                        ent.waribikiApplicationNinzu1.Value = waribikiApplicationNinzuN
                        ent.waribikiTanka1.Value = waribikiTankaN
                    Case CommonHakken.Two1R
                        ent.waribikiApplicationNinzu2.Value = waribikiApplicationNinzuN
                        ent.waribikiTanka2.Value = waribikiTankaN
                    Case CommonHakken.Three1R
                        ent.waribikiApplicationNinzu3.Value = waribikiApplicationNinzuN
                        ent.waribikiTanka3.Value = waribikiTankaN
                    Case CommonHakken.Four1R
                        ent.waribikiApplicationNinzu4.Value = waribikiApplicationNinzuN
                        ent.waribikiTanka4.Value = waribikiTankaN
                    Case CommonHakken.FiveIjyou1R
                        ent.waribikiApplicationNinzu5.Value = waribikiApplicationNinzuN
                        ent.waribikiTanka5.Value = waribikiTankaN
                End Select
            Next

            ent.waribikiApplicationNinzu.Value = waribikiApplicationNinzu '割引適用人数（割引可能な上限の人数
            ent.waribikiBiko.Value = If(TryCast(row.Item("WARIBIKI_BIKO"), String), "")

            If tani.Equals(FixedCdYoyaku.Tani.Per) Then
                '割引「率」の場合
                ent.waribikiKingaku.Value = 0
                ent.waribikiPer.Value = Convert.ToInt32(waribiki)

            ElseIf tani.Equals(FixedCdYoyaku.Tani.Yen) Then
                '割引「額」の場合
                ent.waribikiKingaku.Value = Convert.ToInt32(waribiki)
                ent.waribikiPer.Value = 0
            End If

            ent.deleteDay.Value = 0
            ent.entryDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
            ent.entryTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
            ent.entryPersonCd.Value = UserInfoManagement.userId
            ent.entryPgmid.Value = PgmId
            ent.updateDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
            ent.updateTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
            ent.updatePersonCd.Value = UserInfoManagement.userId
            ent.updatePgmid.Value = PgmId
            ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
            ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
            ent.systemUpdatePgmid.Value = PgmId
            ent.systemEntryDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
            ent.systemEntryPersonCd.Value = UserInfoManagement.userId
            ent.systemEntryPgmid.Value = PgmId

            list.Add(ent)
        Next

        Return list
    End Function

    ''' <summary>
    ''' 予約情報（基本）エンティティの設定　（発券）
    ''' </summary>
    Private Function createYoyakuInfoBasicEntityForHakken(ByRef physicsNameList As List(Of String),
                                                       sysDates As Hashtable) As YoyakuInfoBasicEntity

        Dim ent As New YoyakuInfoBasicEntity
        ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn '予約区分
        physicsNameList.Add(ent.yoyakuKbn.PhysicsName)
        ent.yoyakuNo.Value = Me.ParamData.YoyakuNo
        physicsNameList.Add(ent.yoyakuNo.PhysicsName)
        ent.hakkenDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
        physicsNameList.Add(ent.hakkenDay.PhysicsName)
        ent.hakkenTantosyaCd.Value = UserInfoManagement.userId
        physicsNameList.Add(ent.hakkenTantosyaCd.PhysicsName)
        ent.hakkenEigyosyoCd.Value = UserInfoManagement.eigyosyoCd
        physicsNameList.Add(ent.hakkenEigyosyoCd.PhysicsName)
        '定期　または　企画かつ内金発券でない場合

        ent.hakkenNaiyo.Value = CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zenkin) '発券内容(全金発券
        physicsNameList.Add(ent.hakkenNaiyo.PhysicsName)
        ent.nyuukinSituationKbn.Value = CommonHakken.convertEnumToString(FixedCdYoyaku.NyuukinSituationKbn.NyuukinZumi) '入金状況区分（入金済み
        physicsNameList.Add(ent.nyuukinSituationKbn.PhysicsName)
        Dim dHakkenKingaku As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("HAKKEN_KINGAKU"), 0)
        Dim hakkenKingaku As Integer = Convert.ToInt32(dHakkenKingaku)
        '入金額総計 = 既入金額（入金額総計） + 請求 - 発券金額 - 予約センター振込 - オンライン決済
        ent.nyukingakuSokei.Value = Me.PreNyukinGaku _
                                  + Me.SeikyuTotal _
                                  - hakkenKingaku _
                                  - Me.YoyakuCenterNyuukin _
                                  - Me.OnlineCredit
        physicsNameList.Add(ent.nyukingakuSokei.PhysicsName)
        Dim dSeikiCharge As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("SEIKI_CHARGE_ALL_GAKU"), 0)
        Dim dMaebaraiKei As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("ADD_CHARGE_MAEBARAI_KEI"), 0)
        Dim intSeikiCharge As Integer = Convert.ToInt32(dSeikiCharge)
        Dim intMaebaraiKei As Integer = Convert.ToInt32(dMaebaraiKei)
        ent.hakkenKingaku.Value = intSeikiCharge + intMaebaraiKei '発券金額 = 正規料金総額 + 追加料金前払計
        physicsNameList.Add(ent.hakkenKingaku.PhysicsName)
        ent.waribikiAllGaku.Value = Me.WaribikiTotal '割引総額
        physicsNameList.Add(ent.waribikiAllGaku.PhysicsName)
        Dim intGousya As Integer = 0
        Integer.TryParse(Me.txtGousya.Text.Trim, intGousya)
        ent.oldGousya.Value = intGousya '旧号車
        physicsNameList.Add(ent.oldGousya.PhysicsName)
        ent.oldZaseki.Value = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("ZASEKI"), "") '旧座席
        physicsNameList.Add(ent.oldZaseki.PhysicsName)

        Dim seisanHoho As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("SEISAN_HOHO"), "")
        If Me.IsKikaku _
        AndAlso Not seisanHoho.Equals(CommonHakken.convertEnumToString(FixedCdYoyaku.PaymentHoho.agt)) _
        AndAlso Date.Now < Me.ucoSyuptDay.Value Then
            '企画　かつ　精算方法が代理店でない　かつ　出発日が過去　の場合
            ent.seisanHoho.Value = CommonHakken.convertEnumToString(FixedCdYoyaku.PaymentHoho.eigyosyo)
        End If

        ent.zasekiChangeUmu.Value = ""
        physicsNameList.Add(ent.zasekiChangeUmu.PhysicsName)
        ent.cancelRyouKei.Value = Me.Cancel
        physicsNameList.Add(ent.cancelRyouKei.PhysicsName)
        ent.toriatukaiFeeUriage.Value = 0
        physicsNameList.Add(ent.toriatukaiFeeUriage.PhysicsName)
        ent.toriatukaiFeeCancel.Value = 0
        physicsNameList.Add(ent.toriatukaiFeeCancel.PhysicsName)
        Dim tasyaKennoKingaku As Integer = 0 '他社券
        Dim sonotaNyuukinHenkin As Integer = 0 'その他
        '集計した精算項目から、船車券とその他の金額を取り出し
        If Me._undistributedKinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sensya_ken)) OrElse
             Me._undistributedKinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.bc)) Then

            If Me._undistributedKinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sensya_ken)) Then
                tasyaKennoKingaku = Me._undistributedKinBySeisanKoumoku(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sensya_ken))
            End If

            If Me._undistributedKinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.bc)) Then
                tasyaKennoKingaku = tasyaKennoKingaku + Me._undistributedKinBySeisanKoumoku(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.bc))
            End If
        ElseIf Me._undistributedKinBySeisanKoumoku.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)) Then
            sonotaNyuukinHenkin = Me._undistributedKinBySeisanKoumoku(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota))
        End If
        ent.tasyaKennoKingaku.Value = tasyaKennoKingaku
        physicsNameList.Add(ent.tasyaKennoKingaku.PhysicsName)
        ent.sonotaNyuukinHenkin.Value = sonotaNyuukinHenkin
        physicsNameList.Add(ent.sonotaNyuukinHenkin.PhysicsName)
        'TODO:座席
        'ent.zaseki.Value =
        'physicsNameList.Add(ent.zaseki.PhysicsName)
        'ent.yoyakuZasekiKbn.vaue    =
        'physicsNameList.Add(ent.yoyakuZasekiKbn.PhysicsName)
        'ent.changeHistoryLastDay.Value=
        'physicsNameList.Add(ent.changeHistoryLastDay.PhysicsName)
        'TODO:変更履歴から
        'ent.changeHistoryLastSeq.Value =
        'physicsNameList.Add(ent.changeHistoryLastSeq.PhysicsName)
        ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        physicsNameList.Add(ent.systemUpdateDay.PhysicsName)
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
        physicsNameList.Add(ent.systemUpdatePersonCd.PhysicsName)
        ent.systemUpdatePgmid.Value = PgmId
        physicsNameList.Add(ent.systemUpdatePgmid.PhysicsName)
        If Me.OnlineCredit = 0 Then
            'オンラインクレジット決済でない場合
            '最終入金日更新
            ent.lastNyuukinDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
            physicsNameList.Add(ent.lastNyuukinDay.PhysicsName)

        ElseIf Me.OnlineCredit <> 0 AndAlso Me.Oturi <> 0 Then
            'オンラインクレジット決済　かつ　お釣ありの場合
            '最終返金日更新
            ent.lastHenkinDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
            physicsNameList.Add(ent.lastHenkinDay.PhysicsName)
        End If

        'ノーショー
        If Me.chkNoShow.Checked = True Then
            ent.noShowFlg.Value = "Y"
            physicsNameList.Add(ent.noShowFlg.PhysicsName)
        End If

        Return ent
    End Function

    ''' <summary>
    ''' 予約情報２エンティティの設定
    ''' </summary>
    ''' <param name="sysDates"></param>
    ''' <returns></returns>
    Private Function createYoyakuInfo2(sysDates As Hashtable, existsYoyakuInfo2 As Boolean) As YoyakuInfo2Entity
        Dim ent As New YoyakuInfo2Entity

        ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
        ent.yoyakuNo.Value = Me.ParamData.YoyakuNo
        Dim syuptDay As Integer = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY"), 0)
        Dim strYear As String = syuptDay.ToString().PadLeft(8, "0"c).Substring(0, 4)
        Dim year As Integer = Integer.Parse(strYear)
        ent.year.Value = year

        ent.outDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
        ent.outTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
        ent.outPersonCd.Value = UserInfoManagement.userId
        ent.outPgmid.Value = PgmId

        ent.deleteDay.Value = 0

        ent.updateDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
        ent.updateTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
        ent.updatePersonCd.Value = UserInfoManagement.userId
        ent.updatePgmid.Value = PgmId
        ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
        ent.systemUpdatePgmid.Value = PgmId

        If existsYoyakuInfo2 = False Then
            ent.entryDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
            ent.entryTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
            ent.entryPersonCd.Value = UserInfoManagement.userId
            ent.entryPgmid.Value = PgmId
            ent.systemEntryDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
            ent.systemEntryPersonCd.Value = UserInfoManagement.userId
            ent.systemEntryPgmid.Value = PgmId
        End If

        Return ent
    End Function

    ''' <summary>
    ''' 予約情報(コース料金）エンティティの設定
    ''' </summary>
    ''' <returns></returns>
    Private Function createYoyakuInfoCrsChargeEntityForHakken(sysDates As Hashtable) As YoyakuInfoCrsChargeEntity
        Dim ent As New YoyakuInfoCrsChargeEntity

        ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
        ent.yoyakuNo.Value = Me.ParamData.YoyakuNo
        ent.cancelPer.Value = 0
        If Me.Cancel = Me.CancelYoyakuInfoBasic Then
            ent.cancelRyou.Value = 0
        Else
            'キャンセル料金に変更あればコース料金へ残す
            ent.cancelRyou.Value = Me.Cancel
        End If
        ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
        ent.systemUpdatePgmid.Value = PgmId

        Return ent
    End Function

    ''' <summary>
    ''' 予約情報(コース料金_料金区分）エンティティの設定
    ''' </summary>
    ''' <returns></returns>
    Private Function createYoyakuInfoCrsChargeChargeKbnEntityForHakken(sysDates As Hashtable) As YoyakuInfoCrsChargeChargeKbnEntity
        Dim ent As New YoyakuInfoCrsChargeChargeKbnEntity

        ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
        ent.yoyakuNo.Value = Me.ParamData.YoyakuNo
        ent.cancelNinzu1.Value = 0
        ent.cancelNinzu2.Value = 0
        ent.cancelNinzu3.Value = 0
        ent.cancelNinzu4.Value = 0
        ent.cancelNinzu5.Value = 0
        ent.cancelNinzu.Value = 0
        ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
        ent.systemUpdatePgmid.Value = PgmId

        Return ent
    End Function

    ''' <summary>
    ''' 予約情報（基本）エンティティの設定　（ノーショウのみ）
    ''' </summary>
    Private Function createYoyakuInfoBasicEntityForNoShow(ByRef physicsNameList As List(Of String),
                                                       sysDates As Hashtable) As YoyakuInfoBasicEntity

        Dim ent As New YoyakuInfoBasicEntity
        ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn '予約区分
        physicsNameList.Add(ent.yoyakuKbn.PhysicsName)
        ent.yoyakuNo.Value = Me.ParamData.YoyakuNo
        physicsNameList.Add(ent.yoyakuNo.PhysicsName)

        ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        physicsNameList.Add(ent.systemUpdateDay.PhysicsName)
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
        physicsNameList.Add(ent.systemUpdatePersonCd.PhysicsName)
        ent.systemUpdatePgmid.Value = PgmId
        physicsNameList.Add(ent.systemUpdatePgmid.PhysicsName)

        'ノーショー
        If Me.chkNoShow.Checked = True Then
            ent.noShowFlg.Value = "Y"
            physicsNameList.Add(ent.noShowFlg.PhysicsName)
        End If

        Return ent
    End Function

#End Region

#Region "発券情報エンティティ"
    ''' <summary>
    ''' 発券情報エンティティの設定　(発券）
    ''' </summary>
    ''' <returns></returns>
    Private Function createHakkenInfoEntityForHakken(sysDates As Hashtable) As HakkenInfoEntity

        Dim ent As New HakkenInfoEntity

        ent.eigyosyoKbn.Value = Me._newKenNo.Substring(0, 1)
        ent.tickettypeCd.Value = Me._newKenNo.Substring(1, 1)
        ent.mokuteki.Value = Me._newKenNo.Substring(2, 1)
        ent.issueYearly.Value = Integer.Parse(Me._newKenNo.Substring(3, 2))
        ent.seq1.Value = Me._hakkenInfoSeq1
        ent.seq2.Value = Me._hakkenInfoSeq2

        ent.hakkenDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
        ent.hakkenTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMm), Integer?)
        ent.hakkenTantosyaCd.Value = UserInfoManagement.userId
        ent.hakkenEigyosyoCd.Value = UserInfoManagement.eigyosyoCd

        ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
        ent.yoyakuNo.Value = Me.ParamData.YoyakuNo

        '全金
        ent.hakkenNaiyo.Value = CommonHakken.convertEnumToString(FixedCd.HakkenNaiyo.Zenkin)

        Dim dMaebaraiKei As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("ADD_CHARGE_MAEBARAI_KEI"), 0)
        Dim intMaebaraiKei As Integer = Convert.ToInt32(dMaebaraiKei)
        ent.addChargeMaebaraiKei.Value = intMaebaraiKei '追加料金前払計
        ent.cancelRyou.Value = Me.Cancel 'キャンセル料
        ent.toriatukaiFeeUriage.Value = Convert.ToInt32(Me._toriatukaiFeeUriage) '取扱手数料/ 売上
        ent.toriatukaiFeeCancel.Value = Convert.ToInt32(Me._toriatukaiFeeCancel) '取扱手数料/ ｷｬﾝｾﾙ

        '正規料金 + 追加料金前払計
        Dim dSeikiCharge As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("SEIKI_CHARGE_ALL_GAKU"), 0)
        Dim intSeikiCharge As Integer = Convert.ToInt32(dSeikiCharge)
        Dim charge As Integer = intSeikiCharge + intMaebaraiKei

        ent.hakkenKingaku.Value = charge             '発券金額
        ent.uriageKingaku.Value = charge             '売上金額
        ent.waribikiKingaku.Value = Me.WaribikiTotal '割引金額

        Dim existsGenkin As Boolean = Me._undistributedKinBySeisanKoumoku.Keys _
                                     .Any(Function(key) key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)))

        Dim existsSensyaken As Boolean = Me._undistributedKinBySeisanKoumoku.Keys _
                                     .Any(Function(key) key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sensya_ken)))

        Dim existsCredit As Boolean = Me._undistributedKinBySeisanKoumoku.Keys _
                                     .Any(Function(key) key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.credit_card)) _
                                                        OrElse key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.online_credit)))

        Dim existsSonota As Boolean = Me._undistributedKinBySeisanKoumoku.Keys _
                                     .Any(Function(key) key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)))


        If existsGenkin Then
            '現金精算がある場合
            Me._SeisanHohoHakkenInfo = CommonHakken.SeisanHohoHakkenInfoGenkin

        Else
            '現金精算がない場合
            If existsSensyaken Then
                Me._SeisanHohoHakkenInfo = CommonHakken.SeisanHohoHakkenInfoSensyaKen
            End If
            If existsCredit Then
                Me._SeisanHohoHakkenInfo = CommonHakken.SeisanHohoHakkenInfoCredit
            End If
            If existsSonota Then
                Me._SeisanHohoHakkenInfo = CommonHakken.SeisanHohoHakkenInfoSonota
            End If
        End If

        ent.gousya.Value = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Short?)("GOUSYA"), 0)
        ent.zaseki24Baito.Value = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("ZASEKI"), "")
        ent.oldGousya.Value = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Short?)("GOUSYA"), 0)
        ent.oldZaseki.Value = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("ZASEKI"), "")
        ent.uriageKbn.Value = ""

        ent.deleteDay.Value = 0
        ent.entryDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
        ent.entryTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
        ent.entryPersonCd.Value = UserInfoManagement.userId
        ent.entryPgmid.Value = PgmId
        ent.updateDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
        ent.updateTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
        ent.updatePersonCd.Value = UserInfoManagement.userId
        ent.updatePgmid.Value = PgmId
        ent.systemEntryDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        ent.systemEntryPersonCd.Value = UserInfoManagement.userId
        ent.systemEntryPgmid.Value = PgmId
        ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
        ent.systemUpdatePgmid.Value = PgmId

        Return ent
    End Function

    ''' <summary>
    ''' 発券情報(料金）エンティティの設定　(発券）
    ''' </summary>
    ''' <returns></returns>
    Private Function createHakkenInfoChargeEntityListForHakken(sysDates As Hashtable) As List(Of HakkenInfoChargeEntity)

        '人員SEQの採番用
        Dim jininSeq As New Dictionary(Of String, Integer)

        Dim list As New List(Of HakkenInfoChargeEntity)
        '割引コード別に集計した行から値を取り出し
        For Each waribikiCdRow As DataRow In Me._infoByWaribikiCdJininCd.AsEnumerable()

            '部屋タイプ1～5のループ
            For roomType = CommonHakken.One1R To CommonHakken.FiveIjyou1R
                Dim ninzu As Integer = If(waribikiCdRow.Field(Of Integer?)($"WARIBIKI_NINZU_{roomType}"), 0)

                '0ならエンティティ作らない
                If ninzu = 0 Then
                    Continue For
                End If

                '割引コード
                Dim waribikiCd As String = If(waribikiCdRow.Field(Of String)("WARIBIKI_CD"), "")
                '割引種別
                Dim waribikiType As String = If(waribikiCdRow.Field(Of String)("WARIBIKI_TYPE_KBN"), "")
                If waribikiType.Equals(CommonHakken.WaribikiTypeSeikiCharge) Then
                    waribikiType = "" '(割引種別：正規料金はダミー値なので初期化)
                End If
                '割引金額
                Dim waribiki As Integer = If(waribikiCdRow.Field(Of Integer?)($"WARIBIKI_TANKA_{roomType}"), 0)

                '料金カラムセット
                Dim chargeColumns As String = If(waribikiCdRow.Field(Of String)(CommonHakken.ChargeColumnsSet), "")

                '人員SEQ を採番
                jininSeq = CommonHakken.numberingJininSeq(jininSeq, chargeColumns)

                '区分No
                Dim strKbnNo As String = ""
                Dim kbnNo As Integer = 0
                '料金区分
                Dim chargeKbn As String = ""
                '料金区分（人員）コード
                Dim jininCd As String = ""

                '料金区分のカラムを分解
                CommonHakken.separateChargeColumnsSet(chargeColumns,
                                                      kbnNo,
                                                      chargeKbn,
                                                      jininCd,
                                                      strKbnNo)

                '■正規料金から登録値を取り出し
                '料金
                Dim seikiCharge As Integer = CommonHakken.getSeikiCharge(jininCd, roomType, Me._yoyakuInfoCrsChargeChargeKbn)
                '運賃
                Dim carriage As Integer = CommonHakken.getCarriage(jininCd, Me._yoyakuInfoCrsChargeChargeKbn)

                'エンティティへ値を設定
                Dim ent As New HakkenInfoChargeEntity

                ent.eigyosyoKbn.Value = Me._newKenNo.Substring(0, 1)
                ent.tickettypeCd.Value = Me._newKenNo.Substring(1, 1)
                ent.mokuteki.Value = Me._newKenNo.Substring(2, 1)
                ent.issueYearly.Value = Integer.Parse(Me._newKenNo.Substring(3, 2))
                ent.seq1.Value = Me._hakkenInfoSeq1
                ent.seq2.Value = Me._hakkenInfoSeq2

                ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
                ent.yoyakuNo.Value = Me.ParamData.YoyakuNo

                ent.kbnNo.Value = kbnNo
                ent.chargeKbnJininCd.Value = jininCd
                ent.jininSeq.Value = jininSeq(chargeColumns)
                ent.chargeKbn.Value = chargeKbn

                ent.carriage1.Value = carriage
                ent.charge.Value = seikiCharge
                ent.ninzu.Value = ninzu

                ent.waribikiCd.Value = waribikiCd
                ent.waribikiTypeKbn.Value = waribikiType
                ent.waribikiKingaku.Value = waribiki

                ent.deleteDay.Value = 0
                ent.systemEntryDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
                ent.systemEntryPersonCd.Value = UserInfoManagement.userId
                ent.systemEntryPgmid.Value = PgmId
                ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
                ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
                ent.systemUpdatePgmid.Value = PgmId

                list.Add(ent)
            Next 'ここまで部屋タイプ1～5のループ
        Next 'ここまで割引コード、人員コード単位のループ

        Return list
    End Function
#End Region

#Region "精算情報セットエンティティ"
    ''' <summary>
    ''' 精算情報の按分
    ''' </summary>
    Private Function distributeSeisanInfo(sysDates As Hashtable) As SeisanInfoListSetEntity
        'SEQの取得（存在しない場合、１とする
        Dim seq As Integer = CommonHakken.createSeq(Me.ParamData.YoyakuKbn, Me.ParamData.YoyakuNo)

        'パラメータ用の精算情報エンティティ
        Dim prmSeisanInfo As New SeisanInfoEntity
        prmSeisanInfo.seq.Value = seq
        Dim crsKind As String = _yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_KIND")
        prmSeisanInfo.seisanKbn.Value = CommonHakken.getSeisanKbn(crsKind, Me.IsTeiki)

        '精算情報セットエンティティ
        Dim seisanInfoSetEntity As New SeisanInfoListSetEntity

        Dim isFinishedDistibute As Boolean = False
        '各割引種別から各精算項目への按分
        '割引がある場合、按分を実行
        If Me._existsWaribiki Then
            '割引種別が一般でない按分
            isFinishedDistibute = Me.distributeSeisanInfoNOTGeneralWaribikiType(prmSeisanInfo,
                                                      sysDates,
                                                      seisanInfoSetEntity)
        End If

        '割引種別が一般、正規料金の按分
        If isFinishedDistibute = False Then
            Me.distributeSeisanInfoGeneralWaribikiType(prmSeisanInfo,
                                                       sysDates,
                                                       seisanInfoSetEntity)
        End If

        Return seisanInfoSetEntity
    End Function

    ''' <summary>
    ''' 割引種別が一般でない精算情報の按分
    ''' </summary>
    Private Function distributeSeisanInfoNOTGeneralWaribikiType(prmSeisanInfo As SeisanInfoEntity,
                                                           sysDates As Hashtable,
                                                           ByRef seisanInfoSetEntity As SeisanInfoListSetEntity
                                                           ) As Boolean
        '按分終了のフラグ
        Dim isFinishedDistribute As Boolean = False

        '按分後の精算内訳を格納するDictionary（キー：精算項目、値：精算内訳の金額）
        Dim seisanUtiwakeNotIppan As New Dictionary(Of String, Integer)

        '割引種別単位の請求額ごとに按分を行う
        Dim seikyuByWaribikiType = New Dictionary(Of String, Integer)(Me._seikyuByWaribikiType)
        For Each item In seikyuByWaribikiType.AsEnumerable()
            '精算内訳を初期化
            seisanUtiwakeNotIppan.Clear()

            '割引種別
            Dim waribikiType As String = item.Key
            prmSeisanInfo.waribikiType.Value = waribikiType
            '未按分金（割引種別単位の請求）
            Dim unDistributedKin As Integer = item.Value

            '割引種別が一般の場合、次の割引種別へ
            If waribikiType.Equals(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)) _
            OrElse waribikiType.Equals(CommonHakken.WaribikiTypeSeikiCharge) Then
                Continue For
            End If

            '■予約者が全員、同割引種別かつ一般割引でない場合、
            '　按分を行わず請求額そのままエンティティを作成
            Dim isAllThisWaribikiType As Boolean = (Me._ninzuByWaribikiType(waribikiType) = Me._totalYoyakuNinzu)
            If isAllThisWaribikiType Then
                Me.distributeSeisanInfoAllNOTGeneralWaribikiType(prmSeisanInfo,
                                                      sysDates,
                                                      seisanInfoSetEntity)

                '按分不要 
                isFinishedDistribute = True
                Return isFinishedDistribute
            End If

            '■その他の場合
            '①現金へ按分
            If unDistributedKin > 0 _
            AndAlso Me._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)) _
            AndAlso Me._undistributedKinBySeisanKoumoku(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)) > 0 Then

                '按分処理
                Me.distribute(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin),
                              unDistributedKin,
                              seisanUtiwakeNotIppan)
            End If

            '②現金、その他、取扱手数料以外の按分
            Dim undistributedKinBySeisanKoumoku As New Dictionary(Of String, Integer)(Me._undistributedKinBySeisanKoumoku)
            For Each seisanKoumoku In undistributedKinBySeisanKoumoku
                '現金、その他、取扱手数料の場合、次の精算項目へ
                If seisanKoumoku.Key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.genkin)) _
                OrElse seisanKoumoku.Key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)) _
                OrElse seisanKoumoku.Key.Equals(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)) Then
                    Continue For
                End If

                If unDistributedKin > 0 _
                AndAlso Me._undistributedKinBySeisanKoumoku(seisanKoumoku.Key) > 0 Then

                    '按分処理
                    Me.distribute(seisanKoumoku.Key,
                                  unDistributedKin,
                                  seisanUtiwakeNotIppan)
                End If
            Next 'ここまで精算項目ごとのループ

            '③その他（精算項目）へ按分
            If unDistributedKin > 0 _
            AndAlso Me._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)) _
            AndAlso Me._undistributedKinBySeisanKoumoku(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota)) > 0 Then

                '按分処理
                Me.distribute(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.sonota),
                          unDistributedKin,
                          seisanUtiwakeNotIppan)
            End If

            '定期　または　企画かつ内金発券でない場合
            '④取扱手数料の按分
            If unDistributedKin > 0 AndAlso Me.ToriatukaiFee > 0 Then
                If Not Me._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)) Then
                    Me._undistributedKinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo), Me.ToriatukaiFee)
                End If

                Me.distribute(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo),
                              unDistributedKin,
                              seisanUtiwakeNotIppan)
            End If

            '割引額を精算内訳に追加
            seisanUtiwakeNotIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.waribiki_kingaku),
                              Me._waribikiByWaribikiType(waribikiType))

            '精算情報セットエンティティの作成
            Me.setSeisanInfoSetEntity(seisanInfoSetEntity,
                                      prmSeisanInfo,
                                      seisanUtiwakeNotIppan,
                                      sysDates)

            '精算情報のSEQをインクリメント
            prmSeisanInfo.seq.Value += 1
        Next 'ここまで割引種別ごと（精算項目1レコード単位のループ

        '按分終了
        Return isFinishedDistribute
    End Function

    ''' <summary>
    ''' 全員、割引種別が一般でない精算情報の按分
    ''' </summary>
    Private Sub distributeSeisanInfoAllNOTGeneralWaribikiType(prmSeisanInfo As SeisanInfoEntity,
                                                           sysDates As Hashtable,
                                                           ByRef seisanInfoSetEntity As SeisanInfoListSetEntity
                                                           )
        '取扱手数料を精算内訳に追加
        If Me.ToriatukaiFee > 0 Then
            If Not Me._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)) Then
                Me._undistributedKinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo), Me.ToriatukaiFee)
            End If

            Me._seikyuByWaribikiType.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo),
                          Me._undistributedKinBySeisanKoumoku(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)))
        End If

        'キャンセル料を精算内訳に追加
        If Me.Cancel > 0 Then
            If Not Me._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.cancel_ryo)) Then
                Me._undistributedKinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.cancel_ryo), Me.Cancel)
            End If
        End If

        '割引金額を精算内訳に追加
        If Me.WaribikiTotal > 0 Then
            If Not Me._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.waribiki_kingaku)) Then
                Me._undistributedKinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.waribiki_kingaku), Me.WaribikiTotal)
            End If
        End If

        '精算情報エンティティを作成
        Me.setSeisanInfoSetEntity(seisanInfoSetEntity,
                                  prmSeisanInfo,
                                  Me._undistributedKinBySeisanKoumoku,
                                  sysDates)
    End Sub

    ''' <summary>
    ''' 割引種別が一般の精算情報の按分
    ''' </summary>
    Private Sub distributeSeisanInfoGeneralWaribikiType(prmSeisanInfo As SeisanInfoEntity,
                                                        sysDates As Hashtable,
                                                        ByRef seisanInfoSetEntity As SeisanInfoListSetEntity
                                                        )

        '精算項目ごとの未按分金から全て、精算内訳を作成する
        Dim undistributedKinBySeisanKoumoku = Me._undistributedKinBySeisanKoumoku _
                                              .Where(Function(undisributedKin) undisributedKin.Value <> 0)

        '精算内訳を格納するDictionary（キー：精算項目、値：精算内訳の金額）
        Dim seisanUtiwakeIppan As New Dictionary(Of String, Integer)
        For Each seisanKoumoku In undistributedKinBySeisanKoumoku
            seisanUtiwakeIppan.Add(seisanKoumoku.Key, seisanKoumoku.Value)
        Next 'ここまで精算項目ごとのループ

        '予約センター返金があれば、精算内訳に追加
        If Me.YoyakuCenterHenkin <> 0 Then
            seisanUtiwakeIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.hurikomi_yoyaku_center_modosi),
                               Me.YoyakuCenterHenkin)
        End If

        '取扱手数料を精算内訳に追加
        If Me.ToriatukaiFee > 0 Then
            If Not Me._undistributedKinBySeisanKoumoku.ContainsKey(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)) Then
                Me._undistributedKinBySeisanKoumoku.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo), Me.ToriatukaiFee)
            End If

            seisanUtiwakeIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo),
                              Me._undistributedKinBySeisanKoumoku(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.toriatukai_tesuryo)))
        End If

        '一般割引の存在確認
        Dim seikiChargeByWaribikiTypeGeneral As Integer = 0
        Dim waribikiByWaribikiTypeGeneral As Integer = 0
        If _seikyuByWaribikiType.ContainsKey(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)) Then
            '割引種別を一般割引へ変更
            prmSeisanInfo.waribikiType.Value = CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)
            '一般割引の正規料金、割引額
            seikiChargeByWaribikiTypeGeneral = Me._seikiChargeByWaribikiType(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount))
            waribikiByWaribikiTypeGeneral = Me._waribikiByWaribikiType(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount))
        End If

        '        キャンセル料を精算内訳に追加
        '定期：     払戻手数料、企画かつ内金発券でない：キャンセル料
        If Me.IsTeiki Then
            seisanUtiwakeIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.haraimodosi_tesuryo),
                                  Me.Cancel)
        ElseIf Me.IsKikaku Then
            seisanUtiwakeIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.cancel_ryo),
                                 Me.Cancel)
        End If

        '定期　または　企画かつ内金発券でない場合、
        '割引額を精算内訳に追加
        If waribikiByWaribikiTypeGeneral > 0 Then
            seisanUtiwakeIppan.Add(CommonHakken.convertEnumToString(FixedCd.SeisanItemCd.waribiki_kingaku),
                                waribikiByWaribikiTypeGeneral)
        End If

        '精算情報セットエンティティの作成
        Me.setSeisanInfoSetEntity(seisanInfoSetEntity,
                                  prmSeisanInfo,
                                  seisanUtiwakeIppan,
                                  sysDates)

        '按分終了
    End Sub

    ''' <summary>
    ''' 按分処理
    ''' </summary>
    ''' <param name="seisanKoumokuA">精算項目A</param>
    ''' <param name="unDistributedKin">未按分金</param>
    ''' <param name="seisanUtiwake">精算内訳（キー：精算項目コード、値：金額</param>
    Private Sub distribute(seisanKoumokuA As String,
                           ByRef unDistributedKin As Integer,
                           ByRef seisanUtiwake As Dictionary(Of String, Integer))

        '精算内訳のキーへ「精算項目A」を追加
        If Not seisanUtiwake.ContainsKey(seisanKoumokuA) Then
            seisanUtiwake.Add(seisanKoumokuA, 0)
        End If

        If unDistributedKin <= _undistributedKinBySeisanKoumoku(seisanKoumokuA) Then
            '未按分金 <= 「精算項目A」の入金額の場合

            '未按分金を全て按分し、内訳を生成　※複数の精算情報（割引種別）へ「精算項目A」の按分はまたがる
            Me._undistributedKinBySeisanKoumoku(seisanKoumokuA) -= unDistributedKin

            seisanUtiwake(seisanKoumokuA) += unDistributedKin
            unDistributedKin = 0

        Else
            '未按分金 > 「精算項目A」の入金額の場合

            '「精算項目A」の入金分を按分し、内訳を生成　※当割引種別のみへ「精算項目A」を按分
            seisanUtiwake(seisanKoumokuA) += Me._undistributedKinBySeisanKoumoku(seisanKoumokuA)

            unDistributedKin -= Me._undistributedKinBySeisanKoumoku(seisanKoumokuA)
            Me._undistributedKinBySeisanKoumoku(seisanKoumokuA) = 0
        End If
    End Sub

    ''' <summary>
    ''' 精算情報セットエンティティの設定
    ''' </summary>
    ''' <param name="seisanUtiwake"></param>
    ''' <param name="sysDates"></param>
    Private Sub setSeisanInfoSetEntity(ByRef seisanInfoSetEntity As SeisanInfoListSetEntity,
                                       prmSeisanInfo As SeisanInfoEntity,
                                       seisanUtiwake As Dictionary(Of String, Integer),
                                       sysDates As Hashtable)

        If seisanInfoSetEntity.SeisanInfoEntityList Is Nothing Then
            seisanInfoSetEntity.SeisanInfoEntityList = New List(Of SeisanInfoEntity)
        End If
        If seisanInfoSetEntity.SeisanInfoSankaNinzuEntityList Is Nothing Then
            seisanInfoSetEntity.SeisanInfoSankaNinzuEntityList = New List(Of SeisanInfoSankaNinzuEntity)
        End If
        If seisanInfoSetEntity.SeisanInfoUtiwakeEntityList Is Nothing Then
            seisanInfoSetEntity.SeisanInfoUtiwakeEntityList = New List(Of SeisanInfoUtiwakeEntity)
        End If

        '精算情報エンティティリストの設定
        prmSeisanInfo.seisanInfoSeq.Value = CommonHakken.createSeisanInfoSeq() 'PK採番
        setSeisanInfoEntityList(seisanInfoSetEntity.SeisanInfoEntityList,
                                prmSeisanInfo,
                                sysDates)

        '精算情報（参加人数）エンティティリストの設定
        setSeisanInfoSankaNinzuEntity(seisanInfoSetEntity.SeisanInfoSankaNinzuEntityList,
                                      prmSeisanInfo,
                                      sysDates)

        '精算情報内訳エンティティリストの設定
        setSeisanInfoUtiwakeEntity(seisanInfoSetEntity.SeisanInfoUtiwakeEntityList,
                                   prmSeisanInfo,
                                   seisanUtiwake,
                                   sysDates)
    End Sub

    ''' <summary>
    ''' 精算情報エンティティの作成
    ''' </summary>
    ''' <param name="sysDates"></param>
    Private Sub setSeisanInfoEntityList(ByRef list As List(Of SeisanInfoEntity),
                                               prmSeisanInfo As SeisanInfoEntity,
                                                sysDates As Hashtable)



        '割引種別単位でレコードを作成
        Dim ent As New SeisanInfoEntity
        ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
        ent.yoyakuNo.Value = Me.ParamData.YoyakuNo

        ent.kenno.Value = Me._newKenNo
        ent.seisanInfoSeq.Value = prmSeisanInfo.seisanInfoSeq.Value
        ent.seq.Value = prmSeisanInfo.seq.Value

        ent.createDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer)
        ent.createTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer)

        ent.companyCd.Value = UserInfoManagement.companyCd
        ent.eigyosyoCd.Value = UserInfoManagement.eigyosyoCd
        ent.tantosyaCd.Value = UserInfoManagement.userId
        ent.signonTime.Value = CommonHakken.convertDateToIntTime(UserInfoManagement.signonDate)

        ent.crsCd.Value = Me.txtCourseCd.Text.Trim()

        '出発日
        ent.syuptDay.Value = CommonHakken.convertDateToInt(Me.ucoSyuptDay.Value)
        '号車
        Dim intGousya As Integer = 0
        Integer.TryParse(Me.txtGousya.Text.Trim(), intGousya)
        ent.gousya.Value = intGousya

        ent.tokuteiDayFlg.Value = ""
        ent.teikiCrsKbn.Value = ""
        ent.crsKind.Value = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_KIND"), "")
        ent.crsKbn1.Value = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_KBN_1"), "")
        ent.crsKbn2.Value = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("CRS_KBN_2"), "")
        ent.accessCd.Value = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("ACCESS_CD"), "")

        ent.uriageKbn.Value = ""
        ent.hakkenKbn.Value = ""

        Dim agentCd As String = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("AGENT_CD"), "")
        ent.agentCd.Value = agentCd
        ent.seatKbn.Value = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of String)("YOYAKU_ZASEKI_KBN"), "")
        ent.tasyaKenno.Value = ""
        ent.tasyaKennoIssueDay.Value = 0
        ent.tickettypeCd.Value = If(Me.IsTeiki, CommonHakken.TicketTypeCdTeiki, CommonHakken.TicketTypeCdKikaku)

        ent.otherUriageSyohinKbn.Value = ""
        ent.otherUriageSyohinCd1.Value = ""
        ent.otherUriageSyohinCd2.Value = ""
        ent.otherUriageSyohinQuantity.Value = 0
        ent.otherUriageSyohinTanka.Value = 0
        ent.otherUriageSyohinBiko.Value = ""

        ent.seisanKbn.Value = prmSeisanInfo.seisanKbn.Value '精算区分

        'クーポン売上、払戻
        Dim couponRefund As Integer = If(prmSeisanInfo.couponRefund.Value, 0)
        Dim couponUriage As Integer = 0

        If prmSeisanInfo.waribikiType.Value.Equals(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)) _
        OrElse String.IsNullOrEmpty(prmSeisanInfo.waribikiType.Value) Then
            '正規料金、一般割引の場合、合算
            If Me._seikiChargeByWaribikiType.Keys.Contains(CommonHakken.WaribikiTypeSeikiCharge) Then
                couponUriage += Me._seikiChargeByWaribikiType(CommonHakken.WaribikiTypeSeikiCharge)
            End If
            If Me._seikiChargeByWaribikiType.Keys.Contains(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)) Then
                couponUriage += Me._seikiChargeByWaribikiType(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount))
            End If

        Else
            couponUriage = Me._seikiChargeByWaribikiType(prmSeisanInfo.waribikiType.Value)
        End If

        '発券金額
        Dim dHakkenKingaku As Decimal = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Decimal?)("HAKKEN_KINGAKU"), 0)
        Dim hakkenKingaku As Integer = Convert.ToInt32(dHakkenKingaku)

        ent.couponRefund.Value = couponRefund
        ent.couponUriage.Value = couponUriage

        '割引種別
        If prmSeisanInfo.waribikiType.Value.Equals(CommonHakken.WaribikiTypeSeikiCharge) Then
            '正規料金のダミー値書換え
            ent.waribikiType.Value = ""
        Else
            ent.waribikiType.Value = prmSeisanInfo.waribikiType.Value
        End If

        'ノーサイン区分=オンラインクレジット決済
        If Me.OnlineCredit > 0 Then
            ent.nosignKbn.Value = "Y"
        End If

        'AGT請求対象フラグ
        Dim atoSeisanKbn As Integer = 0
        If Me._agentMaster IsNot Nothing Then
            '後払精算区分の確認
            Dim teikiKikaku As String = If(Me.IsTeiki, "TEIKI", "KIKAKU")
            atoSeisanKbn = If(Me._agentMaster.Rows(0).Field(Of Short?)($"{teikiKikaku}_ATO_SEISAN_KBN"), 0)
        End If
        If atoSeisanKbn = Convert.ToInt32(FixedCd.AtoSeisanKbnType.HakkenSeisan) Then
            ent.agtSeikyuTaisyoFlg.Value = Convert.ToInt32(FixedCd.AtoSeisanKbnType.HakkenSeisan)
        ElseIf atoSeisanKbn = Convert.ToInt32(FixedCd.AtoSeisanKbnType.ChakukenSeisan) Then
            ent.agtSeikyuTaisyoFlg.Value = Convert.ToInt32(FixedCd.AtoSeisanKbnType.ChakukenSeisan)
        End If

        ent.deleteDay.Value = 0
        ent.entryDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
        ent.entryTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
        ent.entryPersonCd.Value = UserInfoManagement.userId
        ent.entryPgmid.Value = PgmId
        ent.updateDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
        ent.updateTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
        ent.updatePersonCd.Value = UserInfoManagement.userId
        ent.updatePgmid.Value = PgmId
        ent.systemEntryDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        ent.systemEntryPersonCd.Value = UserInfoManagement.userId
        ent.systemEntryPgmid.Value = PgmId
        ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
        ent.systemUpdatePgmid.Value = PgmId

        list.Add(ent)
    End Sub

    ''' <summary>
    ''' 精算情報（参加人数）エンティティの作成
    ''' </summary>
    Private Sub setSeisanInfoSankaNinzuEntity(ByRef list As List(Of SeisanInfoSankaNinzuEntity),
                                                      prmSeisanInfo As SeisanInfoEntity,
                                                      sysDates As Hashtable)

        Dim waribikiType As String = prmSeisanInfo.waribikiType.Value

        Dim rows As EnumerableRowCollection(Of DataRow) = Nothing

        '割引コードごとの集計から該当割引種別のみ抽出
        If String.IsNullOrEmpty(waribikiType) OrElse waribikiType.Equals(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount)) Then
            '正規料金または割引ありの場合
            rows = Me._infoByWaribikiCdJininCd.AsEnumerable() _
                  .Where(Function(row)
                             Return If(row.Field(Of String)("WARIBIKI_TYPE_KBN"), "").Equals(CommonHakken.WaribikiTypeSeikiCharge) _
                                       OrElse If(row.Field(Of String)("WARIBIKI_TYPE_KBN"), "").Equals(CommonHakken.convertEnumToString(FixedCd.WaribikiTypeKbn.GeneralDiscount))
                         End Function)


        Else
            rows = Me._infoByWaribikiCdJininCd.AsEnumerable() _
                   .Where(Function(row) If(row.Field(Of String)("WARIBIKI_TYPE_KBN"), "").Equals(waribikiType))
        End If

        'キー：料金カラムセット（料金区分、料金区分（人員）コード、値：人数
        Dim ninzuCnter As New Dictionary(Of String, Integer)

        '同一料金区分、料金区分（人員）を集計
        For Each row As DataRow In rows
            Dim chargeColumns As String = If(row.Field(Of String)(CommonHakken.ChargeColumnsSet), "")
            Dim ninzu As Integer = If(row.Field(Of Integer?)("NINZU"), 0)

            '一般割引、正規料金のPK競合対策
            If Not ninzuCnter.Keys.Contains(chargeColumns) Then
                ninzuCnter.Add(chargeColumns, ninzu)
            Else
                ninzuCnter(chargeColumns) += ninzu
            End If
        Next

        '集計した人数から値を取り出し
        For Each item In ninzuCnter
            '精算情報（参加人数）エンティティを生成
            Dim ent As New SeisanInfoSankaNinzuEntity

            ent.yoyakuNo.Value = Me.ParamData.YoyakuNo
            ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn

            Dim chargeColumns As String = item.Key
            Dim ninzu As Integer = item.Value


            '区分No
            Dim kbnNo As Integer = 0
            '料金区分
            Dim chargeKbn As String = ""
            '料金区分（人員）コード
            Dim jininCd As String = ""
            CommonHakken.separateChargeColumnsSet(chargeColumns, kbnNo, chargeKbn, jininCd)

            ent.kbnNo.Value = kbnNo
            ent.chargeKbn.Value = chargeKbn
            ent.chargeKbnJininCd.Value = jininCd

            ent.seisanInfoSeq.Value = prmSeisanInfo.seisanInfoSeq.Value
            ent.seq.Value = prmSeisanInfo.seq.Value

            ent.seisanKbn.Value = prmSeisanInfo.seisanKbn.Value
            ent.kenno.Value = Me._newKenNo

            ent.sankaNinzu.Value = ninzuCnter(chargeColumns)

            ent.deleteDay.Value = 0
            ent.entryDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
            ent.entryTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
            ent.entryPersonCd.Value = UserInfoManagement.userId
            ent.entryPgmid.Value = PgmId
            ent.updateDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
            ent.updateTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
            ent.updatePersonCd.Value = UserInfoManagement.userId
            ent.updatePgmid.Value = PgmId
            ent.systemEntryDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
            ent.systemEntryPersonCd.Value = UserInfoManagement.userId
            ent.systemEntryPgmid.Value = PgmId
            ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
            ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
            ent.systemUpdatePgmid.Value = PgmId

            list.Add(ent)
        Next 'ここまでパラメータのDictionaryのループ

    End Sub

    ''' <summary>
    ''' 精算情報内訳エンティティの生成
    ''' </summary>
    Private Sub setSeisanInfoUtiwakeEntity(ByRef list As List(Of SeisanInfoUtiwakeEntity),
                                           prmSeisanInfo As SeisanInfoEntity,
                                           seisanUtiwake As Dictionary(Of String, Integer),
                                           sysDates As Hashtable)

        'パラメータより値を取り出し
        For Each item In seisanUtiwake
            Dim prmCd As String = item.Key
            Dim prmKingaku As Integer = item.Value

            '金額が0円ならエンティティを生成しない
            If prmKingaku = 0 Then
                Continue For
            End If

            Dim isListAddedFromGrd As Boolean = False
            'グリッド上の項目からの精算情報内訳エンティティの作成
            isListAddedFromGrd = Me.setSeisanInfoUtiwakeEntityWithGrd(list,
                                                                      prmSeisanInfo,
                                                                      seisanUtiwake,
                                                                      sysDates,
                                                                      item)

            'グリッドのループを抜けてもエンティティを追加できていない場合
            If isListAddedFromGrd = False Then
                'グリッドにない項目からの精算情報内訳エンティティの作成
                Me.setSeisanInfoUtiwakeEntityWithoutGrd(list,
                                                        prmSeisanInfo,
                                                        seisanUtiwake,
                                                        sysDates,
                                                        item)
            End If
        Next 'ここまでパラメータのDictionaryのループ
    End Sub

    ''' <summary>
    ''' 精算情報内訳エンティティの生成（グリッドに存在する場合
    ''' </summary>
    ''' <returns></returns>
    Private Function setSeisanInfoUtiwakeEntityWithGrd(ByRef list As List(Of SeisanInfoUtiwakeEntity),
                                                             prmSeisanInfo As SeisanInfoEntity,
                                                             seisanUtiwake As Dictionary(Of String, Integer),
                                                             sysDates As Hashtable,
                                                          item As KeyValuePair(Of String, Integer)) As Boolean

        Dim prmCd As String = item.Key
        Dim prmKingaku As Integer = item.Value

        Dim isListAdded = False
        For Each row As Row In Me.grdSeisan.Rows
            'ヘッダー行なら次の行へ
            If row.Index = 0 Then
                Continue For
            End If

            Dim grdCd As String = If(TryCast(row.Item("SEISAN_KOUMOKU_CD"), String), "")

            '空セル選択時は、次の行へ
            If grdCd.Equals(CommonHakken.EmptyCellKey) Then
                Continue For
            End If

            'パラメータの情報でなければ、次の行へ
            If Not prmCd.Equals(grdCd) Then
                Continue For
            End If

            '同精算項目コードのエンティティが作成済みなら、次の行へ
            Dim isCreatedEntity As Boolean = list.Any(Function(entInList) entInList.seisanKoumokuCd.Value.Equals(prmCd))
            If isCreatedEntity Then
                Continue For
            End If

            '精算情報内訳のエンティティをグリッドを基に生成
            Dim ent As New SeisanInfoUtiwakeEntity
            ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
            ent.yoyakuNo.Value = Me.ParamData.YoyakuNo

            ent.seisanInfoSeq.Value = prmSeisanInfo.seisanInfoSeq.Value
            ent.seq.Value = prmSeisanInfo.seq.Value

            ent.seisanKbn.Value = prmSeisanInfo.seisanKbn.Value
            ent.kenno.Value = Me._newKenNo

            ent.seisanKoumokuCd.Value = prmCd
            If prmKingaku >= 0 Then
                ent.kingaku.Value = prmKingaku '引数で渡されたDictionaryの値
            Else
                ent.kingaku.Value = -prmKingaku '（貸方）
            End If

            ent.hurikomiKbn.Value = If(TryCast(row.Item("HURIKOMI_KBN"), String), "")
            ent.issueCompanyCd.Value = If(TryCast(row.Item("ISSUE_COMPANY_CD"), String), "")
            ent.biko.Value = If(TryCast(row.Item("BIKO"), String), "")

            ent.deleteDay.Value = 0
            ent.entryDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
            ent.entryTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
            ent.entryPersonCd.Value = UserInfoManagement.userId
            ent.entryPgmid.Value = PgmId
            ent.updateDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
            ent.updateTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
            ent.updatePersonCd.Value = UserInfoManagement.userId
            ent.updatePgmid.Value = PgmId
            ent.systemEntryDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
            ent.systemEntryPersonCd.Value = UserInfoManagement.userId
            ent.systemEntryPgmid.Value = PgmId
            ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
            ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
            ent.systemUpdatePgmid.Value = PgmId

            list.Add(ent)
            isListAdded = True
        Next

        Return isListAdded
    End Function

    ''' <summary>
    ''' 精算情報内訳エンティティの生成（グリッドに存在しない場合
    ''' </summary>
    ''' <returns></returns>
    Private Function setSeisanInfoUtiwakeEntityWithoutGrd(ByRef list As List(Of SeisanInfoUtiwakeEntity),
                                                             prmSeisanInfo As SeisanInfoEntity,
                                                             seisanUtiwake As Dictionary(Of String, Integer),
                                                             sysDates As Hashtable,
                                                            item As KeyValuePair(Of String, Integer)) As List(Of SeisanInfoUtiwakeEntity)

        Dim prmCd As String = item.Key
        Dim prmKingaku As Integer = item.Value

        Dim ent As New SeisanInfoUtiwakeEntity
        ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
        ent.yoyakuNo.Value = Me.ParamData.YoyakuNo

        ent.seisanInfoSeq.Value = prmSeisanInfo.seisanInfoSeq.Value
        ent.seq.Value = prmSeisanInfo.seq.Value

        ent.seisanKbn.Value = prmSeisanInfo.seisanKbn.Value
        ent.kenno.Value = Me._newKenNo

        ent.seisanKoumokuCd.Value = prmCd
        If prmKingaku >= 0 Then
            ent.kingaku.Value = prmKingaku '引数で渡されたDictionaryの値
        Else
            ent.kingaku.Value = -prmKingaku '（貸方）
        End If

        ent.hurikomiKbn.Value = ""
        ent.issueCompanyCd.Value = ""
        ent.biko.Value = ""

        ent.deleteDay.Value = 0
        ent.entryDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
        ent.entryTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
        ent.entryPersonCd.Value = UserInfoManagement.userId
        ent.entryPgmid.Value = PgmId
        ent.updateDay.Value = DirectCast(sysDates(KeyIntSysDate), Integer?)
        ent.updateTime.Value = DirectCast(sysDates(KeyIntSysTimeHhMmSs), Integer?)
        ent.updatePersonCd.Value = UserInfoManagement.userId
        ent.updatePgmid.Value = PgmId
        ent.systemEntryDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        ent.systemEntryPersonCd.Value = UserInfoManagement.userId
        ent.systemEntryPgmid.Value = PgmId
        ent.systemUpdateDay.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        ent.systemUpdatePersonCd.Value = UserInfoManagement.userId
        ent.systemUpdatePgmid.Value = PgmId

        list.Add(ent)

        Return list
    End Function

#Region "入返金情報エンティティ"
    ''' <summary>
    ''' 入返金情報エンティティの作成
    ''' </summary>
    ''' <param name="isVoid"></param>
    ''' <param name="sysDates"></param>
    ''' <returns></returns>
    Private Function createNyuukinInfoEntity(isVoid As Boolean, sysDates As Hashtable) As NyuukinInfoEntity
        Dim ent As New NyuukinInfoEntity

        ent.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
        ent.yoyakuNo.Value = Me.ParamData.YoyakuNo

        ent.hakkenFlg.Value = If(isVoid, "", "Y")

        ent.updateDate.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        ent.updateUserId.Value = UserInfoManagement.userId
        ent.updateClient.Value = PgmId

        Return ent
    End Function

    ''' <summary>
    ''' 入返金情報エンティティの作成（予約センターあり時）
    ''' </summary>
    ''' <param name="physicsNameList"></param>
    ''' <param name="sysDates"></param>
    ''' <returns></returns>
    Private Function createNyuukinInfoEntityHenkinAri(ByRef physicsNameList As List(Of String), sysDates As Hashtable) As NyuukinInfoEntity
        Dim entity As New NyuukinInfoEntity

        '予約区分
        entity.yoyakuKbn.Value = Me.ParamData.YoyakuKbn
        physicsNameList.Add(entity.yoyakuKbn.PhysicsName)
        '予約NO
        entity.yoyakuNo.Value = Me.ParamData.YoyakuNo
        physicsNameList.Add(entity.yoyakuNo.PhysicsName)
        'SEQ
        Dim seq As Integer = CommonHakken.createNyuukinInfoSeq(Me.ParamData.YoyakuKbn, Me.ParamData.YoyakuNo)
        entity.seq.Value = seq
        physicsNameList.Add(entity.seq.PhysicsName)
        '年
        Dim syuptDay As Integer = If(Me._yoyakuInfoBasicTable.Rows(0).Field(Of Integer?)("SYUPT_DAY"), 0)
        Dim strYear As String = syuptDay.ToString().PadLeft(8, "0"c).Substring(0, 4)
        Dim year As Integer = Integer.Parse(strYear)
        entity.nyuukinYear.Value = year
        physicsNameList.Add(entity.nyuukinYear.PhysicsName)
        '会社コード
        entity.companyCd.Value = UserInfoManagement.companyCd
        physicsNameList.Add(entity.companyCd.PhysicsName)
        '営業所
        entity.eigyosyoCd.Value = UserInfoManagement.eigyosyoCd
        physicsNameList.Add(entity.eigyosyoCd.PhysicsName)
        '入金種別
        entity.nyuukinKind.Value = If(Me.IsTeiki, CommonHakken.NyuukinKindTeiki, CommonHakken.NyuukinKindKikaku)
        physicsNameList.Add(entity.nyuukinKind.PhysicsName)
        '発券振込区分
        entity.hakkenHurikomiKbn.Value = CommonHakken.HakkenHurikomiKbnYoyakuCenterHenkin
        physicsNameList.Add(entity.hakkenHurikomiKbn.PhysicsName)
        '処理日
        entity.processDate.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        physicsNameList.Add(entity.processDate.PhysicsName)
        '入金額１（振込）
        entity.nyuukinGaku1.Value = Me.YoyakuCenterHenkin
        physicsNameList.Add(entity.nyuukinGaku1.PhysicsName)
        '入金額２（現金）
        entity.nyuukinGaku2.Value = 0
        physicsNameList.Add(entity.nyuukinGaku2.PhysicsName)
        '入金額３（その他）
        entity.nyuukinGaku3.Value = 0
        physicsNameList.Add(entity.nyuukinGaku3.PhysicsName)
        '入金額４（振込手数料）
        entity.nyuukinGaku4.Value = 0
        physicsNameList.Add(entity.nyuukinGaku4.PhysicsName)
        '入金額５
        entity.nyuukinGaku5.Value = 0
        physicsNameList.Add(entity.nyuukinGaku5.PhysicsName)
        '振込区分
        entity.hurikomiKbn.Value = ""
        physicsNameList.Add(entity.hurikomiKbn.PhysicsName)
        '振込先口座名
        entity.hurikomiSakiKozaName.Value = ""
        physicsNameList.Add(entity.hurikomiSakiKozaName.PhysicsName)
        '券番 
        entity.kenNo.Value = Me._newKenNo
        physicsNameList.Add(entity.kenNo.PhysicsName)
        'キャンセル区分
        entity.cancelKbn.Value = ""
        physicsNameList.Add(entity.cancelKbn.PhysicsName)
        '発券フラグ
        entity.hakkenFlg.Value = "Y"
        physicsNameList.Add(entity.hakkenFlg.PhysicsName)
        '振替フラグ 
        entity.hurikaeFlg.Value = 0
        physicsNameList.Add(entity.hurikaeFlg.PhysicsName)
        '連携結果
        entity.renkeiResult.Value = "0"
        physicsNameList.Add(entity.renkeiResult.PhysicsName)
        '登録日時
        entity.entryDate.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        physicsNameList.Add(entity.entryDate.PhysicsName)
        '登録ユーザーID
        entity.entryUserId.Value = UserInfoManagement.userId
        physicsNameList.Add(entity.entryUserId.PhysicsName)
        '登録クライアント名
        entity.entryClient.Value = PgmId
        physicsNameList.Add(entity.entryClient.PhysicsName)
        '更新日時
        entity.updateDate.Value = DirectCast(sysDates(KeyDtSysDate), Date)
        physicsNameList.Add(entity.updateDate.PhysicsName)
        '更新ユーザーID
        entity.updateUserId.Value = UserInfoManagement.userId
        physicsNameList.Add(entity.updateUserId.PhysicsName)
        '更新クライアント名
        entity.updateClient.Value = PgmId
        physicsNameList.Add(entity.updateClient.PhysicsName)

        Return entity

    End Function
#End Region
#End Region
#End Region

#Region "ユーティリティ"
    ''' <summary>
    ''' システム日付（各型）の取得
    ''' </summary>
    ''' <returns>各型のシステム日付を格納したHashTable</returns>
    Private Function getSysDates() As Hashtable

        'サーバー日付を取得
        Dim dtSysDate As Date = createFactoryDA.getServerSysDate()

        '各型へフォーマット
        Dim strSysDate As String = dtSysDate.ToString("yyyyMMdd")
        Dim intSysDate As Integer = 0
        Integer.TryParse(strSysDate, intSysDate)
        Dim strSysTimeHhMmSs As String = dtSysDate.ToString("HHmmss")
        Dim intSysTimeHhMmSs As Integer = 0
        Integer.TryParse(strSysTimeHhMmSs, intSysTimeHhMmSs)
        Dim strSysTimeHhMm As String = dtSysDate.ToString("HHmm")
        Dim intSysTimeHhMm As Integer = 0
        Integer.TryParse(strSysTimeHhMm, intSysTimeHhMm)

        'ハッシュテーブルへ各型のサーバー日付を格納
        Dim sysDates As New Hashtable
        sysDates.Add(KeyDtSysDate, dtSysDate)
        sysDates.Add(KeyStrSysDate, strSysDate)
        sysDates.Add(KeyIntSysDate, intSysDate)
        sysDates.Add(KeyStrSysTimeHhMmSs, strSysTimeHhMmSs)
        sysDates.Add(KeyIntSysTimeHhMmSs, intSysTimeHhMmSs)
        sysDates.Add(KeyStrSysTimeHhMm, strSysTimeHhMm)
        sysDates.Add(KeyIntSysTimeHhMm, intSysTimeHhMm)

        Return sysDates
    End Function

    ''' <summary>
    ''' 各グリッドの活性化
    ''' </summary>
    ''' <param name="enable"></param>
    Private Sub enableGrds(enable As Boolean)
        Me.grdWaribikiCharge.Enabled = enable
        Me.grdSeisan.Enabled = enable
        Me.grdKingaku.Enabled = enable
        Me.grdKingaku2.Enabled = enable
    End Sub
#End Region
#End Region
#End Region
End Class