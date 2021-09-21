using System.ComponentModel;


/// <summary>
/// 払戻一覧照会
/// </summary>
public class S02_0701 : PT11, iPT11
{
	//Inherits FormBase

	#region 定数

	/// <summary>
	/// 画面ID
	/// </summary>
	private const string ScreenId = "S02_0701";
	/// <summary>
	/// 画面名
	/// </summary>
	private const string ScreenName = "払戻一覧照会";
	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	public const int TopGbxCondition = 41;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	public const int MarginGbxCondition = 6;
	/// <summary>
	/// 列番号 予約区分（非表示）
	/// </summary>
	private const int NoColYoyakuKbn = 11;
	/// <summary>
	/// 列番号 予約NO（非表示）
	/// </summary>
	private const int NoColYoyakuNo = 12;
	/// <summary>
	/// 列番号 発券状態(非表示)
	/// </summary>
	private const int NoColHakkenState = 13;
	/// <summary>
	/// 列番号 座席指定予約フラグ(非表示)
	/// </summary>
	private const int NoColZasekiReserveYoyakuFlg = 14;
	/// <summary>
	/// コース区分(種別)(日本語:1)
	/// </summary>
	private const int Japanese = 1;
	/// <summary>
	/// コース区分(種別)(外国語:2)
	/// </summary>
	private const int Gaikokugo = 2;
	/// <summary>
	/// コース区分(種別)(未選択)
	/// </summary>
	private const int CrsNoSelect = 3;
	/// <summary>
	/// チェックボックスの定期（昼）
	/// </summary>
	private const int TeikiNoon = 1;
	/// <summary>
	/// チェックボックスの定期（夜）
	/// </summary>
	private const int TeikiNight = 2;
	/// <summary>
	/// チェックボックスの企画
	/// </summary>
	private const int Kikaku = 3;
	/// <summary>
	/// キャンセルフラグ状態１
	/// </summary>
	private const string CancelFlgState1 = "1";
	/// <summary>
	/// キャンセルフラグ状態２
	/// </summary>
	private const string CancelFlgState2 = "2";
	/// <summary>
	/// 紛失フラグ状態
	/// </summary>
	private const string LostFlgState = "Y";
	/// <summary>
	/// 検索結果最大表示件数件数
	/// </summary>
	public const int MaxKensu = 100;
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
	/// 画面初期化(ロード時イベント)
	/// </summary>
	protected override void StartupOrgProc()
	{

		// 画面ID/名称の設定
		base.setFormId = ScreenId;
		base.setTitle = ScreenName;

		// 画面終了時確認不要
		base.closeFormFlg = false;

		// フッタボタンの設定
		this.setButtonInitiarize();

		// 画面項目初期化
		setConditionInitiarize();

		//grid初期表示
		setInitiarizeGrid();

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
		if (this.VisibleGbxCondition)
		{
			//表示状態
			this.btnVisiblerCondition.Text = "非表示 <<";

			this.PanelEx1.Top = TopGbxCondition + this.HeightGbxCondition + MarginGbxCondition;
			this.PanelEx1.Height -= this.HeightGbxCondition + MarginGbxCondition;
			this.grdModosiListInquiry.Height -= this.HeightGbxCondition - 3;

		}
		else
		{
			//非表示状態
			this.btnVisiblerCondition.Text = "表示 >>";

			this.PanelEx1.Top = TopGbxCondition;
			this.PanelEx1.Height += this.HeightGbxCondition + MarginGbxCondition;
			this.grdModosiListInquiry.Height += this.HeightGbxCondition - 3;
		}
	}

	/// <summary>
	/// クリアボタンイベント
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
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnSearch_Click(object sender, EventArgs e)
	{

		// F8ボタン押下
		base.btnCom_Click(this.btnSearch, e);

		// log出力
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理");

	}

