#ifndef COMMON_H_
#define COMMON_H_

#include <stdint.h>

#define buttonInit() DDRB &= ~(1<<PB2); PORTB |= (1<<PB2);
#define buttonIsPressed() (!(PINB & (1<<PB2)))

#define ledInit() DDRB |= (1<<PB1);
#define ledOn() PORTB |= (1<<PB1);
#define ledOff() PORTB &= ~(1<<PB1);
#define ledToggle() PORTB ^= (1<<PB1);

#define EEPROM_CURRENT_PROFILE_ADDRESS 0 // size = 1 byte
#define EEPROM_GAMEPAD_CALLIBRATION_ADDRESS 0x1B0 // size = sizeof(gamepadCallibration_t)
#define EEPROM_PROFILES_ADDRESS 0x200 // size of each profile = (profileSettings_t)

#define PROFILECHANGE_LOCKED 0
#define PROFILECHANGE_ALLOWED 1
#define PROFILECHANGE_PROGRESS 2
#define PROFILECHANGE_BEFORE 3
#define PROFILECHANGE_AFTER 4
#define PROFILECHANGE_ALREADYPRESSED 5

// timers count at 20 MHz and prescaler = 1024
#define PROFILECHANGE_TIMER_COUNT 3906 // ~ 200 ms
#define PROFILECHANGE_TIMER_BEFORE_COUNT 9766 // ~ 500 ms
#define PROFILECHANGE_TIMER_AFTER_COUNT 19531 // ~ 1000 ms

// following axis defines are also ADC MUXes
#define ACCELEROMETER_HORIZONTAL 1
#define ACCELEROMETER_VERTICAL 0
#define LEFTANALOG_HORIZONTAL 2
#define LEFTANALOG_VERTICAL 3
#define RIGHTANALOG_HORIZONTAL 4
#define RIGHTANALOG_VERTICAL 5

#define INTERPRETATION_NULL 0
#define INTERPRETATION_X_AXIS 1
#define INTERPRETATION_Y_AXIS 2
#define INTERPRETATION_Z_AXIS 3
#define INTERPRETATION_X_ROTATION 4
#define INTERPRETATION_Y_ROTATION 5
#define INTERPRETATION_Z_ROTATION 6

int8_t fsign(double val);

#endif /* COMMON_H_ */
