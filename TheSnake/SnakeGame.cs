using System;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.IO;

//using System.Diagnostics;
//using System.Windows.Input;
//using System.Text;

namespace TheSnake
{
    class SnakeGame
    {
        string SolidBlock = "█";

        private const int BoardX = 56;
        private const int BoardY = 22;
        private const int ScoreBoardX = 22;

        private string[] TheBoard = new string[BoardY];

        private Point CurrentPosition;

        private readonly Random Rnd = new Random();
        private Point ApplePosition;

        public static bool ReRun = false;

        public void Run()
        {
            DrawBoard();
            DrawScoreBoard();
            LoadHighScore();
            
            CurrentPosition = GenerateStartPosition(BoardX, BoardY);

            var TheSnake = new Snake(Snake.NewSnake(CurrentPosition.X, CurrentPosition.Y, 500));
            DrawScore(TheSnake);
            DrawSnake(TheSnake);            

            while (TheSnake.SnakeAlive == true)
            {
                while (TheSnake.FacingDirection == Snake.SnakeFacingDirection.NOTHING)
                {
                    NewFacingDirection(TheSnake);                    
                    DrawApple(TheSnake);                    
                }                
                MovingSnake(TheSnake);
                TheSnake.SnakeAlive = false;
            }

            Console.WriteLine("GAME OVER!");
            if (TheSnake.Score > TheScores.Min(score => score.Highscore)) NewHighScore(TheSnake);

            ReRun = true;
        }

        public void NewFacingDirection(Snake snake)
        {
            var ChangeDirection = GlobalFunctions.FindKeyInArray(new string[] { "UpArrow", "RightArrow", "DownArrow", "LeftArrow" });

            switch (ChangeDirection)
            {
                case "UpArrow":
                    snake.FacingDirection = Snake.SnakeFacingDirection.UP;
                    break;
                case "RightArrow":
                    snake.FacingDirection = Snake.SnakeFacingDirection.RIGHT;
                    break;
                case "DownArrow":
                    snake.FacingDirection = Snake.SnakeFacingDirection.DOWN;
                    break;
                case "LeftArrow":
                    snake.FacingDirection = Snake.SnakeFacingDirection.LEFT;
                    break;
            }
        }

