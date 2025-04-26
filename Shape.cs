using System;
using System.Drawing;

namespace OOP_laba4
{
    public abstract class Shape
    {
        public static readonly Size MinimumSize = new Size(10, 10); // Константа, которая задает минимальный размер для всех форм (10x10 пикселей)

        public Rectangle Bounds { get; protected set; } // Свойство для хранения размеров и положения фигуры (Bounds — ограничивающий прямоугольник)
        public Color Color { get; set; } // Свойство для хранения цвета фигуры
        public bool IsSelected { get; set; } // Свойство для проверки, выбрана ли фигура
        public Point Location => Bounds.Location; // Свойство для получения координат левого верхнего угла фигуры (из Bounds)
        public Size Size => Bounds.Size; // Свойство для получения размера фигуры (ширина и высота из Bounds)

        protected Shape(Rectangle bounds, Color color) // Конструктор для создания фигуры с заданным прямоугольником (Bounds) и цветом
        {
            int initialWidth = Math.Max(MinimumSize.Width, bounds.Width); // Устанавливаем ширину фигуры, которая не может быть меньше минимальной ширины
            int initialHeight = Math.Max(MinimumSize.Height, bounds.Height); // Устанавливаем высоту фигуры, которая не может быть меньше минимальной высоты
            Bounds = new Rectangle(bounds.Location, new Size(initialWidth, initialHeight)); // Инициализация Bounds с расчетом размеров
            Color = color; // Устанавливаем цвет фигуры
            IsSelected = false; // Изначально фигура не выбрана
        }

        public abstract void Draw(Graphics g); // Абстрактный метод для рисования фигуры на графическом контексте, должен быть реализован в наследниках
        public abstract bool Contains(Point point); // Абстрактный метод для проверки, содержится ли точка внутри фигуры, должен быть реализован в наследниках

        public virtual void Move(int dx, int dy, Rectangle clientBounds) // Метод для перемещения фигуры на dx и dy с учетом ограничений клиентской области
        {
            int potentialX = Bounds.Left + dx; // Потенциальная новая координата X после перемещения
            int potentialY = Bounds.Top + dy; // Потенциальная новая координата Y после перемещения
            // Ограничиваем новые координаты фигуры, чтобы она оставалась в пределах клиентской области
            int newX = Math.Max(clientBounds.Left, Math.Min(potentialX, clientBounds.Right - Bounds.Width));
            int newY = Math.Max(clientBounds.Top, Math.Min(potentialY, clientBounds.Bottom - Bounds.Height));
            // Если новые координаты отличаются от текущих, обновляем местоположение фигуры
            if (newX != Bounds.Left || newY != Bounds.Top)
            {
                SetLocationInternal(new Point(newX, newY)); // Устанавливаем новое местоположение фигуры
            }
        }

        public virtual void Resize(int delta, Rectangle clientBounds) // Метод для изменения размера фигуры с учетом клиентской области
        {
            int newWidth = Math.Max(MinimumSize.Width, Bounds.Width + delta); // Новый расчет ширины фигуры, не меньше минимальной
            int newHeight = Math.Max(MinimumSize.Height, Bounds.Height + delta); // Новый расчет высоты фигуры, не меньше минимальной
            newWidth = Math.Min(newWidth, clientBounds.Right - Bounds.Left); // Ограничиваем ширину, чтобы она не выходила за правую границу клиентской области
            newHeight = Math.Min(newHeight, clientBounds.Bottom - Bounds.Top); // Ограничиваем высоту, чтобы она не выходила за нижнюю границу клиентской области
            newWidth = Math.Max(MinimumSize.Width, newWidth); // Убедимся, что новая ширина не меньше минимальной
            newHeight = Math.Max(MinimumSize.Height, newHeight); // Убедимся, что новая высота не меньше минимальной
            // Если размеры изменились, обновляем Bounds
            if (newWidth != Bounds.Width || newHeight != Bounds.Height)
            {
                Bounds = new Rectangle(Bounds.Left, Bounds.Top, newWidth, newHeight); // Обновляем Bounds с новыми размерами
            }
        }

        protected virtual internal void SetLocationInternal(Point newLocation) // Защищенный метод для обновления местоположения фигуры
        {
            this.Bounds = new Rectangle(newLocation, this.Size); // Обновляем местоположение, но сохраняем текущие размеры
        }
    }
}