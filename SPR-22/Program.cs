using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPR_22
{
    class Program
    {
        private static ConcurrentQueue<DrunkAnimal> drunks;
        private static ConcurrentBag<Task> fights = new ConcurrentBag<Task>();
        private static volatile bool theFightGoesOn = true;
        private static int fightsCount = 0;

        static void Main()
        {
            Console.Write("Enter tournament size between 4 and 32: ");

            int tournamentSize;
            while (!int.TryParse(Console.ReadLine(), out tournamentSize)
                || tournamentSize < 4
                || tournamentSize > 32)
            {
                Console.Write("Incorrect input, try again: ");
            }

            drunks = new ConcurrentQueue<DrunkAnimal>();
            for (int i = 0; i < tournamentSize; i++)
            {
                drunks.Enqueue(new DrunkAnimal($"Fighter {i + 1}"));
            }

            // the judge stops drinking and starts watching
            Task.Run(() => Judge());
            while (theFightGoesOn)
            {
                // poor guy has to wait for someone to stop fighting so he can join
                if (drunks.Count < 2)
                {
                    // lets not loop toooooooo fast
                    Thread.Sleep(100);
                    continue;
                }

                drunks.TryDequeue(out DrunkAnimal drunk);
                drunks.TryDequeue(out DrunkAnimal anotherDrunk);

                fights.Add(Task.Run(() => Fight(drunk, anotherDrunk)));
            }

            drunks.TryDequeue(out DrunkAnimal winner);

            Console.WriteLine();
            Console.WriteLine($"Match is over, the winner is: {winner.Name}");
            Console.WriteLine($"Total fights: {fightsCount}");
        }
        
        // the judge of the fight
        private static void Judge()
        {
            int retries = 0;
            while (retries < 5)
            {
                if (fights.All(x => x.IsCompleted))
                {
                    Thread.Sleep(1 * 1000);
                    retries++;
                    continue;
                }
                retries = 0;
            }

            theFightGoesOn = false;
        }

        private static void Fight(DrunkAnimal drunk, DrunkAnimal anotherDrunk)
        {
            Console.WriteLine($"{Environment.NewLine}{drunk.Name} CHALLENGED {anotherDrunk.Name}'s mother");

            int turn = 1;
            while (!drunk.IsDead() && !anotherDrunk.IsDead())
            {
                if (turn % 2 == 0)
                {
                    drunk.Attack(anotherDrunk);
                }
                else
                {
                    anotherDrunk.Attack(drunk);
                }

                Thread.Sleep(1 * 1000);
                turn++;
            }

            DrunkAnimal survivor = drunk.IsDead() ? anotherDrunk : drunk;
            survivor.Heal(1337);
            drunks.Enqueue(survivor);

            Interlocked.Increment(ref fightsCount);
        }
    }
}
