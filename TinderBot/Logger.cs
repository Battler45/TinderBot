using System;

namespace TinderBot
{
    static class Logger
    {
        static private void LogToConsole(string log)
        {
            Console.WriteLine(log);
        }
        static public void Log(string log)
        {
            LogToConsole(log);
        }
    }
}
