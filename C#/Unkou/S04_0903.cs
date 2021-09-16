using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.ClientCommon.FlexGridEx;
using Hatobus.ReservationManagementSystem.Master;
using Hatobus.ReservationManagementSystem.Tehai;
using Hatobus.ReservationManagementSystem.Unkou.S04_0903_DA;


/// <summary>
/// 最終確認連絡
/// </summary>
public class S04_0903 : PT99, iPT99
{

	#region フィールド
	S03_0408ParamData param = new S03_0408ParamData();
	S03_0408ParamSubData S03_0408Pram = new S03_0408ParamSubData();

	#endregion

	#region 定数
	private enum columnList : int
	{
		[Value("選択")]
		OUTPUT_FLG,
		[Value("出発日")]
		SYUPT_DAY_STR,
		[Value("出発日")]
		SYUPT_DAY,
		[Value("コースコード")]
		CRS_CD,
		[Value("コース名")]
		CRS_NAME,
		[Value("出発時間")]
		SYUPT_TIME_STR,
		[Value("号車")]
		GOUSYA,
		[Value("乗り場")]
		NORIBA,
		[Value("予約件数")]
		YOYAKU_NUM,
		[Value("最終確認連絡日")]
		LAST_CONTACT_DAY
	}
	private const string F7_TXT_LBL = "F7:最終確認連絡履歴";
	private const string F10_TXT_LBL = "F10:連絡先設定";
	private const string E90_022 = "E90_022";
	private const string E90_024 = "E90_024";
	private const string E90_024_PARAM = "通知対象";
	private const string E90_073 = "E90_073";
	private const string E90_073_PARAM1 = "最終確認連絡履歴の参照";
	private const string E90_073_PARAM2 = "出発日、コースコードが指定されている場合";
	private const string Notice = "1";
	private const string NoticeType = "3";
	#endregion

	#region 画面に実装(Interface)
	/// <summary>
	///
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
		base.PtIsPrintFlg = false;
		//[検索](F8)有無設定
		base.PtIsSearchFlg = false;
		//[登録](F10)有無設定
		base.PtIsRegFlg = true;
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
		base.PtDiffChkColName = null;

		//'[帳票用]帳票タイプ
		base.PtPrintType = null;
		//'[帳票用]DS用帳票ID
		base.PtDsPrintId = null;
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
		//フォームの初期設定
		if (area == area.FORM)
		{
			F10Key_Visible = true;
			F10Key_Enabled = false;
			F10Key_Text = F10_TXT_LBL;

			//検索条件エリアの初期設定
		}
		else if (area == area.SEARCH)
		{
			CommonUtil.Control_Init(this.gbxSearch.Controls);
			//出発日Fromには初期値として、システム日付の翌日を設定
			this.ucoSyuptDayFromTo.FromDateText = CommonDateUtil.getSystemTime().AddDays(1);
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
		//  TRUEを戻す
		return true;
	}

	#endregion

	#region 画面に実装(Overrides)
	protected override void OvPt99Init()
	{
		//初期化処理(PT99)
		base.OvPt99Init();

		//独自の初期処理
		//F7ボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnRireki.Click += base.btnCom_Click;

		//F8ボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnSearch.Click += base.btnCom_Click;

		//条件クリアボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnClear.Click += base.btnCom_Click;
		//ベースフォームのF8ボタンプロパティに、検索ボタンを設定
		base.baseBtnF8 = this.btnSearch;

		//ベースフォームのF7ボタンプロパティに、検索ボタンを設定
		base.baseBtnF7 = this.btnRireki;

		//初期フォーカスを出発日に設定する
		this.ActiveControl = this.ucoSyuptDayFromTo;
	}

	protected override void btnF7_ClickOrgProc()
	{
		//エラーの初期化s
		base.clearExistErrorProperty(this.gbxSearch.Controls);
		//ック要件のチェック
		bool checkRequired = false;
		if (string.IsNullOrEmpty(Strings.Trim(System.Convert.ToString(ucoCrsCd.CodeText))))
		{
			checkRequired = true;
			this.ucoCrsCd.ExistError = true;
			ucoCrsCd.Focus();
		}
		if (ReferenceEquals(ucoSyuptDayFromTo.FromDateText, null) && ReferenceEquals(ucoSyuptDayFromTo.ToDateText, null))
		{
			this.ucoSyuptDayFromTo.FocusFromDate();
			this.ucoSyuptDayFromTo.ExistErrorForFromDate = true;
			this.ucoSyuptDayFromTo.ExistErrorForToDate = true;
			checkRequired = true;
			ucoSyuptDayFromTo.Focus();
		}
		if (checkRequired == true)
		{
			CommonProcess.createFactoryMsg().messageDisp(E90_073, E90_073_PARAM1, E90_073_PARAM2);
		}
		else
		{
			using (S04_0905 form = new S04_0905())
			{
				S04_0905ParamData param = new S04_0905ParamData();
				param.SyuptDayFrom = ucoSyuptDayFromTo.FromDateValueInt;
				param.SyuptDayTo = ucoSyuptDayFromTo.ToDateValueInt;
				param.CrsCd = ucoCrsCd.CodeText;
				param.CrsName = ucoCrsCd.ValueText;
				form.ParamData = param;
				form.patternSettings();
				base.openWindow(form);
			}

		}
	}

