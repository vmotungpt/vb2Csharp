using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.ClientCommon.FlexGridEx;
using Hatobus.ReservationManagementSystem.Master;


/// <summary>
/// 降車ヶ所人員連絡
/// </summary>
public class S04_0401 : PT99, iPT99
{

	#region フィールド
	private int selectedRowIndex = 1;

	private int flag = 0;

	private DataRow currentRow = null;

	private DataTable dataTableGrdSiireInfo = null;

	private DataTable dataTableGrdSiireInfoOption = null;

	private DataTable dataTablegrdOption = null;

	#endregion

	#region パラメータのS04_0401
	public S04_0401ParamData ParamData
	{
#endregion

		#region 定数

		#endregion

		#region 列挙体
		/// <summary>
		/// Grid予約情報
		/// </summary>
	private enum YoyakuInfoList
	{
		[Value("")]
		TITLE,
		[Value("")]
		YOYAKU_NUM

	}



	/// <summary>
	/// Grid人員内訳
	/// </summary>
	private enum JininUtiList
	{
		[Value("人員区分")]
		JININ_KBN,
		[Value("人数")]
		NINZU
	}

	/// <summary>
	/// Grid予約内訳
	/// </summary>
	private enum YoyakuUtiList
	{
		[Value("予約人数")]
		YOYAKU_NINZU,
		[Value("予約件数")]
		YOYAKU_KENSU,
		[Value("計")]
		YOYAKU_KEI
	}

	/// <summary>
	/// Grid仕入先情報
	/// </summary>
	private enum SiireInfoList
	{
		[Value("選択")]
		@SELECT,
		[Value("日次")]
		DAILY,
		[Value("仕入先種別コード")]
		SIIRE_SAKI_KIND_CD,
		[Value("仕入先種別")]
		SIIRE_SAKI_KIND_NAME,
		[Value("仕入先コード")]
		SIIRE_SAKI_CD,
		[Value("仕入先枝番")]
		SIIRE_SAKI_EDABAN,
		[Value("仕入先")]
		SIIRESAKI_CD,
		[Value("仕入先名称")]
		SIIRE_SAKI_NAME,
		[Value("TEL")]
		TEL_STR,
		[Value("FAX")]
		FAX_STR,
		[Value("FAX2")]
		FAX2_STR,
		[Value("メールアドレス")]
		MAIL,
		[Value("通知方法")]
		NOTIFICATION_HOHO,
		[Value("連絡状況")]
		CONTACT_STATU,
		[Value("画面出力有無編集許可フラグ")]
		ALLOW_EDIT,
		[Value("SEND_YMDT")]
		SEND_YMDT,
		[Value("NOTEWORTHY")]
		NOTEWORTHY,
		[Value("REMARKS")]
		REMARKS,
		[Value("INFANT_INFO")]
		INFANT_INFO,
		[Value("SYSTEM_ENTRY_PGMID")]
		SYSTEM_ENTRY_PGMID,
		[Value("SYSTEM_ENTRY_PERSON_CD")]
		SYSTEM_ENTRY_PERSON_CD,
		[Value("SYSTEM_ENTRY_DAY")]
		SYSTEM_ENTRY_DAY
	}

	/// <summary>
	/// Grid仕入先情報
	/// </summary>
	private enum OptionList
	{
		[Value("オプション名")]
		OPTIONAL_NAME,
		[Value("予約人数")]
		OPTION_NINZU,
		[Value("予約件数")]
		OPTION_KENSU,
		[Value("計")]
		OPTION_KEI
	}

	/// <summary>
	/// データベース内の名前
	/// </summary>
	private enum ListItemYoyaku
	{
		[Value("YOYAKU_KBN")]
		YOYAKU_KBN,
		[Value("YOYAKU_NO")]
		YOYAKU_NO,
		[Value("CHARGE_APPLICATION_NINZU")]
		CHARGE_APPLICATION_NINZU,
		[Value("INFANT_NINZU")]
		INFANT_NINZU,
		[Value("CHARGE_KBN_JININ_CD")]
		CHARGE_KBN_JININ_CD
	}

	/// <summary>
	/// 料金区分（人員）コード
	/// </summary>
	/// <remarks></remarks>
	public enum ValueChargeKbnJininCd
	{
		[Value("大人(男)")]
		adultMan,
		[Value("大人(女)")]
		adultWoman,
		[Value("中人(男)")]
		juniorMan,
		[Value("小人(男)")]
		childMan,
		[Value("幼児")]
		toddler,
		[Value("シルバー")]
		elder,
		[Value("中人(女)")]
		juniorWoman,
		[Value("小人(女)")]
		childWoman,
		[Value("大人")]
		adult,
		[Value("中人")]
		junior,
		[Value("小人")]
		child
	}

	//01：メール　02：FAX　03：電話
	private const string Mail = "01";
	private const string Fax = "02";
	private const string Tel = "03";

	//未連絡と表示
	private const string ContactStatuTex = "未連絡";
	//1行目：　「予約件数」
	private const string YoyakuInfoLine1 = "予約件数";
	//2行目：　「予約人数」
	private const string YoyakuInfoLine2 = "予約人数";
	//3行目：　「インファント」
	private const string YoyakuInfoLine3 = "インファント";
	//変更が反映されていないデータ
	private const string MessageQ03_003 = "変更が反映されていないデータ";
	//通知対象
	private const string MessageE90_024 = "通知対象";


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
		//PT99プロパティ

		//【ボタン用設定】
		//CSV出力ボタン(F4)の表示可否
		base.PtIsCsvOutFlg = false;
		//プレビューボタン(F6)の表示可否
		base.PtIsPrevFlg = false;
		//印刷ボタン(F7)の表示可否
		base.PtIsPrintFlg = false;
		//検索ボタン(F8)の表示可否　※フッターのF8のため検索エリア時はFalseを設定する
		base.PtIsSearchFlg = false;
		//登録ボタン(F10)の表示可否
		base.PtIsRegFlg = true;
		//更新ボタン(F11)の表示可否
		base.PtIsUpdFlg = false;

		//【コントロール系】
		//検索エリアのグループボックスを設定する
		base.PtSearchControl = null;
		//検索結果エリアのグループボックスを設定する
		base.PtResultControl = null;
		//詳細エリアのグループボックスを設定する
		base.PtDetailControl = pnlBaseFill;
		//検索結果を表示するためのGrid
		base.PtResultGrid = null;
		//検索結果を表示するためのGrid
		base.PtDisplayBtn = null;
		//検索結果を表示するためのGrid
		base.PtResultLblCnt = null;

		//【データ系】
		//結果グリッドの最大表示件数
		//MyBase.PtMaxCount =
		//結果グリッドの最大表示件数
		//MyBase.PtResultDT = -
		//※ReadOnly 結果グリッドの選択行データ
		//MyBase.PtSelectRow = -

		//【その他】
		//実装画面(インターフェースを実装フォーム)
		base.PtMyForm = this;
		//変更チェックを行うDataTableのカラム名(ID)を設定
		base.PtDiffChkColName = new List(Of string);

		//【帳票用】
		//AR/DS選択用プロパティ
		base.PtPrintType = null;
		//呼び出しDaTaStudioID
		base.PtDsPrintId = null;

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
		//検索条件エリアの初期設定
		if (area == area.SEARCH)
		{
			//なし

			//・フォームの初期設定
		}
		else if (area == area.FORM)
		{
			//「画面項目定義」のNo.④の項目を対象に、初期値記載された内容に準じて、項目の初期化を行う
			base.F10Key_Text = "F10:送信";

			//検索結果エリアの初期設定
		}
		else if (area == area.RESULT)
		{
			//「画面項目定義」のNo.①の項目を対象に、初期値記載された内容に準じて、項目の初期化を行う
			//出発日
			this.txtSyupt_day.Text = CommonDateUtil.convertDateFormat(ParamData.SyuptDay.ToString(), CommonDateUtil.NormalTime_JP);
			//コースコード
			this.txtCrsCd.Text = ParamData.CrsCd;
			//コース名
			this.txtCrsName.Text = ParamData.CrsName;
			//号車
			this.txtGousya.Text = ParamData.Gousya.ToString();

			//Grid詳細①参照
			base.CmnInitGrid(grdYoyakuInfo);
			//Grid詳細②参照
			base.CmnInitGrid(grdJininUti);
			//Grid詳細③参照
			base.CmnInitGrid(grdYoyakuUti);
			//Grid詳細④参照の降車ヶ所
			base.CmnInitGrid(grdSiireInfo);
			//Grid詳細④参照のオプション
			base.CmnInitGrid(grdSiireInfoOption);
			//Grid詳細⑤参照
			base.CmnInitGrid(grdOption);

			InitializeResultGrid();

			//検索処理を呼び出す
			//GetAllDataDANo1ToNo4()

			//グリッドデータの取得とグリッド表示
			btnF8_ClickOrgProc();

		}
		else if (area == area.DETAIL)
		{
			//TODO:画面項目定義(詳細)＜詳細エリアの初期設定＞
		}
		else if (area == area.BUTTON)
		{
			//・ボタンエリアの初期設定
			//F10:送信
			//MyBase.F10Key_Visible = True
			base.F10Key_Enabled = true;
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
		//初期フォーカスは以下のように設定を行う
		//降車ヶ所タブを選択
		this.tabControl.SelectedTab = Tab_info;
		if (this.grdSiireInfo.Rows.Count > 1)
		{
			//仕入先情報グリッド1行目にフォーカスをあて
			this.grdSiireInfo.Rows(selectedRowIndex).Selected = true;
			currentRow = ((DataRowView)(this.grdSiireInfo.Rows(selectedRowIndex).DataSource)).Row;
			//1行目のインファント・特記事項・備考・仕入先コード・仕入先枝番・日次を「画面項目定義」のNo.③へ設定し、表示する
			this.txtNoteSp.Text = currentRow.Item(SiireInfoList.NOTEWORTHY.ToString()).ToString();
			this.txtRemarks.Text = currentRow.Item(SiireInfoList.REMARKS.ToString()).ToString();
			this.txtInfant.Text = currentRow.Item(SiireInfoList.INFANT_INFO.ToString()).ToString();
			this.txtSiireSakiCd.Text = currentRow.Item(SiireInfoList.SIIRE_SAKI_CD.ToString()).ToString();
			this.txtSiireSakiEdaban.Text = currentRow.Item(SiireInfoList.SIIRE_SAKI_EDABAN.ToString()).ToString();
			this.txtDaily.Text = currentRow.Item(SiireInfoList.DAILY.ToString()).ToString();
		}
	}

	#region 検索処理用
	///' <summary>
	///' 検索前チェック処理
	///' </summary>
	///' <returns></returns>
	//Protected Overrides Function OvPt99SearchBefore() As Boolean
	//    'TODO:画面項目定義(詳細)＜検索前処理＞

	//    Return MyBase.OvPt99SearchBefore()
	//End Function

	protected override DataTable OvPt99SearchGetData()
	{
		GetAllDataDANo1ToNo4();
		//検索処理を呼び出す
		object dataGrdSiireInfo = (DataTable)this.grdSiireInfo.DataSource;
		object dataGrdSiireInfoOption = (DataTable)this.grdSiireInfoOption.DataSource;
		if (dataGrdSiireInfo.Rows.Count > 0)
		{
			return dataGrdSiireInfo;
		}
		else if (dataGrdSiireInfoOption.Rows.Count > 0)
		{
			return dataGrdSiireInfoOption;
		}
	}

	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void OvPt99SearchAfter()
	{
		if (this.tabControl.SelectedTab.TabIndex == this.Tab_info.TabIndex)
		{
			if (this.grdSiireInfo.DataSource IsNot null && this.grdSiireInfo.Rows.Count > 1)
				{
				currentRow = ((DataRowView)(this.grdSiireInfo.Rows(selectedRowIndex).DataSource)).Row;
				for (int row = this.grdSiireInfo.Rows.Fixed; row <= this.grdSiireInfo.Rows.Count - 1; row++)
				{
					//・画面出力有無編集許可フラグが"0"(False)の場合、非活性
					if (System.Convert.ToBoolean(this.grdSiireInfo.GetData(row, SiireInfoList.ALLOW_EDIT.ToString())) == false)
					{
						this.grdSiireInfo.SetCellCheck(row, this.grdSiireInfo.Cols(SiireInfoList.SELECT.ToString()).Index, CheckEnum.Unchecked);
						this.grdSiireInfo.setColorNonEdit(row, this.grdSiireInfo.Cols(SiireInfoList.SELECT.ToString()).Index, Color.Gray);
					}
					//・最終送信日時取得できた場合、"0"(False)上記以外の場合、"1"（True）
					if (!string.IsNullOrEmpty(this.grdSiireInfo.Rows(row).Item(SiireInfoList.SEND_YMDT.ToString()).ToString()))
					{
						this.grdSiireInfo.SetCellCheck(row, this.grdSiireInfo.Cols(SiireInfoList.SELECT.ToString()).Index, CheckEnum.Checked);
					}
					else
					{
						//最終送信日時がNullの場合”未連絡”と表示
						this.grdSiireInfo.SetData(row, SiireInfoList.CONTACT_STATU.ToString(), ContactStatuTex.ToString());
						this.grdSiireInfo.SetCellCheck(row, this.grdSiireInfo.Cols(SiireInfoList.SELECT.ToString()).Index, CheckEnum.Unchecked);
					}
					//最終送信日時がNullの場合”未連絡”と表示
					if (this.grdSiireInfo.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString()) IsNot DBNull.Value)
						{
					//Mail = 01
					if (string.Compare(System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Mail.ToString()) == 0)
					{
						this.grdSiireInfo.SetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString(), NotificationHohoKoshaRenraku.Mail.GetHashCode);
						//Fax = 02

					}
					else if (string.Compare(System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Fax.ToString()) == 0)
					{
						try
						{
							this.grdSiireInfo.SetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString(), NotificationHohoKoshaRenraku.Fax.GetHashCode);
						}
						catch (Exception)
						{

						}
						//TEL = 03
					}
					else if (string.Compare(System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Tel.ToString()) == 0)
					{
						this.grdSiireInfo.SetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString(), NotificationHohoKoshaRenraku.Tel.GetHashCode);
					}
				}
			}
		}
	}
			else if (this.tabControl.SelectedTab.TabIndex == this.Tab_infoOption.TabIndex)
			{
		if (this.grdSiireInfoOption.DataSource IsNot null && this.grdSiireInfoOption.Rows.Count > 1)
				{
			currentRow = ((DataRowView)(this.grdSiireInfoOption.Rows(selectedRowIndex).DataSource)).Row;
			for (int row = this.grdSiireInfoOption.Rows.Fixed; row <= this.grdSiireInfoOption.Rows.Count - 1; row++)
			{
				//・画面出力有無編集許可フラグが"0"(False)の場合、非活性
				if (System.Convert.ToBoolean(this.grdSiireInfoOption.GetData(row, SiireInfoList.ALLOW_EDIT.ToString())) == false)
				{
					this.grdSiireInfoOption.SetCellCheck(row, this.grdSiireInfoOption.Cols(SiireInfoList.SELECT.ToString()).Index, CheckEnum.Unchecked);
					this.grdSiireInfoOption.setColorNonEdit(row, this.grdSiireInfoOption.Cols(SiireInfoList.SELECT.ToString()).Index, Color.Gray);
				}
				//・最終送信日時取得できた場合、"0"(False)上記以外の場合、"1"（True）
				if (!string.IsNullOrEmpty(this.grdSiireInfoOption.Rows(row).Item(SiireInfoList.SEND_YMDT.ToString()).ToString()))
				{
					this.grdSiireInfoOption.SetCellCheck(row, this.grdSiireInfoOption.Cols(SiireInfoList.SELECT.ToString()).Index, CheckEnum.Checked);
				}
				else
				{
					//最終送信日時がNullの場合”未連絡”と表示
					this.grdSiireInfoOption.SetData(row, SiireInfoList.CONTACT_STATU.ToString(), ContactStatuTex.ToString());
					this.grdSiireInfoOption.SetCellCheck(row, this.grdSiireInfoOption.Cols(SiireInfoList.SELECT.ToString()).Index, CheckEnum.Unchecked);
				}
				//最終送信日時がNullの場合”未連絡”と表示
				if (this.grdSiireInfoOption.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString()) IsNot DBNull.Value)
						{
				//Mail = 01
				if (string.Compare(System.Convert.ToString(this.grdSiireInfoOption.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Mail.ToString()) == 0)
				{
					this.grdSiireInfoOption.SetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString(), NotificationHohoKoshaRenraku.Mail.GetHashCode);
					//Fax = 02

				}
				else if (string.Compare(System.Convert.ToString(this.grdSiireInfoOption.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Fax.ToString()) == 0)
				{
					this.grdSiireInfoOption.SetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString(), NotificationHohoKoshaRenraku.Fax.GetHashCode);

					//TEL = 03
				}
				else if (string.Compare(System.Convert.ToString(this.grdSiireInfoOption.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Tel.ToString()) == 0)
				{
					this.grdSiireInfoOption.SetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString(), NotificationHohoKoshaRenraku.Tel.GetHashCode);
				}
			}
		}
	}
	}
}
#endregion

