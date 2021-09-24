using C1.Win.C1FlexGrid;
using System.ComponentModel;
using Hatobus.ReservationManagementSystem.Master;


/// <summary>
/// コース台帳一括修正（カテゴリ）
/// カテゴリ１～４を変更する
/// </summary>
public class S03_0203 : PT14, iPT14
{

	#region  定数／変数宣言

	private DataTable selectOldData; //検索後のデータを保持
	public ResearchData PrmData //データ格納クラス
	{
		private string Kahi = "Y"; //グリッドの変更可否区分
	private string UpDateKbn = "Y"; //グリッドの更新区分


	#endregion

	#region 列挙
	/// <summary>
	/// キー項目
	/// </summary>
	/// <remarks></remarks>
	public sealed class CrsLeaderKeyValues
	{
		public string CrsCd;
		public string SyuPtDay;
		public int GouSya;
	}

	/// <summary>
	/// カラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum Category_Koumoku : int
	{
		[Value("出発日")]
		syuptday = 1,
		[Value("曜日")]
		yobicd,
		[Value("乗車地")]
		haisyakeiyucd1,
		[Value("出発時間")]
		syupttime1,
		[Value("号車")]
		gousya,
		[Value("運休")]
		unkyukbn,
		[Value("催行")]
		saikoukakuteikbn,
		[Value("カテゴリー1")]
		categorycd1,
		[Value("カテゴリー2")]
		categorycd2,
		[Value("カテゴリー3")]
		categorycd3,
		[Value("カテゴリー4")]
		categorycd4,
		[Value("使用中フラグ")]
		usingflg,
		[Value("変更可否")]
		henkoukahikbn,
		[Value("更新区分")]
		updatekbn
	}

	#endregion

	#region イベント

	#region グリッド関連


	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void FlexGridEx1_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		//データ件数を表示
		ClientCommonKyushuUtil.setGridCount(grdList, lblLengthGrd);
	}

	/// <summary>
	/// ソート処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdList_BeforeSort(object sender, SortColEventArgs e)
	{
		// ヘッダーのソートを無効
		e.Cancel = true;
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
		ClickedMainGrid();
	}

	#endregion

	#endregion

	#region PT14オーバーライド

	#region 初期化処理
	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{

	}

	/// <summary>
	/// 更新対象エリアの項目初期化
	/// </summary>
	protected override void initUpdateAreaItems()
	{

	}
	#endregion

	#region 変更確認

