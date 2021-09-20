using System.Text.RegularExpressions;
using C1.Win.C1FlexGrid;

/// <summary>
/// S02_0503
/// </summary>
/// <remarks>
/// Date:2018/10/12
/// Author:Diem.LX
/// </remarks>
public class S02_0503 : FormBase
{

	#region Entity
	//受取
	private ReceiptEntitiy receiptEntitiy = new ReceiptEntitiy();
	//引渡 S02_0502
	private Delivery_S02_0502Entity dataDelivery_S02_0502 = new Delivery_S02_0502Entity();
	//引渡 S02_0103
	private S02_0103ParamData dataDelivery_S02_0103 = new S02_0103ParamData();
	#endregion
	#region  定数／変数宣言
	private string EntityKeys = "RESER_NM,NAME_BASIC,CODE_NAME1,CODE_NAME2,NAIYO,SYSTEM_UPDATE_DAY,SYSTEM_UPDATE_PERSON_CD";
	private DataTable SearchResultGridData = new DataTable();
	private const int RESER_NM = 1;
	private const string regrex = "^[0-9]*$";
	#endregion

	#region Constructor
	public S02_0503(ReceiptEntitiy _receiptEntitiy)
	{
		// This call is required by the designer.
		InitializeComponent();
		receiptEntitiy = _receiptEntitiy;
		// Add any initialization after the InitializeComponent() call.

	}
	#endregion

	#region    Load時
	/// <summary>
	/// 画面全体
	/// </summary>
	protected override void StartupOrgProc()
	{
		string[] logmsg = new string[2];
		//画面表示時の初期設定
		//以下のボタンを表示させる
		this.initFooterButtonControl();
		this.txtCrsCd.Format = setControlFormat(ControlFormat.HankakuEiSuji);
		this.txtGousya.Format = setControlFormat(ControlFormat.HankakuSuji);
		this.txtCrsNm.Width = 320;
		this.grxCrsInfo.Width = 1248;


		//【各画面毎】遷移元画面からの画面間パラメータを画面項目にセットする
		this.setControlInitiarize(receiptEntitiy);
		this.initialValueControl();
		//検索ボタンの関連付け
		//該当画面内のファンクションキーを使用するボタンのクリックイベントとファンクションキーを関連付ける
		btnSearch.Click += base.btnCom_Click;
		btnClear.Click += base.btnCom_Click;

		//画面項目の検索条件（＝画面間パラメータの値）を使用して表示内容を取得する
		//※データ取得方法については、「IO項目定義」参照
		this.settingGrid();
		SearchResultGridData = reloadGrid_Load();

		//For i = 0 To SearchResultGridData.Rows.Count - 1
		//    If Regex.IsMatch(SearchResultGridData.Rows(i)(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)).ToString(), regrex) Then
		//        SearchResultGridData.Rows(i)(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)) = CInt(SearchResultGridData.Rows(i)(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM))).ToString(CommonKyushuUtil.getEnumValue(GetType(String_FormatType), String_FormatType.formatDecimal))
		//    End If
		//Next
		this.grdMemo.DataSource = SearchResultGridData;
		//【各画面毎】画面下部の一覧表示件数を設定する
		displayMainGridCount();

		// フォーカス設定
		this.ActiveControl = this.btnSearch;

	}
	#endregion

	#region 選択行のデータを取得
	/// <summary>
	/// 選択行のデータを取得
	/// </summary>
	/// <remarks></remarks>
	private DataRow[] getSelectedRowData()
	{
		//選択された行データ
		DataRow[] selectData = null;
		//問合せ文字列
		string whereString = string.Empty;

		//引数の設定
		string[] EntityKeysLst = EntityKeys.Split(',');
		whereString = System.Convert.ToString(CommonMstUtil.MakeWhere(grdMemo, EntityKeysLst));

		//問合せ対象データ取得
		selectData = this.SearchResultGridData.Select(whereString);

		return selectData;
	}
	#endregion

	#region データ取得方法については、「IO項目定義」参照

	/// <summary>
	///メモ一覧
	/// </summary>
	public struct MemoList
	{
		//更新
		public const string Update = "更新";
		//予約番号
		public const string Reservation_Number = "予約番号";
		//姓名
		public const string Name = "姓名";
		//区分
		public const string Distinguish = "区分";
		//分類
		public const string Classification = "分類";
		//内容
		public const string Content = "内容";
		//更新日時
		public const string UpdateDateTime = "更新日時";
		//更新者
		public const string Updater = "更新者";

	}

