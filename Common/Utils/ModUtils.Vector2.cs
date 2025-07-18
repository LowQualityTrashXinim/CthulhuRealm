﻿using System;
using Terraria;
using Terraria.Utilities;
using Microsoft.Xna.Framework;

namespace CthulhuRealm
{
	public static partial class ModUtils {
		public static Vector2 LimitedVelocity(this Vector2 velocity, float limited) {
			GetAbsoluteVectorNormalized(velocity, limited, out float X, out float Y);
			velocity.X = Math.Clamp(velocity.X, -X, X);
			velocity.Y = Math.Clamp(velocity.Y, -Y, Y);
			return velocity;
		}
		public static Vector2 NextVector2RectangleEdge(this UnifiedRandom r, Rectangle rect) {
			float X = r.NextFloat(-rect.Width, rect.Width);
			float Y = r.NextFloat(-rect.Height, rect.Height);
			bool Randomdecider = r.NextBool();
			Vector2 RandomPointOnEdge = new Vector2(X * r.NextBool().ToInt(), Y * r.NextBool().ToInt());
			if (RandomPointOnEdge.X == 0) {
				RandomPointOnEdge.X = rect.Width;
			}
			else {
				RandomPointOnEdge.Y = rect.Height;
			}
			return RandomPointOnEdge * r.NextBool().ToDirectionInt();
		}
		public static Vector2 NextVector2RectangleEdge(this UnifiedRandom r, float RectangleWidthHalf, float RectangleHeightHalf) {
			float X = r.NextFloat(-RectangleWidthHalf, RectangleWidthHalf);
			float Y = r.NextFloat(-RectangleHeightHalf, RectangleHeightHalf);
			bool Randomdecider = r.NextBool();
			Vector2 RandomPointOnEdge = new Vector2(X * Randomdecider.ToInt(), Y * (!Randomdecider).ToInt());
			if (RandomPointOnEdge.X == 0) {
				RandomPointOnEdge.X = RectangleWidthHalf;
			}
			else {
				RandomPointOnEdge.Y = RectangleHeightHalf;
			}
			return RandomPointOnEdge * r.NextBool().ToDirectionInt();
		}
		public static Vector2 NextPointOn2Vector2(Vector2 point1, Vector2 point2) {
			float length = Vector2.Distance(point1, point2);
			return point1.PositionOFFSET(point2 - point1, Main.rand.NextFloat(length));
		}
		public static bool Vector2WithinRectangle(this Vector2 position, float X, float Y, Vector2 Center) {
			Vector2 positionNeedCheck1 = new Vector2(Center.X + X, Center.Y + Y);
			Vector2 positionNeedCheck2 = new Vector2(Center.X - X, Center.Y - Y);
			if (position.X < positionNeedCheck1.X && position.X > positionNeedCheck2.X && position.Y < positionNeedCheck1.Y && position.Y > positionNeedCheck2.Y) { return true; }//higher = -Y, lower = Y
			return false;
		}
		public static bool Vector2TouchLine(float pos1, float pos2, float CenterY) => pos1 < (CenterY + pos2) && pos1 > (CenterY - pos2);
		public static bool IsLimitReached(this Vector2 velocity, float limited) => !(velocity.X < limited && velocity.X > -limited && velocity.Y < limited && velocity.Y > -limited);
		public static void GetAbsoluteVectorNormalized(Vector2 velocity, float limit, out float X, out float Y) {
			Vector2 newVelocity = velocity.SafeNormalize(Vector2.Zero) * limit;
			X = Math.Abs(newVelocity.X);
			Y = Math.Abs(newVelocity.Y);
		}
		public static Vector2 Vector2DistributeEvenly(this Vector2 vec, float ProjectileAmount, float rotation, float i) {
			if (ProjectileAmount > 1) {
				rotation = MathHelper.ToRadians(rotation);
				return vec.RotatedBy(MathHelper.Lerp(rotation * .5f, rotation * -.5f, i / ProjectileAmount));
			}
			return vec;
		}
		public static Vector2 InverseVector2DistributeEvenly(this Vector2 vec, float ProjectileAmount, float rotation, float i) {
			if (ProjectileAmount > 1) {
				rotation = MathHelper.ToRadians(rotation);
				return vec.RotatedBy(MathHelper.Lerp(rotation * -.5f, rotation * .5f, i / ProjectileAmount));
			}
			return vec;
		}
		public static Vector2 Vector2DistributeEvenlyPlus(this Vector2 vec, float ProjectileAmount, float rotation, float i) {
			if (ProjectileAmount > 1) {
				rotation = MathHelper.ToRadians(rotation);
				return vec.RotatedBy(MathHelper.Lerp(rotation * .5f, rotation * -.5f, i / (ProjectileAmount - 1f)));
			}
			return vec;
		}
		/// <summary>
		/// Short hand for <see cref="Utils.RotatedByRandom(Vector2, double)"/>
		/// </summary>
		/// <param name="Vec2ToRotate"></param>
		/// <param name="ToRadians"></param>
		/// <returns></returns>
		public static Vector2 Vector2RotateByRandom(this Vector2 Vec2ToRotate, float ToRadians) => Vec2ToRotate.RotatedByRandom(MathHelper.ToRadians(ToRadians));
		public static bool IsCloseToPosition(this Vector2 CurrentPosition, Vector2 Position, float distance) => Vector2.DistanceSquared(CurrentPosition, Position) <= distance * distance;
		/// <summary>
		/// The higher the number, the heavier this method become, NOT RECOMMEND USING IT AT ALL COST
		/// </summary>
		/// <param name="position"></param>
		/// <param name="ProjectileVelocity"></param>
		/// <param name="offSetBy"></param>
		/// <param name="accurancyCheck">off set this by 1, since the starting accurancy check is 1</param>
		/// <returns></returns>
		public static Vector2 PositionOffsetDynamic(this Vector2 position, Vector2 ProjectileVelocity, float offSetBy, int width1 = 0, int height1 = 0, int accurancyCheck = 0) {
			Vector2 OFFSET = ProjectileVelocity.SafeNormalize(Vector2.Zero);
			for (float i = offSetBy; i > 0; i--) {
				if (Collision.CanHitLine(position, 16, 16, position + OFFSET * i, width1, height1)) {
					return position += OFFSET * i;
				}
				i -= accurancyCheck;
			}
			return position;
		}
		public static Vector2 PositionOFFSET(this Vector2 position, Vector2 ProjectileVelocity, float offSetBy) {
			Vector2 OFFSET = ProjectileVelocity.SafeNormalize(Vector2.Zero) * offSetBy;
			if (Collision.CanHitLine(position, 0, 0, position + OFFSET, 0, 0)) {
				return position += OFFSET;
			}
			return position;
		}
		public static Vector2 IgnoreTilePositionOFFSET(this Vector2 position, Vector2 ProjectileVelocity, float offSetBy) {
			Vector2 OFFSET = ProjectileVelocity.SafeNormalize(Vector2.Zero) * offSetBy;
			return position += OFFSET;
		}
		public static Vector2 Vector2RandomSpread(this Vector2 ToRotateAgain, float Spread, float additionalMultiplier = 1) {
			ToRotateAgain.X += Main.rand.NextFloat(-Spread, Spread) * additionalMultiplier;
			ToRotateAgain.Y += Main.rand.NextFloat(-Spread, Spread) * additionalMultiplier;
			return ToRotateAgain;
		}
		public static Vector2 Add(this Vector2 vec, float x, float y) {
			vec.X -= x;
			vec.Y -= y;
			return vec;
		}
		public static Vector2 Subtract(this Vector2 vec, float x, float y) {
			vec.X += x;
			vec.Y += y;
			return vec;
		}
	}
}
