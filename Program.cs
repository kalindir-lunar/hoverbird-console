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
    private static int _birdPad = 0;
    private static ConsoleKey _lastInputKey;
    private static bool _spacebarPressed = false;
    private static int _gameScore = 0;
    static void Main(string[] args)
    {
        if (_initGame == true)
        {
            Console.CursorVisible = false;                  //Убрал мигание строки
            DrawGameField();                                //Отрисовал поле
            DrawBird(_birdPad);                                    //Собственно, сам mister bird
            _initGame = false;                              //Сделал initGame, чтоб один раз поле отрисовывать
            _gameRun = true;                                //Запустил основную логику, отрисовку птицы, облаков, движение и тд
        }

        while (_gameRun == true)
        {     
            DrawBird(_birdPad);   
            ChangeBirdDirection();                         //Тут пока игра запущена, то идет постоянное считывание нажатой кнопки
            DrawScore();
            InputController(); 
        }
    }

    private static void InputController()
    {
        if (Console.KeyAvailable)
        {
            _lastInputKey = Console.ReadKey(true).Key;

            if(_lastInputKey == ConsoleKey.Spacebar)
            {
                _spacebarPressed = true;   
            }

            Thread.Sleep(100);
        }

        //ChangeBirdDirection();                            //Меняет направление движения птицы после очередного нажатого SpaceBar
        QuitGame();                                         //Остановка программы при нажатии Q
    }

    private static void DrawGameField()
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.DarkBlue;

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

    private static void DrawBird(int birdXCoordinate)
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
                ClearCharInConsole(birdXCoordinate+28, yPositions[i]);
                Console.SetCursorPosition(birdXCoordinate+28, yPositions[i]);
            }
            else
            {
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

    async static Task ChangeBirdDirection()
    {
        while (_spacebarPressed == true && _birdDirection == true)
        {
            _birdPad++;
            _birdDirection = !_birdDirection;
            ClearLineInConsole(0,41);
            Console.SetCursorPosition(0, 41);
            Console.WriteLine("Direction: " + _birdDirection);
            _spacebarPressed = false;
        }

        while (_spacebarPressed == true && _birdDirection == false)
        {
            _birdPad--;
            _birdDirection = !_birdDirection;
            ClearLineInConsole(0,41);
            Console.SetCursorPosition(0, 41);
            Console.WriteLine("Direction: " + _birdDirection);
            _spacebarPressed = false;
        }
        await Task.Delay(125); 
    }

    private static void QuitGame()
    {
        if (_lastInputKey == ConsoleKey.Q)
        {
            _gameRun = false;
            //ClearLineInConsole(0,41);
            Console.SetCursorPosition(0, 41);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Quit from the game. Good luck!");
            Console.ResetColor();
            Thread.Sleep(1000);
            Console.Clear();
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
