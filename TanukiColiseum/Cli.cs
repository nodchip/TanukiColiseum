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
            coliseum.ShowStatus += ShowStatus;
            coliseum.ShowErrorMessage += ShowErrorMessage;
            coliseum.Run(options, logFolderPath);
        }

        private void ShowStatus(Options options, Status status, Engine engine1, Engine engine2)
        {
            Console.WriteLine(Coliseum.CreateStatusMessage(options, status, engine1, engine2));
            Console.Out.Flush();
        }

        private void ShowErrorMessage(string statusMessage)
        {
            Console.WriteLine(statusMessage);
            Console.Out.Flush();
        }
    }
}
