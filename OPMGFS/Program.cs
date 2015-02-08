using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPMGFS
{
    using OPMGFS.Evolution;

    class Program
    {
        public static Random Random { get; private set; }

        static void Main(string[] args)
        {
            Random = new Random();
            TestEvolution();
        }

        private static void TestEvolution()
        {
            var evolverTest = new Evolver<EvolvableDoubleArray>(10, 10, 2, 10);
            for (var i = 0; i < evolverTest.Population.Count; i++)
            //{
            //    Console.Write(i + " has fitness: " + evolverTest.Population[i].Fitness + " [");
            //    //foreach (var number in evolvableDoubleArray.Numbers)
            //    //{
            //    //    Console.Write(number + ", ");
            //    //}
            //    Console.Write("]");
            //    Console.WriteLine();
            //}

            //Console.WriteLine();
            Console.WriteLine("------------------");
            Console.WriteLine("Starting Evolution");
            Console.WriteLine("------------------");
            Console.WriteLine();

            evolverTest.Evolve();

            for (var i = 0; i < evolverTest.Population.Count; i++)
            {
                Console.Write(i + " has fitness: " + evolverTest.Population[i].Fitness + " [");
                //foreach (var number in evolvableDoubleArray.Numbers)
                //{
                //    Console.Write(number + ", ");
                //}
                Console.Write("]");
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }
}
