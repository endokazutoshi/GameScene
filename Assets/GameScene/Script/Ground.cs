using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Ground : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject[] _goalPrefabs; // �S�[���p��3��Prefab���Z�b�g���邽�߂̔z��
    [SerializeField] private GameObject _PIN;
    [SerializeField] private GameObject _deadPrefab;
    [SerializeField] private GameObject[] _switchPrefabs;

    [SerializeField] private float _goalSceneTime;

    const int _nLengthStart = 1;
    const int _nWidthStart = 1;
    const int _nLengthEnd = 22;
    const int _nWidthEnd = 9;
    const int _nCenter = 11;

    [SerializeField] private int _length;
    [SerializeField] private int _width;

    // 2D�}�b�v�f�[�^: 0�͕ǁA1�͈ړ��}�X�A2�̓S�[���}�X�A3�ȍ~�̓M�~�b�N�}�X
    public static int[,] map = {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 2, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 2, 1, 1, 0},
        {0, 1, 1, 1, 0, 1, 1, 5, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0},
        {0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0},
        {0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 0},
        {0, 3, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    private void Start()
    {
        // �X�e�[�W�̈ړ��}�X�A�ǃ}�X�̔z�u�̏���
        for (int length = 0; length < _length; length++)
        {
            for (int width = 0; width < _width; width++)
            {
                int tileType;
                if (length >= _nLengthStart && length < _nLengthEnd && width >= _nWidthStart && width < _nWidthEnd) // ���̍��W��0����22�͈̔͂̏ꍇ
                {
                    tileType = map[width, length]; // ������Map�̏����擾���邽�߂�tileType��map�̏�������
                }
                else
                {
                    tileType = 0; // �O�g�̏ꍇ�͕ǂƂ���
                }

                if (tileType == 0) // map�̏���0�������ꍇ�͕ǂ�z�u����
                {
                    Instantiate(_wallPrefab, new Vector3(length, width, 0), Quaternion.identity, transform); // �ǂ̕\��
                    Debug.Log("�ǃ}�X��z�u: " + new Vector3(length, width, 0));
                }

                if (tileType == 1) // map�̏���1�������ꍇ�͐i�߂�
                {
                    Instantiate(_tilePrefab, new Vector3(length, width, 0), Quaternion.identity, transform); // �ړ��ł���}�X(�^�C��)�̕\��
                    Debug.Log("�^�C���}�X��z�u: " + new Vector3(length, width, 0));
                }

                if (tileType == 2) // map�̏���2�������ꍇ�̓S�[������(�����Ƃ��ė�����Player���S�[�����Ȃ��Ƃ����Ȃ�)
                {
                    Vector3 goalPosition = new Vector3(length, width, 0);
                    StartCoroutine(SwitchGoalPrefab(goalPosition));
                    Debug.Log("�S�[���}�X��z�u: " + goalPosition);
                }

                if (tileType == 3) // map�̏���3�������ꍇ�̓M�~�b�N��W�J����
                {
                    PlacePassword(new Vector3(length, width, 0));
                    Debug.Log("�Ïؔԍ���z�u: " + new Vector3(length, width, 0));
                }

                if (tileType == 4)
                {
                    Vector3 deadPosition = new Vector3(length, width, 0);
                    Debug.Log("���S�}�X��z�u: " + deadPosition);
                }

                if (tileType == 5)
                {
                    Switch(new Vector3(length, width, 0));
                    Debug.Log("�X�C�b�`��z�u: " + new Vector3(length, width, 0));
                }

                //if (tileType == 6)
                //{
                //    Vector3 specialTilePosition = new Vector3(length, width, 0);
                //    PlaceSpecialTile(specialTilePosition);
                //    Debug.Log("����}�X��z�u: " + specialTilePosition);
                //}
            }
        }
    }

    //�E�E�E�E�E�E�E�E�E�E�E�E�E�E�E�����܂ł��ǂƏ��̔z�u�E�E�E�E�E�E�E�E�E�E�E�E�E�E�E

    IEnumerator SwitchGoalPrefab(Vector3 position)
    {
        GameObject goalInstance = null;
        int goalIndex = 0;

        while (true)
        {
            // ���݂�Prefab���폜
            if (goalInstance != null)
            {
                Destroy(goalInstance);
            }

            // ����Prefab���C���X�^���X��
            goalInstance = Instantiate(_goalPrefabs[goalIndex], position, Quaternion.identity, transform);

            // �C���f�b�N�X���X�V
            goalIndex = (goalIndex + 1) % _goalPrefabs.Length;

            // 1�b�҂�
            yield return new WaitForSeconds(_goalSceneTime);
        }
    }

    private void PlacePassword(Vector3 position)
    {
        Instantiate(_PIN, position, Quaternion.identity, transform);
        Debug.Log("�M�~�b�N�}�X��z�u: " + position);

        // �Ïؔԍ��̎��͂ŃX�y�[�X�L�[�������ꂽ��Ïؔԍ��p�l����\��
        if (IsSpaceKeyPressedNearPosition(position))
        {
            // FreezeAndInput�N���X�̃C���X�^���X���擾
            FreezeAndInput freezeAndInput = FindObjectOfType<FreezeAndInput>();
            if (freezeAndInput != null)
            {
                // Freeze���\�b�h���Ăяo��
                freezeAndInput.Freeze();
            }
        }
    }

    // �w�肳�ꂽ�ʒu�̎��͂ŃX�y�[�X�L�[�������ꂽ���ǂ������`�F�b�N���郁�\�b�h
    private bool IsSpaceKeyPressedNearPosition(Vector3 position)
    {
        // �X�y�[�X�L�[��������Ă��邩�A�w�肳�ꂽ�ʒu�̎��͂Ƀv���C���[�����݂��邩���`�F�b�N
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 1f); // �w�肳�ꂽ�ʒu�̔��a1�̉~�̈���̃R���C�_�[���擾
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player")) // �v���C���[�^�O�����I�u�W�F�N�g�����݂���ꍇ
                {
                    return true; // �X�y�[�X�L�[�������ꂽ���v���C���[�����͂ɑ��݂���ꍇ��true��Ԃ�
                }
            }
        }
        return false;
    }



    private void Switch(Vector3 position)
    {
        if (_switchPrefabs.Length > 0) // _switchPrefabs����łȂ����Ƃ��m�F
        {
            Instantiate(_switchPrefabs[0], position, Quaternion.identity, transform); // �Ƃ肠�����ŏ���Prefab��z�u
            Debug.Log("�X�C�b�`��z�u: " + position);
        }
        else
        {
            Debug.LogError("�X�C�b�`��Prefab���Z�b�g����Ă��܂���B_switchPrefabs��Prefab���Z�b�g���Ă��������B");
        }
    }
    public int GetTileTypeAtPosition(Vector3 position)
    {
        // position �𐮐��ɕϊ����A�Ή�����^�C���^�C�v��Ԃ�
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        return map[y, x];
    }

        public int Width
        {
            get { return map.GetLength(1); }
        }
    public int Length
    {
        get { return map.GetLength(0); }
    }

    //// ���̃R�[�h...


}


