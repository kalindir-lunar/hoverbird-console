using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace HoverbirdConsole;

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
        Thread.Sleep(10);
        gameScore++;
        Console.SetCursorPosition(30, 5);
        Console.Write(gameScore.ToString());
    }

    private static void DrawBird(int birdXCoordinate, int previousBirdXCoordinate)
    {
        int[] yPositions = { 31, 32, 33 };

        string[] lines = {
            "A",
            "/^O^\\",
            "X"
        };

        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 1)
            {
                for (int charInLineWithWings = 0; charInLineWithWings < lines[1].Length; charInLineWithWings++)
                {
                    RedrawLastBirdFrame(previousBirdXCoordinate + 28 + charInLineWithWings, yPositions[i]);
                }
                ClearCharInConsole(birdXCoordinate + 28, yPositions[i]);
                Console.SetCursorPosition(birdXCoordinate + 28, yPositions[i]);
            }
            else
            {
                RedrawLastBirdFrame(previousBirdXCoordinate + 30, yPositions[i]);
                ClearCharInConsole(birdXCoordinate + 30, yPositions[i]);
                Console.SetCursorPosition(birdXCoordinate + 30, yPositions[i]);
            }

            foreach (char c in lines[i])
            {
                Console.Write(c);
            }
        }

        previousBirdPad = (int)birdPad;
    }

    private static void RedrawLastBirdFrame(int birdXCoordinate, int yPosition)
    {
        ClearCharInConsole(birdXCoordinate, yPosition);
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
            DrawGameField(ConsoleColor.Red);
            Console.SetCursorPosition(16, 20);
            Console.BackgroundColor = ConsoleColor.Red;
            //Console.WriteLine("Quit from the game, good luck!");
            Console.WriteLine(EndMessage);
            Console.ResetColor();
            Thread.Sleep(3000);
            Console.Clear();
            Console.CursorVisible = true;
        }
    }

    private static void CollideWithBrick()
    {
        if (birdPad <= -28f)
        {
            birdPad = 26f;
            ClearLineInConsole(0, 31);
            ClearLineInConsole(0, 32);
            ClearLineInConsole(0, 33);
            DrawBird((int)birdPad, (int)previousBirdPad);
        }
        else if (birdPad >= 28f)
        {
            birdPad = -26f;
            ClearLineInConsole(0, 31);
            ClearLineInConsole(0, 32);
            ClearLineInConsole(0, 33);
            DrawBird((int)birdPad, (int)previousBirdPad);
        }
    }

    private static void ClearLineInConsole(int X, int Y)
    {
        Console.SetCursorPosition(X, Y);
        Console.Write(new string(' ', Console.WindowWidth));
    }

    private static void ClearCharInConsole(int X, int Y)
    {
        Console.SetCursorPosition(X, Y);
        Console.Write(' ');
    }

    private static async Task DrawAndMoveCloud()
    {
        Random random = new Random();
        const int cloudWidth = 5;
        const int cloudHeight = 2;
        const int spawnInterval = 10; // Интервал в линиях между спавнами облаков

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

                ClearCloud(cloud.x, cloud.y);

                if (CheckCollisionWithBird(cloud.x, newY))
                {
                    EndMessage = "   You loose! Awoid clouds.";
                    lastInputKey = ConsoleKey.Q;
                }

                if (newY + cloudHeight >= 40)
                {
                    clouds.RemoveAt(i);
                    i--; // Корректируем индекс, так как облако было удалено
                }
                else
                {
                    DrawCloud(cloud.x, newY);
                    clouds[i] = (cloud.x, newY);
                }
            }

            await Task.Delay(delayBetweenFrames * 5); // Замедление облаков
        }
    }

    private static void DrawCloud(int x, int y)
    {
        string[] lines =
        {
        "=====",
        "=====",
    };

        for (int i = 0; i < lines.Length; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.Write(lines[i]);
        }
    }


    private static void ClearCloud(int x, int y)
    {
        for (int i = 0; i < 2; i++) // 2 строки в облаке
        {
            Console.SetCursorPosition(x, y + i);
            Console.Write(new string(' ', 5)); // Очистка ширины облака
        }
    }


    private static bool CheckCollisionWithBird(int cloudX, int cloudY)
    {
        int birdX = (int)birdPad + 30; // Позиция X птицы (в середине экрана)
        int birdY = 31; // Позиция Y птицы (линии 31, 32, 33)

        return cloudX < birdX + 5 && cloudX + 5 > birdX && cloudY < birdY + 3 && cloudY + 2 > birdY;
    }
}
