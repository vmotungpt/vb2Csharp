using Hatobus.ReservationManagementSystem.Master;
using Hatobus.ReservationManagementSystem.Yoyaku;
using Hatobus.ReservationManagementSystem.Zaseki;
using System.Reflection;
using C1.Win.C1FlexGrid;
using System.ComponentModel;


/// <summary>
/// S04_0302 乗車状況詳細
/// </summary>
public class S04_0302 : PT81, iPT81
{

	#region 変数
	private DataSet searchDetailData = new DataSet(); // 画面表示用データ
	private DataTable searchResultCouseAddData = new DataTable(); // コース情報・追加情報検索結果データ
	private DataTable searchNinzuKbnData = new DataTable(); // 人数区分検索結果データ
	private DataTable dataBeforeCancel = new DataTable(); // キャンセル前データ
	private DataTable dtAfStayYoyaku = new DataTable(); // キャンセル前データ（後泊予約）
	private int numSyuptDay = 0; // DB更新用出発日
	private int numSyuptTime = 0; // DB更新用出発時間
	private int saveAfStayYoyakuSglNum = 0; // 後泊予約ＳＧＬ数保存用
	private int saveAfStayYoyakuTwnNum = 0; // 後泊予約ＴＷＮ数保存用
	private string saveDummyCrsCdSingle = string.Empty; // 後泊予約シングルダミーコード保存用
	private string saveDummyCrsCdTwin = string.Empty; // 後泊予約ツインダミーコード保存用
	private int saveDummySglKusekiNumTeiseki = 0; // 後泊予約シングル空席数定席保存用
	private int saveDummySglYoyakuNumTeiseki = 0; // 後泊予約シングル予約数定席保存用
	private int saveDummyTwnKusekiNumTeiseki = 0; // 後泊予約ツイン空席数定席保存用
	private int saveDummyTwnYoyakuNumTeiseki = 0; // 後泊予約ツイン予約数定席保存用
	private DateTime operationDate; // システム日付格納用
	private bool _isStay = false; // 宿泊あり
	private Hashtable paramInfoList = null; // 検索用パラメータ
	private int totalNinzu = 0; //人数合計値
	private Z0001_Param zasekiAutoSettingParameter = null; //座席自動設定パラメータ
	#endregion

	#region 定数
	//チェックエラー表示項目
	private const string ErrorDisplaySelectYoyaku = "選択した予約";
	private const string ErrorDisplayCancel = "取消";
	private const string ErrorDisplayHakken = "発券";
	private const string ErrorDisplayUpdate = " ";
	private const string ErrorDisplayAfStayYoyakuSgl = "（後泊予約シングル）";
	private const string ErrorDisplayAfStayYoyakuTwin = "（後泊予約ツイン）";
	private const string ErrorDisplayCrsAddInfo = "コース情報・追加情報";
	private const string ErrorDisplayMemoInfo = "メモ情報";
	private const string ErrorNinzuKbnInfo = "人数区分情報";
	private const string ErrorBusZasekiAutoSetting = "バス座席自動設定（確定配置・座席取消）";

	//人数区分グリッド（宿泊あり）の室タイプ
	private const string RowHeaderOne1R = "1名1室";
	private const string RowHeaderTwo1R = "2名1室";
	private const string RowHeaderThree1R = "3名1室";
	private const string RowHeaderFour1R = "4名1室";
	private const string RowHeaderFiveIjyou1R = "5名以上1室";

	//メモ入力画面引渡パラメータ
	private const string EditMode = "1"; //編集モード（新規登録）
	private const string RegisterType = "0"; //登録内容（通常）
	#endregion

	#region 列挙

	/// <summary>
	/// 室タイプ
	/// </summary>
	/// <remarks></remarks>
	private enum RoomType : int
	{
		[Value("室タイプ　1名1室")]
		One1R = 1,
		[Value("室タイプ　2名1室")]
		Two1R,
		[Value("室タイプ　3名1室")]
		Three1R,
		[Value("室タイプ　4名1室")]
		Four1R,
		[Value("室タイプ　5名以上1室")]
		FiveIjyou1R
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

	#region プロパティ

	/// <summary>
	/// 宿泊あり
	/// </summary>
	/// <returns></returns>
	public bool IsStay
	{
		get
		{
			return this._isStay;
		}
		set
		{
			this._isStay = value;
			this.grdChargeKbnStay.Visible = value;
			this.grdChargeKbn.Visible = !value;
		}
	}
	#endregion

	#region 構造体

	//引渡パラメータ用構造体
	private S04_0301.MihakkenYoyakuParameter transferParameter;

	//メモ入力画面引渡用エンティティ
	private Delivery_S02_0106Entity receiptEntity = new Delivery_S02_0106Entity();

	//自動座席配置引渡パラメータクラス
	private ZasekiKakuteiHaichi setAutoZasekiParameter = new ZasekiKakuteiHaichi();
	#endregion

	#region イベント

	#region 追加ボタン

	/// <summary>
	/// 追加ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnAdd_Click(object sender, EventArgs e)
	{

		JyosyaSituationDetail_DA dataAccess = new JyosyaSituationDetail_DA();
		bool returnResult = false;
		Delivery_S02_0106Entity receipt = new Delivery_S02_0106Entity();

		// 表示前に引数設定
		receipt.yoyakuKbn = transferParameter.YoyakuKbn;
		receipt.yoyakuNo = transferParameter.YoyakuNo;
		receipt.editMode = EditMode;
		receipt.registerType = RegisterType;

		using (S02_0106 form = new S02_0106(receipt))
		{
			//メモ入力画面表示
			openWindow(form, true);

			returnResult = System.Convert.ToBoolean(form.getReturnValue());
		}


		if (returnResult == true)
		{
			//メモ情報情報検索処理
			searchResultMemoData = dataAccess.getMemoInformation(paramInfoList);
			//メモ情報情報表示
			setMemo();
		}

	}

	#endregion

	#endregion

	#region メソッド

	#region 初期化処理

	#endregion

	#endregion

	#region DBパラメータ設定

	/// <summary>
	/// 検索用パラメータ設定処理
	/// </summary>
	/// <returns>検索用パラメータ</returns>
	private Hashtable setParameter()
	{

		//予約情報（基本）エンティティ
		YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();
		//DBパラメータ
		Hashtable paramList = new Hashtable();

		//パラメータ設定

		//遷移元画面からの引継ぎ項目より設定
		//予約区分
		paramList.Add(clsYoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, transferParameter.YoyakuKbn);
		//予約NO
		paramList.Add(clsYoyakuInfoBasicEntity.yoyakuNo.PhysicsName, transferParameter.YoyakuNo);

		return paramList;

	}

	/// <summary>
	/// キャンセル前チェック情報検索用パラメータ設定処理
	/// </summary>
	/// <returns>検索用パラメータ</returns>
	private Hashtable setParameterCheckInfoBeforeCancel()
	{
		//戻り値
		DataTable returnValue = null;
		//DBパラメータ
		Hashtable paramList = new Hashtable();

		//予約情報（基本）エンティティ
		YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();

		//パラメータ設定

		//遷移元画面からの引継ぎ項目より設定
		//予約区分
		paramList.Add(clsYoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, transferParameter.YoyakuKbn);
		//予約NO
		paramList.Add(clsYoyakuInfoBasicEntity.yoyakuNo.PhysicsName, transferParameter.YoyakuNo);

		return paramList;

	}

