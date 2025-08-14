// Assets/scripts/battle/core/RecruitInBattle.cs
using System;

namespace Dreamshade.Battle
{
    /// Pure C# model for a recruit during battle (no MonoBehaviour).
    public sealed class RecruitInBattle
    {
        public string Name { get; }
        public int DefaultAutoCost { get; private set; }  // increments to fire auto attack
        public QueuedAction Current { get; set; }         // may be null

        public RecruitInBattle(string name, int defaultAutoCost)
        {
            Name = name ?? "Recruit";
            DefaultAutoCost = Math.Max(1, defaultAutoCost);
            QueueAutoAttack();
        }

        public void QueueAutoAttack()
        {
            Current = new QueuedAction(BattleActions.AutoAttack, DefaultAutoCost);
        }

        public override string ToString() => Name;
    }
}
