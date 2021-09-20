using System.ComponentModel;
using C1.Win.C1FlexGrid;
using GrapeCity.ActiveReports;


/// <summary>
/// S02_0601 発券情報照会（予約照会）
/// </summary>
public class S02_0601 : FormBase, iPT11
{

	#region 変数／定数
	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	public const int TopGbxCondition = 41;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	public const int MarginGbxCondition = 6;
	/// <summary>
	/// 列番号 予約区分（非表示）
	/// </summary>
	private const int NoColYoyakuKbn = 1;
	/// <summary>
	/// 列番号 予約NO（非表示）
	/// </summary>
	private const int NoColYoyakuNo = 2;
	/// <summary>
	/// 列番号 発券
	/// </summary>
	private const int NoColHakken = 3;
	/// <summary>
	/// 列番号 発券補助
	/// </summary>
	private const int NoColHakkenHojo = 4;
	/// <summary>
	/// グリッド表示件数の上限
	/// </summary>
	private const int MaxDispRow = 60;
	/// <summary>
	/// F6キーの幅
	/// </summary>
	private const int WidthF6 = 40;
	/// <summary>
	/// F7キーの幅
	/// </summary>
	private const int WidthF7 = 30;
	/// <summary>
	/// F7キーのX座標
	/// </summary>
	private const int XPosF7 = 700;

	/// <summary>
	/// 座席表示の可否
	/// </summary>
	/// <remarks>初期値：表示</remarks>
	private bool _isShowingZaseki = true;
	/// <summary>
	/// GMT代理店モード
	/// </summary>
	/// <remarks>初期値：True</remarks>
	private bool _isGmtDairitenMode = true;
	/// <summary>
	/// GMT代理店コード/コードマスタ.コード分類
	/// </summary>
	/// <remarks>初期値：True</remarks>
	private string gmtCdBunrui = "036";
	/// <summary>
	/// GMT代理店コード/コードマスタ.コード値
	/// </summary>
	/// <remarks>初期値：True</remarks>
	private string gmtCdValue = "01";
	/// <summary>
	/// 代理店コード「ＧＭＴ」
	/// </summary>
	string strDairitenCdGMT = null;
	/// <summary>
	/// 検索条件：ピックアップ指定値
	/// </summary>
	public enum PickUpValue : int
	{
		PickUpAri = 1, //ピックアップ有
		PickUpNasi, //ピックアップ無
		PickUpSiteiNasi //ピックアップ未指定
	}
	#endregion

	#region プロパティ
	/// <summary>
	/// 条件GroupBoxの高さ
	/// </summary>
	/// <returns></returns>
	public int HeightGbxCondition
	{
		get
		{
			return this.gbxCondition.Height;
		}
		set
		{
			this.gbxCondition.Height = value;
		}
	}

	/// <summary>
	/// 条件GroupBoxの表示/非表示
	/// </summary>
	/// <returns></returns>
	public bool VisibleGbxCondition
	{
		get
		{
			return this.gbxCondition.Visible;
		}
		set
		{
			this.gbxCondition.Visible = value;
		}
	}
	#endregion

	#region イベント

	/// <summary>
	/// ロード時イベント
	/// </summary>
	protected override void StartupOrgProc()
	{

		//MyBase.StartupOrgProc()

		//GMTの代理店コードを取得
		this.strDairitenCdGMT = getGmtCd();

		// 画面初期化
		this.setControlInitiarize();

		//条件非表示ボタンへフォーカス
		this.ActiveControl = this.btnVisiblerCondition;

	}

	/// <summary>
	/// 条件GroupBox表示制御ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnVisiblerCondition_Click(object sender, EventArgs e)
	{
		this.VisibleGbxCondition = !this.VisibleGbxCondition;

		//Panel, グリッドの座標, サイズを表示/非表示に応じて変更
		if (this.VisibleGbxCondition)
		{
			//表示状態
			this.btnVisiblerCondition.Text = FixedCd.CommonButtonTextType.hiHyoji;

			this.PanelEx1.Top = TopGbxCondition + this.HeightGbxCondition + MarginGbxCondition;
			this.PanelEx1.Height -= this.HeightGbxCondition + MarginGbxCondition;
			this.grdHakkenInfoInquiry.Height -= this.HeightGbxCondition - 3;

		}
		else
		{
			//非表示状態
			this.btnVisiblerCondition.Text = FixedCd.CommonButtonTextType.hyoji;

			this.PanelEx1.Top = TopGbxCondition;
			this.PanelEx1.Height += this.HeightGbxCondition + MarginGbxCondition;
			this.grdHakkenInfoInquiry.Height += this.HeightGbxCondition - 3;
		}
	}