	/// <summary>
	/// Setting Grid
	/// </summary>
	public void settingGrid()
	{

		//更新
		this.grdMemo.AllowSorting = AllowSortingEnum.None;

		//Me.grdMemo.Cols(0).Caption = MemoList.Update
		//Me.grdMemo.Cols(0).AllowEditing = True
		//Me.grdMemo.Cols(0).DataType = GetType(String)
		//Me.grdMemo.Cols(0).Width = 60

		this.grdMemo.Cols(1).Caption = MemoList.Update;
		this.grdMemo.Cols(1).AllowEditing = true;
		this.grdMemo.Cols(1).DataType = typeof(string);
		this.grdMemo.Cols(1).Width = 60;

		//予約番号
		this.grdMemo.Cols(2).Caption = MemoList.Reservation_Number;
		this.grdMemo.Cols(2).AllowEditing = false;
		this.grdMemo.Cols(2).DataType = typeof(string);
		this.grdMemo.Cols(2).Width = 119;

		//姓名
		this.grdMemo.Cols(3).Caption = MemoList.Name;
		this.grdMemo.Cols(3).AllowEditing = false;
		this.grdMemo.Cols(3).DataType = typeof(string);
		this.grdMemo.Cols(3).Width = 183;

		//区分
		this.grdMemo.Cols(4).Caption = MemoList.Distinguish;
		this.grdMemo.Cols(4).AllowEditing = false;
		this.grdMemo.Cols(4).DataType = typeof(string);
		this.grdMemo.Cols(4).Width = 95;

		//分類
		this.grdMemo.Cols(5).Caption = MemoList.Classification;
		this.grdMemo.Cols(5).AllowEditing = false;
		this.grdMemo.Cols(5).DataType = typeof(string);
		this.grdMemo.Cols(5).Width = 175;

		//内容
		this.grdMemo.Cols(6).Caption = MemoList.Content;
		this.grdMemo.Cols(6).AllowEditing = false;
		this.grdMemo.Cols(6).DataType = typeof(string);
		this.grdMemo.Cols(6).Width = 815;

		//更新日時
		this.grdMemo.Cols(7).Caption = MemoList.UpdateDateTime;
		this.grdMemo.Cols(7).AllowEditing = false;
		this.grdMemo.Cols(7).DataType = typeof(string);
		this.grdMemo.Cols(7).Width = 158;

		//更新者
		this.grdMemo.Cols(8).Caption = MemoList.Updater;
		this.grdMemo.Cols(8).AllowEditing = false;
		this.grdMemo.Cols(8).DataType = typeof(string);
		this.grdMemo.Cols(8).Width = 175;

	}
	#endregion

	#region 一覧_更新/ グリッド内ボタン押下
	///' <summary>
	///' 一覧_更新/ グリッド内ボタン押下
	///' </summary>
	///' <param name="sender"></param>
	///' <param name="e"></param>
	//Private Sub grdMemo_Click(sender As Object, e As EventArgs)
	//    Dim hitInfo = grdMemo.HitTest((CType(e, MouseEventArgs)).Location)
	//    If hitInfo.Type = C1.Win.C1FlexGrid.HitTestTypeEnum.ColumnHeader Then
	//        Return
	//    End If
	//    Dim dataSlect As DataRow() = getSelectedRowData()
	//    '選択された行の予約番号が"-"以外の場合
	//    If dataSlect(0)(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)).ToString() <> "-" Then
	//        '選択された行の画面項目．メモ一覧_予約番号
	//        dataDelivery_S02_0103.YoyakuKbn = dataSlect(0)(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)).ToString().Substring(0, 1)
	//        dataDelivery_S02_0103.YoyakuNo = CInt(dataSlect(0)(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)).ToString().Substring(1))

	//        dataDelivery_S02_0103.ScreenMode = CInt(Screen_Mode.UPDATE).ToString()

	//        Dim s02_0103 As S02_0103 = New S02_0103
	//        s02_0103.ParamData = dataDelivery_S02_0103
	//        s02_0103.Show()

	//    Else    '上記以外の場合 ： 『リマークス登録』　（S02_0502）を起動する

	//        '画面項目.コースコード
	//        dataDelivery_S02_0502.CourseCode = Me.txtCrsCd.Text
	//        '画面項目.出発日
	//        dataDelivery_S02_0502.DepartureDate = Me.dtmSyuptDay.Text
	//        '画面項目.号車
	//        dataDelivery_S02_0502.Car = Me.txtGousya.Text
	//        '一覧のボタン押下時 ： 選択された行の枝番
	//        dataDelivery_S02_0502.BranchMumb = CType(Me.SearchResultGridData(0)(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.EDABAN)), String)
	//        '一覧のボタン押下時：  固定値 "2"（更新（削除））
	//        dataDelivery_S02_0502.EditMode = "2"
	//        Dim dlg As S02_0502 = New S02_0502(dataDelivery_S02_0502)
	//        dlg.Show()
	//        '画面間パラメータの再検索フラグが”再検索しない”の場合、処理を終了する。
	//        If dlg.researchFlg = True Then
	//            Call btnF8_ClickOrgProc()
	//        End If
	//    End If
	//End Sub

