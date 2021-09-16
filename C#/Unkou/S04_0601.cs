using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.Master;
using Hatobus.ReservationManagementSystem.Yoyaku;
using System.ComponentModel;
using System.Drawing;


/// <summary>
/// S04_0601 予約一覧照会（ピックアップ）
/// </summary>
public class S04_0601 : PT21, iPT21
{
	public S04_0601()
	{
		UpdateEntityList = new List(Of EntityOperation(Of TYoyakuInfoPickupEntityEx));

	}

	#region 変数
	DataTable searchResultGridYoyakuInfo;
	DataTable searchResultGridYoyakuInfoOld;
	bool rdoPickupFlg = true;
	#endregion
	#region 定数
	//更新対象エンティティ
	private List UpdateEntityList; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.


	private int IndexRow = 0;

	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	public const int TopGbxCondition = 41;

	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	public const int MarginGbxCondition = 6;

	/// <summary>
	/// 条件GroupBox非表示時のGrouBoxAreaの高さ
	/// </summary>
	public const int HeightGbxAreasOnNotVisibleCondition = 450;
	#endregion
	#region 列挙

	/// <summary>
	/// グリッドカラム一覧(ホテル)
	/// </summary>
	private enum grdHotelColNum
	{
		CHK_SEL = 1,
		HOTEL_NAME_JYOSYA_TI,
		RK,
		PICKUP_HOTEL_CD
	}

	/// <summary>
	/// グリッドカラム一覧(ルート)
	/// </summary>
	private enum grdRootyColNum
	{
		CHK_SEL = 1,
		PICKUP_ROUTE_CD,
		HOTEL_NAME_JYOSYA_TI,
		RK,
		SYUPT_TIME
	}


	/// <summary>
	/// グリッドカラム一覧
	/// </summary>
	private enum grdYoyakuListColNum
	{
		CHK_SEL = 1,

		PICKUP_ROUTE_CD,
		NINZU_KEI_ROOT,
		RK,
		SYUPT_TIME,
		NINZU_KEI_HOTEL,
		ZUMI_FLG,
		YNO,
		colDetail,
		NAME,
		NINZU,
		CRS_CD,
		CRS_NAME,
		STATE,
		SEISAN_HOHO_NM,
		AGENT_NM,
		PICKUP_HOTEL_CD,
		YOYAKU_KBN,
		YOYAKU_NO
	}
	#endregion

	#region フィールド

	private Hashtable _searchConditionCache = new Hashtable(); //入力条件の退避

	/// <summary>
	/// 条件GroupBoxの高さ
	/// </summary>
	private int _heightGbxCondition;

	/// <summary>
	/// GroupBoxArea1の高さ
	/// </summary>
	private int _heightGbxArea1;

	/// <summary>
	/// GroupBoxArea2の高さ
	/// </summary>
	private int _heightGbxArea2;

	/// <summary>
	/// GroupBoxArea1のTop座標
	/// </summary>
	private int _topGbxArea1;

	/// <summary>
	/// GroupBoxArea2のTop座標
	/// </summary>
	private int _topGbxArea2;

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

	#region 検索
	/// <summary>
	/// 条件GroupBox表示制御ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnVisiblerCondition_Click(object sender, EventArgs e)
	{
		this.VisibleGbxCondition = !this.VisibleGbxCondition;

		int offSet = this.HeightGbxCondition + MarginGbxCondition;
		int harfOffset = System.Convert.ToInt32((double)offSet / 2);

		//GrpBoxArea1, 2の座標, サイズを表示/非表示に応じて変更
		if (this.VisibleGbxCondition)
		{
			//表示状態
			this.btnHyoji.Text = "非表示 <<";

			this.SetGrpLayout();
		}
		else
		{
			//非表示状態
			this.btnHyoji.Text = "表示 >>";
			this.gbxYoyakuInfo.Height = HeightGbxAreasOnNotVisibleCondition;

			this.gbxPickup.Top = TopGbxCondition;
			this.gbxYoyakuInfo.Top = TopGbxCondition + this.gbxPickup.Height + MarginGbxCondition;
		}
	}

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

	#region メソッド

	/// <summary>
	/// GroupBoxのレイアウト保存
	/// </summary>
	private void SaveGrpLayout()
	{
		this.gbxCondition.Height = this.gbxCondition.Height;
		this._heightGbxArea1 = System.Convert.ToInt32(this.gbxPickup.Height);
		this._heightGbxArea2 = System.Convert.ToInt32(this.gbxYoyakuInfo.Height);
		this._topGbxArea1 = System.Convert.ToInt32(this.gbxPickup.Top);
		this._topGbxArea2 = System.Convert.ToInt32(this.gbxYoyakuInfo.Top);
	}

	/// <summary>
	/// GroupBoxのレイアウト設定
	/// </summary>
	private void SetGrpLayout()
	{
		this.gbxCondition.Height = this.gbxCondition.Height;
		this.gbxPickup.Height = this._heightGbxArea1;
		this.gbxYoyakuInfo.Height = this._heightGbxArea2;
		this.gbxPickup.Top = this._topGbxArea1;
		this.gbxYoyakuInfo.Top = this._topGbxArea2;
	}

	#region DB関連
	//■■■ＤＢ関連処理■■■

