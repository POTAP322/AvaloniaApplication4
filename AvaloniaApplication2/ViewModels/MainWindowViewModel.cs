using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AvaloniaApplication2.Models;

namespace AvaloniaApplication2.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly Dog _dog;
        private readonly Panther _panther;
        private readonly Turtle _turtle;

        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        [ObservableProperty]
        private int dogSpeed;

        [ObservableProperty]
        private int pantherSpeed;

        [ObservableProperty]
        private int turtleSpeed;

        public MainWindowViewModel()
        {
            _dog = new Dog(10, 50);
            _panther = new Panther(15, 75);
            _turtle = new Turtle(5, 15);

            _dog.SoundGiven += (sender, args) => Logs.Add("Dog barked!");
            _panther.SoundGiven += (sender, args) => Logs.Add("Panther roared!");
            _panther.TreeClimbed += (sender, args) => Logs.Add("Panther climbed the tree!");
            _panther.GetDownFromTree += (sender, args) => Logs.Add("Panther got down from the tree!");

            DogSpeed = _dog.Speed;
            PantherSpeed = _panther.Speed;
            TurtleSpeed = _turtle.Speed;
        }

        [RelayCommand]
        private void DogMove()
        {
            _dog.Move();
            DogSpeed = _dog.Speed;
        }

        [RelayCommand]
        private void DogStand()
        {
            _dog.Stand();
            DogSpeed = _dog.Speed;
        }

        [RelayCommand]
        private void DogMakeSound()
        {
            _dog.MakeSound();
        }

        [RelayCommand]
        private void PantherMove()
        {
            _panther.Move();
            PantherSpeed = _panther.Speed;
        }

        [RelayCommand]
        private void PantherStand()
        {
            _panther.Stand();
            PantherSpeed = _panther.Speed;
        }

        [RelayCommand]
        private void PantherMakeSound()
        {
            _panther.MakeSound();
        }

        [RelayCommand]
        private void PantherClimbTree()
        {
            _panther.ClimbTree();
            PantherSpeed = _panther.Speed;
        }

        [RelayCommand]
        private void PantherGetDown()
        {
            _panther.GetDown();
        }

        [RelayCommand]
        private void TurtleMove()
        {
            _turtle.Move();
            TurtleSpeed = _turtle.Speed;
        }

        [RelayCommand]
        private void TurtleStand()
        {
            _turtle.Stand();
            TurtleSpeed = _turtle.Speed;
        }
    }
}
