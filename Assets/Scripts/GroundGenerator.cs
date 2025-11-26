using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GroundGenerator : MonoBehaviour
{
    [Header("アイテムデータベース")]
    public List<ItemData> itemDatabase = new List<ItemData>(); // すべてのItemDataを登録
    
    [Header("生成情報")]
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    
    private TextAsset csvFile; // CSVファイル
    private List<string[]> csvData = new List<string[]>(); // CSVファイルの中身を入れるリスト
    private int[,] tileMap;
    private Dictionary<int, ItemData> itemDictionary;
    
    void Start()
    {
        BuildItemDictionary();
        LoadMapFromCSV();
        GenerateGround();
    }
    
    // ItemDataのIDから検索できるようにDictionaryを作成
    void BuildItemDictionary()
    {
        itemDictionary = new Dictionary<int, ItemData>();
        
        foreach (ItemData item in itemDatabase)
        {
            if (item != null)
            {
                if (itemDictionary.ContainsKey(item.itemId))
                {
                    Debug.LogWarning($"重複するitemId: {item.itemId} ({item.itemName})");
                }
                else
                {
                    itemDictionary.Add(item.itemId, item);
                }
            }
        }
        
        Debug.Log($"アイテムデータベース構築完了: {itemDictionary.Count}個のアイテム");
    }
    
    void LoadMapFromCSV()
    {
        csvFile = Resources.Load("Map") as TextAsset; // ResourcesにあるCSVファイルを格納
        
        if (csvFile == null)
        {
            Debug.LogError("Resources/Map.csv が見つかりません！");
            return;
        }
        
        StringReader reader = new StringReader(csvFile.text); // TextAssetをStringReaderに変換
        
        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine(); // 1行ずつ読み込む
            if (!string.IsNullOrWhiteSpace(line))
            {
                csvData.Add(line.Split(',')); // csvDataリストに追加する
            }
        }
        
        mapHeight = csvData.Count;
        
        if (mapHeight == 0)
        {
            Debug.LogError("CSVファイルが空です！");
            return;
        }
        
        // 最初の行から横幅を取得
        mapWidth = csvData[0].Length;
        
        // 二次元配列を初期化
        tileMap = new int[mapWidth, mapHeight];
        
        // CSVデータを二次元配列に変換
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < csvData[y].Length && x < mapWidth; x++)
            {
                if (int.TryParse(csvData[y][x].Trim(), out int tileId))
                {
                    tileMap[x, y] = tileId;
                }
                else
                {
                    tileMap[x, y] = 0; // パースできない場合は空気
                }
            }
        }
        
        Debug.Log($"マップ読み込み完了: 幅{mapWidth} x 高さ{mapHeight}");
    }
    
    void GenerateGround()
    {
        if (tileMap == null)
        {
            Debug.LogError("マップデータが読み込まれていません");
            return;
        }
        
        // CSVの上から下へ、Unityでは上から下へ配置
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int itemId = tileMap[x, y];
                
                // itemId 0 は空気なのでスキップ
                if (itemId == 0) continue;
                
                // Unity座標: Y軸を反転（CSVの上 = Unityの上）
                Vector3 position = new Vector3(x, mapHeight - 1 - y, 0);
                
                // itemIdからItemDataを取得
                if (itemDictionary.TryGetValue(itemId, out ItemData itemData))
                {
                    // ブロックとして配置可能かチェック
                    if (itemData.isPlaceable && itemData.blockPrefab != null)
                    {
                        Instantiate(itemData.blockPrefab, position, Quaternion.identity, transform);
                    }
                    else
                    {
                        Debug.LogWarning($"アイテムID {itemId} ({itemData.itemName}) は配置できません");
                    }
                }
                else
                {
                    Debug.LogWarning($"アイテムID {itemId} がデータベースに存在しません");
                }
            }
        }
        
        Debug.Log("地面生成完了");
    }
    
    // デバッグ用: マップをConsoleに表示
    [ContextMenu("マップを表示")]
    void DebugPrintMap()
    {
        if (tileMap == null)
        {
            Debug.Log("マップがまだ読み込まれていません");
            return;
        }
        
        string mapString = "=== タイルマップ ===\n";
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                mapString += tileMap[x, y] + ",";
            }
            mapString += "\n";
        }
        Debug.Log(mapString);
    }
    
    // デバッグ用: 登録されているアイテムを表示
    [ContextMenu("アイテムデータベースを表示")]
    void DebugPrintItemDatabase()
    {
        Debug.Log("=== アイテムデータベース ===");
        foreach (var item in itemDatabase)
        {
            if (item != null)
            {
                Debug.Log($"ID: {item.itemId}, 名前: {item.itemName}, 配置可能: {item.isPlaceable}");
            }
        }
    }
}