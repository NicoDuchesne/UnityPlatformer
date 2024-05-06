using UnityEngine;



public class WallDetector : MonoBehaviour
{

    [Header("Wall Detection")]
    [SerializeField] private Transform[] _wallRightDetectionPoints;
    [SerializeField] private Transform[] _wallLefDetectionPoints;
    [SerializeField] private float _detectionLength = 0.1f;
    [SerializeField] private LayerMask _wallLayerMask;

    public bool DetectWallRight()
    {
        foreach (Transform detectionPoint in _wallRightDetectionPoints)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(
                detectionPoint.position,
                Vector2.right,
                _detectionLength,
                _wallLayerMask
                );

            if (hitResult.collider != null)
            {
                return true;
            }
        }

        return false;
    }

    public bool DetectWallLeft()
    {
        foreach (Transform detectionPoint in _wallLefDetectionPoints)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(
                detectionPoint.position,
                Vector2.left,
                _detectionLength,
                _wallLayerMask
                );

            if (hitResult.collider != null)
            {
                return true;
            }
        }

        return false;
    }
    
}
