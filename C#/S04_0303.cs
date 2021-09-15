using Hatobus.ReservationManagementSystem.Master;
using Hatobus.ReservationManagementSystem.Yoyaku;
using System.ComponentModel;


/// <summary>
/// NO SHOW 一覧
/// </summary>
public class S04_0303 : PT11
{

	#region  定数／変数宣言
	private bool checkDisplayFlg = true; // 表示/非表示ボタン用フラグ(True:表示, False:非表示)

	private ButtonEx SetButton = null; // コントロール 表示/非表示ボタン
	private GroupBoxEx SetGroupBox = null; // コントロール 検索条件
	private PanelEx SetPanel = null; // コントロール パネル
	private FlexGridEx SetGrid = null; // コントロール グリッド
	private int HeightgbxConditionToPanelEx1 = 0; // 検索条件と一覧の間

	private DataTable selectOldData = new DataTable(); // 検索後のデータを保持
	private DataTable updateTargetData = new DataTable(); // 更新対象データを保持

	//更新時のメッセージ
	private string UpdateMsg = "NO SHOWフラグ";
	private string InsertMsg = "NO SHOWフラグ";

	// メッセージ表示項目
	private const string MessageDisplay = "登録";
	private const string MessageDisplayCsvfile = "CSVファイル";
	private const string MessageDisplayCsvout = "CSV出力";

	private DateTime crstemUpdateDay; // システム更新ＰＧＭＩＤ
	private string crsUpdatePgmid; // システム更新者コード
	private string crsUpdatePersonCd; // システム更新日
	#endregion

	#region 列挙
	/// <summary>
	/// カラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum No_Show_Koumoku : int
	{
		[Value("コースコード")]
		crscd = 1,
		[Value("コース名")]
		crsname,
		[Value("号車")]
		gousya,
		[Value("NO SHOWフラグ")]
		noshowflg,
		[Value("NO SHOW状況表示")]
		noshow,
		[Value("乗車地１")]
		placename1,
		[Value("出発時間１")]
		syupttime1,
		[Value("乗車地２")]
		placename2,
		[Value("出発時間２")]
		syupttime2,
		[Value("乗車地３")]
		placename3,
		[Value("出発時間３")]
		syupttime3,
		[Value("乗車地４")]
		placename4,
		[Value("出発時間４")]
		syupttime4,
		[Value("乗車地５")]
		placename5,
		[Value("出発時間５")]
		syupttime5,
		[Value("予約№表示")]
		yoyakunoview,
		[Value("名前")]
		yoyakunm,
		[Value("大人")]
		jyosyaninzu1,
		[Value("中人")]
		jyosyaninzu2,
		[Value("小人")]
		jyosyaninzu3,
		[Value("電話番号")]
		telno1,
		[Value("業者名")]
		agentnm,
		[Value("申込者")]
		yykmks,
		[Value("状態表示")]
		stateview,
		[Value("座席")]
		zaseki,
		[Value("予約区分")]
		yoyakukbn,
		[Value("予約№")]
		yoyakuno,
		[Value("キャンセルフラグ")]
		cancelflg,
		[Value("座席指定予約フラグ")]
		zasekireserveyoyakuflg,
		[Value("発券内容")]
		hakkennaiyo,
		[Value("状態")]
		state,
		[Value("差分フラグ")]
		differenceflg,
		[Value("コースコード２")]
		crscd2,
		[Value("出発日")]
		syuptday,
		[Value("号車２")]
		gousya2,
		[Value("コース名２")]
		crsname2
	}

	/// <summary>
	/// カラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum Col_GetCrs : int
	{
		[Value("システム更新ＰＧＭＩＤ")]
		updatepgmid,
		[Value("システム更新者コード")]
		updatepersoncd,
		[Value("システム更新日")]
		updateday
	}

	/// <summary>
	/// カラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum Col_ReturnCrs : int
	{
		[Value("コース台帳（基本）戻し")]
		crsreturn = 1,
		[Value("コース台帳（基本）更新")]
		crsupdate
	}

	/// <summary>
	/// 対象フラグ
	/// </summary>
	/// <remarks></remarks>
	public sealed class Targetflg
	{
		[Value("対象")]
		public const string Target = "Y";
		[Value("対象外")]
		public const string NotTarget = null;
	}
	#endregion

	#region イベント

	#region 表示／非表示ボタン
	/// <summary>
	/// 条件GroupBox表示制御ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnVisiblerCondition_Click(object sender, EventArgs e)
	{

		//表示／非表示 制御
		ClientCommonKyushuUtil.setVisiblerCondition(SetButton, SetGroupBox, SetPanel, SetGrid, checkDisplayFlg, HeightgbxConditionToPanelEx1);

	}
	#endregion

	#region グリッド関連
	/// <summary>
	/// ソート処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdList_AfterSort(object sender, C1.Win.C1FlexGrid.SortColEventArgs e)
	{
		//一覧グリッドイベント時
		clickedMainGrid();
	}

