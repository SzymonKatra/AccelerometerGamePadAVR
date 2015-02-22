#include "usart.h"
#include <avr/io.h>


void usart_init(uint16_t ubrr)
{
	//Set baud rate
	UBRR0H = (unsigned char)(ubrr>>8);
	UBRR0L = (unsigned char)ubrr;
	//Enable receiver and transmitter
	UCSR0B = (1<<RXEN0)|(1<<TXEN0);
	//Set frame format: 8data, 1stop bit
	UCSR0C = (3<<UCSZ00);
}

void usart_send_uint8(uint8_t data)
{
	// Wait for empty transmit buffer
	while ( !( UCSR0A & (1<<UDRE0)) )
	;
	// Put data into buffer, sends the data
	UDR0 = data;
}

void usart_send_string(char* data)
{
    while(*data) usart_send_uint8(*data++);
}
//void usart_send_string_len(char* data, uint8_t len)
//{
//	for (uint8_t i = 0; i < len; i++) usart_send_uint8(*data++);
//}

uint8_t usart_data_available(void)
{
	return (UCSR0A & (1<<RXC0));
}
uint8_t usart_receive_uint8(void)
{
	// Wait for data to be received
	while ( !(UCSR0A & (1<<RXC0)));

	// Get and return received data from buffer
	return UDR0;
}
