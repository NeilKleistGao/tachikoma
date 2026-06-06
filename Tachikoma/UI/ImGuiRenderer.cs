using ImGuiNET;

namespace Tachikoma {
  internal class ImGuiRenderer {
    private Game game;

    public ImGuiRenderer(Game game) {
      this.game = game;
      var context = ImGui.CreateContext();
      ImGui.SetCurrentContext(context);
    }
  }
}
