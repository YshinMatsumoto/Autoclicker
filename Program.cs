using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


namespace AutoClicker;

class Program
{
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

    // Объявляем константы клавиш
    public const int VK_F9 = 0x78;
    public const int VK_F6 = 0x75;
    public const int VK_F7 = 0x76;
    public const int VK_F8 = 0x77;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    static int clickMs = 100;
    static bool isAvailable = false;

    static void HandleF6()
    {
        isAvailable = true;
        Console.WriteLine("Кликер включён");
    }

    static void HandleF7()
    {
        isAvailable = false;
        Console.WriteLine("Кликер выключен");
    }

    static async Task HandleF8()
    {
        // Выключаем кликер, если был включён
        if (isAvailable)
        {
            isAvailable = false;
            Console.WriteLine("Кликер автоматически выключен перед настройкой");
        }

        Console.WriteLine("Настройка частоты");
        Console.Write("Введите количество кликов в секунду (1-50): ");
    
        if (int.TryParse(Console.ReadLine(), out int clicksPerSecond) && clicksPerSecond >= 1 && clicksPerSecond <= 50)
        {
            clickMs = 1000 / clicksPerSecond;
            Console.WriteLine($"Частота установлена: {clicksPerSecond} кликов/сек (задержка {clickMs} мс)");
        }
        else
        {
            Console.WriteLine("Неверно заданный параметр. Частота не изменена.");
        }
    
        Console.WriteLine("Нажмите Enter чтобы продолжить...");
        Console.ReadLine();
        Console.Clear();
        Console.WriteLine("F6 - включить автокликер");
        Console.WriteLine("F7 - выключить автокликер");
        Console.WriteLine("F8 - настроить частоту кликов");
        Console.WriteLine("F9 - закрыть программу");
    }

    static async Task Main(string[] args)
    {
        Console.WriteLine("!Добро пожаловать в автокликер!");
        Console.WriteLine("F6 - включить автокликер");
        Console.WriteLine("F7 - выключить автокликер");
        Console.WriteLine("F8 - настроить частоту кликов");
        Console.WriteLine("F9 - закрыть программу");
       
        while ((GetAsyncKeyState(VK_F9) & 0x8000) == 0)
        {
            if ((GetAsyncKeyState(VK_F6) & 0x8000) != 0)
            {
                HandleF6();
                await Task.Delay(200); // защита от многократного срабатывания
            }
            else if ((GetAsyncKeyState(VK_F7) & 0x8000) != 0)
            {
                HandleF7();
                await Task.Delay(200);
            }
            else if ((GetAsyncKeyState(VK_F8) & 0x8000) != 0)
            {
                await HandleF8();
                await Task.Delay(200);
            }

            // Логика кликов: если кликер включён — эмулируем нажатие
            if (isAvailable)
            {
                var start = DateTime.Now;


                // Эмуляция клика мыши
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                var elapsed = (DateTime.Now - start).TotalMilliseconds;
                var remaining = clickMs - elapsed;
                if (remaining > 0) await Task.Delay((int)remaining);
                await Task.Delay(clickMs);
            }

            // Маленькая пауза только если кликер выключен
            if (!isAvailable)
            {
                await Task.Delay(50);
            }
        }

        Console.WriteLine("Нажмите Enter для выхода...");
        Console.ReadLine();
    }
}