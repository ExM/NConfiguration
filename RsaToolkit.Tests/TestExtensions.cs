using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;

namespace RsaToolkit
{
	public static class TestExtensions
	{
		public static void DeleteIfExist(this string fileName)
		{
			if(File.Exists(fileName))
				File.Delete(fileName);
		}

		public static int Run(this string parameters)
		{
			Console.WriteLine("RsaToolkit.exe " + parameters);

			ProcessStartInfo psi = new ProcessStartInfo("RsaToolkit.exe", parameters);
			psi.CreateNoWindow = true;
			var proc = Process.Start(psi);

			if (!proc.WaitForExit(30000))
			{
				proc.Kill();
				Assert.Fail("process is hung");
			}

			return proc.ExitCode;
		}
	}
}
