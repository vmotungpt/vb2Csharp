using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.ClientCommon.FlexGridEx;
using Hatobus.ReservationManagementSystem.Master;


/// <summary>
/// 準備金基準情報設定
/// </summary>
public class S04_0501 : PT99, iPT99
{

	#region フィールド
	#endregion

	#region 定数
	/// <summary>
	/// カラム名一覧
	/// </summary>
	private enum columnList : int
	{
		[Value("選択")]
		SELECT_CHECKBOX,
		[Value("年")]
		YEAR,
		[Value("季")]
		SEASON,
		[Value("コースコード")]
		CRS_CD,
		[Value("コース名")]
		CRS_NAME,
		[Value("コース種別")]
		HOUJIN_GAIKYAKU_KBN_NM,
		[Value("コース区分")]
		CRS_KBN_NM,
		[Value("処理")]
		PROCESS_FLG,
		[Value("基本")]
		SHINSEI_KINGAKU_NORMAL,
		[Value("追加分（代車）")]
		SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN,
		[Value("同封券枚数")]
		SHINSEI_DOFUKEN_SHEET_NUM,
		[Value("コース区分１")]
		CRS_KBN_1,
		[Value("法人／外客区分")]
		HOUJIN_GAIKYAKU_KBN,
		[Value("コース種別コード")]
		CRS_KIND,
		[Value("季コード")]
		SEASON_CD,
		[Value("号車")]
		GOUSYA
	}

	//ボタンテキスト
	private const string F12_KEY_TEXT = "F12:展開";
	//申請金額"
	private const string counterApps = "申請金額";
	//処理フラグ
	private const string ProcessFlgValue = "展開済";
	//E04_006
	private const string E04_006 = "E04_006";
	//申請金額
	private const string E04_006Param1 = "申請金額";
	//5,000
	private const string E04_006Param2 = "5,000";
	//E90_011
	private const string E90_011 = "E90_011";
	//申請金額、同封券枚数のいずれか
	private const string E90_011Param = "申請金額、同封券枚数のいずれか";
	//E04_007
	private const string E04_007 = "E04_007";
	//E90_068
	private const string E90_068 = "E90_068";
	//I90_002
	private const string I90_002 = "I90_002";
	//登録
	private const string I90_002Param = "登録";
	#endregion

	#region 列挙体

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
		//PT99プロパティ

		//【ボタン用設定】
		//CSV出力ボタン(F4)の表示可否
		base.PtIsCsvOutFlg = false;
		//プレビューボタン(F6)の表示可否
		base.PtIsPrevFlg = false;
		//印刷ボタン(F7)の表示可否
		base.PtIsPrintFlg = false;
		//検索ボタン(F8)の表示可否　※フッターのF8のため検索エリア時はFalseを設定する
		base.PtIsSearchFlg = false;
		//登録ボタン(F10)の表示可否
		base.PtIsRegFlg = false;
		//更新ボタン(F11)の表示可否
		base.PtIsUpdFlg = true;

		//【コントロール系】
		//検索エリアのグループボックスを設定する
		base.PtSearchControl = this.gbxSearch;
		//検索結果エリアのグループボックスを設定する
		base.PtResultControl = this.pnlResult;
		//詳細エリアのグループボックスを設定する
		base.PtDetailControl = null;
		//検索結果を表示するためのGrid
		base.PtResultGrid = this.grdList;
		//検索結果を表示するためのGrid
		base.PtDisplayBtn = btnVisiblerCondition;
		//検索結果を表示するためのGrid
		base.PtResultLblCnt = this.lblCount;

		//【データ系】
		//結果グリッドの最大表示件数
		base.PtMaxCount = 1000;
		//結果グリッドの最大表示件数
		//MyBase.PtResultDT = -
		//※ReadOnly 結果グリッドの選択行データ
		//MyBase.PtSelectRow = -

		//【その他】
		//実装画面(インターフェースを実装フォーム)
		base.PtMyForm = this;
		//変更チェックを行うDataTableのカラム名(ID)を設定
		base.PtDiffChkColName = new List[Of string] From { columnList.SHINSEI_KINGAKU_NORMAL.ToString(), columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString(), columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()};