	/// <summary>
	/// ピックアップ乗車地・ルートの検索対象データの取得
	/// </summary>
	private DataTable GetPickupAndRootData()
	{

		//戻り値
		DataTable returnValue = null;

		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		//DataAccessクラス生成
		PickupRouteLedger_DA dataAccess = new PickupRouteLedger_DA();

		//パラメータ設定
		string syuptDay = string.Empty;
		string jyochachiCd = string.Empty;
		string ttyakTimeFrom = string.Empty;
		string ttyakTimeTo = string.Empty;
		syuptDay = System.Convert.ToString((System.Convert.ToDateTime(this.dtmSyuptDay.Value)).ToString(System.Convert.ToString(getEnumValue(typeof(FixedCdKyushu.Date_FormatType), FixedCdKyushu.Date_FormatType.formatYYYYMMDD))));
		jyochachiCd = System.Convert.ToString(this.ucoJyochachi.CodeText);
		if (!ReferenceEquals(this.ucoTtyakTimeFromTo.FromTimeValue24Int, null))
		{
			ttyakTimeFrom = System.Convert.ToString(this.ucoTtyakTimeFromTo.FromTimeValue24Int.ToString());
		}
		if (!ReferenceEquals(this.ucoTtyakTimeFromTo.ToTimeValue24Int, null))
		{
			ttyakTimeTo = System.Convert.ToString(this.ucoTtyakTimeFromTo.ToTimeValue24Int.ToString());
		}
		paramInfoList.Add("SyuptDay", syuptDay);
		paramInfoList.Add("JyochachiCd", jyochachiCd);
		paramInfoList.Add("TtyakTimeFrom", ttyakTimeFrom);
		paramInfoList.Add("TtyakTimeTo", ttyakTimeTo);
		if (this.rdoPickup.Checked == true)
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


		return returnValue;
	}

	/// <summary>
	/// 予約情報の検索対象データの取得
	/// </summary>
	/// <param name="pDataRow">GridのDataBind(DataTable)の選択行</param>
	private DataTable GetYoyakuInforData(DataRow[] pDataRow)
	{
		//戻り値
		DataTable returnValue = null;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		//DataAccessクラス生成
		PickupRouteLedger_DA dataAccess = new PickupRouteLedger_DA();

		//パラメータ設定
		string jyochachiCd = string.Empty;
		string syuptDay = string.Empty;
		string houjinGaikakuKbn = string.Empty;
		string tyakTimeFrom = string.Empty;
		string tyakTimeTo = string.Empty;
		bool zumiFlg = false;
		bool cancelFlg = false;
		ArrayList pickupHotelCd = new ArrayList();
		ArrayList pickupRouteCd = new ArrayList();

		syuptDay = System.Convert.ToString(_searchConditionCache.Item("SyuptDay"));

		jyochachiCd = System.Convert.ToString(_searchConditionCache.Item("JyochachiCd"));

		if (this.chkCrsKind3Japanese.Checked == true && this.chkCrsKind3Gaikokugo.Checked == false)
		{
			houjinGaikakuKbn = System.Convert.ToString(HoujinGaikyakuKbnType.Houjin);
		}
		else if (this.chkCrsKind3Japanese.Checked == false && this.chkCrsKind3Gaikokugo.Checked == true)
		{
			houjinGaikakuKbn = System.Convert.ToString(HoujinGaikyakuKbnType.Gaikyaku);
		}


		tyakTimeFrom = System.Convert.ToString(this.ucoTtyakTimeFromTo.FromTimeValue24Int.ToString());
		tyakTimeTo = System.Convert.ToString(this.ucoTtyakTimeFromTo.ToTimeValue24Int.ToString());

		if (this.chkZumiFlg.Checked == true)
		{
			zumiFlg = true;
		}


		if (this.chkCancelFlg.Checked == true)
		{
			cancelFlg = true;
		}

		paramInfoList.Add("SyuptDay", syuptDay);
		paramInfoList.Add("JyochachiCd", jyochachiCd);
		paramInfoList.Add("HoujinGaikakuKbn", houjinGaikakuKbn);
		paramInfoList.Add("TtyakTimeFrom", tyakTimeFrom);
		paramInfoList.Add("TtyakTimeTo", tyakTimeTo);
		paramInfoList.Add("ZumiFlg", zumiFlg);
		paramInfoList.Add("CancelFlg", cancelFlg);
		if (rdoPickupFlg == true)
		{



			for (i = 0; i <= pDataRow.Count() - 1; i++)
			{
				pickupHotelCd.Add(pDataRow[i][System.Convert.ToInt32("PICKUP_HOTEL_CD")]);
			}
			paramInfoList.Add("PickupHotelCd", pickupHotelCd);
			return dataAccess.accessPickupRouteInfor(PickupRouteLedger_DA.accessType.getYoyakuJohoInforByJyochachi, paramInfoList);
		}
		else
		{
			for (i = 0; i <= pDataRow.Count() - 1; i++)
			{
				pickupRouteCd.Add(pDataRow[i][System.Convert.ToInt32("PICKUP_ROUTE_CD")]);
			}
			paramInfoList.Add("PickupRouteCd", pickupRouteCd);
			return dataAccess.accessPickupRouteInfor(PickupRouteLedger_DA.accessType.getYoyakuJohoInforByRoot, paramInfoList);
		}

		return returnValue;
	}

