//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class Player : MonoBehaviour
//{
//    [SerializeField] private float _speed = 5f; // プレイヤーの移動スピード
//    private Vector2 targetPos; // プレイヤーの目標位置
//    [SerializeField] private Vector2 initialPosition = new Vector2(1, 1); // プレイヤーの初期位置
//    private Vector2 minPosition = new Vector2(0, 0); // 最小の移動可能な範囲
//    private Vector2 maxPosition = new Vector2(22, 9); // 最大の移動可能な範囲

//    private bool isFrozen = false; // プレイヤーが凍結されているかどうかのフラグ
//    private bool goalReached = false; // ゴールに到達したかどうかのフラグ

//    private enum InputState
//    {
//        Moving,
//        SpaceKey,
//        GoalCheck
//    }

//    private void Start()
//    {
//        transform.position = initialPosition;
//        targetPos = initialPosition;
//    }

//    void Update()
//    {
//        if (goalReached || isFrozen)
//            return;

//        switch (GetInputState())
//        {
//            case InputState.Moving:
//                UpdateMovement();
//                break;
//            case InputState.SpaceKey:
//                CheckSpaceKeyPressed();
//                break;
//            case InputState.GoalCheck:
//                CheckGoalReached();
//                break;
//            default:
//                break;
//        }
//    }

//    private InputState GetInputState()
//    {
//        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
//        {
//            return InputState.Moving;
//        }
//        else if (Input.GetKeyDown(KeyCode.Space))
//        {
//            return InputState.SpaceKey;
//        }
//        else
//        {
//            return InputState.GoalCheck;
//        }
//    }

//    private void UpdateMovement()
//    {
//        Vector2 move = Vector2.zero;

//        if (Input.GetKey(KeyCode.W))
//        {
//            move.y = 1;
//        }
//        else if (Input.GetKey(KeyCode.S))
//        {
//            move.y = -1;
//        }

//        if (Input.GetKey(KeyCode.A))
//        {
//            move.x = -1;
//        }
//        else if (Input.GetKey(KeyCode.D))
//        {
//            move.x = 1;
//        }

//        if (move != Vector2.zero)
//        {
//            Vector2 newPos = targetPos + move;
//            if (IsMoveValid(newPos))
//            {
//                targetPos = newPos;
//            }
//        }

//        Move(targetPos);
//    }

//    private void Move(Vector2 targetPosition)
//    {
//        transform.position = Vector2.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
//    }

//    private void CheckSpaceKeyPressed()
//    {
//        int currentX = Mathf.RoundToInt(transform.position.x);
//        int currentY = Mathf.RoundToInt(transform.position.y);

//        if (Ground.map[currentY, currentX] == 6)
//        {
//            FreezeAndInput freezeAndInput = FindObjectOfType<FreezeAndInput>();
//            if (freezeAndInput != null)
//            {
//                freezeAndInput.Freeze();
//                SetFrozen(true);
//                StartCoroutine(WaitAndUnfreeze(freezeAndInput));
//            }
//        }
//        else if (HasNearbyGimmickWall(currentX, currentY))
//        {
//            FreezeAndInput freezeAndInput = FindObjectOfType<FreezeAndInput>();
//            if (freezeAndInput != null)
//            {
//                freezeAndInput.Freeze();
//                SetFrozen(true);
//            }
//        }
//    }

//    private void CheckGoalReached()
//    {
//        int targetX = Mathf.RoundToInt(targetPos.x);
//        int targetY = Mathf.RoundToInt(targetPos.y);

//        if (Mathf.Approximately(transform.position.x, targetPos.x) && Mathf.Approximately(transform.position.y, targetPos.y))
//        {
//            if (Ground.map[targetY, targetX] == 2)
//            {
//                GameObject[] player1Objects = GameObject.FindGameObjectsWithTag("Player1");
//                GameObject[] player2Objects = GameObject.FindGameObjectsWithTag("Player2");

//                bool player1AtGoal = false;
//                bool player2AtGoal = false;

//                foreach (GameObject p1 in player1Objects)
//                {
//                    Player p1Script = p1.GetComponent<Player>();
//                    int playerX = Mathf.RoundToInt(p1Script.GetTargetPos().x);
//                    int playerY = Mathf.RoundToInt(p1Script.GetTargetPos().y);

//                    if (Ground.map[playerY, playerX] == 2)
//                    {
//                        player1AtGoal = true;
//                        break;
//                    }
//                }

