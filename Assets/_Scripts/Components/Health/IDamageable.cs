using System;

public interface IDamageable
{
    public float HP { get; }
    public float MaxHP { get; }
    public float Defence { get; }

    public Action<float> OnDamageTaken { get; set; }
    public Action<float> OnHealed { get; set; }

    public Action OnDied { get; set; }

    public void TakeDamage(float damage);

    public void Heal(float heal);
}
