using System;
using System.Drawing;

namespace OOP_laba4
{
    public class SquareShape : Shape
    {
        public SquareShape(Rectangle bounds, Color color) : base(bounds, color) // Конструктор класса, вызывающий конструктор базового класса
        {
            int side = Math.Max(bounds.Width, bounds.Height); // Определение стороны квадрата как максимальной из ширины и высоты
            side = Math.Max(side, MinimumSize.Width); // Обеспечение минимального размера квадрата
            Bounds = new Rectangle(bounds.Location, new Size(side, side)); // Установка ограничивающего прямоугольника с равными сторонами
        }

        public override void Draw(Graphics g) // Переопределение метода отрисовки фигуры
        {
            Color outlineColor = IsSelected ? Color.Red : Color.Black; // Цвет контура: красный, если выбран, иначе черный
            float outlineWidth = IsSelected ? 2f : 1f; // Ширина контура: увеличивается при выборе

            using (var brush = new SolidBrush(this.Color)) // Кисть для заполнения квадрата
            using (var outlinePen = new Pen(outlineColor, outlineWidth)) // Перо для обводки контура
            {
                g.FillRectangle(brush, Bounds); // Заполнение квадрата заданным цветом
                g.DrawRectangle(outlinePen, Bounds); // Обводка контура квадрата
            }
        }

        public override bool Contains(Point point) // Переопределение метода проверки, содержится ли точка внутри фигуры
        {
            return Bounds.Contains(point); // Возвращает true, если точка внутри ограничивающего прямоугольника
        }

        public override void Resize(int delta, Rectangle clientBounds) // Переопределение метода изменения размера фигуры
        {
            int newSide = Math.Max(MinimumSize.Width, this.Bounds.Width + delta); // Новый размер стороны с учетом изменения и минимального размера
            newSide = Math.Min(newSide, clientBounds.Right - this.Bounds.Left); // Ограничение по правому краю клиентской области
            newSide = Math.Min(newSide, clientBounds.Bottom - this.Bounds.Top); // Ограничение по нижнему краю клиентской области
            newSide = Math.Max(MinimumSize.Width, newSide); // Повторная проверка минимального размера

            if (newSide != this.Bounds.Width) // Если размер изменился
            {
                this.Bounds = new Rectangle(this.Location, new Size(newSide, newSide)); // Установка новых границ с равной шириной и высотой
            }
        }
    }
}
