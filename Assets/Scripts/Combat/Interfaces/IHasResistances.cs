/// <summary>
/// Optional: Targets with damage resistances
/// </summary>
namespace TidesEnd.Combat
{
    public interface IHasResistances
    {
        float GetResistance(DamageType damageType);
    }
}