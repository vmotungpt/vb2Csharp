/// <summary>
/// 企画旅行予約状況表
/// </summary>
public class S07_0103 : PT01, iPT01
{

	#region  定数／変数宣言
	private const string CountRecords = "NUMBER_OF_RECORDS";

	private const string E90_011Param = "対象年月";

	private const string E70_003Param1 = "対象年月";

	private const string E70_003Param2 = "当月以前の年月";
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
		this.F2Key_Enabled = true;
		this.F7Key_Enabled = true;
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
		base.clearExistErrorProperty(this.gbxSearchJoken.Controls);
		S07_0103DA dataAccess = new S07_0103DA();
		S07_0103DA.S07_0103DASelectParam param = new S07_0103DA.S07_0103DASelectParam();
		//【各画面毎】画面・対象年月チェックを行う
		//・画面・対象年月＝空白の場合、メッセージが表示され処理中断
		if (CommonUtil.checkHissuError(this.gbxSearchJoken.Controls))
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_011", E90_011Param);
			return false;
		}
		//画面・対象年月≠空白の場合
		//画面・対象年月＞=当月の場合、メッセージが表示され処理中断
		object with_1 = this.txtYm;
		if (with_1.Value IsNot null)
		{
			if (CommonConvertUtil.ChkWhenDay(System.Convert.ToDateTime(with_1.Value), CommonConvertUtil.WhenKako) == false)
			{
				with_1.ExistError = true;
				with_1.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E70_003", E70_003Param1, E70_003Param2);
				return false;
			}
		}
		//画面・対象年月で売掛情報よりレコード数を取得し、WKレコード数に格納
		param.dtTaisho_YM = this.txtYm.ValueInt.Value;
		DataTable dataTable = dataAccess.selectCountTUrikakeInfo(param);
		//・WKレコード数＝0の場合、メッセージが表示され処理中断
		if (System.Convert.ToInt32(dataTable.Rows(0).Item(CountRecords)) == 0)
		{
			this.txtYm.ExistError = true;
			CommonProcess.createFactoryMsg().messageDisp("E90_019");
			return false;
		}
		return true;
	}
	#endregion

	#region 固有初期処理
	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{

		//「画面項目定義」を対象に、初期値記載された内容に準じて、項目の初期化を行う
		CommonUtil.Control_Init(this.gbxSearchJoken.Controls);

		//背景色初期化
		base.clearExistErrorProperty(this.gbxSearchJoken.Controls);

		//初期処理
		this.setSeFirsttDisplayData();

		// フッタボタンの設定
		this.initFooterButtonControl();
	}
	#endregion

	#endregion

	#region メソッド

	#region Datastudio 呼び出し
	/// <summary>
	/// 帳票出力結果
	/// </summary>
	protected override void ResultOutputFormData()
	{
		string PostData = string.Empty;
		try
		{
			if (txtYm.Value.HasValue)
			{
				PostData += "base_select0=P07_0102;YM&base_value0=" + this.txtYm.ValueInt.Value;
			}
			ReadAppConfig reaAppConfig = new ReadAppConfig();
			CommonProcess.DataStudioId = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioId);
			CommonProcess.DataStudioPassword = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioPassword);
			BOCommon.showDataStudio(this.Name, BOCommon.SystemSetCd.dsid_ShitenBetsuUriageShukyakuIchiranpyo, PostData);
		}
		catch (Exception)
		{
			throw;
		}
	}

	/// <summary>
	/// F7()ボタン押下時の独自処理
	/// </summary>
	protected override void btnF7_ClickOrgProc()
	{
		try
		{
			// 前処理
			base.comPreEvent();
			//取得件数チェック
			if (checkFormItem() == false)
			{
				return;
			}
			else
			{
				ResultOutputFormData();
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			// 後処理
			base.comPostEvent();
		}
	}


	#endregion

	#region チェック処理(Private)

	#endregion

	#endregion

	#region 実装用メソッド(画面毎に変更)
	/// <summary>
	/// 初期処理(必須画面個別実装)
	/// </summary>
	public void setSeFirsttDisplayData()
	{
		//画面・対象年月=システム年月
		//年月
		this.txtYm.Value = CommonDateUtil.getSystemTime();
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