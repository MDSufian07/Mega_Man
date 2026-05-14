using UnityEngine;

namespace CagneyCarnation
{
    public class PolygonCollider2DUpdater : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private PolygonCollider2D _polygonCollider;

        private Sprite _currentSprite;

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _polygonCollider = GetComponent<PolygonCollider2D>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (_spriteRenderer == null || _polygonCollider == null || _spriteRenderer.sprite == null) return;

            if (_spriteRenderer.sprite != _currentSprite)
            {
                _currentSprite = _spriteRenderer.sprite;
                _polygonCollider.pathCount = _currentSprite.GetPhysicsShapeCount();

                var path = new System.Collections.Generic.List<Vector2>();

                for (int i = 0; i < _polygonCollider.pathCount; i++)
                {
                    path.Clear();
                    _currentSprite.GetPhysicsShape(i, path);
                    _polygonCollider.SetPath(i, path.ToArray());
                }
            }
        }
    }
}
