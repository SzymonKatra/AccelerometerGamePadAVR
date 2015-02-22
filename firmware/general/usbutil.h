#ifndef USBUTILS_H_
#define USBUTILS_H_

#include <stdint.h>
#include "../vusb/usbdrv.h"

#define USBUTIL_WRITE_BUFFERSIZE 16

void usbutilSetLargeInterrupt(void* data, uint8_t len);
uint8_t usbutilProcessLargeInterrupt(void);

void usbutilStartWriting(uint8_t len);
//defined in main.c
extern void usbutilWriteCompleted(uint8_t* buffer, uint8_t len);

#endif /* USBUTILS_H_ */
