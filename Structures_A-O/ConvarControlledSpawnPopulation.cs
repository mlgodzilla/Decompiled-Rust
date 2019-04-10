// Decompiled with JetBrains decompiler
// Type: ConvarControlledSpawnPopulation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(menuName = "Rust/Convar Controlled Spawn Population")]
public class ConvarControlledSpawnPopulation : SpawnPopulation
{
  [Header("Convars")]
  public string PopulationConvar;
  private ConsoleSystem.Command _command;

  protected ConsoleSystem.Command Command
  {
    get
    {
      if (this._command == null)
      {
        this._command = ConsoleSystem.Index.Server.Find(this.PopulationConvar);
        Assert.IsNotNull<ConsoleSystem.Command>((M0) this._command, string.Format("{0} has missing convar {1}", (object) this, (object) this.PopulationConvar));
      }
      return this._command;
    }
  }

  public override float TargetDensity
  {
    get
    {
      return this.Command.get_AsFloat();
    }
  }
}
