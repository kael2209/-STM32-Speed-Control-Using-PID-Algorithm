#include "stdint.h"

#define INA219_ADDR 0x40 // Ð?a ch? I2C m?c d?nh c?a INA219
#define INA219_REG_CONFIG 0x00
#define INA219_REG_SHUNT_VOLTAGE 0x01
#define INA219_REG_BUS_VOLTAGE 0x02
#define INA219_REG_POWER 0x03
#define INA219_REG_CURRENT 0x04
#define INA219_REG_CALIBRATION 0x05

void ina219_init(void);
void read_ina219(float *current_mA);
