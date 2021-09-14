Imports System.ComponentModel
Imports System.Drawing

''' <summary>
''' S04_0103 船車券AGT乗車確認出力
''' </summary>
Public Class S04_0103
    Inherits PT21

#Region "定数"
    ''' <summary>
    ''' 画面PgmId
    ''' </summary>
    Private Const PgmId As String = "S04_0103"
    ''' <summary>
    ''' 画面名
    ''' </summary>
    Private Const ScreenName As String = "船車券AGT乗車確認出力"
    ''' <summary>
    ''' 条件GroupBoxのTop座標
    ''' </summary>
    Public Const TopGbxCondition = 41
    ''' <summary>
    ''' 条件GroupBoxのマージン
    ''' </summary>
    Public Const MarginGbxCondition = 6
    ''' <summary>
    ''' 条件GroupBox非表示時のGrouBoxAreaの高さ
    ''' </summary>
    Public Const HeightGbxAreasOnNotVisibleCondition = 348
    ''' <summary>
    ''' 件数 件
    ''' </summary>
    Private Const Ken = "件"
    ''' <summary>
    ''' 件数の初期表示
    ''' </summary>
    Private Const ZeroKen = "0件"
    ''' <summary>
    ''' 日付初期値
    ''' </summary>
    Private Const DayInitialValue = "__/__/__"
    ''' <summary>
    ''' 時間初期値
    ''' </summary>
    Private Const TimeInitialValue = "__:__"
    ''' <summary>
    ''' チェックインフラグ　乗車済み
    ''' </summary>
    Private Const CheckinFlgJyosyaAlready = "1"
    ''' <summary>
    ''' チェックインフラグ　NOSHOW
    ''' </summary>
    Private Const CheckinFlgNOSHOW = "2"
    ''' <summary>
    ''' NOSHOWフラグ　"Y"
    ''' </summary>
    Private Const NOSHOWFlg = "Y"
    ''' <summary>
    ''' 列名 乗車済み
    ''' </summary>
    Private Const NameColJyosyaAlready = "colJyosyaAlready"
    ''' <summary>
    ''' 列名 NOSHOW
    ''' </summary>
    Private Const NameColNOSHOW = "colNOSHOW"
    ''' <summary>
    ''' 列名 請求対象
    ''' </summary>
    Private Const NameColInquiry = "colInquiry"
    ''' <summary>
    ''' 状態　JTB発券
    ''' </summary>
    Private Const StateJTB = "1"
    ''' <summary>
    ''' 状態　KNT発券
    ''' </summary>
    Private Const StateKNT = "3"
    ''' <summary>
    ''' 前月
    ''' </summary>
    Private Const BfMon = -1
    ''' <summary>
    ''' 1日前
    ''' </summary>
    Private Const OneDayBf = -1
    ''' <summary>
    ''' 状態　「消」
    ''' </summary>
    Private Const StateCancel = "消"
    ''' <summary>
    ''' 状態　「削」
    ''' </summary>
    Private Const StateDelete = "削"
    ''' <summary>
    ''' 状態　「指」
    ''' </summary>
    Private Const StateReserve = "指"
    ''' <summary>
    ''' 状態　「券」
    ''' </summary>
    Private Const StateKen = "券"
    ''' <summary>
    ''' 検索結果最大表示件数件数
    ''' </summary>
    Public Const MaxKensu As Integer = 100

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

#End Region

#Region "変数"

    ''' <summary>
    ''' コース情報テーブル
    ''' </summary>
    Private _crsInfoTable As DataTable = Nothing
    ''' <summary>
    ''' 予約情報テーブル
    ''' </summary>
    Private _yoyakuInfoTable As DataTable = Nothing
    ''' <summary>
    ''' 検索用テーブル
    ''' </summary>
    Private _searchForTable As Hashtable = Nothing
    ''' <summary>
    ''' 予約情報（基本）エンティティ
    ''' </summary>
    Private _yoyakuInfoBasicEntity As New YoyakuInfoBasicEntity
    ''' <summary>
    ''' 精算情報エンティティ
    ''' </summary>
    Private _seisanInfoEntity As New TSeisanInfoEntity
    ''' <summary>
    ''' 判定用リスト
    ''' </summary>
    Private _judgmentForList As New Hashtable
    ''' <summary>
    ''' チェックボックス解除フラグ
    ''' </summary>
    Private _releaseFlg As Boolean = False

