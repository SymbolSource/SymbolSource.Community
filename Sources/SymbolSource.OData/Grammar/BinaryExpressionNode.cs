using System;
using Irony.Interpreter.Ast;

namespace SymbolSource.OData
{
    public class BinaryExpressionNode : AstNode, IExpression
    {
        public AstNode Left { get; private set; }
        public AstNode Right { get; private set; }
        public string Operator { get; private set; }

        public override void Init(Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Left = AddChild("Left", treeNode.MappedChildNodes[0]);
            var opToken = treeNode.MappedChildNodes[1].FindToken();
            Operator = opToken.Text;
            Right = AddChild("Right", treeNode.MappedChildNodes[2]);
            AsString = Operator + " (operator)"; 
        }

        public string Translate()
        {
            var left = Left as IExpression;
            var right = Right as IExpression;

            if (left != null && right != null)
            {
                switch (Operator)
                {
                    case "eq":
                        return "(" + left.Translate() + "==" + right.Translate() + ")";
                    case "ne":
                        return "(" + left.Translate() + "!=" + right.Translate() + ")";
                    case "gt":
                        return "(" + left.Translate() + ">" + right.Translate() + ")";
                    case "ge":
                        return "(" + left.Translate() + ">=" + right.Translate() + ")";
                    case "lt":
                        return "(" + left.Translate() + "<" + right.Translate() + ")";
                    case "le":
                        return "(" + left.Translate() + "<=" + right.Translate() + ")";
                    case "and":
                        return "(" + left.Translate() + "&&" + right.Translate() + ")";
                    case "or":
                        return "(" + left.Translate() + "||" + right.Translate() + ")";

                    case "add":
                        return "(" + left.Translate() + "+" + right.Translate() + ")";
                    case "sub":
                        return "(" + left.Translate() + "-" + right.Translate() + ")";
                    case "mul":
                        return "(" + left.Translate() + "*" + right.Translate() + ")";
                    case "div":
                        return "(" + left.Translate() + "/" + right.Translate() + ")";
                    case "mod":
                        return "(" + left.Translate() + "%" + right.Translate() + ")";
                }
            }

            throw new NotImplementedException();
        }
    }
}