﻿using System;
using OpenTK.Input;
using OpenTK;
using SeaBattles.Messages;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles
{
    internal class InputLayer
    {
        private GameWindow owner;
        private Dictionary<InputVirtualKey, bool> pressed = new Dictionary<InputVirtualKey, bool>(20);
        private Vector2 joystickPos = new Vector2(0, 0);

        public InputLayer(GameWindow owner)
        {
            this.owner = owner;

            owner.Keyboard.KeyDown += new EventHandler<KeyboardKeyEventArgs>(KeyboardKeyDown);
            owner.Keyboard.KeyUp += new EventHandler<KeyboardKeyEventArgs>(KeyboardKeyUp);

            if (owner.Joysticks.Count > 0)
            {
                owner.Joysticks[0].ButtonDown += new EventHandler<JoystickButtonEventArgs>(JoystickButtonDown);
                owner.Joysticks[0].ButtonUp += new EventHandler<JoystickButtonEventArgs>(JoystickButtonUp);
                owner.Joysticks[0].Move += new EventHandler<JoystickMoveEventArgs>(JoystickMove);
            }

            foreach (InputVirtualKey key in Enum.GetValues(typeof(InputVirtualKey)))
            {
                pressed.Add(key, false);
            }
        }

        private void KeyboardKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.F1)
                if (owner.WindowState == WindowState.Fullscreen)
                    owner.WindowState = WindowState.Normal;
                else
                    owner.WindowState = WindowState.Fullscreen;

            InputVirtualKey key = TranslateInput(e.Key);

            if (pressed[key])
            {
                pressed[key] = false;
                //MessageDispatcher.Post(new ButtonPress(key));
            }
            MessageDispatcher.Post(new ButtonUp(key));
        }

        private void KeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                owner.Exit();

            InputVirtualKey key = TranslateInput(e.Key);

            // строго говоря, эта проверка не нужна
            if (!pressed[key])
            {
                pressed[key] = true;
                MessageDispatcher.Post(new ButtonDown(key));
            }
        }

        public bool Pressed(InputVirtualKey key)
        {
            return pressed[key];
        }

        private void JoystickMove(object sender, JoystickMoveEventArgs e)
        {
            InputVirtualKey axisDown = JoystickAxisDown(e.Axis, e.Value);
            InputVirtualKey axisUp = JoystickAxisUp(e.Axis, e.Value);

            // выключаем противоположное направление
            switch (axisDown)
            {
                case InputVirtualKey.AxisLeft:
                    pressed[InputVirtualKey.AxisRight] = false;
                    break;
                case InputVirtualKey.AxisRight:
                    pressed[InputVirtualKey.AxisLeft] = false;
                    break;
                case InputVirtualKey.AxisUp:
                    pressed[InputVirtualKey.AxisDown] = false;
                    break;
                case InputVirtualKey.AxisDown:
                    pressed[InputVirtualKey.AxisUp] = false;
                    break;
            }

            if (axisDown != InputVirtualKey.Unknown && !pressed[axisDown])
                MessageDispatcher.Post(new ButtonDown(axisDown));

            if (axisUp != InputVirtualKey.Unknown && pressed[axisUp])
                MessageDispatcher.Post(new ButtonUp(axisUp));

            // не кошерно, нужен лок
            pressed[axisDown] = true;
            pressed[axisUp] = false;

            //Console.WriteLine("AxisUp pressed: " + pressed[InputVirtualKey.AxisUp]);
            //owner.Title = e.Axis.ToString() + ": " + e.Value;
        }

        private void JoystickButtonDown(object sender, JoystickButtonEventArgs e)
        {
            InputVirtualKey key = TranslateInput(e.Button);

            // строго говоря, эта проверка не нужна
            if (!pressed[key])
            {
                pressed[key] = true;
                MessageDispatcher.Post(new ButtonDown(key));
            }

            //owner.Title = e.Button.ToString();
        }

        private void JoystickButtonUp(object sender, JoystickButtonEventArgs e)
        {
            InputVirtualKey key = TranslateInput(e.Button);

            if (pressed[key])
            {
                pressed[key] = false;
                //MessageDispatcher.Post(new ButtonPress(key));
            }
            MessageDispatcher.Post(new ButtonUp(key));
            //owner.Title = e.Button.ToString();
        }

        private InputVirtualKey JoystickAxisDown(JoystickAxis axis, float value)
        {
            if (value != 0)
                switch (axis)
                {
                    // горизонтальная ось
                    case JoystickAxis.Axis0:
                        joystickPos.X = value;
                        if (value > 0)
                            return InputVirtualKey.AxisRight;
                        else
                            if (value < 0)
                                return InputVirtualKey.AxisLeft;
                        break;
                    // вертикальная ось
                    case JoystickAxis.Axis1:
                        joystickPos.Y = value;
                        if (value > 0)
                            return InputVirtualKey.AxisUp;
                        else
                            if (value < 0)
                                return InputVirtualKey.AxisDown;
                        break;
                }

            return InputVirtualKey.Unknown;
        }

        private InputVirtualKey JoystickAxisUp(JoystickAxis axis, float value)
        {
            if (value == 0)
                switch (axis)
                {
                    // горизонтальная ось
                    case JoystickAxis.Axis0:
                        if (joystickPos.X < 0)
                        {
                            joystickPos.X = value;
                            return InputVirtualKey.AxisLeft; // отпустили левую стрелку
                        }
                        else if (joystickPos.X > 0)
                        {
                            joystickPos.X = value;
                            return InputVirtualKey.AxisRight; // отпустили правую стрелку
                        }

                        break;
                    // вертикальная ось
                    case JoystickAxis.Axis1:
                        if (joystickPos.Y < 0)
                        {
                            joystickPos.Y = value;
                            return InputVirtualKey.AxisDown; // отпустили стрелку вниз
                        }
                        else if (joystickPos.Y > 0)
                        {
                            joystickPos.Y = value;
                            return InputVirtualKey.AxisUp; // отпустили стрелку вверх
                        }
                        break;
                }
            return InputVirtualKey.Unknown;
        }

        /// <summary>
        /// Переводит нажатую кнопку на джойстике в виртуальную кнопку
        /// </summary>
        /// <param name="button">Кнопка джойстика</param>
        /// <returns>Виртуальная кнопка</returns>
        private static InputVirtualKey TranslateInput(JoystickButton button)
        {
            InputVirtualKey key = InputVirtualKey.Unknown;

            switch (button)
            {
                // Крест
                case JoystickButton.Button0:
                    key = InputVirtualKey.Action1;
                    break;
                // Квадрат
                case JoystickButton.Button1:
                    key = InputVirtualKey.Action2;
                    break;
                // Круг
                case JoystickButton.Button2:
                    key = InputVirtualKey.Action3;
                    break;
                // Треугольник
                case JoystickButton.Button3:
                    key = InputVirtualKey.Action4;
                    break;
                // L1
                case JoystickButton.Button4:
                    key = InputVirtualKey.Action5;
                    break;
                // L2
                case JoystickButton.Button5:
                    key = InputVirtualKey.Action6;
                    break;
                // R1
                case JoystickButton.Button6:
                    key = InputVirtualKey.Action7;
                    break;
                // R2
                case JoystickButton.Button7:
                    key = InputVirtualKey.Action8;
                    break;
                // Нижний левый шифт
                case JoystickButton.Button8:
                    key = InputVirtualKey.Action9;
                    break;
                // Нижний правый шифт
                case JoystickButton.Button9:
                    key = InputVirtualKey.Action10;
                    break;
                // L3
                case JoystickButton.Button10:
                    key = InputVirtualKey.Action11;
                    break;
                // R3
                case JoystickButton.Button11:
                    key = InputVirtualKey.Action12;
                    break;
            }

            return key;
        }

        /// <summary>
        /// Переводит нажатую кнопку на клавиатуре в виртуальную кнопку
        /// </summary>
        /// <param name="button">Кнопка клавиатуры</param>
        /// <returns>Виртуальная кнопка</returns>
        private static InputVirtualKey TranslateInput(Key button)
        {
            InputVirtualKey key = InputVirtualKey.Unknown;

            switch (button)
            {
                case Key.W:
                case Key.Up:
                    key = InputVirtualKey.AxisUp;
                    break;
                case Key.S:
                case Key.Down:
                    key = InputVirtualKey.AxisDown;
                    break;
                case Key.A:
                case Key.Left:
                    key = InputVirtualKey.AxisLeft;
                    break;
                case Key.D:
                case Key.Right:
                    key = InputVirtualKey.AxisRight;
                    break;
                case Key.Z:
                    key = InputVirtualKey.Action1;
                    break;
                case Key.X:
                    key = InputVirtualKey.Action3;
                    break;
                case Key.C:
                    key = InputVirtualKey.Action2;
                    break;
            }

            return key;
        }

    }
}