#Region "フィールド"
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
    ''' 画面起動時処理
    ''' </summary>
    Protected Overrides Sub StartupOrgProc()

        ' 画面初期化
        Me.setControlInitiarize()

    End Sub

    ''' <summary>
    ''' 条件GroupBox表示制御ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnVisiblerCondition_Click(sender As Object, e As EventArgs) Handles btnVisiblerCondition.Click
        Me.VisibleGbxCondition = Not Me.VisibleGbxCondition

        Dim offSet As Integer = Me.HeightGbxCondition + MarginGbxCondition
        Dim harfOffset As Integer = CInt(offSet / 2)

        'GrpBoxArea1, 2の座標, サイズを表示/非表示に応じて変更
        If Me.VisibleGbxCondition Then
            '表示状態
            Me.btnVisiblerCondition.Text = "非表示 <<"

            Me.setGrpLayout()
        Else
            '非表示状態
            Me.btnVisiblerCondition.Text = "表示 >>"
            Me.gbxArea1.Height = HeightGbxAreasOnNotVisibleCondition
            Me.gbxArea2.Height = HeightGbxAreasOnNotVisibleCondition

            Me.gbxArea1.Top = TopGbxCondition
            Me.gbxArea2.Top = TopGbxCondition + Me.gbxArea1.Height + MarginGbxCondition
        End If
    End Sub

    ''' <summary>
    ''' F2：戻るボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF2_ClickOrgProc()

        Me.Close()
    End Sub

    ''' <summary>
    ''' キーダウン
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub F8_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyData
            Case Keys.F8
                Me.btnSearch.Select()
                MyBase.btnCom_Click(Me.btnSearch, e)
            Case Else
                Exit Sub
        End Select
    End Sub

    ''' <summary>
    ''' F8：検索ボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF8_ClickOrgProc()

        'エラーのクリア
        Me.errorClear()

        '入力項目のチェック
        If Me.isInputCheck() = False Then
            Exit Sub
        End If

        'コースグリッドの初期化
        Me.setGrdCrsInitiarize()

        '予約グリッドの初期化
        Me.setGrdYoyakuInitiarize()

        '検索用の値を格納
        Me.setSerchForValue()

        'コース情報テーブルの取得
        Me.getCrsInfoTable()

        '取得したデータが0件の場合
        If _crsInfoTable.Rows.Count <= 0 Then
            'メッセージ出力（該当するデータがありません。）
            CommonProcess.createFactoryMsg().messageDisp("E90_019")

            Exit Sub
        End If

        '表示件数オーバーの場合
        If _crsInfoTable.Rows.Count > MaxKensu Then
            'メッセージ出力（検索結果が最大設定可能数を超えました。）
            CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が")

        End If

        'コースグリッドに値を格納
        Me.setGrdCrs()

        '予約番号が入力されている場合
        Dim yoyakuKbnNo As Hashtable = Me.getYoyakuNo()

        If Not String.IsNullOrEmpty(yoyakuKbnNo("yoyakuNo").ToString) Then
            '全選択ボタンを押下
            Me.btnAllSelectionCrs.PerformClick()
        End If

        '検索ボタンにフォーカスを設定
        Me.ActiveControl = Me.btnSearch
    End Sub

    ''' <summary>
    ''' F8：検索ボタン押下イベント
    ''' </summary>
    ''' <param name="sender">イベント送信元</param>
    ''' <param name="e">イベントデータ</param>
    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click

        ' F8ボタン押下
        MyBase.btnCom_Click(Me.btnSearch, e)

        ' log出力
        createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理")

    End Sub

    ''' <summary>
    ''' F10：登録ボタン押下イベント
    ''' </summary>
    Protected Overrides Sub btnF10_ClickOrgProc()

        'エラーのクリア
        Me.errorClear()

        '必須項目のチェック
        Me.isUpdateCheck()

        If _yoyakuInfoTable Is Nothing Then
            Exit Sub
        End If

        '確認メッセージ（）
        If createFactoryMsg.messageDisp("Q90_001", "登録") = MsgBoxResult.Cancel Then

            Exit Sub
        End If

        For Each dataRow As DataRow In _yoyakuInfoTable.Rows

            'サーバー日付取得
            Dim sysdates As Hashtable = Me.getSysDates()

            '使用中フラグをUseに設定
            If Me.executeUsingFlgUse(sysdates, dataRow) = False Then
                'メッセージ出力（"登録に失敗しました。"）
                CommonProcess.createFactoryMsg().messageDisp("E90_025", "登録")
            End If

            'エンティティに値を格納
            Me.setEntityData(dataRow, sysdates)

            Dim yoyakuInfoCrsChargeChargeKbn As YoyakuInfoCrsChargeChargeKbnEntity = Me.createYoyakuInfoCrsChargeChargeKbnEntityForHakken(dataRow("YOYAKU_KBN").ToString, CInt(dataRow("YOYAKU_NO")), sysdates)
            Dim yoyakuInfoCrsChargeChargeKbnList As New List(Of YoyakuInfoCrsChargeChargeKbnEntity)
            yoyakuInfoCrsChargeChargeKbnList.Add(yoyakuInfoCrsChargeChargeKbn)

            '予約と精算の更新
            If Me.executeYoyakuSeisan(yoyakuInfoCrsChargeChargeKbnList) = False Then
                'メッセージ出力（"更新に失敗しました。"）
                CommonProcess.createFactoryMsg().messageDisp("E90_025", "登録")
            End If

            '使用中フラグをUnusedに設定
            If Me.executeUsingFlgUnused() = False Then
                'メッセージ出力（"更新に失敗しました。"）
                CommonProcess.createFactoryMsg().messageDisp("E90_025", "登録")
            End If
        Next

        'メッセージ出力（"登録が完了しました。")
        CommonProcess.createFactoryMsg().messageDisp("I90_002", "登録")

    End Sub

    ''' <summary>
    ''' 条件クリアボタン押下時
    ''' </summary>
    Protected Overrides Sub btnCLEAR_ClickOrgProc()

        ' 初期表示と同一の処理を実行
        Me.setControlInitiarize()

    End Sub

    ''' <summary>
    ''' 条件クリアボタン押下時
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnCLEAR_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClear.Click

        ' CLEARボタン押下
        MyBase.btnCom_Click(Me.btnClear, e)

    End Sub

    ''' <summary>
    ''' 代理店コードボタン押下イベント
    ''' </summary>
    ''' <param name="sender">イベント送信元</param>
    ''' <param name="e">イベントデータ</param>
    Private Sub btnGridRow_Click(ByVal sender As Object, ByVal e As C1.Win.C1FlexGrid.RowColEventArgs) Handles grdYoyaku.CellButtonClick

        Try
            '前処理
            MyBase.comPreEvent()

            'エラーのクリア
            Me.errorClear()

            Dim grd As FlexGridEx = TryCast(sender, FlexGridEx)
            If grd.Row <= 0 Then Return
            Dim tableNum As Integer = grd.Row - 1

            '企画の場合、代理店コードボタンを押下できない。
            If CInt(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) = CInt(FixedCd.CrsKindType.higaeri) OrElse
                        CInt(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) = CInt(FixedCd.CrsKindType.stay) OrElse
                            CInt(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) = CInt(FixedCd.CrsKindType.rcourse) Then

                'メッセージ出力（企画の場合は代理店コードを参照できません。）
                CommonProcess.createFactoryMsg().messageDisp("E90_006", "企画の場合", "代理店コードを参照")

                Exit Sub
            End If

            '画面間パラメータを用意
            Dim agentEntity As New MAgentEntity

            '代理店検索　画面展開
            Using form As New Master.S90_0104(agentEntity)
                form.ShowDialog()
                agentEntity = CType(form.getReturnEntity, MAgentEntity)
            End Using

            'エンティティに値がない時
            If agentEntity Is Nothing Then
                Exit Sub
            End If

            'グリッドに値を格納
            grd.Item(grd.Row, "AGENT_CD") = agentEntity.AgentCd.Value
            grd.Item(grd.Row, "AGENT_NAME") = agentEntity.AgentName.Value
            grd.Item(grd.Row, "colAgentCdChangeFlg") = True

        Catch ex As OracleException

            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString())
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Catch ex As Exception

            createFactoryMsg().messageDisp("E90_046", ex.Message)
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Finally

            ' 後処理
            Call MyBase.comPostEvent()
        End Try

    End Sub

    ''' <summary>
    ''' コースグリッドのチェックボックスの値が変更イベント
    ''' </summary>
    ''' <param name="sender">イベント送信元</param>
    ''' <param name="e">イベントデータ</param>
    Private Sub grdCrs_AfterEdit(ByVal sender As System.Object, ByVal e As C1.Win.C1FlexGrid.RowColEventArgs) Handles grdCrs.AfterEdit

        Try
            '前処理
            MyBase.comPreEvent()

            'エラーのクリア
            Me.errorClear()

            Dim grd As FlexGridEx = TryCast(sender, FlexGridEx)
            Dim checkedAlreadyJyosyaYoyakuNoArray() As String = Nothing
            Dim checkedNoShowYoyakuNoArray() As String = Nothing
            Dim inputAgentYoyakuNoArray(,) As String = Nothing
            Dim checkedInquiryYoyakuNoArray() As String = Nothing

            _releaseFlg = False

            'チェックボックスが解除されていた場合、解除フラグをTrueに設定
            If CBool(grd.Item(grd.Row, "colSelection")) = False Then
                _releaseFlg = True
            End If

            '変更があった行の予約番号を保持する
            ReDim checkedAlreadyJyosyaYoyakuNoArray(grdYoyaku.Rows.Count - 1)   '乗車済み
            ReDim checkedNoShowYoyakuNoArray(grdYoyaku.Rows.Count - 1)          'NoShow
            ReDim inputAgentYoyakuNoArray(grdYoyaku.Rows.Count - 1, 2)          '代理店コード、代理店名
            ReDim checkedInquiryYoyakuNoArray(grdYoyaku.Rows.Count - 1)         '請求対象
            If grdYoyaku.Rows.Count - 1 > 0 Then
                For ii = 1 To grdYoyaku.Rows.Count - 1 Step 1
                    '乗車済み
                    If CBool(grdYoyaku.Rows(ii).Item("colJyosyaAlready")) = True Then
                        checkedAlreadyJyosyaYoyakuNoArray(ii) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString
                    Else
                        checkedAlreadyJyosyaYoyakuNoArray(ii) = " "
                    End If

                    'NO SHOW
                    If CBool(grdYoyaku.Rows(ii).Item("colNOSHOW")) = True Then
                        checkedNoShowYoyakuNoArray(ii) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString
                    Else
                        checkedNoShowYoyakuNoArray(ii) = " "
                    End If

                    '代理店
                    If Not String.IsNullOrEmpty(grdYoyaku.Rows(ii).Item("AGENT_CD").ToString) = True Then
                        inputAgentYoyakuNoArray(ii, 0) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString
                        inputAgentYoyakuNoArray(ii, 1) = grdYoyaku.Rows(ii).Item("AGENT_CD").ToString
                        inputAgentYoyakuNoArray(ii, 2) = grdYoyaku.Rows(ii).Item("AGENT_NAME").ToString
                    Else
                        inputAgentYoyakuNoArray(ii, 0) = " "
                        inputAgentYoyakuNoArray(ii, 1) = " "
                        inputAgentYoyakuNoArray(ii, 2) = " "
                    End If

                    '請求対象
                    If CBool(grdYoyaku.Rows(ii).Item("colInquiry")) = True Then
                        checkedInquiryYoyakuNoArray(ii) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString
                    Else
                        checkedInquiryYoyakuNoArray(ii) = " "
                    End If
                Next
            End If

            '予約情報取得処理
            Me.setYoyakuInfo()

            '新しく表示された予約と前回チェックされていた予約が等しい場合、該当列のチェックボックスをONにする
            If grdYoyaku.Rows.Count - 1 > 0 Then
                For ii = 1 To grdYoyaku.Rows.Count - 1 Step 1
                    '乗車済み
                    If checkedAlreadyJyosyaYoyakuNoArray.Count > 0 Then
                        For ArrayNum01 = 1 To checkedAlreadyJyosyaYoyakuNoArray.Count - 1 Step 1
                            '乗車済みにチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            If checkedAlreadyJyosyaYoyakuNoArray(ArrayNum01) IsNot Nothing AndAlso
                                checkedAlreadyJyosyaYoyakuNoArray(ArrayNum01) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString Then
                                '等しい場合は、乗車済みチェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colJyosyaAlready") = True
                                Exit For
                            End If
                        Next
                    End If

                    'NOSHOW
                    If checkedNoShowYoyakuNoArray.Count > 0 Then
                        For ArrayNum02 = 1 To checkedNoShowYoyakuNoArray.Count - 1 Step 1
                            'NO SHOWにチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            If checkedNoShowYoyakuNoArray(ArrayNum02) IsNot Nothing AndAlso
                                checkedNoShowYoyakuNoArray(ArrayNum02) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString Then
                                '等しい場合は、NO SHOWチェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colNOSHOW") = True
                                Exit For
                            End If
                        Next
                    End If

                    '代理店
                    If inputAgentYoyakuNoArray.GetLength(0) > 0 Then
                        For ArrayNum03 = 1 To inputAgentYoyakuNoArray.GetLength(0) - 1 Step 1
                            '代理店に入力されていた行の予約番号と表示された予約一覧の予約番号を比較
                            If inputAgentYoyakuNoArray(ArrayNum03, 0) IsNot Nothing AndAlso
                                inputAgentYoyakuNoArray(ArrayNum03, 0) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString Then
                                '等しい場合は、代理店コードと代理店名に前回値を表示してループを抜ける
                                grdYoyaku.Rows(ii).Item("AGENT_CD") = inputAgentYoyakuNoArray(ArrayNum03, 1)
                                grdYoyaku.Rows(ii).Item("AGENT_NAME") = inputAgentYoyakuNoArray(ArrayNum03, 2)
                                Exit For
                            End If
                        Next
                    End If

                    '請求対象
                    If checkedInquiryYoyakuNoArray.Count > 0 Then
                        For ArrayNum04 = 1 To checkedInquiryYoyakuNoArray.Count - 1 Step 1
                            '請求対象にチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            If checkedInquiryYoyakuNoArray(ArrayNum04) IsNot Nothing AndAlso
                                checkedInquiryYoyakuNoArray(ArrayNum04) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString Then
                                '等しい場合は、請求対象チェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colInquiry") = True
                                Exit For
                            End If
                        Next
                    End If
                Next
            End If

            '予約グリッドのスクロールバー位置を左上に設定
            grdYoyaku.TopRow = 0
            grdYoyaku.LeftCol = 0

            ' log出力
            createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理")

        Catch ex As OracleException

            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString())
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Catch ex As Exception

            createFactoryMsg().messageDisp("E90_046", ex.Message)
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Finally

            ' 後処理
            Call MyBase.comPostEvent()
        End Try

    End Sub

    ''' <summary>
    ''' 予約グリッドのチェックボックスの値が変更イベント
    ''' </summary>
    ''' <param name="sender">イベント送信元</param>
    ''' <param name="e">イベントデータ</param>
    Private Sub grdYoyaku_AfterEdit(ByVal sender As System.Object, ByVal e As C1.Win.C1FlexGrid.RowColEventArgs) Handles grdYoyaku.AfterEdit

        Try
            '前処理
            MyBase.comPreEvent()

            'エラーのクリア
            Me.errorClear()

            Dim grd As FlexGridEx = TryCast(sender, FlexGridEx)
            Dim selectedCd As String = TryCast(grd.Item(grd.Row, grd.Col), String)
            Dim selectedColumnName As String = grd.Cols(grd.Col).Name

            Select Case selectedColumnName
                Case NameColJyosyaAlready, NameColNOSHOW
                    '乗車済み、NOSHOWが変更された場合

                    Dim s04_0103Da As New S04_0103Da
                    Dim yoyakuNo As New Hashtable
                    Dim usingFlgTable As New DataTable
                    Dim selectedColumn As String = ""
                    Dim notSelectedColumn As String = ""
                    Dim tableNum As Integer = grd.Row - 1

                    If selectedColumnName.Equals(NameColJyosyaAlready) Then
                        selectedColumn = NameColJyosyaAlready
                        notSelectedColumn = NameColNOSHOW
                    Else
                        selectedColumn = NameColNOSHOW
                        notSelectedColumn = NameColJyosyaAlready
                    End If

                    '企画の場合、「乗車済」チェックボックスは選択できない。
                    If CInt(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) = CInt(FixedCd.CrsKindType.higaeri) OrElse
                        CInt(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) = CInt(FixedCd.CrsKindType.stay) OrElse
                            CInt(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) = CInt(FixedCd.CrsKindType.rcourse) Then
                        If selectedColumn.Equals(NameColJyosyaAlready) Then

                            'メッセージ出力（企画の場合は乗車済を選択できません。）
                            CommonProcess.createFactoryMsg().messageDisp("E90_006", "企画の場合", "乗車済を選択")

                            grd.Item(grd.Row, $"{selectedColumn}") = False

                            Exit Sub
                        End If
                    End If

                    If CBool(grd.Item(grd.Row, $"{selectedColumn}")) = True Then
                        'チェックボックスがONの場合

                        '選択したカラムではないカラムのチェックボックスがONの場合
                        If CBool(grd.Item(grd.Row, $"{notSelectedColumn}")) = True Then

                            'メッセージ出力（乗車済みとNOSHOWは同時選択不可です。）
                            CommonProcess.createFactoryMsg().messageDisp("E90_072", "乗車済みとNOSHOW")

                            grd.Item(grd.Row, $"{selectedColumn}") = False

                            Exit Sub
                        End If

                        Dim yoyakuKbnNo As String = grd.Item(grd.Row, "colYoyakuNo").ToString.Replace(",", "")

                        yoyakuNo("YoyakuKbn") = yoyakuKbnNo.Substring(0, 1)
                        yoyakuNo("YoyakuNo") = yoyakuKbnNo.Substring(1, yoyakuKbnNo.Length - 1)

                        usingFlgTable = s04_0103Da.getUsingFlg(yoyakuNo)

                        '予約情報が使用中の場合
                        If usingFlgTable.Rows.Count > 0 AndAlso
                            usingFlgTable.Rows(0)("USING_FLG").ToString.Trim.Equals(FixedCd.UsingFlg.Use) Then

                            'メッセージ出力（該当予約は予約で使用中です。確認後再度実行してください。）
                            CommonProcess.createFactoryMsg().messageDisp("E90_040")

                            grd.Item(grd.Row, $"{selectedColumn}") = False

                            Exit Sub
                        End If

                        '発券済みチェック
                        If CInt(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) = CInt(FixedCd.CrsKindType.hatoBusTeiki) AndAlso
                            Not _yoyakuInfoTable.Rows(tableNum)("STATE").ToString.Trim.Equals(StateJTB) AndAlso
                               Not _yoyakuInfoTable.Rows(tableNum)("STATE").ToString.Trim.Equals(StateKNT) Then

                            If String.IsNullOrEmpty(_yoyakuInfoTable.Rows(tableNum)("HAKKEN_NAIYO").ToString) Then

                                'メッセージ出力（指定された予約は未だ発券されていません。）
                                CommonProcess.createFactoryMsg().messageDisp("E02_051")

                                grd.Item(grd.Row, $"{selectedColumn}") = False

                                Exit Sub
                            End If
                        End If

                        'サーバー日付取得
                        Dim sysdates As Hashtable = Me.getSysDates()

                        If CInt(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) = CInt(FixedCd.CrsKindType.higaeri) OrElse
                            CInt(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) = CInt(FixedCd.CrsKindType.stay) OrElse
                            CInt(_yoyakuInfoTable.Rows(tableNum)("CRS_KIND")) = CInt(FixedCd.CrsKindType.rcourse) AndAlso
                             Not _yoyakuInfoTable.Rows(tableNum)("STATE").ToString.Trim.Equals(StateJTB) AndAlso
                               Not _yoyakuInfoTable.Rows(tableNum)("STATE").ToString.Trim.Equals(StateKNT) Then

                            'TODO:場所はここで良いのか？（検索時か？）
                            '企画日付チェック
                            Dim entryDate As String = _yoyakuInfoTable.Rows(tableNum)("ENTRY_DAY").ToString
                            Dim parseEntryDate As Date = Date.ParseExact(entryDate, "yyyyMMdd", Nothing)
                            If CommonDateUtil.IsPastDate(parseEntryDate) = True Then
                                'メッセージ出力（前月１日より前の日付を指定することはできません。）
                                CommonProcess.createFactoryMsg().messageDisp("E03_001", "前月１日")

                                grd.Item(grd.Row, $"{selectedColumn}") = False

                                Exit Sub
                            End If

                            '企画旅行契約チェック
                            If Not _yoyakuInfoTable(tableNum)("KIKAKU_KEIYAKU_KBN") Is DBNull.Value AndAlso
                                CInt(_yoyakuInfoTable.Rows(tableNum)("KIKAKU_KEIYAKU_KBN")) <> CInt(FixedCd.DairitenKeiyakuKbnType.Kekyaku) Then
                                'メッセージ出力（企画旅行契約が結ばれていない業者です。よろしいですか？）
                                If CommonProcess.createFactoryMsg().messageDisp("Q02_002") = MsgBoxResult.Cancel Then
                                    'キャンセルの場合、チェックOFF
                                    grd.Item(grd.Row, $"{selectedColumn}") = False
                                Else
                                    'キャンセルの場合、チェックOFF
                                    grd.Item(grd.Row, $"{selectedColumn}") = True
                                End If
                            End If
                        End If
                    End If

                    grd.Item(grd.Row, "colChangeFlg") = True

                Case NameColInquiry
                    '請求対象が変更された場合

                    grd.Item(grd.Row, "colInquiryChangeFlg") = True

            End Select

        Catch ex As OracleException

            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString())
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Catch ex As Exception

            createFactoryMsg().messageDisp("E90_046", ex.Message)
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Finally

            ' 後処理
            Call MyBase.comPostEvent()
        End Try

    End Sub

    ''' <summary>
    ''' コースグリッド全選択
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnAllSelectionCrs_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAllSelectionCrs.Click

        Dim checkedAlreadyJyosyaYoyakuNoArray() As String = Nothing
        Dim checkedNoShowYoyakuNoArray() As String = Nothing
        Dim inputAgentYoyakuNoArray(,) As String = Nothing
        Dim checkedInquiryYoyakuNoArray() As String = Nothing

        Try
            '前処理
            MyBase.comPreEvent()

            'データテーブルがNothingだった場合
            If _crsInfoTable Is Nothing Then
                Exit Sub
            End If

            'エラーのクリア
            Me.errorClear()

            _releaseFlg = False

            'チェックボックスを全選択
            For Each dataRow As DataRow In _crsInfoTable.Rows
                dataRow("colSelection") = True
            Next

            '変更があった行の予約番号を保持する
            ReDim checkedAlreadyJyosyaYoyakuNoArray(grdYoyaku.Rows.Count - 1)   '乗車済み
            ReDim checkedNoShowYoyakuNoArray(grdYoyaku.Rows.Count - 1)          'NoShow
            ReDim inputAgentYoyakuNoArray(grdYoyaku.Rows.Count - 1, 2)          '代理店コード、代理店名
            ReDim checkedInquiryYoyakuNoArray(grdYoyaku.Rows.Count - 1)         '請求対象
            If grdYoyaku.Rows.Count - 1 > 0 Then
                For ii = 1 To grdYoyaku.Rows.Count - 1 Step 1
                    '乗車済み
                    If CBool(grdYoyaku.Rows(ii).Item("colJyosyaAlready")) = True Then
                        checkedAlreadyJyosyaYoyakuNoArray(ii) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString
                    Else
                        checkedAlreadyJyosyaYoyakuNoArray(ii) = " "
                    End If

                    'NO SHOW
                    If CBool(grdYoyaku.Rows(ii).Item("colNOSHOW")) = True Then
                        checkedNoShowYoyakuNoArray(ii) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString
                    Else
                        checkedNoShowYoyakuNoArray(ii) = " "
                    End If

                    '代理店
                    If Not String.IsNullOrEmpty(grdYoyaku.Rows(ii).Item("AGENT_CD").ToString) = True Then
                        inputAgentYoyakuNoArray(ii, 0) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString
                        inputAgentYoyakuNoArray(ii, 1) = grdYoyaku.Rows(ii).Item("AGENT_CD").ToString
                        inputAgentYoyakuNoArray(ii, 2) = grdYoyaku.Rows(ii).Item("AGENT_NAME").ToString
                    Else
                        inputAgentYoyakuNoArray(ii, 0) = " "
                        inputAgentYoyakuNoArray(ii, 1) = " "
                        inputAgentYoyakuNoArray(ii, 2) = " "
                    End If

                    '請求対象
                    If CBool(grdYoyaku.Rows(ii).Item("colInquiry")) = True Then
                        checkedInquiryYoyakuNoArray(ii) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString
                    Else
                        checkedInquiryYoyakuNoArray(ii) = " "
                    End If
                Next
            End If

            '予約情報取得処理
            Me.setYoyakuInfo()

            '新しく表示された予約と前回チェックされていた予約が等しい場合、該当列のチェックボックスをONにする
            If grdYoyaku.Rows.Count - 1 > 0 Then
                For ii = 1 To grdYoyaku.Rows.Count - 1 Step 1
                    '乗車済み
                    If checkedAlreadyJyosyaYoyakuNoArray.Count > 0 Then
                        For ArrayNum01 = 1 To checkedAlreadyJyosyaYoyakuNoArray.Count - 1 Step 1
                            '乗車済みにチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            If checkedAlreadyJyosyaYoyakuNoArray(ArrayNum01) IsNot Nothing AndAlso
                                checkedAlreadyJyosyaYoyakuNoArray(ArrayNum01) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString Then
                                '等しい場合は、乗車済みチェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colJyosyaAlready") = True
                                Exit For
                            End If
                        Next
                    End If

                    'NOSHOW
                    If checkedNoShowYoyakuNoArray.Count > 0 Then
                        For ArrayNum02 = 1 To checkedNoShowYoyakuNoArray.Count - 1 Step 1
                            'NO SHOWにチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            If checkedNoShowYoyakuNoArray(ArrayNum02) IsNot Nothing AndAlso
                                checkedNoShowYoyakuNoArray(ArrayNum02) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString Then
                                '等しい場合は、NO SHOWチェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colNOSHOW") = True
                                Exit For
                            End If
                        Next
                    End If

                    '代理店
                    If inputAgentYoyakuNoArray.GetLength(0) > 0 Then
                        For ArrayNum03 = 1 To inputAgentYoyakuNoArray.GetLength(0) - 1 Step 1
                            '代理店に入力されていた行の予約番号と表示された予約一覧の予約番号を比較
                            If inputAgentYoyakuNoArray(ArrayNum03, 0) IsNot Nothing AndAlso
                                inputAgentYoyakuNoArray(ArrayNum03, 0) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString Then
                                '等しい場合は、代理店コードと代理店名に前回値を表示してループを抜ける
                                grdYoyaku.Rows(ii).Item("AGENT_CD") = inputAgentYoyakuNoArray(ArrayNum03, 1)
                                grdYoyaku.Rows(ii).Item("AGENT_NAME") = inputAgentYoyakuNoArray(ArrayNum03, 2)
                                Exit For
                            End If
                        Next
                    End If

                    '請求対象
                    If checkedInquiryYoyakuNoArray.Count > 0 Then
                        For ArrayNum04 = 1 To checkedInquiryYoyakuNoArray.Count - 1 Step 1
                            '請求対象にチェックされていた行の予約番号と表示された予約一覧の予約番号を比較
                            If checkedInquiryYoyakuNoArray(ArrayNum04) IsNot Nothing AndAlso
                                checkedInquiryYoyakuNoArray(ArrayNum04) = grdYoyaku.Rows(ii).Item("colYoyakuNo").ToString Then
                                '等しい場合は、請求対象チェックボックスをONにしてループを抜ける
                                grdYoyaku.Rows(ii).Item("colInquiry") = True
                                Exit For
                            End If
                        Next
                    End If
                Next
            End If

            ' log出力
            createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理")

        Catch ex As OracleException

            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString())
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Catch ex As Exception

            createFactoryMsg().messageDisp("E90_046", ex.Message)
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Finally

            ' 後処理
            Call MyBase.comPostEvent()
        End Try

    End Sub

    ''' <summary>
    ''' コースグリッド全解除
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnAllReleaseCrs_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAllReleaseCrs.Click

        Try
            '前処理
            MyBase.comPreEvent()

            'データテーブルがNothingだった場合
            If _crsInfoTable Is Nothing Then
                Exit Sub
            End If

            'エラーのクリア
            Me.errorClear()

            'チェックボックスを全解除
            For Each dataRow As DataRow In _crsInfoTable.Rows
                dataRow("colSelection") = False
            Next

            _yoyakuInfoTable = New DataTable

            grdYoyaku.DataSource = _yoyakuInfoTable

        Catch ex As OracleException

            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString())
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Catch ex As Exception

            createFactoryMsg().messageDisp("E90_046", ex.Message)
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Finally

            ' 後処理
            Call MyBase.comPostEvent()
        End Try

    End Sub

    ''' <summary>
    ''' 予約グリッド全選択
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnAllSelectionYoyaku_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAllSelectionYoyaku.Click

        Try
            '前処理
            MyBase.comPreEvent()

            'データテーブルがNothingだった場合
            If _yoyakuInfoTable Is Nothing Then
                Exit Sub
            End If

            'エラーのクリア
            Me.errorClear()

            'チェックボックスを全選択
            For Each dataRow As DataRow In _yoyakuInfoTable.Rows
                If CBool(dataRow("colJyosyaAlready")) = False Then

                    '企画の場合、「乗車済」チェックボックスは選択できない。
                    If CInt(dataRow("CRS_KIND")) = CInt(FixedCd.CrsKindType.higaeri) OrElse
                            CInt(dataRow("CRS_KIND")) = CInt(FixedCd.CrsKindType.stay) OrElse
                                CInt(dataRow("CRS_KIND")) = CInt(FixedCd.CrsKindType.rcourse) Then

                        ''メッセージ出力（企画の場合は乗車済を選択できません。）
                        'CommonProcess.createFactoryMsg().messageDisp("E90_006", "企画の場合", "乗車済を選択")

                        dataRow("colJyosyaAlready") = False

                        Continue For
                    End If
                End If

                '選択したカラムではないカラムのチェックボックスがONの場合
                If CBool(dataRow("colNOSHOW")) = True Then

                    ''メッセージ出力（乗車済みとNOSHOWは同時選択不可です。）
                    'CommonProcess.createFactoryMsg().messageDisp("E90_072", "乗車済みとNOSHOW")

                    dataRow("colJyosyaAlready") = False

                    Continue For
                End If

                Dim yoyakuNo As New Hashtable
                Dim s04_0103Da As New S04_0103Da
                yoyakuNo("YoyakuKbn") = dataRow("YOYAKU_KBN")
                yoyakuNo("YoyakuNo") = dataRow("YOYAKU_NO")

                Dim usingFlgTable As DataTable = S04_0103Da.getUsingFlg(yoyakuNo)

                '予約情報が使用中の場合
                If usingFlgTable.Rows.Count > 0 AndAlso
                                usingFlgTable.Rows(0)("USING_FLG").ToString.Trim.Equals(FixedCd.UsingFlg.Use) Then

                    ''メッセージ出力（該当予約は予約で使用中です。確認後再度実行してください。）
                    'CommonProcess.createFactoryMsg().messageDisp("E90_040")

                    dataRow("colJyosyaAlready") = False

                    Continue For
                End If

                '発券済みチェック
                If CInt(dataRow("CRS_KIND")) = CInt(FixedCd.CrsKindType.hatoBusTeiki) AndAlso
                                Not dataRow("STATE").ToString.Trim.Equals(StateJTB) AndAlso
                                   Not dataRow("STATE").ToString.Trim.Equals(StateKNT) Then

                    If String.IsNullOrEmpty(dataRow("HAKKEN_NAIYO").ToString) Then

                        ''メッセージ出力（指定された予約は未だ発券されていません。）
                        'CommonProcess.createFactoryMsg().messageDisp("E02_051")

                        dataRow("colJyosyaAlready") = False

                        Continue For
                    End If
                End If

                'サーバー日付取得
                Dim sysdates As Hashtable = Me.getSysDates()

                If CInt(dataRow("CRS_KIND")) = CInt(FixedCd.CrsKindType.higaeri) OrElse
                                CInt(dataRow("CRS_KIND")) = CInt(FixedCd.CrsKindType.stay) AndAlso
                                 Not dataRow("STATE").ToString.Trim.Equals(StateJTB) AndAlso
                                   Not dataRow("STATE").ToString.Trim.Equals(StateKNT) Then

                    '企画日付チェック
                    If CInt(dataRow("HAKKEN_DAY")) < CInt(DirectCast(sysdates(KeyDtSysDate), Date).AddMonths(BfMon).AddDays(OneDayBf).ToString("yyMMdd")) Then
                        ''メッセージ出力（前月１日より前の日付を指定することはできません。）
                        'CommonProcess.createFactoryMsg().messageDisp("E03_001", "前月１日")

                        dataRow("colJyosyaAlready") = False

                        Continue For
                    End If

                    '企画旅行契約チェック
                    If Not dataRow("KIKAKU_ATO_SEISAN_KBN") Is DBNull.Value AndAlso
                                    CInt(dataRow("KIKAKU_ATO_SEISAN_KBN")) <> CInt(FixedCd.DairitenKeiyakuKbnType.Kekyaku) Then
                        ''メッセージ出力（企画旅行契約が結ばれていない業者です。よろしいですか？）
                        'CommonProcess.createFactoryMsg().messageDisp("Q02_002")

                        dataRow("colJyosyaAlready") = False

                        Continue For
                    End If
                End If

                dataRow("colJyosyaAlready") = True
                dataRow("colChangeFlg") = True
            Next

        Catch ex As OracleException

            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString())
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Catch ex As Exception

            createFactoryMsg().messageDisp("E90_046", ex.Message)
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Finally

            ' 後処理
            Call MyBase.comPostEvent()
        End Try

    End Sub

    ''' <summary>
    ''' 予約グリッド全解除
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnAllReleaseYoyaku_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAllReleaseYoyaku.Click

        Try
            '前処理
            MyBase.comPreEvent()

            'データテーブルがNothingだった場合
            If _yoyakuInfoTable Is Nothing Then
                Exit Sub
            End If

            'エラーのクリア
            Me.errorClear()

            'チェックボックスを全解除
            For Each dataRow As DataRow In _yoyakuInfoTable.Rows
                If CBool(dataRow("colJyosyaAlready")) = True Then
                    dataRow("colJyosyaAlready") = False
                    dataRow("colChangeFlg") = True
                End If
            Next

        Catch ex As OracleException

            createFactoryMsg.messageDisp("E90_018", ex.Number.ToString())
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Catch ex As Exception

            createFactoryMsg().messageDisp("E90_046", ex.Message)
            Dim strErr() As String
            strErr = New String() {ex.Message, ex.Source, ex.StackTrace}
            createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, MyBase.setFormId & ":" & MyBase.setTitle, strErr)
            Me.Close()
        Finally

            ' 後処理
            Call MyBase.comPostEvent()
        End Try


    End Sub

    ''' <summary>
    ''' グリッドのデータソース変更時イベント
    ''' </summary>
    ''' <param name="sender">イベント送信元</param>
    ''' <param name="e">イベントデータ</param>
    Private Sub grdCrs_AfterDataRefresh(ByVal sender As Object, ByVal e As ListChangedEventArgs) Handles grdCrs.AfterDataRefresh
        'データ件数を表示(ヘッダー行分マイナス1)
        Dim formatedCount As String = (Me.grdCrs.Rows.Count - 1).ToString.PadLeft(6)
        Me.lblLengthGrd.Text = formatedCount + Ken
    End Sub
#End Region

#Region "メソッド"

    ''' <summary>
    ''' 画面初期化
    ''' </summary>
    Private Sub setControlInitiarize()

        'ベースフォームの設定
        Me.setFormId = PgmId
        Me.setTitle = ScreenName

        ' フッタボタンの設定
        Me.setButtonInitiarize()

        '検索項目の設定
        Me.setSearchKoumoku()

        'コースグリッドの初期化
        Me.setGrdCrsInitiarize()

        '予約グリッドの初期化
        Me.setGrdYoyakuInitiarize()

        'エラーのクリア
        Me.errorClear()

        '変数の初期化
        Me.setVariableInitial()

        '検索条件を表示状態のGroupAreaのレイアウトを保存
        Me.saveGrpLayout()

        'フォーカス設定
        Me.ActiveControl = Me.dtmSyuptDay

    End Sub

    ''' <summary>
    ''' フッタボタンの設定
    ''' </summary>
    Private Sub setButtonInitiarize()

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
        Me.F10Key_Visible = True
        Me.F11Key_Visible = False
        Me.F12Key_Visible = False

        'Text
        Me.F2Key_Text = "F2:戻る"
        Me.F10Key_Text = "F10:登録"
    End Sub

    ''' <summary>
    ''' コースグリッドの初期化
    ''' </summary>
    Private Sub setGrdCrsInitiarize()
        'グリッド部の初期表示
        Dim dt As New DataTable

        Me.grdCrs.DataSource = dt
        Me.grdCrs.DataMember = ""
        Me.grdCrs.Refresh()

        Me.lblLengthGrd.Text = ZeroKen.PadLeft(7)
    End Sub

    ''' <summary>
    ''' 予約グリッドの初期化
    ''' </summary>
    Private Sub setGrdYoyakuInitiarize()
        'グリッド部の初期表示
        Dim dt As New DataTable

        Me.grdYoyaku.DataSource = dt
        Me.grdYoyaku.DataMember = ""
        Me.grdYoyaku.Refresh()

    End Sub

    ''' <summary>
    ''' 変数の初期化
    ''' </summary>
    Private Sub setVariableInitial()
        _searchForTable = New Hashtable
        _yoyakuInfoTable = Nothing
        _crsInfoTable = Nothing
        _judgmentForList = New Hashtable
        _seisanInfoEntity = New TSeisanInfoEntity
        _yoyakuInfoBasicEntity = New YoyakuInfoBasicEntity
        _releaseFlg = False
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

    ''' <summary>
    ''' 入力項目のチェック
    ''' </summary>
    Private Function isInputCheck() As Boolean

        '日付の入力チェック
        If dtmSyuptDay.Text.Equals(DayInitialValue) Then

            'メッセージ出力（日付を入力してください。）
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "日付")

            'エラー項目を赤く表示
            Me.dtmSyuptDay.ExistError = True

            'フォーカス設定
            Me.ActiveControl = Me.dtmSyuptDay

            Return False
        End If

        Dim checkDate As Date = Nothing

        '日付の値チェック
        If Not DateTime.TryParse(dtmSyuptDay.Text, checkDate) Then

            'メッセージ出力（日付の値が不正です。）
            CommonProcess.createFactoryMsg().messageDisp("E90_016", "日付")

            'エラー項目を赤く表示
            Me.dtmSyuptDay.ExistError = True

            'フォーカス設定
            Me.ActiveControl = Me.dtmSyuptDay

            Return False
        End If

        '乗車地の入力チェック
        If String.IsNullOrEmpty(Me.ucoNoribaCd.CodeText) Then

            'メッセージ出力（乗車地を入力してください。）
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "乗車地")

            'エラー項目を赤く表示
            Me.ucoNoribaCd.ExistError = True

            'フォーカス設定
            Me.ActiveControl = Me.ucoNoribaCd

            Return False
        End If

        'コース種別１の入力チェック
        If Me.chkCrsKind1Japanese.Checked = False AndAlso
            Me.chkCrsKind1Gaikokugo.Checked = False Then

            'メッセージ出力（コース種別１を入力してください。）
            CommonProcess.createFactoryMsg().messageDisp("E90_011", "コース種別１")

            'エラー項目を赤く表示
            Me.chkCrsKind1Japanese.ExistError = True
            Me.chkCrsKind1Gaikokugo.ExistError = True

            'フォーカス設定
            Me.ActiveControl = Me.chkCrsKind1Japanese
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' 入力項目のチェック(予約)
    ''' </summary>
    Private Sub isInputCheckYoyaku()

        Dim checkDate As Date = Nothing

        '出発時刻の値が不正
        If Not Me.ucoSyuptTimeFromTo.FromTimeText Is Nothing AndAlso
            DateTime.TryParse(Me.ucoSyuptTimeFromTo.FromTimeText.ToString, checkDate) = False Then

            'メッセージ出力（出発時間の値が不正です。）
            CommonProcess.createFactoryMsg().messageDisp("E90_016", "出発時間")

            'エラー項目を赤く表示
            Me.ucoSyuptTimeFromTo.ExistErrorForFromTime = True

            'フォーカス設定
            Me.ActiveControl = Me.ucoSyuptTimeFromTo

            Exit Sub
        End If

        If Not Me.ucoSyuptTimeFromTo.ToTimeText Is Nothing AndAlso
            DateTime.TryParse(Me.ucoSyuptTimeFromTo.ToTimeText.ToString, checkDate) = False Then

            'メッセージ出力（出発時間の値が不正です。）
            CommonProcess.createFactoryMsg().messageDisp("E90_016", "出発時間")

            'エラー項目を赤く表示
            Me.ucoSyuptTimeFromTo.ExistErrorForToTime = True

            'フォーカス設定
            Me.ActiveControl = Me.ucoSyuptTimeFromTo

            Exit Sub

        End If

    End Sub

    ''' <summary>
    ''' 更新時のチェック処理
    ''' </summary>
    Private Sub isUpdateCheck()
        'TODO:グリッド変更時に記述したチェック処理はこっちか？（こっちでは必要なし？）
    End Sub

    ''' <summary>
    ''' 検索用の値を格納
    ''' </summary>
    Private Sub setSerchForValue()

        Dim checkDate As Date = Nothing        'チェック用
        Dim yoyakuNo As New Hashtable     '予約番号

        _searchForTable = New Hashtable

        '出発日
        _searchForTable("SyuptDay") = CDate(Me.dtmSyuptDay.Text).ToString("yyyyMMdd")
        '乗車地コード
        _searchForTable("JyochachiCd") = Me.ucoNoribaCd.CodeText
        'コース種別1
        If chkCrsKind1Japanese.Checked = True Then
            _searchForTable("Japanese") = True
        Else
            _searchForTable("Japanese") = False
        End If
        If chkCrsKind1Gaikokugo.Checked = True Then
            _searchForTable("Gaikokugo") = True
        Else
            _searchForTable("Gaikokugo") = False
        End If
        'コース種別2
        If chkCrsKind2Teiki.Checked = True Then
            _searchForTable("Teiki") = True
        Else
            _searchForTable("Teiki") = False
        End If
        If chkCrsKind2Kikaku.Checked = True Then
            _searchForTable("Kikaku") = True
        Else
            _searchForTable("Kikaku") = False
        End If
        '出発時間
        If DateTime.TryParse(ucoSyuptTimeFromTo.FromTimeText.ToString, checkDate) Then
            _searchForTable("SyuptTimeFrom") = CDate(ucoSyuptTimeFromTo.FromTimeText.ToString).ToString("HHmm")
        Else
            _searchForTable("SyuptTimeFrom") = ""
        End If
        If DateTime.TryParse(ucoSyuptTimeFromTo.ToTimeText.ToString, checkDate) Then
            _searchForTable("SyuptTimeTo") = CDate(ucoSyuptTimeFromTo.ToTimeText.ToString).ToString("HHmm")
        Else
            _searchForTable("SyuptTimeTo") = ""
        End If
        '予約区分と予約番号
        yoyakuNo = Me.getYoyakuNo()
        If Not String.IsNullOrEmpty(yoyakuNo("yoyakuNo").ToString) Then
            _searchForTable("YoyakuKbn") = yoyakuNo("yoyakuKbn")
            _searchForTable("YoyakuNo") = yoyakuNo("yoyakuNo")
        Else
            _searchForTable("YoyakuKbn") = ""
            _searchForTable("YoyakuNo") = ""
        End If
        '予約者名
        _searchForTable("Surname") = Me.txtYoyakuPersonSurname.Text & "%"
        _searchForTable("Name") = Me.txtYoyakuPersonName.Text & "%"
        '処理日
        If DateTime.TryParse(dtmProcessDay.Text.ToString, checkDate) Then
            _searchForTable("ProcessDay") = CDate(dtmProcessDay.Text).ToString("yyyyMMdd")
        Else
            _searchForTable("ProcessDay") = ""
        End If
        'ユーザーID
        _searchForTable("UserId") = Me.txtUserId.Text
        '営業所コード
        _searchForTable("EigyosyoCd") = Me.ucoEigyosyoCd.CodeText
        '代理店コード
        _searchForTable("DairitenCd") = Me.ucoDairitenCd.CodeText
        'コースコード
        _searchForTable("CrsCd") = Me.ucoCrsCd.CodeText
        '乗車済み含む
        If chkJyosyaAlreadyWith.Checked = True Then
            _searchForTable("JyosyaAlready") = True
        Else
            _searchForTable("JyosyaAlready") = False
        End If
        '削除済み含む
        If chkDeleteAlreadyWith.Checked = True Then
            _searchForTable("DeleteAlreadyWith") = True
        Else
            _searchForTable("DeleteAlreadyWith") = False
        End If

    End Sub

    ''' <summary>
    ''' コース情報テーブルの取得
    ''' </summary>
    Private Sub getCrsInfoTable()

        Dim s04_0103Da As New S04_0103Da

        _crsInfoTable = s04_0103Da.getCrsInfoTable(_searchForTable)

    End Sub

    ''' <summary>
    ''' 予約情報テーブルの取得
    ''' </summary>
    Private Function getYoyakuInfoTable() As DataTable

        Dim s04_0103Da As New S04_0103Da

        Return s04_0103Da.getYoyakuInfoTable(_searchForTable)

    End Function

    ''' <summary>
    ''' エンティティに値を格納
    ''' </summary>
    Private Sub setEntityData(ByVal dataRow As DataRow, ByVal sysdates As Hashtable)

        '変数の初期化
        _yoyakuInfoBasicEntity = New YoyakuInfoBasicEntity
        _judgmentForList = New Hashtable

        'エンティティに値を格納
        _yoyakuInfoBasicEntity.updatePersonCd.Value = UserInfoManagement.userId
        _yoyakuInfoBasicEntity.updatePgmid.Value = PgmId
        _yoyakuInfoBasicEntity.updateDay.Value = DirectCast(sysdates(KeyIntSysDate), Integer?)
        _yoyakuInfoBasicEntity.updateTime.Value = DirectCast(sysdates(KeyIntSysTimeHhMmSs), Integer?)
        _yoyakuInfoBasicEntity.systemUpdatePersonCd.Value = UserInfoManagement.userId
        _yoyakuInfoBasicEntity.systemUpdatePgmid.Value = PgmId
        _yoyakuInfoBasicEntity.systemUpdateDay.Value = DirectCast(sysdates(KeyDtSysDate), Date)
        _yoyakuInfoBasicEntity.usingFlg.Value = FixedCd.UsingFlg.Unused
        _yoyakuInfoBasicEntity.yoyakuKbn.Value = dataRow("YOYAKU_KBN").ToString
        _yoyakuInfoBasicEntity.yoyakuNo.Value = CInt(dataRow("YOYAKU_NO"))
        _yoyakuInfoBasicEntity.crsCd.Value = dataRow("CRS_CD").ToString
        _yoyakuInfoBasicEntity.syuptDay.Value = CInt(dataRow("SYUPT_DAY"))
        _yoyakuInfoBasicEntity.gousya.Value = CInt(dataRow("GOUSYA"))
        _yoyakuInfoBasicEntity.name.Value = dataRow("NAME").ToString
        _yoyakuInfoBasicEntity.agentCd.Value = dataRow("AGENT_CD").ToString
        _yoyakuInfoBasicEntity.agentNm.Value = dataRow("AGENT_NAME").ToString
        _yoyakuInfoBasicEntity.cancelFlg.Value = dataRow("CANCEL_FLG").ToString

        _judgmentForList("JyosyaAlreadyNOSHOW") = False
        If CBool(dataRow("colJyosyaAlready")) = True Then
            '乗車済みにチェックがある場合
            _yoyakuInfoBasicEntity.checkinFlg1.Value = CheckinFlgJyosyaAlready
            _yoyakuInfoBasicEntity.noShowFlg.Value = ""
            _yoyakuInfoBasicEntity.usingFlg.Value = ""
            _judgmentForList("JyosyaAlreadyNOSHOW") = True

        ElseIf CBool(dataRow("colNOSHOW")) = True Then
            'NOSHOWにチェックがある場合
            _yoyakuInfoBasicEntity.checkinFlg1.Value = CheckinFlgNOSHOW
            _yoyakuInfoBasicEntity.noShowFlg.Value = NOSHOWFlg
            _yoyakuInfoBasicEntity.usingFlg.Value = ""
            _judgmentForList("JyosyaAlreadyNOSHOW") = True

        End If

        'チェックボックスに変化がなかった場合
        If CBool(dataRow("colChangeFlg")) = False Then
            _judgmentForList("JyosyaAlreadyNOSHOW") = False
        End If

        _judgmentForList("Dairiten") = False
        '代理店に変更があった場合
        If CBool(dataRow("colAgentCdChangeFlg")) = True Then
            _yoyakuInfoBasicEntity.agentCd.Value = dataRow("AGENT_CD").ToString
            _yoyakuInfoBasicEntity.agentNm.Value = dataRow("AGENT_NAME").ToString
            '業者名カナを取得
            Dim checkAgentCdParam As New Yoyaku.CheckAgentCdParam

            checkAgentCdParam.agentCd = dataRow("AGENT_CD").ToString
            checkAgentCdParam.crsCd = dataRow("CRS_CD").ToString

            '正常終了の場合
            If Yoyaku.YoyakuBizCommon.checkAgentCd(checkAgentCdParam) = 0 Then
                _yoyakuInfoBasicEntity.agentNameKana.Value = checkAgentCdParam.agentNameKana
            End If

            _seisanInfoEntity.UpdatePersonCd.Value = UserInfoManagement.userId
                _seisanInfoEntity.UpdatePgmid.Value = PgmId
                _seisanInfoEntity.UpdateDay.Value = DirectCast(sysdates(KeyIntSysDate), Integer?)
                _seisanInfoEntity.UpdateTime.Value = DirectCast(sysdates(KeyIntSysTimeHhMmSs), Integer?)
                _seisanInfoEntity.AgentCd.Value = dataRow("AGENT_CD").ToString
                _seisanInfoEntity.SystemUpdatePersonCd.Value = UserInfoManagement.userId
                _seisanInfoEntity.SystemUpdatePgmid.Value = PgmId
                _seisanInfoEntity.SystemUpdateDay.Value = DirectCast(sysdates(KeyDtSysDate), Date)
                _seisanInfoEntity.YoyakuKbn.Value = dataRow("YOYAKU_KBN").ToString
                _seisanInfoEntity.YoyakuNo.Value = CInt(dataRow("YOYAKU_NO"))

                _judgmentForList("Dairiten") = True
            End If

            _judgmentForList("Inquiry") = False
        '請求対象に変更があった場合
        If CBool(dataRow("colInquiryChangeFlg")) = True Then
            _seisanInfoEntity.UpdatePersonCd.Value = UserInfoManagement.userId
            _seisanInfoEntity.UpdatePgmid.Value = PgmId
            _seisanInfoEntity.UpdateDay.Value = DirectCast(sysdates(KeyIntSysDate), Integer?)
            _seisanInfoEntity.UpdateTime.Value = DirectCast(sysdates(KeyIntSysTimeHhMmSs), Integer?)
            _seisanInfoEntity.SystemUpdatePersonCd.Value = UserInfoManagement.userId
            _seisanInfoEntity.SystemUpdatePgmid.Value = PgmId
            _seisanInfoEntity.SystemUpdateDay.Value = DirectCast(sysdates(KeyDtSysDate), Date)
            _seisanInfoEntity.YoyakuKbn.Value = dataRow("YOYAKU_KBN").ToString
            _seisanInfoEntity.YoyakuNo.Value = CInt(dataRow("YOYAKU_NO"))
            If CBool(dataRow("colInquiry")) = True Then
                _seisanInfoEntity.AgtSeikyuTaisyoFlg.Value = FixedCd.AtoSeisanKbnType.ChakukenSeisan
            Else
                _seisanInfoEntity.AgtSeikyuTaisyoFlg.Value = FixedCd.AtoSeisanKbnType.HakkenSeisan
            End If

            _judgmentForList("Inquiry") = True
        End If

    End Sub

    ''' <summary>
    ''' 使用中フラグをUseに設定
    ''' </summary>
    Private Function executeUsingFlgUse(ByVal sysdates As Hashtable, ByVal dataRow As DataRow) As Boolean

        Dim s04_0103Da As New S04_0103Da

        _yoyakuInfoBasicEntity = New YoyakuInfoBasicEntity

        'エンティティに値を格納
        _yoyakuInfoBasicEntity.updatePersonCd.Value = UserInfoManagement.userId
        _yoyakuInfoBasicEntity.updatePgmid.Value = PgmId
        _yoyakuInfoBasicEntity.updateDay.Value = DirectCast(sysdates(KeyIntSysDate), Integer?)
        _yoyakuInfoBasicEntity.updateTime.Value = DirectCast(sysdates(KeyIntSysTimeHhMmSs), Integer?)
        _yoyakuInfoBasicEntity.systemUpdatePersonCd.Value = UserInfoManagement.userId
        _yoyakuInfoBasicEntity.systemUpdatePgmid.Value = PgmId
        _yoyakuInfoBasicEntity.systemUpdateDay.Value = DirectCast(sysdates(KeyDtSysDate), Date)
        _yoyakuInfoBasicEntity.usingFlg.Value = FixedCd.UsingFlg.Use
        _yoyakuInfoBasicEntity.yoyakuKbn.Value = dataRow("YOYAKU_KBN").ToString
        _yoyakuInfoBasicEntity.yoyakuNo.Value = CInt(dataRow("YOYAKU_NO"))

        Return s04_0103Da.executeUsingFlg(_yoyakuInfoBasicEntity)
    End Function

    ''' <summary>
    ''' 使用中フラグをUnusedに設定
    ''' </summary>
    Private Function executeUsingFlgUnused() As Boolean

        Dim s04_0103Da As New S04_0103Da

        'エンティティに値を格納
        _yoyakuInfoBasicEntity.usingFlg.Value = FixedCd.UsingFlg.Unused

        Return s04_0103Da.executeUsingFlg(_yoyakuInfoBasicEntity)

    End Function

    ''' <summary>
    ''' 予約と精算の更新
    ''' </summary>
    Private Function executeYoyakuSeisan(ByVal yoyakuInfoCrsChargeChargeKbnList As List(Of YoyakuInfoCrsChargeChargeKbnEntity)) As Boolean

        Dim s04_0103Da As New S04_0103Da

        Return s04_0103Da.executeYoyakuSeisan(_yoyakuInfoBasicEntity, _seisanInfoEntity, _judgmentForList, yoyakuInfoCrsChargeChargeKbnList)

    End Function

