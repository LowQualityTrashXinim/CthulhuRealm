﻿using CthulhuRealm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace CthulhuRealm
{
    /// <summary>
    /// This is taken from my public source mod, this is a simplified utils libs which work for most general stuff<br/>
    /// https://github.com/LowQualityTrashXinim/BossRush
    /// </summary>
	public static partial class ModUtils
    {
        public static Color ScaleRGB(this Color color, float scale)
        {
            color.R = (byte)MathF.Round(color.R * scale);
            color.B = (byte)MathF.Round(color.B * scale);
            color.G = (byte)MathF.Round(color.G * scale);
            return color;
        }
        public static float EaseInBounce(float x)
        {

            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                return n1 * (x -= 1.5f / d1) * x + 0.75f;
            }
            else if (x < 2.5 / d1)
            {
                return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            }
            else
            {
                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            }
        }
        public static float EaseOutBounce(float x)
        {
            return 1f - EaseInBounce(1f - x);
        }
        public static float ClampedLerp(this float from, float to, float value)
        {
            return MathHelper.Lerp(from, to, MathHelper.Clamp(value, 0f, 1f));
        }
        public static Vector2 ClampedLerp(this Vector2 from, Vector2 to, float value)
        {
            return Vector2.Lerp(from, to, MathHelper.Clamp(value, 0f, 1f));
        }
        public static bool IsAnyVanillaBossAlive()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.boss && npc.active)
                {
                    return true;
                }
                else if ((npc.type == NPCID.EaterofWorldsBody
                    || npc.type == NPCID.EaterofWorldsHead
                    || npc.type == NPCID.EaterofWorldsTail)
                    && npc.active)
                {
                    return true;
                }
            }
            return false;
        }

        public static Color ToColor(this Vector4 v)
        {
            return new Color(v.X, v.Y, v.Z, v.W);
        }

        public static Color AddColor(this Color color, Color color2)
        {
            return new Color(color.R + color2.R, color.G + color2.G, color.B + color2.B, color.A + color2.A);
        }

        public static Color DivideColor(this Color color, int number)
        {
            return new Color(color.R / number, color.G / number, color.B / number, color.A / number);
        }

        public static void Push<T>(ref T[] array, T value)
        {
            Array.Copy(array, 0, array, 1, array.Length - 1);
            array[0] = value;
        }
        public static int FastDropItem(Item item, int fastCheckSlot = 0)
        {
            Player player = Main.LocalPlayer;
            if (item == null || item.type == ItemID.None)
                return 0;
            for (int i = fastCheckSlot; i < 50; i++)
            {
                if (player.CanItemSlotAccept(player.inventory[i], item))
                {
                    player.inventory[i] = item.Clone();
                    item.TurnToAir();
                    return i;
                }
            }
            player.DropItem(player.GetSource_DropAsItem(), player.Center, ref item);
            return 50;
        }
        public static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D sparkleTexture = TextureAssets.Extra[98].Value;
            Color bigColor = shineColor * opacity * 0.5f;
            bigColor.A = 0;
            Vector2 origin = sparkleTexture.Size() / 2f;
            Color smallColor = drawColor * 0.5f;
            float lerpValue = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 scaleLeftRight = new Vector2(fatness.X * 0.5f, scale.X) * lerpValue;
            Vector2 scaleUpDown = new Vector2(fatness.Y * 0.5f, scale.Y) * lerpValue;
            bigColor *= lerpValue;
            smallColor *= lerpValue;
            // Bright, large part
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, bigColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, bigColor, 0f + rotation, origin, scaleUpDown, dir);
            // Dim, small part
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, smallColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight * 0.6f, dir);
            Main.EntitySpriteDraw(sparkleTexture, drawpos, null, smallColor, 0f + rotation, origin, scaleUpDown * 0.6f, dir);

        }
        /// <summary>
        /// Attempt to add a tooltip before JourneyResearch line
        /// </summary>
        /// <param name="tooltipsLines"></param>
        /// <param name="newline"></param>
        public static void AddTooltip(ref List<TooltipLine> tooltipsLines, TooltipLine newline)
        {
            int index = tooltipsLines.FindIndex(l => l.Name == "Tooltip0");
            if (index != -1)
            {
                tooltipsLines.Insert(index, newline);
            }
            else
            {
                index = tooltipsLines.FindIndex(l => l.Name == "JourneyResearch");
                if (index != -1 && index > 0)
                {
                    tooltipsLines.Insert(index, newline);
                }
                else
                {
                    tooltipsLines.Add(newline);
                }
            }
        }
        public static T NextFromHashSet<T>(this UnifiedRandom r, HashSet<T> hashset)
        {
            return hashset.ElementAt(r.Next(hashset.Count));
        }
        public static string JumboString(UnifiedRandom r, string str)
        {
            string strCached = str;
            int length = str.Length;
            string result = string.Empty;
            for (int i = 0; i < length; i++)
            {
                int currentlenght = strCached.Length;
                if (currentlenght <= 1)
                {
                    result += strCached;
                    continue;
                }
                int index = r.Next(0, currentlenght);
                char c = strCached[r.Next(0, currentlenght)];
                result += c;
                strCached = strCached.Remove(index, 1);
            }
            return result;
        }
        /// <summary>
        /// Spawn combat text above player without the random Y position
        /// </summary>
        /// <param name="location">player hitbox</param>
        /// <param name="color"></param>
        /// <param name="combatMessage"></param>
        /// <param name="offsetposY"></param>
        /// <param name="dramatic"></param>
        /// <param name="dot"></param>
        public static void CombatTextRevamp(Rectangle location, Color color, string combatMessage, int offsetposY = 0, int timeleft = 30, bool dramatic = false, bool dot = false)
        {
            int drama = 0;
            if (dramatic)
            {
                drama = 1;
            }
            int text = CombatText.NewText(new Rectangle(), color, combatMessage, dramatic, dot);
            if (text < 0 || text >= Main.maxCombatText)
            {
                return;
            }
            CombatText cbtext = Main.combatText[text];
            Vector2 vector = FontAssets.CombatText[drama].Value.MeasureString(cbtext.text);
            cbtext.position.X = location.X + location.Width * 0.5f - vector.X * 0.5f;
            cbtext.position.Y = location.Y + offsetposY + location.Height * 0.25f - vector.Y * 0.5f;
            cbtext.lifeTime += timeleft;
        }
        /// <summary>
        /// Use to order 2 values from smallest to biggest
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static (int, int) Order(float v1, float v2) => v1 < v2 ? ((int)v1, (int)v2) : ((int)v2, (int)v1);
        /// <summary>
        /// Check if there any NPC that is within radius 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static bool LookForAnyHostileNPC(this Vector2 position, float distance)
        {
            foreach (var target in Main.ActiveNPCs)
            {
                if (!target.friendly && target.CanBeChasedBy() && CompareSquareFloatValue(position, target.Center, distance * distance))
                {
                    return true;
                }
            }
            return false;
        }
        public static Vector2 LookForHostileNPCPositionClosest(this Vector2 position, float distance, bool notHitThroughTiles = true)
        {
            Vector2 hostilePos = Vector2.Zero;
            float maxDistanceSquare = distance * distance;
            foreach (var target in Main.ActiveNPCs)
            {
                NPC npc = target;
                if (CompareSquareFloatValue(npc.Center, position, maxDistanceSquare, out float dis)
                    && npc.CanBeChasedBy()
                    && !npc.friendly
                    && (!notHitThroughTiles || Collision.CanHitLine(position, 10, 10, npc.position, npc.width, npc.height))
                    )
                {
                    maxDistanceSquare = dis;
                    hostilePos = npc.Center;
                }
            }
            return hostilePos;
        }
        public static bool LookForHostileNPC(this Vector2 position, out NPC npc, float distance, bool CanLookThroughTile = false)
        {
            float maxDistanceSquare = distance * distance;
            npc = null;
            foreach (var target in Main.ActiveNPCs)
            {
                NPC mainnpc = target;
                if (CompareSquareFloatValue(mainnpc.Center, position, maxDistanceSquare, out float dis)
                    && mainnpc.CanBeChasedBy()
                    && !mainnpc.friendly
                    && (Collision.CanHitLine(position, 10, 10, mainnpc.position, mainnpc.width, mainnpc.height) || !CanLookThroughTile)
                    )
                {
                    maxDistanceSquare = dis;
                    npc = mainnpc;
                }
            }
            return npc != null;
        }
        public static bool LookForHostileNPCNotImmune(this Vector2 position, out NPC npc, float distance, int whoAmI, bool CanLookThroughTile = false)
        {
            float maxDistanceSquare = distance * distance;
            npc = null;
            foreach (var target in Main.ActiveNPCs)
            {
                NPC mainnpc = target;
                if (CompareSquareFloatValue(mainnpc.Center, position, maxDistanceSquare, out float dis)
                    && mainnpc.CanBeChasedBy()
                    && !mainnpc.friendly
                    && (Collision.CanHitLine(position, 0, 0, mainnpc.position, 0, 0) || CanLookThroughTile)
                    && mainnpc.immune[whoAmI] <= 0
                    )
                {
                    maxDistanceSquare = dis;
                    npc = mainnpc;
                }
            }
            return npc != null;
        }
        public static void LookForHostileNPC(this Vector2 position, out List<NPC> npc, float distance)
        {
            npc = new List<NPC>();
            foreach (var target in Main.ActiveNPCs)
            {
                NPC Npc = target;
                if (Npc.CanBeChasedBy() && Npc.type != NPCID.TargetDummy && !Npc.friendly && CompareSquareFloatValueWithHitbox(position, Npc.position, Npc.Hitbox, distance))
                    npc.Add(Npc);
            }
        }
        public static List<NPC> LookForHostileListNPC(this Vector2 position, float distance)
        {
            List<NPC> npclist = new List<NPC>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Npc = Main.npc[i];
                if (Npc.active && Npc.CanBeChasedBy() && Npc.type != NPCID.TargetDummy && !Npc.friendly && CompareSquareFloatValueWithHitbox(position, Npc.position, Npc.Hitbox, distance))
                    npclist.Add(Npc);
            }
            return npclist;
        }
        public static int Safe_SwitchValue(int value, int max, int min = 0, int extraspeed = 0)
        {
            if (max <= 0)
            {
                return value;
            }
            return ++value > max ? min : value + extraspeed;
        }
        public static int ToMinute(float minute) => (int)(ToSecond(60) * minute);
        public static int ToSecond(float second) => (int)(second * 60);
        public static float ToFloatValue(this StatModifier modifier, float additionalMulti = 1, int round = -1)
            => round == -1 ? modifier.Additive * modifier.Multiplicative * additionalMulti : MathF.Round(modifier.Additive * modifier.Multiplicative * additionalMulti, round);
        public static float InExpo(float t) => (float)Math.Pow(2, 5 * (t - 1));
        public static float OutExpo(float t) => 1 - InExpo(1 - t);
        public static float InOutExpo(float t)
        {
            if (t < 0.5) return InExpo(t * 2) * .5f;
            return 1 - InExpo((1 - t) * 2) * .5f;
        }

        public static float InExpo(float t, float strength) => (float)Math.Pow(2, strength * (t - 1));
        public static float OutExpo(float t, float strength) => 1 - InExpo(1 - t, strength);
        public static float InOutExpo(float t, float strength)
        {
            if (t < 0.5) return InExpo(t * 2, strength) * .5f;
            return 1 - InExpo((1 - t) * 2, strength) * .5f;
        }
        public static float InSine(float t) => (float)-Math.Cos(t * MathHelper.PiOver2);
        public static float OutSine(float t) => (float)Math.Sin(t * MathHelper.PiOver2);
        public static float InOutSine(float t) => (float)(Math.Cos(t * Math.PI) - 1) * -.5f;
        public static float InBack(float t)
        {
            float s = 1.70158f;
            return t * t * ((s + 1) * t - s);
        }
        public static float OutBack(float t) => 1 - InBack(1 - t);
        public static float InOutBack(float t)
        {
            if (t < 0.5) return InBack(t) * .5f;
            return 1 - InBack((1 - t)) * .5f;
        }
        public static bool lineLine(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {

            // calculate the direction of the lines
            float uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            float uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            // if uA and uB are between 0-1, lines are colliding
            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                return true;
            }
            return false;
        }
        public static bool Collision_PointAB_EntityCollide(Rectangle entity_Hitbox, Vector2 pointA, Vector2 pointB)
        {
            // check if the line has hit any of the rectangle's sides
            // uses the Line/Line function below
            bool left = lineLine(pointA.X, pointA.Y, pointB.X, pointB.Y, entity_Hitbox.X, entity_Hitbox.Y, entity_Hitbox.X, entity_Hitbox.Y + entity_Hitbox.Height);
            bool right = lineLine(pointA.X, pointA.Y, pointB.X, pointB.Y, entity_Hitbox.X + entity_Hitbox.Width, entity_Hitbox.Y, entity_Hitbox.X + entity_Hitbox.Width, entity_Hitbox.Y + entity_Hitbox.Height);
            bool top = lineLine(pointA.X, pointA.Y, pointB.X, pointB.Y, entity_Hitbox.X, entity_Hitbox.Y, entity_Hitbox.X + entity_Hitbox.Width, entity_Hitbox.Y);
            bool bottom = lineLine(pointA.X, pointA.Y, pointB.X, pointB.Y, entity_Hitbox.X, entity_Hitbox.Y + entity_Hitbox.Height, entity_Hitbox.X + entity_Hitbox.Width, entity_Hitbox.Y + entity_Hitbox.Height);

            // if ANY of the above are true, the line
            // has hit the rectangle
            if (left || right || top || bottom)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculate square length of Vector2 and check if it is smaller than square max distance
        /// This won't power max distance by 2 so do it yourself
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="maxDistance"></param>
        /// <returns>
        /// Return true if length of Vector2 smaller than max distance<br/>
        /// Return false if length of Vector2 greater than max distance
        /// </returns>
        public static bool CompareSquareFloatValue(Vector2 pos1, Vector2 pos2, float maxDistance)
        {
            double value1X = pos1.X,
                value1Y = pos1.Y,
                value2X = pos2.X,
                value2Y = pos2.Y,
                DistanceX = value1X - value2X,
                DistanceY = value1Y - value2Y;
            return (DistanceX * DistanceX + DistanceY * DistanceY) < maxDistance;
        }
        /// <summary>
        /// Calculate square length of Vector2 and check if it is smaller than square max distance
        /// This won't power max distance by 2 so do it yourself
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="maxDistance"></param>
        /// <returns>
        /// Return true if length of Vector2 smaller than max distance<br/>
        /// Return false if length of Vector2 greater than max distance
        /// </returns>
        public static bool CompareSquareFloatValue(Vector2 pos1, Vector2 pos2, float maxDistance, out float distance)
        {
            float
                DistanceX = pos1.X - pos2.X,
                DistanceY = pos1.Y - pos2.Y;
            distance = (DistanceX * DistanceX + DistanceY * DistanceY);
            return distance < maxDistance;
        }
        public static bool CompareSquareFloatValueWithHitbox(Vector2 position, Vector2 positionEntity, Rectangle hitboxEntity, float maxDis)
        {
            float maxDistanceDouble = maxDis * maxDis;

            float disX = position.X - positionEntity.X;
            float disXWidth = disX - hitboxEntity.Width;
            float disXhalfWidth = disX - hitboxEntity.Width * .5f;

            float disY = position.Y - positionEntity.Y;
            float disYHeight = disY - hitboxEntity.Height;
            float disYhalfHeight = disY - hitboxEntity.Height * .5f;
            //|-|
            //0-|
            //|-|
            if (disX * disX + disYhalfHeight * disYhalfHeight < maxDistanceDouble)
                return true;
            //|O|
            //|-|
            //|-|
            if (disXhalfWidth * disXhalfWidth + disY * disY < maxDistanceDouble)
                return true;
            //|-|
            //|-O
            //|-|
            if (disXWidth * disXWidth + disYhalfHeight * disYhalfHeight < maxDistanceDouble)
                return true;
            //|-|
            //|-|
            //|O|
            if (disXhalfWidth * disXhalfWidth + disYHeight * disYHeight < maxDistanceDouble)
                return true;
            //0-|
            //|-|
            //|-|
            if (disX * disX + disY * disY < maxDistanceDouble)
                return true;
            //|-0
            //|-|
            //|-|
            if (disXWidth * disXWidth + disY * disY < maxDistanceDouble)
                return true;
            //|-|
            //|-|
            //0-|
            if (disX * disX + disYHeight * disYHeight < maxDistanceDouble)
                return true;
            //|-|
            //|-|
            //|-0
            if (disXWidth * disXWidth + disYHeight * disYHeight < maxDistanceDouble)
                return true;
            return false;
        }
        private static readonly RasterizerState OverflowHiddenRasterizerState = new RasterizerState
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };
        /// <summary>
        /// Allow for easy to uses everywhere draw progress line without the need of a texture file
        /// </summary>
        /// <param name="progress">The progress of the line</param>
        /// <param name="maxprogress">The maximum progress of the line</param>
        /// <param name="frame">the width and height of the frame which where progress line would be drawn</param>
        /// <param name="position">The position of where the progress line would be drawn</param>
        /// <param name="offsetX">the offset X for the line, usually you would want to take origin X of the frame and put it here for the line to be centered</param>
        /// <param name="line">This include the extra offset of the line</param>
        /// <param name="spriteBatch"><see cref="Main.spriteBatch"/> should be put here</param>
        public static void DrawProgressLine(SpriteBatch spriteBatch, float progress, float maxprogress, Point position, Rectangle line, Color colorA, Color colorB, float curveValue = 0, float strengthCurve = 1)
        {
            float quotient = progress / maxprogress; // Creating a quotient that represents the difference of your currentResource vs your maximumResource, resulting in a float of 0-1f.
            quotient = Math.Clamp(quotient, 0f, 1f); // Clamping it to 0-1f so it doesn't go over that.

            // Here we get the screen dimensions of the barFrame element, then tweak the resulting rectangle to arrive at a rectangle within the barFrame texture that we will draw the gradient. These values were measured in a drawing program.
            Rectangle hitbox = new(
                line.X + position.X
                , line.Y + position.Y
                , line.Width
                , line.Height);

            // Now, using this hitbox, we draw a gradient by drawing vertical lines while slowly interpolating between the 2 colors.
            int left = hitbox.Left;
            int right = hitbox.Right;
            int length = right - left;
            int steps = (int)(length * quotient);

            curveValue = Math.Max(curveValue, 1);

            for (int i = 0; i < steps; i += 1)
            {

                float percentage = i / curveValue;
                if (curveValue >= length - i)
                {
                    percentage = (length - i) / curveValue;
                }

                percentage = ModUtils.OutExpo(percentage, strengthCurve);
                float percentageCurveClamp = Math.Clamp(percentage, 0, 1f);
                int value = (int)Math.Ceiling(hitbox.Height * percentageCurveClamp);
                // float percent = (float)i / steps; // Alternate Gradient Approach
                float percent = i / (float)length;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y + (hitbox.Height - value) / 2, 1, value), Color.Lerp(colorA, colorB, percent));
            }
        }       /// <summary>
                /// Return weapon base on world/player progression
                /// </summary>
                /// <param name="ReturnWeapon"></param>
                /// <param name="Amount"></param>
                /// <param name="rng"></param>
        public static void GetWeapon(out int ReturnWeapon, out int Amount, int rng = 0)
        {
            if (rng > 6 || rng <= 0)
            {
                rng = Main.rand.Next(1, 6);
            }
            ReturnWeapon = 0;
            Amount = 1;
            List<int> DropItemMelee = new List<int>();
            List<int> DropItemRange = new List<int>();
            List<int> DropItemMagic = new List<int>();
            List<int> DropItemSummon = new List<int>();
            List<int> DropItemMisc = new List<int>();
            DropItemMelee.AddRange(TerrariaArrayID.MeleePreBoss);
            DropItemRange.AddRange(TerrariaArrayID.RangePreBoss);
            DropItemMagic.AddRange(TerrariaArrayID.MagicPreBoss);
            DropItemSummon.AddRange(TerrariaArrayID.SummonPreBoss);
            DropItemMisc.AddRange(TerrariaArrayID.SpecialPreBoss);
            DropItemMelee.AddRange(TerrariaArrayID.MeleePreEoC);
            DropItemRange.AddRange(TerrariaArrayID.RangePreEoC);
            DropItemMagic.AddRange(TerrariaArrayID.MagicPreEoC);
            DropItemSummon.AddRange(TerrariaArrayID.SummonerPreEoC);
            DropItemMisc.AddRange(TerrariaArrayID.Special);
            if (NPC.downedBoss1)
            {
                DropItemMelee.Add(ItemID.Code1);
                DropItemMagic.Add(ItemID.ZapinatorGray);
            }
            if (NPC.downedBoss2)
            {
                DropItemMelee.AddRange(TerrariaArrayID.MeleeEvilBoss);
                DropItemRange.Add(ItemID.MoltenFury);
                DropItemRange.Add(ItemID.StarCannon);
                DropItemRange.Add(ItemID.AleThrowingGlove);
                DropItemRange.Add(ItemID.Harpoon);
                DropItemMagic.AddRange(TerrariaArrayID.MagicEvilBoss);
                DropItemSummon.Add(ItemID.ImpStaff);
            }
            if (NPC.downedBoss3)
            {
                DropItemMelee.AddRange(TerrariaArrayID.MeleeSkel);
                DropItemRange.AddRange(TerrariaArrayID.RangeSkele);
                DropItemMagic.AddRange(TerrariaArrayID.MagicSkele);
                DropItemSummon.AddRange(TerrariaArrayID.SummonSkele);
            }
            if (NPC.downedQueenBee)
            {
                DropItemMelee.Add(ItemID.BeeKeeper);
                DropItemRange.Add(ItemID.BeesKnees); DropItemRange.Add(ItemID.Blowgun);
                DropItemMagic.Add(ItemID.BeeGun);
                DropItemSummon.Add(ItemID.HornetStaff);
                DropItemMisc.Add(ItemID.Beenade);
            }
            if (NPC.downedDeerclops)
            {
                DropItemRange.Add(ItemID.PewMaticHorn);
                DropItemMagic.Add(ItemID.WeatherPain);
                DropItemSummon.Add(ItemID.HoundiusShootius);
            }
            if (Main.hardMode)
            {
                DropItemMelee.AddRange(TerrariaArrayID.MeleeHM);
                DropItemRange.AddRange(TerrariaArrayID.RangeHM);
                DropItemMagic.AddRange(TerrariaArrayID.MagicHM);
                DropItemSummon.AddRange(TerrariaArrayID.SummonHM);
            }
            if (NPC.downedQueenSlime)
            {
                DropItemMelee.AddRange(TerrariaArrayID.MeleeQS);
                DropItemSummon.Add(ItemID.Smolstar);
            }
            if (NPC.downedMechBossAny)
            {
                DropItemMelee.AddRange(TerrariaArrayID.MeleeMech);
                DropItemRange.Add(ItemID.SuperStarCannon);
                DropItemRange.Add(ItemID.DD2PhoenixBow);
                DropItemMagic.Add(ItemID.UnholyTrident);
            }
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                DropItemMelee.AddRange(TerrariaArrayID.MeleePostAllMechs);
                DropItemRange.AddRange(TerrariaArrayID.RangePostAllMech);
                DropItemMagic.AddRange(TerrariaArrayID.MagicPostAllMech);
                DropItemSummon.AddRange(TerrariaArrayID.SummonPostAllMech);
            }
            if (NPC.downedPlantBoss)
            {
                DropItemMelee.AddRange(TerrariaArrayID.MeleePostPlant);
                DropItemRange.AddRange(TerrariaArrayID.RangePostPlant);
                DropItemMagic.AddRange(TerrariaArrayID.MagicPostPlant);
                DropItemSummon.AddRange(TerrariaArrayID.SummonPostPlant);
            }
            if (NPC.downedGolemBoss)
            {
                DropItemMelee.AddRange(TerrariaArrayID.MeleePostGolem);
                DropItemRange.AddRange(TerrariaArrayID.RangePostGolem);
                DropItemMagic.AddRange(TerrariaArrayID.MagicPostGolem);
                DropItemSummon.AddRange(TerrariaArrayID.SummonPostGolem);
            }
            if (NPC.downedEmpressOfLight)
            {
                DropItemMelee.AddRange(TerrariaArrayID.MeleePreLuna);
                DropItemRange.AddRange(TerrariaArrayID.RangePreLuna);
                DropItemMagic.AddRange(TerrariaArrayID.MagicPreLuna);
                DropItemSummon.AddRange(TerrariaArrayID.SummonPreLuna);
            }
            if (NPC.downedAncientCultist)
            {
                DropItemMelee.Add(ItemID.DayBreak);
                DropItemMelee.Add(ItemID.SolarEruption);
                DropItemRange.Add(ItemID.Phantasm);
                DropItemRange.Add(ItemID.VortexBeater);
                DropItemMagic.Add(ItemID.NebulaArcanum);
                DropItemMagic.Add(ItemID.NebulaBlaze);
                DropItemSummon.Add(ItemID.StardustCellStaff);
                DropItemSummon.Add(ItemID.StardustDragonStaff);
            }
            if (NPC.downedMoonlord)
            {
                DropItemMelee.Add(ItemID.StarWrath);
                DropItemMelee.Add(ItemID.Meowmere);
                DropItemMelee.Add(ItemID.Terrarian);
                DropItemRange.Add(ItemID.SDMG);
                DropItemRange.Add(ItemID.Celeb2);
                DropItemMagic.Add(ItemID.LunarFlareBook);
                DropItemMagic.Add(ItemID.LastPrism);
                DropItemSummon.Add(ItemID.RainbowCrystalStaff);
                DropItemSummon.Add(ItemID.MoonlordTurretStaff);
            }
            ChooseWeapon(rng, ref ReturnWeapon, ref Amount, DropItemMelee, DropItemRange, DropItemMagic, DropItemSummon, DropItemMisc);
        }
        private static void ChooseWeapon(int rng, ref int weapon, ref int amount, List<int> DropItemMelee, List<int> DropItemRange, List<int> DropItemMagic, List<int> DropItemSummon, List<int> DropItemMisc)
        {
            switch (rng)
            {
                case 0:
                    weapon = ItemID.None;
                    break;
                case 1:
                    weapon = Main.rand.NextFromCollection(DropItemMelee);
                    break;
                case 2:
                    weapon = Main.rand.NextFromCollection(DropItemRange);
                    break;
                case 3:
                    weapon = Main.rand.NextFromCollection(DropItemMagic);
                    break;
                case 4:
                    weapon = Main.rand.NextFromCollection(DropItemSummon);
                    break;
                case 5:
                    amount += 199;
                    weapon = Main.rand.NextFromCollection(DropItemMisc);
                    break;
            }
        }
        /// <summary>
        /// Automatically quick drop player ammo item accordingly to weapon ammo type
        /// </summary>
        /// <param name="lootbox">The id of lootbox</param>
        /// <param name="player">The player</param>
        /// <param name="weapon">Weapon need to be checked</param>
        /// <param name="AmountModifier">Modify the ammount of ammo will be given</param>
        public static void AmmoForWeapon(IEntitySource entitySource, Player player, int weapon, out int ammo, out int amount, float AmountModifier = 1)
        {
            ammo = 0;
            amount = 0;
            Item weapontoCheck = ContentSamples.ItemsByType[weapon];
            if (weapontoCheck.consumable || weapontoCheck.useAmmo == AmmoID.None)
            {
                if (weapontoCheck.mana > 0)
                {
                    player.QuickSpawnItem(entitySource, ItemID.LesserManaPotion, 5);
                }
                return;
            }
            //The most ugly code
            int Amount = (int)(350 * AmountModifier);
            int Ammo;
            if (Main.masterMode)
            {
                Amount += 150;
            }
            List<int> DropArrowAmmo = new();
            List<int> DropBulletAmmo = new();
            List<int> DropDartAmmo = new();

            DropArrowAmmo.AddRange(TerrariaArrayID.defaultArrow);
            DropBulletAmmo.AddRange(TerrariaArrayID.defaultBullet);
            DropDartAmmo.AddRange(TerrariaArrayID.defaultDart);

            if (Main.hardMode)
            {
                DropArrowAmmo.AddRange(TerrariaArrayID.ArrowHM);
                DropBulletAmmo.AddRange(TerrariaArrayID.BulletHM);
                DropDartAmmo.AddRange(TerrariaArrayID.DartHM);
            }
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                DropArrowAmmo.Add(ItemID.ChlorophyteArrow);
                DropBulletAmmo.Add(ItemID.ChlorophyteBullet);
            }
            if (NPC.downedPlantBoss)
            {
                DropArrowAmmo.Add(ItemID.VenomArrow);
                DropBulletAmmo.Add(ItemID.NanoBullet);
                DropBulletAmmo.Add(ItemID.VenomBullet);
            }
            if (weapontoCheck.useAmmo == AmmoID.Arrow)
            {
                Ammo = Main.rand.NextFromCollection(DropArrowAmmo);
            }
            else if (weapontoCheck.useAmmo == AmmoID.Bullet)
            {
                Ammo = Main.rand.NextFromCollection(DropBulletAmmo);
            }
            else if (weapontoCheck.useAmmo == AmmoID.Dart)
            {
                Ammo = Main.rand.NextFromCollection(DropDartAmmo);
            }
            else if (weapontoCheck.mana > 0)
            {
                Ammo = ItemID.LesserManaPotion;
                Amount = (int)(10 * AmountModifier);
            }
            else if (weapontoCheck.useAmmo == AmmoID.Rocket)
            {
                Ammo = Main.rand.Next(new int[] { ItemID.RocketI, ItemID.RocketII, ItemID.RocketIII, ItemID.RocketIV });
            }
            else if (weapontoCheck.useAmmo == AmmoID.Snowball)
            {
                Ammo = ItemID.Snowball;
            }
            else if (weapontoCheck.useAmmo == AmmoID.CandyCorn)
            {
                Ammo = ItemID.CandyCorn;
            }
            else if (weapontoCheck.useAmmo == AmmoID.JackOLantern)
            {
                Ammo = ItemID.JackOLantern;
            }
            else if (weapontoCheck.useAmmo == AmmoID.Flare)
            {
                Ammo = ItemID.Flare;
            }
            else if (weapontoCheck.useAmmo == AmmoID.Stake)
            {
                Ammo = ItemID.Stake;
            }
            else if (weapontoCheck.useAmmo == AmmoID.StyngerBolt)
            {
                Ammo = ItemID.StyngerBolt;
            }
            else if (weapontoCheck.useAmmo == AmmoID.NailFriendly)
            {
                Ammo = ItemID.Nail;
            }
            else if (weapontoCheck.useAmmo == AmmoID.Gel)
            {
                Ammo = ItemID.Gel;
            }
            else if (weapontoCheck.useAmmo == AmmoID.FallenStar)
            {
                Ammo = ItemID.StarCannon;
            }
            else if (weapontoCheck.useAmmo == AmmoID.Sand)
            {
                Ammo = ItemID.SandBlock;
            }
            else
            {
                Ammo = ItemID.WoodenArrow;
            }
            ammo = Ammo;
            amount = Amount;
        }
        public static List<int> GetArmorForPlayer(IEntitySource entitySource, Player player, bool returnOnlyPiece = false)
        {
            List<int> HeadArmor = new List<int>();
            List<int> BodyArmor = new List<int>();
            List<int> LegArmor = new List<int>();
            HeadArmor.AddRange(TerrariaArrayID.HeadArmorPreBoss);
            BodyArmor.AddRange(TerrariaArrayID.BodyArmorPreBoss);
            LegArmor.AddRange(TerrariaArrayID.LegArmorPreBoss);
            if (NPC.downedBoss2)
            {
                HeadArmor.AddRange(TerrariaArrayID.HeadArmorPostEvil);
                BodyArmor.AddRange(TerrariaArrayID.BodyArmorPostEvil);
                LegArmor.AddRange(TerrariaArrayID.LegArmorPostEvil);
            }
            if (NPC.downedBoss3)
            {
                HeadArmor.Add(ItemID.NecroHelmet);
                BodyArmor.Add(ItemID.NecroBreastplate);
                LegArmor.Add(ItemID.NecroGreaves);
            }
            if (NPC.downedQueenBee)
            {
                HeadArmor.Add(ItemID.BeeHeadgear);
                BodyArmor.Add(ItemID.BeeBreastplate);
                LegArmor.Add(ItemID.BeeGreaves);
            }
            if (Main.hardMode)
            {
                HeadArmor.AddRange(TerrariaArrayID.HeadArmorHardMode);
                BodyArmor.AddRange(TerrariaArrayID.BodyArmorHardMode);
                LegArmor.AddRange(TerrariaArrayID.LegArmorHardMode);
            }
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                HeadArmor.AddRange(TerrariaArrayID.HeadArmorPostMech);
                BodyArmor.AddRange(TerrariaArrayID.BodyArmorPostMech);
                LegArmor.AddRange(TerrariaArrayID.LegArmorPostMech);
            }
            if (NPC.downedPlantBoss)
            {
                HeadArmor.AddRange(TerrariaArrayID.HeadArmorPostPlant);
                BodyArmor.AddRange(TerrariaArrayID.BodyArmorPostPlant);
                LegArmor.AddRange(TerrariaArrayID.LegArmorPostPlant);
            }
            if (NPC.downedGolemBoss)
            {
                HeadArmor.AddRange(TerrariaArrayID.HeadArmorPostGolem);
                BodyArmor.AddRange(TerrariaArrayID.BodyArmorPostGolem);
                LegArmor.AddRange(TerrariaArrayID.LegArmorPostGolem);
            }
            if (!returnOnlyPiece)
            {
                return new() { Main.rand.Next(HeadArmor), Main.rand.Next(BodyArmor), Main.rand.Next(LegArmor) };
            }
            else
            {
                return new() { Main.rand.Next([.. LegArmor, .. BodyArmor, .. HeadArmor]) };
            }
        }
        /// <summary>
        ///      Allow user to return a list of number that contain different data to insert into chest <br/>
        ///      0 : Tier 1 Combat acc <br/>
        ///      1 : Tier 1 Health and Mana acc<br/>
        ///      2 : Tier 1 Movement acc<br/>
        ///      3 : Post evil Combat acc<br/>
        ///      4 : Post evil Health and Mana acc<br/>
        ///      5 : Post evil Movement acc<br/>
        ///      6 : Queen bee acc<br/>
        ///      7 : Cobalt Shield<br/>
        ///      8 : Anhk shield sub acc (not include the shield itself)<br/>
        ///      9 : Hardmode acc<br/>
        ///      10 : Fully randomize<br/>
        /// </summary>
        public static int AddAcc(int flag)
        {
            List<int> Accessories = new();
            switch (flag)
            {
                case 0:
                    Accessories.AddRange(TerrariaArrayID.T1CombatAccessory);
                    break;
                case 1:
                    Accessories.AddRange(TerrariaArrayID.T1HealthAndManaAccessory);
                    break;
                case 2:
                    Accessories.AddRange(TerrariaArrayID.T1MovementAccessory);
                    break;
                case 3:
                    Accessories.AddRange(TerrariaArrayID.PostEvilCombatAccessory);
                    break;
                case 4:
                    Accessories.AddRange(TerrariaArrayID.PostEvilHealthManaAccessory);
                    break;
                case 5:
                    Accessories.AddRange(TerrariaArrayID.PostEvilMovementAccessory);
                    break;
                case 6:
                    Accessories.AddRange(TerrariaArrayID.QueenBeeCombatAccessory);
                    break;
                case 7:
                    Accessories.Add(ItemID.CobaltShield);
                    break;
                case 8:
                    Accessories.AddRange(TerrariaArrayID.AnhkCharm);
                    break;
                case 9:
                    Accessories.AddRange(TerrariaArrayID.HMAccessory);
                    break;
                case 10:
                    Accessories.AddRange(TerrariaArrayID.EveryCombatHealtMovehAcc);
                    break;
            }
            return Main.rand.Next(Accessories);
        }
    }
    /// <summary>
    /// Use this to set up your own logic for multi color changing effect, could done this with shader but well
    /// </summary>
    public class ColorInfo
    {
        public const int MaximumProgress = 255;
        public ColorInfo(List<Color> colorlist, float offsetprogress = 0)
        {
            listcolor = colorlist;
            progress = (int)(MaximumProgress * offsetprogress);
            //Attempt to fill up color
            if (listcolor == null)
            {
                return;
            }
            if (listcolor.Count >= 2)
            {
                color1 = listcolor[0];
                color2 = listcolor[1];
                color3 = listcolor[0];
            }
        }

        int currentIndex = 0, progress = 0;
        Color color1 = new Color(), color2 = new Color(), color3 = new Color();
        List<Color> listcolor = new List<Color>();
        public void OffSet(int offset)
        {
            progress = offset;
        }
        public Color MultiColor(int speed = 1)
        {
            if (progress >= MaximumProgress)
                progress = 0;
            else
                progress = Math.Clamp(progress + 1 * speed, 0, MaximumProgress);

            if (listcolor == null || listcolor.Count < 1)
                return Color.White;

            if (listcolor.Count < 2)
                return listcolor[0];

            if (color1.Equals(color2))
            {
                color1 = listcolor[currentIndex];
                color3 = listcolor[currentIndex];
                currentIndex = Math.Clamp((currentIndex + 1 >= listcolor.Count) ? 0 : currentIndex + 1, 0, listcolor.Count - 1);
                color2 = listcolor[currentIndex];
                progress = 0;
            }

            if (!color1.Equals(color2))
                color1 = Color.Lerp(color3, color2, Math.Clamp(progress / 255f, 0, 1f));

            return color1;
        }

    }
    /// <summary>
    /// idk what it returns, so keep that in mind 
    /// </summary>
    public class PlaceTileWithCheck : GenAction
    {
        private ushort _type;
        private int _style;

        public PlaceTileWithCheck(ushort type, int style = 0)
        {
            _type = type;
            _style = style;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (WorldGen.TileEmpty(x, y))
            {
                WorldGen.PlaceTile(x, y, _type, mute: true, forced: false, -1, _style);

            }

            return UnitApply(origin, x, y, args);
        }
    }
}

