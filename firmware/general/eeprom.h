#ifndef EEPROM_H_INCLUDED
#define EEPROM_H_INCLUDED

#include <avr/io.h>

// MULTI BYTE TYPES ARE STORED IN LITTLE-ENDIAN

void eepromWriteUint8(uint16_t address, uint8_t value);
void eepromWriteUint16(uint16_t address, uint16_t value);
void eepromWriteData(uint16_t address, const void* data, uint16_t length);

uint8_t eepromReadUint8(uint16_t address);
uint16_t eepromReadUint16(uint16_t address);
void eepromReadData(uint16_t address, void* data, uint16_t length);

#endif /* EEPROM_H_ */