#Region "検索項目の設定"
    '検索項目の設定
    Private Sub setSearchKoumoku()
        'サーバー日付取得
        Dim sysdates As Hashtable = Me.getSysDates()

        Me.dtmSyuptDay.Text = DirectCast(sysdates(KeyDtSysDate), Date).ToString("yy/MM/dd")
        '日本語／外国語コース
        If UserInfoManagement.gaikokugoCrsSelectFlg = True Then
            'ユーザーが国際事業部の場合は外国語
            chkCrsKind1Gaikokugo.Checked = True
            chkCrsKind1Japanese.Checked = False
        Else
            'それ以外の場合は日本語をONに設定
            chkCrsKind1Japanese.Checked = True
            chkCrsKind1Gaikokugo.Checked = False
        End If

        '初期化
        Me.ucoNoribaCd.clear()
        Me.chkCrsKind1Japanese.Checked = False
        Me.chkCrsKind2Teiki.Checked = False
        Me.chkCrsKind2Kikaku.Checked = False
        Me.ucoSyuptTimeFromTo.FromTimeText = Nothing
        Me.ucoSyuptTimeFromTo.ToTimeText = Nothing
        Me.ucoYoyakuNo.YoyakuText = ""
        Me.ucoDairitenCd.clear()
        Me.ucoCrsCd.clear()
        Me.chkJyosyaAlreadyWith.Checked = False
        Me.chkDeleteAlreadyWith.Checked = False
        Me.dtmProcessDay.Clear()
        Me.txtUserId.Text = ""
        Me.ucoEigyosyoCd.clear()
        Me.txtYoyakuPersonSurname.Text = ""
        Me.txtYoyakuPersonName.Text = ""
    End Sub

    ''' <summary>
    ''' エラーのクリア
    ''' </summary>
    Private Sub errorClear()
        Me.dtmSyuptDay.ExistError = False
        Me.ucoNoribaCd.ExistError = False
        Me.chkCrsKind1Japanese.ExistError = False
        Me.chkCrsKind1Gaikokugo.ExistError = False
        Me.ucoSyuptTimeFromTo.ExistErrorForFromTime = False
        Me.ucoSyuptTimeFromTo.ExistErrorForToTime = False
    End Sub

#End Region

#Region "コースグリッドの設定"

    ''' <summary>
    ''' コースグリッドに値を格納
    ''' </summary>
    Private Sub setGrdCrs()

        'カラムの追加
        _crsInfoTable.Columns.Add("colTotalNinzu", Type.GetType("System.Int32"))
        _crsInfoTable.Columns.Add("colSelection", Type.GetType("System.Boolean"))
        _crsInfoTable.Columns.Add("colSyuptTime", Type.GetType("System.DateTime"))

        For Each dataRow As DataRow In _crsInfoTable.Rows

            dataRow("colTotalNinzu") = CInt(dataRow("ADULT_NINZU")) + CInt(dataRow("JUNIOR_NINZU")) + CInt(dataRow("CHILD_NINZU"))
            dataRow("colSelection") = False

            If dataRow("SYUPT_TIME").ToString.Length >= 3 Then
                dataRow("colSyuptTime") = dataRow("SYUPT_TIME").ToString.Insert(dataRow("SYUPT_TIME").ToString.Length - 2, ":")
            End If

        Next

        Dim num As Integer = 0
        For rowNum As Integer = 0 To _crsInfoTable.Rows.Count - 1

            '最大表示件数以上だった場合、行を削除
            If rowNum >= MaxKensu Then
                _crsInfoTable.Rows(rowNum).Delete()

                num = num + 1
            End If
        Next

        '結果をコミット
        _crsInfoTable.AcceptChanges()

        Me.grdCrs.DataSource = _crsInfoTable
    End Sub

