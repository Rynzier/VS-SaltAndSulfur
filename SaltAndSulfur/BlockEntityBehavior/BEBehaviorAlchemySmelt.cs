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

            if ((fuelBurnTime >= maxFuelBurnTime) || (!isBurning))
            {
                fuelBurnTime = 0;
                if (fuel.Itemstack != null)
                {
                    currCopts = fuel.Itemstack.Collectible.CombustibleProps;
                    maxFuelBurnTime = currCopts.BurnDuration;
                    maxTemperature = currCopts.BurnTemperature;

                    fuel.Itemstack.StackSize -= 1;
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
                    inputs[i].Itemstack.Collectible.SetTemperature(Api.World, inputs[i].Itemstack, furnaceTemperature);

                    float currentTemp = inputs[i].Itemstack.Collectible.GetTemperature(Api.World, inputs[i].Itemstack);
                    float targetTemp = inputs[i].Itemstack.Collectible.CombustibleProps.MeltingPoint;
                    if (currentTemp >= targetTemp)
                    {
                        atTemp[i] = true;
                    }
                    else
                    {
                        atTemp[i] = false;
                    }

                }

                if (atTemp[0] || atTemp[1] || atTemp[2])
                {
                    inputCookingTime += delta;
                }
            }
        }

        public void ChangeTemperature()
        {

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
