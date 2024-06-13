using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance; // �V���O���g���C���X�^���X

    private void Awake()
    {
        // �V���O���g���p�^�[���̎���
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CheckGoalReached(Player player)
    {
        int targetX = Mathf.RoundToInt(player.GetTargetPos().x);
        int targetY = Mathf.RoundToInt(player.GetTargetPos().y);

        // �v���C���[���ڕW�ʒu�Ɋ��S�ɓ��B���Ă��邩�ǂ������`�F�b�N
        if (Mathf.Approximately(player.transform.position.x, player.GetTargetPos().x) && Mathf.Approximately(player.transform.position.y, player.GetTargetPos().y))
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
}
