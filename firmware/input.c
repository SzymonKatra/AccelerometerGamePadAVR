
#include "input.h"

static gamepadCallibration_t s_callibration;

static axisCallibration_t* inputGetAxisCallibrationPointerGeneric(uint8_t axis);

void inputInit(void)
{
	inputLoadCallibration();

	adcInit();

	mcp23017Init();
	mcp23017WriteDirectionA(0xFF);
	mcp23017WritePullUpA(0xFF);
	mcp23017WritePolarityA(0xFF);
	mcp23017WriteDirectionB(0xFF);
	mcp23017WritePullUpB(0xFF);
	mcp23017WritePolarityB(0xFF);
}
int8_t inputReadAxis(uint8_t axis)
{
	int8_t result = inputProcessAxisRead(adcReadAxis(axis), profilesGetAxisSettingsPointer(axis), profilesGetAxisRuntimePointer(axis), inputGetAxisCallibrationPointer(axis));
	if (AXIS_NEED_INVERT(axis)) result = -result;
	return result;
}
int8_t inputProcessAxisRead(uint16_t adcValue, const axisSettings_t* settings, const axisRuntime_t* runtime, const axisCallibration_t* callibration)
{
	int8_t result;
	double factor, absFactor;

	//const axisSettings_t* settings = profilesGetAxisSettingsPointer(axis);
	//const axisRuntime_t* runtime = profilesGetAxisRuntimePointer(axis);
	//const axisCallibration_t* callibration = inputGetAxisCallibrationPointer(axis);

	int16_t value = (int16_t)adcValue - (int16_t)callibration->zeroValue;
	factor = (double)value / (double)(value >= 0 ? callibration->plusOneVariation : callibration->minusOneVariation);
	absFactor = fabs(factor);

	if(absFactor < runtime->zeroDeadZone)
		result = 0;
	else if (absFactor >= runtime->oneDeadZone)
		result = fsign(factor) * (int8_t)MAX_MOVE;
	else
	{
		if (!(settings->flags & AXISSETTINGS_FLAGS_TRUNCATE))
		{
			double fValue = (double)value;

			if (value >= 0)
			{
				fValue -= runtime->zeroDeadZone * (double)callibration->plusOneVariation;
				factor = fValue / (runtime->oneDeadZone * (double)callibration->plusOneVariation - runtime->zeroDeadZone * (double)callibration->plusOneVariation);
			}
			else
			{
				fValue += runtime->zeroDeadZone * (double)callibration->minusOneVariation;
				factor = fValue / (runtime->oneDeadZone * (double)callibration->minusOneVariation - runtime->zeroDeadZone * (double)callibration->minusOneVariation);
			}
		}

		result = (int8_t)(factor * (double)MAX_MOVE);
	}

	if (settings->flags & AXISSETTINGS_FLAGS_INVERT) result = -result;

	return result;

	/*
	int16_t value = (int16_t)adcReadAxis(axis) - zeroValue;
	factor = (double)value / (double)variationValue;
	absFactor = fabs(factor);

	if(absFactor < runtime->zeroDeadZone)
		return 0;
	else if (absFactor >= runtime->oneDeadZone)
		return fsign(factor) * (int8_t)MAX_MOVE;
	else return (int8_t)(factor * (double)MAX_MOVE);
	*/
}
uint16_t inputReadButtonsAndDPad()
{
	uint16_t reportResult = 0x0;
	uint16_t result = mcp23017ReadPortsAB();
	reportResult |= result & 0xFFF; // highest four bytes are for D-Pad

	result >>= 12; // now result stores only D-Pad bits

	// D-Pad result bits:
	// left  - 0 offset
	// down  - 1 offset
	// right - 2 offset
	// up    - 3 offset

	uint8_t selectedDPadButtonsCount = 0;
	if (result & (1<<0)) ++selectedDPadButtonsCount;
	if (result & (1<<1)) ++selectedDPadButtonsCount;
	if (result & (1<<2)) ++selectedDPadButtonsCount;
	if (result & (1<<3)) ++selectedDPadButtonsCount;

	// do not check states if 0 buttons are pressed
	// or if pressed more than 2 buttons - this may cause errors
	// opposite directions are checked inside up and right conditions
	if(selectedDPadButtonsCount > 0 && selectedDPadButtonsCount <= 2)
	{
		if (result & (1<<3)) // up
		{
			if (result & (1<<2)) reportResult |= DPAD_ANGLE_45 << 12; // up - right
			else if (result & (1<<0)) reportResult |= DPAD_ANGLE_315 << 12; // up - left - why? first parent if must have ifs to check left and right side
			else if (!(result & (1<<1))) reportResult |= DPAD_ANGLE_0 << 12; // up
		}
		else if (result & (1<<2)) // right
		{
			if (result & (1<<1)) reportResult |= DPAD_ANGLE_135 << 12; // right - down
			else if (!(result & (1<<0))) reportResult |= DPAD_ANGLE_90 << 12; // right
		}
		else if (result & (1<<1)) // down
		{
			if (result & (1<<0)) reportResult |= DPAD_ANGLE_225 << 12; // down - left
			else reportResult |= DPAD_ANGLE_180 << 12; // down
		}
		else if (result & (1<<0)) // left
		{
			if (result & (1<<3)) reportResult |= DPAD_ANGLE_315 << 12; // left - up
			else reportResult |= DPAD_ANGLE_270 << 12; // left
		}
	}

	return reportResult;
}
uint16_t inputReadRawButtons()
{
	return mcp23017ReadPortsAB();
}

const axisCallibration_t* inputGetAxisCallibrationPointer(uint8_t axis)
{
	return inputGetAxisCallibrationPointerGeneric(axis);
}
static axisCallibration_t* inputGetAxisCallibrationPointerGeneric(uint8_t axis)
{
	switch (axis)
	{
	case ACCELEROMETER_HORIZONTAL: return &s_callibration.accelerometerHorizontal;
	case ACCELEROMETER_VERTICAL: return &s_callibration.accelerometerVertical;
	case LEFTANALOG_HORIZONTAL: return &s_callibration.leftAnalogHorizontal;
	case LEFTANALOG_VERTICAL: return &s_callibration.leftAnalogVertical;
	case RIGHTANALOG_HORIZONTAL: return &s_callibration.rightAnalogHorizontal;
	case RIGHTANALOG_VERTICAL: return &s_callibration.rightAnalogVertical;
	}

	return 0;
}

void inputLoadCallibration()
{
	eepromReadData(EEPROM_GAMEPAD_CALLIBRATION_ADDRESS, &s_callibration, sizeof(gamepadCallibration_t));
}
void inputSaveCallibration()
{
	eepromWriteData(EEPROM_GAMEPAD_CALLIBRATION_ADDRESS, &s_callibration, sizeof(gamepadCallibration_t));
}
void inputApplyAxisCallibration(uint8_t axis, const axisCallibration_t* callibration)
{
	axisCallibration_t* target = inputGetAxisCallibrationPointerGeneric(axis);
	*target = *callibration;
}
void inputCopyAxisCallibration(uint8_t axis, axisCallibration_t* result)
{
	axisCallibration_t* callibration = inputGetAxisCallibrationPointerGeneric(axis);
	*result = *callibration;
}
