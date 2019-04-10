// Decompiled with JetBrains decompiler
// Type: RadialSpawnPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RadialSpawnPoint : BaseSpawnPoint
{
  public float radius = 10f;

  public override void GetLocation(out Vector3 pos, out Quaternion rot)
  {
    Vector2 vector2 = Vector2.op_Multiply(Random.get_insideUnitCircle(), this.radius);
    pos = Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3((float) vector2.x, 0.0f, (float) vector2.y));
    rot = Quaternion.Euler(0.0f, Random.Range(0.0f, 360f), 0.0f);
    this.DropToGround(ref pos, ref rot);
  }

  public override void ObjectSpawned(SpawnPointInstance instance)
  {
  }

  public override void ObjectRetired(SpawnPointInstance instance)
  {
  }
}
