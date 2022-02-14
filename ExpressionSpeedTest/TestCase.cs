using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSpeedTest
{
    public class TestCase
    {
        static Person person = new Person() { Name = "a" };
        public TestCase()
        {

        }
        public static void Run1()
        {
            var info = person.GetType().GetProperty("Name");

            Console.WriteLine($"----------------------{nameof(Helper.SetValue)}----------------------");
            Helper.SetValue(info, person, "777");
            Console.WriteLine(person.Name);

            Console.WriteLine($"----------------------{nameof(Helper.SetValue2)}----------------------");
            Helper.SetValue2(info, person, "888");
            Console.WriteLine(person.Name);

            Console.WriteLine($"----------------------{nameof(Helper.GetPropertyValue)}----------------------");
            Console.WriteLine(Helper.GetPropertyValue(info, person));
        }

        public static void Run2()
        {
            Console.WriteLine($"----------------------{nameof(Helper.Add)}----------------------");
            Console.WriteLine(Helper.Add(1, 7));
        }
        [Benchmark]
        public void SetWithExpression1()
        {
            var info = person.GetType().GetProperty("Name");

            for (int i = 0; i < 1000; i++)
            {
                Helper.SetValue(info, person, "777");
            }
        }
        [Benchmark]
        public void SetWithExpression2()
        {
            var info = person.GetType().GetProperty("Name");

            for (int i = 0; i < 1000; i++)
            {
                Helper.SetValue2(info, person, "888");
            }
        }
        [Benchmark]
        public void SetWithExpressionAndTemp()
        {
            var info = person.GetType().GetProperty("Name");

            for (int i = 0; i < 1000; i++)
            {
                Helper.SetValueWithTemp(info, person, "888");
            }
        }
        [Benchmark]
        public void SetWithReflection()
        {
            var info = person.GetType().GetProperty("Name");

            for (int i = 0; i < 1000; i++)
            {
                Helper.SetValueWithRefleaction(info, person, "888");
            }
        }

        public static void TestBenchMark()
        {
            BenchmarkRunner.Run<TestCase>();
        }
    }
}
