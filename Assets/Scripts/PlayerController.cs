using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveDistance = 1f; 
    public int maxMoves = 3; 
    private int currentMoves = 0; 

    public TextMeshProUGUI movesText; 
    public TextMeshProUGUI collectText; 
    public GameObject winPanel; 
    public GameObject gameOverPanel;
    private int collectedItems = 0; 
    public int requiredCollectibles;

    private bool isGameOver = false; 

    public AudioSource audioSource; 
    public AudioClip stepSoundClip; 
    public AudioClip collectSoundClip;


    private void Start()
    {
        UpdateMovesText();
        UpdateCollectText();

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // private void Update()
    // {
    //     if (isGameOver)
    //         return; 
    //
    //     if (currentMoves < maxMoves)
    //     {
    //         if (Input.GetKeyDown(KeyCode.W))
    //         {
    //             TryMove(Vector3.forward);
    //         }
    //         else if (Input.GetKeyDown(KeyCode.S))
    //         {
    //             TryMove(Vector3.back);
    //         }
    //         else if (Input.GetKeyDown(KeyCode.A))
    //         {
    //             TryMove(Vector3.left);
    //         }
    //         else if (Input.GetKeyDown(KeyCode.D))
    //         {
    //             TryMove(Vector3.right);
    //         }
    //     }
    // }

    public void InputHandler(int direction )
    {
        if (currentMoves >= maxMoves) return; 
        if (direction == 0) TryMove(Vector3.forward);
        if (direction == 1) TryMove(Vector3.back);
        if (direction == 2) TryMove(Vector3.left);
        if (direction == 3) TryMove(Vector3.right);
    }

    void TryMove(Vector3 direction)
    {
        Vector3 newPosition = transform.position + direction * moveDistance;

        if (!IsPositionBlocked(newPosition))
        {
            UpdatePlayerRotation(direction);

            transform.position = newPosition;

            currentMoves++;

            UpdateMovesText();

            if (audioSource != null && stepSoundClip != null)
            {
                audioSource.PlayOneShot(stepSoundClip);
            }
        }

        if (currentMoves >= maxMoves)
        {
            CheckGameOver();
        }
    }

    bool IsPositionBlocked(Vector3 position)
    {
        // Определяем размер области для проверки (можно настроить в зависимости от вашего коллайдера)
        Vector3 checkSize = new Vector3(moveDistance, 1f, moveDistance);

        // Проверяем, есть ли объекты с тегом "NoMoves" в указанной позиции
        Collider[] hitColliders = Physics.OverlapBox(position, checkSize / 2, Quaternion.identity);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("NoMoves"))
            {
                return true; // Если найден объект с тегом "NoMoves", позиция заблокирована
            }
        }
        return false; // Позиция свободна
    }

    void UpdatePlayerRotation(Vector3 direction)
    {
        if (direction == Vector3.forward)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (direction == Vector3.back)
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if (direction == Vector3.left)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (direction == Vector3.right)
        {
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
        UpdateMovesText(); 
        CheckGameOver();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем, если объект имеет тег "Collect"
        if (collision.gameObject.CompareTag("Collect"))
        {
            // Уничтожаем объект
            Destroy(collision.gameObject);

            // Увеличиваем количество собранных предметов
            collectedItems++;

            // Обновляем текст для отображения количества собранных предметов
            UpdateCollectText();

            // Воспроизводим звук сбора
            if (audioSource != null && collectSoundClip != null)
            {
                audioSource.PlayOneShot(collectSoundClip);
            }

            if (collectedItems >= requiredCollectibles)
            {
                if (winPanel != null)
                {
                    winPanel.SetActive(true);
                    var currentSceneName = SceneManager.GetActiveScene().name;

                    var levelNumberStr = currentSceneName.Substring(5);

                    if (!int.TryParse(levelNumberStr, out var levelNumber)) return;
                    
                    LockLevelSystem.LevelUp(levelNumber);
                }

                isGameOver = true;
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            isGameOver = true;

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }

    private void CheckGameOver()
    {
        if (currentMoves >= maxMoves && collectedItems < requiredCollectibles)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            isGameOver = true;
        }
    }
}      