using Microsoft.Xna.Framework.Content;
using System.Windows.Forms;

namespace Tachikoma {
  internal class Tachikoma : CanvasItem {
    private Sprite sprite;

    public override void Initialize() {
      base.Initialize();
      sprite = new Sprite("tachikoma");
      sprite.Clickable = true;
      sprite.OnClicked += SwitchLLMDialogueUI;
      AddChild(sprite);
    }

    public override void LoadContent(ContentManager content) {
      base.LoadContent(content);

      int width = SystemInformation.WorkingArea.Size.Width;
      int height = SystemInformation.WorkingArea.Size.Height;
      var size = sprite.Size;
      SetPosition(new Vector2(width * 0.9f - size.X, height - size.Y));
    }

    private void SwitchLLMDialogueUI(object sender, EventArgs e) {
      // TODO: 
      Debug.WriteLine("rua: clicked.");
    }
  }
}
