using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveDistance = 1f; // ���������� �����������
    public int maxMoves = 3; // ������������ ���������� �����������
    private int currentMoves = 0; // ������� ���������� �����������

    public TextMeshProUGUI movesText; // ������ �� TMP Text ��� ����������� ���������� �����
    public TextMeshProUGUI collectText; // ������ �� TMP Text ��� ����������� ���������� ��������� ���������
    public GameObject winPanel; // ������ �� ������ ������
    public GameObject gameOverPanel; // ������ �� ������ Game Over

    private int collectedItems = 0; // ���������� ��������� ���������
    public int requiredCollectibles; // ���������� ���������, ����������� ��� ������

    private bool isGameOver = false; // ����, �����������, ��� ���� ��������

    public AudioSource audioSource; // ������ �� AudioSource
    public AudioClip stepSoundClip; // ���� ����
    public AudioClip collectSoundClip; // ���� �����

    private void Start()
    {
        // �������������� ����� � ��������� ����������� ����� � ��������� ���������
        UpdateMovesText();
        UpdateCollectText();

        // �������� ������ � ������ ����
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (isGameOver)
            return; // ���� ���� ��������, ���������� ��������� �����

        // ���������, ���� ����� �������� �������
        if (currentMoves < maxMoves)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                TryMove(Vector3.forward);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                TryMove(Vector3.back);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                TryMove(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                TryMove(Vector3.right);
            }
        }
    }

    void TryMove(Vector3 direction)
    {
        // ��������� �������� �������
        Vector3 newPosition = transform.position + direction * moveDistance;

        // ���������, �������� �� �������� ������� �� �������� � ����� "NoMoves"
        if (!IsPositionBlocked(newPosition))
        {
            // ������������ ������ � ����������� ��������
            UpdatePlayerRotation(direction);

            // ���������� ������ �� ����� �������
            transform.position = newPosition;

            // ����������� ������� �����������
            currentMoves++;

            // ��������� ����� �� ������
            UpdateMovesText();

            // ������������� ���� ����
            if (audioSource != null && stepSoundClip != null)
            {
                audioSource.PlayOneShot(stepSoundClip);
            }
        }

        // ���� ��������� ������ �����, ���������, ������� �� ��� ����������� ��������
        if (currentMoves >= maxMoves)
        {
            CheckGameOver();
        }
    }

    bool IsPositionBlocked(Vector3 position)
    {
        // ���������� ������ ������� ��� �������� (����� ��������� � ����������� �� ������ ����������)
        Vector3 checkSize = new Vector3(moveDistance, 1f, moveDistance);

        // ���������, ���� �� ������� � ����� "NoMoves" � ��������� �������
        Collider[] hitColliders = Physics.OverlapBox(position, checkSize / 2, Quaternion.identity);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("NoMoves"))
            {
                return true; // ���� ������ ������ � ����� "NoMoves", ������� �������������
            }
        }
        return false; // ������� ��������
    }

    void UpdatePlayerRotation(Vector3 direction)
    {
        // ���������� ������ ���� ��������
        if (direction == Vector3.forward)
        {
            // ������� ������ (90 ��������)
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (direction == Vector3.back)
        {
            // ������� ����� (-90 ��������)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if (direction == Vector3.left)
        {
            // ������� ����� (0 ��������)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (direction == Vector3.right)
        {
            // ������� ������ (180 ��������)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    void UpdateMovesText()
    {
        if (movesText != null)
        {
            movesText.text = "" + currentMoves + "/" + maxMoves;
        }
    }

    void UpdateCollectText()
    {
        if (collectText != null)
        {
            collectText.text = "" + collectedItems + "/" + requiredCollectibles;
        }
    }

    public void AddBonusMoves(int bonusMoves)
    {
        maxMoves += bonusMoves;
        Debug.Log("Added bonus moves: " + bonusMoves);
        UpdateMovesText(); // ��������� �����, ����� �������� ����� ���������� �����
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���������, ���� ������ ����� ��� "Collect"
        if (collision.gameObject.CompareTag("Collect"))
        {
            // ���������� ������
            Destroy(collision.gameObject);

            // ����������� ���������� ��������� ���������
            collectedItems++;

            // ��������� ����� ��� ����������� ���������� ��������� ���������
            UpdateCollectText();

            // ������������� ���� �����
            if (audioSource != null && collectSoundClip != null)
            {
                audioSource.PlayOneShot(collectSoundClip);
            }

            // ���������, ���� ��� ����������� �������� �������
            if (collectedItems >= requiredCollectibles)
            {
                // ���������� ������ ������
                if (winPanel != null)
                {
                    winPanel.SetActive(true);
                }

                // ������������� ����
                isGameOver = true;
            }
        }

        // ���������, ���� ������ ����� ��� "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // ������������� ���� ��������� ����
            isGameOver = true;

            // ���������� ������ Game Over
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }

    private void CheckGameOver()
    {
        // ���� ��������� ��� ���� � �� ������� ��� ����������� ��������
        if (currentMoves >= maxMoves && collectedItems < requiredCollectibles)
        {
            // ���������� ������ Game Over
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // ������������� ����
            isGameOver = true;
        }
    }
}      