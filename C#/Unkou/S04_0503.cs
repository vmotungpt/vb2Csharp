using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.Master;


public class S04_0503 : PT99, iPT99
{

	#region フィールド
	#endregion

	#region 定数
	private const string FORMAT_DATE = "yy/MM/dd";
	private const string E04_006_PARAM1 = "予備申請金額";
	private const string E04_006_PARAM2 = "100,000";
	private const string E04_008_PARAM1 = "準備金申請書";
	private const string E04_008_PARAM2 = "準備金受渡明細表";
	private const int TOTAL_FIELD_MAX_LENGTH = 9; //申請金額合計、最大桁数（9桁）
	#endregion

	#region 列挙体

	#endregion

	#region 画面に実装(Interface)
	/// <summary>
	/// パターンの初期設定を行う
	/// (呼出し元から呼び出される想定)
	/// </summary>
	void iPT99.iPt99StartSetting()
	{
		this.patternSettings();
	}

	public void patternSettings()
	{
		//[ボタン制御用]
		//[CSV出力](F4)有無設定
		base.PtIsCsvOutFlg = false;
		//[ﾌﾟﾚﾋﾞｭｰ](F5)有無設定
		base.PtIsPrevFlg = false;
		//[印刷/出力](F7)有無設定
		base.PtIsPrintFlg = true;
		//[検索](F8)有無設定
		base.PtIsSearchFlg = false;
		//[登録](F10)有無設定
		base.PtIsRegFlg = true;
		//[更新](F11)有無設定
		base.PtIsUpdFlg = false;
		//[表示・非表示ボタン]
		base.PtDisplayBtn = null;

		//[検索領域]コントロール設定
		base.PtSearchControl = this.gbxSearch;
		//[検索結果領域]コントロール設定
		base.PtResultControl = null;
		//[検索詳細領域]コントロール設定
		base.PtDetailControl = null;
		//[件数表示ラベル]コントロール設定
		base.PtResultLblCnt = null;
		//[検索結果Max件数]コントロール設定
		base.PtMaxCount = null;
		//[検索結果Grid]コントロール設定
		base.PtResultGrid = null;

		//[帳票用]帳票タイプ
		base.PtPrintType = PRINTTYPE.AR;
		//[帳票用]DS用帳票ID
		base.PtDsPrintId = null;

		//[変更チェック]カラム設定
		base.PtDiffChkColName = null;

		//親画面に自分を設定
		base.PtMyForm = this;

	}

	/// <summary>
	/// データデフォルト値設定
	/// </summary>
	/// <param name="area"></param>
	void iPT99.iPt99SetDefValue(AREA area)
	{
		this.setDefValue(area);
	}

	public void setDefValue(AREA area)
	{
		if (area == area.FORM)
		{
			//初期値：「準備金申請」が選択状態
			this.rdoJunbikinSinsei.Checked = true;
			//申請区分：ログインユーザーの所属部署により以下となる
			if (string.Equals(System.Convert.ToString(UserInfoManagement.toriatukaiBusyo), System.Convert.ToString(FixedCd.ToriatsukaiBusyo.teiki)))
			{
				rdoTeikiKanko.Checked = true;
			}
			else if (string.Equals(System.Convert.ToString(UserInfoManagement.toriatukaiBusyo), System.Convert.ToString(FixedCd.ToriatsukaiBusyo.kikaku)))
			{
				rdoKikakuRyoko.Checked = true;
			}
			else if (string.Equals(System.Convert.ToString(UserInfoManagement.toriatukaiBusyo), System.Convert.ToString(FixedCd.ToriatsukaiBusyo.world)))
			{
				rdoKokusaiJigyo.Checked = true;
			}
			//初期値：「準備金申請チェックリスト」が選択状態
			this.rdoShinseiCheckList.Checked = true;
			//初期値：「準備金受渡明細表」が選択状態
			this.rdoUMeisaihyou.Checked = true;

			//申請区分毎の準備金申請確定済日を取得、設定する。
			SetZeinkaiKakuteibi();
		}
		else if (area == area.SEARCH)
		{
			//画面項目定義のNo.②（gbxResult）を非表示とする
			this.gbxResult.Visible = false;
		}
		else if (area == area.RESULT)
		{
			//なし
		}
		else if (area == area.DETAIL)
		{
			//なし
		}
		else if (area == area.BUTTON)
		{
			//ボタンエリアの初期設定

			//F7：活性
			base.F7Key_Visible = true;
			base.F7Key_Enabled = false;


			//検索押下前、非活性
			base.F10Key_Visible = true;
			base.F10Key_Enabled = false;
			base.F10Key_Text = "F10:確定";
		}
	}

