using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

internal enum Side
{
    Up,
    Right,
    Down,
    Left
}

public class FloorMapGeneration : MonoBehaviour
{
    // префабы пола
    [SerializeField] private GameObject[] floorTiles;
    [SerializeField] private GameObject[] shadowUpFloorTiles;
    [SerializeField] private GameObject[] shadowRightFloorTiles;
    [SerializeField] private GameObject[] shadowCornerFloorTiles;

    // префабы стен
    [SerializeField] private GameObject upWallTile;
    [SerializeField] private GameObject upRightWallTile;
    [SerializeField] private GameObject upRightCornerTile;
    [SerializeField] private GameObject rightWallTile;
    [SerializeField] private GameObject downRightCornerTile;
    [SerializeField] private GameObject downWallTile;
    [SerializeField] private GameObject downLeftCornerTile;
    [SerializeField] private GameObject leftWallTile;
    [SerializeField] private GameObject upLeftCorner;


    // префабы дверей
    [SerializeField] private GameObject entranceDoorPrefab;
    [SerializeField] private GameObject entranceLeftDoorPrefab;
    [SerializeField] private GameObject entranceRightDoorPrefab;

    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject doorLeftPrefab;
    [SerializeField] private GameObject doorRightPrefab;

    [SerializeField] private GameObject shopDoorPrefab;
    [SerializeField] private GameObject shopDoorLeftPrefab;
    [SerializeField] private GameObject shopDoorRightPrefab;

    [SerializeField] private GameObject bossDoorPrefab;

    // Размер матрицы пола
    [SerializeField] private int minWidth = 8;
    [SerializeField] private int minHeight = 5;
    [SerializeField] private int maxWidth = 16;
    [SerializeField] private int maxHeight = 12;

    // размер этажа
    [SerializeField] private int maximumRoomsOnFloor = 7;
    [SerializeField] private int minimumRoomsOnFloor = 5;

    // количество мобов и спавнер
    [SerializeField] private int minAmount = 2;
    [SerializeField] private int maxAmount = 4;

    [SerializeField] private GameObject spawnerPrefab;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject shop;

    private const int NewRoomChance = 50;


    private const float SizeMultiplier = 0.08f;
    private (float, float)[] tilesSizes;
    private Side lastEntranceSide = Side.Up;
    private int roomCounter;
    private Side[] nearRoomSides;
    private float xOffset;
    private float yOffset;
    private int width;
    private int height;

    private int shopIsFound;

    // объекты сцены
    private GameObject player;
    private GameObject mainCamera;
    private GameObject[] doors;

    // Ссылка на созданные тайлы поля
    private GameObject[,] fieldTiles;
    private readonly List<GameObject> wallTiles = new();
    private readonly List<GameObject> doorTiles = new();
    private bool isBossRoom;
    private bool isShopRoom;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        TeleportPlayerToPosition(new Vector3(0, 0, 0));

        FillTileSizes();


        roomCounter = 1;
        nearRoomSides = GenerateNearRoomSides(lastEntranceSide);

