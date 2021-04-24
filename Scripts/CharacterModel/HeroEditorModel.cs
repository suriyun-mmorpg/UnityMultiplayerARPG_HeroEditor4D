using Assets.HeroEditor4D.Common.CharacterScripts;
using HeroEditor.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiplayerARPG.HeroEditor4D
{
    [RequireComponent(typeof(Character4D))]
    public class HeroEditorModel : BaseCharacterModel
    {
        // Clip name variables
        // Idle
        public const string CLIP_IDLE = "__Idle";
        public const string CLIP_MOVE = "__Move";
        public const string CLIP_SPRINT = "__Sprint";
        public const string CLIP_DEAD = "__Dead";
        public const string CLIP_ACTION = "__Action";
        public const string CLIP_CAST_SKILL = "__CastSkill";
        // Animator variables
        public static readonly int ANIM_IS_DEAD = Animator.StringToHash("IsDead");
        public static readonly int ANIM_MOVE_SPEED = Animator.StringToHash("MoveSpeed");
        public static readonly int ANIM_DO_ACTION = Animator.StringToHash("DoAction");
        public static readonly int ANIM_IS_CASTING_SKILL = Animator.StringToHash("IsCastingSkill");
        public static readonly int ANIM_MOVE_CLIP_MULTIPLIER = Animator.StringToHash("MoveSpeedMultiplier");
        public static readonly int ANIM_ACTION_CLIP_MULTIPLIER = Animator.StringToHash("ActionSpeedMultiplier");

        public const List<Sprite> EmptySprites = null;
        public const Sprite EmptySprite = null;
        public SpriteCollection spriteCollection;
        [Header("Animations")]
        public HeroEditorAnimations defaultAnimations;
        [ArrayElementTitle("weaponType")]
        public HeroEditorWeaponAnimation[] weaponAnimations;
        [ArrayElementTitle("skill")]
        public HeroEditorSkillAnimation[] skillAnimations;
        private Character4D character4D;
        private HashSet<EHeroEditorItemPart> equippedParts = new HashSet<EHeroEditorItemPart>();

        [Header("Relates Components")]
        [Tooltip("It will find `Animator` component on automatically if this is NULL")]
        public Animator animator;
        [Tooltip("You can set this when animator controller type is `Custom`")]
        public RuntimeAnimatorController animatorController;
        public AnimatorOverrideController CacheAnimatorController { get; private set; }

        // Private state validater
        private bool isSetupComponent;

        protected override void Awake()
        {
            base.Awake();
            character4D = GetComponent<Character4D>();
            SetupComponent();
        }

        private void SetupComponent()
        {
            if (isSetupComponent)
                return;
            isSetupComponent = true;
            if (CacheAnimatorController == null)
                CacheAnimatorController = new AnimatorOverrideController(animatorController);
            // Use override controller as animator
            if (animator != null && animator.runtimeAnimatorController != CacheAnimatorController)
                animator.runtimeAnimatorController = CacheAnimatorController;
            SetDefaultAnimations();
        }

        public override void SetDefaultAnimations()
        {
            // Set default clips
            if (CacheAnimatorController != null)
            {
                CacheAnimatorController[CLIP_IDLE] = defaultAnimations.idleClip;
                CacheAnimatorController[CLIP_MOVE] = defaultAnimations.moveClip;
                CacheAnimatorController[CLIP_SPRINT] = defaultAnimations.sprintClip;
                CacheAnimatorController[CLIP_DEAD] = defaultAnimations.deadClip;
            }
            base.SetDefaultAnimations();
        }

        public override void SetEquipWeapons(EquipWeapons equipWeapons)
        {
            base.SetEquipWeapons(equipWeapons);
            SetEquipmentSprites();
        }

        public override void SetEquipItems(IList<CharacterItem> equipItems)
        {
            base.SetEquipItems(equipItems);
            SetEquipmentSprites();
        }

        private void SetEquipmentSprites()
        {
            List<CharacterItem> items = new List<CharacterItem>(equipItems);
            items.Add(equipWeapons.leftHand);
            items.Add(equipWeapons.rightHand);
            // Clear equipped items
            foreach (EHeroEditorItemPart part in equippedParts)
            {
                switch (part)
                {
                    case EHeroEditorItemPart.Armor:
                        character4D.EquipArmor(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Helmet:
                        character4D.EquipHelmet(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Shield:
                        character4D.EquipShield(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Melee1H:
                        character4D.EquipMeleeWeapon1H(EmptySprite);
                        break;
                    case EHeroEditorItemPart.Melee2H:
                        character4D.EquipMeleeWeapon2H(EmptySprite);
                        break;
                    case EHeroEditorItemPart.Bow:
                        character4D.EquipBow(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Crossbow:
                        character4D.EquipCrossbow(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Firearm1H:
                        character4D.EquipSecondaryFirearm(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Supplies:
                        character4D.EquipSupply(EmptySprite);
                        break;
                    case EHeroEditorItemPart.Body:
                        character4D.SetBody(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Ears:
                        character4D.SetEars(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Eyebrows:
                        character4D.SetEyebrows(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Eyes:
                        character4D.SetEyes(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Hair:
                        character4D.SetHair(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Beard:
                        character4D.SetBeard(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Mouth:
                        character4D.SetMouth(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Makeup:
                        character4D.SetMakeup(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Mask:
                        character4D.SetMask(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Earrings:
                        character4D.SetEarrings(EmptySprites);
                        break;
                }
            }
            equippedParts.Clear();
            // Set equipping items
            Dictionary<string, SpriteGroupEntry> dict;
            IHeroEditorItem itemData;
            foreach (CharacterItem item in items)
            {
                if (item.IsEmptySlot() || !(item.GetItem() is IHeroEditorItem))
                    continue;
                itemData = item.GetItem() as IHeroEditorItem;
                if (equippedParts.Add(itemData.SpriteData.part))
                {
                    switch (itemData.SpriteData.part)
                    {
                        case EHeroEditorItemPart.Armor:
                            dict = spriteCollection.Armor.ToDictionary(i => i.FullName, i => i);
                            character4D.EquipArmor(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Helmet:
                            dict = spriteCollection.Armor.ToDictionary(i => i.FullName, i => i);
                            character4D.EquipHelmet(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Shield:
                            dict = spriteCollection.Shield.ToDictionary(i => i.FullName, i => i);
                            character4D.EquipShield(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Melee1H:
                            dict = spriteCollection.MeleeWeapon1H.ToDictionary(i => i.FullName, i => i);
                            character4D.EquipMeleeWeapon1H(dict[itemData.SpriteData.id]?.Sprite);
                            break;
                        case EHeroEditorItemPart.Melee2H:
                            dict = spriteCollection.MeleeWeapon2H.ToDictionary(i => i.FullName, i => i);
                            character4D.EquipMeleeWeapon2H(dict[itemData.SpriteData.id]?.Sprite);
                            break;
                        case EHeroEditorItemPart.Bow:
                            dict = spriteCollection.Bow.ToDictionary(i => i.FullName, i => i);
                            character4D.EquipBow(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Crossbow:
                            dict = spriteCollection.Crossbow.ToDictionary(i => i.FullName, i => i);
                            character4D.EquipCrossbow(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Firearm1H:
                            dict = spriteCollection.Firearm1H.ToDictionary(i => i.FullName, i => i);
                            character4D.EquipSecondaryFirearm(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Supplies:
                            dict = spriteCollection.Supplies.ToDictionary(i => i.FullName, i => i);
                            character4D.EquipSupply(dict[itemData.SpriteData.id]?.Sprite);
                            break;
                        case EHeroEditorItemPart.Body:
                            dict = spriteCollection.Body.ToDictionary(i => i.FullName, i => i);
                            character4D.SetBody(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Ears:
                            dict = spriteCollection.Ears.ToDictionary(i => i.FullName, i => i);
                            character4D.SetEars(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Eyebrows:
                            dict = spriteCollection.Eyebrows.ToDictionary(i => i.FullName, i => i);
                            character4D.SetEyebrows(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Eyes:
                            dict = spriteCollection.Eyes.ToDictionary(i => i.FullName, i => i);
                            character4D.SetEyes(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Hair:
                            dict = spriteCollection.Hair.ToDictionary(i => i.FullName, i => i);
                            character4D.SetHair(dict[itemData.SpriteData.id]);
                            break;
                        case EHeroEditorItemPart.Beard:
                            dict = spriteCollection.Beard.ToDictionary(i => i.FullName, i => i);
                            character4D.SetBeard(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Mouth:
                            dict = spriteCollection.Mouth.ToDictionary(i => i.FullName, i => i);
                            character4D.SetMouth(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Makeup:
                            dict = spriteCollection.Makeup.ToDictionary(i => i.FullName, i => i);
                            character4D.SetMakeup(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Mask:
                            dict = spriteCollection.Mask.ToDictionary(i => i.FullName, i => i);
                            character4D.SetMask(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Earrings:
                            dict = spriteCollection.Earrings.ToDictionary(i => i.FullName, i => i);
                            character4D.SetEarrings(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                    }
                }
            }
        }

        public bool TryGetWeaponAnimations(int dataId, out HeroEditorWeaponAnimation anims)
        {
            return CacheAnimationsManager.SetAndTryGetCacheWeaponAnimations(CacheIdentity.HashAssetId, weaponAnimations, skillAnimations, dataId, out anims);
        }

        public bool TryGetSkillAnimations(int dataId, out HeroEditorSkillAnimation anims)
        {
            return CacheAnimationsManager.SetAndTryGetCacheSkillAnimations(CacheIdentity.HashAssetId, weaponAnimations, skillAnimations, dataId, out anims);
        }

        private ActionAnimation GetActionAnimation(AnimActionType animActionType, int dataId)
        {
            ActionAnimation animation = defaultAnimations.attackAnimation;
            HeroEditorWeaponAnimation weaponAnimations;
            HeroEditorSkillAnimation skillAnimations;
            switch (animActionType)
            {
                case AnimActionType.AttackRightHand:
                    if (!TryGetWeaponAnimations(dataId, out weaponAnimations))
                        animation = defaultAnimations.attackAnimation;
                    else
                        animation = weaponAnimations.rightHandAttackAnimation;
                    break;
                case AnimActionType.AttackLeftHand:
                    if (!TryGetWeaponAnimations(dataId, out weaponAnimations))
                        animation = defaultAnimations.attackAnimation;
                    else
                        animation = weaponAnimations.leftHandAttackAnimation;
                    break;
                case AnimActionType.SkillRightHand:
                case AnimActionType.SkillLeftHand:
                    if (!TryGetSkillAnimations(dataId, out skillAnimations))
                        animation = defaultAnimations.skillActivateAnimation;
                    else
                        animation = skillAnimations.activateAnimation;
                    break;
                case AnimActionType.ReloadRightHand:
                    if (!TryGetWeaponAnimations(dataId, out weaponAnimations))
                        animation = defaultAnimations.reloadAnimation;
                    else
                        animation = weaponAnimations.rightHandReloadAnimation;
                    break;
                case AnimActionType.ReloadLeftHand:
                    if (!TryGetWeaponAnimations(dataId, out weaponAnimations))
                        animation = defaultAnimations.reloadAnimation;
                    else
                        animation = weaponAnimations.leftHandReloadAnimation;
                    break;
            }
            return animation;
        }

        public override bool GetLeftHandAttackAnimation(int dataId, int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            ActionAnimation animation = defaultAnimations.attackAnimation;
            HeroEditorWeaponAnimation weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation = weaponAnims.leftHandAttackAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            AnimationClip clip = animation.clip;
            if (clip == null) return false;
            triggerDurations = animation.GetTriggerDurations();
            totalDuration = animation.GetTotalDuration();
            return true;
        }

        public override bool GetRightHandAttackAnimation(int dataId, int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            ActionAnimation animation = defaultAnimations.attackAnimation;
            HeroEditorWeaponAnimation weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation = weaponAnims.rightHandAttackAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            AnimationClip clip = animation.clip;
            if (clip == null) return false;
            triggerDurations = animation.GetTriggerDurations();
            totalDuration = animation.GetTotalDuration();
            return true;
        }

        public override bool GetLeftHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            ActionAnimation animation = defaultAnimations.reloadAnimation;
            HeroEditorWeaponAnimation weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation = weaponAnims.leftHandReloadAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            AnimationClip clip = animation.clip;
            if (clip == null) return false;
            triggerDurations = animation.GetTriggerDurations();
            totalDuration = animation.GetTotalDuration();
            return true;
        }

        public override bool GetRightHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            ActionAnimation animation = defaultAnimations.reloadAnimation;
            HeroEditorWeaponAnimation weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation = weaponAnims.rightHandReloadAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            AnimationClip clip = animation.clip;
            if (clip == null) return false;
            triggerDurations = animation.GetTriggerDurations();
            totalDuration = animation.GetTotalDuration();
            return true;
        }

        public override bool GetRandomLeftHandAttackAnimation(int dataId, out int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            animationIndex = 0;
            return GetLeftHandAttackAnimation(dataId, animationIndex, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public override bool GetRandomRightHandAttackAnimation(int dataId, out int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            animationIndex = 0;
            return GetRightHandAttackAnimation(dataId, animationIndex, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public override bool GetSkillActivateAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            ActionAnimation animation2D = defaultAnimations.skillActivateAnimation;
            HeroEditorSkillAnimation skillAnims;
            if (TryGetSkillAnimations(dataId, out skillAnims))
                animation2D = skillAnims.activateAnimation;
            animSpeedRate = 1f;
            triggerDurations = new float[] { 0f };
            totalDuration = 0f;
            AnimationClip clip = animation2D.clip;
            if (clip == null) return false;
            triggerDurations = animation2D.GetTriggerDurations();
            totalDuration = animation2D.GetTotalDuration();
            return true;
        }

        public override Coroutine PlayActionAnimation(AnimActionType animActionType, int dataId, int index, float playSpeedMultiplier = 1f)
        {
            return StartCoroutine(PlayActionAnimation_Animator(animActionType, dataId, index, playSpeedMultiplier));
        }

        private IEnumerator PlayActionAnimation_Animator(AnimActionType animActionType, int dataId, int index, float playSpeedMultiplier)
        {
            // If animator is not null, play the action animation
            ActionAnimation animation = GetActionAnimation(animActionType, dataId);
            // Action
            CacheAnimatorController[CLIP_ACTION] = animation.clip;
            yield return 0;
            AudioManager.PlaySfxClipAtAudioSource(animation.GetRandomAudioClip(), genericAudioSource);
            animator.SetFloat(ANIM_ACTION_CLIP_MULTIPLIER, playSpeedMultiplier);
            animator.SetBool(ANIM_DO_ACTION, true);
            animator.Play(0, 0, 0f);
            // Waits by current transition + clip duration before end animation
            yield return new WaitForSecondsRealtime(animation.GetClipLength() / playSpeedMultiplier);
            animator.SetBool(ANIM_DO_ACTION, false);
            // Waits by current transition + extra duration before end playing animation state
            yield return new WaitForSecondsRealtime(animation.GetExtraDuration() / playSpeedMultiplier);
        }

        public override Coroutine PlaySkillCastClip(int dataId, float duration)
        {
            return StartCoroutine(PlaySkillCastClip_Animator(dataId, duration));
        }

        private IEnumerator PlaySkillCastClip_Animator(int dataId, float duration)
        {
            AnimationClip clip;
            HeroEditorSkillAnimation skillAnimations;
            if (!TryGetSkillAnimations(dataId, out skillAnimations))
                clip = defaultAnimations.skillCastClip;
            else
                clip = skillAnimations.castClip;

            if (clip != null)
            {
                // Cast Skill
                CacheAnimatorController[CLIP_CAST_SKILL] = clip;
                yield return 0;
                animator.SetBool(ANIM_IS_CASTING_SKILL, true);
                animator.Play(0, 0, 0f);
                yield return new WaitForSecondsRealtime(duration);
                animator.SetBool(ANIM_IS_CASTING_SKILL, false);
            }
        }

        public override void PlayWeaponChargeClip(int dataId, bool isLeftHand)
        {
            // TODO: May implement pulling animation for 2D models
        }

        public override void StopActionAnimation()
        {
            animator.SetBool(ANIM_DO_ACTION, false);
        }

        public override void StopSkillCastAnimation()
        {
            animator.SetBool(ANIM_IS_CASTING_SKILL, false);
        }

        public override void StopWeaponChargeAnimation()
        {
            // TODO: May implement pulling animation for 2D models
        }

        public override void PlayMoveAnimation()
        {
            throw new System.NotImplementedException();
        }

        public override SkillActivateAnimationType UseSkillActivateAnimationType(int dataId)
        {
            HeroEditorSkillAnimation anims;
            if (!TryGetSkillAnimations(dataId, out anims))
                return SkillActivateAnimationType.UseActivateAnimation;
            return anims.activateAnimationType;
        }
    }
}