	/// <summary>
	/// キーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void S02_0701_KeyDown(object sender, KeyEventArgs e)
	{

		if (e.KeyData == Keys.F8)
		{
			e.Handled = true;
			this.btnSearch_Click(sender, e);
		}
		else
		{
			return;
		}

	}

	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		//戻る
		closeCheckFlg = true;
		base.closeFormFlg = false;
		this.Close();
	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void FlexGridEx1_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		//データ件数を表示(ヘッダー行分マイナス1)
		string formatedCount = System.Convert.ToString((this.grdModosiListInquiry.Rows.Count - 1).ToString().PadLeft(6));
		this.lblLengthGrd.Text = formatedCount + "件";
	}

	/// <summary>
	/// 選択列ボタンの押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void modosiList_CellButtonClick()
	{
		)grdModosiListInquiry.CellButtonClick;

		S02_0701Da da = new S02_0701Da();
		FlexGridEx grd = TryCast(sender, FlexGridEx);

		if (grd.Row <= 0)
		{
			return;
		}

		//押下行の予約区分、予約NO取得  (gridに未表示行を作成して予約NO,予約区分,発券状態を保持)
		string yoyakuKbn = System.Convert.ToString(TryCast(grd.GetData(e.Row, NoColYoyakuKbn), string));
		if (ReferenceEquals(grd.GetData(e.Row, NoColYoyakuNo), null))
		{
			return;
		}
		int yoyakuNo = int.Parse(System.Convert.ToString(grd.GetData(e.Row, NoColYoyakuNo).ToString()));
		string hakkenState = System.Convert.ToString(TryCast(grd.GetData(e.Row, NoColHakkenState), string));

		// 画面表示判定と呼出の実行
		showS02_0703(yoyakuKbn, yoyakuNo);


	}

	#endregion

	#region メソッド

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
		this.F10Key_Visible = false;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;

		//Text
		//Me.F1Key_Text = "F1:"
		this.F2Key_Text = "F2:戻る";
		//Me.F3Key_Text = "F3:終了"
		//Me.F4Key_Text = "F4:削除"
		//Me.F5Key_Text = "F5:参照"
		//Me.F6Key_Text = "F6:ﾌﾟﾚﾋﾞｭｰ"
		//Me.F7Key_Text = "F7:印刷"
		//Me.F8Key_Text = "F8:検索"
		//Me.F9Key_Text = "F9:CSV出力"
		//Me.F10Key_Text = "F10:登録"
		//Me.F11Key_Text = "F11:更新"
		//Me.F12Key_Text = "F12:"

		//Me.F1Key_Enabled = False
		//Me.F2Key_Enabled = False
		//Me.F3Key_Enabled = False
		//Me.F4Key_Enabled = False
		//Me.F5Key_Enabled = False
		//Me.F6Key_Enabled = False
		//Me.F7Key_Enabled = False
		//Me.F8Key_Enabled = False
		//Me.F9Key_Enabled = False
		//Me.F10Key_Enabled = False
		//Me.F11Key_Enabled = False
		//Me.F12Key_Enabled = False
	}

	/// <summary>
	/// グリッド初期化
	/// </summary>
	private void setInitiarizeGrid()
	{

		//(Nothingで初期化するとヘッダー名が消える)
		DataTable dt = new DataTable();
		this.grdModosiListInquiry.DataSource = dt;
		this.grdModosiListInquiry.DataMember = "";
		this.grdModosiListInquiry.Refresh();

	}

	/// <summary>
	/// 検索条件初期化
	/// </summary>
	private void setConditionInitiarize()
	{

		this.ucoYoyakuNo.YoyakuText = "";
		this.ucoYoyakuNo.ExistError = false;

		this.chkTeikiNoon.Checked = false;
		this.chkTeikiNoon.ExistError = false;

		this.chkTeikiNight.Checked = false;
		this.chkTeikiNight.ExistError = false;

		this.chkKikaku.Checked = false;
		this.chkKikaku.ExistError = false;

		this.txtSurname.Text = "";
		this.txtSurname.ExistError = false;

		this.txtName.Text = "";
		this.txtName.ExistError = false;

		// 検索項目「日付」にシステム日付初期表示
		this.dtmSyuptDay.Value = CommonDateUtil.getSystemTime();
		this.dtmSyuptDay.ExistError = false;

		//日本語／外国語コース
		if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
		{
			//ユーザーが国際事業部の場合は外国語
			this.chkGaikokugo.Checked = true;
			this.chkJapanese.Checked = false;
		}
		else
		{
			//それ以外の場合は日本語をONに設定
			this.chkJapanese.Checked = true;
			this.chkGaikokugo.Checked = false;
		}

		// グリッド初期化
		this.setInitiarizeGrid();

	}


	/// <summary>
	/// 払戻一覧照会グリッドの設定
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{

		//MyBase.btnF8_ClickOrgProc()

		DataTable dtNo1 = new DataTable(); //IO項目定義No.1取得結果保持用
		DataTable dtNo2 = new DataTable(); //IO項目定義No.2取得結果保持用
		DataTable dtNo3 = new DataTable(); //IO項目定義No.3取得結果保持用
		int rowNum = 0;

		// エラー情報の初期化
		this.ucoYoyakuNo.ExistError = false;
		this.chkTeikiNoon.ExistError = false;
		this.chkTeikiNight.ExistError = false;
		this.chkKikaku.ExistError = false;
		this.txtSurname.ExistError = false;
		this.txtName.ExistError = false;
		this.dtmSyuptDay.ExistError = false;

		DateTime tempSyuptDay = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		string syuptDay = string.Empty;

		string yoyakuKbn = string.Empty;
		string yoyakuNo = string.Empty;

		// 出発日指定ありの場合
		if (ReferenceEquals(dtmSyuptDay.Value, null))
		{
			// 必須
			CommonProcess.createFactoryMsg().messageDisp("E90_023", "出発日");
			this.dtmSyuptDay.ExistError = true;
			this.ActiveControl = this.dtmSyuptDay;
			return;
		}
		else
		{
			// 値を格納
			tempSyuptDay = System.Convert.ToDateTime(dtmSyuptDay.Value);
			syuptDay = tempSyuptDay.ToString("yyyyMMdd"); //画面項目「出発日」
		}

		//画面項目「コース区分(種別)(日本語:1,外国語:2)」
		string crsKbn = ""; //画面項目「コース区分(日本語:1,外国語:2)」
		if (chkJapanese.Checked && !(chkGaikokugo.Checked))
		{
			crsKbn = System.Convert.ToString(Japanese);
		}
		else if (chkGaikokugo.Checked && !(chkJapanese.Checked))
		{
			crsKbn = System.Convert.ToString(Gaikokugo);
		}
		else
		{
			crsKbn = System.Convert.ToString(CrsNoSelect);
		}

		string crsKind = string.Empty; //画面項目「コース種別(定期（昼）/定期（夜）/企画)」
		if (string.IsNullOrEmpty(System.Convert.ToString(ucoYoyakuNo.YoyakuText)) == true)
		{
			// 予約番号が指定された場合、コース種別はいずれか必須
			if (chkTeikiNoon.Checked == false && chkTeikiNight.Checked == false && chkKikaku.Checked == false)
			{
				//予約番号が選択されていない場合、コース種別必須
				CommonProcess.createFactoryMsg().messageDisp("E02_004");
				chkTeikiNoon.ExistError = true;
				chkTeikiNight.ExistError = true;
				chkKikaku.ExistError = true;
				this.ActiveControl = this.chkTeikiNoon;
				return;
			}

			if ((chkTeikiNoon.Checked == true && chkTeikiNight.Checked == false && chkKikaku.Checked == false) ||)
			{
				(chkTeikiNoon.Checked == false && chkTeikiNight.Checked == true && chkKikaku.Checked == false) ||;
				(chkTeikiNoon.Checked == false && chkTeikiNight.Checked == false && chkKikaku.Checked == true);
				//コース種別(定期（昼）/定期（夜）/企画)の設定
				if (chkTeikiNoon.Checked == true)
				{
					crsKind = System.Convert.ToString(TeikiNoon);
				}
				else if (chkTeikiNight.Checked == true)
				{
					crsKind = System.Convert.ToString(TeikiNight);
				}
				else
				{
					crsKind = System.Convert.ToString(Kikaku);
				}
			}
			else
			{
				// 複数チェックあり
				//コース種別のいずれか１つを選択してください
				CommonProcess.createFactoryMsg().messageDisp("E02_001");
				if (chkKikaku.Checked == true)
				{
					chkKikaku.ExistError = true;
					this.ActiveControl = this.chkKikaku;
				}
				if (chkTeikiNight.Checked == true)
				{
					chkTeikiNight.ExistError = true;
					this.ActiveControl = this.chkTeikiNight;
				}
				if (chkTeikiNoon.Checked == true)
				{
					chkTeikiNoon.ExistError = true;
					this.ActiveControl = this.chkTeikiNoon;
				}
				return;
			}
		}
		else
		{
			// 予約番号指定された場合値を格納
			if (!string.IsNullOrEmpty(System.Convert.ToString(ucoYoyakuNo.YoyakuText)))
			{
				yoyakuKbn = System.Convert.ToString(ucoYoyakuNo.YoyakuText.Substring(0, 1));
				yoyakuNo = System.Convert.ToString(ucoYoyakuNo.YoyakuText.Substring(1, ucoYoyakuNo.YoyakuText.Length - 1));
			}
		}

		Hashtable paramInfoList = new Hashtable();
		{ "syuptDay", syuptDay}, // 出発日; // 出発日
			{ "crsKbn", crsKbn}, // コース区分; // コース区分
				{ "crsKind", crsKind}, // コース種別; // コース種別
					{ "surname", txtSurname.Text}, // 姓; // 姓
						{ "name", txtName.Text}, // 名; // 名
							{ "yoyakuKbn", yoyakuKbn}, // 予約区分; // 予約区分
								{ "yoyakuNo", yoyakuNo}, // 予約番号; // 予約番号
									{ "maxrow", MaxKensu};
	}//DBパラメータ; //DBパラメータ

	S02_0701Da s02_0701Da = null;
	s02_0701Da = new S02_0701Da();
									
									// 検索条件の指定に応じて表示
									if (string.IsNullOrEmpty(System.Convert.ToString(ucoYoyakuNo.YoyakuText)) == false)
									{
										// 予約番号が指定された場合
										
										//IO項目定義No.1の取得
										dtNo1 = s02_0701Da.getNo1Data(yoyakuKbn, yoyakuNo);
										
										//IO項目定義No.2の取得
										dtNo2 = s02_0701Da.getNo2Data(yoyakuKbn, yoyakuNo);
										
										if (ReferenceEquals(dtNo1, null) || dtNo1.Rows.Count == 0)
										{
											//検索結果0件時メッセージを表示する
											CommonProcess.createFactoryMsg().messageDisp("E90_019");
											return;
										}
									}
									
									//IO項目定義No.3の取得
									dtNo3 = s02_0701Da.getNo3Data(paramInfoList);

