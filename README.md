# AccelerometerGamePadAVR
USB gamepad project based on ATmega328P and ADXL335 accelerometer module.

Firmware uses VUSB library to handle USB communication. (http://www.obdev.at/products/vusb/index.html)
To compile firmware simply import project into Eclipse (with AVR plugin) workspace.

AccelerometerGamePadUtility is application that can configure various gamepad settings (dead zones, assigment of axes, etc.)