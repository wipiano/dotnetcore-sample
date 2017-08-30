using System;
using System.Linq;
using System.Text;

using NetCoreSample.StandardLibrary;

namespace NetCoreSample.MessagePackNet461
{
    class Program
    {
        static void Main(string[] args)
        {
            var deliveryScore = new ImmutablePerson("taro", "yamada", new DateTime(1993, 1, 5));
            var targetObj = Enumerable.Repeat(deliveryScore, 3).ToArray();

            Console.WriteLine(ToJson(targetObj));

            var serialized = targetObj.ToMessagePackBinary();
            var lz4Serialized = targetObj.ToLZ4MessagePackBinary();

            Console.WriteLine("MessagePack: ");
            Console.WriteLine(Encoding.ASCII.GetChars(serialized));
            Console.WriteLine("LZ4MessagePack: ");
            Console.WriteLine(Encoding.ASCII.GetChars(lz4Serialized));

            var deserialized = PersonArraySerializer.FromMessagePackBinary(serialized);
            var lz4Deserialized = PersonArraySerializer.FromLZ4MessagePackBinary(lz4Serialized);

            Console.WriteLine(ToJson(deserialized));
            Console.WriteLine(ToJson(lz4Deserialized));
        }

        private static string ToJson(object obj) => MessagePack.MessagePackSerializer.ToJson(obj);
    }
}