using System;
using Microsoft.Xna.Framework;

public class Cell
{
    public enum CellType
    {
        Wood,
        Sand,
        Gravel,
        Water
    }

    public enum MovementType
    {
        Static,
        Powder,
        HeavyPowder,
        Fluid
    }

    public Point position;
    public CellType cellType;
    public MovementType movementType;
    public Color color;

    public Cell(Point position, CellType cellType, Random rnd)
    {
        this.position = position;
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
                break;
        }

        //A random number between 0.9 - 1.1
        double factor = 1 + (rnd.NextDouble() / 5 - 0.1);

        color.R = (byte)Math.Min(255, Math.Max(0, color.R * factor));
        color.G = (byte)Math.Min(255, Math.Max(0, color.G * factor));
        color.B = (byte)Math.Min(255, Math.Max(0, color.B * factor));
    }
}