using System;
using BepInEx;
using RoR2;
using UnityEngine;
using RoR2.CharacterAI;
using UnityEngine.Networking;

namespace Basil_ror2
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Basil.DronesInheritItems", "DronesInheritItems", "1.0.0")]
    public class DroneWithItems : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.SummonMasterBehavior.OpenSummon += (orig, self, activator) =>
            {
                if (!NetworkServer.active)
                {
                    Debug.LogWarning("[Server] function 'System.Void RoR2.SummonMasterBehavior::OpenSummon(RoR2.Interactor)' called on client");
                    return;
                }
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(self.masterPrefab, self.transform.position, self.transform.rotation);
                CharacterBody component = activator.GetComponent<CharacterBody>();
                CharacterMaster master = component.master;
                CharacterMaster component2 = gameObject.GetComponent<CharacterMaster>();
                component2.teamIndex = TeamComponent.GetObjectTeam(component.gameObject);
                Inventory component3 = gameObject.GetComponent<Inventory>();
                component3.CopyItemsFrom(master.inventory);
                component3.ResetItem(ItemIndex.WardOnLevel);
                component3.ResetItem(ItemIndex.BeetleGland);
                component3.ResetItem(ItemIndex.CrippleWardOnLevel);
                NetworkServer.Spawn(gameObject);
                component2.SpawnBody(component2.bodyPrefab, self.transform.position + Vector3.up * 0.8f, self.transform.rotation);
                AIOwnership component4 = gameObject.GetComponent<AIOwnership>();
                if (component4 && component && master)
                {
                    component4.ownerMaster = master;
                }
                BaseAI component5 = gameObject.GetComponent<BaseAI>();
                if (component5)
                {
                    component5.leader.gameObject = activator.gameObject;
                }
                UnityEngine.Object.Destroy(self.gameObject);
            };
        }
    }
}
