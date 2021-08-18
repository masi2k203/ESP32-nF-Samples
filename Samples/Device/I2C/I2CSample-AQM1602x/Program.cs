using System.Threading;

// nugetで追加 //
using System.Device.Gpio;
using System.Device.I2c;
using nanoFramework.Hardware.Esp32;

namespace I2CSample_AQM1602x
{
    public class Program
    {
        /// <summary>
        /// GPIO19に接続されたタクトスイッチ
        /// </summary>
        private static GpioPin PushButton;

        /// <summary>
        /// AQM1602x (LCD)
        /// </summary>
        private static AQM1602x LCD;

        /// <summary>
        /// ループ回数を計測するカウンター
        /// </summary>
        private static int _counter = 0;

        public static void Main()
        {
            // GpioControllerの作成 (GPIOの設定や操作を行う)
            GpioController gpioController = new GpioController();

            // GPIO19を入力(プルアップ)に設定
            PushButton = gpioController.OpenPin(Gpio.IO19, PinMode.InputPullUp);

            // GPIO19にピン状態変化時のイベントを追加
            PushButton.ValueChanged += PushButton_ValueChanged;

            // アドレスが0x3EのI2Cデバイスを準備 (引数 : I2Cバス番号, I2Cデバイスアドレス, バススピード)
            I2cDevice i2CDevice_0x3E = new I2cDevice(new I2cConnectionSettings(1, 0x3E, I2cBusSpeed.StandardMode));

            // GPIO21にSDA機能、GPIO22にSCL機能を与える
            Configuration.SetPinFunction(Gpio.IO21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(Gpio.IO22, DeviceFunction.I2C1_CLOCK);

            // AQM1602xインスタンスを作成 (3.3V電源を使用するのでfalse)
            LCD = new AQM1602x(i2CDevice_0x3E, false);

            while (true)
            {
                // カウンターの値をLCDに表示
                string output = _counter.ToString();
                LCD.Write(output);

                // 10ms待つ
                Thread.Sleep(10);

                // カウンターを1増やす
                _counter++;

                // 表示を消す
                LCD.Clear();
            }
        }

        /// <summary>
        /// GPIO19でのValueChangedイベント処理
        /// GPIO19の値が変化するたびにこのメソッドが実行される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PushButton_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            // タクトスイッチが押されたらカウンターを0に戻す
            if (e.ChangeType == PinEventTypes.Falling)
            {
                _counter = 0;
            }
        }
    }
}