	protected override void btnF10_ClickOrgProc()
	{
		//ック要件のチェック
		bool checkChecked = false;
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - this.grdList.Rows.Fixed; row++)
		{
			if (this.grdList.GetCellCheck(row, this.grdList.Cols(columnList.OUTPUT_FLG.ToString()).Index) == CheckEnum.Checked)
			{
				checkChecked = true;
				break;
			}
		}
		if (checkChecked == false)
		{
			CommonProcess.createFactoryMsg().messageDisp(E90_024, E90_024_PARAM);
		}
		else
		{
			param.paramList = new List(Of S03_0408ParamSubData)();
			param.Notice = new List[Of string] From { Notice};
			param.NoticeType = NoticeType;
			for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - this.grdList.Rows.Fixed; row++)
			{
				if (this.grdList.GetCellCheck(row, this.grdList.Cols(columnList.OUTPUT_FLG.ToString()).Index) == CheckEnum.Checked)
				{
					S03_0408Pram = new S03_0408ParamSubData();
					S03_0408Pram.SyuptDay = System.Convert.ToInt32(grdList.GetData(row, columnList.SYUPT_DAY.ToString()));
					S03_0408Pram.CrsCd = grdList.GetData(row, columnList.CRS_CD.ToString()).ToString();
					S03_0408Pram.Gousya = System.Convert.ToInt32(grdList.GetData(row, columnList.GOUSYA.ToString()));
					param.paramList.Add(S03_0408Pram);
				}
			}
			//通知先設定画面を表示する
			using (S03_0408 form = new S03_0408())
			{
				form.ParamData = param;
				form.patternSettings();
				base.openWindow(form, false);
			}

		}
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
		if (ReferenceEquals(ucoSyuptDayFromTo.FromDateText, null) && ReferenceEquals(ucoSyuptDayFromTo.ToDateText, null))
		{
			this.ucoSyuptDayFromTo.FocusFromDate();
			this.ucoSyuptDayFromTo.ExistErrorForFromDate = true;
			this.ucoSyuptDayFromTo.ExistErrorForToDate = true;
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
		S04_0903_DA dataAccess = new S04_0903_DA();
		S04_0903_DASelectParam param = new S04_0903_DASelectParam();
		//パラメータに各値を設定
		//出発日FROM
		param.SyuptDayFrom = ucoSyuptDayFromTo.FromDateValueInt;
		//出発日TO
		param.SyuptDayTo = ucoSyuptDayFromTo.ToDateValueInt;
		//コースコード
		param.CrsCd = this.ucoCrsCd.CodeText;
		//号車
		if (!string.IsNullOrEmpty(System.Convert.ToString(txtGousya.Text)))
		{
			param.Gousya = int.Parse(System.Convert.ToString(this.txtGousya.Text));
		}
		//定期
		param.CrsTeiki = chkTeiki.Checked;
		//企画（Rコース）
		param.CrsKikakuR = chkKikakuR.Checked;
		//連絡済含む
		param.ChkContacted = chkContacted.Checked;
		//DA定義の検索処理を呼び出す
		return dataAccess.selectDataTable(param);
	}

	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void OvPt99SearchAfter()
	{
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - this.grdList.Rows.Fixed; row++)
		{
			this.grdList.SetCellCheck(row, this.grdList.Cols(columnList.OUTPUT_FLG.ToString()).Index, CheckEnum.Checked);
		}
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
		this.grdList.Cols(columnList.SYUPT_DAY_STR).TextAlign = TextAlignEnum.CenterCenter;
		this.grdList.Cols(columnList.LAST_CONTACT_DAY).TextAlign = TextAlignEnum.CenterCenter;
	}

	private List[] GetGridTitle()
	{
		return new List[Of GridProperty] From {
			;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.OUTPUT_FLG),.Name == columnList.OUTPUT_FLG.ToString(),.Width == 71,.DataType == typeof(bool),.AllowEditing == true},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.SYUPT_DAY_STR),.Name == columnList.SYUPT_DAY_STR.ToString(),.Width == 97},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.CRS_CD),.Name == columnList.CRS_CD.ToString(),.Width == 107},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.CRS_NAME),.Name == columnList.CRS_NAME.ToString(),.Width == 390},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.SYUPT_TIME_STR),.Name == columnList.SYUPT_TIME_STR.ToString(),.Width == 90},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.GOUSYA),.Name == columnList.GOUSYA.ToString(),.Width == 85},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.NORIBA),.Name == columnList.NORIBA.ToString(),.Width == 130},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.YOYAKU_NUM),.Name == columnList.YOYAKU_NUM.ToString(),.Width == 110},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.LAST_CONTACT_DAY),.Name == columnList.LAST_CONTACT_DAY.ToString(),.Width == 160},;
			//		new GridProperty (){.Caption = getEnumAttrValue(columnList.SYUPT_DAY),.Name == columnList.SYUPT_DAY.ToString(),.Visible == false};
			//		};
		}


		#endregion
	}