using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Tachikoma {
  internal abstract class CanvasItem {
    public Vector2 LocalPosition { get; private set; } = Vector2.Zero;
    public Vector2 GlobalPosition { get; private set; } = Vector2.Zero;

    public bool Visible { get; set; } = true;
    public bool Clickable { get; set; } = false;

    private List<CanvasItem> children = new();
    private CanvasItem parent = null;

    private MouseState previousMouseState;
    private MouseState currentMouseState;

    public event EventHandler OnClicked;

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

      if (Clickable) {
        UpdateMouseState();
      }
    }

    private void UpdateMouseState() {
      previousMouseState = currentMouseState;
      currentMouseState = Mouse.GetState();

      var mouseRect = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);
      if (GetBounds().Intersects(mouseRect)) {
        if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed) {
          OnClicked?.Invoke(this, EventArgs.Empty);
        }
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

    public virtual Rectangle GetBounds() {
      return new Rectangle((int)GlobalPosition.X, (int)GlobalPosition.Y, 0, 0);
    }
  }
}
