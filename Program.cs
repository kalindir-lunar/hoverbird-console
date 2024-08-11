using System; 
using System.Linq;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace HoverbirdConsole;

class Program
{
    private static bool _initGame = false; //сделал для начала отрисовки
    private static bool _gameRun = false; //выполняет, если игра отрисовалась и запущена
    private static string _starBrick = "*";
    private static bool _birdDirection;
    private static ConsoleKey _lastInputKey;
    private static int _gameScore = 0;
    static void Main(string[] args)
    {
        if (_initGame == false)
        {
            DrawGameField();
            _initGame = true; //Сделал initGame, чтоб один раз поле отрисовывать
            _gameRun = true;
        }

        while (_gameRun == true)
        {
            InputController(); //тут пока игра запущена, то управлять игрой, логично?
            BirdFlyingOnGameField();
            DrawScore();
            QuitGame();
        }
    }

    private static void DrawGameField()
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.Blue;
        //Console.SetBackgroundColor(50, 85, 120, 255); //сделал голубое небо

        for (int i = 0; i < 40; i++)
        {
            if (i == 0 || i == 39)
            {
                for (int j = 0; j < 30; j++)
                {
                    Console.Write(_starBrick.PadRight(2));
                }
                Console.Write(_starBrick);
                Console.WriteLine();
            }
            else if (i == 5)
            {
                Console.WriteLine(_starBrick + _gameScore.ToString().PadLeft(30) + _starBrick.PadLeft(30));
            }
            else
                Console.WriteLine(_starBrick + _starBrick.PadLeft(60));
        }
        Console.ResetColor();
        //Отрисовал поле и вернул цвет, чтоб следующие
    }

    private static void DrawScore()
    {
        Thread.Sleep(500);
        _gameScore++;
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.SetCursorPosition(0, 5);
        Console.Write(_starBrick + _gameScore.ToString().PadLeft(30) + _starBrick.PadLeft(30));
        Console.ResetColor();
    }

    private static void InputController()
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            _lastInputKey = key;
            Task.Delay(100);
        }
    }

    private static void BirdFlyingOnGameField()
    {
        if (_lastInputKey == ConsoleKey.Spacebar)
        {
            _birdDirection = !_birdDirection;
            ClearLineInConsole(0,41);
            Console.SetCursorPosition(0, 41);
            Console.WriteLine("Direction: " + _birdDirection);
        }

        //ЭТО ПОТОМ УДАЛЮ, ОНО НУЖНА ДЛЯ ДЕБАГА!!!
        if (_lastInputKey != ConsoleKey.Spacebar &&
            _lastInputKey != ConsoleKey.Q)
        {
            ClearLineInConsole(0,41);
            Console.SetCursorPosition(0, 41);
            Console.WriteLine("Key:" + _lastInputKey.ToString());
        }
        //ТУТ КОНЕЦ ДЕБАГА!!!
    }

    private static void QuitGame()
    {
        if (_lastInputKey == ConsoleKey.Q)
        {
            _gameRun = false;
            ClearLineInConsole(0,41);
            Console.SetCursorPosition(0, 41);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Quit from the game. Good luck!");
            Console.ResetColor();
        }
    }

    private static void ClearLineInConsole(int X, int Y)
    {
        Console.SetCursorPosition(X, Y);
        Console.Write(new string(' ', Console.WindowWidth));
    }
}
