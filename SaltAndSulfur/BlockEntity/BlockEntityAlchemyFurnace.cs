using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

#nullable disable

namespace SaltAndSulfur
{
    public class BlockEntityAlchemyFurnace : BlockEntityOpenableContainer
    {
        protected InventoryAlchemyFurnace inventory;

        public string DialogTitle => Block?.GetPlacedBlockName(Api?.World, Pos);
        public override InventoryBase Inventory => inventory;
        public override string InventoryClassName => "saltandsulfur:inventoryalchemyfurnace";

        public BlockEntityAlchemyFurnace()
        {
            inventory = new InventoryAlchemyFurnace(null, null);
        }

        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            if (Api.Side == EnumAppSide.Client)
            {
                byPlayer.Entity.Die();
            }
            return true;
        }
    }
}
