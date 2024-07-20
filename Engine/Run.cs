using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MonsterGame.Engine.Enums;
using MonsterGame.Engine.Models;
using MonsterGame.Infrastructure;
using MonsterGame.Infrastructure.Models;

namespace MonsterGame.Engine
{
    public class Run
    {
        public string CurrentScreen { get; private set; } = default!;

        public void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome!");
            Console.WriteLine("Press any key to play.");
            Console.ReadKey();
            CurrentScreen = Screen.CharacterSelect.ToString();
        }

        public Champion CharacterSelect()
        {
            Console.Clear();
            Console.WriteLine("Choose character type:");
            Console.WriteLine("Options:");
            Console.WriteLine("1) Warrior");
            Console.WriteLine("2) Archer");
            Console.WriteLine("3) Mage");
            Champion champion = null;


            while (champion == null)
            {
                Console.Write("Your pick: ");
                char choice = Console.ReadKey().KeyChar;

                switch (choice)
                {
                    case '1':
                        champion = new Warrior();
                        break;
                    case '2':
                        champion = new Archer();
                        break;
                    case '3':
                        champion = new Mage();
                        break;
                    default:
                        Console.WriteLine("\nInvalid choice, please select 1, 2 or 3.");
                        continue;
                }
            }

            Console.Clear();
            Console.WriteLine("Would you like to buff up your stats before starting? (Limit: 3 points total)");
            Console.Write("Response (Y/N): ");
            char response = Console.ReadKey().KeyChar;

            while (response != 'Y' && response != 'y' && response != 'n' && response != 'N')
            {
                Console.Clear();
                Console.WriteLine("Invalid input, try again!");
                Console.WriteLine("Would you like to buff up your stats before starting? (Limit: 3 points total)");
                Console.Write("Response (Y/N): ");
                response = Console.ReadKey().KeyChar;
            }


            if (response == 'Y' || response == 'y')
            {
                Console.Clear();
                int remainingPoints = 3;
                while (remainingPoints > 0)
                {
                    Console.WriteLine($"Remaining Points: {remainingPoints}");
                    Console.Write("Add to Strength: ");
                    string addStr = Console.ReadLine();

                    var isNumber = int.TryParse(addStr, out var addStrength);

                    if (addStrength > remainingPoints || !isNumber)
                    {
                        Console.WriteLine("Total points exceeded. Try again.");
                        continue;
                    }

                    remainingPoints -= addStrength;

                    if (remainingPoints == 0)
                    {
                        continue;
                    }

                    Console.Write("Add to Agility: ");

                    string addAgi = Console.ReadLine();

                    isNumber = int.TryParse(addAgi, out var addAgility);

                    if (addAgility > remainingPoints || !isNumber)
                    {
                        Console.WriteLine("Total points exceeded. Try again.");
                        continue;
                    }

                    remainingPoints -= addAgility;

                    if (remainingPoints == 0)
                    {
                        continue;
                    }

                    Console.Write("Add to Intelligence: ");
                    string addInt = Console.ReadLine();

                    isNumber = int.TryParse(addInt, out var addIntelligence);

                    if (addIntelligence > remainingPoints || !isNumber)
                    {
                        Console.WriteLine("Total points exceeded. Try again.");
                        continue;
                    }

                    remainingPoints -= addIntelligence;

                    if (remainingPoints == 0)
                    {
                        continue;
                    }

                    champion.Strength += addStrength;
                    champion.Agility += addAgility;
                    champion.Intelligence += addIntelligence;
                    champion.Setup();
                    remainingPoints -= (addStrength + addAgility + addIntelligence);

                }
            }

            using var dbContext = new ApplicationDbContext();

            Champ champ = new Champ()
            {
                Damage = champion.Agility,
                ChampType = champion.GetType().Name,
                CreatedOn = DateTime.Now,
                Mana = champion.Intelligence,
                Health = champion.Strength
            };

            dbContext.Champions.Add(champ);

            dbContext.SaveChanges();

            CurrentScreen = Screen.InGame.ToString();

            return champion;

        }

