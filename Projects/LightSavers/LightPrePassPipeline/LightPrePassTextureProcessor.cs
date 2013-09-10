using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace LightPrePassProcessor
{
    [ContentProcessor(DisplayName = "Light Pre Pass Texture Processor")]
    public class LightPrePassTextureProcessor : TextureProcessor
    {
        private bool _isCubemap = false;

        public override bool ColorKeyEnabled
        {
            get
            {
                return false;
            }
        }
        public override bool PremultiplyAlpha
        {
            get
            {
                return false;
            }
        }

        public bool IsCubemap
        {
            get { return _isCubemap; }
            set { _isCubemap = value; }
        }

        public override TextureContent Process(TextureContent input, ContentProcessorContext context)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            //check if its a normal map
            Console.WriteLine("Light pre-pass texture processor: " + context.OutputFilename);

            if (context.OutputFilename.Contains("normal"))
            {
                TextureFormat = TextureProcessorOutputFormat.Color;
            }
            else
            {
                TextureFormat = TextureProcessorOutputFormat.DxtCompressed;
            }
            GenerateMipmaps = true;
            //try to parse meta data
            FileInfo fileInfo = new FileInfo(Path.GetDirectoryName(input.Identity.SourceFilename) + "\\" + Path.GetFileNameWithoutExtension(input.Identity.SourceFilename) + ".metadata");
            if (fileInfo.Exists)
            {
                using (FileStream fileStream = fileInfo.OpenRead())
                {
                    StreamReader streamReader = new StreamReader(fileStream);
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        ParseMetaData(line);
                    }
                }
            }
            if (_isCubemap)
            {
                return GenerateCubemap(input, context);
            }
            return base.Process(input, context);
        }

        private TextureContent GenerateCubemap(TextureContent input, ContentProcessorContext context)
        {
            if (input.Faces[1].Count != 0)
            {
                //its already a cubemap
                return base.Process(input, context);
            }
            TextureCubeContent cubeContent = new TextureCubeContent();
            // Convert the input data to Color format, for ease of processing.
            input.ConvertBitmapType(typeof(PixelBitmapContent<Color>));

            int height = input.Faces[0][0].Height;
            int width = input.Faces[0][0].Width / 6;

            //split the image into 6 pieces, setup: X+,X-, Y+,Y-, Z+, Z-
            cubeContent.Faces[(int)CubeMapFace.PositiveX] = CreateFace(input.Faces[0][0], width, height, 0);
            cubeContent.Faces[(int)CubeMapFace.NegativeX] = CreateFace(input.Faces[0][0], width, height, width * 1);
            cubeContent.Faces[(int)CubeMapFace.PositiveY] = CreateFace(input.Faces[0][0], width, height, width * 2);
            cubeContent.Faces[(int)CubeMapFace.NegativeY] = CreateFace(input.Faces[0][0], width, height, width * 3);
            cubeContent.Faces[(int)CubeMapFace.PositiveZ] = CreateFace(input.Faces[0][0], width, height, width * 4);
            cubeContent.Faces[(int)CubeMapFace.NegativeZ] = CreateFace(input.Faces[0][0], width, height, width * 5);

            // Calculate mipmap data.
            cubeContent.GenerateMipmaps(true);

            // Compress the cubemap into DXT1 format.
            cubeContent.ConvertBitmapType(typeof(Dxt1BitmapContent));
            return cubeContent;
        }

        private MipmapChain CreateFace(BitmapContent bitmapContent, int w, int h, int xOffset)
        {
            PixelBitmapContent<Color> result;

            result = new PixelBitmapContent<Color>(w, h);

            Rectangle sourceRegion = new Rectangle(xOffset, 0, w, h);

            Rectangle destinationRegion = new Rectangle(0, 0, w, h);

            BitmapContent.Copy(bitmapContent, sourceRegion, result, destinationRegion);

            return result;
        }

        private void ParseMetaData(string line)
        {
            if (line.Contains("TextureFormat"))
            {
                string format = line.Replace("TextureFormat=", "");
                if (format == "Color")
                    TextureFormat = TextureProcessorOutputFormat.Color;
                else if (format == "DxtCompressed")
                    TextureFormat = TextureProcessorOutputFormat.DxtCompressed;
            }
            else if (line.Contains("TextureType"))
            {
                string textureType = line.Replace("TextureType=", "");
                if (textureType.Contains("Cubemap"))
                {
                    _isCubemap = true;
                }
            }
            else if (line.Contains("NoMipMaps"))
            {
                GenerateMipmaps = false;
            }
        }
    }
}
