// Decompiled with JetBrains decompiler
// Type: BaseSpawnPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public abstract class BaseSpawnPoint : MonoBehaviour, IServerComponent
{
  public abstract void GetLocation(out Vector3 pos, out Quaternion rot);

  public abstract void ObjectSpawned(SpawnPointInstance instance);

  public abstract void ObjectRetired(SpawnPointInstance instance);

  protected void DropToGround(ref Vector3 pos, ref Quaternion rot)
  {
    if (Object.op_Implicit((Object) TerrainMeta.HeightMap) && Object.op_Implicit((Object) TerrainMeta.Collision) && !TerrainMeta.Collision.GetIgnore(pos, 0.01f))
    {
      float height = TerrainMeta.HeightMap.GetHeight(pos);
      pos.y = (__Null) (double) Mathf.Max((float) pos.y, height);
    }
    RaycastHit hitOut;
    if (!TransformUtil.GetGroundInfo(pos, out hitOut, 20f, LayerMask.op_Implicit(1235288065), (Transform) null))
      return;
    pos = ((RaycastHit) ref hitOut).get_point();
    rot = Quaternion.LookRotation(Quaternion.op_Multiply(rot, Vector3.get_forward()), ((RaycastHit) ref hitOut).get_normal());
  }

  protected BaseSpawnPoint()
  {
    base.\u002Ector();
  }
}
