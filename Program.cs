using System; 
using System.Linq;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace HoverbirdConsole;

class Program
{
    private static bool _initGame = true;                   //сделал для начала отрисовки
    private static bool _gameRun = false;                   //выполняет, если игра отрисовалась и запущена
    private static string _starBrick = "*";
    private static bool _birdDirection;
    private static float _birdPad = 0;
    private static float _previousBirdPad = 0;
    private static ConsoleKey _lastInputKey;
    private static bool _spacebarPressed = false;
    private static int _gameScore = 0;
    static async Task Main(string[] args)
    {
        if (_initGame == true)
        {
            Console.CursorVisible = false;                  //Убрал мигание строки
            DrawGameField(ConsoleColor.DarkBlue);           //Отрисовал поле
            DrawBird((int)_birdPad, (int)_previousBirdPad); //Собственно, сам mister bird
            _initGame = false;                              //Сделал initGame, чтоб один раз поле отрисовывать
            _gameRun = true;                                //Запустил основную логику, отрисовку птицы, облаков, движение и тд
        }

        Task.Run(() => ChangeBirdDirectionAsync());
        Task.Run(() => InputController());

        while (_gameRun)
        {   
            //DrawScore();
            QuitGame();
        }
    }

    private static void InputController()
    {
         while (_gameRun)
        {
            if (Console.KeyAvailable)
            {
                _lastInputKey = Console.ReadKey(true).Key;

                if(_lastInputKey == ConsoleKey.Spacebar)
                {
                    _spacebarPressed = true;
                    _birdDirection = !_birdDirection;
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
                    Console.Write(_starBrick.PadRight(2));
                }
                Console.Write(_starBrick);
                Console.WriteLine();
            }
            else if (i == 5 && _initGame == true)
            {
                Console.WriteLine(_starBrick + _gameScore.ToString().PadLeft(30) + _starBrick.PadLeft(30));
            }
            else
                Console.WriteLine(_starBrick + _starBrick.PadLeft(60));
        }
        Console.ResetColor();
    }

    private static void DrawScore()
    {
        Thread.Sleep(10);
        _gameScore++;
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.SetCursorPosition(30, 5);
        Console.Write(_gameScore.ToString());
        Console.ResetColor();
    }

    private static void DrawBird(int birdXCoordinate, int previousBirdXCoordinate)
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        int[] yPositions = {31, 32, 33};

        string[] lines = {  //Mister Bird
                "A",        //lines.Length 0
              "/^O^\\",     //lines.Length 1
                "X"         //lines.Length 2
        };

        for (int i = 0; i < lines.Length; i++)
        {
            if(i == 1)
            {
                for (int charInLineWithWings = 0; charInLineWithWings < lines[1].Length; charInLineWithWings++)
                {
                    RedrawLastBirdFrame(previousBirdXCoordinate+28+charInLineWithWings, yPositions[i]);
                }
                ClearCharInConsole(birdXCoordinate+28, yPositions[i]);
                Console.SetCursorPosition(birdXCoordinate+28, yPositions[i]);
            }
            else
            {
                RedrawLastBirdFrame(previousBirdXCoordinate+30, yPositions[i]);
                ClearCharInConsole(birdXCoordinate+30, yPositions[i]);
                Console.SetCursorPosition(birdXCoordinate+30, yPositions[i]);
            }
    
            foreach (char c in lines[i])
            {
                Console.Write(c);
            }
        }

        Console.ResetColor();
    }

    private static void RedrawLastBirdFrame(int birdXCoordinate, int yPosition)
    {
        if(_birdDirection == false)
        {
            ClearCharInConsole(birdXCoordinate, yPosition);
        }    
        if(_birdDirection == true)
        {
            ClearCharInConsole(birdXCoordinate, yPosition);
        }
    }

    private static void ChangeBirdDirectionAsync()
    {
        while(_gameRun)
        {
            while (_spacebarPressed == true && _birdDirection == true)
            {
                _previousBirdPad = _birdPad;
                _birdPad = 3;
                DrawBird((int)_birdPad, (int)_previousBirdPad);
            }

            while (_spacebarPressed == true && _birdDirection == false)
            {
                _previousBirdPad = _birdPad;
                _birdPad = -3;
                DrawBird((int)_birdPad, (int)_previousBirdPad);

            }
        }
        Task.Delay(125);
    }

    private static void QuitGame()
    {
        if (_lastInputKey == ConsoleKey.Q)
        {
            _spacebarPressed = false;
            _gameRun = false;
            DrawGameField(ConsoleColor.Red);
            Console.SetCursorPosition(16, 20);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Quit from the game,good luck!");
            Console.ResetColor();
            Thread.Sleep(3000);
            Console.Clear();
            Console.CursorVisible = true;  
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
}
