// Decompiled with JetBrains decompiler
// Type: SphereEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using UnityEngine;

public class SphereEntity : BaseEntity
{
  public float currentRadius = 1f;
  public float lerpRadius = 1f;
  public float lerpSpeed = 1f;

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.sphereEntity = (__Null) Pool.Get<SphereEntity>();
    ((SphereEntity) info.msg.sphereEntity).radius = (__Null) (double) this.currentRadius;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (!this.isServer)
      return;
    if (info.msg.sphereEntity != null)
      this.currentRadius = this.lerpRadius = (float) ((SphereEntity) info.msg.sphereEntity).radius;
    this.UpdateScale();
  }

  public void LerpRadiusTo(float radius, float speed)
  {
    this.lerpRadius = radius;
    this.lerpSpeed = speed;
  }

  protected void UpdateScale()
  {
    ((Component) this).get_transform().set_localScale(new Vector3(this.currentRadius, this.currentRadius, this.currentRadius));
  }

  protected void Update()
  {
    if ((double) this.currentRadius == (double) this.lerpRadius || !this.isServer)
      return;
    this.currentRadius = Mathf.MoveTowards(this.currentRadius, this.lerpRadius, Time.get_deltaTime() * this.lerpSpeed);
    this.UpdateScale();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }
}
