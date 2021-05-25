using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace TheSnake
{
    class GlobalFunctions
    {
        //FUNKTIONER
        public static string FindKeyInArray(string[] StringArray)
        {
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
            } while (!Array.Exists(StringArray, element => element == Convert.ToString(keyInfo.Key)));
            return Convert.ToString(keyInfo.Key);
        }

    }
}
