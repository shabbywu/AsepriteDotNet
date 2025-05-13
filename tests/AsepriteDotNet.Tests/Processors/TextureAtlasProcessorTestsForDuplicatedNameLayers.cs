// Copyright (c) Christopher Whitley. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using AsepriteDotNet.Aseprite;
using AsepriteDotNet.Aseprite.Document;
using AsepriteDotNet.Aseprite.Types;
using AsepriteDotNet.Common;
using AsepriteDotNet.Processors;

namespace AsepriteDotNet.Tests.Processors;

public sealed class TextureAtlasProcessorTestFixtureForDuplicatedNameLayers
{
    public string Name { get; } = "sprite-processor-test";
    public AsepriteFile AsepriteFile { get; }
    public Rgba32 Red { get; } = new Rgba32(255, 0, 0, 255);
    public Rgba32 Green { get; } = new Rgba32(0, 255, 0, 255);
    public Rgba32 Blue { get; } = new Rgba32(0, 0, 255, 255);
    public Rgba32 Yellow { get; } = new Rgba32(255, 255, 0, 255);

    public TextureAtlasProcessorTestFixtureForDuplicatedNameLayers()
    {
        int width = 1;
        int height = 1;

        AsepritePalette palette = new AsepritePalette(0);
        AsepriteTileset[] tilesets = Array.Empty<AsepriteTileset>();

        List<AsepriteLayer> layers = new List<AsepriteLayer>()
        {
            new AsepriteImageLayer(new AsepriteLayerProperties() {Flags = 1, BlendMode = (ushort)AsepriteBlendMode.Addition, Opacity = 255 }, "layer"),
            new AsepriteImageLayer(new AsepriteLayerProperties() {Flags = 1, BlendMode = (ushort)AsepriteBlendMode.Addition, Opacity = 255 }, "layer"),
        };

        AsepriteImageCelProperties imageCelProperties = new AsepriteImageCelProperties() { Width = 1, Height = 1 };
        List<AsepriteCel> Frame0Cels = new List<AsepriteCel>()
        {
            new AsepriteImageCel(new AsepriteCelProperties() { Opacity = 255, LayerIndex = 0 }, layers[0], imageCelProperties, new Rgba32[] {Red, Red, Red, Red }),
            new AsepriteImageCel(new AsepriteCelProperties() { Opacity = 255, LayerIndex = 1 }, layers[1], imageCelProperties, new Rgba32[] { Blue, Blue, Blue, Blue })
        };

        List<AsepriteFrame> frames = new List<AsepriteFrame>()
        {
            new($"{Name} 0", width, height, 100, Frame0Cels),
        };

        AsepriteFile = new AsepriteFile(Name, palette, width, height, AsepriteColorDepth.RGBA, frames, layers, [], [], [], new AsepriteUserData(), []);
    }
}



public sealed class TextureAtlasProcessorTestsForDuplicatedNameLayers: IClassFixture<TextureAtlasProcessorTestFixtureForDuplicatedNameLayers>
{
    private readonly TextureAtlasProcessorTestFixtureForDuplicatedNameLayers _fixture;

    //  These are the colors that will be expected in the  texture created during each process.  They are created
    //  here an named this way so that it's easier to visualize what the pixel array should be in the test below.
    private readonly Rgba32 red = new Rgba32(255, 0, 0, 255); //  Represents a red pixel
    private readonly Rgba32 blue = new Rgba32(0, 0, 255, 255); //  Represents a blue pixel
    private readonly Rgba32 magenta = new Rgba32(255, 0, 255, 255); //  Represents a red + blue = Magenta pixel

    public TextureAtlasProcessorTestsForDuplicatedNameLayers(TextureAtlasProcessorTestFixtureForDuplicatedNameLayers fixture) => _fixture = fixture;

    [Fact]
    public void ProcessByName()
    {
        TextureAtlas atlas = TextureAtlasProcessor.Process(_fixture.AsepriteFile, ["layer"]);
        Rgba32[] expected = new Rgba32[]
        {
            magenta
        };

        Rgba32[] actual = atlas.Texture.Pixels.ToArray();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ProcessByLayer()
    {
        TextureAtlas atlasForLayer0 = TextureAtlasProcessor.Process(_fixture.AsepriteFile, [_fixture.AsepriteFile.Layers[0]]);
        Rgba32[] expectedForLayer0 = new Rgba32[]
        {
            red
        };

        Assert.Equal(expectedForLayer0, atlasForLayer0.Texture.Pixels.ToArray());

        TextureAtlas atlasForLayer1 = TextureAtlasProcessor.Process(_fixture.AsepriteFile, [_fixture.AsepriteFile.Layers[1]]);
        Rgba32[] expectedForLayer1 = new Rgba32[]
        {
            blue
        };
        Assert.Equal(expectedForLayer1, atlasForLayer1.Texture.Pixels.ToArray());
    }
}
