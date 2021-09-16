using C1.Win.C1FlexGrid;


/// <summary>
/// コース一覧照会
/// </summary>
public class S02_0101 : PT11, iPT11
{

	#region 定数／変数
	// "変数"
	//Private SingleCrsCd As String = String.Empty    'Sigleコースコード
	//Private TwinCrsCd As String = String.Empty      'Twinコースコード
	private bool blnCourseAssigne = false; //コースコードFull桁指定（True=指定,False=指定なし）

	// "定数"

	/// <summary>
	/// 画面名
	/// </summary>
	private const string ScreenName = "コース一覧照会";

	/// <summary>
	/// 当日のみフラグ
	/// </summary>
	private const string TojituOnly_All = "1"; //1：当日以外も可能
	private const string TojituOnly_Today = "2"; //2：当日のみ

	/// <summary>
	/// 検索可能コース
	/// </summary>
	public const string SearchCrs_All = "1"; //1：全て
	public const string SearchCrs_Teiki = "2"; //2：定期旅行

	/// <summary>
	/// 区分値 'Y','N'
	/// </summary>
	public const string KbnValueY = "Y";
	public const string KbnValueN = "N";

	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	public const int TopGbxCondition = 41;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	public const int MarginGbxCondition = 6;

	/// <summary>
	/// 検索結果最大表示件数件数
	/// </summary>
	public const int MaxKensu = 100;

	/// <summary>
	/// 共通パラメータ.会社コード
	/// </summary>
	public const string CommonConpanyCd = "00"; //TODO (共通パラメータ.会社コード 決定後に要修正)

	/// <summary>
	/// '「発券(当日)」プログラムID
	/// </summary>
	private const string TojitusHakken = "S02_0105";
	///' <summary>
	///' Sigleコースコード
	///' </summary>
	//Private Const SingleCrsCdKey1 As String = "01"   'Sigleコースコード1
	//Private Const SingleCrsCdKey2 As String = "01"   'Sigleコースコード2
	///' <summary>
	///' Twinコースコード
	///' </summary>
	//Private Const TwinCrsCdKey1 As String = "01"      'Twinコースコード1
	//Private Const TwinCrsCdKey2 As String = "02"      'Twinコースコード2

	/// <summary>
	/// 状態
	/// </summary>
	private const string JOTAI_KAKUTEI = "催行確定";
	private const string JOTAI_UKEMAE = "受付開始前";
	private const string JOTAI_UNKYU = "運休";
	private const string JOTAI_TYUSI = "催行中止";
	private const string JOTAI_FUKA = "予約不可";
	private const string JOTAI_TEJIMAI = "手仕舞済";
	private const string JOTAI_MANSEKI = "満席";
	private const string JOTAI_UKESTOP = "予約受付停止";

	#endregion

	#region プロパティ

	// "パブリック プロパティ"
	//
	/// <summary>
	/// パラメータクラス
	/// </summary>
	public S02_0101ParamData ParamData
	{

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

	/// <summary>
	/// 受付開始日チェックフラグ
	///     (True=チェックあり,False=チェックなし）
	/// </summary>
	/// <returns></returns>
	public bool UketukeStartDayChkFlg
	{

#endregion

		#region イベント
		/// <summary>
		/// 画面ロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
	private void S02_0101_Load(object sender, EventArgs e)
	{
		base.closeFormFlg = false; //クローズ確認フラグ
	}

	/// <summary>
	/// 画面を閉じる
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S02_0101_FormClosing(object sender, FormClosingEventArgs e)
	{
	}

	/// <summary>
	/// F2(戻る)ボタン押下時
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		//戻る
		closeCheckFlg = true;
		base.closeFormFlg = false;
		this.Close();
	}

	/// <summary>
	/// 条件クリアボタン押下時
	/// </summary>
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
	/// 条件GroupBox表示制御ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void BtnVisiblerCondition_Click(object sender, EventArgs e)
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
	/// 検索ボタン押下処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void BtnSearch_Click(object sender, EventArgs e)
	{

		// 検索ボタン押下イベント実行
		base.btnCom_Click(this.btnSearch, e);

		// log出力
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "検索処理");

	}

	/// <summary>
	/// 検索ボタン押下時処理
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{

		//MyBase.btnF8_ClickOrgProc()

		//エラー有無のクリア
		clearError();

		//日付入力値の調整
		setYmdFromTo();

		// [詳細エリア]検索結果部の項目初期化
		initDetailAreaItems();

		//検索条件項目入力チェック
		if (this.CheckSearch() == true)
		{
			//エラーがない場合、検索処理を実行

			// Gridへの表示(グリッドデータの取得とグリッド表示)
			reloadGrid();
		}

	}

