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

            int addStrength = 0;
            int addAgility = 0;
            int addIntelligence = 0;

            if (response == 'Y' || response == 'y')
            {
                Console.Clear();
                int remainingPoints = 3;
                while (remainingPoints > 0)
                {
                    Console.WriteLine($"Remaining Points: {remainingPoints}");
                    Console.Write("Add to Strength: ");
                    string addStr = Console.ReadLine();

                    var isNumber = int.TryParse(addStr, out addStrength);

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

                    isNumber = int.TryParse(addAgi, out addAgility);

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

                    isNumber = int.TryParse(addInt, out addIntelligence);

                    if (addIntelligence > remainingPoints || !isNumber)
                    {
                        Console.WriteLine("Total points exceeded. Try again.");
                        continue;
                    }

                    remainingPoints -= addIntelligence;

                }
                champion.Strength += addStrength;
                champion.Agility += addAgility;
                champion.Intelligence += addIntelligence;
                champion.Setup();
            }

            using var dbContext = new ApplicationDbContext();

            Champ champ = new Champ()
            {
                Damage = champion.Damage,
                ChampType = champion.GetType().Name,
                CreatedOn = DateTime.Now,
                Mana = champion.Mana,
                Health = champion.Health
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
            bool isThePlayerMadeMove = false;
            Random random = new Random();
            List<Monster> monsters = new List<Monster>();
            List<(int Row, int Col)> occupiedPositions = new List<(int Row, int Col)>();
            var grid = ClearAndSetTheFirstLookOfTheGrid(champion, rowOfGrid, colOfGrid);

            while (true)
            {

                Monster newMonster = new Monster
                {
                    PositionRow = random.Next(0, rowOfGrid),
                    PositionCol = random.Next(0, colOfGrid)
                };

                while (IsMonsterSpawningTooClose(newMonster, champion) ||
                       monsters.Any(m => m.PositionRow == newMonster.PositionRow && m.PositionCol == newMonster.PositionCol) ||
                       occupiedPositions.Any(p => p.Row == newMonster.PositionRow && p.Col == newMonster.PositionCol))
                {
                    newMonster.PositionRow = random.Next(0, rowOfGrid);
                    newMonster.PositionCol = random.Next(0, colOfGrid);
                }

                monsters.Add(newMonster);
                occupiedPositions.Add((newMonster.PositionRow, newMonster.PositionCol));
                grid[newMonster.PositionRow, newMonster.PositionCol] = newMonster.Symbol;

                UpdateTheGrid(rowOfGrid, colOfGrid, grid, monsters.Count, champion);

                while (!isThePlayerMadeMove)
                {
                    char action = Console.ReadKey().KeyChar;
                    string request = action.ToString().ToLower();

                    if (request == "t" || request == "w" || request == "s" || request == "a" || request == "d" || request == "q" || request == "e" || request == "z" || request == "x")
                    {
                        grid[champion.PositionRow, champion.PositionCol] = '\u2592';

                        (int newRow, int newCol) = request switch
                        {
                            "w" => (champion.PositionRow - 1, champion.PositionCol),
                            "s" => (champion.PositionRow + 1, champion.PositionCol),
                            "a" => (champion.PositionRow, champion.PositionCol - 1),
                            "d" => (champion.PositionRow, champion.PositionCol + 1),
                            "q" => (champion.PositionRow - 1, champion.PositionCol - 1),
                            "e" => (champion.PositionRow - 1, champion.PositionCol + 1),
                            "z" => (champion.PositionRow + 1, champion.PositionCol - 1),
                            "x" => (champion.PositionRow + 1, champion.PositionCol + 1),
                            _ => (champion.PositionRow, champion.PositionCol)
                        };

                        if (IsMoveOutOfBounds(newRow, newCol, rowOfGrid, colOfGrid) || monsters.Any(m => m.PositionRow == newRow && m.PositionCol == newCol))
                        {
                            Console.WriteLine("Wrong move");
                            continue;
                        }

                        champion.PositionRow = newRow;
                        champion.PositionCol = newCol;
                        isThePlayerMadeMove = true;

                        if (request == "t")
                        {
                            AttackMonster(champion, monsters, grid, ref isThePlayerMadeMove, occupiedPositions);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong move");
                    }
                }

                grid[champion.PositionRow, champion.PositionCol] = champion.Symbol;

                if (isThePlayerMadeMove)
                {
                    MoveMonstersTowardsPlayer(champion, monsters, grid);
                    isThePlayerMadeMove = false;
                }

                foreach (var monster in monsters)
                {
                    if (Math.Abs(monster.PositionRow - champion.PositionRow) <= 1
                        && Math.Abs(monster.PositionCol - champion.PositionCol) <= 1)
                    {
                        champion.Health -= monster.Damage;

                        Console.WriteLine($"Monster attacks the player! Player's health: {champion.Health}");
                        Thread.Sleep(1500);

                        if (champion.Health <= 0)
                        {
                            Console.WriteLine("Game Over!");
                            return;
                        }
                    }
                }
            }
        }

        private void AttackMonster(Champion champion, List<Monster> monsters, char[,] grid, ref bool isThePlayerMadeMove, List<(int Row, int Col)> occupiedPositions)
        {
            List<Monster> monstersToAttack = monsters.Where(m => Math.Abs(m.PositionRow - champion.PositionRow) <= champion.Range && Math.Abs(m.PositionCol - champion.PositionCol) <= champion.Range).ToList();

            if (!monstersToAttack.Any())
            {
                Console.WriteLine("No available targets in your range.");
                Thread.Sleep(1500);
                isThePlayerMadeMove = false;
                return;
            }

            Console.WriteLine("Choose who to attack?");
            for (int i = 0; i < monstersToAttack.Count; i++)
            {
                Console.WriteLine($"To attack monster {i + 1} press {i + 1} with health: {monstersToAttack[i].Health}");
            }

            while (true)
            {
                char actionAttack = Console.ReadKey().KeyChar;
                if (int.TryParse(actionAttack.ToString(), out int pressedKey) && pressedKey > 0 && pressedKey <= monstersToAttack.Count)
                {
                    var selectedMonster = monstersToAttack[pressedKey - 1];
                    selectedMonster.Health -= champion.Damage;

                    if (selectedMonster.Health <= 0)
                    {
                        monsters.Remove(selectedMonster);
                        occupiedPositions.Remove((selectedMonster.PositionRow, selectedMonster.PositionCol));
                        grid[selectedMonster.PositionRow, selectedMonster.PositionCol] = '\u2592';
                        Console.WriteLine("Well done! You killed the monster!");
                        Thread.Sleep(1500);
                    }
                    else
                    {
                        Console.WriteLine($"Remaining health of the monster: {selectedMonster.Health}");
                    }

                    Thread.Sleep(3000);
                    break;
                }

                Console.WriteLine("Try again, choose an available monster to attack.");
            }

            isThePlayerMadeMove = true;
        }

        private void MoveMonstersTowardsPlayer(Champion champion, List<Monster> monsters, char[,] grid)
        {
            foreach (var monster in monsters)
            {
                grid[monster.PositionRow, monster.PositionCol] = '\u2592';

                int rowDirection = Math.Sign(champion.PositionRow - monster.PositionRow);
                int colDirection = Math.Sign(champion.PositionCol - monster.PositionCol);

                (int newRow, int newCol) = (monster.PositionRow + rowDirection, monster.PositionCol);
                if (IsMoveOutOfBounds(newRow, newCol, 10, 10)
                    || monsters.Any(m => m.PositionRow == newRow && m.PositionCol == newCol)
                    || (newRow == champion.PositionRow && newCol == champion.PositionCol))
                {
                    newRow = monster.PositionRow;
                    newCol = monster.PositionCol + colDirection;

                    if (IsMoveOutOfBounds(newRow, newCol, 10, 10)
                        || monsters.Any(m => m.PositionRow == newRow && m.PositionCol == newCol)
                        || (newRow == champion.PositionRow && newCol == champion.PositionCol))
                    {
                        newRow = monster.PositionRow + rowDirection;
                        newCol = monster.PositionCol + colDirection;

                        if (IsMoveOutOfBounds(newRow, newCol, 10, 10)
                            || monsters.Any(m => m.PositionRow == newRow && m.PositionCol == newCol)
                            || (newRow == champion.PositionRow && newCol == champion.PositionCol))
                        {
                            newRow = monster.PositionRow;
                            newCol = monster.PositionCol;
                        }
                    }
                }

                monster.PositionRow = newRow;
                monster.PositionCol = newCol;


                grid[monster.PositionRow, monster.PositionCol] = monster.Symbol;
            }
        }

        private char[,] ClearAndSetTheFirstLookOfTheGrid(Champion champion, int rowOfGrid, int colOfGrid)
        {
            var grid = new char[rowOfGrid, colOfGrid];
            for (int row = 0; row < rowOfGrid; row++)
            {
                for (int col = 0; col < colOfGrid; col++)
                {
                    grid[row, col] = '\u2592';
                }
            }
            grid[champion.PositionRow, champion.PositionCol] = champion.Symbol;
            return grid;
        }

        private bool IsMonsterSpawningTooClose(Monster monster, Champion champion)
        {
            return Math.Abs(monster.PositionRow - champion.PositionRow) <= 1 && Math.Abs(monster.PositionCol - champion.PositionCol) <= 1;
        }

        private bool IsMoveOutOfBounds(int row, int col, int rowOfGrid, int colOfGrid)
        {
            return row < 0 || row >= rowOfGrid || col < 0 || col >= colOfGrid;
        }

        private void UpdateTheGrid(int rowOfGrid, int colOfGrid, char[,] grid, int monstersCount, Champion champion)
        {
            Console.Clear();

            Console.Write($"Your health: {champion.Health} ");
            Console.Write($"Your mana: {champion.Mana}");

            Console.WriteLine();
            Console.WriteLine();

            for (int row = 0; row < rowOfGrid; row++)
            {
                for (int col = 0; col < colOfGrid; col++)
                {
                    Console.Write(grid[row, col]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.WriteLine($"Monsters on the map: {monstersCount}");
            Console.WriteLine("Move with W, S, A, D or attack with T, diagonals: Q (up-left), E (up-right), Z (down-left), X (down-right)");
        }

    }
}