	private void grdMemo_CellButtonClick(object sender, RowColEventArgs e)
	{

		DataRow selectedRow = null;
		DataTable dt = null;

		// 該当データを取得
		dt = (DataTable)grdMemo.DataSource;
		selectedRow = dt.Rows(e.Row - 1);

		string yoyakuKbn = System.Convert.ToString(selectedRow["RESER_NM"].ToString().Replace(",", "").Substring(0, 1));
		string yoyakuNo = System.Convert.ToString(selectedRow["RESER_NM"].ToString().Replace(",", "").Substring(1));

		//選択された行の予約番号が"-"以外の場合
		if (yoyakuKbn != "-")
		{
			// 予約登録画面に遷移
			bool researchFlg = false;
			using (S02_0103 form = new S02_0103())
			{
				//選択された行の画面項目．メモ一覧_予約番号
				dataDelivery_S02_0103.YoyakuKbn = yoyakuKbn;
				dataDelivery_S02_0103.YoyakuNo = int.Parse(yoyakuNo);
				dataDelivery_S02_0103.ScreenMode = (System.Convert.ToInt32(Screen_Mode.UPDATE)).ToString();
				form.ParamData = dataDelivery_S02_0103;
				// dataDelivery_S02_0103
				form.ShowDialog();
				researchFlg = System.Convert.ToBoolean(form.getReturnValue());
			}


			//画面間パラメータの再検索フラグが”再検索しない”の場合、処理を終了する。
			if (researchFlg == true)
			{
				btnF8_ClickOrgProc();
			}
		}
		else
		{
			//上記以外の場合 ： 『リマークス登録』　（S02_0502）を起動する

			//画面項目.コースコード
			//dataDelivery_S02_0502.CourseCode = Me.txtCrsCd.Text
			dataDelivery_S02_0502.CourseCode = receiptEntitiy.txtCrsCd;
			//画面項目.出発日
			//dataDelivery_S02_0502.DepartureDate = Me.dtmSyuptDay.Text
			dataDelivery_S02_0502.DepartureDate = receiptEntitiy.dtmSyuptDay.ToString("yyyyMMdd");
			//画面項目.号車
			//dataDelivery_S02_0502.Car = Me.txtGousya.Text
			dataDelivery_S02_0502.Car = receiptEntitiy.txtGousya;
			//一覧のボタン押下時 ： 選択された行の枝番
			dataDelivery_S02_0502.BranchMumb = selectedRow["EDABAN"].ToString();
			//一覧のボタン押下時：  固定値 "2"（更新（削除））
			dataDelivery_S02_0502.EditMode = "2";
			//Dim dlg As S02_0502 = New S02_0502(dataDelivery_S02_0502)
			//dlg.Show()
			bool researchFlg = false;
			using (S02_0502 form = new S02_0502(dataDelivery_S02_0502))
			{
				form.ShowDialog();
				researchFlg = System.Convert.ToBoolean(form.researchFlg);
			}


			//画面間パラメータの再検索フラグが”再検索しない”の場合、処理を終了する。
			if (researchFlg == true)
			{
				btnF8_ClickOrgProc();
			}
		}

	}

	#endregion

	#region 【各画面毎】遷移元画面からの画面間パラメータを画面項目にセットする

	/// <summary>
	/// 初期値
	/// </summary>
	private void initialValueControl()
	{
		this.chkShareJiko.Checked = true;
		this.chkMemo.Checked = true;
		this.chkKousyaKashoContact.Checked = true;
		this.chkStaffShare.Checked = true;
		this.chkBusinessHistory.Checked = true;
		this.chkNyuukinKakunin.Checked = true;
		this.chkYoyakuKakunin.Checked = true;
		this.chkNoShowKakunin.Checked = true;
		this.chkSaikouChuusiContact.Checked = true;
		this.chkDuburiCheck.Checked = true;
		this.chkWt.Checked = true;
		this.chkRequest.Checked = true;
	}

	/// <summary>
	/// 【各画面毎】遷移元画面からの画面間パラメータを画面項目にセットする
	/// </summary>
	/// <param name="receiptEntitiy"></param>
	private void setControlInitiarize(ReceiptEntitiy receiptEntitiy)
	{
		CommonUtil.Control_Init(this.grxCrsInfo.Controls);
		CommonUtil.Control_Init(this.grxOut.Controls);
		CommonUtil.Control_Init(this.grdMemo.Controls);

		//・画面項目．出発日 ： 画面間パラメータの「出発日」をセット
		this.dtmSyuptDay.Value = receiptEntitiy.dtmSyuptDay;
		//・画面項目．コースコード ： 画面間パラメータの「コースコード」をセット
		this.txtCrsCd.Text = receiptEntitiy.txtCrsCd;
		//・画面項目．コース名 ： 画面間パラメータの「コース名」をセット
		this.txtCrsNm.Text = receiptEntitiy.txtCrsNm;
		//・画面項目．号車 ： 画面間パラメータの「号車」をセット
		this.txtGousya.Text = receiptEntitiy.txtGousya;
		//・画面項目．予約番号 ： 画面間パラメータの「予約区分」＋「予約番号」をセット（文字列として連結）
		this.ucoYoyakuNo.YoyakuText = receiptEntitiy.txtReserClassification + receiptEntitiy.txtReserNumber;
	}
	#endregion

	#region 固有メソッド
	/// <summary>
	/// 画面表示時の初期設定
	/// </summary>
	private void initFooterButtonControl()
	{

		this.F1Key_Visible = false;
		//・「F2：戻る」ボタン
		this.F2Key_Visible = true;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		//・「F7：出力」ボタン
		this.F7Key_Visible = true;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = false;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;

		F7Key_Text = "F7：出力";

		//フォーカス設定
		this.ActiveControl = this.dtmSyuptDay;
	}
	#endregion

	#region 共有事項Change
	/// <summary>
	/// 共有事項
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkShareJiko_Click(object sender, EventArgs e)
	{
		//・「メモ」のチェックボックスをONにする
		this.chkMemo.Checked = chkShareJiko.Checked;
		//・「降車ヶ所連絡」のチェックボックスをONにする
		this.chkKousyaKashoContact.Checked = chkShareJiko.Checked;
		//・「スタッフ共有」のチェックボックスをONにする
		this.chkStaffShare.Checked = chkShareJiko.Checked;
	}

