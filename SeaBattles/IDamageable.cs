using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles
{
    /// <summary>
    /// Интерфейс класса, реализующий DamageAspect
    /// </summary>
    /// <remarks>С одной стороны хорошо, что есть данный интерфейс, так как DamageManager может не проверять типы объектов.
    /// С другой стороны, не только DamageManager может вызывать методы интерфейса, но и любой другой класс.</remarks>
    public interface IDamageable
    {
        bool ApplyDamage(int amount);
    }
}
