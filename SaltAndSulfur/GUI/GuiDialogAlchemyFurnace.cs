using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;


namespace SaltAndSulfur;

public class GuiDialogAlchemyFurnace : GuiDialogBlockEntity
{
    long lastRedrawMs;

    public GuiDialogAlchemyFurnace(string dialogTitle, InventoryBase inventory, BlockPos blockEntityPos, ICoreClientAPI capi) : base(dialogTitle, inventory, blockEntityPos, capi)
    {
        if (IsDuplicate) return;
        capi.World.Player.InventoryManager.OpenInventory(inventory);
        SetupDialog();
    }

    private void OnInventorySlotModified(int slotid)
    {
        capi.Event.EnqueueMainThreadTask(SetupDialog, "setupalcfurnacedlg");
    }

    void SetupDialog()
    {
        ItemSlot hoveredSlot = capi.World.Player.InventoryManager.CurrentHoveredSlot;
        if (hoveredSlot != null && (hoveredSlot.Inventory == Inventory))
        {
            capi.Input.TriggerOnMouseLeaveSlot(hoveredSlot);
        }
        else
        {
            hoveredSlot = null;
        }

        ElementBounds furnaceBounds = ElementBounds.Fixed(0, 0, 200, 90);

        ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
        bgBounds.BothSizing = ElementSizing.FitToChildren;
        bgBounds.WithChildren(furnaceBounds);

        ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.RightMiddle)
            .WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, 0);

        ElementBounds inputSlotBounds1 = ElementStdBounds.SlotGrid(EnumDialogArea.None, 0, 30, 1, 1);
        ElementBounds inputSlotBounds2 = ElementStdBounds.SlotGrid(EnumDialogArea.None, 50, 30, 1, 1);

        ClearComposers();
        SingleComposer = capi.Gui
            .CreateCompo("dialog-title", dialogBounds)
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar(DialogTitle, OnTitleBarClose)
            .BeginChildElements(bgBounds)
                .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 0 }, inputSlotBounds1, "inputSlot1")
                .AddItemSlotGrid(Inventory, SendInvPacket, 1, new int[] { 1 }, inputSlotBounds2, "inputSlot2")
            .EndChildElements()
            .Compose()
            ;

        lastRedrawMs = capi.ElapsedMilliseconds;

        if (hoveredSlot != null)
        {
            SingleComposer.OnMouseMove(new MouseEvent(capi.Input.MouseX, capi.Input.MouseY));
        }
    }

    private void SendInvPacket(object p)
    {
        capi.Network.SendBlockEntityPacket(BlockEntityPosition.X, BlockEntityPosition.Y, BlockEntityPosition.Z, p);
    }

    private void OnTitleBarClose()
    {
        TryClose();
    }

    public override void OnGuiOpened()
    {
        base.OnGuiOpened();
        Inventory.SlotModified += OnInventorySlotModified;
    }

    public override void OnGuiClosed()
    {
        Inventory.SlotModified -= OnInventorySlotModified;


        base.OnGuiClosed();
    }
}
