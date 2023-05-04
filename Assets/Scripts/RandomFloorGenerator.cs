using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class RandomFloorGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] floorTiles;

    // Размер матрицы пола
    [SerializeField] private int width = 32;
    [SerializeField] private int height = 18;

    // Отступы для центрирования матрицы на сцене
    [SerializeField] private float xOffset = -2f;
    [SerializeField] private float yOffset = -2f;
    [SerializeField] private float sizeMultiplier = 0.8f;

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

    private void DrawFloorTiles(int[,] floorMatrix)
    {
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
                var tile = Instantiate(floorTiles[index], position, Quaternion.identity);
                var dx = (int)Math.Round(tilesSizes[index].Item1 / sizeMultiplier);
                var dy = (int)Math.Round(tilesSizes[index].Item2 / sizeMultiplier);
                for (var k = 0; k < dx; k++)
                {
                    for (var l = 0; l < dy; l++)
                    {
                        wasHere.Add((i + dx, j + dy));
                        
                    }
                }
                
                

                fieldTiles[i, j] = tile;
            }
        }
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