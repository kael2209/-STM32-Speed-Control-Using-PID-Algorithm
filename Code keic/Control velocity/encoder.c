#include "stm32f10x.h"              
#include "encoder.h"

void encoder_init(void)
{
	// TIMER 2 CHANNEL 1 2:  PA0 PA1
	RCC->APB2ENR|=(1<<2);
	GPIOA->CRL=0;
	GPIOA->CRL|=(8<<0)|(8<<4);
	GPIOA->ODR|=(1<<0)|(1<<1);
	
	RCC->APB1ENR|=(1<<0);
	TIM2->PSC=0;
	TIM2->ARR = 0xFFFE;
	TIM2->SMCR|=(3<<0); // Encoder mode 3
	
	TIM2->CCMR1|=(1<<0)|(1<<8);
	TIM2->CCMR1&=~(1<<2)&~(1<<10);		// No prescale
	TIM2->CCMR1&=~(1<<4)&~(1<<12);		// No filter
	
	TIM2->CCER|=(1<<0)|(1<<4);				// Capture enabled
	TIM2->CCER|=(1<<1)|(1<<5);				// Falling edge
	
	TIM2->DIER|=(1<<9)|(1<<10);  // DMA
	
}
void encoder_enable(void)
{
	TIM2->CR1|=(1<<0);
}

