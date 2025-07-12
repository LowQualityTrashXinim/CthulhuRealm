using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CthulhuRealm
{
	public class CthulhuRealm : Mod
	{
        public override void Load()
        {
            GameShaders.Misc["CR:WallofFlames"] = new MiscShaderData(this.Assets.Request<Effect>("WallofFlames"),"Flames");
        }
	}
}
