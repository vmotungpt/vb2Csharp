/// <summary>
/// 予約一覧照会
/// </summary>
public class S02_0301 : PT11, iPT11
{

	#region 定数／変数
	// "変数"
	private bool blnYoyakuNoAssigne = false; //予約NOFull桁指定（True=指定,False=指定なし）
	private bool blnCourseAssigne = false; //コースコードFull桁指定（True=指定,False=指定なし）
	private int intChkKensu = 0; //検索チェック時の予約NO件数
								 // "定数"

	/// <summary>
	/// 画面名
	/// </summary>
	private const string ScreenName = "予約一覧照会";
	/// <summary>
	/// 画面モード
	/// </summary>
	public const string ScreenModeNew = "1"; //新規
	public const string ScreenModeEdit = "2"; //更新
	public const string ScreenModeReference = "3"; //参照
	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	public const int TopGbxCondition = 41;
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

	/// <summary>
	/// キャンセルフラグ
	/// </summary>
	public const string CancelFlg_Before = "1"; //発券前
	public const string CancelFlg_After = "2"; //発券後

	/// <summary>
	/// 状況
	/// </summary>
	private const string JOKYO_BLANK = ""; //ブランク
	private const string JOKYO_SHITEI = "指"; //指定
	private const string JOKYO_KEN = "券"; //券
	private const string JOKYO_JTB = "Ｊ"; //JTB
	private const string JOKYO_REN = "連"; //連
	private const string JOKYO_KNT = "Ｋ"; //KNT(近畿日本ﾂｰﾘｽﾄ)
	private const string JOKYO_ERASE = "消"; //消
	private const string JOKYO_DELETE = "削"; //削
	private const string JOKYO_OTHER = ""; //上記以外

	//CSV出力用
	/// <summary>
	/// 列番号(グリッド) 詳細ボタン
	/// </summary>
	private const int NoColBtn = 1;

	/// <summary>
	/// CSV列数
	/// </summary>
	private const int MaxCsvCol = 19;

	/// <summary>
	/// CSVファイル名(デスクトップ\部署_日付_帳票名)
	/// </summary>
	private const string CsvFileName = "{0}\\{1}_{2}_予約確認リスト.csv";

	#endregion

	#region プロパティ

	// "パブリック プロパティ"
	//
	/// <summary>
	/// パラメータクラス
	/// </summary>
	public S02_0301ParamData ParamData
	{

		/// <summary>
		/// 条件GroupBoxの高さ
		/// </summary>
		/// <returns></returns>
	public int HeightGbxCondition
	{
		get
		{
			return this.gbxCondition.Height;
		}
		set
		{
			this.gbxCondition.Height = value;
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
			return this.gbxCondition.Visible;
		}
		set
		{
			this.gbxCondition.Visible = value;
		}
	}
	#endregion

	#region イベント
	/// <summary>
	/// 画面ロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S02_0301_Load(object sender, EventArgs e)
	{
		base.closeFormFlg = false; //クローズ確認フラグ
	}

	/// <summary>
	/// 画面を閉じる
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S02_0301_FormClosing(object sender, FormClosingEventArgs e)
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
	/// F4(CSV出力)ボタン押下時
	/// </summary>
	protected override void btnF4_ClickOrgProc()
	{
		DateTime nowDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		if (this.grdYoyakuListInquiry.Rows.Count <= 1)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_004"); //処理対象データがありません。
			return;
		}
		//CSV出力
		CSVOut csvOutPut = new CSVOut();
		//CSVファイル名(デスクトップ \ 部署_日付_帳票名)を生成してセット
		csvOutPut.StartCSVFileEdit(string.Format(CsvFileName, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), UserInfoManagement.sectionBusyoName, nowDate.ToString("yyyyMMddHHmm")));

		//グリッド内のデータをファイルに出力する
		for (row = 0; row <= grdYoyakuListInquiry.Rows.Count - 1; row++)
		{
			for (col = 1; col <= MaxCsvCol; col++)
			{

				//詳細ボタン列は出力しない
				if (col == NoColBtn)
				{
					col = col + 1;
				}

				csvOutPut.AppendCSVData(grdYoyakuListInquiry.Item(row, col));
			}

			//次の予約データを出力する
			csvOutPut.EditNextCourse();
		}

