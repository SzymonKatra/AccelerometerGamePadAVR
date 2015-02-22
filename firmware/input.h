#ifndef INPUT_H_
#define INPUT_H_

#include <stdint.h>
#include <math.h>

#include "adc.h"
#include "common.h"
#include "profiles.h"
#include "general/eeprom.h"
#include "general/usart.h"
#include "general/mcp23017.h"

#define MAX_MOVE 127

#define DPAD_ANGLE_0   1
#define DPAD_ANGLE_45  2
#define DPAD_ANGLE_90  3
#define DPAD_ANGLE_135 4
#define DPAD_ANGLE_180 5
#define DPAD_ANGLE_225 6
#define DPAD_ANGLE_270 7
#define DPAD_ANGLE_315 8

// due to construction of gamepad these axes are inverted
#define AXIS_NEED_INVERT(axis) (axis == ACCELEROMETER_HORIZONTAL || axis == LEFTANALOG_HORIZONTAL || axis == RIGHTANALOG_HORIZONTAL)

typedef struct
{
	uint16_t zeroValue;
	uint16_t plusOneVariation;
	uint16_t minusOneVariation;
}axisCallibration_t;

typedef struct
{
	axisCallibration_t accelerometerHorizontal;
	axisCallibration_t accelerometerVertical;

	axisCallibration_t leftAnalogHorizontal;
	axisCallibration_t leftAnalogVertical;

	axisCallibration_t rightAnalogHorizontal;
	axisCallibration_t rightAnalogVertical;
}gamepadCallibration_t;

void inputInit(void);

void inputLoadCallibration();
void inputSaveCallibration();

int8_t inputReadAxis(uint8_t axis);
int8_t inputProcessAxisRead(uint16_t adcValue, const axisSettings_t* settings, const axisRuntime_t* runtime, const axisCallibration_t* callibration);
uint16_t inputReadButtonsAndDPad();
uint16_t inputReadRawButtons();

const axisCallibration_t* inputGetAxisCallibrationPointer(uint8_t axis);

void inputApplyAxisCallibration(uint8_t axis, const axisCallibration_t* callibration);
void inputCopyAxisCallibration(uint8_t axis, axisCallibration_t* result);

#endif /* INPUT_H_ */
