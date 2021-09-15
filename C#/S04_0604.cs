using System.ComponentModel;


/// <summary>
/// ピックアップ時刻修正 S04_0604
/// </summary>
public class S04_0604 : PT11, iPT11
{

	#region 変数・定数
	#region 変数
	/// <summary>
	/// チェック用データテーブル
	/// </summary>
	private DataTable selectOldData;
	/// <summary>
	///ピックアップルート台帳 （ホテル）エンティティ
	/// </summary>
	public EntityOperation UpdateEntity; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.

	/// <summary>
	///検索ボタン押下時出発日
	/// </summary>
	private int searchSyuptDay;
	#endregion
	#region 定数
	/// <summary>
	/// 画面ID
	/// </summary>
	private const string pgmid = "S04_0604";
	/// <summary>
	/// 画面タイトル
	/// </summary>
	private const string title = "ピックアップ時刻修正";
	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	public const int TopGbxCondition = 41;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	public const int MarginGbxCondition = 6;

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

	/// <summary>
	/// 時間初期値
	/// </summary>
	private const string TimeInitialValue = "__:__";
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
	#region 列挙
	/// <summary>
	/// グリッドカラム一覧
	/// </summary>
	private enum grdListColNum
	{
		PICKUP_ROUTE_CD = 1,
		USING_FLG_DISP,
		YOYAKU_STOP_FLG_DISP,
		TTYAK_TI,
		TTYAK_TIME,
		HOTEL_NAME_JYOSYA_TI,
		RK,
		NINZU,
		FUKEIYU,
		SYUPT_TIME,
		SYUPT_TIME_MOTO,
		USING_FLG,
		YOYAKU_STOP_FLG,
		PICKUP_HOTEL_CD,
		START_DAY
	}
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
	#endregion
	#region イベント
	/// <summary>
	/// フォームキーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S04_0604_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			//検索ボタン押下
			btnF8_ClickOrgProc();
		}
		else
		{
			//nop
		}
	}
	#region 検索ボタン押下時
	/// <summary>
	/// F8：検索ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			//前処理
			base.comPreEvent();

			//更新ボタン活性
			this.F11Key_Enabled = false;

			// 実処理
			this.btnF8_ClickOrgProc();
		}
		catch (Exception ex)
		{
			createFactoryMsg.messageDisp("E90_046", ex.Message);
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, "S04_0604", ex.Message);
			throw;
		}
		finally
		{
			// 後処理
			base.comPostEvent();
		}
	}
	#endregion
	#region 更新ボタン押下時
	//イベントは発生しない
	//メソッド内（更新内）に記述済
	#endregion
	#region 条件クリアボタン押下時
	/// <summary>
	/// 条件クリアボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnClear_Click(object sender, EventArgs e)
	{
		ButtonEx btn = TryCast(sender, ButtonEx);
		btn.Focus();
		//条件部分初期化
		this.initSearchAreaItems();

		//グリッドの初期化
		grdList.DataSource = new DataTable();

		//更新ボタン非活性
		this.F11Key_Enabled = false;
	}
	#endregion
	#region 表示・非表示ボタン押下時
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
	#endregion
	#region 戻るボタン押下時
	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	protected void btnF2_Click(object sender, EventArgs e)
	{
		base.btnF2_ClickOrgProc();
		this.Close();
	}


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
			for (i = 1; i <= grdList.Rows.Count - 1; i++)
			{

				//(変更前後の出発時間が共にNULL　もしくは　変更前後の出発時間が同一)　以外の場合、に差分あり
				if (!(Information.IsDBNull(grdList.Rows(i)[grdListColNum.SYUPT_TIME]) == false && Information.IsDBNull(grdList.Rows(i)[grdListColNum.SYUPT_TIME_MOTO]) == false &&)
				{
					System.Convert.ToString(grdList.Rows(i)[grdListColNum.SYUPT_TIME]) = System.Convert.ToString(System.Convert.ToString(grdList.Rows(i)[grdListColNum.SYUPT_TIME_MOTO]) ||);
					Information.IsDBNull(grdList.Rows(i)[grdListColNum.SYUPT_TIME]) = true && Information.IsDBNull(grdList.Rows(i)[grdListColNum.SYUPT_TIME_MOTO]) == true);

			return true;
		}

	}
			return false;
		}
	}
