using System.Drawing;

namespace OOP_laba_4
{
    public class EllipseShape : Shape
    {
        public EllipseShape(Rectangle bounds, Color color) : base(bounds, color) { } // Конструктор, вызывающий базовый конструктор Shape

        public override void Draw(Graphics g) // Переопределение метода отрисовки фигуры
        {
            Color outlineColor = IsSelected ? Color.Red : Color.Black; // Выбор цвета обводки: красный при выборе, иначе черный
            float outlineWidth = IsSelected ? 2f : 1f; // Выбор толщины контура: толще при выборе

            using (var brush = new SolidBrush(this.Color)) // Кисть для заполнения эллипса основным цветом
            using (var outlinePen = new Pen(outlineColor, outlineWidth)) // Перо для обводки эллипса
            {
                g.FillEllipse(brush, Bounds); // Заполнение области эллипса
                g.DrawEllipse(outlinePen, Bounds); // Обводка контура эллипса
            }
        }

        public override bool Contains(Point point) // Переопределение метода проверки попадания точки внутрь фигуры
        {
            Point center = new Point(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2); // Вычисление центра эллипса
            double radiusX = Bounds.Width / 2.0; // Горизонтальный радиус эллипса
            double radiusY = Bounds.Height / 2.0; // Вертикальный радиус эллипса
            if (radiusX <= 0 || radiusY <= 0) return false; // Если радиусы некорректны, точка не внутри
            double dx = point.X - center.X; // Смещение точки по X от центра
            double dy = point.Y - center.Y; // Смещение точки по Y от центра
            // Использование уравнения эллипса: (dx^2 / rx^2) + (dy^2 / ry^2) <= 1 означает внутри
            return (dx * dx) / (radiusX * radiusX) + (dy * dy) / (radiusY * radiusY) <= 1;
        }
    }
}
