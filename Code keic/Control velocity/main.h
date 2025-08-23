/*
PWM:			  TIM2 - CH3 - PB10
ENCODER:		TIM4 - CH1&2 - PB6&7
VISUAL:			USART2 - PA2&3 - 115200

CAL.PARA:		TIM3



*/
#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>
#include <math.h>
#include <stdint.h>

#include "stm32f10x.h"        
#include "PWM.h"
#include "systick_time.h"
#include "encoder.h"
#include "dma.h"
#include "usart_lib.h"
#include "timer3.h"
#include "PID.h"
#include "direction.h"

#define PI 3.14