	/// <summary>
	/// 後泊予約情報検索用パラメータ設定処理
	/// </summary>
	/// <param name="paramCourseCd">コースコード</param>
	/// <returns>取得データ(DataTable)</returns>
	private Hashtable setParameterAfStayYoyaku(string paramCourseCd)
	{
		//戻り値
		DataTable returnValue = null;
		//DBパラメータ
		Hashtable paramList = new Hashtable();

		//コース台帳（基本）エンティティ
		CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();

		//パラメータ設定

		//遷移元画面からの引継ぎ項目より設定
		//コースコード
		paramList.Add(clsCrsLedgerBasicEntity.crsCd.PhysicsName, paramCourseCd);
		//出発日
		paramList.Add(clsCrsLedgerBasicEntity.syuptDay.PhysicsName, numSyuptDay);

		return paramList;

	}
	#endregion

	#region コース情報

	/// <summary>
	/// コース情報表示
	/// </summary>
	public void setCourseInformation()
	{

		//予約情報（基本）エンティティ
		YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();

		S04_0302 with_1 = this;
		//出発日
		with_1.dtmSyuptDay.Value = with_1.transferParameter.SyuptDay;
		//コースコード
		with_1.txtCrsCd.Text = with_1.transferParameter.CrsCd;
		//コース名
		with_1.txtCrsNm.Text = with_1.transferParameter.CrsNm;
		//号車
		with_1.txtGousya.Text = System.Convert.ToString(with_1.transferParameter.Gousya);
		//出発時間
		with_1.dtmSyuptTime.Value24 = with_1.transferParameter.SyuptTime;
		//予約番号
		with_1.ucoYoyakuNo.YoyakuText = with_1.transferParameter.YoyakuNumber.Replace(",", string.Empty);
		//予約者名
		with_1.txtYoyakuPersonName.Text = with_1.transferParameter.Seimei;


		//TEL
		with_1.txtTel.Text = searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.telNo1.PhysicsName].ToString();
		//TEL2
		with_1.txtTel2.Text = searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.telNo2.PhysicsName].ToString();
		//代理店
		with_1.txtDairiten.Text = with_1.transferParameter.AgentNm;
		//座席
		with_1.txtZaseki.Text = searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.zaseki.PhysicsName].ToString().Trim();
		//とび席
		if (searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.tobiSeatFlg.PhysicsName].ToString() == Targetflg.Target)
		{
			with_1.chkTobiSeat.Checked = true;
		}
		//予約変更（旧号車）
		with_1.txtOldGousya.Text = searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.oldGousya.PhysicsName].ToString();
	}


	#endregion

	#region 追加情報

	/// <summary>
	/// 追加情報表示
	/// </summary>
	public void setAddInfo()
	{
		string[] hakkenStateArry = null;

		//予約情報（基本）エンティティ
		YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();

		// 確認日
		if (!string.IsNullOrEmpty(System.Convert.ToString(searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.yoyakuKakuninDay.PhysicsName].ToString())))
		{
			this.dtmKakuninDay.Text = searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.yoyakuKakuninDay.PhysicsName].ToString();
		}
		// 発券日
		if (!string.IsNullOrEmpty(System.Convert.ToString(searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.hakkenDay.PhysicsName].ToString())))
		{
			this.dtmHakkenDay.Text = searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.hakkenDay.PhysicsName].ToString();
		}
		// 発券状態
		// 共通処理にて取得（パラメータ：キャンセルフラグ、座席指定予約フラグ、発券内容、状態）
		hakkenStateArry = CommonDAUtil.getYoyakuHakkenState(searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.cancelFlg.PhysicsName].ToString(),;
		searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.zasekiReserveYoyakuFlg.PhysicsName].ToString(),;
		searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.hakkenNaiyo.PhysicsName].ToString(),;
		searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.state.PhysicsName].ToString());
		this.txtHakkenState.Text = hakkenStateArry[1];
		// ツアー番号
		this.txtToursNo.Text = searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.toursNo.PhysicsName].ToString();
		// 名簿
		this.txtMeibo.Text = searchResultCouseAddData.Rows(0)["MEIBO"].ToString();
		// 精算方法
		this.txtSeisanHoho.Text = searchResultCouseAddData.Rows(0)["SEISAN_HOHO_NM"].ToString();
		// キャンセル区分
		this.txtCancelKbn.Text = searchResultCouseAddData.Rows(0)["CANCEL_KBN"].ToString();
		//NO SHOW
		if (searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.noShowFlg.PhysicsName].ToString() == Targetflg.Target)
		{
			this.chkNoShow.Checked = true;
		}
		this.chkNoShow.Enabled = false;
	}

	#endregion

	#region 人数区分

	/// <summary>
	/// 人数区分表示
	/// </summary>
	private void setNinzuKbn()
	{


		if (!this.IsStay)
		{
			//宿泊なし
			this.setGrdChargeKbn();

		}
		else
		{
			//宿泊あり
			this.setGrdChargeKbnStayAri();
		}
	}

	#endregion

	#region メモ

	/// <summary>
	/// メモ情報表示
	/// </summary>
	private void setMemo()
	{

		//編集不可
		grdMemo.AllowEditing = false;
		//自動列生成なし
		grdMemo.AutoGenerateColumns = false;

		//メモ情報表示
		grdMemo.DataSource = searchResultMemoData;

	}

	#endregion

	#region 料金区分(宿泊なし)グリッドの設定
	/// <summary>
	///料金区分(宿泊なし)グリッドの設定
	/// </summary>
	private void setGrdChargeKbn()
	{
		DataView dv = new DataView();

		//グリッドの設定
		this.grdChargeKbn.AllowDragging = AllowDraggingEnum.None;
		this.grdChargeKbn.AllowAddNew = false;
		this.grdChargeKbn.AutoGenerateColumns = false;
		this.grdChargeKbn.ShowButtons = ShowButtonsEnum.Always;

		//部屋タイプごとのデータを生成
		dv = this.formatGrdChargeKbnStayNashi();
		//料金区分(宿泊あり)テーブルをグリッドへ設定
		this.grdChargeKbn.DataSource = dv;
	}

	/// <summary>
	/// 料金区分グリッド（宿泊なし）データを作成
	/// </summary>
	/// <returns>料金区分グリッド（宿泊なし）表示用データ</returns>
	private DataView formatGrdChargeKbnStayNashi()
	{
		DataTable newDt = new DataTable();
		DataTable oldDt = new DataTable();
		DataTable sortDt = new DataTable();

		decimal dNinzu = 0;
		int ninzu = 0;
		DataRow newRow = null;
		totalNinzu = 0;

		//新しいDataTableを作成
		newDt.Columns.Add("CHARGE_KBN", typeof(string));
		newDt.Columns.Add("CHARGE_NAME", typeof(string));
		newDt.Columns.Add("CHARGE_KBN_JININ_CD", typeof(string));
		newDt.Columns.Add("CHARGE_KBN_JININ_NAME", typeof(string));
		newDt.Columns.Add("TANKA_1", typeof(int));
		newDt.Columns.Add("CHARGE_APPLICATION_NINZU_1", typeof(int));

		//宿泊なし
		foreach (DataRow row in this.searchNinzuKbnData.AsEnumerable())
		{
			dNinzu = System.Convert.ToDecimal(if (row.Field(Of Decimal ?)["CHARGE_APPLICATION_NINZU_1"], 0));
			ninzu = System.Convert.ToInt32(Convert.ToInt32(dNinzu));

			//null, 0なら次の行
			if (ninzu == 0)
			{
				continue;
			}

			//新規行を新しいテーブルへ追加
			newRow = newDt.NewRow;
			newRow.Item["CHARGE_KBN"] = row.Item["CHARGE_KBN"];
			newRow.Item["CHARGE_NAME"] = row.Item["CHARGE_NAME"];
			newRow.Item["CHARGE_KBN_JININ_CD"] = row.Item["CHARGE_KBN_JININ_CD"];
			newRow.Item["CHARGE_KBN_JININ_NAME"] = row.Item["CHARGE_KBN_JININ_NAME"];
			newRow.Item["TANKA_1"] = row.Item["TANKA_1"];
			newRow.Item["CHARGE_APPLICATION_NINZU_1"] = ninzu;
			newDt.Rows.Add(newRow);

			//人数合計値へ加算
			totalNinzu += ninzu;
		}

		//ソート
		object dt = From row in newDt.AsEnumerable();
		OrderByrow.Field(Ofint)("TANKA_1")Descending, ;
		row.Field(Of int)["CHARGE_APPLICATION_NINZU_1"] Descending,;
		row.Field(Of string)["CHARGE_KBN"],;
		row.Field(Of string)("CHARGE_KBN_JININ_CD");

		return dt.AsDataView();
	}
	#endregion

	#region 料金区分(宿泊あり)グリッドの設定

	/// <summary>
	///料金区分(宿泊あり)グリッドの設定
	/// </summary>
	private void setGrdChargeKbnStayAri()
	{
		DataView dv = new DataView();

		//グリッドの設定
		this.grdChargeKbnStay.AllowDragging = AllowDraggingEnum.None;
		this.grdChargeKbnStay.AllowAddNew = false;
		this.grdChargeKbnStay.AllowMerging = AllowMergingEnum.Custom;
		this.grdChargeKbnStay.AutoGenerateColumns = false;
		this.grdChargeKbnStay.ShowButtons = ShowButtonsEnum.Always;

		//部屋タイプごとのデータを生成
		dv = this.formatGrdChargeKbnStayAri();
		//料金区分(宿泊あり)テーブルをグリッドへ設定
		this.grdChargeKbnStay.DataSource = dv;

	}

	/// <summary>
	/// 部屋タイプ単位の料金区分グリッド（宿泊あり）データを作成
	/// </summary>
	/// <returns>部屋タイプ単位の料金区分グリッド（宿泊あり）表示用データ</returns>
	/// <remarks></remarks>
	private DataView formatGrdChargeKbnStayAri()
	{
		Dictionary roomTypeNames = new Dictionary(Of int, string);
		DataTable newDt = new DataTable();
		DataTable oldDt = new DataTable();
		DataTable sortDt = new DataTable();
		decimal dNinzu = 0;
		int ninzu = 0;
		DataRow newRow = null;
		totalNinzu = 0;

		//室タイプの名前
		roomTypeNames.Add(RoomType.One1R, RowHeaderOne1R);
		roomTypeNames.Add(RoomType.Two1R, RowHeaderTwo1R);
		roomTypeNames.Add(RoomType.Three1R, RowHeaderThree1R);
		roomTypeNames.Add(RoomType.Four1R, RowHeaderFour1R);
		roomTypeNames.Add(RoomType.FiveIjyou1R, RowHeaderFiveIjyou1R);

		//新しいDataTableを作成
		newDt.Columns.Add("ROOM_TYPE", typeof(string));
		newDt.Columns.Add("CHARGE_KBN_JININ_CD", typeof(string));
		newDt.Columns.Add("CHARGE_KBN_JININ_NAME", typeof(string));
		newDt.Columns.Add("TANKA", typeof(int));
		newDt.Columns.Add("CHARGE_APPLICATION_NINZU", typeof(int));
		newDt.Columns.Add("ROOMING_BETU_NINZU", typeof(int));

		//予約情報（コース料金_料金区分）は人員単位→（人員、部屋タイプ）単位で分解してデータを生成
		oldDt = searchNinzuKbnData;
		foreach (DataRow oldRow in oldDt.AsEnumerable())
		{

			dNinzu = 0;
			ninzu = 0;
			//各行の人数１～５をチェック
			for (int roomType = 1; roomType <= 5; roomType++)
			{
				dNinzu = System.Convert.ToDecimal(if (oldRow.Field(Of Decimal ?)["CHARGE_APPLICATION_NINZU_{roomType}"], 0));
			ninzu = System.Convert.ToInt32(Convert.ToInt32(dNinzu));

			//null, 0なら次の行
			if (ninzu == 0)
			{
				continue;
			}

			//新規行を新しいテーブルへ追加
			newRow = newDt.NewRow;
			newRow.Item["ROOM_TYPE"] = roomTypeNames[roomType];
			newRow.Item["CHARGE_KBN_JININ_CD"] = oldRow.Item["CHARGE_KBN_JININ_CD"];
			newRow.Item["CHARGE_KBN_JININ_NAME"] = oldRow.Item["CHARGE_KBN_JININ_NAME"];
			newRow.Item["TANKA"] = oldRow.Item["TANKA_{roomType}"];
			newRow.Item["CHARGE_APPLICATION_NINZU"] = ninzu;
			newRow.Item["ROOMING_BETU_NINZU"] = oldRow.Item["ROOMING_BETU_NINZU_{roomType}"];
			newDt.Rows.Add(newRow);

			//人数合計値へ加算
			totalNinzu += ninzu;

		}
	}

	//ソート
	object dt = From row in newDt.AsEnumerable();
		OrderByrow.Field(Ofstring) ("ROOM_TYPE")Descending, ;
		row.Field(Of int)["TANKA"] Descending,;
		row.Field(Of int)["CHARGE_APPLICATION_NINZU"] Descending,;
		row.Field(Of string)("CHARGE_KBN_JININ_CD");
		
		return dt.AsDataView();
	}

