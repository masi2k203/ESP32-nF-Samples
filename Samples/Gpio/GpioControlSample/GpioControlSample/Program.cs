using System;
using System.Diagnostics;
using System.Device.Gpio;
using System.Threading;
using nanoFramework.Hardware.Esp32;

namespace GpioControlSample
{
    public class Program
    {
        /// <summary>
        /// GPIO18番ピン
        /// </summary>
        private static GpioPin _io18;

        public static void Main()
        {
            // GPIOコントローラの作成
            var gpioController = new GpioController();

            // GPIO18をアウトプットに設定する
            _io18 = gpioController.OpenPin(Gpio.IO18);

            // アウトプットに設定する
            _io18.SetPinMode(PinMode.Output);

            // 1000msごとにトグルする
            while (true)
            {
                _io18.Toggle();
                Thread.Sleep(1000);
            }
        }
    }
}
