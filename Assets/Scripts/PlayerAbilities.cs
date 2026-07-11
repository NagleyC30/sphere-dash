using System.Collections;
using UnityEngine;

/// <summary>
/// Holds the player's active power-up state and runs their timers. Attach to the
/// player object (alongside PlayerMovement). Power-ups call Activate(); other
/// systems read the public state:
///   - EnemyChase checks EnemiesFrozen and calls TryAbsorbHit() on a catch.
///   - Magnet pulls nearby pickups toward the player each frame.
/// Timers use scaled time, so they pause correctly with the pause menu.
/// </summary>
public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities instance;

    [Header("Speed power")]
    public float speedMultiplier = 1.8f;

    [Header("Magnet power")]
    public float magnetRadius = 5f;
    public float magnetPullSpeed = 8f;

    public bool HasShield { get; private set; }
    public bool EnemiesFrozen { get; private set; }
    public bool MagnetActive { get; private set; }

    private PlayerMovement movement;
    private float baseSpeed;

    private Coroutine speedRoutine;
    private Coroutine shieldRoutine;
    private Coroutine freezeRoutine;
    private Coroutine magnetRoutine;

    void Awake() => instance = this;
    void OnDestroy() { if (instance == this) instance = null; }

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        if (movement != null) baseSpeed = movement.speed;

        // "Start shield" shop unlock: begin the level with one shield charge.
        // No timer — it persists until an enemy hit consumes it (TryAbsorbHit).
        if (SaveManager.IsOwned("startshield")) HasShield = true;
    }

    void Update()
    {
        if (MagnetActive) PullNearbyPickups();
    }

    public void Activate(PowerUp p)
    {
        switch (p.type)
        {
            case PowerUp.Type.Speed:
                Restart(ref speedRoutine, SpeedRoutine(p.duration));
                break;
            case PowerUp.Type.Shield:
                Restart(ref shieldRoutine, ShieldRoutine(p.duration));
                break;
            case PowerUp.Type.FreezeEnemies:
                Restart(ref freezeRoutine, FreezeRoutine(p.duration));
                break;
            case PowerUp.Type.Magnet:
                Restart(ref magnetRoutine, MagnetRoutine(p.duration));
                break;
            case PowerUp.Type.TimeBonus:
                GameManager.instance?.AddTime(p.timeBonusSeconds);
                break;
        }
    }

    /// <summary>
    /// Called by EnemyChase on a catch. If a shield is up it's consumed and the
    /// player survives (returns true); otherwise returns false and the caller
    /// should trigger the loss.
    /// </summary>
    public bool TryAbsorbHit()
    {
        if (!HasShield) return false;

        HasShield = false;
        if (shieldRoutine != null) { StopCoroutine(shieldRoutine); shieldRoutine = null; }
        AudioManager.instance?.PlayLose(); // placeholder shield-break blip
        return true;
    }

    void Restart(ref Coroutine handle, IEnumerator routine)
    {
        if (handle != null) StopCoroutine(handle);
        handle = StartCoroutine(routine);
    }

    IEnumerator SpeedRoutine(float duration)
    {
        if (movement != null) movement.speed = baseSpeed * speedMultiplier;
        yield return new WaitForSeconds(duration);
        if (movement != null) movement.speed = baseSpeed;
        speedRoutine = null;
    }

    IEnumerator ShieldRoutine(float duration)
    {
        HasShield = true;
        yield return new WaitForSeconds(duration);
        HasShield = false;
        shieldRoutine = null;
    }

    IEnumerator FreezeRoutine(float duration)
    {
        EnemiesFrozen = true;
        yield return new WaitForSeconds(duration);
        EnemiesFrozen = false;
        freezeRoutine = null;
    }

    IEnumerator MagnetRoutine(float duration)
    {
        MagnetActive = true;
        yield return new WaitForSeconds(duration);
        MagnetActive = false;
        magnetRoutine = null;
    }

    void PullNearbyPickups()
    {
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");
        foreach (GameObject p in pickups)
        {
            if (p == null || !p.activeInHierarchy) continue;
            if (Vector3.Distance(p.transform.position, transform.position) <= magnetRadius)
            {
                p.transform.position = Vector3.MoveTowards(
                    p.transform.position,
                    transform.position,
                    magnetPullSpeed * Time.deltaTime);
            }
        }
    }
}