#endregion

#region 固有初期処理
/// <summary>
/// 固有初期処理
/// </summary>
protected override void initScreenPattern()
{
	int numOperationDate = 0;
	//予約情報（基本）エンティティ
	YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();

	//コントロール初期化
	CommonUtil.Control_Init(this.gbxCrsInfo.Controls);
	CommonUtil.Control_Init(this.gbxAddInfo.Controls);

	//宿泊有無の設定
	this.IsStay = System.Convert.ToBoolean(CommonCheckUtil.isStay(transferParameter.CrsKind));

	//初期表示内容検索・表示
	screenRefresh();

	//出発日をDB更新用に変換
	numSyuptDay = System.Convert.ToInt32(this.transferParameter.SyuptDay.ToString("yyyyMMddHHmmss").Substring(0, 8));

	//システム日付取得
	numOperationDate = System.Convert.ToInt32(CommonDateUtil.getSystemTime().ToString("yyyyMMddHHmmss").Substring(0, 8));

	if (numSyuptDay != numOperationDate ||)
	{
		(searchResultCouseAddData.Rows(0)(clsYoyakuInfoBasicEntity.teikiKikakuKbn.PhysicsName).ToString().Equals(System.Convert.ToString(Teiki_KikakuKbnType.kikakuTravel)) &&;
		!(searchResultCouseAddData.Rows(0)(clsYoyakuInfoBasicEntity.crsKind.PhysicsName).ToString().Equals(System.Convert.ToString(CrsKindType.rcourse))));
		//キャンセルボタンを押下不可にする
		this.F4Key_Enabled = false;
	}

}

