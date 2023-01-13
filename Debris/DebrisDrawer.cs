using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GazeOGL.Debris
{
    public static class DebrisDrawer
    {
        public static DebrisSet Generate(Texture2D texture, int pieceCount, int seed = 42)
        {
            Random random = new Random(seed);
            int length = Math.Max(texture.Bounds.Width, texture.Bounds.Height);
            Vector2 center = texture.Bounds.Size.ToVector2() * 0.5f;
            float iniDir = (float)random.NextDouble() * (float)Math.PI * 2f;
            Vector2[] corners = new Vector2[]
            {
                Vector2.Zero,
                Vector2.UnitX * texture.Bounds.Width,
                Vector2.UnitX * texture.Bounds.Width + Vector2.UnitY * texture.Bounds.Height,
                Vector2.UnitY * texture.Bounds.Height
            };
            Line[] edges = new Line[]
            {
                new Line( corners[0], corners[1]),
                new Line( corners[1], corners[2]),
                new Line( corners[2], corners[3]),
                new Line( corners[3], corners[0])
            };
            float[] directions = new float[pieceCount];
            Line[] lines = new Line[pieceCount];
            Vector2[] points = new Vector2[pieceCount];
            for(int i =0; i < pieceCount; i++)
            {
                directions[i] = iniDir + ((float)i / (float)pieceCount) * (float)Math.PI * 2f + (float)random.NextDouble() * (3f * (float)Math.PI / ( 4f *pieceCount)) - (1.5f * (float)Math.PI / (4f * pieceCount));
                lines[i] = new Line(center, center + Functions.PolarVector(length, directions[i]));
                Vector2 temp = Vector2.Zero;
                for(int j =0; j < 4; j++)
                {
                    if(edges[j].Colliding(lines[i], ref temp))
                    {
                        points[i] = temp;
                        break;
                    }
                }
            }
            Polygon[] shapes = new Polygon[pieceCount];
            for(int i =0; i < pieceCount; i++)
            {
                int otherI = i+1 >= pieceCount ? 0 : i+1;
                List<Vector2> vertices = new List<Vector2>();
                float[] cornerAngles = new float[] { (corners[0] - center).ToRotation(), (corners[1] - center).ToRotation(), (corners[2] - center).ToRotation(), (corners[3] - center).ToRotation()};
                for(int j =0; j < 4; j++)
                {
                    while(cornerAngles[j] < directions[i])
                    {
                        cornerAngles[j] += (float)Math.PI * 2f;
                    }
                }
                if(otherI == 0)
                {
                    directions[otherI] += (float)Math.PI * 2f;
                }
                float diff = directions[otherI] - directions[i];
                vertices.Add(center);
                vertices.Add(points[i]);
                for(int j =0; j < 4; j++)
                {
                    if(cornerAngles[j] - directions[i] < diff)
                    {
                        vertices.Add(corners[j]);
                    }
                }
                vertices.Add(points[otherI]);
                shapes[i] = new Polygon(vertices.ToArray());
            }
            float[] outputDirections = new float[pieceCount];
            for(int i =0; i < pieceCount;  i++)
            {
                outputDirections[i] = (shapes[i].Center() - center).ToRotation();
            }
            Color[] dataColors = new Color[texture.Width * texture.Height]; //Color array
            texture.GetData(dataColors);
            int width = texture.Width;
            Color[][] outputDataColors = new Color[pieceCount][];
            for(int i =0; i < pieceCount;  i++)
            {
                outputDataColors[i] = new Color[texture.Width * texture.Height];
            }
            for(int x =0; x < width; x++)
            {
                for(int y = 0; y < texture.Height; y++)
                {
                    for(int p=0; p < pieceCount; p++)
                    {
                        if(shapes[p].Colliding(new Vector2(x, y)))
                        {
                            outputDataColors[p][x + y * width] = dataColors[x + y * width];
                        }
                    }
                }
            }
            Texture2D[] outputTextures = new Texture2D[pieceCount];
            for(int p =0; p < pieceCount; p++)
            {
                outputTextures[p] = new Texture2D(Main.device, width, texture.Height);
                outputTextures[p].SetData(0, null, outputDataColors[p], 0, width * texture.Height);
            }
            return new DebrisSet(outputTextures, outputDirections, shapes); 
        }
    }
    
}