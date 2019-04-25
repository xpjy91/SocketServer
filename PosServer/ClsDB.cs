/********************************************************************************************************************
 *  
 *  FileName    : ClsDB.cs
 *  Description : 데이터베이스 연결 Class
 *  Date        : 2019. 04. 18
 *  Author      : UniBiz_ParkJIyun
 *  -----------------------------------------------------------------------------------------------------------------
 *  History
 *  
 *  
 * *****************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosServer
{
    class ClsDB
    {

        private static String sSource = "server=localhost; Database=POSDB; PWD=UNIPOS; UID=UNIPOS;";
        //private static String sSource = "server=localhost; Database=POSDB; PWD=a884752b; UID=sa;";
        private static SqlConnection sqlConn = null;

        // 데이터베이스 열기
        public static void OpenDataBase()
        {
            try
            {
                sqlConn = new SqlConnection(sSource);
                sqlConn.Open();
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        // 데이터베이스 닫기
        public static void CloseDataBase()
        {
            try
            {
                if (sqlConn.State == System.Data.ConnectionState.Open)
                {
                    sqlConn.Close();
                }
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        // SQL문 실행
        public int ExecuteSql(string sSql)
        {
            int iRet = -1;
            try
            {
                SqlCommand sqlCmd = new SqlCommand(sSql, sqlConn);
                iRet = sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return iRet;
        }



        // DB 조회
        public SqlDataReader GetData(string sSql)
        {
            SqlDataReader sqlReader = null;
            SqlCommand sqlCmd = null;
            try
            {
                sqlCmd = new SqlCommand(sSql, sqlConn);
                sqlReader = sqlCmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return sqlReader;

        }

        // DB 조회 DATASET return
        public DataSet GetDataSet(string sSql)
        {
            SqlDataAdapter sqlAdapter = null;
            SqlCommandBuilder sqlBuilder = null;
            DataSet dsRet = null;

            try
            {
                sqlConn = new SqlConnection(sSource);
                sqlAdapter = new SqlDataAdapter(sSql, sqlConn);
                sqlBuilder = new SqlCommandBuilder(sqlAdapter);
                dsRet = new DataSet();
                sqlAdapter.Fill(dsRet);
            }
            catch (Exception ex)
            {
                ClsLog.WriteLog(ClsLog.LOG_EXCEPTION, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return dsRet;
        }

    }
}
