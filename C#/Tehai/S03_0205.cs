using C1.Win.C1FlexGrid;
using System.ComponentModel;
using Hatobus.ReservationManagementSystem.Master;



/// <summary>
/// コース台帳一括修正（コース情報）
/// コース情報を変更する
/// </summary>
public class S03_0205 : PT14, iPT14
{

	#region  定数／変数宣言

	private DataTable selectOldData; //検索後のデータを保持
	public ResearchData PrmData //データ格納クラス
	{
		private string OldInfo = string.Empty; //コース情報変更前
	private string Kahi = "Y"; //グリッドの変更可否区分
	private string UpDateKbn = "Y"; //グリッドの更新区分
	private const int crsinfocdMaxLength = 2; //コード最大バイト数
	private const int crsinfonameMaxLength = 32; //コース情報最大バイト数
	private string CdButton = "|..."; //コードのボタンデザイン
	private string OldSyuptDay = string.Empty; //フラグ戻し比較出発日
	private string OldGousya = string.Empty; //フラグ戻し比較号車


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
		public Integer? Edaban;
		public int Rnk;
		public string Key;
	}

	/// <summary>
	/// カラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum CrsInfo_Koumoku : int
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
		[Value("枝番")]
		lineno,
		[Value("コード")]
		cd,
		[Value("コース情報")]
		info,
		[Value("出発日2")]
		syuptday2,
		[Value("乗車地2")]
		haisyakeiyucd2,
		[Value("号車2")]
		gousya2,
		[Value("出発時間2")]
		syupttime2,
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
			if (CheckCrsInfoUpdate(i) == true)
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

		//定期・企画チェック
		this.CheckTeikiKikaku();

		//過去日付チェック
		this.CheckPastday();

		//使用中フラグチェック
		this.isUseCrsInfo();

