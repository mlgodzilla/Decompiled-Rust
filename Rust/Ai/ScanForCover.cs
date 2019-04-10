// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ScanForCover
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  [FriendlyName("Scan for Cover", "Scanning for cover volumes and the cover points within the relevant ones.")]
  public sealed class ScanForCover : BaseAction
  {
    [ApexSerialization]
    public float MaxDistanceToCover = 15f;
    [ApexSerialization]
    public float CoverArcThreshold = -0.75f;

    public override void DoExecute(BaseContext ctx)
    {
      if (Object.op_Equality((Object) SingletonComponent<AiManager>.Instance, (Object) null) || !((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled() || (!((AiManager) SingletonComponent<AiManager>.Instance).UseCover || Object.op_Equality((Object) ctx.AIAgent.AttackTarget, (Object) null)))
        return;
      NPCHumanContext npcHumanContext = ctx as NPCHumanContext;
      if (npcHumanContext == null)
        return;
      if (npcHumanContext.sampledCoverPoints.Count > 0)
      {
        npcHumanContext.sampledCoverPoints.Clear();
        npcHumanContext.sampledCoverPointTypes.Clear();
      }
      if (!(npcHumanContext.AIAgent.AttackTarget is BasePlayer))
        return;
      if (Object.op_Equality((Object) npcHumanContext.CurrentCoverVolume, (Object) null) || !npcHumanContext.CurrentCoverVolume.Contains(npcHumanContext.Position))
      {
        npcHumanContext.CurrentCoverVolume = ((AiManager) SingletonComponent<AiManager>.Instance).GetCoverVolumeContaining(npcHumanContext.Position);
        if (Object.op_Equality((Object) npcHumanContext.CurrentCoverVolume, (Object) null))
          npcHumanContext.CurrentCoverVolume = AiManager.CreateNewCoverVolume(npcHumanContext.Position, (Transform) null);
      }
      if (!Object.op_Inequality((Object) npcHumanContext.CurrentCoverVolume, (Object) null))
        return;
      foreach (CoverPoint coverPoint in npcHumanContext.CurrentCoverVolume.CoverPoints)
      {
        if (!coverPoint.IsReserved)
        {
          Vector3 position = coverPoint.Position;
          Vector3 vector3_1 = Vector3.op_Subtraction(npcHumanContext.Position, position);
          if ((double) ((Vector3) ref vector3_1).get_sqrMagnitude() <= (double) this.MaxDistanceToCover)
          {
            Vector3 vector3_2 = Vector3.op_Subtraction(position, npcHumanContext.AIAgent.AttackTargetMemory.Position);
            Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
            if (ScanForCover.ProvidesCoverFromDirection(coverPoint, normalized, this.CoverArcThreshold))
            {
              npcHumanContext.sampledCoverPointTypes.Add(coverPoint.NormalCoverType);
              npcHumanContext.sampledCoverPoints.Add(coverPoint);
            }
          }
        }
      }
    }

    public static bool ProvidesCoverFromDirection(
      CoverPoint cp,
      Vector3 directionTowardCover,
      float arcThreshold)
    {
      return (double) Vector3.Dot(cp.Normal, directionTowardCover) < (double) arcThreshold;
    }
  }
}
