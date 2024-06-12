using UnityEngine;
using UnityEngine.UI;

public class FreezeAndInput : MonoBehaviour
{
    public GameObject inputPanel; // 入力パネル
    public InputField inputField; // 入力フィールド
    public Text feedbackText;     // フィードバック用のテキスト

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
        if (Input.GetKeyDown(KeyCode.Space))
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

        if (isFrozen && Input.GetKeyDown(KeyCode.Return))
        {
            CheckInput();
        }
    }

    void Freeze()
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

    void Unfreeze()
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
        if (inputText == "6")
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
}
