using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using InkingNewstand.Utilities;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace InkingNewstand.Test.Utilities
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ImgTest : Page
    {
        public ImgTest()
        {
            this.InitializeComponent();
        }

        public List<ImgsViewModel> Imgs
        {
            get
            {
                List<ImgsViewModel> imgs = new List<ImgsViewModel>();
                string html1 = "小米没有个性https://techcrunch.cn/2019/02/23/still-xiaomi/?ncid=rsshttps://techcrunch.cn/2019/02/23/still-xiaomi/#respondSat, 23 Feb 2019 04:45:39 GMT2019-02-23T04:45:39Z2019-02-23T04:45:39ZJesse ChanJesse ChanCrunchNetwork观点http://techcrunch.cn/?p=64765<img width=1024 height=678 src=\"https://files.techcrunch.cn/2019/02/index1-e1550896829363.jpg?w=1024\" class=\"attachment-post-thumbnail size-post-thumbnail wp-post-image\" alt=\"\" style=\"float:left;margin:0 10px 7px 0;\"> <p><strong>编者按：本文是动点科技/TechCrunch 中文版周末副刊「<a target=_blank href=\"https://tan.today/\" rel=\"noopener noreferrer\">TAN</a>」专栏文章，作者 Jesse 是播客「<a target=_blank href=\"https://jiaocha.io/\" rel=\"noopener noreferrer\">交差点</a>」主持人。</strong></p>\n <p>雷总在台上不停地对比，陈述小米 9 在哪些方面比 iPhone 更好，在哪些方面又比华为更好，力求让小米 9 进入它们所在的那个次元。最后，标示着价格数字轰然落地，他兴奋地宣布，这样一台能和苹果、华为比肩的手机，只要 2999。台下的欢呼和弹幕里大片的「真香」，将气氛推向高潮。</p>\n <p>我早已不再怀疑小米作为一家公司的实力，更相信小米做出好产品的能力，但被「性价比」压得直不起腰的小米，到底要怎么做，才能真正摆脱这层「低端」的桎梏？</p>\n<h2>「堆」与「仿」</h2>\n<p><img src=\"https://tan.today/wp-content/uploads/2019/02/dear.jpg\" class=\"alignnone size-full wp-image-76593\" alt=\"\" width=1231 height=985></p>\n<p>「堆料」可能是这场发布会上出现频率最高的一个词。</p>\n <p>除了电池容量上的些许妥协，小米 9 拿出了一套几乎无懈可击的配置。屏幕指纹、素质优秀的三摄像头、极窄的边框、骁龙 855、20W 无线快充……这是一台标准的「水桶机」，什么都很好，又什么都不「突出」。所以这场发布会才呈现出了这样的一个结构，先在微博上连续一周把所有足够强势配置摆出来，宣明自己的「旗舰」身份，最后的发布会其实也就是「价格公布会」。</p>\n <p>就像老罗说的，大家都是「方案整合商」。但即便如此，不同公司的「方案整合能力」却有着巨大的不同。小米是全世界最会做方案整合的公司，没有之一。这不只在于小米只用一年时间就在 2011 年搞出了一个相当不错的智能手机，更在于它在各种 IoT 领域几乎做什么成什么。小米是一家硬件公司，它最根本的基因就是对方案、对供应链进行高效整合、把控的能力。</p>\n <p>这让我们不难理解，为什么小米要用「堆」这个词来形容自己的产品开发。原料都是现成的，我用多快好省地方式将它们堆在一起，就能做出性价比够高的产品。这没错，但真正最顶级的产品开发者，无论是苹果、三星，还是这两年的华为，它们的产品都不只是「堆」出来的。它们不仅仅是方案整合商，更是方案的提出者和推动者，它们会自研技术方案，会通过投资、收购上游公司来获得技术方案。我们常常在专利文件里寻找苹果下一代产品的蛛丝马迹，但对小米来说，它最主打的产品却有高度的「可预测性」，因为它是「堆」出来的。</p>\n <p>这场发布会上还有另一个很难忽略的问题。</p>\n <p>从 Game Turbo 的营销关键词、「18 个月不卡顿」的梗，到和 macOS Mojave 高度雷同的壁纸、「小爱捷径」，小米似乎一直在试图模仿苹果和华为，来让自己和它们站到同一个海拔上。我不想在这里做过多道德审判，即使这一切都不会引发任何法律纠纷，或者说苹果和华为不会真的去告小米，仍掩盖不了小米在市场影响层面的懒惰。既是你营销的目的就是「让大家把小米 9 和 iPhone、Mate 20 拿到一起比」，你也完全不需要像这样去模仿，去蹭概念。</p>\n <p>雷军在发布会的最后自己感叹，当年不该说那句「没有设计就是最好的设计」，把自己的品牌都做 low 了。但这件事到现在都完全没有停止。每一次蹭别人的概念，拙劣地模仿，试图从通过这种做法获得流量，最后都是在给自己挖坑。</p>\n <h2>一家公司的「个性」</h2>\n<p><img src=\"https://tan.today/wp-content/uploads/2019/02/wangyuan1.jpg\" class=\"alignnone size-full wp-image-76596\" alt=\"\" width=1233 height=914></p>\n <p>黄章说小米 9 是「眼高手低」，我觉得恰恰相反，小米的手从来都不「低」，它低的是「眼」。</p>\n <p>雷军从最初开始，一直亲自发布小米的几乎每一款新产品。靠着自己的真诚，他改变了包括我在内很多人对小米的偏见。他是一个真正的实干家，有着作为一个创始人极为宝贵的热情和谦逊。但我从雷军身上看到的，永远是对「做公司」这件事的热忱，而不是对「做产品」这件事的热忱。优秀的「产品人」大多是有偏好的，但雷军似乎没有什么偏好，他一直追着业界最优秀的公司跑，追着用户喊得最响的呼声跑。</p>\n <p>我不清楚小米内部具体有多少人在从事「新技术」、「新方案」的研发，但从它能成功推出一代 MIX（虽然我不喜欢一代 MIX 这个产品本身，但我喜欢它背后体现的那口气）不难看出，小米内部是有人在做「往前走」这件事的。但光「往前走」还不够，vivo、oppo 都在往前走，它们「往前走」的产品都暂时还没有取得特别大的成功。</p>\n <p>想成为真正最顶尖的公司，你需要的不仅仅要往前走，有前瞻性的视野，还要有敏锐的嗅觉，独到的思考，你要比用户先想到他们的需要，而不只是去听用户有什么需要。最终，你要有对自己内心的笃定坚持和一点冒险的勇气。这一切合在一起，其实就是一个创始人、一家公司的「个性」，就是小米最缺的东西。</p>\n <p>这不是一件简单的事。甚至换个角度说，国内比雷军更固执、更愿意冒险、更有个性的人，现在的下场都有点惨，比如罗永浩。但对于小米来说，想要真正突破自己头顶的那一层屏障，就不可能总是只做一个「价格」的颠覆者，而不做一个「产品」的颠覆者。最后，这总是一件要冒险的事。</p>\n <h2>金子的价值</h2>\n <p>这周，我买了一台索尼黑卡 6 相机，它可能是市面上最强、也最贵的卡片机。有朋友问我，这个价位为什么不买一套入门的单反系统，「性价比」比黑卡可高太多了。我半开玩笑地回复说：「如果我是只看性价比的人，我早就发财了。」我看中了黑卡 6 上面诸多索尼独有的「黑科技」，它的便携，它对全焦段的覆盖。它是一个无可替代的好产品，值得我花很多钱，把它当作我开始学习拍照的入门机。</p>\n <p>优秀的，独一无二的产品从来都是这样的。它们可能很贵，但那些无可取代的产品特性让它们永远可以击中用户，也永远都可以卖得起一个配得上其价值的价格。真正好的产品，会萦绕在用户的眼前，脑海里，让他们忘掉「理性」，为之神魂颠倒，最后不顾一切地去买到它。</p>\n <p>我记得第一次拥有一个 Walkman 时，爱不释手心跳加速的感觉；记得当年双手捧起 iPhone 4 时，手心不停往外冒汗的感觉；记得第一次拥有任天堂 3ds 时，玩到废寝忘食的喜悦……这就是真正的「感动人心」。</p>\n <p>小米能做出一个「让人们几经比较之后决定买它」的小米 9，这很棒。但如何做出一个「让人看过一眼、摸过一次就睡不着觉，攒钱买到之后双手颤抖」的产品，是小米需要实现的自我突破。</p>\n<img width=1200 height=794 src=\"https://files.techcrunch.cn/2019/02/index1-e1550896829363.jpg?w=1200\" class=\"attachment-large size-large wp-post-image\" alt=\"\">&#160;编者按：本文是动点科技/TechCrunch 中文版周末副刊「TAN」专栏文章，作者 Jesse 是播客「交差点」主持人。 雷总在台上不停地对比，陈述小米 9 在哪些方面比 iPhone 更好，在哪些方面又比华为更好，力求让小米 9… <a href=\"https://techcrunch.cn/2019/02/23/still-xiaomi/?ncid=rss\">Read More</a>https://techcrunch.cn/2019/02/23/still-xiaomi/feed/0%E5%B0%8F%E7%B1%B3%E6%B2%A1%E6%9C%89%E4%B8%AA%E6%80%A7 index1 hellyeahz";
                var abc = HtmlConverter.GetImages(html1);
                foreach(var imgUri in abc)
                {
                    imgs.Add(new ImgsViewModel(imgUri));
                }
                return imgs;
            }
        }
    }

    public class ImgsViewModel
    {
        public string Url;
        public ImgsViewModel(Uri url)
        {
            Url = url.AbsoluteUri;
        }
    }
}
