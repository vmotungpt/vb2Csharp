/// <summary>
/// S00_0333 統計サブメニュー（自由分析）
/// </summary>
public class S00_0333 : FormBase
{

	#region イベント

	/// <summary>
	/// フォーム起動時の独自処理
	/// </summary>
	protected override void StartupOrgProc()
	{

		//画面上ボタン制御
		this.setBtnControl();

		//画面表示時の初期設定
		this.setControlInitiarize();

		//フォーカス設定
		this.ActiveControl = this.btnFreeAnalysisYoyakuInfo;

	}

	#region フッタ

	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{

		this.Close();
	}

	#endregion

	#region ボタン

	/// <summary>
	/// 自由分析（予約情報）ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnFreeAnalysisYoyakuInfo_Click(object sender, EventArgs e)
	{
		//自由分析（予約情報）へ遷移
		using (S07_0401 form = new S07_0401())
		{
			form.ShowDialog();
		}

	}

	/// <summary>
	/// 自由分析（顧客情報）ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnFreeAnalysisCustomerInfo_Click(object sender, EventArgs e)
	{
		//自由分析（顧客情報）へ遷移
		//Using form As New S07_0402()
		//    form.ShowDialog()
		//End Using
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
		this.setFormId = "S00_0333";
		this.setTitle = "統計サブメニュー（自由分析）";

		//Visible
		this.F1Key_Visible = false;
		this.F2Key_Visible = true;
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

		//Text
		this.F2Key_Text = "F2:戻る";

	}

	/// <summary>
	/// 画面上ボタン制御
	/// </summary>
	private void setBtnControl()
	{

		//ボタンを無条件で非活性(Phase2で使用するボタンの為)
		//Me.btnFreeAnalysisCustomerInfo.Enabled = False

	}

	#endregion

}