	/// <summary>
	/// キーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void S02_0101_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			e.Handled = true;
			this.BtnSearch_Click(sender, e);
		}
		else
		{
			return;
		}
	}

	/// <summary>
	/// 総数内訳詳細表示/非表示押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void BtnAllNumUtiwakeDisplay_Click(object sender, EventArgs e)
	{
		if (btnAllNumUtiwakeDetailDisplayHihyoji.Text.Equals("総数内訳詳細表示"))
		{
			btnAllNumUtiwakeDetailDisplayHihyoji.Text = "総数内訳詳細非表示";
			grdCrsListInquiry.Cols(13).Visible = true;
			grdCrsListInquiry.Cols(14).Visible = true;
			grdCrsListInquiry.Cols(15).Visible = true;
		}
		else
		{
			btnAllNumUtiwakeDetailDisplayHihyoji.Text = "総数内訳詳細表示";
			grdCrsListInquiry.Cols(13).Visible = false;
			grdCrsListInquiry.Cols(14).Visible = false;
			grdCrsListInquiry.Cols(15).Visible = false;
		}
	}

	/// <summary>
	/// グリッド押下時イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdPickupList_Click(object sender, EventArgs e)
	{
		FlexGridEx grd = this.grdCrsListInquiry;
		//行選択モード
		grd.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
		//セル選択解除
		grd.Col = 0;


	}

	/// <summary>
	/// 予約列、当日発券列ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void GrdCrsListInquiry_CellButtonClick()
	{
				)grdCrsListInquiry.CellButtonClick;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		DataTable dt = new DataTable();
		//Dim strSysDate As String = (Now.Year * 10000 + Now.Month * 100 + Now.Day).ToString.Trim
		//Dim intSysTime As Integer = Now.Hour * 100 + Now.Minute
		DateTime nowDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		string strSysDate = nowDate.ToString("yyyyMMdd");
		int intSysTime = int.Parse(nowDate.ToString("HHmm"));

		object with_1 = grdCrsListInquiry;
		if (e.Col == 1)
		{
			//予約列

			//使用中チェック
			CrsLedgerBasic_DA flgrelation = new CrsLedgerBasic_DA();
			DataTable check = flgrelation.getUsingFlg(with_1.Rows(e.Row).Item("colCrsCd").ToString(), with_1.Rows(e.Row).Item("colSyuptDay").ToString(), with_1.Rows(e.Row).Item("colGousya").ToString());
			string usingFlg = System.Convert.ToString(TryCast(check.Rows(0).Item(0), string));
			if (ReferenceEquals(usingFlg, null))
			{
				usingFlg = "";
			}
			if (usingFlg == FixedCd.UsingFlg.Use) //Y'
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_050"); //現在このコースは他の端末で使用中です。
																		 //grdCrsListInquiry.Rows(e.Row).AllowEditing = False
				return;
			}

			//出発時間チェック
			if (string.IsNullOrEmpty(with_1.Rows(e.Row).Item("colSyuptDay").ToString()) == false &&)
			{
				with_1.Rows(e.Row).Item("colSyuptDay").ToString().Trim = strSysDate;
				if (string.IsNullOrEmpty(with_1.Rows(e.Row).Item("colSyuptTime").ToString()) == false &&)
				{
					System.Convert.ToInt32(with_1.Rows(e.Row).Item("colSyuptTime")) < intSysTime;
					if (string.IsNullOrEmpty(with_1.Rows(e.Row).Item("colCRS_KIND").ToString()) == false && with_1.Rows(e.Row).Item("colCRS_KIND").ToString().Trim() == FixedCdYoyaku.CrsKind.teiki.ToString().Trim())
					{
						//コース種別 ＝ '1'、'2'、'3'  （定期）の場合
						paramInfoList.Add("PLACE_CD", with_1.Rows(e.Row).Item("colKeiyuCd").ToString().Trim());
						S02_0101Da dataAccess = new S02_0101Da();
						dt = new DataTable();
						dt = dataAccess.GetPlaceMasterData(paramInfoList);
						if (dt.Rows.Count > 0)
						{
							if (dt.Rows(0).Item("COMPANY_CD").ToString().Trim() == CommonConpanyCd)
							{
								//共通パラメータ.会社コード ＝　'00'　のとき
								CommonProcess.createFactoryMsg().messageDisp("E90_047", "出発時間を過ぎています。予約登録画面の実行"); //現在このコースは他の端末で使用中です。
								return;
							}
							else
							{
								//以外のとき
								CommonProcess.createFactoryMsg().messageDisp("E90_047", "出発時間に余裕がありません。予約登録画面の実行"); //現在このコースは他の端末で使用中です。
								return;
							}
						}
					}
					else
					{
						//コース種別 ＝主催の場合
						CommonProcess.createFactoryMsg().messageDisp("E90_047", "出発時間を過ぎています。予約登録画面の実行"); //現在このコースは他の端末で使用中です。
						return;
					}
				}
			}

			strSysDate = System.Convert.ToString(CommonDateUtil.getSystemTime().ToString("yyyyMMdd"));
			// コース台帳（基本）.受付開始日 ＞ システム日付の場合
			if (string.Compare(with_1.Rows(e.Row).Item("colUKETUKE_START_DAY").ToString(), strSysDate) > 0)
			{
				CommonProcess.createFactoryMsg.messageDisp("E90_047", "受付開始前です。予約登録画面の実行"); //受付開始前です。予約登録画面の実行ができません。

				return;
			}

			//予約登録画面に遷移
			using (S02_0103 form = new S02_0103())
			{
				S02_0103ParamData prm = new S02_0103ParamData();
				prm.ScreenMode = CommonRegistYoyaku.ScreenModeNew; //登録
				prm.CrsCd = with_1.Rows(e.Row).Item("colCrsCd").ToString().Trim();
				prm.Gousya = System.Convert.ToInt32(with_1.Rows(e.Row).Item("colGousya").ToString().Trim());
				prm.CrsKind = with_1.Rows(e.Row).Item("colCRS_KIND").ToString().Trim();
				prm.SyuptDay = System.Convert.ToInt32(with_1.Rows(e.Row).Item("colSyuptDay").ToString().Trim());
				//prm.SyuptTime = CInt(.Rows(e.Row).Item("colSyuptTime").ToString.Trim)  'TODO
				//prm.JyochatiCd = Rows(e.Row).Item("colKeiyuCd").ToString.Trim          'TODO
				form.ParamData = prm;
				form.ShowDialog();
			}

		}
		else if (e.Col == 2)
		{
			//当日発券列
			//出発時間チェック
			if (string.IsNullOrEmpty(with_1.Rows(e.Row).Item("colSyuptDay").ToString()) == false &&)
			{
				with_1.Rows(e.Row).Item("colSyuptDay").ToString().Trim() != strSysDate;
				CommonProcess.createFactoryMsg().messageDisp("E03_006"); //出発日当日のみ処理可能です。現在は取り扱えません。
				return;
			}
			//当日発行画面に遷移
			using (S02_0105 form = new S02_0105())
			{
				S02_0105ParamData prm = new S02_0105ParamData();
				prm.CrsCd = with_1.Rows(e.Row).Item("colCrsCd").ToString().Trim();
				prm.SyuptDay = System.Convert.ToInt32(with_1.Rows(e.Row).Item("colSyuptDay").ToString().Trim());
				prm.Gousya = System.Convert.ToInt32(with_1.Rows(e.Row).Item("colGousya").ToString().Trim());
				prm.CrsKind = with_1.Rows(e.Row).Item("colCRS_KIND").ToString().Trim();
				prm.JyosyatiCd = with_1.Rows(e.Row).Item("colKeiyuCd").ToString().Trim();
				prm.SyuptTime = System.Convert.ToInt32(with_1.Rows(e.Row).Item("colSyuptTime").ToString().Trim());
				form.ParamData = prm;
				form.ShowDialog();
			}

		}
	}

	/// <summary>
	/// グリッドソート後、予約の状況によって文字色の変更、予約・当日発券ボタン制御を行う。
	/// </summary>
	private void grdCrsListInquiry_AfterSort(object sender, SortColEventArgs e)
	{
		this.GrdJyokyoDisp();
	}
	#endregion

	#region メソッド

	/// <summary>
	/// 画面クローズ時
	/// </summary>
	/// <returns></returns>
	protected override bool closingScreen()
	{
		// 変数宣言
		bool retCode = false;
		return retCode;
	}

	/// <summary>
	/// 画面初期Load時のメソッド
	/// </summary>
	protected override void initScreenPerttern()
	{
		//ベースフォームの初期化処理
		base.initScreenPerttern();

	}

	/// <summary>
	/// フッタボタンの制御(表示\[活性]／非表示[非活性])
	/// </summary>
	protected override void InitFooterButtonControl()
	{
		// 不要ボタンを非表示、無効(非活性)且つにする
		this.F1Key_Visible = false; // F1:未使用
		this.F1Key_Enabled = false;
		this.F3Key_Visible = false; // F3;未使用
		this.F3Key_Enabled = false; //
		this.F1Key_Visible = false; // F1:未使用
		this.F1Key_Enabled = false; //
		this.F3Key_Visible = false; // F3;未使用
		this.F3Key_Enabled = false; //
		this.F4Key_Visible = false; // F4;未使用
		this.F4Key_Enabled = false; //
		this.F5Key_Visible = false; // F5:未使用
		this.F5Key_Enabled = false; //
		this.F6Key_Visible = false; // F6:画面ごと
		this.F6Key_Enabled = false; //
		this.F7Key_Visible = false; // F7:未使用
		this.F7Key_Enabled = false; //
		this.F8Key_Visible = false; // F8:検索
		this.F8Key_Enabled = false; //
		this.F9Key_Visible = false; // F9:画面ごと
		this.F9Key_Enabled = false; //
		this.F10Key_Visible = false; // F10:
		this.F10Key_Enabled = false; //
		this.F11Key_Visible = false; // F11:更新
		this.F11Key_Enabled = false; //
		this.F12Key_Visible = false; // F12:画面ごと
		this.F12Key_Enabled = false; //

	}

	// 初期化用のメソッド

	/// <summary>
	/// [検索エリア]検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{
		S02_0101Da dataAccess = new S02_0101Da(); //DataAccessクラス生成
												  //Dim paramInfoList As New Hashtable 'DBパラメータ
		DataTable returnValue = new DataTable(); //戻り値

		//条件GroupBox内のテキストボックス初期化
		object textBoxs = this.gbxCondition.Controls.OfType(Of TextBoxEx);
		foreach ( in textBoxs)
		{
			textBox.Text = "";
		}
		this.numKusekiNum.Text = ""; //空席数テキストボックス初期化
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

		// コンボボックスの設定
		CommonMstUtil.setComboBox(CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.busType), this.cmbBusType, true, null);
		CommonMstUtil.setComboBox(CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.language), this.cmbGuidoGengo, true, null);

		//言語の初期化
		this.cmbGuidoGengo.SelectedValue = null;
		//バスタイプの初期化
		this.cmbBusType.SelectedValue = null;

		//グリッドの初期化
		DataTable dt = new DataTable();
		this.grdCrsListInquiry.DataSource = dt;
		SetgrdCrsListInquiry();
		this.lblLengthGrd.Text = "";

		this.grdCrsListInquiry.DataMember = "";
		this.grdCrsListInquiry.Refresh();

		this.lblLengthGrd.Text = "     0件";
		DateTime nowDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
		this.dtmSyuptDayFromTo.FromDateText = nowDate;
		this.dtmSyuptDayFromTo.ToDateText = nowDate;

		//受取パラメータによる初期値セット

		//所属部署が国際事業部の場合、外国語にチェック
		if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
		{
			this.chkGaikokugoCrs.Checked = true; // 外国語のみチェック
			this.chkJapaneseCrs.Checked = false;
		}
		else
		{
			this.chkJapaneseCrs.Checked = true; //日本語コースのみチェック
			this.chkGaikokugoCrs.Checked = false;
		}

		if (ParamData IsNot null)
				{
			//検索可能コース
			if (ParamData.SearchKanouCrs == SearchCrs_All || ParamData.SearchKanouCrs == SearchCrs_Teiki)
			{
				//chkTeikiNoon.Checked = True                          2019/03/06  初期値チェックは廃止
				//chkTeikiNight.Checked = True
				//chkTeikiKogai.Checked = True
				if (ParamData.SearchKanouCrs == SearchCrs_All)
				{
					//全て選択
					//chkKikakuDayTrip.Checked = True                  2019/03/06  初期値チェックは廃止
					//chkKikakuStay.Checked = True
					//chkKikakuTonaiR.Checked = True
					//chkCapital.Checked = True
				}
				else
				{
					//定期以外は選択できない。
					chkKikakuDayTrip.Enabled = false;
					chkKikakuStay.Enabled = false;
					chkKikakuTonaiR.Enabled = false;
					chkCapital.Enabled = false;
				}
			}
			//当日のみフラグ
			if (ParamData.TojituOnlyFlg == TojituOnly_Today)
			{
				//画面制御区分が２の場合、
				//出発日From、出発日Toに当日をセット、渡された出発日以外は選択できない
				//Me.dtmSyuptDayFromTo.FromDateText = Convert.ToDateTime(CommonDateUtil.convertDateFormat(ParamData.SyuptDay.ToString.Trim, CommonDateUtil.NormalTime_JP))
				//Me.dtmSyuptDayFromTo.ToDateText = Convert.ToDateTime(CommonDateUtil.convertDateFormat(ParamData.SyuptDay.ToString.Trim, CommonDateUtil.NormalTime_JP))
				this.dtmSyuptDayFromTo.Enabled = false;
			}

			//'画面制御区分 = '1'、'2' の場合、
			//If ParamData.ScreenMode = CommonRegistYoyaku.ScreenModeNew Or ParamData.ScreenMode = CommonRegistYoyaku.ScreenModeEdit Then
			//    'コース区分と出発日From、出発日Toを初期セット
			//    Me.dtmSyuptDayFromTo.FromDateText = Convert.ToDateTime(CommonDateUtil.convertDateFormat(ParamData.SyuptDay.ToString.Trim, CommonDateUtil.NormalTime_JP))
			//    Me.dtmSyuptDayFromTo.ToDateText = Convert.ToDateTime(CommonDateUtil.convertDateFormat(ParamData.SyuptDay.ToString.Trim, CommonDateUtil.NormalTime_JP))
			//    '画面制御区分が２の場合、渡された出発日以外は選択できない
			//    If ParamData.ScreenMode = CommonRegistYoyaku.ScreenModeEdit Then
			//        Me.dtmSyuptDayFromTo.Enabled = False
			//    End If

			//    '受取パラメータ : コース種別 = 1（はとバス定期）の場合、定期をチェック
			//    CheckBoxs = Me.gbxCondition.Controls.OfType(Of CheckBoxEx)
			//    For Each CheckBox In CheckBoxs
			//        If CheckBox.Name <> "chkJapaneseCrs" And
			//           CheckBox.Name <> "chkGaikokugoCrs" Then
			//            Select Case ParamData.CrsKind
			//                Case FixedCdYoyaku.CrsKind.teiki
			//                    '受取パラメータ : コース種別 = 1（はとバス定期）の場合、定期をチェック
			//                    If CheckBox.Name.Substring(0, 8) = "chkTeiki" Then
			//                        CheckBox.Checked = True
			//                    Else
			//                        CheckBox.Enabled = False
			//                    End If
			//                Case FixedCdYoyaku.CrsKind.kikakuHigaeri
			//                    '受取パラメータ : コース種別 = 4（企画（日帰り））の場合、企画（日帰り）をチェック
			//                    If CheckBox.Name = "chkKikakuDayTrip" Then
			//                        CheckBox.Checked = True
			//                    Else
			//                        CheckBox.Enabled = False
			//                    End If
			//                Case FixedCdYoyaku.CrsKind.kikakuStay
			//                    '受取パラメータ : コース種別 = 5（企画（宿泊））の場合、企画（宿泊）をチェック
			//                    If CheckBox.Name = "chkKikakuStay" Then
			//                        CheckBox.Checked = True
			//                    Else
			//                        CheckBox.Enabled = False
			//                    End If
			//                Case FixedCdYoyaku.CrsKind.kikakuTonaiR
			//                    '受取パラメータ : コース種別 = 6（企画（都内Rコース））の場合、企画（都内R）をチェック
			//                    If CheckBox.Name = "chkKikakuTonaiR" Then
			//                        CheckBox.Checked = True
			//                    Else
			//                        CheckBox.Enabled = False
			//                    End If
			//                Case FixedCdYoyaku.CrsKind.capital
			//                    '受取パラメータ : コース種別 = 2（キャピタル）の場合、キャピタルをチェック
			//                    If CheckBox.Name = "chkCapital" Then
			//                        CheckBox.Checked = True
			//                    Else
			//                        CheckBox.Enabled = False
			//                    End If
			//            End Select
			//        End If
			//    Next
			//End If
			//'当日発券から呼ばれた場合
			//If ParamData.CallerID = TojitusHakken Then
			//    '受取パラメータ:営業所コードをセット
			//    Me.ucoNoribaCd.CodeText = ParamData.JyochatiCd
			//    paramInfoList.Clear()
			//    paramInfoList.Add("PLACE_CD", Me.ucoNoribaCd.CodeText)
			//    '受取パラメータ:営業所コードのマスタ値を選択
			//    returnValue = New DataTable
			//    returnValue = dataAccess.GetPlaceMasterData(paramInfoList)
			//    If returnValue.Rows.Count > 0 Then
			//        Me.ucoNoribaCd.ValueText = returnValue.Rows(0).Item("PLACE_NAME_1").ToString.Trim
			//    End If
			//End If
		}
		//    'Single,Twinコースコード設定
		//    paramInfoList.Clear()
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

		returnValue = null;
		dataAccess = null;
		//paramInfoList = Nothing
	}

	/// <summary>
	/// エラー有無のクリア
	/// </summary>
	private void clearError()
	{
		// ExistErrorプロパティのクリア
		dtmSyuptDayFromTo.ExistErrorForFromDate = false;
		dtmSyuptDayFromTo.ExistErrorForToDate = false;
		ucoNoribaCd.ExistError = false;
		ucoCrsCd.ExistError = false;
		chkTeikiNoon.ExistError = false;
		chkTeikiNight.ExistError = false;
		chkTeikiKogai.ExistError = false;
		chkCapital.ExistError = false;
		chkKikakuDayTrip.ExistError = false;
		chkKikakuStay.ExistError = false;
		chkKikakuTonaiR.ExistError = false;

		// Exceptionのクリア
		this.Exception = null;

		// エラーフラグの初期化
		this.ErrorFlg = false;

	}

	/// <summary>
	///  [詳細エリア]検索結果部の項目初期化
	/// </summary>
	protected override void initDetailAreaItems()
	{
		base.initDetailAreaItems();

		DataTable dt = new DataTable();
		this.SetgrdCrsListInquiry();
		this.grdCrsListInquiry.DataSource = dt;
		this.grdCrsListInquiry.DataMember = "";
		this.grdCrsListInquiry.Refresh();
		this.lblLengthGrd.Text = "     0件";
	}

	/// <summary>
	/// エンティティ初期化
	/// </summary>
	protected override void initEntityData()
	{
	}

	/// <summary>
	/// [選択行設定]Grid=>エンティティ(選択行のデータを取得)
	/// </summary>
	protected override void getSelectedRowData()
	{
	}

	/// <summary>
	/// Gridへの表示(グリッドデータの取得とグリッド表示)
	/// </summary>
	protected override void reloadGrid()
	{
		base.reloadGrid();

		GetgrdCrsListInquiry();

	}

	// チェック系
	/// <summary>
	/// [検索処理](検索条件項目入力チェック)
	/// </summary>
	/// <returns></returns>
	protected override bool checkSearchItems()
	{
		return true;
	}

	/// <summary>
	/// 日付入力値の調整
	/// </summary>
	private void setYmdFromTo()
	{
		if (dtmSyuptDayFromTo.FromDateText IsNot null && ReferenceEquals(dtmSyuptDayFromTo.ToDateText, null))
				{
			//日付From <> ブランク かつ 日付To = ブランクの場合、日付To に 日付From の値をセット
			dtmSyuptDayFromTo.ToDateText = dtmSyuptDayFromTo.FromDateText;

		}
				else if (dtmSyuptDayFromTo.ToDateText IsNot null && ReferenceEquals(dtmSyuptDayFromTo.FromDateText, null))
				{
			//日付To <> ブランク かつ 日付From = ブランクの場合、日付From に 日付To の値をセット
			dtmSyuptDayFromTo.FromDateText = dtmSyuptDayFromTo.ToDateText;
		}
	}

	/// <summary>
	/// コース一覧照会グリッドの設定
	/// </summary>
	private void SetgrdCrsListInquiry()
	{
		grdCrsListInquiry.AllowAddNew = true;
		// 行ヘッダを作成。
		grdCrsListInquiry.Styles.Normal.WordWrap = true;
		grdCrsListInquiry.Cols(17).Style.WordWrap = true;
		grdCrsListInquiry.Cols.Count = 50; //gridの行数
		grdCrsListInquiry.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.FixedOnly;
		grdCrsListInquiry.Rows(0).AllowMerging = true;
		grdCrsListInquiry.Cols(0).AllowMerging = true;
		C1.Win.C1FlexGrid.CellRange cr = null;
		//1行目
		// 予約
		cr = grdCrsListInquiry.GetCellRange(0, 1, 1, 1);
		grdCrsListInquiry.MergedRanges.Add(cr);
		// 当日発券
		cr = grdCrsListInquiry.GetCellRange(0, 2, 1, 2);
		grdCrsListInquiry.MergedRanges.Add(cr);
		// 出発日
		cr = grdCrsListInquiry.GetCellRange(0, 3, 1, 3);
		grdCrsListInquiry.MergedRanges.Add(cr);
		// 時間
		cr = grdCrsListInquiry.GetCellRange(0, 4, 1, 4);
		grdCrsListInquiry.MergedRanges.Add(cr);
		// コースコード
		cr = grdCrsListInquiry.GetCellRange(0, 5, 1, 5);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//コース名
		cr = grdCrsListInquiry.GetCellRange(0, 6, 1, 6);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//状況
		cr = grdCrsListInquiry.GetCellRange(0, 7, 1, 7);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//乗り場経由
		cr = grdCrsListInquiry.GetCellRange(0, 8, 1, 8);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//号車
		cr = grdCrsListInquiry.GetCellRange(0, 9, 1, 9);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//空席
		cr = grdCrsListInquiry.GetCellRange(0, 10, 1, 10);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//空席（補助席）
		cr = grdCrsListInquiry.GetCellRange(0, 11, 1, 11);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//総数
		cr = grdCrsListInquiry.GetCellRange(0, 12, 1, 12);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//'内訳
		cr = grdCrsListInquiry.GetCellRange(0, 13, 0, 15);
		cr.Data = "内訳";
		grdCrsListInquiry.MergedRanges.Add(cr);

		//2行目
		grdCrsListInquiry[1, 13] = "予約";
		grdCrsListInquiry[1, 14] = "ブロック";
		grdCrsListInquiry[1, 15] = "空席確保";
		//WT件数
		cr = grdCrsListInquiry.GetCellRange(0, 16, 1, 16);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//リクエスト件数
		cr = grdCrsListInquiry.GetCellRange(0, 17, 1, 17);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//'宿泊室数
		cr = grdCrsListInquiry.GetCellRange(0, 18, 0, 23);
		cr.Data = "宿泊室数";
		grdCrsListInquiry.MergedRanges.Add(cr);
		grdCrsListInquiry[1, 18] = "1Ｒ";
		grdCrsListInquiry[1, 19] = "2Ｒ";
		grdCrsListInquiry[1, 20] = "3Ｒ";
		grdCrsListInquiry[1, 21] = "4Ｒ";
		grdCrsListInquiry[1, 22] = "5Ｒ";
		grdCrsListInquiry[1, 23] = "総";
		//バスタイプ
		cr = grdCrsListInquiry.GetCellRange(0, 24, 1, 24);
		grdCrsListInquiry.MergedRanges.Add(cr);
		//予約以降の列で横スクロール
		grdCrsListInquiry.Cols.Frozen = 1;
		//非表示部分
		for (byte intCnt = 25; intCnt <= 49; intCnt++)
		{
			grdCrsListInquiry.Cols(intCnt).Visible = false;
		}
		grdCrsListInquiry[1, 24] = "24";
		grdCrsListInquiry[1, 25] = "25";
		grdCrsListInquiry[1, 26] = "26";
		grdCrsListInquiry[1, 27] = "27";
		grdCrsListInquiry[1, 28] = "28";
		grdCrsListInquiry.AllowAddNew = false;
	}

	/// <summary>
	/// コース一覧照会モック用グリッド表示値の設定
	/// </summary>
	private void GetgrdCrsListInquiry()
	{
		DateTime dteTmpDate = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
		string strFromDate = string.Empty;
		string strToDate = string.Empty;
		string strSysDate = string.Empty;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		DataTable dt = new DataTable();
		//列作成
		dt.Columns.Add("colYoyakuButton"); //予約
		dt.Columns.Add("colTojituHakken"); //当日発券
		dt.Columns.Add("colSyuptDayHen"); //出発日（編集）
		dt.Columns.Add("colSyuptTimeHen"); //出発時間（編集）
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
		dt.Columns.Add("colSyuptDay"); //出発日
		dt.Columns.Add("colSyuptTime"); //出発時間
		dt.Columns.Add("colTEIINSEI_FLG"); //定員制フラグ
		dt.Columns.Add("colROOM_ZANSU_SOKEI "); //部屋残数総計
												//dt.Columns.Add("colKUSEKI_KAKUHO_NUM ") '空席確保数

		//グリッド初期化
		this.grdCrsListInquiry.DataSource = dt;
		//'パラメータ設定
		//paramInfoList.Add("SingleCrsCd", SingleCrsCd)
		//paramInfoList.Add("TwinCrsCd", TwinCrsCd)

		//出発日
		dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText));
		strFromDate = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
		dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(dtmSyuptDayFromTo.ToDateText));
		strToDate = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
		paramInfoList.Add("SyuptDayFrom", strFromDate);
		paramInfoList.Add("SyuptDayTo", strToDate);
		//コースコード
		paramInfoList.Add("CrsCd", Strings.Trim(System.Convert.ToString(this.ucoCrsCd.CodeText)));
		paramInfoList.Add("CrsCdAssigne", blnCourseAssigne); //コースコード検索用（指定コードFull桁判定）

		//ガイド言語
		if (ReferenceEquals(cmbGuidoGengo.SelectedValue, null) == false)
		{
			paramInfoList.Add("GuidoGengo", Strings.Trim(System.Convert.ToString(cmbGuidoGengo.SelectedValue.ToString())));
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
		paramInfoList.Add("KusekiNum", Strings.Trim(System.Convert.ToString(this.numKusekiNum.Text)));
		//予約可能のみ
		paramInfoList.Add("YoyakuKanouOnly", Strings.Trim(System.Convert.ToString(this.chkYoyakuKanouOnly.Checked.ToString())));
		//宿泊一人参加のみ
		paramInfoList.Add("Stay1NinSanka", Strings.Trim(System.Convert.ToString(this.chkStay1NinSanka.Checked.ToString())));
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
		//企画（日帰り）
		paramInfoList.Add("KikakuDayTrip", Strings.Trim(System.Convert.ToString(this.chkKikakuDayTrip.Checked.ToString())));
		//企画（宿泊）
		paramInfoList.Add("KikakuStay", Strings.Trim(System.Convert.ToString(this.chkKikakuStay.Checked.ToString())));
		//企画（都内R）
		paramInfoList.Add("KikakuTonaiR", Strings.Trim(System.Convert.ToString(this.chkKikakuTonaiR.Checked.ToString())));
		//キャピタル
		paramInfoList.Add("Capital", Strings.Trim(System.Convert.ToString(this.chkCapital.Checked.ToString())));
		//検索可能コース("1":すべて、"2":定期旅行）　←　予約メニューからのパラメータ
		paramInfoList.Add("SearchKanouCrs", ParamData.SearchKanouCrs);

		S02_0101Da dataAccess = new S02_0101Da();
		DataTable dataCRS_LEDGER = dataAccess.AccessCourseDaityo(S02_0101Da.AccessType.courseByHeaderKey, paramInfoList);

		if (dataCRS_LEDGER.Rows.Count == 0)
		{
			// 取得件数0件の場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_004");
			return;
		}

		DataRow drX = null;
		int cntRowCnt = 0;
		//strSysDate = (Now.Year * 10000 + Now.Month * 100 + Now.Day).ToString
		strSysDate = System.Convert.ToString(CommonDateUtil.getSystemTime().ToString("yyyyMMdd"));
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
				if (ii == 2)
				{
					if (Information.IsDate(dr["SYUPT_DAY"].ToString()) == true)
					{
						dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(dr["SYUPT_DAY"].ToString().Trim()));
						drX[ii] = dr["SYUPT_DAY"].ToString().Substring(5) + "(" +;
						Common.CommonDateUtil.getDayOfTheWeek(dteTmpDate).Substring(0, 1) + ")";
					}
				}
				if (ii == 6)
				{
					//催行確定区分 = 'Y' の場合
					if (dr["SAIKOU_KAKUTEI_KBN"].ToString().Trim() == KbnValueY)
					{
						drX[ii] = JOTAI_KAKUTEI; //"催行確定"
					}

					//「受付開始日 ﾁｪｯｸﾌﾗｸ」= 'Y’ で コース台帳（基本）.受付開始日 ＞ システム日付の場合
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
							if (System.Convert.ToInt32(dr["CRS_KIND"]) == crsKbn2Type.suburbs)
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
					if (dr["SAIKOU_KAKUTEI_KBN"].ToString().Trim() == KbnValueN)
					{
						drX[ii] = JOTAI_TYUSI; //"催行中止"
					}
					//手仕舞区分 = 'Y' で催行確定区分 ≠ 'N' の場合
					if (dr["TEJIMAI_KBN"].ToString().Trim() == KbnValueY && dr["SAIKOU_KAKUTEI_KBN"].ToString().Trim() != KbnValueN)
					{
						drX[ii] = JOTAI_TEJIMAI; //    "手仕舞済"
					}
					//空席数（定席）＋空席数（補助席） = 0 の場合
					if ((System.Convert.ToInt32(dr["KUSEKI_NUM_TEISEKI"])) + System.Convert.ToInt32(dr["KUSEKI_NUM_SUB_SEAT"]) == 0)
					{
						drX[ii] = JOTAI_MANSEKI; //"満席"
					}
				}
				if (ii > 16 & ii < 23 && dr["CRS_KIND"].ToString().Trim() != (System.Convert.ToInt32(FixedCdYoyaku.CrsKind.kikakuStay)).ToString())
				{
					drX[ii] = CommonRegistYoyaku.Hyphen; //"-"
				}

			}
			dt.Rows.Add(drX);
		}
		grdCrsListInquiry.DataSource = dt;
		string formatedCount = string.Empty;
		if (dataCRS_LEDGER.Rows.Count > MaxKensu)
		{
			formatedCount = System.Convert.ToString((MaxKensu).ToString().PadLeft(6));
		}
		else
		{
			formatedCount = System.Convert.ToString((dt.Rows.Count).ToString().PadLeft(6));
		}
		this.lblLengthGrd.Text = formatedCount + "件";

		//「状況」の文字色、予約・当日発券ボタンの活性・非活性制御
		this.GrdJyokyoDisp();

		if (dataCRS_LEDGER.Rows.Count > MaxKensu)
		{
			// 取得件数が設定件数より多い場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_027", "検索結果が");
		}
	}

	/// <summary>
	/// '予約ボタン、当日発券ボタン、状況欄表示制御
	/// </summary>
	private void GrdJyokyoDisp()
	{
		//予約の状況によって「状況」の文字色、予約・当日発券ボタンの活性・非活性制御を行う。
		CellStyle cellColor_red = this.grdCrsListInquiry.Styles.Add("red");
		CellStyle cellColor_Black = this.grdCrsListInquiry.Styles.Add("normal");
		cellColor_Black.ForeColor = Color.Black;
		cellColor_red.ForeColor = Color.Red;

		object with_1 = grdCrsListInquiry;
		for (cntRowCnt = 2; cntRowCnt <= with_1.Rows.Count - 1; cntRowCnt++) //
		{
			with_1.Rows(cntRowCnt).AllowEditing = true; //    予約ボタン、当日発券ボタンを活性にする
			with_1.SetCellStyle(cntRowCnt, 7, cellColor_red); //   "催行確定" 以外は文字色赤
			if (with_1.Rows(cntRowCnt).Item(7).ToString() == JOTAI_KAKUTEI) //   "催行確定"    は文字色黒
			{
				with_1.SetCellStyle(cntRowCnt, 7, cellColor_Black);
			}

			{

				with_1.Rows(cntRowCnt).Item(7).ToString = JOTAI_TYUSI); //   "予約受付停止","運休","催行中止" のコースは
		with_1.Rows(cntRowCnt).AllowEditing = false; //    予約ボタン、当日発券ボタンを非活性にする
	}
}
			}
			
			/// <summary>
			/// 検索処理前のチェック処理(必須画面個別実装)
			/// </summary>
			/// <returns>True:エラー無 False:エラー有</returns>
			public bool CheckSearch()
{
	DateTime dteTmpDate = System.Convert.ToDateTime("#01/01/0001 12:00:00 AM#");
	object CheckBoxs = this.gbxCondition.Controls.OfType(Of CheckBoxEx);

	object with_1 = this.dtmSyuptDayFromTo;

	//過去日付
	DateTime nowDate = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());
	if (with_1.FromDateText.HasValue == true && with_1.FromDateText.Value.ToShortDateString() < nowDate.ToShortDateString())
	{
		with_1.ExistErrorForFromDate = true;
		with_1.Focus();
		CommonProcess.createFactoryMsg().messageDisp("E90_013"); //過去日は指定できません。
		return false;
	}
	else if (with_1.ToDateText.HasValue == true && with_1.ToDateText.Value.ToShortDateString() < nowDate.ToShortDateString())
	{
		with_1.ExistErrorForToDate = true;
		with_1.Focus();
		CommonProcess.createFactoryMsg().messageDisp("E90_013"); //過去日は指定できません。
		return false;
	}

	//日付の不整合
	//日付のFromToに入力があり
	//   From　>　To　の場合
	if (with_1.FromDateText.HasValue == true && with_1.ToDateText.HasValue == true)
	{
		if (with_1.FromDateText > with_1.ToDateText)
		{
			with_1.ExistErrorForToDate = true;
			with_1.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_016", "日付のFromTo");
			return false;
		}

		//FROM TO 間が３ヶ月以上空いている場合
		dteTmpDate = System.Convert.ToDateTime(CommonDateUtil.convertDateFormat(CommonDateUtil.calculationAfterNMonths(System.Convert.ToDateTime(this.dtmSyuptDayFromTo.FromDateText), 3), CommonDateUtil.NormalTime_JP));
		if (with_1.ToDateText.HasValue == true && with_1.ToDateText > dteTmpDate)
		{
			with_1.ExistErrorForToDate = true;
			with_1.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_011", "日付のFROM-TOは３ヶ月以内の範囲"); //を入力してください。
			return false;
		}
	}

	//日付未入力
	if (with_1.FromDateText.HasValue == false)
	{
		with_1.ExistErrorForFromDate = true;
		with_1.Focus();
		CommonProcess.createFactoryMsg().messageDisp("E90_023", "出発日");
		return false;
	}
	else
	{
		if (with_1.ToDateText.HasValue == false)
		{
			with_1.ToDateText = with_1.FromDateText;
		}
	}


	//コースコードとコース区分
	//が双方指定されている場合
	Hashtable paramInfoList = new Hashtable();
	DataTable dt = new DataTable();
	S02_0101Da dataAccess = new S02_0101Da();
	blnCourseAssigne = false;
	if (string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)) == false)
	{
		paramInfoList.Add("CRS_CD", this.ucoCrsCd.CodeText);
		dt = dataAccess.AccessCourseDaityo(S02_0101Da.AccessType.courseByPrimaryKey, paramInfoList);
		if (dt.Rows.Count < 1)
		{
			blnCourseAssigne = false; //コースコードFull桁指定なし
									  //コードがマスタにに存在しません。
			this.ucoCrsCd.ValueText = "";
			this.ucoCrsCd.ExistError = true;
			this.ucoCrsCd.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_014", "コースコード");
			return false;
		}
		else
		{
			if (ParamData.SearchKanouCrs == SearchCrs_Teiki &&)
			{
				((FixedCd.TeikiKikakuKbn)(dt.Rows(0).Item("TEIKI_KIKAKU_KBN").ToString())) != TeikiKikakuKbn.Teiki;
				//コードがマスタにに存在しません。
				this.ucoCrsCd.ValueText = "";
				this.ucoCrsCd.ExistError = true;
				this.ucoCrsCd.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_047", "定期コース以外は指定");
				return false;
			}
			if (this.ucoCrsCd.CodeText.ToString().Trim() == dt.Rows(0).Item(0).ToString().Trim())
			{
				blnCourseAssigne = true; //コースコードFull桁指定なし
				this.ucoCrsCd.ValueText = dt.Rows(0).Item("CRS_NAME_RK").ToString();
				//                                                       2019/03/06 コースコードとコース区分は同時に指定のチェック廃止
				//Dim chkErr As Boolean = False
				//For Each CheckBox In CheckBoxs
				//    If CheckBox.Name <> "chkJapaneseCrs" And
				//       CheckBox.Name <> "chkGaikokugoCrs" And
				//       CheckBox.Name <> "chkYoyakuKanouOnly" And
				//       CheckBox.Name <> "chkStay1NinSanka" Then
				//        If CheckBox.Checked = True Then
				//            CheckBox.ExistError = True
				//            CheckBox.Focus()
				//            chkErr = True
				//        End If
				//    End If
				//Next
				//If chkErr = True Then
				//    'コースコードとコース区分が双方指定されている場合、
				//    '「コースコードとコース区分は同時に指定できません」
				//    CommonProcess.createFactoryMsg().messageDisp("E90_047", "コースコードとコース区分は同時に指定")
				//    Return False
				//End If
			}
			else
			{
				this.ucoCrsCd.ValueText = "";
			}
		}
	}
	//コース区分
	//「キャピタル」が指定されていて、それ以外のコード区分も指定されている場合
	//If Me.chkCapital.Checked = True Then
	//    For Each CheckBox In CheckBoxs
	//        If CheckBox.Name <> "chkJapaneseCrs" And
	//           CheckBox.Name <> "chkGaikokugoCrs" And
	//           CheckBox.Name <> "chkCapital" Then
	//            If CheckBox.Checked = True Then
	//                CheckBox.Focus()
	//                'キャピタルの場合、他コース区分は選択できません
	//                CommonProcess.createFactoryMsg().messageDisp("E90_047", "キャピタルの場合、他コース区分は選択")
	//                Return False
	//                Exit For
	//            End If
	//        End If
	//    Next
	//End If

	//乗り場 検索項目	乗り場	"・乗り場がFull桁指定されていて、場所マスタに未登録の場合
	//「該当するコードがマスタにありません」"
	if (string.IsNullOrEmpty(System.Convert.ToString(this.ucoNoribaCd.CodeText)) == false)
	{
		paramInfoList.Add("PLACE_CD", this.ucoNoribaCd.CodeText);
		Place_DA dataAccess2 = new Place_DA();
		dt = new DataTable();
		dt = dataAccess.GetPlaceMasterDataCodeData(false, "PLACE_CD = '" + this.ucoNoribaCd.CodeText.Trim() + "' ");
		if (dt.Rows.Count < 1)
		{
			this.ucoNoribaCd.ValueText = "";
			this.ucoNoribaCd.ExistError = true;
			//コードがマスタに存在しません。
			this.ucoNoribaCd.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_014", "乗車地コード");
			this.ucoNoribaCd.ValueText = "";
			return false;
		}
		else
		{
			this.ucoNoribaCd.ValueText = dt.Rows(0).Item("PLACE_NAME_1").ToString().Trim();
		}
	}
	return true;
}

public void setSeFirsttDisplayData()
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

public void DisplayDataToEntity(ref object ent)
{
	throw (new NotImplementedException());
}

public void EntityDataToDisplay(ref object ent)
{
	throw (new NotImplementedException());
}
			
#endregion
			
		}