using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows { get; private set; }
    public TileCell[] cells { get; private set; }

    public int size => cells.Length;
    public int height => rows.Length;
    public int width => size / height;

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    private void Start()
    {
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].cells.Length; x++)
            {
                rows[y].cells[x].pos = new Vector2Int(x, y);
            }
        }
    }

    public TileCell GetCellFromPosition(Vector2Int pos)
    {
        return GetCell(pos.x, pos.y);
    }

    public TileCell GetAdjecentCell(TileCell cell, Vector2Int dir)
    {
        Vector2Int pos = cell.pos;
        pos.x += dir.x;
        pos.y -= dir.y;

        return GetCellFromPosition(pos);

    }

    public TileCell GetRandomEmptyCell()
    {
        var index = Random.Range(0, cells.Length);
        var startIndex = index;
        while (cells[index].isOccupied)
        {
            index++;
            if(index >= cells.Length)
            {
                index = 0;
            }

            if(index == startIndex)
            {
                return null;
            }
        }

        return cells[index];
    }

    public TileCell GetCell(int x, int y)
    {
        if(x >= 0 && x < width && y >= 0 && y < height)
        {
            return rows[y].cells[x];
        }
        else 
        { 
            return null; 
        }
    }
}
