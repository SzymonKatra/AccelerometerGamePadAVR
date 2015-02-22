#include "usbutil.h"

static void* s_dataPtr = 0;
static uint8_t s_remainingBytes = 0;

void usbutilSetLargeInterrupt(void* data, uint8_t len)
{
	s_dataPtr = data;
	s_remainingBytes = len;
}
uint8_t usbutilProcessLargeInterrupt(void)
{
	if (s_remainingBytes > 0 && usbInterruptIsReady())
	{
		uint8_t sent = 8;
		if(s_remainingBytes < 8) sent = s_remainingBytes;

		usbSetInterrupt(s_dataPtr, sent);

		s_dataPtr += sent; // move pointer to next chunk of data
		s_remainingBytes -= sent;
	}

	return s_remainingBytes;
}

static uint8_t s_writeBuffer[USBUTIL_WRITE_BUFFERSIZE];
static uint8_t s_writeCurrent, s_writeRemaining;

void usbutilStartWriting(uint8_t len)
{
	s_writeCurrent = 0;
	s_writeRemaining = len;
	if (s_writeRemaining > USBUTIL_WRITE_BUFFERSIZE) s_writeRemaining = USBUTIL_WRITE_BUFFERSIZE;
}
uint8_t usbFunctionWrite(uint8_t *data, uint8_t len)
{
	for (uint8_t i = 0; i < len; ++i)
	{
		s_writeBuffer[s_writeCurrent++] = data[i];
	}

	s_writeRemaining -= len;

	if (s_writeRemaining == 0)
	{
		usbutilWriteCompleted((uint8_t*)&s_writeBuffer, s_writeCurrent);
		return 1;
	}

	return 0;
}
