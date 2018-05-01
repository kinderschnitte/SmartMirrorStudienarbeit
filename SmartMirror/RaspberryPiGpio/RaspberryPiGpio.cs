using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspberryPiGpio
{
    public static class RaspberryPiGpio
    {
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static async Task TriggerButton()
        {
            GpioController gpio = await GpioController.GetDefaultAsync();

            if (gpio == null)
                return;

            using (GpioPin gpioPin = gpio.OpenPin(5))
            {
                gpioPin.SetDriveMode(GpioPinDriveMode.Input);

                gpioPin.SetDriveMode(GpioPinDriveMode.Output);
                gpioPin.Write(GpioPinValue.Low);
                await Task.Delay(50);
                gpioPin.SetDriveMode(GpioPinDriveMode.Input);
            }
        }
    }
}
