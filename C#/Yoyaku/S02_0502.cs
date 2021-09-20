/// <summary>
/// S02_0502 リマークス登録
/// <remarks>
/// Author:2018/10/31//QuangTD
/// </remarks>
/// </summary>
public class S02_0502 : FormBase
{

	Delivery_S02_0502Entity receiptEntity = new Delivery_S02_0502Entity();

	private EntityOperation[] updateEntity;

	public bool researchFlg;

	public enum TypeEdit : int
	{
		Register = 1,
		Update = 2
	}

	TypeEdit editMode = TypeEdit.Register;

	/// <summary>
	/// 削除日デフォルト（非削除時）
	/// </summary>
	private const int NotDeleteDayValue = 0;

	/// <summary>
	/// Le X.Diem Add
	/// Contructor
	/// </summary>
	/// <param name="receipt"></param>
	public S02_0502(Delivery_S02_0502Entity receipt)
	{
		updateEntity = new EntityOperation(Of TCrsLedgerRemarksEntityEx);

		InitializeComponent();
		//【各画面毎】遷移元画面からの画面間パラメータを格納する
		receiptEntity.CourseCode = receipt.CourseCode;
		receiptEntity.DepartureDate = receipt.DepartureDate;
		receiptEntity.DepartureTime = receipt.DepartureTime;
		receiptEntity.Car = receipt.Car;
		receiptEntity.BranchMumb = receipt.BranchMumb;
		receiptEntity.EditMode = receipt.EditMode;
	}