#region 更新処理用
//' <summary>
//' 更新前チェック処理
//' </summary>
//' <returns></returns>
protected override bool checkUpdateItems()
{
	// なし
	return true;
}

/// <summary>
/// 更新処理
/// </summary>
/// <returns></returns>
protected override int updateTranData()
{
	//降車ヶ所人員連絡DAの更新処理を呼び出す
	S04_0401_DA dataAccess = new S04_0401_DA();
	S04_0401_DA.TKousyaJininControlParam param = new S04_0401_DA.TKousyaJininControlParam();
	//パラメータはIF(OUT)シートNo2参照
	//出発日
	param.SyuptDay = ParamData.SyuptDay;
	//コースコード
	param.CrsCd = ParamData.CrsCd;
	//号車
	param.Gousya = ParamData.Gousya;
	//日次
	param.Daily = System.Convert.ToInt32(currentRow.Item(SiireInfoList.DAILY.ToString()).ToString());
	//仕入先コード
	param.SiireSakiCd = currentRow.Item(SiireInfoList.SIIRE_SAKI_CD.ToString()).ToString();
	//仕入先枝番
	param.SiireSakiNo = currentRow.Item(SiireInfoList.SIIRE_SAKI_EDABAN.ToString()).ToString();
	//仕入先種別
	param.SiireSakiKindCd = currentRow.Item(SiireInfoList.SIIRE_SAKI_KIND_CD.ToString()).ToString();
	//仕入先名
	param.SiireSakiName = currentRow.Item(SiireInfoList.SIIRE_SAKI_NAME.ToString()).ToString();
	//TEL1
	param.Tel1 = currentRow.Item(SiireInfoList.TEL_STR.ToString()).ToString().Replace(CommonCharType.hyphen.ToString(), string.Empty);
	string[] elementsTel1 = currentRow.Item(SiireInfoList.TEL_STR.ToString()).ToString().Split(new char[] { Convert.ToChar(CommonCharType.hyphen.ToString()) }, StringSplitOptions.RemoveEmptyEntries);
	if (elementsTel1.Length == 3)
	{
		//TEL1_1
		param.Tel11 = elementsTel1[0].Replace("\r\n", string.Empty);
		//TEL1_2
		param.Tel12 = elementsTel1[1].Replace("\r\n", string.Empty);
		//TEL1_3
		param.Tel13 = elementsTel1[2].Replace("\r\n", string.Empty);
	}
	else
	{
		//TEL1_1
		param.Tel11 = string.Empty;
		//TEL1_2
		param.Tel12 = string.Empty;
		//TEL1_3
		param.Tel13 = string.Empty;
	}

	//FAX1
	param.Fax1 = currentRow.Item(SiireInfoList.FAX_STR.ToString()).ToString().Replace(CommonCharType.hyphen.ToString(), string.Empty);
	string[] elementsFax1 = currentRow.Item(SiireInfoList.FAX_STR.ToString()).ToString().Split(new char[] { Convert.ToChar(CommonCharType.hyphen.ToString()) }, StringSplitOptions.RemoveEmptyEntries);
	if (elementsFax1.Length == 3)
	{
		//FAX1_1
		param.Fax11 = elementsFax1[0].Replace("\r\n", string.Empty);
		//FAX1_2
		param.Fax12 = elementsFax1[1].Replace("\r\n", string.Empty);
		//FAX1_3
		param.Fax13 = elementsFax1[2].Replace("\r\n", string.Empty);
	}
	else
	{
		//FAX1_1
		param.Fax11 = string.Empty;
		//FAX1_2
		param.Fax12 = string.Empty;
		//FAX1_3
		param.Fax13 = string.Empty;
	}
	//FAX2
	param.Fax2 = currentRow.Item(SiireInfoList.FAX2_STR.ToString()).ToString().Replace(CommonCharType.hyphen.ToString(), string.Empty);
	string[] elementsFax2 = currentRow.Item(SiireInfoList.FAX2_STR.ToString()).ToString().Split(new char[] { Convert.ToChar(CommonCharType.hyphen.ToString()) }, StringSplitOptions.RemoveEmptyEntries);
	if (elementsFax2.Length == 3)
	{
		//FAX2_1
		param.Fax21 = elementsFax2[0].Replace("\r\n", string.Empty);
		//FAX2_2
		param.Fax22 = elementsFax2[1].Replace("\r\n", string.Empty);
		//FAX2_3
		param.Fax23 = elementsFax2[2].Replace("\r\n", string.Empty);
	}
	//メールアドレス
	param.Mail = currentRow.Item(SiireInfoList.MAIL.ToString()).ToString();

	//・通知方法が「電話」　かつ　連絡済チェックボックスONの場合
	if (System.Convert.ToInt32(NotificationHohoKoshaRenraku.Tel) == System.Convert.ToInt32(currentRow.Item(SiireInfoList.NOTIFICATION_HOHO.ToString()).ToString()) && this.chkContacted.Checked == true)
	{
		//通知方法
		param.SendKind = Tel.ToString();
		//'最終送信日時
		param.SendYmdt = CommonDateUtil.getSystemTime();
	}
	else
	{
		//・上記以外の場合
		//通知方法
		param.SendKind = string.Empty;
		//最終送信日時
		param.SendYmdt = null;
	}

	//特記事項
	param.Noteworthy = this.txtNoteSp.Text;
	//インファント情報
	param.InfantInfo = this.txtInfant.Text;
	//備考
	param.Remarks = this.txtRemarks.Text;
	//システム登録ＰＧＭＩＤ
	if (currentRow.Item(SiireInfoList.SYSTEM_ENTRY_PGMID.ToString()) IsNot null && !string.IsNullOrEmpty(currentRow.Item(SiireInfoList.SYSTEM_ENTRY_PGMID.ToString()).ToString()))
			{
		param.SystemEntryPgmid = if (!Information.IsDBNull(currentRow.Item(SiireInfoList.SYSTEM_ENTRY_PGMID.ToString())), System.Convert.ToString(currentRow.Item(SiireInfoList.SYSTEM_ENTRY_PGMID.ToString())), this.Name);
	}
			else
	{
		param.SystemEntryPgmid = this.Name;
	}
	//システム登録者コード
	if (currentRow.Item(SiireInfoList.SYSTEM_ENTRY_PERSON_CD.ToString()) IsNot null && !string.IsNullOrEmpty(currentRow.Item(SiireInfoList.SYSTEM_ENTRY_PERSON_CD.ToString()).ToString()))
			{
		param.SystemEntryPersonCd = if (!Information.IsDBNull(currentRow.Item(SiireInfoList.SYSTEM_ENTRY_PERSON_CD.ToString())), System.Convert.ToString(currentRow.Item(SiireInfoList.SYSTEM_ENTRY_PERSON_CD.ToString())), UserInfoManagement.userId);
	}
			else
	{
		param.SystemEntryPersonCd = UserInfoManagement.userId;
	}
	//システム登録日
	if (currentRow.Item(SiireInfoList.SYSTEM_ENTRY_DAY.ToString()) IsNot null && !string.IsNullOrEmpty(currentRow.Item(SiireInfoList.SYSTEM_ENTRY_DAY.ToString()).ToString()))
			{
		param.SystemEntryDay = if (!Information.IsDBNull(currentRow.Item(SiireInfoList.SYSTEM_ENTRY_DAY.ToString())), System.Convert.ToDateTime(currentRow.Item(SiireInfoList.SYSTEM_ENTRY_DAY.ToString())), CommonDateUtil.getSystemTime());
	}
			else
	{
		param.SystemEntryDay = CommonDateUtil.getSystemTime();
	}
	//システム更新ＰＧＭＩＤ
	param.SystemUpdatePgmid = this.Name;
	//システム更新者コード
	param.SystemUpdatePersonCd = UserInfoManagement.userId;
	//システム更新日
	param.SystemUpdateDay = CommonDateUtil.getSystemTime();

	//DA定義No5参照
	object returnValue = dataAccess.updateTable(param);
	if (returnValue > 0)
	{
		currentRow.Item[SiireInfoList.NOTEWORTHY.ToString()] = this.txtNoteSp.Text;
		currentRow.Item[SiireInfoList.REMARKS.ToString()] = this.txtRemarks.Text;
		currentRow.Item[SiireInfoList.INFANT_INFO.ToString()] = this.txtInfant.Text;
	}
	return returnValue;
}
#endregion

