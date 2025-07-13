using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace CthulhuRealm.Subworlds;

public class TestSubworld_System : Subworld
{
    public override int Width => 1000;

    public override int Height => 500;

    public override List<GenPass> Tasks => new List<GenPass>
    {
    };
}
