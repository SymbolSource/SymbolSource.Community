using Irony.Interpreter.Ast;

namespace SymbolSource.OData
{
    public class PrimaryNode : AstNode, IExpression
    {
        public object Value { get; private set; }

        public override void Init(Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            var literal = treeNode.MappedChildNodes[0];
            Value = literal.Token.Value;
            AsString = string.Format("{0} ({1})", literal.Token.Value, treeNode.FirstChild.Token.Value.GetType());            
        }

        public string Translate()
        {
            if (Value is string)
                return string.Format("\"{0}\"", Value);
            return Value.ToString();
        }
    }
}