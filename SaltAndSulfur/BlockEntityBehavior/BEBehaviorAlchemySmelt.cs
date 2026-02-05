using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace SaltAndSulfur
{
    public class BEBehaviorAlchemySmelt : BlockEntityBehavior
    {
        private float furnaceTemperature = 20;
        private int maxTemperature;
        private float inputCookingTime = 0;
        private float fuelBurnTime = 0;
        private float maxFuelBurnTime = 0;
        private bool isBurning = false;

        public BEBehaviorAlchemySmelt(BlockEntity be) : base(be) { }

        bool registered = false;
        public override void Initialize(ICoreAPI api, JsonObject properties)
        {
            base.Initialize(api, properties);

            if (Api.Side == EnumAppSide.Server && !registered)
            {
                // Insert event attachments
                registered = true;
            }
        }

        public void UpdateHeat(ItemSlot[] inputs, ItemSlot fuel, float delta)
        {
            CombustibleProperties currCopts;
            bool[] atTemp = [false, false, false];
            float meanTemp = 0;
            float highTarget = 0;

            if ((fuelBurnTime >= maxFuelBurnTime) || (!isBurning))
            {
                fuelBurnTime = 0;
                if (fuel.Itemstack != null)
                {
                    currCopts = fuel.Itemstack.Collectible.CombustibleProps;
                    maxFuelBurnTime = currCopts.BurnDuration;
                    maxTemperature = currCopts.BurnTemperature;

                    fuel.TakeOut(1);
                    // fuel.Itemstack.StackSize -= 1;
                    if (fuel.Itemstack.StackSize <= 0)
                    {
                        fuel.Itemstack = null;
                    }

                    isBurning = true;
                }
                else
                {
                    isBurning = false;
                }
            }
            else if ((fuelBurnTime < maxFuelBurnTime) && isBurning)
            {
                fuelBurnTime += delta;
                furnaceTemperature = maxTemperature;
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i].Itemstack == null || inputs[i].Itemstack.Collectible.CombustibleProps == null) continue;
                    HeatToFurnaceTemp(inputs[i], delta);

                    float currentTemp = inputs[i].Itemstack.Collectible.GetTemperature(Api.World, inputs[i].Itemstack);
                    float targetTemp = inputs[i].Itemstack.Collectible.CombustibleProps.MeltingPoint;
                    if (highTarget < targetTemp) highTarget = targetTemp;

                    if (currentTemp >= targetTemp)
                    {
                        atTemp[i] = true;
                    }
                    else
                    {
                        atTemp[i] = false;
                    }
                    meanTemp += currentTemp;
                }
                meanTemp /= inputs.Length;

                if (atTemp[0] || atTemp[1] || atTemp[2])
                {
                    float diff = meanTemp / highTarget;
                    inputCookingTime += Math.Clamp((int)(diff), 1, 30) * delta;
                }
                else
                {
                    if (inputCookingTime > 0) inputCookingTime--;
                }
            }
        }

        public void HeatToFurnaceTemp(ItemSlot tarslot, float delta)
        {
            if (tarslot.Itemstack == null || tarslot.Itemstack.Collectible.CombustibleProps == null) return;

            ItemStack targetStack = tarslot.Itemstack;
            float oldTemp = targetStack.Collectible.GetTemperature(Api.World, targetStack);
            float nowTemp = oldTemp;
            float meltingPoint = targetStack.Collectible.CombustibleProps.MeltingPoint;

            if (oldTemp < furnaceTemperature)
            {
                float f = (1 + Math.Clamp((furnaceTemperature - oldTemp) / 30, 0, 1.6f)) * delta;
                if (nowTemp >= meltingPoint) f /= 11;

                float newTemp = changeTemperature(oldTemp, furnaceTemperature, f);
                int maxTemp = targetStack.Collectible.CombustibleProps.MaxTemperature;

                if (maxTemp > 0) newTemp = Math.Min(maxTemp, newTemp);
                if (oldTemp != newTemp)
                {
                    targetStack.Collectible.SetTemperature(Api.World, targetStack, newTemp);
                    nowTemp = newTemp;
                }
            }
        }

        public float changeTemperature(float fromTemp, float toTemp, float delta)
        {
            float diff = Math.Abs(fromTemp - toTemp);

            delta = delta + delta * (diff / 28);

            if (diff < delta) return toTemp;
            if (fromTemp > toTemp) delta = -delta;
            if (Math.Abs(fromTemp - toTemp) < 1) return toTemp;

            return fromTemp + delta;
        }

        public void TestBehaviour()
        {
            Api.Logger.Debug("Behavior is working :3");
        }

        public float FurnaceTemperature
        {
            get { return furnaceTemperature; }
        }

        public float BurnTimeLeft
        {
            get { return (fuelBurnTime / maxFuelBurnTime); }
        }

        public float SmeltProgress
        {
            get { return inputCookingTime; }
        }

    }
}
