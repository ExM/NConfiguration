using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace RsaToolkit
{
	public class Program
	{
		public static int Main(string[] args)
		{
			bool showHelp = false;
			BaseCommand command = null;

			var p = new OptionSet()
			{
				{ "h|help", "show this message and exit", v => showHelp = v != null }
			};

			try
			{
				if (args.Length == 0)
					throw new ArgumentNullException("required parameters in command line");

				command = BaseCommand.CreateFromName(args[0]);
				if (command == null)
				{
					if (p.Parse(args).Count != 0)
						throw new FormatException("unexpected command: " + command);
					ShowHelp(p);
					return 0;
				}
				else
				{
					command.Initialize(args.Skip(1));
					command.Validate();
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("RsaToolkit: {0}", BuildMessage(e));
				Console.WriteLine("Try `RsaToolkit --help' for more information.");
				return 1;
			}

			try
			{
				command.Run();
				return 0;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: {0}", ex);
				return 1;
			}
		}

		static string BuildMessage(Exception ex)
		{
			var sb = new StringBuilder(ex.Message);
			ex = ex.InnerException;

			while (ex != null)
			{
				sb.AppendFormat(" Reason: {0}", ex.Message);
				ex = ex.InnerException;
			}
			return sb.ToString();
		}

		static void ShowHelp(OptionSet p)
		{
			Console.WriteLine("Usage: RsaToolkit [OPTIONS]");
			Console.WriteLine("Options:");
			p.WriteOptionDescriptions(Console.Out);
			Console.WriteLine();
			Console.WriteLine("Usage: RsaToolkit [COMMAND] [OPTIONS]");
		}
	}
}
