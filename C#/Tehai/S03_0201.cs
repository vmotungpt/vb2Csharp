using Hatobus.ReservationManagementSystem.Master;


/// <summary>
/// コース台帳一括修正
/// コース情報を変更する画面へ遷移する画面
/// </summary>
public class S03_0201 : FormBase
{
	public S03_0201()
	{
		dtToday = System.Convert.ToDateTime(CommonDateUtil.getSystemTime());

	}

	#region 定数
	#endregion

	#region 変数
	protected DataTable searchResultChkData = new DataTable(); // 検索結果チェックデータ
	public bool closeCheckFlg = false; //
	protected LogOut clsLogOut = new LogOut(); // ログ出力
	private string _connectionString; // 接続文字列

	private string nichiyobiCd = ""; // 日曜日
	private string getsuyobiCd = ""; // 月曜日
	private string kayobiCd = ""; // 火曜日
	private string suiyobiCd = ""; // 水曜日
	private string mokuyobiCd = ""; // 木曜日
	private string kinyobiCd = ""; // 金曜日
	private string doyobiCd = ""; // 土曜日
	private string unkyuFlg = ""; // 運休フラグ
	private string haishiFlg = ""; // 廃止フラグ
								   //Private maruzoFlg As String = ""                                    ' ○増フラグ
	private ResearchData m_form; // データ格納クラス
	private bool nothingFlg = true; // データ有無フラグ

	// 現在の日付を取得する
	DateTime dtToday;
	private TehaiCommon comTehai = new TehaiCommon();


	#endregion

	#region イベント

	#region 画面起動時の独自処理
	/// <summary>
	/// 画面起動時の独自処理
	/// </summary>
	protected override void startupOrgProc()
	{
		//Dim reaAppConfig As New ReadAppConfig
		//_connectionString = reaAppConfig.getConnectString("1")
		//reaAppConfig = Nothing

		// フォーマット設定
		txtGousya.Format = setControlFormat(ControlFormat.HankakuSuji);

		// 検索ボタンの関連付け
		btnSearch.Click += base.btnCom_Click;

	}

	/// <summary>
	/// 画面ロード時のイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void s03_0201_Load(object sender, System.EventArgs e)
	{
		//画面起動時の初期設定
		this.setControlInitiarize();

	}
	#endregion


