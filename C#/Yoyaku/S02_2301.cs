/// <summary>
/// S02_2301 予約使用中表示・解除
/// </summary>
public class S02_2301 : FormBase
{

	#region 定数/変数

	#region 定数
	/// <summary>
	/// 画面ID
	/// </summary>
	private const string PgmId = "S02_2301";
	/// <summary>
	/// 画面名
	/// </summary>
	private const string ScreenName = "予約使用中表示・解除";
	/// <summary>
	/// 1文字目
	/// </summary>
	private const int FirstLetter = 0;
	/// <summary>
	/// 2文字目
	/// </summary>
	private const int SecondCharacter = 1;
	/// <summary>
	/// 日付の文字数
	/// </summary>
	private const int DateLength = 8;
	/// <summary>
	/// 時刻の文字数
	/// </summary>
	private const int TimeLength = 6;
	#endregion

	#region 変数
	/// <summary>
	/// 検索後に格納する予約番号
	/// </summary>
	private string _yoyakuNo = "";
	/// <summary>
	/// 変更有無判定
	/// </summary>
	private bool _isChange = false;
	#endregion

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
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{

		//差分を検出した場合、入力内容破棄確認メッセージを表示
		if (_isChange == true)
		{
			if (CommonProcess.createFactoryMsg().messageDisp("W90_003", "入力内容").ToString().Equals("Cancel"))
			{
				return;
			}
		}

		this.Close();
	}

	/// <summary>
	/// F8：検索ボタン押下イベント
	/// </summary>
	protected override void btnF8_ClickOrgProc()
	{

		//エラークリア
		clearError();

		DataTable yoyakuInfoBasic = null;

		//入力チェック
		if (this.isYoyakuNo() == false)
		{
			return;
		}

		Hashtable yoyakuKbnNo = new Hashtable();

		//予約番号取得処理
		yoyakuKbnNo = this.getYoyakuNo(ref yoyakuKbnNo);

		//予約区分と予約Noの取り出し
		string yoyakuKbn = "";
		int yoyakuNo = 0;

		yoyakuKbn = yoyakuKbnNo["yoyakuKbn"].ToString();
		yoyakuNo = System.Convert.ToInt32(yoyakuKbnNo["yoyakuNo"]);

		//データ取得
		yoyakuInfoBasic = this.getYoyakuInfoBasic(yoyakuKbn, yoyakuNo);

		//データ存在チェック
		if (isDataTable(yoyakuInfoBasic) == false)
		{
			return;
		}

		//使用中フラグの設定
		this.setUsingFlag(yoyakuInfoBasic);

		//予約情報の設定
		this.setYoyakuInfo(yoyakuInfoBasic);

		//コース情報の設定
		this.setCrsInfo(yoyakuInfoBasic);

		//更新者情報の設定
		this.setUpdatePersonInfo(yoyakuInfoBasic);

		//フォーカス設定
		this.ActiveControl = this.btnSearch;

		//予約番号の格納
		_yoyakuNo = System.Convert.ToString(this.txtYoyakuNo.YoyakuText);

	}

	/// <summary>
	/// 条件クリアボタン押下時
	/// </summary>
	protected override void btnCLEAR_ClickOrgProc()
	{

		// 初期表示と同一の処理を実行
		setControlInitiarize();

	}

	/// <summary>
	/// F10：登録ボタン押下イベント
	/// </summary>
	protected override void btnF10_ClickOrgProc()
	{

		//予約番号が空の場合、登録処理は行わない。
		if (string.IsNullOrEmpty(_yoyakuNo))
		{
			return;
		}

		//チェックボックスがTrueの場合、チェックボックスをFalseに設定
		if (chkUsingFlg.Checked == true)
		{
			chkUsingFlg.Checked = false;
		}

		//メッセージ出力（使用中フラグの解除処理を行います。よろしいですか？）
		if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", "使用中フラグの解除").ToString().Equals("Cancel"))
		{
			//キャンセルの場合、処理を抜ける
			return;
		}

		//データ登録処理の実施
		this.executeUsingFlag();

		// log出力
		createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, ScreenName, "登録処理");

		//データの再取得
		this.setDataAgainGet();

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
	protected void btnCLEAR_Click(object sender, EventArgs e)
	{

		// CLEARボタン押下
		base.btnCom_Click(this.btnClear, e);

	}

	/// <summary>
	/// フラグ変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void chkUsingFlg_CheckedChanged(object sender, EventArgs e)
	{

		//変更有無の判定用フラグの更新
		_isChange = true;
	}

