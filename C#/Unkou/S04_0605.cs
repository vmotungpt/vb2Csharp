using System.ComponentModel;
using System.Drawing;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.Master;


/// <summary>
/// S04_0605 ピックアップ在庫調整
/// </summary>
public class S04_0605 : FormBase
{

	DataTable searchResultBusDaisuOld;
	#region 定数
	/// <summary>
	/// 画面ID
	/// </summary>
	public const string pgmid = "S04_0605";
	/// <summary>
	/// 画面名
	/// </summary>
	public const string GamenName = "ピックアップ在庫調整";
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
	public const int HeightGbxAreasOnNotVisibleCondition = 480;
	/// <summary>
	/// 予約人数のカラムリスト
	/// </summary>
	private enum ColList
	{

		ROUTECD = 1,
		CAPA,
		EMGCAPA,
		DAISU,
		NINZUBUS,
		JOUSYATI,
		TTYAKUTIME,
		HOTELNAME,
		RK,
		SYUPTTIME,
		NINZUHOTEL,
		STARTDAY
	}
	/// <summary>
	/// バスのカラムリスト
	/// </summary>
	private enum ColListBus
	{
		LINENO = 1,
		CARCD,
		SANSYO,
		CAPA,
		EMGCAPA,
		DAISU,
		DAISUSUB
	}
	/// <summary>
	/// 合計欄のカラムリスト
	/// </summary>
	private enum ColListSum
	{
		CAPA = ColListBus.CAPA,
		EMGCAPA = ColListBus.EMGCAPA,
		DAISU = ColListBus.DAISU
	}
	/// <summary>
	/// DA用パラメータ名リスト
	/// </summary>
	private enum paramName
	{
		syuptDay, //出発日
		routeCd, //ピックアップルートコード
		jousyaTi, //乗車地
		ttyakuTimeFrom, //到着時間From
		ttyakuTimeTo, //到着時間To
		startDay, //開始日
		lineNo, //行No
		carCd, //車種コード
		daisu, //台数
		tourokuPGIMD, //登録画面ID
		tourokuUserId, //登録ユーザID
		tourokuDay, //登録日
		updatePGMID, //更新画面ID
		updateUserId, //更新者ID
		updateDay, //更新日
		tag, //選択行ナンバー
		ninzuRoute //予約人数
	}
	#endregion
	#region フィールド
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
	/// <summary>
	/// 検索結果格納用テーブル
	/// </summary>
	private DataTable SearchResult;
	/// <summary>
	/// バス台数検索結果用テーブル
	/// </summary>
	private DataTable BusResult;
	/// <summary>
	/// 人数予約パラメータ
	/// </summary>
	private Hashtable NinzuYoyakuParam = new Hashtable();
	/// <summary>
	/// 検索条件保持パラメータ
	/// </summary>
	private Hashtable SearchParam = new Hashtable();
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
	/// ロード時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void PT21_Load(object sender, EventArgs e)
	{
		// 検索条件、フッタボタン初期化
		this.setControlInitiarize();
		//検索条件を表示状態のGroupAreaのレイアウトを保存
		this.saveGrpLayout();
		//予約人数グリッドの設定
		grdYoyakuNinzu.DataSource = new DataTable();

		this.setGrdYoyakuNinzu();
		//バス台数グリッドの設定
		this.setGrdBusDaisu();
	}
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
			this.btnVisiblerCondition.Text = "非表示 <<";

