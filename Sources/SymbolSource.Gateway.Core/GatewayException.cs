using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SymbolSource.Gateway.Core
{
    public class GatewayException : Exception
    {
        public GatewayException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public override string Message
        {
            get
            {
                if (InnerException == null)
                    return base.Message;

                return string.Format("{0}:  {1}", base.Message, InnerException.Message);
            }
        }

        public string ResponseStatusDescription
        {
            get
            {
                const string suffix =
                    "."
                    + " See http://www.symbolsource.org/Public/Home/Help for possible reasons."
                    + " Fiddler may help diagnosing this error if your client discards attached detailed information.";

                const string shortener = " (...)";

                var remaining = 512 - suffix.Length;

                var message = Message;
                message = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(message));
                message = new Regex(@"[\s]+").Replace(message, " ");

                if (message.Length > remaining)
                    message = message.Substring(1, remaining - shortener.Length) + shortener;

                return message + suffix;
            }
        }

        public string ResponseContent
        {
            get { return ToString(); }
        }
    }
}