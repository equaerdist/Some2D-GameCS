using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// Класс оружия
    /// </summary>
    public class Weapon
    {
        public DateTime ShotLastTime { get; set; } = DateTime.Now;
        public Physics Physic { get; set; }
        public int Range { get; }
        public int TimeBetweenShotsInMilliseconds { get; }
        public double Damage { get; }
        public int BulletsAmount { get; }
        public Bitmap Texture { get; }
        public Weapon(int range, double damage, int bulletsAmount, int rateFire, Bitmap texture = null)
        {
            Range = range;
            Damage = damage;
            BulletsAmount = bulletsAmount;
            Texture = texture;
            TimeBetweenShotsInMilliseconds = rateFire;
        }
    }
    public class Physics
    {
        public Weapon ForWeapon { get; }
        public Vector Velocity { get; }
        public double DamageDownCoefficentPerSingleRange { get; }
        public int VelocityIntervalInMillisecond { get; }
        public double FinallyDamage(int range)
        {
            if (range >= ForWeapon.Range) return 0;
            return ForWeapon.Damage - range * DamageDownCoefficentPerSingleRange < 0 ? 0 :
                ForWeapon.Damage - range * DamageDownCoefficentPerSingleRange;
        }
        public Physics(Weapon forWeapon, Vector velocity, double damageDownCoefficentPerSingleRange, int overcomeSingleRangeTime)
        {
            ForWeapon = forWeapon;
            Velocity = velocity;
            DamageDownCoefficentPerSingleRange = damageDownCoefficentPerSingleRange;
            VelocityIntervalInMillisecond = overcomeSingleRangeTime;
        }
    }
}
