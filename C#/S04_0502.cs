using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.ClientCommon.FlexGridEx;
using Hatobus.ReservationManagementSystem.Master;


public class S04_0502 : PT99, iPT99
{
	#region 定数

	//変更されているものデータだけをDataTable
	private DataTable selectedData = new DataTable();
	private const string E04_006 = "E04_006";
	private const string E04_006Param1 = "申請金額";
	private const string E04_006Param2 = "5,000";
	private const string E90_022 = "E90_022";
	private const string DAISHA = "代";
	private const int GOSEN = 5000;
	//未確定
	private const string CONTROL_FLAG_Y = "Y";
	//確定済
	private const string CONTROL_FLAG_N = "N";

	#endregion

	#region 列挙体

	private enum columnList : int
	{
		[Value("選択")]
		SELECT_CHECKBOX,
		[Value("出発日")]
		SYUPT_DAY,
		[Value("日付")]
		SYUPT_DAY_STR,
		[Value("曜日")]
		YOBI,
		[Value("コース" + "\r\n" + "コード")]
		CRS_CD,
		[Value("コース名")]
		CRS_NAME,
		[Value("号車")]
		GOUSYA,
		[Value("バス" + "\r\n" + "会社")]
		BUS_COMPANY,
		[Value("処理")]
		PROCESS_FLG,
		[Value("催行")]
		SAIKOU_KBN,
		[Value("基本")]
		SHINSEI_KINGAKU,
		[Value("追加分（代車）")]
		ADD_DAISHA,
		[Value("追加分（他）")]
		ADD_SONOTA,
		[Value("同封券枚数")]
		DOFUKEN_SHEET_NUM,
		[Value("受渡額")]
		DELIVERY_KINGAKU,
		[Value("備考")]
		BIKO,
		[Value("季コード")]
		SEASON_CD,
		[Value("年")]
		YEAR,
		[Value("申請区分")]
		SHINSEI_KBN,
		[Value("申請時催行区分")]
		SHINSEI_JI_SAIKOU_KBN,
		[Value("申請時運行バス")]
		SHINSEI_JI_UNKOU_BUS,
		[Value("出発年月")]
		SYUPT_YM,
		[Value("グリッド制御フラグ")]
		GRID_CONTROL_FLG
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
		//ボタン用設定】
		//CSV出力可否
		base.PtIsCsvOutFlg = false;
		//ﾌﾟﾚﾋﾞｭｰ可否
		base.PtIsPrevFlg = false;
		//印刷/出力可否
		base.PtIsPrintFlg = false;
		//検索可否
		base.PtIsSearchFlg = false;
		//登録可否
		base.PtIsRegFlg = false;
		//更新可否
		base.PtIsUpdFlg = true;

		//【コントロール系】
		//検索エリアコンテナ
		base.PtSearchControl = this.gbxSearch;
		//検索結果エリアコンテナ
		base.PtResultControl = this.pnlResult;
		//詳細エリアコンテナ
		base.PtDetailControl = null;
		//検索結果グリッド
		base.PtResultGrid = this.grdList;
		//表示/非表示ボタン
		base.PtDisplayBtn = btnVisiblerCondition;
		//件数表示ラベル 1
		base.PtResultLblCnt = this.lblCount;

		//【データ系】
		//最大表示件数
		base.PtMaxCount = 1000;
		//検索結果(ReadOnly)
		//PtResultDT =
		//選択行データ(ReadOnly)
		//PtSelectRow =

		//【その他】
		//実装画面
		base.PtMyForm = this;
		//変更チェックカラム
		base.PtDiffChkColName = new List[Of string] From { columnList.SHINSEI_KINGAKU.ToString(), columnList.ADD_DAISHA.ToString(), columnList.ADD_SONOTA.ToString(), columnList.DOFUKEN_SHEET_NUM.ToString(), columnList.BIKO.ToString()};

