using System.Collections;
using UnityEngine;

public class TowerProjectile : Tower
{
    private const float FADE_TIME = .25f;

    [Header("Projectile Tower")]
    [SerializeField] private LineRenderer _line;
    [SerializeField] private Transform _firePoint;

    protected override void DamageEnemy(Enemy enemy)
    {
        _line.SetPosition(0, _firePoint.position);
        _line.SetPosition(1, enemy.transform.position);
        StartCoroutine(Fade());
        enemy.Health.TakeDamage(_damage);

        IEnumerator Fade()
        {
            var eTime = 0f;
            while (eTime < FADE_TIME)
            {
                Set(1f - EaseInOutQuad(eTime / FADE_TIME));
                eTime += Time.deltaTime;
                yield return null;

                static float EaseInOutQuad(float x)=> (x < 0.5f) ?
                    (2f * x * x) : (1f - Mathf.Pow(-2f * x + 2f, 2f) * .5f);
            }

            Set(0f);

            void Set(float t)
            {
                var colour = new Color(1f, 1f, 1f, t);
                _line.startColor = colour;
                _line.endColor = colour;
            }
        }
    }
}
