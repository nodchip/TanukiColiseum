using System;
using System.Threading.Tasks;

namespace TanukiColiseum
{
    public class Cli
    {
        public void Run(Options options)
        {
            var coliseum = new Coliseum();
            coliseum.OnStatusChanged += ShowResult;
            coliseum.OnError += OnError;
            coliseum.Run(options);
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
