using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;

namespace Лабораторная_работа_4_ZeroMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            ZContext context = new ZContext();
            ZSocket socket = new ZSocket(context, ZSocketType.PUSH);
            socket.Bind("tcp://127.0.0.1:5822");
            Console.WriteLine("Сервер запущен.");
            while (true) {
                Console.Write("> ");
                string message = Console.ReadLine();
                socket.SendFrame(new ZFrame(message));
            }
        }
    }
}
