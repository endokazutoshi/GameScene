using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject _obstaclePrefab; // 通過できないブロックのプレハブ
    [SerializeField] private Vector2Int[] _obstaclePositions; // ブロックを配置する位置の配列

    private void Start()
    {
        GenerateObstacles();
    }

    private void GenerateObstacles()
    {
        foreach (Vector2Int position in _obstaclePositions)
        {
            Instantiate(_obstaclePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, transform);
        }
    }
}