#End Region

#Region "予約グリッドの設定"

    ''' <summary>
    ''' 予約情報取得処理
    ''' </summary>
    Private Sub setYoyakuInfo()

        _yoyakuInfoTable = New DataTable

        '入力項目のチェック
        Me.isInputCheckYoyaku()

        For Each dataRow As DataRow In _crsInfoTable.Rows
            'チェックボックスがONの場合
            If CBool(dataRow("colSelection")) = True Then
                _searchForTable("CrsCd") = dataRow("CRS_CD")
                _searchForTable("Gousya") = dataRow("GOUSYA")

                _yoyakuInfoTable.Merge(Me.getYoyakuInfoTable, True)

            End If
        Next

        '解除フラグがFalseの場合
        If _releaseFlg = False Then
            '取得したデータが0件の場合
            If _yoyakuInfoTable.Rows.Count <= 0 Then
                'メッセージ出力（該当するデータがありません。）
                CommonProcess.createFactoryMsg().messageDisp("E90_019")

                Exit Sub
            End If

            ' DataViewを使用してDataTableの並び替えを行う
            ' 並び替える
            Dim dv = New DataView(_yoyakuInfoTable)
            ' 昇順「姓」「名」
            dv.Sort = "SURNAME, NAME"

            ' 並び替え後のデータをDataTableに戻す
            _yoyakuInfoTable = dv.ToTable

            '表示件数オーバーの場合
            If _yoyakuInfoTable.Rows.Count > MaxKensu Then
                'メッセージ出力（検索結果が最大設定可能数を超えました。）
                CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が")

            End If

            Dim num As Integer = 0
            For rowNum As Integer = 0 To _yoyakuInfoTable.Rows.Count - 1

                '最大表示件数以上だった場合、行を削除
                If rowNum >= MaxKensu Then
                    _yoyakuInfoTable.Rows(rowNum - num).Delete()

                    num = num + 1
                End If
            Next
        End If

        '結果をコミット
        _yoyakuInfoTable.AcceptChanges()

        '予約グリッドにセット
        Me.setGrdYoyaku()
    End Sub

    ''' <summary>
    ''' 予約グリッドの設定
    ''' </summary>
    Private Sub setGrdYoyaku()

        'カラムを追加
        _yoyakuInfoTable.Columns.Add("colJyosyaAlready", Type.GetType("System.Boolean"))        '乗車済み
        _yoyakuInfoTable.Columns.Add("colNOSHOW", Type.GetType("System.Boolean"))               'NOSHOW
        _yoyakuInfoTable.Columns.Add("colInquiry", Type.GetType("System.Boolean"))              '請求対象
        _yoyakuInfoTable.Columns.Add("colSurnameName", Type.GetType("System.String"))           '姓名
        _yoyakuInfoTable.Columns.Add("colState", Type.GetType("System.String"))                 '状態
        _yoyakuInfoTable.Columns.Add("colYoyakuNo", Type.GetType("System.String"))              '予約番号
        _yoyakuInfoTable.Columns.Add("colChangeFlg", Type.GetType("System.Boolean"))            '変更フラグ
        _yoyakuInfoTable.Columns.Add("colAgentCdChangeFlg", Type.GetType("System.Boolean"))     '代理店変更フラグ
        _yoyakuInfoTable.Columns.Add("colInquiryChangeFlg", Type.GetType("System.Boolean"))     '請求対象変更フラグ
        _yoyakuInfoTable.Columns.Add("colTotalNinzu", Type.GetType("System.Int32"))             '合計人数
        _yoyakuInfoTable.Columns.Add("colHakkenDay", Type.GetType("System.DateTime"))           '発券日
        _yoyakuInfoTable.Columns.Add("colProcessDay", Type.GetType("System.DateTime"))          '処理日

        '値を代入
        For Each dataRow As DataRow In _yoyakuInfoTable.Rows
            dataRow("colJyosyaAlready") = False
            dataRow("colNOSHOW") = False
            dataRow("colInquiry") = False
            dataRow("colChangeFlg") = False
            dataRow("colAgentCdChangeFlg") = False
            dataRow("colInquiryChangeFlg") = False

            '乗車済とNOSHOW
            If dataRow("CHECKIN_FLG_1").ToString.Trim.Equals(CheckinFlgJyosyaAlready) OrElse
                dataRow("CHECKIN_FLG_2").ToString.Trim.Equals(CheckinFlgJyosyaAlready) OrElse
                dataRow("CHECKIN_FLG_3").ToString.Trim.Equals(CheckinFlgJyosyaAlready) Then

                dataRow("colJyosyaAlready") = True
                dataRow("colNOSHOW") = False

            ElseIf dataRow("CHECKIN_FLG_1").ToString.Trim.Equals(CheckinFlgNOSHOW) OrElse
                dataRow("CHECKIN_FLG_2").ToString.Trim.Equals(CheckinFlgNOSHOW) OrElse
                dataRow("CHECKIN_FLG_3").ToString.Trim.Equals(CheckinFlgNOSHOW) Then

                dataRow("colJyosyaAlready") = False
                dataRow("colNOSHOW") = True

            End If

            '姓名
            dataRow("colSurnameName") = dataRow("SURNAME").ToString & Space(1) & dataRow("NAME").ToString

            '予約番号
            dataRow("colYoyakuNo") = dataRow("YOYAKU_KBN").ToString & FixedCd.CommonCharType.comma & CInt(dataRow("YOYAKU_NO")).ToString("#,0")

            '状態
            If dataRow("CANCEL_FLG").ToString.Equals(Yoyaku.FixedCdYoyaku.YoyakuCancelFlg.torikesi) Then
                dataRow("colState") = StateCancel
            ElseIf dataRow("CANCEL_FLG").ToString.Equals(Yoyaku.FixedCdYoyaku.YoyakuCancelFlg.sakujo) Then
                dataRow("colState") = StateDelete
            ElseIf dataRow("CANCEL_FLG").ToString.Equals(Yoyaku.FixedCdYoyaku.YoyakuCancelFlg.notCancel) AndAlso
                String.IsNullOrEmpty(dataRow("HAKKEN_NAIYO").ToString) AndAlso
                 datarow("ZASEKI_RESERVE_YOYAKU_FLG").ToString.Equals(Yoyaku.FixedCdYoyaku.ZasekiSiteiFlg.sitei) Then
                dataRow("colState") = StateReserve
            ElseIf dataRow("CANCEL_FLG").ToString.Equals(Yoyaku.FixedCdYoyaku.YoyakuCancelFlg.notCancel) AndAlso
                    Not String.IsNullOrEmpty(dataRow("HAKKEN_NAIYO").ToString) AndAlso
                    String.IsNullOrEmpty(dataRow("STATE").ToString) Then
                dataRow("colState") = StateKen
            Else
                dataRow("colState") = ""
            End If

            Dim hakkenDay As String = dataRow("HAKKEN_DAY").ToString

            '発券日の形式変更
            If hakkenDay.Length = 8 Then

                dataRow("colHakkenDay") = CDate(hakkenDay.Substring(0, 4) & FixedCd.CommonCharType.slash & hakkenDay.Substring(4, 2) & FixedCd.CommonCharType.slash & hakkenDay.Substring(6, 2))
            End If

            Dim processDay As String = dataRow("ENTRY_DAY").ToString

            '処理日の形式変更
            If processDay.Length = 8 Then

                dataRow("colProcessDay") = CDate(processDay.Substring(0, 4) & FixedCd.CommonCharType.slash & processDay.Substring(4, 2) & FixedCd.CommonCharType.slash & processDay.Substring(6, 2))
            End If

            '合計人数の計算
            dataRow("colTotalNinzu") = CInt(dataRow("ADULT_NINZU")) + CInt(dataRow("JUNIOR_NINZU")) + CInt(dataRow("CHILD_NINZU"))

            'TODO:営業所での入力・更新は不可、業務管理課のみ更新可能
            '請求対象
            If Not dataRow("AGT_SEIKYU_TAISYO_FLG") Is DBNull.Value Then

                If CInt(dataRow("AGT_SEIKYU_TAISYO_FLG")) = FixedCd.AtoSeisanKbnType.HakkenSeisan Then

                    dataRow("colInquiry") = False

                ElseIf CInt(dataRow("AGT_SEIKYU_TAISYO_FLG")) = FixedCd.AtoSeisanKbnType.ChakukenSeisan Then

                    dataRow("colInquiry") = True

                End If

            Else
                dataRow("colInquiry") = False
            End If

        Next

        '予約テーブルをグリッドへ設定
        Me.grdYoyaku.DataSource = _yoyakuInfoTable
    End Sub

