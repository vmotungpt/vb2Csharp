/// <summary>
/// S00_0301 予約サブメニュー（発券）
/// </summary>
public class S00_0301 : FormBase
{

	#region イベント

	/// <summary>
	/// フォーム起動時の独自処理
	/// </summary>
	protected override void StartupOrgProc()
	{

		//画面上ボタン制御
		this.setBtnControl();

		// 画面初期化
		this.setControlInitiarize();

		//フォーカス設定
		this.ActiveControl = this.btnHakkenInfoInquiryYoyakuInquiry;

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
	/// 発券情報照会(予約照会)ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnHakkenInfoInquiryYoyakuInquiry_Click(object sender, EventArgs e)
	{
		//発券情報照会（予約照会）へ遷移
		using (S02_0601 form = new S02_0601())
		{
			form.ShowDialog();
		}

	}

	/// <summary>
	/// 発券(船車券)ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnHakkenSensyaKen_Click(object sender, EventArgs e)
	{
		//発券（船車券）へ遷移
		//Using form As New S02_0607
		//    form.showdialog()
		//end using
	}

	/// <summary>
	/// 発券(宿泊券)ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnHakkenStayKen_Click(object sender, EventArgs e)
	{
		//発券（宿泊券）へ遷移
		//Using form As New S02_0608
		//    form.showdialog()
		//End Using
	}

	/// <summary>
	/// 発券(旅行参加券)ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnHakkenTravelSankaKen_Click(object sender, EventArgs e)
	{
		//発券（旅行参加券）へ遷移
		//Using form As New S02_0609
		//    form.showdialog()
		//End Using
	}

	/// <summary>
	/// 発券(観光券)ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnHakkenKankoKen_Click(object sender, EventArgs e)
	{
		//発券（観光券）へ遷移
		//Using form As New S02_0610
		//    form.showdialog()
		//End Using
	}

	/// <summary>
	/// バウチャー(予約書)発行ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnVoucherIssue_Click(object sender, EventArgs e)
	{
		//バウチャー（予約書）発行へ遷移
		//Using form As New S02_0611
		//    form.showdialog()
		//End Using
	}

	#endregion

	#endregion

	#region メソッド

	/// <summary>
	/// 画面初期化
	/// </summary>
	private void setControlInitiarize()
	{

		//ベースフォームの設定
		this.setFormId = "S00_0301";
		this.setTitle = "予約サブメニュー（発券）";

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
	/// フッタボタンの設定
	/// </summary>
	private void setBtnControl()
	{

		//ボタンを無条件で非活性（Phase2で使用するボタンの為）
		this.btnHakkenSensyaKen.Enabled = false;
		this.btnHakkenStayKen.Enabled = false;
		this.btnHakkenTravelSankaKen.Enabled = false;
		this.btnHakkenKankoKen.Enabled = false;
		this.btnVoucherIssue.Enabled = false;

	}

	#endregion

}