//                foreach (GameObject p2 in player2Objects)
//                {
//                    Player p2Script = p2.GetComponent<Player>();
//                    int playerX = Mathf.RoundToInt(p2Script.GetTargetPos().x);
//                    int playerY = Mathf.RoundToInt(p2Script.GetTargetPos().y);

//                    if (Ground.map[playerY, playerX] == 2)
//                    {
//                        player2AtGoal = true;
//                        break;
//                    }
//                }

//                if (player1AtGoal && player2AtGoal)
//                {
//                    Debug.Log("Both players reached the goal. Loading ClearScene...");
//                    SceneManager.LoadScene("ClearScene");
//                }
//            }
//        }
//    }

//    private IEnumerator WaitAndUnfreeze(FreezeAndInput freezeAndInput)
//    {
//        yield return new WaitForSeconds(2f);

//        if (freezeAndInput.IsPasswordCorrect())
//        {
//            freezeAndInput.Unfreeze();
//            SetFrozen(false);
//        }
//        else
//        {
//            Debug.Log("Password incorrect. Player remains frozen.");
//        }
//    }

//    private bool IsMoveValid(Vector2 newPos)
//    {
//        int x = Mathf.RoundToInt(newPos.x);
//        int y = Mathf.RoundToInt(newPos.y);

//        return x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y &&
//               (Ground.map[y, x] == 1 || Ground.map[y, x] == 2 || Ground.map[y, x] == 6);
//    }

//    private bool HasNearbyGimmickWall(int x, int y)
//    {
//        return Ground.map[y, x - 1] == 3 || Ground.map[y, x + 1] == 3 || Ground.map[y - 1, x] == 3 || Ground.map[y + 1, x] == 3;
//    }

//    public Vector2 GetTargetPos()
//    {
//        return targetPos;
//    }

