/// <summary>
/// S00_0308 予約サブメニュー(帳票)
/// </summary>
public class S00_0308 : FormBase
{

	#region イベント

	/// <summary>
	/// フォーム起動時の独自処理
	/// </summary>
	protected override void StartupOrgProc()
	{

		//画面上ボタン制御
		this.setBtnControl();

		//画面初期化
		this.setControlInitiarize();

		//フォーカス設定
		this.ActiveControl = this.btnMihakkenYoyakuListTable;

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
	/// 未発券予約一覧表ボタン実行時のイベント
	/// </summary>
	private void btnDuburiCheckList_Click(object sender, EventArgs e)
	{
		//未発券予約一覧表へ遷移
		//Using form As New S02_2401
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
		this.setFormId = "S00_0308";
		this.setTitle = "予約サブメニュー（帳票）";

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
		this.btnMihakkenYoyakuListTable.Enabled = false;

	}

	#endregion

}