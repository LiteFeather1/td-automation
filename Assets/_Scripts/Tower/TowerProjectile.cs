using System.Collections;
using UnityEngine;

public class TowerProjectile : Tower
{
    private const float ANIMATION_TIME = .25f;

    [Header("Projectile Tower")]
    [SerializeField] private LineRenderer _line;
    [SerializeField] private Transform _firePoint;

    protected override void DamageEnemy(Enemy enemy)
    {
        enemy.Health.TakeDamage(_damage);

        _line.SetPosition(0, _firePoint.position);
        _line.SetPosition(1, enemy.transform.position);
        StartCoroutine(Fade());

        IEnumerator Fade()
        {
            Vector3 halfPoint = (_firePoint.position + enemy.transform.position) * .5f;
            Vector3 startPos = _line.GetPosition(0);
            float defaultWidth = _line.startWidth;
            float eTime = 0f;
            while (eTime < ANIMATION_TIME)
            {
                Set(1f - Helpers.EaseInOutQuad(eTime / ANIMATION_TIME));
                eTime += Time.deltaTime;
                yield return null;
            }

            Set(0f);

            void Set(float t)
            {
                Color colour = new(1f, 1f, 1f, t);
                _line.startColor = colour;
                _line.endColor = colour;
                float width = defaultWidth + defaultWidth * t * 2f;
                _line.startWidth = width;
                _line.endWidth = width;
                _line.SetPosition(0, Vector3.Lerp(halfPoint, startPos, t));
            }
        }
    }
}
