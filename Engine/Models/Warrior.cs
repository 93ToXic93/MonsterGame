namespace MonsterGame.Engine.Models
{
    public class Warrior : Champion
    {
        public Warrior()
        {
            PositionRow = 1;
            PositionCol = 1;
            Strength = 3;
            Agility = 3;
            Intelligence = 0;
            Range = 1;
            Symbol = '@';
            Setup();
        }
    }
}
