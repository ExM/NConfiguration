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
			bool trace = false;
			BaseCommand command = null;

			var p = new OptionSet()
			{
				{ "h|help", "show this message and exit", v => showHelp = v != null },
				{ "t|trace", "show stack trace of exception", v => trace = v != null }
			};

			try
			{
				if (args.Length == 0)
					throw new ArgumentNullException("required parameters in command line");

				args = p.Parse(args).ToArray();
				if (showHelp)
				{
					ShowHelp(p);
					return 0;
				}

				if (args.Length == 0)
					throw new ArgumentNullException("required command in command line");

				command = BaseCommand.CreateFromName(args[0]);
				if (command == null)
					throw new FormatException("unexpected command: " + args[0]);

				command.Initialize(args.Skip(1));
				command.Validate();
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
				if(trace)
					Console.WriteLine("Exception: {0}", ex);
				else
					Console.WriteLine("Exception: {0}", BuildMessage(ex));
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
			foreach (var pair in BaseCommand.AllCommands)
			{
				Console.WriteLine();
				Console.WriteLine("Usage: RsaToolkit {0} [OPTIONS]", pair.Key);
				Console.WriteLine(pair.Value.Description);
				Console.WriteLine("Options:");
				pair.Value.Options.WriteOptionDescriptions(Console.Out);
			}
		}
	}
}
