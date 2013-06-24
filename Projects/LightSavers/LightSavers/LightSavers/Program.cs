using System;

namespace LightSavers
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (LightSaverGame game = new LightSaverGame())
            {
                game.Run();
            }
        }
    }
#endif
}

