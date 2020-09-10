using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace TanukiColiseum
{
    class Program
    {
        public void Run(string[] args)
        {
            var options = new Options();
            options.Parse(args);

            if (options.Interface == Options.UserInterface.Cli)
            {
                options.Validate();
                var cli = new Cli();
                cli.Run(options);
            }
            else
            {
                Application.Run(new Gui());
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            new Program().Run(args);
        }
    }
}
