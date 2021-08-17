using System.Threading;

// nugetで追加 //
using System.Device.Gpio;
using nanoFramework.Hardware.Esp32;

namespace GPIOSample_Basic
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

            // GPIO18をHIGHにする
            GPIO18.Write(PinValue.High);

            // 1000ms待つ
            Thread.Sleep(1000);

            // GPIO18をLOWにする
            GPIO18.Write(PinValue.Low);

            while (true)
            {
                // GPIO19がLowの時(ボタンが押された時)に処理をする
                if (GPIO19.Read() == PinValue.Low)
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
}
