﻿using CthulhuRealm.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CthulhuRealm
{
	public static partial class ModUtils {
		public static void ProjectileDefaultDrawInfo(this Projectile projectile, out Texture2D texture, out Vector2 origin) {
			Main.instance.LoadProjectile(projectile.type);
			texture = TextureAssets.Projectile[projectile.type].Value;
			origin = texture.Size() * .5f;
		}
		public static void ItemDefaultDrawInfo(this Item item, out Texture2D texture, out Vector2 origin) {
			Main.instance.LoadItem(item.type);
			texture = TextureAssets.Item[item.type].Value;
			origin = texture.Size() * .5f;
		}
		public static void DrawTrail(this Projectile projectile, Color lightColor, float ManualScaleAccordinglyToLength = 0) {
			projectile.ProjectileDefaultDrawInfo(out Texture2D texture, out Vector2 origin);
			SpriteEffects effect = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			if (ProjectileID.Sets.TrailingMode[projectile.type] != 2) {
				for (int k = 0; k < projectile.oldPos.Length; k++) {
					Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, projectile.gfxOffY);
					Color color = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
					color.A = (byte)projectile.alpha;
					Main.EntitySpriteDraw(texture, drawPos, null, color, projectile.rotation, origin, projectile.scale - k * ManualScaleAccordinglyToLength, effect, 0);
				}
			}
			else {
				for (int k = 0; k < projectile.oldPos.Length; k++) {
					Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, projectile.gfxOffY);
					Color color = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
					color.A = (byte)projectile.alpha;
					float scaling = Math.Clamp(k * ManualScaleAccordinglyToLength, 0, 10f);
					Main.EntitySpriteDraw(texture, drawPos, null, color, projectile.oldRot[k], origin, projectile.scale - scaling, effect, 0);
				}
			}
		}
		public static void DrawTrailWithoutAlpha(this Projectile projectile, Color lightColor, float ManualScaleAccordinglyToLength = 0) {
			projectile.ProjectileDefaultDrawInfo(out Texture2D texture, out Vector2 origin);
			SpriteEffects effect = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			if (ProjectileID.Sets.TrailingMode[projectile.type] != 2) {
				for (int k = 0; k < projectile.oldPos.Length; k++) {
					Color color = lightColor * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
					Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, projectile.gfxOffY);
					Main.EntitySpriteDraw(texture, drawPos, null, color, projectile.rotation, origin, projectile.scale - k * ManualScaleAccordinglyToLength, effect, 0);
				}
			}
			else {
				for (int k = 0; k < projectile.oldPos.Length; k++) {
					Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, projectile.gfxOffY);
					Color color = lightColor * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
					float scaling = Math.Clamp(k * ManualScaleAccordinglyToLength, 0, 10f);
					Main.EntitySpriteDraw(texture, drawPos, null, color, projectile.oldRot[k], origin, projectile.scale - scaling, effect, 0);
				}
			}
		}
		public static void DrawTrailWithoutColorAdjustment(this Projectile projectile, Color lightColor, float ManualScaleAccordinglyToLength = 0) {
			projectile.ProjectileDefaultDrawInfo(out Texture2D texture, out Vector2 origin);
			if (ProjectileID.Sets.TrailingMode[projectile.type] != 2) {
				for (int k = 0; k < projectile.oldPos.Length; k++) {
					Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, projectile.gfxOffY);
					Main.EntitySpriteDraw(texture, drawPos, null, lightColor, projectile.rotation, origin, projectile.scale - k * ManualScaleAccordinglyToLength, SpriteEffects.None, 0);
				}
			}
			else {
				for (int k = 0; k < projectile.oldPos.Length; k++) {
					Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, projectile.gfxOffY);
					Main.EntitySpriteDraw(texture, drawPos, null, lightColor, projectile.oldRot[k], origin, projectile.scale - k * ManualScaleAccordinglyToLength, SpriteEffects.None, 0);
				}
			}
		}
		public static void ProjectileAlphaDecay(this Projectile projectile, float timeCountdown) {
			projectile.alpha = Math.Clamp((int)MathHelper.Lerp(0, 255, (timeCountdown - projectile.timeLeft) / timeCountdown), 0, 255);
		}
		public static void BresenhamCircle(Vector2 pos, int radius, Color color) {
			Texture2D texture = ModContent.Request<Texture2D>(ModTexture.WHITEDOT).Value;
			int x = 0, y = radius;
			int decesionParameter = 3 - 2 * radius;
			DrawBresenhamCircle(texture, pos, x, y, color);
			while (y >= x) {
				x++;
				if (decesionParameter > 0) {
					y--;
					decesionParameter = decesionParameter + 4 * (x - y) + 10;
				}
				else {
					decesionParameter = decesionParameter + 4 * x + 6;//This is magic math value, don't ask me
				}
				DrawBresenhamCircle(texture, pos, x, y, color);
			}
		}
		private static void DrawBresenhamCircle(Texture2D texture, Vector2 c, int x, int y, Color color) {
			Vector2 drawPos = c - Main.screenPosition;
			Main.spriteBatch.Draw(texture, drawPos.Subtract(x, y), null, color, 0, Vector2.One, 1, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texture, drawPos.Subtract(-x, y), null, color, 0, Vector2.One, 1, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texture, drawPos.Subtract(x, -y), null, color, 0, Vector2.One, 1, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texture, drawPos.Subtract(-x, -y), null, color, 0, Vector2.One, 1, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texture, drawPos.Subtract(y, x), null, color, 0, Vector2.One, 1, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texture, drawPos.Subtract(-y, x), null, color, 0, Vector2.One, 1, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texture, drawPos.Subtract(y, -x), null, color, 0, Vector2.One, 1, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texture, drawPos.Subtract(-y, -x), null, color, 0, Vector2.One, 1, SpriteEffects.None, 0);
		}
	}
}
