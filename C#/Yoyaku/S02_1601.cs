using System.ComponentModel;


/// <summary>
/// コース一覧照会（事後予約登録）
/// </summary>
public class S02_1601 : PT11, iPT11
{

	#region 定数

	/// <summary>
	/// 画面ID
	/// </summary>
	private const string ScreenId = "S02_1601";
	/// <summary>
	/// 画面名
	/// </summary>
	private const string ScreenName = "コース一覧照会（事後予約登録）";
	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	private const int TopGbxCondition = 41;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	private const int MarginGbxCondition = 6;
	/// <summary>
	/// コースコードFull桁指定（True=指定,False=指定なし）
	/// </summary>
	private bool blnCourseAssigne = false;
	/// <summary>
	/// 検索結果最大表示件数件数
	/// </summary>
	private const int MaxKensu = 100;
	/// <summary>
	/// 区分値 'Y','N'
	/// </summary>
	private const string KbnValueY = "Y";
	private const string KbnValueN = "N";
	/// <summary>
	/// コースコードMAXサイズ
	/// </summary>
	private const int CrsCdMax = 6;
	/// <summary>
	/// 乗り場MAXサイズ
	/// </summary>
	private const int NoribaCdMax = 3;
	/// <summary>
	/// 定数：状態の表示内容
	/// </summary>
	private const string JOTAI_KAKUTEI = "催行確定";
	private const string JOTAI_UKEMAE = "受付開始前";
	private const string JOTAI_UNKYU = "運休";
	private const string JOTAI_TYUSI = "催行中止";
	private const string JOTAI_FUKA = "予約不可";
	private const string JOTAI_TEJIMAI = "手仕舞済";
	private const string JOTAI_MANSEKI = "満席";
	private const string JOTAI_UKESTOP = "予約受付停止";

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
	/// 画面初期表示時のメソッド
	/// </summary>
	protected override void initScreenPerttern()
	{

		// 各種値の設定

		// 画面ID/名称の設定
		base.setFormId = ScreenId;
		base.setTitle = ScreenName;

		// 画面終了時確認不要
		base.closeFormFlg = false;

		// PT11から変更なしの為、そのまま実行
		base.initScreenPerttern();

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
			this.btnVisiblerCondition.Text = "非表示 <<";

			this.PanelEx1.Top = TopGbxCondition + this.HeightGbxCondition + MarginGbxCondition;
			this.PanelEx1.Height -= this.HeightGbxCondition + MarginGbxCondition;
			this.grdCrsListInquiry.Height -= this.HeightGbxCondition - 3;

		}
		else
		{
			//非表示状態
			this.btnVisiblerCondition.Text = "表示 >>";

			this.PanelEx1.Top = TopGbxCondition;
			this.PanelEx1.Height += this.HeightGbxCondition + MarginGbxCondition;
			this.grdCrsListInquiry.Height += this.HeightGbxCondition - 3;
		}
	}

	/// <summary>
	/// 条件クリアボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
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
		initScreenPerttern();

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
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理");

	}

	/// <summary>
	/// F8：検索ボタン押下時イベント（実処理）
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{

		// PT11に記載された処理の想定が分からないので元の処理は実行しない
		//MyBase.btnF8_ClickOrgProc()

		// 検索条件チェック
		if (checkSearchItems() == true)
		{

			// 検索及びデータの編集
			setCrsListInquiry();

			// フォーカスセット
			this.ActiveControl = this.btnSearch;

		}

	}

	/// <summary>
	/// キーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void S02_1601_KeyDown(object sender, KeyEventArgs e)
	{

		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			e.Handled = true;
			this.btnSearch_Click(sender, e);
		}
		else
		{
			return;
		}

	}

	/// <summary>
	/// 総数内訳詳細表示/非表示押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnAllNumUtiwakeDisplay_Click(object sender, EventArgs e)
	{

		if (btnAllNumUtiwakeDisplay.Text.Equals("総数内訳詳細表示"))
		{
			btnAllNumUtiwakeDisplay.Text = "総数内訳詳細非表示";
			grdCrsListInquiry.Cols(12).Visible = true;
			grdCrsListInquiry.Cols(13).Visible = true;
			grdCrsListInquiry.Cols(14).Visible = true;
		}
		else
		{
			btnAllNumUtiwakeDisplay.Text = "総数内訳詳細表示";
			grdCrsListInquiry.Cols(12).Visible = false;
			grdCrsListInquiry.Cols(13).Visible = false;
			grdCrsListInquiry.Cols(14).Visible = false;
		}

	}

	/// <summary>
	/// グリッド内予約ボタン押下時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdCrsListInquiry_CellButtonClick(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
	{

		DataRow selectedRow = null;
		DataTable dt = null;

		// 該当データを取得
		dt = (DataTable)grdCrsListInquiry.DataSource;
		selectedRow = dt.Rows(e.Row - 2);


		//使用中チェック
		CrsLedgerBasic_DA flgrelation = new CrsLedgerBasic_DA();
		DataTable check = flgrelation.getUsingFlg(selectedRow["colCrsCd"].ToString(),;
		selectedRow["colSyuptDayHide"].ToString(),;
		selectedRow["colGousya"].ToString());
		// 使用中フラグ取得
		string usingFlg = System.Convert.ToString(TryCast(check.Rows(0).Item(0), string));
		if (usingFlg IsNot null && usingFlg == FixedCd.UsingFlg.Use)
		{
			//現在このコースは他の端末で使用中です。
			CommonProcess.createFactoryMsg().messageDisp("E90_050");
			return;
		}


		// 事後予約登録画面に遷移
		using (S02_1602 form = new S02_1602())
		{
			// 画面遷移パラメータセット
			S02_1602ParamData prm = new S02_1602ParamData();
			prm.ScreenMode = "1"; // 新規登録
			prm.CrsKind = selectedRow["colCRS_KIND"].ToString().Trim(); // コース種別
			prm.CrsCd = selectedRow["colCrsCd"].ToString().Trim(); // コースコード
			prm.SyuptDay = System.Convert.ToInt32(selectedRow["colSyuptDayHide"].ToString().Trim()); // 出発日
			prm.Gousya = System.Convert.ToInt32(selectedRow["colGousya"].ToString().Trim()); // 号車
			prm.YoyakuKbn = string.Empty;
			prm.YoyakuNo = 0;
			form.ParamData = prm;
			// 画面表示
			form.ShowDialog();
		}



	}

	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{

		//MyBase.btnF2_ClickOrgProc()

		closeCheckFlg = false;
		base.closeFormFlg = false;
		this.Close();

	}

	/// <summary>
	/// 画面終了時処理
	/// </summary>
	/// <returns></returns>
	protected override bool closingScreen()
	{

		// 差分チェック不要なので、元処理は実行しない
		//Return MyBase.closingScreen()
		return false;

	}


	/// <summary>
	/// 画面終了時処理
	/// </summary>
	private void S02_1601_FormClosing()
	{

		this.Dispose();

	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void FlexGridEx1_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{

		//データ件数を表示(ヘッダー行分マイナス2(セル結合分))
		string formatedCount = System.Convert.ToString((this.grdCrsListInquiry.Rows.Count - 2).ToString().PadLeft(6));
		this.lblLengthGrd.Text = formatedCount + "件";

	}


	#endregion

	#region メソッド

	/// <summary>
	/// 検索条件部分初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{

		base.initSearchAreaItems();

		//------------------------------
		// コントロール初期化
		//------------------------------
		//条件GroupBox内のテキストボックス初期化
		object textBoxs = this.gbxCondition.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
			textBox.ExistError = false;
		}

		//コードコントロール初期化
		object CodeControls = this.gbxCondition.Controls.OfType(Of Master.CodeControlEx);
		foreach ( in CodeControls)
		{
			CodeControl.CodeText = "";
			CodeControl.ValueText = "";
			CodeControl.ExistError = false;
		}

		//チェックボックス初期化
		object CheckBoxs = this.gbxCondition.Controls.OfType(Of CheckBoxEx);
		foreach ( in CheckBoxs)
		{
			CheckBox.Checked = false;
			CheckBox.ExistError = false;
		}

		// 日付の初期化
		this.dtmSyuptDay.ExistError = false;

		//------------------------------
		// コンボボックスの設定
		//------------------------------
		//バスタイプの初期化
		//Me.SetComboBoxCarKind(Me.cmbBusType, True)
		this.SetComboBoxCode(System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.busType)), this.cmbBusType, true);
		this.cmbBusType.SelectedValue = null;
		//言語の初期化
		this.SetComboBoxCode(System.Convert.ToString(FixedCdYoyaku.CodeBunruiTypeGuideGengoValue), this.cmbLanguage, true);
		this.cmbLanguage.SelectedValue = null;

		//------------------------------
		// その他初期値設定
		//------------------------------
		// 出発日
		this.dtmSyuptDay.Value = CommonDateUtil.getSystemTime();

		//所属部署が国際事業部の場合、外国語にチェック
		if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
		{
			// 外国語がデフォルトの場合
			this.chkGaikokugoCrs.Checked = true;
			this.chkJapaneseCrs.Checked = false;
		}
		else
		{
			// 日本語がデフォルトの場合
			this.chkJapaneseCrs.Checked = true;
			this.chkGaikokugoCrs.Checked = false;
		}


		// 宿泊パックの現行仕様は廃止の為コメント
		//Dim dataAccess As New S02_0101Da 'DataAccessクラス生成
		//Dim paramInfoList As New Hashtable 'DBパラメータ
		//'Single,Twinコースコード設定
		//paramInfoList.Clear()
		//paramInfoList.Add("KEY_1", SingleCrsCdKey1)
		//paramInfoList.Add("KEY_2", SingleCrsCdKey2)
		//returnValue = dataAccess.GetCrsControlInfo(paramInfoList)
		//If returnValue.Rows.Count > 0 Then
		//    'Singleコースコード設定
		//    SingleCrsCd = returnValue.Rows(0).Item("KEY_3").ToString.Trim
		//End If

		//paramInfoList.Clear()
		//paramInfoList.Add("KEY_1", TwinCrsCdKey1)
		//paramInfoList.Add("KEY_2", TwinCrsCdKey2)
		//returnValue = dataAccess.GetCrsControlInfo(paramInfoList)
		//If returnValue.Rows.Count > 0 Then
		//    'Twinコースコード設定
		//    TwinCrsCd = returnValue.Rows(0).Item("KEY_3").ToString.Trim
		//End If
		//dataAccess = Nothing
		//paramInfoList = Nothing

	}

	/// <summary>
	/// 検索結果部分初期化
	/// </summary>
	protected override void initDetailAreaItems()
	{

		base.initDetailAreaItems();

		// 行ヘッダを作成。
		grdCrsListInquiry.AutoResize = false;
		grdCrsListInquiry.Styles.Normal.WordWrap = true;
		grdCrsListInquiry.Cols(13).Style.WordWrap = true;
		grdCrsListInquiry.Cols.Count = 22;
		grdCrsListInquiry.Rows.Fixed = 2;
		grdCrsListInquiry.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.FixedOnly;
		grdCrsListInquiry.Rows(0).AllowMerging = true;
		grdCrsListInquiry.Cols(0).AllowMerging = true;
		C1.Win.C1FlexGrid.CellRange cr = null;

		// 予約
		cr = grdCrsListInquiry.GetCellRange(0, 1, 1, 1);
		grdCrsListInquiry.MergedRanges.Add(cr);
		// 出発日
		cr = grdCrsListInquiry.GetCellRange(0, 2, 1, 2);
		grdCrsListInquiry.MergedRanges.Add(cr);
		// 時間
		cr = grdCrsListInquiry.GetCellRange(0, 3, 1, 3);
		grdCrsListInquiry.MergedRanges.Add(cr);
		// コースコード
		cr = grdCrsListInquiry.GetCellRange(0, 4, 1, 4);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//コース名
		cr = grdCrsListInquiry.GetCellRange(0, 5, 1, 5);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//状況
		cr = grdCrsListInquiry.GetCellRange(0, 6, 1, 6);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//乗り場経由
		cr = grdCrsListInquiry.GetCellRange(0, 7, 1, 7);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//号車
		cr = grdCrsListInquiry.GetCellRange(0, 8, 1, 8);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//空席数
		cr = grdCrsListInquiry.GetCellRange(0, 9, 1, 9);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//空席数(補助席)
		cr = grdCrsListInquiry.GetCellRange(0, 10, 1, 10);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//総数内訳
		cr = grdCrsListInquiry.GetCellRange(0, 11, 1, 11);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//内訳
		cr = grdCrsListInquiry.GetCellRange(0, 12, 0, 14);
		cr.Data = "内訳";
		grdCrsListInquiry.MergedRanges.Add(cr);
		grdCrsListInquiry[1, 12] = "予約";
		grdCrsListInquiry[1, 13] = "ブロック";
		grdCrsListInquiry[1, 14] = "空席確保";
		//'宿泊室数
		cr = grdCrsListInquiry.GetCellRange(0, 15, 0, 20);
		cr.Data = "宿泊室数";
		grdCrsListInquiry.MergedRanges.Add(cr);
		grdCrsListInquiry[1, 15] = "１Ｒ";
		grdCrsListInquiry[1, 16] = "２Ｒ";
		grdCrsListInquiry[1, 17] = "３Ｒ";
		grdCrsListInquiry[1, 18] = "４Ｒ";
		grdCrsListInquiry[1, 19] = "５Ｒ";
		grdCrsListInquiry[1, 20] = "総";
		//バスタイプ
		cr = grdCrsListInquiry.GetCellRange(0, 21, 1, 21);
		grdCrsListInquiry.MergedRanges.Add(cr);

		// 初期として0行表示
		grdCrsListInquiry.DataSource = new DataTable();
		grdCrsListInquiry.Refresh();

	}

	/// <summary>
	/// フッタボタン初期化
	/// </summary>
	protected override void initFooterButtonControl()
	{

		// PT11側と異なるので、元処理実行なしで設定
		//MyBase.initFooterButtonControl()

		// 表示文言設定
		this.F1Key_Text = string.Empty;
		this.F2Key_Text = "F2:戻る";
		this.F3Key_Text = string.Empty;
		this.F4Key_Text = string.Empty;
		this.F5Key_Text = string.Empty;
		this.F6Key_Text = string.Empty;
		this.F7Key_Text = string.Empty;
		this.F8Key_Text = string.Empty;
		this.F9Key_Text = string.Empty;
		this.F10Key_Text = string.Empty;
		this.F11Key_Text = string.Empty;
		this.F12Key_Text = string.Empty;

		// 表示/非表示設定
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

		// 活性/非活性設定
		this.F1Key_Enabled = false;
		this.F2Key_Enabled = true;
		this.F3Key_Enabled = false;
		this.F4Key_Enabled = false;
		this.F5Key_Enabled = false;
		this.F6Key_Enabled = false;
		this.F7Key_Enabled = false;
		this.F8Key_Enabled = false;
		this.F9Key_Enabled = false;
		this.F10Key_Enabled = false;
		this.F11Key_Enabled = false;
		this.F12Key_Enabled = false;

	}

	/// <summary>
	/// 検索前チェック処理
	/// </summary>
	/// <returns></returns>
	protected override bool checkSearchItems()
	{

		//Return MyBase.checkSearchItems()

		return CheckSearch();

	}

	/// <summary>
	/// 検索前チェック処理（個別実装必須）
	/// </summary>
	/// <returns></returns>
	public bool CheckSearch()
	{

		DateTime dteTmpDate = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		object CheckBoxs = this.gbxCondition.Controls.OfType(Of CheckBoxEx);

		//前回エラークリア
		this.dtmSyuptDay.ExistError = false;
		this.ucoCrsCd.ExistError = false;
		this.ucoNoribaCd.ExistError = false;
		object CodeControls = this.gbxCondition.Controls.OfType(Of Master.CodeControlEx);
		foreach ( in CodeControls)
		{
			CodeControl.ExistError = false;
		}
		foreach ( in CheckBoxs)
		{
			CheckBox.ExistError = false;
		}

		//------------------------------
		// 出発日のチェック
		//------------------------------
		if (this.dtmSyuptDay.Value.HasValue == true)
		{

			// 現在時刻の取得
			dteTmpDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime);

			// 未来日付の場合NG
			if (this.dtmSyuptDay.Value.Value.ToShortDateString() > dteTmpDate.ToShortDateString())
			{
				// メッセージ：未来日付は指定できません
				CommonProcess.createFactoryMsg().messageDisp("E90_006", "未来日", "指定");
				// エラー設定&フォーカス設定
				this.dtmSyuptDay.ExistError = true;
				this.ActiveControl = this.dtmSyuptDay;
				return false;
			}
			else if (this.dtmSyuptDay.Value.Value.ToShortDateString() < dteTmpDate.ToShortDateString())
			{
				// 過去日付を指定可能なのは特定権限（業務管理課）のみ
				if (UserInfoManagement.sectionBusyoCd.Substring(0, 5) != FixedCd.SectionBusyoCd.GyomuKanrika)
				{
					// メッセージ：過去日付を指定できません
					CommonProcess.createFactoryMsg().messageDisp("E90_013");
					// エラー設定&フォーカス設定
					this.dtmSyuptDay.ExistError = true;
					this.ActiveControl = this.dtmSyuptDay;
					return false;
				}

			}
		}
		else
		{
			if (string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)) == true)
			{
				// 出発日は必須入力です
				CommonProcess.createFactoryMsg().messageDisp("E90_023", "出発日");
				// エラー設定&フォーカス設定
				this.dtmSyuptDay.ExistError = true;
				this.ActiveControl = this.dtmSyuptDay;
				return false;
			}
		}

		//------------------------------
		// コースコード、コース区分
		//------------------------------
		bool kubunCheck = false;

		// 同時指定は不可
		if (string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)) == false)
		{

			kubunCheck = false;

			if (this.chkCapital.Checked == true)
			{
				kubunCheck = true;
				this.chkCapital.ExistError = true;
				this.ActiveControl = this.chkCapital;
			}
			if (this.chkTeikiKogai.Checked == true)
			{
				kubunCheck = true;
				this.chkTeikiKogai.ExistError = true;
				this.ActiveControl = this.chkTeikiKogai;
			}
			if (this.chkTeikiNight.Checked == true)
			{
				kubunCheck = true;
				this.chkTeikiNight.ExistError = true;
				this.ActiveControl = this.chkTeikiNight;
			}
			if (this.chkTeikiNoon.Checked == true)
			{
				kubunCheck = true;
				this.chkTeikiNoon.ExistError = true;
				this.ActiveControl = this.chkTeikiNoon;
			}

			if (kubunCheck == true)
			{
				// メッセージ：コースコードとコース区分は同時に指定できません
				CommonProcess.createFactoryMsg().messageDisp("E90_047", "コースコードとコース区分は同時に指定");
				return false;
			}

		}

		// コースコードのチェック
		blnCourseAssigne = false;
		if (string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)) == false)
		{
			this.ucoCrsCd.CodeText = this.ucoCrsCd.CodeText.ToUpper();
		}
		if (string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)) == false &&)
		{
			this.ucoCrsCd.CodeText.Length = CrsCdMax;
			blnCourseAssigne = true;
			// コースコードフル桁入力の場合、存在チェック
			if (YoyakuBizCommon.existsCrsCd(this.ucoCrsCd.CodeText) == false)
			{
				//
				CommonProcess.createFactoryMsg().messageDisp("E90_059", "コースコード");
				// エラー設定&フォーカス設定
				this.ucoCrsCd.ExistError = true;
				this.ActiveControl = this.ucoCrsCd;
				return false;
			}
		}

		//「キャピタル」が指定されていて、それ以外のコース区分も指定されている場合
		if (this.chkCapital.Checked == true)
		{

			kubunCheck = false;

			if (this.chkTeikiKogai.Checked == true)
			{
				kubunCheck = true;
				this.chkTeikiKogai.ExistError = true;
				this.ActiveControl = this.chkTeikiKogai;
			}
			if (this.chkTeikiNight.Checked == true)
			{
				kubunCheck = true;
				this.chkTeikiNight.ExistError = true;
				this.ActiveControl = this.chkTeikiNight;
			}
			if (this.chkTeikiNoon.Checked == true)
			{
				kubunCheck = true;
				this.chkTeikiNoon.ExistError = true;
				this.ActiveControl = this.chkTeikiNoon;
			}

			if (kubunCheck == true)
			{
				// キャピタルの場合、他コース区分は選択できません
				CommonProcess.createFactoryMsg().messageDisp("E90_047", "キャピタルの場合、他コース区分は選択");
				return false;
			}

		}

		//------------------------------
		// 乗り場
		//------------------------------
		// 存在チェック(full桁指定時)
		if (string.IsNullOrEmpty(System.Convert.ToString(this.ucoNoribaCd.CodeText)) == false &&)
		{
			this.ucoNoribaCd.CodeText.Length = NoribaCdMax;
			if (YoyakuBizCommon.existsNoribaCd(this.ucoNoribaCd.CodeText) == false)
			{
				//コードがマスタに存在しません。
				CommonProcess.createFactoryMsg().messageDisp("E90_014", "乗り場コード");
				// エラー設定&フォーカス設定
				this.ucoNoribaCd.ExistError = true;
				this.ActiveControl = this.ucoNoribaCd;
				return false;
			}

		}

		// 正常終了
		return true;

	}

	/// <summary>
	/// 一覧用データ取得とグリッド表示値の設定
	/// </summary>
	private void setCrsListInquiry()
	{

		DateTime dteTmpDate = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		string strFromDate = string.Empty;
		string strToDate = string.Empty;
		string strSysDate = string.Empty;
		// DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		object dt = createBaseDataTable();

		//グリッド初期化
		this.grdCrsListInquiry.DataSource = new DataTable();

		// 現在時刻を設定
		strSysDate = System.Convert.ToString(CommonDateUtil.getSystemTime.ToString("yyyyMMdd"));


		//------------------------------
		// 検索条件のパラメータ設定
		//------------------------------
		//paramInfoList.Add("SingleCrsCd", SingleCrsCd)  ' 宿泊パック廃止
		//paramInfoList.Add("TwinCrsCd", TwinCrsCd)      ' 宿泊パック廃止
		//出発日
		paramInfoList.Add("SyuptDay", if (ReferenceEquals(this.dtmSyuptDay.Value, null), string.Empty, this.dtmSyuptDay.Value.Value.ToString("yyyyMMdd")));
		//paramInfoList.Add("SyuptDayTo", Me.dtmSyuptDay.Value.Value.ToString("yyyyMMdd"))
		//コースコード
		paramInfoList.Add("CrsCd", Strings.Trim(System.Convert.ToString(this.ucoCrsCd.CodeText)));
		paramInfoList.Add("CrsCdAssigne", blnCourseAssigne); //コースコード検索用（指定コードFull桁判定）
															 //ガイド言語
		if (ReferenceEquals(cmbLanguage.SelectedValue, null) == false)
		{
			paramInfoList.Add("GuidoGengo", Strings.Trim(System.Convert.ToString(cmbLanguage.SelectedValue.ToString())));
		}
		else
		{
			paramInfoList.Add("GuidoGengo", "");
		}
		//フリーワード
		paramInfoList.Add("FreeWord", Strings.Trim(System.Convert.ToString(this.txtFreeWord.Text)));
		paramInfoList.Add("CodeBunrui", FixedCdYoyaku.CodeBunruiTypeCategory); //フリーワード検索用（カテゴリ）
																			   //乗り場
		paramInfoList.Add("NoribaCd", Strings.Trim(System.Convert.ToString(ucoNoribaCd.CodeText)));
		//バスタイプ
		if (ReferenceEquals(cmbBusType.SelectedValue, null) == false)
		{
			paramInfoList.Add("BusType", Strings.Trim(System.Convert.ToString(this.cmbBusType.SelectedValue.ToString())));
		}
		else
		{
			paramInfoList.Add("BusType", "");
		}
		//空席数
		//paramInfoList.Add("KusekiNum", Trim(Me.txtKusekiNum.Text))
		//予約可能のみ
		//paramInfoList.Add("YoyakuKanouOnly", Trim(Me.chkYoyakuKanouOnly.Checked.ToString))
		//宿泊一人参加のみ
		//paramInfoList.Add("Stay1NinSanka", Trim(Me.chkStay1NinSanka.Checked.ToString))
		//日本語コース
		paramInfoList.Add("JapaneseCrs", Strings.Trim(System.Convert.ToString(this.chkJapaneseCrs.Checked.ToString())));
		//外国語コース
		paramInfoList.Add("GaikokugoCrs", Strings.Trim(System.Convert.ToString(this.chkGaikokugoCrs.Checked.ToString())));
		//定期（昼）
		paramInfoList.Add("TeikiNoon", Strings.Trim(System.Convert.ToString(this.chkTeikiNoon.Checked.ToString())));
		//定期（夜）
		paramInfoList.Add("TeikiNight", Strings.Trim(System.Convert.ToString(this.chkTeikiNight.Checked.ToString())));
		//定期（郊外）
		paramInfoList.Add("TeikiKogai", Strings.Trim(System.Convert.ToString(this.chkTeikiKogai.Checked.ToString())));
		//キャピタル
		paramInfoList.Add("Capital", Strings.Trim(System.Convert.ToString(this.chkCapital.Checked.ToString())));

		S02_1601Da dataAccess = new S02_1601Da();
		DataTable dataCRS_LEDGER = null;

		//------------------------------
		// データを取得
		//------------------------------
		dataCRS_LEDGER = dataAccess.AccessCourseDaityo(S02_1601Da.AccessType.courseByHeaderKey, paramInfoList);

		if (ReferenceEquals(dataCRS_LEDGER, null) || dataCRS_LEDGER.Rows.Count == 0)
		{
			// 取得件数0件の場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_019");
			return;
		}


		DataRow drX = null;
		int cntRowCnt = 0;

		// 取得件数分処理
		foreach (DataRow dr in dataCRS_LEDGER.Rows)
		{
			cntRowCnt++;
			if (cntRowCnt > MaxKensu)
			{
				break;
			}

			drX = dt.NewRow;
			for (int ii = 0; ii <= dr.ItemArray.Count - 1; ii++)
			{
				drX[ii] = dr[ii];

				// 出発日フォーマット変換
				if (ii == 2)
				{
					drX[ii] = CommonRegistYoyaku.convertDateFormat(dr[ii].ToString(), CommonRegistYoyaku.DateFromatDayOfTheWeek);
				}

				// 状態の設定
				if (ii == 6)
				{
					//催行確定区分 = 'Y' の場合
					if (dr["SAIKOU_KAKUTEI_KBN"].ToString().Trim() == KbnValueY)
					{
						drX[ii] = JOTAI_KAKUTEI; //"催行確定"
					}

					// コース台帳（基本）.受付開始日 ＞ システム日付の場合
					if (dr["UKETUKE_START_DAY"].ToString().Trim() > strSysDate)
					{
						drX[ii] = JOTAI_UKEMAE; //"受付開始前"
					}
					//予約停止フラグ ≠ ブランク　の場合
					if (!string.IsNullOrEmpty(System.Convert.ToString(dr["YOYAKU_STOP_FLG"].ToString().Trim())))
					{
						drX[ii] = JOTAI_UKESTOP; //"予約受付停止"
					}
					//運休区分 = 'Y' の場合、'コース種別、コース区分２で
					if (dr["UNKYU_KBN"].ToString().Trim() == KbnValueY)
					{
						if ((((string)(dr["CRS_KIND"].ToString().Trim()) == "1") || ((string)(dr["CRS_KIND"].ToString().Trim()) == "2")) || ((string)(dr["CRS_KIND"].ToString().Trim()) == "3"))
						{
							drX[ii] = JOTAI_UNKYU; //"運休"
												   // Rコースはコース種別（CRS_KIND）が6の為、この処理は通らない
							if (System.Convert.ToInt32(dr["CRS_KBN_2"]) == crsKbn2Type.RCrs)
							{
								drX[ii] = JOTAI_TYUSI; //"催行中止"
							}
						}
						else if (((string)(dr["CRS_KIND"].ToString().Trim()) == "4") || ((string)(dr["CRS_KIND"].ToString().Trim()) == "5"))
						{
							drX[ii] = "";
						}
					}
					//催行確定区分 = 'N' の場合
					if (dr["SAIKOU_KAKUTEI_KBN"].ToString().Trim() == "N")
					{
						drX[ii] = JOTAI_TYUSI; //"催行中止"
					}
					//手仕舞区分 = 'Y' で催行確定区分 ≠ 'N' の場合
					if (dr["TEJIMAI_KBN"].ToString().Trim() == KbnValueY && dr["SAIKOU_KAKUTEI_KBN"].ToString().Trim() != KbnValueN)
					{
						drX[ii] = JOTAI_TEJIMAI; //    "手仕舞済"
					}
					//'空席確保数 = 0 の場合
					//If IsNumeric(dr("KUSEKI_KAKUHO_NUM")) AndAlso CInt(dr("KUSEKI_KAKUHO_NUM")) = 0 Then
					//    drX(ii) = JOTAI_MANSEKI     '"満席"
					//End If
					if (((System.Convert.ToInt32(dr["KUSEKI_NUM_TEISEKI"])) + System.Convert.ToInt32(dr["KUSEKI_NUM_SUB_SEAT"])) == 0)
					{
						drX[ii] = JOTAI_MANSEKI; //"満席"
					}
				}

			}
			dt.Rows.Add(drX);
		}

		// 検索結果にセット
		grdCrsListInquiry.DataSource = dt;

		// グリッド活性/非活性切替
		object with_1 = grdCrsListInquiry;
		for (cntRowCnt = 2; cntRowCnt <= with_1.Rows.Count - 1; cntRowCnt++) //
		{
			// 予約受付停止、運休、催行中止の場合、該当行を編集不可

			{

				with_1.Rows(cntRowCnt).Item(7).ToString = JOTAI_TYUSI;

				with_1.Rows(cntRowCnt).AllowEditing = false;
			}
		}

		// 件数オーバー時はメッセージ表示
		if (dataCRS_LEDGER.Rows.Count > MaxKensu)
		{
			// 取得件数が設定件数より多い場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が");
		}

	}

	/// <summary>
	/// コンボボックスのデータ設定
	/// </summary>
	private void SetComboBoxCarKind(ComboBoxEx ComboName, bool NullRecord)
	{

		//パラメータ
		Hashtable paramInfoList = new Hashtable();

		//DataAccessクラス生成
		S02_1601Da dataAccess = new S02_1601Da();

		//戻り値
		DataTable returnValue = null;

		returnValue = dataAccess.GetCarKindMasterDataCodeData(NullRecord);
		ComboName.DataSource = returnValue;
		ComboBoxEx with_1 = ComboName;
		with_1.ListColumns(ComboBoxCdType.CODE_NAME).Visible = true;
		with_1.ListColumns(ComboBoxCdType.CODE_NAME).Width = with_1.Width;
		with_1.ListColumns(ComboBoxCdType.CODE_VALUE).Visible = false;
		with_1.TextSubItemIndex = ComboBoxCdType.CODE_NAME;
		with_1.ValueSubItemIndex = ComboBoxCdType.CODE_VALUE;

	}

	/// <summary>
	/// コンボボックスのデータ設定
	/// </summary>
	private void SetComboBoxCode(string CdBunruiType, ComboBoxEx ComboName, bool NullRecord)
	{

		//パラメータ
		Hashtable paramInfoList = new Hashtable();

		//DataAccessクラス生成
		Code_DA dataAccess = new Code_DA();

		//戻り値
		DataTable returnValue = null;

		returnValue = dataAccess.GetCodeMasterData(CdBunruiType,, true);
		ComboName.DataSource = returnValue;
		ComboBoxEx with_1 = ComboName;
		with_1.ListColumns(ComboBoxCdType.CODE_NAME).Visible = true;
		with_1.ListColumns(ComboBoxCdType.CODE_NAME).Width = with_1.Width;
		with_1.ListColumns(ComboBoxCdType.CODE_VALUE).Visible = false;
		with_1.TextSubItemIndex = ComboBoxCdType.CODE_NAME;
		with_1.ValueSubItemIndex = ComboBoxCdType.CODE_VALUE;

	}

	/// <summary>
	/// 画面表示用データテーブル生成
	/// </summary>
	/// <returns></returns>
	private DataTable createBaseDataTable()
	{

		DataTable dt = new DataTable();

		//列作成
		dt.Columns.Add("colYoyakuButton"); //予約
		dt.Columns.Add("colTojituHakken"); //当日発券
		dt.Columns.Add("colSyuptDay"); //出発日
		dt.Columns.Add("colSyuptTime"); //出発時間
		dt.Columns.Add("colCrsCd"); //コースコード
		dt.Columns.Add("colCrsName"); //コース名
		dt.Columns.Add("colSituation"); //状況
		dt.Columns.Add("colNoribaKeiyu"); //乗り場経由
		dt.Columns.Add("colGousya"); //号車
		dt.Columns.Add("colKusekiNum"); //空席
		dt.Columns.Add("colKusekiSubSeat"); //空席（補助）
		dt.Columns.Add("colAllNumUtiwakeAllNum"); //総数
		dt.Columns.Add("colUtiwakeYoyakuSeatNum"); //内訳予約
		dt.Columns.Add("colUtiwakeBlockNum"); //内訳ブロック
		dt.Columns.Add("colUtiwakeKusekiKakuhoNum"); //内訳空席確保
		dt.Columns.Add("colWTKensu"); //WT件数
		dt.Columns.Add("colRequestKensu"); //リクエスト件数
		dt.Columns.Add("colStaySitusu1r"); //宿泊室数1r
		dt.Columns.Add("colStaySitusu2r"); //宿泊室数2r
		dt.Columns.Add("colStaySitusu3r"); //宿泊室数3r
		dt.Columns.Add("colStaySitusu4r"); //宿泊室数4r
		dt.Columns.Add("colStaySitusu5r"); //宿泊室数5r
		dt.Columns.Add("colStaySitusuAllNum"); //宿泊室数総
		dt.Columns.Add("colBusType"); //バスタイプ
		dt.Columns.Add("colKeiyuCd"); //経由コード
		dt.Columns.Add("colGuideGengo"); //ガイド言語
		dt.Columns.Add("colCATEGORY1"); //カテゴリ１
		dt.Columns.Add("colCATEGORY2"); //カテゴリ2
		dt.Columns.Add("colCATEGORY3"); //カテゴリ3
		dt.Columns.Add("colCATEGORY4"); //カテゴリ4
		dt.Columns.Add("colSyuptDayHide"); //出発日(隠し項目)
		dt.Columns.Add("colSyuptTimeHide"); //出発時間(隠し項目)
		dt.Columns.Add("colONE_SANKA_FLG"); //1名参加フラグ
		dt.Columns.Add("colTEIKI_KIKAKU_KBN"); //定期・企画区分
		dt.Columns.Add("colHOUJIN_GAIKYAKU_KBN"); //邦人/ 外客区分
		dt.Columns.Add("colCRS_KBN_1"); //コース区分１ / 昼 / 夜
		dt.Columns.Add("colCRS_KBN_2"); //コース区分２ / 都内 / 郊外
		dt.Columns.Add("colCRS_KIND"); //コース種別 / はとバス定期 / ｷｬﾋﾟﾀﾙ / 企画日帰り / 企画宿泊 / 企画都内Rコース
		dt.Columns.Add("colCAR_TYPE_CD "); //車種コード
		dt.Columns.Add("colTEJIMAI_KBN"); //手仕舞区分
		dt.Columns.Add("colYOYAKU_STOP_FLG"); //予約停止フラグ
		dt.Columns.Add("colUNKYU_KBN"); //運休区分 / 運休 / 廃止
		dt.Columns.Add("colSAIKOU_KAKUTEI_KBN"); //催行確定区分
		dt.Columns.Add("colUKETUKE_START_DAY"); //受付開始日
		dt.Columns.Add("colTEIINSEI_FLG"); //定員制フラグ
		dt.Columns.Add("colROOM_ZANSU_SOKEI "); //部屋残数総計
												//dt.Columns.Add("colKUSEKI_KAKUHO_NUM ") '空席確保数

		return dt;

	}

	//---------- 以下個別実装必須分かつ未使用メソッド ----------

	public void setSeFirsttDisplayData()
	{
		throw (new NotImplementedException());
	}

	public void DisplayDataToEntity(ref object ent)
	{
		throw (new NotImplementedException());
	}

	public void EntityDataToDisplay(ref object ent)
	{
		throw (new NotImplementedException());
	}

	public void OldDataToEntity(DataRow pDataRow)
	{
		throw (new NotImplementedException());
	}

	public bool CheckInsert()
	{
		throw (new NotImplementedException());
	}

	public bool CheckUpdate()
	{
		throw (new NotImplementedException());
	}

	public bool isExistHissuError()
	{
		throw (new NotImplementedException());
	}


	#endregion


}