/// <summary>
/// 引渡パラメータ設定
/// 親画面から情報を引渡す
/// </summary>
public void setTransferParameter(S04_0301.MihakkenYoyakuParameter pTransferParameter)
{
	transferParameter = pTransferParameter;
}

#region チェック

/// <summary>
/// キャンセル前チェック
/// </summary>
protected override bool checkBeforeCancel()
{
	//戻り値
	DataTable returnValue = null;
	DataTable returnAfStayYoyakuSglValue = null;
	DataTable returnAfStayYoyakuTwinValue = null;
	//判定結果
	bool returnResult = true;
	bool isDaisyoError = false;
	//キャンセルチェックエラーフラグ
	bool isCancelError = false;
	//発券済チェックエラーフラグ
	bool isHakkenZumiError = false;
	//使用中チェックエラーフラグ
	bool isUsingError = false;
	//更新可否チェックエラーフラグ
	bool isUpdateYesOrNoError = false;
	//後泊予約シングル更新可否チェックエラーフラグ
	bool isUpdateYesOrNoAfStayYoyakuSglError = false;
	//後泊予約ツイン更新可否チェックエラーフラグ
	bool isUpdateYesOrNoAfStayYoyakuTwinError = false;
	//エラー表示項目
	string errorDisplay = string.Empty;
	//DBパラメータ
	Hashtable paramInfoListBeforeCancelData = new Hashtable();
	Hashtable paramInfoListAfStayYoyakuSgl = new Hashtable();
	Hashtable paramInfoListAfStayYoyakuTwin = new Hashtable();

	//DataAccessクラス生成
	JyosyaSituationDetail_DA dataAccess = new JyosyaSituationDetail_DA();

	//予約情報（基本）エンティティ
	YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();
	//コース台帳（基本）エンティティ
	CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();

	//キャンセル前チェック情報検索用パラメータ設定処理
	paramInfoListBeforeCancelData = setParameterCheckInfoBeforeCancel();

	//キャンセル前データ検索処理
	dataBeforeCancel = dataAccess.getCheckInfoBeforeCancel(paramInfoListBeforeCancelData);

	//キャンセルチェック
	if (!string.IsNullOrEmpty(Strings.Trim(System.Convert.ToString(dataBeforeCancel.Rows(0)[clsYoyakuInfoBasicEntity.cancelFlg.PhysicsName].ToString()))))
	{
		//「{1}は既に{2}済みです。」のエラーを表示
		CommonProcess.createFactoryMsg().messageDisp("E90_069", ErrorDisplaySelectYoyaku, ErrorDisplayCancel);
		return false;
	}

	//発券済チェック
	if (string.IsNullOrEmpty(System.Convert.ToString(dataBeforeCancel.Rows(0)[clsYoyakuInfoBasicEntity.hakkenDay.PhysicsName].ToString())) == false && dataBeforeCancel.Rows(0)[clsYoyakuInfoBasicEntity.hakkenDay.PhysicsName].ToString() != "0")
	{
		//「{1}は既に{2}済みです。」のエラーを表示
		CommonProcess.createFactoryMsg().messageDisp("E90_069", ErrorDisplaySelectYoyaku, ErrorDisplayHakken);
		return false;
	}

	//使用中チェック
	if (!string.IsNullOrEmpty(Strings.Trim(System.Convert.ToString(dataBeforeCancel.Rows(0)["RES_BASE_USING_FLG"].ToString()))))
	{
		//「現在この予約は他端末で使用中のためキャンセルできません。」のエラーを表示
		CommonProcess.createFactoryMsg().messageDisp("E04_003");
		return false;
	}

	//使用中フラグチェック（コース台帳（基本））
	if (CommonCheckUtil.checkUsingFlgCrsLedgerBasic(numSyuptDay.ToString(),)
		{
		transferParameter.CrsCd(,);
		transferParameter.Gousya.ToString()) == false;
		//更新可否チェック
		//「コースが修正中（使用中）のため、予約変更できません。{1}」のエラーを表示
		CommonProcess.createFactoryMsg().messageDisp("E04_004", ErrorDisplayUpdate);
		return false;
	}

	//予約情報（基本）.後泊予約ＳＧＬ数 > 0の場合のみ取得を行う
	if (!string.IsNullOrEmpty(Strings.Trim(System.Convert.ToString(searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.afStayYoyakuSglNum.PhysicsName].ToString()))))
	{
		//キャンセル処理時使用のため保存
		saveAfStayYoyakuSglNum = System.Convert.ToInt32(searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.afStayYoyakuSglNum.PhysicsName].ToString());
		if (saveAfStayYoyakuSglNum > 0)
		{
			//キャンセル処理時使用のため保存
			saveDummyCrsCdSingle = System.Convert.ToString(searchResultCouseAddData.Rows(0)["DUMMY_CRS_CD_SINGLE"].ToString());
			//後泊予約シングル情報検索用パラメータ設定
			paramInfoListAfStayYoyakuSgl = setParameterAfStayYoyaku(saveDummyCrsCdSingle);
		}
	}
	//更新可否チェック（後泊予約シングル）
	if (paramInfoListAfStayYoyakuSgl.Count > 0)
	{
		if (checkUsingFlgAfYoyaku(paramInfoListAfStayYoyakuSgl) == false)
		{
			//「コースが修正中（使用中）のため、予約変更できません。{1}」のエラーを表示
			CommonProcess.createFactoryMsg().messageDisp("E04_004", ErrorDisplayAfStayYoyakuSgl);
			return false;
		}
		saveDummySglKusekiNumTeiseki = System.Convert.ToInt32(Nvl(dtAfStayYoyaku.Rows(0)["KUSEKI_NUM_TEISEKI"].ToString(), 0));
		saveDummySglYoyakuNumTeiseki = System.Convert.ToInt32(Nvl(dtAfStayYoyaku.Rows(0)["YOYAKU_NUM_TEISEKI"].ToString(), 0));
	}

	//予約情報（基本）.後泊予約ＴＷＮ数 > 0の場合のみ取得を行う
	if (!string.IsNullOrEmpty(Strings.Trim(System.Convert.ToString(searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.afStayYoyakuTwnNum.PhysicsName].ToString()))))
	{
		//キャンセル処理時使用のため保存
		saveAfStayYoyakuTwnNum = System.Convert.ToInt32(searchResultCouseAddData.Rows(0)[clsYoyakuInfoBasicEntity.afStayYoyakuTwnNum.PhysicsName].ToString());
		if (saveAfStayYoyakuTwnNum > 0)
		{
			//キャンセル処理時使用のため保存
			saveDummyCrsCdTwin = System.Convert.ToString(searchResultCouseAddData.Rows(0)["DUMMY_CRS_CD_TWIN"].ToString());
			//後泊予約ツイン情報検索用パラメータ設定
			paramInfoListAfStayYoyakuTwin = setParameterAfStayYoyaku(saveDummyCrsCdTwin);
		}
	}
	//更新可否チェック（後泊予約ツイン）
	if (paramInfoListAfStayYoyakuTwin.Count > 0)
	{
		if (checkUsingFlgAfYoyaku(paramInfoListAfStayYoyakuTwin) == false)
		{
			//「コースが修正中（使用中）のため、予約変更できません。{1}」のエラーを表示
			CommonProcess.createFactoryMsg().messageDisp("E04_004", ErrorDisplayAfStayYoyakuTwin);
			return false;
		}
		saveDummyTwnKusekiNumTeiseki = System.Convert.ToInt32(Nvl(dtAfStayYoyaku.Rows(0)["KUSEKI_NUM_TEISEKI"].ToString(), 0));
		saveDummyTwnYoyakuNumTeiseki = System.Convert.ToInt32(Nvl(dtAfStayYoyaku.Rows(0)["YOYAKU_NUM_TEISEKI"].ToString(), 0));
	}


	return true;

}