	/// <summary>
	/// フォームクローズ時処理
	/// (呼出し元から呼び出される想定)
	/// </summary>
	/// <returns></returns>
	public bool iPt99Closing()
	{
		return true;
	}

	#endregion

	#region 画面に実装(Overrides)
	/// <summary>
	/// 初期処理
	/// </summary>
	protected override void OvPt99Init()
	{
		//初期化処理(PT99)
		base.OvPt99Init();
		//独自の初期処理
		//F8ボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnSet.Click += base.btnCom_Click;
		//条件クリアボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnClear.Click += base.btnCom_Click;
		//フォームのF8ボタンプロパティに、検索ボタンを設定
		base.baseBtnF8 = this.btnSet;
		//初期フォーカスを出発日FROMに設定する
		this.ActiveControl = this.dtmSyuptDayFromTo;
		this.dtmSyuptDayFromTo.FocusFromDate();
	}

	#region 検索処理用
	///' <summary>
	///' 検索前チェック処理
	///' </summary>
	///' <returns></returns>
	//Protected Overrides Function OvPt99SearchBefore() As Boolean
	//    Return MyBase.OvPt99SearchBefore()
	//End Function

	///' <summary>
	///' データ取得処理
	///' </summary>
	///' <returns></returns>
	//Protected Overrides Function OvPt99SearchGetData() As DataTable
	//    Return Nothing
	//End Function

	///' <summary>
	///' グリッドデータの取得とグリッド表示
	///' </summary>
	//Protected Overrides Sub OvPt99SearchAfter()
	//End Sub
	#endregion

	#region 更新処理用
	///' <summary>
	///' 更新前チェック処理
	///' </summary>
	///' <returns></returns>
	//Protected Overrides Function checkUpdateItems() As Boolean
	//    Return True
	//End Function

	///' <summary>
	///' 更新処理
	///' </summary>
	///' <returns></returns>
	//Protected Overrides Function updateTranData() As Integer
	//    Return 1
	//End Function
	#endregion