	/// <summary>
	/// データ更新処理の実施
	/// </summary>
	protected override int ExecuteUpdateTran()
	{
		//戻り値
		int returnValue = 0;

		List[] paramInfoList = new List(Of Hashtable);
		foreach (EntityOperation[] updateEntity in UpdateEntityList)
		{

			if (updateEntity.compare(0) == false)
			{
				//DBパラメータ
				Hashtable paramInfo = new Hashtable();

				//パラメータ作成(共通部)
				CommonLogic.setTableCommonInfo(ConstantCode.DbShoriKbn.Update, this.Name, (iEntity)(updateEntity.EntityData(0)));

				object with_1 = updateEntity.EntityData(0);
				if (updateEntity.EntityData(0).YoyakuKbn.Value != "")
				{

					if (System.Convert.ToBoolean(with_1.ZumiFlg.Value) == true)
					{
						//確認済み
						paramInfo.Add("ZumiFlg", "Y");
						//確認日
						paramInfo.Add("CheckDay", DateTime.Now.ToString(System.Convert.ToString(getEnumValue(typeof(FixedCdKyushu.Date_FormatType), FixedCdKyushu.Date_FormatType.formatYYYYMMDD))));
					}
					else
					{
						//確認済み
						paramInfo.Add("ZumiFlg", " ");
						//確認日
						paramInfo.Add("CheckDay", "0");
					}
					//予約区分
					paramInfo.Add("YoyakuKbn", with_1.YoyakuKbn.Value);
					//予約NO
					paramInfo.Add("YoyakuNo", with_1.YoyakuNo.Value);
					//ピックアップルートコード
					paramInfo.Add("PickupRouteCd", with_1.PickupRouteCd.Value);
					//ピックアップホテルコード
					paramInfo.Add("PickupHotelCd", with_1.PickupHotelCd.Value);
					paramInfo.Add("SystemUpdateDay", with_1.SystemUpdateDay.Value);
					paramInfo.Add("SystemUpdatePgmid", with_1.SystemUpdatePgmid.Value);
					paramInfo.Add("SystemUpdatePersonCd", with_1.SystemUpdatePersonCd.Value);

					paramInfoList.Add(paramInfo);
				}

			}
		}

		//DataAccessクラス生成
		YoyakuInfoPickup_DA dataAccess = new YoyakuInfoPickup_DA();
		//Updateの実施
		returnValue += System.Convert.ToInt32(dataAccess.executeYoyakuInfoPickup(YoyakuInfoPickup_DA.accessType.executeUpdateYoyakuInfoPickup, paramInfoList));


		return returnValue;
	}
	#endregion

	#region Datastudio 呼び出し
	/// <summary>
	/// 取得したデータを元に、帳票を印刷する。
	/// </summary>
	/// <remarks></remarks>
	private void ShowPickupListDS()
	{
		try
		{
			string PostData = string.Empty;
			List[] dataLst = new List(Of string);

			PostData += "base_select0=P04_0601;SYUPT_DAY&base_opecomp0=>=&base_value0=" + System.Convert.ToString(_searchConditionCache.Item("SyuptDay"));

			for (index = 1; index <= searchResultGridYoyakuInfo.Rows.Count; index++)
			{
				//対象行の選択ボタンがチェックされている場合、対象行のピックアップルートコード、予約番号を設定
				if (this.grdYoyakuInfo.GetCellCheck(index, grdYoyakuListColNum.CHK_SEL) == C1.Win.C1FlexGrid.CheckEnum.Checked)
				{
					//dataLstPRC.Add(CType(grdYoyakuInfo.Item(index, grdYoyakuListColNum.PICKUP_ROUTE_CD), String))
					if (!Information.IsDBNull(grdYoyakuInfo.Item(index, grdYoyakuListColNum.YNO)))
					{
						dataLst.Add(System.Convert.ToString(grdYoyakuInfo.Item(index, grdYoyakuListColNum.PICKUP_ROUTE_CD)).Trim().PadRight(6, '@') +;
						System.Convert.ToString(grdYoyakuInfo.Item(index, grdYoyakuListColNum.PICKUP_HOTEL_CD)).Trim().PadRight(6, '@') +;
						System.Convert.ToString(grdYoyakuInfo.Item(index, grdYoyakuListColNum.YNO)).Replace(",", "").Trim());
		}
				}
}

if (dataLst.Count() > 0)
{
	PostData += "&base_opelogic1=AND&base_select1=P04_0601;CONDITIONS&base_opecomp1=in($VALS$)&base_value1=" + string.Join(" ", dataLst);
}
else
{
	CommonProcess.createFactoryMsg().messageDisp("E03_028", "出力対象が");
	return;
}

ReadAppConfig reaAppConfig = new ReadAppConfig();
CommonProcess.DataStudioId = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioId);
CommonProcess.DataStudioPassword = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioPassword);
BOCommon.showDataStudio(this.Name, BOCommon.SystemSetCd.dsid_PickupList, PostData);

//ログメッセージ
string logmsg = string.Empty;
logmsg += "USERID=" + CommonProcess.DataStudioId;
logmsg += "&PASSWORD=" + CommonProcess.DataStudioPassword;
logmsg += logmsg + PostData;
createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.DSCall, this.setTitle, logmsg);
		}
		catch (Exception ex)
{
	createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.DSCall, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
	throw;
}
	}
#endregion
	
