using UnityEngine;
using UnityEngine.UI;

public class FreezeAndInput : MonoBehaviour
{
    public GameObject inputPanel; // 入力パネル
    public InputField inputField; // 入力フィールド
    public Text feedbackText;     // フィードバック用のテキスト
    [SerializeField] string anserPanel;

    private bool isFrozen = false;

    void Start()
    {
        if (inputPanel == null)
        {
            Debug.LogError("inputPanel is not assigned in the inspector");
            return;
        }

        if (inputField == null)
        {
            Debug.LogError("inputField is not assigned in the inspector");
            return;
        }

        if (feedbackText == null)
        {
            Debug.LogError("feedbackText is not assigned in the inspector");
            return;
        }

        inputPanel.SetActive(false);
        feedbackText.gameObject.SetActive(false);

        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    void Update()
    {
        // tileTypeが3でその周囲にプレイヤーがいる場合にスペースキーが押されたとみなす
        if (IsSpaceKeyPressedNearTileType3())
        {
            if (isFrozen)
            {
                Unfreeze();
            }
            else
            {
                Freeze();
            }
        }
        // この部分は不要になるので削除
        // if (isFrozen && Input.GetKeyDown(KeyCode.Return))
        // {
        //     CheckInput();
        // }


    }

    public void Freeze()
    {
        foreach (var go in FindObjectsOfType<GameObject>())
        {
            if (go.CompareTag("Player"))
            {
                var component = go.GetComponent<MonoBehaviour>();
                if (component != null)
                {
                    component.enabled = false;
                }
                else
                {
                    Debug.LogWarning($"GameObject '{go.name}' with tag 'Player' does not have a MonoBehaviour component.");
                }
            }
        }

        inputPanel.SetActive(true);
        inputField.text = ""; // 入力フィールドをクリア
        feedbackText.gameObject.SetActive(false); // フィードバックテキストを非表示にする
        inputField.ActivateInputField(); // 入力フィールドをアクティブにする
        isFrozen = true;
    }

    public void Unfreeze()
    {
        foreach (var go in FindObjectsOfType<GameObject>())
        {
            if (go.CompareTag("Player") || go.CompareTag("Player2"))
            {
                var component = go.GetComponent<MonoBehaviour>();
                if (component != null)
                {
                    component.enabled = true;
                }
                else
                {
                    Debug.LogWarning($"GameObject '{go.name}' with tag '{go.tag}' does not have a MonoBehaviour component.");
                }
            }
        }

        inputPanel.SetActive(false);
        isFrozen = false;
    }

    void OnEndEdit(string inputText)
    {
        Debug.Log("OnEndEdit called with input: " + inputText);
    }

    void CheckInput()
    {
        string inputText = inputField.text;
        if (inputText == anserPanel)
        {
            Unfreeze();
        }
        else
        {
            feedbackText.text = "間違いです";
            Debug.Log("不正解! もう一度試してください。");
        }

        feedbackText.gameObject.SetActive(true);
        inputField.ActivateInputField();
    }
    private bool IsSpaceKeyPressedNearTileType3()
    {
        // プレイヤーの現在の位置を取得
        Vector3 playerPosition = transform.position;

        // プレイヤーの周囲のタイルをチェック
        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                // プレイヤーの周囲のタイルの位置を計算
                Vector3 tilePosition = playerPosition + new Vector3(xOffset, yOffset, 0);

                // タイルの位置が tileType が 3 かどうかチェック
                if (IsTileType3(tilePosition))
                {
                    return true; // プレイヤーの周囲に tileType が 3 のタイルがある場合は true を返す
                }
            }
        }
        return false;
    }

    // 指定された位置のタイルがタイプ3かどうかをチェックするメソッド
    // 指定された位置のタイルがタイプ3かどうかをチェックするメソッド
    private bool IsTileType3(Vector3 position)
    {
        // Groundクラスのインスタンスを取得
        Ground ground = FindObjectOfType<Ground>();
        if (ground != null)
        {
            // 指定された位置がマップの範囲内かどうかをチェック
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);
            if (x >= 0 && x < ground.Width && y >= 0 && y < ground.Length)
            {
                // タイルの位置が tileType が 3 かどうかチェック
                int tileType = ground.GetTileTypeAtPosition(position);
                return tileType == 3;
            }
        }
        return false;
    }



}
