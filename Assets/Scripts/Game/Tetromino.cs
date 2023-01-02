using UnityEngine;
using System;
using UnityEngine.Tilemaps;
public enum TetrominoType
{
  I,
  J,
  L,
  O,
  S,
  T,
  Z
}

[Serializable]
public struct TetrominoData
{
  public TetrominoType type;
  public Tile tile;
  public Vector2Int[] cells { get; private set; }
  public Vector2Int[,] wallKicks { get; private set; }

  public void Initialize()
  {
    this.cells = Data.Cells[this.type];
    this.wallKicks = Data.WallKicks[this.type];
  }
}