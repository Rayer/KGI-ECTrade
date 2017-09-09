using System;
using System.Text;
using Intelligence;
using Package;

namespace KGIECTrade
{
    public class Adapter
    {
        private const string token = "b6eb";
        private const string sid = "API";
        private const string url = "quoteapi.kgi.com.tw";
        private readonly QuoteCom quoteCom;
        private readonly UTF8Encoding encoding = new UTF8Encoding();


        public Adapter()
        {
            quoteCom = new Intelligence.QuoteCom(url, 8000, sid, token);
            quoteCom.OnRcvMessage += OnQuoteRcvMessage;
            quoteCom.OnGetStatus += OnQuoteGetStatus;
            quoteCom.OnRecoverStatus += OnRecoverStatus;
            quoteCom.QDebugLog = false;  
            quoteCom.FQDebugLog = false; 
            Console.WriteLine("QuoteCom 範例程式 FOR 證券 [ Version : {0}]", quoteCom.version);  //2014.6.19 ADD 
            quoteCom.Connect(url, 8000);
        }
        
        private void PrintInfo(string msg) {
            var fMsg = $"[{DateTime.Now:hh:mm:ss:ffff}] {msg} {Environment.NewLine}";
            Console.WriteLine(fMsg);
        }

        private void OnRecoverStatus(object sender, string Topic, RECOVER_STATUS status, uint RecoverCount) {
            QuoteCom com = (QuoteCom)sender;
            switch (status) {
                case RECOVER_STATUS.RS_DONE:        //回補資料結束
                    PrintInfo($"結束回補 Topic:[{Topic}]{RecoverCount}");
                    break;
                case RECOVER_STATUS.RS_BEGIN:       //開始回補資料
                    PrintInfo($"開始回補 Topic:[{Topic}]");
                    break;
            }
        }

