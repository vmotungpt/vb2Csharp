using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.Master;
using Hatobus.ReservationManagementSystem.ClientCommon.FlexGridEx;
using Hatobus.ReservationManagementSystem.Yoyaku;


public class S04_0801 : PT99, iPT99
{
	public S04_0801()
	{
		updateParamList = new List[Of S04_0801_DA.S04_00801DAUpdateParam + 1];

	}

	#region フィールド
	private List[] updateParamList;

	private const string E90_017_PARAM = "車番";
	#endregion

	#region 定数
	private enum columnList : int
	{

		[Value("出発日")]
		SYUPT_DAY_STR,
		[Value("日付")]
		SYUPT_DAY,
		[Value("コースコード")]
		CRS_CD,
		[Value("号車")]
		GOUSYA,
		[Value("コース名")]
		CRS_NAME,
		[Value("車番")]
		CAR_NO,
		[Value("経由地")]
		HAISYA_KEIYU_NAME_1,
		[Value("出発時間")]
		TIME_1,
		[Value("人数")]
		NINZU_1,
		[Value("車番")]
		CAR_NO_1,
		[Value("経由地")]
		HAISYA_KEIYU_NAME_2,
		[Value("出発時間")]
		TIME_2,
		[Value("人数")]
		NINZU_2,
		[Value("車番")]
		CAR_NO_2,
		[Value("経由地")]
		HAISYA_KEIYU_NAME_3,
		[Value("出発時間")]
		TIME_3,
		[Value("人数")]
		NINZU_3,
		[Value("車番")]
		CAR_NO_3,
		[Value("経由地")]
		HAISYA_KEIYU_NAME_4,
		[Value("出発時間")]
		TIME_4,
		[Value("人数")]
		NINZU_4,
		[Value("車番")]
		CAR_NO_4,
		[Value("経由地")]
		HAISYA_KEIYU_NAME_5,
		[Value("出発時間")]
		TIME_5,
		[Value("人数")]
		NINZU_5,
		[Value("車番")]
		CAR_NO_5,
		[Value("EDIT_FLG1")]
		EDIT_FLG_1,
		[Value("EDIT_FLG2")]
		EDIT_FLG_2,
		[Value("EDIT_FLG3")]
		EDIT_FLG_3,
		[Value("EDIT_FLG4")]
		EDIT_FLG_4,
		[Value("コースコード1(親)")]
		OYA_CRS_CD_1,
		[Value("号車1(親)")]
		OYA_GOUSYA_1,
		[Value("出発日 1(親)")]
		OYA_SYUPT_DAY_1,
		[Value("コースコード2(親)")]
		OYA_CRS_CD_2,
		[Value("号車2(親)")]
		OYA_GOUSYA_2,
		[Value("出発日 2(親)")]
		OYA_SYUPT_DAY_2,
		[Value("コースコード3(親)")]
		OYA_CRS_CD_3,
		[Value("号車3(親)")]
		OYA_GOUSYA_3,
		[Value("出発日 3(親)")]
		OYA_SYUPT_DAY_3,
		[Value("コースコード4(親)")]
		OYA_CRS_CD_4,
		[Value("号車4(親)")]
		OYA_GOUSYA_4,
		[Value("出発日 4(親)")]
		OYA_SYUPT_DAY_4

	}
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
		//ボタン用設定】
		// CSV出力可否
		base.PtIsCsvOutFlg = false;
		// ﾌﾟﾚﾋﾞｭｰ可否
		base.PtIsPrevFlg = false;
		// 印刷/出力可否
		base.PtIsPrintFlg = false;
		// 検索可否
		base.PtIsSearchFlg = false;
		// 登録可否
		base.PtIsRegFlg = false;
		// 更新可否
		base.PtIsUpdFlg = true;

