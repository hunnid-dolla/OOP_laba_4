using System;
using System.Drawing;

namespace OOP_laba4
{
    public class CircleShape : Shape
    {
        public CircleShape(Rectangle bounds, Color color) : base(bounds, color) // Конструктор, принимающий прямоугольник и цвет
        {
            int diameter = Math.Max(bounds.Width, bounds.Height); // Вычисление диаметра круга, выбираем большее из ширины и высоты
            diameter = Math.Max(diameter, MinimumSize.Width); // Убедимся, что диаметр не меньше минимального размера
            Bounds = new Rectangle(bounds.Location, new Size(diameter, diameter)); // Устанавливаем новый прямоугольник с одинаковыми шириной и высотой
        }

        public override void Draw(Graphics g) // Метод рисования круга
        {
            Color outlineColor = IsSelected ? Color.Red : Color.Black; // Определение цвета обводки, если выбран — красный, иначе черный
            float outlineWidth = IsSelected ? 2f : 1f; // Определение ширины обводки, если выбран — 2 пикселя, иначе 1 пиксель

            using (var brush = new SolidBrush(this.Color)) // Создание кисти для заливки круга
            using (var outlinePen = new Pen(outlineColor, outlineWidth)) // Создание пера для обводки круга
            {
                g.FillEllipse(brush, Bounds); // Заполнение круга цветом
                g.DrawEllipse(outlinePen, Bounds); // Рисование обводки круга
            }
        }

        public override bool Contains(Point point) // Метод для проверки, находится ли точка внутри круга
        {
            Point center = new Point(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2); // Определение центра круга
            double radiusX = Bounds.Width / 2.0; // Радиус по оси X
            double radiusY = Bounds.Height / 2.0; // Радиус по оси Y
            if (radiusX <= 0 || radiusY <= 0) return false; // Если радиус нулевой или отрицательный, точка не может быть внутри круга
            double dx = point.X - center.X; // Разница по оси X между точкой и центром круга
            double dy = point.Y - center.Y; // Разница по оси Y между точкой и центром круга
            return (dx * dx) / (radiusX * radiusX) + (dy * dy) / (radiusY * radiusY) <= 1; // Проверка, лежит ли точка внутри круга по уравнению эллипса
        }

        public override void Resize(int delta, Rectangle clientBounds) // Метод изменения размера круга
        {
            int newDiameter = Math.Max(MinimumSize.Width, this.Bounds.Width + delta); // Увеличиваем диаметр на delta, но не меньше минимального размера
            newDiameter = Math.Min(newDiameter, clientBounds.Right - this.Bounds.Left); // Ограничиваем новый диаметр правой границей клиентской области
            newDiameter = Math.Min(newDiameter, clientBounds.Bottom - this.Bounds.Top); // Ограничиваем новый диаметр нижней границей клиентской области
            newDiameter = Math.Max(MinimumSize.Width, newDiameter); // Убедимся, что новый диаметр не меньше минимального размера
            if (newDiameter != this.Bounds.Width) // Если новый диаметр отличается от текущего, обновляем границы
            {
                this.Bounds = new Rectangle(this.Location, new Size(newDiameter, newDiameter)); // Устанавливаем новые границы для круга
            }
        }
    }
}