#endregion
	
	
#region ピックアップ乗車地・ルート
	
	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	private void setPickupAndRoot()
{

	if (this.rdoPickup.Checked)
	{

		this.grdPickup.Visible = true;
		this.grdRoot.Visible = false;
		rdoPickupFlg = true;

		this.SetPickupInfo();
	}
	else
	{

		this.grdPickup.Visible = false;
		this.grdRoot.Visible = true;
		rdoPickupFlg = false;

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
		//検索ボタンにフォーカス
		this.btnSearch.Focus();
		//予約情報表示ボタン非活性
		btnYoyakuInfo.Enabled = false;
	}
	else
	{
		//該当データがある
		this.grdPickup.Focus();
		//予約情報表示ボタン活性
		btnYoyakuInfo.Enabled = true;
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
		//検索ボタンにフォーカス
		this.btnSearch.Focus();
		//予約情報表示ボタン非活性
		btnYoyakuInfo.Enabled = false;
	}
	else
	{
		//該当データがある
		this.grdRoot.Focus();

		mergeGrdListRoot();

		//予約情報表示ボタン活性
		btnYoyakuInfo.Enabled = true;
	}

}

/// <summary>
/// 一覧マージ設定
/// </summary>
private void mergeGrdListRoot()
{

	C1.Win.C1FlexGrid.CellRange cr = null;

	int topRow = 0;
	int bottomRow = 0;

	// マージ解除
	grdRoot.MergedRanges.Clear();

	topRow = 1;
	bottomRow = 1;

	for (int row1 = 1; row1 <= grdRoot.Rows.Count - 1; row1++)
	{

		// 最終行
		if (row1.Equals(grdRoot.Rows.Count - 1))
		{
			cr = grdRoot.GetCellRange(topRow, grdRootyColNum.PICKUP_ROUTE_CD, row1, grdRootyColNum.PICKUP_ROUTE_CD);
			grdRoot.MergedRanges.Add(cr);

			//選択列
			cr = grdRoot.GetCellRange(topRow, grdRootyColNum.CHK_SEL, row1, grdRootyColNum.CHK_SEL);
			grdRoot.MergedRanges.Add(cr);
			break;
		}

		// マージチェック（次行データと同じか）
		if (checkRowdataRoot(row1, (int)grdRootyColNum.PICKUP_ROUTE_CD) == false)
		{

			// 次行データと異なるため、マージする
			cr = grdRoot.GetCellRange(topRow, grdRootyColNum.PICKUP_ROUTE_CD, row1, grdRootyColNum.PICKUP_ROUTE_CD);
			grdRoot.MergedRanges.Add(cr);

			//選択列
			cr = grdRoot.GetCellRange(topRow, grdRootyColNum.CHK_SEL, row1, grdRootyColNum.CHK_SEL);
			grdRoot.MergedRanges.Add(cr);

			// 次行の開始行設定
			topRow = row1 + 1;
			bottomRow = row1 + 1;

		}
		else
		{
			bottomRow = row1;
		}


	}

}

/// <summary>
/// マージチェック（次行データと同じか）
/// </summary>
/// <param name="nowRow"></param>
/// <param name="nowCol"></param>
/// <returns></returns>
private bool checkRowdataRoot(int nowRow, int nowCol)
{

	// 2列目から現在列まで
	for (int col1 = grdRootyColNum.PICKUP_ROUTE_CD; col1 <= nowCol; col1++)
	{
		// 次行データと同じか
		if (ReferenceEquals(grdRoot(nowRow, col1), null) == false && ReferenceEquals(grdRoot(nowRow + 1, col1), null) == false)
		{
			if (!(grdRoot(nowRow, col1).ToString().TrimEnd.Equals(grdRoot(nowRow + 1, col1).ToString().TrimEnd)))
			{
				// 異なる
				return false;
			}
		}
	}

	return true;
}

/// <summary>
/// 予約情報表示ボタンクリック時イベント
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void btnYoyakuInfo_Click(object sender, EventArgs e)
{

	getSelectedRowData();

}
#endregion

#region 予約情報

/// <summary>
/// 詳細ボタン押下イベント
/// </summary>
/// <param name="sender">イベント送信元</param>
/// <param name="e">イベントデータ</param>
private void btnGridRow_Click(object sender, EventArgs e)
{

	if (Information.IsDBNull(grdYoyakuInfo.Item(grdYoyakuInfo.Row, grdYoyakuListColNum.YOYAKU_KBN)))
	{
		return;
	}

	string yoyakuKbn = System.Convert.ToString(grdYoyakuInfo.Item(grdYoyakuInfo.Row, grdYoyakuListColNum.YOYAKU_KBN));
	int yoyakuNo = System.Convert.ToInt32(grdYoyakuInfo.Item(grdYoyakuInfo.Row, grdYoyakuListColNum.YOYAKU_NO));
	S02_0103ParamData s02_0103ParamData = new S02_0103ParamData();

	//予約番号
	s02_0103ParamData.YoyakuKbn = yoyakuKbn;
	s02_0103ParamData.YoyakuNo = yoyakuNo;
	//モード
	s02_0103ParamData.ScreenMode = CommonRegistYoyaku.ScreenModeReference; //参照

	//予約登録画面に遷移
	using (S02_0103 form = new S02_0103())
	{
		form.ParamData = s02_0103ParamData;
		form.ShowDialog(this);
	}


}