		//【帳票用】
		//AR/DS選択用プロパティ
		//MyBase.PtPrintType = -
		//呼び出しDaTaStudioID
		//MyBase.PtDsPrintId = -
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
			//'画面項目定義(詳細)＜フォームの初期設定＞
			//定期観光／企画旅行
			object teikiDt = CommonProcess.getComboboxDataOfDatatable(typeof(Teiki_KikakuKbnType), false);
			//旅行ログインユーザーの所属部署に応じて初期選択を制御する
			//定期観光部／国際事業部：「定期観光」, その他の部署：「定期観光」
			CommonMstUtil.setComboCommonProperty(this.cmbTeikiKikaku, teikiDt);
			if (string.Equals(System.Convert.ToString(UserInfoManagement.toriatukaiBusyo), System.Convert.ToString(System.Convert.ToInt32(Teiki_KikakuKbnType.kikakuTravel))))
			{
				this.cmbTeikiKikaku.SelectedIndex = Teiki_KikakuKbnType.kikakuTravel - 1;
				SetDefauldValue(System.Convert.ToInt32(this.cmbTeikiKikaku.SelectedIndex));
			}
			else
			{
				//企画旅行部：「企画旅行」
				this.cmbTeikiKikaku.SelectedIndex = (System.Convert.ToInt32(Teiki_KikakuKbnType.teikiKanko)) - 1;
				SetDefauldValue(System.Convert.ToInt32(this.cmbTeikiKikaku.SelectedIndex));
			}

