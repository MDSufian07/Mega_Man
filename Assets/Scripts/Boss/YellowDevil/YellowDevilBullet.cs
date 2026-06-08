using Combat;
using UnityEngine;
using Utilities;

namespace Boss.YellowDevil
{
    public class YellowDevilBullet : ProjectileBase
    {
        [Header("Targeting")]
        [SerializeField] private GameTags playerTag = GameTags.Player;
        public void SetPlayerTag(GameTags tag)
        {
            if (!string.IsNullOrWhiteSpace(tag.ToString()))
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
            return collision.CompareTag(playerTag.ToString());
        }
    }
}
