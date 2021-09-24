/// <summary>
/// S00_0334 統計サブメニュー（利用人員表）
/// </summary>
public class S00_0334 : FormBase
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
		this.ActiveControl = this.btnRiyouJininTable1Out;

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
	/// 利用人員表（Ⅰ）出力ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnRiyouJininTable1Out_Click(object sender, EventArgs e)
	{
		//利用人員表（Ⅰ）出力へ遷移
		using (S07_0201 form = new S07_0201())
		{
			form.ShowDialog();
		}

	}

	/// <summary>
	/// 利用人員表（Ⅱ）出力ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnRiyouJininTable2Out_Click(object sender, EventArgs e)
	{
		//利用人員表（Ⅱ）出力へ遷移
		using (S07_0202 form = new S07_0202())
		{
			form.ShowDialog();
		}

	}

	/// <summary>
	/// 共催会社別コース別乗車人員表出力ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnKyosaiCompanyBetuCrsBetuJyosyaJininTableOut_Click(object sender, EventArgs e)
	{
		//共催会社別コース別乗車人員表出力へ遷移
		//Using form As New S07_0203()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 利用人員表（年報）ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnRiyouJininTableNenpo_Click(object sender, EventArgs e)
	{
		//利用人員表（年報）へ遷移
		//Using form As New S07_0204()
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
		this.setFormId = "S00_0334";
		this.setTitle = "統計サブメニュー（利用人員表）";

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
		this.btnKyosaiCompanyBetuCrsBetuJyosyaJininTableOut.Enabled = false;
		this.btnRiyouJininTableNenpo.Enabled = false;

	}

	#endregion

}