using System;

namespace TanukiColiseum
{
    class Program
    {
        public void Run(string[] args)
        {
            var options = new Options();
            options.Parse(args);
            options.Validate();
            var cli = new Cli();
            cli.Run(options);
        }

        [STAThread]
        static void Main(string[] args)
        {
            new Program().Run(args);
        }
    }
}
