using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Effects
{
    public class PaletteSwapPostProcessor : PostProcessor
    {
        public Matrix ColorMatrix => _ColorMatrix;
        Matrix _ColorMatrix;
        Dictionary<Palette, Matrix> colorPalettes = new Dictionary<Palette, Matrix>();

        EffectParameter colorMatrixParam;
        public PaletteSwapPostProcessor(int executionOrder) : base(executionOrder, null)
        {
            InitColorMatrix();
        }

        public override void OnAddedToScene(Scene scene)
        {
            Effect = scene.Content.Load<Effect>("effects/paletteSwap");
            colorMatrixParam = Effect.Parameters["_ColorMatrix"];
            colorMatrixParam.SetValue(_ColorMatrix);
        }

        private void AddColorsToMatrix(Palette palette, Color color0, Color color1, Color color2, Color color3)
        {

            var matrix = new Matrix(color0.R / 255f, color0.G / 255f, color0.B / 255f, color0.A / 255f,
                                    color1.R / 255f, color1.G / 255f, color1.B / 255f, color1.A / 255f,
                                    color2.R / 255f, color2.G / 255f, color2.B / 255f, color2.A / 255f,
                                    color3.R / 255f, color3.G / 255f, color3.B / 255f, color3.A / 255f);
            colorPalettes.Add(palette, matrix);
        }

        public void SetColors(Palette palette)
        {
            _ColorMatrix = colorPalettes[palette];
            colorMatrixParam?.SetValue(_ColorMatrix);
        }


        /// <summary>
        /// Hand writing all the color palettes?
        /// </summary>
        private void InitColorMatrix()
        {
            Color color0 = new Color(0f, 0f, 0f, 1f);
            Color color1 = new Color(0.34f, 0.34f, 0.34f, 1f);
            Color color2 = new Color(0.67f, 0.67f, 0.67f, 1f);
            Color color3 = new Color(1f, 1f, 1f, 1f);

            //grey
            AddColorsToMatrix(Palette.GrayScale, color0, color1, color2, color3);

            //downwell
            color0 = new Color(0f, 0f, 0f, 1f);
            color1 = new Color(0.34f, 0.34f, 0.34f, 1f);
            color2 = new Color(255, 0, 0, 255);
            color3 = new Color(1f, 1f, 1f, 1f);
            AddColorsToMatrix(Palette.Default, color0, color1, color2, color3);

            //gb
            color0 = new Color(52, 104, 86, 255);
            color1 = new Color(36, 49, 55, 255);
            color2 = new Color(136, 192, 112, 255);
            color3 = new Color(224, 248, 208, 255);
            AddColorsToMatrix(Palette.GameGuy, color0, color1, color2, color3);

            //debug
            color0 = new Color(64, 31, 62, 255);
            color1 = new Color(63, 46, 86, 255);
            color2 = new Color(117, 154, 171, 255);
            color3 = new Color(250, 242, 161, 255);
            AddColorsToMatrix(Palette.Purple, color0, color1, color2, color3);


            color0 = new Color(39, 40, 56, 255);
            color1 = new Color(126, 127, 154, 255);
            color2 = new Color(235, 148, 134, 255);
            color3 = new Color(232, 232, 232, 255);
            AddColorsToMatrix(Palette.Soft, color0, color1, color2, color3);

            //color0 = new Color(0f, 0f, 0f, 1f);
            //color1 = new Color(1f, 1f, 1f, 1f);
            //color2 = new Color(1f, 1f, 1f, 1f);
            //color3 = new Color(1f, 1f, 1f, 1f);
            //AddColorsToMatrix(Palette.Monochrome, color0, color1, color2, color3);

            color0 = new Color(38, 0, 33, 255);
            color1 = new Color(0, 191, 243, 255);
            color2 = new Color(237, 0, 140, 255);
            color3 = new Color(218, 234, 236, 255);
            AddColorsToMatrix(Palette.JB4, color0, color1, color2, color3);

            color0 = new Color(247, 231, 198, 255);
            color1 = new Color(214, 142, 73, 255);
            color2 = new Color(166, 55, 37, 255);
            color3 = new Color(51, 30, 80, 255);
            AddColorsToMatrix(Palette.SuperGB, color0, color1, color2, color3);

            color0 = new Color(44, 33, 55, 255);
            color1 = new Color(68, 97, 118, 255);
            color2 = new Color(63, 172, 149, 255);
            color3 = new Color(161, 239, 140, 255);
            AddColorsToMatrix(Palette.Nymph, color0, color1, color2, color3);


            color0 = new Color(119, 51, 231, 255);
            color1 = new Color(44, 44, 150, 255);
            color2 = new Color(231, 134, 134, 255);
            color3 = new Color(247, 190, 247, 255);
            AddColorsToMatrix(Palette.PuffBall, color0, color1, color2, color3);

            //pumpkin
            color0 = new Color(48, 0, 48, 255);
            color1 = new Color(96, 40, 120, 255);
            color2 = new Color(248, 144, 32, 255);
            color3 = new Color(248, 240, 136, 255);
            AddColorsToMatrix(Palette.Pumpkin, color0, color1, color2, color3);

            //ENDESGA
            color0 = new Color(32, 40, 61, 255);
            color1 = new Color(66, 110, 93, 255);
            color2 = new Color(229, 176, 131, 255);
            color3 = new Color(251, 247, 243, 255);
            AddColorsToMatrix(Palette.ENDESGA4, color0, color1, color2, color3);


            //BSoD
            color0 = new Color(0, 27, 88, 255);
            color1 = new Color(169, 169, 168, 255);
            color2 = new Color(8, 79, 221, 255);
            color3 = new Color(251, 247, 243, 255);
            AddColorsToMatrix(Palette.BSoD, color0, color1, color2, color3);

            //MetroidII Super Gameboy
            color0 = new Color(4, 126, 96, 255);
            color1 = new Color(44, 23, 0, 255);
            color2 = new Color(182, 37, 88, 255);
            color3 = new Color(174, 223, 30, 255);
            AddColorsToMatrix(Palette.DIORTEM2, color0, color1, color2, color3);

            //MM4 Super gameboy
            color0 = new Color(66, 103, 142, 255);
            color1 = new Color(16, 37, 51, 255);
            color2 = new Color(111, 158, 223, 255);
            color3 = new Color(206, 206, 206, 255);
            AddColorsToMatrix(Palette.BlueBomber, color0, color1, color2, color3);


            //SPACEBOY
            color0 = new Color(26, 31, 94, 255);
            color1 = new Color(0, 63, 89, 255);
            color2 = new Color(0, 162, 232, 255);
            color3 = new Color(34, 177, 76, 255);
            AddColorsToMatrix(Palette.SPACEBOY, color0, color1, color2, color3);

        }

        public override void Process(RenderTarget2D source, RenderTarget2D destination)
        {
            base.Process(source, destination);
        }
    }

    public enum Palette
    {
        Default,
        GrayScale,
        GameGuy,
        Purple,
        Soft,
        //Monochrome, //removing beacuse it ruins menus
        JB4,
        SuperGB,
        //CGA,
        Nymph,
        PuffBall,
        Pumpkin,
        ENDESGA4,
        BSoD,
        DIORTEM2,
        BlueBomber,
        SPACEBOY
    }
}
