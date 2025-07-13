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
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CthulhuRealm.Subworlds;

public class CthulhuRealmSubworld : SubworldLibrary.Subworld
{
    public override int Width => 1331;

    public override int Height => 825;

    public override List<GenPass> Tasks => new List<GenPass>
    {
        new PlaceRealm_Pass("PlaceRealm",1),
    };
}

public class PlaceRealm_Pass : GenPass
{
    public PlaceRealm_Pass(string name, double loadWeight) : base(name, loadWeight)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        ModContent.GetInstance<PlaceRealm_System>().PlaceRealm();
    }
}

public class PlaceRealm_System : ModSystem
{
    public override void PreUpdateWorld()
    {
        if (!SubworldSystem.IsActive<CthulhuRealmSubworld>())
            return;

        TileEntity.UpdateStart();
        foreach (TileEntity te in TileEntity.ByID.Values)
        {
            te.Update();
        }
        TileEntity.UpdateEnd();
    }
    public void PlaceRealm()
    {
        Main.spawnTileX = 695;
        Main.spawnTileY = 343;
        Main.worldSurface = 400;
        Main.rockLayer = 479;

        StructureData structureData = Generator.GetStructureData("Content/Structures/CthulhuRealmSH", Mod);
        if (Generator.IsInBounds(structureData, new(0, 0)))
        {
            Generator.GenerateStructure("Content/Structures/CthulhuRealmSH", new(0, 0), Mod);
        }
        for (int i = 0; i < 99999; i++)
        {
            Point point = new(Main.rand.Next(1, 1329), Main.rand.Next(1, 823));
            if (!WorldGen.InWorld(point.X, point.Y))
            {
                continue;
            }
            if (WorldGen.TileEmpty(point.X, point.Y)
                && WorldGen.TileEmpty(point.X + 1, point.Y)
                && WorldGen.TileEmpty(point.X, point.Y + 1)
                && WorldGen.TileEmpty(point.X + 1, point.Y + 1)
                && WorldGen.SolidTile(point.X, point.Y + 2) && WorldGen.SolidTile(point.X + 1, point.Y + 2))
            {
                int success = WorldGen.PlaceChest(point.X, point.Y, TileID.Containers, style: ModContent.TileType<ChallengeChestTile>());
                if(success != -1)
                {
                    Console.WriteLine($"Success placement at {point.ToString()}");
                }
                WorldGen.PlaceTile(point.X, point.Y, ModContent.TileType<ChallengeChestTile>());
            }
        }
    }
}

public class IridescentShard : ModItem
{

    public override string Texture => ModUtils.GetVanillaTexture<Item>(ItemID.BloodMoonStarter);
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        return base.PreDrawInInventory(spriteBatch, position, frame, Color.Pink, itemColor, origin, scale);
    }
    public override void SetDefaults()
    {
        Item.consumable = false;
        Item.width = Item.height = 32;
        Item.useAnimation = Item.useTime = 20;
        Item.useStyle = ItemUseStyleID.HoldUp;
    }

    public override bool? UseItem(Player player)
    {
        if (SubworldSystem.Current == null && player.ItemAnimationJustStarted)
            SubworldSystem.Enter<CthulhuRealmSubworld>();
        return base.UseItem(player);
    }
}