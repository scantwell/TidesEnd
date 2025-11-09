namespace TidesEnd.Combat
{
    public interface IWeapon
    {
        /// <summary>
        /// Fires the weapon on client side (visual effects only, no damage)
        /// </summary>
        void FireClient();

        /// <summary>
        /// Fires the weapon on server side (authoritative damage application)
        /// </summary>
        /// <param name="validate">If true, validates CanFire and updates state (for remote clients). If false, skips validation (for host).</param>
        void FireServer(bool validate = true);

        /// <summary>
        /// Reload the weapon
        /// </summary>
        void Reload();

        /// <summary>
        /// Check if weapon can fire (has ammo, not reloading, cooldown elapsed)
        /// </summary>
        bool CanFire();

        /// <summary>
        /// Whether the weapon is currently reloading
        /// </summary>
        bool IsReloading { get; }

        /// <summary>
        /// Current ammo in magazine
        /// </summary>
        int CurrentAmmo { get; }
    }
}
