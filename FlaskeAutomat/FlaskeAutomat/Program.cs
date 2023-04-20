using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FlaskeAutomat
{
    internal class Program
    {
        static readonly object _lock = new object();
        static readonly object _beerLock = new object();
        static readonly object _energyLock = new object();


        static int bufferIndex = 0;
        static Drink[] buffer = new Drink[10];

        static int beersIndex = 0;
        static Drink[] beers = new Beer[10];

        static int energyIndex = 0;
        static Drink[] energiDrinks = new EnergiDrink[10];

        static void Main(string[] args)
        {
            Thread p1 = new Thread(new ThreadStart(Producer));
            p1.Name = "Producer";
            p1.Start();

            Thread s1 = new Thread(new ThreadStart(Splitter));
            s1.Name = "Splitter";
            s1.Priority = ThreadPriority.BelowNormal;
            s1.Start();

            Thread c1 = new Thread(new ThreadStart(BeerConsumer));
            c1.Name = "Consumer 1";
            c1.Start();

            Thread c2 = new Thread(new ThreadStart(EnergyConsumer));
            c2.Name = "Consumer 2";
            c2.Start();


            Console.ReadLine();
        }
        static void Producer()
        {
            while (true)
            {
                Random rand = new Random();
                Monitor.Enter(_lock);
                while (bufferIndex < 9)
                {
                    Thread.Sleep(200);
                    int rng = rand.Next(0, 2);


                    if (rng % 2 == 0)
                    {
                        buffer[bufferIndex] = new Beer("Beer"); bufferIndex++;
                        Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Produced beer");
                    }
                    else
                    {
                        buffer[bufferIndex] = new EnergiDrink("EnergyDrink"); bufferIndex++;
                        Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Produced EnergyDrink");
                    }
                }
                Monitor.Exit(_lock);
            }
        }
        static void Splitter()
        {
            while (true)
            {
                if (bufferIndex > 1)
                {
                    Monitor.Enter(_lock);

                    foreach (Drink item in buffer)
                    {
                        Thread.Sleep(200);
                        if (item == null)
                        {
                        }
                        else if (item.Name == "Beer")
                        {
                            beers[beersIndex] = item; beersIndex++;

                            buffer[bufferIndex] = null; bufferIndex--;
                            Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Moved Beer from buffer to beers");
                        }
                        else
                        {
                            energiDrinks[energyIndex] = item; energyIndex++;

                            buffer[bufferIndex] = null; bufferIndex--;
                            Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Moved Energy Drink from buffer to Energydrinks");
                        }
                    }
                    Monitor.Exit(_lock);

                }
            }
        }
        static void BeerConsumer()
        {
            while (true)
            {
                if (beers.Length > 1)
                {
                    lock (_beerLock)
                    {

                        foreach (Drink item in beers)
                        {
                            Thread.Sleep(200);
                            if (item == null)
                            {
                            }
                            else
                            {
                                Console.WriteLine($"[{Thread.CurrentThread.Name}]| Beer has been consumt");
                                beers[beersIndex] = null; if (beersIndex == 0) { } else { beersIndex--; }
                            }
                        }
                    }
                }
            }
        }
        static void EnergyConsumer()
        {
            while (true)
            {
                if (energiDrinks.Length > 1)
                {
                    lock (_energyLock)
                    {

                        foreach (Drink item in energiDrinks)
                        {
                            Thread.Sleep(200);
                            if (item == null)
                            {
                            }
                            else
                            {
                                Console.WriteLine($"[{Thread.CurrentThread.Name}]| Energy Drink has been consumt");
                                energiDrinks[energyIndex] = null; if (energyIndex == 0) { } else { energyIndex--; };
                            }
                        }
                    }
                }
            }
        }
    }

}
