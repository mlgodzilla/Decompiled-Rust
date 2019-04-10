// Decompiled with JetBrains decompiler
// Type: GameManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
  public static GameManager server = new GameManager(false, true);
  internal PrefabPreProcess preProcessed;
  internal PrefabPoolCollection pool;
  private bool Clientside;
  private bool Serverside;

  public void Reset()
  {
    this.pool.Clear();
  }

  public GameManager(bool clientside, bool serverside)
  {
    this.Clientside = clientside;
    this.Serverside = serverside;
    this.preProcessed = new PrefabPreProcess(clientside, serverside, false);
    this.pool = new PrefabPoolCollection();
  }

  public GameObject FindPrefab(uint prefabID)
  {
    string strPrefab = StringPool.Get(prefabID);
    if (string.IsNullOrEmpty(strPrefab))
      return (GameObject) null;
    return this.FindPrefab(strPrefab);
  }

  public GameObject FindPrefab(BaseEntity ent)
  {
    if (Object.op_Equality((Object) ent, (Object) null))
      return (GameObject) null;
    return this.FindPrefab(ent.PrefabName);
  }

  public GameObject FindPrefab(string strPrefab)
  {
    GameObject gameObject = this.preProcessed.Find(strPrefab);
    if (Object.op_Inequality((Object) gameObject, (Object) null))
      return gameObject;
    GameObject go = FileSystem.LoadPrefab(strPrefab);
    if (Object.op_Equality((Object) go, (Object) null))
      return (GameObject) null;
    this.preProcessed.Process(strPrefab, go);
    return this.preProcessed.Find(strPrefab);
  }

  public GameObject CreatePrefab(
    string strPrefab,
    Vector3 pos,
    Quaternion rot,
    Vector3 scale,
    bool active = true)
  {
    GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
    if (Object.op_Implicit((Object) gameObject))
    {
      gameObject.get_transform().set_localScale(scale);
      if (active)
        gameObject.AwakeFromInstantiate();
    }
    return gameObject;
  }

  public GameObject CreatePrefab(
    string strPrefab,
    Vector3 pos,
    Quaternion rot,
    bool active = true)
  {
    GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
    if (Object.op_Implicit((Object) gameObject) && active)
      gameObject.AwakeFromInstantiate();
    return gameObject;
  }

  public GameObject CreatePrefab(string strPrefab, bool active = true)
  {
    GameObject gameObject = this.Instantiate(strPrefab, Vector3.get_zero(), Quaternion.get_identity());
    if (Object.op_Implicit((Object) gameObject) && active)
      gameObject.AwakeFromInstantiate();
    return gameObject;
  }

  public GameObject CreatePrefab(string strPrefab, Transform parent, bool active = true)
  {
    GameObject gameObject = this.Instantiate(strPrefab, parent.get_position(), parent.get_rotation());
    if (Object.op_Implicit((Object) gameObject))
    {
      gameObject.get_transform().SetParent(parent, false);
      gameObject.Identity();
      if (active)
        gameObject.AwakeFromInstantiate();
    }
    return gameObject;
  }

  public BaseEntity CreateEntity(
    string strPrefab,
    Vector3 pos = null,
    Quaternion rot = null,
    bool startActive = true)
  {
    if (string.IsNullOrEmpty(strPrefab))
      return (BaseEntity) null;
    GameObject prefab = this.CreatePrefab(strPrefab, pos, rot, startActive);
    if (Object.op_Equality((Object) prefab, (Object) null))
      return (BaseEntity) null;
    BaseEntity component = (BaseEntity) prefab.GetComponent<BaseEntity>();
    if (Object.op_Implicit((Object) component))
      return component;
    Debug.LogError((object) ("CreateEntity called on a prefab that isn't an entity! " + strPrefab));
    Object.Destroy((Object) prefab);
    return (BaseEntity) null;
  }

  private GameObject Instantiate(string strPrefab, Vector3 pos, Quaternion rot)
  {
    if (!StringEx.IsLower(strPrefab))
    {
      Debug.LogWarning((object) ("Converting prefab name to lowercase: " + strPrefab));
      strPrefab = strPrefab.ToLower();
    }
    GameObject prefab = this.FindPrefab(strPrefab);
    if (!Object.op_Implicit((Object) prefab))
    {
      Debug.LogError((object) ("Couldn't find prefab \"" + strPrefab + "\""));
      return (GameObject) null;
    }
    GameObject gameObject = this.pool.Pop(StringPool.Get(strPrefab), pos, rot);
    if (Object.op_Equality((Object) gameObject, (Object) null))
    {
      gameObject = Instantiate.GameObject(prefab, pos, rot);
      ((Object) gameObject).set_name(strPrefab);
    }
    else
      gameObject.get_transform().set_localScale(prefab.get_transform().get_localScale());
    if (!this.Clientside && this.Serverside && Object.op_Equality((Object) gameObject.get_transform().get_parent(), (Object) null))
      SceneManager.MoveGameObjectToScene(gameObject, Rust.Server.EntityScene);
    return gameObject;
  }

  public static void Destroy(Component component, float delay = 0.0f)
  {
    if ((component as BaseEntity).IsValid())
      Debug.LogError((object) ("Trying to destroy an entity without killing it first: " + ((Object) component).get_name()));
    Object.Destroy((Object) component, delay);
  }

  public static void Destroy(GameObject instance, float delay = 0.0f)
  {
    if (!Object.op_Implicit((Object) instance))
      return;
    if (((BaseEntity) instance.GetComponent<BaseEntity>()).IsValid())
      Debug.LogError((object) ("Trying to destroy an entity without killing it first: " + ((Object) instance).get_name()));
    Object.Destroy((Object) instance, delay);
  }

  public static void DestroyImmediate(Component component, bool allowDestroyingAssets = false)
  {
    if ((component as BaseEntity).IsValid())
      Debug.LogError((object) ("Trying to destroy an entity without killing it first: " + ((Object) component).get_name()));
    Object.DestroyImmediate((Object) component, allowDestroyingAssets);
  }

  public static void DestroyImmediate(GameObject instance, bool allowDestroyingAssets = false)
  {
    if (((BaseEntity) instance.GetComponent<BaseEntity>()).IsValid())
      Debug.LogError((object) ("Trying to destroy an entity without killing it first: " + ((Object) instance).get_name()));
    Object.DestroyImmediate((Object) instance, allowDestroyingAssets);
  }

  public void Retire(GameObject instance)
  {
    if (!Object.op_Implicit((Object) instance))
      return;
    using (TimeWarning.New("GameManager.Retire", 0.1f))
    {
      if (((BaseEntity) instance.GetComponent<BaseEntity>()).IsValid())
        Debug.LogError((object) ("Trying to retire an entity without killing it first: " + ((Object) instance).get_name()));
      if (Application.isQuitting == null && Pool.enabled && instance.SupportsPooling())
        this.pool.Push(instance);
      else
        Object.Destroy((Object) instance);
    }
  }
}
