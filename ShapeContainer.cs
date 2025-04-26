using System.Collections.Generic;
using System.Linq;

namespace OOP_laba_4
{
    public class ShapeContainer
    {
        private readonly List<Shape> _shapes = new List<Shape>(); // Инициализация приватного списка для хранения объектов Shape

        public void Add(Shape shape) // Метод для добавления нового объекта Shape в контейнер
        {
            if (shape != null) // Проверка на null, чтобы избежать добавления пустого объекта
            {
                _shapes.Add(shape); // Добавление объекта в список
            }
        }

        public void Remove(Shape shape) // Метод для удаления объекта Shape из контейнера
        {
            _shapes.Remove(shape); // Удаление объекта из списка
        }

        public IEnumerable<Shape> GetAll() // Метод для получения всех объектов Shape из контейнера
        {
            return _shapes; // Возвращаем все объекты в контейнере
        }

        public IEnumerable<Shape> GetSelected() // Метод для получения всех выбранных объектов Shape
        {
            return _shapes.Where(s => s.IsSelected); // Используем LINQ для фильтрации объектов, у которых свойство IsSelected равно true
        }

        public void ClearSelection() // Метод для снятия выделения со всех объектов
        {
            foreach (var shape in _shapes) // Проходим по всем объектам в списке
            {
                shape.IsSelected = false; // Устанавливаем свойство IsSelected в false для каждого объекта
            }
        }

        public void SelectAll() // Метод для выделения всех объектов в контейнере
        {
            foreach (var shape in _shapes) // Проходим по всем объектам в списке
            {
                shape.IsSelected = true; // Устанавливаем свойство IsSelected в true для каждого объекта
            }
        }

        public void RemoveSelected() // Метод для удаления всех выбранных объектов из контейнера
        {
            var selected = GetSelected().ToList(); // Получаем список выбранных объектов и преобразуем его в список
            foreach (var shape in selected) // Проходим по всем выбранным объектам
            {
                Remove(shape); // Удаляем объект из контейнера
            }
        }
    }
}