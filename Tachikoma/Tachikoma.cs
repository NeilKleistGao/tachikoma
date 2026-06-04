namespace Tachikoma {
  internal class Tachikoma : Nez.Core {
    public Tachikoma() : base(isFullScreen: true, windowTitle: "Tachikoma") {
    }

    protected override void Initialize() {
      base.Initialize();

      var handler = Window.Handle;
    }
  }
}
