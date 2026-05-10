using UnityEngine;
using Utilities;

namespace Combat
{
    public class Bullet : ProjectileBase
    {
        [Header("Targeting")]
        [SerializeField] private bool isPlayerBullet = true;
        [SerializeField] private string playerTag = GameTags.Player;
        [SerializeField] private string enemyTag = GameTags.Enemy;
        protected override bool ShouldIgnore(Collider2D collision)
        {
            // Ignore the shooter side so player bullets don't disappear on player contact.
            return isPlayerBullet && collision.CompareTag(playerTag);
        }

        protected override bool CanDamageTarget(Collider2D collision)
        {
            return !isPlayerBullet || collision.CompareTag(enemyTag);
        }
    }
}