#End Region

    ''' <summary>
    ''' 予約情報(コース料金_料金区分）エンティティの設定
    ''' </summary>
    ''' <returns></returns>
    Private Function createYoyakuInfoCrsChargeChargeKbnEntityForHakken(ByVal yoyakuKbn As String, ByVal yoyakuNo As Integer, ByVal sysDates As Hashtable) As YoyakuInfoCrsChargeChargeKbnEntity
        Dim ent As New YoyakuInfoCrsChargeChargeKbnEntity

        ent.yoyakuKbn.Value = yoyakuKbn
        ent.yoyakuNo.Value = yoyakuNo
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
    ''' 予約番号取得処理
    ''' </summary>
    ''' <returns></returns>
    Private Function getYoyakuNo() As Hashtable

        Dim yoyakuNo As New Hashtable

        '予約区分+NO
        Dim yoyakuKbnNo As String = Me.ucoYoyakuNo.YoyakuText

        If yoyakuKbnNo.Length = 10 Then
            yoyakuNo("yoyakuKbn") = yoyakuKbnNo.Substring(0, 1)
            yoyakuNo("yoyakuNo") = yoyakuKbnNo.Substring(1, yoyakuKbnNo.Length - 1)
        ElseIf yoyakuKbnNo.Length < 10 Then
            yoyakuNo("yoyakuKbn") = "0"
            yoyakuNo("yoyakuNo") = yoyakuKbnNo
        End If

        Return yoyakuNo
    End Function

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

#End Region

End Class