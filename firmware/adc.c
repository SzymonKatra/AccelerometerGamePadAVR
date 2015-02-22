#include "adc.h"

void adcInit(void)
{
	ADCSRA |= (1<<ADEN) | (1<<ADPS2) | (1<<ADPS1) | (1<<ADPS0);
	//ADMUX |= (1<<REFS0);
}

void adcStartReadAxis(uint8_t axis)
{
	ADMUX &= (0xFF<<5); // zero channel
	ADMUX |= axis; // axis number is also ADC MUX. see common.h file

	ADCSRA |= (1<<ADSC);
}
uint16_t adcRead(void)
{
	while (ADCSRA & (1<<ADSC));
	return ADC;
}

uint16_t adcReadAxis(uint8_t axis)
{
	adcStartReadAxis(axis);
	return adcRead();
}
