/// <summary>
/// S00_0306 予約サブメニュー（予約ログ照会）
/// </summary>
/// <remarks>
/// Date:2018/10/12
/// Author:Luan.tx
/// </remarks>
public class S00_0306 : FormBase
{

	#region フッタ
	/// <summary>
	/// インターネット予約ログ照会ボタン実行時のイベント
	/// </summary>
	private void btnIntYoyakuLogInquiry_Click(object sender, EventArgs e)
	{
		using (S02_1901 S02_1901 = new S02_1901())
		{
			S02_1901.ShowDialog();
		}

	}

	//TODO:遷移方法の反映
	/// <summary>
	/// 予約メール送信エラー照会ボタン実行時のイベント
	/// </summary>
	private void btnYoyakuMailSendingErrorInquiry_Click(object sender, EventArgs e)
	{
		using (S02_1903 S02_1903 = new S02_1903())
		{
			S02_1903.ShowDialog();
		}

	}


	#endregion

	#region メソッド

	/// <summary>
	/// 画面初期化
	/// </summary>
	private void setControlInitiarize()
	{

		//ベースフォームの設定
		this.setFormId = "S00_0306";
		this.setTitle = "予約サブメニュー（予約ログ照会）";

		//【各画面毎】画面上ボタン制御
		initialControl();

		// フッタボタンの設定
		this.setButtonInitiarize();
		// ロード時にフォーカスを設定する
		this.ActiveControl = this.btnIntYoyakuLogInquiry;
	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{
		//Visible
		this.F1Key_Visible = false;
		//各画面毎】フッターボタン制御
		//	「F2：戻る」ボタンのみを表示させる
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
		//Enabled
		this.F2Key_Enabled = true;
	}

	/// <summary>
	/// 画面全体
	/// </summary>
	private void initialControl()
	{

		// 現状処理無し

	}

	#endregion

	#region    F2
	/// <summary>
	/// F2()ボタン押下時の独自処理
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		//戻る
		base.closeFormFlg = true;
		this.Close();
	}
	#endregion

	#region    Load時
	/// <summary>
	/// 画面起動時の独自処理
	/// </summary>
	protected override void StartupOrgProc()
	{
		//画面パターン毎の固有初期処理
		setControlInitiarize();
	}
	#endregion

}