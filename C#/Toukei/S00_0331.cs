/// <summary>
/// S00_0331 統計サブメニュー（予約・売上集計分析）
/// </summary>
public class S00_0331 : FormBase
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
		this.ActiveControl = this.btnUriageGaisanTable;

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
	/// 売上概算表ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnUriageGaisanTable_Click(object sender, EventArgs e)
	{
		//売上概算表へ遷移
		//Using form As New S07_0101()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 売上集計ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnUriageTtl_Click(object sender, EventArgs e)
	{
		//売上集計へ遷移
		//Using form As New S07_0102()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 支店別売上集客一覧ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnBranchBetuUriageSyukyakList_Click(object sender, EventArgs e)
	{
		//支店別売上集客一覧へ遷移
		//Using form As New S07_0103()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 営業所別予約受付一覧表ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnEigyosyoBetuYoyakuUketukeListTable_Click(object sender, EventArgs e)
	{
		//営業所別予約受付一覧表へ遷移
		using (S07_0104 form = new S07_0104())
		{
			form.ShowDialog();
		}

	}

	/// <summary>
	/// 座席販売実績出力ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnZasekiHanbaiJissekiOut_Click(object sender, EventArgs e)
	{
		//座席販売実績出力へ遷移
		//Using form As New S07_0105()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 媒体別予約実績出力ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnMediaBetuYoyakuJissekiOut_Click(object sender, EventArgs e)
	{
		//媒体別予約実績出力へ遷移
		//Using form As New S07_0106()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// チャネル別予約受付実績出力ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnChannelBetuYoyakuUketukeJissekiOut_Click(object sender, EventArgs e)
	{
		//チャネル別予約受付実績出力へ遷移
		//Using form As New S07_0107()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// チャネル別売上出力ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnChannelBetuUriageOut_Click(object sender, EventArgs e)
	{
		//チャネル別売上出力へ遷移
		//Using form As New S07_0108()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 部署別利用人員・売上出力ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnBusyoBetuRiyouJininUriageOut_Click(object sender, EventArgs e)
	{
		//部署別利用人員・売上出力へ遷移
		//Using form As New S07_0109()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 宿泊パック利用実績出力ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnStayPackRiyouJissekiOut_Click(object sender, EventArgs e)
	{
		//宿泊パック利用実績出力へ遷移
		//Using form As New S07_0110()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 予約受付実績表ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnYoyakuUketukeJissekiTable_Click(object sender, EventArgs e)
	{
		//予約受付実績表へ遷移
		//Using form As New S07_0111()
		//    form.ShowDialog()
		//End Using
	}

	/// <summary>
	/// 企画旅行予約状況表ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnKikakuTravelYoyakuSituationTable_Click(object sender, EventArgs e)
	{
		//企画旅行予約状況表へ遷移
		//Using form As New S07_0112()
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
		this.setFormId = "S00_0331";
		this.setTitle = "統計サブメニュー（予約・売上集計分析）";

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
		//Me.btnBranchBetuUriageSyukyakList.Enabled = False
		//Me.btnEigyosyoBetuYoyakuUketukeListTable.Enabled = False
		//Me.btnZasekiHanbaiJissekiOut.Enabled = False
		//Me.btnMediaBetuYoyakuJissekiOut.Enabled = False
		//Me.btnChannelBetuYoyakuUketukeJissekiOut.Enabled = False
		//Me.btnChannelBetuUriageOut.Enabled = False
		//Me.btnBusyoBetuRiyouJininUriageOut.Enabled = False
		//Me.btnStayPackRiyouJissekiOut.Enabled = False
		//Me.btnYoyakuUketukeJissekiTable.Enabled = False
		//Me.btnKikakuTravelYoyakuSituationTable.Enabled = False

	}

	#endregion

}