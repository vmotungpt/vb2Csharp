using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;


/// <summary>
///
/// </summary>
public class ConPayTG
{
	
#region 定数/変数
	
	/// <summary>
	/// PayTG送信ステータス：成功
	/// </summary>
	public const string PayTgSendStatusSucess = "0";
	/// <summary>
	/// PayTG送信ステータス：接続失敗
	/// </summary>
	public const string PayTgSendStatusConnectFailure = "1";
	/// <summary>
	/// PayTG送信ステータス：要求電文不正
	/// </summary>
	public const string PayTgSendStatusSendDataFailure = "2";
	/// <summary>
	/// PayTG送信ステータス：要求電文送信失敗
	/// </summary>
	public const string PayTgSendStatusSendFailure = "3";
	/// <summary>
	/// PayTG送信ステータス：Bcc送信失敗
	/// </summary>
	public const string PayTgSendStatusSendBccFailure = "4";
	/// <summary>
	/// PayTG送信ステータス：PayTG送信キャンセル
	/// </summary>
	public const string PayTgSendStatusSendCancelFailure = "9";
	/// <summary>
	/// PayTG送信ステータス：Exception
	/// </summary>
	public const string PayTgSendStatusSendException = "99";
	
	/// <summary>
	/// 決済ログメッセージ
	/// </summary>
	public const string LogMessageSendKessaiData = "決済データ送信 {0} OrderID：{1}";
	/// <summary>
	/// 開始
	/// </summary>
	public const string KessaiStart = "開始";
	/// <summary>
	/// キャンセル
	/// </summary>
	public const string KessaiCancel = "キャンセル";
	/// <summary>
	/// 失敗
	/// </summary>
	public const string KessaiFailure = "失敗";
	/// <summary>
	/// 完了
	/// </summary>
	public const string KessaiCompleted = "完了";
	/// <summary>
	/// 画面パラメータ
	/// </summary>
	/// <returns></returns>
	public ConPayTGParamData ParamData
	{
		/// <summary>
		/// 決済ステータスフラグ
		/// </summary>
		public bool KessaiStatus;
		
		/// <summary>
		/// 受信XMLデータ
		/// </summary>
		private string xmlText = "";
		/// <summary>
		/// DB登録フラグ
		/// </summary>
		private bool isDbRegistFlag = false;
		/// <summary>
		/// キャンセルフラグ
		/// </summary>
		private bool isCancelFlag = false;
		
		private System.Windows.Forms.Timer closeTimer;
		/// <summary>
		/// 画面クローズフラグ
		/// </summary>
		private bool isCloseFlag = false;
		
#endregion
		
#region Enum
		
		/// <summary>
		/// ASCII コード制御文字
		/// </summary>
		private enum ASCIISignConst : byte
		{
			
			/// <summary>テキスト開始</summary>
			STX = 0x2,
			/// <summary>テキスト終了</summary>
			ETX = 0x3,
			/// <summary>転送終了</summary>
			EOT = 0x4,
			/// <summary>受信OK</summary>
			ACK = 0x6,
			/// <summary>受信失敗</summary>
			NAK = 0x15,
			/// <summary>転送ブロック終了</summary>
			ETB = 0x17
		}
		
#endregion
		
#region イベント
		
		/// <summary>
		/// 画面ロードイベント
		/// </summary>
		/// <param name="sender">イベント送信元</param>
		/// <param name="e">イベントデータ</param>
		private void ConPayTG_Load(object sender, EventArgs e)
		{
			
			if (this.ParamData.IsKessai == true)
			{
				
				this.lblMessage.Text = "決済情報送信中";
			}
			else
			{
				
				this.lblMessage.Text = "取消決済情報送信中";
			}
			
			this.lblInputOrderId.Text = this.ParamData.Request.OrderId;
			this.lblInputAmount.Text = this.ParamData.Request.Amount.ToString("#,##0");
		}
		
		/// <summary>
		/// 画面表示後イベント
		/// </summary>
		/// <param name="sender">イベント送信元</param>
		/// <param name="e">イベントデータ</param>
		private void ConPayTG_Shown(object sender, EventArgs e)
		{
			
			this.closeTimer = new System.Windows.Forms.Timer();
			this.closeTimer.Interval = 1000;
			this.closeTimer.Enabled = true;
			
			this.closeTimer.Tick += new EventHandler(closeTimer_Tick);
			
			try
			{
				
				if (this.ParamData.IsKessai == true)
				{
					// 決済の場合
					
					this.KessaiStatus = this.sendKessaiData();
					
					if (this.KessaiStatus == false)
					{
						// 結果がFalseの場合、画面を閉じる
						this.Close();
					}
				}
				else
				{
					// 取消の場合
					
					// 前回予約情報（決済）取得
					DataTable bfKessaiInfo = CommonKessai.getYoyakuInfoKessai(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
					int bfAmount = System.Convert.ToInt32(CommonKessai.getBfKessaiAmount(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo));
					
					this.KessaiStatus = this.torikeshiKessai(bfKessaiInfo, bfAmount, System.Convert.ToDateTime(this.ParamData.RegistDate));
					
					this.Close();
				}
			}
			catch (Exception ex)
			{
				
				string[] strErr;
				strErr = new string[] {ex.Message, ex.Source, ex.StackTrace};
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, this.ParamData.FormId + ":" + this.ParamData.ScreenName, strErr);
				
				this.Close();
				return ;
			}
		}
		
		/// <summary>
		/// シリアルポート受信イベント
		/// </summary>
		/// <param name="sender">イベント送信元</param>
		/// <param name="e">イベントデータ</param>
		private void slpPayTg_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			
			// 決済情報受信
			string revStatus = this.ReceiveKessaiData();
			
			if (revStatus != PayTgSendStatusSucess)
			{
				
				CommonProcess.createFactoryMsg().messageDisp("E90_025", "決済", "決済情報送信失敗");
				this.KessaiStatus = false;
				this.isCloseFlag = true;
				return ;
			}
			
			// キャンセルされた場合、ここで終了
			if (this.isCancelFlag == true)
			{
				
				MessageBox.Show("キャンセルしました。");
				//CommonProcess.createFactoryMsg().messageDisp("Exx_xxx", "")
				this.KessaiStatus = false;
				this.isCloseFlag = true;
				return ;
			}
			
			// ここで、強制終了不可にする
			// ここまできたら、受信ができているので、DB書き込み処理を優先にする
			this.isDbRegistFlag = true;
			
			// 応答電文をデシアライズ
			ResponseUriageEntity response = CommonKessai.convertXmlToEntity(Of ResponseUriageEntity)[this.xmlText];
			
			if (response.ComResult != FixedCdYoyaku.ComResultStatus.Sucess)
			{
				// 正常終了以外の場合、メッセージを表示し、処理終了
				this.dispPayTgResultMessage(System.Convert.ToString(response.ComResult));
				this.KessaiStatus = false;
				this.isCloseFlag = true;
				return ;
			}
			
			// 前回予約情報（決済）取得
			DataTable bfKessaiInfo = CommonKessai.getYoyakuInfoKessai(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo);
			int bfAmount = System.Convert.ToInt32(CommonKessai.getBfKessaiAmount(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo));
			
			// 予約情報（決済）登録
			bool kessaiResult = System.Convert.ToBoolean(CommonKessai.registYoyakuInfoSettlementForUriage(this.ParamData.YoyakuKbn,);
			Me.ParamData.YoyakuNo (,);
			Me.ParamData.SyuptDay (,);
			Me.ParamData.Request (,);
			response (,);
			Me.ParamData.RegistDate (,);
			Me.ParamData.FormId ());
			
			if (kessaiResult == false)
			{
				
				CommonProcess.createFactoryMsg().messageDisp("E90_025", "決済", "予約情報（決済）登録");
				this.KessaiStatus = false;
				this.isCloseFlag = true;
				return ;
			}
			
			if (response.Mstatus != FixedCdYoyaku.MStatus.Sucess)
			{
				
				CommonProcess.createFactoryMsg().messageDisp("E90_025", "決済", "決済情報送信失敗");
				this.KessaiStatus = false;
				this.isCloseFlag = true;
				return ;
			}
			
			// 予約関連登録
			string uriageUpdateStatus = System.Convert.ToString(CommonKessai.registYoyakuInfoUriage(this.ParamData.YoyakuKbn,);
			Me.ParamData.YoyakuNo (,);
			Me.ParamData.SyuptDay (,);
			Me.ParamData.TeikiKikakuKbn (,);
			Me.ParamData.Request (,);
			response (,);
			Me.ParamData.RegistDate (,);
			Me.ParamData.FormId ());
			
			if (this.isegistKessaiStatusCheck(uriageUpdateStatus) == false)
			{
				
				this.KessaiStatus = false;
				this.isCloseFlag = true;
				return ;
			}
			
			if (bfKessaiInfo.Rows.Count > 0)
			{
				// 前回決済した情報がある場合、前回分の決済を取消する
				
				if (this.torikeshiKessai(bfKessaiInfo, bfAmount, System.Convert.ToDateTime(this.ParamData.RegistDate)) == false)
				{
					
					this.KessaiStatus = false;
					this.isCloseFlag = true;
					return ;
				}
			}
			
			this.KessaiStatus = true;
			this.isCloseFlag = true;
		}
		
