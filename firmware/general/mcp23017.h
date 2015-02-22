#ifndef MCP23017_H_
#define MCP23017_H_

#include "i2cmaster.h"
#include "usart.h"

#define MCP23017_SELECTED_ADDRESS 0x0

#define MCP23017_WRITE 0
#define MCP23017_READ 1
#define MCP23017_BASE_ADDRESS 0x40

#define MCP23017_BUILD_ADDRESS(rw) (MCP23017_BASE_ADDRESS | MCP23017_SELECTED_ADDRESS | rw)

#define MCP23017_IODIRA 0x00
#define MCP23017_IODIRB 0x01
#define MCP23017_GPIOA 0x12
#define MCP23017_GPIOB 0x13
#define MCP23017_GPPUA 0x0C
#define MCP23017_GPPUB 0x0D
#define MCP23017_IPOLA 0x02
#define MCP23017_IPOLB 0x03


// fake functions to save flash and runtime stack in memory

#define mcp23017WriteDirectionA(value) mcp23017WriteUint8(MCP23017_IODIRA, value)
#define mcp23017WriteDirectionB(value) mcp23017WriteUint8(MCP23017_IODIRB, value)
#define mcp23017ReadDirectionA() mcp23017ReadUint8(MCP23017_IODIRA)
#define mcp23017ReadDirectionB() mcp23017ReadUint8(MCP23017_IODIRB)

#define mcp23017WritePullUpA(value) mcp23017WriteUint8(MCP23017_GPPUA, value)
#define mcp23017WritePullUpB(value) mcp23017WriteUint8(MCP23017_GPPUB, value)
#define mcp23017ReadPullUpA() mcp23017ReadUint8(MCP23017_GPPUA)
#define mcp23017ReadPullUpB() mcp23017ReadUint8(MCP23017_GPPUB)

#define mcp23017WritePortA(value) mcp23017WriteUint8(MCP23017_GPIOA, value)
#define mcp23017WritePortB(value) mcp23017WriteUint8(MCP23017_GPIOB, value)
#define mcp23017ReadPortA() mcp23017ReadUint8(MCP23017_GPIOA)
#define mcp23017ReadPortB() mcp23017ReadUint8(MCP23017_GPIOB)

// mcp23017 is default in sequential mode that switches increment address after read
// when we set address = GPIOA, after read is GPIOB so we can read port B
#define mcp23017ReadPortsAB() mcp23017ReadUint16(MCP23017_GPIOA)

#define mcp23017WritePolarityA(value) mcp23017WriteUint8(MCP23017_IPOLA, value)
#define mcp23017WritePolarityB(value) mcp23017WriteUint8(MCP23017_IPOLB, value)
#define mcp23017ReadPolarityA() mcp23017ReadUint8(MCP23017_IPOLA)
#define mcp23017ReadPolarityB() mcp23017ReadUint8(MCP23017_IPOLB)

void mcp23017Init(void);

void mcp23017WriteUint8(uint8_t reg, uint8_t value);
uint8_t mcp23017ReadUint8(uint8_t reg);
uint16_t mcp23017ReadUint16(uint8_t reg);

#endif /* MCP23017_H_ */
