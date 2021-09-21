using System.ComponentModel;
using System.Text.RegularExpressions;

/// <summary>
/// インターネット予約ログ照会
/// <remarks>
/// Author: 2018/10/31//QuangTD
/// </remarks>
/// </summary>
public class S02_1901 : PT11, iPT11
{
	#region  定数／変数宣言

	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	private const int TopGbxCondition = 41;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	private const int MarginGbxCondition = 6;
	/// <summary>
	/// スラッシュ : /
	/// </summary>
	private const string slashSympol = "/";
	/// <summary>
	/// スラッシュ : _
	/// </summary>
	private const string dashSympol = "_";
	/// <summary>
	/// スラッシュ : -
	/// </summary>
	private const string hyphenSympol = "-";

	#endregion

	#region イベント
	//画面共通的なイベントはパターンに実装し、各画面はAddhandlerのみの方がすっきりする(※新規ボタンなど参照)
	#region グリッド関連
	/// <summary>
	/// ソート処理
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdList_AfterSort(object sender, C1.Win.C1FlexGrid.SortColEventArgs e)
	{
		//一覧グリッドイベント時
		clickedMainGrid();
	}

	/// <summary>
	/// 行選択時のイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdList_RowColChange(object sender, System.EventArgs e)
	{
		//一覧グリッドイベント時
		clickedMainGrid();
	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdList_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{

		FlexGridEx grd = TryCast(sender, FlexGridEx);
		if (ReferenceEquals(grd, null))
		{
			return;
		}

		//データ件数を表示(ヘッダー行分マイナス1)
		this.lblLengthGrd.Text = string.Format("{0:#,0}件", grd.Rows.Count - 2).PadLeft(6);
	}

	#endregion

	#region フォーム

	/// <summary>
	/// フォームキーダウンイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void PT11_KeyDown(object sender, KeyEventArgs e)
	{

		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			base.btnCom_Click(this.btnSearch, e);
			btnSearch.Select();
		}
		else
		{
			return;
		}
	}
	#endregion

	#endregion

	#region PT11オーバーライド(基本的には変えない)

	#region 初期化処理
	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{
		//背景色初期化はここ
		CommonUtil.Control_Init(gbxCondition.Controls);
		//背景色初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);
		txtProcessUketukeDay.Select();
	}

	#endregion

	#region チェック系
	/// <summary>
	/// 検索入力項目チェック
	/// </summary>
	protected override bool checkSearchItems()
	{
		// 検索処理前のチェック処理
		return CheckSearch();
	}

	/// <summary>
	/// 更新入力項目チェック
	/// </summary>
	protected override bool checkUpdateItems()
	{
		// 更新処理前のチェック処理
		return CheckUpdate();
	}

	/// <summary>
	/// 更新入力項目チェック
	/// </summary>
	protected override bool checkInsertItems()
	{
		// 登録処理前のチェック処理
		return CheckInsert();
	}

	/// <summary>
	/// [変更チェック](差分チェック)
	/// </summary>
	/// <returns></returns>
	protected override bool checkDifference()
	{
		return false;
	}
	#endregion

	#region 固有初期処理
	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{
		base.initScreenPerttern();
		setSeFirsttDisplayData();
		btnClear.Click += btnCom_Click;
		btnSearch.Click += btnCom_Click;
	}
	#endregion

	#endregion

	#region Grid、データ関連
	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void reloadGrid()
	{
		this.grdCrsListInquiry.DataSource = SearchResultGridData;
	}

	#endregion

	#region 実装用メソッド(画面毎に変更)

	#region 初期処理
	/// <summary>
	/// 初期処理(必須画面個別実装)
	/// </summary>
	public void setSeFirsttDisplayData()
	{
		//グリッドの設定
		setcolgrdCrsListInquiry();
		setgrdCrsListInquiry();
	}

	#endregion

	#region エンティティ操作系
	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">添乗員一括入力エンティティ</param>
	public void DisplayDataToEntity(ref object ent)
	{
	}

	/// <summary>
	/// エンティティから画面に設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">行定表連結記号エンティティ</param>
	public void EntityDataToDisplay(ref object ent)
	{
	}

	/// <summary>
	/// DataGridからエンティティ(前回値)に設定する処理(必須画面個別実装)
	/// ※DataGrid上の1レコードから関連するデータも取得する。Keyがない場合などは未対応
	/// </summary>
	/// <param name="pDataRow">GridのDataBind(DataTable)の選択行</param>
	/// <remarks></remarks>
	public void OldDataToEntity(DataRow pDataRow)
	{
	}
	#endregion

