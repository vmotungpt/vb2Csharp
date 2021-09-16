using System.ComponentModel;
using Hatobus.ReservationManagementSystem.Master;


/// <summary>
/// ピックアップ用リスト画面
/// </summary>
public class S04_0602 : PT01, iPT01
{

	#region 定数

	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	public const int TOP_GBX_CONDITION = 41;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	public const int MARGIN_GBX_CONDITION = 6;

	/// <summary>
	/// 条件GroupBox非表示時のGrouBoxAreaの高さ
	/// </summary>
	public const int HeightGbxAreasOnNotVisibleCondition = 700;

	private Hashtable _searchConditionCache = new Hashtable(); //入力条件の退避

	#endregion
	#region フィールド
	/// <summary>
	/// 条件GroupBoxの高さ
	/// </summary>
	private int _heightGbxCondition;

	/// <summary>
	/// PanelEx1の高さ
	/// </summary>
	private int _heightPnlArea1;

	/// <summary>
	/// PanelEx1のTop座標
	/// </summary>
	private int _topPnlArea1;
	#endregion
	#region プロパティ

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

			SetGrpLayout();
		}
		else
		{
			//非表示状態
			this.btnVisiblerCondition.Text = "表示 >>";

			this.PanelEx1.Top = TOP_GBX_CONDITION;
			this.PanelEx1.Height = HeightGbxAreasOnNotVisibleCondition;
		}
	}

	/// <summary>
	/// GroupBoxのレイアウト保存
	/// </summary>
	private void SaveGrpLayout()
	{
		this._heightGbxCondition = System.Convert.ToInt32(this.gbxCondition.Height);
		this._heightPnlArea1 = System.Convert.ToInt32(this.PanelEx1.Height);
		this._topPnlArea1 = System.Convert.ToInt32(this.PanelEx1.Top);

	}

	/// <summary>
	/// GroupBoxのレイアウト設定
	/// </summary>
	private void SetGrpLayout()
	{
		this.gbxCondition.Height = this._heightGbxCondition;
		this.PanelEx1.Height = this._heightPnlArea1;
		this.PanelEx1.Top = this._topPnlArea1;

	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdPickup_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		FlexGridEx grd = TryCast(sender, FlexGridEx);
		if (ReferenceEquals(grd, null))
		{
			return;
		}

		//データ件数を表示(ヘッダー行分マイナス1)
		string formatedCount = System.Convert.ToString((grd.Rows.Count - 1).ToString().PadLeft(3));
		this.lblLengthGrd.Text = formatedCount + "件";
	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdRoot_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		FlexGridEx grd = TryCast(sender, FlexGridEx);
		if (ReferenceEquals(grd, null))
		{
			return;
		}

		//データ件数を表示(ヘッダー行分マイナス1)
		string formatedCount = System.Convert.ToString((grd.Rows.Count - 1).ToString().PadLeft(3));
		this.lblLengthGrd.Text = formatedCount + "件";
	}

	#region 検索

	#region フォーム
	private void PT21_KeyDown(object sender, KeyEventArgs e)
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

	#region FormBaseから
	/// <summary>
	/// CLEARボタン押下時の独自処理
	/// </summary>
	protected override void btnCLEAR_ClickOrgProc()
	{
		initSearchAreaItems();
		this.ucoJyochachiCd.CodeText = string.Empty;
		this.ucoJyochachiCd.ValueText = string.Empty;
		this.rdoPickupJyosyaTi.Checked = true;
		this.rdoRoot.Checked = false;

		this.grdPickup.DataSource = new DataTable();
		this.grdRoot.DataSource = new DataTable();
	}

	/// <summary>
	/// F8：検索ボタン押下イベント
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{
		//チェックして検索
		if (checkSearchItems() == false)
		{
			return;
		}
		else
		{
			this.setPickupAndRoot();
		}
	}
	#endregion

	#region PT01オーバーライド(基本的には変えない)

	#region 初期化処理
	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{

		CommonUtil.Control_Init(this.gbxCondition.Controls);

		//背景色初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);
	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	protected override void initFooterButtonControl()
	{

		//Visible
		//TODO:ボタンの表示/非表示を変更
		this.F1Key_Visible = false;
		this.F2Key_Visible = true;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		this.F7Key_Visible = false;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = false;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;


		this.F2Key_Text = "F2:戻る";

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

		//初期処理
		this.setSeFirsttDisplayData();

		btnClear.Click += base.btnCom_Click;
		btnSearch.Click += base.btnCom_Click;

		// ロード時にフォーカスを設定する
		this.ActiveControl = this.dtmSyuptDay;
	}
	#endregion

	#region チェック系
	/// <summary>
	/// 検索入力項目チェック
	/// </summary>
	protected override bool checkSearchItems()
	{

		//検索処理前のチェック処理を実装(メッセージは画面側でしてください)
		//背景色初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		return this.CheckSearch();
	}
	#endregion

	#endregion

	#region メソッド

	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	private void setPickupAndRoot()
	{

		if (this.rdoPickupJyosyaTi.Checked)
		{

			this.grdPickup.Visible = true;
			this.grdRoot.Visible = false;

			this.SetPickupInfo();
		}
		else
		{

			this.grdPickup.Visible = false;
			this.grdRoot.Visible = true;

			this.SetRootInfo();
		}
	}

	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// (ピックアップ乗車地選択時)
	/// </summary>
	private void SetPickupInfo()
	{

		// ピックアップ乗車地・ルートグリッドの検索結果(ピックアップ乗車地選択時)
		DataTable searchResultGridPickup = new DataTable();

		//検索処理
		searchResultGridPickup = GetPickupAndRootData();
		this.grdPickup.DataSource = searchResultGridPickup;

		if (searchResultGridPickup.Rows.Count == 0)
		{
			//取得件数0件の場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_019");
			this.grdPickup.Focus();
		}
	}

	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// (ルート選択時)
	/// </summary>
	private void SetRootInfo()
	{

		// ピックアップ乗車地・ルートグリッドの検索結果(ルート選択時)
		DataTable searchResultGridRoot = new DataTable();

		//検索処理
		searchResultGridRoot = GetPickupAndRootData();
		this.grdRoot.DataSource = searchResultGridRoot;
		if (searchResultGridRoot.Rows.Count == 0)
		{
			//取得件数0件の場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_019");
			this.grdRoot.Focus();
		}
	}

	//■■■ＤＢ関連処理■■■

	/// <summary>
	/// ピックアップ乗車地・ルートの検索対象データの取得
	/// </summary>
	private DataTable GetPickupAndRootData()
	{
		//戻り値
		DataTable returnValue = null;
		try
		{
			//DBパラメータ
			Hashtable paramInfoList = new Hashtable();

			//DAクラス作成、パラメータ設定、選択処理実施までを実装
			//DataAccessクラス生成
			PickupRouteLedger_DA dataAccess = new PickupRouteLedger_DA();

			//パラメータ設定
			string syuptDay = string.Empty;
			string jyochachiCd = string.Empty;
			string ttyakTimeFrom = string.Empty;
			string ttyakTimeTo = string.Empty;
			syuptDay = System.Convert.ToString((System.Convert.ToDateTime(this.dtmSyuptDay.Value)).ToString("yyyyMMdd"));
			jyochachiCd = System.Convert.ToString(this.ucoJyochachiCd.CodeText);
			if (!ReferenceEquals(this.dtmTtyakTimeFromTo.FromTimeValue24Int, null))
			{
				ttyakTimeFrom = System.Convert.ToString(this.dtmTtyakTimeFromTo.FromTimeValue24Int.ToString());
			}
			if (!ReferenceEquals(this.dtmTtyakTimeFromTo.ToTimeValue24Int, null))
			{
				ttyakTimeTo = System.Convert.ToString(this.dtmTtyakTimeFromTo.ToTimeValue24Int.ToString());
			}
			paramInfoList.Add("SyuptDay", syuptDay);
			paramInfoList.Add("JyochachiCd", jyochachiCd);
			paramInfoList.Add("TtyakTimeFrom", ttyakTimeFrom);
			paramInfoList.Add("TtyakTimeTo", ttyakTimeTo);

			if (this.rdoPickupJyosyaTi.Checked == true)
			{
				returnValue = dataAccess.accessPickupRouteInfor(PickupRouteLedger_DA.accessType.getPickupRouteInforByJyochachi, paramInfoList);
			}
			else
			{
				returnValue = dataAccess.accessPickupRouteInfor(PickupRouteLedger_DA.accessType.getPickupRouteInforByRoot, paramInfoList);
			}


			//検索結果が１件以上（表示できた）の場合は、入力条件を退避しておく
			if (returnValue.Rows.Count > 0)
			{
				_searchConditionCache = paramInfoList;
			}
		}
		catch (Exception)
		{
			throw;
		}

		return returnValue;
	}

	#region チェック処理(Private)
	/// <summary>
	/// 必須入力項目エラーチェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isExistHissuError()
	{
		bool returnValue = false;

		return System.Convert.ToBoolean(CommonUtil.checkHissuError(this.gbxCondition.Controls));

		return returnValue;
	}

	/// <summary>
	/// 範囲の大小チェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isExistHaniNoDaisoError()
	{
		bool returnValue = false;

		//到着時間From > 到着時間Toはエラー
		object with_1 = this.dtmTtyakTimeFromTo;
		if (with_1.FromTimeValue24Int IsNot null && with_1.ToTimeValue24Int IsNot null)
		{
			if (with_1.FromTimeValue24Int > with_1.ToTimeValue24Int)
			{
				with_1.ExistErrorForFromTime = true;
				with_1.ExistErrorForToTime = true;
				with_1.Focus();
				returnValue = true;
			}
		}

		return returnValue;
	}
	#endregion

	#endregion

	#region 実装用メソッド(画面毎に変更)

	#region 初期処理
	/// <summary>
	/// 初期処理(必須画面個別実装)
	/// </summary>
	public void setSeFirsttDisplayData()
	{
		//初期値設定
		//選択
		this.rdoPickupJyosyaTi.Checked = true;
		this.rdoRoot.Checked = false;
		this.lblLengthGrd.Text = "0件";
		this.grdPickup.Visible = true;
		this.grdRoot.Visible = false;
		this.grdPickup.DataSource = new DataTable();
		this.grdRoot.DataSource = new DataTable();

		//検索条件を表示状態のGroupAreaのレイアウトを保存
		this.SaveGrpLayout();
	}
	#endregion

	#region エンティティ操作系
	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent"></param>
	public void DisplayDataToEntity(ref object ent)
	{

	}
	#endregion

	#region チェック系
	/// <summary>
	/// 入力項目のチェック
	/// </summary>
	/// <returns></returns>
	public bool CheckSearch()
	{
		//必須項目のチェック
		if (this.isExistHissuError())
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			return false;
		}

		//範囲の大小チェック
		if (this.isExistHaniNoDaisoError())
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_048");
			return false;
		}

		return true;
	}
	#endregion

	#endregion
}