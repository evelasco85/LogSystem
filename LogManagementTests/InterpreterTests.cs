using LogManagement.Event;
using LogManagement.Event.Conditions;
using LogManagement.Event.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManagementTests
{
    [TestClass]
    public class InterpreterTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            EventVariable x = new EventVariable("index");
            EventBoolean resultExpression = new EventEqualExpression(x, new EventLiteral("1"));
            EventBoolean all = new EventAndExpression(resultExpression, EventLiteralBoolean.False());
            EventContext context = new EventContext();

            context.Assign(x, "1");

            bool result = all.Evaluate(context);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestMethod2()
        {
            EventVariable x = new EventVariable("index");
            EventBoolean resultExpression = new EventGreatherThanExpression(x, new EventLiteral(1));
            EventContext context = new EventContext();

            context.Assign(x, 2);

            bool result = resultExpression.Evaluate(context);

            Assert.IsTrue(result);
        }
    }
}
