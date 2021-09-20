using C1.Win.C1FlexGrid;


/// <summary>
/// S02_0107 予約変更履歴照会
/// </summary>
public class S02_0107 : FormBase
{
	public S02_0107()
	{
		_yoyakuCanselFeeEntity = new EntityOperation(Of YoyakuCanselInfoEntity);

	}

	#region 定数/変数

	/// <summary>
	/// 予約変更履歴情報
	/// </summary>
	private DataTable _yoyakuChangeHistry;
	/// <summary>
	/// 予約情報（基本）
	/// </summary>
	private DataTable _yoyakuInfoBasic;
	/// <summary>
	/// コース情報
	/// </summary>
	private DataTable _crsInfo;
	/// <summary>
	/// 料金区分一覧
	/// </summary>
	private DataTable _chargeKbnList;
	/// <summary>
	/// 乗車地情報
	/// </summary>
	private DataTable _joshatiInfo;
	/// <summary>
	/// メッセージ情報
	/// </summary>
	private DataTable _msgInfo;

	/// <summary>
	/// 更新対象エンティティ
	/// </summary>
	private EntityOperation _yoyakuCanselFeeEntity;

	/// <summary>
	/// コース種別
	/// </summary>
	private string _CrsKind;
	/// <summary>
	/// コースコード
	/// </summary>
	private string _CrsCd;

	/// <summary>
	/// 号車
	/// </summary>
	private int _Gousya;

	/// <summary>
	/// 出発日
	/// </summary>
	private int _SyuptDay;

	/// <summary>
	/// 予約人数
	/// </summary>
	private int _yoyakuNinzu = 0;
	/// <summary>
	/// SEQ：選択された予約変更履歴のSEQ
	/// </summary>
	private int _seq = 0;
	/// <summary>
	/// SEQ：選択された予約変更履歴グリッドの行番号
	/// </summary>
	private int _row = 0;

	/// <summary>
	/// 予約区分
	/// </summary>
	private string _YoyakuKbn;
	/// <summary>
	/// 予約NO
	/// </summary>
	private int _YoyakuNo;
	/// <summary>
	/// 使用中フラグ
	/// </summary>
	private string _UsingFlg = string.Empty;

	/// <summary>
	/// パラメータクラス
	/// </summary>
	public S02_0107ParamData ParamData
	{

#endregion

		#region イベント
		/// <summary>
		/// フォーム起動時の独自処理
		/// </summary>
	protected override void StartupOrgProc()
	{
		//受取パラメータによる初期値セット

		//TODO:所属部署が国際事業部の場合、外国語にチェック
		if (ParamData IsNot null)
			{
			try
			{
				//前処理
				base.comPreEvent();

				this.gbxYoyakuHistry.AutoSize = false;
				this.gbxYoyakuInfo.AutoSize = false;
				this.gbxMessage.AutoSize = false;

				//画面の項目初期化
				initItems();

				this._YoyakuKbn = System.Convert.ToString(ParamData.YoyakuKbn);
				this._YoyakuNo = System.Convert.ToInt32(ParamData.YoyakuNo);
				if (this.CheckSearch() == false)
				{
					this.Close();
					return;
				}
				// 予約者情報設定
				this.setYoyakuChangeHistry();

				this.ucoCrsYoyakuNo.YoyakuText = this._YoyakuKbn + this._YoyakuNo.ToString().Trim();

				//予約変更履歴グリッド項目表示制御                         TODO 権限制御決定後修正
				this.pnlIssueEdit.Visible = true;
				this.grdYoyakuHiostry.Cols("INVALID").Visible = true;
				this.grdYoyakuHiostry.Cols("SEIKYU_ALREADY").Visible = true;

				//  ボタン設定
				this.setButtonlInitiarize();
			}
			catch
			{

				throw;
			}
			finally
			{
				// 後処理
				base.comPostEvent();
			}
		}

	}

	/// <summary>
	/// 画面ロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S02_0107_Load(object sender, EventArgs e)
	{
		base.closeFormFlg = false; //クローズ確認フラグ
	}

	/// <summary>
	/// F2：閉じるボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		// 閉じる
		this.Close();
	}

	/// <summary>
	/// 変更履歴一覧グリッド：表示ボタン押下処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdYoyakuHiostry_CellButtonClick(object sender, RowColEventArgs e)
	{

		if (Information.IsNumeric(grdYoyakuHiostry.Rows(e.Row).Item("SEQ")) == true)
		{
			_seq = System.Convert.ToInt32(grdYoyakuHiostry.Rows(e.Row).Item("SEQ"));
			_row = System.Convert.ToInt32(e.Row);
			//画面表示設定
			this.setScreenInfo();
		}
	}

	/// <summary>
	/// ピックアップグリッドボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdPickup_CellButtonClick(object sender, RowColEventArgs e)
	{

		MPickupHotelEntity entity = new MPickupHotelEntity();

		using (S90_0117 form = new S90_0117(entity))
		{

			form.ShowDialog();
		}


		// TODO：ピックアップの子画面を表示する
		//       ピックアップの子画面に表示している内容がちがうような。。
		//       ピックアップ台帳から取得していないので、不要なものまで表示されている。。

		int rowIndex = System.Convert.ToInt32(e.Row);
		int colIndex = System.Convert.ToInt32(e.Col);

	}

	///' <summary>
	///' キャンセル率ロストフォーカスイベント
	///' </summary>
	///' <param name="sender">イベント送信元</param>
	///' <param name="e">イベントデータ</param>
	//Private Sub numCanselPar_LostFocus(sender As Object, e As EventArgs) Handles numCanselPar.LostFocus

	//    'キャンセル料
	//    Call CalcCancel()

	//    'キャンセル料合計
	//    Call CalcCancelAll()

	//End Sub

	///' <summary>
	///' 無効チェックボックスチェックチェンジイベント
	///' </summary>
	///' <param name="sender"></param>
	///' <param name="e"></param>
	//Private Sub chkCanselRyouInvalidFlg_CheckedChanged(sender As Object, e As EventArgs) Handles chkCanselRyouInvalidFlg.CheckedChanged
	//    If Me.chkCanselRyouInvalidFlg.Enabled = True Then
	//        'キャンセル料
	//        Call CalcCancel()

	//        'キャンセル料合計
	//        Call CalcCancelAll()
	//    End If
	//End Sub

	///' <summary>
	///' 部屋(宿泊あり)一覧グリッド内訳ボタン押下イベント
	///' </summary>
	///' <param name="sender">イベント送信元</param>
	///' <param name="e">イベントデータ</param>
	//Private Sub grdRoomList_CellButtonClick(ByVal sender As Object, ByVal e As RowColEventArgs) Handles grdRoomList.CellButtonClick

	//    Dim roomSuList As New List(Of String)()
	//    roomSuList.Add(CommonRegistYoyaku.convertObjectToString(Me.grdRoomList.GetData(CommonRegistYoyaku.GridStartRowIndex, "ROOMING_BETU_NINZU_1")))
	//    roomSuList.Add(CommonRegistYoyaku.convertObjectToString(Me.grdRoomList.GetData(CommonRegistYoyaku.GridStartRowIndex, "ROOMING_BETU_NINZU_2")))
	//    roomSuList.Add(CommonRegistYoyaku.convertObjectToString(Me.grdRoomList.GetData(CommonRegistYoyaku.GridStartRowIndex, "ROOMING_BETU_NINZU_3")))
	//    roomSuList.Add(CommonRegistYoyaku.convertObjectToString(Me.grdRoomList.GetData(CommonRegistYoyaku.GridStartRowIndex, "ROOMING_BETU_NINZU_4")))
	//    roomSuList.Add(CommonRegistYoyaku.convertObjectToString(Me.grdRoomList.GetData(CommonRegistYoyaku.GridStartRowIndex, "ROOMING_BETU_NINZU_5")))

