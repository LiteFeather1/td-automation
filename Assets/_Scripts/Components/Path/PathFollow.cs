using UnityEngine;

public class PathFollow : MonoBehaviour
{
    [SerializeField] private Path _path;
    [SerializeField] private float _speed;
    private int _currentPointIndex = 0;

    public System.Action OnPathFinished { get; set; } 

    public void SetPath(Path path) => _path = path;

    private void Update()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            _path.Points[_currentPointIndex].position,
            _speed * Time.deltaTime
        );

        if (float.Epsilon < Vector2.Distance(
            transform.position, _path.Points[_currentPointIndex].position
        ))
            return;

        _currentPointIndex++;

        if (_currentPointIndex == _path.Points.Length)
        {
            enabled = false;
            OnPathFinished?.Invoke();
        }
    }
}
