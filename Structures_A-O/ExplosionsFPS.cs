// Decompiled with JetBrains decompiler
// Type: ExplosionsFPS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionsFPS : MonoBehaviour
{
  private readonly GUIStyle guiStyleHeader;
  private float timeleft;
  private float fps;
  private int frames;

  private void Awake()
  {
    this.guiStyleHeader.set_fontSize(14);
    this.guiStyleHeader.get_normal().set_textColor(new Color(1f, 1f, 1f));
  }

  private void OnGUI()
  {
    GUI.Label(new Rect(0.0f, 0.0f, 30f, 30f), "FPS: " + (object) (int) this.fps, this.guiStyleHeader);
  }

  private void Update()
  {
    this.timeleft -= Time.get_deltaTime();
    ++this.frames;
    if ((double) this.timeleft > 0.0)
      return;
    this.fps = (float) this.frames;
    this.timeleft = 1f;
    this.frames = 0;
  }

  public ExplosionsFPS()
  {
    base.\u002Ector();
  }
}
