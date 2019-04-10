// Decompiled with JetBrains decompiler
// Type: UnityEngine.TransformEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
  public static class TransformEx
  {
    public static string GetRecursiveName(this Transform transform, string strEndName = "")
    {
      string strEndName1 = ((Object) transform).get_name();
      if (!string.IsNullOrEmpty(strEndName))
        strEndName1 = strEndName1 + "/" + strEndName;
      if (Object.op_Inequality((Object) transform.get_parent(), (Object) null))
        strEndName1 = transform.get_parent().GetRecursiveName(strEndName1);
      return strEndName1;
    }

    public static void RemoveComponent<T>(this Transform transform) where T : Component
    {
      T component = ((Component) transform).GetComponent<T>();
      if (Object.op_Equality((Object) (object) component, (Object) null))
        return;
      GameManager.Destroy((Component) (object) component, 0.0f);
    }

    public static void DestroyAllChildren(this Transform transform, bool immediate = false)
    {
      List<GameObject> list = (List<GameObject>) Pool.GetList<GameObject>();
      IEnumerator enumerator1 = transform.GetEnumerator();
      try
      {
        while (enumerator1.MoveNext())
        {
          Transform current = (Transform) enumerator1.Current;
          if (!((Component) current).CompareTag("persist"))
            list.Add(((Component) current).get_gameObject());
        }
      }
      finally
      {
        (enumerator1 as IDisposable)?.Dispose();
      }
      if (immediate)
      {
        using (List<GameObject>.Enumerator enumerator2 = list.GetEnumerator())
        {
          while (enumerator2.MoveNext())
            GameManager.DestroyImmediate(enumerator2.Current, false);
        }
      }
      else
      {
        using (List<GameObject>.Enumerator enumerator2 = list.GetEnumerator())
        {
          while (enumerator2.MoveNext())
            GameManager.Destroy(enumerator2.Current, 0.0f);
        }
      }
      // ISSUE: cast to a reference type
      Pool.FreeList<GameObject>((List<M0>&) ref list);
    }

    public static void RetireAllChildren(this Transform transform, GameManager gameManager)
    {
      List<GameObject> list = (List<GameObject>) Pool.GetList<GameObject>();
      IEnumerator enumerator1 = transform.GetEnumerator();
      try
      {
        while (enumerator1.MoveNext())
        {
          Transform current = (Transform) enumerator1.Current;
          if (!((Component) current).CompareTag("persist"))
            list.Add(((Component) current).get_gameObject());
        }
      }
      finally
      {
        (enumerator1 as IDisposable)?.Dispose();
      }
      using (List<GameObject>.Enumerator enumerator2 = list.GetEnumerator())
      {
        while (enumerator2.MoveNext())
        {
          GameObject current = enumerator2.Current;
          gameManager.Retire(current);
        }
      }
      // ISSUE: cast to a reference type
      Pool.FreeList<GameObject>((List<M0>&) ref list);
    }

    public static List<Transform> GetChildren(this Transform transform)
    {
      return ((IEnumerable) transform).Cast<Transform>().ToList<Transform>();
    }

    public static void OrderChildren(this Transform tx, Func<Transform, object> selector)
    {
      using (IEnumerator<Transform> enumerator = ((IEnumerable<Transform>) ((IEnumerable) tx).Cast<Transform>().OrderBy<Transform, object>(selector)).GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
          enumerator.Current.SetAsLastSibling();
      }
    }

    public static List<Transform> GetAllChildren(this Transform transform)
    {
      List<Transform> list = new List<Transform>();
      if (Object.op_Inequality((Object) transform, (Object) null))
        transform.AddAllChildren(list);
      return list;
    }

    public static void AddAllChildren(this Transform transform, List<Transform> list)
    {
      list.Add(transform);
      for (int index = 0; index < transform.get_childCount(); ++index)
      {
        Transform child = transform.GetChild(index);
        if (!Object.op_Equality((Object) child, (Object) null))
          child.AddAllChildren(list);
      }
    }

    public static Transform[] GetChildrenWithTag(this Transform transform, string strTag)
    {
      return ((IEnumerable<Transform>) transform.GetAllChildren()).Where<Transform>((Func<Transform, bool>) (x => ((Component) x).CompareTag(strTag))).ToArray<Transform>();
    }

    public static void Identity(this GameObject go)
    {
      go.get_transform().set_localPosition(Vector3.get_zero());
      go.get_transform().set_localRotation(Quaternion.get_identity());
      go.get_transform().set_localScale(Vector3.get_one());
    }

    public static GameObject CreateChild(this GameObject go)
    {
      GameObject go1 = new GameObject();
      go1.get_transform().set_parent(go.get_transform());
      go1.Identity();
      return go1;
    }

    public static GameObject InstantiateChild(this GameObject go, GameObject prefab)
    {
      GameObject go1 = Instantiate.GameObject(prefab, (Transform) null);
      go1.get_transform().SetParent(go.get_transform(), false);
      go1.Identity();
      return go1;
    }

    public static void SetLayerRecursive(this GameObject go, int Layer)
    {
      if (go.get_layer() != Layer)
        go.set_layer(Layer);
      for (int index = 0; index < go.get_transform().get_childCount(); ++index)
        ((Component) go.get_transform().GetChild(index)).get_gameObject().SetLayerRecursive(Layer);
    }

    public static bool DropToGround(this Transform transform, bool alignToNormal = false, float fRange = 100f)
    {
      Vector3 pos;
      Vector3 normal;
      if (!transform.GetGroundInfo(out pos, out normal, fRange))
        return false;
      transform.set_position(pos);
      if (alignToNormal)
        transform.set_rotation(Quaternion.LookRotation(transform.get_forward(), normal));
      return true;
    }

    public static bool GetGroundInfo(
      this Transform transform,
      out Vector3 pos,
      out Vector3 normal,
      float range = 100f)
    {
      return TransformUtil.GetGroundInfo(transform.get_position(), out pos, out normal, range, transform);
    }

    public static bool GetGroundInfoTerrainOnly(
      this Transform transform,
      out Vector3 pos,
      out Vector3 normal,
      float range = 100f)
    {
      return TransformUtil.GetGroundInfoTerrainOnly(transform.get_position(), out pos, out normal, range);
    }

    public static Bounds WorkoutRenderBounds(this Transform tx)
    {
      Bounds bounds;
      ((Bounds) ref bounds).\u002Ector(Vector3.get_zero(), Vector3.get_zero());
      foreach (Renderer componentsInChild in (Renderer[]) ((Component) tx).GetComponentsInChildren<Renderer>())
      {
        if (!(componentsInChild is ParticleSystemRenderer))
        {
          if (Vector3.op_Equality(((Bounds) ref bounds).get_center(), Vector3.get_zero()))
            bounds = componentsInChild.get_bounds();
          else
            ((Bounds) ref bounds).Encapsulate(componentsInChild.get_bounds());
        }
      }
      return bounds;
    }

    public static List<T> GetSiblings<T>(this Transform transform, bool includeSelf = false)
    {
      List<T> objList = new List<T>();
      if (Object.op_Equality((Object) transform.get_parent(), (Object) null))
        return objList;
      for (int index = 0; index < transform.get_parent().get_childCount(); ++index)
      {
        Transform child = transform.get_parent().GetChild(index);
        if (includeSelf || !Object.op_Equality((Object) child, (Object) transform))
        {
          T component = ((Component) child).GetComponent<T>();
          if ((object) component != null)
            objList.Add(component);
        }
      }
      return objList;
    }

    public static void DestroyChildren(this Transform transform)
    {
      for (int index = 0; index < transform.get_childCount(); ++index)
        GameManager.Destroy(((Component) transform.GetChild(index)).get_gameObject(), 0.0f);
    }

    public static void SetChildrenActive(this Transform transform, bool b)
    {
      for (int index = 0; index < transform.get_childCount(); ++index)
        ((Component) transform.GetChild(index)).get_gameObject().SetActive(b);
    }

    public static Transform ActiveChild(
      this Transform transform,
      string name,
      bool bDisableOthers)
    {
      Transform transform1 = (Transform) null;
      for (int index = 0; index < transform.get_childCount(); ++index)
      {
        Transform child = transform.GetChild(index);
        if (((Object) child).get_name().Equals(name, StringComparison.InvariantCultureIgnoreCase))
        {
          transform1 = child;
          ((Component) child).get_gameObject().SetActive(true);
        }
        else if (bDisableOthers)
          ((Component) child).get_gameObject().SetActive(false);
      }
      return transform1;
    }

    public static T GetComponentInChildrenIncludeDisabled<T>(this Transform transform) where T : Component
    {
      List<T> list = Pool.GetList<T>();
      ((Component) transform).GetComponentsInChildren<T>(true, list);
      T obj = list.Count > 0 ? list[0] : default (T);
      Pool.FreeList<T>(ref list);
      return obj;
    }

    public static void SetHierarchyGroup(
      this Transform transform,
      string strRoot,
      bool groupActive = true,
      bool persistant = false)
    {
      transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).get_transform(), true);
    }

    public static Bounds GetBounds(
      this Transform transform,
      bool includeRenderers = true,
      bool includeColliders = true,
      bool includeInactive = true)
    {
      Bounds bounds1;
      ((Bounds) ref bounds1).\u002Ector(Vector3.get_zero(), Vector3.get_zero());
      if (includeRenderers)
      {
        foreach (MeshFilter componentsInChild in (MeshFilter[]) ((Component) transform).GetComponentsInChildren<MeshFilter>(includeInactive))
        {
          if (Object.op_Implicit((Object) componentsInChild.get_sharedMesh()))
          {
            Matrix4x4 matrix = Matrix4x4.op_Multiply(transform.get_worldToLocalMatrix(), ((Component) componentsInChild).get_transform().get_localToWorldMatrix());
            Bounds bounds2 = componentsInChild.get_sharedMesh().get_bounds();
            ((Bounds) ref bounds1).Encapsulate(bounds2.Transform(matrix));
          }
        }
        foreach (SkinnedMeshRenderer componentsInChild in (SkinnedMeshRenderer[]) ((Component) transform).GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive))
        {
          if (Object.op_Implicit((Object) componentsInChild.get_sharedMesh()))
          {
            Matrix4x4 matrix = Matrix4x4.op_Multiply(transform.get_worldToLocalMatrix(), ((Component) componentsInChild).get_transform().get_localToWorldMatrix());
            Bounds bounds2 = componentsInChild.get_sharedMesh().get_bounds();
            ((Bounds) ref bounds1).Encapsulate(bounds2.Transform(matrix));
          }
        }
      }
      if (includeColliders)
      {
        foreach (MeshCollider componentsInChild in (MeshCollider[]) ((Component) transform).GetComponentsInChildren<MeshCollider>(includeInactive))
        {
          if (Object.op_Implicit((Object) componentsInChild.get_sharedMesh()) && !((Collider) componentsInChild).get_isTrigger())
          {
            Matrix4x4 matrix = Matrix4x4.op_Multiply(transform.get_worldToLocalMatrix(), ((Component) componentsInChild).get_transform().get_localToWorldMatrix());
            Bounds bounds2 = componentsInChild.get_sharedMesh().get_bounds();
            ((Bounds) ref bounds1).Encapsulate(bounds2.Transform(matrix));
          }
        }
      }
      return bounds1;
    }
  }
}
