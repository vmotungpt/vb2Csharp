using System.ComponentModel;


/// <summary>
/// コース一覧照会(リマークス／メモ一覧)
/// <remarks>
/// Author:2018/10/31//QuangTD
/// </remarks>
/// </summary>
public class S02_0501 : PT11, iPT11
{

	#region  定数／変数宣言

	//条件GroupBoxのTop座標
	public const int TopGbxCondition = 41;
	// 条件GroupBoxのマージン
	public const int MarginGbxCondition = 6;
	private const string hyphenSymbol = "-";
	private const string dashSymbol = "_";
	private const string colonSymbol = ":";
	private const string timeDefaultString = "__:__";
	private const string asteriskSymbol = "*";
	private const string yyyyMMddFormat = "yyyyMMdd";
	private const string hhmmFormat = "HH:mm";

	/// <summary>
	/// 表示上限件数
	/// </summary>
	private const int MaxDisplayCount = 100;

	#endregion

	#region イベント
	#region グリッド関連
	/// <summary>
	/// ソート処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdList_AfterSort(object sender, C1.Win.C1FlexGrid.SortColEventArgs e)
	{
		//一覧グリッドイベント時
		clickedMainGrid();
	}

	/// <summary>
	/// 行選択時のイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void grdList_RowColChange(object sender, System.EventArgs e)
	{
		//一覧グリッドイベント時
		clickedMainGrid();
	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdList_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		FlexGridEx grd = TryCast(sender, FlexGridEx);
		if (ReferenceEquals(grd, null))
		{
			return;
		}

		//データ件数を表示(ヘッダー行分マイナス1)
		string formatedCount = System.Convert.ToString((grd.Rows.Count - 2).ToString("N0").PadLeft(6));
		this.lblLengthGrd.Text = formatedCount + "件";
	}

	#endregion

	#region フォーム

