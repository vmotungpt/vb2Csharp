using C1.Win.C1FlexGrid;


/// <summary>
/// インターネット予約ログ詳細
/// <remarks>
/// Author: 2018/10/23//QuangTD
/// </remarks>
/// </summary>
public class S02_1902 : FormBase
{

	#region  定数／変数宣言
	/// <summary>
	/// インターネット予約ログ詳細パラメータ
	/// </summary>
	private Delivery_S02_1902Entity receiptEntity = new Delivery_S02_1902Entity();
	/// <summary>
	/// インターネット予約トランザクション管理
	/// </summary>
	private EntityOperation[] updateEntity; 
	/// <summary>
	/// 営業時間内外　営業時間内
	/// </summary>
	private const string salesTimeInside = "営業時間内";
	/// <summary>
	/// 営業時間内外　営業時間外
	/// </summary>
	private const string salesTimeOutside = "営業時間外";
	#endregion
	/// <summary>
	/// コンストラクタ関数
	/// </summary>
	/// <param name="receiptEntity">画面間パラメータ</param>
	public S02_1902(Delivery_S02_1902Entity receiptEntity)
	{
		
		updateEntity = new EntityOperation(Of TIntYoyakuTranManagementEntityEx);


		InitializeComponent();

		this.receiptEntity = receiptEntity;
	}

	/// <summary>
	/// 画面起動時の独自処理
	/// </summary>
	protected override void StartupOrgProc()
	{
		try
		{
			setButtonInitiarize();
			setDataInfomation();
		}
		catch (Exception ex)
		{
			throw (ex);
		}
	}

	/// <summary>
	/// 表示データ取得
	/// </summary>
	private void setDataInfomation()
	{

		// 今回、前回の値クリア
		updateEntity.clear(0, clearType.value);
		updateEntity.clear(0, clearType.zenkaiValue);

		object with_1 = updateEntity.EntityData(0);

		with_1.TranKey.Value = receiptEntity.tranKey;
		with_1.TranKbn.Value = receiptEntity.tranKbn;
		with_1.TranKey.ZenkaiValue = receiptEntity.tranKey;
		with_1.TranKbn.ZenkaiValue = receiptEntity.tranKbn;

		// 予約人数取得
		object ninzuData = getNinzuData();

		// 人数一覧の初期値
		setNinzuData(ninzuData);

		// 詳細データ取得
		object initData = getInitialData();

		if (initData IsNot null && initData.Rows.Count == 1)
		{

			object dataRow = initData.Rows(0);
			// 対応済みのデータを設定
			setCheckbox(dataRow);
			// 曜日のデータを設定
			setYobiData(dataRow);
			// 出発日、予約者名、予約番号、コースコードのデータを設定
			setYoyakuInfo(dataRow);
			// 料金、追加料金、キャンセル料、割引額、取扱手数料 のデータを設定
			setPriceData(dataRow);
			// オーダーID のデータを設定
			setOrderInfo(dataRow);

			with_1.YoyakuKbn.Value = System.Convert.ToString(dataRow["YOYAKU_KBN"]);
			with_1.YoyakuKbn.ZenkaiValue = System.Convert.ToString(dataRow["YOYAKU_KBN"]);
			with_1.YoyakuNo.Value = (Integer?)(dataRow["YOYAKU_NO"]);
			with_1.YoyakuNo.ZenkaiValue = (Integer?)(dataRow["YOYAKU_NO"]);
			with_1.OperationTaishoFlg.Value = if (!Information.IsDBNull(dataRow["OPERATION_TAISHO_FLG"]), System.Convert.ToString(dataRow["OPERATION_TAISHO_FLG"]), string.Empty);
			with_1.OperationTaishoFlg.ZenkaiValue = if (!Information.IsDBNull(dataRow["OPERATION_TAISHO_FLG"]), System.Convert.ToString(dataRow["OPERATION_TAISHO_FLG"]), string.Empty);
			with_1.InvalidFlg.Value = if (!Information.IsDBNull(dataRow["INVALID_FLG"]), System.Convert.ToString(dataRow["INVALID_FLG"]), string.Empty);
			with_1.InvalidFlg.ZenkaiValue = if (!Information.IsDBNull(dataRow["INVALID_FLG"]), System.Convert.ToString(dataRow["INVALID_FLG"]), string.Empty);
		}
		else
		{

			F11Key_Enabled = false;
			F6Key_Enabled = false;
		}

		// 処理状況一覧のデータを設定
		setProcessSituationProcess(initData);
		// 取込結果一覧にデータを設定
		setToriatukaiResultListData(initData);
	}