#endregion
#endregion
#region カラム操作時
	/// <summary>
	/// データソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void dgvResult_Pick_DataSourceChanged(object sender, EventArgs e)
{
	//データ件数を表示(ヘッダー行分マイナス1)
	string formatedCount = System.Convert.ToString((this.grdList.Rows.Count - 1).ToString().PadLeft(6));
	lblLengthGrd.Text = formatedCount + "件";
	//データの行数が一行以上あるなら処理をする
	if (this.grdList.Rows.Count <= 1)
	{
		return;
	}
	//背景色の準備
	CellStyle cellColor_disabled = this.grdList.Styles.Add("disabled");
	cellColor_disabled.BackColor = SystemColors.ControlLight;
	//データソース確認
	DataTable table = (DataTable)this.grdList.DataSource;
	int counter = 0;
	foreach (Row row in this.grdList.Rows)
	{

		this.grdList.Cols(grdListColNum.SYUPT_TIME).Editor = this.timForGrid;

		//使用中フラグがYのときに変更を不可に設定（色変も）
		if (!(ReferenceEquals(row.Item(grdListColNum.USING_FLG.ToString()), null)))
		{
			if (row.Item(grdListColNum.USING_FLG.ToString()).ToString().Equals(FixedCd.UsingFlg.Use))
			{
				//編集不可
				this.grdList.Rows(counter).AllowEditing = false;
				//色変
				this.grdList.SetCellStyle(counter, grdListColNum.FUKEIYU, cellColor_disabled);
				this.grdList.SetCellStyle(counter, grdListColNum.SYUPT_TIME, cellColor_disabled);
			}
		}
		counter++;
	}

}
/// <summary>
/// 編集後イベント
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void dgvResult_Pick_AfterEdit(object sender, RowColEventArgs e)
{
	if (e.Col == grdListColNum.SYUPT_TIME)
	{
		this.grdList.SetCellCheck(e.Row, grdListColNum.FUKEIYU, CheckEnum.Unchecked);
	}
}

/// <summary>
/// 不経由チェックボックス操作時イベント
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void dgvResult_Pick_CellChecked(object sender, RowColEventArgs e)
{
	if (e.Col == grdListColNum.FUKEIYU)
	{
		switch (System.Convert.ToString(this.grdList.Rows(e.Row).Item(grdListColNum.FUKEIYU.ToString())))
		{
			case "1":
				this.grdList.Rows(e.Row).Item[grdListColNum.SYUPT_TIME.ToString()] = DBNull.Value;
				break;
			case "0":
				this.grdList.Rows(e.Row).Item[grdListColNum.SYUPT_TIME.ToString()] = this.grdList.Rows(e.Row).Item[grdListColNum.SYUPT_TIME_MOTO.ToString()];
				break;
				//nop
		}
	}
}
#endregion
#endregion
#region メソッド
#region 必須
/// <summary>
/// 対象マスタのデータ取得
/// </summary>
protected override DataTable getMstData()
{
	return GetDbTable(DbTableKbn.Select);
}
#region エンティティ操作系
/// <summary>
/// 画面からエンティティに設定する処理(必須画面個別実装)
/// </summary>
/// <param name="ent">エンティティ</param>
public void DisplayDataToEntity(ref object ent)
{

}
/// <summary>
/// エンティティから画面に設定する処理(必須画面個別実装)
/// </summary>
/// <param name="ent">エンティティ</param>
public void EntityDataToDisplay(ref object ent)
{
	//nop
}

/// <summary>
/// DataGridからエンティティ(前回値)に設定する処理(必須画面個別実装)
/// ※DataGrid上の1レコードから関連するデータも取得する。Keyがない場合などは未対応
/// </summary>
/// <param name="pDataRow">GridのDataBind(DataTable)の選択行</param>
/// <remarks></remarks>
public void OldDataToEntity(DataRow pDataRow)
{
	//nop
}
#endregion
#endregion
#region 新規作成メソッド
#region 検索

