using UnityEngine;
using UnityEngine.UI;

public class FreezeAndInput : MonoBehaviour
{
    public GameObject inputPanel; // ���̓p�l��
    public InputField inputField; // ���̓t�B�[���h
    public Text feedbackText;     // �t�B�[�h�o�b�N�p�̃e�L�X�g
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
        inputField.text = ""; // ���̓t�B�[���h���N���A
        feedbackText.gameObject.SetActive(false); // �t�B�[�h�o�b�N�e�L�X�g���\���ɂ���
        inputField.ActivateInputField(); // ���̓t�B�[���h���A�N�e�B�u�ɂ���
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
        if (inputText == anserPanel)
        {
            Unfreeze();
        }
        else
        {
            feedbackText.text = "�ԈႢ�ł�";
            Debug.Log("�s����! ������x�����Ă��������B");
        }

        feedbackText.gameObject.SetActive(true);
        inputField.ActivateInputField();
    }
}
