using Irony.Parsing;

namespace SymbolSource.OData
{
    [Language("FilterGrammar", "1.0", "OData $filter grammar")]
    public class FilterGrammar : Grammar
    {
        public FilterGrammar()
        {
            //Terminals
            var identifier = TerminalFactory.CreateCSharpIdentifier("Identifier");
            var stringLiteral = new StringLiteral("LiteralExpression", "'", StringOptions.NoEscapes);
            var numberLiteral = new NumberLiteral("NumberLiteral");
            
            //NonTerminals
            var commonExpression = new NonTerminal("CommonExpression");

            var binaryExpression = new NonTerminal("BinaryExpression", typeof(BinaryExpressionNode));
            var binaryExpressionOp = new NonTerminal("CommonExpressionOp");

            var unaryExpression = new NonTerminal("UnaryExpression", typeof (UnaryExpressionNode));
            var unaryExpressionOp = new NonTerminal("UnaryExpressionOp");

            var parenExpression = new NonTerminal("ParenExpression");

            var memberExpression = new NonTerminal("MemberExpression", typeof(MemberExperssionNode));

            var literalExpression = new NonTerminal("LiteralExpression", typeof(PrimaryNode));

            var methodCallExpression = new NonTerminal("MethodCallExpression");

            #region Methods
            // bool
            var substringOfMethodCallExpression = new NonTerminal("SubstringOfMethodCallExpression", typeof (MethodCallNode));
            var endsWithMethodCallExpression = new NonTerminal("EndsWithMethodCallExpression", typeof(MethodCallNode));
            var startsWithMethodCallExpression = new NonTerminal("StartsWithMethodCallExpression", typeof(MethodCallNode));

            // int
            var lengthMethodCallExpression = new NonTerminal("LengthMethodCallExpression", typeof (MethodCallNode));
            var indexOfMethodCallExpression = new NonTerminal("IndexOfMethodCallExpression", typeof (MethodCallNode));

            // string
            var replaceMethodCallExpression = new NonTerminal("ReplaceMethodCallExpression", typeof (MethodCallNode));
            var substring1MethodCallExpression = new NonTerminal("Substring1MethodCalleEpression", typeof (MethodCallNode));
            var substring2MethodCallExpression = new NonTerminal("Substring2MethodCallExpressiong", typeof (MethodCallNode));
            var toLowerMethodCallExpression = new NonTerminal("ToLowerMethodCallExpression", typeof(MethodCallNode));
            var toUpperMethodCallExpression = new NonTerminal("ToUpperMethodCallExpression", typeof (MethodCallNode));
            var trimMethodCallExpression = new NonTerminal("TrimMethodCallExpression", typeof (MethodCallNode));
            var concatMethodCallExpression = new NonTerminal("ConcatMethodCallExpression", typeof (MethodCallNode));

            // date
            var secondMethodCallExpression = new NonTerminal("SecondMethodCallExpression", typeof(MethodCallNode));
            var minuteMethodCallExpression = new NonTerminal("MinuteMethodCallExpression", typeof(MethodCallNode));
            var hourMethodCallExpression = new NonTerminal("HourMethodCallExpression", typeof(MethodCallNode));
            var dayMethodCallExpression = new NonTerminal("DayMethodCallExpression", typeof (MethodCallNode));
            var monthMethodCallExpression = new NonTerminal("MonthMethodCallExpression", typeof(MethodCallNode));
            var yearMethodCallExpression = new NonTerminal("YearMethodCallExpression", typeof(MethodCallNode));

            // math
            var roundMethodCallExpression = new NonTerminal("RoundMethodCallExpression", typeof (MethodCallNode));
            var floorMethodCallExpression = new NonTerminal("FloorMethodCallExpression", typeof(MethodCallNode));
            var ceilingMethodCallExpression = new NonTerminal("CeilingMethodCallExpression", typeof(MethodCallNode));

            // type
            var isOf1MethodCallExpression = new NonTerminal("isOf1MethodCallExpression", typeof(MethodCallNode));
            var isOf2MethodCallExpression = new NonTerminal("isOf2MethodCallExpression", typeof(MethodCallNode));
            #endregion

            Root = commonExpression;
            commonExpression.Rule = parenExpression
                                    | memberExpression
                                    | literalExpression
                                    | methodCallExpression
                                    | binaryExpression
                                    | unaryExpression
                ;

            unaryExpression.Rule = unaryExpressionOp + commonExpression
                ;

            unaryExpressionOp.Rule = "not"
                ;

            binaryExpression.Rule = commonExpression + binaryExpressionOp + commonExpression
                ;

            binaryExpressionOp.Rule = (BnfExpression)"eq" | "ne" | "lt" | "le" | "gt" | "ge" | "add" | "sub" | "mul" | "div" | "mod" | "and" | "or"
                ;

            parenExpression.Rule = "(" + commonExpression + ")"
                ;

            memberExpression.Rule = identifier
                ;

            literalExpression.Rule = stringLiteral
                                     | numberLiteral
                ;

            methodCallExpression.Rule = replaceMethodCallExpression
                                      | substring1MethodCallExpression
                                      | substring2MethodCallExpression
                                      | toLowerMethodCallExpression
                                      | toUpperMethodCallExpression
                                      | trimMethodCallExpression
                                      | concatMethodCallExpression
                                      | lengthMethodCallExpression
                                      | indexOfMethodCallExpression
                                      | secondMethodCallExpression
                                      | minuteMethodCallExpression
                                      | hourMethodCallExpression
                                      | dayMethodCallExpression
                                      | monthMethodCallExpression
                                      | yearMethodCallExpression
                                      | roundMethodCallExpression
                                      | floorMethodCallExpression
                                      | ceilingMethodCallExpression
                                      | isOf1MethodCallExpression
                                      | isOf2MethodCallExpression
                                      | substringOfMethodCallExpression
                                      | startsWithMethodCallExpression
                                      | endsWithMethodCallExpression
                ;

            replaceMethodCallExpression.Rule = (BnfExpression) "replace" + "(" + commonExpression + "," + commonExpression + "," + commonExpression + ")";
            substring1MethodCallExpression.Rule = (BnfExpression)"substring" + "(" + commonExpression + "," + commonExpression + ")";
            substring2MethodCallExpression.Rule = (BnfExpression) "substring" + "(" + commonExpression + "," + commonExpression + "," + commonExpression + ")";
            toLowerMethodCallExpression.Rule = (BnfExpression) "tolower" + "(" + commonExpression + ")";
            toUpperMethodCallExpression.Rule = (BnfExpression) "toupper" + "(" + commonExpression + ")";
            trimMethodCallExpression.Rule = (BnfExpression) "trim" + "(" + commonExpression + ")";
            concatMethodCallExpression.Rule = (BnfExpression) "concat" + "(" + commonExpression + "," + commonExpression + ")";

            lengthMethodCallExpression.Rule = (BnfExpression) "length" + "(" + commonExpression + ")";
            indexOfMethodCallExpression.Rule = (BnfExpression) "indexof" + "(" + commonExpression + "," + commonExpression + ")";

            substringOfMethodCallExpression.Rule = (BnfExpression) "substringof" + "(" + commonExpression + "," + commonExpression + ")";
            startsWithMethodCallExpression.Rule = (BnfExpression)"startswith" + "(" + commonExpression + "," + commonExpression + ")";
            endsWithMethodCallExpression.Rule = (BnfExpression) "endswith" + "(" + commonExpression + "," + commonExpression + ")";

            secondMethodCallExpression.Rule = (BnfExpression) "second" + "(" + commonExpression + ")";
            minuteMethodCallExpression.Rule = (BnfExpression) "minute" + "(" + commonExpression + ")";
            hourMethodCallExpression.Rule = (BnfExpression) "hour" + "(" + commonExpression + ")";
            dayMethodCallExpression.Rule = (BnfExpression) "day" + "(" + commonExpression + ")";
            monthMethodCallExpression.Rule = (BnfExpression) "month" + "(" + commonExpression + ")";
            yearMethodCallExpression.Rule = (BnfExpression) "year" + "(" + commonExpression + ")";

            roundMethodCallExpression.Rule = (BnfExpression)"round" + "(" + commonExpression + ")";
            ceilingMethodCallExpression.Rule = (BnfExpression) "ceiling" + "(" + commonExpression + ")";
            floorMethodCallExpression.Rule = (BnfExpression) "floor" + "(" + commonExpression + ")";

            isOf1MethodCallExpression.Rule = (BnfExpression) "isof" + "(" + commonExpression + ")";
            isOf2MethodCallExpression.Rule = (BnfExpression) "isof" + "(" + commonExpression + "," + commonExpression + ")";

            RegisterOperators(10, "or");
            RegisterOperators(20, "and");
            RegisterOperators(30, "eq", "ne", "lt", "le", "gt", "ge", "add", "sub", "mul", "div", "mod" );

            RegisterBracePair("(", ")");

            MarkPunctuation(",", "(", ")");

            MarkTransient(commonExpression);
            MarkTransient(binaryExpressionOp);
            MarkTransient(unaryExpressionOp);
            
            MarkTransient(parenExpression);
            MarkTransient(methodCallExpression);

            LanguageFlags = LanguageFlags.CreateAst;
        }
    }
}