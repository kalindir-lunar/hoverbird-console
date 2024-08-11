using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace HoverbirdConsole
{
    class Program
    {
        private const int framesPerSecond = 30;
        private const float movementPerFrame = 1f;
        private const int delayBetweenFrames = 1000 / framesPerSecond;
        private static bool initGame = true;
        private static bool gameRun = false;
        private static string starBrick = "*";
        private static bool birdDirection;
        private static float birdPad = 0;
        private static int previousBirdPad = 0;
        private static ConsoleKey lastInputKey;
        private static bool spacebarPressed = false;
        private static int gameScore = 0;
        private static string EndMessage = "Quit from the game, good luck!";
        private static readonly object consoleLock = new object();

        static async Task Main(string[] args)
        {
            if (initGame)
            {
                Console.CursorVisible = false;
                DrawGameField(ConsoleColor.Black);
                DrawBird((int)birdPad, (int)previousBirdPad);
                initGame = false;
                gameRun = true;
            }

            Task.Run(() => ChangeBirdDirectionAsync());
            Task.Run(() => InputController());
            Task.Run(() => DrawAndMoveCloud());

            while (gameRun)
            {
                DrawScore();
                QuitGame();
                await Task.Delay(delayBetweenFrames);
            }
        }

        private static void InputController()
        {
            while (gameRun)
            {
                if (Console.KeyAvailable)
                {
                    lastInputKey = Console.ReadKey(true).Key;

                    if (lastInputKey == ConsoleKey.Spacebar)
                    {
                        spacebarPressed = true;
                        birdDirection = !birdDirection;
                    }
                    Thread.Sleep(100);
                }
            }
        }

        private static void DrawGameField(ConsoleColor consoleColor)
        {
            Console.Clear();
            Console.BackgroundColor = consoleColor;
            for (int i = 0; i < 40; i++)
            {
                if (i == 0 || i == 39)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        Console.Write(starBrick.PadRight(2));
                    }
                    Console.Write(starBrick);
                    Console.WriteLine();
                }
                else if (i == 5 && initGame == true)
                {
                    Console.WriteLine(starBrick + gameScore.ToString().PadLeft(30) + starBrick.PadLeft(30));
                }
                else
                {
                    Console.WriteLine(starBrick + starBrick.PadLeft(60));
                }
            }
        }

        private static void DrawScore()
        {
            lock (consoleLock)
            {
                Console.SetCursorPosition(30, 5);
                Console.Write(new string(' ', 10)); // Очистка области
                Console.SetCursorPosition(30, 5);
                Console.Write(gameScore.ToString());
            }
        }

        private static void DrawBird(int birdXCoordinate, int previousBirdXCoordinate)
        {
            int[] yPositions = { 31, 32, 33 };
            string[] lines = { "  A", "/^O^\\", "  X" };

            lock (consoleLock)
            {
                // Очистка предыдущей позиции
                for (int i = 0; i < lines.Length; i++)
                {
                    Console.SetCursorPosition(previousBirdXCoordinate + 28, yPositions[i]);
                    Console.Write(new string(' ', lines[i].Length));
                }

                // Отрисовка новой позиции
                for (int i = 0; i < lines.Length; i++)
                {
                    Console.SetCursorPosition(birdXCoordinate + 28, yPositions[i]);
                    Console.Write(lines[i]);
                }
            }

            previousBirdPad = (int)birdPad;
        }

        private static async Task ChangeBirdDirectionAsync()
        {
            while (gameRun)
            {
                if (spacebarPressed)
                {
                    CollideWithBrick();

                    if (birdDirection)
                    {
                        birdPad += movementPerFrame;
                    }
                    else
                    {
                        birdPad -= movementPerFrame;
                    }

                    if ((int)birdPad != (int)previousBirdPad)
                    {
                        DrawBird((int)birdPad, (int)previousBirdPad);
                    }
                }

                await Task.Delay(delayBetweenFrames);
            }
        }

        private static void QuitGame()
        {
            if (lastInputKey == ConsoleKey.Q)
            {
                spacebarPressed = false;
                gameRun = false;
                lock (consoleLock)
                {
                    DrawGameField(ConsoleColor.Red);
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(16, 20);
                    Console.WriteLine(EndMessage);
                    Console.SetCursorPosition(16, 22);
                    EndMessage = " Press any key for Restart";
                    Console.WriteLine(EndMessage);
                    Console.SetCursorPosition(16, 24);
                    EndMessage = " Press   'Q'   for    Quit";
                    Console.WriteLine(EndMessage);
                    Console.SetCursorPosition(16, 15);
                    EndMessage = "       Your score: " + gameScore;
                    Console.WriteLine(EndMessage);
                    Console.ResetColor();
                    if (Console.ReadKey(true).Key == ConsoleKey.Q)
                    {
                        Console.Clear();
                        Console.CursorVisible = true;
                    }
                    else
                    {
                        lastInputKey = ConsoleKey.D6;
                        gameScore = 0;
                        initGame = true;
                        if (initGame)
                        {
                            Console.CursorVisible = false;
                            DrawGameField(ConsoleColor.Black);
                            DrawBird((int)birdPad, (int)previousBirdPad);
                            initGame = false;
                            gameRun = true;
                        }

                        Task.Run(() => ChangeBirdDirectionAsync());
                        Task.Run(() => InputController());
                        Task.Run(() => DrawAndMoveCloud());

                        gameRun = true;
                    }

                    Thread.Sleep(100);
                }
            }
        }

        private static void CollideWithBrick()
        {
            if (birdPad <= -28f)
            {
                birdPad = 26f;
                lock (consoleLock)
                {
                    ClearLineInConsole(0, 31);
                    ClearLineInConsole(0, 32);
                    ClearLineInConsole(0, 33);
                    DrawBird((int)birdPad, (int)previousBirdPad);
                    Console.SetCursorPosition(0, 31);
                    Console.WriteLine(starBrick + starBrick.PadLeft(60));
                    Console.SetCursorPosition(0, 32);
                    Console.WriteLine(starBrick + starBrick.PadLeft(60));
                    Console.SetCursorPosition(0, 33);
                    Console.WriteLine(starBrick + starBrick.PadLeft(60));
                }
            }
            else if (birdPad >= 28f)
            {
                birdPad = -26f;
                lock (consoleLock)
                {
                    ClearLineInConsole(0, 31);
                    ClearLineInConsole(0, 32);
                    ClearLineInConsole(0, 33);
                    DrawBird((int)birdPad, (int)previousBirdPad);
                    Console.SetCursorPosition(0, 31);
                    Console.WriteLine(starBrick + starBrick.PadLeft(60));
                    Console.SetCursorPosition(0, 32);
                    Console.WriteLine(starBrick + starBrick.PadLeft(60));
                    Console.SetCursorPosition(0, 33);
                    Console.WriteLine(starBrick + starBrick.PadLeft(60));
                }
            }
        }

        private static void ClearLineInConsole(int X, int Y)
        {
            lock (consoleLock)
            {
                Console.SetCursorPosition(X, Y);
                Console.Write(new string(' ', Console.WindowWidth));
            }
        }

        private static async Task DrawAndMoveCloud()
        {
            Random random = new Random();
            const int cloudWidth = 5;
            const int cloudHeight = 2;
            const int spawnInterval = 9; // Интервал в линиях между спавнами облаков

            List<(int x, int y)> clouds = new List<(int x, int y)>();

            while (gameRun)
            {
                if (clouds.Count == 0 || clouds.Last().y >= spawnInterval)
                {
                    int xPosition = random.Next(3, 58 - cloudWidth);
                    clouds.Add((xPosition, 3)); // Спавн облака на 3-й линии
                }

                for (int i = 0; i < clouds.Count; i++)
                {
                    var cloud = clouds[i];
                    int newY = cloud.y + 1;

                    lock (consoleLock)
                    {
                        ClearCloud(cloud.x, cloud.y);

                        if (CheckCollisionWithBird(cloud.x, newY))
                        {
                            EndMessage = "  You lose!  Avoid clouds";
                            lastInputKey = ConsoleKey.Q;
                        }

                        if (newY + cloudHeight >= 40)
                        {
                            clouds.RemoveAt(i);
                            i--; // Корректируем индекс, так как облако было удалено
                            gameScore++;
                        }
                        else
                        {
                            DrawCloud(cloud.x, newY);
                            clouds[i] = (cloud.x, newY);
                        }
                    }
                }

                await Task.Delay(delayBetweenFrames * 2); // Замедление облаков
            }
        }

        private static void DrawCloud(int x, int y)
        {
            string[] lines =
            {
                "=====",
                "=====",
            };

            lock (consoleLock)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    Console.SetCursorPosition(x, y + i);
                    Console.Write(lines[i]);
                }
            }
        }

        private static void ClearCloud(int x, int y)
        {
            lock (consoleLock)
            {
                for (int i = 0; i < 2; i++) // 2 строки в облаке
                {
                    Console.SetCursorPosition(x, y + i);
                    Console.Write(new string(' ', 5)); // Очистка ширины облака
                }
            }
        }

        private static bool CheckCollisionWithBird(int cloudX, int cloudY)
        {
            int birdX = (int)birdPad + 30; // Позиция X птицы (в середине экрана)
            int birdY = 31; // Позиция Y птицы (линии 31, 32, 33)

            return cloudX < birdX + 5 && cloudX + 5 > birdX && cloudY < birdY + 3 && cloudY + 2 > birdY;
        }
    }
}
