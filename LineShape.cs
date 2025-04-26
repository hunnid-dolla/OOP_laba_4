using System;
using System.Drawing;

namespace OOP_laba4
{
    public class LineShape : Shape
    {
        public Point StartPoint { get; protected set; } // Свойство: начальная точка линии с защищенным сеттером
        public Point EndPoint { get; protected set; } // Свойство: конечная точка линии с защищенным сеттером

        private const float SelectionTolerance = 4.0f; // Константа: допустимое отклонение при выборе линии
        private const float MinLength = 2.0f; // Константа: минимальная длина линии при изменении размера

        public LineShape(Point start, Point end, Color color) // Конструктор LineShape
            : base(CalculateBounds(start, end), color) // Вызов конструктора базового класса Shape с рассчитанными границами и цветом
        {
            this.StartPoint = start; // Установка начальной точки
            this.EndPoint = end; // Установка конечной точки
            this.Bounds = CalculateBounds(this.StartPoint, this.EndPoint); // Обновление границ линии
        }

        private static Rectangle CalculateBounds(Point start, Point end) // Метод для вычисления ограничивающего прямоугольника
        {
            int minX = Math.Min(start.X, end.X); // Вычисление минимальной X-координаты
            int minY = Math.Min(start.Y, end.Y); // Вычисление минимальной Y-координаты
            int maxX = Math.Max(start.X, end.X); // Вычисление максимальной X-координаты
            int maxY = Math.Max(start.Y, end.Y); // Вычисление максимальной Y-координаты

            int width = Math.Max(1, maxX - minX); // Вычисление ширины прямоугольника (не менее 1)
            int height = Math.Max(1, maxY - minY); // Вычисление высоты прямоугольника (не менее 1)

            return new Rectangle(minX, minY, width, height); // Возвращение прямоугольника
        }

        private void UpdateBounds() // Вспомогательный метод для обновления свойства Bounds
        {
            this.Bounds = CalculateBounds(this.StartPoint, this.EndPoint); // Пересчет и установка прямоугольника
        }

        public override void Draw(Graphics g) // Переопределение метода отрисовки фигуры
        {
            Color lineColor = IsSelected ? Color.Red : this.Color; // Если линия выбрана, цвет красный, иначе заданный
            float lineWidth = IsSelected ? 2.5f : 2f; // Если выбрана, ширина пера больше

            using (var linePen = new Pen(lineColor, lineWidth)) // Создание пера для рисования
            {
                g.DrawLine(linePen, StartPoint, EndPoint); // Отрисовка линии между двумя точками
            }
        }

        public override bool Contains(Point point) // Переопределение метода проверки, содержит ли фигура точку
        {
            float distance = DistancePointSegment(point, StartPoint, EndPoint); // Вычисление расстояния от точки до сегмента
            return distance <= SelectionTolerance; // Если расстояние меньше порога, точка считается внутри
        }

        private float DistancePointSegment(Point p, Point a, Point b) // Метод для вычисления расстояния от точки до отрезка
        {
            float l2 = DistSq(a, b); // Квадрат длины отрезка
            if (l2 == 0.0) return Dist(p, a); // Если отрезок вырожден в точку, вернуть расстояние до этой точки

            float t = ((p.X - a.X) * (b.X - a.X) + (p.Y - a.Y) * (b.Y - a.Y)) / l2; // Проекция точки на прямую
            t = Math.Max(0, Math.Min(1, t)); // Ограничение проекции между 0 и 1

            Point projection = new Point((int)Math.Round(a.X + t * (b.X - a.X)), (int)Math.Round(a.Y + t * (b.Y - a.Y))); // Вычисление координат проекции

            return Dist(p, projection); // Возвращение расстояния от точки до проекции
        }

        private float Dist(Point p1, Point p2) // Метод для вычисления евклидова расстояния между двумя точками
        {
            float dx = p1.X - p2.X; // Разница по X
            float dy = p1.Y - p2.Y; // Разница по Y
            return (float)Math.Sqrt(dx * dx + dy * dy); // Вычисление корня из суммы квадратов
        }

