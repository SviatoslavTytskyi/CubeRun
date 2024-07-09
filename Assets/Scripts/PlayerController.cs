using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveDistance = 1f; // Расстояние перемещения
    public int maxMoves = 3; // Максимальное количество перемещений
    private int currentMoves = 0; // Текущее количество перемещений

    public TextMeshProUGUI movesText; // Ссылка на TMP Text для отображения количества ходов
    public TextMeshProUGUI collectText; // Ссылка на TMP Text для отображения количества собранных предметов
    public GameObject winPanel; // Ссылка на панель победы
    public GameObject gameOverPanel; // Ссылка на панель Game Over

    private int collectedItems = 0; // Количество собранных предметов
    public int requiredCollectibles; // Количество предметов, необходимых для победы

    private bool isGameOver = false; // Флаг, указывающий, что игра окончена

    public AudioSource audioSource; // Ссылка на AudioSource
    public AudioClip stepSoundClip; // Звук шага
    public AudioClip collectSoundClip; // Звук сбора

    private void Start()
    {
        // Инициализируем текст с начальным количеством ходов и собранных предметов
        UpdateMovesText();
        UpdateCollectText();

        // Скрываем панели в начале игры
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
            return; // Если игра окончена, прекращаем обработку ввода

        // Проверяем, если игрок нажимает клавишу
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
        // Вычисляем конечную позицию
        Vector3 newPosition = transform.position + direction * moveDistance;

        // Проверяем, свободна ли конечная позиция от объектов с тегом "NoMoves"
        if (!IsPositionBlocked(newPosition))
        {
            // Поворачиваем игрока в направлении движения
            UpdatePlayerRotation(direction);

            // Перемещаем игрока на новую позицию
            transform.position = newPosition;

            // Увеличиваем счетчик перемещений
            currentMoves++;

            // Обновляем текст на экране
            UpdateMovesText();

            // Воспроизводим звук шага
            if (audioSource != null && stepSoundClip != null)
            {
                audioSource.PlayOneShot(stepSoundClip);
            }
        }

        // Если достигнут предел ходов, проверяем, собраны ли все необходимые предметы
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
        // Определяем нужный угол поворота
        if (direction == Vector3.forward)
        {
            // Смотрит вперед (90 градусов)
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (direction == Vector3.back)
        {
            // Смотрит назад (-90 градусов)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if (direction == Vector3.left)
        {
            // Смотрит влево (0 градусов)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (direction == Vector3.right)
        {
            // Смотрит вправо (180 градусов)
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
        UpdateMovesText(); // Обновляем текст, чтобы отразить новое количество ходов
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

            // Проверяем, если все необходимые предметы собраны
            if (collectedItems >= requiredCollectibles)
            {
                // Показываем панель победы
                if (winPanel != null)
                {
                    winPanel.SetActive(true);
                }

                // Останавливаем игру
                isGameOver = true;
            }
        }

        // Проверяем, если объект имеет тег "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Устанавливаем флаг окончания игры
            isGameOver = true;

            // Показываем панель Game Over
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }

    private void CheckGameOver()
    {
        // Если потрачены все ходы и не собраны все необходимые предметы
        if (currentMoves >= maxMoves && collectedItems < requiredCollectibles)
        {
            // Показываем панель Game Over
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // Останавливаем игру
            isGameOver = true;
        }
    }
}      