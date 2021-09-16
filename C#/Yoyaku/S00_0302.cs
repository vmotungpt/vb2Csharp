/// <summary>
/// S00_0302
/// </summary>
public class S00_0302 : FormBase
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
	/// 座席照会・変更ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnZasekiInquiryChange_Click(object sender, EventArgs e)
	{
		// パラメータ設定
		S02_0902_ParamData S02_0902_param = new S02_0902_ParamData();
		S02_0902_param.editmode = 1; // 変更
		S02_0902_param.editkind = 2; // 座席照会・変更
		S02_0902_param.crsCd = string.Empty; // コースコード未指定
		S02_0902_param.syuptDay = 0; // 出発日未指定
		S02_0902_param.gousya = 0; // 号車未指定

		using (S02_0902 form = new S02_0902())
		{
			form.ParamData = S02_0902_param;
			// フォーム表示
			form.ShowDialog();
		}

	}

	/// <summary>
	/// 発券(船車券)ボタン実行時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnYoyakuGousyaMove_Click(object sender, EventArgs e)
	{
		// パラメータ設定
		S02_0902_ParamData S02_0902_param = new S02_0902_ParamData();
		S02_0902_param.editmode = 1; // 変更
		S02_0902_param.editkind = 1; // 号車移動
		S02_0902_param.crsCd = string.Empty; // コースコード未指定
		S02_0902_param.syuptDay = 0; // 出発日未指定
		S02_0902_param.gousya = 0; // 号車未指定

		using (S02_0902 form = new S02_0902())
		{
			form.ParamData = S02_0902_param;
			// フォーム表示
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
		this.setFormId = "S00_0302";
		this.setTitle = "予約サブメニュー（座席照会・変更）";

		//フッタボタンの設定
		this.setButtonInitiarize();

		//フォーカス設定
		this.ActiveControl = this.btnZasekiInquiryChange;

	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{

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

		//TODO:権限周りが決まるまで、一旦コメントアウト
		//ボタンを全て非活性に設定
		//Me.btnZasekiInquiryChange.Enabled = False
		//Me.btnYoyakuGousyaMove.Enabled = False

		//権限制御情報に従い各ボタンの活性/ 非活性を設定
		//TODO:権限に関する事が決まっていない為、一旦保留

	}

	#endregion

}