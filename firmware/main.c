#include <avr/io.h>
#include <avr/wdt.h>
#include <avr/interrupt.h>
#include <util/delay.h>
#include <avr/pgmspace.h>
#include <math.h>
#include "vusb/usbdrv.h"

#include "adc.h"
#include "common.h"
#include "input.h"
#include "general/usbutil.h"
#include "reports.h"
#include "general/mcp23017.h"

#include "descriptor.h"

static report_t s_reportBuffer;
static settingsReport_t s_settingsReportBuffer;
static selectedAxisReport_t s_selectedAxisReportBuffer;
static selectedProfileReport_t s_selectedProfileReportBuffer;
static callibrationReport_t s_callibrationReportBuffer;
static rawAxisReport_t s_rawAxisReportBuffer;

static uint8_t s_selectedAxis = 0;
static uint8_t s_idleRate; // repeat rate for keyboards, never used

static uint8_t s_profileChange = PROFILECHANGE_ALLOWED;
static uint8_t s_profileChangeCounter = 0;

void mainInitAll(void);
void reportsInit(void);
void checkProfileChange();

int main(void)
{
	mainInitAll();

    while (1)
    {
        wdt_reset();
        usbPoll();

        checkProfileChange();

        if(usbutilProcessLargeInterrupt() == 0)
        {
            reportsPoolInputAndFillReport(&s_reportBuffer);

            usbutilSetLargeInterrupt(&s_reportBuffer, sizeof(s_reportBuffer));
        }
    }
    return 0;
}

usbMsgLen_t usbFunctionSetup(uint8_t data[8])
{
	usbRequest_t *rq = (void *)data;

    if((rq->bmRequestType & USBRQ_TYPE_MASK) == USBRQ_TYPE_CLASS)
    {
        if(rq->bRequest == USBRQ_HID_GET_REPORT)
        {
        	// wValue: ReportType (highbyte), ReportID (lowbyte)
        	switch(rq->wValue.bytes[0])
        	{
        		case REPORT_GET_STATE:
        		{
        			reportsPoolInputAndFillReport(&s_reportBuffer);

        			usbMsgPtr = (usbMsgPtr_t)&s_reportBuffer;
        			return sizeof(report_t);
        		}

        		case REPORT_GET_SETTINGS:
        		{
        			profilesCopyAxisSettings(s_selectedAxis, &s_settingsReportBuffer.settings);

        			usbMsgPtr = (usbMsgPtr_t)&s_settingsReportBuffer;
        			return sizeof(settingsReport_t);
        		}

        		case REPORT_GET_CALLIBRATION:
        		{
        			inputCopyAxisCallibration(s_selectedAxis, &s_callibrationReportBuffer.callibration);

        			usbMsgPtr = (usbMsgPtr_t)&s_callibrationReportBuffer;
        			return sizeof(callibrationReport_t);
        		}

        		case REPORT_GET_SELECTED_AXIS:
        		{
        			s_selectedAxisReportBuffer.axis = s_selectedAxis;

        			usbMsgPtr = (usbMsgPtr_t)&s_selectedAxisReportBuffer;
        			return sizeof(selectedAxisReport_t);
        		}

        		case REPORT_GET_SELECTED_PROFILE:
        		{
        			s_selectedProfileReportBuffer.profile = profilesGetCurrentProfileIndex();

        			usbMsgPtr = (usbMsgPtr_t)&s_selectedProfileReportBuffer;
        			return sizeof(selectedProfileReport_t);
        		}

        		case REPORT_GET_RAW_AXIS:
        		{
        			s_rawAxisReportBuffer.value = adcReadAxis(s_selectedAxis);

        			usbMsgPtr = (usbMsgPtr_t)&s_rawAxisReportBuffer;
        			return sizeof(rawAxisReport_t);
        		}
        	}
        }
        else if(rq->bRequest == USBRQ_HID_SET_REPORT)
        {
        	usbutilStartWriting((uint8_t)rq->wLength.word);
        	return USB_NO_MSG; // uses usbFunctionWrite
        }
        else if(rq->bRequest == USBRQ_HID_GET_IDLE)
        {
            usbMsgPtr = (unsigned short)&s_idleRate;
            return 1;
        }
        else if(rq->bRequest == USBRQ_HID_SET_IDLE)
        {
            s_idleRate = rq->wValue.bytes[1];
        }
    }
    else
    {
        /* no vendor specific requests implemented */
    }

    return 0;
}
void usbutilWriteCompleted(uint8_t* buffer, uint8_t len)
{
	switch (buffer[0]) // buffer[0] is report id
	{
		case REPORT_SET_SETTINGS:
		{
			settingsReport_t* report = (settingsReport_t*)buffer;
			profilesApplyAxisSettings(s_selectedAxis, &report->settings);
			break;
		}

		case REPORT_SET_CALLIBRATION:
		{
			callibrationReport_t* report = (callibrationReport_t*)buffer;
			inputApplyAxisCallibration(s_selectedAxis, &report->callibration);
			break;
		}

		case REPORT_SELECT_AXIS:
		{
			selectedAxisReport_t* report = (selectedAxisReport_t*)buffer;
			s_selectedAxis = report->axis;
			break;
		}

		case REPORT_SELECT_PROFILE:
		{
			selectedProfileReport_t* report = (selectedProfileReport_t*)buffer;
			profilesLoadProfile(report->profile);
			break;
		}

		case REPORT_SAVE_CALLIBRATION:
		{
			inputSaveCallibration();
			break;
		}

		case REPORT_SAVE_PROFILE:
		{
			profilesSaveCurrentProfile();
			break;
		}
	}
}