		/// <summary>
		/// フォームキーダウンイベント
		/// </summary>
		/// <param name="sender">イベント送信元</param>
		/// <param name="e">イベントデータ</param>
		protected void FormBase_KeyDown(object sender, KeyEventArgs e)
		{
			
			// TODO：キャンセルボタンはなにか？
			//       予定では同時3つのボタン押下でキャンセルさせる
			if (e.KeyData == Keys.Escape)
			{
				
				if (this.isDbRegistFlag == true)
				{
					
					// TODO：キャンセルできない旨のメッセージを表示するべきか？
					return ;
				}
				
				this.isCancelFlag = true;
				
				this.closePort();
				this.Close();
			}
		}
		
		/// <summary>
		///
		/// </summary>
		/// <param name="sender">イベント送信元</param>
		/// <param name="e">イベントデータ</param>
		private void closeTimer_Tick(object sender, EventArgs e)
		{
			
			if (this.isCloseFlag == true)
			{
				
				this.closePort();
				this.Close();
			}
		}
		
#endregion
		
#region メソッド
		
		/// <summary>
		/// シリアルポート接続
		/// </summary>
		/// <returns>接続成功時：true それ以外：false</returns>
		private bool openPort()
		{
			
			try
			{
				string port = this.getComProtName();
				
				if (string.IsNullOrWhiteSpace(port) == true)
				{
					
					return false;
				}
				
				// 設定
				this.slpPayTg = new SerialPort();
				this.slpPayTg.PortName = port;
				this.slpPayTg.BaudRate = 115200;
				this.slpPayTg.DataBits = 8;
				this.slpPayTg.Parity = Parity.None;
				this.slpPayTg.StopBits = StopBits.One;
				this.slpPayTg.Encoding = Encoding.UTF8;
				this.slpPayTg.WriteTimeout = 100000;
				
				// 接続
				this.slpPayTg.Open();
				
			}
			catch (Exception)
			{
				
				// シリアルポート切断
				this.closePort();
				return false;
			}
			
			return true;
		}
		
