using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа_4
{
    class Program
    {
        [DllImport("user32", EntryPoint = "CallWindowProcW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr CallWindowProcW([In] byte[] bytes, IntPtr hWnd, int msg, [In, Out] byte[] wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool VirtualProtect([In] byte[] bytes, IntPtr size, int newProtect, out int oldProtect);

        const int PAGE_EXECUTE_READWRITE = 0x40;

        static void Main(string[] args)
        {
            Console.WriteLine("Введите количество элементов массива:");
            int[] arr = new int[int.Parse(Console.ReadLine())];

            Console.WriteLine("Введите элементы массива (через Enter)");
            for (int index = 0; index < arr.Length; index++)
                arr[index] = int.Parse(Console.ReadLine());

            Console.WriteLine("Введённый массив: ");
            for (int index = 0; index < arr.Length; index++)
                Console.Write(arr[index] + " ");
            Console.WriteLine();

            int max = arr[0];
            for (int index = 1; index < arr.Length; index++)
                if (compare(arr[index], max))
                    max = arr[index];
            Console.WriteLine("Максимальное число: " + max);

            int evens = 0;
            for (int index = 0; index < arr.Length; index++)
                if (isEven(arr[index]))
                    evens++;
            Console.WriteLine("Количество чётных чисел: " + evens);


            int sum = 0;
            for (int index = 0; index < arr.Length; sum = add(arr[index], sum), index++) ;
            Console.WriteLine("Сумма всех чисел: " + sum);

            int[] arr2 = new int[arr.Length];
            Console.WriteLine("Введите второй массив (такой же длины)");
            for (int index = 0; index < arr2.Length; index++)
                arr2[index] = int.Parse(Console.ReadLine());

            Console.WriteLine("Сумма массивов:");
            for (int index = 0; index < arr.Length; index++)
                Console.Write(add(arr[index], arr2[index]) + " ");
            Console.WriteLine();


            Console.ReadKey();

            Console.ReadKey();
        }

        private static bool isEven(int a) {
            int num;
            byte[] result = new byte[256];
            byte[] abytes = BitConverter.GetBytes(a);
            
            byte[] code = new byte[] {
                //Нужно для возврата результата (из eax)
                0x55,                      /* push ebp */
                0x89, 0xe5,                /* mov  ebp, esp */
                0x57,                      /* push edi */
                0x8b, 0x7d, 0x10,          /* mov  edi, [ebp+0x10] */


                0xb8, abytes[0], abytes[1], abytes[2], abytes[3], //mov eax, A - Пишем число а в регистр eax
                0x83, 0xe0, 0x01, //Применяем операцию & к числу и 1. Если оно чётное, то в eax запишется 0, если нет, то 1
               
                0x83, 0xf8, 0x00, //Сравниваем число в eax с нулём
                0x74, 0x07,                   //jg 7 - Если число в eax = 0, то "прыгаем" на 7 байт дальше
                0xb8, 0x00, 0x00, 0x00, 0x00, //mov eax,0x0 - пишем 0 в eax
                0xeb, 0x05,                   //jmp 5 - прыгаем на 5 байт вперёд
                0xb8, 0x01, 0x00, 0x00, 0x00, //mov eax,0x1 - пишем 1 в eax

                //Нужно для возврата результата (из eax)
                0x89, 0x07,                /* mov  [edi], eax */
                0x89, 0x57, 0x04,          /* mov  [edi+0x4], edx */
                0x5f,                      /* pop  edi */
                0x89, 0xec,                /* mov  esp, ebp */
                0x5d,                      /* pop  ebp */
                0xc2, 0x10, 0x00,          /* ret  0x10 */
            };

            IntPtr ptr = new IntPtr(code.Length);

            if (!VirtualProtect(code, ptr, PAGE_EXECUTE_READWRITE, out num))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            ptr = new IntPtr(result.Length);

            CallWindowProcW(code, IntPtr.Zero, 0, result, ptr);
            return BitConverter.ToInt32(result, 0) != 0;
        }

        private static int add(int a, int b) {
            int num;
            byte[] result = new byte[256];
            byte[] abytes = BitConverter.GetBytes(a);
            byte[] bbytes = BitConverter.GetBytes(b);
            byte[] code = new byte[] {
                //Нужно для возврата результата (из eax)
                0x55,                      /* push ebp */
                0x89, 0xe5,                /* mov  ebp, esp */
                0x57,                      /* push edi */
                0x8b, 0x7d, 0x10,          /* mov  edi, [ebp+0x10] */


                0xB8, abytes[0], abytes[1], abytes[2], abytes[3], //Пишем число а в регистр eax
                0x05, bbytes[0], bbytes[1], bbytes[2], bbytes[3], //Добавляем число b в регистр eax

                //Нужно для возврата результата (из eax)
                0x89, 0x07,                /* mov  [edi], eax */
                0x89, 0x57, 0x04,          /* mov  [edi+0x4], edx */
                0x5f,                      /* pop  edi */
                0x89, 0xec,                /* mov  esp, ebp */
                0x5d,                      /* pop  ebp */
                0xc2, 0x10, 0x00,          /* ret  0x10 */
            };

            IntPtr ptr = new IntPtr(code.Length);

            if (!VirtualProtect(code, ptr, PAGE_EXECUTE_READWRITE, out num))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            ptr = new IntPtr(result.Length);

            CallWindowProcW(code, IntPtr.Zero, 0, result, ptr);
            return BitConverter.ToInt32(result, 0);
        }

        private static bool compare(int a, int b)
        {
            int num;
            byte[] result = new byte[256];
            byte[] abytes = BitConverter.GetBytes(a);
            byte[] bbytes = BitConverter.GetBytes(b);
            byte[] code = new byte[] {
                //Нужно для возврата результата (из eax)
                0x55,                      /* push ebp */
                0x89, 0xe5,                /* mov  ebp, esp */
                0x57,                      /* push edi */
                0x8b, 0x7d, 0x10,          /* mov  edi, [ebp+0x10] */

                0xB8, abytes[0], abytes[1], abytes[2], abytes[3], //mov eax, A - Пишем число а в регистр eax
                0x3d, bbytes[0], bbytes[1], bbytes[2], bbytes[3], //cmp eax, B - Сравниваем число в регистре eax (a) и b
                0x7f, 0x07,                   //jg 7 - Если а > b, то "прыгаем" на 7 байт дальше
                0xb8, 0x00, 0x00, 0x00, 0x00, //mov eax,0x0 - пишем 0 в eax
                0xeb, 0x05,                   //jmp 5 - прыгаем на 5 байт вперёд
                0xb8, 0x01, 0x00, 0x00, 0x00, //mov eax,0x1 - пишем 1 в eax

                //Нужно для возврата результата (из eax)
                0x89, 0x07,                /* mov  [edi], eax */
                0x89, 0x57, 0x04,          /* mov  [edi+0x4], edx */
                0x5f,                      /* pop  edi */
                0x89, 0xec,                /* mov  esp, ebp */
                0x5d,                      /* pop  ebp */
                0xc2, 0x10, 0x00,          /* ret  0x10 */
            };

            IntPtr ptr = new IntPtr(code.Length);

            if (!VirtualProtect(code, ptr, PAGE_EXECUTE_READWRITE, out num))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            ptr = new IntPtr(result.Length);

            CallWindowProcW(code, IntPtr.Zero, 0, result, ptr);
            return BitConverter.ToInt32(result, 0) != 0;
        }
    }
}
