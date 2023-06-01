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
    // [SerializeField] private GameObject justWall;
    // [SerializeField] private GameObject rightUpWall;
    // [SerializeField] private GameObject rightUpWallCorner;
    // [SerializeField] private GameObject rightWall;


    // TODO: заглушка, нет боковых спрайтов с тенями.
    // [SerializeField] private GameObject[] shadowRightFloorTiles;


    // префабы дверей
    [SerializeField] private GameObject entranceDoorPrefab;
    [SerializeField] private GameObject shopDoorPrefab;
    [SerializeField] private GameObject doorPrefab;

    // Размер матрицы пола
    [SerializeField] private int minWidth = 10;
    [SerializeField] private int minHeight = 8;
    [SerializeField] private int maxWidth = 20;
    [SerializeField] private int maxHeight = 16;
    [SerializeField] private float floorPrefabScaleMultiplier = 0.2f;

    [SerializeField] private int maximumRoomsOnFloor = 7;
    [SerializeField] private int minimumRoomsOnFloor = 5;
    [SerializeField] private int shopsOnFloor = 3;

    private const int NewRoomChance = 50;
    private const int ShopHeight = 8;
    private const int ShopWidth = 12;

    private float sizeMultiplier;
    private (float, float)[] tilesSizes;
    private Side lastEntranceSide = Side.Up;
    private int roomCounter;
    private Side[] nearRoomSides;
    private List<GameObject> doorTiles = new();
    private float xOffset;
    private float yOffset;
    private int width;
    private int height;
    private int shopsFound;
    private GameObject player;
    private GameObject camera;
    private GameObject[] doors;
    private List<GameObject> wallTiles = new();

    // Ссылка на созданные тайлы поля
    private GameObject[,] fieldTiles;

    private void Start()
    {
        sizeMultiplier = 0.4f * floorPrefabScaleMultiplier;

        ResizePrefabsInArray(floorTiles);
        ResizePrefabsInArray(shadowUpFloorTiles);

        doorPrefab.transform.localScale *= floorPrefabScaleMultiplier;
        entranceDoorPrefab.transform.localScale *= floorPrefabScaleMultiplier;
        
        // justWall.transform.localScale *= floorPrefabScaleMultiplier * 5;
        // rightUpWall.transform.localScale *= floorPrefabScaleMultiplier * 5;
        // rightWall.transform.localScale *= floorPrefabScaleMultiplier * 5;

        FillTileSizes();
        roomCounter = 1;
        nearRoomSides = GenerateNearRoomSides(lastEntranceSide);

        GenerateCurrentRoom();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearFloorTiles();
            ClearDoorTiles();
            ClearWallTiles();
            lastEntranceSide = Side.Up;
            roomCounter = 1;
            shopsFound = 0;
            nearRoomSides = GenerateNearRoomSides(lastEntranceSide);

            GenerateCurrentRoom();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var door in doors)
            {
                var intersected = Physics2D.OverlapBoxAll(player.transform.position,
                        player.GetComponent<BoxCollider2D>().size / 2, 0f)
                    .Any(obj => obj.gameObject == door &&
                                door.activeSelf); // определяем, пересекается ли игрок с текущей дверью

                if (intersected)
                {
                    var doorPos = door.transform.position;

                    switch (doorPos.x)
                    {
                        case 0 when doorPos.y > 0:
                            lastEntranceSide = Side.Down;
                            GenerateMoveToNextRoom();
                            Debug.Log("up");
                            break;
                        case 0 when doorPos.y < 0:
                            lastEntranceSide = Side.Up;
                            GenerateMoveToNextRoom();
                            Debug.Log("down");
                            break;
                        case > 0 when doorPos.y == 0:
                            lastEntranceSide = Side.Left;
                            GenerateMoveToNextRoom();
                            Debug.Log("right");
                            break;
                        case < 0 when doorPos.y == 0:
                            lastEntranceSide = Side.Right;
                            GenerateMoveToNextRoom();
                            Debug.Log("left");
                            break;
                    }
                }
            }
        }
    }

    private void TeleportToPosition(Vector3 newPosition)
    {
        player.transform.position = newPosition;
        camera.transform.position = newPosition;
    }

    private void ResizePrefabsInArray(IEnumerable<GameObject> prefabs)
    {
        foreach (var prefab in prefabs)
        {
            prefab.transform.localScale *= floorPrefabScaleMultiplier;
        }
    }

    private void FillTileSizes()
    {
        tilesSizes = floorTiles.Select(tile =>
                tile.GetComponent<Renderer>().bounds.size)
            .Select(tileSize => (tileSize.x, tileSize.y)).ToArray();
    }

    private void GenerateCurrentRoom()
    {
        height = Random.Range(minHeight, maxHeight) * 2;
        width = Random.Range(minWidth, maxWidth) * 2;
        xOffset = width * 0.5f * -sizeMultiplier;
        yOffset = height * 0.5f * -sizeMultiplier + sizeMultiplier;
        fieldTiles = new GameObject[height, width];
        var floorMatrix = CreateFloorMatrix();

        player = GameObject.FindGameObjectWithTag("Player"); // находим игрока по тегу
        camera = GameObject.FindGameObjectWithTag("MainCamera"); // находим камеру

        DrawFloorTiles(floorMatrix);
        MovePlayerToNextRoom();
        DrawDoorsTiles();
        // DrawWalls();

        doors = GameObject.FindGameObjectsWithTag("Door"); // находим все объекты с тегом "Door"
    }

    private void MovePlayerToNextRoom()
    {
        var up = new Vector3(0, height * 0.5f * sizeMultiplier - sizeMultiplier, 0);
        var left = new Vector3(-width * 0.5f * sizeMultiplier + sizeMultiplier, 0, 0);
        var right = new Vector3(width * 0.5f * sizeMultiplier - sizeMultiplier, 0, 0);
        var down = new Vector3(0, -height * 0.5f * sizeMultiplier + sizeMultiplier, 0);
        switch (lastEntranceSide)
        {
            case Side.Up:
                TeleportToPosition(up);
                break;
            case Side.Down:
                TeleportToPosition(down);
                break;
            case Side.Left:
                TeleportToPosition(left);
                break;
            case Side.Right:
                TeleportToPosition(right);
                break;
            default:
                throw new ArgumentOutOfRangeException();
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

    private void DrawWalls()
    {
        var currentX = xOffset;
        var currentY = yOffset;
        var maxX = xOffset + width * sizeMultiplier;
        var maxY = yOffset + height * sizeMultiplier;
        var modified = maxX - sizeMultiplier * 3;
        
        // while (currentX < modified)
        // {
        //     var position = new Vector3(currentX, maxY + sizeMultiplier * 3, 0);
        //     var tile = Instantiate(justWall, position, Quaternion.identity);
        //     wallTiles.Add(tile);
        //     currentX += sizeMultiplier * 2;
        //     
        //     if (currentX < modified) continue;
        //     
        //     position = new Vector3(currentX, maxY + sizeMultiplier * 3, 0);
        //     tile = Instantiate(rightUpWall, position, Quaternion.identity);
        //     wallTiles.Add(tile);
        //     currentX += sizeMultiplier * 2;
        //     
        //     position = new Vector3(currentX, maxY + sizeMultiplier * 3, 0);
        //     tile = Instantiate(rightUpWallCorner, position, Quaternion.identity);
        //     wallTiles.Add(tile);
        //     maxY -= sizeMultiplier * 2;
        // }
        //
        // while (currentY < maxY)
        // {
        //     var position = new Vector3(currentX, maxY + sizeMultiplier * 3, 0);
        //     var tile = Instantiate(rightWall, position, Quaternion.identity);
        //     wallTiles.Add(tile);
        //     maxY -= sizeMultiplier * 2;
        // }

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

    private void ClearWallTiles()
    {
        foreach (var wall in wallTiles)
        {
            Destroy(wall);
        }

        wallTiles.Clear();
    }

    private void GenerateMoveToNextRoom()
    {
        var a = Random.Range(1, 100);
        if (roomCounter < minimumRoomsOnFloor)
        {
            nearRoomSides = GenerateNearRoomSides(lastEntranceSide);
        }

        if ((a > NewRoomChance && maximumRoomsOnFloor > roomCounter &&
             roomCounter >= minimumRoomsOnFloor) || roomCounter < minimumRoomsOnFloor)
            nearRoomSides = GenerateNearRoomSides(lastEntranceSide);
        else
            nearRoomSides = Array.Empty<Side>();
        
        ClearWallTiles();
        ClearFloorTiles();
        ClearDoorTiles();
        roomCounter++;

        GenerateCurrentRoom();
    }
}