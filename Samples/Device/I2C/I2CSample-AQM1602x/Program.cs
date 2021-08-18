using System.Threading;

// nuget�Œǉ� //
using System.Device.Gpio;
using System.Device.I2c;
using nanoFramework.Hardware.Esp32;

namespace I2CSample_AQM1602x
{
    public class Program
    {
        /// <summary>
        /// GPIO19�ɐڑ����ꂽ�^�N�g�X�C�b�`
        /// </summary>
        private static GpioPin PushButton;

        /// <summary>
        /// AQM1602x (LCD)
        /// </summary>
        private static AQM1602x LCD;

        /// <summary>
        /// ���[�v�񐔂��v������J�E���^�[
        /// </summary>
        private static int _counter = 0;

        public static void Main()
        {
            // GpioController�̍쐬 (GPIO�̐ݒ�⑀����s��)
            GpioController gpioController = new GpioController();

            // GPIO19�����(�v���A�b�v)�ɐݒ�
            PushButton = gpioController.OpenPin(Gpio.IO19, PinMode.InputPullUp);

            // GPIO19�Ƀs����ԕω����̃C�x���g��ǉ�
            PushButton.ValueChanged += PushButton_ValueChanged;

            // �A�h���X��0x3E��I2C�f�o�C�X������ (���� : I2C�o�X�ԍ�, I2C�f�o�C�X�A�h���X, �o�X�X�s�[�h)
            I2cDevice i2CDevice_0x3E = new I2cDevice(new I2cConnectionSettings(1, 0x3E, I2cBusSpeed.StandardMode));

            // GPIO21��SDA�@�\�AGPIO22��SCL�@�\��^����
            Configuration.SetPinFunction(Gpio.IO21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(Gpio.IO22, DeviceFunction.I2C1_CLOCK);

            // AQM1602x�C���X�^���X���쐬 (3.3V�d�����g�p����̂�false)
            LCD = new AQM1602x(i2CDevice_0x3E, false);

            while (true)
            {
                // �J�E���^�[�̒l��LCD�ɕ\��
                string output = _counter.ToString();
                LCD.Write(output);

                // 10ms�҂�
                Thread.Sleep(10);

                // �J�E���^�[��1���₷
                _counter++;

                // �\��������
                LCD.Clear();
            }
        }

        /// <summary>
        /// GPIO19�ł�ValueChanged�C�x���g����
        /// GPIO19�̒l���ω����邽�тɂ��̃��\�b�h�����s�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PushButton_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            // �^�N�g�X�C�b�`�������ꂽ��J�E���^�[��0�ɖ߂�
            if (e.ChangeType == PinEventTypes.Falling)
            {
                _counter = 0;
            }
        }
    }
}