        GenerateCurrentRoom();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length != 0) return;

            foreach (var door in doors)
            {
                var intersected = Physics2D.OverlapBoxAll(player.transform.position,
                        player.GetComponent<BoxCollider2D>().size / 2, 0f)
                    .Any(obj => obj.gameObject == door &&
                                door.activeSelf); // определяем, пересекается ли игрок с текущей дверью
                if (intersected)
                {
                    var doorPos = door.transform.position;

                    isShopRoom = false;

                    switch (doorPos.x)
                    {
                        case 0 when doorPos.y > 0:
                            lastEntranceSide = Side.Down;
                            GenerateMoveToNextRoom();
                            break;
                        case 0 when doorPos.y < 0:
                            lastEntranceSide = Side.Up;
                            GenerateMoveToNextRoom();
                            break;
                        case > 0 when doorPos.y == 0:
                            lastEntranceSide = Side.Left;
                            GenerateMoveToNextRoom();
                            break;
                        case < 0 when doorPos.y == 0:
                            lastEntranceSide = Side.Right;
                            GenerateMoveToNextRoom();
                            break;
                    }
                }

                var shopDoor = GameObject.FindGameObjectWithTag("ShopDoor");
                if (shopDoor is not null)
                {
                    if (shopDoor.GetComponent<BoxCollider2D>().bounds.Intersects(
                            player.GetComponent<BoxCollider2D>().bounds))
                    {
                        var doorPos = shopDoor.transform.position;

                        isShopRoom = true;

                        switch (doorPos.x)
                        {
                            case 0 when doorPos.y > 0:
                                lastEntranceSide = Side.Down;
                                GenerateMoveToNextRoom();
                                break;
                            case 0 when doorPos.y < 0:
                                lastEntranceSide = Side.Up;
                                GenerateMoveToNextRoom();
                                break;
                            case > 0 when doorPos.y == 0:
                                lastEntranceSide = Side.Left;
                                GenerateMoveToNextRoom();
                                break;
                            case < 0 when doorPos.y == 0:
                                lastEntranceSide = Side.Right;
                                GenerateMoveToNextRoom();
                                break;
                        }
                    }
                }

                var bossDoor = GameObject.FindGameObjectWithTag("BossDoor");
                if (bossDoor is not null)
                {
                    if (bossDoor.GetComponent<BoxCollider2D>().bounds.Intersects(
                            player.GetComponent<BoxCollider2D>().bounds))
                    {
                        var doorPos = bossDoor.transform.position;

                        isBossRoom = true;

                        switch (doorPos.x)
                        {
                            case 0 when doorPos.y > 0:
                                lastEntranceSide = Side.Down;
                                GenerateMoveToNextRoom();
                                break;
                            case 0 when doorPos.y < 0:
                                lastEntranceSide = Side.Up;
                                GenerateMoveToNextRoom();
                                break;
                        }
                    }
                }
            }
        }
    }

    private static void ClearAndDestroyTileList(List<GameObject> tiles)
    {
        foreach (var tile in tiles)
        {
            Destroy(tile);
        }

        tiles.Clear();
    }

    private void FillTileSizes()
    {
        tilesSizes = floorTiles.Select(tile =>
                tile.GetComponent<Renderer>().bounds.size)
            .Select(tileSize => (tileSize.x, tileSize.y)).ToArray();
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

    private void GenerateCurrentRoom()
    {
        height = Random.Range(minHeight, maxHeight) * 2;
        width = Random.Range(minWidth, maxWidth) * 2;
        xOffset = width * 0.5f * -SizeMultiplier;
        yOffset = height * 0.5f * -SizeMultiplier + SizeMultiplier;
        fieldTiles = new GameObject[height, width];
        var floorMatrix = CreateFloorMatrix();
        DrawFloorTiles(floorMatrix);
        DrawWallTiles();
        DrawDoorsTiles();
        if (!(isShopRoom || isBossRoom))
        {
            Spawn(maxAmount, minAmount);
            minAmount++;
            maxAmount = minAmount * 2;
        }

        if (isShopRoom)
        {
            isShopRoom = false;
            Instantiate(shop, Vector2.zero, Quaternion.identity);
        }
            

        if (isBossRoom)
        {
            isBossRoom = false;
            Instantiate(boss, Vector2.zero, Quaternion.identity).GetComponent<Boss>().target = player.transform;
        }

        doors = GameObject.FindGameObjectsWithTag("Door"); // находим все объекты с тегом "Door"

        MovePlayerToNextRoom();
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

                var position = new Vector3(j * SizeMultiplier + xOffset, i * SizeMultiplier + yOffset, 0);

                // Создаем новый тайл и устанавливаем его позицию
                var (tile, dx, dy) = i >= height - 2 && j >= width - 2
                    ? GenerateSmallTile(index, position, isShadowedUp, isShadowedRight)
                    : GenerateAnyTile(index, position, isShadowedUp, isShadowedRight);


                if (wasHere.Contains((i, j)))
                {
                    Destroy(tile);
                    fieldTiles[i, j] = null;
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
                        if (j == width - 2)
                        {
                            Destroy(tile);
                            (tile, _, dy) = GenerateAnyTile(index, position, isShadowedUp, true);
                        }
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

    private void DrawWallTiles()
    {
        var currentX = xOffset;
        var currentY = yOffset;
        var maxX = xOffset + width * SizeMultiplier;
        var maxY = yOffset + height * SizeMultiplier;
        var modified = maxX - SizeMultiplier * 3;

        while (currentX < modified)
        {
            var position = new Vector3(currentX, maxY + SizeMultiplier * 3, 0);
            var tile = Instantiate(upWallTile, position, Quaternion.identity);
            wallTiles.Add(tile);
            currentX += SizeMultiplier * 2;

            if (currentX < modified) continue;

            position = new Vector3(currentX, maxY + SizeMultiplier * 3, 0);
            tile = Instantiate(upRightWallTile, position, Quaternion.identity);
            wallTiles.Add(tile);
            currentX += SizeMultiplier * 2;

            position = new Vector3(currentX, maxY + SizeMultiplier * 3, 0);
            tile = Instantiate(upRightCornerTile, position, Quaternion.identity);
            wallTiles.Add(tile);
            maxY -= SizeMultiplier * 2;
        }

        while (currentY - SizeMultiplier < maxY)
        {
            var position = new Vector3(currentX, maxY + SizeMultiplier * 3, 0);
            var tile = Instantiate(rightWallTile, position, Quaternion.identity);
            wallTiles.Add(tile);
            maxY -= SizeMultiplier * 2;

            if (currentY - SizeMultiplier < maxY) continue;

            position = new Vector3(currentX, maxY + SizeMultiplier * 3, 0);
            tile = Instantiate(downRightCornerTile, position, Quaternion.identity);
            wallTiles.Add(tile);
            maxY -= SizeMultiplier * 2;
        }

        while (currentX - SizeMultiplier > xOffset)
        {
            var position = new Vector3(currentX, maxY + SizeMultiplier * 3, 0);
            var tile = Instantiate(downWallTile, position, Quaternion.identity);
            wallTiles.Add(tile);
            currentX -= SizeMultiplier * 2;

            if (currentX - SizeMultiplier > xOffset) continue;

            maxY += SizeMultiplier * 2;

            position = new Vector3(currentX, maxY + SizeMultiplier * 3, 0);
            tile = Instantiate(downLeftCornerTile, position, Quaternion.identity);
            wallTiles.Add(tile);
        }

        while (maxY < -yOffset - SizeMultiplier * 3)
        {
            var position = new Vector3(currentX, maxY + SizeMultiplier * 3, 0);
            var tile = Instantiate(leftWallTile, position, Quaternion.identity);
            wallTiles.Add(tile);
            maxY += SizeMultiplier * 2;

            if (maxY < -yOffset - SizeMultiplier * 3) continue;

            position = new Vector3(currentX, maxY + SizeMultiplier * 3, 0);
            tile = Instantiate(upLeftCorner, position, Quaternion.identity);
            wallTiles.Add(tile);
        }
    }

    private void DrawDoorsTiles()
    {
        var up = new Vector3(0, height * 0.5f * SizeMultiplier, 0);
        var left = new Vector3(-width * 0.5f * SizeMultiplier, 0, 0);
        var right = new Vector3(width * 0.5f * SizeMultiplier, 0, 0);
        var down = new Vector3(0, -height * 0.5f * SizeMultiplier, 0);

        var needShop = roomCounter == 3;
        var needBoss = nearRoomSides.Length == 0;

        foreach (var side in nearRoomSides)
        {
            switch (side)
            {
                case Side.Up:
                    GameObject upDoorTile;
                    if (needShop)
                    {
                        needShop = false;
                        upDoorTile = Instantiate(shopDoorPrefab, up, Quaternion.identity);
                    }
                    else if (needBoss)
                    {
                        needBoss = false;
                        upDoorTile = Instantiate(bossDoorPrefab, up, Quaternion.identity);
                    }
                    else
                    {
                        upDoorTile = Instantiate(doorPrefab, up, Quaternion.identity);
                    }

                    doorTiles.Add(upDoorTile);
                    break;
                case Side.Right:
                    GameObject rightDoorTile;
                    if (needShop)
                    {
                        needShop = false;
                        rightDoorTile = Instantiate(shopDoorRightPrefab, right, Quaternion.identity);
                    }
                    else
                    {
                        rightDoorTile = Instantiate(doorRightPrefab, right, Quaternion.identity);
                    }

                    doorTiles.Add(rightDoorTile);
                    break;
                case Side.Down:
                    GameObject downDoorTile;
                    if (needShop)
                    {
                        needShop = false;
                        downDoorTile = Instantiate(shopDoorPrefab, down, Quaternion.Euler(0, 0, 180));
                    }
                    else if (needBoss)
                    {
                        needBoss = false;
                        downDoorTile = Instantiate(bossDoorPrefab, up, Quaternion.Euler(0, 0, 180));
                    }
                    else
                    {
                        downDoorTile = Instantiate(doorPrefab, down, Quaternion.Euler(0, 0, 180));
                    }

                    doorTiles.Add(downDoorTile);
                    break;
                case Side.Left:
                    GameObject leftDoorTile;
                    if (needShop)
                    {
                        needShop = false;
                        leftDoorTile = Instantiate(shopDoorLeftPrefab, left, Quaternion.identity);
                    }
                    else
                    {
                        leftDoorTile = Instantiate(doorLeftPrefab, left, Quaternion.identity);
                    }

                    doorTiles.Add(leftDoorTile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        var entranceDoorTile = lastEntranceSide switch
        {
            Side.Up => Instantiate(entranceDoorPrefab, up, Quaternion.identity),
            Side.Right => Instantiate(entranceRightDoorPrefab, right, Quaternion.identity),
            Side.Down => Instantiate(entranceDoorPrefab, down, Quaternion.Euler(0, 0, 180)),
            Side.Left => Instantiate(entranceLeftDoorPrefab, left, Quaternion.identity),
            _ => throw new ArgumentOutOfRangeException()
        };
        doorTiles.Add(entranceDoorTile);
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

    private (GameObject, int, int) GenerateAnyTile(int index, Vector3 position, bool isShadowedUp, bool isShadowedRight)
    {
        var tiles = isShadowedUp ? shadowUpFloorTiles : floorTiles;
        tiles = isShadowedRight ? shadowRightFloorTiles : tiles;
        tiles = isShadowedRight && isShadowedUp ? shadowCornerFloorTiles : tiles;

        // TODO: исправить заглушку
        // tiles = isShadowedRight ? shadowRightFloorTiles : tiles;

        var tile = Instantiate(tiles[index], position, Quaternion.identity);
        var dx = (int)Math.Round(tilesSizes[index].Item1 / SizeMultiplier);
        var dy = (int)Math.Round(tilesSizes[index].Item2 / SizeMultiplier);

        return (tile, dx, dy);
    }

    private void ClearFloorTiles()
    {
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                if (fieldTiles[i, j] is not null)
                {
                    Destroy(fieldTiles[i, j]);
                }
            }
        }
    }

    private void TeleportPlayerToPosition(Vector3 newPosition)
    {
        player.transform.position = newPosition;
        mainCamera.transform.position = newPosition;
    }

    private void GenerateMoveToNextRoom()
    {
        var roomChance = Random.Range(1, 100);

        if (roomCounter < minimumRoomsOnFloor)
        {
            nearRoomSides = GenerateNearRoomSides(lastEntranceSide);
        }

        if ((roomChance < NewRoomChance && maximumRoomsOnFloor > roomCounter &&
             roomCounter >= minimumRoomsOnFloor) || roomCounter < minimumRoomsOnFloor)
            nearRoomSides = GenerateNearRoomSides(lastEntranceSide);
        else
        {
            nearRoomSides = Array.Empty<Side>();
            isBossRoom = true;
        }

        ClearAndDestroyTileList(doorTiles);
        ClearAndDestroyTileList(wallTiles);
        ClearFloorTiles();

        roomCounter++;

        GenerateCurrentRoom();
    }

    private void MovePlayerToNextRoom()
    {
        var up = new Vector3(0, height * 0.5f * SizeMultiplier - SizeMultiplier, 0);
        var left = new Vector3(-width * 0.5f * SizeMultiplier + SizeMultiplier, 0, 0);
        var right = new Vector3(width * 0.5f * SizeMultiplier - SizeMultiplier, 0, 0);
        var down = new Vector3(0, -height * 0.5f * SizeMultiplier + SizeMultiplier, 0);
        switch (lastEntranceSide)
        {
            case Side.Up:
                TeleportPlayerToPosition(up);
                break;
            case Side.Down:
                TeleportPlayerToPosition(down);
                break;
            case Side.Left:
                TeleportPlayerToPosition(left);
                break;
            case Side.Right:
                TeleportPlayerToPosition(right);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Spawn(int maxAmount, int minAmount)
    {
        var position = new Vector2(xOffset, yOffset);

        var tile = Instantiate(spawnerPrefab, position, Quaternion.identity).GetComponent<SpawnerScript>();
        tile.maxAmount = maxAmount;
        tile.minAmount = minAmount;
        tile.topRightCorner = new Vector2(width, height) * SizeMultiplier;
    }
}