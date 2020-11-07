using System;
using System.IO;
using System.Threading.Tasks;

namespace TanukiColiseum
{
    public class Cli
    {
        private const string LogFolderName = "log";

        public void Run(Options options)
        {
            string logFolderPath = Path.Combine(LogFolderName, Util.GetDateString());
            options.ToModel().Save(Path.Combine(logFolderPath, "setting.xml"));

            var coliseum = new Coliseum();
            coliseum.OnStatusChanged += ShowResult;
            coliseum.OnError += OnError;
            coliseum.Run(options, logFolderPath);
        }

        private void ShowResult(Status status)
        {
            Console.WriteLine(status.ToHumanReadableString());
            Console.Out.Flush();
        }

        public void OnError(string errorMessage)
        {
            Console.Out.WriteLine(errorMessage);
            Console.Out.Flush();
        }
    }
}