        private float DistSq(Point p1, Point p2) // Метод для вычисления квадрата евклидова расстояния
        {
            float dx = p1.X - p2.X; // Разница по X
            float dy = p1.Y - p2.Y; // Разница по Y
            return dx * dx + dy * dy; // Сумма квадратов
        }

        public override void Move(int dx, int dy, Rectangle clientBounds) // Переопределение метода перемещения фигуры
        {
            Point newStart = new Point(StartPoint.X + dx, StartPoint.Y + dy); // Новая начальная точка с учетом смещения
            Point newEnd = new Point(EndPoint.X + dx, EndPoint.Y + dy); // Новая конечная точка с учетом смещения
            SetPointsInternal(newStart, newEnd); // Внутренний метод установки точек и обновления границ
        }

        protected internal override void SetLocationInternal(Point newTopLeftLocation) // Переопределение метода установки местоположения через верхний левый угол
        {
            int dx = newTopLeftLocation.X - this.Bounds.Left; // Смещение по X
            int dy = newTopLeftLocation.Y - this.Bounds.Top; // Смещение по Y

            if (dx != 0 || dy != 0) // Если есть смещение
            {
                Point newStart = new Point(StartPoint.X + dx, StartPoint.Y + dy); // Вычисление новой начальной точки
                Point newEnd = new Point(EndPoint.X + dx, EndPoint.Y + dy); // Вычисление новой конечной точки
                SetPointsInternal(newStart, newEnd); // Установка новых точек
            }
        }

        private void SetPointsInternal(Point newStart, Point newEnd) // Внутренний метод для обновления точек и границ
        {
            this.StartPoint = newStart; // Установка новой начальной точки
            this.EndPoint = newEnd; // Установка новой конечной точки
            UpdateBounds(); // Обновление ограничивающего прямоугольника
        }

        public override void Resize(int delta, Rectangle clientBounds) // Переопределение метода изменения размера фигуры
        {
            if (delta == 0) return; // Если изменение размера нулевое, ничего не делать

            Point currentStart = this.StartPoint; // Текущая начальная точка
            Point currentEnd = this.EndPoint; // Текущая конечная точка

            float midX = (currentStart.X + currentEnd.X) / 2.0f; // X-координата середины отрезка
            float midY = (currentStart.Y + currentEnd.Y) / 2.0f; // Y-координата середины отрезка

            float currentLength = Dist(currentStart, currentEnd); // Текущая длина отрезка

            if (currentLength < 0.001f) // Если длина слишком мала, выходим
            {
                return; // Нечего масштабировать
            }

            float newLength = Math.Max(MinLength, currentLength + delta); // Новая длина с учетом минимальной границы
            float scaleFactor = newLength / currentLength; // Коэффициент масштабирования

            float vecX = currentEnd.X - midX; // Вектор от середины к конечной точке по X
            float vecY = currentEnd.Y - midY; // Вектор от середины к конечной точке по Y

            float newEndX = midX + vecX * scaleFactor; // Новая X-координата конечной точки
            float newEndY = midY + vecY * scaleFactor; // Новая Y-координата конечной точки
            float newStartX = midX - vecX * scaleFactor; // Новая X-координата начальной точки
            float newStartY = midY - vecY * scaleFactor; // Новая Y-координата начальной точки

            Point potentialStart = new Point((int)Math.Round(newStartX), (int)Math.Round(newStartY)); // Округление и создание новой начальной точки
            Point potentialEnd = new Point((int)Math.Round(newEndX), (int)Math.Round(newEndY)); // Округление и создание новой конечной точки

            Rectangle potentialBounds = CalculateBounds(potentialStart, potentialEnd); // Расчет новых границ

            Rectangle safeClientBounds = clientBounds; // Клиентская область
            safeClientBounds.Inflate(-1, -1); // Сужение клиентской области на 1 пиксель с каждой стороны

            if (safeClientBounds.Contains(potentialBounds)) // Если новые границы помещаются в клиентскую область
            {
                SetPointsInternal(potentialStart, potentialEnd); // Применяем обновленные точки
            }
        }
    }
}
