using System;
using System.Linq;
using Irony.Parsing;

namespace SymbolSource.OData
{
    public static class ODataQueryable
    {
        private static Parser parser;

        static ODataQueryable()
        {
            var grammar = new FilterGrammar();
            parser = new Parser(grammar);
        }

        public static IQueryable<T> Process<T>(this IQueryable<T> source, ODataUrlQueryOptions options)
        {
            if (!string.IsNullOrEmpty(options.Filter))
                source = source.Where(options.Filter);
            //if (!string.IsNullOrEmpty(options.OrderBy))
            //    source = System.Linq.Dynamic.DynamicQueryable.Where(source, options.OrderBy);
            if (options.Skip.HasValue)
                source = source.Skip(options.Skip.Value);
            if (options.Top.HasValue)
                source = source.Take(options.Top.Value);
            return source;
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return source;

            IExpression root = null;
            try
            {
                var parseTree = parser.Parse(filter);
                root = parseTree.Root.AstNode as IExpression;
            }
            catch(Exception e)
            {
                throw new Exception(string.Format("Problem with parse '{0}'", filter), e);
            }
            
            if (root != null)
            {
                var expression = root.Translate();
                return System.Linq.Dynamic.DynamicQueryable.Where(source, expression);
            }

            throw new NotImplementedException();
        }
    }
}