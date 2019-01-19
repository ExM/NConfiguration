using System;
using System.IO;
using NUnit.Framework;

namespace RsaToolkit
{
	public static class TestExtensions
	{
		public static void AreSuccess(this int resultCode)
		{
			Assert.AreEqual(0, resultCode, "run the program was a failure");
		}
		
		public static void AreFail(this int resultCode)
		{
			Assert.AreEqual(1, resultCode, "unexpectedly successful execution of the program");
		}
	}
}