			this.setGrpLayout();
		}
		else
		{
			//非表示状態
			this.btnVisiblerCondition.Text = "表示 >>";
			this.gbxArea1.Height = HeightGbxAreasOnNotVisibleCondition;

			this.gbxArea1.Top = TopGbxCondition;
		}
	}
	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		this.Close();
	}

	/// <summary>
	/// 画面Close時の独自処理(判定付き)
	/// </summary>
	/// <returns></returns>
	protected override bool closingScreen()
	{
		//変更ありの場合には 確認メッセージ表示(FormBase)
		if (F10Key_Enabled && checkChenge() == true)
		{
			return false;
		}
		else
		{
			return true;
		}
	}


	/// <summary>
	/// F8：検索ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnSearch_Click(object sender, EventArgs e)
	{
		btnF8_ClickOrgProc();
	}
	/// <summary>
	/// 条件クリアボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnClear_Click(object sender, EventArgs e)
	{
		this.PT21_Load(new object(), null);
	}
	/// <summary>
	/// F10：登録ボタン押下イベント
	/// </summary>
	protected override void btnF10_ClickOrgProc()
	{
		if (!NinzuYoyakuParam.ContainsKey(paramName.routeCd.ToString()))
		{
			createFactoryMsg.messageDisp("E90_024", "ピックアップルート");
			return;
		}
		//登録処理を行います。よろしいですか？
		if (createFactoryMsg.messageDisp("Q90_001", "登録") == MsgBoxResult.Cancel)
		{
			//キャンセル押下
			return;
		}
		Cursor = Cursors.WaitCursor;
		//削除用パラメータ
		Hashtable deleteParam = new Hashtable();
		List TourokuParamList = new List(Of Hashtable);

		try
		{
			S04_0605_DA da = new S04_0605_DA();
			//変更チェック
			if (!(checkChenge()))
			{
				//更新対象の値が変更されていません。
				createFactoryMsg.messageDisp("E90_049");
				return;
			}
			//重複チェック
			if (!(checkTyoufuku()))
			{
				//車種が重複しています
				createFactoryMsg.messageDisp("E90_045", "車種");
				return;
			}
			//論理チェック
			if (!(checkNinzu()))
			{
				//空席数が不足しています
				createFactoryMsg.messageDisp("E03_004");
				return;
			}
			//削除用パラメータ格納
			//出発日
			deleteParam.Add(paramName.syuptDay.ToString(), SearchParam.Item(paramName.syuptDay.ToString()));
			//ルートコード
			deleteParam.Add(paramName.routeCd.ToString(), NinzuYoyakuParam.Item(paramName.routeCd.ToString()));
			//開始日
			deleteParam.Add(paramName.startDay.ToString(), NinzuYoyakuParam.Item(paramName.startDay.ToString()));
			//登録用パラメータ格納
			int counter = 0;
			foreach (Row row in grdBusDaisu.Rows)
			{
				//車種がないものはパス
				if (counter > 0 && string.IsNullOrEmpty(row.Item(ColListBus.CARCD.ToString()).ToString()) == false)
				{
					//台数が空のときはパス
					if (!(string.IsNullOrEmpty(row.Item(ColListBus.DAISU.ToString()).ToString())))
					{
						//台数が0より大きいとき処理を実行
						if (int.Parse(row.Item(ColListBus.DAISU.ToString()).ToString()) >= 0)
						{
							Hashtable param = new Hashtable();
							//出発日
							param.Add(paramName.syuptDay.ToString(), SearchParam.Item(paramName.syuptDay.ToString()));
							//ルートコード
							param.Add(paramName.routeCd.ToString(), NinzuYoyakuParam.Item(paramName.routeCd.ToString()));
							//開始日
							param.Add(paramName.startDay.ToString(), NinzuYoyakuParam.Item(paramName.startDay.ToString()));
							//行NoについてはDA側で用意するため格納しない
							//車種コード
							param.Add(paramName.carCd.ToString(), row.Item(ColListBus.CARCD.ToString()));
							//台数
							if (row.Item(ColListBus.DAISU.ToString()).ToString() IsNot null)
							{
								param.Add(paramName.daisu.ToString(), row.Item(ColListBus.DAISU.ToString()));
							}
							else
							{
								param.Add(paramName.daisu.ToString(), 0);
							}
							//登録プログラムID
							param.Add(paramName.tourokuPGIMD.ToString(), pgmid);
							//登録ユーザID
							param.Add(paramName.tourokuUserId.ToString(), UserInfoManagement.userId);
							//現在の日時取得
							DateTime Day = System.Convert.ToDateTime(CommonDateUtil.getSystemTime);
							//登録日
							param.Add(paramName.tourokuDay.ToString(), Day);
							//更新プログラムID
							param.Add(paramName.updatePGMID.ToString(), pgmid);
							//更新ユーザID
							param.Add(paramName.updateUserId.ToString(), UserInfoManagement.userId);
							//更新日
							param.Add(paramName.updateDay.ToString(), Day);
							//パラメータリストに追加
							TourokuParamList.Add(param);
						}
					}
				}
				counter++;
			}
			//削除登録実行
			bool result = System.Convert.ToBoolean(da.DeleteAndUpdate(NinzuYoyakuParam, deleteParam, TourokuParamList));
			//チェック
			if (result)
			{
				//登録が完了しました。
				createFactoryMsg.messageDisp("I90_002", "登録");
				//登録後再検索
				btnF8_ClickOrgProc();
			}
			else
			{

				return;
			}
		}
		catch (Exception)
		{
			List errstrList = new List(Of string);
			errstrList.Add(" SearchCondition→");
			foreach (string param in NinzuYoyakuParam.Keys)
			{
				errstrList.Add(NinzuYoyakuParam[param].ToString());
			}
			errstrList.Add(" DeleteParameter→");
			foreach (string param in deleteParam.Keys)
			{
				errstrList.Add(deleteParam[param].ToString());
			}
			errstrList.Add(" RegistrationParameter→");
			foreach (Hashtable param in TourokuParamList)
			{
				foreach (string data in param.Keys)
				{
					errstrList.Add(param(data).ToString());
				}
			}
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.delete, "S04_0605", errstrList.ToArray());
			//エラーが発生しました。
			createFactoryMsg.messageDisp("E90_046", "返却値：False");
			return;
		}
		finally
		{
			Cursor = Cursors.Default;
		}
	}

	/// <summary>
	/// ピックアップ毎のバス台数グリッド表示
	/// </summary>
	/// <param name="tag">予約人数グリッド選択行数</param>
	private void dispBusGrid(int tag)
	{
		if (tag < 0)
		{
			return;
		}
		//カーソルを砂時計にする
		Cursor = Cursors.WaitCursor;

		//台数と参照ボタンを操作可能にする
		grdBusDaisu.Cols(ColListBus.DAISU).AllowEditing = true;
		grdBusDaisu.Cols(ColListBus.SANSYO).AllowEditing = true;

		//登録ボタン活性
		F10Key_Enabled = true;
		try
		{

			//パラメータ初期化
			NinzuYoyakuParam = new Hashtable();
			//パラメータ格納

			//出発日 検索必須のため必ずTrueになるがテスト用にElseパターンを入れてある
			if (dtmSyuptDay.Value IsNot null)
			{
				NinzuYoyakuParam.Add(paramName.syuptDay.ToString(), SearchParam.Item(paramName.syuptDay.ToString()).ToString());
			}
			else
			{
				NinzuYoyakuParam.Add(paramName.syuptDay.ToString(), null);
			}

			//ルートコード
			if (grdYoyakuNinzu.Rows(tag).Item(ColList.ROUTECD.ToString()) IsNot DBNull.Value && grdYoyakuNinzu.Rows(tag).Item(ColList.ROUTECD.ToString()) IsNot null)
			{
				NinzuYoyakuParam.Add(paramName.routeCd.ToString(), grdYoyakuNinzu.Rows(tag).Item(ColList.ROUTECD.ToString()).ToString());
			}
			else
			{
				NinzuYoyakuParam.Add(paramName.routeCd.ToString(), "");
			}
			//開始日
			if (grdYoyakuNinzu.Rows(tag).Item(ColList.STARTDAY.ToString()) IsNot DBNull.Value && grdYoyakuNinzu.Rows(tag).Item(ColList.STARTDAY.ToString()) IsNot null)
			{
				NinzuYoyakuParam.Add(paramName.startDay.ToString(), grdYoyakuNinzu.Rows(tag).Item(ColList.STARTDAY.ToString()));
			}
			else
			{
				NinzuYoyakuParam.Add(paramName.startDay.ToString(), "");
			}
			//タグ（何行目のデータか）必ず存在する
			NinzuYoyakuParam.Add(paramName.tag.ToString(), tag);
			//人数
			if (grdYoyakuNinzu.Rows(tag).Item(ColList.NINZUBUS.ToString()) IsNot DBNull.Value && grdYoyakuNinzu.Rows(tag).Item(ColList.NINZUBUS.ToString()) IsNot null)
			{
				NinzuYoyakuParam.Add(paramName.ninzuRoute.ToString(), System.Convert.ToInt32(grdYoyakuNinzu.Rows(tag).Item(ColList.NINZUBUS.ToString())));
			}
			//バス台数グリッド取得設定
			setBusGridData(tag);
			//合計計算
			sumResult();
		}
		finally
		{
			Cursor = Cursors.Default;
		}
	}


	/// <summary>
	/// 予約人数グリッドチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdYoyakuNinzu_Clicked(object sender, EventArgs e)
	{

		//ピックアップ毎のバス台数グリッド表示
		dispBusGrid(System.Convert.ToInt32(grdYoyakuNinzu.Row));

	}
	/// <summary>
	/// フォームキーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S04_0605_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			//F8ボタン有効化
			btnF8_ClickOrgProc();
		}
	}
	/// <summary>
	/// バス台数グリッド　参照ボタン押下時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdBusDaisu_CellButtonClick(object sender, RowColEventArgs e)
	{
		//子画面起動
		MCarKindEntity ReturnyEntitMCarKind = new MCarKindEntity();
		ReturnyEntitMCarKind.CarCd.Value = grdBusDaisu.Item(grdBusDaisu.Row, ColListBus.CARCD).ToString();

		S90_0110 Form = new S90_0110(ReturnyEntitMCarKind);

		Form.ShowDialog(this);

		if (!ReferenceEquals(Form.getReturnEntity, null))
		{
			ReturnyEntitMCarKind = (MCarKindEntity)Form.getReturnEntity;
			this.grdBusDaisu.SetData(e.Row, ColListBus.CARCD, ReturnyEntitMCarKind.CarCd.Value);
			this.grdBusDaisu.SetData(e.Row, ColListBus.CAPA, ReturnyEntitMCarKind.CarCapacity.Value);
			this.grdBusDaisu.SetData(e.Row, ColListBus.EMGCAPA, ReturnyEntitMCarKind.CarEmgCapacity.Value);

		}
		sumResult();
	}
	/// <summary>
	/// データソースチェンジイベント 合計計算
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdBusDaisu_DataSourceChanged(object sender, EventArgs e)
	{
		sumResult();
	}
	/// <summary>
	/// データ編集後イベント　バス台数グリッド
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdBusDaisu_AfterEdit(object sender, RowColEventArgs e)
	{
		if (!(string.IsNullOrEmpty(grdBusDaisu.Rows(e.Row).Item(ColListBus.DAISU.ToString()).ToString()))
			&& string.IsNullOrEmpty(grdBusDaisu.Rows(e.Row).Item(ColListBus.CARCD.ToString()).ToString()))
		{
			//車種が未設定です。
			createFactoryMsg.messageDisp("E90_044", "車種");
			grdBusDaisu.Rows(e.Row).Item[ColListBus.CARCD.ToString()] = null;
			return;
		}

		if (string.IsNullOrEmpty(grdBusDaisu.Rows(e.Row).Item(ColListBus.DAISU.ToString()).ToString())
			&& string.IsNullOrEmpty(grdBusDaisu.Rows(e.Row).Item(ColListBus.CARCD.ToString()).ToString()) == false)
		{
			//台数をクリアした場合は車種もクリアする
			grdBusDaisu.Rows(e.Row).Item[ColListBus.CARCD.ToString()] = null;
			grdBusDaisu.Rows(e.Row).Item[ColListBus.CAPA.ToString()] = null;
			grdBusDaisu.Rows(e.Row).Item[ColListBus.EMGCAPA.ToString()] = null;
			return;
		}

		if (string.IsNullOrEmpty(grdBusDaisu.Rows(e.Row).Item(ColListBus.DAISU.ToString()).ToString()) == false
			&& System.Convert.ToInt32(grdBusDaisu.Rows(e.Row).Item(ColListBus.DAISU.ToString())) > 99)
		{
			//台数の桁数が不正です。99以内の数字を指定してください
			createFactoryMsg.messageDisp("E70_003", "台数の桁数", "99以内の数字");
			grdBusDaisu.Rows(e.Row).Item[ColListBus.DAISU.ToString()] = null;
			return;
		}
		else
		{
			int sumDaisu = 0;
			int capa = 0;
			int emgcapa = 0;
			for (i = 1; i <= grdBusDaisu.Rows.Count - 1; i++)
			{
				if (!(Information.IsDBNull(grdBusDaisu.Rows(i).Item(ColListBus.DAISU.ToString())))
					&& grdBusDaisu.Rows(i).Item(ColListBus.DAISU.ToString()) IsNot null)
				{
				//台数がNullでなければ
				if (!(string.IsNullOrEmpty(grdBusDaisu.Rows(i).Item(ColListBus.DAISU.ToString()).ToString())))
				{
					//台数が空欄でなければ
					capa += System.Convert.ToInt32(grdBusDaisu.Rows(i).Item(ColListBus.DAISU.ToString())) * System.Convert.ToInt32(grdBusDaisu.Rows(i).Item(ColListBus.CAPA.ToString()));
					emgcapa += System.Convert.ToInt32(grdBusDaisu.Rows(i).Item(ColListBus.DAISU.ToString())) * System.Convert.ToInt32(grdBusDaisu.Rows(i).Item(ColListBus.EMGCAPA.ToString()));
					sumDaisu += System.Convert.ToInt32(grdBusDaisu.Rows(i).Item(ColListBus.DAISU.ToString()));
				}
			}
		}
		grdSum[1, ColListSum.DAISU] = sumDaisu;
		grdSum[1, ColListBus.CAPA] = capa;
		grdSum[1, ColListBus.EMGCAPA] = emgcapa;
		if (e.Row == grdBusDaisu.Rows.Count - 1)
		{
			grdBusDaisu.AddItem("\t" + (int.Parse(grdBusDaisu(e.Row, ColListBus.LINENO.ToString()).ToString()) + 1));
		}
	}
}
#endregion
#region メソッド
#region 予約人数グリッド検索
/// <summary>
/// 検索メソッド
/// </summary>
protected override void btnF8_ClickOrgProc()
{
	//登録ボタン非活性
	this.F10Key_Enabled = false;

	//バス台数グリッドの設定
	this.setGrdBusDaisu();

	setGrdYoyakuNinzu();
	grdYoyakuNinzu.Refresh();
	//カーソルを砂時計に設定
	Cursor = Cursors.WaitCursor;
	try
	{
		//パラメータクリア
		SearchParam.Clear();
		//入力チェック
		if (!(check()))
		{
			//エラー表示はcheckメソッド内で実装
			return;
		}
		//DA用意
		S04_0605_DA da = new S04_0605_DA();
		//検索条件パラメータ格納
		//出発日
		DateTime syuptDay = System.Convert.ToDateTime(this.dtmSyuptDay.Value);
		string syuptDayStr = syuptDay.ToShortDateString().Replace("/", "").Trim();
		SearchParam.Add(paramName.syuptDay.ToString(), syuptDayStr);
		//乗車地コード
		string jousyaTiCd = System.Convert.ToString(this.ucoNoribaCd.CodeText);
		SearchParam.Add(paramName.jousyaTi.ToString(), jousyaTiCd);
		//到着時間From
		string ttyakuTimeFrom = "";
		if (this.dtmTtyakTime.FromTimeValue24Int IsNot null)
			{
			ttyakuTimeFrom = System.Convert.ToString(this.dtmTtyakTime.FromTimeValue24Int.ToString());
		}
		SearchParam.Add(paramName.ttyakuTimeFrom.ToString(), ttyakuTimeFrom);
		//到着時間To
		string ttyakuTimeTo = "";
		if (this.dtmTtyakTime.ToTimeValue24Int IsNot null)
			{
			ttyakuTimeTo = System.Convert.ToString(this.dtmTtyakTime.ToTimeValue24Int.ToString());
		}
		SearchParam.Add(paramName.ttyakuTimeTo.ToString(), ttyakuTimeTo);
		//SQL実行
		DataTable result = da.searchMain(SearchParam);


		//データソース格納
		SearchResult = result;
		this.grdYoyakuNinzu.DataSource = result;

		//検索結果が0件の場合
		if (result.Rows.Count == 0)
		{
			//該当データが存在しません
			createFactoryMsg.messageDisp("E90_019");
			return;
		}
		else
		{
			//ピックアップ毎のバス台数グリッド表示（1行目）
			dispBusGrid(1);
		}

		// 一覧マージ設定
		mergeGrdList();

	}
	finally
	{
		Cursor = Cursors.Default;
	}
}
/// <summary>
/// 入力チェックメソッド
/// </summary>
/// <returns></returns>
private bool check()
{

	//背景色初期化
	base.clearExistErrorProperty(this.gbxCondition.Controls);

	if (CommonUtil.checkHissuError(this.gbxCondition.Controls) == true)
	{
		CommonProcess.createFactoryMsg().messageDisp("E90_022");
		return false;
	}

	if (dtmTtyakTime.ToTimeValue24Int IsNot null && dtmTtyakTime.FromTimeValue24Int IsNot null)
		{
	if (dtmTtyakTime.ToTimeValue24Int < dtmTtyakTime.FromTimeValue24Int)
	{
		//指定期間が間違っています。
		createFactoryMsg.messageDisp("E90_048");
		dtmTtyakTime.ExistErrorForFromTime = true;
		dtmTtyakTime.ExistErrorForToTime = true;
		dtmTtyakTime.Focus();

		return false;
	}
	else
	{
		//入力エラーなし
		return true;
	}
}
		else
{
	return true;
}
	}
	