/// <summary>
/// 後泊予約使用中フラグチェック（コース台帳（基本））
/// </summary>
/// <parameter>検索パラメータ</parameter>
/// <parameter>チェック回数</parameter>
/// <parameter>チェック間隔（ミリ秒）</parameter>
/// <returns>True:エラーなし、False:エラーあり</returns>
private object checkUsingFlgAfYoyaku()
{
	Optional ByVal paramCheckCount int = 0,;
	Optional ByVal paramCheckInterval int = 0) bool;
int checkCount = 0;
int checkInterval = 0;
//DataAccessクラス生成
JyosyaSituationDetail_DA dataAccess = new JyosyaSituationDetail_DA();

//パラメータ（チェック回数）の指定がないまたは0が指定された場合は
//App.configより取得したチェック回数、チェック間隔でチェックを行う
if (paramCheckCount == 0)
{
	checkCount = System.Convert.ToInt32(usingFlgCheckCount);
	checkInterval = System.Convert.ToInt32(usingFlgCheckInterval);
}

for (int i = 1; i <= checkCount; i++)
{
	if (i > 1)
	{
		//チェック間隔に指定された時間スリープする
		Threading.Thread.Sleep(checkInterval);
	}
	dtAfStayYoyaku = dataAccess.getAfStayYoyakuUsingFlg(paramInfoListAfStayYoyaku);
	if (dtAfStayYoyaku.Rows.Count > 0)
	{
		if (dtAfStayYoyaku.Rows(0)["USING_FLG"].ToString().Equals(Targetflg.Target) == false)
		{
			//使用中フラグが立っていなければループを抜ける
			return true;
		}
	}
	else
	{
		//データが無い場合はループを抜ける
		return true;
	}
}

return false;
		
	}
#endregion
	
	/// <summary>
	/// 使用中フラグ更新
	/// </summary>
	protected override bool updateUsingFlg(int usingFlgOnOff)
{
	//DBパラメータ
	Hashtable paramInfoListYoyakuBasic = new Hashtable();
	int returnResult = 0;
	bool returnValue = false;

	//DataAccessクラス生成
	JyosyaSituationDetail_DA dataAccess = new JyosyaSituationDetail_DA();

	//予約情報（基本）エンティティ
	YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();

	//システム日付取得
	operationDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());

	//パラメータ設定
	//予約情報（基本）
	if (usingFlgOnOff == base.UsingOnOffType.usingOn)
	{
		//システム更新日
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.systemUpdateDay.PhysicsName, operationDate);
		//システム更新者コード
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.systemUpdatePersonCd.PhysicsName, UserInfoManagement.userId);
		//システム更新PGMID
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.systemUpdatePgmid.PhysicsName, this.Name);
	}
	else
	{
		//システム更新日
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.systemUpdateDay.PhysicsName, System.Convert.ToDateTime(searchResultCouseAddData.Rows(0)["SYSTEM_UPDATE_DAY"].ToString()));
		//システム更新者コード
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.systemUpdatePersonCd.PhysicsName, searchResultCouseAddData.Rows(0)["SYSTEM_UPDATE_PERSON_CD"].ToString());
		//システム更新PGMID
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.systemUpdatePgmid.PhysicsName, searchResultCouseAddData.Rows(0)["SYSTEM_UPDATE_PGMID"].ToString());
	}
	//予約NO
	paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.yoyakuNo.PhysicsName, transferParameter.YoyakuNo);
	//予約区分
	paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, transferParameter.YoyakuKbn);

	try
	{
		base.errorTableName = "予約情報（基本）[T_YOYAKU_INFO_BASIC]";
		//DB更新処理
		returnResult = System.Convert.ToInt32(dataAccess.executeUpdateYoyakuUsing(paramInfoListYoyakuBasic, usingFlgOnOff));

		if (returnResult > 0)
		{
			return true;
		}

		return returnValue;
	}
	catch (OracleException)
	{
		throw;
	}
	catch (Exception)
	{
		throw;
	}

}

