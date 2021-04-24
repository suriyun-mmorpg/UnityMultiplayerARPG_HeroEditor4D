using Assets.HeroEditor4D.Common.CharacterScripts;
using HeroEditor.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MultiplayerARPG.HeroEditor4D
{
    [RequireComponent(typeof(Character4D))]
    public class HeroEditorModel : BaseCharacterModel
    {
        // Animator variables
        public static readonly int ANIM_STATE = Animator.StringToHash("State");
        public static readonly int ANIM_ACTION = Animator.StringToHash("Action");
        public static readonly int ANIM_TWO_HANDED = Animator.StringToHash("TwoHanded");
        public static readonly int ANIM_SPEED_MULTIPLIER = Animator.StringToHash("Speed");
        public static readonly int ANIM_WEAPON_TYPE = Animator.StringToHash("WeaponType");

        public enum StateTypes
        {
            Idle = 0,
            Ready = 1,
            Walk = 2,
            Run = 3,
            Jump = 4,
            Climb = 5,
            Death = 9,
            Nothing = 13,
        }

        public const List<Sprite> EmptySprites = null;
        public const Sprite EmptySprite = null;
        public SpriteCollection spriteCollection;
        [Header("Animations")]
        public HeroEditorAnimations defaultAnimations;
        [ArrayElementTitle("weaponType")]
        public HeroEditorWeaponAnimation[] weaponAnimations;
        [ArrayElementTitle("skill")]
        public HeroEditorSkillAnimation[] skillAnimations;
        public Character4D Character4D { get; private set; }
        private HashSet<EHeroEditorItemPart> equippedParts = new HashSet<EHeroEditorItemPart>();

        [Header("Relates Components")]
        [Tooltip("It will find `Animator` component on automatically if this is NULL")]
        public Animator animator;

        // Private state validater
        private bool isSetupComponent;
        private bool isCasting;

        protected override void Awake()
        {
            base.Awake();
            Character4D = GetComponent<Character4D>();
            Character4D.SetDirection(Vector2.down);
            SetupComponent();
        }

        private void SetupComponent()
        {
            if (isSetupComponent)
                return;
            isSetupComponent = true;
            SetDefaultAnimations();
        }

        public override void SetDefaultAnimations()
        {
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
            List<CharacterItem> items = new List<CharacterItem>();
            if (equipItems != null)
            {
                items.AddRange(equipItems);
            }
            if (equipWeapons != null)
            {
                items.Add(equipWeapons.leftHand);
                items.Add(equipWeapons.rightHand);
            }
            // Clear equipped items
            foreach (EHeroEditorItemPart part in equippedParts)
            {
                switch (part)
                {
                    case EHeroEditorItemPart.Armor:
                        Character4D.EquipArmor(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Helmet:
                        Character4D.EquipHelmet(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Shield:
                        Character4D.EquipShield(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Melee1H:
                        Character4D.EquipMeleeWeapon1H(EmptySprite);
                        break;
                    case EHeroEditorItemPart.Melee2H:
                        Character4D.EquipMeleeWeapon2H(EmptySprite);
                        break;
                    case EHeroEditorItemPart.Bow:
                        Character4D.EquipBow(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Crossbow:
                        Character4D.EquipCrossbow(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Firearm1H:
                        Character4D.EquipSecondaryFirearm(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Supplies:
                        Character4D.EquipSupply(EmptySprite);
                        break;
                    case EHeroEditorItemPart.Body:
                        Character4D.SetBody(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Ears:
                        Character4D.SetEars(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Eyebrows:
                        Character4D.SetEyebrows(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Eyes:
                        Character4D.SetEyes(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Hair:
                        Character4D.SetHair(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Beard:
                        Character4D.SetBeard(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Mouth:
                        Character4D.SetMouth(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Makeup:
                        Character4D.SetMakeup(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Mask:
                        Character4D.SetMask(EmptySprites);
                        break;
                    case EHeroEditorItemPart.Earrings:
                        Character4D.SetEarrings(EmptySprites);
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
                            Character4D.EquipArmor(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Helmet:
                            dict = spriteCollection.Armor.ToDictionary(i => i.FullName, i => i);
                            Character4D.EquipHelmet(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Shield:
                            dict = spriteCollection.Shield.ToDictionary(i => i.FullName, i => i);
                            Character4D.EquipShield(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Melee1H:
                            dict = spriteCollection.MeleeWeapon1H.ToDictionary(i => i.FullName, i => i);
                            Character4D.EquipMeleeWeapon1H(dict[itemData.SpriteData.id]?.Sprite);
                            break;
                        case EHeroEditorItemPart.Melee2H:
                            dict = spriteCollection.MeleeWeapon2H.ToDictionary(i => i.FullName, i => i);
                            Character4D.EquipMeleeWeapon2H(dict[itemData.SpriteData.id]?.Sprite);
                            break;
                        case EHeroEditorItemPart.Bow:
                            dict = spriteCollection.Bow.ToDictionary(i => i.FullName, i => i);
                            Character4D.EquipBow(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Crossbow:
                            dict = spriteCollection.Crossbow.ToDictionary(i => i.FullName, i => i);
                            Character4D.EquipCrossbow(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Firearm1H:
                            dict = spriteCollection.Firearm1H.ToDictionary(i => i.FullName, i => i);
                            Character4D.EquipSecondaryFirearm(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Supplies:
                            dict = spriteCollection.Supplies.ToDictionary(i => i.FullName, i => i);
                            Character4D.EquipSupply(dict[itemData.SpriteData.id]?.Sprite);
                            break;
                        case EHeroEditorItemPart.Body:
                            dict = spriteCollection.Body.ToDictionary(i => i.FullName, i => i);
                            Character4D.SetBody(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Ears:
                            dict = spriteCollection.Ears.ToDictionary(i => i.FullName, i => i);
                            Character4D.SetEars(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Eyebrows:
                            dict = spriteCollection.Eyebrows.ToDictionary(i => i.FullName, i => i);
                            Character4D.SetEyebrows(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Eyes:
                            dict = spriteCollection.Eyes.ToDictionary(i => i.FullName, i => i);
                            Character4D.SetEyes(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Hair:
                            dict = spriteCollection.Hair.ToDictionary(i => i.FullName, i => i);
                            Character4D.SetHair(dict[itemData.SpriteData.id]);
                            break;
                        case EHeroEditorItemPart.Beard:
                            dict = spriteCollection.Beard.ToDictionary(i => i.FullName, i => i);
                            Character4D.SetBeard(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Mouth:
                            dict = spriteCollection.Mouth.ToDictionary(i => i.FullName, i => i);
                            Character4D.SetMouth(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Makeup:
                            dict = spriteCollection.Makeup.ToDictionary(i => i.FullName, i => i);
                            Character4D.SetMakeup(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Mask:
                            dict = spriteCollection.Mask.ToDictionary(i => i.FullName, i => i);
                            Character4D.SetMask(dict[itemData.SpriteData.id]?.Sprites);
                            break;
                        case EHeroEditorItemPart.Earrings:
                            dict = spriteCollection.Earrings.ToDictionary(i => i.FullName, i => i);
                            Character4D.SetEarrings(dict[itemData.SpriteData.id]?.Sprites);
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

        private HeroEditorActionAnimation GetHeroEditorActionAnimation(AnimActionType animActionType, int dataId)
        {
            HeroEditorActionAnimation animation = defaultAnimations.attackAnimation;
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
            HeroEditorActionAnimation animation = defaultAnimations.attackAnimation;
            HeroEditorWeaponAnimation weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation = weaponAnims.leftHandAttackAnimation;
            animSpeedRate = animation.GetAnimSpeedRate();
            triggerDurations = animation.GetTriggerDurations();
            totalDuration = animation.GetTotalDuration();
            return true;
        }

        public override bool GetRightHandAttackAnimation(int dataId, int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            HeroEditorActionAnimation animation = defaultAnimations.attackAnimation;
            HeroEditorWeaponAnimation weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation = weaponAnims.rightHandAttackAnimation;
            animSpeedRate = animation.GetAnimSpeedRate();
            triggerDurations = animation.GetTriggerDurations();
            totalDuration = animation.GetTotalDuration();
            return true;
        }

        public override bool GetLeftHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            HeroEditorActionAnimation animation = defaultAnimations.reloadAnimation;
            HeroEditorWeaponAnimation weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation = weaponAnims.leftHandReloadAnimation;
            animSpeedRate = animation.GetAnimSpeedRate();
            triggerDurations = animation.GetTriggerDurations();
            totalDuration = animation.GetTotalDuration();
            return true;
        }

        public override bool GetRightHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            HeroEditorActionAnimation animation = defaultAnimations.reloadAnimation;
            HeroEditorWeaponAnimation weaponAnims;
            if (TryGetWeaponAnimations(dataId, out weaponAnims))
                animation = weaponAnims.rightHandReloadAnimation;
            animSpeedRate = animation.GetAnimSpeedRate();
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
            HeroEditorActionAnimation animation = defaultAnimations.skillActivateAnimation;
            HeroEditorSkillAnimation skillAnims;
            if (TryGetSkillAnimations(dataId, out skillAnims))
                animation = skillAnims.activateAnimation;
            animSpeedRate = animation.GetAnimSpeedRate();
            triggerDurations = animation.GetTriggerDurations();
            totalDuration = animation.GetTotalDuration();
            return true;
        }

        public override Coroutine PlayActionAnimation(AnimActionType animActionType, int dataId, int index, float playSpeedMultiplier = 1f)
        {
            return StartCoroutine(PlayHeroEditorActionAnimation_Animator(animActionType, dataId, index, playSpeedMultiplier));
        }

        private IEnumerator PlayHeroEditorActionAnimation_Animator(AnimActionType animActionType, int dataId, int index, float playSpeedMultiplier)
        {
            // If animator is not null, play the action animation
            HeroEditorActionAnimation animation = GetHeroEditorActionAnimation(animActionType, dataId);
            // Action
            animator.SetTrigger(animation.GetTriggerName());
            AudioManager.PlaySfxClipAtAudioSource(animation.GetRandomAudioClip(), genericAudioSource);
            // Waits by current transition + clip duration before end animation
            yield return new WaitForSecondsRealtime(animation.GetClipLength() / playSpeedMultiplier);
            // Waits by current transition + extra duration before end playing animation state
            yield return new WaitForSecondsRealtime(animation.GetExtraDuration() / playSpeedMultiplier);
        }

        public override Coroutine PlaySkillCastClip(int dataId, float duration)
        {
            return StartCoroutine(PlaySkillCastClip_Animator(dataId, duration));
        }

        private IEnumerator PlaySkillCastClip_Animator(int dataId, float duration)
        {
            // Cast Skill
            isCasting = true;
            yield return new WaitForSecondsRealtime(duration);
            isCasting = false;
        }

        public override void PlayWeaponChargeClip(int dataId, bool isLeftHand)
        {
            // TODO: May implement pulling animation for 2D models
        }

        public override void StopActionAnimation()
        {
            animator.SetBool(ANIM_ACTION, false);
        }

        public override void StopSkillCastAnimation()
        {
            animator.SetBool(ANIM_ACTION, false);
            animator.SetInteger(ANIM_STATE, (int)StateTypes.Idle);
        }

        public override void StopWeaponChargeAnimation()
        {
            // TODO: May implement pulling animation for 2D models
        }

        public override void PlayMoveAnimation()
        {
            if (!animator.gameObject.activeInHierarchy)
                return;

            if (isDead)
            {
                animator.SetInteger(ANIM_STATE, (int)StateTypes.Death);
                return;
            }

            if (isCasting)
            {
                animator.SetInteger(ANIM_STATE, (int)StateTypes.Ready);
                return;
            }

            if (movementState.HasFlag(MovementState.Forward) ||
                movementState.HasFlag(MovementState.Backward) ||
                movementState.HasFlag(MovementState.Right) ||
                movementState.HasFlag(MovementState.Left))
            {
                if (extraMovementState.HasFlag(ExtraMovementState.IsSprinting))
                    animator.SetInteger(ANIM_STATE, (int)StateTypes.Run);
                else
                    animator.SetInteger(ANIM_STATE, (int)StateTypes.Walk);
            }
            else
            {
                animator.SetInteger(ANIM_STATE, (int)StateTypes.Idle);
            }

            // Update direction
            if (Mathf.Abs(direction2D.x) > Mathf.Abs(direction2D.y))
            {
                if (direction2D.x > 0)
                {
                    Character4D.SetDirection(Vector2.right);
                }
                else
                {
                    Character4D.SetDirection(Vector2.left);
                }
            }
            else
            {
                if (direction2D.y > 0)
                {
                    Character4D.SetDirection(Vector2.up);
                }
                else
                {
                    Character4D.SetDirection(Vector2.down);
                }
            }
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
