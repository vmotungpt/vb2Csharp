using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.Zaseki;


/// <summary>
/// S02_2302 台帳・座席・予約使用中表示・解除
/// </summary>
public class S02_2302 : PT11
{

	#region 定数
	/// <summary>
	/// 画面ID
	/// </summary>
	private const string PgmId = "S02_2302";
	/// <summary>
	/// 画面名
	/// </summary>
	private const string ScreenName = "台帳・座席・予約使用中表示・解除";
	/// <summary>
	/// 条件GroupBoxのTop座標
	/// </summary>
	public const int TopGbxCondition = 41;
	/// <summary>
	/// 条件GroupBoxのマージン
	/// </summary>
	public const int MarginGbxCondition = 6;
	/// <summary>
	/// 列番号 予約区分（非表示）
	/// </summary>
	private const int NoColYoyakuKbn = 11;
	/// <summary>
	/// 列番号 予約NO（非表示）
	/// </summary>
	private const int NoColYoyakuNo = 12;
	/// <summary>
	/// 列番号 発券状態(非表示)
	/// </summary>
	private const int NoColHakkenState = 13;
	/// <summary>
	/// 列番号 座席指定予約フラグ(非表示)
	/// </summary>
	private const int NoColZasekiReserveYoyakuFlg = 14;
	/// <summary>
	/// 予約番号のカラム番号
	/// </summary>
	private const int YoyakuNoColumnNo = 2;
	/// <summary>
	/// 1文字目
	/// </summary>
	private const int FirstLetter = 0;
	/// <summary>
	/// 2文字目
	/// </summary>
	private const int SecondCharacter = 1;
	/// <summary>
	/// 出発日初期値
	/// </summary>
	private const string SyuptDayInitialValue = "____/__/__";
	/// <summary>
	/// 座席数表示フォーマット
	/// </summary>
	private const string ZasekiNumFormat = "{0} + {1}";
	/// <summary>
	/// 座席指定状態：空
	/// </summary>
	private const string StateEmpty = "空";
	/// <summary>
	/// 座席指定状態：消
	/// </summary>
	private const string StateCancel = "消";
	/// <summary>
	/// 座席指定状態：作
	/// </summary>
	private const string StateCreating = "作";
	/// <summary>
	/// 座席指定状態：連
	/// </summary>
	private const string StateCollaboration = "連";
	/// <summary>
	/// 座席指定状態：仮
	/// </summary>
	private const string StateTemp = "仮";
	/// <summary>
	/// 座席指定状態：指
	/// </summary>
	private const string StateDesignation = "指";
	/// <summary>
	/// 座席指定状態：券
	/// </summary>
	private const string StateHakken = "券";
	/// <summary>
	/// 座席指定状態：*
	/// </summary>
	private const string StateOther = "*";
	/// <summary>
	/// 使用中フラグ解除
	/// </summary>
	private const string UsingFlagOff = " ";

	#endregion

	#region 変数
	/// <summary>
	/// 検索後に格納する出発日
	/// </summary>
	private DateTime _syuptDay;
	/// <summary>
	/// 検索後に格納する号車
	/// </summary>
	private int _gousya;
	/// <summary>
	/// 検索後に格納するコースコード
	/// </summary>
	private string _crsCd;
	/// <summary>
	/// 号車情報
	/// </summary>
	private DataTable _gousyaInfo;
	/// <summary>
	/// 予約者情報一覧
	/// </summary>
	private DataTable _yoyakuList;
	/// <summary>
	/// 座席イメージ一覧
	/// </summary>
	private DataTable _zasekiImageList;
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
	/// フォーム起動時の独自処理
	/// </summary>
	protected override void StartupOrgProc()
	{

		// 画面初期化
		this.setControlInitiarize();
	}

	/// <summary>
	/// 条件GroupBox表示制御ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnVisiblerCondition_Click(object sender, EventArgs e)
	{
		this.VisibleGbxCondition = !this.VisibleGbxCondition;

		//Panel, グリッドの座標, サイズを表示/非表示に応じて変更
		if (this.VisibleGbxCondition)
		{
			//表示状態
			this.btnHihyoji.Text = FixedCd.CommonButtonTextType.hiHyoji;

			this.PanelEx1.Top = TopGbxCondition + this.HeightGbxCondition + MarginGbxCondition;
			this.PanelEx1.Height -= this.HeightGbxCondition + MarginGbxCondition;
			this.grdSearchResult.Height -= this.HeightGbxCondition - 3;
		}
		else
		{
			//非表示状態
			this.btnHihyoji.Text = FixedCd.CommonButtonTextType.hyoji;

			this.PanelEx1.Top = TopGbxCondition;
			this.PanelEx1.Height += this.HeightGbxCondition + MarginGbxCondition;
			this.grdSearchResult.Height += this.HeightGbxCondition - 3;
		}
	}

