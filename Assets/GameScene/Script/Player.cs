using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed; // �v���C���[�̈ړ��X�s�[�h
    private Vector2 targetPos; // �v���C���[�̖ڕW�ʒu

    [SerializeField] private Vector2 initialPosition = new Vector2(1, 1); // �v���C���[�̏����ʒu
    private Vector2 minPosition = new Vector2(0, 0); // �ŏ��̈ړ��\�Ȕ͈�
    private Vector2 maxPosition = new Vector2(22, 9); // �ő�̈ړ��\�Ȕ͈�

    private KeyCode currentKeyCode = KeyCode.None; // ���݉�����Ă���L�[
    private bool isKeyDown = false; // �L�[��������Ă��邩�ǂ����̃t���O
    private float keyDownTime = 0f; // �L�[�������ꂽ����

    private bool goalReached = false; // �S�[���ɓ��B�������ǂ����̃t���O

    private void Start()
    {
        transform.position = initialPosition;
        targetPos = initialPosition;
    }

    void Update()
    {
        if (goalReached) return;

        Vector2 move = Vector2.zero;

        // �L�[��������Ă��邩�ǂ����𔻒�
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (!isKeyDown)
            {
                // �L�[�����߂ĉ����ꂽ�Ƃ��̏���
                isKeyDown = true;
                keyDownTime = Time.time;
            }

            // �L�[��������Ă���ԂɌo�߂������Ԃ��v�Z
            float keyElapsedTime = Time.time - keyDownTime;
            if (keyElapsedTime > 0.1f) // 0.1�b�ȏ�o�߂��Ă�����L�[���������Ƃ݂Ȃ�
            {
                // �L�[������������Ă���ꍇ�̏���
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
            // �L�[�������ꂽ�Ƃ��̏���
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

        // �S�[���ɓ��B�������ǂ������`�F�b�N
        CheckGoalReached();
    }

    private bool IsMoveValid(Vector2 newPos)
    {
        int x = Mathf.RoundToInt(newPos.x);
        int y = Mathf.RoundToInt(newPos.y);

        // �}�b�v�͈͓̔����`�F�b�N
        if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
        {
            // �}�b�v�f�[�^���`�F�b�N���Ĉړ��\������
            return Ground.map[y, x] == 1 || Ground.map[y, x] == 2; // Ground��map�Ƃ���1��������ړ��\
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

        // �v���C���[���ڕW�ʒu�Ɋ��S�ɓ��B���Ă��邩�ǂ������`�F�b�N
        if (Mathf.Approximately(transform.position.x, targetPos.x) && Mathf.Approximately(transform.position.y, targetPos.y))
        {
            // �v���C���[��Goal�̃}�X�ɓ��B�������ǂ������`�F�b�N
            if (Ground.map[targetY, targetX] == 2)
            {
                GameObject[] player1Objects = GameObject.FindGameObjectsWithTag("Player1");
                GameObject[] player2Objects = GameObject.FindGameObjectsWithTag("Player2");

                bool player1AtGoal = false;
                bool player2AtGoal = false;

                foreach (GameObject player in player1Objects)
                {
                    Player playerScript = player.GetComponent<Player>();
                    int playerX = Mathf.RoundToInt(playerScript.targetPos.x);
                    int playerY = Mathf.RoundToInt(playerScript.targetPos.y);

                    if (Ground.map[playerY, playerX] == 2)
                    {
                        player1AtGoal = true;
                        break;
                    }
                }

                foreach (GameObject player in player2Objects)
                {
                    Player playerScript = player.GetComponent<Player>();
                    int playerX = Mathf.RoundToInt(playerScript.targetPos.x);
                    int playerY = Mathf.RoundToInt(playerScript.targetPos.y);

                    if (Ground.map[playerY, playerX] == 2)
                    {
                        player2AtGoal = true;
                        break;
                    }
                }

                if (player1AtGoal && player2AtGoal)
                {
                    // �����̃v���C���[���S�[���ɓ��B�����ꍇ�ɃS�[�����������s
                    goalReached = true;

                    Debug.Log("Both players reached the goal. Loading ClearScene...");
                    SceneManager.LoadScene("ClearScene");
                }
            }
        }
    }
}
