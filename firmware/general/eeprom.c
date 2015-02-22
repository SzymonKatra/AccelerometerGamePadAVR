#include "eeprom.h"

void eepromWriteUint8(uint16_t address, uint8_t value)
{
	while (EECR & (1<<EEPE));

	EEAR = address;
	EEDR = value;

	EECR |= (1<<EEMPE);
	EECR |= (1<<EEPE);
}
void eepromWriteUint16(uint16_t address, uint16_t value)
{
	eepromWriteUint8(address + 1, (uint8_t)(value >> 8));
	eepromWriteUint8(address, (uint8_t)(value));
}
void eepromWriteData(uint16_t address, const void* data, uint16_t length)
{
	const uint8_t* ptr = (uint8_t*)data;
	while (length > 0)
	{
		eepromWriteUint8(address, *ptr);
		++address;
		++ptr;
		--length;
	}
}

uint8_t eepromReadUint8(uint16_t address)
{
	while (EECR & (1<<EEPE));

	EEAR = address;

	EECR |= (1<<EERE);

	return EEDR;
}
uint16_t eepromReadUint16(uint16_t address)
{
	uint16_t result = 0;

	result |= eepromReadUint8(address);
	result |= eepromReadUint8(address + 1) << 8;

	return result;
}
void eepromReadData(uint16_t address, void* data, uint16_t length)
{
	uint8_t* ptr = (uint8_t*)data;
	while (length > 0)
	{
		*ptr = eepromReadUint8(address);
		++address;
		++ptr;
		--length;
	}
}
