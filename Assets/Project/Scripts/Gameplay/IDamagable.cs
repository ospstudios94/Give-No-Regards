using UnityEngine;

public interface IDamagable 
{
    public int Health{ get; set;}

     void Damage(int damage);
}
