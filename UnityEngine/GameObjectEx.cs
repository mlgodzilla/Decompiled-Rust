// Decompiled with JetBrains decompiler
// Type: UnityEngine.GameObjectEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using Rust.Registry;

namespace UnityEngine
{
  public static class GameObjectEx
  {
    public static BaseEntity ToBaseEntity(this GameObject go)
    {
      IEntity ientity = GameObjectEx.GetEntityFromRegistry(go);
      if (ientity == null && !((Component) go.get_transform()).get_gameObject().get_activeSelf())
        ientity = GameObjectEx.GetEntityFromComponent(go);
      return ientity as BaseEntity;
    }

    private static IEntity GetEntityFromRegistry(GameObject go)
    {
      Transform transform = go.get_transform();
      IEntity ientity;
      for (ientity = Entity.Get(((Component) transform).get_gameObject()); ientity == null && Object.op_Inequality((Object) transform.get_parent(), (Object) null); ientity = Entity.Get(((Component) transform).get_gameObject()))
        transform = transform.get_parent();
      if (ientity != null && !ientity.get_IsDestroyed())
        return ientity;
      return (IEntity) null;
    }

    private static IEntity GetEntityFromComponent(GameObject go)
    {
      Transform transform = go.get_transform();
      IEntity component;
      for (component = (IEntity) ((Component) transform).GetComponent<IEntity>(); component == null && Object.op_Inequality((Object) transform.get_parent(), (Object) null); component = (IEntity) ((Component) transform).GetComponent<IEntity>())
        transform = transform.get_parent();
      if (component != null && !component.get_IsDestroyed())
        return component;
      return (IEntity) null;
    }

    public static void SetHierarchyGroup(
      this GameObject obj,
      string strRoot,
      bool groupActive = true,
      bool persistant = false)
    {
      obj.get_transform().SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).get_transform(), true);
    }
  }
}
