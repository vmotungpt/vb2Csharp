using Hatobus.ReservationManagementSystem.Master;
using System.Text.RegularExpressions;


/// <summary>
/// 人員表／日別運行表画面
/// </summary>
public class S04_1002 : PT01, iPT01
{

	#region  定数／変数宣言
	///<summary>
	///時間チェック用の正規表現
	///</summary>
	//Private Const regexCheckTime As String = "^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$"
	#endregion

	#region イベント

	#endregion

	#region PT01オーバーライド(基本的には変えない)

	#region 初期化処理
	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	protected override void initFooterButtonControl()
	{
		//Visible
		//ボタンの表示/非表示を変更
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
		//ボタンのテキストを変更(必要に応じて)
		this.F2Key_Text = "F2:戻る";
		this.F7Key_Text = "F7:出力";

	}
	#endregion

	#region 固有初期処理
	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{

		//ベースフォームの設定
		//画面IDを設定
		this.setFormId = "S04_1002";
		//画面名を設定
		this.setTitle = "人員表／日別運行表";

		CommonUtil.Control_Init(this.gbxOutJoken.Controls);

		//背景色初期化
		base.clearExistErrorProperty(this.gbxOutJoken.Controls);

		//初期処理
		this.setSeFirsttDisplayData();

		//フッタボタンの設定
		this.initFooterButtonControl();

		//所属部署が国際事業部の場合、外国語にチェック
		if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
		{
			this.chkCrsKind3Gaikokugo.Checked = true; // 外国語のみチェック
		}
		else
		{
			this.chkCrsKind3Nihongo.Checked = true; // 日本語コースのみチェック
		}

		// ロード時にフォーカスを設定する
		this.ActiveControl = this.chkTeikiJininTable;
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
		base.clearExistErrorProperty(this.gbxOutJoken.Controls);
		return this.CheckSearch();
	}
	#endregion

	#endregion