#region 出力処理用
//'    '' <summary>
//'    '' 出力前(エラーチェック)処理
//'    '' </summary>
//'    '' <returns></returns>
//'    Protected Overrides Function OvPt99PrintBefore() As Boolean
//'TODO:   画面項目定義(詳細)<出力前処理＞

//'        Return MyBase.OvPt99PrintBefore()
//'    End Function

///' <summary>
///' DS出力時(パラメータ設定処理)処理
///' </summary>
///' <returns></returns>
//Protected Overrides Function OvPt99PrintSetDSParameter() As Boolean
//    'TODO:DSWEB帳票設計書

//    Return MyBase.OvPt99PrintSetDSParameter()
//End Function

///' <summary>
///' AR出力時(データ取得)処理
///' </summary>
///' <returns></returns>
//Protected Overrides Function OvPt99PrintARGetData() As DataTable
//    'TODO:画面項目定義(詳細)＜出力処理＞(ARの場合)

//    Return MyBase.OvPt99PrintARGetData()
//End Function

///' <summary>
///' 出力後(データ加工等)処理
///' </summary>
//Protected Overrides Sub OvPt99printAfter()
//    'TODO:画面項目定義(詳細)＜出力後処理＞

//End Sub
#endregion

#region PT99外ファンクションキー
///' <summary>
///' FXXボタン押下時の独自処理
///' </summary>
//Protected Overrides Sub btnFXX_ClickOrgProc()
//End Sub
#endregion

#endregion


