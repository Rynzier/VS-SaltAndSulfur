using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

#nullable disable

namespace SaltAndSulfur
{
    public class BlockEntityAlchemyFurnace : BlockEntityOpenableContainer, IInteractable
    {
        protected InventoryAlchemyFurnace inventory;


        public string DialogTitle => Block?.GetPlacedBlockName(Api?.World, Pos);
        public override InventoryBase Inventory => inventory;
        public override string InventoryClassName => "saltandsulfur:inventoryalchemyfurnace";

        public BlockEntityAlchemyFurnace() : base()
        {
            inventory = new InventoryAlchemyFurnace(null, null);
        }


        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            return true;
        }

        public bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            if (world.Api.Side == EnumAppSide.Client)
            {
                byPlayer.Entity.Die();
            }
            return true;
        }
    }
}
