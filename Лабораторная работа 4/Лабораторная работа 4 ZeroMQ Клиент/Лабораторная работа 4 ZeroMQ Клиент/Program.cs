using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;

namespace Лабораторная_работа_4_ZeroMQ_Клиент
{
    class Program
    {
        static void Main(string[] args)
        {
            ZContext context = new ZContext();
            ZSocket socket = new ZSocket(context, ZSocketType.PULL);
            socket.Connect("tcp://127.0.0.1:5822");
            while (true) {
                ZFrame frame = socket.ReceiveFrame();
                Console.WriteLine(frame.ReadString());
            }
        }
    }
}
