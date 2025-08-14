using System;

namespace Dreamshade.Battle
{
    public sealed class QueuedAction
    {
        public BattleActionDef Def { get; }
        public int IncrementsRemaining { get; set; }

        public QueuedAction(BattleActionDef def, int increments)
        {
            Def = def ?? throw new ArgumentNullException(nameof(def));
            IncrementsRemaining = Math.Max(0, increments);
        }
    }
}
