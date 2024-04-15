using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private TileState[] tileStates;
    [SerializeField] private GameController game;

    private TileGrid grid;
    private List<Tile> tiles;
    private bool isPlayingAnim;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    public void ClearBoard()
    {
        foreach(var cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }

    void Update()
    {   
        if (!isPlayingAnim)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1,1,1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left, 1, 1,0,1);

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }
    }

    public void CreateNewTile()
    {
        var newTile = Instantiate(tilePrefab, grid.transform);
        newTile.SetState(tileStates[0], 2);
        newTile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(newTile);
    }


    private void MoveTiles(Vector2Int dir, int startX, int incrementX, int startY, int incrementY)
    {
        var changed = false;

        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for(int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if(cell.isOccupied)
                {
                    changed |= MoveTile(cell.tile, dir);
                }
            }
        }

        if(changed)
        {
            StartCoroutine(WairForChanges());
        }


    }

    private bool MoveTile(Tile tile, Vector2Int dir)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjecentCell(tile.cell, dir);

        while(adjacent != null)
        {
            if (adjacent.isOccupied)
            {
                if(CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjecentCell(adjacent, dir);
        }

        if(newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile tileA, Tile tileB)
    {
        return tileA.curCellNumber == tileB.curCellNumber && !tileB.locked;
    }

    private void Merge(Tile tileA, Tile tileB)
    {
        tiles.Remove(tileA);
        tileA.Merge(tileB.cell);

        var index = Mathf.Clamp(IndexOf(tileB.curState) + 1, 0, tileStates.Length - 1);
        var number = tileB.curCellNumber * 2;

        tileB.SetState(tileStates[index], number);
    }

    private int IndexOf(TileState tileState)
    {
        for(int i = 0; i < tiles.Count; i++)
        {
            if(tileState == tileStates[i])
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WairForChanges()
    {
        isPlayingAnim = true;

        yield return new WaitForSeconds(0.1f);

        isPlayingAnim = false;

        foreach(var tile in tiles)
        {
            tile.locked = false;
        }

        if(tiles.Count != grid.size)
        {
            CreateNewTile();
        }

        if (CheckForGameOver())
        {
            game.GameOver();
        }

    }

    private bool CheckForGameOver()
    {
        if(tiles.Count != grid.size)
        {
            return false;
        }

        foreach(var tile in tiles)
        {
            var up = grid.GetAdjecentCell(tile.cell, Vector2Int.up);
            var down = grid.GetAdjecentCell(tile.cell, Vector2Int.down);
            var left = grid.GetAdjecentCell(tile.cell, Vector2Int.left);
            var right = grid.GetAdjecentCell(tile.cell, Vector2Int.right);

            if(up != null && CanMerge(tile, up.tile))
            {
                return false;
            }

            if(down != null && CanMerge(tile, down.tile))
            {
                return false;
            }

            if(left != null && CanMerge(tile, left.tile))
            {
                return false;
            }

            if(right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
    }
}
