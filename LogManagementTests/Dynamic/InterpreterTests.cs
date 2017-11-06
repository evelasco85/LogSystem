using LogManagement.Dynamic.Event;
using LogManagement.Dynamic.Event.Conditions;
using LogManagement.Dynamic.Event.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManagementTests.Dynamic
{
    [TestClass]
    public class InterpreterTests
    {
        [TestMethod]
        public void TestCondition()
        {
            Variable x = new Variable("index");
            IBooleanBase resultExpression = EqualToExpression.New(x, new Literal("L1", "1"));
            IBooleanBase all = AndExpression.New(resultExpression, BooleanExpression.True());
            Context context = new Context();

            context.Assign(x.Name, "1");

            bool invertResult = (NotExpression.New(all)).Evaluate(context);
            bool result =  all.Evaluate(context);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCondition2()
        {
            Variable x = new Variable("index");
            IBooleanBase resultExpression = GreatherThanExpression.New(x, new Literal("L2", 1));
            Context context = new Context();

            context.Assign(x.Name, 2);

            bool result = resultExpression.Evaluate(context);

            Assert.IsTrue(result);
        }
    }
}
