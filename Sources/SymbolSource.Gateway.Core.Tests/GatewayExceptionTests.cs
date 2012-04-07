using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace SymbolSource.Gateway.Core.Tests
{
    public class GatewayExceptionTests
    {
        private static void AssertStatusDescription(string description)
        {
            Debug.WriteLine(description);
            Assert.InRange(description.Length, 1, 512);
            Assert.DoesNotContain('\r', description);
            Assert.DoesNotContain('\n', description);
            Assert.Equal(Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(description)), description);
        }
        
        [Fact]
        public void TestShortException()
        {
            var outer = new GatewayException("Outer", new Exception("Inner"));
            AssertStatusDescription(outer.ResponseStatusDescription);
        }

        [Fact]
        public void TestLongException()
        {
            var outer = new GatewayException("Outer", new Exception(string.Join(" ", Enumerable.Repeat("Inner", 100))));
            AssertStatusDescription(outer.ResponseStatusDescription);
        }

        [Fact]
        public void TestMultilineException()
        {
            try
            {
                TextMultilineExceptionHelper();
            }
            catch (Exception inner)
            {
                var outer = new GatewayException("Outer", new Exception(inner.StackTrace));
                AssertStatusDescription(outer.ResponseStatusDescription);
            }
        }

        private static void TextMultilineExceptionHelper()
        {
            throw new Exception();
        }

        [Fact]
        public void TestNonAscii()
        {
            var outer = new GatewayException("ą", new Exception("ę"));
            AssertStatusDescription(outer.ResponseStatusDescription);
        }
    }
}
