// Assets/scripts/battle/BattleController.cs
using System.Collections.Generic;
using UnityEngine;

namespace Dreamshade.Battle
{
    /// MonoBehaviour that owns the loop and decrements "increments remaining".
    public sealed class BattleController : MonoBehaviour
    {
        [Header("Timing")]
        [Tooltip("Each increment equals this many seconds in real time.")]
        [Min(0.01f)] public float intervalSeconds = 0.1f;
        [Tooltip("0 = paused, 1 = normal, 2 = double-speed, etc.")]
        [Range(0f, 8f)] public float speed = 1f;
        public bool paused = false;

        public long Ticks { get; private set; } = 0;  // how many increments have elapsed

        private readonly List<RecruitInBattle> _participants = new();
        private double _accum;

        // --- Public API ---
        public void AddRecruit(RecruitInBattle recruit)
        {
            if (recruit != null && !_participants.Contains(recruit))
                _participants.Add(recruit);
        }

        public void ClearParticipants() => _participants.Clear();

        /// Step by seconds (used by realtime Update and by manual simulation).
        public void StepSeconds(float seconds)
        {
            if (seconds <= 0f) return;
            var step = Mathf.Max(0.0001f, intervalSeconds);
            _accum += seconds;

            while (_accum >= step)
            {
                _accum -= step;
                ProcessOneIncrement();
            }
        }

        /// Step by a fixed number of increments (headless/fast simulation).
        public void StepIncrements(int increments)
        {
            for (int i = 0; i < increments; i++)
                ProcessOneIncrement();
        }

        // --- Loop ---
        private void Update()
        {
            if (paused || speed <= 0f) return;
            StepSeconds(Time.deltaTime * speed);
        }

        private void ProcessOneIncrement()
        {
            Ticks++;

            // Decrement each recruit's current action by 1 increment, firing if ready.
            for (int i = 0; i < _participants.Count; i++)
            {
                var actor = _participants[i];
                var qa = actor.Current;
                if (qa == null) continue;

                qa.IncrementsRemaining -= 1;

                if (qa.IncrementsRemaining <= 0)
                {
                    // Clear before execute to avoid re-entrancy surprises.
                    var def = qa.Def;
                    actor.Current = null;
                    def.Execute(actor, this);
                }
            }
        }
    }
}
