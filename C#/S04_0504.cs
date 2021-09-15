using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.ClientCommon.FlexGridEx;
using Hatobus.ReservationManagementSystem.Master;
using Hatobus.ReservationManagementSystem.Unkou.S04_0504_DA;


public class S04_0504 : PT99, iPT99
{

	#region フィールド
	DataTable dataTable = new DataTable();
	#endregion

	#region 定数
	private const string E90_022 = "E90_022";
	private const string E90_024 = "E90_024";
	private const string E90_024_PARAM = "出力対象";
	private const string COLUMN_MEGER = "申請金額";
	#endregion

	#region 列挙体
	private enum columnList : int
	{
		[Value("選択")]
		SELECT_CHECKBOX,
		[Value("日付")]
		SYUPT_DAY_STR,
		[Value("曜日")]
		YOUBI,
		[Value("コースコード")]
		CRS_CD,
		[Value("コース名")]
		CRS_NAME,
		[Value("号車")]
		GOUSYA,
		[Value("バス会社")]
		BUS_COMPANY,
		[Value("処理")]
		PROCESS_FLG,
		[Value("催行")]
		SAIKOU_NM,
		[Value("基本")]
		SHINSEI_KINGAKU,
		[Value("追加分（代車）")]
		KINGAKU_DAISHA,
		[Value("追加分（他）")]
		KINGAKU_OTHER,
		[Value("同封券枚数")]
		DOFUKEN_SHEET_NUM,
		[Value("受渡額")]
		DELIVERY_KINGAKU,
		[Value("SYUPT_DAY")]
		SYUPT_DAY,
		[Value("PLACE_NAME_1")]
		PLACE_NAME_1,
		[Value("DELIVERY_ICHIMANEN_SHEET_NUM")]
		DELIVERY_ICHIMANEN_SHEET_NUM,
		[Value("DELIVERY_GOSENEN_SHEET_NUM")]
		DELIVERY_GOSENEN_SHEET_NUM,
		[Value("DELIVERY_SENEN_SHEET_NUM")]
		DELIVERY_SENEN_SHEET_NUM
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
		base.PtSearchControl = this.gbxSearch;
		//[検索結果領域]コントロール設定
		base.PtResultControl = this.pnlResult;
		//[検索詳細領域]コントロール設定
		base.PtDetailControl = null;
		//[検索結果Grid]コントロール設定
		base.PtResultGrid = this.grdList;
		//[表示・非表示ボタン]
		base.PtDisplayBtn = btnVisiblerCondition;
		//[件数表示ラベル]コントロール設定
		base.PtResultLblCnt = this.lblCount;

		//[検索結果Max件数]コントロール設定
		base.PtMaxCount = 1000;

		//'親画面に自分を設定
		base.PtMyForm = this;
		//'[変更チェック]カラム設定
		base.PtDiffChkColName = new List[Of string] From { columnList.SELECT_CHECKBOX.ToString()};

		//'[帳票用]帳票タイプ
		base.PtPrintType = PRINTTYPE.AR;
		//'[帳票用]DS用帳票ID
		//MyBase.PtDsPrintId = Nothing

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
			F7Key_Enabled = false;
			F7Key_Visible = true;
		}
		else if (area == area.RESULT)
		{
			//grdの設定処理
			base.CmnInitGrid(grdList);
			// grd初期設定
			InitializeResultGrid();
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
		//F8ボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnSearch.Click += base.btnCom_Click;
		//条件クリアボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnClear.Click += base.btnCom_Click;
		//ベースフォームのF8ボタンプロパティに、検索ボタンを設定
		base.baseBtnF8 = this.btnSearch;
		//初期フォーカスを出発日に設定する
		this.ActiveControl = this.dtmSyuptDayFromTo;

		this.ucoCrsKind.SetInitState();
	}

	#region 検索処理用

	/// <summary>
	/// 検索前チェック処理
	/// </summary>
	/// <returns></returns>
	protected override bool OvPt99SearchBefore()
	{
		//エラーの初期化
		base.clearExistErrorProperty(this.gbxSearch.Controls);
		//チェック要件のチェック
		if (ReferenceEquals(dtmSyuptDayFromTo.FromDateText, null) && ReferenceEquals(dtmSyuptDayFromTo.ToDateText, null))
		{
			this.dtmSyuptDayFromTo.FocusFromDate();
			this.dtmSyuptDayFromTo.ExistErrorForFromDate = true;
			this.dtmSyuptDayFromTo.ExistErrorForToDate = true;
			//共通メッセージ処理 [E90_022 未入力項目があります。]
			CommonProcess.createFactoryMsg().messageDisp(E90_022);
			return false;
		}
		return true;
	}

