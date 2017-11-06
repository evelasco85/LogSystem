using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using LogManagement.Dynamic.Event.Conditions;
using LogManagement.Dynamic.Event.Parameters;

namespace LogManagement.Dynamic.Event
{
    public interface IRuleParser
    {
        IRule CreateRule(string sourceCode, out string successAction, out string failAction);
        IList<string> ParseConditionToPostFixTokenList(StringBuilder data);
    }

    public delegate IBooleanBase InstantiationDelegate<T>(T operand1, T operand2);

    public class RuleParser : IRuleParser
    {
        const string OPEN_PARENTHESIS = "(";
        const string CLOSE_PARENTHESIS = ")";
        const string SPACE = " ";
        public const string LABEL_PARAM = "param";
        public const string LABEL_LITERAL = "literal";
        public const string LABEL_TEST_DATA = "param-test-data";
        public const string LABEL_CONDITION = "condition";
        public const string LABEL_SUCCESS = "success";
        public const string LABEL_FAILED = "failure";
        public const string LABEL_ID = "id";

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

        private IDictionary<string, Type> _conditionDictionary = new Dictionary<string, Type>
        {
            {AndExpression.Operator, typeof(AndExpression)},
            {OrExpression.Operator, typeof(OrExpression)},
            {NotExpression.Operator, typeof(NotExpression)},
            {EqualToExpression.Operator, typeof(EqualToExpression)},
            {GreatherThanExpression.Operator, typeof(GreatherThanExpression)},
            {GreatherThanOrEqualToExpression.Operator, typeof(GreatherThanOrEqualToExpression)},
            {LessThanExpression.Operator, typeof(LessThanExpression)},
            {LessThanOrEqualToExpression.Operator, typeof(LessThanOrEqualToExpression)},
            {BooleanExpression.Operator, typeof(BooleanExpression)},
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

        Type GetOperatorType(string conditionOperator)
        {
            return _conditionDictionary[conditionOperator];
        }

        object CreateConditionInstance(Type conditionType, params object[] operands)
        {
            // create an object of the type
            return Activator.CreateInstance(conditionType, operands);
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

        void AddTestData(ref IContext context, string declaration)
        {
            if ((context == null) || (string.IsNullOrEmpty(declaration)))
                return;

            IList<string> segment = declaration
                .Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            string varName = segment[1];
            string value = segment[2];

            context.Assign(varName, DetectType(value));
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

        void GetDeclarationFromSourceCode(string sourceCode,
            ref IDictionary<string, Tuple<string, IData, bool>> variables ,
            ref IContext context,
            ref string conditionString,
            ref string successAction,
            ref string failAction,
            ref string id
            )
        {
            using (StringReader reader = new StringReader(sourceCode))
            {
                string line = string.Empty;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(LABEL_TEST_DATA))
                    {
                        AddTestData(ref context, line);
                    }
                    else if (line.StartsWith(LABEL_PARAM) || line.StartsWith(LABEL_LITERAL))
                    {
                        Tuple<string, IData, bool> variable = ConstructVariable(line);

                        variables.Add(variable.Item1, variable);
                    }
                    else if (line.StartsWith(LABEL_CONDITION))
                    {
                        int startIndex = line.IndexOf(":") + 1;

                        conditionString = line.Substring(startIndex);
                    }
                    else if (line.StartsWith(LABEL_SUCCESS))
                    {
                        int startIndex = line.IndexOf(":") + 1;

                        successAction = line.Substring(startIndex);
                    }
                    else if (line.StartsWith(LABEL_FAILED))
                    {
                        int startIndex = line.IndexOf(":") + 1;

                        failAction = line.Substring(startIndex);
                    }
                    else if (line.StartsWith(LABEL_ID))
                    {
                        int startIndex = line.IndexOf(":") + 1;

                        id = line.Substring(startIndex);
                    }
                }
            }
        }

        IBooleanBase ConvertTokenToCondition(string conditionTokens, IDictionary<string, Tuple<string, IData, bool>> variables, ref IRule rule)
        {
            IList<string> postFixConditionTokens = ParseConditionToPostFixTokenList(new StringBuilder(conditionTokens));

            Stack<object> stack = new Stack<object>();

            for (int index = 0; index < postFixConditionTokens.Count; index++)
            {
                string token = postFixConditionTokens[index];

                if (IsOperator(token))
                {
                    IList<object> operands = new List<object>();

                    if (((token == AndExpression.Operator) || (token == OrExpression.Operator)))
                    {
                        while (stack.Any())
                        {
                            operands.Add(stack.Pop());
                        }
                    }
                    else
                    {
                        operands.Add(stack.Pop());
                        operands.Add(stack.Pop());
                    }

                    Type operatorType = GetOperatorType(token);

                    stack.Push(CreateConditionInstance(operatorType, operands.ToArray()));
                }
                else
                {
                    stack.Push(variables[token].Item2);
                    rule.AddVariableScope(variables[token].Item2, variables[token].Item3);
                }
            }

            IBooleanBase condition = (IBooleanBase)stack.Pop();

            return condition;
        }

        public IRule CreateRule(string sourceCode, out string successAction, out string failAction)
        {
            IDictionary<string, Tuple<string, IData, bool>> variables = new Dictionary<string, Tuple<string, IData, bool>>();
            IContext context = new Context();
            string conditionTokens = string.Empty;
            
            successAction = string.Empty;
            failAction = string.Empty;

            IRule rule = null;

            try
            {
                string id = string.Empty;

                GetDeclarationFromSourceCode(sourceCode, ref variables, ref context, ref conditionTokens, ref successAction, ref failAction, ref id);

                rule = new Rule(id);

                IBooleanBase condition = ConvertTokenToCondition(conditionTokens, variables, ref rule);

                if (!condition.Evaluate(context))
                    throw new ArgumentException("Built-in self-test for rule creation failed.");

                rule.SetCondition(condition);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to parse source code, please see inner exception.", ex);
            }
            
            return rule;
        }
    }
}