	/// <summary>
	/// F8キーダウンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void S02_2301_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.F8)
		{
			e.Handled = true;
			this.btnSearch_Click(sender, e);
		}
	}

	#endregion

	#region メソッド
	/// <summary>
	/// 画面初期化
	/// </summary>
	private void setControlInitiarize()
	{

		//ベースフォームの設定
		this.setFormId = "S02_2301";
		this.setTitle = "予約使用中表示・解除";

		// 検索条件部の項目初期化
		this.setSearchJokenBu();

		// フッタボタンの設定
		this.setButtonInitiarize();

		//予約情報の初期表示
		this.setInitiarizeYoyakuInfo();

		//コース情報の初期表示
		this.setInitiarizeCrsInfo();

		//使用中フラグの初期表示
		this.setInitiarizeUsingFlag();

		//更新者情報の初期表示
		this.setInitiarizeUpdatePersonInfo();

		//更新エリアの初期化
		this._yoyakuNo = "";

		//エラークリア
		clearError();

	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{

		//Visible
		this.F1Key_Visible = false;
		this.F2Key_Visible = true;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		this.F7Key_Visible = false;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = true;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;

		//Text
		this.F2Key_Text = "F2:戻る";
		this.F10Key_Text = "F10:登録";

	}

	/// <summary>
	/// 料金区分グリッドの設定
	/// </summary>
	private void setGrdChargeKbn(DataTable yoyakuInfoBasic)
	{

		DataTable dt = new DataTable();

		//列作成
		dt.Columns.Add("colChargeKbnJinin"); //料金区分
		dt.Columns.Add("colYoyakuNinzu"); //予約人数

		for (rowNum = 0; rowNum <= yoyakuInfoBasic.Rows.Count - 1; rowNum++)
		{

			object dtRow = dt.NewRow;

			dtRow["colChargeKbnJinin"] = yoyakuInfoBasic.Rows(rowNum)["CHARGE_KBN_JININ_NAME"];
			dtRow["colYoyakuNinzu"] = yoyakuInfoBasic.Rows(rowNum)["CHARGE_APPLICATION_NINZU"];

			dt.Rows.Add(dtRow);
		}

		//グリッドにテーブルをセット
		this.grdChargeKbn.DataSource = dt;

	}

	/// <summary>
	/// 予約No入力チェック
	/// </summary>
	/// <returns></returns>
	private bool isYoyakuNo()
	{
		if (string.IsNullOrEmpty(System.Convert.ToString(this.txtYoyakuNo.YoyakuText)))
		{


			//メッセージ出力処理(予約Noが入力されていません。)
			CommonProcess.createFactoryMsg().messageDisp("E90_023", "予約No");

			txtYoyakuNo.ExistError = true;
			//フォーカス設定
			this.ActiveControl = this.txtYoyakuNo;

			return false;
		}
		else if (this.txtYoyakuNo.YoyakuText.Length <= 1)
		{

			//メッセージ出力処理(予約Noが不正です。)
			CommonProcess.createFactoryMsg().messageDisp("E90_016", "予約No");

			txtYoyakuNo.ExistError = true;
			//フォーカス設定
			this.ActiveControl = this.txtYoyakuNo;

			return false;
		}

		return true;
	}

	/// <summary>
	/// エラークリア
	/// </summary>
	private void clearError()
	{

		txtYoyakuNo.ExistError = false;

	}

	/// <summary>
	/// 予約情報基本テーブル取得
	/// </summary>
	/// <param name="yoykuKbn"></param>
	/// <param name="yoyakuNo"></param>
	/// <returns></returns>
	private object getYoyakuInfoBasic()
	{
		ByValyoyakuNoint)DataTable;
		S02_2301Da S02_2301Da = new S02_2301Da();

		return S02_2301Da.getYoyakuInfoBasic(yoykuKbn, yoyakuNo);

	}

	/// <summary>
	/// データ存在チェック
	/// </summary>
	/// <param name="yoyakuInfoBasic "></param>
	/// <returns></returns>
	private bool isDataTable(DataTable yoyakuInfoBasic)
	{
		if (yoyakuInfoBasic.Rows.Count <= 0)
		{
			//メッセージ出力（該当するデータがありません。）
			CommonProcess.createFactoryMsg().messageDisp("E90_019", "");

			return false;
		}

		return true;
	}

	/// <summary>
	/// 使用中フラグの設定
	/// </summary>
	/// <param name="yoyakuInfoBasic"></param>
	private void setUsingFlag(DataTable yoyakuInfoBasic)
	{

		string usingFlag = "";

		usingFlag = yoyakuInfoBasic.Rows(0)["USING_FLG"].ToString();

		//使用中の場合、使用中フラグを表示
		if (usingFlag.Equals(FixedCd.UsingFlg.Use))
		{
			this.chkUsingFlg.Checked = true;
		}
		else
		{
			this.chkUsingFlg.Checked = false;
		}

	}

	/// <summary>
	/// 予約情報の設定
	/// </summary>
	/// <param name="yoyakuInfoBasic "></param>
	private void setYoyakuInfo(DataTable yoyakuInfoBasic)
	{

		//名前をセット
		this.txtName.Text = yoyakuInfoBasic.Rows(0)["SURNAME"].ToString() + Strings.Space(1) + yoyakuInfoBasic.Rows(0)["NAME"].ToString();

		//料金区分グリッドの設定
		this.setGrdChargeKbn(yoyakuInfoBasic);

	}

	/// <summary>
	/// コース情報の設定
	/// </summary>
	/// <param name="yoyakuInfoBasic "></param>
	private void setCrsInfo(DataTable yoyakuInfoBasic)
	{

		string syuptDay = "";

		//出発日の値が不正な場合は、空文字を代入
		//日付フォーマットチェック
		if (CommonDateUtil.isDateYYYYMMDD(yoyakuInfoBasic.Rows(0)["SYUPT_DAY"].ToString()) == false)
		{

			syuptDay = "";
		}
		else
		{
			syuptDay = yoyakuInfoBasic.Rows(0)["SYUPT_DAY"].ToString().Substring(0, 4) + "/";
			syuptDay += yoyakuInfoBasic.Rows(0)["SYUPT_DAY"].ToString().Substring(4, 2) + "/";
			syuptDay += System.Convert.ToString(yoyakuInfoBasic.Rows(0)["SYUPT_DAY"].ToString().Substring(6, 2));
		}

		//テキストボックスに値をセット
		this.txtCrsCd.Text = yoyakuInfoBasic.Rows(0)["CRS_CD"].ToString();
		this.txtSyuptDay.Text = syuptDay;
		this.txtGousya.Text = yoyakuInfoBasic.Rows(0)["GOUSYA"].ToString();

	}

	/// <summary>
	/// 更新者情報の設定
	/// </summary>
	/// <param name="yoyakuInfoBasic"></param>
	private void setUpdatePersonInfo(DataTable yoyakuInfoBasic)
	{

		string updateDate = "";

		//「yyyyMMddHHmm」を「yyyy/MM/dd/HH:mm」に変換
		//更新日の値が不正な場合は、空文字を代入
		if (CommonDateUtil.isDateYYYYMMDD(yoyakuInfoBasic.Rows(0)["UPDATE_DAY"].ToString()) == false)
		{

			updateDate = "";
		}
		else
		{
			updateDate = yoyakuInfoBasic.Rows(0)["UPDATE_DAY"].ToString().Substring(0, 4) + "/";
			updateDate += yoyakuInfoBasic.Rows(0)["UPDATE_DAY"].ToString().Substring(4, 2) + "/";
			updateDate += yoyakuInfoBasic.Rows(0)["UPDATE_DAY"].ToString().Substring(6, 2) + "/";
		}

		//更新時刻の値が不正な場合は、空文字を代入
		if (string.IsNullOrEmpty(yoyakuInfoBasic.Rows(0)["UPDATE_TIME"].ToString()) ||)
		{
			yoyakuInfoBasic.Rows(0)["UPDATE_TIME"].ToString().Trim().Length != TimeLength;


		}
		else
		{
			updateDate += yoyakuInfoBasic.Rows(0)["UPDATE_TIME"].ToString().Substring(0, 2) + ":";
			updateDate += System.Convert.ToString(yoyakuInfoBasic.Rows(0)["UPDATE_TIME"].ToString().Substring(2, 2));
		}

		//テキストボックスに値をセット
		this.txtId.Text = yoyakuInfoBasic.Rows(0)["UPDATE_PGMID"].ToString();
		this.txtUpdateDate.Text = updateDate;
		this.txtUpdatePersonId.Text = yoyakuInfoBasic.Rows(0)["UPDATE_PERSON_CD"].ToString();
		this.txtUpdatePersonName.Text = yoyakuInfoBasic.Rows(0)["USER_NAME"].ToString();
	}

	/// <summary>
	/// 検索条件部の初期表示
	/// </summary>
	private void setSearchJokenBu()
	{

		this.txtYoyakuNo.YoyakuText = "";
	}

	/// <summary>
	/// 使用中フラグの初期表示
	/// </summary>
	private void setInitiarizeUsingFlag()
	{

		this.chkUsingFlg.Checked = false;
	}

	/// <summary>
	/// 予約情報の初期表示
	/// </summary>
	private void setInitiarizeYoyakuInfo()
	{

		this.txtName.Text = "";

		//グリッド部の初期表示
		//(Nothingで初期化するとヘッダー名が消える)
		DataTable dt = new DataTable();
		this.grdChargeKbn.DataSource = dt;
		this.grdChargeKbn.DataMember = "";
		this.grdChargeKbn.Refresh();

	}

	/// <summary>
	/// コース情報の初期表示
	/// </summary>
	private void setInitiarizeCrsInfo()
	{

		this.txtCrsCd.Text = "";
		this.txtSyuptDay.Text = "";
		this.txtGousya.Text = "";

	}

	/// <summary>
	/// 更新者情報の初期表示
	/// </summary>
	private void setInitiarizeUpdatePersonInfo()
	{

		this.txtId.Text = "";
		this.txtUpdateDate.Text = "";
		this.txtUpdatePersonId.Text = "";
		this.txtUpdatePersonName.Text = "";

	}

	/// <summary>
	/// 使用中フラグの更新
	/// </summary>
	private void executeUsingFlag()
	{

		S02_2301Da s02_2301Da = new S02_2301Da();
		YoyakuInfoBasicEntity yoyakuEntity = new YoyakuInfoBasicEntity(); //予約情報（基本）エンティティ
		string personCd = System.Convert.ToString(UserInfoManagement.userId);

		//予約Noから予約区分と予約番号の抽出
		yoyakuEntity.yoyakuKbn.Value = this._yoyakuNo[FirstLetter];
		yoyakuEntity.yoyakuNo.Value = int.Parse(this._yoyakuNo.Substring(SecondCharacter));

		//エンティティに格納
		yoyakuEntity.usingFlg.Value = FixedCd.UsingFlg.Unused;
		yoyakuEntity.updatePersonCd.Value = personCd;
		yoyakuEntity.updatePgmid.Value = PgmId;
		yoyakuEntity.systemUpdatePersonCd.Value = personCd;
		yoyakuEntity.systemUpdatePgmid.Value = PgmId;

		//更新処理
		if (s02_2301Da.updateUsingFlg(yoyakuEntity) == true)
		{
			//メッセージ出力（更新が完了しました。）
			CommonProcess.createFactoryMsg().messageDisp("I90_002", "更新");
		}
		else
		{
			//メッセージ出力（更新に失敗しました。）
			CommonProcess.createFactoryMsg().messageDisp("E90_025", "更新");
		}

	}

	/// <summary>
	/// データの再取得
	/// </summary>
	private void setDataAgainGet()
	{

		DataTable yoyakuInfoBasic = new DataTable();
		string yoyakuKbn = "";
		int yoyakuNo = 0;
		Hashtable yoyakuKbnNo = new Hashtable();

		//使用中フラグの初期表示
		this.setInitiarizeUsingFlag();

		//予約情報の初期表示
		this.setInitiarizeYoyakuInfo();

		//コース情報の初期表示
		this.setInitiarizeCrsInfo();

		//更新者情報の初期表示
		this.setInitiarizeUpdatePersonInfo();

		//予約番号取得処理
		yoyakuKbnNo = this.getYoyakuNo(ref yoyakuKbnNo);

		//予約区分と予約Noの取り出し
		yoyakuKbn = yoyakuKbnNo["yoyakuKbn"].ToString();
		yoyakuNo = System.Convert.ToInt32(yoyakuKbnNo["yoyakuNo"]);

		//'予約Noから予約区分と予約番号の抽出
		//yoyakuKbn = Me._yoyakuNo(FirstLetter)
		//yoyakuNo = CInt(Me._yoyakuNo.Substring(SecondCharacter))

		//テーブル取得処理
		yoyakuInfoBasic = this.getYoyakuInfoBasic(yoyakuKbn, yoyakuNo);

		//データ存在チェック
		if (isDataTable(yoyakuInfoBasic) == false)
		{
			_yoyakuNo = "";
			return;
		}

		//使用中フラグの設定
		this.setUsingFlag(yoyakuInfoBasic);

		//予約情報の設定
		this.setYoyakuInfo(yoyakuInfoBasic);

		//コース情報の設定
		this.setCrsInfo(yoyakuInfoBasic);

		//更新者情報の設定
		this.setUpdatePersonInfo(yoyakuInfoBasic);

	}

	/// <summary>
	/// 予約番号取得処理
	/// </summary>
	/// <param name="yoyakuNo"></param>
	/// <returns></returns>
	private Hashtable getYoyakuNo(ref Hashtable yoyakuNo)
	{

		//予約区分+NO
		string yoyakuKbnNo = System.Convert.ToString(this.txtYoyakuNo.YoyakuText);

		if (yoyakuKbnNo.Length == 10)
		{
			yoyakuNo["yoyakuKbn"] = yoyakuKbnNo.Substring(0, 1);
			yoyakuNo["yoyakuNo"] = yoyakuKbnNo.Substring(1, yoyakuKbnNo.Length - 1);
		}
		else if (yoyakuKbnNo.Length < 10)
		{
			yoyakuNo["yoyakuKbn"] = "0";
			yoyakuNo["yoyakuNo"] = yoyakuKbnNo;
		}

		return yoyakuNo;
	}


	#endregion

}