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
    /// <summary>
    /// 
    ///
    /// </summary>
    public interface IDictionaryResult
    {
        string GetResult(Language_t language);
    }
}