        public void InGame(Champion champion)
        {
            const int rowOfGrid = 10;
            const int colOfGrid = 10;
            bool isThePlayerMadeMove = true;

            Random random = new Random();
            List<Monster> monsters = new List<Monster>();

            while (true)
            {
                Console.Clear();

                char[,] grid = new char[rowOfGrid, colOfGrid];

                // Fill the grid with the default symbol
                for (int row = 0; row < rowOfGrid; row++)
                {
                    for (int col = 0; col < colOfGrid; col++)
                    {
                        grid[row, col] = '\u2592'; // Default symbol
                    }
                }

                Console.Write($"Health: {champion.Health} ");
                Console.Write($"Mana: {champion.Mana}");

                Console.WriteLine();
                Console.WriteLine();

                grid[champion.PositionRow, champion.PositionCol] = champion.Symbol;

                if (isThePlayerMadeMove)
                {
                    Monster newMonster = new Monster
                    {

                        PositionRow = random.Next(0, 10),
                        PositionCol = random.Next(0, 10)
                    };

                    monsters.Add(newMonster);

                    foreach (var monster in monsters)
                    {
                        if (monster.PositionRow < champion.PositionRow)
                        {
                            monster.PositionRow++;
                        }
                        else if (monster.PositionRow > champion.PositionRow)
                        {
                            monster.PositionRow--;
                        }

                        if (monster.PositionCol < champion.PositionCol)
                        {
                            monster.PositionCol++;
                        }
                        else if (monster.PositionCol > champion.PositionCol)
                        {
                            monster.PositionCol--;
                        }

                        grid[monster.PositionRow, monster.PositionCol] = monster.Symbol;
                    }
                }

                for (int row = 0; row < rowOfGrid; row++)
                {
                    for (int col = 0; col < colOfGrid; col++)
                    {
                        Console.Write(grid[row, col]);
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();

                Console.WriteLine("Choose action:");
                Console.WriteLine("1) Attack");
                Console.WriteLine("2) Move");
                Console.WriteLine("The only options are:");
                Console.WriteLine("W -> Move up");
                Console.WriteLine("S -> Move down");
                Console.WriteLine("D -> Move right");
                Console.WriteLine("А -> Move left");
                Console.WriteLine("Е -> Move diagonally up &right");
                Console.WriteLine("X -> Move diagonally down &right");
                Console.WriteLine("Q -> Move diagonally up &left");
                Console.WriteLine("Z -> Move diagonally down &left");
                Console.WriteLine("T -> Attack");

                char action = Console.ReadKey().KeyChar;

                string request = action.ToString().ToLower();

                if (request == "w") { champion.PositionRow -= 1; isThePlayerMadeMove = true; }
                else if (request == "s") { champion.PositionRow += 1; isThePlayerMadeMove = true; }
                else if (request == "a") { champion.PositionCol -= 1; isThePlayerMadeMove = true; }
                else if (request == "d") { champion.PositionCol += 1; isThePlayerMadeMove = true; }
                else if (request == "q") { champion.PositionRow -= 1; champion.PositionCol -= 1; isThePlayerMadeMove = true; }
                else if (request == "e") { champion.PositionRow -= 1; champion.PositionCol += 1; isThePlayerMadeMove = true; }
                else if (request == "z") { champion.PositionRow += 1; champion.PositionCol -= 1; isThePlayerMadeMove = true; }
                else if (request == "x") { champion.PositionRow += 1; champion.PositionCol += 1; isThePlayerMadeMove = true; }
                else if (request == "t")
                {
                    isThePlayerMadeMove = true;
                }
                else
                {
                    isThePlayerMadeMove = false;
                    continue;
                }


                if (champion.Health <= 0)
                {
                    Console.WriteLine("Game Over!");
                    break;
                }
            }
        }
    }
}
