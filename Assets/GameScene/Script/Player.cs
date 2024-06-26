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
    public enum SelectScene
    {
        ProccesInput,
        CheckSpaceKey,
        CheckGoal
    }


    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
    }
    private SelectScene currentScene = SelectScene.ProccesInput;

    private void Start()
    {
        transform.position = initialPosition;
        targetPos = initialPosition;
    }

    void Update()
    {
        if (goalReached || isFrozen) return;

        switch (currentScene)
        {
            case SelectScene.ProccesInput:
                Debug.Log("プレイヤーの操作に戻ります。");
                ProcessMovementInput();
                if (CheckSpaceKeyPressed())
                {
                    currentScene = SelectScene.CheckSpaceKey;
                    Debug.Log("Updateのswitch文の一つ目のif文");
                }
                break;
            case SelectScene.CheckSpaceKey:
                if (CheckSpaceKeyPressed())
                {
                    currentScene = SelectScene.CheckGoal;
                    Debug.Log("Updateのswitch文の2つ目のif文");
                }
                else
                {
                    currentScene = SelectScene.ProccesInput; // パスワード入力が終わったら再び入力処理に戻る
                    Debug.Log("パスワードが終わった際に出てくるelse文");
                }
                break;
            case SelectScene.CheckGoal:
                CheckGoalReached();
                if (!goalReached) // ゴールに到達していない場合、再び入力処理に戻る
                {
                    currentScene = SelectScene.ProccesInput;
                }
                break;
            default:
                break;
        }

        // ゴールに到達しているかどうかを毎フレームチェック
        CheckGoalReached();
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

    private bool CheckSpaceKeyPressed()
    {
        bool spaceKeyPressed = false; // スペースキーが押されたかどうかのフラグ

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

                    // Coroutineを開始する
                    StartCoroutine(WaitAndUnfreeze(freezeAndInput));

                    spaceKeyPressed = true; // スペースキーが押されたとフラグを立てる
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

                        // Coroutineを開始する
                        StartCoroutine(WaitAndUnfreeze(freezeAndInput));

                        spaceKeyPressed = true; // スペースキーが押されたとフラグを立てる
                    }
                }
            }
        }

        return spaceKeyPressed; // スペースキーが押されたかどうかの結果を返す
    }



    private IEnumerator WaitAndUnfreeze(FreezeAndInput freezeAndInput)
    {
        bool passwordCorrect = false;

        // パスワードが正しいかどうかを確認するまでループする
        while (!passwordCorrect)
        {
            // ESCキーが押されたかどうかをチェックする
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // ESCキーが押された場合の処理
                // ここでは凍結状態を解除しないまま、処理を終了し操作画面に戻る
                Debug.Log("ESCキーが押されたため、操作画面に戻ります。");

                // フリーズ画面を閉じる処理
                CloseFreezeScreen(); // このメソッドは適切なものに置き換えてください

                SetFrozen(false); // プレイヤーの凍結を解除する

                yield break; // Coroutineを終了して元の処理に戻る
            }

            // パスワードが正しいかどうかを即座にチェックする
            if (freezeAndInput.IsPasswordCorrect())
            {
                freezeAndInput.Unfreeze();
                SetFrozen(false); // プレイヤーの凍結を解除する

                // パスワードが正しい場合の処理
                currentScene = SelectScene.ProccesInput;
                Debug.Log("パスワードが正しいため、アンフリーズされました。ここには通りました。");

                passwordCorrect = true; // パスワードが正しいことをフラグで示す
            }
            else
            {
                // パスワードが正しくない場合の処理
                //Debug.Log("パスワードが正しくありません。プレイヤーはまだ凍結されています。");
                SetFrozen(true); // プレイヤーを凍結状態にしたままとする

                yield return null; // 1フレーム待機する
            }
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


        public void CloseFreezeScreen()
        {
            FreezeAndInput freezeAndInput = FindObjectOfType<FreezeAndInput>();
            if (freezeAndInput != null)
            {
                freezeAndInput.CloseFreezeScreen();
            }
        }

}

