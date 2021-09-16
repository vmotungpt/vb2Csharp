using Hatobus.ReservationManagementSystem.Master;
using System.ComponentModel;


/// <summary>
/// 添乗員一括入力
/// </summary>
public class S04_0701 : PT11
{

	#region  定数／変数宣言
	private bool checkDisplayFlg = true; // 表示/非表示ボタン用フラグ(True:表示, False:非表示)

	private ButtonEx SetButton = null; // コントロール 表示/非表示ボタン
	private GroupBoxEx SetGroupBox = null; // コントロール 検索条件
	private PanelEx SetPanel = null; // コントロール パネル
	private FlexGridEx SetGrid = null; // コントロール グリッド
	private int HeightgbxConditionToPanelEx1 = 0; // 検索条件と一覧の間

	private DataTable selectOldData = new DataTable(); // 検索後のデータを保持
	private DataTable updateTargetData = new DataTable(); // 更新対象データを保持

	// 更新時のメッセージ
	private string UpdateMsg = "添乗員コード";
	private string InsertMsg = "添乗員コード";

	// メッセージ表示項目
	private const string MessageDisplay = "更新";
	private const string MessageDisplaySyuptDay = "出発日";
	private const string MessageTenjyoincd = "添乗員コード";
	private const string MessageNotInput = "未入力分のみ";

	// 処理日取得用
	private int GetSystemDate;

	#endregion

	#region 列挙
	/// <summary>
	/// カラム定義
	/// </summary>
	/// <remarks></remarks>
	private enum Tenjyoinikkatu_Koumoku : int
	{
		[Value("出発日")]
		syuptday = 1,
		[Value("コースコード")]
		crscd,
		[Value("コース名")]
		crsname,
		[Value("号車")]
		gousya,
		[Value("乗車地")]
		placename1,
		[Value("時刻")]
		syupttime1,
		[Value("添乗員コード")]
		tenjyoincd,
		[Value("添乗員名")]
		tenjyoinname,
		[Value("使用中フラグ")]
		usingflg,
		[Value("差分フラグ")]
		differenceflg,
		[Value("更新対象フラグ")]
		uptargetflg
	}

	/// <summary>
	/// 対象フラグ
	/// </summary>
	/// <remarks></remarks>
	public sealed class Targetflg
	{
		[Value("対象")]
		public const string Target = "Y";
		[Value("対象外")]
		public const string NotTarget = null;
	}
	#endregion

	#region イベント

	#region 表示／非表示ボタン
	/// <summary>
	/// 条件GroupBox表示制御ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnVisiblerCondition_Click(object sender, EventArgs e)
	{

		//表示／非表示 制御
		ClientCommonKyushuUtil.setVisiblerCondition(SetButton, SetGroupBox, SetPanel, SetGrid, checkDisplayFlg, HeightgbxConditionToPanelEx1);

	}

	#endregion

	#region グリッド関連
	/// <summary>
	/// ソート処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdList_AfterSort(object sender, C1.Win.C1FlexGrid.SortColEventArgs e)
	{

		// 添乗員コードの入力制御
		// 使用中フラグが立っている行の添乗員コードを入力不可に設定する

		for (int i = 1; i <= grdList.Rows.Count - 1; i++)
		{

			if (grdList(i, Tenjyoinikkatu_Koumoku.usingflg).ToString() == FixedCd.UsingFlg.Use)
			{
				grdList.Rows(i).AllowEditing = false;
			}
			else
			{
				// 出発日 ＜ 処理日の行の添乗員コードを入力不可に設定する
				if (System.Convert.ToInt32(Strings.Replace(System.Convert.ToString(grdList(i, Tenjyoinikkatu_Koumoku.syuptday).ToString()), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0)) < GetSystemDate)
				{
					grdList.Rows(i).AllowEditing = false;
				}
				else
				{
					grdList.Rows(i).AllowEditing = true;
				}
			}

		}

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

		//データ件数を表示
		ClientCommonKyushuUtil.setGridCount(grdList, lblLengthGrd);

	}