/// <summary>
/// データ取得処理(必須画面個別実装)
/// </summary>
/// <returns>取得データ(DataTable)</returns>
private DataTable GetDbTable(DbTableKbn kbn)
{
	//問い合わせ結果
	DataTable returnValue = null;
	//DBパラメータ
	Hashtable paramInfoList = new Hashtable();
	//DataAccessクラス生成
	PickupRouteLedger_DA dataAccess = new PickupRouteLedger_DA();
	//パラメータ格納
	//出発日
	if (!(ReferenceEquals(this.dtmSyuptDay.Value, null)))
	{
		DateTime day = System.Convert.ToDateTime(this.dtmSyuptDay.Value);
		searchSyuptDay = int.Parse(day.ToShortDateString().Replace("/", "").Trim());
		paramInfoList.Add("SyuptDay", searchSyuptDay);

	}

	//到着地(乗車地コード)
	string JyosyaTi = System.Convert.ToString(this.ucoJyosyachiCd.CodeText);
	if (!string.IsNullOrEmpty(JyosyaTi))
	{
		paramInfoList.Add("JyochachiCd", JyosyaTi);
	}

	// 到着時間Froms
	if (this.dtmTtyakTime.FromTimeValue24Int IsNot null)
		{
	paramInfoList.Add("TtyakTimeFrom", this.dtmTtyakTime.FromTimeValue24Int.ToString());

}
		else
{
	paramInfoList.Add("TtyakTimeFrom", "");
}
// 到着時間To
if (this.dtmTtyakTime.ToTimeValue24Int IsNot null)
		{
	paramInfoList.Add("TtyakTimeTo", this.dtmTtyakTime.ToTimeValue24Int.ToString());

}
		else
{
	paramInfoList.Add("TtyakTimeTo", "");
}


return dataAccess.accessPickupRouteInfor(PickupRouteLedger_DA.accessType.getPickupRouteInforByRoot, paramInfoList);

return returnValue;
		
	}
