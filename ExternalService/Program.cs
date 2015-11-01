using System;
using Microsoft.Owin.Hosting;

namespace ExternalService
{
    class Program
    {
        static void Main(string[] args)
        {
            WebApp.Start<Startup>(new StartOptions("http://localhost:8088"));

            Console.ReadLine();
        }
    }
}