		//【コントロール系】
		// 検索エリアコンテナ
		base.PtSearchControl = this.gbxSearch;
		// 検索結果エリアコンテナ
		base.PtResultControl = this.pnlResult;
		// 詳細エリアコンテナ
		base.PtDetailControl = null;
		// 検索結果グリッド
		base.PtResultGrid = this.grdList;
		// 表示/非表示ボタン
		base.PtDisplayBtn = btnVisiblerCondition;
		// 件数表示ラベル
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
		base.PtDiffChkColName = null;

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
		}
		else if (area == area.RESULT)
		{
			//grdの設定処理
			base.CmnInitGrid(grdList);
			// grd初期設定
			InitializeResultGrid();
		}
		else if (area == area.DETAIL)
		{
		}
		else if (area == area.BUTTON)
		{
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

	/// <summary>
	/// 差分チェック
	/// </summary>
	protected override bool checkDifference()
	{
		return false;
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
		this.ActiveControl = this.ucoSyuptDay;

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
		S04_0801_DA.S04_0801DASelectParam param = new S04_0801_DA.S04_0801DASelectParam();
		S04_0801_DA dataAccess = new S04_0801_DA();
		// 出発日
		param.SyuptDay = this.ucoSyuptDay.ValueInt.Value;
		return dataAccess.selectDataTable(param);
	}

	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void OvPt99SearchAfter()
	{

		//検索結果が0件の場合
		if (this.grdList.Rows.Count == 0)
		{
			//「F11:更新」ボタンを非活性とする
			base.F11Key_Enabled = false;
		}
		else
		{
			//「F11:更新」ボタンを活性とする
			base.F11Key_Enabled = true;
		}

		int _EDIT_FLG = 0;
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
		{
			//DA項目EDIT_FLG1>1の場合編集可能
			if (!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.EDIT_FLG_1).ToString()))
			{
				_EDIT_FLG = int.Parse(this.grdList.GetData(row, columnList.EDIT_FLG_1.ToString()).ToString());
				if (!(_EDIT_FLG > 1))
				{
					this.grdList.setColorNonEdit(row, this.grdList.Cols(columnList.CAR_NO_1.ToString()).Index);
				}
			}
			//DA項目EDIT_FLG2>1の場合編集可能
			if (!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.EDIT_FLG_2.ToString()).ToString()))
			{
				_EDIT_FLG = int.Parse(this.grdList.GetData(row, columnList.EDIT_FLG_2.ToString()).ToString());
				if (!(_EDIT_FLG > 1))
				{
					this.grdList.setColorNonEdit(row, this.grdList.Cols(columnList.CAR_NO_2.ToString()).Index);
				}
			}
			//DA項目EDIT_FLG3>1の場合編集可能
			if (!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.EDIT_FLG_3.ToString()).ToString()))
			{
				_EDIT_FLG = int.Parse(this.grdList.GetData(row, columnList.EDIT_FLG_3.ToString()).ToString());
				if (!(_EDIT_FLG > 1))
				{
					this.grdList.setColorNonEdit(row, this.grdList.Cols(columnList.CAR_NO_3.ToString()).Index);
				}
			}
			//DA項目EDIT_FLG4>1の場合編集可能
			if (!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.EDIT_FLG_4.ToString()).ToString()))
			{
				_EDIT_FLG = int.Parse(this.grdList.GetData(row, columnList.EDIT_FLG_4.ToString()).ToString());
				if (!(_EDIT_FLG > 1))
				{
					this.grdList.setColorNonEdit(row, this.grdList.Cols(columnList.CAR_NO_4.ToString()).Index);
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

		S04_0801_DA.S04_00801DAUpdateParam param = null;
		updateParamList = new List(Of S04_0801_DA.S04_00801DAUpdateParam);
		this.grdList.clearErrorCell();
		//(結果)車番
		int CarNo = new int();
		//配車経由地１
		System.String HaisyaKeiyuName1 = string.Empty;
		System.String Time1 = string.Empty;
		int CarNo1 = new int();
		//配車経由地２
		System.String HaisyaKeiyuName2 = string.Empty;
		System.String Time2 = string.Empty;
		int CarNo2 = new int();
		//配車経由地３
		System.String HaisyaKeiyuName3 = string.Empty;
		System.String Time3 = string.Empty;
		int CarNo3 = new int();
		//配車経由地４
		System.String HaisyaKeiyuName4 = string.Empty;
		System.String Time4 = string.Empty;
		int CarNo4 = new int();
		//配車経由地５
		System.String HaisyaKeiyuName5 = string.Empty;

		//車番リスト
		List carNoList = new List(Of int);
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
		{
			carNoList.Add(int.Parse(this.grdList.GetData(row, columnList.CAR_NO.ToString()).ToString()));
		}

		List[] DiffChkColName = new List[Of string + 1];
		DiffChkColName = new List[Of string] From { columnList.CAR_NO_1.ToString(), columnList.CAR_NO_2.ToString(), columnList.CAR_NO_3.ToString(), columnList.CAR_NO_4.ToString()};

		int CarNoCompare = new int();
		System.String HaisyaKeiyuNameNowComapre = string.Empty;
		System.String TimeCompare = string.Empty;
		System.String HaisyaKeiyuNameNextComapre = string.Empty;

		int countError = 0;

		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - 1; row++)
		{
			object dataRow = ((DataRowView)(this.grdList.Rows(row).DataSource)).Row;
			//データが変更されたことを確認
			if (!dataRow.RowState.Equals(DataRowState.Unchanged))
			{
				foreach (string carNoItem in DiffChkColName)
				{
					if (!dataRow.Item(carNoItem, DataRowVersion.Original).ToString().Equals(dataRow.Item(carNoItem, DataRowVersion.Current).ToString()))
					{

						//1.「車番1が変更されている場合」
						if (carNoItem.Equals(columnList.CAR_NO_1.ToString()))
						{
							if (string.IsNullOrEmpty(this.grdList.GetData(row, columnList.CAR_NO_1.ToString()).ToString()))
							{
								this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_1.ToString()).Index);
								countError++;
								continue;
							}
							//車番
							if (!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.CAR_NO.ToString()).ToString()))
							{
								CarNo = int.Parse(this.grdList.GetData(row, columnList.CAR_NO.ToString()).ToString());
							}
							//配車経由地１(車番)
							CarNo1 = int.Parse(this.grdList.GetData(row, columnList.CAR_NO_1.ToString()).ToString());
							//「CAR_NO_1」は「車番リスト」に存在しません
							if (carNoList.Contains(CarNo1) == false)
							{
								this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_1.ToString()).Index);
								countError++;
								continue;
							}

							if (CarNo != CarNo1)
							{
								for (int rCN = this.grdList.Rows.Fixed; rCN <= this.grdList.Rows.Count - 1; rCN++)
								{
									if (rCN != row)
									{
										//車番
										CarNo = int.Parse(this.grdList.GetData(rCN, columnList.CAR_NO.ToString()).ToString());
										if (CarNo == CarNo1)
										{
											//配車経由地１(経由地)
											HaisyaKeiyuName1 = this.grdList.GetData(row, columnList.HAISYA_KEIYU_NAME_1.ToString()).ToString();
											//配車経由地１(出発時間)
											Time1 = this.grdList.GetData(row, columnList.TIME_1.ToString()).ToString();
											//配車経由地２(経由地)
											HaisyaKeiyuName2 = this.grdList.GetData(row, columnList.HAISYA_KEIYU_NAME_2.ToString()).ToString();

											//比較用データ
											CarNoCompare = int.Parse(this.grdList.GetData(rCN, columnList.CAR_NO_1.ToString()).ToString());
											HaisyaKeiyuNameNowComapre = this.grdList.GetData(rCN, columnList.HAISYA_KEIYU_NAME_1.ToString()).ToString();
											TimeCompare = this.grdList.GetData(rCN, columnList.TIME_1.ToString()).ToString();
											HaisyaKeiyuNameNextComapre = this.grdList.GetData(rCN, columnList.HAISYA_KEIYU_NAME_2.ToString()).ToString();

											//・変更後の値と一致する車番=車番1 且 出発時間1、配車経由コード１、配車経由コード２ が一致するレコードが存在するかをチェック、存在しない場合はエラーメッセージを表示する。
											//・(変更後の値 = 車番が異なる場合)車番が車番1 且 出発時間1、配車経由コード１、配車経由コード２が一致するレコードが存在する場合、エラーメッセージを表示する。
											if (string.Equals(HaisyaKeiyuName1, HaisyaKeiyuNameNowComapre) == false ||)
											{
												string.Equals(Time1, TimeCompare) = System.Convert.ToBoolean(false ||);
												CarNo1(!= CarNoCompare ||);
												string.Equals(HaisyaKeiyuName2, HaisyaKeiyuNameNextComapre) = System.Convert.ToBoolean(false);
												this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_1.ToString()).Index);
												countError++;
												continue;
											}

											//変更されたデータを追加し、エラーなし
											//配車経由地１
											param = getDataParamForUpdate(((DataRowView)(this.grdList.Rows(row).DataSource)).Row, ((DataRowView)(this.grdList.Rows(rCN).DataSource)).Row, 1, false);
											updateParamList.Add(param);
										}
									}
								}
							}
							else if (CarNo == CarNo1)
							{
								//変更されたデータを追加し、エラーなし
								//配車経由地１
								param = getDataParamForUpdate(((DataRowView)(this.grdList.Rows(row).DataSource)).Row, ((DataRowView)(this.grdList.Rows(row).DataSource)).Row, 1, true);
								updateParamList.Add(param);
							}

							//2.「車番2が変更されている場合」
						}
						else if (carNoItem.Equals(columnList.CAR_NO_2.ToString()))
						{
							if (string.IsNullOrEmpty(this.grdList.GetData(row, columnList.CAR_NO_2.ToString()).ToString()))
							{
								this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_2.ToString()).Index);
								countError++;
								continue;
							}
							//車番
							if (!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.CAR_NO.ToString()).ToString()))
							{
								CarNo = int.Parse(this.grdList.GetData(row, columnList.CAR_NO.ToString()).ToString());
							}
							//配車経由地２(車番)
							CarNo2 = int.Parse(this.grdList.GetData(row, columnList.CAR_NO_2.ToString()).ToString());
							//「CAR_NO_2」は「車番リスト」に存在しません
							if (carNoList.Contains(CarNo2) == false)
							{
								this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_2.ToString()).Index);
								countError++;
								continue;
							}

							if (CarNo != CarNo2)
							{
								for (int rCN = this.grdList.Rows.Fixed; rCN <= this.grdList.Rows.Count - 1; rCN++)
								{
									if (rCN != row)
									{
										//車番
										CarNo = int.Parse(this.grdList.GetData(rCN, columnList.CAR_NO.ToString()).ToString());
										if (CarNo == CarNo2)
										{
											//配車経由地２(経由地)
											HaisyaKeiyuName2 = this.grdList.GetData(row, columnList.HAISYA_KEIYU_NAME_2.ToString()).ToString();
											//配車経由地２(出発時間)
											Time2 = this.grdList.GetData(row, columnList.TIME_2.ToString()).ToString();
											//配車経由地３(経由地)
											HaisyaKeiyuName3 = this.grdList.GetData(row, columnList.HAISYA_KEIYU_NAME_3.ToString()).ToString();

											//比較用データ
											CarNoCompare = int.Parse(this.grdList.GetData(rCN, columnList.CAR_NO_2.ToString()).ToString());
											HaisyaKeiyuNameNowComapre = this.grdList.GetData(rCN, columnList.HAISYA_KEIYU_NAME_2.ToString()).ToString();
											TimeCompare = this.grdList.GetData(rCN, columnList.TIME_2.ToString()).ToString();
											HaisyaKeiyuNameNextComapre = this.grdList.GetData(rCN, columnList.HAISYA_KEIYU_NAME_3.ToString()).ToString();

											//・変更後の値と一致する車番=車番2 且 出発時間2、配車経由コード２、配車経由コード３が一致するレコードが存在するかをチェック、存在しない場合はエラーメッセージを表示する。
											//・(変更後の値 = 車番が異なる場合)車番が車番2 且 出発時間2、配車経由コード２、配車経由コード３が一致するレコードが存在する場合、エラーメッセージを表示する。
											if (string.Equals(HaisyaKeiyuName2, HaisyaKeiyuNameNowComapre) == false ||)
											{
												string.Equals(Time2, TimeCompare) = System.Convert.ToBoolean(false ||);
												CarNo2(!= CarNoCompare ||);
												string.Equals(HaisyaKeiyuName3, HaisyaKeiyuNameNextComapre) = System.Convert.ToBoolean(false);
												this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_2.ToString()).Index);
												countError++;
												continue;
											}

											//変更されたデータを追加し、エラーなし
											//配車経由地2
											param = getDataParamForUpdate(((DataRowView)(this.grdList.Rows(row).DataSource)).Row, ((DataRowView)(this.grdList.Rows(rCN).DataSource)).Row, 2, false);
											updateParamList.Add(param);
										}
									}
								}
							}
							else if (CarNo == CarNo2)
							{
								//変更されたデータを追加し、エラーなし
								//配車経由地２
								param = getDataParamForUpdate(((DataRowView)(this.grdList.Rows(row).DataSource)).Row, ((DataRowView)(this.grdList.Rows(row).DataSource)).Row, 2, true);
								updateParamList.Add(param);
							}

							//3.「車番3が変更されている場合」
						}
						else if (carNoItem.Equals(columnList.CAR_NO_3.ToString()))
						{
							if (string.IsNullOrEmpty(this.grdList.GetData(row, columnList.CAR_NO_3.ToString()).ToString()))
							{
								this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_3.ToString()).Index);
								countError++;
								continue;
							}
							//車番
							if (!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.CAR_NO.ToString()).ToString()))
							{
								CarNo = int.Parse(this.grdList.GetData(row, columnList.CAR_NO.ToString()).ToString());
							}
							//配車経由地３(車番)
							CarNo3 = int.Parse(this.grdList.GetData(row, columnList.CAR_NO_3.ToString()).ToString());
							//「CAR_NO_3」は「車番リスト」に存在しません
							if (carNoList.Contains(CarNo3) == false)
							{
								this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_3.ToString()).Index);
								countError++;
								continue;
							}
							if (CarNo != CarNo3)
							{
								for (int rCN = this.grdList.Rows.Fixed; rCN <= this.grdList.Rows.Count - 1; rCN++)
								{
									if (rCN != row)
									{
										//車番
										CarNo = int.Parse(this.grdList.GetData(rCN, columnList.CAR_NO.ToString()).ToString());
										if (CarNo == CarNo3)
										{
											//配車経由地３(経由地)
											HaisyaKeiyuName3 = this.grdList.GetData(row, columnList.HAISYA_KEIYU_NAME_3.ToString()).ToString();
											//配車経由地３(出発時間)
											Time3 = this.grdList.GetData(row, columnList.TIME_3.ToString()).ToString();
											//配車経由地４(経由地)
											HaisyaKeiyuName4 = this.grdList.GetData(row, columnList.HAISYA_KEIYU_NAME_4.ToString()).ToString();

											//比較用データ
											CarNoCompare = int.Parse(this.grdList.GetData(rCN, columnList.CAR_NO_3.ToString()).ToString());
											HaisyaKeiyuNameNowComapre = this.grdList.GetData(rCN, columnList.HAISYA_KEIYU_NAME_3.ToString()).ToString();
											TimeCompare = this.grdList.GetData(rCN, columnList.TIME_3.ToString()).ToString();
											HaisyaKeiyuNameNextComapre = this.grdList.GetData(rCN, columnList.HAISYA_KEIYU_NAME_4.ToString()).ToString();

											//・変更後の値と一致する車番=車番3 且 出発時間3、配車経由コード３、配車経由コード４が一致するレコードが存在するかをチェック、存在しない場合はエラーメッセージを表示する。
											//・(変更後の値 = 車番が異なる場合)車番が車番3 且 出発時間3、配車経由コード３、配車経由コード４が一致するレコードが存在する場合、エラーメッセージを表示する。
											if (string.Equals(HaisyaKeiyuName3, HaisyaKeiyuNameNowComapre) == false ||)
											{
												string.Equals(Time3, TimeCompare) = System.Convert.ToBoolean(false ||);
												CarNo3(!= CarNoCompare ||);
												string.Equals(HaisyaKeiyuName4, HaisyaKeiyuNameNextComapre) = System.Convert.ToBoolean(false);
												this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_3.ToString()).Index);
												countError++;
												continue;
											}

											//変更されたデータを追加し、エラーなし
											//配車経由地3
											param = getDataParamForUpdate(((DataRowView)(this.grdList.Rows(row).DataSource)).Row, ((DataRowView)(this.grdList.Rows(rCN).DataSource)).Row, 3, false);
											updateParamList.Add(param);
										}
									}
								}
							}
							else if (CarNo == CarNo3)
							{
								//変更されたデータを追加し、エラーなし
								//配車経由地3
								param = getDataParamForUpdate(((DataRowView)(this.grdList.Rows(row).DataSource)).Row, ((DataRowView)(this.grdList.Rows(row).DataSource)).Row, 3, true);
								updateParamList.Add(param);
							}

							//4.「車番4が変更されている場合」
						}
						else if (carNoItem.Equals(columnList.CAR_NO_4.ToString()))
						{
							if (string.IsNullOrEmpty(this.grdList.GetData(row, columnList.CAR_NO_4.ToString()).ToString()))
							{
								this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_4.ToString()).Index);
								countError++;
								continue;
							}
							//車番
							if (!string.IsNullOrEmpty(this.grdList.GetData(row, columnList.CAR_NO.ToString()).ToString()))
							{
								CarNo = int.Parse(this.grdList.GetData(row, columnList.CAR_NO.ToString()).ToString());
							}
							//配車経由地４(車番)
							CarNo4 = int.Parse(this.grdList.GetData(row, columnList.CAR_NO_4.ToString()).ToString());
							//「CAR_NO_4」は「車番リスト」に存在しません
							if (carNoList.Contains(CarNo4) == false)
							{
								this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_4.ToString()).Index);
								countError++;
								continue;
							}

							if (CarNo != CarNo4)
							{
								for (int rCN = this.grdList.Rows.Fixed; rCN <= this.grdList.Rows.Count - 1; rCN++)
								{
									if (rCN != row)
									{
										//車番
										CarNo = int.Parse(this.grdList.GetData(rCN, columnList.CAR_NO.ToString()).ToString());
										if (CarNo == CarNo4)
										{
											//配車経由地４(経由地)
											HaisyaKeiyuName4 = this.grdList.GetData(row, columnList.HAISYA_KEIYU_NAME_4.ToString()).ToString();
											//配車経由地４(出発時間)
											Time4 = this.grdList.GetData(row, columnList.TIME_4.ToString()).ToString();
											//配車経由地５(経由地)
											HaisyaKeiyuName5 = this.grdList.GetData(row, columnList.HAISYA_KEIYU_NAME_5.ToString()).ToString();

											//比較用データ
											CarNoCompare = int.Parse(this.grdList.GetData(rCN, columnList.CAR_NO_4.ToString()).ToString());
											HaisyaKeiyuNameNowComapre = this.grdList.GetData(rCN, columnList.HAISYA_KEIYU_NAME_4.ToString()).ToString();
											TimeCompare = this.grdList.GetData(rCN, columnList.TIME_4.ToString()).ToString();
											HaisyaKeiyuNameNextComapre = this.grdList.GetData(rCN, columnList.HAISYA_KEIYU_NAME_5.ToString()).ToString();

											//・変更後の値と一致する車番=車番4 且 出発時間4、配車経由コード４、配車経由コード５が一致するレコードが存在するかをチェック、存在しない場合はエラーメッセージを表示する。
											//・(変更後の値 = 車番が異なる場合)車番が車番4 且 出発時間4、配車経由コード４、配車経由コード５が一致するレコードが存在する場合、エラーメッセージを表示する。
											if (string.Equals(HaisyaKeiyuName4, HaisyaKeiyuNameNowComapre) == false ||)
											{
												string.Equals(Time4, TimeCompare) = System.Convert.ToBoolean(false ||);
												CarNo4(!= CarNoCompare ||);
												string.Equals(HaisyaKeiyuName5, HaisyaKeiyuNameNextComapre) = System.Convert.ToBoolean(false);
												this.grdList.setColorError(row, this.grdList.Cols(columnList.CAR_NO_4.ToString()).Index);
												countError++;
												continue;
											}

											//変更されたデータを追加し、エラーなし
											//配車経由地4
											param = getDataParamForUpdate(((DataRowView)(this.grdList.Rows(row).DataSource)).Row, ((DataRowView)(this.grdList.Rows(rCN).DataSource)).Row, 4, false);
											updateParamList.Add(param);
										}
									}
								}
							}
							else if (CarNo == CarNo4)
							{
								//変更されたデータを追加し、エラーなし
								//配車経由地4
								param = getDataParamForUpdate(((DataRowView)(this.grdList.Rows(row).DataSource)).Row, ((DataRowView)(this.grdList.Rows(row).DataSource)).Row, 4, true);
								updateParamList.Add(param);
							}
						}

					}
				}
			}
		}

		if (countError > 0)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_017", E90_017_PARAM);
			return false;
		}

		return true;
	}

	/// <summary>
	/// 更新処理
	/// </summary>
	/// <returns></returns>
	protected override int updateTranData()
	{

		S04_0801_DA dataAccess = new S04_0801_DA();
		int returnValue = 0;
		if (updateParamList.Count() > 0)
		{
			//DA定義の更新処理を呼び出す
			returnValue = System.Convert.ToInt32(dataAccess.execute(updateParamList));
			this.grdList.clearErrorCell();
		}
		return returnValue;
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
	private void InitializeResultGrid()
	{
		this.grdList.Initialize(GetGridTitle());
		SetGridHeaderitem();
		this.grdList.Cols(columnList.SYUPT_DAY_STR).TextAlign = TextAlignEnum.CenterCenter;
	}
	/// <summary>
	/// 一覧ヘッダー項目設定
	/// </summary>
	private List[] GetGridTitle()
	{
		List colsList = new List[Of GridProperty] From {
			;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SYUPT_DAY_STR),.Name == columnList.SYUPT_DAY_STR.ToString(),.Width == 129,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CRS_CD),.Name == columnList.CRS_CD.ToString(),.Width == 109,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.GOUSYA),.Name == columnList.GOUSYA.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CRS_NAME),.Name == columnList.CRS_NAME.ToString(),.Width == 124,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CAR_NO),.Name == columnList.CAR_NO.ToString(),.Width == 74,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.HAISYA_KEIYU_NAME_1),.Name == columnList.HAISYA_KEIYU_NAME_1.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.TIME_1),.Name == columnList.TIME_1.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.NINZU_1),.Name == columnList.NINZU_1.ToString(),.Width == 60,.DataType == typeof(int) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CAR_NO_1),.Name == columnList.CAR_NO_1.ToString(),.Width == 50,.DataType == typeof(string),.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(3, ControlFormat.HankakuSuji) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.HAISYA_KEIYU_NAME_2),.Name == columnList.HAISYA_KEIYU_NAME_2.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.TIME_2),.Name == columnList.TIME_2.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.NINZU_2),.Name == columnList.NINZU_2.ToString(),.Width == 60,.DataType == typeof(int) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CAR_NO_2),.Name == columnList.CAR_NO_2.ToString(),.Width == 50,.DataType == typeof(string),.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(3, ControlFormat.HankakuSuji) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.HAISYA_KEIYU_NAME_3),.Name == columnList.HAISYA_KEIYU_NAME_3.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.TIME_3),.Name == columnList.TIME_3.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.NINZU_3),.Name == columnList.NINZU_3.ToString(),.Width == 60,.DataType == typeof(int) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CAR_NO_3),.Name == columnList.CAR_NO_3.ToString(),.Width == 50,.DataType == typeof(string),.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(3, ControlFormat.HankakuSuji) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.HAISYA_KEIYU_NAME_4),.Name == columnList.HAISYA_KEIYU_NAME_4.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.TIME_4),.Name == columnList.TIME_4.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.NINZU_4),.Name == columnList.NINZU_4.ToString(),.Width == 60,.DataType == typeof(int) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CAR_NO_4),.Name == columnList.CAR_NO_4.ToString(),.Width == 50,.DataType == typeof(string),.AllowEditing == true,.Editor == CommonUtil.makeTextBoxEx(3, ControlFormat.HankakuSuji) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.HAISYA_KEIYU_NAME_5),.Name == columnList.HAISYA_KEIYU_NAME_5.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.TIME_5),.Name == columnList.TIME_5.ToString(),.Width == 100,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.NINZU_5),.Name == columnList.NINZU_5.ToString(),.Width == 60,.DataType == typeof(int) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.CAR_NO_5),.Name == columnList.CAR_NO_5.ToString(),.Width == 50,.DataType == typeof(string) },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.EDIT_FLG_1),.Name == columnList.EDIT_FLG_1.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.EDIT_FLG_2),.Name == columnList.EDIT_FLG_2.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.EDIT_FLG_3),.Name == columnList.EDIT_FLG_3.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.EDIT_FLG_4),.Name == columnList.EDIT_FLG_4.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.SYUPT_DAY),.Name == columnList.SYUPT_DAY.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_SYUPT_DAY_1),.Name == columnList.OYA_SYUPT_DAY_1.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_CRS_CD_1),.Name == columnList.OYA_CRS_CD_1.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_GOUSYA_1),.Name == columnList.OYA_GOUSYA_1.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_SYUPT_DAY_2),.Name == columnList.OYA_SYUPT_DAY_2.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_CRS_CD_2),.Name == columnList.OYA_CRS_CD_2.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_GOUSYA_2),.Name == columnList.OYA_GOUSYA_2.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_SYUPT_DAY_3),.Name == columnList.OYA_SYUPT_DAY_3.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_CRS_CD_3),.Name == columnList.OYA_CRS_CD_3.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_GOUSYA_3),.Name == columnList.OYA_GOUSYA_3.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_SYUPT_DAY_4),.Name == columnList.OYA_SYUPT_DAY_4.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_CRS_CD_4),.Name == columnList.OYA_CRS_CD_4.ToString(),.Width == -1,.Visible == false },;
			new GridProperty() {.Caption = getEnumAttrValue(columnList.OYA_GOUSYA_4),.Name == columnList.OYA_GOUSYA_4.ToString(),.Width == -1,.Visible == false };
		};

		this.grdList.Cols.Count = colsList.Count;
		return colsList;
	}

	private void SetGridHeaderitem()
	{

		// セルの結合
		// 固定セルのマージモードを設定
		this.grdList.AllowMerging = AllowMergingEnum.Custom;

		CellRange cr = null;
		//1段目
		cr = grdList.GetCellRange(0, 0, 1, 0);
		grdList.MergedRanges.Add(cr);
		grdList[0, 0] = getEnumAttrValue(columnList.SYUPT_DAY_STR);

		cr = grdList.GetCellRange(0, 1, 1, 1);
		grdList.MergedRanges.Add(cr);
		grdList[0, 1] = getEnumAttrValue(columnList.CRS_CD);

		cr = grdList.GetCellRange(0, 2, 1, 2);
		grdList.MergedRanges.Add(cr);
		grdList[0, 2] = getEnumAttrValue(columnList.GOUSYA);

		cr = grdList.GetCellRange(0, 3, 1, 3);
		grdList.MergedRanges.Add(cr);
		grdList[0, 3] = getEnumAttrValue(columnList.CRS_NAME);

		cr = grdList.GetCellRange(0, 4, 1, 4);
		grdList.MergedRanges.Add(cr);
		grdList[0, 4] = getEnumAttrValue(columnList.CAR_NO);

		//MERGED CELL
		//配車経由地１
		cr = grdList.GetCellRange(0, 5, 0, 8);
		grdList.MergedRanges.Add(cr);
		grdList[0, 5] = "配車経由地１";
		grdList[1, 5] = getEnumAttrValue(columnList.HAISYA_KEIYU_NAME_1);
		grdList[1, 6] = getEnumAttrValue(columnList.TIME_1);
		grdList[1, 7] = getEnumAttrValue(columnList.NINZU_1);
		grdList[1, 8] = getEnumAttrValue(columnList.CAR_NO_1);

		//配車経由地 2
		cr = grdList.GetCellRange(0, 9, 0, 12);
		grdList.MergedRanges.Add(cr);
		grdList[0, 9] = "配車経由地 2";
		grdList[1, 9] = getEnumAttrValue(columnList.HAISYA_KEIYU_NAME_2);
		grdList[1, 10] = getEnumAttrValue(columnList.TIME_2);
		grdList[1, 11] = getEnumAttrValue(columnList.NINZU_2);
		grdList[1, 12] = getEnumAttrValue(columnList.CAR_NO_2);

		//配車経由地 3
		cr = grdList.GetCellRange(0, 13, 0, 16);
		grdList.MergedRanges.Add(cr);
		grdList[0, 13] = "配車経由地 3";
		grdList[1, 13] = getEnumAttrValue(columnList.HAISYA_KEIYU_NAME_3);
		grdList[1, 14] = getEnumAttrValue(columnList.TIME_3);
		grdList[1, 15] = getEnumAttrValue(columnList.NINZU_3);
		grdList[1, 16] = getEnumAttrValue(columnList.CAR_NO_3);

		//配車経由地 4
		cr = grdList.GetCellRange(0, 17, 0, 20);
		grdList.MergedRanges.Add(cr);
		grdList[0, 17] = "配車経由地 4";
		grdList[1, 17] = getEnumAttrValue(columnList.HAISYA_KEIYU_NAME_4);
		grdList[1, 18] = getEnumAttrValue(columnList.TIME_4);
		grdList[1, 19] = getEnumAttrValue(columnList.NINZU_4);
		grdList[1, 20] = getEnumAttrValue(columnList.CAR_NO_4);


		//配車経由地 5
		cr = grdList.GetCellRange(0, 21, 0, 24);
		grdList.MergedRanges.Add(cr);
		grdList[0, 21] = "配車経由地 5";
		grdList[1, 21] = getEnumAttrValue(columnList.HAISYA_KEIYU_NAME_5);
		grdList[1, 22] = getEnumAttrValue(columnList.TIME_5);
		grdList[1, 23] = getEnumAttrValue(columnList.NINZU_5);
		grdList[1, 24] = getEnumAttrValue(columnList.CAR_NO_5);

	}

	/// <summary>
	/// 更新用のデータを設定
	/// </summary>
	private S04_0801_DA.S04_00801DAUpdateParam getDataParamForUpdate(DataRow dataRowParrent, DataRow dataRow, int kaisyakeiyuNum, bool flagCheckCarNo)
	{

		S04_0801_DA.S04_00801DAUpdateParam param = new S04_0801_DA.S04_00801DAUpdateParam();
		param.SyuptDay = int.Parse(dataRowParrent[columnList.SYUPT_DAY.ToString()].ToString());
		//コースコード
		param.Crscd = dataRowParrent[columnList.CRS_CD.ToString()].ToString();
		//号車
		param.Gousya = int.Parse(dataRowParrent[columnList.GOUSYA.ToString()].ToString());

		if (flagCheckCarNo == true)
		{
			//出発日1(親)
			param.SyuptDayOya = int.Parse(dataRow[columnList.SYUPT_DAY.ToString()].ToString());
			//コースコード1(親)
			param.CrsCdOya = dataRow[columnList.CRS_CD.ToString()].ToString();
			//号車1(親)
			param.GousyaOya = int.Parse(dataRow[columnList.GOUSYA.ToString()].ToString());
		}
		else
		{
			if (kaisyakeiyuNum == 1)
			{
				//出発日1(親)
				param.SyuptDayOya = int.Parse(dataRow[columnList.OYA_SYUPT_DAY_1.ToString()].ToString());
				//コースコード1(親)
				param.CrsCdOya = dataRow[columnList.OYA_CRS_CD_1.ToString()].ToString();
				//号車1(親)
				if (!string.IsNullOrEmpty(dataRow[columnList.OYA_GOUSYA_1.ToString()].ToString()))
				{
					param.GousyaOya = int.Parse(dataRow[columnList.OYA_GOUSYA_1.ToString()].ToString());
				}
			}
			else if (kaisyakeiyuNum == 2)
			{
				//出発日2(親)
				param.SyuptDayOya = int.Parse(dataRow[columnList.OYA_SYUPT_DAY_2.ToString()].ToString());
				//コースコード2(親)
				param.CrsCdOya = dataRow[columnList.OYA_CRS_CD_2.ToString()].ToString();
				//号車2(親)
				if (!string.IsNullOrEmpty(dataRow[columnList.OYA_GOUSYA_2.ToString()].ToString()))
				{
					param.GousyaOya = int.Parse(dataRow[columnList.OYA_GOUSYA_2.ToString()].ToString());
				}
			}
			else if (kaisyakeiyuNum == 3)
			{
				//出発日3(親)
				param.SyuptDayOya = int.Parse(dataRow[columnList.OYA_SYUPT_DAY_3.ToString()].ToString());
				//コースコード3(親)
				param.CrsCdOya = dataRow[columnList.OYA_CRS_CD_3.ToString()].ToString();
				//号車3(親)
				if (!string.IsNullOrEmpty(dataRow[columnList.OYA_GOUSYA_3.ToString()].ToString()))
				{
					param.GousyaOya = int.Parse(dataRow[columnList.OYA_GOUSYA_3.ToString()].ToString());
				}
			}
			else if (kaisyakeiyuNum == 4)
			{
				//出発日4(親)
				param.SyuptDayOya = int.Parse(dataRow[columnList.OYA_SYUPT_DAY_4.ToString()].ToString());
				//コースコード4(親)
				param.CrsCdOya = dataRow[columnList.OYA_CRS_CD_4.ToString()].ToString();
				//号車4(親)
				if (!string.IsNullOrEmpty(dataRow[columnList.OYA_GOUSYA_4.ToString()].ToString()))
				{
					param.GousyaOya = int.Parse(dataRow[columnList.OYA_GOUSYA_4.ToString()].ToString());
				}
			}
		}

		//配車経由番号
		param.HaisyakeiyuNo = kaisyakeiyuNum;
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
		return param;
	}
	#endregion

	#region Privateメソッド(イベント)
	#endregion

}