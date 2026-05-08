using System;
using UnityEngine;

namespace Utilities
{
    public static class FlipUtility
    {
        public static void FlipX(Transform objectTransform, Transform target, Vector3 orginalPosition )
        {
            if (target == null) return;
            
            Vector3 scale = objectTransform.localScale;

            if (objectTransform.position.x < target.position.x)
            {
                scale.x =Math.Abs(orginalPosition.x);
            }
            else
            {
                scale.x= -Math.Abs(orginalPosition.x);
            }
            
            objectTransform.localScale = scale;
        }
    
        
    }
}