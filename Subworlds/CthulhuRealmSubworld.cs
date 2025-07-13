using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StructureHelper.API;
using StructureHelper.Models;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CthulhuRealm.Subworlds;

public class CthulhuRealmSubworld : SubworldLibrary.Subworld
{
    public override int Width => 1331;
    public override int Width => 3000;

    public override int Height => 3000;

    public override List<GenPass> Tasks => new List<GenPass>
    {
        new PlaceRealm_Pass("Place Realm",100),
    };
}

public class PlaceRealm_Pass : GenPass
{
    public PlaceRealm_Pass(string name, double loadWeight) : base(name, loadWeight)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Entering The Realm";
    }
}

public class PlaceRealm_System : ModSystem
{
    public void PlaceRealm()
    {
    }
}

public class IridescentShard : ModItem
{

    public override string Texture => ModUtils.GetVanillaTexture<Item>(ItemID.BloodMoonStarter);
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        return base.PreDrawInInventory(spriteBatch, position, frame, Color.Pink, Color.Pink, origin, scale);
    }
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.Star);
        Item.consumable = false;
        Item.width = Item.height = 32;
        Item.useAnimation = Item.useTime = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
        
    }

    public override bool? UseItem(Player player)
    {
        if (SubworldSystem.Current == null && player.ItemAnimationJustStarted)
            SubworldSystem.Enter<CthulhuRealmSubworld>();
        else
            SubworldSystem.Exit();
        return base.UseItem(player);
    }
}