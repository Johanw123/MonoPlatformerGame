using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Moose
{
	class Program
	{
		private static NetManager s_netManager;
		private static ProcessManager s_processManager;

		public static NetManager NetManager
		{
			get { return s_netManager; }
		}

		public static ProcessManager ProcessManager
		{
			get { return s_processManager; }
		}

		static void Main(string[] args)
		{
			SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

			Print("Moose started");

			s_netManager = new NetManager();
			s_processManager = new ProcessManager();

			ProcessManager.StartProcess();

			while (true)
			{
				NetManager.Listen();
			}
		}

		private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
		{
			switch (ctrlType)
			{
				case CtrlTypes.CTRL_C_EVENT:
				case CtrlTypes.CTRL_BREAK_EVENT:
				case CtrlTypes.CTRL_CLOSE_EVENT:
				case CtrlTypes.CTRL_LOGOFF_EVENT:
				case CtrlTypes.CTRL_SHUTDOWN_EVENT:
					Exit();
					return true;
			}
			return false;
		}

		public static void Exit()
		{
			NetManager.Exit();
			ProcessManager.Exit();

			Print("Shutting down Moose...");

			Environment.Exit(0);
		}

		// Declare the SetConsoleCtrlHandler function
		// as external and receiving a delegate.
		[DllImport("Kernel32")]
		public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

		// A delegate type to be used as the handler routine
		// for SetConsoleCtrlHandler.
		public delegate bool HandlerRoutine(CtrlTypes CtrlType);

		// An enumerated type for the control messages
		// sent to the handler routine.
		public enum CtrlTypes
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT,
			CTRL_CLOSE_EVENT,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT
		}

		public static void Print(string message, ConsoleColor color = ConsoleColor.Cyan, bool log = true)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.White;

			// TODO: Add logging support
		}
	}
}