		//【帳票用】
		//[帳票用]帳票タイプ
		//MyBase.PtPrintType =
		//[帳票用]DS用帳票ID
		// MyBase.PtDsPrintId =
		//[変更チェック]カラム設定
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
		else if (area == area.SEARCH)
		{
			this.ucoCrsKind.SetInitState();
		}
		else if (area == area.RESULT)
		{
			//grdの設定処理
			base.CmnInitGrid(grdList);
			//grd初期設定
			InitializeResultGrid();
			this.grdList.clearErrorCell();
		}
		else if (area == area.DETAIL)
		{
			//なし
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

	#region 変更確認
	/// <summary>
	/// 差分チェック
	/// </summary>
	protected override bool checkDifference()
	{
		bool noDifferent = System.Convert.ToBoolean(base.OvPt99CheckDifference());
		if (noDifferent == false)
		{
			selectedData = base.PtResultDT.Clone;
			selectedData.Clear();
			//選択にチェックされていて基本・追加分・同封券枚数のいずれかが変更されているものデータだけをDataTableにセットする
			for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
			{
				if (this.grdList.getCheckBoxValue(row, this.grdList.Cols(columnList.SELECT_CHECKBOX.ToString()).Index))
				{
					object checkedDataRow = ((DataRowView)(this.grdList.Rows(row).DataSource)).Row;
					if (!checkedDataRow.RowState.Equals(DataRowState.Unchanged))
					{
						foreach (string str in base.PtDiffChkColName)
						{
							if (!checkedDataRow.Item(str, DataRowVersion.Original).ToString().Equals(checkedDataRow.Item(str, DataRowVersion.Current).ToString()))
							{
								selectedData.ImportRow(checkedDataRow);
								break;
							}
						}
					}
				}
			}
			if (selectedData.Rows.Count == 0)
			{
				//0件の場合はTrueを返却し
				noDifferent = true;
			}
		}
		return noDifferent;
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
		//独自の初期処理
		//F8ボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnSearch.Click += base.btnCom_Click;
		//条件クリアボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnClear.Click += base.btnCom_Click;
		//ベースフォームのF8ボタンプロパティに、検索ボタンを設定
		base.baseBtnF8 = this.btnSearch;
		//初期フォーカスを出発日に設定する
		this.ActiveControl = this.dtmSyuptDayFromTo;
	}

	#region 検索処理用
	//' <summary>
	//' 検索前チェック処理
	//' </summary>
	//' <returns></returns>
	protected override bool OvPt99SearchBefore()
	{
		//エラーの初期化
		base.clearExistErrorProperty(this.gbxSearch.Controls);
		//チェック要件のチェック
		if (CommonUtil.checkHissuError(this.gbxSearch.Controls) == true)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
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
		S04_0502_DA.S04_0502ParamData param = new S04_0502_DA.S04_0502ParamData();
		S04_0502_DA dataAccess = new S04_0502_DA();
		// 出発日FROM
		param.SyuptdayFrom = this.dtmSyuptDayFromTo.FromDateValueInt.Value;
		//出発日TO
		param.SyuptdayTo = this.dtmSyuptDayFromTo.ToDateValueInt.Value;
		// コースコード
		if (!string.IsNullOrEmpty(System.Convert.ToString(this.ucoCrsCd.CodeText)))
		{
			param.CrsCd = this.ucoCrsCd.CodeText;
		}
		//号車
		if (!string.IsNullOrEmpty(System.Convert.ToString(txtGousya.Text)))
		{
			param.Gousya = System.Convert.ToInt32(this.txtGousya.Text);
		}
		//定期（昼）
		if (this.chkNoon.Checked == true)
		{
			param.TeikiHiru = this.chkNoon.Checked;
		}
		// 定期（夜）
		if (this.chkNight.Checked == true)
		{
			param.TeikiYoru = this.chkNight.Checked;
		}
		// 企画（日帰り）
		if (this.chkKikakuDayTrip.Checked == true)
		{
			param.KikakuHigaeri = this.chkKikakuDayTrip.Checked;
		}
		//企画（宿泊）
		if (this.chkKikakuStay.Checked == true)
		{
			param.KikakuStay = this.chkKikakuStay.Checked;
		}
		//  企画（Ｒコース）
		if (this.chkRCrs.Checked == true)
		{
			param.KikakuR = this.chkRCrs.Checked;
		}
		//日本語
		if (this.ucoCrsKind.JapaneseState == true)
		{
			param.CrsJapanese = this.ucoCrsKind.JapaneseState;
		}
		// 外国語
		if (this.ucoCrsKind.ForeignState == true)
		{
			param.CrsForeign = this.ucoCrsKind.ForeignState;
		}
		//確定済
		if (this.chkKakuteiZumi.Checked == true)
		{
			param.Kakuteizumi = this.chkKakuteiZumi.Checked;
		}
		//未確定
		if (this.chkMikakutei.Checked == true)
		{
			param.Mikakutei = this.chkMikakutei.Checked;
		}
		//催行
		if (this.ChkSaikou.Checked == true)
		{
			param.Saikou = this.ChkSaikou.Checked;
		}
		// 未定
		if (this.chkMitei.Checked == true)
		{
			param.Mitei = this.chkMitei.Checked;
		}
		//中止
		if (this.chkChushi.Checked == true)
		{
			param.Chuushi = this.chkChushi.Checked;
		}

		return dataAccess.selectDataTable(param);
	}

	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void OvPt99SearchAfter()
	{
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
		{
			//未確定の場合、申請金額（追加分（代車）、追加分（他））を編集不可とする
			if (this.chkMikakutei.Checked == true)
			{
				this.grdList.setColorNonEdit(row, this.grdList.Cols(columnList.ADD_DAISHA.ToString()).Index, Color.Gray);
				this.grdList.setColorNonEdit(row, this.grdList.Cols(columnList.ADD_SONOTA.ToString()).Index, Color.Gray);
			}
			else if (this.chkKakuteiZumi.Checked == true)
			{
				//確定済の場合、申請金額（基本）を編集不可とする
				this.grdList.setColorNonEdit(row, this.grdList.Cols(columnList.SHINSEI_KINGAKU.ToString()).Index, Color.Gray);
				//・バス会社が’代車’以外の場合、追加分（代車）を編集不可とする
				object busCompany = this.grdList.GetData(row, columnList.BUS_COMPANY.ToString()).ToString();
				if (!string.Equals(DAISHA, busCompany))
				{
					this.grdList.setColorNonEdit(row, this.grdList.Cols(columnList.ADD_DAISHA.ToString()).Index, Color.Gray);
				}
			}

		}
	}
	#endregion

	#region 更新処理用

	/// <summary>
	/// 更新前チェック処理
	/// </summary>
	/// <returns></returns>
	protected override bool checkUpdateItems()
	{
		this.grdList.clearErrorCell();
		//チェック要件、No.2を行う
		//【「F11：更新」ボタン押下時】
		//・申請金額_基本、申請金額_追加分（代車）、申請金額_追加分（他）のいずれかに入力があり、
		//入力された内容が5, 0円の倍数でない場合、5, 0円単位で入力する規則がある旨のメッセージを表示する
		if (selectedData.Rows.Count > 0)
		{
			//申請金額_基本
			int SHINSEI_KINGAKU = 0;
			//申請金額_追加分（代車）
			int ADD_DAISHA = 0;
			//申請金額_追加分（他）
			int ADD_SONOTA = 0;
			int remainder = 0;
			//出発日
			System.String syuptDay = string.Empty;
			//コースコード
			System.String crsCd = string.Empty;
			//号車
			System.String gousya = string.Empty;
			for (int row = 0; row <= selectedData.Rows.Count - 1; row++)
			{
				SHINSEI_KINGAKU = 0;
				ADD_DAISHA = 0;
				ADD_SONOTA = 0;
				remainder = 0;
				syuptDay = selectedData.Rows(row).Item(columnList.SYUPT_DAY.ToString()).ToString();
				crsCd = selectedData.Rows(row).Item(columnList.CRS_CD.ToString()).ToString();
				gousya = selectedData.Rows(row).Item(columnList.GOUSYA.ToString()).ToString();
				if (!string.IsNullOrEmpty(selectedData.Rows(row).Item(columnList.SHINSEI_KINGAKU.ToString()).ToString()))
				{
					int.TryParse(selectedData.Rows(row).Item(columnList.SHINSEI_KINGAKU.ToString()).ToString(), out SHINSEI_KINGAKU);
					if (SHINSEI_KINGAKU > 0)
					{
						remainder = SHINSEI_KINGAKU % GOSEN;
						if (remainder > 0)
						{
							CommonProcess.createFactoryMsg().messageDisp(E04_006, E04_006Param1, E04_006Param2);
							for (int r = this.grdList.Rows.Fixed; r <= this.grdList.Rows.Count - 1; r++)
							{
								if (string.Equals(syuptDay, this.grdList.GetData(r, columnList.SYUPT_DAY.ToString()).ToString())
									&& string.Equals(crsCd, this.grdList.GetData(r, columnList.CRS_CD.ToString()).ToString())
									&& string.Equals(gousya, this.grdList.GetData(r, columnList.GOUSYA.ToString()).ToString()))
								{
									this.grdList.setColorError(r, this.grdList.Cols(columnList.SHINSEI_KINGAKU.ToString()).Index);
									break;
								}
							}
							return false;
						}
					}
				}
				if (!string.IsNullOrEmpty(selectedData.Rows(row).Item(columnList.ADD_DAISHA.ToString()).ToString()))
				{
					int.TryParse(selectedData.Rows(row).Item(columnList.ADD_DAISHA.ToString()).ToString(), out ADD_DAISHA);
					if (ADD_DAISHA > 0)
					{
						remainder = ADD_DAISHA % GOSEN;
						if (remainder > 0)
						{
							CommonProcess.createFactoryMsg().messageDisp(E04_006, E04_006Param1, E04_006Param2);
							for (int r = this.grdList.Rows.Fixed; r <= this.grdList.Rows.Count - 1; r++)
							{
								if (string.Equals(syuptDay, this.grdList.GetData(r, columnList.SYUPT_DAY.ToString()).ToString())
									&& string.Equals(crsCd, this.grdList.GetData(r, columnList.CRS_CD.ToString()).ToString())
									&& string.Equals(gousya, this.grdList.GetData(r, columnList.GOUSYA.ToString()).ToString()))
								{
									this.grdList.setColorError(r, this.grdList.Cols(columnList.ADD_DAISHA.ToString()).Index);
									break;
								}
							}
							return false;
						}
					}
				}
				if (!string.IsNullOrEmpty(selectedData.Rows(row).Item(columnList.ADD_SONOTA.ToString()).ToString()))
				{
					int.TryParse(selectedData.Rows(row).Item(columnList.ADD_SONOTA.ToString()).ToString(), out ADD_SONOTA);
					if (ADD_SONOTA > 0)
					{
						remainder = ADD_SONOTA % GOSEN;
						if (remainder > 0)
						{
							CommonProcess.createFactoryMsg().messageDisp(E04_006, E04_006Param1, E04_006Param2);
							for (int r = this.grdList.Rows.Fixed; r <= this.grdList.Rows.Count - 1; r++)
							{
								if (string.Equals(syuptDay, this.grdList.GetData(r, columnList.SYUPT_DAY.ToString()).ToString())
									&& string.Equals(crsCd, this.grdList.GetData(r, columnList.CRS_CD.ToString()).ToString())
									&& string.Equals(gousya, this.grdList.GetData(r, columnList.GOUSYA.ToString()).ToString()))
								{
									this.grdList.setColorError(r, this.grdList.Cols(columnList.ADD_SONOTA.ToString()).Index);
									break;
								}
							}
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	/// <summary>
	/// 更新処理
	/// </summary>
	/// <returns></returns>
	protected override int updateTranData()
	{
		S04_0502_DA dataAccess = new S04_0502_DA();
		List listParam = new List(Of S04_0502_DA.S04_0502DAUpdateParam);
		S04_0502_DA.S04_0502DAUpdateParam param = null;
		for (int row = 0; row <= selectedData.Rows.Count - 1; row++)
		{
			//パラメータはIF(OUT)シートNo.2参照
			param = new S04_0502_DA.S04_0502DAUpdateParam();
			//出発日
			object syuptDay = selectedData.Rows(row).Item(columnList.SYUPT_DAY.ToString());
			if (!Information.IsDBNull(syuptDay) && !string.IsNullOrEmpty(syuptDay.ToString()))
			{
				param.Syuptday = System.Convert.ToInt32(syuptDay);
			}
			//コースコード
			object crsCd = selectedData.Rows(row).Item(columnList.CRS_CD.ToString());
			if (!Information.IsDBNull(crsCd))
			{
				param.CrsCd = crsCd.ToString();
			}
			//号車
			object gousya = selectedData.Rows(row).Item(columnList.GOUSYA.ToString());
			if (!Information.IsDBNull(gousya) && !string.IsNullOrEmpty(gousya.ToString()))
			{
				param.Gousya = System.Convert.ToInt32(gousya);
			}

			//申請金額
			int applicationAmount = 0;
			object shinseiKingaku = selectedData.Rows(row).Item(columnList.SHINSEI_KINGAKU.ToString());
			if (!Information.IsDBNull(shinseiKingaku) && !string.IsNullOrEmpty(shinseiKingaku.ToString()))
			{
				param.Kingaku = System.Convert.ToInt32(shinseiKingaku);
				applicationAmount += System.Convert.ToInt32(shinseiKingaku);
			}

			//追加代車
			object addDaisha = selectedData.Rows(row).Item(columnList.ADD_DAISHA.ToString());
			if (!Information.IsDBNull(addDaisha) && !string.IsNullOrEmpty(addDaisha.ToString()))
			{
				param.AddDaisha = System.Convert.ToInt32(addDaisha);
				applicationAmount += System.Convert.ToInt32(addDaisha);
			}

			//追加その他
			object addSonota = selectedData.Rows(row).Item(columnList.ADD_SONOTA.ToString());
			if (!Information.IsDBNull(addSonota) && !string.IsNullOrEmpty(addSonota.ToString()))
			{
				param.AddSonota = System.Convert.ToInt32(addSonota);
				applicationAmount += System.Convert.ToInt32(addSonota);
			}

			//備考
			object biko = selectedData.Rows(row).Item(columnList.BIKO.ToString());
			if (!Information.IsDBNull(biko))
			{
				param.Biko = biko.ToString();
			}
			//季コード
			object seasonCd = selectedData.Rows(row).Item(columnList.SEASON_CD.ToString());
			if (!Information.IsDBNull(seasonCd))
			{
				param.SeasonCd = seasonCd.ToString();
			}
			//同封券枚数
			object dofukenSheetNum = selectedData.Rows(row).Item(columnList.DOFUKEN_SHEET_NUM.ToString());
			if (!Information.IsDBNull(dofukenSheetNum) && !string.IsNullOrEmpty(dofukenSheetNum.ToString()))
			{
				param.DofukenSheetNum = System.Convert.ToInt32(dofukenSheetNum);
			}

			calcBills(selectedData.Rows(row), param);
			//申請時催行区分
			object shinseiJiSaikouKbn = selectedData.Rows(row).Item(columnList.SHINSEI_JI_SAIKOU_KBN.ToString());
			if (!Information.IsDBNull(shinseiJiSaikouKbn))
			{
				param.ShinseiJiSaikouKbn = shinseiJiSaikouKbn.ToString();
			}
			//申請時運行バス
			object shinseiJiUnkouBus = selectedData.Rows(row).Item(columnList.SHINSEI_JI_UNKOU_BUS.ToString());
			if (!Information.IsDBNull(shinseiJiUnkouBus))
			{
				param.ShinseiJiUnkouBus = shinseiJiUnkouBus.ToString();
			}
			//申請区分
			object shinseiKbn = selectedData.Rows(row).Item(columnList.SHINSEI_KBN.ToString());
			if (!Information.IsDBNull(shinseiKbn))
			{
				param.ShinseiKbn = shinseiKbn.ToString();
			}
			//グリッド制御フラグ
			object gridControlFlg = selectedData.Rows(row).Item(columnList.GRID_CONTROL_FLG.ToString());
			if (!Information.IsDBNull(gridControlFlg))
			{
				param.GridControlFlg = gridControlFlg.ToString();
			}
			else
			{
				gridControlFlg = string.Empty;
			}
			//金額
			//選択チェックボックスにチェックされた行のグリッド制御フラグが
			//N'（未確定）の場合、申請金額
			//Y'（確定済）の場合、受渡金額
			object deliveryKingaku = selectedData.Rows(row).Item(columnList.DELIVERY_KINGAKU.ToString());
			if (!Information.IsDBNull(deliveryKingaku) && !string.IsNullOrEmpty(deliveryKingaku.ToString()))
			{
				if (string.Equals(System.Convert.ToString(gridControlFlg), CONTROL_FLAG_N))
				{
					//N'（未確定）の場合、申請金額
					param.Kingaku = applicationAmount;
				}
				else if (string.Equals(System.Convert.ToString(gridControlFlg), CONTROL_FLAG_Y))
				{
					//Y'（確定済）の場合、受渡金額
					param.Kingaku = System.Convert.ToInt32(deliveryKingaku);
				}
			}
			//出発年月
			object syuptYm = selectedData.Rows(row).Item(columnList.SYUPT_DAY.ToString());
			if (!Information.IsDBNull(syuptYm) && !string.IsNullOrEmpty(syuptYm.ToString()))
			{
				param.SyuptYm = int.Parse(syuptYm.ToString().Substring(0, 6));
			}
			//年
			object year = selectedData.Rows(row).Item(columnList.YEAR.ToString());
			if (!Information.IsDBNull(year) && !string.IsNullOrEmpty(year.ToString()))
			{
				param.Year = int.Parse(year.ToString());
			}
			object systemTime = CommonDateUtil.getSystemTime();
			// システム登録ＰＧＭＩＤ
			param.SystemEntryPgmid = this.Name;
			//システム登録者コード
			param.SystemEntryPersonCd = UserInfoManagement.userId;
			//システム登録日
			param.SystemEntryDay = systemTime;
			//システム更新ＰＧＭＩＤ
			param.SystemUpdatePgmid = this.Name;
			//システム更新者コード
			param.SystemUpdatePersonCd = UserInfoManagement.userId;
			//システム更新日
			param.SystemUpdateDay = systemTime;
			listParam.Add(param);
		}
		//DA定義の更新処理を呼び出す
		return dataAccess.executeUpdate(listParam);
	}
	#endregion

	#endregion

	#region Privateメソッド(画面独自)
	/// <summary>
	/// Grid列初期設定
	/// </summary>
	private void InitializeResultGrid()
	{
		this.grdList.Initialize(GetGridTitle());
		CreateHeaderRow();
	}

	private List[] GetGridTitle()
	{
		List colsList = new List[Of GridProperty] From {
			;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SELECT_CHECKBOX),.Name == columnList.SELECT_CHECKBOX.ToString(),.Width == 71,.DataType == typeof(bool),.AllowEditing == true },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SYUPT_DAY_STR),.Name == columnList.SYUPT_DAY_STR.ToString(),.Width == 100 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.YOBI),.Name == columnList.YOBI.ToString(),.Width == 39 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CRS_CD),.Name == columnList.CRS_CD.ToString(),.Width == 55 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CRS_NAME),.Name == columnList.CRS_NAME.ToString(),.Width == 290 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.GOUSYA),.Name == columnList.GOUSYA.ToString(),.Width == 40 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.BUS_COMPANY),.Name == columnList.BUS_COMPANY.ToString(),.Width == 87 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.PROCESS_FLG),.Name == columnList.PROCESS_FLG.ToString(),.Width == 56 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SAIKOU_KBN),.Name == columnList.SAIKOU_KBN.ToString(),.Width == 55 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SHINSEI_KINGAKU),.Name == columnList.SHINSEI_KINGAKU.ToString(),.Width == 95,.DataType == typeof(int),.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(9, ControlFormat.HankakuSuji) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.ADD_DAISHA),.Name == columnList.ADD_DAISHA.ToString(),.Width == 103,.DataType == typeof(int),.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(9, ControlFormat.HankakuSuji) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.ADD_SONOTA),.Name == columnList.ADD_SONOTA.ToString(),.Width == 95,.DataType == typeof(int),.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(9, ControlFormat.HankakuSuji) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.DOFUKEN_SHEET_NUM),.Name == columnList.DOFUKEN_SHEET_NUM.ToString(),.Width == 102,.DataType == typeof(int),.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(3, ControlFormat.HankakuSuji) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.DELIVERY_KINGAKU),.Name == columnList.DELIVERY_KINGAKU.ToString(),.Width == 102 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.BIKO),.Name == columnList.BIKO.ToString(),.Width == 102,.AllowEditing == true },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SEASON_CD),.Name == columnList.SEASON_CD.ToString(),.Width == 102,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.YEAR),.Name == columnList.YEAR.ToString(),.Width == 50,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SYUPT_DAY),.Name == columnList.SYUPT_DAY.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SHINSEI_KBN),.Name == columnList.SHINSEI_KBN.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SHINSEI_JI_SAIKOU_KBN),.Name == columnList.SHINSEI_JI_SAIKOU_KBN.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SHINSEI_JI_UNKOU_BUS),.Name == columnList.SHINSEI_JI_UNKOU_BUS.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.GRID_CONTROL_FLG),.Name == columnList.GRID_CONTROL_FLG.ToString(),.Width == 102,.Visible == false };
		};
		this.grdList.Cols.Count = colsList.Count;
		this.grdList.Cols.Fixed = 1;
		return colsList;
	}

	/// <summary>
	/// 一覧ヘッダー項目設定
	/// </summary>
	private void CreateHeaderRow()
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
		grdList[0, 2] = getEnumAttrValue(columnList.YOBI);

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
		grdList[0, 8] = getEnumAttrValue(columnList.SAIKOU_KBN);

		//MERGED CELL
		cr = grdList.GetCellRange(0, 9, 0, 11);
		grdList.MergedRanges.Add(cr);
		grdList[0, 9] = "申請金額";

		grdList[1, 9] = getEnumAttrValue(columnList.SHINSEI_KINGAKU);
		grdList[1, 10] = getEnumAttrValue(columnList.ADD_DAISHA);
		grdList[1, 11] = getEnumAttrValue(columnList.ADD_SONOTA);

		cr = grdList.GetCellRange(0, 12, 1, 12);
		grdList.MergedRanges.Add(cr);
		grdList[0, 12] = getEnumAttrValue(columnList.DOFUKEN_SHEET_NUM);

		cr = grdList.GetCellRange(0, 13, 1, 13);
		grdList.MergedRanges.Add(cr);
		grdList[0, 13] = getEnumAttrValue(columnList.DELIVERY_KINGAKU);

		cr = grdList.GetCellRange(0, 14, 1, 14);
		grdList.MergedRanges.Add(cr);
		grdList[0, 14] = getEnumAttrValue(columnList.BIKO);

		cr = grdList.GetCellRange(0, 15, 1, 15);
		grdList.MergedRanges.Add(cr);
		grdList[0, 15] = getEnumAttrValue(columnList.SEASON_CD);

		cr = grdList.GetCellRange(0, 16, 1, 16);
		grdList.MergedRanges.Add(cr);
		grdList[0, 16] = getEnumAttrValue(columnList.GRID_CONTROL_FLG);

		cr = grdList.GetCellRange(0, 17, 1, 17);
		grdList.MergedRanges.Add(cr);
		grdList[0, 17] = getEnumAttrValue(columnList.YEAR);

		cr = grdList.GetCellRange(0, 18, 1, 18);
		grdList.MergedRanges.Add(cr);
		grdList[0, 18] = getEnumAttrValue(columnList.SYUPT_DAY);

		cr = grdList.GetCellRange(0, 19, 1, 19);
		grdList.MergedRanges.Add(cr);
		grdList[0, 19] = getEnumAttrValue(columnList.SHINSEI_JI_SAIKOU_KBN);

		cr = grdList.GetCellRange(0, 20, 1, 20);
		grdList.MergedRanges.Add(cr);
		grdList[0, 20] = getEnumAttrValue(columnList.SHINSEI_JI_UNKOU_BUS);

		cr = grdList.GetCellRange(0, 21, 1, 21);
		grdList.MergedRanges.Add(cr);
		grdList[0, 21] = getEnumAttrValue(columnList.SHINSEI_KBN);

	}

	/// <summary>
	/// 受渡額に申請金額_基本、申請金額_追加分（代車）、申請金額_追加分（他）の合計を受渡額に設定する
	/// </summary>
	/// <param name="e"></param>
	private void calcUkewatashigaku(RowColEventArgs e)
	{
		int SHINSEI_KINGAKU = 0;
		int ADD_DAISHA = 0;
		int ADD_SONOTA = 0;
		int DOFUKEN_SHEET_NUM = 0;
		//受渡額に申請金額_基本、申請金額_追加分（代車）、申請金額_追加分（他）の合計を受渡額に設定する
		int.TryParse(this.grdList.GetData(e.Row, columnList.SHINSEI_KINGAKU.ToString()).ToString(), out SHINSEI_KINGAKU);
		this.grdList.SetData(e.Row, columnList.SHINSEI_KINGAKU.ToString(), SHINSEI_KINGAKU);
		//申請金額_追加分（代車）
		int.TryParse(this.grdList.GetData(e.Row, columnList.ADD_DAISHA.ToString()).ToString(), out ADD_DAISHA);
		this.grdList.SetData(e.Row, columnList.ADD_DAISHA.ToString(), ADD_DAISHA);
		//申請金額_追加分（他）
		int.TryParse(this.grdList.GetData(e.Row, columnList.ADD_SONOTA.ToString()).ToString(), out ADD_SONOTA);
		this.grdList.SetData(e.Row, columnList.ADD_SONOTA.ToString(), ADD_SONOTA);
		//受渡額
		System.Int32 DELIVERY_KINGAKU = SHINSEI_KINGAKU + ADD_DAISHA + ADD_SONOTA;
		this.grdList.SetData(e.Row, columnList.DELIVERY_KINGAKU.ToString(), DELIVERY_KINGAKU);
		//同封券枚数
		int.TryParse(this.grdList.GetData(e.Row, columnList.DOFUKEN_SHEET_NUM.ToString()).ToString(), out DOFUKEN_SHEET_NUM);
		this.grdList.SetData(e.Row, columnList.DOFUKEN_SHEET_NUM.ToString(), DOFUKEN_SHEET_NUM);
	}

	private void grdList_AfterEdit(object sender, RowColEventArgs e)
	{
		if (e.Row >= this.grdList.Rows().Fixed)
		{
			if (e.Col == this.grdList.Cols(columnList.SHINSEI_KINGAKU.ToString()).Index
				|| e.Col == this.grdList.Cols(columnList.ADD_DAISHA.ToString()).Index
				|| e.Col == this.grdList.Cols(columnList.ADD_SONOTA.ToString()).Index
				|| e.Col == this.grdList.Cols(columnList.DOFUKEN_SHEET_NUM.ToString()).Index)
			{
				calcUkewatashigaku(e);
			}
		}
	}

	private void calcBills(DataRow row, S04_0502_DA.S04_0502DAUpdateParam param)
	{
		object deliveryKingaku = row.Item(columnList.DELIVERY_KINGAKU.ToString());
		int _Sum = 0;
		if (!Information.IsDBNull(deliveryKingaku) && !string.IsNullOrEmpty(deliveryKingaku.ToString()))
		{
			_Sum = int.Parse(deliveryKingaku.ToString());
		}
		if (_Sum % 5000 == 0)
		{
			int _Business = 0;
			int _remainder = 0;
			//商 = 金額 \ 10000
			_Business = System.Convert.ToInt32(Math.Floor((double)_Sum / 10000));
			//余り = 金額 Mod 10000
			_remainder = _Sum % 10000;
			//余りが0の場合
			if (_remainder == 0)
			{
				//壱万円札枚数 = 商 - 1
				param.IchimanenSheetNum = _Business - 1;
				//五千円札枚数 = 1
				param.GosenenSheetNum = 1;
				//壱千円札枚数 = 5
				param.SenenSheetNum = 5;
			}
			else
			{
				//余りが0でない場合
				//壱万円札枚数 = 商
				param.IchimanenSheetNum = _Business;
				//五千円札枚数 = 0
				param.GosenenSheetNum = 0;
				//壱千円札枚数 = 5
				param.SenenSheetNum = 5;
			}
		}
	}

	#endregion

	#region Privateメソッド(イベント)

	#endregion

}