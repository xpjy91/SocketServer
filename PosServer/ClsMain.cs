/********************************************************************************************************************
 *  
 *  FileName    : ClsMain.cs
 *  Description : Main Class
 *  Date        : 2019. 04. 18
 *  Author      : UniBiz_ParkJIyun
 *  -----------------------------------------------------------------------------------------------------------------
 *  History
 *  
 *  
 * *****************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PosServer
{
    class ClsMain
    {   
        ClsDB clsDb = new ClsDB();


        public int TestDB()
        {
            string sQuery = "SELECT 1";
            SqlDataReader sqlRead = null;
            int iRet = -1;
            try
            {
                sqlRead = clsDb.GetData(sQuery);
                if (sqlRead.Read() == true)
                {
                    iRet = (int)sqlRead[0];
                }
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                if ( sqlRead != null)
                {
                    sqlRead.Close();
                }
            }
            return iRet;
        }

        public bool SaveTranLog(Dictionary<String,Object> tranCommand)
        {
            bool bRet = false;
            try
            {
                string sQuery = "INSERT INTO TRANLOG " +
                                        "(" +
                                           "STORE_NO" +
                                           ", SALE_DATE" +
                                           ", POS_NO" +
                                           ", TRAN_NO" +
                                           ", SYS_DATE" +
                                           ", SALE_TIME" +
                                           ", TR_DATA" +
                                           ", TRAN_TYPE" +
                                           ", TRAN_KIND" +
                                           ", CASHIER_NO" +
                                           ", ORG_SALE_DATE" +
                                           ", ORG_POS_NO" +
                                           ", ORG_TRAN_NO" +
                                           ", ORG_CASHIER_NO" +
                                           ", ITEM_CNT" +
                                           ", TOT_AMT" +
                                           ", DIS_AMT" +
                                           ", CUT_AMT" +
                                           ", CASH_AMT" +
                                           ", CARD_AMT" +
                                           ", CPN_AMT" +
                                           ", GIFT_AMT" +
                                           ", CASH_FLAG" +
                                           ", GC_FLAG" +
                                           ", PP_FLAG" +
                                           ", COUPON_FLAG" +
                                           ", CARD_FLAG" +
                                           ", DB_FLAG" +
                                           ", POINT_FLAG" +
                                           ", HALBU_FLAG" +
                                           ", RET_FLAG" +
                                           ", CUST_SEX" +
                                           ", CUST_AGE" +
                                           ", UPD_FLAG" +
                                           ", UPD_DATE" +
                                           ", SND_FLAG" +
                                           ", SND_DATE" +
                                        ")" +
                                "VALUES " +
                                        "(" +
                                            "'" + tranCommand["sStoreNo"] + "', " +
                                            "'" + tranCommand["sSaleDate"] + "', " +
                                            "'" + tranCommand["sPosNo"] + "', " +
                                            "'" + tranCommand["sTranNo"] + "', " +
                                            "'" + tranCommand["sSysdate"] + "', " +
                                            "'" + tranCommand["sSaleTime"] + "', " +
                                            "'" + tranCommand["sTrData"] + "', " +
                                            "'" + tranCommand["sTranType"] + "', " +
                                            "'" + tranCommand["sTranKind"] + "', " +
                                            "'" + tranCommand["sCashierNo"] + "', " +
                                            "'" + tranCommand["sOrgSaleDate"] + "', " +
                                            "'" + tranCommand["sOrgPosNo"] + "', " +
                                            "'" + tranCommand["sOrgTranNo"] + "', " +
                                            "'" + tranCommand["sOrgCashierNo"] + "', " +
                                            "'" + tranCommand["sItemCnt"] + "', " +
                                            "'" + tranCommand["sTotAmt"] + "', " +
                                            "'" + tranCommand["sDisAmt"] + "', " +
                                            "'" + tranCommand["sCutAmt"] + "', " +
                                            "'" + tranCommand["sCashAmt"] + "', " +
                                            "'" + tranCommand["sCardAmt"] + "', " +
                                            "'" + tranCommand["sCpnAmt"] + "', " +
                                            "'" + tranCommand["sGiftAmt"] + "', " +
                                            "'" + tranCommand["sCashFlag"] + "', " +
                                            "'" + tranCommand["sGcFlag"] + "', " +
                                            "'" + tranCommand["sPpFlag"] + "', " +
                                            "'" + tranCommand["sCouponFlag"] + "', " +
                                            "'" + tranCommand["sCardFlag"] + "', " +
                                            "'" + tranCommand["sDbFlag"] + "', " +
                                            "'" + tranCommand["sPointFlag"] + "', " +
                                            "'" + tranCommand["sHalbuFlag"] + "', " +
                                            "'" + tranCommand["sRetFlag"] + "', " +
                                            "'" + tranCommand["sCustSex"] + "', " +
                                            "'" + tranCommand["sCustAge"] + "', " +
                                            "'" + tranCommand["sUpdFlag"] + "', " +
                                            "'" + tranCommand["sUpdDate"] + "', " +
                                            "'" + tranCommand["sSndFlag"] + "', " +
                                            "'" + tranCommand["sSndDate"] + "'" +
                                        ")";

                clsDb.ExecuteSql(sQuery);
                bRet = true;
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return bRet;
        }

        public Dictionary<string, string> SearchMst(String sStoreNo, String sPluNo)
        {
            SqlDataReader sqlRead = null;
            Dictionary<string, string> dicMst = null;
            string sQuery = null;

            try
            {
                sQuery = "SELECT store_no                   as sStoreNo," +
                                "plu_no                     as sPluNo, " +
                                "dept_no                    as sDeptNo, " +
                                "trade_no                   as sTradeNo, " +
                                "class_no                   as sClassNo, " +
                                "plu_name                   as sPluName, " +
                                "receipt_name               as sReceiptName, " +
                                "instore_no                 as sInstoreNo, " +
                                "link_plu                   as sLinkPlu, " +
                                "mlink_plu                  as sMlinkPlu, " +
                                "ROUND(wonga_unit, 0)       as sWongaUnit, " +
                                "ROUND(wonga_unitsale, 0)   as sWongaUnitSale, " +
                                "ROUND(maega_unit, 0)       as sMaegaUnit, " +
                                "ROUND(maega_unitsale, 0)   as sMaegaUnitSale, " +
                                "plu_sep                    as sPluSep, " +
                                "tax_sep                    as sTaxSep, " +
                                "brand_sep                  as sBrandSep, " +
                                "maker_no                   as sMakerNo, " +
                                "price_code                 as sPriceCode, " +
                                "max_ext                    as sMaxExt, " +
                                "min_ext                    as sMinExt, " +
                                "upd_flag                   as sUpdFlag, " +
                                "sale_flag                  as sSaleFlag, " +
                                "box_qty                    as sBoxQty, " +
                                "source_sep                 as sSourceSep, " +
                                "ext_loc1                   as sExtLoc1, " +
                                "ext_loc2                   as sExtLoc2, " +
                                "ext_loc3                   as sExtLoc3, " +
                                "inv_loc                    as sInvLoc, " +
                                "ord_unit                   as sOrdUnit, " +
                                "dell_sep                   as sDellSep, " +
                                "pur_sep                    as sPurSep, " +
                                "ROUND(profit_rate, 0)      as sProfitRate, " +
                                "md_flag                    as sMdFlag, " +
                                "jeul_flag                  as sJeulFlag, " +
                                "jeul_price                 as sJeulPrice, " +
                                "min_ordqty                 as sMinOrdQty, " +
                                "plu_name_pop               as sPluNamePop, " +
                                "plu_name_ord               as sPluNameOrd, " +
                                "maker_name                 as sMakerName, " +
                                "standard                   as sStandard, " +
                                "origin_region              as sOriginRegion, " +
                                "wonbj_flag                 as sWonbjFlag, " +
                                "logi_min_ordqty            as sLogiMinOrdQty, " +
                                "ord_method                 as sOrdMethod, " +
                                "logi_flag                  as sLogiFlag, " +
                                "cigar_drink_flag           as sCigarDrinkFlag, " +
                                "sku_no                     as sSkuNo, " +
                                "ord_end_dt                 as sOrdEndDt, " +
                                "sale_end_dt                as sSaleEndDt, " +
                                "bottle_flag                as sBottleFlag, " +
                                "safe_day_cnt               as sSafeDayCnt, " +
                                "ord_posbl_flag             as sOrdPosblFlag, " +
                                "con_disp_unit              as sConDispUnit, " +
                                "reg_date                   as sRegDate, " +
                                "reg_emp_no                 as sRegEmpNo, " +
                                "chg_date                   as sChgDate, " +
                                "chg_emp_no                 as sChgEmpNo" +
                        " FROM plumst" +
                        " WHERE store_no = '" + sStoreNo + "'" +
                            "AND plu_no = '" + sPluNo + "'";

                sqlRead = clsDb.GetData(sQuery);

                while (sqlRead.Read() == true)
                {
                    dicMst = new Dictionary<string, string>();
                    dicMst["sStoreNo"] = sqlRead["sStoreNo"].ToString();
                    dicMst["sPluNo"] = sqlRead["sPluNo"].ToString();
                    dicMst["sDeptNo"] = sqlRead["sStoreNo"].ToString();
                    dicMst["sTradeNo"] = sqlRead["sTradeNo"].ToString();
                    dicMst["sClassNo"] = sqlRead["sClassNo"].ToString();
                    dicMst["sPluName"] = sqlRead["sPluName"].ToString();
                    dicMst["sReceiptName"] = sqlRead["sReceiptName"].ToString();
                    dicMst["sInstoreNo"] = sqlRead["sInstoreNo"].ToString();
                    dicMst["sLinkPlu"] = sqlRead["sLinkPlu"].ToString();
                    dicMst["sMlinkPlu"] = sqlRead["sMlinkPlu"].ToString();
                    dicMst["sWongaUnit"] = sqlRead["sWongaUnit"].ToString();
                    dicMst["sWongaUnitSale"] = sqlRead["sWongaUnitSale"].ToString();
                    dicMst["sMaegaUnit"] = sqlRead["sMaegaUnit"].ToString();
                    dicMst["sMaegaUnitSale"] = sqlRead["sMaegaUnitSale"].ToString();
                    dicMst["sPluSep"] = sqlRead["sPluSep"].ToString();
                    dicMst["sTaxSep"] = sqlRead["sTaxSep"].ToString();
                    dicMst["sBrandSep"] = sqlRead["sBrandSep"].ToString();
                    dicMst["sMakerNo"] = sqlRead["sMakerNo"].ToString();
                    dicMst["sPriceCode"] = sqlRead["sPriceCode"].ToString();
                    dicMst["sMaxExt"] = sqlRead["sMaxExt"].ToString();
                    dicMst["sMinExt"] = sqlRead["sMinExt"].ToString();
                    dicMst["sUpdFlag"] = sqlRead["sUpdFlag"].ToString();
                    dicMst["sSaleFlag"] = sqlRead["sSaleFlag"].ToString();
                    dicMst["sBoxQty"] = sqlRead["sBoxQty"].ToString();
                    dicMst["sSourceSep"] = sqlRead["sSourceSep"].ToString();
                    dicMst["sExtLoc1"] = sqlRead["sExtLoc1"].ToString();
                    dicMst["sExtLoc2"] = sqlRead["sExtLoc2"].ToString();
                    dicMst["sExtLoc3"] = sqlRead["sExtLoc3"].ToString();
                    dicMst["sInvLoc"] = sqlRead["sInvLoc"].ToString();
                    dicMst["sOrdUnit"] = sqlRead["sOrdUnit"].ToString();
                    dicMst["sDellSep"] = sqlRead["sDellSep"].ToString();
                    dicMst["sPurSep"] = sqlRead["sPurSep"].ToString();
                    dicMst["sProfitRate"] = sqlRead["sProfitRate"].ToString();
                    dicMst["sMdFlag"] = sqlRead["sMdFlag"].ToString();
                    dicMst["sJeulFlag"] = sqlRead["sJeulFlag"].ToString();
                    dicMst["sJeulPrice"] = sqlRead["sJeulPrice"].ToString();
                    dicMst["sMinOrdQty"] = sqlRead["sMinOrdQty"].ToString();
                    dicMst["sPluNamePop"] = sqlRead["sPluNamePop"].ToString();
                    dicMst["sPluNameOrd"] = sqlRead["sPluNameOrd"].ToString();
                    dicMst["sMakerName"] = sqlRead["sMakerName"].ToString();
                    dicMst["sStandard"] = sqlRead["sStandard"].ToString();
                    dicMst["sOriginRegion"] = sqlRead["sOriginRegion"].ToString();
                    dicMst["sWonbjFlag"] = sqlRead["sWonbjFlag"].ToString();
                    dicMst["sLogiMinOrdQty"] = sqlRead["sLogiMinOrdQty"].ToString();
                    dicMst["sOrdMethod"] = sqlRead["sOrdMethod"].ToString();
                    dicMst["sLogiFlag"] = sqlRead["sLogiFlag"].ToString();
                    dicMst["sCigarDrinkFlag"] = sqlRead["sCigarDrinkFlag"].ToString();
                    dicMst["sSkuNo"] = sqlRead["sSkuNo"].ToString();
                    dicMst["sOrdEndDt"] = sqlRead["sOrdEndDt"].ToString();
                    dicMst["sSaleEndDt"] = sqlRead["sSaleEndDt"].ToString();
                    dicMst["sBottleFlag"] = sqlRead["sBottleFlag"].ToString();
                    dicMst["sSafeDayCnt"] = sqlRead["sSafeDayCnt"].ToString();
                    dicMst["sOrdPosblFlag"] = sqlRead["sOrdPosblFlag"].ToString();
                    dicMst["sConDispUnit"] = sqlRead["sConDispUnit"].ToString();
                    dicMst["sRegDate"] = sqlRead["sRegDate"].ToString();
                    dicMst["sRegEmpNo"] = sqlRead["sRegEmpNo"].ToString();
                    dicMst["sChgDate"] = sqlRead["sChgDate"].ToString();
                    dicMst["sChgEmpNo"] = sqlRead["sChgEmpNo"].ToString();
                }
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                if (sqlRead.IsClosed != true)
                {
                    sqlRead.Close();
                }
            }
            return dicMst;
        }

        public Dictionary<string, string> SearchTran(string sStoreNo, string sSaleDate, string sPosNo, string sTranNo)
        {
            SqlDataReader sqlRead = null;
            Dictionary<string, string> dicTranLog = null;
            string sQuery = null;

            try
            {
                sQuery = "SELECT STORE_NO           AS sStroeNo " +
                              ",SALE_DATE           AS sSaleDate " +
                              ",POS_NO              AS sPosNo " +
                              ",TRAN_NO             AS sTranNo " +
                              ",SYS_DATE            AS sSysDate " +
                              ",SALE_TIME           AS sSaleTime " +
                              ",TR_DATA             AS sTrData " +
                              ",TRAN_TYPE           AS sTranType " +
                              ",TRAN_KIND           AS sTranKind " +
                              ",CASHIER_NO          AS sCashierNo " +
                              ",ORG_SALE_DATE       AS sOrgSaleDate " +
                              ",ORG_POS_NO          AS sOrgPosNo " +
                              ",ORG_TRAN_NO         AS sOrgTranNo " +
                              ",ORG_CASHIER_NO      AS sOrgCashierNo " +
                              ",ITEM_CNT            AS sItemCnt " +
                              ",TOT_AMT             AS sTotAmt " +
                              ",DIS_AMT             AS sDisAmt " +
                              ",CUT_AMT             AS sCutAmt " +
                              ",CASH_AMT            AS sCashAmt " +
                              ",CARD_AMT            AS sCardAmt " +
                              ",CPN_AMT             AS sCpnAmt " +
                              ",GIFT_AMT            AS sGiftAmt " +
                              ",CASH_FLAG           AS sCashFlag " +
                              ",GC_FLAG             AS sGcFlag " +
                              ",PP_FLAG             AS sPpFlag " +
                              ",COUPON_FLAG         AS sCouponFlag " +
                              ",CARD_FLAG           AS sCardFlag " +
                              ",DB_FLAG             AS sDbFlag " +
                              ",POINT_FLAG          AS sPointFlag " +
                              ",HALBU_FLAG          AS sHalbuFlag " +
                              ",RET_FLAG            AS sRetFlag " +
                              ",CUST_SEX            AS sCustSex " +
                              ",CUST_AGE            AS sCustAge " +
                              ",UPD_FLAG            AS sUpdFlag " +
                              ",UPD_DATE            AS sUpdDate " +
                              ",SND_FLAG            AS sSndFlag " +
                              ",SND_DATE            AS sSndDate " +
                         " FROM TRANLOG" +
                         " WHERE STORE_NO = '" + sStoreNo + "'" +
                            " AND SALE_DATE = '" + sSaleDate + "'" +
                            " AND POS_NO = '" + sPosNo + "'" +
                            " AND TRAN_NO = '" + sTranNo + "'";
                sqlRead = clsDb.GetData(sQuery);

                while (sqlRead.Read() == true)
                {
                    dicTranLog = new Dictionary<string, string>();
                    dicTranLog["sStoreNo"] = sqlRead["sStoreNo"].ToString();
                    dicTranLog["sPluNo"] = sqlRead["sPluNo"].ToString();
                    dicTranLog["sDeptNo"] = sqlRead["sStoreNo"].ToString();
                    dicTranLog["sTradeNo"] = sqlRead["sTradeNo"].ToString();
                    dicTranLog["sClassNo"] = sqlRead["sClassNo"].ToString();
                    dicTranLog["sPluName"] = sqlRead["sPluName"].ToString();
                    dicTranLog["sReceiptName"] = sqlRead["sReceiptName"].ToString();
                    dicTranLog["sInstoreNo"] = sqlRead["sInstoreNo"].ToString();
                    dicTranLog["sLinkPlu"] = sqlRead["sLinkPlu"].ToString();
                    dicTranLog["sMlinkPlu"] = sqlRead["sMlinkPlu"].ToString();
                    dicTranLog["sWongaUnit"] = sqlRead["sWongaUnit"].ToString();
                    dicTranLog["sWongaUnitSale"] = sqlRead["sWongaUnitSale"].ToString();
                    dicTranLog["sMaegaUnit"] = sqlRead["sMaegaUnit"].ToString();
                    dicTranLog["sMaegaUnitSale"] = sqlRead["sMaegaUnitSale"].ToString();
                    dicTranLog["sPluSep"] = sqlRead["sPluSep"].ToString();
                    dicTranLog["sTaxSep"] = sqlRead["sTaxSep"].ToString();
                    dicTranLog["sBrandSep"] = sqlRead["sBrandSep"].ToString();
                    dicTranLog["sMakerNo"] = sqlRead["sMakerNo"].ToString();
                    dicTranLog["sPriceCode"] = sqlRead["sPriceCode"].ToString();
                    dicTranLog["sMaxExt"] = sqlRead["sMaxExt"].ToString();
                    dicTranLog["sMinExt"] = sqlRead["sMinExt"].ToString();
                    dicTranLog["sUpdFlag"] = sqlRead["sUpdFlag"].ToString();
                    dicTranLog["sSaleFlag"] = sqlRead["sSaleFlag"].ToString();
                    dicTranLog["sBoxQty"] = sqlRead["sBoxQty"].ToString();
                    dicTranLog["sSourceSep"] = sqlRead["sSourceSep"].ToString();
                    dicTranLog["sExtLoc1"] = sqlRead["sExtLoc1"].ToString();
                    dicTranLog["sExtLoc2"] = sqlRead["sExtLoc2"].ToString();
                    dicTranLog["sExtLoc3"] = sqlRead["sExtLoc3"].ToString();
                    dicTranLog["sInvLoc"] = sqlRead["sInvLoc"].ToString();
                    dicTranLog["sOrdUnit"] = sqlRead["sOrdUnit"].ToString();
                    dicTranLog["sDellSep"] = sqlRead["sDellSep"].ToString();
                    dicTranLog["sPurSep"] = sqlRead["sPurSep"].ToString();
                    dicTranLog["sProfitRate"] = sqlRead["sProfitRate"].ToString();
                    dicTranLog["sMdFlag"] = sqlRead["sMdFlag"].ToString();
                    dicTranLog["sJeulFlag"] = sqlRead["sJeulFlag"].ToString();
                    dicTranLog["sJeulPrice"] = sqlRead["sJeulPrice"].ToString();
                    dicTranLog["sMinOrdQty"] = sqlRead["sMinOrdQty"].ToString();
                    dicTranLog["sPluNamePop"] = sqlRead["sPluNamePop"].ToString();
                    dicTranLog["sPluNameOrd"] = sqlRead["sPluNameOrd"].ToString();
                    dicTranLog["sMakerName"] = sqlRead["sMakerName"].ToString();
                    dicTranLog["sStandard"] = sqlRead["sStandard"].ToString();
                    dicTranLog["sOriginRegion"] = sqlRead["sOriginRegion"].ToString();
                    dicTranLog["sWonbjFlag"] = sqlRead["sWonbjFlag"].ToString();
                    dicTranLog["sLogiMinOrdQty"] = sqlRead["sLogiMinOrdQty"].ToString();
                    dicTranLog["sOrdMethod"] = sqlRead["sOrdMethod"].ToString();
                    dicTranLog["sLogiFlag"] = sqlRead["sLogiFlag"].ToString();
                    dicTranLog["sCigarDrinkFlag"] = sqlRead["sCigarDrinkFlag"].ToString();
                    dicTranLog["sSkuNo"] = sqlRead["sSkuNo"].ToString();
                    dicTranLog["sOrdEndDt"] = sqlRead["sOrdEndDt"].ToString();
                    dicTranLog["sSaleEndDt"] = sqlRead["sSaleEndDt"].ToString();
                    dicTranLog["sBottleFlag"] = sqlRead["sBottleFlag"].ToString();
                    dicTranLog["sSafeDayCnt"] = sqlRead["sSafeDayCnt"].ToString();
                    dicTranLog["sOrdPosblFlag"] = sqlRead["sOrdPosblFlag"].ToString();
                    dicTranLog["sConDispUnit"] = sqlRead["sConDispUnit"].ToString();
                    dicTranLog["sRegDate"] = sqlRead["sRegDate"].ToString();
                    dicTranLog["sRegEmpNo"] = sqlRead["sRegEmpNo"].ToString();
                    dicTranLog["sChgDate"] = sqlRead["sChgDate"].ToString();
                    dicTranLog["sChgEmpNo"] = sqlRead["sChgEmpNo"].ToString();
                }
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                if (sqlRead.IsClosed != true)
                {
                    sqlRead.Close();
                }
            }

            return dicTranLog;
        }

        public String SearchMaxTranNo()
        {
            SqlDataReader sqlRead = null;
            String bRet = "0";
            String sQuery = null;
            try
            {
                //sQuery = "SELECT REPLICATE('0', (4 - LEN((SELECT max(TRAN_NO) +1 from tranlog where sale_date = CONVERT(varchar, getdate(), 112)))))"  +
                //         "+ (SELECT TRIM(CONVERT(CHAR, (SELECT max(TRAN_NO) + 1 from tranlog where sale_date = CONVERT(varchar, getdate(), 112)))))";

                sQuery =
                    "SELECT REPLICATE('0', (4 - LEN( " +
                    "( " +
                        "SELECT TRIM(CONVERT(CHAR,( " +
                        "SELECT " +
                        "CASE " +
                            "WHEN(SELECT max(TRAN_NO) from tranlog where sale_date = CONVERT(varchar, getdate(), 112)) is null THEN '1' " +
                            "ELSE(SELECT max(TRAN_NO) + 1 from tranlog where sale_date = CONVERT(varchar, getdate(), 112)) " +
                        "END " +
                        "))) " +
                    ") " +
                "))) " +
                " + " +
                "( " +
                    "SELECT TRIM(CONVERT(CHAR, ( " +
                    "SELECT " +
                    "CASE " +
                        "WHEN(SELECT max(TRAN_NO) from tranlog where sale_date = CONVERT(varchar, getdate(), 112)) is null THEN '1' " +
                        "ELSE(SELECT max(TRAN_NO) + 1 from tranlog where sale_date = CONVERT(varchar, getdate(), 112)) " +
                    "END " +
                    "))) " +
                ") ";
                sqlRead = clsDb.GetData(sQuery);

                while (sqlRead.Read() == true)
                {
                    bRet = sqlRead[0].ToString();
                }
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                if (sqlRead.IsClosed != true)
                {
                    sqlRead.Close();
                }
            }
            return bRet;
        }


        public void PrintHeader(String sMsg)
        {

            String sMsgLength = sMsg.Substring(0, 6);          //전문길이
            String sPath = sMsg.Substring(6, 2);            //전문경로
            String sType = sMsg.Substring(8, 2);            //전문구분
            String sKind = sMsg.Substring(10, 2);           //전문종별
            String sSeqNo = sMsg.Substring(12, 2);          //전문순번
            String sStoreNo = sMsg.Substring(16, 4);        //점포번호
            String sPosNo = sMsg.Substring(20, 2);          //포스번호      
            String sTranNo = sMsg.Substring(24, 4);         //거래번호
            String sSaleDate = sMsg.Substring(28, 8);       //영업일자
            String sErrorCd = sMsg.Substring(36, 4);        //에러코드

            try
            {
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
                Console.WriteLine("==================== 통신헤더 데이터 ====================");
                Console.WriteLine("통신 DATA =============> [" + sMsg + "]");
                Console.WriteLine("전문길이(sMsgLength : 0,2) =============> [" + sMsg.Substring(0, 2) + "]");
                Console.WriteLine("전문경로(sPath : 6,2) =============> [" + sMsg.Substring(6, 2) + "]");
                Console.WriteLine("전문구분(sType : 8,2) =============> [" + sMsg.Substring(8, 2) + "]");
                Console.WriteLine("전문종별(sKind : 10,2) =============> [" + sMsg.Substring(10, 2) + "]");
                Console.WriteLine("전문순번(sSeqNo : 12,4) =============> [" + sMsg.Substring(12, 4) + "]");
                Console.WriteLine("점포번호(sStoreNo : 16,4) =============> [" + sMsg.Substring(16, 4) + "]");
                Console.WriteLine("POS 번호(sPosNo : 20,4) =============> [" + sMsg.Substring(20, 4) + "]");
                Console.WriteLine("거래번호(sTranNo : 24,4) =============> [" + sMsg.Substring(24, 4) + "]");
                Console.WriteLine("영업일자(sSaleDate : 28,8) =============> [" + sMsg.Substring(28, 8) + "]");
                Console.WriteLine("에러코드(sErrorCd : 36,4) =============> [" + sMsg.Substring(36, 4) + "]");
                Console.WriteLine("==================== 통신헤더 데이터 ====================");
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void PrintTranHeader(String sMsg)
        {

            String sMsgLength = sMsg.Substring(0, 6);           //전문길이
            String sStorNo = sMsg.Substring(6, 4);              //점포번호
            String sPosNo = sMsg.Substring(10, 4);              //포스번호
            String sTranNo = sMsg.Substring(14, 4);             //거래번호
            String sSaleDate = sMsg.Substring(18, 8);           //영업일자
            String sSysDate = sMsg.Substring(26, 8);            //시스템일자
            String sSysTime = sMsg.Substring(34, 6);            //시스템시각
            String sTranType = sMsg.Substring(40, 2);           //거래구분
            String sTranKind = sMsg.Substring(42, 2);           //거래종별
            String sCashierNo = sMsg.Substring(44, 6);          //캐셔번호
            String sOrgSaleDate = sMsg.Substring(50, 8);        //원 거래일자
            String sOrgPosNo = sMsg.Substring(58, 4);           //원 포스번호
            String sOrgTranNo = sMsg.Substring(62, 4);          //원 거래번호
            String sOrgCashierNo = sMsg.Substring(66, 6);       //원 캐셔번호
            String sTotAmt = sMsg.Substring(72, 9);             //총 매출
            String sDcAmt = sMsg.Substring(81, 9);              //총 할인
            String sCutAmt = sMsg.Substring(90, 9);             //총 에누리
            String sCashFlag = sMsg.Substring(99, 1);           //현금구분
            String sGcFlag = sMsg.Substring(100, 1);            //상품권구분
            String sCouponFlag = sMsg.Substring(101, 1);        //쿠폰구분
            String sPointFlag = sMsg.Substring(102, 1);         //포인트구분
            String sCardFlag = sMsg.Substring(103, 1);          //신용카드구분
            String sCheckFlag = sMsg.Substring(104, 1);         //체크카드구분
            String sHalbuFlag = sMsg.Substring(105, 1);         //할부구분
            String sCardCompNo = sMsg.Substring(106, 4);        //카드회사번호
            String sCardNo = sMsg.Substring(110, 20);           //카드번호
            String sSendDate = sMsg.Substring(130, 8);          //실송신일자
            String sSendTime = sMsg.Substring(138, 6);          //실송신시각
            String sStartTranNo = sMsg.Substring(144, 4);       //최초거래번호



            try
            {
                /**
                *                      TRAN헤더
                *                 TRAN헤더 ( 148byte )
                *                 StartByte - 41
                *     전문길이  (6)
                *     점포번호  (4)
                *     포스번호  (4)
                *     거래번호  (4)
                *     영업일자  (8)
                *     시스템일자(8)
                *     시스템시각(6)
                *     거래구분  (2)
                *     거래종별  (2)
                *     캐셔번호  (6)
                *     원거래일자(8)
                *     원포스번호(4)
                *     원거래번호(4)
                *     원캐셔번호(6)
                *     총 매출   (9)
                *     총 할인   (9)
                *     총 에누리 (9)
                *     현금 구분 (1)
                *     상품권구분(1)
                *     쿠폰 구분 (1)
                *     포인트구분(1)
                *     신카 구분 (1)
                *     체카 구분 (1)
                *     할부 구분 (1)
                *     카드번호 (20)
                *     실송신일자(8)
                *     실송신시각(6)
                *     최초거래번호(4)
                * 
                **/

                Console.WriteLine("==================== TRAN헤더 데이터 ====================");
                Console.WriteLine("TRAN DATA =============> [" + sMsg + "]");
                Console.WriteLine("전문길이(sMsgLength : 0,6) =============> [" + sMsgLength + "]");
                Console.WriteLine("점포번호(sStorNo : 6,4) =============> [" + sStorNo + "]");
                Console.WriteLine("포스번호(sPosNo : 10,4) =============> [" + sPosNo + "]");
                Console.WriteLine("거래번호(sTranNo : 14,4) =============> [" + sTranNo + "]");
                Console.WriteLine("영업일자(sSaleDate : 18,8) =============> [" + sSaleDate + "]");
                Console.WriteLine("시스템일자(sSysDate : 26,8) =============> [" + sSysDate + "]");
                Console.WriteLine("시스템시각(sSysTime : 34,6) =============> [" + sSysTime + "]");
                Console.WriteLine("거래구분(sType : 40,2) =============> [" + sTranType + "]");
                Console.WriteLine("거래종별(sKind : 42,2) =============> [" + sTranKind + "]");
                Console.WriteLine("캐셔번호(sCashierNo : 44,6) =============> [" + sCashierNo + "]");
                Console.WriteLine("원 거래일자(sOrgDate : 50,8) =============> [" + sOrgSaleDate + "]");
                Console.WriteLine("원 포스번호(sOrgPosNo : 58,4) =============> [" + sOrgPosNo + "]");
                Console.WriteLine("원 거래번호(sOrgTranNo : 62,4) =============> [" + sOrgTranNo + "]");
                Console.WriteLine("원 캐셔번호(sOrgCashierNo : 66,6) =============> [" + sOrgCashierNo + "]");
                Console.WriteLine("총 매출(sTotAmt : 72,9) =============> [" + sTotAmt + "]");
                Console.WriteLine("총 할인(sDcAmt : 81,9) =============> [" + sDcAmt + "]");
                Console.WriteLine("총 에누리(sCutAmt : 90,9) =============> [" + sCutAmt + "]");
                Console.WriteLine("현금구분(sCashFlag : 99,1) =============> [" + sCashFlag + "]");
                Console.WriteLine("상품권구분(sGcFlag : 100,1) =============> [" + sGcFlag + "]");
                Console.WriteLine("쿠폰구분(sCouponFlag : 101,1) =============> [" + sCouponFlag + "]");
                Console.WriteLine("포인트구분(sPointFlag : 102,1) =============> [" + sPointFlag + "]");
                Console.WriteLine("신용카드구분(sCardFlag : 103,1) =============> [" + sCardFlag + "]");
                Console.WriteLine("체크카드구분(sCheckFlag : 104,1) =============> [" + sCheckFlag + "]");
                Console.WriteLine("할부구분(sHalbuFlag : 105,1) =============> [" + sHalbuFlag + "]");
                Console.WriteLine("카드회사번호(sCardCompNo : 106,4) =============> [" + sCardCompNo + "]");
                Console.WriteLine("카드번호(sCardNo : 110,20) =============> [" + sCardNo + "]");
                Console.WriteLine("실송신일자(sSendDate : 130,8) =============> [" + sSendDate + "]");
                Console.WriteLine("실송신시각(sSendTime : 138,6) =============> [" + sSendTime + "]");
                Console.WriteLine("최초거래번호(sStartTranNo : 144,4) =============> [" + sStartTranNo + "]");
                Console.WriteLine("==================== TRAN헤더 데이터 ====================");
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void PrintPayList(String sMsg)
        {
            String[] sList = sMsg.Split('|');
            String sGubun = ""; //지불방법 (2) - 현금:11,쿠폰:10,상품권:12,신용카드:15
            String sItemId = null;          //아이템ID
            String sItemLen = null;         //아이템길이
            String sTranType = null;        //거래구분
            String sTranKind = null;        //거래종별
            String sHostId = null;          //승인밴사
            String sMsgSeqNo = null;        //전문일련번호
            String sCardNo = null;          //신용카드번호
            String sExpDate = null;         //유효기간
            String sPassWd = null;          //비밀번호
            String sHalbuMonth = null;      //할부개월
            String sSaleAmt = null;         //판매금액
            String sAppNo = null;           //승인번호
            String sWcc = null;             //입력구분
            String sCardData = null;        //신용카드 데이터
            String sAppDate = null;         //승인일자
            String sAppTime = null;          //승인시각
            String sIssCode = null;         //발급사코드
            String sPurCode = null;         //매입사코드
            String sRegNo = null;           //가맹점번호
            String sTotPoint = null;        //합계포인트
            String sTranPoint = null;       //거래포인트
            String sValPoint = null;        //유효포인트
            String sCardType = null;        //카드구분 - 0:일반,1:월드패스
            String sBagAmt = null;          //공병/쇼핑백금액

            String sCashAmt = null;         //현금금액
            String sChange = null;          //거스름

            try
            {
                foreach (String sData in sList)
                {
                    sGubun = sData.Substring(0, 2);
                    switch (sGubun)
                    {
                        case "11":
                            //현금

                            /**
                             *                       CASH_ITEM
                             *                 현금 ITEM ( 30 Byte )
                             *      아이템ID (2)
                             *      아이템길이 (3)
                             *      현금금액 (9)
                             *      거스름 (8)
                             *      공병/쇼핑백 금액 (8)
                             *      
                             **/

                            sItemId = sData.Substring(0, 2);      //아이템ID
                            sItemLen = sData.Substring(2, 3);    //아이템길이
                            sCashAmt = sData.Substring(5, 9);    //현금금액
                            sChange = sData.Substring(14, 8);    //거스름
                            sBagAmt = sData.Substring(22, 8);    //공병/쇼핑백금액
                            Console.WriteLine("==================== 현금ITEM 데이터 ====================");
                            Console.WriteLine("ITEM DATA =============> [" + sData + "]");
                            Console.WriteLine("아이템ID(sItemId : 0,2) =============> [" + sItemId + "]");
                            Console.WriteLine("아이템길이(sItemLen : 6,4) =============> [" + sItemLen + "]");
                            Console.WriteLine("현금금액(sCashAmt : 10,4) =============> [" + sCashAmt + "]");
                            Console.WriteLine("거스름(sChange : 14,4) =============> [" + sChange + "]");
                            Console.WriteLine("공병/쇼핑백금액(sBagAmt : 18,8) =============> [" + sBagAmt + "]");
                            Console.WriteLine("==================== 현금ITEM 데이터 ====================");

                            break;
                        case "10":
                            //쿠폰
                            break;
                        case "12":
                            //상품권
                            break;
                        case "15":
                            //신용카드
                            /**
                             *                       CARD_ITEM
                             *                 신용카드 ITEM ( 180 Byte )
                             *      아이템ID (2)
                             *      아이템길이 (3)
                             *      거래구분 (4) - 0200:승인요청,0420:승인취소요청
                             *      거래종별 (2) - 10:신용카드,20:포인트적립,40:포인트지불
                             *      승인밴사 (2) - 00:Easy-Check,01:KICC
                             *      전문일련번호 (6)
                             *      신용카드번호 (16)
                             *      유효기간 (4)
                             *      비밀번호 (16)
                             *      할부개월 (2)
                             *      판매금액 (8)
                             *      승인번호 (9)
                             *      입력구분 (1)
                             *      신용카드데이터 (37)
                             *      승인일자 (8)
                             *      승인시각 (6)
                             *      발급사코드 (3)
                             *      매입사코드 (3)
                             *      가맹점번호 (15)
                             *      합계포인트 (8)
                             *      거래포인트 (8)
                             *      유효포인트 (8)
                             *      공병/쇼핑백금액 (8)
                             *      카드구분 - 0:일반,1:월드패스 (1)
                             *      
                             **/
                            sGubun = ""; //지불방법 (2) - 현금:11,쿠폰:10,상품권:12,신용카드:15
                            sItemId = sData.Substring(0, 2);          //아이템ID
                            sItemLen = sData.Substring(2, 3);         //아이템길이
                            sTranType = sData.Substring(5, 4);        //거래구분
                            sTranKind = sData.Substring(9, 2);        //거래종별
                            sHostId = sData.Substring(11, 2);          //승인밴사
                            sMsgSeqNo = sData.Substring(13, 6);        //전문일련번호
                            sCardNo = sData.Substring(19, 16);          //신용카드번호
                            sExpDate = sData.Substring(35, 4);         //유효기간
                            sPassWd = sData.Substring(39, 16);          //비밀번호
                            sHalbuMonth = sData.Substring(55, 2);      //할부개월
                            sSaleAmt = sData.Substring(57, 8);         //판매금액
                            sAppNo = sData.Substring(65, 9);           //승인번호
                            sWcc = sData.Substring(74, 1);             //입력구분
                            sCardData = sData.Substring(75, 37);        //신용카드 데이터
                            sAppDate = sData.Substring(112, 8);         //승인일자
                            sAppTime = sData.Substring(120, 6);          //승인시각
                            sIssCode = sData.Substring(126, 3);         //발급사코드
                            sPurCode = sData.Substring(129, 3);         //매입사코드
                            sRegNo = sData.Substring(132, 15);           //가맹점번호
                            sTotPoint = sData.Substring(147, 8);        //합계포인트
                            sTranPoint = sData.Substring(155, 8);       //거래포인트
                            sValPoint = sData.Substring(163, 8);        //유효포인트
                            sBagAmt = sData.Substring(171, 8);          //공병/쇼핑백금액
                            sCardType = sData.Substring(179, 1);        //카드구분 - 0:일반,1:월드패스
                            Console.WriteLine("==================== 카드ITEM 데이터 ====================");
                            Console.WriteLine("CARD DATA =============> [" + sData + "]");
                            Console.WriteLine("아이템ID(sItemId : 0,2) =============> [" + sItemId + "]");
                            Console.WriteLine("아이템길이(sItemLen : 2,3) =============> [" + sItemLen + "]");
                            Console.WriteLine("거래구분(sTranType : 5,4) =============> [" + sTranType + "]");
                            Console.WriteLine("거래종별(sTranKind : 9,2) =============> [" + sTranKind + "]");
                            Console.WriteLine("승인밴사(sHostId : 11,2) =============> [" + sHostId + "]");
                            Console.WriteLine("전문일련번호(sMsgSeqNo : 13,6) =============> [" + sMsgSeqNo + "]");
                            Console.WriteLine("신용카드번호(sCardNo : 19,16) =============> [" + sCardNo + "]");
                            Console.WriteLine("유효기간(sExpDate : 35,4) =============> [" + sExpDate + "]");
                            Console.WriteLine("비밀번호(sPassWd : 39,16) =============> [" + sPassWd + "]");
                            Console.WriteLine("할부개월(sHalbuMonth : 55,2) =============> [" + sHalbuMonth + "]");
                            Console.WriteLine("판매금액(sSaleAmt : 57,8) =============> [" + sSaleAmt + "]");
                            Console.WriteLine("승인번호(sAppNo : 65,9) =============> [" + sAppNo + "]");
                            Console.WriteLine("입력구분(sWcc : 74,1) =============> [" + sWcc + "]");
                            Console.WriteLine("신용카드데이터(sCardData : 75,37) =============> [" + sCardData + "]");
                            Console.WriteLine("승인일자(sAppDate : 112,8) =============> [" + sAppDate + "]");
                            Console.WriteLine("승인시각(sAppTime : 120,6) =============> [" + sAppTime + "]");
                            Console.WriteLine("발급사코드(sIssCode : 126,3) =============> [" + sIssCode + "]");
                            Console.WriteLine("매입사코드(sPurCode : 129,3) =============> [" + sPurCode + "]");
                            Console.WriteLine("가맹점번호(sRegNo : 132,15) =============> [" + sRegNo + "]");
                            Console.WriteLine("합계포인트(sTotPoint : 147,8) =============> [" + sTotPoint + "]");
                            Console.WriteLine("거래포인트(sTranPoint : 155,8) =============> [" + sTranPoint + "]");
                            Console.WriteLine("유효포인트(sValPoint : 163,8) =============> [" + sValPoint + "]");
                            Console.WriteLine("공병/쇼핑백금액(sBagAmt : 171,8) =============> [" + sBagAmt + "]");
                            Console.WriteLine("카드구분(sCardType : 179,1) =============> [" + sCardType + "]");
                            Console.WriteLine("==================== 카드ITEM 데이터 ====================");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        public void PrintItemList(String sMsg)
        {
            /**
             *                      상품 ITEM
             *                 TRAN헤더 ( 121byte )
             *                 Start - ITEM // END - ITEMEND
             *     아이템ID     (6)
             *     아이템길이   (4)
             *     아이템순번   (4)
             *     취소구분     (4)
             *     단품코드     (8)
             *     거래선코드   (8)
             *     본부코드     (6)
             *     분류코드     (2)
             *     단품구분     (2)
             *     수량         (6)
             *     단가         (8)
             *     판매금액     (4)
             *     행사구분     (4)
             *     구단가       (6)
             *     할인구분     (9)
             *     할인율       (9)
             *     할인금액     (9)
             *     과세구분     (1)
             *     박스/보루입수(1)
             *     판매유형     (1)
             *     스캔플래그   (1)
             *     쿠폰코드     (1)
             * 
             **/
            String[] sList = sMsg.Split('|');
            int iIndex = 0;
            String sItemID = null;              //아이템ID
            String sItemLen = null;             //아이템길이
            String sItemSeq = null;             //아이템순번
            String sCancelFlag = null;          //취소구분
            String sPluNo = null;               //단품코드
            String sTradeNo = null;             //거래선코드
            String sHostPlu = null;             //본부코드
            String sClassNo = null;             //분류코드
            String sItemType = null;            //단품구분
            String sItemQty = null;             //수량
            String sItemPrice = null;           //단가
            String sItemAmt = null;             //판매금액
            String sEventFlag = null;           //행사구분
            String sOldPrice = null;            //구단가
            String sDcFlag = null;              //할인구분
            String sDcRate = null;              //할인율
            String sDcAmt = null;               //할인금액
            String sTaxFlag = null;             //과세구분
            String sBoxQty = null;              //박스/보루입수
            String sPluSep = null;              //판매유형
            String sScanFlag = null;            //스캔플래그
            String sCouponCode = null;          //쿠폰코드

            try
            {
                foreach (String sData in sList)
                {

                    sItemID = sData.Substring(0, 2);                //아이템ID
                    sItemLen = sData.Substring(2, 3);               //아이템길이
                    sItemSeq = sData.Substring(5, 3);               //아이템순번
                    sCancelFlag = sData.Substring(8, 1);            //취소구분
                    sPluNo = sData.Substring(9, 13);                //단품코드
                    sTradeNo = sData.Substring(22, 4);              //거래선코드
                    sHostPlu = sData.Substring(26, 13);             //본부코드
                    sClassNo = sData.Substring(39, 15);             //분류코드
                    sItemType = sData.Substring(54, 1);             //단품구분
                    sItemQty = sData.Substring(55, 6);              //수량
                    sItemPrice = sData.Substring(61, 9);            //단가
                    sItemAmt = sData.Substring(70, 9);              //판매금액
                    sEventFlag = sData.Substring(79, 1);            //행사구분
                    sOldPrice = sData.Substring(80, 9);             //구단가
                    sDcFlag = sData.Substring(89, 1);               //할인구분
                    sDcRate = sData.Substring(90, 2);               //할인율
                    sDcAmt = sData.Substring(92, 9);                //할인금액
                    sTaxFlag = sData.Substring(101, 1);             //과세구분
                    sBoxQty = sData.Substring(102, 4);              //박스/보루입수
                    sPluSep = sData.Substring(106, 1);              //판매유형
                    sScanFlag = sData.Substring(107, 1);            //스캔플래그
                    sCouponCode = sData.Substring(108, 13);         //쿠폰코드

                    Console.WriteLine("==================== ITEM[" + iIndex + "]번째  ====================");
                    Console.WriteLine("아이템ID(sItemID : 0,6) =============> [" + sItemID + "]");
                    Console.WriteLine("아이템길이(sItemLen : 2,3) =============> [" + sItemLen + "]");
                    Console.WriteLine("아이템순번(sItemSeq : 5,3) =============> [" + sItemSeq + "]");
                    Console.WriteLine("취소구분(sCancelFlag : 8,1) =============> [" + sCancelFlag + "]");
                    Console.WriteLine("단품코드(sPluNo : 9,13) =============> [" + sPluNo + "]");
                    Console.WriteLine("거래선코드(sTradeNo : 22,4) =============> [" + sTradeNo + "]");
                    Console.WriteLine("본부코드(sHostPlu : 26,13) =============> [" + sHostPlu + "]");
                    Console.WriteLine("분류코드(sClassNo : 39,15) =============> [" + sClassNo + "]");
                    Console.WriteLine("단품구분(sItemType : 54,1) =============> [" + sItemType + "]");
                    Console.WriteLine("수량(sItemQty : 55,6) =============> [" + sItemQty + "]");
                    Console.WriteLine("단가(sItemPrice : 61,9) =============> [" + sItemPrice + "]");
                    Console.WriteLine("판매금액(sItemAmt : 70,9) =============> [" + sItemAmt + "]");
                    Console.WriteLine("행사구분(sEventFlag : 79,1) =============> [" + sEventFlag + "]");
                    Console.WriteLine("구단가(sOldPrice : 80,9) =============> [" + sOldPrice + "]");
                    Console.WriteLine("할인구분(sDcFlag : 89,1) =============> [" + sDcFlag + "]");
                    Console.WriteLine("할인율(sDcRate : 90,2) =============> [" + sDcRate + "]");
                    Console.WriteLine("할인금액(sDcAmt : 92,9) =============> [" + sDcAmt + "]");
                    Console.WriteLine("과세구분(sTaxFlag : 101,1) =============> [" + sTaxFlag + "]");
                    Console.WriteLine("박스/보루입수(sBoxQty : 102,4) =============> [" + sBoxQty + "]");
                    Console.WriteLine("판매유형(sPluSep : 106,1) =============> [" + sPluSep + "]");
                    Console.WriteLine("스캔플래그(sScanFlag : 107,1) =============> [" + sScanFlag + "]");
                    Console.WriteLine("쿠폰코드(sCouponCode : 108,13) =============> [" + sCouponCode + "]");
                    Console.WriteLine("==================== ITEM[" + iIndex + "]번째  ====================");


                    iIndex++;
                }


            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        public void PrintInqHeader(String sMsg)
        {
            

            String sMsgLength = sMsg.Substring(0, 6);           //전문길이
            String sStorNo = sMsg.Substring(6, 4);              //점포번호
            String sPosNo = sMsg.Substring(10, 4);              //포스번호
            String sTranNo = sMsg.Substring(14, 4);             //거래번호
            String sSaleDate = sMsg.Substring(18, 8);           //영업일자
            String sSysDate = sMsg.Substring(26, 8);            //시스템일자
            String sSysTime = sMsg.Substring(34, 6);            //시스템시각
            String sCashierNo = sMsg.Substring(40, 6);          //캐셔번호

            try
            {
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
                Console.WriteLine("==================== INQ헤더 데이터 ====================");
                Console.WriteLine("INQ DATA =============> [" + sMsg + "]");
                Console.WriteLine("전문길이(sMsgLength : 0,6) =============> [" + sMsgLength + "]");
                Console.WriteLine("점포번호(sStorNo : 6,4) =============> [" + sStorNo + "]");
                Console.WriteLine("포스번호(sPosNo : 10,4) =============> [" + sPosNo + "]");
                Console.WriteLine("거래번호(sTranNo : 14,4) =============> [" + sTranNo + "]");
                Console.WriteLine("영업일자(sSaleDate : 18,8) =============> [" + sSaleDate + "]");
                Console.WriteLine("시스템일자(sSysDate : 26,8) =============> [" + sSysDate + "]");
                Console.WriteLine("시스템시각(sSysTime : 34,6) =============> [" + sSysTime + "]");
                Console.WriteLine("캐셔번호(sCashierNo : 40,6) =============> [" + sCashierNo + "]");
                Console.WriteLine("==================== INQ헤더 데이터 ====================");
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void PrintPluInq(String sMsg)
        {
            String sItemId = sMsg.Substring(0, 2);           //전문길이
            String sPluNo = sMsg.Substring(2, 13);           //상품번호
            try
            {
                /**
                *                 PLU 조회 요청 ( 15 )
                *         - 요청구분  (2) - 01:MST INQ
                *         - PLU코드   (13)
                *
                */
                Console.WriteLine("==================== PLU조회 데이터 ====================");
                Console.WriteLine("====================     command    ====================");
                Console.WriteLine("전문길이(sItemId) =============> [" + sItemId + "]");
                Console.WriteLine("상품번호(sPluNo : 2,13) =============> [" + sPluNo + "]");

                Console.WriteLine("==================== RESULT (상품마스터) ====================");
                Dictionary<String, String> dicRet = SearchMst("0001", sPluNo);
                Console.WriteLine("점포코드(sStoreNo) =============> [" + dicRet["sStoreNo"] + "]");
                Console.WriteLine("상품코드(sPluNo ) =============> [" + dicRet["sPluNo"] + "]");
                Console.WriteLine("부문(sDeptNo) =============> [" + dicRet["sDeptNo"] + "]");
                Console.WriteLine("거래선(sTradeNo) =============> [" + dicRet["sTradeNo"] + "]");
                Console.WriteLine("분류(sClassNo) =============0> [" + dicRet["sClassNo"] + "]");
                Console.WriteLine("상품명(sPluName) =============> [" + dicRet["sPluName"] + "]");
                Console.WriteLine("영수증명(sReceiptName) =============> [" + dicRet["sReceiptName"] + "]");
                Console.WriteLine("자사상품코드(sInstoreNo) =============> [" + dicRet["sInstoreNo"] + "]");
                Console.WriteLine("공병연결코드(sLinkPlu) =============> [" + dicRet["sLinkPlu"] + "]");
                Console.WriteLine("연결코드(sMlinkPlu) =============> [" + dicRet["sMlinkPlu"] + "]");
                Console.WriteLine("낱개원가(sWongaUnit) =============> [" + dicRet["sWongaUnit"] + "]");
                Console.WriteLine("낱개행사원가(sWongaUnitSale) =============> [" + dicRet["sWongaUnitSale"] + "]");
                Console.WriteLine("낱개매가(sMaegaUnit) =============> [" + dicRet["sMaegaUnit"] + "]");
                Console.WriteLine("낱개행사메가(sMaegaUnitSale) =============> [" + dicRet["sMaegaUnitSale"] + "]");
                Console.WriteLine("거래구분(sPluSep) =============> [" + dicRet["sPluSep"] + "]");
                Console.WriteLine("과세구분(sTaxSep) =============> [" + dicRet["sTaxSep"] + "]");
                Console.WriteLine("브랜드구분(sBrandSep) =============> [" + dicRet["sBrandSep"] + "]");
                Console.WriteLine("메이커코드(sMakerNo) =============> [" + dicRet["sMakerNo"] + "]");
                Console.WriteLine("프라이스택구분(sPriceCode) =============> [" + dicRet["sPriceCode"] + "]");
                Console.WriteLine("최대진열량(sMaxExt) =============> [" + dicRet["sMaxExt"] + "]");
                Console.WriteLine("최소진열량(sMinExt) =============> [" + dicRet["sMinExt"] + "]");
                Console.WriteLine("수정구분(sUpdFlag) =============> [" + dicRet["sUpdFlag"] + "]");
                Console.WriteLine("할인구분(sSaleFlag) =============> [" + dicRet["sSaleFlag"] + "]");
                Console.WriteLine("입수(sBoxQty) =============> [" + dicRet["sBoxQty"] + "]");
                Console.WriteLine("판매단위(sSourceSep) =============> [" + dicRet["sSourceSep"] + "]");
                Console.WriteLine("진열대코드(sExtLoc1) =============> [" + dicRet["sExtLoc1"] + "]");
                Console.WriteLine("진열대-대(sExtLoc2) =============> [" + dicRet["sExtLoc2"] + "]");
                Console.WriteLine("진열대-열(sExtLoc3) =============> [" + dicRet["sExtLoc3"] + "]");
                Console.WriteLine("재고수량관리유무(sInvLoc) =============> [" + dicRet["sInvLoc"] + "]");
                Console.WriteLine("발주단위(sOrdUnit) =============> [" + dicRet["sOrdUnit"] + "]");
                Console.WriteLine("배송구분(sDellSep) =============> [" + dicRet["sDellSep"] + "]");
                Console.WriteLine("발주구분(sPurSep) =============> [" + dicRet["sPurSep"] + "]");
                Console.WriteLine("이익율(sProfitRate) =============> [" + dicRet["sProfitRate"] + "]");
                Console.WriteLine("생식품구분(sMdFlag) =============> [" + dicRet["sMdFlag"] + "]");
                Console.WriteLine("저울구분(sJeulFlag) =============> [" + dicRet["sJeulFlag"] + "]");
                Console.WriteLine("저울단위가격(sJeulPrice) =============> [" + dicRet["sJeulPrice"] + "]");
                Console.WriteLine("최소발주량(sMinOrdQty) =============> [" + dicRet["sMinOrdQty"] + "]");
                Console.WriteLine("상품명(POP)(sPluNamePop) =============> [" + dicRet["sPluNamePop"] + "]");
                Console.WriteLine("상품명(발주용)(sPluNameOrd) =============> [" + dicRet["sPluNameOrd"] + "]");
                Console.WriteLine("제조사명(sMakerName) =============> [" + dicRet["sMakerName"] + "]");
                Console.WriteLine("규격(sStandard) =============> [" + dicRet["sStandard"] + "]");
                Console.WriteLine("원산지(sOriginRegion) =============> [" + dicRet["sOriginRegion"] + "]");
                Console.WriteLine("원부재료구분(sWonbjFlag) =============> [" + dicRet["sWonbjFlag"] + "]");
                Console.WriteLine("물류최저발주량(sLogiMinOrdQty) =============> [" + dicRet["sLogiMinOrdQty"] + "]");
                Console.WriteLine("발주방법(sOrdMethod) =============> [" + dicRet["sOrdMethod"] + "]");
                Console.WriteLine("물류구분(sLogiFlag) =============> [" + dicRet["sLogiFlag"] + "]");
                Console.WriteLine("담배주류구분(sCigarDrinkFlag) =============> [" + dicRet["sCigarDrinkFlag"] + "]");
                Console.WriteLine("재고관리코드(sSkuNo) =============> [" + dicRet["sSkuNo"] + "]");
                Console.WriteLine("발주종료일(sOrdEndDt) =============> [" + dicRet["sOrdEndDt"] + "]");
                Console.WriteLine("판매종료일(sSaleEndDt) =============> [" + dicRet["sSaleEndDt"] + "]");
                Console.WriteLine("공용기구분(sBottleFlag) =============> [" + dicRet["sBottleFlag"] + "]");
                Console.WriteLine("안전재고일수(sSafeDayCnt) =============> [" + dicRet["sSafeDayCnt"] + "]");
                Console.WriteLine("발주가능여부(sOrdPosblFlag) =============> [" + dicRet["sOrdPosblFlag"] + "]");
                Console.WriteLine("단위가격표시기준(sConDispUnit) =============> [" + dicRet["sConDispUnit"] + "]");
                Console.WriteLine("등록일자(sRegDate) =============> [" + dicRet["sRegDate"] + "]");
                Console.WriteLine("등록사번(sRegEmpNo) =============> [" + dicRet["sRegEmpNo"] + "]");
                Console.WriteLine("변경일자(sChgDate) =============> [" + dicRet["sChgDate"] + "]");
                Console.WriteLine("변경사번(sChgEmpNo) =============> [" + dicRet["sChgEmpNo"] + "]");
                Console.WriteLine("==================== PLU조회 데이터 ====================");
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


    }
}
