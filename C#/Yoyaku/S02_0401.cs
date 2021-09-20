using C1.Win.C1FlexGrid;
using Microsoft.VisualBasic.CompilerServices;


/// <summary>
/// 予約照会（バス単位）
/// </summary>
public class S02_0401 : PT11, iPT11
{


	#region 定数／変数
	// "変数"
	private bool blnCourseAssigne = false; //コースコードFull桁指定（True=指定,False=指定なし）

	// "定数"

	/// <summary>
	/// 画面名
	/// </summary>
	private const string ScreenName = "予約照会（バス単位）";
	/// <summary>
	/// 画面モード
	/// </summary>
	public const string ScreenModeNew = "1"; //新規
	public const string ScreenModeEdit = "2"; //更新
	public const string ScreenModeReference = "3"; //参照

	/// <summary>
	/// キャンセルフラグ
	/// </summary>
	private const string CancelFlg_Before = "1"; //発券前
	private const string CancelFlg_After = "2"; //発券後

	/// <summary>
	/// 状態
	/// </summary>
	private const string JOTAI_BLANK = ""; //ブランク
	private const string JOTAI_SHITEI = "指"; //指定
	private const string JOTAI_KEN = "券"; //券
	private const string JOTAI_JTB = "Ｊ"; //JTB
	private const string JOTAI_REN = "連"; //連
	private const string JOTAI_KNT = "Ｋ"; //KNT(近畿日本ﾂｰﾘｽﾄ)
	private const string JOTAI_ERASE = "消"; //消
	private const string JOTAI_DELETE = "削"; //削
	private const string JOTAI_OTHER = ""; //上記以外

	/// <summary>
	/// 区分値'Y'
	/// </summary>
	private const string KbnValueY = "Y";
	/// <summary>
	/// 区分値'N'
	/// </summary>
	private const string KbnValueN = "N";
	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	public const int TopGbxCondition = 27;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	public const int MarginGbxCondition = 6;

	/// <summary>
	/// 検索結果最大表示件数件数
	/// </summary>
	public const int MaxKensu = 100;

	/// <summary>
	/// '「予約登録」プログラムID
	/// </summary>
	private const string YoyakuToroku = "S02_0103";

	#endregion

	#region プロパティ

	// "パブリック プロパティ"
	//
	/// <summary>
	/// パラメータクラス
	/// </summary>
	public S02_0401ParamData ParamData
	{

		/// <summary>
		/// 条件GroupBoxの高さ
		/// </summary>
		/// <returns></returns>
	public int HeightGbxCondition
	{
		get
		{
			return this.gbxSearchKoumoku.Height;
		}
		set
		{
			this.gbxSearchKoumoku.Height = value;
			this.gbxCrs.Height = value;
		}
	}

	/// <summary>
	/// 条件GroupBoxの表示/非表示
	/// </summary>
	/// <returns></returns>
	public bool VisibleGbxCondition
	{
		get
		{
			return this.gbxSearchKoumoku.Visible;
		}
		set
		{
			this.gbxSearchKoumoku.Visible = value;
			this.gbxCrs.Visible = value;
		}
	}
	#endregion

	#region イベント
	/// <summary>
	/// 画面ロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S02_0401_Load(object sender, EventArgs e)
	{
		base.closeFormFlg = false; //クローズ確認フラグ
	}

	/// <summary>
	/// 画面を閉じる
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S02_0401_FormClosing(object sender, FormClosingEventArgs e)
	{
	}

	/// <summary>
	/// F2(戻る)ボタン押下時
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		//戻る
		closeCheckFlg = true;
		base.closeFormFlg = false;
		this.Close();
	}

	/// <summary>
	/// F8(検索)ボタン押下イベント
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{
		//MyBase.btnF8_ClickOrgProc()
		//エラー有無のクリア
		clearError();

		//日付入力値の調整
		setYmdFromTo();

		// [詳細エリア]検索結果部の項目初期化
		initDetailAreaItems();

		//検索条件項目入力チェック
		if (this.CheckSearch() == true)
		{
			//エラーがない場合、検索処理を実行

			// Gridへの表示(グリッドデータの取得とグリッド表示)
			reloadGrid();
		}
	}

	/// <summary>
	/// F9(内訳)ボタン押下イベント
	/// </summary>
	protected override void btnF9_ClickOrgProc()
	{
		//内訳詳細表示前チェック
		if (this.CheckUtiwake() == true)
		{
			//エラーがない場合、検索処理を実行

			// Gridへの表示(グリッドデータの取得とグリッド表示)
			GetgrdYoyakuInquiry();
		}
		// 後処理
		base.comPostEvent();
		this.btnSearch.Focus();
		//End Try
	}

	/// <summary>
	/// 条件クリアボタン押下時
	/// </summary>
	private void btnClear_Click(object sender, EventArgs e)
	{
		// クリアボタン押下イベント実行
		base.btnCom_Click(this.btnClear, e);
	}

	/// <summary>
	/// 条件クリアボタン押下時
	/// </summary>
	protected override void btnCLEAR_ClickOrgProc()
	{
		//' 検索条件部の項目初期化
		//initSearchAreaItems()
		// 初期処理と同じ処理を実行
		StartupOrgProc();

	}

