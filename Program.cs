using System;

namespace GBJAM9
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new NezGame())
                game.Run();
        }
    }
}
