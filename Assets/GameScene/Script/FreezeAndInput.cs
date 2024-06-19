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
        // tileType��3�ł��̎��͂Ƀv���C���[������ꍇ�ɃX�y�[�X�L�[�������ꂽ�Ƃ݂Ȃ�
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
        // ���̕����͕s�v�ɂȂ�̂ō폜
        if (inputPanel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            CheckInput();
        }


    }

    public void Freeze()
    {
        Debug.Log("Freeze ���\�b�h���Ăяo����܂����B");

        // "Player" �^�O�����I�u�W�F�N�g�� MonoBehaviour �𖳌��ɂ���
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player1");
        foreach (var player in players)
        {
            var component = player.GetComponent<MonoBehaviour>();
            if (component != null)
            {
                component.enabled = false;
                Debug.Log($"Player '{player.name}' ���t���[�Y����܂����B");
            }
            else
            {
                Debug.LogWarning($"GameObject '{player.name}' with tag 'Player' does not have a MonoBehaviour component.");
            }
        }

        inputPanel.SetActive(true);
        inputField.text = ""; // ���̓t�B�[���h���N���A
        feedbackText.gameObject.SetActive(false); // �t�B�[�h�o�b�N�e�L�X�g���\���ɂ���
        inputField.ActivateInputField(); // ���̓t�B�[���h���A�N�e�B�u�ɂ���
        isFrozen = true;
    }

    public void Unfreeze()
    {
        Debug.Log("Unfreeze ���\�b�h���Ăяo����܂����B");

        // "Player" �^�O�����I�u�W�F�N�g�� MonoBehaviour ���ēx�L���ɂ���
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player1");
        foreach (var player in players)
        {
            var component = player.GetComponent<MonoBehaviour>();
            if (component != null)
            {
                component.enabled = true;
                Debug.Log($"Player '{player.name}' ���A���t���[�Y����܂����B");
            }
            else
            {
                Debug.LogWarning($"GameObject '{player.name}' with tag 'Player' does not have a MonoBehaviour component.");
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
    private bool IsSpaceKeyPressedNearTileType3()
    {
        // �v���C���[�̌��݂̈ʒu���擾
        Vector3 playerPosition = transform.position;

        // �v���C���[�̎��͂̃^�C�����`�F�b�N
        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                // �v���C���[�̎��͂̃^�C���̈ʒu���v�Z
                Vector3 tilePosition = playerPosition + new Vector3(xOffset, yOffset, 0);

                // �^�C���̈ʒu�� tileType �� 3 ���ǂ����`�F�b�N
                if (IsTileType3(tilePosition))
                {
                    return true; // �v���C���[�̎��͂� tileType �� 3 �̃^�C��������ꍇ�� true ��Ԃ�
                }
            }
        }
        return false;
    }

    // �w�肳�ꂽ�ʒu�̃^�C�����^�C�v3���ǂ������`�F�b�N���郁�\�b�h
    // �w�肳�ꂽ�ʒu�̃^�C�����^�C�v3���ǂ������`�F�b�N���郁�\�b�h
    private bool IsTileType3(Vector3 position)
    {
        // Ground�N���X�̃C���X�^���X���擾
        Ground ground = FindObjectOfType<Ground>();
        if (ground != null)
        {
            // �w�肳�ꂽ�ʒu���}�b�v�͈͓̔����ǂ������`�F�b�N
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);
            if (x >= 0 && x < ground.Width && y >= 0 && y < ground.Length)
            {
                // �^�C���̈ʒu�� tileType �� 3 ���ǂ����`�F�b�N
                int tileType = ground.GetTileTypeAtPosition(position);
                return tileType == 3;
            }
        }
        return false;
    }



}
