using System.ComponentModel;
using C1.Win.C1FlexGrid;
using Hatobus.ReservationManagementSystem.ClientCommon.FlexGridEx;
using Hatobus.ReservationManagementSystem.Master;

/// <summary>
/// 最終確認連絡履歴
/// </summary>
public class S04_0905 : PT99, iPT99
{

	#region フィールド
	private DataTable GridDataTable = null;
	#endregion

	#region S04_0904画面から入力情報
	public S04_0905ParamData ParamData
	{
#endregion

		#region 列挙体
		/// <summary>
		/// カラム名一覧
		/// </summary>
	private enum columnList : int
	{
		[Value("通知日")]
		CONTACT_DAY_STR,
		[Value("予約番号")]
		YOYAKU_NUMBER,
		[Value("名前")]
		NAME,
		[Value("連絡先")]
		CONTENT_INFO,
		[Value("通知結果")]
		CONTENT_RESULT,
		[Value("担当者")]
		CONTENT_PERSON_NAME
	}
	#endregion

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
		//CSV出力ボタン(F4)の表示可否
		base.PtIsCsvOutFlg = false;
		//プレビューボタン(F5)の表示可否
		base.PtIsPrevFlg = false;
		//印刷ボタン(F7)の表示可否
		base.PtIsPrintFlg = false;
		//検索ボタン(F8)の表示可否　※フッターのF8のため検索エリア時はFalseを設定する
		base.PtIsSearchFlg = false;
		//登録ボタン(F10)の表示可否
		base.PtIsRegFlg = false;
		//更新ボタン(F11)の表示可否
		base.PtIsUpdFlg = false;

		//【コントロール系】
		//検索エリアのグループボックスを設定する
		base.PtSearchControl = this.gbxSelect;
		//検索結果エリアのグループボックスを設定する
		base.PtResultControl = this.pnlResult;
		//詳細エリアのグループボックスを設定する
		base.PtDetailControl = null;
		//[検索結果Grid]コントロール設定
		base.PtResultGrid = this.grdList;
		//検索結果を表示するためのGrid
		base.PtDisplayBtn = null;
		//検索結果を表示するためのGrid
		base.PtResultLblCnt = this.lblCount;

		//【データ系】
		//[検索結果Max件数]コントロール設定
		base.PtMaxCount = 1000;
		//結果グリッドの最大表示件数
		//MyBase.PtResultDT = -
		//※ReadOnly 結果グリッドの選択行データ
		//PtSelectRow = -

		//【その他】
		//実装画面(インターフェースを実装フォーム)
		base.PtMyForm = this;
		//変更チェックを行うDataTableのカラム名(ID)を設定
		base.PtDiffChkColName = null;

		//【帳票用】
		//AR/DS選択用プロパティ
		base.PtPrintType = null;
		//呼び出しDaTaStudioID
		base.PtDsPrintId = null;
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
		if (area == area.SEARCH)
		{
			//引渡しされたパラメータを設定する
			//※ パラメータはIF(IN)シート参照
			this.txtFromSyuptDay.Text = ParamData.SyuptDayFrom.ToString().Substring(0, 4) + "/" + ParamData.SyuptDayFrom.ToString().Substring(4, 2) + "/" + ParamData.SyuptDayFrom.ToString().Substring(6, 2) + "/";
			this.txtToSyuptDay.Text = ParamData.SyuptDayTo.ToString().Substring(0, 4) + "/" + ParamData.SyuptDayTo.ToString().Substring(4, 2) + "/" + ParamData.SyuptDayTo.ToString().Substring(6, 2) + "/";
			this.txtCrsCd.Text = ParamData.CrsCd;
			this.txtCrsName.Text = ParamData.CrsName;
		}
		else if (area == area.RESULT)
		{
			//・検索結果エリアの初期設定
			//「画面項目定義」のNo.2の項目を対象に、初期値記載された内容に準じて、項目の初期化を行う
			//grdの設定処理
			base.CmnInitGrid(grdList);
			// grd初期設定
			InitializeResultGrid();
			//検索処理を呼び出す
			base.btnF8_ClickOrgProc();
		}
		else if (area == area.DETAIL)
		{
			//なし
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
		//TRUEを戻す
		return true;
	}

	#endregion
	#region Privateメソッド(画面独自)
	//return<summary>
	//returnGrid列初期設定
	//return</summary>
	private void InitializeResultGrid()
	{
		this.grdList.Initialize(GetGridTitle());
		this.grdList.Cols(columnList.CONTACT_DAY_STR).TextAlign = C1.Win.C1FlexGrid.TextAlignEnum.CenterCenter;
	}

	private List[] GetGridTitle()
	{
		return new List[Of GridProperty] From {
			;
			//			new GridProperty (){.Caption = getEnumAttrValue(columnList.CONTACT_DAY_STR),.Name == columnList.CONTACT_DAY_STR.ToString(),.Width == 108},;
			//			new GridProperty (){.Caption = getEnumAttrValue(columnList.YOYAKU_NUMBER),.Name == columnList.YOYAKU_NUMBER.ToString(),.Width == 229},;
			//			new GridProperty (){.Caption = getEnumAttrValue(columnList.NAME),.Name == columnList.NAME.ToString(),.Width == 256},;
			//			new GridProperty (){.Caption = getEnumAttrValue(columnList.CONTENT_INFO),.Name == columnList.CONTENT_INFO.ToString(),.Width == 318},;
			//			new GridProperty (){.Caption = getEnumAttrValue(columnList.CONTENT_RESULT),.Name == columnList.CONTENT_RESULT.ToString(),.Width == 185},;
			//			new GridProperty (){.Caption = getEnumAttrValue(columnList.CONTENT_PERSON_NAME),.Name == columnList.CONTENT_PERSON_NAME.ToString(),.Width == 143};
			//			};
		}

#endregion

		#region 画面に実装(Overrides)
		/// <summary>
		/// 共通の初期処理を呼び出す
		/// </summary>
	protected override void OvPt99Init()
	{
		//初期化処理(PT99)
		base.OvPt99Init();
		//・初期フォーカスを出発日に設定する
		this.ActiveControl = this.txtFromSyuptDay;
	}

	#region 検索処理用
	///return<summary>
	///return検索前チェック処理
	///return</summary>
	///return<returns></returns>
	//Protected Overrides Function OvPt99SearchBefore() As Boolean
	//    'TODO:画面項目定義(詳細)＜検索前処理＞
	//    Return MyBase.OvPt99SearchBefore()
	//End Function

	/// <summary>
	/// データ取得処理
	/// </summary>
	/// <returns></returns>
	protected override DataTable OvPt99SearchGetData()
	{
		//DA定義No1参照
		//DataAccessクラス生成
		S04_0905DA dataAccess = new S04_0905DA();
		S04_0905DA.S04_0905DASelectParam param = new S04_0905DA.S04_0905DASelectParam();
		//'パラメータに各値を設定
		//パラメータに各値を設定
		//出発日FROM
		if (!ReferenceEquals(this.txtFromSyuptDay, null))
		{
			param.SyuptDayFrom = ParamData.SyuptDayFrom;
		}
		//出発日TO
		if (!ReferenceEquals(this.txtToSyuptDay, null))
		{
			param.SyuptDayTo = ParamData.SyuptDayTo;
		}
		//コースコード
		param.CrsCd = ParamData.CrsCd;
		GridDataTable = dataAccess.selectDataTable(param);
		return GridDataTable;
	}

	/// <summary>
	/// グリッドデータの取得とグリッド表示
	/// </summary>
	protected override void OvPt99SearchAfter()
	{
		for (int row = this.grdList.Rows.Fixed; row <= this.grdList.Rows.Count - this.grdList.Rows.Fixed; row++)
		{
			if (grdList.GetData(row, columnList.YOYAKU_NUMBER.ToString()).ToString().Equals("0,000,000,000"))
			{
				this.grdList.SetData(row, columnList.YOYAKU_NUMBER.ToString(), "0");
			}
		}
		//TODO:画面項目定義(詳細)＜検索後処理＞
	}
	#endregion

	#region 更新処理用
	///return<summary>
	///return更新前チェック処理
	///return</summary>
	///return<returns></returns>
	//Protected Overrides Function checkUpdateItems() As Boolean
	//    'TODO:画面項目定義(詳細)＜更新前処理＞
	//    Return True
	//End Function

	///return<summary>
	///return更新処理
	///return</summary>
	///return<returns></returns>
	//Protected Overrides Function updateTranData() As Integer
	//    'TODO:画面項目定義(詳細)＜更新処理＞
	//    Return 1
	//End Function
	#endregion

	#region 出力処理用
	///return<summary>
	///return出力前(エラーチェック)処理
	///return</summary>
	///return<returns></returns>
	//Protected Overrides Function OvPt99PrintBefore() As Boolean
	//    'TODO:画面項目定義(詳細)＜出力前処理＞

	//    Return MyBase.OvPt99PrintBefore()
	//End Function

	///return<summary>
	///returnDS出力時(パラメータ設定処理)処理
	///return</summary>
	///return<returns></returns>
	//Protected Overrides Function OvPt99PrintSetDSParameter() As Boolean
	//    'TODO:DSWEB帳票設計書

	//    Return MyBase.OvPt99PrintSetDSParameter()
	//End Function

	///return<summary>
	///returnAR出力時(データ取得)処理
	///return</summary>
	///return<returns></returns>
	//Protected Overrides Function OvPt99PrintARGetData() As DataTable
	//    'TODO:画面項目定義(詳細)＜出力処理＞(ARの場合)

	//    Return MyBase.OvPt99PrintARGetData()
	//End Function

	///return<summary>
	///return出力後(データ加工等)処理
	///return</summary>
	//Protected Overrides Sub OvPt99printAfter()
	//    'TODO:画面項目定義(詳細)＜出力後処理＞

	//End Sub
	#endregion

	#region PT99外ファンクションキー
	///return<summary>
	///returnFXXボタン押下時の独自処理
	///return</summary>
	//Protected Overrides Sub btnFXX_ClickOrgProc()
	//End Sub
	#endregion
	#endregion

	#region Privateメソッド(イベント)

	#endregion

}