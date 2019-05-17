using System;

namespace SPR_22
{
    class DrunkAnimal
    {
        private string name;
        private int health;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Health
        {
            get { return health; }
            private set
            {
                if (value > 100)
                {
                    health = 100;
                }
                else if (value < 0)
                {
                    health = 0;
                }
                else
                {
                    health = value;
                }
            }
        }
        public DrunkAnimal()
        {
            this.Name = string.Empty;
            this.Health = 100;
        }

        public DrunkAnimal(string name) : this()
        {
            this.Name = name;
        }

        public bool IsDead()
        {
            return health <= 0;
        }

        public void Attack(DrunkAnimal enemy)
        {
            int damage = new Random().Next(1, 20);
            enemy.TakeDamage(damage);
            Console.WriteLine($"{this.name} hit {enemy.name} for {damage} damage ({enemy.health}% hp left)!");

            if (enemy.health == 0)
            {
                Console.WriteLine($"{this.name} killed {enemy.name}...");
            }
        }

        public void TakeDamage(int damage)
        {
            this.Health -= damage;
        }

        public void Heal(int health)
        {
            this.Health += health;
        }

        public override string ToString()
        {
            return $"{name} with {health}% hp";
        }
    }
}
