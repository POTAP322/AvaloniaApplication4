namespace AvaloniaApplication2.Models
{
    public abstract class Creature : ICreature
    {
        private int _speed;
        private readonly int _speedStep;
        private readonly int _maxSpeed;

        public int Speed
        {
            get => _speed;
            protected set => _speed = value < 0 ? 0 : value;
        }

        public int SpeedStep => _speedStep;
        public int MaxSpeed => _maxSpeed;

        protected Creature(int speedStep, int maxSpeed)
        {
            _speedStep = speedStep;
            _maxSpeed = maxSpeed;
            _speed = 0;
        }

        public abstract void Move();
        public abstract void Stand();
    }
}