/*
 * profiles.h
 *
 *  Created on: 14-11-2014
 *      Author: Szymon
 */

#ifndef PROFILES_H_
#define PROFILES_H_

#include "general/eeprom.h"
#include "common.h"

#define PROFILE_EEPROM_ADDRESS(index) (EEPROM_PROFILES_ADDRESS + (index * sizeof(profileSettings_t)))

#define AXISSETTINGS_FLAGS_TRUNCATE (1 << 0)
#define AXISSETTINGS_FLAGS_INVERT (1 << 1)

typedef struct
{
	uint8_t zeroDeadZoneRaw;
	uint8_t oneDeadZoneRaw;
	uint8_t flags;
	uint8_t interpretation;
}axisSettings_t;

typedef struct
{
	double zeroDeadZone;
	double oneDeadZone;
}axisRuntime_t;

typedef struct
{
	axisSettings_t accelerometerHorizontal;
	axisSettings_t accelerometerVertical;

	axisSettings_t leftAnalogHorizontal;
	axisSettings_t leftAnalogVertical;

	axisSettings_t rightAnalogHorizontal;
	axisSettings_t rightAnalogVertical;
}profileSettings_t;

typedef struct
{
	axisRuntime_t accelerometerHorizontal;
	axisRuntime_t accelerometerVertical;

	axisRuntime_t leftAnalogHorizontal;
	axisRuntime_t leftAnalogVertical;

	axisRuntime_t rightAnalogHorizontal;
	axisRuntime_t rightAnalogVertical;
}profileRuntime_t;

typedef struct
{
	profileSettings_t settings;
	profileRuntime_t runtime;
}profile_t;


void profilesInit(void);

void profilesLoadProfile(uint8_t index);
void profilesSaveCurrentProfile(void);
void profilesSaveCurrentProfileTo(uint8_t index);
uint8_t profilesGetCurrentProfileIndex(void);

void profilesApplyAxisSettings(uint8_t axis, const axisSettings_t* settings);
void profilesCopyAxisSettings(uint8_t axis, axisSettings_t* result);

const axisSettings_t* profilesGetAxisSettingsPointer(uint8_t axis);
const axisRuntime_t* profilesGetAxisRuntimePointer(uint8_t axis);

#endif /* PROFILES_H_ */
