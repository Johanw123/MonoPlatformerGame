using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlatformerGame
{
    public class JapeLog : Log
    {
        protected override void WriteLineInternal(string line)
        {
            Console.WriteLine(line);
        }
        protected override void WriteLineInternal(params object[] a)
        {
            Console.WriteLine(a);
        }
        public override void Draw()
        {
        }
    }
}
