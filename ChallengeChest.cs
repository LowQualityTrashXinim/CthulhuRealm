using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
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
using Terraria.GameContent.LootSimulation;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CthulhuRealm
{
	public class ChallengeSpawnRateIncrease : GlobalNPC
	{
		public static bool isActive = false;
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
			if (!isActive) 
				return;
            maxSpawns *= 3;
			spawnRate /= 10;
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
			Projectile.NewProjectileDirect(null,Position.ToWorldCoordinates() + new Vector2(800,0),new Vector2(-20,0),ModContent.ProjectileType<FlameWall>(),1,0,-1,1);
			Projectile.NewProjectileDirect(null,Position.ToWorldCoordinates() - new Vector2(800,0),new Vector2(20,0),ModContent.ProjectileType<FlameWall>(),1,0,-1,0);
			ChallengeSpawnRateIncrease.isActive = true;
			Main.NewText("Survive for 60 Seconds, then the Chest opens!",Color.Yellow);
			Main.SkipToTime(0, false);
		}
        public override void Update()
        {
            if(isChallengeActive)
			{ 
				if(timer == 0) 
				{
					isChallengeActive=false;
					isChallengeCompleted=true;
					ChallengeSpawnRateIncrease.isActive = false;
					for(int i = 0; i < 64; i++)
						Dust.NewDustPerfect(Position.ToWorldCoordinates(),DustID.AmberBolt,Main.rand.NextVector2CircularEdge(12,12),newColor: Color.Orange).noGravity = true;
					foreach(NPC npc in Main.ActiveNPCs)
						if(npc.life > 5 && npc.damage > 0)
						{
							for(int i = 0; i < 64; i++)
								Dust.NewDustPerfect(npc.Center,DustID.AmberBolt,Main.rand.NextVector2CircularEdge(4,4),newColor: Color.Orange).noGravity = true;
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
		public override void SetStaticDefaults() {
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
			TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Containers,2));
			TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<ChallengeChest>().Generic_HookPostPlaceMyPlayer;

			TileObjectData.newTile.Style = 2;
			TileObjectData.newTile.UsesCustomCanPlace = true; 
			TileObjectData.addTile(Type);

		}
		
		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			// When the tile is removed, we need to remove the Tile Entity as well.
			ModContent.GetInstance<ChallengeChest>().Kill(i, j);
		}
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			if(!TileEntity.TryGet(i,j,out ChallengeChest entity))
				return false;

			if(!entity.isChallengeCompleted)
				return true;
			Vector2 pos = entity.Position.ToWorldCoordinates() - Main.screenPosition - new Vector2(-200,-164 + MathF.Sin((float)Main.timeForVisualEffects * 0.04f) * 16);
            spriteBatch.Draw(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPlay").Value,pos,null,Color.White,MathHelper.PiOver2,new Vector2(11,11),1f,SpriteEffects.None,0f);
            return base.PreDraw(i, j, spriteBatch);
        }

        public override bool RightClick(int i, int j)
        {
			if(!TileEntity.TryGet(i, j, out ChallengeChest tileEntity))
				return false;
			if(!tileEntity.isChallengeActive && !tileEntity.isChallengeCompleted)
			{
				tileEntity.ActivateChallenge();
				return true;		
			}
			if(tileEntity.isChallengeCompleted)
			{
				Main.LocalPlayer.QuickSpawnItem(null,ItemID.TerraBlade);
				WorldGen.KillTile(i,j);
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
        public override string Texture =>  "Terraria/Images/Item_1";
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ChallengeChestTile>());
        }
	}

	public class FlameWall : ModProjectile
	{

        public override string Texture => "Terraria/Images/Projectile_0";
		public override void SetDefaults() 
		{
			Projectile.width = 128;
			Projectile.height = 2048;
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
			foreach(Player player in Main.ActivePlayers)
				if(player.getRect().Intersects(Projectile.Hitbox))
					player.KillMe(PlayerDeathReason.ByProjectile(player.whoAmI,Projectile.whoAmI),999999,0);

        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.KillMe(info.DamageSource,999999,0);
        }
        public override bool PreDraw(ref Color lightColor)
        {

			float progress = 1;

			if(Projectile.timeLeft > 3400)
				progress = MathHelper.Lerp(1,0,(float)(Projectile.timeLeft - 3400) / 200);
			if(Projectile.timeLeft <= 200)
				progress = MathHelper.Lerp(0,1,(float)(Projectile.timeLeft) / 200);

			default(WallofFlamesShader).Draw(Projectile.Center,progress);
			
            return base.PreDraw(ref lightColor);
        }
		struct WallofFlamesShader 
		{ 
			private static VertexRectangle rect = new(); 
			public void Draw(Vector2 position,float progress)
			{
				MiscShaderData shader = GameShaders.Misc["CR:WallofFlames"].UseImage1(TextureAssets.Extra[193]);
				shader.UseColor(Color.Red);
				shader.UseSecondaryColor(Color.Crimson);
				shader.UseShaderSpecificData(new Vector4(progress));
				shader.Apply();
				rect.Draw(position - Main.screenPosition,size: new Vector2(256,2000));

				Main.pixelShader.CurrentTechnique.Passes[0].Apply();
			}
		}
	}
}