/// <summary>
/// 予約情報グリッドセルチェック時イベント
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void grdYoyakuInfo_CellChecked(object sender, RowColEventArgs e)
{
	//グリッド範囲外は処理しない
	if (this.grdYoyakuInfo.RowSel < 0 || this.grdYoyakuInfo.ColSel < 0)
	{
		return;
	}
	else
	{
		if (this.grdYoyakuInfo.ColSel == grdYoyakuListColNum.CHK_SEL)
		{
			//出力対象チェック
			if (this.grdYoyakuInfo.GetCellCheck(this.grdYoyakuInfo.RowSel, this.grdYoyakuInfo.ColSel) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
			{
				this.grdYoyakuInfo[this.grdYoyakuInfo.RowSel, this.grdYoyakuInfo.ColSel] = "0";
			}
			else
			{
				this.grdYoyakuInfo[this.grdYoyakuInfo.RowSel, this.grdYoyakuInfo.ColSel] = "1";
			}
		}
		else if (this.grdYoyakuInfo.ColSel == grdYoyakuListColNum.ZUMI_FLG)
		{
			//連携済みチェック
			if (this.grdYoyakuInfo.GetCellCheck(this.grdYoyakuInfo.RowSel, this.grdYoyakuInfo.ColSel) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
			{
				this.grdYoyakuInfo[this.grdYoyakuInfo.RowSel, this.grdYoyakuInfo.ColSel] = "0";
			}
			else
			{
				this.grdYoyakuInfo[this.grdYoyakuInfo.RowSel, this.grdYoyakuInfo.ColSel] = "1";
			}
		}
	}
}

/// <summary>
/// 予約情報グリッドのヘッダの設定
/// </summary>
private void SetYoyakuHeader()
{

	// 行の高さ設定
	this.grdYoyakuInfo.Rows(0).Height = 37;
	// 人数計(ルート)
	CellRange range1 = this.grdYoyakuInfo.GetCellRange(0, grdYoyakuListColNum.NINZU_KEI_ROOT);
	range1.Data = "人数計" + "\r\n" + "(ルート)";
	// 人数計(ホテル)
	CellRange range2 = this.grdYoyakuInfo.GetCellRange(0, grdYoyakuListColNum.NINZU_KEI_HOTEL);
	range2.Data = "人数計" + "\r\n" + "(ホテル)";
	// 人数計(ホテル)
	CellRange range3 = this.grdYoyakuInfo.GetCellRange(0, grdYoyakuListColNum.CRS_CD);
	range3.Data = "コース" + "\r\n" + "コード";

}

/// <summary>
/// 選択行のデータを取得
/// </summary>
/// <remarks></remarks>
protected override void getSelectedRowData()
{
	//選択された行データ
	DataRow[] selectData = null;
	//問合せ文字列
	string whereString = string.Empty;

	//予約情報グリッドのヘッダの設定
	SetYoyakuHeader();

	//引数の設定
	if (rdoPickupFlg == true)
	{
		whereString = "CHK_SEL=1";
		//問合せ対象データ取得
		selectData = ((DataTable)this.grdPickup.DataSource).Select(whereString);
	}
	else
	{
		whereString = "CHK_SEL=1";
		//問合せ対象データ取得
		selectData = ((DataTable)this.grdRoot.DataSource).Select(whereString);
	}

	if (selectData.Length > 0)
	{
		this.SetYoyakuInfo(selectData);
	}
	else
	{
		//Entityの初期化
		initEntityData();
		//予約情報エリアの項目初期化
		initDetailAreaItems();
	}

}

/// <summary>
/// グリッドデータの取得とグリッド表示
/// </summary>
/// <param name="pDataRow">GridのDataBind(DataTable)の選択行</param>
private void SetYoyakuInfo(DataRow[] pDataRow)
{

	//検索処理
	searchResultGridYoyakuInfo = this.GetYoyakuInforData(pDataRow);
	//差分比較用
	searchResultGridYoyakuInfoOld = searchResultGridYoyakuInfo.Copy;

	if (searchResultGridYoyakuInfo.Rows.Count == 0)
	{
		UpdateEntityList.Clear();
		this.grdYoyakuInfo.DataSource = searchResultGridYoyakuInfo;


		//更新ボタン非活性
		F11Key_Enabled = false;
		//出力ボタン非活性
		F7Key_Enabled = false;
	}

	if (searchResultGridYoyakuInfo.Rows.Count > 0) //該当データがある場合
	{

		//取得データをEntityの前回値にセット
		UpdateEntityList.Clear();
		for (index = 0; index <= searchResultGridYoyakuInfo.Rows.Count - 1; index++)
		{
			OldDataToEntity(searchResultGridYoyakuInfo.Rows(index));
		}

		//取得結果をグリッドへ設定
		this.grdYoyakuInfo.DataSource = searchResultGridYoyakuInfo;

		// 一覧マージ設定
		mergeGrdList();

		DataTable dataTable = (DataTable)this.grdYoyakuInfo.DataSource;
		//Dim ninzuKeiRoot As Double = 0
		dataTable.Columns("NINZU_KEI_HOTEL").MaxLength = 3;
		dataTable.Columns("NINZU_KEI_ROOT").MaxLength = 3;
		for (index = 0; index <= searchResultGridYoyakuInfo.Rows.Count - 1; index++)
		{
			//ホテル計
			CellRange cellRange = this.grdYoyakuInfo.GetMergedRange(index + 1, grdYoyakuListColNum.RK);
			double ninzuKeiHotel = System.Convert.ToDouble(this.grdYoyakuInfo.Aggregate(AggregateEnum.Sum, cellRange.TopRow, grdYoyakuListColNum.NINZU, cellRange.BottomRow, grdYoyakuListColNum.NINZU));
			dataTable.Rows(index).Item["NINZU_KEI_HOTEL"] = ninzuKeiHotel;

			//ルート計
			cellRange cellRangeRoot = this.grdYoyakuInfo.GetMergedRange(index + 1, grdYoyakuListColNum.PICKUP_ROUTE_CD);
			double ninzuKeiRoot = System.Convert.ToDouble(this.grdYoyakuInfo.Aggregate(AggregateEnum.Sum, cellRangeRoot.TopRow, grdYoyakuListColNum.NINZU, cellRangeRoot.BottomRow, grdYoyakuListColNum.NINZU));
			dataTable.Rows(index).Item["NINZU_KEI_ROOT"] = ninzuKeiRoot;
		}
		//予約情報グリッド全体の人数計を表示
		this.txtNinzuSum.Text = System.Convert.ToString(this.grdYoyakuInfo.Aggregate(AggregateEnum.Sum, 0, grdYoyakuListColNum.NINZU, searchResultGridYoyakuInfo.Rows.Count, grdYoyakuListColNum.NINZU));
		//検索結果(ピックアップ）グリッド全体（行には当てない）にフォーカス
		this.grdYoyakuInfo.RowSel = 1;

		// 静止列 設定
		this.grdYoyakuInfo.Cols.Frozen = grdYoyakuListColNum.NINZU_KEI_HOTEL;

		//更新ボタン活性
		F11Key_Enabled = true;
		//出力ボタン活性
		F7Key_Enabled = true;
	}
	else
	{
		//０件の場合
		this.grdYoyakuInfo.Focus();
	}
}

/// <summary>
/// 一覧マージ設定
/// </summary>
private void mergeGrdList()
{

	C1.Win.C1FlexGrid.CellRange cr = null;

	int topRow = 0;
	int bottomRow = 0;

	// マージ解除
	grdYoyakuInfo.MergedRanges.Clear();

	for (int col1 = grdYoyakuListColNum.PICKUP_ROUTE_CD; col1 <= grdYoyakuListColNum.NINZU_KEI_HOTEL; col1++)
	{

		topRow = 1;
		bottomRow = 1;

		for (int row1 = 1; row1 <= grdYoyakuInfo.Rows.Count - 1; row1++)
		{

			// 最終行
			if (row1.Equals(grdYoyakuInfo.Rows.Count - 1))
			{
				cr = grdYoyakuInfo.GetCellRange(topRow, col1, row1, col1);
				grdYoyakuInfo.MergedRanges.Add(cr);

				if (col1 == (int)grdYoyakuListColNum.PICKUP_ROUTE_CD)
				{
					// 次行データと異なるため、マージする
					cr = grdYoyakuInfo.GetCellRange(topRow, grdYoyakuListColNum.CHK_SEL, row1, grdYoyakuListColNum.CHK_SEL);
					grdYoyakuInfo.MergedRanges.Add(cr);
				}

				break;
			}

			// マージチェック（次行データと同じか）
			if (checkRowdata(row1, col1) == false)
			{

				// 次行データと異なるため、マージする
				cr = grdYoyakuInfo.GetCellRange(topRow, col1, row1, col1);
				grdYoyakuInfo.MergedRanges.Add(cr);

				if (col1 == (int)grdYoyakuListColNum.PICKUP_ROUTE_CD)
				{
					// 次行データと異なるため、マージする
					cr = grdYoyakuInfo.GetCellRange(topRow, grdYoyakuListColNum.CHK_SEL, row1, grdYoyakuListColNum.CHK_SEL);
					grdYoyakuInfo.MergedRanges.Add(cr);
				}

				// 次行の開始行設定
				topRow = row1 + 1;
				bottomRow = row1 + 1;

			}
			else
			{
				bottomRow = row1;
			}

		}

	}

}

/// <summary>
/// マージチェック（次行データと同じか）
/// </summary>
/// <param name="nowRow"></param>
/// <param name="nowCol"></param>
/// <returns></returns>
private bool checkRowdata(int nowRow, int nowCol)
{

	// 2列目から現在列まで
	for (int col1 = grdYoyakuListColNum.PICKUP_ROUTE_CD; col1 <= nowCol; col1++)
	{
		// 次行データと同じか
		if (ReferenceEquals(grdYoyakuInfo(nowRow, col1), null) == false && ReferenceEquals(grdYoyakuInfo(nowRow + 1, col1), null) == false)
		{
			if (!(grdYoyakuInfo(nowRow, col1).ToString().TrimEnd.Equals(grdYoyakuInfo(nowRow + 1, col1).ToString().TrimEnd)))
			{
				// 異なる
				return false;
			}
		}
	}

	return true;
}
#endregion


#region FormBaseから
/// <summary>
/// CLEARボタン押下時の独自処理
/// </summary>
protected override void btnCLEAR_ClickOrgProc()
{

	initSearchAreaItems();
	//検索項目エリア
	this.dtmSyuptDay.Value = CommonDateUtil.getSystemTime().AddDays(1);
	this.ucoJyochachi.CodeText = string.Empty;
	this.ucoJyochachi.ValueText = string.Empty;

	//ピックアップ乗車地・ルートエリア
	this.chkCrsKind3Japanese.Checked = false;
	this.chkCrsKind3Gaikokugo.Checked = false;
	this.chkZumiFlg.Checked = true;
	this.chkCancelFlg.Checked = false;
	this.rdoPickup.Checked = true;
	this.rdoRoot.Checked = false;

	this.grdPickup.DataSource = new DataTable();
	this.grdRoot.DataSource = new DataTable();

	//予約情報表示ボタン非活性
	btnYoyakuInfo.Enabled = false;

	//予約情報エリアの項目初期化
	initDetailAreaItems();


}

/// <summary>
/// F7()ボタン押下時の独自処理
/// </summary>
protected override void btnF7_ClickOrgProc()
{
	//ピックアップ用リストＤＳ呼び出し
	this.ShowPickupListDS();
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
		//予約情報エリアの項目初期化
		initDetailAreaItems();
	}

}

/// <summary>
/// F11(更新)ボタン押下時
/// </summary>
protected override void btnF11_ClickOrgProc()
{

	object ret = DbOperator((ConstantCode.DbShoriKbn)DbShoriKbn.Update);
	if (ret > 0)
	{
		CommonProcess.createFactoryMsg().messageDisp("I90_002", "更新");
		getSelectedRowData();

		//Else
	}
	else if (ret == base.RET_NONDATAUPDATED)
	{
		CommonProcess.createFactoryMsg().messageDisp("E90_068");
	}
}

#region 画面->エンティティ
/// <summary>
/// 更新対象項目をエンティティにセット
/// </summary>
protected override void setEntityDataValue()
{
	foreach (EntityOperation[] updateEntity in UpdateEntityList)
	{

		//エンティティの初期化
		updateEntity.clear(0, clearType.value);
		updateEntity.clear(0, clearType.errorInfo);

		iEntity temp_ent = (iEntity)(updateEntity.EntityData(0));
		DisplayDataToEntity(ref temp_ent);
		IndexRow++;
	}
	IndexRow = 0;
}
#endregion

#endregion

#region PT21オーバーライド(基本的には変えない)

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
/// 検索結果部の項目初期化
/// </summary>
protected override void initDetailAreaItems()
{
	//検索条件を表示状態のGroupAreaのレイアウトを保存
	this.SaveGrpLayout();

	this.SetYoyakuHeader();

	this.grdYoyakuInfo.DataSource = new DataTable();
	this.txtNinzuSum.Text = "";

	//F11ボタンを非活性にする
	F11Key_Enabled = false;

	//F7ボタンを非活性にする
	F7Key_Enabled = false;
}

/// <summary>
/// フッタボタンの設定
/// </summary>
protected override void initFooterButtonControl()
{

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
	this.F11Key_Visible = true;
	this.F12Key_Visible = false;

	this.F2Key_Text = "F2:戻る";
	this.F7Key_Text = "F7:出力";
	this.F11Key_Text = "F11:更新";


	this.F11Key_AllowAuth = FixedCd.AuthLevel.update;

	this.F11Key_Enabled = false;
	this.F7Key_Enabled = false;

}
#endregion

#region エンティティ初期化
/// <summary>
/// エンティティ初期化
/// </summary>
protected override void initEntityData()
{
	foreach (EntityOperation[] updateEntity in UpdateEntityList)
	{
		updateEntity.clear();
	}

	IndexRow = 0;
}
#endregion

#region 変更確認
/// <summary>
/// 差分チェック
/// </summary>
protected override bool checkDifference()
{

	//更新ボタン非活性の場合はチェック不要
	if (F11Key_Enabled == false)
	{
		return false;
	}
	else
	{
		for (i = 0; i <= searchResultGridYoyakuInfo.Rows.Count - 1; i++)
		{

			if (Information.IsDBNull(searchResultGridYoyakuInfoOld.Rows(i)[grdYoyakuListColNum.ZUMI_FLG.ToString()]) == false &&)
			{
				System.Convert.ToBoolean(searchResultGridYoyakuInfoOld.Rows(i)[grdYoyakuListColNum.ZUMI_FLG.ToString()]) != System.Convert.ToBoolean(searchResultGridYoyakuInfo.Rows(i)[grdYoyakuListColNum.ZUMI_FLG.ToString()]);

				return false;
			}


		}
		return true;
	}
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

	this.ActiveControl = this.dtmSyuptDay;

	//セル結合可
	this.grdYoyakuInfo.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;
	this.grdRoot.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;
}
#endregion

#region チェック系
/// <summary>
/// 検索入力項目チェック
/// </summary>
protected override bool checkSearchItems()
{

	//背景色初期化
	base.clearExistErrorProperty(this.gbxCondition.Controls);

	return this.CheckSearch();
}

/// <summary>
/// 予約情報の使用中チェック
/// </summary>
protected override bool checkUpdateItems()
{
	//予約情報の使用中チェック
	return this.CheckUpdate();
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
	//出発日
	this.dtmSyuptDay.Value = CommonDateUtil.getSystemTime().AddDays(1);
	//選択
	this.rdoPickup.Checked = true;
	this.rdoRoot.Checked = false;

	this.chkZumiFlg.Checked = true;
	this.chkCancelFlg.Checked = false;

	this.grdPickup.DataSource = new DataTable();
	this.grdRoot.DataSource = new DataTable();
	this.grdYoyakuInfo.DataSource = new DataTable();

	//予約情報表示ボタン非活性
	btnYoyakuInfo.Enabled = false;
}

#endregion

#region エンティティ操作系
/// <summary>
/// 画面からエンティティに設定する処理(必須画面個別実装)
/// </summary>
/// <param name="ent">エンティティ</param>
public void DisplayDataToEntity(ref iEntity ent)
{
	try
	{
		if (typeof(TYoyakuInfoPickupEntityEx).Equals(ent.GetType()))
		{
			object dtYoyaku = (DataTable)this.grdYoyakuInfo.DataSource;
			TYoyakuInfoPickupEntityEx with_1 = ((TYoyakuInfoPickupEntityEx)ent);
			if (dtYoyaku.Rows(IndexRow).Item(with_1.YoyakuKbn.PhysicsName).ToString() != "")
			{
				with_1.ZumiFlg.Value = dtYoyaku.Rows(IndexRow).Item(with_1.ZumiFlg.PhysicsName).ToString();
				with_1.YoyakuKbn.Value = dtYoyaku.Rows(IndexRow).Item(with_1.YoyakuKbn.PhysicsName).ToString();
				with_1.YoyakuNo.Value = (Integer?)(dtYoyaku.Rows(IndexRow).Item(with_1.YoyakuNo.PhysicsName).ToString());
				with_1.PickupRouteCd.Value = dtYoyaku.Rows(IndexRow).Item(with_1.PickupRouteCd.PhysicsName).ToString();
				with_1.PickupHotelCd.Value = dtYoyaku.Rows(IndexRow).Item(with_1.PickupHotelCd.PhysicsName).ToString();
			}

		}
	}
	catch (Exception)
	{
		throw;
	}
}

/// <summary>
/// エンティティから画面に設定する処理(必須画面個別実装)
/// </summary>
/// <param name="ent">エンティティ</param>
public void EntityDataToDisplay(ref iEntity ent)
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
	//Grid上の選択データ(pDataRow)をエンティティに設定する処理を実装
	EntityOperation updateEntity = new EntityOperation(Of TYoyakuInfoPickupEntityEx);
	//エンティティの初期化
	updateEntity.clear();

	//取得データが設定されている
	object with_2 = updateEntity.EntityData(0);
	if (string.IsNullOrEmpty(System.Convert.ToString(pDataRow.Item(with_2.ZumiFlg.PhysicsName)?.ToString())) == false)
	{
		with_2.ZumiFlg.ZenkaiValue = pDataRow.Item(with_2.ZumiFlg.PhysicsName)?.ToString();
	}
	if (string.IsNullOrEmpty(System.Convert.ToString(pDataRow.Item(with_2.YoyakuKbn.PhysicsName)?.ToString())) == false)
	{
		with_2.YoyakuKbn.ZenkaiValue = pDataRow.Item(with_2.YoyakuKbn.PhysicsName)?.ToString();
	}
	if (string.IsNullOrEmpty(System.Convert.ToString(pDataRow.Item(with_2.YoyakuNo.PhysicsName)?.ToString())) == false)
	{
		with_2.YoyakuNo.ZenkaiValue = (Integer?)(pDataRow.Item(with_2.YoyakuNo.PhysicsName));
	}
	if (string.IsNullOrEmpty(System.Convert.ToString(pDataRow.Item(with_2.PickupRouteCd.PhysicsName)?.ToString())) == false)
	{
		with_2.PickupRouteCd.ZenkaiValue = pDataRow.Item(with_2.PickupRouteCd.PhysicsName)?.ToString();
	}
	if (string.IsNullOrEmpty(System.Convert.ToString(pDataRow.Item(with_2.PickupHotelCd.PhysicsName)?.ToString())) == false)
	{
		with_2.PickupHotelCd.ZenkaiValue = pDataRow.Item(with_2.PickupHotelCd.PhysicsName)?.ToString();
	}

	UpdateEntityList.Add(updateEntity);
}
#endregion

