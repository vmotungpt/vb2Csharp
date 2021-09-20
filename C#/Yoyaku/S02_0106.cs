/// <summary>
/// S02-0106 メモ入力
/// <remarks>
/// Author:2018/10/31//DatPV
/// </remarks>
/// </summary>
public class S02_0106 : FormBase
{
	//確認済み Y：チェックON
	private const string verifyCode = "Y";
	Delivery_S02_0106Entity receiptEntity = new Delivery_S02_0106Entity();
	private EntityOperation clsTYoyakuInfoMemoEntity; 
	private EntityOperation clsTWtRequestInfoMemoEntity; 
														 //編集モード
	private string editMode;
	private const string dateFormat = "yyyyMMdd";
	//再検索フラグ
	private bool reSearchFlag = false;
	//F4 Click
	private bool deleteClick = false;
	/// <summary>
	/// "1"（新規登録)`
	/// </summary>
	private const string signUpMode = "1";
	/// <summary>
	/// "2"（更新(削除)）
	/// </summary>
	private const string updateMode = "2";
	/// <summary>
	/// "0"（通常）
	/// </summary>
	public const string signUpNormalType = "0";
	/// <summary>
	/// "1"（WT)
	/// </summary>
	public const string signUpWTType = "1";
	/// <summary>
	/// "2"（リクエスト)
	/// </summary>
	public const string signUpRequestType = "2";
	// "0" Checkbox not checked value
	private const string unCheckedValue = "0";
	// "0" Checkbox checked value
	private const string checkedValue = "1";
	// "True" True value
	private const string trueValue = "True";

	/// <summary>
	/// 削除日デフォルト（非削除時）
	/// </summary>
	private const int NotDeleteDayValue = 0;

	/// <summary>
	/// ＤＢ問合せ区分
	/// </summary>
	private enum DbTableKbn : int
	{
		//0
		getUpdateData,
		//1
		getPlaceReportData,
		//2
		getStaffSharingData
	}

	/// <summary>
	/// Contructor
	/// </summary>
	/// <param name="receipt"></param>
	public S02_0106(Delivery_S02_0106Entity receipt)
	{
		clsTYoyakuInfoMemoEntity = new EntityOperation(Of TYoyakuInfoMemoEntityEx);
		clsTWtRequestInfoMemoEntity = new EntityOperation(Of TWtRequestInfoMemoEntityEx);

		//画面パターン毎の初期化処理
		InitializeComponent();
		//更新対象エリアの初期設定
		receiptEntity.yoyakuKbn = receipt.yoyakuKbn;
		receiptEntity.yoyakuNo = receipt.yoyakuNo;
		receiptEntity.edaban = receipt.edaban;
		receiptEntity.editMode = receipt.editMode;
		receiptEntity.registerType = receipt.registerType;
	}

	/// <summary>
	/// 各種処理情報を格納する
	/// </summary>
	/// <param name="researchFlg"></param>
	public void SetResearchFlg(bool researchFlg)
	{
		this.reSearchFlag = researchFlg;
	}

