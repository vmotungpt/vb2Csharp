/// <summary>
/// S00_0307 予約サブメニュー(使用中解除)
/// </summary>
public class S00_0307 : FormBase
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
		this.ActiveControl = this.btnYoyakuUsingDisplayRelease;

	}

	#region フッタ

	/// <summary>
	/// F2：閉じるボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{

		this.Close();
	}

	#endregion

	#region ボタン

	/// <summary>
	/// 予約使用中表示・解除ボタン実行時のイベント
	/// </summary>
	private void btnYoyakuUsingDisplayRelease_Click(object sender, EventArgs e)
	{
		//予約使用中表示・解除へ遷移
		using (S02_2301 form = new S02_2301())
		{
			form.ShowDialog();
		}

	}

	/// <summary>
	/// 予台帳・座席・予約使用中表示・解除ボタン実行時のイベント
	/// </summary>
	private void btnLedgerZasekiYoyakuUsingDisplayRelease_Click(object sender, EventArgs e)
	{
		//台帳・座席・予約使用中表示・解除へ遷移
		using (S02_2302 form = new S02_2302())
		{
			form.ShowDialog();
		}

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
		this.setFormId = "S00_0307";
		this.setTitle = "予約サブメニュー（使用中解除）";

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

		// 現状処理なし

	}

	#endregion

}