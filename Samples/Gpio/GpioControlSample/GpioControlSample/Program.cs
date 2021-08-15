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
        /// GPIO18�ԃs��
        /// </summary>
        private static GpioPin _io18;

        public static void Main()
        {
            // GPIO�R���g���[���̍쐬
            var gpioController = new GpioController();

            // GPIO18���A�E�g�v�b�g�ɐݒ肷��
            _io18 = gpioController.OpenPin(Gpio.IO18);

            // �A�E�g�v�b�g�ɐݒ肷��
            _io18.SetPinMode(PinMode.Output);

            // 1000ms���ƂɃg�O������
            while (true)
            {
                _io18.Toggle();
                Thread.Sleep(1000);
            }
        }
    }
}
