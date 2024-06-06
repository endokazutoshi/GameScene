using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private int playerNumber;
    [SerializeField] private Player otherPlayer;
    private bool player1ReachedGoal = false; // �v���C���[1���S�[���ɓ��B�������ǂ�����ǐՂ���ϐ�
    private bool player2ReachedGoal = false; // �v���C���[2���S�[���ɓ��B�������ǂ�����ǐՂ���ϐ�


    [SerializeField] private float _speed; // �v���C���[�̈ړ��X�s�[�h
    private Vector2 targetPos; // �v���C���[�̖ڕW�ʒu

    [SerializeField] private Vector2 initialPosition = new Vector2(1, 1); // �v���C���[�̏����ʒu
    private Vector2 minPosition = new Vector2(0, 0); // �ŏ��̈ړ��\�Ȕ͈�
    private Vector2 maxPosition = new Vector2(22, 9); // �ő�̈ړ��\�Ȕ͈�

    private KeyCode currentKeyCode = KeyCode.None; // ���݉�����Ă���L�[
    private bool isKeyDown = false; // �L�[��������Ă��邩�ǂ����̃t���O
    private float keyDownTime = 0f; // �L�[�������ꂽ����

    private float elapsedTime = 0.0f;
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

        // ���݂̃v���C���[�̈ʒu���f�o�b�O���O�ŕ\��
        //Debug.Log("Player's position: " + targetPos);
    }

    private bool IsMoveValid(Vector2 newPos)
    {
        int x = Mathf.RoundToInt(newPos.x);
        int y = Mathf.RoundToInt(newPos.y);

        // �}�b�v�͈͓̔����`�F�b�N
        if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
        {
            // �}�b�v�f�[�^���`�F�b�N���Ĉړ��\������
            return Ground.map[y, x] == 1 || Ground.map[y, x] == 2; //Ground��map�Ƃ���1��������ړ��\�B
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
            // �v���C���[��Goal�̃}�X�ɓ��B�������ǂ������`�F�b�N
            if (Ground.map[targetY, targetX] == 2 && this.playerNumber == 2) 
               // Ground.map[targetY, targetX] == 2 && this.playerNumber == 2)
            {
            
                    // �����̃v���C���[���S�[���ɓ��B�����ꍇ�ɃS�[�����������s
                    goalReached = true;
                    elapsedTime += Time.deltaTime;

                    Debug.Log("Goal reached, elapsedTime: " + elapsedTime); // �f�o�b�O���O�ǉ�
                    Debug.Log("Loading ClearScene"); // �f�o�b�O���O�ǉ�
                    SceneManager.LoadScene("ClearScene");
               
            }



        }

        //Player���S�[���}�X�ɓ��B���A���APlayerNumber��1��PlayerNumber�̂Q�������ɃS�[���}�X�ɓ��B���Ȃ��ƃS�[���ł��܂���

    }
}
