using C1.Win.C1FlexGrid;
using System.ComponentModel;



/// <summary>
/// コース台帳一括修正（ルーム数等）
/// 1名参加・相部屋・定員・ルーム数・室数を変更する画面
/// </summary>
public class S03_0208 : PT14, iPT14
{

	#region  定数／変数宣言

	private DataTable selectOldData; //検索後のデータを保持
	public ResearchData PrmData //データ格納クラス
	{
		private string Kahi = "Y"; //グリッドの変更可否区分
	private string UpDateKbn = "Y"; //グリッドの更新区分
	private const int BlockCapacityMaxLength = 5; //定員最大バイト数
	private const int BlockRoomMaxLength = 3; //コースブロック数最大バイト数

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
	private enum Room_Koumoku : int
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
		[Value("1名参加")]
		onesankaflg,
		[Value("相部屋")]
		aibeyauseflg,
		[Value("定員制")]
		teiinseiflg,
		[Value("定員")]
		crsblockcapacity,
		[Value("ルーム数_ブロック数")]
		crsblockroomnum,
		[Value("ルーム数_予約済数")]
		roomingtotal,
		[Value("1名1室_ブロック数")]
		crsblockone1r,
		[Value("1名1室_予約済数")]
		roomingbetuninzu1,
		[Value("1名1室_受止")]
		uketomekbnone1r,
		[Value("2名1室_ブロック数")]
		crsblocktwo1r,
		[Value("2名1室_予約済数")]
		roomingbetuninzu2,
		[Value("2名1室_受止")]
		uketomekbntwo1r,
		[Value("3名1室_ブロック数")]
		crsblockthree1r,
		[Value("3名1室_予約済数")]
		roomingbetuninzu3,
		[Value("3名1室_受止")]
		uketomekbnthree1r,
		[Value("4名1室_ブロック数")]
		crsblockfour1r,
		[Value("4名1室_予約済数")]
		roomingbetuninzu4,
		[Value("4名1室_受止")]
		uketomekbnfour1r,
		[Value("5名1室_ブロック数")]
		crsblockfive1r,
		[Value("5名1室_予約済数")]
		roomingbetuninzu5,
		[Value("5名1室_受止")]
		uketomekbnfive1r,
		[Value("使用中フラグ")]
		usingflg,
		[Value("変更可否")]
		henkoukahikbn,
		[Value("更新区分")]
		updatekbn,
		[Value("ブロック確保数")]
		blockkakuhonum,
		[Value("営ブロック定")]
		eiblocktei,
		[Value("営ブロック補")]
		eiblockho,
		[Value("空席確保数")]
		kusekikakuhonum,
		[Value("予約数")]
		yoyakunum

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
		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CheckRoomUpdate(i) == true)
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
		//Me.CheckUsingFlg()
		this.isUseCrsInfo();

		//1名参加・定員制チェック
		this.CheckTeiinOnesanka();

		//初期値設定
		this.ReservedRoomQuery(base.SearchResultGridData);

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
		Hashtable paramJyosyaList = new Hashtable();
		bool RoomSuFlg = false;
		bool RoomReserveSuSuFlg = false;
		bool RoomReserveSuFlg = false;
		bool ReserveTeisekiFlg = false;
		bool differenceFlg = false;
		bool PropertyFlg = false;

		//更新対象項目の初期化
		initUpdateAreaItems();

