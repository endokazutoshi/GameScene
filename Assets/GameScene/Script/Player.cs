using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed; // プレイヤーの移動スピード
    private Vector2 targetPos; // プレイヤーの目標位置

    [SerializeField] private Vector2 initialPosition = new Vector2(1, 1); // プレイヤーの初期位置
    private Vector2 minPosition = new Vector2(0, 0); // 最小の移動可能な範囲
    private Vector2 maxPosition = new Vector2(22, 9); // 最大の移動可能な範囲

    private KeyCode currentKeyCode = KeyCode.None; // 現在押されているキー
    private bool isKeyDown = false; // キーが押されているかどうかのフラグ
    private float keyDownTime = 0f; // キーが押された時間

    private float waitTime = 2.0f; //待機時間
    private float elapsedTime = 0.0f;
    private bool goalReached = false; // ゴールに到達したかどうかのフラグ

    private void Start()
    {
        transform.position = initialPosition;
        targetPos = initialPosition;
    }

    void Update()
    {
        if (goalReached) return;

        Vector2 move = Vector2.zero;

        // キーが押されているかどうかを判定
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (!isKeyDown)
            {
                // キーが初めて押されたときの処理
                isKeyDown = true;
                keyDownTime = Time.time;
            }

            // キーが押されている間に経過した時間を計算
            float keyElapsedTime = Time.time - keyDownTime;
            if (keyElapsedTime > 0.1f) // 0.1秒以上経過していたらキーが長押しとみなす
            {
                // キーが長押しされている場合の処理
                if (Input.GetKey(KeyCode.W) && currentKeyCode == KeyCode.None)
                {
                    move.y = 1;
                    currentKeyCode = KeyCode.W;
                }
                else if (Input.GetKey(KeyCode.S) && currentKeyCode == KeyCode.None)
                {
                    move.y = -1;
                    currentKeyCode = KeyCode.S;
                }

                if (Input.GetKey(KeyCode.A) && currentKeyCode == KeyCode.None)
                {
                    move.x = -1;
                    currentKeyCode = KeyCode.A;
                }
                else if (Input.GetKey(KeyCode.D) && currentKeyCode == KeyCode.None)
                {
                    move.x = 1;
                    currentKeyCode = KeyCode.D;
                }
            }
        }
        else
        {
            // キーが離されたときの処理
            isKeyDown = false;
            currentKeyCode = KeyCode.None;
        }

        if (move != Vector2.zero)
        {
            Vector2 newPos = targetPos + move;
            if (IsMoveValid(newPos))
            {
                targetPos = newPos;
            }
        }

        Move(targetPos);

        // ゴールに到達したかどうかをチェック
        CheckGoalReached();

        // 現在のプレイヤーの位置をデバッグログで表示
        //Debug.Log("Player's position: " + targetPos);
    }

    private bool IsMoveValid(Vector2 newPos)
    {
        int x = Mathf.RoundToInt(newPos.x);
        int y = Mathf.RoundToInt(newPos.y);

        // マップの範囲内かチェック
        if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
        {
            // マップデータをチェックして移動可能か判定
            return Ground.map[y, x] == 1 || Ground.map[y, x] == 2; //Groundのmapとして1だったら移動可能。
        }
        return false;
    }

    private void Move(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards((Vector2)transform.position, targetPosition, _speed * Time.deltaTime);
    }

    private void CheckGoalReached()
    {
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);

        // プレイヤーがGoalのマスに到達したかどうかをチェック
        if (Ground.map[y, x] == 2)
        {
            goalReached = true;
            Debug.Log("Goal Reached!");
            elapsedTime = 0.0f; // 経過時間をリセット
        }
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= waitTime)
        {
            SceneManager.LoadScene("ClearScene");
        }
    }
}