	/// <summary>
	/// 画面終了時処理
	/// </summary>
	/// <returns></returns>
	protected override bool closingScreen()
	{

		return true;
	}

	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{

		this.Close();
	}

	/// <summary>
	/// F8：検索ボタン押下イベント
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{

		//エラークリア
		this.clearError();

		// 登録ボタン活性化
		base.F10Key_Enabled = false;

		//出発日入力チェック
		if (this.isSyuptDay() == false)
		{
			return;
		}

		//日付フォーマットチェック
		if (CommonDateUtil.isDateYYYYMMDD(txtSyuptDay.Text) == false)
		{

			//メッセージ出力（出発日が不正です。）
			CommonProcess.createFactoryMsg().messageDisp("E90_016", "出発日");
			txtSyuptDay.ExistError = true;
			this.ActiveControl = this.txtSyuptDay;
			return;
		}

		//号車入力チェック
		if (this.isGousya() == false)
		{
			return;
		}

		//コースコード入力チェック
		if (this.isCrsCd() == false)
		{
			return;
		}

		//入力された値をセット
		this.setInputValue();

		//データ取得
		this.setDataAgainGet();
	}

	/// <summary>
	/// F10:登録ボタン押下イベント
	/// </summary>
	protected override void btnF10_ClickOrgProc()
	{

		try
		{
			//メッセージ出力（更新処理を行います。よろしいですか？）
			if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", "更新") == MsgBoxResult.Cancel)
			{
				//キャンセルの場合、処理を抜ける
				return;
			}

			// 予約情報、コース台帳、座席イメージ使用中フラグロック
			if (this.executeUsingFlag(System.Convert.ToString(FixedCd.UsingFlg.Use)) == false)
			{

				return;
			}

			S02_2302Da s02_2302Da = new S02_2302Da();
			s02_2302Da.updateZaseki(this._gousyaInfo, this._yoyakuList, this._zasekiImageList, this._crsCd, int.Parse(this._syuptDay.ToString("yyyyMMdd")), this._gousya);

			// 予約情報、コース台帳、座席イメージ使用中フラグ解除
			this.executeUsingFlag(System.Convert.ToString(FixedCd.UsingFlg.Unused));

			//データの再取得
			this.setDataAgainGet();

			// 処理終了ログ＆メッセージ出力
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.entry, ScreenName, "登録処理");
			// {1}が完了しました。
			CommonProcess.createFactoryMsg().messageDisp("I90_002", "使用中解除処理");

		}
		catch (Exception)
		{

			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, ScreenName, "登録処理");
			// {1}に失敗しました。[{2}]
			CommonProcess.createFactoryMsg().messageDisp("E90_025", "使用中解除処理", "データ更新異常");
			throw;
		}
	}

	/// <summary>
	/// 条件クリアボタン押下時
	/// </summary>
	protected override void btnCLEAR_ClickOrgProc()
	{

		// 初期表示と同一の処理を実行
		this.setControlInitiarize();
	}

	/// <summary>
	/// キーダウンイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	protected void S02_2302_KeyDown(object sender, KeyEventArgs e)
	{

		if (e.KeyData == Keys.F8)
		{
			this.btnSearch.Select();
			e.Handled = true;
			this.btnSearch_Click(sender, e);
		}
		else
		{
			return;
		}
	}

	/// <summary>
	/// F8：検索ボタン押下イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void btnSearch_Click(object sender, EventArgs e)
	{

		// F8ボタン押下
		base.btnCom_Click(this.btnSearch, e);
		// log出力
		createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, ScreenName, "検索処理");
	}

	/// <summary>
	/// 条件クリアボタン押下時
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	protected void btnCLEAR_Click(object sender, EventArgs e)
	{

		// CLEARボタン押下
		base.btnCom_Click(this.btnClear, e);
	}

	/// <summary>
	/// グリッドのデータソース変更時イベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void FlexGridEx1_AfterDataRefresh(object sender, ListChangedEventArgs e)
	{
		//データ件数を表示(ヘッダー行分マイナス1)
		string formatedCount = System.Convert.ToString((this.grdSearchResult.Rows.Count - 1).ToString().PadLeft(6));
		this.lblSearchResultNum.Text = formatedCount + "件";
	}

	#endregion

	#region メソッド
	/// <summary>
	/// 画面初期化
	/// </summary>
	private void setControlInitiarize()
	{

		// フッタボタンの設定
		this.setButtonInitiarize();
		// 検索条件部の項目初期化
		this.setSearchJokenBu();
		//号車情報の初期表示
		this.setGousyaInfo();
		//grid初期表示
		this.setInitiarizeGrid();
		//合計数の初期表示
		this.setTotalNum();
		//エラークリア
		this.clearError();
	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{

		// フッタボタンの制御(表示\[活性]／非表示[非活性])
		base.initFooterButtonControl();
		//Visible
		this.F2Key_Visible = true;
		this.F4Key_Visible = false;
		this.F10Key_Visible = true;
		this.F11Key_Visible = false;

		// Enabled
		this.F10Key_Enabled = false;
	}

	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	private void setSearchJokenBu()
	{

		this.txtSyuptDay.Text = "";
		this.ucoCrsCd.CodeText = "";
		this.ucoCrsCd.ValueText = "";
		this.txtGousya.Text = "";
	}

	/// <summary>
	/// 号車情報の項目初期化
	/// </summary>
	private void setGousyaInfo()
	{

		this.txtCarType.Text = "";
		this.txtCarNo.Text = "";
		this.txtTeiseki.Text = "";
		this.txtKuseki.Text = "";
		this.txtEigyosyoBlock.Text = "";
	}

	/// <summary>
	/// 合計数の項目初期化
	/// </summary>
	private void setTotalNum()
	{

		this.txtTotalNinzu.Text = "";
		this.txtTotalZasekiNum.Text = "";
	}

	/// <summary>
	/// grid初期表示
	/// </summary>
	private void setInitiarizeGrid()
	{
		//(Nothingで初期化するとヘッダー名が消える)
		DataTable dt = new DataTable();
		this.grdSearchResult.DataSource = dt;
		this.grdSearchResult.DataMember = "";
		this.grdSearchResult.Refresh();
		this.lblSearchResultNum.Text = "0件";
	}

	/// <summary>
	/// エラークリア
	/// </summary>
	private void clearError()
	{

		this.txtSyuptDay.ExistError = false;
		this.txtGousya.ExistError = false;
		this.ucoCrsCd.ExistError = false;
	}

	/// <summary>
	/// 予約者情報一覧取得
	/// </summary>
	/// <param name="busReserveCd">バス指定コード</param>
	/// <returns>予約者情報一覧</returns>
	private DataTable getYoyakuList(string busReserveCd)
	{

		CrsLedgerBasicEntity entity = new CrsLedgerBasicEntity();
		entity.busReserveCd.Value = busReserveCd;
		entity.syuptDay.Value = int.Parse(this._syuptDay.ToString("yyyyMMdd"));
		entity.gousya.Value = this._gousya;

		S02_2302Da s02_2302Da = new S02_2302Da();
		DataTable yoyakuList = s02_2302Da.getYoyakuList(entity);

		// エラー情報カラム追加
		yoyakuList.Columns.Add("ERROR_CD");

		return yoyakuList;
	}

	/// <summary>
	/// 座席イメージ一覧取得
	/// </summary>
	/// <param name="busReserveCd">バス指定コード</param>
	/// <returns>座席イメージ一覧</returns>
	private DataTable getZasekiImageList(string busReserveCd)
	{

		Common.TZasekiImageEntity entity = new Common.TZasekiImageEntity();
		entity.BusReserveCd.Value = busReserveCd;
		entity.SyuptDay.Value = int.Parse(this._syuptDay.ToString("yyyyMMdd"));
		entity.Gousya.Value = this._gousya;

		S02_2302Da s02_2302Da = new S02_2302Da();
		DataTable zasekiImageList = s02_2302Da.getZasekiImageList(entity);

		return zasekiImageList;
	}

	/// <summary>
	/// 座席イメージ（バス情報）取得
	/// </summary>
	/// <param name="busReserveCd">バス指定コード</param>
	/// <returns>座席イメージ（バス情報）</returns>
	private TZasekiImageEntity getZasekiImage(string busReserveCd)
	{

		TZasekiImageEntity entity = new TZasekiImageEntity();
		entity.BusReserveCd.Value = busReserveCd;
		entity.SyuptDay.Value = int.Parse(this._syuptDay.ToString("yyyyMMdd"));
		entity.Gousya.Value = this._gousya;

		S02_2302Da s02_2302Da = new S02_2302Da();
		DataTable zasekiImage = s02_2302Da.getZasekiImage(entity);

		EntityOperation[] zaseki = YoyakuBizCommon.setEntityFromDataTable(Of TZasekiImageEntity)[zasekiImage];

		return zaseki.EntityData(0);
	}

	/// <summary>
	/// 座席イメージ（座席情報）一覧取得
	/// </summary>
	/// <param name="busReserveCd">バス指定コード</param>
	/// <returns>座席イメージ（座席情報）一覧</returns>
	private List[] getZasekiImageInfo(string busReserveCd)
	{

		TZasekiImageInfoEntity entity = new TZasekiImageInfoEntity();
		entity.BusReserveCd.Value = busReserveCd;
		entity.SyuptDay.Value = int.Parse(this._syuptDay.ToString("yyyyMMdd"));
		entity.Gousya.Value = this._gousya;

		S02_2302Da s02_2302Da = new S02_2302Da();
		DataTable zasekiImageInfo = s02_2302Da.getZasekiImageInfo(entity);

		EntityOperation[] zaseki = YoyakuBizCommon.setEntityFromDataTable(Of TZasekiImageInfoEntity)[zasekiImageInfo];

		List list = new List();
		foreach (TZasekiImageInfoEntity rec in zaseki.EntityData)
		{

			list.Add(rec);
		}

		return list;
	}

	/// <summary>
	/// 座席イメージ一覧取得
	/// </summary>
	/// <param name="busReserveCd">バス指定コード</param>
	/// <returns>座席イメージ一覧</returns>
	private DataTable getZasekiInfo(string busReserveCd)
	{

		// 座席イメージ一覧取得
		DataTable zasekiImageList = this.getZasekiImageList(busReserveCd);

		TZasekiImageEntity zasekiImage = this.getZasekiImage(busReserveCd);
		List[] zasekiImageInfo = this.getZasekiImageInfo(busReserveCd);

		if (zasekiImageList IsNot null && zasekiImageList.Rows.Count > 0)
		{
			zasekiImageList.Columns("ZASEKI").MaxLength = 90;
		}

		foreach (DataRow row in zasekiImageList.Rows)
		{

			int groupNo = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(row["GROUP_NO"]));
			string yoyakuKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["YOYAKU_KBN"]));
			int yoyakuNo = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(row["YOYAKU_NO"]));
			string carTypeCd = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["CAR_TYPE_CD"]));

			Z0007_Param param = new Z0007_Param();
			param.ProcessKbn = Z0007_Param.Z0007_Param_ProcessKbn.ProcessKbn_10;
			param.GroupNo = groupNo;
			param.YoyakuNo = yoyakuNo;
			param.CarType = carTypeCd;

			param.BusInfo = zasekiImage;
			param.ZasekiInfo = zasekiImageInfo;

			Z0007 z0007 = new Z0007();
			Z0007_Result result = z0007.Execute(param);

			row["ZASEKI"] = result.Zaseki;
		}

		// エラー情報カラム追加
		zasekiImageList.Columns.Add("ERROR_CD");

		return zasekiImageList;
	}

	/// <summary>
	/// 検索結果グリッドの設定
	/// </summary>
	/// <param name="busReserveCd">バス指定コード</param>
	private void setGrdSearchResult(string busReserveCd)
	{

		// 予約者情報一覧取得
		this._yoyakuList = this.getYoyakuList(busReserveCd);
		// 座席イメージ一覧取得
		this._zasekiImageList = this.getZasekiInfo(busReserveCd);

		DataTable dt = new DataTable();
		//列作成
		foreach (Column col in this.grdSearchResult.Cols)
		{

			dt.Columns.Add(col.Name);
		}

		for (index = 1; index <= 999; index++)
		{

			// グループNOをキーに、対象データ取得
			DataRow[] yoyakuRows = this._yoyakuList.Select(string.Format("GROUP_NO = {0}", index));
			DataRow[] zasekiRows = this._zasekiImageList.Select(string.Format("GROUP_NO = {0}", index));

			if (yoyakuRows.Length <= 0 && zasekiRows.Length <= 0)
			{
				// 予約者情報、座席イメージ情報の両方にデータがない場合、次レコードへ
				continue;
			}

			object dtRow = dt.NewRow;

			// 予約番号
			string yoyakuKbn = "";
			int yoyakuNo = 0;
			if (yoyakuRows.Length > 0)
			{

				yoyakuKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(yoyakuRows[0]["YOYAKU_KBN"]));
				yoyakuNo = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(yoyakuRows[0]["YOYAKU_NO"]));
			}
			else
			{

				yoyakuKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(zasekiRows[0]["YOYAKU_KBN"]));
				yoyakuNo = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(zasekiRows[0]["YOYAKU_NO"]));
			}

			dtRow["colYoyakuNo"] = CommonRegistYoyaku.createManagementNumber(yoyakuKbn, yoyakuNo);

			// グループID
			dtRow["colGroupId"] = index;

			if (yoyakuRows.Length > 0)
			{

				// 予約名
				dtRow["colNameBf"] = yoyakuRows[0]["SURNAME"].ToString() + Strings.Space(1) + yoyakuRows[0]["NAME"].ToString();
				// 予約人数
				dtRow["colYoyakuNinzu"] = yoyakuRows[0]["CHARGE_APPLICATION_NINZU"];
			}

			//座席指定状況
			dtRow["colZasekiReserveSituation"] = this.getZasekiState(zasekiRows, yoyakuRows);

			// 座席
			if (zasekiRows.Length > 0)
			{

				dtRow["colZaseki"] = CommonRegistYoyaku.convertObjectToString(zasekiRows[0]["ZASEKI"]);
			}

			// 使用中ユーザ
			if (yoyakuRows.Length > 0)
			{

				string usingFlg = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(yoyakuRows[0]["USING_FLG"]));
				if (usingFlg == CommonRegistYoyaku.FlagValueTrue)
				{

					string updatePersonCd = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(yoyakuRows[0]["UPDATE_PERSON_CD"]));

					dtRow["colUsingUser"] = updatePersonCd;
				}
			}

			// 予約座席比較確認
			this.setYoyakuZasekiErrCd(dtRow, yoyakuRows, zasekiRows);

			dt.Rows.Add(dtRow);
		}

		//グリッドにテーブルをセット
		this.grdSearchResult.DataSource = dt;
	}

	/// <summary>
	/// 座席指定状況取得
	/// </summary>
	/// <param name="zasekiRows">座席情報</param>
	/// <param name="yoyakuRows">予約情報</param>
	/// <returns>座席指定状況</returns>
	private string getZasekiState(DataRow[] zasekiRows, DataRow[] yoyakuRows)
	{

		string zasekiStatus = "";

		if (zasekiRows.Length <= 0)
		{
			// 座席情報がない場合、状態は空とする
			return zasekiStatus;
		}

		// 座席状態取得
		int zasekiState = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(zasekiRows[0]["ZASEKI_STATE"]));
		if (zasekiState <= 99)
		{

			//ステータス = 空
			zasekiStatus = StateEmpty;

			string cancelFlg = "";
			if (yoyakuRows.Length > 0)
			{

				cancelFlg = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(yoyakuRows[0]["CANCEL_FLG"]));
			}

			if (cancelFlg == CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.YoyakuCancelFlg.torikesi) ||)
			{
				cancelFlg = System.Convert.ToString(CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.YoyakuCancelFlg.sakujo));
				// 予約情報がキャンセルされている場合

				//ステータス = 消
				zasekiStatus = StateCancel;
			}
			else
			{

				string carTypeCd = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(this._gousyaInfo.Rows(0)["CAR_TYPE_CD"]));

				if (carTypeCd == "XX")
				{
					// 架空車種の場合

					if (cancelFlg == CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.YoyakuCancelFlg.sakuseichu))
					{
						// 予約情報が作成中の場合

						//ステータス = 作
						zasekiStatus = StateCreating;
					}
				}
				else
				{

					string state = "";
					if (yoyakuRows.Length > 0)
					{

						state = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(yoyakuRows[0]["STATE"]));
					}

					if (state == CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.State.ContactHakken))
					{
						// 予約情報.状態が連絡発券の場合

						//ステータス = 連
						zasekiStatus = StateCollaboration;
					}
					else
					{

						//ステータス = *
						zasekiStatus = StateOther;
					}
				}
			}
		}
		else if (zasekiState <= 299)
		{

			//ステータス = 仮
			zasekiStatus = StateTemp;

		}
		else if (zasekiState <= 499)
		{

			//ステータス = 作
			zasekiStatus = StateCreating;

		}
		else if (zasekiState <= 599)
		{

			//ステータス = 指
			zasekiStatus = StateDesignation;

		}
		else if (zasekiState <= 699)
		{

			//ステータス = 券
			zasekiStatus = StateHakken;

		}

		return zasekiStatus;
	}

	/// <summary>
	/// 予約座席比較確認
	/// </summary>
	/// <param name="dtRow">グリッドレコード</param>
	/// <param name="yoyakuRows">予約情報</param>
	/// <param name="zasekiRows">座席情報</param>
	private void setYoyakuZasekiErrCd(DataRow dtRow, DataRow[] yoyakuRows, DataRow[] zasekiRows)
	{

		// 車種コード
		string carTypeCd = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(this._gousyaInfo.Rows(0)["CAR_TYPE_CD"]));
		if (carTypeCd == "XX")
		{
			// バスを使用しない場合

			if (dtRow["colZasekiReserveSituation"].ToString() == StateCreating)
			{

				yoyakuRows[0]["ERROR_CD"] = "U";
			}

			return;
		}

		if (yoyakuRows.Length > 0)
		{
			// キャンセル予約が椅子を押えている場合

			string cancelFlg = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(yoyakuRows[0]["CANCEL_FLG"]));
			if (cancelFlg == CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.YoyakuCancelFlg.torikesi) ||)
			{
				cancelFlg = System.Convert.ToString(CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.YoyakuCancelFlg.sakujo));
				// 予約情報がキャンセルされている場合

				if (zasekiRows.Length > 0)
				{

					int ninzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(zasekiRows[0]["NINZU"]));
					if (ninzu > 0)
					{

						yoyakuRows[0]["ERROR_CD"] = "X";
						zasekiRows[0]["ERROR_CD"] = "X";
					}
				}

				return;
			}
		}

		if (zasekiRows.Length > 0)
		{

			int ninzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(zasekiRows[0]["NINZU"]));
			if (ninzu > 0 && yoyakuRows.Length == 0)
			{
				// 予約が見つからない場合

				zasekiRows[0]["ERROR_CD"] = "Y";
				return;
			}
		}

		if (yoyakuRows.Length > 0)
		{

			if (zasekiRows.Length <= 0)
			{
				// 座席が見つからない場合

				yoyakuRows[0]["ERROR_CD"] = "Z";
				return;
			}
		}

		if (zasekiRows.Length > 0)
		{

			// 座席状態取得
			int zasekiState = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(zasekiRows[0]["ZASEKI_STATE"]));

			if (zasekiState >= 300 && zasekiState <= 400)
			{
				// 作業中の状態の場合

				yoyakuRows[0]["ERROR_CD"] = "U";
				zasekiRows[0]["ERROR_CD"] = "U";
				return;
			}
		}

		if (yoyakuRows.Length > 0 && zasekiRows.Length > 0)
		{

			int yoyakuNinzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(yoyakuRows[0]["CHARGE_APPLICATION_NINZU"]));
			int zasekiNinzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(zasekiRows[0]["NINZU"]));

			if (yoyakuNinzu != zasekiNinzu)
			{
				// 予約人数と座席人数が異なる場合

				yoyakuRows[0]["ERROR_CD"] = "N";
				zasekiRows[0]["ERROR_CD"] = "N";
				return;
			}
		}

		if (yoyakuRows.Length > 0 && zasekiRows.Length > 0)
		{

			string hakkenNaiyo = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(yoyakuRows[0]["HAKKEN_NAIYO"]));

			if (hakkenNaiyo == CommonRegistYoyaku.convertObjectToString(FixedCdYoyaku.HakkenNaiyo.allkinHakken) ||)
			{
				hakkenNaiyo = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(FixedCdYoyaku.HakkenNaiyo.zankinHakken));

				// 座席状態取得
				int zasekiState = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(zasekiRows[0]["ZASEKI_STATE"]));
				if (zasekiState >= 600 && zasekiState <= 699)
				{

				}
				else
				{
					// 発券済になっていない場合

					yoyakuRows[0]["ERROR_CD"] = "H";
					zasekiRows[0]["ERROR_CD"] = "H";
				}

				return;
			}
		}

		if (yoyakuRows.Length > 0 && zasekiRows.Length > 0)
		{

			string zasekiReserveYoyakuFlg = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(yoyakuRows[0]["ZASEKI_RESERVE_YOYAKU_FLG"]));
			if (zasekiReserveYoyakuFlg == CommonRegistYoyaku.FlagValueTrue)
			{

				// 座席状態取得
				int zasekiState = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(zasekiRows[0]["ZASEKI_STATE"]));

				if (zasekiState >= 500 && zasekiState <= 599)
				{

				}
				else
				{
					// 指定席になっていない場合

					yoyakuRows[0]["ERROR_CD"] = "R";
					zasekiRows[0]["ERROR_CD"] = "R";
				}

				return;
			}
		}
	}

	/// <summary>
	/// 予約合計人数取得
	/// </summary>
	/// <param name="yoyakuList">予約情報一覧</param>
	/// <returns>予約合計人数</returns>
	private int getTotalYoyakuNinzu(DataTable yoyakuList)
	{

		int totalYoyakuNinzu = 0;

		foreach (DataRow row in yoyakuList.Rows)
		{

			string cancelFlg = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["CANCEL_FLG"]));
			if (cancelFlg != CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.YoyakuCancelFlg.torikesi) &&)
			{
				cancelFlg(!= CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.YoyakuCancelFlg.sakujo));
				// キャンセルの場合、人数を加算しない

				string state = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["STATE"]));
				if (state != CommonRegistYoyaku.convertEnumToString(FixedCdYoyaku.State.ContactHakken))
				{
					// ｼｰﾗｲﾝ 連絡発券も加算しない

					int ninzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(row["CHARGE_APPLICATION_NINZU"]));

					totalYoyakuNinzu += ninzu;
				}
			}
		}

		return totalYoyakuNinzu;
	}

	/// <summary>
	/// 合計座席数取得
	/// </summary>
	/// <param name="zasekiImageList">座席情報一覧</param>
	/// <returns>合計座席数</returns>
	private int getTotalZasekiNum(DataTable zasekiImageList)
	{

		int totalZasekiNum = 0;

		foreach (DataRow row in zasekiImageList.Rows)
		{

			int ninzu = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(row["NINZU"]));

			totalZasekiNum += ninzu;
		}

		return totalZasekiNum;
	}

	/// <summary>
	/// 出発日入力チェック
	/// </summary>
	/// <returns></returns>
	private bool isSyuptDay()
	{

		if (txtSyuptDay.Text.Equals(SyuptDayInitialValue))
		{
			//メッセージ出力（出発日が入力されていません）
			CommonProcess.createFactoryMsg().messageDisp("E90_023", "出発日");
			txtSyuptDay.ExistError = true;
			this.ActiveControl = this.txtSyuptDay;
			return false;
		}

		return true;
	}

	/// <summary>
	/// コースコード入力チェック
	/// </summary>
	/// <returns></returns>
	private bool isCrsCd()
	{

		if (string.IsNullOrEmpty(System.Convert.ToString(ucoCrsCd.CodeText)))
		{
			//メッセージ出力（コースコードが入力されていません）
			CommonProcess.createFactoryMsg().messageDisp("E90_023", "コースコード");
			ucoCrsCd.ExistError = true;
			this.ActiveControl = this.ucoCrsCd;
			return false;
		}

		return true;
	}

	/// <summary>
	/// 号車入力チェック
	/// </summary>
	/// <returns></returns>
	private bool isGousya()
	{
		if (string.IsNullOrEmpty(System.Convert.ToString(txtGousya.Text)))
		{

			CommonProcess.createFactoryMsg().messageDisp("E90_023", "号車");
			txtGousya.ExistError = true;
			this.ActiveControl = this.txtGousya;
			return false;
		}

		return true;
	}

	/// <summary>
	/// データ存在チェック
	/// </summary>
	/// <param name="gousyaInfo"></param>
	/// <returns></returns>
	private bool isDataTable(DataTable gousyaInfo)
	{

		if (gousyaInfo.Rows.Count <= 0)
		{
			//メッセージ出力（該当するデータがありません。）
			CommonProcess.createFactoryMsg().messageDisp("E90_019");
			return false;
		}

		string maruZoManagementKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(gousyaInfo.Rows(0)["MARU_ZOU_MANAGEMENT_KBN"]));
		string unkyuKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(gousyaInfo.Rows(0)["UNKYU_KBN"]));

		if (maruZoManagementKbn == FixedCd.MaruzouKanriKbn.Maruzou || unkyuKbn == FixedCd.UnkyuKbn.Haishi)
		{
			// ○増または、運休廃止の場合、処理終了
			//メッセージ出力（該当するデータがありません。）
			CommonProcess.createFactoryMsg().messageDisp("E90_019");
			return false;
		}

		return true;
	}

	/// <summary>
	/// 入力された値をセット
	/// </summary>
	private void setInputValue()
	{
		this._syuptDay = System.Convert.ToDateTime(txtSyuptDay.DisplayText);
		this._gousya = System.Convert.ToInt32(txtGousya.Text);
		this._crsCd = System.Convert.ToString(ucoCrsCd.CodeText);
	}

	/// <summary>
	/// 号車情報取得
	/// </summary>
	/// <returns>号車情報</returns>
	private DataTable getGousyaInfo()
	{

		CrsLedgerBasicEntity entity = new CrsLedgerBasicEntity();
		entity.crsCd.Value = this._crsCd;
		entity.syuptDay.Value = int.Parse(this._syuptDay.ToString("yyyyMMdd"));
		entity.gousya.Value = this._gousya;

		S02_2302Da s02_2302Da = new S02_2302Da();
		DataTable gousyaInfo = s02_2302Da.getGousyaInfo(entity);

		return gousyaInfo;
	}

	/// <summary>
	/// 号車情報セット
	/// </summary>
	private void setGousyaInfo(DataTable gousyaInfo)
	{

		DataRow dtRow = gousyaInfo.Rows(0);

		// 車種
		this.txtCarType.Text = CommonRegistYoyaku.convertObjectToString(dtRow["CAR_TYPE_CD"]);
		// 車番
		this.txtCarNo.Text = CommonRegistYoyaku.convertObjectToString(dtRow["CAR_NO"].ToString());
		// 定席数
		this.txtTeiseki.Text = string.Format(ZasekiNumFormat,;
		CommonRegistYoyaku.convertObjectToString(dtRow["CAPACITY_REGULAR"]),;
		CommonRegistYoyaku.convertObjectToString(dtRow["CAPACITY_HO_1KAI"]));
		// 空席数
		this.txtKuseki.Text = string.Format(ZasekiNumFormat,;
		CommonRegistYoyaku.convertObjectToString(dtRow["KUSEKI_NUM_TEISEKI"]),;
		CommonRegistYoyaku.convertObjectToString(dtRow["KUSEKI_NUM_SUB_SEAT"]));
		// 営業所ブロック数
		this.txtEigyosyoBlock.Text = string.Format(ZasekiNumFormat,;
		CommonRegistYoyaku.convertObjectToString(dtRow["EI_BLOCK_REGULAR"]),;
		CommonRegistYoyaku.convertObjectToString(dtRow["EI_BLOCK_HO"]));
	}

	/// <summary>
	/// 使用中フラグ解除
	/// </summary>
	/// <param name="usingFlg">使用中フラグ</param>
	/// <returns>更新結果</returns>
	private bool executeUsingFlag(string usingFlg)
	{

		string busReserveCd = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(this._gousyaInfo.Rows(0)["BUS_RESERVE_CD"]));
		int syuptDay = int.Parse(this._syuptDay.ToString("yyyyMMdd"));

		//値をエンティティに格納
		CrsLedgerBasicEntity crsLedgerEntity = new CrsLedgerBasicEntity();
		crsLedgerEntity.busReserveCd.Value = busReserveCd;
		crsLedgerEntity.syuptDay.Value = syuptDay;
		crsLedgerEntity.gousya.Value = this._gousya;
		crsLedgerEntity.usingFlg.Value = usingFlg;

		List yoyakuInfoList = new List();

		List[] crsCdList = this.getCrsCdList();
		foreach (string crsCd in crsCdList)
		{

			YoyakuInfoBasicEntity yoyakuInfoEntity = new YoyakuInfoBasicEntity();
			yoyakuInfoEntity.crsCd.Value = crsCd;
			yoyakuInfoEntity.syuptDay.Value = syuptDay;
			yoyakuInfoEntity.gousya.Value = this._gousya;
			yoyakuInfoEntity.usingFlg.Value = usingFlg;

			yoyakuInfoList.Add(yoyakuInfoEntity);
		}

		ZasekiImageEntity zasekiImageEntity = new ZasekiImageEntity();
		zasekiImageEntity.busReserveCd.Value = busReserveCd;
		zasekiImageEntity.gousya.Value = this._gousya;
		zasekiImageEntity.syuptDay.Value = syuptDay;
		zasekiImageEntity.usingFlg.Value = usingFlg;

		//更新処理
		S02_2302Da s02_2302Da = new S02_2302Da();
		if (s02_2302Da.updateUsingFlg(crsLedgerEntity, yoyakuInfoList, zasekiImageEntity) == true)
		{

			return true;
		}
		else
		{
			//メッセージ出力（更新に失敗しました。）
			CommonProcess.createFactoryMsg().messageDisp("E90_025", "更新");
			return false;
		}
	}

	/// <summary>
	/// コースコード一覧取得
	/// </summary>
	/// <returns>コースコード一覧</returns>
	private List[] getCrsCdList()
	{

		List list = new List();

		DataView dv = new DataView(this._yoyakuList);

		DataTable dtDis = dv.ToTable(true, "CRS_CD");

		foreach (DataRow row in dtDis.Rows)
		{

			string crsCd = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(row["CRS_CD"]));
			list.Add(crsCd);
		}

		return list;
	}

	/// <summary>
	/// データの再取得
	/// </summary>
	private void setDataAgainGet()
	{

		//テーブル取得処理
		this._gousyaInfo = this.getGousyaInfo();

		//データ存在チェック
		if (this.isDataTable(this._gousyaInfo) == false)
		{
			return;
		}

		//号車情報セット
		this.setGousyaInfo(this._gousyaInfo);

		// バス指定コード
		string busReserveCd = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(this._gousyaInfo.Rows(0)["BUS_RESERVE_CD"]));

		//検索結果グリッドの設定
		this.setGrdSearchResult(busReserveCd);

		// 合計予約人数設定
		this.txtTotalNinzu.Text = this.getTotalYoyakuNinzu(this._yoyakuList).ToString("#,##0");
		// 合計座席数設定
		this.txtTotalZasekiNum.Text = this.getTotalZasekiNum(this._zasekiImageList).ToString("#,##0");

		// 登録ボタン活性化
		base.F10Key_Enabled = true;

		//フォーカス設定
		this.ActiveControl = this.btnSearch;
	}

	#endregion

}