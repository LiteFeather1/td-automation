using System.Collections;
using UnityEngine;

public class ZapEffectManager : Singleton<ZapEffectManager>
{
    [SerializeField] private ObjectPool<LineRenderer> _effectPool;
    [SerializeField] private Vector2 _zapDurationRange = new(.333f, .5f);
    [SerializeField] private Vector2Int _zapUpdatesRange = new(4, 6);
    [SerializeField] private Vector2 _zapSegmentLengthRange = new(1f, 1.5f);
    [SerializeField] private Vector2 _zapSegmentOffsetRange = new(.333f, .666f);

    private void Start()
    {
        _effectPool.InitPool(null, true);
    }

    private void OnDestroy()
    {
        _effectPool.Deinit(null);
    }

    public void ZapEffect(Vector2 center, Vector2[] positions)
    {
        StartCoroutine(ZapEffect());

        IEnumerator ZapEffect()
        {
            var length = positions.Length;
            LineRenderer[] lines = new LineRenderer[length];
            for (int i = 0; i < length; i++)
            {
                lines[i] = _effectPool.GetObject();
                lines[i].SetPosition(0, center);

                float distance = Vector2.Distance(center, positions[i]);
                var segmentLength = Random.Range(_zapSegmentLengthRange.x, _zapSegmentLengthRange.y);
                lines[i].positionCount = distance > segmentLength
                        ? Mathf.FloorToInt(distance / segmentLength) + 2 : 4;

                lines[i].enabled = true;
            }

            int updates = Random.Range(_zapUpdatesRange.x, _zapUpdatesRange.y);
            WaitForSeconds wait = new(Random.Range(_zapDurationRange.x, _zapDurationRange.y) / updates);

            for (int i = 0; i < updates; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    LineRenderer line = lines[j];
                    int pointCount = line.positionCount;
                    for (int k = 1; k < pointCount - 1; k++)
                    {
                        Vector2 lerp = Vector2.Lerp(center, positions[j], (float)k / pointCount);
                        line.SetPosition(k, new(lerp.x + GetOffset(), lerp.y + GetOffset()));

                        float GetOffset()
                        {
                            return Random.Range(_zapSegmentOffsetRange.x, _zapSegmentOffsetRange.y)
                                * (Random.value >= .5f ? 1f : -1f);
                        }
                    }

                    line.SetPosition(pointCount - 1, positions[j]);
                }

                yield return wait;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                _effectPool.ReturnObject(lines[i]);
                lines[i].enabled = false;
            }
        }
    }

#region Test
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         Test();
    //     }
    // }

    // [ContextMenu("Test")]
    // private void Test()
    // {
    //     Vector2[] positions = new Vector2[8];
    //     for (var i = 0; i < positions.Length; i++)
    //         positions[i] = new(
    //             Random.value * 5f * (Random.value >= .5f ? 1f : -1f),
    //             Random.value * 5f * (Random.value >= .5f ? 1f : -1f)
    //         );

    //     LightningEffect(transform.position, positions);
    // }
#endregion
}