#region Privateメソッド(画面独自)
/// <summary>
/// Grid列初期設定
/// </summary>
private void InitializeResultGrid()
{
	DataTable dtNotificationHohoSiireSakiType = CommonProcess.getComboboxDataOfDatatable(typeof(NotificationHohoKoshaRenraku), false);
	//予約情報
	List[] grdYoyakuInfoList = new List[Of GridProperty] From {
		;
		new GridProperty() {.Caption = getEnumAttrValue(YoyakuInfoList.TITLE),.Name == YoyakuInfoList.TITLE.ToString(),.Visible == true,.Width == 104 },;
		new GridProperty() {.Caption = getEnumAttrValue(YoyakuInfoList.YOYAKU_NUM),.Name == YoyakuInfoList.YOYAKU_NUM.ToString(),.Visible == true,.Width == 105 };
	};
	this.grdYoyakuInfo.Initialize(grdYoyakuInfoList);
	//人員区分内訳
	List[] grdJininUtiList = new List[Of GridProperty] From {
		;
		new GridProperty() {.Caption = getEnumAttrValue(JininUtiList.JININ_KBN),.Name == JininUtiList.JININ_KBN.ToString(),.Visible == true,.Width == 135 },;
		new GridProperty() {.Caption = getEnumAttrValue(JininUtiList.NINZU),.Name == JininUtiList.NINZU.ToString(),.Visible == true,.Width == 130 };
	};
	this.grdJininUti.Initialize(grdJininUtiList);
	//予約人数内訳
	List[] grdYoyakuUtiList = new List[Of GridProperty] From {
		;
		new GridProperty() {.Caption = getEnumAttrValue(YoyakuUtiList.YOYAKU_NINZU),.Name == YoyakuUtiList.YOYAKU_NINZU.ToString(),.Visible == true,.Width == 122 },;
		new GridProperty() {.Caption = getEnumAttrValue(YoyakuUtiList.YOYAKU_KENSU),.Name == YoyakuUtiList.YOYAKU_KENSU.ToString(),.Visible == true,.Width == 120 },;
		new GridProperty() {.Caption = getEnumAttrValue(YoyakuUtiList.YOYAKU_KEI),.Name == YoyakuUtiList.YOYAKU_KEI.ToString(),.Visible == true,.Width == 122 };
	};
	this.grdYoyakuUti.Initialize(grdYoyakuUtiList);
	//降車ヶ所の仕入先情報
	List[] grdSiireInfoList = new List[Of GridProperty] From {
		;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SELECT),.Name == SiireInfoList.SELECT.ToString(),.Width == 40,.DataType == typeof(bool),.AllowEditing == true },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.DAILY),.Name == SiireInfoList.DAILY.ToString(),.Visible == true,.Width == 100 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRE_SAKI_KIND_CD),.Name == SiireInfoList.SIIRE_SAKI_KIND_CD.ToString(),.Visible == false,.Width == 120 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRE_SAKI_KIND_NAME),.Name == SiireInfoList.SIIRE_SAKI_KIND_NAME.ToString(),.Visible == true,.Width == 145 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRE_SAKI_CD),.Name == SiireInfoList.SIIRE_SAKI_CD.ToString(),.Visible == false,.Width == 100 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRE_SAKI_EDABAN),.Name == SiireInfoList.SIIRE_SAKI_EDABAN.ToString(),.Visible == false,.Width == 120 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRESAKI_CD),.Name == SiireInfoList.SIIRESAKI_CD.ToString(),.Visible == true,.Width == 90 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRE_SAKI_NAME),.Name == SiireInfoList.SIIRE_SAKI_NAME.ToString(),.Visible == true,.Width == 143 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.TEL_STR),.Name == SiireInfoList.TEL_STR.ToString(),.Visible == true,.Width == 130 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.FAX_STR),.Name == SiireInfoList.FAX_STR.ToString(),.Visible == true,.Width == 130 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.FAX2_STR),.Name == SiireInfoList.FAX2_STR.ToString(),.Visible == true,.Width == 130 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.MAIL),.Name == SiireInfoList.MAIL.ToString(),.Visible == true,.Width == 140 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.NOTIFICATION_HOHO),.Name == SiireInfoList.NOTIFICATION_HOHO.ToString(),.Width == 70,.DataMap == CommonProcess.getComboboxData(dtNotificationHohoSiireSakiType, false),.AllowEditing == true },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.CONTACT_STATU),.Name == SiireInfoList.CONTACT_STATU.ToString(),.Visible == true,.Width == 90 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.ALLOW_EDIT),.Name == SiireInfoList.ALLOW_EDIT.ToString(),.Visible == false,.Width == 90 };
	};
	this.grdSiireInfo.Initialize(grdSiireInfoList);

	//オプションの仕入先情報
	List[] grdSiireInfoOptionList = new List[Of GridProperty] From {
		;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SELECT),.Name == SiireInfoList.SELECT.ToString(),.Width == 40,.DataType == typeof(bool),.AllowEditing == true },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.DAILY),.Name == SiireInfoList.DAILY.ToString(),.Visible == false,.Width == 100 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRE_SAKI_KIND_CD),.Name == SiireInfoList.SIIRE_SAKI_KIND_CD.ToString(),.Visible == false,.Width == 120 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRE_SAKI_KIND_NAME),.Name == SiireInfoList.SIIRE_SAKI_KIND_NAME.ToString(),.Visible == true,.Width == 161 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRE_SAKI_CD),.Name == SiireInfoList.SIIRE_SAKI_CD.ToString(),.Visible == false,.Width == 100 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRE_SAKI_EDABAN),.Name == SiireInfoList.SIIRE_SAKI_EDABAN.ToString(),.Visible == false,.Width == 120 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRESAKI_CD),.Name == SiireInfoList.SIIRESAKI_CD.ToString(),.Visible == true,.Width == 114 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.SIIRE_SAKI_NAME),.Name == SiireInfoList.SIIRE_SAKI_NAME.ToString(),.Visible == true,.Width == 161 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.TEL_STR),.Name == SiireInfoList.TEL_STR.ToString(),.Visible == true,.Width == 135 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.FAX_STR),.Name == SiireInfoList.FAX_STR.ToString(),.Visible == true,.Width == 135 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.FAX2_STR),.Name == SiireInfoList.FAX2_STR.ToString(),.Visible == true,.Width == 135 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.MAIL),.Name == SiireInfoList.MAIL.ToString(),.Visible == true,.Width == 150 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.NOTIFICATION_HOHO),.Name == SiireInfoList.NOTIFICATION_HOHO.ToString(),.Width == 80,.DataMap == CommonProcess.getComboboxData(dtNotificationHohoSiireSakiType, false),.AllowEditing == true },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.CONTACT_STATU),.Name == SiireInfoList.CONTACT_STATU.ToString(),.Visible == true,.Width == 92 },;
		new GridProperty() {.Caption = getEnumAttrValue(SiireInfoList.ALLOW_EDIT),.Name == SiireInfoList.ALLOW_EDIT.ToString(),.DataType == typeof(bool),.Visible == false,.Width == 90 };
	};
	this.grdSiireInfoOption.Initialize(grdSiireInfoOptionList);

	//オプション内訳
	List[] grdOptionList = new List[Of GridProperty] From {
		;
		new GridProperty() {.Caption = getEnumAttrValue(OptionList.OPTIONAL_NAME),.Name == OptionList.OPTIONAL_NAME.ToString(),.Visible == true,.Width == 120 },;
		new GridProperty() {.Caption = getEnumAttrValue(OptionList.OPTION_NINZU),.Name == OptionList.OPTION_NINZU.ToString(),.Visible == true,.Width == 152 },;
		new GridProperty() {.Caption = getEnumAttrValue(OptionList.OPTION_KENSU),.Name == OptionList.OPTION_KENSU.ToString(),.Visible == true,.Width == 139 },;
		new GridProperty() {.Caption = getEnumAttrValue(OptionList.OPTION_KEI),.Name == OptionList.OPTION_KEI.ToString(),.Visible == true,.Width == 154 };
	};
	this.grdOption.Initialize(grdOptionList);
}

/// <summary>
/// 処理区分「検索」参考
/// </summary>
private void GetAllDataDANo1ToNo4()
{
	//DA定義No1～No4を実施
	S04_0401_DA dataAccess = new S04_0401_DA();
	S04_0401_DA.S04_0401SelectParam param = new S04_0401_DA.S04_0401SelectParam();

	//パラメータはIF(OUT)シートNo1参照
	param.SyuptDay = ParamData.SyuptDay;
	param.CrsCd = ParamData.CrsCd;
	param.Gousya = ParamData.Gousya;

	//DA定義No.2で取得したものは「画面項目定義」のNo.①で使用
	DataTable dataTYoyakuInfoBasic = dataAccess.selectDataTYoyakuInfoBasicDANo2(param);

	//Grid詳細①参照
	SetDataYoyakuInfo(dataTYoyakuInfoBasic);

	//Grid詳細②参照
	SetDataJininUti(dataTYoyakuInfoBasic);

	//Grid詳細③参照
	SetDataYoyakuUti(dataTYoyakuInfoBasic);

	//DA定義No.1・3・4で取得したものは「画面項目定義」のNo.②・③で使用
	//Grid詳細④参照
	//DA定義No1はgrdSiireInfo
	DataTable dataGrdSiireInfo = dataAccess.selectDataTCrsLedgerBasicDANo1(param);
	this.grdSiireInfo.DataSource = SetDataSiireInfo(dataGrdSiireInfo);
	dataTableGrdSiireInfo = SetDataSiireInfo(dataGrdSiireInfo);

	//DA定義No.3はgrdSiireInfoOption
	DataTable dataGrdSiireInfoOption = dataAccess.selectDataTCrsLedgerBasicDANo3(param);
	this.grdSiireInfoOption.DataSource = SetDataSiireInfo(dataGrdSiireInfoOption);
	dataTableGrdSiireInfoOption = SetDataSiireInfo(dataGrdSiireInfoOption);


	dataTablegrdOption = dataAccess.selectDataTCrsLedgerBasicDANo4(param);
	//DA定義No4の取得件数が0件であった場合、オプションタブを削除する
	if (ReferenceEquals(dataTablegrdOption, null) || dataTablegrdOption.Rows.Count == 0)
	{
		this.tabControl.TabPages.Remove(Tab_infoOption);
	}
}


/// <summary>
/// セットアップの予約情報
/// </summary>
private void SetDataYoyakuInfo(DataTable dataTYoyakuInfoBasic)
{
	int YoyakuKbn = System.Convert.ToInt32(dataTYoyakuInfoBasic.Rows.Count);
	int ChargeApplicationNinzu = 0;
	int InfantNinzu = 0;
	for (i = 0; i <= dataTYoyakuInfoBasic.Rows.Count - 1; i++)
	{
		ChargeApplicationNinzu += System.Convert.ToInt32(if (!Information.IsDBNull(dataTYoyakuInfoBasic.Rows(i).Item(ListItemYoyaku.CHARGE_APPLICATION_NINZU.ToString())), System.Convert.ToInt32(dataTYoyakuInfoBasic.Rows(i).Item(ListItemYoyaku.CHARGE_APPLICATION_NINZU.ToString())), 0));
InfantNinzu += System.Convert.ToInt32(if (!Information.IsDBNull(dataTYoyakuInfoBasic.Rows(i).Item(ListItemYoyaku.INFANT_NINZU.ToString())), System.Convert.ToInt32(dataTYoyakuInfoBasic.Rows(i).Item(ListItemYoyaku.INFANT_NINZU.ToString())), 0));
}
//Grid詳細①参照
DataTable dtTYoyakuInfoBasic = new DataTable();
dtTYoyakuInfoBasic.Columns.Add(YoyakuInfoList.TITLE.ToString(), typeof(string)); //タイトル
dtTYoyakuInfoBasic.Columns.Add(YoyakuInfoList.YOYAKU_NUM.ToString(), typeof(int)); //予約数

//1行目：「予約件数」, 1行目：　COUNT(予約区分)
dtTYoyakuInfoBasic.Rows.Add(YoyakuInfoLine1.ToString(), YoyakuKbn);
//2行目：「予約人数」, 2行目：　SUM（料金適用人数）
dtTYoyakuInfoBasic.Rows.Add(YoyakuInfoLine2.ToString(), ChargeApplicationNinzu);
//3行目：　「インファント」を設定, 3行目：　SUM（インファント人数）
dtTYoyakuInfoBasic.Rows.Add(YoyakuInfoLine3.ToString(), InfantNinzu);
//予約情報
this.grdYoyakuInfo.DataSource = dtTYoyakuInfoBasic;
}

/// <summary>
/// セットアップの人員内訳
/// </summary>
private void SetDataJininUti(DataTable dataTYoyakuInfoBasic)
{

	//料金区分（人員）コード単位でSUM
	DataRow listTYoyakuInfoBasic = (From row DataRow in dataTYoyakuInfoBasic;
	Group(row.Item(ListItemYoyaku.CHARGE_APPLICATION_NINZU.ToString()));
	By CHARGE_KBN_JININ_CD = row.Item(ListItemYoyaku.CHARGE_KBN_JININ_CD.ToString()) Into ChargeGroup == Group;
	Key(CHARGE_KBN_JININ_CD,);
.CHARGE_APPLICATION_NINZU = ChargeGroup.Sum(Function(item) System.Convert.ToInt32(item));
}).ToList();

