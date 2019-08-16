
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using InkingNewsstand.Utilities;
using InkingNewsstand.Model;

namespace InkingNewsstand.Test
{
    [TestClass]
    public class UnitTest1
    {
        static Translator translator = new Translator();

        [TestMethod]
        public async void TestMethod1()
        {
            var result = await translator.TranslateAsync("fuck", LanguageCode.zh);
        }

        public static async void TranslationTest()
        {
            var result = await translator.TranslateAsync("apple", LanguageCode.zh);
        }

        
    }
}
