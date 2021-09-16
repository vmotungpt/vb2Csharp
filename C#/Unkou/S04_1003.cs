/// <summary>
/// 乗客名簿
/// </summary>
public class S04_1003 : PT01
{

	#region  定数／変数宣言

	#region 変数
	public OracleConnection _oracleConnection = new OracleConnection(); // Oracle Connection
	#endregion

	#region 定数
	// チェックエラー表示項目
	private const string ErrorDisplaySyuptDay = "出発日";
	private const string ErrorDisplaySyuptTime = "出発時間";

	// メッセージ表示項目
	private const string MsgDisplayPrint = "印刷";
	#endregion

	#region 列挙

	#endregion

	#endregion

	#region プロパティ

	#endregion

	#region フォーム

	#endregion

	#region メソッド

	#region 固有初期処理
	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{

		// 検索条件部の項目初期化
		initSearchAreaItems();

		// PT01 フッタボタンの制御
		initFooterButtonControl();

		//非使用ボタン設定
		this.F4Key_Visible = false;
		this.F6Key_Visible = false;
		this.F8Key_Visible = false;

	}

	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{

		// 項目の初期値設定
		this.dtmSyuptDay.Value = CommonDateUtil.getSystemTime(); // 出発日
		this.ucoNoribaCd.setControlInitiarize(); // 乗車地コード
		this.ucoCrsCd.setControlInitiarize(); // コースコード

		this.rdoTeikiNoon.Checked = true; // 定期（昼）
		this.rdoTeikiNight.Checked = false; // 定期（夜）
		this.rdoTeikiKogai.Checked = false; // 定期（郊外）
		this.rdoCapital.Checked = false; // キャピタル
		this.rdoHigaeri.Checked = false; // 企画（日帰り）
		this.rdoStay.Checked = false; // 企画（宿泊）
		this.rdoRCrs.Checked = false; // 企画（都内R）

		this.dtmSyuptTimeFromTo.FromTimeValue24 = null; // 出発時間From
		this.dtmSyuptTimeFromTo.ToTimeValue24 = null; // 出発時間To
		this.txtGousya.Text = null; // 号車

		this.rdoPassengerMeibo.Checked = true; // 帳票選択 乗客名簿
		this.rdoOptionYoyakuMeisai.Checked = false; // 帳票選択 オプション予約明細表

		this.chkCrsKindJapanese.Checked = false; // 日本語
		this.chkCrsKindGaikokugo.Checked = false; // 外国語

		// コントロールの書式設定
		txtGousya.Format = setControlFormat(ControlFormat.HankakuSuji); // 号車

		// 初期フォーカスのコントロールを設定を実装
		this.dtmSyuptDay.Select();

	}

	#endregion

	#region チェック処理

	/// <summary>
	/// 検索条件項目チェック
	/// </summary>
	protected override bool checkFormItem()
	{

		//フォーカスセットフラグ
		bool focusSetFlg = false;
		//エラー表示項目
		string errorDisplay = string.Empty;

		//エラーの初期化
		base.clearExistErrorProperty(this.gbxOutJoken.Controls);

		//
		//必須チェック
		//

		if (CommonUtil.checkHissuError(this.gbxOutJoken.Controls) == true)
		{

			//必須エラーフォーカス設定（エラーが発生した先頭にフォーカスを当てる）
			if (dtmSyuptDay.ExistError == true)
			{
				focusSetFlg = true;
				dtmSyuptDay.Focus();
			}

			CommonProcess.createFactoryMsg().messageDisp("E90_022"); // 共通メッセージ処理 [E90_022 未入力項目があります。]

			return false;
		}

		//
		// 相関チェック
		//

		//出発時間の大小チェック
		if (ReferenceEquals(this.dtmSyuptTimeFromTo.FromTimeValue24, null) == false && ReferenceEquals(this.dtmSyuptTimeFromTo.ToTimeValue24, null) == false)
		{
			if (dtmSyuptTimeFromTo.FromTimeValue24Int > dtmSyuptTimeFromTo.ToTimeValue24Int)
			{
				this.dtmSyuptTimeFromTo.ExistErrorForFromTime = true;
				this.dtmSyuptTimeFromTo.ExistErrorForToTime = true;
				this.dtmSyuptTimeFromTo.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_017", ErrorDisplaySyuptTime); // 共通メッセージ処理 [E90_017 {1}の設定が不正です。]

				return false;
			}
		}

		return true;

	}