//Grid詳細②参照
DataTable dtTYoyakuInfoBasicGirdNo2 = new DataTable();
dtTYoyakuInfoBasicGirdNo2.Columns.Add(JininUtiList.JININ_KBN.ToString(), typeof(string)); //人員区分
dtTYoyakuInfoBasicGirdNo2.Columns.Add(JininUtiList.NINZU.ToString(), typeof(int)); //人数
string ChargeKbnJininCd = string.Empty;
int ChargeSumNinzu = 0;
for (i = 0; i <= listTYoyakuInfoBasic.Count - 1; i++)
{
	ChargeKbnJininCd = System.Convert.ToString(listTYoyakuInfoBasic.Item(i).CHARGE_KBN_JININ_CD.ToString());
	//Fixedcdの料金区分（人員）コードのバリューを設定
	if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.adultMan)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.adultMan));
	}
	else if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.adultWoman)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.adultWoman));
	}
	else if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.juniorMan)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.juniorMan));
	}
	else if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.childMan)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.childMan));
	}
	else if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.toddler)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.toddler));
	}
	else if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.elder)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.elder));
	}
	else if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.juniorWoman)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.juniorWoman));
	}
	else if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.childWoman)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.childWoman));
	}
	else if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.adult)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.adult));
	}
	else if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.junior)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.junior));
	}
	else if (ChargeKbnJininCd == FixedCd.ChargeKbnJininCd.child)
	{
		ChargeKbnJininCd = System.Convert.ToString(getEnumAttrValue(ValueChargeKbnJininCd.child));
	}
	ChargeSumNinzu = System.Convert.ToInt32(listTYoyakuInfoBasic.Item(i).CHARGE_APPLICATION_NINZU);
	dtTYoyakuInfoBasicGirdNo2.Rows.Add(ChargeKbnJininCd, ChargeSumNinzu);
}
//人員内訳
this.grdJininUti.DataSource = dtTYoyakuInfoBasicGirdNo2;
}

/// <summary>
/// セットアップの予約内訳
/// </summary>
private void SetDataYoyakuUti(DataTable dataTYoyakuInfoBasic)
{
	//予約区分、予約NO単位でSUM ※重複分は表示しない
	//予約区分、予約NO単位でSUMした予約件数をカウント
	DataRow listTYoyakuInfoBasic = (From row DataRow in dataTYoyakuInfoBasic;
	Group;
	{
		;
		row.Item(ListItemYoyaku.YOYAKU_KBN.ToString()),;
		row.Item(ListItemYoyaku.CHARGE_APPLICATION_NINZU.ToString());
	}
	By;
	YOYAKU_KBN = row.Item(ListItemYoyaku.YOYAKU_KBN.ToString()), YOYAKU_NO == row.Item(ListItemYoyaku.YOYAKU_NO.ToString()) Into YoyakuGroup == Group;
	Key(YOYAKU_KBN, YOYAKU_NO,);
.YOYAKU_KENSU = YoyakuGroup.Sum(Function(item) System.Convert.ToInt32(item(0))),;
.YOYAKU_NINZU = YoyakuGroup.Sum(Function(item) System.Convert.ToInt32(item(1)));
}).ToList();

DataTable dtTYoyakuInfoBasicGirdNo3 = new DataTable();
dtTYoyakuInfoBasicGirdNo3.Columns.Add(YoyakuUtiList.YOYAKU_NINZU.ToString(), typeof(int)); //予約人数
dtTYoyakuInfoBasicGirdNo3.Columns.Add(YoyakuUtiList.YOYAKU_KENSU.ToString(), typeof(int)); //予約件数
dtTYoyakuInfoBasicGirdNo3.Columns.Add(YoyakuUtiList.YOYAKU_KEI.ToString(), typeof(int)); //計
int YoyakuNinzu = 0;
int YoyakuKensu = 0;
int YoyakuKei = 0;
for (i = 0; i <= listTYoyakuInfoBasic.Count - 1; i++)
{
	//予約人数
	YoyakuNinzu = System.Convert.ToInt32(listTYoyakuInfoBasic.Item(i).YOYAKU_NINZU);
	//予約件数
	YoyakuKensu = System.Convert.ToInt32(listTYoyakuInfoBasic.Item(i).YOYAKU_KENSU);
	//計
	YoyakuKei = YoyakuNinzu * YoyakuKensu;
	dtTYoyakuInfoBasicGirdNo3.Rows.Add(YoyakuNinzu, YoyakuKensu, YoyakuKei);
}
//予約内訳
this.grdYoyakuUti.DataSource = dtTYoyakuInfoBasicGirdNo3;
}

/// <summary>
/// 仕入先情報
/// </summary>
public DataTable SetDataSiireInfo(DataTable dataTable)
{
	DataTable dataSiireInfo = new DataTable();
	dataSiireInfo.Columns.Add(SiireInfoList.DAILY.ToString(), typeof(int)); //日次
	dataSiireInfo.Columns.Add(SiireInfoList.SIIRE_SAKI_KIND_CD.ToString(), typeof(string)); //仕入先種別コード
	dataSiireInfo.Columns.Add(SiireInfoList.SIIRE_SAKI_KIND_NAME.ToString(), typeof(string)); //仕入先種別
	dataSiireInfo.Columns.Add(SiireInfoList.SIIRE_SAKI_CD.ToString(), typeof(string)); //仕入先コード
	dataSiireInfo.Columns.Add(SiireInfoList.SIIRE_SAKI_EDABAN.ToString(), typeof(string)); //仕入先枝番
	dataSiireInfo.Columns.Add(SiireInfoList.SIIRESAKI_CD.ToString(), typeof(string)); //仕入先
	dataSiireInfo.Columns.Add(SiireInfoList.SIIRE_SAKI_NAME.ToString(), typeof(string)); //仕入先名称
	dataSiireInfo.Columns.Add(SiireInfoList.TEL_STR.ToString(), typeof(string)); //TEL
	dataSiireInfo.Columns.Add(SiireInfoList.FAX_STR.ToString(), typeof(string)); //FAX
	dataSiireInfo.Columns.Add(SiireInfoList.FAX2_STR.ToString(), typeof(string)); //FAX2
	dataSiireInfo.Columns.Add(SiireInfoList.MAIL.ToString(), typeof(string)); //メールアドレス
	dataSiireInfo.Columns.Add(SiireInfoList.NOTIFICATION_HOHO.ToString(), typeof(string)); //通知方法
	dataSiireInfo.Columns.Add(SiireInfoList.SEND_YMDT.ToString(), typeof(DateTime)); //最終送信日時
	dataSiireInfo.Columns.Add(SiireInfoList.NOTEWORTHY.ToString(), typeof(string)); //特記事項
	dataSiireInfo.Columns.Add(SiireInfoList.REMARKS.ToString(), typeof(string)); //備考
	dataSiireInfo.Columns.Add(SiireInfoList.INFANT_INFO.ToString(), typeof(string)); //備考
	dataSiireInfo.Columns.Add(SiireInfoList.SYSTEM_ENTRY_PGMID.ToString(), typeof(string)); //システム登録ＰＧＭＩＤ
	dataSiireInfo.Columns.Add(SiireInfoList.SYSTEM_ENTRY_PERSON_CD.ToString(), typeof(string)); //システム登録者コード
	dataSiireInfo.Columns.Add(SiireInfoList.SYSTEM_ENTRY_DAY.ToString(), typeof(DateTime)); //システム登録者コード
	dataSiireInfo.Columns.Add(SiireInfoList.CONTACT_STATU.ToString(), typeof(string)); //連絡状況
	dataSiireInfo.Columns.Add(SiireInfoList.ALLOW_EDIT.ToString(), typeof(bool)); //画面出力有無編集許可フラグ

	for (i = 0; i <= dataTable.Rows.Count - 1; i++)
	{
		//日次
		System.Int32 Daily = System.Convert.ToInt32(dataTable.Rows(i).Item(SiireInfoList.DAILY.ToString()));
		//仕入先種別コード
		object SiireSakiKindCd = if (!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.SIIRE_SAKI_KIND_CD.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.SIIRE_SAKI_KIND_CD.ToString())), string.Empty);
