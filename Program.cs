using MonsterGame.Engine;
using MonsterGame.Engine.Enums;

namespace MonsterGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Run run = new Run();

            run.MainMenu();

            while (run.CurrentScreen != Screen.Exit.ToString())
            {
                if (Screen.CharacterSelect.ToString() == run.CurrentScreen)
                {
                    var champion = run.CharacterSelect();

                    if (Screen.InGame.ToString() == run.CurrentScreen)
                    {
                        run.InGame(champion);
                    }
                }
            }
        }
    }
}
