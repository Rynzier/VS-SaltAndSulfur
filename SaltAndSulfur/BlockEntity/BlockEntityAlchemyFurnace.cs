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

        // Smelting variables

        // Half second tick and full tick temperatures
        public float prevFurnaceTemperature = 20;
        public float furnaceTemperature = 20;

        // Maximum temperature for current fuel
        public int maxTemperature;
        // How long the inputs have been cooking
        public float inputStackCookingTime;
        // How much of the current fuel is consumed
        public float fuelBurnTime;
        // How much fuel is available
        public float maxFuelBurnTime;
        public bool IsBurning;

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

            RegisterGameTickListener(OnBurnTick, 100);
        }

        private void OnBurnTick(float dt)
        {
            if (Api is ICoreClientAPI) return;

            if (fuelBurnTime > 0)
            {
                fuelBurnTime -= dt;

                if (fuelBurnTime <= 0)
                {
                    fuelBurnTime = 0;
                    maxFuelBurnTime = 0;
                }
            }

            if (IsBurning)
            {
                furnaceTemperature = changeTemperature(furnaceTemperature, maxTemperature, dt);
            }
        }

        public bool isInputValid()
        {
            return false;
        }

        public float changeTemperature(float fromTemp, float toTemp, float dt)
        {
            float diff = Math.Abs(fromTemp - toTemp);

            dt = dt + dt * (diff / 28);

            if (diff < dt)
            {
                return toTemp;
            }

            if (fromTemp > toTemp)
            {
                dt = -dt;
            }

            if (Math.Abs(fromTemp - toTemp) < 1)
            {
                return toTemp;
            }

            return fromTemp + dt;
        }

        public void heatInput(float dt)
        {
        }

        float GetTemp(int inputid)
        {
            ItemStack stack = inputSlots[inputid].Itemstack;
            if (stack == null) return 20;

            return stack.Collectible.GetTemperature(Api.World, stack);

        }

        void SetTemp(int inputid, float value)
        {
            ItemStack stack = inputSlots[inputid].Itemstack;
            if (stack == null) return;

            stack.Collectible.SetTemperature(Api.World, stack, value);
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