        private void OnQuoteRcvMessage(object sender, PackageBase package) {
//            if (package.TOPIC != null)
//                if (RecoverMap.ContainsKey(package.TOPIC))
//                    RecoverMap[package.TOPIC]++;


            StringBuilder sb;
            
            switch (package.DT) {
                case (ushort)DT.LOGIN: 
                    P001503 _p001503 = (P001503)package;
                    if (_p001503.Code == 0) { 
                        PrintInfo("可註冊檔數：" + _p001503.Qnum);
                        if (quoteCom.QuoteFuture) PrintInfo("可註冊期貨報價");
                        if (quoteCom.QuoteStock) PrintInfo("可註冊證券報價");
                    }
                    break;
                
                case (ushort)DT.QUOTE_STOCK_MATCH1:   //上市成交
                case (ushort)DT.QUOTE_STOCK_MATCH2:   //上櫃成交
                    PI31001 pi31001 = (PI31001)package;
                    sb = new StringBuilder(Environment.NewLine);
                    sb.Append((package.DT == (ushort)DT.QUOTE_STOCK_MATCH1) ? "上市 " : "上櫃 "); 
                    if (pi31001.Status == 0) sb.Append("<試撮>"); 
                    sb.Append("商品代號: ").Append(pi31001.StockNo).Append("  更新時間: ").Append(pi31001.Match_Time).Append(Environment.NewLine);
                    sb.Append(" 成交價: ").Append(pi31001.Match_Price).Append("  單量: ").Append(pi31001.Match_Qty);
                    sb.Append(" 總量: ").Append(pi31001.Total_Qty).Append("  來源: ").Append(pi31001.Source ).Append(Environment.NewLine);
                    sb.Append("=========================================");
                    PrintInfo(sb.ToString());
                    break;
                
                case (ushort)DT.QUOTE_STOCK_DEPTH1: //上市五檔
                case (ushort)DT.QUOTE_STOCK_DEPTH2: //上櫃五檔
                    PI31002 i31002 = (PI31002)package;
                    sb = new StringBuilder(Environment.NewLine);
                    sb.Append((package.DT == (ushort)DT.QUOTE_STOCK_DEPTH1) ? "上市 " : "上櫃 ");
                    if (i31002.Status == 0) sb.Append("<試撮> "); 
                    sb.Append("商品代號: ").Append(i31002.StockNo).Append(" 更新時間: ").Append(i31002.Match_Time).Append("  來源: ").Append(i31002.Source ).Append(Environment.NewLine);
                    for (int i = 0; i < 5; i++)
                        sb.Append(String.Format("五檔[{0}] 買[價:{1:N} 量:{2:N}]    賣[價:{3:N} 量:{4:N}]", i + 1, i31002.BUY_DEPTH[i].PRICE, i31002.BUY_DEPTH[i].QUANTITY, i31002.SELL_DEPTH[i].PRICE, i31002.SELL_DEPTH[i].QUANTITY)).Append(Environment.NewLine);
                    

                        sb.Append("=========================================");
                    
                    PrintInfo(sb.ToString());
                    break;
                case (ushort)DT.QUOTE_LAST_PRICE_STOCK:
                    PI30026 pi30026 = (PI30026)package;
                    sb = new StringBuilder(Environment.NewLine);
                    sb.Append("商品代號:").Append(pi30026.StockNo).Append(" 最後價格:").Append(pi30026.LastMatchPrice).Append(Environment.NewLine);
                    sb.Append("當日最高成交價格:").Append(pi30026.DayHighPrice).Append(" 當日最低成交價格:").Append(pi30026.DayLowPrice);
                    sb.Append("開盤價:").Append(pi30026.FirstMatchPrice).Append(" 開盤量:").Append(pi30026.FirstMatchQty).Append(Environment.NewLine);
                    sb.Append("參考價:").Append(pi30026.ReferencePrice).Append(Environment.NewLine);
                    sb.Append("成交單量:").Append(pi30026.LastMatchQty).Append(Environment.NewLine);
                    sb.Append("成交總量:").Append(pi30026.TotalMatchQty).Append(Environment.NewLine);
                    for (int i = 0; i < 5; i++)
                        sb.Append(String.Format("五檔[{0}] 買[價:{1:N} 量:{2:N}]    賣[價:{3:N} 量:{4:N}]", i + 1, pi30026.BUY_DEPTH[i].PRICE, pi30026.BUY_DEPTH[i].QUANTITY, pi30026.SELL_DEPTH[i].PRICE, pi30026.SELL_DEPTH[i].QUANTITY)).Append(Environment.NewLine);
                    sb.Append("==============================================");
                    PrintInfo(sb.ToString());
                    break;
                case (ushort)DT.QUOTE_STOCK_INDEX1:  //上市指數
                    PI31011 pi31011 = (PI31011)package;
                    sb = new StringBuilder(Environment.NewLine);
                    sb.Append("[上市指數]更新時間：").Append(pi31011.Match_Time).Append("   筆數: ").Append(pi31011.COUNT).Append(Environment.NewLine);
                    for (int i = 0; i < pi31011.COUNT; i++)
                        sb.Append(" [" + (i + 1) + "] ").Append(pi31011.IDX[i].VALUE);
                    sb.Append("==============================================");
                    PrintInfo(sb.ToString());
                    break;
                case (ushort)DT.QUOTE_STOCK_INDEX2:  //上櫃指數
                    PI31011 pi32011 = (PI31011)package;
                    sb = new StringBuilder(Environment.NewLine);
                    sb.Append("[上櫃指數]更新時間：").Append(pi32011.Match_Time).Append("   筆數: ").Append(pi32011.COUNT).Append(Environment.NewLine);
                    for (int i = 0; i < pi32011.COUNT; i++)
                        sb.Append(" [" + (i + 1) + "]").Append(pi32011.IDX[i].VALUE);
                    sb.Append("==============================================");
                    PrintInfo(sb.ToString());
                    break;
                case (ushort)DT.QUOTE_STOCK_NEWINDEX1:  //上市新編指數
                    PI31021 pi31021 = (PI31021)package;
                    sb = new StringBuilder(Environment.NewLine);
                    sb.Append("上市新編指數[").Append(pi31021.IndexNo).Append("] 時間:").Append(pi31021.IndexTime);
                    sb.Append("指數:  ").Append(pi31021.LatestIndex).Append(Environment.NewLine); 
                    PrintInfo(sb.ToString());
                    break;
                case (ushort)DT.QUOTE_STOCK_NEWINDEX2:  //上櫃新編指數
                    PI31021 pi32021 = (PI31021)package;
                    sb = new StringBuilder(Environment.NewLine);
                    sb.Append("上櫃新編指數[").Append(pi32021.IndexNo).Append("] 時間:").Append(pi32021.IndexTime);
                    sb.Append("最新指數: ").Append(pi32021.LatestIndex).Append(Environment.NewLine); 
                    PrintInfo(sb.ToString());
                    break; 
                case (ushort)DT.QUOTE_LAST_INDEX1:  //上市最新指數查詢
                    PI31026 pi31026 = (PI31026)package;
                    sb = new StringBuilder(Environment.NewLine);
                    sb.Append("  最新上市指數  筆數: ").Append(pi31026.COUNT).Append(Environment.NewLine);
                    for (int i = 0; i < pi31026.COUNT; i++) {
                        sb.Append(" [" + (i + 1) + "] ").Append(" 昨日收盤指數:").Append(pi31026.IDX[i].RefIndex);
                        sb.Append(" 開盤指數:").Append(pi31026.IDX[i].FirstIndex).Append(" 最新指數:").Append(pi31026.IDX[i].LastIndex);
                        sb.Append(" 最高指數:").Append(pi31026.IDX[i].DayHighIndex).Append(" 最低指數:").Append(pi31026.IDX[i].DayLowIndex).Append(Environment.NewLine);
                        sb.Append("==============================================");
                    }
                    PrintInfo(sb.ToString()); 
                    break;
                case (ushort)DT.QUOTE_LAST_INDEX2:  //上櫃最新指數查詢
                    PI31026 pi32026 = (PI31026)package;
                    sb = new StringBuilder(Environment.NewLine);
                    sb.Append("  最新上櫃指數  筆數: ").Append(pi32026.COUNT).Append(Environment.NewLine);
                    for (int i = 0; i < pi32026.COUNT; i++) {
                        sb.Append(" [" + (i + 1) + "] ").Append(" 昨日收盤指數:").Append(pi32026.IDX[i].RefIndex);
                        sb.Append(" 開盤指數:").Append(pi32026.IDX[i].FirstIndex).Append(" 最新指數:").Append(pi32026.IDX[i].LastIndex);
                        sb.Append(" 最高指數:").Append(pi32026.IDX[i].DayHighIndex).Append(" 最低指數:").Append(pi32026.IDX[i].DayLowIndex).Append(Environment.NewLine); 
                        sb.Append("==============================================");
                    }
                    PrintInfo(sb.ToString()); 
                    break;
                case (ushort)DT.QUOTE_STOCK_AVGINDEX:  //加權平均指數 2014.8.6 ADD ; 
                    PI31022 pi31022 = (PI31022)package;
                    sb = new StringBuilder(Environment.NewLine);
                    sb.Append("加權平均指數[").Append(pi31022.IndexNo).Append("] 時間:").Append(pi31022.IndexTime);
                    sb.Append("最新指數: ").Append(pi31022.LatestIndex).Append(Environment.NewLine);
                    PrintInfo(sb.ToString());
                    break; 
            }
        }


