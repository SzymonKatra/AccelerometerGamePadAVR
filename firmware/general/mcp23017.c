#include "mcp23017.h"

void mcp23017Init(void)
{
	i2c_init();
}

void mcp23017WriteUint8(uint8_t reg, uint8_t value)
{
	i2c_start_wait(MCP23017_BUILD_ADDRESS(MCP23017_WRITE));
	i2c_write(reg);
	i2c_write(value);
	i2c_stop();
}
uint8_t mcp23017ReadUint8(uint8_t reg)
{
	uint8_t result;

	i2c_start_wait(MCP23017_BUILD_ADDRESS(MCP23017_WRITE));
	i2c_write(reg);
	i2c_rep_start(MCP23017_BUILD_ADDRESS(MCP23017_READ));
	result = i2c_readNak();
	i2c_stop();

	return result;
}
uint16_t mcp23017ReadUint16(uint8_t reg)
{
	uint16_t result;

	i2c_start_wait(MCP23017_BUILD_ADDRESS(MCP23017_WRITE));
	i2c_write(reg);
	i2c_rep_start(MCP23017_BUILD_ADDRESS(MCP23017_READ));
	result = i2c_readAck();
	result |= (i2c_readNak() << 8);
	i2c_stop();

	return result;
}
