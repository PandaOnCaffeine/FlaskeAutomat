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
        //Buffer array, Used by producer and splitter
        static int bufferIndex = 0;
        static Drink[] buffer = new Drink[10];

        //BeersBuffer Array, Used by splitter and beerConsumer
        static int beersIndex = 0;
        static Drink[] beers = new Drink[10];

        //Energydrinks array, Used by splitter and EnergyConsumer  
        static int energyIndex = 0;
        static Drink[] energydrinks = new Drink[10];

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
            Random rand = new Random();
            while (true)
            {
                Thread.Sleep(100);

                lock (buffer)
                {
                    if (bufferIndex == buffer.Length)
                    {
                        Monitor.Wait(buffer);
                    }
                    int rng = rand.Next(0, 2);

                    if (rng % 2 == 0)
                    {
                        buffer[bufferIndex] = new Beer("Beer"); bufferIndex++;
                        Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Produced beer", Console.ForegroundColor = ConsoleColor.DarkBlue);
                    }
                    else
                    {
                        buffer[bufferIndex] = new EnergiDrink("EnergyDrink"); bufferIndex++;
                        Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Produced EnergyDrink", Console.ForegroundColor = ConsoleColor.Green);
                    }
                }
            }
        }
        static void Splitter()
        {
            while (true)
            {
                lock (buffer)
                {
                    if (bufferIndex > 9)
                    {
                        for (int i = 0; i < bufferIndex; i++)
                        {
                            Thread.Sleep(100);

                            if (buffer[i].Name == "Beer")
                            {
                                lock (beers)
                                {
                                    if (beersIndex > 9)
                                    {
                                        Monitor.Wait(beers);
                                    }
                                    beers[beersIndex] = buffer[i]; beersIndex++;

                                    buffer[i] = null;
                                    Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Moved Beer from buffer to beers", Console.ForegroundColor = ConsoleColor.DarkBlue);
                                    Monitor.PulseAll(beers);
                                }
                            }
                            else
                            {
                                lock (energydrinks)
                                {
                                    if (energyIndex > 9)
                                    {
                                        Monitor.Wait(energydrinks);
                                    }
                                    energydrinks[energyIndex] = buffer[i]; energyIndex++;

                                    buffer[i] = null;
                                    Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Moved Energy Drink from buffer to Energydrinks", Console.ForegroundColor = ConsoleColor.Green);
                                }
                            }
                        }
                        bufferIndex = 0;
                        Monitor.PulseAll(buffer);
                    }

                }
            }
        }
        static void BeerConsumer()
        {
            while (true)
            {
                //If beers array is full
                if (beersIndex > 9)
                {
                    //Locks beers array
                    lock (beers)
                    {
                        //for loop, loops through beers array and consumes all the beers 
                        for (int i = 0; i < beersIndex; i++)
                        {
                            Thread.Sleep(100);
                            Console.WriteLine($"[{Thread.CurrentThread.Name}]| Beer has been consumt", Console.ForegroundColor = ConsoleColor.DarkBlue);
                            beers[i] = null;
                        }
                        //Sets beersIndex back to 0 beers, because there's no beers left in the array
                        beersIndex = 0;

                        //Pulses the monitor wait that are waiting on a pulse from beers 
                        Monitor.PulseAll(beers);
                    }
                }
            }
        }
        static void EnergydrinksConsumer()
        {
            while (true)
            {

                //If energydrink array is full
                if (energyIndex > 9)
                {
                    //Locks energydrinks array
                    lock (energydrinks)
                    {
                        //for loop, loops through energydrinks array and consumes the drinks
                        for (int i = 0; i < energyIndex - 1; i++)
                        {
                            Thread.Sleep(100);
                            Console.WriteLine($"[{Thread.CurrentThread.Name}]| Energy Drink has been consumt", Console.ForegroundColor = ConsoleColor.Green);
                            energydrinks[i] = null;
                        }
                        //sets energydrink index to 0
                        energyIndex = 0;

                        //Pulses all waiting threads that use energydrinks array
                        Monitor.PulseAll(energydrinks);
                    }
                }
            }
        }
    }

}
