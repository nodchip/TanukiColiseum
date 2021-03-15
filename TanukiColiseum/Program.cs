using CommandLine;
using System.Threading;
using System.Windows;

namespace TanukiColiseum
{
	class Program
	{
		static void Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<Options>(args);

			if (result.Tag != ParserResultType.Parsed)
			{
				return;
			}
			var parsed = (Parsed<Options>)result;
			var options = parsed.Value;
			var cli = new Cli();
			cli.Run(options);
		}
	}
}