//仕入先種別
object SiireSakiKindName = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.SIIRE_SAKI_KIND_NAME.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.SIIRE_SAKI_KIND_NAME.ToString())), string.Empty);
//仕入先コード
object SiireSakiCd = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.SIIRE_SAKI_CD.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.SIIRE_SAKI_CD.ToString())), string.Empty);
//仕入先枝番
object SiireSakieDaban = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.SIIRE_SAKI_EDABAN.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.SIIRE_SAKI_EDABAN.ToString())), string.Empty);
//仕入先
object SiireSaki_Cd = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.SIIRESAKI_CD.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.SIIRESAKI_CD.ToString())), string.Empty);
//仕入先名称
object SiireSakiName = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.SIIRE_SAKI_NAME.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.SIIRE_SAKI_NAME.ToString())), string.Empty);
//TEL
object TelStr = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.TEL_STR.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.TEL_STR.ToString())), string.Empty);
//FAX
object FaxStr = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.FAX_STR.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.FAX_STR.ToString())), string.Empty);
//FAX2
object Fax2Str = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.FAX2_STR.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.FAX2_STR.ToString())), string.Empty);
//メールアドレス
object Mail = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.MAIL.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.MAIL.ToString())), string.Empty);
//通知方法
object NoticationHoho = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.NOTIFICATION_HOHO.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.NOTIFICATION_HOHO.ToString())), string.Empty);
//最終送信日時
Date? SendYmdt = null;
if (dataTable.Rows(i).Item(SiireInfoList.SEND_YMDT.ToString()) IsNot null && !string.IsNullOrEmpty(dataTable.Rows(i).Item(SiireInfoList.SEND_YMDT.ToString()).ToString()))
{
	SendYmdt = if (!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.SEND_YMDT.ToString())), System.Convert.ToDateTime(dataTable.Rows(i).Item(SiireInfoList.SEND_YMDT.ToString())), null);
}
//特記事項
object Noteworthy = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.NOTEWORTHY.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.NOTEWORTHY.ToString())), string.Empty);
//備考
object Remarks = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.REMARKS.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.REMARKS.ToString())), string.Empty);
//インファント情報
object InfantInfo = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.INFANT_INFO.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.INFANT_INFO.ToString())), string.Empty);
//システム登録ＰＧＭＩＤ
object SystemEntryPgmid = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.SYSTEM_ENTRY_PGMID.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.SYSTEM_ENTRY_PGMID.ToString())), string.Empty);
//システム登録者コード
object SystemEntryPersonCd = if(!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.SYSTEM_ENTRY_PERSON_CD.ToString())), System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.SYSTEM_ENTRY_PERSON_CD.ToString())), string.Empty);
//システム登録日
DateTime SystemEntryDay = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
if (ReferenceEquals(dataTable.Rows(i).Item(SiireInfoList.SYSTEM_ENTRY_DAY.ToString()), null) && !string.IsNullOrEmpty(dataTable.Rows(i).Item(SiireInfoList.SYSTEM_ENTRY_DAY.ToString()).ToString()))
{
	SystemEntryDay = System.Convert.ToDateTime(if (!Information.IsDBNull(dataTable.Rows(i).Item(SiireInfoList.SYSTEM_ENTRY_DAY.ToString())), System.Convert.ToDateTime(dataTable.Rows(i).Item(SiireInfoList.SYSTEM_ENTRY_DAY.ToString())), CommonDateUtil.getSystemTime()));
}
else
{
	SystemEntryDay = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
}
System.String ContactStatu = string.Empty;
//画面出力有無編集許可フラグ
bool AllowEdit = false;
if (string.Equals(System.Convert.ToString(dataTable.Rows(i).Item(SiireInfoList.ALLOW_EDIT.ToString())), "1"))
{
	AllowEdit = true;
}

//連絡状況
dataSiireInfo.Rows.Add(;
Daily(, SiireSakiKindCd, SiireSakiKindName, SiireSakiCd, SiireSakieDaban, SiireSaki_Cd, SiireSakiName, TelStr, FaxStr,);
Fax2Str(, Mail, NoticationHoho, SendYmdt, Noteworthy, Remarks, InfantInfo,);
SystemEntryPgmid(, SystemEntryPersonCd, SystemEntryDay, ContactStatu, AllowEdit));
}
return dataSiireInfo;
}

/// <summary>
/// オプション内訳
/// </summary>
public void SetDataGrdOption(string SiireSakiCd, string SiireSakiEdaban)
{
	//予約区分、予約NO単位でSUMした予約件数をカウント
	DataRow listGrdOption = (From row DataRow in dataTablegrdOption;
	Where row.Item(SiireInfoList.SIIRE_SAKI_CD.ToString()).ToString = System.Convert.ToString(SiireSakiCd && row.Item(SiireInfoList.SIIRE_SAKI_EDABAN.ToString()).ToString() == SiireSakiEdaban);
	Group;
	{
		;
		row.Item(OptionList.OPTIONAL_NAME.ToString()),;
		row.Item(OptionList.OPTION_NINZU.ToString()),;
		row.Item(ListItemYoyaku.YOYAKU_KBN.ToString());
	}
	By;
	YOYAKU_KBN = row.Item(ListItemYoyaku.YOYAKU_KBN.ToString()), YOYAKU_NO == row.Item(ListItemYoyaku.YOYAKU_NO.ToString()) Into YoyakuGroup == Group;
	Key(YOYAKU_KBN, YOYAKU_NO,);
.OPTIONAL_NAME = YoyakuGroup.Select(Function(item) item(0)).First,;
.OPTION_NINZU = YoyakuGroup.Sum(Function(item) System.Convert.ToInt32(item(1))),;
.YOYAKU_KENSU = YoyakuGroup.Select(Function(item) item(2)).First;
}).ToList();

DataTable dataGrdOption = new DataTable();
dataGrdOption.Columns.Add(OptionList.OPTIONAL_NAME.ToString(), typeof(string)); //オプション名
dataGrdOption.Columns.Add(OptionList.OPTION_NINZU.ToString(), typeof(string)); //予約人数
dataGrdOption.Columns.Add(OptionList.OPTION_KENSU.ToString(), typeof(string)); //予約件数
dataGrdOption.Columns.Add(OptionList.OPTION_KEI.ToString(), typeof(string)); //計

for (i = 0; i <= listGrdOption.Count - 1; i++)
{
	//オプション名
	object optionalName = listGrdOption.Item(i).OPTIONAL_NAME;
	//予約人数
	System.Int32 optionNinzu = System.Convert.ToInt32(listGrdOption.Item(i).OPTION_NINZU);
	//予約件数
	System.Int32 optionKensu = System.Convert.ToInt32(listGrdOption.Item(i).YOYAKU_KENSU);
	//計
	System.Int32 optionKei = System.Convert.ToInt32(listGrdOption.Item(i).OPTION_NINZU);

	dataGrdOption.Rows.Add(optionalName, optionNinzu, optionKensu, optionKei);
}
this.grdOption.DataSource = dataGrdOption;
}

