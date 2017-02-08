using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogManagement.Event.Conditions;

namespace LogManagement.Event
{
    public interface IRuleParser
    {
        IList<string> ParseConditionToPostFixTokenList(StringBuilder data);
    }

    public class RuleParser : IRuleParser
    {
        static IRuleParser s_instance = new RuleParser();

        private IList<string> _operator = new List<string>
        {
            "=",
            "<",
            ">",
            "!",
            "&",
            AndExpression.Operator,
            OrExpression.Operator,
            NotExpression.Operator,
            EqualToExpression.Operator,
            GreatherThanExpression.Operator,
            GreatherThanOrEqualToExpression.Operator,
            LessThanExpression.Operator,
            LessThanOrEqualToExpression.Operator,
            BooleanExpression.Operator,
        };

        public static IRuleParser GetInstance()
        {
            return s_instance;
        }

        private RuleParser()
        {
        }

        const string OPEN_PARENTHESIS = "(";
        const string CLOSE_PARENTHESIS = ")";
        const string SPACE = " ";

        public IList<string> ParseConditionToPostFixTokenList(StringBuilder data)
        {
            IList<string> tokenList = new List<string>();

            if((data == null) || (data.Length < 1))
                return tokenList;

            Stack<string> operatorStack = new Stack<string>();
            IList<string> token = new List<string>();

            for (int index = 0; index < data.Length; index++)
            {
                string input = data[index].ToString();

                if (input == OPEN_PARENTHESIS) 
                    operatorStack.Push(input);
                else if (input == SPACE)
                    continue;
                else if (input == CLOSE_PARENTHESIS)
                {
                    AddToken(ref tokenList, string.Concat(token));
                    token.Clear();

                    AddToken(ref tokenList, GetOperators(ref operatorStack));
                    token.Clear();

                }
                else if (IsOperator(input))
                {
                    operatorStack.Push(input);

                    AddToken(ref tokenList, string.Concat(token));
                    token.Clear();
                }
                else
                {
                    token.Add(input);
                }
            }

            while (operatorStack.Any())
            {
                token.Add(operatorStack.Pop());
            }

            AddToken(ref tokenList, string.Concat(token));
            token.Clear();

            return tokenList;
        }


        string GetOperators(ref Stack<string> operatorStack)
        {
            IList<string> token = new List<string>();
            string item = string.Empty;

            do
            {
                item = operatorStack.Pop();

                if (IsOperator(item))
                    token.Add(item);
            } while (item != OPEN_PARENTHESIS);

            return string.Concat(token);
        }

        void AddToken(ref IList<string> tokenList, string item)
        {
            if (!string.IsNullOrEmpty(item))
                tokenList.Add(item);
        }

        bool IsOperator(string input)
        {
            return _operator.Any(op => op == input);
        }
    }
}
