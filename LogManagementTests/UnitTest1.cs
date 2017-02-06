﻿using LogManagement.Event;
using LogManagement.Event.Conditions;
using LogManagement.Event.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManagementTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            EventVariable x = new EventVariable("index");
            EventBoolean resultExpression = new EventEqualExpression(x, new EventLiteral("1"));
            EventContext context = new EventContext();

            context.Assign(x, "1");

            bool result = resultExpression.Evaluate(context);
        }
    }
}