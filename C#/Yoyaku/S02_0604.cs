using C1.Win.C1FlexGrid;
using GrapeCity.ActiveReports;


/// <summary>
/// S02_0604 領収書発行
/// </summary>
public class S02_0604 : FormBase
{

	#region 定数／変数
	#region 定数
	/// <summary>
	/// 画面ID
	/// </summary>
	private const string PgmId = "S02_0604";
	/// <summary>
	/// 領収書発行
	/// </summary>
	private const string ReceiptIssue = "領収書発行";
	/// <summary>
	/// 印刷区分　印刷
	/// </summary>
	private const string printKbnCrsName = "C";
	/// <summary>
	/// 領収書発行フラグ　全金発券
	/// </summary>
	private const string ReceiptIssueFlgAllkinHakken = "Y";
	/// <summary>
	/// 領収書発行フラグ　全金発券以外
	/// </summary>
	private const string ReceiptIssueFlgNOTAllkinHakken = "U";
	/// <summary>
	/// エラー印紙税額
	/// </summary>
	private const int ErrorInshiTax = 99999;
	/// <summary>
	/// 前
	/// </summary>
	private const string Before = "B";
	/// <summary>
	/// 後
	/// </summary>
	private const string After = "A";

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
	#endregion

	#region 変数
	/// <summary>
	/// 定期
	/// </summary>
	private bool _isTeiki = true;
	/// <summary>
	/// 企画
	/// </summary>
	private bool _isKikaku = false;
	/// <summary>
	/// 券番
	/// </summary>
	private string _kenNo = "";
	/// <summary>
	/// 宿泊あり
	/// </summary>
	private bool _isStay = false;
	/// <summary>
	/// 払戻あり
	/// </summary>
	private bool _existsHaraiModosi = false;
	/// <summary>
	/// 予約番号単位の総予約人数
	/// </summary>
	private int _totalYoyakuNinzu = 0;
	/// <summary>
	/// 売上
	/// </summary>
	private int _uriage = 0;
	/// <summary>
	/// 割引
	/// </summary>
	private int _waribiki = 0;
	/// <summary>
	/// 請求
	/// </summary>
	private int _seikyu = 0;
	/// <summary>
	/// 入金
	/// </summary>
	private int _nyuukin = 0;
	/// <summary>
	/// 収入印紙リスト
	/// </summary>
	private Dictionary[,] _inshiList; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.

	#region テーブル
	/// <summary>
	/// 予約情報（基本）テーブル
	/// </summary>
	private DataTable _yoyakuInfoBasicTable = null;
	/// <summary>
	/// 発券情報テーブル
	/// </summary>
	private DataTable _hakkenInfoTable = null;
	/// <summary>
	/// 予約情報（コース料金_料金区分）テーブル
	/// </summary>
	private DataTable _yoyakuInfoCrsChargeChargeKbn = null;
	#endregion

