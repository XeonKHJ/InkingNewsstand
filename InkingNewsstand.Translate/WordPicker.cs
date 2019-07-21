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


namespace InkingNewsstand.Translate
{
    public class WordPicker : IWordPicker
    {

        public DictionaryResult Lookfor(string word, Language_t sourceLanguage)
        {
            DictionaryResult result = new DictionaryResult
            {
                word = word,
                from_langguage = sourceLanguage
            };
            return result;
            //throw new NotImplementedException();
        }
    }
}