void mainInitAll(void)
{
	wdt_enable(WDTO_1S);
	wdt_reset();

	ledInit();
	ledOff();

	buttonInit();

	// setting not used pins as inputs with pull-up
	// DDRx registers equals 0 as default so all pins are already input
	// we only need to set bits in PORTx registers to enable pull-ups
	PORTB |= (1<<PB3) | (1<<PB4) | (1<<PB5);
	PORTD |= (1<<PD0) | (1<<PD1) | (1<<PD3) | (1<<PD5) | (1<<PD6);

	profilesInit();
	reportsInit();
	inputInit();

	// here we need D+ (PD2) and D- (PD4) default state - input without pull-up
	usbInit();
	usbDeviceDisconnect();  // enforce re-enumeration, do this while interrupts are disabled!
	uint8_t i = 250;
	while(i--)
	{
		// fake USB disconnect for ~ 250 ms
		wdt_reset();
		_delay_ms(1);

		if (i % 30 == 0) ledToggle();
	}
	usbDeviceConnect();
	sei();

	ledOn();

	// initializing timer for led flashing
	TCCR1B |= (1<<CS12) | (1<<CS10); // prescaler = 1024
	TCCR1B |= (1<<WGM12); // ctc mode - compares with OCR1A

	/*
	axisCallibration_t callib;
	callib.zeroValue=655;
	callib.plusOneVariation = 123;
	callib.minusOneVariation = 123;
	inputApplyAxisCallibration(ACCELEROMETER_HORIZONTAL, &callib);
	inputApplyAxisCallibration(ACCELEROMETER_VERTICAL, &callib);

	callib.zeroValue=410;
	callib.plusOneVariation = 410;
	callib.minusOneVariation = 410;
	inputApplyAxisCallibration(LEFTANALOG_HORIZONTAL, &callib);
	inputApplyAxisCallibration(LEFTANALOG_VERTICAL, &callib);
	inputApplyAxisCallibration(RIGHTANALOG_HORIZONTAL, &callib);
	inputApplyAxisCallibration(RIGHTANALOG_VERTICAL, &callib);

	inputSaveCallibration();
	*/

}
void reportsInit(void)
{
	s_reportBuffer.reportId = REPORT_GET_STATE;
	s_reportBuffer.buttonsAndDPad = 0;
	s_reportBuffer.x = 0;
	s_reportBuffer.y = 0;
	s_reportBuffer.z = 0;
	s_reportBuffer.rx = 0;
	s_reportBuffer.ry = 0;
	s_reportBuffer.rz = 0;

	s_settingsReportBuffer.reportId = REPORT_GET_SETTINGS;
	s_settingsReportBuffer.settings.oneDeadZoneRaw = 0;
	s_settingsReportBuffer.settings.zeroDeadZoneRaw = 0;

	s_selectedAxisReportBuffer.reportId = REPORT_GET_SELECTED_AXIS;
	s_selectedAxisReportBuffer.axis = 0;

	s_selectedProfileReportBuffer.reportId = REPORT_GET_SELECTED_PROFILE;
	s_selectedProfileReportBuffer.profile = 0;

	s_callibrationReportBuffer.reportId = REPORT_GET_CALLIBRATION;
	s_callibrationReportBuffer.callibration.minusOneVariation = 0;
	s_callibrationReportBuffer.callibration.plusOneVariation = 0;
	s_callibrationReportBuffer.callibration.zeroValue = 0;

	s_rawAxisReportBuffer.reportId = REPORT_GET_RAW_AXIS;
	s_rawAxisReportBuffer.value = 0;
}
void checkProfileChange()
{
	if (buttonIsPressed())
	{
		uint16_t buttons = inputReadRawButtons();
		if (s_profileChange == PROFILECHANGE_ALLOWED)
		{
			if (buttons > 0) // any button must not be pressed while trying to change profile
			{
				s_profileChange = PROFILECHANGE_LOCKED;
			}
			else
			{
				s_profileChange = PROFILECHANGE_ALREADYPRESSED;
				ledOff();
			}
		}
		else if (s_profileChange == PROFILECHANGE_ALREADYPRESSED)
		{
			if (buttons > 0)
			{
				s_profileChange = PROFILECHANGE_BEFORE;
				TCNT1 = 0; // zero timer
				TIFR1 |= (1<<OCF1A);
				OCR1A = PROFILECHANGE_TIMER_BEFORE_COUNT;
				for (uint8_t i = 0; i < 16; ++i)
				{
					if (buttons & (1<<i))
					{
						profilesLoadProfile(i);
						s_profileChangeCounter = (i + 1) * 2;
						break;
					}
				}
			}
		}
	}
	else if (s_profileChange != PROFILECHANGE_PROGRESS && s_profileChange != PROFILECHANGE_BEFORE && s_profileChange != PROFILECHANGE_AFTER)
	{
		s_profileChange = PROFILECHANGE_ALLOWED;
		ledOn();
	}

	if (s_profileChange == PROFILECHANGE_PROGRESS)
	{
		if (TIFR1 & (1<<OCF1A))
		{
			TIFR1 |= (1<<OCF1A);

			ledToggle();
			--s_profileChangeCounter;
			if (s_profileChangeCounter == 0)
			{
				OCR1A = PROFILECHANGE_TIMER_AFTER_COUNT;
				s_profileChange = PROFILECHANGE_AFTER;
			}
		}
	}
	else if (s_profileChange == PROFILECHANGE_BEFORE)
	{
		if (TIFR1 & (1<<OCF1A))
		{
			TIFR1 |= (1<<OCF1A);

			OCR1A = PROFILECHANGE_TIMER_COUNT;
			s_profileChange = PROFILECHANGE_PROGRESS;
		}
	}
	else if (s_profileChange == PROFILECHANGE_AFTER)
	{
		if (TIFR1 & (1<<OCF1A))
		{
			TIFR1 |= (1<<OCF1A);

			s_profileChange = PROFILECHANGE_LOCKED;
			ledOn();
		}
	}
}

