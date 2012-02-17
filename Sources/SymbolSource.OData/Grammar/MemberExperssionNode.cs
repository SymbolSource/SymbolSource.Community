using Irony.Interpreter.Ast;

namespace SymbolSource.OData
{
    public class MemberExperssionNode : AstNode, IExpression
    {
        public string Member { get; private set; }

        public override void Init(Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            var literal = treeNode.MappedChildNodes[0];
            Member = literal.Token.ValueString;
            AsString = literal.Token.ValueString + " (identifier)";
        }

        public string Translate()
        {
            return Member;
        }
    }
}