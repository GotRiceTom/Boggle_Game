using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyGraphTestCases;

namespace Profiler
{
    class Program
    {
        static void Main(string[] args)
        {
            new DependencyGraphTestCases.UnitTest1().StressTest15();
            for (int i = 0; i < 1000000; i ++) {
                Console.WriteLine(i);

            }
        }
    }
}
