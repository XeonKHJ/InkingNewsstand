
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using InkingNewsstand.Utilities;

namespace InkingNewsstand.Test
{
    [TestClass]
    public class UnitTest1
    {
        static Translator translator = new Translator();

        [TestMethod]
        public async void TestMethod1()
        {
            var result = await translator.Translate("fuck", LanguageCode.zh);
        }

        public static async void TranslationTest()
        {
            var result = await translator.Translate("apple", LanguageCode.zh);
        }
    }
}
