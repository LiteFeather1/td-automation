using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private Transform[] _points;

    public Transform[] Points => _points;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_points.Length == 0)
            return;

        Gizmos.color = new(255f, 69f, 0f, 1f);
        Gizmos.DrawLine(transform.position, _points[0].position);

        for (var i = 0; i < _points.Length - 1; i++)
        {
            if (_points[i] == null || _points[i + 1] == null)
                continue;

            Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
        }
    }
#endif
}
