using UnityEngine;

namespace Utilities
{
    public static class GroundCheckUtility
    {
        public static bool IsGrounded(Vector2 position, float radius, LayerMask groundLayer )
        {
            return Physics2D.OverlapCircle(position, radius, groundLayer);
            
        }
        
    }
}