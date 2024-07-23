namespace MonsterGame.Engine.Models
{
    public class Monster : Champion
    {
        private static Random _random;

        static Monster()
        {
            _random = new Random();
        }

        public Monster()
        {
            Strength = _random.Next(1, 4);
            Agility = _random.Next(1, 4);
            Intelligence = _random.Next(1, 4);
            Range = 1;
            Symbol = 'M';
            SetupMonster();
        }
        private void SetupMonster()
        {
            Health = Strength;
            Mana = Intelligence;
            Damage = Agility;
        }
    }
}
