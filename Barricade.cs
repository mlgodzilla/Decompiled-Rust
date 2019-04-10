// Decompiled with JetBrains decompiler
// Type: Barricade
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Rust;
using UnityEngine;
using UnityEngine.AI;

public class Barricade : DecayEntity
{
  private static int nonWalkableArea = -1;
  private static int animalAgentTypeId = -1;
  private static int humanoidAgentTypeId = -1;
  public float reflectDamage = 5f;
  public bool canNpcSmash = true;
  public GameObjectRef reflectEffect;
  public NavMeshModifierVolume NavMeshVolumeAnimals;
  public NavMeshModifierVolume NavMeshVolumeHumanoids;
  public NPCBarricadeTriggerBox NpcTriggerBox;

  public override void ServerInit()
  {
    base.ServerInit();
    if (Barricade.nonWalkableArea < 0)
      Barricade.nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
    if (Barricade.animalAgentTypeId < 0)
    {
      NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
      Barricade.animalAgentTypeId = ((NavMeshBuildSettings) ref settingsByIndex).get_agentTypeID();
    }
    if (Object.op_Equality((Object) this.NavMeshVolumeAnimals, (Object) null))
    {
      this.NavMeshVolumeAnimals = (NavMeshModifierVolume) ((Component) this).get_gameObject().AddComponent<NavMeshModifierVolume>();
      this.NavMeshVolumeAnimals.set_area(Barricade.nonWalkableArea);
      this.NavMeshVolumeAnimals.AddAgentType(Barricade.animalAgentTypeId);
      this.NavMeshVolumeAnimals.set_center(Vector3.get_zero());
      this.NavMeshVolumeAnimals.set_size(Vector3.get_one());
    }
    if (!this.canNpcSmash)
    {
      if (Barricade.humanoidAgentTypeId < 0)
      {
        NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(0);
        Barricade.humanoidAgentTypeId = ((NavMeshBuildSettings) ref settingsByIndex).get_agentTypeID();
      }
      if (!Object.op_Equality((Object) this.NavMeshVolumeHumanoids, (Object) null))
        return;
      this.NavMeshVolumeHumanoids = (NavMeshModifierVolume) ((Component) this).get_gameObject().AddComponent<NavMeshModifierVolume>();
      this.NavMeshVolumeHumanoids.set_area(Barricade.nonWalkableArea);
      this.NavMeshVolumeHumanoids.AddAgentType(Barricade.humanoidAgentTypeId);
      this.NavMeshVolumeHumanoids.set_center(Vector3.get_zero());
      this.NavMeshVolumeHumanoids.set_size(Vector3.get_one());
    }
    else
    {
      if (!Object.op_Equality((Object) this.NpcTriggerBox, (Object) null))
        return;
      this.NpcTriggerBox = (NPCBarricadeTriggerBox) new GameObject("NpcTriggerBox").AddComponent<NPCBarricadeTriggerBox>();
      this.NpcTriggerBox.Setup(this);
    }
  }

  public override void OnAttacked(HitInfo info)
  {
    if (this.isServer && info.WeaponPrefab is BaseMelee && !info.IsProjectile())
    {
      BasePlayer initiator = info.Initiator as BasePlayer;
      if (Object.op_Implicit((Object) initiator) && (double) this.reflectDamage > 0.0)
      {
        initiator.Hurt(this.reflectDamage * Random.Range(0.75f, 1.25f), DamageType.Stab, (BaseEntity) this, true);
        if (this.reflectEffect.isValid)
          Effect.server.Run(this.reflectEffect.resourcePath, (BaseEntity) initiator, StringPool.closest, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
      }
    }
    base.OnAttacked(info);
  }
}
