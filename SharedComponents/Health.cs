using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.SharedComponents
{
    public class Health : Component
    {
        public Health(int maxHp)
        {
            this.hp = maxHp;
        }
        public Action OnDeath;
        public Action OnHit;
        public virtual void Hit(int dmg)
        {
            hp -= dmg;
            if (dmg > 0) OnHit?.Invoke();
            if (hp <= 0) OnDeath?.Invoke();
        }
        public int hp { get; protected set; }
    }
}
