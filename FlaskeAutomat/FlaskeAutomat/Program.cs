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
        static Drink[] beers = new Drink[10];

        static int energyIndex = 0;
        static Drink[] energidrinks = new Drink[10];

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

            Thread c2 = new Thread(new ThreadStart(EnergydrinksConsumer));
            c2.Name = "Consumer 2";
            c2.Start();


            Console.ReadLine();
        }
        static void Producer()
        {
            while (true)
            {
                Monitor.Enter(_lock);
                Random rand = new Random();
                while (bufferIndex < 9)
                {

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
                if (bufferIndex > 5)
                {
                    Monitor.TryEnter(_lock);
                    Monitor.TryEnter(_beerLock);
                    Monitor.TryEnter(_energyLock);

                    for (int i = 0; i < bufferIndex - 1; i++)
                    {
                        if (buffer[i].Name == "Beer")
                        {
                            beers[beersIndex] = buffer[i]; beersIndex++;

                            buffer[i] = null; bufferIndex--;
                            Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Moved Beer from buffer to beers");
                        }
                        else
                        {
                            energidrinks[energyIndex] = buffer[i]; energyIndex++;

                            buffer[i] = null; bufferIndex--;
                            Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Moved Energy Drink from buffer to Energydrinks");
                        }
                    }
                    Monitor.PulseAll(_beerLock);
                    Monitor.PulseAll(_energyLock);
                    Monitor.PulseAll(_lock);

                }
            }
        }
        static void BeerConsumer()
        {
            while (true)
            {
                if (beersIndex > 4)
                {
                    lock (_beerLock)
                    {
                        Monitor.Wait(_beerLock);
                        for (int i = 0; i < beersIndex - 1; i++)
                        {
                            Console.WriteLine($"[{Thread.CurrentThread.Name}]| Beer has been consumt");
                            beers[i] = null;
                        }
                        beersIndex = 0;
                    }
                }
            }
        }
        static void EnergydrinksConsumer()
        {
            while (true)
            {
                if (energyIndex > 0)
                {
                    lock (_energyLock)
                    {
                        Monitor.Wait(_energyLock);
                        for (int i = 0; i < energyIndex - 1; i++)
                        {
                            Console.WriteLine($"[{Thread.CurrentThread.Name}]| Energy Drink has been consumt");
                            energidrinks[i] = null;
                        }

                    }
                }
            }
        }
    }

}