/// <summary>
/// タブ変更時
/// </summary>
private void tabControlSelecting(object sender, System.EventArgs e)
{
	//チェック要件のチェックNo1を実施する
	if (flag == 0 && checkNotReflected() == true)
	{
		OvPt99SearchAfter();
		//インファント・特記事項・備考の表示をクリア、連絡済チェックボックスを非活性とする
		this.txtNoteSp.Text = string.Empty;
		this.txtRemarks.Text = string.Empty;
		this.txtInfant.Text = string.Empty;
		this.chkContacted.Checked = false;
		this.chkContacted.Enabled = false;
		//選択タブの仕入先情報グリッド1行目にフォーカスをあて、1行目のインファント・特記事項・備考・仕入先コード・仕入先枝番・日次を「画面項目定義」のNo.③へ設定し、表示する
		selectedRowIndex = 1;
		if (this.tabControl.SelectedTab.TabIndex == this.Tab_info.TabIndex)
		{
			if (this.grdSiireInfo.Rows.Count > 0 && this.grdSiireInfo.DataSource IsNot null)
{
	currentRow = ((DataRowView)(this.grdSiireInfo.Rows(selectedRowIndex).DataSource)).Row;
	this.grdSiireInfo.Rows(selectedRowIndex).Selected = true;
}
}
else if (this.tabControl.SelectedTab.TabIndex == this.Tab_infoOption.TabIndex)
{
	if (this.grdSiireInfoOption.Rows.Count > 0 && this.grdSiireInfoOption.DataSource IsNot null)
{
		currentRow = ((DataRowView)(this.grdSiireInfoOption.Rows(selectedRowIndex).DataSource)).Row;
		SetDataGrdOption(System.Convert.ToString(currentRow.Item(SiireInfoList.SIIRE_SAKI_CD.ToString()).ToString()), System.Convert.ToString(currentRow.Item(SiireInfoList.SIIRE_SAKI_EDABAN.ToString()).ToString()));
		this.grdSiireInfoOption.Rows(selectedRowIndex).Selected = true;
	}
}

this.txtNoteSp.Text = currentRow.Item(SiireInfoList.NOTEWORTHY.ToString()).ToString();
this.txtRemarks.Text = currentRow.Item(SiireInfoList.REMARKS.ToString()).ToString();
this.txtInfant.Text = currentRow.Item(SiireInfoList.INFANT_INFO.ToString()).ToString();
this.txtSiireSakiCd.Text = currentRow.Item(SiireInfoList.SIIRE_SAKI_CD.ToString()).ToString();
this.txtSiireSakiEdaban.Text = currentRow.Item(SiireInfoList.SIIRE_SAKI_EDABAN.ToString()).ToString();
this.txtDaily.Text = currentRow.Item(SiireInfoList.DAILY.ToString()).ToString();
//1行目の通知手段が「電話」であった場合、連絡済チェックボックスを活性とする
if (currentRow.Item(SiireInfoList.NOTIFICATION_HOHO.ToString()).ToString() IsNot DBNull.Value)
{
	if (string.Compare(System.Convert.ToString(this.grdSiireInfo.GetData(selectedRowIndex, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Tel.ToString()) == 0)
	{
		this.chkContacted.Enabled = true;
	}
}
}
else
{
	flag++;
	if (flag == 1)
	{
		if (this.tabControl.SelectedTab.TabIndex == this.Tab_info.TabIndex)
		{
			if (this.grdSiireInfo.Rows.Count > 0 && this.grdSiireInfo.DataSource IsNot null)
{
				this.tabControl.SelectedIndex = 1;
				this.grdSiireInfoOption.Rows(selectedRowIndex).Selected = true;
			}
		}
		else if (this.tabControl.SelectedTab.TabIndex == this.Tab_infoOption.TabIndex)
		{
			if (this.grdSiireInfoOption.Rows.Count > 0 && this.grdSiireInfoOption.DataSource IsNot null)
{
				this.tabControl.SelectedIndex = 0;
				this.grdSiireInfo.Rows(selectedRowIndex).Selected = true;
			}
		}
	}
	else
	{
		flag = 0;
	}
}
}

/// <summary>
/// 行変更時
/// </summary>
private void grdSiireInfoClick(object sender, EventArgs e)
{
	//Tab 降車ヶ所
	FlexGridEx grid = (FlexGridEx)sender;
	if (this.tabControl.SelectedTab.TabIndex == this.Tab_info.TabIndex && grid.RowSel != selectedRowIndex && grid.RowSel != 0)
	{
		//チェック要件のチェックNo1を実施する
		if (checkNotReflected() == true)
		{
			if (grdSiireInfo.Rows.Count != 0 && this.grdSiireInfo.DataSource IsNot null)
{
	//選択行のインファント・特記事項・備考・仕入先コード・仕入先枝番・仕入先コード・仕入先枝番を「画面項目定義」のNo.③へ設定し、表示する
	this.grdSiireInfo.Rows(selectedRowIndex).Selected = false;
	if (grid.RowSel >= 0)
	{
		selectedRowIndex = System.Convert.ToInt32(grid.RowSel);
	}
	this.grdSiireInfo.Rows(selectedRowIndex).Selected = true;
	currentRow = ((DataRowView)(this.grdSiireInfo.Rows(selectedRowIndex).DataSource)).Row;
	this.txtNoteSp.Text = currentRow.Item(SiireInfoList.NOTEWORTHY.ToString()).ToString();
	this.txtRemarks.Text = currentRow.Item(SiireInfoList.REMARKS.ToString()).ToString();
	this.txtInfant.Text = currentRow.Item(SiireInfoList.INFANT_INFO.ToString()).ToString();
	this.txtSiireSakiCd.Text = currentRow.Item(SiireInfoList.SIIRE_SAKI_CD.ToString()).ToString();
	this.txtSiireSakiEdaban.Text = currentRow.Item(SiireInfoList.SIIRE_SAKI_EDABAN.ToString()).ToString();
	this.txtDaily.Text = currentRow.Item(SiireInfoList.DAILY.ToString()).ToString();
	//反映ボタンを活性とする
	this.btnDataEntry.Enabled = true;

	//通知方法が「電話」以外の場合、非活性
	if (System.Convert.ToInt32(NotificationHohoKoshaRenraku.Tel) == System.Convert.ToInt32(currentRow.Item(SiireInfoList.NOTIFICATION_HOHO.ToString()).ToString()))
	{
		//連絡済チェックボックスを活性とする
		this.chkContacted.Enabled = true;
	}
	else
	{
		//連絡済チェックボックスを非活性とする
		this.chkContacted.Enabled = false;
	}
}
}
else
{
	//キャンセルが押下された場合、以降の処理は行わなず、フォーカスを元あった行へ戻す
	if (this.grdSiireInfo.Rows.Count > this.grdSiireInfo.Rows.Fixed)
	{
		this.grdSiireInfo.Rows(selectedRowIndex).Selected = true;
	}
}
}
}

/// <summary>
/// 行変更時
/// </summary>
private void grdSiireInfoOptionClick(object sender, EventArgs e)
{
	//Tab 仕入先情報
	FlexGridEx grid = (FlexGridEx)sender;
	if (this.tabControl.SelectedTab.TabIndex == this.Tab_infoOption.TabIndex && grid.RowSel != selectedRowIndex && grid.RowSel > 0)
	{
		//チェック要件のチェックNo1を実施する
		if (checkNotReflected() == true)
		{
			if (this.grdSiireInfoOption.Rows.Count != 0 && this.grdSiireInfoOption.DataSource IsNot null)
{
	//選択行のインファント・特記事項・備考・仕入先コード・仕入先枝番・仕入先コード・仕入先枝番を「画面項目定義」のNo.③へ設定し、表示する
	this.grdSiireInfoOption.Rows(selectedRowIndex).Selected = false;
	if (grid.RowSel >= 0)
	{
		selectedRowIndex = System.Convert.ToInt32(grid.RowSel);
	}
	this.grdSiireInfoOption.Rows(selectedRowIndex).Selected = true;
	currentRow = ((DataRowView)(this.grdSiireInfoOption.Rows(selectedRowIndex).DataSource)).Row;
	this.txtNoteSp.Text = currentRow.Item(SiireInfoList.NOTEWORTHY.ToString()).ToString();
	this.txtRemarks.Text = currentRow.Item(SiireInfoList.REMARKS.ToString()).ToString();
	this.txtInfant.Text = currentRow.Item(SiireInfoList.INFANT_INFO.ToString()).ToString();
	this.txtSiireSakiCd.Text = currentRow.Item(SiireInfoList.SIIRE_SAKI_CD.ToString()).ToString();
	this.txtSiireSakiEdaban.Text = currentRow.Item(SiireInfoList.SIIRE_SAKI_EDABAN.ToString()).ToString();
	this.txtDaily.Text = currentRow.Item(SiireInfoList.DAILY.ToString()).ToString();
	//反映ボタンを活性とする
	this.btnDataEntry.Enabled = true;
	//通知方法が「電話」以外の場合、非活性
	if (System.Convert.ToInt32(NotificationHohoKoshaRenraku.Tel) == System.Convert.ToInt32(currentRow.Item(SiireInfoList.NOTIFICATION_HOHO.ToString()).ToString()))
	{
		//連絡済チェックボックスを活性とする
		this.chkContacted.Enabled = true;
	}
	else
	{
		//連絡済チェックボックスを非活性とする
		this.chkContacted.Enabled = false;
	}

	SetDataGrdOption(System.Convert.ToString(currentRow.Item(SiireInfoList.SIIRE_SAKI_CD.ToString()).ToString()), System.Convert.ToString(currentRow.Item(SiireInfoList.SIIRE_SAKI_EDABAN.ToString()).ToString()));
}
}
else
{
	//キャンセルが押下された場合、以降の処理は行わなず、フォーカスを元あった行へ戻す
	if (this.grdSiireInfoOption.Rows.Count > this.grdSiireInfoOption.Rows.Fixed)
	{
		this.grdSiireInfoOption.Rows(selectedRowIndex).Selected = true;
	}
}
}
}

/// <summary>
/// 通知手段変更時
/// </summary>
private void grdCellChanged(object sender, RowColEventArgs e)
{
	if (this.tabControl.SelectedTab.TabIndex == this.Tab_info.TabIndex)
	{
		if (e.Row >= this.grdSiireInfo.Rows().Fixed && e.Col == this.grdSiireInfo.Cols(SiireInfoList.NOTIFICATION_HOHO.ToString()).Index)
		{
			//通知手段を「電話」とした場合
			object notificationHoho = this.grdSiireInfo.GetData(e.Row, e.Col);

			if (System.Convert.ToInt32(NotificationHohoKoshaRenraku.Tel) == System.Convert.ToInt32(notificationHoho) && System.Convert.ToBoolean(this.grdSiireInfo.GetData(e.Row, SiireInfoList.ALLOW_EDIT.ToString())) != false)
			{
				//選択チェックボックスを非活性とする
				this.grdSiireInfo.SetCellCheck(e.Row, this.grdSiireInfo.Cols(SiireInfoList.SELECT.ToString()).Index, CheckEnum.Unchecked);
				this.grdSiireInfo.setColorNonEdit(e.Row, this.grdSiireInfo.Cols(SiireInfoList.SELECT.ToString()).Index, Color.Gray);
				//連絡済チェックボックスを活性とする
				this.chkContacted.Enabled = true;
			}
			else
			{
				if (System.Convert.ToBoolean(this.grdSiireInfo.GetData(e.Row, SiireInfoList.ALLOW_EDIT.ToString())) == true)
				{
					//通知手段を「電話」以外とした場合
					//選択チェックボックスを活性とする
					this.grdSiireInfo.clearBackColor(e.Row, this.grdSiireInfo.Cols(SiireInfoList.SELECT.ToString()).Index);
					//連絡済チェックボックスを非活性とする
					this.chkContacted.Enabled = false;
				}
			}
		}
	}
	else if (this.tabControl.SelectedTab.TabIndex == this.Tab_infoOption.TabIndex)
	{
		if (e.Row >= this.grdSiireInfoOption.Rows().Fixed && e.Col == this.grdSiireInfoOption.Cols(SiireInfoList.NOTIFICATION_HOHO.ToString()).Index)
		{
			//通知手段を「電話」とした場合
			object notificationHoho = this.grdSiireInfoOption.GetData(e.Row, e.Col);
			if (System.Convert.ToInt32(NotificationHohoKoshaRenraku.Tel) == System.Convert.ToInt32(notificationHoho) && System.Convert.ToBoolean(this.grdSiireInfoOption.GetData(e.Row, SiireInfoList.ALLOW_EDIT.ToString())) != false)
			{
				//選択チェックボックスを非活性とする
				this.grdSiireInfoOption.SetCellCheck(e.Row, this.grdSiireInfoOption.Cols(SiireInfoList.SELECT.ToString()).Index, CheckEnum.Unchecked);
				this.grdSiireInfoOption.setColorNonEdit(e.Row, this.grdSiireInfoOption.Cols(SiireInfoList.SELECT.ToString()).Index, Color.Gray);
				//連絡済チェックボックスを活性とする
				this.chkContacted.Enabled = true;
			}
			else
			{
				if (System.Convert.ToBoolean(this.grdSiireInfo.GetData(e.Row, SiireInfoList.ALLOW_EDIT.ToString())) == true)
				{
					//通知手段を「電話」以外とした場合
					//選択チェックボックスを活性とする
					this.grdSiireInfoOption.clearBackColor(e.Row, this.grdSiireInfoOption.Cols(SiireInfoList.SELECT.ToString()).Index);
					//連絡済チェックボックスを非活性とする
					this.chkContacted.Enabled = false;
				}
			}
		}
	}
}

/// <summary>
/// 反映未実施チェック
/// </summary>
private bool checkNotReflected()
{
	if (this.tabControl.SelectedTab.TabIndex == this.Tab_info.TabIndex)
	{
		if (this.grdSiireInfo.Rows.Count > this.grdSiireInfo.Rows.Fixed)
		{
			if (string.Equals(System.Convert.ToString(currentRow.Item(SiireInfoList.NOTEWORTHY.ToString()).ToString()), this.txtNoteSp.Text) == false ||)
			{
				string.Equals(System.Convert.ToString(currentRow.Item(SiireInfoList.REMARKS.ToString()).ToString()), this.txtRemarks.Text) = System.Convert.ToBoolean(false ||);
				string.Equals(System.Convert.ToString(currentRow.Item(SiireInfoList.INFANT_INFO.ToString()).ToString()), this.txtInfant.Text) = System.Convert.ToBoolean(false);
				if (createFactoryMsg.messageDisp("Q03_003", MessageQ03_003.ToString()) == MsgBoxResult.Cancel)
				{
					//③仕入先コード・③仕入先枝番に紐づく行へフォーカスを戻す.
					return false;
				}
				return true;
			}
			else
			{
				return true;
			}
		}
	}
	else if (this.tabControl.SelectedTab.TabIndex == this.Tab_infoOption.TabIndex)
	{
		if (this.grdSiireInfoOption.Rows.Count > this.grdSiireInfoOption.Rows.Fixed)
		{
			if (string.Equals(System.Convert.ToString(currentRow.Item(SiireInfoList.NOTEWORTHY.ToString()).ToString()), this.txtNoteSp.Text) == false ||)
			{
				string.Equals(System.Convert.ToString(currentRow.Item(SiireInfoList.REMARKS.ToString()).ToString()), this.txtRemarks.Text) = System.Convert.ToBoolean(false ||);
				string.Equals(System.Convert.ToString(currentRow.Item(SiireInfoList.INFANT_INFO.ToString()).ToString()), this.txtInfant.Text) = System.Convert.ToBoolean(false);
				if (createFactoryMsg.messageDisp("Q03_003", MessageQ03_003.ToString()) == MsgBoxResult.Cancel)
				{
					//③仕入先コード・③仕入先枝番に紐づく行へフォーカスを戻す.
					return false;
				}
				return true;
			}
			else
			{
				return true;
			}
		}
	}
	return false;
}

/// <summary>
/// 反映ボタン押下時
/// </summary>
private void btnDataEntryClick(object sender, EventArgs e)
{
	//＜反映処理＞
	updateTranData();
	//＜反映後処理＞

	//再検索処理を実施する
	//処理内容は初期化処理の検索処理と同じ
	GetAllDataDANo1ToNo4();
	OvPt99SearchAfter();

	//連絡済
	this.chkContacted.Checked = false;
}

/// <summary>
/// F10ボタン押下時
/// </summary>
protected override void btnF10_ClickOrgProc()
{
	//降車ヶ所人員連絡DAの更新処理を呼び出す
	S04_0401_DA dataAccess = new S04_0401_DA();
	List listParam = new List(Of S04_0401_DA.TKousyaJininControlUpdate);
	if (this.tabControl.SelectedTab.TabIndex == this.Tab_info.TabIndex)
	{
		if (getListParamGrdSiireInfo() IsNot null)
{
			listParam = getListParamGrdSiireInfo();
		}
	}
	else if (this.tabControl.SelectedTab.TabIndex == this.Tab_infoOption.TabIndex)
	{
		if (getListParamGrdSiireInfoOption() IsNot null)
{
			listParam = getListParamGrdSiireInfo();
		}
	}
	if (listParam.Count > 0)
	{
		//DA定義No6参照
		dataAccess.updateTKousyaJininControl(listParam);
	}
}

/// <summary>
/// 仕入先情報のデータを取得する
/// </summary>
private List[] getListParamGrdSiireInfo()
{
	List listParam = new List(Of S04_0401_DA.TKousyaJininControlUpdate);
	S04_0401_DA.TKousyaJininControlUpdate param = null;
	int checkedCnt = 0;
	for (int row = this.grdSiireInfo.Rows.Fixed; row <= this.grdSiireInfo.Rows.Count - 1; row++)
	{
		if (this.grdSiireInfo.getCheckBoxValue(row, 0))
		{
			checkedCnt++;
			//「出力有無」にチェックが入っているレコードは以下を処理する
			if (System.Convert.ToBoolean(this.grdSiireInfo.GetData(row, SiireInfoList.ALLOW_EDIT.ToString())) == true)
			{
				//通知方法が電話となっている場合、ここでは処理を行わない（処理をスキップする）
				if (string.Compare(System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Tel.ToString()) != 0)
				{
					//通知方法が不要となっている場合、3-2-1をスキップする
					string NotificationHoho = System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString()).ToString().ToString());
					if (!string.IsNullOrEmpty(NotificationHoho))
					{
						//通知方法がFAXの場合、
						if (string.Compare(System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Fax.ToString()) == 0)
						{
							//API仕様未決定のためToDoとして残し、実装は不要
						}
						else
						{
							//API仕様未決定のためToDoとして残し、実装は不要
						}
					}
					//変更行のみパラメータに設定する。
					if (string.Compare(System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(),)
					{
						System.Convert.ToString(dataTableGrdSiireInfo.Rows(row - 1).Item(SiireInfoList.NOTIFICATION_HOHO.ToString()).ToString()).Trim()) != 0;
//パラメータはIF(OUT)シートNo.3参照
param = new S04_0401_DA.TKousyaJininControlUpdate();
//出発日
param.SyuptDay = ParamData.SyuptDay;
//コースコード
param.CrsCd = ParamData.CrsCd;
//号車
param.Gousya = ParamData.Gousya;
//仕入先コード
param.SiireSakiCd = System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.SIIRE_SAKI_CD.ToString()));
//仕入先枝番
param.SiireSakiNo = System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.SIIRE_SAKI_EDABAN.ToString()));
//日次
param.Daily = System.Convert.ToInt32(this.grdSiireInfo.GetData(row, SiireInfoList.DAILY.ToString()));
//通知方法
if (string.Compare(System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())), System.Convert.ToString(NotificationHohoSiireSakiType.Mail.GetHashCode.ToString())) == 0)
{
	param.SendKind = Mail.ToString();
}
else if (string.Compare(System.Convert.ToString(this.grdSiireInfo.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())), System.Convert.ToString(NotificationHohoSiireSakiType.Fax.GetHashCode.ToString())) == 0)
{
	param.SendKind = Fax.ToString();
}
//最終送信日時
param.LastSendDay = CommonDateUtil.getSystemTime();
//システム更新ＰＧＭＩＤ
param.SystemUpdatePgmid = this.Name;
//システム更新者コード
param.SystemUpdatePersonCd = UserInfoManagement.userId;
//システム更新日
param.SystemUpdateDay = CommonDateUtil.getSystemTime();
listParam.Add(param);
}
}
}
}
}
if (checkedCnt == 0)
{
	//確定押下時に選択されているレコードがなかった場合、エラーメッセージを表示する
	CommonProcess.createFactoryMsg().messageDisp("E90_024", MessageE90_024.ToString());
	return null;
}
return listParam;
}