        public void MovingSnake(Snake snake)
        {
            if (Console.ForegroundColor != ConsoleColor.DarkGreen) Console.ForegroundColor = ConsoleColor.DarkGreen;
                        
            ConsoleKeyInfo keyInfo;
            var HitTheSnake = false;

            while (TheBoard[CurrentPosition.Y][CurrentPosition.X] != 'X' && HitTheSnake == false)
            {
                if (snake.NextAppleValue > 10)
                {
                    Console.SetCursorPosition(33, 23);
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    snake.NextAppleValue -= 10;
                    Console.Write(snake.NextAppleValue);
                    if (snake.NextAppleValue == 90) Console.Write(' ');
                }

                var LastPosition = new Point(CurrentPosition.X, CurrentPosition.Y);
                switch (snake.FacingDirection)
                {
                    case Snake.SnakeFacingDirection.UP:
                        CurrentPosition.Y--;
                        break;
                    case Snake.SnakeFacingDirection.RIGHT:
                        CurrentPosition.X++;
                        break;
                    case Snake.SnakeFacingDirection.DOWN:
                        CurrentPosition.Y++;
                        break;
                    case Snake.SnakeFacingDirection.LEFT:
                        CurrentPosition.X--;
                        break;
                }

                for (int i = 1; i < snake.SnakeLenght; i++)
                {
                    if (snake.SnakeExists[i] == CurrentPosition)
                    {
                        HitTheSnake = true;
                    }                      
                }

                Thread.Sleep(snake.MoveDelay);
                                
                if (CurrentPosition == ApplePosition)
                {
                    snake.SnakeLenght++;
                    DrawApple(snake);
                    Console.SetCursorPosition(LastPosition.X, LastPosition.Y);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(SolidBlock);
                    snake.Score += snake.NextAppleValue;
                    snake.NextAppleValue = 300;
                    if (snake.SnakeLenght > 10)
                    {
                        snake.NextAppleValue += snake.SnakeLenght * 10;
                        if (snake.NextAppleValue >= 1000) snake.NextAppleValue = 990;
                    }
                    DrawScore(snake);
                }
                
                ReDrawSnake(snake);

                if (Console.KeyAvailable)
                {
                    keyInfo = Console.ReadKey(true);

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (snake.FacingDirection != Snake.SnakeFacingDirection.DOWN)
                            {
                                snake.FacingDirection = Snake.SnakeFacingDirection.UP;
                            }                            
                            break;
                        case ConsoleKey.RightArrow:
                            if (snake.FacingDirection != Snake.SnakeFacingDirection.LEFT)
                            {
                                snake.FacingDirection = Snake.SnakeFacingDirection.RIGHT;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (snake.FacingDirection != Snake.SnakeFacingDirection.UP)
                            {
                                snake.FacingDirection = Snake.SnakeFacingDirection.DOWN;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            if (snake.FacingDirection != Snake.SnakeFacingDirection.RIGHT)
                            {
                                snake.FacingDirection = Snake.SnakeFacingDirection.LEFT;
                            }
                            break;
                    }
                }
            }                     
        }
    

        public void DrawSnake(Snake snake)
        {
            snake.SnakeExists[0] = new Point(CurrentPosition.X, CurrentPosition.Y);
            Console.SetCursorPosition(snake.SnakeExists[0].X, snake.SnakeExists[0].Y);
            if (Console.ForegroundColor != ConsoleColor.DarkGreen) Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(SolidBlock);
        }

        public void ReDrawSnake(Snake snake)
        {
            if (snake.SnakeExists[0].X > 0 && snake.SnakeExists[0].Y > 0)
            {
                Console.SetCursorPosition(snake.SnakeExists[0].X, snake.SnakeExists[0].Y);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(" ");
            }
                        
            snake.SnakeExists = snake.SnakeExists.Skip(1).ToArray();

            List<Point> tonytheturkey = snake.SnakeExists.ToList(); //fullösning, gör om property till list istället för array <<
            tonytheturkey.Add(new Point(0, 0));

            snake.SnakeExists = tonytheturkey.ToArray(); // TonyTheTurkey Bjuder >>

            snake.SnakeExists[snake.SnakeLenght] = new Point(CurrentPosition.X, CurrentPosition.Y);

            Console.SetCursorPosition(snake.SnakeExists[snake.SnakeLenght-1].X, snake.SnakeExists[snake.SnakeLenght - 1].Y);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            if (snake.SnakeExists[snake.SnakeLenght - 1].X > 0 && snake.SnakeExists[snake.SnakeLenght - 1].Y > 0)
            {
                Console.Write(SolidBlock);
            }
        }

        public void DrawApple(Snake snake)
        {
            do
            {
                ApplePosition = new Point(Rnd.Next(1, BoardX-1), Rnd.Next(1, BoardY-1));
            } while (Array.Exists(snake.SnakeExists, element => element == ApplePosition));
            
            Console.SetCursorPosition(ApplePosition.X, ApplePosition.Y);
            if (Console.ForegroundColor != ConsoleColor.DarkRed) Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(SolidBlock);
        }

        public void DrawBoard()
        {

            for (int i = 0; i < BoardY; i++)
            {
                for (int j = 0; j < BoardX; j++)
                {

                    if (i == 0 || j == 0 || j == BoardX - 1 || i == BoardY-1)
                    {
                        if (Console.ForegroundColor != ConsoleColor.DarkCyan) Console.ForegroundColor = ConsoleColor.DarkCyan;
                        {
                            Console.Write(SolidBlock);
                            TheBoard[i] += "X";
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(SolidBlock);
                        TheBoard[i] += "O";
                    }

                    if (j == BoardX - 1) Console.WriteLine();
                }
            }
        }

        private void DrawScoreBoard()
        {
            for (int i = 0; i < BoardY; i++)
            {
                Console.SetCursorPosition(BoardX, i);
                for (int j = 0; j < ScoreBoardX; j++)
                {
                    if (i == 0 || j == ScoreBoardX - 1 || i == BoardY - 1)
                    {

                        if(Console.ForegroundColor != ConsoleColor.DarkCyan) Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write(SolidBlock);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(SolidBlock);
                    }

                    if (j == ScoreBoardX - 1) Console.WriteLine();
                }
            }
            Console.SetCursorPosition(BoardX + (ScoreBoardX / 2) -6, 2);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("HIGHSCORES:");
        }

        private void DrawScore(Snake snake)
        {
            Console.SetCursorPosition(1, 23);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"SCORE: {snake.Score}");
            Console.SetCursorPosition(15, 23);
            Console.Write($"NEXT APPLE VALUE: {snake.NextAppleValue}");
            Console.SetCursorPosition(CurrentPosition.X, CurrentPosition.Y);
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            if (snake.Score >= 8000) snake.MoveDelay = 15;
            else if (snake.Score >= 6500) snake.MoveDelay = 30;
            else if (snake.Score >= 5000) snake.MoveDelay = 45;
            else if (snake.Score >= 3000) snake.MoveDelay = 60;
            else if (snake.Score >= 1000) snake.MoveDelay = 70;
        }

        public Point GenerateStartPosition(int x, int y)
        {
            var startX = 0;
            var startY = 0;

            if (x % 2 > 0) startX = (x - (x % 2)) / 2;
            else startX = x / 2;
            if (y % 2 > 0) startY = (y - (y % 2)) / 2;
            else startY = y / 2;

            return new Point(startX, startY);
        }

        //HIGHSCORE
        public string HighscoresTXT = @"bestsnake.txt";
        public List<HighScores> TheScores = new List<HighScores> { new HighScores{Name="DOLPH",Highscore=10000 },
                                            new HighScores{Name="LUFFA",Highscore=6000 },
                                            new HighScores{Name="ROLLE",Highscore=5050 },
                                            new HighScores{Name="BENNY",Highscore=4440 },
                                            new HighScores{Name="TEDDY",Highscore=3500 },
                                            new HighScores{Name="RONNY",Highscore=3000 },
                                            new HighScores{Name="RONNY",Highscore=1050 },
                                            new HighScores{Name="LUCIF",Highscore=666 },
                                            new HighScores{Name="ALPHA",Highscore=20 },
                                            new HighScores{Name="ALPHA",Highscore=10 }
                                           };

        public void LoadHighScore()
        {
            if (File.Exists(HighscoresTXT)) TheScores = JsonConvert.DeserializeObject<List<HighScores>>(File.ReadAllText(HighscoresTXT));
            else WriteHighScore(TheScores);
            
            var OffsetY = 0;
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            foreach (var highscore in TheScores)
            {
                Console.SetCursorPosition(BoardX + (ScoreBoardX / 2) - 9, 4 + OffsetY);
                if (OffsetY < 9) Console.Write($"{OffsetY + 1}.  {highscore.Name} > {highscore.Highscore}");
                else Console.Write($"{OffsetY + 1}. {highscore.Name} > {highscore.Highscore}");

                OffsetY++;
            }            

        }

        public void WriteHighScore(List<HighScores> highscores)
        {
            File.WriteAllText(HighscoresTXT, JsonConvert.SerializeObject(highscores));
        }

        public void NewHighScore(Snake snake)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(15, 23);
            Console.CursorVisible = true;

            Console.Write("NEW HIGHSCORE! - ENTER NAME:>");

            var NewName = "";
            Console.ForegroundColor = ConsoleColor.White;

            while (NewName.Length != 5)
            {
                Console.SetCursorPosition(45, 23);
                for (int i = 0; i < NewName.Length; i++) Console.Write(' ');
                
                Console.SetCursorPosition(45, 23);
                NewName = Console.ReadLine();

                if (NewName.Length < 5 && NewName.Length > 0 && NewName.All(char.IsLetter))
                {
                    for (int j = NewName.Length; j < 5; j++)
                    {
                        NewName += ' ';
                    }
                }
            }
            TheScores.Remove(TheScores[9]);
            TheScores.Add(new HighScores { Name = NewName.ToUpper(), Highscore = snake.Score });
            TheScores = TheScores.OrderByDescending(sort => sort.Highscore).ToList();
            WriteHighScore(TheScores);

            Console.CursorVisible = false;
        }
    }
}
