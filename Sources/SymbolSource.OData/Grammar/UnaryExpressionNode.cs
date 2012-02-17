using System;
using Irony.Interpreter.Ast;

namespace SymbolSource.OData
{
    public class UnaryExpressionNode : AstNode, IExpression
    {
        public AstNode Right { get; private set; }
        public string Operator { get; private set; }

        public override void Init(Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            var opToken = treeNode.MappedChildNodes[0].FindToken();
            Operator = opToken.Text;
            Right = AddChild("Right", treeNode.MappedChildNodes[1]);
            AsString = Operator + " (operator)";
        }

        public string Translate()
        {
            var right = Right as IExpression;

            if (right != null)
            {
                switch (Operator)
                {
                    case "not":
                        return "!(" + right.Translate() + ")";
                }
            }

            throw new NotImplementedException();
        }
    }
}