	/// <summary>
	/// 条件GroupBox表示制御ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void BtnVisiblerCondition_Click(object sender, EventArgs e)
	{
		this.VisibleGbxCondition = !this.VisibleGbxCondition;

		//Panel, グリッドの座標, サイズを表示/非表示に応じて変更
		if (this.VisibleGbxCondition)
		{
			//表示状態
			this.btnVisiblerCondition.Text = "非表示 <<";

			this.PanelEx1.Top = TopGbxCondition + this.HeightGbxCondition + MarginGbxCondition;
			this.PanelEx1.Height -= this.HeightGbxCondition + MarginGbxCondition;
			this.grdYoyakuInquiry.Height -= this.HeightGbxCondition - 3;
		}
		else
		{
			//非表示状態
			this.btnVisiblerCondition.Text = "表示 >>";

			this.PanelEx1.Top = TopGbxCondition;
			this.PanelEx1.Height += this.HeightGbxCondition + MarginGbxCondition;
			this.grdYoyakuInquiry.Height += this.HeightGbxCondition - 3;
		}
	}

	/// <summary>
	/// 検索ボタン押下処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void BtnSearch_Click(object sender, EventArgs e)
	{

		// 検索ボタン押下イベント実行
		base.btnCom_Click(this.btnSearch, e);

		// log出力
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理");

	}