       private void OnQuoteGetStatus(object sender, COM_STATUS staus, byte[] msg) {
            QuoteCom com = (QuoteCom)sender;
            string smsg = null;
            switch (staus) {
                case COM_STATUS.LOGIN_READY:
                    PrintInfo(String.Format("LOGIN_READY:[{0}]", encoding.GetString(msg)));
                    break;
                case COM_STATUS.LOGIN_FAIL:
                    PrintInfo(String.Format("LOGIN FAIL:[{0}]", encoding.GetString(msg)));
                    break;
                case COM_STATUS.LOGIN_UNKNOW:
                    PrintInfo(String.Format("LOGIN UNKNOW:[{0}]", encoding.GetString(msg)));
                    break;
                case COM_STATUS.CONNECT_READY:
                    //quoteCom.Login(tfcom.Main_ID, tfcom.Main_PWD, tfcom.Main_CENTER);
                    smsg = "QuoteCom: [" + encoding.GetString(msg) + "] MyIP=" + quoteCom.MyIP;
                    PrintInfo(smsg);
                    break;
                case COM_STATUS.CONNECT_FAIL:
                    smsg = encoding.GetString(msg);
                    PrintInfo("CONNECT_FAIL:" + smsg);
                    break;
                case COM_STATUS.DISCONNECTED:
                    smsg = encoding.GetString(msg);
                    PrintInfo("DISCONNECTED:" + smsg);
                    break;
                case COM_STATUS.SUBSCRIBE:
                    smsg = encoding.GetString(msg, 0, msg.Length - 1);
                    PrintInfo(String.Format("SUBSCRIBE:[{0}]", smsg));
                    //txtQuoteList.AppendText(String.Format("SUBSCRIBE:[{0}]", smsg));  //2012.02.16 LYNN TEMPORARY ; 
                    break;
                case COM_STATUS.UNSUBSCRIBE:
                    smsg = encoding.GetString(msg, 0, msg.Length - 1);
                    PrintInfo(String.Format("UNSUBSCRIBE:[{0}]", smsg));
                    break;
                case COM_STATUS.ACK_REQUESTID:
                    long RequestId = BitConverter.ToInt64(msg, 0);
                    byte status = msg[8];
                    PrintInfo("Request Id BACK: " + RequestId + " Status=" + status);
                    break;
//                case COM_STATUS.RECOVER_DATA:
//                    smsg = encoding.GetString(msg, 1, msg.Length - 1);
//                    if (!RecoverMap.ContainsKey(smsg))
//                        RecoverMap.Add(smsg, 0);
//
//                    if (msg[0] == 0) {
//                        RecoverMap[smsg] = 0;
//                        AddInfo(String.Format("開始回補 Topic:[{0}]", smsg));
//                    }
//
//                    if (msg[0] == 1) {
//                        AddInfo(String.Format("結束回補 Topic:[{0} 筆數:{1}]", smsg, RecoverMap[smsg]));
//                    }
//                    break;
            }
            com.Processed();
        }

    }
}