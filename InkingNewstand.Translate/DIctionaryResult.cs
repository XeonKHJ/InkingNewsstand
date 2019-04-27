using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft;

namespace InkingNewstand.Translate
{
    internal class Translation_result
    {

        //错误码，翻译结果无法正常返回
        public string Error_code { get; set; }
        public string Error_msg { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Query { get; set; }
        //翻译正确，返回的结果
        //这里是数组的原因是百度翻译支持多个单词或多段文本的翻译，在发送的字段q中用换行符（\n）分隔
        public Translation[] Trans_result { get; set; }

        public Translation_result(string error_code, string error_msg, string from, string to, string query, Translation[] trans_result)
        {
            Error_code = error_code;
            Error_msg = error_msg;
            From = from;
            To = to;
            Query = query;
            Trans_result = trans_result;
        }

    }

    public class DictionaryResult : IDictionaryResult
    {
        internal Translation_result f;
        //错误码，翻译结果无法正常返回
        public string Error_code { get; set; }
        public string Error_msg { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Query { get; set; }
        //翻译正确，返回的结果
        //这里是数组的原因是百度翻译支持多个单词或多段文本的翻译，在发送的字段q中用换行符（\n）分隔
        internal Translation[] Trans_result { get; set; }

        //实例化类时从选词器获得参数
        public Language_t from_langguage;
        public Language_t to_language;
        public string word;




        //对字符串做md5加密
        private static string GetMD5WithString(string input)
        {
            if (input == null)
            {
                return null;
            }
            MD5 md5Hash = MD5.Create();
            //将输入字符串转换为字节数组并计算哈希数据  
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            //创建一个 Stringbuilder 来收集字节并创建字符串  
            StringBuilder sBuilder = new StringBuilder();
            //循环遍历哈希数据的每一个字节并格式化为十六进制字符串  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            //返回十六进制字符串  
            return sBuilder.ToString();
        }

        private readonly string appId = "20190308000275134";  //测试错误时加了01
        private readonly string password = "BXDte3KBBP5eKDyLrIW_";
        private string jsonResult = String.Empty;
        public string GetResult(Language_t tolanguage)
        {
            to_language = tolanguage;

            //源语言
            string languageFrom = from_langguage.ToString().ToLower();
            //目标语言
            string languageTo = to_language.ToString().ToLower(); ;
            //随机数
            string randomNum = System.DateTime.Now.Millisecond.ToString();
            //md5加密
            string md5Sign = GetMD5WithString(appId + word + randomNum + password);
            //url
            string url = String.Format("http://api.fanyi.baidu.com/api/trans/vip/translate?q={0}&from={1}&to={2}&appid={3}&salt={4}&sign={5}",
                HttpUtility.UrlEncode(word, Encoding.UTF8),
                languageFrom,
                languageTo,
                appId,
                randomNum,
                md5Sign
                );
            WebClient wc = new WebClient();
            try
            {
                jsonResult = wc.DownloadString(url);
            }
            catch
            {
                jsonResult = string.Empty;
            }

            Translation_result rb = JsonConvert.DeserializeObject<Translation_result>(jsonResult);

            this.f = rb;

            if (rb != null && rb.Trans_result != null)
            {
                return rb.Trans_result[0].Dst;
            }
            if (rb != null && rb.Error_code != null)
            {
                throw new Exception("翻译出错，错误码：" + rb.Error_code + "，错误信息：" + rb.Error_msg);
            }
            throw new Exception("网络未连接，请检查网络！");
        }
    }
}