#endregion
#region バス台数グリッド検索
	private void setBusGridData(int rowNumber)
{
	//DA準備
	S04_0605_DA da = new S04_0605_DA();
	//検索条件格納
	Hashtable param = new Hashtable();
	//出発日　NULL禁止のためNullチェック不要
	DateTime syuptDay = System.Convert.ToDateTime(dtmSyuptDay.Value);
	string syuptDayStr = syuptDay.ToShortDateString().Replace("/", "").Trim();
	param.Add(paramName.syuptDay.ToString(), syuptDayStr);
	//ルートコード
	string routeCd = grdYoyakuNinzu.Rows(rowNumber).Item(ColList.ROUTECD.ToString()).ToString();
	param.Add(paramName.routeCd.ToString(), routeCd);
	//開始日
	DateTime startDay = System.Convert.ToDateTime(grdYoyakuNinzu.Rows(rowNumber).Item(ColList.STARTDAY.ToString()));
	param.Add(paramName.startDay.ToString(), startDay);
	//検索実行
	DataTable result = da.searchBus(param);
	//データ編集
	editDataBus(result);
	//合計計算
	sumResult();
}
#region データ編集
/// <summary>
/// データ編集メソッド　バス台数グリッド
/// </summary>
/// <param name="dt"></param>
/// <returns></returns>
private DataTable editDataBus(DataTable dt)
{
	//返却用データテーブル準備
	DataTable result = new DataTable();
	result.Columns.Add(ColListBus.LINENO.ToString(), typeof(int));
	result.Columns.Add(ColListBus.CARCD.ToString(), typeof(string));
	result.Columns.Add(ColListBus.SANSYO.ToString(), typeof(object));
	result.Columns.Add(ColListBus.CAPA.ToString(), typeof(int));
	result.Columns.Add(ColListBus.EMGCAPA.ToString(), typeof(int));
	result.Columns.Add(ColListBus.DAISU.ToString(), typeof(int));
	result.Columns.Add(ColListBus.DAISUSUB.ToString(), typeof(int));
	//カウンタ
	int counter = 0;
	foreach (DataRow row in dt.Rows)
	{
		DataRow newRow = result.NewRow;
		counter++;
		//行ナンバー
		newRow[ColListBus.LINENO.ToString()] = row.Item(ColListBus.LINENO.ToString());
		//車種コード
		newRow[ColListBus.CARCD.ToString()] = row.Item(ColListBus.CARCD.ToString());
		//定員（定）
		newRow[ColListBus.CAPA.ToString()] = row.Item(ColListBus.CAPA.ToString());
		//定員計（補・階）
		newRow[ColListBus.EMGCAPA.ToString()] = row.Item(ColListBus.EMGCAPA.ToString());
		//台数
		newRow[ColListBus.DAISU.ToString()] = row.Item(ColListBus.DAISU.ToString());
		//台数サブ
		newRow[ColListBus.DAISUSUB.ToString()] = row.Item(ColListBus.DAISU.ToString());
		//行追加
		result.Rows.Add(newRow);
	}
	if (counter == 0)
	{
		makeBusTable();
	}
	else
	{
		if (counter < 5)
		{
			for (i = counter; i <= 4; i++)
			{
				DataRow newRow = result.NewRow;
				//行ナンバー
				newRow[ColListBus.LINENO.ToString()] = i + 1;
				//行追加
				result.Rows.Add(newRow);
			}
		}
		//データテーブル代入
		grdBusDaisu.DataSource = new DataTable();
		grdBusDaisu.DataSource = result;
	}

	//差分検出用
	searchResultBusDaisuOld = result.Copy;

	//リターン
	return result;
}
#endregion
#endregion
#region 合計グリッド
/// <summary>
/// 合計計算
/// </summary>
private void sumResult()
{
	//合計欄計算
	if (grdBusDaisu.Rows.Count > 1)
	{
		int capa = 0;
		int emgcapa = 0;
		int daisu = 0;
		int counter = 0;
		foreach (Row row in grdBusDaisu.Rows)
		{
			if (counter > 0)
			{
				if (!(string.IsNullOrEmpty(row.Item(ColListBus.CAPA.ToString()).ToString()) || string.IsNullOrEmpty(row.Item(ColListBus.DAISU.ToString()).ToString())))
				{
					capa += int.Parse(row.Item(ColListBus.CAPA.ToString()).ToString()) * int.Parse(row.Item(ColListBus.DAISU.ToString()).ToString());
				}
				if (!(string.IsNullOrEmpty(row.Item(ColListBus.EMGCAPA.ToString()).ToString()) || string.IsNullOrEmpty(row.Item(ColListBus.DAISU.ToString()).ToString())))
				{
					emgcapa += int.Parse(row.Item(ColListBus.EMGCAPA.ToString()).ToString()) * int.Parse(row.Item(ColListBus.DAISU.ToString()).ToString());
				}
				if (!(string.IsNullOrEmpty(row.Item(ColListBus.DAISU.ToString()).ToString())))
				{
					daisu += int.Parse(row.Item(ColListBus.DAISU.ToString()).ToString());
				}
			}
			counter++;
		}
		grdSum[1, ColListSum.CAPA] = capa;
		grdSum[1, ColListSum.EMGCAPA] = emgcapa;
		grdSum[1, ColListSum.DAISU] = daisu;
	}
	else
	{
		grdSum[1, ColListSum.CAPA] = 0;
		grdSum[1, ColListSum.EMGCAPA] = 0;
		grdSum[1, ColListSum.DAISU] = 0;
	}
}
#endregion
#region 登録
#region チェック
/// <summary>
/// 変更チェック
/// </summary>
/// <returns></returns>
private bool checkChenge()
{
	int counter = 0;
	bool result = false;
	foreach (Row row in grdBusDaisu.Rows)
	{
		if (counter > 0)
		{
			if (!(row.Item(ColListBus.DAISU.ToString()).Equals(row.Item(ColListBus.DAISUSUB.ToString()))))
			{
				//台数の初期値と変化がある場合
				result = true;
			}

			//変更前データより件数が多い場合は車種が入力されていれば差分ありとみなす
			if (counter > searchResultBusDaisuOld.Rows.Count)
			{
				if (Information.IsDBNull(grdBusDaisu.Rows(counter)[ColListBus.CARCD]) == false)
				{
					return true;
				}
			}
			else
			{
				//(変更前後の車種が共にNULL　もしくは　変更前後の車種が同一)　以外の場合、に差分あり
				if (!(Information.IsDBNull(grdBusDaisu.Rows(counter)[ColListBus.CARCD]) == false &&)
					{
					Information.IsDBNull(searchResultBusDaisuOld.Rows(counter - 1)[ColListBus.CARCD.ToString()]) = System.Convert.ToBoolean(false &&);
					System.Convert.ToString(grdBusDaisu.Rows(counter)[ColListBus.CARCD]) = System.Convert.ToString(System.Convert.ToString(searchResultBusDaisuOld.Rows(counter - 1)[ColListBus.CARCD.ToString()])
						||);
					Information.IsDBNull(grdBusDaisu.Rows(counter)[ColListBus.CARCD]) = System.Convert.ToBoolean(true &&);
					Information.IsDBNull(searchResultBusDaisuOld.Rows(counter - 1)[ColListBus.CARCD.ToString()]) = System.Convert.ToBoolean(true));

return true;
					}
				}
			}
			counter++;
		}
		return result;
	}
	/// <summary>
	/// 登録時重複チェックメソッド
	/// </summary>
	/// <returns></returns>
	private bool checkTyoufuku()
{
	int counterBase = 0;
	foreach (Row row in grdBusDaisu.Rows)
	{
		if (counterBase > 0 && row.Item(ColListBus.CARCD.ToString()) IsNot DBNull.Value && row.Item(ColListBus.CARCD.ToString()) IsNot null)
			{
	//車種に何かしら入っているとき
	string pk = row.Item(ColListBus.CARCD.ToString()).ToString();
	if (!(string.IsNullOrEmpty(pk)))
	{
		//車種が空欄でないとき
		int counter = 0;
		foreach (Row row2 in grdBusDaisu.Rows)
		{
			if (counter > counterBase)
			{
				//検索対象の行よりカウンターが大きいとき
				if (pk.Equals(row2.Item(ColListBus.CARCD.ToString()).ToString()))
				{
					//何かイコールのものがあるときFalseを返す
					return false;
				}
			}
			counter++;
		}
	}
}
counterBase++;
		}
		return true;
	}
	/// <summary>
	/// 人数チェック
	/// </summary>
	/// <returns></returns>
	private bool checkNinzu()
{
	int yoyakuNinzu = System.Convert.ToInt32(NinzuYoyakuParam[paramName.ninzuRoute.ToString()]);
	int teiin = System.Convert.ToInt32(grdSum.Rows(1).Item(ColListSum.CAPA));
	if (teiin < yoyakuNinzu)
	{
		return false;
	}
	else
	{
		return true;
	}
}
#endregion
#endregion
#region 初期化
#region コントロール
/// <summary>
/// 画面初期化
/// </summary>
private void setControlInitiarize()
{

	// フッタボタンの設定
	this.setButtonInitiarize();
	//検索条件の設定
	setSearchKoumoku();

	//背景色初期化
	base.clearExistErrorProperty(this.gbxCondition.Controls);

	//セル結合可
	this.grdYoyakuNinzu.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;

	//初期フォーカス
	this.dtmSyuptDay.Focus();
}
/// <summary>
/// フッタボタンの設定
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
	this.F10Key_Visible = true;
	this.F11Key_Visible = false;
	this.F12Key_Visible = false;

	//Text
	this.F2Key_Text = "F2:戻る";
	this.F10Key_Text = "F10:登録";

	this.F10Key_AllowAuth = FixedCd.AuthLevel.add;

	//初期状態は登録ボタンは非活性
	this.F10Key_Enabled = false;
}
#endregion
#region グループボックス
/// <summary>
/// GroupBoxのレイアウト保存
/// </summary>
private void saveGrpLayout()
{
	this.gbxCondition.Height = this.gbxCondition.Height;
	this._heightGbxArea1 = System.Convert.ToInt32(this.gbxArea1.Height);
	this._heightGbxArea2 = System.Convert.ToInt32(this.gbxArea2.Height);
	this._topGbxArea1 = System.Convert.ToInt32(this.gbxArea1.Top);
	this._topGbxArea2 = System.Convert.ToInt32(this.gbxArea2.Top);
}

