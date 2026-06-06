using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tachikoma {
  internal abstract class CanvasItem {
    public Vector2 Position { get; set; } = Vector2.Zero;

    protected List<CanvasItem> children = new();

    public virtual void Initialize() {
      foreach (var child in children) {
        child.Initialize();
      }
    }

    public virtual void LoadContent(ContentManager content) {
      foreach (var child in children) {
        child.LoadContent(content);
      }
    }

    public virtual void Update(GameTime gameTime) {
      foreach (var child in children) {
        child.Update(gameTime);
      } 
    }

    public virtual void Draw(SpriteBatch batch, GameTime gameTime) {
      foreach (var child in children) {
        child.Draw(batch, gameTime);
      }
    }
  }
}
