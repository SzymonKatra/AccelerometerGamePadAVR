#ifndef DESCRIPTOR_H_
#define DESCRIPTOR_H_

#include <avr/pgmspace.h>
#include "vusb/usbdrv.h"

PROGMEM const char usbHidReportDescriptor[USB_CFG_HID_REPORT_DESCRIPTOR_LENGTH] =
{
	0x05, 0x01, // USAGE_PAGE (Generic Desktop)
	0x09, 0x05, // USAGE (Game Pad)
	0xA1, 0x01, // COLLECTION (Application)

	0xA1, 0x00, //   COLLECTION (Physical)
	0x85, 0x01, //     REPORT_ID (1)
	0x05, 0x09, //     USAGE_PAGE (Button)
	0x19, 0x01, //     USAGE_MINIMUM (Button 1)
	0x29, 0x0C, //     USAGE_MAXIMUM (Button 12)
	0x15, 0x00, //     LOGICAL_MINIMUM (0)
	0x25, 0x01, //     LOGICAL_MAXIMUM (1)
	0x95, 0x0C, //     REPORT_COUNT (12)
	0x75, 0x01, //     REPORT_SIZE (1)
	0x81, 0x02, //     INPUT (Data,Var,Abs)
	0x05, 0x01, //     USAGE_PAGE (Generic Desktop)
	0x09, 0x39, //     USAGE (Hat switch)
	0x15, 0x01, //     LOGICAL_MINIMUM (1)
	0x25, 0x08, //     LOGICAL_MAXIMUM (8)
	0x35, 0x00, //     PHYSICAL_MINIMUM (0)
	0x46, 0x3B, 0x01,//PHYSICAL_MAXIMUM (315)
	0x95, 0x01, //     REPORT_COUNT (1)
	0x75, 0x04, //     REPORT_SIZE (4)
	0x81, 0x02, //     INPUT (Data,Var,Abs)
	0x05, 0x01, //     USAGE_PAGE (Generic Desktop)
	0x09, 0x30, //     USAGE (X)
	0x09, 0x31, //     USAGE (Y)
	0x09, 0x32, //     USAGE (Z)
	0x09, 0x33, //     USAGE (Rx)
	0x09, 0x34, //     USAGE (Ry)
	0x09, 0x35, //     USAGE (Rz)
	0x15, 0x81, //     LOGICAL_MINIMUM (-127)
	0x25, 0x7F, //     LOGICAL_MAXIMUM (127)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x06, //     REPORT_COUNT (6)
	0x81, 0x02, //     INPUT (Data, Var, Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x0A, //     REPORT_ID (10)
	0x15, 0x00, //     LOGICAL_MINIMUM (0)
	0x26, 0xFF, 0x00,//LOGICAL_MAXIMUM (255)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x01, //     REPORT_COUNT (1)
	0x91, 0x02, //     OUTPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x0B, //     REPORT_ID (11)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x01, //     REPORT_COUNT (1)
	0x81, 0x02, //     INPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x0C, //     REPORT_ID (12)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x01, //     REPORT_COUNT (1)
	0x91, 0x02, //     OUTPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x0D, //     REPORT_ID (13)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x01, //     REPORT_COUNT (1)
	0x81, 0x02, //     INPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x12, //     REPORT_ID (18)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x01, //     REPORT_COUNT (1)
	0x91, 0x02, //     OUTPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x13, //     REPORT_ID (19)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x01, //     REPORT_COUNT (1)
	0x91, 0x02, //     OUTPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x14, //     REPORT_ID (20)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x04, //     REPORT_COUNT (4)
	0x91, 0x02, //     OUTPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x15, //     REPORT_ID (21)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x04, //     REPORT_COUNT (4)
	0x81, 0x02, //     INPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x16, //     REPORT_ID (22)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x06, //     REPORT_COUNT (6)
	0x91, 0x02, //     OUTPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x17, //     REPORT_ID (23)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x06, //     REPORT_COUNT (6)
	0x81, 0x02, //     INPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xA1, 0x01, //   COLLECTION (Application)
	0x09, 0x00, //     USAGE (Undefined)
	0x85, 0x32, //     REPORT_ID (50)
	0x75, 0x08, //     REPORT_SIZE (8)
	0x95, 0x02, //     REPORT_COUNT (2)
	0x81, 0x02, //     INPUT (Data,Var,Abs)
	0xC0,       //   END_COLLECTION

	0xC0,       // END_COLLECTION
};

#endif /* DESCRIPTOR_H_ */
