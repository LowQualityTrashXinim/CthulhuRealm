using Microsoft.Xna.Framework;
using Origins.Graphics.Primitives;
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
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CthulhuRealm
{
	public class ChallengeEntity
	{
		ModTileEntity tileEntity = null;
	}
	public class ChallengeSpawnRateIncrease : GlobalNPC
	{
		public static bool isActive = false;
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
			if (!isActive) 
				return;
            maxSpawns *= 3;
			spawnRate /= 3;
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
		public override bool LockChest(int i, int j, ref short frameXAdjustment, ref bool manual) {
			if (frameXAdjustment == 36 * 3) {
				return true;
			}
			return false;
		}
		public override bool IsLockedChest(int i, int j) {
			return Main.tile[i, j].TileFrameX / (36 * 3) == 1;
		}

		public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual) {
			if(!TileEntity.TryGet(i, j, out ChallengeChest tileEntity))
				return false;			
			return tileEntity.isChallengeCompleted;
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
            return false;
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
			Projectile.width = 32;
			Projectile.height = 1024;
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
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.KillMe(info.DamageSource,999999,0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
			default(WallofFlamesShader).Draw(Projectile.Center,MathHelper.Lerp(1,0,Projectile.timeLeft / 200));
            return base.PreDraw(ref lightColor);
        }
		struct WallofFlamesShader 
		{ 
			private static VertexRectangle rect = new(); 
			public void Draw(Vector2 position,float progress)
			{
				MiscShaderData shader = GameShaders.Misc["CR:WallofFlames"].UseImage1(TextureAssets.Extra[193]);
				shader.UseColor(Color.Orange);
				shader.UseSecondaryColor(Color.DarkRed);
				shader.UseShaderSpecificData(new Vector4());
				shader.Apply();
				rect.Draw(position - Main.screenPosition,size: new Vector2(256,2000));

				Main.pixelShader.CurrentTechnique.Passes[0].Apply();
			}
		}
	}
}
