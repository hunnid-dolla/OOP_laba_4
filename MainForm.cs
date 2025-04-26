using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OOP_laba_4
{
    public partial class MainForm : Form
    {
        private readonly ShapeContainer _container = new ShapeContainer(); // Контейнер для хранения фигур
        private bool _isCtrlPressed; // Флаг для отслеживания нажатия клавиши Ctrl
        private enum InteractionMode // Перечисление режимов взаимодействия пользователя
        {
            Idle, // Нет активного действия
            Selecting, // Режим выбора фигур
            Creating, // Режим создания новой фигуры
            Dragging // Режим перетаскивания фигур
        }
        private InteractionMode _mode = InteractionMode.Selecting; // Текущий режим взаимодействия
        private Type _shapeTypeToCreate = null; // Тип фигуры для создания
        private Point _creationStartPoint; // Точка начала создания фигуры
        private Point _dragStartPoint; // Точка начала перетаскивания
        private Shape _previewShape = null; // Предварительный просмотр создаваемой фигуры
        private static readonly Color DefaultShapeColor = Color.CornflowerBlue; // Цвет по умолчанию для новых фигур
        private const int MinDragThresholdSquared = 25; // Минимальное расстояние для начала перетаскивания
        private ToolStripButton btnSelect; // Кнопка выбора режима "Выбрать"
        private ToolStripDropDownButton btnShapes; // Кнопка выбора типа фигуры

        public MainForm() // Конструктор формы
        {
            InitializeComponent(); // Инициализация компонентов формы
            this.DoubleBuffered = true; // Включение двойной буферизации для уменьшения мерцания
            this.Paint += MainForm_Paint; // Привязка обработчика события рисования
            this.MouseDown += MainForm_MouseDown; // Привязка обработчика нажатия мыши
            this.MouseMove += MainForm_MouseMove; // Привязка обработчика движения мыши
            this.MouseUp += MainForm_MouseUp; // Привязка обработчика отпускания кнопки мыши
            this.KeyDown += MainForm_KeyDown; // Привязка обработчика нажатия клавиш
            this.KeyUp += MainForm_KeyUp; // Привязка обработчика отпускания клавиш
            this.Resize += MainForm_Resize; // Привязка обработчика изменения размера формы
            SetupToolbar(); // Настройка панели инструментов
        }

        private void SetupToolbar() // Метод для настройки панели инструментов
        {
            ToolStrip toolStrip = new ToolStrip(); // Создание новой панели инструментов
            this.Controls.Add(toolStrip); // Добавление панели на форму
            btnSelect = new ToolStripButton("Выбрать"); // Создание кнопки "Выбрать"
            btnSelect.ToolTipText = "Выбрать и перемещать фигуры"; // Подсказка для кнопки
            btnSelect.CheckOnClick = true; // Кнопка может быть "нажата" (выбрана)
            btnSelect.Click += SelectTool_Click; // Привязка обработчика клика
            toolStrip.Items.Add(btnSelect); // Добавление кнопки на панель
            btnShapes = new ToolStripDropDownButton("Фигуры"); // Создание выпадающей кнопки "Фигуры"
            btnShapes.ToolTipText = "Выберите фигуру для рисования"; // Подсказка для кнопки
            toolStrip.Items.Add(btnShapes); // Добавление кнопки на панель
            CreateShapeMenuItem("Круг", typeof(CircleShape)); // Добавление пункта меню для круга
            CreateShapeMenuItem("Прямоугольник", typeof(RectangleShape)); // Добавление пункта меню для прямоугольника
            CreateShapeMenuItem("Эллипс", typeof(EllipseShape)); // Добавление пункта меню для эллипса
            CreateShapeMenuItem("Квадрат", typeof(SquareShape)); // Добавление пункта меню для квадрата
            CreateShapeMenuItem("Треугольник", typeof(TriangleShape)); // Добавление пункта меню для треугольника
            CreateShapeMenuItem("Линия", typeof(LineShape)); // Добавление пункта меню для линии
            ToolStripButton btnColor = new ToolStripButton("Цвет"); // Создание кнопки изменения цвета
            btnColor.ToolTipText = "Изменить цвет выбранных фигур"; // Подсказка для кнопки
            btnColor.Click += ChangeSelectedColor_Click; // Привязка обработчика клика
            toolStrip.Items.Add(btnColor); // Добавление кнопки на панель
            btnSelect.Checked = true; // Установка кнопки "Выбрать" в активное состояние
            SetMode(InteractionMode.Selecting); // Установка режима выбора
        }

        private void CreateShapeMenuItem(string shapeName, Type shapeType) // Метод для создания пункта меню фигуры
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(shapeName); // Создание нового пункта меню
            menuItem.Click += (sender, e) => ShapeMenuItem_Click(shapeName, shapeType); // Привязка обработчика клика
            btnShapes.DropDownItems.Add(menuItem); // Добавление пункта в выпадающее меню
        }

        private void ShapeMenuItem_Click(string shapeName, Type shapeType) // Обработчик клика по пункту меню фигуры
        {
            SetMode(InteractionMode.Creating); // Установка режима создания
            _shapeTypeToCreate = shapeType; // Сохранение типа фигуры для создания
            btnShapes.Text = shapeName; // Изменение текста кнопки "Фигуры"
            btnSelect.Checked = false; // Отключение кнопки "Выбрать"
        }

        private void SelectTool_Click(object sender, EventArgs e) // Обработчик клика по кнопке "Выбрать"
        {
            if (btnSelect.Checked) // Если кнопка уже выбрана
            {
                SetMode(InteractionMode.Selecting); // Установка режима выбора
                _shapeTypeToCreate = null; // Сброс типа фигуры для создания
                btnShapes.Text = "Фигуры"; // Восстановление текста кнопки "Фигуры"
            }
            else // Если кнопка не выбрана
            {
                btnSelect.Checked = true; // Выбор кнопки
                SetMode(InteractionMode.Selecting); // Установка режима выбора
                _shapeTypeToCreate = null; // Сброс типа фигуры для создания
                btnShapes.Text = "Фигуры"; // Восстановление текста кнопки "Фигуры"
            }
        }

        private void SetMode(InteractionMode newMode) // Метод для установки режима взаимодействия
        {
            _mode = newMode; // Установка нового режима
            if (newMode != InteractionMode.Creating && _previewShape != null) // Если режим не "Создание"
            {
                _previewShape = null; // Удаление предварительного просмотра
                Invalidate(); // Перерисовка формы
            }
            if (newMode == InteractionMode.Creating) // Если режим "Создание"
            {
                this.Cursor = Cursors.Cross; // Установка курсора в виде креста
            }
            else if (newMode == InteractionMode.Dragging) // Если режим "Перетаскивание"
            {
                this.Cursor = Cursors.SizeAll; // Установка курсора для перетаскивания
            }
            else // Для других режимов
            {
                this.Cursor = Cursors.Default; // Установка стандартного курсора
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e) // Обработчик события рисования формы
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Включение сглаживания
            foreach (var shape in _container.GetAll()) // Перебор всех фигур в контейнере
            {
                shape.Draw(e.Graphics); // Рисование каждой фигуры
            }
            _previewShape?.Draw(e.Graphics); // Рисование предварительного просмотра, если он существует
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e) // Обработчик нажатия мыши
        {
            if (e.Button == MouseButtons.Left) // Если нажата левая кнопка мыши
            {
                if (_mode == InteractionMode.Creating && _shapeTypeToCreate != null) // Если режим "Создание"
                {
                    _creationStartPoint = e.Location; // Сохранение начальной точки создания
                    Rectangle initialBounds = new Rectangle(_creationStartPoint, Shape.MinimumSize); // Создание начальных границ
                    try
                    {
                        if (_shapeTypeToCreate == typeof(LineShape)) // Если создаётся линия
                        {
                            _previewShape = new LineShape(_creationStartPoint, _creationStartPoint, Color.LightGray); // Создание предварительного просмотра линии
                        }
                        else if (_shapeTypeToCreate == typeof(TriangleShape)) // Если создаётся треугольник
                        {
                            _previewShape = (Shape)Activator.CreateInstance(_shapeTypeToCreate, initialBounds, Color.LightGray, true); // Создание предварительного просмотра треугольника
                        }
                        else // Для остальных фигур
                        {
                            _previewShape = (Shape)Activator.CreateInstance(_shapeTypeToCreate, initialBounds, Color.LightGray); // Создание предварительного просмотра
                        }
                        _previewShape.IsSelected = true; // Выделение предварительного просмотра
                        Invalidate(); // Перерисовка формы
                    }
                    catch (Exception ex) // Обработка ошибок
                    {
                        Console.WriteLine($"Ошибка при создании предварительного просмотра фигуры: {ex.Message}"); // Вывод ошибки в консоль
                        MessageBox.Show($"Ошибка при начале создания фигуры: {ex.Message}", "Ошибка создания", MessageBoxButtons.OK, MessageBoxIcon.Error); // Показ сообщения об ошибке
                        SetMode(InteractionMode.Selecting); // Возврат в режим выбора
                        btnSelect.Checked = true; // Выбор кнопки "Выбрать"
                        btnShapes.Text = "Фигуры"; // Восстановление текста кнопки "Фигуры"
                        return; // Выход из метода
                    }
                }
                else // Если режим не "Создание"
                {
                    Shape topClickedShape = _container.GetAll() // Поиск верхней фигуры под курсором
                                             .Where(shape => shape.Contains(e.Location))
                                             .LastOrDefault();
                    if (topClickedShape != null) // Если найдена фигура
                    {
                        if (!_isCtrlPressed) // Если клавиша Ctrl не нажата
                        {
                            if (!topClickedShape.IsSelected) // Если фигура не выделена
                            {
                                _container.ClearSelection(); // Снятие выделения со всех фигур
                                topClickedShape.IsSelected = true; // Выделение текущей фигуры
                            }
                        }
                        else // Если клавиша Ctrl нажата
                        {
                            topClickedShape.IsSelected = !topClickedShape.IsSelected; // Инвертирование выделения
                        }
                        if (topClickedShape.IsSelected) // Если фигура выделена
                        {
                            _mode = InteractionMode.Dragging; // Установка режима перетаскивания
                            _dragStartPoint = e.Location; // Сохранение начальной точки перетаскивания
                            this.Cursor = Cursors.SizeAll; // Установка курсора для перетаскивания
                        }
                        else // Если фигура не выделена
                        {
                            SetMode(InteractionMode.Selecting); // Возврат в режим выбора
                        }
                    }
                    else // Если фигура не найдена
                    {
                        if (!_isCtrlPressed) // Если клавиша Ctrl не нажата
                        {
                            _container.ClearSelection(); // Снятие выделения со всех фигур
                        }
                        if (_mode != InteractionMode.Selecting) SetMode(InteractionMode.Selecting); // Возврат в режим выбора
                    }
                    Invalidate(); // Перерисовка формы
                }
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e) // Обработчик движения мыши
        {
            if (_mode == InteractionMode.Dragging && e.Button == MouseButtons.Left) // Если режим "Перетаскивание"
            {
                var selectedShapes = _container.GetSelected().ToList(); // Получение списка выделенных фигур
                if (!selectedShapes.Any()) // Если нет выделенных фигур
                {
                    SetMode(InteractionMode.Selecting); // Возврат в режим выбора
                    return; // Выход из метода
                }
                int ideal_dx = e.X - _dragStartPoint.X; // Расчёт идеального смещения по X
                int ideal_dy = e.Y - _dragStartPoint.Y; // Расчёт идеального смещения по Y
                if (ideal_dx != 0 || ideal_dy != 0) // Если есть смещение
                {
                    (int actual_dx, int actual_dy) = CalculateAllowedGroupMovement(selectedShapes, ideal_dx, ideal_dy); // Расчёт допустимого смещения
                    if (actual_dx != 0 || actual_dy != 0) // Если есть допустимое смещение
                    {
                        foreach (var shape in selectedShapes) // Перемещение каждой выделенной фигуры
                        {
                            shape.SetLocationInternal(new Point(shape.Location.X + actual_dx, shape.Location.Y + actual_dy));
                        }
                        Invalidate(); // Перерисовка формы
                    }
                    if (actual_dx != 0 || actual_dy != 0) // Если было смещение
                    {
                        _dragStartPoint = new Point(_dragStartPoint.X + actual_dx, _dragStartPoint.Y + actual_dy); // Обновление начальной точки перетаскивания
                    }
                }
            }
            else if (_mode == InteractionMode.Creating && e.Button == MouseButtons.Left && _previewShape != null) // Если режим "Создание"
            {
                if (_shapeTypeToCreate == typeof(LineShape) && _previewShape is LineShape linePreview) // Если создаётся линия
                {
                    Point currentEndPoint = e.Location; // Текущая конечная точка линии
                    currentEndPoint.X = Math.Max(ClientRectangle.Left, Math.Min(currentEndPoint.X, ClientRectangle.Right - 1)); // Ограничение по X
                    currentEndPoint.Y = Math.Max(ClientRectangle.Top, Math.Min(currentEndPoint.Y, ClientRectangle.Bottom - 1)); // Ограничение по Y
                    if (currentEndPoint != linePreview.EndPoint) // Если конечная точка изменилась
                    {
                        _previewShape = new LineShape(_creationStartPoint, currentEndPoint, Color.LightGray); // Обновление предварительного просмотра
                        _previewShape.IsSelected = true; // Выделение предварительного просмотра
                        Invalidate(); // Перерисовка формы
                    }
                }
                else if (_shapeTypeToCreate != typeof(LineShape)) // Если создаётся не линия
                {
                    int rawX = Math.Min(_creationStartPoint.X, e.X); // Расчёт минимальной координаты X
                    int rawY = Math.Min(_creationStartPoint.Y, e.Y); // Расчёт минимальной координаты Y
                    int rawWidth = Math.Abs(_creationStartPoint.X - e.X); // Расчёт ширины
                    int rawHeight = Math.Abs(_creationStartPoint.Y - e.Y); // Расчёт высоты
                    Rectangle potentialBounds; // Потенциальные границы фигуры
                    bool isSquareLike = _shapeTypeToCreate == typeof(CircleShape) || _shapeTypeToCreate == typeof(SquareShape); // Проверка, является ли фигура квадратной
                    if (isSquareLike) // Если фигура квадратная
                    {
                        int side = Math.Max(rawWidth, rawHeight); // Расчёт стороны квадрата
                        int shapeX = (_creationStartPoint.X <= e.X) ? _creationStartPoint.X : _creationStartPoint.X - side; // Расчёт координаты X
                        int shapeY = (_creationStartPoint.Y <= e.Y) ? _creationStartPoint.Y : _creationStartPoint.Y - side; // Расчёт координаты Y
                        potentialBounds = new Rectangle(shapeX, shapeY, side, side); // Создание потенциальных границ
                    }
                    else // Для остальных фигур
                    {
                        potentialBounds = new Rectangle(rawX, rawY, rawWidth, rawHeight); // Создание потенциальных границ
                    }
                    potentialBounds.Width = Math.Max(Shape.MinimumSize.Width, potentialBounds.Width); // Ограничение минимальной ширины
                    potentialBounds.Height = Math.Max(Shape.MinimumSize.Height, potentialBounds.Height); // Ограничение минимальной высоты
                    if (isSquareLike) // Если фигура квадратная
                    {
                        int finalSide = Math.Max(potentialBounds.Width, potentialBounds.Height); // Расчёт финальной стороны
                        potentialBounds.X = (_creationStartPoint.X <= e.X) // Расчёт координаты X
                               ? Math.Min(potentialBounds.X, ClientRectangle.Right - finalSide)
                               : Math.Max(ClientRectangle.Left, _creationStartPoint.X - finalSide);
                        potentialBounds.Y = (_creationStartPoint.Y <= e.Y) // Расчёт координаты Y
                              ? Math.Min(potentialBounds.Y, ClientRectangle.Bottom - finalSide)
                              : Math.Max(ClientRectangle.Top, _creationStartPoint.Y - finalSide);
                        potentialBounds.Width = finalSide; // Установка ширины
                        potentialBounds.Height = finalSide; // Установка высоты
                    }
                    Rectangle clientRect = this.ClientRectangle; // Границы клиентской области
                    int clampedX = Math.Max(clientRect.Left, potentialBounds.Left); // Ограничение по X
                    int clampedY = Math.Max(clientRect.Top, potentialBounds.Top); // Ограничение по Y
                    clampedX = Math.Min(clampedX, clientRect.Right - Shape.MinimumSize.Width); // Ограничение по правой границе
                    clampedY = Math.Min(clampedY, clientRect.Bottom - Shape.MinimumSize.Height); // Ограничение по нижней границе
                    int clampedWidth = Math.Min(potentialBounds.Width, clientRect.Right - clampedX); // Ограничение ширины
                    int clampedHeight = Math.Min(potentialBounds.Height, clientRect.Bottom - clampedY); // Ограничение высоты
                    clampedWidth = Math.Max(Shape.MinimumSize.Width, clampedWidth); // Ограничение минимальной ширины
                    clampedHeight = Math.Max(Shape.MinimumSize.Height, clampedHeight); // Ограничение минимальной высоты
                    if (isSquareLike) // Если фигура квадратная
                    {
                        int finalClampedSide = Math.Min(clampedWidth, clampedHeight); // Расчёт финальной стороны
                        clampedWidth = finalClampedSide; // Установка ширины
                        clampedHeight = finalClampedSide; // Установка высоты
                        clampedX = (_creationStartPoint.X <= e.X) // Расчёт координаты X
                               ? Math.Min(clampedX, clientRect.Right - finalClampedSide)
                               : Math.Max(clientRect.Left, _creationStartPoint.X - finalClampedSide);
                        clampedY = (_creationStartPoint.Y <= e.Y) // Расчёт координаты Y
                              ? Math.Min(clampedY, clientRect.Bottom - finalClampedSide)
                              : Math.Max(clientRect.Top, _creationStartPoint.Y - finalClampedSide);
                    }
                    Rectangle finalBounds = new Rectangle(clampedX, clampedY, clampedWidth, clampedHeight); // Финальные границы
                    bool trianglePointsUp = true; // Флаг направления треугольника
                    if (_shapeTypeToCreate == typeof(TriangleShape)) // Если создаётся треугольник
                    {
                        trianglePointsUp = (e.Y >= _creationStartPoint.Y); // Определение направления
                    }
                    bool needsUpdate = _previewShape.Bounds != finalBounds; // Проверка необходимости обновления
                    if (_previewShape is TriangleShape currentTriangle && currentTriangle.IsPointingUp != trianglePointsUp) // Если направление треугольника изменилось
                    {
                        needsUpdate = true; // Необходимость обновления
                    }
                    if (needsUpdate) // Если необходимо обновить предварительный просмотр
                    {
                        try
                        {
                            Color previewColor = _previewShape.Color; // Сохранение цвета
                            bool wasSelected = _previewShape.IsSelected; // Сохранение состояния выделения
                            if (_shapeTypeToCreate == typeof(TriangleShape)) // Если создаётся треугольник
                            {
                                _previewShape = (Shape)Activator.CreateInstance(_shapeTypeToCreate, finalBounds, previewColor, trianglePointsUp); // Обновление предварительного просмотра
                            }
                            else // Для остальных фигур
                            {
                                _previewShape = (Shape)Activator.CreateInstance(_shapeTypeToCreate, finalBounds, previewColor); // Обновление предварительного просмотра
                            }
                            _previewShape.IsSelected = wasSelected; // Восстановление состояния выделения
                            Invalidate(); // Перерисовка формы
                        }
                        catch (Exception ex) // Обработка ошибок
                        {
                            Console.WriteLine($"Ошибка обновления предварительного просмотра фигуры: {ex.Message}"); // Вывод ошибки в консоль
                        }
                    }
                }
            }
            else // Если режим не "Создание" и не "Перетаскивание"
            {
                if (_mode == InteractionMode.Selecting) // Если режим выбора
                    this.Cursor = Cursors.Default; // Установка стандартного курсора
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e) // Обработчик отпускания кнопки мыши
        {
            if (e.Button == MouseButtons.Left) // Если отпущена левая кнопка мыши
            {
                if (_mode == InteractionMode.Dragging) // Если режим "Перетаскивание"
                {
                    SetMode(InteractionMode.Selecting); // Возврат в режим выбора
                }
                else if (_mode == InteractionMode.Creating && _previewShape != null) // Если режим "Создание"
                {
                    Point endPoint = e.Location; // Конечная точка создания
                    int dragDistanceSquared = (_creationStartPoint.X - endPoint.X) * (_creationStartPoint.X - endPoint.X) +
                                             (_creationStartPoint.Y - endPoint.Y) * (_creationStartPoint.Y - endPoint.Y); // Расчёт расстояния
                    bool canAdd = false; // Флаг возможности добавления фигуры
                    if (dragDistanceSquared >= MinDragThresholdSquared) // Если расстояние достаточно большое
                    {
                        if (_shapeTypeToCreate == typeof(LineShape)) // Если создаётся линия
                        {
                            canAdd = true; // Разрешение добавления
                        }
                        else // Для остальных фигур
                        {
                            canAdd = _previewShape.Bounds.Width >= Shape.MinimumSize.Width &&
                                     _previewShape.Bounds.Height >= Shape.MinimumSize.Height; // Проверка минимальных размеров
                        }
                    }
                    if (canAdd) // Если можно добавить фигуру
                    {
                        if (_shapeTypeToCreate == typeof(LineShape) && _previewShape is LineShape linePreview) // Если создаётся линия
                        {
                            _container.Add(new LineShape(linePreview.StartPoint, linePreview.EndPoint, DefaultShapeColor)); // Добавление линии в контейнер
                        }
                        else if (_shapeTypeToCreate == typeof(TriangleShape) && _previewShape is TriangleShape trianglePreview) // Если создаётся треугольник
                        {
                            Shape finalShape = new TriangleShape(trianglePreview.Bounds, DefaultShapeColor, trianglePreview.IsPointingUp); // Создание финальной фигуры
                            finalShape.IsSelected = false; // Снятие выделения
                            _container.Add(finalShape); // Добавление треугольника в контейнер
                        }
                        else // Для остальных фигур
                        {
                            _previewShape.IsSelected = false; // Снятие выделения
                            _previewShape.Color = DefaultShapeColor; // Установка цвета по умолчанию
                            _container.Add(_previewShape); // Добавление фигуры в контейнер
                        }
                    }
                    _previewShape = null; // Сброс предварительного просмотра
                    Invalidate(); // Перерисовка формы
                }
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) // Обработчик нажатия клавиш
        {
            if (e.KeyCode == Keys.ControlKey) { _isCtrlPressed = true; } // Установка флага нажатия Ctrl
            else if (e.KeyCode == Keys.Delete) // Если нажата клавиша Delete
            {
                DeleteSelectedShapes(); // Удаление выделенных фигур
                e.Handled = true; // Пометка события как обработанного
            }
            else if (e.KeyCode >= Keys.Left && e.KeyCode <= Keys.Down) // Если нажата стрелка
            {
                HandleArrowKeyMove(e.KeyCode); // Обработка перемещения
                e.Handled = true; // Пометка события как обработанного
            }
            else if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus) // Если нажата клавиша "+"
            {
                HandleResize(5); // Увеличение размера
                e.Handled = true; // Пометка события как обработанного
            }
            else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus) // Если нажата клавиша "-"
            {
                HandleResize(-5); // Уменьшение размера
                e.Handled = true; // Пометка события как обработанного
            }
            else if (e.KeyCode == Keys.A && _isCtrlPressed) // Если нажата комбинация Ctrl+A
            {
                _container.SelectAll(); // Выделение всех фигур
                Invalidate(); // Перерисовка формы
                e.Handled = true; // Пометка события как обработанного
                e.SuppressKeyPress = true; // Подавление звука нажатия клавиши
            }
            else if (e.KeyCode == Keys.Escape) // Если нажата клавиша Escape
            {
                if (_mode == InteractionMode.Creating) // Если режим "Создание"
                {
                    _previewShape = null; // Сброс предварительного просмотра
                    SelectTool_Click(btnSelect, EventArgs.Empty); // Возврат в режим выбора
                    Invalidate(); // Перерисовка формы
                    e.Handled = true; // Пометка события как обработанного
                }
                else if (_mode == InteractionMode.Dragging) // Если режим "Перетаскивание"
                {
                    SelectTool_Click(btnSelect, EventArgs.Empty); // Возврат в режим выбора
                    Invalidate(); // Перерисовка формы
                    e.Handled = true; // Пометка события как обработанного
                }
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e) // Обработчик отпускания клавиш
        {
            if (e.KeyCode == Keys.ControlKey) { _isCtrlPressed = false; } // Сброс флага нажатия Ctrl
        }

        private void MainForm_Resize(object sender, EventArgs e) // Обработчик изменения размера формы
        {
            Invalidate(); // Перерисовка формы
        }

        private void DeleteSelectedShapes() // Метод удаления выделенных фигур
        {
            if (_container.GetSelected().Any()) // Если есть выделенные фигуры
            {
                _container.RemoveSelected(); // Удаление выделенных фигур
                Invalidate(); // Перерисовка формы
            }
        }

        private void HandleArrowKeyMove(Keys key) // Метод обработки перемещения стрелками
        {
            var selectedShapes = _container.GetSelected().ToList(); // Получение списка выделенных фигур
            if (!selectedShapes.Any()) return; // Если нет выделенных фигур, выход
            int ideal_dx = 0, ideal_dy = 0; // Идеальное смещение
            int step = _isCtrlPressed ? 1 : 5; // Шаг перемещения
            switch (key) // Определение направления
            {
                case Keys.Left: ideal_dx = -step; break; // Влево
                case Keys.Right: ideal_dx = step; break; // Вправо
                case Keys.Up: ideal_dy = -step; break; // Вверх
                case Keys.Down: ideal_dy = step; break; // Вниз
            }
            if (ideal_dx != 0 || ideal_dy != 0) // Если есть смещение
            {
                (int actual_dx, int actual_dy) = CalculateAllowedGroupMovement(selectedShapes, ideal_dx, ideal_dy); // Расчёт допустимого смещения
                if (actual_dx != 0 || actual_dy != 0) // Если есть допустимое смещение
                {
                    foreach (var shape in selectedShapes) // Перемещение каждой выделенной фигуры
                    {
                        shape.SetLocationInternal(new Point(shape.Location.X + actual_dx, shape.Location.Y + actual_dy));
                    }
                    Invalidate(); // Перерисовка формы
                }
            }
        }

        private void HandleResize(int delta) // Метод изменения размера
        {
            var selectedShapes = _container.GetSelected().ToList(); // Получение списка выделенных фигур
            if (!selectedShapes.Any()) return; // Если нет выделенных фигур, выход
            foreach (var shape in selectedShapes) // Изменение размера каждой выделенной фигуры
            {
                shape.Resize(delta, this.ClientRectangle);
            }
            Invalidate(); // Перерисовка формы
        }

        private void ChangeSelectedColor_Click(object sender, EventArgs e) // Обработчик изменения цвета
        {
            var selectedShapes = _container.GetSelected().ToList(); // Получение списка выделенных фигур
            if (!selectedShapes.Any()) // Если нет выделенных фигур
            {
                MessageBox.Show("Пожалуйста, сначала выберите фигуру(ы).", "Фигуры не выбраны", MessageBoxButtons.OK, MessageBoxIcon.Information); // Показ сообщения
                return; // Выход
            }
            using (ColorDialog colorDialog = new ColorDialog()) // Создание диалога выбора цвета
            {
                colorDialog.Color = selectedShapes.First().Color; // Установка текущего цвета
                colorDialog.FullOpen = true; // Открытие диалога в расширенном режиме
                if (colorDialog.ShowDialog(this) == DialogResult.OK) // Если выбран цвет
                {
                    foreach (var shape in selectedShapes) // Изменение цвета каждой выделенной фигуры
                    {
                        shape.Color = colorDialog.Color;
                    }
                    Invalidate(); // Перерисовка формы
                }
            }
        }

        private (int actual_dx, int actual_dy) CalculateAllowedGroupMovement(IEnumerable<Shape> shapes, int ideal_dx, int ideal_dy) // Метод расчёта допустимого смещения
        {
            if (shapes == null || !shapes.Any()) return (0, 0); // Если нет фигур, возврат нулевого смещения
            Rectangle groupBounds = GetGroupBounds(shapes); // Получение границ группы фигур
            Rectangle clientRect = this.ClientRectangle; // Границы клиентской области
            int actual_dx = ideal_dx; // Идеальное смещение по X
            int actual_dy = ideal_dy; // Идеальное смещение по Y
            if (groupBounds.Left + actual_dx < clientRect.Left) // Ограничение по левой границе
            {
                actual_dx = clientRect.Left - groupBounds.Left;
            }
            else if (groupBounds.Right + actual_dx > clientRect.Right) // Ограничение по правой границе
            {
                actual_dx = clientRect.Right - groupBounds.Right;
            }
            if (groupBounds.Top + actual_dy < clientRect.Top) // Ограничение по верхней границе
            {
                actual_dy = clientRect.Top - groupBounds.Top;
            }
            else if (groupBounds.Bottom + actual_dy > clientRect.Bottom) // Ограничение по нижней границе
            {
                actual_dy = clientRect.Bottom - groupBounds.Bottom;
            }
            return (actual_dx, actual_dy); // Возврат допустимого смещения
        }

        private Rectangle GetGroupBounds(IEnumerable<Shape> shapes) // Метод получения границ группы фигур
        {
            if (shapes == null || !shapes.Any()) return Rectangle.Empty; // Если нет фигур, возврат пустых границ
            int minX = shapes.Min(s => s.Bounds.Left); // Минимальная координата X
            int minY = shapes.Min(s => s.Bounds.Top); // Минимальная координата Y
            int maxX = shapes.Max(s => s.Bounds.Right); // Максимальная координата X
            int maxY = shapes.Max(s => s.Bounds.Bottom); // Максимальная координата Y
            return new Rectangle(minX, minY, maxX - minX, maxY - minY); // Возврат границ
        }

        protected override void Dispose(bool disposing) // Метод освобождения ресурсов
        {
            if (disposing && (components != null)) // Если нужно освободить управляемые ресурсы
            {
                components.Dispose(); // Освобождение компонентов
            }
            base.Dispose(disposing); // Вызов базового метода
        }

        private void InitializeComponent() // Метод инициализации компонентов формы
        {
            this.components = new System.ComponentModel.Container(); // Создание контейнера компонентов
            this.SuspendLayout(); // Приостановка макета формы
            this.AutoScaleDimensions = new SizeF(6F, 13F); // Установка масштаба шрифта
            this.AutoScaleMode = AutoScaleMode.Font; // Режим автоматического масштабирования
            this.ClientSize = new Size(800, 600); // Размеры формы
            this.Name = "MainForm"; // Имя формы
            this.Text = "Визуальный редактор"; // Заголовок формы
            this.ResumeLayout(false); // Возобновление макета формы
        }
    }
}