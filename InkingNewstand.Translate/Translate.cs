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


    internal class Translation
    {

        public string Src { get; set; }
        public string Dst { get; set; }

        public Translation(){

        }


    }
    /*
    public enum Language
    {
        //百度翻译API官网提供了多种语言，这里只列了几种
        auto = 0,
        zh = 1,
        en = 2,
        cht = 3,
        yue,
        wyw,
        jp,
        kor,
        fra,
        spa,
        th,
        ara,
        ru,
        pt,
        de,
        it,
        el,
        nl,
        pl,
        bul,
        est,
        dan,
        fin,
        cs,
        rom,
        slo,
        swe,
        hu,
        vie,
    }
    */
    public enum Language_t
    {
        //百度翻译API官网提供了多种语言，这里只列了几种
        auto = 0,
        zh = 1,
        en = 2,
        cht = 3,
        yue,
        wyw,
        jp,
        kor,
        fra,
        spa,
        th,
        ara,
        ru,
        pt,
        de,
        it,
        el,
        nl,
        pl,
        bul,
        est,
        dan,
        fin,
        cs,
        rom,
        slo,
        swe,
        hu,
        vie,
    }
    /*
    auto
    自动检测
    zh
    中文
    en
    英语
    yue
    粤语
    wyw
    文言文
    jp
    日语
    kor
    韩语
    fra
    法语
    spa
    西班牙语
    th
    泰语
    ara
    阿拉伯语
    ru
    俄语
    pt
    葡萄牙语
    de
    德语
    it
    意大利语
    el
    希腊语
    nl
    荷兰语
    pl
    波兰语
    bul
    保加利亚语
    est
    爱沙尼亚语
    dan
    丹麦语
    fin
    芬兰语
    cs
    捷克语
    rom
    罗马尼亚语
    slo
    斯洛文尼亚语
    swe
    瑞典语
    hu
    匈牙利语
    cht
    繁体中文
    vie
    越南语*/

}
