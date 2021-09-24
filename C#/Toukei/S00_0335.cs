/// <summary>
/// S00_0335 統計サブメニュー（帳票）
/// </summary>
public class S00_0335 : FormBase
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
		this.ActiveControl = this.btnTiikiBetuUketukeBetuConstitutionPerTable;

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
	/// 地域別受付別構成率表ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnTiikiBetuUketukeBetuConstitutionPerTable_Click(object sender, EventArgs e)
	{
		//地域別受付別構成率表へ遷移
		//Using form As New S07_0302()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 割引実績表ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnWaribikiJissekiTable_Click(object sender, EventArgs e)
	{
		//割引実績表へ遷移
		//Using form As New S07_0303()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// インターネットコース別実績表ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnIntCrsBetuJissekiTable_Click(object sender, EventArgs e)
	{
		//インターネットコース別実績表へ遷移
		//Using form As New S07_0304()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 代理店WEB予約件数ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnDairitenWebYoyakuKensu_Click(object sender, EventArgs e)
	{
		//代理店WEB予約件数へ遷移
		//Using form As New S07_0305()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// バス会社台数集計ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnBusCompanyDaisuTtl_Click(object sender, EventArgs e)
	{
		//バス会社台数集計へ遷移
		//Using form As New S07_0306()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 催行率ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnSaikouPer_Click(object sender, EventArgs e)
	{
		//催行率へ遷移
		//Using form As New S07_0307()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 目的別降車ヶ所送客人数一覧表ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnMokutekiBetuKoshakashoSokyakNinzuListTable_Click(object sender, EventArgs e)
	{
		//目的別降車ヶ所送客人数一覧表へ遷移
		//Using form As New S07_0308()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 添乗員実績表ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnTenjyoinJissekiTable_Click(object sender, EventArgs e)
	{
		//添乗員実績表へ遷移
		//Using form As New S07_0309()
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
		this.setFormId = "S00_0335";
		this.setTitle = "統計サブメニュー（帳票）";

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
		this.btnTiikiBetuUketukeBetuConstitutionPerTable.Enabled = false;
		this.btnWaribikiJissekiTable.Enabled = false;
		this.btnIntCrsBetuJissekiTable.Enabled = false;
		this.btnDairitenWebYoyakuKensu.Enabled = false;
		this.btnBusCompanyDaisuTtl.Enabled = false;
		this.btnSaikouPer.Enabled = false;
		this.btnMokutekiBetuKoshakashoSokyakNinzuListTable.Enabled = false;
		this.btnTenjyoinJissekiTable.Enabled = false;

	}

	#endregion

}