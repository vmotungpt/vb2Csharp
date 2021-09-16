using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.Master;
using Hatobus.ReservationManagementSystem.Unkou.S04_1004_DA;


public class S04_1004 : PT99, iPT99
{

	#region フィールド
	private const string E90_022 = "E90_022";
	private const int PLACE_CD_LENGTH = 3;
	private const char PADDING_SPACE = ' ';
	#endregion

	#region 定数

	#endregion

	#region 列挙体
	private enum columnList : int
	{
		[Value("出発日")]
		SYUPT_DAY,
		[Value("コースコード")]
		CRS_CD,
		[Value("コース名")]
		CRS_NAME,
		[Value("号車")]
		GOUSYA,
		[Value("車番")]
		CAR_NO,
		[Value("運行運転手")]//TODO QA_268
		DRIVER,
		[Value("ガイド名")]//TODO QA_268
		GUIDE_NAME,
		[Value("出発時間１")]
		SYUPT_TIME_1,
		[Value("配車経由コード")]
		PLACE_NAME,
		[Value("営業所名１")]
		EIGYOSYO_NAME_1
	}
	#endregion

	#region 画面に実装(Interface)
	/// <summary>
	/// パターンの初期設定を行う
	/// (呼出し元から呼び出される想定)
	/// </summary>
	void iPT99.iPt99StartSetting()
	{
		this.patternSettings();
	}

	public void patternSettings()
	{
		//[ボタン制御用]
		//[CSV出力](F4)有無設定
		base.PtIsCsvOutFlg = false;
		//[ﾌﾟﾚﾋﾞｭｰ](F5)有無設定
		base.PtIsPrevFlg = false;
		//[印刷/出力](F7)有無設定
		base.PtIsPrintFlg = true;
		//[検索](F8)有無設定
		base.PtIsSearchFlg = false;
		//[登録](F10)有無設定
		base.PtIsRegFlg = false;
		//[更新](F11)有無設定
		base.PtIsUpdFlg = false;

		//[検索領域]コントロール設定
		base.PtSearchControl = gbxSearch;
		//[検索結果領域]コントロール設定
		base.PtResultControl = null;
		//[検索詳細領域]コントロール設定
		base.PtDetailControl = null;

		//[帳票用]帳票タイプ
		base.PtPrintType = PRINTTYPE.AR;
		//[帳票用]DS用帳票ID
		//MyBase.PtDsPrintId = Nothing

		//親画面に自分を設定
		base.PtMyForm = this;
	}


	/// <summary>
	/// データデフォルト値設定
	/// </summary>
	/// <param name="area"></param>
	void iPT99.iPt99SetDefValue(AREA area)
	{
		this.setDefValue(area);
	}

	public void setDefValue(AREA area)
	{
		if (area == area.FORM)
		{
		}
		else if (area == area.BUTTON)
		{
			//なし
		}
	}

	/// <summary>
	/// フォームクローズ時処理
	/// (呼出し元から呼び出される想定)
	/// </summary>
	/// <returns></returns>
	public bool iPt99Closing()
	{
		return true;
	}

	#endregion

	#region 画面に実装(Overrides)
	/// <summary>
	/// 初期処理
	/// </summary>
	protected override void OvPt99Init()
	{
		//初期化処理(PT99)
		base.OvPt99Init();
		btnClear.Click += base.btnCom_Click;
		this.ActiveControl = this.dtmSyuptDay;
		ucoCrsKind.SetInitState();
	}

