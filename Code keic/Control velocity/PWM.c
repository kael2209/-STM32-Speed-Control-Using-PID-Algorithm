#include "stm32f10x.h"               
#include "PWM.h"

void PWM_init(void)
{
	// TIM4 - CH3 - PB8;
	RCC->APB2ENR|=(1<<0)|(1<<3);
	GPIOB->CRH=0;
	GPIOB->CRH|=(9<<0);
	RCC->APB1ENR|=(1<<2);
	TIM4->PSC=1;
	TIM4->ARR=59999;
	TIM4->CCR3 =0;
	TIM4->CCMR2|=(6<<4);
	TIM4->CCER|=(1<<8);
	TIM4->CR1|=(1<<0);
}
