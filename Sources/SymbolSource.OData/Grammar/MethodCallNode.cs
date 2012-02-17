using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Interpreter.Ast;

namespace SymbolSource.OData
{
    public class MethodCallNode : AstNode, IExpression
    {
        public string MethodName { get; private set; }
        public IList<AstNode> Parameters { get; private set; }

        public MethodCallNode()
        {
            Parameters = new List<AstNode>();            
        }

        public override void Init(Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            
            var method = treeNode.MappedChildNodes[0];
            MethodName = method.FindTokenAndGetText();


            for (int i=1; i<treeNode.MappedChildNodes.Count; i++)
            {
                var parseTreeNode = treeNode.MappedChildNodes[i];
                Parameters.Add(AddChild("Arg" + i, parseTreeNode));
            }

            AsString = MethodName + " (method)";
        }

        public string Translate()
        {
            var parameters = Parameters.OfType<IExpression>().ToArray();
            if(parameters.Length == Parameters.Count)
            {
                switch (MethodName)
                {
                    case "replace":
                        return TwoParamsMethodCall("Replace", parameters);
                    case "substring":
                        return Substring(parameters);
                    case "tolower":
                        return parameters[0].Translate() + ".ToLower()";
                    case "toupper":
                        return parameters[0].Translate() + ".ToUpper()";
                    case "trim":
                        return parameters[0].Translate() + ".Trim()";
                    case "concat":
                        return Concat(parameters);

                    case "substringof":
                        return OneParamMethodCall("Contains", parameters);
                    case "startswith":
                        return OneParamMethodCall("StartsWith", parameters);
                    case "endswith":
                        return OneParamMethodCall("EndsWith", parameters);

                    case "length":
                        return PropertyCall("Length", parameters);
                    case "indexof":
                        return OneParamMethodCall("IndexOf", parameters);

                    case "second":
                        return PropertyCall("Second", parameters);
                    case "minute":
                        return PropertyCall("Minute", parameters);
                    case "hour":
                        return PropertyCall("Hour", parameters);
                    case "day":
                        return PropertyCall("Day", parameters);
                    case "month":
                        return PropertyCall("Month", parameters);
                    case "year":
                        return PropertyCall("Year", parameters);

                    case "round":
                        return MathMethodCall("Round", parameters);
                    case "floor":
                        return MathMethodCall("Floor", parameters);
                    case "ceiling":
                        return MathMethodCall("Ceiling", parameters);

                    case "isof":
                        return IsOf(parameters);
                }
            }
            throw new NotImplementedException();
        }

        private string MathMethodCall(string methodName, IList<IExpression> parameters)
        {
            return string.Format("Math.{0}({1})",
                                 methodName,
                                 parameters[0].Translate());
        }

        private string IsOf(IList<IExpression> parameters)
        {
            throw new NotSupportedException();
        }

        private string OneParamMethodCall(string methodName, IList<IExpression> parameters)
        {
            return string.Format("{0}.{1}({2})",
                                 parameters[0].Translate(),
                                 methodName,
                                 parameters[1].Translate());
        }
        private string TwoParamsMethodCall(string methodName, IList<IExpression> parameters)
        {
            return string.Format("{0}.{1}({2}, {3})",
                                 parameters[0].Translate(),
                                 methodName,
                                 parameters[1].Translate(),
                                 parameters[2].Translate());
        }
        private string PropertyCall(string propertyName, IList<IExpression> parameters)
        {
            return string.Format("{0}.{1}",
                                 parameters[0].Translate(),
                                 propertyName);
        }

        private string Substring(IList<IExpression> parameters)
        {
            if (parameters.Count == 2)
                return OneParamMethodCall("Substring", parameters);
            if(parameters.Count == 3)
                return TwoParamsMethodCall("Substring", parameters);

            throw new ArgumentException();
        }

        private static string Concat(IList<IExpression> parameters)
        {
            return string.Format("{0} + {1}", parameters[0].Translate(), parameters[1].Translate());
        }
    }
}