using System.Diagnostics;

// nugetで追加 //
using System.Device.Gpio;
using nanoFramework.Hardware.Esp32;

namespace GPIOSample_ValueChangedEvent
{
    public class Program
    {
        /// <summary>
        /// GPIO18ピンのインスタンス
        /// </summary>
        private static GpioPin GPIO18;
        /// <summary>
        /// GPIO19ピンのインスタンス
        /// </summary>
        private static GpioPin GPIO19;


        public static void Main()
        {
            // GpioControllerの作成 (GPIOの設定や操作を行う)
            GpioController gpioController = new GpioController();

            // GPIO18を開く
            GPIO18 = gpioController.OpenPin(Gpio.IO18);
            // GPIO18を出力に設定
            GPIO18.SetPinMode(PinMode.Output);

            // GPIO19を開く
            GPIO19 = gpioController.OpenPin(Gpio.IO19);
            // GPIO19を入力(プルアップ)に設定
            GPIO19.SetPinMode(PinMode.InputPullUp);

            // GPIO19にピン状態変化時のイベントを追加
            GPIO19.ValueChanged += GPIO19_ValueChanged;

            while (true)
            {
                // 何もしない
            }
        }

        private static void GPIO19_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            // イベントからGPIO19ピンの状態を確認
            Debug.WriteLine("Button (event) : " + e.ChangeType.ToString());

            // GPIO19ピンの状態を直接読み取る
            Debug.WriteLine("Button (direct) : " + GPIO19.Read());

            // GPIO19がLowからHighになるとき
            if (e.ChangeType == PinEventTypes.Rising)
            {
                GPIO18.Write(PinValue.High);
            }
            else
            {
                GPIO18.Write(PinValue.Low);
            }
        }
    }
}
