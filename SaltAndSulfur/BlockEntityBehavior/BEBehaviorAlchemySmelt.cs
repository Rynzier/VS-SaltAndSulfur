using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace SaltAndSulfur
{
    public class BEBehaviorAlchemySmelt : BlockEntityBehavior
    {
        public float furnaceTemperature = 20;
        public int maxTemperature;
        public float inputCookingTime = 0;
        public float fuelBurnTime = 0;
        public float maxFuelBurnTime = 0;
        public bool isBurning = false;

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

        public void TestBehaviour()
        {
            Api.Logger.Debug("Behavior is working :3");
        }
    }
}