	//    Using form As New S02_0103Sub

	//        form.ChargeKbnList = Me._chargeKbnList
	//        form.RoomSuList = roomSuList
	//        form.ShowDialog()
	//    End Using
	//End Sub

	///' <summary>
	///' ピックアップグリッドボタン押下イベント
	///' </summary>
	///' <param name="sender">イベント送信元</param>
	///' <param name="e">イベントデータ</param>
	//Private Sub grdPickup_CellButtonClick(ByVal sender As Object, ByVal e As RowColEventArgs) Handles grdPickup.CellButtonClick

	//    Dim entity As New MPickupHotelEntity()

	//    Using form As New S90_0117(entity)

	//        form.ShowDialog()
	//    End Using


	//    Dim rowIndex As Integer = e.Row
	//    Dim colIndex As Integer = e.Col



	//End Sub
	#endregion

	#region メソッド
	// 初期化用のメソッド

	/// <summary>
	/// 画面の項目初期化
	/// </summary>
	protected virtual void initItems() // 画面の項目初期化
	{
		// 各コンボボックスのデータを設定
		this.setCombBoxControl();

		//Header情報

		this.lblSyuptDay.Text = string.Empty; // 出発日
		this.lblCrsCd.Text = string.Empty; // コースコード
		this.lblCrsName.Text = string.Empty; // コース名
		this.lblGousya.Text = string.Empty; // 号車

		//テキストボックス初期化
		object textBoxs = this.gbxYoyakuHistry.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
		}
		this.numCanselPar.Text = "";