/// <summary>
/// キャンセルDB更新
/// </summary>
/// <returns>データ更新件数</returns>
protected override int cancelDBUpdate()
{
	//DBパラメータ
	Hashtable paramInfoListYoyakuBasic = new Hashtable();
	Hashtable paramInfoListCrsBasic = new Hashtable();
	Hashtable paramInfoListYoyakuCrsCharge = new Hashtable();
	Hashtable paramInfoListYoyakuCrsChargeChargeKbn = new Hashtable();
	Hashtable paramInfoListYoyakuInfo2 = new Hashtable();
	ArrayList paramInfoListArrayList = new ArrayList();

	int crsLedgerYoyakuNumTeiseki = 0;
	int crsLedgerYoyakuNumSubSeat = 0;
	int crsLedgerKusekiNumTeiseki = 0;
	int crsLedgerKusekiNumSubSeat = 0;
	int crsLedgerYoyakuNumTeisekiDummySgl = 0;
	int crsLedgerKusekiNumTeisekiDummySgl = 0;
	int crsLedgerYoyakuNumTeisekiDummyTwn = 0;
	int crsLedgerKusekiNumTeisekiDummyTwn = 0;
	int zesekiAutoKagenTeiseki = 0;
	int zesekiAutoKagenSubSeat = 0;
	int zesekiAutoKusekiNumTeiseki = 0;
	int zesekiAutoKusekiNumSubSeat = 0;
	int zesekiAutoSubCyoseiSeatNum = 0;
	//空席数補助席算出結果
	int calcKusekiNumSubSeat = 0;

	//座席加減数取得サブ戻り値
	Z0001_Result returnZasekiResult = null;

	//戻り値
	int returnResult = 0;

	//DataAccessクラス生成
	JyosyaSituationDetail_DA dataAccess = new JyosyaSituationDetail_DA();

	//予約情報（基本）エンティティ
	YoyakuInfoBasicEntity clsYoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();
	//コース台帳（基本）エンティティ
	CrsLedgerBasicEntity clsCrsLedgerBasicEntity = new CrsLedgerBasicEntity();
	//予約情報（コース料金）エンティティ
	YoyakuInfoCrsChargeEntity clsYoyakuInfoCrsChargeEntity = new YoyakuInfoCrsChargeEntity();
	//予約情報（コース料金_料金区分）エンティティ
	YoyakuInfoCrsChargeChargeKbnEntity clsYoyakuInfoCrsChargeChargeKbnEntity = new YoyakuInfoCrsChargeChargeKbnEntity();
	//予約情報２エンティティ
	YoyakuInfo2Entity clsYoyakuInfo2Entity = new YoyakuInfo2Entity();

	string[] logNaiyo = new string[3];

	OracleTransaction oracleTransaction = null;
	try
	{
		//トランザクション開始
		oracleTransaction = dataAccess.callBeginTransactionWrap();
		//システム日付を文字列に変換
		string strOperationDate = operationDate.ToString("yyyyMMddHHmmss");

		//座席加減数取得
		//共通処理（バス座席自動設定（確定配置・座席取消））
		returnZasekiResult = getZasekiAutoSetting(oracleTransaction);

		//パラメータ設定
		//予約情報（基本）
		//システム更新日
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.systemUpdateDay.PhysicsName, operationDate);
		//システム更新者コード
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.systemUpdatePersonCd.PhysicsName, UserInfoManagement.userId);
		//システム更新PGMID
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.systemUpdatePgmid.PhysicsName, this.Name);
		//変更履歴最終日
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.changeHistoryLastDay.PhysicsName, int.Parse(strOperationDate.Substring(0, 8)));
		//変更履歴最終ＳＥＱ
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.changeHistoryLastSeq.PhysicsName, dataBeforeCancel.Rows(0)[clsYoyakuInfoBasicEntity.changeHistoryLastSeq.PhysicsName].ToString());
		//後泊予約ＳＧＬ数
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.afStayYoyakuSglNum.PhysicsName, saveAfStayYoyakuSglNum);
		//後泊予約ＴＷＮ数
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.afStayYoyakuTwnNum.PhysicsName, saveAfStayYoyakuTwnNum);
		//更新日
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.updateDay.PhysicsName, int.Parse(strOperationDate.Substring(0, 8)));
		//更新者コード
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.updatePersonCd.PhysicsName, UserInfoManagement.userId);
		//更新PGMID
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.updatePgmid.PhysicsName, this.Name);
		//更新時刻
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.updateTime.PhysicsName, int.Parse(strOperationDate.Substring(8, 6)));
		//予約NO
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.yoyakuNo.PhysicsName, transferParameter.YoyakuNo);
		//予約区分
		paramInfoListYoyakuBasic.Add(clsYoyakuInfoBasicEntity.yoyakuKbn.PhysicsName, transferParameter.YoyakuKbn);

		//コース台帳（基本）
		//システム更新日
		paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.systemUpdateDay.PhysicsName, operationDate);
		//システム更新者コード
		paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.systemUpdatePersonCd.PhysicsName, UserInfoManagement.userId);
		//システム更新PGMID
		paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.systemUpdatePgmid.PhysicsName, this.Name);
		//コース台帳（基本）.予約数定席
		crsLedgerYoyakuNumTeiseki = System.Convert.ToInt32(Nvl(dataBeforeCancel.Rows(0)[clsCrsLedgerBasicEntity.yoyakuNumTeiseki.PhysicsName], 0));
		//コース台帳（基本）.予約数補助席
		crsLedgerYoyakuNumSubSeat = System.Convert.ToInt32(Nvl(dataBeforeCancel.Rows(0)[clsCrsLedgerBasicEntity.yoyakuNumSubSeat.PhysicsName], 0));
		//コース台帳（基本）.空席数定席
		crsLedgerKusekiNumTeiseki = System.Convert.ToInt32(Nvl(dataBeforeCancel.Rows(0)[clsCrsLedgerBasicEntity.kusekiNumTeiseki.PhysicsName], 0));
		//コース台帳（基本）.空席数補助席
		crsLedgerKusekiNumSubSeat = System.Convert.ToInt32(Nvl(dataBeforeCancel.Rows(0)[clsCrsLedgerBasicEntity.kusekiNumSubSeat.PhysicsName], 0));

		//ステータス = '00'の場合
		if (returnZasekiResult.Status == Z0001_Result.Z0001_Result_Status.OK)
		{
			//車種に"NORMAL"を設定
			paramInfoListCrsBasic.Add("CAR_TYPE", "NORMAL");
			//'共通処理（バス座席自動設定（確定配置・座席取消））の返却値
			//バス座席自動設定（確定配置・座席取消）.座席加減／定席
			zesekiAutoKagenTeiseki = System.Convert.ToInt32(Nvl(returnZasekiResult.ZasekiKagenTeiseki, 0));
			//バス座席自動設定（確定配置・座席取消）.空席数／定席
			zesekiAutoKusekiNumTeiseki = System.Convert.ToInt32(Nvl(returnZasekiResult.KusekiNumTeiseki, 0));
			//バス座席自動設定（確定配置・座席取消）.座席加減／補助席
			zesekiAutoKagenSubSeat = System.Convert.ToInt32(Nvl(returnZasekiResult.ZasekiKagenSub1F, 0));
			//バス座席自動設定（確定配置・座席取消）.空席数／補助席
			zesekiAutoKusekiNumSubSeat = System.Convert.ToInt32(Nvl(returnZasekiResult.KusekiNumSub1F, 0));
			//バス座席自動設定（確定配置・座席取消）.補助調整席
			zesekiAutoSubCyoseiSeatNum = System.Convert.ToInt32(Nvl(returnZasekiResult.SubCyoseiSeatNum, 0));

			//予約数定席
			paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.yoyakuNumTeiseki.PhysicsName, crsLedgerYoyakuNumTeiseki + zesekiAutoKagenTeiseki);
			//予約数補助席
			paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.yoyakuNumSubSeat.PhysicsName, crsLedgerYoyakuNumSubSeat + zesekiAutoKagenSubSeat);
			//空席数定席
			if (crsLedgerKusekiNumTeiseki - zesekiAutoKagenTeiseki >= zesekiAutoKusekiNumTeiseki)
			{
				paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.kusekiNumTeiseki.PhysicsName, zesekiAutoKusekiNumTeiseki);
			}
			else
			{
				paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.kusekiNumTeiseki.PhysicsName, crsLedgerKusekiNumTeiseki - zesekiAutoKagenTeiseki);
			}
			//空席数補助席
			calcKusekiNumSubSeat = crsLedgerKusekiNumSubSeat - zesekiAutoKagenSubSeat - zesekiAutoSubCyoseiSeatNum;
			if (calcKusekiNumSubSeat >= zesekiAutoKusekiNumSubSeat)
			{
				paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.kusekiNumSubSeat.PhysicsName, zesekiAutoSubCyoseiSeatNum);
			}
			else
			{
				if (calcKusekiNumSubSeat < 0)
				{
					calcKusekiNumSubSeat = 0;
				}
				paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.kusekiNumSubSeat.PhysicsName, calcKusekiNumSubSeat);
			}
			//ステータス =  '10'（座席イメージ.車種コード='XX'の場合）
		}
		else if (returnZasekiResult.Status == Z0001_Result.Z0001_Result_Status.Kaku)
		{
			//予約数定席
			paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.yoyakuNumTeiseki.PhysicsName, crsLedgerYoyakuNumTeiseki - totalNinzu);
			//空席数定席
			paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.kusekiNumTeiseki.PhysicsName, crsLedgerKusekiNumTeiseki + totalNinzu);
		}
		else
		{
			//ステータスが上記以外の場合、エラーメッセージ・ログを出力し処理を中断する
			base.errorTableName = ErrorBusZasekiAutoSetting;
			logNaiyo[0] = ErrorBusZasekiAutoSetting;
			if (ReferenceEquals(zasekiAutoSettingParameter, null))
			{
				logNaiyo[1] = "【ステータス】" + returnZasekiResult.Status +;
				",【パラメータ】Nothing";
			}
			else
			{
				logNaiyo[1] = "【ステータス】" + returnZasekiResult.Status +;
				",【パラメータ】処理区分：" + zasekiAutoSettingParameter.ProcessKbn.ToString() +;
				",出発日：" + zasekiAutoSettingParameter.SyuptDay.ToString() + ",号車：" + zasekiAutoSettingParameter.Gousya.ToString() +;
				",バス指定コード：" + zasekiAutoSettingParameter.BusReserveCd.ToString() + ",人数：" + zasekiAutoSettingParameter.Ninzu.ToString() +;
				",予約区分：" + zasekiAutoSettingParameter.YoyakuKbn.ToString() + ",予約ＮＯ：" + zasekiAutoSettingParameter.YoyakuNo.ToString();
			}
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, base.setFormId + ":" + base.setTitle, logNaiyo);
			//更新失敗時処理
			commonRollBack(oracleTransaction, dataAccess, true);
			return -1;
		}
		//コースコード
		paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.crsCd.PhysicsName, transferParameter.CrsCd);
		//出発日
		paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.syuptDay.PhysicsName, numSyuptDay);
		//号車
		paramInfoListCrsBasic.Add(clsCrsLedgerBasicEntity.gousya.PhysicsName, transferParameter.Gousya);

		if (saveAfStayYoyakuSglNum > 0)
		{
			//後泊用ダミーコースコード（SINGLE）
			paramInfoListCrsBasic.Add("DUMMY_CRS_CD_SINGLE", saveDummyCrsCdSingle);
			paramInfoListCrsBasic.Add("DUMMY_SINGLE_YOYAKU_NUM_TEISEKI", saveDummySglYoyakuNumTeiseki - saveAfStayYoyakuSglNum);
			paramInfoListCrsBasic.Add("DUMMY_SINGLE_KUSEKI_NUM_TEISEKI", saveDummySglKusekiNumTeiseki + saveAfStayYoyakuSglNum);
		}
		//後泊用ダミーコースコード（TWIN）
		if (saveAfStayYoyakuTwnNum > 0)
		{
			//後泊用ダミーコースコード（SINGLE）
			paramInfoListCrsBasic.Add("DUMMY_CRS_CD_TWIN", saveDummyCrsCdTwin);
			paramInfoListCrsBasic.Add("DUMMY_TWIN_YOYAKU_NUM_TEISEKI", saveDummyTwnYoyakuNumTeiseki - saveAfStayYoyakuTwnNum);
			paramInfoListCrsBasic.Add("DUMMY_TWIN_KUSEKI_NUM_TEISEKI", saveDummyTwnKusekiNumTeiseki + saveAfStayYoyakuTwnNum);
		}

		//予約情報２
		//システム登録日
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.systemEntryDay.PhysicsName, operationDate);
		//システム登録者コード
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.systemEntryPersonCd.PhysicsName, UserInfoManagement.userId);
		//システム登録PGMID
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.systemEntryPgmid.PhysicsName, this.Name);
		//システム更新日
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.systemUpdateDay.PhysicsName, operationDate);
		//システム更新者コード
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.systemUpdatePersonCd.PhysicsName, UserInfoManagement.userId);
		//システム更新PGMID
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.systemUpdatePgmid.PhysicsName, this.Name);
		//年
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.year.PhysicsName, System.Convert.ToInt32(numSyuptDay.ToString().Substring(0, 4)));
		//出力日
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.outDay.PhysicsName, int.Parse(strOperationDate.Substring(0, 8)));
		//出力者コード
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.outPersonCd.PhysicsName, UserInfoManagement.userId);
		//出力PGMID
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.outPgmid.PhysicsName, this.Name);
		//出力時刻
		paramInfoListYoyakuBasic.Add(clsYoyakuInfo2Entity.outTime.PhysicsName, int.Parse(strOperationDate.Substring(8, 6)));
		//予約区分
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.yoyakuKbn.PhysicsName, transferParameter.YoyakuKbn);
		//予約NO
		paramInfoListYoyakuInfo2.Add(clsYoyakuInfo2Entity.yoyakuNo.PhysicsName, transferParameter.YoyakuNo);

		//予約情報（コース料金）
		//システム更新日
		paramInfoListYoyakuCrsCharge.Add(clsYoyakuInfoCrsChargeEntity.systemUpdateDay.PhysicsName, operationDate);
		//システム更新者コード
		paramInfoListYoyakuCrsCharge.Add(clsYoyakuInfoCrsChargeEntity.systemUpdatePersonCd.PhysicsName, UserInfoManagement.userId);
		//システム更新PGMID
		paramInfoListYoyakuCrsCharge.Add(clsYoyakuInfoCrsChargeEntity.systemUpdatePgmid.PhysicsName, this.Name);
		//予約区分
		paramInfoListYoyakuCrsCharge.Add(clsYoyakuInfoCrsChargeEntity.yoyakuKbn.PhysicsName, transferParameter.YoyakuKbn);
		//予約NO
		paramInfoListYoyakuCrsCharge.Add(clsYoyakuInfoCrsChargeEntity.yoyakuNo.PhysicsName, transferParameter.YoyakuNo);

		//予約情報（コース料金_料金区分）
		foreach (DataRow row in searchNinzuKbnData.Rows)
		{
			//パラメータ設定
			paramInfoListYoyakuCrsChargeChargeKbn = new Hashtable();

			//システム更新日
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.systemUpdateDay.PhysicsName, operationDate);
			//システム更新者コード
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.systemUpdatePersonCd.PhysicsName, UserInfoManagement.userId);
			//システム更新PGMID
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.systemUpdatePgmid.PhysicsName, this.Name);
			//料金適用人数１
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeApplicationNinzu1.PhysicsName, 0);
			//キャンセル人数１
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.cancelNinzu1.PhysicsName, System.Convert.ToInt32(Nvl(row["CHARGE_APPLICATION_NINZU_1"])));
			if (this.IsStay == true)
			{
				//料金適用人数２
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeApplicationNinzu2.PhysicsName, 0);
				//料金適用人数３
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeApplicationNinzu3.PhysicsName, 0);
				//料金適用人数４
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeApplicationNinzu4.PhysicsName, 0);
				//料金適用人数５
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeApplicationNinzu5.PhysicsName, 0);
				//キャンセル人数２
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.cancelNinzu2.PhysicsName, System.Convert.ToInt32(Nvl(row["CHARGE_APPLICATION_NINZU_2"])));
				//キャンセル人数３
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.cancelNinzu3.PhysicsName, System.Convert.ToInt32(Nvl(row["CHARGE_APPLICATION_NINZU_3"])));
				//キャンセル人数４
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.cancelNinzu4.PhysicsName, System.Convert.ToInt32(Nvl(row["CHARGE_APPLICATION_NINZU_4"])));
				//キャンセル人数５
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.cancelNinzu5.PhysicsName, System.Convert.ToInt32(Nvl(row["CHARGE_APPLICATION_NINZU_5"])));
			}
			else
			{
				//料金適用人数２
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeApplicationNinzu2.PhysicsName, string.Empty);
				//料金適用人数３
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeApplicationNinzu3.PhysicsName, string.Empty);
				//料金適用人数４
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeApplicationNinzu4.PhysicsName, string.Empty);
				//料金適用人数５
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeApplicationNinzu5.PhysicsName, string.Empty);
				//キャンセル人数２
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.cancelNinzu2.PhysicsName, string.Empty);
				//キャンセル人数３
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.cancelNinzu3.PhysicsName, string.Empty);
				//キャンセル人数４
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.cancelNinzu4.PhysicsName, string.Empty);
				//キャンセル人数５
				paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.cancelNinzu5.PhysicsName, string.Empty);
			}
			//料金適用人数
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeApplicationNinzu.PhysicsName, 0);
			//キャンセル人数
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.cancelNinzu.PhysicsName, System.Convert.ToInt32(Nvl(row["CHARGE_APPLICATION_NINZU"])));
			//予約区分
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.yoyakuKbn.PhysicsName, transferParameter.YoyakuKbn);
			//予約NO
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.yoyakuNo.PhysicsName, transferParameter.YoyakuNo);
			//区分No
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.kbnNo.PhysicsName, System.Convert.ToInt32(row["KBN_NO"]));
			//料金区分（人員）コード
			paramInfoListYoyakuCrsChargeChargeKbn.Add(clsYoyakuInfoCrsChargeChargeKbnEntity.chargeKbnJininCd.PhysicsName, row["CHARGE_KBN_JININ_CD"].ToString());
			paramInfoListArrayList.Add(paramInfoListYoyakuCrsChargeChargeKbn);
		}

		//DB更新処理
		returnResult = System.Convert.ToInt32(dataAccess.executeUpdateYoyakuCancel(paramInfoListYoyakuBasic, paramInfoListCrsBasic,);
		paramInfoListYoyakuCrsCharge(, paramInfoListArrayList,);
		searchNinzuKbnData(, paramInfoListYoyakuInfo2, this.setTitle, this.IsStay, oracleTransaction));
		base.errorTableName = dataAccess.errorTableNm;

		if (returnResult > 0)
		{
			//一括登録成功
			//コミット
			dataAccess.callCommitTransactionWrap(oracleTransaction);
		}
		else
		{
			//一括登録失敗
			commonRollBack(oracleTransaction, dataAccess, false);
		}
		//★使用中フラグ解除は、コミット or ロールバックの後★
		ZasekiCommon clsZasekiCommon = new ZasekiCommon();
		//使用中フラグ解除[座席部品]
		clsZasekiCommon.ReleaseZaseki(numSyuptDay,;
		transferParameter.BusReserveCd(,);
		transferParameter.Gousya(,);
		setFormId(,);
		UserInfoManagement.userId());
	}
	catch (OracleException)
	{
		commonRollBack(oracleTransaction, dataAccess, true);
		throw;
	}
	catch (Exception)
	{
		commonRollBack(oracleTransaction, dataAccess, true);
		throw;
	}
	finally
	{
		oracleTransaction.Dispose();
	}

	return returnResult;

}

