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

        // Freeze power-up: stop chasing and can't catch while it's active.
        if (PlayerAbilities.instance != null && PlayerAbilities.instance.EnemiesFrozen)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, player.position) < 1f)
        {
            // Shield power-up absorbs the hit; knock the enemy back so it doesn't
            // instantly re-trigger the next frame.
            if (PlayerAbilities.instance != null && PlayerAbilities.instance.TryAbsorbHit())
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, player.position, -3f);
                return;
            }

            GameManager.instance.TriggerLose();
        }
    }
}
