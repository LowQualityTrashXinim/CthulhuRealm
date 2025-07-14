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

    public override int Height => 865;

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
public class PlaceChests_Pass : GenPass
{
    public PlaceChests_Pass(string name, double loadWeight) : base(name, loadWeight)
    {
    }
    
    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        
        for(int i = 0; i < SubworldSystem.Current.Width; i++)
            for(int j = 0; j < SubworldSystem.Current.Height; j++)
            {
                WorldGen.PlaceObject(i,j,ModContent.TileType<ChallengeChestTile>(),true);
            }
    }
}
public class PlaceRealm_System : ModSystem
{
    public override void PreUpdateWorld()
    {
        if (!SubworldSystem.IsActive<CthulhuRealmSubworld>())
        {
            ChallengeGlobalNPC.isActive = false;
            return;
        }

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
        //for (int i = 0; i < 99999; i++)
        //{
        //    Point point = new(Main.rand.Next(1, 1329), Main.rand.Next(1, 823));
        //    if (!WorldGen.InWorld(point.X, point.Y))
        //    {
        //        continue;
        //    }
        //    WorldGen.PlaceChest(point.X, point.Y, TileID.Containers);
        //}
    }
}

public class RealmPortal : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.AnimatesAsSoul[Type] = true;
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 10));
        ItemID.Sets.ItemIconPulse[Item.type] = true;
        ItemID.Sets.ItemNoGravity[Item.type] = true;
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
        if (SubworldSystem.Current == null && player.ItemAnimationJustStarted && player.BuyItem(Item.buyPrice(0,15)))
            SubworldSystem.Enter<CthulhuRealmSubworld>();
        return base.UseItem(player);
    }
}