/// <summary>
/// 共通ロールバック処理
/// (使用中フラグ解除[座席部品]含む)
/// </summary>
/// <param name="oracleTransaction"></param>
/// <param name="dataAccess"></param>
/// <param name="ReleaseZasekiFlg"></param>
private void commonRollBack()
{
	dataAccess(JyosyaSituationDetail_DA,);
	ReleaseZasekiFlg(bool));
if (!ReferenceEquals(oracleTransaction, null))
{
	if (oracleTransaction.Connection.State == ConnectionState.Open)
	{
		//ロールバック
		dataAccess.callRollbackTransactionWrap(oracleTransaction);
	}
}
if (ReleaseZasekiFlg == true)
{
	//★使用中フラグ解除は、コミット or ロールバックの後★
	ZasekiCommon clsZasekiCommon = new ZasekiCommon();
	//使用中フラグ解除[座席部品]
	clsZasekiCommon.ReleaseZaseki(numSyuptDay,;
	transferParameter.BusReserveCd(,);
	transferParameter.Gousya(,);
	setFormId(,);
	UserInfoManagement.userId());
}
	}
	
	/// <summary>
	/// バス座席自動設定処理
	/// </summary>
	/// <returns>座席自動配置（確定配置・座席取消）結果</returns>
	private Z0001_Result getZasekiAutoSetting(OracleTransaction oracleTransaction)
{
	Z0001_Param param = new Z0001_Param();

	//処理区分：取消（すべて空席に戻す）
	param.ProcessKbn = Z0001_Param.Z0001_Param_ProcessKbn.ProcessKbn_99;
	//出発日
	param.SyuptDay = numSyuptDay;
	//号車
	param.Gousya = transferParameter.Gousya;
	//バス指定コード
	param.BusReserveCd = transferParameter.BusReserveCd;
	//人数
	param.Ninzu = totalNinzu;
	//予約区分
	param.YoyakuKbn = transferParameter.YoyakuKbn;
	//予約NO
	param.YoyakuNo = transferParameter.YoyakuNo;
	//トランザクション
	param.Transaction = oracleTransaction;

	//エラー時パラメータ内容表示用変数
	zasekiAutoSettingParameter = new Z0001_Param();
	zasekiAutoSettingParameter.ProcessKbn = Z0001_Param.Z0001_Param_ProcessKbn.ProcessKbn_99;
	zasekiAutoSettingParameter.SyuptDay = numSyuptDay;
	zasekiAutoSettingParameter.Gousya = transferParameter.Gousya;
	zasekiAutoSettingParameter.BusReserveCd = transferParameter.BusReserveCd;
	zasekiAutoSettingParameter.Ninzu = totalNinzu;
	zasekiAutoSettingParameter.YoyakuKbn = transferParameter.YoyakuKbn;
	zasekiAutoSettingParameter.YoyakuNo = transferParameter.YoyakuNo;

	Z0001 z0001 = new Z0001();
	Z0001_Result result = z0001.Execute(param);

	return result;
}

