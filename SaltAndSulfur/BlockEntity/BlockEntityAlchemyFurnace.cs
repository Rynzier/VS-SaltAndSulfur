using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

#nullable disable

namespace SaltAndSulfur
{
    public class BlockEntityAlchemyFurnace : BlockEntity, IInteractable
    {
        protected InventoryAlchemyFurnace inventory;
        protected GuiDialogAlchemyFurnace clientDialog;

        public string DialogTitle => Block?.GetPlacedBlockName(Api?.World, Pos);
        public InventoryBase Inventory => inventory;

        public BlockEntityAlchemyFurnace()
        {
            inventory = new InventoryAlchemyFurnace(null, null);
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            inventory.LateInitialize("alchemyfurnace-" + Pos.X + "/" + Pos.Y + "/" + Pos.Z, api);
        }

        public bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            if (world.Api is ICoreClientAPI capi)
            {
                var dlg = new GuiDialogAlchemyFurnace(DialogTitle, Inventory, this.Pos, capi);
                dlg.TryOpen();
            }
            return true;
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);
            inventory.FromTreeAttributes(tree.GetTreeAttribute("inventory"));

            if (Api != null)
            {
                inventory.AfterBlocksLoaded(Api.World);
            }
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            ITreeAttribute invtree = new TreeAttribute();
            inventory.ToTreeAttributes(invtree);
            tree["inventory"] = invtree;

        }

    }
}
