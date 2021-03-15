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
			if (options.Gui)
			{
				var thread = new Thread(() =>
				{
					var application = new Application();
					application.Run(new MainView());
				});
				thread.SetApartmentState(System.Threading.ApartmentState.STA);
				thread.IsBackground = true; //メインスレッドが終了した場合に、動作中のスレッドも終了させる場合
				thread.Start();
				thread.Join();
			}
			else
			{
				var cli = new Cli();
				cli.Run(options);
			}
		}
	}
}
