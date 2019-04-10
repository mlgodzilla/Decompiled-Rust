// Decompiled with JetBrains decompiler
// Type: MaterialEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/MaterialEffect")]
public class MaterialEffect : ScriptableObject
{
  public GameObjectRef DefaultEffect;
  public MaterialEffect.Entry[] Entries;

  public void SpawnOnRay(Ray ray, int mask, float length = 0.5f, Vector3 forward = null)
  {
    RaycastHit hitInfo;
    if (!GamePhysics.Trace(ray, 0.0f, out hitInfo, length, mask, (QueryTriggerInteraction) 0))
      Effect.client.Run(this.DefaultEffect.resourcePath, ((Ray) ref ray).get_origin(), Vector3.op_Multiply(((Ray) ref ray).get_direction(), -1f), forward);
    else
      Effect.client.Run(this.DefaultEffect.resourcePath, ((RaycastHit) ref hitInfo).get_point(), ((RaycastHit) ref hitInfo).get_normal(), forward);
  }

  public MaterialEffect()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class Entry
  {
    public PhysicMaterial Material;
    public GameObjectRef Effect;
  }
}
