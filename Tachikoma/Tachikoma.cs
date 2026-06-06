namespace Tachikoma {
  internal class Tachikoma : CanvasItem {
    private Sprite sprite;

    public override void Initialize() {
      base.Initialize();
      sprite = new Sprite("tachikoma");
      children.Add(sprite);
    }
  }
}