		//バッファ内のデータをファイルに出力する
		if (csvOutPut.WriteCSVFile() == true)
		{
			// 処理成功時
			CommonProcess.createFactoryMsg().messageDisp("I90_002", "CSV出力");
			// log出力
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.CSVOut, CsvFileName, "CSV出力");
		}
		else
		{
			// 失敗時
			CommonProcess.createFactoryMsg().messageDisp("E90_028", "CSV出力");
			// log出力
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.CSVOut, CsvFileName, "CSV出力");
		}

	}

	/// <summary>
	/// F8ボタン押下イベント
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
			this.grdYoyakuListInquiry.Height -= this.HeightGbxCondition - 3;
		}
		else
		{
			//非表示状態
			this.btnVisiblerCondition.Text = "表示 >>";

			this.PanelEx1.Top = TopGbxCondition;
			this.PanelEx1.Height += this.HeightGbxCondition + MarginGbxCondition;
			this.grdYoyakuListInquiry.Height += this.HeightGbxCondition - 3;
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
	/// キーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void S02_0301_KeyDown(object sender, KeyEventArgs e)
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
				else if ((string)cControl.Name == "ucoDairitenName")
				{
					//【代理店検索】画面へ遷移
					//TODO:共通処理：遷移先画面未完成
					//Using form As New S90_0104()
					//    Dim prm As New S90_0104ParamData()
					//    prm.DairitenCd = ucoDairitenName.CodeText.ToString.Trim
					//    form.ParamData = prm    '画面間パラメータセット
					//    form.ShowDialog()
					//End Using
				}
				else if ((string)cControl.Name == "txtDairitenTEL")
				{
					//'【代理店検索】画面へ遷移
					//TODO:共通処理：遷移先画面未完成
					//Using form As New S90_0104()
					//    Dim prm As New S90_0104ParamData()
					//    prm.TelNo = txtDairitenTEL.Text
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
		else
		{
			return;
		}
	}

	/// <summary>
	/// 詳細ボタン押下イベント              'TODO:遷移先画面未完成
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdYoyakuListInquiry_CellButtonClick()
	{
			)grdYoyakuListInquiry.CellButtonClick;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		DataTable dt = new DataTable();
		S02_0301Da dataAccess = new S02_0301Da();
		DateTime nowDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		string strSysDate = System.Convert.ToString((nowDate.Year * 10000 + nowDate.Month * 100 + nowDate.Day).ToString().Trim());
		int intSysTime = nowDate.Hour * 100 + nowDate.Minute;

		object with_1 = grdYoyakuListInquiry;
		paramInfoList.Add("YoyakuNo", System.Convert.ToInt32(with_1.Rows(e.Row).Item("YOYAKU_NO")));
		paramInfoList.Add("YoyakuKbn", with_1.Rows(e.Row).Item("YOYAKU_KBN").ToString().Trim());
		try
		{
			dt = dataAccess.AccessYoyakuIList(S02_0301Da.AccessType.yoyakuInfoByPrimaryKey, paramInfoList);

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
			this.ucoYoyakuNo.ExistError = true;
			//該当する予約情報が存在しません。
			this.ucoYoyakuNo.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_014", "予約情報");
			return;
		}

		//予約登録画面に遷移
		using (S02_0103 form = new S02_0103())
		{
			S02_0103ParamData prm = new S02_0103ParamData();
			prm.ScreenMode = ScreenModeEdit; //編集モード
											 //prm.CrsCd = .Rows(e.Row).Item("CRS_CD").ToString.Trim
											 //prm.Gousya = CInt(.Rows(e.Row).Item("GOUSYA").ToString.Trim)
											 //prm.CrsKind = .Rows(e.Row).Item("CRS_KIND").ToString.Trim
											 //prm.SyuptDay = CInt(.Rows(e.Row).Item("SYUPT_DAY").ToString.Trim)
											 //prm.SyuptTime = CInt(.Rows(e.Row).Item("SYUPT_TIME").ToString.Trim)
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
		this.F4Key_Visible = true; // F4;未使用
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

		this.F4Key_Text = "F4:CSV出力";

	}

	// 初期化用のメソッド

	/// <summary>
	/// [検索エリア]検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{
		S02_0301Da dataAccess = new S02_0301Da(); //DataAccessクラス生成
		Hashtable paramInfoList = new Hashtable(); //DBパラメータ
		DataTable returnValue = new DataTable(); //戻り値
		DateTime nowDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());

		//条件GroupBox内のテキストボックス初期化
		object textBoxs = this.gbxCondition.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
		}

		//コードコントロール初期化
		object CodeControls = this.gbxCondition.Controls.OfType(Of Master.CodeControlEx);
		foreach ( in CodeControls)
		{
			CodeControl.CodeText = "";
			CodeControl.ValueText = "";
			CodeControl.ExistError = false;
		}

		//チェックボックス初期化
		object CheckBoxs = this.gbxCondition.Controls.OfType(Of CheckBoxEx);
		foreach ( in CheckBoxs)
		{
			CheckBox.Checked = false;
			CheckBox.ExistError = false;
		}
		this.chkCancelWith.Checked = true;
		this.chkHakkenAlreadyWith.Checked = true;

		//日本語／外国語コース
		if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
		{
			//ユーザーが国際事業部の場合は外国語
			chkGaikokugoCrs.Checked = true;
			chkJapaneseCrs.Checked = false;
		}
		else
		{
			//それ以外の場合は日本語をONに設定
			chkJapaneseCrs.Checked = true;
			chkGaikokugoCrs.Checked = false;
		}

		//グリッドの初期化
		DataTable dt = new DataTable();
		this.grdYoyakuListInquiry.DataSource = dt;
		this.SetgrdYoyakuListInquiry();
		this.lblLengthGrd.Text = "";
		this.grdYoyakuListInquiry.DataMember = "";
		this.grdYoyakuListInquiry.Refresh();

		//他コントロールの初期化
		this.lblLengthGrd.Text = "     0件";
		this.dtmSyuptDayFromTo.FromDateText = nowDate;
		this.dtmSyuptDayFromTo.ToDateText = nowDate;
		this.dtmUketukeDay.Value = DateTime.Now;
		this.ucoYoyakuNo.YoyakuText = "";
		this.ucoCrsCd.CharLength = 40;
		returnValue = null;
		dataAccess = null;
		paramInfoList = null;
	}

	/// <summary>
	/// エラー有無のクリア
	/// </summary>
	private void clearError()
	{
		// ExistErrorプロパティのクリア
		ucoYoyakuNo.ExistError = false;
		dtmSyuptDayFromTo.ExistErrorForFromDate = false;
		dtmSyuptDayFromTo.ExistErrorForToDate = false;
		//ucoNoribaCd.ExistError = False
		ucoCrsCd.ExistError = false;
		chkTeikiNoon.ExistError = false;
		chkTeikiNight.ExistError = false;
		chkTeikiKogai.ExistError = false;
		chkCapital.ExistError = false;
		chkKikakuStay.ExistError = false;
		chkKikakuTonaiR.ExistError = false;

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
		this.SetgrdYoyakuListInquiry();
		this.grdYoyakuListInquiry.DataSource = dt;
		this.grdYoyakuListInquiry.DataMember = "";
		this.grdYoyakuListInquiry.Refresh();
		this.grdYoyakuListInquiry.Text = "     0件";
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

		GetgrdYoyakuListInquiry();

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
		if (dtmSyuptDayFromTo.FromDateText IsNot null && ReferenceEquals(dtmSyuptDayFromTo.ToDateText, null))
			{
			//日付From <> ブランク かつ 日付To = ブランクの場合、日付To に 日付From の値をセット
			dtmSyuptDayFromTo.ToDateText = dtmSyuptDayFromTo.FromDateText;

		}
			else if (dtmSyuptDayFromTo.ToDateText IsNot null && ReferenceEquals(dtmSyuptDayFromTo.FromDateText, null))
			{
			//日付To <> ブランク かつ 日付From = ブランクの場合、日付From に 日付To の値をセット
			dtmSyuptDayFromTo.FromDateText = dtmSyuptDayFromTo.ToDateText;
		}
	}

	/// <summary>
	/// 予約一覧照会グリッドの設定
	/// </summary>
	private void SetgrdYoyakuListInquiry()
	{
		DataTable dt = new DataTable();
		object with_1 = grdYoyakuListInquiry;

		//グリッド初期化
		this.grdYoyakuListInquiry.AllowAddNew = false;
		this.grdYoyakuListInquiry.DataSource = dt;
		this.grdYoyakuListInquiry.DataMember = "";
		this.grdYoyakuListInquiry.Refresh();
		this.lblLengthGrd.Text = "     0件";
		this.grdYoyakuListInquiry.SuspendLayout();
		//.AllowAddNew = True
		with_1.Cols.Count = 32;
		with_1.Cols.Frozen = 7;
		//.Rows.Count = 50
		with_1.Rows(0).AllowMerging = true;

		//非表示部分
		//For intCnt As Byte = 20 To 31
		for (byte intCnt = 21; intCnt <= 31; intCnt++)
		{
			with_1.Cols(intCnt).Visible = false;
		}

		with_1.AllowAddNew = false;
	}

	/// <summary>
	/// 予約一覧グリッド表示値の設定
	/// </summary>
	private void GetgrdYoyakuListInquiry()
	{
		//予約NO
		string yoyakuNo = System.Convert.ToString(this.ucoYoyakuNo.YoyakuText);
		string yoyakuKbn = string.Empty;
		int yoyakuNum = 0;

		DateTime dteTmpDate = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		string strDate = string.Empty;
		string strFromDate = string.Empty;
		string strToDate = string.Empty;
		string strSysDate = string.Empty;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		DataTable dt = new DataTable();

		//'グリッド初期化
		this.grdYoyakuListInquiry.AllowAddNew = false;
		this.grdYoyakuListInquiry.DataSource = dt;
		this.grdYoyakuListInquiry.DataMember = "";
		this.grdYoyakuListInquiry.Refresh();
		this.lblLengthGrd.Text = "     0件";
		this.grdYoyakuListInquiry.SuspendLayout();
		//パラメータ設定

		//予約NO
		if (yoyakuNo.Length > 9)
		{
			//予約NO
			// 予約NOを予約区分、予約番号に分解
			yoyakuKbn = yoyakuNo.Substring(0, 1);
			// 予約Noが9桁を超える場合、予約番号部を取り出す
			int.TryParse(yoyakuNo.Substring(1), out yoyakuNum);
		}
		else
		{
			int.TryParse(yoyakuNo, out yoyakuNum);
		}
		paramInfoList.Add("YoyakuNo", yoyakuNum);
		paramInfoList.Add("YoyakuKbn", yoyakuKbn);
		paramInfoList.Add("YoyakuNoAssigne", blnYoyakuNoAssigne); //予約NO検索用（指定コードFull桁判定）
																  //姓名
		paramInfoList.Add("Surname", Strings.Trim(System.Convert.ToString(this.txtSurname.Text)));
		paramInfoList.Add("Name", Strings.Trim(System.Convert.ToString(this.txtName.Text)));
		//TEL
		paramInfoList.Add("TelNo", Strings.Trim(System.Convert.ToString(this.txtTEL.Text)));
		//出発日
		dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText));
		strFromDate = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
		dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(dtmSyuptDayFromTo.ToDateText));
		strToDate = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
		paramInfoList.Add("SyuptDayFrom", strFromDate);
		paramInfoList.Add("SyuptDayTo", strToDate);
		//コースコード
		paramInfoList.Add("CrsCd", Strings.Trim(System.Convert.ToString(this.ucoCrsCd.CodeText)));
		paramInfoList.Add("CrsCdAssigne", blnCourseAssigne); //コースコード検索用（指定コードFull桁判定）
															 //乗車地
		paramInfoList.Add("JyosyaTiCd", Strings.Trim(System.Convert.ToString(ucoJyosyaTiCd.CodeText)));
		//コース区分
		//   日本語コース
		paramInfoList.Add("JapaneseCrs", Strings.Trim(System.Convert.ToString(this.chkJapaneseCrs.Checked.ToString())));
		//   外国語コース
		paramInfoList.Add("GaikokugoCrs", Strings.Trim(System.Convert.ToString(this.chkGaikokugoCrs.Checked.ToString())));
		//   定期（昼）
		paramInfoList.Add("TeikiNoon", Strings.Trim(System.Convert.ToString(this.chkTeikiNoon.Checked.ToString())));
		//   定期（夜）
		paramInfoList.Add("TeikiNight", Strings.Trim(System.Convert.ToString(this.chkTeikiNight.Checked.ToString())));
		//   定期（郊外）
		paramInfoList.Add("TeikiKogai", Strings.Trim(System.Convert.ToString(this.chkTeikiKogai.Checked.ToString())));
		//   企画（日帰り）
		paramInfoList.Add("KikakuDayTrip", Strings.Trim(System.Convert.ToString(this.chkKikakuDayTrip.Checked.ToString())));
		//   企画（宿泊）
		paramInfoList.Add("KikakuStay", Strings.Trim(System.Convert.ToString(this.chkKikakuStay.Checked.ToString())));
		//   企画（都内R）
		paramInfoList.Add("KikakuTonaiR", Strings.Trim(System.Convert.ToString(this.chkKikakuTonaiR.Checked.ToString())));
		//   キャピタル
		paramInfoList.Add("Capital", Strings.Trim(System.Convert.ToString(this.chkCapital.Checked.ToString())));
		//予約受付日

		if (Information.IsDate(this.dtmUketukeDay.Text) == true)
		{
			dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(this.dtmUketukeDay.Text));
			strDate = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
			paramInfoList.Add("YoyakuDay", strDate);
		}
		//担当者
		paramInfoList.Add("Tantosya", Strings.Trim(System.Convert.ToString(this.txtTantosya.Text)));
		//申込者
		paramInfoList.Add("Yykmks", Strings.Trim(System.Convert.ToString(this.txtYykmks.Text)));
		//代理店
		paramInfoList.Add("AgentCd", Strings.Trim(System.Convert.ToString(this.ucoDairitenName.CodeText)));
		//代理店TEL
		paramInfoList.Add("AgentTel", Strings.Trim(System.Convert.ToString(this.txtDairitenTEL.Text)));
		//予約種別
		//   予約
		paramInfoList.Add("Yoyaku", Strings.Trim(System.Convert.ToString(this.chkYoyaku.Checked.ToString())));
		//   WT
		paramInfoList.Add("WT", Strings.Trim(System.Convert.ToString(this.chkWT.Checked.ToString())));
		//   リクエスト
		paramInfoList.Add("Request", Strings.Trim(System.Convert.ToString(this.chkRequest.Checked.ToString())));
		//状態
		//   キャンセル含む
		paramInfoList.Add("CancelWith", Strings.Trim(System.Convert.ToString(this.chkCancelWith.Checked.ToString())));
		//   発券済含む
		paramInfoList.Add("HakkenAlreadyWith", Strings.Trim(System.Convert.ToString(this.chkHakkenAlreadyWith.Checked.ToString())));

		// Web予約作成ゴミデータ削除処理
		S02_0301Da dataAccess = new S02_0301Da();
		dataAccess.updateWebWastedData();

		DataTable dataYoyaku_Info_Basic = dataAccess.AccessYoyakuIList(S02_0301Da.AccessType.yoyakuInfoByHeaderKey, paramInfoList);

		if (dataYoyaku_Info_Basic.Rows.Count == 0)
		{
			// 取得件数0件の場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_004");
			this.grdYoyakuListInquiry.ResumeLayout();
			return;
		}
		grdYoyakuListInquiry.DataSource = dataYoyaku_Info_Basic;

		//状況セット
		for (i = 1; i <= grdYoyakuListInquiry.Rows.Count - 1; i++)
		{
			grdYoyakuListInquiry[i, "SITUATION"] = JOKYO_BLANK;
			//キャンセルフラグ(1: 発券前 2: 発券後)
			if ((string)(grdYoyakuListInquiry(i, "CANCEL_FLG").ToString().Trim()) == "")
			{
				//発券内容
				if (grdYoyakuListInquiry(i, "HAKKEN_NAIYO").ToString().Trim() == "")
				{
					//座席指定予約フラグ
					if (grdYoyakuListInquiry(i, "ZASEKI_RESERVE_YOYAKU_FLG").ToString().Trim() == "")
					{
						grdYoyakuListInquiry[i, "SITUATION"] = JOKYO_BLANK; //ブランク
					}
					else if (grdYoyakuListInquiry(i, "ZASEKI_RESERVE_YOYAKU_FLG").ToString().Trim() == "Y")
					{
						grdYoyakuListInquiry[i, "SITUATION"] = JOKYO_SHITEI; //指
					}
				}
				else
				{
					//状態STATE
					if ((string)(grdYoyakuListInquiry(i, "STATE").ToString().Trim()) == "")
					{
						grdYoyakuListInquiry[i, "SITUATION"] = JOKYO_KEN; //券
					}
					else if ((string)(grdYoyakuListInquiry(i, "STATE").ToString().Trim()) == "1")
					{
						grdYoyakuListInquiry[i, "SITUATION"] = JOKYO_JTB; //Ｊ  JTB
					}
					else if ((string)(grdYoyakuListInquiry(i, "STATE").ToString().Trim()) == "2")
					{
						grdYoyakuListInquiry[i, "SITUATION"] = JOKYO_REN; //連
					}
					else if ((string)(grdYoyakuListInquiry(i, "STATE").ToString().Trim()) == "3")
					{
						grdYoyakuListInquiry[i, "SITUATION"] = JOKYO_KNT; //Ｋ  KNT(近畿日本ﾂｰﾘｽﾄ)
					}
				}
			} //1:発券前
			else if (grdYoyakuListInquiry(i, "CANCEL_FLG").ToString().Trim() == CancelFlg_Before)
			{
				grdYoyakuListInquiry[i, "SITUATION"] = JOKYO_ERASE; //消
			} //2:発券後
			else if (grdYoyakuListInquiry(i, "CANCEL_FLG").ToString().Trim() == CancelFlg_After)
			{
				grdYoyakuListInquiry[i, "SITUATION"] = JOKYO_DELETE; //削
			}
			else
			{
				grdYoyakuListInquiry[i, "SITUATION"] = JOKYO_OTHER; //"" 上記以外
			}
		}
		string formatedCount = string.Empty;
		if (dataYoyaku_Info_Basic.Rows.Count > MaxKensu)
		{
			formatedCount = System.Convert.ToString((MaxKensu).ToString().PadLeft(6));
		}
		else
		{
			formatedCount = System.Convert.ToString((dataYoyaku_Info_Basic.Rows.Count).ToString().PadLeft(6));
		}
		this.lblLengthGrd.Text = formatedCount + "件";
		if (dataYoyaku_Info_Basic.Rows.Count > MaxKensu)
		{
			// 取得件数が設定件数より多い場合、メッセージを表示
			grdYoyakuListInquiry.Rows.Remove(MaxKensu + 1);
			CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が");
		}
		this.grdYoyakuListInquiry.Refresh();
		this.grdYoyakuListInquiry.ResumeLayout();
		//予約番号を入力して検索した場合、該当する予約情報がある場合は一覧表示後、予約登録画面に遷移
		//If yoyakuNo.Length > 8 AndAlso
		//  (blnYoyakuNoAssigne = True Or
		//   dataYoyaku_Info_Basic.Rows.Count = 1) Then
		if (intChkKensu == 1)
		{
			object with_1 = grdYoyakuListInquiry;
			using (S02_0103 form = new S02_0103())
			{
				S02_0103ParamData prm = new S02_0103ParamData();
				prm.ScreenMode = ScreenModeEdit; //編集モード
												 //prm.CrsCd = .Rows(1).Item("CRS_CD").ToString.Trim
												 //prm.Gousya = CInt(.Rows(1).Item("GOUSYA").ToString.Trim)
												 //prm.CrsKind = .Rows(1).Item("CRS_KIND").ToString.Trim
												 //prm.SyuptDay = CInt(.Rows(1).Item("SYUPT_DAY").ToString.Trim)
				prm.YoyakuNo = System.Convert.ToInt32(with_1.Rows(1).Item("YOYAKU_NO"));
				prm.YoyakuKbn = with_1.Rows(1).Item("YOYAKU_KBN").ToString().Trim();
				form.ParamData = prm;
				form.ShowDialog();
			}

		}
	}

	/// <summary>
	/// 検索処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckSearch()
	{
		DateTime nowDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		object CheckBoxs = this.gbxCondition.Controls.OfType(Of CheckBoxEx);
		//予約NO
		string yoyakuNo = System.Convert.ToString(this.ucoYoyakuNo.YoyakuText);
		string yoyakuKbn = string.Empty;
		int yoyakuNum = 0;
		DataTable dtYoyaku = new DataTable();
		DataTable dt = new DataTable();
		Hashtable paramInfoList = new Hashtable();

		S02_0301Da dataAccess = new S02_0301Da();
		S02_0101Da dataAccess2 = new S02_0101Da();
		intChkKensu = 0;
		object with_1 = this.dtmSyuptDayFromTo;
		//前回エラークリア
		//Me.ucoYoyakuNo.ExistError = False
		//.ExistErrorForFromDate = False
		//.ExistErrorForToDate = False
		//Me.ucoCrsCd.ExistError = False
		//Me.ucoJyosyaTiCd.ExistError = False
		//Dim CodeControls = Me.gbxCondition.Controls.OfType(Of Master.CodeControlEx)
		//For Each CodeControl In CodeControls
		//    CodeControl.ExistError = False
		//Next
		//For Each CheckBox In CheckBoxs
		//    CheckBox.ExistError = False
		//Next

		if (yoyakuNo.Length > 9)
		{
			//予約NO
			// 予約NOを予約区分、予約番号に分解
			yoyakuKbn = yoyakuNo.Substring(0, 1);
			// 予約Noが9桁を超える場合、予約番号部を取り出す
			int.TryParse(yoyakuNo.Substring(1), out yoyakuNum);
		}
		else
		{
			int.TryParse(yoyakuNo, out yoyakuNum);
		}
		blnYoyakuNoAssigne = false;
		if (string.IsNullOrEmpty(System.Convert.ToString(this.ucoYoyakuNo.YoyakuText)) == false)
		{
			paramInfoList.Add("YoyakuNo", yoyakuNum);
			paramInfoList.Add("YoyakuKbn", yoyakuKbn);
			try
			{
				dtYoyaku = dataAccess.AccessYoyakuIList(S02_0301Da.AccessType.yoyakuInfoByPrimaryKey, paramInfoList);
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, this.Name);
			}
			catch (Exception ex)
			{
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
				throw;
			}
			if (dtYoyaku.Rows.Count < 1)
			{
				blnYoyakuNoAssigne = false; //予約NOFull桁指定
				this.ucoYoyakuNo.ExistError = true;
				//該当する予約情報が存在しません。
				this.ucoYoyakuNo.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_014", "予約情報");
				return false;
			}
			else
			{
				intChkKensu = System.Convert.ToInt32(dtYoyaku.Rows.Count);
				blnYoyakuNoAssigne = true; //予約NOFull桁指定なし
										   //If dtYoyaku.Rows(0).Item("YOYAKU_NO").ToString.Trim = yoyakuNum.ToString.Trim And
										   //   dtYoyaku.Rows(0).Item("YOYAKU_KBN").ToString.Trim = yoyakuKbn Then
										   //    blnYoyakuNoAssigne = True '予約NOFull桁指定なし
										   //End If
				return true;
				//'予約Noか出発日どとらも指定されていない場合
				//If String.IsNullOrEmpty(Me.ucoYoyakuNo.YoyakuText) = True And
				//    String.IsNullOrEmpty(Me.txtSurname.Text) = True And
				//    String.IsNullOrEmpty(Me.txtName.Text) = True Then
				//    Me.ucoCrsCd.Focus()
				//    CommonProcess.createFactoryMsg().messageDisp("E90_011", "'予約NOか出発日のどちらか")
				//    Return False
				//End If
			}
		}

		//日付未入力
		//予約Noか出発日どとらも指定されていない場合

		{
			string.IsNullOrEmpty(System.Convert.ToString(this.ucoYoyakuNo.YoyakuText)) = System.Convert.ToBoolean(true);
			ucoYoyakuNo.ExistError = true;
			this.ucoYoyakuNo.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_011", "予約NOか出発日のどちらか");
			return false;
		}
		if (with_1.FromDateText.HasValue == false)
		{
			with_1.FromDateText = nowDate;
		}
		if (with_1.ToDateText.HasValue == false)
		{
			with_1.ToDateText = with_1.FromDateText;
		}
		//日付のFromToに入力があり
		//   From　>　To　の場合
		if (with_1.FromDateText.HasValue == true && with_1.ToDateText.HasValue == true)
		{
			if (with_1.FromDateText > with_1.ToDateText)
			{
				with_1.ExistErrorForToDate = true;
				with_1.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_016", "日付のFromTo");
				return false;
			}
		}

		//コースコード、乗車地、氏名のうち１つは入力                                  2018/03/06 チェック廃止
		//If String.IsNullOrEmpty(Me.ucoCrsCd.CodeText) = True And
		//   String.IsNullOrEmpty(Me.ucoJyosyaTiCd.CodeText) = True And
		//   String.IsNullOrEmpty(Me.txtName.Text) = True And
		//   String.IsNullOrEmpty(Me.txtSurname.Text) = True Then
		//    Me.ucoCrsCd.ExistError = True
		//    Me.ucoCrsCd.Focus()
		//    CommonProcess.createFactoryMsg().messageDisp("E90_011", "コースコード、乗車地、氏名のうち１つは指定")

		//    Return False
		//End If

		//コースコード
		blnCourseAssigne = false;
		if (string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)) == false)
		{
			paramInfoList.Add("CRS_CD", this.ucoCrsCd.CodeText);

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
				this.ucoCrsCd.ExistError = true;
				//コードがマスタにに存在しません。
				this.ucoCrsCd.ValueText = "";
				this.ucoCrsCd.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_062", "該当するコードがマスタ", " ");
				return false;
			}
			else
			{
				if (this.ucoCrsCd.CodeText.ToString().Trim() == dt.Rows(0).Item(0).ToString().Trim())
				{
					blnCourseAssigne = true; //コースコードFull桁指定なし
					this.ucoCrsCd.ValueText = dt.Rows(0).Item("CRS_NAME_RK").ToString();
				}
				else
				{
					this.ucoCrsCd.ValueText = "";
				}
			}
		}

		//乗車地 検索項目	乗車地	"・乗車地がFull桁指定されていて、場所マスタに未登録の場合
		//「該当するコードがマスタにありません」"
		if (string.IsNullOrEmpty(System.Convert.ToString(this.ucoJyosyaTiCd.CodeText)) == false)
		{
			paramInfoList.Add("PLACE_CD", this.ucoJyosyaTiCd.CodeText);
			dt = new DataTable();
			try
			{
				dt = dataAccess2.GetPlaceMasterDataCodeData(false, "PLACE_CD = '" + this.ucoJyosyaTiCd.CodeText.Trim() + "' ");
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, this.Name);
			}
			catch (Exception ex)
			{
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
				throw;
			}

			if (dt.Rows.Count < 1)
			{
				this.ucoJyosyaTiCd.ValueText = "";
				this.ucoJyosyaTiCd.ExistError = true;
				this.ucoJyosyaTiCd.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_062", "該当するコードがマスタ", " ");
				this.ucoJyosyaTiCd.ValueText = "";
				return false;
			}
			else
			{
				this.ucoJyosyaTiCd.ValueText = dt.Rows(0).Item("PLACE_NAME_1").ToString().Trim();
			}
		}
		if (string.IsNullOrEmpty(System.Convert.ToString(this.dtmUketukeDay.Text)) == false && ReferenceEquals(this.dtmUketukeDay.Value, null) == false)
		{
			if (Information.IsDate(this.dtmUketukeDay.Text) == false)
			{
				this.dtmUketukeDay.ExistError = true;
				this.dtmUketukeDay.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_013"); //過去日は指定できません。
				return false;
			}
		}
		//代理店コード
		if (string.IsNullOrEmpty(System.Convert.ToString(ucoDairitenName.CodeText)) == false)
		{
			YoyakuBizCommonDa yoyakuBizCommonDa = new YoyakuBizCommonDa();
			DataTable dt3 = new DataTable();
			try
			{
				dt3 = yoyakuBizCommonDa.getAgentMaster(this.ucoDairitenName.CodeText);
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, this.Name);
			}
			catch (Exception ex)
			{
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
				throw;
			}
			if (dt3.Rows.Count < 1)
			{
				this.ucoDairitenName.ExistError = true;
				//代理店コードがマスタにに存在しません。
				this.ucoDairitenName.ValueText = "";
				this.ucoDairitenName.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_062", "該当するコードがマスタ", " ");
				return false;
			}
			else
			{
				if (this.ucoDairitenName.CodeText.ToString().Trim() == dt3.Rows(0).Item(0).ToString().Trim())
				{
					this.ucoDairitenName.ValueText = dt3.Rows(0).Item("AGENT_NAME").ToString();
				}
				else
				{
					this.ucoDairitenName.ValueText = string.Empty;
				}
				return true;
			}
		}
		return true;
	}

	public void setSeFirsttDisplayData()
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

	public void DisplayDataToEntity(ref object ent)
	{
		throw (new NotImplementedException());
	}

	public void EntityDataToDisplay(ref object ent)
	{
		throw (new NotImplementedException());
	}

	#endregion

}