using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class Cell
{
    public enum CellType
    {
        None,
        Wood,
        Sand,
        Gravel,
        Water,
        Acid
    }

    public enum MovementType
    {
        Static,
        Powder,
        HeavyPowder,
        Fluid
    }

    //Base
    public Point position;
    public CellType cellType;
    public MovementType movementType;
    public Color color;
    public int density = int.MaxValue;

    //Morphing
    public Dictionary<CellType, CellType> morphCollisionsWhiteList = new Dictionary<CellType, CellType>(); //(From, To)
    public bool blackList = false; //false = white list is used, true = black list is used
    public List<CellType> morphCollisionsBlackList = new List<CellType>(); //Which cell types should not be morphed
    public CellType morphInto = CellType.None; //If none than morphed cell is deleted
    public bool destroyAfterMorph = false;
    public (int, int, int, int) morphChances = (0, 0, 0, 0); //Left, right, up, down

    public Cell(Point position, CellType cellType, Random rnd)
    {
        this.position = position;
        SetCellType(cellType, rnd);
    }

    public void SetCellType(CellType cellType, Random rnd)
    {
        this.cellType = cellType;

        switch (cellType)
        {
            case CellType.Wood:
                color = new Color(99, 45, 0);
                movementType = MovementType.Static;
                break;
            case CellType.Sand:
                color = new Color(255, 187, 0);
                movementType = MovementType.Powder;
                break;
            case CellType.Gravel:
                color = new Color(87, 86, 86);
                movementType = MovementType.HeavyPowder;
                break;
            case CellType.Water:
                color = new Color(39, 65, 196);
                movementType = MovementType.Fluid;
                density = 1000;
                break;
            case CellType.Acid:
                color = new Color(45, 242, 19);
                movementType = MovementType.Fluid;
                density = 1500;
                blackList = true;
                destroyAfterMorph = true;
                morphChances = (5, 5, 10, 10);
                break;
        }

        //A random number between 0.9 - 1.1
        double factor = 1 + (rnd.NextDouble() / 5 - 0.1);

        color.R = (byte)Math.Min(255, Math.Max(0, color.R * factor));
        color.G = (byte)Math.Min(255, Math.Max(0, color.G * factor));
        color.B = (byte)Math.Min(255, Math.Max(0, color.B * factor));
    }
}