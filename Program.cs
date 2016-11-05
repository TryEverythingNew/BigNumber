using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS422
{
    class Program
    {
        static void Main(string[] args)
        {
            // a lot of tests below
            BigNum test = new BigNum((double)-0.654493524, false);
            Console.Out.WriteLine(test);
            BigNum test1 = new BigNum((double)-0.654493524, true);
            Console.Out.WriteLine(test1);
            BigNum test2 = new BigNum("-5840.568");
            Console.Out.WriteLine(test2);
            BigNum test3 = new BigNum((double)-4.54896185, true);
            Console.Out.WriteLine(test3);
            BigNum test4 = new BigNum((double)-4.54896185, false);
            Console.Out.WriteLine(test4);
            test4 = new BigNum((double)-4.25, true);
            Console.Out.WriteLine(test4);
            test4 = new BigNum((double)-4.25, false);
            Console.Out.WriteLine(test4);
            test4 = new BigNum(test4.ToString());
            Console.Out.WriteLine(test4);
            test3 = new BigNum(test3.ToString());
            Console.Out.WriteLine(test3);
            test4 = new BigNum("0");
            Console.Out.WriteLine(test4);
            test4 = new BigNum("0.0");
            Console.Out.WriteLine(test4);
            Console.Out.WriteLine(test3 - test);
            Console.Out.WriteLine(test2 - test1);
            Console.Out.WriteLine(test1 + test2);
            Console.Out.WriteLine(test + test3);
            Console.Out.WriteLine(test > test3);
            Console.Out.WriteLine(test + test3 <= test2);
            Console.Out.WriteLine(test + test3 != test1);
            Console.Out.WriteLine(test - test3 < test4);
            Console.Out.WriteLine("Marker1.................");
            Console.Out.WriteLine(test - test3 * test4);
            Console.Out.WriteLine(test2 - test3 / test4);
            test4 = new BigNum("0.0");
            test3 = new BigNum("0.");
            test2 = new BigNum("0");
            test1 = new BigNum("0.00");
            Console.Out.WriteLine(test2 - test3 / test4);
            Console.Out.WriteLine(test1 == test1);
            Console.Out.WriteLine(test1 == test2);
            Console.Out.WriteLine(test1 == test3);
            Console.Out.WriteLine(test1 == test4);
            Console.Out.WriteLine(test2 == test3);
            Console.Out.WriteLine(test3 == test4);
            Console.Out.WriteLine(test2 == test4);
            test1 = new BigNum(0.5,false);
            test2 = new BigNum(-5.0,false);
            test3 = new BigNum(-0.03, false);
            Console.Out.WriteLine("Marker2.................");
            Console.Out.WriteLine(test4 / test3);
            Console.Out.WriteLine(test3 / test2);
            Console.Out.WriteLine(test3 / test1);
            Console.Out.WriteLine(test2 / test1);
            Console.Out.WriteLine(test2 / test2);
            Console.Out.WriteLine(test1 / test2);

            Console.Out.WriteLine("Marker3.................");
            Console.Out.WriteLine(double.MaxValue);
            Console.Out.WriteLine(double.MinValue);
            test4 = new BigNum( double.MaxValue.ToString());
            Console.Out.WriteLine(test4);
            test4 = new BigNum(double.MinValue.ToString());
            Console.Out.WriteLine(test4);
            Console.Out.WriteLine(test4.ToString().Length);
            Console.Out.WriteLine(BigNum.IsToStringCorrect(5));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(-5.0));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(-0.5));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(-0.25));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(0.15));

            Console.Out.WriteLine("Marker4.................");
            Console.Out.WriteLine(BigNum.IsToStringCorrect(double.MinValue));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(double.MaxValue));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(5));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(6.75));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(-1.25));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(-1.25e-3));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(-2.5e-1));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(-1.56e-5));
            Console.Out.WriteLine(BigNum.IsToStringCorrect(97.56e-89));

            Console.Out.WriteLine("Marker5.................");
            test4 = new BigNum(double.MinValue.ToString());
            Console.Out.WriteLine(test4);
            Console.Out.WriteLine(test4.ToString().Length);
            test4 = new BigNum(test4.ToString());
            Console.Out.WriteLine(test4);
            Console.Out.WriteLine(test4.ToString().Length);

            Console.Out.WriteLine("Marker5.................");
            double aaa = 1e-100;
            Console.Out.WriteLine(aaa);
            test4 = new BigNum(aaa, false);
            Console.Out.WriteLine(test4);


            aaa = 1e-321;
            Console.Out.WriteLine(aaa);
            test4 = new BigNum(aaa, false);
            Console.Out.WriteLine(test4);

            aaa = 1e-323;
            Console.Out.WriteLine(aaa);
            test4 = new BigNum(aaa, false);
            Console.Out.WriteLine(test4);

            aaa = 1e-324;
            Console.Out.WriteLine(aaa);
            test4 = new BigNum(aaa, false);
            Console.Out.WriteLine(test4);
        }
    }
}
