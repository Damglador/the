﻿using RoR2;
using System.Collections.Generic;

namespace HenryMod.Modules.Characters {
    public abstract class ItemDisplaysBase {

        public void SetItemDIsplays(ItemDisplayRuleSet itemDisplayRuleSet) {

            List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            SetItemDisplayRules(itemDisplayRules);

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            itemDisplayRuleSet.GenerateRuntimeValues();
        }

        protected abstract void SetItemDisplayRules(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules);
    }
}