		/// <summary>
		/// 通信確認
		/// </summary>
		/// <returns>検証結果</returns>
		private bool canSend()
		{
			
			if (this.slpPayTg IsNot null && this.slpPayTg.IsOpen == true)
			{
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// シリアルポート切断
		/// </summary>
		/// <returns>切断成功：true それ以外：false</returns>
		private bool closePort()
		{
			
			if (ReferenceEquals(this.slpPayTg, null))
			{
				
				return false;
			}
			
			// 切断
			this.slpPayTg.Close();
			this.slpPayTg = null;
			
			return true;
		}
		
		/// <summary>
		/// ポート名取得
		/// </summary>
		/// <returns>ポート名</returns>
		private string getComProtName()
		{
			
			string query = "Select * from Win32_PNPEntity Where (Name like '%(COM%)')";
			
			Regex chk = new Regex("(COM[1-9][0-9]?[0-9]?)");
			
			ManagementObjectSearcher mos = new ManagementObjectSearcher();
			mos.Query.QueryString = query;
			
			ManagementObjectCollection moc = mos.Get();
			
			string portName = "";
			foreach (ManagementBaseObject m in moc)
			{
				
				string value = (string) (m.GetPropertyValue("Name"));
				
				if (string.IsNullOrEmpty(value) == true)
				{
					
					continue;
				}
				
				if (value.Contains("Castles CDC USB To UART") == false)
				{
					
					// Windows10の場合、下記の名称でドライバー名となる
					if (value.Contains("USB シリアル デバイス") == false)
					{
						
						continue;
					}
				}
				
				if (chk.IsMatch(value) == false)
				{
					
					continue;
				}
				
				Match match = chk.Match(value);
				portName = System.Convert.ToString(match.Captures(0).Value);
			}
			
			return portName;
		}
		
		/// <summary>
		/// 決済情報送信
		/// </summary>
		/// <returns>送信結果</returns>
		private bool sendKessaiData()
		{
			
			// シリアルポート接続
			if (this.openPort() == false)
			{
				
				MessageBox.Show("PayTGに接続できませんでした。");
				//CommonProcess.createFactoryMsg().messageDisp("Exx_xxx", "")
				return false;
			}
			
			// 決済情報送信
			// XML変換
			object sendText = CommonKessai.convertEntityToXml(Of RequestUriageEntity)[this.ParamData.Request];
			System.Byte sendTextByte = Encoding.UTF8.GetBytes((char[]) (sendText.Replace("\\r", "").Replace("\\n", "")));
			
			List sendList = new List(Of byte);
			sendList.Add(ASCIISignConst.STX);
			sendList.AddRange(sendTextByte);
			sendList.Add(ASCIISignConst.ETX);
			sendList.Add(this.calculationBCC(sendTextByte));
			
			// PayTG端末にデータ送信
			bool isSuccess = this.sendToPayTG(sendList);
			
			if (isSuccess == false)
			{
				
				CommonProcess.createFactoryMsg().messageDisp("E90_025", "決済", "決済情報送信失敗");
			}
			
			return isSuccess;
		}
		
		/// <summary>
		/// Blolk Check Character の計算
		/// </summary>
		/// <remarks>
		/// このメソッドはSTX の次からETX 直前までのデータから計算します
		/// なのでdata にはETX を含めないでください
		/// 計算方法は"Pay TG端末PC連動仕様書.1.0.1.docx" を参照してください
		/// </remarks>
		private byte calculationBCC(IEnumerable[] data)
		{
			
			byte bcc = (byte) (0xFF);
			
			foreach (byte item in data)
			{
				
				bcc = bcc ^ item;
			}
			
			bcc = bcc ^ (int) ASCIISignConst.ETX;
			
			return bcc;
		}
		
		/// <summary>
		/// PayTG端末にデータ送信
		/// </summary>
		/// <param name="sendData">送信データ</param>
		/// <returns>送信結果</returns>
		private bool sendToPayTG(List[] sendData)
		{
			
			if (ReferenceEquals(sendData, null) || sendData.Count() <= 0)
			{
				// 送信データがない場合は何もしない
				return false;
			}
			
			try
			{
				
				slpPayTg.Write(sendData.ToArray(), 0, sendData.Count());
			}
			catch (Exception)
			{
				
				return false;
			}
			
			return true;
		}
		
		/// <summary>
		/// 決済情報受信
		/// </summary>
		/// <returns>受信結果</returns>
		private string ReceiveKessaiData()
		{
			
			byte[] buffer = new byte[2];
			bool isEscape = false;
			List receiveData = new List(Of byte);
			
			try
			{
				do
				{
					object length = slpPayTg.Read(buffer, 0, 1);
					if (length < 1)
					{
						
						// データが受信できていない場合は、一定時間待機してリトライ
						Thread.Sleep(20);
						continue;
					}
					
					// 受信データの解析
					if ((buffer[0]) == ASCIISignConst.STX)
					{
						// 応答電文受信時
						
						// Pay TG 端末から受信したデータの取得
						object item = slpPayTg.ReadByte();
						while (item != (int) ASCIISignConst.ETX)
						{
							
							receiveData.Add(System.Convert.ToByte(item));
							item = slpPayTg.ReadByte();
						}
						
						// BCC の判定
						List code = new List(Of byte);
						object bcc = slpPayTg.ReadByte();
						System.Byte check = calculationBCC(receiveData);
						if (bcc == check)
						{
							// 受け取り成功時
							code.Add(ASCIISignConst.ACK);
							
							string result = Encoding.UTF8.GetString(receiveData.ToArray());
							this.xmlText = result;
						}
						else
						{
							// 受け取り失敗時
							isEscape = true;
							code.Add(ASCIISignConst.NAK);
							
							// 送信した旨をログに書き出し
							this.writeLogMessage(LogKindType.operationLog, ProcessKindType.sonota, System.Convert.ToString(this.ParamData.FormId), System.Convert.ToString(this.ParamData.ScreenName), KessaiFailure, System.Convert.ToString(this.ParamData.Request.OrderId));
						}
						
						// BCC 判定結果の送信
						this.sendToPayTG(code);
					}
					else if ((buffer[0]) == ASCIISignConst.EOT)
					{
						// Pay TG 端末からの応答電文送信完了時
						isEscape = true;
						
						// 送信した旨をログに書き出し
						this.writeLogMessage(LogKindType.operationLog, ProcessKindType.sonota, System.Convert.ToString(this.ParamData.FormId), System.Convert.ToString(this.ParamData.ScreenName), KessaiCompleted, System.Convert.ToString(this.ParamData.Request.OrderId));
					}
					else if ((buffer[0]) == ASCIISignConst.ACK)
					{
						// Pay TG 端末への送信とBCC 成功時
						List sendACK = new List(Of byte);
						sendACK.Add(ASCIISignConst.EOT);
						this.sendToPayTG(sendACK);
					}
					else if ((buffer[0]) == ASCIISignConst.NAK)
					{
						// Pay TG 端末への送信したがBCC 失敗時
						// 送信した旨をログに書き出し
						this.writeLogMessage(LogKindType.operationLog, ProcessKindType.sonota, System.Convert.ToString(this.ParamData.FormId), System.Convert.ToString(this.ParamData.ScreenName), KessaiFailure, System.Convert.ToString(this.ParamData.Request.OrderId));
						return PayTgSendStatusSendBccFailure;
					}
				} while (!isEscape);
			}
			catch (Exception ex)
			{
				
				string[] strErr;
				strErr = new string[] {ex.Message, ex.Source, ex.StackTrace};
				this.writeLogMessage(LogKindType.errorLog, ProcessKindType.sonota, System.Convert.ToString(this.ParamData.FormId), System.Convert.ToString(this.ParamData.ScreenName), KessaiFailure, System.Convert.ToString(this.ParamData.Request.OrderId));
				return PayTgSendStatusSendException;
			}
			
			return PayTgSendStatusSucess;
		}
		
		/// <summary>
		/// Pay TG 端末連動結果メッセージ表示
		/// </summary>
		/// <param name="resCode">Pay TG 端末連動結果</param>
		private void dispPayTgResultMessage(string resCode)
		{
			
			string msg = "";
			
			if (resCode == FixedCdYoyaku.ComResultStatus.RejectedReceipt)
			{
				
				msg = string.Format("{0}：{1}", FixedCdYoyaku.ComResultStatus.RejectedReceipt, "要求電文受付拒否");
			}
			else if (resCode == FixedCdYoyaku.ComResultStatus.SendCanceled)
			{
				
				msg = string.Format("{0}：{1}", FixedCdYoyaku.ComResultStatus.SendCanceled, "処理中止");
			}
			else if (resCode == FixedCdYoyaku.ComResultStatus.NoCertificate)
			{
				
				msg = string.Format("{0}：{1}", FixedCdYoyaku.ComResultStatus.NoCertificate, "初期認証/ペアリングエラー");
			}
			else if (resCode == FixedCdYoyaku.ComResultStatus.ReceiveDataFraud)
			{
				
				msg = string.Format("{0}：{1}", FixedCdYoyaku.ComResultStatus.ReceiveDataFraud, "要求電文不正");
			}
			else if (resCode == FixedCdYoyaku.ComResultStatus.NewworkErr)
			{
				
				msg = string.Format("{0}：{1}", FixedCdYoyaku.ComResultStatus.NewworkErr, "ネットワークエラー");
			}
			else if (resCode == FixedCdYoyaku.ComResultStatus.MobileRelatedErr)
			{
				
				msg = string.Format("{0}：{1}", FixedCdYoyaku.ComResultStatus.MobileRelatedErr, "モバイル関連エラー");
			}
			
			CommonProcess.createFactoryMsg().messageDisp("E90_025", "決済", msg);
		}
		
		/// <summary>
		/// 決済ステータスチェック
		/// </summary>
		/// <param name="kessaiStatus">決済ステータス</param>
		/// <returns>検証結果</returns>
		private bool isegistKessaiStatusCheck(string kessaiStatus)
		{
			
			if (kessaiStatus == CommonKessai.RegistKessaiStatusSucess)
			{
				
				return true;
			}
			
			string message = "";
			
			if (kessaiStatus == CommonKessai.RegistKessaiStatusNyukinFailure)
			{
				
				message = "決済登録失敗";
			}
			else if (kessaiStatus == CommonKessai.RegistKessaiStatusHistoryFailure)
			{
				
				message = "変更履歴登録失敗";
			}
			else if (kessaiStatus == CommonKessai.RegistKessaiStatusYoyakuInfoBasicFailure)
			{
				
				message = "予約情報（基本）登録失敗";
			}
			else if (kessaiStatus == CommonKessai.RegistKessaiStatusYoyakuInfo2Failure)
			{
				
				message = "予約情報２登録失敗";
			}
			else if (kessaiStatus == CommonKessai.RegistKessaiStatusYoyakuInfoCrsChargeFailure)
			{
				
				message = "予約情報（コース料金）登録失敗";
			}
			else if (kessaiStatus == CommonKessai.RegistKessaiStatusYoyakuInfoCrsChargeChargeKbnFailure)
			{
				
				message = "予約情報（コース料金_料金区分）登録失敗";
			}
			
			CommonProcess.createFactoryMsg().messageDisp("E90_025", "決済", message);
			
			return false;
		}
		
		/// <summary>
		/// 取消決済
		/// </summary>
		/// <param name="bfKessaiInfo">前回予約情報（決済）情報</param>
		/// <param name="bfAmount">前回決済金額</param>
		/// <param name="registDate">登録日</param>
		/// <returns>取消結果</returns>
		private bool torikeshiKessai(DataTable bfKessaiInfo, int bfAmount, DateTime registDate)
		{
			
			string bfOrderId = System.Convert.ToString(CommonRegistYoyaku.convertObjectToString(bfKessaiInfo.Rows(0)["ORDER_ID"]).Trim());
			
			// キャンセル決済処理
			HatobusCreditAPI.CancelWebServiceInputBean input = null;
			CancelWebServiceOutputEntity output = CommonKessai.cancelKessai(bfOrderId, this.ParamData.HojinGaikyakuKbn, input, this.ParamData.FormId, this.ParamData.ScreenName);
			
			if (output.ResultCode != FixedCdYoyaku.TorikeshiKessaiResult.Sucess)
			{
				
				// 送信失敗した旨をログに書き出し
				createFactoryLog.logOutput(LogKindType.errorLog, ProcessKindType.sonota, this.ParamData.FormId + ":" + this.ParamData.ScreenName, string.Format("決済取消データ送信失敗 OrderID：{0}", input.orderID));
				CommonProcess.createFactoryMsg().messageDisp("E90_025", "決済", "取消送信失敗");
				return false;
			}
			
			// 送信成功した旨をログに書き出し
			createFactoryLog.logOutput(LogKindType.operationLog, ProcessKindType.entry, this.ParamData.FormId + ":" + this.ParamData.ScreenName, string.Format("決済取消データ送信成功 OrderID：{0}", input.orderID));
			
			// 予約情報（決済）更新
			bool kessaiResult = System.Convert.ToBoolean(CommonKessai.updateYoyakuInfoSettlementForTorikeshi(this.ParamData.YoyakuKbn, this.ParamData.YoyakuNo, bfOrderId, output, registDate, this.ParamData.FormId));
			if (kessaiResult == false)
			{
				
				CommonProcess.createFactoryMsg().messageDisp("E90_025", "決済", "予約情報（決済）更新");
				return false;
			}
			
			// 予約関連登録
			string torikeshiUpdateStatus = System.Convert.ToString(CommonKessai.registYoyakuInfoTorikeshi(this.ParamData.YoyakuKbn,);
			Me.ParamData.YoyakuNo (,);
			Me.ParamData.SyuptDay (,);
			bfAmount (,);
			Me.ParamData.TeikiKikakuKbn (,);
			input (,);
			output (,);
			registDate (,);
			Me.ParamData.FormId ());
			if (this.isegistKessaiStatusCheck(torikeshiUpdateStatus) == false)
			{
				
				return false;
			}
			
			return true;
		}
		
		/// <summary>
		/// ログ書き出し
		/// </summary>
		/// <param name="opeLog">オペレーションログ</param>
		/// <param name="processType">ログ処理種別タイプ</param>
		/// <param name="formId">画面ID</param>
		/// <param name="screenName">画面名</param>
		/// <param name="message">メッセージ</param>
		/// <param name="orderId">オーダーID</param>
		private void writeLogMessage(LogKindType opeLog, ProcessKindType processType, string formId, string screenName, string message, string orderId)
		{
			
			string msg = string.Format(LogMessageSendKessaiData, message, orderId);
			// ログ書き出し
			createFactoryLog.logOutput(opeLog, processType, formId + ":" + screenName, msg);
		}
		
#endregion
		
	}