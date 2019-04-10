// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ManualCoverPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust.Ai
{
  public class ManualCoverPoint : FacepunchBehaviour
  {
    public bool IsDynamic;
    public float Score;
    public CoverPointVolume Volume;
    public Vector3 Normal;
    public CoverPoint.CoverType NormalCoverType;

    public Vector3 Position
    {
      get
      {
        return ((Component) this).get_transform().get_position();
      }
    }

    public float DirectionMagnitude
    {
      get
      {
        if (Object.op_Inequality((Object) this.Volume, (Object) null))
          return this.Volume.CoverPointRayLength;
        return 1f;
      }
    }

    private void Awake()
    {
      if (!Object.op_Inequality((Object) ((Component) this).get_transform().get_parent(), (Object) null))
        return;
      this.Volume = (CoverPointVolume) ((Component) ((Component) this).get_transform().get_parent()).GetComponent<CoverPointVolume>();
    }

    public CoverPoint ToCoverPoint(CoverPointVolume volume)
    {
      this.Volume = volume;
      if (this.IsDynamic)
      {
        CoverPoint coverPoint = new CoverPoint(this.Volume, this.Score);
        coverPoint.IsDynamic = true;
        coverPoint.SourceTransform = ((Component) this).get_transform();
        coverPoint.NormalCoverType = this.NormalCoverType;
        Transform transform = ((Component) this).get_transform();
        coverPoint.Position = transform != null ? transform.get_position() : Vector3.get_zero();
        return coverPoint;
      }
      Vector3 vector3 = Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), this.Normal);
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      return new CoverPoint(this.Volume, this.Score)
      {
        IsDynamic = false,
        Position = ((Component) this).get_transform().get_position(),
        Normal = normalized,
        NormalCoverType = this.NormalCoverType
      };
    }

    public ManualCoverPoint()
    {
      base.\u002Ector();
    }
  }
}
