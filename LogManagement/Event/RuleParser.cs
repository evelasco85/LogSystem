using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using LogManagement.Event.Conditions;
using LogManagement.Event.Parameters;

namespace LogManagement.Event
{
    public interface IRuleParser
    {
        IList<string> ParseConditionToPostFixTokenList(StringBuilder data);
        bool IsOperator(string input);
        IDictionary<string, Tuple<string, IData, bool>> GetVariables(string sourceCode);
        string GetConditionDeclaration(string sourceCode);
    }

    public class RuleParser : IRuleParser
    {
        const string OPEN_PARENTHESIS = "(";
        const string CLOSE_PARENTHESIS = ")";
        const string SPACE = " ";
        public const string LABEL_PARAM = "param";
        public const string LABEL_LITERAL = "literal";

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

        private List<Type> _expectedTypes = new List<Type>
        {
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime),
            typeof(string),
        };

        public static IRuleParser GetInstance()
        {
            return s_instance;
        }

        private RuleParser()
        {
        }

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

        public bool IsOperator(string input)
        {
            return _operator.Any(op => op == input);
        }

        public IDictionary<string, Tuple<string, IData, bool>> GetVariables(string sourceCode)
        {
            IDictionary<string, Tuple<string, IData, bool>> variables = new Dictionary<string, Tuple<string, IData, bool>>();

            using (StringReader reader = new StringReader(sourceCode))
            {
                string line = string.Empty;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(LABEL_PARAM) || line.StartsWith(LABEL_LITERAL))
                    {
                        Tuple<string, IData, bool> variable = ConstructVariable(line);

                        variables.Add(variable.Item1, variable);
                    }
                }
            }

            return variables;
        }

        public string GetConditionDeclaration(string sourceCode)
        {
            string declaration = string.Empty;

            if (string.IsNullOrEmpty(sourceCode) || (!sourceCode.Contains("condition")))
                return declaration;

            int startIndex = sourceCode.IndexOf("condition");

            startIndex = startIndex + sourceCode.Substring(startIndex).IndexOf(":") + 1;

            int endIndex = startIndex + sourceCode.Substring(startIndex).IndexOf(Environment.NewLine);

            declaration = sourceCode.Substring(startIndex, endIndex - startIndex);

            return declaration;
        }
        Tuple<string, IData, bool> ConstructVariable(string declaration)
        {
            if(string.IsNullOrEmpty(declaration))
                return null;

            IList<string> segment = declaration
                .Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            string type = segment[0];
            string varName = segment[1];
            string paramNameOrValue = segment[2];
            IData data = null;

            switch (type)
            {
                case "param":
                    data = new Variable(paramNameOrValue);
                    break;
                case "literal":
                    data = new Literal(varName, DetectType(paramNameOrValue));
                    break;
            }

            string required = ((segment.Count == 4) && (!string.IsNullOrEmpty(segment[3]))) ? segment[3] : "false";

            return new Tuple<string, IData, bool>(
                varName,
                data,
                bool.Parse(required)
                );
        }

        object DetectType(string stringValue)
        {
            foreach (var type in _expectedTypes)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);

                if (converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        object newValue = converter.ConvertFromInvariantString(stringValue);

                        if (newValue != null)
                            return newValue;
                    }
                    catch
                    {
                        continue;
                    }

                }
            }

            return null;
        }
    }
}
