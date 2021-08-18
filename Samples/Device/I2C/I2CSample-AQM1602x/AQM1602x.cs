using System;
using System.Threading;
using System.Device.I2c;

namespace I2CSample_AQM1602x
{
    class AQM1602x
    {
        #region フィールド

        /// <summary>
        /// I2Cバス上のデバイス
        /// </summary>
        private I2cDevice _device;

        /// <summary>
        /// 5V電源を使用するフラグ
        /// </summary>
        private bool _is5V;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="device">I2cDeviceインスタンス（既に初期化してある必要アリ）</param>
        /// <param name="is5v">5V電源を使用する場合はtrue, 3.3V電源を使用する場合はfalse</param>
        public AQM1602x(I2cDevice device, bool is5v)
        {
            _device = device;
            _is5V = is5v;

            Initialize();
        }

        /// <summary>
        /// LCDを初期化するメソッド
        /// </summary>
        private void Initialize()
        {
            // 初期化用コマンド
            byte[] initializeCommand1 = new byte[]
            {
                0x38, 0x39, 0x14, 0x73, 0x5E, 0x6C
            };
            byte[] initializeCommand2 = new byte[]
            {
                0x38, 0x01, 0x0C
            };

            // 電源電圧によってコマンドを変更
            if (_is5V)
            {
                initializeCommand1[4] = 0x56;
            }

            // コマンド送信
            for (int i = 0; i < initializeCommand1.Length; i++)
            {
                WriteCommand(initializeCommand1[i]);
            }

            // 待機
            Thread.Sleep(200);

            for (int i = 0; i < initializeCommand2.Length; i++)
            {
                WriteCommand(initializeCommand2[i]);
            }
        }

        /// <summary>
        /// LCDにコマンドを送信するメソッド
        /// </summary>
        /// <param name="command">送信するコマンド(1byte)</param>
        /// <returns>送信結果</returns>
        private I2cTransferResult WriteCommand(byte command)
        {
            // AQM1602x I2Cデータ形式
            // I2Cアドレス, コントロールバイト, データバイト

            // 送信i2Cデータ配列を作成 ()
            SpanByte i2cData = new byte[2];
            i2cData[0] = 0x00;      // コントロールバイト  => コマンドモード
            i2cData[1] = command;   // データバイト       => 引数

            // 送信し、送信結果を取得
            var result = _device.Write(i2cData);

            // 1ms待機 (LCDコントローラを確実に設定するため)
            Thread.Sleep(1);

            return result;
        }

        /// <summary>
        /// LCDに1バイト文字を送信するメソッド
        /// </summary>
        /// <param name="data">送信する文字</param>
        /// <returns>送信結果</returns>
        private I2cTransferResult WriteByteData(char data)
        {
            // AQM1602x I2Cデータ形式
            // I2Cアドレス, コントロールバイト, データバイト

            // 送信i2Cデータ配列を作成 ()
            SpanByte i2cData = new byte[2];
            i2cData[0] = 0x40;          // コントロールバイト  => データモード
            i2cData[1] = (byte)data;    // データバイト       => 引数

            // 送信し、送信結果を取得
            var result = _device.Write(i2cData);

            // 1ms待機 (LCDコントローラを確実に設定するため)
            Thread.Sleep(1);

            return result;
        }

        /// <summary>
        /// LCDに文字列を表示するメソッド
        /// 0文字から32文字を表示可能。それ以上は非表示。
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            char[] outLine1 = null;
            char[] outLine2 = null;

            if (text.Length > 16)
            {
                outLine1 = text.ToCharArray(0, 16);
                outLine2 = text.ToCharArray(16, text.Length - 16);

                // 1行目
                for (int i = 0; i < outLine1.Length; i++)
                {
                    WriteByteData(outLine1[i]);
                }
                // 改行
                WriteCommand(0x40 + 0x80);
                // 2行目
                for (int i = 0; i < outLine2.Length; i++)
                {
                    // 2行目は16文字目まで処理する。その後は捨てる
                    if (i < 16)
                    {
                        WriteByteData(outLine2[i]);
                    }
                }
            }
            else
            {
                outLine1 = text.ToCharArray();
                for (int i = 0; i < outLine1.Length; i++)
                {
                    WriteByteData(outLine1[i]);
                }
            }
        }

        /// <summary>
        /// LCDの表示を消去するメソッド
        /// </summary>
        public void Clear()
        {
            WriteCommand(0x01);
        }

    }
}
