using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMining : MonoBehaviour
{
    [Header("採掘設定")]
    [SerializeField] private float miningRange = 3f; // 採掘可能な距離
    [SerializeField] private float miningSpeed = 1f; // 採掘速度
    
    [Header("デバッグ")]
    [SerializeField] private bool showMiningRange = true;
    
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        // マウス左クリックで採掘
        if (Mouse.current.leftButton.isPressed)
        {
            TryMine();
        }
    }
    
    void TryMine()
    {
        // マウスのワールド座標を取得
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        
        // プレイヤーからの距離をチェック
        float distance = Vector2.Distance(transform.position, mousePos);
        
        if (distance > miningRange)
        {
            // 範囲外
            return;
        }
        
        // マウス位置のブロックを取得
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);
        
        if (hit.collider != null)
        {
            Block block = hit.collider.GetComponent<Block>();
            
            if (block != null)
            {
                // ブロックを掘る
                block.Mine(miningSpeed * Time.deltaTime);
            }
        }
    }
    
    // デバッグ用：採掘範囲を可視化
    void OnDrawGizmos()
    {
        if (showMiningRange)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, miningRange);
        }
    }
}