	/// <summary>
	/// フォームキーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void PT11_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			base.btnCom_Click(this.btnSearch, e);
			btnSearch.Select();
		}
		else
		{
			return;
		}

	}
	#endregion

	#region [変更チェック](差分チェック)
	/// <summary>
	/// [変更チェック](差分チェック)
	/// </summary>
	/// <returns></returns>
	protected override bool checkDifference()
	{
		return false;
	}
	#endregion

	#endregion

	#region PT11オーバーライド(基本的には変えない)

	#region 初期化処理
	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{
		//背景色初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);
		object currentTime = CommonDateUtil.getSystemTime();
		//背景色初期化はここ
		CommonUtil.Control_Init(gbxCondition.Controls);
		dtmSyuptDayFromTo.FromDateText = currentTime;
		dtmSyuptDayFromTo.ToDateText = currentTime;
		dtmSyuptTime.Text = string.Empty;
		CodeControlEx1.CodeText = string.Empty;
		CodeControlEx1.ValueText = string.Empty;
		ucoNoribaCd.CodeText = string.Empty;
		ucoNoribaCd.ValueText = string.Empty;
		grdCrsListInquiry.DataSource = createDataTable();
		dtmSyuptDayFromTo.Select();

		//日本語／外国語チェックボックス
		if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
		{
			//ユーザーが国際事業部の場合は外国語
			chkGaikokugo.Checked = true;
			chkJapanese.Checked = false;
		}
		else
		{
			//それ以外の場合は日本語をONに設定
			chkJapanese.Checked = true;
			chkGaikokugo.Checked = false;
		}

	}

	#endregion

	#region チェック系
	/// <summary>
	/// 検索入力項目チェック
	/// </summary>
	protected override bool checkSearchItems()
	{
		return CheckSearch();
	}

	/// <summary>
	/// 更新入力項目チェック
	/// </summary>
	protected override bool checkUpdateItems()
	{
		return CheckUpdate();
	}

	/// <summary>
	/// 更新入力項目チェック
	/// </summary>
	protected override bool checkInsertItems()
	{
		return CheckInsert();
	}
	#endregion

	#region 固有初期処理
	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern() // 画面パターン毎の固有初期処理
	{
		base.initScreenPerttern();
		//フォーカスセット
		setSeFirsttDisplayData();
	}
	#endregion

	#endregion

	#region Grid、データ関連
	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void reloadGrid()
	{
		object dt = createDataTable();
		foreach ( in this.SearchResultGridData.Rows)
		{
			DataRow DataRow = (DataRow)row;
			object displayRow = dt.NewRow();
			if (!Information.IsDBNull(DataRow("SYUPT_DAY")))
			{
				displayRow["SYUPT_DAY"] = DataRow("SYUPT_DAY");
			}
			else
			{
				displayRow["SYUPT_DAY"] = string.Empty;
			}
			if (!Information.IsDBNull(DataRow("YOBI_CD")))
			{
				displayRow["YOBI_CD"] = DataRow("YOBI_CD");
			}
			else
			{
				displayRow["YOBI_CD"] = null;
			}
			string syuptDay = "";
			if (!Information.IsDBNull(DataRow("SYUPT_DAY")))
			{
				syuptDay = System.Convert.ToString(DataRow("SYUPT_DAY"));
			}
			else
			{
				syuptDay = string.Empty;
			}

			int yobiCd = 0;
			if (!Information.IsDBNull(DataRow("YOBI_CD")))
			{
				yobiCd = System.Convert.ToInt32(DataRow("YOBI_CD"));
			}
			else
			{
				yobiCd = -1;
			}

			System.String yobiValue = string.Empty;
			if (syuptDay.Length == 8 && syuptDay.First != hyphenSymbol)
			{
				System.Char month = syuptDay.Substring(4, 2);
				System.Char day = syuptDay.Substring(6, 2);
				if (yobiCd == FixedCd.YobiCd.Sunday)
				{
					yobiValue = "日";
				}
				else if (yobiCd == FixedCd.YobiCd.Monday)
				{
					yobiValue = "月";
				}
				else if (yobiCd == FixedCd.YobiCd.Tuesday)
				{
					yobiValue = "火";
				}
				else if (yobiCd == FixedCd.YobiCd.Wednesday)
				{
					yobiValue = "水";
				}
				else if (yobiCd == FixedCd.YobiCd.Thursday)
				{
					yobiValue = "木";
				}
				else if (yobiCd == FixedCd.YobiCd.Friday)
				{
					yobiValue = "金";
				}
				else if (yobiCd == FixedCd.YobiCd.Saturday)
				{
					yobiValue = "土";
				}
				else
				{
				}
				displayRow["COL_SYUPT_DAY"] = month + '/' + System.Convert.ToString(day) + '(' + yobiValue + ")";
			}

			string situation = string.Empty;
			if (string.IsNullOrWhiteSpace(System.Convert.ToString(DataRow("YOYAKU_STOP_FLG"))) == false)
			{
				situation = "予約受付停止";
			}
			else if (string.Equals(DataRow("TEJIMAI_KBN"), FixedCdYoyaku.TejimaiKbn.Y) == true)
			{
				situation = "手仕舞済";
			}
			else if (string.Equals(DataRow("UNKYU_KBN"), FixedCd.UnkyuKbn.Unkyu) == true && string.Equals(DataRow("CRS_KIND"), (System.Convert.ToInt32(FixedCd.CrsKindType.hatoBusTeiki)).ToString()))
			{
				situation = "運休";
			}
			else if (string.Equals(DataRow("UNKYU_KBN"), FixedCd.UnkyuKbn.Unkyu) &&)
			{
				(string.Equals(DataRow("CRS_KIND"), System.Convert.ToInt32(FixedCd.CrsKindType.higaeri).ToString()) ||;
				string.Equals(DataRow("CRS_KIND"), (System.Convert.ToInt32(FixedCd.CrsKindType.stay)).ToString()) ||;
				string.Equals(DataRow("CRS_KIND"), (System.Convert.ToInt32(FixedCd.CrsKindType.rcourse)).ToString()));
				situation = "催行中止";
			}

			displayRow["COL_SITUATION"] = situation;
			if (!Information.IsDBNull(DataRow("CRS_CD")))
			{
				displayRow["COL_CRS_CD"] = DataRow("CRS_CD");
			}
			else
			{
				displayRow["COL_CRS_CD"] = string.Empty;
			}
			if (!Information.IsDBNull(DataRow("CRS_NAME")))
			{
				displayRow["COL_CRS_NAME"] = DataRow("CRS_NAME");
			}
			else
			{
				displayRow["COL_CRS_NAME"] = string.Empty;
			}
			if (!Information.IsDBNull(DataRow("PLACE_NAME_1")))
			{
				displayRow["COL_JYOSYATI"] = DataRow("PLACE_NAME_1");
			}
			else
			{
				displayRow["COL_JYOSYATI"] = string.Empty;
			}

			//経由データの場合は乗り場に「*」を付与
			//If Not IsDBNull(DataRow("HAISYA_KEIYU_CD_2")) Then
			//    displayRow("COL_JYOSYATI") = CType(displayRow("COL_JYOSYATI"), String) & asteriskSymbol
			//End If

			if (!Information.IsDBNull(DataRow("SYUPT_TIME_1")))
			{
				displayRow["COL_TIME"] = formatTime(System.Convert.ToString(DataRow("SYUPT_TIME_1")));
			}
			else
			{
				displayRow["COL_TIME"] = string.Empty;
			}
			displayRow["COL_GOUSYA"] = DataRow("GOUSYA");

			if (!Information.IsDBNull(DataRow("KUSEKI_NUM_TEISEKI")) && !string.IsNullOrEmpty(System.Convert.ToString(DataRow("KUSEKI_NUM_TEISEKI"))))
			{
				displayRow["COL_KUSEKI"] = DataRow("KUSEKI_NUM_TEISEKI");
			}
			else
			{
				displayRow["COL_KUSEKI"] = "0";
			}
			if (!Information.IsDBNull(DataRow("KUSEKI_NUM_SUB_SEAT")) && !string.IsNullOrEmpty(System.Convert.ToString(DataRow("KUSEKI_NUM_SUB_SEAT"))))
			{
				displayRow["COL_KUSEKI_SUB_SEAT"] = DataRow("KUSEKI_NUM_SUB_SEAT");
			}
			else
			{
				displayRow["COL_KUSEKI_SUB_SEAT"] = "0";
			}

			dt.Rows.Add(displayRow);
			//colJyosyaTi
			for (index = 2; index <= 5; index++)
			{
				string nameColumnPlace = "HAISYA_KEIYU_CD_" + index;
				if (Information.IsDBNull(DataRow(nameColumnPlace)))
				{
					break;
				}
				object newRow = dt.NewRow();
				newRow["COL_SYUPT_DAY"] = displayRow["COL_SYUPT_DAY"];
				newRow["SYUPT_DAY"] = displayRow["SYUPT_DAY"];
				newRow["YOBI_CD"] = displayRow["YOBI_CD"];
				newRow["COL_SITUATION"] = displayRow["COL_SITUATION"];
				newRow["COL_CRS_CD"] = displayRow["COL_CRS_CD"];
				newRow["COL_CRS_NAME"] = displayRow["COL_CRS_NAME"];
				if (!Information.IsDBNull(DataRow("PLACE_NAME_" + index)))
				{
					newRow["COL_JYOSYATI"] = System.Convert.ToString(DataRow("PLACE_NAME_" + index)) + asteriskSymbol;
				}
				else
				{
					newRow["COL_JYOSYATI"] = string.Empty;
				}
				if (!Information.IsDBNull(DataRow("SYUPT_TIME_" + index)))
				{
					newRow["COL_TIME"] = formatTime(System.Convert.ToString(DataRow("SYUPT_TIME_" + index)));
				}
				else
				{
					newRow["COL_TIME"] = string.Empty;
				}
				newRow["COL_GOUSYA"] = displayRow["COL_GOUSYA"];
				newRow["COL_KUSEKI"] = displayRow["COL_KUSEKI"];
				newRow["COL_KUSEKI_SUB_SEAT"] = displayRow["COL_KUSEKI_SUB_SEAT"];

				//経由データの場合は乗り場に「*」を付与
				if (index < 5 && !Information.IsDBNull(DataRow("HAISYA_KEIYU_CD_" + (index + 1))))
				{
					newRow["COL_JYOSYATI"] = System.Convert.ToString(newRow["COL_JYOSYATI"]) + asteriskSymbol;
				}
				dt.Rows.Add(newRow);
			}
		}
		this.grdCrsListInquiry.HostedControlArr = new ArrayList();
		if (dt.Rows.Count > 100)
		{
			createFactoryMsg().messageDisp("E90_027", "検索結果が");
			// 取得結果をグリッドへ設定(100件のみ)
			dt = dt.AsEnumerable.Take(MaxDisplayCount).CopyToDataTable;
			this.grdCrsListInquiry.DataSource = dt;
		}
		else
		{
			//取得結果をグリッドへ設定
			this.grdCrsListInquiry.DataSource = dt;
		}
	}


	#endregion

	#region 実装用メソッド(画面毎に変更)

	#region 初期処理
	/// <summary>
	/// 初期処理(必須画面個別実装)
	/// </summary>
	public void setSeFirsttDisplayData()
	{
		//グリッドの設定
		setcolgrdCrsListInquiry();
		setgrdCrsListInquiry();
		CommonMstUtil.setComboBox(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.language), cmbLanguage, true);
		CommonMstUtil.setComboBox(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.categoryMaster), cmbCategory, true);
	}

	#endregion

	#region グリッド設定

	#endregion

	#region エンティティ操作系
	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">添乗員一括入力エンティティ</param>
	public void DisplayDataToEntity(ref object ent)
	{
	}

	/// <summary>
	/// エンティティから画面に設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">行定表連結記号エンティティ</param>
	public void EntityDataToDisplay(ref object ent)
	{
	}

	/// <summary>
	/// DataGridからエンティティ(前回値)に設定する処理(必須画面個別実装)
	/// ※DataGrid上の1レコードから関連するデータも取得する。Keyがない場合などは未対応
	/// </summary>
	/// <param name="pDataRow">GridのDataBind(DataTable)の選択行</param>
	/// <remarks></remarks>
	public void OldDataToEntity(DataRow pDataRow)
	{
	}
	#endregion

	#region チェック系

	/// <summary>
	/// 検索処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckSearch()
	{
		//背景色初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		//各画面毎】チェック要件
		//(1) 必須チェック
		if (CommonUtil.checkHissuError(gbxCondition.Controls))
		{
			this.dtmSyuptDayFromTo.Focus();
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			return false;
		}

		//出発日Fromが未入力　かつ　出発日Toが入力済みの場合 ： 出発日Fromに出発日Toの日付を設定する
		if (string.IsNullOrEmpty(System.Convert.ToString(this.dtmSyuptDayFromTo.FromDateText.ToString().Trim())))
		{
			dtmSyuptDayFromTo.FromDateText = dtmSyuptDayFromTo.ToDateText;
		}

		//出発日Fromが入力済み　かつ　出発日Toが未入力の場合 ： 出発日Toに出発日Fromの日付を設定する
		if (string.IsNullOrEmpty(System.Convert.ToString(this.dtmSyuptDayFromTo.ToDateText.ToString().Trim())))
		{
			dtmSyuptDayFromTo.ToDateText = dtmSyuptDayFromTo.FromDateText;
		}

		//(2) 相関チェック
		//No.1 日付の逆転
		if (this.dtmSyuptDayFromTo.FromDateText > this.dtmSyuptDayFromTo.ToDateText)
		{
			dtmSyuptDayFromTo.ExistErrorForFromDate = true;
			this.dtmSyuptDayFromTo.Focus();
			createFactoryMsg().messageDisp("E90_006", "「逆転した日付」", "「指定」");
			return false;
		}

		//No2. 日付指定の範囲チェック
		if (this.dtmSyuptDayFromTo.FromDateText.GetValueOrDefault.AddMonths(3) < this.dtmSyuptDayFromTo.ToDateText)
		{
			dtmSyuptDayFromTo.ExistErrorForFromDate = true;
			this.dtmSyuptDayFromTo.Focus();
			createFactoryMsg().messageDisp("E90_054", "3ヶ月");
			return false;
		}

		//No.3 マスタ存在チェック
		if (!string.IsNullOrEmpty(System.Convert.ToString(ucoNoribaCd.CodeText)))
		{
			MPlace_DA dataAccess = new MPlace_DA();
			Hashtable paramSearch = new Hashtable();
			paramSearch.Add("PlaceCd", ucoNoribaCd.CodeText);
			object result = dataAccess.accessPlaceMaster(MPlace_DA.accessType.getMPlcaeByPrimaryKey, paramSearch);
			if (result.Rows.Count == 0)
			{
				ucoNoribaCd.ExistError = true;
				createFactoryMsg().messageDisp("E02_002");
				ucoNoribaCd.setFocus();
				return false;
			}
			else
			{
				ucoNoribaCd.ExistError = false;
			}
		}

		return true;
	}

	/// <summary>
	/// 登録処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckInsert()
	{
		return true;
	}

	/// <summary>
	/// 更新処理前のチェック処理(必須画面個別実装)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckUpdate()
	{
		return true;
	}

	#region チェック処理(Private)
	/// <summary>
	/// 必須入力項目エラーチェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	public bool isExistHissuError()
	{
		bool returnValue = false;
		return returnValue;
	}

	#endregion
	#endregion
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
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void FlexGridEx1_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		//データ件数を表示(ヘッダー行分マイナス1)
		string formatedCount = System.Convert.ToString((this.grdCrsListInquiry.Rows.Count - 2).ToString("N0").PadLeft(6));
		this.lblLengthGrd.Text = formatedCount + "件";
	}

	/// <summary>
	/// 表示列、登録列ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void grdCrsListInquiry_CellButtonClick(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
	{
		object sortColumn = grdCrsListInquiry.SortColumn;
		string nameColSort = null;
		if (sortColumn IsNot null)
		{
			nameColSort = System.Convert.ToString(sortColumn.Name);
			if (sortColumn.Sort == C1.Win.C1FlexGrid.SortFlags.Ascending)
			{
				((DataTable)grdCrsListInquiry.DataSource).DefaultView.Sort = nameColSort + " ASC";
			}
			else
			{
				((DataTable)grdCrsListInquiry.DataSource).DefaultView.Sort = nameColSort + " DESC";
			}
			grdCrsListInquiry.DataSource = ((DataTable)grdCrsListInquiry.DataSource).DefaultView.ToTable;
		}

		if (e.Col == 1)
		{
			int rowIndex = System.Convert.ToInt32(e.Row - 2);
			object dataRow = ((DataTable)grdCrsListInquiry.DataSource).Rows(rowIndex);
			ReceiptEntitiy deliveryEntity = new ReceiptEntitiy();
			deliveryEntity.txtCrsCd = System.Convert.ToString(dataRow["COL_CRS_CD"]);
			deliveryEntity.txtCrsNm = System.Convert.ToString(dataRow["COL_CRS_NAME"]);
			deliveryEntity.dtmSyuptDay = DateTime.ParseExact(System.Convert.ToString(dataRow["SYUPT_DAY"]), "yyyyMMdd", null);
			deliveryEntity.txtGousya = System.Convert.ToString(dataRow["COL_GOUSYA"]);
			deliveryEntity.txtDaycode = System.Convert.ToString(dataRow["YOBI_CD"]);
			using (S02_0503 Form = new S02_0503(deliveryEntity))
			{
				Form.ShowDialog();
			}

		}
		else if (e.Col == 2)
		{
			int rowIndex = System.Convert.ToInt32(e.Row - 2);
			object dataRow = ((DataTable)grdCrsListInquiry.DataSource).Rows(rowIndex);
			string syuptDay = System.Convert.ToString(dataRow["SYUPT_DAY"]);
			string t = System.Convert.ToString(dataRow["COL_TIME"]);
			string currentDate = System.Convert.ToString(CommonDateUtil.getSystemTime().ToString(yyyyMMddFormat));

			if (string.Compare(syuptDay, currentDate) < 0)
			{
				createFactoryMsg().messageDisp("E90_067", "リマークス情報を登録");
				return;
			}
			else if (syuptDay == currentDate)
			{
				if (CommonDateUtil.getSystemTime().ToString(hhmmFormat) > t)
				{
					createFactoryMsg().messageDisp("E90_067", "リマークス情報を登録");
					return;
				}
			}

			Delivery_S02_0502Entity deliveryEntity = new Delivery_S02_0502Entity() {;
			.CourseCode = System.Convert.ToString(dataRow["COL_CRS_CD"]),;
			.DepartureDate = System.Convert.ToString(dataRow["SYUPT_DAY"]),;
			.DepartureTime = System.Convert.ToString(dataRow["COL_TIME"]),;
			.Car = System.Convert.ToString(dataRow["COL_GOUSYA"]),;
			.BranchMumb = string.Empty,;
			.EditMode = (System.Convert.ToInt32(S02_0502.TypeEdit.Register)).ToString();
		};
		using (S02_0502 form = new S02_0502(deliveryEntity))
		{
			form.ShowDialog();
			object researchFlg = form.researchFlg;
			if (researchFlg == true)
			{
				btnF8_ClickOrgProc();
			}
		}

	}
}

