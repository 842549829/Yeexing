using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Qunau.AirportData.Common;
using Qunau.NetFrameWork.Common.Code;
using HttpHelper = Qunau.AirportData.Common.HttpHelper;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //string content = "total_amount=0.01&buyer_id=2088802210835072&trade_no=2017041421001004070205315092&body=%u80a5%u80a0%u7c89&notify_time=2017-04-14+16%3a03%3a57&subject=%u80a5%u80a0%u7c89&sign_type=RSA2&buyer_logon_id=hyy***%40qq.com&auth_app_id=2017041006623425&charset=utf-8&notify_type=trade_status_sync&invoice_amount=0.01&out_trade_no=170414160356021277&trade_status=TRADE_SUCCESS&gmt_payment=2017-04-14+16%3a03%3a57&version=1.0&point_amount=0.00&sign=qIUCtOT64SqvUVQ7ekdYkQuKQ6gwIIhjV10L87eGnkGalvkKt3nN6wG1PXgHMzmrEQPpM1VJ717RhgN50kPax8uXV81vWjwIeHmjS5L2jT%2f99StDEC61cOPcNrIfeaZST%2bkJXEbLIAhZGfaiyfj7mNPko4x9tbY0ctAVwjO5gYIRHYF1NA4LMB%2fkKoh7VMtoUwxpD53Byf2rzsGGXApmiQetuEwWHIpCYzw0Juf1hcmEPBl%2fpZi2%2fU4Vlkjd3XQKBET5QLCk%2bQg%2byD%2bENdlRYlTNu10G9v3vTM0HY8tfPHthAQIrHOzRJNlhhepCHghRj30dFJJ4hZZO6wQBPN2u0g%3d%3d&gmt_create=2017-04-14+16%3a03%3a56&buyer_pay_amount=0.01&receipt_amount=0.01&fund_bill_list=%5b%7b%22amount%22%3a%220.01%22%2c%22fundChannel%22%3a%22ALIPAYACCOUNT%22%7d%5d&app_id=2017041006623425&seller_id=2088111473711680&notify_id=71dfbab0fdd56f11b1297df241e0f47gji&seller_email=sclxgl01%40163.com";
            //HttpHelper http = new HttpHelper();
            //string rel = http.HttpPost("http://localhost:38599/AliPayNotice/PaySdkNotice", string.Empty, content, Encoding.UTF8, false, false, 5000);

            //ServiceReference1.RefuseOrderForVenderService_1_0Client cc = new ServiceReference1.RefuseOrderForVenderService_1_0Client();
            //var r = cc.refuseOrderForVender(new ServiceReference1.refuseOrderForVenderRequest
            //{
            //    agencyCode = "ELONGGY",
            //    officeNo = "",
            //    orderNo = "102016071460139811",
            //    refuseCause = "无座位",
            //    refuseCode = "22",
            //    sign = "9336f5c275bcfab706b9130883bc2bab"
            //});
            //SendTest();
            // var html = File.ReadAllText(@"C:\Users\rongbo.720U\Desktop\text.txt");
            //string str = GetTitleContent(html, "p", "my-p ft-cl3 text-elli3");

            Console.WriteLine();
            Console.ReadLine();
        }


        public static void Proxy()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://192.168.10.41:8010/");
            listener.Start();
            //listener.BeginGetContext(GetContextCallBack, listener);
        }

        public static string GetTitleContent(string str, string title, string classvalue)
        {
            string tmpStr = string.Format(@"<{0} class=""{2}""[^>]*?>(?<Text>[^<]*)</{1}>", title, title, classvalue); //获取<title>之间内容
            Match TitleMatch = Regex.Match(str, tmpStr, RegexOptions.IgnoreCase);
            string result = TitleMatch.Groups["Text"].Value;
            return result;
        }

        public static string GetElementByClassName(string htmlConetnt, string label, string className)
        {
            Regex reg = new Regex(string.Format(@"(?is)<{0} class=""{1}""[^>]*>(?><{0}[^>]*>(?<o>)|</{0}>(?<-o>)|(?:(?!</?{0}\b).)*)*(?(o)(?!))</{0}>", label, className));
            Match first = reg.Match(htmlConetnt);
            return first.Value;
        }

        public static string GetElementById(string htmlConetnt, string label, string id)
        {
            Regex reg = new Regex(string.Format(@"(?is)<{0} id=""{1}""[^>]*>(?><{0}[^>]*>(?<o>)|</{0}>(?<-o>)|(?:(?!</?{0}\b).)*)*(?(o)(?!))</{0}>", label, id));
            Match first = reg.Match(htmlConetnt);
            return first.Value;
        }

        public static string GetLabel(string htmlConetnt, string label)
        {
            Regex reg = new Regex(string.Format(@"(?is)<{0}[^>]*>(?><{0}[^>]*>(?<o>)|</{0}>(?<-o>)|(?:(?!</?{0}\b).)*)*(?(o)(?!))</{0}>", label));
            Match first = reg.Match(htmlConetnt);
            return first.Value;
        }


        private static string GetInstructResult(string instruct)
        {
            const int constantsIdentification = 80;
            List<string> result = new List<string>();
            var instructs = instruct.Split('\r');
            foreach (var item in instructs)
            {
                string conetnt = item;
                int strLength = conetnt.Length;
                if (strLength <= constantsIdentification)
                {
                    result.Add(conetnt);
                }
                else
                {
                    int i;
                    if (strLength % constantsIdentification == 0)
                    {
                        i = strLength / constantsIdentification;
                    }
                    else
                    {
                        i = strLength / constantsIdentification + 1;
                    }
                    for (int j = 0; j < i; j++)
                    {
                        var strResult = j + 1 != i
                            ? conetnt.Substring(j * constantsIdentification, constantsIdentification)
                            : conetnt.Substring(j * constantsIdentification);
                        result.Add(strResult);
                    }
                }
            }
            return string.Join("\r\n", result);
        }

        public static void SendTest()
        {
            //var content = "H4sIAAAAAAAEAO29B2AcSZYlJi9tyntSvVK1%2BB0oQiAYBMk2JBAEOzBiM3mkuwdaUcjKasqgcplVmVdZhZAzO2dvPfee%2B%2B999577733ujudTif338XGZkAWz2zkrayZ4hgKrIHz9%2BfB8In7xR1U9y%2BsX1UePPtrb2f105%2BHepzu7Ozt7e7sHn%2B7s3P9o9NFqWdOX369H%2Bycfof%2BnBQXLmTl6fffZPnNInDOF1m7Xr5qNHe6OPivOTeba8yLE5x892hl9NHVc08fodFXq4s6m%2BUMjNqsvTR4CIrflGRLVlbVHni3zZ0sf5VDxXf9f95UXtfnnGH1R11yXfXf854Xdf4nl36R0B9Xf%2Bdf9V8eX9UKi3%2Bs73LP8Q5r487o%2BJP%2Bwfy7%2Bz0DTPGvy%2BtL7P%2Bjqb4s8iwP8e747r%2B8P%2Bi%2BNP%2Bjv8z0LrO9y9L%2BkP%2BK%2Btv%2B7vQ%2BvhPyL%2Brusy5z6uP4%2B3v%2B2v8Twz4ui%2BT17TP%2BgP%2BayDyvrbvr8q8g%2BrPKv%2BS%2Bzr3P9jqLC7Kzv%2BEv%2BS%2BqLzPj8T6m4G7zPHvTDsgaUDTatkWRIZle5Ktm1xG2RbTt3n75nqFv5%2B8fkkf1VmbywdElVWZtedVvXhaNNNqDSLsjT%2Blz2f5bD1ti2p5vJCPd8b06XlZXMzbF18SrMf7NIkooc6u8zLV5gA%2BvjNi%2BPf67vP6eOmzer2KXWlE7%2B983B772G6eDo3g59ndV1cZn3v9bk%2B9unhHY6Hv7u0%2BxNiySbGkv76yHboRZPVPZuUacB48ICzxdpu9ozv619VUT7L8b3%2BvcqaJid06xMZ2q73kVJq122Ilvgs8rTos29iZOoZs9m9BzsHe4Tgg4Pdvb0DGsXvbSnO3z%2B4t7MNKu3QlsHHzlqv6yLKYG8N96jATRZmRsqP3j46fjBAfq%2BBksqNl%2B9%2BL1efPndFx%2B5z%2Bts2WQ8PzLN9LkFwUQgChf1tDI9hR92JxtvTUTQJNWwUeXxSr4u2iO1231hsfK03CeFaXHd9X5OXXMVHh5%2Bnsd7BxYsIGUrtY1yW6Tv8yudaw9ezJR7kwGXsPnfNgQAAA==";
            var content = System.Web.HttpUtility.UrlDecode("H4sIAAAAAAAEAO29B2AcSZYlJi9tyntSvVK1%2BB0oQiAYBMk2JBAEOzBiM3mkuwdaUcjKasqgcplVmVdZhZAzO2dvPfee%2B%2B999577733ujudTif338XGZkAWz2zkrayZ4hgKrIHz9%2BfB8In7xR1U9y%2BsX1UePPtrb2f105%2BHepzu7Ozt7e7sHn%2B7s3P9o9NFqWdOX369H%2Bycfof%2BnBQXLmTl6fffZPnNInDOF1m7Xr5qNHe6OPivOTeba8yLE5x892hl9NHVc08fodFXq4s6m%2BUMjNqsvTR4CIrflGRLVlbVHni3zZ0sf5VDxXf9f95UXtfnnGH1R11yXfXf854Xdf4nl36R0B9Xf%2Bdf9V8eX9UKi3%2Bs73LP8Q5r487o%2BJP%2Bwfy7%2Bz0DTPGvy%2BtL7P%2Bjqb4s8iwP8e747r%2B8P%2Bi%2BNP%2Bjv8z0LrO9y9L%2BkP%2BK%2Btv%2B7vQ%2BvhPyL%2Brusy5z6uP4%2B3v%2B2v8Twz4ui%2BT17TP%2BgP%2BayDyvrbvr8q8g%2BrPKv%2BS%2Bzr3P9jqLC7Kzv%2BEv%2BS%2BqLzPj8T6m4G7zPHvTDsgaUDTatkWRIZle5Ktm1xG2RbTt3n75nqFv5%2B8fkkf1VmbywdElVWZtedVvXhaNNNqDSLsjT%2Blz2f5bD1ti2p5vJCPd8b06XlZXMzbF18SrMf7NIkooc6u8zLV5gA%2BvjNi%2BPf67vP6eOmzer2KXWlE7%2B983B772G6eDo3g59ndV1cZn3v9bk%2B9unhHY6Hv7u0%2BxNiySbGkv76yHboRZPVPZuUacB48ICzxdpu9ozv619VUT7L8b3%2BvcqaJid06xMZ2q73kVJq122Ilvgs8rTos29iZOoZs9m9BzsHe4Tgg4Pdvb0DGsXvbSnO3z%2B4t7MNKu3QlsHHzlqv6yLKYG8N96jATRZmRsqP3j46fjBAfq%2BBksqNl%2B9%2BL1efPndFx%2B5z%2Bts2WQ8PzLN9LkFwUQgChf1tDI9hR92JxtvTUTQJNWwUeXxSr4u2iO1231hsfK03CeFaXHd9X5OXXMVHh5%2Bnsd7BxYsIGUrtY1yW6Tv8yudaw9ezJR7kwGXsPnfNgQAAA==");
            //var url = "http://115.28.161.82:8083/JinRi/OrderNotice.aspx";
            var url = "http://localhost:50147/JinRi/OrderNotice.aspx";
            //var request = string.Format("{0}?content={1}&operateTime={2}&providerName={3}&sign={4}", url, content, "2016-09-26 091258", "ywm2008", "2BA6C15BC6A8BEF6910FBC07D7365601");

            var r = Decompress(content);

            var request = string.Format("{0}?{1}", url, "providerName=ywm2008&operateTime=2016-09-26 091258&sign=2BA6C15BC6A8BEF6910FBC07D7365601&content=H4sIAAAAAAAEAO29B2AcSZYlJi9tyntSvVK1%2BB0oQiAYBMk2JBAEOzBiM3mkuwdaUcjKasqgcplVmVdZhZAzO2dvPfee%2B%2B999577733ujudTif338XGZkAWz2zkrayZ4hgKrIHz9%2BfB8In7xR1U9y%2BsX1UePPtrb2f105%2BHepzu7Ozt7e7sHn%2B7s3P9o9NFqWdOX369H%2Bycfof%2BnBQXLmTl6fffZPnNInDOF1m7Xr5qNHe6OPivOTeba8yLE5x892hl9NHVc08fodFXq4s6m%2BUMjNqsvTR4CIrflGRLVlbVHni3zZ0sf5VDxXf9f95UXtfnnGH1R11yXfXf854Xdf4nl36R0B9Xf%2Bdf9V8eX9UKi3%2Bs73LP8Q5r487o%2BJP%2Bwfy7%2Bz0DTPGvy%2BtL7P%2Bjqb4s8iwP8e747r%2B8P%2Bi%2BNP%2Bjv8z0LrO9y9L%2BkP%2BK%2Btv%2B7vQ%2BvhPyL%2Brusy5z6uP4%2B3v%2B2v8Twz4ui%2BT17TP%2BgP%2BayDyvrbvr8q8g%2BrPKv%2BS%2Bzr3P9jqLC7Kzv%2BEv%2BS%2BqLzPj8T6m4G7zPHvTDsgaUDTatkWRIZle5Ktm1xG2RbTt3n75nqFv5%2B8fkkf1VmbywdElVWZtedVvXhaNNNqDSLsjT%2Blz2f5bD1ti2p5vJCPd8b06XlZXMzbF18SrMf7NIkooc6u8zLV5gA%2BvjNi%2BPf67vP6eOmzer2KXWlE7%2B983B772G6eDo3g59ndV1cZn3v9bk%2B9unhHY6Hv7u0%2BxNiySbGkv76yHboRZPVPZuUacB48ICzxdpu9ozv619VUT7L8b3%2BvcqaJid06xMZ2q73kVJq122Ilvgs8rTos29iZOoZs9m9BzsHe4Tgg4Pdvb0DGsXvbSnO3z%2B4t7MNKu3QlsHHzlqv6yLKYG8N96jATRZmRsqP3j46fjBAfq%2BBksqNl%2B9%2BL1efPndFx%2B5z%2Bts2WQ8PzLN9LkFwUQgChf1tDI9hR92JxtvTUTQJNWwUeXxSr4u2iO1231hsfK03CeFaXHd9X5OXXMVHh5%2Bnsd7BxYsIGUrtY1yW6Tv8yudaw9ezJR7kwGXsPnfNgQAAA==");
            var result = Send(request, 30 * 1000, "POST", "UTF-8");
        }

        /// <summary>
        /// 获取HTTP请求结果
        /// </summary>
        /// <param name="requestUrl">请求URL</param>
        /// <param name="timeout">时限</param>
        /// <param name="method">请求方法</param>
        /// <param name="codeName">编码方式</param>
        /// <returns>HTTP请求结果</returns>
        public static string Send(string requestUrl, int timeout, string method, string codeName)
        {
            StreamReader sr = null;
            Stream stream = null;
            WebResponse wr = null;
            HttpWebRequest hp = null;
            string postData = null;
            try
            {
                // 执行http调用
                if (method.ToUpper() == "POST")
                {
                    string[] sarray = System.Text.RegularExpressions.Regex.Split(requestUrl, "\\?");
                    hp = (HttpWebRequest)WebRequest.Create(new Uri(sarray[0]));

                    if (sarray.Length >= 2)
                    {
                        postData = sarray[1];
                    }
                }
                else
                {
                    hp = (HttpWebRequest)WebRequest.Create(new Uri(requestUrl));
                }

                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                hp.Timeout = timeout;
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(codeName);
                if (postData != null)
                {
                    byte[] data = encoding.GetBytes(postData);
                    hp.Method = "POST";
                    hp.ContentType = "application/x-www-form-urlencoded";
                    hp.ContentLength = data.Length;
                    using (Stream ws = hp.GetRequestStream())
                    {
                        // 发送数据
                        ws.Write(data, 0, data.Length);
                    }
                }

                wr = hp.GetResponse();
                stream = wr.GetResponseStream();
                if (stream != null)
                {
                    sr = new StreamReader(stream, encoding);
                    var strResult = sr.ReadToEnd();
                    return strResult;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
                if (wr != null)
                {
                    wr.Close();
                }
                if (hp != null)
                {
                    hp.Abort();
                }
            }
        }

        /// <summary>
        /// GZip解压
        /// </summary>
        /// <param name="zippedString">压缩字符串</param>
        /// <returns>结果</returns>
        public static string GZipDecompress(string zippedString)
        {
            byte[] zippedData = Convert.FromBase64String(zippedString);
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                {
                    break;
                }
                outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return Encoding.UTF8.GetString(outBuffer.ToArray());
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="zippedString">压缩字符串</param>
        /// <returns></returns>
        public static string Decompress(string zippedString)
        {

            byte[] zippedData = Convert.FromBase64String(zippedString);
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return System.Text.Encoding.UTF8.GetString(outBuffer.ToArray());
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="conetnt">内容</param>
        /// <returns>结果</returns>
        private static string GetSign(string conetnt)
        {
            return Md5(conetnt + "720uReserveFlightPlanKey");
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="conetnt">内容</param>
        /// <returns>结果</returns>
        public static string Md5(string conetnt)
        {
            var md5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(conetnt))).Replace("-", string.Empty);
        }

        public static void Test2()
        {
            //1. 创建参数对象
            var para = new
            {
                CompanyId = 21818,
                TicketNo = "3242392998273",
                CompanyAccountNo = "1111",
                RequestTime = DateTime.Now.AddMinutes(2).ToString("yyyy-MM-dd HH:mm:ss"),
                NotifyUrl = "http://localhost:59982/Detr/DetrTn",
                FlightNo = "3U5005",
                DepartureTime = "2017-01-22 08:05",
                DepartureCode = "CTU",
                ArriveCode = "SHA",
                SearchType = 0
            };
            // 2.将参数序列化成JSON字符串
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(para);
            // 3.生成签名
            var sign = GetSign(data);
            // 4.组装请求参数
            var rq = string.Format("?param={0}&sign={1}", data, sign);
            // 5.POST请求
            var rel = Post("http://115.28.161.82:8056/ReserveFlightPlan/Index", rq);

            var x = rel.Result;
        }

        public static void Test()
        {
            string content = "sign=966bb96e3c8ce08c0a02bde143c6a812&notify_time=2017-02-08 06:54:27&pay_user_id=2088002182360169&pay_user_name=陈健&sign_type=MD5&success_details=RC20170207212943776284242101^santai96080@126.com^贵阳三泰航空服务有限公司^1.00^S^^20170207573547644^20170207212954|&notify_type=batch_trans_notify&pay_account_no=20880021823601690156&notify_id=eb10f0c558b6421f804f098480aec70k3a&batch_no=RC201702072129437762842421";

            HttpHelper http = new HttpHelper();
            string rel = http.HttpPost("http://localhost:3525/PrepareDeposit/AliPay/NewAliRechargeNotify.aspx", string.Empty,
                content, Encoding.UTF8, false, false, 5000);


        }

        private static async Task<string> Post(string url, string data)
        {
            // 设置HttpClientHandler的AutomaticDecompression
            ////var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip };
            Uri uri;
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);

            // 创建HttpClient（注意传入HttpClientHandler）
            using (var http = new HttpClient())
            {
                ////设置要的数据格式
                ////http.DefaultRequestHeaders.Add("Accept", "application/xml");
                //http.DefaultRequestHeaders.Add("Accept", "application/json");

                // 使用FormUrlEncodedContent做HttpContent
                var content = new ByteArrayContent(Encoding.UTF8.GetBytes(data));

                // await异步等待回应
                var response = await http.PostAsync(uri, content);

                // await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                var rel = await response.Content.ReadAsStringAsync();

                // 确保HTTP成功状态值
                response.EnsureSuccessStatusCode();

                return rel;
            }
        }
    }
}
