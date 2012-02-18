using System;
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
        private LinkedList<InputVirtualKey> holdingButtonsList = new LinkedList<InputVirtualKey>();
        private List<InputVirtualKey> buttonsToHold = new List<InputVirtualKey>();

        /// <summary>
        /// Кнопки клавиатуры.
        /// </summary>
        private Dictionary<Key, string> keyToKeyName = new Dictionary<Key, string>();
        /// <summary>
        /// Оси геймпада.
        /// </summary>
        private Dictionary<KeyValuePair<JoystickAxis, int>, string> axisToKeyName = new Dictionary<KeyValuePair<JoystickAxis, int>, string>();
        /// <summary>
        /// Кнопки геймпада.
        /// </summary>
        private Dictionary<JoystickButton, string> buttonToKeyName = new Dictionary<JoystickButton, string>();
        private Dictionary<string, string> keyNameToFunctionName = new Dictionary<string, string>();
        private Dictionary<string, InputVirtualKey> functionNameToInputVirtualKey = new Dictionary<string, InputVirtualKey>();

        public LinkedListNode<InputVirtualKey> FirstHoldingButton
        {
            get { return holdingButtonsList.First; }
        }

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

            // заполняем словарь кнопок ключами
            // по умолчанию все кнопки не нажаты
            foreach (InputVirtualKey key in Enum.GetValues(typeof(InputVirtualKey)))
            {
                pressed.Add(key, false);
            }

            // хардкод клавиш, которые должны удерживаться для генерирования сообщений
            // TODO: в будущем нужно иметь не список клавиш, а список виртуальных действий,
            // которые требуют удерживания клавиш, на которые динамически отображаются виртуальные коды клавиш
            buttonsToHold.Add(InputVirtualKey.AxisLeft);
            buttonsToHold.Add(InputVirtualKey.AxisRight);
            buttonsToHold.Add(InputVirtualKey.Action7);
            buttonsToHold.Add(InputVirtualKey.Action8);

            CreateButtonsMapping("Config/Controls.ini");
        }

        private void CreateButtonsMapping(string path)
        {
            IniProcessor ini = new IniProcessor(path);
            ini.ReadFile();

            GetDeviceAxisMapping<JoystickAxis>(JoystickAxis.Axis0, ini, "Controls.Gamepad", "Positive");
            GetDeviceAxisMapping<JoystickAxis>(JoystickAxis.Axis0, ini, "Controls.Gamepad", "Negative");
            GetDeviceAxisMapping<JoystickButton>(JoystickButton.Button0, ini, "Controls.Gamepad", "");
            GetDeviceAxisMapping<Key>(Key.A, ini, "Controls.Keyboard", "");

            functionNameToInputVirtualKey.Add("ShootLeft".ToLower(), InputVirtualKey.Action1);
            functionNameToInputVirtualKey.Add("ShootBack".ToLower(), InputVirtualKey.Action3);
            functionNameToInputVirtualKey.Add("ShootRight".ToLower(), InputVirtualKey.Action2);
            functionNameToInputVirtualKey.Add("IncreaseSpeed".ToLower(), InputVirtualKey.AxisUp);
            functionNameToInputVirtualKey.Add("DecreaseSpeed".ToLower(), InputVirtualKey.AxisDown);
            functionNameToInputVirtualKey.Add("TurnLeft".ToLower(), InputVirtualKey.AxisLeft);
            functionNameToInputVirtualKey.Add("TurnRight".ToLower(), InputVirtualKey.AxisRight);
            functionNameToInputVirtualKey.Add("ZoomIn".ToLower(), InputVirtualKey.Action7);
            functionNameToInputVirtualKey.Add("ZoomOut".ToLower(), InputVirtualKey.Action8);
            functionNameToInputVirtualKey.Add("Escape".ToLower(), InputVirtualKey.Action17);
        }

        private void GetDeviceAxisMapping<T>(T buttonsType, IniProcessor ini, string section, string suffix)
        {
            Type t = typeof(T);
            Dictionary<T, List<string>> tempKeyToKeyNameList = new Dictionary<T, List<string>>();
            foreach (string keyName in Enum.GetNames(typeof(T)))
            {
                T key = (T)Enum.Parse(typeof(T), keyName);

                if (!tempKeyToKeyNameList.ContainsKey(key))
                {
                    List<string> keyNames = new List<string>();
                    keyNames.Add(keyName.ToLower() + suffix.ToLower());
                    tempKeyToKeyNameList.Add(key, keyNames);
                }
                else
                {
                    List<string> keyNames = tempKeyToKeyNameList[key];
                    keyNames.Add(keyName.ToLower() + suffix.ToLower());
                }
            }

            Dictionary<T, List<string>>.ValueCollection keyLists = tempKeyToKeyNameList.Values;
            foreach (List<string> keyNames in keyLists)
            {
                foreach (string keyName in keyNames)
                {
                    string functionName = ini.GetValue(section, keyName, "");

                    if (!String.IsNullOrEmpty(functionName))
                        keyNameToFunctionName.Add(keyName.ToLower(), functionName.ToLower());
                }
            }

            // теперь заполняем словарь соответствия кнопок названию кнопки только для тех кнопок, которые обозначены в настройках
            // другие кнопки будут игнорироваться, поэтому словарь будет содержать не 150 элементов, а пару десятков
            foreach (T key in tempKeyToKeyNameList.Keys)
            {
                foreach (string keyName in tempKeyToKeyNameList[key])
                {
                    if (keyNameToFunctionName.ContainsKey(keyName))
                    {
                        if (t == typeof(Key))
                            if (!keyToKeyName.ContainsKey((Key)(object)key))
                                keyToKeyName.Add((Key)(object)key, keyName.ToLower());

                        if (t == typeof(JoystickAxis))
                        {
                            KeyValuePair<JoystickAxis, int> pair = new KeyValuePair<JoystickAxis, int>((JoystickAxis)(object)key, suffix == "Positive" ? 1 : -1);
                            if (!axisToKeyName.ContainsKey(pair))
                                axisToKeyName.Add(pair, keyName.ToLower());
                        }

                        if (t == typeof(JoystickButton))
                            if (!buttonToKeyName.ContainsKey((JoystickButton)(object)key))
                                buttonToKeyName.Add((JoystickButton)(object)key, keyName.ToLower());
                    }

                }
            }
        }

        /// <summary>
        /// Обрабатывает события отпускания клавиши на клавиатуре.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                // нам нет смысла искать предварительно клавишу в списке,
                // так как она сначала ищется за время O(n),
                // а затем удаляется за время O(n)
                // если её нет в списке, то Remove просто вернёт false

                //if (buttonsToHold.Contains(key))
                // проверка лишняя, так как список всех возможных нажатых клавиш наверняка
                //короче списка нажатых в данный момент
                holdingButtonsList.Remove(key);

                pressed[key] = false;
                //MessageDispatcher.Post(new ButtonPress(key));
            }
            MessageDispatcher.Post(new ButtonUp(key));
        }

        /// <summary>
        /// Обрабатывает события нажатия клавиши на клавиатуре.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            //if (e.Key == Key.Escape)
            //    owner.Exit();

            InputVirtualKey key = TranslateInput(e.Key);

            // строго говоря, эта проверка не нужна
            if (!pressed[key])
            {
                PutKeyToHoldingButtonsList(key);
                pressed[key] = true;
                MessageDispatcher.Post(new ButtonDown(key));
            }
        }

        private void PutKeyToHoldingButtonsList(InputVirtualKey key)
        {
            if (buttonsToHold.Contains(key) && !holdingButtonsList.Contains(key))
                holdingButtonsList.AddLast(key);
        }

        public bool Pressed(InputVirtualKey key)
        {
            return pressed[key];
        }

        private void JoystickMove(object sender, JoystickMoveEventArgs e)
        {
            InputVirtualKey axisDown = JoystickAxisDown(e.Axis, e.Value);
            InputVirtualKey axisUp = JoystickAxisUp(e.Axis, e.Value);

            if (axisDown != InputVirtualKey.Unknown && !pressed[axisDown])
                MessageDispatcher.Post(new ButtonDown(axisDown));

            if (axisUp != InputVirtualKey.Unknown && pressed[axisUp])
                MessageDispatcher.Post(new ButtonUp(axisUp));

            // не кошерно, нужен лок
            pressed[axisDown] = true;
            pressed[axisUp] = false;

            holdingButtonsList.Remove(axisUp);

            if (buttonsToHold.Contains(axisDown) && !holdingButtonsList.Contains(axisDown))
                holdingButtonsList.AddLast(axisDown);

            //Console.WriteLine("AxisUp pressed: " + pressed[InputVirtualKey.AxisUp]);
            //owner.Title = e.Axis.ToString() + ": " + e.Value;
        }

        private void JoystickButtonDown(object sender, JoystickButtonEventArgs e)
        {
            InputVirtualKey key = TranslateInput(e.Button);

            // строго говоря, эта проверка не нужна
            if (!pressed[key])
            {
                PutKeyToHoldingButtonsList(key);
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
                holdingButtonsList.Remove(key);
                pressed[key] = false;

                //MessageDispatcher.Post(new ButtonPress(key));
            }
            MessageDispatcher.Post(new ButtonUp(key));
            //owner.Title = e.Button.ToString();
        }

        private InputVirtualKey JoystickAxisDown(JoystickAxis axis, float value)
        {
            if (value != 0)
            {
                int sign = Math.Sign(value);
                KeyValuePair<JoystickAxis, int> pair = new KeyValuePair<JoystickAxis, int>(axis, sign);
                if (axisToKeyName.ContainsKey(pair))
                    return functionNameToInputVirtualKey[keyNameToFunctionName[axisToKeyName[pair]]];
                else
                    return InputVirtualKey.Unknown;
            }

            return InputVirtualKey.Unknown;
        }

        private InputVirtualKey JoystickAxisUp(JoystickAxis axis, float value)
        {
            if (value == 0)
            {
                KeyValuePair<JoystickAxis, int> positivePair = new KeyValuePair<JoystickAxis, int>(axis, 1);
                KeyValuePair<JoystickAxis, int> negativePair = new KeyValuePair<JoystickAxis, int>(axis, -1);
                if (axisToKeyName.ContainsKey(positivePair) &&
                    pressed[functionNameToInputVirtualKey[keyNameToFunctionName[axisToKeyName[positivePair]]]]
                    ) // отпустили положительое направление
                    return functionNameToInputVirtualKey[keyNameToFunctionName[axisToKeyName[positivePair]]];
                else
                    if (axisToKeyName.ContainsKey(negativePair) &&
                        pressed[functionNameToInputVirtualKey[keyNameToFunctionName[axisToKeyName[negativePair]]]]
                        )
                        return functionNameToInputVirtualKey[keyNameToFunctionName[axisToKeyName[negativePair]]]; // отпустили отрицательное направление
                    else
                        return InputVirtualKey.Unknown;
            }

            return InputVirtualKey.Unknown;
        }

        /// <summary>
        /// Переводит нажатую кнопку на джойстике в виртуальную кнопку
        /// </summary>
        /// <param name="button">Кнопка джойстика</param>
        /// <returns>Виртуальная кнопка</returns>
        private InputVirtualKey TranslateInput(JoystickButton button)
        {
            InputVirtualKey key = InputVirtualKey.Unknown;

            if (buttonToKeyName.ContainsKey(button))
                return functionNameToInputVirtualKey[keyNameToFunctionName[buttonToKeyName[button]]];

            //switch (button)
            //{
            //    // Крест
            //    case JoystickButton.Button0:
            //        key = InputVirtualKey.Action3;
            //        break;
            //    // Квадрат
            //    case JoystickButton.Button1:
            //        key = InputVirtualKey.Action1;
            //        break;
            //    // Круг
            //    case JoystickButton.Button2:
            //        key = InputVirtualKey.Action2;
            //        break;
            //    // Треугольник
            //    case JoystickButton.Button3:
            //        key = InputVirtualKey.Action4;
            //        break;
            //    // L1
            //    case JoystickButton.Button4:
            //        key = InputVirtualKey.Action5;
            //        break;
            //    // L2
            //    case JoystickButton.Button5:
            //        key = InputVirtualKey.Action6;
            //        break;
            //    // R1
            //    case JoystickButton.Button6:
            //        key = InputVirtualKey.Action7;
            //        break;
            //    // R2
            //    case JoystickButton.Button7:
            //        key = InputVirtualKey.Action8;
            //        break;
            //    // Нижний левый шифт
            //    case JoystickButton.Button8:
            //        key = InputVirtualKey.Action9;
            //        break;
            //    // Нижний правый шифт
            //    case JoystickButton.Button9:
            //        key = InputVirtualKey.Action10;
            //        break;
            //    // L3
            //    case JoystickButton.Button10:
            //        key = InputVirtualKey.Action11;
            //        break;
            //    // R3
            //    case JoystickButton.Button11:
            //        key = InputVirtualKey.Action12;
            //        break;
            //}

            return key;
        }

        /// <summary>
        /// Переводит нажатую кнопку на клавиатуре в виртуальную кнопку
        /// </summary>
        /// <param name="button">Кнопка клавиатуры</param>
        /// <returns>Виртуальная кнопка</returns>
        private InputVirtualKey TranslateInput(Key button)
        {
            InputVirtualKey key = InputVirtualKey.Unknown;

            if (keyToKeyName.ContainsKey(button))
                return functionNameToInputVirtualKey[keyNameToFunctionName[keyToKeyName[button]]];

            return key;
        }

        /// <summary>
        /// Посылаем сообщение в эфир, что данная кнопка до сих пор нажата.
        /// </summary>
        /// <param name="inputVirtualKey">Удерживаемая кнопка.</param>
        internal void ProcessHoldingKey(InputVirtualKey inputVirtualKey, double dt)
        {
            MessageDispatcher.Post(new ButtonHold(inputVirtualKey, dt));
        }
    }
}