/// <summary>
/// GroupBoxのレイアウト設定
/// </summary>
private void setGrpLayout()
{
	this.gbxCondition.Height = this.gbxCondition.Height;
	this.gbxArea1.Height = this._heightGbxArea1;
	this.gbxArea2.Height = this._heightGbxArea2;
	this.gbxArea1.Top = this._topGbxArea1;
	this.gbxArea2.Top = this._topGbxArea2;
}
#endregion
#region 検索条件の設定
//検索項目の設定
private void setSearchKoumoku()
{
	this.ucoNoribaCd.CodeText = null;
	this.ucoNoribaCd.ValueText = null;
	//モック上に存在するが、DB上に存在しないため非表示
	this.chkCrsKindGaikokugo.Visible = false;
	this.chkCrsKindJapanese.Visible = false;
	this.lblCrsKind.Visible = false;
}
#endregion
#region 予約人数グリッドの設定
/// <summary>
/// 予約人数グリッドの設定
/// </summary>
private void setGrdYoyakuNinzu()
{


	// 行の高さ設定
	this.grdYoyakuNinzu.Rows(0).Height = 37;
	// 人数計(ルート)
	CellRange range1 = this.grdYoyakuNinzu.GetCellRange(0, ColList.CAPA);
	range1.Data = "定員計" + "\r\n" + "(定)";
	// 人数計(ホテル)
	CellRange range2 = this.grdYoyakuNinzu.GetCellRange(0, ColList.EMGCAPA);
	range2.Data = "定員計" + "\r\n" + "(補・1階)";
	// 人数計(ホテル)
	CellRange range3 = this.grdYoyakuNinzu.GetCellRange(0, ColList.NINZUBUS);
	range3.Data = "人数計" + "\r\n" + "(ルート)";
	// 人数計(ホテル)
	CellRange range4 = this.grdYoyakuNinzu.GetCellRange(0, ColList.NINZUHOTEL);
	range4.Data = "人数計" + "\r\n" + "(ホテル)";

}