/// <summary>
/// 画面再描画処理
/// </summary>
protected override void screenRefresh()
{
	//DataAccessクラス生成
	JyosyaSituationDetail_DA dataAccess = new JyosyaSituationDetail_DA();

	paramInfoList = new Hashtable();

	paramInfoList = setParameter();

	//コース情報・追加情報取得
	searchDetailData = dataAccess.getDetailData(paramInfoList);
	searchResultCouseAddData = searchDetailData.Tables(0);
	searchNinzuKbnData = searchDetailData.Tables(1);
	searchResultMemoData = searchDetailData.Tables(2);

	if (searchResultCouseAddData.Rows.Count > 0)
	{
		if (searchNinzuKbnData.Rows.Count > 0)
		{
			//コース情報表示
			setCourseInformation();
			//追加情報表示
			setAddInfo();
			//人数区分表示
			setNinzuKbn();

			if (searchResultMemoData.Rows.Count > 0)
			{
				//メモ情報表示
				setMemo();
			}
			else
			{
				//メモ情報取得件数0件の場合、処理なし
			}
		}
		else
		{
			//人数区分情報取得件数0件の場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_014", ErrorNinzuKbnInfo);

			//キャンセルボタンを押下不可にする
			this.F4Key_Enabled = false;
		}
	}
	else
	{
		//コース情報・追加情報取得件数0件の場合、メッセージを表示
		CommonProcess.createFactoryMsg().messageDisp("E90_014", ErrorDisplayCrsAddInfo);

		//キャンセルボタンを押下不可にする
		this.F4Key_Enabled = false;
	}
}
	
#endregion
	
}