# AccelerometerGamePadAVR
USB gamepad project based on ATmega328P and ADXL335 accelerometer module.

Used libraries:
- VUSB by Objective Development - http://www.obdev.at/products/vusb/index.html
- I2C Master Interface by Peter Fleury - http://homepage.hispeed.ch/peterfleury/index.html

To compile firmware simply import project into Eclipse (with AVR plugin) workspace.

LFuse: EE HFuse: D1 EFuse: 07

AccelerometerGamePadUtility is application that can configure various gamepad settings (dead zones, assigment of axes, etc.)