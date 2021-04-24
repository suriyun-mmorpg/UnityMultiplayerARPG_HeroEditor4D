using UnityEngine;

namespace MultiplayerARPG.HeroEditor4D
{
    [System.Serializable]
    public class HeroEditorActionAnimation
    {
        public enum TriggerName
        {
            Slash1H,
            HeavySlash1H,
            HeavySlash2H,
            Slash2H,
            Stab,
            FastStab,
            ShotBow,
            ShotCrossbow,
            TripleStab,
            Throw,
            SecondaryShot,
        }

        public TriggerName triggerName;
        [Tooltip("Clip length for this action (highest(TripleStab) = 0.667)")]
        public float clipLength = 0.667f;
        [Tooltip("If this <= 0, it will not be used to calculates with animation speed multiplier")]
        public float animSpeedRate;
        [Tooltip("This will be in use with attack/skill animations, This is rate of total animation duration at when it should hit enemy or apply skill")]
        [Range(0f, 1f)]
        public float triggerDurationRate;
        [Tooltip("If this length more than 1, will use each entry as trigger duration rate")]
        [Range(0f, 1f)]
        public float[] multiHitTriggerDurationRates;
        [Tooltip("This will be in use with attack/skill animations, This is duration after played animation clip to add delay before next animation")]
        public float extraDuration;
        [Tooltip("This will be in use with attack/skill animations, These audio clips playing randomly while play this animation (not loop)")]
        public AudioClip[] audioClips;

        public string GetTriggerName()
        {
            return triggerName.ToString();
        }

        public AudioClip GetRandomAudioClip()
        {
            AudioClip clip = null;
            if (audioClips != null && audioClips.Length > 0)
                clip = audioClips[Random.Range(0, audioClips.Length)];
            return clip;
        }

        public float GetAnimSpeedRate()
        {
            return animSpeedRate > 0 ? animSpeedRate : 1f;
        }

        public float GetClipLength()
        {
            return clipLength;
        }

        public float GetExtraDuration()
        {
            return extraDuration;
        }

        public float[] GetTriggerDurations()
        {
            float clipLength = GetClipLength();
            if (multiHitTriggerDurationRates != null &&
                multiHitTriggerDurationRates.Length > 0)
            {
                float previousRate = 0f;
                float[] durations = new float[multiHitTriggerDurationRates.Length];
                for (int i = 0; i < durations.Length; ++i)
                {
                    durations[i] = clipLength * (multiHitTriggerDurationRates[i] - previousRate);
                    previousRate = multiHitTriggerDurationRates[i];
                }
                return durations;
            }
            return new float[] { clipLength * triggerDurationRate };
        }

        public float GetTotalDuration()
        {
            return GetClipLength() + extraDuration;
        }
    }
}