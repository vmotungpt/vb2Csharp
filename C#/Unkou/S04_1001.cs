using System.Text.RegularExpressions;
using Hatobus.ReservationManagementSystem.Master;


/// <summary>
/// 予約者リスト
/// </summary>
public class S04_1001 : PT01, iPT01
{

	#region  定数／変数宣言
	//Private Const regexCheckTime As String = "^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$"
	#endregion

	#region イベント
	/// <summary>
	/// 「コースコード」ラジオボタンの押下
	/// </summary>
	private void rdoCrsCd_CheckedChanged(object sender, EventArgs e)
	{
		settingCrsCdAndCrskbn();
	}

	/// <summary>
	/// 「コース区分」ラジオボタンの押下
	/// </summary>
	private void rdoCrsKbn_CheckedChanged(object sender, EventArgs e)
	{
		settingCrsCdAndCrskbn();
	}

	/// <summary>
	/// 「定期（昼）」ラジオボタンの押下
	/// </summary>
	private void chkTeikiNoon_CheckedChanged(object sender, EventArgs e)
	{
		this.settingItemsOfCrskbn();
	}

	/// <summary>
	/// 「定期（夜）」ラジオボタンの押下
	/// </summary>
	private void chkTeikiNight_CheckedChanged(object sender, EventArgs e)
	{
		this.settingItemsOfCrskbn();
	}

	/// <summary>
	/// 「定期（郊外）」ラジオボタンの押下
	/// </summary>
	private void chkTeikiSuburbs_CheckedChanged(object sender, EventArgs e)
	{
		this.settingItemsOfCrskbn();
	}

	/// <summary>
	/// 「企画（日帰り）」ラジオボタンの押下
	/// </summary>
	private void chkKikakuDayTrip_CheckedChanged(object sender, EventArgs e)
	{
		this.settingItemsOfCrskbn();
	}

	/// <summary>
	/// 「企画（宿泊）」ラジオボタンの押下
	/// </summary>
	private void chkKikakuStay_CheckedChanged(object sender, EventArgs e)
	{
		this.settingItemsOfCrskbn();
	}

	/// <summary>
	/// 「企画（Rコース）」ラジオボタンの押下
	/// </summary>
	private void chkKikakuRCrs_CheckedChanged(object sender, EventArgs e)
	{
		this.settingItemsOfCrskbn();
	}

	/// <summary>
	/// 「キャピタル」ラジオボタンの押下
	/// </summary>
	private void chkCapital_CheckedChanged(object sender, EventArgs e)
	{
		this.settingItemsOfCrskbn();
	}
	#endregion

	#region PT01オーバーライド(基本的には変えない)

	#region 初期化処理
	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	protected override void initFooterButtonControl()
	{
		//Visible
		//TODO:ボタンの表示/非表示を変更
		this.F1Key_Visible = false;
		this.F2Key_Visible = true;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		this.F7Key_Visible = true;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = false;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;

		//Text
		//TODO:ボタンのテキストを変更(必要に応じて)
		this.F2Key_Text = "F2:戻る";
		this.F7Key_Text = "F7:出力";

	}
	#endregion

	#region チェック系
	/// <summary>
	/// 入力項目のチェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	protected override bool checkFormItem()
	{

		//背景色初期化
		base.clearExistErrorProperty(this.gbxOutCondition.Controls);

		//必須項目のチェック
		if (this.isExistHissuError())
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			return false;
		}

		//範囲チェック
		if (this.isExistHaniError())
		{
			if (this.dtmSyuptDayFromTo.ExistErrorForFromDate || this.dtmSyuptDayFromTo.ExistErrorForToDate)
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_066", "２ヶ月前", "１年後", "出発日");
				return false;
			}

