using UnityEngine;

public class PlayerBonus : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        // �������� ��������� PlayerController �� ������
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController component not found on the player");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ��������� ���� �������, � ������� ��������� ������������
        string tag = collision.gameObject.tag;

        int bonusMoves = 0;

        switch (tag)
        {
            case "Bonus1":
                bonusMoves = 1;
                break;
            case "Bonus2":
                bonusMoves = 2;
                break;
            case "Bonus3":
                bonusMoves = 3;
                break;
            case "Bonus4":
                bonusMoves = 4;
                break;
            case "Bonus5":
                bonusMoves = 5;
                break;
            case "Disable1":
                bonusMoves = -1;
                break;
            case "Disable2":
                bonusMoves = -2;
                break;
            case "Disable3":
                bonusMoves = -3;
                break;
            case "Disable4":
                bonusMoves = -4;
                break;
            case "Disable5":
                bonusMoves = -5;
                break;
        }

        if (bonusMoves != 0)
        {
            // ���������� ������ �� ����� ������
            transform.position = collision.transform.position;

            // ��������� ��� ������� ���������� �����
            playerController.AddBonusMoves(bonusMoves);

            // ���������� ������ ������
            Destroy(collision.gameObject);
        }
    }
}