			this.cmbTeikiKikaku.ReadOnly = true;
			this.cmbTeikiKikaku.Enabled = false;
			this.ucoCrsKind.SetInitState();
		}
		else if (area == area.SEARCH)
		{
			//画面項目定義(詳細)＜検索条件エリアの初期設定＞
		}
		else if (area == area.RESULT)
		{
			//検索結果エリアの初期設定
			//grdの設定処理
			base.CmnInitGrid(grdList);
			//「画面項目定義」の[Grid詳細]①の項目を対象に、初期値記載された内容に準じて、項目の初期化を行う
			// grd初期設定
			InitializeResultGrid();
		}
		else if (area == area.DETAIL)
		{
			//詳細エリアの初期設定
			//　なし
		}
		else if (area == area.BUTTON)
		{
			//・ボタンエリアの初期設定
			//F12：活性
			base.F12Key_Visible = true;
			base.F12Key_Enabled = false;
			base.F12Key_Text = F12_KEY_TEXT;
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
		//独自の初期処理
		//F8ボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnSearch.Click += base.btnCom_Click;
		//条件クリアボタンクリック処理イベントを共通ボタンクリックイベントにハンドル追加を行う
		btnClear.Click += base.btnCom_Click;
		//フォームのF8ボタンプロパティに、検索ボタンを設定
		base.baseBtnF8 = this.btnSearch;
		this.ActiveControl = this.cmbTeikiKikaku;
	}

	#region 検索処理用
	///' <summary>
	///' 検索前チェック処理
	///' </summary>
	///' <returns></returns>
	//Protected Overrides Function OvPt99SearchBefore() As Boolean
	//    '＜検索前処理＞
	//    'なし
	//    Return MyBase.OvPt99SearchBefore()
	//End Function

	/// <summary>
	/// データ取得処理
	/// </summary>
	/// <returns></returns>
	protected override DataTable OvPt99SearchGetData()
	{
		//'DataAccessクラス生成
		S04_0501DA dataAccess = new S04_0501DA();
		S04_0501DA.S04_0501DASelectParam param = new S04_0501DA.S04_0501DASelectParam();

		//パラメータはIF(OUT)シートNo.1参照
		//定期・企画区分
		if (string.Equals(System.Convert.ToString(this.cmbTeikiKikaku.SelectedValue), System.Convert.ToString(System.Convert.ToInt32(Teiki_KikakuKbnType.teikiKanko))))
		{
			param.TeikiKikakuKbn = System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko);
		}
		else
		{
			param.TeikiKikakuKbn = System.Convert.ToString(Teiki_KikakuKbnType.kikakuTravel);
		}
		//コースコード
		param.CrsCd = ucoCrsCd.CodeText;
		//年
		if (!ReferenceEquals(this.txtYear.Value, null))
		{
			param.Year = System.Convert.ToInt32(this.txtYear.Value);
		}
		//季コード
		if (this.cmbSeason.SelectedIndex != 0)
		{
			param.SeasonCd = System.Convert.ToString(this.cmbSeason.SelectedIndex);
		}
		//定期（昼）
		param.TeikiHiru = this.chkNoon.Checked;
		//定期（夜）
		param.TeikiYoru = this.chkNight.Checked;
		//企画（日帰り）
		param.KikakuHigaeri = this.chkKikakuDayTrip.Checked;
		//企画（宿泊）
		param.KikakuStay = this.chkKikakuStay.Checked;
		//企画（Ｒコース）
		param.KikakuR = this.chkRCrs.Checked;
		//日本語
		param.CrsJapanese = this.ucoCrsKind.JapaneseState;
		//外国語
		param.CrsForeign = this.ucoCrsKind.ForeignState;
		//コース名
		param.CrsName = this.txtCrsName.Text;
		//コース名（カナ）
		param.CrsNameKana = this.txtCrsNameKana.Text;
		//処理フラグ
		param.ProcessFlg = this.chkProcess.Checked;
		//DA定義の検索処理を呼び出す
		return dataAccess.selectDataTable(param);
	}

	///' <summary>
	///' グリッドデータの取得とグリッド表示
	///' </summary>
	protected override void OvPt99SearchAfter()
	{
		//F12：検索結果が1件以上の場合、活性
		if (this.grdList.Rows.Count > this.grdList.Rows.Fixed)
		{
			base.F12Key_Enabled = true;
		}
		else
		{
			base.F12Key_Enabled = false;
		}
		PtResultDT.AcceptChanges();
	}
	#endregion

	#region 更新処理用
	/// <summary>
	/// 変更チェック処理
	/// </summary>
	/// <returns></returns>
	protected override bool checkDifference()
	{
		int checkedCnt = 0;
		int CountRow = 1;
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
		{
			CountRow++;
			if (this.grdList.getCheckBoxValue(row, columnList.SELECT_CHECKBOX))
			{
				object currentRow = ((DataRowView)(this.grdList.Rows(row).DataSource)).Row;
				//'選択にチェックされていて基本・追加分・同封券枚数のいずれかが変更されているものデータだけをDataTableにセットする
				if (!currentRow.RowState.Equals(DataRowState.Unchanged))
				{
					checkedCnt++;
					PtResultDT.Rows(row - this.grdList.Rows.Fixed).Item[columnList.SHINSEI_KINGAKU_NORMAL.ToString()] = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString());
					PtResultDT.Rows(row - this.grdList.Rows.Fixed).Item[columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()] = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString());
					PtResultDT.Rows(row - this.grdList.Rows.Fixed).Item[columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()] = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString());
				}

			}
		}
		//0件の場合はTrueを返却し、メッセージを出力する（CommonProcess.createFactoryMsg().messageDisp("E90_068")）
		if (checkedCnt == 0)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 更新前チェック処理
	/// </summary>
	/// <returns></returns>
	protected override bool checkUpdateItems()
	{
		S04_0501DA.S04_0501DAUpdateParam param = new S04_0501DA.S04_0501DAUpdateParam();
		int checkedCnt = 0;
		int CountRow = 1;
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
		{
			CountRow++;
			if (this.grdList.getCheckBoxValue(row, columnList.SELECT_CHECKBOX))
			{
				object currentRow = ((DataRowView)(this.grdList.Rows(row).DataSource)).Row;
				//選択にチェックされていて基本・追加分・同封券枚数のいずれかが変更されているものデータだけをDataTableにセットする
				if (!currentRow.RowState.Equals(DataRowState.Unchanged))
				{
					checkedCnt++;
					PtResultDT.Rows(row - this.grdList.Rows.Fixed).Item[columnList.SHINSEI_KINGAKU_NORMAL.ToString()] = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString());
					PtResultDT.Rows(row - this.grdList.Rows.Fixed).Item[columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()] = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString());
					PtResultDT.Rows(row - this.grdList.Rows.Fixed).Item[columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()] = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString());
				}
				if (checkedCnt != 0)
				{
					// 「画面項目定義」のチェック要件、No.1、No.2を行う
					if (!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString()) &&)
					{
						!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString()) &&;
						!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString()) &&;
						System.Convert.ToInt32(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString()) != 0 &&;
						System.Convert.ToInt32(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString()) != 0 &&;
						System.Convert.ToInt32(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString()) != 0;

						param.ShinseiKingakuNormal = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString());
						param.ShinseiKingakuDaishaJiAddBun = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString());
						param.ShinseiDofukenSheetNum = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString());
						//・申請金額_基本、または申請金額_追加分(代車)に入力があり、入力された内容が5,000円の倍数でない場合、5,000円単位で入力する規則がある旨のメッセージを表示する
						if (param.ShinseiKingakuNormal % 5000 != 0)
						{
							this.grdList.setColorError(CountRow, 8);
							CommonProcess.createFactoryMsg().messageDisp(E04_006, E04_006Param1, E04_006Param2);
							return false;
						}
						else
						{
							this.grdList.clearErrorCell();
						}
						if (param.ShinseiKingakuDaishaJiAddBun % 5000 != 0)
						{
							this.grdList.setColorError(CountRow, 9);
							CommonProcess.createFactoryMsg().messageDisp(E04_006, E04_006Param1, E04_006Param2);
							return false;
						}
						else
						{
							this.grdList.clearErrorCell();
						}
					}
					else
					{
						if (int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString()) == 0)
						{
							this.grdList.setColorError(CountRow, 8);
						}
						if (int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString()) == 0)
						{
							this.grdList.setColorError(CountRow, 9);
						}
						if (int.Parse(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString()) == 0)
						{
							this.grdList.setColorError(CountRow, 10);
						}
						//申請金額_基本、申請金額_追加分(代車)、同封券枚数のいずれかの入力が行われていない場合、入力を行う必要がある旨のメッセージを表示する
						CommonProcess.createFactoryMsg().messageDisp(E90_011, E90_011Param);
						return false;
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
		S04_0501DA dataAccess = new S04_0501DA();
		List listParam = new List(Of S04_0501DA.S04_0501DAUpdateParam);
		S04_0501DA.S04_0501DAUpdateParam param = null;
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
		{
			//変更行のみパラメータに設定する。（checkDifference()で格納したDataTable）
			if (this.grdList.getCheckBoxValue(row, columnList.SELECT_CHECKBOX))
			{
				//パラメータはIF(OUT)シートNo.2参照
				param = new S04_0501DA.S04_0501DAUpdateParam();
				//コースコード
				param.CrsCd = this.grdList.GetData(row, columnList.CRS_CD.ToString()).ToString();
				//年
				param.Year = int.Parse(this.grdList.GetData(row, columnList.YEAR.ToString()).ToString());
				//季コード
				param.SeasonCd = this.grdList.GetData(row, columnList.SEASON_CD.ToString()).ToString();
				//処理フラグ
				if (string.Equals(this.grdList.GetData(row, columnList.PROCESS_FLG.ToString()).ToString(), ProcessFlgValue.ToString()))
				{
					param.ProcessFlg = true;
				}
				else
				{
					param.ProcessFlg = false;
				}
				//同封券枚数
				param.ShinseiDofukenSheetNum = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString());
				//申請金額通常
				param.ShinseiKingakuNormal = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString());
				//申請金額代車時追加分
				param.ShinseiKingakuDaishaJiAddBun = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString());
				//システム登録ＰＧＭＩＤ
				param.SystemEntryPgmid = this.Name;
				//システム登録者コード
				param.SystemEntryPersonCd = UserInfoManagement.userId;
				//システム登録日
				param.SystemEntryDay = CommonDateUtil.getSystemTime();
				//システム更新ＰＧＭＩＤ
				param.SystemUpdatePgmid = this.Name;
				//システム更新者コード
				param.SystemUpdatePersonCd = UserInfoManagement.userId;
				//システム更新日
				param.SystemUpdateDay = CommonDateUtil.getSystemTime();

				listParam.Add(param);
			}
		}
		//DA定義の更新処理を呼び出す
		int countChange = System.Convert.ToInt32(dataAccess.executeUpdate(listParam));
		return countChange;
	}
	#endregion

	#region 出力処理用
	///' <summary>
	///' 出力前(エラーチェック)処理
	///' </summary>
	///' <returns></returns>
	//Protected Overrides Function OvPt99PrintBefore() As Boolean
	//    'TODO:画面項目定義(詳細)＜出力前処理＞

	//    Return MyBase.OvPt99PrintBefore()
	//End Function

	///' <summary>
	///' DS出力時(パラメータ設定処理)処理
	///' </summary>
	///' <returns></returns>
	//Protected Overrides Function OvPt99PrintSetDSParameter() As Boolean
	//    'TODO:DSWEB帳票設計書

	//    Return MyBase.OvPt99PrintSetDSParameter()
	//End Function

	///' <summary>
	///' AR出力時(データ取得)処理
	///' </summary>
	///' <returns></returns>
	//Protected Overrides Function OvPt99PrintARGetData() As DataTable
	//    'TODO:画面項目定義(詳細)＜出力処理＞(ARの場合)

	//    Return MyBase.OvPt99PrintARGetData()
	//End Function

	///' <summary>
	///' 出力後(データ加工等)処理
	///' </summary>
	//Protected Overrides Sub OvPt99printAfter()
	//    'TODO:画面項目定義(詳細)＜出力後処理＞
	//End Sub
	#endregion

	#region F12
	/// <summary>
	/// F12キー（台帳作成）押下時イベント
	/// </summary>
	protected override void btnF12_ClickOrgProc()
	{
		object returnValue = DbOperator(DbShoriKbn.Insert);
		//DbOperatorの処理区分：登録の処理結果が１以上の場合、以下の処理を行う。
		if (returnValue > 0)
		{
			//メッセージ出力（CommonProcess.createFactoryMsg().messageDisp("I90_002", "登録")）
			CommonProcess.createFactoryMsg().messageDisp(I90_002, I90_002Param);
			//データの再表示（btnF8_ClickOrgProc）
			btnF8_ClickOrgProc();
		}
	}
	/// <summary>
	/// 登録対象データの登録[トランザクション系]
	/// </summary>
	/// <returns></returns>
	protected override int insertTranData()
	{
		S04_0501DA dataAccess = new S04_0501DA();
		List listParam = new List(Of S04_0501DA.S04_0501DAInsertParam);
		S04_0501DA.S04_0501DAInsertParam param = null;
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
		{
			//変更行のみパラメータに設定する。（checkDifference()で格納したDataTable）
			if (this.grdList.getCheckBoxValue(row, columnList.SELECT_CHECKBOX))
			{
				//パラメータはIF(OUT)シートNo.3参照
				param = new S04_0501DA.S04_0501DAInsertParam();
				//コースコード
				param.CrsCd = this.grdList.GetData(row, columnList.CRS_CD.ToString()).ToString();
				//年
				param.Year = int.Parse(this.grdList.GetData(row, columnList.YEAR.ToString()).ToString());
				//季コード
				param.SeasonCd = this.grdList.GetData(row, columnList.SEASON_CD.ToString()).ToString();
				//邦人／外客区分
				param.HoujinGaikyakuKbn = this.grdList.GetData(row, columnList.HOUJIN_GAIKYAKU_KBN.ToString()).ToString();
				//コース種別
				param.CrsKind = this.grdList.GetData(row, columnList.CRS_KIND.ToString()).ToString();
				//コース区分１
				param.CrsKbn1 = this.grdList.GetData(row, columnList.CRS_KBN_1.ToString()).ToString();
				//定期・企画区分
				param.TeikiKikakuKbn = System.Convert.ToString(this.cmbTeikiKikaku.SelectedIndex + 1);
				//処理フラグ
				if (string.Equals(this.grdList.GetData(row, columnList.PROCESS_FLG.ToString()).ToString(), ProcessFlgValue.ToString()))
				{
					param.ProcessFlg = true;
				}
				else
				{
					param.ProcessFlg = false;
				}
				//同封券枚数
				param.ShinseiDofukenSheetNum = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString());
				//申請金額通常
				param.ShinseiKingakuNormal = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString());
				//申請金額代車時追加分
				param.ShinseiKingakuDaishaJiAddBun = int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString());
				//システム登録ＰＧＭＩＤ
				param.SystemEntryPgmid = this.Name;
				//システム登録者コード
				param.SystemEntryPersonCd = UserInfoManagement.userId;
				//システム登録日
				param.SystemEntryDay = CommonDateUtil.getSystemTime();
				//システム更新ＰＧＭＩＤ
				param.SystemUpdatePgmid = this.Name;
				//システム更新者コード
				param.SystemUpdatePersonCd = UserInfoManagement.userId;
				//システム更新日
				param.SystemUpdateDay = CommonDateUtil.getSystemTime();

				listParam.Add(param);
			}
		}
		//DA定義の追加処理を呼び出す
		int countChange = System.Convert.ToInt32(dataAccess.executeInsert(listParam));
		return countChange;
	}
	/// <summary>
	/// 更新入力項目チェック
	/// </summary>
	protected override bool checkInsertItems()
	{
		S04_0501DA.S04_0501DAUpdateParam param = new S04_0501DA.S04_0501DAUpdateParam();
		int checkedCnt = 0;
		int CountRow = 1;
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
		{
			CountRow++;
			if (this.grdList.getCheckBoxValue(row, columnList.SELECT_CHECKBOX))
			{
				checkedCnt++;
				//申請金額_基本、申請金額_追加分(代車)、同封券枚数のいずれかの入力が行われていない場合、入力を行う必要がある旨のメッセージを表示する ※申請金額_基本、申請金額_追加分(代車)、同封券枚数全てが０の場合も含む
				if (!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString()) &&)
				{
					!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString()) &&;
					!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString()) &&;
					System.Convert.ToInt32(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString()) != 0 &&;
					System.Convert.ToInt32(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString()) != 0 &&;
					System.Convert.ToInt32(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString()) != 0;
					object currentRow = ((DataRowView)(this.grdList.Rows(row).DataSource)).Row;
					this.grdList.clearErrorCell();
					//「照会結果一覧」グリッドの値変更後、更新操作（DBへのデータ反映）が行われていない場合、先にDB更新を行う必要がある旨のメッセージを表示する
					if (!currentRow.RowState.Equals(DataRowState.Unchanged))
					{
						CommonProcess.createFactoryMsg().messageDisp(E04_007);
						return false;
					}
				}
				else
				{
					if (int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_NORMAL.ToString()).ToString()) == 0)
					{
						this.grdList.setColorError(CountRow, 8);
					}
					if (int.Parse(this.grdList.GetData(row, columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString()).ToString()) == 0)
					{
						this.grdList.setColorError(CountRow, 9);
					}
					if (int.Parse(this.grdList.GetData(row, columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString()).ToString()) == 0)
					{
						this.grdList.setColorError(CountRow, 10);
					}
					//申請金額_基本、申請金額_追加分(代車)、同封券枚数のいずれかの入力が行われていない場合、入力を行う必要がある旨のメッセージを表示する
					CommonProcess.createFactoryMsg().messageDisp(E90_011, E90_011Param);
					return false;
				}
			}
		}
		//0件の場合はTrueを返却し、メッセージを出力する（CommonProcess.createFactoryMsg().messageDisp("E90_068")）
		if (checkedCnt == 0)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_068");
			return false;
		}
		return true;
	}
	#endregion

	#region PT99外ファンクションキー
	///' <summary>
	///' FXXボタン押下時の独自処理
	///' </summary>
	//Protected Overrides Sub btnFXX_ClickOrgProc()
	//End Sub
	#endregion
	#endregion

	#region Privateメソッド(画面独自)

	/// <summary>
	/// Grid列初期設定
	/// </summary>
	private void InitializeResultGrid()
	{
		List[] grdListGridTitle = new List[Of GridProperty] From {
			;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SELECT_CHECKBOX),.Name == columnList.SELECT_CHECKBOX.ToString(),.Width == 39,.DataType == typeof(bool),.Visible == true,.AllowEditing == true },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.YEAR),.Name == columnList.YEAR.ToString(),.Width == 39 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SEASON),.Name == columnList.SEASON.ToString(),.Width == 80 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CRS_CD),.Name == columnList.CRS_CD.ToString(),.Width == 108 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CRS_NAME),.Name == columnList.CRS_NAME.ToString(),.Width == 360 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.HOUJIN_GAIKYAKU_KBN_NM),.Name == columnList.HOUJIN_GAIKYAKU_KBN_NM.ToString(),.Width == 117 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CRS_KBN_NM),.Name == columnList.CRS_KBN_NM.ToString(),.Width == 112 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.PROCESS_FLG),.Name == columnList.PROCESS_FLG.ToString(),.Width == 72 },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SHINSEI_KINGAKU_NORMAL),.Name == columnList.SHINSEI_KINGAKU_NORMAL.ToString(),.Width == 95,.Visible == true,.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(9, ControlFormat.HankakuSujiKigou) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN),.Name == columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN.ToString(),.Width == 113,.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(9, ControlFormat.HankakuSujiKigou) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SHINSEI_DOFUKEN_SHEET_NUM),.Name == columnList.SHINSEI_DOFUKEN_SHEET_NUM.ToString(),.Width == 87,.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(3, ControlFormat.HankakuSujiKigou) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CRS_KBN_1),.Name == columnList.CRS_KBN_1.ToString(),.Width == 50,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.HOUJIN_GAIKYAKU_KBN),.Name == columnList.HOUJIN_GAIKYAKU_KBN.ToString(),.Width == 50,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CRS_KIND),.Name == columnList.CRS_KIND.ToString(),.Width == 50,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SEASON_CD),.Name == columnList.SEASON_CD.ToString(),.Width == 50,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.GOUSYA),.Name == columnList.GOUSYA.ToString(),.Width == 50,.Visible == false };
		};
		this.grdList.Initialize(grdListGridTitle);
		this.grdList.Cols(columnList.YEAR).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
		SetGridHeaderitem();
	}

	/// <summary>
	/// 一覧ヘッダー項目設定
	/// </summary>
	private void SetGridHeaderitem()
	{
		//' セルの結合
		//' 固定セルのマージモードを設定
		// グリッドのAllowMergingプロパティを設定
		this.grdList.AllowMerging = AllowMergingEnum.Custom;
		//マージ（結合）したいセル範囲を取得
		CellRange cr = null;
		//選択の横幅を結合
		cr = grdList.GetCellRange(0, 0, 1, 0);
		grdList.MergedRanges.Add(cr);
		grdList[0, 0] = getEnumAttrValue(columnList.SELECT_CHECKBOX);
		//年の横幅を結合
		cr = grdList.GetCellRange(0, 1, 1, 1);
		grdList.MergedRanges.Add(cr);
		grdList[0, 1] = getEnumAttrValue(columnList.YEAR);
		//季の横幅を結合
		cr = grdList.GetCellRange(0, 2, 1, 2);
		grdList.MergedRanges.Add(cr);
		grdList[0, 2] = getEnumAttrValue(columnList.SEASON);
		//コースコードの横幅を結合
		cr = grdList.GetCellRange(0, 3, 1, 3);
		grdList.MergedRanges.Add(cr);
		grdList[0, 3] = getEnumAttrValue(columnList.CRS_CD);
		//コース名の横幅を結合
		cr = grdList.GetCellRange(0, 4, 1, 4);
		grdList.MergedRanges.Add(cr);
		grdList[0, 4] = getEnumAttrValue(columnList.CRS_NAME);
		//コース種別の横幅を結合
		cr = grdList.GetCellRange(0, 5, 1, 5);
		grdList.MergedRanges.Add(cr);
		grdList[0, 5] = getEnumAttrValue(columnList.HOUJIN_GAIKYAKU_KBN_NM);
		//コース区分の横幅を結合
		cr = grdList.GetCellRange(0, 6, 1, 6);
		grdList.MergedRanges.Add(cr);
		grdList[0, 6] = getEnumAttrValue(columnList.CRS_KBN_NM);
		//処理の横幅を結合
		cr = grdList.GetCellRange(0, 7, 1, 7);
		grdList.MergedRanges.Add(cr);
		grdList[0, 7] = getEnumAttrValue(columnList.PROCESS_FLG);
		//申請金額の横幅を結合
		cr = grdList.GetCellRange(0, 8, 0, 9);
		grdList.MergedRanges.Add(cr);
		grdList[0, 8] = counterApps;
		//基本の横幅を結合
		grdList[1, 8] = getEnumAttrValue(columnList.SHINSEI_KINGAKU_NORMAL);
		//追加分（代車）の横幅を結合
		grdList[1, 9] = getEnumAttrValue(columnList.SHINSEI_KINGAKU_DAISHA_JI_ADD_BUN);
		cr = grdList.GetCellRange(0, 10, 1, 10);
		grdList.MergedRanges.Add(cr);
		//同封券枚数の横幅を結合
		grdList[0, 10] = getEnumAttrValue(columnList.SHINSEI_DOFUKEN_SHEET_NUM);
	}

	/// <summary>
	/// デフォルト値を設定
	/// </summary>
	private void SetDefauldValue(int indexOfTeikiKikaku)
	{
		//【定期／企画に「定期観光」選択時】
		if (indexOfTeikiKikaku == 0)
		{
			//ブランク／冬・早春／春・初夏／夏／秋
			CommonMstUtil.setComboBox(CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.seasonMaster_Teiki), this.cmbSeason, true);
			//日帰り・宿泊・Rコースチェックボックスを非活性(チェックもオフ)とし、昼・夜チェックボックスを活性とする
			this.chkKikakuDayTrip.Enabled = false;
			this.chkKikakuDayTrip.Checked = false;
			this.chkKikakuStay.Enabled = false;
			this.chkKikakuStay.Checked = false;
			this.chkRCrs.Enabled = false;
			this.chkRCrs.Checked = false;

			this.chkNoon.Enabled = true;
			this.chkNoon.Checked = false;
			this.chkNight.Enabled = true;
			this.chkNight.Checked = false;
		}
		else
		{
			//【定期／企画に「企画旅行」選択時】
			//ブランク／正月／早春／春／初夏／夏／秋／冬／通年／その他
			CommonMstUtil.setComboBox(CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.seasonMaster_Kikaku), this.cmbSeason, true);
			//日帰り・宿泊・Rコースチェックボックスを活性とし、昼・夜チェックボックスを非活性(チェックもオフ)とする
			this.chkKikakuDayTrip.Enabled = true;
			this.chkKikakuDayTrip.Checked = false;
			this.chkKikakuStay.Enabled = true;
			this.chkKikakuStay.Checked = false;
			this.chkRCrs.Enabled = true;
			this.chkRCrs.Checked = false;

			this.chkNoon.Enabled = false;
			this.chkNoon.Checked = false;
			this.chkNight.Enabled = false;
			this.chkNight.Checked = false;
		}
	}

	private void btnClear_Click(object sender, EventArgs e)
	{
		//対象年
		this.txtYear.Text = string.Empty;
		//対象季
		this.cmbSeason.SelectedIndex = 0;
		//コースコード/名称
		this.ucoCrsCd.CodeText = string.Empty;
		this.ucoCrsCd.ValueText = string.Empty;
		//日本語／外国語
		this.ucoCrsKind.JapaneseState = false;
		this.ucoCrsKind.ForeignState = false;
		//定期（昼）
		this.chkNoon.Checked = false;
		//定期（夜）
		this.chkNight.Checked = false;
		//企画（日帰り）
		this.chkKikakuDayTrip.Checked = false;
		//企画（宿泊）
		this.chkKikakuStay.Checked = false;
		//企画（Ｒコース）
		this.chkRCrs.Checked = false;
		//コース名
		this.txtCrsName.Text = string.Empty;
		//コース名（ｶﾅ）
		this.txtCrsNameKana.Text = string.Empty;
		//展開済み含む
		this.chkProcess.Checked = false;
	}

	#endregion

	#region Privateメソッド(イベント)

	#endregion

}