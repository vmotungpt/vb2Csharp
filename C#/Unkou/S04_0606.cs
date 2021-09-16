using Hatobus.ReservationManagementSystem.Master;
using System.ComponentModel;



/// <summary>
/// ピックアップ台帳修正（予約停止）
/// </summary>
public class S04_0606 : PT71, iPT71
{
	public S04_0606()
	{
		EntityOperation _updateDetailEntity = new EntityOperation(TPickupRouteLedgerEntity);

	}

	#region  定数／変数宣言

	//更新対象エンティティ
	//TODO:Ofのあとのエンティティクラスとキー配列を変更(変更必須)
	private EntityOperation _updateDetailEntity; 
	private string[] _entityKeys = new string[] { "PICKUP_ROUTE_CD", "START_DAY" };

	// 選択時の内容を退避するデータテーブル
	private DataTable _detailDataTable;

	//更新時のメッセージ
	private string _updateMsg = "PUルートコード";

	private enum grdListColNum
	{
		PICKUP_ROUTE_CD = 1,
		START_DAY_DISP,
		START_DAY
	}

	private enum grdYoyakuteisiColNum
	{
		PICKUP_ROUTE_CD = 1,
		START_DAY,
		SYUPT_DAY_DISP,
		SYUPT_DAY,
		YOBI,
		YOYAKU_STOP_FLG,
		DAISU,
		NINZU,
		CAPA,
		EMGCAPA,
		YOYAKU_STOP_FLG_HIDDEN,
		USING_FLG
	}

	#endregion

