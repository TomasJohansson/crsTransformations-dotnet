using System;
namespace Programmerare.CrsTransformations.TestClient {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Programmerare.CrsTransformations.TestClient starts");
            //var p = new LargerCSharpeExample();
            //var p = new SmallCSharpeExample();
            var p = new EpsgExample();
            p.method();
            Console.WriteLine("Programmerare.CrsTransformations.TestClient ends");
            Console.ReadLine();
        }
    }
}