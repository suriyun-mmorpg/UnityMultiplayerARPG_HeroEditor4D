using MultiplayerARPG.HeroEditor4D;

namespace MultiplayerARPG
{
    public partial class Item : IHeroEditorItem
    {
        public HeroEditorSpriteData spriteData;
        public HeroEditorSpriteData SpriteData { get { return spriteData; } }
    }
}
