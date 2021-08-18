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
            for (int i = 0; i < (int)EHeroEditorItemPart.Count; ++i)
            {
                switch ((EHeroEditorItemPart)i)
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

            Dictionary<string, SpriteGroupEntry> dict;
            foreach (HeroEditorSpriteData sprite in sprites.Values)
            {
                switch (sprite.part)
                {
                    case EHeroEditorItemPart.Armor:
                        dict = spriteCollection.Armor.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.EquipArmor(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.ArmorRenderers != null)
                                j.ArmorRenderers.ForEach(k => k.color = sprite.color);
                        });
                        break;
                    case EHeroEditorItemPart.Helmet:
                        dict = spriteCollection.Armor.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.EquipHelmet(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.HelmetRenderer != null)
                                j.HelmetRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Shield:
                        dict = spriteCollection.Shield.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.EquipShield(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.ShieldRenderers != null)
                                j.ShieldRenderers.ForEach(k => k.color = sprite.color);
                        });
                        break;
                    case EHeroEditorItemPart.Melee1H:
                        dict = spriteCollection.MeleeWeapon1H.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.EquipMeleeWeapon1H(dict[sprite.id].Sprite);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.PrimaryWeaponRenderer != null)
                                j.PrimaryWeaponRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Melee2H:
                        dict = spriteCollection.MeleeWeapon2H.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.EquipMeleeWeapon2H(dict[sprite.id].Sprite);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.PrimaryWeaponRenderer != null)
                                j.PrimaryWeaponRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Bow:
                        dict = spriteCollection.Bow.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.EquipBow(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.BowRenderers != null)
                                j.BowRenderers.ForEach(k => k.color = sprite.color);
                        });
                        break;
                    case EHeroEditorItemPart.Crossbow:
                        dict = spriteCollection.Crossbow.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.EquipCrossbow(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.PrimaryWeaponRenderer != null)
                                j.PrimaryWeaponRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Firearm1H:
                        dict = spriteCollection.Firearm1H.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.EquipSecondaryFirearm(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.SecondaryWeaponRenderer != null)
                                j.SecondaryWeaponRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Supplies:
                        dict = spriteCollection.Supplies.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.EquipSupply(dict[sprite.id].Sprite);
                        break;
                    case EHeroEditorItemPart.Body:
                        dict = spriteCollection.Body.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.SetBody(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.BodyRenderers != null)
                                j.BodyRenderers.ForEach(k => k.color = sprite.color);
                        });
                        break;
                    case EHeroEditorItemPart.Ears:
                        dict = spriteCollection.Ears.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.SetEars(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.EarsRenderers != null)
                                j.EarsRenderers.ForEach(k => k.color = sprite.color);
                        });
                        break;
                    case EHeroEditorItemPart.Eyebrows:
                        dict = spriteCollection.Eyebrows.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.SetEyebrows(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.EyebrowsRenderer != null)
                                j.EyebrowsRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Eyes:
                        dict = spriteCollection.Eyes.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.SetEyes(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.EyesRenderer != null)
                                j.EyesRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Hair:
                        dict = spriteCollection.Hair.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.SetHair(dict[sprite.id]);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.HairRenderer != null)
                                j.HairRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Beard:
                        dict = spriteCollection.Beard.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.SetBeard(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.BeardRenderer != null)
                                j.BeardRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Mouth:
                        dict = spriteCollection.Mouth.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.SetMouth(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.MouthRenderer != null)
                                j.MouthRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Makeup:
                        dict = spriteCollection.Makeup.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.SetMakeup(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.MakeupRenderer != null)
                                j.MakeupRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Mask:
                        dict = spriteCollection.Mask.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.SetMask(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.MaskRenderer != null)
                                j.MaskRenderer.color = sprite.color;
                        });
                        break;
                    case EHeroEditorItemPart.Earrings:
                        dict = spriteCollection.Earrings.ToDictionary(i => i.FullName, i => i);
                        if (dict.ContainsKey(sprite.id))
                            Character4D.SetEarrings(dict[sprite.id].Sprites);
                        Character4D.Parts.ForEach(j =>
                        {
                            if (j.EarringsRenderers != null)
                                j.EarringsRenderers.ForEach(k => k.color = sprite.color);
                        });
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

        public override void PlayActionAnimation(AnimActionType animActionType, int dataId, int index, float playSpeedMultiplier = 1f)
        {
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

        public override void PlaySkillCastClip(int dataId, float duration)
        {
            StartCoroutine(PlaySkillCastClip_Animator(dataId, duration));
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

            if (isDead)
            {
                Animator.SetInteger(ANIM_STATE, (int)StateTypes.Death);
                return;
            }

            if (isCasting)
            {
                Animator.SetInteger(ANIM_STATE, (int)StateTypes.Ready);
                return;
            }

            if (movementState.HasFlag(MovementState.Forward) ||
                movementState.HasFlag(MovementState.Backward) ||
                movementState.HasFlag(MovementState.Right) ||
                movementState.HasFlag(MovementState.Left))
            {
                if (extraMovementState == ExtraMovementState.IsSprinting)
                    Animator.SetInteger(ANIM_STATE, (int)StateTypes.Run);
                else
                    Animator.SetInteger(ANIM_STATE, (int)StateTypes.Walk);
            }
            else
            {
                Animator.SetInteger(ANIM_STATE, (int)StateTypes.Idle);
            }

            // Update direction
            if (Mathf.Abs(Mathf.Abs(direction2D.x) - Mathf.Abs(direction2D.y)) < 0.01f)
            {
                // Up, Down is higher priority
                Vector2 applyDirection2D = direction2D;
                applyDirection2D.x = 0;
                direction2D = applyDirection2D.normalized;
            }
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

        public override SkillActivateAnimationType GetSkillActivateAnimationType(int dataId)
        {
            HeroEditorSkillAnimation anims;
            if (!TryGetSkillAnimations(dataId, out anims))
                return SkillActivateAnimationType.UseActivateAnimation;
            return anims.activateAnimationType;
        }
    }
}
