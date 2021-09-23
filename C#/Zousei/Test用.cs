public class Test用
{
	public Test用()
	{
		// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
		itinerary_KoshakashoEntity = new EntityOperation(Of KoshakashoEntity);
		_processMode = ProcessMode.shinki;
		crsMasterEntity = new EntityOperation(Of CrsMasterEntity);
		koshakashoEntity = new EntityOperation(Of KoshakashoEntity);

	}
	private EntityOperation itinerary_KoshakashoEntity; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors. //行程_降車ヶ所エンティティ
	private CrsMasterKeyKoumoku _taisyoCrsInfo = new CrsMasterKeyKoumoku(); //_対象コース情報
	private string _SuppliersName = "東京タワー・大展望台";
	private ProcessMode _processMode; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
	private ItineraryInfoKeyKoumoku _itineraryKey = null; //_行程キー
	public EntityOperation crsMasterEntity; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors. //コースマスタエンティティ
	private EntityOperation koshakashoEntity; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors. //降車ヶ所エンティティ
	private CrsMasterKeyKoumoku[] _taisyoCrsInfoz; //_対象コース情報()

	private void Button1_Click(object sender, EventArgs e)
	{
		S01_0308 _frmBasicInfo_Rosenzu = null; //_frm基本情報_路線図

		try
		{
			_frmBasicInfo_Rosenzu = new S01_0308();

			_frmBasicInfo_Rosenzu.ImageStorageForIchijiFolder = string.Empty; //imageStorageForIchijiFolder
																			  //_frmBasicInfo_Rosenzu.SetRosenzuFile = Me.txtRosenzuFile.Text

			_frmBasicInfo_Rosenzu.ShowDialog(this);

			this.txtRosenzuFile.Text = _frmBasicInfo_Rosenzu.SetRosenzuFile;

			//If _frmBasicInfo_Rosenzu.SetRosenzuFile <> String.Empty Then
			//    _rosenzuIchijiFile = imageStorageForIchijiFolder & Me.txtRosenzuFile.Text
			//Else
			//    Me.pctRosenzu.Image = Nothing
			//    _rosenzuIchijiFile = imageStorageForIchijiFolder
			//End If

			//Call dispRosenzu()

			System.GC.Collect();

		}
		catch (Exception)
		{
			//TODO:共通変更対応
			//メッセージ出力.messageDisp("9001")
			createFactoryMsg.messageDisp("9001");
		}
		finally
		{
			_frmBasicInfo_Rosenzu.Dispose();
		}
	}

	private void Button3_Click(object sender, EventArgs e)
	{
		S01_0316 _frmMealSet = null; //_frm食事設定

		try
		{
			_frmMealSet = new S01_0316();

			_frmMealSet.MealDetailEntity = itinerary_KoshakashoEntity.EntityData(0).MealDetailEntity;
			_itineraryKey.ItineraryKind = (ItineraryKindType)(itinerary_KoshakashoEntity.EntityData(0).ItineraryKind.Value);

			_frmMealSet.SuppliersName = _SuppliersName; //_仕入先名
			_itineraryKey.CrsMasterKeyKoumoku = _taisyoCrsInfo;
			_itineraryKey.LineNo = 1;
			_frmMealSet.ItineraryInfo = _itineraryKey;
			_frmMealSet.ProcessMode = _processMode;
			_frmMealSet.ShowDialog(this);

		}
		catch (Exception ex)
		{
			throw (ex);
		}

	}

	private void Button8_Click(object sender, EventArgs e)
	{
		S01_0320 _frmSiireInfo = null; //_frm仕入情報
		ItineraryInfoKeyKoumoku _itineraryKey = null; //_行程キー
		string _suppliersCd = string.Empty; //_仕入先コード
		string _suppliersEdaban = string.Empty; //_仕入先枝番

		try
		{
			_frmSiireInfo = new S01_0320();


			_frmSiireInfo.SiireInfoEntity = itinerary_KoshakashoEntity.EntityData(0).SiireInfoEntity;
			_itineraryKey.ItineraryKind = (ItineraryKindType)(itinerary_KoshakashoEntity.EntityData(0).ItineraryKind.Value);
			_suppliersCd = System.Convert.ToString(itinerary_KoshakashoEntity.EntityData(0).Koshakasho.Value);
			_suppliersEdaban = System.Convert.ToString(itinerary_KoshakashoEntity.EntityData(0).KoshakashoEdaban.Value);

			_itineraryKey.CrsMasterKeyKoumoku = _taisyoCrsInfo;
			_itineraryKey.LineNo = 1;
			_frmSiireInfo.CrsMasterEntity = crsMasterEntity.deepCopy;
			_frmSiireInfo.SuppliersCd = _suppliersCd;
			_frmSiireInfo.SuppliersEdaban = _suppliersEdaban;
			_frmSiireInfo.ItineraryInfo = _itineraryKey;
			_frmSiireInfo.ProcessMode = _processMode;
			_frmSiireInfo.TyakTime = "";
			_frmSiireInfo.HatuTime = "";
			//印刷用エンティティを渡す↓
			returnKoshakashoEntity();
			//setPrintForEntity()
			//_frmSiireInfo.PrintForEntity = printForEntity.deepCopy
			_frmSiireInfo.ShowDialog(this);
		}
		catch (Exception ex)
		{
			throw (ex);
		}
	}

	private void Button4_Click(object sender, EventArgs e)
	{
		S01_0317 _frmStaySet = null; //_frm宿泊設定
		try
		{
			_frmStaySet = new S01_0317();
			_frmStaySet.SuppliersName = _SuppliersName;
			_frmStaySet.StayDetailEntity = itinerary_KoshakashoEntity.EntityData(0).StayDetailEntity.EntityData(0);
			_itineraryKey.CrsMasterKeyKoumoku = _taisyoCrsInfo;
			_itineraryKey.LineNo = 1;
			_itineraryKey.ItineraryKind = (ItineraryKindType)(itinerary_KoshakashoEntity.EntityData(0).ItineraryKind.Value);
			_frmStaySet.TaisyoItineraryInfo = _itineraryKey;
			_frmStaySet.ProcessMode = _processMode;
			_frmStaySet.ShowDialog(this);

		}
		catch (Exception ex)
		{
			throw (ex);
		}
	}

	private void Test用_Load(object sender, EventArgs e)
	{

		_taisyoCrsInfo.Teiki_KikakuKbn = "1";
		_taisyoCrsInfo.CrsCd = "R0101A";
		_taisyoCrsInfo.KaiteiDay = "20180820";
		_taisyoCrsInfo.Season = "1";
		_taisyoCrsInfo.Year = "1";
		_taisyoCrsInfo.InvalidFlg = 0;

		itinerary_KoshakashoEntity.EntityData(0).ItineraryKind.Value = "1";

	}

	private void Button5_Click(object sender, EventArgs e)
	{
		S01_0318 _frmBikoSet = null; //_frm備考設定
		ItineraryInfoKeyKoumoku _itineraryKey = null; //_行程キー
		string _suppliersCd = string.Empty; //_仕入先コード
		string _suppliersEdaban = string.Empty; //_仕入先枝番

		try
		{
			_frmBikoSet = new S01_0318();

			_frmBikoSet.KoshakashoEntityBase = itinerary_KoshakashoEntity.deepCopy;
			_itineraryKey.ItineraryKind = (ItineraryKindType)(itinerary_KoshakashoEntity.EntityData(0).ItineraryKind.Value);
			_suppliersCd = System.Convert.ToString(itinerary_KoshakashoEntity.EntityData(0).Koshakasho.Value);
			_suppliersEdaban = System.Convert.ToString(itinerary_KoshakashoEntity.EntityData(0).KoshakashoEdaban.Value);

			_frmBikoSet.SuppliersName = _SuppliersName;
			_itineraryKey.CrsMasterKeyKoumoku = _taisyoCrsInfo;
			_itineraryKey.LineNo = 1;
			_frmBikoSet.LineNo = 0;
			_frmBikoSet.ItineraryInfo = _itineraryKey;
			_frmBikoSet.ProcessMode = _processMode;
			_frmBikoSet.SuppliersCd = _suppliersCd;
			_frmBikoSet.SuppliersEdaban = _suppliersEdaban;
			_frmBikoSet.ShowDialog(this);

			if (_frmBikoSet.KakuteiFlg == true)
			{
				itinerary_KoshakashoEntity.EntityData(0).Biko.Value
					= _frmBikoSet.KoshakashoEntityBase.EntityData(0).Biko.Value;
				itinerary_KoshakashoEntity.EntityData(0).Midokoro.Value
					= _frmBikoSet.KoshakashoEntityBase.EntityData(0).Midokoro.Value;
				itinerary_KoshakashoEntity.copy_BackupFromZenkaiValue();
			}

		}
		catch (Exception ex)
		{
			throw (ex);
		}
	}

	private void Button6_Click(object sender, EventArgs e)
	{
		S01_0319 _frmToriatukaiSet = null; //_frm取扱設定
		ItineraryInfoKeyKoumoku _itineraryKey = null; //_行程キー
		string _suppliersCd = string.Empty; //_仕入先コード
		string _suppliersEdaban = string.Empty; //_仕入先枝番

		try
		{
			_frmToriatukaiSet = new S01_0319();

			_frmToriatukaiSet.ToriatukaiInfoEntityBase = itinerary_KoshakashoEntity.EntityData(0).ToriatukaiInfoEntity;
			_itineraryKey.ItineraryKind = (ItineraryKindType)(itinerary_KoshakashoEntity.EntityData(0).ItineraryKind.Value);
			_suppliersCd = System.Convert.ToString(itinerary_KoshakashoEntity.EntityData(0).Koshakasho.Value);
			_suppliersEdaban = System.Convert.ToString(itinerary_KoshakashoEntity.EntityData(0).KoshakashoEdaban.Value);

			//ADD-20120423-金額初期値に原価情報を挿入（4月運用サポート）↓
			_frmToriatukaiSet.KoshakashoEntityBase = itinerary_KoshakashoEntity.EntityData(0);


			_frmToriatukaiSet.SuppliersName = _SuppliersName;
			_itineraryKey.CrsMasterKeyKoumoku = _taisyoCrsInfo;
			_itineraryKey.LineNo = 1;
			_frmToriatukaiSet.ItineraryInfo = _itineraryKey;
			_frmToriatukaiSet.ProcessMode = _processMode;
			_frmToriatukaiSet.SuppliersCd = _suppliersCd;
			_frmToriatukaiSet.SuppliersEdaban = _suppliersEdaban;
			_frmToriatukaiSet.ShowDialog(this);

			if (_frmToriatukaiSet.KakuteiFlg == true)
			{
				itinerary_KoshakashoEntity.EntityData(0).ToriatukaiInfoEntity.clear();
				for (int index = 0; index <= _frmToriatukaiSet.ToriatukaiInfoEntityBase.EntityData.Length - 1; index++)
				{
					itinerary_KoshakashoEntity.EntityData(0).ToriatukaiInfoEntity.add(_frmToriatukaiSet.ToriatukaiInfoEntityBase.EntityData(index));
				}
				itinerary_KoshakashoEntity.EntityData(0).ToriatukaiInfoEntity.remove(0);
			}

		}
		catch (Exception ex)
		{
			throw (ex);
		}
	}

	private void Button7_Click(object sender, EventArgs e)
	{
		S01_0314 _frmCostSet = null; //_frm原価設定
		ItineraryInfoKeyKoumoku _ItineraryKey = null; //_行程キー
		string _reserveApplicationStartDay = string.Empty; //_指定適用開始日
		string _suppliersCd = string.Empty; //_仕入先コード
		string _suppliersEdaban = string.Empty; //_仕入先枝番

		try
		{
			_frmCostSet = new S01_0314();


			_frmCostSet.CostMaster_KoshakashoEntityBase = itinerary_KoshakashoEntity.EntityData(0).CostMaster_KoshakashoEntity.deepCopy;
			_ItineraryKey.ItineraryKind = (ItineraryKindType)(itinerary_KoshakashoEntity.EntityData(0).ItineraryKind.Value);
			_suppliersCd = System.Convert.ToString(itinerary_KoshakashoEntity.EntityData(0).Koshakasho.Value);
			_suppliersEdaban = System.Convert.ToString(itinerary_KoshakashoEntity.EntityData(0).KoshakashoEdaban.Value);

			_ItineraryKey.CrsMasterKeyKoumoku = _taisyoCrsInfo;
			_ItineraryKey.LineNo = 1;
			_frmCostSet.SuppliersName = _SuppliersName;
			_frmCostSet.TaisyoCrsInfo = _ItineraryKey;
			_frmCostSet.ApplicationStartDay = "2018/08/20".ToString().Replace("/", "");
			_frmCostSet.DeleteDay = "";
			_frmCostSet.ProcessMode = _processMode;
			_frmCostSet.SuppliersCd = _suppliersCd;
			_frmCostSet.SuppliersEdaban = _suppliersEdaban;
			_frmCostSet.ShowDialog(this);

			if (_frmCostSet.KakuteiFlg == true)
			{
				itinerary_KoshakashoEntity.EntityData(0).CostMaster_KoshakashoEntity.clear();
				for (int index = 0; index <= _frmCostSet.CostMaster_KoshakashoEntityBase.EntityData.Length - 1; index++)
				{
					itinerary_KoshakashoEntity.EntityData(0).CostMaster_KoshakashoEntity.add(_frmCostSet.CostMaster_KoshakashoEntityBase.EntityData(index));
				}
				itinerary_KoshakashoEntity.EntityData(0).CostMaster_KoshakashoEntity.remove(0);
				itinerary_KoshakashoEntity.EntityData(0).CostMaster_KoshakashoEntity.copy_BackupFromZenkaiValue();
			}
		}
		catch (Exception ex)
		{
			throw (ex);
		}
	}

	/// <summary>
	/// 行程_降車ヶ所エンティティ＆その他行程_降車ヶ所エンティティから降車ヶ所エンティティへマージ
	/// ※コースマスタエンティティ→降車ヶ所マスタエンティティ→行程_降車ヶ所エンティティ→tbl行程情報→grd行程情報
	/// </summary>
	/// <remarks></remarks>
	private void returnKoshakashoEntity() //return降車ヶ所エンティティ()
	{
		EntityOperation _copyEntity = new EntityOperation(Of KoshakashoEntity); //_copyエンティティ

		for (int index = 0; index <= itinerary_KoshakashoEntity.EntityData.Length - 1; index++)
		{
			if (itinerary_KoshakashoEntity.EntityData(index).Teiki_KikakuKbn.Value != string.Empty &&)
			{
				itinerary_KoshakashoEntity.EntityData(index).Year.Value != string.Empty &&;
				itinerary_KoshakashoEntity.EntityData(index).LineNo.Value IsNot null;
				_copyEntity.add(itinerary_KoshakashoEntity.EntityData(index));
			}
		}
		if (_copyEntity.EntityData(0).Teiki_KikakuKbn.Value == string.Empty &&)
		{
			_copyEntity.EntityData(0).Year.Value = string.Empty &&;
			ReferenceEquals(_copyEntity.EntityData(0).LineNo.Value, null) &&;
			_copyEntity.EntityData.Length(!= 1);
			_copyEntity.remove(0);
		}
		koshakashoEntity = _copyEntity.deepCopy;

	}

	private void Button9_Click(object sender, EventArgs e)
	{

		_taisyoCrsInfoz = new CrsMasterKeyKoumoku[1];

		CrsMasterKeyKoumoku with_1 = _taisyoCrsInfoz[0];
		//定期
		with_1.Teiki_KikakuKbn = TextBox1.Text;
		with_1.CrsCd = TextBox2.Text;
		with_1.KaiteiDay = TextBox3.Text;
		with_1.Year = TextBox4.Text;
		with_1.Season = ComboBoxEx1.SelectedValue;
		with_1.InvalidFlg = "0";

		//企画
		//.Teiki_KikakuKbn = FixedCd.Teiki_KikakuKbnType.teikiKanko
		//.CrsCd = CStr(grdCrsList.GetData(row, CrsList_Koumoku.crsCd))
		//.Year = CStr(grdCrsList.GetData(row, CrsList_Koumoku.year))
		//.Season = CStr(grdCrsList.GetData(row, CrsList_Koumoku.season))
		//.KaiteiDay = CStr(grdCrsList.GetData(row, CrsList_Koumoku.kaiteiDay)).Replace("/", "")
		//.InvalidFlg = CInt(grdCrsList.GetData(row, CrsList_Koumoku.invalidFlg))


		crsMasterOperation_DA clsCrsMasterOperation_DA = new crsMasterOperation_DA(); //clsコースマスタ操作_DA
																					  //データ格納用のエンティティクラス初期化
		crsMasterEntity = new EntityOperation(Of CrsMasterEntity);

		//DBのデータをエンティティクラスに格納
		if (clsCrsMasterOperation_DA.getBasicInfoEntity(_taisyoCrsInfoz[0], crsMasterEntity) == false)
		{
			//エラー処理
			return;
		}

		DBInsertUpdate();

		//Dim frm As S01_0311 = New S01_0311()

		//'親画面から入力情報
		//frm.TaisyoCrsInfo = _taisyoCrsInfoz(0)
		//frm.CostApplicationStartDay = crsMasterEntity.EntityData(0).CostMaster_HeaderEntity.EntityData(0).ApplicationStartDay.Value
		//frm.BaseCrsMasterEntity = crsMasterEntity
		//frm.ShowDialog()



	}

	private void Button10_Click(object sender, EventArgs e)
	{
		CdMasterGet_DA clsCdMasterGet_DA = new CdMasterGet_DA(); //clsコードマスタ取得_DA
		DataTable dtSeason = null; //dt季
		if (TextBox1.Text == System.Convert.ToString(Teiki_KikakuKbnType.teikiKanko))
		{
			dtSeason = clsCdMasterGet_DA.GetCodeMasterData(CdBunruiType.seasonMaster_Teiki, true);
		}
		else
		{
			dtSeason = clsCdMasterGet_DA.GetCodeMasterData(CdBunruiType.seasonMaster_Kikaku, true);
		}
		setComboBoxToDataTable(this.ComboBoxEx1, dtSeason);
	}
	/// <summary>
	/// コンボボックスにDataTableの割付けを行う
	/// </summary>
	/// <param name="targetCtl"></param>
	/// <param name="dtList"></param>
	/// <remarks></remarks>
	private void setComboBoxToDataTable(ComboBoxEx targetCtl, DataTable dtList)
	{

		ComboBoxEx with_1 = targetCtl;
		with_1.DataSource = dtList;
		with_1.ValueSubItemIndex = 0;
		with_1.ListColumns(0).Visible = false;
		with_1.ListHeaderPane.Visible = false;
		with_1.TextSubItemIndex = 1;
		with_1.ListColumns(1).Width = with_1.Width;
		with_1.DropDown.AllowResize = false;
	}


	private bool DBInsertUpdate()
	{

		OracleTransaction oraTran = null;
		crsMasterOperation_DA clsCrsMasterOperation_DA = new crsMasterOperation_DA(); //clsコースマスタ操作_DA
		crsMasterOperation_DA.crsMasterWritingMode mode = null;
		CrsMasterKeyKoumoku _taisyoCrsInfo_EvacuateFor = null; //_対象コース情報_退避用
		_taisyoCrsInfo_EvacuateFor = _taisyoCrsInfoz[0];

		try
		{
			//トランザクション開始
			oraTran = clsCrsMasterOperation_DA.beginTransaction();

			//「更新」時は元データを削除
			//If _processMode = FixedCd.ProcessMode.shinki Then
			//    mode = crsMasterOperation_DA.crsMasterWritingMode.newEntry
			//ElseIf _processMode = FixedCd.ProcessMode.update Then
			clsCrsMasterOperation_DA.deleteTableBeforeInsert(oraTran, _taisyoCrsInfo_EvacuateFor);
			mode = crsMasterOperation_DA.crsMasterWritingMode.update;
			//ElseIf _processMode = FixedCd.ProcessMode.copy Then
			//    mode = crsMasterOperation_DA.crsMasterWritingMode.copy
			//End If

			//Insert
			if (clsCrsMasterOperation_DA.InsertALLFromcrsMasterEntity(oraTran, mode, crsMasterEntity, _taisyoCrsInfoz[0]) == false)
			{
				clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
				createFactoryMsg.messageDisp("0008", "コースコード");
				return false;
			}

			//コミット
			clsCrsMasterOperation_DA.commitTransaction(oraTran);

		}
		catch (OracleException ex)
		{
			//TODO:共通変更対応
			//outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.登録, Me.setTitle, _対象コース情報(0).定期_企画区分 & _対象コース情報(0).年 & _対象コース情報(0).季 & _対象コース情報(0).コースコード & _対象コース情報(0).無効フラグ.ToString, ex.Message)
			//Call clsコースマスタ操作_DA.rollbackTransaction(oraTran)
			//メッセージ出力.messageDisp("0006", ex.Number.ToString)
			//createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, Me.setTitle, _taisyoCrsInfo(0).Teiki_KikakuKbn & _taisyoCrsInfo(0).Year & _taisyoCrsInfo(0).Season & _taisyoCrsInfo(0).CrsCd & _taisyoCrsInfo(0).InvalidFlg.ToString, ex.Message)
			clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
			createFactoryMsg.messageDisp("0006", ex.Number.ToString());
			return false;
		}
		catch (Exception)
		{
			//TODO:共通変更対応
			//outputLog(ログ種別タイプ.エラーログ, 処理種別タイプ.登録, Me.setTitle, _対象コース情報(0).定期_企画区分 & _対象コース情報(0).年 & _対象コース情報(0).季 & _対象コース情報(0).コースコード & _対象コース情報(0).無効フラグ.ToString, ex.Message)
			//Call clsコースマスタ操作_DA.rollbackTransaction(oraTran)
			//メッセージ出力.messageDisp("9001")
			//createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.entry, Me.setTitle, _taisyoCrsInfo(0).Teiki_KikakuKbn & _taisyoCrsInfo(0).Year & _taisyoCrsInfo(0).Season & _taisyoCrsInfo(0).CrsCd & _taisyoCrsInfo(0).InvalidFlg.ToString, ex.Message)
			clsCrsMasterOperation_DA.rollbackTransaction(oraTran);
			createFactoryMsg.messageDisp("9001");
			return false;
		}

		return true;

	}

	private void Button11_Click(object sender, EventArgs e)
	{

		_taisyoCrsInfoz = new CrsMasterKeyKoumoku[1];

		CrsMasterKeyKoumoku with_1 = _taisyoCrsInfoz[0];
		//定期
		with_1.Teiki_KikakuKbn = TextBox1.Text;
		with_1.CrsCd = TextBox2.Text;
		with_1.KaiteiDay = TextBox3.Text;
		with_1.Year = TextBox4.Text;
		with_1.Season = ComboBoxEx1.SelectedValue;
		with_1.InvalidFlg = "0";

		//企画
		//.Teiki_KikakuKbn = FixedCd.Teiki_KikakuKbnType.teikiKanko
		//.CrsCd = CStr(grdCrsList.GetData(row, CrsList_Koumoku.crsCd))
		//.Year = CStr(grdCrsList.GetData(row, CrsList_Koumoku.year))
		//.Season = CStr(grdCrsList.GetData(row, CrsList_Koumoku.season))
		//.KaiteiDay = CStr(grdCrsList.GetData(row, CrsList_Koumoku.kaiteiDay)).Replace("/", "")
		//.InvalidFlg = CInt(grdCrsList.GetData(row, CrsList_Koumoku.invalidFlg))


		crsMasterOperation_DA clsCrsMasterOperation_DA = new crsMasterOperation_DA(); //clsコースマスタ操作_DA
																					  //データ格納用のエンティティクラス初期化
		crsMasterEntity = new EntityOperation(Of CrsMasterEntity);

		//DBのデータをエンティティクラスに格納
		if (clsCrsMasterOperation_DA.getBasicInfoEntity(_taisyoCrsInfoz[0], crsMasterEntity) == false)
		{
			//エラー処理
			return;
		}

		S01_0311 frm = new S01_0311();

		//親画面から入力情報
		frm.TaisyoCrsInfo = _taisyoCrsInfoz[0];
		frm.CostApplicationStartDay = crsMasterEntity.EntityData(0).CostMaster_HeaderEntity.EntityData(0).ApplicationStartDay.Value;
		frm.BaseCrsMasterEntity = crsMasterEntity;
		frm.ShowDialog();
	}
}