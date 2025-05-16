using System;

namespace AvaloniaApplication2.Models
{
    public class Panther : Creature
    {
        public event EventHandler? SoundGiven;
        public event EventHandler? TreeClimbed;
        public event EventHandler? GetDownFromTree;
        public bool IsOnTree { get; private set; }

        public Panther(int speedStep, int maxSpeed) : base(speedStep, maxSpeed) { }

        public override void Move()
        {
            if (Speed < MaxSpeed && IsOnTree == false)
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

        public void ClimbTree()
        {
            if( IsOnTree == false){
                Speed = 0;
                TreeClimbed?.Invoke(this, EventArgs.Empty);
                IsOnTree = true;
            }
            
        }

        public void GetDown()
        {
            if(IsOnTree == true){
                GetDownFromTree?.Invoke(this, EventArgs.Empty);
                IsOnTree = false;
            }
            
        }
    }
}
