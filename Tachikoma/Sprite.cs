using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tachikoma {
  internal class Sprite : CanvasItem {
    private Texture2D texture;
    private string textureID;

    public Vector2 Size {
      get {
        if (texture != null) {
          return new Vector2(texture.Width, texture.Height);
        }
        else {
          return Vector2.Zero;
        }
      }
    }

    // TODO: animation support

    public Sprite(string textureID) {
      this.textureID = textureID;
    }

    public override void LoadContent(ContentManager content) {
      base.LoadContent(content);
      texture = content.Load<Texture2D>(textureID);
    }

    public override void Draw(SpriteBatch batch, GameTime gameTime) {
      base.Draw(batch, gameTime);
      if (texture != null) {
        batch.Draw(texture, GlobalPosition, Color.White);
      }
    }
  }
}