	/// <summary>
	/// 画面起動時の独自処理
	/// </summary>
	protected override void StartupOrgProc()
	{
		string[] logmsg = new string[2];

		// 差分チェック初期状態セット
		base.closeFormFlg = false;

		try
		{
			//遷移元画面からの画面間パラメータを格納する
			initializeInfomation();
			//画面表示時の初期設定
			setControlInitiarize();
			//表示データ取得
			setDataInfomation();
			if (editMode == TypeEdit.Update)
			{
				logmsg[0] = System.Convert.ToString(updateEntity.EntityData(0).crsCd.ToString());
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, setTitle, logmsg);
			}
		}
		catch (Exception ex)
		{
			if (editMode == TypeEdit.Update)
			{
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
			}
			throw (ex);
		}
	}

	/// <summary>
	/// 遷移元画面からの画面間パラメータを格納する
	/// </summary>
	private void initializeInfomation()
	{
		editMode = (TypeEdit)((TypeEdit)receiptEntity.EditMode);
		Code_DA codeDA = new Code_DA();
		Hashtable paramList = new Hashtable();
		paramList.Add("codeBunrui", CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memokbn));
		paramList.Add("codeValue", NoteClassification.Shared_Items);
		object data = codeDA.accessCodeMaster(Code_DA.accessType.getCodeDataByPrimaryKey, paramList);
		if (data IsNot null && data.Rows.Count > 0)
		{
			rdoShareJiko.Text = if (!Information.IsDBNull(data.Rows(0)["CODE_NAME"]), data.Rows(0)["CODE_NAME"].ToString(), string.Empty);
		}
		else
		{
			rdoShareJiko.Text = string.Empty;
		}
		Code_DA codeDA1 = new Code_DA();
		paramList.Clear();
		paramList.Add("codeBunrui", CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui));
		paramList.Add("codeValue", MemoClassification.memo);
		data = codeDA1.accessCodeMaster(Code_DA.accessType.getCodeDataByPrimaryKey, paramList);
		if (data IsNot null && data.Rows.Count > 0)
		{
			rdoMemo.Text = if (!Information.IsDBNull(data.Rows(0)["CODE_NAME"]), data.Rows(0)["CODE_NAME"].ToString(), string.Empty);
		}
		else
		{
			rdoMemo.Text = string.Empty;
		}
		Code_DA codeDA2 = new Code_DA();
		paramList.Clear();
		paramList.Add("codeBunrui", CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui));
		paramList.Add("codeValue", MemoClassification.disembarkation_Place_Report);
		data = codeDA2.accessCodeMaster(Code_DA.accessType.getCodeDataByPrimaryKey, paramList);
		if (data IsNot null && data.Rows.Count > 0)
		{
			rdoKousyaKashoContact.Text = if (!Information.IsDBNull(data.Rows(0)["CODE_NAME"]), data.Rows(0)["CODE_NAME"].ToString(), string.Empty);
		}
		else
		{
			rdoKousyaKashoContact.Text = string.Empty;
		}
		Code_DA codeDA3 = new Code_DA();
		paramList.Clear();
		paramList.Add("codeBunrui", CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui));
		paramList.Add("codeValue", MemoClassification.staff_Sharing);
		data = codeDA3.accessCodeMaster(Code_DA.accessType.getCodeDataByPrimaryKey, paramList);
		if (data IsNot null && data.Rows.Count > 0)
		{
			rdoStaffShare.Text = if (!Information.IsDBNull(data.Rows(0)["CODE_NAME"]), data.Rows(0)["CODE_NAME"].ToString(), string.Empty);
		}
		else
		{
			rdoStaffShare.Text = string.Empty;
		}
	}

	/// <summary>
	/// 表示データ取得
	/// </summary>
	private void setDataInfomation()
	{
		object with_1 = updateEntity.EntityData(0);
		with_1.crsCd.Value = receiptEntity.CourseCode;
		if (Information.IsNumeric(receiptEntity.DepartureDate))
		{
			with_1.syuptDay.Value = (Integer?)receiptEntity.DepartureDate;
		}
		if (Information.IsNumeric(receiptEntity.Car))
		{
			with_1.gousya.Value = (Integer?)receiptEntity.Car;
		}
		if (Information.IsNumeric(receiptEntity.BranchMumb))
		{
			with_1.edaban.Value = (Integer?)receiptEntity.BranchMumb;
		}
		if (editMode == TypeEdit.Update)
		{
			//画面間パラメータの編集モードが「更新（削除）」の場合、画面間パラメータの値を使用して表示内容を取得する
			object data = getCrsLedgerRemarkDataTable();
			if (data IsNot null && data.Rows.Count == 1)
			{
				if (data.Rows(0)["MEMO_BUNRUI"].ToString() == MemoClassification.disembarkation_Place_Report ||)
				{
					data.Rows(0)["MEMO_BUNRUI"].ToString = System.Convert.ToString(MemoClassification.staff_Sharing);
					// 分類＝降車ヶ所orスタッフ共有の場合、明細の情報を取得
					grdTaisyo.DataSource = setDataToTable(data);
				}
				else
				{
					// 上記以外は明細無し
					grdTaisyo.DataSource = new DataTable();
				}
				txtNaiyo.Text = data.Rows(0)["NAIYO"].ToString();
				string memoType = System.Convert.ToString(data.Rows(0)["MEMO_BUNRUI"]);
				CheckMemoBunrui(memoType.ToString().Trim(), true);
			}
			else
			{
				createFactoryMsg().messageDisp("E90_019");
				SetUpBottomButton(false);
			}
		}
		else
		{
			CheckMemoBunrui(System.Convert.ToString(MemoClassification.memo), true);
		}
		setResponseValue(false);
		//差分チェック用情報の格納
		setZenkaiValue();
	}

	/// <summary>
	/// ラジオボタンを設定する
	/// </summary>
	/// <param name="memoBunrui"></param>
	/// <param name="performClick"></param>
	private void CheckMemoBunrui(string memoBunrui, bool performClick)
	{
		if (memoBunrui == MemoClassification.disembarkation_Place_Report)
		{
			rdoKousyaKashoContact.Checked = false;
			rdoKousyaKashoContact.Checked = true;
			if (performClick)
			{
				rdoKousyaKashoContact.PerformClick();
			}
		}
		else if (memoBunrui == MemoClassification.staff_Sharing)
		{
			rdoStaffShare.Checked = false;
			rdoStaffShare.Checked = true;
			if (performClick)
			{
				rdoStaffShare.PerformClick();
			}
		}
		else
		{
			rdoMemo.Checked = false;
			rdoMemo.Checked = true;
			if (performClick)
			{
				rdoMemo.PerformClick();
			}
		}
	}

	/// <summary>
	/// 差分チェック用情報の格納
	/// </summary>
	private void setZenkaiValue()
	{
		object with_1 = updateEntity.EntityData(0);
		with_1.crsCd.ZenkaiValue = receiptEntity.CourseCode;
		if (Information.IsNumeric(receiptEntity.DepartureDate))
		{
			with_1.syuptDay.ZenkaiValue = (Integer?)receiptEntity.DepartureDate;
		}
		if (Information.IsNumeric(receiptEntity.Car))
		{
			with_1.gousya.ZenkaiValue = (Integer?)receiptEntity.Car;
		}
		if (Information.IsNumeric(receiptEntity.BranchMumb))
		{
			with_1.edaban.ZenkaiValue = (Integer?)receiptEntity.BranchMumb;
		}
		with_1.naiyo.ZenkaiValue = this.txtNaiyo.Text;
		if (rdoShareJiko.Checked)
		{
			with_1.memoKbn.ZenkaiValue = NoteClassification.Shared_Items;
		}
		else
		{
			with_1.memoKbn.ZenkaiValue = string.Empty;
		}
		if (rdoMemo.Checked)
		{
			with_1.memoBunrui.ZenkaiValue = MemoClassification.memo;
			with_1.koshakashoCd.ZenkaiValue = null;
			with_1.koshakashoEdaban.ZenkaiValue = null;
			with_1.staffShareTaisyo.ZenkaiValue = null;
		}
		else
		{
			object dataSelected = getSelectedRowData();
			if (rdoKousyaKashoContact.Checked)
			{
				with_1.memoBunrui.ZenkaiValue = MemoClassification.disembarkation_Place_Report;
				if (dataSelected IsNot null)
				{
					if (!Information.IsDBNull(dataSelected["colTaisyoCode"]))
					{
						with_1.koshakashoCd.ZenkaiValue = System.Convert.ToString(dataSelected["colTaisyoCode"]);
					}
					else
					{
						with_1.koshakashoCd.ZenkaiValue = null;
					}
					if (!Information.IsDBNull(dataSelected["colKindCode"]))
					{
						with_1.koshakashoEdaban.ZenkaiValue = System.Convert.ToString(dataSelected["colKindCode"]);
					}
					else
					{
						with_1.koshakashoEdaban.ZenkaiValue = null;
					}
					with_1.staffShareTaisyo.ZenkaiValue = null;
				}
			}
			else if (rdoStaffShare.Checked)
			{
				with_1.memoBunrui.ZenkaiValue = MemoClassification.staff_Sharing;
				with_1.koshakashoCd.ZenkaiValue = null;
				with_1.koshakashoEdaban.ZenkaiValue = null;
				if (dataSelected IsNot null)
				{
					with_1.staffShareTaisyo.ZenkaiValue = System.Convert.ToString(dataSelected["colTaisyoCode"]);
				}
			}
		}
	}

	/// <summary>
	///  応答用の画面間パラメータの再検索フラグに”再検索しない”を設定する
	/// </summary>
	private void setResponseValue(bool flg)
	{
		researchFlg = flg;
	}

	#region リマークス
	/// <summary>
	/// メモラジオボタンチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void rdoMemo_CheckedChanged(object sender, EventArgs e)
	{
		if (editMode == TypeEdit.Register & getSelectedRowData() IsNot null)
		{
			if (CommonProcess.createFactoryMsg().messageDisp("W90_003", "変更") == MsgBoxResult.Cancel)
			{
				rdoMemo.Checked = false;
				CheckMemoBunrui(System.Convert.ToString(updateEntity.EntityData(0).memoBunrui.ZenkaiValue), false);
				return;
			}
		}
		if (!rdoMemo.Checked)
		{
			return;
		}
		clearTaisyoList();
		SetUpBottomButton(true);
		//各種処理情報を格納する
		setResponseValue(false);
		//差分チェック用情報の格納
		setZenkaiValue();
		//フォーカス設定
		txtNaiyo.Select();
	}

	#endregion
	/// <summary>
	/// 降車箇所連絡ラジオボタンチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void rdoKousyaKashoContact_CheckedChanged(object sender, EventArgs e)
	{
		if (editMode == TypeEdit.Register & getSelectedRowData() IsNot null)
		{
			if (CommonProcess.createFactoryMsg().messageDisp("W90_003", "変更") == MsgBoxResult.Cancel)
			{
				rdoKousyaKashoContact.Checked = false;
				CheckMemoBunrui(System.Convert.ToString(updateEntity.EntityData(0).memoBunrui.ZenkaiValue), false);
				return;
			}
		}
		//区分が「共有事項」かつ分類が「降車ヶ所」の場合
		if (!rdoKousyaKashoContact.Checked)
		{
			return;
		}
		DataTable data = null;
		if (editMode == TypeEdit.Register)
		{
			data = getCrsLedgerKoshaKashoDataTable();
		}
		else if (editMode == TypeEdit.Update)
		{
			data = getCrsLedgerRemarkDataTable();
		}
		else
		{
			data = new DataTable();
		}
		grdTaisyo.DataSource = setDataToTable(data);
		grdTaisyo.Enabled = true;
		if (data.Rows.Count == 0)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_019");
			SetUpBottomButton(false);
		}
		else
		{
			SetUpBottomButton(true);
		}
		//各種処理情報を格納する
		setResponseValue(false);
		//差分チェック用情報の格納
		setZenkaiValue();
		//フォーカス設定
		txtNaiyo.Select();
	}


	/// <summary>
	/// スタッフ共有ラジオボタンチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void rdoStaffShare_CheckedChanged(object sender, EventArgs e)
	{
		if (editMode == TypeEdit.Register & getSelectedRowData() IsNot null)
		{
			if (CommonProcess.createFactoryMsg().messageDisp("W90_003", "変更") == MsgBoxResult.Cancel)
			{
				rdoStaffShare.Checked = false;
				CheckMemoBunrui(System.Convert.ToString(updateEntity.EntityData(0).memoBunrui.ZenkaiValue), false);
				return;
			}
		}
		// 区分が「共有事項」かつ分類が「スタッフ共有」の場合
		if (!rdoStaffShare.Checked)
		{
			return;
		}
		if (editMode == TypeEdit.Register)
		{
			DataTable dt = getStaffShareDataTable();
			grdTaisyo.DataSource = setDataToTable(dt);
			grdTaisyo.Enabled = true;
			if (dt.Rows.Count == 0)
			{
				SetUpBottomButton(false);
				CommonProcess.createFactoryMsg().messageDisp("E90_019");
			}
			else
			{
				F10Key_Enabled = true;
			}
		}
		else if (editMode == TypeEdit.Update)
		{
			DataTable dt = getCrsLedgerRemarkDataTable();
			grdTaisyo.DataSource = setDataToTable(dt);
			grdTaisyo.Enabled = true;
			if (dt.Rows.Count == 0)
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_019");
				SetUpBottomButton(false);
			}
			else
			{
				F11Key_Enabled = true;
				F4Key_Enabled = true;
			}
		}
		else
		{
		}
		//各種処理情報を格納する
		setResponseValue(false);
		//差分チェック用情報の格納
		setZenkaiValue();
		//フォーカス設定
		txtNaiyo.Select();
	}

	/// <summary>
	/// 取得したデータを一覧に表示する
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	private DataTable setDataToTable(DataTable data)
	{
		DataTable dt = createTaisyoDataTable();
		foreach (DataRow rowData in data.Rows)
		{
			DataRow row = dt.NewRow();
			if (System.Convert.ToInt32(rowData["colSelection"]) == 1)
			{
				row["colSelection"] = true;
			}
			else
			{
				row["colSelection"] = false;
			}
			row["colTaisyo"] = rowData["colTaisyo"];
			row["colKind"] = rowData["colKind"];
			row["colTaisyoCode"] = rowData["colTaisyoCode"];
			row["colKindCode"] = rowData["colKindCode"];
			dt.Rows.Add(row);
		}
		return dt;
	}

	/// <summary>
	/// 表示対象のデータを取得する
	/// </summary>
	private DataTable getCrsLedgerRemarkDataTable()
	{
		CrsLedgerRemarks_DA tCrsLedgerRemarks_DA = new CrsLedgerRemarks_DA();
		DataTable data = null;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		paramInfoList.Add("CrsCd", receiptEntity.CourseCode);
		paramInfoList.Add("SyuptDay", receiptEntity.DepartureDate);
		paramInfoList.Add("Gousya", receiptEntity.Car);
		paramInfoList.Add("Edaban", receiptEntity.BranchMumb);
		data = tCrsLedgerRemarks_DA.accessCrsLedgerRemarks(CrsLedgerRemarks_DA.accessType.getCrsLedgerRemarksByPrimaryKey, paramInfoList);
		return data;
	}

	/// <summary>
	/// 表示対象のデータを取得する
	/// </summary>
	private DataTable getCrsLedgerKoshaKashoDataTable()
	{
		CrsLedgerKoshakasho_DA tCrsLedgerKoshakasho_DA = new CrsLedgerKoshakasho_DA();
		DataTable data = null;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		paramInfoList.Add("CrsCd", receiptEntity.CourseCode);
		paramInfoList.Add("SyuptDay", receiptEntity.DepartureDate);
		paramInfoList.Add("Gousya", receiptEntity.Car);
		data = tCrsLedgerKoshakasho_DA.accessCrsLedgerKoshakasho(CrsLedgerKoshakasho_DA.accessType.getCrsLedgerKoshakasho, paramInfoList);
		return data;
	}

	/// <summary>
	/// 表示対象のデータを取得する
	/// </summary>
	private DataTable getStaffShareDataTable()
	{
		Code_DA mCode_DA = new Code_DA();
		DataTable data = null;
		//DBパラメータ
		data = mCode_DA.getCodeRemarkKoshakasho();
		return data;
	}

	#region    F2
	/// <summary>
	/// F2()ボタン押下時の独自処理
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		// 戻る
		base.closeFormFlg = true;
		Close();
	}
	#endregion

	#region    Close時
	/// <summary>
	/// 画面クロージング時
	/// </summary>
	protected override bool closingScreen()
	{
		//確認必要パターン
		//Entitiyに値を格納
		setEntityDataValue();
		//差分チェック
		return checkDifference();
	}
	#endregion

	#region    F4
	/// <summary>
	/// F4ボタン押下時の独自処理
	/// </summary>
	protected override void btnF4_ClickOrgProc()
	{
		string[] logmsg = new string[2];
		logmsg[0] = System.Convert.ToString(this.txtNaiyo.Text);
		//updateEntity.EntityData(0).DeleteDate.Value = CInt(CommonProcess.getDateTime().ToString(CType(Date_FormatType.formatYYYYMMDD, String)))
		updateEntity.EntityData(0).DeleteDate.Value = System.Convert.ToInt32(CommonDateUtil.getSystemTime().ToString("yyyyMMdd"));
		if (DbOperator(DbShoriKbn.Update) > 0)
		{
			//正常終了メッセージ表示
			CommonProcess.createFactoryMsg().messageDisp("I90_002", "削除");
			//ログ出力(操作ログ)
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.delete, setTitle, logmsg);
			updateEntity.EntityData(0).DeleteDate.ZenkaiValue = updateEntity.EntityData(0).DeleteDate.Value;
			btnF2_ClickOrgProc();
		}
		else
		{
			//異常終了メッセージ表示
			CommonProcess.createFactoryMsg().messageDisp("E90_020", "削除");
			//ログ出力(エラーログ)
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
		}
		//各種処理情報を格納する
		setResponseValue(true);
	}
	#endregion

	#region    F10
	/// <summary>
	/// F10ボタン押下時の独自処理
	/// </summary>
	protected override void btnF10_ClickOrgProc()
	{
		// 新規登録時の削除日デフォルト設定
		updateEntity.EntityData(0).DeleteDate.Value = NotDeleteDayValue;
		object returnValue = DbOperator(DbShoriKbn.Insert);
		if (returnValue > 0)
		{
			setZenkaiValue();
			setResponseValue(true);
			//正常終了メッセージ表示
			createFactoryMsg().messageDisp("I90_002", "登録");
			//ログ出力(操作ログ)
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.entry, setTitle);
			// 画面を終了する
			//Call btnF2_ClickOrgProc()
			base.closeFormFlg = false;
			this.Close();
		}
		else if (returnValue == base.RET_CANCEL)
		{
			//処理確認でキャンセルの場合
			return;
		}
		else if (returnValue == base.RET_NONEXEC)
		{
			//更新対象無し以外の場合
			return;
		}
		else
		{
			//異常終了メッセージ表示
			createFactoryMsg().messageDisp("E90_020", "登録");
			//ログ出力(エラーログ)
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, setTitle);
		}
	}

	#region    F11
	/// <summary>
	/// F11()ボタン押下時の独自処理
	/// </summary>
	protected override void btnF11_ClickOrgProc()
	{
		// 更新時は削除日未設定（nothingのセットを修正しないこと。後続で使用するため）
		updateEntity.EntityData(0).DeleteDate.Value = null;
		object returnValue = DbOperator(DbShoriKbn.Update);
		if (returnValue > 0)
		{
			//データの再表示
			setResponseValue(true);
			editMode = TypeEdit.Update;
			setZenkaiValue();
			//正常終了メッセージ表示
			createFactoryMsg().messageDisp("I90_002", "更新");
			//ログ出力(操作ログ)
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.update, setTitle);
		}
		else if (returnValue == base.RET_CANCEL)
		{
			//処理確認でキャンセルの場合
			return;
		}
		else if (returnValue == base.RET_NONEXEC)
		{
			//更新対象無し以外の場合
			return;
		}
		else
		{
			//異常終了メッセージ表示
			createFactoryMsg().messageDisp("E90_020", "更新");
			//ログ出力(エラーログ)
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle);
		}
	}

	/// <summary>
	/// 必須入力項目エラーチェック
	/// (更新エリア)
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool isExistHissuError()
	{
		base.clearExistErrorProperty(pnlBaseFill.Controls);
		if (CommonUtil.checkHissuError(pnlBaseFill.Controls) == true)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			return true;
		}
		else
		{
			return false;
		}
	}

	#region 画面->エンティティ
	/// <summary>
	/// 更新対象項目をエンティティにセット
	/// </summary>
	protected override void setEntityDataValue()
	{
		object deleteDate = updateEntity.EntityData(0).deleteDate.Value;
		//エンティティの初期化
		updateEntity.clear(0, clearType.value);
		updateEntity.clear(0, clearType.errorInfo);
		updateEntity.EntityData(0).deleteDate.Value = deleteDate;
		displayDataToEntity(updateEntity.EntityData(0));
	}
	#endregion

	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">行定表連結記号エンティティ</param>
	private void displayDataToEntity(object ent)
	{
		if (typeof(TCrsLedgerRemarksEntityEx).Equals(ent.GetType()))
		{
			TCrsLedgerRemarksEntityEx with_1 = ((TCrsLedgerRemarksEntityEx)ent);
			with_1.CrsCd.Value = receiptEntity.CourseCode;
			if (Information.IsNumeric(receiptEntity.DepartureDate))
			{
				with_1.SyuptDay.Value = (Integer?)receiptEntity.DepartureDate;
			}
			if (Information.IsNumeric(receiptEntity.Car))
			{
				with_1.Gousya.Value = (Integer?)receiptEntity.Car;
			}
			if (Information.IsNumeric(receiptEntity.BranchMumb))
			{
				with_1.Edaban.Value = (Integer?)receiptEntity.BranchMumb;
			}
			with_1.Naiyo.Value = txtNaiyo.Text;
			if (rdoShareJiko.Checked)
			{
				with_1.MemoKbn.Value = NoteClassification.Shared_Items;
			}
			else
			{
				with_1.MemoKbn.Value = string.Empty;
			}
			if (rdoMemo.Checked)
			{
				with_1.MemoBunrui.Value = MemoClassification.memo;
				with_1.KoshakashoCd.Value = null;
				with_1.KoshakashoEdaban.Value = null;
				with_1.StaffShareTaisyo.Value = null;
			}
			else
			{
				object dataSelected = getSelectedRowData();
				if (dataSelected IsNot null)
				{
					if (rdoKousyaKashoContact.Checked)
					{
						with_1.MemoBunrui.Value = MemoClassification.disembarkation_Place_Report;
						with_1.KoshakashoCd.Value = dataSelected["colTaisyoCode"].ToString();
						with_1.KoshakashoEdaban.Value = dataSelected["colKindCode"].ToString();
						with_1.StaffShareTaisyo.Value = null;
					}
					else if (rdoStaffShare.Checked)
					{
						with_1.MemoBunrui.Value = MemoClassification.staff_Sharing;
						with_1.KoshakashoCd.Value = null;
						with_1.KoshakashoEdaban.Value = null;
						with_1.StaffShareTaisyo.Value = dataSelected["colTaisyoCode"].ToString();
					}
				}
			}
		}
	}

	/// <summary>
	/// 選択行のデータを取得
	/// </summary>
	/// <remarks></remarks>
	protected DataRow getSelectedRowData()
	{
		if (ReferenceEquals(grdTaisyo.DataSource, null))
		{
			return null;
		}
		foreach (DataRow dataRow in ((DataTable)grdTaisyo.DataSource).Rows)
		{
			if ((bool)(dataRow["colSelection"]))
			{
				return dataRow;
			}
		}
		return null;
	}

	/// <summary>
	/// 選択行のデータを取得
	/// </summary>
	/// <remarks></remarks>
	protected DataTable getListSelectedRowData()
	{
		object dataTable = createTaisyoDataTable();
		if (ReferenceEquals(grdTaisyo.DataSource, null))
		{
			return null;
		}
		foreach (DataRow dataRow in ((DataTable)grdTaisyo.DataSource).Rows)
		{
			if ((bool)(dataRow["colSelection"]))
			{
				object newRow = dataTable.NewRow();
				newRow.ItemArray = (object[])dataRow.ItemArray.Clone;
				dataTable.Rows.Add(newRow);
			}
		}
		return dataTable;
	}

	#region 登録対象データ登録用SQLの実行[マスタ系]
	/// <summary>
	/// 登録対象データ登録用SQLの実行
	/// </summary>
	/// <returns></returns>
	protected override int ExecuteInsertTran()
	{
		string[] logmsg = new string[2];
		int returnValue = 0;
		try
		{
			//戻り値
			//DBパラメータ
			Hashtable paramInfoList = new Hashtable();
			//crsLedgerRemarks_DAクラス生成
			CrsLedgerRemarks_DA crsLedgerRemarks_DA = new CrsLedgerRemarks_DA();
			//パラメータ作成(共通部)
			CommonLogic.setTableCommonInfo(DbShoriKbn.Insert, this.Name, (iEntity)(updateEntity.EntityData(0)));
			object with_1 = updateEntity.EntityData(0);
			paramInfoList.Add("CrsCd", with_1.crsCd.Value);
			paramInfoList.Add("Gousya", with_1.gousya.Value);
			paramInfoList.Add("SyuptDay", with_1.syuptDay.Value);
			paramInfoList.Add("MemoKbn", with_1.memoKbn.Value);
			paramInfoList.Add("MemoBunrui", with_1.memoBunrui.Value);
			paramInfoList.Add("Naiyo", with_1.Naiyo.Value);
			// 降車ヶ所連絡の場合のみセット
			if (this.rdoKousyaKashoContact.Checked == true)
			{
				paramInfoList.Add("KoshakashoCd", with_1.KoshakashoCd.Value);
				paramInfoList.Add("KoshakashoEdaban", with_1.KoshakashoEdaban.Value);
			}
			else
			{
				paramInfoList.Add("KoshakashoCd", string.Empty);
				paramInfoList.Add("KoshakashoEdaban", string.Empty);
			}
			// スタッフ共有の場合のみセット
			if (this.rdoStaffShare.Checked == true)
			{
				paramInfoList.Add("StaffShareTaisyo", with_1.StaffShareTaisyo.Value);
			}
			else
			{
				paramInfoList.Add("StaffShareTaisyo", string.Empty);
			}
			paramInfoList.Add("DeleteDate", with_1.DeleteDate.Value);
			paramInfoList.Add("SystemEntryPgmid", with_1.systemEntryPgmid.Value);
			paramInfoList.Add("SystemEntryPersonCd", with_1.systemEntryPersonCd.Value);
			paramInfoList.Add("SystemEntryDay", with_1.systemEntryDay.Value);
			paramInfoList.Add("SystemUpdatePgmid", with_1.systemUpdatePgmid.Value);
			paramInfoList.Add("SystemUpdatePersonCd", with_1.systemUpdatePersonCd.Value);
			paramInfoList.Add("SystemUpdateDay", with_1.systemUpdateDay.Value);
			paramInfoList.Add("ListSelected", getListSelectedRowData());
			DataTable maxEdabanDataTable = crsLedgerRemarks_DA.accessCrsLedgerRemarks(crsLedgerRemarks_DA.accessType.getMaxEdaban, paramInfoList);
			object edaban = maxEdabanDataTable.Rows(0).Item("MaxEdaban");
			if (Information.IsDBNull(edaban))
			{
				edaban = 0;
			}
			paramInfoList.Add("Edaban", edaban);
			receiptEntity.BranchMumb = System.Convert.ToString(edaban);
			returnValue = System.Convert.ToInt32(crsLedgerRemarks_DA.executeCrsLedgerRemarks(crsLedgerRemarks_DA.accessType.executeInsertCrsLedgerRemarks, paramInfoList));
		}
		catch (OracleException ex)
		{
			logmsg[0] = this.txtNaiyo.Text + "[" + "ex.Message;" + ex.Message + "ex.Source;" + ex.Source + "ex.StackTrace;" + ex.StackTrace + "]";
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, setTitle, logmsg);
			throw;
			//例外
		}
		catch (Exception ex)
		{
			logmsg[0] = this.txtNaiyo.Text + "[" + "ex.Message;" + ex.Message + "ex.Source;" + ex.Source + "ex.StackTrace;" + ex.StackTrace + "]";
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, setTitle, logmsg);
			throw;
		}
		return returnValue;
	}

	#region 更新対象データ更新用SQLの実行[マスタ系]
	/// <summary>
	/// 更新対象データ更新用SQLの実行
	/// </summary>
	/// <returns></returns>
	protected override int ExecuteUpdateTran()
	{
		string[] logmsg = new string[2];
		//戻り値
		int returnValue = 0;
		try
		{
			//DBパラメータ
			Hashtable paramInfoList = new Hashtable();
			//パラメータ作成(共通部)
			CommonLogic.setTableCommonInfo(DbShoriKbn.Update, Name, (iEntity)(updateEntity.EntityData(0)));
			CrsLedgerRemarks_DA crsLedgerRemarks_DA = new CrsLedgerRemarks_DA();
			object with_1 = updateEntity.EntityData(0);
			paramInfoList.Add("Naiyo", with_1.Naiyo.Value);
			if (ReferenceEquals(with_1.DeleteDate.Value, null) || string.IsNullOrEmpty(System.Convert.ToString(with_1.DeleteDate.Value)))
			{
				paramInfoList.Add("DeleteDate", string.Empty);
			}
			else
			{
				paramInfoList.Add("DeleteDate", Strings.Replace(System.Convert.ToString(with_1.DeleteDate.Value), "/", "", 1, -1, (Microsoft.VisualBasic.CompareMethod)0));
			}
			paramInfoList.Add("SystemUpdatePgmid", with_1.SystemUpdatePgmid.Value);
			paramInfoList.Add("SystemUpdatePersonCd", with_1.systemUpdatePersonCd.Value);
			paramInfoList.Add("SystemUpdateDay", with_1.systemUpdateDay.Value);
			paramInfoList.Add("CrsCd", with_1.crsCd.Value);
			paramInfoList.Add("Gousya", with_1.gousya.Value);
			paramInfoList.Add("SyuptDay", with_1.syuptDay.Value);
			paramInfoList.Add("Edaban", with_1.edaban.Value);
			returnValue = System.Convert.ToInt32(crsLedgerRemarks_DA.executeCrsLedgerRemarks(crsLedgerRemarks_DA.accessType.executeUpdateCrsLedgerRemarks, paramInfoList));
		}
		catch (OracleException ex)
		{
			logmsg[0] = this.txtNaiyo.Text + "[" + "ex.Message;" + ex.Message + "ex.Source;" + ex.Source + "ex.StackTrace;" + ex.StackTrace + "]";
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, setTitle, logmsg);
			throw;
			//例外
		}
		catch (Exception ex)
		{
			logmsg[0] = this.txtNaiyo.Text + "[" + "ex.Message;" + ex.Message + "ex.Source;" + ex.Source + "ex.StackTrace;" + ex.StackTrace + "]";
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, setTitle, logmsg);
			throw;
		}
		return returnValue;
	}
	#endregion
	#endregion

	#endregion

	#endregion

	/// <summary>
	/// 「画面項目定義」の活性/非活性条件（F10：登録後）に記載された内容に準じて、活性/非活性を設定する。
	/// </summary>
	private void setUpViewAfterRegister()
	{
		rdoMemo.Enabled = false;
		rdoKousyaKashoContact.Enabled = false;
		rdoStaffShare.Enabled = false;
		grdTaisyo.AllowEditing = false;
		F4Key_Enabled = true;
		F10Key_Enabled = false;
		F11Key_Enabled = true;
	}

	/// <summary>
	/// 「F4:削除」、「F10：登録」、
	/// 「F11：更新」ボタンを非活性にする
	/// </summary>
	private void SetUpBottomButton(bool hasData)
	{
		if (hasData)
		{
			if (editMode == TypeEdit.Register)
			{
				F10Key_Enabled = true;
				F4Key_Enabled = false;
				F11Key_Enabled = false;
			}
			else
			{
				F10Key_Enabled = false;
				F4Key_Enabled = true;
				F11Key_Enabled = true;
			}
		}
		else
		{
			F4Key_Enabled = false;
			F10Key_Enabled = false;
			F11Key_Enabled = false;
		}
	}

	#region メソッド

	/// <summary>
	/// 画面表示時の初期設定
	/// </summary>
	private void setControlInitiarize()
	{
		//ベースフォームの設定
		setFormId = "S02_0502";
		setTitle = "リマークス登録";
		// フッタボタンの設定
		F1Key_Visible = false;
		F2Key_Visible = true;
		F3Key_Visible = false;
		F4Key_Visible = true;
		F5Key_Visible = false;
		F6Key_Visible = false;
		F7Key_Visible = false;
		F8Key_Visible = false;
		F9Key_Visible = false;
		F10Key_Visible = true;
		F11Key_Visible = true;
		F12Key_Visible = false;

		F2Key_Text = "F2:戻る";
		F4Key_Text = "F4：削除";
		F10Key_Text = "F10:登録";
		F11Key_Text = "F11：更新";

		switch (editMode)
		{
			case TypeEdit.Register:
				F2Key_Enabled = true;
				F10Key_Enabled = true;
				F4Key_Enabled = false;
				F11Key_Enabled = false;
				break;
			case TypeEdit.Update:
				F2Key_Enabled = true;
				F10Key_Enabled = false;
				F4Key_Enabled = true;
				F11Key_Enabled = true;
				rdoMemo.Enabled = false;
				rdoKousyaKashoContact.Enabled = false;
				rdoStaffShare.Enabled = false;
				grdTaisyo.AllowEditing = false;
				break;
		}

		// ボタン位置調整
		this.F2Key_Location_X = 10;
		this.F4Key_Location_X = F2Key_Location_X + (F2Key_Width + 10) * 2;
		this.F10Key_Location_X = 600;
		this.F11Key_Location_X = System.Convert.ToInt32(F10Key_Location_X + F10Key_Width) + 10;

		// フォーカスセット
		txtNaiyo.Select();

	}

	#endregion

	#region モック用
	///<summary>
	///対象一覧のグリッドを初期化する
	///</summary>
	private void clearTaisyoList()
	{
		DataTable dt = createTaisyoDataTable();
		grdTaisyo.DataSource = dt;
		grdTaisyo.Enabled = false;
	}

	/// <summary>
	/// 対象一覧のグリッドを初期化する
	/// </summary>
	private DataTable createTaisyoDataTable()
	{
		DataTable dt = new DataTable();
		dt.Columns.Add("colSelection", typeof(bool));
		dt.Columns.Add("colTaisyo", typeof(string));
		dt.Columns.Add("colKind", typeof(string));
		dt.Columns.Add("colTaisyoCode", typeof(string));
		dt.Columns.Add("colKindCode", typeof(string));
		return dt;
	}

	#endregion

	#region 変更確認
	/// <summary>
	/// 差分チェック
	/// </summary>
	protected override bool checkDifference()
	{
		//差分チェック
		return updateEntity.compare(0);
	}

	/// <summary>
	/// 登録可能チェック2
	/// </summary>
	/// <returns></returns>
	private bool checkCorrelation2()
	{
		System.DateTime syuptDay = DateTime.ParseExact(System.Convert.ToString(updateEntity.EntityData(0).syuptDay.Value.ToString()), "yyyyMMdd", null);
		System.Int32 resultCompare = DateTime.Compare(syuptDay, System.Convert.ToDateTime(CommonProcess.getDateTime.Date));
		if (resultCompare > 0)
		{
			return true;
		}
		if (resultCompare == 0)
		{
			if (receiptEntity.DepartureTime < CommonProcess.getDateTime.ToString("HH:mm"))
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_066", "登録・更新");
				return false;
			}
			else
			{
				return true;
			}
		}
		if (resultCompare < 0)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_066", "登録・更新");
			return false;
		}
		return false;
	}

	/// <summary>
	/// 登録可能チェック1
	/// </summary>
	/// <returns></returns>
	private bool checkCorrelation1()
	{
		CrsLedgerBasic_DA crsLedgerBasicDA = new CrsLedgerBasic_DA();
		Hashtable paraminfoList = new Hashtable();
		object with_1 = updateEntity.EntityData(0);
		paraminfoList.Add("CrsCd", with_1.CrsCd.Value);
		paraminfoList.Add("Gousya", with_1.Gousya.Value);
		paraminfoList.Add("SyuptDay", with_1.SyuptDay.Value);
		object result = crsLedgerBasicDA.accessCrsLedgerBasic(CrsLedgerBasic_DA.accessType.getCrsLedgerBasicByPrimaryKey, paraminfoList);
		if (result.Rows.Count != 1 || ReferenceEquals(result.Rows(0).Item("USING_FLG"), null))
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_050");
			return false;
		}
		return true;
	}

	/// <summary>
	/// 必須選択
	/// </summary>
	private bool checkInsertRequiredItems()
	{
		if (isExistHissuError())
		{
			return false;
		}
		// 共有事項が選択されている
		if (this.rdoShareJiko.Checked)
		{
			// 降車ヶ所連絡またはスタッフ共有が選択されている
			if (this.rdoKousyaKashoContact.Checked || this.rdoStaffShare.Checked)
			{
				//対象一覧内に1件も「選択」がチェックされた行が無い
				if (!ReferenceEquals(getSelectedRowData(), null))
				{
					return true;
				}
			}
			else
			{
				return true;
			}
		}
		CommonProcess.createFactoryMsg().messageDisp("E90_024", "登録・更新対象");
		return false;
	}

	/// <summary>
	/// [更新処理](更新処理入力チェック)
	/// </summary>
	protected override bool checkUpdateItems()
	{
		if (isExistHissuError())
		{
			return false;
		}
		if (!checkCorrelation1())
		{
			return false;
		}
		if (!checkCorrelation2())
		{
			return false;
		}
		return true;
	}

	/// <summary>
	///登録処理入力チェック
	/// </summary>
	protected override bool checkInsertItems()
	{
		if (!checkInsertRequiredItems())
		{
			return false;
		}
		if (!checkCorrelation1())
		{
			return false;
		}
		if (!checkCorrelation2())
		{
			return false;
		}
		return true;
	}
	#endregion
}