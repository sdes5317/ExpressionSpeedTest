using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSpeedTest
{
    public class TestCase
    {
        static Person person = new Person() { Name = "a" };

        private readonly int _numbers = 1000;

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
        public void SetByExpressionNoCache1()
        {
            var info = person.GetType().GetProperty("Name");

            for (int i = 0; i < _numbers; i++)
            {
                Helper.SetValue(info, person, "777");
            }
        }
        //[Benchmark]
        public void SetByExpressionNoCache2()
        {
            var info = person.GetType().GetProperty("Name");

            for (int i = 0; i < _numbers; i++)
            {
                Helper.SetValue2(info, person, "888");
            }
        }
        [Benchmark]
        public void SetByExpressionAndCache()
        {
            var info = person.GetType().GetProperty("Name");

            for (int i = 0; i < _numbers; i++)
            {
                Helper.SetValueByTempCache(info, person, "888");
            }
        }
        [Benchmark]
        public void SetByExpressionAndCacheDynamic()
        {
            var info = person.GetType().GetProperty("Name");

            for (int i = 0; i < _numbers; i++)
            {
                Helper.SetValueByCacheDynamic(info, person, "888");
            }
        }
        [Benchmark]
        public void SetByReflection()
        {
            var info = person.GetType().GetProperty("Name");

            for (int i = 0; i < _numbers; i++)
            {
                Helper.SetValueByRefleaction(info, person, "888");
            }
        }
        [Benchmark]
        public void SetByReflectionCache()
        {
            var info = person.GetType().GetProperty("Name");

            for (int i = 0; i < _numbers; i++)
            {
                Helper.SetValueByRefleactionCache(info, person, "888");
            }
        }
        [Benchmark]
        public void SetByNormal()
        {
            for (int i = 0; i < _numbers; i++)
            {
                person.Name = "888";
            }
        }
        [Benchmark]
        public void SetByAutoMapper()
        {
            var personForAutoMapper = new Person() { Name = "888" };

            for (int i = 0; i < _numbers; i++)
            {
                Helper.SetValueByAutoMapper(person, personForAutoMapper);
            }
        }

        public static void TestBenchMark()
        {
            BenchmarkRunner.Run<TestCase>();
        }

        public static void TestStopWatch()
        {
            var test = new TestCase();
            var stopWatch = new Stopwatch();
            Console.WriteLine("no cache");
            stopWatch.Start();
            test.SetByExpressionNoCache1();
            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);
            
            Console.WriteLine(nameof(SetByExpressionAndCache));
            stopWatch.Restart();
            test.SetByExpressionAndCache();
            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);

            Console.WriteLine(nameof(SetByReflection));
            stopWatch.Restart();
            test.SetByReflection();
            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }
    }
}
