using UnityEngine;

public class PathFollow : MonoBehaviour
{
    private IPath _path;
    [SerializeField] private float _speed;
    private int _currentSegmentIndex = 0;
    private int _currentPointIndex = 0;

    public System.Action OnPathFinished { get; set; } 

    public void SetPath(IPath path) => _path = path;

    public void Update()
    {
        var currentPoint = _path.GetPoint(_currentSegmentIndex, _currentPointIndex);
        transform.position = Vector2.MoveTowards(
            transform.position, currentPoint, _speed * Time.deltaTime
        );

        if (float.Epsilon < Vector2.Distance(transform.position, currentPoint))
            return;

        _currentPointIndex++;

        if (!_path.ReachedSegmentEnd(_currentSegmentIndex, _currentPointIndex))
            return;

        _currentSegmentIndex++;
        _currentPointIndex = 0;
        if (!_path.ReachedPathEnd(_currentSegmentIndex))
            return;

        OnPathFinished?.Invoke();
    }
}