		//同行の表示設定
		this.CheckDoGyo();

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
			if (grdList.Rows(i).AllowEditing == false)
			{
			}
			else
			{
				this.grdList.GetCellRange(i, CrsInfo_Koumoku.info).StyleNew.BackColor = BackColorType.Standard;
			}
		}

		//入力差分チェック
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CheckCrsInfoUpdate(i) == true)
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

		msgParam = "更新";
		//メッセージ出力(更新します。よろしいですか？)
		if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", msgParam) == MsgBoxResult.Cancel)
		{
			//キャンセル
			return RET_CANCEL;
		}

		// 歯抜けチェック
		this.chkHanuke();

		//更新処理実行
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (System.Convert.ToString(this.grdList(i, CrsInfo_Koumoku.updatekbn)) == UpDateKbn)
			{
				if (ExecuteNonQuery(DbShoriKbn.Update, i) > 0)
				{
					returnValue = 1;
				}
			}
		}

		if (returnValue == 0)
		{
			return RET_NONEXEC;
		}

		return returnValue;
	}

	/// <summary>
	/// 戻しデータを取得
	/// </summary>
	/// <remarks></remarks>
	protected override void ExecuteReturn()
	{
	}

	/// <summary>
	/// グリッドのEnterCellイベント
	/// </summary>
	/// <remarks></remarks>
	private void grdList_EnterCell(object sender, EventArgs e)
	{

		//グリッド範囲外は処理しない
		if (this.grdList.RowSel < 0 || this.grdList.ColSel < 0)
		{
			return;
		}
		else
		{
			if (this.grdList.ColSel == CrsInfo_Koumoku.info)
			{
				if (Information.IsDBNull(this.grdList(this.grdList.RowSel, this.grdList.ColSel)) == false)
				{
					OldInfo = System.Convert.ToString(this.grdList(this.grdList.RowSel, this.grdList.ColSel));
				}
			}
		}
	}

	/// <summary>
	/// グリッドのLeaveCellイベント
	/// </summary>
	/// <remarks></remarks>
	private void grdList_LeaveCell(object sender, EventArgs e)
	{

		//DataAccessクラス生成
		CrsInfo_DA dataAccess = new CrsInfo_DA();

		//グリッド範囲外は処理しない
		if (this.grdList.RowSel < 0 || this.grdList.ColSel < 0)
		{
			return;
		}
		else
		{
			if (this.grdList.ColSel == CrsInfo_Koumoku.cd)
			{
				if (ReferenceEquals(this.grdList.GetData(this.grdList.Row, this.grdList.Col), null) == false)
				{
					if (this.grdList(this.grdList.Row, this.grdList.Col).ToString() != string.Empty)
					{
						string masterSyubetsu = "M_CODE";
						string masterWhere = "CODE_BUNRUI = '" + FixedCd.CodeBunrui.crsinfo + "' AND CODE_VALUE = '" + this.grdList(this.grdList.RowSel, CrsInfo_Koumoku.cd).ToString() + "' AND DELETE_DATE IS NULL";
						string msg = "";
						if (CommonCheckUtil.chkMaster(masterWhere, masterSyubetsu) == false)
						{
							this.grdList.SetData(this.grdList.Row, CrsInfo_Koumoku.info, string.Empty);
							// コードマスタに該当データがない場合は、メッセージを表示する
							CommonProcess.createFactoryMsg().messageDisp("E90_019");
							return;
						}
						else
						{
							msg = System.Convert.ToString(dataAccess.GetCdMaster(CdBunruiType.crsInfoMaster, this.grdList(this.grdList.RowSel, CrsInfo_Koumoku.cd).ToString()));
							this.grdList.SetData(this.grdList.Row, CrsInfo_Koumoku.info, msg);
						}
					}
				}
			}
			else if (this.grdList.ColSel == CrsInfo_Koumoku.info)
			{
				if (Information.IsDBNull(this.grdList(this.grdList.RowSel, this.grdList.ColSel)) == false)
				{
					if (this.grdList(this.grdList.RowSel, this.grdList.ColSel).Equals(string.Empty))
					{
						this.grdList[this.grdList.RowSel, this.grdList.ColSel] = Strings.Replace(System.Convert.ToString(this.grdList[this.grdList.RowSel, this.grdList.ColSel]), " ", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);
					}
					else
					{
						//入力した内容に空白がある場合は、空白を削除する
						this.grdList[this.grdList.RowSel, this.grdList.ColSel] = System.Convert.ToString(this.grdList[this.grdList.RowSel, this.grdList.ColSel]).Trim();
						//手入力した場合は、コードをクリアする
						if (OldInfo == System.Convert.ToString(this.grdList(this.grdList.RowSel, this.grdList.ColSel)))
						{
						}
						else
						{
							this.grdList[this.grdList.RowSel, CrsInfo_Koumoku.cd] = string.Empty;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// グリッドのStartEditイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void grdList_StartEdit(System.Object sender, RowColEventArgs e)
	{

		if (e.Col == CrsInfo_Koumoku.cd)
		{
			//コード
			TextBoxEx txtCrsInfoCd = new TextBoxEx();
			txtCrsInfoCd.MaxLengthUnit = GrapeCity.Win.Editors.LengthUnit.Byte;
			txtCrsInfoCd.MaxLength = crsinfocdMaxLength;
			txtCrsInfoCd.Format = Common.setControlFormat(FixedCd.ControlFormat.HankakuEiSuji);
			this.grdList.Editor = txtCrsInfoCd;
		}
		else if (e.Col == CrsInfo_Koumoku.info)
		{
			//コース情報
			TextBoxEx txtCrsInfoName = new TextBoxEx();
			txtCrsInfoName.MaxLengthUnit = GrapeCity.Win.Editors.LengthUnit.Byte;
			txtCrsInfoName.MaxLength = crsinfonameMaxLength;
			this.grdList.Editor = txtCrsInfoName;
		}
	}

	/// <summary>
	/// grdコード_CellButtonClick
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void grdList_CellButtonClick(object sender, RowColEventArgs e)
	{

		CdMasterEntity ReturnEntityCdMaster = null;

		try
		{
			S90_0101 cmsel = new S90_0101();
			cmsel.setKey1(CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.crsInfoMaster));
			cmsel.ShowDialog(this);
			if (!ReferenceEquals(cmsel.getReturnEntity, null))
			{
				ReturnEntityCdMaster = (CdMasterEntity)cmsel.getReturnEntity;
				this.grdList.SetData(e.Row, CrsInfo_Koumoku.cd, ReturnEntityCdMaster.CdValue.Value);
				this.grdList.SetData(e.Row, CrsInfo_Koumoku.info, ReturnEntityCdMaster.CdNm.Value);
				grdList_LeaveCell(sender, e);
			}
		}
		catch (Exception ex)
		{
			//outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.その他, ex.Message)
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, ex.Message);
			createFactoryMsg.messageDisp("9001");
		}
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

		// セルにいつコンボボタンを表示するか (初期値：Inherit)
		this.grdList.ShowButtons = ShowButtonsEnum.Always;

		//コードのボタン表示
		this.grdList.Cols(CrsInfo_Koumoku.cd).ComboList = CdButton;

		//幅の設定
		this.grdList.Cols(CrsInfo_Koumoku.cd).Width = 80;
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
	/// 定期・企画チェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckTeikiKikaku()
	{

		//定期・企画によってヘッダの表示/非表示を設定
		if (System.Convert.ToInt32(base.SearchResultGridData.Rows(0).ItemArray(8)) == FixedCd.TeikiKikakuKbn.Teiki)
		{
			this.grdList.Cols(CrsInfo_Koumoku.unkyukbn).Visible = true;
			this.grdList.Cols(CrsInfo_Koumoku.saikoukakuteikbn).Visible = false;
		}
		else if (System.Convert.ToInt32(base.SearchResultGridData.Rows(0).ItemArray(8)) == FixedCd.TeikiKikakuKbn.Kikaku)
		{
			this.grdList.Cols(CrsInfo_Koumoku.unkyukbn).Visible = false;
			this.grdList.Cols(CrsInfo_Koumoku.saikoukakuteikbn).Visible = true;
		}
		return true;

	}

	/// <summary>
	/// 同行の表示設定処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckDoGyo()
	{

		//日付と号車が同行な場合、空白に設定する
		if (base.SearchResultGridData.Rows.Count > 1)
		{
			for (int i = 2; i <= base.SearchResultGridData.Rows.Count; i++)
			{
				if (System.Convert.ToString(this.grdList(i, CrsInfo_Koumoku.syuptday2)) == System.Convert.ToString(this.grdList(i - 1, CrsInfo_Koumoku.syuptday2)) &&)
				{
					System.Convert.ToDecimal(this.grdList(i, CrsInfo_Koumoku.gousya2)) = System.Convert.ToDecimal(System.Convert.ToDecimal(this.grdList(i - 1, CrsInfo_Koumoku.gousya2)));
					this.grdList[i, CrsInfo_Koumoku.syuptday] = string.Empty;
					this.grdList[i, CrsInfo_Koumoku.yobicd] = string.Empty;
					this.grdList[i, CrsInfo_Koumoku.haisyakeiyucd1] = string.Empty;
					this.grdList[i, CrsInfo_Koumoku.syupttime1] = string.Empty;
					this.grdList[i, CrsInfo_Koumoku.gousya] = string.Empty;
					this.grdList[i, CrsInfo_Koumoku.unkyukbn] = string.Empty;
					this.grdList[i, CrsInfo_Koumoku.saikoukakuteikbn] = string.Empty;
				}
			}
		}
		return true;

	}

	/// <summary>
	/// 過去日付チェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckPastday()
	{

		DateTime dtNow = new DateTime();
		CommonDaProcess commonDaProcess = new CommonDaProcess();
		dtNow = System.Convert.ToDateTime(commonDaProcess.getDbSysTime.Rows(0).Item(0));

		//過去日付チェック
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CommonConvertUtil.ChkWhenDay(System.Convert.ToDateTime(this.grdList(i, CrsInfo_Koumoku.syuptday)), CommonConvertUtil.WhenKako, dtNow) == true)
			{
				this.grdList.Rows(i).AllowEditing = false;
				this.grdList[i, CrsInfo_Koumoku.henkoukahikbn] = Kahi;
				// 過去日であるデータ背景色をグレー
				this.grdList.Rows(i).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
			}
		}
		return true;

	}

	/// <summary>
	/// 更新処理前のチェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckCrsInfoUpdate(int i)
	{

		string CRSJOKEN = string.Empty;

		//入力不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		//引数の設定
		foreach (DataRow row in selectOldData.Rows)
		{
			if (row["SYUPT_DAY"].Equals(this.grdList(i, CrsInfo_Koumoku.syuptday2)) &&)
			{
				row["HAISYA_KEIYU_CD_1"].ToString().Equals(this.grdList(i, CrsInfo_Koumoku.haisyakeiyucd2)) &&;
				row["GOUSYA"].Equals(this.grdList(i, CrsInfo_Koumoku.gousya2)) &&;
				row["SYUPT_TIME_1"].Equals(this.grdList(i, CrsInfo_Koumoku.syupttime2)) &&;
				row["LINE_NO"].Equals(this.grdList(i, CrsInfo_Koumoku.lineno));
				CRSJOKEN = row["CRS_JOKEN"].ToString();
			}
		}

		//コース情報
		if (Information.IsDBNull(this.grdList(i, CrsInfo_Koumoku.info)) == true)
		{
			this.grdList[i, CrsInfo_Koumoku.info] = string.Empty;
		}
		if (CRSJOKEN.Equals(System.Convert.ToString(this.grdList(i, CrsInfo_Koumoku.info))) == false)
		{
			this.grdList[i, CrsInfo_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		return false;

	}
	#endregion
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
		CrsInfo_DA dataAccess = new CrsInfo_DA();

		if (DbShoriKbn.Update.Equals(kbn))
		{
			//パラメータ設定
			paramInfoList.Add("SYUPT_DAY", Strings.Replace(System.Convert.ToString(this.grdList(i, 11)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));
			paramInfoList.Add("CRS_CD", this.txtCrsCd.Text);
			paramInfoList.Add("GOUSYA", this.grdList(i, CrsInfo_Koumoku.gousya2));
			paramInfoList.Add("USING_FLG", FixedCd.UsingFlg.Use);
			paramInfoList.Add("LINE_NO", this.grdList(i, CrsInfo_Koumoku.lineno));
			paramInfoList.Add("CRS_JOKEN", this.grdList(i, CrsInfo_Koumoku.info));
			//コース台帳（コース情報）用
			foreach (DataRow row in selectOldData.Rows)
			{
				if (row["SYUPT_DAY"].Equals(this.grdList(i, CrsInfo_Koumoku.syuptday2)) &&)
				{
					row["GOUSYA"].Equals(this.grdList(i, CrsInfo_Koumoku.gousya2)) &&;
					row["SYUPT_TIME_1"].Equals(this.grdList(i, CrsInfo_Koumoku.syupttime2)) &&;
					row["LINE_NO"].Equals(this.grdList(i, CrsInfo_Koumoku.lineno));
					//パラメータ設定
					paramInfoList.Add("DELETE_DAY", row["DELETE_DAY"]);
					paramInfoList.Add("SYSTEM_ENTRY_DAY", row["SYSTEM_ENTRY_DAY"]);
					paramInfoList.Add("SYSTEM_ENTRY_PGMID", row["SYSTEM_ENTRY_PGMID"]);
					paramInfoList.Add("SYSTEM_ENTRY_PERSON_CD", row["SYSTEM_ENTRY_PERSON_CD"]);
				}
			}
			paramInfoList.Add("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime());
			paramInfoList.Add("SYSTEM_UPDATE_PGMID", this.Name);
			paramInfoList.Add("SYSTEM_UPDATE_PERSON_CD", UserInfoManagement.userId);
		}

		try
		{
			if (DbShoriKbn.Update.Equals(kbn))
			{
				//Updateの実施
				returnValue = System.Convert.ToInt32(dataAccess.executeCrsInfoTehai(CrsInfo_DA.accessType.executeUpdateCrsInfo, paramInfoList));
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
		totalValue.Columns.Add("TEIKI_KIKAKU_KBN"); //定期・企画区分
		totalValue.Columns.Add("LINE_NO"); //行ＮＯ
		totalValue.Columns.Add("CRS_INFO"); //コード
		totalValue.Columns.Add("CRS_JOKEN"); //コース条件
		totalValue.Columns.Add("DELETE_DAY"); //削除日
		totalValue.Columns.Add("SYSTEM_ENTRY_DAY"); //コース情報_システム更新日
		totalValue.Columns.Add("SYSTEM_ENTRY_PERSON_CD"); //コース情報_システム更新者コード
		totalValue.Columns.Add("SYSTEM_ENTRY_PGMID"); //コース情報_システム更新ＰＧＭＩＤ
		totalValue.Columns.Add("SYSTEM_UPDATE_DAY"); //システム更新日
		totalValue.Columns.Add("SYSTEM_UPDATE_PERSON_CD"); //システム更新者コード
		totalValue.Columns.Add("SYSTEM_UPDATE_PGMID"); //システム更新ＰＧＭＩＤ
		totalValue.Columns.Add("SYUPT_DAY2"); //出発日
		totalValue.Columns.Add("HAISYA_KEIYU_CD_12"); //乗車地コード
		totalValue.Columns.Add("GOUSYA2"); //号車
		totalValue.Columns.Add("SYUPT_TIME_12"); //出発時間
		totalValue.Columns.Add("USING_FLG"); //使用中フラグ
		totalValue.Columns.Add("HENKOU_KAHI_KBN"); //変更可否区分
		totalValue.Columns.Add("UPDATE_KBN"); //更新区分
		totalValue.Columns.Add("HIDDEN_GOUSYA"); //号車（ソートキー用３桁補正）

		//DataAccessクラス生成
		CrsInfo_DA dataAccess = new CrsInfo_DA();

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
				returnValue = dataAccess.accessCrsInfoTehai(CrsInfo_DA.accessType.getCrsInfo, paramInfoList);

				int i = 1;
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
					row3["TEIKI_KIKAKU_KBN"] = row2["TEIKI_KIKAKU_KBN"];
					row3["LINE_NO"] = row2["LINE_NO"];
					row3["CRS_INFO"] = row2["CRS_INFO"];
					row3["CRS_JOKEN"] = row2["CRS_JOKEN"];
					row3["DELETE_DAY"] = row2["DELETE_DAY"];
					row3["SYSTEM_ENTRY_DAY"] = row2["SYSTEM_ENTRY_DAY"];
					row3["SYSTEM_ENTRY_PERSON_CD"] = row2["SYSTEM_ENTRY_PERSON_CD"];
					row3["SYSTEM_ENTRY_PGMID"] = row2["SYSTEM_ENTRY_PGMID"];
					row3["SYSTEM_UPDATE_DAY"] = row2["SYSTEM_UPDATE_DAY"];
					row3["SYSTEM_UPDATE_PERSON_CD"] = row2["SYSTEM_UPDATE_PERSON_CD"];
					row3["SYSTEM_UPDATE_PGMID"] = row2["SYSTEM_UPDATE_PGMID"];
					row3["SYUPT_DAY2"] = row2["SYUPT_DAY2"];
					row3["HAISYA_KEIYU_CD_12"] = row2["HAISYA_KEIYU_CD_12"];
					row3["GOUSYA2"] = row2["GOUSYA2"];
					row3["SYUPT_TIME_12"] = row2["SYUPT_TIME_12"];
					row3["USING_FLG"] = row2["USING_FLG"];
					row3["HENKOU_KAHI_KBN"] = row2["HENKOU_KAHI_KBN"];
					row3["UPDATE_KBN"] = row2["UPDATE_KBN"];
					row3["HIDDEN_GOUSYA"] = row2["GOUSYA"].ToString().PadLeft(3, '0');

					totalValue.Rows.Add(row3);

					//追加登録用にデータ+空行1件追加（最大10件まで）
					if (i == returnValue.Rows.Count)
					{
						//If IsDBNull(row2("CRS_JOKEN")) = True Then
						//Else
						for (int edaban = returnValue.Rows.Count; edaban <= 10 - 1; edaban++)
						{
							//If returnValue.Rows.Count < 10 Then
							DataRow row4 = totalValue.NewRow;
							row4["SYUPT_DAY"] = row2["SYUPT_DAY"];
							row4["CRS_CD"] = row2["CRS_CD"];
							row4["YOBI_CD"] = row2["YOBI_CD"];
							row4["HAISYA_KEIYU_CD_1"] = row2["HAISYA_KEIYU_CD_1"];
							row4["SYUPT_TIME_1"] = row2["SYUPT_TIME_1"];
							row4["GOUSYA"] = row2["GOUSYA"];
							row4["UNKYU_KBN"] = row2["UNKYU_KBN"];
							row4["SAIKOU_KAKUTEI_KBN"] = row2["SAIKOU_KAKUTEI_KBN"];
							row4["TEIKI_KIKAKU_KBN"] = row2["TEIKI_KIKAKU_KBN"];
							row4["LINE_NO"] = edaban + 1;
							row4["CRS_INFO"] = row2["CRS_INFO"];
							row4["CRS_JOKEN"] = string.Empty;
							row4["DELETE_DAY"] = row2["DELETE_DAY"];
							row4["SYSTEM_ENTRY_DAY"] = row2["SYSTEM_ENTRY_DAY"];
							row4["SYSTEM_ENTRY_PERSON_CD"] = row2["SYSTEM_ENTRY_PERSON_CD"];
							row4["SYSTEM_ENTRY_PGMID"] = row2["SYSTEM_ENTRY_PGMID"];
							row4["SYSTEM_UPDATE_DAY"] = row2["SYSTEM_UPDATE_DAY"];
							row4["SYSTEM_UPDATE_PERSON_CD"] = row2["SYSTEM_UPDATE_PERSON_CD"];
							row4["SYSTEM_UPDATE_PGMID"] = row2["SYSTEM_UPDATE_PGMID"];
							row4["SYUPT_DAY2"] = row2["SYUPT_DAY2"];
							row4["HAISYA_KEIYU_CD_12"] = row2["HAISYA_KEIYU_CD_12"];
							row4["GOUSYA2"] = row2["GOUSYA2"];
							row4["SYUPT_TIME_12"] = row2["SYUPT_TIME_12"];
							row4["USING_FLG"] = row2["USING_FLG"];
							row4["HENKOU_KAHI_KBN"] = row2["HENKOU_KAHI_KBN"];
							row4["UPDATE_KBN"] = row2["UPDATE_KBN"];
							row4["HIDDEN_GOUSYA"] = row2["GOUSYA"].ToString().PadLeft(3, '0');

							totalValue.Rows.Add(row4);
							//End If
						}
						//End If
					}

					i++;
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
		int setCount = 1;
		// ソート用カウント
		totalValue.Columns.Add("num", typeof(double));
		foreach (DataRow row in totalValue.Rows)
		{
			row["num"] = setCount;
			setCount++;
		}

		DataRow[] sortRows = (DataRow[])(totalValue.Select(null, "SYUPT_DAY ASC , SYUPT_TIME_1 ASC , HIDDEN_GOUSYA ASC , num ASC").Clone());

		// ソート用カラムを削除
		totalValue.Columns.Remove("num");

		DataTable sortTable = totalValue.Clone();

		foreach (DataRow row in sortRows)
		{
			sortTable.ImportRow(row);
		}

		totalValue = sortTable.Copy;

		return totalValue;

	}
	#endregion
	/// <summary>
	/// 歯抜けチェック
	/// </summary>
	///
	private void chkHanuke()
	{
		int rownum = 10;

		//10行固定でループ
		for (int cnt = 0; cnt <= (System.Convert.ToInt32((this.grdList.Rows.Count - 1) / rownum)) - 1; cnt++)
		{
			//注意事項/1行目を除いてループ
			for (i = 2; i <= 10; i++)
			{
				//設定があった場合
				if (this.grdList(i + (cnt * rownum), CrsInfo_Koumoku.info).ToString() != string.Empty)
				{
					//カレント行より上の空白を取得
					int nullRow = 0;
					for (nullRow = (i + (cnt * rownum)) - 1; nullRow >= ((cnt * rownum) + 1); nullRow--)
					{
						if (this.grdList(nullRow, CrsInfo_Koumoku.info).ToString() != string.Empty)
						{
							break;
						}
					}
					//空白があった場合
					if (nullRow != (i + (cnt * rownum)) - 1)
					{
						nullRow++;
						//null行に設定を移動
						this.grdList.SetData(nullRow, CrsInfo_Koumoku.info, this.grdList(i + (cnt * rownum), CrsInfo_Koumoku.info));
						this.grdList.SetData(nullRow, CrsInfo_Koumoku.cd, this.grdList(i + (cnt * rownum), CrsInfo_Koumoku.cd));
						this.grdList.SetData(nullRow, CrsInfo_Koumoku.updatekbn, UpDateKbn);

						//元設定にstring.emptyを挿入
						this.grdList.SetData(i + (cnt * rownum), CrsInfo_Koumoku.info, string.Empty);
						this.grdList.SetData(i + (cnt * rownum), CrsInfo_Koumoku.cd, string.Empty);
						this.grdList.SetData(i + (cnt * rownum), CrsInfo_Koumoku.updatekbn, string.Empty);
					}
				}
			}
		}
		//Dim upFlg As Boolean = False
		//Dim dataChg As Integer = 1

		//For i As Integer = 1 To Me.grdList.Rows.Count - 1
		//    '先頭行の場合は次行へ
		//    If i = 1 Then
		//        ' 変更有無チェック
		//        If CType(Me.grdList(i, CrsInfo_Koumoku.updatekbn), String) = UpDateKbn Then
		//            '変更ありのフラグを構築
		//            upFlg = True
		//        End If
		//        Continue For
		//    Else
		//        '前行と日付・号車が同じ場合
		//        If CType(Me.grdList(i, CrsInfo_Koumoku.syuptday2), String) = CType(Me.grdList(i - 1, CrsInfo_Koumoku.syuptday2), String) AndAlso
		//        CType(Me.grdList(i, CrsInfo_Koumoku.gousya2), Decimal) = CType(Me.grdList(i - 1, CrsInfo_Koumoku.gousya2), Decimal) Then
		//            '最終行である場合
		//            If i = Me.grdList.Rows.Count - 1 Then
		//                '変更があった場合
		//                If upFlg = True Then
		//                    Dim nullRow As Integer = 0
		//                    'レコードのスタート位置からループ
		//                    For uprow As Integer = dataChg To i
		//                        'コース情報がnullの場合
		//                        If CType(Me.grdList(uprow, CrsInfo_Koumoku.info), String) = String.Empty Then
		//                            'null地点からループ
		//                            For chnRow As Integer = uprow To i
		//                                '設定があった場合
		//                                If CType(Me.grdList(chnRow, CrsInfo_Koumoku.info), String) <> String.Empty Then
		//                                    'null行に設定を移動
		//                                    Me.grdList.SetData(uprow + nullRow, CrsInfo_Koumoku.info, Me.grdList(chnRow, CrsInfo_Koumoku.info))
		//                                    Me.grdList.SetData(uprow + nullRow, CrsInfo_Koumoku.updatekbn, UpDateKbn)

		//                                    '元設定にstring.emptyを挿入
		//                                    Me.grdList.SetData(chnRow, CrsInfo_Koumoku.info, String.Empty)
		//                                    Me.grdList.SetData(chnRow, CrsInfo_Koumoku.updatekbn, UpDateKbn)

		//                                    nullRow = nullRow + 1
		//                                End If
		//                            Next
		//                            '歯抜け処理が完了しているので、ループ処理を抜ける
		//                            Exit For
		//                        End If
		//                    Next
		//                End If
		//            End If

		//            ' 変更有無チェック
		//            If CType(Me.grdList(i, CrsInfo_Koumoku.updatekbn), String) = UpDateKbn Then
		//                '変更ありのフラグを構築
		//                upFlg = True
		//            End If
		//            '次行へ
		//            Continue For
		//        Else
		//            '変更があった場合
		//            If upFlg = True Then
		//                Dim nullRow As Integer = 0
		//                'レコードのスタート位置からループ
		//                For uprow As Integer = dataChg To i - 1
		//                    'コース情報がnullの場合
		//                    If CType(Me.grdList(uprow, CrsInfo_Koumoku.info), String) = String.Empty Then
		//                        'null地点からループ
		//                        For chnRow As Integer = uprow To i - 1
		//                            '設定があった場合
		//                            If CType(Me.grdList(chnRow, CrsInfo_Koumoku.info), String) <> String.Empty Then
		//                                'null行に設定を移動
		//                                Me.grdList.SetData(uprow + nullRow, CrsInfo_Koumoku.info, Me.grdList(chnRow, CrsInfo_Koumoku.info))
		//                                Me.grdList.SetData(uprow + nullRow, CrsInfo_Koumoku.updatekbn, UpDateKbn)

		//                                '元設定にstring.emptyを挿入
		//                                Me.grdList.SetData(chnRow, CrsInfo_Koumoku.info, String.Empty)
		//                                Me.grdList.SetData(chnRow, CrsInfo_Koumoku.updatekbn, UpDateKbn)

		//                                nullRow = nullRow + 1
		//                            End If
		//                        Next
		//                        '歯抜け処理が完了しているので、ループ処理を抜ける
		//                        Exit For
		//                    End If
		//                Next
		//            End If
		//            ' 変更有無チェック
		//            If CType(Me.grdList(i, CrsInfo_Koumoku.updatekbn), String) = UpDateKbn Then
		//                '変更ありのフラグを構築
		//                upFlg = True
		//            Else
		//                '初期値へ戻す
		//                upFlg = False
		//            End If
		//            '次のレコードのスタート位置を格納
		//            dataChg = i
		//        End If
		//    End If
		//Next
	}

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
				crsKey.SyuPtDay = Strings.Replace(System.Convert.ToString(grdList.GetData(i, CrsInfo_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);
				crsKey.GouSya = System.Convert.ToInt32(grdList.GetData(i, CrsInfo_Koumoku.gousya));

				// 使用中フラグ確認
				useFlg = checkUseFlg(PrmData, crsKey);
			}

			//日付と号車が同行な場合、処理スキップ
			if (System.Convert.ToString(this.grdList(i, CrsInfo_Koumoku.syuptday)) == System.Convert.ToString(this.grdList(i - 1, CrsInfo_Koumoku.syuptday)) &&)
			{
				System.Convert.ToDecimal(this.grdList(i, CrsInfo_Koumoku.gousya)) = System.Convert.ToDecimal(System.Convert.ToDecimal(this.grdList(i - 1, CrsInfo_Koumoku.gousya)));
			}
			else
			{
				// コースキーセット
				crsKey.CrsCd = System.Convert.ToString(txtCrsCd.Text.ToString());
				crsKey.SyuPtDay = Strings.Replace(System.Convert.ToString(grdList.GetData(i, CrsInfo_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);
				crsKey.GouSya = System.Convert.ToInt32(grdList.GetData(i, CrsInfo_Koumoku.gousya));

				// 使用中フラグ確認
				useFlg = checkUseFlg(PrmData, crsKey);
			}

			//使用中であるレコードへ設定
			if (useFlg == true)
			{
				this.grdList.Rows(i).AllowEditing = false;
				this.grdList[i, CrsInfo_Koumoku.henkoukahikbn] = Kahi;
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

}