	#endregion

	#region DB関連
	/// <summary>
	/// 対象マスタのデータ取得
	/// </summary>
	protected override DataTable getFormData()
	{

		//オプション予約明細表が選択されている場合、ダミーテーブルを返却する
		if (this.rdoOptionYoyakuMeisai.Checked)
		{
			//DataTable作成(ダミー)
			DataTable dmyTable = new DataTable();
			dmyTable.Columns.Add("DUMMY");

			//新規行追加
			DataRow dmyRow = dmyTable.NewRow();
			dmyRow["DUMMY"] = "";
			dmyTable.Rows.Add(dmyRow);

			//返却
			return dmyTable;
		}

		//DataAccessクラス生成
		Passengerlist_DA dataAccess = new Passengerlist_DA();

		DataTable returnValue = new DataTable();

		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		// 出発日
		paramInfoList.Add("SyuptDay", Strings.Replace(System.Convert.ToString(this.dtmSyuptDay.Value), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));

		// 乗車地コード
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.ucoNoribaCd.CodeText)))
		{
			paramInfoList.Add("HaisyaKeiyuCd", System.Convert.ToString(this.ucoNoribaCd.CodeText));
		}

		// コースコード
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)))
		{
			paramInfoList.Add("CrsCd", System.Convert.ToString(this.ucoCrsCd.CodeText));
		}

		// 定期（昼） ラジオボタン
		if (this.rdoTeikiNoon.Checked == true)
		{
			paramInfoList.Add("RdoTeikiNoon", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("RdoTeikiNoon", FixedCd.CheckBoxValue.OffValue);
		}

		// 定期（夜） ラジオボタン
		if (this.rdoTeikiNight.Checked == true)
		{
			paramInfoList.Add("RdoTeikiNight", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("RdoTeikiNight", FixedCd.CheckBoxValue.OffValue);
		}

		// 定期（郊外） ラジオボタン
		if (this.rdoTeikiKogai.Checked == true)
		{
			paramInfoList.Add("RdoTeikiKogai", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("RdoTeikiKogai", FixedCd.CheckBoxValue.OffValue);
		}

		// キャピタル ラジオボタン
		if (this.rdoCapital.Checked == true)
		{
			paramInfoList.Add("RdoCapital", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("RdoCapital", FixedCd.CheckBoxValue.OffValue);
		}

		// 企画（日帰り） ラジオボタン
		if (this.rdoHigaeri.Checked == true)
		{
			paramInfoList.Add("RdoHigaeri", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("RdoHigaeri", FixedCd.CheckBoxValue.OffValue);
		}

		// 企画（宿泊） ラジオボタン
		if (this.rdoStay.Checked == true)
		{
			paramInfoList.Add("RdoStay", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("RdoStay", FixedCd.CheckBoxValue.OffValue);
		}

		// 企画（都内R） ラジオボタン
		if (this.rdoRCrs.Checked == true)
		{
			paramInfoList.Add("RdoRCrs", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("RdoRCrs", FixedCd.CheckBoxValue.OffValue);
		}

		// 出発時間From
		if (!(ReferenceEquals(this.dtmSyuptTimeFromTo.FromTimeValue24, null)))
		{
			paramInfoList.Add("SyuptTimeFr", dtmSyuptTimeFromTo.FromTimeValue24Int);
		}

		// 出発時間To
		if (!(ReferenceEquals(this.dtmSyuptTimeFromTo.ToTimeValue24, null)))
		{
			paramInfoList.Add("SyuptTimeTo", dtmSyuptTimeFromTo.ToTimeValue24Int);
		}

		// 号車
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.txtGousya.Text)))
		{
			paramInfoList.Add("Gousya", System.Convert.ToInt32(this.txtGousya.Text));
		}

		// コース種別：日本語 チェックボックス
		if (this.chkCrsKindJapanese.Checked == true)
		{
			paramInfoList.Add("ChkCrsKindJapanese", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("ChkCrsKindJapanese", FixedCd.CheckBoxValue.OffValue);
		}

		// コース種別：外国語 チェックボックス
		if (this.chkCrsKindGaikokugo.Checked == true)
		{
			paramInfoList.Add("ChkCrsKindGaikokugo", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("ChkCrsKindGaikokugo", FixedCd.CheckBoxValue.OffValue);
		}

		//コネクション取得
		_oracleConnection = dataAccess.getDetailData();

		return dataAccess.getQueryAsYoyakuMain(paramInfoList, _oracleConnection);

	}

	#endregion

	#region 帳票関連
	/// <summary>
	/// 帳票出力処理
	/// </summary>
	protected override void ResultOutputFormData()
	{

		//「乗客名簿」ラジオボタン選択時
		if (rdoPassengerMeibo.Checked)
		{
			//「乗客名簿」出力処理
			printPassengerMeibo();
		}
		else if (rdoOptionYoyakuMeisai.Checked)
		{
			//「オプション予約明細表」出力処理
			printOptionYoyakuMeisai();
		}

	}
	#endregion

	#endregion

	#region イベント
	/// <summary>
	/// 「オプション予約明細表」ラジオボタンの押下
	/// </summary>
	private void rdoOptionYoyakuMeisai_CheckedChanged(object sender, EventArgs e)
	{
		if (this.rdoOptionYoyakuMeisai.Checked)
		{
			this.ucoNoribaCd.Enabled = false; // 乗車地 非活性
			this.ucoNoribaCd.CodeText = ""; // 乗車地コード 空
			this.ucoNoribaCd.ValueText = ""; // 乗車地名称　 空
			base.F7Key_Text = "F7:出力"; // F7ボタン名称 "F7:出力"
		}
		else
		{
			this.ucoNoribaCd.Enabled = true; // 乗車地 活性
			base.F7Key_Text = "F7:印刷"; // F7ボタン名称 "F7:印刷"
		}
	}
	#endregion

	#region Private メソッド
	/// <summary>
	/// 「乗客名簿」出力処理
	/// </summary>
	private void printPassengerMeibo()
	{

		// データなしの場合は帳票出力はしない
		if (SeachResultFormData.Rows.Count == 0)
		{
			return;
		}

		// 乗客名簿
		Passengerlist_Form_New Passengerlist = new Passengerlist_Form_New();

		try
		{
			// コネクション渡し
			Passengerlist._oracleConnection = _oracleConnection;

			Passengerlist.DataSource = SeachResultFormData;

			// 対象レポートの呼出
			Passengerlist.Run();

			// コネクションクローズ
			if (_oracleConnection IsNot null)
			{
				if (_oracleConnection.State == ConnectionState.Open)
				{
					_oracleConnection.Close();
				}
				if (_oracleConnection IsNot null)
				{
					_oracleConnection.Dispose();
				}
			}


			//デフォルトを白黒印刷に設定
			Passengerlist.Document.Printer.DefaultPageSettings.Color = false;

			// 印刷
			if (Passengerlist.Document.Print(true, true))
			{
				CommonProcess.createFactoryMsg().messageDisp("I90_002", MsgDisplayPrint); // 共通メッセージ処理 [I90_002 {1}が完了しました。]

				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.formOut, "乗客名簿", "印刷が完了しました。");
			}

		}
		catch (Exception ex)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_043"); // 共通メッセージ処理 [E90_043 帳票出力でエラーが発生しました。]

			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.formOut, "乗客名簿", ex.ToString());

			throw;
		}

	}

	/// <summary>
	/// 「オプション予約明細表」出力処理
	/// </summary>
	private void printOptionYoyakuMeisai()
	{

		string PostData = string.Empty;

		//出発日
		PostData += "&base_select0=P04_1006;SYUPT_DAY&base_opecomp0==&base_value0=" + (System.Convert.ToDateTime(this.dtmSyuptDay.Value)).ToString("yyyyMMdd");

		//コースコード
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)))
		{
			PostData += "&free_opelogic_0_0=AND&free_select_0_0=P04_1006;CRS_CD&free_opecomp_0_0==&free_value_0_0=" + this.ucoCrsCd.CodeText;
		}

		//コース区分
		//定期（昼）
		if (this.rdoTeikiNoon.Checked)
		{
			PostData += "&free_opelogic_1_0=OR&free_select_1_0=P04_1006;CRS_KIND&free_opecomp_1_0==&free_value_1_0=" + CrsKindType.hatoBusTeiki;
			PostData += "&free_opelogic_1_1=AND&free_select_1_1=P04_1006;CRS_KBN_1&free_opecomp_1_1==&free_value_1_1=" + CrsKbn1Type.noon;
		}
		//定期（夜）
		if (this.rdoTeikiNight.Checked)
		{
			PostData += "&free_opelogic_1_2=OR&free_select_1_2=P04_1006;CRS_KIND&free_opecomp_1_2==&free_value_1_2=" + CrsKindType.hatoBusTeiki;
			PostData += "&free_opelogic_1_3=AND&free_select_1_3=P04_1006;CRS_KBN_1&free_opecomp_1_3==&free_value_1_3=" + CrsKbn1Type.night;
		}
		//定期（郊外）
		if (this.rdoTeikiKogai.Checked)
		{
			PostData += "&free_opelogic_1_4=OR&free_select_1_4=P04_1006;CRS_KBN_2&free_opecomp_1_4==&free_value_1_4=" + crsKbn2Type.suburbs;
		}
		//キャピタル
		if (this.rdoCapital.Checked)
		{
			PostData += "&free_opelogic_1_5=OR&free_select_1_5=P04_1006;CRS_KIND&free_opecomp_1_5==&free_value_1_5=" + CrsKindType.capital;
		}
		//企画（日帰り）
		if (this.rdoHigaeri.Checked)
		{
			PostData += "&free_opelogic_1_6=OR&free_select_1_6=P04_1006;CRS_KIND&free_opecomp_1_6==&free_value_1_6=" + CrsKindType.higaeri;
		}
		//企画（宿泊）
		if (this.rdoStay.Checked)
		{
			PostData += "&free_opelogic_1_7=OR&free_select_1_7=P04_1006;CRS_KIND&free_opecomp_1_7==&free_value_1_7=" + CrsKindType.stay;
		}
		//企画（都内R）
		if (this.rdoRCrs.Checked)
		{
			PostData += "&free_opelogic_1_8=OR&free_select_1_8=P04_1006;CRS_KIND&free_opecomp_1_8==&free_value_1_8=" + CrsKindType.rcourse;
		}

		//出発時刻
		if (!ReferenceEquals(this.dtmSyuptTimeFromTo.FromTimeValue24, null))
		{
			PostData += "&free_opelogic_2_0=AND&free_select_2_0=P04_1006;SYUPT_TIME_FROM&free_opecomp_2_0=>=&free_value_2_0=" + this.dtmSyuptTimeFromTo.FromTimeValue24Int;
		}
		if (!ReferenceEquals(this.dtmSyuptTimeFromTo.ToTimeValue24, null))
		{
			PostData += "&free_opelogic_3_0=AND&free_select_3_0=P04_1006;SYUPT_TIME_TO&free_opecomp_3_0=<=&free_value_3_0=" + this.dtmSyuptTimeFromTo.ToTimeValue24Int;
		}

		//号車
		if (!string.IsNullOrWhiteSpace(System.Convert.ToString(this.txtGousya.Text)))
		{
			PostData += "&free_opelogic_4_0=AND&free_select_4_0=P04_1006;GOUSYA&free_opecomp_4_0==&free_value_4_0=" + this.txtGousya.Text;
		}

		//邦人／外客区分
		if (this.chkCrsKindJapanese.Checked == true && this.chkCrsKindGaikokugo.Checked == false)
		{
			PostData += "&free_opelogic_5_0=AND&free_select_5_0=P04_1006;HOUJIN_GAIKYAKU_KBN&free_opecomp_5_0==&free_value_5_0=" + HoujinGaikyakuKbnType.Houjin;
		}
		else if (this.chkCrsKindJapanese.Checked == false && this.chkCrsKindGaikokugo.Checked == true)
		{
			PostData += "&free_opelogic_5_0=AND&free_select_5_0=P04_1006;HOUJIN_GAIKYAKU_KBN&free_opecomp_5_0==&free_value_5_0=" + HoujinGaikyakuKbnType.Gaikyaku;
		}

		ReadAppConfig reaAppConfig = new ReadAppConfig();
		CommonProcess.DataStudioId = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioId);
		CommonProcess.DataStudioPassword = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioPassword);
		BOCommon.showDataStudio(this.Name, BOCommon.SystemSetCd.dsid_OptionYoyakuMeisai, PostData);

		//ログメッセージ
		string logmsg = string.Empty;
		logmsg += "USERID=" + CommonProcess.DataStudioId;
		logmsg += "&PASSWORD=" + CommonProcess.DataStudioPassword;
		logmsg += logmsg + PostData;
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.DSCall, this.setTitle, logmsg);

	}
	#endregion
}
}