if (ReferenceEquals(dtNo3, null) || dtNo3.Rows.Count == 0)
{
	//検索結果0件時メッセージを表示する
	CommonProcess.createFactoryMsg().messageDisp("E90_019");
	return;
}


grdModosiListInquiry.HostedControlArr = new ArrayList();

DataTable dt = new DataTable();

//列作成
dt.Columns.Add("colSelection"); //選択ボタン
dt.Columns.Add("colSituation"); //状態
dt.Columns.Add("colSurname"); //姓
dt.Columns.Add("colName"); //名
dt.Columns.Add("colLost"); //紛失
dt.Columns.Add("colYmd"); //日付
dt.Columns.Add("colCrsName"); //コース名
dt.Columns.Add("colNoriba"); //乗り場
dt.Columns.Add("colTime"); //時間
dt.Columns.Add("colNinzu"); //人数
dt.Columns.Add("colYoyakuKbn"); //予約区分
dt.Columns.Add("colYoyakuNo"); //予約番号
dt.Columns.Add("colHakkenState"); //発券状態
dt.Columns.Add("colZasekiReserveYoyakuFlg"); //座席指定予約フラグ


// 払戻一覧照会グリッドへの値設定

if (dtNo3.Rows.Count > MaxKensu)
{
	// 取得件数が設定件数より多い場合、メッセージを表示
	CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が");
}