//    public void SetFrozen(bool frozen)
//    {
//        isFrozen = frozen;
//    }
//}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    private bool goalReached = false; // ゴールに到達したかどうかのフラグ

    private bool isFrozen = false; // プレイヤーが凍結されているかどうかのフラグ
    private int a = 1;

    private enum InputState
    {
        Moving,
        SpaceKey,
        GoalCheck
    }


    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
    }

    private void Start()
    {
        transform.position = initialPosition;
        targetPos = initialPosition;
    }

    void Update()
    {
        if (goalReached || isFrozen) return;
        switch(a)
        {
            case 1:
                ProcessMovementInput();
            case 2:
                //番号入力マスの近くでスペースキーが押された場合の処理
                CheckSpaceKeyPressed();
                a = 1;
            case 3:
                // ゴールに到達したかどうかをチェック
                CheckGoalReached();
            default:
                break;
        }
        



       
    }

    private void ProcessMovementInput()
    {
        Vector2 move = Vector2.zero;

        // キーが押されているかどうかを判定
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


    }

    private bool IsMoveValid(Vector2 newPos)
    {
        int x = Mathf.RoundToInt(newPos.x);
        int y = Mathf.RoundToInt(newPos.y);

        // マップの範囲内かチェック
        if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
        {
            // マップデータをチェックして移動可能か判定
            return Ground.map[y, x] == 1 || Ground.map[y, x] == 2 || Ground.map[y, x] == 6; // Groundのmapとして1だったら移動可能
        }
        return false;
    }
    private void Move(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards((Vector2)transform.position, targetPosition, _speed * Time.deltaTime);
    }

    private void CheckGoalReached()
    {
        int targetX = Mathf.RoundToInt(targetPos.x);
        int targetY = Mathf.RoundToInt(targetPos.y);

        // プレイヤーが目標位置に完全に到達しているかどうかをチェック
        if (Mathf.Approximately(transform.position.x, targetPos.x) && Mathf.Approximately(transform.position.y, targetPos.y))
        {
            // プレイヤーがGoalのマスに到達したかどうかをチェック
            if (Ground.map[targetY, targetX] == 2)
            {
                GameObject[] player1Objects = GameObject.FindGameObjectsWithTag("Player1");
                GameObject[] player2Objects = GameObject.FindGameObjectsWithTag("Player2");

                bool player1AtGoal = false;
                bool player2AtGoal = false;

                foreach (GameObject p1 in player1Objects)
                {
                    Player p1Script = p1.GetComponent<Player>();
                    int playerX = Mathf.RoundToInt(p1Script.GetTargetPos().x);
                    int playerY = Mathf.RoundToInt(p1Script.GetTargetPos().y);

                    if (Ground.map[playerY, playerX] == 2)
                    {
                        player1AtGoal = true;
                        break;
                    }
                }

                foreach (GameObject p2 in player2Objects)
                {
                    Player p2Script = p2.GetComponent<Player>();
                    int playerX = Mathf.RoundToInt(p2Script.GetTargetPos().x);
                    int playerY = Mathf.RoundToInt(p2Script.GetTargetPos().y);

                    if (Ground.map[playerY, playerX] == 2)
                    {
                        player2AtGoal = true;
                        break;
                    }
                }

                if (player1AtGoal && player2AtGoal)
                {
                    // 両方のプレイヤーがゴールに到達した場合にゴール処理を実行
                    Debug.Log("Both players reached the goal. Loading ClearScene...");
                    SceneManager.LoadScene("ClearScene");
                }
            }
        }
    }

    public Vector2 GetTargetPos()
    {
        return targetPos;
    }

    private void CheckSpaceKeyPressed()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int currentX = Mathf.RoundToInt(transform.position.x);
            int currentY = Mathf.RoundToInt(transform.position.y);

            // プレイヤーの現在位置がtileType==6のマスかどうかをチェック
            if (Ground.map[currentY, currentX] == 6)
            {
                // FreezeAndInputコンポーネントを持つオブジェクトを探す
                FreezeAndInput freezeAndInput = FindObjectOfType<FreezeAndInput>();
                if (freezeAndInput != null)
                {
                    // Freezeメソッドを呼び出す
                    freezeAndInput.Freeze();
                    SetFrozen(true); // プレイヤーを凍結状態にする

                    // ここでパスワード入力の後にUnfreezeメソッドを呼び出す
                    // これにより、パスワードが正しい場合にプレイヤーが動けるようになる
                    StartCoroutine(WaitAndUnfreeze(freezeAndInput));
                }
            }
            else
            {
                // プレイヤーの現在位置を基準にして上下左右の位置をチェックし、
                // もし周囲にギミックの壁があるならばFreezeAndInputコンポーネントのFreezeメソッドを呼び出す
                if (HasNearbyGimmickWall(currentX, currentY))
                {
                    // FreezeAndInputコンポーネントを持つオブジェクトを探す
                    FreezeAndInput freezeAndInput = FindObjectOfType<FreezeAndInput>();
                    if (freezeAndInput != null)
                    {
                        // Freezeメソッドを呼び出す
                        freezeAndInput.Freeze();
                        SetFrozen(true); // プレイヤーを凍結状態にする
                    }
                }
            }
        }
    }

    private IEnumerator WaitAndUnfreeze(FreezeAndInput freezeAndInput)
    {
        // パスワードが正しいかどうかを確認するための待機時間
        yield return new WaitForSeconds(2f); // 例として2秒待つ

        // パスワードが正しい場合、Unfreezeメソッドを呼び出す
        if (freezeAndInput.IsPasswordCorrect())
        {
            freezeAndInput.Unfreeze();
            SetFrozen(false); // プレイヤーの凍結を解除する
        }
        else
        {
            // パスワードが正しくない場合の処理
            Debug.Log("Password incorrect. Player remains frozen.");
            // ここで何かしらのエラーメッセージなどを表示するなどの処理を行う
        }
    }

    Vector3 FindNearestPasswordPosition()
    {
        GameObject[] passwordObjects = GameObject.FindGameObjectsWithTag("Password");
        Vector3 playerPosition = transform.position;
        Vector3 nearestPosition = Vector3.zero;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject passwordObject in passwordObjects)
        {
            Vector3 passwordPosition = passwordObject.transform.position;
            float distance = Vector3.Distance(playerPosition, passwordPosition);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPosition = passwordPosition;
            }
        }

        return nearestPosition;
    }

    // プレイヤーの周囲にギミックの壁があるかどうかをチェックする関数
    private bool HasNearbyGimmickWall(int x, int y)
    {
        // 上下左右の位置をチェックし、どれか一つでもギミックの壁があればtrueを返す
        return Ground.map[y, x - 1] == 3 || // 左にギミックの壁があるかチェック
        Ground.map[y, x + 1] == 3 || // 右にギミックの壁があるかチェック
        Ground.map[y - 1, x] == 3 || // 上にギミックの壁があるかチェック
        Ground.map[y + 1, x] == 3; // 下にギミックの壁があるかチェック
    }
}