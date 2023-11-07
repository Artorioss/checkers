using CheckersGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class Form1 : Form
    {
        const int mapSize = 8; //Размер карты
        const int cellSize = 60; //Размер каждой ячейки
         
        int currentPlayer; //Игрок который ходит
        ToolStrip toolStrip; 
        ToolStripLabel label1;
        ToolStripLabel label2;
        ToolStripLabel label3;

        List<Button> simpleSteps = new List<Button>(); //Список для хранения доступных ходов для пешки

        int countEatSteps = 0;
        Button prevButton; //Предыдущая кнопка на которую кликнул игрок
        Button pressedButton; //любая клетка куда нажмёт игрок
        bool isContinue = false;

        bool isMoving;

        int[,] map = new int[mapSize, mapSize]; //Инициализация игрового поля

        Button[,] buttons = new Button[mapSize, mapSize]; //ячейки игрового поля

        Image whiteFigure; 
        Image blackFigure;

        public Form1()
        {
            InitializeComponent();

            
            whiteFigure = new Bitmap(Resources.w, new Size(cellSize - 10, cellSize - 10)); 
            blackFigure = new Bitmap(Resources.b, new Size(cellSize - 10, cellSize - 10));

            create_panel(); //Инициализация игрового поля
        }

        //создание игрового поля и размещение на нем шашек
        public void Init()
        {
            currentPlayer = 1; //Первым ходит игрок 1. 
            isMoving = false;
            prevButton = null;

            map = new int[mapSize,mapSize] { //двумерный массив имитирующий поле. Единицы - белые шашки. Двойки - черный шашки.
                { 0,1,0,1,0,1,0,1 },
                { 1,0,1,0,1,0,1,0 },
                { 0,1,0,1,0,1,0,1 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 2,0,2,0,2,0,2,0 },
                { 0,2,0,2,0,2,0,2 },
                { 2,0,2,0,2,0,2,0 }
            };

            MaximumSize = new Size(0, 0);
            //MinimumSize = new Size(0,0);
            //Настройка размера окна программы (ширина и высота)
            this.Width = ((mapSize) * cellSize) + 16;
            this.Height = ((mapSize) * cellSize) + 70;

            //Максимальный размер окна программы
            MaximumSize = new Size(Width, Height);
            //MinimumSize = new Size(Width, Height);

            //Создание панели
            toolStrip = new ToolStrip();
            Controls.Add(toolStrip);

            //Создание кнопки на панели для выхода из программы
            ToolStripButton toolbutton = new ToolStripButton();
            toolbutton.Click += button_exit_Click; //Присвоение события (При клике по кнопке будет вызван метод button_exit_Click)
            toolbutton.Image = Resources.иконка_выход; //Иконка для кнопки
            toolStrip.Items.Add(toolbutton); //Добавление кнопки на панель
            toolStrip.Items.Add(new ToolStripSeparator()); //Добавление разделителя 

            //Создание лейбла
            label1 = new ToolStripLabel();
            toolStrip.Items.Add(label1); //Добавление лейбла на панель
            label1.Text = "Ход белых"; //Присвоение текста лейблу
            toolStrip.Items.Add(new ToolStripSeparator()); //Добавление лейбла на панель

            label2 = new ToolStripLabel(); //Создание лейбла
            label2.Text = "Кол-во белых фигур - 12"; //присвоение название лейблу
            toolStrip.Items.Add(label2); //Добавление лейбла на панель 
            toolStrip.Items.Add(new ToolStripSeparator()); //Создание разделителя

            label3 = new ToolStripLabel(); //Создание лейбла
            label3.Text = "Кол-во чёрных фигур - 12"; //присвоение текста лейблу
            toolStrip.Items.Add(label3); //Добавление лейбла на панель

            //Заполнение игрового поля
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button(); //Создание кнопки
                    button.Location = new Point(j * cellSize, i * cellSize + 30); //Позиция кнопки
                    button.Size = new Size(cellSize, cellSize); //Размер кнопки
                    button.Click += new EventHandler(OnFigurePress); //Присвоение кнопке события OnFigurePress (метод будет вызван после клика по полю)
                    if (map[i, j] == 1) //Размещение фигур на поле
                        button.Image = whiteFigure; //Картинка белой фигуры
                    else if (map[i, j] == 2) button.Image = blackFigure; //картинка черной фигуры

                    button.BackColor = GetPrevButtonColor(button); //Назначение цвета кнопке
                    button.ForeColor = Color.Red; //Цвет переднего плана

                    buttons[i, j] = button; //Присвоение кнопки элементу в массиве с кнопками

                    this.Controls.Add(button);
                }
            }
        }

        //Подсчёт пешек и перезагрузка поля
        public void ResetGame()
        {
            bool player1 = false; //Если на поле не будут найдены белые фигуры, значение останется false - игрок проиграл
            bool player2 = false; 
            int i1 = 0; //кол-во белых фигур
            int i2 = 0; //кол-во черных фигур
            for(int i = 0; i < mapSize; i++) //перебор всего поля
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == 1) { //если найдена белая фигура
                        player1 = true; 
                        i1++; //счётчик белых фигур увеличивается на 1
                    }

                    if (map[i, j] == 2) { //если найдена черная фигура
                        player2 = true;
                        i2++; //счётчик белых фигур увеличивается на 1
                    }
                }
            }
            label2.Text = $"Кол-во белых фигур - {i1}";
            label3.Text = $"Кол-во чёрных фигур - {i2}";
            if (!player1 || !player2) //Если у одного из игроков закончились фигуры
            {
                //Фигуры закончились у первого игрока (белые фигуры)
                if (!player1) MessageBox.Show("Победили чёрные!", "Игра закончена!", MessageBoxButtons.OK, MessageBoxIcon.Information); //Закончились черные фигуры
                else MessageBox.Show("Победили Белые!", "Игра закончена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Controls.Clear(); //Удаление всех элементов с игрового поля
                create_panel(); // Инициализация игрового поля
                this.Size = new Size(654, 461);
            }
        }

        public void create_panel()
        {
            //Инициализация игрового поля
            Panel panel = new Panel();
            panel.Size = new Size(268, 265);
            panel.Location = new Point(110, 77);

            //Создание кнопки "Новая игра"
            Button button1 = new Button();
            button1.Size = new Size(173, 60);
            panel.Controls.Add(button1);
            button1.Parent = panel;
            button1.Location = new Point(48, 21);
            button1.Text = "Новая игра";
            button1.Font = new Font(button1.Font.FontFamily, 10);
            button1.Click += new EventHandler(button1_Click);

            //Создание кнопки "Справка"
            Button button2 = new Button();
            button2.Size = new Size(173, 60);
            panel.Controls.Add(button2);
            button2.Parent = panel;
            button2.Location = new Point(48, 103);
            button2.Text = "Справка";
            button2.Font = new Font(button2.Font.FontFamily, 10);
            button2.Click += new EventHandler(button2_Click);

            //Создание кнопки для завершения программы
            Button button3 = new Button();
            button3.Size = new Size(173, 60);
            panel.Controls.Add(button3);
            button3.Parent = panel;
            button3.Location = new Point(48, 186);
            button3.Text = "Выход";
            button3.Font = new Font(button3.Font.FontFamily, 10);
            button3.Click += new EventHandler(button3_Click);
            this.Controls.Add(panel);
        }

        public void SwitchPlayer() //переключает текущего игрока.
        {
            currentPlayer = currentPlayer == 1 ? 2 : 1; //Смена значения currentPlayer на 1 или 2
            if (currentPlayer == 1) label1.Text = "Ход белых"; 
            else label1.Text = "Ход чёрных";
            ResetGame(); //
        }

        //Возврат цвета для ячейки поля (Раскраска поля, приведение его в шахматному виду)
        public Color GetPrevButtonColor(Button prevButton)
        {
            //Если кнопка находится на не четной линии и является чётной по счету
            if ((prevButton.Location.Y / cellSize % 2) != 0) //
            {
                if ((prevButton.Location.X / cellSize % 2) == 0)
                {
                    return Color.Gray;
                }
            }
            //Если кнопка находится на чётной линии и является нечётной по счету
            if ((prevButton.Location.Y / cellSize) % 2 == 0)
            {
                if ((prevButton.Location.X / cellSize) % 2 != 0)
                {
                    return Color.Gray;
                }
            }
            return Color.White;
        }

        public void OnFigurePress(object sender, EventArgs e) //обрабатывает нажатие на кнопку, соответствующую шашке.
                                                              //Если на кнопке расположена шашка текущего игрока, то выделяются доступные для нее ходы.
                                                              //Если же на кнопке нет шашки или она принадлежит другому игроку, то происходит попытка переместить шашку на эту клетку.
                                                              //Если при этом была взята шашка противника, то игрок может продолжить свой ход.
                                                              //После каждого хода проверяется возможность продолжения игры и переключается текущий игрок.
        {
            if (prevButton != null) //Если уже выбрана клетка для хода
                prevButton.BackColor = GetPrevButtonColor(prevButton); //изменение цвета фигуры

            pressedButton = sender as Button; //Объект вызвавший метод (в данном случае фигура на которую нажали)
            //Проверка на то что игрок нажал на свою фигуру
            if(map[pressedButton.Location.Y/cellSize,pressedButton.Location.X/cellSize] != 0 && map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] == currentPlayer)
            {
                CloseSteps(); //изменение цвета всех кнопок на белый
                pressedButton.BackColor = Color.Red; //изменение цвета кнопки на которую нажал игрок на красный
                DeactivateAllButtons(); //Блокировка всех кнопок
                pressedButton.Enabled = true; //Разблокировка кнопки которая была нажата игроком
                countEatSteps = 0;
                if(pressedButton.Text == "D") //Если фигура является дамкой
                ShowSteps(pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize, false); //Отображение доступных клеток для передвижения на всю диагональ
                else ShowSteps(pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize); //Отображение доступных клеток для передвижения на одну клетку

                //Если фигура уже двигалась
                if (isMoving)
                {
                    CloseSteps(); //Изменение цвета ячеек на игровом поле 
                    pressedButton.BackColor = GetPrevButtonColor(pressedButton);
                    ShowPossibleSteps(); //Отображение возможных ходов
                    isMoving = false;
                }
                else
                    isMoving = true;
            }
            else
            {
                if (isMoving) //если был завершен ход
                {
                    isContinue = false;
                    //если была взята шашка противника
                      if (Math.Abs(pressedButton.Location.X / cellSize - prevButton.Location.X/cellSize) > 1)
                      {
                        isContinue = true;
                        DeleteEaten(pressedButton, prevButton); //удаление кнопки с поля                         
                      }
                    //Получение значения клетки куда нажал игрок
                    int temp = map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize];
                    //Перемещение фигуры
                    map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] = map[prevButton.Location.Y / cellSize, prevButton.Location.X / cellSize];
                    map[prevButton.Location.Y / cellSize, prevButton.Location.X / cellSize] = temp; //Изменение значения предыдущей клетки
                    pressedButton.Image = prevButton.Image; //у объекта(кнопки) вызвавшего функцию меняется иконка
                    prevButton.Image = null; //У старого положения фигуры(кнопки) удаляется картинка 
                    pressedButton.Text = prevButton.Text; //Смена текста местами
                    prevButton.Text = "";
                    SwitchButtonToCheat(pressedButton); //Проверка стала ли шашка дамкой
                    countEatSteps = 0;
                    isMoving = false;                    
                    CloseSteps(); //Изменение цвета у всех кнопок на белый
                    DeactivateAllButtons(); //деактивация всех кнопок
                    if (pressedButton.Text == "D") //если фигура является дамкой
                        ShowSteps(pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize, false); //Отображение доступных клеток для передвижения на всю диагональ
                    else ShowSteps(pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize); //Отображение доступных клеток для передвижения на одну клетку
                    if (countEatSteps == 0 || !isContinue) //если не было взято ни одной фигуры
                    {
                        CloseSteps(); //Изменение цвета ячеек на игровом поле
                        SwitchPlayer(); //Смена ходившего
                        ShowPossibleSteps(); //Отображение возможных ходов 
                        isContinue = false;
                    }else if(isContinue)
                    {
                        pressedButton.BackColor = Color.Red;
                        pressedButton.Enabled = true;
                        isMoving = true;
                    }
                }
            }

            prevButton = pressedButton; //Запись кнопки на которую нажал пользователь
        }

        public void ShowPossibleSteps() //отображает доступные ходы для текущего игрока
        {
            bool isOneStep = true;
            bool isEatStep = false;
            DeactivateAllButtons(); //Блокировка всех кнопок на поле
            for(int i = 0; i < mapSize; i++)
            {
                for (int j= 0; j < mapSize; j++)
                {
                    if (map[i, j] == currentPlayer)
                    {
                        if (buttons[i, j].Text == "D") //Если фигура является дамкой
                            isOneStep = false; //разрешить двигаться на несколько клеток
                        else isOneStep = true; //иначе двигаться только на одну клетку
                        if (IsButtonHasEatStep(i, j, isOneStep, new int[2] { 0, 0 })) //Если есть фигуры соперника которые можно взять
                        {
                            isEatStep = true; //Флаг
                            buttons[i, j].Enabled = true; //Разблокировка кнопки
                        }
                    }
                }
            }
            if (!isEatStep) //если нет фигур соперника для взятия
                ActivateAllButtons(); //разблокировка всех фигур на поле
        }

        public void SwitchButtonToCheat(Button button)  //заменяет шашку на дамку, если она достигла противоположного края поля.
        {
            if (map[button.Location.Y / cellSize, button.Location.X / cellSize] == 1 && button.Location.Y / cellSize == mapSize - 1) 
            {
                button.Text = "D";
                
            }
            if (map[button.Location.Y / cellSize, button.Location.X / cellSize] == 2 && button.Location.Y / cellSize == 0)
            {
                button.Text = "D";
            }
        }

        public void DeleteEaten(Button endButton, Button startButton) //удаляет шашку противника, если она была взята в результате хода.
        {
            int count = Math.Abs(endButton.Location.Y / cellSize - startButton.Location.Y / cellSize); //Вычисляется количество клеток, пройденных шашкой за один ход.
            int startIndexX = endButton.Location.Y / cellSize - startButton.Location.Y / cellSize; //Вычисляются индексы начальной клетки, откуда началось движение шашки.
            int startIndexY = endButton.Location.X / cellSize - startButton.Location.X / cellSize;
            startIndexX = startIndexX < 0 ? -1 : 1; //Индексы начальной клетки корректируются, чтобы получить направление движения шашки.
            startIndexY = startIndexY < 0 ? -1 : 1;
            int currCount = 0;
            int i = startButton.Location.Y / cellSize + startIndexX;
            int j = startButton.Location.X / cellSize + startIndexY;
            //Запускается цикл while, который будет выполняться до тех пор, пока currCount не станет равным количеству пройденных клеток
            while (currCount < count-1)
            {
                map[i, j] = 0; //удаление фигуры из массива
                buttons[i, j].Image = null; //Удаление картинки
                buttons[i, j].Text = ""; //Удаление текста
                i += startIndexX;
                j += startIndexY;
                currCount++;
            }

        }
        //Отображение доступных ходов
        public void ShowSteps(int iCurrFigure, int jCurrFigure, bool isOnestep = true)
        {
            simpleSteps.Clear();
            ShowDiagonal(iCurrFigure, jCurrFigure, isOnestep); //Отображение диагонали по которой может двигаться фигура
            if (countEatSteps > 0)
                CloseSimpleSteps(simpleSteps); //блокировка всех пустых ячеек
        }

        public void ShowDiagonal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)  //метод, отображающий доступные ходы по диагонали для шашки на клетке                                                                                     //с координатами `IcurrFigure` и `JcurrFigure`. Если `isOneStep` равен `true`,                                                                            //то отображаются только ходы на одну клетку.
        {
            //для белых шашек
            int j = JcurrFigure + 1; 
            //перебор всех клеток по координате i
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j)) //Проверка чтобы не выйти за границы игрового поля
                {
                    if (!DeterminePath(i, j)) //если нет возможности хода
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep) //если ход только на одну клетку
                    break;
            }

            j = JcurrFigure - 1;
            //перебор всех клеток по координате i
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j)) //Проверка чтобы не выйти за границы игрового поля
                {
                    if (!DeterminePath(i, j)) //если нет возможности хода
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep) //если ход только на одну клетку
                    break;
            }

            //для черных шашек
            j = JcurrFigure - 1;
            //перебор всех клеток по координате j
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j)) //Проверка чтобы не выйти за границы игрового поля
                {
                    if (!DeterminePath(i, j)) //если нет возможности хода
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep) //если ход только на одну клетку
                    break;
            }

            j = JcurrFigure + 1;
            //перебор всех клеток по координате j
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j)) //Проверка чтобы не выйти за границы игрового поля
                {
                    if (!DeterminePath(i, j)) //если нет возможности хода
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep) //если ход только на одну клетку
                    break;
            }
        }
        
        public bool DeterminePath(int ti,int tj) //метод, определяющий доступные ходы для шашки на клетке
        {
            //Если клетка пустая
            if (map[ti, tj] == 0 && !isContinue)
            {
                buttons[ti, tj].BackColor = Color.Yellow; //изменение цвета клетки
                buttons[ti, tj].Enabled = true; //Разблокировка 
                simpleSteps.Add(buttons[ti, tj]); //Добавление в список
            }
            else
            {
                
                if (map[ti, tj] != currentPlayer) //Если на клетке находится фигура противника
                {
                    if (pressedButton.Text == "D") //Если объект вызвавший метод является дамкой
                        ShowProceduralEat(ti, tj, false); //доступные шашки для взятия для дамки
                    else ShowProceduralEat(ti, tj); //доступные шашки для взятия для обычной шашки
                }

                return false;
            }
            return true;
        }

        public void CloseSimpleSteps(List<Button> simpleSteps) //метод, блокирующий все кнопки переданные в списке
        {
            if (simpleSteps.Count > 0)
            {
                for (int i = 0; i < simpleSteps.Count; i++)
                {
                    simpleSteps[i].BackColor = GetPrevButtonColor(simpleSteps[i]);
                    simpleSteps[i].Enabled = false; //блокировка кнопки
                }
            }
        }
        public void ShowProceduralEat(int i,int j,bool isOneStep = true) //метод, отображающий доступные ходы для шашки при ее взятии противника.
                                                                         //Если `isOneStep` равен `true`, то отображаются только ходы на одну клетку.
        {
            int dirX = i - pressedButton.Location.Y / cellSize;
            int dirY = j - pressedButton.Location.X / cellSize;
            dirX = dirX < 0 ? -1 : 1;
            dirY = dirY < 0 ? -1 : 1;
            //Координаты шашки
            int il = i; 
            int jl = j;
            bool isEmpty = true;
            //Пока шашка находится в границах игрового поля
            while (IsInsideBorders(il, jl))
            {
                if (map[il, jl] != 0 && map[il, jl] != currentPlayer) //Если в клетке находится шашка соперника
                { 
                    isEmpty = false; //Флаг - false
                    break;
                }
                il += dirX;
                jl += dirY;

                if (isOneStep)
                    break;
            }
            if (isEmpty)
                return;
            List<Button> toClose = new List<Button>(); //Список с кнопками которые будут заблокированы
            bool closeSimple = false;
            //Получение координаты шашки
            int ik = il + dirX; 
            int jk = jl + dirY;
            //Пока шашка находится в границах игрового поля
            while (IsInsideBorders(ik,jk))
            {
                if (map[ik, jk] == 0 ) //Если ячейка пустая
                {
                    if (IsButtonHasEatStep(ik, jk, isOneStep, new int[2] { dirX, dirY })) //Если есть фигуры соперника которые можно взять
                    {
                        closeSimple = true; 
                    }
                    else
                    {
                        toClose.Add(buttons[ik, jk]); //Кнопка будет заблокирована
                    }
                    buttons[ik, jk].BackColor = Color.Yellow; //Желтый цвет кнопки
                    buttons[ik, jk].Enabled = true; //Разблокировка кнопки
                    countEatSteps++; //Кол-во шашек которые можно взять
                }
                else break;
                if (isOneStep) //если пешка не шашка
                    break;
                jk += dirY;
                ik += dirX;
            }
            if (closeSimple && toClose.Count > 0) 
            {
                CloseSimpleSteps(toClose); //Блокировка всех пустых ячеек
            }
            
        }

        public bool IsButtonHasEatStep(int IcurrFigure, int JcurrFigure, bool isOneStep, int[] dir) //метод, проверяющий, есть ли у кнопки на клетке с координатами `i` и `j`
                                                                                                    //доступный взятию ход в направлении `dir`. Если `isOneStep` равен `true`, то проверяется только один ход.
        {
            bool eatStep = false;
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--) //Проверка на нахождение чужой фигуры по координате i
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (dir[0] == 1 && dir[1] == -1 && !isOneStep)break;
                if (IsInsideBorders(i, j)) //Проверка на выход за границы
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer) //если найдена чужая фигура
                    {
                        eatStep = true; //Доступная фигура для взятия = true
                        if (!IsInsideBorders(i - 1, j + 1)) //Проверка на границы игрового поля с какой либо стороны фигуры
                            eatStep = false; 
                        else if (map[i - 1, j + 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep) //если был доступен ход только на одну клетку вперед
                    break;
            }

            j = JcurrFigure - 1; //Проверка на наличие фигур соперника по координате j
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (dir[0] == 1 && dir[1] == 1 && !isOneStep) break;
                if (IsInsideBorders(i, j)) //Проверка границ игрового поля
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer) //если найдена фигура соперника
                    {
                        eatStep = true; //Доступная фигура для взятия = true
                        if (!IsInsideBorders(i - 1, j - 1)) //Если с какой либо стороны фигуры соперника заканчивается игровое поле, то фигуру нельзя взять
                            eatStep = false;
                        else if (map[i - 1, j - 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep) //если был доступен ход только на одну клетку вперед
                    break;
            }

            j = JcurrFigure - 1; //Проверка на наличие фигур соперника по координате j 
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (dir[0] == -1 && dir[1] == 1 && !isOneStep) break;
                if (IsInsideBorders(i, j)) //Проверка границ игрового поля
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer) //если найдена фигура соперника
                    {
                        eatStep = true; //Доступная фигура для взятия = true
                        if (!IsInsideBorders(i + 1, j - 1)) //Если с какой либо стороны фигуры соперника заканчивается игровое поле, то фигуру нельзя взять
                            eatStep = false;
                        else if (map[i + 1, j - 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep) //если был доступен ход только на одну клетку вперед
                    break;
            }

            j = JcurrFigure + 1; //Проверка на наличие фигур противника по координате j
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (dir[0] == -1 && dir[1] == -1 && !isOneStep) break;
                if (IsInsideBorders(i, j)) //Проверка границ игрового поля
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer) //если найдена фигура соперника
                    {
                        eatStep = true; //Доступная фигура для взятия = true
                        if (!IsInsideBorders(i + 1, j + 1)) //Если с какой либо стороны фигуры соперника заканчивается игровое поле, то фигуру нельзя взять
                            eatStep = false;
                        else if (map[i + 1, j + 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep) //если был доступен ход только на одну клетку вперед
                    break;
            }
            return eatStep;
        }

        public void CloseSteps() //перекрашивает доступные ходы для всех кнопок на игровом поле
        {
            //Перебор каждой кнопки в массиве buttons и изменение цвета на белый\серый
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    buttons[i, j].BackColor = GetPrevButtonColor(buttons[i, j]);
                }
            }
        }

        public bool IsInsideBorders(int ti,int tj) //проверяет, находятся ли переданные координаты в пределах игрового поля.
        {
            if(ti>=mapSize || tj >= mapSize || ti<0 || tj < 0)
            {
                return false;
            }
            return true;
        }

        public void ActivateAllButtons() //делает все кнопки на игровом поле доступными для нажатия.
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    buttons[i, j].Enabled = true;
                }
            }
        }

        public void DeactivateAllButtons() //делает все кнопки на игровом поле недоступными для нажатия
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    buttons[i, j].Enabled = false;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) //Событие вызывает метод Close
        {
            Close(); //Закрытие формы
        }


        private void button2_Click(object sender, EventArgs e)
        {
            new Form2().ShowDialog(); //Отображение формы "Спрака"
        }

        //Новая игра
        private void button1_Click(object sender, EventArgs e)
        {
            Controls.Clear();
            Init();  
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            Controls.Clear();
            create_panel(); //Инициализация игрового поля
        }
    }
}
