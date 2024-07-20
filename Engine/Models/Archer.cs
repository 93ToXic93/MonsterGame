namespace MonsterGame.Engine.Models
{
    public class Archer : Champion
    {
        public Archer()
        {
            PositionRow = 1;
            PositionCol = 1;
            Strength = 2;
            Agility = 4;
            Intelligence = 0;
            Range = 2;
            Symbol = '#';
            Setup();
        }
    }
}
