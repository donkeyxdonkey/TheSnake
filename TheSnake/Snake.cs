using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace TheSnake
{

    public class Snake
    {
        public enum SnakeFacingDirection
        {
            NOTHING, UP, RIGHT, DOWN, LEFT
        }

        public int SnakeLenght { get; set; }
        public Point[] SnakeExists { get; set; }
        public SnakeFacingDirection FacingDirection { get; set; }
        public bool SnakeAlive { get; set; }
        public int MoveDelay { get; set; }
        public int NextAppleValue { get; set; }
        public int Score { get; set; }

        public Snake(Point[] SnakeStartPosition) //Konstruktor
        {
            SnakeLenght = 1;
            SnakeExists = SnakeStartPosition;
            FacingDirection = SnakeFacingDirection.NOTHING;
            SnakeAlive = true;
            MoveDelay = 100;
            NextAppleValue = 10;

        }

        public static Point[] NewSnake(int StartX, int StartY, int MaxSize)
        {
            var x = new Point[MaxSize];
            x[0] = new Point(StartX, StartY);
            return x;
        }
    }
}
