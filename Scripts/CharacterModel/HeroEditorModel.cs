using Assets.HeroEditor4D.Common.CharacterScripts;
using HeroEditor.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiplayerARPG.HeroEditor4D
{
    [RequireComponent(typeof(Character4D))]
    public class HeroEditorModel : BaseCharacterModel
    {
        public const List<Sprite> EmptySprites = null;
        public const Sprite EmptySprite = null;
        public SpriteCollection spriteCollection;
        private Character4D character4D;
        private HashSet<EHeroEditorItemPart> equippedParts = new HashSet<EHeroEditorItemPart>();

        protected override void Awake()
        {
            base.Awake();
            character4D = GetComponent<Character4D>();
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

        public override bool GetLeftHandAttackAnimation(int dataId, int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            throw new System.NotImplementedException();
        }

        public override bool GetLeftHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            throw new System.NotImplementedException();
        }

        public override bool GetRandomLeftHandAttackAnimation(int dataId, out int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            throw new System.NotImplementedException();
        }

        public override bool GetRandomRightHandAttackAnimation(int dataId, out int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            throw new System.NotImplementedException();
        }

        public override bool GetRightHandAttackAnimation(int dataId, int animationIndex, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            throw new System.NotImplementedException();
        }

        public override bool GetRightHandReloadAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            throw new System.NotImplementedException();
        }

        public override bool GetSkillActivateAnimation(int dataId, out float animSpeedRate, out float[] triggerDurations, out float totalDuration)
        {
            throw new System.NotImplementedException();
        }

        public override Coroutine PlayActionAnimation(AnimActionType animActionType, int dataId, int index, float playSpeedMultiplier = 1)
        {
            throw new System.NotImplementedException();
        }

        public override void PlayMoveAnimation()
        {
            throw new System.NotImplementedException();
        }

        public override Coroutine PlaySkillCastClip(int dataId, float duration)
        {
            throw new System.NotImplementedException();
        }

        public override void PlayWeaponChargeClip(int dataId, bool isLeftHand)
        {
            throw new System.NotImplementedException();
        }

        public override void StopActionAnimation()
        {
            throw new System.NotImplementedException();
        }

        public override void StopSkillCastAnimation()
        {
            throw new System.NotImplementedException();
        }

        public override void StopWeaponChargeAnimation()
        {
            throw new System.NotImplementedException();
        }

        public override SkillActivateAnimationType UseSkillActivateAnimationType(int dataId)
        {
            throw new System.NotImplementedException();
        }
    }
}
