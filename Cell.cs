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
        Acid,
        Fire,
        Lava,
        Virus,
        Steam,
        Smoke
    }

    public enum MovementType
    {
        Static,
        Powder,
        HeavyPowder,
        Fluid,
        Gas
    }

    //Base
    public Point position;
    public CellType cellType;
    public MovementType movementType;
    public Color color;
    public int density = int.MaxValue;
    public int moveChance = -1;

    //Morphing
    public Dictionary<CellType, (CellType, int, int)> morphCollisionsWhiteList = new Dictionary<CellType, (CellType, int, int)>(); //(From, (To, Morph chance, Disapper chance))
    public bool blackList = false; //false = white list is used, true = black list is used
    public List<CellType> morphCollisionsBlackList = new List<CellType>(); //Which cell types should not be morphed
    public CellType morphInto = CellType.None; //If none than morphed cell is deleted
    public int morphChance = 0; //For blacklist
    public int morphDisapperChance = 0; //For blacklist

    //lifespan
    public float lifeSpan = float.MaxValue;
    public float currentTime = 0;
    public CellType afterLifeMorph = CellType.None;
    public int chanceToDisapper = 1000; // (0 - 1000)

    public Cell(Point position, CellType cellType, Random rnd)
    {
        this.position = position;
        SetCellType(cellType, rnd);
    }

    public void SetCellType(CellType cellType, Random rnd)
    {
        this.cellType = cellType;
        density = int.MaxValue;
        moveChance = -1;
        morphCollisionsWhiteList = new Dictionary<CellType, (CellType, int, int)>();
        blackList = false;
        morphCollisionsBlackList = new List<CellType>();
        morphInto = CellType.None;
        morphChance = 0;
        morphDisapperChance = 0;
        lifeSpan = 0;
        currentTime = 0;
        afterLifeMorph = CellType.None;
        chanceToDisapper = 1000;

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
                morphDisapperChance = 100;
                morphChance = 7;
                break;
            case CellType.Smoke:
                color = new Color(60, 60, 60);
                movementType = MovementType.Gas;
                density = 2;
                lifeSpan = 2;
                chanceToDisapper = 50;
                break;
            case CellType.Fire:
                density = 5;
                color = new Color(255, 0, 0);
                movementType = MovementType.Gas;
                morphCollisionsWhiteList.Add(CellType.Wood, (CellType.Fire, 11, 0));
                lifeSpan = 0.5f;
                afterLifeMorph = CellType.Smoke;
                chanceToDisapper = 60;
                moveChance = 5;
                break;
            case CellType.Lava:
                density = 3100;
                color = new Color(255, 97, 18);
                movementType = MovementType.Fluid;
                morphCollisionsWhiteList.Add(CellType.Wood, (CellType.Fire, 80, 0));
                morphCollisionsWhiteList.Add(CellType.Water, (CellType.Steam, 80, 50));
                moveChance = 30;
                break;
            case CellType.Steam:
                density = 1;
                lifeSpan = 3f;
                chanceToDisapper = 50;
                color = new Color(200, 200, 200);
                movementType = MovementType.Gas;
                break;
            case CellType.Virus:
                color = new Color(88, 0, 176);
                movementType = MovementType.HeavyPowder;
                blackList = true;
                morphChance = 50;
                morphInto = CellType.Virus;
                moveChance = 10;
                break;
        }

        if (moveChance == -1)
        {
            switch (movementType)
            {
                case MovementType.Powder:
                    moveChance = 90;
                    break;
                case MovementType.HeavyPowder:
                    moveChance = 90;
                    break;
                case MovementType.Gas:
                    moveChance = 20;
                    break;
                case MovementType.Fluid:
                    moveChance = 90;
                    break;
            }
        }

        //A random number between 0.9 - 1.1
        double factor = 1 + (rnd.NextDouble() / 5 - 0.1);

        color.R = (byte)Math.Min(255, Math.Max(0, color.R * factor));
        color.G = (byte)Math.Min(255, Math.Max(0, color.G * factor));
        color.B = (byte)Math.Min(255, Math.Max(0, color.B * factor));
    }
}