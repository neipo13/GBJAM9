using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.SharedComponents
{
    public class Health
    {
        public Health(int maxHp)
        {
            this.hp = maxHp;
        }
        public Action OnDeath;
        public virtual void Hit(int dmg)
        {
            hp -= dmg;
            if (hp <= 0) OnDeath();
        }
        public int hp { get; protected set; }
    }
}
