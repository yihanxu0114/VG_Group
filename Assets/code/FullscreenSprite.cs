using UnityEngine;
using System.Collections.Generic;

public class RepeatBackground : MonoBehaviour
{
    [Header("引用")]
    public Camera targetCamera;
    public GameObject bgPrefab;

    private const float parallax = 0f;

    [Header("图片缩放")]
    public float tileScale = 1f;

    [Header("额外扩展格数（防止边缘露白）")]
    public int extraCells = 1;

    private float cellW, cellH;
    private int cols, rows;
    private Dictionary<Vector2Int, Transform> grid = new Dictionary<Vector2Int, Transform>();
    private Vector2Int lastOrigin = new Vector2Int(int.MaxValue, 0);

    void Start()
    {
        if (bgPrefab == null || targetCamera == null)
        {
            Debug.LogError("请在 Inspector 指定 bgPrefab 和 targetCamera");
            enabled = false;
            return;
        }

        var sr = bgPrefab.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("bgPrefab 需要带 SpriteRenderer");
            enabled = false;
            return;
        }

        Vector2 spriteLocalSize = sr.sprite.bounds.size;
        Vector3 prefabScale = bgPrefab.transform.localScale;
        cellW = spriteLocalSize.x * prefabScale.x * tileScale;
        cellH = spriteLocalSize.y * prefabScale.y * tileScale;

        float camH = targetCamera.orthographicSize * 2f;
        float camW = camH * targetCamera.aspect;

        cols = Mathf.CeilToInt(camW / cellW) + extraCells * 2 + 1;
        rows = Mathf.CeilToInt(camH / cellH) + extraCells * 2 + 1;

        if (cols % 2 == 0) cols++;
        if (rows % 2 == 0) rows++;

        RefreshGrid(GetOrigin());
    }

    void LateUpdate()
    {
        Vector3 camPos = targetCamera.transform.position;
        transform.position = new Vector3(
            camPos.x * parallax,
            camPos.y * parallax,
            transform.position.z
        );

        Vector2Int origin = GetOrigin();
        if (origin != lastOrigin)
            RefreshGrid(origin);
    }

    Vector2Int GetOrigin()
    {
        int ox = Mathf.RoundToInt(targetCamera.transform.position.x / cellW);
        int oy = Mathf.RoundToInt(targetCamera.transform.position.y / cellH);
        return new Vector2Int(ox, oy);
    }

    void RefreshGrid(Vector2Int origin)
    {
        lastOrigin = origin;

        int halfC = cols / 2;
        int halfR = rows / 2;

        var needed = new HashSet<Vector2Int>();
        for (int x = -halfC; x <= halfC; x++)
            for (int y = -halfR; y <= halfR; y++)
                needed.Add(new Vector2Int(origin.x + x, origin.y + y));

        var toRemove = new List<Vector2Int>();
        foreach (var kv in grid)
            if (!needed.Contains(kv.Key))
            {
                Destroy(kv.Value.gameObject);
                toRemove.Add(kv.Key);
            }
        foreach (var k in toRemove) grid.Remove(k);

        foreach (var cell in needed)
        {
            if (grid.ContainsKey(cell)) continue;

            GameObject go = Instantiate(bgPrefab);
            go.transform.localScale = bgPrefab.transform.localScale * tileScale;
            go.transform.position = new Vector3(cell.x * cellW, cell.y * cellH, 0f);
            go.transform.SetParent(transform, true);

            grid[cell] = go.transform;
        }
    }
}