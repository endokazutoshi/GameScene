using System.Collections;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject[] _goalPrefabs; // �S�[���p��3��Prefab���Z�b�g���邽�߂̔z��
    [SerializeField] private GameObject _PIN;

    const int _nLengthStart = 1;
    const int _nWidthStart = 1;
    const int _nLengthEnd = 22;
    const int _nWidthEnd = 9;
    const int _nCenter = 11;

    [SerializeField] private int _length;
    [SerializeField] private int _width;

    // 2D�}�b�v�f�[�^: 0�͕ǁA1�͈ړ��}�X�A2�̓S�[���}�X�A3�ȍ~�̓M�~�b�N�}�X
    public static int[,] map = {
        {0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    private void Start()
    {
        // �X�e�[�W�̈ړ��}�X�A�ǃ}�X�̔z�u�̏���
        for (int length = 0; length < _length; length++)
        {
            for (int width = 0; width < _width; width++)
            {
                if (length >= _nLengthStart && length < _nLengthEnd && width >= _nWidthStart && width < _nWidthEnd) // ���̍��W��0����22�͈̔͂̏ꍇ
                {
                    int tileType = map[width, length];//������Map�̏����擾���邽�߂�tileType��map�̏�������
                    if (tileType == 0)//map�̏���0�������ꍇ�͐i�߂Ȃ�
                    {
                        Instantiate(_wallPrefab, new Vector3(length, width, 0), Quaternion.identity, transform);//�ǂ̕\��
                    }
                    else if (tileType == 1)//map�̏���1�������ꍇ�͐i�߂�
                    {
                        Instantiate(_tilePrefab, new Vector3(length, width, 0), Quaternion.identity, transform);//�ړ��ł���}�X(�^�C��)�̕\��
                    }
                    else if (tileType == 2)//map�̏���2�������ꍇ�̓S�[������(�����Ƃ��ė�����Player���S�[�����Ȃ��Ƃ����Ȃ�)
                    {
                        Vector3 goalPosition = new Vector3(length, width, 0);
                        StartCoroutine(SwitchGoalPrefab(goalPosition));
                    }
                    else if (tileType == 3)//map�̏��łR�������ꍇ�̓M�~�b�N��W�J����悤�ɂ���B(����ɂ��M�~�b�N�͕ʁX�ɂȂ�܂��B
                    {
                        Instantiate(_PIN, new Vector3(length, width, 0), Quaternion.identity, transform);
                    }
                }
                else // ����ȊO�̏ꍇ
                {
                    Instantiate(_wallPrefab, new Vector3(length, width, 0), Quaternion.identity, transform);
                }
            }
        }
        //�E�E�E�E�E�E�E�E�E�E�E�E�E�E�E�����܂ł��ǂƏ��̔z�u�E�E�E�E�E�E�E�E�E�E�E�E�E�E�E
    }

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
            yield return new WaitForSeconds(1.0f);
        }
    }
}
