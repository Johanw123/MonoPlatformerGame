using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moose
{
	class ProcessManager
	{
		Process m_process;

		public ProcessManager()
		{

		}

		public void StartProcess()
		{
			string path = @"apps\monoplatformergame\DedicatedServerConsole.exe";

			if (!File.Exists(path))
			{
				Program.Print("App executable not found (" + path + ")");
				Program.Exit();
			}

			Program.Print("Starting process...");

			m_process = new Process()
			{
				StartInfo = new ProcessStartInfo(path)
				{
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					RedirectStandardInput = true,
					CreateNoWindow = true,
				}
			};

			m_process.OutputDataReceived += (s, e) => Program.Print(e.Data, ConsoleColor.White);

			m_process.Start();
			m_process.BeginOutputReadLine();
		}

		public void SendInput(string message)
		{
			m_process.StandardInput.WriteLine(message);
			Program.Print(message, ConsoleColor.Green);
		}

		public void Exit()
		{
			if (m_process != null && !m_process.HasExited)
			{
				Program.Print("Killing process...");
				m_process.Kill();
			}
		}
	}
}