	#region 出力処理用
	/// <summary>
	/// 出力前(エラーチェック)処理
	/// </summary>
	/// <returns></returns>
	protected override bool OvPt99PrintBefore()
	{
		//エラークリア
		base.clearExistErrorProperty(gbxResult.Controls);

		DateTime dtToday = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		//＜出力前処理＞
		//準備金申請チェックリストが選択されている場合
		if (this.rdoShinseiCheckList.Visible && this.rdoShinseiCheckList.Checked)
		{
			if (System.Convert.ToInt32(this.txtYobiSinseiKingaku.Value) == 0 || (System.Convert.ToInt32(this.txtYobiSinseiKingaku.Value)) % 100000 != 0)
			{
				//「画面項目定義」のチェック要件、No.3を行う
				this.txtYobiSinseiKingaku.ExistError = true;
				createFactoryMsg.messageDisp("E04_006", E04_006_PARAM1, E04_006_PARAM2);
				return false;
			}
			//申請対象となるデータの申請金額より壱万円札、五千円札、壱千円札の枚数を算出する（「お札の計算式」シート参照）
			object lsNum = calcBills(System.Convert.ToInt32(txtYobiSinseiKingaku.Value));

			//DataAccessクラス生成
			S04_0503DA dataAccess = new S04_0503DA();
			S04_0503DA.S04_0503Param param = new S04_0503DA.S04_0503Param();
			//出発日FROM
			param.SyuptdayFrom = System.Convert.ToString(this.dtmSyuptDayFromTo.FromDateValueInt);
			//出発日TO
			param.SyuptdayTo = System.Convert.ToString(this.dtmSyuptDayFromTo.ToDateValueInt);
			//申請区分
			param.ShinseiKbn = getShinseiKbn();
			//コース毎申請金額
			param.CrsEveryShinseiKingaku = System.Convert.ToDecimal(this.txtSinseiKingakuTotal.Value);
			//予備申請壱万円札枚数
			param.YobiIchimanenNum = lsNum.Item(0);
			//予備申請五千円札枚数
			param.YobiGosenenNum = lsNum.Item(1);
			//予備申請壱千円札枚数
			param.YobiSenenNum = lsNum.Item(2);
			//予備申請金額
			param.YobiShinseiKingaku = System.Convert.ToInt32(this.txtYobiSinseiKingaku.Value);
			//システム登録ＰＧＭＩＤ
			param.SystemEntryPgmid = this.Name;
			//システム登録者コード
			param.SystemEntryPersonCd = UserInfoManagement.userId;
			//システム登録日
			param.SystemEntryDay = dtToday;
			//システム更新ＰＧＭＩＤ
			param.SystemUpdatePgmid = this.Name;
			//システム更新者コード
			param.SystemUpdatePersonCd = UserInfoManagement.userId;
			//システム更新日
			param.SystemUpdateDay = dtToday;
			//DA定義の更新処理を呼び出す
			dataAccess.updateTenjyoJunbikinYobiShinsei(param);
		}
		//準備金申請書が選択されている場合
		if (this.rdoShinseisho.Visible && this.rdoShinseisho.Checked)
		{
			if (System.Convert.ToInt32(this.txtYobiSinseiKingaku.Value) == 0 || (System.Convert.ToInt32(this.txtYobiSinseiKingaku.Value)) % 100000 != 0)
			{
				//「画面項目定義」のチェック要件、No.3を行う
				this.txtYobiSinseiKingaku.ExistError = true;
				createFactoryMsg.messageDisp("E04_006", E04_006_PARAM1, E04_006_PARAM2);
				return false;
			}
			//「画面項目定義」のチェック要件、No.4を行う
			if (!string.IsNullOrWhiteSpace(System.Convert.ToString(lblZeinkaiKakuteibi.Text)))
			{
				System.DateTime dtConfirm = DateTime.ParseExact(System.Convert.ToString(lblZeinkaiKakuteibi.Text), FORMAT_DATE, Globalization.CultureInfo.InvariantCulture);
				if (DateTime.Compare(System.Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText.Value), dtConfirm) > 0)
				{
					this.dtmSyuptDayFromTo.ExistErrorForFromDate = true;
					createFactoryMsg.messageDisp("E04_008", E04_008_PARAM1);
					return false;
				}
			}
		}
		//準備金受渡明細表が選択されている場合
		if (this.rdoUMeisaihyou.Visible && this.rdoUMeisaihyou.Checked)
		{
			//「画面項目定義」のチェック要件、No.4を行う
			if (!string.IsNullOrWhiteSpace(System.Convert.ToString(lblZeinkaiKakuteibi.Text)))
			{
				System.DateTime dtConfirm = DateTime.ParseExact(System.Convert.ToString(lblZeinkaiKakuteibi.Text), FORMAT_DATE, Globalization.CultureInfo.InvariantCulture);
				if (DateTime.Compare(System.Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText.Value), dtConfirm) > 0)
				{
					this.dtmSyuptDayFromTo.ExistErrorForFromDate = true;
					createFactoryMsg.messageDisp("E04_008", E04_008_PARAM1);
					return false;
				}
			}
		}
		return base.OvPt99PrintBefore();
	}

	///' <summary>
	///' DS出力時(パラメータ設定処理)処理
	///' </summary>
	///' <returns></returns>
	//Protected Overrides Function OvPt99PrintSetDSParameter() As Boolean
	//    Return MyBase.OvPt99PrintSetDSParameter()
	//End Function

	/// <summary>
	/// AR出力時(データ取得)処理
	/// </summary>
	/// <returns></returns>
	protected override DataTable OvPt99PrintARGetData()
	{
		DataTable resultDT = null;
		S04_0503DA dataAccess = new S04_0503DA();
		S04_0503DA.S04_0503Param param = new S04_0503DA.S04_0503Param();
		//出発日FROM
		param.SyuptdayFrom = System.Convert.ToString(this.dtmSyuptDayFromTo.FromDateValueInt);
		//出発日TO
		param.SyuptdayTo = System.Convert.ToString(this.dtmSyuptDayFromTo.ToDateValueInt);
		//申請区分
		param.ShinseiKbn = getShinseiKbn();
		//P04_0501_準備金申請書
		if (this.rdoShinseisho.Visible && this.rdoShinseisho.Checked)
		{
			resultDT = dataAccess.selectReserveInfo(param);
			if (resultDT.Rows.Count == 0)
			{
				resultDT = dataAccess.selectAppAmount(param);
			}
		}
		//P04_0503_準備金受渡明細表
		if (this.rdoUMeisaihyou.Visible && this.rdoUMeisaihyou.Checked)
		{
			resultDT = dataAccess.selectDeliverySchedule(param);
		}
		//P04_0502_準備金申請チェックリスト
		if (this.rdoShinseiCheckList.Visible && this.rdoShinseiCheckList.Checked)
		{
			resultDT = dataAccess.selectReserveInfo(param);
			if (resultDT.Rows.Count == 0)
			{
				resultDT = dataAccess.selectAppAmount(param);
			}
		}
		//P04_0504_準備金仮払い明細表
		if (this.rdoKMeisaihyou.Visible && this.rdoKMeisaihyou.Checked)
		{
			resultDT = dataAccess.selectAdditionalCourse(param);
			if (resultDT.Rows.Count == 0)
			{
				resultDT = dataAccess.selectReturnCourse(param);
			}
		}
		return resultDT;
	}

	/// <summary>
	/// レポート印刷する
	/// </summary>
	protected override void OvPt99Print()
	{
		S04_0503DA dataAccess = new S04_0503DA();
		S04_0503DA.S04_0503Param param = new S04_0503DA.S04_0503Param();
		//出発日FROM
		param.SyuptdayFrom = System.Convert.ToString(this.dtmSyuptDayFromTo.FromDateValueInt);
		//出発日TO
		param.SyuptdayTo = System.Convert.ToString(this.dtmSyuptDayFromTo.ToDateValueInt);
		//申請区分
		param.ShinseiKbn = getShinseiKbn();
		//P04_0501_準備金申請書
		if (this.rdoShinseisho.Visible && this.rdoShinseisho.Checked)
		{
			//P04_0504_準備金仮払い明細表
			P04_0501Output p04_0501Output = new P04_0501Output();
			P04_0501ParamData paramData = new P04_0501ParamData();
			paramData.MeisaiList = dataAccess.selectReserveInfo(param);
			paramData.AppAmountDt = dataAccess.selectAppAmount(param);
			paramData.S04_0503Param = param;
			p04_0501Output.OutputP04_0501(paramData);
		}
		//P04_0503_準備金受渡明細表
		if (this.rdoUMeisaihyou.Visible && this.rdoUMeisaihyou.Checked)
		{
			//P04_0503_準備金受渡明細表
			P04_0503OutPut p04_0503Output = new P04_0503OutPut();
			P04_0503ParamData paramData = new P04_0503ParamData();
			paramData._DatatableData = dataAccess.selectDeliverySchedule(param);
			paramData.ParamData = param;
			p04_0503Output.OutputP04_0503(paramData);
		}
		//P04_0502_準備金申請チェックリスト
		if (this.rdoShinseiCheckList.Visible && this.rdoShinseiCheckList.Checked)
		{
			//P04_0502_準備金申請チェックリスト
			P04_0502OutPut p04_0502Output = new P04_0502OutPut();
			P04_0502ParamData paramData = new P04_0502ParamData();
			paramData.MeisaiList = dataAccess.selectReserveInfo(param);
			paramData.AppAmountDt = dataAccess.selectAppAmount(param);
			paramData.S04_0503Param = param;
			p04_0502Output.OutputP04_0502(paramData);
		}
		//P04_0504_準備金仮払い明細表
		if (this.rdoKMeisaihyou.Visible && this.rdoKMeisaihyou.Checked)
		{
			//P04_0501_準備金申請書レポート
			P04_0504Output p04_0504Output = new P04_0504Output();
			P04_0504ParamData paramData = new P04_0504ParamData();
			paramData.Subreports1 = dataAccess.selectAdditionalCourse(param);
			paramData.Subreports2 = dataAccess.selectReturnCourse(param);
			paramData.S04_0503Param = param;
			p04_0504Output.OutputP04_0504(paramData);
		}
	}

	///' <summary>
	///' 出力後(データ加工等)処理
	///' </summary>
	//Protected Overrides Sub OvPt99printAfter()

	//End Sub
	#endregion

	#region PT99外ファンクションキー
	/// <summary>
	/// F8ボタン押下時の独自処理
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{
		//＜設定前処理＞
		//エラークリア
		base.clearExistErrorProperty(gbxSearch.Controls);
		//「画面項目定義」のチェック要件、No.1を行う
		if (!this.dtmSyuptDayFromTo.FromDateText.HasValue || !this.dtmSyuptDayFromTo.ToDateText.HasValue)
		{
			if (!this.dtmSyuptDayFromTo.ToDateText.HasValue)
			{
				this.dtmSyuptDayFromTo.ExistErrorForToDate = true;
				this.dtmSyuptDayFromTo.FocusToDate();
			}
			if (!this.dtmSyuptDayFromTo.FromDateText.HasValue)
			{
				this.dtmSyuptDayFromTo.ExistErrorForFromDate = true;
				this.dtmSyuptDayFromTo.FocusFromDate();
			}
			createFactoryMsg.messageDisp("E90_022");
			return;
		}

		//分類：準備金申請の場合、準備金申請
		if (this.rdoJunbikinSinsei.Checked)
		{
			this.gbxResult.Text = "準備金申請";

		}
		else //分類：明細表出力の場合、明細表出力
		{
			this.gbxResult.Text = "明細表出力";
		}

		//＜設定処理＞
		//分類が準備金申請の場合
		if (this.rdoJunbikinSinsei.Checked)
		{
			//検索結果エリアを表示する
			this.gbxResult.Visible = true;
			this.pnlShinsei.Visible = true;
			this.pnlMeisaihyou.Visible = false;
			this.lblSinseiKingakuTotal.Visible = true;
			this.lblYobiSinseiKingaku.Visible = true;
			this.lblOutputShinsei.Visible = true;
			this.txtSinseiKingakuTotal.Visible = true;
			this.txtYobiSinseiKingaku.Visible = true;
			this.txtSinseiKingakuTotal.Visible = true;
			S04_0503DA dataAccess = new S04_0503DA();
			S04_0503DA.S04_0503Param param = new S04_0503DA.S04_0503Param();
			//出発日FROM
			param.SyuptdayFrom = System.Convert.ToString(this.dtmSyuptDayFromTo.FromDateValueInt);
			//出発日TO
			param.SyuptdayTo = System.Convert.ToString(this.dtmSyuptDayFromTo.ToDateValueInt);
			//申請区分
			param.ShinseiKbn = getShinseiKbn();
			//申請金額合計の設定
			DataTable totalAmtDt = dataAccess.selectTotalAmount(param);
			if (!Information.IsDBNull(totalAmtDt.Rows(0).Item(0)))
			{
				this.txtSinseiKingakuTotal.Value = System.Convert.ToDecimal(totalAmtDt.Rows(0).Item(0));
			}
			else
			{
				this.txtSinseiKingakuTotal.Value = 0;
			}
			//予備申請金額の設定
			DataTable amtMoneyDt = dataAccess.selectAmountMoney(param);
			if (amtMoneyDt.Rows().Count > 0)
			{
				this.txtYobiSinseiKingaku.Value = System.Convert.ToInt32(amtMoneyDt.Rows(0).Item(0));
			}
			else
			{
				this.txtYobiSinseiKingaku.Value = 0;
			}

			//申請金額合計が最大桁数9桁を超える場合
			if (this.txtSinseiKingakuTotal.Value?.ToString().Length > TOTAL_FIELD_MAX_LENGTH)
			{
				createFactoryMsg.messageDisp("E90_075", "申請金額合計", string.Format("最大桁数（{0}桁）", TOTAL_FIELD_MAX_LENGTH));
				return;
			}

			//「確定」ボタンは添乗準備金確定日＜出発日ＦＲＯＭの場合、活性とする
			if (!string.IsNullOrWhiteSpace(System.Convert.ToString(lblZeinkaiKakuteibi.Text)))
			{
				System.DateTime dtConfirm = DateTime.ParseExact(System.Convert.ToString(lblZeinkaiKakuteibi.Text), FORMAT_DATE, Globalization.CultureInfo.InvariantCulture);
				if (DateTime.Compare(System.Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText.Value), dtConfirm) > 0)
				{
					base.F10Key_Enabled = true;
				}
			}
			else
			{
				base.F10Key_Enabled = true;
			}

			//分類が明細表出力の場合
		}
		else if (this.rdoMeisaiShuturyoku.Checked)
		{
			//検索結果エリアを表示する
			this.gbxResult.Visible = true;
			this.pnlShinsei.Visible = false;
			this.pnlMeisaihyou.Visible = true;
			this.lblSinseiKingakuTotal.Visible = false;
			this.lblYobiSinseiKingaku.Visible = false;
			this.lblOutputShinsei.Visible = false;
			this.txtSinseiKingakuTotal.Visible = false;
			this.txtYobiSinseiKingaku.Visible = false;
		}
		//「印刷」ボタンを活性とする
		base.F7Key_Enabled = true;

		//＜設定後処理＞
		this.dtmSyuptDayFromTo.Enabled = false;
		this.pnlGroup.Enabled = false;
		this.pnlSinseiKbn.Enabled = false;
		this.btnSet.Enabled = false;
	}

	/// <summary>
	/// F10ボタン押下時の独自処理
	/// </summary>
	protected override void btnF10_ClickOrgProc()
	{
		//エラークリア
		base.clearExistErrorProperty(gbxResult.Controls);
		DateTime dtToday = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		//＜確定前処理＞
		if (!string.IsNullOrWhiteSpace(System.Convert.ToString(lblZeinkaiKakuteibi.Text)))
		{
			System.DateTime dtNextConfirm = System.Convert.ToDateTime(DateTime.ParseExact(System.Convert.ToString(lblZeinkaiKakuteibi.Text), FORMAT_DATE, Globalization.CultureInfo.InvariantCulture).AddDays(1));
			//「画面項目定義」のチェック要件、No.2を行う
			if (DateTime.Compare(System.Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText.Value), dtNextConfirm) != 0)
			{
				this.dtmSyuptDayFromTo.ExistErrorForFromDate = true;
				createFactoryMsg.messageDisp("E04_009");
				return;
			}


		}
		if (System.Convert.ToInt32(this.txtYobiSinseiKingaku.Value) == 0 || (System.Convert.ToInt32(this.txtYobiSinseiKingaku.Value)) % 100000 != 0)
		{
			//「画面項目定義」のチェック要件、No.3を行う
			this.txtYobiSinseiKingaku.ExistError = true;
			createFactoryMsg.messageDisp("E04_006", E04_006_PARAM1, E04_006_PARAM2);
			return;
		}
		//＜確定処理＞
		//確定対象となるデータの申請金額より壱万円札、五千円札、壱千円札の枚数を算出する（「お札の計算式」シート参照）
		object lsNum = calcBills(System.Convert.ToInt32(txtYobiSinseiKingaku.Value));
		//DataAccessクラス生成
		S04_0503DA dataAccess = new S04_0503DA();
		//確定対象のデータをパラメータに設定する
		S04_0503DA.S04_0503Param param = new S04_0503DA.S04_0503Param();
		//出発日FROM
		param.SyuptdayFrom = System.Convert.ToString(this.dtmSyuptDayFromTo.FromDateValueInt);
		//出発日TO
		param.SyuptdayTo = System.Convert.ToString(this.dtmSyuptDayFromTo.ToDateValueInt);
		//申請区分
		param.ShinseiKbn = getShinseiKbn();
		//コース毎申請金額
		param.CrsEveryShinseiKingaku = System.Convert.ToDecimal(this.txtSinseiKingakuTotal.Value);
		//予備申請壱万円札枚数
		param.YobiIchimanenNum = lsNum.Item(0);
		//予備申請五千円札枚数
		param.YobiGosenenNum = lsNum.Item(1);
		//予備申請壱千円札枚数
		param.YobiSenenNum = lsNum.Item(2);
		//予備申請金額
		param.YobiShinseiKingaku = System.Convert.ToInt32(this.txtYobiSinseiKingaku.Value);
		//システム登録ＰＧＭＩＤ
		param.SystemEntryPgmid = this.Name;
		//システム登録者コード
		param.SystemEntryPersonCd = UserInfoManagement.userId;
		//システム登録日
		param.SystemEntryDay = dtToday;
		//システム更新ＰＧＭＩＤ
		param.SystemUpdatePgmid = this.Name;
		//システム更新者コード
		param.SystemUpdatePersonCd = UserInfoManagement.userId;
		//システム更新日
		param.SystemUpdateDay = dtToday;
		//DA定義の更新処理を呼び出す
		dataAccess.updateTables(param);
		//＜確定後処理＞
		//DA定義の準備金申請確定済日取得を申請区分毎に呼び出し、申請区分毎の準備金申請確定済日を取得、設定する
		SetZeinkaiKakuteibi();
	}

	/// <summary>
	/// 条件クリアボタン押下イベント
	/// </summary>
	protected override void btnCLEAR_ClickOrgProc()
	{
		base.btnCLEAR_ClickOrgProc();
		//エラークリア
		base.clearExistErrorProperty(gbxResult.Controls);
		this.dtmSyuptDayFromTo.Enabled = true;
		this.pnlGroup.Enabled = true;
		this.pnlSinseiKbn.Enabled = true;
		this.btnSet.Enabled = true;
	}
	#endregion
	#endregion

	#region Privateメソッド(画面独自)

	#endregion

	#region Privateメソッド(イベント)
	/// <summary>
	/// お札枚数の設定
	/// </summary>
	/// <param name="amount"></param>
	/// <returns></returns>
	private List[] calcBills(int amount)
	{
		int _Business = 0;
		int _PieceNum = 0;
		int _IchiManNum = 0;
		int _GosenNum = 0;
		int _SenNum = 0;
		//商 = 金額 \ 10000
		_Business = System.Convert.ToInt32((double)amount / 100000);
		//札枚数ワーク = 0
		_PieceNum = 0;
		//札枚数ワーク = 商 + 1
		_PieceNum = _Business + 1;
		//壱千円札枚数 = 札枚数ワーク × 5
		_SenNum = _PieceNum * 5;
		//五千円札枚数 = 商 ＋ 9
		_GosenNum = _Business + 9;
		//札枚数ワーク = 0
		_PieceNum = 0;
		//商 = 商 × 10
		_Business = _Business * 10;
		//札枚数ワーク = 5 × 五千円札枚数
		_PieceNum = 5 * _GosenNum;
		//札枚数ワーク = 札枚数ワーク ＋ 壱千円札枚数
		_PieceNum = _PieceNum + _SenNum;
		//札枚数ワーク = 札枚数ワーク \ 10
		_PieceNum = System.Convert.ToInt32((double)_PieceNum / 10);
		//札枚数ワーク = 札枚数ワーク - 商
		_PieceNum = _PieceNum - _Business;
		//札枚数ワーク = 札枚数ワーク × -1
		_PieceNum = _PieceNum * -1;
		//壱万円札枚数 = 札枚数ワーク
		_IchiManNum = _PieceNum;
		List lsNum = new List(Of int);
		//壱万円札枚数
		lsNum.Add(_IchiManNum);
		//五千円札枚数
		lsNum.Add(_GosenNum);
		//壱千円札枚数
		lsNum.Add(_SenNum);
		return lsNum;
	}

	private string getShinseiKbn()
	{
		if (rdoTeikiKanko.Checked)
		{
			return System.Convert.ToString(FixedCd.ToriatsukaiBusyo.teiki);
		}
		else if (rdoKikakuRyoko.Checked)
		{
			return System.Convert.ToString(FixedCd.ToriatsukaiBusyo.kikaku);
		}
		else if (rdoKokusaiJigyo.Checked)
		{
			return System.Convert.ToString(FixedCd.ToriatsukaiBusyo.world);
		}
		else
		{
			return string.Empty;
		}
	}
	/// <summary>
	/// 申請区分ラジオボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void rdoKokusaiJigyo_CheckedChanged(object sender, EventArgs e)
	{
		SetZeinkaiKakuteibi();
	}
	/// <summary>
	/// 申請区分ラジオボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void rdoKikakuRyoko_CheckedChanged(object sender, EventArgs e)
	{
		SetZeinkaiKakuteibi();
	}
	/// <summary>
	/// 申請区分ラジオボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void rdoTeikiKanko_CheckedChanged(object sender, EventArgs e)
	{
		SetZeinkaiKakuteibi();
	}

	/// <summary>
	/// 申請確定日を取得、設定する。
	/// </summary>
	private void SetZeinkaiKakuteibi()
	{
		//DataAccessクラス生成
		S04_0503DA dataAccess = new S04_0503DA();
		S04_0503DA.S04_0503Param param = new S04_0503DA.S04_0503Param();
		param.ShinseiKbn = getShinseiKbn();
		//DA定義の準備金申請確定済日取得を申請区分毎に呼び出し
		DataTable confirmDateDt = dataAccess.selectConfirmationDate(param);
		//申請区分毎の準備金申請確定済日を取得、設定する。
		if (!Information.IsDBNull(confirmDateDt.Rows(0).Item(0)))
		{
			object confirmDate = CommonDateUtil.convertDateFormat(System.Convert.ToString(confirmDateDt.Rows(0).Item(0)), FORMAT_DATE);
			this.lblZeinkaiKakuteibi.Text = confirmDate;
		}
		else
		{
			this.lblZeinkaiKakuteibi.Text = string.Empty;
		}
	}
	#endregion

}