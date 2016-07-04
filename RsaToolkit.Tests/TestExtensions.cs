using System;
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

			return Program.Main(parameters.Split(' '));

			/*
			ProcessStartInfo psi = new ProcessStartInfo("RsaToolkit.exe", parameters);
			psi.CreateNoWindow = true;
			var proc = Process.Start(psi);

			if (!proc.WaitForExit(30000))
			{
				proc.Kill();
				Assert.Fail("process is hung");
			}

			return proc.ExitCode;
			 */
		}

		public static void SuccessRun(this string parameters)
		{
			Assert.AreEqual(0, Run(parameters), "run the program was a failure");
		}

		public static void FailRun(this string parameters)
		{
			Assert.AreEqual(1, Run(parameters), "unexpectedly successful execution of the program");
		}
	}
}
