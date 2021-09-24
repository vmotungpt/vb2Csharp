using C1.Win.C1FlexGrid;
using System.ComponentModel;



/// <summary>
/// コース台帳一括修正（受付開始日・予約停止・キャンセル料区分）
/// 受付開始日、予約停止、キャンセル料区分を変更する
/// </summary>
public class S03_0209 : PT14, iPT14
{

	#region  定数／変数宣言

	private DataTable selectOldData; //検索後のデータを保持
	public ResearchData PrmData //データ格納クラス
	{
		private string Kahi = "Y"; //グリッドの変更可否区分
	private string UpDateKbn = "Y"; //グリッドの更新区分
	private const int BlockCapacityMaxLength = 8; //定員最大バイト数
	private string StUketukeStartDay = "____/__/__"; //グリッドの変更可否区分
	private int TeikiKikakuKbnFlg = 0; //定期企画区分
	private bool uketukedayChangeFlg = false; //受付開始日変更フラグ





	#endregion
	#region プロパティ

	public string subTitle
	{

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
	private enum Uketuke_Koumoku : int
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
		[Value("受付開始日（ヶ月前）")]
		uketukestartkagetumae,
		[Value("受付開始日（日）")]
		uketukestartbi,
		[Value("受付開始日")]
		uketukestartday,
		[Value("予約停止")]
		yoyakustopflg,
		[Value("キャンセル料区分")]
		cancelryoukbn,
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

	/// <summary>
	/// グリッドセルチェンジイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void grdList_CellChanged(object sender, RowColEventArgs e)
	{

		//定期、受付開始日（ヶ月前）、受付開始日（日）でなければ処理を抜ける
		if (TeikiKikakuKbnFlg != FixedCd.TeikiKikakuKbn.Teiki ||)
		{
			!(e.Col == Uketuke_Koumoku.uketukestartkagetumae || e.Col == Uketuke_Koumoku.uketukestartbi);
			return;
		}

		//定期の場合、内部で算出
		if (this.grdList(e.Row, Uketuke_Koumoku.uketukestartbi).ToString() == string.Empty ||)
		{
			this.grdList(e.Row, Uketuke_Koumoku.uketukestartbi).ToString = System.Convert.ToString(0.ToString());
			//受付開始日(日)が空白またはNULLの場合
			DateTime dtSyuptday = System.Convert.ToDateTime(this.grdList(e.Row, Uketuke_Koumoku.syuptday));
			DateTime FirstDay = new DateTime(dtSyuptday.Year, dtSyuptday.Month, 1);
			DateTime ZenFirstDay = FirstDay.AddMonths(System.Convert.ToInt32(this.grdList(e.Row, Uketuke_Koumoku.uketukestartkagetumae)) * -1);
			int iDay = dtSyuptday.Day;
			DateTime KaFirstDay = ZenFirstDay.AddDays(iDay - 1);
			DateTime SoFirstDay = new DateTime(KaFirstDay.Year, KaFirstDay.Month, 1);

			if (ZenFirstDay.Month == KaFirstDay.Month)
			{
				this.grdList[e.Row, Uketuke_Koumoku.uketukestartday] = System.Convert.ToString(KaFirstDay);
			}
			else
			{
				this.grdList[e.Row, Uketuke_Koumoku.uketukestartday] = System.Convert.ToString(SoFirstDay);
			}
		}
		else
		{
			//受付開始日(日)が"1"または"2"の場合
			DateTime dtSyuptday = System.Convert.ToDateTime(this.grdList(e.Row, Uketuke_Koumoku.syuptday));
			DateTime FirstDay = new DateTime(dtSyuptday.Year, dtSyuptday.Month, 1);
			DateTime ZenFirstDay = FirstDay.AddMonths(System.Convert.ToInt32(this.grdList(e.Row, Uketuke_Koumoku.uketukestartkagetumae)) * -1);

			if (System.Convert.ToInt32(this.grdList(e.Row, Uketuke_Koumoku.uketukestartbi)) == FixedCd.UketukeStartBi.ONE)
			{
				this.grdList[e.Row, Uketuke_Koumoku.uketukestartday] = System.Convert.ToString(ZenFirstDay);
			}
			else if (System.Convert.ToInt32(this.grdList(e.Row, Uketuke_Koumoku.uketukestartbi)) == FixedCd.UketukeStartBi.Two)
			{
				this.grdList[e.Row, Uketuke_Koumoku.uketukestartday] = System.Convert.ToString(ZenFirstDay.AddDays(System.Convert.ToDouble(FixedCd.UketukeStartBi.ONE)));
			}
		}

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
			if (CheckUketukeUpdate(i) == true)
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

		//タイトル設定
		string titleName = "コース台帳一括修正（" + subTitle + "）";
		this.setTitle = titleName;

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

		//予約停止の表示設定
		this.CheckYoyakuStop();

		//定期・企画チェック
		this.CheckTeikiKikaku();

		//過去日付チェック
		this.CheckPastday();

		//使用中フラグチェック
		//Me.CheckUsingFlg()
		this.isUseCrsInfo();

		//受付開始日の空白変換
		this.CheckUketukeStart();

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
		bool DateInputFlg = false;

		//グリッドの背景色をクリア
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (this.grdList.Rows(i).AllowEditing == true)
			{
				this.grdList.GetCellRange(i, Uketuke_Koumoku.uketukestartkagetumae).StyleNew.BackColor = BackColorType.Standard;
				this.grdList.GetCellRange(i, Uketuke_Koumoku.uketukestartbi).StyleNew.BackColor = BackColorType.Standard;
				this.grdList.GetCellRange(i, Uketuke_Koumoku.uketukestartday).StyleNew.BackColor = BackColorType.Standard;
				this.grdList.GetCellRange(i, Uketuke_Koumoku.yoyakustopflg).StyleNew.BackColor = BackColorType.Standard;
				this.grdList.GetCellRange(i, Uketuke_Koumoku.cancelryoukbn).StyleNew.BackColor = BackColorType.Standard;
			}
		}

		//入力チェック
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			uketukedayChangeFlg = false;
			//入力差分チェック
			if (CheckUketukeUpdate(i) == true)
			{
				differenceFlg = true;
			}


			//受付開始日の変更があった場合
			if (uketukedayChangeFlg == true)
			{
				//日付入力チェック
				//定期・企画区分が企画のみ実施
				if (System.Convert.ToInt32(base.SearchResultGridData.Rows(0).ItemArray(8)) == FixedCd.TeikiKikakuKbn.Kikaku)
				{

					//現在日付取得
					DateTime dtNow = new DateTime();
					CommonDaProcess commonDaProcess = new CommonDaProcess();
					dtNow = System.Convert.ToDateTime(commonDaProcess.getDbSysTime.Rows(0).Item(0));

					if (checkUketukeStartDayInput(i, dtNow) == true)
					{
						DateInputFlg = true;
					}
				}
			}
		}

		//'日付入力チェック
		//'定期・企画区分が企画のみ実施
		//If CType(MyBase.SearchResultGridData.Rows(0).ItemArray(8), Integer) = FixedCd.TeikiKikakuKbn.Kikaku Then

		//    '現在日付取得
		//    Dim dtNow As New Date
		//    Dim commonDaProcess As New CommonDaProcess
		//    dtNow = CDate(commonDaProcess.getDbSysTime.Rows(0).Item(0))

		//    For i As Integer = 1 To Me.grdList.Rows.Count - 1
		//        If checkUketukeStartDayInput(i, dtNow) = True Then
		//            DateInputFlg = True
		//        End If
		//    Next
		//End If

		if (differenceFlg == false)
		{
			//入力に差分がない場合、エラーメッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_049");
			return RET_NONEXEC;
		}

		if (DateInputFlg == true)
		{
			//受付開始日が過去日付の場合、エラーメッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_013");
			return RET_NONEXEC;
		}

		msgParam = "更新";
		//メッセージ出力(更新します。よろしいですか？)
		if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", msgParam) == MsgBoxResult.Cancel)
		{
			//キャンセル
			return RET_CANCEL;
		}

		//更新処理実行
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.updatekbn)) == UpDateKbn)
			{
				if (ExecuteNonQuery(DbShoriKbn.Update, i) > 0)
				{
					//Me.grdList(i, Uketuke_Koumoku.usingflg) = FixedCd.UsingFlg.Unused
					returnValue = 1;
				}
			}
		}

		if (returnValue == 0)
		{
			return RET_NONEXEC;
			//Else
			//    '再検索のために当画面で使用中フラグを更新したデータは初期化する
			//    Me.ExecuteReturn()
		}

		//受付開始日の空白変換
		this.CheckUketukeStart();

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

	/// <summary>
	/// グリッドのLeaveCellイベント
	/// </summary>
	/// <remarks></remarks>
	private void grdList_LeaveCell(object sender, EventArgs e)
	{

		//グリッド範囲外は処理しない
		if (this.grdList.RowSel < 0 || this.grdList.ColSel < 0)
		{
			return;
		}
		else
		{
			if (this.grdList.ColSel == Uketuke_Koumoku.yoyakustopflg)
			{
				if (this.grdList.GetCellCheck(this.grdList.RowSel, this.grdList.ColSel) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
				{
					if (System.Convert.ToString(this.grdList(this.grdList.RowSel, this.grdList.ColSel)) == FixedCd.YoyakuStopFlg.Teishi)
					{
					}
					else
					{
						this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "0";
					}
				}
				else
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "1";
				}
			}
			else if (this.grdList.ColSel == Uketuke_Koumoku.uketukestartday)
			{
				if (Information.IsDBNull(this.grdList(this.grdList.RowSel, this.grdList.ColSel)) == true)
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = string.Empty;
				}
				else
				{
					if (System.Convert.ToString(this.grdList(this.grdList.RowSel, this.grdList.ColSel)) == StUketukeStartDay)
					{
						this.grdList[this.grdList.RowSel, this.grdList.ColSel] = string.Empty;
					}
				}
			}
		}
	}

	/// <summary>
	/// グリッドのチェックイベント
	/// </summary>
	/// <remarks></remarks>
	private void grdList_CellChecked(object sender, RowColEventArgs e)
	{

		//グリッド範囲外は処理しない
		if (this.grdList.RowSel < 0 || this.grdList.ColSel < 0)
		{
			return;
		}
		else
		{
			if (this.grdList.ColSel == Uketuke_Koumoku.yoyakustopflg)
			{
				if (this.grdList.GetCellCheck(this.grdList.RowSel, this.grdList.ColSel) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "0";
				}
				else
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "1";
				}
			}
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

		//数値拡張ユーザーコントロール
		DateEx UketukeStartDay = new DateEx();

		//受付開始日の表示フォーマットの設定
		GrapeCity.Win.Editors.Fields.DateYearField DateYearField1 = new GrapeCity.Win.Editors.Fields.DateYearField();
		GrapeCity.Win.Editors.Fields.DateLiteralField DateLiteralField1 = new GrapeCity.Win.Editors.Fields.DateLiteralField();
		GrapeCity.Win.Editors.Fields.DateMonthField DateMonthField1 = new GrapeCity.Win.Editors.Fields.DateMonthField();
		GrapeCity.Win.Editors.Fields.DateLiteralField DateLiteralField2 = new GrapeCity.Win.Editors.Fields.DateLiteralField();
		GrapeCity.Win.Editors.Fields.DateDayField DateDayField1 = new GrapeCity.Win.Editors.Fields.DateDayField();
		DateLiteralField1.Text = "/";
		DateLiteralField2.Text = "/";
		UketukeStartDay.ExistError = false;
		UketukeStartDay.Fields.AddRange(new GrapeCity.Win.Editors.Fields.DateField[] { DateYearField1, DateLiteralField1, DateMonthField1, DateLiteralField2, DateDayField1 });
		UketukeStartDay.HatoMode = false;
		UketukeStartDay.HighlightText = GrapeCity.Win.Editors.HighlightText.All;
		UketukeStartDay.ValidatorMode = false;
		UketukeStartDay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		UketukeStartDay.Font = new System.Drawing.Font(UketukeStartDay.Font.FontFamily, 11.25, UketukeStartDay.Font.Style);
		this.grdList.Cols(Uketuke_Koumoku.uketukestartday).Editor = UketukeStartDay;

		// カラムの並び替えを可能にするか (初期値：True)
		this.grdList.AllowDragging = AllowDraggingEnum.None;

		// グリッドがデータソースに連結された時に列を自動的に作成するか (初期値：True)
		this.grdList.AutoGenerateColumns = false;

		// セルにいつコンボボタンを表示するか (初期値：Inherit)
		this.grdList.ShowButtons = ShowButtonsEnum.Always;

		//コードマスタからコンボデータを取得する
		setGridInitialize_grdUketukeStartKagetumae();
		setGridInitialize_grdUketukeStartBi();
		setGridInitialize_grdCancelRyouKbn();

		//ヘッダタイトル・高さの設定
		this.grdList.Rows(0).Height = 40;
		this.grdList[0, Uketuke_Koumoku.uketukestartkagetumae] = "受付開始日" + "\r\n" + "（ヶ月前）";
		this.grdList[0, Uketuke_Koumoku.uketukestartbi] = "受付開始日" + "\r\n" + "（日）";
		this.grdList[0, Uketuke_Koumoku.cancelryoukbn] = "キャンセル料" + "\r\n" + "区分";

	}

	/// <summary>
	/// グリッド初期化（受付開始ヶ月前）
	/// </summary>
	private void setGridInitialize_grdUketukeStartKagetumae()
	{

		// Enum情報 取得
		DataTable dtUketukeStartKagetumae = CommonProcessKyushu.getComboboxDataOfDatatable(typeof(FixedCd.UketukeStartKagetumae));

		this.grdList.Cols(Uketuke_Koumoku.uketukestartkagetumae).DataType = typeof(string);
		this.grdList.Cols(Uketuke_Koumoku.uketukestartkagetumae).AllowEditing = true;
		this.grdList.Cols(Uketuke_Koumoku.uketukestartkagetumae).DataMap = CommonProcess.getComboboxData(dtUketukeStartKagetumae);

	}

	/// <summary>
	/// グリッド初期化（受付開始日）
	/// </summary>
	private void setGridInitialize_grdUketukeStartBi()
	{

		// Enum情報 取得
		DataTable dtUketukeStartBi = CommonProcessKyushu.getComboboxDataOfDatatable(typeof(FixedCd.UketukeStartBi));

		this.grdList.Cols(Uketuke_Koumoku.uketukestartbi).DataType = typeof(string);
		this.grdList.Cols(Uketuke_Koumoku.uketukestartbi).AllowEditing = true;
		this.grdList.Cols(Uketuke_Koumoku.uketukestartbi).DataMap = CommonProcess.getComboboxData(dtUketukeStartBi, true);

	}

	/// <summary>
	/// グリッド初期化（キャンセル料区分）
	/// </summary>
	private void setGridInitialize_grdCancelRyouKbn()
	{

		// Enum情報 取得
		DataTable dtCancelRyouKbn = CommonProcessKyushu.getComboboxDataOfDatatable(typeof(FixedCd.CancelRyouKbnType));

		this.grdList.Cols(Uketuke_Koumoku.cancelryoukbn).DataType = typeof(string);
		this.grdList.Cols(Uketuke_Koumoku.cancelryoukbn).AllowEditing = true;
		this.grdList.Cols(Uketuke_Koumoku.cancelryoukbn).DataMap = CommonProcess.getComboboxData(dtCancelRyouKbn);

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
	/// 予約停止の表示設定
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckYoyakuStop()
	{

		//予約停止の表示設定
		for (int i = 1; i <= base.SearchResultGridData.Rows.Count; i++)
		{
			if (Information.IsDBNull(System.Convert.ToString(base.SearchResultGridData.Rows(i - 1).ItemArray(12))) == true)
			{
				this.grdList[i, Uketuke_Koumoku.yoyakustopflg] = "0";
			}
			else if (System.Convert.ToString(base.SearchResultGridData.Rows(i - 1).ItemArray(12)) == FixedCd.YoyakuStopFlg.Teishi)
			{
				this.grdList[i, Uketuke_Koumoku.yoyakustopflg] = "1";
			}
			else
			{
				this.grdList[i, Uketuke_Koumoku.yoyakustopflg] = "0";
			}
		}
		return true;

	}

	/// <summary>
	/// 定期・企画チェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckTeikiKikaku()
	{

		//定期・企画によってヘッダの表示/非表示を設定
		if (System.Convert.ToInt32(base.SearchResultGridData.Rows(0).ItemArray(8)) == FixedCd.TeikiKikakuKbn.Teiki)
		{
			this.grdList.Cols(Uketuke_Koumoku.unkyukbn).Visible = true;
			this.grdList.Cols(Uketuke_Koumoku.saikoukakuteikbn).Visible = false;
			this.grdList.Cols(Uketuke_Koumoku.uketukestartkagetumae).Visible = true;
			this.grdList.Cols(Uketuke_Koumoku.uketukestartbi).Visible = true;
			this.grdList.Cols(Uketuke_Koumoku.uketukestartday).Visible = true;
			this.grdList.Cols(Uketuke_Koumoku.uketukestartday).AllowEditing = false;
			this.grdList.Cols(Uketuke_Koumoku.uketukestartday).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
			this.grdList.Cols(Uketuke_Koumoku.cancelryoukbn).Visible = false;
		}
		else if (System.Convert.ToInt32(base.SearchResultGridData.Rows(0).ItemArray(8)) == FixedCd.TeikiKikakuKbn.Kikaku)
		{
			this.grdList.Cols(Uketuke_Koumoku.unkyukbn).Visible = false;
			this.grdList.Cols(Uketuke_Koumoku.saikoukakuteikbn).Visible = true;
			this.grdList.Cols(Uketuke_Koumoku.uketukestartkagetumae).Visible = false;
			this.grdList.Cols(Uketuke_Koumoku.uketukestartbi).Visible = false;
			this.grdList.Cols(Uketuke_Koumoku.uketukestartday).Visible = true;
			this.grdList.Cols(Uketuke_Koumoku.cancelryoukbn).Visible = true;
		}
		TeikiKikakuKbnFlg = System.Convert.ToInt32(base.SearchResultGridData.Rows(0).ItemArray(8));
		return true;

	}

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
			if (CommonConvertUtil.ChkWhenDay(System.Convert.ToDateTime(this.grdList(i, Uketuke_Koumoku.syuptday)), CommonConvertUtil.WhenKako, dtNow) == true)
			{
				this.grdList.Rows(i).AllowEditing = false;
				this.grdList[i, Uketuke_Koumoku.henkoukahikbn] = Kahi;
				// 過去日であるデータ背景色をグレー
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
	//    Dim dataAccess As New Uketuke_DA
	//    Dim systemupdatepgmid As String = Me.Name
	//    Dim i As Integer = 1
	//    Dim FlgChk As Integer = 0

	//    '使用中フラグ更新
	//    dtUsingFlg = dataAccess.executeUsingFlgCrs(selectOldData, systemupdatepgmid)

	//    For Each row As DataRow In dtUsingFlg.Rows
	//        If CType(row("USING_FLG"), String) = FixedCd.UsingFlg.Use Then
	//            Me.grdList(i, Uketuke_Koumoku.usingflg) = FixedCd.UsingFlg.Use
	//        Else
	//            FlgChk = 1
	//            Me.grdList.Rows(i).AllowEditing = False
	//            Me.grdList(i, Uketuke_Koumoku.henkoukahikbn) = Kahi
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
				crsKey.SyuPtDay = Strings.Replace(System.Convert.ToString(grdList.GetData(i, Uketuke_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);
				crsKey.GouSya = System.Convert.ToInt32(grdList.GetData(i, Uketuke_Koumoku.gousya));

				// 使用中フラグ確認
				useFlg = checkUseFlg(PrmData, crsKey);
			}

			//日付と号車が同行な場合、処理スキップ
			if (System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.syuptday)) == System.Convert.ToString(this.grdList(i - 1, Uketuke_Koumoku.syuptday)) &&)
			{
				System.Convert.ToDecimal(this.grdList(i, Uketuke_Koumoku.gousya)) = System.Convert.ToDecimal(System.Convert.ToDecimal(this.grdList(i - 1, Uketuke_Koumoku.gousya)));
			}
			else
			{
				// コースキーセット
				crsKey.CrsCd = System.Convert.ToString(txtCrsCd.Text.ToString());
				crsKey.SyuPtDay = Strings.Replace(System.Convert.ToString(grdList.GetData(i, Uketuke_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);
				crsKey.GouSya = System.Convert.ToInt32(grdList.GetData(i, Uketuke_Koumoku.gousya));

				// 使用中フラグ確認
				useFlg = checkUseFlg(PrmData, crsKey);
			}

			//使用中であるレコードへ設定
			if (useFlg == true)
			{
				this.grdList.Rows(i).AllowEditing = false;
				this.grdList[i, Uketuke_Koumoku.henkoukahikbn] = Kahi;
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

	/// <summary>
	/// 受付開始日の空白変換処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckUketukeStart()
	{

		int TeikiKikaku = System.Convert.ToInt32(base.SearchResultGridData.Rows(0).ItemArray(8));

		//受付開始日の空白変換
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (TeikiKikaku == FixedCd.TeikiKikakuKbn.Teiki)
			{
				if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.uketukestartbi)) == true ||)
				{
					this.grdList(i, Uketuke_Koumoku.uketukestartbi).Equals(string.Empty);
					this.grdList[i, Uketuke_Koumoku.uketukestartbi] = string.Empty;
				}
				else if (System.Convert.ToDecimal(this.grdList(i, Uketuke_Koumoku.uketukestartbi)) == 0)
				{
					this.grdList[i, Uketuke_Koumoku.uketukestartbi] = string.Empty;
				}
				if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.uketukestartday)) == true)
				{
					this.grdList[i, Uketuke_Koumoku.uketukestartday] = string.Empty;
				}
			}
			else if (TeikiKikaku == FixedCd.TeikiKikakuKbn.Kikaku)
			{
				if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.uketukestartday)) == true)
				{
					this.grdList[i, Uketuke_Koumoku.uketukestartday] = string.Empty;
				}
			}
		}
		return true;

	}

	/// <summary>
	/// 更新処理前のチェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckUketukeUpdate(int i)
	{

		string UKETUKESTARTKAGETUMAE = string.Empty;
		string UKETUKESTARTBI = string.Empty;
		string UKETUKESTARTDAY = string.Empty;
		string YOYAKUSTOPFLG = string.Empty;
		string CANCELRYOUKBN = string.Empty;
		string TEIKIKIKAKUKBN = string.Empty;
		string NEWYOYAKUSTOPFLG = string.Empty;

		//入力不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		//引数の設定
		foreach (DataRow row in selectOldData.Rows)
		{
			if (row["SYUPT_DAY"].Equals(this.grdList(i, Uketuke_Koumoku.syuptday)) &&)
			{
				row["HAISYA_KEIYU_CD_1"].ToString().Equals(this.grdList(i, Uketuke_Koumoku.haisyakeiyucd1)) &&;
				row["GOUSYA"].Equals(this.grdList(i, Uketuke_Koumoku.gousya)) &&;
				row["SYUPT_TIME_1"].Equals(this.grdList(i, Uketuke_Koumoku.syupttime1));
				UKETUKESTARTKAGETUMAE = row["UKETUKE_START_KAGETUMAE"].ToString();
				if (row["UKETUKE_START_BI"].ToString() == 0.ToString())
				{
					UKETUKESTARTBI = string.Empty;
				}
				else
				{
					UKETUKESTARTBI = row["UKETUKE_START_BI"].ToString();
				}
				UKETUKESTARTDAY = row["UKETUKE_START_DAY"].ToString();
				YOYAKUSTOPFLG = row["YOYAKU_STOP_FLG"].ToString();
				if (YOYAKUSTOPFLG == "0")
				{
					YOYAKUSTOPFLG = string.Empty;
				}
				CANCELRYOUKBN = row["CANCEL_RYOU_KBN"].ToString();
				TEIKIKIKAKUKBN = row["TEIKI_KIKAKU_KBN"].ToString();
			}
		}

		if (System.Convert.ToInt32(TEIKIKIKAKUKBN) == FixedCd.TeikiKikakuKbn.Teiki)
		{
			//受付開始ヶ月
			if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.uketukestartkagetumae)) == true)
			{
				this.grdList[i, Uketuke_Koumoku.uketukestartkagetumae] = string.Empty;
			}
			if (UKETUKESTARTKAGETUMAE.Equals(System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.uketukestartkagetumae))) == false)
			{
				this.grdList[i, Uketuke_Koumoku.updatekbn] = UpDateKbn;
				uketukedayChangeFlg = true;
				return true;
			}
			//受付開始（日）
			if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.uketukestartbi)) == true)
			{
				this.grdList[i, Uketuke_Koumoku.uketukestartbi] = string.Empty;
			}
			if (UKETUKESTARTBI.Equals(System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.uketukestartbi))) == false)
			{
				this.grdList[i, Uketuke_Koumoku.updatekbn] = UpDateKbn;
				uketukedayChangeFlg = true;
				return true;
			}
		}
		else if (System.Convert.ToInt32(TEIKIKIKAKUKBN) == FixedCd.TeikiKikakuKbn.Kikaku)
		{
			//受付開始日
			if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.uketukestartday)) == true)
			{
				this.grdList[i, Uketuke_Koumoku.uketukestartday] = string.Empty;
			}
			if (UKETUKESTARTDAY.Equals(System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.uketukestartday))) == false)
			{
				this.grdList[i, Uketuke_Koumoku.updatekbn] = UpDateKbn;
				uketukedayChangeFlg = true;
				return true;
			}
			//キャンセル料区分
			if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.cancelryoukbn)) == true)
			{
				this.grdList[i, Uketuke_Koumoku.cancelryoukbn] = string.Empty;
			}
			if (CANCELRYOUKBN.Equals(System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.cancelryoukbn))) == false)
			{
				this.grdList[i, Uketuke_Koumoku.updatekbn] = UpDateKbn;
				return true;
			}
		}
		//予約停止チェックボックス値から予約停止フラグに変換
		if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.yoyakustopflg)) == true)
		{
			NEWYOYAKUSTOPFLG = string.Empty;
		}
		else if (System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.yoyakustopflg)) == "0")
		{
			NEWYOYAKUSTOPFLG = string.Empty;
		}
		else if (System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.yoyakustopflg)) == "1")
		{
			NEWYOYAKUSTOPFLG = System.Convert.ToString(FixedCd.YoyakuStopFlg.Teishi);
		}
		//予約停止
		if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.yoyakustopflg)) == true)
		{
			this.grdList[i, Uketuke_Koumoku.yoyakustopflg] = string.Empty;
		}
		if (YOYAKUSTOPFLG.Equals(NEWYOYAKUSTOPFLG) == false)
		{
			this.grdList[i, Uketuke_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		return false;

	}

	/// <summary>
	/// 受付開始日入力チェック処理
	/// </summary>
	/// <param name="i">行数</param>
	/// <returns>True:不可あり False:不可なし</returns>
	private bool checkUketukeStartDayInput(int i, DateTime dtNow)
	{

		//入力不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		//更新区分が"Y"ではない行は読み飛ばす
		if (System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.updatekbn)) != UpDateKbn)
		{
			return false;
		}

		DateTime dt;
		//入力した値が日付か確認
		if (DateTime.TryParse(System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.uketukestartday)), out dt) == true)
		{
			//過去日付チェック
			//出発日＜システム日付の場合は入力不可とする
			if (CommonConvertUtil.ChkWhenDay(System.Convert.ToDateTime(this.grdList(i, Uketuke_Koumoku.uketukestartday)), CommonConvertUtil.WhenKako, dtNow) == true)
			{
				//グリッドの背景色を変更
				this.grdList.GetCellRange(i, Uketuke_Koumoku.uketukestartday).StyleNew.BackColor = BackColorType.InputError;
				return true;
			}
		}
		else
		{
			if (this.grdList(i, Uketuke_Koumoku.uketukestartday).Equals(string.Empty))
			{
			}
			else
			{
				//グリッドの背景色を変更
				this.grdList.GetCellRange(i, Uketuke_Koumoku.uketukestartday).StyleNew.BackColor = BackColorType.InputError;
				return true;
			}
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
		Uketuke_DA dataAccess = new Uketuke_DA();

		if (DbShoriKbn.Update.Equals(kbn))
		{
			//パラメータ設定
			paramInfoList.Add("SYUPT_DAY", Strings.Replace(System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));
			paramInfoList.Add("CRS_CD", this.txtCrsCd.Text);
			paramInfoList.Add("GOUSYA", this.grdList(i, Uketuke_Koumoku.gousya));
			//paramInfoList.Add("USING_FLG", FixedCd.UsingFlg.Unused)
			paramInfoList.Add("UKETUKE_START_KAGETUMAE", this.grdList(i, Uketuke_Koumoku.uketukestartkagetumae));
			if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.uketukestartbi)) == true ||)
			{
				this.grdList(i, Uketuke_Koumoku.uketukestartbi).Equals(string.Empty);
				paramInfoList.Add("UKETUKE_START_BI", 0);
			}
			else
			{
				paramInfoList.Add("UKETUKE_START_BI", this.grdList(i, Uketuke_Koumoku.uketukestartbi));
			}
			//Select Case TeikiKikakuKbnFlg
			//    Case FixedCd.TeikiKikakuKbn.Teiki
			//        '定期の場合のグリッド上に受付開始日がないため、内部で算出
			//        If Me.grdList(i, Uketuke_Koumoku.uketukestartbi).ToString = String.Empty OrElse
			//            Me.grdList(i, Uketuke_Koumoku.uketukestartbi).ToString = 0.ToString Then
			//            '受付開始日(日)が空白またはNULLの場合
			//            Dim dtSyuptday As DateTime = CType(Me.grdList(i, Uketuke_Koumoku.syuptday), DateTime)
			//            Dim FirstDay As DateTime = New DateTime(dtSyuptday.Year, dtSyuptday.Month, 1)
			//            Dim ZenFirstDay As DateTime = FirstDay.AddMonths(CType(Me.grdList(i, Uketuke_Koumoku.uketukestartkagetumae), Integer) * -1)
			//            Dim iDay As Integer = dtSyuptday.Day
			//            Dim KaFirstDay As DateTime = ZenFirstDay.AddDays(iDay - 1)
			//            Dim SoFirstDay As DateTime = New DateTime(KaFirstDay.Year, KaFirstDay.Month, 1)

			//            If ZenFirstDay.Month = KaFirstDay.Month Then
			//                paramInfoList.Add("UKETUKE_START_DAY", Replace(CType(KaFirstDay, String), "/", ""))
			//            Else
			//                paramInfoList.Add("UKETUKE_START_DAY", Replace(CType(SoFirstDay, String), "/", ""))
			//            End If
			//        Else
			//            '受付開始日(日)が"1"または"2"の場合
			//            Dim dtSyuptday As DateTime = CType(Me.grdList(i, Uketuke_Koumoku.syuptday), DateTime)
			//            Dim FirstDay As DateTime = New DateTime(dtSyuptday.Year, dtSyuptday.Month, 1)
			//            Dim ZenFirstDay As DateTime = FirstDay.AddMonths(CType(Me.grdList(i, Uketuke_Koumoku.uketukestartkagetumae), Integer) * -1)

			//            Select Case CType(Me.grdList(i, Uketuke_Koumoku.uketukestartbi), Integer)
			//                Case FixedCd.UketukeStartBi.ONE
			//                    paramInfoList.Add("UKETUKE_START_DAY", Replace(CType(ZenFirstDay, String), "/", ""))
			//                Case FixedCd.UketukeStartBi.Two
			//                    paramInfoList.Add("UKETUKE_START_DAY", Replace(CType(ZenFirstDay.AddDays(FixedCd.UketukeStartBi.ONE), String), "/", ""))
			//            End Select
			//        End If
			//    Case FixedCd.TeikiKikakuKbn.Kikaku
			//        '企画の場合のグリッド上の受付開始日
			if (Information.IsDBNull(this.grdList(i, Uketuke_Koumoku.uketukestartday)) == true ||)
			{
				this.grdList(i, Uketuke_Koumoku.uketukestartday).Equals(string.Empty);
				paramInfoList.Add("UKETUKE_START_DAY", 0);
			}
			else
			{
				paramInfoList.Add("UKETUKE_START_DAY", Strings.Replace(this.grdList(i, Uketuke_Koumoku.uketukestartday).ToString(), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));
			}
			//End Select
			if (this.grdList.GetData(i, Uketuke_Koumoku.yoyakustopflg) IsNot DBNull.Value)
					{
				if (System.Convert.ToString(this.grdList(i, Uketuke_Koumoku.yoyakustopflg)) == "1")
				{
					paramInfoList.Add("YOYAKU_STOP_FLG", FixedCd.YoyakuStopFlg.Teishi);
				}
				else
				{
					paramInfoList.Add("YOYAKU_STOP_FLG", null);
				}
			}
			if (this.grdList.GetData(i, Uketuke_Koumoku.cancelryoukbn) IsNot DBNull.Value)
					{
				paramInfoList.Add("CANCEL_RYOU_KBN", this.grdList(i, Uketuke_Koumoku.cancelryoukbn));
			}
			paramInfoList.Add("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime());
			paramInfoList.Add("SYSTEM_UPDATE_PGMID", this.Name);
			paramInfoList.Add("SYSTEM_UPDATE_PERSON_CD", UserInfoManagement.userId);
			//Else
			//    '引数の設定
			//    For Each row As DataRow In selectOldData.Rows
			//        If row("SYUPT_DAY").Equals(Me.grdList(i, Uketuke_Koumoku.syuptday)) AndAlso
			//                    row("HAISYA_KEIYU_CD_1").ToString.Equals(Me.grdList(i, Uketuke_Koumoku.haisyakeiyucd1)) AndAlso
			//                    row("GOUSYA").Equals(Me.grdList(i, Uketuke_Koumoku.gousya)) AndAlso
			//                    row("SYUPT_TIME_1").Equals(Me.grdList(i, Uketuke_Koumoku.syupttime1)) Then
			//            If row("USING_FLG").ToString.Equals(Me.grdList(i, Uketuke_Koumoku.usingflg)) Then
			//                Return returnValue
			//            ElseIf row("USING_FLG").ToString = String.empty AndAlso
			//                IsDBNull(Me.grdList(i, Uketuke_Koumoku.usingflg)) = True Then
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
				returnValue = System.Convert.ToInt32(dataAccess.executeUketukeTehai(Uketuke_DA.accessType.executeUpdateUketuke, paramInfoList));
				//Else
				//    'Returnの実施
				//    returnValue = dataAccess.executeUketukeTehai(Uketuke_DA.accessType.executeReturnUketuke, paramInfoList)
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
		totalValue.Columns.Add("UKETUKE_START_KAGETUMAE"); //受付開始ヶ月前
		totalValue.Columns.Add("UKETUKE_START_BI"); //受付開始日
		totalValue.Columns.Add("UKETUKE_START_DAY"); //受付開始日
		totalValue.Columns.Add("YOYAKU_STOP_FLG"); //予約停止フラグ
		totalValue.Columns.Add("CANCEL_RYOU_KBN"); //キャンセル料区分
		totalValue.Columns.Add("SYSTEM_UPDATE_DAY"); //システム更新日
		totalValue.Columns.Add("SYSTEM_UPDATE_PERSON_CD"); //システム更新者コード
		totalValue.Columns.Add("SYSTEM_UPDATE_PGMID"); //システム更新ＰＧＭＩＤ
		totalValue.Columns.Add("USING_FLG"); //使用中フラグ
		totalValue.Columns.Add("HENKOU_KAHI_KBN"); //変更可否区分
		totalValue.Columns.Add("UPDATE_KBN"); //更新区分
		totalValue.Columns.Add("HIDDEN_GOUSYA"); //号車（ソートキー用３桁補正）

		//DataAccessクラス生成
		Uketuke_DA dataAccess = new Uketuke_DA();

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
				returnValue = dataAccess.accessUketukeTehai(Uketuke_DA.accessType.getUketuke, paramInfoList);

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
					row3["UKETUKE_START_KAGETUMAE"] = row2["UKETUKE_START_KAGETUMAE"];
					row3["UKETUKE_START_BI"] = row2["UKETUKE_START_BI"];
					row3["UKETUKE_START_DAY"] = row2["UKETUKE_START_DAY"];
					row3["YOYAKU_STOP_FLG"] = row2["YOYAKU_STOP_FLG"];
					row3["CANCEL_RYOU_KBN"] = row2["CANCEL_RYOU_KBN"];
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