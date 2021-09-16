/// <summary>
/// S00_0304 予約サブメニュー（入金確認）
/// </summary>
public class S00_0304 : FormBase
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

		// ロード時にフォーカスを設定する
		this.ActiveControl = this.btnMiNyuukinList;

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
	/// 未入金一覧ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnMinyuukinList_Click(object sender, EventArgs e)
	{
		//未入金一覧へ遷移
		using (S02_1501 form = new S02_1501())
		{
			form.ShowDialog();
		}

	}

	/// <summary>
	/// 過剰入金一覧ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnKazyouNyuukinList_Click(object sender, EventArgs e)
	{
		using (S02_1502 form = new S02_1502())
		{
			form.ShowDialog();
		}

	}

	/// <summary>
	/// 入金確認連絡ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnNyuukinKakuninContact_Click(object sender, EventArgs e)
	{
		//入金確認連絡へ遷移
		//Phase2にて使用
		//Using form As New S02_1503
		//    form.ShowDialog()
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
		this.setFormId = "S00_0304";
		this.setTitle = "予約サブメニュー（入金確認）";

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

		//無条件で非活性(Phase2で使用するボタンの為)
		btnNyuukinKakuninContact.Enabled = false;

	}

	#endregion

}