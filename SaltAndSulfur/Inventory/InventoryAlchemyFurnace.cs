using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;


namespace SaltAndSulfur
{
    public class InventoryAlchemyFurnace : InventoryBase, ISlotProvider
    {
        private ItemSlot[] slots;

        public ItemSlot[] Slots => slots;

        public override int Count => 5;

        public override ItemSlot this[int slotId]
        {
            get
            {
                if (slotId < 0 || slotId >= Count)
                {
                    return null;
                }

                return slots[slotId];
            }
            set
            {
                if (slotId < 0 || slotId >= Count)
                {
                    throw new ArgumentOutOfRangeException("slotId");
                }

                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                slots[slotId] = value;
            }
        }

        public InventoryAlchemyFurnace(string inventoryID, ICoreAPI api)
            : base(inventoryID, api)
        {
            slots = GenEmptySlots(5);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            slots = SlotsFromTreeAttributes(tree, slots);
        }

        public override void FromTreeAttributes(ITreeAttribute tree)
        {
            SlotsToTreeAttributes(slots, tree);
        }
    }
}