#region チェック系
/// <summary>
/// 検索処理前のチェック処理(必須画面個別実装)
/// </summary>
/// <returns>True:エラー無 False:エラー有</returns>
public bool CheckSearch()
{
	//検索処理前のチェック処理を実装(メッセージは画面側でしてください)
	//必須項目のチェック
	if (this.isExistHissuError() == true)
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
	foreach (EntityOperation[] updateEntity in UpdateEntityList)
	{
		if (updateEntity.compare(0) == false)
		{
			object with_1 = updateEntity.EntityData(0);
			if (with_1.YoyakuKbn.Value != "")
			{
				//USING_FLGを取得
				YoyakuInfoBasic_DA flgrelation = new YoyakuInfoBasic_DA();
				//SQL実行
				DataTable check = flgrelation.getUsingFlg(with_1.YoyakuKbn.Value, System.Convert.ToString(with_1.YoyakuNo.Value));
				//Nullチェック
				string usingFlg = System.Convert.ToString(TryCast(check.Rows(0).Item(0), string));
				if (ReferenceEquals(usingFlg, null))
				{
					usingFlg = "";
				}

				if (usingFlg.Equals(FixedCd.UsingFlg.Use))
				{
					CommonProcess.createFactoryMsg().messageDisp("E90_040");
					return false;
				}
			}
		}
	}
	return true;
}

#endregion

#region チェック処理(Private)
/// <summary>
/// 必須入力項目エラーチェック
/// </summary>
/// <returns>True:エラー無 False:エラー有</returns>
/// <remarks></remarks>
public bool isExistHissuError()
{
	bool returnValue = false;

	returnValue = System.Convert.ToBoolean(CommonUtil.checkHissuError(this.gbxCondition.Controls));

	if (this.rdoPickup.Checked == false && this.rdoRoot.Checked == false)
	{
		this.rdoPickup.ExistError = true;
		this.rdoRoot.ExistError = true;
		this.rdoPickup.Focus();

		return true;
	}

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
	object with_1 = this.ucoTtyakTimeFromTo;
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
	
#region フッタ
	
	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
{
	this.Close();
}


/// <summary>
/// 画面クロージングイベント
/// </summary>
protected override bool closingScreen()
{


	if (checkDifference() == true)
	{
		//差分ありの場合、確認メッセージ表示(FormBase)
		return false;
	}
	else
	{
		return true;
	}

}
	
#endregion
	
	
	
}