	#region 出力処理用
	///' <summary>
	///' 出力前(エラーチェック)処理
	///' </summary>
	///' <returns></returns>
	protected override bool OvPt99PrintBefore()
	{
		//背景色初期化
		base.clearExistErrorProperty(this.gbxSearch.Controls);

		//必須項目のチェック
		if (CommonUtil.checkHissuError(this.gbxSearch.Controls) == true)
		{
			CommonProcess.createFactoryMsg().messageDisp(E90_022);
			return false;
		}
		return true;
	}
	/// <summary>
	/// AR出力時(データ取得)処理
	/// </summary>
	/// <returns></returns>
	protected override DataTable OvPt99PrintARGetData()
	{
		//'DataAccessクラス生成
		S04_1004_DA dataAccess = new S04_1004_DA();
		//DA定義の検索処理（営業所名取得）を呼び出す
		S04_1004DASelectSaleOfficeNameParam paramOfficeName = new S04_1004DASelectSaleOfficeNameParam();
		paramOfficeName.Jyoshachi = this.ucoNoribaCd.CodeText;
		object dtOfficeName = dataAccess.selectDataTableSaleOfficeName(paramOfficeName);

		//DA定義の検索処理（コース情報取得）を呼び出す
		S04_1004DASelectCourseParam paramCrsInfo = new S04_1004DASelectCourseParam();
		//出発日
		paramCrsInfo.SyuptDay = this.dtmSyuptDay.ValueInt;
		//乗車地
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.ucoNoribaCd.CodeText)) && this.ucoNoribaCd.CodeText.Length < PLACE_CD_LENGTH)
		{
			paramCrsInfo.Jyoshachi = this.ucoNoribaCd.CodeText.PadRight(PLACE_CD_LENGTH, PADDING_SPACE);
		}
		else
		{
			paramCrsInfo.Jyoshachi = this.ucoNoribaCd.CodeText;
		}
		//出発時間
		if (this.dtmSyuptTime.Value.HasValue)
		{
			paramCrsInfo.SyuptTime = this.dtmSyuptTime.Value24Int;
		}
		//日本語
		if (this.ucoCrsKind.JapaneseState == true)
		{
			paramCrsInfo.CrsJapanese = this.ucoCrsKind.JapaneseState;
		}
		//外国語
		if (this.ucoCrsKind.ForeignState == true)
		{
			paramCrsInfo.CrsForeign = this.ucoCrsKind.ForeignState;
		}
		//定期（昼）
		if (this.chkNoon.Checked == true)
		{
			paramCrsInfo.CrsKbnHiru = this.chkNoon.Checked;
		}
		//定期（夜）
		if (this.chkNight.Checked == true)
		{
			paramCrsInfo.CrsKbnYoru = this.chkNight.Checked;
		}
		//企画（日帰り）
		if (this.chkKikakuDayTrip.Checked == true)
		{
			paramCrsInfo.CrsKbnDay = this.chkKikakuDayTrip.Checked;
		}
		//企画（宿泊）
		if (this.chkKikakuStay.Checked == true)
		{
			paramCrsInfo.CrsKbnStay = this.chkKikakuStay.Checked;
		}
		//企画（Ｒコース）
		if (this.chkRCrs.Checked == true)
		{
			paramCrsInfo.CrsKbnR = this.chkRCrs.Checked;
		}
		//コースコード
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)))
		{
			paramCrsInfo.CrsCd = this.ucoCrsCd.CodeText;
		}

		object dtCrsInfo = dataAccess.selectDataTableCourse(paramCrsInfo);
		dtCrsInfo.Columns.Add(columnList.EIGYOSYO_NAME_1.ToString(), typeof(string));
		object dtReport = createDetailDataTable();
		if (dtCrsInfo.Rows.Count > 0)
		{
			if (dtOfficeName.Rows.Count == 1)
			{
				dtCrsInfo.Rows(0).Item[columnList.EIGYOSYO_NAME_1.ToString()] = dtOfficeName.Rows(0).Item[columnList.EIGYOSYO_NAME_1.ToString()];
			}

			//出力データ作成方法
			//①１件目は、SQL取得結果より、そのまま設定する
			DataRow compareRow = dtCrsInfo.Rows(0);
			dtReport.ImportRow(compareRow);
			object compareCrsCd = compareRow.Item(columnList.CRS_CD.ToString()).ToString();
			object compareGousya = compareRow.Item(columnList.GOUSYA.ToString()).ToString();
			DataRow blankRow = null;
			for (row = 1; row <= dtCrsInfo.Rows.Count - 1; row++)
			{
				//②前レコードと比べ、コースコード、号車が同じ場合、経由地のみ設定する
				if (string.Equals(System.Convert.ToString(compareCrsCd), dtCrsInfo.Rows(row).Item(columnList.CRS_CD.ToString()).ToString()))
				{
					if (string.Equals(System.Convert.ToString(compareGousya), dtCrsInfo.Rows(row).Item(columnList.GOUSYA.ToString()).ToString()))
					{
						blankRow = dtReport.NewRow;
						blankRow.Item[columnList.PLACE_NAME.ToString()] = dtCrsInfo.Rows(row).Item[columnList.PLACE_NAME.ToString()];
						dtReport.Rows.Add(blankRow);
					}
					else
					{
						dtReport.ImportRow(dtCrsInfo.Rows(row));
						compareCrsCd = dtCrsInfo.Rows(row).Item(columnList.CRS_CD.ToString()).ToString();
						compareGousya = dtCrsInfo.Rows(row).Item(columnList.GOUSYA.ToString()).ToString();
					}
				}
				else
				{
					//④全レコードと比べ、コースコードが変わる場合、空行（１行分）入れた後、SQL取得結果からそのまま設定する
					dtReport.Rows.Add(dtReport.NewRow);
					compareRow = dtCrsInfo.Rows(row);
					dtReport.ImportRow(compareRow);
					compareCrsCd = compareRow.Item(columnList.CRS_CD.ToString()).ToString();
					compareGousya = compareRow.Item(columnList.GOUSYA.ToString()).ToString();
				}
			}
		}

		return dtReport;
	}

	private DataTable createDetailDataTable()
	{
		DataTable dt = new DataTable();
		dt.Columns.Add(columnList.SYUPT_DAY.ToString(), typeof(string));
		dt.Columns.Add(columnList.CRS_CD.ToString(), typeof(string));
		dt.Columns.Add(columnList.GOUSYA.ToString(), typeof(string));
		dt.Columns.Add(columnList.CRS_NAME.ToString(), typeof(string));
		dt.Columns.Add(columnList.CAR_NO.ToString(), typeof(string));
		dt.Columns.Add(columnList.DRIVER.ToString(), typeof(string));
		dt.Columns.Add(columnList.GUIDE_NAME.ToString(), typeof(string));
		dt.Columns.Add(columnList.SYUPT_TIME_1.ToString(), typeof(string));
		dt.Columns.Add(columnList.PLACE_NAME.ToString(), typeof(string));
		dt.Columns.Add(columnList.EIGYOSYO_NAME_1.ToString(), typeof(string));
		return dt;
	}

	/// <summary>
	/// レポート印刷する P04_1003
	/// </summary>
	protected override void OvPt99Print()
	{
		//帳票出力
		P04_1003 rpt = new P04_1003();
		rpt.ParamData = base.PtResultDT;
		rpt.Run();
		rpt.Document.Print(true, true);
	}


	#endregion

	#endregion

	#region Privateメソッド(画面独自)

	#endregion

	#region Privateメソッド(イベント)

	#endregion

}