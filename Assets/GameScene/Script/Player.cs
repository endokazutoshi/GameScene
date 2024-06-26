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
                Debug.Log("�v���C���[�̑���ɖ߂�܂��B");
                ProcessMovementInput();
                if (CheckSpaceKeyPressed())
                {
                    currentScene = SelectScene.CheckSpaceKey;
                    Debug.Log("Update��switch���̈�ڂ�if��");
                }
                break;
            case SelectScene.CheckSpaceKey:
                if (CheckSpaceKeyPressed())
                {
                    currentScene = SelectScene.CheckGoal;
                    Debug.Log("Update��switch����2�ڂ�if��");
                }
                else
                {
                    currentScene = SelectScene.ProccesInput; // �p�X���[�h���͂��I�������Ăѓ��͏����ɖ߂�
                    Debug.Log("�p�X���[�h���I������ۂɏo�Ă���else��");
                }
                break;
            case SelectScene.CheckGoal:
                CheckGoalReached();
                if (!goalReached) // �S�[���ɓ��B���Ă��Ȃ��ꍇ�A�Ăѓ��͏����ɖ߂�
                {
                    currentScene = SelectScene.ProccesInput;
                }
                break;
            default:
                break;
        }

        // �S�[���ɓ��B���Ă��邩�ǂ����𖈃t���[���`�F�b�N
        CheckGoalReached();
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

    private bool CheckSpaceKeyPressed()
    {
        bool spaceKeyPressed = false; // �X�y�[�X�L�[�������ꂽ���ǂ����̃t���O

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

                    // Coroutine���J�n����
                    StartCoroutine(WaitAndUnfreeze(freezeAndInput));

                    spaceKeyPressed = true; // �X�y�[�X�L�[�������ꂽ�ƃt���O�𗧂Ă�
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

                        // Coroutine���J�n����
                        StartCoroutine(WaitAndUnfreeze(freezeAndInput));

                        spaceKeyPressed = true; // �X�y�[�X�L�[�������ꂽ�ƃt���O�𗧂Ă�
                    }
                }
            }
        }

        return spaceKeyPressed; // �X�y�[�X�L�[�������ꂽ���ǂ����̌��ʂ�Ԃ�
    }



    private IEnumerator WaitAndUnfreeze(FreezeAndInput freezeAndInput)
    {
        bool passwordCorrect = false;

        // �p�X���[�h�����������ǂ������m�F����܂Ń��[�v����
        while (!passwordCorrect)
        {
            // ESC�L�[�������ꂽ���ǂ������`�F�b�N����
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // ESC�L�[�������ꂽ�ꍇ�̏���
                // �����ł͓�����Ԃ��������Ȃ��܂܁A�������I���������ʂɖ߂�
                Debug.Log("ESC�L�[�������ꂽ���߁A�����ʂɖ߂�܂��B");

                // �t���[�Y��ʂ���鏈��
                CloseFreezeScreen(); // ���̃��\�b�h�͓K�؂Ȃ��̂ɒu�������Ă�������

                SetFrozen(false); // �v���C���[�̓�������������

                yield break; // Coroutine���I�����Č��̏����ɖ߂�
            }

            // �p�X���[�h�����������ǂ����𑦍��Ƀ`�F�b�N����
            if (freezeAndInput.IsPasswordCorrect())
            {
                freezeAndInput.Unfreeze();
                SetFrozen(false); // �v���C���[�̓�������������

                // �p�X���[�h���������ꍇ�̏���
                currentScene = SelectScene.ProccesInput;
                Debug.Log("�p�X���[�h�����������߁A�A���t���[�Y����܂����B�����ɂ͒ʂ�܂����B");

                passwordCorrect = true; // �p�X���[�h�����������Ƃ��t���O�Ŏ���
            }
            else
            {
                // �p�X���[�h���������Ȃ��ꍇ�̏���
                //Debug.Log("�p�X���[�h������������܂���B�v���C���[�͂܂���������Ă��܂��B");
                SetFrozen(true); // �v���C���[�𓀌���Ԃɂ����܂܂Ƃ���

                yield return null; // 1�t���[���ҋ@����
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

    // �v���C���[�̎��͂ɃM�~�b�N�̕ǂ����邩�ǂ������`�F�b�N����֐�
    private bool HasNearbyGimmickWall(int x, int y)
    {
        // �㉺���E�̈ʒu���`�F�b�N���A�ǂꂩ��ł��M�~�b�N�̕ǂ������true��Ԃ�
        return Ground.map[y, x - 1] == 3 || // ���ɃM�~�b�N�̕ǂ����邩�`�F�b�N
        Ground.map[y, x + 1] == 3 || // �E�ɃM�~�b�N�̕ǂ����邩�`�F�b�N
        Ground.map[y - 1, x] == 3 || // ��ɃM�~�b�N�̕ǂ����邩�`�F�b�N
        Ground.map[y + 1, x] == 3; // ���ɃM�~�b�N�̕ǂ����邩�`�F�b�N
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

