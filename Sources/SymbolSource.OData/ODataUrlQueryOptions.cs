using System;
using System.Web.Mvc;

namespace SymbolSource.OData
{
    public class ODataUrlQueryOptions
    {
        public string OrderBy { get; set; }
        public int? Top { get; set; }
        public int? Skip { get; set; }
        public string Filter { get; set; }
        public string Expand { get; set; }
        public string Format { get; set; }
        public string Select { get; set; }
        public string InlineCount { get; set; }
    }

    public class ODataUrlQueryOptionsModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var options = new ODataUrlQueryOptions();

            options.OrderBy = GetO<string>(bindingContext, "$orderby");
            options.Top = GetA<int>(bindingContext, "$top");
            options.Skip = GetA<int>(bindingContext, "$skip");
            options.Filter = GetO<string>(bindingContext, "$filter");
            options.Expand = GetO<string>(bindingContext, "$expand");
            options.Format = GetO<string>(bindingContext, "$format");
            options.Select = GetO<string>(bindingContext, "$select");
            options.InlineCount = GetO<string>(bindingContext, "$inlineCount");

            return options;
        }

        private T? GetA<T>(ModelBindingContext bindingContext, string key) where T : struct
        {
            if (String.IsNullOrEmpty(key)) return null;
            ValueProviderResult valueResult;
            //Try it with the prefix...            
            valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + key);
            //Didn't work? Try without the prefix if needed...
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix == true)
            {
                valueResult = bindingContext.ValueProvider.GetValue(key);
            }
            if (valueResult == null)
            {
                return null;
            }
            return (T?)valueResult.ConvertTo(typeof(T));
        }

        private T GetO<T>(ModelBindingContext bindingContext, string key)
        {
            if (String.IsNullOrEmpty(key)) return default(T);
            ValueProviderResult valueResult;
            //Try it with the prefix...            
            valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + key);
            //Didn't work? Try without the prefix if needed...
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix == true)
            {
                valueResult = bindingContext.ValueProvider.GetValue(key);
            }
            if (valueResult == null)
            {
                return default(T);
            }
            return (T)valueResult.ConvertTo(typeof(T));
        }
    }
}