for (rowNum = 0; rowNum <= if (dtNo3.Rows.Count > MaxKensu, MaxKensu - 1, dtNo3.Rows.Count - 1); rowNum++)
									{

	object dr = dt.NewRow;

	// 判定用値の格納
	string valCancelFlg;
	string valHakkenNaiyo = "";
	string valZasekiSiteiFlg;
	string valState;

	valCancelFlg = System.Convert.ToString(if (ReferenceEquals(dtNo3.Rows(rowNum)["CANCEL_FLG"], DBNull.Value), "", dtNo3.Rows(rowNum)["CANCEL_FLG"].ToString().Trim()));
	valHakkenNaiyo = System.Convert.ToString(if (ReferenceEquals(dtNo3.Rows(rowNum)["HAKKEN_NAIYO"], DBNull.Value), "", dtNo3.Rows(rowNum)["HAKKEN_NAIYO"].ToString().Trim()));
	valZasekiSiteiFlg = System.Convert.ToString(if (ReferenceEquals(dtNo3.Rows(rowNum)["ZASEKI_RESERVE_YOYAKU_FLG"], DBNull.Value), "", dtNo3.Rows(rowNum)["ZASEKI_RESERVE_YOYAKU_FLG"].ToString().Trim()));
	valState = System.Convert.ToString(if (ReferenceEquals(dtNo3.Rows(rowNum)["STATE"], DBNull.Value), "", dtNo3.Rows(rowNum)["STATE"].ToString().Trim()));

	//
	if (valCancelFlg == FixedCdYoyaku.YoyakuCancelFlg.torikesi.ToString())
	{
		// キャンセルフラグ=1の場合
		dr["colSituation"] = "取消"; //状態列
		dr["colHakkenState"] = "消"; //発券状態列

	}
	else if (valCancelFlg == FixedCdYoyaku.YoyakuCancelFlg.sakujo.ToString())
	{
		// キャンセルフラグ=2の場合
		dr["colSituation"] = "削除"; //状態列
		dr["colHakkenState"] = "削"; //発券状態列

	}
	else
	{
		// キャンセルフラグ=""の場合
		if (string.IsNullOrWhiteSpace(valHakkenNaiyo) == true)
		{
			// 発券内容 = ""
			if (valZasekiSiteiFlg == FixedCdYoyaku.ZasekiSiteiFlg.sitei.ToString())
			{
				// 座席指定フラグ＝Y
				dr["colSituation"] = "座席指定予約"; //状態列
				dr["colHakkenState"] = "指"; //発券状態列

			}
			else
			{
				// 座席指定フラグ=""
				dr["colSituation"] = "未発券"; //状態列
				dr["colHakkenState"] = " "; //発券状態列

			}
		}
		else
		{
			// 発券内容 <> ""
			if (valState == FixedCdYoyaku.YoyakuState.renrakuHakken.ToString())
			{
				// 状態=2（連絡発券）
				dr["colSituation"] = "連絡発券"; //状態列
				dr["colHakkenState"] = "連"; //発券状態列
			}
			else if (valState == FixedCdYoyaku.YoyakuState.otaHakken.ToString())
			{
				// OTA
				dr["colSituation"] = "OTA発券"; //状態列
				dr["colHakkenState"] = "Ｏ"; //発券状態列

			}
			else
			{
				// 状態＝""
				dr["colSituation"] = "発券済"; //状態列
				dr["colHakkenState"] = "券"; //発券状態列

			}
		}
	}

	//姓名列表示設定
	dr["colSurname"] = dtNo3.Rows(rowNum)["SURNAME"]; //姓列
	dr["colName"] = dtNo3.Rows(rowNum)["NAME"]; //名列

	//紛失フラグ判定
	if (dtNo3.Rows(rowNum)["LOST_FLG"].ToString() == LostFlgState)
	{
		dr["colLost"] = "紛失";
	}
	else
	{
		dr["colLost"] = dtNo3.Rows(rowNum)["LOST_FLG"];
	}

	//下記列表示設定
	string tempSyuptDayString = System.Convert.ToString(dtNo3.Rows(rowNum)["SYUPT_DAY"].ToString());
	dr["colYmd"] = tempSyuptDayString.Substring(0, 4) + "/"
		+ tempSyuptDayString.Substring(4, 2) + "/" + tempSyuptDayString.Substring(6, 2); //出発日列
	dr["colCrsName"] = dtNo3.Rows(rowNum)["CRS_NAME"]; //コース名列
	if (ReferenceEquals(dtNo3.Rows(rowNum)["JYOCHACHI_NAME"], DBNull.Value) || dtNo3.Rows(rowNum)["JYOCHACHI_NAME"].ToString() == "")
	{
		//乗車地がマスタに存在しません
		CommonProcess.createFactoryMsg().messageDisp("E02_002");
		return;
	}
	dr["colNoriba"] = dtNo3.Rows(rowNum)["JYOCHACHI_NAME"]; //乗り場列
	string tempSyuptTime = "";
	if (dtNo3.Rows(rowNum)["SYUPT_TIME"].ToString().Length == 3)
	{
		tempSyuptTime = "0" + dtNo3.Rows(rowNum)["SYUPT_TIME"].ToString();
	}
	else
	{
		tempSyuptTime = System.Convert.ToString(dtNo3.Rows(rowNum)["SYUPT_TIME"].ToString());
	}
	dr["colTime"] = tempSyuptTime.Substring(0, 2) + ":" + tempSyuptTime.ToString().Substring(2, 2); //時間列
	dr["colNinzu"] = dtNo3.Rows(rowNum)["YOYAKU_NINZU"]; //人数列
	dr["colYoyakuKbn"] = dtNo3.Rows(rowNum)["YOYAKU_KBN"]; //予約区分列
	dr["colYoyakuNo"] = dtNo3.Rows(rowNum)["YOYAKU_NO"]; //予約番号列

	dt.Rows.Add(dr);
}

