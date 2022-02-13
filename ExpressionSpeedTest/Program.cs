// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("It's a speed test for set ");

var person = new Person() { Name = "a" };
var info = person.GetType().GetProperty("Name");

Console.WriteLine($"----------------------{nameof(Helper.SetValue)}----------------------");
Helper.SetValue(info, person, "777");
Console.WriteLine(person.Name);

Console.WriteLine($"----------------------{nameof(Helper.SetValue2)}----------------------");
Helper.SetValue2(info, person, "777");
Console.WriteLine(person.Name);

Console.WriteLine($"----------------------{nameof(Helper.Add)}----------------------");
Console.WriteLine(Helper.Add(1, 7));

Console.WriteLine($"----------------------{nameof(Helper.GetPropertyValue)}----------------------");
Console.WriteLine(Helper.GetPropertyValue(info, person));