	/// <summary>
	/// グリッドのチェックイベント
	/// </summary>
	/// <remarks></remarks>
	private void grdList_CellChecked(object sender, RowColEventArgs e)
	{

		//グリッド範囲外は処理しない
		if (this.grdList.RowSel < 0 || this.grdList.ColSel < 0)
		{
			return;
		}
		else
		{
			if (this.grdList.ColSel == No_Show_Koumoku.noshowflg)
			{
				if (this.grdList.GetCellCheck(this.grdList.RowSel, this.grdList.ColSel) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "0";
				}
				else
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "1";
				}
			}
		}
	}

	/// <summary>
	/// 行選択時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void grdList_RowColChange(object sender, System.EventArgs e)
	{
		//一覧グリッドイベント時
		clickedMainGrid();
	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdList_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{

		//データ件数を表示
		ClientCommonKyushuUtil.setGridCount(grdList, lblLengthGrd);

	}

	#region F4(CSV出力)ボタン押下時

	/// <summary>
	/// F4(CSV出力)ボタン押下時
	/// </summary>
	protected override void btnF4_ClickOrgProc()
	{

		//CSV出力
		makeCSV();

	}

	#endregion

	#region F10(登録)ボタン押下時

	/// <summary>
	/// 更新処理
	/// </summary>
	protected override bool insertProc()
	{

		OracleTransaction oracleTransaction = null;

		// DataAccessクラス生成
		NoShowIchiran_DA dataAccess = new NoShowIchiran_DA();

		DataTable returnCrs = new DataTable();
		DataTable returnValue = new DataTable();

		bool returnResult = false;

		// メッセージ出力(更新します。よろしいですか？)
		if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", MessageDisplay) == MsgBoxResult.Ok)
		{

			for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
			{

				if (this.grdList(i, No_Show_Koumoku.differenceflg).ToString() == Targetflg.Target)
				{

					if (CommonCheckUtil.setUsingFlg_Crs(this.grdList(i, No_Show_Koumoku.syuptday).ToString(), this.grdList(i, No_Show_Koumoku.crscd2).ToString(), this.grdList(i, No_Show_Koumoku.gousya2).ToString(), this.Name, oracleTransaction, false) == true)
					{
						// ------------------------------------
						// コース台帳（基本）が使用中でない場合
						// ------------------------------------

						// コース台帳（基本）戻し用取得
						returnCrs = null;
						returnCrs = GetCrs(System.Convert.ToString(this.grdList(i, No_Show_Koumoku.crscd2).ToString()), System.Convert.ToString(this.grdList(i, No_Show_Koumoku.syuptday).ToString()), System.Convert.ToString(this.grdList(i, No_Show_Koumoku.gousya2).ToString()));

						// コース台帳（基本）使用中フラグ更新
						returnValue = dataAccess.executeUsingFlgCrs(this.grdList(i, No_Show_Koumoku.syuptday).ToString(), this.grdList(i, No_Show_Koumoku.crscd2).ToString(), this.grdList(i, No_Show_Koumoku.gousya2).ToString(), this.Name);

						if (CommonCheckUtil.setUsingFlg_Yoyaku(this.grdList(i, No_Show_Koumoku.yoyakukbn).ToString(), this.grdList(i, No_Show_Koumoku.yoyakuno).ToString(), this.Name, oracleTransaction, false) == true)
						{
							// ------------------------------------
							// 予約情報（基本）が使用中でない場合
							// ------------------------------------

							// 予約情報（基本）使用中フラグ更新
							returnValue = dataAccess.executeUsingFlgYoyaku(this.grdList(i, No_Show_Koumoku.yoyakukbn).ToString(), this.grdList(i, No_Show_Koumoku.yoyakuno).ToString(), this.Name);

							// 予約情報（基本）更新
							UpdateNoShowIchiran(i);

							// コース台帳（基本）更新
							UpdateReturnCrsData((int)Col_ReturnCrs.crsupdate, i);

							returnResult = true;
						}
						else
						{
							// ------------------------------------
							// コース台帳（基本）が使用中の場合
							// ------------------------------------

							// コース台帳（基本）戻し更新
							crsUpdatePgmid = System.Convert.ToString(returnCrs.Rows(0).Item(Col_GetCrs.updatepgmid).ToString()); // システム更新ＰＧＭＩＤ
							crsUpdatePersonCd = System.Convert.ToString(returnCrs.Rows(0).Item(Col_GetCrs.updatepersoncd).ToString()); // システム更新者コード
							crstemUpdateDay = System.Convert.ToDateTime(returnCrs.Rows(0).Item(Col_GetCrs.updateday).ToString()); // システム更新日
							UpdateReturnCrsData((int)Col_ReturnCrs.crsreturn, i);
						}
					}
				}
			}

			if (returnResult == false)
			{
				//他端末で使用中のため、更新対象がありません。
				CommonProcess.createFactoryMsg().messageDisp("E03_022");
				return false;
			}

		}

		return returnResult;

	}

	/// <summary>
	/// 登録処理前のチェック
	/// </summary>
	/// <returns>True:エラーなし False:エラーあり</returns>
	protected override bool checkBeforeInsert()
	{

		bool differenceFlg = false;

		//更新前のグリッドを退避
		updateTargetData = null;
		updateTargetData = ((DataTable)this.grdList.DataSource).Copy;

		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (checkNoshowDifference(i) == true)
			{

				this.grdList[i, No_Show_Koumoku.differenceflg] = Targetflg.Target;

				differenceFlg = true;
			}
		}

		if (differenceFlg == false)
		{
			// 入力に差分がない場合、エラーメッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_049");
			return false;
		}

		return true;

	}

	#endregion

	#region    セルボタン押下イベント
	/// <summary>
	/// grdコード_CellButtonClick
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void grdList_CellButtonClick(object sender, RowColEventArgs e)
	{

		string YoyakuKbn = string.Empty;
		string YoyakuNo = string.Empty;

		if (this.grdList.GetData(e.Row, No_Show_Koumoku.yoyakukbn) IsNot DBNull.Value)
		{
			YoyakuKbn = System.Convert.ToString(this.grdList.GetData(e.Row, No_Show_Koumoku.yoyakukbn)).Trim();
		}

		if (this.grdList.GetData(e.Row, No_Show_Koumoku.yoyakuno) IsNot DBNull.Value)
		{
			YoyakuNo = System.Convert.ToString(this.grdList.GetData(e.Row, No_Show_Koumoku.yoyakuno)).Trim();
		}

		// 予約登録画面に遷移
		using (S02_0103 form = new S02_0103())
		{
			S02_0103ParamData prm = new S02_0103ParamData();
			prm.ScreenMode = CommonRegistYoyaku.ScreenModeReference; // 画面モードに参照を設定
			prm.YoyakuNo = int.Parse(YoyakuNo); // 予約No
			prm.YoyakuKbn = YoyakuKbn; // 予約区分
			form.ParamData = prm;
			form.ShowDialog();
		}


	}
	#endregion

	#region    条件クリア
	/// <summary>
	/// 条件クリアボタン押下時
	/// </summary>
	protected override void btnCLEAR_ClickOrgProc()
	{

		// 検索条件部の項目初期化
		initSearchAreaItems();

		// データ件数を表示
		ClientCommonKyushuUtil.setGridCount(grdList, lblLengthGrd);

		// 使用不可に設定
		this.F4Key_Enabled = false; // CSV出力ボタン
		this.F10Key_Enabled = false; // 登録ボタン

	}

	#endregion

	#region フォーム
	private void PT11_KeyDown(object sender, KeyEventArgs e)
	{

		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			base.btnCom_Click(this.btnSearch, e);
		}
		else
		{
			return;
		}

	}
	#endregion

	#endregion

	#endregion

	#region 初期化処理
	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{

		DataTable returnValue = null;

		//Controlの初期値を設定
		CommonUtil.Control_Init(this.gbxCondition.Controls);

		//エラーの初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		this.dtmSyuptDay.Value = CommonDateUtil.getSystemTime(); // 出発日
		this.rdoCrsKind1Gaikokugo.Checked = false; // 外国語
		this.rdoCrsKind1Nihongo.Checked = false; // 日本語

		//所属部署が国際事業部の場合、外国語にチェック
		if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
		{
			this.rdoCrsKind1Gaikokugo.Checked = true; // 外国語のみチェック
		}
		else
		{
			this.rdoCrsKind1Nihongo.Checked = true; // 日本語コースのみチェック
		}

		this.rdoCrsKind2Teiki.Checked = true; // 定期
		this.rdoCrsKind2Kikaku.Checked = false; // 企画

		this.ucoJyosyachiCd.setControlInitiarize(); // 乗車地
		this.chkMicheckin_Mihakken.Checked = false; // 未チェックイン・未発券含む

		//場所コード・名称の表示
		returnValue = GetDbTablePlaceMaster();
		if (returnValue.Rows.Count > 0)
		{
			ucoJyosyachiCd.CodeText = System.Convert.ToString(returnValue.Rows(0)["PLACE_CD"]);
			ucoJyosyachiCd.ValueText = System.Convert.ToString(returnValue.Rows(0)["PLACE_NAME_1"]);
		}

	}
	#endregion

	#region チェック系
	/// <summary>
	/// 検索入力項目チェック
	/// </summary>
	protected override bool checkSearchItems()
	{

		// 必須チェック用フラグ
		bool CheckHissuFlg;

		//エラー表示項目
		string errorDisplay = string.Empty;

		//エラーの初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		CheckHissuFlg = true;

		// 必須チェック
		if (CommonUtil.checkHissuError(this.gbxCondition.Controls) == true)
		{

			//必須エラーフォーカス設定（エラーが発生した先頭にフォーカスを当てる）
			if (dtmSyuptDay.ExistError == true)
			{
				dtmSyuptDay.Focus();
			}

			CheckHissuFlg = false;

		}

		if (CheckHissuFlg == false)
		{
			// 共通メッセージ処理 [E90_022 未入力項目があります。]
			CommonProcess.createFactoryMsg().messageDisp("E90_022");

			return false;
		}

		return true;

	}
	#endregion

	#region 固有初期処理
	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{

		//ベースフォームの初期化処理
		base.initScreenPerttern();

		//データ件数を表示
		ClientCommonKyushuUtil.setGridCount(grdList, lblLengthGrd);

		// セルボタンを常に表示 (初期値：Inherit)
		this.grdList.ShowButtons = ShowButtonsEnum.Always;

		// グリッドのソート不可
		this.grdList.AllowSorting = AllowSortingEnum.None;

		//セルボタン表示
		this.grdList.Cols(No_Show_Koumoku.yoyakunoview).ComboList = "...";

		//ボタンの関連付け
		btnSearch.Click += base.btnCom_Click;
		btnClear.Click += base.btnCom_Click;

		//非使用ボタン設定
		this.F11Key_Visible = false; // F11:更新

		base.UpdateMsgParam = this.UpdateMsg;
		base.InsertMsgParam = this.InsertMsg;

		//グリッド初期化
		setGridInitialize();

		//表示／非表示ボタン
		SetButton = TryCast(this.btnVisiblerCondition, ButtonEx);
		SetGroupBox = TryCast(this.gbxCondition, GroupBoxEx);
		SetPanel = TryCast(this.PanelEx1, PanelEx);
		SetGrid = TryCast(this.grdList, FlexGridEx);

		HeightgbxConditionToPanelEx1 = System.Convert.ToInt32(SetPanel.Top - (SetGroupBox.Top + SetGroupBox.Height)); //検索条件と一覧の間


		//初期フォーカスのコントロールを設定を実装
		this.dtmSyuptDay.Select();

		// 初期表示時使用不可に設定
		this.F4Key_Enabled = false; // CSV出力ボタン
		this.F10Key_Enabled = false; // 登録ボタン

	}
	#endregion

	#region Grid、データ関連
	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void reloadGrid()
	{

		//取得結果をグリッドへ設定
		object with_1 = this.grdList;
		with_1.DataSource = base.SearchResultGridData;
		//グリッド設定
		setGridItem();
		if (with_1.Rows.Count == with_1.Rows.Fixed)
		{
			this.F4Key_Enabled = false; // CSV出力ボタンを使用不可に設定
			this.F10Key_Enabled = false; // 登録ボタンを使用不可に設定
		}
		else
		{
			this.F4Key_Enabled = true; // CSV出力ボタンを使用可に設定
			this.F10Key_Enabled = true; // 登録ボタンを使用可に設定
		}

		// 検索時のグリッドを差分用に退避
		selectOldData = null;
		selectOldData = ((DataTable)this.grdList.DataSource).Copy;

	}
	#endregion

	#region DB関連
	//'■■■ＤＢ関連処理■■■
	/// <summary>
	/// キー値での問合せ
	/// </summary>
	protected override DataTable getMstDataByPrimaryKey()
	{
		return GetDbTable(DbTableKbn.Key);
	}

	/// <summary>
	/// 対象マスタのデータ取得
	/// </summary>
	protected override DataTable getMstData()
	{
		return GetDbTable(DbTableKbn.Select);
	}

	#endregion

	#region 実装用メソッド(画面毎に変更)

	#region 初期処理

	/// <summary>
	/// グリッド初期化
	/// </summary>
	/// <remarks></remarks>
	private void setGridInitialize()
	{

		C1.Win.C1FlexGrid.CellRange rng = null;

		object with_1 = this.grdList;
		with_1.DataSource = null;
		with_1.Rows.Count = with_1.Rows.Fixed;

		// ヘッダ設定
		with_1.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;

		rng = with_1.GetCellRange(0, No_Show_Koumoku.crscd, 1, No_Show_Koumoku.crscd);
		rng.Data = "コース" + Constants.vbLf + "コード";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.crsname, 1, No_Show_Koumoku.crsname);
		rng.Data = "コース名";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.gousya, 1, No_Show_Koumoku.gousya);
		rng.Data = "号車";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.noshowflg, 1, No_Show_Koumoku.noshowflg);
		rng.Data = "NO SHOW";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.noshow, 1, No_Show_Koumoku.noshow);
		rng.Data = "NO SHOW状況";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.placename1, 0, No_Show_Koumoku.syupttime5);
		rng.Data = "乗車場所・出発時間";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(1, No_Show_Koumoku.placename1, 1, No_Show_Koumoku.placename1);
		rng.Data = "乗車地1";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.syupttime1, 1, No_Show_Koumoku.syupttime1);
		rng.Data = "時間1";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.placename2, 1, No_Show_Koumoku.placename2);
		rng.Data = "乗車地2";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.syupttime2, 1, No_Show_Koumoku.syupttime2);
		rng.Data = "時間2";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.placename3, 1, No_Show_Koumoku.placename3);
		rng.Data = "乗車地3";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.syupttime3, 1, No_Show_Koumoku.syupttime3);
		rng.Data = "時間3";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.placename4, 1, No_Show_Koumoku.placename4);
		rng.Data = "乗車地4";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.syupttime4, 1, No_Show_Koumoku.syupttime4);
		rng.Data = "時間4";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.placename5, 1, No_Show_Koumoku.placename5);
		rng.Data = "乗車地5";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.syupttime5, 1, No_Show_Koumoku.syupttime5);
		rng.Data = "時間5";

		rng = with_1.GetCellRange(0, No_Show_Koumoku.yoyakunoview, 1, No_Show_Koumoku.yoyakunoview);
		rng.Data = "予約№";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.yoyakunm, 1, No_Show_Koumoku.yoyakunm);
		rng.Data = "名前";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.jyosyaninzu1, 0, No_Show_Koumoku.jyosyaninzu3);
		rng.Data = "乗車人数";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(1, No_Show_Koumoku.jyosyaninzu1, 1, No_Show_Koumoku.jyosyaninzu1);
		rng.Data = "大人";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.jyosyaninzu2, 1, No_Show_Koumoku.jyosyaninzu2);
		rng.Data = "中人";

		rng = with_1.GetCellRange(1, No_Show_Koumoku.jyosyaninzu3, 1, No_Show_Koumoku.jyosyaninzu3);
		rng.Data = "小人";

		rng = with_1.GetCellRange(0, No_Show_Koumoku.telno1, 1, No_Show_Koumoku.telno1);
		rng.Data = "電話番号";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.agentnm, 1, No_Show_Koumoku.agentnm);
		rng.Data = "業者名";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.yykmks, 1, No_Show_Koumoku.yykmks);
		rng.Data = "申込者";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.stateview, 1, No_Show_Koumoku.stateview);
		rng.Data = "状態";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.zaseki, 1, No_Show_Koumoku.zaseki);
		rng.Data = "座席";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.yoyakukbn, 1, No_Show_Koumoku.yoyakukbn);
		rng.Data = "予約区分";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.yoyakuno, 1, No_Show_Koumoku.yoyakuno);
		rng.Data = "予約№";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.cancelflg, 1, No_Show_Koumoku.cancelflg);
		rng.Data = "キャンセルフラグ";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.zasekireserveyoyakuflg, 1, No_Show_Koumoku.zasekireserveyoyakuflg);
		rng.Data = "座席指定予約フラグ";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.hakkennaiyo, 1, No_Show_Koumoku.hakkennaiyo);
		rng.Data = "発券内容";
		with_1.MergedRanges.Add(rng);

		rng = with_1.GetCellRange(0, No_Show_Koumoku.state, 1, No_Show_Koumoku.state);
		rng.Data = "状態";
		with_1.MergedRanges.Add(rng);

		// 静止列 設定 (NO SHOWまで)
		grdList.Cols.Frozen = No_Show_Koumoku.noshowflg;

		//NO SHOW 状態 は非表示
		with_1.Cols(No_Show_Koumoku.noshow).Visible = false;

		// 座席 ～ コース名２ は非表示
		for (int idx = No_Show_Koumoku.zaseki; idx <= No_Show_Koumoku.crsname2; idx++)
		{
			with_1.Cols(idx).Visible = false;
		}


	}

	#endregion

	#region グリッド設定

	/// <summary>
	/// グリッド設定
	/// </summary>
	/// <remarks></remarks>
	private void setGridItem()
	{

		int row = 0;

		object with_1 = this.grdList;

		// セルの配置と自動サイズ調整
		with_1.Styles.Fixed.TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;

		if (with_1.Rows.Count > 1)
		{
			for (int i = 2; i <= with_1.Rows.Count - 1; i++)
			{
				if (System.Convert.ToString(this.grdList(i, No_Show_Koumoku.crscd2)) == System.Convert.ToString(this.grdList(i - 1, No_Show_Koumoku.crscd2)) &&)
				{
					System.Convert.ToDecimal(this.grdList(i, No_Show_Koumoku.gousya2)) = System.Convert.ToDecimal(System.Convert.ToDecimal(this.grdList(i - 1, No_Show_Koumoku.gousya2)));
					this.grdList[i, No_Show_Koumoku.crscd] = string.Empty;
					this.grdList[i, No_Show_Koumoku.crsname] = string.Empty;
					this.grdList[i, No_Show_Koumoku.gousya] = string.Empty;
				}
			}
		}

		for (row = 2; row <= with_1.Rows.Count - 1; row++)
		{
			if (string.IsNullOrWhiteSpace(this.grdList.GetData(row, No_Show_Koumoku.yoyakunoview).ToString()))
			{
				// 該当行を入力不可にする
				this.grdList.Rows(row).AllowEditing = false;

				// NoShow(チェックボックス)が表示されないようにする
				CellStyle cellStyle = this.grdList.Styles.Add("String");
				cellStyle.DataType = typeof(string);
				this.grdList.SetCellStyle(row, No_Show_Koumoku.noshowflg, cellStyle);

				this.grdList.SetData(row, No_Show_Koumoku.noshowflg, string.Empty);
				this.grdList.SetData(row, No_Show_Koumoku.jyosyaninzu1, string.Empty);
				this.grdList.SetData(row, No_Show_Koumoku.jyosyaninzu2, string.Empty);
				this.grdList.SetData(row, No_Show_Koumoku.jyosyaninzu3, string.Empty);
				this.grdList.SetData(row, No_Show_Koumoku.stateview, string.Empty);
			}
		}

	}

	/// <summary>
	/// グリッド編集
	/// </summary>
	/// <param name="paramGridData">データテーブル</param>
	private DataTable setGridDataInfo(DataTable paramGridData)
	{

		//戻り値
		DataTable returnValue = null;

		//予約№編集
		string editYoyakuNumber = string.Empty;

		//共通処理用（状態取得）
		string[] hakkenStateArry = null;

		foreach (DataRow row in paramGridData.Rows)
		{
			// 予約番号をカンマ編集後設定
			if (!string.IsNullOrEmpty(System.Convert.ToString(row["YOYAKU_KBN"].ToString())))
			{
				// 共通処理にてカンマ編集（パラメータ：予約区分、予約NOを文字列結合した値）
				//editYoyakuNumber = CommonCheckUtil.editYoyakuNo(row("YOYAKU_KBN").ToString() & CType(String.Format("{0:D9}", CType(row("YOYAKU_NO").ToString(), Integer)), String))
				editYoyakuNumber = System.Convert.ToString(CommonRegistYoyaku.createManagementNumber(row["YOYAKU_KBN"].ToString(), System.Convert.ToInt32(row["YOYAKU_NO"].ToString())));
				row["YOYAKU_NO_VIEW"] = editYoyakuNumber;
			}

			// 状態に表示する文言を取得後設定
			// 共通処理にて取得（パラメータ：キャンセルフラグ、座席指定予約フラグ、発券内容、状態）
			hakkenStateArry = CommonDAUtil.getYoyakuHakkenState(row["CANCEL_FLG"].ToString(),;
			row["ZASEKI_RESERVE_YOYAKU_FLG"].ToString(),;
			row["HAKKEN_NAIYO"].ToString(),;
			row["STATE"].ToString());
			row["STATE_VIEW"] = hakkenStateArry[1];
		}

		return paramGridData;

		return returnValue;

	}

	#endregion

	#region チェック処理(Private)

	#region 変更確認

	/// <summary>
	/// 差分チェック
	/// </summary>
	protected override bool checkDifference()
	{

		//入力差分チェック
		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (checkNoshowDifference(i) == true)
			{
				return true;
			}
		}

		return false;

	}

	/// <summary>
	/// 入力差分チェック処理
	/// </summary>
	/// <param name="i">行数</param>
	/// <returns>True:差分あり False:差分なし</returns>
	private bool checkNoshowDifference(int i)
	{

		//使用不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		// チェック処理
		foreach (DataRow row in selectOldData.Rows)
		{
			if (row["YOYAKU_KBN"].Equals(this.grdList(i, No_Show_Koumoku.yoyakukbn)) &&)
			{
				row["YOYAKU_NO"].Equals(this.grdList(i, No_Show_Koumoku.yoyakuno));
				if (!row["NO_SHOW_FLG"].Equals(this.grdList(i, No_Show_Koumoku.noshowflg)))
				{
					return true;
					//					break;
				}
			}
		}

		return false;

	}
	#endregion

	#endregion

	#region DB取得処理
	/// <summary>
	/// 場所コード・名称検索処理
	/// </summary>
	/// <returns>取得データ(DataTable)</returns>
	private DataTable GetDbTablePlaceMaster()
	{
		//戻り値
		DataTable returnValue = null;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		//DataAccessクラス生成
		Place_DA dataAccess = new Place_DA();

		//場所マスタエンティティ
		PlaceMasterEntity clsPlaceMasterEntity = new PlaceMasterEntity();

		//パラメータ設定
		//会社コード
		paramInfoList.Add(clsPlaceMasterEntity.CompanyCd.PhysicsName, UserInfoManagement.companyCd);
		//営業所コード
		paramInfoList.Add(clsPlaceMasterEntity.EigyosyoCd.PhysicsName, UserInfoManagement.eigyosyoCd);

		try
		{
			//場所マスタ検索
			returnValue = dataAccess.GetPlaceMasterDataLoginUser(paramInfoList);
		}
		catch (OracleException)
		{
			throw;
		}
		catch (Exception)
		{
			throw;
		}

		return returnValue;

	}

	/// <summary>
	/// データ取得処理(必須画面個別実装)
	/// </summary>
	/// <returns>取得データ(DataTable)</returns>
	private DataTable GetDbTable(DbTableKbn kbn)
	{

		// 戻り値
		DataTable returnValue = null;
		// DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		// DataAccessクラス生成
		NoShowIchiran_DA dataAccess = new NoShowIchiran_DA();

		// 出発日
		if (!(ReferenceEquals(this.dtmSyuptDay.Text, null)))
		{
			paramInfoList.Add("SyuptDay", Strings.Replace(System.Convert.ToString(this.dtmSyuptDay.Value), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));
		}

		// 日本語
		if (this.rdoCrsKind1Nihongo.Checked == true)
		{
			paramInfoList.Add("CrsKind1Nihongo", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("CrsKind1Nihongo", FixedCd.CheckBoxValue.OffValue);
		}
		// 外国語
		if (this.rdoCrsKind1Gaikokugo.Checked == true)
		{
			paramInfoList.Add("CrsKind1Gaikokugo", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("CrsKind1Gaikokugo", FixedCd.CheckBoxValue.OffValue);
		}
		// 定期
		if (this.rdoCrsKind2Teiki.Checked == true)
		{
			paramInfoList.Add("CrsKind2Teiki", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("CrsKind2Teiki", FixedCd.CheckBoxValue.OffValue);
		}
		// 企画
		if (this.rdoCrsKind2Kikaku.Checked == true)
		{
			paramInfoList.Add("CrsKind2Kikaku", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("CrsKind2Kikaku", FixedCd.CheckBoxValue.OffValue);
		}
		// 乗車地
		string JyosyachiCd = System.Convert.ToString(this.ucoJyosyachiCd.CodeText);
		if (!string.IsNullOrEmpty(JyosyachiCd))
		{
			paramInfoList.Add("JyosyachiCd", JyosyachiCd);
		}
		// 未チェックイン・未発券含む
		if (this.chkMicheckin_Mihakken.Checked == true)
		{
			paramInfoList.Add("MicheckinMihakken", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("MicheckinMihakken", FixedCd.CheckBoxValue.OffValue);
		}

		try
		{
			DataTable getValue = null;

			getValue = dataAccess.accessNoShowIchiran(NoShowIchiran_DA.accessType.getNoShowIchiran, paramInfoList);

			if (getValue.Rows.Count > 0)
			{
				// グリッドへの設定
				returnValue = setGridDataInfo(getValue);
			}
			else
			{
				returnValue = getValue;
			}

		}
		catch (OracleException)
		{
			throw;
		}
		catch (Exception)
		{
			throw;
		}

		return returnValue;

	}

	/// <summary>
	/// データ取得処理(必須画面個別実装)
	/// </summary>
	/// <returns>取得データ(DataTable)</returns>
	private DataTable GetCrs(string _CRS_CD, string _SYUPT_DAY, string _GOUSYA)
	{

		// 戻り値
		DataTable returnValue = null;
		// DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		// DataAccessクラス生成
		NoShowIchiran_DA dataAccess = new NoShowIchiran_DA();

		paramInfoList.Add("SyuptDay", _SYUPT_DAY);
		paramInfoList.Add("CrsCd", _CRS_CD);
		paramInfoList.Add("Gousya", _GOUSYA);

		try
		{
			returnValue = dataAccess.accessNoShowIchiran(NoShowIchiran_DA.accessType.getReturnCrs, paramInfoList);
		}
		catch (OracleException)
		{
			throw;
		}
		catch (Exception)
		{
			throw;
		}

		return returnValue;

	}
	#endregion

	#region DB更新処理
	/// <summary>
	/// 更新実行
	/// </summary>
	/// <param name="i">行数</param>
	/// <returns>更新処理件数</returns>
	private int UpdateNoShowIchiran(int i)
	{

		// 戻り値
		int returnValue = 0;
		// DataAccessクラス生成
		NoShowIchiran_DA dataAccess = new NoShowIchiran_DA();
		// パラメータ
		Hashtable paramInfoList = new Hashtable();

		paramInfoList.Add("noShowFlg", System.Convert.ToString(this.grdList.GetData(i, No_Show_Koumoku.noshowflg)));
		paramInfoList.Add("yoyakuKbn", System.Convert.ToString(this.grdList.GetData(i, No_Show_Koumoku.yoyakukbn)));
		paramInfoList.Add("yoyakuNo", System.Convert.ToInt32(this.grdList.GetData(i, No_Show_Koumoku.yoyakuno)));
		paramInfoList.Add("systemUpdateDay", CommonProcess.getDateTime());
		paramInfoList.Add("systemUpdatePgmid", this.Name);
		paramInfoList.Add("systemUpdatePersonCd", UserInfoManagement.userId);

		try
		{
			//更新処理
			returnValue = System.Convert.ToInt32(dataAccess.executeNoShowIchiran(NoShowIchiran_DA.accessType.executeUpdateNoShowIchiran, paramInfoList));

		}
		catch (OracleException)
		{
			throw;
		}
		catch (Exception)
		{
			throw;
		}

		return returnValue;
	}

	/// <summary>
	/// 更新実行
	/// </summary>
	/// <param name="Syori">処理（1:戻し, 2：更新）</param>
	/// <param name="i">行数</param>
	/// <returns>更新処理件数</returns>
	private int UpdateReturnCrsData(int Syori, int i)
	{

		// 戻り値
		int returnValue = 0;
		// DataAccessクラス生成
		NoShowIchiran_DA dataAccess = new NoShowIchiran_DA();
		// パラメータ
		Hashtable paramInfoList = new Hashtable();

		paramInfoList.Add("SyuptDay", System.Convert.ToString(this.grdList.GetData(i, No_Show_Koumoku.syuptday)));
		paramInfoList.Add("CrsCd", System.Convert.ToString(this.grdList.GetData(i, No_Show_Koumoku.crscd2)));
		paramInfoList.Add("Gousya", System.Convert.ToString(this.grdList.GetData(i, No_Show_Koumoku.gousya2)));

		switch (Syori)
		{
			case (int)Col_ReturnCrs.crsreturn:
				// コース台帳（基本）戻し
				paramInfoList.Add("systemUpdateDay", crstemUpdateDay);
				paramInfoList.Add("systemUpdatePgmid", crsUpdatePgmid);
				paramInfoList.Add("systemUpdatePersonCd", crsUpdatePersonCd);
				break;
			case (int)Col_ReturnCrs.crsupdate:
				// コース台帳（基本）更新
				paramInfoList.Add("systemUpdateDay", CommonProcess.getDateTime());
				paramInfoList.Add("systemUpdatePgmid", this.Name);
				paramInfoList.Add("systemUpdatePersonCd", UserInfoManagement.userId);
				break;
			default:
				// 該当処理なし
				return returnValue;
		}

		try
		{
			//更新処理
			returnValue = System.Convert.ToInt32(dataAccess.executeNoShowIchiran(NoShowIchiran_DA.accessType.executeReturnCrsData, paramInfoList));

		}
		catch (OracleException)
		{
			throw;
		}
		catch (Exception)
		{
			throw;
		}

		return returnValue;
	}
	#endregion

	#region CSV出力処理

	/// <summary>
	/// CSV出力
	/// </summary>
	/// <remarks></remarks>
	private void makeCSV()
	{

		System.IO.StreamWriter sw = null;
		string buf = "";

		try
		{
			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				//デフォルトのファイル名を指定します
				sfd.FileName = "NOSHOWリスト_" + CommonDateUtil.getSystemTime().ToString("yyyyMMdd_HHmmss") + ".csv";

				if (selectOldData.Rows.Count > 0)
				{
					if (sfd.ShowDialog() == DialogResult.OK)
					{
						sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8);
						// ヘッダ出力
						buf = string.Empty;
						buf = buf + "\"" + "コースコード" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "コース名" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "乗車地１" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "出発時間１" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "乗車地２" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "出発時間２" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "乗車地３" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "出発時間３" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "乗車地４" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "出発時間４" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "乗車地５" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "出発時間５" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "号車" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "予約No" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "名前" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "大人" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "中人" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "小人" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "電話番号" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "業者名" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "申込者" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "状態" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "NOSHOW状態" + "\"";
						buf = buf + ",";
						buf = buf + "\"" + "座席" + "\"";
						// ファイル出力
						sw.WriteLine(buf);
						// データ出力
						foreach (DataRow row in selectOldData.Rows)
						{
							buf = string.Empty;
							buf = buf + "\"" + row["CRS_CD2"].ToString() + "\""; // コースコード
							buf = buf + ",";
							buf = buf + "\"" + row["CRS_NAME2"].ToString() + "\""; // コース名
							buf = buf + ",";
							buf = buf + "\"" + row["PLACE_NAME_1"].ToString() + "\""; // 乗車地１
							buf = buf + ",";
							buf = buf + "\"" + row["SYUPT_TIME_1"].ToString() + "\""; // 出発時間１
							buf = buf + ",";
							buf = buf + "\"" + row["PLACE_NAME_2"].ToString() + "\""; // 乗車地２
							buf = buf + ",";
							buf = buf + "\"" + row["SYUPT_TIME_2"].ToString() + "\""; // 出発時間２
							buf = buf + ",";
							buf = buf + "\"" + row["PLACE_NAME_3"].ToString() + "\""; // 乗車地３
							buf = buf + ",";
							buf = buf + "\"" + row["SYUPT_TIME_3"].ToString() + "\""; // 出発時間３
							buf = buf + ",";
							buf = buf + "\"" + row["PLACE_NAME_4"].ToString() + "\""; // 乗車地４
							buf = buf + ",";
							buf = buf + "\"" + row["SYUPT_TIME_4"].ToString() + "\""; // 出発時間４
							buf = buf + ",";
							buf = buf + "\"" + row["PLACE_NAME_5"].ToString() + "\""; // 乗車地５
							buf = buf + ",";
							buf = buf + "\"" + row["SYUPT_TIME_5"].ToString() + "\""; // 出発時間５
							buf = buf + ",";
							buf = buf + "\"" + row["GOUSYA2"].ToString() + "\""; // 号車
							buf = buf + ",";
							buf = buf + "\"" + Strings.Replace(row["YOYAKU_NO_VIEW"].ToString(), ",", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0) + "\""; // 予約No
							buf = buf + ",";
							buf = buf + "\"" + row["YOYAKU_NM"].ToString() + "\""; // 名前
							buf = buf + ",";
							buf = buf + "\"" + row["JYOSYA_NINZU_1"].ToString() + "\""; // 大人
							buf = buf + ",";
							buf = buf + "\"" + row["JYOSYA_NINZU_2"].ToString() + "\""; // 中人
							buf = buf + ",";
							buf = buf + "\"" + row["JYOSYA_NINZU_3"].ToString() + "\""; // 小人
							buf = buf + ",";
							buf = buf + "\"" + row["TEL_NO_1"].ToString() + "\""; // 電話番号
							buf = buf + ",";
							buf = buf + "\"" + row["AGENT_NM"].ToString() + "\""; // 業者名
							buf = buf + ",";
							buf = buf + "\"" + row["YYKMKS"].ToString() + "\""; // 申込者
							buf = buf + ",";
							buf = buf + "\"" + row["STATE_VIEW"].ToString() + "\""; // 状態
							buf = buf + ",";
							buf = buf + "\"" + row["NO_SHOW"].ToString() + "\""; // NOSHOW状態
							buf = buf + ",";
							buf = buf + "\"" + row["ZASEKI"].ToString() + "\""; // 座席

							// ファイル出力
							sw.WriteLine(buf);
						}
						// 正常終了メッセージ
						// I90_001 {1}を出力しました。
						createFactoryMsg().messageDisp("I90_001", MessageDisplayCsvfile);
					}
				}
			}


			if (ReferenceEquals(sw, null) == false)
			{
				sw.Close();
			}
		}
		catch (Exception)
		{
			// 異常終了メッセージ
			// E90_028 ファイル出力エラーが発生しました。[{1}]処理を中断します。
			createFactoryMsg().messageDisp("E90_028", MessageDisplayCsvout);
			throw;
		}

	}

	#endregion

	#endregion

}