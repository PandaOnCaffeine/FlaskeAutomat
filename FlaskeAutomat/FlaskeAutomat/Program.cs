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
            //Creates a random variable
            Random rand = new Random();

            while (true)
            {
                //100 millisec delay
                Thread.Sleep(100);

                //Locks buffer array
                lock (buffer)
                {
                    //if buffer is full then it waits on a pulse from splitter
                    if (bufferIndex == buffer.Length)
                    {
                        Monitor.Wait(buffer);
                    }
                    //random number between 0 and 1
                    int rng = rand.Next(0, 2);

                    //If number is 0 then produces a beer 
                    if (rng == 0)
                    {
                        //Adds a beer to the buffer array
                        buffer[bufferIndex] = new Beer("Beer"); bufferIndex++;
                        Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Produced beer", Console.ForegroundColor = ConsoleColor.DarkBlue);
                    }
                    //Else produces a energydrink
                    else
                    {
                        //Adds a energydrink to the buffer array
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
                //Locks buffer array
                lock (buffer)
                {
                    //If buffer array is full
                    if (bufferIndex > 9)
                    {
                        //For loop that loop through buffer array and splits the beers into a beers array and energydrinks to a energydrinks array 
                        for (int i = 0; i < bufferIndex; i++)
                        {
                            //100 millisec delay
                            Thread.Sleep(100);

                            //if object is a beer
                            if (buffer[i].Name == "Beer")
                            {
                                //Locks beers array
                                lock (beers)
                                {
                                    //if beers array is full, then monitor wait, waits till consumer send a pulse and array is empty
                                    if (beersIndex > 9)
                                    {
                                        Monitor.Wait(beers);
                                    }
                                    
                                    //Sets a index in beers array to the beer from buffer array
                                    beers[beersIndex] = buffer[i]; beersIndex++;

                                    //Removes the beer from buffer array
                                    buffer[i] = null;
                                    Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Moved Beer from buffer to beers", Console.ForegroundColor = ConsoleColor.DarkBlue);
                                }
                            }
                            //else, its not a beer then its a energydrink
                            else
                            {
                                //Locks energydrinks array
                                lock (energydrinks)
                                {
                                    //If energydrinks array is full, then monitor wait, waits till consumer send a pulse and array is empty
                                    if (energyIndex > 9)
                                    {
                                        Monitor.Wait(energydrinks);
                                    }

                                    //Sets a index in energydrinks array to the energydrink from buffer array
                                    energydrinks[energyIndex] = buffer[i]; energyIndex++;

                                    //Removes the energydrink from buffer array
                                    buffer[i] = null;
                                    Console.WriteLine($"[{Thread.CurrentThread.Name}]  | Moved Energy Drink from buffer to Energydrinks", Console.ForegroundColor = ConsoleColor.Green);
                                }
                            }
                        }
                        //Sets buffer back to 0
                        bufferIndex = 0;

                        //Pulse the producer that it can start producing again
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
