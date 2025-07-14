using CthulhuRealm.Common.Utils;
using CthulhuRealm.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.LootSimulation;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CthulhuRealm
{
    public class ChallengeDropRule : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info)
        {   
            return SubworldSystem.IsActive<CthulhuRealmSubworld>();
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "must be not in the Cthulhu's Realm";
        }
    }
    
    public class ChallengePlayer : ModPlayer
    {
        public override void PreUpdateBuffs()
        {
            if (SubworldSystem.Current is CthulhuRealmSubworld)
            {
                Player.AddBuff(BuffID.NoBuilding, 60);

            }
        }

        public override bool CanUseItem(Item item)
        {
            if (SubworldSystem.Current is not CthulhuRealmSubworld)
                return true;    

             if(item.pick > 0 || item.axe > 0 || item.hammer > 0)
                return false;

             return true;    
        }

        public override void OnRespawn()
        {
            if (SubworldSystem.Current is CthulhuRealmSubworld)
                SubworldSystem.Exit();
        }
    }
    public class ChallengeGlobalNPC : GlobalNPC
    {
        public static bool isActive = false;
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (!isActive || !SubworldSystem.IsActive<CthulhuRealmSubworld>())
                return; 
            spawnRate /= 8;
        }

        public override void SetDefaults(NPC entity)
        {
            if (!SubworldSystem.IsActive<CthulhuRealmSubworld>())
                return;

            entity.knockBackResist *= 0.1f;

            if (!Main.hardMode)
            {
                if (NPC.downedBoss2)
                    entity.lifeMax += (int)(entity.lifeMax * .25f);

                if (NPC.downedBoss3)
                    entity.lifeMax += (int)(entity.lifeMax * .25f);
            }
            else
                entity.lifeMax += (int)(entity.lifeMax * .25f);

            if (NPC.downedMechBossAny)
                entity.lifeMax += (int)(entity.lifeMax * .25f);

            if (NPC.downedPlantBoss)
                entity.lifeMax += (int)(entity.lifeMax * .25f);

            if (NPC.downedGolemBoss)
                entity.lifeMax += (int)(entity.lifeMax * .25f);

            if (NPC.downedAncientCultist)
                entity.lifeMax += (int)(entity.lifeMax * 0.75f);

            if (NPC.downedMoonlord)
                entity.lifeMax += (int)(entity.lifeMax * .25f);
        }
        // make it so that npcs dont drop items when inside the subworld cuz balance lol
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // dont know how to make it work lol
            //foreach(IItemDropRule rule in npcLoot.Get())
            //{
            //    npcLoot.Remove(rule);
            //    var challengeRule = new LeadingConditionRule(new ChallengeDropRule()).OnSuccess(rule);
            //    npcLoot.Add(challengeRule);
            //}

            if(npc.type == NPCID.EyeofCthulhu)
            { 
                
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RealmPortal>(),1));

            }
        }
    }
    public class ChallengeChest : ModTileEntity
    {
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<ChallengeChestTile>();
        }

        public int timer = 3600;
        public bool isChallengeActive = false;
        public bool isChallengeCompleted = false;

        public void ActivateChallenge()
        {
            isChallengeActive = true;
            Projectile.NewProjectileDirect(null, Position.ToWorldCoordinates() + new Vector2(800, 0), new Vector2(-20, 0), ModContent.ProjectileType<FlameWall>(), 1, 0, -1, 1);
            Projectile.NewProjectileDirect(null, Position.ToWorldCoordinates() - new Vector2(800, 0), new Vector2(20, 0), ModContent.ProjectileType<FlameWall>(), 1, 0, -1, 0);
            ChallengeGlobalNPC.isActive = true;
            Main.NewText("Survive for 60 seconds to unlock the Chest!", Color.Yellow);
            Main.SkipToTime(0, false);
        }
        public override void Update()
        {
            if (isChallengeActive)
            {
                if (timer == 0)
                {
                    isChallengeActive = false;
                    isChallengeCompleted = true;
                    ChallengeGlobalNPC.isActive = false;
                    for (int i = 0; i < 64; i++)
                        Dust.NewDustPerfect(Position.ToWorldCoordinates(), DustID.AmberBolt, Main.rand.NextVector2CircularEdge(12, 12), newColor: Color.Orange).noGravity = true;
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        for (int i = 0; i < 64; i++)
                            Dust.NewDustPerfect(npc.Center, DustID.AmberBolt, Main.rand.NextVector2CircularEdge(4, 4), newColor: Color.Orange).noGravity = true;
                        npc.active = false;
                    }

                }
                timer--;
            }
        }
    }
    public class ChallengeChestTile : ModTile
    {

        public override string Texture => "Terraria/Images/Tiles_21";
        public override string HighlightTexture => "Terraria/Images/Misc/TileOutlines/Tiles_21";
        public static LocalizedText InfoText { get; private set; }
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileSpelunker[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1200;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileOreFinderPriority[Type] = 500;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.BasicChest[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.AvoidedByNPCs[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.IsAContainer[Type] = true;
            TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
            AdjTiles = [TileID.Containers];
            VanillaFallbackOnModDeletion = TileID.Containers;
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Containers, 2));
            TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<ChallengeChest>().Generic_HookPostPlaceMyPlayer;

            TileObjectData.newTile.Style = 2;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
            InfoText = this.GetLocalization(nameof(InfoText));
        }
        public override void MouseOver(int i, int j)
        {
            if (!TileEntity.TryGet(i, j, out ChallengeChest entity))
                return;
            if(entity.isChallengeActive || entity.isChallengeCompleted)
                return;
            Player player = Main.LocalPlayer;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = -1;
			player.cursorItemIconText = InfoText.Format("Costs 5 Gold Coins to open"); 
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<ChallengeChest>().Kill(i, j);
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!TileEntity.TryGet(i, j, out ChallengeChest entity))
                return false;

            if (!entity.isChallengeCompleted)
                return true;
            Vector2 pos = entity.Position.ToWorldCoordinates() - Main.screenPosition - new Vector2(-200, -164 + MathF.Sin((float)Main.timeForVisualEffects * 0.04f) * 16);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPlay").Value, pos, null, Color.White, MathHelper.PiOver2, new Vector2(11, 11), 1f, SpriteEffects.None, 0f);
            return base.PreDraw(i, j, spriteBatch);
        }

        public override bool RightClick(int i, int j)
        {
            if (!TileEntity.TryGet(i, j, out ChallengeChest tileEntity))
                return false;
            if (!tileEntity.isChallengeActive && !tileEntity.isChallengeCompleted && Main.LocalPlayer.BuyItem(Item.buyPrice(0,5)))
            {
                tileEntity.ActivateChallenge();
                return true;
            }
            if (tileEntity.isChallengeCompleted)
            {
                ModUtils.GetWeapon(out int type, out int amount);
                ModUtils.GetArmorForPlayer(null, Main.LocalPlayer,true);
                Main.LocalPlayer.QuickSpawnItem(null, type);
                WorldGen.KillTile(i, j);
            }
            return false;
        }
    }

    public class WhyIhaveToDoThisTileGlobal : GlobalTile
    {
        public override bool CanDrop(int i, int j, int type)
        {
            return type != ModContent.TileType<ChallengeChestTile>();

        }
    }

    public class DebugChallengeChest : ModItem
    {
        public override string Texture => "Terraria/Images/Item_1";
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ChallengeChestTile>());
        }
    }

    public class FlameWall : ModProjectile
    {

        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 20000;
        }
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.9f;
            foreach (Player player in Main.ActivePlayers)
                if (player.getRect().Intersects(Projectile.Hitbox))
                    player.KillMe(PlayerDeathReason.ByProjectile(player.whoAmI, Projectile.whoAmI), 999999, 0);

        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.KillMe(info.DamageSource, 999999, 0);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if(Projectile.Center.X > targetHitbox.X && Projectile.ai[0] == 0)
                return true;

            if(Projectile.Center.X < targetHitbox.X && Projectile.ai[0] == 1)
                return true;


            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            float progress = 1;

            if (Projectile.timeLeft > 3400)
                progress = MathHelper.Lerp(1, 0, (float)(Projectile.timeLeft - 3400) / 200);
            if (Projectile.timeLeft <= 200)
                progress = MathHelper.Lerp(0, 1, (float)(Projectile.timeLeft) / 200);

            default(WallofFlamesShader).Draw(Projectile.Center, progress, (int)Projectile.ai[0]);

            return base.PreDraw(ref lightColor);
        }
        struct WallofFlamesShader
        {
            private static VertexRectangle rect = new();
            public void Draw(Vector2 position, float progress, int side)
            {
                MiscShaderData shader = GameShaders.Misc["CR:WallofFlames"].UseImage1(TextureAssets.Extra[193]);
                shader.UseColor(Color.Red);
                shader.UseSecondaryColor(Color.Crimson);
                shader.UseShaderSpecificData(new Vector4(progress));
                shader.Apply();
                rect.Draw(position - Main.screenPosition, size: new Vector2(256, 16000));

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                
            }
        }
    }
}
