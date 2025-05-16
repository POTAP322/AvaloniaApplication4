using System;

namespace AvaloniaApplication2.Models
{
    public class Dog : Creature
    {
        public event EventHandler? SoundGiven;

        public Dog(int speedStep, int maxSpeed) : base(speedStep, maxSpeed) { }

        public override void Move()
        {
            if (Speed < MaxSpeed)
            {
                Speed += SpeedStep;
            }
        }

        public override void Stand()
        {
            if (Speed > 0)
            {
                Speed -= SpeedStep;
            }
        }

        public void MakeSound()
        {
            SoundGiven?.Invoke(this, EventArgs.Empty);
        }
    }
}
