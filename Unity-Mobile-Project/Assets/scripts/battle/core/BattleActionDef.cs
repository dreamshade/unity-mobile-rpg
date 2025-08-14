using System;

namespace Dreamshade.Battle
{
    public delegate void BattleActionExec(RecruitInBattle actor, BattleController battle);

    public sealed class BattleActionDef
    {
        public string Id { get; }
        public BattleActionExec Execute { get; }

        public BattleActionDef(string id, BattleActionExec execute)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }
    }
}