	#region チェック系

	/// <summary>
	/// 検索処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckSearch()
	{
		//背景色初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);
		grdCrsListInquiry.DataSource = new DataTable();
		Regex regex = new Regex("([12]\\d{3}/(0[1-9]|1[0-2])/(0[1-9]|[12]\\d|3[01]))");
		//1	入力値チェック(日付)
		//「処理受付日」の暦上チェックする。
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.txtProcessUketukeDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty))))
		{
			Match match = regex.Match(txtProcessUketukeDay.Text);
			if (!match.Success)
			{
				txtProcessUketukeDay.ExistError = true;
				this.txtProcessUketukeDay.Select();
				createFactoryMsg().messageDisp("E90_016", "日付の入力");
				return false;
			}
		}

		//2 相関チェック(範囲の大小チェック)
		//「処理受付日」がマイナスではないかをチェックする。
		if (txtProcessUketukeDay.Text.Contains(hyphenSympol))
		{
			createFactoryMsg().messageDisp("E90_014", "コースコード");
			txtProcessUketukeDay.ExistError = true;
			this.txtProcessUketukeDay.Select();
			return false;
		}

		//3 入力値チェック(日付)
		//「出発日」の暦上チェックする。
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.txtSyuptDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty))))
		{
			Match match = regex.Match(txtSyuptDay.Text);
			if (!match.Success)
			{
				txtSyuptDay.ExistError = true;
				this.txtSyuptDay.Select();
				createFactoryMsg().messageDisp("E90_051");
				return false;
			}
		}
		//4	 相関チェック(範囲の大小チェック)
		//「出発日」がマイナスではないかをチェックする。
		if (txtSyuptDay.Text.Contains(hyphenSympol))
		{
			createFactoryMsg().messageDisp("E90_070");
			txtSyuptDay.ExistError = true;
			this.txtSyuptDay.Select();
			return false;
		}

		//5 存在有無チェック
		//「CRS_CD」が存在するかチェックする。
		if (!string.IsNullOrEmpty(System.Convert.ToString(txtCrsCd.CodeText)))
		{
			CourseMst_DA courseMst_DA = new CourseMst_DA();
			Hashtable paramCheckCrsCd = new Hashtable();
			paramCheckCrsCd.Add("crsCd", txtCrsCd.CodeText);
			object resultCheckCrsCd = courseMst_DA.getCrsCdExist(paramCheckCrsCd);
			if (resultCheckCrsCd.Rows.Count == 0)
			{
				txtCrsCd.ExistError = true;
				createFactoryMsg().messageDisp("E90_016", "入力内容");
				return false;
			}
		}

		//6 相関チェック(範囲の大小チェック)「
		//予約番号」がマイナスではないかをチェックする。
		if (txtYoyakuNo.YoyakuText.Contains(hyphenSympol))
		{
			txtYoyakuNo.ExistError = true;
			this.txtYoyakuNo.Select();
			createFactoryMsg().messageDisp("E90_014", "マスタにコード");
			return false;
		}

		//7必須チェック
		//「処理受付日」「出発日」「予約番号」の全てが未入力ではないかチェックする。
		if (string.IsNullOrEmpty(System.Convert.ToString(this.txtSyuptDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty))) && string.IsNullOrEmpty(System.Convert.ToString(this.txtProcessUketukeDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty))) && string.IsNullOrEmpty(System.Convert.ToString(txtYoyakuNo.YoyakuText)))
		{
			txtSyuptDay.ExistError = true;
			txtProcessUketukeDay.ExistError = true;
			txtYoyakuNo.ExistError = true;
			txtProcessUketukeDay.Select();
			createFactoryMsg().messageDisp("E90_006", "どちらか片方のみ指定できます。双方の指定", " ");
			return false;
		}

		//8	相関チェック(同時指定可能条件)
		//「電話番号」のフォーマットが正しいかチェックする。
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.txtTelNo.Text)))
		{
			if (!CommonCheckUtil.IsValidateTel(Microsoft.VisualBasic.Strings.StrConv(this.txtTelNo.Text, VbStrConv.Narrow)))
			{
				txtTelNo.ExistError = true;
				this.txtTelNo.Select();
				createFactoryMsg().messageDisp("E90_016", "電話番号のフォーマット");
				return false;
			}
		}

		//9 存在有無チェック
		//「AGENT_CD」が存在するかチェックする。
		if (!string.IsNullOrEmpty(System.Convert.ToString(txtDairitenCd.CodeText)))
		{
			Agent_DA agentDA = new Agent_DA();
			Hashtable paramCheckAgentCD = new Hashtable();
			paramCheckAgentCD.Add("AgentCd", txtDairitenCd.CodeText);
			object resultCheckAgentCd = agentDA.getAgentCDExist(paramCheckAgentCD);
			if (resultCheckAgentCd.Rows.Count == 0)
			{
				txtDairitenCd.ExistError = true;
				createFactoryMsg().messageDisp("E90_014", "業者コード");
				return false;
			}
		}

		//10 相関チェック(同時指定可能条件)
		//「対応済み」「対応不要」が同時選択されていないかチェックする。
		if (chkTaiouAlreadyWith.Checked && chkTaiouHuyoWith.Checked)
		{
			chkTaiouAlreadyWith.ExistError = true;
			chkTaiouHuyoWith.ExistError = true;
			createFactoryMsg().messageDisp("E90_072", "「対応済み」「対応不要」");
			return false;
		}
		return true;
	}

	/// <summary>
	/// 登録処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckInsert()
	{
		return true;
	}

	/// <summary>
	/// 更新処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckUpdate()
	{
		return true;
	}

	#region チェック処理(Private)
	/// <summary>
	/// 必須入力項目エラーチェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	public bool isExistHissuError()
	{
		bool returnValue = false;
		return returnValue;
	}

	#endregion
	#endregion
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
	/// 条件GroupBox表示制御ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnVisiblerCondition_Click(object sender, EventArgs e)
	{
		this.VisibleGbxCondition = !this.VisibleGbxCondition;

		//Panel, グリッドの座標, サイズを表示/非表示に応じて変更
		if (this.VisibleGbxCondition)
		{
			//表示状態
			this.btnVisiblerCondition.Text = "非表示 <<";
			this.PanelEx1.Top = TopGbxCondition + this.HeightGbxCondition + MarginGbxCondition;
			this.PanelEx1.Height -= this.HeightGbxCondition + MarginGbxCondition;
			this.grdCrsListInquiry.Height -= this.HeightGbxCondition - 3;

		}
		else
		{
			//非表示状態
			this.btnVisiblerCondition.Text = "表示 >>";
			this.PanelEx1.Top = TopGbxCondition;
			this.PanelEx1.Height += this.HeightGbxCondition + MarginGbxCondition;
			this.grdCrsListInquiry.Height += this.HeightGbxCondition - 3;
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
		string formatedCount = System.Convert.ToString((this.grdCrsListInquiry.Rows.Count - 2).ToString().PadLeft(6));
		this.lblLengthGrd.Text = formatedCount + "件";
	}

	/// <summary>
	/// 表示列、登録列ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdCrsListInquiry_CellButtonClick(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
	{

		if (e.Col != 1)
		{
			// 照会ボタンでない場合、処理終了
			return;
		}

		int rowIndex = System.Convert.ToInt32(e.Row - 2);

		object dataRow = ((DataTable)grdCrsListInquiry.DataSource).Rows(rowIndex);
		string tranKbn = System.Convert.ToString(dataRow["TRAN_KBN"]);
		string tranKey = System.Convert.ToString(dataRow["TRAN_KEY"]);

		Delivery_S02_1902Entity deliveryEntity = new Delivery_S02_1902Entity();
		//トランザクション区分
		deliveryEntity.tranKbn = tranKbn;
		//トランザクションキー
		deliveryEntity.tranKey = tranKey;

		// インターネット予約ログ詳細遷移
		using (S02_1902 form = new S02_1902(deliveryEntity))
		{

			this.Visible = false;
			form.ShowDialog();
			this.Visible = true;
		}

	}

	#endregion

	#region メソッド

	/// <summary>
	/// フッタボタンの制御(表示\[活性]／非表示[非活性])
	/// </summary>
	protected override void initFooterButtonControl()
	{
		base.initFooterButtonControl();
		this.F1Key_Visible = false;
		this.F2Key_Visible = true;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		this.F7Key_Visible = true;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = false;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;
		this.F2Key_Text = "F2:戻る";
		this.F7Key_Text = "F7：出力";
		this.F7Key_Enabled = true;
		//ベースフォームの設定
		this.setFormId = "S02_1901";
		this.setTitle = "インターネット予約ログ照会";
	}

	#region コース一覧照会グリッドの設定
	//現払登録対象コース照会グリッドの設定
	private void setcolgrdCrsListInquiry()
	{
		// 行ヘッダを作成。
		grdCrsListInquiry.Styles.Normal.WordWrap = true;
		grdCrsListInquiry.Cols.Count = 12; //gridの行数
		grdCrsListInquiry.Rows.Fixed = 2; // 行固定
										  //grdGenbaraiEntryTaisyoCrsInquiry.Cols.Fixed = 6 ' 列固定
		grdCrsListInquiry.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;
		grdCrsListInquiry.Rows(0).AllowMerging = true;
		grdCrsListInquiry.Cols(0).AllowMerging = true;
		C1.Win.C1FlexGrid.CellRange cr = null;
		// メモ
		cr = grdCrsListInquiry.GetCellRange(0, 8, 0, 10);
		cr.Data = "希望コース";
		grdCrsListInquiry.MergedRanges.Add(cr);
		grdCrsListInquiry[1, 8] = "出発日";
		grdCrsListInquiry[1, 9] = "コースコード";
		grdCrsListInquiry[1, 10] = "号車";
		// 日付
		cr = grdCrsListInquiry.GetCellRange(0, 1, 1, 1);
		grdCrsListInquiry.MergedRanges.Add(cr);
		// コースコード
		cr = grdCrsListInquiry.GetCellRange(0, 2, 1, 2);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//コース名
		cr = grdCrsListInquiry.GetCellRange(0, 3, 1, 3);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//状況
		cr = grdCrsListInquiry.GetCellRange(0, 4, 1, 4);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//乗車地
		cr = grdCrsListInquiry.GetCellRange(0, 5, 1, 5);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//時間
		cr = grdCrsListInquiry.GetCellRange(0, 6, 1, 6);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//号車
		cr = grdCrsListInquiry.GetCellRange(0, 7, 1, 7);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//空席
		cr = grdCrsListInquiry.GetCellRange(0, 11, 1, 11);
		grdCrsListInquiry.MergedRanges.Add(cr);

		grdCrsListInquiry.Cols(0).AllowEditing = false;
		grdCrsListInquiry.Cols(1).AllowEditing = true;
		grdCrsListInquiry.Cols(8).TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.LeftCenter;
		for (int i = 2; i <= 11; i++)
		{
			grdCrsListInquiry.Cols(i).AllowEditing = false;
		}
	}


	//コース一覧照会(メモ一覧)モック用グリッド表示値の設定
	private void setgrdCrsListInquiry()
	{
		grdCrsListInquiry.Styles.Normal.WordWrap = true;
		grdCrsListInquiry.Rows(0).Height = 40;
		grdCrsListInquiry.DataSource = new DataTable();
	}

	/// <summary>
	/// 対象マスタのデータ取得
	/// </summary>
	protected override DataTable getMstData()
	{

		Hashtable paramList = new Hashtable();

		//作成日
		paramList.Add("createDay", txtProcessUketukeDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty));
		//コース出発日
		paramList.Add("crsSyuptDay", txtSyuptDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty));
		//コースコード
		paramList.Add("crsCd", txtCrsCd.CodeText);
		if (!string.IsNullOrEmpty(System.Convert.ToString(txtYoyakuNo.YoyakuText)))
		{
			//予約区分
			paramList.Add("yoyakuKbn", txtYoyakuNo.YoyakuText.Substring(0, 1));
			if (txtYoyakuNo.YoyakuText.Length > 1)
			{
				//予約ＮＯ
				paramList.Add("yoyakuNo", txtYoyakuNo.YoyakuText.Substring(1, txtYoyakuNo.YoyakuText.Length - 1));
			}
		}
		//申込者名
		paramList.Add("yykmksName", txtNameBf.Text);
		//業者電話番号
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.txtTelNo.Text)))
		{
			paramList.Add("agentTelNo", Microsoft.VisualBasic.Strings.StrConv(this.txtTelNo.Text, VbStrConv.Narrow));
		}
		//業者コード
		paramList.Add("agentCd", txtDairitenCd.CodeText);
		//チェックボックス
		if (chkTaiouAlreadyWith.Checked)
		{
			paramList.Add("chkTaiouAlreadyWith", chkTaiouAlreadyWith.Checked);
		}
		if (chkTaiouHuyoWith.Checked)
		{
			paramList.Add("chkTaiouHuyoWith", chkTaiouHuyoWith.Checked);
		}

		DataTable ReturnData = null;

		try
		{
			//検索処理
			IntYoyakuTranManagement_DA dataAccess = new IntYoyakuTranManagement_DA();
			ReturnData = dataAccess.accessIntYoyakuTranManagement(IntYoyakuTranManagement_DA.accessType.getIntYoyakuTranLog, paramList);
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, setTitle);
		}
		catch (Exception ex)
		{
			//ログ出力(エラーログ)
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
			throw;
		}

		return ReturnData;
	}

	#endregion
	#endregion

	#region Datastudio 呼び出し
	/// <summary>
	/// F7ボタン押下時の独自処理
	/// </summary>
	protected override void btnF7_ClickOrgProc()
	{
		string[] logmsg = new string[2];
		try
		{
			//入力項目のチェック
			if (checkRequiredItemDsReport() == false)
			{
				return;
			}
			logmsg[0] = System.Convert.ToString(txtProcessUketukeDay.Value.ToString());
			//DataStudio 呼び出し
			callDsReport();
			//ログ出力(操作ログ)
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.DSCall, setTitle, logmsg);
		}
		catch (Exception ex)
		{
			//【システム全体】例外エラーメッセージ表示
			createFactoryMsg.messageDisp("E90_032");
			//ログ出力(エラーログ)
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.DSCall, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
		}
	}

	/// <summary>
	/// 入力項目のチェック
	/// </summary>
	private bool checkRequiredItemDsReport()
	{
		//背景色初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);
		//11	必須チェック「処理受付日」「出発日」の全てが未入力ではないかチェックする。
		if (string.IsNullOrEmpty(System.Convert.ToString(this.txtProcessUketukeDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty))) && string.IsNullOrEmpty(System.Convert.ToString(this.txtSyuptDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty))))
		{
			txtSyuptDay.ExistError = true;
			txtProcessUketukeDay.ExistError = true;
			txtProcessUketukeDay.Select();
			createFactoryMsg().messageDisp("E90_071");
			return false;
		}
		//12	相関チェック(同時指定可能条件)  「処理受付日」「出発日」が同時選択されていないかチェックする。
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.txtProcessUketukeDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty))) && !string.IsNullOrEmpty(System.Convert.ToString(this.txtSyuptDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty))))
		{
			txtSyuptDay.ExistError = true;
			txtProcessUketukeDay.ExistError = true;
			txtProcessUketukeDay.Select();
			createFactoryMsg().messageDisp("E90_006", "どちらか片方のみ指定できます。", "双方の指定");
			return false;
		}
		//13	入力値チェック(日付) 「処理受付日」が入力値＞システム日付になっていないかチェックする。
		Regex Regex = new Regex("([12]\\d{3}/(0[1-9]|1[0-2])/(0[1-9]|[12]\\d|3[01]))");
		if (!string.IsNullOrEmpty(System.Convert.ToString(txtProcessUketukeDay.Text.Replace(slashSympol, string.Empty).Replace(dashSympol, string.Empty))))
		{
			Match match = Regex.Match(txtProcessUketukeDay.Text);
			if (!match.Success || DateTime.Compare(System.Convert.ToDateTime(txtProcessUketukeDay.Value.Value), System.Convert.ToDateTime(getDateTime)) > 0)
			{
				txtProcessUketukeDay.ExistError = true;
				txtProcessUketukeDay.Focus();
				createFactoryMsg().messageDisp("E90_011", "今日以前の日付");
				return false;
			}
		}
		//10 「対応済み」「対応不要」が同時選択されていないかチェックする。
		if (chkTaiouAlreadyWith.Checked && chkTaiouHuyoWith.Checked)
		{
			chkTaiouAlreadyWith.ExistError = true;
			chkTaiouHuyoWith.ExistError = true;
			chkTaiouAlreadyWith.Select();
			createFactoryMsg().messageDisp("E90_072", "「対応済み」「対応不要」");
			return false;
		}
		return true;
	}

	/// <summary>
	/// DataStudio 呼び出し
	/// </summary>
	private void callDsReport()
	{
		string PostData = string.Empty;
		string processUketukeDay = "";
		string syuptDay = "";
		//作成日P02_1901を参照


		if (txtProcessUketukeDay.Value IsNot null)
		{
			processUketukeDay = System.Convert.ToString(System.Convert.ToDateTime(this.txtProcessUketukeDay.Value).ToString("yyyyMMdd"));
			PostData += "free_select_0_0=P02_1901;CREATE_DAY&free_opecomp_0_0=is not null";
			PostData += "&free_opelogic_0_1=AND&free_select_0_1=P02_1901;CREATE_DAY&free_opecomp_0_1==&free_value_0_1=" + processUketukeDay;
		}
		//出発日P02_1901を参照
		if (txtSyuptDay.Value IsNot null)
		{
			syuptDay = System.Convert.ToString(System.Convert.ToDateTime(this.txtSyuptDay.Value).ToString("yyyyMMdd"));
			if (txtProcessUketukeDay.Value IsNot null)
			{
				PostData += "&free_opelogic_0_2=AND&free_select_0_2=P02_1901;CRS_SYUPT_DAY&free_opecomp_0_2=is not null";
				PostData += "&free_opelogic_0_3=AND&free_select_0_3=P02_1901;CRS_SYUPT_DAY&free_opecomp_0_3==&free_value_0_3=" + syuptDay;
			}
			else
			{
				PostData += "free_select_0_2=P02_1901;CRS_SYUPT_DAY&free_opecomp_0_2=is not null";
				PostData += "&free_opelogic_0_3=AND&free_select_0_3=P02_1901;CRS_SYUPT_DAY&free_opecomp_0_3==&free_value_0_3=" + syuptDay;
			}
		}
		//運用対処フラグP02_1901を参照
		//If chkTaiouAlreadyWith.Checked = False Then
		if (chkTaiouAlreadyWith.Checked == true)
		{
			if (ReferenceEquals(txtProcessUketukeDay.Value, null) && ReferenceEquals(txtSyuptDay.Value, null))
			{
				//PostData &= "&free_opelogic_0_2=OR&free_select_0_2=P02_1901;OPERATION_TAISHO_FLG&free_opecomp_0_2=<>&free_value_0_2=Y"
				PostData += "&free_opelogic_0_4=OR&free_select_0_4=P02_1901;OPERATION_TAISHO_FLG&free_opecomp_0_4==&free_value_0_4=Y";
			}
			else
			{
				//PostData &= "&free_opelogic_0_2=AND&free_select_0_2=P02_1901;OPERATION_TAISHO_FLG&free_opecomp_0_2=<>&free_value_0_2=Y"
				PostData += "&free_opelogic_0_4=AND&free_select_0_4=P02_1901;OPERATION_TAISHO_FLG&free_opecomp_0_4==&free_value_0_4=Y";
			}
		}
		//無効フラグP02_1901を参照
		//If chkTaiouHuyoWith.Checked = False Then
		if (chkTaiouHuyoWith.Checked == true)
		{
			if (ReferenceEquals(txtProcessUketukeDay.Value, null) && ReferenceEquals(txtSyuptDay.Value, null) && chkTaiouAlreadyWith.Checked == true)
			{
				//PostData &= "&free_opelogic_0_3=OR&free_select_0_3=P02_1901;INVALID_FLG&free_opecomp_0_3=<>&free_value_0_3=Y"
				PostData += "&free_opelogic_0_5=OR&free_select_0_5=P02_1901;INVALID_FLG&free_opecomp_0_5==&free_value_0_5=Y";
			}
			else
			{
				//PostData &= "&free_opelogic_0_3=AND&free_select_0_3=P02_1901;INVALID_FLG&free_opecomp_0_3=<>&free_value_0_3=Y"
				PostData += "&free_opelogic_0_5=AND&free_select_0_5=P02_1901;INVALID_FLG&free_opecomp_0_5==&free_value_0_5=Y";
			}
		}

		ReadAppConfig readAppConfig = new ReadAppConfig();
		CommonProcess.DataStudioId = readAppConfig.getAppSetting(readAppConfig.DataStudioId);
		CommonProcess.DataStudioPassword = readAppConfig.getAppSetting(readAppConfig.DataStudioPassword);
		BOCommon.showDataStudio(this.Name, BOCommon.SystemSetCd.dsid_IntanettoYoyakuErarisuto, PostData);

	}
	#endregion
}