			//If Me.dtmSyuptTimeFromTo.ExistErrorForFromTime OrElse Me.dtmSyuptTimeFromTo.ExistErrorForToTime Then
			//    CommonProcess.createFactoryMsg().messageDisp("E90_066", "00：00", "23：59", "出発時間")
			//    Return False
			//End If
		}

		//範囲の大小チェック
		if (this.isExistHaniNoDaisoError())
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_048");
			return false;
		}

		//コード存在チェック
		if (this.isNotExistCode())
		{
			if (this.ucoCrsCd.ExistError)
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_014", "コースコード");
				return false;
			}
			if (this.ucoJyochachiCd.ExistError)
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_014", "乗車地コード");
				return false;
			}
			if (this.ucoAgentCd.ExistError)
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_014", "代理店コード");
				return false;
			}
		}

		//検索結果件数


		return true;
	}
	#endregion

	#region 固有初期処理
	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{

		//ベースフォームの設定
		//TODO:画面IDを設定
		this.setFormId = "S04_1001";
		//TODO:画面名を設定
		this.setTitle = "予約者リスト";

		CommonUtil.Control_Init(this.gbxOutCondition.Controls);

		//背景色初期化
		base.clearExistErrorProperty(this.gbxOutCondition.Controls);

		this.txtGousya.Format = setControlFormat(ControlFormat.HankakuSuji);
		this.txtNinzu.Format = setControlFormat(ControlFormat.HankakuSuji);
		//Me.txtAgent.Format = setControlFormat(ControlFormat.HankakuSuji)

		//初期処理
		this.setSeFirsttDisplayData();

		// フッタボタンの設定
		this.initFooterButtonControl();
		// ロード時にフォーカスを設定する
		this.ActiveControl = this.dtmSyuptDayFromTo;

		// 出発時間FromTo初期化
		this.dtmSyuptTimeFromTo.FromTimeValue24 = null;
		this.dtmSyuptTimeFromTo.ToTimeValue24 = null;
	}
	#endregion

	#endregion

	#region 実装用メソッド(画面毎に変更)

	#region チェック処理(Private)
	/// <summary>
	/// 必須入力項目エラーチェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isExistHissuError()
	{
		bool returnValue = false;
		return System.Convert.ToBoolean(CommonUtil.checkHissuError(this.gbxOutCondition.Controls));
		return returnValue;
	}

	/// <summary>
	/// 範囲チェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isExistHaniError()
	{
		bool returnValue = false;
		DateTime now = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		System.DateTime minDate = now.AddMonths(-2);
		System.DateTime maxDate = now.AddYears(1);

		//出発日
		object with_1 = this.dtmSyuptDayFromTo;
		if (with_1.FromDateText.Value.Date < minDate || with_1.FromDateText.Value.Date > maxDate)
		{
			with_1.ExistErrorForFromDate = true;
			with_1.Focus();
			returnValue = true;
		}

		if (with_1.ToDateText IsNot null && (with_1.ToDateText.Value.Date < minDate || with_1.ToDateText.Value.Date > maxDate))
		{
			with_1.ExistErrorForToDate = true;
			with_1.Focus();
			returnValue = true;
		}

		//'出発時間
		//With Me.dtmSyuptTimeFromTo
		//    If .FromTimeText IsNot Nothing Then
		//        Dim fromTimeArr = .FromTimeText.ToString.Split(CType(":", Char))
		//        Dim fromTime = fromTimeArr(0) & ":" & fromTimeArr(1)
		//        If Regex.IsMatch(fromTime, regexCheckTime) = False Then
		//            .ExistErrorForFromTime = True
		//            .Focus()
		//            returnValue = True
		//        End If
		//    End If

		//    If .ToTimeText IsNot Nothing Then
		//        Dim toTimeArr = .ToTimeText.ToString.Split(CType(":", Char))
		//        Dim toTime = toTimeArr(0) & ":" & toTimeArr(1)
		//        If Regex.IsMatch(toTime, regexCheckTime) = False Then
		//            .ExistErrorForToTime = True
		//            .Focus()
		//            returnValue = True
		//        End If
		//    End If
		//End With

		return returnValue;

	}

	/// <summary>
	/// 範囲の大小チェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isExistHaniNoDaisoError()
	{
		bool returnValue = false;

		//出発日
		object with_1 = this.dtmSyuptDayFromTo;
		if (with_1.FromDateText IsNot null && with_1.ToDateText IsNot null)
		{
			if (CommonDateUtil.chkDayFromTo(with_1.FromDateText.Value.Date, with_1.ToDateText.Value.Date) == false)
			{
				with_1.ExistErrorForFromDate = true;
				with_1.Focus();
				returnValue = true;
			}
		}

		//出発時間
		object with_2 = this.dtmSyuptTimeFromTo;
		if (with_2.FromTimeValue24 IsNot null && with_2.ToTimeValue24 IsNot null)
		{
			if (with_2.FromTimeValue24Int > with_2.ToTimeValue24Int)
			{
				with_2.ExistErrorForFromTime = true;
				with_2.Focus();
				returnValue = true;
			}
		}

		return returnValue;

	}

	/// <summary>
	/// コード存在チェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isNotExistCode()
	{
		bool returnValue = false;

		object with_1 = this.ucoCrsCd;
		if ((!string.IsNullOrEmpty(System.Convert.ToString(with_1.CodeText))) && string.IsNullOrEmpty(System.Convert.ToString(with_1.ValueText)))
		{
			with_1.ExistError = true;
			with_1.Focus();
			returnValue = true;
		}

		object with_2 = this.ucoJyochachiCd;
		if ((!string.IsNullOrEmpty(System.Convert.ToString(with_2.CodeText))) && string.IsNullOrEmpty(System.Convert.ToString(with_2.ValueText)))
		{
			with_2.ExistError = true;
			with_2.Focus();
			returnValue = true;
		}

		object with_3 = this.ucoAgentCd;
		if ((!string.IsNullOrEmpty(System.Convert.ToString(with_3.CodeText))) && string.IsNullOrEmpty(System.Convert.ToString(with_3.ValueText)))
		{
			with_3.ExistError = true;
			with_3.Focus();
			returnValue = true;
		}

		return returnValue;
	}

	#endregion

	/// <summary>
	/// 帳票出力結果
	/// </summary>
	protected override void ResultOutputFormData()
	{
		this.showYoyakuShaDS();
	}

	/// <summary>
	/// F7()ボタン押下時の独自処理
	/// </summary>
	protected override void btnF7_ClickOrgProc()
	{
		// 取得件数チェック
		if (checkFormItem() == false)
		{
			return;
		}
		else
		{
			ResultOutputFormData();
		}
	}
	#endregion

	#region メソッド

	#region Datastudio 呼び出し
	/// <summary>
	/// 取得したデータを元に、帳票を印刷する。
	/// </summary>
	/// <remarks></remarks>
	private void showYoyakuShaDS()
	{
		try
		{
			string PostData = string.Empty;
			string SyuptDayFrom = System.Convert.ToString((System.Convert.ToDateTime(this.dtmSyuptDayFromTo.FromDateText)).ToString("yyyyMMdd"));
			string SyuptDayTo = string.Empty;
			string CrsCd = string.Empty;
			string Gousya = string.Empty;
			string Ninzu = string.Empty;
			string JyochachiCd = string.Empty;
			string SyuptTimeFrom = string.Empty;
			string SyuptTimeTo = string.Empty;
			string Agent = string.Empty;

			PostData += "base_select0=BOS_V_YOYAKU_LIST;SYUPT_DAY&base_opecomp0=>=&base_value0=" + SyuptDayFrom;

			//コースコード入力済の場合
			if (this.rdoCrsCd.Checked == true && !string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)))
			{
				CrsCd = System.Convert.ToString(this.ucoCrsCd.CodeText);
				PostData += "&free_opelogic_0_0=AND&free_select_0_0=BOS_V_YOYAKU_LIST;CRS_CD&free_opecomp_0_0==&free_value_0_0=" + CrsCd;
			}

			if (this.rdoCrsKbn.Checked == true)
			{
				//コース区分選択で「定期（昼）」が選択された場合
				if (this.chkTeikiNoon.Checked == true)
				{
					PostData += "&free_opelogic_1_0=OR&free_select_1_0=BOS_V_YOYAKU_LIST;CRS_KBN_1&free_opecomp_1_0==&free_value_1_0=1";
					PostData += "&free_opelogic_1_1=AND&free_select_1_1=BOS_V_YOYAKU_LIST;CRS_KBN_2&free_opecomp_1_1==&free_value_1_1=1";
				}

				//コース区分選択で「定期（夜）」が選択された場合
				if (this.chkTeikiNight.Checked == true)
				{
					PostData += "&free_opelogic_1_2=OR&free_select_1_2=BOS_V_YOYAKU_LIST;CRS_KBN_1&free_opecomp_1_2==&free_value_1_2=2";
					PostData += "&free_opelogic_1_3=AND&free_select_1_3=BOS_V_YOYAKU_LIST;CRS_KBN_2&free_opecomp_1_3==&free_value_1_3=1";
				}

				//コース区分選択で「定期（郊外）」が選択された場合
				if (this.chkTeikiSuburbs.Checked == true)
				{
					PostData += "&free_opelogic_1_4=OR&free_select_1_4=BOS_V_YOYAKU_LIST;CRS_KBN_2&free_opecomp_1_4==&free_value_1_4=2";
				}

				//コース区分選択で「キャピタル」が選択された場合
				if (this.chkCapital.Checked == true)
				{
					PostData += "&free_opelogic_1_5=OR&free_select_1_5=BOS_V_YOYAKU_LIST;CRS_KIND&free_opecomp_1_5==&free_value_1_5=2";
				}

				//コース区分選択で「企画（日帰り）」が選択された場合
				if (this.chkKikakuDayTrip.Checked == true)
				{
					PostData += "&free_opelogic_1_6=OR&free_select_1_6=BOS_V_YOYAKU_LIST;CRS_KIND&free_opecomp_1_6==&free_value_1_6=4";
				}

				//コース区分選択で「企画（宿泊）」が選択された場合
				if (this.chkKikakuStay.Checked == true)
				{
					PostData += "&free_opelogic_1_7=OR&free_select_1_7=BOS_V_YOYAKU_LIST;CRS_KIND&free_opecomp_1_7==&free_value_1_7=5";
				}

				//コース区分選択で「企画（都内Ｒ）」が選択された場合
				if (this.chkKikakuRCrs.Checked == true)
				{
					PostData += "&free_opelogic_1_8=OR&free_select_1_8=BOS_V_YOYAKU_LIST;CRS_KIND&free_opecomp_1_8==&free_value_1_8=6";
				}
			}

			//検索条件｢号車｣ ≠BLANK の場合
			if (!string.IsNullOrEmpty(System.Convert.ToString(this.txtGousya.Text)))
			{
				Gousya = System.Convert.ToString(this.txtGousya.Text);
				PostData += "&free_opelogic_2_0=AND&free_select_2_0=BOS_V_YOYAKU_LIST;GOUSYA&free_opecomp_2_0==&free_value_2_0=" + Gousya;
			}

			//検索条件｢人数｣ ≠BLANK の場合
			if (!string.IsNullOrEmpty(System.Convert.ToString(this.txtNinzu.Text)))
			{
				Ninzu = System.Convert.ToString(this.txtNinzu.Text);
				PostData += "&free_opelogic_3_0=AND&free_select_3_0=BOS_V_YOYAKU_LIST;CHARGE_APPLICATION_NINZU&free_opecomp_3_0=>=&free_value_3_0=" + Ninzu;
			}

			//検索条件｢乗車地｣ ≠BLANK の場合
			if (!string.IsNullOrEmpty(System.Convert.ToString(this.ucoJyochachiCd.CodeText)))
			{
				JyochachiCd = Strings.Trim(System.Convert.ToString(this.ucoJyochachiCd.CodeText));
				PostData += "&free_opelogic_4_0=AND&free_select_4_0=BOS_V_YOYAKU_LIST;(JYOCHACHI_CD_1&free_opecomp_4_0==&free_value_4_0=" + JyochachiCd;
				PostData += "&free_opelogic_4_1=OR&free_select_4_1=BOS_V_YOYAKU_LIST;JYOCHACHI_CD_2&free_opecomp_4_1==&free_value_4_1=" + JyochachiCd;
				PostData += "&free_opelogic_4_2=OR&free_select_4_2=BOS_V_YOYAKU_LIST;JYOCHACHI_CD_3&free_opecomp_4_2==&free_value_4_2=" + JyochachiCd;
				PostData += "&free_opelogic_4_3=OR&free_select_4_3=BOS_V_YOYAKU_LIST;JYOCHACHI_CD_4&free_opecomp_4_3==&free_value_4_3=" + JyochachiCd;
				PostData += "&free_opelogic_4_4=OR&free_select_4_4=BOS_V_YOYAKU_LIST;JYOCHACHI_CD_5&free_opecomp_4_4==&free_value_4_4=" + JyochachiCd + ")";
			}

			//検索条件｢出発時刻｣From ≠BLANK の場合
			if (this.dtmSyuptTimeFromTo.FromTimeValue24 IsNot null)
			{
				SyuptTimeFrom = System.Convert.ToString(this.dtmSyuptTimeFromTo.FromTimeValue24Int.ToString());
				PostData += "&free_opelogic_5_0=AND&free_select_5_0=BOS_V_YOYAKU_LIST;TIME_FROM&free_opecomp_5_0=>=&free_value_5_0=" + SyuptTimeFrom;
			}

			//検索条件｢出発時刻｣To ≠BLANK の場合
			if (this.dtmSyuptTimeFromTo.ToTimeValue24 IsNot null)
			{
				SyuptTimeTo = System.Convert.ToString(this.dtmSyuptTimeFromTo.ToTimeValue24Int.ToString());
				PostData += "&free_opelogic_5_1=AND&free_select_5_1=BOS_V_YOYAKU_LIST;TIME_TO&free_opecomp_5_1=<=&free_value_5_1=" + SyuptTimeTo;
			}

			//検索条件｢予約取消区分」の「取消前のみ」 = ON の場合
			if (this.chkCancelBFOnly.Checked == true)
			{
				PostData += "&free_opelogic_6_0=AND&free_select_6_0=BOS_V_YOYAKU_LIST;CANCEL_FLG&free_opecomp_6_0==&free_value_6_0=%28P04_1004%2eCANCEL_FLG%3c%3e%271%27%20or%20P04_1004%2eCANCEL_FLG%20is%20null%29";
			}

			//検索条件｢予約取消区分」の「取消済のみ」 = ON の場合
			if (this.chkCancelAlreadyOnly.Checked == true)
			{
				PostData += "&free_opelogic_6_0=AND&free_select_6_0=BOS_V_YOYAKU_LIST;CANCEL_FLG&free_opecomp_6_0==&free_value_6_0=P04_1004%2eCANCEL_FLG%3d%271%27";
			}

			//検索条件｢業者｣ ≠BLANK の場合
			if (!string.IsNullOrEmpty(System.Convert.ToString(this.ucoAgentCd.CodeText)))
			{
				Agent = Strings.Trim(System.Convert.ToString(this.ucoAgentCd.CodeText));
				PostData += "&free_opelogic_7_0=AND&free_select_7_0=BOS_V_YOYAKU_LIST;AGENT_CD&free_opecomp_7_0==&free_value_7_0=" + Agent;
			}

			//検索条件｢発券済｣ が OFF の場合
			if (this.chkHakkenAlready.Checked == false)
			{
				PostData += "&free_opelogic_8_0=AND&free_select_8_0=BOS_V_YOYAKU_LIST;HAKKEN_NAIYO&free_opecomp_8_0==&free_value_8_0=%28%28P04_1004%2eHAKKEN_NAIYO%3c%3e%271%27%20and%20P04_1004%2eHAKKEN_NAIYO%3c%3e%273%27%29%20or%20P04_1004%2eHAKKEN_NAIYO%20is%20null%29";
			}

			//検索条件｢NO SHOWのみ｣ が ON の場合
			if (this.chkchkNoShowOnly.Checked == true)
			{
				PostData += "&free_opelogic_9_0=AND&free_select_9_0=BOS_V_YOYAKU_LIST;NO_SHOW_FLG&free_opecomp_9_0==&free_value_9_0=1";
			}

			if (this.dtmSyuptDayFromTo.ToDateText IsNot null)
			{
				SyuptDayTo = System.Convert.ToString((System.Convert.ToDateTime(this.dtmSyuptDayFromTo.ToDateText)).ToString("yyyyMMdd"));
				PostData += "&free_opelogic_10_0=AND&free_select_10_0=BOS_V_YOYAKU_LIST;SYUPT_DAY&free_opecomp_10_0=<=&free_value_10_0=" + SyuptDayTo;
			}
			else
			{
				PostData += "&free_opelogic_10_0=AND&free_select_10_0=BOS_V_YOYAKU_LIST;SYUPT_DAY&free_opecomp_10_0=<=&free_value_10_0=" + SyuptDayFrom;
			}

			//PostData &= "&alldisp_enable=OFF&data_btn_enable=OFF"

			ReadAppConfig reaAppConfig = new ReadAppConfig();
			CommonProcess.DataStudioId = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioId);
			CommonProcess.DataStudioPassword = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioPassword);
			BOCommon.showDataStudio(this.Name, BOCommon.SystemSetCd.dsid_YoyakuShaRisuto, PostData);

			//ログメッセージ
			string logmsg = string.Empty;
			logmsg += "USERID=" + CommonProcess.DataStudioId;
			logmsg += "&PASSWORD=" + CommonProcess.DataStudioPassword;
			logmsg += logmsg + PostData;
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.DSCall, this.setTitle, logmsg);
		}
		catch (Exception ex)
		{
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.DSCall, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
			throw;
		}
	}

	#endregion

	/// <summary>
	/// チェックされたコース区分に応じて
	/// コース区分チェックボックスの使用可・不可を
	/// 切り替える。とする
	/// </summary>
	private void settingCrsCdAndCrskbn()
	{
		if (this.rdoCrsCd.Checked == true)
		{
			//コースコードを活性
			this.ucoCrsCd.Enabled = true;

			//コース区分のチェックボックスを非活性
			this.chkTeikiNoon.Enabled = false;
			this.chkTeikiNight.Enabled = false;
			this.chkTeikiSuburbs.Enabled = false;
			this.chkKikakuDayTrip.Enabled = false;
			this.chkKikakuStay.Enabled = false;
			this.chkKikakuRCrs.Enabled = false;
			this.chkCapital.Enabled = false;
		}

		if (this.rdoCrsKbn.Checked == true)
		{
			//コースコードを非活性
			this.ucoCrsCd.Enabled = false;

			//コース区分のチェックボックスを活性
			this.chkTeikiNoon.Enabled = true;
			this.chkTeikiNight.Enabled = true;
			this.chkTeikiSuburbs.Enabled = true;
			this.chkKikakuDayTrip.Enabled = true;
			this.chkKikakuStay.Enabled = true;
			this.chkKikakuRCrs.Enabled = true;
			this.chkCapital.Enabled = true;
		}
	}

	/// <summary>
	/// 「定期（昼）」ラジオボタンの押下
	/// </summary>
	private void settingItemsOfCrskbn()
	{

		//すべて活性状態とする。
		this.chkTeikiNoon.Enabled = true;
		this.chkTeikiNight.Enabled = true;
		this.chkTeikiSuburbs.Enabled = true;
		this.chkKikakuDayTrip.Enabled = true;
		this.chkKikakuStay.Enabled = true;
		this.chkKikakuRCrs.Enabled = true;
		this.chkCapital.Enabled = true;

		//定期（昼）
		if (this.chkTeikiNoon.Checked == true)
		{
			//企画（日帰り）を非活性
			this.chkKikakuDayTrip.Enabled = false;
			//企画（宿泊）を非活性
			this.chkKikakuStay.Enabled = false;
			//キャピタルを非活性
			this.chkCapital.Enabled = false;
		}

		//定期（夜）
		if (this.chkTeikiNight.Checked == true)
		{
			//企画（日帰り）を非活性
			this.chkKikakuDayTrip.Enabled = false;
			//企画（宿泊）を非活性
			this.chkKikakuStay.Enabled = false;
			//キャピタルを非活性
			this.chkCapital.Enabled = false;
		}

		//定期（郊外）
		if (this.chkTeikiSuburbs.Checked == true)
		{
			//企画（日帰り）を非活性
			this.chkKikakuDayTrip.Enabled = false;
			//企画（宿泊）を非活性
			this.chkKikakuStay.Enabled = false;
			//キャピタルを非活性
			this.chkCapital.Enabled = false;
		}

		//企画（日帰り）
		if (this.chkKikakuDayTrip.Checked == true)
		{
			//定期（昼）を非活性
			this.chkTeikiNoon.Enabled = false;
			//定期（夜）を非活性
			this.chkTeikiNight.Enabled = false;
			//定期（夜）を非活性
			this.chkTeikiSuburbs.Enabled = false;
			//企画（Ｒコース）を非活性
			this.chkKikakuRCrs.Enabled = false;
			//キャピタルを非活性
			this.chkCapital.Enabled = false;
		}

		//企画（宿泊）
		if (this.chkKikakuStay.Checked == true)
		{
			//定期（昼）を非活性
			this.chkTeikiNoon.Enabled = false;
			//定期（夜）を非活性
			this.chkTeikiNight.Enabled = false;
			//定期（夜）を非活性
			this.chkTeikiSuburbs.Enabled = false;
			//企画（Ｒコース）を非活性
			this.chkKikakuRCrs.Enabled = false;
			//キャピタルを非活性
			this.chkCapital.Enabled = false;
		}

		//企画（Ｒコース）
		if (this.chkKikakuRCrs.Checked == true)
		{
			//企画（日帰り）を非活性
			this.chkKikakuDayTrip.Enabled = false;
			//企画（宿泊）を非活性
			this.chkKikakuStay.Enabled = false;
			//キャピタルを非活性
			this.chkCapital.Enabled = false;
		}

		//キャピタル
		if (this.chkCapital.Checked == true)
		{
			//定期（昼）を非活性
			this.chkTeikiNoon.Enabled = false;
			//定期（夜）を非活性
			this.chkTeikiNight.Enabled = false;
			//定期（夜）を非活性
			this.chkTeikiSuburbs.Enabled = false;
			//企画（日帰り）を非活性
			this.chkKikakuDayTrip.Enabled = false;
			//企画（宿泊）を非活性
			this.chkKikakuStay.Enabled = false;
			//企画（Ｒコース）を非活性
			this.chkKikakuRCrs.Enabled = false;
		}

	}
	#endregion

	#region 実装用メソッド(画面毎に変更)
	/// <summary>
	/// 初期処理(必須画面個別実装)
	/// </summary>
	public void setSeFirsttDisplayData()
	{
		//F7:印刷の設定
		//Me.F7Key_Enabled = False

		//コースコードを非活性
		this.ucoCrsCd.Enabled = true;

		//コース区分のチェックボックスを非活性
		this.chkTeikiNoon.Enabled = false;
		this.chkTeikiNight.Enabled = false;
		this.chkTeikiSuburbs.Enabled = false;
		this.chkKikakuDayTrip.Enabled = false;
		this.chkKikakuStay.Enabled = false;
		this.chkKikakuRCrs.Enabled = false;
		this.chkCapital.Enabled = false;

		//「取消前のみ」デフォルトON
		this.chkCancelBFOnly.Checked = true;
	}

	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent"></param>
	public void DisplayDataToEntity(ref object ent)
	{

	}

	public bool CheckSearch()
	{
		return true;
	}
	#endregion

}