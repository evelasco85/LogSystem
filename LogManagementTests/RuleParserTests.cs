using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LogManagement.Event;
using LogManagement.Event.Parameters;
using LogManagementTests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManagementTests
{
    [TestClass]
    public class RuleParserTests
    {
        [TestMethod]
        public void TestParseConditionToPostFixTokenList()
        {
            string conditon = "((a == data) && (b==data)) && ((c == data) && (d == data))";
            IList<string>  postFixConditionTokens = RuleParser.GetInstance().ParseConditionToPostFixTokenList(new StringBuilder(conditon));
            
            Assert.AreEqual(15, postFixConditionTokens.Count);
            Assert.AreEqual("a,data,==,b,data,==,&&,c,data,==,d,data,==,&&,&&", string.Join(",", postFixConditionTokens));
        }

        [TestMethod]
        public void Test()
        {
            IRuleParser parser = RuleParser.GetInstance();
            IDictionary<string, Tuple<string, IData, bool>> variables = RuleParser.GetInstance().GetVariables(Syntax.SourceCode1);
            string condition = parser.GetConditionDeclaration(Syntax.SourceCode1);
            IList<string> postFixConditionTokens = parser.ParseConditionToPostFixTokenList(new StringBuilder(condition));

            for (int index = 0; index < postFixConditionTokens.Count; index++)
            {
                string token = postFixConditionTokens[index];


            }
        }
    }
}