		//グリッドの背景色をクリア
		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (this.grdList.Rows(i).AllowEditing == true)
			{
				//定員制である場合、定員数のみ初期化
				if (this.grdList(i, Room_Koumoku.teiinseiflg).ToString() == "1")
				{
					this.grdList.GetCellRange(i, Room_Koumoku.crsblockcapacity).StyleNew.BackColor = BackColorType.Standard;
				}
				else
				{
					//1名参加が可である場合のみ1名の項目を初期化
					if (this.grdList(i, Room_Koumoku.onesankaflg).ToString() == "1")
					{
						this.grdList.GetCellRange(i, Room_Koumoku.crsblockone1r).StyleNew.BackColor = BackColorType.Standard;
						this.grdList.GetCellRange(i, Room_Koumoku.roomingbetuninzu1).StyleNew.BackColor = BackColorType.Standard;
					}
					this.grdList.GetCellRange(i, Room_Koumoku.crsblockroomnum).StyleNew.BackColor = BackColorType.Standard;
					this.grdList.GetCellRange(i, Room_Koumoku.roomingtotal).StyleNew.BackColor = BackColorType.Standard;
					this.grdList.GetCellRange(i, Room_Koumoku.crsblocktwo1r).StyleNew.BackColor = BackColorType.Standard;
					this.grdList.GetCellRange(i, Room_Koumoku.roomingbetuninzu2).StyleNew.BackColor = BackColorType.Standard;
					this.grdList.GetCellRange(i, Room_Koumoku.crsblockthree1r).StyleNew.BackColor = BackColorType.Standard;
					this.grdList.GetCellRange(i, Room_Koumoku.roomingbetuninzu3).StyleNew.BackColor = BackColorType.Standard;
					this.grdList.GetCellRange(i, Room_Koumoku.crsblockfour1r).StyleNew.BackColor = BackColorType.Standard;
					this.grdList.GetCellRange(i, Room_Koumoku.roomingbetuninzu4).StyleNew.BackColor = BackColorType.Standard;
					this.grdList.GetCellRange(i, Room_Koumoku.crsblockfive1r).StyleNew.BackColor = BackColorType.Standard;
					this.grdList.GetCellRange(i, Room_Koumoku.roomingbetuninzu5).StyleNew.BackColor = BackColorType.Standard;
				}
			}
		}

		//入力差分チェック
		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CheckRoomUpdate(i) == true)
			{
				differenceFlg = true;
			}
		}

		//ルーム数と室数の依存関係チェック
		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CheckRoomSu(i) == true)
			{
				RoomSuFlg = true;
			}
		}

		//ルーム予約数とルーム数の依存関係チェック
		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CheckRoomReserveSu(i) == true)
			{
				RoomReserveSuSuFlg = true;
			}
		}

		//定員の依存関係チェック
		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CheckTeiin(i) == true)
			{
				RoomReserveSuFlg = true;
			}
		}

		//予約数定席（+補助席）と定員の依存関係チェック
		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CheckReserveTeiseki(i) == true)
			{
				ReserveTeisekiFlg = true;
			}
		}

		if (differenceFlg == false)
		{
			//入力に差分がない場合、エラーメッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_049");
			return RET_NONEXEC;
		}

		if (RoomSuFlg == true)
		{
			//ルーム数と室数の依存関係が不正な場合、エラーメッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_052", "ルーム数", "室数");
			return RET_NONEXEC;
		}

		if (RoomReserveSuSuFlg == true)
		{
			//ルーム予約数とルーム数の依存関係が不正な場合、エラーメッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E03_002");
			return RET_NONEXEC;
		}

		if (RoomReserveSuFlg == true)
		{
			msgParam = "定員数";
			//定員の依存関係が不正な場合、エラーメッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_016", msgParam);
			return RET_NONEXEC;
		}

		if (ReserveTeisekiFlg == true)
		{
			//予約数定席（+補助席）と定員の依存関係が不正な場合、エラーメッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E03_002");
			return RET_NONEXEC;
		}

		//メッセージ出力(更新します。よろしいですか？)
		msgParam = "更新";
		if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", msgParam) == MsgBoxResult.Cancel)
		{
			//キャンセル
			return RET_CANCEL;
		}

		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (System.Convert.ToString(this.grdList(i, Room_Koumoku.updatekbn)) == UpDateKbn)
			{
				//部屋残数の算出
				paramJyosyaList = RoomZanCalculationQuery(i);

				//更新処理実行
				if (ExecuteNonQuery(DbShoriKbn.Update, i, paramJyosyaList) > 0)
				{
					//Me.grdList(i, Room_Koumoku.usingflg) = FixedCd.UsingFlg.Unused
					returnValue = 1;
				}
			}
		}

		if (returnValue == 0)
		{
			return RET_NONEXEC;
			//Else
			//    '再検索のために当画面で使用中フラグを更新したデータは初期化する
			//    For i As Integer = 2 To Me.grdList.Rows.Count - 1
			//        '更新処理実行
			//        ExecuteNonQuery(DbShoriKbn.Insert, i, paramJyosyaList)
			//    Next
		}

		return returnValue;
	}

	/// <summary>
	/// 戻しデータを取得
	/// </summary>
	/// <remarks></remarks>
	protected override void ExecuteReturn()
	{

		//Dim paramJyosyaList As New Hashtable
		//For i As Integer = 2 To Me.grdList.Rows.Count - 1
		//    '更新処理実行
		//    ExecuteNonQuery(DbShoriKbn.Insert, i, paramJyosyaList)
		//Next
	}

	/// <summary>
	/// グリッドのBeforeEditイベント
	/// </summary>
	/// <remarks></remarks>
	private void grdList_BeforeEdit(object sender, RowColEventArgs e)
	{

		//予約済数の入力不可設定
		if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.roomingtotal) == true))
		{
			e.Cancel = true;
		}
		if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.roomingbetuninzu1) == true))
		{
			e.Cancel = true;
		}
		if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.roomingbetuninzu2) == true))
		{
			e.Cancel = true;
		}
		if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.roomingbetuninzu3) == true))
		{
			e.Cancel = true;
		}
		if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.roomingbetuninzu4) == true))
		{
			e.Cancel = true;
		}
		if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.roomingbetuninzu5) == true))
		{
			e.Cancel = true;
		}

		//１名参加入力可否チェック
		if (this.grdList.GetCellCheck(this.grdList.RowSel, Room_Koumoku.onesankaflg) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
		{
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockone1r) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnone1r) == true))
			{
				e.Cancel = true;
			}
		}
		else
		{
			if (this.grdList.GetCellCheck(this.grdList.RowSel, Room_Koumoku.teiinseiflg) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
			{
				if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockone1r) == true))
				{
					e.Cancel = true;
				}
				if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnone1r) == true))
				{
					e.Cancel = true;
				}
			}
			else
			{
				if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockone1r) == true))
				{
					e.Cancel = false;
				}
				if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnone1r) == true))
				{
					e.Cancel = false;
				}
			}
		}

		//定員制入力可否チェック
		if (this.grdList.GetCellCheck(this.grdList.RowSel, Room_Koumoku.teiinseiflg) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
		{
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockcapacity) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockroomnum) == true))
			{
				e.Cancel = false;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblocktwo1r) == true))
			{
				e.Cancel = false;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbntwo1r) == true))
			{
				e.Cancel = false;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockthree1r) == true))
			{
				e.Cancel = false;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnthree1r) == true))
			{
				e.Cancel = false;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockfour1r) == true))
			{
				e.Cancel = false;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnfour1r) == true))
			{
				e.Cancel = false;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockfive1r) == true))
			{
				e.Cancel = false;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnfive1r) == true))
			{
				e.Cancel = false;
			}
			if (this.grdList.GetCellCheck(this.grdList.RowSel, Room_Koumoku.onesankaflg) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
			{
				if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockone1r) == true))
				{
					e.Cancel = true;
				}
				if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnone1r) == true))
				{
					e.Cancel = true;
				}
			}
			else
			{
				if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockone1r) == true))
				{
					e.Cancel = false;
				}
				if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnone1r) == true))
				{
					e.Cancel = false;
				}
			}
		}
		else
		{
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockcapacity) == true))
			{
				e.Cancel = false;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockroomnum) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockone1r) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnone1r) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblocktwo1r) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbntwo1r) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockthree1r) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnthree1r) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockfour1r) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnfour1r) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.crsblockfive1r) == true))
			{
				e.Cancel = true;
			}
			if ((e.Row.Equals(this.grdList.RowSel) == true) && (e.Col.Equals(Room_Koumoku.uketomekbnfive1r) == true))
			{
				e.Cancel = true;
			}
		}
	}

	/// <summary>
	/// グリッドのロストイベント
	/// </summary>
	/// <remarks></remarks>
	private void grdList_LeaveCell(object sender, EventArgs e)
	{

		//グリッド範囲外は処理しない
		//編集不可となっている行も処理しない
		if (this.grdList.RowSel < 0 || this.grdList.ColSel < 0)
		{
			return;
		}
		else if (this.grdList.Rows(this.grdList.RowSel).AllowEditing == false)
		{
			return;
		}
		else
		{
			if (((this.grdList.ColSel == Room_Koumoku.onesankaflg) || (this.grdList.ColSel == Room_Koumoku.aibeyauseflg)) || (this.grdList.ColSel == ))
			{
				Room_Koumoku.uketomekbnone1r(, Room_Koumoku.uketomekbntwo1r, Room_Koumoku.uketomekbnthree1r,);
				Room_Koumoku.uketomekbnfour1r(, Room_Koumoku.uketomekbnfive1r);
				if (this.grdList.GetCellCheck(this.grdList.RowSel, this.grdList.ColSel) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "0";
				}
				else
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "1";
				}
			}
			else if ((((this.grdList.ColSel == Room_Koumoku.crsblockcapacity) || (this.grdList.ColSel == Room_Koumoku.crsblockroomnum)) || (this.grdList.ColSel == Room_Koumoku.crsblockone1r)) || (this.grdList.ColSel == ))
			{
				Room_Koumoku.crsblocktwo1r(, Room_Koumoku.crsblockthree1r, Room_Koumoku.crsblockfour1r,);
				Room_Koumoku.crsblockfive1r;
				//空欄となった場合、0を補完する
				if (ReferenceEquals(this.grdList(this.grdList.RowSel, this.grdList.ColSel), DBNull.Value) || this.grdList(this.grdList.RowSel, this.grdList.ColSel).ToString() == string.Empty)
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = 0;
				}
			}
		}
	}

	/// <summary>
	/// グリッドの編集イベント
	/// </summary>
	/// <remarks></remarks>
	private void grdList_ValidateEdit(object sender, ValidateEditEventArgs e)
	{
		//グリッド範囲外は処理しない
		//編集不可となっている行も処理しない
		if (this.grdList.RowSel < 0 || this.grdList.ColSel < 0)
		{
			return;
		}
		else if (this.grdList.Rows(this.grdList.RowSel).AllowEditing == false)
		{
			return;
		}
		else
		{
			if (this.grdList.ColSel == Room_Koumoku.crsblockcapacity)
			{
				string Set_teiin = System.Convert.ToString(grdList.Editor.Text);
				//0より小さい、または999より大きい値が入力された場合、エラーメッセージを出力する
				if (System.Convert.ToInt32(Set_teiin) < 0 ||)
				{
					System.Convert.ToInt32(Set_teiin) > 999;
					CommonProcess.createFactoryMsg().messageDisp("E90_066", "1", "999", "定員");
					e.Cancel = true;
				}
			}
		}
	}


	/// <summary>
	/// グリッドのチェックイベント
	/// </summary>
	/// <remarks></remarks>
	private void grdList_CellChecked(object sender, EventArgs e)
	{
		//各セル範囲の定義
		//ルーム総計設定のセル範囲の定義
		CellRange roomBlc = this.grdList.GetCellRange(this.grdList.RowSel, Room_Koumoku.crsblockroomnum, this.grdList.RowSel, Room_Koumoku.roomingtotal);
		//1名1室
		CellRange onesanka = this.grdList.GetCellRange(this.grdList.RowSel, Room_Koumoku.crsblockone1r, this.grdList.RowSel, Room_Koumoku.uketomekbnone1r);
		//2名1室
		CellRange twoRoom = this.grdList.GetCellRange(this.grdList.RowSel, Room_Koumoku.crsblocktwo1r, this.grdList.RowSel, Room_Koumoku.uketomekbntwo1r);
		//3名1室
		CellRange threeRoom = this.grdList.GetCellRange(this.grdList.RowSel, Room_Koumoku.crsblockthree1r, this.grdList.RowSel, Room_Koumoku.uketomekbnthree1r);
		//4名1室
		CellRange fourRoom = this.grdList.GetCellRange(this.grdList.RowSel, Room_Koumoku.crsblockfour1r, this.grdList.RowSel, Room_Koumoku.uketomekbnfour1r);
		//5名1室
		CellRange fiveRoom = this.grdList.GetCellRange(this.grdList.RowSel, Room_Koumoku.crsblockfive1r, this.grdList.RowSel, Room_Koumoku.uketomekbnfive1r);
		//定員数
		CellRange teiinCap = this.grdList.GetCellRange(this.grdList.RowSel, Room_Koumoku.crsblockcapacity);
		//グリッド範囲外は処理しない
		if (this.grdList.RowSel < 0 || this.grdList.ColSel < 0)
		{
			return;
		}
		else
		{
			if (((this.grdList.ColSel == Room_Koumoku.aibeyauseflg) || (this.grdList.ColSel == Room_Koumoku.uketomekbnone1r)) || (this.grdList.ColSel == ))
			{
				Room_Koumoku.uketomekbntwo1r(, Room_Koumoku.uketomekbnthree1r,);
				Room_Koumoku.uketomekbnfour1r(, Room_Koumoku.uketomekbnfive1r);
				if (this.grdList.GetCellCheck(this.grdList.RowSel, this.grdList.ColSel) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "0";
				}
				else
				{
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "1";
				}
			}
			else if (this.grdList.ColSel == Room_Koumoku.onesankaflg)
			{
				//1名参加のチェックを外した場合、1名1室のブロック数を0とする
				if (this.grdList.GetCellCheck(this.grdList.RowSel, this.grdList.ColSel) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
				{
					this.grdList[this.grdList.RowSel, Room_Koumoku.crsblockone1r] = 0;

					//1名参加フラグの値を0（チェックなし）とする
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "0";

					//定員制にチェックがついていない場合
					if (this.grdList.GetCellCheck(this.grdList.RowSel, Room_Koumoku.teiinseiflg) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
					{
						//1名1室のブロック数～受止設定までの背景色をグレーとする
						onesanka.StyleNew.BackColor = Drawing.SystemColors.ControlLight;
					}
				}
				else
				{
					//1名参加フラグの値を0（チェックあり）とする
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "1";

					//定員制にチェックがついていない場合
					if (this.grdList.GetCellCheck(this.grdList.RowSel, Room_Koumoku.teiinseiflg) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
					{
						//1名1室のブロック数～受止設定までの背景色を元に戻す
						onesanka.StyleNew.BackColor = BackColorType.Standard;
					}
				}
			}
			else if (this.grdList.ColSel == Room_Koumoku.teiinseiflg)
			{
				//定員制にチェックをつけた場合、ルーム定員以外のブロック数を0にする
				//チェックを外した場合、ルーム定員数を0とする
				if (this.grdList.GetCellCheck(this.grdList.RowSel, this.grdList.ColSel) == C1.Win.C1FlexGrid.CheckEnum.Checked)
				{
					this.grdList[this.grdList.RowSel, Room_Koumoku.crsblockroomnum] = 0;
					this.grdList[this.grdList.RowSel, Room_Koumoku.crsblockone1r] = 0;
					this.grdList[this.grdList.RowSel, Room_Koumoku.crsblocktwo1r] = 0;
					this.grdList[this.grdList.RowSel, Room_Koumoku.crsblockthree1r] = 0;
					this.grdList[this.grdList.RowSel, Room_Koumoku.crsblockfour1r] = 0;
					this.grdList[this.grdList.RowSel, Room_Koumoku.crsblockfive1r] = 0;

					//定員制フラグの値を1（チェックあり）とする
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "1";

					//定員数以外の項目の背景色をグレーとし、定員数の項目の背景色を元に戻す
					roomBlc.StyleNew.BackColor = Drawing.SystemColors.ControlLight;
					onesanka.StyleNew.BackColor = Drawing.SystemColors.ControlLight;
					twoRoom.StyleNew.BackColor = Drawing.SystemColors.ControlLight;
					threeRoom.StyleNew.BackColor = Drawing.SystemColors.ControlLight;
					fourRoom.StyleNew.BackColor = Drawing.SystemColors.ControlLight;
					fiveRoom.StyleNew.BackColor = Drawing.SystemColors.ControlLight;
					teiinCap.StyleNew.BackColor = BackColorType.Standard;
				}
				else
				{
					this.grdList[this.grdList.RowSel, Room_Koumoku.crsblockcapacity] = 0;

					//定員制フラグの値を0（チェックなし）とする
					this.grdList[this.grdList.RowSel, this.grdList.ColSel] = "0";

					//定員数以外の項目の背景色を元に戻し、定員数の項目の背景色をグレーとする
					roomBlc.StyleNew.BackColor = BackColorType.Standard;
					twoRoom.StyleNew.BackColor = BackColorType.Standard;
					threeRoom.StyleNew.BackColor = BackColorType.Standard;
					fourRoom.StyleNew.BackColor = BackColorType.Standard;
					fiveRoom.StyleNew.BackColor = BackColorType.Standard;
					teiinCap.StyleNew.BackColor = Drawing.SystemColors.ControlLight;
					//1名参加のチェックが外れている場合
					if (this.grdList.GetCellCheck(this.grdList.RowSel, Room_Koumoku.onesankaflg) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
					{
						onesanka.StyleNew.BackColor = Drawing.SystemColors.ControlLight;
					}
					else
					{
						onesanka.StyleNew.BackColor = BackColorType.Standard;
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

		if (e.Col == Room_Koumoku.crsblockroomnum || e.Col == Room_Koumoku.crsblockone1r ||)
		{
			e.Col = Room_Koumoku.crsblocktwo1r || e.Col == Room_Koumoku.crsblockthree1r ||;
			e.Col = Room_Koumoku.crsblockfour1r || e.Col == Room_Koumoku.crsblockfive1r;
			//コースブロック数
			TextBoxEx txtBlockRoom = new TextBoxEx();
			txtBlockRoom.Format = Common.setControlFormat(FixedCd.ControlFormat.HankakuSuji);
			txtBlockRoom.MaxLengthUnit = GrapeCity.Win.Editors.LengthUnit.Byte;
			txtBlockRoom.MaxLength = BlockRoomMaxLength;
			this.grdList.Editor = txtBlockRoom;
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
		NumberEx BlockCapacity = new NumberEx();

		//定員の表示フォーマットの設定
		if (BlockCapacity.SideButtons.Count > 0)
		{
			BlockCapacity.SideButtons.RemoveAt(0);
		}
		BlockCapacity.ImeMode = System.Windows.Forms.ImeMode.Disable;
		BlockCapacity.Fields.DecimalPart.MaxDigits = 0;
		BlockCapacity.Fields.IntegerPart.MaxDigits = BlockCapacityMaxLength;
		BlockCapacity.Fields.IntegerPart.GroupSeparator = ',';
		BlockCapacity.Font = new System.Drawing.Font(BlockCapacity.Font.FontFamily, 11.25, BlockCapacity.Font.Style);
		BlockCapacity.Spin.AllowSpin = false;
		BlockCapacity.AllowDeleteToNull = true;
		BlockCapacity.ValueSign = GrapeCity.Win.Editors.ValueSignControl.Positive;
		this.grdList.Cols(Room_Koumoku.crsblockcapacity).Editor = BlockCapacity;

		// グリッドのAllowMergingプロパティを設定
		this.grdList.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;

		// マージ（結合）したいセル範囲を取得
		C1.Win.C1FlexGrid.CellRange cr = null;

		// 出発日～ルーム数の1行目と2行目を結合
		for (int i = 1; i <= 11; i++)
		{
			cr = this.grdList.GetCellRange(0, i, 1, i);
			// セル範囲をMergedRangesコレクションに追加
			this.grdList.MergedRanges.Add(cr);
		}

		//ルーム数の横幅を結合
		cr = this.grdList.GetCellRange(0, Room_Koumoku.crsblockroomnum, 0, Room_Koumoku.roomingtotal);
		// セル範囲をMergedRangesコレクションに追加
		this.grdList.MergedRanges.Add(cr);

		// ルーム数の1行目前半のタイトル
		cr = this.grdList.GetCellRange(1, Room_Koumoku.crsblockroomnum, 1, Room_Koumoku.crsblockroomnum);
		// セル範囲にデータを設定
		cr.Data = "ブロック数";
		// セル範囲をMergedRangesコレクションに追加
		this.grdList.MergedRanges.Add(cr);

		// ルーム数の1行目後半のタイトル
		cr = this.grdList.GetCellRange(1, Room_Koumoku.roomingtotal, 1, Room_Koumoku.roomingtotal);
		// セル範囲にデータを設定
		cr.Data = "予約済数";
		// セル範囲をMergedRangesコレクションに追加
		this.grdList.MergedRanges.Add(cr);

		// 1室1名～5室1名の横幅を結合
		int cs = 11;
		for (int i = 1; i <= 5; i++)
		{
			cs = cs + 3;
			cr = this.grdList.GetCellRange(0, cs, 0, cs + 2);
			// セル範囲をMergedRangesコレクションに追加
			this.grdList.MergedRanges.Add(cr);
		}

		// 1室1名～5室1名の2行目前半のタイトル
		int cs1 = 11;
		for (int i = 1; i <= 5; i++)
		{
			cs1 = cs1 + 3;
			cr = grdList.GetCellRange(1, cs1, 1, cs1);
			// セル範囲にデータを設定
			cr.Data = "ブロック数";
			// セル範囲をMergedRangesコレクションに追加
			this.grdList.MergedRanges.Add(cr);
		}

		// 1室1名～5室1名の2行目後半のタイトル
		int cs2 = 12;
		for (int i = 1; i <= 5; i++)
		{
			cs2 = cs2 + 3;
			cr = this.grdList.GetCellRange(1, cs2, 1, cs2);
			// セル範囲にデータを設定
			cr.Data = "予約済数";
			// セル範囲をMergedRangesコレクションに追加
			this.grdList.MergedRanges.Add(cr);
		}

		// 1室1名～5室1名の2行目後半のタイトル
		int cs3 = 13;
		for (int i = 1; i <= 5; i++)
		{
			cs3 = cs3 + 3;
			cr = this.grdList.GetCellRange(1, cs3, 1, cs3);
			// セル範囲にデータを設定
			cr.Data = "受止";
			// セル範囲をMergedRangesコレクションに追加
			this.grdList.MergedRanges.Add(cr);
		}

		// 静止列 設定 (～催行)
		grdList.Cols.Frozen = 7;

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

		//ルームは定期からの遷移はないため企画を表示
		this.grdList.Cols(Room_Koumoku.unkyukbn).Visible = false;
		this.grdList.Cols(Room_Koumoku.saikoukakuteikbn).Visible = true;
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
		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (CommonConvertUtil.ChkWhenDay(System.Convert.ToDateTime(this.grdList(i, Room_Koumoku.syuptday)), CommonConvertUtil.WhenKako, dtNow) == true)
			{
				this.grdList.Rows(i).AllowEditing = false;
				this.grdList[i, Room_Koumoku.henkoukahikbn] = Kahi;
				// 過去日であるデータ背景色をグレー
				this.grdList.Rows(i).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
			}
		}
		return true;

	}

	/// <summary>
	/// 定員・1名参加有無チェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckTeiinOnesanka()
	{

		//定員・1名参加有無チェック
		for (int i = 2; i <= this.grdList.Rows.Count - 1; i++)
		{
			//定員制フラグにチェックがあるかないか
			if (grdList(i, Room_Koumoku.teiinseiflg).ToString() == "1")
			{
				//定員制にチェックがある場合、ルーム設定の背景色をグレーとする
				//ルーム総計設定のセル範囲の定義
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockroomnum, i, Room_Koumoku.roomingtotal).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
				//1名1室
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockone1r, i, Room_Koumoku.uketomekbnone1r).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
				//2名1室
				this.grdList.GetCellRange(i, Room_Koumoku.crsblocktwo1r, i, Room_Koumoku.uketomekbntwo1r).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
				//3名1室
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockthree1r, i, Room_Koumoku.uketomekbnthree1r).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
				//4名1室
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockfour1r, i, Room_Koumoku.uketomekbnfour1r).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
				//5名1室
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockfive1r, i, Room_Koumoku.uketomekbnfive1r).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
			}
			else
			{
				//1名参加が不可である場合、背景色をグレーとする
				if (grdList(i, Room_Koumoku.onesankaflg).ToString() == "0")
				{
					this.grdList.GetCellRange(i, Room_Koumoku.crsblockone1r, i, Room_Koumoku.uketomekbnone1r).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
				}
				//定員制にチェックがない場合、背景色をグレーとする
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockcapacity).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
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
	//    Dim dataAccess As New Room_DA
	//    Dim systemupdatepgmid As String = Me.Name
	//    Dim i As Integer = 2
	//    Dim FlgChk As Integer = 0

	//    '使用中フラグ更新
	//    dtUsingFlg = dataAccess.executeUsingFlgCrs(selectOldData, systemupdatepgmid)

	//    For Each row As DataRow In dtUsingFlg.Rows
	//        If CType(row("USING_FLG"), String) = FixedCd.UsingFlg.Use Then
	//            Me.grdList(i, Room_Koumoku.usingflg) = FixedCd.UsingFlg.Use
	//        Else
	//            FlgChk = 1
	//            Me.grdList.Rows(i).AllowEditing = False
	//            Me.grdList(i, Room_Koumoku.henkoukahikbn) = Kahi
	//            ' 過去日であるデータ背景色をグレー
	//            Me.grdList.Rows(i).StyleNew.BackColor = Drawing.SystemColors.ControlLight
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
				crsKey.SyuPtDay = Strings.Replace(System.Convert.ToString(grdList.GetData(i + 1, Room_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);
				crsKey.GouSya = System.Convert.ToInt32(grdList.GetData(i + 1, Room_Koumoku.gousya));

				// 使用中フラグ確認
				useFlg = checkUseFlg(PrmData, crsKey);
			}

			//日付と号車が同行な場合、処理スキップ
			if (System.Convert.ToString(this.grdList(i + 1, Room_Koumoku.syuptday)) == System.Convert.ToString(this.grdList(i, Room_Koumoku.syuptday)) &&)
			{
				System.Convert.ToDecimal(this.grdList(i + 1, Room_Koumoku.gousya)) = System.Convert.ToDecimal(System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.gousya)));
			}
			else
			{
				// コースキーセット
				crsKey.CrsCd = System.Convert.ToString(txtCrsCd.Text.ToString());
				crsKey.SyuPtDay = Strings.Replace(System.Convert.ToString(grdList.GetData(i + 1, Room_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0);
				crsKey.GouSya = System.Convert.ToInt32(grdList.GetData(i + 1, Room_Koumoku.gousya));

				// 使用中フラグ確認
				useFlg = checkUseFlg(PrmData, crsKey);
			}

			//使用中であるレコードへ設定
			if (useFlg == true)
			{
				this.grdList.Rows(i + 1).AllowEditing = false;
				this.grdList[i + 1, Room_Koumoku.henkoukahikbn] = Kahi;
				// 使用中であるデータ背景色をグレー
				grdList.Rows(i + 1).StyleNew.BackColor = Drawing.SystemColors.ControlLight;
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
	/// 更新処理前のチェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckRoomUpdate(int i)
	{

		string ONESANKAFLG = string.Empty;
		string AIBEYAUSEFLG = string.Empty;
		string TEIINSEIFLG = string.Empty;
		string CRSBLOCKCAPACITY = string.Empty;
		string CRSBLOCKROOMNUM = string.Empty;
		string CRSBLOCKONE1R = string.Empty;
		string UKETOMEKBNONE1R = string.Empty;
		string CRSBLOCKTWO1R = string.Empty;
		string UKETOMEKBNTWO1R = string.Empty;
		string CRSBLOCKTHREE1R = string.Empty;
		string UKETOMEKBNTHREE1R = string.Empty;
		string CRSBLOCKFOUR1R = string.Empty;
		string UKETOMEKBNFOUR1R = string.Empty;
		string CRSBLOCKFIVE1R = string.Empty;
		string UKETOMEKBNFIVE1R = string.Empty;

		//入力不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		//引数の設定
		foreach (DataRow row in selectOldData.Rows)
		{
			if (row["SYUPT_DAY"].Equals(this.grdList(i, Room_Koumoku.syuptday)) &&)
			{
				row["HAISYA_KEIYU_CD_1"].ToString().Equals(this.grdList(i, Room_Koumoku.haisyakeiyucd1)) &&;
				row["GOUSYA"].Equals(this.grdList(i, Room_Koumoku.gousya)) &&;
				row["SYUPT_TIME_1"].Equals(this.grdList(i, Room_Koumoku.syupttime1));
				ONESANKAFLG = row["ONE_SANKA_FLG"].ToString();
				AIBEYAUSEFLG = row["AIBEYA_USE_FLG"].ToString();
				TEIINSEIFLG = row["TEIINSEI_FLG"].ToString();
				CRSBLOCKCAPACITY = row["CRS_BLOCK_CAPACITY"].ToString();
				CRSBLOCKROOMNUM = row["CRS_BLOCK_ROOM_NUM"].ToString();
				CRSBLOCKONE1R = row["CRS_BLOCK_ONE_1R"].ToString();
				UKETOMEKBNONE1R = row["UKETOME_KBN_ONE_1R"].ToString();
				CRSBLOCKTWO1R = row["CRS_BLOCK_TWO_1R"].ToString();
				UKETOMEKBNTWO1R = row["UKETOME_KBN_TWO_1R"].ToString();
				CRSBLOCKTHREE1R = row["CRS_BLOCK_THREE_1R"].ToString();
				UKETOMEKBNTHREE1R = row["UKETOME_KBN_THREE_1R"].ToString();
				CRSBLOCKFOUR1R = row["CRS_BLOCK_FOUR_1R"].ToString();
				UKETOMEKBNFOUR1R = row["UKETOME_KBN_FOUR_1R"].ToString();
				CRSBLOCKFIVE1R = row["CRS_BLOCK_FIVE_1R"].ToString();
				UKETOMEKBNFIVE1R = row["UKETOME_KBN_FIVE_1R"].ToString();
			}
		}

		//1名参加フラグ
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.onesankaflg)) == true)
		{
			this.grdList[i, Room_Koumoku.onesankaflg] = string.Empty;
		}
		if (ONESANKAFLG.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.onesankaflg))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//相部屋フラグ
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.aibeyauseflg)) == true)
		{
			this.grdList[i, Room_Koumoku.aibeyauseflg] = string.Empty;
		}
		if (AIBEYAUSEFLG.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.aibeyauseflg))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//定員制フラグ
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.teiinseiflg)) == true)
		{
			this.grdList[i, Room_Koumoku.teiinseiflg] = string.Empty;
		}
		if (TEIINSEIFLG.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.teiinseiflg))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//定員
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.crsblockcapacity)) == true)
		{
			this.grdList[i, Room_Koumoku.crsblockcapacity] = string.Empty;
		}
		if (CRSBLOCKCAPACITY.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.crsblockcapacity))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//ルーム数
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.crsblockroomnum)) == true)
		{
			this.grdList[i, Room_Koumoku.crsblockroomnum] = string.Empty;
		}
		if (CRSBLOCKROOMNUM.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.crsblockroomnum))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//1名1室ブロック数
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.crsblockone1r)) == true)
		{
			this.grdList[i, Room_Koumoku.crsblockone1r] = string.Empty;
		}
		if (CRSBLOCKONE1R.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.crsblockone1r))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//1名1室受止
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.uketomekbnone1r)) == true)
		{
			this.grdList[i, Room_Koumoku.uketomekbnone1r] = string.Empty;
		}
		if (UKETOMEKBNONE1R.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.uketomekbnone1r))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//2名1室ブロック数
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.crsblocktwo1r)) == true)
		{
			this.grdList[i, Room_Koumoku.crsblocktwo1r] = string.Empty;
		}
		if (CRSBLOCKTWO1R.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.crsblocktwo1r))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//2名1室受止
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.uketomekbntwo1r)) == true)
		{
			this.grdList[i, Room_Koumoku.uketomekbntwo1r] = string.Empty;
		}
		if (UKETOMEKBNTWO1R.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.uketomekbntwo1r))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//3名1室ブロック数
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.crsblockthree1r)) == true)
		{
			this.grdList[i, Room_Koumoku.crsblockthree1r] = string.Empty;
		}
		if (CRSBLOCKTHREE1R.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.crsblockthree1r))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//3名1室受止
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.uketomekbnthree1r)) == true)
		{
			this.grdList[i, Room_Koumoku.uketomekbnthree1r] = string.Empty;
		}
		if (UKETOMEKBNTHREE1R.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.uketomekbnthree1r))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//4名1室ブロック数
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.crsblockfour1r)) == true)
		{
			this.grdList[i, Room_Koumoku.crsblockfour1r] = string.Empty;
		}
		if (CRSBLOCKFOUR1R.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.crsblockfour1r))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//4名1室受止
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.uketomekbnfour1r)) == true)
		{
			this.grdList[i, Room_Koumoku.uketomekbnfour1r] = string.Empty;
		}
		if (UKETOMEKBNFOUR1R.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.uketomekbnfour1r))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//5名1室ブロック数
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.crsblockfive1r)) == true)
		{
			this.grdList[i, Room_Koumoku.crsblockfive1r] = string.Empty;
		}
		if (CRSBLOCKFIVE1R.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.crsblockfive1r))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		//5名1室受止
		if (Information.IsDBNull(this.grdList(i, Room_Koumoku.uketomekbnfive1r)) == true)
		{
			this.grdList[i, Room_Koumoku.uketomekbnfive1r] = string.Empty;
		}
		if (UKETOMEKBNFIVE1R.Equals(System.Convert.ToString(this.grdList(i, Room_Koumoku.uketomekbnfive1r))) == false)
		{
			this.grdList[i, Room_Koumoku.updatekbn] = UpDateKbn;
			return true;
		}
		return false;

	}

	/// <summary>
	/// 更新処理前のチェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckRoomSu(int i)
	{
		string flgOn = "1";

		//入力不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		//更新区分が"Y"ではない行は読み飛ばす
		if (System.Convert.ToString(this.grdList(i, Room_Koumoku.updatekbn)) != UpDateKbn)
		{
			return false;
		}

		//ルーム数と室数の依存関係チェック
		if (System.Convert.ToString(this.grdList(i, Room_Koumoku.onesankaflg)) == flgOn)
		{
			if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockone1r)) > System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockroomnum)))
			{
				//グリッドの背景色を変更
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockone1r).StyleNew.BackColor = BackColorType.InputError;
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockroomnum).StyleNew.BackColor = BackColorType.InputError;
				return true;
			}
		}

		if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblocktwo1r)) > System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockroomnum)))
		{
			//グリッドの背景色を変更
			this.grdList.GetCellRange(i, Room_Koumoku.crsblocktwo1r).StyleNew.BackColor = BackColorType.InputError;
			this.grdList.GetCellRange(i, Room_Koumoku.crsblockroomnum).StyleNew.BackColor = BackColorType.InputError;
			return true;
		}
		if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockthree1r)) > System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockroomnum)))
		{
			//グリッドの背景色を変更
			this.grdList.GetCellRange(i, Room_Koumoku.crsblockthree1r).StyleNew.BackColor = BackColorType.InputError;
			this.grdList.GetCellRange(i, Room_Koumoku.crsblockroomnum).StyleNew.BackColor = BackColorType.InputError;
			return true;
		}
		if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockfour1r)) > System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockroomnum)))
		{
			//グリッドの背景色を変更
			this.grdList.GetCellRange(i, Room_Koumoku.crsblockfour1r).StyleNew.BackColor = BackColorType.InputError;
			this.grdList.GetCellRange(i, Room_Koumoku.crsblockroomnum).StyleNew.BackColor = BackColorType.InputError;
			return true;
		}
		if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockfive1r)) > System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockroomnum)))
		{
			//グリッドの背景色を変更
			this.grdList.GetCellRange(i, Room_Koumoku.crsblockfive1r).StyleNew.BackColor = BackColorType.InputError;
			this.grdList.GetCellRange(i, Room_Koumoku.crsblockroomnum).StyleNew.BackColor = BackColorType.InputError;
			return true;
		}
		return false;

	}

	/// <summary>
	/// 更新処理前のチェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckRoomReserveSu(int i)
	{
		string flgOn = "1";

		//入力不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		//更新区分が"Y"ではない行は読み飛ばす
		if (System.Convert.ToString(this.grdList(i, Room_Koumoku.updatekbn)) != UpDateKbn)
		{
			return false;
		}

		//ルーム予約数とルーム数の依存関係チェック
		if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockroomnum)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingtotal)) || System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockone1r)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingbetuninzu1)) || System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblocktwo1r)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingbetuninzu2)) || System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockthree1r)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingbetuninzu3)) || System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockfour1r)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingbetuninzu4)) || System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockfive1r)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingbetuninzu5)))
		{
			//グリッドの背景色を変更
			if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockroomnum)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingtotal)))
			{
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockroomnum).StyleNew.BackColor = BackColorType.InputError;
				this.grdList.GetCellRange(i, Room_Koumoku.roomingtotal).StyleNew.BackColor = BackColorType.InputError;
			}
			if (System.Convert.ToString(this.grdList(i, Room_Koumoku.onesankaflg)) == flgOn)
			{
				if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockone1r)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingbetuninzu1)))
				{
					this.grdList.GetCellRange(i, Room_Koumoku.crsblockone1r).StyleNew.BackColor = BackColorType.InputError;
					this.grdList.GetCellRange(i, Room_Koumoku.roomingbetuninzu1).StyleNew.BackColor = BackColorType.InputError;
				}
			}
			if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblocktwo1r)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingbetuninzu2)))
			{
				this.grdList.GetCellRange(i, Room_Koumoku.crsblocktwo1r).StyleNew.BackColor = BackColorType.InputError;
				this.grdList.GetCellRange(i, Room_Koumoku.roomingbetuninzu2).StyleNew.BackColor = BackColorType.InputError;
			}
			if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockthree1r)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingbetuninzu3)))
			{
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockthree1r).StyleNew.BackColor = BackColorType.InputError;
				this.grdList.GetCellRange(i, Room_Koumoku.roomingbetuninzu3).StyleNew.BackColor = BackColorType.InputError;
			}
			if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockfour1r)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingbetuninzu4)))
			{
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockfour1r).StyleNew.BackColor = BackColorType.InputError;
				this.grdList.GetCellRange(i, Room_Koumoku.roomingbetuninzu4).StyleNew.BackColor = BackColorType.InputError;
			}
			if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockfive1r)) < System.Convert.ToInt32(this.grdList(i, Room_Koumoku.roomingbetuninzu5)))
			{
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockfive1r).StyleNew.BackColor = BackColorType.InputError;
				this.grdList.GetCellRange(i, Room_Koumoku.roomingbetuninzu5).StyleNew.BackColor = BackColorType.InputError;
			}
			return true;
		}
		return false;

	}

	/// <summary>
	/// 更新処理前のチェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckTeiin(int i)
	{
		string teiinsei = "1";

		//入力不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		//更新区分が"Y"ではない行は読み飛ばす
		if (System.Convert.ToString(this.grdList(i, Room_Koumoku.updatekbn)) != UpDateKbn)
		{
			return false;
		}

		//定員の依存関係チェック
		if (System.Convert.ToString(this.grdList(i, Room_Koumoku.teiinseiflg)) == teiinsei)
		{
			if (1 > System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockcapacity)))
			{
				//グリッドの背景色を変更
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockcapacity).StyleNew.BackColor = BackColorType.InputError;
				return true;
			}
		}

		return false;

	}

	/// <summary>
	/// 更新処理前のチェック処理
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool CheckReserveTeiseki(int i)
	{
		string teiinsei = "1";

		//入力不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		//更新区分が"Y"ではない行は読み飛ばす
		if (System.Convert.ToString(this.grdList(i, Room_Koumoku.updatekbn)) != UpDateKbn)
		{
			return false;
		}

		//予約数定席（+補助席）と定員の依存関係チェック
		if (System.Convert.ToString(this.grdList(i, Room_Koumoku.teiinseiflg)) == teiinsei)
		{
			if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.crsblockcapacity)) < base.SearchResultGridData.Rows(i - 2).ItemArray(12) + base.SearchResultGridData.Rows(i - 2).ItemArray(13))
			{
				//グリッドの背景色を変更
				this.grdList.GetCellRange(i, Room_Koumoku.crsblockcapacity).StyleNew.BackColor = BackColorType.InputError;
				return true;
			}
		}

		return false;

	}

	///' <summary>
	///' 予約済ルーム数の算出
	///' </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	private bool ReservedRoomQuery(DataTable GridData)
	{

		decimal ReservedRoomTotal = new decimal(); //予約済ルーム数総計
		decimal RoomingBetuNinzu1 = new decimal(); //予約済1名1R
		decimal RoomingBetuNinzu2 = new decimal(); //予約済2名1R
		decimal RoomingBetuNinzu3 = new decimal(); //予約済3名1R
		decimal RoomingBetuNinzu4 = new decimal(); //予約済4名1R
		decimal RoomingBetuNinzu5 = new decimal(); //予約済5名1R
		decimal WK_RoomMaxCapacity = new decimal(); //MAX定員
		decimal WK_AibeyaNinzuMale = new decimal(); //相部屋予約人数男性
		decimal WK_AibeyaNinzuJyosei = new decimal(); //相部屋予約人数女性
		decimal WK_Mod; //剰余

		//予約済ルーム数の算出方法を記載
		foreach (DataRow row in GridData.Rows)
		{
			ReservedRoomTotal = 0;
			RoomingBetuNinzu1 = 0;
			RoomingBetuNinzu2 = 0;
			RoomingBetuNinzu3 = 0;
			RoomingBetuNinzu4 = 0;
			RoomingBetuNinzu5 = 0;
			WK_RoomMaxCapacity = 99;
			WK_AibeyaNinzuMale = 0;
			WK_AibeyaNinzuJyosei = 0;
			WK_Mod = 0;
			int RowNo = System.Convert.ToInt32(GridData.Rows.IndexOf(row));

			ReservedRoomTotal = System.Convert.ToDecimal(row["ROOMING_BETU_NINZU_1"]) + System.Convert.ToDecimal(row["ROOMING_BETU_NINZU_2"])
				+ System.Convert.ToDecimal(row["ROOMING_BETU_NINZU_3"]) + System.Convert.ToDecimal(row["ROOMING_BETU_NINZU_4"])
				+ System.Convert.ToDecimal(row["ROOMING_BETU_NINZU_5"]);

			RoomingBetuNinzu1 = System.Convert.ToDecimal(row["ROOMING_BETU_NINZU_1"]);
			RoomingBetuNinzu2 = System.Convert.ToDecimal(row["ROOMING_BETU_NINZU_2"]);
			RoomingBetuNinzu3 = System.Convert.ToDecimal(row["ROOMING_BETU_NINZU_3"]);
			RoomingBetuNinzu4 = System.Convert.ToDecimal(row["ROOMING_BETU_NINZU_4"]);
			RoomingBetuNinzu5 = System.Convert.ToDecimal(row["ROOMING_BETU_NINZU_5"]);

			//WK_ROOMMAX定員=99として、コース台帳（ホテル）・仕入先マスタよりROOMMAX定員を取得
			if (System.Convert.ToDecimal(row["ROOM_MAX_CAPACITY"]) < WK_RoomMaxCapacity)
			{
				WK_RoomMaxCapacity = System.Convert.ToDecimal(row["ROOM_MAX_CAPACITY"]);
			}

			if (System.Convert.ToDecimal(row["AIBEYA_YOYAKU_NINZU_MALE"]) == 0 || WK_RoomMaxCapacity == 0)
			{
			}
			else
			{
				WK_AibeyaNinzuMale = System.Convert.ToDecimal(row["AIBEYA_YOYAKU_NINZU_MALE"]) / WK_RoomMaxCapacity;
				WK_Mod = System.Convert.ToDecimal(System.Convert.ToDecimal(row["AIBEYA_YOYAKU_NINZU_MALE"]) % WK_RoomMaxCapacity);
				if (WK_Mod == 0)
				{
				}
				else
				{
					WK_AibeyaNinzuMale++;
				}
				ReservedRoomTotal = ReservedRoomTotal + WK_AibeyaNinzuMale;
			}

			if (System.Convert.ToDecimal(row["AIBEYA_YOYAKU_NINZU_JYOSEI"]) == 0 || WK_RoomMaxCapacity == 0)
			{
			}
			else
			{
				WK_AibeyaNinzuJyosei = System.Convert.ToDecimal(row["AIBEYA_YOYAKU_NINZU_JYOSEI"]) / WK_RoomMaxCapacity;
				WK_Mod = System.Convert.ToDecimal(System.Convert.ToDecimal(row["AIBEYA_YOYAKU_NINZU_JYOSEI"]) % WK_RoomMaxCapacity);
				if (WK_Mod == 0)
				{
				}
				else
				{
					WK_AibeyaNinzuJyosei++;
				}
				ReservedRoomTotal = ReservedRoomTotal + WK_AibeyaNinzuJyosei;
			}

			this.grdList[RowNo + 2, Room_Koumoku.roomingtotal] = ReservedRoomTotal;
			this.grdList[RowNo + 2, Room_Koumoku.roomingbetuninzu1] = RoomingBetuNinzu1;
			this.grdList[RowNo + 2, Room_Koumoku.roomingbetuninzu2] = RoomingBetuNinzu2;
			this.grdList[RowNo + 2, Room_Koumoku.roomingbetuninzu3] = RoomingBetuNinzu3;
			this.grdList[RowNo + 2, Room_Koumoku.roomingbetuninzu4] = RoomingBetuNinzu4;
			this.grdList[RowNo + 2, Room_Koumoku.roomingbetuninzu5] = RoomingBetuNinzu5;
		}
		return true;
	}

	#endregion
	#endregion

	#region DB更新処理

	///' <summary>
	///' 算出処理
	///' ※テーブル複数などは未対応
	///' </summary>
	///' <returns>部屋残数値</returns>
	private Hashtable RoomZanCalculationQuery(int i)
	{

		decimal CAPACITYHO1KAI = System.Convert.ToDecimal(base.SearchResultGridData.Rows(i - 2).ItemArray(28)); //定員補１階
		decimal CAPACITYREGULAR = System.Convert.ToDecimal(base.SearchResultGridData.Rows(i - 2).ItemArray(29)); //定員定
		Hashtable paramInfoList = new Hashtable(); //DBパラメータ
		decimal ROOMZANSUSOKEI = 0; //部屋残数総計
		decimal ROOMZANSUONEROOM = 0; //部屋残数１人部屋
		decimal ROOMZANSUTWOROOM = 0; //部屋残数２人部屋
		decimal ROOMZANSUTHREEROOM = 0; //部屋残数３人部屋
		decimal ROOMZANSUFOURROOM = 0; //部屋残数４人部屋
		decimal ROOMZANSUFIVEROOM = 0; //部屋残数５人部屋
		decimal JYOSYACAPACITY = 0; //乗車定員
		decimal YOYAKUKANOUNUM = 0; //予約可能数
		decimal KUSEKISUU = 0; //空席数

		//乗車定員・空席数の算出方法を記載
		//定員制以外の場合
		if (System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.teiinseiflg)) == 0)
		{
			ROOMZANSUSOKEI = System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblockroomnum)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.roomingtotal));
			//1名１室 ≠ 0である場合
			if (System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblockone1r)) == 0)
			{
				//1名参加がONの場合
				if (System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.onesankaflg)) == 1)
				{
					this.grdList[i, Room_Koumoku.crsblockone1r] = System.Convert.ToDecimal(this.grdList[i, Room_Koumoku.crsblockroomnum]);
					ROOMZANSUONEROOM = ROOMZANSUSOKEI;
				}
			}
			else
			{
				//1名参加がONの場合
				if (System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.onesankaflg)) == 1)
				{
					ROOMZANSUONEROOM = System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblockone1r)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.roomingbetuninzu1));
					//部屋残数・総計 <= 部屋残数・１人部屋の場合
					if (ROOMZANSUSOKEI <= ROOMZANSUONEROOM)
					{
						ROOMZANSUONEROOM = ROOMZANSUSOKEI;
					}
				}
				else
				{
					// 1名参加フラグがOffの場合、0を設定
					this.grdList[i, Room_Koumoku.crsblockone1r] = 0;
				}
			}
			//2名１室 ≠ 0である場合
			if (System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblocktwo1r)) == 0)
			{
				this.grdList[i, Room_Koumoku.crsblocktwo1r] = System.Convert.ToDecimal(this.grdList[i, Room_Koumoku.crsblockroomnum]);
				ROOMZANSUTWOROOM = ROOMZANSUSOKEI;
			}
			else
			{
				ROOMZANSUTWOROOM = System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblocktwo1r)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.roomingbetuninzu2));
				//部屋残数・総計 <= 部屋残数・2人部屋の場合
				if (ROOMZANSUSOKEI <= ROOMZANSUTWOROOM)
				{
					ROOMZANSUTWOROOM = ROOMZANSUSOKEI;
				}
			}
			//3名１室 ≠ 0である場合
			if (System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblockthree1r)) == 0)
			{
				this.grdList[i, Room_Koumoku.crsblockthree1r] = System.Convert.ToDecimal(this.grdList[i, Room_Koumoku.crsblockroomnum]);
				ROOMZANSUTHREEROOM = ROOMZANSUSOKEI;
			}
			else
			{
				ROOMZANSUTHREEROOM = System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblockthree1r)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.roomingbetuninzu3));
				//部屋残数・総計 <= 部屋残数・3人部屋の場合
				if (ROOMZANSUSOKEI <= ROOMZANSUTHREEROOM)
				{
					ROOMZANSUTHREEROOM = ROOMZANSUSOKEI;
				}
			}
			//4名１室 ≠ 0である場合
			if (System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblockfour1r)) == 0)
			{
				this.grdList[i, Room_Koumoku.crsblockfour1r] = System.Convert.ToDecimal(this.grdList[i, Room_Koumoku.crsblockroomnum]);
				ROOMZANSUFOURROOM = ROOMZANSUSOKEI;
			}
			else
			{
				ROOMZANSUFOURROOM = System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblockfour1r)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.roomingbetuninzu4));
				//部屋残数・総計 <= 部屋残数・4人部屋の場合
				if (ROOMZANSUSOKEI <= ROOMZANSUFOURROOM)
				{
					ROOMZANSUFOURROOM = ROOMZANSUSOKEI;
				}
			}
			//5名１室 ≠ 0である場合
			if (System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblockfive1r)) == 0)
			{
				this.grdList[i, Room_Koumoku.crsblockfive1r] = System.Convert.ToDecimal(this.grdList[i, Room_Koumoku.crsblockroomnum]);
				ROOMZANSUFIVEROOM = ROOMZANSUSOKEI;
			}
			else
			{
				ROOMZANSUFIVEROOM = System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblockfive1r)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.roomingbetuninzu5));
				//部屋残数・総計 <= 屋残数・5人部屋の場合
				if (ROOMZANSUSOKEI <= ROOMZANSUFIVEROOM)
				{
					ROOMZANSUFIVEROOM = ROOMZANSUSOKEI;
				}
			}
			// 定員に0を設定
			this.grdList[i, Room_Koumoku.crsblockcapacity] = 0;
		}
		else
		{
			//定員制の場合
			JYOSYACAPACITY = System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.crsblockcapacity));
			//乗車定員 < コース台帳（基本）.定員・定 + コース台帳（基本）.定員・補/1Fの場合
			if (JYOSYACAPACITY < CAPACITYREGULAR + CAPACITYHO1KAI)
			{
				YOYAKUKANOUNUM = JYOSYACAPACITY;
				KUSEKISUU = YOYAKUKANOUNUM - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.blockkakuhonum)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.eiblocktei)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.kusekikakuhonum)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.yoyakunum));
			}
			//乗車定員 >= コース台帳（基本）.定員・定 + コース台帳（基本）.定員・補/1Fの場合
			if (JYOSYACAPACITY >= CAPACITYREGULAR + CAPACITYHO1KAI)
			{
				YOYAKUKANOUNUM = CAPACITYREGULAR + CAPACITYHO1KAI;
				KUSEKISUU = YOYAKUKANOUNUM - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.blockkakuhonum)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.eiblocktei)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.kusekikakuhonum)) - System.Convert.ToDecimal(this.grdList(i, Room_Koumoku.yoyakunum));
			}
		}
		//受止にチェックが入っている場合
		//定員制以外の場合
		if (this.grdList.GetCellCheck(i, Room_Koumoku.teiinseiflg) == C1.Win.C1FlexGrid.CheckEnum.Unchecked)
		{
			//受止１名１室にチェックがついている場合
			if (this.grdList.GetCellCheck(i, Room_Koumoku.uketomekbnone1r) == C1.Win.C1FlexGrid.CheckEnum.Checked)
			{
				//部屋残数・１人部屋≠0場合
				if (ROOMZANSUONEROOM == 0)
				{
				}
				else
				{
					ROOMZANSUONEROOM = 0;
				}
			}
			//受止２名１室にチェックがついている場合
			if (this.grdList.GetCellCheck(i, Room_Koumoku.uketomekbntwo1r) == C1.Win.C1FlexGrid.CheckEnum.Checked)
			{
				//部屋残数・２人部屋≠0場合
				if (ROOMZANSUTWOROOM == 0)
				{
				}
				else
				{
					ROOMZANSUTWOROOM = 0;
				}
			}
			//受止３名１室にチェックがついている場合
			if (this.grdList.GetCellCheck(i, Room_Koumoku.uketomekbnthree1r) == C1.Win.C1FlexGrid.CheckEnum.Checked)
			{
				//部屋残数・３人部屋≠0場合
				if (ROOMZANSUTHREEROOM == 0)
				{
				}
				else
				{
					ROOMZANSUTHREEROOM = 0;
				}
			}
			//受止４名１室にチェックがついている場合
			if (this.grdList.GetCellCheck(i, Room_Koumoku.uketomekbnfour1r) == C1.Win.C1FlexGrid.CheckEnum.Checked)
			{
				//部屋残数・４人部屋≠0場合
				if (ROOMZANSUFOURROOM == 0)
				{
				}
				else
				{
					ROOMZANSUFOURROOM = 0;
				}
			}
			//受止５名１室にチェックがついている場合
			if (this.grdList.GetCellCheck(i, Room_Koumoku.uketomekbnfive1r) == C1.Win.C1FlexGrid.CheckEnum.Checked)
			{
				//部屋残数・５人部屋≠0場合
				if (ROOMZANSUFIVEROOM == 0)
				{
				}
				else
				{
					ROOMZANSUFIVEROOM = 0;
				}
			}
		}

		paramInfoList.Add("ROOM_ZANSU_SOKEI", ROOMZANSUSOKEI); //部屋残数総計
		paramInfoList.Add("ROOM_ZANSU_ONE_ROOM", ROOMZANSUONEROOM); //部屋残数１人部屋
		paramInfoList.Add("ROOM_ZANSU_TWO_ROOM", ROOMZANSUTWOROOM); //部屋残数２人部屋
		paramInfoList.Add("ROOM_ZANSU_THREE_ROOM", ROOMZANSUTHREEROOM); //部屋残数３人部屋
		paramInfoList.Add("ROOM_ZANSU_FOUR_ROOM", ROOMZANSUFOURROOM); //部屋残数４人部屋
		paramInfoList.Add("ROOM_ZANSU_FIVE_ROOM", ROOMZANSUFIVEROOM); //部屋残数５人部屋
		paramInfoList.Add("JYOSYA_CAPACITY", JYOSYACAPACITY); //乗車定員
		paramInfoList.Add("YOYAKU_KANOU_NUM", YOYAKUKANOUNUM); //予約可能数
		paramInfoList.Add("KUSEKI_NUM_TEISEKI", KUSEKISUU); //空席数定席

		return paramInfoList;
	}

	///' <summary>
	///' 更新・登録処理
	///' </summary>
	///' <returns>更新処理件数</returns>
	private int ExecuteNonQuery(DbShoriKbn kbn, int i, Hashtable paramJyosyaList)
	{

		//戻り値
		int returnValue = 0;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		//DAクラス作成、パラメータ設定、登録(更新)処理実施までを実装
		//DataAccessクラス生成
		Room_DA dataAccess = new Room_DA();

		if (DbShoriKbn.Update.Equals(kbn))
		{
			//パラメータ設定
			paramInfoList.Add("SYUPT_DAY", Strings.Replace(System.Convert.ToString(this.grdList(i, Room_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));
			paramInfoList.Add("CRS_CD", this.txtCrsCd.Text);
			paramInfoList.Add("GOUSYA", this.grdList(i, Room_Koumoku.gousya));
			//paramInfoList.Add("USING_FLG", FixedCd.UsingFlg.Unused)
			paramInfoList.Add("ROOM_ZANSU_SOKEI", paramJyosyaList.Item("ROOM_ZANSU_SOKEI"));
			paramInfoList.Add("ROOM_ZANSU_ONE_ROOM", paramJyosyaList.Item("ROOM_ZANSU_ONE_ROOM"));
			paramInfoList.Add("ROOM_ZANSU_TWO_ROOM", paramJyosyaList.Item("ROOM_ZANSU_TWO_ROOM"));
			paramInfoList.Add("ROOM_ZANSU_THREE_ROOM", paramJyosyaList.Item("ROOM_ZANSU_THREE_ROOM"));
			paramInfoList.Add("ROOM_ZANSU_FOUR_ROOM", paramJyosyaList.Item("ROOM_ZANSU_FOUR_ROOM"));
			paramInfoList.Add("ROOM_ZANSU_FIVE_ROOM", paramJyosyaList.Item("ROOM_ZANSU_FIVE_ROOM"));
			paramInfoList.Add("JYOSYA_CAPACITY", paramJyosyaList.Item("JYOSYA_CAPACITY"));
			paramInfoList.Add("YOYAKU_KANOU_NUM", paramJyosyaList.Item("YOYAKU_KANOU_NUM"));
			paramInfoList.Add("KUSEKI_NUM_TEISEKI", paramJyosyaList.Item("KUSEKI_NUM_TEISEKI"));
			if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.onesankaflg)) == 1)
			{
				paramInfoList.Add("ONE_SANKA_FLG", "Y");
			}
			else
			{
				paramInfoList.Add("ONE_SANKA_FLG", string.Empty);
			}
			if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.aibeyauseflg)) == 1)
			{
				paramInfoList.Add("AIBEYA_USE_FLG", "Y");
			}
			else
			{
				paramInfoList.Add("AIBEYA_USE_FLG", string.Empty);
			}
			if (System.Convert.ToInt32(this.grdList(i, Room_Koumoku.teiinseiflg)) == 1)
			{
				paramInfoList.Add("TEIINSEI_FLG", "Y");
			}
			else
			{
				paramInfoList.Add("TEIINSEI_FLG", string.Empty);
			}
			paramInfoList.Add("CRS_BLOCK_CAPACITY", Strings.Replace(System.Convert.ToString(this.grdList(i, Room_Koumoku.crsblockcapacity)), ",", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));
			paramInfoList.Add("CRS_BLOCK_ROOM_NUM", this.grdList(i, Room_Koumoku.crsblockroomnum));
			paramInfoList.Add("CRS_BLOCK_ONE_1R", this.grdList(i, Room_Koumoku.crsblockone1r));
			paramInfoList.Add("CRS_BLOCK_TWO_1R", this.grdList(i, Room_Koumoku.crsblocktwo1r));
			paramInfoList.Add("CRS_BLOCK_THREE_1R", this.grdList(i, Room_Koumoku.crsblockthree1r));
			paramInfoList.Add("CRS_BLOCK_FOUR_1R", this.grdList(i, Room_Koumoku.crsblockfour1r));
			paramInfoList.Add("CRS_BLOCK_FIVE_1R", this.grdList(i, Room_Koumoku.crsblockfive1r));
			paramInfoList.Add("UKETOME_KBN_ONE_1R", this.grdList(i, Room_Koumoku.uketomekbnone1r));
			paramInfoList.Add("UKETOME_KBN_TWO_1R", this.grdList(i, Room_Koumoku.uketomekbntwo1r));
			paramInfoList.Add("UKETOME_KBN_THREE_1R", this.grdList(i, Room_Koumoku.uketomekbnthree1r));
			paramInfoList.Add("UKETOME_KBN_FOUR_1R", this.grdList(i, Room_Koumoku.uketomekbnfour1r));
			paramInfoList.Add("UKETOME_KBN_FIVE_1R", this.grdList(i, Room_Koumoku.uketomekbnfive1r));
			paramInfoList.Add("SYSTEM_UPDATE_DAY", CommonDateUtil.getSystemTime());
			paramInfoList.Add("SYSTEM_UPDATE_PGMID", this.Name);
			paramInfoList.Add("SYSTEM_UPDATE_PERSON_CD", UserInfoManagement.userId);
			//Else
			//    '引数の設定
			//    For Each row As DataRow In selectOldData.Rows
			//        If row("SYUPT_DAY").Equals(Me.grdList(i, Room_Koumoku.syuptday)) AndAlso
			//                    row("HAISYA_KEIYU_CD_1").ToString.Equals(Me.grdList(i, Room_Koumoku.haisyakeiyucd1)) AndAlso
			//                    row("GOUSYA").Equals(Me.grdList(i, Room_Koumoku.gousya)) AndAlso
			//                    row("SYUPT_TIME_1").Equals(Me.grdList(i, Room_Koumoku.syupttime1)) Then
			//            If row("USING_FLG").ToString.Equals(Me.grdList(i, Room_Koumoku.usingflg)) Then
			//                Return returnValue
			//            ElseIf row("USING_FLG").ToString = String.empty AndAlso
			//                IsDBNull(Me.grdList(i, Room_Koumoku.usingflg)) = True Then
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
				returnValue = System.Convert.ToInt32(dataAccess.executeRoomTehai(Room_DA.accessType.executeUpdateRoom, paramInfoList));
				//Else
				//    'Returnの実施
				//    returnValue = dataAccess.executeRoomTehai(Room_DA.accessType.executeReturnRoom, paramInfoList)
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
		totalValue.Columns.Add("ONE_SANKA_FLG"); //１名参加フラグ
		totalValue.Columns.Add("AIBEYA_USE_FLG"); //相部屋使用フラグ
		totalValue.Columns.Add("TEIINSEI_FLG"); //定員制フラグ
		totalValue.Columns.Add("YOYAKU_NUM_TEISEKI"); //予約数定席
		totalValue.Columns.Add("YOYAKU_NUM_SUB_SEAT"); //予約数補助席
		totalValue.Columns.Add("CRS_BLOCK_CAPACITY"); //コースブロック定員
		totalValue.Columns.Add("CRS_BLOCK_ROOM_NUM"); //コースブロックルーム数
		totalValue.Columns.Add("CRS_BLOCK_ONE_1R"); //コースブロック１名１R
		totalValue.Columns.Add("CRS_BLOCK_TWO_1R"); //コースブロック２名１R
		totalValue.Columns.Add("CRS_BLOCK_THREE_1R"); //コースブロック３名１R
		totalValue.Columns.Add("CRS_BLOCK_FOUR_1R"); //コースブロック４名１R
		totalValue.Columns.Add("CRS_BLOCK_FIVE_1R"); //コースブロック５名１R
		totalValue.Columns.Add("ROOM_ZANSU_SOKEI"); //部屋残数総計
		totalValue.Columns.Add("ROOM_ZANSU_ONE_ROOM"); //部屋残数１人部屋
		totalValue.Columns.Add("ROOM_ZANSU_TWO_ROOM"); //部屋残数２人部屋
		totalValue.Columns.Add("ROOM_ZANSU_THREE_ROOM"); //部屋残数３人部屋
		totalValue.Columns.Add("ROOM_ZANSU_FOUR_ROOM"); //部屋残数４人部屋
		totalValue.Columns.Add("ROOM_ZANSU_FIVE_ROOM"); //部屋残数５人部屋
		totalValue.Columns.Add("JYOSYA_CAPACITY"); //乗車定員
		totalValue.Columns.Add("CAPACITY_HO_1KAI"); //定員補１階
		totalValue.Columns.Add("CAPACITY_REGULAR"); //定員定
		totalValue.Columns.Add("YOYAKU_KANOU_NUM"); //予約可能数
		totalValue.Columns.Add("SYSTEM_UPDATE_DAY"); //システム更新日
		totalValue.Columns.Add("SYSTEM_UPDATE_PERSON_CD"); //システム更新者コード
		totalValue.Columns.Add("SYSTEM_UPDATE_PGMID"); //システム更新ＰＧＭＩＤ
		totalValue.Columns.Add("UKETOME_KBN_ONE_1R"); //受止区分１名１Ｒ
		totalValue.Columns.Add("UKETOME_KBN_TWO_1R"); //受止区分２名１Ｒ
		totalValue.Columns.Add("UKETOME_KBN_THREE_1R"); //受止区分３名１Ｒ
		totalValue.Columns.Add("UKETOME_KBN_FOUR_1R"); //受止区分４名１Ｒ
		totalValue.Columns.Add("UKETOME_KBN_FIVE_1R"); //受止区分５名１Ｒ
		totalValue.Columns.Add("ROOMING_TOTAL"); //予約済ルーム数
		totalValue.Columns.Add("ROOMING_BETU_NINZU_1"); //SUM（ＲＯＯＭＩＮＧ別人数１）
		totalValue.Columns.Add("ROOMING_BETU_NINZU_2"); //SUM（ＲＯＯＭＩＮＧ別人数２）
		totalValue.Columns.Add("ROOMING_BETU_NINZU_3"); //SUM（ＲＯＯＭＩＮＧ別人数３）
		totalValue.Columns.Add("ROOMING_BETU_NINZU_4"); //SUM（ＲＯＯＭＩＮＧ別人数４）
		totalValue.Columns.Add("ROOMING_BETU_NINZU_5"); //SUM（ＲＯＯＭＩＮＧ別人数５）
		totalValue.Columns.Add("SIIRE_SAKI_CD"); //仕入先コード
		totalValue.Columns.Add("SIIRE_SAKI_EDABAN"); //仕入先枝番
		totalValue.Columns.Add("ROOM_MAX_CAPACITY"); //MIN（ROOMMAX定員）
		totalValue.Columns.Add("AIBEYA_YOYAKU_NINZU_MALE"); //相部屋予約人数男性
		totalValue.Columns.Add("AIBEYA_YOYAKU_NINZU_JYOSEI"); //相部屋予約人数女性
		totalValue.Columns.Add("USING_FLG"); //使用中フラグ
		totalValue.Columns.Add("HENKOU_KAHI_KBN"); //変更可否区分
		totalValue.Columns.Add("UPDATE_KBN"); //更新区分
		totalValue.Columns.Add("HIDDEN_GOUSYA"); //号車（ソートキー用３桁補正）
		totalValue.Columns.Add("BLOCK_KAKUHO_NUM"); //ブロック確保数
		totalValue.Columns.Add("EI_BLOCK_REGULAR"); //営ブロック定
		totalValue.Columns.Add("EI_BLOCK_HO"); //営ブロック補
		totalValue.Columns.Add("KUSEKI_KAKUHO_NUM"); //空席確保数

		//DataAccessクラス生成
		Room_DA dataAccess = new Room_DA();

		foreach (DataRow row in PrmData.Prmtable.Rows)
		{
			//DBパラメータ
			Hashtable paramInfoList = new Hashtable();
			DataTable returnValue = null;
			string flgOn = "1";
			string flgOff = "0";

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
				returnValue = dataAccess.accessRoomTehai(Room_DA.accessType.getRoom, paramInfoList);

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
					// 1名参加変換
					if (row2["ONE_SANKA_FLG"].ToString() == "Y")
					{
						row3["ONE_SANKA_FLG"] = flgOn;
					}
					else
					{
						row3["ONE_SANKA_FLG"] = flgOff;
					}
					// 相部屋変換
					if (row2["AIBEYA_USE_FLG"].ToString() == "Y")
					{
						row3["AIBEYA_USE_FLG"] = flgOn;
					}
					else
					{
						row3["AIBEYA_USE_FLG"] = flgOff;
					}
					// 定員制変換
					if (row2["TEIINSEI_FLG"].ToString() == "Y")
					{
						row3["TEIINSEI_FLG"] = flgOn;
					}
					else
					{
						row3["TEIINSEI_FLG"] = flgOff;
					}
					row3["YOYAKU_NUM_TEISEKI"] = row2["YOYAKU_NUM_TEISEKI"];
					row3["YOYAKU_NUM_SUB_SEAT"] = row2["YOYAKU_NUM_SUB_SEAT"];
					row3["CRS_BLOCK_CAPACITY"] = row2["CRS_BLOCK_CAPACITY"];
					row3["CRS_BLOCK_ROOM_NUM"] = row2["CRS_BLOCK_ROOM_NUM"];
					row3["CRS_BLOCK_ONE_1R"] = row2["CRS_BLOCK_ONE_1R"];
					row3["CRS_BLOCK_TWO_1R"] = row2["CRS_BLOCK_TWO_1R"];
					row3["CRS_BLOCK_THREE_1R"] = row2["CRS_BLOCK_THREE_1R"];
					row3["CRS_BLOCK_FOUR_1R"] = row2["CRS_BLOCK_FOUR_1R"];
					row3["CRS_BLOCK_FIVE_1R"] = row2["CRS_BLOCK_FIVE_1R"];
					row3["ROOM_ZANSU_SOKEI"] = row2["ROOM_ZANSU_SOKEI"];
					row3["ROOM_ZANSU_ONE_ROOM"] = row2["ROOM_ZANSU_ONE_ROOM"];
					row3["ROOM_ZANSU_TWO_ROOM"] = row2["ROOM_ZANSU_TWO_ROOM"];
					row3["ROOM_ZANSU_THREE_ROOM"] = row2["ROOM_ZANSU_THREE_ROOM"];
					row3["ROOM_ZANSU_FOUR_ROOM"] = row2["ROOM_ZANSU_FOUR_ROOM"];
					row3["ROOM_ZANSU_FIVE_ROOM"] = row2["ROOM_ZANSU_FIVE_ROOM"];
					row3["JYOSYA_CAPACITY"] = row2["JYOSYA_CAPACITY"];
					row3["CAPACITY_HO_1KAI"] = row2["CAPACITY_HO_1KAI"];
					row3["CAPACITY_REGULAR"] = row2["CAPACITY_REGULAR"];
					row3["YOYAKU_KANOU_NUM"] = row2["YOYAKU_KANOU_NUM"];
					row3["SYSTEM_UPDATE_DAY"] = row2["SYSTEM_UPDATE_DAY"];
					row3["SYSTEM_UPDATE_PERSON_CD"] = row2["SYSTEM_UPDATE_PERSON_CD"];
					row3["SYSTEM_UPDATE_PGMID"] = row2["SYSTEM_UPDATE_PGMID"];
					row3["UKETOME_KBN_ONE_1R"] = row2["UKETOME_KBN_ONE_1R"];
					row3["UKETOME_KBN_TWO_1R"] = row2["UKETOME_KBN_TWO_1R"];
					row3["UKETOME_KBN_THREE_1R"] = row2["UKETOME_KBN_THREE_1R"];
					row3["UKETOME_KBN_FOUR_1R"] = row2["UKETOME_KBN_FOUR_1R"];
					row3["UKETOME_KBN_FIVE_1R"] = row2["UKETOME_KBN_FIVE_1R"];
					row3["ROOMING_TOTAL"] = row2["ROOMING_TOTAL"];
					row3["ROOMING_BETU_NINZU_1"] = row2["ROOMING_BETU_NINZU_1"];
					row3["ROOMING_BETU_NINZU_2"] = row2["ROOMING_BETU_NINZU_2"];
					row3["ROOMING_BETU_NINZU_3"] = row2["ROOMING_BETU_NINZU_3"];
					row3["ROOMING_BETU_NINZU_4"] = row2["ROOMING_BETU_NINZU_4"];
					row3["ROOMING_BETU_NINZU_5"] = row2["ROOMING_BETU_NINZU_5"];
					//row3("SIIRE_SAKI_CD") = row2("SIIRE_SAKI_CD")
					//row3("SIIRE_SAKI_EDABAN") = row2("SIIRE_SAKI_EDABAN")
					row3["ROOM_MAX_CAPACITY"] = row2["ROOM_MAX_CAPACITY"];
					row3["AIBEYA_YOYAKU_NINZU_MALE"] = row2["AIBEYA_YOYAKU_NINZU_MALE"];
					row3["AIBEYA_YOYAKU_NINZU_JYOSEI"] = row2["AIBEYA_YOYAKU_NINZU_JYOSEI"];
					row3["USING_FLG"] = row2["USING_FLG"];
					row3["HENKOU_KAHI_KBN"] = row2["HENKOU_KAHI_KBN"];
					row3["UPDATE_KBN"] = row2["UPDATE_KBN"];
					row3["HIDDEN_GOUSYA"] = row2["GOUSYA"].ToString().PadLeft(3, '0');
					row3["BLOCK_KAKUHO_NUM"] = row2["BLOCK_KAKUHO_NUM"];
					row3["EI_BLOCK_REGULAR"] = row2["EI_BLOCK_REGULAR"];
					row3["EI_BLOCK_HO"] = row2["EI_BLOCK_HO"];
					row3["KUSEKI_KAKUHO_NUM"] = row2["KUSEKI_KAKUHO_NUM"];

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