#endregion

#region メソッド

/// <summary>
/// フッタボタンの制御(表示\[活性]／非表示[非活性])
/// </summary>
protected override void initFooterButtonControl()
{
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
	this.setFormId = "S02_0501";
	this.setTitle = "コース一覧照会（リマークス/メモ一覧）";
	btnClear.Click += btnCom_Click;
	btnSearch.Click += btnCom_Click;
}

#region コース一覧照会グリッドの設定
/// <summary>
/// 現払登録対象コース照会グリッドの設定
/// </summary>
private void setcolgrdCrsListInquiry()
{

	// 行ヘッダを作成。
	grdCrsListInquiry.Styles.Normal.WordWrap = true;
	grdCrsListInquiry.Cols.Count = 12; //gridの行数
	grdCrsListInquiry.Rows.Fixed = 2; // 行固定
	grdCrsListInquiry.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;
	grdCrsListInquiry.Rows(0).AllowMerging = true;
	grdCrsListInquiry.Cols(0).AllowMerging = true;
	C1.Win.C1FlexGrid.CellRange cr = null;
	// メモ
	cr = grdCrsListInquiry.GetCellRange(0, 1, 0, 2);
	cr.Data = "メモ";
	grdCrsListInquiry.MergedRanges.Add(cr);
	grdCrsListInquiry[1, 1] = "表示";
	grdCrsListInquiry[1, 2] = "登録";
	// 日付
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
	//乗車地
	cr = grdCrsListInquiry.GetCellRange(0, 7, 1, 7);
	grdCrsListInquiry.MergedRanges.Add(cr);
	//時間
	cr = grdCrsListInquiry.GetCellRange(0, 8, 1, 8);
	grdCrsListInquiry.MergedRanges.Add(cr);
	//号車
	cr = grdCrsListInquiry.GetCellRange(0, 9, 1, 9);
	grdCrsListInquiry.MergedRanges.Add(cr);
	//空席
	cr = grdCrsListInquiry.GetCellRange(0, 10, 1, 10);
	grdCrsListInquiry.MergedRanges.Add(cr);
	//空席
	cr = grdCrsListInquiry.GetCellRange(0, 11, 1, 11);
	grdCrsListInquiry.MergedRanges.Add(cr);
	grdCrsListInquiry.Cols(0).AllowEditing = false;
	grdCrsListInquiry.Cols(1).AllowEditing = true;
	grdCrsListInquiry.Cols(2).AllowEditing = true;
	for (int i = 3; i <= 11; i++)
	{
		grdCrsListInquiry.Cols(i).AllowEditing = false;
	}

}


