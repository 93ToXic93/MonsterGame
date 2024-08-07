﻿namespace MonsterGame.Engine.Models
{
    public abstract class Champion
    {
        public int Strength { get; set; }

        public int Agility { get; set; }

        public int Intelligence { get; set; }

        public int Range { get; set; }

        public char Symbol { get; set; }

        public int Health { get; set; }

        public int Mana { get; set; }

        public int Damage { get; set; }

        public int PositionRow { get; set; }

        public int PositionCol { get; set; }

        public void Setup()
        {
            Health = Strength * 5;
            Mana = Intelligence * 3;
            Damage = Agility * 2;
        }
    }
}