	/// <summary>
	/// 内訳ボタン押下処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void BtnUtiwake_Click(object sender, EventArgs e)
	{

		// 内訳ボタン押下イベント実行
		base.btnCom_Click(this.btnUtiwake, e);

		// log出力
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.sonota, ScreenName, "内訳表示処理");

	}

	/// <summary>
	/// キーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void S02_0401_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.F5)
		{
			Control cControl = this.ActiveControl;
			if (!ReferenceEquals(cControl, null))
			{
				if ((string)cControl.Name == "ucoCrsCd")
				{
					//【コースコード検索】画面へ遷移
					//TODO:共通処理：遷移先画面未完成
					//Using form As New ????()
					//    Dim prm As New ????ParamData()
					//    prm.CrsCd =ucoCrsCd.CodeText.ToString.Trim
					//    form.ParamData = prm    '画面間パラメータセット
					//    form.ShowDialog()
					//End Using
				}
				else if ((string)cControl.Name == "ucoJyosyaTiCd")
				{
					//【場所検索】画面へ遷移
					//TODO:共通処理：遷移先画面未完成
					//Using form As New S90_0109()
					//    Dim prm As New S90_0109ParamData()
					//    prm.PlaceCd = ucoJyosyaTiCd.CodeText.ToString.Trim
					//    form.ParamData = prm    '画面間パラメータセット
					//    form.ShowDialog()
					//End Using
				}
				else if ((string)cControl.Name == "ucoKoshakasho")
				{
					//【仕入先選択】画面へ遷移（降車ヶ所検索は仕入先選択より行う）
					//TODO:共通処理：遷移先画面未完成
					//Using form As New S90_0102()
					//    Dim prm As New S90_0102ParamData()
					//    prm.SiiresakiCd = ucoKoshakasho.CodeText.ToString.Trim
					//    form.ParamData = prm    '画面間パラメータセット
					//    form.ShowDialog()
					//End Using
				}
			}
		}
		else if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			e.Handled = true;
			this.BtnSearch_Click(sender, e);
		}
		else if (e.KeyData == Keys.F9)
		{
			e.Handled = true;
			this.BtnUtiwake_Click(sender, e);
		}
		else
		{
			return;
		}
	}

	/// <summary>
	/// コース一覧ダブルクリックイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void gbxCrs_DoubleClick(object sender, EventArgs e)
	{
		this.BtnUtiwake_Click(sender, e);
	}

	/// <summary>
	/// 詳細ボタン押下イベント              'TODO:遷移先画面未完成
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdYoyakuInquiry_CellButtonClick(object sender, RowColEventArgs e)
	{
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		DataTable dt = new DataTable();
		S02_0401Da dataAccess = new S02_0401Da();
		DateTime nowDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		string strSysDate = System.Convert.ToString((nowDate.Year * 10000 + nowDate.Month * 100 + nowDate.Day).ToString().Trim());
		int intSysTime = nowDate.Hour * 100 + nowDate.Minute;

		object with_1 = grdYoyakuInquiry;
		paramInfoList.Add("YoyakuNo", System.Convert.ToInt32(with_1.Rows(e.Row).Item("YOYAKU_NO")));
		paramInfoList.Add("YoyakuKbn", with_1.Rows(e.Row).Item("YOYAKU_KBN").ToString().Trim());
		try
		{
			dt = dataAccess.AccessYoyakuIList(S02_0401Da.AccessType.yoyakuInfoByPrimaryKey, paramInfoList);
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, this.Name);
			//Exit Sub
		}
		catch (Exception ex)
		{
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
			throw;
		}

		if (dt.Rows.Count < 1)
		{
			//該当する予約情報が存在しません。
			with_1.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_014", "予約情報");
			return;
		}
		//予約登録画面に遷移
		using (S02_0103 form = new S02_0103())
		{
			S02_0103ParamData prm = new S02_0103ParamData();
			//2019/07/02 過去日チェック廃止により、出発済みコースでも、参照モードで予約登録画面に遷移可とする。
			if (Information.IsDate(this.dtmSyuptDay.Value) == true &&)
			{
				Me.dtmSyuptDay.Text(< CommonDateUtil.getSystemTime().ToShortDateString().Substring(2));
				// 出発済みのコースは編集不可
				prm.ScreenMode = ScreenModeReference; //参照モード
			}
			else
			{
				prm.ScreenMode = ScreenModeEdit; //編集モード
			}
			prm.YoyakuNo = System.Convert.ToInt32(with_1.Rows(e.Row).Item("YOYAKU_NO"));
			prm.YoyakuKbn = with_1.Rows(e.Row).Item("YOYAKU_KBN").ToString().Trim();

			form.ParamData = prm;
			form.ShowDialog();
		}

	}

	#endregion

	#region メソッド

	/// <summary>
	/// 画面クローズ時
	/// </summary>
	/// <returns></returns>
	protected override bool closingScreen()
	{
		// 変数宣言
		bool retCode = false;
		return retCode;
	}

	/// <summary>
	/// 画面初期Load時のメソッド
	/// </summary>
	protected override void initScreenPerttern()
	{
		//ベースフォームの初期化処理
		base.initScreenPerttern();

	}

	/// <summary>
	/// フッタボタンの制御(表示\[活性]／非表示[非活性])
	/// </summary>
	protected override void InitFooterButtonControl()
	{
		// 不要ボタンを非表示、無効(非活性)且つにする
		this.F1Key_Visible = false; // F1:未使用
		this.F1Key_Enabled = false;
		this.F3Key_Visible = false; // F3;未使用
		this.F3Key_Enabled = false; //
		this.F1Key_Visible = false; // F1:未使用
		this.F1Key_Enabled = false; //
		this.F3Key_Visible = false; // F3;未使用
		this.F3Key_Enabled = false; //
		this.F4Key_Visible = false; // F4;未使用
		this.F4Key_Enabled = true; //
		this.F5Key_Visible = false; // F5:未使用
		this.F5Key_Enabled = false; //
		this.F6Key_Visible = false; // F6:画面ごと
		this.F6Key_Enabled = false; //
		this.F7Key_Visible = false; // F7:未使用
		this.F7Key_Enabled = false; //
		this.F8Key_Visible = false; // F8:検索
		this.F8Key_Enabled = false; //
		this.F9Key_Visible = false; // F9:画面ごと
		this.F9Key_Enabled = false; //
		this.F10Key_Visible = false; // F10:
		this.F10Key_Enabled = false; //
		this.F11Key_Visible = false; // F11:更新
		this.F11Key_Enabled = false; //
		this.F12Key_Visible = false; // F12:画面ごと
		this.F12Key_Enabled = false; //

	}

	// 初期化用のメソッド

	/// <summary>
	/// [検索エリア]検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{
		DateTime nowDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		//'前回エラークリア
		//Me.dtmSyuptDay.ExistError = False
		//Me.ucoCrsCd.ExistError = False
		//Me.ucoJyosyaTiCd.ExistError = False
		//Me.dtmSyuptTime.ExistError = False
		//Me.ucoKoshakasho.ExistError = False

		//条件GroupBox内のテキストボックス初期化
		object textBoxs = this.gbxSearchKoumoku.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
		}

		//コードコントロール初期化
		object CodeControls = this.gbxSearchKoumoku.Controls.OfType(Of Master.CodeControlEx);
		foreach ( in CodeControls)
		{
			CodeControl.CodeText = "";
			CodeControl.ValueText = "";
			CodeControl.ExistError = false;
		}

		//チェックボックス初期化
		object CheckBoxs = this.gbxSearchKoumoku.Controls.OfType(Of CheckBoxEx);
		foreach ( in CheckBoxs)
		{
			CheckBox.Checked = false;
			CheckBox.ExistError = false;
		}
		this.chkCancelWith.Checked = true;

		//条件GroupBox内のテキストボックス初期化
		textBoxs = gbxTotal.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
		}

		//グリッドの初期化
		DataTable dt = new DataTable();
		this.grdCrs.DataSource = dt;
		this.SetgrdCrsInquiry();
		this.lblLengthGrd01.Text = "";
		this.grdCrs.DataMember = "";
		this.grdCrs.Refresh();

		this.grdYoyakuInquiry.DataSource = dt;
		this.SetgrdYoyakuInquiry();
		this.lblLengthGrd02.Text = "";
		this.grdYoyakuInquiry.DataMember = "";
		this.grdYoyakuInquiry.Refresh();

		this.grdGroup.DataSource = dt;
		this.SetgrdGroupInquiry();
		this.grdGroup.DataMember = "";
		this.grdGroup.Refresh();

		//他コントロールの初期化
		this.dtmSyuptDay.Value = nowDate;

		ucoJyosyaTiCd.CodeText = "";
		ucoJyosyaTiCd.ValueText = "";
		ucoJyosyaTiCd.ExistError = false;
		chkCancelWith.Checked = false;
		this.dtmSyuptTime.Value = null;
		this.chkCancelWith.Checked = false;

		this.lblLengthGrd01.Text = "     0件";
		this.lblLengthGrd02.Text = "     0件";

	}

	/// <summary>
	/// エラー有無のクリア
	/// </summary>
	private void clearError()
	{
		// ExistErrorプロパティのクリア
		//前回エラークリア
		this.dtmSyuptDay.ExistError = false;
		this.ucoCrsCd.ExistError = false;
		this.ucoJyosyaTiCd.ExistError = false;
		this.dtmSyuptTime.ExistError = false;
		this.ucoKoshakasho.ExistError = false;
		this.chkUnkyuSaikouChuusiWith.ExistError = false;
		this.chkCancelWith.ExistError = false;

		// Exceptionのクリア
		this.Exception = null;

		// エラーフラグの初期化
		this.ErrorFlg = false;

	}
	/// <summary>
	///  [詳細エリア]検索結果部の項目初期化
	/// </summary>
	protected override void initDetailAreaItems()
	{
		base.initDetailAreaItems();

		DataTable dt = new DataTable();
		this.SetgrdYoyakuInquiry();
		//Me.grdYoyakuInquiry.DataSource = dt
		//Me.grdYoyakuInquiry.DataMember = ""
		//Me.grdYoyakuInquiry.Refresh()
		//Me.grdYoyakuInquiry.Text = "     0件"
	}

	/// <summary>
	/// エンティティ初期化
	/// </summary>
	protected override void initEntityData()
	{
	}

	/// <summary>
	/// [選択行設定]Grid=>エンティティ(選択行のデータを取得)
	/// </summary>
	protected override void getSelectedRowData()
	{
	}

	/// <summary>
	/// Gridへの表示(グリッドデータの取得とグリッド表示)
	/// </summary>
	protected override void reloadGrid()
	{
		base.reloadGrid();
		GetgrdCrsListInquiry();

	}

	// チェック系
	/// <summary>
	/// [検索処理](検索条件項目入力チェック)
	/// </summary>
	/// <returns></returns>
	protected override bool checkSearchItems()
	{
		return true;
	}

	/// <summary>
	/// 日付入力値の調整
	/// </summary>
	private void setYmdFromTo()
	{
		if (dtmSyuptDay.Text IsNot null && ReferenceEquals(dtmSyuptDay.Text, null))
			{
			//日付 <> ブランクの場合、日付 にシステム日付の値をセット
			dtmSyuptDay.Value = CommonDateUtil.getSystemTime();
		}
	}

	/// <summary>
	/// コースグリッドの設定
	/// </summary>
	private void SetgrdCrsInquiry()
	{
		DataTable dt = new DataTable();
		object with_1 = grdCrs;
		//グリッド初期化
		with_1.DataSource = dt;
		with_1.DataMember = "";
		with_1.Refresh();
		this.lblLengthGrd01.Text = "0件";
		with_1.Cols.Count = 16;
		with_1.Rows(0).AllowMerging = true;
		with_1.AllowAddNew = false;
		//項目タイトルを改行して表示するため、列ヘッダの高さとヘッダタイトルの指定
		with_1.Rows(0).Height = 37;

		object range1 = with_1.GetCellRange(0, 1);
		range1.Data = "乗り場" + "\r\n" + "*経由";

		object range2 = with_1.GetCellRange(0, 2);
		range2.Data = "コース" + "\r\n" + "コード";

		//非表示部分
		for (byte intCnt = 8; intCnt <= 15; intCnt++)
		{
			with_1.Cols(intCnt).Visible = false;
		}

	}

	/// <summary>
	/// コースグリッドの設定
	/// </summary>
	private void SetgrdGroupInquiry()
	{
		DataTable dt = new DataTable();
		object with_1 = grdGroup;
		//グリッド初期化
		with_1.DataSource = dt;
		with_1.DataMember = "";
		with_1.Refresh();
		with_1.Cols.Count = 4;
		with_1.Rows(0).AllowMerging = true;
		with_1.AllowAddNew = false;
	}

	/// <summary>
	/// 予約一覧照会グリッドの設定
	/// </summary>
	private void SetgrdYoyakuInquiry()
	{
		DataTable dt = new DataTable();
		object with_1 = grdYoyakuInquiry;

		//グリッド初期化
		with_1.AllowAddNew = false;
		with_1.DataSource = dt;
		with_1.DataMember = "";
		with_1.Refresh();
		this.lblLengthGrd02.Text = "0件";
		with_1.SuspendLayout();
		with_1.Cols.Count = 24;
		with_1.Rows(0).AllowMerging = true;

		//非表示部分
		for (byte intCnt = 13; intCnt <= 23; intCnt++)
		{
			with_1.Cols(intCnt).Visible = false;
		}

		with_1.AllowAddNew = false;
	}

	/// <summary>
	/// コース一覧用グリッド表示値の設定
	/// </summary>
	private void GetgrdCrsListInquiry()
	{
		DateTime dteTmpDate = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		int intSyuptDate = 0;
		int intSyuptTime = 0;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		DataTable dt = new DataTable();
		//'グリッド初期化
		this.grdCrs.AllowAddNew = false;
		this.grdCrs.DataSource = dt;
		this.grdCrs.DataMember = "";
		this.grdCrs.Refresh();
		this.lblLengthGrd01.Text = "0件";

		this.grdGroup.AllowAddNew = false;
		this.grdGroup.DataSource = dt;
		this.grdGroup.DataMember = "";
		this.grdGroup.Refresh();

		this.grdYoyakuInquiry.AllowAddNew = false;
		this.grdYoyakuInquiry.DataSource = dt;
		this.grdYoyakuInquiry.DataMember = "";
		this.grdYoyakuInquiry.Refresh();
		this.lblLengthGrd02.Text = "0件";

		// Web予約作成ゴミデータ削除処理
		S02_0301Da dataAccess1 = new S02_0301Da();
		dataAccess1.updateWebWastedData();

		//条件GroupBox内のテキストボックス初期化
		object textBoxs = this.gbxTotal.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
		}

		this.grdCrs.SuspendLayout();
		//パラメータ設定

		//出発日
		if (Information.IsDate(this.dtmSyuptDay.Text) == true)
		{
			dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(this.dtmSyuptDay.Text));
			intSyuptDate = dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day;
			paramInfoList.Add("SyuptDay", intSyuptDate);
		}

		//コースコード
		paramInfoList.Add("CrsCd", Strings.Trim(System.Convert.ToString(this.ucoCrsCd.CodeText)));
		paramInfoList.Add("CrsCdAssigne", blnCourseAssigne); //コースコード検索用（指定コードFull桁判定）

		//乗車地1
		paramInfoList.Add("JyosyaTiCd01", Strings.Trim(System.Convert.ToString(ucoJyosyaTiCd.CodeText)));

		//出発時間
		if (Information.IsDate(this.dtmSyuptTime.Text) == true)
		{
			dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(this.dtmSyuptTime.Text));
			intSyuptTime = dteTmpDate.Hour * 100 + dteTmpDate.Minute;
			paramInfoList.Add("SyuptTime", intSyuptTime);
		}

		//降車ヵ所
		if (this.ucoKoshakasho.CodeText.Length >= 4)
		{
			paramInfoList.Add("Koshakasho", Strings.Trim(System.Convert.ToString(this.ucoKoshakasho.CodeText.Substring(0, 4))));
			if (this.ucoKoshakasho.CodeText.Length > 4)
			{
				// 降車ヵ所CDが4桁を超える場合、枝番を取り出す
				paramInfoList.Add("KoshakashoEdaban", Strings.Trim(System.Convert.ToString(this.ucoKoshakasho.CodeText.Substring(4))));
			}
		}

		//運休・催行中止を含む
		paramInfoList.Add("UnkyuSaikouChuusiWith", Strings.Trim(System.Convert.ToString(this.chkUnkyuSaikouChuusiWith.Checked.ToString())));

		S02_0401Da dataAccess = new S02_0401Da();
		DataTable dataCRS_LEDGER = dataAccess.AccessCourseInfo(S02_0401Da.AccessType.courseByHeaderKey, paramInfoList);

		if (dataCRS_LEDGER.Rows.Count == 0)
		{
			// 取得件数0件の場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_004");
			this.grdCrs.ResumeLayout();
			return;
		}
		grdCrs.DataSource = dataCRS_LEDGER;
		string formatedCount = string.Empty;
		if (dataCRS_LEDGER.Rows.Count > MaxKensu)
		{
			formatedCount = System.Convert.ToString((MaxKensu).ToString().PadLeft(6));
		}
		else
		{
			formatedCount = System.Convert.ToString((dataCRS_LEDGER.Rows.Count).ToString().PadLeft(6));
		}

		this.lblLengthGrd01.Text = formatedCount + "件";
		if (dataCRS_LEDGER.Rows.Count > MaxKensu)
		{
			grdCrs.Rows.Remove(MaxKensu + 1); //セレクト時に、Max件数（100件）+ 1件分を削除
											  // 取得件数が設定件数より多い場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が");
		}
		this.grdCrs.Refresh();
		this.grdCrs.ResumeLayout();
	}

	/// <summary>
	/// 予約一覧グリッド表示値の設定
	/// </summary>
	private void GetgrdYoyakuInquiry()
	{
		int intDate = 0;
		DateTime dteTmpDate = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		string strDate = string.Empty;
		string strFromDate = string.Empty;
		string strToDate = string.Empty;
		string strSysDate = string.Empty;
		int intAdultNinzu = 0;
		int intJuniorNinzu = 0;
		int intChildNinzu = 0;
		int[] intNinzu = null;
		int intGroupNinzu = 0;
		int intMaxGroupNinzu = 0;
		int intNinzuKei = 0;
		int intGpKei = 0;

		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		DataTable dt = new DataTable();
		DataRow drX = null;
		DataTable dataYoyaku_Info_Basic = null;

		S02_0401Da dataAccess = new S02_0401Da();

		//'グリッド初期化
		this.grdYoyakuInquiry.AllowAddNew = false;
		this.grdYoyakuInquiry.DataSource = dt;
		this.grdYoyakuInquiry.DataMember = "";
		this.grdYoyakuInquiry.Refresh();
		this.lblLengthGrd02.Text = "0件";

		this.grdGroup.AllowAddNew = false;
		this.grdGroup.DataSource = dt;
		this.grdGroup.DataMember = "";
		this.grdGroup.Refresh();

		//条件GroupBox内のテキストボックス初期化
		object textBoxs = this.gbxTotal.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
		}
		this.grdYoyakuInquiry.SuspendLayout();
		//パラメータ設定
		object with_1 = this.grdCrs;
		dataYoyaku_Info_Basic = new DataTable();
		//コースコード
		paramInfoList.Add("CrsCd", with_1.Item(with_1.Row, "CRS_CD").ToString().Trim());
		//号車
		paramInfoList.Add("Gousya", with_1.Item(with_1.Row, "GOUSYA").ToString().Trim());
		//乗車地
		paramInfoList.Add("JyosyaTi", with_1.Item(with_1.Row, "JYOSYATI").ToString().Trim());
		//出発日
		paramInfoList.Add("SyuptDay", with_1.Item(with_1.Row, "SYUPT_DAY").ToString().Trim());
		//乗車地コード
		paramInfoList.Add("JyosyaTiCd", with_1.Item(with_1.Row, "HAISYA_KEIYU_CD").ToString().Trim());
		//キャンセルを含む
		paramInfoList.Add("CanselWith", Strings.Trim(System.Convert.ToString(this.chkCancelWith.Checked.ToString())));
		try
		{
			dataYoyaku_Info_Basic = dataAccess.AccessYoyakuIList(S02_0401Da.AccessType.yoyakuNormalByHeaderKey, paramInfoList);
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, this.Name);
		}
		catch (Exception ex)
		{
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
			throw;
		}
		if (dataYoyaku_Info_Basic.Rows.Count == 0)
		{
			// 取得件数0件の場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_004");
			this.grdYoyakuInquiry.ResumeLayout();
			return;
		}
		grdYoyakuInquiry.DataSource = dataYoyaku_Info_Basic;
		if (dataYoyaku_Info_Basic.Rows.Count > MaxKensu)
		{
			// 取得件数が設定件数より多い場合、メッセージを表示
			grdYoyakuInquiry.Rows.Remove(MaxKensu + 1);
			CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が");
		}
		//状態セット
		for (i = 1; i <= grdYoyakuInquiry.Rows.Count - 1; i++)
		{
			//If IsDate(grdYoyakuInquiry(i, "UKETSUKE_DAY").ToString.Trim) = True Then
			//    dteTmpDate = Convert.ToDateTime(grdYoyakuInquiry(i, "UKETSUKE_DAY").ToString.Trim)
			//    grdYoyakuInquiry(i, "UKETSUKE_DAY") = grdYoyakuInquiry(i, "UKETSUKE_DAY").ToString.Substring(5) & "(" &
			//        Common.CommoDateUtil.DayOfTheWeek(dteTmpDate).Substring(0, 1) & ")"
			//End If
			grdYoyakuInquiry[i, "JOTAI"] = JOTAI_BLANK; //ブランク
														//キャンセルフラグ	1: 発券前 2: 発券後
			if ((string)(grdYoyakuInquiry(i, "CANCEL_FLG").ToString().Trim()) == "")
			{
				//発券内容
				if (grdYoyakuInquiry(i, "HAKKEN_NAIYO").ToString().Trim() == "")
				{
					//座席指定予約フラグ
					if (grdYoyakuInquiry(i, "ZASEKI_RESERVE_YOYAKU_FLG").ToString().Trim() == "")
					{
						grdYoyakuInquiry[i, "JOTAI"] = JOTAI_BLANK; //ブランク
					}
					else if (grdYoyakuInquiry(i, "ZASEKI_RESERVE_YOYAKU_FLG").ToString().Trim() == KbnValueY)
					{
						grdYoyakuInquiry[i, "JOTAI"] = JOTAI_SHITEI; //指：座席指定
					}
				}
				else
				{
					//状態STATE
					if ((string)(grdYoyakuInquiry(i, "STATE").ToString().Trim()) == "")
					{
						grdYoyakuInquiry[i, "JOTAI"] = JOTAI_KEN; //券
						if (grdYoyakuInquiry(i, "ZASEKI_RESERVE_YOYAKU_FLG").ToString().Trim() == KbnValueY) //座席指定予約＆発券済
						{
							grdYoyakuInquiry[i, "JOTAI"] = JOTAI_SHITEI + JOTAI_KEN; //指券"
						}
					}
					else if ((string)(grdYoyakuInquiry(i, "STATE").ToString().Trim()) == "1")
					{
						grdYoyakuInquiry[i, "JOTAI"] = JOTAI_JTB; //Ｊ:JTB
					}
					else if ((string)(grdYoyakuInquiry(i, "STATE").ToString().Trim()) == "2")
					{
						grdYoyakuInquiry[i, "JOTAI"] = JOTAI_REN; //連
					}
					else if ((string)(grdYoyakuInquiry(i, "STATE").ToString().Trim()) == "3")
					{
						grdYoyakuInquiry[i, "JOTAI"] = JOTAI_KNT; //Ｋ:KNT(近畿日本ﾂｰﾘｽﾄ)
					}
				}
			} //1:発券前
			else if (grdYoyakuInquiry(i, "CANCEL_FLG").ToString().Trim() == CancelFlg_Before)
			{
				grdYoyakuInquiry[i, "JOTAI"] = JOTAI_ERASE; //消
				if (grdYoyakuInquiry(i, "ZASEKI_RESERVE_YOYAKU_FLG").ToString().Trim() == KbnValueY) //取消＆座席指定予約
				{
					grdYoyakuInquiry[i, "JOTAI"] = JOTAI_ERASE + JOTAI_KEN; //消指

					{
						grdYoyakuInquiry(i, "STATE").ToString().Trim = ""; //全てに該当するケース
						grdYoyakuInquiry[i, "JOTAI"] = JOTAI_ERASE + JOTAI_SHITEI + JOTAI_KEN; //消指券
					}
				}

				{
					grdYoyakuInquiry(i, "STATE").ToString().Trim = ""; //取消＆発券済
					grdYoyakuInquiry[i, "JOTAI"] = JOTAI_ERASE + JOTAI_KEN; //消券
				}
			} //2:発券後
			else if (grdYoyakuInquiry(i, "CANCEL_FLG").ToString().Trim() == CancelFlg_After)
			{
				grdYoyakuInquiry[i, "JOTAI"] = JOTAI_DELETE; //削
			}
			else
			{
				grdYoyakuInquiry[i, "JOTAI"] = JOTAI_OTHER; //"":上記以外
			}
			if (grdYoyakuInquiry(i, "CANCEL_FLG").ToString().Trim() == "")
			{
				//人数計
				intAdultNinzu += System.Convert.ToInt32(grdYoyakuInquiry(i, "ADULT_NINZU"));
				intJuniorNinzu += System.Convert.ToInt32(grdYoyakuInquiry(i, "JUNIOR_NINZU"));
				intChildNinzu += System.Convert.ToInt32(grdYoyakuInquiry(i, "CHILD_NINZU"));
				//グループ
				intGroupNinzu = System.Convert.ToInt32(grdYoyakuInquiry(i, "NINZU_KEI"));
				if (intGroupNinzu > intMaxGroupNinzu)
				{
					intMaxGroupNinzu = intGroupNinzu;
					Array.Resize(ref intNinzu, intMaxGroupNinzu + 1);
				}
				if (ReferenceEquals(intNinzu, null) == true)
				{
					Array.Resize(ref intNinzu, intMaxGroupNinzu + 1);
				}
				intNinzu[intGroupNinzu]++;
			}
		}
		//人数計
		this.txtAdultNinzu.Text = intAdultNinzu.ToString("#,###");
		this.txtJuniorNinzu.Text = intJuniorNinzu.ToString("#,###");
		this.txtChildNinzu.Text = intChildNinzu.ToString("#,###");
		this.txtTotalNinzu.Text = (intAdultNinzu + intJuniorNinzu + intChildNinzu).ToString("#,###");
		//列作成
		dt = new DataTable();
		dt.Columns.Add("SUU"); //数
		dt.Columns.Add("GP"); //グループ
		dt.Columns.Add("KEI"); //計
							   //グリッド初期化
		this.grdGroup.DataSource = dt;
		//グループ
		if (intMaxGroupNinzu > 0)
		{
			for (i = 0; i <= intMaxGroupNinzu; i++)
			{
				if (intNinzu[i] > 0)
				{
					drX = dt.NewRow;
					drX["SUU"] = i;
					drX["GP"] = intNinzu[i];
					drX["KEI"] = i * intNinzu[i];
					intGpKei += intNinzu[i]; //合計行（GP）用加算
					intNinzuKei += System.Convert.ToInt32(i * intNinzu[i]); //合計行（数）用加算
					dt.Rows.Add(drX);
				}
			}
		}
		if (intGpKei > 0)
		{
			drX = dt.NewRow;
			dt.Rows.Add(drX); //空行
			drX = dt.NewRow;
			drX["SUU"] = "合計";
			drX["GP"] = intGpKei;
			drX["KEI"] = intNinzuKei;
			dt.Rows.Add(drX);
		}

		this.grdGroup.DataSource = dt;
		this.grdGroup.Refresh();
		string formatedCount = string.Empty;
		if (dataYoyaku_Info_Basic.Rows.Count > MaxKensu)
		{
			formatedCount = System.Convert.ToString((MaxKensu).ToString().PadLeft(6));
		}
		else
		{
			formatedCount = System.Convert.ToString((dataYoyaku_Info_Basic.Rows.Count).ToString().PadLeft(6));
		}
		this.lblLengthGrd02.Text = formatedCount + "件";
		this.grdYoyakuInquiry.ResumeLayout();
		this.grdYoyakuInquiry.Refresh();
	}

	/// <summary>
	/// 検索処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckSearch()
	{
		DateTime nowDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		object CheckBoxs = this.gbxSearchKoumoku.Controls.OfType(Of CheckBoxEx);
		Hashtable paramInfoList = new Hashtable();
		DataTable dt = new DataTable();
		S02_0401Da dataAccess = new S02_0401Da();
		S02_0101Da dataAccess2 = new S02_0101Da();

		//'前回エラークリア
		//Me.dtmSyuptDay.ExistError = False
		//Me.ucoCrsCd.ExistError = False
		//Me.ucoJyosyaTiCd.ExistError = False
		//Me.dtmSyuptTime.ExistError = False
		//Me.ucoKoshakasho.ExistError = False
		//出発日
		object with_1 = this.dtmSyuptDay;
		if (ReferenceEquals(with_1.Value, null) == true)
		{
			with_1.ExistError = true;
			with_1.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_023", "出発日"); //出発日が入力されていません。
			return false;
		}
		//日付不整合
		if (ReferenceEquals(with_1.Value, null) == false && Information.IsDate(with_1.Value) == false)
		{
			with_1.ExistError = true;
			with_1.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_016", "日付"); //日付が不正です。
			return false;
		}
		//2019/07/02 過去日チェック廃止により、下記ソースコメント化
		//'過去日付
		//If IsDate(.Value) = True AndAlso .Text < nowDate.ToShortDateString.Substring(2) Then
		//    .ExistError = True
		//    .Focus()
		//    CommonProcess.createFactoryMsg().messageDisp("E90_013") '過去日は指定できません。
		//    Return False
		//End If
		//コースコード、降車ヶ所
		//コースコード、降車ヶ所が共に入力済の場合、エラー

		{
			string.IsNullOrEmpty(System.Convert.ToString(this.ucoKoshakasho.CodeText)) = System.Convert.ToBoolean(false);
			this.ucoCrsCd.ExistError = true;
			this.ucoKoshakasho.ExistError = true;
			this.ucoCrsCd.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_047", "コースコード、降車ヶ所、双方同時に指定は");
			return false;
		}
		//時間不整合
		if (dtmSyuptTime.DisplayText != dtmSyuptTime.Text && Information.IsDate(dtmSyuptTime.Text) == false)
		{
			dtmSyuptTime.ExistError = true;
			dtmSyuptTime.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_016", "時間"); //時間が不正です。
			return false;
		}

		//コースコード
		object with_2 = this.ucoCrsCd;
		blnCourseAssigne = false;
		if (string.IsNullOrEmpty(System.Convert.ToString(with_2.CodeText)) == false)
		{
			paramInfoList.Add("CRS_CD", with_2.CodeText);
			try
			{
				dt = dataAccess2.AccessCourseDaityo(S02_0101Da.AccessType.courseByPrimaryKey, paramInfoList);
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, this.Name);
			}
			catch (Exception ex)
			{
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
				throw;
			}
			if (dt.Rows.Count < 1)
			{
				blnCourseAssigne = false; //コースコードFull桁指定なし
										  //該当するコードがマスタにに存在しません。
				with_2.ValueText = "";
				with_2.ExistError = true;
				with_2.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_062", "該当するコードがマスタ", "コース台帳");
				return false;
			}
			else
			{
				if (with_2.CodeText.ToString().Trim() == dt.Rows(0).Item(0).ToString().Trim())
				{
					blnCourseAssigne = true; //コースコードFull桁指定なし
					with_2.ValueText = dt.Rows(0).Item("CRS_NAME_RK").ToString();
				}
				else
				{
					with_2.ValueText = "";
				}
			}
		}
		//乗車地 検索項目	乗車地	"・乗車地がFull桁指定されていて、場所マスタに未登録の場合
		//「該当するコードがマスタにありません」
		object with_3 = this.ucoJyosyaTiCd;
		if (string.IsNullOrEmpty(System.Convert.ToString(with_3.CodeText)) == false)
		{
			paramInfoList.Add("PLACE_CD", with_3.CodeText);
			try
			{
				dt = new DataTable();
				dt = dataAccess2.GetPlaceMasterData(paramInfoList);
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, this.Name);
			}
			catch (Exception ex)
			{
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
				throw;
			}
			if (dt.Rows.Count < 1)
			{
				with_3.ValueText = "";
				with_3.ExistError = true;
				with_3.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_062", "該当するコードがマスタ", "場所マスタ");
				with_3.ValueText = "";
				return false;
			}
			else
			{
				with_3.ValueText = dt.Rows(0).Item("PLACE_NAME_1").ToString().Trim();
			}
		}
		//降車ヵ所がFull桁指定されていて、仕入先マスタに未登録の場合
		//「該当するコードがマスタにありません」
		object with_4 = this.ucoKoshakasho;
		if (string.IsNullOrEmpty(System.Convert.ToString(with_4.CodeText)) == false)
		{
			paramInfoList.Add("SiireSakiCd", with_4.CodeText);
			Oracle.ManagedDataAccess.Client.OracleConnection con = null;
			Master.SiireSaki_DA SiireSakiDa = new Master.SiireSaki_DA();
			try
			{
				dt = new DataTable();
				dt = dataAccess.GetSiiresaki_KousyaPlace(paramInfoList);
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, this.Name);
			}
			catch (Exception ex)
			{
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
				throw;
			}

			if (dt.Rows.Count < 1)
			{
				with_4.ValueText = "";
				with_4.ExistError = true;
				with_4.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_062", "該当するコードがマスタ", "仕入先マスタ");
				with_4.ValueText = "";
				return false;
			}
			else
			{
				with_4.ValueText = dt.Rows(0).Item("SIIRE_SAKI_NAME").ToString().Trim();
			}
		}
		return true;
	}

	/// <summary>
	/// 検索処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckUtiwake()
	{

		if (grdCrs.Row < 0)
		{
			//コースが選択されていません。</MsgString>
			this.grdCrs.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_024", "コース");
			return false;
		}
		return true;

	}

	public void setSeFirsttDisplayData()
	{
		throw (new NotImplementedException());
	}

	public void DisplayDataToEntity(ref object ent)
	{
		throw (new NotImplementedException());
	}

	public void EntityDataToDisplay(ref object ent)
	{
		throw (new NotImplementedException());
	}

	public void OldDataToEntity(DataRow pDataRow)
	{
		throw (new NotImplementedException());
	}

	public bool CheckInsert()
	{
		throw (new NotImplementedException());
	}

	public bool CheckUpdate()
	{
		throw (new NotImplementedException());
	}

	public bool isExistHissuError()
	{
		throw (new NotImplementedException());
	}

	#endregion

}