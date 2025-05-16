using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using AvaloniaApplication2.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ReflectionApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _assemblyPath;

        [ObservableProperty]
        private ObservableCollection<string> _availableClasses = new();

        [ObservableProperty]
        private string? _selectedClass;

        [ObservableProperty]
        private ObservableCollection<MethodInfo> _availableMethods = new();

        [ObservableProperty]
        private MethodInfo? _selectedMethod;

        [ObservableProperty]
        private ObservableCollection<ParameterViewModel> _methodParameters = new();

        [ObservableProperty]
        private string _statusMessage = "Выберите сборку с классами существ";

        private Assembly? _loadedAssembly;
        private object? _currentInstance;

        [RelayCommand]
        private void LoadAssembly()
        {
            if (string.IsNullOrWhiteSpace(AssemblyPath)) return;

            try
            {
                _loadedAssembly = Assembly.LoadFrom(AssemblyPath);
                var creatureTypes = _loadedAssembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(ICreature).IsAssignableFrom(t))
                    .ToList();

                AvailableClasses.Clear();
                foreach (var type in creatureTypes)
                {
                    AvailableClasses.Add(type.Name);
                }

                StatusMessage = $"Загружено классов: {creatureTypes.Count}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }
        [ObservableProperty]
        private ObservableCollection<string> _eventLogs = new();
        private void SetupCreatureEvents()
        {
            if (_currentInstance is Dog dog)
            {
                dog.SoundGiven += (sender, args) => 
                    EventLogs.Add($"[{DateTime.Now:T}] Собака {dog.GetType().Name} лает!");
            }
            else if (_currentInstance is Panther panther)
            {
                panther.SoundGiven += (sender, args) => 
                    EventLogs.Add($"[{DateTime.Now:T}] Пантера {panther.GetType().Name} рычит!");
                
                panther.TreeClimbed += (sender, args) => 
                    EventLogs.Add($"[{DateTime.Now:T}] Пантера залезла на дерево");
                
                panther.GetDownFromTree += (sender, args) => 
                    EventLogs.Add($"[{DateTime.Now:T}] Пантера слезла с дерева");
            }
        }

        [RelayCommand]
        private void SelectClass()
        {
            if (_loadedAssembly == null || string.IsNullOrWhiteSpace(SelectedClass)) 
            {
                StatusMessage = "Сборка не загружена или класс не выбран";
                return;
            }

            try
            {
                // поиск типа
                var selectedType = _loadedAssembly.GetType($"AvaloniaApplication2.Models.{SelectedClass}");
                
                if (selectedType == null)
                {
                    StatusMessage = $"Класс {SelectedClass} не найден в сборке";
                    return;
                }

                //экземпляр класса
                _currentInstance = Activator.CreateInstance(selectedType, 10, 50);
                SetupCreatureEvents();

                //получаем методы
                AvailableMethods.Clear();
                var methods = selectedType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => !m.IsSpecialName);

                foreach (var method in methods)
                {
                    AvailableMethods.Add(method);
                }

                StatusMessage = $"Выбран класс: {SelectedClass} (методов: {methods.Count()})";
                
                // Логирование для отладки
                Console.WriteLine($"Тип найден: {selectedType.FullName}");
                Console.WriteLine($"Методы: {string.Join(", ", methods.Select(m => m.Name))}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                Console.WriteLine($"Ошибка: {ex}");
            }
        }

        [RelayCommand]
        private void SelectMethod()
        {
            if (SelectedMethod == null) return;

            MethodParameters.Clear();
            foreach (var param in SelectedMethod.GetParameters())
            {
                MethodParameters.Add(new ParameterViewModel
                {
                    Name = param.Name!,
                    Type = param.ParameterType,
                    Value = GetDefaultValue(param.ParameterType)
                });
            }

            StatusMessage = $"Выбран метод: {SelectedMethod.Name}";
        }

        [RelayCommand]
        private void ExecuteMethod()
        {
            if (_currentInstance == null || SelectedMethod == null) return;

            try
            {
                //вызов метода
                SelectedMethod.Invoke(_currentInstance, null);
                
                if (_currentInstance is ICreature creature)
                {
                    StatusMessage = $"Текущая скорость: {creature.Speed}";
                    
                    //добавление записи в лог
                    if (SelectedMethod.Name == "Move" || SelectedMethod.Name == "Stand")
                    {
                        EventLogs.Add($"[{DateTime.Now:T}] {SelectedMethod.Name} → Скорость: {creature.Speed}");
                    }
                    else
                    {
                        EventLogs.Add($"[{DateTime.Now:T}] Вызван метод: {SelectedMethod.Name}");
                    }
                }
                else
                {
                    EventLogs.Add($"[{DateTime.Now:T}] Вызван метод: {SelectedMethod.Name}");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.InnerException?.Message ?? ex.Message}";
                EventLogs.Add($"[{DateTime.Now:T}] Ошибка: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private static object? GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }

    public class ParameterViewModel
    {
        public string Name { get; set; } = "";
        public Type Type { get; set; } = typeof(object);
        public object? Value { get; set; }
    }
}