grdModosiListInquiry.DataSource = dt;

if (string.IsNullOrEmpty(System.Convert.ToString(ucoYoyakuNo.YoyakuText)) == false)
{
	//予約番号が指定されている場合

	// 画面表示判定と呼出の実行
	showS02_0703(yoyakuKbn, int.Parse(yoyakuNo));

}
									
								}
								
								/// <summary>
								/// 払戻画面の表示判定及び表示処理
								/// </summary>
								private void showS02_0703(string yoyakuKbn, int yoyakuNo)
{

	S02_0701Da da = new S02_0701Da();

	// 画面遷移のチェック
	DataTable dtNo1 = da.getNo1Data(yoyakuKbn, yoyakuNo.ToString());

	// 使用中
	string usingFlg = System.Convert.ToString(dtNo1.Rows(0)["USING_FLG"].ToString());
	if (usingFlg == FixedCd.UsingFlg.Use)
	{
		// 該当予約は予約で使用中です。確認後再度実行してください。。
		CommonProcess.createFactoryMsg().messageDisp("E90_040");
		return;
	}
	// 発券内容
	if (string.IsNullOrWhiteSpace(dtNo1.Rows(0)["HAKKEN_NAIYO"].ToString()))
	{
		//未発券の為、払戻できません。
		CommonProcess.createFactoryMsg().messageDisp("E90_047", "未発券の為、払戻");
		return;
	}


	//IO項目定義No.2取得結果保持用
	object dtNo2 = da.getNo2Data(yoyakuKbn, yoyakuNo.ToString());

	if (System.Convert.ToInt32(dtNo2.Rows(0)["KENSU"]) == 0)
	{
		//未発券の為、払戻できません。
		CommonProcess.createFactoryMsg().messageDisp("E90_047", "払戻処理されているため処理");
		return;
	}

	//選択ボタン押下
	//画面間パラメータを用意
	S02_0703ParamData prm = new S02_0703ParamData() {;
									.YoyakuKbn = yoyakuKbn,;
									.YoyakuNo = yoyakuNo,;
									.HakkenState = dtNo1.Rows(0)["STATE"].ToString();
};

//払戻処理　画面展開
using (S02_0703 form = new S02_0703())
{
	form.ParamData = prm;
	form.ShowDialog();
}
								
								
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

public bool CheckSearch()
{
	throw (new NotImplementedException());
}
							
							
#endregion
							
						}