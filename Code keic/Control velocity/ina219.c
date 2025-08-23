#include "stm32f10x.h"  
#include "ina219.h"
#include "i2c.h"

//void ina219_init(void)
//{
//    // C?u hình thanh ghi CONFIG:
//    // - Bus voltage range: 32V
//    // - Shunt voltage range: ±320mV
//    // - ADC resolution: 12-bit
//    // - Mode: Shunt and Bus, continuous
//    uint16_t config = (0x00 << 13) | // BRNG = 0 (32V)
//                      (0x03 << 11) | // PG = 3 (±320mV)
//                      (0x03 << 7) |  // BADC = 3 (12-bit)
//                      (0x03 << 3) |  // SADC = 3 (12-bit)
//                      (0x07);        // MODE = 7 (Shunt and Bus, continuous)
//    i2c_writereg16(INA219_ADDR, INA219_REG_CONFIG, config);

//    // Hi?u ch?nh cho shunt resistor 0.1O và dòng t?i da 3A
//    // Current_LSB = 3A / 2^15 = 0.0001 A/bit (100 µA/bit)
//    // Cal = trunc(0.04096 / (Current_LSB * R_shunt)) = 4096
//    i2c_writereg16(INA219_ADDR, INA219_REG_CALIBRATION, 4096);
//}
//void read_ina219(float *current_mA)
//{
//    // Ð?c thanh ghi CURRENT (0x04)
//    int16_t current_raw = i2c_readreg16(INA219_ADDR, INA219_REG_CURRENT);
//    *current_mA = (float)current_raw * 0.1; // Current_LSB = 100 µA/bit
//}
void read_ina219(float *current_mA)
{
    int16_t current_raw = i2c_readreg16(INA219_ADDR, INA219_REG_CURRENT);
    *current_mA = (float)current_raw * 0.1f; // Current_LSB = 100 µA/bit
}
void ina219_init(void)
{
    uint16_t config = (0x00 << 13) | // BRNG = 0 (32V)
                      (0x03 << 11) | // PG = 3 (±320mV)
                      (0x07 << 7)  | // BADC = 7 (12-bit + 128 samples)
                      (0x07 << 3)  | // SADC = 7 (12-bit + 128 samples)
                      (0x07);        // MODE = 7 (Shunt and Bus, continuous)

    i2c_writereg16(INA219_ADDR, INA219_REG_CONFIG, config);

    // Calibration: 0.1O shunt, max 2A ? Current_LSB = 0.0001 A
    i2c_writereg16(INA219_ADDR, INA219_REG_CALIBRATION, 4096);
}


