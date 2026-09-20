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

    [Fact]
    public void PopupScript_UsesNativeCodecsWithoutTranscodingPermissions()
    {
        const string resourceName = "Jellyfin.Plugin.StreamGenerator.Web.PopupContent.js";
        using var stream = typeof(StreamGeneratorPlugin).Assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        var script = reader.ReadToEnd();

        script.Should().Contain("EnableVideoPlaybackTranscoding");
        script.Should().Contain("EnableAudioPlaybackTranscoding");
        script.Should().Contain("videoTranscodingAllowed");
        script.Should().Contain("audioTranscodingAllowed");
        script.Should().Contain("System/Configuration/encoding");
        script.Should().Contain("AllowHevcEncoding");
        script.Should().Contain("AllowAv1Encoding");
        script.Should().Contain("const videoCodecs = ['h264']");
        script.Should().Contain("['aac', 'flac']");
        script.Should().NotContain("TranscodingVideoCodecs");
        script.Should().Contain("if (videoTranscodingAllowed)");
        script.Should().Contain("!videoTranscodingAllowed ? 'Hls' : 'Encode'");
    }

    [Fact]
    public void PopupScript_ContainsIndependentOriginalFormatOptions()
    {
        const string resourceName = "Jellyfin.Plugin.StreamGenerator.Web.PopupContent.js";
        using var stream = typeof(StreamGeneratorPlugin).Assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        var script = reader.ReadToEnd();

        script.Should().Contain("id=\"useOriginalVideoFormat\" checked");
        script.Should().Contain("id=\"useOriginalAudioFormat\" checked");
        script.Should().Contain("Native (' + sourceVideoCodecs.join");
        script.Should().Contain("Native (' + sourceAudioCodecs.join");
        script.Should().Contain("nativeAudioRequiresTs");
        script.Should().Contain("'eac3', 'ac3', 'truehd', 'dts', 'dts-hd'");
        script.Should().Contain("subtitleMethod === 'Encode'");
        script.Should().Contain("? ['h264']");
    }
}
