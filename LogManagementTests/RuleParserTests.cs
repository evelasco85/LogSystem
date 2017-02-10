using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LogManagement.Event;
using LogManagement.Event.Conditions;
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
        public void TestDynamicRuleCreation()
        {
            IRuleParser parser = RuleParser.GetInstance();

            string successAction;
            string failAction;
            IRule rule = parser.CreateRule(Files.SourceCode1, out successAction, out failAction);

            Assert.IsNotNull(rule);
            Assert.AreEqual("TEST ID 123", rule.Id);
            Assert.AreEqual("PREDEFINE_ACTION_1", successAction);
            Assert.AreEqual("PREDEFINE_ACTION_2", failAction);
        }
    }
}
