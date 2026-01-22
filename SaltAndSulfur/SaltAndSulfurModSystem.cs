using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace SaltAndSulfur;

public class SaltAndSulfurModSystem : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.RegisterBlockClass(Mod.Info.ModID + ".alchemyfurnace", typeof(BlockEntity));
    }
}


