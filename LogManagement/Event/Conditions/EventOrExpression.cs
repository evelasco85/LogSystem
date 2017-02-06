namespace LogManagement.Event.Conditions
{
    public interface IEventOrExpression
    {
        
    }

    public class EventOrExpression : EventBoolean, IEventOrExpression
    {
         private IEventBoolean _operand1;
        private IEventBoolean _operand2;

        public override bool Evaluate(IEventContext context)
        {
            return _operand1.Evaluate(context) || _operand2.Evaluate(context);
        }

        public EventOrExpression(IEventBoolean operand1, IEventBoolean operand2)
        {
            _operand1 = operand1;
            _operand2 = operand2;
        }
    }
}
