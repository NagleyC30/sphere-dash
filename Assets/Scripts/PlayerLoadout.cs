using UnityEngine;

/// <summary>
/// Applies shop purchases to the player at the start of a level. Attach to the
/// player object (next to PlayerMovement / PlayerAbilities).
///   - Equipped skin color -> player Renderer.
///   - Speed upgrade level  -> added to PlayerMovement.speed.
/// The speed upgrade is applied in Awake so PlayerAbilities.Start reads the
/// upgraded value as its base (the speed power then multiplies the upgraded base).
/// The "start shield" one-off is honored in PlayerAbilities.Start.
/// </summary>
public class PlayerLoadout : MonoBehaviour
{
    [Tooltip("Speed added per purchased speed-upgrade level.")]
    public float speedPerUpgrade = 1.5f;

    [Tooltip("Player mesh renderer; auto-found in children if left empty.")]
    public Renderer playerRenderer;

    void Awake()
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null)
            pm.speed += speedPerUpgrade * SaveManager.GetUpgradeLevel("speed");
    }

    void Start()
    {
        if (playerRenderer == null) playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer != null)
            playerRenderer.material.color = SaveManager.EquippedColor;
    }
}
