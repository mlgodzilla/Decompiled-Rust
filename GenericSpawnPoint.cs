// Decompiled with JetBrains decompiler
// Type: GenericSpawnPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;
using UnityEngine.Events;

public class GenericSpawnPoint : BaseSpawnPoint
{
  public bool dropToGround = true;
  public UnityEvent OnObjectSpawnedEvent = new UnityEvent();
  public UnityEvent OnObjectRetiredEvent = new UnityEvent();
  public bool randomRot;
  public GameObjectRef spawnEffect;

  public override void GetLocation(out Vector3 pos, out Quaternion rot)
  {
    pos = ((Component) this).get_transform().get_position();
    rot = !this.randomRot ? ((Component) this).get_transform().get_rotation() : Quaternion.Euler(0.0f, Random.Range(0.0f, 360f), 0.0f);
    if (!this.dropToGround)
      return;
    this.DropToGround(ref pos, ref rot);
  }

  public override void ObjectSpawned(SpawnPointInstance instance)
  {
    if (this.spawnEffect.isValid)
      Effect.server.Run(this.spawnEffect.resourcePath, (BaseEntity) ((Component) instance).GetComponent<BaseEntity>(), 0U, Vector3.get_zero(), Vector3.get_up(), (Connection) null, false);
    this.OnObjectSpawnedEvent.Invoke();
    ((Component) this).get_gameObject().SetActive(false);
  }

  public override void ObjectRetired(SpawnPointInstance instance)
  {
    this.OnObjectRetiredEvent.Invoke();
    ((Component) this).get_gameObject().SetActive(true);
  }
}
