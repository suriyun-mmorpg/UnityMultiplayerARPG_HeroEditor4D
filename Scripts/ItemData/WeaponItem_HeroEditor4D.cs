using MultiplayerARPG.HeroEditor4D;
using Newtonsoft.Json;

namespace MultiplayerARPG
{
    public partial class WeaponItem : IHeroEditorItem
    {
        [Category(11, "In-Scene Objects/Appearance (HeroEditor4D)")]
        [JsonIgnore]
        public HeroEditorSpriteData spriteData;
        [JsonIgnore]
        public HeroEditorSpriteData SpriteData { get { return spriteData; } }
    }
}