	#region イベント
	#region F4
	/// <summary>
	/// F4ボタン押下時の独自処理
	/// </summary>
	protected override void btnF4_ClickOrgProc()
	{
		//更新対象項目をエンティティにセット
		this.setEntityDataValue();
		string[] logmsg = new string[2];
		this.deleteClick = true;
		//戻り値
		int returnValue = 0;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		//DAクラス作成、パラメータ設定、登録(更新)処理実施までを実装
		//DataAccessクラス生成
		S02_0106Da dataAccess = new S02_0106Da();
		//更新時の情報の値を設定する
		if (receiptEntity.registerType == signUpNormalType)
		{
			//通常予約の場合
			CommonLogic.setTableCommonInfo(DbShoriKbn.Update, this.Name, (iEntity)(clsTYoyakuInfoMemoEntity.EntityData(0)));
			//必須項目のチェック
			if (this.inputDataAreaError() == true)
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_022");
				return;
			}
			object with_1 = clsTYoyakuInfoMemoEntity.EntityData(0);
			//予約区分
			paramInfoList.Add("YoyakuKbn", with_1.YoyakuKbn.Value);
			//予約ＮＯ
			paramInfoList.Add("YoyakuNo", with_1.YoyakuNo.Value);
			//枝番
			paramInfoList.Add("Edaban", with_1.Edaban.Value);
			//内容
			paramInfoList.Add("Naiyo", with_1.Naiyo.Value);
			//削除日
			paramInfoList.Add("DeleteDate", getDateTime().ToString(dateFormat));
			//システム登録日
			paramInfoList.Add("SystemUpdateDay", with_1.SystemUpdateDay.Value);
			//システム更新ＰＧＭＩＤ
			paramInfoList.Add("SystemUpdatePgmid", with_1.SystemUpdatePgmid.Value);
			//システム登録者コード
			paramInfoList.Add("SystemUpdatePersonCd", with_1.SystemUpdatePersonCd.Value);
			//登録内容
			paramInfoList.Add("RegisterType", receiptEntity.registerType);
			//処理実施確認
			if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", "削除") == MsgBoxResult.Cancel)
			{
				return;
			}
			else
			{
				try
				{
					//Insertの実施
					returnValue = System.Convert.ToInt32(dataAccess.executeUpdate(S02_0106Da.accessType.executeUpdate, paramInfoList));
					logmsg[0] = System.Convert.ToString(this.txtNaiyo.Text);
					if (returnValue > 0)
					{
						//正常終了メッセージ表示
						CommonProcess.createFactoryMsg().messageDisp("I90_002", "削除");
						//ログ出力(操作ログ)
						createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.update, setTitle, logmsg);
					}
					else
					{
						//異常終了メッセージ表示
						CommonProcess.createFactoryMsg().messageDisp("E90_025", "処理", "削除");
						//ログ出力(エラーログ)
						createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
					}
					//Oracleの例外
				}
				catch (OracleException ex)
				{
					logmsg[0] = this.txtNaiyo.Text + "[" + "ex.Message;" + ex.Message + "ex.Source;" + ex.Source + "ex.StackTrace;" + ex.StackTrace + "]";
					createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
					throw;
					//例外
				}
				catch (Exception ex)
				{
					logmsg[0] = this.txtNaiyo.Text + "[" + "ex.Message;" + ex.Message + "ex.Source;" + ex.Source + "ex.StackTrace;" + ex.StackTrace + "]";
					createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
					throw;
				}
			}
		}
		else
		{
			//WT、リクエストの場合
			CommonLogic.setTableCommonInfo(DbShoriKbn.Update, this.Name, (iEntity)(clsTWtRequestInfoMemoEntity.EntityData(0)));
			//必須項目のチェック
			if (this.inputDataAreaError() == true)
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_022");
				return;
			}
			object with_2 = clsTWtRequestInfoMemoEntity.EntityData(0);
			//管理区分
			paramInfoList.Add("ManagementKbn", with_2.ManagementKbn.Value);
			//管理ＮＯ
			paramInfoList.Add("ManagementNo", with_2.ManagementNo.Value);
			//枝番
			paramInfoList.Add("Edaban", with_2.Edaban.Value);
			//内容
			paramInfoList.Add("Naiyo", with_2.Naiyo.Value);
			//削除日
			paramInfoList.Add("DeleteDate", getDateTime().ToString(dateFormat));
			//システム登録日
			paramInfoList.Add("SystemUpdateDay", with_2.SystemUpdateDay.Value);
			//システム更新ＰＧＭＩＤ
			paramInfoList.Add("SystemUpdatePgmid", with_2.SystemUpdatePgmid.Value);
			//システム登録者コード
			paramInfoList.Add("SystemUpdatePersonCd", with_2.SystemUpdatePersonCd.Value);
			//登録内容
			paramInfoList.Add("RegisterType", receiptEntity.registerType);
			//処理実施確認
			if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", "削除") == MsgBoxResult.Cancel)
			{
				return;
			}
			else
			{
				try
				{
					//Insertの実施
					returnValue = System.Convert.ToInt32(dataAccess.executeUpdate(S02_0106Da.accessType.executeUpdate, paramInfoList));
					logmsg[0] = System.Convert.ToString(this.txtNaiyo.Text);
					if (returnValue > 0)
					{
						//正常終了メッセージ表示
						CommonProcess.createFactoryMsg().messageDisp("I90_002", "削除");
						//ログ出力(操作ログ)
						createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.update, setTitle, logmsg);
					}
					else
					{
						//異常終了メッセージ表示
						CommonProcess.createFactoryMsg().messageDisp("E90_025", "処理", "削除");
						//ログ出力(エラーログ)
						createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
					}
					//Oracleの例外
				}
				catch (OracleException ex)
				{
					logmsg[0] = this.txtNaiyo.Text + "[" + "ex.Message;" + ex.Message + "ex.Source;" + ex.Source + "ex.StackTrace;" + ex.StackTrace + "]";
					createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
					throw;
					//例外
				}
				catch (Exception ex)
				{
					logmsg[0] = this.txtNaiyo.Text + "[" + "ex.Message;" + ex.Message + "ex.Source;" + ex.Source + "ex.StackTrace;" + ex.StackTrace + "]";
					createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
					throw;
				}
			}
		}
		//各種処理情報を格納する
		SetResearchFlg(true);
		//F2ボタン押下時の独自処理
		btnF2_ClickOrgProc();
	}
	#endregion

	#region F11
	/// <summary>
	/// F11ボタン押下時の独自処理
	/// </summary>
	protected override void btnF11_ClickOrgProc()
	{
		//変更確認追加
		//Entityに値をセット
		setEntityDataValue();
		if (checkDifference() == true)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_068");
			return;
		}
		string[] logmsg = new string[2];
		//戻り値
		int returnValue = 0;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		//DAクラス作成、パラメータ設定、登録(更新)処理実施までを実装
		//呼ばれる時点で、UpdateEntit.EntityData(0)には画面の値は設定されている
		//DataAccessクラス生成
		S02_0106Da dataAccess = new S02_0106Da();
		//更新時の情報の値を設定する
		if (receiptEntity.registerType == signUpNormalType)
		{
			//通常予約の場合
			CommonLogic.setTableCommonInfo(DbShoriKbn.Update, this.Name, (iEntity)(clsTYoyakuInfoMemoEntity.EntityData(0)));
			//必須項目のチェック
			if (this.inputDataAreaError())
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_022");
				return;
			}
			//パラメータ設定
			object with_1 = clsTYoyakuInfoMemoEntity.EntityData(0);
			//予約区分
			paramInfoList.Add("YoyakuKbn", with_1.YoyakuKbn.Value);
			//予約ＮＯ
			paramInfoList.Add("YoyakuNo", with_1.YoyakuNo.Value);
			//枝番
			paramInfoList.Add("Edaban", with_1.Edaban.Value);
			//内容
			paramInfoList.Add("Naiyo", this.txtNaiyo.Text);
			//システム登録日
			paramInfoList.Add("SystemUpdateDay", with_1.SystemUpdateDay.Value);
			//システム更新ＰＧＭＩＤ
			paramInfoList.Add("SystemUpdatePgmid", with_1.SystemUpdatePgmid.Value);
			//システム登録者コード
			paramInfoList.Add("SystemUpdatePersonCd", with_1.SystemUpdatePersonCd.Value);
			//登録内容
			paramInfoList.Add("RegisterType", receiptEntity.registerType);
		}
		else
		{
			//WT、リクエストの場合
			CommonLogic.setTableCommonInfo(DbShoriKbn.Update, this.Name, (iEntity)(clsTWtRequestInfoMemoEntity.EntityData(0)));
			//必須項目のチェック
			if (this.inputDataAreaError())
			{
				CommonProcess.createFactoryMsg().messageDisp("E90_022");
				return;
			}
			//パラメータ設定
			object with_2 = clsTWtRequestInfoMemoEntity.EntityData(0);
			//予約区分
			paramInfoList.Add("ManagementKbn", with_2.ManagementKbn.Value);
			//予約ＮＯ
			paramInfoList.Add("ManagementNo", with_2.ManagementNo.Value);
			//枝番
			paramInfoList.Add("Edaban", with_2.Edaban.Value);
			//内容
			paramInfoList.Add("Naiyo", this.txtNaiyo.Text);
			//システム登録日
			paramInfoList.Add("SystemUpdateDay", with_2.SystemUpdateDay.Value);
			//システム更新ＰＧＭＩＤ
			paramInfoList.Add("SystemUpdatePgmid", with_2.SystemUpdatePgmid.Value);
			//システム登録者コード
			paramInfoList.Add("SystemUpdatePersonCd", with_2.SystemUpdatePersonCd.Value);
			//登録内容
			paramInfoList.Add("RegisterType", receiptEntity.registerType);
		}
		//処理実施確認
		if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", "更新") == MsgBoxResult.Cancel)
		{
			return;
		}
		else
		{
			try
			{
				//Insertの実施
				returnValue = System.Convert.ToInt32(dataAccess.executeUpdate(S02_0106Da.accessType.executeUpdate, paramInfoList));
				logmsg[0] = System.Convert.ToString(this.txtNaiyo.Text);
				if (returnValue > 0)
				{
					//正常終了メッセージ表示
					CommonProcess.createFactoryMsg().messageDisp("I90_002", "更新");
					//【各画面毎】差分チェック用情報の格納
					this.saveCheckDifferent();
					//ログ出力(操作ログ)
					createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.update, setTitle, logmsg);
				}
				else
				{
					//異常終了メッセージ表示
					CommonProcess.createFactoryMsg().messageDisp("E90_025", "処理", "更新");
					//ログ出力(エラーログ)
					createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
				}
				//Oracleの例外
			}
			catch (OracleException ex)
			{
				logmsg[0] = this.txtNaiyo.Text + "[" + ex.Message + "]";
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
				throw;
				//例外
			}
			catch (Exception ex)
			{
				logmsg[0] = this.txtNaiyo.Text + "[" + ex.Message + "]";
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.update, setTitle, logmsg);
				throw;
			}
		}
		//再検索フラグを設定
		this.SetResearchFlg(true);
		//画面の編集モードを”更新（削除）”に変更する。
		this.editMode = updateMode;
		//表示制御
		this.setUpdateStatus();
	}
	#endregion

	#region F2
	/// <summary>
	/// F2ボタン押下時の独自処理
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{
		//F4 Click
		if (this.deleteClick == true)
		{
			base.closeFormFlg = false;
			this.Close();
		}
		else
		{
			base.closeFormFlg = true;
			Close();
		}
	}

	#endregion

	#region F10
	/// <summary>
	/// F10ボタン押下時の独自処理
	/// </summary>
	protected override void btnF10_ClickOrgProc()
	{
		//更新対象項目をエンティティにセット
		this.setEntityDataValue();
		string[] logmsg = new string[2];
		//戻り値
		int returnValue = 0;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		//DAクラス作成、パラメータ設定、登録(更新)処理実施までを実装
		//DataAccessクラス生成
		S02_0106Da dataAccess = new S02_0106Da();
		//更新時の情報の値を設定する
		if (receiptEntity.registerType == signUpNormalType)
		{
			//通常予約の場合
			CommonLogic.setTableCommonInfo(DbShoriKbn.Insert, this.Name, (iEntity)(clsTYoyakuInfoMemoEntity.EntityData(0)));
		}
		else
		{
			//WT、リクエストの場合
			CommonLogic.setTableCommonInfo(DbShoriKbn.Insert, this.Name, (iEntity)(clsTWtRequestInfoMemoEntity.EntityData(0)));
		}
		//必須項目のチェック
		if (this.inputDataAreaError() == true)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			return;
		}
		// 相関チェック
		if (this.checkInsertItems() == true)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_024", "登録対象");
			return;
		}
		//処理実施確認
		if (CommonProcess.createFactoryMsg().messageDisp("Q90_001", "登録") == MsgBoxResult.Cancel)
		{
			return;
		}
		else
		{
			try
			{
				//データ登録処理の実施
				if (receiptEntity.registerType == signUpNormalType)
				{
					//通常予約の場合
					object with_1 = clsTYoyakuInfoMemoEntity.EntityData(0);
					//予約区分
					paramInfoList.Add("YoyakuKbn", with_1.yoyakuKbn.Value);
					//予約ＮＯ
					paramInfoList.Add("YoyakuNo", with_1.yoyakuNo.Value);
					//登録内容
					paramInfoList.Add("RegisterType", receiptEntity.registerType);
					//SELECT用DBアクセス
					DataTable dataTable = dataAccess.getData(S02_0106Da.accessType.getMaxEdban, paramInfoList);
					with_1.edaban.Value = (Integer?)(dataTable.Rows(0).Item("EDABAN").ToString());
					//枝番
					paramInfoList.Add("Edaban", with_1.edaban.Value);
					//メモ区分
					paramInfoList.Add("MemoKbn", with_1.memoKbn.Value);
					//メモ分類
					paramInfoList.Add("MemoBunrui", with_1.memoBunrui.Value);
					//内容
					paramInfoList.Add("Naiyo", with_1.naiyo.Value);
					//降車ヶ所コード
					//降車ヶ所枝番
					if (this.rdoShareJiko.Checked == true && this.rdoKousyaKashoContact.Checked == true)
					{
						// 該当する場合のみ設定
						paramInfoList.Add("KoshakashoCd", with_1.KoshakashoCd.Value);
						paramInfoList.Add("KoshakashoEdaban", with_1.KoshakashoEdaban.Value);
					}
					else
					{
						// 空をセット
						paramInfoList.Add("KoshakashoCd", string.Empty);
						paramInfoList.Add("KoshakashoEdaban", string.Empty);
					}
					//スタッフ共有対象
					if (this.rdoShareJiko.Checked == true && this.rdoStaffShare.Checked == true)
					{
						// 該当する場合のみ設定
						paramInfoList.Add("StaffShareTaisyo", with_1.StaffShareTaisyo.Value);
					}
					else
					{
						// 空をセット
						paramInfoList.Add("StaffShareTaisyo", string.Empty);
					}

					//確認済み
					paramInfoList.Add("ZumiFlg", with_1.zumiFlg.Value);
					//削除日
					paramInfoList.Add("DeleteDate", NotDeleteDayValue);
					//システム登録日
					paramInfoList.Add("SystemEntryDay", with_1.systemUpdateDay.Value);
					//システム登録ＰＧＭＩＤ
					paramInfoList.Add("SystemEntryPgmid", with_1.systemUpdatePgmid.Value);
					//システム登録者コード
					paramInfoList.Add("SystemEntryPersonCd", with_1.systemEntryPersonCd.Value);
					//システム更新日
					paramInfoList.Add("SystemUpdateDay", with_1.systemUpdateDay.Value);
					//システム更新ＰＧＭＩＤ
					paramInfoList.Add("SystemUpdatePgmid", with_1.systemUpdatePgmid.Value);
					//システム更新者コード
					paramInfoList.Add("SystemUpdatePersonCd", with_1.SystemUpdatePersonCd.Value);
					// 複数選択用の場合のデータセット
					paramInfoList.Add("ListSelected", getListSelectedRowData());

					//Insertの実施
					returnValue = System.Convert.ToInt32(dataAccess.executeUpdate(S02_0106Da.accessType.executeInsert, paramInfoList));
					logmsg[0] = System.Convert.ToString(this.txtNaiyo.Text);
					if (returnValue > 0)
					{
						receiptEntity.edaban = System.Convert.ToInt32(with_1.edaban.Value);
						//正常終了メッセージ表示
						CommonProcess.createFactoryMsg().messageDisp("I90_002", "登録");
						//ログ出力(操作ログ)
						createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.entry, setTitle, logmsg);
						//'画面の編集モードを”更新（削除）”に変更する。
						//Call setUpdateStatus()
						//'【各画面毎】差分チェック用情報の格納
						//Call Me.saveCheckDifferent()
						// 画面を終了する
						base.closeFormFlg = false;
						this.Close();

					}
					else
					{
						//異常終了メッセージ表示
						CommonProcess.createFactoryMsg().messageDisp("E90_025", "処理、登録");
						//ログ出力(エラーログ)
						createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, setTitle, logmsg);
					}
					//各種処理情報を格納する
					this.SetResearchFlg(true);
				}
				else
				{
					//WT、リクエストの場合
					object with_2 = clsTWtRequestInfoMemoEntity.EntityData(0);
					//管理区分
					paramInfoList.Add("ManagementKbn", with_2.ManagementKbn.Value);
					//管理ＮＯ
					paramInfoList.Add("ManagementNo", with_2.ManagementNo.Value);
					//登録内容
					paramInfoList.Add("RegisterType", receiptEntity.registerType);
					//SELECT用DBアクセス
					DataTable dataTable = dataAccess.getData(S02_0106Da.accessType.getMaxEdban, paramInfoList);
					with_2.Edaban.Value = (Integer?)(dataTable.Rows(0).Item("EDABAN").ToString());
					//枝番
					paramInfoList.Add("Edaban", with_2.Edaban.Value);
					//メモ区分
					paramInfoList.Add("MemoKbn", with_2.MemoKbn.Value);
					//メモ分類
					paramInfoList.Add("MemoBunrui", with_2.MemoBunrui.Value);
					//内容
					paramInfoList.Add("Naiyo", with_2.Naiyo.Value);
					//降車ヶ所コード、降車ヶ所枝番
					// 空をセット
					paramInfoList.Add("KoshakashoCd", string.Empty);
					paramInfoList.Add("KoshakashoEdaban", string.Empty);
					//スタッフ共有対象
					// 空をセット
					paramInfoList.Add("StaffShareTaisyo", string.Empty);

					//確認済み
					paramInfoList.Add("ZumiFlg", with_2.ZumiFlg.Value);
					//削除日
					paramInfoList.Add("DeleteDate", NotDeleteDayValue);
					//システム登録日
					paramInfoList.Add("SystemEntryDay", with_2.SystemUpdateDay.Value);
					//システム登録ＰＧＭＩＤ
					paramInfoList.Add("SystemEntryPgmid", with_2.SystemUpdatePgmid.Value);
					//システム登録者コード
					paramInfoList.Add("SystemEntryPersonCd", with_2.SystemEntryPersonCd.Value);
					//システム更新日
					paramInfoList.Add("SystemUpdateDay", with_2.SystemUpdateDay.Value);
					//システム更新ＰＧＭＩＤ
					paramInfoList.Add("SystemUpdatePgmid", with_2.SystemUpdatePgmid.Value);
					//システム更新者コード
					paramInfoList.Add("SystemUpdatePersonCd", with_2.SystemUpdatePersonCd.Value);

					//Insertの実施
					returnValue = System.Convert.ToInt32(dataAccess.executeUpdate(S02_0106Da.accessType.executeInsert, paramInfoList));
					logmsg[0] = System.Convert.ToString(this.txtNaiyo.Text);
					if (returnValue > 0)
					{
						receiptEntity.edaban = System.Convert.ToInt32(with_2.Edaban.Value);
						//正常終了メッセージ表示
						CommonProcess.createFactoryMsg().messageDisp("I90_002", "登録");
						//ログ出力(操作ログ)
						createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.entry, setTitle, logmsg);
						//'画面の編集モードを”更新（削除）”に変更する。
						//Call setUpdateStatus()
						//'【各画面毎】差分チェック用情報の格納
						//Call Me.saveCheckDifferent()
						// 画面を終了する
						base.closeFormFlg = false;
						this.Close();

					}
					else
					{
						//異常終了メッセージ表示
						CommonProcess.createFactoryMsg().messageDisp("E90_025", "処理、登録");
						//ログ出力(エラーログ)
						createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, setTitle, logmsg);
					}
					//各種処理情報を格納する
					this.SetResearchFlg(true);
				}
				//Oracleの例外
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
		}
	}
	#endregion

	#region ラジオの変更

	/// <summary>
	/// 入金確認
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void rdoNyuukinKakunin_CheckedChanged(object sender, EventArgs e)
	{
		this.chkKakuninAlready.Enabled = this.rdoNyuukinKakunin.Checked;
		if (editMode == signUpMode && F10Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
		if (editMode == updateMode && F11Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
	}

	/// <summary>
	/// 予約確認
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void rdoYoyakuKakunin_CheckedChanged(object sender, EventArgs e)
	{
		if (editMode == signUpMode && F10Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
		if (editMode == updateMode && F11Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
	}

	/// <summary>
	/// ＮＯＳＨＯＷ確認
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void rdoNoShowKakunin_CheckedChanged(object sender, EventArgs e)
	{
		if (editMode == signUpMode && F10Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
		if (editMode == updateMode && F11Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
	}

	/// <summary>
	/// 催行中止連絡
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void rdoSaikouChuusiContact_CheckedChanged(object sender, EventArgs e)
	{
		if (editMode == signUpMode && F10Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
		if (editMode == updateMode && F11Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
	}

	/// <summary>
	/// ダブリチェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void rdoDuburiCheck_CheckedChanged(object sender, EventArgs e)
	{
		if (editMode == signUpMode && F10Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
		if (editMode == updateMode && F11Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
	}

	/// <summary>
	/// 共有事項ラジオボタンチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void rdoShareJiko_CheckedChanged(object sender, EventArgs e)
	{
		this.rdoKousyaKashoContact.Enabled = this.rdoShareJiko.Checked;
		this.rdoStaffShare.Enabled = this.rdoShareJiko.Checked;
		this.rdoNyuukinKakunin.Enabled = this.rdoBusinessHistory.Checked;
		this.rdoYoyakuKakunin.Enabled = this.rdoBusinessHistory.Checked;
		this.rdoNoShowKakunin.Enabled = this.rdoBusinessHistory.Checked;
		this.rdoSaikouChuusiContact.Enabled = this.rdoBusinessHistory.Checked;
		this.rdoDuburiCheck.Enabled = this.rdoBusinessHistory.Checked;
		// 対象一覧を初期化
		this.clearTaisyoList();
		// 確認済みチェックボックス
		this.chkKakuninAlready.Enabled = this.rdoNyuukinKakunin.Checked;
		// 分類内容のラジオボタンを初期化
		this.clearBunruiRadioButton();
		this.rdoMemo.Enabled = this.rdoShareJiko.Checked;
		this.rdoMemo.Checked = this.rdoShareJiko.Checked;
		if (editMode == updateMode)
		{
			disableBunruiRadioButton();
		}
	}

	/// <summary>
	/// メモラジオボタンチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void rdoMemo_CheckedChanged(object sender, EventArgs e)
	{
		if (editMode == signUpMode && getSelectedRowData() IsNot null)
		{
			if (CommonProcess.createFactoryMsg().messageDisp("W90_003", "変更") == MsgBoxResult.Cancel)
			{
				rdoMemo.Checked = false;
				CheckMemoBunrui(System.Convert.ToString(clsTYoyakuInfoMemoEntity.EntityData(0).memoBunrui.ZenkaiValue), false);
				return;
			}
		}
		if (!this.rdoMemo.Checked)
		{
			return;
		}
		//空のテーブルを作成する
		DataTable dt = this.createTaisyoDataTable();
		this.grdTaisyo.DataSource = dt;
		this.grdTaisyo.Enabled = false;
		if (editMode == signUpMode && F10Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
		if (editMode == updateMode && F11Key_Enabled == false)
		{
			F10Key_Enabled = true;
		}
		//各種処理情報を格納する
		this.saveCheckDifferent();
	}

	/// <summary>
	/// 降車箇所連絡ラジオボタンチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void rdoKousyaKashoContact_CheckedChanged(object sender, EventArgs e)
	{
		if (editMode == signUpMode && getSelectedRowData() IsNot null)
		{
			if (CommonProcess.createFactoryMsg().messageDisp("W90_003", "変更") == MsgBoxResult.Cancel)
			{
				rdoKousyaKashoContact.Checked = false;
				CheckMemoBunrui(System.Convert.ToString(clsTYoyakuInfoMemoEntity.EntityData(0).memoBunrui.ZenkaiValue), false);
				return;
			}
		}
		if (!this.rdoKousyaKashoContact.Checked)
		{
			return;
		}
		//降車箇所連絡の情報を設定する
		this.setData(DbTableKbn.getPlaceReportData);
		//各種処理情報を格納する
		this.SetResearchFlg(false);
		//【各画面毎】差分チェック用情報の格納
		this.saveCheckDifferent();
	}

	/// <summary>
	/// スタッフ共有ラジオボタンチェンジイベント
	/// </summary>
	/// <param name="sender">イベント送信元</param>
	/// <param name="e">イベントデータ</param>
	private void rdoStaffShare_CheckedChanged(object sender, EventArgs e)
	{
		if (editMode == signUpMode && getSelectedRowData() IsNot null)
		{
			if (CommonProcess.createFactoryMsg().messageDisp("W90_003", "変更") == MsgBoxResult.Cancel)
			{
				rdoStaffShare.Checked = false;
				CheckMemoBunrui(System.Convert.ToString(clsTYoyakuInfoMemoEntity.EntityData(0).memoBunrui.ZenkaiValue), false);
				return;
			}
		}
		if (!this.rdoStaffShare.Checked)
		{
			return;
		}
		//降車箇所連絡の情報を設定する
		this.setData(DbTableKbn.getStaffSharingData);
		//【各画面毎】差分チェック用情報の格納
		this.saveCheckDifferent();
	}

	/// <summary>
	/// 業務履歴 change
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void rdoBusinessHistory_CheckedChanged(object sender, EventArgs e)
	{
		rdoNyuukinKakunin.Checked = rdoBusinessHistory.Checked;
	}

	#endregion

	#region DB 選択する
	/// <summary>
	/// 降車箇所連絡の情報を設定する
	/// </summary>
	private void setData(DbTableKbn kbn)
	{
		//戻り値
		DataTable returnValue = null;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		//DAクラス作成、パラメータ設定、選択処理実施までを実装
		//DataAccessクラス生成
		S02_0106Da dataAccess = new S02_0106Da();
		//パラメータ設定
		object with_1 = clsTYoyakuInfoMemoEntity.EntityData(0);
		//予約区分
		paramInfoList.Add("YoyakuKbn", with_1.YoyakuKbn.Value);
		//予約ＮＯ
		paramInfoList.Add("YoyakuNo", with_1.YoyakuNo.Value);
		//枝番
		paramInfoList.Add("Edaban", with_1.Edaban.Value);
		try
		{
			if (DbTableKbn.getUpdateData.Equals(kbn))
			{
				//一覧結果取得検索
				returnValue = dataAccess.getData(S02_0106Da.accessType.getBusinessHistoryData, paramInfoList);
			}
			if (DbTableKbn.getPlaceReportData.Equals(kbn))
			{
				returnValue = dataAccess.getData(S02_0106Da.accessType.getPlaceReportData, paramInfoList);
			}
			if (DbTableKbn.getStaffSharingData.Equals(kbn))
			{
				returnValue = dataAccess.getData(S02_0106Da.accessType.getStaffSharingData);
			}
			grdTaisyo.DataSource = returnValue;
			grdTaisyo.Refresh();
			if (returnValue.Rows.Count == 0)
			{
				//検索結果0件時メッセージを表示する
				createFactoryMsg().messageDisp("E90_019");
				//「F4:削除」、「F10：登録」、「F11：更新」ボタンを非活性にする
				this.disableBottomButton();
			}
			else
			{
				//アクティブ「F4:削除」、「F10：登録」、「F11：更新」
				this.enableBottomButton();
			}
			//編集モードでgrdTaisyoのステータスを設定する
			setActiveEditArea();
			//ログ出力
			createFactoryLog().logOutput(LogKindType.operationLog, ProcessKindType.search, setTitle);
		}
		catch (Exception ex)
		{
			createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, this.setTitle, "ex.Message;" + ex.Message, "ex.Source;" + ex.Source, "ex.StackTrace;" + ex.StackTrace);
			//ログ出力
			createFactoryLog().logOutput(LogKindType.errorLog, ProcessKindType.search, setTitle);
			throw;
		}
	}
	#endregion

	#region フッタ
	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	private void setButtonInitiarize()
	{

		this.F1Key_Visible = false;
		this.F2Key_Visible = true;
		this.F3Key_Visible = false;
		this.F4Key_Visible = true;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		this.F7Key_Visible = false;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = true;
		this.F11Key_Visible = true;
		this.F12Key_Visible = false;

		// ボタン位置調整
		this.F2Key_Location_X = 10;
		this.F4Key_Location_X = F2Key_Location_X + (F2Key_Width + 10) * 2;
		this.F10Key_Location_X = 600;
		this.F11Key_Location_X = System.Convert.ToInt32(F10Key_Location_X + F10Key_Width) + 10;

	}
	#endregion

	#endregion

	#region メソッド
	/// <summary>
	/// オフ分類,区分 ラジオの設定
	/// </summary>
	private void clearBunruiRadioButton()
	{
		foreach (Control control in grxBunrui.Controls)
		{
			if (!ReferenceEquals(control.GetType(), typeof(RadioButtonEx)))
			{
				continue;
			}
			RadioButton chk = (RadioButtonEx)control;
			chk.Checked = false;
		}
	}

	/// <summary>
	/// オフ分類,区分 ラジオの設定
	/// </summary>
	private void disableBunruiRadioButton()
	{
		foreach (Control control in grxBunrui.Controls)
		{
			if (!ReferenceEquals(control.GetType(), typeof(RadioButtonEx)))
			{
				continue;
			}
			RadioButton chk = (RadioButtonEx)control;
			chk.Checked = false;
			chk.Enabled = false;
		}
	}

	/// <summary>
	/// 空データテーブルを選択対象グリッドに設定する
	/// </summary>
	private void clearTaisyoList()
	{
		DataTable dt = this.createTaisyoDataTable();
		this.grdTaisyo.DataSource = dt;
		this.grdTaisyo.Enabled = false;
	}

	/// <summary>
	/// 空のテーブルを作成する
	/// </summary>
	/// <returns></returns>
	private DataTable createTaisyoDataTable()
	{
		DataTable dt = new DataTable();
		dt.Columns.Add("COLSELECTION", typeof(bool));
		dt.Columns.Add("COLTAISYO", typeof(string));
		dt.Columns.Add("COLTAISYO_CODE", typeof(string));
		dt.Columns.Add("COLKIND", typeof(string));
		dt.Columns.Add("COLKIND_CODE", typeof(string));
		return dt;
	}

	/// <summary>
	/// 選択行のデータを取得、応答
	/// </summary>
	/// <remarks></remarks>
	private DataTable getListSelectedRowData()
	{

		System.Object dataTable = createTaisyoDataTable();

		if (ReferenceEquals(grdTaisyo.DataSource, null))
		{
			return null;
		}

		foreach (DataRow dataRow in ((DataTable)grdTaisyo.DataSource).Rows)
		{
			string tmpstr = System.Convert.ToString(dataRow["COLSELECTION"].ToString());

			if (System.Convert.ToBoolean(tmpstr))
			{
				object newRow = dataTable.NewRow();
				newRow.ItemArray = (object[])dataRow.ItemArray.Clone;
				dataTable.Rows.Add(newRow);
			}
		}

		return dataTable;

	}

	/// <summary>
	/// Set after update
	/// </summary>
	private void setUpdateStatus()
	{
		this.editMode = updateMode;
		//区分の設定
		this.setRadioInitiarize();
		//情報を設定する [更新（削除)]
		this.setUpdateData();
		// フッタボタンの設定
		this.setActiveButtonByEditMode();
		//フォーカス設定
		this.txtNaiyo.Select();
	}

	/// <summary>
	/// すべてのラジオボタンを無効にする、F10、F4、F11
	/// </summary>
	private void setDisableAll()
	{
		foreach (Control control in grxBunrui.Controls)
		{
			if (!ReferenceEquals(control.GetType(), typeof(RadioButtonEx)))
			{
				continue;
			}
			RadioButton chk = (RadioButtonEx)control;
			chk.Enabled = false;
		}
		foreach (Control control in grxKbn.Controls)
		{
			if (!ReferenceEquals(control.GetType(), typeof(RadioButtonEx)))
			{
				continue;
			}
			RadioButton chk = (RadioButtonEx)control;
			chk.Enabled = false;
		}
		this.txtNaiyo.Enabled = false;
		this.grdTaisyo.AllowEditing = false;
		this.F10Key_Enabled = false;
		this.F11Key_Enabled = false;
		this.F4Key_Enabled = false;
		this.chkKakuninAlready.Enabled = false;
	}

	/// <summary>
	/// フォーム起動時の独自処理
	/// </summary>
	protected override void StartupOrgProc()
	{

		// 差分チェック初期状態セット
		base.closeFormFlg = false;

		//画面表示時の初期設定
		this.setControlInitiarize();

	}

	/// <summary>
	/// 画面表示時の初期設定
	/// </summary>
	private void setControlInitiarize()
	{
		//ベースフォームの設定
		grdTaisyo.DataSource = this.createTaisyoDataTable();
		grdTaisyo.Refresh();
		//検索結果の格納先をクリアする
		this.SetResearchFlg(false);
		//フッターボタン制御
		this.setButtonInitiarize();
		//画面間パラメータを格納する
		this.getTransferData();
		//フッターボタン制御
		this.setActiveButtonByEditMode();
		//ラジオの設定
		this.setRadioInitiarize();
		//表示データ取得
		this.getUpdateData();
		//フォーカス設定
		this.txtNaiyo.Select();
		this.chkKakuninAlready.Enabled = false;
		//【各画面毎】差分チェック用情報の格納
		this.saveCheckDifferent();
	}

	/// <summary>
	/// 【各画面毎】差分チェック用情報の格納
	/// </summary>
	private void saveCheckDifferent()
	{
		if (receiptEntity.registerType == signUpNormalType)
		{
			//通常予約の場合
			object with_1 = clsTYoyakuInfoMemoEntity.EntityData(0);
			if (rdoShareJiko.Checked == true)
			{
				with_1.MemoKbn.ZenkaiValue = NoteClassification.Shared_Items;
			}
			if (rdoBusinessHistory.Checked == true)
			{
				with_1.MemoKbn.ZenkaiValue = NoteClassification.Business_History;
			}
			if (rdoMemo.Checked == true)
			{
				with_1.MemoBunrui.ZenkaiValue = MemoClassification.memo;
			}
			if (rdoKousyaKashoContact.Checked == true)
			{
				with_1.MemoBunrui.ZenkaiValue = MemoClassification.disembarkation_Place_Report;
			}
			if (rdoStaffShare.Checked == true)
			{
				with_1.MemoBunrui.ZenkaiValue = MemoClassification.staff_Sharing;
			}
			if (rdoNyuukinKakunin.Checked == true)
			{
				with_1.MemoBunrui.ZenkaiValue = MemoClassification.payment_Contact;
			}
			if (rdoYoyakuKakunin.Checked == true)
			{
				with_1.MemoBunrui.ZenkaiValue = MemoClassification.booking_Confirmation;
			}
			if (rdoNoShowKakunin.Checked == true)
			{
				with_1.MemoBunrui.ZenkaiValue = MemoClassification.NOSHOW_Confirmation;
			}
			if (rdoSaikouChuusiContact.Checked == true)
			{
				with_1.MemoBunrui.ZenkaiValue = MemoClassification.cancellation_Of_Liaison;
			}
			if (rdoDuburiCheck.Checked == true)
			{
				with_1.MemoBunrui.ZenkaiValue = MemoClassification.dubricity_Check;
			}
			with_1.Naiyo.ZenkaiValue = this.txtNaiyo.Text;
			DataTable dataTable = (DataTable)grdTaisyo.DataSource;
			if (ReferenceEquals(dataTable, null))
			{
				return;
			}
			if (dataTable.Rows.Count > 0)
			{
				foreach (DataRow row in dataTable.Rows)
				{
					if (row.Item("COLSELECTION").ToString() != unCheckedValue)
					{
						if (!Information.IsDBNull(row.Item("COLTAISYO_CODE")))
						{
							object koshakashoCd = row.Item("COLTAISYO_CODE").ToString();
							with_1.KoshakashoCd.ZenkaiValue = koshakashoCd;
							with_1.KoshakashoEdaban.ZenkaiValue = row.Item("COLKIND_CODE").ToString();
							with_1.StaffShareTaisyo.ZenkaiValue = koshakashoCd;
						}
					}
				}
			}
		}
		else
		{
			//WT、リクエストの場合
			object with_2 = clsTWtRequestInfoMemoEntity.EntityData(0);
			with_2.MemoKbn.ZenkaiValue = NoteClassification.Business_History;
			if (receiptEntity.registerType == signUpWTType)
			{
				with_2.MemoBunrui.ZenkaiValue = MemoClassification.weight;
				with_2.MemoBunrui.Value = MemoClassification.weight;
			}
			if (receiptEntity.registerType == signUpRequestType)
			{
				with_2.MemoBunrui.ZenkaiValue = MemoClassification.request;
				with_2.MemoBunrui.Value = MemoClassification.request;
			}
			with_2.Naiyo.ZenkaiValue = this.txtNaiyo.Text;
		}
	}

	/// <summary>
	/// 表示データ取得
	/// </summary>
	private void getUpdateData()
	{
		if (editMode == updateMode)
		{
			//情報を設定する [更新（削除)]
			this.setUpdateData();
		}
	}

	/// <summary>
	///  情報を設定する [更新（削除)]
	/// </summary>
	private void setUpdateData()
	{
		string[] logmsg = new string[2];
		//戻り値
		DataTable returnValue = null;
		//DBパラメータ
		Hashtable paramInfoList = new Hashtable();
		//DAクラス作成、パラメータ設定、選択処理実施までを実装
		//DataAccessクラス生成
		S02_0106Da dataAccess = new S02_0106Da();
		//パラメータ設定
		if (receiptEntity.registerType == signUpNormalType)
		{
			//通常予約の場合
			object with_1 = clsTYoyakuInfoMemoEntity.EntityData(0);
			paramInfoList.Add("YoyakuKbn", with_1.YoyakuKbn.Value);
			paramInfoList.Add("YoyakuNo", with_1.YoyakuNo.Value);
			paramInfoList.Add("Edaban", with_1.Edaban.Value);
			paramInfoList.Add("RegisterType", receiptEntity.registerType);
			try
			{
				returnValue = dataAccess.getData(S02_0106Da.accessType.getBusinessHistoryData, paramInfoList);
				//取得データが0件の場合 ： 検索結果0件時メッセージを表示する
				if (returnValue.Rows.Count == 0)
				{
					CommonProcess.createFactoryMsg().messageDisp("E90_019");
					//取得データが0件の場合
					this.disableBottomButton();
				}
				else
				{
					this.enableBottomButton();
					this.txtNaiyo.Text = returnValue.Rows(0).Item("NAIYO").ToString();
					string memmoKbn = returnValue.Rows(0).Item("MEMO_KBN").ToString();
					string memmoBunrui = returnValue.Rows(0).Item("MEMO_BUNRUI").ToString();

					if (NoteClassification.Shared_Items == memmoKbn)
					{
						rdoShareJiko.Checked = true;
						CheckMemoBunrui(memmoBunrui, true);
						if (MemoClassification.memo != memmoBunrui)
						{
							grdTaisyo.DataSource = returnValue;
						}
						else
						{
							grdTaisyo.DataSource = this.createTaisyoDataTable();
						}
						grdTaisyo.Refresh();
					}
					else
					{
						if (NoteClassification.Business_History == memmoKbn)
						{
							rdoBusinessHistory.Checked = true;
							if (MemoClassification.payment_Contact == memmoBunrui)
							{
								rdoNyuukinKakunin.Checked = true;
								if (returnValue.Rows(0).Item("ZUMI_FLG").ToString() == verifyCode)
								{
									this.chkKakuninAlready.Checked = true;
									this.chkKakuninAlready.Enabled = false;
								}
							}
							if (MemoClassification.booking_Confirmation == memmoBunrui)
							{
								rdoYoyakuKakunin.Checked = true;
							}
							if (MemoClassification.NOSHOW_Confirmation == memmoBunrui)
							{
								rdoNoShowKakunin.Checked = true;
							}
							if (MemoClassification.cancellation_Of_Liaison == memmoBunrui)
							{
								rdoSaikouChuusiContact.Checked = true;
							}
							if (MemoClassification.dubricity_Check == memmoBunrui)
							{
								rdoDuburiCheck.Checked = true;
							}
						}
					}
					setActiveEditArea();
				}
				this.SetResearchFlg(true);
				logmsg[0] = System.Convert.ToString(this.txtNaiyo.Text);
				//ログ出力
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, setTitle, logmsg);
			}
			catch (Exception ex)
			{
				logmsg[0] = this.txtNaiyo.Text + "[" + ex.Message + "]";
				//ログ出力
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, setTitle, logmsg);
				throw;
			}
		}
		else
		{
			//WT､リクエストの場合
			object with_2 = clsTWtRequestInfoMemoEntity.EntityData(0);
			paramInfoList.Add("ManagementKbn", with_2.ManagementKbn.Value);
			paramInfoList.Add("ManagementNo", with_2.ManagementNo.Value);
			paramInfoList.Add("Edaban", with_2.Edaban.Value);
			paramInfoList.Add("RegisterType", receiptEntity.registerType);
			try
			{
				returnValue = dataAccess.getData(S02_0106Da.accessType.getBusinessHistoryData, paramInfoList);
				//取得データが0件の場合 ： 検索結果0件時メッセージを表示する
				if (returnValue.Rows.Count == 0)
				{
					CommonProcess.createFactoryMsg().messageDisp("E90_019");
					//取得データが0件の場合
					this.disableBottomButton();
				}
				else
				{
					this.enableBottomButton();
					this.txtNaiyo.Text = returnValue.Rows(0).Item("NAIYO").ToString();
					string memmoKbn = returnValue.Rows(0).Item("MEMO_KBN").ToString();
					string memmoBunrui = returnValue.Rows(0).Item("MEMO_BUNRUI").ToString();

					if (NoteClassification.Shared_Items == memmoKbn)
					{
						rdoShareJiko.Checked = true;
						CheckMemoBunrui(memmoBunrui, true);
						if (MemoClassification.memo != memmoBunrui)
						{
							grdTaisyo.DataSource = returnValue;
						}
						else
						{
							grdTaisyo.DataSource = this.createTaisyoDataTable();
						}
						grdTaisyo.Refresh();
					}
					else
					{
						if (NoteClassification.Business_History == memmoKbn)
						{
							rdoBusinessHistory.Checked = true;
							if (MemoClassification.payment_Contact == memmoBunrui)
							{
								rdoNyuukinKakunin.Checked = true;
								if (returnValue.Rows(0).Item("ZUMI_FLG").ToString() == verifyCode)
								{
									this.chkKakuninAlready.Checked = true;
									this.chkKakuninAlready.Enabled = false;
								}
							}
							if (MemoClassification.booking_Confirmation == memmoBunrui)
							{
								rdoYoyakuKakunin.Checked = true;
							}
							if (MemoClassification.NOSHOW_Confirmation == memmoBunrui)
							{
								rdoNoShowKakunin.Checked = true;
							}
							if (MemoClassification.cancellation_Of_Liaison == memmoBunrui)
							{
								rdoSaikouChuusiContact.Checked = true;
							}
							if (MemoClassification.dubricity_Check == memmoBunrui)
							{
								rdoDuburiCheck.Checked = true;
							}
						}
					}
					setActiveEditArea();
				}
				this.SetResearchFlg(true);
				logmsg[0] = System.Convert.ToString(this.txtNaiyo.Text);
				//ログ出力
				createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.search, setTitle, logmsg);
			}
			catch (Exception ex)
			{
				logmsg[0] = this.txtNaiyo.Text + "[" + ex.Message + "]";
				//ログ出力
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.search, setTitle, logmsg);
				throw;
			}
		}
	}

	/// <summary>
	/// 編集モードでgrdTaisyoのステータスを設定する
	/// </summary>
	private void setActiveEditArea()
	{
		grdTaisyo.Enabled = true;
		grdTaisyo.AllowEditing = true;
		if (receiptEntity.registerType != signUpNormalType)
		{
			grdTaisyo.Cols("COLSELECTION").AllowEditing = false;
			grdTaisyo.Cols("COLTAISYO").AllowEditing = false;
			grdTaisyo.Cols("COLKIND").AllowEditing = false;
		}
		this.txtNaiyo.Select();
	}

	/// <summary>
	/// 「F4:削除」、「F10：登録」、「F11：更新」ボタンを非活性にする
	/// </summary>
	private void disableBottomButton()
	{
		this.F4Key_Enabled = false;
		this.F10Key_Enabled = false;
		this.F11Key_Enabled = false;
	}

	/// <summary>
	/// アクティブ「F4:削除」、「F10：登録」、「F11：更新」
	/// </summary>
	private void enableBottomButton()
	{
		if (this.editMode == updateMode)
		{
			this.F11Key_Enabled = true;
			this.F10Key_Enabled = false;
		}
		else
		{
			if (this.editMode == signUpMode)
			{
				this.F10Key_Enabled = true;
			}
		}
	}

	/// <summary>
	/// 遷移元画面からの画面間パラメータを格納する
	/// </summary>
	private void getTransferData()
	{
		if (receiptEntity.registerType == signUpNormalType)
		{
			//通常予約の場合
			object with_1 = this.clsTYoyakuInfoMemoEntity.EntityData(0);
			with_1.YoyakuKbn.Value = receiptEntity.yoyakuKbn;
			with_1.YoyakuNo.Value = receiptEntity.yoyakuNo;
			with_1.Edaban.Value = receiptEntity.edaban;
			with_1.YoyakuKbn.ZenkaiValue = receiptEntity.yoyakuKbn;
			with_1.YoyakuNo.ZenkaiValue = receiptEntity.yoyakuNo;
			with_1.Edaban.ZenkaiValue = receiptEntity.edaban;
		}
		else
		{
			//WT、リクエストの場合
			object with_2 = this.clsTWtRequestInfoMemoEntity.EntityData(0);
			with_2.ManagementKbn.Value = receiptEntity.yoyakuKbn;
			with_2.ManagementNo.Value = receiptEntity.yoyakuNo;
			with_2.Edaban.Value = receiptEntity.edaban;
			with_2.ManagementKbn.ZenkaiValue = receiptEntity.yoyakuKbn;
			with_2.ManagementNo.ZenkaiValue = receiptEntity.yoyakuNo;
			with_2.Edaban.ZenkaiValue = receiptEntity.edaban;
			if (receiptEntity.registerType == signUpWTType)
			{
				with_2.MemoBunrui.ZenkaiValue = MemoClassification.weight;
				with_2.MemoBunrui.Value = MemoClassification.weight;
			}
			if (receiptEntity.registerType == signUpRequestType)
			{
				with_2.MemoBunrui.ZenkaiValue = MemoClassification.request;
				with_2.MemoBunrui.Value = MemoClassification.request;
			}
		}
		editMode = System.Convert.ToString(receiptEntity.editMode);
	}

	/// <summary>
	/// 区分の設定
	/// </summary>
	private void setRadioInitiarize()
	{

		SetRadioLabel(rdoShareJiko, System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memokbn)), System.Convert.ToString(NoteClassification.Shared_Items));
		SetRadioLabel(rdoBusinessHistory, System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memokbn)), System.Convert.ToString(NoteClassification.Business_History));
		SetRadioLabel(rdoMemo, System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui)), System.Convert.ToString(MemoClassification.memo));
		SetRadioLabel(rdoKousyaKashoContact, System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui)), System.Convert.ToString(MemoClassification.disembarkation_Place_Report));
		SetRadioLabel(rdoStaffShare, System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui)), System.Convert.ToString(MemoClassification.staff_Sharing));
		SetRadioLabel(rdoNyuukinKakunin, System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui)), System.Convert.ToString(MemoClassification.payment_Contact));
		SetRadioLabel(rdoYoyakuKakunin, System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui)), System.Convert.ToString(MemoClassification.booking_Confirmation));
		SetRadioLabel(rdoNoShowKakunin, System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui)), System.Convert.ToString(MemoClassification.NOSHOW_Confirmation));
		SetRadioLabel(rdoSaikouChuusiContact, System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui)), System.Convert.ToString(MemoClassification.cancellation_Of_Liaison));
		SetRadioLabel(rdoDuburiCheck, System.Convert.ToString(CommonType_MojiColValue.CdBunruiType_Value(FixedCd.CdBunruiType.memobunrui)), System.Convert.ToString(MemoClassification.dubricity_Check));

		if (this.editMode == signUpMode)
		{
			if (receiptEntity.registerType == signUpNormalType)
			{
				this.rdoShareJiko.Checked = true;
				this.rdoMemo.Checked = true;
			}
			else
			{
				this.rdoBusinessHistory.Checked = true;
				this.rdoBusinessHistory.Enabled = false;
				this.rdoShareJiko.Checked = false;
				this.rdoShareJiko.Enabled = false;
				disableBunruiRadioButton();
			}
		}
		if (this.editMode == updateMode)
		{
			this.rdoBusinessHistory.Enabled = false;
			this.rdoShareJiko.Enabled = false;
			this.disableBunruiRadioButton();
		}
	}

	/// <summary>
	/// ラジオやラジオを聞く
	/// </summary>
	/// <param name="cdBunrui"></param>
	/// <param name="codeValue"></param>
	private void SetRadioLabel(RadioButtonEx rdo, string cdBunrui, string codeValue)
	{
		Code_DA codeDA = new Code_DA();
		Hashtable paramList = new Hashtable();
		paramList.Add("codeBunrui", cdBunrui);
		paramList.Add("codeValue", codeValue);
		object data = codeDA.accessCodeMaster(Code_DA.accessType.getCodeDataByPrimaryKey, paramList);
		if (data IsNot null && data.Rows.Count > 0)
		{
			rdo.Text = if (!Information.IsDBNull(data.Rows(0)["CODE_NAME"]), data.Rows(0)["CODE_NAME"].ToString(), string.Empty);
		}
		else
		{
			rdo.Text = string.Empty;
		}
	}

	/// <summary>
	/// 編集モードでボタンのステータスを設定する
	/// </summary>
	private void setActiveButtonByEditMode()
	{
		if (this.editMode == signUpMode)
		{
			this.F10Key_Enabled = true;
			this.F11Key_Enabled = false;
			this.F4Key_Enabled = false;
		}
		else
		{
			this.F11Key_Enabled = true;
			this.F4Key_Enabled = true;
		}
	}

	#endregion

	#region メソッド

	/// <summary>
	/// 選択 change
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void grdTaisyo_CellChecked(object sender, C1.Win.C1FlexGrid.RowColEventArgs e)
	{
		int roww = System.Convert.ToInt32(e.Row - 1);
		DataTable dataTable = (DataTable)grdTaisyo.DataSource;
		DataRow dataRow = dataTable.Rows(roww);
		string value = System.Convert.ToString(dataRow.Item("COLSELECTION").ToString());
		//If value = trueValue Then
		if (value == checkedValue)
		{
			dataTable.Rows(roww).Item[0] = checkedValue;
		}
		else
		{
			dataTable.Rows(roww).Item[0] = unCheckedValue;
		}
	}

	/// <summary>
	/// データ入力範囲チェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	/// <remarks></remarks>
	private bool inputDataAreaError()
	{
		bool returnValue = false;
		if (string.IsNullOrEmpty(System.Convert.ToString(this.txtNaiyo.Text)))
		{
			this.txtNaiyo.ExistError = true;
			this.txtNaiyo.Select();
			returnValue = true;
		}
		else
		{
			txtNaiyo.ExistError = false;
		}
		DataTable dataTable = (DataTable)grdTaisyo.DataSource;
		if (ReferenceEquals(dataTable, null))
		{
			return false;
		}
		if (dataTable.Rows.Count > 0)
		{
			int count = 0;
			foreach (DataRow row in dataTable.Rows)
			{
				if (row.Item("COLSELECTION").ToString() != unCheckedValue)
				{
					count++;
				}
			}
			if (count == 0)
			{
				return true;
			}
		}
		return returnValue;
	}

	#endregion

	#region 画面->エンティティ
	/// <summary>
	/// 更新対象項目をエンティティにセット
	/// </summary>
	protected override void setEntityDataValue()
	{
		if (receiptEntity.registerType == signUpNormalType)
		{
			//通常予約の場合
			//エンティティの初期化
			this.clsTYoyakuInfoMemoEntity.clear(0, clearType.value);
			this.clsTYoyakuInfoMemoEntity.clear(0, clearType.errorInfo);
			DisplayDataToEntity(this.clsTYoyakuInfoMemoEntity.EntityData(0));
		}
		else
		{
			//WT、リクエストの場合
			//エンティティの初期化
			this.clsTWtRequestInfoMemoEntity.clear(0, clearType.value);
			this.clsTWtRequestInfoMemoEntity.clear(0, clearType.errorInfo);
			DisplayDataToEntity(this.clsTWtRequestInfoMemoEntity.EntityData(0));
		}
	}
	#endregion

	#region エンティティ操作系
	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent">行定表連結記号エンティティ</param>
	private void DisplayDataToEntity(object ent)
	{
		getTransferData();
		if (receiptEntity.registerType == signUpNormalType)
		{
			//通常予約の場合
			if (typeof(TYoyakuInfoMemoEntityEx).Equals(ent.GetType()))
			{
				TYoyakuInfoMemoEntityEx with_1 = ((TYoyakuInfoMemoEntityEx)ent);
				//画面のデータをエンティティに設定する処理を実装
				if (rdoShareJiko.Checked == true)
				{
					with_1.MemoKbn.Value = NoteClassification.Shared_Items;
				}
				if (rdoBusinessHistory.Checked == true)
				{
					with_1.MemoKbn.Value = NoteClassification.Business_History;
				}
				if (rdoMemo.Checked == true)
				{
					with_1.MemoBunrui.Value = MemoClassification.memo;
				}
				if (rdoKousyaKashoContact.Checked == true)
				{
					with_1.MemoBunrui.Value = MemoClassification.disembarkation_Place_Report;
				}
				if (rdoStaffShare.Checked == true)
				{
					with_1.MemoBunrui.Value = MemoClassification.staff_Sharing;
				}
				if (rdoNyuukinKakunin.Checked == true)
				{
					with_1.MemoBunrui.Value = MemoClassification.payment_Contact;
				}
				if (rdoYoyakuKakunin.Checked == true)
				{
					with_1.MemoBunrui.Value = MemoClassification.booking_Confirmation;
				}
				if (rdoNoShowKakunin.Checked == true)
				{
					with_1.MemoBunrui.Value = MemoClassification.NOSHOW_Confirmation;
				}
				if (rdoSaikouChuusiContact.Checked == true)
				{
					with_1.MemoBunrui.Value = MemoClassification.cancellation_Of_Liaison;
				}
				if (rdoDuburiCheck.Checked == true)
				{
					with_1.MemoBunrui.Value = MemoClassification.dubricity_Check;
				}
				with_1.Naiyo.Value = this.txtNaiyo.Text;
				DataTable dataTable = (DataTable)grdTaisyo.DataSource;
				if (dataTable.Rows.Count > 0)
				{
					foreach (DataRow row in dataTable.Rows)
					{
						if (editMode == signUpMode)
						{
							if (row.Item("COLSELECTION").ToString() != unCheckedValue)
							{
								object koshakashoCd = row.Item("COLTAISYO_CODE").ToString();
								with_1.KoshakashoCd.Value = koshakashoCd;
								with_1.KoshakashoEdaban.Value = row.Item("COLKIND_CODE").ToString();
								with_1.StaffShareTaisyo.Value = koshakashoCd;
							}
						}
					}
				}
				if (chkKakuninAlready.Checked == true && chkKakuninAlready.Enabled == true)
				{
					with_1.ZumiFlg.Value = verifyCode;
				}
				else
				{
					with_1.ZumiFlg.Value = string.Empty;
				}
			}
		}
		else
		{
			//WT、リクエストの場合
			if (typeof(TWtRequestInfoMemoEntityEx).Equals(ent.GetType()))
			{
				TWtRequestInfoMemoEntityEx with_2 = ((TWtRequestInfoMemoEntityEx)ent);
				//画面のデータをエンティティに設定する処理を実装
				with_2.MemoKbn.Value = NoteClassification.Business_History;
				if (receiptEntity.registerType == signUpWTType)
				{
					with_2.MemoBunrui.ZenkaiValue = MemoClassification.weight;
					with_2.MemoBunrui.Value = MemoClassification.weight;
				}
				if (receiptEntity.registerType == signUpRequestType)
				{
					with_2.MemoBunrui.ZenkaiValue = MemoClassification.request;
					with_2.MemoBunrui.Value = MemoClassification.request;
				}
				with_2.Naiyo.Value = this.txtNaiyo.Text;
				with_2.ZumiFlg.Value = string.Empty;
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
			if (dataRow["COLSELECTION"].ToString() == "1")
			{
				return dataRow;
			}
		}
		return null;
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
	#endregion

	#region [登録処理](登録処理入力チェック)
	/// <summary>
	/// [登録処理](登録処理入力チェック)
	/// </summary>
	/// <returns></returns>
	protected override bool checkInsertItems()
	{
		//区分 ：  共有事項が選択されている
		if (rdoShareJiko.Checked == true)
		{
			//種別 ： 降車ヶ所連絡またはスタッフ共有が選択されている
			if (rdoKousyaKashoContact.Checked == true || rdoStaffShare.Checked == true)
			{
				DataTable dataTable = (DataTable)grdTaisyo.DataSource;
				if (dataTable.Rows.Count > 0)
				{
					int count = 0;
					foreach (DataRow row in dataTable.Rows)
					{
						if (row.Item("COLSELECTION").ToString() != unCheckedValue)
						{
							count++;
						}
					}
					if (count == 0)
					{
						return true;
					}
				}
			}
		}
		return false;
	}
	#endregion

	#region 変更確認
	/// <summary>
	/// 差分チェック
	/// </summary>
	protected override bool checkDifference()
	{
		//差分チェック
		if (receiptEntity.registerType == signUpNormalType)
		{
			//通常予約の場合
			return clsTYoyakuInfoMemoEntity.compare(0);
		}
		else
		{
			//WT、リクエストの場合
			return clsTWtRequestInfoMemoEntity.compare(0);
		}
	}

	#endregion

	#region Close時
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

	#region 引渡
	/// <summary>
	/// 引渡
	/// </summary>
	/// <returns></returns>
	public bool getReturnValue()
	{
		return reSearchFlag;
	}

	#endregion
}