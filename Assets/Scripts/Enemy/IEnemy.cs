using UnityEngine;

namespace TDH.EnemyAI
{
    public interface IEnemy
    {
        void SetHitVelocity(Vector3 vel, float power);

        void ActivatedPlayerShield(bool activated);

        void PlayerDie();
    }
}