#endregion
#region チェック
	/// <summary>
	/// 検索処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckSearch()
{

	//背景色初期化
	base.clearExistErrorProperty(this.gbxCondition.Controls);

	//必須項目のチェック
	if (CommonUtil.checkHissuError(this.gbxCondition.Controls) == true)
	{
		CommonProcess.createFactoryMsg().messageDisp("E90_022");
		return false;
	}


	//出発日がシステム日付より過去日付
	if (CommonConvertUtil.ChkWhenDay(System.Convert.ToDateTime(this.dtmSyuptDay.Value), CommonConvertUtil.WhenKako) == true)
	{
		CommonProcess.createFactoryMsg().messageDisp("E90_013");
		dtmSyuptDay.ExistError = true;
		dtmSyuptDay.Focus();

		return false;
	}

	// 乗車地コード存在チェック
	if (this.ucoJyosyachiCd.CodeText.Trim() != "" && this.ucoJyosyachiCd.ValueText.Trim() == "")
	{
		//共通メッセージ処理[E90_014	該当の{1}が存在しません。]
		CommonProcess.createFactoryMsg().messageDisp("E90_014", "コース乗車地");
		this.ucoJyosyachiCd.Focus();
		return false;
	}
	// 到着時刻整合性チェック
	if (this.dtmTtyakTime.FromTimeValue24 IsNot null && this.dtmTtyakTime.ToTimeValue24 IsNot null)
		{
	if (this.dtmTtyakTime.FromTimeValue24Int > this.dtmTtyakTime.ToTimeValue24Int)
	{

		createFactoryMsg.messageDisp("E90_048");
		dtmTtyakTime.ExistErrorForFromTime = true;
		dtmTtyakTime.ExistErrorForToTime = true;
		dtmTtyakTime.Focus();
		return false;
	}
}
return true;
	}
	/// <summary>
	/// 更新録処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckInsert()
{
	//nop
	return true;
}
/// <summary>
/// 更新処理前のチェック処理(必須画面個別実装)
/// </summary>
/// <returns>True:エラー無 False:エラー有</returns>
public bool CheckUpdate()
{
	//nop
	return true;
}
/// <summary>
/// 更新処理前のチェック処理(必須画面個別実装)
/// </summary>
/// <returns>True:エラー無 False:エラー有</returns>
private bool CheckUpdate(int i)
{
	//nop
	return true;
}
/// <summary>
/// 必須入力項目エラーチェック
/// </summary>
/// <returns>True:エラー無 False:エラー有</returns>
/// <remarks></remarks>
public bool isExistHissuError()
{
	//nop
	return true;
}
#endregion
#endregion
#region オーバーライドメソッド
/// <summary>
/// 検索入力項目チェック
/// </summary>
protected override bool checkSearchItems()
{
	return CheckSearch();
}
/// <summary>
/// グリッドデータの取得とグリッド表示
/// </summary>
protected override void reloadGrid()
{

	//'取得結果をグリッドへ設定
	//MyBase.reloadGrid()

	grdList.DataSource = base.SearchResultGridData;
	if (base.SearchResultGridData.Rows.Count >= 1)
	{

		// 一覧マージ設定
		mergeGrdList();

	}

	// スクロール位置を変更する (一番左に設定)
	this.grdList.LeftCol = grdListColNum.PICKUP_ROUTE_CD;


	//' 検索時のグリッドを差分用に退避
	selectOldData = null;
	selectOldData = ((DataTable)this.grdList.DataSource).Copy;
}
#endregion
#region 更新
/// <summary>
/// f11ボタン押下時処理
/// </summary>
protected override void btnF11_ClickOrgProc()
{
	//変更フラグ
	bool flg = true;
	//変更内容保持用テーブル
	DataTable dt = new DataTable();
	dt.Columns.Add(grdListColNum.PICKUP_ROUTE_CD.ToString(), typeof(string));
	dt.Columns.Add(grdListColNum.TTYAK_TI.ToString(), typeof(string));
	dt.Columns.Add(grdListColNum.TTYAK_TIME.ToString(), typeof(DateTime));
	dt.Columns.Add(grdListColNum.HOTEL_NAME_JYOSYA_TI.ToString(), typeof(string));
	dt.Columns.Add(grdListColNum.RK.ToString(), typeof(string));
	dt.Columns.Add(grdListColNum.FUKEIYU.ToString(), typeof(bool));
	dt.Columns.Add(grdListColNum.SYUPT_TIME.ToString(), typeof(string));
	dt.Columns.Add(grdListColNum.USING_FLG.ToString(), typeof(string));
	dt.Columns.Add(grdListColNum.YOYAKU_STOP_FLG.ToString(), typeof(string));
	dt.Columns.Add(grdListColNum.SYUPT_TIME_MOTO.ToString(), typeof(string));
	dt.Columns.Add(grdListColNum.PICKUP_HOTEL_CD.ToString(), typeof(string));
	dt.Columns.Add(grdListColNum.START_DAY.ToString(), typeof(DateTime));
	//変更チェック
	int counter = 0;
	foreach (Row row in this.grdList.Rows)
	{
		//カウンターがO以上かつ、出発時間と出発時間（初期値）が等しくないとき
		if (counter > 0 && !(row.Item(grdListColNum.SYUPT_TIME.ToString(
			)).Equals(row.Item(grdListColNum.SYUPT_TIME_MOTO.ToString()))))
		{
			//乗車人数が0でない場合、不経由は不可
			if (row.Item(grdListColNum.NINZU.ToString()).ToString() != "0" && (row.Item(grdListColNum.SYUPT_TIME.ToString()).ToString() == "" || row.Item(grdListColNum.SYUPT_TIME.ToString()).ToString() == TimeInitialValue))
			{
				createFactoryMsg.messageDisp("E04_005");
				return;

			}

			flg = false;
			DataRow newRow = dt.NewRow;
			newRow.Item[grdListColNum.PICKUP_ROUTE_CD.ToString()] = row.Item[grdListColNum.PICKUP_ROUTE_CD.ToString()];
			newRow.Item[grdListColNum.TTYAK_TI.ToString()] = row.Item[grdListColNum.TTYAK_TI.ToString()];
			newRow.Item[grdListColNum.TTYAK_TIME.ToString()] = row.Item[grdListColNum.TTYAK_TIME.ToString()];
			newRow.Item[grdListColNum.HOTEL_NAME_JYOSYA_TI.ToString()] = row.Item[grdListColNum.HOTEL_NAME_JYOSYA_TI.ToString()];
			newRow.Item[grdListColNum.RK.ToString()] = row.Item[grdListColNum.RK.ToString()];
			newRow.Item[grdListColNum.FUKEIYU.ToString()] = row.Item[grdListColNum.FUKEIYU.ToString()];
			newRow.Item[grdListColNum.SYUPT_TIME.ToString()] = row.Item[grdListColNum.SYUPT_TIME.ToString()];
			newRow.Item[grdListColNum.USING_FLG.ToString()] = row.Item[grdListColNum.USING_FLG.ToString()];
			newRow.Item[grdListColNum.YOYAKU_STOP_FLG.ToString()] = row.Item[grdListColNum.YOYAKU_STOP_FLG.ToString()];
			newRow.Item[grdListColNum.SYUPT_TIME_MOTO.ToString()] = row.Item[grdListColNum.SYUPT_TIME_MOTO.ToString()];
			newRow.Item[grdListColNum.PICKUP_HOTEL_CD.ToString()] = row.Item[grdListColNum.PICKUP_HOTEL_CD.ToString()];
			newRow.Item[grdListColNum.START_DAY.ToString()] = row.Item[grdListColNum.START_DAY.ToString()];
			dt.Rows.Add(newRow);
		}
		counter++;
	}
	//変更がない場合
	if (flg)
	{
		//更新対象の値が変更されていません
		createFactoryMsg.messageDisp("E90_049");
		return;
	}

	//更新処理を行います。よろしいですか？
	if (createFactoryMsg.messageDisp("Q90_001", "更新") == MsgBoxResult.Cancel)
	{
		return;
	}

	//エンティティ生成
	List entList = new List(Of S04_0604_Entity);
	foreach (DataRow row in dt.Rows)
	{
		S04_0604_Entity ent = new S04_0604_Entity();
		//出発時間
		if (ReferenceEquals(row.Item(grdListColNum.SYUPT_TIME.ToString()), null) || ReferenceEquals(row.Item(grdListColNum.SYUPT_TIME.ToString()), DBNull.Value))
		{
			//nop
		}
		else
		{
			if (System.Convert.ToString(row.Item(grdListColNum.SYUPT_TIME.ToString())) == TimeInitialValue)
			{
				//nop
			}
			else
			{
				ent.syuptTime.Value = System.Convert.ToInt32(System.Convert.ToString(row.Item(grdListColNum.SYUPT_TIME.ToString())).Replace(":", ""));

			}
		}
		//出発時間（初期値）
		if (ReferenceEquals(row.Item(grdListColNum.SYUPT_TIME_MOTO.ToString()), null) || ReferenceEquals(row.Item(grdListColNum.SYUPT_TIME_MOTO.ToString()), DBNull.Value))
		{
			//nop
		}
		else
		{
			ent.syuptTime.ZenkaiValue = System.Convert.ToInt32(System.Convert.ToString(row.Item(grdListColNum.SYUPT_TIME_MOTO.ToString())).Replace(":", ""));

		}
		//PGMID
		ent.systemUpdatePgmid.Value = pgmid;
		//更新日時
		ent.systemUpdateDay.Value = CommonDateUtil.getSystemTime;
		//更新者ID
		ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
		//ルートコード
		ent.pickupRouteCd.Value = row.Item(grdListColNum.PICKUP_ROUTE_CD.ToString()).ToString();
		//ホテルコード
		ent.pickupHotelCd.Value = row.Item(grdListColNum.PICKUP_HOTEL_CD.ToString()).ToString();
		//開始日
		ent.startDay.Value = System.Convert.ToDateTime(row.Item(grdListColNum.START_DAY.ToString()));
		//出発日
		ent.syuptDay.Value = searchSyuptDay;
		//エンティティリストに格納
		entList.Add(ent);
	}
	//更新実行
	S04_0604_DA da = new S04_0604_DA();
	int result = System.Convert.ToInt32(da.update(entList));
	if (result == dt.Rows.Count)
	{
		//更新が完了しました。
		createFactoryMsg.messageDisp("I90_002", "更新");
		//再表示
		btnF8_ClickOrgProc();
	}
	else
	{
		//一部更新に失敗しました。[試行件数：*件    成功件数：*件]
		createFactoryMsg.messageDisp("E90_025", "一部更新", "試行件数：" + dt.Rows.Count + "件    成功件数：" + System.Convert.ToString(result) + "件");
	}
}
#endregion
#endregion
#region 初期化関連
#region イベント
/// <summary>
/// ロード時イベント
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void PT11_Load(object sender, EventArgs e)
{
	this.setFormId = pgmid;
	// 画面初期化
	setControlInitiarize();
}
#endregion
#region メソッド
#region 必須

