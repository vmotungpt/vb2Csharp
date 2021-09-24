/// <summary>
/// S00_0332 統計サブメニュー(顧客関連情報分析)
/// </summary>
public class S00_0332 : FormBase
{

	#region イベント

	/// <summary>
	/// フォーム起動時の独自処理
	/// </summary>
	protected override void StartupOrgProc()
	{

		//画面表示時の初期設定
		this.setControlInitiarize();

		//フォーカス設定
		this.ActiveControl = this.ButtonEx1;

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
	/// 「顧客管理情報出力」ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnEx1_Click(object sender, EventArgs e)
	{
		//【各画面毎】以下の画面を起動する
		// 起動画面： 『顧客管理情報出力』
		using (S07_0501 form = new S07_0501())
		{
			form.ShowDialog();
		}

	}
	/// <summary>
	/// 「割引クーポン利用実績集計」ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnEx2_Click(object sender, EventArgs e)
	{
		//【各画面毎】以下の画面を起動する
		// 起動画面： 『割引クーポン利用実績集計』
		using (S07_0502 form = new S07_0502())
		{
			form.ShowDialog();
		}

	}
	/// <summary>
	/// 「予約・UG状況表示」ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnEx3_Click(object sender, EventArgs e)
	{
		//【各画面毎】以下の画面を起動する
		// 起動画面： 『予約・UG状況表示』
		using (S07_0503 form = new S07_0503())
		{
			form.ShowDialog();
		}

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

	#endregion

}