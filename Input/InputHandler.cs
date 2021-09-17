using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Nez;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBJAM9.Input
{
    public class InputHandler
    {
        VirtualIntegerAxis _xAxisInput;
        VirtualIntegerAxis _yAxisInput;

        public VirtualButton JumpButton { get; private set; }
        public VirtualButton ShootButton { get; private set; }
        public VirtualButton StartButton { get; private set; }
        public VirtualButton SelectButton { get; private set; }
        public VirtualButton ConfirmButton { get; private set; }
        public VirtualButton BackButton { get; private set; }

        Vector2 _axialInput; //utility vec2 to hold input values without constantly creating/destroying vec2s
        public int gamepadIndex = 0;
        public InputMapping mapping;
        public Vector2 LeftStickInput
        {
            get
            {
                _axialInput.X = _xAxisInput.Value;
                _axialInput.Y = _yAxisInput.Value;
                return _axialInput;
            }
        }

        private const float A_CONST = -5f;

        public float XInput => _xAxisInput.Value;

        public float YInput => _yAxisInput.Value;



        public bool AnyButtonPressed => JumpButton.IsPressed || ConfirmButton.IsPressed || BackButton.IsPressed;

        public InputHandler(int index)
        {
            this.gamepadIndex = index;
            using (StreamReader reader = new StreamReader("data/input.json"))
            {
                string json = reader.ReadToEnd();
                mapping = JsonConvert.DeserializeObject<List<InputMapping>>(json).Single(m => m.index == index);
            }
            SetupInput();
        }

        public InputHandler(InputMapping mapping)
        {
            gamepadIndex = mapping.index;
            SetupInput();
        }

        /// <summary>
        /// Needs a better way to bind keys, just hard bind for now
        /// </summary>
        public void SetupInput()
        {
            _axialInput = Vector2.Zero;
            // horizontal input from dpad, left stick or keyboard left/right
            _xAxisInput = new VirtualIntegerAxis();
            _xAxisInput.Nodes.Add(new Nez.VirtualAxis.GamePadDpadLeftRight(gamepadIndex));
            _xAxisInput.Nodes.Add(new Nez.VirtualAxis.GamePadLeftStickX(gamepadIndex));
            for (int i = 0; i < mapping.Left.Length; i++)
            {
                _xAxisInput.Nodes.Add(new Nez.VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, (Keys)mapping.Left[i], (Keys)mapping.Right[i]));
            }

            // vertical input from dpad, left stick or keyboard up/down
            _yAxisInput = new VirtualIntegerAxis();
            _yAxisInput.Nodes.Add(new Nez.VirtualAxis.GamePadDpadUpDown(gamepadIndex));
            _yAxisInput.Nodes.Add(new Nez.VirtualAxis.GamePadLeftStickY(gamepadIndex));
            for (int i = 0; i < mapping.Up.Length; i++)
            {
                _yAxisInput.Nodes.Add(new Nez.VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, (Keys)mapping.Up[i], (Keys)mapping.Down[i]));
            }

            //action buttons
            JumpButton = new VirtualButton();
            foreach (var key in mapping.AKey)
            {
                JumpButton.Nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)key));
            }
            foreach (var button in mapping.AButton)
            {
                JumpButton.Nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)button));
            }

            ShootButton = new VirtualButton();
            foreach (var key in mapping.BKey)
            {
                ShootButton.Nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)key));
            }
            foreach (var button in mapping.BButton)
            {
                ShootButton.Nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)button));
            }


            ConfirmButton = new VirtualButton();
            foreach (var key in mapping.AKey)
            {
                ConfirmButton.Nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)key));
            }
            foreach (var key in mapping.StartKey)
            {
                ConfirmButton.Nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)key));
            }
            foreach (var button in mapping.AButton)
            {
                ConfirmButton.Nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)button));
            }
            foreach (var button in mapping.StartButton)
            {
                ConfirmButton.Nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)button));
            }


            BackButton = new VirtualButton();
            foreach (var key in mapping.BKey)
            {
                BackButton.Nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)key));
            }
            foreach (var button in mapping.BButton)
            {
                BackButton.Nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)button));
            }


            StartButton = new VirtualButton();
            foreach (var key in mapping.StartKey)
            {
                StartButton.Nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)key));
            }
            foreach (var button in mapping.StartButton)
            {
                StartButton.Nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)button));
            }


            SelectButton = new VirtualButton();
            foreach (var key in mapping.SelectKey)
            {
                SelectButton.Nodes.Add(new Nez.VirtualButton.KeyboardKey((Keys)key));
            }
            foreach (var button in mapping.SelectButton)
            {
                SelectButton.Nodes.Add(new Nez.VirtualButton.GamePadButton(gamepadIndex, (Buttons)button));
            }
        }
    }
}