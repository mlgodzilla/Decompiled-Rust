// Decompiled with JetBrains decompiler
// Type: ComponentInfo`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public abstract class ComponentInfo<T> : ComponentInfo
{
  public T component;

  public void Initialize(T source)
  {
    this.component = source;
    this.Setup();
  }
}
