public interface ILivable
{
    public float CurrentHealth { get; set; }
    public void ApplyDamage(DamageInfo damageInfo);
}