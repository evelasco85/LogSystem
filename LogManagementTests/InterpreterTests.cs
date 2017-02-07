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
            Variable x = new Variable("index");
            BooleanBase resultExpression = new EqualToExpression(x, new Literal("L1", "1"));
            BooleanBase all = new AndExpression(resultExpression, LiteralBoolean.True());
            Context context = new Context();

            context.Assign(x.Name, "1");

            bool invertResult = (new NotExpression(all)).Evaluate(context);
            bool result =  all.Evaluate(context);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Variable x = new Variable("index");
            BooleanBase resultExpression = new GreatherThanExpression(x, new Literal("L2", 1));
            Context context = new Context();

            context.Assign(x.Name, 2);

            bool result = resultExpression.Evaluate(context);

            Assert.IsTrue(result);
        }
    }
}
