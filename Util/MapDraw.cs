using AoCGodot;
using Godot;
using System;
using System.Collections.Generic;

public partial class MapDraw : Control
{
    Color[,] pixelColors;
    int width = 0;
    int height = 0;
    float pixelDim = 5;

    public void SetMap<T>(Map<T> map, Dictionary<T,Color> colormap, Color defaultColor){
        pixelColors = new Color[map.Width,map.Height];
        width = map.Width;
        height = map.Height;

        foreach(var p in map){
            pixelColors[p.X,p.Y] = colormap.GetValueOrDefault(map.ValueAt(p), defaultColor);
        }
        QueueRedraw();
    }

    public override void _Draw()
    {
        GD.Print($"Draw {width} {height}");
        for(int x = 0; x < width; x++){
            for(int y = 0; y < width; y++){
                DrawRect(new(x*pixelDim, y*pixelDim, pixelDim, pixelDim), pixelColors[x,y]);
            }
        }
    }
}