/// <summary>
/// コース一覧照会(メモ一覧)モック用グリッド表示値の設定
/// </summary>
private void setgrdCrsListInquiry()
{
	grdCrsListInquiry.Styles.Normal.WordWrap = true;
	grdCrsListInquiry.Rows(0).Height = 40;
	grdCrsListInquiry.DataSource = createDataTable();
}

/// <summary>
/// 構造テーブルを作成する
/// </summary>
/// <returns></returns>
private DataTable createDataTable()
{
	DataTable dt = new DataTable();
	//列作成
	dt.Columns.Add("COL_MEMO_DISPLAY"); //予約
	dt.Columns.Add("COL_MEMO_ENTRY"); //予約
	dt.Columns.Add("COL_SYUPT_DAY"); //日付
	dt.Columns.Add("COL_CRS_CD"); //コースコード
	dt.Columns.Add("COL_CRS_NAME"); //コース名
	dt.Columns.Add("COL_SITUATION"); //状況
	dt.Columns.Add("COL_JYOSYATI"); //乗車地
	dt.Columns.Add("COL_TIME"); //時間
	dt.Columns.Add("COL_GOUSYA"); //号車
	dt.Columns.Add("COL_KUSEKI"); //空席
	dt.Columns.Add("COL_KUSEKI_SUB_SEAT"); //空席（補助席）
	dt.Columns.Add("SYUPT_DAY"); //日付
	dt.Columns.Add("YOBI_CD"); //日付
	return dt;
}

