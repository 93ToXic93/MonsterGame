namespace MonsterGame.Engine.Models
{
    public class Mage : Champion
    {
        public Mage()
        {
            PositionRow = 1;
            PositionCol = 1;
            Strength = 2;
            Agility = 1;
            Intelligence = 3;
            Range = 3;
            Symbol = '*';
            Setup();
        }
    }
}
