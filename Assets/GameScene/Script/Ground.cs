using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _tilePrefab;

    const int _nLengthStart = 1;
    const int _nWidthStart = 1;
    const int _nLengthEnd = 22;
    const int _nWidthEnd = 9;
    const int _nCenter = 11;

    [SerializeField] private int _length;
    [SerializeField] private int _width;

    // 2Dマップデータ: 0は壁、１は移動マス、２はゴールマス、３以降はギミックマス
    //ゴールマスはPlayer２人が同時にそのマスにいない限りはシーンに以降できないようにする。
    public static int[,] map = {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    private void Start()
    {
        //ステージの移動マス、壁マスの配置の処理
        for (int length = 0; length < _length; length++)
        {
            for (int width = 0; width < _width; width++)
            {
                if (length >= _nLengthStart && length < _nLengthEnd && width >= _nWidthStart && width < _nWidthEnd) // 横の座標が0から22の範囲の場合
                {
                    int tileType = map[width, length];//ここでMapの情報を取得するためにtileTypeにmapの情報を入れる
                    if (tileType == 0)//mapの情報で0だった場合は進めない
                    {
                        Instantiate(_wallPrefab, new Vector3(length, width, 0), Quaternion.identity, transform);//壁の表示
                    }
                    else if(tileType == 1)//mapの情報で1だった場合は進める
                    {
                        Instantiate(_tilePrefab, new Vector3(length, width, 0), Quaternion.identity, transform);//移動できるマス(タイル)の表示
                    }
                    else if(tileType == 2)//mapの情報で2だった場合はゴールする(条件として両方のPlayerがゴールしないといけない)
                    {
                        //Instantiate(_goalPrefab, new Vector)
                    }
                    else if(tileType == 3)//mapの情報で３だった場合はギミックを展開するようにする。(それによりギミックは別々になります。
                    {
                        //
                    }
                }
                else // それ以外の場合
                {
                    Instantiate(_wallPrefab, new Vector3(length, width, 0), Quaternion.identity, transform);
                }
            }
        }
        //・・・・・・・・・・・・・・・ここまでが壁と床の配置・・・・・・・・・・・・・・・

    }
}
