using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tachikoma {
  internal abstract class CanvasItem {
    public Vector2 LocalPosition { get; private set; } = Vector2.Zero;
    public Vector2 GlobalPosition { get; private set; } = Vector2.Zero;

    public bool Visible { get; set; } = true;

    private List<CanvasItem> children = new();
    private CanvasItem parent = null;

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
      if (!Visible) { return; }

      foreach (var child in children) {
        child.Draw(batch, gameTime);
      }
    }

    public void SetPosition(Vector2 position) {
      LocalPosition = position;
      UpdateGlobalPosition();
    }

    private void UpdateGlobalPosition() {
      if (parent == null) {
        GlobalPosition = LocalPosition;
      }
      else {
        GlobalPosition = parent.GlobalPosition + LocalPosition;
      }
      foreach (var child in children) {
        child.UpdateGlobalPosition();
      }
    }

    public void AddChild(CanvasItem child) {
      children.Add(child);
      child.parent = this;
      child.UpdateGlobalPosition();
    }
  }
}
