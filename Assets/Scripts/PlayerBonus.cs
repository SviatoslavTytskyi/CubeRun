using UnityEngine;

public class PlayerBonus : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        // Получаем компонент PlayerController на игроке
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController component not found on the player");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем теги объекта, с которым произошло столкновение
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
            // Перемещаем игрока на место бонуса
            transform.position = collision.transform.position;

            // Добавляем или убираем количество ходов
            playerController.AddBonusMoves(bonusMoves);

            // Уничтожаем объект бонуса
            Destroy(collision.gameObject);
        }
    }
}