	#region F11(更新)ボタン押下時

	/// <summary>
	/// 更新処理
	/// </summary>
	protected override bool updateProc()
	{

		//DataAccessクラス生成
		TenjyoinIkkatuInput_DA dataAccess = new TenjyoinIkkatuInput_DA();

		bool returnResult = false;

		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{

			if (grdList(i, Tenjyoinikkatu_Koumoku.uptargetflg).ToString() == Targetflg.Target)
			{

				//更新処理実行
				if (UpdateTenjyoinIkkatuInput(i) > 0)
				{
					returnResult = true;
				}

			}

		}

		return returnResult;

	}

	/// <summary>
	/// 更新実行
	/// </summary>
	/// <param name="i">行数</param>
	/// <returns>更新処理件数</returns>
	private int UpdateTenjyoinIkkatuInput(int i)
	{

		// 戻り値
		int returnValue = 0;
		// DataAccessクラス生成
		TenjyoinIkkatuInput_DA dataAccess = new TenjyoinIkkatuInput_DA();
		// パラメータ
		Hashtable paramInfoList = new Hashtable();

		paramInfoList.Add("SyuptDay", (Integer?)(Strings.Replace(System.Convert.ToString(this.grdList.GetData(i, Tenjyoinikkatu_Koumoku.syuptday)), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0)));
		paramInfoList.Add("CrsCd", System.Convert.ToString(this.grdList.GetData(i, Tenjyoinikkatu_Koumoku.crscd)));
		paramInfoList.Add("Gousya", System.Convert.ToInt32(this.grdList.GetData(i, Tenjyoinikkatu_Koumoku.gousya)));
		paramInfoList.Add("TenjyoinCd", System.Convert.ToString(this.grdList.GetData(i, Tenjyoinikkatu_Koumoku.tenjyoincd)));
		paramInfoList.Add("UpdateDate", CommonProcess.getDateTime());
		paramInfoList.Add("UpdatePgmid", this.Name);
		paramInfoList.Add("UpdatePersonCd", UserInfoManagement.userId);

		try
		{
			//更新処理
			returnValue = System.Convert.ToInt32(dataAccess.executeTenjyoinIkkatuInput(paramInfoList));

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
	/// 更新処理前のチェック
	/// </summary>
	/// <returns>True:エラーなし False:エラーあり</returns>
	protected override bool checkBeforeUpdate()
	{

		bool differenceFlg = false;
		bool notExistMasterFlg = false;
		DataRow[] selectData = null;
		string whereString = string.Empty;

		bool updateFlg = false;
		DataTable returnValue = new DataTable();

		//フォーカスセットフラグ
		bool focusSetFlg = false;
		//エラー表示項目
		string errorDisplay = string.Empty;

		//エラーの初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			//グリッドの背景色をクリア
			grdList.GetCellRange(i, Tenjyoinikkatu_Koumoku.tenjyoincd).StyleNew.BackColor = BackColorType.Standard;

			//グリッドの差分フラグをクリア
			this.grdList[i, Tenjyoinikkatu_Koumoku.differenceflg] = string.Empty;
		}

		//更新前のグリッドを退避
		updateTargetData = null;
		updateTargetData = ((DataTable)this.grdList.DataSource).Copy;

		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (checkTenjyoinCdDifference(i) == true)
			{

				this.grdList[i, Tenjyoinikkatu_Koumoku.differenceflg] = Targetflg.Target;

				//条件の設定
				whereString = "SYUPT_DAY = '" + System.Convert.ToString(this.grdList(i, Tenjyoinikkatu_Koumoku.syuptday)) + "'" +;
				" AND CRS_CD = '" + (this.grdList(i, Tenjyoinikkatu_Koumoku.crscd), string) + "'" +;
				" AND GOUSYA = '" + (this.grdList(i, Tenjyoinikkatu_Koumoku.gousya), string) + "'";

				//問合せ対象データ取得
				selectData = updateTargetData.Select(whereString);

				if (selectData.Length > 0)
				{
					selectData[0][System.Convert.ToInt32("DIFFERENCE_FLG")] = Targetflg.Target;

					// 添乗員コード存在チェック

					{
						string.IsNullOrEmpty(getTenjyoinNm(System.Convert.ToString(System.Convert.ToString(grdList(i, Tenjyoinikkatu_Koumoku.tenjyoincd)).Trim()))) = System.Convert.ToBoolean(true);

						grdList.GetCellRange(i, Tenjyoinikkatu_Koumoku.tenjyoincd).StyleNew.BackColor = BackColorType.InputError;

						notExistMasterFlg = true;

					}

					differenceFlg = true;
				}
			}
		}

		if (notExistMasterFlg == true)
		{
			// 該当の{1}が存在しません。
			CommonProcess.createFactoryMsg().messageDisp("E90_014", MessageTenjyoincd);
			return false;
		}

		if (differenceFlg == false)
		{
			// 入力に差分がない場合、エラーメッセージを表示
			CommonProcess.createFactoryMsg().messageDisp("E90_049");
			return false;
		}

		//メッセージ出力(更新します。よろしいですか？)
		if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", MessageDisplay) == MsgBoxResult.Ok)
		{
			//更新処理前使用中フラグチェック
			returnValue = checkUsingFlgBeforeUpdate();
			foreach (DataRow row in returnValue.Rows)
			{
				if (row["UP_TARGET_FLG"].ToString() == Targetflg.Target)
				{
					for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
					{
						if (this.grdList(i, Tenjyoinikkatu_Koumoku.differenceflg).ToString() == Targetflg.Target)
						{
							if (System.Convert.ToString(row["SYUPT_DAY"]).Equals(System.Convert.ToString(this.grdList(i, Tenjyoinikkatu_Koumoku.syuptday))) &&)
							{
								System.Convert.ToString(row["CRS_CD"]).Equals(System.Convert.ToString(this.grdList(i, Tenjyoinikkatu_Koumoku.crscd))) &&;
								System.Convert.ToInt32(row["GOUSYA"]).Equals(System.Convert.ToInt32(this.grdList(i, Tenjyoinikkatu_Koumoku.gousya)));
								this.grdList[i, Tenjyoinikkatu_Koumoku.uptargetflg] = Targetflg.Target;
								updateFlg = true;
								break;
							}
						}
					}
				}
			}

			if (updateFlg == false)
			{
				//更新対象がない場合、エラーメッセージを表示
				CommonProcess.createFactoryMsg().messageDisp("E03_022");
				return false;
			}
		}
		else
		{
			return false;
		}

		return true;

	}

