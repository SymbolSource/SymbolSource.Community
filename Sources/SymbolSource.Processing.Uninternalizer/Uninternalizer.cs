using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;

namespace SymbolSource.Processing.Uninternalizer
{
    public class Uninternalizer
    {
        public void Uninternalize(string file)
        {
            using (var parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(File.ReadAllText(file))))
            {
                parser.Parse();
                
                if (parser.Errors.Count > 0)
                {
                    Console.WriteLine(parser.Errors.ErrorOutput);
                    return;
                }

                var specials = parser.Lexer.SpecialTracker.RetrieveSpecials();

                parser.CompilationUnit.AcceptVisitor(new UninternalizerVisitor(), null);

                var visitor = new CSharpOutputVisitor();
                using (SpecialNodesInserter.Install(specials, visitor))
                    parser.CompilationUnit.AcceptVisitor(visitor, null);

                File.WriteAllText(file, visitor.Text);
            }
        }
    }

    public class UninternalizerVisitor : AbstractAstVisitor
    {
        private static void MakePublic(AttributedNode node)
        {
            node.Modifier = node.Modifier & ~Modifiers.Visibility | Modifiers.Public;
        }

        private static void MakePublic(MemberNode node)
        {
            if (node.InterfaceImplementations.Count > 1)
                return;

            MakePublic((AttributedNode)node);
        }

        private static bool HasContainerType(INode node, ClassType type)
        {
            return node.Parent is TypeDeclaration && (node.Parent as TypeDeclaration).Type == type;
        }

        private static bool IsPrivateOrNone(Modifiers modifiers)
        {
            if ((modifiers & Modifiers.Private) == Modifiers.Private)
                return true;

            if ((modifiers & Modifiers.Visibility) == Modifiers.None)
                return true;

            return false;
        }

        public override object VisitTypeDeclaration(TypeDeclaration declaration, object data)
        {
            MakePublic(declaration);
            return base.VisitTypeDeclaration(declaration, data);
        }

        public override object VisitConstructorDeclaration(ConstructorDeclaration declaration, object data)
        {
            MakePublic(declaration);            
            return base.VisitConstructorDeclaration(declaration, data);
        }

        public override object VisitMethodDeclaration(MethodDeclaration declaration, object data)
        {
            if (!HasContainerType(declaration, ClassType.Interface) && declaration.InterfaceImplementations.Count == 0) 
                MakePublic(declaration);

            return base.VisitMethodDeclaration(declaration, data);
        }

        public override object VisitPropertyDeclaration(PropertyDeclaration declaration, object data)
        {
            if (!HasContainerType(declaration, ClassType.Interface) && declaration.InterfaceImplementations.Count == 0)
                MakePublic(declaration);

            return base.VisitPropertyDeclaration(declaration, data);
        }

        public override object VisitFieldDeclaration(FieldDeclaration declaration, object data)
        {
            if (HasContainerType(declaration, ClassType.Struct) || !IsPrivateOrNone(declaration.Modifier))
                MakePublic(declaration);

            return base.VisitFieldDeclaration(declaration, data);
        }
    }
}
