using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.SharedComponents
{
    public class Health : Component
    {
        public int hp { get; protected set; }
        public int maxHp { get; protected set; }

        public Health(int maxHp)
        {
            this.maxHp = maxHp;
            this.hp = maxHp;
        }

        public void Reset()
        {
            this.hp = maxHp;
        }
        public Action OnDeath;
        public Func<bool> OnHit;
        public virtual void Hit(int dmg)
        {
            var valid = true;
            if (dmg > 0)
            {
                if(OnHit != null)
                {
                    valid = OnHit.Invoke();
                }
            }
            if (!valid) return;
            hp -= dmg;
            if (hp > maxHp) hp = maxHp;
            if (hp <= 0) OnDeath?.Invoke();
        }
    }
}