	/// <summary>
	/// 曜日のデータを設定する
	/// </summary>
	/// <param name="dataRow"></param>
	private void setYobiData(DataRow dataRow)
	{

		string yobi = System.Convert.ToString(CommonDateUtil.chkDayConversion(dataRow["CRS_SYUPT_DAY_YOBI"].ToString(), (System.Convert.ToInt32(FixedCd.CodeBunrui.yobi)).ToString()));

		if (!string.IsNullOrEmpty(yobi))
		{
			lblYobi.Text = "(" + System.Convert.ToString(yobi[0]) + ")";
		}
	}

	/// <summary>
	/// 取込結果一覧にデータを設定する
	/// </summary>
	/// <param name="dataTableToriatukaiResult"></param>
	private void setToriatukaiResultListData(DataTable dataTableToriatukaiResult)
	{
		grdToriatukaiResultList.AutoGenerateColumns = false;
		if (dataTableToriatukaiResult.Rows.Count <= 0)
		{
			object newRow = dataTableToriatukaiResult.NewRow;
			newRow.Item["COL_YOYAKU"] = "×";
			newRow.Item["TRAN_KEY"] = "1";
			newRow.Item["TRAN_KBN"] = "1";
			dataTableToriatukaiResult.Rows.Add(newRow);
			grdToriatukaiResultList.DataSource = dataTableToriatukaiResult;
		}
		else
		{
			object row = dataTableToriatukaiResult.Rows(0);
			row.Item["COL_SETTLEMENT_CANCEL"] = string.Empty;
			grdToriatukaiResultList.DataSource = dataTableToriatukaiResult;
		}
	}
	/// <summary>
	/// オーダーID のデータを設定する
	/// </summary>
	/// <param name="dataRow"></param>
	private void setOrderInfo(DataRow dataRow)
	{

		txtOrderId.Text = dataRow["ORDER_ID"].ToString().Trim();
		txtOrderId.ReadOnly = true;
	}

	/// <summary>
	/// 料金、追加料金、キャンセル料、割引額、取扱手数料 のデータを設定する
	/// </summary>
	/// <param name="dataRow"></param>
	private void setPriceData(DataRow dataRow)
	{
		// 料金
		txtCharge.Text = (System.Convert.ToInt32(dataRow["SEIKI_CHARGE_ALL_GAKU"])).ToString("#,0");
		txtCharge.ReadOnly = true;
		// 追加料金
		txtAddCharge.Text = (System.Convert.ToInt32(dataRow["ADD_CHARGE_MAEBARAI_KEI"])).ToString("#,0");
		txtAddCharge.ReadOnly = true;
		// キャンセル料
		txtCancelRyou.Text = (System.Convert.ToInt32(dataRow["CANCEL_RYOU"])).ToString("#,0");
		txtCancelRyou.ReadOnly = true;
		// 割引額
		txtWaribikiGaku.Text = (System.Convert.ToInt32(dataRow["WARIBIKI_ALL_GAKU"])).ToString("#,0");
		txtWaribikiGaku.ReadOnly = true;
		// 取扱手数料
		txtToriatukaiFee.Text = (System.Convert.ToInt32(dataRow["TORIATUKAI_FEE_SAGAKU"])).ToString("#,0");
		txtToriatukaiFee.ReadOnly = true;
		// 合計
		txtTotal.Text = (System.Convert.ToInt32(dataRow["COL_TOTAL"])).ToString("#,0");
		txtTotal.ReadOnly = true;
		txtKiSettlementKingaku.Text = (System.Convert.ToInt32(dataRow["TXT_KI_SETTLEMENT_KINGAKU"])).ToString("#,0") + "円";
		txtKiSettlementKingaku.ReadOnly = true;
	}

