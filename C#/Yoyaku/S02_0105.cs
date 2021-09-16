using System.Text;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.Zaseki;


/// <summary>
/// S02_0105 発券（当日）
/// </summary>
public class S02_0105 : FormBase
{

	#region 定数／変数
	/// <summary>
	/// 画面ID
	/// </summary>
	private const string PgmId = "S02_0105";
	/// <summary>
	/// 画面名
	/// </summary>
	private const string ScreenName = "発券（当日）";
	/// <summary>
	/// 運休
	/// </summary>
	private const string Unkyu = "運休";
	/// <summary>
	/// 警告時間
	/// システム日付の３０分前
	/// </summary>
	private const int MinutesAlert = -30;
	/// <summary>
	/// 警告時間（他営業所）
	/// システム日付の１時間後
	/// </summary>
	private const int HourAlertOtherEigyosyo = 1;
	/// <summary>
	/// 24時
	/// </summary>
	private const int TwentyFourOClock = 2400;

	#region ハッシュキー
	/// <summary>
	/// ハッシュキー　システム日付（日付型)
	/// </summary>
	private const string KeyDtSysDate = "dtSysDate";
	/// <summary>
	/// ハッシュキー　システム日付（文字列型)
	/// </summary>
	private const string KeyStrSysDate = "strSysDate";
	/// <summary>
	/// ハッシュキー　システム日付（数値型)
	/// </summary>
	private const string KeyIntSysDate = "intSysDate";
	/// <summary>
	/// ハッシュキー　システム時刻（時分秒）（文字列型)
	/// </summary>
	private const string KeyStrSysTimeHhMmSs = "strSysTimeHhMmSs";
	/// <summary>
	/// ハッシュキー　システム時刻（時分秒）（数値型)
	/// </summary>
	private const string KeyIntSysTimeHhMmSs = "intSysTimeHhMmSs";
	/// <summary>
	/// ハッシュキー　システム時刻（時分）（文字列型)
	/// </summary>
	private const string KeyStrSysTimeHhMm = "strSysTimeHhMm";
	/// <summary>
	/// ハッシュキー　システム時刻（時分）（数値型)
	/// </summary>
	private const string KeyIntSysTimeHhMm = "intSysTimeHhMm";
	#endregion

	/// <summary>
	/// 予約区分
	/// </summary>
	private string _yoyakuKbn;
	/// <summary>
	/// 予約NO
	/// </summary>
	private int _yoyakuNo;
	/// <summary>
	/// 正規料金総額
	/// </summary>
	private int _seikiChargeAllGaku = 0;
	/// <summary>
	/// 予約人数
	/// </summary>
	private int _yoyakuNinzu = 0;
	/// <summary>
	/// 座席自動配置（新規予約）結果
	/// </summary>
	private Z0001_Result _z0001Result;

	#region DataTable
	/// <summary>
	/// コース台帳（基本）
	/// </summary>
	private DataTable _crsLedgerBasic = null;
	/// <summary>
	/// 料金区分一覧
	/// </summary>
	private DataTable _chargeKbnList = null;
	/// <summary>
	/// 場所マスタ
	/// </summary>
	private DataTable _placeMaster = null;
	/// <summary>
	/// 任意選択一覧
	/// </summary>
	private DataTable _anySelectionList = null;
	/// <summary>
	/// 人毎必須選択一覧
	/// </summary>
	private DataTable _personRequiredSelectionList = null;
	/// <summary>
	/// 予約毎必須選択一覧
	/// </summary>
	private DataTable _yoyakuRequiredSelectionList = null;
	/// <summary>
	/// オプション当日金額
	/// </summary>
	private int _tojituAmount = 0;
	/// <summary>
	/// オプション事前金額
	/// </summary>
	private int _jizenAmount = 0;

	#endregion
	#endregion

