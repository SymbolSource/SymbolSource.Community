using System;
using System.Linq;
using Irony.Parsing;
using Xunit;

namespace SymbolSource.OData.Tests
{
    public class ODataTest
    {
        private Parser parser;

        public ODataTest()
        {
            var grammar = new FilterGrammar();
            parser = new Parser(grammar);
        }

        [Fact]
        public void Nuget_install_SymbolSource_DemoLibrary()
        {
            var tree = parser.Parse("tolower(Id) eq 'symbolsource.demolibrary'");
            Assert.Equal(ParseTreeStatus.Parsed, tree.Status);
            var node = tree.Root.AstNode;            
        }

        [Fact]
        public void Visual_Studio()
        {
            var tree = parser.Parse("IsLatestVersion");
            Assert.Equal(ParseTreeStatus.Parsed, tree.Status);
            var node = tree.Root.AstNode;
        }


        [Fact]
        public void Visual_Studio_PackageManager_Autocomplete()
        {
            var tree = parser.Parse("startswith(tolower(Id),'symb') and IsLatestVersion");
            Assert.Equal(ParseTreeStatus.Parsed, tree.Status);
            var node = tree.Root.AstNode;
        }

        [Fact]
        public void Nuget_Package_Explorer_Search()
        {
            var tree = parser.Parse("((((Id ne null) and substringof('symbol',tolower(Id))) or ((Description ne null) and substringof('symbol',tolower(Description)))) or ((Tags ne null) and substringof(' symbol ',tolower(Tags)))) and IsLatestVersion");
            Assert.Equal(ParseTreeStatus.Parsed, tree.Status);
            var node = tree.Root.AstNode as IExpression;
            Assert.NotNull(node);
            node.Translate();
        }        

        [Fact]
        public void Query0()
        {
            var nodes = new[] { new { A = 123, B = 456 }, new { A = 456, B = 123 } };
            var result = nodes.AsQueryable().Where(null).ToArray();
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void Query1()
        {
            var nodes = new[] {new {A = 123, B = 456}, new {A = 456, B = 123}};
            var result = nodes.AsQueryable().Where("A eq 123").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void Query2()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("IsLatestVersion").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryNotOperator()
        {
            var nodes = new[] { new { A = 123, B = 456 }, new { A = 456, B = 123 } };
            var result = nodes.AsQueryable().Where("not (A gt B)").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryOperators1()
        {
            var nodes = new[] { new { A = 123, B = 456 }, new { A = 456, B = 123 } };
            var result = nodes.AsQueryable().Where("A lt B and not (A gt B) and A eq 123").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryOperators2()
        {
            var nodes = new[] { new { A = 123, B = 456, C = 12 }, new { A = 456, B = 123, C = 246 } };
            var result = nodes.AsQueryable().Where("(C mod B eq 0) and (A sub C eq 210)").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryOperators3()
        {
            var nodes = new[] { new { A = 123, B = 456 }, new { A = 456, B = 123 } };
            var result = nodes.AsQueryable().Where("A eq 456 and (B eq 234 or B eq 123)").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryOperators4()
        {
            var nodes = new[] { new { P = true, Q = false }, new { P = false, Q = true } };
            var result = nodes.AsQueryable().Where("(not P and Q) eq (not P or not Q)").ToArray();
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void QueryOperators5()
        {
            var nodes = new[] { new { P = true, Q = false }, new { P = false, Q = true } };
            var result = nodes.AsQueryable().Where("(not P or Q) eq (not P and not Q)").ToArray();
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void QuerySubstringOf()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("substringof(Id, 'Symbol') eq true").ToArray();
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void QueryEndsWith()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("endswith(Id, 'DemoLibrary') eq true").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryStartsWith()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("startswith(Id, 'SymbolSource') eq true").ToArray();
            Assert.Equal(2, result.Length);
        }


        [Fact]
        public void QueryLength()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("length(Id) eq 12").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryIndexOf()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("indexof(Id, '.') eq 12").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryReplace()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("replace(Id, 'DemoLibrary', 'OData') eq 'SymbolSource.OData'").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QuerySubstring1()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("substring(Id, 6) eq 'Source'").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QuerySubstring2()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("substring(Id, 0, 6) eq 'Symbol'").ToArray();
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void QueryToLower()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("tolower(Id) eq 'symbolsource.demolibrary'").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryToUpper()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("toupper(Id) eq 'SYMBOLSOURCE.DEMOLIBRARY'").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryTrim()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("trim(Id) eq 'SymbolSource.DemoLibrary'").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryConcat()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false } };
            var result = nodes.AsQueryable().Where("concat(Id, '.Test') eq 'SymbolSource.Test'").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QuerySecond()
        {
            var nodes = new[] {
                new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Date = new DateTime(2011, 11, 14, 14, 43, 50) },
                new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Date = new DateTime(2011, 12, 11, 15, 42, 51) } };

            var result = nodes.AsQueryable().Where("second(Date) eq 51").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryMinute()
        {
            var nodes = new[] {
                new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Date = new DateTime(2011, 11, 14, 14, 43, 50) },
                new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Date = new DateTime(2011, 12, 11, 15, 42, 51) } };

            var result = nodes.AsQueryable().Where("minute(Date) eq 43").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryHour()
        {
            var nodes = new[] {
                new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Date = new DateTime(2011, 11, 14, 14, 43, 50) },
                new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Date = new DateTime(2011, 12, 11, 15, 42, 51) } };

            var result = nodes.AsQueryable().Where("hour(Date) eq 15").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryDay()
        {
            var nodes = new[] {
                new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Date = new DateTime(2011, 11, 14, 14, 43, 50) },
                new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Date = new DateTime(2011, 12, 11, 15, 42, 51) } };

            var result = nodes.AsQueryable().Where("day(Date) eq 14").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryMonth()
        {
            var nodes = new[] {
                new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Date = new DateTime(2011, 11, 14, 14, 43, 50) },
                new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Date = new DateTime(2011, 12, 11, 15, 42, 51) } };

