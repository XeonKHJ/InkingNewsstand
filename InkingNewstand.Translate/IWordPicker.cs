using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft;


namespace InkingNewstand.Translate
{
    public interface IWordPicker
    {
        /// <summary>
        /// 选词器
        /// </summary>
        /// <param name="word">选中的单词</param>
        /// <param name="sourceLanguage">目标语言</param>
        /// <returns>词语的词典查询结果</returns>
        DictionaryResult Lookfor(string word, Language_t sourceLanguage);
    }
}
