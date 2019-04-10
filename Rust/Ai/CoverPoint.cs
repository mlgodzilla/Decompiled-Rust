// Decompiled with JetBrains decompiler
// Type: Rust.Ai.CoverPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

namespace Rust.Ai
{
  public class CoverPoint
  {
    public CoverPoint.CoverType NormalCoverType;
    public bool IsDynamic;
    public Transform SourceTransform;
    private Vector3 _staticPosition;
    private Vector3 _staticNormal;

    public CoverPointVolume Volume { get; private set; }

    public Vector3 Position
    {
      get
      {
        if (this.IsDynamic && Object.op_Inequality((Object) this.SourceTransform, (Object) null))
          return this.SourceTransform.get_position();
        return this._staticPosition;
      }
      set
      {
        this._staticPosition = value;
      }
    }

    public Vector3 Normal
    {
      get
      {
        if (this.IsDynamic && Object.op_Inequality((Object) this.SourceTransform, (Object) null))
          return this.SourceTransform.get_forward();
        return this._staticNormal;
      }
      set
      {
        this._staticNormal = value;
      }
    }

    public BaseEntity ReservedFor { get; set; }

    public bool IsReserved
    {
      get
      {
        return Object.op_Inequality((Object) this.ReservedFor, (Object) null);
      }
    }

    public bool IsCompromised { get; set; }

    public float Score { get; set; }

    public bool IsValidFor(BaseEntity entity)
    {
      if (this.IsCompromised)
        return false;
      if (!Object.op_Equality((Object) this.ReservedFor, (Object) null))
        return Object.op_Equality((Object) this.ReservedFor, (Object) entity);
      return true;
    }

    public CoverPoint(CoverPointVolume volume, float score)
    {
      this.Volume = volume;
      this.Score = score;
    }

    public void CoverIsCompromised(float cooldown)
    {
      if (this.IsCompromised || !Object.op_Inequality((Object) this.Volume, (Object) null))
        return;
      this.Volume.StartCoroutine(this.StartCooldown(cooldown));
    }

    private IEnumerator StartCooldown(float cooldown)
    {
      this.IsCompromised = true;
      yield return (object) CoroutineEx.waitForSeconds(cooldown);
      this.IsCompromised = false;
    }

    public bool ProvidesCoverFromPoint(Vector3 point, float arcThreshold)
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.Position, point);
      return (double) Vector3.Dot(this.Normal, ((Vector3) ref vector3).get_normalized()) < (double) arcThreshold;
    }

    public enum CoverType
    {
      Full,
      Partial,
      None,
    }
  }
}