            var result = nodes.AsQueryable().Where("month(Date) eq 12").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryYear()
        {
            var nodes = new[] {
                new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Date = new DateTime(2011, 11, 14, 14, 43, 50) },
                new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Date = new DateTime(2010, 12, 11, 15, 42, 51) } };

            var result = nodes.AsQueryable().Where("year(Date) eq 2010").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryRoundDouble()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Value = 10.5 }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Value = 2.2 } };
            var result = nodes.AsQueryable().Where("round(Value) eq 2").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryRoundDecimal()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Value = 10.5m }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Value = 2.2m } };
            var result = nodes.AsQueryable().Where("round(Value) eq 2").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryFloorDouble()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Value = 10.5 }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Value = 2.2 } };
            var result = nodes.AsQueryable().Where("floor(Value) eq 2").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryFloorDecimal()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Value = 10.5m }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Value = 2.2m } };
            var result = nodes.AsQueryable().Where("round(Value) eq 10").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryCeilingDouble()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Value = 10.5 }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Value = 2.2 } };
            var result = nodes.AsQueryable().Where("ceiling(Value) eq 3").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryCeilingDecimal()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Value = 10.5m }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Value = 2.2m } };
            var result = nodes.AsQueryable().Where("ceiling(Value) eq 3").ToArray();
            Assert.Equal(1, result.Length);
        }

        [Fact]
        public void QueryIsOf1()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Type = typeof(FactAttribute) }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Type = typeof(FactAttribute) } };
            Assert.Throws(typeof(NotSupportedException), () =>
                                 {
                                     var result =  nodes.AsQueryable().Where("isof(Type)").ToArray();
                                 });
        }

        [Fact]
        public void QueryIsOf2()
        {
            var nodes = new[] { new { Id = "SymbolSource", Version = "12.1.1.1", IsLatestVersion = true, Type = typeof(FactAttribute) }, new { Id = "SymbolSource.DemoLibrary", Version = "12.2.1.1", IsLatestVersion = false, Type = typeof(FactAttribute) } };
            Assert.Throws(typeof(NotSupportedException), () =>
            {
                var result = nodes.AsQueryable().Where("isof(Type, 'Xunit.FactAttribute')").ToArray();
            });
        }
    }
}
