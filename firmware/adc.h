/*
 * adcutil.h
 *
 *  Created on: 08-11-2014
 *      Author: Szymon
 */

#ifndef ADCUTIL_H_
#define ADCUTIL_H_

#include <avr/io.h>

void adcInit(void);

void adcStartReadAxis(uint8_t axis);
uint16_t adcRead(void);
uint16_t adcReadAxis(uint8_t axis);

#endif /* ADCUTIL_H_ */