	#endregion

	/// <summary>
	/// グリッドデータ押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void flex_ValidateEdit(object sender, C1.Win.C1FlexGrid.ValidateEditEventArgs e)
	{

		int row = System.Convert.ToInt32(this.grdList.Row);

		this.grdList.SetData(row, Tenjyoinikkatu_Koumoku.tenjyoinname, getTenjyoinNm(System.Convert.ToString(grdList.Editor.Text)));

	}

	/// <summary>
	/// グリッドのStartEditイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks></remarks>
	private void grdList_StartEdit(System.Object sender, RowColEventArgs e)
	{

		if (e.Col == Tenjyoinikkatu_Koumoku.tenjyoincd)
		{
			TextBoxEx txtCd = new TextBoxEx();
			txtCd.Format = setControlFormat(ControlFormat.HankakuEiSuji);
			txtCd.MaxLength = 5;
			this.grdList.Editor = txtCd;
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

		MTenjyoinEntity ReturnyEntitMTenjyoin = null;

		try
		{
			S90_0103 cmsel = new S90_0103(ReturnyEntitMTenjyoin);

			cmsel.ShowDialog(this);

			if (!ReferenceEquals(cmsel.getReturnEntity, null))
			{
				ReturnyEntitMTenjyoin = (MTenjyoinEntity)cmsel.getReturnEntity;
				this.grdList.SetData(e.Row, Tenjyoinikkatu_Koumoku.tenjyoincd, ReturnyEntitMTenjyoin.tenjyoinCd.Value);
				this.grdList.SetData(e.Row, Tenjyoinikkatu_Koumoku.tenjyoinname, ReturnyEntitMTenjyoin.tenjyoinName.Value);
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

	#region    条件クリア
	/// <summary>
	/// 条件クリアボタン押下時
	/// </summary>
	protected override void btnCLEAR_ClickOrgProc()
	{

		// 検索条件部の項目初期化
		initSearchAreaItems();

		// データ件数を表示
		ClientCommonKyushuUtil.setGridCount(grdList, lblLengthGrd);

		// 更新ボタンを使用不可に設定
		this.F11Key_Enabled = false;

	}

	#endregion

	#region フォーム
	private void PT11_KeyDown(object sender, KeyEventArgs e)
	{

		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			base.btnCom_Click(this.btnSearch, e);
		}
		else
		{
			return;
		}

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

		//Controlの初期値を設定
		CommonUtil.Control_Init(this.gbxCondition.Controls);

		//エラーの初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		this.dtmSyuptDayFromTo.FromDateText = (Date?)(CommonDateUtil.getSystemTime()); // 出発日From
		this.dtmSyuptDayFromTo.ToDateText = (Date?)(CommonDateUtil.getSystemTime()); // 出発日To
		this.rdoCrsKindHatoBusTeiki.Select(); // 選択(初期値：はとバス定期)
		this.ucoCrsCd.setControlInitiarize(); // コースコード
		this.ucoTenjyoinCd.setControlInitiarize(); // 添乗員コード
		this.chkTenjyoinCd_NotInputBun.Checked = false; // 未入力分のみ

	}

	#endregion

	#region チェック系
	/// <summary>
	/// 検索入力項目チェック
	/// </summary>
	protected override bool checkSearchItems()
	{

		//フォーカスセットフラグ
		bool focusSetFlg = false;
		//エラー表示項目
		string errorDisplay = string.Empty;

		//エラーの初期化
		base.clearExistErrorProperty(this.gbxCondition.Controls);

		// 出発日必須チェック
		if (this.dtmSyuptDayFromTo.FromDateText.ToString().Trim() == "")
		{
			dtmSyuptDayFromTo.ExistErrorForFromDate = true;
			// 共通メッセージ処理[E90_022 未入力項目があります。]
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			dtmSyuptDayFromTo.Focus();

			return false;
		}

		// 出発日整合性チェック

		{
			!this.dtmSyuptDayFromTo.ToDateText,null;
			//出発日の大小チェック
			if (CommonDateUtil.chkDayFromTo(System.Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText), System.Convert.ToDateTime(dtmSyuptDayFromTo.ToDateText)) == false)
			{
				dtmSyuptDayFromTo.ExistErrorForFromDate = true;
				dtmSyuptDayFromTo.ExistErrorForToDate = true;
				dtmSyuptDayFromTo.Focus();
				//「{1}の設定が不正です。」のエラーを表示
				CommonProcess.createFactoryMsg().messageDisp("E90_017", MessageDisplaySyuptDay);

				return false;
			}
		}

		// 添乗員コード・未入力分のみ整合性チェック
		if (this.ucoTenjyoinCd.CodeText.Trim() != "" && this.chkTenjyoinCd_NotInputBun.Checked == true)
		{
			ucoTenjyoinCd.ExistError = true;

			// 共通メッセージ処理[E90_052	{1}と{2}の入力が不整合です。]
			CommonProcess.createFactoryMsg().messageDisp("E90_052", MessageTenjyoincd, MessageNotInput);
			ucoTenjyoinCd.Focus();

			return false;
		}

		return true;

	}

	/// <summary>
	/// 検索処理後使用中フラグチェック処理
	/// </summary>
	protected override void checkUsingFlgAfterSearch()
	{

		// 添乗員コードの入力制御
		// 使用中フラグが立っている行の添乗員コードを入力不可に設定する

		for (int i = 1; i <= grdList.Rows.Count - 1; i++)
		{
			if (grdList(i, Tenjyoinikkatu_Koumoku.usingflg).ToString() == FixedCd.UsingFlg.Use)
			{
				grdList.Rows(i).AllowEditing = false;
			}
		}

		// 出発日 ＜ 処理日の行の添乗員コードを入力不可に設定する
		for (int i = 1; i <= grdList.Rows.Count - 1; i++)
		{
			if (System.Convert.ToInt32(Strings.Replace(System.Convert.ToString(grdList(i, Tenjyoinikkatu_Koumoku.syuptday).ToString()), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0)) < GetSystemDate)
			{
				grdList.Rows(i).AllowEditing = false;
			}
		}

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

		//データ件数を表示
		ClientCommonKyushuUtil.setGridCount(grdList, lblLengthGrd);

		// セルボタンを常に表示 (初期値：Inherit)
		this.grdList.ShowButtons = ShowButtonsEnum.Always;

		//セルボタン表示
		this.grdList.Cols(Tenjyoinikkatu_Koumoku.tenjyoincd).ComboList = "|...";

		//ボタンの関連付け
		btnSearch.Click += base.btnCom_Click;
		btnClear.Click += base.btnCom_Click;

		//非使用ボタン設定
		this.F4Key_Visible = false; // F4 :CSV出力
		this.F10Key_Visible = false; // F10:登録

		base.UpdateMsgParam = this.UpdateMsg;
		base.InsertMsgParam = this.InsertMsg;

		//グリッド初期化
		setGridInitialize();

		//表示／非表示ボタン
		SetButton = TryCast(this.btnVisiblerCondition, ButtonEx);
		SetGroupBox = TryCast(this.gbxCondition, GroupBoxEx);
		SetPanel = TryCast(this.PanelEx1, PanelEx);
		SetGrid = TryCast(this.grdList, FlexGridEx);

		HeightgbxConditionToPanelEx1 = System.Convert.ToInt32(SetPanel.Top - (SetGroupBox.Top + SetGroupBox.Height)); //検索条件と一覧の間

		// 処理日取得
		GetSystemDate = System.Convert.ToInt32(Strings.Replace(System.Convert.ToString((Date?)(CommonDateUtil.getSystemTime().ToString("yyyy/MM/dd"))), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));

		// 更新ボタンを使用不可に設定
		this.F11Key_Enabled = false;
	}
	#endregion

	#region Grid、データ関連
	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void reloadGrid()
	{

		//取得結果をグリッドへ設定
		object with_1 = this.grdList;
		with_1.DataSource = base.SearchResultGridData;
		//グリッド設定
		setGridItem();
		if (with_1.Rows.Count == with_1.Rows.Fixed)
		{
			//更新ボタンを使用不可に設定
			this.F11Key_Enabled = false;
		}
		else
		{
			//更新ボタンを使用可に設定
			this.F11Key_Enabled = true;
		}

		// 検索時のグリッドを差分用に退避
		selectOldData = null;
		selectOldData = ((DataTable)this.grdList.DataSource).Copy;

	}

	#endregion

	#region DB関連
	//'■■■ＤＢ関連処理■■■
	/// <summary>
	/// キー値での問合せ
	/// </summary>
	protected override DataTable getMstDataByPrimaryKey()
	{
		return GetDbTable(DbTableKbn.Key);
	}

	/// <summary>
	/// 対象マスタのデータ取得
	/// </summary>
	protected override DataTable getMstData()
	{
		return GetDbTable(DbTableKbn.Select);
	}

	//■■■ＤＢ関連処理■■■
	/// <summary>
	/// 対象マスタのデータ取得
	/// </summary>
	private string getTenjyoinNm(string TenjyoinCd)
	{
		string returnValue = "";

		//DataAccessクラス生成
		TenjyoinIkkatuInput_DA dataAccess = new TenjyoinIkkatuInput_DA();
		//戻り値
		DataTable returnValue = null;
		//パラメータ
		Hashtable paramInfoList = new Hashtable();

		returnValue = "";

		// 添乗員コード
		if (!string.IsNullOrEmpty(TenjyoinCd))
		{

			paramInfoList.Add("TenjyoinCd", TenjyoinCd);

			try
			{
				//一覧データ取得処理
				returnValue = dataAccess.accessTenjyoinIkkatuInput(TenjyoinIkkatuInput_DA.accessType.getTenjyoinNm, paramInfoList);
			}
			catch (OracleException)
			{
				throw;
			}
			catch (Exception)
			{
				throw;
			}

			if (returnValue.Rows.Count == 1)
			{
				return System.Convert.ToString(returnValue.Rows(0).Item("TENJYOIN_NAME").ToString());
			}

		}

		return returnValue;

	}

	#endregion
	#endregion

	#region 実装用メソッド(画面毎に変更)

	#region 初期処理

	/// <summary>
	/// グリッド初期化
	/// </summary>
	/// <remarks></remarks>
	private void setGridInitialize()
	{

		C1.Win.C1FlexGrid.CellRange rng = null;

		object with_1 = this.grdList;
		with_1.DataSource = null;
		with_1.Rows.Count = with_1.Rows.Fixed;

		// ヘッダ設定
		with_1.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;

		rng = with_1.GetCellRange(0, Tenjyoinikkatu_Koumoku.tenjyoincd, 0, Tenjyoinikkatu_Koumoku.tenjyoinname);
		rng.Data = "添乗員";
		with_1.MergedRanges.Add(rng);

		// 使用フラグ ～ 更新対象フラグ は非表示
		for (int idx = Tenjyoinikkatu_Koumoku.usingflg; idx <= Tenjyoinikkatu_Koumoku.uptargetflg; idx++)
		{
			with_1.Cols(idx).Visible = false;
		}


	}

	#endregion

	#region グリッド設定

	/// <summary>
	/// グリッド設定
	/// </summary>
	/// <remarks></remarks>
	private void setGridItem()
	{

		object with_1 = this.grdList;

		// セルの配置と自動サイズ調整
		with_1.Styles.Fixed.TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;


	}

	#endregion

	#region チェック処理(Private)

	#region 変更確認

	/// <summary>
	/// 差分チェック
	/// </summary>
	protected override bool checkDifference()
	{

		//入力差分チェック
		for (int i = 1; i <= this.grdList.Rows.Count - 1; i++)
		{
			if (checkTenjyoinCdDifference(i) == true)
			{
				return true;
			}
		}

		return false;

	}

	#endregion

	/// <summary>
	/// 入力差分チェック処理
	/// </summary>
	/// <param name="i">行数</param>
	/// <returns>True:差分あり False:差分なし</returns>
	private bool checkTenjyoinCdDifference(int i)
	{

		string tenjyoincd = string.Empty;

		//使用不可の行は読み飛ばす
		if (this.grdList.Rows(i).AllowEditing == false)
		{
			return false;
		}

		//引数の設定
		foreach (DataRow row in selectOldData.Rows)
		{
			if (row["SYUPT_DAY"].Equals(this.grdList(i, Tenjyoinikkatu_Koumoku.syuptday)) &&)
			{
				row["CRS_CD"].ToString().Equals(this.grdList(i, Tenjyoinikkatu_Koumoku.crscd)) &&;
				row["GOUSYA"].Equals(this.grdList(i, Tenjyoinikkatu_Koumoku.gousya));
				tenjyoincd = row["TENJYOIN_CD"].ToString();
			}
		}

		if (Information.IsDBNull(this.grdList(i, Tenjyoinikkatu_Koumoku.tenjyoincd)) == true)
		{
			this.grdList[i, Tenjyoinikkatu_Koumoku.tenjyoincd] = string.Empty;
		}

		if (tenjyoincd.Equals(System.Convert.ToString(this.grdList(i, Tenjyoinikkatu_Koumoku.tenjyoincd))) == false)
		{
			return true;
		}

		return false;

	}

	/// <summary>
	/// 更新処理前使用中フラグチェック処理
	/// </summary>
	/// <returns>DataTable</returns>
	private DataTable checkUsingFlgBeforeUpdate()
	{

		// DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		// DataAccessクラス生成
		TenjyoinIkkatuInput_DA dataAccess = new TenjyoinIkkatuInput_DA();
		DataTable returnValue = new DataTable();

		return dataAccess.updateUsingFlag(updateTargetData, this.Name);

		return returnValue;

	}


	#endregion
	#endregion

	#region DB取得処理

	/// <summary>
	/// データ取得処理(必須画面個別実装)
	/// </summary>
	/// <returns>取得データ(DataTable)</returns>
	private DataTable GetDbTable(DbTableKbn kbn)
	{

		// 戻り値
		DataTable returnValue = null;

		// DBパラメータ
		Hashtable paramInfoList = new Hashtable();

		//DataAccessクラス生成
		TenjyoinIkkatuInput_DA dataAccess = new TenjyoinIkkatuInput_DA();

		// 出発日From
		if (!(ReferenceEquals(this.dtmSyuptDayFromTo.FromDateText, null)))
		{
			paramInfoList.Add("SyuptDayFr", Strings.Replace(System.Convert.ToString(this.dtmSyuptDayFromTo.FromDateText), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));
		}

		// 出発日To
		if (!(ReferenceEquals(this.dtmSyuptDayFromTo.ToDateText, null)))
		{
			paramInfoList.Add("SyuptDayTo", Strings.Replace(System.Convert.ToString(this.dtmSyuptDayFromTo.ToDateText), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));
		}

		// コース種別
		if (this.rdoCrsKindHatoBusTeiki.Checked == true)
		{
			//はとバス定期の場合
			paramInfoList.Add("CrsKind", FixedCd.CrsKindType.hatoBusTeiki);
		}
		else if (this.rdoCrsKindHigaeri.Checked == true)
		{
			//日帰りの場合
			paramInfoList.Add("CrsKind", FixedCd.CrsKindType.higaeri);
		}
		else if (this.rdoCrsKindStay.Checked == true)
		{
			//宿泊の場合
			paramInfoList.Add("CrsKind", FixedCd.CrsKindType.stay);
		}
		else if (this.rdoCrsKindRcourse.Checked == true)
		{
			//Rコースの場合
			paramInfoList.Add("CrsKind", FixedCd.CrsKindType.rcourse);

		}

		// コースコード
		string CrsCd = System.Convert.ToString(this.ucoCrsCd.CodeText);
		if (!string.IsNullOrEmpty(CrsCd))
		{
			paramInfoList.Add("CrsCd", CrsCd);
		}

		// 添乗員コード
		string TenjyoinCd = System.Convert.ToString(this.ucoTenjyoinCd.CodeText);
		if (!string.IsNullOrEmpty(TenjyoinCd))
		{
			paramInfoList.Add("TenjyoinCd", TenjyoinCd);
		}

		// 未入力分のみ
		if (this.chkTenjyoinCd_NotInputBun.Checked == true)
		{
			paramInfoList.Add("NotInputBun", FixedCd.CheckBoxValue.OnValue);
		}
		else
		{
			paramInfoList.Add("NotInputBun", FixedCd.CheckBoxValue.OffValue);
		}

		try
		{
			returnValue = dataAccess.accessTenjyoinIkkatuInput(TenjyoinIkkatuInput_DA.accessType.getTenjyoinIkkatuInput, paramInfoList);
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

	#endregion

}