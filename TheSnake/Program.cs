using System;

namespace TheSnake
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            var SN = new SnakeGame();
            SN.Run();

            if (SnakeGame.ReRun == true)
            {
                lurk:
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                SN.Run();
                goto lurk;
            }
        }
    }
}
