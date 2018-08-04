using System;
using System.Diagnostics;
using CommonLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonLibTest
{
    [TestClass]
    public class HttpLibTest
    {
        [TestMethod]
        public void TestGetAsString()
        {
            var rslt = HttpUtils.GetAsString("http://www.baidu.com");
            var content = rslt.Result;
            Assert.IsTrue(content.IndexOf("百度")>-1);
        }
    }
}
