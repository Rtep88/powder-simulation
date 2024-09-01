using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace powder_simulation;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private const int resolutionX = 1280;
    private const int resolutionY = 720;
    private const int scale = 3;
    private const int sizeX = resolutionX / scale;
    private const int sizeY = resolutionY / scale;
    private List<Cell> cells = new List<Cell>();
    private Cell[,] cellArray = new Cell[sizeX, sizeY];
    private Texture2D pixel;
    private Random rnd = new Random();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = sizeX * scale;
        _graphics.PreferredBackBufferHeight = sizeY * scale;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
        pixel.SetData(new Color[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.D1))
            AddCell(Mouse.GetState().Position / new Point(scale), Cell.CellType.Wood);
        if (Keyboard.GetState().IsKeyDown(Keys.D2))
            AddCell(Mouse.GetState().Position / new Point(scale), Cell.CellType.Sand);
        if (Keyboard.GetState().IsKeyDown(Keys.D3))
            AddCell(Mouse.GetState().Position / new Point(scale), Cell.CellType.Gravel);
        if (Keyboard.GetState().IsKeyDown(Keys.D4))
            AddCell(Mouse.GetState().Position / new Point(scale), Cell.CellType.Water);

        if (Mouse.GetState().RightButton == ButtonState.Pressed)
            RemoveCell(Mouse.GetState().Position / new Point(scale));

        foreach (Cell cell in cells)
        {
            if (cell.movementType == Cell.MovementType.Powder || cell.movementType == Cell.MovementType.HeavyPowder)
            {
                if (cell.position.Y < sizeY - 1 && rnd.Next(0, 100) < 90)
                {
                    if (cellArray[cell.position.X, cell.position.Y + 1] == null)
                        MoveCell(cell.position, cell.position + new Point(0, 1));
                    else if (cellArray[cell.position.X, cell.position.Y + 1].movementType == Cell.MovementType.Fluid)
                    {
                        if (rnd.Next(0, 100) < 30)
                            SwitchCells(cell.position, cell.position + new Point(0, 1));
                    }
                    else if (rnd.Next(0, 100) < 40 && cell.movementType == Cell.MovementType.Powder)
                    {
                        if (cell.position.X > 0 && cellArray[cell.position.X - 1, cell.position.Y] == null && cellArray[cell.position.X - 1, cell.position.Y + 1] == null)
                            MoveCell(cell.position, cell.position + new Point(-1, 1));
                        else if (cell.position.X < sizeX - 1 && cellArray[cell.position.X + 1, cell.position.Y] == null && cellArray[cell.position.X + 1, cell.position.Y + 1] == null)
                            MoveCell(cell.position, cell.position + new Point(1, 1));
                    }
                }
            }
            if (cell.movementType == Cell.MovementType.Fluid && rnd.Next(0, 100) < 90)
            {
                if (cell.position.Y < sizeY - 1 && cellArray[cell.position.X, cell.position.Y + 1] == null)
                    MoveCell(cell.position, cell.position + new Point(0, 1));
                else if (cell.position.Y < sizeY - 1 && cell.position.X > 0 && cellArray[cell.position.X - 1, cell.position.Y + 1] == null)
                    MoveCell(cell.position, cell.position + new Point(-1, 1));
                else if (cell.position.Y < sizeY - 1 && cell.position.X < sizeX - 1 && cellArray[cell.position.X + 1, cell.position.Y + 1] == null)
                    MoveCell(cell.position, cell.position + new Point(1, 1));
                else if (rnd.Next(0, 100) < 75)
                {
                    bool leftDrop = false;
                    bool rightDrop = false;
                    int leftDistance = 0;
                    int rightDistance = 0;
                    int xPosition = cell.position.X - 1;
                    while (xPosition >= 0)
                    {
                        if ((cellArray[xPosition, cell.position.Y] != null && cellArray[xPosition, cell.position.Y].movementType != Cell.MovementType.Fluid) || xPosition == 0)
                            break;
                        else if (cell.position.Y < sizeY - 1 && cellArray[xPosition, cell.position.Y + 1] == null)
                        {
                            leftDrop = true;
                            break;
                        }
                        xPosition--;
                    }
                    xPosition = cell.position.X + 1;
                    while (xPosition < sizeX)
                    {
                        if ((cellArray[xPosition, cell.position.Y] != null && cellArray[xPosition, cell.position.Y].movementType != Cell.MovementType.Fluid) || xPosition == sizeX - 1)
                            break;
                        else if (cell.position.Y < sizeY - 1 && cellArray[xPosition, cell.position.Y + 1] == null)
                        {
                            rightDrop = true;
                            break;
                        }
                        xPosition++;
                    }
                    if (((rnd.Next(0, 2) == 1) || (leftDrop && !rightDrop) || leftDistance < rightDistance) && (leftDrop || !rightDrop) && cell.position.X > 0 && cellArray[cell.position.X - 1, cell.position.Y] == null)
                        MoveCell(cell.position, cell.position + new Point(-1, 0));
                    else if (cell.position.X < sizeX - 1 && cellArray[cell.position.X + 1, cell.position.Y] == null)
                        MoveCell(cell.position, cell.position + new Point(1, 0));
                    else if (cell.position.X > 0 && cellArray[cell.position.X - 1, cell.position.Y] == null)
                        MoveCell(cell.position, cell.position + new Point(-1, 0));
                }
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        foreach (Cell cell in cells)
        {
            _spriteBatch.Draw(pixel, new Rectangle(cell.position * new Point(scale), new Point(scale)), cell.color);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void AddCell(Point position, Cell.CellType cellType)
    {
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                Point shiftedPosition = position + new Point(x, y);
                if (shiftedPosition.X >= 0 && shiftedPosition.Y >= 0 && shiftedPosition.X < sizeX && shiftedPosition.Y < sizeY &&
                    cellArray[shiftedPosition.X, shiftedPosition.Y] == null)
                {
                    Cell newCell = new Cell(shiftedPosition, cellType, rnd);
                    cells.Add(newCell);
                    cellArray[shiftedPosition.X, shiftedPosition.Y] = newCell;
                }
            }
        }
    }

    private void MoveCell(Point from, Point to)
    {
        if (cellArray[from.X, from.Y] != null && cellArray[to.X, to.Y] == null)
        {
            Cell movedCell = cellArray[from.X, from.Y];
            movedCell.position = to;
            cellArray[from.X, from.Y] = null;
            cellArray[to.X, to.Y] = movedCell;
        }
    }

    private void SwitchCells(Point from, Point to)
    {
        if (cellArray[from.X, from.Y] != null && cellArray[to.X, to.Y] != null)
        {
            cellArray[to.X, to.Y].position = from;
            cellArray[from.X, from.Y].position = to;
            Cell switchedCell = cellArray[from.X, from.Y];
            cellArray[from.X, from.Y] = cellArray[to.X, to.Y];
            cellArray[to.X, to.Y] = switchedCell;
        }
    }

    private void RemoveCell(Point position)
    {
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                Point shiftedPosition = position + new Point(x, y);
                if (shiftedPosition.X >= 0 && shiftedPosition.Y >= 0 && shiftedPosition.X < sizeX && shiftedPosition.Y < sizeY &&
                    cellArray[shiftedPosition.X, shiftedPosition.Y] != null)
                {
                    Cell removedCell = cellArray[shiftedPosition.X, shiftedPosition.Y];
                    cellArray[shiftedPosition.X, shiftedPosition.Y] = null;
                    cells.Remove(removedCell);
                }
            }
        }
    }
}