	/// <summary>
	/// データ取得処理
	/// </summary>
	/// <returns></returns>
	protected override DataTable OvPt99SearchGetData()
	{
		//'DataAccessクラス生成
		S04_0504_DA dataAccess = new S04_0504_DA();
		S04_0504_DASelectParam param = new S04_0504_DASelectParam();
		//出発日FROM
		param.SyuptDayFrom = this.dtmSyuptDayFromTo.FromDateValueInt;

		//出発日TO
		param.SyuptDayTo = this.dtmSyuptDayFromTo.ToDateValueInt;

		// コースコード
		param.CrsCd = ucoCrsCd.CodeText;

		//号車
		if (!string.IsNullOrEmpty(System.Convert.ToString(txtGousya.Text)))
		{
			param.Gousya = int.Parse(System.Convert.ToString(this.txtGousya.Text));
		}

		//日本語
		param.CrsJapanese = ucoCrsKind.JapaneseState;

		//外国語
		param.CrsForeign = ucoCrsKind.ForeignState;

		//定期（昼）
		param.CrsKbnHiru = chkNoon.Checked;

		//定期（夜）
		param.CrsKbnYoru = chkNight.Checked;

		//企画（日帰り）
		param.CrsKbnDay = chkKikakuDayTrip.Checked;

		//企画（宿泊）
		param.CrsKbnStay = chkKikakuStay.Checked;

		//企画（Ｒコース）
		param.CrsKbnR = chkRCrs.Checked;

		//確定済
		param.InputKakuteiZumi = chkKakuteiZumi.Checked;

		//未確定
		param.InputKakuteimi = chkMikakutei.Checked;

		//催行
		param.SaikouKbnY = ChkSaikou.Checked;

		//未定
		param.SaikouKbn = chkMitei.Checked;

		//中止
		param.SaikouKbnN = chkChushi.Checked;

		return dataAccess.selectDataTable(param);
	}

	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void OvPt99SearchAfter()
	{
		if (grdList.Rows.Count > grdList.Rows.Fixed)
		{
			F7Key_Enabled = true;

			for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
			{
				this.grdList.SetCellCheck(row, this.grdList.Cols(columnList.SELECT_CHECKBOX.ToString()).Index, CheckEnum.Checked);
			}
		}
	}
	#endregion

	#region 出力処理用
	/// <summary>
	/// 出力前(エラーチェック)処理
	/// </summary>
	/// <returns></returns>
	protected override bool OvPt99PrintBefore()
	{
		bool checkChecked = false;
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
		{
			if (this.grdList.GetCellCheck(row, this.grdList.Cols(columnList.SELECT_CHECKBOX.ToString()).Index) == CheckEnum.Checked)
			{
				checkChecked = true;
				break;
			}
		}
		if (checkChecked == false)
		{
			CommonProcess.createFactoryMsg().messageDisp(E90_024, E90_024_PARAM);
			return false;
		}
		else
		{
			return true;
		}
	}

	/// <summary>
	/// AR出力時(データ取得)処理
	/// </summary>
	/// <returns></returns>
	protected override DataTable OvPt99PrintARGetData()
	{
		dataTable = ((DataTable)this.grdList.DataSource).Clone();
		for (rowIndex = grdList.Rows.Fixed; rowIndex <= grdList.Rows.Count - 1; rowIndex++)
		{
			if (this.grdList.GetCellCheck(rowIndex, this.grdList.Cols(columnList.SELECT_CHECKBOX.ToString()).Index) == CheckEnum.Checked)
			{
				dataTable.ImportRow(((DataRowView)(this.grdList.Rows(rowIndex).DataSource)).Row);
			}
		}
		return dataTable;
	}

	/// <summary>
	/// レポート印刷する P04_0506
	/// </summary>
	protected override void OvPt99Print()
	{
		if (dataTable.Rows.Count > 0)
		{
			P04_0506 rpt = new P04_0506(dataTable);

			// レポートを実行します。
			rpt.Run();

			//印刷
			rpt.Document.Print(true, true, true);
		}
	}
	#endregion

	#region PT99外ファンクションキー
	#endregion
	#endregion

	#region Privateメソッド(画面独自)
	/// <summary>
	/// Grid列初期設定
	/// </summary>
	private void InitializeResultGrid()
	{
		this.grdList.Initialize(GetGridTitle());
		this.grdList.Cols(columnList.SYUPT_DAY_STR).TextAlign = TextAlignEnum.CenterCenter;
		SetGridHeaderitem();
	}

