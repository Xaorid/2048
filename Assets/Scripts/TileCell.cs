using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int pos {  get; set;}
    public Tile tile { get; set;}
    public bool isEmpty => tile == null;
    public bool isOccupied => tile != null;
}