	#region フォーム
	private void s03_0201_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			base.btnCom_Click(this.btnSearch, e);
		}
		else
		{
			return;
		}
	}
	#endregion

	#region コントロールイベント

	#region    Close時
	/// <summary>
	/// 画面クロージング時
	/// </summary>
	protected override bool closingScreen()
	{
		return true;
	}
	#endregion

	#region F2(戻る)ボタン押下時
	/// <summary>
	/// F2(戻る)ボタン押下時
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		//戻る
		closeCheckFlg = true;
		base.closeFormFlg = true;
		this.Close();
	}
	#endregion

	#region F8(検索)ボタン押下時
	/// <summary>
	/// F8(検索)ボタン押下時
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{
		//チェックして検索
		if (checkSearchItems() == false)
		{
			return;
		}
		else
		{
			//データの取得
			searchResultChkData = getCrsData();

			if (searchResultChkData.Rows.Count == 0)
			{
				//取得件数0件の場合、メッセージを表示
				CommonProcess.createFactoryMsg().messageDisp("E90_019");
			}
			else
			{
				//メニューの表示制御
				this.procSearch(1);

				//'検索後処理 引渡しパラメータセット
				//Call Me.baseData()
			}
		}
	}
	#endregion

	#region 条件クリア
	/// <summary>
	/// 条件クリアボタン押下時
	/// </summary>
	protected override void btnCLEAR_ClickOrgProc()
	{
		// 検索条件部の項目初期化
		this.procSearch(0);
	}
	#endregion

	#region 表示クリア
	/// <summary>
	/// 表示クリアボタン押下時
	/// </summary>
	protected void btnDispClear_ClickOrgProc()
	{
		// 選択対象の表示をクリア
		this.gbxMenuButton.Visible = false;
		// 検索ボタンを活性
		this.btnSearch.Enabled = true;
		//入力項目活性化
		this.dtmSyuptDayFromTo.Enabled = true;
		this.ucoCrsCd.Enabled = true;
		this.ucoNoribaCd.Enabled = true;
		this.chkMonday.Enabled = true;
		this.chkTuesday.Enabled = true;
		this.chkWednesday.Enabled = true;
		this.chkThursday.Enabled = true;
		this.chkFriday.Enabled = true;
		this.chkSaturday.Enabled = true;
		this.chkSunday.Enabled = true;
		this.txtGousya.Enabled = true;
		this.chkUnkyu.Enabled = true;
		this.chkHaishi.Enabled = true;
		//Me.chkMaruzo.Enabled = True
		//表示クリアボタンを非活性
		this.btnDispClear.Enabled = false;
	}
	#endregion

	#region 検索条件部の項目初期化
	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	protected void initSearchAreaItems()
	{
		CommonUtil.Control_Init(this.gbxCondition.Controls);
		//背景色初期化はここ
	}
	#endregion

	#region 検索結果部の項目初期化
	protected void initDetailAreaItems()
	{
		CommonUtil.Control_Init(this.gbxCondition.Controls);
		//背景色初期化はここ
	}
	#endregion

	#region チェックボックス入力値設定
	//月曜日
	protected void chkMonday_Check()
	{
		if (chkMonday.Checked == true)
		{
			getsuyobiCd = System.Convert.ToString(2);
		}
		else
		{
			getsuyobiCd = "";
		}

	}

	//火曜日
	protected void chkTuesday_Check()
	{
		if (chkTuesday.Checked == true)
		{
			kayobiCd = System.Convert.ToString(3);
		}
		else
		{
			kayobiCd = "";
		}

	}

	//水曜日
	protected void chkWednesday_Check()
	{
		if (chkWednesday.Checked == true)
		{
			suiyobiCd = System.Convert.ToString(4);
		}
		else
		{
			suiyobiCd = "";
		}

	}

	//木曜日
	protected void chkThursday_Checked()
	{
		if (chkThursday.Checked == true)
		{
			mokuyobiCd = System.Convert.ToString(5);
		}
		else
		{
			mokuyobiCd = "";
		}

	}

	//金曜日
	protected void chkFriday_Checked()
	{
		if (chkFriday.Checked == true)
		{
			kinyobiCd = System.Convert.ToString(6);
		}
		else
		{
			kinyobiCd = "";
		}

	}

	//土曜日
	protected void chkSaturday_Checked()
	{
		if (chkSaturday.Checked == true)
		{
			doyobiCd = System.Convert.ToString(7);
		}
		else
		{
			doyobiCd = "";
		}

	}

	//日曜日
	protected void chkSunday_Checked()
	{
		if (chkSunday.Checked == true)
		{
			nichiyobiCd = System.Convert.ToString(1);
		}
		else
		{
			nichiyobiCd = "";
		}
	}

	//運休含む
	protected void chkUnkyu_Checked()
	{
		if (chkUnkyu.Checked == true)
		{
			unkyuFlg = "1";
		}
		else
		{
			unkyuFlg = "";
		}

	}

	//廃止含む
	protected void chkHaishi_Checked()
	{
		if (chkHaishi.Checked == true)
		{
			haishiFlg = "1";
		}
		else
		{
			haishiFlg = "";
		}

	}

	//○増のみ
	//Protected Sub chkMaruzo_Checked() Handles chkMaruzo.Click
	//If chkMaruzo.Checked = True Then
	//       maruzoFlg = "1"
	//Else
	//       maruzoFlg = ""
	//End If

	//End Sub

	#endregion


	#endregion

	#region チェック系
	/// <summary>
	/// 検索入力項目チェック
	/// </summary>
	protected bool checkSearchItems()
	{
		return System.Convert.ToBoolean(checkSearch());
	}

	#endregion

	#endregion

	#region DB関連
	//■■■ＤＢ関連処理■■■
	/// <summary>
	/// 対象コースのデータ取得
	/// </summary>
	protected DataTable getCrsData()
	{
		return getDbTable();
	}

	/// <summary>
	/// 対象コースのデータ再取得・引渡しパラメータ設定渡し
	/// </summary>
	protected void getCrsReData()
	{
		//データの取得
		searchResultChkData = getCrsData();

		if (searchResultChkData.Rows.Count == 0)
		{
			//取得件数0件の場合、メッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_019");
			nothingFlg = false;
		}
		else
		{
			nothingFlg = true;
			//引渡しパラメータセット（遷移前にDBのデータを再取得して、パラメータ設定）
			this.baseData();
		}

	}

	#endregion

	#region チェック処理
	/// <summary>
	/// 検索処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private object checkSearch()
	{
		//エラーの初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		// 必須項目のチェック
		if (CommonUtil.checkHissuError(this.gbxCondition.Controls) == true)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			return false;
		}

		// From/To値
		// From/Toに入力がなかった場合、出発日From・出発日Toを設定
		if (ReferenceEquals(dtmSyuptDayFromTo.ToDateText, null) == true)
		{
			dtmSyuptDayFromTo.ToDateText = this.dtmSyuptDayFromTo.FromDateText;
		}
		else if (ReferenceEquals(dtmSyuptDayFromTo.FromDateText, null) == true)
		{
			dtmSyuptDayFromTo.FromDateText = this.dtmSyuptDayFromTo.ToDateText;
		}

		//出発日FromToの関係性チェック
		if (this.isExistFromToError() == false)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_052", "出発日From", "出発日To");
			return false;
		}

		//日付期間チェック
		if (this.isExistDayRange() == false)
		{
			return false;
		}

		// 必須項目の背景色を正常時の状態へ設定
		dtmSyuptDayFromTo.ExistErrorForFromDate = false;
		dtmSyuptDayFromTo.ExistErrorForToDate = false;
		ucoCrsCd.ExistError = false;

		return true;
	}
	#endregion

	#region チェック処理（詳細）
	/// <summary>
	/// FromTo関係性チェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isExistFromToError()
	{
		bool returnValue = true;
		//出発日From＞出発日Toである場合
		if (CommonDateUtil.chkDayFromTo(System.Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText), System.Convert.ToDateTime(dtmSyuptDayFromTo.ToDateText)) == false)
		{
			//エラーを検出したらFalseとする
			returnValue = false;
			// 背景色を入力不足の色に変更する
			dtmSyuptDayFromTo.ExistErrorForFromDate = true;
			dtmSyuptDayFromTo.ExistErrorForToDate = true;
		}
		return returnValue;
	}

	/// <summary>
	/// 日付期間チェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isExistDayRange()
	{
		bool returnValue = true;

		DateTime FirstDay = new DateTime(dtToday.Year, dtToday.Month, 1); //システム日付の月初日
		DateTime ZenFirstDay = FirstDay.AddMonths(-1); //システム日付の前月月初日
		DateTime YokYokFirstDay = FirstDay.AddMonths(2); //システム日付の翌々月月初日
		DateTime YokLastDay = YokYokFirstDay.AddDays(-1); //システム日付の翌月月末日
		DateTime fromFirstDay = new DateTime(System.Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText).Year, System.Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText).Month, 1); //From日付の月初日
		DateTime fromYokYokFirstDay = fromFirstDay.AddMonths(3); //from日付の翌々々月月初日
		DateTime fromYokLastDay = fromYokYokFirstDay.AddDays(-1); //from日付の翌々月月末日

		//出発日Fromが前月1日から期間外の場合
		//If ZenFirstDay > dtmSyuptDayFromTo.FromDateText Or dtmSyuptDayFromTo.FromDateText > YokLastDay Then
		if (ZenFirstDay > dtmSyuptDayFromTo.FromDateText)
		{
			//エラーを検出したらFalseとする
			returnValue = false;
			// 背景色を入力不足の色に変更する
			dtmSyuptDayFromTo.ExistErrorForFromDate = true;
			//エラーメッセージを出力
			CommonProcess.createFactoryMsg().messageDisp("E90_011", "前月1日以降の出発日");
			return returnValue;
		}

		//出発日Toが出発日Fromから翌月月末日の期間外の場合
		//If ZenFirstDay > dtmSyuptDayFromTo.ToDateText Or dtmSyuptDayFromTo.ToDateText > YokLastDay Then
		if (dtmSyuptDayFromTo.ToDateText > fromYokLastDay)
		{
			//エラーを検出したらFalseとする
			returnValue = false;
			// 背景色を入力不足の色に変更する
			dtmSyuptDayFromTo.ExistErrorForToDate = true;
			//エラーメッセージを出力
			CommonProcess.createFactoryMsg().messageDisp("E90_066", "出発日開始", "2ヵ月後の月末日まで", "出発日終了");
			return returnValue;
		}

		return returnValue;

	}

	#endregion


	#region 引渡し値設定
	public void baseData()
	{
		DataRow selectData = null;

		//取得データの先頭行を取得
		selectData = this.searchResultChkData.Rows(0);

		// 引渡し値
		// 取得データを引渡し値に設定
		m_form = new ResearchData();

		m_form.DepartureDayFrom = System.Convert.ToDateTime(this.dtmSyuptDayFromTo.FromDateText);
		m_form.DepartureDayTo = System.Convert.ToDateTime(this.dtmSyuptDayFromTo.ToDateText);
		m_form.CrsCd_Hedder = this.ucoCrsCd.CodeText;
		m_form.CrsName_Hedder = this.ucoCrsCd.ValueText;
		m_form.TeikiKikakuKbn = (Teiki_KikakuKbnType)(selectData["TEIKI_KIKAKU_KBN"]);
		m_form.CoruseSyuBetsu = (CrsKindType)(selectData["CRS_KIND"]);
		m_form.SyuptjiCarrier = (SyuptJiCarrierKbnType)(selectData["SYUPT_JI_CARRIER_KBN"]);
		m_form.Prmtable = this.searchResultChkData;

		// 取得データから不要な項目を削除
		m_form.Prmtable.Columns.Remove("TEIKI_KIKAKU_KBN");
		m_form.Prmtable.Columns.Remove("CRS_KIND");
		m_form.Prmtable.Columns.Remove("ACCESS_CD");
		m_form.Prmtable.Columns.Remove("DOUBLE_DECKER_FLG");
		m_form.Prmtable.Columns.Remove("SYUPT_JI_CARRIER_KBN");

		// 引渡し値（発着用）
		// 取得データを引渡し値に設定
		m_form.Prmtable_Hattyaku = this.searchResultChkData.Copy;

		// 入力がない場合取得データを引渡さない（発着変更用）
		if (ucoNoribaCd.CodeText == "" || ucoNoribaCd.CodeText == string.Empty)
		{
			// 配車経由地
			m_form.Prmtable_Hattyaku.Columns.Remove("HAISYA_KEIYU_CD_1");
			m_form.Prmtable_Hattyaku.Columns.Add("HAISYA_KEIYU_CD_1");
			// 出発場所コードキャリア
			m_form.Prmtable_Hattyaku.Columns.Remove("SYUPT_PLACE_CD_CARRIER");
			m_form.Prmtable_Hattyaku.Columns.Add("SYUPT_PLACE_CD_CARRIER");
		}

		// 引渡し値（座席使用中フラグ構築用）
		// 取得データを引渡し値に設定
		m_form.Prmtable_zaseki = this.searchResultChkData.Copy;

	}

	#endregion

	#region DB処理
	/// <summary>
	/// 検索処理(必須画面個別実装)
	/// ※テーブル複数などは未対応
	/// </summary>
	/// <returns>取得データ(DataTable)</returns>
	private DataTable getDbTable()
	{
		//戻り値
		DataTable returnValue = null;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		//DataAccessクラス生成
		DaityoMain_DA dataAccess = new DaityoMain_DA();

		//月数変換用変数
		string Month = null;
		string Day = null;
		string MonthFrom = null;
		string DayFrom = null;
		string MonthTo = null;
		string DayTo = null;

		// パラメータ設定
		// From値
		// 数値型へ変換
		string YearFrom = System.Convert.ToString(dtmSyuptDayFromTo.FromDateText.Value.Year);
		Month = "0" + dtmSyuptDayFromTo.FromDateText.Value.Month;
		if (2 <= Month.Length)
		{
			MonthFrom = Month.Substring(Month.Length - 2);
		}
		Day = "0" + dtmSyuptDayFromTo.FromDateText.Value.Day;
		if (2 <= Day.Length)
		{
			DayFrom = Day.Substring(Day.Length - 2);
		}

		int YMDFrom = int.Parse(YearFrom + MonthFrom + DayFrom);

		paramInfoList.Add("SYUPTDAYFROM", YMDFrom);
		// To値
		// Toに入力がなかった場合、出発日Fromを設定
		if (ReferenceEquals(dtmSyuptDayFromTo.ToDateText, null) == true)
		{
			dtmSyuptDayFromTo.ToDateText = this.dtmSyuptDayFromTo.FromDateText;
		}

		// 数値型へ変換
		string YearTo = System.Convert.ToString(dtmSyuptDayFromTo.ToDateText.Value.Year);
		Month = "0" + dtmSyuptDayFromTo.ToDateText.Value.Month;
		if (2 <= Month.Length)
		{
			MonthTo = Month.Substring(Month.Length - 2);
		}

		Day = "0" + dtmSyuptDayFromTo.ToDateText.Value.Day;
		if (2 <= Day.Length)
		{
			DayTo = Day.Substring(Day.Length - 2);
		}

		int ymdTo = int.Parse(YearTo + MonthTo + DayTo);

		paramInfoList.Add("SYUPTDAYTO", ymdTo);
		// コースコード値
		paramInfoList.Add("CRSCD", ucoCrsCd.CodeText);
		// 乗車地コード値
		paramInfoList.Add("HAISYAKEIYUCD1", ucoNoribaCd.CodeText);
		// 号車値
		paramInfoList.Add("GOUSYA", txtGousya.Text);
		// 月曜日コード値
		paramInfoList.Add("GETSUYOBI_CD", getsuyobiCd);
		// 火曜日コード値
		paramInfoList.Add("KAYOBI_CD", kayobiCd);
		// 水曜日コード値
		paramInfoList.Add("SUIYOBI_CD", suiyobiCd);
		// 木曜日コード値
		paramInfoList.Add("MOKUYOBI_CD", mokuyobiCd);
		// 金曜日コード値
		paramInfoList.Add("KINYOBI_CD", kinyobiCd);
		// 土曜日コード値
		paramInfoList.Add("DOYOBI_CD", doyobiCd);
		// 日曜日コード値
		paramInfoList.Add("NICHIYOBI_CD", nichiyobiCd);
		// 運休フラグ値
		paramInfoList.Add("UNKYU", unkyuFlg);
		// 廃止フラグ値
		paramInfoList.Add("HAISHI", haishiFlg);
		// 〇増フラグ値
		//paramInfoList.Add("MARUZOUMANAGEMENTKBN", maruzoFlg)
		// 出発場所キャリアコード値
		paramInfoList.Add("SYUPT_PLACE_CD_CARRIER", ucoNoribaCd.CodeText);

		try
		{
			returnValue = dataAccess.AccessDaityoMainTehai(DaityoMain_DA.accessType.getDaityoMain, paramInfoList);

		}
		catch (OracleException)
		{
			throw;
		}
		catch (Exception)
		{
			throw;
		}

		return returnValue;

	}

	#endregion

	#region メニューボタン

	/// <summary>
	/// オプション
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0202_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0202 S03_0202 = new S03_0202();
		S03_0202.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0202);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}
	}

	/// <summary>
	/// カテゴリ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0203_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0203 S03_0203 = new S03_0203();
		S03_0203.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0203);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}

	}

	/// <summary>
	/// コース名
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0204_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0204 S03_0204 = new S03_0204();
		S03_0204.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0204);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}

	}

	/// <summary>
	/// コース情報
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0205_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0205 S03_0205 = new S03_0205();
		S03_0205.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0205);
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}
	}

	/// <summary>
	/// バス指定・乗車定員・受付限定人数
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0206_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0206 S03_0206 = new S03_0206();
		S03_0206.PrmData = m_form;
		// 遷移先画面を開く
		//openWindow(S03_0206)

		DataRow[] chkRow = m_form.Prmtable.Select("USING_FLG = 'Y'");
		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			if (chkRow.Count() > 0)
			{
				// 座席イメージ（バス情報）使用中フラグチェック（更新）
				comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.@lock, m_form.Prmtable_zaseki, this.Name, null, dtToday);
			}
			else
			{
				// 座席イメージ（バス情報）使用中フラグチェック（更新）
				comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.@lock, m_form.Prmtable_zaseki, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			}
			// 遷移先画面を開く
			openWindow(S03_0206);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.reject, m_form.Prmtable_zaseki, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}
	}

	/// <summary>
	/// メッセージ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0207_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0207 S03_0207 = new S03_0207();
		int msgType = 0;
		S03_0207.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 予約使用中フラグチェック（更新）
			if (m_form.Prmtable.Select(string.Format(TehaiCommon.usingFlgTableType.USING_FLG + "='{0}'", UsingFlg.Use)).Count == 0)
			{
				msgType = System.Convert.ToInt32(TehaiCommon.messageType.infoMeg);
			}
			comTehai.updateUsingFlgYoyaku(TehaiCommon.updateModeType.@lock, m_form.Prmtable, msgType);
			// 遷移先画面を開く
			openWindow(S03_0207);
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 予約使用中フラグの戻し処理
			comTehai.updateUsingFlgYoyaku(TehaiCommon.updateModeType.reject, m_form.Prmtable, null);
			// 台帳の戻し処理の事前処理(処理順番は変えないこと)
			comTehai.updateUsingFlgYoyaku(TehaiCommon.updateModeType.befReject, m_form.Prmtable, null);
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}
	}

	/// <summary>
	/// ルーム数
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0208_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0208 S03_0208 = new S03_0208();
		S03_0208.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0208);
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}

	}

	/// <summary>
	/// 受付開始日・予約停止・キャンセル料区分
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0209_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0209 S03_0209 = new S03_0209();
		S03_0209.PrmData = m_form;
		S03_0209.subTitle = this.btnS03_0209.Text;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0209);
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}

	}

	/// <summary>
	/// 台帳削除・廃止
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0210_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0210 S03_0210 = new S03_0210();
		S03_0210.PrmData = m_form;

		DataRow[] chkRow = m_form.Prmtable.Select("USING_FLG = 'Y'");

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			if (chkRow.Count() > 0)
			{
				// 座席イメージ（バス情報）使用中フラグチェック（更新）
				comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.@lock, m_form.Prmtable_zaseki, this.Name, null, dtToday);
			}
			else
			{
				// 座席イメージ（バス情報）使用中フラグチェック（更新）
				comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.@lock, m_form.Prmtable_zaseki, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			}
			// 遷移先画面を開く
			openWindow(S03_0210);
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.reject, m_form.Prmtable_zaseki, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}
	}

	/// <summary>
	/// 料金情報
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0211_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0211 S03_0211 = new S03_0211();
		S03_0211.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0211);
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}
	}

	/// <summary>
	/// 発着情報
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0213_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0213 S03_0213 = new S03_0213();
		S03_0213.prmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			//遷移先画面を開く
			openWindow(S03_0213);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}

	}

	/// <summary>
	/// 空席確保数
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0214_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0214 S03_0214 = new S03_0214();
		S03_0214.PrmData = m_form;

		DataRow[] chkRow = m_form.Prmtable.Select("USING_FLG = 'Y'");
		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			if (chkRow.Count() > 0)
			{
				// 座席イメージ（バス情報）使用中フラグチェック（更新）
				comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.@lock, m_form.Prmtable_zaseki, this.Name, null, dtToday);
			}
			else
			{
				// 座席イメージ（バス情報）使用中フラグチェック（更新）
				comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.@lock, m_form.Prmtable_zaseki, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			}
			// 遷移先画面を開く
			openWindow(S03_0214);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.reject, m_form.Prmtable_zaseki, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}
	}

	/// <summary>
	/// 補助席発売・１階席発売
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0215_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0215 S03_0215 = new S03_0215();
		S03_0215.PrmData = m_form;

		DataRow[] chkRow = m_form.Prmtable.Select("USING_FLG = 'Y'");
		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			if (chkRow.Count() > 0)
			{
				// 座席イメージ（バス情報）使用中フラグチェック（更新）
				comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.@lock, m_form.Prmtable_zaseki, this.Name, null, dtToday);
			}
			else
			{
				// 座席イメージ（バス情報）使用中フラグチェック（更新）
				comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.@lock, m_form.Prmtable_zaseki, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			}
			// 遷移先画面を開く
			openWindow(S03_0215);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.zasekiImage, TehaiCommon.updateModeType.reject, m_form.Prmtable_zaseki, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}
	}

	/// <summary>
	/// 販売課所
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0216_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0216 S03_0216 = new S03_0216();
		S03_0216.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0216);
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}

	}

	/// <summary>
	/// 有料座席
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0212_Click(object sender, EventArgs e)
	{


	}

	/// <summary>
	/// 車種・台数カウント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0217_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0217 S03_0217 = new S03_0217();
		S03_0217.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0217);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}

	}

	/// <summary>
	/// 運休
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0218_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0218 S03_0218 = new S03_0218();
		S03_0218.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0218);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}

	}

	/// <summary>
	/// 降車ヶ所・原価情報
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0219_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0219 S03_0219 = new S03_0219();
		S03_0219.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0219);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}
	}

	/// <summary>
	/// ﾚﾃﾞｨｰｽｼｰﾄ・媒体入力・座席指定方法
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnS03_0220_Click(object sender, EventArgs e)
	{
		//データの再取得
		getCrsReData();

		if (nothingFlg == false)
		{
			return;
		}

		S03_0220 S03_0220 = new S03_0220();
		S03_0220.PrmData = m_form;

		try
		{
			// 使用中フラグチェック（更新）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.@lock, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
			// 遷移先画面を開く
			openWindow(S03_0220);

		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			// 戻し処理（使用中フラグを取得時の状態へ戻す）
			comTehai.updateUsingFlg(TehaiCommon.targetTableType.crsMst, TehaiCommon.updateModeType.reject, m_form.Prmtable, this.Name, TehaiCommon.messageType.infoMeg, dtToday);
		}

	}

	#endregion

	#region メソッド

	/// <summary>
	/// 画面表示時の初期設定
	/// </summary>
	private void setControlInitiarize()
	{

		// フッタボタンの設定
		this.setButtonInitiarize();

		//初期化
		this.setControlInitialize();

	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{

		//Visible
		//ボタンの表示/非表示を変更
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
		//ボタンのテキストを変更(必要に応じて)
		//Me.F1Key_Text = "F1:"
		this.F2Key_Text = "F2:戻る";
		//Me.F3Key_Text = "F3:終了"
		//Me.F4Key_Text = "F4:削除"
		//Me.F5Key_Text = "F5:参照"
		//Me.F6Key_Text = "F6:ﾌﾟﾚﾋﾞｭｰ"
		//Me.F7Key_Text = "F7:印刷"
		//Me.F8Key_Text = "F8:検索"
		//Me.F9Key_Text = "F9:CSV出力"
		//Me.F10Key_Text = "F10:登録"
		//Me.F11Key_Text = "F11:"
		//Me.F12Key_Text = "F12:"

		//ボタンの使用可/非を変更(必要に応じて)
		//Me.F1Key_Enabled = False
		this.F2Key_Enabled = true;
		//Me.F3Key_Enabled = False
		//Me.F4Key_Enabled = False
		//Me.F5Key_Enabled = False
		//Me.F6Key_Enabled = False
		//Me.F7Key_Enabled = False
		//Me.F8Key_Enabled = True
		//Me.F9Key_Enabled = False
		//Me.F10Key_Enabled = False
		//Me.F11Key_Enabled = False
		//Me.F12Key_Enabled = False

	}

	/// <summary>
	/// 表示項目初期化
	/// </summary>
	private void setControlInitialize()
	{

		//エラーの初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		//項目初期化
		this.dtmSyuptDayFromTo.ToDateText = null;
		this.dtmSyuptDayFromTo.FromDateText = dtToday;
		this.ucoCrsCd.CodeText = string.Empty;
		this.ucoCrsCd.ValueText = string.Empty;
		this.ucoNoribaCd.CodeText = string.Empty;
		this.ucoNoribaCd.ValueText = string.Empty;

		this.chkMonday.Checked = false;
		this.getsuyobiCd = "";
		this.chkTuesday.Checked = false;
		this.kayobiCd = "";
		this.chkWednesday.Checked = false;
		this.suiyobiCd = "";
		this.chkThursday.Checked = false;
		this.mokuyobiCd = "";
		this.chkFriday.Checked = false;
		this.kinyobiCd = "";
		this.chkSaturday.Checked = false;
		this.doyobiCd = "";
		this.chkSunday.Checked = false;
		this.nichiyobiCd = "";

		this.txtGousya.Text = string.Empty;
		this.chkUnkyu.Checked = false;
		this.unkyuFlg = "";
		this.chkHaishi.Checked = false;
		this.haishiFlg = "";
		//Me.chkMaruzo.Checked = False
		//Me.maruzoFlg = ""

		this.gbxMenuButton.Visible = false;
		this.btnSearch.Text = "F8:検索";
		this.btnClear.Enabled = true;
		this.btnDispClear.Enabled = false;

		//フォーカス設定
		//dtmSyuptDayFromTo.Select()
		dtmSyuptDayFromTo.FocusFromDate();

	}

	/// <summary>
	/// 検索ボタン処理
	/// </summary>
	private void procSearch(int mode)
	{

		if (mode == 1)
		{

			this.gbxMenuButton.Visible = true;

			this.btnSearch.Enabled = false;

			this.btnClear.Enabled = true;
			this.btnDispClear.Enabled = true;

			//入力項目非活性化
			this.dtmSyuptDayFromTo.Enabled = false;
			this.ucoCrsCd.Enabled = false;
			this.ucoNoribaCd.Enabled = false;
			this.chkMonday.Enabled = false;
			this.chkTuesday.Enabled = false;
			this.chkWednesday.Enabled = false;
			this.chkThursday.Enabled = false;
			this.chkFriday.Enabled = false;
			this.chkSaturday.Enabled = false;
			this.chkSunday.Enabled = false;
			this.txtGousya.Enabled = false;
			this.chkUnkyu.Enabled = false;
			this.chkHaishi.Enabled = false;
			//Me.chkMaruzo.Enabled = False

			//コース種別判定
			//コース種別データ
			DataRow selectData = null;

			//取得データの先頭行を取得
			selectData = this.searchResultChkData.Rows(0);


			//メニューボタン表示可否の設定
			//コース種別で表示ボタンを制御
			this.changeDispButton((CrsKindType)(selectData["CRS_KIND"]));

		}
		else
		{


			this.gbxMenuButton.Visible = true;

			this.btnSearch.Enabled = true;

			this.btnClear.Enabled = false;

			//入力項目活性化
			this.dtmSyuptDayFromTo.Enabled = true;
			this.ucoCrsCd.Enabled = true;
			this.ucoNoribaCd.Enabled = true;
			this.chkMonday.Enabled = true;
			this.chkTuesday.Enabled = true;
			this.chkWednesday.Enabled = true;
			this.chkThursday.Enabled = true;
			this.chkFriday.Enabled = true;
			this.chkSaturday.Enabled = true;
			this.chkSunday.Enabled = true;
			this.txtGousya.Enabled = true;
			this.chkUnkyu.Enabled = true;
			this.chkHaishi.Enabled = true;
			//Me.chkMaruzo.Enabled = True

			// 表示項目初期化
			this.setControlInitialize();

		}

	}

	/// <summary>
	/// メニューボタン表示可否の設定
	/// </summary>
	/// <param name="courseType"></param>
	private void changeDispButton(CrsKindType courseType)
	{

		// -------------------------------------------------------------
		// [修正対象選択]グループ内の、全ボタンを一旦、非表示にする
		// -------------------------------------------------------------
		Control[] controls = getAllControls(this.gbxMenuButton);
		foreach (Control cControl in controls)
		{
			if (cControl is Button)
			{
				cControl.Visible = false;
			}
		}

		// -------------------------------------------------------------
		// 表示するボタンのみ設定
		// -------------------------------------------------------------
		// kbn の定数 は暫定
		if (courseType == CrsKindType.hatoBusTeiki)
		{
			// メッセージ
			this.btnS03_0207.Visible = true;

			// コース情報
			this.btnS03_0205.Visible = true;

			// 販売課所
			this.btnS03_0216.Visible = true;

			// 有料座席
			this.btnS03_0212.Visible = true;

			// 降車ヶ所・原価情報
			this.btnS03_0219.Visible = true;

			// 運休
			this.btnS03_0218.Text = "運休";
			this.btnS03_0218.Visible = true;

			// 車種・台数カウント
			this.btnS03_0217.Visible = true;

			// バス指定・乗車定員・受付限定人数
			this.btnS03_0206.Text = "バス指定・乗車定員・受付限定人数";
			this.btnS03_0206.Visible = true;

			// オプション
			this.btnS03_0202.Visible = true;

			// 発着情報
			this.btnS03_0213.Visible = true;

			// コース名
			this.btnS03_0204.Visible = true;
			//' 部署が本社でない場合
			if (UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote)
			{
				this.btnS03_0204.Enabled = false;
			}

			// 補助席発売・１階席発売
			// ※車種が２階建てかつ、出発日が単日指定かつ当日の場合のみ表示
			// 出発日が単日指定である場合
			if (this.dtmSyuptDayFromTo.FromDateText == this.dtmSyuptDayFromTo.ToDateText)
			{

				DateTime syuptDay = System.Convert.ToDateTime(this.dtmSyuptDayFromTo.FromDateText);
				// 出発日当日である場合
				if (syuptDay.Date == dtToday.Date)
				{
					// 2階建てチェックデータ
					DataRow seatChkData = null;

					// 取得データの先頭行を取得
					seatChkData = this.searchResultChkData.Rows(0);

					// 2階建てフラグチェック
					if (System.Convert.ToString(seatChkData["DOUBLE_DECKER_FLG"]) == "1")
					{
						this.btnS03_0215.Text = "１階席販売（当日のみ）";
						this.btnS03_0215.Visible = true;
					}

				}
			}

			// 空席確保数
			this.btnS03_0214.Visible = true;

			// 受付開始日・予約停止・キャンセル料区分
			this.btnS03_0209.Text = "受付開始日・予約停止";
			this.btnS03_0209.Visible = true;

			// ﾚﾃﾞｨｰｽｼｰﾄ・媒体入力・座席指定方法
			this.btnS03_0220.Visible = true;

			// 台帳削除・廃止
			this.btnS03_0210.Text = "台帳削除・廃止";
			this.btnS03_0210.Visible = true;
			// 部署が本社でない場合
			if (UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote && UserInfoManagement.sectionBusyoCd != SectionBusyoCd.InternationalPlanninng && UserInfoManagement.sectionBusyoCd != SectionBusyoCd.SalesPromote)
			{
				this.btnS03_0210.Enabled = false;
			}

			// 料金情報
			this.btnS03_0211.Visible = true;
			// 部署が本社、予約センターでない場合
			if ((UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote) && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.ReserveCenter)
			{
				this.btnS03_0211.Enabled = false;
			}
		}
		else if (courseType == courseType.higaeri)
		{
			// メッセージ
			this.btnS03_0207.Visible = true;

			// コース情報
			this.btnS03_0205.Visible = true;

			// 販売課所
			this.btnS03_0216.Visible = true;

			// 有料座席
			this.btnS03_0212.Visible = true;

			// 降車ヶ所・原価情報
			this.btnS03_0219.Visible = true;

			// 車種・台数カウント
			this.btnS03_0217.Visible = true;

			// バス指定・乗車定員・受付限定人数
			this.btnS03_0206.Text = "バス指定・乗車定員・受付限定人数";
			this.btnS03_0206.Visible = true;

			// オプション
			this.btnS03_0202.Visible = true;

			// 発着情報
			this.btnS03_0213.Visible = true;

			// コース名
			this.btnS03_0204.Visible = true;
			// 部署が本社でない場合
			if (UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote)
			{
				this.btnS03_0204.Enabled = false;
			}

			// 補助席発売・１階席発売
			this.btnS03_0215.Text = "補助席発売";
			this.btnS03_0215.Visible = true;

			// 受付開始日・予約停止・キャンセル料区分
			this.btnS03_0209.Text = "受付開始日・予約停止・キャンセル料区分";
			this.btnS03_0209.Visible = true;

			// カテゴリ
			this.btnS03_0203.Visible = true;

			// ﾚﾃﾞｨｰｽｼｰﾄ・媒体入力・座席指定方法
			this.btnS03_0220.Visible = true;

			// 台帳削除・廃止
			this.btnS03_0210.Text = "台帳削除・廃止";
			this.btnS03_0210.Visible = true;
			// 部署が本社でない場合
			if (UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote && UserInfoManagement.sectionBusyoCd != SectionBusyoCd.InternationalPlanninng && UserInfoManagement.sectionBusyoCd != SectionBusyoCd.SalesPromote)
			{
				this.btnS03_0210.Enabled = false;
			}

			// 料金情報
			this.btnS03_0211.Visible = true;
			//' 部署が本社、予約センターでない場合
			if ((UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote) && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.ReserveCenter)
			{
				this.btnS03_0211.Enabled = false;
			}
		}
		else if (courseType == courseType.rcourse)
		{
			// メッセージ
			this.btnS03_0207.Visible = true;

			// コース情報
			this.btnS03_0205.Visible = true;

			// 販売課所
			this.btnS03_0216.Visible = true;

			// 有料座席
			this.btnS03_0212.Visible = true;

			// 降車ヶ所・原価情報
			this.btnS03_0219.Visible = true;

			// 車種・台数カウント
			this.btnS03_0217.Visible = true;

			// バス指定・乗車定員・受付限定人数
			this.btnS03_0206.Text = "バス指定・乗車定員・受付限定人数";
			this.btnS03_0206.Visible = true;

			// オプション
			this.btnS03_0202.Visible = true;

			// 発着情報
			this.btnS03_0213.Visible = true;

			// コース名
			this.btnS03_0204.Visible = true;
			// 部署が本社でない場合
			if (UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote)
			{
				this.btnS03_0204.Enabled = false;
			}

			// 補助席発売・１階席発売
			this.btnS03_0215.Text = "補助席発売";
			this.btnS03_0215.Visible = true;

			// 受付開始日・予約停止・キャンセル料区分
			this.btnS03_0209.Text = "受付開始日・予約停止・キャンセル料区分";
			this.btnS03_0209.Visible = true;

			// カテゴリ
			this.btnS03_0203.Visible = true;

			// ﾚﾃﾞｨｰｽｼｰﾄ・媒体入力・座席指定方法
			this.btnS03_0220.Visible = true;

			// 台帳削除・廃止
			this.btnS03_0210.Text = "台帳削除・廃止";
			this.btnS03_0210.Visible = true;
			// 部署が本社でない場合
			if (UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote && UserInfoManagement.sectionBusyoCd != SectionBusyoCd.InternationalPlanninng && UserInfoManagement.sectionBusyoCd != SectionBusyoCd.SalesPromote)
			{
				this.btnS03_0210.Enabled = false;
			}

			// 料金情報
			this.btnS03_0211.Visible = true;
			//' 部署が本社、予約センターでない場合
			if ((UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote) && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.ReserveCenter)
			{
				this.btnS03_0211.Enabled = false;
			}
		}
		else if (courseType == courseType.stay)
		{
			// メッセージ
			this.btnS03_0207.Visible = true;

			// コース情報
			this.btnS03_0205.Visible = true;

			// 販売課所
			this.btnS03_0216.Visible = true;

			// 有料座席
			this.btnS03_0212.Visible = true;

			// 降車ヶ所・原価情報
			this.btnS03_0219.Visible = true;

			// 車種・台数カウント
			this.btnS03_0217.Visible = true;

			// バス指定・乗車定員・受付限定人数
			this.btnS03_0206.Text = "バス指定・乗車定員・受付限定人数";
			this.btnS03_0206.Visible = true;

			// オプション
			this.btnS03_0202.Visible = true;

			// 発着情報
			this.btnS03_0213.Visible = true;

			// コース名
			this.btnS03_0204.Visible = true;
			//' 部署が本社でない場合
			if (UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote)
			{
				this.btnS03_0204.Enabled = false;
			}

			// 補助席発売・１階席発売
			this.btnS03_0215.Text = "補助席発売";
			this.btnS03_0215.Visible = true;

			// ルーム数等
			this.btnS03_0208.Visible = true;

			// 受付開始日・予約停止・キャンセル料区分
			this.btnS03_0209.Text = "受付開始日・予約停止・キャンセル料区分";
			this.btnS03_0209.Visible = true;

			// カテゴリ
			this.btnS03_0203.Visible = true;

			// ﾚﾃﾞｨｰｽｼｰﾄ・媒体入力・座席指定方法
			this.btnS03_0220.Visible = true;

			// 台帳削除・廃止
			this.btnS03_0210.Text = "台帳削除・廃止";
			this.btnS03_0210.Visible = true;
			//' 部署が本社でない場合
			if (UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote && UserInfoManagement.sectionBusyoCd != SectionBusyoCd.InternationalPlanninng && UserInfoManagement.sectionBusyoCd != SectionBusyoCd.SalesPromote)
			{
				this.btnS03_0210.Enabled = false;
			}

			// 料金情報
			this.btnS03_0211.Visible = true;
			//' 部署が本社、予約センターでない場合
			if ((UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote) && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.ReserveCenter)
			{
				this.btnS03_0211.Enabled = false;
			}
		}
		else if (courseType == courseType.capital)
		{
			// ホテル商品チェック
			DataRow hotelCommoditiesChk = null;

			// 取得データの先頭行を取得
			hotelCommoditiesChk = this.searchResultChkData.Rows(0);

			// 定期・企画区分が定期である場合
			if (hotelCommoditiesChk["TEIKI_KIKAKU_KBN"].ToString() == System.Convert.ToString(FixedCd.TeikiKikakuKbn.Teiki))
			{
				// 部署がキャピタルである場合
				if (UserInfoManagement.eigyosyoKbn == EigyosyoKbn.Capital || UserInfoManagement.eigyosyoKbn == EigyosyoKbn.CapitalPlanning)
				{
					// バス指定・乗車定員・受付限定人数
					this.btnS03_0206.Text = "乗車定員";
					this.btnS03_0206.Visible = true;
					// 部署が本社である場合
				}
				else if (UserInfoManagement.eigyosyoKbn == EigyosyoKbn.Honsya || UserInfoManagement.eigyosyoKbn == EigyosyoKbn.SystemPromote)
				{
					// メッセージ
					this.btnS03_0207.Visible = true;

					// 運休
					this.btnS03_0218.Text = "運休";
					this.btnS03_0218.Visible = true;

					// バス指定・乗車定員・受付限定人数
					this.btnS03_0206.Text = "乗車定員";
					this.btnS03_0206.Visible = true;
				}
			}
			else
			{

				// メッセージ
				this.btnS03_0207.Visible = true;
				// 運休
				this.btnS03_0218.Text = "運休";
				this.btnS03_0218.Visible = true;

				// バス指定・乗車定員・受付限定人数
				this.btnS03_0206.Text = "乗車定員";
				this.btnS03_0206.Visible = true;

				// コース名
				this.btnS03_0204.Visible = true;
				//' 部署が本社でない場合
				if (UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote)
				{
					this.btnS03_0204.Enabled = false;
				}

				// 受付開始日・予約停止・キャンセル料区分
				this.btnS03_0209.Text = "予約停止";
				this.btnS03_0209.Visible = true;

				// 台帳削除・廃止
				this.btnS03_0210.Text = "台帳削除";
				this.btnS03_0210.Visible = true;
				// 部署が本社でない場合
				if (UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote && UserInfoManagement.sectionBusyoCd != SectionBusyoCd.InternationalPlanninng && UserInfoManagement.sectionBusyoCd != SectionBusyoCd.SalesPromote)
				{
					this.btnS03_0210.Enabled = false;
				}

				// 料金情報
				this.btnS03_0211.Visible = true;
				//' 部署が本社、予約センターでない場合
				if ((UserInfoManagement.eigyosyoKbn != EigyosyoKbn.Honsya && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.SystemPromote) && UserInfoManagement.eigyosyoKbn != EigyosyoKbn.ReserveCenter)
				{
					this.btnS03_0211.Enabled = false;
				}
			}
		}

	}

	private Control[] getAllControls(Control top)
	{
		ArrayList buf = new ArrayList();
		foreach (Control c in top.Controls)
		{
			buf.Add(c);
			buf.AddRange(getAllControls(c));
		}
		return ((Control[])(buf.ToArray(typeof(Control))));
	}

	~S03_0201()
	{
		//base.Finalize();
	}
	#endregion

}