	#region メソッド
	/// <summary>
	///コンボボックスの設定
	/// </summary>
	private void SetComboBoxControlInitialize()
	{

		//DataAccessクラス生成
		EigyosyoMasterGet_DA dataAccess = new EigyosyoMasterGet_DA();

		//日別運行表のコンボボックスに値をセット
		CommonMstUtil.setComboCommonProperty(this.cmbDayBetuUnkouTable, CommonProcessMst.getComboboxDataOfDatatable(typeof(TeikiKikakuKbn), true));

		//取扱部署のコンボボックスに値をセット
		CommonMstUtil.setComboBox(CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.toriatsukai_busho), this.cmbToriatukaiBusyo, true, null);
	}

	/// <summary>
	/// 帳票出力結果
	/// </summary>
	protected override void ResultOutputFormData()
	{
		//人員表
		if (this.chkTeikiJininTable.Checked == true)
		{
			this.showPersonalChart();
		}

		//日別運行表
		if (this.chkDayBetuUnkouTable.Checked == true)
		{
			this.showDailyTravelTable();
		}

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

	#region チェック処理(Private)
	/// <summary>
	/// 必須入力項目エラーチェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isExistHissuError()
	{
		bool returnValue = false;

		//選択
		if (this.chkTeikiJininTable.Checked == false && this.chkDayBetuUnkouTable.Checked == false)
		{
			object with_1 = this.chkTeikiJininTable;
			with_1.ExistError = true;
			with_1.Focus();

			object with_2 = this.chkDayBetuUnkouTable;
			with_2.ExistError = true;
			with_2.Focus();

			returnValue = true;
		}

		//出発日
		if (CommonUtil.checkHissuError(this.gbxOutJoken.Controls) == true)
		{
			returnValue = true;
		}

		//コース種別
		if (this.chkCrsKind3Nihongo.Checked == false && this.chkCrsKind3Gaikokugo.Checked == false)
		{
			object with_3 = this.chkCrsKind3Nihongo;
			with_3.ExistError = true;
			with_3.Focus();

			object with_4 = this.chkCrsKind3Gaikokugo;
			with_4.ExistError = true;
			with_4.Focus();

			return true;
		}

		return returnValue;

	}

	///<summary>
	///範囲チェック
	///</summary>
	///<returns>True:エラー無 False:エラー有</returns>
	///<remarks></remarks>
	//Private Function isExistHaniError() As Boolean
	//    Dim returnValue As Boolean = False

	//    '出発時間
	//    With Me.dtmSyuptTime
	//        If .Value IsNot Nothing Then
	//            If Regex.IsMatch(.Text, regexCheckTime) = False Then
	//                .ExistError = True
	//                .Focus()
	//                returnValue = True
	//            End If
	//        End If
	//    End With

	//    Return returnValue

	//End Function

	/// <summary>
	/// コード存在チェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isNotExistCode()
	{
		bool returnValue = false;

		object with_1 = this.ucoJyochachiCd;
		if ((!string.IsNullOrEmpty(System.Convert.ToString(with_1.CodeText))) && string.IsNullOrEmpty(System.Convert.ToString(with_1.ValueText)))
		{
			with_1.ExistError = true;
			with_1.Focus();
			returnValue = true;
		}

		return returnValue;
	}
	#endregion

	#region Datastudio 呼び出し
	/// <summary>
	/// 取得したデータを元に、帳票を印刷する。
	/// </summary>
	/// <remarks></remarks>
	private void showDailyTravelTable()
	{
		try
		{
			string PostData = string.Empty;
			string SyuptDay = System.Convert.ToString(System.Convert.ToDateTime(this.dtmSyuptDay.Value).ToString("yyyyMMdd"));
			string DayBetuUnkouTable = string.Empty;
			string yochachiCd = string.Empty;
			string SyuptTime = string.Empty;
			string ToriatukaiBusyo = string.Empty;

			PostData += "&base_select0=BOS_V_UNKOU_LIST;SYUPT_DAY&base_opecomp0==&base_value0=" + SyuptDay;

			if (this.chkDayBetuUnkouTable.Checked == true && this.cmbDayBetuUnkouTable.SelectedIndex != 0)
			{
				DayBetuUnkouTable = System.Convert.ToString(this.cmbDayBetuUnkouTable.SelectedValue);
				PostData += "&free_opelogic_0_0=AND&free_select_0_0=BOS_V_UNKOU_LIST;TEIKI_KIKAKU_KBN&free_opecomp_0_0==&free_value_0_0=" + DayBetuUnkouTable;
			}

			if (this.chkCrsKind3Nihongo.Checked == true && this.chkCrsKind3Gaikokugo.Checked == false)
			{
				PostData += "&free_opelogic_1_0=AND&free_select_1_0=BOS_V_UNKOU_LIST;HOUJIN_GAIKYAKU_KBN&free_opecomp_1_0==&free_value_1_0=H";
			}

			if (this.chkCrsKind3Nihongo.Checked == false && this.chkCrsKind3Gaikokugo.Checked == true)
			{
				PostData += "&free_opelogic_1_0=AND&free_select_1_0=BOS_V_UNKOU_LIST;HOUJIN_GAIKYAKU_KBN&free_opecomp_1_0==&free_value_1_0=G";
			}

			if (this.chkCrsKind3Nihongo.Checked == true && this.chkCrsKind3Gaikokugo.Checked == true)
			{
				PostData += "&free_opelogic_1_0=AND&free_select_1_0=BOS_V_UNKOU_LIST;HOUJIN_GAIKYAKU_KBN&free_opecomp_1_0=in($VALS$)&free_value_1_0=H G";
			}

			if (!string.IsNullOrEmpty(System.Convert.ToString(this.ucoJyochachiCd.CodeText)))
			{
				yochachiCd = System.Convert.ToString(this.ucoJyochachiCd.CodeText);
				PostData += "&free_opelogic_2_0=AND&free_select_2_0=BOS_V_UNKOU_LIST;JYOCHACHI_CD&free_opecomp_2_0==&free_value_2_0=" + yochachiCd;
			}

			if (this.dtmSyuptTime.Value24 IsNot null)
			{
				SyuptTime = System.Convert.ToString(this.dtmSyuptTime.Value24Int.ToString());
				PostData += "&free_opelogic_3_0=AND&free_select_3_0=BOS_V_UNKOU_LIST;SYUPT_TIME&free_opecomp_3_0=>=&free_value_3_0=" + SyuptTime;
			}

			if (this.cmbToriatukaiBusyo.SelectedIndex != 0)
			{
				ToriatukaiBusyo = System.Convert.ToString(this.cmbToriatukaiBusyo.SelectedValue);
				PostData += "&free_opelogic_4_0=AND&free_select_4_0=BOS_V_UNKOU_LIST;MANAGEMENT_SEC&free_opecomp_4_0==&free_value_4_0=" + ToriatukaiBusyo;
			}

			//PostData &= "&alldisp_enable=OFF&data_btn_enable=OFF"

			ReadAppConfig reaAppConfig = new ReadAppConfig();
			CommonProcess.DataStudioId = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioId);
			CommonProcess.DataStudioPassword = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioPassword);
			BOCommon.showDataStudio(this.Name, BOCommon.SystemSetCd.dsid_NiTsuBetsuUnkoHyo, PostData);

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

	/// <summary>
	/// 取得したデータを元に、帳票を印刷する。
	/// </summary>
	/// <remarks></remarks>
	private void showPersonalChart()
	{
		try
		{
			string PostData = string.Empty;
			string SyuptDay = System.Convert.ToString(System.Convert.ToDateTime(this.dtmSyuptDay.Value).ToString("yyyyMMdd"));
			string DayBetuUnkouTable = string.Empty;
			string yochachiCd = string.Empty;
			string SyuptTime = string.Empty;
			string ToriatukaiBusyo = string.Empty;

			PostData += "base_select0=BOS_V_JININ_LIST;SYUPT_DAY&base_opecomp0==&base_value0=" + SyuptDay;

			if (this.chkCrsKind3Nihongo.Checked == true && this.chkCrsKind3Gaikokugo.Checked == false)
			{
				PostData += "&free_opelogic_0_0=AND&free_select_0_0=BOS_V_JININ_LIST;HOUJIN_GAIKYAKU_KBN&free_opecomp_0_0==&free_value_0_0=H";
			}

			if (this.chkCrsKind3Nihongo.Checked == false && this.chkCrsKind3Gaikokugo.Checked == true)
			{
				PostData += "&free_opelogic_0_0=AND&free_select_0_0=BOS_V_JININ_LIST;HOUJIN_GAIKYAKU_KBN&free_opecomp_0_0==&free_value_0_0=G";
			}

			if (this.chkCrsKind3Nihongo.Checked == true && this.chkCrsKind3Gaikokugo.Checked == true)
			{
				PostData += "&free_opelogic_0_0=AND&free_select_0_0=BOS_V_JININ_LIST;HOUJIN_GAIKYAKU_KBN&free_opecomp_0_0=in($VALS$)&free_value_0_0=H G";
			}

			if (!string.IsNullOrEmpty(System.Convert.ToString(this.ucoJyochachiCd.CodeText)))
			{
				yochachiCd = System.Convert.ToString(this.ucoJyochachiCd.CodeText);
				// 配車経由地
				PostData += "&free_opelogic_1_0=AND&free_select_1_0=BOS_V_JININ_LIST;HAISYA_KEIYU_CD&free_opecomp_1_0==&free_value_1_0=" + yochachiCd;
			}

			if (this.dtmSyuptTime.Value24 IsNot null)
			{
				SyuptTime = System.Convert.ToString(this.dtmSyuptTime.Value24Int.ToString());
				// 配車時間
				PostData += "&free_opelogic_2_0=AND&free_select_2_0=BOS_V_JININ_LIST;HAISYA_TIME&free_opecomp_2_0=>=&free_value_2_0=" + SyuptTime;
			}

			if (this.cmbToriatukaiBusyo.SelectedIndex != 0)
			{
				ToriatukaiBusyo = System.Convert.ToString(this.cmbToriatukaiBusyo.SelectedValue);
				PostData += "&free_opelogic_3_0=AND&free_select_3_0=BOS_V_JININ_LIST;MANAGEMENT_SEC&free_opecomp_3_0==&free_value_3_0=" + ToriatukaiBusyo;
			}

			//PostData &= "&alldisp_enable=OFF&data_btn_enable=OFF"

			ReadAppConfig reaAppConfig = new ReadAppConfig();
			CommonProcess.DataStudioId = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioId);
			CommonProcess.DataStudioPassword = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioPassword);
			BOCommon.showDataStudio(this.Name, BOCommon.SystemSetCd.dsid_PersonnelChart, PostData);

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

	#endregion

	#region 実装用メソッド(画面毎に変更)

	#region 初期処理
	/// <summary>
	/// 初期処理(必須画面個別実装)
	/// </summary>
	public void setSeFirsttDisplayData()
	{
		//コンボボックスの設定
		this.SetComboBoxControlInitialize();
	}
	#endregion

	#region エンティティ操作系
	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent"></param>
	public void DisplayDataToEntity(ref object ent)
	{

	}
	#endregion

	#region チェック系
	/// <summary>
	/// 入力項目のチェック
	/// </summary>
	/// <returns></returns>
	public bool CheckSearch()
	{
		//必須項目のチェック
		if (this.isExistHissuError())
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			return false;
		}

		//'範囲チェック
		//If Me.isExistHaniError Then
		//    CommonProcess.createFactoryMsg().messageDisp("E90_066", "00：00", "23：59", "出発時間")
		//    Return False
		//End If

		//コード存在チェック
		if (this.isNotExistCode())
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_014", "乗車地コード");
			return false;
		}

		//検索結果件数

		return true;
	}
	#endregion

	#endregion

}