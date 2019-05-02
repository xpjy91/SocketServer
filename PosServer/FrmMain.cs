/********************************************************************************************************************
 *  
 *  FileName    : FrmMain.cs
 *  Description : PosServer Main Frame
 *  Date        : 2019. 04. 18
 *  Author      : UniBiz_ParkJIyun
 *  -----------------------------------------------------------------------------------------------------------------
 *  History
 *  
 *  
 * *****************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PosServer
{
    public partial class FrmMain : Form
    {

        private ClsMain clsMain = new ClsMain();
        //서버
        private IPAddress ip = null;
        private IPEndPoint endPoint = null;
        ////Tcp Socket
        private Socket serverSocket = null;
        //Client Socket
        private Socket clientSocket = null;
        //수신 버퍼
        private byte[] receiveBuffer = new byte[1024];
        //송신 버퍼
        private byte[] sendBuffer = null;


        private Thread listenThread;    //Accept()
        private Thread recevieThread;   //Recevie()

        private void Log(string sMsg)
        {
            btnLog.Text = (string.Format("[{0}] {1}", DateTime.Now.ToString(), sMsg));
        }

        //접속요청
        private void Listen()
        {
            try
            {
                this.Invoke(new Action(delegate ()
                {
                    //종단점
                    ip = IPAddress.Parse("127.0.0.1");
                    endPoint = new IPEndPoint(ip, 8000);

                    //소켓생성
                    serverSocket = new Socket(
                        AddressFamily.InterNetwork,
                        SocketType.Stream,
                        ProtocolType.Tcp
                    );

                    //바인드
                    serverSocket.Bind(endPoint);

                    //준비
                    serverSocket.Listen(10);

                    //수신대기
                    // - 여기서 블럭이 걸려야 하지만 스레드로 따로 뺏기때문에 다른 작업이가능
                    Log("클라이언트 요청 대기중..");
                    clientSocket = serverSocket.Accept();
                    Log("클라이언트 접속됨 - " + clientSocket.LocalEndPoint.ToString());

                    //Receive 스레드 호출
                    recevieThread = new Thread(new ThreadStart(Receive));
                    recevieThread.IsBackground = true;
                    recevieThread.Start(); //Receive() 호출
                }));
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
        
        //수신처리..
        private void Receive()
        {
            try
            {
                this.Invoke(new Action(delegate ()
                {
                    //연결된 클라이언트가 보낸 데이터 수신
                    byte[] receiveBuffer = new byte[512];
                    int iLength = clientSocket.Receive(
                        receiveBuffer, 
                        receiveBuffer.Length, 
                        SocketFlags.None
                    );

                    //디코딩
                    String sRevData = Encoding.UTF8.GetString(receiveBuffer, 0, iLength);
                    Console.WriteLine("====================================================");
                    Console.WriteLine("받은 데이터 : " + sRevData);
                    Console.WriteLine("====================================================");

                    txtInput.Text = sRevData;
                    ReceiveProcess(sRevData);

                }));

            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
        }


        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                clsMain.TestDB();
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                ClsDB.OpenDataBase();
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            String sRevData = "";
            int iLength = 0;
            try
            {
                ///* 쓰레드 사용 */
                //if (btnSwitch.Text == "시작")
                //{
                //    btnSwitch.Text = "멈춤";
                //    Log("서버 시작됨");
                //    //Listen스레드 처리
                //    listenThread = new Thread(new ThreadStart(Listen));
                //    listenThread.IsBackground = true;
                //    listenThread.Start();
                //}
                //else
                //{
                //    btnSwitch.Text = "시작";
                //    Log("서버 멈춤");
                //    if (clientSocket != null) clientSocket.Close();
                //    if (serverSocket != null) serverSocket.Close();
                //}



                /* 쓰레드 미사용 */
                if (btnSwitch.Text == "시작")
                {
                    btnSwitch.Text = "멈춤";
                    Log("서버 시작됨");

                    for (int i = 0; i < 3; i++)
                    {
                        //1.종단점 생성(서버쪽..자신)
                        ip = IPAddress.Parse("127.0.0.1");
                        endPoint = new IPEndPoint(ip, 8000);

                        //2. Tcp Socket 생성
                        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        //3. 바인드
                        serverSocket.Bind(endPoint);

                        //4. 대기 큐 설정
                        serverSocket.Listen(10);

                        //5. 연결 대기 -> 접속 후 소켓 생성
                        clientSocket = serverSocket.Accept();

                        //6. 수신 버퍼 생성
                        receiveBuffer = new byte[1024];

                        //7. 데이터 받기
                        iLength = clientSocket.Receive(receiveBuffer, receiveBuffer.Length, SocketFlags.None);

                        //8. 디코딩&출력&서비스처리
                        sRevData = Encoding.UTF8.GetString(receiveBuffer, 0, iLength);
                        Console.WriteLine("받은 데이터 : " + sRevData);
                        txtInput.Text = sRevData;
                        ReceiveProcess(sRevData);

                        //9. 마무리
                        clientSocket.Close();
                        serverSocket.Close();
                    }
                }
                else
                {
                    btnSwitch.Text = "시작";
                    Log("서버 멈춤");
                }


            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void SendMsg(string sData)
        {
            try
            {
                Console.WriteLine("====================================================");
                Console.WriteLine("보낼 데이터 : " + sData);
                Console.WriteLine("====================================================");
                // 데이터
                sendBuffer = Encoding.UTF8.GetBytes(sData);//전송할 데이터를 인코딩,,인자값 : 전송할 데이터
                // 전송
                clientSocket.Send(sendBuffer);
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        private String ReceiveProcess(string sMsg)
        {
            string sRet = null;
            String sGubun = null;           //구분
            try
            {
                sGubun = sMsg.Substring(8, 2);
                switch (sGubun)
                {
                    //TRAN
                    case "00" :
                        sRet = SaveTran(sMsg);
                        break;
                    //CARD INQ
                    case "01":
                        break;
                    //POS INQ
                    case "02":
                        sRet = InquiryPos(sMsg);
                        break;
                    //JOURNAL
                    case "03":
                        break;
                    //DUMMY INQ
                    case "04":
                        sRet = InquiryDummy(sMsg);
                        break;
                    default:
                        break;
                }

                //클라이언트에게 처리메세지 보내줌
                SendMsg(sRet);
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return sRet;
        }

        private String InquiryPos(String sMsg)
        {
            String sGubun = null;   // 아이템ID - 20:Plu Inq
            String sRet = null;
            try
            {
                sGubun = sMsg.Substring(86, 2);
                switch (sGubun)
                {
                    case "10" :
                        /* 운영로그 */
                        sRet = InquiryOperation(sMsg);
                        break;
                    case "20" :
                        /* PLU조회 */
                        sRet = InquiryPlu(sMsg);
                        break;
                    case "30" :
                        /* 거래로그 */
                        sRet = InquiryTran(sMsg);
                        break;
                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return sRet;
        }

        //더미통신
        private String InquiryDummy(String sMsg)
        {
            String sRet = sMsg + "NG";
            int iCnt = 0;
            try
            {
                iCnt = clsMain.TestDB();
                if(iCnt > 0)
                {
                    sRet = sMsg + "OK";
                }
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return sRet;
        }

        //운영로그
        private String InquiryOperation(String sMsg)
        {
            String sHeader = sMsg.Substring(0, 40);          //헤더데이터 (40)
            String sInqHeader = sMsg.Substring(40, 46);      //INQ헤더 (48)
            String sRet = sHeader + sInqHeader;              //return msg
            String sInqData = sMsg.Substring(86);        //운영조회INQ (15)
            String sOperRet = "10";                          //아이템ID  - 10:운영로그

            Dictionary<String, String> dicCashier = new Dictionary<string, String>();   //캐셔정보
            DateTime dtNow = DateTime.Now;
            String sDate = dtNow.ToString("yyyyMMdd");       //영업일자
            String sRspCd = "99";                           //응답코드
            try
            {
                dicCashier = clsMain.SearchCashier("10000001");
                if(dicCashier != null)
                {
                    sRspCd = "00";                       
                    sOperRet += sRspCd;                  //응답코드      (2)      - 00:정상, 99:기타에러
                    sOperRet += sDate;                   //영업일자
                    sOperRet += dicCashier["sNo"].ToString();       //캐셔번호
                    sOperRet += dicCashier["sName"].ToString();     //이름
                    sOperRet += dicCashier["sPass"].ToString();     //비밀번호
                    sOperRet += dicCashier["sLvl"].ToString();      //레벨
                    clsMain.PrintOperation(dicCashier);
                }
                sRet += sOperRet;
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return sRet;
        }

        //거래로그저장
        private String SaveTran(String sMsg)
        {
            Dictionary<String, Object> dicCommand = new Dictionary<string, Object>();
            String sRet = sMsg + "NG";
            /**
                *                      통신헤더
                *                 통신헤더 ( 40byte )
                *        - 전문길이 6    MSG_LEN 을 포함한 길이
                *        - 전문경로 2    PS : POS->SC, SP : SC->POS
                *        - 전문구분 2    0:TRAN, 1:CARD INQ, 2:POS INQ, 3:JOURNAL, 4:DUMMY INQ
                *        - 전문종별 2    00:신카, 01:상품권, 02:포인트
                *        - 전문순번 4
                *        - 점포번호 4
                *        - POS번호  4
                *        - 거래번호 4
                *        - 영업일자 8
                *        - 에러코드 4
                * 
                **/
            String sHeader = sMsg.Substring(0,40);          //헤더데이터 (40)
            clsMain.PrintHeader(sHeader);
            /**
            *                      TRAN헤더
            *                 TRAN헤더 ( 148byte ) , StartByte - 41
            *           - 전문길이  (6)
            *           - 점포번호  (4)
            *           - 포스번호  (4)
            *           - 거래번호  (4)
            *           - 영업일자  (8)
            *           - 시스템일자(8)
            *           - 시스템시각(6)
            *           - 거래구분  (2)
            *           - 거래종별  (2)
            *           - 캐셔번호  (6)
            *           - 원거래일자(8)
            *           - 원포스번호(4)
            *           - 원거래번호(4)
            *           - 원캐셔번호(6)
            *           - 총 매출   (9)
            *           - 총 할인   (9)
            *           - 총 에누리 (9)
            *           - 현금 구분 (1)
            *           - 상품권구분(1)
            *           - 쿠폰 구분 (1)
            *           - 포인트구분(1)
            *           - 신카 구분 (1)
            *           - 체카 구분 (1)
            *           - 할부 구분 (1)
            *           - 카드번호 (20)
            *           - 실송신일자(8)
            *           - 실송신시각(6)
            *           - 최초거래번호(4)
            * 
            **/
            String sTranHeader = sMsg.Substring(40, 148);      //트랜헤더   (148)
            clsMain.PrintTranHeader(sTranHeader);
            String sItemData = null;          //ITEM DATA
            String[] itemList = null;

            String sPayData = null;           //지불 데이터
            String[] payList = null;

            //송신일자,집계일자
            DateTime dtNow = DateTime.Now;          
            String sDate = dtNow.ToString("yyyyMMdd");

            bool bCheck = false;
            try
            {
                //헤더
                sHeader = sMsg.Substring(0, 40);
                //트랜헤더 데이터
                sTranHeader = sMsg.Substring(40, 148);
                //ITEM 데이터
                sItemData = sMsg.Substring((sMsg.IndexOf("ITEM") + 4), (sMsg.LastIndexOf("ITEMEND") - (sMsg.IndexOf("ITEM") + 4)));
                clsMain.PrintItemList(sItemData);
                itemList = sItemData.Split('|');

                dicCommand["sStoreNo"] = sTranHeader.Substring(6, 4);            //점포코드
                dicCommand["sSaleDate"] = sTranHeader.Substring(18, 8);        //영업일
                dicCommand["sPosNo"] = sTranHeader.Substring(10, 4);        //POS번호
                //dicCommand["sTranNo"] = sTranHeader.Substring(24, 4);        //거래번호
                dicCommand["sTranNo"] = clsMain.SearchMaxTranNo();        //거래번호
                dicCommand["sSysdate"] = sTranHeader.Substring(26, 8);        //시스템일자
                dicCommand["sSaleTime"] = sTranHeader.Substring(34, 6);        //시스템시간
                dicCommand["sTrData"] = sMsg;        //거래데이터
                dicCommand["sTranType"] = sTranHeader.Substring(40, 2);        //거래구분
                dicCommand["sTranKind"] = sTranHeader.Substring(42, 2);        //거래종류
                dicCommand["sCashierNo"] = sTranHeader.Substring(44, 6);        //캐셔번호
                dicCommand["sOrgSaleDate"] = sTranHeader.Substring(50, 8);        //원 거래일자
                dicCommand["sOrgPosNo"] = sTranHeader.Substring(58, 4);        //원 POS번호
                dicCommand["sOrgTranNo"] = sTranHeader.Substring(62, 4);        //원 거래번호
                dicCommand["sOrgCashierNo"] = sTranHeader.Substring(66, 6);        //원 캐셔번호
                dicCommand["sItemCnt"] = itemList.Length;        //아이템건수
                dicCommand["sTotAmt"] = sTranHeader.Substring(72, 9);        //총거래금액
                dicCommand["sDisAmt"] = sTranHeader.Substring(81, 9);        //총할인금액
                dicCommand["sCutAmt"] = sTranHeader.Substring(90, 9);        //총에누리금액
                dicCommand["sCashAmt"] = "";        //현금금액
                dicCommand["sCardAmt"] = "";        //신용카드금액
                dicCommand["sCpnAmt"] = "";        //쿠폰금액
                dicCommand["sGiftAmt"] = "";        //상품권금액
                dicCommand["sCashFlag"] = "0";        //현금 FLAG
                dicCommand["sGcFlag"] = "0";        //상품권 FLAG
                dicCommand["sPpFlag"] = "0";        //PP FLAG
                dicCommand["sCouponFlag"] = "0";        //쿠폰 FLAG
                dicCommand["sCardFlag"] = "0";        //신용카드 FLAG
                dicCommand["sDbFlag"] = "0";        //직불카드 FLAG
                dicCommand["sPointFlag"] = "0";        //포인트 FLAG
                dicCommand["sHalbuFlag"] = "0";        //신용카드 할부 FLAG
                dicCommand["sRetFlag"] = "0";        //반품처리 FLAG
                dicCommand["sCustSex"] = "0";        //고객성별
                dicCommand["sCustAge"] = "0";        //고객층
                dicCommand["sUpdFlag"] = "0";        //집계 FLAG
                dicCommand["sUpdDate"] = sDate;        //집계일자
                dicCommand["sSndFlag"] = "1";        //송신 FLAG
                dicCommand["sSndDate"] = sDate;        //송신일자

                //지불 데이터
                //아이템ID (2) - 현금:11,쿠폰:10,상품권:12,신용카드:15
                sPayData = sMsg.Substring((sMsg.IndexOf("PAY") + 3), (sMsg.LastIndexOf("PAYEND") - (sMsg.IndexOf("PAY") + 3)));
                clsMain.PrintPayList(sPayData);
                payList = sPayData.Split('|');

                String sPayType = null;   // 지불타입
                foreach (String sData in payList)
                {
                    sPayType = sData.Substring(0, 2);
                    switch (sPayType)
                    {
                        case "11":
                            //현금
                            dicCommand["sCashAmt"] = sData.Substring(5, 9); //현금금액
                            dicCommand["sCashFlag"] = "1";                  //현금 FLAG
                            break;
                        case "10":
                            //쿠폰
                            dicCommand["sCpnAmt"] = sData.Substring(5, 9);  //쿠폰금액
                            dicCommand["sCouponFlag"] = "1";                //쿠폰 FLAG
                            break;
                        case "12":
                            //상품권
                            dicCommand["sGiftAmt"] = sData.Substring(23, 9);//상품권금액
                            dicCommand["sGcFlag"] = "1";                    //상품권 FLAG
                            break;
                        case "15":
                            //신용카드
                            dicCommand["sCardAmt"] = sData.Substring(57, 8);//신용카드금액
                            dicCommand["sCardFlag"] = "1";                  //신용카드 FLAG
                            break;
                        case "99":
                            //마감입금
                            break;
                    }
                }


                bCheck = clsMain.SaveTranLog(dicCommand);
                if( bCheck == true)
                {
                    sRet = sHeader + "OK";
                }
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            
            return sRet;
        }

        public String InquiryPlu(String sMsg)
        {
            Dictionary<String, String> dicCommand = new Dictionary<string, String>();   // 조회커맨드
            Dictionary<String, String> dicTranLog = new Dictionary<string, String>();       // 조회결과

            /**
              *                      통신헤더
              *                 통신헤더 ( 40byte )
              *        - 전문길이 6    MSG_LEN 을 포함한 길이
              *        - 전문경로 2    PS : POS->SC, SP : SC->POS
              *        - 전문구분 2    0:TRAN, 1:CARD INQ, 2:POS INQ, 3:JOURNAL, 4:DUMMY INQ
              *        - 전문종별 2    00:신카, 01:상품권, 02:포인트
              *        - 전문순번 4
              *        - 점포번호 4
              *        - POS번호  4
              *        - 거래번호 4
              *        - 영업일자 8
              *        - 에러코드 4
              * 
              **/
            String sHeader = sMsg.Substring(0, 40);          //헤더데이터 (40)
            /**
             *                      INQ헤더
             *                 INQ헤더 ( 46byte )
             *         - 전문길이  (6)
             *         - 점포번호  (4)
             *         - 포스번호  (4)
             *         - 거래번호  (4)
             *         - 영업일자  (8)
             *         - 시스템일자(8)
             *         - 시스템시각(6)
             *         - 캐셔번호  (6)
             *
             */
            String sInqHeader = sMsg.Substring(40, 46);      //INQ헤더 (48)
            String sInqData = sMsg.Substring(86, 15);        //PLU조회INQ (15)
            clsMain.PrintInqHeader(sInqHeader);
            clsMain.PrintPluInq(sInqData);

            String sPluRet = "20";  // plu 결과 Msg
            String sRspCd = "99";   //응답코드 -00:정상, 99:기타에러
            String sRet = sHeader + sInqHeader; //return Msg

            /* PLU INQ COMMAND */
            String sStoreNo = "0001";                   //점포코드
            String sPluNo = sInqData.Substring(2, 13);   //상품번호
            try
            {

                dicTranLog = clsMain.SearchMst(sStoreNo, sPluNo);
                /**
                 * PLU 조회 응답데이터
                 * 
                 * 아이템ID         (2) - 20
                 * 응답코드         (2) - 00:정상, 99:기타에러
                 * PLU 코드         (13)
                 * 점포코드         (4)
                 * 거래처코드       (4)
                 * 본부코드         (13)
                 * 분류코드         (15)
                 * 상품구분         (1) - 1:박스,2:보루,3:낱개
                 * 낱개매가         (8)
                 * 박스매가         (8)
                 * 보루매가         (8)
                 * 할인낱개매가     (8)
                 * 할인박스매가     (8)
                 * 할인보루매가     (8)
                 * 우대1낱개매가    (8)
                 * 우대1박스매가    (8)
                 * 우대1보루매가    (8)
                 * 우대2낱개매가    (8)
                 * 우대2박스매가    (8)
                 * 우대2보루매가    (8)
                 * 행사구분         (1) - 7:행사구분
                 * 할인가능여부     (1) - 0:할인가능,1:할인불가
                 * 과세구분         (1) - 0:과세,1:면세,2:영세
                 * 박스입수         (4)
                 * 포인트적립가능구분(1) - 1:적립가능,2:적립불가
                 * 예비             (3)
                 * 연결코드         (13) - 첫자리,0:일반,1:공병,2:쇼핑백,3:P박스
                 * 상품명           (30)
                 * PLU구분          (1) - 1:직영,2:특정
                 * 
                 */

                if (dicTranLog != null)
                {
                    sRspCd = "00";
                    sPluRet += sRspCd;                  //응답코드      (2)      - 00:정상, 99:기타에러
                    sPluRet += dicTranLog["sPluNo"].PadLeft(13,'0');        //상품코드      (13)
                    sPluRet += dicTranLog["sStoreNo"].PadLeft(4, '0');      //점포코드      (4)
                    sPluRet += "8888";          //거래처코드    (4)
                    sPluRet += dicTranLog["sPluNo"].PadLeft(13, '0');        //본부코드      (13)
                    sPluRet += "123456789012345";//분류코드      (15)
                    sPluRet += "3";             //상품구분      (1)      - 1:박스,2:보루,3:낱개
                    sPluRet += dicTranLog["sMaegaUnit"].PadLeft(8, '0');    //낱개매가      (8)
                    sPluRet += dicTranLog["sMaegaUnit"].PadLeft(8, '0');    //박스매가      (8)
                    sPluRet += dicTranLog["sMaegaUnit"].PadLeft(8, '0');    //보루매가      (8)
                    sPluRet += dicTranLog["sMaegaUnitSale"].PadLeft(8, '0');//할인낱개매가  (8)
                    sPluRet += dicTranLog["sMaegaUnitSale"].PadLeft(8, '0');//할인박스매가  (8)
                    sPluRet += dicTranLog["sMaegaUnitSale"].PadLeft(8, '0');//할인보루매가  (8)
                    sPluRet += dicTranLog["sMaegaUnit"].PadLeft(8, '0');    //우대1낱개매가 (8)
                    sPluRet += dicTranLog["sMaegaUnit"].PadLeft(8, '0');    //우대1보루매가 (8)
                    sPluRet += dicTranLog["sMaegaUnit"].PadLeft(8, '0');    //우대2낱개매가 (8)
                    sPluRet += dicTranLog["sMaegaUnit"].PadLeft(8, '0');    //우대2박스매가 (8)
                    sPluRet += dicTranLog["sMaegaUnit"].PadLeft(8, '0');    //우대2보루매가 (8)
                    sPluRet += "0";                     //행사구분      (1)     - 7:행사구분
                    sPluRet += "1";                     //할인가능여부  (1)     - 0:할인가능,1:할인불가
                    sPluRet += dicTranLog["sTaxSep"].PadLeft(1, '0');       //과세구분      (1)     - 0:과세,1:면세,2:영세
                    sPluRet += "0003";                  //박스입수      (4)
                    sPluRet += "2";                     //포인트적립가능구분 (1)- 1:적립가능,2:적립불가
                    sPluRet += "000";                   //예비          (3)
                    sPluRet += dicTranLog["sMlinkPlu"].PadLeft(13, '0');     //연결코드      (13)    - 첫자리,0:일반,1:공병,2:쇼핑백,3:P박스
                    sPluRet += dicTranLog["sPluNameOrd"].PadLeft(30, '0');   //상품명        (30)
                    sPluRet += dicTranLog["sPluSep"].PadLeft(1, '0');       //PLU구분       (1)     - 1:직영,2:특정
                }
                sRet += sPluRet;
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return sRet;
        }

        public String InquiryTran(String sMsg)
        {
            Dictionary<String, String> dicCommand = new Dictionary<string, string>();   // 조회 커맨드
            Dictionary<String, String> dicRet = new Dictionary<string, string>();       // 조회결과
            String sHeader = null;         //헤더데이터 (40)
            String sInqHeader = null;      //INQ헤더 (48)
            String sInqData = null;        //Inq Data (15)
            String sRet = sHeader + sInqHeader; //결과 Ret

            /* TRAN LOG COMMAND */
            String sStoreNo = null;     //점포코드
            String sSaleDate = null;    //영업일자
            String sPosNo = null;       //포스번호
            String sTranNo = null;      //거래번호

            /* Tran Log Result */
            String sTranRet = "30";  // tranLog 결과 Msg
            String sRspCd = "99";   //응답코드 -00:정상, 99:기타에러

            try
            {
                /* 임시 */
                sStoreNo = "0001";
                sSaleDate = "20190426";
                sPosNo = "0001";
                sTranNo = "0001";

                sHeader = sMsg.Substring(0, 40);          //헤더데이터 (40)   
                sInqHeader = sMsg.Substring(40, 46);      //INQ헤더 (48)
                sInqData = sMsg.Substring(86);           //Inq Data

                dicRet = clsMain.SearchTran(sStoreNo, sSaleDate, sPosNo, sTranNo);

                if(dicRet != null)
                {
                    sRspCd = "00";
                    sTranRet += sRspCd;
                    sTranRet += dicRet["sStoreNo"].PadLeft(4, '0'); //점포코드(4)
                    sTranRet += dicRet["sSaleDate"].PadLeft(4, '0');//영업일(8)
                    sTranRet += dicRet["sPosNo"].PadLeft(4, '0');//POS번호(4)
                    sTranRet += dicRet["sTranNo"].PadLeft(4, '0');//거래번호(4)
                    sTranRet += dicRet["sSysDate"].PadLeft(4, '0');//시스템일자(8)
                    sTranRet += dicRet["sSaleTime"].PadLeft(4, '0');//시스템시간(6)
                    sTranRet += dicRet["sTrData"];//거래데이터(TEXT)
                    sTranRet += dicRet["sTranType"].PadLeft(4, '0');//거래구분(2)
                    sTranRet += dicRet["sTranKind"].PadLeft(4, '0');//거래종류(2)
                    sTranRet += dicRet["sCashierNo"].PadLeft(4, '0');//캐셔번호(6)
                    sTranRet += dicRet["sOrgSaleDate"].PadLeft(4, '0');//원 거래일자(8)
                    sTranRet += dicRet["sOrgPosNo"].PadLeft(4, '0');//원 POS번호(4)
                    sTranRet += dicRet["sOrgTranNo"].PadLeft(4, '0');//원 거래번호(4)
                    sTranRet += dicRet["sOrgCashierNo"].PadLeft(4, '0');//원 캐셔번호(6)
                    sTranRet += dicRet["sItemCnt"].PadLeft(4, '0');//아이템건수(9)
                    sTranRet += dicRet["sTotAmt"].PadLeft(4, '0');//총거래금액(9)
                    sTranRet += dicRet["sDisAmt"].PadLeft(4, '0');//총할인금액(9)
                    sTranRet += dicRet["sCutAmt"].PadLeft(4, '0');//총에누리금액(9)
                    sTranRet += dicRet["sCashAmt"].PadLeft(4, '0');//현금금액(9)
                    sTranRet += dicRet["sCardAmt"].PadLeft(4, '0');//신용카드금액(9)
                    sTranRet += dicRet["sCpnAmt"].PadLeft(4, '0');//쿠폰금액(9)
                    sTranRet += dicRet["sGiftAmt"].PadLeft(4, '0');//상품권금액(9)
                    sTranRet += dicRet["sCashFlag"].PadLeft(4, '0');//현금 FLAG(1)
                    sTranRet += dicRet["sGcFlag"].PadLeft(4, '0');//상품권 FLAG(1)
                    sTranRet += dicRet["sPpFlag"].PadLeft(4, '0');//PP FLAG(1)
                    sTranRet += dicRet["sCouponFlag"].PadLeft(4, '0');//쿠폰 FLAG(1)
                    sTranRet += dicRet["sCardFlag"].PadLeft(4, '0');//신용카드 FLAG(1)
                    sTranRet += dicRet["sDbFlag"].PadLeft(4, '0');//직불카드 FLAG(1)
                    sTranRet += dicRet["sPointFlag"].PadLeft(4, '0');//포인트 FLAG(1)
                    sTranRet += dicRet["sHalbuFlag"].PadLeft(4, '0');//신용카드 할부 FLAG(1)
                    sTranRet += dicRet["sRetFlag"].PadLeft(4, '0');//반품처리 FLAG(1)
                    sTranRet += dicRet["sCustSex"].PadLeft(4, '0');//고객성별(1)
                    sTranRet += dicRet["sCustAge"].PadLeft(4, '0');//고객층(1)
                    sTranRet += dicRet["sUpdFlag"].PadLeft(4, '0');//집계 FLAG(1)
                    sTranRet += dicRet["sUpdDate"].PadLeft(4, '0');//집계일자(8)
                    sTranRet += dicRet["sSndFlag"].PadLeft(4, '0');//송신 FLAG(1)
                    sTranRet += dicRet["sSndDate"].PadLeft(4, '0');//송신일자(8)
                }

                sRet += sTranRet;
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return sRet;
        }
    }
}
