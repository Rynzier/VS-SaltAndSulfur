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
        // Necessary variables
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
            Block = Api.World.BlockAccessor.GetBlock(Pos);

            if (Api is ICoreClientAPI && clientDialog != null)
            {
                return;
            }
            return;
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            inventory.LateInitialize("alchemyfurnace-" + Pos.X + "/" + Pos.Y + "/" + Pos.Z, api);
            inventory.AfterBlocksLoaded(Api.World);

            RegisterGameTickListener(UpdateFurnace, 100);
        }

        public void UpdateFurnace(float delta)
        {
            if (Api.Side == EnumAppSide.Server)
            {
                MarkDirty();
            }
            BEBehaviorAlchemySmelt smeltBehavior = this.GetBehavior<BEBehaviorAlchemySmelt>();
            smeltBehavior.UpdateHeat(inputSlots, fuelSlot, delta);
            float temp = smeltBehavior.FurnaceTemperature;
            float burnTime = smeltBehavior.BurnTimeLeft;
            float cookProg = smeltBehavior.SmeltProgress;
            if (Api.Side == EnumAppSide.Server)
            {
                Api.Logger.Debug("Temperature: {0} | Remaining Fuel: {1} | Cook Progress: {2}", [temp, burnTime, cookProg]);
            }
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

                BEBehaviorAlchemySmelt test = this.GetBehavior<BEBehaviorAlchemySmelt>();
                test.TestBehaviour();
            }

            return true;
        }

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

        public override void OnBlockRemoved()
        {
            base.OnBlockRemoved();
            UnregisterAllTickListeners();
        }

        public ItemSlot[] inputSlots
        {
            get { return new ItemSlot[] { inventory[0], inventory[1], inventory[2] }; }
        }

        public ItemSlot[] outputSlots
        {
            get { return new ItemSlot[] { inventory[3], inventory[4], inventory[5] }; }
        }

        public ItemSlot fuelSlot
        {
            get { return inventory[6]; }
        }

        public CombustibleProperties fuelCombustibleOpts
        {
            get { return getCombustibleOpts(6); }
        }
        public CombustibleProperties getCombustibleOpts(int slotid)
        {
            ItemSlot slot = inventory[slotid];
            if (slot.Itemstack == null) return null;
            return slot.Itemstack.Collectible.CombustibleProps;
        }
    }
}