/// <summary>
/// フォーマット時間hh：mm
/// </summary>
/// <param name="time">フォーマットhhmmで時間</param>
/// <returns></returns>
private string formatTime(string time)
{

	string tmpTime = time.Trim();

	if (time.Length < 4)
	{
		tmpTime = tmpTime.PadLeft(4, '0');
	}

	if (tmpTime.Length == 4 && tmpTime.First != hyphenSymbol)
	{
		System.Char hours = tmpTime.Substring(0, 2);
		System.Char minutes = tmpTime.Substring(2, 2);
		return hours + colonSymbol + System.Convert.ToString(minutes);
	}
	return string.Empty;
}

/// <summary>
/// 対象マスタのデータ取得
/// </summary>
protected override DataTable getMstData()
{
	Hashtable paramList = new Hashtable();
	//【各画面毎】検索条件用マスタ値取得
	//(1) 特定のコースコードのデータを除外する為、マスタより除外するコースコードを取得する。
	CrsControlInfo_DA crsControlInfo = new CrsControlInfo_DA();
	object exceptKey = crsControlInfo.accessCrsControlInfo(CrsControlInfo_DA.accessType.getCrsControlInfo);
	foreach ( in exceptKey.Rows)
	{
		DataRow row = (DataRow)dt;
		if (row["KEY_2"].ToString().Trim() == "01")
		{
			paramList.Add("EXCEPT_CRS_CD_1", row["KEY_3"]);
		}
		if (row["KEY_2"].ToString().Trim() == "02")
		{
			paramList.Add("EXCEPT_CRS_CD_2", row["KEY_3"]);
		}
	}
	ArrayList listCrsKbn = new ArrayList();
	if (chkTeikiNoon.Checked)
	{
		listCrsKbn.Add(1);
	}
	if (chkTeikiNight.Checked)
	{
		listCrsKbn.Add(2);
	}
	if (chkTeikiKogai.Checked)
	{
		listCrsKbn.Add(3);
	}
	if (chkKikakuDayTrip.Checked)
	{
		listCrsKbn.Add(4);
	}
	if (chkKikakuStay.Checked)
	{
		listCrsKbn.Add(5);
	}
	if (chkKikakuTonaiR.Checked)
	{
		listCrsKbn.Add(6);
	}
	if (chkCapital.Checked)
	{
		listCrsKbn.Add(7);
	}

	if (listCrsKbn.Count != 0)
	{
		paramList.Add("CRS_KIND", listCrsKbn);
	}
	paramList.Add("CRS_CD", CodeControlEx1.CodeText);

	//コースコード、コース区分どちらも指定しない場合は、キャピタル以外のコース区分で検索を行う。
	if (listCrsKbn.Count == 0 && string.IsNullOrEmpty(System.Convert.ToString(CodeControlEx1.CodeText)))
	{
		for (int i = 1; i <= 6; i++)
		{
			listCrsKbn.Add(i);
		}
	}

	paramList.Add("SYUPT_DAY_FROM", (System.Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText)).ToString(yyyyMMddFormat));
	paramList.Add("SYUPT_DAY_TO", (System.Convert.ToDateTime(dtmSyuptDayFromTo.ToDateText)).ToString(yyyyMMddFormat));
	paramList.Add("GUIDE_GENGO", cmbLanguage.SelectedValue.ToString().Trim());
	paramList.Add("HAISYA_KEIYU_CD", ucoNoribaCd.CodeText);

	object syuptTime = dtmSyuptTime.Text;
	if ((string)syuptTime != timeDefaultString)
	{
		paramList.Add("SYUPT_TIME", syuptTime.Replace(dashSymbol, "0").Replace(colonSymbol, string.Empty));
	}

	ArrayList listJapaneseGaikokugo = new ArrayList();
	if (chkGaikokugo.Checked)
	{
		listJapaneseGaikokugo.Add(HoujinGaikyakuKbnType.Gaikyaku);
	}
	if (chkJapanese.Checked)
	{
		listJapaneseGaikokugo.Add(HoujinGaikyakuKbnType.Houjin);
	}
	if (listJapaneseGaikokugo.Count != 0)
	{
		paramList.Add("HOUJIN_GAIKYAKU_KBN", listJapaneseGaikokugo);
	}

	ArrayList listYobiCd = new ArrayList();
	if (chkSun.Checked)
	{
		listYobiCd.Add(FixedCd.YobiCd.Sunday);
	}
	if (chkMon.Checked)
	{
		listYobiCd.Add(FixedCd.YobiCd.Monday);
	}
	if (chkTue.Checked)
	{
		listYobiCd.Add(FixedCd.YobiCd.Tuesday);
	}
	if (chkWed.Checked)
	{
		listYobiCd.Add(FixedCd.YobiCd.Wednesday);
	}
	if (chkThu.Checked)
	{
		listYobiCd.Add(FixedCd.YobiCd.Thursday);
	}
	if (chkFri.Checked)
	{
		listYobiCd.Add(FixedCd.YobiCd.Friday);
	}
	if (chkSat.Checked)
	{
		listYobiCd.Add(FixedCd.YobiCd.Saturday);
	}

	if (listYobiCd.Count != 0 && listYobiCd.Count != 7)
	{
		paramList.Add("YOBI_CD", listYobiCd);
	}
	paramList.Add("KUSEKI_NUM_TEISEKI", txtKusekiNum.Text);

	if (chkYoyakuKanouOnly.Checked)
	{
		paramList.Add("YOYAKU_STOP_FLG", "YOYAKU_STOP_FLG");
	}
	if (chkStay1NinSanka.Checked)
	{
		paramList.Add("ONE_SANKA_FLG", "ONE_SANKA_FLG");
	}
	paramList.Add("CATEGORY_CD", cmbCategory.SelectedValue.ToString().Trim());
	try
	{
		CrsLedgerBasic_DA dataAccess = new CrsLedgerBasic_DA();
		object returnData = dataAccess.accessCrsLedgerBasic(CrsLedgerBasic_DA.accessType.getCrsLedgerBasic, paramList);
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, this.Name);
		return returnData;
	}
	catch (Exception ex)
	{
		createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
		throw;
	}
}

#endregion
#endregion
}