using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using System.Collections.Generic;
using System.Text;

namespace GBJAM9.Effects
{
    public class MenuBackgroundScrollMat : Material
    {
        EffectParameter time;
        EffectParameter speed;
        EffectParameter view_projection;
        EffectParameter uv_transform;
        Vector2 xy;
        Vector2 size;

        public MenuBackgroundScrollMat(Effect effect, Vector2 size, float scrollSpeed = 0.5f) : base(effect)
        {
            time = Effect.Parameters["time"];
            speed = Effect.Parameters["speed"];
            speed.SetValue(scrollSpeed);
            view_projection = Effect.Parameters["view_projection"];
            uv_transform = Effect.Parameters["uv_transform"];

            this.SamplerState = SamplerState.LinearWrap;
            this.size = size;
        }

        public override void OnPreRender(Camera camera)
        {
            base.OnPreRender(camera);
            // set the time variable
            time.SetValue(Time.TotalTime);
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, NezGame.designWidth, NezGame.designHeight, 0, 0, 1);
            Matrix uvTransformMtx = GetUVTransform();

            view_projection.SetValue(Matrix.Identity * projection);
            uv_transform.SetValue(Matrix.Invert(uvTransformMtx));
        }

        private Matrix GetView()
        {
            int width = NezGame.designWidth;
            int height = NezGame.designHeight;
            Vector2 origin = new Vector2(width / 2f, height / 2f);

            return
                Matrix.CreateTranslation(-origin.X, -origin.Y, 0f) *
                Matrix.CreateTranslation(-xy.X, -xy.Y, 0f) *
                Matrix.CreateTranslation(origin.X, origin.Y, 0f);
        }
        private Matrix GetUVTransform()
        {
            return
                Matrix.CreateScale(size.X, size.Y, 1f) *
                GetView() *
                Matrix.CreateScale(1f / NezGame.designWidth, 1f / NezGame.designHeight, 1f);
        }
    }
}