	/// <summary>
	/// 差分チェック
	/// </summary>
	protected override bool checkDifference()
	{

		//入力差分チェック
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CheckCategoryUpdate(i) == true)
			{
				return true;
			}
		}
		return false;
	}
	#endregion

	#region 入力変更確認

	/// <summary>
	/// 入力差分チェック
	/// </summary>
	protected override int checkBasicDifference()
	{

		//入力差分チェック
		return getAllRowData();
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
	#endregion

	#region 固有初期処理

	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{

		//ベースフォームの初期化処理
		base.initScreenPerttern();

		//件数0件設定
		lblLengthGrd.Text = "0件";

		//グリッドの初期設定
		setSeFirsttDisplayData();
	}
	#endregion

	#region Grid、データ関連

	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void reloadGrid()
	{

		//コース台帳一括修正から受取った値を表示
		this.dtmSyuptDayFromTo.FromDateText = PrmData.DepartureDayFrom;
		this.dtmSyuptDayFromTo.ToDateText = PrmData.DepartureDayTo;
		this.txtCrsCd.Text = PrmData.CrsCd_Hedder;
		this.txtCrsName.Text = PrmData.CrsName_Hedder;

		//取得結果の確認
		if (base.SearchResultGridData.Rows.Count > 0)
		{
			//取得結果をグリッドへ設定
			this.grdList.DataSource = base.SearchResultGridData;
		}
		else
		{
			return;
		}

		//検索時のグリッドを差分用に退避
		selectOldData = null;
		selectOldData = ((DataTable)this.grdList.DataSource).Copy;

		//過去日付チェック
		this.CheckPastday();

		//使用中フラグチェック
		//Me.CheckUsingFlg()
		this.isUseCrsInfo();

		//フォーカス設定
		this.grdList.Select();

	}

	/// <summary>
	/// 選択行のデータを取得
	/// </summary>
	/// <remarks></remarks>
	protected override void getSelectedRowData()
	{

		//選択された行データ
		DataRow[] selectData = null;
		//問合せ文字列
		string whereString = string.Empty;

		//更新対象項目の初期化
		initUpdateAreaItems();

		//問合せ対象データ取得
		selectData = base.SearchResultGridData.Select(whereString);

		if (selectData.Length > 0)
		{
		}
		else
		{
			//更新対象エリアの項目初期化
			initUpdateAreaItems();
		}
	}

	/// <summary>
	/// 全行のデータを取得
	/// </summary>
	/// <remarks></remarks>
	protected override int getAllRowData()
	{

		int returnValue = 0;
		string msgParam = "";
		bool differenceFlg = false;

		//グリッドの背景色をクリア
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (this.grdList.Rows(i).AllowEditing == true)
			{
				this.grdList.GetCellRange(i, Category_Koumoku.categorycd1).StyleNew.BackColor = BackColorType.Standard;
				this.grdList.GetCellRange(i, Category_Koumoku.categorycd2).StyleNew.BackColor = BackColorType.Standard;
				this.grdList.GetCellRange(i, Category_Koumoku.categorycd3).StyleNew.BackColor = BackColorType.Standard;
				this.grdList.GetCellRange(i, Category_Koumoku.categorycd4).StyleNew.BackColor = BackColorType.Standard;
			}
		}

		//入力差分チェック
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CheckCategoryUpdate(i) == true)
			{
				differenceFlg = true;
			}
		}

		if (differenceFlg == false)
		{
			//入力に差分がない場合、エラーメッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_049");
			return RET_NONEXEC;
		}

		//メッセージ出力(更新します。よろしいですか？)
		msgParam = "更新";
		if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", msgParam) == MsgBoxResult.Cancel)
		{
			//キャンセル
			return RET_CANCEL;
		}

		//更新処理実行
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (System.Convert.ToString(this.grdList(i, Category_Koumoku.updatekbn)) == UpDateKbn)
			{
				if (ExecuteNonQuery(DbShoriKbn.Update, i) > 0)
				{
					this.grdList[i, Category_Koumoku.usingflg] = FixedCd.UsingFlg.Unused;
					returnValue = 1;
				}
			}
		}

		if (returnValue == 0)
		{
			return RET_NONEXEC;
		}
		else
		{
			//再検索のために当画面で使用中フラグを更新したデータは初期化する
			this.ExecuteReturn();
		}

		return returnValue;
	}

	/// <summary>
	/// 戻しデータを取得
	/// </summary>
	/// <remarks></remarks>
	protected override void ExecuteReturn()
	{

		//For i As Integer = 1 To Me.grdList.Rows.Count - 1
		//    '更新処理実行
		//    ExecuteNonQuery(DbShoriKbn.Insert, i)
		//Next
	}
	#endregion

	#region DB関連

	/// <summary>
	/// 対象コースのデータ取得
	/// </summary>
	protected override DataTable getCrsData()
	{

		return GetDbTable();
	}
	#endregion
	#endregion

	#region 実装用メソッド

	#region 初期処理

	/// <summary>
	/// 初期処理
	/// </summary>
	public void setSeFirsttDisplayData()
	{

		//カテゴリーは企画のみのため、定期は非表示
		this.grdList.Cols(Category_Koumoku.unkyukbn).Visible = false;

		// カラムの並び替えを可能にするか (初期値：True)
		this.grdList.AllowDragging = AllowDraggingEnum.None;

		// グリッドがデータソースに連結された時に列を自動的に作成するか (初期値：True)
		this.grdList.AutoGenerateColumns = false;

		// セルにいつコンボボタンを表示するか (初期値：Inherit)
		this.grdList.ShowButtons = ShowButtonsEnum.Always;

		//コードマスタからコンボデータを取得する
		setGridInitialize_grdCategory();

	}

	/// <summary>
	/// グリッド初期化（カテゴリー）
	/// </summary>
	private void setGridInitialize_grdCategory()
	{

		// コードマスタ情報 取得
		Code_DA dataAccess = new Code_DA();
		DataTable dtCategory = dataAccess.GetCodeMasterData(FixedCd.CodeBunrui.category);

		this.grdList.Cols(Category_Koumoku.categorycd1).DataType = typeof(string);
		this.grdList.Cols(Category_Koumoku.categorycd1).AllowEditing = true;
		this.grdList.Cols(Category_Koumoku.categorycd1).DataMap = CommonProcess.getComboboxData(dtCategory, true);
		this.grdList.Cols(Category_Koumoku.categorycd2).DataType = typeof(string);
		this.grdList.Cols(Category_Koumoku.categorycd2).AllowEditing = true;
		this.grdList.Cols(Category_Koumoku.categorycd2).DataMap = CommonProcess.getComboboxData(dtCategory, true);
		this.grdList.Cols(Category_Koumoku.categorycd3).DataType = typeof(string);
		this.grdList.Cols(Category_Koumoku.categorycd3).AllowEditing = true;
		this.grdList.Cols(Category_Koumoku.categorycd3).DataMap = CommonProcess.getComboboxData(dtCategory, true);
		this.grdList.Cols(Category_Koumoku.categorycd4).DataType = typeof(string);
		this.grdList.Cols(Category_Koumoku.categorycd4).AllowEditing = true;
		this.grdList.Cols(Category_Koumoku.categorycd4).DataMap = CommonProcess.getComboboxData(dtCategory, true);

	}
	#endregion

	#region チェック系

	/// <summary>
	/// 検索処理前のチェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	public bool CheckSearch()
	{
		return true;
	}

	#region チェック処理(Private)

	/// <summary>
	/// 過去日付チェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckPastday()
	{

		//現在日付取得
		DateTime dtNow = new DateTime();
		CommonDaProcess commonDaProcess = new CommonDaProcess();
		dtNow = System.Convert.ToDateTime(commonDaProcess.getDbSysTime.Rows(0).Item(0));

		//過去日付チェック
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CommonConvertUtil.ChkWhenDay(System.Convert.ToDateTime(this.grdList(i, Category_Koumoku.syuptday)), CommonConvertUtil.WhenKako, dtNow) == true)
			{
				this.grdList.Rows(i).AllowEditing = false;
				this.grdList[i, Category_Koumoku.henkoukahikbn] = Kahi;
				this.grdList.Rows(i).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
			}
		}
		return true;

	}

	///' <summary>
	///' 使用中フラグ更新処理
	///' </summary>
	///' <returns>True:エラー無 False:エラー有</returns>
	//Private Function CheckUsingFlg() As Boolean

	//    Dim dtUsingFlg As DataTable
	//    Dim dataAccess As New Category_DA
	//    Dim systemupdatepgmid As String = Me.Name
	//    Dim i As Integer = 1
	//    Dim FlgChk As Integer = 0

	//    '使用中フラグ更新
	//    dtUsingFlg = dataAccess.executeUsingFlgCrs(selectOldData, systemupdatepgmid)

	//    For Each row As DataRow In dtUsingFlg.Rows
	//        If CType(row("USING_FLG"), String) = FixedCd.UsingFlg.Use Then
	//            Me.grdList(i, Category_Koumoku.usingflg) = FixedCd.UsingFlg.Use
	//        Else
	//            FlgChk = 1
	//            Me.grdList.Rows(i).AllowEditing = False
	//            Me.grdList(i, Category_Koumoku.henkoukahikbn) = Kahi
	//        End If
	//        i += 1
	//    Next

	//    '1件でも使用中のレコードがあればメッセージを表示
	//    If FlgChk = 1 Then
	//        CommonProcess.createFactoryMsg().messageDisp("E90_050")
	//    End If
	//    Return True

	//End Function

	/// <summary>
	/// 更新処理前のチェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckCategoryUpdate(int i)
	{

		string CATEGORYCD1 = string.Empty;
		string CATEGORYCD2 = string.Empty;
		string CATEGORYCD3 = string.Empty;
		string CATEGORYCD4 = string.Empty;

		//入力不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		//引数の設定
		foreach (DataRow row in selectOldData.Rows)
		{
			if (row["SYUPT_DAY"].Equals(this.grdList(i, Category_Koumoku.syuptday)) &&)
			{
				row["HAISYA_KEIYU_CD_1"].ToString().Equals(this.grdList(i, Category_Koumoku.haisyakeiyucd1)) &&;
				row["GOUSYA"].Equals(this.grdList(i, Category_Koumoku.gousya)) &&;
				row["SYUPT_TIME_1"].Equals(this.grdList(i, Category_Koumoku.syupttime1));
				CATEGORYCD1 = row["CATEGORY_CD_1"].ToString();
				CATEGORYCD2 = row["CATEGORY_CD_2"].ToString();
				CATEGORYCD3 = row["CATEGORY_CD_3"].ToString();
				CATEGORYCD4 = row["CATEGORY_CD_4"].ToString();
			}
		}

		//'カテゴリー1
		if (Information.IsDBNull(this.grdList(i, Category_Koumoku.categorycd1)) == true)
		{
			this.grdList[i, Category_Koumoku.categorycd1] = string.Empty;
		}
		if (CATEGORYCD1.Equals(System.Convert.ToString(this.grdList(i, Category_Koumoku.categorycd1))) == false)
		{
			this.grdList[i, Category_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//カテゴリー2
		if (Information.IsDBNull(this.grdList(i, Category_Koumoku.categorycd2)) == true)
		{
			this.grdList[i, Category_Koumoku.categorycd2] = string.Empty;
		}
		if (CATEGORYCD2.Equals(System.Convert.ToString(this.grdList(i, Category_Koumoku.categorycd2))) == false)
		{
			this.grdList[i, Category_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//カテゴリー3
		if (Information.IsDBNull(this.grdList(i, Category_Koumoku.categorycd3)) == true)
		{
			this.grdList[i, Category_Koumoku.categorycd3] = string.Empty;
		}
		if (CATEGORYCD3.Equals(System.Convert.ToString(this.grdList(i, Category_Koumoku.categorycd3))) == false)
		{
			this.grdList[i, Category_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//カテゴリー4
		if (Information.IsDBNull(this.grdList(i, Category_Koumoku.categorycd4)) == true)
		{
			this.grdList[i, Category_Koumoku.categorycd4] = string.Empty;
		}
		if (CATEGORYCD4.Equals(System.Convert.ToString(this.grdList(i, Category_Koumoku.categorycd4))) == false)
		{
			this.grdList[i, Category_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		return false;

	}
	#endregion
	/// <summary>
	/// 使用中フラグ確認
	/// </summary>
	private void isUseCrsInfo()
	{
		CrsLeaderKeyValues crsKey = new CrsLeaderKeyValues();
		bool useFlg = false;
		int msgFlg = 0;

		for (int i = 1; i <= base.SearchResultGridData.Rows.Count; i++)
		{
			//先頭行の場合
			if (i == 1)
			{
				// コースキーセット
				crsKey.CrsCd = System.Convert.ToString(txtCrsCd.Text.ToString());
				crsKey.SyuPtDay = Strings.Replace(System.Convert.ToString(grdList.GetData(i, Category_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);
				crsKey.GouSya = System.Convert.ToInt32(grdList.GetData(i, Category_Koumoku.gousya));

				// 使用中フラグ確認
				useFlg = checkUseFlg(PrmData, crsKey);
			}

			//日付と号車が同行な場合、処理スキップ
			if (System.Convert.ToString(this.grdList(i, Category_Koumoku.syuptday)) == System.Convert.ToString(this.grdList(i - 1, Category_Koumoku.syuptday)) &&)
			{
				System.Convert.ToDecimal(this.grdList(i, Category_Koumoku.gousya)) = System.Convert.ToDecimal(System.Convert.ToDecimal(this.grdList(i - 1, Category_Koumoku.gousya)));
			}
			else
			{
				// コースキーセット
				crsKey.CrsCd = System.Convert.ToString(txtCrsCd.Text.ToString());
				crsKey.SyuPtDay = Strings.Replace(System.Convert.ToString(grdList.GetData(i, Category_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);
				crsKey.GouSya = System.Convert.ToInt32(grdList.GetData(i, Category_Koumoku.gousya));

				// 使用中フラグ確認
				useFlg = checkUseFlg(PrmData, crsKey);
			}

			//使用中であるレコードへ設定
			if (useFlg == true)
			{
				this.grdList.Rows(i).AllowEditing = false;
				this.grdList[i, Category_Koumoku.henkoukahikbn] = Kahi;
				// 使用中であるデータ背景色をグレー
				grdList.Rows(i).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
			}
		}
	}

	/// <summary>
	/// 使用中フラグ確認
	/// </summary>
	/// <param name="prmData">受渡しクラス</param>
	/// <param name="crsKey">キー項目</param>
	/// <returns>使用中:True 未使用:False</returns>
	private bool checkUseFlg(ResearchData prmData, CrsLeaderKeyValues crsKey)
	{
		DataRow[] rows;
		bool returnFlg = false;

		string selstring = string.Empty;
		selstring += "CRS_CD = '" + System.Convert.ToString(crsKey.CrsCd) + "'";
		selstring += " AND ";
		selstring += "SYUPT_DAY = '" + System.Convert.ToString(crsKey.SyuPtDay) + "'";
		selstring += " AND ";
		selstring += "GOUSYA = " + System.Convert.ToString(System.Convert.ToInt32(crsKey.GouSya));

		rows = PrmData.Prmtable.Select(selstring);
		foreach ( in rows)
		{
			if (drCrs("USING_FLG").ToString().Equals(UsingFlg.Use))
			{
				returnFlg = true;
				break;
			}
		}

		return returnFlg;
	}
	#endregion

	#region DB更新処理

	///' <summary>
	///' 更新・登録処理
	///' </summary>
	///' <returns>更新処理件数</returns>
	private int ExecuteNonQuery(DbShoriKbn kbn, int i)
	{

		//戻り値
		int returnValue = 0;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		//DAクラス作成、パラメータ設定、登録(更新)処理実施までを実装
		//DataAccessクラス生成
		Category_DA dataAccess = new Category_DA();

		if (DbShoriKbn.Update.Equals(kbn))
		{
			//パラメータ設定
			paramInfoList.Add("SYUPT_DAY", Strings.Replace(System.Convert.ToString(this.grdList(i, 1)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));
			paramInfoList.Add("CRS_CD", this.txtCrsCd.Text);
			paramInfoList.Add("GOUSYA", this.grdList(i, Category_Koumoku.gousya));
			paramInfoList.Add("CATEGORY_CD_1", this.grdList(i, Category_Koumoku.categorycd1));
			paramInfoList.Add("CATEGORY_CD_2", this.grdList(i, Category_Koumoku.categorycd2));
			paramInfoList.Add("CATEGORY_CD_3", this.grdList(i, Category_Koumoku.categorycd3));
			paramInfoList.Add("CATEGORY_CD_4", this.grdList(i, Category_Koumoku.categorycd4));
			//paramInfoList.Add("USING_FLG", FixedCd.UsingFlg.Unused)
			paramInfoList.Add("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime());
			paramInfoList.Add("SYSTEM_UPDATE_PGMID", this.Name);
			paramInfoList.Add("SYSTEM_UPDATE_PERSON_CD", UserInfoManagement.userId);
			//Else
			//    '引数の設定
			//    For Each row As DataRow In selectOldData.Rows
			//        If row("SYUPT_DAY").Equals(Me.grdList(i, Category_Koumoku.syuptday)) AndAlso
			//                    row("HAISYA_KEIYU_CD_1").ToString.Equals(Me.grdList(i, Category_Koumoku.haisyakeiyucd1)) AndAlso
			//                    row("GOUSYA").Equals(Me.grdList(i, Category_Koumoku.gousya)) AndAlso
			//                    row("SYUPT_TIME_1").Equals(Me.grdList(i, Category_Koumoku.syupttime1)) Then
			//            If row("USING_FLG").ToString.Equals(Me.grdList(i, Category_Koumoku.usingflg)) Then
			//                Return returnValue
			//            ElseIf row("USING_FLG").ToString = String.empty AndAlso
			//                IsDBNull(Me.grdList(i, Category_Koumoku.usingflg)) = True Then
			//                Return returnValue
			//            Else
			//                'パラメータ設定
			//                paramInfoList.Add("SYUPT_DAY", CType(Replace(CType(row("SYUPT_DAY"), String), "/", ""), Decimal))
			//                paramInfoList.Add("CRS_CD", CType(row("CRS_CD"), String))
			//                paramInfoList.Add("GOUSYA", CType(row("GOUSYA"), Decimal))
			//                paramInfoList.Add("SYSTEM_UPDATE_DAY", CType(row("SYSTEM_UPDATE_DAY"), Date))
			//                paramInfoList.Add("SYSTEM_UPDATE_PGMID", CType(row("SYSTEM_UPDATE_PGMID"), String))
			//                paramInfoList.Add("SYSTEM_UPDATE_PERSON_CD", CType(row("SYSTEM_UPDATE_PERSON_CD"), String))
			//                paramInfoList.Add("USING_FLG", FixedCd.UsingFlg.Unused)
			//            End If
			//        End If
			//    Next

		}

		try
		{
			if (DbShoriKbn.Update.Equals(kbn))
			{
				//Updateの実施
				returnValue = System.Convert.ToInt32(dataAccess.executeCategoryTehai(Category_DA.accessType.executeUpdateCategory, paramInfoList));
				//Else
				//    'Returnの実施
				//    returnValue = dataAccess.executeCategoryTehai(Category_DA.accessType.executeReturnCategory, paramInfoList)
			}

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

	/// <summary>
	/// 検索処理
	/// </summary>
	/// <returns>取得データ(DataTable)</returns>
	private DataTable GetDbTable()
	{

		//戻り値
		DataTable totalValue = new DataTable();

		totalValue.Columns.Add("SYUPT_DAY"); //出発日
		totalValue.Columns.Add("CRS_CD"); //コースコード
		totalValue.Columns.Add("YOBI_CD"); //曜日コード
		totalValue.Columns.Add("HAISYA_KEIYU_CD_1"); //乗車地コード
		totalValue.Columns.Add("SYUPT_TIME_1"); //出発時間
		totalValue.Columns.Add("GOUSYA"); //号車
		totalValue.Columns.Add("UNKYU_KBN"); //運休区分
		totalValue.Columns.Add("SAIKOU_KAKUTEI_KBN"); //催行確定区分
		totalValue.Columns.Add("CATEGORY_CD_1"); //カテゴリー1
		totalValue.Columns.Add("CATEGORY_CD_2"); //カテゴリー2
		totalValue.Columns.Add("CATEGORY_CD_3"); //カテゴリー3
		totalValue.Columns.Add("CATEGORY_CD_4"); //カテゴリー4
		totalValue.Columns.Add("SYSTEM_UPDATE_DAY"); //システム更新日
		totalValue.Columns.Add("SYSTEM_UPDATE_PERSON_CD"); //システム更新者コード
		totalValue.Columns.Add("SYSTEM_UPDATE_PGMID"); //システム更新ＰＧＭＩＤ
		totalValue.Columns.Add("USING_FLG"); //使用中フラグ
		totalValue.Columns.Add("HENKOU_KAHI_KBN"); //変更可否区分
		totalValue.Columns.Add("UPDATE_KBN"); //更新区分
		totalValue.Columns.Add("HIDDEN_GOUSYA"); //号車（ソートキー用３桁補正）

		//DataAccessクラス生成
		Category_DA dataAccess = new Category_DA();

		foreach (DataRow row in PrmData.Prmtable.Rows)
		{
			//DBパラメータ
			Hashtable paramInfoList = new Hashtable();
			DataTable returnValue = null;

			paramInfoList.Add("SYUPT_DAY", row["SYUPT_DAY"]); //出発日
			paramInfoList.Add("CRS_CD", row["CRS_CD"]); //コースコード
			paramInfoList.Add("YOBI_CD", row["YOBI_CD"].ToString()); //曜日コード
			paramInfoList.Add("HAISYA_KEIYU_CD_1", row["HAISYA_KEIYU_CD_1"].ToString()); //乗車地コード
			paramInfoList.Add("GOUSYA", row["GOUSYA"]); //号車
			if (row["UNKYU_KBN"].ToString() == string.Empty)
			{
				paramInfoList.Add("UNKYU_KBN", string.Empty); //運休区分
			}
			else
			{
				paramInfoList.Add("UNKYU_KBN", row["UNKYU_KBN"].ToString()); //運休区分
			}
			if (row["SAIKOU_KAKUTEI_KBN"].ToString() == string.Empty)
			{
				paramInfoList.Add("SAIKOU_KAKUTEI_KBN", string.Empty); //催行区分
			}
			else
			{
				paramInfoList.Add("SAIKOU_KAKUTEI_KBN", row["SAIKOU_KAKUTEI_KBN"].ToString()); //催行区分
			}
			if (row["MARU_ZOU_MANAGEMENT_KBN"].ToString() == string.Empty)
			{
				paramInfoList.Add("MARU_ZOU_MANAGEMENT_KBN", string.Empty); //〇増区分
			}
			else
			{
				paramInfoList.Add("MARU_ZOU_MANAGEMENT_KBN", row["MARU_ZOU_MANAGEMENT_KBN"].ToString()); //〇増区分
			}

			try
			{
				returnValue = dataAccess.accessCategoryTehai(Category_DA.accessType.getCategory, paramInfoList);

				foreach (DataRow row2 in returnValue.Rows)
				{
					DataRow row3 = totalValue.NewRow;
					row3["SYUPT_DAY"] = row2["SYUPT_DAY"];
					row3["CRS_CD"] = row2["CRS_CD"];
					row3["YOBI_CD"] = row2["YOBI_CD"];
					row3["HAISYA_KEIYU_CD_1"] = row2["HAISYA_KEIYU_CD_1"];
					row3["SYUPT_TIME_1"] = row2["SYUPT_TIME_1"];
					row3["GOUSYA"] = row2["GOUSYA"];
					row3["UNKYU_KBN"] = row2["UNKYU_KBN"];
					row3["SAIKOU_KAKUTEI_KBN"] = row2["SAIKOU_KAKUTEI_KBN"];
					row3["CATEGORY_CD_1"] = row2["CATEGORY_CD_1"];
					row3["CATEGORY_CD_2"] = row2["CATEGORY_CD_2"];
					row3["CATEGORY_CD_3"] = row2["CATEGORY_CD_3"];
					row3["CATEGORY_CD_4"] = row2["CATEGORY_CD_4"];
					row3["SYSTEM_UPDATE_DAY"] = row2["SYSTEM_UPDATE_DAY"];
					row3["SYSTEM_UPDATE_PERSON_CD"] = row2["SYSTEM_UPDATE_PERSON_CD"];
					row3["SYSTEM_UPDATE_PGMID"] = row2["SYSTEM_UPDATE_PGMID"];
					row3["USING_FLG"] = row2["USING_FLG"];
					row3["HENKOU_KAHI_KBN"] = row2["HENKOU_KAHI_KBN"];
					row3["UPDATE_KBN"] = row2["UPDATE_KBN"];
					row3["HIDDEN_GOUSYA"] = row2["GOUSYA"].ToString().PadLeft(3, '0');

					totalValue.Rows.Add(row3);
				}

			}
			catch (OracleException)
			{
				throw;
			}
			catch (Exception)
			{
				throw;
			}
		}

		// ソート処理
		DataRow[] sortRows = (DataRow[])(totalValue.Select(null, "SYUPT_DAY ASC , SYUPT_TIME_1 ASC , HIDDEN_GOUSYA ASC").Clone());

		DataTable sortTable = totalValue.Clone();

		foreach (DataRow row in sortRows)
		{
			sortTable.ImportRow(row);
		}

		totalValue = sortTable.Copy;

		return totalValue;

	}
	#endregion

	#endregion





}