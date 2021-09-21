using System.ComponentModel;


/// <summary>
///     過剰入金一覧
///     検索条件（予約日など）を指定して、過剰入金の予約情報を表示する。
/// </summary>
/// <remarks>
///    Author：2018/10/25//DTS佐藤
/// </remarks>
public class S02_1502 : PT11
{

	#region 定数/変数

	/// <summary>
	/// 画面ID
	/// </summary>
	private const string ScreenId = "S02_1502";
	/// <summary>
	/// 画面名
	/// </summary>
	private const string ScreenName = "過剰入金一覧";

	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	private const int TopGbxCondition = 41;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	private const int MarginGbxCondition = 6;

	/// <summary>
	/// 「日付」パラメーター値生成用(年算出)
	/// </summary>
	public const int calcYear = 10000;
	/// <summary>
	/// 「日付」パラメーター値生成用(月算出)
	/// </summary>
	public const int calcMonth = 100;

	/// <summary>
	/// 連絡状況（「未連絡」データ用文字列）
	/// </summary>
	public const string StrMiContact = "未";
	/// <summary>
	/// 列番号(グリッド) 連絡状況
	/// </summary>
	private const int NoColContactSituation = 13;

	//画面遷移用
	/// <summary>
	/// 列番号(グリッド) 予約区分
	/// </summary>
	private const int NoColYoyakuKbn = 14;
	/// <summary>
	/// 列番号(グリッド) 予約NO
	/// </summary>
	private const int NoColYoyakuNo = 15;
	/// <summary>
	/// 列番号(グリッド) コース種別
	/// </summary>
	private const int NoColCrsKind = 16;

	//CSV出力用
	/// <summary>
	/// 列番号(グリッド) 予約ボタン
	/// </summary>
	private const int NoColYoyakuBtn = 6;
	/// <summary>
	/// CSV列数
	/// </summary>
	private const int MaxCsvCol = 13;
	/// <summary>
	/// CSVファイル名(デスクトップ\部署_日付_帳票名)
	/// </summary>
	private const string CsvFileName = "{0}\\{1}_{2}_過剰入金一覧表.csv";

	#endregion

	#region プロパティ

	/// <summary>
	/// 条件GroupBoxの高さ
	/// </summary>
	/// <returns></returns>
	private int HeightGbxCondition
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
	private bool VisibleGbxCondition
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
	/// 画面初期処理
	/// </summary>
	protected override void StartupOrgProc()
	{

		// 画面終了時確認不要
		base.closeFormFlg = false;

		base.StartupOrgProc();

		//エラー有無のクリア
		clearError();

	}

	/// <summary>
	/// 条件GroupBox表示制御ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnVisiblerCondition_Click(object sender, EventArgs e)
	{
		this.VisibleGbxCondition = !this.VisibleGbxCondition;

		//Panel, グリッドの座標, サイズを表示/非表示に応じて変更
		if (this.VisibleGbxCondition == true)
		{
			//表示状態
			this.btnVisiblerCondition.Text = "非表示 <<";

			this.PanelEx1.Top = TopGbxCondition + this.HeightGbxCondition + MarginGbxCondition;
			this.PanelEx1.Height -= this.HeightGbxCondition + MarginGbxCondition;
			this.grdYoyakuInfo.Height -= this.HeightGbxCondition - 3;

		}
		else
		{
			//非表示状態
			this.btnVisiblerCondition.Text = "表示 >>";

			this.PanelEx1.Top = TopGbxCondition;
			this.PanelEx1.Height += this.HeightGbxCondition + MarginGbxCondition;
			this.grdYoyakuInfo.Height += this.HeightGbxCondition - 3;
		}
	}

	/// <summary>
	/// 条件クリアボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
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
	/// F8：検索ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnSearch_Click(object sender, EventArgs e)
	{

		// 検索ボタン押下イベント実行
		base.btnCom_Click(this.btnSearch, e);

		// log出力
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理");

	}