#endregion
#region バス台数グリッドの設定
/// <summary>
/// バス台数グリッドの設定
/// </summary>
private void setGrdBusDaisu()
{
	//グリッドの設定
	for (i = 1; i <= grdBusDaisu.Rows.Count - 1; i++)
	{
		grdBusDaisu[i, ColListBus.SANSYO] = "...";
	}
	object with_1 = this.grdBusDaisu;
	//ドラッグ禁止
	with_1.AllowDragging = AllowDraggingEnum.None;
	//列自動生成禁止
	with_1.AutoGenerateColumns = false;
	//表示列数
	with_1.Cols.Count = 8;
	with_1.Cols.Fixed = 1;
	//スクロールバーの設定
	with_1.ScrollOptions = ScrollFlags.AlwaysVisible;
	with_1.ScrollBars = ScrollBars.Vertical;
	//デフォルトの行の高さ設定
	with_1.Rows.DefaultSize = 23;
	//マージ設定
	with_1.AllowMerging = AllowMergingEnum.FixedOnly;
	//ソート禁止
	with_1.AllowSorting = AllowSortingEnum.None;
	//ヘッダ
	object with_2 = with_1.Rows(0);
	with_2.Height = 33;
	with_2.AllowMerging = true;
	CellRange merge = this.grdBusDaisu.GetCellRange(0, ColListBus.CARCD, 0, ColListBus.SANSYO);
	merge.Data = "車種";
	grdBusDaisu.MergedRanges.Add(merge);
	//行NO
	object with_3 = with_1.Cols(ColListBus.LINENO);
	with_3.Caption = "NO";
	with_3.Name = ColListBus.LINENO.ToString();
	with_3.Width = 23;
	with_3.DataType = typeof(int);
	with_3.AllowEditing = false;
	with_3.AllowResizing = false;
	with_3.TextAlign = TextAlignEnum.RightCenter;
	with_3.TextAlignFixed = TextAlignEnum.CenterCenter;
	with_3.Style.BackColor = SystemColors.ControlLight;
	//車種コード
	object with_4 = with_1.Cols(ColListBus.CARCD);
	with_4.Caption = "車種";
	with_4.Name = ColListBus.CARCD.ToString();
	with_4.Width = 39;
	with_4.DataType = typeof(string);
	with_4.AllowEditing = false;
	with_4.AllowResizing = false;
	with_4.TextAlign = TextAlignEnum.LeftCenter;
	with_4.TextAlignFixed = TextAlignEnum.CenterCenter;
	with_4.Style.BackColor = SystemColors.ControlLight;
	//参照ボタン
	object with_5 = with_1.Cols(ColListBus.SANSYO);
	with_5.Caption = "参" + "\r\n" + "照";
	with_5.Name = ColListBus.SANSYO.ToString();
	with_5.Width = 23;
	with_5.AllowResizing = false;
	with_5.ComboList = "...";
	with_5.ShowButtons = ShowButtonsEnum.Always;
	with_5.ImageAlign = ImageAlignEnum.CenterCenter;
	with_5.ImageAlignFixed = ImageAlignEnum.CenterCenter;
	with_5.TextAlign = TextAlignEnum.CenterCenter;
	with_5.TextAlignFixed = TextAlignEnum.CenterCenter;
	with_5.Style.BackColor = SystemColors.ControlLight;
	with_5.AllowEditing = false;
	//定員（定）
	object with_6 = with_1.Cols(ColListBus.CAPA);
	with_6.Caption = "定員計" + "\r\n" + "(定)";
	with_6.Name = ColListBus.CAPA.ToString();
	with_6.Width = 80;
	with_6.DataType = typeof(int);
	with_6.AllowEditing = false;
	with_6.AllowResizing = false;
	with_6.TextAlign = TextAlignEnum.RightCenter;
	with_6.TextAlignFixed = TextAlignEnum.CenterCenter;
	with_6.Style.BackColor = SystemColors.ControlLight;
	//定員（補・1階）
	object with_7 = with_1.Cols(ColListBus.EMGCAPA);
	with_7.Caption = "定員計" + "\r\n" + "(補・1階)";
	with_7.Name = ColListBus.EMGCAPA.ToString();
	with_7.Width = 80;
	with_7.DataType = typeof(int);
	with_7.AllowEditing = false;
	with_7.AllowResizing = false;
	with_7.TextAlign = TextAlignEnum.RightCenter;
	with_7.TextAlignFixed = TextAlignEnum.CenterCenter;
	with_7.Style.BackColor = SystemColors.ControlLight;
	//台数
	object with_8 = with_1.Cols(ColListBus.DAISU);
	with_8.Caption = "台数";
	with_8.Name = ColListBus.DAISU.ToString();
	with_8.Width = 80;
	with_8.DataType = typeof(int);
	with_8.AllowResizing = false;
	with_8.TextAlign = TextAlignEnum.RightCenter;
	with_8.TextAlignFixed = TextAlignEnum.CenterCenter;
	with_8.AllowEditing = false;
	//台数サブ
	object with_9 = with_1.Cols(ColListBus.DAISUSUB);
	with_9.Name = ColListBus.DAISUSUB.ToString();
	with_9.Visible = false;
	with_9.DataType = typeof(int);
	//合計欄の設定
	object with_10 = this.grdSum;
	//ドラッグ禁止
	with_10.AllowDragging = AllowDraggingEnum.None;
	//列自動生成禁止
	with_10.AutoGenerateColumns = false;
	//選択スタイル
	with_10.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
	with_10.HighLight = C1.Win.C1FlexGrid.HighLightEnum.WithFocus;
	with_10.DrawMode = C1.Win.C1FlexGrid.DrawModeEnum.OwnerDraw;
	//表示列数
	with_10.Cols.Count = 7;
	with_10.Cols.Fixed = 1;
	//表示行数
	with_10.Rows.Count = 2;
	//編集不可
	with_10.AllowEditing = false;
	//ヘッダ非表示
	with_10.Rows(0).Height = 0;
	//マージ設定
	with_10.AllowMerging = AllowMergingEnum.Custom;
	//スクロールバーの設定
	with_10.ScrollOptions = ScrollFlags.AlwaysVisible;
	with_10.ScrollBars = ScrollBars.Vertical;
	//各列設定
	object with_11 = with_10.Cols(ColListBus.LINENO);
	with_11.Width = this.grdBusDaisu.Cols(ColListBus.LINENO.ToString()).Width;
	with_11.TextAlign = TextAlignEnum.CenterCenter;
	with_11.Style.BackColor = SystemColors.ControlLight;
	object with_12 = with_10.Cols(ColListBus.CARCD);
	with_12.Width = this.grdBusDaisu.Cols(ColListBus.CARCD.ToString()).Width;
	with_12.TextAlign = TextAlignEnum.CenterCenter;
	with_12.Style.BackColor = SystemColors.ControlLight;
	object with_13 = with_10.Cols(ColListBus.SANSYO);
	with_13.Width = this.grdBusDaisu.Cols(ColListBus.SANSYO.ToString()).Width;
	with_13.TextAlign = TextAlignEnum.CenterCenter;
	with_13.Style.BackColor = SystemColors.ControlLight;
	object with_14 = with_10.Cols(ColListSum.CAPA);
	with_14.Name = ColListSum.CAPA.ToString();
	with_14.Width = this.grdBusDaisu.Cols(ColListSum.CAPA.ToString()).Width;
	with_14.TextAlign = TextAlignEnum.RightCenter;
	with_14.Style.BackColor = SystemColors.ControlLight;
	with_14.Format = "N0";
	object with_15 = with_10.Cols(ColListSum.EMGCAPA);
	with_15.Name = ColListSum.EMGCAPA.ToString();
	with_15.Width = this.grdBusDaisu.Cols(ColListSum.EMGCAPA.ToString()).Width;
	with_15.TextAlign = TextAlignEnum.RightCenter;
	with_15.Style.BackColor = SystemColors.ControlLight;
	with_15.Format = "N0";
	object with_16 = with_10.Cols(ColListSum.DAISU);
	with_16.Name = ColListSum.DAISU.ToString();
	with_16.Width = this.grdBusDaisu.Cols(ColListSum.DAISU.ToString()).Width;
	with_16.TextAlign = TextAlignEnum.RightCenter;
	with_16.DataType = typeof(int);
	with_16.Style.BackColor = SystemColors.ControlLight;
	with_16.Format = "N0";

	//合計欄のマージ
	CellRange sum = null;
	sum = grdSum.GetCellRange(1, 1, 1, 3);
	sum.Data = "合計";
	grdSum.MergedRanges.Add(sum);
	//初期データテーブル作成
	makeBusTable();
}

