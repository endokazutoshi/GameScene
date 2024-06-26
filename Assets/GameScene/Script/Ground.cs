using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Ground : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject[] _goalPrefabs; // ゴール用の3つのPrefabをセットするための配列
    [SerializeField] private GameObject _PIN;
    [SerializeField] private GameObject _deadPrefab;
    [SerializeField] private GameObject[] _switchPrefabs;
    [SerializeField] private GameObject _leverPrefab; // レバーのPrefabを追加

    [SerializeField] private float _goalSceneTime;

    const int _nLengthStart = 1;
    const int _nWidthStart = 1;
    const int _nLengthEnd = 22;
    const int _nWidthEnd = 9;
    const int _nCenter = 11;

    [SerializeField] private int _length;
    [SerializeField] private int _width;

    // 2Dマップデータ:
    //0=壁、1=床、2=ゴール、3=暗号、4=死亡、5=スイッチ、6=移動キーぐちゃぐちゃマス、7=レバー
    public static int[,] map = {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0},
        {0, 1, 2, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 2, 1, 0, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 3, 0, 1, 0, 0},
        {0, 1, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0},
        {0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 1, 0},
        {0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0},
        {0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0},
        {0, 3, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    private void Start()
    {
        // ステージの移動マス、壁マスの配置の処理
        for (int length = 0; length < _length; length++)
        {
            for (int width = 0; width < _width; width++)
            {
                int tileType;
                if (length >= _nLengthStart && length < _nLengthEnd && width >= _nWidthStart && width < _nWidthEnd) // 横の座標が0から22の範囲の場合
                {
                    tileType = map[width, length]; // ここでMapの情報を取得するためにtileTypeにmapの情報を入れる
                }
                else
                {
                    tileType = 0; // 外枠の場合は壁とする
                }

                if (tileType == 0) // mapの情報で0だった場合は壁を配置する
                {
                    Instantiate(_wallPrefab, new Vector3(length, width, 0), Quaternion.identity, transform); // 壁の表示
                }

                if (tileType == 1) // mapの情報で1だった場合は進める
                {
                    Instantiate(_tilePrefab, new Vector3(length, width, 0), Quaternion.identity, transform); // 移動できるマス(タイル)の表示
                }

                if (tileType == 2) // mapの情報で2だった場合はゴールする(条件として両方のPlayerがゴールしないといけない)
                {
                    Vector3 goalPosition = new Vector3(length, width, 0);
                    StartCoroutine(SwitchGoalPrefab(goalPosition));
                }

                if (tileType == 3) // mapの情報で3だった場合はギミックを展開する
                {
                    PlacePassword(new Vector3(length, width, 0));
                }

                if (tileType == 4)
                {
                    Vector3 deadPosition = new Vector3(length, width, 0);
                }

                if (tileType == 5)
                {
                    Switch(new Vector3(length, width, 0));
                }

                // レバーの追加
                if (tileType == 7)
                {
                    Vector3 leverPosition = new Vector3(length, width, 0);
                    PlaceLever(leverPosition);
                }
            }
        }
    }

    // レバーを配置するメソッド
    private void PlaceLever(Vector3 position)
    {
        Instantiate(_leverPrefab, position, Quaternion.identity, transform);
        Debug.Log("レバーを配置: " + position);
    }

    IEnumerator SwitchGoalPrefab(Vector3 position)
    {
        GameObject goalInstance = null;
        int goalIndex = 0;

        while (true)
        {
            // 現在のPrefabを削除
            if (goalInstance != null)
            {
                Destroy(goalInstance);
            }

            // 次のPrefabをインスタンス化
            goalInstance = Instantiate(_goalPrefabs[goalIndex], position, Quaternion.identity, transform);

            // インデックスを更新
            goalIndex = (goalIndex + 1) % _goalPrefabs.Length;

            // 1秒待つ
            yield return new WaitForSeconds(_goalSceneTime);
        }
    }

    private void PlacePassword(Vector3 position)
    {
        Instantiate(_PIN, position, Quaternion.identity, transform);
        Debug.Log("ギミックマスを配置: " + position);

        // 暗証番号の周囲でスペースキーが押されたら暗証番号パネルを表示
        if (IsSpaceKeyPressedNearPosition(position))
        {
            // FreezeAndInputクラスのインスタンスを取得
            FreezeAndInput freezeAndInput = FindObjectOfType<FreezeAndInput>();
            if (freezeAndInput != null)
            {
                // Freezeメソッドを呼び出す
                freezeAndInput.Freeze();
            }
        }
    }

    // 指定された位置の周囲でスペースキーが押されたかどうかをチェックするメソッド
    private bool IsSpaceKeyPressedNearPosition(Vector3 position)
    {
        // スペースキーが押されているかつ、指定された位置の周囲にプレイヤーが存在するかをチェック
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 1f); // 指定された位置の半径1の円領域内のコライダーを取得
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player")) // プレイヤータグを持つオブジェクトが存在する場合
                {
                    return true; // スペースキーが押されたかつプレイヤーが周囲に存在する場合はtrueを返す
                }
            }
        }
        return false;
    }

    private void Switch(Vector3 position)
    {
        if (_switchPrefabs.Length > 0) // _switchPrefabsが空でないことを確認
        {
            Instantiate(_switchPrefabs[0], position, Quaternion.identity, transform); // とりあえず最初のPrefabを配置
            Debug.Log("スイッチを配置: " + position);
        }
        else
        {
            Debug.LogError("スイッチのPrefabがセットされていません。_switchPrefabsにPrefabをセットしてください。");
        }
    }

    public int GetTileTypeAtPosition(Vector3 position)
    {
        // position を整数に変換し、対応するタイルタイプを返す
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        return map[y, x];
    }

    public int Width
    {
        get { return map.GetLength(1); }
    }
    public int Length
    {
        get { return map.GetLength(0); }
    }
}