/// <summary>
/// 仕入先情報のデータを取得する
/// </summary>
private List[] getListParamGrdSiireInfoOption()
{
	List listParam = new List(Of S04_0401_DA.TKousyaJininControlUpdate);
	S04_0401_DA.TKousyaJininControlUpdate param = null;
	int checkedCnt = 0;
	for (int row = this.grdSiireInfoOption.Rows.Fixed; row <= this.grdSiireInfoOption.Rows.Count - 1; row++)
	{
		if (this.grdSiireInfoOption.getCheckBoxValue(row, 0))
		{
			checkedCnt++;
			//「出力有無」にチェックが入っているレコードは以下を処理する
			if (System.Convert.ToBoolean(this.grdSiireInfoOption.GetData(row, SiireInfoList.ALLOW_EDIT.ToString())) == true)
			{
				//通知方法が電話となっている場合、ここでは処理を行わない（処理をスキップする）
				if (string.Compare(System.Convert.ToString(this.grdSiireInfoOption.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Tel.ToString()) != 0)
				{
					//通知方法が不要となっている場合、3-2-1をスキップする
					string NotificationHoho = System.Convert.ToString(this.grdSiireInfoOption.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString()).ToString().ToString());
					if (!string.IsNullOrEmpty(NotificationHoho))
					{
						//通知方法がFAXの場合、
						if (string.Compare(System.Convert.ToString(this.grdSiireInfoOption.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())).Trim(), Fax.ToString()) == 0)
						{
							//API仕様未決定のためToDoとして残し、実装は不要
						}
						else
						{
							//API仕様未決定のためToDoとして残し、実装は不要
						}
					}
					//変更行のみパラメータに設定する。
					if (System.Convert.ToInt32(this.grdSiireInfoOption.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())) != int.Parse(dataTableGrdSiireInfoOption.Rows(row - 1).Item(SiireInfoList.NOTIFICATION_HOHO.ToString()).ToString()))
					{
						//パラメータはIF(OUT)シートNo.3参照
						param = new S04_0401_DA.TKousyaJininControlUpdate();
						//出発日
						param.SyuptDay = ParamData.SyuptDay;
						//コースコード
						param.CrsCd = ParamData.CrsCd;
						//号車
						param.Gousya = ParamData.Gousya;
						//仕入先コード
						param.SiireSakiCd = System.Convert.ToString(this.grdSiireInfoOption.GetData(row, SiireInfoList.SIIRE_SAKI_CD.ToString()));
						//仕入先枝番
						param.SiireSakiNo = System.Convert.ToString(this.grdSiireInfoOption.GetData(row, SiireInfoList.SIIRE_SAKI_EDABAN.ToString()));
						//日次
						param.Daily = System.Convert.ToInt32(this.grdSiireInfoOption.GetData(row, SiireInfoList.DAILY.ToString()));
						//通知方法
						if (string.Compare(System.Convert.ToString(this.grdSiireInfoOption.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())), System.Convert.ToString(NotificationHohoSiireSakiType.Mail.GetHashCode.ToString())) == 0)
						{
							param.SendKind = Mail.ToString();
						}
						else if (string.Compare(System.Convert.ToString(this.grdSiireInfoOption.GetData(row, SiireInfoList.NOTIFICATION_HOHO.ToString())), System.Convert.ToString(NotificationHohoSiireSakiType.Fax.GetHashCode.ToString())) == 0)
						{
							param.SendKind = Fax.ToString();
						}
						//最終送信日時
						param.LastSendDay = CommonDateUtil.getSystemTime();
						//システム更新ＰＧＭＩＤ
						param.SystemUpdatePgmid = this.Name;
						//システム更新者コード
						param.SystemUpdatePersonCd = UserInfoManagement.userId;
						//システム更新日
						param.SystemUpdateDay = CommonDateUtil.getSystemTime();
						listParam.Add(param);
					}
				}
			}
		}
	}
	if (checkedCnt == 0)
	{
		//確定押下時に選択されているレコードがなかった場合、エラーメッセージを表示する
		CommonProcess.createFactoryMsg().messageDisp("E90_024", MessageE90_024.ToString());
		return null;
	}
	return listParam;
}
#endregion

#region Privateメソッド(イベント)

#endregion

}