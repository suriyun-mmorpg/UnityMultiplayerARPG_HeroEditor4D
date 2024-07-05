using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using Assets.HeroEditor4D.Common.Scripts.Collections;
using Enums = Assets.HeroEditor4D.Common.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public static readonly int ANIM_SPEED = Animator.StringToHash("Speed");
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
        public HeroEditorSpriteData[] defaultSprites;
        [Header("Animations")]
        public HeroEditorAnimations defaultAnimations;
        [ArrayElementTitle("weaponType")]
        public HeroEditorWeaponAnimation[] weaponAnimations;
        [ArrayElementTitle("skill")]
        public HeroEditorSkillAnimation[] skillAnimations;
        private Character4D character4D;
        public Character4D Character4D
        {
            get
            {
                if (character4D == null)
                    character4D = GetComponent<Character4D>();
                return character4D;
            }
        }

        public Animator Animator { get { return Character4D.Animator; } }

        // Private state validater
        private bool isSetupComponent;
        private bool isCasting;

        protected override void Awake()
        {
            base.Awake();
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

        public override void SetEquipItems(IList<CharacterItem> equipItems, IList<EquipWeapons> selectableWeaponSets, byte equipWeaponSet, bool isWeaponsSheathed)
        {
            base.SetEquipItems(equipItems, selectableWeaponSets, equipWeaponSet, isWeaponsSheathed);
            SetEquipmentSprites();
        }

        private void SetEquipmentSprites()
        {
            List<CharacterItem> items = new List<CharacterItem>();
            if (EquipItems != null)
            {
                items.AddRange(EquipItems);
            }
            EquipWeapons equipWeapons;
            if (SelectableWeaponSets != null && EquipWeaponSet >= 0 && EquipWeaponSet < SelectableWeaponSets.Count)
                equipWeapons = SelectableWeaponSets[EquipWeaponSet];
            else
                equipWeapons = new EquipWeapons();
            items.Add(equipWeapons.leftHand);
            items.Add(equipWeapons.rightHand);
            // Clear equipped items
            for (int i = 0; i < (int)EHeroEditorItemPart.Count; ++i)
            {
                switch ((EHeroEditorItemPart)i)
                {
                    case EHeroEditorItemPart.Armor:
                        Character4D.UnEquip(Enums.EquipmentPart.Armor);
                        break;
                    case EHeroEditorItemPart.Helmet:
                        Character4D.UnEquip(Enums.EquipmentPart.Helmet);
                        break;
                    case EHeroEditorItemPart.Shield:
                        Character4D.UnEquip(Enums.EquipmentPart.Shield);
                        break;
                    case EHeroEditorItemPart.Melee1H:
                        Character4D.UnEquip(Enums.EquipmentPart.MeleeWeapon1H);
                        break;
                    case EHeroEditorItemPart.Melee2H:
                        Character4D.UnEquip(Enums.EquipmentPart.MeleeWeapon2H);
                        break;
                    case EHeroEditorItemPart.Bow:
                        Character4D.UnEquip(Enums.EquipmentPart.Bow);
                        break;
                    case EHeroEditorItemPart.Crossbow:
                        Character4D.UnEquip(Enums.EquipmentPart.Crossbow);
                        break;
                    case EHeroEditorItemPart.Firearm1H:
                        Character4D.UnEquip(Enums.EquipmentPart.SecondaryFirearm1H);
                        break;
                    case EHeroEditorItemPart.Supplies:
                        break;
                    case EHeroEditorItemPart.Body:
                        Character4D.SetBody(null, Enums.BodyPart.Body);
                        break;
                    case EHeroEditorItemPart.Ears:
                        Character4D.SetBody(null, Enums.BodyPart.Ears);
                        break;
                    case EHeroEditorItemPart.Eyebrows:
                        Character4D.SetBody(null, Enums.BodyPart.Eyebrows);
                        break;
                    case EHeroEditorItemPart.Eyes:
                        Character4D.SetBody(null, Enums.BodyPart.Eyes);
                        break;
                    case EHeroEditorItemPart.Hair:
                        Character4D.SetBody(null, Enums.BodyPart.Hair);
                        break;
                    case EHeroEditorItemPart.Beard:
                        Character4D.SetBody(null, Enums.BodyPart.Beard);
                        break;
                    case EHeroEditorItemPart.Mouth:
                        Character4D.SetBody(null, Enums.BodyPart.Mouth);
                        break;
                    case EHeroEditorItemPart.Makeup:
                        Character4D.SetBody(null, Enums.BodyPart.Makeup);
                        break;
                    case EHeroEditorItemPart.Mask:
                        Character4D.UnEquip(Enums.EquipmentPart.Mask);
                        break;
                    case EHeroEditorItemPart.Earrings:
                        Character4D.UnEquip(Enums.EquipmentPart.Earrings);
                        break;
                    case EHeroEditorItemPart.Vest:
                        Character4D.UnEquip(Enums.EquipmentPart.Vest);
                        break;
                    case EHeroEditorItemPart.Bracers:
                        Character4D.UnEquip(Enums.EquipmentPart.Bracers);
                        break;
                    case EHeroEditorItemPart.Leggings:
                        Character4D.UnEquip(Enums.EquipmentPart.Leggings);
                        break;
                    case EHeroEditorItemPart.Cape:
                        break;
                    case EHeroEditorItemPart.Back:
                        break;
                }
            }

            // Set default part
            Dictionary<EHeroEditorItemPart, HeroEditorSpriteData> sprites = new Dictionary<EHeroEditorItemPart, HeroEditorSpriteData>();
            foreach (HeroEditorSpriteData defaultSprite in defaultSprites)
            {
                sprites[defaultSprite.part] = defaultSprite;
            }

            // Set equipping items
            IHeroEditorItem itemData;
            foreach (CharacterItem item in items)
            {
                if (item.IsEmptySlot() || !(item.GetItem() is IHeroEditorItem))
                    continue;
                itemData = item.GetItem() as IHeroEditorItem;
                sprites[itemData.SpriteData.part] = itemData.SpriteData;
            }

            foreach (HeroEditorSpriteData sprite in sprites.Values)
            {
                switch (sprite.part)
                {
                    case EHeroEditorItemPart.Armor:
                        Character4D.Equip(spriteCollection.Armor.Single(i => i.Name == sprite.id), Enums.EquipmentPart.Armor, sprite.color);
                        break;
                    case EHeroEditorItemPart.Helmet:
                        Character4D.Equip(spriteCollection.Armor.Single(i => i.Name == sprite.id), Enums.EquipmentPart.Helmet, sprite.color);
                        break;
                    case EHeroEditorItemPart.Shield:
                        Character4D.Equip(spriteCollection.Shield.Single(i => i.Name == sprite.id), Enums.EquipmentPart.Shield, sprite.color);
                        break;
                    case EHeroEditorItemPart.Melee1H:
                        Character4D.Equip(spriteCollection.MeleeWeapon1H.Single(i => i.Name == sprite.id), Enums.EquipmentPart.MeleeWeapon1H, sprite.color);
                        break;
                    case EHeroEditorItemPart.Melee2H:
                        Character4D.Equip(spriteCollection.MeleeWeapon2H.Single(i => i.Name == sprite.id), Enums.EquipmentPart.MeleeWeapon2H, sprite.color);
                        break;
                    case EHeroEditorItemPart.Bow:
                        Character4D.Equip(spriteCollection.Bow.Single(i => i.Name == sprite.id), Enums.EquipmentPart.Bow, sprite.color);
                        break;
                    case EHeroEditorItemPart.Crossbow:
                        Character4D.Equip(spriteCollection.Crossbow.Single(i => i.Name == sprite.id), Enums.EquipmentPart.Crossbow, sprite.color);
                        break;
                    case EHeroEditorItemPart.Firearm1H:
                        Character4D.Equip(spriteCollection.Firearm1H.Single(i => i.Name == sprite.id), Enums.EquipmentPart.SecondaryFirearm1H, sprite.color);
                        break;
                    case EHeroEditorItemPart.Supplies:
                        break;
                    case EHeroEditorItemPart.Body:
                        Character4D.SetBody(spriteCollection.Body.Single(i => i.Name == sprite.id), Enums.BodyPart.Body, sprite.color);
                        break;
                    case EHeroEditorItemPart.Ears:
                        Character4D.SetBody(spriteCollection.Ears.Single(i => i.Name == sprite.id), Enums.BodyPart.Ears, sprite.color);
                        break;
                    case EHeroEditorItemPart.Eyebrows:
                        Character4D.SetBody(spriteCollection.Eyebrows.Single(i => i.Name == sprite.id), Enums.BodyPart.Eyebrows, sprite.color);
                        break;
                    case EHeroEditorItemPart.Eyes:
                        Character4D.SetBody(spriteCollection.Eyes.Single(i => i.Name == sprite.id), Enums.BodyPart.Eyes, sprite.color);
                        break;
                    case EHeroEditorItemPart.Hair:
                        Character4D.SetBody(spriteCollection.Hair.Single(i => i.Name == sprite.id), Enums.BodyPart.Hair, sprite.color);
                        break;
                    case EHeroEditorItemPart.Beard:
                        Character4D.SetBody(spriteCollection.Beard.Single(i => i.Name == sprite.id), Enums.BodyPart.Beard, sprite.color);
                        break;
                    case EHeroEditorItemPart.Mouth:
                        Character4D.SetBody(spriteCollection.Mouth.Single(i => i.Name == sprite.id), Enums.BodyPart.Mouth, sprite.color);
                        break;
                    case EHeroEditorItemPart.Makeup:
                        Character4D.SetBody(spriteCollection.Makeup.Single(i => i.Name == sprite.id), Enums.BodyPart.Makeup, sprite.color);
                        break;
                    case EHeroEditorItemPart.Mask:
                        Character4D.Equip(spriteCollection.Mask.Single(i => i.Name == sprite.id), Enums.EquipmentPart.Mask, sprite.color);
                        break;
                    case EHeroEditorItemPart.Earrings:
                        Character4D.Equip(spriteCollection.Earrings.Single(i => i.Name == sprite.id), Enums.EquipmentPart.Earrings, sprite.color);
                        break;
                    case EHeroEditorItemPart.Vest:
                        Character4D.Equip(spriteCollection.Armor.Single(i => i.Name == sprite.id), Enums.EquipmentPart.Vest, sprite.color);
                        break;
                    case EHeroEditorItemPart.Bracers:
                        Character4D.Equip(spriteCollection.Armor.Single(i => i.Name == sprite.id), Enums.EquipmentPart.Bracers, sprite.color);
                        break;
                    case EHeroEditorItemPart.Leggings:
                        Character4D.Equip(spriteCollection.Armor.Single(i => i.Name == sprite.id), Enums.EquipmentPart.Leggings, sprite.color);
                        break;
                    case EHeroEditorItemPart.Cape:
                        break;
                    case EHeroEditorItemPart.Back:
                        break;
                }
            }
        }

        public bool TryGetWeaponAnimations(int dataId, out HeroEditorWeaponAnimation anims)
        {
            return CacheAnimationsManager.SetAndTryGetCacheWeaponAnimations(Id, weaponAnimations, skillAnimations, dataId, out anims);
        }

        public bool TryGetSkillAnimations(int dataId, out HeroEditorSkillAnimation anims)
        {
            return CacheAnimationsManager.SetAndTryGetCacheSkillAnimations(Id, weaponAnimations, skillAnimations, dataId, out anims);
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

        public override bool GetRandomLeftHandAttackAnimation(int dataId, int randomSeed, out int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            animationIndex = 0;
            return GetLeftHandAttackAnimation(dataId, animationIndex, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public override bool GetRandomRightHandAttackAnimation(int dataId, int randomSeed, out int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            animationIndex = 0;
            return GetRightHandAttackAnimation(dataId, animationIndex, out animSpeedRate, out triggerDurations, out totalDuration);
        }

        public override int GetRightHandAttackRandomMax(int dataId)
        {
            return 1;
        }

        public override int GetLeftHandAttackRandomMax(int dataId)
        {
            return 1;
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

        public override void PlayActionAnimation(AnimActionType animActionType, int dataId, int index, out bool skipMovementValidation, out bool shouldUseRootMotion, float playSpeedMultiplier = 1f)
        {
            skipMovementValidation = false;
            shouldUseRootMotion = false;
            StartCoroutine(PlayActionAnimation_Animator(animActionType, dataId, index, playSpeedMultiplier));
        }

        private IEnumerator PlayActionAnimation_Animator(AnimActionType animActionType, int dataId, int index, float playSpeedMultiplier)
        {
            // If animator is not null, play the action animation
            HeroEditorActionAnimation animation = GetHeroEditorActionAnimation(animActionType, dataId);
            // Action
            Animator.SetFloat(ANIM_SPEED, playSpeedMultiplier);
            Animator.SetTrigger(animation.GetTriggerName());
            AudioManager.PlaySfxClipAtAudioSource(animation.GetRandomAudioClip(), genericAudioSource);
            // Waits by current transition + clip duration before end animation
            yield return new WaitForSecondsRealtime(animation.GetClipLength() / playSpeedMultiplier);
            // Waits by current transition + extra duration before end playing animation state
            yield return new WaitForSecondsRealtime(animation.GetExtraDuration() / playSpeedMultiplier);
        }

        public override void PlaySkillCastClip(int dataId, float duration, out bool skipMovementValidation, out bool shouldUseRootMotion)
        {
            skipMovementValidation = false;
            shouldUseRootMotion = false;
            StartCoroutine(PlaySkillCastClip_Animator(dataId, duration));
        }

        private IEnumerator PlaySkillCastClip_Animator(int dataId, float duration)
        {
            // Cast Skill
            isCasting = true;
            yield return new WaitForSecondsRealtime(duration);
            isCasting = false;
        }

        public override void PlayWeaponChargeClip(int dataId, bool isLeftHand, out bool skipMovementValidation, out bool shouldUseRootMotion)
        {
            // TODO: May implement pulling animation for 2D models
            skipMovementValidation = false;
            shouldUseRootMotion = false;
        }

        public override void StopActionAnimation()
        {
            Animator.SetBool(ANIM_ACTION, false);
        }

        public override void StopSkillCastAnimation()
        {
            Animator.SetBool(ANIM_ACTION, false);
            isCasting = false;
        }

        public override void StopWeaponChargeAnimation()
        {
            // TODO: May implement pulling animation for 2D models
        }

        public override void PlayMoveAnimation()
        {
            if (!Animator.gameObject.activeInHierarchy)
                return;

            if (IsDead)
            {
                Animator.SetInteger(ANIM_STATE, (int)StateTypes.Death);
                return;
            }

            if (isCasting)
            {
                Animator.SetInteger(ANIM_STATE, (int)StateTypes.Ready);
                return;
            }

            if (MovementState.Has(MovementState.Forward) ||
                MovementState.Has(MovementState.Backward) ||
                MovementState.Has(MovementState.Right) ||
                MovementState.Has(MovementState.Left))
            {
                if (ExtraMovementState == ExtraMovementState.IsSprinting)
                    Animator.SetInteger(ANIM_STATE, (int)StateTypes.Run);
                else
                    Animator.SetInteger(ANIM_STATE, (int)StateTypes.Walk);
            }
            else
            {
                Animator.SetInteger(ANIM_STATE, (int)StateTypes.Idle);
            }

            // Update direction
            if (Mathf.Abs(Mathf.Abs(Direction2D.x) - Mathf.Abs(Direction2D.y)) < 0.01f)
            {
                // Up, Down is higher priority
                Vector2 applyDirection2D = Direction2D;
                applyDirection2D.x = 0;
                Direction2D = applyDirection2D.normalized;
            }
            if (Mathf.Abs(Direction2D.x) > Mathf.Abs(Direction2D.y))
            {
                if (Direction2D.x > 0)
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
                if (Direction2D.y > 0)
                {
                    Character4D.SetDirection(Vector2.up);
                }
                else
                {
                    Character4D.SetDirection(Vector2.down);
                }
            }
        }

        public override SkillActivateAnimationType GetSkillActivateAnimationType(int dataId)
        {
            HeroEditorSkillAnimation anims;
            if (!TryGetSkillAnimations(dataId, out anims))
                return SkillActivateAnimationType.UseActivateAnimation;
            return anims.activateAnimationType;
        }
    }
}
