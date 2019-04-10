// Decompiled with JetBrains decompiler
// Type: Spawnable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class Spawnable : MonoBehaviour, IServerComponent
{
  [ReadOnly]
  public SpawnPopulation Population;
  internal uint PrefabID;
  internal bool SpawnIndividual;
  internal Vector3 SpawnPosition;
  internal Quaternion SpawnRotation;

  protected void OnEnable()
  {
    if (Application.isLoadingSave != null)
      return;
    this.Add();
  }

  protected void OnDisable()
  {
    if (Application.isQuitting != null || Application.isLoadingSave != null)
      return;
    this.Remove();
  }

  private void Add()
  {
    this.SpawnPosition = ((Component) this).get_transform().get_position();
    this.SpawnRotation = ((Component) this).get_transform().get_rotation();
    if (!Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
      return;
    if ((BaseScriptableObject) this.Population != (BaseScriptableObject) null)
    {
      ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).AddInstance(this);
    }
    else
    {
      if (Application.isLoading == null || Application.isLoadingSave != null)
        return;
      BaseEntity component = (BaseEntity) ((Component) this).GetComponent<BaseEntity>();
      if (!Object.op_Inequality((Object) component, (Object) null) || !component.enableSaving || component.syncPosition)
        return;
      ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).AddRespawn(new global::SpawnIndividual(component.prefabID, this.SpawnPosition, this.SpawnRotation));
    }
  }

  private void Remove()
  {
    if (!Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance) || !((BaseScriptableObject) this.Population != (BaseScriptableObject) null))
      return;
    ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).RemoveInstance(this);
  }

  internal void Save(BaseNetworkable.SaveInfo info)
  {
    if ((BaseScriptableObject) this.Population == (BaseScriptableObject) null)
      return;
    info.msg.spawnable = (__Null) Pool.Get<Spawnable>();
    ((Spawnable) info.msg.spawnable).population = (__Null) (int) this.Population.FilenameStringId;
  }

  internal void Load(BaseNetworkable.LoadInfo info)
  {
    if (info.msg.spawnable != null)
      this.Population = FileSystem.Load<SpawnPopulation>(StringPool.Get((uint) ((Spawnable) info.msg.spawnable).population), true);
    this.Add();
  }

  protected void OnValidate()
  {
    this.Population = (SpawnPopulation) null;
  }

  public Spawnable()
  {
    base.\u002Ector();
  }
}
