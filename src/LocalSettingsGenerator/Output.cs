using System;
using System.Linq;

namespace LocalSettingsGenerator
{
    public class Output
    {
        private int indent = 0;

        public void WriteLine(string line)
        {
            var indentStr = string.Join("", Enumerable.Repeat("  ", indent));
            Console.WriteLine($"{indentStr}{line}");
        }

        public IDisposable Indent()
        {
            indent ++;

            return new CallbackOnDispose(Deindent);
        }

        public void Deindent()
        {
            indent --;
        }
    }

    public class CallbackOnDispose : IDisposable
    {
        private readonly Action callback;

        public CallbackOnDispose(Action callback)
        {
            this.callback = callback;
        }

        public void Dispose()
        {
            callback();
        }
    }
}