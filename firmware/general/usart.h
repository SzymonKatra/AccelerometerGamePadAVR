#ifndef USART_H_INCLUDED
#define USART_H_INCLUDED

#define UBRR_COMPUTE(fosc, baud) ((fosc+baud*8UL) / (16UL*baud)-1)

#include <stdio.h>

void usart_init(uint16_t ubrr);

void usart_send_uint8(uint8_t data);
void usart_send_string(char* data);
//void usart_send_string_len(char* data, uint8_t len);

uint8_t usart_data_available(void);
uint8_t usart_receive_uint8(void);

#endif // USART_H_INCLUDED
