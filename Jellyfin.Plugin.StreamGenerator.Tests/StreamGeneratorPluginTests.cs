using Jellyfin.Plugin.StreamGenerator.Model;

namespace Jellyfin.Plugin.StreamGenerator.Tests;

public class StreamGeneratorPluginTests
{
    [Fact]
    public void PatchContextMenu_AddsPopupLoaderWithoutCallingResolver()
    {
        const string contextMenu = "items.push({id:\"copy-stream\",icon:\"content_copy\"});switch(x){case\"copy-stream\":break;}";

        var result = StreamGeneratorPlugin.PatchContextMenu(new PatchRequestPayload
        {
            Contents = contextMenu
        });

        result.Should().Contain("id:\"generate-stream\",icon:\"link\"");
        result.Should().Contain("StreamGenerator/PopupContent.js");
        result.Should().NotContain("getResolveFunction");
    }
}
