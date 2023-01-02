using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
  public Tilemap tilemap { get; private set; }
  public TetrominoData[] tetrominoes;
  public Piece activePiece { get; private set; }
  public Vector3Int spawnPosition;
  public Vector2Int boardSize = new Vector2Int(10, 20);
  public bool isGameOver;
  public MenuManager menuManager;
  public GameObject menuWrapper;

  public int score = 0;

  public RectInt Bounds
  {
    get
    {
      Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
      return new RectInt(position, this.boardSize);
    }
  }

  private void Awake()
  {
    this.tilemap = this.GetComponentInChildren<Tilemap>();
    this.activePiece = this.GetComponentInChildren<Piece>();
    isGameOver = false;
    for (int i = 0; i < tetrominoes.Length; i++)
    {
      tetrominoes[i].Initialize();
    }
  }

  private void Start()
  {
    SpawnTetromino();
  }

  public void SpawnTetromino()
  {
    int random = Random.Range(0, this.tetrominoes.Length);
    TetrominoData tetromino = this.tetrominoes[random];
    this.activePiece.Initialize(this, this.spawnPosition, tetromino);
    if (!IsValidPosition(this.activePiece, this.activePiece.position))
    {
      GameOver();
    }
    Set(activePiece);
  }

  public void GameOver()
  {
    isGameOver = true;
    menuManager.ShowScore(score);
    menuManager.gameObject.SetActive(true);
    menuWrapper.SetActive(false);
  }

  public void Set(Piece piece)
  {
    for (int i = 0; i < piece.cells.Length; i++)
    {
      Vector3Int tilePosition = piece.cells[i] + piece.position;
      tilemap.SetTile(tilePosition, piece.data.tile);
    }
  }

  public void Clear(Piece piece)
  {
    for (int i = 0; i < piece.cells.Length; i++)
    {
      Vector3Int tilePosition = piece.cells[i] + piece.position;
      tilemap.SetTile(tilePosition, null);
    }
  }

  public bool IsValidPosition(Piece piece, Vector3Int position)
  {
    RectInt bounds = this.Bounds;
    for (int i = 0; i < piece.cells.Length; i++)
    {
      Vector3Int tilePosition = piece.cells[i] + position;
      if (!bounds.Contains((Vector2Int)tilePosition))
      {
        return false;
      }
      if (tilemap.HasTile(tilePosition))
      {
        return false;
      }
    }
    return true;
  }

  public void ClearLines()
  {
    RectInt bounds = this.Bounds;
    int row = bounds.yMin;
    while (row < bounds.yMax)
    {
      if (IsFullRowAt(row))
      {
        ClearRowAt(row);
      }
      else
      {
        row++;
      }
    }
  }

  public void ClearRowAt(int row)
  {
    RectInt bounds = this.Bounds;
    for (int column = bounds.xMin; column < bounds.xMax; column++)
    {
      Vector3Int tilePosition = new Vector3Int(column, row, 0);
      tilemap.SetTile(tilePosition, null);
    }

    while (row < bounds.yMax)
    {
      for (int column = bounds.xMin; column < bounds.xMax; column++)
      {
        Vector3Int tileAbovePosition = new Vector3Int(column, row + 1, 0);
        TileBase tileAbove = this.tilemap.GetTile(tileAbovePosition);
        tileAbovePosition = new Vector3Int(column, row, 0);
        this.tilemap.SetTile(tileAbovePosition, tileAbove);
      }
      row++;
    }
    score += 10;
    menuWrapper.GetComponentInChildren<Text>().text = score.ToString();
    this.activePiece.stepDelay -= 0.10f;
  }

  public bool IsFullRowAt(int row)
  {
    RectInt bounds = this.Bounds;
    for (int column = bounds.xMin; column < bounds.xMax; column++)
    {
      Vector3Int tilePosition = new Vector3Int(column, row, 0);
      if (!tilemap.HasTile(tilePosition))
      {
        return false;
      }
    }
    return true;
  }
}