/// <summary>
/// 初期処理(必須画面個別実装)
/// </summary>
public void setSeFirsttDisplayData()
{
	this.dtmSyuptDay.Select();
}
#endregion

#region Grid、データ関連
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
#endregion



#region 新規作成
/// <summary>
/// 画面初期化
/// </summary>
private void setControlInitiarize()
{
	//ベースフォームの設定
	this.setFormId = pgmid;
	this.setTitle = title;
	// フッタボタンの設定
	this.setButtonInitiarize();

	//セル結合可
	this.grdList.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;
	//setgrdGenbaraiEntryTaisyoCrsInquiry()
}

/// <summary>
/// フッタボタンの初期化
/// </summary>
private void setButtonInitiarize()
{
	//Visible
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
	this.F11Key_Visible = true;
	this.F12Key_Visible = false;
	//Text

	this.F2Key_Text = "F2:戻る";
	this.F11Key_Text = "F11:更新";

	//有効化
	this.F1Key_Enabled = false;
	this.F2Key_Enabled = true;
	this.F3Key_Enabled = false;
	this.F4Key_Enabled = false;
	this.F5Key_Enabled = false;
	this.F6Key_Enabled = false;
	this.F7Key_Enabled = false;
	this.F8Key_Enabled = true;
	this.F9Key_Enabled = false;
	this.F10Key_Enabled = false;
	this.F11Key_Enabled = false;
	this.F12Key_Enabled = false;

}
#endregion
#region オーバーライド
/// <summary>
/// 検索条件部の項目初期化
/// </summary>
protected override void initSearchAreaItems()
{
	CommonUtil.Control_Init(this.gbxCondition.Controls);
	this.dtmSyuptDay.Value = CommonDateUtil.getSystemTime(); // 出発日
	this.ucoJyosyachiCd.setControlInitiarize(); // 乗車地コード

	//背景色初期化
	base.clearExistErrorProperty(this.gbxCondition.Controls);
}
/// <summary>
/// エンティティ初期化
/// </summary>
protected override void initEntityData()
{
	UpdateEntity.clear();
}
/// <summary>
/// 固有初期処理
/// </summary>
protected override void initScreenPerttern()
{

	//フォーカスセット
	setSeFirsttDisplayData();

	//ベースフォームの初期化処理
	base.initScreenPerttern();

	this.grdList.DataSource = new DataTable();

	//件数0件設定
	lblLengthGrd.Text = "0".PadLeft(6) + "件";

	//ボタンの関連付け
	btnSearch.Click += base.btnCom_Click;
	btnClear.Click += base.btnCom_Click;

	//非使用ボタン設定
	this.F4Key_Visible = false; // F4:CSV出力
	this.F11Key_Visible = false; // F11:更新

	//検索条件を表示状態のGroupAreaのレイアウトを保存
	this.SaveGrpLayout();

}
	
#endregion
#endregion
#endregion
}