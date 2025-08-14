// Assets/scripts/battle/core/BattleActions.cs
using UnityEngine;

namespace Dreamshade.Battle
{
    /// Where we define functions for actions. Start simple: AutoAttack logs.
    public static class BattleActions
    {
        public static readonly BattleActionDef AutoAttack = new(
            id: "auto_attack",
            execute: (actor, battle) =>
            {
                Debug.Log($"[AUTO] {actor.Name} attacks at tick {battle.Ticks}");
                // After firing, default back to auto-attack
                actor.QueueAutoAttack();
            });
    }
}
