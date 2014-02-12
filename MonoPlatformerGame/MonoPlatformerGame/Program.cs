#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace MonoPlatformerGame
{

    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(System.String[] args)
        {
			Game1 game = new Game1();
			game.Run ();

        }
    }

}