	#region プロパティ
	/// <summary>
	/// パラメータクラス
	/// </summary>
	public S02_0105ParamData ParamData
	{

#endregion

		#region イベント
		/// <summary>
		/// 画面ロード時のイベント
		/// </summary>
		/// <param name="sender">イベント送信元</param>
		/// <param name="e">イベントデータ</param>
	private void S02_0105_Load(object sender, System.EventArgs e)
	{
		try
		{
			//前処理
			comPreEvent();
			//パラメータチェック
			if (this.ParamData.SyuptDay == 0)
			{
				return;
			}
			if (string.IsNullOrEmpty(System.Convert.ToString(this.ParamData.CrsCd)))
			{
				return;
			}
			if (this.ParamData.Gousya == 0)
			{
				return;
			}
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(this.ParamData.JyosyatiCd)))
			{
				return;
			}
			if (this.ParamData.SyuptTime == 0)
			{
				return;
			}

			//画面表示時の初期設定
			this.setControlInitiarize();

		}
		catch (Exception ex)
		{
			createFactoryMsg().messageDisp("E90_046", ex.Message);
			this.Close();

		}
		finally
		{
			// 後処理
			comPostEvent();
		}

	}

	/// <summary>
	/// ロード後処理
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void S02_0105_Shown(object sender, EventArgs e)
	{

		//チェック処理
		if (this.isValidDisp() == false)
		{

			this.F10Key_Enabled = false;
		}
	}

	/// <summary>
	/// 料金区分グリッドフォーカスイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdChargeList_BeforeEdit(object sender, RowColEventArgs e)
	{

		int rowIndex = System.Convert.ToInt32(e.Row);
		int colIndex = System.Convert.ToInt32(e.Col);

		if (this.grdChargeList.Styles.Contains(string.Format(CommonRegistYoyaku.GridStyleNameCombBox, System.Convert.ToString(rowIndex))) == true)
		{
			// 対象レコードの人員区分に、コンボボックスが付与されている場合、処理なし
			return;
		}

		switch (colIndex)
		{

			case 1:
				// 人員区分のセルを入力不可にする
				e.Cancel = true;
				return;
		}
	}

	/// <summary>
	/// 料金区分セルチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdChargeList_CellChanged(object sender, RowColEventArgs e)
	{

		int rowIndex = System.Convert.ToInt32(e.Row);
		int colIndex = System.Convert.ToInt32(e.Col);

		if (colIndex == 0)
		{
			// 料金区分の場合、人員区分のコンボボックス設定
			this.setChargeKbnJininCombo(rowIndex, colIndex);
		}
		else if (colIndex == 1)
		{
			// 人員区分の場合、料金設定
			this.setJininCharge(rowIndex, colIndex);
		}
	}

	#region フッタ

	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		base.btnF2_ClickOrgProc();

		this.Close();
	}

	/// <summary>
	/// F10：確定ボタン押下イベント
	/// </summary>
	protected override void btnF10_ClickOrgProc()
	{

		try
		{
			base.btnF10_ClickOrgProc();

			//サーバー日付の取得
			Hashtable sysDates = this.getSysDates();

			//予約区分,予約番号の初期化
			this._yoyakuKbn = "";
			this._yoyakuNo = 0;

			// エラー内容初期化
			this.initExistError();

			// 予約番号採番
			int yoyakuRes = this.numberingYoyakuNo();

			if (yoyakuRes != 0)
			{

				CommonProcess.createFactoryMsg().messageDisp("E90_025", "当日発券");
				return;
			}
			//End If

			//正規料金の集計
			//予約情報（コース料金_料金区分）の設定
			//入力値チェック
			if (this.aggregateGrdCharge(sysDates) == false)
			{

				return;
			}

			// オプションチェック
			if (this.isYoyakuOptionCheck() == false)
			{

				return;
			}

			//座席確保、予約登録
			if (this.canZasekiKakuhoAndRegistYoyaku(sysDates) == false)
			{
				//メッセージは処理内
				return;
			}

			//画面間パラメータの用意
			S02_0602ParamData prm = new S02_0602ParamData();
			prm.YoyakuKbn = this._yoyakuKbn;
			prm.YoyakuNo = this._yoyakuNo;

			// 処理成功時
			CommonProcess.createFactoryMsg().messageDisp("I90_002", "登録");
			// log出力
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.entry, ScreenName, "登録処理");

			//発券画面の展開
			bool isUpdate = false;
			using (S02_0602 s02_0602 = new S02_0602())
			{
				s02_0602.ParamData = prm;
				s02_0602.ShowDialog();
				isUpdate = System.Convert.ToBoolean(s02_0602.IsUpdated);
			}


			if (isUpdate == true)
			{
				//発券されていた場合は画面を閉じる
				this.Close();
			}
			else
			{
				// 発券していない場合、座席の取消、予約情報（基本）と予約情報（料金区分）を削除

				//座席取消処理
				this.canTorikeshiZaseki(sysDates);

			}

		}
		catch
		{

			// 失敗時
			CommonProcess.createFactoryMsg().messageDisp("E90_028", "登録");
			// log出力
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, ScreenName, "登録処理");
			throw;

		}

	}

	/// <summary>
	/// オプション必須選択一覧セルチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdRequiredSelectionList_CellChanged(object sender, RowColEventArgs e)
	{

		int rowIndex = System.Convert.ToInt32(e.Row);
		int colIndex = System.Convert.ToInt32(e.Col);

		if (colIndex == 3)
		{
			// 内容のカラムの場合
			string requiredKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(this.grdRequiredSelectionList.GetData(rowIndex, "REQUIRED_KBN")));
			if (string.IsNullOrEmpty(requiredKbn) == true)
			{
				// 必須区分が空の場合、処理終了
				return;
			}

			if (requiredKbn == CommonRegistYoyaku.convertEnumToString(FixedCd.HissuKbn.hitogoto))
			{
				// 必須区分が人毎の場合、処理終了
				return;
			}

			string groupNo = System.Convert.ToString(this.grdRequiredSelectionList.GetData(rowIndex, "GROUP_NO").ToString());
			string lineNo = System.Convert.ToString(this.grdRequiredSelectionList.GetData(rowIndex, colIndex).ToString());

			DataRow[] rows = this._yoyakuRequiredSelectionList.Select(string.Format("GROUP_NO = {0} AND LINE_NO = {1} ", groupNo, lineNo));

			this.grdRequiredSelectionList.SetData(rowIndex, "PAYMENT_HOHO", rows[0]["PAYMENT_HOHO"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "LINE_NO", lineNo);
			this.grdRequiredSelectionList.SetData(rowIndex, "TAX_KBN", rows[0]["TAX_KBN"]);

		}
		else if (colIndex == 4)
		{
			// 適用数の場合

			// オプションの各合計設定
			this.setOptionTotalMoney();
		}
	}

	/// <summary>
	/// オプション任意選択一覧セルチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdAnySelectionList_CellChanged(object sender, RowColEventArgs e)
	{

		// オプションの各合計設定
		this.setOptionTotalMoney();
	}

	#endregion

	#endregion

	#region メソッド

	/// <summary>
	/// 画面表示時の初期設定
	/// </summary>
	private void setControlInitiarize()
	{

		//ベースフォームの設定
		this.setFormId = PgmId;
		this.setTitle = "発券（当日）";

		//フッタボタンの設定
		this.setButtonInitiarize();

		//コース台帳（基本）の取得
		DataTable dt = this.getCrsLedgerBasic();
		if (ReferenceEquals(dt, null))
		{
			return;
		}
		this._crsLedgerBasic = dt;
		//場所マスタの取得
		this._placeMaster = this.getPlaceMaster(System.Convert.ToString(this.ParamData.JyosyatiCd));

		//コース情報の設定
		this.setCrsInfo();

		//料金区分グリッド(宿泊なし)の設定
		this.setGrdChargeKbn();
		this.setNewChargeKbnListForShukuhakuNashi();

		//発券グリッドの設定
		this.setGrdHakken();

		// オプション設定
		this.setOptionList();
	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{

		//Visible
		this.F2Key_Visible = true;
		this.F10Key_Visible = true;

		this.F1Key_Visible = false;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		this.F7Key_Visible = false;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;

		//Text
		this.F2Key_Text = "F2:戻る";
		this.F10Key_Text = "F10:確定";
	}

	/// <summary>
	/// コース情報の設定
	/// </summary>
	private void setCrsInfo()
	{
		string strSyuptDay = this.ParamData.SyuptDay.ToString().PadLeft(8, '0').Substring(0, 4) + "/" + this.ParamData.SyuptDay.ToString().PadLeft(8, '0').Substring(4, 2) + "/" + this.ParamData.SyuptDay.ToString().PadLeft(8, '0').Substring(6, 2);
		DateTime dtSyuptDay = DateTime.Parse(strSyuptDay);
		this.ucoSyuptDay.Value = dtSyuptDay;
		this.txtCrsCd.Text = this.ParamData.CrsCd;
		this.txtCrsName.Text = if (this._crsLedgerBasic.Rows(0).Field(Of string)["CRS_NAME"], "");
		this.txtJyosyaTiCd.Text = if (this.ParamData.JyosyatiCd, "");
		if (this._placeMaster IsNot null)
		{
			this.txtJyosyaTiName.Text = if (this._placeMaster.Rows(0).Field(Of string)["PLACE_NAME_SHORT"], "");
		}
		string strSyuptTime = this.ParamData.SyuptTime.ToString().PadLeft(4, '0').Substring(0, 2) + ":" + this.ParamData.SyuptTime.ToString().PadLeft(4, '0').Substring(2, 2);
		this.txtSyuptTime.Text = strSyuptTime;
	}

	/// <summary>
	/// 料金区分（宿泊なし）グリッドの設定
	/// </summary>
	private void setGrdChargeKbn()
	{
		//グリッドの設定
		this.grdChargeList.AllowDragging = AllowDraggingEnum.None;
		this.grdChargeList.AutoGenerateColumns = false;

	}

	/// <summary>
	/// 発券グリッドの設定
	/// </summary>
	private void setGrdHakken()
	{
		//グリッドの設定
		this.grdHakken.AllowDragging = AllowDraggingEnum.None;
		this.grdHakken.AutoGenerateColumns = false;

		this.grdHakken.Cols(1).AllowEditing = false;
		this.grdHakken.Cols(2).AllowEditing = false;
		this.grdHakken.Cols(3).AllowEditing = false;
		this.grdHakken.Cols(4).AllowEditing = false;
		this.grdHakken.Cols(5).AllowEditing = false;

		//コース台帳（基本）の整形
		DataView dv = this.formatCrsLedgerBasic();
		if (dv.Count == 0)
		{
			CommonProcess.createFactoryMsg.messageDisp("E90_019");
		}
		//データソースの設定
		this.grdHakken.DataSource = dv;
	}

	/// <summary>
	/// コース台帳（基本）の整形
	/// </summary>
	private DataView formatCrsLedgerBasic()
	{
		bool isHaisiCrs = false;
		bool isYoyakuNgCrs = false;

		//コース台帳（基本）データより整形
		foreach (DataRow dr in this._crsLedgerBasic.AsEnumerable())
		{
			//出発時間の整形
			string syuptTime = System.Convert.ToString(if (dr.Field(Of Short ?)["NUM_SYUPT_TIME"], 0).ToString().PadLeft(4, '0'));
			syuptTime = syuptTime.Substring(0, 2) + ":" + syuptTime.Substring(2, 2);
			dr.Item["SYUPT_TIME"] = syuptTime;

			//空席数の整形
			string kusekiNumTeiseki = System.Convert.ToString(if (dr.Field(Of Decimal ?)["KUSEKI_NUM_TEISEKI"], 0).ToString());
			string kusekiNumSubseat = System.Convert.ToString(if (dr.Field(Of Decimal ?)["KUSEKI_NUM_SUB_SEAT"], 0).ToString());
			dr.Item["KUSEKI_NUM"] = kusekiNumTeiseki + " + " + kusekiNumSubseat;

			string unkyuKbn = System.Convert.ToString(if (dr.Field(Of string)["UNKYU_KBN"], ""));
			if (unkyuKbn.Equals(FixedCd.UnkyuKbn.Unkyu))
			{
				//運休区分が"運休":経由を「運休」
				dr.Item["KEIYUTI"] = Unkyu;

			}
			else if (unkyuKbn.Equals(FixedCd.UnkyuKbn.Haishi))
			{
				//運休区分フラグ設定
				isHaisiCrs = true;
			}

			string yoyakuNgFlg = System.Convert.ToString(if (dr.Field(Of string)["YOYAKU_NG_FLG"], ""));
			if (!string.IsNullOrWhiteSpace(yoyakuNgFlg))
			{
				//予約不可フラグ設定
				isYoyakuNgCrs = true;
			}
		}

		object dt = this._crsLedgerBasic.AsEnumerable();

		//運休区分が"廃止":非表示、予約不可フラグがNothing以外:非表示
		DataView dv = dt.Where(Function(row)!if (row.Field(Of string)["UNKYU_KBN"], "").Equals(FixedCd.UnkyuKbn.Haishi) && ReferenceEquals(row.Field(Of string)["YOYAKU_NG_FLG"], null);
		).AsDataView();

		//表示できるデータがない場合
		if (dv.Count == 0)
		{
			if (isHaisiCrs)
			{
				//廃止
				CommonProcess.createFactoryMsg.messageDisp("E02_046", "廃止済み");
			}
			else if (isYoyakuNgCrs)
			{
				//予約不可
				CommonProcess.createFactoryMsg.messageDisp("E02_046", "予約不可");
			}
		}

		return dv;
	}

	/// <summary>
	/// オプション一覧の設定
	/// </summary>
	private void setOptionList()
	{

		S02_0103Da s02_0103Da = new S02_0103Da();

		CrsLedgerOptionGroupEntity entity = new CrsLedgerOptionGroupEntity();
		entity.crsCd.Value = this.ParamData.CrsCd;
		entity.syuptDay.Value = this.ParamData.SyuptDay;
		entity.gousya.Value = this.ParamData.Gousya;

		// 必須選択一覧設定
		this.setNewRequiredSelectionList(entity, s02_0103Da);

		// 任意選択一覧設定
		this.setNewAnySelectionList(entity, s02_0103Da);
	}

	/// <summary>
	/// オプション必須選択一覧設定
	/// </summary>
	/// <param name="entity">コース台帳（オプショングループ）Entity</param>
	/// <param name="s02_0103Da">データアクセスクラス</param>
	private void setNewRequiredSelectionList(CrsLedgerOptionGroupEntity entity, S02_0103Da s02_0103Da)
	{

		// 人毎の必須選択一覧取得
		entity.requiredKbn.Value = CommonRegistYoyaku.convertEnumToString(FixedCd.HissuKbn.hitogoto);
		this._personRequiredSelectionList = s02_0103Da.getRequiredSelectionList(entity);
		this._personRequiredSelectionList.Columns.Add("ADD_CHARGE_APPLICATION_NINZU");

		// 予約毎の必須選択一覧取得
		entity.requiredKbn.Value = CommonRegistYoyaku.convertEnumToString(FixedCd.HissuKbn.yoyakugoto);
		this._yoyakuRequiredSelectionList = s02_0103Da.getRequiredSelectionList(entity);
		this._yoyakuRequiredSelectionList.Columns.Add("ADD_CHARGE_APPLICATION_NINZU");

		// 必須選択一覧に空行を設定
		CommonRegistYoyaku.setEmptyRequiredSelectionList(this._yoyakuRequiredSelectionList, this._personRequiredSelectionList, this.grdRequiredSelectionList);

		// 人毎の必須選択一覧をグリッドに設定
		int rowIndex = System.Convert.ToInt32(CommonRegistYoyaku.GridStartRowIndex);
		foreach (DataRow row in this._personRequiredSelectionList.Rows)
		{

			this.grdRequiredSelectionList.SetData(rowIndex, "OPTION_GROUP_NM", row["OPTION_GROUP_NM"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "TANKA_KBN_NM", row["TANKA_KBN_NM"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "OPTIONAL_NAME", row["OPTIONAL_NAME"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "ADD_CHARGE_APPLICATION_NINZU", row["ADD_CHARGE_APPLICATION_NINZU"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "HANBAI_TANKA", row["HANBAI_TANKA"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "PAYMENT_HOHO", row["PAYMENT_HOHO"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "REQUIRED_KBN", row["REQUIRED_KBN"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "GROUP_NO", row["GROUP_NO"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "TANKA_KBN", row["TANKA_KBN"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "LINE_NO", row["LINE_NO"]);
			this.grdRequiredSelectionList.SetData(rowIndex, "TAX_KBN", row["TAX_KBN"]);

			// 行インクリメント
			rowIndex++;
		}

		List list = new List(Of string);
		string groupNo = "";

		foreach (DataRow items in this._yoyakuRequiredSelectionList.Rows)
		{

			groupNo = System.Convert.ToString(items["GROUP_NO"].ToString());

			if (list.Any(Function(x) x == groupNo))
			{
				// 重複するグループNoの場合、次レコードへ
				continue;
			}

			DataRow[] rows = this._yoyakuRequiredSelectionList.Select(string.Format("GROUP_NO = {0}", groupNo));

			Dictionary dic = new Dictionary(Of string, string);
			foreach (DataRow row in rows)
			{

				this.grdRequiredSelectionList.SetData(rowIndex, "OPTION_GROUP_NM", row["OPTION_GROUP_NM"]);
				this.grdRequiredSelectionList.SetData(rowIndex, "TANKA_KBN_NM", row["TANKA_KBN_NM"]);
				this.grdRequiredSelectionList.SetData(rowIndex, "ADD_CHARGE_APPLICATION_NINZU", row["ADD_CHARGE_APPLICATION_NINZU"]);
				this.grdRequiredSelectionList.SetData(rowIndex, "HANBAI_TANKA", row["HANBAI_TANKA"]);

				this.grdRequiredSelectionList.SetData(rowIndex, "REQUIRED_KBN", row["REQUIRED_KBN"]);
				this.grdRequiredSelectionList.SetData(rowIndex, "GROUP_NO", row["GROUP_NO"]);
				this.grdRequiredSelectionList.SetData(rowIndex, "TANKA_KBN", row["TANKA_KBN"]);

				dic.Add(row["LINE_NO"].ToString(), row["OPTIONAL_NAME"].ToString());
			}

			CellRange rg;
			// カスタムスタイル"Combo1"を作成
			string customName = string.Format(CommonRegistYoyaku.GridStyleNameCombBox, System.Convert.ToString(rowIndex));
			this.grdRequiredSelectionList.Styles.Add(customName);
			// ComboListプロパティを設定
			this.grdRequiredSelectionList.Styles(customName).DataType = typeof(string);
			this.grdRequiredSelectionList.Styles(customName).DataMap = dic;
			// 内容セルを選択
			rg = this.grdRequiredSelectionList.GetCellRange(rowIndex, 3);
			// カスタムスタイルを割り当てます
			rg.Style = this.grdRequiredSelectionList.Styles(customName);

			// 初期値設定
			this.grdRequiredSelectionList[rowIndex, 3] = dic.First().Key;

			// 適用数の背景色変更
			CellStyle cellStyle = this.grdRequiredSelectionList.Styles.Add(string.Format("NewStyle{0}", rowIndex));
			cellStyle.BackColor = FixedCdYoyaku.ControlColor.ControlLight;
			this.grdRequiredSelectionList.SetCellStyle(rowIndex, 4, cellStyle);

			// グループNo格納
			list.Add(groupNo);
			// 行インクリメント
			rowIndex++;
		}
	}

	/// <summary>
	/// オプション任意選択一覧設定
	/// </summary>
	/// <param name="entity">コース台帳（オプショングループ）Entity</param>
	/// <param name="s02_0103Da">データアクセスクラス</param>
	private void setNewAnySelectionList(CrsLedgerOptionGroupEntity entity, S02_0103Da s02_0103Da)
	{

		// 任意のみ
		entity.requiredKbn.Value = CommonRegistYoyaku.convertEnumToString(FixedCd.HissuKbn.any);

		// オプション任意選択一覧取得
		this._anySelectionList = s02_0103Da.getAnySelectionList(entity);
		this._anySelectionList.Columns.Add("ADD_CHARGE_APPLICATION_NINZU");

		CommonRegistYoyaku.setGridLength(this.grdAnySelectionList, "ADD_CHARGE_APPLICATION_NINZU", CommonRegistYoyaku.NinzuMaxLength);

		// 任意選択一覧バインド
		this.grdAnySelectionList.DataSource = this._anySelectionList;
	}


	#region 料金区分
	/// <summary>
	/// 新規登録用
	/// 料金区分設定(宿泊なし)
	/// </summary>
	private void setNewChargeKbnListForShukuhakuNashi()
	{

		this.grdChargeList.AllowAddNew = true;
		// 空行バインド
		this.grdChargeList.DataSource = CommonRegistYoyaku.createEmptyDataTableForC1FlexGrid(this.grdChargeList);

		CommonRegistYoyaku.setGridLength(this.grdChargeList, "YOYAKU_NINZU", CommonRegistYoyaku.NinzuMaxLength);

		// 料金区分取得
		this._chargeKbnList = CommonRegistYoyaku.getChargeKbnList(this.ParamData.CrsCd, this.ParamData.SyuptDay, this.ParamData.Gousya);

		Dictionary[,] dic = CommonRegistYoyaku.createChargeKbnComboList(this._chargeKbnList);

		this.grdChargeList.Cols("KBN_NO").DataType = typeof(string);
		this.grdChargeList.Cols("KBN_NO").DataMap = dic;
	}

	/// <summary>
	/// コースパラメータ作成
	/// </summary>
	/// <param name="crsCd">コースコード</param>
	/// <param name="gousya">号車</param>
	/// <param name="syuptDay">出発日</param>
	/// <returns></returns>
	private Hashtable createCrsParamList(string crsCd, int gousya, int syuptDay)
	{

		// パラメータ
		Hashtable paramInfoList = new Hashtable();

		paramInfoList.Add("CRS_CD", crsCd);
		paramInfoList.Add("GOUSYA", gousya);
		paramInfoList.Add("SYUPT_DAY", syuptDay);

		return paramInfoList;
	}

	/// <summary>
	/// 料金区分コンボボックス値作成
	/// </summary>
	/// <param name="chargeList">料金区分情報</param>
	/// <returns>料金区分コンボボックス値</returns>
	private Dictionary[,] createChargeKbnComboList(DataTable chargeList)
	{

		Dictionary dic = new Dictionary(Of string, string);
		// 空行追加
		dic.Add("", "");

		string kbnNo = "";
		foreach (DataRow row in chargeList.Rows)
		{

			if (kbnNo == row["KBN_NO"].ToString())
			{
				// 重複する区分Noは、追加しない
				continue;
			}

			dic.Add(row["KBN_NO"].ToString(), row["CHARGE_NAME"].ToString());
			// 前回分として区分Noを保持
			kbnNo = System.Convert.ToString(row["KBN_NO"].ToString());
		}

		return dic;
	}
	#endregion

	#region 料金区分(人員）
	/// <summary>
	/// 料金区分(人員)コンボボックス値設定
	/// </summary>
	/// <param name="rowIndex">行インデックス</param>
	/// <param name="colIndex">列インデックス</param>
	private void setChargeKbnJininCombo(int rowIndex, int colIndex)
	{

		// 区分No取得
		string kbnNo = System.Convert.ToString(this.grdChargeList.GetData(rowIndex, colIndex).ToString());
		// 料金区分(人員)コンボボックス値作成
		Dictionary[,] dic = CommonRegistYoyaku.createChargeKbnJininComboList(kbnNo, this._chargeKbnList);

		CellRange rg;
		// カスタムスタイル"Combo1"を作成
		string customName = string.Format(CommonRegistYoyaku.GridStyleNameCombBox, System.Convert.ToString(rowIndex));
		this.grdChargeList.Styles.Add(customName);
		// ComboListプロパティを設定
		this.grdChargeList.Styles(customName).DataType = typeof(string);
		this.grdChargeList.Styles(customName).DataMap = dic;
		// 人員セルを選択
		rg = this.grdChargeList.GetCellRange(rowIndex, colIndex + 1);
		// カスタムスタイルを割り当てます
		rg.Style = this.grdChargeList.Styles(customName);

		// 料金区分を変更したら、人員、料金、人数を初期化
		this.grdChargeList.SetData(rowIndex, "CHARGE_KBN_JININ_CD", CommonRegistYoyaku.ValueEmpty);
		this.grdChargeList.SetData(rowIndex, "CHARGE", CommonRegistYoyaku.ValueEmpty);
		this.grdChargeList.SetData(rowIndex, "YOYAKU_NINZU", CommonRegistYoyaku.ValueEmpty);
		this.grdChargeList.SetData(rowIndex, "CHARGE_KBN_JININ_CD_URA", CommonRegistYoyaku.ValueEmpty);
		this.grdChargeList.SetData(rowIndex, "SEX_BETU", CommonRegistYoyaku.ValueEmpty);

		if (string.IsNullOrEmpty(kbnNo) == true)
		{
			// 女性料金フラグ、宿泊付フラグ、食事付フラグを初期化
			this.grdChargeList.SetData(rowIndex, "JYOSEI_CHARGE_FLG", CommonRegistYoyaku.ValueEmpty);
			this.grdChargeList.SetData(rowIndex, "STAY_ADD_FLG", CommonRegistYoyaku.ValueEmpty);
			this.grdChargeList.SetData(rowIndex, "MEAL_ADD_FLG", CommonRegistYoyaku.ValueEmpty);
			this.grdChargeList.SetData(rowIndex, "KBN_NO_URA", CommonRegistYoyaku.ValueEmpty);
			return;
		}

		DataRow[] rows = this._chargeKbnList.Select(string.Format("KBN_NO = {0}", kbnNo));
		this.grdChargeList.SetData(rowIndex, "JYOSEI_CHARGE_FLG", rows[0]["JYOSEI_CHARGE_FLG"].ToString());
		this.grdChargeList.SetData(rowIndex, "STAY_ADD_FLG", rows[0]["STAY_ADD_FLG"].ToString());
		this.grdChargeList.SetData(rowIndex, "MEAL_ADD_FLG", rows[0]["MEAL_ADD_FLG"].ToString());
		this.grdChargeList.SetData(rowIndex, "KBN_NO_URA", kbnNo);
	}

	/// <summary>
	/// 料金区分一覧料金設定
	/// </summary>
	/// <param name="rowIndex">行インデックス</param>
	/// <param name="colIndex">列インデックス</param>
	private void setJininCharge(int rowIndex, int colIndex)
	{

		// 区分No
		string kbnNo = System.Convert.ToString(this.grdChargeList.GetData(rowIndex, colIndex - 1).ToString());
		// 人員コード取得
		string jininCd = System.Convert.ToString(this.grdChargeList.GetData(rowIndex, colIndex).ToString());

		if (string.IsNullOrEmpty(jininCd))
		{
			// 人員コードが空の場合、料金を空にする
			this.grdChargeList.SetData(rowIndex, "CHARGE", CommonRegistYoyaku.ValueEmpty);
			this.grdChargeList.SetData(rowIndex, "CHARGE_SUB_SEAT", CommonRegistYoyaku.ValueEmpty);
			this.grdChargeList.SetData(rowIndex, "CHARGE_KBN", CommonRegistYoyaku.ValueEmpty);
			this.grdChargeList.SetData(rowIndex, "CARRIAGE", CommonRegistYoyaku.ValueEmpty);
			this.grdChargeList.SetData(rowIndex, "CARRIAGE_SUB_SEAT", CommonRegistYoyaku.ValueEmpty);
			this.grdChargeList.SetData(rowIndex, "CHARGE_KBN_JININ_CD_URA", CommonRegistYoyaku.ValueEmpty);
			this.grdChargeList.SetData(rowIndex, "SEX_BETU", CommonRegistYoyaku.ValueEmpty);

			return;
		}

		// 区分No、人員コードをキーに、料金取得
		DataRow[] rows = this._chargeKbnList.Select(string.Format("KBN_NO = '{0}' AND CHARGE_KBN_JININ_CD = '{1}' ", kbnNo, jininCd));
		// 料金取得
		int charge = System.Convert.ToInt32(CommonRegistYoyaku.ZERO);
		// 定期の場合、料金から表示する
		charge = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(rows[0]["CHARGE"]));

		// 料金設定
		this.grdChargeList.SetData(rowIndex, "CHARGE", charge.ToString(System.Convert.ToString(CommonRegistYoyaku.MoneyFormat)));
		this.grdChargeList.SetData(rowIndex, "CHARGE_SUB_SEAT", rows[0]["CHARGE_SUB_SEAT"]);
		this.grdChargeList.SetData(rowIndex, "CHARGE_KBN", rows[0]["CHARGE_KBN"]);
		this.grdChargeList.SetData(rowIndex, "CARRIAGE", rows[0]["CARRIAGE"]);
		this.grdChargeList.SetData(rowIndex, "CARRIAGE_SUB_SEAT", rows[0]["CARRIAGE_SUB_SEAT"]);
		this.grdChargeList.SetData(rowIndex, "CHARGE_KBN_JININ_CD_URA", jininCd);
		this.grdChargeList.SetData(rowIndex, "SEX_BETU", rows[0]["SEX_BETU"]);
	}
	#endregion

	/// <summary>
	/// 料金グリッドの集計
	/// </summary>
	private bool aggregateGrdCharge(Hashtable sysDates)
	{

		this._yoyakuNinzu = 0;
		this._seikiChargeAllGaku = 0;
		RegularExpressions.Regex regex = new RegularExpressions.Regex(CommonHakken.RegexNotNumber); //正規表現（数値以外）

		foreach (Row row in this.grdChargeList.Rows)
		{
			//ヘッダー行なら次の行へ
			if (row.Index == 0)
			{
				continue;
			}

			//料金区分がnullなら終了
			if (Information.IsDBNull(row.Item("KBN_NO")) || ReferenceEquals(row.Item("KBN_NO"), null) || row.Item("KBN_NO").ToString().Equals(""))
			{
				break;
			}

			//人員がnullならエラー
			if (Information.IsDBNull(row.Item("CHARGE_KBN_JININ_CD")) || ReferenceEquals(row.Item("CHARGE_KBN_JININ_CD"), null) || row.Item("CHARGE_KBN_JININ_CD").ToString().Equals(""))
			{

				this.grdChargeList.Styles(string.Format(System.Convert.ToString(CommonRegistYoyaku.GridStyleNameCombBox), row.Index)).BackColor = Color.Red;
				CommonProcess.createFactoryMsg.messageDisp("E90_044", "人員");
				return false;
			}

			//人数がnullならエラー
			if (Information.IsDBNull(row.Item("YOYAKU_NINZU")) || ReferenceEquals(row.Item("YOYAKU_NINZU"), null) || row.Item("YOYAKU_NINZU").ToString().Equals(""))
			{

				CommonRegistYoyaku.changeGridBackColor(row.Index, 3, this.grdChargeList);
				CommonProcess.createFactoryMsg.messageDisp("E70_003", "人数", "0以外");
				return false;
			}

			//値の取り出し
			string strCharge = System.Convert.ToString(row.Item("CHARGE").ToString());
			strCharge = System.Convert.ToString(regex.Replace(strCharge, ""));
			int charge = int.Parse(strCharge);
			int ninzu = 0;
			int.TryParse(System.Convert.ToString(row.Item("YOYAKU_NINZU").ToString()), out ninzu);

			//入力チェック
			if (ninzu == 0)
			{

				CommonRegistYoyaku.changeGridBackColor(row.Index, 3, this.grdChargeList);
				CommonProcess.createFactoryMsg.messageDisp("E70_003", "人数", "0以外");
				return false;
			}

			//予約人数へ加算
			this._yoyakuNinzu += ninzu;
		}

		//入力値チェック
		if (this._yoyakuNinzu == 0)
		{
			CommonProcess.createFactoryMsg.messageDisp("E70_003", "人数", "0以外");
			return false;
		}

		return true;
	}

	/// <summary>
	/// オプションチェック
	/// </summary>
	/// <returns>検証結果</returns>
	private bool isYoyakuOptionCheck()
	{

		if (this.isRequiredSelectionForNinzuUnit(this._yoyakuNinzu) == false)
		{

			this.grdRequiredSelectionList.Focus();
			CommonProcess.createFactoryMsg.messageDisp("E90_011", "オプション：必須選択一覧の適用数");
			return false;
		}

		if (this.isAnySelectionForNinzuUnit(this._yoyakuNinzu) == false)
		{

			this.grdAnySelectionList.Focus();
			CommonProcess.createFactoryMsg.messageDisp("E90_011", "オプション：任意選択一覧の適用数");
			return false;
		}

		return true;
	}

	/// <summary>
	/// 適用数(必須選択一覧）必須チェック
	/// </summary>
	/// <param name="yoyakuNinzu">予約人数</param>
	/// <returns>検証結果</returns>
	private bool isRequiredSelectionForNinzuUnit(int yoyakuNinzu)
	{

		if (this._personRequiredSelectionList.Rows.Count <= 0)
		{
			// 人数単位の必須選択がない場合、チェックなし
			return true;
		}

		int reqNinzu = 0;
		foreach (Row reqRow in this.grdRequiredSelectionList.Rows)
		{

			if (reqRow.Index == CommonRegistYoyaku.ZERO)
			{
				// ヘッダ行の場合、次レコードへ
				continue;
			}

			string requiredKbn = System.Convert.ToString(this.grdRequiredSelectionList.GetData(reqRow.Index, "REQUIRED_KBN").ToString());
			int ninzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(this.grdRequiredSelectionList.GetData(reqRow.Index, "ADD_CHARGE_APPLICATION_NINZU")));

			if (requiredKbn != CommonRegistYoyaku.convertEnumToString(FixedCd.HissuKbn.hitogoto))
			{
				// 必須区分が「必須(人毎)」でない場合、次レコードへ
				continue;
			}

			reqNinzu = reqNinzu + ninzu;
		}

		if (reqNinzu != yoyakuNinzu)
		{

			this.changeRequiredSelectionListGrid();

			return false;
		}

		return true;
	}

	/// <summary>
	/// 必須選択一覧背景色変更
	/// </summary>
	private void changeRequiredSelectionListGrid()
	{

		foreach (Row reqRow in this.grdRequiredSelectionList.Rows)
		{

			if (reqRow.Index == CommonRegistYoyaku.ZERO)
			{
				// ヘッダ行の場合、次レコードへ
				continue;
			}

			string requiredKbn = System.Convert.ToString(this.grdRequiredSelectionList.GetData(reqRow.Index, "REQUIRED_KBN").ToString());

			if (requiredKbn != CommonRegistYoyaku.convertEnumToString(FixedCd.HissuKbn.hitogoto))
			{
				// 必須区分が「必須(人毎)」でない場合、次レコードへ
				continue;
			}

			CommonRegistYoyaku.changeGridBackColor(reqRow.Index, 4, this.grdRequiredSelectionList);
		}
	}

	/// <summary>
	/// 適用数（任意選択一覧）チェック
	/// </summary>
	/// <param name="yoyakuNinzu">予約人数</param>
	/// <returns>検証結果</returns>
	private bool isAnySelectionForNinzuUnit(int yoyakuNinzu)
	{

		if (this._anySelectionList.Rows.Count <= 0)
		{
			// 任意選択一覧が0件の場合、チェックなし
			return true;
		}

		foreach (Row row in this.grdAnySelectionList.Rows)
		{

			if (row.Index == CommonRegistYoyaku.ZERO)
			{
				// ヘッダー行の場合、次レコードへ
				continue;
			}

			int ninzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(this.grdAnySelectionList.GetData(row.Index, "ADD_CHARGE_APPLICATION_NINZU")));
			if (ninzu <= CommonRegistYoyaku.ZERO)
			{

				continue;
			}

			string tankaKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(this.grdAnySelectionList.GetData(row.Index, "TANKA_KBN")));
			// 当日発券は、定期のみなのでエラールーム単価のチェックはしない
			if (tankaKbn != CommonRegistYoyaku.convertEnumToString(FixedCd.TankaKbnType.roomTanka))
			{

				if (ninzu > yoyakuNinzu)
				{
					// 人数単位人数が予約人数より超えている場合
					CommonRegistYoyaku.changeGridBackColor(row.Index, 5, this.grdAnySelectionList);
					return false;
				}
			}
		}

		return true;
	}

	#region DataAccess
	/// <summary>
	/// コース台帳（基本）の取得
	/// </summary>
	/// <returns></returns>
	private DataTable getCrsLedgerBasic()
	{
		//エンティティの設定
		CrsLedgerBasicEntity ent = this.createCrsLedgerBasicEntity();

		S02_0105Da da = new S02_0105Da();
		DataTable dt = da.getCrsLedgerBasic(ent);

		return if (CommonHakken.existsDatas(dt), dt, null);
	}

	/// <summary>
	/// 場所マスタの取得
	/// </summary>
	/// <returns></returns>
	private DataTable getPlaceMaster(string placeCd)
	{
		S02_0105Da da = new S02_0105Da();
		DataTable dt = da.getMPlace(this.ParamData.JyosyatiCd);

		return if (CommonHakken.existsDatas(dt), dt, null);
	}

	/// <summary>
	/// 他営業所の判定
	/// </summary>
	/// <returns></returns>
	private bool isOtherEigyosyo()
	{
		//エンティティの設定
		CrsLedgerBasicEntity ent = this.createCrsLedgerBasicEntity();

		S02_0105Da da = new S02_0105Da();
		DataTable dt = da.getCountMeEigyoSyoCrs(ent);

		decimal cntMeEigyosyoCrs = System.Convert.ToDecimal(dt.Rows(0).Field(Of decimal)["COUNT_ME_EIGYOSYO_CRS"]);
		bool isOtherEigyoSyoCrs = if (cntMeEigyosyoCrs == 0, true, false);

		return isOtherEigyoSyoCrs;
	}

	/// <summary>
	/// コース台帳（基本）エンティティの作成
	/// </summary>
	/// <returns></returns>
	private CrsLedgerBasicEntity createCrsLedgerBasicEntity()
	{
		//エンティティの設定
		CrsLedgerBasicEntity ent = new CrsLedgerBasicEntity();

		ent.syuptDay.Value = this.ParamData.SyuptDay;
		ent.crsCd.Value = this.ParamData.CrsCd;
		ent.gousya.Value = this.ParamData.Gousya;
		ent.haisyaKeiyuCd1.Value = this.ParamData.JyosyatiCd;

		return ent;
	}

	#endregion

	#region エンティティの作成
	/// <summary>
	/// 発券（当日）グループエンティティの作成
	/// </summary>
	/// <returns></returns>
	private HakkenTojituGroupEntity createHakkenTojituGroup(Hashtable sysDates)
	{

		// 予約情報（コース料金_料金区分)エンティティ作成
		List[] yoyakuInfoCrsChargeChargeKbnList = this.createYoyakuInfoCrsChargeChargeKbnList(sysDates, this._z0001Result);
		// 予約情報（コース料金）エンティティの作成
		YoyakuInfoCrsChargeEntity yoyakuInfoCrsCharge = this.createYoyakuInfoCrsChargeEntity(sysDates);
		// 予約情報（基本）エンティティの作成
		YoyakuInfoBasicEntity yoyakuInfoBasic = this.createYoyakuInfoBasicEntity(sysDates);
		// 予約情報（オプション）エンティティ作成
		List[] YoyakuInfoOptionList = this.createYoyakuInfoOption(sysDates);

		HakkenTojituGroupEntity hakkenTojituGroupEntity = new HakkenTojituGroupEntity();
		hakkenTojituGroupEntity.YoyakuInfoBasicEntity = yoyakuInfoBasic;
		hakkenTojituGroupEntity.YoyakuInfoCrsChargeEntity = yoyakuInfoCrsCharge;
		hakkenTojituGroupEntity.YoyakuInfoCrsChargeChargeKbnEntityList = yoyakuInfoCrsChargeChargeKbnList;
		hakkenTojituGroupEntity.YoyakuInfoOptionList = YoyakuInfoOptionList;

		return hakkenTojituGroupEntity;
	}

	/// <summary>
	/// エラー内容初期化
	/// </summary>
	private void initExistError()
	{

		CommonRegistYoyaku.initGirdExistError(this.grdChargeList);
		CommonRegistYoyaku.removeGridBackColorStyle(this.grdChargeList);
		CommonRegistYoyaku.removeGridBackColorStyle(this.grdRequiredSelectionList);
		CommonRegistYoyaku.removeGridBackColorStyle(this.grdAnySelectionList);
	}

	/// <summary>
	/// 予約NOの採番
	/// </summary>
	private int numberingYoyakuNo()
	{
		string houjinGaikyakuKbn = System.Convert.ToString(this._crsLedgerBasic.Rows(0).Field(Of string)["HOUJIN_GAIKYAKU_KBN"]);

		numberingYoyakuNoParam param = new numberingYoyakuNoParam();
		param.crsKind = this.convertStringToEnum(Of FixedCdYoyaku.CrsKind)[this.ParamData.CrsKind];
		param.houjinGaikyakuKbn = houjinGaikyakuKbn;

		// 予約番号の採番
		int result = System.Convert.ToInt32(YoyakuBizCommon.numberingYoyakuNo(param));

		if (result == 0)
		{

			this._yoyakuKbn = System.Convert.ToString(param.yoyakuKbn);
			this._yoyakuNo = System.Convert.ToInt32(param.yoyakuNo);
		}

		return result;
	}

	/// <summary>
	/// 予約情報（基本）エンティティの作成
	/// </summary>
	/// <returns></returns>
	private YoyakuInfoBasicEntity createYoyakuInfoBasicEntity(Hashtable sysdates)
	{

		YoyakuInfoBasicEntity ent = new YoyakuInfoBasicEntity();

		ent.yoyakuKbn.Value = this._yoyakuKbn;
		ent.yoyakuNo.Value = this._yoyakuNo;

		string strYear = System.Convert.ToString(this.ParamData.SyuptDay.ToString().Substring(0, 4));
		ent.year.Value = int.Parse(strYear);
		ent.syuptDay.Value = this.ParamData.SyuptDay;
		ent.crsCd.Value = this.ParamData.CrsCd;
		ent.teikiKikakuKbn.Value = CommonHakken.convertEnumToString(FixedCd.TeikiKikakuKbn.Teiki);

		ent.crsKind.Value = CommonHakken.convertEnumToString(FixedCd.CrsKind1Type.teikiKanko);
		ent.crsKbn1.Value = this._crsLedgerBasic.Rows(0).Field(Of string)["CRS_KBN_1"];
		ent.crsKbn2.Value = this._crsLedgerBasic.Rows(0).Field(Of string)["CRS_KBN_2"];
		ent.gousya.Value = this.ParamData.Gousya;

		// 乗車地、乗車人数
		string haisyaKeiyutiKey = System.Convert.ToString(this._crsLedgerBasic.Rows(0).Field(Of string)["HAISYA_KEIYUTI_KEY"]);
		switch (haisyaKeiyutiKey)
		{

			case "1":

				ent.jyochachiCd1.Value = if (this.ParamData.JyosyatiCd, "");
				ent.jyosyaNinzu1.Value = this._yoyakuNinzu;
				break;

			case "2":

				ent.jyochachiCd2.Value = if (this.ParamData.JyosyatiCd, "");
				ent.jyosyaNinzu2.Value = this._yoyakuNinzu;
				break;

			case "3":

				ent.jyochachiCd3.Value = if (this.ParamData.JyosyatiCd, "");
				ent.jyosyaNinzu3.Value = this._yoyakuNinzu;
				break;

			case "4":

				ent.jyochachiCd4.Value = if (this.ParamData.JyosyatiCd, "");
				ent.jyosyaNinzu4.Value = this._yoyakuNinzu;
				break;

			case "5":

				ent.jyochachiCd5.Value = if (this.ParamData.JyosyatiCd, "");
				ent.jyosyaNinzu5.Value = this._yoyakuNinzu;
				break;
		}

		ent.name.Value = CommonHakken.NameTojitu;
		// 正規料金総額
		ent.seikiChargeAllGaku.Value = this._seikiChargeAllGaku;
		// 精算方法
		ent.seisanHoho.Value = CommonHakken.convertEnumToString(FixedCdYoyaku.PaymentHoho.tojituPayment);
		// グループNO
		ent.groupNo.Value = this._z0001Result.GroupNo;
		// とび席フラグ
		ent.tobiSeatFlg.Value = this._z0001Result.TobiSeatKbn;
		// 予約座席取得区分
		ent.yoyakuZasekiGetKbn.Value = this._z0001Result.YoyakuZasekiGetKbn;
		// 予約座席区分
		ent.yoyakuZasekiKbn.Value = this._z0001Result.SeatKbn;
		// 関連予約区分
		ent.relationYoyakuKbn.Value = this._yoyakuKbn;
		// 関連予約NO
		ent.relationYoyakuNo.Value = this._yoyakuNo;
		// 追加料金前払計
		ent.addChargeMaebaraiKei.Value = this._jizenAmount;
		// 追加料金当日払計
		ent.addChargeTojituPaymentKei.Value = this._tojituAmount;

		//共通カラム
		ent.deleteDay.Value = 0;
		ent.entryDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
		ent.entryTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
		ent.entryPersonCd.Value = UserInfoManagement.userId;
		ent.entryPgmid.Value = PgmId;
		ent.updateDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
		ent.updateTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
		ent.updatePersonCd.Value = UserInfoManagement.userId;
		ent.updatePgmid.Value = PgmId;
		ent.systemUpdateDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
		ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
		ent.systemUpdatePgmid.Value = PgmId;
		ent.systemEntryDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
		ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
		ent.systemEntryPgmid.Value = PgmId;
		return ent;
	}

	/// <summary>
	/// 予約情報（コース料金）エンティティの作成
	/// </summary>
	/// <returns></returns>
	private YoyakuInfoCrsChargeEntity createYoyakuInfoCrsChargeEntity(Hashtable sysdates)
	{
		YoyakuInfoCrsChargeEntity ent = new YoyakuInfoCrsChargeEntity();

		ent.yoyakuKbn.Value = this._yoyakuKbn;
		ent.yoyakuNo.Value = this._yoyakuNo;
		ent.year.Value = int.Parse(System.Convert.ToString(this.ParamData.SyuptDay.ToString().Substring(0, 4)));

		ent.cancelRyou.Value = 0;
		ent.cancelPer.Value = 0;

		//共通カラム
		ent.deleteDay.Value = 0;
		ent.entryDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
		ent.entryTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
		ent.entryPersonCd.Value = UserInfoManagement.userId;
		ent.entryPgmid.Value = PgmId;
		ent.updateDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
		ent.updateTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
		ent.updatePersonCd.Value = UserInfoManagement.userId;
		ent.updatePgmid.Value = PgmId;
		ent.systemUpdateDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
		ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
		ent.systemUpdatePgmid.Value = PgmId;
		ent.systemEntryDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
		ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
		ent.systemEntryPgmid.Value = PgmId;
		return ent;
	}

	/// <summary>
	/// 予約情報（コース料金_料金区分)エンティティ作成
	/// </summary>
	/// <param name="sysdates"></param>
	/// <param name="z0001Result">自動座席取得結果</param>
	/// <returns>予約情報（コース料金_料金区分)エンティティ</returns>
	private List[] createYoyakuInfoCrsChargeChargeKbnList(Hashtable sysdates, Z0001_Result z0001Result)
	{

		List list = new List();

		// 正規料金総額初期化
		this._seikiChargeAllGaku = 0;

		foreach (Row row in this.grdChargeList.Rows)
		{

			if (row.Index == CommonRegistYoyaku.ZERO)
			{
				// ヘッダー行の場合、次レコードへ
				continue;
			}

			//料金区分がnullなら終了
			if (Information.IsDBNull(row.Item("KBN_NO")) || ReferenceEquals(row.Item("KBN_NO"), null) || row.Item("KBN_NO").ToString().Equals(""))
			{
				break;
			}

			YoyakuInfoCrsChargeChargeKbnEntity ent = new YoyakuInfoCrsChargeChargeKbnEntity();

			//PK
			// 予約区分
			ent.yoyakuKbn.Value = this._yoyakuKbn;
			// 予約NO
			ent.yoyakuNo.Value = this._yoyakuNo;
			// 区分NO
			ent.kbnNo.Value = int.Parse(System.Convert.ToString(row["KBN_NO"].ToString()));
			// 料金区分（人員）コード
			ent.chargeKbnJininCd.Value = row["CHARGE_KBN_JININ_CD"].ToString();

			// 料金区分
			ent.chargeKbn.Value = row["CHARGE_KBN"].ToString();
			// 適用人数１
			ent.chargeApplicationNinzu1.Value = CommonRegistYoyaku.convertObjectToInteger(row["YOYAKU_NINZU"]);
			// 適用人数
			ent.chargeApplicationNinzu.Value = CommonRegistYoyaku.convertObjectToInteger(row["YOYAKU_NINZU"]);

			// 人数
			int ninzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(row["YOYAKU_NINZU"]));
			// 料金
			int charge = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(row["CHARGE"].ToString().Replace(",", "")));
			// 料金_補助席
			int chargeSubSeat = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(row["CHARGE_SUB_SEAT"]));
			// 運賃
			int carriage = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(row["CARRIAGE"]));
			// 運賃補助席
			int carriageSubSeat = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(row["CARRIAGE_SUB_SEAT"]));

			if (string.IsNullOrEmpty(System.Convert.ToString(this._z0001Result.SeatKbn)) == true)
			{
				// 定席の場合

				// 単価
				ent.tanka1.Value = charge;
				// 運賃
				ent.carriage.Value = carriage;

				// 正規料金総額上乗せ
				this._seikiChargeAllGaku += charge * ninzu;
			}
			else
			{
				// 補助席の場合

				// 単価
				ent.tanka1.Value = chargeSubSeat;
				// 運賃
				ent.carriage.Value = carriageSubSeat;

				// 正規料金総額上乗せ
				this._seikiChargeAllGaku += chargeSubSeat * ninzu;
			}

			// キャンセル人数１
			ent.cancelNinzu1.Value = 0;
			// キャンセル人数
			ent.cancelNinzu.Value = 0;

			//共通カラム
			ent.deleteDay.Value = 0;
			ent.entryDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
			ent.entryTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
			ent.entryPersonCd.Value = UserInfoManagement.userId;
			ent.entryPgmid.Value = PgmId;
			ent.updateDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
			ent.updateTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
			ent.updatePersonCd.Value = UserInfoManagement.userId;
			ent.updatePgmid.Value = PgmId;
			ent.systemUpdateDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
			ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
			ent.systemUpdatePgmid.Value = PgmId;
			ent.systemEntryDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
			ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
			ent.systemEntryPgmid.Value = PgmId;

			list.Add(ent);
		}

		return list;
	}

	/// <summary>
	/// 予約情報（オプション）エンティティ作成
	/// </summary>
	/// <param name="sysdates"></param>
	/// <returns>予約情報（オプション）エンティティ</returns>
	private List[] createYoyakuInfoOption(Hashtable sysdates)
	{

		List list = new List();
		YoyakuInfoOptionEntity entity = null;

		// 必須選択
		foreach (Row row in this.grdRequiredSelectionList.Rows)
		{

			if (row.Index == CommonRegistYoyaku.ZERO)
			{
				// ヘッダー行の場合、次レコードへ
				continue;
			}

			entity = new YoyakuInfoOptionEntity();

			// 予約区分
			entity.yoyakuKbn.Value = this._yoyakuKbn;
			// 予約NO
			entity.yoyakuNo.Value = this._yoyakuNo;
			// 年
			string strYear = System.Convert.ToString(this.ParamData.SyuptDay.ToString().Substring(0, 4));
			entity.year.Value = int.Parse(strYear);
			// グループNO
			entity.groupNo.Value = int.Parse(System.Convert.ToString(row["GROUP_NO"].ToString()));
			// 行番号
			entity.lineNo.Value = int.Parse(System.Convert.ToString(row["LINE_NO"].ToString()));
			// 必須区分
			entity.hisuKbn.Value = int.Parse(System.Convert.ToString(row["REQUIRED_KBN"].ToString()));
			// 単価区分
			entity.tankaKbn.Value = row["TANKA_KBN"].ToString();

			// 人数
			int ninzu = 0;
			if (int.TryParse(System.Convert.ToString(row["ADD_CHARGE_APPLICATION_NINZU"].ToString()), out ninzu) == true)
			{

				entity.addChargeApplicationNinzu.Value = ninzu;
			}

			// 販売単価
			entity.addChargeTanka.Value = CommonRegistYoyaku.convertMoneyToInteger(row["HANBAI_TANKA"].ToString());
			// 支払い方法
			entity.paymentHoho.Value = row["PAYMENT_HOHO"].ToString();
			// 消費税区分
			entity.taxKbn.Value = row["TAX_KBN"].ToString();

			//共通カラム
			entity.deleteDay.Value = CommonRegistYoyaku.ZERO;
			entity.entryDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
			entity.entryTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
			entity.entryPersonCd.Value = UserInfoManagement.userId;
			entity.entryPgmid.Value = PgmId;
			entity.updateDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
			entity.updateTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
			entity.updatePersonCd.Value = UserInfoManagement.userId;
			entity.updatePgmid.Value = PgmId;
			entity.systemUpdateDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
			entity.systemUpdatePersonCd.Value = UserInfoManagement.userId;
			entity.systemUpdatePgmid.Value = PgmId;
			entity.systemEntryDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
			entity.systemEntryPersonCd.Value = UserInfoManagement.userId;
			entity.systemEntryPgmid.Value = PgmId;

			list.Add(entity);
		}

		// 任意選択
		foreach (Row row in this.grdAnySelectionList.Rows)
		{

			if (row.Index == CommonRegistYoyaku.ZERO)
			{
				// ヘッダー行の場合、次レコードへ
				continue;
			}

			entity = new YoyakuInfoOptionEntity();

			// 予約区分
			entity.yoyakuKbn.Value = this._yoyakuKbn;
			// 予約NO
			entity.yoyakuNo.Value = this._yoyakuNo;
			// 年
			string strYear = System.Convert.ToString(this.ParamData.SyuptDay.ToString().Substring(0, 4));
			entity.year.Value = int.Parse(strYear);
			// グループNO
			entity.groupNo.Value = int.Parse(System.Convert.ToString(row["GROUP_NO"].ToString()));
			// 行番号
			entity.lineNo.Value = int.Parse(System.Convert.ToString(row["LINE_NO"].ToString()));
			// 必須区分
			entity.hisuKbn.Value = int.Parse(System.Convert.ToString(row["REQUIRED_KBN"].ToString()));
			// 単価区分
			entity.tankaKbn.Value = row["TANKA_KBN"].ToString();
			// 人数
			int ninzu = System.Convert.ToInt32(CommonRegistYoyaku.ZERO);
			if (int.TryParse(System.Convert.ToString(row["ADD_CHARGE_APPLICATION_NINZU"].ToString()), out ninzu) == true)
			{

				entity.addChargeApplicationNinzu.Value = ninzu;
			}
			// 販売単価
			entity.addChargeTanka.Value = CommonRegistYoyaku.convertMoneyToInteger(row["HANBAI_TANKA"].ToString());
			// 支払い方法
			entity.paymentHoho.Value = row["PAYMENT_HOHO"].ToString();
			// 消費税区分
			entity.taxKbn.Value = row["TAX_KBN"].ToString();

			//共通カラム
			entity.deleteDay.Value = CommonRegistYoyaku.ZERO;
			entity.entryDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
			entity.entryTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
			entity.entryPersonCd.Value = UserInfoManagement.userId;
			entity.entryPgmid.Value = PgmId;
			entity.updateDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
			entity.updateTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
			entity.updatePersonCd.Value = UserInfoManagement.userId;
			entity.updatePgmid.Value = PgmId;
			entity.systemUpdateDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
			entity.systemUpdatePersonCd.Value = UserInfoManagement.userId;
			entity.systemUpdatePgmid.Value = PgmId;
			entity.systemEntryDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
			entity.systemEntryPersonCd.Value = UserInfoManagement.userId;
			entity.systemEntryPgmid.Value = PgmId;

			list.Add(entity);
		}

		return list;
	}

	#endregion

	#region チェック処理
	/// <summary>
	/// 表示チェック
	/// </summary>
	/// <returns></returns>
	private bool isValidDisp()
	{
		//乗車地存在有無チェック
		if (!string.IsNullOrWhiteSpace(System.Convert.ToString(this.ParamData.JyosyatiCd)))
		{
			if (ReferenceEquals(this._placeMaster, null))
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_014", "乗車地コード");
				return false;
			}
		}

		//出発時間チェック
		if (this.isValidSyuptTime() == false)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 出発時間チェック
	/// </summary>
	/// <returns></returns>
	private bool isValidSyuptTime()
	{
		//画面時間
		int formSyuptTime = System.Convert.ToInt32(this.ParamData.SyuptTime);
		//システム日付
		DateTime sysTime = System.Convert.ToDateTime(createFactoryDA.getServerSysDate());

		//警告時間
		DateTime dtAlertTime = sysTime.AddMinutes(MinutesAlert);
		string strAlertTime = dtAlertTime.ToString("HHmm");
		int alertTime = 0;
		int.TryParse(strAlertTime, out alertTime);
		//警告時間（他営業所
		DateTime dtAlertTimeOtherEigyosyo = sysTime.AddHours(HourAlertOtherEigyosyo);
		string strAlertTimeOtherEigyosyo = dtAlertTimeOtherEigyosyo.ToString("HHmm");
		int alertTimeOtherEigyosyo = 0;
		int.TryParse(strAlertTimeOtherEigyosyo, out alertTimeOtherEigyosyo);

		//コース台帳の出発時間
		int crsSyuptTime = System.Convert.ToInt32(if (this._crsLedgerBasic.Rows(0).Field(Of Short ?)["NUM_SYUPT_TIME"], 0));


		//画面上の出発時間より過去の場合、エラーとする
		if (crsSyuptTime < formSyuptTime)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_053");
			return false;
		}

		//警告時間より、過去の場合、エラーとする
		if (crsSyuptTime < alertTime)
		{
			CommonProcess.createFactoryMsg().messageDisp("E02_043", "発券可能");
			return false;
		}

		//警告時間（他営業所）より過去
		//かつ、他営業所（はとバス）から出発のコースの場合、エラーとする
		if (crsSyuptTime < alertTimeOtherEigyosyo && UserInfoManagement.companyCd.Equals(FixedCd.CompanyCdType.hatoBus) && this.isOtherEigyosyo())
		{
			CommonProcess.createFactoryMsg().messageDisp("E02_043", "他営業所の発券可能");
			return false;
		}

		return true;
	}

	/// <summary>
	/// 座席確保、予約登録
	/// </summary>
	/// <returns>処理結果</returns>
	/// <remarks>
	/// S02_0103（予約登録）のcanZasekiKakuho()を参考にしているため、
	/// 該当メソッドの改修時には当メソッドへも反映
	/// </remarks>
	private bool canZasekiKakuhoAndRegistYoyaku(Hashtable sysDates)
	{
		//座席取得
		this._z0001Result = this.getCommonZasekiJidoData();

		if ((_z0001Result.Status == Z0001_Result.Z0001_Result_Status.OK) || (_z0001Result.Status == ))
		{
			Z0001_Result.Z0001_Result_Status.Kaku;
			// "00"、"10"は正常、処理なし
		}
		else if (_z0001Result.Status == Z0001_Result.Z0001_Result_Status.ZasekiShiyouchu)
		{

			CommonProcess.createFactoryMsg().messageDisp("E02_030");
			return false;
		}
		else if (_z0001Result.Status == Z0001_Result.Z0001_Result_Status.ZansekiNashi)
		{

			CommonProcess.createFactoryMsg().messageDisp("E03_004");
			return false;
		}
		else if (_z0001Result.Status == Z0001_Result.Z0001_Result_Status.LadiesSeatManseki)
		{

			CommonProcess.createFactoryMsg().messageDisp("E02_031");
			return false;
		}
		else if (_z0001Result.Status == Z0001_Result.Z0001_Result_Status.OtherReason)
		{

			CommonProcess.createFactoryMsg().messageDisp("E02_030");
			return false;
		}
		else if (_z0001Result.Status == Z0001_Result.Z0001_Result_Status.ParameterError)
		{

			CommonProcess.createFactoryMsg().messageDisp("E02_032");
			return false;
		}
		else
		{

			CommonProcess.createFactoryMsg().messageDisp("E02_032");
			return false;
		}

		// 座席確保更新ステータス
		string zasekiKakuhoStatus = "";

		// 座席更新の検索条件作成
		Hashtable crsZasekiData = (Hashtable)(CommonRegistYoyaku.createCrsZasekiData(this._yoyakuNinzu,);
		DirectCast(sysDates[KeyDtSysDate], DateTime), ;
		Me.ParamData.CrsCd(,);
		Me.ParamData.SyuptDay(,);
		Me.ParamData.Gousya(,);
		PgmId());

		// 当日発券エンティティグループの作成
		HakkenTojituGroupEntity hakkenTojituGroupEntity = this.createHakkenTojituGroup(sysDates);

		S02_0105Da s02_0105Da = new S02_0105Da();
		// 座席数更新、予約登録
		zasekiKakuhoStatus = System.Convert.ToString(s02_0105Da.updateZasekiInfoAndRegistYoyakuInfo(this._z0001Result, crsZasekiData, hakkenTojituGroupEntity));

		// 座席数更新、予約登録　成功確認
		if (zasekiKakuhoStatus != CommonRegistYoyaku.UpdateStatusSucess)
		{

			//座席取消処理
			this.canTorikeshiZaseki(sysDates);

			if (zasekiKakuhoStatus == CommonRegistYoyaku.UpdateStatusUsing)
			{

				CommonProcess.createFactoryMsg().messageDisp("E04_004");
			}
			else if (zasekiKakuhoStatus == CommonRegistYoyaku.UpdateStatusKusekiNothing)
			{

				CommonProcess.createFactoryMsg().messageDisp("E03_004");
			}
			else if (zasekiKakuhoStatus == CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure)
			{

				CommonProcess.createFactoryMsg().messageDisp("E90_025", "座席確保");
			}
			else if (zasekiKakuhoStatus == CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure)
			{

				CommonProcess.createFactoryMsg().messageDisp("E90_025", "座席確保");
			}

			return false;
		}

		return true;
	}

	/// <summary>
	/// 座席取消処理
	/// </summary>
	/// <returns>処理結果</returns>
	private bool canTorikeshiZaseki(Hashtable sysDates)
	{

		Z0001_Result z0001Result = this.getTorikeshiCommonZasekiJidoData();

		// 座席確保更新ステータス
		string zasekiKakuhoStatus = System.Convert.ToString(CommonRegistYoyaku.ValueEmpty);

		// 座席更新の検索条件作成
		Hashtable crsZasekiData = (Hashtable)(CommonRegistYoyaku.createCrsZasekiData(this._yoyakuNinzu * -1, (DateTime)(sysDates[KeyDtSysDate]), this.ParamData.CrsCd, this.ParamData.SyuptDay, this.ParamData.Gousya, this.Name));
		// 当日発券エンティティグループの作成
		HakkenTojituGroupEntity hakkenTojituGroupEntity = this.createHakkenTojituGroup(sysDates);

		S02_0105Da s02_0105Da = new S02_0105Da();

		// コース台帳取消座席更新処理
		zasekiKakuhoStatus = System.Convert.ToString(s02_0105Da.updateTorikeshiZasekiInfoForTeiki(z0001Result, crsZasekiData, hakkenTojituGroupEntity));

		// 座席数、予約情報仮登録成功確認
		if (zasekiKakuhoStatus != CommonRegistYoyaku.UpdateStatusSucess)
		{

			if (zasekiKakuhoStatus == CommonRegistYoyaku.UpdateStatusUsing)
			{

				CommonProcess.createFactoryMsg().messageDisp("E04_004", " ");
			}
			else if (zasekiKakuhoStatus == CommonRegistYoyaku.UpdateStatusKusekiNothing)
			{

				CommonProcess.createFactoryMsg().messageDisp("E03_004");
			}
			else if (zasekiKakuhoStatus == CommonRegistYoyaku.UpdateStatusZasekiUpdateFailure)
			{

				string message = System.Convert.ToString(CommonRegistYoyaku.getUpdateStatusMessage(zasekiKakuhoStatus));
				CommonProcess.createFactoryMsg().messageDisp("E90_025", "座席の取消", message);
			}
			else if (zasekiKakuhoStatus == CommonRegistYoyaku.UpdateStatusYoyakuUpdateFailure)
			{

				string message = System.Convert.ToString(CommonRegistYoyaku.getUpdateStatusMessage(zasekiKakuhoStatus));
				CommonProcess.createFactoryMsg().messageDisp("E90_025", "座席の取消", message);
			}

			return false;
		}

		return true;
	}


	/// <summary>
	/// バス座席自動設定処理
	/// </summary>
	/// <returns>座席自動配置（新規予約）結果</returns>
	private Z0001_Result getCommonZasekiJidoData()
	{

		Z0001_Param param = new Z0001_Param();
		param.ProcessKbn = Z0001_Param.Z0001_Param_ProcessKbn.ProcessKbn_10;
		param.CrsCd = this.ParamData.CrsCd;
		param.SyuptDay = this.ParamData.SyuptDay;
		param.Gousya = this.ParamData.Gousya;
		param.BusReserveCd = CommonRegistYoyaku.convertObjectToString(this._crsLedgerBasic.Rows(0)["BUS_RESERVE_CD"]);
		param.Ninzu = this._yoyakuNinzu;
		param.JyoseiSenyoSeatFlg = ""; // 当日予約は女性専用席無し
		param.ZasekiReserveKbn = CommonRegistYoyaku.convertObjectToString(this._crsLedgerBasic.Rows(0)["ZASEKI_RESERVE_KBN"]);
		param.YoyakuKbn = this._yoyakuKbn;
		param.YoyakuNo = this._yoyakuNo;

		Z0001 z0001 = new Z0001();
		Z0001_Result result = z0001.Execute(param);

		return result;
	}

	/// <summary>
	/// バス座席自動設定処理
	/// (取消)
	/// </summary>
	/// <returns>座席自動配置（新規予約）結果</returns>
	private Z0001_Result getTorikeshiCommonZasekiJidoData()
	{

		Z0001_Param param = new Z0001_Param();
		param.ProcessKbn = Z0001_Param.Z0001_Param_ProcessKbn.ProcessKbn_99;
		param.CrsCd = this.ParamData.CrsCd;
		param.SyuptDay = this.ParamData.SyuptDay;
		param.Gousya = this.ParamData.Gousya;
		param.BusReserveCd = CommonRegistYoyaku.convertObjectToString(this._crsLedgerBasic.Rows(0)["BUS_RESERVE_CD"]);
		param.Ninzu = this._yoyakuNinzu;
		param.JyoseiSenyoSeatFlg = ""; // 当日予約は女性専用フラグ設定無し
		param.ZasekiReserveKbn = CommonRegistYoyaku.convertObjectToString(this._crsLedgerBasic.Rows(0)["ZASEKI_RESERVE_KBN"]);
		param.YoyakuKbn = this._yoyakuKbn;
		param.YoyakuNo = this._yoyakuNo;

		// 座席取得実施
		Z0001 z0001 = new Z0001();
		Z0001_Result result = z0001.Execute(param);

		return result;
	}

	/// <summary>
	/// オプションの各合計設定
	/// </summary>
	private void setOptionTotalMoney()
	{

		int tojituTotal = 0;
		int jizenTotal = 0;

		// 必須選択一覧
		int reqApplicationNum = 0;
		int reqTanka = 0;
		string reqPayMentHoho = "";
		string requiredKbn = "";
		string reqTaxKbn = "";

		int money = 0;

		foreach (Row row in this.grdRequiredSelectionList.Rows)
		{

			if (row.Index == CommonRegistYoyaku.ZERO)
			{
				// ヘッダー行の場合、次レコードへ
				continue;
			}

			reqApplicationNum = 0;
			reqTanka = 0;
			reqPayMentHoho = "";
			reqTaxKbn = "";

			requiredKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["REQUIRED_KBN"]));
			if (requiredKbn == CommonRegistYoyaku.convertEnumToString(FixedCd.HissuKbn.hitogoto))
			{

				if (ReferenceEquals(row["ADD_CHARGE_APPLICATION_NINZU"], null) || string.IsNullOrEmpty(System.Convert.ToString(row["ADD_CHARGE_APPLICATION_NINZU"].ToString())) == true)
				{
					// 適用数が空の場合、次レコードへ
					continue;
				}

				if (row["ADD_CHARGE_APPLICATION_NINZU"].ToString() == CommonRegistYoyaku.ZERO.ToString())
				{
					// 適用数が0の場合、次レコードへ
					continue;
				}

				int.TryParse(System.Convert.ToString(row["ADD_CHARGE_APPLICATION_NINZU"].ToString()), out reqApplicationNum);
				int.TryParse(System.Convert.ToString(row["HANBAI_TANKA"].ToString().Replace(",", "")), out reqTanka);
				// 適用数 * 販売単価
				money = reqApplicationNum * reqTanka;
			}
			else
			{

				int.TryParse(System.Convert.ToString(row["HANBAI_TANKA"].ToString().Replace(",", "")), out reqTanka);
				money = reqTanka;
			}

			reqTaxKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["TAX_KBN"]));

			SyohiTaxCalcParam taxParam = new SyohiTaxCalcParam();
			taxParam.TAXKbn = CommonRegistYoyaku.convertObjectToInteger(reqTaxKbn);
			taxParam.standardDay = this.ParamData.SyuptDay;
			taxParam.kingaku = money;

			if (reqTaxKbn == CommonRegistYoyaku.convertEnumToString(FixedCd.TaxKbnType.zeinuki))
			{
				// 税抜きの場合、オプション金額に消費税をプラスする
				money = money + taxParam.syohiTaxGaku;
			}

			reqPayMentHoho = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["PAYMENT_HOHO"]));
			if (reqPayMentHoho == CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.OptionPaymentHoho.maebarai))
			{

				jizenTotal = jizenTotal + money;
			}
			else
			{

				tojituTotal = tojituTotal + money;
			}
		}

		// 任意選択一覧
		int anyApplicationNum = 0;
		int anyTanka = 0;
		string anyPayMentHoho = "";
		string anyTaxKbn = "";

		foreach (Row row in this.grdAnySelectionList.Rows)
		{

			if (row.Index == CommonRegistYoyaku.ZERO)
			{
				// ヘッダー行の場合、次レコードへ
				continue;
			}

			// 初期化
			anyTanka = 0;
			anyPayMentHoho = "";
			money = 0;
			anyTaxKbn = "";

			anyApplicationNum = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(row["ADD_CHARGE_APPLICATION_NINZU"]));
			if (anyApplicationNum == CommonRegistYoyaku.ZERO)
			{
				// 適用数が0の場合、次レコードへ
				continue;
			}

			string tanka = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["HANBAI_TANKA"]).Replace(",", ""));
			anyTanka = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(tanka));
			anyPayMentHoho = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["PAYMENT_HOHO"]));
			anyTaxKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["TAX_KBN"]));

			// 適用数 * 販売単価
			money = anyApplicationNum * anyTanka;

			SyohiTaxCalcParam taxParam = new SyohiTaxCalcParam();
			taxParam.TAXKbn = CommonRegistYoyaku.convertObjectToInteger(anyTaxKbn);
			taxParam.standardDay = this.ParamData.SyuptDay;
			taxParam.kingaku = money;

			if (anyTaxKbn == CommonRegistYoyaku.convertEnumToString(FixedCd.TaxKbnType.zeinuki))
			{
				// 税抜きの場合、オプション金額に消費税をプラスする
				money = money + taxParam.syohiTaxGaku;
			}

			if (anyPayMentHoho == CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.OptionPaymentHoho.maebarai))
			{

				jizenTotal = jizenTotal + money;
			}
			else
			{

				tojituTotal = tojituTotal + money;
			}
		}

		// 当日、事前
		_tojituAmount = tojituTotal;
		_jizenAmount = jizenTotal;
	}

	#endregion

	#region ユーティリティ
	/// <summary>
	/// システム日付（各型）の取得
	/// </summary>
	/// <returns>各型のシステム日付を格納したHashTable</returns>
	private Hashtable getSysDates()
	{

		//サーバー日付を取得
		DateTime dtSysDate = System.Convert.ToDateTime(createFactoryDA.getServerSysDate());

		//各型へフォーマット
		string strSysDate = dtSysDate.ToString("yyyyMMdd");
		int intSysDate = 0;
		int.TryParse(strSysDate, out intSysDate);
		string strSysTimeHhMmSs = dtSysDate.ToString("HHmmss");
		int intSysTimeHhMmSs = 0;
		int.TryParse(strSysTimeHhMmSs, out intSysTimeHhMmSs);
		string strSysTimeHhMm = dtSysDate.ToString("HHmm");
		int intSysTimeHhMm = 0;
		int.TryParse(strSysTimeHhMm, out intSysTimeHhMm);

		//ハッシュテーブルへ各型のサーバー日付を格納
		Hashtable sysDates = new Hashtable();
		sysDates.Add(KeyDtSysDate, dtSysDate);
		sysDates.Add(KeyStrSysDate, strSysDate);
		sysDates.Add(KeyIntSysDate, intSysDate);
		sysDates.Add(KeyStrSysTimeHhMmSs, strSysTimeHhMmSs);
		sysDates.Add(KeyIntSysTimeHhMmSs, intSysTimeHhMmSs);
		sysDates.Add(KeyStrSysTimeHhMm, strSysTimeHhMm);
		sysDates.Add(KeyIntSysTimeHhMm, intSysTimeHhMm);

		return sysDates;
	}

	/// <summary>
	/// Stringの値のEnumへの変換
	/// </summary>
	private T convertStringToEnum(object Of)
	{
		return ((T)(Enum.Parse(typeof(T), strVal)));
	}
	#endregion
	#endregion
}