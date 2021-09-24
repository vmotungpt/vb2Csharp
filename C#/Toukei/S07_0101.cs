/// <summary>
/// S07_0101 売上概算表
/// </summary>
public class S07_0101 : PT01, iPT01
{

	#region 定数／変数宣言
	public DataTable dtUriaGaisanHyo; //売上概算表出力用ワークテーブル

	/// <summary>
	/// パラメータクラス
	/// </summary>
	public S07_0101ParamData ParamData
	{

#endregion

		#region イベント

		/// <summary>
		/// 固有初期処理
		/// </summary>
	protected override void initScreenPerttern()
	{

		//初期画面設定
		this.setSeFirsttDisplayData();
		// フッタボタンの設定
		this.initFooterButtonControl();
	}

	/// <summary>
	/// 画面クロージングイベント
	/// </summary>
	protected override bool closingScreen()
	{

		return true;
	}

	/// <summary>
	/// F2：戻るボタン押下イベント
	/// </summary>
	protected override void btnF2_ClickOrgProc()
	{

		base.closeFormFlg = true;
		this.Close();
	}

	/// <summary>
	/// F7：印刷ボタン押下イベント
	/// </summary>
	protected override void btnF7_ClickOrgProc()
	{
		S07_0101Da s07_0101da = new S07_0101Da();
		DataTable UriageGaisanhyoList = new DataTable();
		int intUpdateCount = 0;
		// 入力チェック
		if (this.CheckSearch() == false)
		{

			return;
		}

		//確認メッセージ
		if (createFactoryMsg.messageDisp("Q90_001", "印刷") == MsgBoxResult.Ok)
		{

			DateTime dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(dtmSyuptDayFromTo.FromDateText));
			S07_0101ParamData prm = new S07_0101ParamData();
			//出発日(開始)
			prm.SyuptDayFrom = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
			//前年出発日(開始)
			if (this.rdoSameYobi.Checked == true)
			{
				dteTmpDate = dteTmpDate.AddDays(-364);
				prm.ZenSyuptDayFrom = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
			}
			else
			{
				dteTmpDate = dteTmpDate.AddYears(-1);
				prm.ZenSyuptDayFrom = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
			}
			//出発日(終了)
			dteTmpDate = System.Convert.ToDateTime(Convert.ToDateTime(dtmSyuptDayFromTo.ToDateText));
			prm.SyuptDayTo = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
			//前年出発日(終了)
			if (this.rdoSameYobi.Checked == true)
			{
				dteTmpDate = dteTmpDate.AddDays(-364);
				prm.ZenSyuptDayTo = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
			}
			else
			{
				dteTmpDate = dteTmpDate.AddYears(-1);
				prm.ZenSyuptDayTo = (dteTmpDate.Year * 10000 + dteTmpDate.Month * 100 + dteTmpDate.Day).ToString();
			}
			//コース種別(日本語コース)
			if (this.chkJapanese.Checked == true)
			{
				prm.houjinKbn = "Y";
			}
			else
			{
				prm.houjinKbn = "";
			}
			//コース種別(外国語コース)
			if (this.chkGaikokugo.Checked == true)
			{
				prm.GaikyakuKbn = "Y";
			}
			else
			{
				prm.GaikyakuKbn = "";
			}
			//取扱部署
			if (this.cmbToriatukaiBusyo.SelectedIndex != -1)
			{
				prm.ManagementSec = this.cmbToriatukaiBusyo.SelectedValue.ToString().Trim();
			}
			else
			{
				prm.ManagementSec = "";
			}
			//表示単位
			if (this.rdoCrsKbn.Checked == true)
			{
				prm.OutputType = "0"; //コース区分
			}
			else
			{
				if (this.rdoToriatukaiBusyo.Checked == true)
				{
					prm.OutputType = "1"; //取扱部署
				}
				else
				{
					prm.OutputType = "2"; //管理会計部門
				}
			}
			//管理会計部門
			//   コース種別(定期コース)
			if (this.chkTeiki.Checked == true)
			{
				prm.Crs_Teiki = "Y";
			}
			else
			{
				prm.Crs_Teiki = "";
			}
			//   コース種別(企画コース)
			if (this.chkKikaku.Checked == true)
			{
				prm.Crs_Kikaku = "Y";
			}
			else
			{
				prm.Crs_Kikaku = "";
			}
			//前年数値表示
			if (this.rdoSameDay.Checked == true)
			{
				prm.ZennenDayYobiKbn = "0"; //同日
			}
			else
			{
				prm.ZennenDayYobiKbn = "1"; //同曜日
			}

			if (prm.OutputType != "2")
			{
				// 売上概算表（コース区分毎、取扱部署毎）Work:作成
				if (s07_0101da.registUriageGaisanhyo(prm, intUpdateCount) == false)
				{
					CommonProcess.createFactoryMsg().messageDisp("E90_019");
					return;
				}

				// 売上概算表データソース設定
				dtUriaGaisanHyo = s07_0101da.getUriageGaisanhyoWork(prm);

				//プレビュー
				if (showPreview(prm) == true)
				{
					// 処理成功時
					CommonProcess.createFactoryMsg().messageDisp("I90_002", "帳票出力");
				}
				else
				{
					// 失敗時
					CommonProcess.createFactoryMsg().messageDisp("E90_043");
				}
			}
			else
			{
				if (prm.Crs_Teiki == "Y")
				{
					// 売上概算表（管理会計部門・定期）Work():作成
					if (s07_0101da.registUriageGaisanhyo(prm, intUpdateCount,)
						{
						Teiki_KikakuKbnType.teikiKanko() == false);
						CommonProcess.createFactoryMsg().messageDisp("E03_028", "［管理会計部門：定期］該当データが");
						return;
					}
					else
					{
						// 売上概算表データソース設定
						dtUriaGaisanHyo = s07_0101da.getUriageGaisanhyoWork(prm);

						//プレビュー
						if (showPreview(prm, Teiki_KikakuKbnType.teikiKanko) == true)
						{
							// 処理成功時
							CommonProcess.createFactoryMsg().messageDisp("I90_002", "帳票出力");
						}
						else
						{
							// 失敗時
							CommonProcess.createFactoryMsg().messageDisp("E90_043");
						}
					}
				}
				if (prm.Crs_Kikaku == "Y")
				{
					// 売上概算表（管理会計部門・企画）Work():作成
					if (s07_0101da.registUriageGaisanhyo(prm, intUpdateCount,)
						{
						Teiki_KikakuKbnType.kikakuTravel() == false);
						CommonProcess.createFactoryMsg().messageDisp("E03_028", "［管理会計部門：企画］該当データが");
						return;
					}

					// 売上概算表データソース設定
					dtUriaGaisanHyo = s07_0101da.getUriageGaisanhyoWork(prm);

					//プレビュー
					if (showPreview(prm) == true)
					{
						// 処理成功時
						CommonProcess.createFactoryMsg().messageDisp("I90_002", "帳票出力");
					}
					else
					{
						// 失敗時
						CommonProcess.createFactoryMsg().messageDisp("E90_043");
					}
				}
			}




			//Dim viewer As New P07_0101Viewer
			//Dim rpt As New P07_0101

			//rpt.DataSource = dtUriaGaisanHyo
			//rpt.TaisyoYm = Me.dtmSyuptDayFromTo.FromDateText.Value.ToString("yy/MM/dd  ～  ") &
			//               Me.dtmSyuptDayFromTo.ToDateText.Value.ToString("MM/dd  出発分")
			//rpt.OutputType = prm.OutputType

			//viewer.Report = rpt
			//viewer.Show()
			//rpt.Run()

			//'印刷
			//rpt.Document.Print(True, True)
		}

	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	protected override void initFooterButtonControl()
	{

		//Visible
		this.F1Key_Visible = false;
		this.F2Key_Visible = true;
		this.F3Key_Visible = false;
		this.F4Key_Visible = false;
		this.F5Key_Visible = false;
		this.F6Key_Visible = false;
		this.F7Key_Visible = true;
		this.F8Key_Visible = false;
		this.F9Key_Visible = false;
		this.F10Key_Visible = false;
		this.F11Key_Visible = false;
		this.F12Key_Visible = false;

		//Text
		this.F2Key_Text = "F2:戻る";
		this.F7Key_Text = "F7:印刷";
	}

	#endregion

	#region メソッド

	/// <summary>
	/// 初期画面設定
	/// </summary>
	public void setSeFirsttDisplayData()
	{
		//所属部署が国際事業部の場合、外国語にチェック
		if (UserInfoManagement.gaikokugoCrsSelectFlg == true)
		{
			this.chkGaikokugo.Checked = true; // 外国語のみチェック
			this.chkJapanese.Checked = false;
		}
		else
		{
			this.chkJapanese.Checked = true; //日本語コースのみチェック
			this.chkGaikokugo.Checked = false;
		}
		// 出発日(From)設定
		this.dtmSyuptDayFromTo.FromDateText = CommonDa.createFactoryDA.getServerSysDate();

		// 取扱部署取得
		MUserSecEntity entity = new MUserSecEntity();
		entity.CompanyCd.Value = FixedCd.MuserCompanyCdType.hatoBus;

		S07_0101Da s07_0101Da = new S07_0101Da();
		DataTable userSectionList = s07_0101Da.getUserSection(entity);

		// 空行追加
		DataRow row = userSectionList.NewRow();
		foreach (DataColumn col in userSectionList.Columns)
		{

			row[col.ColumnName] = " ";
		}

		userSectionList.Rows.InsertAt(row, 0);

		//取扱部署のコンボボックスに値をセット
		CommonMstUtil.setComboBox(CommonType_MojiColValue.CdBunruiType_Value(CdBunruiType.toriatsukai_busho), this.cmbToriatukaiBusyo, true, null);
		// ログインユーザの取扱部署設定
		if (UserInfoManagement.toriatukaiBusyo != (System.Convert.ToInt32(FixedCd.ToriatsukaiBusyo.others)).ToString())
		{
			this.cmbToriatukaiBusyo.SelectedValue = UserInfoManagement.toriatukaiBusyo;
		}
		//表示単位
		this.rdoCrsKbn.Checked = true;
		//前年数値表示
		this.rdoSameYobi.Checked = true;
	}

	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent"></param>
	public void DisplayDataToEntity(ref object ent)
	{

	}

	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <returns>検証結果</returns>
	public bool CheckSearch()
	{

		// エラー初期化
		this.initExitError();

		// 出発日(From)必須チェック
		if (ReferenceEquals(this.dtmSyuptDayFromTo.FromDateText, null))
		{

			this.dtmSyuptDayFromTo.ExistErrorForFromDate = true;
			this.dtmSyuptDayFromTo.Focus();
			CommonProcess.createFactoryMsg.messageDisp("E90_011", "出発日(From)");
			return false;
		}

		// 出発日(To)必須チェック
		if (ReferenceEquals(this.dtmSyuptDayFromTo.ToDateText, null))
		{

			this.dtmSyuptDayFromTo.ExistErrorForToDate = true;
			this.dtmSyuptDayFromTo.Focus();
			CommonProcess.createFactoryMsg.messageDisp("E90_011", "出発日(To)");
			return false;
		}

		// 出発日範囲チェック
		if (ReferenceEquals(this.dtmSyuptDayFromTo.ToDateText, null) == false)
		{

			if (this.dtmSyuptDayFromTo.FromDateText > this.dtmSyuptDayFromTo.ToDateText)
			{

				this.dtmSyuptDayFromTo.ExistErrorForToDate = true;
				this.dtmSyuptDayFromTo.Focus();
				CommonProcess.createFactoryMsg().messageDisp("E90_016", this.lblSyuptDay.Text);
				return false;
			}
		}

		// コース種別必須チェック
		if (this.chkJapanese.Checked == false && this.chkGaikokugo.Checked == false)
		{

			this.chkJapanese.ExistError = true;
			this.chkGaikokugo.ExistError = true;
			this.chkJapanese.Focus();
			CommonProcess.createFactoryMsg.messageDisp("E90_024", this.lblCrsKind.Text);
			return false;
		}

		// 管理会計部門選択時:コース必須チェック
		if (this.rdoKanriKaikeiBumon.Checked == true && this.chkTeiki.Checked == false && this.chkKikaku.Checked == false)
		{
			this.chkTeiki.ExistError = true;
			this.chkKikaku.ExistError = true;
			this.chkTeiki.Focus();
			CommonProcess.createFactoryMsg.messageDisp("E90_024", this.rdoKanriKaikeiBumon.Text + " ");
			return false;
		}

		return true;
	}

	/// <summary>
	/// エラー初期化
	/// </summary>
	private void initExitError()
	{

		this.dtmSyuptDayFromTo.ExistErrorForFromDate = false;
		this.dtmSyuptDayFromTo.ExistErrorForToDate = false;
		this.chkJapanese.ExistError = false;
		this.chkGaikokugo.ExistError = false;
		this.chkTeiki.ExistError = false;
		this.chkKikaku.ExistError = false;
	}

	/// <summary>
	/// プレビュー処理の実行
	///     True:正常終了 False:出力データ無し
	/// </summary>
	/// <param name="plam"></param>
	/// <returns></returns>
	private object showPreview()
	{
		Optional ByVal KanriKaikeiBumon Teiki_KikakuKbnType = 0) bool;
		P07_0101Viewer viewer = new P07_0101Viewer();
		P07_0101 reportCrs = new P07_0101();
		P07_0101_Teiki reportTeiki = new P07_0101_Teiki();
		P07_0101_Kikaku reportKikaku = new P07_0101_Kikaku();

		try
		{
			if (dtUriaGaisanHyo.Rows.Count > 0)
			{
				if (KanriKaikeiBumon == Teiki_KikakuKbnType.teikiKanko)
				{
					reportTeiki.TaisyoYm = this.dtmSyuptDayFromTo.FromDateText.Value.ToString("yy/MM/dd  ～  ") +;
					this.dtmSyuptDayFromTo.ToDateText.Value.ToString("MM/dd  出発分");
					reportTeiki.DataSource = dtUriaGaisanHyo;
					reportTeiki.OutputType = plam.OutputType;
					viewer.Report = reportTeiki;
				}
				else if (KanriKaikeiBumon == Teiki_KikakuKbnType.kikakuTravel)
				{
					reportKikaku.TaisyoYm = this.dtmSyuptDayFromTo.FromDateText.Value.ToString("yy/MM/dd  ～  ") +;
					this.dtmSyuptDayFromTo.ToDateText.Value.ToString("MM/dd  出発分");
					reportKikaku.DataSource = dtUriaGaisanHyo;
					reportKikaku.OutputType = plam.OutputType;
					viewer.Report = reportKikaku;
				}
				else
				{
					reportCrs.TaisyoYm = this.dtmSyuptDayFromTo.FromDateText.Value.ToString("yy/MM/dd  ～  ") +;
					this.dtmSyuptDayFromTo.ToDateText.Value.ToString("MM/dd  出発分");
					reportCrs.DataSource = dtUriaGaisanHyo;
					reportCrs.OutputType = plam.OutputType;
					viewer.Report = reportCrs;
				}
				viewer.Show();
			}
			else
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			viewer.Close();
			throw (ex);
		}
		finally
		{
			reportCrs.Dispose();
			reportTeiki.Dispose();
			reportKikaku.Dispose();
		}

		return true;
	}
	#endregion

}