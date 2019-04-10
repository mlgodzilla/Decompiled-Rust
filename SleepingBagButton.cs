// Decompiled with JetBrains decompiler
// Type: SleepingBagButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

public class SleepingBagButton : MonoBehaviour
{
  public GameObject timerInfo;
  public Text BagName;
  public Text LockTime;
  internal Button button;
  internal RespawnInformation.SpawnOptions spawnOptions;
  internal float releaseTime;

  public float timerSeconds
  {
    get
    {
      return Mathf.Clamp(this.releaseTime - Time.get_realtimeSinceStartup(), 0.0f, 216000f);
    }
  }

  public string friendlyName
  {
    get
    {
      if (this.spawnOptions == null || string.IsNullOrEmpty((string) this.spawnOptions.name))
        return "Null Sleeping Bag";
      return (string) this.spawnOptions.name;
    }
  }

  public void Setup(RespawnInformation.SpawnOptions options)
  {
    this.button = (Button) ((Component) this).GetComponent<Button>();
    this.spawnOptions = options;
    if (options.unlockSeconds > 0.0)
    {
      ((Selectable) this.button).set_interactable(false);
      this.timerInfo.SetActive(true);
      this.releaseTime = Time.get_realtimeSinceStartup() + (float) options.unlockSeconds;
    }
    else
    {
      ((Selectable) this.button).set_interactable(true);
      this.timerInfo.SetActive(false);
      this.releaseTime = 0.0f;
    }
    this.BagName.set_text(this.friendlyName);
  }

  public void Update()
  {
    if ((double) this.releaseTime == 0.0)
      return;
    if ((double) this.releaseTime < (double) Time.get_realtimeSinceStartup())
    {
      this.releaseTime = 0.0f;
      this.timerInfo.SetActive(false);
      ((Selectable) this.button).set_interactable(true);
    }
    else
      this.LockTime.set_text(this.timerSeconds.ToString("N0"));
  }

  public void DoSpawn()
  {
    ConsoleSystem.Run(ConsoleSystem.Option.get_Client(), "respawn_sleepingbag", new object[1]
    {
      (object) (uint) this.spawnOptions.id
    });
  }

  public void DeleteBag()
  {
    ConsoleSystem.Run(ConsoleSystem.Option.get_Client(), "respawn_sleepingbag_remove", new object[1]
    {
      (object) (uint) this.spawnOptions.id
    });
  }

  public SleepingBagButton()
  {
    base.\u002Ector();
  }
}
