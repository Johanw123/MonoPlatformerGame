using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    public abstract class Log
    {
        public static void WriteLine(string line)
        {
            mInstance.WriteLineInternal(line);
        }
        public static void WriteLine(params object[] a)
        {
            mInstance.WriteLineInternal(a);
        }

        private static Log mInstance;
        public static void Init(Log log)
        {
            mInstance = log;
        }

        protected abstract void WriteLineInternal(string line);
        protected abstract void WriteLineInternal(params object[] a);
        public abstract void Draw();
    }
}