	/// <summary>
	/// Update status chkShareJiko
	/// </summary>
	private void chkShareJiko_Change()
	{
		if (chkMemo.Checked == false && chkKousyaKashoContact.Checked == false && chkStaffShare.Checked == false)
		{
			chkShareJiko.Checked = false;
		}
	}

	/// <summary>
	/// メモ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkMemo_CheckedChanged(object sender, EventArgs e)
	{
		chkShareJiko_Change();
		if (chkMemo.Checked == true)
		{
			chkShareJiko.Checked = true;
		}
	}

	/// <summary>
	/// 降車箇所連絡
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkKousyaKashoContact_CheckedChanged(object sender, EventArgs e)
	{
		chkShareJiko_Change();
		if (chkKousyaKashoContact.Checked == true)
		{
			chkShareJiko.Checked = true;
		}
	}

	/// <summary>
	/// スタッフ共有
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkStaffShare_CheckedChanged(object sender, EventArgs e)
	{
		chkShareJiko_Change();
		if (chkStaffShare.Checked == true)
		{
			chkShareJiko.Checked = true;
		}
	}
	#endregion

	#region 業務連絡Change
	/// <summary>
	/// 業務連絡
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkBusinessHistory_CheckedChanged(object sender, EventArgs e)
	{
		//・「入金連絡」のチェックボックスをONにする
		this.chkNyuukinKakunin.Checked = chkBusinessHistory.Checked;
		//・「予約確認」のチェックボックスをONにする
		this.chkYoyakuKakunin.Checked = chkBusinessHistory.Checked;
		//・「NOSHOW確認」のチェックボックスをONにする
		this.chkNoShowKakunin.Checked = chkBusinessHistory.Checked;
		//・「催行中止連絡」のチェックボックスをONにする
		this.chkSaikouChuusiContact.Checked = chkBusinessHistory.Checked;
		//・「ダブリチェック」のチェックボックスをONにする
		this.chkDuburiCheck.Checked = chkBusinessHistory.Checked;
		//ウェイト_ ・「ウェイト」のチェックボックスをONにする
		this.chkWt.Checked = chkBusinessHistory.Checked;
		//リクエスト _ ・「リクエスト」のチェックボックスをONにする
		this.chkRequest.Checked = chkBusinessHistory.Checked;
	}

	/// <summary>
	/// Update status chkBusinessHistory
	/// </summary>
	private void chkBusinessHistory_Change()
	{
		if (chkNyuukinKakunin.Checked == false && this.chkYoyakuKakunin.Checked == false &&)
		{
			chkNoShowKakunin.Checked = false && chkSaikouChuusiContact.Checked == false &&;
			chkDuburiCheck.Checked = false && chkWt.Checked == false && chkRequest.Checked == false;

			chkBusinessHistory.Checked = false;
		}
	}

	/// <summary>
	/// 入金連絡
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkNyuukinKakunin_CheckedChanged(object sender, EventArgs e)
	{
		chkBusinessHistory_Change();
		if (chkNyuukinKakunin.Checked == true)
		{
			chkBusinessHistory.Checked = true;
		}
	}

	/// <summary>
	/// 予約確認
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkYoyakuKakunin_CheckedChanged(object sender, EventArgs e)
	{
		chkBusinessHistory_Change();
		if (chkYoyakuKakunin.Checked == true)
		{
			chkBusinessHistory.Checked = true;
		}
	}

	/// <summary>
	/// ＮＯＳＨＯＷ確認
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkNoShowKakunin_CheckedChanged(object sender, EventArgs e)
	{
		chkBusinessHistory_Change();
		if (chkNoShowKakunin.Checked == true)
		{
			chkBusinessHistory.Checked = true;
		}
	}

	/// <summary>
	/// 催行中止連絡
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkSaikouChuusiContact_CheckedChanged(object sender, EventArgs e)
	{
		chkBusinessHistory_Change();
		if (chkSaikouChuusiContact.Checked == true)
		{
			chkBusinessHistory.Checked = true;
		}
	}

	/// <summary>
	/// ダブリチェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkDuburiCheck_CheckedChanged(object sender, EventArgs e)
	{
		chkBusinessHistory_Change();
		if (chkDuburiCheck.Checked == true)
		{
			chkBusinessHistory.Checked = true;
		}
	}

	/// <summary>
	/// ウェイト
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkWt_CheckedChanged(object sender, EventArgs e)
	{
		chkBusinessHistory_Change();
		if (chkWt.Checked == true)
		{
			chkBusinessHistory.Checked = true;
		}
	}

	/// <summary>
	/// リクエスト
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkRequest_CheckedChanged(object sender, EventArgs e)
	{
		chkBusinessHistory_Change();
		if (chkRequest.Checked == true)
		{
			chkBusinessHistory.Checked = true;
		}
	}
	#endregion

	#region    F8