	#region イベント
	//画面共通的なイベントはパターンに実装し、各画面はAddhandlerのみの方がすっきりする(※新規ボタンなど参照)
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
		//データ件数を表示(ヘッダー行分マイナス1)
		base.displayMainGridCount();
	}

	#endregion

	#region フォーム
	private void PT71_KeyDown(object sender, KeyEventArgs e)
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

	#region    F11
	/// <summary>
	/// F11()ボタン押下時の独自処理
	/// </summary>
	protected override void btnF11_ClickOrgProc()
	{

		// コース台帳 使用中フラグクリアが必要なためイベントを個別に実装

		if (this.grdYoyakuStop.Rows.Count <= this.grdYoyakuStop.Rows.Fixed)
		{
			// 該当データが存在しません。
			CommonProcess.createFactoryMsg().messageDisp("E90_019", UpdateMsgParam);
			return;
		}

		// コース台帳 使用中フラグ更新("" → "Y")
		//   上記は、[CheckUpdate] 内で行なう。ここで実装すると使用中チェックでエラーになる (DbOperator 内でチェック処理が走るため)

		int res = System.Convert.ToInt32(DbOperator(DbShoriKbn.Update));
		if (res.Equals(RET_NONEXEC))
		{
			return;
		}

		// 更新処理終了後、使用中フラグ をクリアする
		// コース台帳 使用中フラグ更新("Y" → "")
		setPickupRouteLedgerUsingFlg(System.Convert.ToString(UsingFlg.Unused));

		if (res > 0)
		{
			CommonProcess.createFactoryMsg().messageDisp("I90_002", "更新");
			//データの再表示
			btnF8_ClickOrgProc();
		}
		else
		{
			if (res == base.RET_CANCEL)
			{
				//処理確認でキャンセルの場合
				return;
			}
			else if (res == RET_NONEXEC)
			{
				//更新対象無し以外の場合
				return;
			}
			else if (res == RET_NONDATAUPDATED)
			{
				//更新対象無し
				CommonProcess.createFactoryMsg().messageDisp("E90_014", UpdateMsgParam);
				return;
			}
		}

	}
	#endregion

	#endregion

	#region PT71オーバーライド(基本的には変えない)

	#region 初期化処理
	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{
		CommonUtil.Control_Init(this.gbxCondition.Controls);
		//Me.Control_Init(Me.gbxCondition.Controls)
		//背景色初期化はここ
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		this.txtPickupRootCdSearch.Format = "A9";
		this.txtHotelName_JyochachiSearch.Format = "";

		///' [出発時間FromTo]テキストプロパティ初期設定 → デザインで設定済のためコメント (必要時復活)
		//Call _comUnkou.initTimeExProperty(Me.timTtyakTimeFromTo.FromTime24Ex)
		//Call _comUnkou.initTimeExProperty(Me.timTtyakTimeFromTo.ToTime24Ex)

		//Me.timTtyakTimeFromTo.FromTimeValue24 = Nothing  ' 到着時間From
		//Me.timTtyakTimeFromTo.ToTimeValue24 = Nothing    ' 到着時間To

	}

	/// <summary>
	/// 詳細エリアの項目初期化
	/// </summary>
	protected override void initDetailAreaItems()
	{

		//ピックアップルート詳細
		//行選択モード
		//Me.grdList.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row
		this.grdList.FocusRect = C1.Win.C1FlexGrid.FocusRectEnum.None;
		//セル結合可
		this.grdList.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;

	}

	/// <summary>
	/// 更新対象エリアの項目初期化
	/// </summary>
	protected override void initUpdateAreaItems()
	{
		//Gridの初期化を回避するため(remove->add)
		this.gbxUpdate.Controls.Remove(this.grdYoyakuStop);
		CommonUtil.Control_Init(this.gbxUpdate.Controls);
		this.gbxUpdate.Controls.Add(this.grdYoyakuStop);

		//背景色初期化はここ
		base.clearExistErrorProperty(this.gbxUpdate.Controls);

		this.grdYoyakuStop.Rows.Count = 1;

		//行選択モード
		this.grdYoyakuStop.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;

		//編集不可
		for (int col = 1; col <= this.grdYoyakuStop.Cols.Count - 1; col++)
		{
			this.grdYoyakuStop.Cols(col).AllowEditing = false;
		}
		//編集可
		this.grdYoyakuStop.Cols(grdYoyakuteisiColNum.YOYAKU_STOP_FLG).AllowEditing = true;

		// 予約停止一覧の件数
		this.lblDetailCnt.Text = string.Format("{0:#,0}件", getDetailListCount()).PadLeft(6);

	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	protected override void initFooterButtonControl()
	{

		//フォーム起動時のフッタの各ボタンの設定
		F1Key_Visible = false; // F1:未使用
		F2Key_Visible = true; // F2:戻る
		F3Key_Visible = false; // F3:未使用
		F4Key_Visible = false; // F4:未使用
		F5Key_Visible = false; // F5:未使用
		F6Key_Visible = false; // F6:未使用
		F7Key_Visible = false; // F7:未使用
		F8Key_Visible = false; // F8:未使用
		F9Key_Visible = false; // F9:未使用
		F10Key_Visible = false; // F10:未使用
		F11Key_Visible = true; // F11:更新
		F12Key_Visible = false; // F12:未使用

		this.F11Key_AllowAuth = FixedCd.AuthLevel.update;

	}

	/// <summary>
	/// エンティティ初期化
	/// </summary>
	protected override void initEntityData()
	{
		_updateDetailEntity.clear();
	}
	#endregion

	#region 変更確認
	/// <summary>
	/// 差分チェック
	/// </summary>
	protected override bool checkDifference()
	{
		//差分チェック
		bool retValue = true;

		for (int idxDetail = 0; idxDetail <= _updateDetailEntity.EntityData.Length - 1; idxDetail++)
		{
			if (_updateDetailEntity.compare(idxDetail) == false)
			{
				retValue = false;
			}
		}

		return retValue;
	}
	#endregion

	#region チェック系
	/// <summary>
	/// 検索入力項目チェック
	/// </summary>
	protected override bool checkSearchItems()
	{
		return CheckSearch();
	}

	/// <summary>
	/// 更新入力項目チェック
	/// </summary>
	protected override bool checkUpdateItems()
	{
		return CheckUpdate();
	}

	/// <summary>
	/// 更新入力項目チェック
	/// </summary>
	protected override bool checkInsertItems()
	{
		return CheckInsert();
	}
	#endregion

	#region 固有初期処理
	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{

		//フォーカスセット
		setSeFirsttDisplayData();

		//ベースフォームの初期化処理
		base.mainGrid = this.grdList;
		base.mainGridDataCntDsp = this.lblLengthGrd;

		//ベースフォームの初期化処理
		base.initScreenPerttern();

		//検索ボタンの関連付け
		btnSearch.Click += base.btnCom_Click;
		btnClear.Click += base.btnCom_Click;

		base.UpdateMsgParam = this._updateMsg;

	}

	#endregion

	#region 画面->エンティティ
	/// <summary>
	/// 更新対象項目をエンティティにセット
	/// </summary>
	protected override void setEntityDataValue()
	{

		// 画面からエンティティに設定する処理(必須画面個別実装)
		object temp_ent = (object)this._updateDetailEntity;
		this.DisplayDataToEntity(ref temp_ent);

	}
	#endregion

	#region Grid、データ関連
	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void reloadGrid()
	{
		//取得結果をグリッドへ設定
		base.reloadGrid();

		if (base.SearchResultGridData.Rows.Count >= 1)
		{

			// 一覧マージ設定
			mergeGrdList();

		}

		// スクロール位置を変更する (一番左に設定)
		this.grdList.LeftCol = grdListColNum.PICKUP_ROUTE_CD;

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

		//更新対象項目の初期化
		initUpdateAreaItems();

		//引数の設定
		whereString = MakeWhere(grdList, _entityKeys);

		//問合せ対象データ取得
		selectData = base.SearchResultGridData.Select(whereString);

		if (selectData.Length > 0)
		{

			// 予約停止一覧に表示するコース台帳情報を取得
			_detailDataTable = getDetailData(selectData[0]);

			// 予約停止一覧にデータ表示
			setDetailGrid(_detailDataTable);

			// ↓これがないと、画面を閉じる際、差分チェックがスルーされる
			// 前回値設定実績あり
			base.setZenkaiValueFlg = true;

		}
		else
		{
			//Entityの初期化
			initEntityData();
			//更新対象エリアの項目初期化
			initUpdateAreaItems();
		}

		//データ件数を取得
		int listCount = getDetailListCount();

		// 予約停止一覧の件数
		this.lblDetailCnt.Text = string.Format("{0:#,0}件", listCount).PadLeft(6);

		if (listCount.Equals(0))
		{
			//更新ボタンを使用不可に設定
			this.F11Key_Enabled = false;
		}
		else
		{
			//更新ボタンを使用可に設定
			this.F11Key_Enabled = true;
		}

	}

	/// <summary>
	/// WHERE文字列作成
	/// </summary>
	/// <param name="grd">対象Grid</param>
	/// <param name="keys">Key配列</param>
	private string MakeWhere(FlexGridEx grd, string[] keys)
	{
		string whereString = "";
		FlexGridEx with_1 = grd;
		if (with_1.Row >= 0 && with_1.Rows.Count > 1)
		{
			foreach (string s in keys)
			{
				if (with_1.Item(with_1.Row, s).ToString() != string.Empty)
				{
					if (whereString != string.Empty)
					{
						whereString += " AND ";
					}
					whereString += s + " = '" + with_1.Item(with_1.Row, s).ToString() + "'";
				}
			}
		}

		return whereString;
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
		grdList.MergedRanges.Clear();

		for (int col1 = grdListColNum.PICKUP_ROUTE_CD; col1 <= grdList.Cols.Count - 1; col1++)
		{

			topRow = 1;
			bottomRow = 1;

			for (int row1 = 1; row1 <= grdList.Rows.Count - 1; row1++)
			{

				// 最終行
				if (row1.Equals(grdList.Rows.Count - 1))
				{
					cr = grdList.GetCellRange(topRow, col1, row1, col1);
					grdList.MergedRanges.Add(cr);

					break;
				}

				// マージチェック（次行データと同じか）
				if (checkRowdata(row1, col1) == false)
				{

					// 次行データと異なるため、マージする
					cr = grdList.GetCellRange(topRow, col1, row1, col1);
					grdList.MergedRanges.Add(cr);

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
		for (int col1 = grdListColNum.PICKUP_ROUTE_CD; col1 <= nowCol; col1++)
		{
			// 次行データと同じか
			if (ReferenceEquals(grdList(nowRow, col1), null) == false && ReferenceEquals(grdList(nowRow + 1, col1), null) == false)
			{
				if (!(grdList(nowRow, col1).ToString().TrimEnd.Equals(grdList(nowRow + 1, col1).ToString().TrimEnd)))
				{
					// 異なる
					return false;
				}
			}
		}

		return true;
	}

	/// <summary>
	/// 予約停止一覧の件数
	/// </summary>
	/// <returns></returns>
	private int getDetailListCount()
	{

		return System.Convert.ToInt32(this.grdYoyakuStop.Rows.Count - this.grdYoyakuStop.Rows.Fixed);

	}
	#endregion

	#region DB関連

	/// <summary>
	/// マスタデータ更新
	/// </summary>
	protected override int ExecuteUpdateMaster()
	{

		//戻り値
		int returnValue = 0;
		//DBパラメータ
		ArrayList paramInfoListDetailList = new ArrayList();

		//TODO:DAクラス作成、パラメータ設定、登録(更新)処理実施までを実装
		//呼ばれる時点で、UpdateEntit.EntityData(0)には画面の値は設定されている
		//DataAccessクラス生成
		PickupYoyakuTeisi_DA dataAccess = new PickupYoyakuTeisi_DA();

		//パラメータ作成(共通部)
		//CommonLogic.setTableCommonInfo(DbShoriKbn.Update, Me.Name, CType(_updateDetailEntity.EntityData(0), iEntity))
		// iEntity 継承なしのため個別に設定
		for (int row = 0; row <= _updateDetailEntity.EntityData.Length - 1; row++)
		{
			_updateDetailEntity.EntityData(row).SystemUpdatePgmid.Value = this.Name;
			_updateDetailEntity.EntityData(row).SystemUpdatePersonCd.Value = UserInfoManagement.userId;
			_updateDetailEntity.EntityData(row).SystemUpdateDay.Value = CommonProcess.getDateTime();
		}

		// エンティティデータをハッシュテーブルテーブル に変換
		paramInfoListDetailList = entityDetailToHash(_updateDetailEntity);

		try
		{
			//Updateの実施
			returnValue = System.Convert.ToInt32(dataAccess.executePickupRouteLedger(PickupYoyakuTeisi_DA.accessType.executeUpdatePickupRoute, paramInfoListDetailList));
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
	/// 対象マスタのデータ取得
	/// </summary>
	protected override DataTable getMstData()
	{
		return GetDbTable();
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

		//TODO:初期フォーカスのコントロールを設定を実装
		this.txtPickupRootCdSearch.Select();

	}

	#endregion

	#region エンティティ操作系
	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">ピックアップルートマスタエンティティ</param>
	public void DisplayDataToEntity(ref object ent)
	{

		this.setGridDetailToEntity(this.grdYoyakuStop, (EntityOperation[OfTPickupRouteLedgerEntity]) ent);

	}

	/// <summary>
	/// エンティティから画面に設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">エンティティ</param>
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

	/// <summary>
	/// 予約停止一覧に表示するコース台帳情報を取得
	/// </summary>
	/// <param name="pDataRow">GridのDataBind(DataTable)の選択行</param>
	/// <remarks></remarks>
	private DataTable getDetailData(DataRow pDataRow)
	{
		DataTable dtDetail = null;

		PickupYoyakuTeisi_DA dataAccess = new PickupYoyakuTeisi_DA();
		Hashtable paramInfoList = new Hashtable();

		try
		{
			//詳細を取得する
			object with_1 = _updateDetailEntity.EntityData(0);
			// パラメータ設定
			paramInfoList.Add(with_1.PickupRouteCd.PhysicsName, pDataRow.Item(with_1.PickupRouteCd.PhysicsName).ToString());
			paramInfoList.Add(with_1.StartDay.PhysicsName, (Date?)(pDataRow.Item(with_1.StartDay.PhysicsName).ToString()));
			// コース台帳取得
			dtDetail = dataAccess.accessPickupRouteMaster(PickupYoyakuTeisi_DA.accessType.getPickupYoyakuTeisi, paramInfoList);

		}
		catch (OracleException)
		{
			throw;
		}
		catch (Exception)
		{
			throw;
		}

		return dtDetail;
	}

	/// <summary>
	/// 予約停止一覧をコース台帳エンティティに退避する
	/// </summary>
	/// <param name="grd"></param>
	/// <param name="ent"></param>
	private void setGridDetailToEntity(FlexGridEx grd, EntityOperation[] ent)
	{
		int idxDetailCnt = 0;
		TPickupRouteLedgerEntity entDetail = null;
		int entRow = 0;

		ent.clear();

		for (int idxTbl = grd.Rows.Fixed; idxTbl <= grd.Rows.Count - 1; idxTbl++)
		{

			// [変更なし]の行はスキップ (DBへ反映しない)
			if (grd.GetCellCheck(idxTbl, grdYoyakuteisiColNum.YOYAKU_STOP_FLG).Equals(grd.GetCellCheck(idxTbl, grdYoyakuteisiColNum.YOYAKU_STOP_FLG_HIDDEN)))
			{
				continue;
			}

			entDetail = new TPickupRouteLedgerEntity();

			// 出発日
			entDetail.SyuptDay.Value = (Integer?)(grd.GetData(idxTbl, grdYoyakuteisiColNum.SYUPT_DAY).ToString());

			// PUルートコード
			entDetail.PickupRouteCd.Value = grd.GetData(idxTbl, grdYoyakuteisiColNum.PICKUP_ROUTE_CD).ToString();

			// 開始日
			entDetail.StartDay.Value = (Date?)(grd.GetData(idxTbl, grdYoyakuteisiColNum.START_DAY).ToString());

			// 予約停止
			if (grd.GetCellCheck(idxTbl, grdYoyakuteisiColNum.YOYAKU_STOP_FLG).Equals(CheckEnum.Checked))
			{
				entDetail.YoyakuStopFlg.Value = FixedCd.YoyakuStopFlg.Teishi;
			}

			ent.Add(entDetail);
		}

		//1行目は削除する(空レコードのため)
		if (ent.EntityData.Length > 1)
		{
			ent.Remove(0);
		}

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

		//大小チェック開始日
		if ((dtmStartDayFromTo.FromDateText IsNot null && dtmStartDayFromTo.ToDateText IsNot null) &&)
		{
			!(CommonDateUtil.chkDayFromTo(System.Convert.ToDateTime(dtmStartDayFromTo.FromDateText), System.Convert.ToDateTime(dtmStartDayFromTo.ToDateText)));
			// 背景色を入力不足の色に変更する
			this.dtmStartDayFromTo.ExistErrorForFromDate = true;
			this.dtmStartDayFromTo.ExistErrorForToDate = true;
			CommonProcess.createFactoryMsg().messageDisp("E90_052", "出発日From", "出発日To");
			return false;
		}
		//大小チェック終了日
		if ((dtmEndDayFromTo.FromDateText IsNot null && dtmEndDayFromTo.ToDateText IsNot null) &&)
		{
			!(CommonDateUtil.chkDayFromTo(System.Convert.ToDateTime(dtmEndDayFromTo.FromDateText), System.Convert.ToDateTime(dtmEndDayFromTo.ToDateText)));
			// 背景色を入力不足の色に変更する
			dtmEndDayFromTo.ExistErrorForFromDate = true;
			dtmEndDayFromTo.ExistErrorForToDate = true;
			CommonProcess.createFactoryMsg().messageDisp("E90_052", "終了日From", "終了日To");
			return false;
		}

		// 大小チェック到着時間
		if ((timTtyakTimeFromTo.FromTimeValue24 IsNot null && timTtyakTimeFromTo.ToTimeValue24 IsNot null) &&)
		{
			timTtyakTimeFromTo.ToTimeValue24Int(< timTtyakTimeFromTo.FromTimeValue24Int);
			// 背景色を入力不足の色に変更する
			timTtyakTimeFromTo.ExistErrorForFromTime = true;
			timTtyakTimeFromTo.ExistErrorForToTime = true;
			CommonProcess.createFactoryMsg().messageDisp("E90_052", "到着時間From", "到着時間To");
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

		// 使用中チェック
		if (checkDetailUsing() == false)
		{
			return false;
		}

		// チェックが全て OK の場合
		// コース台帳 使用中フラグ更新("" → "Y")
		setPickupRouteLedgerUsingFlg(System.Convert.ToString(UsingFlg.Use));

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
		return true;
	}

	/// <summary>
	/// 必須入力項目エラーチェック
	/// </summary>
	/// <param name="pChkKbn">チェック区分(True:新規 Flase:更新)</param>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isExistHissuErrorEx(bool pChkKbn)
	{
		return true;
	}

	/// <summary>
	/// 使用中チェック（コース台帳）
	/// </summary>
	/// <returns></returns>
	private bool checkDetailUsing()
	{

		PickupYoyakuTeisi_DA dataAccess = new PickupYoyakuTeisi_DA();
		DataTable chkTable = null;

		foreach (TPickupRouteLedgerEntity detailEntity in _updateDetailEntity.EntityData)
		{

			string syuptDay = System.Convert.ToString(detailEntity.SyuptDay.Value.ToString());
			string pickupRouteCd = System.Convert.ToString(detailEntity.PickupRouteCd.Value.ToString());
			string startDay = System.Convert.ToString(detailEntity.StartDay.Value.ToString());

			chkTable = dataAccess.getUsingFlg(syuptDay, pickupRouteCd, startDay);
			if (ReferenceEquals(chkTable, null) ||)
			{
				chkTable.Rows.Count = 0;
				string errMsg = string.Format("データなし ({0}) ", (int.Parse(syuptDay)).ToString("0000/00/00"));
				// エラーが発生しました。[{1}]
				CommonProcess.createFactoryMsg().messageDisp("E90_046", errMsg);
				return false;
			}

			// 使用中フラグが "Y" の場合、エラー
			string usingFlg = System.Convert.ToString(chkTable.Rows(0).Item(0).ToString());
			if (usingFlg.Equals(FixedCd.UsingFlg.Use))
			{
				string errMsg = string.Format(" ({0}) ", (int.Parse(syuptDay)).ToString("0000/00/00"));
				// コースが修正中（使用中）のため、予約変更できません。{1}
				CommonProcess.createFactoryMsg().messageDisp("E04_004", errMsg);
				return false;
			}

		}

		return true;
	}
	#endregion

	#endregion

	#region DB取得処理

	/// <summary>
	/// データ取得処理(必須画面個別実装)
	/// </summary>
	/// <returns>取得データ(DataTable)</returns>
	private DataTable GetDbTable()
	{
		//戻り値
		DataTable returnValue = null;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		MPickupRouteEntity clsMPickupRouteEntity = new MPickupRouteEntity();

		//DataAccessクラス生成
		PickupYoyakuTeisi_DA dataAccess = new PickupYoyakuTeisi_DA();

		//パラメータ設定
		string pickupRootCd = string.Empty;
		string crsJyosyaTi = string.Empty;
		string hotelNameJyosyaTi = string.Empty;
		string startDayFrom = string.Empty;
		string startDayTo = string.Empty;
		string endDayFrom = string.Empty;
		string endDayTo = string.Empty;
		string ttyakTimeFrom = string.Empty;
		string ttyakTimeTo = string.Empty;

		pickupRootCd = System.Convert.ToString(this.txtPickupRootCdSearch.Text);
		crsJyosyaTi = System.Convert.ToString(this.ucoPlaceCdSearch.CodeText.PadRight(clsMPickupRouteEntity.CrsJyosyaTi.IntegerBu));
		hotelNameJyosyaTi = System.Convert.ToString(this.txtHotelName_JyochachiSearch.Text);
		if (!ReferenceEquals(this.dtmStartDayFromTo.FromDateText, null))
		{
			startDayFrom = System.Convert.ToString(this.dtmStartDayFromTo.FromDateText);
		}
		if (!ReferenceEquals(this.dtmStartDayFromTo.ToDateText, null))
		{
			startDayTo = System.Convert.ToString(this.dtmStartDayFromTo.ToDateText);
		}
		if (!ReferenceEquals(this.dtmEndDayFromTo.FromDateText, null))
		{
			endDayFrom = System.Convert.ToString(this.dtmEndDayFromTo.FromDateText);
		}
		if (!ReferenceEquals(this.dtmEndDayFromTo.ToDateText, null))
		{
			endDayTo = System.Convert.ToString(this.dtmEndDayFromTo.ToDateText);
		}
		if (!(ReferenceEquals(this.timTtyakTimeFromTo.FromTimeValue24, null)))
		{
			ttyakTimeFrom = System.Convert.ToString(this.timTtyakTimeFromTo.FromTimeValue24Int);
		}
		if (!(ReferenceEquals(this.timTtyakTimeFromTo.ToTimeValue24, null)))
		{
			ttyakTimeTo = System.Convert.ToString(this.timTtyakTimeFromTo.ToTimeValue24Int);
		}

		// ピックアップルートコード
		if (!string.IsNullOrWhiteSpace(pickupRootCd))
		{
			paramInfoList.Add(clsMPickupRouteEntity.PickupRouteCd.PhysicsName, pickupRootCd);
		}
		// コース乗車地コード
		if (!string.IsNullOrWhiteSpace(crsJyosyaTi))
		{
			paramInfoList.Add(clsMPickupRouteEntity.CrsJyosyaTi.PhysicsName, crsJyosyaTi);
		}
		// ホテル名・乗車地
		if (!string.IsNullOrWhiteSpace(hotelNameJyosyaTi))
		{
			paramInfoList.Add(PickupYoyakuTeisi_DA.ParamKeys.hotelName, hotelNameJyosyaTi);
		}
		// 開始日 (FROM)
		if (!string.IsNullOrEmpty(startDayFrom))
		{
			paramInfoList.Add(PickupYoyakuTeisi_DA.ParamKeys.startDayFrom, System.Convert.ToDateTime(startDayFrom));
		}
		// 開始日 (TO)
		if (!string.IsNullOrEmpty(startDayTo))
		{
			paramInfoList.Add(PickupYoyakuTeisi_DA.ParamKeys.startDayTo, System.Convert.ToDateTime(startDayTo));
		}
		// 終了日 (FROM)
		if (!string.IsNullOrEmpty(endDayFrom))
		{
			paramInfoList.Add(PickupYoyakuTeisi_DA.ParamKeys.endDayFrom, System.Convert.ToDateTime(endDayFrom));
		}
		// 終了日 (TO)
		if (!string.IsNullOrEmpty(endDayTo))
		{
			paramInfoList.Add(PickupYoyakuTeisi_DA.ParamKeys.endDayTo, System.Convert.ToDateTime(endDayTo));
		}
		// 到着時間 (FROM)
		if (!string.IsNullOrEmpty(ttyakTimeFrom))
		{
			paramInfoList.Add(PickupYoyakuTeisi_DA.ParamKeys.ttyakTimeFrom, (Integer?)ttyakTimeFrom);
		}
		// 到着時間 (TO)
		if (!string.IsNullOrEmpty(ttyakTimeTo))
		{
			paramInfoList.Add(PickupYoyakuTeisi_DA.ParamKeys.ttyakTimeTo, (Integer?)ttyakTimeTo);
		}

		try
		{
			returnValue = dataAccess.accessPickupRouteMaster(PickupYoyakuTeisi_DA.accessType.getPickupRoute, paramInfoList);
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
	/// エンティティデータをハッシュテーブルテーブル に変換
	/// </summary>
	/// <param name="headEnt"></param>
	/// <returns></returns>
	public ArrayList entityDetailToHash(object headEnt)
	{
		ArrayList arrayList = new ArrayList();
		Hashtable hash = new Hashtable();
		EntityOperation[] ent = new EntityOperation[Of TPickupRouteLedgerEntity + 1];
		ent = (EntityOperation[OfTPickupRouteLedgerEntity]) headEnt;

		//カラム情報作成
		for (int idxRow = 0; idxRow <= ent.EntityData.Length - 1; idxRow++)
		{
			if (!string.Empty.Equals(ent.EntityData(idxRow).SyuptDay.Value))
			{
				hash = new Hashtable();
				for (int idx = 0; idx <= ent.getPropertyDataLength - 1; idx++)
				{
					if (ReferenceEquals(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).GetType, typeof(EntityKoumoku_MojiType)))
					{
						hash.Add(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).PhysicsName, ((EntityKoumoku_MojiType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).Value);
					}
					else if (ReferenceEquals(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).GetType, typeof(EntityKoumoku_NumberType)))
					{
						hash.Add(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).PhysicsName, ((EntityKoumoku_NumberType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).Value);
					}
					else if (ReferenceEquals(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).GetType, typeof(EntityKoumoku_YmdType)))
					{
						hash.Add(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).PhysicsName, ((EntityKoumoku_YmdType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).Value);
					}
					else if (ReferenceEquals(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).GetType, typeof(EntityKoumoku_Number_DecimalType)))
					{
						hash.Add(((IEntityKoumokuType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).PhysicsName, ((EntityKoumoku_Number_DecimalType)(ent.getPtyValue(idx, ent.EntityData(idxRow)))).Value);
					}
				}
				arrayList.Add(hash);
			}
		}

		return arrayList;

	}

	/// <summary>
	/// 予約停止一覧にデータを設定
	/// </summary>
	/// <param name="pdtDetail">コース台帳詳細DataTable</param>
	private void setDetailGrid(DataTable pdtDetail)
	{

		int setRow = 0;

		// 行数初期化
		this.grdYoyakuStop.Rows.Count = this.grdYoyakuStop.Rows.Fixed;

		// コース台帳一覧にデータ表示
		object with_1 = this.grdYoyakuStop;

		foreach (DataRow dr in pdtDetail.Rows)
		{

			with_1.Rows.Count += 1;
			setRow = System.Convert.ToInt32(with_1.Rows.Count - 1);

			// PUルートコード（非表示）
			with_1.SetData(setRow, grdYoyakuteisiColNum.PICKUP_ROUTE_CD, dr.Item(grdYoyakuteisiColNum.PICKUP_ROUTE_CD).ToString());

			// 開始日（非表示）
			with_1.SetData(setRow, grdYoyakuteisiColNum.START_DAY, dr.Item(grdYoyakuteisiColNum.START_DAY).ToString());

			// 出発日（YYYY/MM/DD）
			with_1.SetData(setRow, grdYoyakuteisiColNum.SYUPT_DAY_DISP, dr.Item(grdYoyakuteisiColNum.SYUPT_DAY_DISP).ToString());

			// 出発日（非表示）
			with_1.SetData(setRow, grdYoyakuteisiColNum.SYUPT_DAY, dr.Item(grdYoyakuteisiColNum.SYUPT_DAY).ToString());

			// 曜日
			with_1.SetData(setRow, grdYoyakuteisiColNum.YOBI, dr.Item(grdYoyakuteisiColNum.YOBI).ToString());

			// 予約停止
			if (dr.Item(grdYoyakuteisiColNum.YOYAKU_STOP_FLG).ToString().Equals(FixedCd.YoyakuStopFlg.Teishi))
			{
				with_1.SetCellCheck(setRow, grdYoyakuteisiColNum.YOYAKU_STOP_FLG, CheckEnum.Checked);
			}
			else
			{
				with_1.SetCellCheck(setRow, grdYoyakuteisiColNum.YOYAKU_STOP_FLG, CheckEnum.Unchecked);
			}

			// 台数計
			if (string.IsNullOrWhiteSpace(dr.Item(grdYoyakuteisiColNum.DAISU).ToString()) == false)
			{
				with_1.SetData(setRow, grdYoyakuteisiColNum.DAISU, dr.Item(grdYoyakuteisiColNum.DAISU));
			}

			// 人員計（ルート）
			if (string.IsNullOrWhiteSpace(dr.Item(grdYoyakuteisiColNum.NINZU).ToString()) == false)
			{
				with_1.SetData(setRow, grdYoyakuteisiColNum.NINZU, dr.Item(grdYoyakuteisiColNum.NINZU));
			}

			// 定数計（定席）
			if (string.IsNullOrWhiteSpace(dr.Item(grdYoyakuteisiColNum.CAPA).ToString()) == false)
			{
				with_1.SetData(setRow, grdYoyakuteisiColNum.CAPA, dr.Item(grdYoyakuteisiColNum.CAPA));
			}

			// 定数計（補助席・１階）
			if (string.IsNullOrWhiteSpace(dr.Item(grdYoyakuteisiColNum.EMGCAPA).ToString()) == false)
			{
				with_1.SetData(setRow, grdYoyakuteisiColNum.EMGCAPA, dr.Item(grdYoyakuteisiColNum.EMGCAPA));
			}

			// 予約停止（非表示）
			if (dr.Item(grdYoyakuteisiColNum.YOYAKU_STOP_FLG).ToString().Equals(FixedCd.YoyakuStopFlg.Teishi))
			{
				with_1.SetCellCheck(setRow, grdYoyakuteisiColNum.YOYAKU_STOP_FLG_HIDDEN, CheckEnum.Checked);
			}
			else
			{
				with_1.SetCellCheck(setRow, grdYoyakuteisiColNum.YOYAKU_STOP_FLG_HIDDEN, CheckEnum.Unchecked);
			}

			// 使用中フラグ
			with_1.SetData(setRow, grdYoyakuteisiColNum.USING_FLG, dr.Item(grdYoyakuteisiColNum.USING_FLG).ToString());

		}


	}

	#endregion

	#region 使用中フラグ更新処理
	/// <summary>
	/// 使用中フラグ更新処理
	/// </summary>
	private void setPickupRouteLedgerUsingFlg(string inUsingFlg)
	{

		// エンティティ→データテーブル
		DataTable selData = new DataTable();
		selData.Columns.Add(_updateDetailEntity.EntityData(0).SyuptDay.PhysicsName);
		selData.Columns.Add(_updateDetailEntity.EntityData(0).PickupRouteCd.PhysicsName);
		selData.Columns.Add(_updateDetailEntity.EntityData(0).StartDay.PhysicsName);

		foreach (TPickupRouteLedgerEntity detailEntity in _updateDetailEntity.EntityData)
		{

			string syuptDay = System.Convert.ToString(detailEntity.SyuptDay.Value.ToString());
			string pickupRouteCd = System.Convert.ToString(detailEntity.PickupRouteCd.Value.ToString());
			string startDay = System.Convert.ToString(detailEntity.StartDay.Value.ToString());

			DataRow dr = selData.NewRow;
			dr[_updateDetailEntity.EntityData(0).SyuptDay.PhysicsName] = syuptDay;
			dr[_updateDetailEntity.EntityData(0).PickupRouteCd.PhysicsName] = pickupRouteCd;
			dr[_updateDetailEntity.EntityData(0).StartDay.PhysicsName] = startDay;

			selData.Rows.Add(dr);
		}

		// 使用中フラグ更新
		PickupYoyakuTeisi_DA dataAccess = new PickupYoyakuTeisi_DA();
		if (inUsingFlg == FixedCd.UsingFlg.Use)
		{
			// ("" → "Y")
			dataAccess.executeUsingFlg_Pickup(selData, this.Name);
		}
		else
		{
			// クリア
			dataAccess.executeUsingFlgClear_Pickup(selData, this.Name);
		}

	}
	#endregion

	#endregion

}