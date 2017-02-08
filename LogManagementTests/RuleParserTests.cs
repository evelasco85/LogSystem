using System;
using System.Collections.Generic;
using System.Text;
using LogManagement.Event;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManagementTests
{
    [TestClass]
    public class RuleParserTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            string conditon = "((a == data) && (b==data)) && ((c == data) && (d == data))";
            IList<string>  postFixConditionTokens = RuleParser.GetInstance().ParseConditionToPostFixTokenList(new StringBuilder(conditon));
            
            Assert.AreEqual(15, postFixConditionTokens.Count);
            Assert.AreEqual("adata==bdata==&&cdata==ddata==&&&&", string.Concat(postFixConditionTokens));
        }
    }
}
