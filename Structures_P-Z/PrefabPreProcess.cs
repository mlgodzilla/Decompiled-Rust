// Decompiled with JetBrains decompiler
// Type: PrefabPreProcess
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Rust.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using VLB;

public class PrefabPreProcess : IPrefabProcessor
{
  public static System.Type[] clientsideOnlyTypes = new System.Type[35]
  {
    typeof (IClientComponent),
    typeof (ImageEffectLayer),
    typeof (NGSS_Directional),
    typeof (VolumetricDustParticles),
    typeof (VolumetricLightBeam),
    typeof (Cloth),
    typeof (MeshFilter),
    typeof (Renderer),
    typeof (AudioLowPassFilter),
    typeof (AudioSource),
    typeof (AudioListener),
    typeof (ParticleSystemRenderer),
    typeof (ParticleSystem),
    typeof (ParticleEmitFromParentObject),
    typeof (Light),
    typeof (LODGroup),
    typeof (Animator),
    typeof (AnimationEvents),
    typeof (PlayerVoiceSpeaker),
    typeof (PlayerVoiceRecorder),
    typeof (ParticleScaler),
    typeof (PostEffectsBase),
    typeof (TOD_ImageEffect),
    typeof (TOD_Scattering),
    typeof (TOD_Rays),
    typeof (Tree),
    typeof (Projector),
    typeof (HttpImage),
    typeof (EventTrigger),
    typeof (StandaloneInputModule),
    typeof (UIBehaviour),
    typeof (Canvas),
    typeof (CanvasRenderer),
    typeof (CanvasGroup),
    typeof (GraphicRaycaster)
  };
  public static System.Type[] serversideOnlyTypes = new System.Type[2]
  {
    typeof (IServerComponent),
    typeof (NavMeshObstacle)
  };
  internal Dictionary<string, GameObject> prefabList = new Dictionary<string, GameObject>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  private List<Component> destroyList = new List<Component>();
  private List<GameObject> cleanupList = new List<GameObject>();
  public bool isClientside;
  public bool isServerside;
  public bool isBundling;

  public PrefabPreProcess(bool clientside, bool serverside, bool bundling = false)
  {
    this.isClientside = clientside;
    this.isServerside = serverside;
    this.isBundling = bundling;
  }

  public GameObject Find(string strPrefab)
  {
    GameObject gameObject;
    if (!this.prefabList.TryGetValue(strPrefab, out gameObject))
      return (GameObject) null;
    if (!Object.op_Equality((Object) gameObject, (Object) null))
      return gameObject;
    this.prefabList.Remove(strPrefab);
    return (GameObject) null;
  }

  public bool NeedsProcessing(GameObject go)
  {
    return !go.CompareTag("NoPreProcessing") && (PrefabPreProcess.HasComponents<IPrefabPreProcess>(go.get_transform()) || PrefabPreProcess.HasComponents<IPrefabPostProcess>(go.get_transform()) || PrefabPreProcess.HasComponents<IEditorComponent>(go.get_transform()) || !this.isClientside && (((IEnumerable<System.Type>) PrefabPreProcess.clientsideOnlyTypes).Any<System.Type>((Func<System.Type, bool>) (type => PrefabPreProcess.HasComponents(go.get_transform(), type))) || PrefabPreProcess.HasComponents<IClientComponentEx>(go.get_transform())) || !this.isServerside && (((IEnumerable<System.Type>) PrefabPreProcess.serversideOnlyTypes).Any<System.Type>((Func<System.Type, bool>) (type => PrefabPreProcess.HasComponents(go.get_transform(), type))) || PrefabPreProcess.HasComponents<IServerComponentEx>(go.get_transform())));
  }

