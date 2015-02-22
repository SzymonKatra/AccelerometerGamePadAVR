#ifndef REPORTS_H_
#define REPORTS_H_

#include "profiles.h"
#include "input.h"
#include "adc.h"

#define REPORT_GET_STATE 1              // device  to  host
#define REPORT_SELECT_AXIS  10          // host    to  device
#define REPORT_GET_SELECTED_AXIS 11     // device  to  host
#define REPORT_SELECT_PROFILE 12        // host    to  device
#define REPORT_GET_SELECTED_PROFILE 13  // device  to  host
#define REPORT_SAVE_CALLIBRATION 18     // host    to  device
#define REPORT_SAVE_PROFILE 19          // host    to  device
#define REPORT_SET_SETTINGS 20          // host    to  device
#define REPORT_GET_SETTINGS 21          // device  to  host
#define REPORT_SET_CALLIBRATION 22      // host    to  device
#define REPORT_GET_CALLIBRATION 23      // device  to  host
#define REPORT_GET_RAW_AXIS 50          // device  to  host

typedef struct
{
	uint8_t reportId;
	uint16_t buttonsAndDPad;
	int8_t x;
	int8_t y;
	int8_t z;
	int8_t rx;
	int8_t ry;
	int8_t rz;
}report_t;

typedef struct
{
	uint8_t reportId;
	axisSettings_t settings;
}settingsReport_t;

typedef struct
{
	uint8_t reportId;
	uint8_t axis;
}selectedAxisReport_t;

typedef struct
{
	uint8_t reportId;
	uint8_t profile;
}selectedProfileReport_t;

typedef struct
{
	uint8_t reportId;
	axisCallibration_t callibration;
}callibrationReport_t;

typedef struct
{
	uint8_t reportId;
	uint16_t value;
}rawAxisReport_t;

void reportsPoolInputAndFillReport(report_t* report);

#endif /* REPORTS_H_ */
