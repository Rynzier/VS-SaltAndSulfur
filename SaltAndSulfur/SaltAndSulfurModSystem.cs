using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace SaltAndSulfur;

public class SaltAndSulfurModSystem : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.RegisterBlockClass(Mod.Info.ModID + ".BlockAlchemyFurnace", typeof(BlockAlchemyFurnace));
        api.RegisterBlockEntityClass(Mod.Info.ModID + ".BlockEntityAlchemyFurnace", typeof(BlockEntityAlchemyFurnace));
        api.RegisterBlockEntityBehaviorClass("AlchemySmelt", typeof(BEBehaviorAlchemySmelt));
    }
}


