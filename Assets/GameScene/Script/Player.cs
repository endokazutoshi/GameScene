//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class Player : MonoBehaviour
//{
//    [SerializeField] private float _speed = 5f; // �v���C���[�̈ړ��X�s�[�h
//    private Vector2 targetPos; // �v���C���[�̖ڕW�ʒu
//    [SerializeField] private Vector2 initialPosition = new Vector2(1, 1); // �v���C���[�̏����ʒu
//    private Vector2 minPosition = new Vector2(0, 0); // �ŏ��̈ړ��\�Ȕ͈�
//    private Vector2 maxPosition = new Vector2(22, 9); // �ő�̈ړ��\�Ȕ͈�

//    private bool isFrozen = false; // �v���C���[����������Ă��邩�ǂ����̃t���O
//    private bool goalReached = false; // �S�[���ɓ��B�������ǂ����̃t���O

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
    [SerializeField] private float _speed; // �v���C���[�̈ړ��X�s�[�h
    private Vector2 targetPos; // �v���C���[�̖ڕW�ʒu
    [SerializeField] private Vector2 initialPosition = new Vector2(1, 1); // �v���C���[�̏����ʒu
    private Vector2 minPosition = new Vector2(0, 0); // �ŏ��̈ړ��\�Ȕ͈�
    private Vector2 maxPosition = new Vector2(22, 9); // �ő�̈ړ��\�Ȕ͈�

    private KeyCode currentKeyCode = KeyCode.None; // ���݉�����Ă���L�[
    private bool isKeyDown = false; // �L�[��������Ă��邩�ǂ����̃t���O
    private float keyDownTime = 0f; // �L�[�������ꂽ����

    private bool goalReached = false; // �S�[���ɓ��B�������ǂ����̃t���O

    private bool isFrozen = false; // �v���C���[����������Ă��邩�ǂ����̃t���O
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
                //�ԍ����̓}�X�̋߂��ŃX�y�[�X�L�[�������ꂽ�ꍇ�̏���
                CheckSpaceKeyPressed();
                a = 1;
            case 3:
                // �S�[���ɓ��B�������ǂ������`�F�b�N
                CheckGoalReached();
            default:
                break;
        }
        



       
    }

    private void ProcessMovementInput()
    {
        Vector2 move = Vector2.zero;

        // �L�[��������Ă��邩�ǂ����𔻒�
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


    }

    private bool IsMoveValid(Vector2 newPos)
    {
        int x = Mathf.RoundToInt(newPos.x);
        int y = Mathf.RoundToInt(newPos.y);

        // �}�b�v�͈͓̔����`�F�b�N
        if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
        {
            // �}�b�v�f�[�^���`�F�b�N���Ĉړ��\������
            return Ground.map[y, x] == 1 || Ground.map[y, x] == 2 || Ground.map[y, x] == 6; // Ground��map�Ƃ���1��������ړ��\
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
                    // �����̃v���C���[���S�[���ɓ��B�����ꍇ�ɃS�[�����������s
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

            // �v���C���[�̌��݈ʒu��tileType==6�̃}�X���ǂ������`�F�b�N
            if (Ground.map[currentY, currentX] == 6)
            {
                // FreezeAndInput�R���|�[�l���g�����I�u�W�F�N�g��T��
                FreezeAndInput freezeAndInput = FindObjectOfType<FreezeAndInput>();
                if (freezeAndInput != null)
                {
                    // Freeze���\�b�h���Ăяo��
                    freezeAndInput.Freeze();
                    SetFrozen(true); // �v���C���[�𓀌���Ԃɂ���

                    // �����Ńp�X���[�h���͂̌��Unfreeze���\�b�h���Ăяo��
                    // ����ɂ��A�p�X���[�h���������ꍇ�Ƀv���C���[��������悤�ɂȂ�
                    StartCoroutine(WaitAndUnfreeze(freezeAndInput));
                }
            }
            else
            {
                // �v���C���[�̌��݈ʒu����ɂ��ď㉺���E�̈ʒu���`�F�b�N���A
                // �������͂ɃM�~�b�N�̕ǂ�����Ȃ��FreezeAndInput�R���|�[�l���g��Freeze���\�b�h���Ăяo��
                if (HasNearbyGimmickWall(currentX, currentY))
                {
                    // FreezeAndInput�R���|�[�l���g�����I�u�W�F�N�g��T��
                    FreezeAndInput freezeAndInput = FindObjectOfType<FreezeAndInput>();
                    if (freezeAndInput != null)
                    {
                        // Freeze���\�b�h���Ăяo��
                        freezeAndInput.Freeze();
                        SetFrozen(true); // �v���C���[�𓀌���Ԃɂ���
                    }
                }
            }
        }
    }

    private IEnumerator WaitAndUnfreeze(FreezeAndInput freezeAndInput)
    {
        // �p�X���[�h�����������ǂ������m�F���邽�߂̑ҋ@����
        yield return new WaitForSeconds(2f); // ��Ƃ���2�b�҂�

        // �p�X���[�h���������ꍇ�AUnfreeze���\�b�h���Ăяo��
        if (freezeAndInput.IsPasswordCorrect())
        {
            freezeAndInput.Unfreeze();
            SetFrozen(false); // �v���C���[�̓�������������
        }
        else
        {
            // �p�X���[�h���������Ȃ��ꍇ�̏���
            Debug.Log("Password incorrect. Player remains frozen.");
            // �����ŉ�������̃G���[���b�Z�[�W�Ȃǂ�\������Ȃǂ̏������s��
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

    // �v���C���[�̎��͂ɃM�~�b�N�̕ǂ����邩�ǂ������`�F�b�N����֐�
    private bool HasNearbyGimmickWall(int x, int y)
    {
        // �㉺���E�̈ʒu���`�F�b�N���A�ǂꂩ��ł��M�~�b�N�̕ǂ������true��Ԃ�
        return Ground.map[y, x - 1] == 3 || // ���ɃM�~�b�N�̕ǂ����邩�`�F�b�N
        Ground.map[y, x + 1] == 3 || // �E�ɃM�~�b�N�̕ǂ����邩�`�F�b�N
        Ground.map[y - 1, x] == 3 || // ��ɃM�~�b�N�̕ǂ����邩�`�F�b�N
        Ground.map[y + 1, x] == 3; // ���ɃM�~�b�N�̕ǂ����邩�`�F�b�N
    }
}