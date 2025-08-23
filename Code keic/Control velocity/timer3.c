#include "stm32f10x.h"             
#include "timer3.h"

void tim3_init(void)
{
	RCC->APB1ENR|=(1<<1);
	TIM3->PSC =11;
	TIM3->ARR =59999;	// 10MS
	TIM3->EGR|=(1<<0);
	TIM3->DIER|=(1<<0);
	NVIC->ISER[0]|=(1<<29);
}
void tim3_enable(void)
{
	TIM3->CR1|=(1<<0);
}

