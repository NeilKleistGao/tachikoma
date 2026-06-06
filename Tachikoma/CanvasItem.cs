using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tachikoma {
  internal abstract class CanvasItem {
    private Vector2 localPosition = Vector2.Zero;
    private Vector2 globalPosition = Vector2.Zero;

    public Vector2 LocalPosition { get => localPosition; }
    public Vector2 GlobalPosition { get => globalPosition; }

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
      foreach (var child in children) {
        child.Draw(batch, gameTime);
      }
    }

    public void SetPosition(Vector2 position) {
      localPosition = position;
      UpdateGlobalPosition();
    }

    private void UpdateGlobalPosition() {
      if (parent == null) {
        globalPosition = localPosition;
      }
      else {
        globalPosition = parent.GlobalPosition + localPosition;
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
