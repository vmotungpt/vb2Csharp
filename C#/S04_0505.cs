using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.Master;


public class S04_0505 : PT99, iPT99
{

	#region 画面に実装(Interface)
	/// <summary>
	/// パターンの初期設定を行う
	/// (呼出し元から呼び出される想定)
	/// </summary>
	void iPT99.iPt99StartSetting()
	{
		this.patternSettings();
	}

	public void patternSettings()
	{
		//[ボタン制御用]
		//[CSV出力可否]CSV出力ボタン(F4)の表示可否
		base.PtIsCsvOutFlg = false;
		//[ﾌﾟﾚﾋﾞｭｰ可否]プレビューボタン(F6)の表示可否
		base.PtIsPrevFlg = false;
		//[印刷/出力可否]印刷ボタン(F7)の表示可否
		base.PtIsPrintFlg = true;
		//[検索可否]検索ボタン(F8)の表示可否　※フッターのF8のため検索エリア時はFalseを設定する
		base.PtIsSearchFlg = false;
		//[登録可否]登録ボタン(F10)の表示可否
		base.PtIsRegFlg = false;
		//[更新可否]更新ボタン(F11)の表示可否
		base.PtIsUpdFlg = false;

		//【コントロール系】
		//[検索エリアコンテナ]検索エリアのグループボックスを設定する
		base.PtSearchControl = gbxSearch;
		//[検索結果エリアコンテナ]検索結果エリアのグループボックスを設定する
		base.PtResultControl = null;
		//[詳細エリアコンテナ]詳細エリアのグループボックスを設定する
		base.PtDetailControl = null;
		//[検索結果グリッド]検索結果を表示するためのGrid
		//MyBase.PtResultGrid = -
		//[表示/非表示ボタン]検索結果を表示するためのGrid
		//MyBase.PtDisplayBtn = -
		//[件数表示ラベル]検索結果を表示するためのGrid
		//MyBase.PtResultLblCnt = -

		//【データ系】
		//[最大表示件数]結果グリッドの最大表示件数
		//MyBase.PtMaxCount = -
		//[検索結果(ReadOnly)]結果グリッドの最大表示件数
		//MyBase.PtResultDT = -
		//[選択行データ(ReadOnly)]※ReadOnly 結果グリッドの選択行データ
		//MyBase.PtSelectRow = -

		//【その他】
		//[実装画面]実装画面(インターフェースを実装フォーム)
		base.PtMyForm = this;
		//[変更チェックカラム]変更チェックを行うDataTableのカラム名(ID)を設定
		//MyBase.PtDiffChkColName = -

		//【帳票用】
		//[帳票タイプ]AR/DS選択用プロパティ
		base.PtPrintType = PRINTTYPE.AR;
		//[DS用帳票ID]呼び出しDaTaStudioID
		//MyBase.PtDsPrintId = -
	}

	/// <summary>
	/// データデフォルト値設定
	/// </summary>
	/// <param name="area"></param>
	void iPT99.iPt99SetDefValue(AREA area)
	{
		this.setDefValue(area);
	}

	public void setDefValue(AREA area)
	{
		if (area == area.FORM)
		{
			//「日本語」が選択状態
			rdoJapanese.Checked = true;
			rdoForeign.Checked = false;

			//「定期（昼）」が選択状態
			rdoNoon.Checked = true;

			//「〇増ふくまない」が選択状態
			rdoMaruzouFukumanai.Checked = true;

			//初期フォーカスを対象年月に設定する
			dtmTaisyoYm.Select();
		}
		else if (area == area.BUTTON)
		{
			//なし
		}
	}

	/// <summary>
	/// フォームクローズ時処理
	/// (呼出し元から呼び出される想定)
	/// </summary>
	/// <returns></returns>
	public bool iPt99Closing()
	{
		return true;
	}

	#endregion

	#region 出力処理用
	/// <summary>
	/// 出力前(エラーチェック)処理
	/// </summary>
	/// <returns></returns>
	protected override bool OvPt99PrintBefore()
	{
		//エラークリア処理
		this.clearExistErrorProperty(gbxSearch.Controls);
		//必須チェック
		if (CommonUtil.checkHissuError(gbxSearch.Controls, null) == true)
		{
			CommonProcess.createFactoryMsg().messageDisp("E90_022");
			dtmTaisyoYm.Select();
			return false;
		}
		else
		{
			return true;
		}
	}

	/// <summary>
	/// AR出力時(データ取得)処理
	/// </summary>
	/// <returns></returns>
	protected override DataTable OvPt99PrintARGetData()
	{
		S04_0505DA dataAccess = new S04_0505DA();
		object param = createSelectParam();
		return dataAccess.selectDataTable(param);
	}

	protected override void OvPt99Print()
	{
		object param = createSelectParam();
		P04_0505Output p04_0505Output = new P04_0505Output();
		P04_0505ParamData paramData = new P04_0505ParamData();
		paramData.DataList = base.PtResultDT;
		paramData.S04_0505Param = param;
		p04_0505Output.OutputP04_0505(paramData);
	}
	#endregion

	#region Privateメソッド(画面独自)
	private S04_0505DA.S04_0505Param createSelectParam()
	{
		S04_0505DA.S04_0505Param param = new S04_0505DA.S04_0505Param();

		//対象年月
		if (dtmTaisyoYm.Value.HasValue)
		{
			param.Taisyonengetu = dtmTaisyoYm.ValueInt;
		}
		//日本語
		param.CrsJapanese = rdoJapanese.Checked;

		//外国語
		param.CrsForeign = rdoForeign.Checked;

		//定期（昼）
		param.CrsKbnHiru = rdoNoon.Checked;

		//定期（夜）
		param.CrsKbnYoru = rdoNight.Checked;

		//企画（日帰り）
		param.CrsKbnDay = rdoKikakuDayTrip.Checked;

		//企画（宿泊）
		param.CrsKbnStay = rdoKikakuStay.Checked;

		//企画（Ｒコース）
		param.CrsKbnR = rdoRCrs.Checked;

		//○増含まない
		param.MaruzouHukumanai = rdoMaruzouFukumanai.Checked;

		//○増のみ
		param.MaruzouOnly = rdoMaruzouNomi.Checked;

		return param;
	}
	#endregion

}