using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Net;

namespace InkingNewsstand.Utilities
{
    public enum LanguageCode { auto, zh, en, yue, wyw, jp};
    public class Translator
    {
        private readonly HttpClient translationProvider;
        private readonly string appId = "20190308000275134";
        private readonly string key = "BXDte3KBBP5eKDyLrIW_";
        private readonly Random random = new Random();
        private readonly Uri translationProviderUrl = new Uri("http://api.fanyi.baidu.com/api/trans/vip/");
        public Translator()
        {
            translationProvider = new HttpClient
            {
                BaseAddress = translationProviderUrl
            };
        }

        public string Translate(string stringToTranslate, LanguageCode language)
        {
            int salt = random.Next();
            string parameterList = "translate?q=" + stringToTranslate
                                 + "&from=auto" + "&to=" + language
                                 + "&appid=" + appId + "&salt=" + salt.ToString()
                                 + "&sign=" + GetMD5WithString(appId + stringToTranslate + salt + key);

            TranslationResponse result;
            var getTask = translationProvider.GetAsync(parameterList);
            getTask.Wait();
            using (var responseMessage = getTask.Result)
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new System.Net.Http.HttpRequestException();
                }

                try
                {
                    var convertToStringTask = responseMessage.Content.ReadAsStringAsync();
                    convertToStringTask.Wait();
                    var responseString = convertToStringTask.Result;
                    result = JsonConvert.DeserializeObject<TranslationResponse>(responseString);
                }
                catch (ArgumentException ex)
                {
                    throw new TranslationException();
                }
            }
            return result.Trans_result[0].Dst;
        }

        public async Task<string> TranslateAsync(string stringToTranslate, LanguageCode language)
        {
            int salt = random.Next();
            string parameterList = "translate?q=" + stringToTranslate
                                 + "&from=auto" + "&to=" + language
                                 + "&appid=" + appId + "&salt=" + salt.ToString()
                                 + "&sign=" + GetMD5WithString(appId + stringToTranslate + salt + key);

            TranslationResponse result;
            using (var responseMessage = await translationProvider.GetAsync(parameterList))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    throw new System.Net.Http.HttpRequestException();
                }

                try
                {
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<TranslationResponse>(responseString);
                }
                catch (ArgumentException ex)
                {
                    throw new TranslationException();
                }
            }
            return result.Trans_result[0].Dst;
        }

        //MD5编码
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

        private class TranslationResponse
        {
            string Error_code;
            string Error_msg;
            string From;
            string To;
            string Query;
            public TranslationResult[] Trans_result;

            public TranslationResponse(string error_code, string error_msg, string from, string to, string query, TranslationResult[] trans_result)
            {
                Error_code = error_code;
                Error_msg = error_msg;
                From = from;
                To = to;
                Query = query;
                Trans_result = trans_result;
            }
        }

        private class TranslationResult
        {
            public string Src;
            public string Dst;

            public TranslationResult(string src, string dst)
            {
                Src = src;
                Dst = dst;
            }
        }
    }

    public class TranslationException : Exception
    {
        
    }
}
