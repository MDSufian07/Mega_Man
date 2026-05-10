using Combat;
using UnityEngine;
using Utilities;

namespace Boss.YellowDevil
{
    public class YellowDevilBullet : ProjectileBase
    {
        [Header("Targeting")]
        [SerializeField] private string playerTag = GameTags.Player;
        public void SetPlayerTag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                playerTag = tag;
            }
        }

        protected override bool ShouldIgnore(Collider2D collision)
        {
            return false;
        }

        protected override bool CanDamageTarget(Collider2D collision)
        {
            return collision.CompareTag(playerTag);
        }
    }
}
