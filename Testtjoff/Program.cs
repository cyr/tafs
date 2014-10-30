using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testtjoff
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\test\tjoff\hej.rar\what\haha.txt";

            Console.WriteLine(Path.GetPathRoot(path));

            Console.WriteLine(path);

            path = Path.GetDirectoryName(path);

            Console.WriteLine(path);

            path = Path.GetDirectoryName(path);

            Console.WriteLine(path);

            path = Path.GetDirectoryName(path);
            Console.WriteLine(path);

            path = Path.GetDirectoryName(path);

            Console.WriteLine(path);

            path = Path.GetDirectoryName(path);

            Console.WriteLine(path);

            path = Path.GetDirectoryName(path);

            Console.WriteLine(path);

            path = Path.GetDirectoryName(path);
            Console.WriteLine(path);

            Console.ReadLine();
        }
    }
}
