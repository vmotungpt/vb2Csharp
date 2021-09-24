/// <summary>
/// S07_0102 売上集計
/// </summary>
public class S07_0102 : PT01, iPT01
{

	#region 定数／変数宣言
	/// <summary>
	/// 画面ID
	/// </summary>
	private const string ScreenId = "S07_0102";
	/// <summary>
	/// 画面名
	/// </summary>
	private const string ScreenName = "売上集計";

	/// <summary>
	/// 集計単位
	/// </summary>
	public enum TtlTaniNo : int
	{
		TtlTaniAgt = 1, //直・ＡＧＴ
		TtlTaniAffiliate = 2, //アフィリエイト
		TtlTaniInt = 3, //インターネット
		TtlTaniUriage = 4 //売上
	}

	#endregion

	#region イベント

	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{

		// エラー初期化
		this.initExitError();
		//初期画面設定
		this.setSeFirsttDisplayData();
		// フッタボタンの設定
		this.initFooterButtonControl();
	}

	/// <summary>
	/// 画面クロージングイベント
	/// </summary>
	protected override bool closingScreen()
	{

		return true;
	}

	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{

		base.closeFormFlg = true;
		this.Close();
	}

	/// <summary>
	/// F7：印刷ボタン押下イベント
	/// </summary>
	protected override void btnF7_ClickOrgProc()
	{

		DataTable dt = new DataTable(); //帳票出力用データテーブル
		int ttlTani = 0; //集計単位

		try
		{
			//前処理
			base.comPreEvent();

			// 入力チェック
			if (this.CheckSearch() == false)
			{

				return;
			}

			if (rdoHanbaiChannelBetuJyosyaTtl.Checked == true)
			{

				//P07_0113 販売チャネル別乗車集計を出力
				P07_0113 p07_0113 = new P07_0113();
				P07_0113Da P07_0113DataAcces = new P07_0113Da();

				//集計単位
				if (rdoAgt.Checked == true)
				{
					ttlTani = (int)TtlTaniNo.TtlTaniAgt;
				}
				else if (rdoAffiliate01.Checked == true)
				{
					ttlTani = (int)TtlTaniNo.TtlTaniAffiliate;
				}
				else if (rdoInt.Checked == true)
				{
					ttlTani = (int)TtlTaniNo.TtlTaniInt;
				}
				else
				{
					ttlTani = (int)TtlTaniNo.TtlTaniUriage;
				}

				//出力条件を基にデータを取得
				dt = P07_0113DataAcces.getDataP07_0113(setPrmInfoList(ttlTani));

				if (dt.Rows.Count == 0)
				{
					// 取得件数0件の場合、メッセージを表示
					CommonProcess.createFactoryMsg().messageDisp("E90_019");
					return;
				}
				else
				{

					//帳票のプロパティにパラメータをセット
					p07_0113.reportData = dt; //取得データ
					p07_0113.TaisyoYm = Strings.Format(dtmTtlYm.Value, "yyyy年MM月"); //対象年月
					p07_0113.ttlTaniKbn = ttlTani; //集計単位

					//帳票出力
					p07_0113.Run();
					p07_0113.Document.Print(true, true, false);

				}

			}
			else
			{

				// 出力条件パラメータのセット
				Hashtable paramInfoList = this.setPrmInfoList((int)TtlTaniNo.TtlTaniAffiliate);

				P07_0112Da p07_0112Da = new P07_0112Da();
				// 販売チャネル別コース別乗車リスト取得
				dt = p07_0112Da.getSalesChannelList(paramInfoList);

				if (dt.Rows.Count <= 0)
				{
					// 取得件数0件の場合、メッセージを表示
					CommonProcess.createFactoryMsg().messageDisp("E90_019");
					return;
				}

				// 販売チャネル別コース別乗車リスト出力
				P07_0112 p07_0112 = new P07_0112();
				p07_0112.DataSource = dt;
				p07_0112.Run();
				p07_0112.Document.Print(true, true, false);
			}

			// 処理成功時
			CommonProcess.createFactoryMsg().messageDisp("I90_002", "帳票出力");
			// log出力
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.formOut, ScreenName, "帳票出力");
		}
		catch
		{
			// 失敗時
			CommonProcess.createFactoryMsg().messageDisp("E90_028", "帳票出力");
			// log出力
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.formOut, ScreenName, "帳票出力");
			throw;

		}
		finally
		{
			//後処理
			base.comPostEvent();
		}

	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	protected override void initFooterButtonControl()
	{

		//Visible
		this.F1Key_Visible = false;
		this.F2Key_Visible = true;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		this.F7Key_Visible = true;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = false;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;

		//Text
		this.F2Key_Text = "F2:戻る";
		this.F7Key_Text = "F7:印刷";
	}

	#endregion

	#region メソッド

	/// <summary>
	/// 初期画面設定
	/// </summary>
	public void setSeFirsttDisplayData()
	{

		// 集計年月設定(システム日付の前月)
		this.dtmTtlYm.Value = CommonDa.createFactoryDA.getServerSysDate().AddMonths(-1);

		//帳票選択・集計単位
		this.rdoHanbaiChannelBetuJyosyaTtl.Checked = true;
		this.rdoAgt.Checked = true;
		//選択されていない帳票の集計単位は非活性にする
		this.rdoAffiliate02.Enabled = false;

		// 取扱部署設定
		CommonMstUtil.setComboBox(CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.toriatsukai_busho), this.cmbToriatukaiBusyo, true, null);
		// ログインユーザの取扱部署設定
		if (UserInfoManagement.toriatukaiBusyo != (System.Convert.ToInt32(FixedCd.ToriatsukaiBusyo.others)).ToString())
		{
			this.cmbToriatukaiBusyo.SelectedValue = UserInfoManagement.toriatukaiBusyo;
		}

	}

	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent"></param>
	public void DisplayDataToEntity(ref object ent)
	{

	}

	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <returns>検証結果</returns>
	public bool CheckSearch()
	{

		// エラー初期化
		this.initExitError();

		// 集計年月必須チェック
		if (ReferenceEquals(this.dtmTtlYm.Value, null))
		{
			this.dtmTtlYm.ExistError = true;
			this.dtmTtlYm.Focus();
			CommonProcess.createFactoryMsg.messageDisp("E90_011", this.lblTtlYm.Text);
			return false;
		}

		// コース種別必須チェック
		if (this.chkJapanese.Checked == false && this.chkGaikokugo.Checked == false
			&& this.chkTeikiNoon.Checked == false && this.chkTeikiNight.Checked == false
			&& this.chkKikaku.Checked == false)
		{

			this.chkJapanese.ExistError = true;
			this.chkGaikokugo.ExistError = true;
			this.chkTeikiNoon.ExistError = true;
			this.chkTeikiNight.ExistError = true;
			this.chkKikaku.ExistError = true;
			this.chkJapanese.Focus();
			CommonProcess.createFactoryMsg.messageDisp("E90_024", this.lblCrsKind.Text);
			return false;
		}

		return true;
	}

	/// <summary>
	/// エラー初期化
	/// </summary>
	private void initExitError()
	{
		this.dtmTtlYm.ExistError = false;
		this.chkJapanese.ExistError = false;
		this.chkGaikokugo.ExistError = false;
		this.chkTeikiNoon.ExistError = false;
		this.chkTeikiNight.ExistError = false;
		this.chkKikaku.ExistError = false;
	}

	/// <summary>
	/// 帳票選択「販売チャネル別乗車集計」選択時画面制御
	/// </summary>
	private void rdoHanbaiChannelBetuJyosyaTtl_CheckedChanged(object sender, EventArgs e)
	{

		//選択した帳票の「集計単位」を活性、初期値を設定する
		this.rdoAgt.Enabled = true;
		this.rdoAffiliate01.Enabled = true;
		this.rdoInt.Enabled = true;
		this.rdoUriage.Enabled = true;
		this.rdoAgt.Checked = true;

		//選択されていない帳票の「集計単位」を非活性、未選択にする
		this.rdoAffiliate02.Enabled = false;
		this.rdoAffiliate02.Checked = false;

	}

	/// <summary>
	/// 帳票選択「販売チャネル別コース別乗車リスト」選択時画面制御
	/// </summary>
	private void rdoHanbaiChannelBetuCrsBetuJyosyaList_CheckedChanged(object sender, EventArgs e)
	{

		//選択した帳票の「集計単位」を活性、初期値を設定する
		rdoAffiliate02.Enabled = true;
		rdoAffiliate02.Checked = true;

		//選択されていない帳票の「集計単位」を非活性、未選択にする
		rdoAgt.Enabled = false;
		rdoAffiliate01.Enabled = false;
		rdoInt.Enabled = false;
		rdoUriage.Enabled = false;
		rdoAgt.Checked = false;
		rdoAffiliate01.Checked = false;
		rdoInt.Checked = false;
		rdoUriage.Checked = false;

	}

	/// <summary>
	/// 出力条件パラメータのセット
	/// </summary>
	/// <param name="ttlTani">指定されている集計単位の値</param>
	/// <returns>検索条件パラメータ</returns>
	private Hashtable setPrmInfoList(int ttlTani)
	{

		//画面の出力条件をパラメータにセットする
		Hashtable paramInfoList = new Hashtable();

		paramInfoList.Add("TtlYm", Strings.Format(dtmTtlYm.Value, "yyyyMM")); //集計年月
		paramInfoList.Add("TtlTani", ttlTani); //集計単位

		paramInfoList.Add("Japanese", Strings.Trim(System.Convert.ToString(chkJapanese.Checked))); //日本語
		paramInfoList.Add("Gaikokugo", Strings.Trim(System.Convert.ToString(chkGaikokugo.Checked))); //外国語
		paramInfoList.Add("TeikiNoon", Strings.Trim(System.Convert.ToString(chkTeikiNoon.Checked))); //定期（昼）
		paramInfoList.Add("TeikiNight", Strings.Trim(System.Convert.ToString(chkTeikiNight.Checked))); //定期（夜）
		paramInfoList.Add("Kikaku", Strings.Trim(System.Convert.ToString(chkKikaku.Checked))); //企画

		if (!(string.IsNullOrWhiteSpace(System.Convert.ToString(cmbToriatukaiBusyo.SelectedItem.SubItems[0].Value.ToString())) == true))
		{
			paramInfoList.Add("ManagementSec", Strings.Trim(System.Convert.ToString(cmbToriatukaiBusyo.SelectedItem.SubItems[0].Value.ToString()))); //取扱部署
		}

		return paramInfoList;

	}


	#endregion

}