	/// <summary>
	/// 検索ボタン押下時処理
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
		if (checkSearchItems() == true)
		{
			//エラーがない場合、検索処理を実行

			// Gridへの表示(グリッドデータの取得とグリッド表示)
			reloadGrid();

		}

	}

	/// <summary>
	/// F8キーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S02_1502_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.F8)
		{
			e.Handled = true;
			this.btnSearch_Click(sender, e);
		}
	}

	/// <summary>
	/// F4：CSV出力ボタン押下イベント
	/// </summary>
	protected override void btnF4_ClickOrgProc()
	{

		base.btnF4_ClickOrgProc();

		try
		{
			//前処理
			base.comPreEvent();

			//検索結果存在チェック
			if (grdYoyakuInfo.Row <= 0)
			{
				//一覧に検索結果が表示されていない
				//処理対象データがありません。
				CommonProcess.createFactoryMsg().messageDisp("E90_004");
			}
			else
			{
				//一覧に検索結果が表示されている場合
				//CSV出力処理
				outCsv();
			}

		}
		catch
		{
			throw;

		}
		finally
		{
			// 後処理
			base.comPostEvent();
		}

	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void FlexGridEx1_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		//データ件数を表示(ヘッダー行分マイナス1)
		string formatedCount = System.Convert.ToString((this.grdYoyakuInfo.Rows.Count - 1).ToString().PadLeft(6));
		this.lblLengthGrd.Text = formatedCount + "件";
	}

	/// <summary>
	/// 予約ボタン実行時の画面遷移イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void miNyuukin_CellButtonClick()
	{
		)grdYoyakuInfo.CellButtonClick;

		FlexGridEx grd = null;
		S02_0103ParamData prm = new S02_0103ParamData();
		string yoyakuKbn = ""; //予約区分(画面遷移パラメータ)
		int yoyakuNo = 0; //予約NO(画面遷移パラメータ)
		string crsKind = ""; //コース種別(画面遷移パラメータ)

		grd = TryCast(sender, FlexGridEx);

		//選択行が0以下の場合は処理をしない
		if (grd.Row <= 0)
		{
			return;
		}

		//押下行の予約区分、予約NO、コース種別取得
		yoyakuKbn = System.Convert.ToString(TryCast(grd.GetData(e.Row, NoColYoyakuKbn), string));

		if (ReferenceEquals(grd.GetData(e.Row, NoColYoyakuNo), null))
		{
			return;
		}

		yoyakuNo = int.Parse(System.Convert.ToString(grd.GetData(e.Row, NoColYoyakuNo).ToString()));
		crsKind = System.Convert.ToString(TryCast(grd.GetData(e.Row, NoColCrsKind), string));

		//予約ボタン押下
		//画面間パラメータを用意
		prm.YoyakuKbn = yoyakuKbn; //予約区分
		prm.YoyakuNo = yoyakuNo; //予約NO
		prm.CrsKind = crsKind; //コース種別
		prm.ScreenMode = CommonRegistYoyaku.ScreenModeReference; //遷移先画面モード(参照)

		using (S02_0103 form = new S02_0103())
		{
			form.ParamData = prm;
			form.ShowDialog();
		}

	}

	/// <summary>
	/// グリッドソート後、予約の連絡状況によって行の文字色を変更する
	/// </summary>
	private void grdYoyakuInfo_AfterSort(object sender, EventArgs e)
	{

		//予約の連絡状況によって、グリッドの行の文字色を変更する
		changeFontColor();

	}

	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{

		//MyBase.btnF2_ClickOrgProc()

		closeCheckFlg = false;
		base.closeFormFlg = false;
		this.Close();

	}

	/// <summary>
	/// 画面終了時処理
	/// </summary>
	private void S02_1501_FormClosing()
	{

		this.Dispose();

	}

	#endregion

	#region メソッド
	/// <summary>
	/// [検索エリア]検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{
		base.initSearchAreaItems();

		//日付FROMTO：システム日付を設定
		dtmYmdFromTo.FromDateText = System.DateTime.Today;
		dtmYmdFromTo.ToDateText = System.DateTime.Today;

		//基準日：予約更新日をONに設定
		rdoYoyakuUpdateDay.Checked = true;

		//支払区分：振込、ONカードをONに設定
		chkHurikomi.Checked = true;
		chkONCard.Checked = true;

		//日本語／外国語コース
		if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
		{
			//ユーザーが国際事業部の場合は外国語
			chkGaikokugo.Checked = true;
			chkJapanese.Checked = false;
		}
		else
		{
			//それ以外の場合は日本語をONに設定
			chkJapanese.Checked = true;
			chkGaikokugo.Checked = false;
		}

		//キャンセル済み、催行中止・運休、通常予約をONに設定
		chkCancelAlready.Checked = true;
		chkSaikouChuusiUnkyu.Checked = true;
		chkNormalYoyaku.Checked = true;

		//上記以外の項目は全て空欄/チェックOFF
		//取消料100%
		chkCancelRyou100Percent.Checked = false;

		//連絡済みを含む
		chkContactAlreadyWith.Checked = false;

		// ロード時にフォーカスを設定する
		this.ActiveControl = this.dtmYmdFromTo;
	}

	/// <summary>
	/// エラー有無のクリア
	/// </summary>
	private void clearError()
	{

		// ExistErrorプロパティのクリア
		dtmYmdFromTo.ExistErrorForToDate = false;
		dtmYmdFromTo.ExistErrorForFromDate = false;

		// Exceptionのクリア
		this.Exception = null;

		// エラーフラグの初期化
		this.ErrorFlg = false;

	}

	/// <summary>
	/// [詳細エリア]検索結果部の項目初期化
	/// </summary>
	protected override void initDetailAreaItems()
	{
		base.initDetailAreaItems();

		DataTable dt = new DataTable();
		setGrdYoyakuInfo();
		this.grdYoyakuInfo.DataSource = dt;
		this.grdYoyakuInfo.DataMember = "";
		this.grdYoyakuInfo.Refresh();
		this.lblLengthGrd.Text = "     0件";
	}

	/// <summary>
	/// フッタボタンの制御(表示\[活性]／非表示[非活性])
	/// </summary>
	protected override void initFooterButtonControl()
	{
		base.initFooterButtonControl();

		this.F10Key_Visible = false; // F10:非表示
		this.F11Key_Visible = false; // F11:非表示
		this.F4Key_Enabled = true; // F4:活性
	}

	/// <summary>
	/// 予約情報グリッドの設定
	/// </summary>
	private void setGrdYoyakuInfo()
	{
		//グリッドの設定
		this.grdYoyakuInfo.AllowDragging = (C1.Win.C1FlexGrid.AllowDraggingEnum)false;
		this.grdYoyakuInfo.AllowAddNew = false;
		this.grdYoyakuInfo.AllowMerging = (C1.Win.C1FlexGrid.AllowMergingEnum)(8);
		this.grdYoyakuInfo.AutoGenerateColumns = false;
		this.grdYoyakuInfo.ShowButtons = (C1.Win.C1FlexGrid.ShowButtonsEnum)(2);

		this.grdYoyakuInfo.Cols(1).AllowEditing = false;
		this.grdYoyakuInfo.Cols(2).AllowEditing = false;
		this.grdYoyakuInfo.Cols(3).AllowEditing = false;
		this.grdYoyakuInfo.Cols(4).AllowEditing = false;
		this.grdYoyakuInfo.Cols(5).AllowEditing = false;
		//Me.grdYoyakuInfo.Cols(6).AllowEditing = False      ボタン列のため
		this.grdYoyakuInfo.Cols(7).AllowEditing = false;
		this.grdYoyakuInfo.Cols(8).AllowEditing = false;
		this.grdYoyakuInfo.Cols(9).AllowEditing = false;
		this.grdYoyakuInfo.Cols(10).AllowEditing = false;
		this.grdYoyakuInfo.Cols(11).AllowEditing = false;
		this.grdYoyakuInfo.Cols(12).AllowEditing = false;
		this.grdYoyakuInfo.Cols(13).AllowEditing = false;

	}

	/// <summary>
	/// 日付入力値の調整
	/// </summary>
	private void setYmdFromTo()
	{
		if (dtmYmdFromTo.FromDateText IsNot null && ReferenceEquals(dtmYmdFromTo.ToDateText, null))
		{
			//日付From <> ブランク かつ 日付To = ブランクの場合、日付To に 日付From の値をセット
			dtmYmdFromTo.ToDateText = dtmYmdFromTo.FromDateText;

		}
		else if (dtmYmdFromTo.ToDateText IsNot null && ReferenceEquals(dtmYmdFromTo.FromDateText, null))
		{
			//日付To <> ブランク かつ 日付From = ブランクの場合、日付From に 日付To の値をセット
			dtmYmdFromTo.FromDateText = dtmYmdFromTo.ToDateText;
		}
	}

	/// <summary>
	///検索条件項目入力チェック
	/// </summary>
	/// <returns>エラーがない場合：True、エラーの場合：False</returns>
	protected override bool checkSearchItems()
	{
		base.checkSearchItems();

		//日付入力チェック
		if (ReferenceEquals(dtmYmdFromTo.FromDateText, null) && ReferenceEquals(dtmYmdFromTo.ToDateText, null))
		{
			//日付Fromと日付Toどちらも未入力の場合、エラー
			//日付を入力してください。
			CommonProcess.createFactoryMsg().messageDisp("E90_011", "日付");

			//背景色を赤にする
			dtmYmdFromTo.ExistErrorForToDate = true;
			dtmYmdFromTo.ExistErrorForFromDate = true;

			//先頭のエラー項目にフォーカスを設定する
			this.ActiveControl = this.dtmYmdFromTo;

			return false;
		}

		//日付値チェック
		if (CommonDateUtil.chkDayFromTo(System.Convert.ToDateTime(dtmYmdFromTo.FromDateText), System.Convert.ToDateTime(dtmYmdFromTo.ToDateText)) == false)
		{
			//日付From＞日付Toの場合、エラー
			//日付の設定が不正です。
			CommonProcess.createFactoryMsg().messageDisp("E90_017", "日付");

			//背景色を赤にする
			dtmYmdFromTo.ExistErrorForToDate = true;
			dtmYmdFromTo.ExistErrorForFromDate = true;

			//先頭のエラー項目にフォーカスを設定する
			this.ActiveControl = this.dtmYmdFromTo;

			return false;
		}

		return true;

	}

	/// <summary>
	/// Gridへの表示(グリッドデータの取得とグリッド表示)
	/// </summary>
	protected override void reloadGrid()
	{
		base.reloadGrid();

		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		DataTable dt = new DataTable();

		//日付(パラメータ)編集用変数
		DateTime dteTmpDate = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		string strFromDate = string.Empty;
		string strToDate = string.Empty;

		//データアクセス
		S02_1502Da dataAccess = new S02_1502Da();
		DataTable dataKazyouNyuukinList = new DataTable();

		//グリッド表示用dataRow
		DataRow dr = null;
		//dt.NewRow
		DataRow drX = null;

		//パラメータ設定
		//選択
		//日本語
		paramInfoList.Add("Japanese", Strings.Trim(System.Convert.ToString(this.chkJapanese.Checked)));
		//外国語
		paramInfoList.Add("Gaikokugo", Strings.Trim(System.Convert.ToString(this.chkGaikokugo.Checked)));
		//キャンセル済み
		paramInfoList.Add("CancelAlready", Strings.Trim(System.Convert.ToString(this.chkCancelAlready.Checked)));
		//通常予約
		paramInfoList.Add("NormalYoyaku", Strings.Trim(System.Convert.ToString(this.chkNormalYoyaku.Checked)));
		//催行中止・運休
		paramInfoList.Add("SaikouChuusiUnkyu", Strings.Trim(System.Convert.ToString(this.chkSaikouChuusiUnkyu.Checked)));
		//取消料100%
		paramInfoList.Add("CancelRyou100Percent", Strings.Trim(System.Convert.ToString(this.chkCancelRyou100Percent.Checked)));

		//基準日
		//予約更新日
		paramInfoList.Add("YoyakuUpdateDay", Strings.Trim(System.Convert.ToString(this.rdoYoyakuUpdateDay.Checked)));
		//出発日
		paramInfoList.Add("SyuptDay", Strings.Trim(System.Convert.ToString(this.rdoSyuptDay.Checked)));
		//日付
		dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(dtmYmdFromTo.FromDateText));
		strFromDate = System.Convert.ToString(dteTmpDate.Year * calcYear + dteTmpDate.Month * calcMonth + dteTmpDate.Day);
		dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(dtmYmdFromTo.ToDateText));
		strToDate = System.Convert.ToString(dteTmpDate.Year * calcYear + dteTmpDate.Month * calcMonth + dteTmpDate.Day);
		paramInfoList.Add("YmdFrom", strFromDate);
		paramInfoList.Add("YmdTo", strToDate);

		//支払区分
		//振込
		paramInfoList.Add("Hurikomi", Strings.Trim(System.Convert.ToString(this.chkHurikomi.Checked)));
		//ONカード
		paramInfoList.Add("ONCard", Strings.Trim(System.Convert.ToString(this.chkONCard.Checked)));

		//連絡済含む
		paramInfoList.Add("ContactAlreadyWith", Strings.Trim(System.Convert.ToString(this.chkContactAlreadyWith.Checked)));

		//SQLでデータを取得
		dataKazyouNyuukinList = dataAccess.getKazyouNyuukinList(paramInfoList);

		//データ取得件数チェック
		if (dataKazyouNyuukinList.Rows.Count <= 0)
		{
			//取得件数が0件の場合、エラー

			// [詳細エリア]検索結果部の項目初期化
			initDetailAreaItems();

			//該当データが存在しません。
			CommonProcess.createFactoryMsg().messageDisp("E90_019");

		}
		else
		{
			//データが取得できた場合

			//列作成
			dt.Columns.Add("colSelectionKbn"); //選択区分
			dt.Columns.Add("colSyuptDay"); //出発日
			dt.Columns.Add("colCrsCd"); //コースコード
			dt.Columns.Add("colSaikouSituation"); //催行状況
			dt.Columns.Add("colYoyakuNo"); //予約番号
			dt.Columns.Add("colYoyaku"); //予約ボタン
			dt.Columns.Add("colName"); //名前
			dt.Columns.Add("colNyuukinDay"); //入金日
			dt.Columns.Add("colNyuukinHoho"); //入金方法
			dt.Columns.Add("colNyuukinGaku"); //入金額
			dt.Columns.Add("colUriageKingaku"); //売上金額
			dt.Columns.Add("colKazyouKingaku"); //過剰金額
			dt.Columns.Add("colContactSituation"); //連絡状況
			dt.Columns.Add("colHihyoji01"); //予約区分(非表示/画面遷移パラメータ用)
			dt.Columns.Add("colHihyoji02"); //予約NO(非表示/画面遷移パラメータ用)
			dt.Columns.Add("colHihyoji03"); //コース種別(非表示/画面遷移パラメータ用)

			//取得した値を各列に設定
			foreach (DataRow tempLoopVar_dr in dataKazyouNyuukinList.Rows)
			{
				dr = tempLoopVar_dr;
				drX = dt.NewRow;

				drX["colSelectionKbn"] = dr["SELECTION_KBN_WORDING"];
				drX["colSyuptDay"] = dr["SYUPT_DAY"];
				drX["colCrsCd"] = dr["CRS_CD"];
				drX["colSaikouSituation"] = dr["SAIKOU_SITUATION"];
				drX["colYoyakuNo"] = dr["YOYAKU_NO_DISP"];
				drX["colName"] = dr["NAME"];
				drX["colNyuukinDay"] = dr["LAST_NYUUKIN_DAY"];
				drX["colNyuukinHoho"] = dr["NYUUKIN_HOHO_WORDING"];
				drX["colNyuukinGaku"] = dr["NYUUKIN_GAKU"];
				drX["colUriageKingaku"] = dr["URIAGE_KINGAKU"];
				drX["colKazyouKingaku"] = dr["KAZYOU_KINGAKU"];
				drX["colContactSituation"] = dr["CONTACT_SITUATION"];
				drX["colHihyoji01"] = dr["YOYAKU_KBN"];
				drX["colHihyoji02"] = dr["YOYAKU_NO"];
				drX["colHihyoji03"] = dr["CRS_KIND"];

				dt.Rows.Add(drX);

			}

			//グリッドに取得したデータを表示する
			grdYoyakuInfo.DataSource = dt;

			//未連絡予約の場合、グリッドの行の文字色を変更する
			changeFontColor();

		}
	}

	/// <summary>
	/// CSV出力処理
	/// </summary>
	private void outCsv()
	{

		//CSV出力
		CSVOut outCsv = new CSVOut();

		//CSVファイル出力開始処理
		//CSVファイル名(デスクトップ \ 部署_日付_帳票名)を生成してセット
		outCsv.StartCSVFileEdit(string.Format(CsvFileName, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), UserInfoManagement.sectionBusyoName, CommonDateUtil.getSystemTime().ToString("yyyyMMddHHmm")));

		//グリッドのデータを1セルずつファイルに出力する
		for (row = 0; row <= grdYoyakuInfo.Rows.Count - 1; row++)
		{
			for (col = 1; col <= MaxCsvCol; col++)
			{

				//予約ボタン列は表示しない
				if (col == NoColYoyakuBtn)
				{
					col = col + 1;
				}
				outCsv.AppendCSVData(grdYoyakuInfo.Item(row, col));
			}

			//次の予約データを出力する
			outCsv.EditNextCourse();

		}

		//バッファ内のデータをファイルに出力
		if (outCsv.WriteCSVFile() == true)
		{
			// 処理成功時
			CommonProcess.createFactoryMsg().messageDisp("I90_002", "CSV出力");
			// log出力
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.CSVOut, ScreenName, "CSV出力");
		}
		else
		{
			// 失敗時
			CommonProcess.createFactoryMsg().messageDisp("E90_028", "CSV出力");
			// log出力
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.CSVOut, ScreenName, "CSV出力");
		}


	}

	/// <summary>
	/// 予約の連絡状況によって、グリッドの行の文字色を変更する
	/// </summary>
	private void changeFontColor()
	{

		//グリッド行の文字色を赤にするスタイルを作成
		C1.Win.C1FlexGrid.CellStyle rowStyleMiContact = null;
		rowStyleMiContact = grdYoyakuInfo.Styles.Add("changeFontColorMiContact");
		rowStyleMiContact.ForeColor = Color.Red;

		//グリッド行の文字色を黒にするスタイルを作成
		C1.Win.C1FlexGrid.CellStyle rowStyleDefault = null;
		rowStyleDefault = grdYoyakuInfo.Styles.Add("changeFontColorDefault");
		rowStyleDefault.ForeColor = Color.Black;

		for (row = 0; row <= grdYoyakuInfo.Rows.Count - 1; row++)
		{
			//未連絡予約チェック
			if (grdYoyakuInfo.Item(row, NoColContactSituation).ToString().Trim() == StrMiContact)
			{
				//未連絡の予約の場合、行の文字色を赤にする
				grdYoyakuInfo.Rows(row).Style = rowStyleMiContact;
			}
			else
			{
				//未連絡の予約以外の場合、行の文字色を黒にする
				grdYoyakuInfo.Rows(row).Style = rowStyleDefault;
			}
		}
	}

	#endregion

}