	/// <summary>
	/// 「F8：検索」ボタン
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{
		//画面項目の検索条件を使用して表示内容を取得する
		SearchResultGridData = reloadGrid_SearchF8();
		//For i = 0 To SearchResultGridData.Rows.Count - 1
		//    If Regex.IsMatch(SearchResultGridData.Rows(i)(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)).ToString(), regrex) Then
		//        SearchResultGridData.Rows(i)(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)) =
		//            CInt(SearchResultGridData.Rows(i)(CommonKyushuUtil.getEnumValue(GetType(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM))).ToString(CommonKyushuUtil.getEnumValue(GetType(String_FormatType), String_FormatType.formatDecimal))
		//    End If
		//Next
		this.grdMemo.DataSource = SearchResultGridData;

		//検索結果0件時メッセージを表示する
		if (SearchResultGridData.Rows.Count <= 0)
		{
			// 取得件数0件の場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_019");
		}

		//【各画面毎】画面下部の一覧表示件数を設定する
		displayMainGridCount();
	}
	#endregion

	#region    F7
	/// <summary>
	/// F7ボタン押下時の独自処理
	/// </summary>
	protected override void btnF7_ClickOrgProc()
	{
		// (「P03-X15_DSWEB帳票設計書_P02_0501_メモ一覧表」参照)
		showGeppouDS_Memo();
		// (「P03-X15_DSWEB帳票設計書_P02_0502_リマークス／メモ一覧」参照)
		showGeppouDS_Remarks();
	}
	#endregion

	#region Datastudio 呼び出し
	/// <summary>
	/// Datastudio 呼び出し
	/// </summary>
	/// <remarks></remarks>
	private void showGeppouDS_Memo()
	{
		string PostData = string.Empty;
		string logmsg = string.Empty;
		List[] memo_Bunrui_SharedItems = new List(Of string);
		List[] memo_Bunrui = new List(Of string);

		if (chkMemo.Checked == true)
		{
			memo_Bunrui_SharedItems.Add("01");
		}
		if (chkKousyaKashoContact.Checked == true)
		{
			memo_Bunrui_SharedItems.Add("02");
		}
		if (chkStaffShare.Checked == true)
		{
			memo_Bunrui_SharedItems.Add("03");
		}

		if (chkNyuukinKakunin.Checked == true)
		{
			memo_Bunrui.Add("04");
		}
		if (chkYoyakuKakunin.Checked == true)
		{
			memo_Bunrui.Add("05");
		}
		if (chkNoShowKakunin.Checked == true)
		{
			memo_Bunrui.Add("06");
		}
		if (chkSaikouChuusiContact.Checked == true)
		{
			memo_Bunrui.Add("07");
		}
		if (chkDuburiCheck.Checked == true)
		{
			memo_Bunrui.Add("08");
		}
		if (chkWt.Checked == true)
		{
			memo_Bunrui.Add("09");
		}
		if (chkRequest.Checked == true)
		{
			memo_Bunrui.Add("10");
		}

		try
		{
			//出発日
			if (string.IsNullOrEmpty(System.Convert.ToString(receiptEntitiy.dtmSyuptDay)) == false)
			{
				PostData += "base_select0=P02_0501;SYUPT_DAY&base_opecomp0==&base_value0=" + receiptEntitiy.dtmSyuptDay.ToString(CommonKyushuUtil.getEnumValue(typeof(Date_FormatType), Date_FormatType.formatYYYYMMDD));
			}
			if (this.txtCrsCd.Text != string.Empty)
			{
				//コースコード
				PostData += "&base_opelogic1=AND&base_select1=P02_0501;CRS_CD&base_opecomp1==&base_value1=" + this.txtCrsCd.Text;
			}
			if (string.IsNullOrEmpty(System.Convert.ToString(this.txtGousya.Text)) == false)
			{
				//号車
				PostData += "&base_opelogic2=AND&base_select2=P02_0501;GOUSYA&base_opecomp2==&base_value2=" + this.txtGousya.Text;
			}
			if (chkShareJiko.Checked == true)
			{
				//共有事項
				PostData += "&free_opelogic_0_0=AND&free_select_0_0=P02_0501;MEMO_KBN&free_opecomp_0_0==&free_value_0_0=01";
				if (memo_Bunrui_SharedItems.Count() > 0)
				{
					PostData += "&free_opelogic_0_1=AND&free_select_0_1=P02_0501;MEMO_BUNRUI&free_opecomp_0_1=in($VALS$)&free_value_0_1=" + string.Join("%20", memo_Bunrui_SharedItems);
				}
				if (string.IsNullOrEmpty(System.Convert.ToString(ucoYoyakuNo.YoyakuText)) == false)
				{
					//予約番号
					PostData += "&free_opelogic_0_2=AND&free_select_0_2=P02_0501;YOYAKU_NO_SEARCH&free_opecomp_0_2==&free_value_0_2=" + this.ucoYoyakuNo.YoyakuText;
				}
			}
			if (chkBusinessHistory.Checked == true)
			{
				//業務履歴
				if (chkShareJiko.Checked == true)
				{
					//共有事項が選択されている場合
					PostData += "&free_opelogic_0_3=OR&free_select_0_3=P02_0501;MEMO_KBN&free_opecomp_0_3==&free_value_0_3=02";
				}
				else
				{
					//選択されていない場合
					PostData += "&free_opelogic_0_3=AND&free_select_0_3=P02_0501;MEMO_KBN&free_opecomp_0_3==&free_value_0_3=02";
				}
				if (memo_Bunrui.Count() > 0)
				{
					PostData += "&free_opelogic_0_4=AND&free_select_0_4=P02_0501;MEMO_BUNRUI&free_opecomp_0_4=in($VALS$)&free_value_0_4=" + string.Join("%20", memo_Bunrui);
				}
				if (string.IsNullOrEmpty(System.Convert.ToString(ucoYoyakuNo.YoyakuText)) == false)
				{
					//予約番号
					PostData += "&free_opelogic_0_5=AND&free_select_0_5=P02_0501;YOYAKU_NO_SEARCH&free_opecomp_0_5==&free_value_0_5=" + this.ucoYoyakuNo.YoyakuText;
				}
			}
			if (chkShareJiko.Checked == false && chkBusinessHistory.Checked == false)
			{
				if (string.IsNullOrEmpty(System.Convert.ToString(ucoYoyakuNo.YoyakuText)) == false)
				{
					//予約番号
					PostData += "&free_opelogic_0_6=AND&free_select_0_6=P02_0501;YOYAKU_NO_SEARCH&free_opecomp_0_6==&free_value_0_6=" + this.ucoYoyakuNo.YoyakuText;
				}
			}


			logmsg += "USERID=" + CommonProcess.DataStudioId;
			logmsg += "&PASSWORD=" + CommonProcess.DataStudioPassword;
			logmsg += logmsg + PostData;
			ReadAppConfig reaAppConfig = new ReadAppConfig();
			CommonProcess.DataStudioId = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioId);
			CommonProcess.DataStudioPassword = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioPassword);
			BOCommon.showDataStudio(this.Name, BOCommon.SystemSetCd.dsid_MemoIchiranpyo, PostData);

