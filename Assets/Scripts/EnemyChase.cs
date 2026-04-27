using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float baseSpeed = 3f;
    private float speed;

    void Start()
    {
        int difficulty = PlayerPrefs.GetInt("Difficulty", 1);
        if (difficulty == 0) speed = baseSpeed * 0.6f;
        else if (difficulty == 1) speed = baseSpeed;
        else speed = baseSpeed * 1.6f;
    }

    void Update()
    {
        if (player == null || GameManager.instance == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, player.position) < 1f)
            GameManager.instance.TriggerLose();
    }
}
