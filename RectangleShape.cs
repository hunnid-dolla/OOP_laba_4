using System.Drawing;

namespace OOP_laba_4
{
    public class RectangleShape : Shape
    {
        public RectangleShape(Rectangle bounds, Color color) : base(bounds, color) { } // Конструктор, вызывающий конструктор базового класса Shape и устанавливающий границы и цвет

        public override void Draw(Graphics g) // Переопределение метода для отрисовки прямоугольника
        {
            Color outlineColor = IsSelected ? Color.Red : Color.Black; // Если фигура выбрана, контур красный, иначе черный
            float outlineWidth = IsSelected ? 2f : 1f; // Толщина контура увеличена при выборе

            using (var brush = new SolidBrush(this.Color)) // Кисть для заполнения прямоугольника основным цветом
            using (var outlinePen = new Pen(outlineColor, outlineWidth)) // Перо для обводки контура
            {
                g.FillRectangle(brush, Bounds); // Заполнение прямоугольника цветом
                g.DrawRectangle(outlinePen, Bounds); // Обводка контура прямоугольника
            }
        }

        public override bool Contains(Point point) // Переопределение метода проверки, содержится ли точка внутри фигуры
        {
            return Bounds.Contains(point); // Возвращает true, если точка находится внутри ограничивающего прямоугольника
        }
    }
}
