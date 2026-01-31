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
    public class BlockEntityAlchemyFurnace : BlockEntityOpenableContainer
    {
        protected InventoryAlchemyFurnace inventory;
        protected GuiDialogAlchemyFurnace clientDialog;

        public string DialogTitle => Block?.GetPlacedBlockName(Api?.World, Pos);
        public override InventoryBase Inventory
        {
            get { return inventory; }
        }

        public override string InventoryClassName
        {
            get { return "alchemyfurnace"; }
        }

        public BlockEntityAlchemyFurnace()
        {
            inventory = new InventoryAlchemyFurnace(null, null);
            inventory.SlotModified += OnSlotModifid;
        }

        private void OnSlotModifid(int slotid)
        {
            return;
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            inventory.LateInitialize("alchemyfurnace-" + Pos.X + "/" + Pos.Y + "/" + Pos.Z, api);
            inventory.AfterBlocksLoaded(Api.World);
        }

        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            if (Api.Side == EnumAppSide.Client)
            {
                toggleInventoryDialogClient(byPlayer, () =>
                {
                    clientDialog = new GuiDialogAlchemyFurnace(DialogTitle, Inventory, Pos, Api as ICoreClientAPI);
                    return clientDialog;
                });
            }
            return true;
        }


        /*
        public bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            if (world.Api is ICoreClientAPI capi)
            {
                var dlg = new GuiDialogAlchemyFurnace(DialogTitle, Inventory, this.Pos, capi);
                dlg.TryOpen();
            }
            return true;
        }
        */

        public override void OnReceivedClientPacket(IPlayer player, int packetid, byte[] data)
        {
            base.OnReceivedClientPacket(player, packetid, data);
        }

        public override void OnReceivedServerPacket(int packetid, byte[] data)
        {
            base.OnReceivedServerPacket(packetid, data);

            if (packetid == (int)EnumBlockEntityPacketId.Close)
            {
                (Api.World as IClientWorldAccessor).Player.InventoryManager.CloseInventory(Inventory);
                invDialog?.TryClose();
                invDialog?.Dispose();
                invDialog = null;
            }
        }


        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            base.FromTreeAttributes(tree, worldAccessForResolve);
            inventory.FromTreeAttributes(tree.GetTreeAttribute("inventory"));
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            Api.Logger.Debug("Saving furnace!");
            ITreeAttribute invtree = new TreeAttribute();
            inventory.ToTreeAttributes(invtree);
            tree["inventory"] = invtree;

        }

    }
}