	#region プロパティ
	/// <summary>
	/// パラメータクラス
	/// </summary>
	public S02_0604ParamData ParamData
	{
		/// <summary>
		/// 定期
		/// </summary>
		/// <returns></returns>
	private bool IsTeiki
	{
		get
		{
			return this._isTeiki;
		}
		set
		{
			this._isTeiki = value;
		}
	}
	/// <summary>
	/// 企画
	/// </summary>
	/// <returns></returns>
	private bool IsKikaku
	{
		get
		{
			return this._isKikaku;
		}
		set
		{
			this._isKikaku = value;
		}
	}
	/// <summary>
	/// 宿泊あり
	/// </summary>
	/// <returns></returns>
	private bool IsStay
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
	/// <summary>
	/// 売上
	/// </summary>
	/// <returns></returns>
	private int Uriage
	{
		get
		{
			return _uriage;
		}
		set
		{
			this._uriage = value;
		}
	}
	/// <summary>
	/// 割引
	/// </summary>
	/// <returns></returns>
	private int Waribiki
	{
		get
		{
			return _waribiki;
		}
		set
		{
			this._waribiki = value;
		}
	}

	/// <summary>
	/// 請求 （クーポン売上- 割引金額）
	/// </summary>
	/// <returns></returns>
	private int Seikyu
	{
		get
		{
			return _seikyu;
		}
		set
		{
			this._seikyu = value;
			this.txtSeikyuGaku.Text = "{value:#,0}";
		}
	}
	/// <summary>
	/// 入金　（精算情報内訳の合算　※割引金額、キャンセル料を除く）
	/// </summary>
	/// <returns></returns>
	private int Nyuukin
	{
		get
		{
			return _nyuukin;
		}
		set
		{
			this._nyuukin = value;
			this.txtNyuukinGaku.Text = "{value:#,0}";
		}
	}
	/// <summary>
	/// クレジット（オンライン決済含む）
	/// </summary>
	/// <returns></returns>
	private int CreditKingaku
	{
			/// <summary>
			/// 領収書金額
			/// </summary>
			/// <returns></returns>
	private int TotalReceiptKingaku
	{
				/// <summary>
				/// 合計枚数
				/// </summary>
				/// <returns></returns>
	private int TotalSheetNum
	{
					//Private
		#endregion
		#endregion
		#endregion

		#region イベント
		/// <summary>
		/// 画面ロード時のイベント
		/// </summary>
		/// <param name="sender">イベント送信元</param>
		/// <param name="e">イベントデータ</param>
	private void S02_0604_Load(object sender, System.EventArgs e)
	{
		try
		{
			//画面表示時の初期設定
			this.setControlInitiarize();

			//画面の初期表示
			this.setScreen();

			//表示チェック
			if (!this.isValidDisp())
			{
				this.Close();
			}

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
	/// 領収書グリッド編集後イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdReceipt_AfterEdit(object sender, RowColEventArgs e)
	{
		//領収書合計金額の計算
		this.calcurateReceiptKingaku();
	}



	#region 領収書情報
	/// <summary>
	/// 追加ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnAdd_Click(object sender, EventArgs e)
	{
		FlexGridEx grd = TryCast(this.grdReceipt, FlexGridEx);
		grd.Rows.Add();
	}

	/// <summary>
	/// 削除ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnDelete_Click(object sender, EventArgs e)
	{
		FlexGridEx grd = TryCast(this.grdReceipt, FlexGridEx);

		if (grd.Row <= 0)
		{
			return;
		}

		//削除
		grd.RemoveItem(grd.Row);

		//再計算
		this.calcurateReceiptKingaku();
	}
	#endregion

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
	/// F7：印刷ボタン押下イベント
	/// </summary>
	protected override void btnF7_ClickOrgProc()
	{
		base.btnF7_ClickOrgProc();

		//入力チェック
		if (!isValidInput())
		{
			return;
		}

		//領収書発行
		this.executeReceiptIssue();
	}
	#endregion

	#endregion

	#region メソッド
	#region 画面の設定
	/// <summary>
	/// 画面表示時の初期設定
	/// </summary>
	private void setControlInitiarize()
	{

		//ベースフォームの設定
		this.setFormId = PgmId;
		this.setTitle = "領収書発行";

		// フッタボタンの設定
		this.setButtonInitiarize();

		//パラメータ確認
		if (string.IsNullOrWhiteSpace(System.Convert.ToString(this.ParamData.YoyakuKbn)))
		{
			return;
		}
		if (this.ParamData.YoyakuNo == 0)
		{
			return;
		}

		//予約情報（基本）の取得、存在チェック
		this._yoyakuInfoBasicTable = CommonHakken.getYoyakuInfoBasic(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
		if (ReferenceEquals(_yoyakuInfoBasicTable, null))
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_014", "予約情報");
			return;
		}

		//発券情報の取得
		this._hakkenInfoTable = CommonHakken.getHakkenInfo(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);

		//各区分を設定
		string teikiKikakuKbn = System.Convert.ToString(_yoyakuInfoBasicTable.Rows(0).Field(Of string)["TEIKI_KIKAKU_KBN"]);
		string crsKind = System.Convert.ToString(_yoyakuInfoBasicTable.Rows(0).Field(Of string)["CRS_KIND"]);
		this.setEachKbn(teikiKikakuKbn, crsKind);


	}

	/// <summary>
	/// 各区分を設定
	/// </summary>
	private void setEachKbn(string teikiKikakuKbn, string crsKind)
	{
		//定期/企画区分の設定
		int intTeikiKikakuKbn = 0;
		int.TryParse(teikiKikakuKbn, out intTeikiKikakuKbn);
		if (intTeikiKikakuKbn == FixedCd.TeikiKikakuKbn.Teiki)
		{
			this.IsTeiki = true;
			this.IsKikaku = false;
		}
		if (intTeikiKikakuKbn == FixedCd.TeikiKikakuKbn.Kikaku)
		{
			this.IsTeiki = false;
			this.IsKikaku = true;
		}

		//宿泊有無の設定
		this.IsStay = CommonCheckUtil.isStay(crsKind);
	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{

		//Visible
		this.F2Key_Visible = true;
		this.F7Key_Visible = true;

		this.F1Key_Visible = false;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = false;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;

		this.F2Key_Text = "F2:戻る";
		this.F7Key_Text = "F7:印刷";
	}

	/// <summary>
	/// 画面の値設定
	/// </summary>
	private void setScreen()
	{
		//予約情報（基本）の設定
		this.setYoyakuInfoBasic();

		//予約情報（コース料金_料金区分）の設定
		this.setYoyakuInfoCrsChargeChargeKbn();

		//金額の設定
		this.setSeisanInfo();

		//領収書グリッドへの初期値設定
		this.setGrdReceipt();

		//領収書合計金額の計算
		this.calcurateReceiptKingaku();
	}

	#region 予約情報（基本）の設定
	/// <summary>
	/// 予約情報（基本）の設定
	/// </summary>
	private void setYoyakuInfoBasic()
	{
		//値の取り出し
		DataTable dt = this._yoyakuInfoBasicTable;
		string yoyakuKbn = System.Convert.ToString(if (dt.Rows(0).Field(Of string)["YOYAKU_KBN"], ""));
		string yoyakuNo = System.Convert.ToString(if (dt.Rows(0).Field(Of Integer ?)["YOYAKU_NO"], 0).ToString());
		string yoyakuKbnNo = yoyakuKbn + yoyakuNo;
		int intSyuptDay = System.Convert.ToInt32(if (dt.Rows(0).Field(Of Integer ?)["SYUPT_DAY"], 0));
		int intSyuptTime = System.Convert.ToInt32(if (dt.Rows(0).Field(Of Short ?)["SYUPT_TIME"], 0));
		string crsCd = System.Convert.ToString(if (dt.Rows(0).Field(Of string)["CRS_CD"], ""));
		string crsNm = System.Convert.ToString(if (dt.Rows(0).Field(Of string)["CRS_NAME"], ""));
		string jyoshaTi = System.Convert.ToString(if (dt.Rows(0).Field(Of string)["JYOSHATI"], ""));
		string gousya = System.Convert.ToString(if (dt.Rows(0).Field(Of Short ?)["GOUSYA"], 0).ToString());
		string yoyakuName = System.Convert.ToString(if (dt.Rows(0).Field(Of string)["NAME"], ""));
		string telNo = System.Convert.ToString(if (dt.Rows(0).Field(Of string)["TEL_NO_1"], ""));
		string agentName = System.Convert.ToString(if (dt.Rows(0).Field(Of string)["AGENT_NM"], ""));

		//値の設定
		this.ucoYoyakuNo.YoyakuText = yoyakuKbnNo;
		this.ucoSyuptDay.Value = CommonHakken.convertIntToDate(intSyuptDay);
		this.txtCourseCd.Text = crsCd;
		this.txtCourseNm.Text = crsNm;
		this.txtJyosyaTi.Text = jyoshaTi;
		this.ucoTime.Value = CommonHakken.convertIntToTime(intSyuptTime);
		this.txtGousya.Text = gousya;
		this.txtYoyakuPersonName.Text = yoyakuName;
		this.txtTelNo.Text = telNo;
		this.txtDairitenName.Text = agentName;

		//券番を設定
		if (this._hakkenInfoTable IsNot null)
						{
			DataTable hakkenInfo = this._hakkenInfoTable;

			this._kenNo = System.Convert.ToString(CommonHakken.createKenNoFromHakkenInfoRow(hakkenInfo.Rows(0)));
		}
	}
	#endregion

	#region 予約情報（コース料金_料金区分）の設定
	/// <summary>
	/// 予約情報（コース料金_料金区分）の設定
	/// </summary>
	private void setYoyakuInfoCrsChargeChargeKbn()
	{
		//グリッドの初期化
		CommonHakken.setInitiarizeGrid(this.grdChargeKbnStay);
		CommonHakken.setInitiarizeGrid(this.grdChargeKbn);

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

	#region 料金区分(宿泊なし)グリッドの設定
	/// <summary>
	/// 料金区分(宿泊なし)グリッドの設定
	/// </summary>
	private void setGrdChargeKbn()
	{
		//グリッドの設定
		this.grdChargeKbn.AllowDragging = AllowDraggingEnum.None;
		this.grdChargeKbn.AllowAddNew = false;
		this.grdChargeKbn.AutoGenerateColumns = false;
		this.grdChargeKbn.ShowButtons = ShowButtonsEnum.Always;

		//予約情報（コース料金_料金区分）の取得
		DataTable yoyakuInfoCrsChargeChargeKbn = CommonHakken.getYoyakuInfoCrsChargeChargeKbn(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
		if (ReferenceEquals(yoyakuInfoCrsChargeChargeKbn, null))
		{
			return;
		}

		//フィールドへ格納
		this._yoyakuInfoCrsChargeChargeKbn = yoyakuInfoCrsChargeChargeKbn;

		//予約総人数を集計
		this.calculateCharge();

		//グリッドへ設定
		this.grdChargeKbn.DataSource = yoyakuInfoCrsChargeChargeKbn;
	}

	/// <summary>
	/// コース料金（宿泊なし）の集計（非活性なので初期表示時のみ）
	/// </summary>
	private void calculateCharge()
	{
		//宿泊なし
		foreach (DataRow row in this._yoyakuInfoCrsChargeChargeKbn.AsEnumerable())
		{
			decimal dNinzu = System.Convert.ToDecimal(if (row.Field(Of Decimal ?)["CHARGE_APPLICATION_NINZU_1"], 0));
			int ninzu = System.Convert.ToInt32(Convert.ToInt32(dNinzu));

			//null,0 なら次の行へ
			if (ninzu == 0)
			{
				continue;
			}

			decimal dTanka = System.Convert.ToDecimal(if (row.Field(Of Decimal ?)["TANKA_1"], 0));
			int tanka = System.Convert.ToInt32(Convert.ToInt32(dTanka));

			//予約総人数へ加算
			this._totalYoyakuNinzu += ninzu;
		}
	}
	#endregion

	#region 料金区分(宿泊あり)グリッドの設定
	/// <summary>
	/// 料金区分(宿泊あり)グリッドの設定
	/// </summary>
	private void setGrdChargeKbnStayAri()
	{
		//グリッドの設定
		this.grdChargeKbnStay.AllowDragging = AllowDraggingEnum.None;
		this.grdChargeKbnStay.AllowAddNew = false;
		this.grdChargeKbnStay.AllowMerging = AllowMergingEnum.Custom;
		this.grdChargeKbnStay.AutoGenerateColumns = false;
		this.grdChargeKbnStay.ShowButtons = ShowButtonsEnum.Always;

		//予約情報（コース料金_料金区分）の取得
		DataTable yoyakuInfoCrsChargeChargeKbn = CommonHakken.getYoyakuInfoCrsChargeChargeKbn(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
		if (ReferenceEquals(yoyakuInfoCrsChargeChargeKbn, null))
		{
			return;
		}

		//フィールドへ格納（※割引時に正規料金を確認するため。
		this._yoyakuInfoCrsChargeChargeKbn = yoyakuInfoCrsChargeChargeKbn;

		//部屋タイプごとのデータを生成
		DataTable dt = CommonHakken.formatGrdChargeKbnStayAri(yoyakuInfoCrsChargeChargeKbn);

		//予約総人数を集計
		this._totalYoyakuNinzu = System.Convert.ToInt32(dt.AsEnumerable().Sum(Function(dr) dr.Field(Of int)["CHARGE_APPLICATION_NINZU"]));

		//料金区分(宿泊あり)テーブルをグリッドへ設定
		this.grdChargeKbnStay.DataSource = dt.AsDataView();
	}
	#endregion
	#endregion

	/// <summary>
	/// 精算情報の設定
	/// </summary>
	private void setSeisanInfo()
	{
		SeisanInfoEntity ent = new SeisanInfoEntity();
		ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
		ent.yoyakuNo.Value = this.ParamData.YoyakuNo;
		ent.kenno.Value = this._kenNo;

		//払戻の存在確認
		this._existsHaraiModosi = this.existsHaraiModosi(ent);

		//請求 = クーポン売上 - 割引
		decimal couponUriage = this.getCouponUriage(ent);
		decimal waribiki = this.getWaribiki(ent);
		this.Seikyu = Convert.ToInt32(couponUriage - waribiki);

		//入金 = 精算情報内訳の総計
		decimal nyuukin = this.getNyuukin(ent, true);
		decimal henkin = this.getNyuukin(ent, false);
		this.Nyuukin = Convert.ToInt32(nyuukin - henkin);

		//クレジット = クレジット + オンライン決済(収入印紙金額にはクレジット代金を含めない)
		decimal creditNyuukin = this.getCredit(ent, true);
		decimal creditHenkin = this.getCredit(ent, false);
		this.CreditKingaku = Convert.ToInt32(creditNyuukin - creditHenkin);
	}

	/// <summary>
	/// 領収書グリッドの設定
	/// </summary>
	private void setGrdReceipt()
	{
		this.grdReceipt.Rows(1).Item["KINGAKU"] = "{Me.Nyuukin:#,0}";
		this.grdReceipt.Rows(1).Item["SHEET_NUM"] = "1";
		//当日発券は宛名出さない
		string name = System.Convert.ToString(if (_yoyakuInfoBasicTable.Rows(0).Field(Of string)["NAME"], ""));
		if (name.Equals(" " + CommonHakken.NameTojitu) == false)
		{
			this.grdReceipt.Rows(1).Item["ADDRESS"] = name;
		}
	}

	/// <summary>
	/// 領収金額の計算
	/// </summary>
	private void calcurateReceiptKingaku()
	{
		this.TotalReceiptKingaku = 0; //合計領収書金額
		int totalIssueSheetNum = 0; //合計発行枚数

		FlexGridEx grd = this.grdReceipt;

		foreach (Row row in grd.Rows)
		{
			int kingaku = 0;

			//ヘッダーなら次の行へ
			if (row.Index == 0)
			{
				continue;
			}

			//nullなら次の行へ
			if (CommonHakken.isNull(row.Item("KINGAKU")))
			{
				continue;
			}

			//値の取り出し
			int.TryParse(System.Convert.ToString(row.Item("KINGAKU").ToString()), out kingaku);

			//枚数
			string strSheetNum = "";
			if (!CommonHakken.isNull(row.Item("SHEET_NUM")))
			{
				strSheetNum = row.Item("SHEET_NUM").ToString();
			}
			int sheetNUm = 0;
			int.TryParse(strSheetNum, out sheetNUm);
			//人数
			string strNinzu = "";
			if (!CommonHakken.isNull(row.Item("NINZU")))
			{
				strNinzu = row.Item("NINZU").ToString();
			}
			int ninzu = 0;
			int.TryParse(strNinzu, out ninzu);

			//小計
			int receiptSyokei = kingaku * sheetNUm;

			//領収書金額へ追加
			this.TotalReceiptKingaku += receiptSyokei;
			totalIssueSheetNum += sheetNUm;

			//領収金額の小計を表示
			row.Item["RECEIPT_KINGAKU"] = if (receiptSyokei == 0, "", receiptSyokei.ToString());
		}

		//表示
		this.txtTotalReceiptKingaku.Text = "{Me.TotalReceiptKingaku:#,0}";
		this.txtTotalSheetNum.Text = totalIssueSheetNum.ToString();
	}
	#endregion

	#region チェック処理
	/// <summary>
	/// 表示チェック
	/// </summary>
	/// <returns></returns>
	private bool isValidDisp()
	{
		//予約キャンセルチェック
		string cancelFlg = System.Convert.ToString(if (this._yoyakuInfoBasicTable.Rows(0).Field(Of string)["CANCEL_FLG"], ""));
		if (!string.IsNullOrWhiteSpace(cancelFlg))
		{
			//キャンセル済み
			CommonProcess.createFactoryMsg.messageDisp("E90_034", "予約情報", "キャンセル");
			return false;
		}

		//使用中チェック
		string usingFlg = System.Convert.ToString(this._yoyakuInfoBasicTable.Rows(0).Field(Of string)["USING_FLG"]);
		if (!string.IsNullOrWhiteSpace(usingFlg))
		{
			CommonProcess.createFactoryMsg.messageDisp("E90_040");
			return false;
		}

		//発券チェック
		if (this.isValidHakken() == false)
		{
			return false;
		}

		//領収書発行チェック
		string receiptIssueFlg = System.Convert.ToString(if (this._yoyakuInfoBasicTable.Rows(0).Field(Of string)["RECEIPT_ISSUE_FLG"], ""));
		if (receiptIssueFlg.Equals(ReceiptIssueFlgAllkinHakken))
		{
			//全金発券の領収書発行済み
			CommonProcess.createFactoryMsg.messageDisp("E90_069", "領収書", "発行");
			return false;
		}

		//払戻チェック
		if (this._existsHaraiModosi)
		{
			CommonProcess.createFactoryMsg.messageDisp("E90_069", "発券情報", "払戻");
			return false;
		}

		//入金額チェック
		if (this.Nyuukin <= 0)
		{
			//入金なし
			CommonProcess.createFactoryMsg.messageDisp("E90_006", "未入金の発券情報", "領収書発行");
			return false;
		}

		return true;
	}

	/// <summary>
	/// 発券チェック
	/// </summary>
	/// <returns></returns>
	private bool isValidHakken()
	{
		//発券情報取得
		DataTable hakkenInfo = this._hakkenInfoTable;
		if (ReferenceEquals(hakkenInfo, null))
		{
			//発券情報存在チェック
			//未発券の場合、エラー
			CommonProcess.createFactoryMsg().messageDisp("E02_051");
			return false;
		}

		//未VOIDのデータ
		object notVoidDatas = hakkenInfo.AsEnumerable().Where(Function(row) string.IsNullOrWhiteSpace(System.Convert.ToString(row.Field(Of string)["VOID_KBN"])));
		if (notVoidDatas.Count == 0)
		{
			//発券情報存在チェック
			//未VOIDがなければエラー
			CommonProcess.createFactoryMsg.messageDisp("E90_069", "発券情報", "VOID");
			return false;
		}

		//未VOIDのデータは発券者が発券日にのみVOID可能
		foreach (DataRow row in notVoidDatas)
		{
			//発券日チェック
			DateTime hakkenDay = System.Convert.ToDateTime(row.Field(Of DateTime)["SYSTEM_ENTRY_DAY"]);
			bool isHakkenedToday = System.Convert.ToBoolean(CommonConvertUtil.ChkWhenDay(hakkenDay, CommonConvertUtil.WhenTojitu));
			//発券日当日でなければエラー
			if (isHakkenedToday == false)
			{
				CommonProcess.createFactoryMsg.messageDisp("E90_073", ReceiptIssue, "発券当日");
				return false;

			}

			//発券営業所チェック
			string hakkenEigyosyoCd = System.Convert.ToString(row.Field(Of string)["HAKKEN_EIGYOSYO_CD"]);
			//発券営業所でなければエラー
			if (!hakkenEigyosyoCd.Equals(UserInfoManagement.eigyosyoCd))
			{
				CommonProcess.createFactoryMsg.messageDisp("E90_073", ReceiptIssue, "発券営業所");
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <returns></returns>
	private bool isValidInput()
	{
		//領収書グリッドの入力チェック
		if (this.isValidGrdReceipt() == false)
		{
			return false;
		}

		//過剰領収金額チェック
		if (this.Nyuukin < this.TotalReceiptKingaku)
		{
			CommonProcess.createFactoryMsg.messageDisp("E70_003", "領収金額", "入金額未満");
			return false;
		}

		//領収額チェック
		if (this.TotalReceiptKingaku == 0)
		{
			CommonProcess.createFactoryMsg.messageDisp("E70_003", "領収金額", "0以上");
			return false;
		}

		return true;
	}

	/// <summary>
	/// 領収書グリッドチェック
	/// </summary>
	/// <returns></returns>
	private bool isValidGrdReceipt()
	{
		//精算グリッド上のチェック
		foreach (Row row in this.grdReceipt.Rows)
		{
			//ヘッダー行飛ばす
			if (row.Index == 0)
			{
				continue;
			}

			//値の取り出し
			//金額
			string strKingaku = "";
			if (!CommonHakken.isNull(row.Item("KINGAKU")))
			{
				strKingaku = System.Convert.ToString(row.Item("KINGAKU").ToString());
			}
			int kingaku = 0;
			int.TryParse(strKingaku, out kingaku);
			//枚数
			string strSheetNum = "";
			if (!CommonHakken.isNull(row.Item("SHEET_NUM")))
			{
				strSheetNum = System.Convert.ToString(row.Item("SHEET_NUM").ToString());
			}
			int sheetNum = 0;
			int.TryParse(strSheetNum, out sheetNum);
			//人数
			string strNinzu = "";
			if (!CommonHakken.isNull(row.Item("NINZU")))
			{
				strNinzu = System.Convert.ToString(row.Item("NINZU").ToString());
			}
			int ninzu = 0;
			int.TryParse(strNinzu, out ninzu);
			//宛名
			string address = System.Convert.ToString(TryCast(row.Item("ADDRESS"), string));


			if (kingaku == 0)
			{
				//金額が0の場合

				//全部空でなければエラー
				if (!(sheetNum == 0 && ninzu == 0 && string.IsNullOrWhiteSpace(address)))
				{

					//枚数が入力されていたらエラー
					CommonProcess.createFactoryMsg.messageDisp("E70_003", "領収書金額の入力", "金額");
					return false;
				}

			}
			else
			{
				//金額が入力されている場合

				if (sheetNum == 0)
				{
					//枚数が0の場合エラー
					CommonProcess.createFactoryMsg.messageDisp("E70_003", "領収書金額の入力", "枚数");
					return false;
				}
			}
		} //ここまで精算グリッドのチェック

		return true;

	}
	#endregion

	#region 領収書発行
	/// <summary>
	/// 領収書発行
	/// </summary>
	private void executeReceiptIssue()
	{
		//エンティティの設定
		ReceiptGroupEntity receiptGroupEntity = this.setReceiptGroupEntity();

		//登録処理
		bool isSuccess = insertReceiptGroup(receiptGroupEntity);
		if (isSuccess == false)
		{
			//異常終了
			CommonProcess.createFactoryMsg().messageDisp("E90_025", ReceiptIssue);
			// log出力
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, ReceiptIssue, "登録処理");

			return;
		}

		// log出力
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.entry, ReceiptIssue, "登録処理");


		//帳票出力処理
		this.printReceipt(receiptGroupEntity);

		//正常終了
		CommonProcess.createFactoryMsg().messageDisp("I90_002", ReceiptIssue);
		this.Close();
	}

	/// <summary>
	/// 帳票出力処理
	/// </summary>
	private void printReceipt(ReceiptGroupEntity receiptGroupEntity)
	{
		List[] entList = receiptGroupEntity.ReceiptEntityList;
		List prmList = new List(Of P02_0603ParamData);

		//パラメータ用のエンティティリスト作成
		foreach (ReceiptEntity ent in entList)
		{
			P02_0603ParamData prmEnt = new P02_0603ParamData();


			//宛名
			prmEnt.Address = ent.address.Value;
			//領収金額
			prmEnt.ReceiptKingaku = if (ent.receiptKingaku.Value, 0);
			//コース種別
			prmEnt.CrsKind = if (_yoyakuInfoBasicTable.Rows(0).Field(Of string)["CRS_KIND"], "");


			//印紙税額
			prmEnt.InshiTaxGaku = if (ent.inshiTaxGaku.Value, 0);
			//発券日
			prmEnt.HakkenDay = if (_hakkenInfoTable.Rows(0).Field(Of Integer ?)["HAKKEN_DAY"], 0);
			//営業所コード
			prmEnt.EigyosyoCd = UserInfoManagement.eigyosyoCd;
			//会社コード
			prmEnt.CompanyCd = UserInfoManagement.companyCd;
			//予約区分
			prmEnt.YoyakuKbn = this.ParamData.YoyakuKbn;
			//予約NO
			prmEnt.YoyakuNo = this.ParamData.YoyakuNo;
			//連番
			prmEnt.Nlgsqn = if (ent.printNlgsqn.Value, 0) +100;
			//枚数
			//prmEnt.SheetNum =
			//全額クレジットフラグ
			if (this.CreditKingaku != 0 && prmEnt.ReceiptKingaku >= this.CreditKingaku)
			{
				//全額クレジット支払いの場合、True
				prmEnt.CreditFlg = true;
			}
			else
			{
				prmEnt.CreditFlg = false;
			}

			prmList.Add(prmEnt);
		}

		//パラメータのセット
		P02_0603Output p02_0603Output = new P02_0603Output();
		p02_0603Output.ParamList = prmList;

		//Listから帳票作成
		SectionReport report = p02_0603Output.setReportPrinting();

		//帳票出力
		report.Document.Print(true, true, false);
	}
	#endregion

	#region DataAccess
	/// <summary>
	/// 払戻の存在確認
	/// </summary>
	/// <param name="ent"></param>
	/// <returns></returns>
	private bool existsHaraiModosi(SeisanInfoEntity ent)
	{
		S02_0604Da da = new S02_0604Da();
		DataTable dt = da.getCountHaraiModosi(ent);

		bool existsModosi = if (dt.Rows(0).Field(Of Decimal ?)["COUNT"], 0) > 0;
		return existsModosi;
	}

	/// <summary>
	/// クーポン売上の取得
	/// </summary>
	/// <returns></returns>
	private decimal getCouponUriage(SeisanInfoEntity ent)
	{
		S02_0604Da da = new S02_0604Da();
		DataTable dt = da.getCouponUriage(ent);

		return if (dt.Rows(0).Field(Of Decimal ?)["COUPON_URIAGE"], 0);
	}

	/// <summary>
	/// 割引の取得
	/// </summary>
	/// <returns></returns>
	private decimal getWaribiki(SeisanInfoEntity ent)
	{
		S02_0604Da da = new S02_0604Da();
		DataTable dt = da.getWaribiki(ent);

		return if (dt.Rows(0).Field(Of Decimal ?)["WARIBIKI"], 0);
	}

	/// <summary>
	/// 入返金の取得
	/// </summary>
	/// <returns></returns>
	private decimal getNyuukin(SeisanInfoEntity ent, bool isNyuukin)
	{
		S02_0604Da da = new S02_0604Da();
		DataTable dt = da.getNyuukin(ent, isNyuukin);

		return if (dt.Rows(0).Field(Of Decimal ?)["NYUUKIN"], 0);
	}

	/// <summary>
	/// クレジット金額の取得
	/// </summary>
	/// <returns></returns>
	private decimal getCredit(SeisanInfoEntity ent, bool isNyuukin)
	{
		S02_0604Da da = new S02_0604Da();
		DataTable dt = da.getCredit(ent, isNyuukin);

		return if (dt.Rows(0).Field(Of Decimal ?)["NYUUKIN"], 0);
	}

	/// <summary>
	/// 領収書の登録
	/// </summary>
	/// <returns></returns>
	private bool insertReceiptGroup(ReceiptGroupEntity receiptGroupEntitiy)
	{
		S02_0604Da da = new S02_0604Da();
		return da.insertReceipt(receiptGroupEntitiy);
	}

	/// <summary>
	/// 印刷連番の取得
	/// </summary>
	/// <returns></returns>
	private int getPrntNlgSqn()
	{
		S02_0604Da da = new S02_0604Da();
		DataTable dt = da.getPrintNlgsqn(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);

		decimal printNlgSqn = 0;
		if (dt IsNot null)
						{
			printNlgSqn = System.Convert.ToDecimal(if (dt.Rows(0).Field(Of Decimal ?)["PRINT_NLGSQN"], 0));
		}
		printNlgSqn++;

		return Convert.ToInt32(printNlgSqn);
	}

	/// <summary>
	/// 収入印紙金額マスタの取得
	/// </summary>
	/// <returns></returns>
	private DataTable getShunyuInshiKingakuMaster()
	{
		S02_0604Da da = new S02_0604Da();
		DataTable dt = da.getShunyuInshiKingakuMaster();

		return if (CommonHakken.existsDatas(dt), dt, null);
	}
	#endregion

	#region エンティティの設定
	private ReceiptGroupEntity setReceiptGroupEntity()
	{
		//サーバー日付を取得
		Hashtable sysDates = this.getSysDates();

		ReceiptGroupEntity receiptGroupEntity = new ReceiptGroupEntity();
		receiptGroupEntity.YoyakuInfoBasicEntity = this.createYoyakuInfoBasicEntity(sysDates);
		receiptGroupEntity.ReceiptEntityList = this.createReceiptEntityList(sysDates);

		return receiptGroupEntity;
	}

	/// <summary>
	/// 予約情報（基本）エンティティの設定
	/// </summary>
	/// <param name="sysdates"></param>
	/// <returns></returns>
	private YoyakuInfoBasicEntity createYoyakuInfoBasicEntity(Hashtable sysdates)
	{
		YoyakuInfoBasicEntity ent = new YoyakuInfoBasicEntity();

		ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
		ent.yoyakuNo.Value = this.ParamData.YoyakuNo;

		//全金発券の確認
		bool isAllkinHakken = false;
		string hakkenNaiyo = System.Convert.ToString(if (this._yoyakuInfoBasicTable.Rows(0).Field(Of string)["HAKKEN_NAIYO"], ""));
		if (hakkenNaiyo.Equals(CommonHakken.convertEnumToString(FixedCdYoyaku.HakkenNaiyo.allkinHakken)))
		{
			isAllkinHakken = true;
		}

		//領収書発行フラグ
		ent.receiptIssueFlg.Value = if (isAllkinHakken, ReceiptIssueFlgAllkinHakken, ReceiptIssueFlgNOTAllkinHakken);

		ent.deleteDay.Value = 0;

		ent.updateDay.Value = (Integer?)(sysdates[KeyIntSysDate]);
		ent.updateTime.Value = (Integer?)(sysdates[KeyIntSysTimeHhMmSs]);
		ent.updatePersonCd.Value = UserInfoManagement.userId;
		ent.updatePgmid.Value = PgmId;

		ent.systemUpdateDay.Value = (DateTime)(sysdates[KeyDtSysDate]);
		ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
		ent.systemUpdatePgmid.Value = PgmId;

		return ent;
	}

	/// <summary>
	/// 領収書エンティティの設定
	/// </summary>
	/// <returns></returns>
	private List[] createReceiptEntityList(Hashtable sysDates)
	{
		List list = new List(Of ReceiptEntity);

		int tmpCredit = System.Convert.ToInt32(this.CreditKingaku);

		//印刷連番を取得
		int nlgsqn = this.getPrntNlgSqn();

		foreach (Row row in this.grdReceipt.Rows)
		{
			int kingaku = 0;

			//ヘッダー行なら次の行へ
			if (row.Index == 0)
			{
				continue;
			}

			//nullなら次の行へ
			if (CommonHakken.isNull(row.Item("KINGAKU")))
			{
				continue;
			}

			//値の取り出し
			int.TryParse(System.Convert.ToString(row.Item("KINGAKU").ToString()), out kingaku);

			//0なら次の行へ
			if (kingaku == 0)
			{
				continue;
			}


			//枚数
			string strSheetNum = "";
			if (!CommonHakken.isNull(row.Item("SHEET_NUM")))
			{
				strSheetNum = row.Item("SHEET_NUM").ToString();
			}
			int sheetNum = 0;
			int.TryParse(strSheetNum, out sheetNum);
			//人数
			string strNinzu = "";
			if (!CommonHakken.isNull(row.Item("NINZU")))
			{
				strNinzu = row.Item("NINZU").ToString();
			}
			int ninzu = 0;
			int.TryParse(strNinzu, out ninzu);
			//宛名
			string address = System.Convert.ToString(TryCast(row.Item("ADDRESS"), string));

			//行ごとに枚数分のエンティティを生成
			for (index = 1; index <= sheetNum; index++)
			{
				//エンティティを生成
				ReceiptEntity ent = new ReceiptEntity();

				ent.yoyakuKbn.Value = this.ParamData.YoyakuKbn;
				ent.yoyakuNo.Value = this.ParamData.YoyakuNo;

				ent.companyCd.Value = UserInfoManagement.companyCd;
				ent.eigyosyoCd.Value = UserInfoManagement.eigyosyoCd;
				ent.signonTime.Value = CommonHakken.convertDateToIntTime(UserInfoManagement.signonDate);

				ent.teikiKikakuKbn.Value = if (this._yoyakuInfoBasicTable.Rows(0).Field(Of string)["TEIKI_KIKAKU_KBN"], "");
			ent.crsKind.Value = if (this._yoyakuInfoBasicTable.Rows(0).Field(Of string)["CRS_KIND"], "");
			ent.crsCd.Value = this.txtCourseCd.Text.Trim();

			ent.ninzu.Value = ninzu;
			ent.address.Value = address;
			ent.receiptKingaku.Value = kingaku;

			ent.printCount.Value = 1;
			ent.printNlgsqn.Value = nlgsqn;

			//印紙代の計算
			int forInshi = kingaku - tmpCredit;
			tmpCredit -= kingaku;
			ent.inshiTaxGaku.Value = this.getShunyuInshiKingaku(forInshi);

			ent.deleteDay.Value = 0;

			ent.entryDay.Value = (Integer?)(sysDates[KeyIntSysDate]);
			ent.entryTime.Value = (Integer?)(sysDates[KeyIntSysTimeHhMmSs]);
			ent.entryPersonCd.Value = UserInfoManagement.userId;
			ent.entryPgmid.Value = PgmId;

			ent.updateDay.Value = (Integer?)(sysDates[KeyIntSysDate]);
			ent.updateTime.Value = (Integer?)(sysDates[KeyIntSysTimeHhMmSs]);
			ent.updatePersonCd.Value = UserInfoManagement.userId;
			ent.updatePgmid.Value = PgmId;

			ent.systemEntryDay.Value = (DateTime)(sysDates[KeyDtSysDate]);
			ent.systemEntryPersonCd.Value = UserInfoManagement.userId;
			ent.systemEntryPgmid.Value = PgmId;

			ent.systemUpdateDay.Value = (DateTime)(sysDates[KeyDtSysDate]);
			ent.systemUpdatePersonCd.Value = UserInfoManagement.userId;
			ent.systemUpdatePgmid.Value = PgmId;



			list.Add(ent);

			//連番をインクリメント
			nlgsqn++;
		} //ここまで枚数のループ
	} //ここまでグリッドのループ
						
						return list;
					}

/// <summary>
/// 収入印紙金額の計算
/// </summary>
private int getShunyuInshiKingaku(int input)
{
	DataTable master = this.getShunyuInshiKingakuMaster();

	//改定日の確認
	int intKaiteiDay = System.Convert.ToInt32(if (master.Rows(0).Field(Of Integer ?)["KAITEI_DAY"], 0));
Date? kaiteiDayNUll = CommonHakken.convertIntToDate(intKaiteiDay);
DateTime kaiteiDay = DateTime.Parse(kaiteiDay.ToString());
bool isBeforeKaiteiDay = System.Convert.ToBoolean(CommonConvertUtil.ChkWhenDay(kaiteiDay, CommonConvertUtil.WhenMirai));

//改定前/後
string beforeAfter = System.Convert.ToString(if(isBeforeKaiteiDay, Before, After));

//1~5のカラムの金額と判定を行い、税額を決定する
for (index = 1; index <= 5; index++)
{
	//値の取り出し
	int kingaku = System.Convert.ToInt32(if (master.Rows(0).Field(Of Integer ?)["KINGAKU_{index}_KAITEI_{beforeAfter}F"], 0));
int tax = System.Convert.ToInt32(if(master.Rows(0).Field(Of Integer ?)["TAX_GAKU_{index}_KAITEI_{beforeAfter}F"], 0));

if (input < kingaku)
{
	return tax;
}

if (index == 5 && input > kingaku)
{
	return ErrorInshiTax;
}
						}
						
						//印紙額が設定できなかったら印刷しない
						throw (new Exception());
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
#endregion
#endregion
				}