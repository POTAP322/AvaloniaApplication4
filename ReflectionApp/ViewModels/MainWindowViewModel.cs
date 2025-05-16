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

        [RelayCommand]
        private void SelectClass()
        {
            if (_loadedAssembly == null || string.IsNullOrWhiteSpace(SelectedClass)) return;

            var selectedType = _loadedAssembly.GetType($"{_loadedAssembly.GetName().Name}.{SelectedClass}");
            if (selectedType == null) return;

            // Создаем экземпляр существа (speedStep=10, maxSpeed=50)
            _currentInstance = Activator.CreateInstance(selectedType, 10, 50);

            // Получаем методы
            AvailableMethods.Clear();
            var methods = selectedType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.IsSpecialName); // Исключаем свойства/события

            foreach (var method in methods)
            {
                AvailableMethods.Add(method);
            }

            StatusMessage = $"Выбран класс: {SelectedClass} (доступно методов: {methods.Count()})";
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
                var parameters = MethodParameters
                    .Select(p => Convert.ChangeType(p.Value, p.Type))
                    .ToArray();

                var result = SelectedMethod.Invoke(_currentInstance, parameters);
                StatusMessage = $"Метод выполнен! Результат: {result ?? "null"}";

                // Обновляем скорость, если это существо
                if (_currentInstance is ICreature creature)
                {
                    StatusMessage += $"\nТекущая скорость: {creature.Speed}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.InnerException?.Message ?? ex.Message}";
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