// Decompiled with JetBrains decompiler
// Type: ChildrenFromScene
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChildrenFromScene : MonoBehaviour
{
  public string SceneName;
  public bool StartChildrenDisabled;

  private IEnumerator Start()
  {
    ChildrenFromScene childrenFromScene = this;
    Scene sceneByName = SceneManager.GetSceneByName(childrenFromScene.SceneName);
    if (!((Scene) ref sceneByName).get_isLoaded())
      yield return (object) SceneManager.LoadSceneAsync(childrenFromScene.SceneName, (LoadSceneMode) 1);
    sceneByName = SceneManager.GetSceneByName(childrenFromScene.SceneName);
    foreach (GameObject rootGameObject in ((Scene) ref sceneByName).GetRootGameObjects())
    {
      rootGameObject.get_transform().SetParent(((Component) childrenFromScene).get_transform(), false);
      rootGameObject.Identity();
      RectTransform transform = rootGameObject.get_transform() as RectTransform;
      if (Object.op_Implicit((Object) transform))
      {
        transform.set_pivot(Vector2.get_zero());
        transform.set_anchoredPosition(Vector2.get_zero());
        transform.set_anchorMin(Vector2.get_zero());
        transform.set_anchorMax(Vector2.get_one());
        transform.set_sizeDelta(Vector2.get_one());
      }
      foreach (SingletonComponent componentsInChild in (SingletonComponent[]) rootGameObject.GetComponentsInChildren<SingletonComponent>(true))
        componentsInChild.Setup();
      if (childrenFromScene.StartChildrenDisabled)
        rootGameObject.SetActive(false);
    }
    SceneManager.UnloadSceneAsync(sceneByName);
  }

  public ChildrenFromScene()
  {
    base.\u002Ector();
  }
}
