using System.Drawing;

namespace OOP_laba_4
{
    public class TriangleShape : Shape
    {
        public bool IsPointingUp { get; set; } // Свойство, определяющее направление вершины треугольника: вверх или вниз

        public TriangleShape(Rectangle bounds, Color color) : this(bounds, color, true) // Конструктор по умолчанию: указывает вершину вверх
        {
        }

        public TriangleShape(Rectangle bounds, Color color, bool pointsUp) : base(bounds, color) // Основной конструктор, вызывающий базовый конструктор Shape
        {
            this.IsPointingUp = pointsUp; // Установка направления треугольника
        }

        private Point[] GetTriangleVertices() // Метод для вычисления трёх вершин треугольника
        {
            Point apex; // Вершина треугольника
            Point baseLeft; // Левый угол основания
            Point baseRight; // Правый угол основания

            if (IsPointingUp) // Если треугольник обращён вершиной вверх
            {
                apex = new Point(Bounds.Left + Bounds.Width / 2, Bounds.Top); // Вершина посередине сверху
                baseLeft = new Point(Bounds.Left, Bounds.Bottom); // Левый край основания снизу
                baseRight = new Point(Bounds.Right, Bounds.Bottom); // Правый край основания снизу
            }
            else // Если треугольник обращён вершиной вниз
            {
                apex = new Point(Bounds.Left + Bounds.Width / 2, Bounds.Bottom); // Вершина посередине снизу
                baseLeft = new Point(Bounds.Left, Bounds.Top); // Левый край основания сверху
                baseRight = new Point(Bounds.Right, Bounds.Top); // Правый край основания сверху
            }

            return new Point[] { apex, baseLeft, baseRight }; // Возвращение массива точек-вершин
        }

        public override void Draw(Graphics g) // Переопределение метода отрисовки фигуры
        {
            Point[] vertices = GetTriangleVertices(); // Получение вершин треугольника

            Color outlineColor = IsSelected ? Color.Red : Color.Black; // Цвет контура: красный при выборе, иначе чёрный
            float outlineWidth = IsSelected ? 2f : 1f; // Ширина контура: больше при выборе

            using (var brush = new SolidBrush(this.Color)) // Кисть для заполнения треугольника выбранным цветом
            using (var outlinePen = new Pen(outlineColor, outlineWidth)) // Перо для обводки контура
            {
                g.FillPolygon(brush, vertices); // Заполнение треугольника
                g.DrawPolygon(outlinePen, vertices); // Обводка контура треугольника
            }
        }

        public override bool Contains(Point point) // Переопределение метода проверки попадания точки внутрь фигуры
        {
            return Bounds.Contains(point); // Упрощённая проверка: если точка в ограничивающем прямоугольнике, считается внутри
        }
    }
}
