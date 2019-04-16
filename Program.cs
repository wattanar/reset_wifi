using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Diagnostics;

namespace ping_reset
{
    class Program
    {
        static void Main(string[] args)
        {
            int current_timeout = 0;
            int timeout_rate = 5;
            int prev_state = 1;
            byte[] send_byte = { 16 }; 
            int ping_timeout = 1000;

            while (true)
            {
                try
                {
                    using(var ping = new Ping())
                    { 
                        var reply = ping.Send("www.google.com", ping_timeout, send_byte);

                        if (reply.Status == IPStatus.Success)
                        {
                            current_timeout = 0;
                            prev_state = 1;
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine($"{DateTime.Now}, {reply.Status}({current_timeout})");
                            Console.ResetColor();
                        }
                        else 
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            current_timeout = calTimeout(current_timeout, prev_state);
                            Console.WriteLine($"{DateTime.Now}, {reply.Status}({current_timeout})");
                            Console.ResetColor();
                            prev_state = 0;
                        }
                    }

                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    current_timeout = calTimeout(current_timeout, prev_state);
                    Console.WriteLine($"{DateTime.Now}, Request timed out({current_timeout})");
                    Console.ResetColor();
                    prev_state = 0;
                }

                if (current_timeout == timeout_rate) 
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Process.Start("cmd.exe", @"/c reset_wifi.bat");
                    Console.WriteLine("**RESET WIFI**");
                    Console.ResetColor();
                    current_timeout = 0;
                    prev_state = 1;
                }

                Thread.Sleep(1000);
            }
        }

        static int calTimeout(int _current_timeout, int _prev_state)
        {
            if (_prev_state == 1)
            {
                return 1;
            }
            else 
            {
                return _current_timeout+=1;
            }
        }
    }
}