  public void ProcessObject(string name, GameObject go, bool resetLocalTransform = true)
  {
    if (!this.isClientside)
    {
      foreach (System.Type clientsideOnlyType in PrefabPreProcess.clientsideOnlyTypes)
        this.DestroyComponents(clientsideOnlyType, go, this.isClientside, this.isServerside);
      foreach (IClientComponentEx component in PrefabPreProcess.FindComponents<IClientComponentEx>(go.get_transform()))
        component.PreClientComponentCull((IPrefabProcessor) this);
    }
    if (!this.isServerside)
    {
      foreach (System.Type serversideOnlyType in PrefabPreProcess.serversideOnlyTypes)
        this.DestroyComponents(serversideOnlyType, go, this.isClientside, this.isServerside);
      foreach (IServerComponentEx component in PrefabPreProcess.FindComponents<IServerComponentEx>(go.get_transform()))
        component.PreServerComponentCull((IPrefabProcessor) this);
    }
    this.DestroyComponents(typeof (IEditorComponent), go, this.isClientside, this.isServerside);
    if (resetLocalTransform)
    {
      go.get_transform().set_localPosition(Vector3.get_zero());
      go.get_transform().set_localRotation(Quaternion.get_identity());
    }
    List<Transform> components = PrefabPreProcess.FindComponents<Transform>(go.get_transform());
    components.Reverse();
    foreach (IPrefabPreProcess component in PrefabPreProcess.FindComponents<IPrefabPreProcess>(go.get_transform()))
      component.PreProcess((IPrefabProcessor) this, go, name, this.isServerside, this.isClientside, this.isBundling);
    using (List<Transform>.Enumerator enumerator = components.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        Transform current = enumerator.Current;
        if (Object.op_Implicit((Object) current) && Object.op_Implicit((Object) ((Component) current).get_gameObject()))
        {
          if (this.isServerside && ((Component) current).get_gameObject().CompareTag("Server Cull"))
          {
            this.RemoveComponents(((Component) current).get_gameObject());
            this.NominateForDeletion(((Component) current).get_gameObject());
          }
          if (this.isClientside && ((((Component) current).get_gameObject().CompareTag("Client Cull") ? 1 : 0) | (!Object.op_Inequality((Object) current, (Object) go.get_transform()) ? (false ? 1 : 0) : (Object.op_Inequality((Object) ((Component) current).get_gameObject().GetComponent<BaseEntity>(), (Object) null) ? 1 : 0))) != 0)
          {
            this.RemoveComponents(((Component) current).get_gameObject());
            this.NominateForDeletion(((Component) current).get_gameObject());
          }
        }
      }
    }
    this.RunCleanupQueue();
    foreach (IPrefabPostProcess component in PrefabPreProcess.FindComponents<IPrefabPostProcess>(go.get_transform()))
      component.PostProcess((IPrefabProcessor) this, go, name, this.isServerside, this.isClientside, this.isBundling);
  }

  public void Process(string name, GameObject go)
  {
    if (go.CompareTag("NoPreProcessing"))
      return;
    GameObject hierarchyGroup = this.GetHierarchyGroup();
    GameObject gameObject = go;
    go = Instantiate.GameObject(gameObject, hierarchyGroup.get_transform());
    ((Object) go).set_name(((Object) gameObject).get_name());
    if (this.NeedsProcessing(go))
      this.ProcessObject(name, go, true);
    this.AddPrefab(name, go);
  }

  public GameObject GetHierarchyGroup()
  {
    if (this.isClientside && this.isServerside)
      return HierarchyUtil.GetRoot("PrefabPreProcess - Generic", false, true);
    if (this.isServerside)
      return HierarchyUtil.GetRoot("PrefabPreProcess - Server", false, true);
    return HierarchyUtil.GetRoot("PrefabPreProcess - Client", false, true);
  }

  public void AddPrefab(string name, GameObject go)
  {
    go.SetActive(false);
    this.prefabList.Add(name, go);
  }

  private void DestroyComponents(System.Type t, GameObject go, bool client, bool server)
  {
    List<Component> list = new List<Component>();
    PrefabPreProcess.FindComponents(go.get_transform(), list, t);
    list.Reverse();
    using (List<Component>.Enumerator enumerator = list.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        Component current = enumerator.Current;
        RealmedRemove component = (RealmedRemove) current.GetComponent<RealmedRemove>();
        if (!Object.op_Inequality((Object) component, (Object) null) || component.ShouldDelete(current, client, server))
        {
          if (!current.get_gameObject().CompareTag("persist"))
            this.NominateForDeletion(current.get_gameObject());
          Object.DestroyImmediate((Object) current, true);
        }
      }
    }
  }

  private static bool ShouldExclude(Transform transform)
  {
    return Object.op_Inequality((Object) ((Component) transform).GetComponent<BaseEntity>(), (Object) null);
  }

  private static bool HasComponents<T>(Transform transform)
  {
    if ((object) ((Component) transform).GetComponent<T>() != null)
      return true;
    IEnumerator enumerator = transform.GetEnumerator();
    try
    {
      while (enumerator.MoveNext())
      {
        Transform current = (Transform) enumerator.Current;
        if (!PrefabPreProcess.ShouldExclude(current) && PrefabPreProcess.HasComponents<T>(current))
          return true;
      }
    }
    finally
    {
      (enumerator as IDisposable)?.Dispose();
    }
    return false;
  }

  private static bool HasComponents(Transform transform, System.Type t)
  {
    if (Object.op_Inequality((Object) ((Component) transform).GetComponent(t), (Object) null))
      return true;
    IEnumerator enumerator = transform.GetEnumerator();
    try
    {
      while (enumerator.MoveNext())
      {
        Transform current = (Transform) enumerator.Current;
        if (!PrefabPreProcess.ShouldExclude(current) && PrefabPreProcess.HasComponents(current, t))
          return true;
      }
    }
    finally
    {
      (enumerator as IDisposable)?.Dispose();
    }
    return false;
  }

  public static List<T> FindComponents<T>(Transform transform)
  {
    List<T> list = new List<T>();
    PrefabPreProcess.FindComponents<T>(transform, list);
    return list;
  }

  public static void FindComponents<T>(Transform transform, List<T> list)
  {
    list.AddRange((IEnumerable<T>) ((Component) transform).GetComponents<T>());
    IEnumerator enumerator = transform.GetEnumerator();
    try
    {
      while (enumerator.MoveNext())
      {
        Transform current = (Transform) enumerator.Current;
        if (!PrefabPreProcess.ShouldExclude(current))
          PrefabPreProcess.FindComponents<T>(current, list);
      }
    }
    finally
    {
      (enumerator as IDisposable)?.Dispose();
    }
  }

  public static List<Component> FindComponents(Transform transform, System.Type t)
  {
    List<Component> list = new List<Component>();
    PrefabPreProcess.FindComponents(transform, list, t);
    return list;
  }

  public static void FindComponents(Transform transform, List<Component> list, System.Type t)
  {
    list.AddRange((IEnumerable<Component>) ((Component) transform).GetComponents(t));
    IEnumerator enumerator = transform.GetEnumerator();
    try
    {
      while (enumerator.MoveNext())
      {
        Transform current = (Transform) enumerator.Current;
        if (!PrefabPreProcess.ShouldExclude(current))
          PrefabPreProcess.FindComponents(current, list, t);
      }
    }
    finally
    {
      (enumerator as IDisposable)?.Dispose();
    }
  }

  public void RemoveComponent(Component c)
  {
    if (Object.op_Equality((Object) c, (Object) null))
      return;
    this.destroyList.Add(c);
  }

  public void RemoveComponents(GameObject gameObj)
  {
    foreach (Component component in (Component[]) gameObj.GetComponents<Component>())
    {
      if (!(component is Transform))
        this.destroyList.Add(component);
    }
  }

  public void NominateForDeletion(GameObject gameObj)
  {
    this.cleanupList.Add(gameObj);
  }

  private void RunCleanupQueue()
  {
    using (List<Component>.Enumerator enumerator = this.destroyList.GetEnumerator())
    {
      while (enumerator.MoveNext())
        Object.DestroyImmediate((Object) enumerator.Current, true);
    }
    this.destroyList.Clear();
    using (List<GameObject>.Enumerator enumerator = this.cleanupList.GetEnumerator())
    {
      while (enumerator.MoveNext())
        this.DoCleanup(enumerator.Current);
    }
    this.cleanupList.Clear();
  }

  private void DoCleanup(GameObject go)
  {
    if (Object.op_Equality((Object) go, (Object) null) || go.GetComponentsInChildren<Component>(true).Length > 1)
      return;
    Transform parent = go.get_transform().get_parent();
    if (Object.op_Equality((Object) parent, (Object) null) || ((Object) parent).get_name().StartsWith("PrefabPreProcess - "))
      return;
    Object.DestroyImmediate((Object) go, true);
  }
}