/// <summary>
/// バス台数の初期テーブル
/// </summary>
private void makeBusTable()
{
	//データソース用データテーブル
	DataTable dt = new DataTable();
	//列作成
	dt.Columns.Add(ColListBus.LINENO.ToString());
	dt.Columns.Add(ColListBus.CARCD.ToString());
	dt.Columns.Add(ColListBus.SANSYO.ToString());
	dt.Columns.Add(ColListBus.CAPA.ToString());
	dt.Columns.Add(ColListBus.EMGCAPA.ToString());
	dt.Columns.Add(ColListBus.DAISU.ToString());
	dt.Columns.Add(ColListBus.DAISUSUB.ToString());
	for (i = 1; i <= 5; i++)
	{
		object dr = dt.NewRow;
		dr[ColListBus.LINENO.ToString()] = i;
		dt.Rows.Add(dr);
	}
	//データソース登録
	BusResult = dt;
	grdBusDaisu.DataSource = new DataTable();
	grdBusDaisu.DataSource = dt;
}

#endregion
#endregion
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
	grdYoyakuNinzu.MergedRanges.Clear();

	for (int col1 = ColList.ROUTECD; col1 <= grdYoyakuNinzu.Cols.Count - 1; col1++)
	{

		topRow = 1;
		bottomRow = 1;

		for (int row1 = 1; row1 <= grdYoyakuNinzu.Rows.Count - 1; row1++)
		{

			// 最終行
			if (row1.Equals(grdYoyakuNinzu.Rows.Count - 1))
			{
				cr = grdYoyakuNinzu.GetCellRange(topRow, col1, row1, col1);
				grdYoyakuNinzu.MergedRanges.Add(cr);

				break;
			}

			// マージチェック（次行データと同じか）
			if (checkRowdata(row1, col1) == false)
			{

				// 次行データと異なるため、マージする
				cr = grdYoyakuNinzu.GetCellRange(topRow, col1, row1, col1);
				grdYoyakuNinzu.MergedRanges.Add(cr);

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
	for (int col1 = ColList.ROUTECD; col1 <= nowCol; col1++)
	{
		// 次行データと同じか
		if (ReferenceEquals(grdYoyakuNinzu(nowRow, col1), null) == false && ReferenceEquals(grdYoyakuNinzu(nowRow + 1, col1), null) == false)
		{
			if (!(grdYoyakuNinzu(nowRow, col1).ToString().TrimEnd.Equals(grdYoyakuNinzu(nowRow + 1, col1).ToString().TrimEnd)))
			{
				// 異なる
				return false;
			}
		}
	}

	return true;
}
#endregion
}