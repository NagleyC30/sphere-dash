using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float baseSpeed = 3f;

    [Tooltip("How much the enemy anticipates the player's movement (0 = dumb " +
             "chase of current position, ~0.5 = cuts corners toward where " +
             "the player is heading).")]
    public float leadFactor = 0.5f;

    private float speed;
    private Rigidbody playerRb;
    private Vector3 lastPlayerPos;

    void Start()
    {
        int difficulty = PlayerPrefs.GetInt("Difficulty", 1);
        if (difficulty == 0) speed = baseSpeed * 0.6f;
        else if (difficulty == 1) speed = baseSpeed;
        else speed = baseSpeed * 1.6f;

        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
            lastPlayerPos = player.position;
        }
    }

    void Update()
    {
        if (player == null || GameManager.instance == null) return;

        // Freeze power-up: stop chasing and can't catch while it's active.
        if (PlayerAbilities.instance != null && PlayerAbilities.instance.EnemiesFrozen)
        {
            lastPlayerPos = player.position; // avoid a velocity spike on unfreeze
            return;
        }

        // Predict where the player is heading and aim there (interception).
        Vector3 playerVelocity = playerRb != null
            ? playerRb.linearVelocity
            : (player.position - lastPlayerPos) / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPlayerPos = player.position;

        float distance = Vector3.Distance(transform.position, player.position);
        float leadTime = (speed > 0.01f ? distance / speed : 0f) * leadFactor;
        Vector3 aim = player.position + playerVelocity * leadTime;

        transform.position = Vector3.MoveTowards(
            transform.position,
            aim,
            speed * Time.deltaTime
        );

        // Catch is judged against the player's actual position, not the aim point.
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
