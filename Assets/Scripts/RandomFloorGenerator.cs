using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Random = UnityEngine.Random;

internal enum Side
{
    Up,
    Right,
    Down,
    Left
}

public class RandomFloorGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] floorTiles;
    [SerializeField] private GameObject[] shadowUpFloorTiles;


    // TODO: заглушка, нет боковых спрайтов с тенями.
    // [SerializeField] private GameObject[] shadowRightFloorTiles;


    // префабы дверей
    [SerializeField] private GameObject entranceDoorPrefab;
    [SerializeField] private GameObject doorPrefab;

    // Размер матрицы пола
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 16;

    // Отступы для центрирования матрицы на сцене
    [SerializeField] private float xOffset = -4f;
    [SerializeField] private float yOffset = -2.8f;
    [SerializeField] private float sizeMultiplier = 0.4f;

    private (float, float)[] tilesSizes;
    private Side lastEntranceSide = Side.Up;
    private int newRoomChance = 100;
    private Side[] nearRoomSides;
    private List<GameObject> doorTiles = new ();

    // Ссылка на созданные тайлы поля
    private GameObject[,] fieldTiles;

    private void FillTileSizes()
    {
        tilesSizes = floorTiles.Select(tile =>
                tile.GetComponent<Renderer>().bounds.size)
            .Select(tileSize => (tileSize.x, tileSize.y)).ToArray();
    }

    private void Start()
    {
        FillTileSizes();
        // Создаем матрицу пола
        fieldTiles = new GameObject[height, width];
        nearRoomSides = GenerateNearRoomSides(lastEntranceSide);
        var floorMatrix = CreateFloorMatrix();
        DrawFloorTiles(floorMatrix);
        DrawDoorsTiles();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearFloorTiles();
            ClearDoorTiles();
            lastEntranceSide = Side.Up;
            newRoomChance = 100;
            var floorMatrix = CreateFloorMatrix();
            nearRoomSides = GenerateNearRoomSides(lastEntranceSide);
            DrawFloorTiles(floorMatrix);
            DrawDoorsTiles();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (lastEntranceSide == Side.Up)
            {
                Debug.Log("Нельзя вернуться назад");
            }
            else if (nearRoomSides.Contains(Side.Up))
            {
                Debug.Log("Вы успешно прошли вверх");
                lastEntranceSide = Side.Down;
                newRoomChance -= Random.Range(1, 4) * 3;
                GenerateMoveToNextRoom();
            }
            else
            {
                Debug.Log("Нельзя идти туда, где нет двери");
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (lastEntranceSide == Side.Down)
            {
                Debug.Log("Нельзя вернуться назад");
            }
            else if (nearRoomSides.Contains(Side.Down))
            {
                Debug.Log("Вы успешно прошли вниз");
                lastEntranceSide = Side.Up;
                GenerateMoveToNextRoom();
            }
            else
            {
                Debug.Log("Нельзя идти туда, где нет двери");
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (lastEntranceSide == Side.Left)
            {
                Debug.Log("Нельзя вернуться назад");
            }
            else if (nearRoomSides.Contains(Side.Left))
            {
                Debug.Log("Вы успешно прошли налево");
                lastEntranceSide = Side.Right;
                GenerateMoveToNextRoom();
            }
            else
            {
                Debug.Log("Нельзя идти туда, где нет двери");
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (lastEntranceSide == Side.Right)
            {
                Debug.Log("Нельзя вернуться назад");
            }
            else if (nearRoomSides.Contains(Side.Right))
            {
                Debug.Log("Вы успешно прошли направо");
                lastEntranceSide = Side.Left;
                GenerateMoveToNextRoom();
            }
            else
            {
                Debug.Log("Нельзя идти туда, где нет двери");
            }
        }


    }

    private int[,] CreateFloorMatrix()
    {
        var floorMatrix = new int[height, width];
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                // Выбираем случайный индекс префаба тайла пола из массива
                var index = Random.Range(0, floorTiles.Length);
                floorMatrix[i, j] = index;
            }
        }

        return floorMatrix;
    }

    private (GameObject, int, int) GenerateAnyTile(int index, Vector3 position, bool isShadowedUp, bool isShadowedRight)
    {
        var tiles = isShadowedUp ? shadowUpFloorTiles : floorTiles;

        // TODO: исправить заглушку
        // tiles = isShadowedRight ? shadowRightFloorTiles : tiles;

        var tile = Instantiate(tiles[index], position, Quaternion.identity);
        var dx = (int)Math.Round(tilesSizes[index].Item1 / sizeMultiplier);
        var dy = (int)Math.Round(tilesSizes[index].Item2 / sizeMultiplier);

        return (tile, dx, dy);
    }

    private (GameObject, int, int) GenerateSmallTile(int index, Vector3 position, bool isShadowedUp,
        bool isShadowedRight)
    {
        var (tile, dx, dy) = GenerateAnyTile(index, position, isShadowedUp, isShadowedRight);
        while (!(dx == dy && dy == 1))
        {
            Destroy(tile);
            index = Random.Range(0, floorTiles.Length);
            (tile, dx, dy) = GenerateAnyTile(index, position, isShadowedUp, isShadowedRight);
        }

        return (tile, dx, dy);
    }

    private void DrawFloorTiles(int[,] floorMatrix)
    {
        var onlySmall = new List<(int, int)>();
        var wasHere = new List<(int, int)>();
        // Отрисовываем пол на сцене
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                var isShadowedUp = i == height - 1;
                var isShadowedRight = j == width - 1;
                // Получаем индекс префаба тайла для текущего элемента матрицы
                var index = floorMatrix[i, j];

                // Получаем позицию для текущего тайла

                var position = new Vector3(j * sizeMultiplier + xOffset, i * sizeMultiplier + yOffset, 0);

                // Создаем новый тайл и устанавливаем его позицию
                var (tile, dx, dy) = GenerateAnyTile(index, position, isShadowedUp, isShadowedRight);


                if (wasHere.Contains((i, j)))
                {
                    Destroy(tile);
                    continue;
                }

                if (dx > 1 && j == width - 1)
                {
                    Destroy(tile);
                    (tile, dx, dy) = GenerateSmallTile(index, position, isShadowedUp, isShadowedRight);
                }

                if (i == 0)
                    onlySmall.Add((i, j));

                if (onlySmall.Contains((i, j)))
                {
                    Destroy(tile);
                    (tile, dx, dy) = GenerateSmallTile(index, position, isShadowedUp, isShadowedRight);
                }

                if (!(dx == dy && dy == 1))
                {
                    onlySmall.Add((i + 1, j + 1));
                    onlySmall.Add((i + 1, j));
                    onlySmall.Add((i, j + 1));

                    if (dx > 1)
                    {
                        wasHere.Add((i, j + 1));
                    }

                    if (dy > 1)
                    {
                        Destroy(fieldTiles[i - 1, j]);
                    }
                }

                if (!onlySmall.Contains((i, j)))
                    onlySmall.Add((i, j));


                fieldTiles[i, j] = tile;
            }
        }

        onlySmall.Clear();
        wasHere.Clear();
    }

    private void ClearFloorTiles()
    {
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                if (fieldTiles[i, j] != null)
                {
                    Destroy(fieldTiles[i, j]);
                }
            }
        }
    }

    private static Side[] GenerateNearRoomSides(Side entranceSide)
    {
        var roomCount = Random.Range(1, 4);

        var sides = new List<Side> { Side.Up, Side.Right, Side.Down, Side.Left };
        sides.Remove(entranceSide); // убираем сторону входа

        while (roomCount != sides.Count)
        {
            var randomIndex = Random.Range(0, sides.Count);
            sides.RemoveAt(randomIndex);
        }

        return sides.ToArray();
    }

    private void DrawDoorsTiles()
    {
        var up = new Vector3(0, height * 0.5f * sizeMultiplier, 0);
        var left = new Vector3(-width * 0.5f * sizeMultiplier, 0, 0);
        var right = new Vector3(width * 0.5f * sizeMultiplier, 0, 0);
        var down = new Vector3(0, -height * 0.5f * sizeMultiplier, 0);
        foreach (var side in nearRoomSides)
        {
            switch (side)
            {
                case Side.Up:
                    var upDoorTile = Instantiate(doorPrefab, up, Quaternion.identity);
                    doorTiles.Add(upDoorTile);
                    break;
                case Side.Right:
                    var rightDoorTile = Instantiate(doorPrefab, right, Quaternion.Euler(0, 0, -90));
                    doorTiles.Add(rightDoorTile);
                    break;
                case Side.Down:
                    var downDoorTile = Instantiate(doorPrefab, down, Quaternion.Euler(0, 0, 180));
                    doorTiles.Add(downDoorTile);
                    break;
                case Side.Left:
                    var leftDoorTile = Instantiate(doorPrefab, left, Quaternion.Euler(0, 0, 90));
                    doorTiles.Add(leftDoorTile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        var entranceDoorTile = lastEntranceSide switch
        {
            Side.Up => Instantiate(entranceDoorPrefab, up, Quaternion.identity),
            Side.Right => Instantiate(entranceDoorPrefab, right, Quaternion.Euler(0, 0, -90)),
            Side.Down => Instantiate(entranceDoorPrefab, down, Quaternion.Euler(0, 0, 180)),
            Side.Left => Instantiate(entranceDoorPrefab, left, Quaternion.Euler(0, 0, 90)),
            _ => throw new ArgumentOutOfRangeException()
        };
        doorTiles.Add(entranceDoorTile);
    }
    
    private void ClearDoorTiles()
    {
        foreach (var door in doorTiles)
        {
            Destroy(door);
        }
    }

    private void GenerateMoveToNextRoom()
    {
        nearRoomSides = newRoomChance <= 0 ? Array.Empty<Side>() : GenerateNearRoomSides(lastEntranceSide);
                
        ClearFloorTiles();
        ClearDoorTiles();
        var floorMatrix = CreateFloorMatrix();
        DrawFloorTiles(floorMatrix);
        DrawDoorsTiles();
    }
}