	/// <summary>
	/// 出発日、予約者名、予約番号、コースコードのデータを設定する
	/// </summary>
	/// <param name="dataRow"></param>
	private void setYoyakuInfo(DataRow dataRow)
	{

		// 出発日
		lblSyuptDay.Text = dataRow["CRS_SYUPT_DAY"].ToString();
		// コースコード
		lblCrsCd.Text = dataRow["CRS_CD"].ToString();
		// コース名 + 号車
		lblCrsName.Text = string.Format("{0}　{1}号車", dataRow["CRS_NAME"].ToString().Trim(), dataRow["GOUSYA"].ToString());
		// 予約番号
		string yoyakuKbn = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(dataRow["YOYAKU_KBN"]));
		int yoyakuNo = System.Convert.ToInt32(CommonRegistYoyaku.convertObjectToInteger(dataRow["YOYAKU_NO"]));
		txtYoyakuNo.Text = CommonRegistYoyaku.createManagementNumber(yoyakuKbn, yoyakuNo);
		txtYoyakuNo.ReadOnly = true;
		// 予約社名
		txtYoyakuPersonName.Text = dataRow["YYKMKS_NAME"].ToString();
		// 電話番号
		txtTelNo.Text = dataRow["YYKMKS_TEL_1"].ToString();
		txtTelNo.ReadOnly = true;
		// 代理店名
		txtAgent.Text = dataRow["AGENT_NM"].ToString();
		txtAgent.ReadOnly = true;
	}

	/// <summary>
	///  人数一覧の初期値
	/// </summary>
	/// <param name="dataRow"></param>
	private void setNinzuData(DataTable dataRow)
	{
		if (dataRow.Rows.Count != 0)
		{
			int colAdult = 0;
			int colJunior = 0;
			int colChild = 0;
			foreach (DataRow row in dataRow.Rows)
			{
				System.Object shuyakuChargeKbnCd = row["SHUYAKU_CHARGE_KBN_CD"];
				if (!Information.IsDBNull(shuyakuChargeKbnCd))
				{
					switch (System.Convert.ToString(shuyakuChargeKbnCd))
					{
						case "10":
							colAdult = System.Convert.ToInt32(if (Information.IsDBNull(row["TOTAL_CHARGE_APPLICATION_NINZU"]), 0, System.Convert.ToInt32(row["TOTAL_CHARGE_APPLICATION_NINZU"])));
							break;
						case "20":
							colJunior = System.Convert.ToInt32(if (Information.IsDBNull(row["TOTAL_CHARGE_APPLICATION_NINZU"]), 0, System.Convert.ToInt32(row["TOTAL_CHARGE_APPLICATION_NINZU"])));
							break;
						case "30":
							colChild = System.Convert.ToInt32(if (Information.IsDBNull(row["TOTAL_CHARGE_APPLICATION_NINZU"]), 0, System.Convert.ToInt32(row["TOTAL_CHARGE_APPLICATION_NINZU"])));
							break;
						default:
							break;
					}
				}
			}
			DataTable dataTableNinzu = new DataTable();
			dataTableNinzu.Columns.Add("COLUMN1", typeof(string));
			dataTableNinzu.Columns.Add("COLUMN2", typeof(string));
			object rowAdult = dataTableNinzu.NewRow();
			rowAdult.Item["COLUMN1"] = "大人";
			rowAdult.Item["COLUMN2"] = if (colAdult > 0, colAdult.ToString(), string.Empty);
			object rowJunior = dataTableNinzu.NewRow();
			rowJunior.Item["COLUMN1"] = "中人";
			rowJunior.Item["COLUMN2"] = if (colJunior > 0, colJunior.ToString(), string.Empty);
			object rowChild = dataTableNinzu.NewRow();
			rowChild.Item["COLUMN1"] = "小人";
			rowChild.Item["COLUMN2"] = if (colChild > 0, colChild.ToString(), string.Empty);
			dataTableNinzu.Rows.Add(rowAdult);
			dataTableNinzu.Rows.Add(rowJunior);
			dataTableNinzu.Rows.Add(rowChild);
			grdNinzuList.DataSource = dataTableNinzu;
			txtYoyakuNinzu.Text = (colAdult + colJunior + colChild).ToString() + "人";
			grdNinzuList.Cols(1).Caption = string.Empty;
			grdNinzuList.Cols(2).Caption = "人数";
			grdNinzuList.Cols(0).Width = 0;
			grdNinzuList.Cols(1).Width = 100;
			grdNinzuList.Cols(2).Width = 100;
			grdNinzuList.Cols(2).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.RightTop;
			grdNinzuList.Cols(2).TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
			grdNinzuList.Cols(1).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
			grdNinzuList.Cols(1).TextAlignFixed = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
			grdNinzuList.Select(null);
		}
	}

	/// <summary>
	/// 処理状況一覧のデータを設定する
	/// </summary>
	/// <param name="dataTable"></param>
	private void setProcessSituationProcess(DataTable dataTable)
	{
		DataTable dataProcessSituation = new DataTable();
		dataProcessSituation.Columns.Add("COLUMN1", typeof(string));
		dataProcessSituation.Columns.Add("COLUMN2", typeof(string));
		System.String processUketukeDate = string.Empty;
		System.String latestProcessStatus = string.Empty;
		if (dataTable.Rows.Count > 0)
		{
			object dataRow = dataTable.Rows(0);
			processUketukeDate = dataRow["COL_PROCESS_UKETUKE_DATE"].ToString();
			latestProcessStatus = dataRow["LATEST_PROCESS_STATUS"].ToString();
		}
		object rowProcessUketukeDate = dataProcessSituation.NewRow();
		rowProcessUketukeDate.Item["COLUMN1"] = "処理受付日時";
		rowProcessUketukeDate.Item["COLUMN2"] = processUketukeDate;
		string salesTimeInsideOutside = string.Empty;

		if (dataTable.Rows(0)["TRAN_KBN"].Equals("B") ||)
		{
			dataTable.Rows(0)["TRAN_KBN"].Equals("D");
			salesTimeInsideOutside = salesTimeInside;
		}
		else
		{
			salesTimeInsideOutside = salesTimeOutside;
		}
		object rowSituation = dataProcessSituation.NewRow();
		rowSituation.Item["COLUMN1"] = "状況";
		rowSituation.Item["COLUMN2"] = "（" + salesTimeInsideOutside + ")" + latestProcessStatus;
		dataProcessSituation.Rows.Add(rowProcessUketukeDate);
		dataProcessSituation.Rows.Add(rowSituation);
		grdProcessSituationList.DataSource = dataProcessSituation;
		grdProcessSituationList.Rows(0).Height = 0;
		grdProcessSituationList.Cols(0).Width = 0;
		grdProcessSituationList.Cols(1).Width = 100;
		grdProcessSituationList.Cols(2).Width = 150;
		CellStyle cellStyle = this.grdProcessSituationList.Styles.Add("style");
		cellStyle.BackColor = FixedCdYoyaku.ControlColor.ControlLight;
		this.grdProcessSituationList.SetCellStyle(1, 1, cellStyle);
		this.grdProcessSituationList.SetCellStyle(2, 1, cellStyle);
		grdProcessSituationList.Select(null);
	}

	/// <summary>
	/// 対応済みのデータを設定する
	/// </summary>
	/// <param name="dataRow"></param>
	private void setCheckbox(DataRow dataRow)
	{
		object operationTaishoFlg = if (!Information.IsDBNull(dataRow["OPERATION_TAISHO_FLG"]), dataRow("OPERATION_TAISHO_FLG").ToString(), string.Empty);
		object invalidFlg = if (!Information.IsDBNull(dataRow["INVALID_FLG"]), dataRow("INVALID_FLG").ToString(), string.Empty);
		this.lblOperationTaisyoDay.Text = dataRow["OPERATION_TAISHO_DAY"].ToString();
		this.lblOperationTaisyoTime.Text = dataRow["OPERATION_TAISHO_TIME"].ToString();
		if ((string)operationTaishoFlg == "Y" && string.IsNullOrEmpty(System.Convert.ToString(invalidFlg)))
		{
			chkTaiouAlready.Checked = true;
			chkTaiouAlready.Enabled = true;
			chkTaiouHuyo.Checked = false;
			chkTaiouHuyo.Enabled = false;
		}
		if (string.IsNullOrEmpty(System.Convert.ToString(operationTaishoFlg)) && (string)invalidFlg == "Y")
		{
			chkTaiouAlready.Checked = false;
			chkTaiouAlready.Enabled = true;
			chkTaiouHuyo.Checked = true;
			chkTaiouHuyo.Enabled = true;
		}
		if (string.IsNullOrEmpty(System.Convert.ToString(operationTaishoFlg)) && string.IsNullOrEmpty(System.Convert.ToString(invalidFlg)))
		{
			chkTaiouAlready.Checked = false;
			chkTaiouAlready.Enabled = true;
			chkTaiouHuyo.Checked = false;
			chkTaiouHuyo.Enabled = true;
		}
	}

	/// <summary>
	/// 予約人数取得
	/// </summary>
	/// <returns></returns>
	private DataTable getNinzuData()
	{
		IntYoyakuTranManagement_DA dataAccess = new IntYoyakuTranManagement_DA();
		DataTable result = new DataTable();
		Hashtable paramList = new Hashtable();
		paramList.Add("tranKey", updateEntity.EntityData(0).TranKey.Value);
		paramList.Add("tranKbn", updateEntity.EntityData(0).TranKbn.Value);
		result = dataAccess.accessIntYoyakuTranManagement(IntYoyakuTranManagement_DA.accessType.getChargeApplicationNinzu, paramList);
		return result;
	}

	/// <summary>
	/// 初期化
	/// </summary>
	/// <returns></returns>
	private DataTable getInitialData()
	{
		IntYoyakuTranManagement_DA dataAccess = new IntYoyakuTranManagement_DA();
		DataTable result = new DataTable();
		Hashtable paramList = new Hashtable();
		paramList.Add("tranKey", updateEntity.EntityData(0).TranKey.Value);
		paramList.Add("tranKbn", updateEntity.EntityData(0).TranKbn.Value);
		result = dataAccess.accessIntYoyakuTranManagement(IntYoyakuTranManagement_DA.accessType.getIntYoyakuTranLogDetail, paramList);
		return result;
	}

	/// <summary>
	/// 更新対象項目をエンティティにセット
	/// </summary>
	protected override void setEntityDataValue()
	{
		if (chkTaiouAlready.Enabled)
		{
			//エンティティの初期化
			updateEntity.clear(0, clearType.value);
			updateEntity.clear(0, clearType.errorInfo);
			displayDataToEntity(updateEntity.EntityData(0));
		}
	}

	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">>行定表連結記号エンティティ</param>
	private void displayDataToEntity(object ent)
	{
		//TODO 280
		if (typeof(TIntYoyakuTranManagementEntityEx).Equals(ent.GetType()))
		{
			TIntYoyakuTranManagementEntityEx with_1 = ((TIntYoyakuTranManagementEntityEx)ent);
			if (chkTaiouAlready.Checked)
			{
				with_1.OperationTaishoFlg.Value = "Y";
			}
			else if (chkTaiouHuyo.Checked)
			{
				with_1.InvalidFlg.Value = "Y";
			}
			else
			{
				with_1.OperationTaishoFlg.Value = string.Empty;
				with_1.InvalidFlg.Value = string.Empty;
			}
			with_1.TranKbn.Value = receiptEntity.tranKbn;
			with_1.TranKey.Value = receiptEntity.tranKey;
			if (!string.IsNullOrEmpty(System.Convert.ToString(txtYoyakuNo.Text.Replace(",", ""))))
			{
				with_1.YoyakuKbn.Value = txtYoyakuNo.Text.Replace(",", "").Substring(0, 1);
				with_1.YoyakuNo.Value = (Integer?)(txtYoyakuNo.Text.Substring(1, txtYoyakuNo.Text.Length - 1).Replace(",", ""));
			}
		}
	}

	#region イベント
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

	/// <summary>
	/// [変更チェック](差分チェック)
	/// </summary>
	/// <returns></returns>
	protected override bool checkDifference()
	{
		return updateEntity.compare(0);
	}

	/// <summary>
	/// [更新処理](更新処理入力チェック)
	/// </summary>
	/// <returns></returns>
	protected override bool checkUpdateItems()
	{
		base.clearExistErrorProperty(this.gbxCondition.Controls);
		//TODO QA 280
		if (chkTaiouAlready.Checked && chkTaiouHuyo.Checked)
		{
			chkTaiouAlready.ExistError = true;
			chkTaiouHuyo.ExistError = true;
			createFactoryMsg().messageDisp("E90_006", "どちらか片方のみ指定できます。双方の指定");
			return false;
		}
		return true;
	}

	/// <summary>
	/// F6ボタン押下時の独自処理
	/// </summary>
	protected override void btnF6_ClickOrgProc()
	{
		S02_0103ParamData prm = new S02_0103ParamData();
		//予約ボタン押下
		//画面間パラメータを用意
		//予約区分
		prm.YoyakuKbn = updateEntity.EntityData(0).YoyakuKbn.Value;
		//予約NO
		prm.YoyakuNo = System.Convert.ToInt32(updateEntity.EntityData(0).YoyakuNo.Value);
		// 参照モード
		prm.ScreenMode = CommonRegistYoyaku.ScreenModeReference;

		using (S02_0103 form = new S02_0103())
		{

			form.ParamData = prm;

			this.Visible = false;
			form.ShowDialog();
			this.Visible = true;
		}

	}

	/// <summary>
	/// F11()ボタン押下時の独自処理
	/// </summary>
	protected override void btnF11_ClickOrgProc()
	{
		string[] logmsg = new string[2];
		try
		{
			int res = System.Convert.ToInt32(DbOperator(DbShoriKbn.Update));
			logmsg[0] = System.Convert.ToString(this.txtAgent.Text);
			if (res > 0)
			{
				setDataInfomation();
				createFactoryMsg().messageDisp("I90_002", "登録");
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.update, setTitle, logmsg);
			}
			else if (res == base.RET_CANCEL)
			{
				//処理確認でキャンセルの場合
				return;
			}
			else if (res == base.RET_NONEXEC)
			{
				//更新対象無し以外の場合
				return;
			}
			else
			{
				createFactoryMsg().messageDisp("E90_014", "登録");
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
			}
		}
		catch (Exception ex)
		{
			//ログ出力(エラーログ)
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
		}
	}

	/// <summary>
	/// F2()ボタン押下時の独自処理
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		//戻る
		base.closeFormFlg = true;
		Close();
	}

	#region 更新対象データ更新用SQLの実行[マスタ系]
	/// <summary>
	/// 更新対象データ更新用SQLの実行
	/// </summary>
	/// <returns></returns>
	protected override int ExecuteUpdateTran()
	{
		//戻り値
		int returnValue = 0;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		//パラメータ作成(共通部)
		IntYoyakuTranManagement_DA dataAccess = new IntYoyakuTranManagement_DA();
		CommonLogic.setTableCommonInfo(DbShoriKbn.Update, this.Name, (iEntity)(updateEntity.EntityData(0)));
		object with_1 = updateEntity.EntityData(0);
		with_1.OperationTaishoDay.Value = (Integer?)(getDateTime.ToString("yyyyMMdd"));
		with_1.OperationTaishoTime.Value = (Integer?)(getDateTime.ToString("HHmmss"));
		with_1.OperationTaishoPerson.Value = UserInfoManagement.userId;
		with_1.OperationTaishoPgmid.Value = Name;
		with_1.UpdateDay.Value = with_1.OperationTaishoDay.Value;
		with_1.UpdateTime.Value = with_1.OperationTaishoTime.Value;
		with_1.UpdatePerson.Value = with_1.OperationTaishoPerson.Value;
		with_1.UpdatePgmid.Value = with_1.OperationTaishoPgmid.Value;
		paramInfoList.Add("operationTaishoFlg", with_1.OperationTaishoFlg.Value);
		//TODO QA.
		paramInfoList.Add("invalidFlg", with_1.InvalidFlg.Value);
		paramInfoList.Add("operationTaishoDay", with_1.OperationTaishoDay.Value);
		paramInfoList.Add("operationTaishoTime", with_1.OperationTaishoTime.Value);
		paramInfoList.Add("operationTaishoPerson", with_1.OperationTaishoPerson.Value);
		paramInfoList.Add("operationTaishoPgmid", with_1.OperationTaishoPgmid.Value);
		paramInfoList.Add("updateDay", with_1.UpdateDay.Value);
		paramInfoList.Add("updateTime", with_1.UpdateTime.Value);
		paramInfoList.Add("updatePerson", with_1.UpdatePerson.Value);
		paramInfoList.Add("updatePgmid", with_1.UpdatePgmid.Value);
		paramInfoList.Add("tranKey", with_1.TranKey.Value);
		paramInfoList.Add("tranKbn", with_1.TranKbn.Value);
		returnValue = System.Convert.ToInt32(dataAccess.executeIntYoyakuTranManagement(IntYoyakuTranManagement_DA.accessType.executeUpdateIntYoyokuTranLogDetail, paramInfoList));
		return returnValue;
	}
	#endregion
	#endregion

	#region メソッド
	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{
		this.F1Key_Visible = false;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F7Key_Visible = false;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = false;
		this.F12Key_Visible = false;
		this.F2Key_Visible = true;
		this.F6Key_Visible = true;
		this.F11Key_Visible = true;
		this.F6Key_Text = "F6：予約照会";
		grdProcessSituationList.Enabled = false;
		grdToriatukaiResultList.Enabled = false;
		grdNinzuList.Enabled = false;
		grdProcessSituationList.Select(null);
		grdToriatukaiResultList.Select(null);
		grdNinzuList.Select(null);
	}

	#endregion
}