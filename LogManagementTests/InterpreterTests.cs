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
            EventBoolean resultExpression = new EqualToExpression(x, new EventLiteral("L1", "1"));
            EventBoolean all = new AndExpression(resultExpression, EventLiteralBoolean.True());
            EventContext context = new EventContext();

            context.Assign(x.Name, "1");

            bool invertResult = (new NotExpression(all)).Evaluate(context);
            bool result =  all.Evaluate(context);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestMethod2()
        {
            EventVariable x = new EventVariable("index");
            EventBoolean resultExpression = new GreatherThanExpression(x, new EventLiteral("L2", 1));
            EventContext context = new EventContext();

            context.Assign(x.Name, 2);

            bool result = resultExpression.Evaluate(context);

            Assert.IsTrue(result);
        }
    }
}