		textBoxs = this.pnlIssueEdit.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
		}

		textBoxs = this.gbxYoyakuInfo.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
		}
		textBoxs = tabYoyakuPersonInfo.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
		}
		ucoCrsYoyakuNo.ExistError = false;
		this.txtCrsKingaku.Text = string.Empty;
		this.txtCrsYoyakuJiWaribiki.Text = string.Empty;
		this.txtCrsTotalKingaku.Text = string.Empty;
		this.txtCrsCancelRyou.Text = string.Empty;
		this.numCanselPar.Text = string.Empty;
		this.txtDairitenCd.Text = string.Empty;
		this.txtDairitenName.Text = string.Empty;
		this.cmbSeisanDairiten.SelectedIndex = -1;
		this.txtDairitenTel.Text = string.Empty;
		this.txtDairitenTanto.Text = string.Empty;
		this.txtDairitenToursNo.Text = string.Empty;

		//チェックボックス初期化
		object CheckBoxs = this.gbxYoyakuHistry.Controls.OfType(Of CheckBoxEx);
		foreach ( in CheckBoxs)
		{
			CheckBox.Checked = false;
			CheckBox.ExistError = false;
		}
		this.chkWomen.Checked = false;
		this.chkWomen.ExistError = false;
		CheckBoxs = this.tabYoyakuPersonInfo.Controls.OfType(Of CheckBoxEx);
		foreach ( in CheckBoxs)
		{
			CheckBox.Checked = false;
			CheckBox.ExistError = false;
		}

		CheckBoxs = this.pnlIssueEdit.Controls.OfType(Of CheckBoxEx);
		foreach ( in CheckBoxs)
		{
			CheckBox.Checked = false;
			CheckBox.ExistError = false;
		}

		// グリッドの初期化
		DataTable dt = new DataTable();
		object with_1 = this.grdYoyakuHiostry;
		//グリッド初期化
		with_1.DataSource = dt;
		with_1.DataMember = "";
		with_1.Cols.Count = 19;
		//非表示部分
		for (byte intCnt = 9; intCnt <= 18; intCnt++)
		{
			with_1.Cols(intCnt).Visible = false;
		}
		with_1.Refresh();
		this.lblLengthGrd01.Text = "0件";
		object with_2 = this.grdYoyakuNinzuList;
		//グリッド初期化
		with_2.DataSource = dt;
		with_2.DataMember = "";
		with_2.Refresh();
		//グリッド表示
		with_2.Visible = true;
		object with_3 = this.grdChargeList;
		//グリッド初期化
		with_3.DataSource = dt;
		with_3.DataMember = "";
		with_3.Refresh();
		//グリッド非表示
		with_3.Visible = false;
		object with_4 = this.grdRoomList;
		//グリッド初期化
		with_4.DataSource = dt;
		with_4.DataMember = "";
		with_4.Refresh();
		//グリッド表示
		with_4.Visible = true;
		object with_5 = this.grdYoyakuHiostry;
		//グリッド初期化
		with_5.DataSource = dt;
		with_5.DataMember = "";
		with_5.Refresh();
		object with_6 = this.grdNoribaList;
		//グリッド初期化
		with_6.DataSource = dt;
		with_6.DataMember = "";
		with_6.Refresh();
		object with_7 = this.grdMessageList;
		//グリッド初期化
		with_7.DataSource = dt;
		with_7.DataMember = "";
		with_7.Refresh();
		object with_8 = this.grdPickup;
		//グリッド初期化
		with_8.DataSource = dt;
		with_8.DataMember = "";
		with_8.Refresh();
		object with_9 = this.grdMemoList;
		//グリッド初期化
		with_9.DataSource = dt;
		with_9.DataMember = "";
		with_9.Refresh();

	}

	/// <summary>
	/// ボタン設定
	/// </summary>
	private void setButtonlInitiarize()
	{

		bool flag = true;

		// フッターボタン設定
		this.F4Key_Enabled = flag;
		this.F6Key_Enabled = flag;
		this.F8Key_Enabled = flag;
		this.F9Key_Enabled = flag;
		this.F10Key_Enabled = flag;
		this.F11Key_Enabled = flag;

		// フッターボタン名称設定
		this.F2Key_Text = "F2：閉じる";

		// 不要ボタン設定
		this.F1Key_Visible = false;
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
	}

	/// <summary>
	/// 各コンボボックスのデータを設定
	/// </summary>
	private void setCombBoxControl()
	{

		S02_0103Da s02_0103Da = new S02_0103Da();

		// 国籍_
		CommonRegistYoyaku.setComboBoxData(s02_0103Da, this.cmbKokuseki, FixedCdYoyaku.CodeBunruiTypeKokuseki);
		// 性別
		CommonRegistYoyaku.setComboBoxData(s02_0103Da, this.cmbSex, FixedCdYoyaku.CodeBunruiTypeSexBetu);
		// 支払方法
		CommonRegistYoyaku.setComboBoxData(s02_0103Da, this.cmbSiharaiHoho, FixedCdYoyaku.CodeBunruiTypeShiharaiHoho);
		//  精算代理店
		CommonRegistYoyaku.setComboBoxData(s02_0103Da, this.cmbSeisanDairiten, FixedCdYoyaku.CodeBunruiTypeSeisanDairiten);
		// メモ分類
		CommonRegistYoyaku.setComboBoxData(s02_0103Da, this.cmbBunrui, FixedCdYoyaku.CodeBunruiTypeMemoBunrui);
		// 広告媒体
		CommonRegistYoyaku.setComboBoxData(s02_0103Da, this.cmbAdMedia, FixedCdYoyaku.CodeBunruiTypeAdMedia);
	}

	/// <summary>
	/// 画面表示設定
	/// 「表示」ボタン押下時
	/// </summary>
	private void setScreenInfo()
	{
		// 予約情報ヘッダー部設定
		this.setYoyakuInfo();
		// 予約情報設定
		this.setEditYoyakuInfo(this._CrsKind);
		// メッセージ情報設定
		this.setEditMessageInfo();
		// ピックアップ設定
		this.setEditPickupInfo();
		// 代理店設定
		this.setEditDairitenInfo(this._yoyakuInfoBasic);
		// 予約者情報設定
		this.setYoyakushaInfo(this._yoyakuInfoBasic);
		// メモ情報設定
		this.setMemoInfo();
	}

	/// <summary>
	/// コース情報取得
	/// </summary>
	private void setCrsInfo()
	{
		this._crsInfo = CommonRegistYoyaku.getYoyakuInfoData(this._YoyakuKbn, this._YoyakuNo);
	}

	/// <summary>
	/// 予約情報ヘッダー部設定
	/// </summary>
	private void setYoyakuInfo()
	{

		// 予約情報データ取得
		this._yoyakuInfoBasic = this.getYoyakuInfoData();
		// コースコード
		this._CrsCd = System.Convert.ToString(_yoyakuInfoBasic.Rows(0).Item("CRS_CD").ToString().Trim());
		// 号車
		if (Information.IsNumeric(_yoyakuInfoBasic.Rows(0).Item("GOUSYA")) == true)
		{
			this._Gousya = int.Parse(_yoyakuInfoBasic.Rows(0).Item("GOUSYA").ToString());
		}
		// 出発日
		if (Information.IsNumeric(_yoyakuInfoBasic.Rows(0).Item("SYUPT_DAY")) == true)
		{
			this._SyuptDay = int.Parse(_yoyakuInfoBasic.Rows(0).Item("SYUPT_DAY").ToString());
		}
		this._CrsKind = System.Convert.ToString(this._crsInfo.Rows(0)["CRS_KIND"].ToString());

		// 予約情報ヘッダー部設定
		this.setYoyakuHeaderPart(this._CrsKind);

		int seikiChargeGaku = CommonRegistYoyaku.convertObjectToInteger(this._yoyakuChangeHistry.Rows(_row - 1).Item("SEIKI_CHARGE_ALL_GAKU")); //正規料金総額
		int waribkiGaku = CommonRegistYoyaku.convertObjectToInteger(this._yoyakuChangeHistry.Rows(_row - 1)["WARIBIKI_ALL_GAKU"]); //割引総額
		int addChargeMaebaraiKei = CommonRegistYoyaku.convertObjectToInteger(this._yoyakuChangeHistry.Rows(_row - 1)["ADD_CHARGE_MAEBARAI_KEI"]); //追加料金前払計
		int kingaku = CommonRegistYoyaku.convertObjectToInteger(this._yoyakuChangeHistry.Rows(_row - 1)["KINGAKU"]); //キャンセル料
		int comulative = CommonRegistYoyaku.convertObjectToInteger(this._yoyakuChangeHistry.Rows(_row - 1)["CUMULATIVE"]); //キャンセル料計
		int cancelPer = CommonRegistYoyaku.convertObjectToInteger(this._yoyakuChangeHistry.Rows(_row - 1)["CANCEL_PER"]); //キャンセル率
																														  //金額
		this.txtCrsKingaku.Text = seikiChargeGaku.ToString(System.Convert.ToString(CommonRegistYoyaku.MoneyFormat));
		//予約時割引
		this.txtCrsYoyakuJiWaribiki.Text = waribkiGaku.ToString(System.Convert.ToString(CommonRegistYoyaku.MoneyFormat));
		//合計金額
		this.txtCrsTotalKingaku.Text = (seikiChargeGaku - waribkiGaku + addChargeMaebaraiKei).ToString(System.Convert.ToString(CommonRegistYoyaku.MoneyFormat));
		//キャンセル料
		this.txtCrsCancelRyou.Text = kingaku.ToString(System.Convert.ToString(CommonRegistYoyaku.MoneyFormat));
		//キャンセル率
		this.numCanselPar.Value = cancelPer;

		//キャンセル総額
		this.txtCanselAllGaku.Text = comulative.ToString(System.Convert.ToString(CommonRegistYoyaku.MoneyFormat));

		//座席
		this.txtCrsZaseki.Text = this._yoyakuChangeHistry.Rows(_row - 1)["ZASEKI"].ToString();

		//無効
		if (this._yoyakuChangeHistry.Rows(_row - 1)["CANCEL_RYOU_INVALID_FLG"].ToString().Trim() == CommonRegistYoyaku.FlagValueTrue)
		{
			this.chkCanselRyouInvalidFlg.Checked = true;
		}
		else
		{
			this.chkCanselRyouInvalidFlg.Checked = false;
		}
		this.chkCanselRyouInvalidFlg.Enabled = true; //一旦、無効チェックボックスを活性化
													 //　非活性制御
		if (_seq == -1 || CommonRegistYoyaku.convertObjectToInteger(this._yoyakuInfoBasic.Rows(0)["HAKKEN_NAIYO"]) != FixedCdYoyaku.HakkenNaiyo.allkinHakken)
		{
			//・発券内容 ≠ 1、SEQ = 0 の場合：非活性
			this.chkCanselRyouInvalidFlg.Enabled = false;
		}
		if (this._yoyakuChangeHistry.Rows(_row - 1)["CANCEL_RYOU_INVALID_FLG"].ToString().Trim() == CommonRegistYoyaku.FlagValueTrue)
		{
			//【表示している履歴情報.キャンセル無効フラグ = Y の場合】
			for (i = _row; i >= 1; i--)
			{
				//表示している履歴情報.SEQ > キャンセル修正履歴一覧.SEQ かつキャンセル無効フラグ = Yの履歴がある場合：   非活性
				if (this._yoyakuChangeHistry.Rows(_row - 1)["CANCEL_RYOU_INVALID_FLG"].ToString().Trim() == CommonRegistYoyaku.FlagValueTrue)
				{
					this.chkCanselRyouInvalidFlg.Enabled = false;
					break;
				}
			}
		}
		else
		{
			//【表示している履歴情報.キャンセル無効フラグ = Y 以外の場合】

			//表示している履歴情報.SEQ ≠ 1または 表示している履歴情報の直前の履歴情報.キャンセル無効フラグ ≠ Yの場合：   非活性

			{
				(_row - 1 < 0 && this._yoyakuChangeHistry.Rows(_row - 1)("CANCEL_RYOU_INVALID_FLG").ToString().Trim() != CommonRegistYoyaku.FlagValueTrue);
				this.chkCanselRyouInvalidFlg.Enabled = false;
			}
		}
		if (this._yoyakuChangeHistry.Rows(_row - 1)["SEIKYU_ALREADY_FLG"].ToString().Trim() == CommonRegistYoyaku.FlagValueTrue)
		{
			this.chkSeikyuAlreadyFlg.Checked = true;
		}
		else
		{
			this.chkSeikyuAlreadyFlg.Checked = false;
		}

		CalcCancel();
		CalcCancelAll();

	}

	/// <summary>
	/// 予約情報ヘッダー部設定
	/// </summary>
	/// <param name="crsKind">コース種別</param>
	private void setYoyakuHeaderPart(string crsKind)
	{

		// 出発日
		string syupt = System.Convert.ToString(CommonRegistYoyaku.convertDateFormat(this._yoyakuInfoBasic.Rows(0)["SYUPT_DAY"].ToString(), CommonRegistYoyaku.DateFromatDayOfTheWeek));
		if (crsKind == CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.CrsKind.kikakuStay))
		{

			string kichaku = System.Convert.ToString(CommonRegistYoyaku.convertDateFormat(this._crsInfo.Rows(0)["RETURN_DAY"].ToString(), CommonRegistYoyaku.DateFromatDayOfTheWeek));
			this.lblSyuptDay.Text = string.Format(CommonRegistYoyaku.SyuptDayFormatKikakuStay, syupt, kichaku);
		}
		else
		{

			this.lblSyuptDay.Text = syupt;
		}

		// コースコード
		this.lblCrsCd.Text = CommonRegistYoyaku.convertObjectToString(this._crsInfo.Rows(0)["CRS_CD"]);
		// コース名
		this.lblCrsName.Text = CommonRegistYoyaku.convertObjectToString(this._crsInfo.Rows(0)["CRS_NAME"]);
		// 号車
		this.lblGousya.Text = string.Format(System.Convert.ToString(CommonRegistYoyaku.GousyaFormat), this._crsInfo.Rows(0)["GOUSYA"]);

	}

	/// <summary>
	/// コース台帳(基本)情報取得
	/// </summary>
	/// <returns>コース台帳(基本)情報取得</returns>
	private DataTable getCrsInfo()
	{

		// パラメータ作成
		CrsLedgerBasicEntity entity = new CrsLedgerBasicEntity();
		entity.crsCd.Value = this._CrsCd;
		entity.syuptDay.Value = this._SyuptDay;
		entity.gousya.Value = this._Gousya;
		S02_0103Da s02_0103Da = new S02_0103Da();
		DataTable crsInfo = s02_0103Da.getCrsLedgerBasicData(entity);

		return crsInfo;
	}

	/// <summary>
	/// 予約変更履歴（料金情報）取得
	/// </summary>
	/// <returns>予約変更履歴（料金情報）データ</returns>
	private DataTable getYoyakuChangeHistryData()
	{

		YoyakuChangeHistoryEntity entity = new YoyakuChangeHistoryEntity();
		entity.yoyakuKbn.Value = this._YoyakuKbn;
		entity.yoyakuNo.Value = this._YoyakuNo;

		S02_1201Da S02_1201Da = new S02_1201Da();
		DataTable yoyakuInfoData = S02_1201Da.getYoyakuChangeHistory(entity);

		return yoyakuInfoData;
	}

	/// <summary>
	/// 予約情報データ取得
	/// </summary>
	/// <returns>予約情報データ</returns>
	private DataTable getYoyakuInfoData()
	{

		YoyakuInfoBasicEntity entity = new YoyakuInfoBasicEntity();
		entity.yoyakuKbn.Value = _YoyakuKbn;
		entity.yoyakuNo.Value = _YoyakuNo;

		S02_1201Da S02_1201Da = new S02_1201Da();
		DataTable yoyakuInfoData = S02_1201Da.getYoyakuInfoBasic(entity);

		return yoyakuInfoData;
	}

	/// <summary>
	/// コースパラメータ作成
	/// </summary>
	/// <param name="crsCd">コースコード</param>
	/// <param name="gousya">号車</param>
	/// <param name="syuptDay">出発日</param>
	/// <returns>コースパラメータ</returns>
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
	/// 乗車地データテーブル作成
	/// 新規用
	/// </summary>
	/// <param name="crsInfo">コース情報</param>
	/// <returns>乗車地データテーブル</returns>
	private DataTable createNewJoshatiDataTable(DataTable crsInfo)
	{

		DataTable dt = this.createtJoshatiDataTable();

		string jyochachiColName = "";
		string placeColName = "";
		string syugoColName = "";
		string syuptColName = "";
		for (index = 1; index <= 5; index++)
		{

			jyochachiColName = "JYOCHACHI_CD_" + index.ToString();
			placeColName = "PLACE_NAME_SHORT_" + index.ToString();
			syugoColName = "SYUGO_TIME_" + index.ToString();
			syuptColName = "SYUPT_TIME_" + index.ToString();

			if (string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)[jyochachiColName].ToString())) == true)
			{
				// 乗車地コードに値がない場合、次レコードへ
				continue;
			}

			DataRow row = dt.NewRow();
			row["JYOCHACHI_CD"] = crsInfo.Rows(0)[jyochachiColName].ToString();
			row["PLACE_NAME_SHORT"] = crsInfo.Rows(0)[placeColName].ToString();
			row["SYUGO_TIME"] = CommonRegistYoyaku.convertTimeString(crsInfo.Rows(0)[syugoColName]);
			row["SYUPT_TIME"] = CommonRegistYoyaku.convertTimeString(crsInfo.Rows(0)[syuptColName]);
			row["JYOSYA_NINZU"] = "";

			dt.Rows.Add(row);
		}

		return dt;
	}

	/// <summary>
	/// 乗車地のDataTable作成
	/// </summary>
	/// <returns>乗車地のDataTable</returns>
	private DataTable createtJoshatiDataTable()
	{

		DataTable dt = new DataTable();
		dt.Columns.Add("JYOCHACHI_CD");
		dt.Columns.Add("PLACE_NAME_SHORT");
		dt.Columns.Add("SYUGO_TIME");
		dt.Columns.Add("SYUPT_TIME");
		dt.Columns.Add("JYOSYA_NINZU");

		return dt;
	}

	/// <summary>
	/// 空部屋数設定
	/// </summary>
	/// <param name="row">部屋リストバインド用DataRow</param>
	/// <returns>部屋リストバインド用DataRow</returns>
	private DataRow setEmptyRoomsu(DataRow row)
	{

		row = this.setRoomSu(row, "ROOM_ZANSU_ONE_ROOM");
		row = this.setRoomSu(row, "ROOM_ZANSU_TWO_ROOM");
		row = this.setRoomSu(row, "ROOM_ZANSU_THREE_ROOM");
		row = this.setRoomSu(row, "ROOM_ZANSU_FOUR_ROOM");
		row = this.setRoomSu(row, "ROOM_ZANSU_FIVE_ROOM");

		return row;
	}

	/// <summary>
	/// 部屋数設定
	/// </summary>
	/// <param name="row">部屋リストバインド用DataRow</param>
	/// <param name="colName">カラム名</param>
	/// <returns>部屋リストバインド用DataRow</returns>
	private DataRow setRoomSu(DataRow row, string colName)
	{

		if (this._yoyakuInfoBasic.Rows(0)[colName] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(this._yoyakuInfoBasic.Rows(0)[colName].ToString())) == false)
			{

			row[colName] = string.Format(System.Convert.ToString(CommonRegistYoyaku.EmptyRoomSuDisplayFormat), this._yoyakuInfoBasic.Rows(0)[colName].ToString());
		}
			else
		{

			row[colName] = CommonRegistYoyaku.Hyphen;
		}

		return row;
	}

	/// <summary>
	/// 部屋グリッドヘッダ結合設定
	/// </summary>
	private void setMergedCellRoomList()
	{

		this.grdRoomList.AllowMerging = AllowMergingEnum.Custom;
		CellRange cr = null;
		// 1名1室ヘッダ結合
		cr = this.grdRoomList.GetCellRange(0, 1, 0, 2);
		this.grdRoomList.MergedRanges.Add(cr);
		// 2名1室ヘッダ結合
		cr = this.grdRoomList.GetCellRange(0, 3, 0, 4);
		this.grdRoomList.MergedRanges.Add(cr);
		// 3名1室ヘッダ結合
		cr = this.grdRoomList.GetCellRange(0, 5, 0, 6);
		this.grdRoomList.MergedRanges.Add(cr);
		// 4名1室ヘッダ結合
		cr = this.grdRoomList.GetCellRange(0, 7, 0, 8);
		this.grdRoomList.MergedRanges.Add(cr);
		// 5名1室ヘッダ結合
		cr = this.grdRoomList.GetCellRange(0, 9, 0, 10);
		this.grdRoomList.MergedRanges.Add(cr);
	}

	/// <summary>
	/// 予約人数一覧設定
	/// </summary>
	private void setEditYoyakuNinzuList()
	{

		DataTable dt = new DataTable();
		foreach (Column col in this.grdYoyakuNinzuList.Cols)
		{

			dt.Columns.Add(col.Name);
		}

		DataRow row = dt.NewRow();
		foreach (DataRow chargeRow in this._chargeKbnList.Rows)
		{

			string colName = System.Convert.ToString(chargeRow["CHARGE_KBN_JININ_CD"].ToString());
			int ninzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(chargeRow["CHARGE_APPLICATION_NINZU"]));

			row[colName] = ninzu;
			this._yoyakuNinzu = this._yoyakuNinzu + ninzu;
		}

		dt.Rows.Add(row);

		this.grdYoyakuNinzuList.DataSource = dt;
	}

	/// <summary>
	/// 部屋(宿泊あり)一覧設定
	/// 更新用
	/// </summary>
	private void setEditRoomList()
	{

		// 部屋グリッドヘッダ結合設定
		this.setMergedCellRoomList();

		DataTable dt = new DataTable();
		foreach (Column col in this.grdRoomList.Cols)
		{

			dt.Columns.Add(col.Name);
		}

		DataRow row = dt.NewRow();

		row = this.setEmptyRoomsu(row);

		if (this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_1"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_1"].ToString())) == false)
			{

			row["ROOMING_BETU_NINZU_1"] = this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_1"].ToString();
		}

		if (this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_2"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_2"].ToString())) == false)
			{

			row["ROOMING_BETU_NINZU_2"] = this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_2"].ToString();
		}

		if (this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_3"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_3"].ToString())) == false)
			{

			row["ROOMING_BETU_NINZU_3"] = this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_3"].ToString();
		}

		if (this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_4"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_4"].ToString())) == false)
			{

			row["ROOMING_BETU_NINZU_4"] = this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_4"].ToString();
		}

		if (this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_5"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_5"].ToString())) == false)
			{

			row["ROOMING_BETU_NINZU_5"] = this._yoyakuInfoBasic.Rows(0)["ROOMING_BETU_NINZU_5"].ToString();
		}

		dt.Rows.Add(row);

		this.grdRoomList.DataSource = dt;
	}

	/// <summary>
	/// 予約情報設定
	/// 更新用
	/// </summary>
	/// <param name="crsKind">コース種別</param>
	private void setEditYoyakuInfo(string crsKind)
	{

		// 料金区分設定
		if (crsKind == CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.CrsKind.kikakuStay))
		{
			// 宿泊ありの場合
			this.visibleChargeControl(true);

			// 料金区分設定(宿泊あり)
			this.setEditChargeKbnListForShukuhakuAri();
			this.grdRoomList.ShowButtons = ShowButtonsEnum.Always;
		}
		else
		{
			// 宿泊なしの場合
			this.visibleChargeControl(false);

			// 料金区分設定(宿泊なし)
			this.setEditChargeKbnListForShukuhakuNashi();
		}

		// 乗車地設定
		this._joshatiInfo = CommonRegistYoyaku.createJoshatiDataTable(this._crsInfo);
		this.grdNoribaList.DataSource = this._joshatiInfo;
		CommonRegistYoyaku.setGridLength(this.grdNoribaList, "JYOSYA_NINZU", CommonRegistYoyaku.NinzuMaxLength);

		if (this._YoyakuKbn != CommonRegistYoyaku.WaitManagementKbn)
		{
			// WT以外は、料金区分情報は活性化の状態にする

			this.grdYoyakuNinzuList.Enabled = false;
			this.grdNoribaList.Enabled = false;

			this.grdRoomList.Cols("ROOMING_BETU_NINZU_1").AllowEditing = false;
			this.grdRoomList.Cols("ROOMING_BETU_NINZU_2").AllowEditing = false;
			this.grdRoomList.Cols("ROOMING_BETU_NINZU_3").AllowEditing = false;
			this.grdRoomList.Cols("ROOMING_BETU_NINZU_4").AllowEditing = false;
			this.grdRoomList.Cols("ROOMING_BETU_NINZU_5").AllowEditing = false;

			this.chkWomen.Enabled = false;
			this.txtInfant.ReadOnly = true;
			this.chkAibeya.Enabled = false;
		}

		// 女専
		this.chkWomen.Checked = CommonRegistYoyaku.getFlagOnOff(this._crsInfo.Rows(0)["JYOSEI_SENYO"]);
		// インファント
		int infantNinzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(this._crsInfo.Rows(0)["INFANT_NINZU"]));
		if (infantNinzu == CommonRegistYoyaku.ZERO)
		{
			this.txtInfant.Text = CommonRegistYoyaku.ValueEmpty;
		}
		else
		{
			this.txtInfant.Text = CommonRegistYoyaku.convertObjectToString(this._crsInfo.Rows(0)["INFANT_NINZU"]);
		}
		// 相部屋
		this.chkAibeya.Checked = CommonRegistYoyaku.getFlagOnOff(this._crsInfo.Rows(0)["AIBEYA_FLG"]);

		//' 料金区分情報を座席確保時料金区分へコピー
		//Me._zenkaiChargeKbnList = Me._chargeKbnList.Copy()
		//' 予約人数取得
		//Me._yoyakuNinzu = CommonRegistYoyaku.getYoyakuNinzu(Me.ParamData.CrsKind, Me.grdYoyakuNinzuList, Me.grdChargeList)
		//' 予約人数を座席確保時予約人数へコピー
		//Me._zenkaiYoyakuNinzu = Me._yoyakuNinzu

		this.grdYoyakuNinzuList.Enabled = false;
		//Me.grdRoomList.Enabled = False
		this.grdNoribaList.Enabled = false;
	}

	/// <summary>
	/// 料金区分設定
	/// 宿泊あり
	/// </summary>
	private void setEditChargeKbnListForShukuhakuAri()
	{

		// 料金区分一覧取得
		YoyakuChangeHistoryEntity entity = new YoyakuChangeHistoryEntity();
		entity.yoyakuKbn.Value = this._YoyakuKbn;
		entity.yoyakuNo.Value = this._YoyakuNo;
		entity.seq.Value = this._seq;
		entity.crsCd.Value = this._CrsCd;
		entity.syuptDay.Value = this._SyuptDay;
		entity.gousya.Value = this._Gousya;

		S02_1201Da s02_1201Da = new S02_1201Da();
		this._chargeKbnList = s02_1201Da.getYoyakuChargeList(entity);

		//Me._chargeKbnList = CommonRegistYoyaku.getChargeKbnList(Me._YoyakuKbn, Me._YoyakuNo, Me._CrsCd, Me._SyuptDay, Me._Gousya)
		// 宿泊あり用予約人数グリッド作成
		CommonRegistYoyaku.createYoyakuNinzuListGrid(this.grdYoyakuNinzuList, this._chargeKbnList);
		// 予約人数一覧設定
		this._yoyakuNinzu = 0;
		CommonRegistYoyaku.setEditYoyakuNinzuList(this.grdYoyakuNinzuList, this._chargeKbnList, this._yoyakuNinzu);
		// 合計予約人数設定
		this.grdYoyakuNinzuList.SetData(CommonRegistYoyaku.GridStartRowIndex, "YOYAKU_TOTAL_NINZU", this._yoyakuNinzu);
		// 部屋(宿泊あり)一覧設定
		CommonRegistYoyaku.setEditRoomList(this.grdRoomList, this._crsInfo);
	}

	/// <summary>
	/// 料金区分設定
	/// 宿泊なし
	/// </summary>
	private void setEditChargeKbnListForShukuhakuNashi()
	{

		YoyakuChangeHistoryChargeInfoChargeKbnEntity entity = new YoyakuChangeHistoryChargeInfoChargeKbnEntity();
		entity.yoyakuKbn.Value = this._YoyakuKbn;
		entity.yoyakuNo.Value = this._YoyakuNo;
		entity.seq.Value = this._seq;

		S02_1201Da s02_1201Da = new S02_1201Da();
		this._chargeKbnList = s02_1201Da.getYoyakuChargeKbnList(entity);

		this.grdChargeList.DataSource = this._chargeKbnList;

		// 予約人数が0人は表示しない
		foreach (Row row in this.grdChargeList.Rows)
		{

			if (row.Index == CommonRegistYoyaku.ZERO)
			{

				continue;
			}

			int ninzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(this.grdChargeList.GetData(row.Index, "YOYAKU_NINZU")));
			if (ninzu > CommonRegistYoyaku.ZERO)
			{

				continue;
			}

			row.Visible = false;
		}

		this.grdChargeList.Enabled = false;

	}

	/// <summary>
	/// メッセージ一覧設定
	/// 更新用
	/// </summary>
	private void setEditMessageInfo()
	{

		// コース台帳(メッセージ)情報取得
		DataTable messageData = this.getMessageInfo();
		// メッセージデータテーブル作成
		this._msgInfo = this.createEditMessageDataTable(messageData, this._yoyakuInfoBasic);

		this.grdMessageList.DataSource = this._msgInfo;

		DataRow[] msgRow = this._msgInfo.Select(string.Format("LINE_NO = '{0}'", "1"));

		int rowIndex = 0;
		foreach (DataRow row in msgRow)
		{

			CellStyle cellStyle = this.grdMessageList.Styles.Add(string.Format("MsgStyle{0}", rowIndex));
			cellStyle.BackColor = SystemColors.ControlLight;
			this.grdMessageList.SetCellStyle(rowIndex, 0, cellStyle);

			this.grdMessageList.Rows(rowIndex).AllowEditing = false;
			rowIndex++;
		}

		this.cmbAdMedia.SelectedValue = this._yoyakuInfoBasic.Rows(0)["MEDIA_CD"];
	}

	/// <summary>
	/// メッセージ情報取得
	/// </summary>
	/// <returns>メッセージ情報</returns>
	private DataTable getMessageInfo()
	{

		// パラメータ作成
		CrsLedgerMessageEntity entity = new CrsLedgerMessageEntity();
		entity.crsCd.Value = this._CrsCd;
		entity.syuptDay.Value = this._SyuptDay;
		entity.gousya.Value = this._Gousya;
		S02_0103Da s02_0103Da = new S02_0103Da();
		DataTable messageData = s02_0103Da.getCrsInfoMessage(entity);

		return messageData;
	}

	/// <summary>
	/// メッセージデータテーブル作成
	/// </summary>
	/// <param name="messageData">メッセージ情報</param>
	/// <param name="crsInfo">コース情報</param>
	/// <returns>メッセージデータテーブル</returns>
	private DataTable createEditMessageDataTable(DataTable messageData, DataTable crsInfo)
	{

		DataTable dt = CommonRegistYoyaku.createMessageDataTable();

		// ライン0のメッセージ作成
		dt = CommonRegistYoyaku.createLineZeroMessage(dt, crsInfo);

		DataRow row = null;

		string msgColName = "";
		int idx = 1;

		foreach (DataRow msgRow in messageData.Rows)
		{

			msgColName = string.Format("MESSAGE_CHECK_FLG_{0}", idx);

			row = dt.NewRow();

			if (string.IsNullOrEmpty(System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(crsInfo.Rows(0)[msgColName]))) == true)
			{

				row["MESSAGE_CHECK_FLG"] = false;
			}
			else
			{

				row["MESSAGE_CHECK_FLG"] = true;
			}

			row["LINE_NO"] = CommonRegistYoyaku.convertObjectToString(msgRow["LINE_NO"]);
			row["MESSAGE"] = CommonRegistYoyaku.convertObjectToString(msgRow["MESSAGE"]);

			dt.Rows.Add(row);
			idx++;
		}

		return dt;

	}

	/// <summary>
	/// インフォメッセージ作成
	/// </summary>
	/// <param name="crsInfo">コース情報</param>
	/// <returns>インフォメッセージ</returns>
	private string createInfoMessage(DataTable crsInfo)
	{

		string infoMsg = "";

		// １８才未満禁
		if (string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)["UNDER_KINSI_18OLD"].ToString())) == false)
		{

			infoMsg = "18ｻｲ";
		}

		// 上着着用
		if (string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)["UWAGI_TYAKUYO"].ToString())) == false)
		{

			infoMsg = infoMsg + "ｾﾋﾞﾛ";
		}

		// ネクタイ着用
		if (string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)["TIE_TYAKUYO"].ToString())) == false)
		{

			infoMsg = infoMsg + "ﾈｸﾀｲ";
		}

		// 前売期限
		if (string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)["MAEURI_KIGEN"].ToString())) == false)
		{

			infoMsg = infoMsg + "DDﾆﾁ";
		}

		return infoMsg;
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

	/// <summary>
	/// 予約変更履歴一覧設定
	/// </summary>
	private void setYoyakuChangeHistry()
	{
		string formatedCount = string.Empty;
		// 予約情報データ取得
		this._yoyakuChangeHistry = this.getYoyakuChangeHistryData();
		this.grdYoyakuHiostry.DataSource = _yoyakuChangeHistry;
		if (ReferenceEquals(_yoyakuChangeHistry.Rows.Count, null) == false)
		{
			formatedCount = System.Convert.ToString(_yoyakuChangeHistry.Rows.Count.ToString().PadLeft(6));
			this.lblLengthGrd01.Text = formatedCount + "件";
		}

	}

	/// <summary>
	/// 予約者情報設定
	/// </summary>
	/// <param name="crsInfo">コース予約情報</param>
	private void setYoyakushaInfo(DataTable crsInfo)
	{
		this.txtMembersId.Text = crsInfo.Rows(0)["TOMONOKAI_NO"].ToString();
		this.cmbKokuseki.SelectedValue = crsInfo.Rows(0)["KOKUSEKI"];
		this.cmbSeisanDairiten.SelectedValue = crsInfo.Rows(0)["AGENT_SEISAN_KBN"];
		this.txtSurnameNameHalfsize.Text = crsInfo.Rows(0)["SURNAME"].ToString();
		this.txtNameHalfsize.Text = crsInfo.Rows(0)["NAME"].ToString();
		this.txtKjSurname.Text = crsInfo.Rows(0)["SURNAME_KJ"].ToString();
		this.txtKjName.Text = crsInfo.Rows(0)["NAME_KJ"].ToString();
		this.cmbSex.SelectedValue = crsInfo.Rows(0)["SEX_BETU"];
		this.txtTel1.Text = crsInfo.Rows(0)["TEL_NO_1"].ToString();
		this.txtTel2.Text = crsInfo.Rows(0)["TEL_NO_2"].ToString();
		this.txtMail.Text = crsInfo.Rows(0)["MAIL_ADDRESS"].ToString().Trim();

		if (crsInfo.Rows(0)["MAIL_SENDING_KBN"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)["MAIL_SENDING_KBN"].ToString())) == false)
			{

			this.chkMailNotificationHope.Checked = true;
		}

		if (crsInfo.Rows(0)["YUBIN_NO"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)["YUBIN_NO"].ToString())) == false)
			{

			this.txtYubinNoUp.Text = crsInfo.Rows(0)["YUBIN_NO"].ToString().Substring(0, 3);
			this.txtYubinNoLow.Text = crsInfo.Rows(0)["YUBIN_NO"].ToString().Substring(3);
		}

		if (crsInfo.Rows(0)["MOSHIKOMI_HOTEL_FLG"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)["MOSHIKOMI_HOTEL_FLG"].ToString())) == false)
			{

			this.chkYykmksHotelName.Checked = true;
		}

		this.txtTodouhukenSikutyouson.Text = crsInfo.Rows(0)["ADDRESS_1"].ToString();
		this.txtBanti.Text = crsInfo.Rows(0)["ADDRESS_2"].ToString();
		this.txtBuilding.Text = crsInfo.Rows(0)["ADDRESS_3"].ToString();
		this.txtYykmks.Text = crsInfo.Rows(0)["YYKMKS"].ToString();
		this.cmbSiharaiHoho.SelectedValue = crsInfo.Rows(0)["SEISAN_HOHO"].ToString();

		if (crsInfo.Rows(0)["FURIKOMIYOSHI_YOHI_FLG"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)["FURIKOMIYOSHI_YOHI_FLG"].ToString())) == false)
			{

			this.chkHurikomiHyo.Checked = true;
		}

		if (crsInfo.Rows(0)["SEIKYUSYO_YOHI_FLG"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)["SEIKYUSYO_YOHI_FLG"].ToString())) == false)
			{

			this.chkSeikyusyo.Checked = true;
		}

		if ((crsInfo.Rows(0)["RELATION_YOYAKU_KBN"] IsNot null && string.IsNullOrEmpty(System.Convert.ToString(crsInfo.Rows(0)["RELATION_YOYAKU_KBN"].ToString())) == false) &&)
			{
			(crsInfo.Rows(0)("RELATION_YOYAKU_NO")IsNotnull && string.IsNullOrEmpty(crsInfo.Rows(0)("RELATION_YOYAKU_NO").ToString()) == false);

			string relationYoyakuNo = System.Convert.ToString(CommonRegistYoyaku.convertObjectToInteger(crsInfo.Rows(0)["RELATION_YOYAKU_NO"]).ToString("D9"));

			this.txtRelationNo.Text = crsInfo.Rows(0)["RELATION_YOYAKU_KBN"].ToString() + relationYoyakuNo;
		}
	}

	/// <summary>
	/// ピックアップ一覧設定
	/// </summary>
	private void setEditPickupInfo()
	{
		YoyakuInfoBasicEntity entity = new YoyakuInfoBasicEntity();
		entity.yoyakuKbn.Value = this._YoyakuKbn;
		entity.yoyakuNo.Value = this._YoyakuNo;
		entity.syuptDay.Value = this._SyuptDay;
		S02_0107Da s02_0107Da = new S02_0107Da();
		DataTable yoyakuPickup = s02_0107Da.getPickupList(entity);

		this.grdPickup.DataSource = yoyakuPickup;

	}

	/// <summary>
	/// メモ一覧設定
	/// </summary>
	private void setMemoInfo()
	{

		DataTable yoyakuMemoList = CommonRegistYoyaku.getYoyakuMemoList(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
		this.grdMemoList.DataSource = yoyakuMemoList;
	}

	/// <summary>
	/// 代理店情報設定
	/// </summary>
	/// <param name="crsInfo">コース予約情報</param>
	private void setEditDairitenInfo(DataTable crsInfo)
	{

		this.txtDairitenCd.Text = CommonRegistYoyaku.convertObjectToString(crsInfo.Rows(0)["AGENT_CD"]);
		this.txtDairitenName.Text = CommonRegistYoyaku.convertObjectToString(crsInfo.Rows(0)["AGENT_NM"]);
		this.cmbSeisanDairiten.SelectedValue = crsInfo.Rows(0)["AGENT_SEISAN_KBN"];
		this.txtDairitenTel.Text = CommonRegistYoyaku.convertObjectToString(crsInfo.Rows(0)["AGENT_TEL_NO"]);
		this.txtDairitenTanto.Text = CommonRegistYoyaku.convertObjectToString(crsInfo.Rows(0)["AGENT_TANTOSYA"]);
		this.txtDairitenToursNo.Text = CommonRegistYoyaku.convertObjectToString(crsInfo.Rows(0)["TOURS_NO"]);

	}

	/// <summary>
	/// 検索処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckSearch()
	{
		string yoyakuNo = this._YoyakuKbn + this._YoyakuNo.ToString().Trim(); //Me.ucoCrsYoyakuNo.YoyakuText
		string yoyakuKbn = this._YoyakuKbn;
		int yoyakuNum = this._YoyakuNo;

		this.ucoCrsYoyakuNo.YoyakuText = this._YoyakuKbn + this._YoyakuNo.ToString().Trim();
		object with_1 = this.ucoCrsYoyakuNo;
		//前回エラークリア
		with_1.ExistError = false;
		if (string.IsNullOrEmpty(System.Convert.ToString(with_1.YoyakuText)) == true)
		{
			with_1.ExistError = true;
			with_1.Focus();
			//予約番号が指定されていません。</MsgString>
			CommonProcess.createFactoryMsg().messageDisp("E90_008", "予約番号");
			return false;
		}

		YoyakuInfoBasicEntity entity = new YoyakuInfoBasicEntity();
		entity.yoyakuKbn.Value = yoyakuKbn;
		entity.yoyakuNo.Value = yoyakuNum;

		S02_1201Da s02_1201Da = new S02_1201Da();
		YoyakuInfoBasic_DA yoyakuInfoBasicDa = new YoyakuInfoBasic_DA();

		this._yoyakuInfoBasic = s02_1201Da.getYoyakuInfoBasic(entity);

		//チェック用件
		if (_yoyakuInfoBasic.Rows.Count <= CommonRegistYoyaku.ZERO)
		{
			with_1.ExistError = true;
			with_1.Focus();
			//該当の{予約番号}が存在しません。
			CommonProcess.createFactoryMsg().messageDisp("E90_014", "予約番号");
			return false;
		}


		//コース情報取得
		this.setCrsInfo();
		setYoyakuHeaderPart(System.Convert.ToString(_yoyakuInfoBasic.Rows(0).Item("CRS_Kind").ToString().Trim()));
		return true;
	}

	/// <summary>
	/// 料金区分の表示・非表示
	/// </summary>
	/// <param name="shukuhakuFlag">宿泊フラグ</param>
	private void visibleChargeControl(bool shukuhakuFlag)
	{

		this.grdChargeList.Visible = !shukuhakuFlag;
		this.grdYoyakuNinzuList.Visible = shukuhakuFlag;
		this.lblRoom.Visible = shukuhakuFlag;
		this.grdRoomList.Visible = shukuhakuFlag;
		this.chkAibeya.Visible = shukuhakuFlag;

	}

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
		string customName = string.Format("Combo{0}", rowIndex);
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
		DataRow[] rows = this._chargeKbnList.Select(string.Format("KBN_NO = {0} AND CHARGE_KBN_JININ_CD = {1} ", kbnNo, jininCd));
		// 料金取得
		int charge = int.Parse(System.Convert.ToString(rows[0]["CHARGE"].ToString()));
		// 料金設定
		this.grdChargeList.SetData(rowIndex, "CHARGE", charge.ToString(System.Convert.ToString(CommonRegistYoyaku.MoneyFormat)));
		this.grdChargeList.SetData(rowIndex, "CHARGE_SUB_SEAT", rows[0]["CHARGE_SUB_SEAT"]);
		this.grdChargeList.SetData(rowIndex, "CHARGE_KBN", rows[0]["CHARGE_KBN"]);
		this.grdChargeList.SetData(rowIndex, "CARRIAGE", rows[0]["CARRIAGE"]);
		this.grdChargeList.SetData(rowIndex, "CARRIAGE_SUB_SEAT", rows[0]["CARRIAGE_SUB_SEAT"]);
		this.grdChargeList.SetData(rowIndex, "CHARGE_KBN_JININ_CD_URA", jininCd);
		this.grdChargeList.SetData(rowIndex, "SEX_BETU", rows[0]["SEX_BETU"]);
	}

	/// <summary>
	/// キャンセル料金計算
	/// </summary>
	private void CalcCancel()
	{
		int tanka = 0;
		int canselNinzu = 0;
		int canselRyou = 0;

		if (Information.IsNumeric(this.numCanselPar.Value) == false ||)
		{
			Information.IsNumeric(this.numCanselPar.Value) = true && System.Convert.ToInt32(this.numCanselPar.Value) == 0;
			return;
		}
		//入力値と連動して下記数式で算出。
		foreach (DataRow row in this._chargeKbnList.Rows)
		{
			//①(単価×キャンセル料率100) を予約一人ごと（1円未満切捨て）に行う
			if (((FixedCdYoyaku.CrsKind)this._CrsKind) == FixedCdYoyaku.CrsKind.kikakuStay)
			{
				//宿泊有り
				if (row["TANKA_1"].ToString() != "" && Information.IsNumeric(row["TANKA_1"]) == true)
				{
					tanka = System.Convert.ToInt32(Convert.ToInt32(row["TANKA_1"]));
				}
				else
				{
					tanka = 0;
				}
				if (row["CANCEL_NINZU_1"].ToString() != "" && Information.IsNumeric(row["CANCEL_NINZU_1"]) == true)
				{
					canselNinzu = System.Convert.ToInt32(Convert.ToInt32(row["CANCEL_NINZU_1"]));
				}
				else
				{
					canselNinzu = 0;
				}
			}
			else
			{
				//宿泊無し
				if (row["CHARGE"].ToString() != "" && Information.IsNumeric(row["CHARGE"]) == true)
				{
					tanka = System.Convert.ToInt32(Convert.ToInt32(row["CHARGE"]));
				}
				else
				{
					tanka = 0;
				}
				if (row["CANCEL_NINZU"].ToString() != "" && Information.IsNumeric(row["CANCEL_NINZU"]) == true)
				{
					canselNinzu = System.Convert.ToInt32(Convert.ToInt32(row["CANCEL_NINZU"]));
				}
				else
				{
					canselNinzu = 0;
				}
			}
			//②(①で算出した値の合計)
			for (i = 1; i <= canselNinzu; i++)
			{
				canselRyou += System.Convert.ToInt32(Convert.ToInt32(Int(tanka * numCanselPar.Value / 100)));
			}
		}
		this.txtCrsCancelRyou.Text = canselRyou.ToString(System.Convert.ToString(CommonRegistYoyaku.MoneyFormat));

	}

	/// <summary>
	/// キャンセル料合計計算
	/// </summary>
	private void CalcCancelAll()
	{
		//キャンセル料合計
		int canselRyou = 0; //キャンセル料
		int kingaku = 0; //キャンセル料
		int comulative = 0; //キャンセル料計
		System.Globalization.NumberStyles fmtInteger = System.Globalization.NumberStyles.Integer | System.Globalization.NumberStyles.AllowThousands;

		if (Information.IsNumeric(this.txtCrsCancelRyou.Text) == true)
		{
			canselRyou = int.Parse(System.Convert.ToString(this.txtCrsCancelRyou.Text), fmtInteger); //キャンセル料（画面）
		}
		kingaku = CommonRegistYoyaku.convertObjectToInteger(this._yoyakuChangeHistry.Rows(_row - 1)["KINGAKU"]); //キャンセル料（テーブル）
		comulative = CommonRegistYoyaku.convertObjectToInteger(this._yoyakuChangeHistry.Rows(_row - 1)["CUMULATIVE"]); //キャンセル料計（テーブル）

		//項目表示制御                                                            TODO 権限制御決定後修正
		if (chkCanselRyouInvalidFlg.Checked == true)
		{
			//【無効フラグをONにした場合
			//または 変更していない場合】
			//キャンセル修正履歴一覧.キャンセル料計
			//- キャンセル修正履歴一覧.キャンセル料
			//+ 画面の値を基に算出したキャンセル料
			this.txtCancelRyouTotal.Text = (comulative - kingaku + canselRyou).ToString(System.Convert.ToString(CommonRegistYoyaku.MoneyFormat));
		}
		else
		{
			//【無効フラグをOFFにした場合】
			//画面の値を基に算出したキャンセル料
			this.txtCancelRyouTotal.Text = canselRyou.ToString(System.Convert.ToString(CommonRegistYoyaku.MoneyFormat));
		}
		//Else
		//    '■発券後修正不可画面の場合
		//    'キャンセル修正履歴一覧.キャンセル料計
		//    '- キャンセル修正履歴一覧.キャンセル料
		//    '+ 画面の値を基に算出したキャンセル料
		//    Me.txtCancelRyouTotal.Text = (comulative - kingaku + canselRyou).ToString(CommonRegistYoyaku.MoneyFormat)
		//End If
	}

	/// <summary>
	/// メモ分類コンボボックスSelectedIndexChangedイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void cmbBunrui_SelectedIndexChanged(object sender, EventArgs e)
	{

		string value = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(this.cmbBunrui.SelectedValue));

		if (string.IsNullOrWhiteSpace(value) == true)
		{
			// 空の場合は、フィルタをクリア
			this.grdMemoList.ClearFilter();
			return;
		}

		// グリッドのフィルタリングを有効にします
		this.grdMemoList.AllowFiltering = true;
		// 新しいValueFilterを作成します
		ValueFilter filter = new ValueFilter();
		filter.ShowValues = new object[] { value };
		// 新しいフィルタを1列目に割り当てます
		this.grdMemoList.Cols("MEMO_BUNRUI").Filter = filter;
		// フィルタ条件を適用します
		this.grdMemoList.ApplyFilters();
	}
	#endregion

}