	private List[] GetGridTitle()
	{
		return new List[Of GridProperty] From {
			;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.SELECT_CHECKBOX),.Name == columnList.SELECT_CHECKBOX.ToString(),.Width == 48,.DataType == typeof(bool),.AllowEditing == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.SYUPT_DAY_STR),.Name == columnList.SYUPT_DAY_STR.ToString(),.Width == 66,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.YOUBI),.Name == columnList.YOUBI.ToString(),.Width == 39,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.CRS_CD),.Name == columnList.CRS_CD.ToString(),.Width == 105,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.CRS_NAME),.Name == columnList.CRS_NAME.ToString(),.Width == 240,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.GOUSYA),.Name == columnList.GOUSYA.ToString(),.Width == 40,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.BUS_COMPANY),.Name == columnList.BUS_COMPANY.ToString(),.Width == 87,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.PROCESS_FLG),.Name == columnList.PROCESS_FLG.ToString(),.Width == 56,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.SAIKOU_NM),.Name == columnList.SAIKOU_NM.ToString(),.Width == 55,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.SHINSEI_KINGAKU),.Name == columnList.SHINSEI_KINGAKU.ToString(),.Width == 95,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.KINGAKU_DAISHA),.Name == columnList.KINGAKU_DAISHA.ToString(),.Width == 108,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.KINGAKU_OTHER),.Name == columnList.KINGAKU_OTHER.ToString(),.Width == 108,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.DOFUKEN_SHEET_NUM),.Name == columnList.DOFUKEN_SHEET_NUM.ToString(),.Width == 108,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.DELIVERY_KINGAKU),.Name == columnList.DELIVERY_KINGAKU.ToString(),.Width == 68,.Visible == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.SYUPT_DAY),.Name == columnList.SYUPT_DAY.ToString(),.Width == -1,.Visible == false},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.PLACE_NAME_1),.Name == columnList.PLACE_NAME_1.ToString(),.Width == -1,.Visible == false},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.DELIVERY_ICHIMANEN_SHEET_NUM),.Name == columnList.DELIVERY_ICHIMANEN_SHEET_NUM.ToString(),.Width == -1,.Visible == false},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.DELIVERY_GOSENEN_SHEET_NUM),.Name == columnList.DELIVERY_GOSENEN_SHEET_NUM.ToString(),.Width == -1,.Visible == false},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.DELIVERY_SENEN_SHEET_NUM),.Name == columnList.DELIVERY_SENEN_SHEET_NUM.ToString(),.Width == -1,.Visible == false};
			//		};
		}
		/// <summary>
		/// 一覧ヘッダー項目設定
		/// </summary>
		private void SetGridHeaderitem()
		{

			//' セルの結合
			//' 固定セルのマージモードを設定
			this.grdList.AllowMerging = AllowMergingEnum.Custom;

			CellRange cr = null;
			//1段目
			cr = grdList.GetCellRange(0, 0, 1, 0);
			grdList.MergedRanges.Add(cr);
			grdList[0, 0] = getEnumAttrValue(columnList.SELECT_CHECKBOX);

			cr = grdList.GetCellRange(0, 1, 1, 1);
			grdList.MergedRanges.Add(cr);
			grdList[0, 1] = getEnumAttrValue(columnList.SYUPT_DAY_STR);

			cr = grdList.GetCellRange(0, 2, 1, 2);
			grdList.MergedRanges.Add(cr);
			grdList[0, 2] = getEnumAttrValue(columnList.YOUBI);

			cr = grdList.GetCellRange(0, 3, 1, 3);
			grdList.MergedRanges.Add(cr);
			grdList[0, 3] = getEnumAttrValue(columnList.CRS_CD);

			cr = grdList.GetCellRange(0, 4, 1, 4);
			grdList.MergedRanges.Add(cr);
			grdList[0, 4] = getEnumAttrValue(columnList.CRS_NAME);

			cr = grdList.GetCellRange(0, 5, 1, 5);
			grdList.MergedRanges.Add(cr);
			grdList[0, 5] = getEnumAttrValue(columnList.GOUSYA);

			cr = grdList.GetCellRange(0, 6, 1, 6);
			grdList.MergedRanges.Add(cr);
			grdList[0, 6] = getEnumAttrValue(columnList.BUS_COMPANY);

			cr = grdList.GetCellRange(0, 7, 1, 7);
			grdList.MergedRanges.Add(cr);
			grdList[0, 7] = getEnumAttrValue(columnList.PROCESS_FLG);

			cr = grdList.GetCellRange(0, 8, 1, 8);
			grdList.MergedRanges.Add(cr);
			grdList[0, 8] = getEnumAttrValue(columnList.SAIKOU_NM);

			//MERGED CELL
			cr = grdList.GetCellRange(0, 9, 0, 11);
			grdList.MergedRanges.Add(cr);
			grdList[0, 9] = COLUMN_MEGER;

			grdList[1, 9] = getEnumAttrValue(columnList.SHINSEI_KINGAKU);
			grdList[1, 10] = getEnumAttrValue(columnList.KINGAKU_DAISHA);
			grdList[1, 11] = getEnumAttrValue(columnList.KINGAKU_OTHER);

			cr = grdList.GetCellRange(0, 12, 1, 12);
			grdList.MergedRanges.Add(cr);
			grdList[0, 12] = getEnumAttrValue(columnList.DOFUKEN_SHEET_NUM);

			cr = grdList.GetCellRange(0, 13, 1, 13);
			grdList.MergedRanges.Add(cr);
			grdList[0, 13] = getEnumAttrValue(columnList.DELIVERY_KINGAKU);
		}

		#endregion

		#region Privateメソッド(イベント)

		#endregion

	}