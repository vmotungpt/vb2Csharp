using System.Text;

/// <summary>
/// 営業所別予約受付一覧表
/// </summary>
public class S07_0104 : PT01, iPT01
{

	#region 定数／変数宣言
	/// <summary>
	/// 対象年月
	/// </summary>
	private const string DtTaishoMessage = "対象年月";
	/// <summary>
	/// AGENT
	/// </summary>
	private const string Agent = "AGENT";
	/// <summary>
	/// Zero
	/// </summary>
	private const string Zero = "0000";
	/// <summary>
	/// コース種別
	/// </summary>
	private const string E90_024Param = "コース種別";
	#endregion

	#region イベント
	/// <summary>
	/// 固有初期処理
	/// </summary>
	protected override void initScreenPerttern()
	{
		// 検索条件部の項目初期化
		initSearchAreaItems();
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
		try
		{
			// 前処理
			base.comPreEvent();
			// チェックして検索
			if (checkFormItem() == false)
			{
				return;
			}
			// 帳票出力結果
			ResultOutputFormData();
		}
		catch (Exception)
		{

		}
		finally
		{
			// 後処理
			base.comPostEvent();
		}

	}

	/// <summary>
	/// 検索条件部の項目初期化
	/// </summary>
	protected override void initSearchAreaItems()
	{
		//【各画面毎】検索条件エリアの初期設定
		//「画面項目定義」を対象に、初期値記載された内容に準じて、項目の初期化を行う
		CommonUtil.Control_Init(this.gbxOutJoken.Controls);
		// ロード時にフォーカスを設定する
		this.ActiveControl = this.dtTaisho_YM;
		//定期選択
		this.rdoTeiki.Checked = true;
		//企画未選択
		this.rdoKikaku.Checked = false;

	}

