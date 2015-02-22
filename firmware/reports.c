/*
 * reports.c
 *
 *  Created on: 18-12-2014
 *      Author: Szymon
 */

#include "reports.h"

static void reportsFillProperAxis(report_t* report, uint8_t axis);

void reportsPoolInputAndFillReport(report_t* report)
{
	reportsFillProperAxis(report, ACCELEROMETER_HORIZONTAL);
	reportsFillProperAxis(report, ACCELEROMETER_VERTICAL);
	reportsFillProperAxis(report, LEFTANALOG_HORIZONTAL);
	reportsFillProperAxis(report, LEFTANALOG_VERTICAL);
	reportsFillProperAxis(report, RIGHTANALOG_HORIZONTAL);
	reportsFillProperAxis(report, RIGHTANALOG_VERTICAL);

	report->buttonsAndDPad = inputReadButtonsAndDPad();
}

static void reportsFillProperAxis(report_t* report, uint8_t axis)
{
	const axisSettings_t* settings =  profilesGetAxisSettingsPointer(axis);
	const axisRuntime_t* runtime = profilesGetAxisRuntimePointer(axis);
	const axisCallibration_t* callibration = inputGetAxisCallibrationPointer(axis);

	uint16_t adcValue = adcReadAxis(axis);

	int8_t result = inputProcessAxisRead(adcValue, settings, runtime, callibration);
	if (AXIS_NEED_INVERT(axis)) result = -result;

	switch (settings->interpretation)
	{
	case INTERPRETATION_X_AXIS: report->x = result; break;
	case INTERPRETATION_Y_AXIS: report->y = result; break;
	case INTERPRETATION_Z_AXIS: report->z = result; break;
	case INTERPRETATION_X_ROTATION: report->rx = result; break;
	case INTERPRETATION_Y_ROTATION: report->ry = result; break;
	case INTERPRETATION_Z_ROTATION: report->rz = result; break;
	}
}
