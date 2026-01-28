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

        ClearComposers();
        SingleComposer = capi.Gui
            .CreateCompo("dialog-title", dialogBounds)
            .AddShadedDialogBG(bgBounds)
            .AddDialogTitleBar(DialogTitle, OnTitleBarClose)
            .BeginChildElements(bgBounds)
            .EndChildElements()
            .Compose()
            ;

        lastRedrawMs = capi.ElapsedMilliseconds;

        if (hoveredSlot != null)
        {
            SingleComposer.OnMouseMove(new MouseEvent(capi.Input.MouseX, capi.Input.MouseY));
        }
    }

    private void OnTitleBarClose()
    {
        TryClose();
    }
}