	/// <summary>
	/// フォームキーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
	{
		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			e.Handled = true;
			this.btnSearch_Click(sender, e);
		}
	}

	/// <summary>
	/// F6：予約確認書ボタン押下イベント
	/// </summary>
	protected override void btnF6_ClickOrgProc()
	{
		base.btnF6_ClickOrgProc();

		try
		{
			//前処理
			base.comPreEvent();

			//確認メッセージ
			string msg = System.Convert.ToString(CommonHakken.createHakkenMsg(CommonHakken.OutFormKbnReservationCertificate, CommonHakken.HakkenModePrintOnly));
			MsgBoxResult msgResult = CommonProcess.createFactoryMsg.messageDisp("Q90_001", msg);
			if (msgResult == MsgBoxResult.Cancel)
			{
				return;
			}

			//帳票出力
			if (printP02_0601(System.Convert.ToString(CommonHakken.OutFormKbnReservationCertificate)) == false)
			{
				//異常終了
				CommonProcess.createFactoryMsg().messageDisp("E90_025", "処理", msg);
				// log出力
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.formOut, setFormId, "帳票出力");

				return;
			}

			//正常終了
			CommonProcess.createFactoryMsg().messageDisp("I90_002", msg);
			// log出力
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.formOut, setFormId, "帳票出力");

		}
		catch
		{
			throw;

		}
		finally
		{
			// 後処理
			base.comPostEvent();
		}

	}

	/// <summary>
	/// F7：ご乗車案内ボタン押下イベント
	/// </summary>
	protected override void btnF7_ClickOrgProc()
	{
		base.btnF7_ClickOrgProc();

		try
		{
			//前処理
			base.comPreEvent();

			//確認メッセージ
			string msg = System.Convert.ToString(CommonHakken.createHakkenMsg(CommonHakken.OutFormKbnBordingInfomation, CommonHakken.HakkenModePrintOnly));
			MsgBoxResult msgResult = CommonProcess.createFactoryMsg.messageDisp("Q90_001", msg);
			if (msgResult == MsgBoxResult.Cancel)
			{
				return;
			}

			//帳票出力
			if (printP02_0601(System.Convert.ToString(CommonHakken.OutFormKbnBordingInfomation)) == false)
			{
				//異常終了
				CommonProcess.createFactoryMsg().messageDisp("E90_025", "処理", msg);
				// log出力
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.formOut, setFormId, "帳票出力");

				return;
			}

			//正常終了
			CommonProcess.createFactoryMsg().messageDisp("I90_002", msg);
			// log出力
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.formOut, setFormId, "帳票出力");

		}
		catch
		{
			throw;

		}
		finally
		{
			// 後処理
			base.comPostEvent();
		}

	}

	/// <summary>
	/// F8：検索ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnSearch_Click(object sender, EventArgs e)
	{

		// F8ボタン処理実行
		base.btnCom_Click(this.btnSearch, e);

		// log出力
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, setFormId, "検索処理");

	}

	/// <summary>
	/// F8ボタン押下イベント
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{
		base.btnF8_ClickOrgProc();

		// エラー有無のクリア
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		//予約区分+NO
		string yoyakuKbnNo = System.Convert.ToString(this.ucoYoyakuNo.YoyakuText);

		// Web予約作成ゴミデータ削除処理
		S02_0301Da dataAccess = new S02_0301Da();
		dataAccess.updateWebWastedData();

		if (string.IsNullOrWhiteSpace(yoyakuKbnNo))
		{
			//予約番号が空
			this.processF8ByYoyakuEmpty();

		}
		else
		{
			//予約番号が入力済み
			this.processF8ByYoyaku(yoyakuKbnNo);
		}
	}

	/// <summary>
	/// 条件クリアボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnClear_Click(object sender, EventArgs e)
	{

		// クリアボタン押下イベント実行
		base.btnCom_Click(this.btnClear, e);

	}

	/// <summary>
	/// 条件クリアボタン押下時
	/// </summary>
	protected override void btnCLEAR_ClickOrgProc()
	{

		// 初期処理と同じ処理を実行
		StartupOrgProc();

	}

	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		base.btnF2_ClickOrgProc();

		this.Close();
	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void FlexGridEx1_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		//データ件数を表示(ヘッダー行分マイナス1)
		string formatedCount = System.Convert.ToString((this.grdHakkenInfoInquiry.Rows.Count - 1).ToString().PadLeft(6));
		this.lblLengthGrd.Text = formatedCount + "件";
	}

	/// <summary>
	/// 発券列、発券補助列ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void hakken_CellButtonClick()
	{
		)grdHakkenInfoInquiry.CellButtonClick;

		FlexGridEx grd = TryCast(sender, FlexGridEx);
		if (grd.Row <= 0)
		{
			return;
		}

		//押下行の予約区分、予約NO取得
		string yoyakuKbn = System.Convert.ToString(TryCast(grd.GetData(e.Row, NoColYoyakuKbn), string));
		if (ReferenceEquals(grd.GetData(e.Row, NoColYoyakuNo), null))
		{
			return;
		}
		int yoyakuNo = int.Parse(System.Convert.ToString(grd.GetData(e.Row, NoColYoyakuNo).ToString()));

		//押下列番号
		if (e.Col == NoColHakken)
		{
			//発券ボタン押下の場合

			//画面間パラメータを用意
			S02_0602ParamData prm = new S02_0602ParamData();
			prm.YoyakuKbn = yoyakuKbn;
			prm.YoyakuNo = yoyakuNo;

			//発券　画面展開
			using (S02_0602 form = new S02_0602())
			{
				form.ParamData = prm;
				form.ShowDialog();

				//更新あればリロード
				if (form.IsUpdated == true)
				{
					//グリッド初期化
					this.setInitiarizeGrid();

					//予約区分+NO
					string yoyakuKbnNo = System.Convert.ToString(this.ucoYoyakuNo.YoyakuText);
					DataTable yoyakuInfoBasicTable = null;

					if (string.IsNullOrWhiteSpace(yoyakuKbnNo))
					{
						//予約番号が空
						yoyakuInfoBasicTable = this.getYoyakuInfoBasic();

					}
					else
					{
						//予約番号が入力済み
						yoyakuInfoBasicTable = this.getYoyakuInfoBasic(yoyakuKbn, yoyakuNo);
					}

					//表示用に加工
					DataView yoyakuInfoBasicView = this.formatYoyakuInfoBasic(yoyakuInfoBasicTable);

					//予約情報（基本）を表示
					this.grdHakkenInfoInquiry.DataSource = yoyakuInfoBasicView;
				}
			}

		}
		else if (e.Col == NoColHakkenHojo)
		{
			//発券補助ボタン押下の場合

			//画面間パラメータを用意
			S02_0603ParamData prm = new S02_0603ParamData();
			prm.YoyakuKbn = yoyakuKbn;
			prm.YoyakuNo = yoyakuNo;

			//発券補助　画面展開
			using (S02_0603 form = new S02_0603())
			{
				form.ParamData = prm;
				form.ShowDialog();
			}

		}
	}

	/// <summary>
	/// 「GMTのみ」チェックON/OFF時制御
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void chkGmtOnly_CheckedChanged(object sender, EventArgs e)
	{

		this._isGmtDairitenMode = System.Convert.ToBoolean(chkGmtOnly.Checked);

		if (this._isGmtDairitenMode == true)
		{
			//チェックボックス「GMTのみ」がONの場合
			this.setGmt();
			this.ucoDairitenName.CompanyCodeText = "";
			this.ucoDairitenName.Enabled = false;

		}
		else
		{
			//チェックボックス「GMTのみ」がOFFの場合
			this.ucoDairitenName.CodeText = "";
			this.ucoDairitenName.ValueText = "";
			this.ucoDairitenName.CompanyCodeText = "";
			this.ucoDairitenName.Enabled = true;
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
		this.setFormId = "S02_0601";
		this.setTitle = "発券情報照会（予約照会）";

		// フッタボタンの設定
		this.setButtonInitiarize();

		// グリッドの設定
		this.setGrid();

		//条件部分初期化
		this.setConditionInitiarize();

		// エラー有無のクリア
		base.clearExistErrorProperty(this.gbxCondition.Controls);

	}

	/// <summary>
	/// 条件部分初期化
	/// </summary>
	private void setConditionInitiarize()
	{
		this.ucoYoyakuNo.YoyakuText = "";
		this.pnlTeikiKikaku.BorderStyle = BorderStyle.None;
		this.pnlTeikiKikaku.BorderColor = Color.Transparent;
		this.rdoTeiki.Checked = true;
		this.ucoSyuptDay.ResetText();
		this.txtNameBf.Text = "";
		this.ucoCrsCd.setControlInitiarize();
		this.ucoNoribaCd.setControlInitiarize();
		this.txtTelNo.Text = "";
		//GMT初期値ON
		this.chkGmtOnly.Checked = true;
		this.ucoDairitenName.CodeText = strDairitenCdGMT;
		this.ucoDairitenName.Enabled = false;
		//ピックアップ初期値ON
		this.pnlPickUpFlg.BorderStyle = BorderStyle.None;
		this.pnlPickUpFlg.BorderColor = Color.Transparent;

		this.setInitiarizeGrid();
	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{

		//Visible
		this.F2Key_Visible = true;
		this.F6Key_Visible = true;
		this.F7Key_Visible = true;

		this.F1Key_Visible = false;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = false;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;

		//Text
		this.F2Key_Text = "F2:戻る";
		this.F6Key_Text = "F6:" + CommonHakken.TitleReservationCertificateJapanese;
		this.F7Key_Text = "F7:" + CommonHakken.TitleBordingInformationJapanese;

		//Width
		this.F6Key_Width = WidthF6;
		this.F7Key_Width = WidthF7;

		//Location_X
		this.F7Key_Location_X = XPosF7;
	}

	/// <summary>
	/// F8押下処理 「予約番号が空」
	/// </summary>
	private void processF8ByYoyakuEmpty()
	{
		//出発日の必須入力チェック
		if (ReferenceEquals(this.ucoSyuptDay.Value, null))
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_023", "出発日");
			this.ucoSyuptDay.ExistError = true;
			return;
		}

		//出発日の暦上チェック （1年よりも前ならエラー
		if (this.ucoSyuptDay.Value < CommonProcess.getDateTime().AddYears(-1))
		{
			CommonProcess.createFactoryMsg().messageDisp("E03_001", "1年");
			this.ucoSyuptDay.ExistError = true;
			return;
		}

		//コースコードの存在チェック
		string crsCd = System.Convert.ToString(this.ucoCrsCd.CodeText);
		if (!string.IsNullOrWhiteSpace(crsCd))
		{
			string strSyuptDay = "";
			if (this.ucoSyuptDay.Value IsNot null)
			{
				strSyuptDay = System.Convert.ToString(this.ucoSyuptDay.Value.ToString().Substring(0, 10).Replace("/", "").Replace("_", ""));
			}
			int intSyuptDay = 0;
			int.TryParse(strSyuptDay, out intSyuptDay);
			if (YoyakuBizCommon.existsCrsCd(crsCd, intSyuptDay) == false)
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_014", "コースコード");
				this.ucoCrsCd.ExistError = true;
				return;
			}
		}

		//乗車地の存在チェック
		string noribaCd = System.Convert.ToString(this.ucoNoribaCd.CodeText);
		if (!string.IsNullOrWhiteSpace(noribaCd))
		{
			if (YoyakuBizCommon.existsNoribaCd(noribaCd) == false)
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_014", "乗車地コード");
				this.ucoNoribaCd.ExistError = true;
				return;
			}
		}


		//■グリッドへの表示
		//グリッド初期化
		this.setInitiarizeGrid();

		//予約情報（基本）取得、存在チェック
		DataTable yoyakuInfoBasicTable = this.getYoyakuInfoBasic();
		if (!existsDatas(yoyakuInfoBasicTable))
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_014", "予約情報");
			return;
		}

		//件数チェック
		if (yoyakuInfoBasicTable.Rows.Count > MaxDispRow)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が");
		}

		//表示用に加工
		DataView yoyakuInfoBasicView = this.formatYoyakuInfoBasic(yoyakuInfoBasicTable);

		//予約情報（基本）を表示
		this.grdHakkenInfoInquiry.DataSource = yoyakuInfoBasicView;
	}

	/// <summary>
	/// F8押下処理 「予約番号あり」
	/// </summary>
	/// <param name="yoyakuKbnNo">予約区分 + NO</param>
	private void processF8ByYoyaku(string yoyakuKbnNo)
	{
		string yoyakuKbn = "";
		string strYoyakuNo = "";
		if (yoyakuKbnNo.Length == 10)
		{
			yoyakuKbn = yoyakuKbnNo.Substring(0, 1);
			strYoyakuNo = yoyakuKbnNo.Substring(1, yoyakuKbnNo.Length - 1);
		}
		else if (yoyakuKbnNo.Length < 10)
		{
			yoyakuKbn = "0";
			strYoyakuNo = yoyakuKbnNo;
		}

		int yoyakuNo = 0; //予約NO
		if (!int.TryParse(strYoyakuNo, out yoyakuNo))
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_014", "予約情報");
			return;
		}

		//予約情報（基本）取得、存在チェック
		DataTable yoyakuInfoBasicTable = this.getYoyakuInfoBasic(yoyakuKbn, yoyakuNo);
		if (!existsDatas(yoyakuInfoBasicTable))
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_014", "予約情報");
			return;
		}

		//出発日の過去日付チェック
		if (ReferenceEquals(yoyakuInfoBasicTable.Rows(0).Item("SYUPT_DAY"), null))
		{
			return;
		}
		if (Information.IsDBNull(yoyakuInfoBasicTable.Rows(0).Item("SYUPT_DAY")))
		{
			return;
		}
		int intSyuptDay = 0;
		int.TryParse(System.Convert.ToString(yoyakuInfoBasicTable.Rows(0).Item("SYUPT_DAY").ToString()), out intSyuptDay);
		string strSyuptDay = intSyuptDay.ToString().Substring(0, 4) + "/" + intSyuptDay.ToString().Substring(4, 2) + "/" + intSyuptDay.ToString().Substring(6, 2);
		DateTime dtSyuptDay = DateTime.Parse(strSyuptDay);
		bool isKakoSyuptDay = System.Convert.ToBoolean(CommonConvertUtil.ChkWhenDay(dtSyuptDay, CommonConvertUtil.WhenKako));
		if (isKakoSyuptDay)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_053");
			return;
		}

		//その他条件の入力チェック
		if (!ReferenceEquals(this.ucoSyuptDay.Value, null) || !string.IsNullOrWhiteSpace(System.Convert.ToString(this.txtNameBf.Text)) || !string.IsNullOrWhiteSpace(System.Convert.ToString(this.ucoCrsCd.CodeText)) || !string.IsNullOrWhiteSpace(System.Convert.ToString(this.ucoNoribaCd.CodeText)) || !string.IsNullOrWhiteSpace(System.Convert.ToString(this.txtTelNo.Text)))
		{

			CommonProcess.createFactoryMsg().messageDisp("E90_011", "予約番号のみ");
			return;
		}


		//■グリッドへの表示
		//グリッド初期化
		this.setInitiarizeGrid();

		//表示用に加工
		DataView yoyakuInfoBasicView = this.formatYoyakuInfoBasic(yoyakuInfoBasicTable);

		//予約情報（基本）を表示
		this.grdHakkenInfoInquiry.DataSource = yoyakuInfoBasicView;


		//■画面遷移
		//画面間パラメータを用意
		S02_0602ParamData prm = new S02_0602ParamData();
		prm.YoyakuKbn = yoyakuKbn;
		prm.YoyakuNo = yoyakuNo;

		//発券　画面展開
		using (S02_0602 form = new S02_0602())
		{
			form.ParamData = prm;
			form.ShowDialog();

			//更新あればリロード
			if (form.IsUpdated == true)
			{
				DataTable updedYoyakuInfoBasicTable = this.getYoyakuInfoBasic(yoyakuKbn, yoyakuNo);

				//■グリッドへの表示
				//グリッド初期化
				this.setInitiarizeGrid();

				//表示用に加工
				DataView updedYoyakuInfoBasicView = this.formatYoyakuInfoBasic(updedYoyakuInfoBasicTable);

				//予約情報（基本）を表示
				this.grdHakkenInfoInquiry.DataSource = updedYoyakuInfoBasicView;
			}
		}

	}

	/// <summary>
	/// 予約情報（基本）の取得　「予約番号が空」
	/// </summary>
	/// <returns></returns>
	private DataTable getYoyakuInfoBasic()
	{
		//エンティティへ値を設定
		object entity = null;
		this.DisplayDataToEntity(ref entity);

		YoyakuInfoBasicEntity yoyakuInfoBasicEntity = TryCast(entity, yoyakuInfoBasicEntity);
		if (ReferenceEquals(yoyakuInfoBasicEntity, null))
		{
			return null;
		}

		//検索条件：ピックアップの有無を設定
		int pickUpFlg = (int)PickUpValue.PickUpSiteiNasi;
		if (this.chkPickUpAri.Checked && !this.chkPickUpNasi.Checked)
		{
			//ピックアップ有のみチェックONの場合True
			pickUpFlg = (int)PickUpValue.PickUpAri;

		}
		else if (this.chkPickUpNasi.Checked && !this.chkPickUpAri.Checked)
		{
			//ピックアップ無のみチェックONの場合False
			pickUpFlg = (int)PickUpValue.PickUpNasi;

		}

		S02_0601Da da = new S02_0601Da();
		DataTable dt = da.getYoyakuInfoHakkenInfoSyokai(yoyakuInfoBasicEntity, pickUpFlg);

		return if (this.existsDatas(dt), dt, null);
	}

	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent"></param>
	public void DisplayDataToEntity(ref object ent)
	{
		//定期/企画
		string teikiKikaku = "";
		if (this.rdoTeiki.Checked)
		{
			teikiKikaku = System.Convert.ToString(Convert.ToInt32(FixedCd.TeikiKikakuKbn.Teiki).ToString());

		}
		else if (this.rdoKikaku.Checked)
		{
			teikiKikaku = System.Convert.ToString(Convert.ToInt32(FixedCd.TeikiKikakuKbn.Kikaku).ToString());
		}

		//出発日
		Date? syuptDay = this.ucoSyuptDay.Value;
		if (ReferenceEquals(syuptDay, null))
		{
			ent = null;
			return;
		}
		string fmtSyuptDay = System.Convert.ToString(syuptDay.ToString().Substring(0, 10).Replace("/", "").Replace("_", ""));
		int intSyuptDay = 0;
		if (!int.TryParse(fmtSyuptDay, out intSyuptDay))
		{
			ent = null; //数値変換失敗
			return;
		}

		//代理店
		string agent = "";
		if (string.IsNullOrWhiteSpace(System.Convert.ToString(this.ucoDairitenName.CodeText)) == false)
		{
			agent = System.Convert.ToString(this.ucoDairitenName.CodeText);

		}

		YoyakuInfoBasicEntity yoyakuInfoBasicEntity = new YoyakuInfoBasicEntity();
		yoyakuInfoBasicEntity.teikiKikakuKbn.Value = teikiKikaku; //定期/企画
		yoyakuInfoBasicEntity.syuptDay.Value = intSyuptDay; //出発日
		yoyakuInfoBasicEntity.name.Value = this.txtNameBf.Text; //名前
		yoyakuInfoBasicEntity.crsCd.Value = this.ucoCrsCd.CodeText; //コースコード
		yoyakuInfoBasicEntity.telNo1.Value = this.txtTelNo.Text; //電話番号
		yoyakuInfoBasicEntity.jyochachiCd1.Value = this.ucoNoribaCd.CodeText; //乗車地コード
		yoyakuInfoBasicEntity.agentCd.Value = agent; //代理店コード

		ent = yoyakuInfoBasicEntity;
	}

	/// <summary>
	/// 予約情報（基本）の取得　「予約番号あり」
	/// </summary>
	/// <param name="yoyakuKbn">予約区分</param>
	/// <param name="yoyakuNo">予約NO</param>
	/// <returns></returns>
	private DataTable getYoyakuInfoBasic(string yoyakuKbn, int yoyakuNo)
	{
		S02_0601Da da = new S02_0601Da();
		DataTable dt = da.getYoyakuKbnNoSyuptDay(;
		yoyakuKbn(,);
		yoyakuNo());

		return if (this.existsDatas(dt), dt, null);
	}

	/// <summary>
	/// 予約情報（基本）の加工
	/// </summary>
	/// <param name="yoyakuInfoBasicTable">予約情報（基本）テーブル</param>
	/// <returns></returns>
	private DataView formatYoyakuInfoBasic(DataTable yoyakuInfoBasicTable)
	{
		object dt = yoyakuInfoBasicTable.AsEnumerable();
		foreach (DataRow row in dt)
		{
			//状態を取得
			string cancelFlg = System.Convert.ToString(TryCast(row.Item("CANCEL_FLG"), string));
			string zasekiFlg = System.Convert.ToString(TryCast(row.Item("ZASEKI_RESERVE_YOYAKU_FLG"), string));
			string hakkenNaiyo = System.Convert.ToString(TryCast(row.Item("HAKKEN_NAIYO"), string));
			string jotai = System.Convert.ToString(TryCast(row.Item("STATE"), string));
			string[] hakkenState = CommonDAUtil.getYoyakuHakkenState(cancelFlg,;
			zasekiFlg(,);
			hakkenNaiyo(,);
			jotai());
			row.Item["HAKKEN_STATE"] = hakkenState[0];

			for (cnt = 1; cnt <= 5; cnt++)
			{
				//乗車地コードの存在を確認
				string jyoshachiCd = System.Convert.ToString(row.Item("JYOCHACHI_CD_{cnt}").ToString());
				if (!jyoshachiCd.Equals(""))
				{
					//出発場所を設定
					string placeName = System.Convert.ToString(row.Item("PLACE_NAME_{cnt}").ToString());
					row.Item["PLACE_NAME"] = placeName;

					//出発時間を設定
					string syuptTime = System.Convert.ToString(row.Item("SYUPT_TIME_{cnt}").ToString().PadLeft(4, '0'));
					row.Item["SYUPT_TIME"] = syuptTime.Substring(0, 2) + ":" + syuptTime.Substring(2, 2);
				}
			}
		}

		//ソート
		DataRow sortedDt = From row DataRow in dt;
		OrderByrow("CRS_NAME"), ;
		row("PLACE_NAME"),;
		row("SYUPT_TIME");

		// 最大件数分のみを応答
		return sortedDt.Take(MaxDispRow).CopyToDataTable.AsDataView();
	}

	/// <summary>
	/// 利用者情報の取得
	/// </summary>
	/// <returns></returns>
	private DataTable getUserMaster(string loginId)
	{
		//TODO:利用者情報の仕様検討中
		//Dim da As New Common.getUserMaster_DA
		//Return da.GetUserMasterData(loginId)
		DataTable da = new DataTable();
		return da;
		//Return If(Me.existsDatas(dt), dt, Nothing)
	}

	/// <summary>
	/// 存在チェック
	/// </summary>
	/// <param name="dt">データテーブル</param>
	/// <returns>存在判定</returns>
	private bool existsDatas(DataTable dt)
	{
		if (ReferenceEquals(dt, null))
		{
			return false;
		}
		if (dt.Rows.Count <= 0)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// グリッドの設定
	/// </summary>
	private void setGrid()
	{
		this.grdHakkenInfoInquiry.AllowDragging = AllowDraggingEnum.None;
		this.grdHakkenInfoInquiry.AllowAddNew = false;
		this.grdHakkenInfoInquiry.AutoGenerateColumns = false;
		this.grdHakkenInfoInquiry.ShowButtons = ShowButtonsEnum.Always;
	}

	/// <summary>
	/// グリッドの初期化
	/// </summary>
	private void setInitiarizeGrid()
	{
		//(Nothingで初期化するとヘッダー名が消える)
		DataTable dt = new DataTable();
		this.grdHakkenInfoInquiry.DataSource = dt;
		this.grdHakkenInfoInquiry.DataMember = "";
		this.grdHakkenInfoInquiry.Refresh();
		this.lblLengthGrd.Text = "0件";
	}

	/// <summary>
	/// ご乗車案内/予約確認書出力処理
	/// </summary>
	/// <param name="outFormKbn">ご乗車案内:1、予約確認書:2</param>
	/// <returns>処理成功フラグ</returns>
	private bool printP02_0601(string outFormKbn)
	{

		P02_0601Output P02_0601Output = new P02_0601Output();
		List paramList = new List(Of P02_0601ParamData);
		DataTable yoyakuInfoBasicTable = null;
		DataTable yoyakuInfoCrsChargeChargeKbnTable = null;
		DataTable HakkenInfoTable = null;

		int rowCnt = System.Convert.ToInt32(grdHakkenInfoInquiry.Rows.Count); //発券情報一覧の行数
		int seikiChargeAllGaku = 0; //正規料金総額
		int addChargeMaebaraiKei = 0; //追加料金前払計

		bool IsStayAri = false; //宿泊の有無

		if (grdHakkenInfoInquiry.Rows.Count <= 1)
		{
			//発券情報一覧にデータが表示されていない場合、エラーとする
			//処理対象データがありません。
			CommonProcess.createFactoryMsg().messageDisp("E90_004");

			return false;
		}

		for (row = 1; row <= rowCnt - 1; row++)
		{

			P02_0601ParamData param = new P02_0601ParamData();
			//未発券判定用 発券内容
			string hakkenNaiyo = null;

			//1行ずつパラメータをセットする
			param.YoyakuKbn = System.Convert.ToString(grdHakkenInfoInquiry.Item(row, NoColYoyakuKbn));
			param.YoyakuNo = System.Convert.ToInt32(grdHakkenInfoInquiry.Item(row, NoColYoyakuNo));

			//予約情報（基本）、予約情報（コース料金_料金区分）、発券情報を取得
			yoyakuInfoBasicTable = CommonHakken.getYoyakuInfoBasic(param.YoyakuKbn, param.YoyakuNo);
			yoyakuInfoCrsChargeChargeKbnTable = CommonHakken.getYoyakuInfoCrsChargeChargeKbn(param.YoyakuKbn, param.YoyakuNo);
			HakkenInfoTable = CommonHakken.getHakkenInfo(param.YoyakuKbn, param.YoyakuNo);

			//未発券判定用に発券内容を取得
			hakkenNaiyo = System.Convert.ToString(if (yoyakuInfoBasicTable.Rows(0).Field(Of string)["HAKKEN_NAIYO"], ""));

		param.OutFormKbn = outFormKbn;

		//座席の表示/非表示
		string zasekiHihyojiFlg = System.Convert.ToString(if (yoyakuInfoBasicTable.Rows(0).Field(Of string)["ZASEKI_HIHYOJI_FLG"], ""));
		this._isShowingZaseki = System.Convert.ToBoolean(if (zasekiHihyojiFlg.Equals("Y"), false, true));

		if (outFormKbn.Equals(CommonHakken.OutFormKbnReservationCertificate))
		{
			//予約確認書は、座席表示なし
			param.IsShowingZaseki = false;
		}
		else if (outFormKbn.Equals(CommonHakken.OutFormKbnBordingInfomation))
		{
			//ご乗車案内は、コースによって座席表示を制御
			param.IsShowingZaseki = this._isShowingZaseki;

			//GMT代理店の場合、座席を表示
			if (this._isGmtDairitenMode == true)
			{
				param.IsShowingZaseki = true;
			}
		}

		param.Name = if (yoyakuInfoBasicTable.Rows(0).Field(Of string)["NAME"], ""); //名前
		param.SexBetu = if (yoyakuInfoBasicTable.Rows(0).Field(Of string)["SEX_BETU"], ""); //性別
		param.CrsCd = if (yoyakuInfoBasicTable.Rows(0).Field(Of string)["CRS_CD"], ""); //コースコード
		param.CrsName = if (yoyakuInfoBasicTable.Rows(0).Field(Of string)["CRS_NAME"], ""); //コース名
		param.SyuptDay = if (yoyakuInfoBasicTable.Rows(0).Field(Of Integer ?)["SYUPT_DAY"], 0); //出発日
		param.SyuptTime = if (yoyakuInfoBasicTable.Rows(0).Field(Of Short ?)["SYUPT_TIME"], 0); //出発時間
		param.Gousya = if (yoyakuInfoBasicTable.Rows(0).Field(Of Short ?)["GOUSYA"], 0); //号車
		param.ZasekiNo = if (yoyakuInfoBasicTable.Rows(0).Field(Of string)["ZASEKI"], ""); //座席

		param.HakkenDay = if (yoyakuInfoBasicTable.Rows(0).Field(Of Integer ?)["HAKKEN_DAY"], 0); //発券日

		if (!(string.IsNullOrEmpty(hakkenNaiyo) == true))
		{
			//未発券の予約でない場合、発券情報の精算方法を取得
			param.SeisanHoho = if (HakkenInfoTable.Rows(0).Field(Of string)["SEISAN_HOHO"], ""); //発券情報.精算方法
		}

		seikiChargeAllGaku = System.Convert.ToInt32(Convert.ToInt32(if (yoyakuInfoBasicTable.Rows(0).Field(Of Decimal ?)["SEIKI_CHARGE_ALL_GAKU"], 0)));
		addChargeMaebaraiKei = System.Convert.ToInt32(Convert.ToInt32(if (yoyakuInfoBasicTable.Rows(0).Field(Of Decimal ?)["ADD_CHARGE_MAEBARAI_KEI"], 0)));
		param.Charge = seikiChargeAllGaku + addChargeMaebaraiKei; //料金

		param.WaribikiKingaku = Convert.ToInt32(if (yoyakuInfoBasicTable.Rows(0).Field(Of Decimal ?)["WARIBIKI_ALL_GAKU"], 0)); //割引金額
		param.CancelRyou = Convert.ToInt32(if (yoyakuInfoBasicTable.Rows(0).Field(Of Decimal ?)["CANCEL_RYOU_KEI"], 0)); //キャンセル料計
		param.Nyukingaku = Convert.ToInt32(if (yoyakuInfoBasicTable.Rows(0).Field(Of Decimal ?)["NYUKINGAKU_SOKEI"], 0)); //入金額
		param.NyuukinSituationKbn = if (yoyakuInfoBasicTable.Rows(0).Field(Of string)["NYUUKIN_SITUATION_KBN"], ""); //入金状況区分

		param.KbnName = new List(Of string);
		param.JininKbnName = new List(Of string);
		param.Ninzu = new List(Of int);
		param.Tanka = new List(Of int);

		if (!(string.IsNullOrWhiteSpace(System.Convert.ToString(if (yoyakuInfoBasicTable.Rows(0).Field(Of string)["CRS_KIND"], ""))) == true))
			{
			//コース種別がNULLでない場合、宿泊有無を取得
			IsStayAri = System.Convert.ToBoolean(CommonCheckUtil.isStay(yoyakuInfoBasicTable.Rows(0).Field(Of string)["CRS_KIND"]));
		}

		if (IsStayAri == false)
		{
			//宿泊なしの場合
			for (int i = 0; i <= yoyakuInfoCrsChargeChargeKbnTable.Rows.Count - 1; i++)
			{
				//取得した予約情報（コース料金_料金区分）のデータをパラメータにセットする
				param.KbnName.Add(if (yoyakuInfoCrsChargeChargeKbnTable.Rows(i).Field(Of string)["CHARGE_NAME"], " "));
			param.JininKbnName.Add(if (yoyakuInfoCrsChargeChargeKbnTable.Rows(i).Field(Of string)["CHARGE_KBN_JININ_NAME"], " "));
			param.Ninzu.Add(Convert.ToInt32(if (yoyakuInfoCrsChargeChargeKbnTable.Rows(i).Field(Of Decimal ?)["CHARGE_APPLICATION_NINZU_1"], 0)));
			param.Tanka.Add(Convert.ToInt32(if (yoyakuInfoCrsChargeChargeKbnTable.Rows(i).Field(Of Decimal ?)["TANKA_1"], 0)));
		}
	}
			else
			{
				//宿泊ありの場合
				DataTable dt = CommonHakken.formatGrdChargeKbnStayAri(yoyakuInfoCrsChargeChargeKbnTable);
				
				for (int i = 0; i <= dt.Rows.Count - 1; i++)
				{
					//取得した予約情報（コース料金_料金区分）のデータをパラメータにセットする
					param.KbnName.Add(if(dt.Rows(i).Field(Of string)["ROOM_TYPE"], " "));
					param.JininKbnName.Add(if(dt.Rows(i).Field(Of string)["CHARGE_KBN_JININ_NAME"], " "));
					param.Ninzu.Add(if(dt.Rows(i).Field(Of Integer?)["CHARGE_APPLICATION_NINZU"], 0));
					param.Tanka.Add(if(dt.Rows(i).Field(Of Integer?)["TANKA"], 0));
				}
			}
			
			paramList.Add(param);
		}
		
		//一覧のデータを基に、帳票生成処理を開始
		SectionReport rpt = P02_0601Output.OutputP02_0601(paramList);

//印刷処理を開始。印刷確認画面を表示
rpt.Document.Print(true, true, false);
return true;
		
	}
	
	/// <summary>
	/// GMT代理店名の取得
	/// </summary>
	private void setGmt()
{
	//DBパラメータ
	Hashtable paramInfoList = new Hashtable();
	//DataAccessクラス生成
	Agent_DA dataAccess = new Agent_DA();

	//パラメータ設定
	//代理店コード
	paramInfoList.Add("AgentCd", strDairitenCdGMT);
	//検索処理
	DataTable returnValue = dataAccess.getAgentExactMatchAgentCd(paramInfoList);

	if (returnValue.Rows.Count == 1)
	{
		this.ucoDairitenName.CodeText = returnValue.Rows(0)["AGENT_CD"].ToString();
		this.ucoDairitenName.ValueText = returnValue.Rows(0)["AGENT_NAME"].ToString();
	}
	else
	{
		this.ucoDairitenName.CodeText = strDairitenCdGMT;
		this.ucoDairitenName.ValueText = string.Empty;
	}
}

/// <summary>
/// GMT代理店コードの取得
/// </summary>
/// <returns>GMTの代理店コード</returns>
private string getGmtCd()
{
	//DataAccessクラス生成

	S02_0601Da da = new S02_0601Da();
	DataTable dt = new DataTable();

	//コードマスタからGMTの代理店コードを取得
	dt = da.getGmtCd(gmtCdBunrui, gmtCdValue);

	if (dt.Rows.Count > 0)
	{
		//データが取得できていた場合、GMTの代理店コードを返す
		return if (dt.Rows(0).Field(Of string)["NAIYO_1"], "");
		}
		else
{
	return string.Empty;
}
		
	}
	
#region 未使用のImplementsメソッド
	/// <summary>
	/// 初期処理(必須画面個別実装)
	/// </summary>
	public void setSeFirsttDisplayData()
{
	//初期表示なし
}

/// <summary>
/// エンティティから画面に設定する処理(必須画面個別実装)
/// </summary>
/// <param name="ent"></param>
public void EntityDataToDisplay(ref object ent)
{
	//登録処理なし
	throw (new Exception());
}

/// <summary>
/// DataGridからエンティティ(前回値)に設定する処理(必須画面個別実装)
/// ※DataGrid上の1レコードから関連するデータも取得する。Keyがない場合などは未対応
/// </summary>
/// <param name="pDataRow"></param>
public void OldDataToEntity(DataRow pDataRow)
{
	//登録処理なし
	throw (new Exception());
}

/// <summary>
///
/// </summary>
/// <returns></returns>
public bool CheckSearch()
{
	//検索条件が分かれるため個別実装
	throw (new Exception());
}

/// <summary>
/// 登録処理前のチェック処理(必須画面個別実装)
/// </summary>
/// <returns></returns>
public bool CheckInsert()
{
	//登録処理なし
	throw (new Exception());
}

/// <summary>
/// 更新処理前のチェック処理(必須画面個別実装)
/// </summary>
/// <returns></returns>
public bool CheckUpdate()
{
	//更新処理なし
	throw (new Exception());
}

/// <summary>
/// 必須入力項目エラーチェック(登録時／更新時)
/// </summary>
/// <returns></returns>
public bool isExistHissuError()
{
	//登録処理なし
	throw (new NotImplementedException());
}
	
#endregion
#endregion
}