			//ログ出力(操作ログ)
			createFactoryLog().logOutput(LogKindType.operationLog, ProcessKindType.DSCall, this.setTitle, logmsg);

		}
		catch (Exception ex)
		{
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
			throw;
		}

	}

	private void showGeppouDS_Remarks()
	{
		string PostData = string.Empty;
		string logmsg = string.Empty;
		List[] memo_Bunrui_SharedItems = new List(Of string);
		List[] memo_Bunrui = new List(Of string);

		if (chkMemo.Checked == true)
		{
			memo_Bunrui_SharedItems.Add("01");
		}
		if (chkKousyaKashoContact.Checked == true)
		{
			memo_Bunrui_SharedItems.Add("02");
		}
		if (chkStaffShare.Checked == true)
		{
			memo_Bunrui_SharedItems.Add("03");
		}

		if (chkNyuukinKakunin.Checked == true)
		{
			memo_Bunrui.Add("04");
		}
		if (chkYoyakuKakunin.Checked == true)
		{
			memo_Bunrui.Add("05");
		}
		if (chkNoShowKakunin.Checked == true)
		{
			memo_Bunrui.Add("06");
		}
		if (chkSaikouChuusiContact.Checked == true)
		{
			memo_Bunrui.Add("07");
		}
		if (chkDuburiCheck.Checked == true)
		{
			memo_Bunrui.Add("08");
		}
		if (chkWt.Checked == true)
		{
			memo_Bunrui.Add("09");
		}
		if (chkRequest.Checked == true)
		{
			memo_Bunrui.Add("10");
		}
		try
		{
			//出発日
			if (string.IsNullOrEmpty(System.Convert.ToString(receiptEntitiy.dtmSyuptDay)) == false)
			{
				PostData += "base_select0=P02_0502;SYUPT_DAY&base_opecomp0==&base_value0=" + receiptEntitiy.dtmSyuptDay.ToString(CommonKyushuUtil.getEnumValue(typeof(Date_FormatType), Date_FormatType.formatYYYYMMDD));
			}
			if (this.txtCrsCd.Text != string.Empty)
			{
				//コースコード
				PostData += "&base_opelogic1=AND&base_select1=P02_0502;CRS_CD&base_opecomp1==&base_value1=" + this.txtCrsCd.Text;
			}
			if (string.IsNullOrEmpty(System.Convert.ToString(this.txtGousya.Text)) == false)
			{
				//号車
				PostData += "&base_opelogic2=AND&base_select2=P02_0502;GOUSYA&base_opecomp2==&base_value2=" + this.txtGousya.Text;
			}
			if (chkShareJiko.Checked == true)
			{
				PostData += "&free_opelogic_0_0=AND&free_select_0_0=P02_0502;MEMO_KBN&free_opecomp_0_0==&free_value_0_0=01";
				if (memo_Bunrui_SharedItems.Count() > 0)
				{
					PostData += "&free_opelogic_0_1=AND&free_select_0_1=P02_0502;MEMO_BUNRUI&free_opecomp_0_1=in($VALS$)&free_value_0_1=" + string.Join("%20", memo_Bunrui_SharedItems);
				}
			}
			if (chkBusinessHistory.Checked == true)
			{
				if (chkShareJiko.Checked == true)
				{
					PostData += "&free_opelogic_0_2=OR&free_select_0_2=P02_0502;MEMO_KBN&free_opecomp_0_2==&free_value_0_2=02";
				}
				else
				{
					PostData += "&free_opelogic_0_2=AND&free_select_0_2=P02_0502;MEMO_KBN&free_opecomp_0_2==&free_value_0_2=02";
				}
				if (memo_Bunrui.Count() > 0)
				{
					PostData += "&free_opelogic_0_3=AND&free_select_0_3=P02_0502;MEMO_BUNRUI&free_opecomp_0_3=in($VALS$)&free_value_0_3=" + string.Join("%20", memo_Bunrui);
				}
			}
			logmsg += "USERID=" + CommonProcess.DataStudioId;
			logmsg += "&PASSWORD=" + CommonProcess.DataStudioPassword;
			logmsg += logmsg + PostData;
			ReadAppConfig reaAppConfig = new ReadAppConfig();
			CommonProcess.DataStudioId = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioId);
			CommonProcess.DataStudioPassword = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioPassword);
			BOCommon.showDataStudio(this.Name, BOCommon.SystemSetCd.dsid_MemoIchiran, PostData);

			//ログ出力(操作ログ)
			createFactoryLog().logOutput(LogKindType.operationLog, ProcessKindType.DSCall, this.setTitle, logmsg);

		}
		catch (Exception ex)
		{
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
			throw;
		}

	}
	#endregion

	#region reLoad グリッドデータの取得とグリッド表示
	/// <summary>
	/// グリッドデータの取得とグリッド表示/Load
	/// </summary>
	private DataTable reloadGrid_Load()
	{
		DataTable result = new DataTable();
		RemarksMemoListDisplay_DA lstSearchDataAccess = new RemarksMemoListDisplay_DA();
		Hashtable paramInfoList = new Hashtable();
		string logmsg = string.Empty;

		try
		{
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CRS_CD), txtCrsCd.Text);
			if (dtmSyuptDay.Value IsNot null)
			{
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SYUPT_DAY), (System.Convert.ToDateTime(dtmSyuptDay.Value)).ToString(System.Convert.ToString(CommonKyushuUtil.getEnumValue(typeof(Date_FormatType), Date_FormatType.formatYYYYMMDD))));
			}
			else
			{
				DateTime data = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SYUPT_DAY), (data).ToString(System.Convert.ToString(CommonKyushuUtil.getEnumValue(typeof(Date_FormatType), Date_FormatType.formatYYYYMMDD))));
			}
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.GOUSYA), txtGousya.Text);
			if (ucoYoyakuNo.YoyakuText().Length > 0)
			{
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.YOYAKU_KBN), ucoYoyakuNo.YoyakuText().Substring(0, 1));
			}
			else
			{
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.YOYAKU_KBN), string.Empty);
			}
			if (ucoYoyakuNo.YoyakuText().Length > 1)
			{
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.YOYAKU_NO), ucoYoyakuNo.YoyakuText().Substring(1));
			}
			else
			{
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.YOYAKU_NO), string.Empty);
			}
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM), ucoYoyakuNo.YoyakuText());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SHARED_ITEMS), chkShareJiko.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.MEMO), chkMemo.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DISEMBARKATION_PLACE_REPORT), chkKousyaKashoContact.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.STAFF_SHARING), chkStaffShare.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BUSINESS_HISTORY), chkBusinessHistory.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.PAYMENT_CONTACT), chkNyuukinKakunin.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BOOKING_CONFIRMATION), chkYoyakuKakunin.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.NOSHOW_CONFIRMATION), chkNoShowKakunin.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CANCELLATION_OF_LIAISON), chkSaikouChuusiContact.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DUBRICITY_CHECK), chkDuburiCheck.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.WEIGHT), chkWt.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.REQUEST), chkRequest.Checked.ToString());
			foreach ( in paramInfoList.Keys)
			{
				if (paramInfoList.Item(itemKey) IsNot null && paramInfoList.Item(itemKey).ToString() != string.Empty)
				{
					logmsg = logmsg + itemKey.ToString() + ":" + paramInfoList.Item(itemKey).ToString() + ";";
				}
			}
			result = lstSearchDataAccess.getDataSource(paramInfoList, lstSearchDataAccess.getSubQueryLoad(paramInfoList).ToString());
			createFactoryLog().logOutput(LogKindType.operationLog, ProcessKindType.search, this.setTitle, logmsg);

		}
		catch (Exception ex)
		{
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
			throw;
		}
		return result;
	}

	/// <summary>
	/// グリッドデータの取得とグリッド表示 / F8
	/// </summary>
	private DataTable reloadGrid_SearchF8()
	{
		DataTable result = new DataTable();
		RemarksMemoListDisplay_DA lstSearchDataAccess = new RemarksMemoListDisplay_DA();
		Hashtable paramInfoList = new Hashtable();
		string logmsg = string.Empty;
		try
		{
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CRS_CD), txtCrsCd.Text);
			if (dtmSyuptDay.Value IsNot null)
			{
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SYUPT_DAY), (System.Convert.ToDateTime(dtmSyuptDay.Value)).ToString(System.Convert.ToString(CommonKyushuUtil.getEnumValue(typeof(Date_FormatType), Date_FormatType.formatYYYYMMDD))));
			}
			else
			{
				DateTime data = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SYUPT_DAY), (data).ToString(System.Convert.ToString(CommonKyushuUtil.getEnumValue(typeof(Date_FormatType), Date_FormatType.formatYYYYMMDD))));
			}
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.GOUSYA), txtGousya.Text);
			if (ucoYoyakuNo.YoyakuText().Length > 0)
			{
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.YOYAKU_KBN), ucoYoyakuNo.YoyakuText().Substring(0, 1));
			}
			else
			{
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.YOYAKU_KBN), string.Empty);
			}
			if (ucoYoyakuNo.YoyakuText().Length > 1)
			{
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.YOYAKU_NO), ucoYoyakuNo.YoyakuText().Substring(1));
			}
			else
			{
				paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.YOYAKU_NO), string.Empty);
			}
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM), ucoYoyakuNo.YoyakuText());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.SHARED_ITEMS), chkShareJiko.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.MEMO), chkMemo.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DISEMBARKATION_PLACE_REPORT), chkKousyaKashoContact.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.STAFF_SHARING), chkStaffShare.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BUSINESS_HISTORY), chkBusinessHistory.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.PAYMENT_CONTACT), chkNyuukinKakunin.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.BOOKING_CONFIRMATION), chkYoyakuKakunin.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.NOSHOW_CONFIRMATION), chkNoShowKakunin.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.CANCELLATION_OF_LIAISON), chkSaikouChuusiContact.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.DUBRICITY_CHECK), chkDuburiCheck.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.WEIGHT), chkWt.Checked.ToString());
			paramInfoList.Add(CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.REQUEST), chkRequest.Checked.ToString());
			foreach ( in paramInfoList.Keys)
			{
				if (paramInfoList.Item(itemKey) IsNot null && paramInfoList.Item(itemKey).ToString() != string.Empty)
				{
					logmsg = logmsg + itemKey.ToString() + ":" + paramInfoList.Item(itemKey).ToString() + ";";
				}
			}
			result = lstSearchDataAccess.getDataSource(paramInfoList, lstSearchDataAccess.getSubQuerySearch(paramInfoList).ToString());
			createFactoryLog().logOutput(LogKindType.operationLog, ProcessKindType.search, this.setTitle, logmsg);
		}
		catch (Exception ex)
		{
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
			throw;
		}
		return result;
	}
	#endregion

	#region 条件クリアClick
	///' <summary>
	///' 「条件クリア」ボタン
	///' </summary>
	protected override void btnCLEAR_ClickOrgProc()
	{
		try
		{
			//画面起動時の初期化処理と同様の処理を行う。
			this.setControlInitiarize(receiptEntitiy);
			this.initialValueControl();
			//画面項目の検索条件（＝画面間パラメータの値）を使用して表示内容を取得する
			//※データ取得方法については、「IO項目定義」参照
			this.settingGrid();
			SearchResultGridData = reloadGrid_Load();
			for (i = 0; i <= SearchResultGridData.Rows.Count - 1; i++)
			{
				if (Regex.IsMatch(SearchResultGridData.Rows(i)[CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)].ToString(), regrex))
				{
					SearchResultGridData.Rows(i)[CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)] = (System.Convert.ToInt32(SearchResultGridData.Rows(i)[CommonKyushuUtil.getEnumValue(typeof(ConditionSearchMemoIchiranHyoji), ConditionSearchMemoIchiranHyoji.RESER_NM)])).ToString(System.Convert.ToString(CommonKyushuUtil.getEnumValue(typeof(String_FormatType), String_FormatType.formatDecimal)));
				}
			}

			this.grdMemo.DataSource = SearchResultGridData;
			//【各画面毎】画面下部の一覧表示件数を設定する
			displayMainGridCount();
		}
		catch (Exception)
		{
			throw;
		}
	}
	#endregion

	#region イベント
	///' <summary>
	///' フォームキーダウンイベント
	///' </summary>
	///' <param name="sender"></param>
	///' <param name="e"></param>
	///' <remarks></remarks>
	private void S02_0503_KeyDown(object sender, KeyEventArgs e)
	{
		ButtonEx btn = null;
		if (e.KeyData == Keys.F8)
		{
			btn = this.btnSearch;
		}
		else
		{
			//その他キー
		}
		if (ReferenceEquals(btn, null) == false)
		{
			if (btn.Visible == true && btn.Enabled == true)
			{
				e.Handled = true;
				btnCom_Click(btn, e);
				btn.Focus();
			}
		}
	}
	#endregion

	#region    F2
	/// <summary>
	/// F2ボタン押下時の独自処理
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		// 戻る
		base.closeFormFlg = true;
		this.Close();
	}
	#endregion

	#region    グリッドのデータソース変更時イベント
	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdList_AfterDataRefresh(object sender, System.ComponentModel.ListChangedEventArgs e)
	{
		//メイングリッド件数表示
		displayMainGridCount();
	}

	/// <summary>
	/// メイングリッド件数表示
	/// </summary>
	protected virtual void displayMainGridCount()
	{

		if (!ReferenceEquals(grdMemo, null) && !ReferenceEquals(lblLengthGrd, null))
		{
			//データ件数を表示(ヘッダー行分マイナス)
			//Me.mainGridDataCntDsp.Text = String.Format("{0:#,0}件", (mainGrid.Rows.Count - mainGrid.Rows.Fixed)).PadLeft(6)
			ClientCommonKyushuUtil.setGridCount(grdMemo, this.lblLengthGrd);
		}
	}

	#endregion

}