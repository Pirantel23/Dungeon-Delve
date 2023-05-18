using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Random = UnityEngine.Random;

public class RandomFloorGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] floorTiles;
    [SerializeField] private GameObject[] shadowFloorTiles;

    // Размер матрицы пола
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 16;

    // Отступы для центрирования матрицы на сцене
    [SerializeField] private float xOffset = -4f;
    [SerializeField] private float yOffset = -2.8f;
    [SerializeField] private float sizeMultiplier = 0.4f;

    private (float, float)[] tilesSizes;

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
        var floorMatrix = CreateFloorMatrix();
        DrawFloorTiles(floorMatrix);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.R)) return;

        ClearFloorTiles();
        var floorMatrix = CreateFloorMatrix();
        DrawFloorTiles(floorMatrix);
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

    private (GameObject, int, int) GenerateAnyTile(int index, Vector3 position)
    {
        var tile = Instantiate(floorTiles[index], position, Quaternion.identity);
        var dx = (int)Math.Round(tilesSizes[index].Item1 / sizeMultiplier);
        var dy = (int)Math.Round(tilesSizes[index].Item2 / sizeMultiplier);

        return (tile, dx, dy);
    }

    private (GameObject, int, int) GenerateSmallTile(int index, Vector3 position)
    {
        var (tile, dx, dy) = GenerateAnyTile(index, position);
        while (!(dx == dy && dy == 1))
        {
            Destroy(tile);
            index = Random.Range(0, floorTiles.Length);
            (tile, dx, dy) = GenerateAnyTile(index, position);
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
                // Получаем индекс префаба тайла для текущего элемента матрицы
                var index = floorMatrix[i, j];

                // Получаем позицию для текущего тайла

                var position = new Vector3(j * sizeMultiplier + xOffset, i * sizeMultiplier + yOffset, 0);

                // Создаем новый тайл и устанавливаем его позицию
                var (tile, dx, dy) = GenerateAnyTile(index, position);


                if (wasHere.Contains((i, j)))
                {
                    Destroy(tile);
                    continue;
                }

                if (dx > 1 && j == width - 1)
                {
                    Destroy(tile);
                    (tile, dx, dy) = GenerateSmallTile(index, position);
                }

                if (i == 0)
                    onlySmall.Add((i, j));

                if (onlySmall.Contains((i, j)))
                {
                    Destroy(tile);
                    (tile, dx, dy) = GenerateSmallTile(index, position);
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
}