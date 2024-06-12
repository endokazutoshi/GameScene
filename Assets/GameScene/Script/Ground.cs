using System.Collections;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject[] _goalPrefabs; // ゴール用の3つのPrefabをセットするための配列
    [SerializeField] private GameObject _PIN;
    [SerializeField] private float _goalSceneTime;

    const int _nLengthStart = 1;
    const int _nWidthStart = 1;
    const int _nLengthEnd = 22;
    const int _nWidthEnd = 9;
    const int _nCenter = 11;

    [SerializeField] private int _length;
    [SerializeField] private int _width;

    // 2Dマップデータ: 0は壁、1は移動マス、2はゴールマス、3以降はギミックマス
    public static int[,] map = {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
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

                if (tileType == 0) // mapの情報で0だった場合は進めない
                {
                    Instantiate(_wallPrefab, new Vector3(length, width, 0), Quaternion.identity, transform); // 壁の表示
                    Debug.Log("壁マスを配置: " + new Vector3(length, width, 0));
                }
                else if (tileType == 1) // mapの情報で1だった場合は進める
                {
                    Instantiate(_tilePrefab, new Vector3(length, width, 0), Quaternion.identity, transform); // 移動できるマス(タイル)の表示
                    Debug.Log("タイルマスを配置: " + new Vector3(length, width, 0));
                }
                else if (tileType == 2) // mapの情報で2だった場合はゴールする(条件として両方のPlayerがゴールしないといけない)
                {
                    Vector3 goalPosition = new Vector3(length, width, 0);
                    StartCoroutine(SwitchGoalPrefab(goalPosition));
                    Debug.Log("ゴールマスを配置: " + goalPosition);
                }
                else if (tileType == 3) // mapの情報で３だった場合はギミックを展開するようにする。(それによりギミックは別々になります。
                {
                    Instantiate(_PIN, new Vector3(length, width, 0), Quaternion.identity, transform); // ギミックの表示
                    Debug.Log("ギミックマスを配置: " + new Vector3(length, width, 0));
                }
            }
        }
    }


    //・・・・・・・・・・・・・・・ここまでが壁と床の配置・・・・・・・・・・・・・・・
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
}
