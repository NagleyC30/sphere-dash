using UnityEngine;

/// <summary>
/// A collectible power-up. Put this on a prefab with a trigger collider, pick a
/// Type in the Inspector, and place/spawn it in a level. On contact with the
/// player it applies its effect via PlayerAbilities and disappears.
///
/// IMPORTANT: do NOT tag the prefab "PickUp" — that tag is scored/collected by
/// PlayerMovement. Use its own tag (e.g. "PowerUp") or leave it untagged; this
/// script detects the player by component, not tag.
/// </summary>
public class PowerUp : MonoBehaviour
{
    public enum Type { Speed, Shield, FreezeEnemies, TimeBonus, Magnet }

    public Type type = Type.Speed;

    [Tooltip("Seconds the effect lasts (ignored by TimeBonus).")]
    public float duration = 5f;

    [Tooltip("Seconds added to the clock (TimeBonus only).")]
    public float timeBonusSeconds = 10f;

    void OnTriggerEnter(Collider other)
    {
        PlayerAbilities abilities = other.GetComponent<PlayerAbilities>();
        if (abilities == null) return; // only the player carries PlayerAbilities

        abilities.Activate(this);
        AudioManager.instance?.PlayPickup();
        gameObject.SetActive(false);
    }
}
