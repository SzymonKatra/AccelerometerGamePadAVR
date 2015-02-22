#include "profiles.h"

static profile_t s_currentProfile;
static uint8_t s_currentProfileIndex;

static void profilesLoadProfileGeneric(uint8_t index);
static axisSettings_t* profilesGetAxisSettingsPointerGeneric(uint8_t axis);
static axisRuntime_t* profilesGetAxisRuntimePointerGeneric(uint8_t axis);
static void profilesAxisRawToRuntime(const axisSettings_t* settings, axisRuntime_t* runtime);

void profilesInit(void)
{
	uint8_t current = eepromReadUint8(EEPROM_CURRENT_PROFILE_ADDRESS);
	profilesLoadProfileGeneric(current);
}

void profilesLoadProfile(uint8_t index)
{
	profilesLoadProfileGeneric(index);
	eepromWriteUint8(EEPROM_CURRENT_PROFILE_ADDRESS, s_currentProfileIndex);
}
static void profilesLoadProfileGeneric(uint8_t index)
{
	s_currentProfileIndex = index;

	eepromReadData(PROFILE_EEPROM_ADDRESS(index), &s_currentProfile.settings, sizeof(profileSettings_t));

	profilesAxisRawToRuntime(&s_currentProfile.settings.accelerometerHorizontal, &s_currentProfile.runtime.accelerometerHorizontal);
	profilesAxisRawToRuntime(&s_currentProfile.settings.accelerometerVertical, &s_currentProfile.runtime.accelerometerVertical);

	profilesAxisRawToRuntime(&s_currentProfile.settings.leftAnalogHorizontal, &s_currentProfile.runtime.leftAnalogHorizontal);
	profilesAxisRawToRuntime(&s_currentProfile.settings.leftAnalogVertical, &s_currentProfile.runtime.leftAnalogVertical);

	profilesAxisRawToRuntime(&s_currentProfile.settings.rightAnalogHorizontal, &s_currentProfile.runtime.rightAnalogHorizontal);
	profilesAxisRawToRuntime(&s_currentProfile.settings.rightAnalogVertical, &s_currentProfile.runtime.rightAnalogVertical);
}
void profilesSaveCurrentProfile(void)
{
	profilesSaveCurrentProfileTo(s_currentProfileIndex);
}
void profilesSaveCurrentProfileTo(uint8_t index)
{
	eepromWriteData(PROFILE_EEPROM_ADDRESS(index), &s_currentProfile.settings, sizeof(profileSettings_t));
}
uint8_t profilesGetCurrentProfileIndex(void)
{
	return s_currentProfileIndex;
}

void profilesApplyAxisSettings(uint8_t axis, const axisSettings_t* settings)
{
	axisSettings_t* sett = profilesGetAxisSettingsPointerGeneric(axis);
	axisRuntime_t* runt = profilesGetAxisRuntimePointerGeneric(axis);
	*sett = *settings;
	profilesAxisRawToRuntime(sett, runt);
}
void profilesCopyAxisSettings(uint8_t axis, axisSettings_t* result)
{
	*result = *profilesGetAxisSettingsPointerGeneric(axis);
}

const axisSettings_t* profilesGetAxisSettingsPointer(uint8_t axis)
{
	return profilesGetAxisSettingsPointerGeneric(axis);
}
const axisRuntime_t* profilesGetAxisRuntimePointer(uint8_t axis)
{
	return profilesGetAxisRuntimePointerGeneric(axis);
}

static axisSettings_t* profilesGetAxisSettingsPointerGeneric(uint8_t axis)
{
	switch (axis)
	{
	case ACCELEROMETER_HORIZONTAL: return &s_currentProfile.settings.accelerometerHorizontal;
	case ACCELEROMETER_VERTICAL: return &s_currentProfile.settings.accelerometerVertical;
	case LEFTANALOG_HORIZONTAL: return &s_currentProfile.settings.leftAnalogHorizontal;
	case LEFTANALOG_VERTICAL: return &s_currentProfile.settings.leftAnalogVertical;
	case RIGHTANALOG_HORIZONTAL: return &s_currentProfile.settings.rightAnalogHorizontal;
	case RIGHTANALOG_VERTICAL: return &s_currentProfile.settings.rightAnalogVertical;
	}

	return 0;
}
static axisRuntime_t* profilesGetAxisRuntimePointerGeneric(uint8_t axis)
{
	switch (axis)
	{
	case ACCELEROMETER_HORIZONTAL: return &s_currentProfile.runtime.accelerometerHorizontal;
	case ACCELEROMETER_VERTICAL: return &s_currentProfile.runtime.accelerometerVertical;
	case LEFTANALOG_HORIZONTAL: return &s_currentProfile.runtime.leftAnalogHorizontal;
	case LEFTANALOG_VERTICAL: return &s_currentProfile.runtime.leftAnalogVertical;
	case RIGHTANALOG_HORIZONTAL: return &s_currentProfile.runtime.rightAnalogHorizontal;
	case RIGHTANALOG_VERTICAL: return &s_currentProfile.runtime.rightAnalogVertical;
	}

	return 0;
}
static void profilesAxisRawToRuntime(const axisSettings_t* settings, axisRuntime_t* runtime)
{
	runtime->zeroDeadZone = (double)(settings->zeroDeadZoneRaw) / 100.0;
	runtime->oneDeadZone = (double)(settings->oneDeadZoneRaw) / 100.0;
}

