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
            var halfPoint = (_firePoint.position + enemy.transform.position) * .5f;
            var startPos = _line.GetPosition(0);
            var defaultWidth = _line.startWidth;
            var eTime = 0f;
            while (eTime < ANIMATION_TIME)
            {
                Set(1f - Helpers.EaseInOutQuad(eTime / ANIMATION_TIME));
                eTime += Time.deltaTime;
                yield return null;
            }

            Set(0f);

            void Set(float t)
            {
                var colour = new Color(1f, 1f, 1f, t);
                _line.startColor = colour;
                _line.endColor = colour;
                var width = defaultWidth + defaultWidth * t * 2f;
                _line.startWidth = width;
                _line.endWidth = width;
                _line.SetPosition(0, Vector3.Lerp(halfPoint, startPos, t));
            }
        }
    }
}