	/// <summary>
	/// フッタボタンの設定
	/// </summary>
	protected override void initFooterButtonControl()
	{
		//Visible
		//ボタンの表示/非表示を変更
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
		//ボタンのテキストを変更(必要に応じて)
		this.F2Key_Text = "F2:戻る";
		this.F7Key_Text = "F7:印刷";
		F2Key_Enabled = true;
		F7Key_Enabled = true;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// 初期処理(必須画面個別実装)
	/// </summary>
	public void setSeFirsttDisplayData()
	{
		//【各画面毎】対象年月設定
		//1)画面・対象年月＝システム年月
		this.dtTaisho_YM.Value = CommonDa.createFactoryDA.getServerSysDate();
		//部署を判定しチェックON/OFFする
		this.ucoCrsKind.SetInitState();
	}

	/// <summary>
	/// 画面からエンティティに設定する処理(必須画面個別実装)
	/// </summary>
	/// <param name="ent"></param>
	public void DisplayDataToEntity(ref object ent)
	{

	}

	/// <summary>
	/// 入力項目のチェック
	/// </summary>
	/// <returns></returns>
	public bool CheckSearch()
	{
		return true;
	}
	#endregion

	#region チェック系
	/// <summary>
	/// 入力項目のチェック
	/// </summary>
	/// <returns>True:エラー無 False:エラー有</returns>
	protected override bool checkFormItem()
	{
		//背景色初期化
		base.clearExistErrorProperty(this.gbxOutJoken.Controls);
		//【各画面毎】チェック要件
		// 対象年月チェック
		// 画面・対象年月＝空白の場合
		if (ReferenceEquals(this.dtTaisho_YM.Value, null))
		{
			//メッセージが表示され処理中断
			this.dtTaisho_YM.ExistError = true;
			CommonProcess.createFactoryMsg().messageDisp("E90_011", DtTaishoMessage);
			return false;
		}

		//コース種別チェッ+ク
		//画面・コース種別・日本語＝未選択　且つ　画面・コース種別・外国語＝未選択　の場合
		if (this.ucoCrsKind.JapaneseState == false && this.ucoCrsKind.ForeignState == false)
		{
			//メッセージが表示され処理中断
			CommonProcess.createFactoryMsg().messageDisp("E90_024", E90_024Param);
			return false;
		}

		//AGNETチェック
		List[] listMAgentEntity = listMEigyosyo();
		//AGNET必須チェック
		//画面・AGENT1～18が全て空白の場合
		if (listMAgentEntity.Count() == 0)
		{
			//メッセージが表示され処理中断
			CommonProcess.createFactoryMsg().messageDisp("E90_011", Agent);
			return false;
		}
		return this.CheckSearch();
	}


	/// <summary>
	/// 帳票出力結果
	/// </summary>
	protected override void ResultOutputFormData()
	{
		string PostData = string.Empty;
		List HoujinGaikyakuKbn = new List(Of string);
		List listMAgentEntity = new List(Of string);
		List[] listMAgent = listMEigyosyo();
		foreach (MAgentEntity item in listMAgent)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(System.Convert.ToString(item.AgentCd.Value.Substring(0, 4)));
			sb.AppendLine(Zero);
			listMAgentEntity.Add(sb.Replace("\r\n", string.Empty).ToString());
		}
		try
		{
			//対象年月
			if (this.dtTaisho_YM.Value.HasValue == true)
			{
				PostData += "base_select0=P07_0103;YYYYMM&base_value0=" + this.dtTaisho_YM.Value.Value.ToString(CommonFormatType.dateFormatyyyyMM.ToString());
			}

			//コース種別
			if (this.ucoCrsKind.JapaneseState == true)
			{
				HoujinGaikyakuKbn.Add(HoujinGaikyakuKbnType.Houjin);
			}
			if (this.ucoCrsKind.ForeignState == true)
			{
				HoujinGaikyakuKbn.Add(HoujinGaikyakuKbnType.Gaikyaku);
			}
			if (HoujinGaikyakuKbn.Count > 0)
			{
				PostData += "&base_opelogic1=AND&base_select1=P07_0103;HOUJIN_GAIKYAKU_KBN&base_value1=" + string.Join(" ", HoujinGaikyakuKbn);
			}

			//定期 ON
			if (rdoTeiki.Checked == true)
			{
				PostData += "&base_opelogic2=AND&base_select2=P07_0103;TEIKI_KIKAKU_KBN&base_value2=" + Teiki_KikakuKbnType.teikiKanko;
			}

			//企画 ON
			if (rdoKikaku.Checked == true)
			{
				PostData += "&base_opelogic2=AND&base_select2=P07_0103;TEIKI_KIKAKU_KBN&base_value2=" + Teiki_KikakuKbnType.kikakuTravel;
			}

			//画面・AGENT1～18≠空白の物
			if (listMAgentEntity.Count > 0)
			{
				PostData += "&base_opelogic3=AND&base_select3=P07_0103;AGENT_CD_MAIN&base_value3=" + string.Join(" ", listMAgentEntity);
			}

			ReadAppConfig reaAppConfig = new ReadAppConfig();
			CommonProcess.DataStudioId = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioId);
			CommonProcess.DataStudioPassword = reaAppConfig.getAppSetting(ReadAppConfig.DataStudioPassword);
			BOCommon.showDataStudio(this.Name, BOCommon.SystemSetCd.dsid_EigyojoBetsuYoyakuUketsukeIchiranpyo, PostData);
		}
		catch (Exception ex)
		{
			throw (ex);
		}
	}

	#endregion

	#region チェック処理(Private)
	/// <summary>
	/// WK営業所エンティティ
	/// </summary>
	/// <returns></returns>
	private List[] listMEigyosyo()
	{
		//結果＞0件の場合、WK営業所エンティティに格納し、画面・AGENT名＝WK営業所エンティティに格納・営業所名１
		List listMAgentEntity = new List(Of MAgentEntity);
		//AGENT 1
		if (this.ucoAgent1.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent1.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 2
		if (this.ucoAgent2.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent2.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 3
		if (this.ucoAgent3.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent3.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 4
		if (this.ucoAgent4.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent4.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 5
		if (this.ucoAgent5.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent5.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 6
		if (this.ucoAgent6.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent6.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 7
		if (this.ucoAgent7.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent7.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 8
		if (this.ucoAgent8.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent8.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 9
		if (this.ucoAgent9.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent9.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 10
		if (this.ucoAgent10.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent10.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 11
		if (this.ucoAgent11.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent11.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 12
		if (this.ucoAgent12.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent12.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 13
		if (this.ucoAgent13.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent13.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 14
		if (this.ucoAgent14.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent14.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 15
		if (this.ucoAgent15.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent15.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 16
		if (this.ucoAgent16.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent16.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 17
		if (this.ucoAgent17.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent17.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		//AGENT 18
		if (this.ucoAgent18.getReturnEntity IsNot null)
		{
			MAgentEntity ucoAgent = (MAgentEntity)this.ucoAgent18.getReturnEntity;
			listMAgentEntity.Add(ucoAgent);
		}
		return listMAgentEntity;
	}
	#endregion
}