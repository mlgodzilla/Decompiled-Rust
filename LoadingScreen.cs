// Decompiled with JetBrains decompiler
// Type: LoadingScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : SingletonComponent<LoadingScreen>
{
  public CanvasRenderer panel;
  public UnityEngine.UI.Text title;
  public UnityEngine.UI.Text subtitle;
  public Button skipButton;
  public AudioSource music;

  public static bool isOpen
  {
    get
    {
      if (Object.op_Implicit((Object) SingletonComponent<LoadingScreen>.Instance) && Object.op_Implicit((Object) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).panel))
        return ((Component) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).panel).get_gameObject().get_activeSelf();
      return false;
    }
  }

  public static bool WantsSkip { get; private set; }

  public static string Text { private set; get; }

  protected virtual void Awake()
  {
    ((SingletonComponent) this).Awake();
    LoadingScreen.HideSkip();
    LoadingScreen.Hide();
  }

  public static void Show()
  {
    if (!Object.op_Implicit((Object) SingletonComponent<LoadingScreen>.Instance))
    {
      Debug.LogWarning((object) "Wanted to show loading screen but not ready");
    }
    else
    {
      if (((Component) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).panel).get_gameObject().get_activeSelf())
        return;
      ((Component) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).panel).get_gameObject().SetActive(true);
      ((Component) SingletonComponent<LoadingScreen>.Instance).get_gameObject().SetActive(false);
      ((Component) SingletonComponent<LoadingScreen>.Instance).get_gameObject().SetActive(true);
      MusicManager.RaiseIntensityTo(0.5f, 999);
    }
  }

  public static void Hide()
  {
    if (!Object.op_Implicit((Object) SingletonComponent<LoadingScreen>.Instance) || !Object.op_Implicit((Object) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).panel) || (!Object.op_Implicit((Object) ((Component) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).panel).get_gameObject()) || !((Component) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).panel).get_gameObject().get_activeSelf()))
      return;
    ((Component) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).panel).get_gameObject().SetActive(false);
    ((Component) SingletonComponent<LoadingScreen>.Instance).get_gameObject().SetActive(false);
    ((Component) SingletonComponent<LoadingScreen>.Instance).get_gameObject().SetActive(true);
    if (!LevelManager.isLoaded || !Object.op_Inequality((Object) SingletonComponent<MusicManager>.Instance, (Object) null))
      return;
    ((MusicManager) SingletonComponent<MusicManager>.Instance).StopMusic();
  }

  public static void ShowSkip()
  {
    LoadingScreen.WantsSkip = false;
    if (!Object.op_Implicit((Object) SingletonComponent<LoadingScreen>.Instance) || !Object.op_Implicit((Object) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).skipButton))
      return;
    ((Component) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).skipButton).get_gameObject().SetActive(true);
  }

  public static void HideSkip()
  {
    LoadingScreen.WantsSkip = false;
    if (!Object.op_Implicit((Object) SingletonComponent<LoadingScreen>.Instance) || !Object.op_Implicit((Object) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).skipButton))
      return;
    ((Component) ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).skipButton).get_gameObject().SetActive(false);
  }

  public static void Update(string strType)
  {
    if (LoadingScreen.Text == strType)
      return;
    LoadingScreen.Text = strType;
    if (!Object.op_Implicit((Object) SingletonComponent<LoadingScreen>.Instance))
      return;
    ((LoadingScreen) SingletonComponent<LoadingScreen>.Instance).subtitle.set_text(strType.ToUpper());
    GameObject gameObject = GameObject.Find("MenuMusic");
    if (!Object.op_Implicit((Object) gameObject))
      return;
    AudioSource component = (AudioSource) gameObject.GetComponent<AudioSource>();
    if (!Object.op_Implicit((Object) component))
      return;
    component.Pause();
  }

  public void UpdateFromServer(string strTitle, string strSubtitle)
  {
    this.title.set_text(strTitle);
    this.subtitle.set_text(strSubtitle);
  }

  public void CancelLoading()
  {
    ConsoleSystem.Option client = ConsoleSystem.Option.get_Client();
    ConsoleSystem.Run(((ConsoleSystem.Option) ref client).Quiet(), "client.disconnect", (object[]) Array.Empty<object>());
  }

  public void SkipLoading()
  {
    LoadingScreen.WantsSkip = true;
  }

  public LoadingScreen()
  {
    base.\u002Ector();
  }
}
